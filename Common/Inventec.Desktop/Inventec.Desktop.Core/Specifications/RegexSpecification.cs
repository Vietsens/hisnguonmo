#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Text.RegularExpressions;

namespace Inventec.Desktop.Core.Specifications
{
	public class RegexSpecification : StringMatchingSpecification
	{
		public RegexSpecification(string pattern, bool ignoreCase, bool nullMatches)
			: base(pattern, ignoreCase, nullMatches)
		{
		}

		public RegexSpecification(string pattern)
			: this(pattern, true, false)
		{
		}

		protected override bool IsMatch(string test, string pattern, bool ignoreCase)
		{
			return ignoreCase ?
				Regex.Match(test, pattern, RegexOptions.IgnoreCase).Success
				: Regex.Match(test, pattern).Success;
		}
	}
}
