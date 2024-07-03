using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Loader
{
 public class TreatmentTypeLoader
 {
  public static void LoadDataToComboTreatmentType(DevExpress.XtraEditors.LookUpEdit cboTreatmentType, object data)
  {
   try
   {
    cboTreatmentType.Properties.DataSource = data;
    cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
    cboTreatmentType.Properties.ValueMember = "ID";
    cboTreatmentType.Properties.ForceInitialize();
    cboTreatmentType.Properties.Columns.Clear();
    cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_CODE", "", 100));
    cboTreatmentType.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_NAME", "", 200));
    cboTreatmentType.Properties.ShowHeader = false;
    cboTreatmentType.Properties.ImmediatePopup = true;
    cboTreatmentType.Properties.DropDownRows = 10;
    cboTreatmentType.Properties.PopupWidth = 300;
   }
   catch (Exception ex)
   {
    Inventec.Common.Logging.LogSystem.Warn(ex);
   }
  }

  public static void LoadDataToComboTreatmentType(DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType, object data)
  {
   try
   {
    cboTreatmentType.Properties.DataSource = data;
    cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
    cboTreatmentType.Properties.ValueMember = "ID";

    cboTreatmentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
    cboTreatmentType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
    cboTreatmentType.Properties.ImmediatePopup = true;
    cboTreatmentType.ForceInitialize();
    cboTreatmentType.Properties.View.Columns.Clear();

    GridColumn aColumnCode = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
    aColumnCode.Caption = "Mã";
    aColumnCode.Visible = true;
    aColumnCode.VisibleIndex = 1;
    aColumnCode.Width = 50;

    GridColumn aColumnName = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
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
