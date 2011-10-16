using System;
using System.Collections.Generic;
using Jodo.Rules;

namespace Jodo
{
	public interface IRulesProvider
	{
        /// <summary>
        /// Returns all rules for the requested type.
        /// </summary>
        /// <typeparam name="TRuleContext">The context in which to retrieve rules for.</typeparam>
        /// <typeparam name="TCandidate">The rule candidate type.</typeparam>
        /// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
        /// <returns>A IEnumerable of rule factories.</returns>
        IEnumerable<Func<IRule<TCandidate>>> GetRulesFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
            where TRuleContext : IRule<TCandidate>;
  	}
}