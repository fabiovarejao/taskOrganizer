# Computes adjusted coverage percent from the newest coverage.cobertura.xml
# Excludes files that match: \Migrations\, *.Designer.cs, *.g.cs, *Generated*.cs
# Usage: .\tools\coverage-adjust.ps1

$cov = Get-ChildItem -Path . -Recurse -Filter 'coverage.cobertura.xml' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $cov) {
    Write-Error "No coverage.cobertura.xml found under the repository. Run `dotnet test --collect:\"XPlat Code Coverage\"` first."
    exit 2
}

$text = Get-Content $cov.FullName -Raw
if ($text -match 'line-rate="([0-9.]+)"') { $rawLineRate = [double]$matches[1] } else { $rawLineRate = 0 }
[xml]$x = $text
$classNodes = $x.SelectNodes('//class')
$linesCovered = 0
$linesValid = 0
foreach ($c in $classNodes) {
    $fname = $c.GetAttribute('filename')
    if ($null -eq $fname) { continue }
    if ($fname -match '\\Migrations\\' -or $fname -match '\.Designer\.cs$' -or $fname -match '\.g\.cs$' -or $fname -match 'Generated') {
        # skip migrations / designer / generated files
        continue
    }
    $lines = $c.SelectNodes('.//line')
    if ($null -eq $lines) { continue }
    foreach ($l in $lines) {
        $hits = [int]$l.GetAttribute('hits')
        $linesValid += 1
        if ($hits -gt 0) { $linesCovered += 1 }
    }
}
$adjustedPercent = if ($linesValid -gt 0) { [math]::Round(($linesCovered / $linesValid) * 100, 2) } else { 0 }

Write-Output "Coverage file used: $($cov.FullName)"
Write-Output "Raw reported line-rate: $rawLineRate (i.e. $([math]::Round($rawLineRate * 100,2))%)"
Write-Output "Adjusted coverage (excluding migrations/designer/generated): $adjustedPercent% (lines-covered=$linesCovered lines-valid=$linesValid)"

exit 0
