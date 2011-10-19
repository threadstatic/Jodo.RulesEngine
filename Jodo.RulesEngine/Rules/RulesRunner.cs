using System;

namespace Jodo.Rules
{
    public sealed class RulesRunner 
    {
        public static void TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate)
            where TRuleContext : IRule<TCandidate>
        {
            RulesEngine.RulesRunner.TestRules<TRuleContext, TCandidate>(typeToGetRulesFor, candidate);
        }

        public static void TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) 
            where TRuleContext : IRule<TCandidate, TDecisionData>
        {
            RulesEngine.RulesRunner.TestRules<TRuleContext, TCandidate, TDecisionData>(typeToGetRulesFor, candidate, decisionData); 
        }
    }
}
