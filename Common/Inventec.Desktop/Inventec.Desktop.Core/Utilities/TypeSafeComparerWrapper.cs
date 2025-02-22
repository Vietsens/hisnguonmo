#region License

// Created by phuongdt

#endregion

using System.Collections;
using System.Collections.Generic;

namespace Inventec.Desktop.Core.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IComparer"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be compared.</typeparam>
    public class TypeSafeComparerWrapper<T> : Comparer<T>, IComparer
    {
        private IComparer _inner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inner">The untyped <see cref="IComparer"/> to wrap.</param>
		public TypeSafeComparerWrapper(IComparer inner)
        {
            _inner = inner;
        }

		/// <summary>
		/// Compares the objects <paramref name="x"/> and <paramref name="y"/> and returns
		/// a value indicating the relationship between them.
		/// </summary>
		/// <remarks>
		/// A value of 0 indicates equality, &gt; 0 indicates that x &gt; y, &lt; 0 indicates that x &lt; y.
		/// </remarks>
		public override int Compare(T x, T y)
        {
            return _inner.Compare(x, y);
        }

        #region IComparer Members

		/// <summary>
		/// Compares the objects <paramref name="x"/> and <paramref name="y"/> and returns
		/// a value indicating the relationship between them.
		/// </summary>
		/// <remarks>
		/// A value of 0 indicates equality, &gt; 0 indicates that x &gt; y, &lt; 0 indicates that x &lt; y.
		/// </remarks>
		int IComparer.Compare(object x, object y)
        {
            return _inner.Compare(x, y);
        }

        #endregion
    }
}
