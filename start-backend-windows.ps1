Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "   OMDB Terminal - Infrastructure Setup  " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$defaultKey = "47c4ba47"
$apiKey = Read-Host "Please enter your OMDb API Key (Press Enter to use the default demo key)"

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    $apiKey = $defaultKey
    Write-Host "`n[+] No key provided. Using the built-in demo key." -ForegroundColor DarkGray
}

Write-Host "`n[+] Ensuring .NET User Secrets are initialized..." -ForegroundColor Yellow
dotnet user-secrets init --project "OmdbTerminal\OmdbTerminal.ApiService\OmdbTerminal.ApiService.csproj"

Write-Host "[+] Setting API Key securely in .NET User Secrets..." -ForegroundColor Yellow
dotnet user-secrets set "Omdb:ApiKey" "$apiKey" --project "OmdbTerminal\OmdbTerminal.ApiService\OmdbTerminal.ApiService.csproj"
Write-Host "[+] Key saved successfully." -ForegroundColor Green

Write-Host "`n[+] Booting up .NET Aspire Orchestrator (API + MySQL)..." -ForegroundColor Yellow
Write-Host "    (Keep this window open to keep the backend running)`n" -ForegroundColor DarkGray

dotnet run --project "OmdbTerminal\OmdbTerminal.AppHost\OmdbTerminal.AppHost.csproj" --configuration Release