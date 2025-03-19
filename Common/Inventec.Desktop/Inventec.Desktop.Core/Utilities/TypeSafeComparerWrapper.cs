/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
