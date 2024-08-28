using System;

using DevExpress.XtraEditors.DXErrorProvider;

namespace Inventec.Desktop.CustomControl
{
	public class RowErrorEventArgs
		: EventArgs
	{
		private readonly ErrorInfo _info;
		private readonly int _rowHandle;

		public RowErrorEventArgs(ErrorInfo info, int rowHandle)
		{
			_info = info;
			_rowHandle = rowHandle;
		}

		public ErrorInfo Info
		{
			get { return _info; }
		}
		public int RowHandle
		{
			get { return _rowHandle; }
		}
	}
}