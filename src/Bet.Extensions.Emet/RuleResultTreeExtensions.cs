using System;
using System.Collections.Generic;
using System.Linq;

using RulesEngine.Models;

namespace RulesEngine.Extensions
{
    public static class RuleResultTreeExtensions
    {
        /// <summary>
        /// Creates an entry point on success.
        /// </summary>
        /// <param name="ruleResultTrees"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public static List<RuleResultTree> OnResultSuccess(this List<RuleResultTree> ruleResultTrees, Action<string, string> onSuccess)
        {
            var successfulRuleResult = ruleResultTrees.FirstOrDefault(ruleResult => ruleResult.IsSuccess);
            if (successfulRuleResult != null)
            {
                var eventName = successfulRuleResult.Rule.SuccessEvent ?? successfulRuleResult.Rule.RuleName;
                onSuccess(eventName, successfulRuleResult.Rule.RuleName);
            }

            return ruleResultTrees;
        }

        /// <summary>
        /// Calls the Failure Func if all rules failed in the ruleReults.
        /// </summary>
        /// <param name="ruleResultTrees"></param>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        public static List<RuleResultTree> OnResultFail(this List<RuleResultTree> ruleResultTrees, Action<string> onFailure)
        {
            var allFailure = ruleResultTrees.All(ruleResult => !ruleResult.IsSuccess);
            if (allFailure)
            {
                var firstFailure = ruleResultTrees.FirstOrDefault(ruleResult => !ruleResult.IsSuccess);
                onFailure(firstFailure.Rule.ErrorMessage);
            }

            return ruleResultTrees;
        }

        public static bool IsFailed(this List<RuleResultTree> ruleResultTrees)
        {
            return ruleResultTrees.All(ruleResult => !ruleResult.IsSuccess);
        }

        public static bool IsSuccess(this List<RuleResultTree> ruleResultTrees)
        {
            return ruleResultTrees.Any(ruleResult => ruleResult.IsSuccess);
        }
    }
}
