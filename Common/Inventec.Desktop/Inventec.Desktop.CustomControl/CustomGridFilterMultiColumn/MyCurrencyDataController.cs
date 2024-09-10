using System;

using DevExpress.Data;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Inventec.Desktop.CustomControl
{
	public class MyCurrencyDataController
		: CurrencyDataController
	{
		private readonly MyGridView _view;

		public MyCurrencyDataController(MyGridView view)
		{
			_view = view;
		}

		public override ErrorInfo GetErrorInfo(int controllerRow)
		{
			var info = base.GetErrorInfo(controllerRow);
			_view.FillRowError(controllerRow, info);
			return info;
		}

		public override ErrorInfo GetErrorInfo(int controllerRow, int column)
		{
			var info = base.GetErrorInfo(controllerRow, column);

			if (column < 0 || column >= Columns.Count)
			{
				return info;
			}
			DataColumnInfo dataColumnInfo = Columns[column];
			_view.FillRowColumnError(controllerRow, dataColumnInfo.Name, info);
			return info;
		}
	}
}
