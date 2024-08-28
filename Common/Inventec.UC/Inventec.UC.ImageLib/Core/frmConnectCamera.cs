using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AForge.Video.DirectShow;
using System.Diagnostics;
using AForge.Video;
using System.IO;
using System.Drawing.Imaging;
using Inventec.UC.ImageLib.Base;

namespace Inventec.UC.ImageLib.Core
{
    public partial class frmConnectCamera : DevExpress.XtraEditors.XtraForm
    {
        DelegateCaptureImage CaptureImage { get; set; }
        UCCamera ucCamera1;
        #region Construct
        public frmConnectCamera()
        {
            try
            {
                InitializeComponent();
                ucCamera1 = new UCCamera();
                this.Controls.Add(ucCamera1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        public frmConnectCamera(DelegateCaptureImage captureImage)
        {
            try
            {
                InitializeComponent();
                CaptureImage = captureImage;
                ucCamera1 = new UCCamera(CaptureImage);
                this.Controls.Add(ucCamera1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
        #endregion

        #region Private method
        // On form closing
        private void frmConnectCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ucCamera1.StopCameras();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void frmConnectCamera_Load(object sender, EventArgs e)
        {

        }

        private void btnCaptureCam1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ucCamera1.CaptureCam1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void btnStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ucCamera1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void btnStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ucCamera1.Stop();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                if (tileItems != null && tileItems.Count > 0)
                {
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
    }
}