
namespace Jodo.Rules.Operators
{
	internal class OrRule<T> : Rule<T>
	{
		private readonly IRule<T> rule1;
		private readonly IRule<T> rule2;

		internal OrRule(IRule<T> rule1, IRule<T> rule2)
		{
			this.rule1 = rule1;
			this.rule2 = rule2;
		}

		public override RuleResult IsSatisfiedBy(T candidate)
		{
			RuleResult rule1Result = rule1.IsSatisfiedBy(candidate);

			if (rule1Result || rule2.IsSatisfiedBy(candidate))
				return new RuleResult(true);

			return new RuleResult(false, rule1Result.Messages);
		}
	}
}