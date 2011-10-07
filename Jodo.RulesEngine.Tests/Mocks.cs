using System;
using Jodo.Rules;

namespace Jodo.Tests
{
    public class Account
    {

    }

    #region rules context objects

    public interface IAccountBalanceRules : IRule<decimal> { }
    public interface IRuleContextWithDecimalDecisionData : IRule<decimal, decimal> { }

    public class RuleContextObjectAsAClassInsteadOfAInterface : IRule<decimal>
    {
        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string FailedMessage
        {
            get { throw new NotImplementedException(); }
        }

        public RuleResult IsSatisfiedBy(decimal candidate)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Rules

    public class MockRule : Rule<decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            throw new NotImplementedException();
        }
    }

    public class MockRuleWithStringDecisionData : Rule<decimal, string>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            throw new NotImplementedException();
        }
    }

    public class MeetsTheMinimumRequiredAccountBalance : Rule<decimal>
    {
        private readonly decimal minimumRequiredAccountBalance;

        public MeetsTheMinimumRequiredAccountBalance(decimal minimumRequiredAccountBalance)
        {
            this.minimumRequiredAccountBalance = minimumRequiredAccountBalance;
            Description = "Checks that a account balance is at least $100.00."; 
        }

        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            var result = new RuleResult(candidate >= minimumRequiredAccountBalance, FailedMessage);

            if (!result)
                FailedMessage = String.Format("The account balance must be at least ${0}, but it was ${1}", minimumRequiredAccountBalance, candidate);

            return result;
        }
    }

    #endregion
}