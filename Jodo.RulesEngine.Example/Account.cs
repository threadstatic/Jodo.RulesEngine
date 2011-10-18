using System;
using Jodo.Rules;

namespace Jodo
{
    public class Account
    {
        public int Id { get; private set; }
        public decimal Balance { get; private set; }

        public Account(int id)
        {
            Id = id;
            Balance = -10;
        }

        public void MakeDeposit(decimal amount)
        {
            Balance += amount;
        }

        public AccountStatus GetAccountStatus()
        {
            return Balance <= 0 ? AccountStatus.OnHold : AccountStatus.GoodStanding;
        }

        [EnforeRule(typeof(IAccountBalanceWithdrawlRules), typeof(Account))]
        public void WithDrawl(decimal amount)
        {
            TestWithDrawlRules();
            Balance -= amount;
        }

        protected virtual void TestWithDrawlRules()
        {
            RulesRunner.TestRules<IAccountBalanceWithdrawlRules, decimal, Account>(GetType(), Balance, this);
            RulesRunner.TestRules<IAccountStatusWithdrawRules, Account>(GetType(), this);
        }

        public enum AccountStatus
        {
            OnHold,
            GoodStanding
        }
    }

    
    public class EnforeRuleAttribute : Attribute
    {
        public EnforeRuleAttribute(object context, Type type)
        {
        }
    }

    public class GetRuleData
    {
        public object Candidate { get; set; }
        public Account DecisionData { get; set; }
        public Type TypeToGetRulesFor { get; set; }

        public GetRuleData(Account account)
        {
            DecisionData = account;
            Candidate = account.Balance;
            TypeToGetRulesFor = account.GetType();
        }
    }
}