#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Actions
{
    ///<summary>
    ///Exception that indicates a problem with the way action attributes are applied to a tool.
    ///</summary>
    public class ActionBuilderException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal ActionBuilderException(string message)
            : base(message)
        {
        }
    }
}
