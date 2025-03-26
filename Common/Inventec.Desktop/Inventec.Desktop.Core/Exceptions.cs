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
