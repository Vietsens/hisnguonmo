#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System;

namespace Inventec.Desktop.Core.Specifications
{
	public class ContainsSpecification : StringMatchingSpecification
	{
		public ContainsSpecification(string pattern, bool ignoreCase, bool nullMatches)
			: base(pattern, ignoreCase, nullMatches)
		{
		}

		protected override bool IsMatch(string test, string pattern, bool ignoreCase)
		{
			return test.IndexOf(pattern, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) > -1;
		}
	}
}
