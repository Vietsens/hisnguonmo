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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.UC.ImageLib.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraBars;
using System.ComponentModel;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        UCCamera ucCamera1;
        UCCameraDXC ucCameraDXC1;
        int maxImage;
        int numberImage;
        int timeCapture;
        long dem;
        bool IsStart;
        bool VisibleOptionCamera;

        private void StartCamera()
        {
            this.panelControlCamera.Controls.Clear();
            if ((int)cboConnectionType.EditValue == 2)
            {
                this.ucCameraDXC1 = new UCCameraDXC(DelegateCaptureImage, camMonikerString);
                this.ucCameraDXC1.Dock = DockStyle.Fill;
                this.panelControlCamera.Controls.Add(this.ucCameraDXC1);
                if (!this.ucCameraDXC1.IsDisposed)
                {
                    this.ucCameraDXC1.Stop();
                }
                this.ucCameraDXC1.Start();
                this.IsStart = true;
                this.ucCameraDXC1.SetDisable();
                this.ucCameraDXC1.VisibleControl(VisibleOptionCamera);
                this.ucCameraDXC1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                this.ucCameraDXC1.SetClientCode(this.currentServiceReq.TDL_TREATMENT_CODE);
            }
            else
            {
                this.ucCamera1 = new UCCamera(DelegateCaptureImage, camMonikerString,true);
                this.ucCamera1.Dock = DockStyle.Fill;
                this.panelControlCamera.Controls.Add(this.ucCamera1);
                if (!this.ucCamera1.IsDisposed)
                {
                    this.ucCamera1.Stop();
                }
                this.ucCamera1.Start();
                this.IsStart = true;
                this.ucCamera1.SetDisable();
                this.ucCamera1.VisibleControl(VisibleOptionCamera);
                this.ucCamera1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                this.ucCamera1.SetClientCode(this.currentServiceReq.TDL_TREATMENT_CODE);
            }

            btnCamera.Enabled = false;
            string shortcutCapture = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY);
            Inventec.Common.Logging.LogSystem.Info("Start Camera 4");
            try
            {
                DevExpress.XtraBars.BarShortcut shortcut1 = new DevExpress.XtraBars.BarShortcut((Keys)Enum.Parse(typeof(Keys), shortcutCapture, true));
                //btnCapture.ItemShortcut = shortcut1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Gan phim tat cho nut chup anh that bai, " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => shortcutCapture), shortcutCapture), ex);
            }

            maxImage = HisConfigs.Get<int>("HIS.Desktop.Plugins.Camera.MaxImage");
            Inventec.Common.Logging.LogSystem.Info("Start Camera 5");
        }

        public void DelegateCaptureImage(Stream stream)
        {
            try
            {
                if (stream != null && stream.Length > 0)
                {

                    ADO.ImageADO image = new ADO.ImageADO();
                    image.FileName = DateTime.Now.ToString("HH:mm:ss:fff");
                    image.streamImage = new MemoryStream();
                    stream.Position = 0;
                    stream.CopyTo(image.streamImage);

                    stream.Position = 0;
                    Image i = Image.FromStream(stream);
                    i.Tag = DateTime.Now.ToString("HHmmssfff");
                    image.IMAGE_DISPLAY = i;
                    image.CREATOR = Loginname;

                    if (AppConfigKeys.IsAutoSelectImageCapture)
                    {
                        image.IsChecked = true;
                        int maxSTT = (listImage != null && listImage.Count > 0) ? listImage.Max(o => o.STTImage ?? 0) : 0;
                        image.STTImage = maxSTT + 1;
                    }

                    //if (GlobalVariables.dicImageCapture == null)
                    //{
                    //    GlobalVariables.dicImageCapture = new Dictionary<string, List<Image>>();
                    //}
                    //if (GlobalVariables.listImage == null)
                    //{
                    //    GlobalVariables.listImage = new List<Image>();
                    //}

                    //string clientCode = currentServiceReq.TDL_TREATMENT_CODE;
                    //List<Image> images;
                    //if (GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                    //{
                    //    images = GlobalVariables.dicImageCapture[clientCode];
                    //    if (images == null)
                    //    {
                    //        images = new List<Image>();
                    //    }
                    //    images.Insert(0, i);
                    //    GlobalVariables.dicImageCapture[clientCode] = images;
                    //}
                    //else
                    //{
                    //    images = new List<Image>();
                    //    images.Insert(0, i);
                    //    GlobalVariables.dicImageCapture.Add(clientCode, images);
                    //}

                    if (chkSaveImageToFile.Checked)
                    {
                        if (!String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage))
                        {
                            SaveImageToFile(image, this.SelectedFolderForSaveImage);
                        }
                        else
                        {
                            if (XtraMessageBox.Show("Bạn chưa chọn đường dẫn lưu ảnh. Bạn có muốn lưu ảnh không?", ResourceMessage.ThongBao, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                SelectFolder();
                            }
                            else
                            {
                                chkSaveImageToFile.Checked = false;
                            }
                        }
                    }

                    //GlobalVariables.listImage.Insert(0, i);

                    //if (GlobalVariables.listImage.Count > maxImage)
                    //{
                    //    GlobalVariables.listImage.RemoveAt(GlobalVariables.listImage.Count - 1);
                    //    if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 1)
                    //    {
                    //        foreach (var itemImg in GlobalVariables.dicImageCapture)
                    //        {
                    //            var imgs = itemImg.Value;
                    //            if (imgs != null && imgs.Count > 0)
                    //            {
                    //                imgs.RemoveAt(0);
                    //                Inventec.Common.Logging.LogSystem.Debug("Xoa bot anh do so luong anh vuot qua gioi han max so anh____" +
                    //                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemImg.Key), itemImg.Key) + Inventec.Common.Logging.LogUtil.TraceData("imgs.Count", imgs.Count)
                    //                    );
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    //string detail = "|";
                    //if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 0)
                    //{
                    //    foreach (var dicImg in GlobalVariables.dicImageCapture)
                    //    {
                    //        detail += dicImg.Key + " - count = " + ((dicImg.Value != null && dicImg.Value.Count > 0) ? dicImg.Value.Count : 0) + "|";
                    //    }
                    //}

                    //Inventec.Common.Logging.LogSystem.Debug("DelegateCaptureImage____" +
                    //                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage)
                    //                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => detail), detail)
                    //                    + Inventec.Common.Logging.LogUtil.TraceData("GlobalVariables.listImage.Count", GlobalVariables.listImage.Count));

                    if (listImage == null)
                    {
                        listImage = new List<ImageADO>();
                    }
                    listImage.Insert(0, image);

                    ProcessLoadGridImage(listImage);
                    stream.Flush();
                }

                Inventec.Common.Logging.LogSystem.Info("end DelegateCaptureImage");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveImageToFile(ImageADO image, string folder)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(folder) && image != null && image.streamImage != null && image.streamImage.Length > 0)
                {
                    string treatmentFolderName = string.Format("{0} {1}", this.TreatmentWithPatientTypeAlter.TREATMENT_CODE, this.TreatmentWithPatientTypeAlter.TDL_PATIENT_NAME);
                    treatmentFolderName = string.Concat(treatmentFolderName.Split(Path.GetInvalidFileNameChars()));
                    string path = Path.Combine(folder, treatmentFolderName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullFileName = Path.Combine(path, string.Format("{0}_{1}.{2}", image.FileName.Replace(':', '_'), image.CAPTION, ImageFormat.Jpeg));

                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(fullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            image.streamImage.Position = 0;
                            image.streamImage.CopyTo(memory);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("lỗi hình ảnh");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void saveImage(TileItem item)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("item=" + item.Text + " item.Image=" + item.Image.Width + "x" + item.Image.Height);
                Bitmap file = new Bitmap(item.Image);
                Inventec.Common.Logging.LogSystem.Info(" file=" + file.Width + "x" + file.Height);
                (item.Tag as Bitmap).Dispose();
                file.Save(System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH, item.Text), ImageFormat.Png);
                Inventec.Common.Logging.LogSystem.Info("save file finish");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Bitmap ScaleImage(Image oldImage)
        {
            double resizeFactor = 1;

            if (oldImage.Width > 300 || oldImage.Height > 300)
            {
                double widthFactor = Convert.ToDouble(oldImage.Width) / 300;
                double heightFactor = Convert.ToDouble(oldImage.Height) / 125;
                resizeFactor = Math.Max(widthFactor, heightFactor);
            }

            int width = Convert.ToInt32(oldImage.Width / resizeFactor);
            int height = Convert.ToInt32(oldImage.Height / resizeFactor);
            Bitmap newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            newImage.MakeTransparent(Color.White);

            Graphics g = Graphics.FromImage(newImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(oldImage, 0, 0, newImage.Width, newImage.Height);
            return newImage;
        }

        private void LoadCombo(LookUpEdit cbo, string resource)
        {
            try
            {
                List<ComboADO> combo = new List<ComboADO>();

                for (int i = 1; i <= 10; i++)
                {
                    combo.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value(resource, Resources.ResourceLanguageManager.LanguageFormServiceExecute, LanguageManager.GetCulture())));
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                cbo.EditValue = combo[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoCapture.Checked)
                {
                    btnCapture.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();

                }
                else
                {
                    this.ucCamera1.CaptureCam1();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimerGetPicture()
        {
            try
            {
                for (int i = 0; i < Int32.Parse(spnTotalCapture.Value.ToString()); i++)
                {
                    ucCamera1.CaptureCam1();
                    System.Threading.Thread.Sleep((int)(spnTotalTimeToCapture.Value) * 1000);
                }

                btnCapture.Enabled = true;
            }
            catch (Exception ex)
            {
                btnCapture.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < Int32.Parse(spnTotalCapture.Value.ToString()); i++)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep((int)(double.Parse(spnTotalTimeToCapture.Value.ToString()) * 1000));
                        backgroundWorker1.ReportProgress(i);
                    }
                }
            }
            catch (Exception ex)
            {
                btnCapture.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn(e.ProgressPercentage.ToString());
                this.ucCamera1.CaptureCam1();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                btnCapture.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                btnCapture.Enabled = true;
            }
            catch (Exception ex)
            {
                btnCapture.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteImage_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.listImage = new List<ADO.ImageADO>();
                string clientCode = currentServiceReq.TDL_TREATMENT_CODE;
                List<Image> images = null;
                if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                {
                    images = GlobalVariables.dicImageCapture[clientCode];
                    GlobalVariables.dicImageCapture[clientCode] = null;
                    GlobalVariables.dicImageCapture.Remove(clientCode);
                }
                try
                {
                    if (GlobalVariables.listImage.Count > 0)
                    {
                        foreach (var item in images)
                        {
                            GlobalVariables.listImage.Remove(item);
                        }
                    }
                }
                catch (Exception exx)
                {
                    LogSystem.Warn(exx);
                }

                ProcessLoadGridImage(this.listImage);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listImage.Count", listImage != null ? listImage.Count : 0));

                string detail = "|";
                if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 0)
                {
                    foreach (var dicImg in GlobalVariables.dicImageCapture)
                    {
                        detail += dicImg.Key + " - count = " + ((dicImg.Value != null && dicImg.Value.Count > 0) ? dicImg.Value.Count : 0) + "|";
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("DelegateCaptureImage____" +
                                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => detail), detail)
                                    + Inventec.Common.Logging.LogUtil.TraceData("GlobalVariables.listImage.Count", GlobalVariables.listImage.Count));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UCServiceExecute_Leave(object sender, EventArgs e)
        {
            try
            {
                StopClick();
                btnCamera.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<ComboADO> combo = new List<ComboADO>();
                List<ComboADO> combo1 = new List<ComboADO>();
                List<ComboADO> comboConnectionType = new List<ComboADO>();
                for (int i = 1; i <= 10; i++)
                {
                    combo.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value("frmCamera.Giay", Resources.ResourceLanguageManager.LanguageFormServiceExecute, LanguageManager.GetCulture())));
                    combo1.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value("frmCamera.Anh", Resources.ResourceLanguageManager.LanguageFormServiceExecute, LanguageManager.GetCulture())));
                }

                comboConnectionType.Add(new ComboADO(1, Inventec.Common.Resource.Get.Value("frmCamera.KetNoiCongSvideo", Resources.ResourceLanguageManager.LanguageFormServiceExecute, LanguageManager.GetCulture())));
                comboConnectionType.Add(new ComboADO(2, Inventec.Common.Resource.Get.Value("frmCamera.KetNoiCongUsb", Resources.ResourceLanguageManager.LanguageFormServiceExecute, LanguageManager.GetCulture())));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboConnectionType, comboConnectionType, controlEditorADO);

                cboConnectionType.EditValue = ApplicationCaptureTypeWorker.GetCaptureConnectType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StartClick()
        {
            try
            {
                if ((int)cboConnectionType.EditValue == 2)
                {
                    ucCameraDXC1.Start();
                }
                else
                {
                    ucCamera1.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StopClick()
        {
            try
            {
                if (ucCameraDXC1 != null && !ucCameraDXC1.IsDisposed)
                {
                    ucCameraDXC1.Stop();
                }
                if (ucCamera1 != null && !ucCamera1.IsDisposed)
                {
                    ucCamera1.Stop();
                }

                panelControlCamera.Controls.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlCamera(bool isEnable)
        {
            try
            {
                cboConnectionType.Enabled = isEnable;
                btnCapture.Enabled = isEnable;
                btnDeleteImage.Enabled = isEnable;
                btnShowConfig.Enabled = isEnable;
                btnCameramax.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectFolder()
        {
            try
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (!String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage))
                    {
                        dialog.SelectedPath = this.SelectedFolderForSaveImage;
                    }
                    else
                    {
                        dialog.SelectedPath = this.FolderSaveImage;
                    }

                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        this.SelectedFolderForSaveImage = dialog.SelectedPath;
                    }
                    else
                    {
                        this.SelectedFolderForSaveImage = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveAllImage()
        {
            try
            {
                if (this.listImage != null && listImage.Count > 0)
                {
                    string folder = "";
                    if (!String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage))
                    {
                        folder = this.SelectedFolderForSaveImage;
                    }
                    else
                    {
                        folder = this.FolderSaveImage;
                    }

                    foreach (var item in this.listImage)
                    {
                        SaveImageToFile(item, folder);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    RightButtonType type = (RightButtonType)(e.Item.Tag);
                    switch (type)
                    {
                        case RightButtonType.Copy:
                            ProsseccCopyImageToClipboard();
                            break;
                        case RightButtonType.ChangeStt:
                            ShowFromChangeStt();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFromChangeStt()
        {
            try
            {
                if (this.currentDataClick != null && this.currentDataClick.IsChecked)
                {
                    frmSTTNumber frmSTTNumber = new frmSTTNumber(this.currentDataClick, this.listImage, ProcessUpdateImageOrder);
                    frmSTTNumber.ShowDialog();
                    Inventec.Common.Logging.LogSystem.Debug(this.currentDataClick.FileName + " " + this.currentDataClick.STTImage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProsseccCopyImageToClipboard()
        {
            try
            {
                if (this.currentDataClick != null)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        this.currentDataClick.streamImage.Position = 0;
                        this.currentDataClick.streamImage.CopyTo(memory);

                        Bitmap image = ResizeImage(Image.FromStream(memory), 250, 140);

                        System.Windows.Forms.Clipboard.SetImage(image);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowHideOptionCamera()
        {
            try
            {
                this.VisibleOptionCamera = !this.VisibleOptionCamera;
                if (ucCamera1 != null)
                {
                    ucCamera1.VisibleControl(VisibleOptionCamera);
                }

                if (ucCameraDXC1 != null)
                {
                    ucCameraDXC1.VisibleControl(VisibleOptionCamera);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
