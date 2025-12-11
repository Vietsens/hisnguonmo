using System;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.HisExecuteRoom.ToolTipService
{
    public class GridBubbleToolTipService
    {
        internal readonly ToolTipController _controller;
        private GridView _gridView;
        private Func<object, string> _messageProvider = _ => string.Empty;

        public GridBubbleToolTipService()
        {
            _controller = new ToolTipController
            {
                AutoPopDelay = 15000,
                InitialDelay = 0,
                ReshowDelay = 250
            };
            _controller.Appearance.Options.UseFont = true;
            _controller.Appearance.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12f, FontStyle.Regular);
            _controller.GetActiveObjectInfo += OnGetActiveObjectInfo;
        }

        public ToolTipController Controller => _controller;

        public void Attach(GridLookUpEdit editor, GridView gridView, Func<object, string> messageProvider)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (gridView == null)
            {
                throw new ArgumentNullException(nameof(gridView));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            _gridView = gridView;
            _messageProvider = messageProvider;

            editor.ToolTipController = _controller;
            if (gridView.GridControl != null)
            {
                gridView.GridControl.ToolTipController = _controller;
            }
        }

        private void OnGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (_gridView == null || !ReferenceEquals(e.SelectedControl, _gridView.GridControl))
            {
                return;
            }

            var hitInfo = _gridView.CalcHitInfo(e.ControlMousePosition);
            if (!hitInfo.InRowCell)
            {
                return;
            }

            var row = _gridView.GetRow(hitInfo.RowHandle);
            var message = _messageProvider(row);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            e.Info = new ToolTipControlInfo($"row_{hitInfo.RowHandle}_col_{hitInfo.Column?.FieldName}", message)
            {
                IconType = ToolTipIconType.Warning,
                AllowHtmlText = DefaultBoolean.True,
                Title = "Lưu ý",
            };
        }
    }
}
