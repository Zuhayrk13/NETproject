# Starts the API with "dotnet exec" (used instead of "dotnet run" on the company laptop).
# "dotnet exec" skips Properties/launchSettings.json, so we set its two key values here.

# Stop at the first error
$ErrorActionPreference = "Stop"

# Always run from this script's folder (the project folder)
Set-Location $PSScriptRoot

# 1. Compile the latest code
dotnet build
if ($LASTEXITCODE -ne 0) { Write-Host "Build failed - fix the errors above." -ForegroundColor Red; exit 1 }

# 2. Development mode: loads User Secrets (the connection string) and enables OpenAPI
$env:ASPNETCORE_ENVIRONMENT = "Development"

# 3. Listen on port 5158, where the React (Vite) proxy sends /api requests
$env:ASPNETCORE_URLS = "http://localhost:5158"

# 4. Start the compiled app
dotnet exec .\bin\Debug\net10.0\EmployeeManagement.Api.dll
