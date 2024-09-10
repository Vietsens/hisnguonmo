#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System;

namespace Inventec.Desktop.Core.Specifications
{
	public class StartsWithSpecification : StringMatchingSpecification
	{
		public StartsWithSpecification(string pattern, bool ignoreCase, bool nullMatches)
			: base(pattern, ignoreCase, nullMatches)
		{
		}

		protected override bool IsMatch(string test, string pattern, bool ignoreCase)
		{
			return test.StartsWith(pattern, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
		}
	}
}
