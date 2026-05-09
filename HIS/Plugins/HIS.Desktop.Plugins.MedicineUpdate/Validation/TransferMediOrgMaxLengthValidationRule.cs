/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;

namespace HIS.Desktop.Plugins.MedicineUpdate.Validation
{
    /// <summary>
    /// Cảnh báo inline (icon Warning trước control) khi giá trị "CSKCB chuyển" vượt 10 ký tự.
    /// Dùng kèm dxValidationProvider — không popup MessageBox.
    /// </summary>
    class TransferMediOrgMaxLengthValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTransferMediOrg;
        internal int maxLength;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTransferMediOrg == null)
                {
                    return true;
                }

                string text = (txtTransferMediOrg.Text ?? "").Trim();

                // Trường không bắt buộc — rỗng vẫn hợp lệ
                if (string.IsNullOrEmpty(text))
                {
                    return true;
                }

                if (text.Length > maxLength)
                {
                    this.ErrorText = Resources.ResourceMessage.MaCSKCBChuyenToiDa10KyTu;
                    return valid;
                }

                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
