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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectX.Capture;
using System.Diagnostics;
using Inventec.UC.ImageLib.Base;
using System.IO;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using DShowNET;
using System.Runtime.InteropServices;
using AForge.Controls;

namespace Inventec.UC.ImageLib.Core
{
    public partial class UCCameraDXC : UserControl
    {
        private Capture capture = null;
        private Filters filters = new Filters();
        List<CameraDevice> CameraDevices;
        List<VideoResolutionDevice> VideoResolutionDevices;
        AForge.Video.DirectShow.VideoCapabilities currentVideoResolution;
        #region Public variable
        /// <summary>
        /// Cau hinh co tu dong luu anh vao thu muc cau hinh hay khong
        /// True: tu dong luu anh vao thu muc cau hinh
        /// False: Chon vi tri luu anh tren may local
        /// Mac dinh la false
        /// </summary>
        public bool IsAutoSaveImageInStore { get; set; }
        #endregion
        private Stopwatch stopWatch = null;
        DelegateCaptureImage CaptureImage1 { get; set; }
        string pathFileStoreForCapture = Path.Combine(Path.Combine(Application.StartupPath, "Img"), "ImgTemp");
        string ClientCode { get; set; }
        bool isChangeResolution;

        public UCCameraDXC()
        {
            InitializeComponent();
            this.Init("");
        }
        public UCCameraDXC(DelegateCaptureImage captureImage)
            : this(captureImage, "")
        {

        }

        public UCCameraDXC(DelegateCaptureImage captureImage, string camMonikerString)
        {
            InitializeComponent();

            this.CaptureImage1 = captureImage;
            this.cboResolutionDevice.Enabled = false;
            this.Init(camMonikerString);
        }

