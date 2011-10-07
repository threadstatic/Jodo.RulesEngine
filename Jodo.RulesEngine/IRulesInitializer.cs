using System;
using System.Collections.Generic;
using Jodo.Rules;

namespace Jodo
{
	public interface IRulesInitializer
	{
        /// <summary>
        /// Register a rule.
        /// </summary>
        /// <typeparam name="TRuleContext">
        /// This should be a interface that is used to scope the registered
        /// rule to a specific context(eg. IWithDrawlRules, or IDepositRules). It will be used when retrieving the rules,
        /// and allows many different rules to be registered for the same object Type,
        /// but under different contexts (TRuleContext).
        /// </typeparam>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <param name="rulesForType">The type of object that the rule applies to.</param>
        /// <param name="rule">A factory that returns the rule.</param>
        /// <returns></returns>
		IRulesInitializer RegisterRule<TRuleContext, TCandidate>(Type rulesForType, Func<IRule<TCandidate>> rule)
			where TRuleContext : IRule<TCandidate>;

        /// <summary>
        /// Register a collection of rules.
        /// </summary>
        /// <typeparam name="TRuleContext">
        /// This should be a interface that is used to scope the registered
        /// rule to a specific context(eg. IWithDrawlRules, or IDepositRules). It will be used when retrieving the rules,
        /// and allows many different rules to be registered for the same object Type,
        /// but under different contexts (TRuleContext).
        /// </typeparam>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <param name="rulesForType">The type of object that the rule applies to.</param>
        /// <param name="rules">A factory that returns the rule.</param>
        /// <returns></returns>
		IRulesInitializer RegisterRules<TRuleContext, TCandidate>(Type rulesForType, IEnumerable<Func<IRule<TCandidate>>> rules)
			where TRuleContext : IRule<TCandidate>;
	}
}