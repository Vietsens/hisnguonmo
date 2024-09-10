using System;

using DevExpress.XtraEditors.DXErrorProvider;

namespace Inventec.Desktop.CustomControl
{
	public class RowColumnErrorEventArgs
		: RowErrorEventArgs
	{
		private readonly string _columnName;

		public RowColumnErrorEventArgs(ErrorInfo info, int rowHandle, string columnName)
			: base(info, rowHandle)
		{
			_columnName = columnName;
		}

		public string ColumnName
		{
			get { return _columnName; }
		}
	}
}