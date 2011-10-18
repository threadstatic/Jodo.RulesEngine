using System;
using Jodo.Rules;

namespace Jodo
{
    public class Account
    {
        private readonly IRulesRunner rulesRunner;

        public int Id { get; private set; }
        public decimal Balance { get; private set; }

        public Account(int id, IRulesRunner rulesRunner)
        {
            Id = id;
            this.rulesRunner = rulesRunner;
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
            rulesRunner.TestRules<IAccountBalanceWithdrawlRules, decimal, Account>(GetType(), Balance, this);
            rulesRunner.TestRules<IAccountStatusWithdrawRules, Account>(GetType(), this);
            Balance -= amount;
        }

        public enum AccountStatus
        {
            OnHold,
            GoodStanding
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

    
    public class EnforeRuleAttribute : Attribute
    {
        public EnforeRuleAttribute(object context, Type type)
        {
        }
    }

}