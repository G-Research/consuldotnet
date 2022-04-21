$workflow = ".github/workflows/ci.yml"
$owner = "hashicorp"
$repo = "consul"

# Suppress telemetry reminder
Set-GitHubConfiguration -SuppressTelemetryReminder

# Use GITHUB_TOKEN if provided
if ($Env:GITHUB_TOKEN -ne $null)
{
  $token = ($Env:GITHUB_TOKEN | ConvertTo-SecureString -AsPlainText -Force)
  $cred = New-Object System.Management.Automation.PSCredential "username is ignored", $token
  Set-GitHubAuthentication -Credential $cred -SessionOnly
}

# Fetch all the latest stable Consul releases
$releases = Get-GitHubRelease -OwnerName $owner -RepositoryName $repo | Where-Object { ! $_.PreRelease }

# Find the latest version for each minor release
$latest = @{}
foreach ($release in $releases)
{
  $version = [version]($release.tag_name.Substring(1))
  $minor = "$($version.Major).$($version.Minor)"
  if (!$latest.ContainsKey($minor) -or $latest[$minor] -lt $version)
  {
    $latest[$minor] = $version
  }
}

# Update the CI workflow with the latest versions
(Get-Content $workflow) -Replace "consul: \[(\d+\.\d+\.\d+(, )?)+\]", "consul: [$($latest.Values | Sort-Object | ForEach-Object { [string]$_ } | Join-String -Separator ", ")]" | Set-Content $workflow
