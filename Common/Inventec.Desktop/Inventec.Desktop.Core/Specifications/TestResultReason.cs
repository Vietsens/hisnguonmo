#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class TestResultReason
	{
		private readonly string _message;
		private readonly TestResultReason[] _reasons;

		public TestResultReason(string message)
			: this(message, new TestResultReason[] { })
		{
		}

		public TestResultReason(string message, TestResultReason reason)
			: this(message, new [] { reason })
		{
		}

		public TestResultReason(string message, TestResultReason[] reasons)
		{
			_message = message;
			_reasons = reasons;
		}

		public string Message
		{
			get { return _message; }
		}

		public TestResultReason[] Reasons
		{
			get { return _reasons; }
		}
	}
}
