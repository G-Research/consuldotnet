import os
import re
import sys
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent
MDX_FILE_PATH = PROJECT_ROOT / "docs/docs/2-guides/3-supported-apis.mdx"
CODEBASE_DIR = PROJECT_ROOT / "Consul"
MDX_FILE_PATH = str(MDX_FILE_PATH)
CODEBASE_DIR = str(CODEBASE_DIR)

print(f"Checking MDX at: {MDX_FILE_PATH}")
print(f"Scanning Code at: {CODEBASE_DIR}")

# Manual overrides for highly dynamic routes or known deviations
OVERRIDES = {
    # These are constructed dynamically via string.Format("/v1/agent/check/{0}/{1}")
    "v1/agent/check/pass/:check_id": True,
    "v1/agent/check/warn/:check_id": True,
    "v1/agent/check/fail/:check_id": True,
    
    # Example of redirecting a doc claim to the actual code implementation
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
                    
                    if status_symbol in ['✅', '❌', '🚧', '🛑']:
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

def run_validation():
    print(f"Parsing {MDX_FILE_PATH}...")
    endpoints = parse_mdx_table(MDX_FILE_PATH)
    print(f"Found {len(endpoints)} endpoints to validate.\n")
    
    print(f"Scanning codebase in {CODEBASE_DIR}...\n")
    
    errors = 0
    warnings = 0
    
    for ep in endpoints:
        route = ep['route']
        status = ep['status']
        
        is_found = search_codebase_for_route(CODEBASE_DIR, route)
        
        if status == '✅':
            if not is_found:
                print(f"[ERROR] Overclaimed: Route '{route}' is marked ✅ but NOT found in codebase.")
                errors += 1
        
        elif status == '❌':
            if is_found:
                print(f"[ERROR] Violation: Route '{route}' is marked ❌ but WAS found in codebase.")
                errors += 1
                
        elif status in ['🚧', '🛑']:
            if is_found:
                print(f"[INFO] Implemented: Route '{route}' is marked {status} and was found in codebase.")
                warnings += 1

    print("\n" + "="*40)
    print("VALIDATION SUMMARY")
    print("="*40)
    print(f"Total endpoints checked : {len(endpoints)}")
    print(f"Total Errors            : {errors}")
    print(f"Total Info/Warnings     : {warnings}")
    
    if errors > 0:
        sys.exit(1)
    else:
        print("Success! Documentation matches the codebase logic.")
        sys.exit(0)

if __name__ == "__main__":
    run_validation()