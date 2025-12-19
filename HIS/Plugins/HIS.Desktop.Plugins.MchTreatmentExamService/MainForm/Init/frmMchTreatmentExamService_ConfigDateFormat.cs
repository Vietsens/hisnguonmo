using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Cấu hình định dạng DateTime cho tất cả các DateEdit control
        /// Format: dd/MM/yyyy HH:mm
        /// </summary>
        private void ConfigDateTimeFormat()
        {
            try
            {
                // Tab 1: Sàng lọc ung thư cổ tử cung
                ConfigDateEditFormat(dteExam1);
                
                // Tab 2: Khám thai
                ConfigDateEditFormat(dteExam2);
                
                // Tab 3: Sinh đẻ
                ConfigDateEditFormat(dteExam3);
                
                // Tab 4: Tránh thai
                ConfigDateEditFormat(dteExam4);
                
                // Tab 5: Phá thai
                ConfigDateEditFormat(dteExam5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        /// <summary>
        /// Cấu hình định dạng DateTime: dd/MM/yyyy HH:mm
        /// </summary>
        private void ConfigDateEditFormat(DevExpress.XtraEditors.DateEdit dateEdit)
        {
            try
            {
                if (dateEdit == null) return;
                
                // Set properties cho DateEdit
                dateEdit.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                dateEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                dateEdit.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                dateEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                dateEdit.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                dateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
                
                // Cho phép chọn giờ
                dateEdit.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
                dateEdit.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
                dateEdit.Properties.ShowToday = true;
                dateEdit.Properties.ShowClear = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        /// <summary>
        /// Cấu hình định dạng Date chỉ ngày: dd/MM/yyyy
        /// </summary>
        private void ConfigDateOnlyFormat(DevExpress.XtraEditors.DateEdit dateEdit)
        {
            try
            {
                if (dateEdit == null) return;
                
                // Set properties cho DateEdit
                dateEdit.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                dateEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                dateEdit.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                dateEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                dateEdit.Properties.Mask.EditMask = "dd/MM/yyyy";
                dateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
                
                // Không hiển thị giờ
                dateEdit.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
                dateEdit.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.False;
                dateEdit.Properties.ShowToday = true;
                dateEdit.Properties.ShowClear = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
