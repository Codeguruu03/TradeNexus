# Test API Endpoints with Basic Authentication
Write-Host "Testing TradeNexus API Endpoints" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

$baseUrl = "http://localhost:5000"

# Create Basic Auth header
function Get-BasicAuthHeader {
    param(
        [string]$Username,
        [string]$Password
    )
    $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("${Username}:${Password}"))
    return @{
        Authorization = "Basic $base64AuthInfo"
    }
}

# Test 1: Get Clients as Admin
Write-Host "Test 1: GET /api/clients (Admin)" -ForegroundColor Cyan
try {
    $headers = Get-BasicAuthHeader -Username "admin" -Password "admin123"
    $response = Invoke-RestMethod -Uri "$baseUrl/api/clients" -Method Get -Headers $headers -ErrorAction Stop
    Write-Host "✓ Success: Retrieved $($response.Count) clients" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Get SubBrokers as Trader
Write-Host "Test 2: GET /api/subbrokers (Trader)" -ForegroundColor Cyan
try {
    $headers = Get-BasicAuthHeader -Username "trader" -Password "trader123"
    $response = Invoke-RestMethod -Uri "$baseUrl/api/subbrokers" -Method Get -Headers $headers -ErrorAction Stop
    Write-Host "✓ Success: Retrieved $($response.Count) sub-brokers" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Get Trades as Risk
Write-Host "Test 3: GET /api/trades (Risk)" -ForegroundColor Cyan
try {
    $headers = Get-BasicAuthHeader -Username "risk" -Password "risk123"
    $response = Invoke-RestMethod -Uri "$baseUrl/api/trades" -Method Get -Headers $headers -ErrorAction Stop
    Write-Host "✓ Success: Retrieved $($response.Count) trades" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Get Trades without Authentication (should fail)
Write-Host "Test 4: GET /api/trades (No Auth - should fail)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/trades" -Method Get -ErrorAction Stop
    Write-Host "✗ Unexpected: Request succeeded without auth" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "✓ Success: Correctly returned 401 Unauthorized" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# Test 5: Try to create client as Trader (should fail - requires Admin)
Write-Host "Test 5: POST /api/clients (Trader - should fail)" -ForegroundColor Cyan
try {
    $headers = Get-BasicAuthHeader -Username "trader" -Password "trader123"
    $newClient = @{
        clientName = "Test Client"
        subBrokerId = 1
        tradingLimit = 100000
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/api/clients" -Method Post -Headers $headers -Body $newClient -ContentType "application/json" -ErrorAction Stop
    Write-Host "✗ Unexpected: Trader was able to create client" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 403) {
        Write-Host "✓ Success: Correctly returned 403 Forbidden" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed with unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

Write-Host "=================================" -ForegroundColor Green
Write-Host "API Testing Complete" -ForegroundColor Green
