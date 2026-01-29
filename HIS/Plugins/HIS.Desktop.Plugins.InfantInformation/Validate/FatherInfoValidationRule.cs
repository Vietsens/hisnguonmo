using System;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.InfantInformation.Validate
{
    /// <summary>
    /// Nếu nhập 1 trong các trường thông tin cha thì bắt buộc nhập đủ tất cả.
    /// Ngược lại, nếu không nhập trường nào thì không bắt buộc.
    /// Control nào đã có dữ liệu thì không hiện lỗi bắt buộc trên control đó.
    /// </summary>
    internal class FatherInfoValidationRule : ValidationRule
    {
        internal TextEdit TxtFatherName;
        internal DateEdit DteFatherDob;
        internal GridLookUpEdit CboFatherEthnicity;
        internal TextEdit TxtFatherCccd;
        internal DateEdit DteFatherCccdDate;
        internal TextEdit TxtFatherCccdPlace;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            try
            {
                string fatherName = (TxtFatherName == null ? null : TxtFatherName.Text).Trim();
                bool hasDob = DteFatherDob != null && DteFatherDob.EditValue != null && !string.IsNullOrEmpty(DteFatherDob.EditValue.ToString());
                object fatherEthnicValue = CboFatherEthnicity != null ? CboFatherEthnicity.EditValue : null;
                string fatherCccd = (TxtFatherCccd == null ? null : TxtFatherCccd.Text).Trim();
                bool hasCccdDate = DteFatherCccdDate != null && DteFatherCccdDate.EditValue != null && !string.IsNullOrEmpty(DteFatherCccdDate.EditValue.ToString());
                string fatherCccdPlace = (TxtFatherCccdPlace == null ? null : TxtFatherCccdPlace.Text).Trim();

                bool hasAny =
                    !string.IsNullOrEmpty(fatherName) ||
                    hasDob ||
                    fatherEthnicValue != null ||
                    !string.IsNullOrEmpty(fatherCccd) ||
                    hasCccdDate ||
                    !string.IsNullOrEmpty(fatherCccdPlace);

                // Không nhập gì ở phần cha → không bắt buộc
                if (!hasAny)
                {
                    ErrorText = string.Empty;
                    ErrorType = ErrorType.None;
                    return true;
                }

                // Đã nhập ít nhất 1 → kiểm tra đủ 6 trường
                bool missingName = string.IsNullOrEmpty(fatherName);
                bool missingDob = !hasDob;
                bool missingEthnic = fatherEthnicValue == null;
                bool missingCccd = string.IsNullOrEmpty(fatherCccd);
                bool missingCccdDate = !hasCccdDate;
                bool missingCccdPlace = string.IsNullOrEmpty(fatherCccdPlace);

                bool hasAll = !missingName && !missingDob && !missingEthnic && !missingCccd && !missingCccdDate && !missingCccdPlace;

                // Nếu đã đủ tất cả các trường, tiếp tục validate định dạng CMND/CCCD/Hộ chiếu
                if (hasAll && control == (System.Windows.Forms.Control)TxtFatherCccd)
                {
                    bool validId = ValidateIdCard(TxtFatherCccd, true);
                    if (!validId)
                    {
                        // Lỗi định dạng CCCD/CMND/Hộ chiếu → form không hợp lệ
                        return false;
                    }

                    // Đúng định dạng
                    ErrorText = string.Empty;
                    ErrorType = ErrorType.None;
                    return true;
                }

                // Đủ hết (và control hiện tại không phải TxtFatherCccd) → hợp lệ
                if (hasAll)
                {
                    ErrorText = string.Empty;
                    ErrorType = ErrorType.None;
                    return true;
                }

                // Còn thiếu ít nhất 1 trường → form không hợp lệ,
                // nhưng chỉ hiển thị lỗi trên control đang thiếu.
                bool isCurrentMissing = false;

                if (control == (System.Windows.Forms.Control)TxtFatherName)
                    isCurrentMissing = missingName;
                else if (control == (System.Windows.Forms.Control)DteFatherDob)
                    isCurrentMissing = missingDob;
                else if (control == (System.Windows.Forms.Control)CboFatherEthnicity)
                    isCurrentMissing = missingEthnic;
                else if (control == (System.Windows.Forms.Control)TxtFatherCccd)
                    isCurrentMissing = missingCccd;
                else if (control == (System.Windows.Forms.Control)DteFatherCccdDate)
                    isCurrentMissing = missingCccdDate;
                else if (control == (System.Windows.Forms.Control)TxtFatherCccdPlace)
                    isCurrentMissing = missingCccdPlace;

                if (isCurrentMissing)
                {
                    ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                        LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = ErrorType.Warning;
                }
                else
                {
                    ErrorText = string.Empty;
                    ErrorType = ErrorType.None;
                }

                // Vẫn trả về false để dxValidationProvider biết form chưa hợp lệ
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Có lỗi trong quá trình validate → cho qua để không chặn lưu do lỗi code
                return true;
            }
        }

        /// <summary>
        /// Validate CMND/CCCD/Hộ chiếu cho TextEdit chỉ định.
        /// </summary>
        private bool ValidateIdCard(TextEdit textEdit, bool isValid)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                if (String.IsNullOrEmpty(textEdit.Text.Trim()) && isValid)
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }

                if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsNumber(textEdit.Text.Trim()) && textEdit.Text.Trim().Length == 9)
                {
                    //La CMND
                    return true;

                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsNumber(textEdit.Text.Trim()) && textEdit.Text.Trim().Length == 12)
                {
                    //La CCCD
                    return true;
                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsValid(textEdit.Text.Trim()) && textEdit.Text.Trim().Length < 10)
                {
                    //La passport
                    return true;
                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()))
                {
                    this.ErrorText = "CMND/CCCD/Hộ chiếu không đúng định dạng";
                    this.ErrorType = ErrorType.Warning;
                    return false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^\d+$");
            Inventec.Common.Logging.LogSystem.Debug("regex.IsMatch(pText)_" + regex.IsMatch(pText));
            return regex.IsMatch(pText);
        }

        private bool IsValid(string txtCMND)
        {
            bool valid = false;
            var txt = txtCMND;
            int countNumber = 0;
            int total = txt.Length;
            for (int i = 0; i < txt.Length; i++)
            {
                if (IsNumber(txt[i].ToString()))
                {
                    countNumber++;
                }
            }
            if (countNumber == 0)
            {
                valid = false;
            }
            else if (countNumber != 0 && countNumber < total)
            {
                valid = true;
            }
            return valid;
        }
    }
}