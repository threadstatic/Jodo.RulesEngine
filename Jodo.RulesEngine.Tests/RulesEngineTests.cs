﻿using System;
using System.Linq;
using Jodo.Rules;
using NUnit.Framework;

namespace Jodo.Tests
{
	public class RulesEngineTests
	{
        private RulesEngine rulesEngine;
        private IRulesInitializer RulesInitializer { get { return rulesEngine; } }
        private IRulesProvider RulesProvider { get { return rulesEngine; } }

        [SetUp]
        public void Setup()
        {
            rulesEngine = new RulesEngine();
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
        }

        [TearDown]
        public void TearDown()
        {
            rulesEngine.Dispose();
        }

		[Test]
        public void GetRuleFor_ReturnsOneRuleThatIsTheRegisteredRule()
		{
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList().Single().Invoke().GetType());
		}

        [Test]
        public void RegisterRule_WhenRuleHasAlreadyBeenRegisted_WillRegisterTheRuleAgain()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[1].Invoke().GetType());
        }

        [Test]
        public void GetRulesFor_ReturnsAllRegisteredRules()
        {
            RuleThatWillAlwaysPass rule = new RuleThatWillAlwaysPass();
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(RuleThatWillAlwaysPass), rules.ToList()[1].Invoke().GetType());
        }

        [Test]
        public void RegisterRule_WithRuleContextClassInsteadOfAInterface_ThrowsException()
        {
            Action action = () => RulesInitializer.RegisterRule<RuleContextThatIsAClassInsteadOfAInterface, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass());
            Assert.Throws<RulesInitializationException>(new TestDelegate(action));
        }

        [Test]
        public void RegisterRule_WithARuleContextDecisionDataTypeAndRuleDecisionDataTypeMismatch_ThrowsException()
        {
            Action action = () => RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatIsAString());
            Assert.Throws<DecisionDataTypeMismatchException>(new TestDelegate(action));
        }

        [Test]
        public void GetRulesFor_ASubType_OfATypeThatHasRulesRegistered_SubTypeAndBaseTypeRulesAreReturned()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(AccountSubType), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(AccountSubType));
            Assert.AreEqual(2, rules.ToList().Count);
        }

        [Test]
        public void GetRulesFor_ForASubTypeOfATypeThatHasRules_OnlyBaseTypeRulesAreReturned()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(AccountSubType), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));
            Assert.AreEqual(1, rules.ToList().Count);
        }

        [Test]
        public void GetRulesFor_AInterfaceThatHasOneRuleRegistered_RuleIsReturned()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(IAccount), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(IAccount));
            Assert.AreEqual(1, rules.ToList().Count);
        }
	}
}
