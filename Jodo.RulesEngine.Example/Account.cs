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
    }
}