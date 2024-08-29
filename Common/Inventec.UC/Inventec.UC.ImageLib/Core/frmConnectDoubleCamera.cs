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

namespace Inventec.UC.ImageLib.Core
{
    public partial class frmConnectDoubleCamera : DevExpress.XtraEditors.XtraForm
    {
        #region Construct
        public frmConnectDoubleCamera()
        {
            InitializeComponent();
        }
        #endregion

        #region Private method
        // On form closing
        private void frmConnectCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ucDoubleCamera1.StopCameras();
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
                ucDoubleCamera1.CaptureCam1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        private void btnCaptureCam2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ucDoubleCamera1.CaptureCam2();
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
                ucDoubleCamera1.Start();
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
                ucDoubleCamera1.Stop();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }
        #endregion
    }
}