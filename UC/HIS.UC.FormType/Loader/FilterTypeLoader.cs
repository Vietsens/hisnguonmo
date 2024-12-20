using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.UC.FormType.Core.MultiFilter_F16__;
using HIS.UC.FormType.MultiFilter_F16__.HisMultiGetByString;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Loader
{
    class FilterTypeLoader
    {
        
        public static void LoadDataToComboFilterType(DevExpress.XtraEditors.LookUpEdit cboFilterType,string JSONOUTPUT)
        {
            try
            {
                FilterFDO FDO = new FilterFDO();
                FDO = HisFilterTypes(JSONOUTPUT);

                cboFilterType.Properties.DataSource = HisMultiGetByString.GetByString(FDO.FilterTypeCode);
                cboFilterType.Properties.DisplayMember = "FilterTypeName";
                cboFilterType.Properties.ValueMember = "Id";
                cboFilterType.Properties.ForceInitialize();
                cboFilterType.Properties.Columns.Clear();
                cboFilterType.Properties.Columns.Add(new LookUpColumnInfo("FilterTypeCode", "", 100));
                cboFilterType.Properties.Columns.Add(new LookUpColumnInfo("FilterTypeName", "", 200));
                cboFilterType.Properties.ShowHeader = false;
                cboFilterType.Properties.ImmediatePopup = true;
                cboFilterType.Properties.DropDownRows = 10;
                cboFilterType.Properties.PopupWidth = 300;
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboServiceType,string JSONOUTPUT)
        {
            try
            {
                FilterFDO FDO = new FilterFDO();
                FDO = HisFilterTypes(JSONOUTPUT);
                cboServiceType.Properties.DataSource = HisMultiGetByString.GetByString(FDO.FilterTypeCode);
                cboServiceType.Properties.DisplayMember = "FilterTypeName";
                cboServiceType.Properties.ValueMember = "Id";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("FilterTypeCode");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("FilterTypeName");
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
