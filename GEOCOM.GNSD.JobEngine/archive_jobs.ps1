# This example script archives jobs older that 65 days

$olderThanDays = 65
$createdBefore = (Get-Date).AddDays(-$olderThanDays) # substract days from the current date
.\GEOCOM.GNSD.JobEngine.exe -archive ($createdBefore.ToString()) 