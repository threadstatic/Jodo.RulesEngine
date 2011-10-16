
namespace Jodo.Rules.Operators
{
    public class AndRule<TCandidate, TDecisionData> : Rule<TCandidate, TDecisionData>
    {
        private readonly IRule<TCandidate, TDecisionData> rule1;
        private readonly IRule<TCandidate, TDecisionData> rule2;

        public AndRule(IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate, TDecisionData> rule2)
        {
            this.rule1 = rule1;
            this.rule2 = rule2;
        }

        public override RuleResult IsSatisfiedBy(TCandidate candidate)
        {
            rule1.DecisionData = DecisionData;
            rule2.DecisionData = DecisionData;

            return AndRule<TCandidate>.ExecuteRules(rule1, rule2, candidate);
        }
    }

    public class AndRule<TCandidate> : Rule<TCandidate>
	{
        private readonly IRule<TCandidate> rule1;
        private readonly IRule<TCandidate> rule2;

        public AndRule(IRule<TCandidate> rule1, IRule<TCandidate> rule2)
		{
			this.rule1 = rule1;
			this.rule2 = rule2;
		}

        public override RuleResult IsSatisfiedBy(TCandidate candidate)
		{
            return ExecuteRules(rule1, rule2, candidate);
		}

        internal static RuleResult ExecuteRules(IRule<TCandidate> rule1, IRule<TCandidate> rule2, TCandidate candidate)
        {
            RuleResult rule1Result = rule1.IsSatisfiedBy(candidate);

            if (rule1Result)
            {
                RuleResult rule2Result = rule2.IsSatisfiedBy(candidate);

                if (rule2Result)
                    return new RuleResult(true);

                return new RuleResult(false, rule2Result.Messages);
            }

            return new RuleResult(false, rule1Result.Messages);
        }
	}
}