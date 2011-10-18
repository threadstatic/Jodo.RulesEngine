using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Jodo
{
    [Export]
    public class AccountWithdrawlHandler : IHandler<AccountWithdrawl>
    {
        private readonly IAccountRepository accountRepository;

        [ImportingConstructor]
        public AccountWithdrawlHandler(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public void Handle(AccountWithdrawl command)
        {
            Account account = accountRepository.Load(command.AccountId);
            bool withDrawlFailed;

            do
            {
                try
                {
                    Console.WriteLine(String.Format("Account balance is ${0}", account.Balance));
                    Console.WriteLine("Attempt to withdrawl $5.00.");

                    account.WithDrawl(5);

                    withDrawlFailed = false;
                    Console.WriteLine("Withdrawl successful");
                    Console.WriteLine(String.Format("Account balance is ${0}", account.Balance));
                }
                catch (InvalidOperationException ex)
                {
                    withDrawlFailed = true;
                    Console.WriteLine(String.Format("Make deposit of ${0}", command.Amount));

                    account.MakeDeposit(command.Amount);
                }
            }
            while (withDrawlFailed);
        }
    }

    public interface IHandler<in TCommand>
    {
        void Handle(TCommand command);
    }

    public interface IAccountRepository
    {
        void Save(Account account);
        Account Load(int id);
    }
   
    public class AccountRepository : IAccountRepository
    {
        private static readonly Dictionary<int, Account> store = new Dictionary<int, Account>();
       
        public void Save(Account account)
        {
            store[account.Id] = account;
        }

        public Account Load(int id)
        {
            return store[id];
        }
    }

    public class AccountWithdrawl   
    {
        public readonly int AccountId;
        public readonly decimal Amount;

        public AccountWithdrawl(int accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}
