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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraGrid.Views.Tile;
using System.Drawing.Drawing2D;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmCamera : FormBase
    {
        UCCamera ucCamera1;
        UCCameraDXC ucCameraDXC1;
        int maxImage;
        int numberImage;
        int timeCapture;
        int valueType;
        long dem;
        bool IsStart;
        bool VisibleOptionCamera;
        HIS_SERVICE_REQ currentServiceReq;
        private string camMonikerString;
        private string Loginname;
        List<ImageADO> listImage;
        string SelectedFolderForSaveImage;
        bool IsAutoCapture;
        int totalCapture;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_SERVICE> lstService;
        internal enum RightButtonType
        {
            Copy,
            ChangeStt
        }
        List<ADO.ServiceADO> listServiceADOForAllInOne;
        ADO.ServiceADO sereServ;
        Action<List<ImageADO>> SendListImage;
        Timer timerDoubleClick = new Timer();

        public frmCamera(Inventec.Desktop.Common.Modules.Module moduleData, List<V_HIS_SERVICE> lstService, List<ADO.ServiceADO> listServiceADOForAllInOne, ADO.ServiceADO sereServ, List<ImageADO> listImage, Action<List<ImageADO>> SendListImage, int valueType, HIS_SERVICE_REQ currentServiceReq, string camMonikerString, string Loginname, string SelectedFolderForSaveImage, bool IsAutoCapture, int totalCapture)
        {
            InitializeComponent();
            this.currentServiceReq = currentServiceReq;
            this.valueType = valueType;
            this.camMonikerString = camMonikerString;
            this.Loginname = Loginname;
            this.SelectedFolderForSaveImage = SelectedFolderForSaveImage;
            this.IsAutoCapture = IsAutoCapture;
            this.totalCapture = totalCapture;
            this.moduleData = moduleData;
            this.lstService = lstService;
            this.listServiceADOForAllInOne = listServiceADOForAllInOne;
            this.sereServ = sereServ;
            this.listImage = listImage;
            this.SendListImage = SendListImage;
        }

        private void StartCamera()
        {
            try
            {
            this.panelControlCamera.Controls.Clear();
            if (valueType == 2)
            {
                this.ucCameraDXC1 = new UCCameraDXC(DelegateCaptureImage);
                this.panelControlCamera.Controls.Add(this.ucCameraDXC1);
                this.ucCameraDXC1.Dock = DockStyle.Fill;
                if (!this.ucCameraDXC1.IsDisposed)
                {
                    this.ucCameraDXC1.Stop();
                }
                this.ucCameraDXC1.Start();
                this.ucCameraDXC1.SetDisable();
                this.ucCameraDXC1.VisibleControl(VisibleOptionCamera);
                this.ucCameraDXC1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                this.ucCameraDXC1.SetClientCode(this.currentServiceReq.TDL_TREATMENT_CODE);
            }
            else
            {
                this.ucCamera1 = new UCCamera(DelegateCaptureImage, false, true);
                this.panelControlCamera.Controls.Add(this.ucCamera1);
                this.ucCamera1.Dock = DockStyle.Fill;
                if (!this.ucCamera1.IsDisposed)
                {
                    this.ucCamera1.Stop();
                }
                this.ucCamera1.Start();
                this.ucCamera1.SetDisable();
                this.ucCamera1.VisibleControl(VisibleOptionCamera);
                this.ucCamera1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                this.ucCamera1.SetClientCode(this.currentServiceReq.TDL_TREATMENT_CODE);
            }

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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerDoubleClick_Tick()
        {
            try
            {
                StopTimer(this.Name, "timerDoubleClick");
                if (this.currentDataClick != null)
                {
                    ProcessRefeshImageOrder(this.currentDataClick);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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


                    if (!String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage))
                    {
                        SaveImageToFile(image, this.SelectedFolderForSaveImage);
                    }

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
        private void ProcessLoadGridImage(List<ADO.ImageADO> listImage)
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                if (listImage != null && listImage.Count > 0)
                {
                    gridControl1.DataSource = listImage;
                }
                gridControl1.EndUpdate();
                //lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
            }
            catch (Exception ex)
            {
                gridControl1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveImageToFile(ImageADO image, string folder)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(folder) && image != null && image.streamImage != null && image.streamImage.Length > 0)
                {
                    string treatmentFolderName = string.Format("{0} {1}", this.currentServiceReq.TDL_TREATMENT_CODE, this.currentServiceReq.TDL_PATIENT_NAME);
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

        private void btnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsAutoCapture)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < totalCapture; i++)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(totalCapture * 1000);
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

        private void btnShowConfig_Click(object sender, EventArgs e)
        {
            ShowHideOptionCamera();
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
        private void btnDelete_Click(object sender, EventArgs e)
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

        private void tileView1_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Name == "btnDelete")
                {
                    if (XtraMessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var dataItem = (DevExpress.XtraGrid.Views.Tile.TileViewItem)e.DataItem;
                        var item = (ADO.ImageADO)tileView1.GetRow(dataItem.RowHandle);
                        //nếu đã lưu thì gọi api xóa và check document
                        if (item.ID > 0)
                        {
                            CommonParam param = new CommonParam();
                            HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                            data.ID = item.ID;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERE_SERV_FILE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                            //gọi api xóa thành công thì xóa ở danh sách và xóa document
                            if (apiResult)
                            {
                                tileView1.DeleteRow(dataItem.RowHandle);
                            }
                        }
                        else
                        {
                            tileView1.DeleteRow(dataItem.RowHandle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_ContextButtonCustomize(object sender, DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventArgs e)
        {
            try
            {
                var item = (ADO.ImageADO)tileView1.GetRow(e.RowHandle);
                if (item != null && item.CREATOR == Loginname && e.Item.Name == "btnDelete")
                {
                    e.Item.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
                }
                else
                {
                    e.Item.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemClick");
                e.Item.Checked = !e.Item.Checked;
                TileView tileView = sender as TileView;
                this.currentDataClick = (ADO.ImageADO)tileView.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;
                StartTimer(this.Name, "timerDoubleClick");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public ImageADO currentDataClick { get; set; }

        private void tileView1_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemDoubleClick");
                StartTimer(this.Name, "timerDoubleClick");
                this.currentDataClick = null;
                e.Item.Checked = !e.Item.Checked;

                // mở form xem ảnh
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ImageViewOption) == "1")
                {
                    //check gộp sẽ truyền tất cả service_id gộp. ngược lại chỉ add 1 service_id của dịch vụ đang xử lý
                    List<long> serviceIds = new List<long>();
                    if (listServiceADOForAllInOne != null && listServiceADOForAllInOne.Count > 0)
                    {
                        serviceIds.AddRange(listServiceADOForAllInOne.Select(s => s.SERVICE_ID));
                    }
                    else if (sereServ != null)
                    {
                        serviceIds.Add(sereServ.SERVICE_ID);
                    }

                    var formView = new ViewImage.FormViewImageV2(this.moduleData, serviceIds, this.listImage, SaveImageData, lstService);
                    if (formView != null) formView.ShowDialog();
                }
                else
                {
                    var item = (ADO.ImageADO)tileView1.GetRow(e.Item.RowHandle);
                    Form formView = new ViewImage.FormViewImage(this.listImage, item);
                    if (formView != null) formView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SaveImageData(List<ImageADO> obj)
        {
            try
            {
                if (obj != null)
                {
                    this.listImage = ProcessOrderImage(obj);
                    gridControl1.RefreshDataSource();
                    //lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<HIS.Desktop.Plugins.ServiceExecute.ADO.ImageADO> ProcessOrderImage(List<HIS.Desktop.Plugins.ServiceExecute.ADO.ImageADO> images)
        {
            try
            {
                if (images != null && images.Count > 0)
                {
                    List<ADO.ImageADO> listImageTemp = new List<ADO.ImageADO>();
                    var listImageSTTOrder = images.Where(o => o.IsChecked).OrderBy(o => o.STTImage).ToList();
                    var listImageNoSTTOrder = images.Where(o => !o.IsChecked).OrderBy(o => o.FileName).ToList();
                    if (listImageSTTOrder != null && listImageSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageSTTOrder);
                    if (listImageNoSTTOrder != null && listImageNoSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageNoSTTOrder);
                    return listImageTemp;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return null;
        }

        private void tileView1_ItemRightClick(object sender, TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemRightClick");
                e.Item.Checked = !e.Item.Checked;
                this.currentDataClick = (ADO.ImageADO)tileView1.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;

                PopupMenu menu = new PopupMenu(this.barManager1);

                BarButtonItem itemCopy = new BarButtonItem(this.barManager1, ResourceMessage.CopyImage);
                itemCopy.Tag = RightButtonType.Copy;
                itemCopy.ItemClick += new ItemClickEventHandler(MouseRightClick);
                menu.AddItems(new BarItem[] { itemCopy });

                if (this.currentDataClick.IsChecked)
                {
                    BarButtonItem itemStt = new BarButtonItem(this.barManager1, ResourceMessage.ChangeStt);
                    itemStt.Tag = RightButtonType.ChangeStt;
                    itemStt.ItemClick += new ItemClickEventHandler(MouseRightClick);
                    menu.AddItems(new BarItem[] { itemStt });
                }

                menu.ShowPopup(Cursor.Position);
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
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage((Image)destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, width, height);
            }

            return destImage;
        }

        /// <summary>
        /// - Khi người dùng nhập số thứ tự vào thì sẽ cho phép hình ảnh đó được chèn vào stt được nhập đó. Và sắp xếp lại số thứ tự:
        ///VD: i1 = A, i2 = B, i3 = C, i4 = D, i5 = E (i1,2,3,4,5: STT; A,B,C,D,E: Hình ảnh)
        ///TH1: Đổi B có STT = 4 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = C, i3 = D, i4 = B, i5 = E (Các STT của C,D sẽ bị đẩy lên 1 đơn vị để B thay thế vào).
        ///TH2: Đổi D có STT = 2 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = D, i3 = B, i4 = C, i5 = E (Các STT của B,C sẽ bị đẩy xuống 1 đơn vị để D chèn vào).
        /// </summary>
        /// <param name="sttNumber"></param>
        private void ProcessUpdateImageOrder(decimal sttNumber)
        {
            try
            {
                if (this.currentDataClick.STTImage == (int)sttNumber)
                    return;

                bool isChangePlus = this.currentDataClick.STTImage < (int)sttNumber ? true : false;

                if (this.listImage != null && this.listImage.Count > 0)
                {
                    this.listImage = ProcessOrderImage(this.listImage);

                    var imgDuplicate = this.listImage.Where(o => o.IsChecked && o.STTImage == (int)sttNumber).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imgDuplicate), imgDuplicate) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isChangePlus), isChangePlus));
                    int duplicateSttNumber = imgDuplicate != null ? (imgDuplicate.STTImage ?? 0) : 0;
                    int rawSttNumber = currentDataClick != null ? (currentDataClick.STTImage ?? 0) : 0;
                    int numC = 1;
                    foreach (var itemImg in this.listImage)
                    {
                        int maxStt = this.listImage.Where(o => o.IsChecked).Max(o => o.STTImage ?? 0);
                        if (itemImg.IsChecked)
                        {
                            if (isChangePlus)
                            {
                                if ((int)sttNumber > maxStt)
                                {
                                    if ((itemImg.STTImage ?? 0) > rawSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                    else if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                        itemImg.STTImage = maxStt;
                                }
                                else if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) > rawSttNumber && (itemImg.STTImage ?? 0) <= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                }
                            }
                            else
                            {
                                if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) < rawSttNumber && (itemImg.STTImage ?? 0) >= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage + 1;
                                    }
                                }
                            }

                            numC += 1;
                        }
                    }
                }

                gridControl1.RefreshDataSource();
                //lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    TileView view = sender as TileView;
                    var checkedRows = view.GetCheckedRows();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkedRows), checkedRows));
                    if (checkedRows != null && checkedRows.Count() > 0)
                    {
                        var listImagedelete = this.listImage.Where(o => o.IsChecked == true).ToList();
                        string clientCode = currentServiceReq.TDL_TREATMENT_CODE;
                        List<Image> images = null;
                        if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                        {
                            images = GlobalVariables.dicImageCapture[clientCode];
                            foreach (var item in listImagedelete)
                            {
                                try
                                {
                                    images.Remove(item.IMAGE_DISPLAY);
                                    GlobalVariables.listImage.Remove(item.IMAGE_DISPLAY);
                                }
                                catch (Exception exx)
                                {
                                    LogSystem.Warn(exx);
                                }
                            }
                            GlobalVariables.dicImageCapture[clientCode] = images;
                        }
                        this.listImage = this.listImage.Where(o => o.IsChecked == false).ToList();
                        this.listImage = ProcessOrderImage(this.listImage);
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCamera_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SendListImage(listImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCamera_Load(object sender, EventArgs e)
        {
            RegisterTimer(this.Name, "timerDoubleClick", timerDoubleClick.Interval, timerDoubleClick_Tick);
            StartCamera();
            ProcessLoadGridImage(listImage);
        }
        private void ProcessRefeshImageOrder(ADO.ImageADO currentData)
        {
            try
            {
                if (currentData.IsChecked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessRefeshImageOrder.1");
                    List<int> listSTT = listImage != null ? listImage.Select(o => o.STTImage ?? 0).Distinct().ToList() : new List<int>();
                    listSTT = listSTT != null ? listSTT.OrderBy(o => o).ToList() : listSTT;
                    currentData.STTImage = 1;

                    if (listSTT != null && listSTT.Count() == 1)
                        currentData.STTImage = listSTT.Max() + 1;
                    else
                        for (int i = 0; i < listSTT.Count() - 1; i++)
                        {
                            if (listSTT[i] + 1 != listSTT[i + 1])
                            {
                                currentData.STTImage = listSTT[i] + 1;
                                break;
                            }
                            else
                                currentData.STTImage = listSTT.Max() + 1;
                        }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessRefeshImageOrder.2");
                    //bs chụp các ảnh a -> b -> c. stt sẽ tương ứng a=1, b=2, c=3. Khi bỏ chọn b thì làm lại stt như sau: a=1,c=2
                    currentData.STTImage = null;
                    if (listImage != null && listImage.Count > 0)
                    {
                        var listImageTemp = listImage.OrderByDescending(o => o.IsChecked).ThenBy(o => o.FileName).ToList();
                        int sttNew = 1;
                        foreach (var imgADO in listImageTemp)
                        {
                            if (imgADO.IsChecked)
                            {
                                imgADO.STTImage = sttNew;
                                sttNew += 1;
                            }
                        }
                    }
                }

                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                //ShowName
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.ImageADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ShowName")
                        {
                            try
                            {
                                e.Value = !String.IsNullOrWhiteSpace(data.CAPTION) ? data.CAPTION : data.FileName;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (valueType == 2)
            {
                this.ucCameraDXC1.Stop();
            }
            else
            {
                this.ucCamera1.Stop();
            }
        }
        private void frmCamera_ResizeEnd(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void frmCamera_ResizeBegin(object sender, EventArgs e)
        {

        }
    }
}
