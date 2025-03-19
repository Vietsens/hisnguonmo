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

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Defines a generic mechanism for binding to an arbitrary observable property of an object.
	/// </summary>
	/// <remarks>
	/// An observable property is any property which has a corresponding change notification event.
	/// </remarks>
	/// <typeparam name="T">The type of the property.</typeparam>
	internal interface IObservablePropertyBinding<T>
	{
		/// <summary>
		/// The event that provides notification of when the <see cref="PropertyValue"/> has changed.
		/// </summary>
		event EventHandler PropertyChanged;

		/// <summary>
		/// Gets or sets the underlying property value.
		/// </summary>
		T PropertyValue { get; set; }
	}
}
