using System;
using System.Linq;
using Jodo.Rules;
using NUnit.Framework;

namespace Jodo.Tests
{
	public class RulesEngineTests
	{
        private static RulesEngine rulesEngine = new RulesEngine();
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
            MeetsTheMinimumRequiredAccountBalance rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);

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
	}
}
