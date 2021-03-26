# Bet.Extensions.Emet

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.Extensions.Emet/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/j6mq3lcd64axi4av?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-extensions-emet)
[![NuGet](https://img.shields.io/nuget/v/Bet.Extensions.Emet.svg)](https://www.nuget.org/packages?q=Bet.Extensions.Emet)
![Nuget](https://img.shields.io/nuget/dt/Bet.Extensions.Emet)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/kcllc/shield/Bet.Extensions.Emet/latest)](https://f.feedz.io/kdcllc/kcllc/packages/Bet.Extensions.Emet/latest/download)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

> `Emet` stands for truth in Hebrew.

_Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/kcllc/nuget/index.json)._

The backbone of any business application is their complex business rules that applied to data.
If business rules are not implemented with DDD principles it will be a challenge to re-factor some of that code.

This project is designed to provide with DotNetCore application with A Truth Engine for Business Rules.

The backbone of this library is [RulesEngine](`https://github.com/microsoft/RulesEngine/`) designed by Microsoft team.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

https://martinfowler.com/bliki/RulesEngine.html

## Install

```csharp
    dotnet add package Bet.Extensions.Emet
```

## Usage

For complete examples please refer to sample projects:

1. [`Bet.Extensions.Emet.WorkerSample`](src/Bet.Extensions.Emet.WorkerSample/) - `Console DI` application with several workflows.

## The implementation Ideas

- [ ] - the ability to load rules workflows from different sources, local files, azure blob storage, sql and etc.
- [x] - the ability to utilize Dependency Injection.
