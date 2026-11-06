using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.SupplyOrderCreate.Resources;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    /// <summary>
    /// Order over the remain amount: list the items, the user must tick "read" before confirming (not blocked)
    /// </summary>
    public partial class frmOverWarning : DevExpress.XtraEditors.XtraForm
    {
        private List<HisSupplyOrderOverWarningSDO> warnings;

        public frmOverWarning()
        {
            InitializeComponent();
        }

        public frmOverWarning(List<HisSupplyOrderOverWarningSDO> warnings)
        {
            InitializeComponent();
            this.warnings = warnings ?? new List<HisSupplyOrderOverWarningSDO>();
        }

        private void frmOverWarning_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = ResourceLanguageManager.Get("frmOverWarning.Text");
                this.lblMessage.Text = ResourceLanguageManager.Get("frmOverWarning.lblMessage.Text");
                this.chkRead.Properties.Caption = ResourceLanguageManager.Get("frmOverWarning.chkRead.Text");
                this.btnOk.Text = ResourceLanguageManager.Get("frmOverWarning.btnOk.Text");
                this.btnCancel.Text = ResourceLanguageManager.Get("frmOverWarning.btnCancel.Text");
                this.btnOk.Enabled = false;

                DevExpress.XtraGrid.Views.Grid.GridView view = this.gridViewWarning;
                view.Columns.Clear();
                int i = 0;
                this.AddColumn("SupplyOrderCode", "frmOverWarning.Column.OrderCode", 100, i++, null);
                this.AddColumn("TypeCode", "frmSupplyOrderCreate.Column.TypeCode", 90, i++, null);
                this.AddColumn("TypeName", "frmSupplyOrderCreate.Column.TypeName", 230, i++, null);
                this.AddColumn("RemainAmount", "frmOverWarning.Column.Remain", 90, i++, "#,##0.####");
                this.AddColumn("OrderAmount", "frmOverWarning.Column.Order", 90, i++, "#,##0.####");
                GridColumn colOver = this.AddColumn("OverAmount", "frmOverWarning.Column.Over", 90, i++, "#,##0.####");
                colOver.AppearanceCell.ForeColor = System.Drawing.Color.Red;
                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.OptionsView.ColumnAutoWidth = false;
                this.gridControlWarning.DataSource = this.warnings;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private GridColumn AddColumn(string fieldName, string captionKey, int width, int visibleIndex, string format)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = ResourceLanguageManager.Get(captionKey);
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            if (!String.IsNullOrEmpty(format))
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            this.gridViewWarning.Columns.Add(col);
            return col;
        }

        private void chkRead_CheckedChanged(object sender, EventArgs e)
        {
            this.btnOk.Enabled = this.chkRead.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.chkRead.Checked)
            {
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
