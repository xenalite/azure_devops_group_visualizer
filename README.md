# azure_devops_group_visualizer
For Rogan Ferguson's benefit.
A tool to see how AD and AAD groups are nested within each other in Azure DevOps and TFS.
Originally forked from [here](https://github.com/konste/TfsPermissionVisualizer).

We use this tool within the company to quickly report on audit compliant Team projects:
- No direct user memberships.
- Appropriate AD groups assigned to VSTS teams / groups.
- Colour coding of different groups allows us to quickly identify non-compliant teams.
- Duplicate or empty groups appear as collapsed.

Colours:
- White: Compliant user assignment to AD group
- Blue: VSTS group
- Green: Compliant AD group assignment to VSTS group
- Yellow: TFS built-in identity
- Orange: Non-compliant AD group assignment to VSTS group
- Red: Non-compliant direct user assignment to VSTS group

Sample Team Project report:
[![Sample Report](https://github.com/xenalite/azure_devops_group_visualizer/blob/master/sample_diagram.PNG)](https://github.com/xenalite/azure_devops_group_visualizer/blob/master/sample_diagram.PNG)

Usage (TFS):
```
Import-Module 'PATH_TO_CMDLETS_DIR\DevOps.VSTS.Cmdlets.dll' -Force
$tfsProjectCollectionUrl = 'YOUR_TFS_COLLECTION_URL'

$tfsMemberships = Get-IxsVstsUserMemberships -CollectionUrl $tfsProjectCollectionUrl
$tfsMembershipsOutputDir = (New-Item (Join-Path $env:TEMP 'tfs') -ItemType 'directory' -Force).FullName
$tfsMembershipsJsonFile = Join-Path $tfsMembershipsOutputDir 'memberships.json'
$tfsMemberships | ConvertTo-Json -Depth 99  | Set-Content $tfsMembershipsJsonFile
$identitiesToIgnore = @('[Agent Queues]\Team Foundation Valid Users')
Get-IxsVstsUserMembershipsReport -JsonInputFilePath $tfsMembershipsJsonFile -DiagramOutputDir $tfsMembershipsOutputDir -IdentityNamesToCollapse $identitiesToIgnore
```
Output diagrams and JSON file will appear in `%TEMP%\tfs`.

Usage (Azure DevOps):
```
Import-Module 'PATH_TO_CMDLETS_DIR\DevOps.VSTS.Cmdlets.dll' -Force
$vstsAccountUrl = 'YOUR_VSTS_ACCOUNT_URL'
$personalAccessToken = 'YOUR_PAT'
$tenantId = 'YOUR_TENANT_ID'

$vstsMemberships = Get-IxsVstsUserMemberships -CollectionUrl $vstsAccountUrl -AccessToken $personalAccessToken -TenantId $tenantId
$vstsMembershipsOutputDir = (New-Item (Join-Path $env:TEMP 'vsts') -ItemType 'directory' -Force).FullName
$vstsMembershipsJsonFile = Join-Path $vstsMembershipsOutputDir 'memberships.json'
$vstsMemberships | ConvertTo-Json -Depth 99  | Set-Content $vstsMembershipsJsonFile
Get-IxsVstsUserMembershipsReport -JsonInputFilePath $vstsMembershipsJsonFile -DiagramOutputDir $vstsMembershipsOutputDir
```
Output diagrams and JSON file will appear in `%TEMP%\vsts`.