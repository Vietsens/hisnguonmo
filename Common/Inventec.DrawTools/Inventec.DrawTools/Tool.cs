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
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	/// <summary>
	/// Base class for all drawing tools
	/// </summary>
	internal abstract class Tool
	{
		/// <summary>
		/// Left nous button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
		}


		/// <summary>
		/// Mouse is moved, left mouse button is pressed or none button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
		}


		/// <summary>
		/// Left mouse button is released
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
		}
	}
}
