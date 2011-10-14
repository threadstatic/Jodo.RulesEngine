using System;
using Jodo.Rules;

namespace Jodo.Tests
{
    public interface IAccount { }

    public class Account : IAccount
    {
        public decimal Balance { get; set; }
    }

    public class AccountSubType : Account
    {

    }

    #region rules context objects

    public interface IAccountBalanceRules : IRule<decimal> { }
    public interface IRuleContextWithDecimalDecisionData : IRule<decimal, decimal> { }
    public interface IRuleContextWithNullableDecisionData : IRule<decimal, Account> { }

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

    public class RuleThatWillAlwaysPass : Rule<decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(true);
        }
    }

    public class RuleWithDecisionDataThatWillAlwaysPass : Rule<decimal, decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(true);
        }
    }

    public class AnotherRuleWithDecisionDataThatWillAlwaysPass : Rule<decimal, decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(true);
        }
    }

    public class RuleWithDecisionDataThatWillAlwaysFail : Rule<decimal, decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(false);
        }
    }

    public class RuleThatWillAlwaysPassAndDeclaresDecisionDataButDoesNotUseTheDecisionData : Rule<decimal, decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(true);
        }
    }

    public class RuleDeclaresAndUsesDecisionDataThatWillThrowAnExceptionIfNull : Rule<decimal, Account>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            if (DecisionData.Balance > 0)
                return new RuleResult(true);

            return new RuleResult(false);
        }
    }

    public class RuleWithDecisionDataThatIsAString : Rule<decimal, string>
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
            Description = String.Format("Checks that a account balance is at least {0}.", minimumRequiredAccountBalance.ToString("c")); 
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