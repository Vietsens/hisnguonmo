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
using System.Drawing.Drawing2D;

namespace Inventec.DrawTools
{
	public class DrawingPens
	{
		#region Enumerations
		public enum PenType
		{
			Generic,
			RedPen,
			BluePen,
			GreenPen,
			RedDottedPen,
			RedDotDashPen,
			DoubleLinePen,
			DashedArrowPen,
			NumberOfPens
		} ;
		#endregion Enumerations

		/// <summary>
		/// Return a pen based on the type requested
		/// </summary>
		/// <param name="_penType">Type of pen from the PenType enumeration</param>
		/// <returns>Requested pen</returns>
		public static Pen SetCurrentPen(PenType _penType)
		{
			Pen pen;
			switch (_penType)
			{
				case PenType.Generic:
					pen = null;
					break;
				case PenType.RedPen:
					pen = RedPen();
					break;
				case PenType.BluePen:
					pen = BluePen();
					break;
				case PenType.GreenPen:
					pen = GreenPen();
					break;
				case PenType.RedDotDashPen:
					pen = RedDotDashPen();
					break;
				case PenType.RedDottedPen:
					pen = RedDottedPen();
					break;
				case PenType.DoubleLinePen:
					pen = DoubleLinePen();
					break;
				case PenType.DashedArrowPen:
					pen = DashedArrowLinePen();
					break;
				default:
					pen = null;
					break;
			}
			return pen;
		}

		/// <returns>Returns a 5 pixel wide Red pen</returns>
		private static Pen RedPen()
		{
			Pen p = new Pen(Color.Red);
			p.LineJoin = LineJoin.Round;
			p.Width = 5f;
			return p;
		}

		/// <returns>Returns a 3 pixel wide Blue pen</returns>
		private static Pen BluePen()
		{
			Pen p = new Pen(Color.Blue);
			p.LineJoin = LineJoin.Round;
			p.Width = 3f;
			return p;
		}

		/// <returns>Returns a 7 pixel wide Green pen</returns>
		private static Pen GreenPen()
		{
			Pen p = new Pen(Color.Green);
			p.LineJoin = LineJoin.Round;
			p.Width = 7f;
			return p;
		}

		/// <returns>Returns a 3 pixel wide red dotted pen</returns>
		private static Pen RedDottedPen()
		{
			Pen p = new Pen(Color.Red);
			p.LineJoin = LineJoin.Round;
			p.DashStyle = DashStyle.Dot;
			p.Width = 3f;
			return p;
		}

		/// <returns>Returns a 5 pixel wide red dot-dash pen</returns>
		private static Pen RedDotDashPen()
		{
			Pen p = new Pen(Color.Red);
			p.LineJoin = LineJoin.Round;
			p.Width = 5f;
			p.DashStyle = DashStyle.DashDot;
			p.DashCap = DashCap.Round;
			return p;
		}

		/// <returns>Returns a 7 pixel wide black custom pen</returns>
		private static Pen DoubleLinePen()
		{
			Pen p = new Pen(Color.Black);
			p.CompoundArray = new float[] {0.0f, 0.1f, 0.2f, 0.3f, 0.7f, 0.8f, 0.9f, 1.0f};
			p.LineJoin = LineJoin.Round;
			p.Width = 7f;
			return p;
		}

		/// <returns>Returns a 10 pixel wide red dashed pen with an arrow on one end</returns>
		private static Pen DashedArrowLinePen()
		{
			Pen p = new Pen(Color.Red);
			p.LineJoin = LineJoin.Round;
			p.Width = 10f;
			p.DashStyle = DashStyle.Dash;
			p.EndCap = LineCap.ArrowAnchor;
			p.DashCap = DashCap.Flat;
			return p;
		}
	}
}
