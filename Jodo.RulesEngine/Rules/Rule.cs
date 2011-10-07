using System;

namespace Jodo.Rules
{
	public abstract class Rule<TCandidate> : IRule<TCandidate>
	{
	    public string Description { get; protected set; }
        public string FailedMessage { get; protected set; }

		protected Rule()
		{
            FailedMessage = String.Format("Rule not satisfied for {0}",  this); 
		}

		protected Rule(string description, string failedMessage) : this()
		{
			Description = description;
			FailedMessage = failedMessage;
		}

		#region ISpecification<TCandidate> Members

		public abstract RuleResult IsSatisfiedBy(TCandidate candidate);

		#endregion

        public override string ToString()
        {
            return String.Format("{0} - {1}", GetType().Name, Description);
        }
	}

	public abstract class Rule<TCandidate, TDecisionData> : Rule<TCandidate>, IRule<TCandidate, TDecisionData>
	{
        public TDecisionData DecisionData { get; set; }
	}
}