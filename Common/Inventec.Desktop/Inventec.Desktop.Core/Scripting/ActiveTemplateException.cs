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
