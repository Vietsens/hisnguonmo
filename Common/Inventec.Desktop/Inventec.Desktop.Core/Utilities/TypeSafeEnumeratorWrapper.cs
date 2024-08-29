#region License

// Created by phuongdt

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Inventec.Desktop.Core.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IEnumerator"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be enumerated.</typeparam>
    public class TypeSafeEnumeratorWrapper<T> : IEnumerator<T>
    {
        private IEnumerator _inner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inner">The untyped <see cref="IEnumerator" /> to be wrapped.</param>
        public TypeSafeEnumeratorWrapper(IEnumerator inner)
        {
            _inner = inner;
        }

        #region IEnumerator<T> Members

		/// <summary>
		/// Gets the object at the current <see cref="IEnumerator{T}"/> position.
		/// </summary>
        public T Current
        {
            get { return (T)_inner.Current; }
        }

        #endregion

        #region IEnumerator Members

		/// <summary>
		/// Gets the object at the current <see cref="IEnumerator"/> position.
		/// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return _inner.Current; }
        }

		/// <summary>
		/// Moves to the next element.
		/// </summary>
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

		/// <summary>
		/// Resets the <see cref="IEnumerator"/>.
		/// </summary>
        public void Reset()
        {
            _inner.Reset();
        }

        #endregion

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}
