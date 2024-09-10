using System;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;

namespace Inventec.Desktop.CustomControl
{
	public class MyGridControl
		: GridControl
	{
		protected override void RegisterAvailableViewsCore(InfoCollection collection)
		{
			base.RegisterAvailableViewsCore(collection);
			collection.Add(new MyGridInfoRegistrator());
		}
	}
}