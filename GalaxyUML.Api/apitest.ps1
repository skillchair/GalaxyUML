param(
    [string]$Port = "5248"
)

Write-Output @"
# ======================================================================
#   █████╗ ██╗   ██╗████████╗ ██████╗ ████████╗███████╗███████╗████████╗
#  ██╔══██╗██║   ██║╚══██╔══╝██╔═══██╗╚══██╔══╝██╔════╝██╔════╝╚══██╔══╝
#  ███████║██║   ██║   ██║   ██║   ██║   ██║   █████╗  ███████╗   ██║
#  ██╔══██║██║   ██║   ██║   ██║   ██║   ██║   ██╔══╝  ╚════██║   ██║
#  ██║  ██║╚██████╔╝   ██║   ╚██████╔╝   ██║   ███████╗███████║   ██║
#  ╚═╝  ╚═╝ ╚═════╝    ╚═╝    ╚═════╝    ╚═╝   ╚══════╝╚══════╝   ╚═╝
#
#  AUTOTEST SKRIPTA ZA TESTIRANJE API ENDPOINTA
#  - Podrazumevani host: https://localhost:$Port
#  - Unesi drugi host ili pritisni Enter za default
# ======================================================================
"@

Write-Output @"
ne radi :(
"@
return

# Pitaj korisnika za host (može samo Enter za default)
$userInput = Read-Host "Unesi host (npr. https://localhost:5000 ili http://localhost:5000)"
$myhost = [string]::IsNullOrWhiteSpace($userInput) ? "https://localhost:$Port" : $userInput
Write-Host "Koristi se host: $myhost" -ForegroundColor Cyan

# Ignoriši self-signed SSL sertifikate
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
[System.Net.ServicePointManager]::SecurityProtocol = `
    [System.Net.SecurityProtocolType]::Tls12 -bor `
    [System.Net.SecurityProtocolType]::Tls13

# Definicija testova
$tests = @(
    @{ name="Health"; method="GET";  url="$myhost/swagger/index.html"; body=$null },
    @{ name="CreateUser"; method="POST"; url="$myhost/api/users"; body='{"firstName":"Ana","lastName":"Test","username":"ana1","email":"ana@test.com","password":"Passw0rd!"}' }
)

foreach ($t in $tests) {
    try {
        if ($t.method -eq "GET") {
            $resp = Invoke-RestMethod -Uri $t.url -Method $t.method
        } else {
            $resp = Invoke-RestMethod -Uri $t.url -Method $t.method -Body $t.body -ContentType "application/json"
        }
        Write-Host "[OK] $($t.name)" -ForegroundColor Green
    }
    catch {
        $code = $_.Exception.Response?.StatusCode.value__ ?? "UNKNOWN"
        Write-Host "[FAIL] $($t.name) -> $code" -ForegroundColor Red
        Write-Host $_.Exception.Message
    }
}

# Vrati SSL callback na default
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = $null
