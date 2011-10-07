
namespace Jodo.Rules.Operators
{
	internal class AndRule<T> : Rule<T>
	{
		private readonly IRule<T> rule1;
		private readonly IRule<T> rule2;

		internal AndRule(IRule<T> rule1, IRule<T> rule2)
		{
			this.rule1 = rule1;
			this.rule2 = rule2;
		}

		public override RuleResult IsSatisfiedBy(T candidate)
		{
			RuleResult rule1Result = rule1.IsSatisfiedBy(candidate);

			if (rule1Result)
			{
				RuleResult rule2Result = rule2.IsSatisfiedBy(candidate);

				if (rule2Result)
				{
					return new RuleResult(true);
				}

				return new RuleResult(false, rule2Result.Messages);
			}

			return new RuleResult(false, rule1Result.Messages);
		}
	}
}