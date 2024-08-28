#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Set of flags that customize the behaviour of click actions.
    /// </summary>
    [Flags]
    public enum ClickActionFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Specifies that the action is a "check" action (e.g. that it has toggle behaviour).
        /// </summary>
        CheckAction = 0x01,

		/// <summary>
		/// Specifies that parents of the action should be checked when the action is checked.
		/// </summary>
		CheckParents = 0x02
    }
}
