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