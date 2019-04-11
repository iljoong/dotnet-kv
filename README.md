# .NET Core App with Key Vault

There is a security risk if you put the application settings, especially security key or secret, in the source code or config file. You can avoid security key by using AAD integration but some Azure services, like _Event Hubs_ and _Cosomos DB_, still need security key to access.

This sample .net core app demonstrates how to build secure application with Key Vault in Azure.

- How to store key config in Azure Key Vault
- Sample code to access key/config from Azure Key Vault using MSI
- How to set test and production environment setting

Read [document](./docs.md) for more detail
