#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Scripting
{
	/// <summary>
	/// Used by the <see cref="ActiveTemplate"/> class.
	/// </summary>
	[Serializable]
	public class ActiveTemplateException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
        public ActiveTemplateException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
}
