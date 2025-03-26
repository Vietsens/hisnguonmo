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
using System.Linq;
using DevExpress.XtraEditors;

namespace Inventec.UC.ImageLib.Core
{
    public partial class UCDoubleCamera : UserControl
    {
        #region Private variable
        // list of video devices
        FilterInfoCollection videoDevices;
        List<CameraDevice> CameraDevices;
        CameraDevice currentCameraDevice1;
        CameraDevice currentCameraDevice2;
        bool isChangeCamera1;
        bool isChangeCamera2;
        // stop watch for measuring fps
        private Stopwatch stopWatch = null;
        DelegateCaptureImage CaptureImage1 { get; set; }
        DelegateCaptureImage CaptureImage2 { get; set; }
        #endregion

        #region Public variable
        /// <summary>
        /// Cau hinh co tu dong luu anh vao thu muc cau hinh hay khong
        /// True: tu dong luu anh vao thu muc cau hinh
        /// False: Khong tu dong luu anh ve local, chi tra ve anh duoi dang stream hoac chon duong dan luu anh
        /// Mac dinh la false
        /// </summary>
        public bool IsAutoSaveImageInStore { get; set; }
        #endregion

        #region Private method
        // Start cameras
        private void StartCameras()
        {
            try
            {
                if (videoDevices != null && videoDevices.Count > 0)
                {
                    // create first video source
                    if (cboCameraSource1.Enabled && cboCameraSource1.EditValue != null)
                    {
                        VideoCaptureDevice videoSource1 = new VideoCaptureDevice(cboCameraSource1.EditValue.ToString());
                        videoSource1.DesiredFrameRate = 10;

                        videoSourcePlayer1.VideoSource = videoSource1;
                        videoSourcePlayer1.Start();
                    }

                    // create second video source
                    if (cboCameraSource2.Enabled && cboCameraSource2.EditValue != null)
                    {
                        System.Threading.Thread.Sleep(500);

                        VideoCaptureDevice videoSource2 = new VideoCaptureDevice(cboCameraSource2.EditValue.ToString());
                        videoSource2.DesiredFrameRate = 10;

                        videoSourcePlayer2.VideoSource = videoSource2;
                        videoSourcePlayer2.Start();
                    }

                    // reset stop watch
                    stopWatch = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void UCDoubleCamera_Load(object sender, EventArgs e)
        {

        }

        // On form closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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
                CameraLoader.LoadDataToComboDevice(cboCameraSource2, CameraDevices);

                isChangeCamera1 = false;
                isChangeCamera2 = false;
            }
            catch (Exception ex)
            {
                currentCameraDevice1 = null;
                currentCameraDevice2 = null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        void SetupCamera()
        {
            try
            {
                if (CameraDevices != null && CameraDevices.Count > 0)
                {
                    cboCameraSource1.EditValue = CameraDevices[0].MonikerString;
                    currentCameraDevice1 = CameraDevices[0];
                    if (videoDevices.Count > 1)
                    {
                        cboCameraSource2.EditValue = CameraDevices[1].MonikerString;
                        currentCameraDevice2 = CameraDevices[1];
                    }
                    else
                    {
                        currentCameraDevice2 = null;
                    }
                }
                else
                {
                    cboCameraSource1.EditValue = null;
                    cboCameraSource2.EditValue = null;
                    currentCameraDevice1 = null;
                    currentCameraDevice2 = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        #endregion

        #region Public method
        public UCDoubleCamera()
        {
            InitializeComponent();
            try
            {
                Init();
                //SetupCamera();
                // show device list
                //ConnectDeviceProcessor(-1, "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public UCDoubleCamera(DelegateCaptureImage captureImage1, DelegateCaptureImage captureImage2)
        {
            InitializeComponent();
            try
            {
                this.CaptureImage1 = captureImage1;
                this.CaptureImage2 = captureImage2;

                Init();
                //SetupCamera();
                // show device list
                //ConnectDeviceProcessor(-1, "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
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
                    currentCameraDevice1 = CameraDevices.FirstOrDefault(o => o.MonikerString == (cboCameraSource1.EditValue ?? "").ToString());
                    if (currentCameraDevice1 == null)
                    {
                        cboCameraSource1.EditValue = null;

                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();
                    }
                    if ((cboCameraSource1.EditValue ?? "").ToString() == (cboCameraSource2.EditValue ?? "").ToString())
                    {
                        currentCameraDevice2 = null;
                        cboCameraSource2.EditValue = null;
                        isChangeCamera2 = true;

                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer1.VideoSource = null;
                        videoSourcePlayer2.VideoSource = null;
                    }
                    else if (!String.IsNullOrWhiteSpace((cboCameraSource2.EditValue ?? "").ToString()))
                    {
                        isChangeCamera2 = true;

                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer1.VideoSource = null;
                        videoSourcePlayer2.VideoSource = null;
                    }
                }

                if ((cboCameraSource2.EditValue ?? "").ToString() != (currentCameraDevice2 == null ? "" : currentCameraDevice2.MonikerString))
                {
                    isChangeCamera2 = true;
                    currentCameraDevice2 = CameraDevices.FirstOrDefault(o => o.MonikerString == (cboCameraSource2.EditValue ?? "").ToString());
                    if (currentCameraDevice2 == null)
                    {
                        cboCameraSource2.EditValue = null;
                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();
                    }
                    if ((cboCameraSource2.EditValue ?? "").ToString() == (cboCameraSource1.EditValue ?? "").ToString())
                    {
                        currentCameraDevice1 = null;
                        cboCameraSource1.EditValue = null;
                        isChangeCamera1 = true;

                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer1.VideoSource = null;
                        videoSourcePlayer2.VideoSource = null;
                    }
                    else if (!String.IsNullOrWhiteSpace((cboCameraSource2.EditValue ?? "").ToString()))
                    {
                        isChangeCamera1 = true;

                        videoSourcePlayer1.SignalToStop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer1.Stop();
                        videoSourcePlayer1.WaitForStop();

                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer1.VideoSource = null;
                        videoSourcePlayer2.VideoSource = null;
                    }
                }

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


                VideoCaptureDevice videoSource2 = videoSourcePlayer2.VideoSource as VideoCaptureDevice;
                if (isChangeCamera2)
                {
                    System.Threading.Thread.Sleep(500);
                    if (currentCameraDevice2 != null)
                    {
                        videoSource2 = new VideoCaptureDevice(currentCameraDevice2.MonikerString);
                        videoSource2.DesiredFrameRate = 10;

                        videoSourcePlayer2.VideoSource = videoSource2;
                        videoSourcePlayer2.Start();
                    }
                    else
                    {
                        videoSourcePlayer2.SignalToStop();
                        videoSourcePlayer2.WaitForStop();

                        videoSourcePlayer2.Stop();
                        videoSourcePlayer2.WaitForStop();
                    }
                    isChangeCamera2 = false;
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
            }
            catch (Exception ex)
            {
                isChangeCamera1 = false;
                //videoSourcePlayer1.SignalToStop();
                //videoSourcePlayer1.WaitForStop();

                isChangeCamera2 = false;
                //videoSourcePlayer2.SignalToStop();
                //videoSourcePlayer2.WaitForStop();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void SetDelegate(DelegateCaptureImage captureImage1, DelegateCaptureImage captureImage2)
        {
            try
            {
                this.CaptureImage1 = captureImage1;
                this.CaptureImage2 = captureImage2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        // Stop cameras
        public void StopCameras()
        {
            try
            {
                videoSourcePlayer1.SignalToStop();
                videoSourcePlayer2.SignalToStop();

                videoSourcePlayer1.WaitForStop();
                videoSourcePlayer2.WaitForStop();
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
                    if (!Directory.Exists(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig))
                    {
                        Directory.CreateDirectory(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig);
                    }
                    string fileName = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                    frame.Save(System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig, fileName), ImageFormat.Jpeg);
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public void CaptureCam2()
        {
            try
            {
                //videoSourcePlayer2;
                var frame = videoSourcePlayer2.GetCurrentVideoFrame();
                if (IsAutoSaveImageInStore)
                {
                    if (!Directory.Exists(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig))
                    {
                        Directory.CreateDirectory(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig);
                    }
                    string fileName = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                    frame.Save(System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LocalStoragePathConfig, fileName), ImageFormat.Jpeg);
                }
                else
                {
                    //saving image
                    if (CaptureImage2 != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            frame.Save(stream, ImageFormat.Jpeg);
                            if (stream.Length > 0)
                            {
                                stream.Position = 0;
                                CaptureImage2(stream);
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
        #endregion

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

        private void cboCameraSource2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCameraSource2.EditValue != null)
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

        private void cboCameraSource2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCameraSource2.EditValue != null)
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

        private void cboCameraSource1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCameraSource1.EditValue != null && !String.IsNullOrWhiteSpace(cboCameraSource1.EditValue.ToString()))
                {
                    cboCameraSource1.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboCameraSource1.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCameraSource2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCameraSource2.EditValue != null && !String.IsNullOrWhiteSpace(cboCameraSource2.EditValue.ToString()))
                {
                    cboCameraSource2.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboCameraSource2.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCameraSource1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCameraSource1.EditValue = null;
                    Reload();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCameraSource2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCameraSource2.EditValue = null;
                    Reload();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
