using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ImageLib.Core
{
    class CameraLoader
    {
        public static void LoadDataToComboDevice(DevExpress.XtraEditors.LookUpEdit cboCamera, object data)
        {
            try
            {
                cboCamera.Properties.DataSource = data;
                cboCamera.Properties.DisplayMember = "CameraName";
                cboCamera.Properties.ValueMember = "MonikerString";
                cboCamera.Properties.ForceInitialize();
                cboCamera.Properties.Columns.Clear();
                cboCamera.Properties.Columns.Add(new LookUpColumnInfo("CameraName", "", 200));
                //cboCamera.Properties.Columns.Add(new LookUpColumnInfo("MonikerString", "", 200));
                cboCamera.Properties.ShowHeader = false;
                cboCamera.Properties.ImmediatePopup = true;
                cboCamera.Properties.DropDownRows = 10;
                cboCamera.Properties.PopupWidth = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboResolution(DevExpress.XtraEditors.LookUpEdit cboResolution, object data)
        {
            try
            {
                cboResolution.Properties.DataSource = data;
                cboResolution.Properties.DisplayMember = "FrameSizeDisplay";
                cboResolution.Properties.ValueMember = "FrameSizeString";
                cboResolution.Properties.ForceInitialize();
                cboResolution.Properties.Columns.Clear();
                //cboResolution.Properties.Columns.Add(new LookUpColumnInfo("FrameSizeString", "", 200));
                cboResolution.Properties.Columns.Add(new LookUpColumnInfo("FrameSizeDisplay", "", 200));
                cboResolution.Properties.ShowHeader = false;
                cboResolution.Properties.ImmediatePopup = true;
                cboResolution.Properties.DropDownRows = 10;
                cboResolution.Properties.PopupWidth = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
