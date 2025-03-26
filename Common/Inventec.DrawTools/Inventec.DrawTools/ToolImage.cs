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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	/// <summary>
	/// Image tool
	/// </summary>
	internal class ToolImage : ToolObject
	{
		public ToolImage()
		{
			Cursor = new Cursor(GetType(), "Rectangle.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			AddNewObject(drawArea, new DrawImage(p.X, p.Y));
		}

		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;

			if (e.Button ==
			    MouseButtons.Left)
			{
				Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				int al = drawArea.TheLayers.ActiveLayerIndex;
				drawArea.TheLayers[al].Graphics[0].MoveHandleTo(point, 5);
				drawArea.Refresh();
			}
		}

		public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn hình";
            ofd.Filter = "All files|*.*|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|Fireworks (*.png)|*.png|GIF (*.gif)|*.gif|Icon (*.ico)|*.ico";
            ofd.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
            int al = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.Owner.HinhAnh != null)
            {
                ((DrawImage)drawArea.TheLayers[al].Graphics[0]).TheImage = (Bitmap)drawArea.Owner.HinhAnh;
            }
            else if(ofd.ShowDialog() == DialogResult.OK)
            {
                ((DrawImage)drawArea.TheLayers[al].Graphics[0]).TheImage = (Bitmap)Bitmap.FromFile(ofd.FileName);
            }
            ofd.Dispose();
            base.OnMouseUp(drawArea, e);
		}
	}
}
