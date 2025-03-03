using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Loader
{
    public class ExpMestTypeLoader
    {
        public static void LoadDataToComboExpMestType(DevExpress.XtraEditors.LookUpEdit cboExpMestType)
        {
            try
            {
                cboExpMestType.Properties.DataSource = Config.HisFormTypeConfig.HisExpMestTypes;
                cboExpMestType.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                cboExpMestType.Properties.ValueMember = "ID";
                cboExpMestType.Properties.ForceInitialize();
                cboExpMestType.Properties.Columns.Clear();
                cboExpMestType.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TYPE_CODE", "", 100));
                cboExpMestType.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TYPE_NAME", "", 200));
                cboExpMestType.Properties.ShowHeader = false;
                cboExpMestType.Properties.ImmediatePopup = true;
                cboExpMestType.Properties.DropDownRows = 10;
                cboExpMestType.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = Config.HisFormTypeConfig.HisExpMestTypes;
                cboServiceType.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("EXP_MEST_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
