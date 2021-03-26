# Bet.Extensions.Emet.Workersample

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

> `Emet` stands for truth in Hebrew.

This sample application was started with [Bet.Extensions.Templating](https://github.com/kdcllc/Bet.Extensions.Templating),
it provides with required infrastructure out of the box for enabling DI inside of Console App with `Serilog` logging.

There are three workflows that this sample explores:

1. `IRetirementService` - utilizes typed `Employee` object to provide with business rules around retirement.
2. `IDiscountService` - demonstrates how to use complex business rules for generating a discount value in `%`
3. `ICountryService` - demonstrates utilization of custom utility class to be used inside of the workflow part of the expression.
