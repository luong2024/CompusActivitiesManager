
$path = "CampusActivitiesManager\CampusActivitiesManager\Pages\LoginPage.xaml"
$lines = Get-Content $path
$startIndex = -1
$endIndex = -1

for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match "<!-- Quick Login for Demo / Testing -->") {
        $startIndex = $i
    }
    if ($startIndex -ne -1 -and $i -gt $startIndex -and $lines[$i] -match "</Border>") {
        $endIndex = $i
        break
    }
}

if ($startIndex -ne -1 -and $endIndex -ne -1) {
    $newLines = $lines[0..($startIndex-1)] + $lines[($endIndex+1)..($lines.Count-1)]
    Set-Content $path $newLines -Encoding UTF8
    Write-Host "Removed Quick Login section."
} else {
    Write-Host "Could not find Quick Login section."
}

