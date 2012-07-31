namespace Jodo.Testing
{
    public class Rule<TRule>
    {
        public static bool IsRegisteredFor<TType>()
        {
            return RulesEngine.RuleIsRegisteredFor<TRule>(typeof(TType));
        }
    }
}