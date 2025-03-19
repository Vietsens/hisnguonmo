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
	public class FillBrushes
	{
		#region Enumerations
		public enum BrushType
		{
			Brown,
			Aqua,
			GrayDivot,
			RedDiag,
			ConfettiGreen,
			NoBrush,
			NumberOfBrushes
		} ;
		#endregion Enumerations

		public static Brush SetCurrentBrush(BrushType _bType)
		{
			Brush b = null;
			switch (_bType)
			{
				case BrushType.Aqua:
					b = AquaBrush();
					break;
				case BrushType.Brown:
					b = BrownBrush();
					break;
				case BrushType.ConfettiGreen:
					b = ConfettiBrush();
					break;
				case BrushType.GrayDivot:
					b = GrayDivotBrush();
					break;
				case BrushType.RedDiag:
					b = RedDiagBrush();
					break;
				default:
					break;
			}
			return b;
		}

		private static Brush BrownBrush()
		{
			return new SolidBrush(Color.Brown);
		}

		private static Brush AquaBrush()
		{
			return new SolidBrush(Color.Aqua);
		}

		private static Brush GrayDivotBrush()
		{
			return new HatchBrush(HatchStyle.Divot, Color.Gray, Color.Gainsboro);
		}

		private static Brush RedDiagBrush()
		{
			return new HatchBrush(HatchStyle.ForwardDiagonal, Color.Red, Color.Yellow);
		}

		private static Brush ConfettiBrush()
		{
			return new HatchBrush(HatchStyle.LargeConfetti, Color.Green, Color.White);
		}
	}
}
