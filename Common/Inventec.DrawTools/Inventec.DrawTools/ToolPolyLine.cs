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
using System.Drawing;
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	/// <summary>
	/// PolyLine tool (a PolyLine is a series of connected straight lines where each line is drawn individually)
	/// </summary>
	internal class ToolPolyLine : ToolObject
	{
		public ToolPolyLine()
		{
			Cursor = new Cursor(GetType(), "Pencil.cur");
		}

		private DrawPolyLine newPolyLine;
		private bool _drawingInProcess = false; // Set to true when drawing

		/// <summary>
		/// Left nouse button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			if (e.Button ==
			    MouseButtons.Right)
			{
				_drawingInProcess = false;
				newPolyLine = null;
			}
			else
			{
				Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				if (drawArea.PenType ==
				    DrawingPens.PenType.Generic)
				{
					if (_drawingInProcess == false)
					{
						newPolyLine = new DrawPolyLine(p.X, p.Y, p.X + 1, p.Y + 1, drawArea.LineColor, drawArea.LineWidth);
						newPolyLine.EndPoint = new Point(p.X + 1, p.Y + 1);
						AddNewObject(drawArea, newPolyLine);
						_drawingInProcess = true;
					}
					else
					{
						// Drawing is in process, so simply add a new point
						newPolyLine.AddPoint(p);
						newPolyLine.EndPoint = p;
					}
				}
				else
				{
					if (_drawingInProcess == false)
					{
						newPolyLine = new DrawPolyLine(p.X, p.Y, p.X + 1, p.Y + 1, drawArea.PenType);
						newPolyLine.EndPoint = new Point(p.X + 1, p.Y + 1);
						AddNewObject(drawArea, newPolyLine);
						_drawingInProcess = true;
					}
					else
					{
						// Drawing is in process, so simply add a new point
						newPolyLine.AddPoint(p);
						newPolyLine.EndPoint = p;
					}
				}
			}
		}

		/// <summary>
		/// Mouse move - resize new polygon
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;

			if (e.Button !=
			    MouseButtons.Left)
				return;

			if (newPolyLine == null)
				return; // precaution

			Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			// move last point
			newPolyLine.MoveHandleTo(point, newPolyLine.HandleCount);
			drawArea.Refresh();
		}
	}
}
