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
	/// Base class for all tools which create new graphic object
	/// </summary>
	internal abstract class ToolObject : Tool
	{
		private Cursor cursor;

		/// <summary>
		/// Tool cursor.
		/// </summary>
		protected Cursor Cursor
		{
			get { return cursor; }
			set { cursor = value; }
		}


		/// <summary>
		/// Left mouse is released.
		/// New object is created and resized.
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.Count > 0)
				drawArea.TheLayers[al].Graphics[0].Normalize();
			//drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;

			drawArea.Capture = false;
			drawArea.Refresh();
		}

		/// <summary>
		/// Add new object to draw area.
		/// Function is called when user left-clicks draw area,
		/// and one of ToolObject-derived tools is active.
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="o"></param>
		protected void AddNewObject(DrawArea drawArea, DrawObject o)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[al].Graphics.UnselectAll();

			o.Selected = true;
			o.Dirty = true;
			int objectID = 0;
			// Set the object id now
			for (int i = 0; i < drawArea.TheLayers.Count; i++)
			{
				objectID = +drawArea.TheLayers[i].Graphics.Count;
			}
			objectID++;
			o.ID = objectID;
			drawArea.TheLayers[al].Graphics.Add(o);

			drawArea.Capture = true;
			drawArea.Refresh();
		}
	}
}
