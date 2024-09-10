#region License

// Created by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionSet"/>.
    /// </summary>
    public class ActionSet : IActionSet
    {
        private readonly List<IAction> _actions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing the specified actions.
        /// </summary>
        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions =  new List<IAction>(actions ?? new IAction[0]);
        }

        #region IActionSet members

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the predicate is true.
        /// </summary>
        public IActionSet Select(Predicate<IAction> predicate)
        {
            return new ActionSet(_actions.Where(a => predicate(a)));
        }

        /// <summary>
        /// Gets the number of actions in the set.
        /// </summary>
        public int Count
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        public IActionSet Union(IActionSet other)
        {
            //This is done for reasons of efficiency. Populating an array of know size is way faster
            //than populating a list that keeps having to adjust its size.
            var union = new IAction[other.Count + Count];
            _actions.CopyTo(union);
            int index = Count;
            foreach (var action in other)
                union[index++] = action;
            
            return new ActionSet(union);
        }

        #endregion

        #region IEnumerable<IAction> Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

    }
}
