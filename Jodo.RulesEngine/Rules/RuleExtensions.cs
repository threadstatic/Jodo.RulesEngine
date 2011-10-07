using System;
using System.Collections.Generic;
using Jodo.Rules.Operators;

namespace Jodo.Rules
{
	public static class RuleExtensions
	{
		public static IRule<TEntity> And<TEntity>(this IRule<TEntity> rule1, IRule<TEntity> rule2)
		{
			return new AndRule<TEntity>(rule1, rule2);
		}

		public static IRule<TEntity> Or<TEntity>(this IRule<TEntity> rule1, IRule<TEntity> rule2)
		{
			return new OrRule<TEntity>(rule1, rule2);
		}

		public static IRule<TEntity> Not<TEntity>(this IRule<TEntity> rule)
		{
			return new NotRule<TEntity>(rule);
		}

		public static RuleResult ExecuteRules<TRule, TCandidate>(this IEnumerable<Func<TRule>> rulesCollection, TCandidate candidate)
			where TRule : IRule<TCandidate>
		{
			if (rulesCollection != null)
			{
				foreach (Func<TRule> specification in rulesCollection)
				{
					RuleResult ruleResult = specification().IsSatisfiedBy(candidate);

					if (!ruleResult)
						return ruleResult;
				}
			}

			return new RuleResult(true);
		}

		public static RuleResult ExecuteRules<TRule, TCandidate, TDecisionData>(this IEnumerable<Func<TRule>> rulesCollection, TCandidate candidate, TDecisionData decisionData)
			where TRule : IRule<TCandidate, TDecisionData>
		{
			if (rulesCollection != null)
			{
				foreach (Func<TRule> specificationDelegate in rulesCollection)
				{
					IRule<TCandidate, TDecisionData> rule = specificationDelegate();
					rule.DecisionData = decisionData;

					RuleResult ruleResult = rule.IsSatisfiedBy(candidate);

					if (!ruleResult)
						return ruleResult;
				}
			}

			return new RuleResult(true);
		}
	}
}