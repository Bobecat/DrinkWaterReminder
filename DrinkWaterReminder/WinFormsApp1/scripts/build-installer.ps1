param(
	[string] $Configuration = 'Release',
	[string] $Runtime = 'win-x64',
	[bool] $SelfContained = $true,
	[string] $PublishDir = "$PSScriptRoot\..\publish",
	[string] $InnoSetupCompiler = '"C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe"'
)

Write-Host "Publishing application..."
.
& "$PSScriptRoot\publish.ps1" -Configuration $Configuration -Runtime $Runtime -SelfContained $SelfContained -Output $PublishDir

if ($LASTEXITCODE -ne 0) { throw "Publish failed" }

Write-Host "Building installer with Inno Setup..."

$iss = "$PSScriptRoot\..\installer\setup.iss"

# Replace placeholder publish dir in ISS to actual path
$issTemp = "$PSScriptRoot\..\installer\setup_temp.iss"
(Get-Content $iss) -replace '\{#PUBLISH_DIR\}', ($PublishDir -replace '\\','\\\\') | Set-Content $issTemp -Encoding UTF8

& $InnoSetupCompiler $issTemp

if ($LASTEXITCODE -ne 0) { throw "Inno Setup compilation failed" }

Write-Host "Installer built."
