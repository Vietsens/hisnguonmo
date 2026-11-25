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
 * GNU General Public License for more details.IS_ADMIN
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2 : HIS.Desktop.Utility.FormBase
    {
        private DevExpress.XtraBars.BarButtonItem bbiTakeAvatar;
        private DevExpress.XtraBars.PopupMenu popupMenuAvatar;

        /// <summary>
        /// Bổ sung menu "Chụp ảnh chân dung" vào context menu của picture ảnh (pictureEdit1).
        /// Xử lý tương tự nút "Chụp ảnh chân dung" của HIS.Desktop.Plugins.PatientUpdate.
        /// </summary>
        private void InitAvatarContextMenu()
        {
            try
            {
                this.popupMenuAvatar = new DevExpress.XtraBars.PopupMenu(this.barManager1);

                this.bbiTakeAvatar = new DevExpress.XtraBars.BarButtonItem(this.barManager1, "Chụp ảnh chân dung");
                this.bbiTakeAvatar.Name = "bbiTakeAvatar";
                this.bbiTakeAvatar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiTakeAvatar_ItemClick);
                this.popupMenuAvatar.AddItems(new DevExpress.XtraBars.BarItem[] { this.bbiTakeAvatar });

                // Tắt menu mặc định của editor, dùng popup tùy chỉnh hiển thị khi chuột phải
                this.pictureEdit1.Properties.ShowMenu = false;
                this.pictureEdit1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit1_MouseDown);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pictureEdit1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && this.popupMenuAvatar != null)
                {
                    this.popupMenuAvatar.ShowPopup(System.Windows.Forms.Control.MousePosition);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbiTakeAvatar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                CallModuleCamera((HIS.Desktop.Common.DelegateSelectData)FillImageAvatarFromModuleCamera);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallModuleCamera(HIS.Desktop.Common.DelegateSelectData delegateSelect)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(delegateSelect);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Camera", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Callback nhận ảnh từ chức năng Camera: lưu vào ImgAvatarData qua api/HisPatient/UpdateSdo
        /// (giống HIS.Desktop.Plugins.PatientUpdate), lưu được thì mới hiển thị lên picture.
        /// </summary>
        internal void FillImageAvatarFromModuleCamera(object dataImage)
        {
            try
            {
                if (dataImage == null)
                {
                    return;
                }

                Image img = (Image)dataImage;
                byte[] avatarData = ImageToByteArray(img);
                // Chỉ hiện ảnh mới khi đã lưu được vào HIS_PATIENT — tránh thấy ảnh trên form nhưng mở lại thì mất.
                if (!SaveAvatarToPatient(avatarData))
                {
                    return;
                }

                pictureEdit1.Image = img;
                pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            try
            {
                MemoryStream memory = new MemoryStream();
                var bitMap = new System.Drawing.Bitmap(imageIn);
                bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memory.ToArray();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Lưu ảnh chân dung vào HIS_PATIENT. Trả về true khi MOS đã lưu được ảnh (có AVATAR_URL trả về).
        /// </summary>
        private bool SaveAvatarToPatient(byte[] avatarData)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (currentServiceReq != null && currentServiceReq.TDL_PATIENT_ID > 0 && avatarData != null)
                {
                    WaitingManager.Show();

                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = currentServiceReq.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                    HIS_PATIENT patient = (patients != null && patients.Count > 0) ? patients[0] : null;
                    if (patient != null)
                    {
                        string oldAvatarUrl = patient.AVATAR_URL;
                        string oldBhytUrl = patient.BHYT_URL;

                        // MOS (HisPatientUpdateInfo.ProcessPatientImage) xóa cả AVATAR_URL lẫn BHYT_URL rồi chỉ gán lại
                        // theo dữ liệu ảnh gửi kèm -> phải gửi kèm lại ảnh thẻ BHYT hiện có, không thì mất ảnh thẻ.
                        byte[] bhytData = null;
                        if (!String.IsNullOrEmpty(oldBhytUrl))
                        {
                            bhytData = DownloadFssFile(oldBhytUrl);
                            if (bhytData == null)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(this,
                                    "Không tải được ảnh thẻ BHYT hiện có của bệnh nhân. Nếu tiếp tục lưu ảnh chân dung thì ảnh thẻ BHYT sẽ bị xóa. Bạn có muốn tiếp tục không?",
                                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                                {
                                    return false;
                                }
                                WaitingManager.Show();
                            }
                        }

                        MOS.SDO.HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                        patientUpdateSdo.HisPatient = patient;
                        patientUpdateSdo.ImgAvatarData = avatarData;
                        patientUpdateSdo.ImgBhytData = bhytData;

                        var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, param);
                        // Đẩy ảnh lên FSS thất bại thì MOS vẫn trả về bệnh nhân (AVATAR_URL rỗng) -> phải có URL mới tính là lưu được.
                        success = resultData != null && !String.IsNullOrEmpty(resultData.AVATAR_URL);
                        if (success)
                        {
                            // Biểu in và lần hiển thị sau lấy ảnh theo TDL_PATIENT_AVATAR_URL của y lệnh đang mở.
                            currentServiceReq.TDL_PATIENT_AVATAR_URL = resultData.AVATAR_URL;
                        }
                        else if (resultData != null && (!String.IsNullOrEmpty(oldAvatarUrl) || !String.IsNullOrEmpty(oldBhytUrl)))
                        {
                            // MOS đã ghi rỗng 2 đường dẫn ảnh cũ trước khi đẩy ảnh mới -> ghi trả lại để không mất ảnh cũ.
                            RestorePatientImageUrl(resultData, oldAvatarUrl, oldBhytUrl);
                        }
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
            return success;
        }

        /// <summary>
        /// Tải nội dung 1 file trên FSS. Trả về null khi không tải được.
        /// </summary>
        private byte[] DownloadFssFile(string url)
        {
            try
            {
                using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(url))
                {
                    if (stream != null && stream.Length > 0)
                    {
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Ghi trả lại AVATAR_URL / BHYT_URL cũ của bệnh nhân khi lưu ảnh mới thất bại
        /// (IsNotUpdateImage = true để MOS không xử lý ảnh, chỉ cập nhật bản ghi).
        /// </summary>
        private void RestorePatientImageUrl(HIS_PATIENT patient, string oldAvatarUrl, string oldBhytUrl)
        {
            try
            {
                patient.AVATAR_URL = oldAvatarUrl;
                patient.BHYT_URL = oldBhytUrl;

                MOS.SDO.HisPatientUpdateSDO restoreSdo = new MOS.SDO.HisPatientUpdateSDO();
                restoreSdo.HisPatient = patient;
                restoreSdo.IsNotUpdateImage = true;

                CommonParam restoreParam = new CommonParam();
                var restored = new BackendAdapter(restoreParam).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, restoreSdo, restoreParam);
                if (restored == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Ghi tra lai duong dan anh cu cua benh nhan that bai. PatientId=" + patient.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
