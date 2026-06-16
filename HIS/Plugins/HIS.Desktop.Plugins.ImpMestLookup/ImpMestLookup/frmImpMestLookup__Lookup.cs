/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.ImpMestLookup.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestLookup.ImpMestLookup
{
    public partial class frmImpMestLookup : HIS.Desktop.Utility.FormBase
    {
        #region Mode UI

        /// <summary>
        /// Thiết lập giao diện theo chế độ vận hành (tra cứu / xem chi tiết).
        /// Cả 2 chế độ đều ẩn các nút thay đổi dữ liệu (Lưu/Duyệt/Thực nhập/Hội đồng kiểm nhập).
        /// </summary>
        private void InitModeUI()
        {
            try
            {
                // Ẩn toàn bộ nút thay đổi dữ liệu - plugin chỉ tra cứu/xem
                HideLayoutButton(btnSave);
                HideLayoutButton(btnApproval);
                HideLayoutButton(btnImport);
                HideLayoutButton(btnHoiDongKiemNhap);

                // Nhãn ô Mã nhập + nút Làm mới theo ngôn ngữ
                lblMaNhapSearch.Text = Inventec.Common.Resource.Get.Value(
                    "frmImpMestLookup.lblImpMestCode.Text",
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                btnReset.Text = Inventec.Common.Resource.Get.Value(
                    "frmImpMestLookup.btnReset.Text",
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                if (isLookupMode)
                {
                    // Chế độ tra cứu: tiêu đề "Tra cứu phiếu nhập", cho nhập Mã nhập, hiện nút Làm mới
                    this.Text = Inventec.Common.Resource.Get.Value(
                        "frmImpMestLookup.Text.Lookup",
                        Resources.ResourceLanguageManager.LanguageResource,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    txtImpMestCode.Properties.ReadOnly = false;
                    txtImpMestCode.Enabled = true;
                    btnReset.Visible = true;
                }
                else
                {
                    // Chế độ xem chi tiết: tiêu đề "Chi tiết nhập", Mã nhập chỉ đọc, ẩn nút Làm mới
                    this.Text = Inventec.Common.Resource.Get.Value(
                        "frmImpMestLookup.Text",
                        Resources.ResourceLanguageManager.LanguageResource,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    txtImpMestCode.Properties.ReadOnly = true;
                    btnReset.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ẩn 1 nút nằm trong LayoutControl (ẩn cả layout item để không để lại khoảng trống).
        /// </summary>
        private void HideLayoutButton(Control ctrl)
        {
            try
            {
                if (ctrl == null) return;
                var item = layoutControl2.GetItemByControl(ctrl) as DevExpress.XtraLayout.LayoutControlItem;
                if (item != null)
                    item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                ctrl.Visible = false;
                ctrl.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đưa con trỏ về ô Mã nhập. Defer bằng BeginInvoke vì gọi Focus() ngay trong
        /// sự kiện Load không có tác dụng (form chưa hiển thị) - mặc định focus sẽ rơi
        /// vào control đầu tiên theo TabIndex (ô Số hóa đơn trong layoutControl1).
        /// </summary>
        private void FocusImpMestCode()
        {
            try
            {
                this.ActiveControl = txtImpMestCode;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        txtImpMestCode.Focus();
                        txtImpMestCode.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Lookup process

        /// <summary>
        /// Người dùng nhập Mã nhập + Enter ở chế độ tra cứu.
        /// </summary>
        private void txtImpMestCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;
                if (!isLookupMode) return;
                DoLookupByCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tra cứu phiếu nhập theo Mã nhập (so khớp chính xác) rồi đổ dữ liệu lên màn hình.
        /// </summary>
        private void DoLookupByCode()
        {
            CommonParam param = new CommonParam();
            try
            {
                // 1. Cắt khoảng trắng đầu/cuối
                string code = (txtImpMestCode.Text ?? "").Trim();
                txtImpMestCode.Text = code;

                // 2. Rỗng -> cảnh báo trường bắt buộc, không gọi tra cứu
                if (string.IsNullOrEmpty(code))
                {
                    dxErrorProvider1.SetError(txtImpMestCode,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                        ErrorType.Warning);
                    txtImpMestCode.Focus();
                    return;
                }
                dxErrorProvider1.SetError(txtImpMestCode, "", ErrorType.None);

                // 3. < 12 ký tự và toàn chữ số -> pad zero thành 12 ký tự
                if (code.Length < 12 && CheckDigit(code))
                {
                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtImpMestCode.Text = code;
                }

                // 4. Tra cứu theo Mã nhập (so khớp chính xác)
                WaitingManager.Show();
                HisImpMestViewFilter filter = new HisImpMestViewFilter();
                filter.IMP_MEST_CODE__EXACT = code;
                var impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>(
                    HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                SessionManager.ProcessTokenLost(param);

                // 5. Không tìm thấy -> thông báo, giữ nguyên ô Mã nhập, không xáo trộn vùng chi tiết
                if (impMests == null || impMests.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.KhongTimThayPhieuNhapCoMa, code),
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtImpMestCode.Focus();
                    txtImpMestCode.SelectAll();
                    return;
                }

                // 6. Tìm thấy -> gán thông tin và đổ dữ liệu
                var found = impMests.FirstOrDefault();
                this.ImpMestId = found.ID;
                this.IMP_MEST_TYPE_ID = found.IMP_MEST_TYPE_ID;
                this.ImpMestSttId = found.IMP_MEST_STT_ID;
                LoadImpMestData();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Người dùng ấn Làm mới: xóa Mã nhập, dọn vùng chi tiết, focus về ô Mã nhập.
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtImpMestCode.Text = "";
                dxErrorProvider1.SetError(txtImpMestCode, "", ErrorType.None);
                ClearAllData();
                txtImpMestCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra chuỗi toàn bộ là chữ số.
        /// </summary>
        private bool CheckDigit(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char c in value)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        #endregion

        #region Load / Clear data

        /// <summary>
        /// Đổ đầy đủ thông tin phiếu nhập (thông tin chung + 3 tab Thuốc/Vật tư/Máu) theo ImpMestId hiện tại.
        /// </summary>
        private void LoadImpMestData()
        {
            try
            {
                SetDataToCommonControl();
                if (impMest == null) return;

                txtImpMestCode.Text = impMest.IMP_MEST_CODE;

                EnableGridColumn(IsAdmin);
                LoadMobaExpMest();
                loadDataToGridMaterial();
                loadDataToGridMedicine();

                medicineTypes = new List<V_HIS_MEDICINE_TYPE>();
                materialTypes = new List<V_HIS_MATERIAL_TYPE>();
                if (this.impMestMedicines != null && this.impMestMedicines.Count() > 0)
                {
                    var bidIdMedicines = this.impMestMedicines.Select(o => o.BID_ID ?? 0).Distinct().ToList();
                    var medicineTypeIds = this.impMestMedicines.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    medicineTypes = FilterMedicineType(bidIdMedicines, medicineTypeIds);
                    InitComboMedicineType(medicineTypes);
                }

                if (this.impMestMaterials != null && this.impMestMaterials.Count() > 0)
                {
                    var bidIdMaterials = this.impMestMaterials.Select(o => o.BID_ID ?? 0).Distinct().ToList();
                    var materialTypeIds = this.impMestMaterials.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                    materialTypes = FilterMaterialType(bidIdMaterials, materialTypeIds);
                    InitComboMaterialType(materialTypes);
                }

                LoadDataToGridBlood();
                ShowTab();
                cboPrint.Enabled = true;
                InitMenuToButtonPrint();
                ValidateControlForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dọn toàn bộ vùng thông tin chung và 3 tab Thuốc/Vật tư/Máu về trạng thái trống.
        /// </summary>
        private void ClearAllData()
        {
            try
            {
                this.ImpMestId = 0;
                this.IMP_MEST_TYPE_ID = 0;
                this.ImpMestSttId = 0;
                this.impMest = null;
                this.impMestMedicines = null;
                this.impMestMaterials = null;
                this.impMestBloods = null;

                lblImpMestCode.Text = "";
                lblImpMedistock.Text = "";
                lblImpUserName.Text = "";
                lblImpTime.Text = "";
                TxtDocumentNumber.Text = "";
                SpDocumentPrice.EditValue = null;
                SpDocumentVatPrice.EditValue = null;
                TxtDeliverer.Text = "";
                txtDescription.Text = "";
                SpDiscount.EditValue = null;
                SpDiscountRatio.EditValue = null;
                txtDocumentDate.Text = null;
                dtDocumentDate.EditValue = null;

                gridControlMedicine.DataSource = null;
                gridControlMaterial.DataSource = null;
                gridControlBlood.DataSource = null;

                cboPrint.DropDownControl = null;
                cboPrint.Enabled = false;

                dxValidationProvider1.RemoveControlError(TxtDocumentNumber);
                dxValidationProvider1.RemoveControlError(dtDocumentDate);
                dxValidationProvider1.RemoveControlError(txtDocumentDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
