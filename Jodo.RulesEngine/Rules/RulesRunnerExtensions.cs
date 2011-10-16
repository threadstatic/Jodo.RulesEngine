using System;

namespace Jodo.Rules
{
    public static class RulesRunnerExtensions
    {
        /// <summary>
        /// Executes all rules for a Type.
        /// Returns void if all rules pass; 
        /// throws a <see cref="InvalidOperationException"/> if any rule fails.
        /// </summary>
        public static void TestRules<TRuleContext, TCandidate>(this IRulesRunner rulesRunner, IRulesProvider rulesProvider, Type typeToGetRulesFor, TCandidate candidate)
         where TRuleContext : IRule<TCandidate>
        {
            var rules = rulesProvider.GetRulesFor<TRuleContext, TCandidate>(typeToGetRulesFor);

            foreach (Func<IRule<TCandidate>> ruleDelegate in rules)
                RunRule(ruleDelegate(), candidate);
        }
      
        /// <summary>
        /// Executes all rules for a Type.
        /// Returns void if all rules pass; 
        /// throws a <see cref="InvalidOperationException"/> if any rule fails.
        /// </summary>
        public static void TestRules<TRuleContext, TCandidate, TDecisionData>(this IRulesRunner rulesRunner, IRulesProvider rulesProvider, Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData)
            where TRuleContext : IRule<TCandidate, TDecisionData>
        {
            var rules = rulesProvider.GetRulesFor<TRuleContext, TCandidate>(typeToGetRulesFor);

            foreach (Func<IRule<TCandidate>> ruleDelegate in rules)
            {
                IRule<TCandidate> rule = ruleDelegate();

                SetDecisionData(rule, decisionData);
                RunRule(rule, candidate);
            }
        }

        private static void SetDecisionData<TCandidate, TDecisionData>(IRule<TCandidate> rule, TDecisionData decisionData)
        {
            IRule<TCandidate, TDecisionData> decisionDataRule = rule as IRule<TCandidate, TDecisionData>;

            if (decisionDataRule != null)
                decisionDataRule.DecisionData = decisionData;
        }

        private static void RunRule<TCandidate>(IRule<TCandidate> rule, TCandidate candidate)
        {
#if DEBUG
            Console.WriteLine(String.Format("Running Rule: {0}", rule));
#endif
            RuleResult ruleResult = rule.IsSatisfiedBy(candidate);

#if DEBUG
            if (ruleResult)
                Console.WriteLine(String.Format("PASSED : {0}.", (String)ruleResult));
            else
                Console.WriteLine(String.Format("FAILED : {0}.", (String)ruleResult));
#endif
            if (!ruleResult)
                throw new InvalidOperationException(ruleResult);
        }
    }
}
