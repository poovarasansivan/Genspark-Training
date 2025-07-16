# Azure Functions

- Working On Azure Function by create a function that generates a SAS URL for the key vault to perform image Upload and Read from the Blobstorage Container.


## Commands Performed on CLI

1. To Login Azure Account

`az login`

2. Creating a Resource Group.
`az group create --name appdev-training --location eastus`

3. Creating a Storage Account with Resource Group.
`az storage account create --name blobstorageps --location eastus --resource-group appdev-training --sku Standard_LRS`

4. Creating a Function.
`az functionapp create --resource-group appdev-training --consumption-plan-location eastus --name psdotnetfunction --storage-account  blobstorageps --runtime dotnet-isolated --functions-version 4`

5. Configuring the Function
`az functionapp config appsettings set  --name psdotnetfunction --resource-group appdev-training --settings AzureStorageConnectionString="DefaultEndpointsProtocol=https;AccountName=blobstorageps;AccountKey=yTZPVePn4QqPKBednsAqbxUMyOLS+CP/Ix8lEhhr+ZRM6b9kJ2tyQWftcXpXPHBW3PXhrS9WhDxS+ASt+VcEWw==;EndpointSuffix=core.windows.net" ContainerName="filesandimahge" KeyVaultUri="https://blobconnectiong3.vault.azure.net/"`

6. Function List Command
`az functionapp function keys list --resource-group appdev-training --name psdotnetfunction --function-name Function`