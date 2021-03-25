# Bet.Extensions.Emet

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.Extensions.Emet/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/j6mq3lcd64axi4av?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-extensions-emet)
[![NuGet](https://img.shields.io/nuget/v/Bet.Extensions.Emet.svg)](https://www.nuget.org/packages?q=Bet.Extensions.Emet)
![Nuget](https://img.shields.io/nuget/dt/Bet.Extensions.Emet)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/kcllc/shield/Bet.Extensions.Emet/latest)](https://f.feedz.io/kdcllc/kcllc/packages/Bet.Extensions.Emet/latest/download)

*Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/kcllc/nuget/index.json).*

> `Emet` stands for truth in Hebrew.

The backbone of any business application is their complex business rules that applied to data. 
If business rules are not implemented with DDD principles it will be a challenge to re-factor some of that code.

This project is designed to provide with DotNetCore application with A Truth Engine for Business Rules.

The backbone of this library is [RulesEngine](`https://github.com/microsoft/RulesEngine/`) designed by Microsoft team.

https://martinfowler.com/bliki/RulesEngine.html

## The implementation Ideas

- [ ] - the ability to load rules workflows from different sources, local files, azure blob storage, sql and etc.
- [ ] - the ability to utilize Dependecy Injection