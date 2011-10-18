using System;

namespace Jodo.Rules
{
    public sealed class RulesRunner : IRulesRunner
    {
        void IRulesRunner.TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate)
        {
            this.TestRules<TRuleContext, TCandidate>(new RulesEngine(), typeToGetRulesFor, candidate);
        }

        void IRulesRunner.TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData)
        {
            this.TestRules<TRuleContext, TCandidate, TDecisionData>(new RulesEngine(), typeToGetRulesFor, candidate, decisionData);
        }

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
