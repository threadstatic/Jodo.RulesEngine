using System;

namespace Jodo
{
    public class BusinessRuleViolatedException : Exception
    {
        public BusinessRuleViolatedException(string message) : base(message)
        {
           
        }
    }
}
