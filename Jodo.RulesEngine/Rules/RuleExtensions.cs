using System;
using Jodo.Rules.Operators;

namespace Jodo.Rules
{
	public static class RuleExtensions
	{
        public static IRule<TCandidate> And<TCandidate>(this IRule<TCandidate> rule1, IRule<TCandidate> rule2)
		{
            return new AndRule<TCandidate>(rule1, rule2);
		}

        public static IRule<TCandidate, TDecisionData> And<TCandidate, TDecisionData>(this IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate> rule2)
        {
            return new AndRule<TCandidate, TDecisionData>(rule1, rule2.ConvertToRuleWithDecisionData<TCandidate, TDecisionData>());
        }

        public static IRule<TCandidate, TDecisionData> And<TCandidate, TDecisionData>(this IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate, TDecisionData> rule2)
        {
            return new AndRule<TCandidate, TDecisionData>(rule1, rule2);
        }

        public static IRule<TCandidate> Or<TCandidate>(this IRule<TCandidate> rule1, IRule<TCandidate> rule2)
		{
            return new OrRule<TCandidate>(rule1, rule2);
		}

        public static IRule<TCandidate, TDecisionData> Or<TCandidate, TDecisionData>(this IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate> rule2)
        {
            return new OrRule<TCandidate, TDecisionData>(rule1, rule2.ConvertToRuleWithDecisionData<TCandidate, TDecisionData>());
        }

        public static IRule<TCandidate, TDecisionData> Or<TCandidate, TDecisionData>(this IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate, TDecisionData> rule2)
        {
            return new OrRule<TCandidate, TDecisionData>(rule1, rule2);
        }

        public static IRule<TCandidate> Not<TCandidate>(this IRule<TCandidate> rule)
		{
            return new NotRule<TCandidate>(rule);
		}

        public static IRule<TCandidate, TDecisionData> Not<TCandidate, TDecisionData>(this IRule<TCandidate, TDecisionData> rule)
        {
            return new NotRule<TCandidate, TDecisionData>(rule);
        }

        private static RuleWithDecisionData<TCandidate, TDecisionData> ConvertToRuleWithDecisionData<TCandidate, TDecisionData>(this IRule<TCandidate> rule)
		{
            return new RuleWithDecisionData<TCandidate, TDecisionData>(rule);
		}

        private class RuleWithDecisionData<TCandidate, TDecisionData> : Rule<TCandidate, TDecisionData>
        {
            private readonly IRule<TCandidate> rule;

            public RuleWithDecisionData(IRule<TCandidate> rule)
            {
                this.rule = rule;
            }

            public override RuleResult IsSatisfiedBy(TCandidate candidate)
            {
                return rule.IsSatisfiedBy(candidate);
            }
        }
	}
}