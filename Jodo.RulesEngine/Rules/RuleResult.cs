using System;
using System.Linq;
using System.Text;

namespace Jodo.Rules
{
	public struct RuleResult
	{
		public bool IsSatisfied { get; private set; }
		public string[] Messages { get; private set; }

		public RuleResult(bool isSatisfied)
			: this()
		{
			IsSatisfied = isSatisfied;
			Messages = new [] { string.Empty };
		}

		public RuleResult(bool isSatisfied, string message)
			: this()
		{
			IsSatisfied = isSatisfied;
			Messages = new [] { message };
		}

		public RuleResult(bool isSatisfied, string[] messages)
			: this()
		{
			IsSatisfied = isSatisfied;
			Messages = messages;
		}

		public void AddMessage(string message)
		{
			if(String.IsNullOrEmpty(message))
				return;

			Messages = Messages.Where(m => !string.IsNullOrEmpty(m)).ToArray();
		}

		public void AddMessages(string[] messages)
		{
			if (messages.Length < 1)
				return;

			foreach (var message in messages)
				AddMessage(message);
		}

		public static implicit operator bool(RuleResult ruleResult)
		{
			return ruleResult.IsSatisfied;
		}

        public static implicit operator string(RuleResult ruleResult)
        {
            return FlattenMessage(ruleResult.Messages);
        }

        public override string ToString()
        {
            return FlattenMessage(Messages);
        }

        private static string FlattenMessage(string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in messages)
                sb.AppendLine(message);

            return sb.ToString();
        }

	}
}
