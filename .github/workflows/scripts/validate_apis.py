import os
import re
import sys
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent.parent.parent
MDX_FILE_PATH = PROJECT_ROOT / "docs/docs/2-guides/3-supported-apis.mdx"
CODEBASE_DIR = PROJECT_ROOT / "Consul"
MDX_FILE_PATH = str(MDX_FILE_PATH)
CODEBASE_DIR = str(CODEBASE_DIR)

# Manual overrides for highly dynamic routes or known deviations
OVERRIDES = {
    # 1. Handle the health service naming variations
    "v1/agent/health/service/name": "v1/agent/health/service/name/:service_name",
    "v1/agent/health/service/id": "v1/agent/health/service/id/:service_id",
    
    # 2. Handle the numeric indexed dynamic route
    "v1/agent/check/{0}/{1}": True,
    
    # 3. Handle the config kind naming
    "v1/config/{kind}": "v1/config/:Kind",

    # overrides
    "v1/agent/check/pass/:check_id": True,
    "v1/agent/check/warn/:check_id": True,
    "v1/agent/check/fail/:check_id": True,
    "v1/operator/utilization": "v1/operator/usage" 
}

def parse_mdx_table(filepath):
    """Parses the MDX file and returns a list of API endpoints to check."""
    endpoints = []
    
    if not os.path.exists(filepath):
        print(f"Error: Could not find file at {filepath}")
        sys.exit(1)
        
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()
        
    in_table = False
    for line in lines:
        line = line.strip()
        if line.startswith('| Group') or line.startswith('| ---'):
            in_table = True
            continue
            
        if in_table and line.startswith('|'):
            if not line:
                break
                
            cols = [c.strip() for c in line.split('|')]
            if len(cols) >= 5:
                route_cell = cols[3]
                status_cell = cols[4]
                
                route_parts = route_cell.split(' ', 1)
                if len(route_parts) == 2:
                    method, route = route_parts
                    status_symbol = status_cell[0] if status_cell else ""
                    
                    if status_symbol in ['✅', '❌']:
                        endpoints.append({
                            'method': method,
                            'route': route,
                            'status': status_symbol,
                            'raw_line': line
                        })
    return endpoints

def search_codebase_for_route(base_dir, route):
    """
    Searches the C# codebase using regex to catch string formatting/interpolation.
    """
    # Check overrides first
    if route in OVERRIDES:
        override_val = OVERRIDES[route]
        if isinstance(override_val, bool):
            return override_val
        elif isinstance(override_val, str):
            route = override_val # Search for the substituted route instead

    # Convert MDX params (e.g., :uuid) to C# format strings (e.g., {0} or {id})
    # This turns "v1/query/:uuid/execute" into "v1/query/\{[^}]+\}/execute"
    regex_string = re.sub(r':([a-zA-Z0-9_]+)', r'\\{[^}]+\\}', route)
    pattern = re.compile(regex_string)
    
    for root, _, files in os.walk(base_dir):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        content = f.read()
                        # Check regex match first (handles {0})
                        if pattern.search(content):
                            return True
                            
                        # Fallback: exact match in case the string isn't interpolated
                        if route in content:
                            return True
                except Exception:
                    pass
    return False

def normalize_route(route):
    """
    Standardizes routes for comparison by replacing all dynamic parts 
    (:name, {name}, {0}) with a generic placeholder.
    """
    # 1. Handle C# string.Format or interpolated variables: {0}, {name}, {ns.Name}
    route = re.sub(r'\{[a-zA-Z0-9_.]+\}', ':var', route)
    # 2. Handle MDX colon variables: :uuid, :AccessorID
    route = re.sub(r':[a-zA-Z0-9_]+', ':var', route)
    # 3. Clean slashes, generic param tags, and handle casing for robust matching
    return route.strip('/').replace(':param', ':var').lower()

def get_all_implemented_routes(base_dir):
    """Extracts potential API route strings (v1/...) from the codebase."""
    implemented = set()
    # Matches strings like "v1/acl/tokens" or "v1/agent/check/{0}"
    route_pattern = re.compile(r'v1/[a-zA-Z0-9/_.\-{}]+')
    
    for root, _, files in os.walk(base_dir):
        for file in files:
            if file.endswith('.cs'):
                try:
                    with open(os.path.join(root, file), 'r', encoding='utf-8') as f:
                        content = f.read()
                        for match in route_pattern.findall(content):
                            clean = match.strip('/').strip('"').strip("'")
                            # Ignore common false positives
                            if clean.endswith('.cs') or clean.startswith('v1/helpers'):
                                continue
                            implemented.add(clean)
                except Exception: pass
    return implemented

def run_validation():
    print(f"Checking MDX at: {MDX_FILE_PATH}")
    print(f"Parsing {MDX_FILE_PATH}...")
    endpoints = parse_mdx_table(MDX_FILE_PATH)
    
    # Create a set of normalized routes from the documentation for easy comparison
    doc_routes_normalized = {normalize_route(ep['route']) for ep in endpoints}
    
    # Add actual code routes from OVERRIDES to the documented set
    for val in OVERRIDES.values():
        if isinstance(val, str):
            doc_routes_normalized.add(normalize_route(val))

    print(f"Scanning codebase in {CODEBASE_DIR}...\n")
    print("1. Checking for documented endpoints...")
    
    errors = 0
    for ep in endpoints:
        route = ep['route']
        status = ep['status']
        is_found = search_codebase_for_route(CODEBASE_DIR, route)
        
        if status == '✅' and not is_found:
            print(f"[ERROR] Overclaimed: Route '{route}' is marked ✅ but NOT found in codebase.")
            errors += 1
        elif status == '❌' and is_found:
            print(f"[ERROR] Violation: Route '{route}' is marked ❌ but WAS found in codebase.")
            errors += 1

    print("\n2. Checking for undocumented endpoints...")
    implemented_routes = get_all_implemented_routes(CODEBASE_DIR)
    for imp in implemented_routes:
        # Check if we have an explicit override mapping for this code route
        route_to_check = OVERRIDES.get(imp, imp)
        
        # If it maps to True, it's a known dynamic route we are ignoring here
        if route_to_check is True:
            continue
            
        # Normalize the (potentially translated) route
        norm_imp = normalize_route(route_to_check)
        
        # Check if this implementation exists in our documented set
        if norm_imp not in doc_routes_normalized:
            print(f"[ERROR] Undocumented: Route '{imp}' is implemented in code but missing from MDX.")
            errors += 1

    print("\n" + "="*40)
    print("VALIDATION SUMMARY")
    print("="*40)
    print(f"Total documented checked : {len(endpoints)}")
    print(f"Total Errors             : {errors}")
    
    if errors > 0:
        sys.exit(1)
    else:
        print("Success! Documentation matches the codebase logic.")
        sys.exit(0)

if __name__ == "__main__":
    run_validation()