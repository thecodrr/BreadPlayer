Write-Host "Installing Microsoft Store Services SDK..." 
 
$msiPath = "$($env:MOBILECENTER_SOURCE_DIRECTORY)\MicrosoftStoreServicesSDK.msi" 
 
(New-Object Net.WebClient).DownloadFile('https://visualstudiogallery.msdn.microsoft.com/229b7858-2c6a-4073-886e-cbb79e851211/file/206533/6/MicrosoftStoreServicesSDK.msi', $msiPath) 
 
cmd /c start /wait msiexec /i $msiPath /quiet 
 
Write-Host "Installed" -ForegroundColor green 
 
 
Write-Host "Installing Microsoft Advertising SDK..." 
 
$msiPath = "$($env:MOBILECENTER_SOURCE_DIRECTORY)\MicrosoftAdvertisingSDK.msi" 
 
(New-Object Net.WebClient).DownloadFile('https://visualstudiogallery.msdn.microsoft.com/345ed584-cc35-4ff0-bbcf-25b31515ae8d/file/258419/5/MicrosoftAdvertisingSDK.msi', $msiPath) 
 
cmd /c start /wait msiexec /i $msiPath /quiet 
 
Write-Host "Installed" -ForegroundColor green 
