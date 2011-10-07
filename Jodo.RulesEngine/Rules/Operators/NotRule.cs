
namespace Jodo.Rules.Operators
{
	internal class NotRule<T> : Rule<T>
	{
		private readonly IRule<T> ruleWrapped;

		internal NotRule(IRule<T> rule)
		{
			ruleWrapped = rule;
		}

		public override RuleResult IsSatisfiedBy(T candidate)
		{
			RuleResult ruleResult = ruleWrapped.IsSatisfiedBy(candidate);

			if (!ruleResult)
				return new RuleResult(true);

			return new RuleResult(false, ruleResult.Messages);
		}
	}
}