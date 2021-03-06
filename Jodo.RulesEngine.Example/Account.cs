﻿using System;
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
}