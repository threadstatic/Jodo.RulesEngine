using System;
using System.Linq;
using Jodo.Rules;
using NUnit.Framework;

namespace Jodo.Tests
{
	public class RulesEngineTests
	{
        private RulesEngine rulesEngine = new RulesEngine();
        private IRulesInitializer RulesInitializer { get { return rulesEngine; } }
        private IRulesProvider RulesProvider { get { return rulesEngine; } }

        [SetUp]
        public void Setup()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
        }

		[Test]
        public void RegisterRule_GetRuleFor_ReturnsOneRuleThatIsTheRegisteredRule()
		{
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList().First().Invoke().GetType());
		}

        [Test]
        public void RegisterRule_WhenRuleHasAlreadyBeenRegisted_WillRegisterTheRuleAgain()
        {
            new RulesEngine();
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[1].Invoke().GetType());
        }

        [Test]
        public void RegisterRules_GetRulesFor_ReturnsAllRegisteredRules()
        {
            MockRule rule = new MockRule();
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(MockRule), rules.ToList()[1].Invoke().GetType());
        }

		[Test]
        public void GetRuleFor_WhenNoRulesHaveBeenRegisteredForTheRequestedType_NullRuleReturned()
		{
            var rule = RulesProvider.GetRuleFor<IAccountBalanceRules, decimal>(typeof(decimal));
            Assert.IsTrue(rule.GetType().Equals(typeof(NullRule<decimal>)));
		}

        [Test]
        public void GetRuleFor_WhenNoRulesHaveBeenRegistered_NullRuleReturned()
        {
            rulesEngine = new RulesEngine(); 
            var rule = RulesProvider.GetRuleFor<IAccountBalanceRules, decimal>(typeof(decimal));
            Assert.IsTrue(rule.GetType().Equals(typeof(NullRule<decimal>)));
        }

        [Test]
        public void RegisterRule_WithRuleContextClassInsteadOfAInterface_ThrowsRulesInitializationException()
        {
            Action action = () => RulesInitializer.RegisterRule<RuleContextObjectAsAClassInsteadOfAInterface, decimal>(typeof(Account), () => new MockRule());
            Assert.Throws<RulesInitializationException>(new TestDelegate(action));
        }

        [Test]
        public void RegisterRule_WithARuleContextDecisionDataTypeAndRuleDecisionDataTypeMismatch_ThrowsDecisionDataTypeException()
        {
            Action action = () => RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new MockRuleWithStringDecisionData());
            Assert.Throws<DecisionDataTypeException>(new TestDelegate(action));
        }

        [Test]
        public void GetRulesFor_ForASubTypeOfATypeThatHasRules_BaseTypeRulesAreReturnedAsWell()
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
        public void GetRulesFor_ForAInterfaceThatHasOneRuleRegistered_RuleIsReturned()
        {
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(IAccount), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(IAccount));
            Assert.AreEqual(1, rules.ToList().Count);
        }
	}
}
