using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Moq;

using RulesEngine.Models;

using Xunit;

namespace Bet.Extensions.Emet.UnitTest;

public class EmetProviderUnitTests
{
    [Fact]
    public async Task Test_Workflow_With_Emet_Provider()
    {
        var host = new Mock<IHostApplicationLifetime>();

        var store = new Mock<IEmetStore>();

        var workflowName = "inputWorkflow";

        var workflows = new List<Workflow>
        {
            new Workflow
            {
                WorkflowName = workflowName,
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        RuleName = "GiveDiscount10",
                        SuccessEvent = "10",
                        ErrorMessage = "One or more adjust rules failed.",
                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "input1.country == \"canada\" AND input1.loyaltyFactor <= 4"
                    }
                }
            }
        };

        store.Setup(x => x.RetrieveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(workflows.ToArray());

        var provider = new EmetProvider("testProvider", new ReSettings(), store.Object, host.Object);

        var engine = await provider.RulesEngine.Value;

        Assert.NotNull(engine);

        dynamic input1 = new ExpandoObject();
        input1.country = "canada";
        input1.loyaltyFactor = 4;

        var inputs = new dynamic[]
        {
           input1
        };

        var result = await engine.ExecuteAllRulesAsync(workflowName, inputs);
        Assert.Contains(result, c => c.IsSuccess);
    }
}
