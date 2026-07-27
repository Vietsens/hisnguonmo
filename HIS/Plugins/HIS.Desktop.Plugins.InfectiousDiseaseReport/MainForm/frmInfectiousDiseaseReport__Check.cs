/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Validate các trường bắt buộc trước khi đẩy. Hiển thị lỗi tại control (DXErrorProvider).
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private bool ValidateForm(out string firstError)
        {
            firstError = null;
            bool valid = true;
            try
            {
                if (dxErr != null) dxErr.ClearErrors();

                // caption hiển thị khi lỗi — control
                var required = new List<KeyValuePair<BaseEdit, string>>
                {
                    new KeyValuePair<BaseEdit, string>(cboBenh, "Chọn bệnh chẩn đoán"),
                    new KeyValuePair<BaseEdit, string>(cboLoaiChanDoan, "Chọn phân loại chẩn đoán"),
                    new KeyValuePair<BaseEdit, string>(cboTinhTrang, "Chọn tình trạng hiện nay"),
                    new KeyValuePair<BaseEdit, string>(dteNgayNhapVien, "Nhập ngày nhập viện"),
                    new KeyValuePair<BaseEdit, string>(txtHoTen, "Nhập họ và tên"),
                    new KeyValuePair<BaseEdit, string>(dteNgaySinh, "Nhập ngày sinh"),
                    new KeyValuePair<BaseEdit, string>(spnTuoi, "Nhập tuổi"),
                    new KeyValuePair<BaseEdit, string>(cboGioiTinh, "Chọn giới tính"),
                    new KeyValuePair<BaseEdit, string>(cboDanToc, "Chọn dân tộc"),
                    new KeyValuePair<BaseEdit, string>(cboNgheNghiep, "Chọn nghề nghiệp"),
                    new KeyValuePair<BaseEdit, string>(txtCccd, "Nhập số CCCD/CMND"),
                    new KeyValuePair<BaseEdit, string>(txtDienThoai, "Nhập số điện thoại"),
                    new KeyValuePair<BaseEdit, string>(cboTinh, "Chọn tỉnh/TP hiện nay"),
                    new KeyValuePair<BaseEdit, string>(cboXa, "Chọn xã/phường hiện nay"),
                    new KeyValuePair<BaseEdit, string>(txtDiaChi, "Nhập địa chỉ chi tiết"),
                    new KeyValuePair<BaseEdit, string>(cboLoaiPhatHien, "Chọn loại cơ sở điều trị"),
                    new KeyValuePair<BaseEdit, string>(txtNguoiBaoCao, "Nhập người báo cáo"),
                    new KeyValuePair<BaseEdit, string>(txtDienThoaiBaoCao, "Nhập SĐT người báo cáo"),
                    new KeyValuePair<BaseEdit, string>(txtEmailBaoCao, "Nhập email người báo cáo"),
                };

                foreach (var item in required)
                {
                    if (item.Key == null) continue;
                    if (IsEmpty(item.Key))
                    {
                        dxErr.SetError(item.Key, item.Value, ErrorType.Critical);
                        if (firstError == null) firstError = item.Value;
                        valid = false;
                    }
                }

                if (!valid)
                {
                    XtraMessageBox.Show(
                        "Vui lòng nhập đầy đủ các trường bắt buộc." +
                        (firstError != null ? "\n- " + firstError : ""),
                        "Thông báo", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool IsEmpty(BaseEdit ctrl)
        {
            if (ctrl.EditValue == null) return true;
            string s = ctrl.EditValue.ToString();
            return string.IsNullOrWhiteSpace(s);
        }
    }
}
