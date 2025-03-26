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

using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using Inventec.UC.ImageLib.Base;
using DevExpress.XtraEditors;
using AForge.Controls;

namespace Inventec.UC.ImageLib.Core
{
    public partial class UCCamera : UserControl
    {
        #region Private variable
        // list of video devices
        FilterInfoCollection videoDevices;
        List<CameraDevice> CameraDevices;
        List<VideoResolutionDevice> VideoResolutionDevices;
        VideoCapabilities currentVideoResolution;
        VideoCapabilities currentVideoResolutionMax;
        bool isChangeResolution;
        CameraDevice currentCameraDevice1;
        bool isChangeCamera1;
        // stop watch for measuring fps
        private Stopwatch stopWatch = null;
        DelegateCaptureImage CaptureImage1 { get; set; }
        AForge.Controls.VideoSourcePlayer videoSourcePlayer1 { get; set; }
        bool IsOptionFullScreen { get; set; }

        bool IsOptionFullSize { get; set; }
        #endregion

        #region Public variable
        /// <summary>
        /// Cau hinh co tu dong luu anh vao thu muc cau hinh hay khong
        /// True: tu dong luu anh vao thu muc cau hinh
        /// False: Chon vi tri luu anh tren may local
        /// Mac dinh la false
        /// </summary>
        public bool IsAutoSaveImageInStore { get; set; }
        string ClientCode { get; set; }
        #endregion

        #region Private method
        private void UCCamera_Load(object sender, EventArgs e)
        {
            cboResolutionDevice.Enabled = false;
        }

        void Init()
        {
            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    throw new ArgumentNullException("videoDevices.Count == 0");
                }

                this.CameraDevices = new List<CameraDevice>();
                for (int i = 1, n = videoDevices.Count; i <= n; i++)
                {
                    CameraDevice cam = new CameraDevice(i + " : " + videoDevices[i - 1].Name, videoDevices[i - 1].MonikerString);
                    CameraDevices.Add(cam);
                }
                CameraLoader.LoadDataToComboDevice(cboCameraSource1, CameraDevices);

