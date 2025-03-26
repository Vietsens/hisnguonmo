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
#region Using directives
using System.Drawing;
#endregion

namespace Inventec.DrawTools
{
	/// <summary>
	/// Helper class used to show properties
	/// for one or more graphic objects
	/// </summary>
	internal class GraphicsProperties
	{
		private Color? color;
		private int? penWidth;

		public GraphicsProperties()
		{
			color = null;
			penWidth = null;
		}

		public Color? Color
		{
			get { return color; }
			set { color = value; }
		}

		public int? PenWidth
		{
			get { return penWidth; }
			set { penWidth = value; }
		}
	}
}
