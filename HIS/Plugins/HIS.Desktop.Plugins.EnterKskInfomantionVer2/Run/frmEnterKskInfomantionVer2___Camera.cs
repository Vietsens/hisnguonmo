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
        /// Callback nhận ảnh từ chức năng Camera: hiển thị lên picture và lưu vào ImgAvatarData
        /// qua api/HisPatient/UpdateSdo (giống HIS.Desktop.Plugins.PatientUpdate).
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
                pictureEdit1.Image = img;
                if (img.Tag != null)
                {
                    pictureEdit1.Image.Tag = img.Tag;
                }
                pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;

                byte[] avatarData = ImageToByteArray(img);
                SaveAvatarToPatient(avatarData);
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

        private void SaveAvatarToPatient(byte[] avatarData)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (currentServiceReq == null || currentServiceReq.TDL_PATIENT_ID <= 0 || avatarData == null)
                {
                    return;
                }

                WaitingManager.Show();

                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = currentServiceReq.TDL_PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                if (patients == null || patients.Count == 0)
                {
                    WaitingManager.Hide();
                    return;
                }

                MOS.SDO.HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                patientUpdateSdo.HisPatient = patients[0];
                patientUpdateSdo.ImgAvatarData = avatarData;

                var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, param);
                success = resultData != null;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
        }
    }
}
