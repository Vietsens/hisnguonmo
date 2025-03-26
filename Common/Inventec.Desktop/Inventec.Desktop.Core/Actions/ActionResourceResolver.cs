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
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// A specialization of the <see cref="ResourceResolver"/> class for use in resolving resources
	/// related to actions.
	/// </summary>
	public class ActionResourceResolver : ApplicationThemeResourceResolver
	{
		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="actionTarget">The action target for which resources will be resolved.</param>
		public ActionResourceResolver(object actionTarget)
			: base(actionTarget.GetType(), true) {}

		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="targetType">The action target type for which resources will be resolved.</param>
		public ActionResourceResolver(Type targetType)
			: base(targetType, true) {}
	}
}
