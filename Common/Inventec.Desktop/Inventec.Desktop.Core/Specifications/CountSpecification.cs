#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System;
using System.Collections;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Specifications
{
	/// <summary>
	/// Counts the number of items in a <see cref="IEnumerable"/> that satisfy the inner specification. If no
	/// inner specification is supplied, all items are counted.
	/// </summary>
	public class CountSpecification : PrimitiveSpecification
	{
		class NullSpecification : ISpecification
		{
			public TestResult Test(object obj)
			{
				return new TestResult(true);
			}
		}

		private static readonly ISpecification NullFilter = new NullSpecification();

		private readonly int _min = 0;
		private readonly int _max = Int32.MaxValue;
		private readonly ISpecification _filterSpecification;

		public CountSpecification(int min, int max, ISpecification filterSpecification)
		{
			Platform.CheckArgumentRange(min, 0, int.MaxValue, "min");
			Platform.CheckArgumentRange(max, 0, int.MaxValue, "max");
			if (max < min)
				throw new ArgumentException("min cannot be larger than max");

			_max = max;
			_min = min;
			_filterSpecification = filterSpecification ?? NullFilter;
		}

		public int Min
		{
			get { return _min; }
		}

		public int Max
		{
			get { return _max; }
		}

		public ISpecification FilterSpecification
		{
			get { return _filterSpecification; }
		}

		protected override TestResult InnerTest(object exp, object root)
		{
			// optimizations
			// if _innerSpecification is NullFilter, and exp is an Array or ICollection, we can just use Length/Count
			if (_filterSpecification == NullFilter)
			{
				if (exp is Array)
				{
					return DefaultTestResult(InRange((exp as Array).Length));
				}

				if (exp is ICollection)
				{
					return DefaultTestResult(InRange((exp as ICollection).Count));
				}
			}

			// otherwise, treat as IEnumerable and evaluate _innerSpecification
			if (exp is IEnumerable)
			{
				ICollection countableItems = CollectionUtils.Select(exp as IEnumerable, item => _filterSpecification.Test(item).Success);

				return DefaultTestResult(InRange(countableItems.Count));
			}

			throw new SpecificationException(SR.ExceptionCastExpressionArrayCollectionEnumerable);
		}

		protected bool InRange(int n)
		{
			return n >= _min && n <= _max;
		}
	}
}
