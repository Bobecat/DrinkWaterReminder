param(
	[string] $Configuration = 'Release',
	[string] $Runtime = 'win-x64',
	[bool] $SelfContained = $true,
	[string] $Output = "$PSScriptRoot\..\publish"
)

Write-Host "Publishing project..."

$sc = if ($SelfContained) { 'true' } else { 'false' }

dotnet publish ..\WinFormsApp1.csproj -c $Configuration -r $Runtime --self-contained:$sc /p:PublishSingleFile=true /p:PublishTrimmed=false /p:IncludeAllContentForSelfExtract=true -o $Output

if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE" }

Write-Host "Publish output at: $Output"
