$url  = "http://localhost:5269/api/auctions/1/bids"
$body = '{"amount": 46000}'

$scriptBlock = {
    param($url, $body)
    try {
        $r = Invoke-WebRequest -Uri $url -Method Post `
            -Body $body -ContentType "application/json" `
            -Headers @{ "UserId" = "3" }
        "$($r.StatusCode)"
    }
    catch {
        "$($_.Exception.Response.StatusCode.value__)"
    }
}

$jobs = 1..5 | ForEach-Object {
    Start-Job -ScriptBlock $scriptBlock -ArgumentList $url, $body
}


$jobs | Wait-Job | Out-Null
$results = $jobs | Receive-Job
$jobs | Remove-Job

Write-Host "Resultados de los 5 intentos:"
$results | Sort-Object