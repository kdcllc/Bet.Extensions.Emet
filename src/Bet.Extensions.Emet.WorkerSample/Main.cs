using System;
using System.Dynamic;
using System.Threading.Tasks;

using Bet.Extensions.Emet.WorkerSample.Models;
using Bet.Extensions.Emet.WorkerSample.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bet.Extensions.Emet.WorkerSample;

public class Main : IMain
{
    private readonly IDiscountService _discountService;
    private readonly IRetirementService _retirementService;
    private readonly ICountryService _countryService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private ILogger<Main> _logger;

    public Main(
        IDiscountService discountService,
        IRetirementService retirementService,
        ICountryService countryService,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        _retirementService = retirementService ?? throw new ArgumentNullException(nameof(retirementService));
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        var employee = new Employee
        {
            LengthOfServiceInDays = 990,
            IsOverridden = false
        };

        var isEligible = await _retirementService.IsEligibleAsync(employee);
        _logger.LogInformation("Employee is eligible for retirement: {isEligible}", isEligible);

        var discountOffered = await _discountService.CalculateDiscountAsync(GetDiscountInput());
        var discountMessage = discountOffered == 0m
            ? "The user is not eligible for any discount."
            : $"Discount offered is {discountOffered * 100}% over MRP.";

        _logger.LogInformation(discountMessage);

        await IsValidCountry("Israel");
        await IsValidCountry("Ukraine");

        return 0;
    }

    private async Task IsValidCountry(string country)
    {
        var isValidCountry = await _countryService.IsAcceptableAsync(country);

        _logger.LogInformation("{country} is acceptable: {isValid}", country, isValidCountry);
    }

    private dynamic[] GetDiscountInput()
    {
        var basicInfo = "{\"name\": \"Dishant\",\"email\": \"dishantmunjal@live.com\",\"creditHistory\": \"good\",\"country\": \"india\",\"loyalityFactor\": 3,\"totalPurchasesToDate\": 10000}";
        var orderInfo = "{\"totalOrders\": 5,\"recurringItems\": 2}";
        var telemetryInfo = "{\"noOfVisitsPerMonth\": 10,\"percentageOfBuyingToVisit\": 15}";

        var converter = new ExpandoObjectConverter();
        dynamic input1 = JsonConvert.DeserializeObject<ExpandoObject>(basicInfo, converter)!;
        dynamic input2 = JsonConvert.DeserializeObject<ExpandoObject>(orderInfo, converter)!;
        dynamic input3 = JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo, converter)!;

        return new dynamic[]
                {
                    input1!,
                    input2!,
                    input3!
                };
    }
}
