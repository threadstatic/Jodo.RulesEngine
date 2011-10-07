using System;

namespace Jodo.Rules
{
    public interface IRulesRunner
    {
        void TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate) where TRuleContext : IRule<TCandidate>;
        void TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) where TRuleContext : IRule<TCandidate, TDecisionData>;
    }
}
