param(
    [String] $query,
    [String] $skip = 0,
    [String] $take = 1
)
$response = Invoke-RestMethod -Uri "https://api-v2v3search-0.nuget.org/query?q=$query&skip=$skip&take=$take"
#write-host $response.data[0].versions
$lastVersion = "1.0.0"
if ($response.data.Count -eq 1)
{
    # Package already founded :
    $lastVersion = $response.data[0].versions[$response.data[0].versions.Count - 1].version
    write-host "Last version :" $lastVersion

    # We have to increment from last published version :
    $versionSplited = $lastVersion.Split(".")
    write-host "    Major   :" $versionSplited[0]
    write-host "    Minor   :" $versionSplited[1]
    write-host "    Revision:" $versionSplited[2]

    # Calculate new version :
    $lastVersion = $versionSplited[0] + "." + ([int]$versionSplited[1] + 1) + ".0"
}

$versionSplited = $lastVersion.Split(".")
write-host "New version :"
write-host "    Major   :" $versionSplited[0]
write-host "    Minor   :" $versionSplited[1]
write-host "    Revision:" $versionSplited[2]

write-output $lastVersion