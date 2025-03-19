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

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;

namespace Inventec.Desktop.CustomControl
{
	public class MyGridInfoRegistrator
		: GridInfoRegistrator
	{
		public override BaseView CreateView(GridControl grid)
		{
			return new MyGridView(grid);
		}

		public override bool IsInternalView
		{
			get { return false; }
		}

		public override string ViewName
		{
			get { return MyGridView.ViewNameValue; }
		}
	}
}
