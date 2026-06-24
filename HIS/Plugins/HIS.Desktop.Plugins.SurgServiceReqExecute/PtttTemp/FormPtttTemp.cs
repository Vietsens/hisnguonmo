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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.PtttTemp
{
    public partial class FormPtttTemp : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module Module;
        private HIS_SERE_SERV_PTTT_TEMP TempData;
        private List<ImageADO> images;
        private int positionHandle = -1;

        public FormPtttTemp(Inventec.Desktop.Common.Modules.Module _module, HIS_SERE_SERV_PTTT_TEMP tempData)
            : this(_module, tempData, null)
        {
        }

        public FormPtttTemp(Inventec.Desktop.Common.Modules.Module _module, HIS_SERE_SERV_PTTT_TEMP tempData, List<ImageADO> images)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.Module = _module;
                this.TempData = tempData;
                this.images = images;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormPtttTemp_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                ValidateForm();

                txtPtttTempCode.Focus();
                txtPtttTempCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ControlMaxLengthValidationRule ptttTempCodeValidate = new ControlMaxLengthValidationRule();
                ptttTempCodeValidate.editor = txtPtttTempCode;
                ptttTempCodeValidate.maxLength = 50;
                ptttTempCodeValidate.IsRequired = true;
                ptttTempCodeValidate.ErrorText = string.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, "50");
                ptttTempCodeValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTempCode, ptttTempCodeValidate);

                ControlMaxLengthValidationRule ptttTempNameValidate = new ControlMaxLengthValidationRule();
                ptttTempNameValidate.editor = txtPtttTempName;
                ptttTempNameValidate.maxLength = 500;
                ptttTempNameValidate.IsRequired = true;
                ptttTempNameValidate.ErrorText = string.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, "500");
                ptttTempNameValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTempName, ptttTempNameValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPtttTempName.Focus();
                    txtPtttTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttTempName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPublic.Focus();
                    chkPublic.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPublic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPublicDepartment.Focus();
                    chkPublicDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPublicDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (TempData == null)
                {
                    MessageBox.Show(ResourceMessage.KhongCoDuLieuMau);
                    return;
                }

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                CommonParam param = new CommonParam();
                HIS_SERE_SERV_PTTT_TEMP ptttTemp = new HIS_SERE_SERV_PTTT_TEMP();

                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT_TEMP>(ptttTemp, this.TempData);

                ptttTemp.SERE_SERV_PTTT_TEMP_CODE = txtPtttTempCode.Text.Trim();
                ptttTemp.SERE_SERV_PTTT_TEMP_NAME = txtPtttTempName.Text.Trim();
                ptttTemp.IS_PUBLIC = chkPublic.Checked ? (short?)1 : null;

                long? departmentId = GetCurrentDepartmentId();
                if (this.chkPublicDepartment.Checked)
                {
                    ptttTemp.DEPARTMENT_ID = departmentId;
                    ptttTemp.IS_PUBLIC_IN_DEPARTMENT = chkPublicDepartment.Checked ? (short?)1 : null;
                }

                WaitingManager.Show();

                // Build danh sách ID thư viện ảnh (lược đồ) -> gán vào TEXT_LIB_IDS.
                // Nếu lưu lược đồ thất bại thì DỪNG, không tạo mẫu.
                if (!BuildTextLibIds(param, departmentId, ptttTemp))
                {
                    WaitingManager.Hide();
                    return;
                }

                var ptttTempRS = new BackendAdapter(param).Post<HIS_SERE_SERV_PTTT_TEMP>("api/HisSereServPtttTemp/Create", ApiConsumers.MosConsumer, ptttTemp, param);
                WaitingManager.Hide();
                if (ptttTempRS != null)
                {
                    success = true;
                    this.Close();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy khoa hiện tại theo phòng làm việc (Module.RoomId).
        /// </summary>
        private long? GetCurrentDepartmentId()
        {
            long? departmentId = null;
            try
            {
                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                    .FirstOrDefault(o => o.RoomId == this.Module.RoomId);
                if (workPlace != null)
                {
                    departmentId = workPlace.DepartmentId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return departmentId;
        }

        /// <summary>
        /// Duyệt danh sách ảnh đính kèm dịch vụ, dựng danh sách ID thư viện ảnh và gán vào TEXT_LIB_IDS.
        /// - Ảnh đã biết TextLibId -> dùng lại ID đó.
        /// - Ảnh chưa biết -> đọc bytes (stream runtime hoặc tải từ URL file đính kèm),
        ///   encode base64 -> bytes UTF-8, tạo bản ghi HIS_TEXT_LIB loại ảnh (public trong khoa).
        /// Trả về false nếu lưu lược đồ thất bại (gọi nơi gọi sẽ dừng quá trình lưu mẫu).
        /// </summary>
        private bool BuildTextLibIds(CommonParam param, long? departmentId, HIS_SERE_SERV_PTTT_TEMP ptttTemp)
        {
            try
            {
                if (this.images == null || this.images.Count == 0)
                    return true;

                List<string> textLibIds = new List<string>();
                foreach (var image in this.images)
                {
                    if (image == null)
                        continue;

                    // Đã biết ID thư viện gốc -> dùng lại
                    if (image.TextLibId.HasValue && image.TextLibId.Value > 0)
                    {
                        textLibIds.Add(image.TextLibId.Value.ToString());
                        continue;
                    }

                    // Chưa biết -> đọc bytes ảnh
                    byte[] imageBytes = GetImageBytes(image);
                    if (imageBytes == null || imageBytes.Length == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(ResourceMessage.LuuLuocDoThatBaiKhongTheLuuMau);
                        return false;
                    }

                    HIS_TEXT_LIB textLib = new HIS_TEXT_LIB();
                    textLib.LIB_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__IMAGE;
                    textLib.TITLE = !string.IsNullOrEmpty(image.SERE_SERV_FILE_NAME) ? image.SERE_SERV_FILE_NAME : image.FileName;
                    textLib.CONTENT = Encoding.UTF8.GetBytes(Convert.ToBase64String(imageBytes));
                    textLib.IS_PUBLIC_IN_DEPARTMENT = 1;
                    textLib.DEPARTMENT_ID = departmentId;

                    var textLibRS = new BackendAdapter(param).Post<HIS_TEXT_LIB>("api/HisTextLib/Create", ApiConsumers.MosConsumer, textLib, param);
                    if (textLibRS != null && textLibRS.ID > 0)
                    {
                        image.TextLibId = textLibRS.ID;
                        textLibIds.Add(textLibRS.ID.ToString());
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(ResourceMessage.LuuLuocDoThatBaiKhongTheLuuMau);
                        return false;
                    }
                }

                // Loại bỏ trùng -> ghép thành chuỗi phân cách dấu phẩy
                var distinctIds = textLibIds.Distinct().ToList();
                if (distinctIds.Count > 0)
                {
                    ptttTemp.TEXT_LIB_IDS = string.Join(",", distinctIds);
                }

                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show(ResourceMessage.LuuLuocDoThatBaiKhongTheLuuMau);
                return false;
            }
        }

        /// <summary>
        /// Đọc bytes ảnh từ ADO: ưu tiên stream runtime, nếu không có thì tải từ URL file đính kèm.
        /// </summary>
        private byte[] GetImageBytes(ImageADO image)
        {
            byte[] bytes = null;
            try
            {
                if (image.streamImage != null)
                {
                    MemoryStream msStream = image.streamImage as MemoryStream;
                    if (msStream != null)
                    {
                        bytes = msStream.ToArray();
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.streamImage.Position = 0;
                            image.streamImage.CopyTo(ms);
                            bytes = ms.ToArray();
                        }
                    }
                }

                // Fallback: tải từ URL file đính kèm
                if ((bytes == null || bytes.Length == 0) && !string.IsNullOrEmpty(image.URL))
                {
                    MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(image.URL);
                    if (stream != null && stream.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.Position = 0;
                            stream.CopyTo(ms);
                            bytes = ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return bytes;
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormPtttTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormPtttTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.chkPublicDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublicDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.lciPtttTempCode.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempCode.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.lciPtttTempName.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempName.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttTemp, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
