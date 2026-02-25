# Simple API Test
$base64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("admin:admin123"))
$headers = @{ Authorization = "Basic $base64" }

Write-Host "Testing API..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

try {
    $clients = Invoke-RestMethod -Uri "http://localhost:5000/api/clients" -Headers $headers
    Write-Host "Success: GET /api/clients" -ForegroundColor Green
    Write-Host "Retrieved: $($clients.Count) clients" -ForegroundColor Cyan
} catch {
    Write-Host "Failed: GET /api/clients" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
