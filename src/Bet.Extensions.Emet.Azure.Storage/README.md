# Bet.Extensions.Emet.Azure.Storage

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.Extensions.Emet.Azure.Storage/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/j6mq3lcd64axi4av?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-extensions-emet)
[![NuGet](https://img.shields.io/nuget/v/Bet.Extensions.Emet.Azure.Storage.svg)](https://www.nuget.org/packages?q=Bet.Extensions.Emet.Azure.Storage)
![Nuget](https://img.shields.io/nuget/dt/Bet.Extensions.Emet.Azure.Storage)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/kdcllc/shield/Bet.Extensions.Emet.Azure.Storage/latest)](https://f.feedz.io/kdcllc/kdcllc/packages/Bet.Extensions.Emet.Azure.Storage/latest/download)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

> `Emet` stands for truth in Hebrew.

_Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/kcllc/nuget/index.json)._

## Summary

The storage provider for Azure Storage Blob for [RulesEngine](`https://github.com/microsoft/RulesEngine/`) library.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Install

```csharp
    dotnet add package Bet.Extensions.Emet.Azure.Storage
```

## Usage

The default authentication method for Azure Storage Blob is token.
In order to make it work on local machine please make sure the following steps are completed:

1. [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed and you are login with Azure Account that have `Storage Blob Data Contributor` associated with your development account.

2. `dotnet new tool-manifest` and install [AppAuthentication](https://github.com/kdcllc/AppAuthentication) - `dotnet tool install appauthentication`

3. Run in the command prompt `appauthentication run --local --verbose:debug` in the root of your project. Make sure that Visual Studio or VSCode is restarted.

```json
{
  "profiles": {
    "Bet.Extensions.Emet.Workersample": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "MSI_ENDPOINT": "http://localhost:5050/oauth2/token",
        "MSI_SECRET": "38597b24-96c9-4b5a-b5c5-405469523460"
      },
      "dotnetRunMessages": "true"
    },
    "Docker": {
      "commandName": "Docker"
    }
  }
}
```

4. Add Workflow to Storage Blob -> Container `workflow`

```json
[
  {
    "WorkflowName": "RetirementEligibilityWorkflow",
    "Rules": [
      {
        "RuleName": "IsEligible",
        "SuccessEvent": "",
        "ErrorMessage": "Employee is not eligible.",
        "ErrorType": "Error",
        "RuleExpressionType": "LambdaExpression",
        "Expression": "input1.LengthOfServiceInDays >= 190 OR input1.IsOverridden == true"
      }
    ]
  }
]
```

5. Add the DI Registration

```csharp
public static IServiceCollection AddRetirementEligibilityWorkflow(this IServiceCollection services)
{
    services.AddTransient<IRetirementService>(
            sp =>
            {
                var providers = sp.GetServices<IEmetProvider>().ToList();
                var provider = providers.FirstOrDefault(x => x.Name ==  Workflows.AzureRetirementEligibilityWorkflow);
                if (provider == null)
                {
                    throw new ArgumentNullException( Workflows.AzureRetirementEligibilityWorkflow, $"IEmetProvider wasn't register");
                }

                return new RetirementService(provider);
            });

    var builder = services.AddEmetProvider( Workflows.AzureRetirementEligibilityWorkflow).AddAzureStorageLoader(
            workflowName,
            configOptions: (options, config) =>
            {
                options.BlobServiceUri = new Uri(config["SharedBlobServiceUri"]);
            });
    return services;
}
```

6. `appsetting.json`

```json
  "SharedBlobServiceUri": "https://betstorage.blob.core.windows.net/",

  "AzureCountryWorkflow": {
    "BlobServiceUri": "",
    "ContainerName": "workflows",
    "FileNames": [
        "CountryWorkflow.json"
    ]
  },

  "AzureRetirementEligibilityWorkflow": {
    "BlobServiceUri": "",
    "ContainerName": "workflows",
    "FileNames": [
        "RetirementEligibilityWorkflow.json"
    ]
  },

  "AzureDiscountWorkflow": {
    "BlobServiceUri": "",
    "ContainerName": "workflows",
    "FileNames": [
        "DiscountWorkflow.json"
    ]
  },

```

## References

- https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
- https://devblogs.microsoft.com/azure-sdk/best-practices-for-using-azure-sdk-with-asp-net-core/
- https://github.com/Azure/azure-sdk-for-net/tree/master/samples/CloudClipboard
