# Bet.Extensions.Emet

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.Extensions.Emet/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/j6mq3lcd64axi4av?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-extensions-emet)
[![NuGet](https://img.shields.io/nuget/v/Bet.Extensions.Emet.svg)](https://www.nuget.org/packages?q=Bet.Extensions.Emet)
![Nuget](https://img.shields.io/nuget/dt/Bet.Extensions.Emet)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/kdcllc/shield/Bet.Extensions.Emet/latest)](https://f.feedz.io/kdcllc/kdcllc/packages/Bet.Extensions.Emet/latest/download)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

> `Emet` stands for truth in Hebrew.

_Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/kcllc/nuget/index.json)._

## Summary

The backbone of any business application is their complex business rules that applied to data.

This project is designed to provide with DotNetCore application with A Truth Engine for Business Rules.

The backbone of this library is [RulesEngine](`https://github.com/microsoft/RulesEngine/`) designed by Microsoft team.

[Read more on the design pattern by Martin Fowler](https://martinfowler.com/bliki/RulesEngine.html)
### Features

- [x] [File Loader](src/Bet.Extensions.Emet/)
- [x] [Azure Storage Blob Loader](src/Bet.Extensions.Emet.Azure.Storage/)
- [x] Dependency Injection registration; one workflow file per one specific data loader store.
- [ ] SQLite
- [ ] MS Sql
- [ ] RavenDb

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!



## Install

```csharp
    dotnet add package Bet.Extensions.Emet
```

## Usage

### Sample Project

[`Bet.Extensions.Emet.WorkerSample`](src/Bet.Extensions.Emet.WorkerSample/) - `Console DI` application with several workflows.

![rules engine diagram](/img/block-diagram.png)

### Steps

The follow is an example of including a workflow json file for validating country within provided input.

1. Add Workflow to your project `CountryWorkflow.json`

```json
[
  {
    "WorkflowName": "CountryWorkflow",
    "Rules": [
      {
        "RuleName": "Country",
        "SuccessEvent": "true",
        "ErrorMessage": "Country is not acceptable",
        "ErrorType": "Error",
        "localParams": [
          {
            "Name": "model2",
            "Expression": "\"india,usa,canada,France,China\""
          }
        ],
        "RuleExpressionType": "LambdaExpression",
        "Expression": "ExpressionUtils.CheckContains(input1.country, model2) == true"
      }
    ]
  }
]
```

2. Add DI Registration

```csharp
        public static IServiceCollection AddCountryWorkflow(this IServiceCollection services)
        {
            services.AddTransient<ICountryService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var logger = sp.GetRequiredService<ILogger<CountryService>>();
                        var provider = providers.FirstOrDefault(x => x.Name == "CountryWorkflow");

                        return new CountryService(provider, logger);
                    });

            services.AddEmetProvider(
                    "CountryWorkflow",
                    reSettings: settings =>
                    {
                        settings.CustomTypes = new Type[] { typeof(ExpressionUtils) };
                    })
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = "Data/CountryWorkflow.json";
                    });
            return services;
        }
```

3. Create `CountryService.cs`

```csharp
public class CountryService : ICountryService
    {
        private readonly IEmetProvider _provider;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            IEmetProvider provider,
            ILogger<CountryService> logger)
        {
            _provider = provider ?? throw new System.ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsAcceptableAsync(string countryName, CancellationToken cancellationToken = default)
        {
            var engine = await _provider.RulesEngine.Value;

            var input = new
            {
                country = countryName
            };

            var results = await engine.ExecuteAllRulesAsync(_provider.Name, input);
            return results.FirstOrDefault()?.IsSuccess ?? false;
        }
    }
```

4. Use it with your code

```csharp
   var country = "Israel";
    var isValidCountry = await _countryService.IsAcceptableAsync(country);

    _logger.LogInformation("{country} is acceptable: {isValid}", country, isValidCountry);
```
