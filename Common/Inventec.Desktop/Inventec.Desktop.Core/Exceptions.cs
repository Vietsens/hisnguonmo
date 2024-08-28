#region License

// Created by phuongdt

#endregion

using System;
using System.Runtime.Serialization;

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// Used by the framework to relay errors regarding plugins.
	/// </summary>
	/// <seealso cref="PluginManager"/>
    [SerializableAttribute]
	public class PluginException : ApplicationException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message, Exception inner) : base(message, inner) { }
	}

	/// <summary>
	/// Used by the framework to relay errors regarding <see cref="IExtensionPoint"/>s.
	/// </summary>
	/// <seealso cref="PluginInfo"/>
    public class ExtensionPointException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message, Exception inner) : base(message, inner) { }
    }

	/// <summary>
	/// Used by the framework to relay errors regarding extensions (created via <see cref="IExtensionPoint"/>s).
	/// </summary>
    public class ExtensionException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message, Exception inner) : base(message, inner) { }
    }
}
