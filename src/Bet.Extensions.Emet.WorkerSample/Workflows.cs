namespace Bet.Extensions.Emet.WorkerSample
{
    public class Workflows
    {
        // file store workflows
        public const string DiscountWorkflow = nameof(DiscountWorkflow);
        public const string RetirementEligibilityWorkflow = nameof(RetirementEligibilityWorkflow);
        public const string CountryWorkflow = nameof(CountryWorkflow);

        // azure storge blob workflows
        public const string AzureDiscountWorkflow = nameof(AzureDiscountWorkflow);
        public const string AzureRetirementEligibilityWorkflow = nameof(AzureRetirementEligibilityWorkflow);
        public const string AzureCountryWorkflow = nameof(AzureCountryWorkflow);
    }
}