        private void Init(string camMonikerString)
        {
            try
            {
                //#if DEBUG
                bool isAutoVideoInput = false;
                if (!String.IsNullOrEmpty(camMonikerString))
                {
                    foreach (Filter item in filters.VideoInputDevices)
                    {
                        if (item.MonikerString == camMonikerString)
                        {
                            capture = new Capture(item, filters.AudioInputDevices != null && filters.AudioInputDevices.Count > 0 ? filters.AudioInputDevices[0] : null);
                            isAutoVideoInput = true;
                            break;
                        }
                    }
                }
                if (!isAutoVideoInput)
                {
                    capture = new Capture(filters.VideoInputDevices[0], filters.AudioInputDevices != null && filters.AudioInputDevices.Count > 0 ? filters.AudioInputDevices[0] : null);
                }

                //capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                //#endif

                // Update the main menu
                // Much of the interesting work of this sample occurs here
                //try { updateMenu(); }            catch { }
                Filter f;
                //Source s;
                //Source current;
                //PropertyPage p;
                //Control oldPreviewWindow = null;
                this.CameraDevices = new List<CameraDevice>();
                for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                {
                    f = filters.VideoInputDevices[c];
                    CameraDevice cam = new CameraDevice(c + " : " + f.Name, f.MonikerString);
                    CameraDevices.Add(cam);
                }

                CameraLoader.LoadDataToComboDevice(cboCameraSource1, CameraDevices);
                if (this.CameraDevices == null || this.CameraDevices.Count() == 0)
                {
                    capture.PreviewWindow = null;
                }
                else
                {
                    capture.PreviewWindow = panelVideo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnCaptureComplete(object sender, EventArgs e)
        {
            try
            {
                // Demonstrate the Capture.CaptureComplete event.
                Inventec.Common.Logging.LogSystem.Warn("Capture complete.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDisable()
        {
            try
            {
                layoutControlItem1.TextVisible = false;
                layoutControlItem3.TextVisible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void VisibleControl(bool visibility)
        {
            try
            {
                if (visibility)
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Reload()
        {
            try
            {
                int videoCounts = filters.VideoInputDevices.Count;
                Filters filterss = new Filters();
                if (filterss == null || filterss.VideoInputDevices.Count == 0)
                {
                    throw new ArgumentNullException("videoDevices.Count == 0");
                }
                if (filterss.VideoInputDevices.Count != videoCounts)
                {
                    filters = filterss;
                    Init("");
                }

                // Get current devices and dispose of capture object
                // because the video and audio device can only be changed
                // by creating a new Capture object.
                Filter videoDevice = null;
                Filter audioDevice = null;

                if (capture != null && (cboCameraSource1.EditValue ?? "").ToString() != (videoDevice == null ? "" : videoDevice.MonikerString))
                {
                    videoDevice = capture.VideoDevice;
                    audioDevice = capture.AudioDevice;
                    capture.Dispose();
                    capture = null;
                }

                // Get new video device
                //MenuItem m = sender as MenuItem;
                for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                {
                    Filter f = filters.VideoInputDevices[c];
                    if ((cboCameraSource1.EditValue ?? "").ToString() == f.MonikerString)
                    {
                        videoDevice = f;
                    }
                }

                // Create capture object
                if ((videoDevice != null) || (audioDevice != null))
                {
                    capture = new Capture(videoDevice, audioDevice);
                }

                if (capture != null && cboCameraSource1.EditValue != null)
                {
                    isChangeResolution = false;
                    VideoResolutionDevices = VideoResolutionDeviceLoader.GetVideoResolutionDevices(cboCameraSource1.EditValue.ToString());

                    var vrd = this.VideoResolutionDevices.FirstOrDefault(k => k.VideoCapabilities.FrameSize == capture.VideoCaps.MaxFrameSize);
                    this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                    cboResolutionDevice.EditValue = this.currentVideoResolution.FrameSize.ToString();
                    CaptureResolutionStorage.ChangeCaptureResolution(this.currentVideoResolution.FrameSize.ToString());
                    capture.FrameSize = this.currentVideoResolution.FrameSize;
                    capture.FrameRate = 30.0;

                    panelVideo.Size = capture.FrameSize;
                    isChangeResolution = true;
                }
                else
                {
                    cboResolutionDevice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Video device not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void UCCameraDXC_Load(object sender, EventArgs e)
        {

        }

        public List<CameraDevice> GetCameraDevices()
        {
            return CameraDevices;
        }

        public void SetDelegate(DelegateCaptureImage captureImage)
        {
            try
            {
                this.CaptureImage1 = captureImage;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Stop()
        {
            try
            {
                //capture.Stop();
                capture.PreviewWindow.Dispose();
                capture.PreviewWindow = null;
                capture.Dispose();
                capture = null;
                cboResolutionDevice.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CaptureCam1()
        {
            try
            {
                capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
                capture.GrapImg();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetClientCode(string clientCode)
        {
            this.ClientCode = clientCode;
        }

        private void CaptureDone(System.Drawing.Bitmap e)
        {
            try
            {

                var image = e.Clone() as Bitmap;
                if (IsAutoSaveImageInStore)
                {
                    string savePath = System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig, DateTime.Now.ToString("yyyyMMdd"));
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    string fileName = String.Format("{0}_{1}.jpg", this.ClientCode, DateTime.Now.ToString("HHmmssfff"));

                    image.Save(System.IO.Path.Combine(savePath, fileName), ImageFormat.Jpeg);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => savePath), savePath)
                        +
                        Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                }

                //saving image
                if (CaptureImage1 != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, ImageFormat.Jpeg);
                        if (stream.Length > 0)
                        {
                            stream.Position = 0;
                            CaptureImage1(stream);
                        }
                    }
                }
                else
                {
                    SaveFileDialog storage = new SaveFileDialog();
                    storage.Filter = "JPEG (*.jpeg)|*.jpg|All files|*.*";

                    if (storage.ShowDialog() == DialogResult.OK)
                    {
                        FileStream fs = (FileStream)storage.OpenFile();
                        image.Save(fs, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Start()
        {
            try
            {
                if (!Directory.Exists(pathFileStoreForCapture))
                {
                    Directory.CreateDirectory(pathFileStoreForCapture);
                }

                capture.PreviewWindow = panelVideo;
                capture.Filename = pathFileStoreForCapture;
                capture.Start();

                if (this.filters.VideoInputDevices.Count == 0)
                    throw new Exception("Brak urządzeń do przechwytywania obrazu!");

                capture = new Capture(filters.VideoInputDevices[0], null);

                cboResolutionDevice.Enabled = true;
                VideoResolutionDevices = VideoResolutionDeviceLoader.GetVideoResolutionDevices(cboCameraSource1.EditValue.ToString());
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => VideoResolutionDevices), VideoResolutionDevices));
                CameraLoader.LoadDataToComboResolution(cboResolutionDevice, this.VideoResolutionDevices);
                isChangeResolution = false;
                if (this.currentVideoResolution != null)
                {
                    
                }
                else if (!String.IsNullOrEmpty(CaptureResolutionStorage.GetCaptureResolution()))
                {
                    var vrd = this.VideoResolutionDevices.FirstOrDefault(o => o.VideoCapabilities.FrameSize.ToString() == CaptureResolutionStorage.GetCaptureResolution());
                    if (vrd != null && vrd.VideoCapabilities != null)
                    {
                        this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                        cboResolutionDevice.EditValue = this.currentVideoResolution.FrameSize.ToString();
                    }
                }
                if (this.currentVideoResolution == null)
                {
                    var vrd = this.VideoResolutionDevices.FirstOrDefault(k => k.VideoCapabilities.FrameSize == capture.VideoCaps.MaxFrameSize);
                    this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                    cboResolutionDevice.EditValue = this.currentVideoResolution.FrameSize.ToString();
                    CaptureResolutionStorage.ChangeCaptureResolution(this.currentVideoResolution.FrameSize.ToString());
                    capture.FrameSize = this.currentVideoResolution.FrameSize;
                    capture.FrameRate = 30.0;
                }
                else
                {
                    capture.FrameSize = this.currentVideoResolution.FrameSize;
                    capture.FrameRate = 30.0;
                }

                isChangeResolution = true;

                //capture.VideoCaps.MaxFrameSize = ImageLayout.Zoom;

                panelVideo.Size = capture.FrameSize;
                capture.PreviewWindow = panelVideo;
                //capture.FrameEvent2 += new Capture.HeFrame(CapturedFrame);
                //capture.GrapImg();

                // pauza pozwalająca na kompletne zainicjowanie kamery
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void cboCameraSource1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCameraSource1.EditValue != null)
                    {
                        Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void cboCameraSource1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCameraSource1.EditValue != null)
                    {
                        Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void cboCameraSource1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
                {
                    capture.PropertyPages[0].Show(this);//TODO                  
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResolutionDevice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isChangeResolution), isChangeResolution)
                    + Inventec.Common.Logging.LogUtil.TraceData("cboResolutionDevice.EditValue", cboResolutionDevice.EditValue));
                if (isChangeResolution && cboResolutionDevice.EditValue != null)
                {
                    var vrd = this.VideoResolutionDevices.FirstOrDefault(o => o.VideoCapabilities.FrameSize.ToString() == (string)cboResolutionDevice.EditValue);
                    this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                    if (this.currentVideoResolution != null)
                    {
                        CaptureResolutionStorage.ChangeCaptureResolution(this.currentVideoResolution.FrameSize.ToString());
                        Stop();
                        Start();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVideoResolution), currentVideoResolution));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
