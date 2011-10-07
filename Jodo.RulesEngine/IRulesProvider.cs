using System;
using System.Collections.Generic;
using Jodo.Rules;

namespace Jodo
{
	public interface IRulesProvider
	{
		IEnumerable<Func<IRule<TCandidate>>> GetRulesFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
			where TRuleContext : IRule<TCandidate>;

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using an AND <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <returns></returns>
		IRule<TCandidate> GetRuleFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
			where TRuleContext : IRule<TCandidate>;

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using a <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <param name="compositionStrategy"></param>
		/// <returns></returns>
		IRule<TCandidate> GetRuleFor<TRuleContext, TCandidate>(Type typeToGetRulesFor, RuleCompositionStrategy compositionStrategy)
			where TRuleContext : IRule<TCandidate>;

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using an AND <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <typeparam name="TDecisionData"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <param name="decisionData"></param>
		/// <returns></returns>
		IRule<TCandidate> GetRuleFor<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TDecisionData decisionData)
			where TRuleContext : IRule<TCandidate, TDecisionData>;

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using an AND <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <typeparam name="TDecisionData"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <param name="compositionStrategy"></param>
		/// <param name="decisionData"></param>
		/// <returns></returns>
		IRule<TCandidate> GetRuleFor<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, RuleCompositionStrategy compositionStrategy, TDecisionData decisionData)
			where TRuleContext : IRule<TCandidate, TDecisionData>;
  	}
}