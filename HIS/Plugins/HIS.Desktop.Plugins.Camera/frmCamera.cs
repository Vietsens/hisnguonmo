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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Camera.ADO;
using HIS.Desktop.Plugins.Camera.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.UC.ImageLib.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Camera
{
    public partial class frmCamera : HIS.Desktop.Utility.FormBase
    {
        CameraADO initADO = null;
        EventClosing formClosing = null;
        UCCamera ucCamera1;
        UCCameraDXC ucCameraDXC1;
        HIS_SERVICE_REQ serviceReq;
        DelegateSelectData delegateSelect;
        DelegateRefreshData _dlgBeforClose;
        int maxImage;
        Inventec.Desktop.Common.Modules.Module currentModule;
        int numberImage;
        int timeCapture;
        int count;
        long dem;
        
        #region Construct

        public frmCamera()
        {
            try
            {
                InitializeComponent();
                this.ucCamera1 = new UCCamera(DelegateCaptureImage);
                pnlLeft.Controls.Add(this.ucCamera1);
                this.ucCamera1.Dock = DockStyle.Fill;
                this.ucCamera1.Start();
                this.ucCamera1.SetDisable();
                this.TopMost = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmCamera(Inventec.Desktop.Common.Modules.Module module, CameraADO ado, DelegateSelectData deleSelect, DelegateRefreshData _dlg, HIS_SERVICE_REQ serviceReq)
            : base(module)
        {
            try
            {
                InitializeComponent();

                if (ado != null)
                {
                    this.initADO = ado;
                    this.formClosing = ado.cameraClosing;
                }
                this.currentModule = module;
                this.delegateSelect = deleSelect;
                this._dlgBeforClose = _dlg;
                this.serviceReq = serviceReq;

                this.TopMost = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmCamera(Inventec.Desktop.Common.Modules.Module module, CameraADO ado, DelegateSelectData deleSelect, DelegateRefreshData _dlg, HIS_SERVICE_REQ serviceReq, int cameratype)
            : base(module)
        {
            try
            {
                InitializeComponent();

                if (ado != null)
                {
                    this.initADO = ado;
                    this.formClosing = ado.cameraClosing;
                }
                this.currentModule = module;
                this.delegateSelect = deleSelect;
                this._dlgBeforClose = _dlg;
                this.serviceReq = serviceReq;

                this.TopMost = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method

        private void InitUcCamera()
        {
            try
            {
                if (ApplicationCaptureTypeWorker.GetCaptureConnectType() == 2)
                {
                    this.ucCameraDXC1 = new UCCameraDXC(DelegateCaptureImage);
                    this.pnlLeft.Controls.Add(this.ucCameraDXC1);
                    this.ucCameraDXC1.Dock = DockStyle.Fill;
                    this.ucCameraDXC1.Start();
                    this.ucCameraDXC1.SetDisable();
                    this.ucCameraDXC1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                    this.ucCameraDXC1.SetClientCode(txtTreatmentCode.Text);
                }
                else
                {
                    this.ucCamera1 = new UCCamera(DelegateCaptureImage);
                    this.pnlLeft.Controls.Add(this.ucCamera1);
                    this.ucCamera1.Dock = DockStyle.Fill;
                    this.ucCamera1.Start();
                    this.ucCamera1.SetDisable();
                    this.ucCamera1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                    this.ucCamera1.SetClientCode(txtTreatmentCode.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcCamera(int connectionType)
        {
            try
            {
                pnlLeft.Controls.Clear();
                if (connectionType == 2)
                {
                    this.ucCameraDXC1 = new UCCameraDXC(DelegateCaptureImage);
                    this.pnlLeft.Controls.Add(this.ucCameraDXC1);
                    this.ucCameraDXC1.Dock = DockStyle.Fill;
                    this.ucCameraDXC1.Start();
                    this.ucCameraDXC1.SetDisable();
                    this.ucCameraDXC1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                    this.ucCameraDXC1.SetClientCode(txtTreatmentCode.Text);
                }
                else
                {
                    this.ucCamera1 = new UCCamera(DelegateCaptureImage);
                    this.pnlLeft.Controls.Add(this.ucCamera1);
                    this.ucCamera1.Dock = DockStyle.Fill;
                    this.ucCamera1.Start();
                    this.ucCamera1.SetDisable();
                    this.ucCamera1.IsAutoSaveImageInStore = AppConfigKeys.IsSavingInLocal;
                    this.ucCamera1.SetClientCode(txtTreatmentCode.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // On form closing
        private void frmConnectCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StopTimer(GetModuleLink(), "timer1");
                this.count = 0;
                this.StopClick();
                if (this._dlgBeforClose != null)
                {
                    this._dlgBeforClose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmConnectCamera_Load(object sender, EventArgs e)
        {
            try
            {
                RegisterTimer(GetModuleLink(), "timer1", timer1.Interval, timer1_Tick);
                string shortcutCapture = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY);

                try
                {
                    DevExpress.XtraBars.BarShortcut shortcut1 = new DevExpress.XtraBars.BarShortcut((Keys)Enum.Parse(typeof(Keys), shortcutCapture, true));
                    btnCaptureCam1.ItemShortcut = shortcut1;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Gan phim tat cho nut chup anh that bai, " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => shortcutCapture), shortcutCapture), ex);
                }

                SetCaptionByLanguageKey();
                maxImage = HisConfigs.Get<int>("HIS.Desktop.Plugins.Camera.MaxImage");
                LoadDataToCombo();

                GetCheckAuto();
                txtPatientName.ReadOnly = true;
                //LoadCombo(cboNumber,"frmCamera.Anh");
                //LoadCombo(cboTime, "frmCamera.Giay");
                if (this.serviceReq != null)
                {
                    txtPatientName.Text = this.serviceReq.TDL_PATIENT_NAME;
                    txtTreatmentCode.Text = this.serviceReq.TDL_TREATMENT_CODE;
                    txtTreatmentCode.ReadOnly = true;
                }

                if (cboConnectionType.EditValue != null)
                {
                    this.InitUcCamera((int)cboConnectionType.EditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Camera.Resources.Lang", typeof(HIS.Desktop.Plugins.Camera.frmCamera).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCamera.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCaptureCam1.Caption = Inventec.Common.Resource.Get.Value("frmCamera.btnCaptureCam1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnStart.Caption = Inventec.Common.Resource.Get.Value("frmCamera.btnStart.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnStop.Caption = Inventec.Common.Resource.Get.Value("frmCamera.btnStop.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmCamera.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOnTop.Text = Inventec.Common.Resource.Get.Value("frmCamera.btnOnTop.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOnTop.ToolTip = Inventec.Common.Resource.Get.Value("frmCamera.btnOnTop.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmCamera.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmCamera.btnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCapture.ToolTip = Inventec.Common.Resource.Get.Value("frmCamera.btnCapture.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileControl1.Text = Inventec.Common.Resource.Get.Value("frmCamera.tileControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTime.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCamera.cboTime.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNumber.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCamera.cboNumber.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CheckAuto.Properties.Caption = Inventec.Common.Resource.Get.Value("frmCamera.CheckAuto.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmCamera.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCamera.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateCaptureImage(Stream stream)
        {
            try
            {
                if (stream != null && stream.Length > 0)
                {

                    if (GlobalVariables.dicImageCapture == null)
                    {
                        GlobalVariables.dicImageCapture = new Dictionary<string, List<Image>>();
                    }
                    if (GlobalVariables.listImage == null)
                    {
                        GlobalVariables.listImage = new List<Image>();
                    }
                    stream.Position = 0;
                    Image i = Image.FromStream(stream);
                    i.Tag = DateTime.Now.ToString("HHmmssfff");

                    string clientCode = txtTreatmentCode.Text;

                    if (!String.IsNullOrEmpty(clientCode))
                    {
                        List<Image> images;
                        if (GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                        {
                            images = GlobalVariables.dicImageCapture[clientCode];
                            if (images == null)
                            {
                                images = new List<Image>();
                            }
                            images.Add(i);
                            GlobalVariables.dicImageCapture[clientCode] = images;
                        }
                        else
                        {
                            images = new List<Image>();
                            images.Add(i);
                            GlobalVariables.dicImageCapture.Add(clientCode, images);
                        }
                    }

                    GlobalVariables.listImage.Add(i);

                    CaptureImage(stream);
                    if (tileGroup2.Items.Count > maxImage)
                    {
                        tileGroup2.Items.RemoveAt(0);
                    }
                    if (GlobalVariables.listImage.Count > maxImage)
                    {
                        GlobalVariables.listImage.RemoveAt(0);
                        if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 1)
                        {
                            foreach (var itemImg in GlobalVariables.dicImageCapture)
                            {
                                var imgs = itemImg.Value;
                                if (imgs != null && imgs.Count > 0)
                                {
                                    imgs.RemoveAt(0);
                                    Inventec.Common.Logging.LogSystem.Debug("Xoa bot anh do so luong anh vuot qua gioi han max so anh____" +
                                        Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientCode), clientCode));
                                    break;
                                }
                            }
                        }
                    }

                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveImage(TileItem item)
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

        public void CaptureImage(Stream stream)
        {
            try
            {
                dem++;
                TileItem tileNew = new TileItem();
                tileNew.Name = txtTreatmentCode.Text + "_" + DateTime.Now.ToString("HHmmss");
                if (String.IsNullOrEmpty(txtPatientName.Text) && String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    tileNew.Text = "Image_" + DateTime.Now.ToString("HHmmss_" + dem.ToString()) + ".jpg";
                    tileNew.Name = "Image_" + DateTime.Now.ToString("HHmmss_" + dem.ToString()) + ".jpg";
                }
                else
                {
                    tileNew.Text = txtPatientName.Text + "_" + txtTreatmentCode.Text + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + dem.ToString() + ".jpg";
                }
                tileNew.AppearanceItem.Normal.ForeColor = Color.Black;
                tileNew.TextShowMode = TileItemContentShowMode.Hover;
                tileNew.TextAlignment = TileItemContentAlignment.BottomCenter;
                tileNew.ItemSize = TileItemSize.Large;
                tileNew.Image = Image.FromStream(stream);
                tileNew.Tag = new Bitmap(tileNew.Image);
                tileNew.ImageAlignment = TileItemContentAlignment.MiddleCenter;
                Thread.Sleep(10);
                tileNew.AppearanceItem.Normal.BorderColor = Color.Green;
                tileNew.Checked = true;
                tileNew.Visible = true;
                tileNew.ItemClick += TileItemClickUnchecked;
                //tileNew.ItemDoubleClick += TileItemClick;
                //SaveImage(tileNew);
                tileNew.Text = "";
                tileGroup2.Items.Add(tileNew);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TileItemClickUnchecked(object sender, TileItemEventArgs e)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    if (e.Item.Checked)
                    {
                        e.Item.Checked = false;
                    }
                    else 
                    {
                        e.Item.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            } 
        }

        private void TileItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    e.Item.Image.Tag = System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH, e.Item.Name);
                    this.delegateSelect(e.Item.Image);
                    this.Close();
                }
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

        private void btnCaptureCam1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCapture_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                StartClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                StopClick();
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
                //if ((int)cboConnectionType.EditValue == 2)
                //{
                //    ucCameraDXC1.Stop();
                //}
                //else
                //{
                //    ucCamera1.Stop();
                //}

                if (ucCameraDXC1 != null && !ucCameraDXC1.IsDisposed)
                {
                    ucCameraDXC1.Stop();
                }
                if (ucCamera1 != null && !ucCamera1.IsDisposed)
                {
                    ucCamera1.Stop();
                }

                pnlLeft.Controls.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try 
            {
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                for (int i = 0; i < tileItems.Count; i++)
                {
                    tileItems[i].Image.Tag = System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH, tileItems[i].Name);
                    this.delegateSelect(tileItems[i].Image);
                }
                    this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCapture_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (CheckAuto.Checked == true)
                {
                    StartTimer(GetModuleLink(),"timer1");

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

        private void btnFolder_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    frmImage.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < tileGroup2.Items.Count; i++)
                {
                    tileGroup2.Items[i].Checked = true;
                }
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                for (int i = 0; i < tileItems.Count; i++)
                {
                    tileGroup2.Items.Remove(tileItems[i]);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnOnTop_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = !this.TopMost;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadCombo(LookUpEdit cbo, string resource)
        //{
        //    try
        //    {
        //        List<ComboADO> combo = new List<ComboADO>();

        //        for (int i = 1; i <= 10; i++)
        //        {
        //            combo.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value(resource, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
        //        }
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("statusName", "", 50, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
        //        cbo.EditValue = combo[0].id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        private void LoadDataToCombo()
        {
            try
            {
                List<ComboADO> combo = new List<ComboADO>();
                List<ComboADO> combo1 = new List<ComboADO>();
                List<ComboADO> comboConnectionType = new List<ComboADO>();
                for (int i = 1; i <= 10; i++)
                {
                    combo.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value("frmCamera.Giay", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                    combo1.Add(new ComboADO(i, (i).ToString() + Inventec.Common.Resource.Get.Value("frmCamera.Anh", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                }

                comboConnectionType.Add(new ComboADO(1, Inventec.Common.Resource.Get.Value("frmCamera.KetNoiCongSvideo", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                comboConnectionType.Add(new ComboADO(2, Inventec.Common.Resource.Get.Value("frmCamera.KetNoiCongUsb", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboNumber, combo1, controlEditorADO);
                ControlEditorLoader.Load(cboTime, combo, controlEditorADO);
                ControlEditorLoader.Load(cboConnectionType, comboConnectionType, controlEditorADO);

                cboTime.EditValue = combo[0].id;
                cboNumber.EditValue = combo1[0].id;

                cboConnectionType.EditValue = ApplicationCaptureTypeWorker.GetCaptureConnectType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetCheckAuto()
        {
            try
            {
                if (CheckAuto.Checked == true)
                {
                    layoutControlItem4.Enabled = true;
                    layoutControlItem5.Enabled = true;
                    this.numberImage = Inventec.Common.TypeConvert.Parse.ToInt32((cboNumber.EditValue).ToString());
                    this.timeCapture = Inventec.Common.TypeConvert.Parse.ToInt32((cboTime.EditValue).ToString());
                    timer1.Interval = timeCapture * 1000;

                }
                else if (CheckAuto.Checked == false)
                {
                    layoutControlItem4.Enabled = false;
                    layoutControlItem5.Enabled = false;
                    StopTimer(GetModuleLink(), "timer1");
                    this.count = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                timeCapture = Inventec.Common.TypeConvert.Parse.ToInt32((cboTime.EditValue).ToString());
                timer1.Interval = timeCapture * 1000;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNumber_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                this.numberImage = Inventec.Common.TypeConvert.Parse.ToInt32((cboNumber.EditValue).ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNumber.EditValue != null)
                    {
                        cboTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNumber.EditValue != null)
                    {
                        //cboTime.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAuto_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetCheckAuto();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick()
        {
            try
            {
                count++;
                ucCamera1.CaptureCam1();
                if (count == numberImage)
                {
                    StopTimer(GetModuleLink(), "timer1");
                    this.count = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    GetPatientName();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPatientName()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    var codeFind = string.Format("{0:000000000000}", Convert.ToInt64(txtTreatmentCode.Text));
                    treatmentFilter.TREATMENT_CODE__EXACT = codeFind;
                    var treatment = new BackendAdapter(param)
                .Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (treatment != null && treatment.Count > 0)
                    {
                        txtPatientName.Text = treatment.FirstOrDefault().TDL_PATIENT_NAME;
                        txtTreatmentCode.Text = codeFind;
                        if (ApplicationCaptureTypeWorker.GetCaptureConnectType() == 2)
                        {                           
                            this.ucCameraDXC1.SetClientCode(txtTreatmentCode.Text);
                        }
                        else
                        {                           
                            this.ucCamera1.SetClientCode(txtTreatmentCode.Text);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy bệnh nhân tương ứng với mã điều trị này", "Thông báo");
                        if (ApplicationCaptureTypeWorker.GetCaptureConnectType() == 2)
                        {
                            this.ucCameraDXC1.SetClientCode("");
                        }
                        else
                        {
                            this.ucCamera1.SetClientCode("");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConnectionType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
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
                btnCapture.Enabled = isEnable;
                btnDelete.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void btnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                EnableControlCamera(true);
                if (cboConnectionType.EditValue != null)
                {
                    InitUcCamera((int)cboConnectionType.EditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConnectionType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboConnectionType.EditValue != null)
                    {
                        ApplicationCaptureTypeWorker.ChangeCaptureConnectType((int)cboConnectionType.EditValue);
                        StopClick();
                        InitUcCamera((int)cboConnectionType.EditValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