                isChangeCamera1 = false;
            }
            catch (Exception ex)
            {
                currentCameraDevice1 = null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        void SetupCamera(string camMonikerString)
        {
            try
            {
                if (CameraDevices != null && CameraDevices.Count > 0)
                {
                    if (!String.IsNullOrEmpty(camMonikerString))
                    {
                        currentCameraDevice1 = CameraDevices.Where(o => o.MonikerString == camMonikerString).FirstOrDefault();
                    }
                    if (currentCameraDevice1 == null)
                    {
                        currentCameraDevice1 = CameraDevices[0];
                    }

                    cboCameraSource1.EditValue = currentCameraDevice1.MonikerString;
                }
                else
                {
                    cboCameraSource1.EditValue = null;
                    currentCameraDevice1 = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        // Start cameras
        private void StartCameras()
        {
            try
            {
                if (videoDevices != null && videoDevices.Count > 0)
                {
                    VideoCaptureDevice videoSource1 = null;
                    // create first video source
                    if (cboCameraSource1.Enabled && cboCameraSource1.EditValue != null)
                    {
                        videoSource1 = new VideoCaptureDevice(cboCameraSource1.EditValue.ToString());
                    }
                    if (cboCameraSource1.EditValue != null && (videoSource1 == null || videoSource1.IsRunning) && CameraDevices.Count > 1)
                    {
                        currentCameraDevice1 = CameraDevices.Where(o => o.MonikerString != cboCameraSource1.EditValue.ToString()).FirstOrDefault();

                        if (currentCameraDevice1 == null)
                        {
                            currentCameraDevice1 = CameraDevices[0];
                        }

                        cboCameraSource1.EditValue = currentCameraDevice1.MonikerString;
                        videoSource1 = new VideoCaptureDevice(currentCameraDevice1.MonikerString);
                    }
                    videoSource1.DesiredFrameRate = 10;

                    //videoSource1.VideoResolution = videoSource1.VideoCapabilities[i];//List capa
                    //Lấy danh sách resolution of divice đẩy dữ liệu vaò combo danh sách cap
                    cboResolutionDevice.Enabled = true;
                    VideoResolutionDevices = VideoResolutionDeviceLoader.GetVideoResolutionDevices(cboCameraSource1.EditValue.ToString());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => VideoResolutionDevices), VideoResolutionDevices));
                    CameraLoader.LoadDataToComboResolution(cboResolutionDevice, this.VideoResolutionDevices);
                    isChangeResolution = false;


                    var vrdMax = this.VideoResolutionDevices.OrderByDescending(k => k.VideoCapabilities.FrameSize.Width).FirstOrDefault();
                    if (vrdMax != null && vrdMax.VideoCapabilities != null)
                    {
                        currentVideoResolutionMax = vrdMax != null ? vrdMax.VideoCapabilities : null;
                    }

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
                        else
                        {
                            this.currentVideoResolution = null;
                        }
                    }

                    if (this.currentVideoResolution == null)
                    {
                        var vrd = this.VideoResolutionDevices.OrderByDescending(k => k.VideoCapabilities.FrameSize.Width).FirstOrDefault();
                        if (vrd != null && vrd.VideoCapabilities != null)
                        {
                            this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                            CaptureResolutionStorage.ChangeCaptureResolution(this.currentVideoResolution.FrameSize.ToString());
                            cboResolutionDevice.EditValue = this.currentVideoResolution.FrameSize.ToString();
                            videoSource1.VideoResolution = this.currentVideoResolution;
                        }
                    }
                    else
                    {
                        videoSource1.VideoResolution = this.currentVideoResolution;
                    }
                    isChangeResolution = true;

                    videoSourcePlayer1.BackgroundImageLayout = ImageLayout.Zoom;
                    videoSourcePlayer1.MaximumSize = IsOptionFullSize ? new Size(w,h) : this.currentVideoResolutionMax.FrameSize;
                    videoSourcePlayer1.Size = IsOptionFullSize ? new Size(w, h) : this.currentVideoResolution.FrameSize;
                    videoSourcePlayer1.VideoSource = videoSource1;
                    videoSourcePlayer1.Start();


                    //videoSource1.DisplayPropertyPage(IntPtr.Zero);
                    // reset stop watch
                    stopWatch = null;
                }
                UpdateSize(panel1);
                //Gọi lần 2 để cập nhật lại vị trí
                UpdateSize(panel1);
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

        public List<CameraDevice> GetCameraDevices()
        {
            return CameraDevices;
        }

        public void Reload()
        {
            try
            {
                int videoCounts = videoDevices.Count;
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices != null && videoDevices.Count == 0)
                {
                    throw new ArgumentNullException("videoDevices.Count == 0");
                }
                if (videoDevices.Count != videoCounts)
                {
                    Init();
                }

                if ((cboCameraSource1.EditValue ?? "").ToString() != (currentCameraDevice1 == null ? "" : currentCameraDevice1.MonikerString))
                {
                    isChangeCamera1 = true;

                    videoSourcePlayer1.SignalToStop();
                    videoSourcePlayer1.WaitForStop();

                    videoSourcePlayer1.Stop();
                    videoSourcePlayer1.WaitForStop();

                    currentCameraDevice1 = CameraDevices.FirstOrDefault(o => o.MonikerString == (cboCameraSource1.EditValue ?? "").ToString());
                    if (currentCameraDevice1 == null)
                    {
                        cboCameraSource1.EditValue = null;
                    }
                }


                //videoSourcePlayer1.SignalToStop();
                //videoSourcePlayer1.WaitForStop();

                VideoCaptureDevice videoSource1 = videoSourcePlayer1.VideoSource as VideoCaptureDevice;
                if (isChangeCamera1)
                {
                    if (currentCameraDevice1 != null)
                    {
                        videoSource1 = new VideoCaptureDevice(currentCameraDevice1.MonikerString);
                        videoSource1.DesiredFrameRate = 10;

                        videoSourcePlayer1.VideoSource = videoSource1;
                        videoSourcePlayer1.Start();
                    }
                    else
                    {
                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();
                    }
                    isChangeCamera1 = false;
                }

                if (stopWatch == null)
                {
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                }
                else
                {
                    stopWatch.Stop();
                    stopWatch.Reset();
                    stopWatch.Start();
                }


                if (cboCameraSource1.EditValue != null)
                {
                    isChangeResolution = false;
                    VideoResolutionDevices = VideoResolutionDeviceLoader.GetVideoResolutionDevices(cboCameraSource1.EditValue.ToString());

                    var vrd = this.VideoResolutionDevices.OrderByDescending(k => k.VideoCapabilities.FrameSize.Width).FirstOrDefault();
                    if (vrd != null && vrd.VideoCapabilities != null)
                    {
                        this.currentVideoResolution = vrd != null ? vrd.VideoCapabilities : null;
                        CaptureResolutionStorage.ChangeCaptureResolution(this.currentVideoResolution.FrameSize.ToString());
                        cboResolutionDevice.EditValue = this.currentVideoResolution.FrameSize.ToString();
                        videoSource1.VideoResolution = this.currentVideoResolution;
                        videoSourcePlayer1.BackgroundImageLayout = ImageLayout.Zoom;
                        videoSourcePlayer1.MaximumSize = IsOptionFullSize ? new Size(w, h) : this.currentVideoResolution.FrameSize;
                        videoSourcePlayer1.Size = IsOptionFullSize ? new Size(w, h) : this.currentVideoResolution.FrameSize;
                    }

                    isChangeResolution = true;
                }
                else
                {
                    cboResolutionDevice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                isChangeCamera1 = false;
                //videoSourcePlayer1.SignalToStop();
                //videoSourcePlayer1.WaitForStop();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        #endregion

        #region Public method
        public UCCamera()
        {
            InitializeComponent();
            try
            {
                CreateVideoPlayer();
                Init();
                SetupCamera("");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public UCCamera(DelegateCaptureImage captureImage)
            : this(captureImage, "")
        {

        }

        public UCCamera(DelegateCaptureImage captureImage, bool IsFullScreen)
    : this(captureImage, "", IsFullScreen)
        {

        }
        public UCCamera(DelegateCaptureImage captureImage, bool IsFullScreen, bool IsFullSize)
: this(captureImage, "", IsFullScreen, IsFullSize)
        {

        }
        public UCCamera(DelegateCaptureImage captureImage, string camMonikerString)
        {
            InitializeComponent();
            try
            {
                CreateVideoPlayer();
                this.CaptureImage1 = captureImage;

                // show device list
                Init();
                SetupCamera(camMonikerString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
        public UCCamera(DelegateCaptureImage captureImage, string camMonikerString, bool IsFullScreen)
        {
            InitializeComponent();
            try
            {
                this.IsOptionFullScreen = IsFullScreen;
                CreateVideoPlayer();
                this.CaptureImage1 = captureImage;

                // show device list
                Init();
                SetupCamera(camMonikerString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
        public UCCamera(DelegateCaptureImage captureImage, string camMonikerString, bool IsFullScreen, bool IsFullSize)
        {
            InitializeComponent();
            try
            {
                this.IsOptionFullScreen = IsFullScreen;
                this.IsOptionFullSize = IsFullSize;
                CreateVideoPlayer();
                this.CaptureImage1 = captureImage;

                // show device list
                Init();
                SetupCamera(camMonikerString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void CreateVideoPlayer()
        {
            try
            {
                this.videoSourcePlayer1 = new VideoSourcePlayer();
                this.videoSourcePlayer1.BackColor = System.Drawing.SystemColors.ControlDark;
                this.videoSourcePlayer1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                this.videoSourcePlayer1.ForeColor = System.Drawing.Color.White;
                this.videoSourcePlayer1.Name = "videoSourcePlayer1";
                this.videoSourcePlayer1.VideoSource = null;
                if (IsOptionFullScreen)
                    this.videoSourcePlayer1.Dock = DockStyle.Fill;
                this.panel1.Controls.Add(videoSourcePlayer1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void SetDelegate(DelegateCaptureImage captureImage)
        {
            try
            {
                this.CaptureImage1 = captureImage;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void SetDisable()
        {
            try
            {
                layoutControlItem3.TextVisible = false;
                layoutControlItem4.TextVisible = false;
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
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Stop cameras
        public void StopCameras()
        {
            try
            {
                videoSourcePlayer1.SignalToStop();

                videoSourcePlayer1.WaitForStop();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void CaptureCam1()
        {
            try
            {
                //videoSourcePlayer1.;
                var frame = videoSourcePlayer1.GetCurrentVideoFrame();
                if (IsAutoSaveImageInStore)
                {
                    string savePath = System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig, DateTime.Now.ToString("yyyyMMdd"));
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    string fileName = String.Format("{0}_{1}.jpg", this.ClientCode, DateTime.Now.ToString("HHmmssfff"));

                    frame.Save(System.IO.Path.Combine(savePath, fileName), ImageFormat.Jpeg);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => savePath), savePath)
                        +
                        Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                }

                //saving image
                if (CaptureImage1 != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        frame.Save(stream, ImageFormat.Jpeg);
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
                        frame.Save(fs, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void Start()
        {
            try
            {
                StartCameras();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void Stop()
        {
            try
            {
                StopCameras();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void SetClientCode(string clientCode)
        {
            this.ClientCode = clientCode;
        }
        #endregion

        private void cboCameraSource1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
                {
                    VideoCaptureDevice videoSource1 = videoSourcePlayer1.VideoSource as VideoCaptureDevice;//TODO
                    videoSource1.DisplayPropertyPage(IntPtr.Zero);//TODO
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
                        StopCameras();
                        StartCameras();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVideoResolution), currentVideoResolution));
                }
                //isChangeResolution = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            try
            {
                Panel panel = sender as Panel;
                UpdateSize(panel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        int w = 0;
        int h = 0;
        private void UpdateSize(Panel panel)
        {
            try
            {
                Size clientSize = panel.ClientSize;
                w = clientSize.Width;
                if (IsOptionFullSize)
                {
                    h = clientSize.Height;
                    videoSourcePlayer1.MaximumSize = IsOptionFullSize ? new Size(w, h) : this.currentVideoResolutionMax.FrameSize;
                    videoSourcePlayer1.Size = IsOptionFullSize ? new Size(w, h) : this.currentVideoResolution.FrameSize;
                    return;
                }
                if (IsOptionFullScreen || videoSourcePlayer1.MaximumSize.Height == 0 || videoSourcePlayer1.MaximumSize.Width == 0)
                    return;
                h = videoSourcePlayer1.MaximumSize.Height * w / videoSourcePlayer1.MaximumSize.Width;
                if (h > clientSize.Height)
                {
                    h = clientSize.Height;
                    w = videoSourcePlayer1.MaximumSize.Width * h / videoSourcePlayer1.MaximumSize.Height;
                }
                videoSourcePlayer1.SetBounds((clientSize.Width - videoSourcePlayer1.Width) / 2, (clientSize.Height - videoSourcePlayer1.Height) / 2, (int)w, (int)h);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCamera_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                this.SuspendLayout();
                this.UpdateSize(panel1);
                this.ResumeLayout();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
