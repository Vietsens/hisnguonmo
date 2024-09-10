#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class StringMatchingSpecification : PrimitiveSpecification
	{
		private readonly string _pattern;
		private readonly bool _ignoreCase = true;	// true by default
		private readonly bool _nullMatches;

		protected StringMatchingSpecification(string pattern, bool ignoreCase, bool nullMatches)
		{
			Platform.CheckForNullReference(pattern, "pattern");
			Platform.CheckForEmptyString(pattern, "pattern");

			_pattern = pattern;
			_ignoreCase = ignoreCase;
			_nullMatches = nullMatches;
		}

		public string Pattern
		{
			get { return _pattern; }
		}

		public bool IgnoreCase
		{
			get { return _ignoreCase; }
		}

		public bool NullMatches
		{
			get { return _nullMatches; }
		}

		protected sealed override TestResult InnerTest(object exp, object root)
		{
			if (exp == null)
				return DefaultTestResult(_nullMatches);

			if (exp is string)
			{
				return DefaultTestResult(IsMatch(exp as string, _pattern, _ignoreCase));
			}
			throw new SpecificationException(SR.ExceptionCastExpressionString);
		}

		protected abstract bool IsMatch(string test, string pattern, bool ignoreCase);
	}
}
