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
using HIS.Desktop.Plugins.BhxhApiSend.Config;
using HIS.Desktop.Plugins.BhxhApiSend.Entity;
using HIS.Desktop.Plugins.BhxhApiSend.Sda;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Login.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BhxhApiSend
{
    public partial class FormBhxhApiSend : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<CategoryTypeADO> categoryTypes;
        CategoryTypeADO selectedCategoryType;
        #endregion

        #region Constructor
        public FormBhxhApiSend(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.currentModule = moduleData;
            try
            {
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private methods

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(
                    ApplicationStoreLocation.ApplicationStartupPath,
                    ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FormBhxhApiSend_Load(object sender, EventArgs e)
        {
            try
            {
                BhxhApiSendCFG.LoadConfig();
                InitComboBoxCategoryType();
                ClearResult();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboBoxCategoryType()
        {
            try
            {
                categoryTypes = CategoryTypeADO.GetAll();
                cboLoaiMau.Properties.Items.Clear();
                foreach (var item in categoryTypes)
                {
                    cboLoaiMau.Properties.Items.Add(item.DisplayName);
                }
                if (categoryTypes.Count > 0)
                {
                    cboLoaiMau.SelectedIndex = 0;
                    selectedCategoryType = categoryTypes[0];
                    UpdateKyQTVisibility();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboLoaiMau_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboLoaiMau.SelectedIndex >= 0 && cboLoaiMau.SelectedIndex < categoryTypes.Count)
                {
                    selectedCategoryType = categoryTypes[cboLoaiMau.SelectedIndex];
                    UpdateKyQTVisibility();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateKyQTVisibility()
        {
            try
            {
                bool visible = selectedCategoryType != null && selectedCategoryType.RequireKyQT;
                lciKyQT.Visibility = visible
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnChonFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFile = new OpenFileDialog())
                {
                    openFile.Filter = "XML files (*.xml)|*.xml";
                    openFile.Title = "Chọn file XML đã ký số";
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        txtFilePath.Text = openFile.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void btnGuiBhxh_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (!ValidateBeforeSend())
                    return;

                ClearResult();

                // Show waiting
                this.Cursor = Cursors.WaitCursor;
                btnGuiBhxh.Enabled = false;
                string address = BhxhApiSendCFG.ADDRESS;
                string username = BhxhApiSendCFG.USERNAME;
                string password = BhxhApiSendCFG.PASSWORD;
                string maTinh = BhxhApiSendCFG.MA_TINH;
                string maCsKCB = BhxhApiSendCFG.MA_CSKCB;
                string md5Password = BhxhApiHelper.ConvertStringToMD5(password);
                string kyQT = txtKyQT.Text.Trim();

                // Doc file XML
                byte[] xmlFileBytes = File.ReadAllBytes(txtFilePath.Text.Trim());

                // Xac thuc
                BhxhTokenResultADO tokenResult = await BhxhApiHelper.Authenticate(address, username, md5Password);

                if (tokenResult == null || tokenResult.APIKey == null || tokenResult.maKetQua != "200")
                {
                    string errMsg = "Xác thực thất bại.";
                    if (tokenResult != null)
                    {
                        errMsg += " Mã kết quả: " + (tokenResult.maKetQua ?? "null");
                    }
                    txtMaKetQua.Text = tokenResult != null ? tokenResult.maKetQua : "";
                    txtThongDiep.Text = errMsg;
                    XtraMessageBox.Show(errMsg, "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gui danh muc
                BhxhCategoryResultADO sendResult = await BhxhApiHelper.SendCategory(
                    address,
                    tokenResult,
                    selectedCategoryType,
                    username,
                    md5Password,
                    maTinh,
                    maCsKCB,
                    xmlFileBytes,
                    kyQT);

                // Neu token het han (401/402/403) -> refresh token va retry 1 lan
                if (sendResult != null &&
                    (sendResult.maKetQua == "401" || sendResult.maKetQua == "402" || sendResult.maKetQua == "403"))
                {
                    LogSystem.Info("BhxhApiSend - Token het han, refresh va retry...");
                    BhxhApiHelper.ClearTokenCache();
                    tokenResult = await BhxhApiHelper.Authenticate(address, username, md5Password);

                    if (tokenResult != null && tokenResult.APIKey != null && tokenResult.maKetQua == "200")
                    {
                        sendResult = await BhxhApiHelper.SendCategory(
                            address,
                            tokenResult,
                            selectedCategoryType,
                            username,
                            md5Password,
                            maTinh,
                            maCsKCB,
                            xmlFileBytes,
                            kyQT);
                    }
                }

                // Hien thi ket qua
                DisplayResult(sendResult);

                if (sendResult != null && sendResult.maKetQua == "200")
                {
                    // Ghi lich su tac dong qua SDA
                    LogEventToSda(sendResult);
                    XtraMessageBox.Show("Gửi thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string errMsg = "Gửi thất bại";
                    if (sendResult != null && !string.IsNullOrEmpty(sendResult.thongDiep))
                    {
                        errMsg += ": " + sendResult.thongDiep;
                    }
                    XtraMessageBox.Show(errMsg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnGuiBhxh.Enabled = true;
            }
        }

        private bool ValidateBeforeSend()
        {
            try
            {
                // Kiem tra cau hinh
                if (string.IsNullOrEmpty(BhxhApiSendCFG.ADDRESS)
                    || string.IsNullOrEmpty(BhxhApiSendCFG.USERNAME)
                    || string.IsNullOrEmpty(BhxhApiSendCFG.PASSWORD))
                {
                    XtraMessageBox.Show("Chưa cấu hình tài khoản Cổng BHXH", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (string.IsNullOrEmpty(BhxhApiSendCFG.MA_TINH) || string.IsNullOrEmpty(BhxhApiSendCFG.MA_CSKCB))
                {
                    XtraMessageBox.Show("Chưa cấu hình Mã tỉnh hoặc Mã CSKCB", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiem tra chon file
                if (string.IsNullOrEmpty(txtFilePath.Text.Trim()))
                {
                    XtraMessageBox.Show("Vui lòng chọn file XML để gửi", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFilePath.Focus();
                    return false;
                }

                // Kiem tra file ton tai va la XML
                string filePath = txtFilePath.Text.Trim();
                if (!File.Exists(filePath))
                {
                    XtraMessageBox.Show("File không tồn tại: " + filePath, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".xml")
                {
                    XtraMessageBox.Show("File phải có định dạng .xml", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiem tra loai mau
                if (selectedCategoryType == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn loại mẫu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiem tra ky quyet toan (chi cho Mau 01/BH)
                if (selectedCategoryType.RequireKyQT)
                {
                    string kyQT = txtKyQT.Text.Trim();
                    if (string.IsNullOrEmpty(kyQT) || kyQT.Length != 6)
                    {
                        XtraMessageBox.Show("Kỳ quyết toán phải gồm 6 ký tự (VD: 202604)", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtKyQT.Focus();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void DisplayResult(BhxhCategoryResultADO result)
        {
            try
            {
                if (result != null)
                {
                    txtMaKetQua.Text = result.maKetQua ?? "";
                    txtMaGiaoDich.Text = result.maGiaoDich ?? "";
                    txtThongDiep.Text = result.thongDiep ?? "";
                    txtThoiGianTN.Text = result.thoiGianTiepNhan ?? "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ClearResult()
        {
            try
            {
                txtMaKetQua.Text = "";
                txtMaGiaoDich.Text = "";
                txtThongDiep.Text = "";
                txtThoiGianTN.Text = "";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LogEventToSda(BhxhCategoryResultADO result)
        {
            try
            {
                string jsonResponse = JsonConvert.SerializeObject(result);
                string logContent = string.Format("Gửi thông tin {0}. {1}", selectedCategoryType.LogName, jsonResponse);

                // Gioi han 4000 ky tu theo SDA_EVENT_LOG.DESCRIPTION
                if (logContent.Length > 4000)
                {
                    logContent = logContent.Substring(0, 4000);
                }

                string loginName = ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                SdaEventLogCreate eventLog = new SdaEventLogCreate();
                eventLog.Create(loginName, null, true, logContent);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
