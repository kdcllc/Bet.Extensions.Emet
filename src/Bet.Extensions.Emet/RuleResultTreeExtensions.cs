using System;
using System.Collections.Generic;
using System.Linq;

using RulesEngine.Models;

namespace RulesEngine.Extensions
{
    public static class RuleResultTreeExtensions
    {
        public static List<RuleResultTree> OnSuccess2(this List<RuleResultTree> ruleResultTrees, Action<string, string> onSuccessFunc)
        {
            var successfulRuleResult = ruleResultTrees.FirstOrDefault(ruleResult => ruleResult.IsSuccess);
            if (successfulRuleResult != null)
            {
                var c = successfulRuleResult.Rule.ErrorMessage;
                var eventName = successfulRuleResult.Rule.SuccessEvent ?? successfulRuleResult.Rule.RuleName;
                onSuccessFunc(eventName, c);
            }

            return ruleResultTrees;
        }

        /// <summary>
        /// Calls the Failure Func if all rules failed in the ruleReults.
        /// </summary>
        /// <param name="ruleResultTrees"></param>
        /// <param name="onFailureFunc"></param>
        /// <returns></returns>
        public static List<RuleResultTree> OnFail2(this List<RuleResultTree> ruleResultTrees, Action<string> onFailureFunc)
        {
            var allFailure = ruleResultTrees.All(ruleResult => !ruleResult.IsSuccess);
            if (allFailure)
            {
                var firstFailure = ruleResultTrees.FirstOrDefault(ruleResult => !ruleResult.IsSuccess);
                onFailureFunc(firstFailure.Rule.ErrorMessage);
            }

            return ruleResultTrees;
        }

        public static bool IsFailed(this List<RuleResultTree> ruleResultTrees)
        {
            return ruleResultTrees.All(ruleResult => !ruleResult.IsSuccess);
        }
    }
}
