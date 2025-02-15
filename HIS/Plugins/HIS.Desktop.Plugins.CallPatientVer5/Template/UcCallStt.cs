using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.CallPatientVer5.Class;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientVer5.Template
{
    public enum ShowType
    {
        Old,
        New
    }
    public enum Temp
    {
        Now,
        Next
    }
    public partial class UcCallStt : UserControl
    {
        ServiceReq1ADO serviceReqAdo { get; set; }
        Temp Temp { get; set; }
        public UcCallStt(Temp temp)
        {
            InitializeComponent();
            Temp = temp;
        }

        private void UcCallStt_Load(object sender, EventArgs e)
        {

            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public void SetConfigDisplay(DisplayOptionADO ado)
        {

            try
            {
                lblCaption.Appearance.ForeColor = ado.ColorTittle;
                lblStt.BackColor = ado.ColorBackround;
                lblPatient.BackColor = ado.ColorBackround;
                lblPriority.BackColor = ado.ColorBackround;
                lblCaption.Appearance.BackColor = ado.ColorBackround;
                if (Temp == Temp.Now)
                {
                    lblCaption.Text = ado.Title;
                    lblPatient.Appearance.ForeColor = ado.ColorSTT;
                    lblPriority.Appearance.ForeColor = ado.ColorSTT;
                    lblStt.Appearance.ForeColor = ado.ColorSTT;
                }
                else
                {
                    lblCaption.Text = ado.TitleSTTNext;
                    lblStt.Appearance.ForeColor = ado.ColorSTTNext;
                    lblPatient.Appearance.ForeColor = ado.ColorSTTNext;
                    lblPriority.Appearance.ForeColor = ado.ColorSTTNext;
                }
                lblPriority.Appearance.ForeColor = ado.ColorPriority;
                lblStt.Appearance.Font = new Font(new FontFamily("Arial"), (float)(ado.SizeSTT ?? 0), FontStyle.Bold);
                lblCaption.Appearance.Font = new Font(new FontFamily("Arial"), (float)(ado.SizeTitleSTT ?? 0), FontStyle.Bold);
                lblPatient.Appearance.Font = new Font(new FontFamily("Arial"), (float)(ado.SizeContentSTT ?? 0), FontStyle.Bold);
                lblPriority.Appearance.Font = new Font(new FontFamily("Arial"), (float)(ado.SizeContentSTT ?? 0), FontStyle.Bold);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public void Reload(ServiceReq1ADO serviceReq1ADO)
        {
            try
            {
                this.serviceReqAdo = serviceReq1ADO;
                if (serviceReq1ADO == null)
                {
                    lblStt.Text = lblPatient.Text = null;
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lblStt.Text = serviceReq1ADO.NUM_ORDER + "";
                    lblPatient.Text = string.Format("{0} ({1})", serviceReq1ADO.TDL_PATIENT_NAME, serviceReq1ADO.TDL_PATIENT_DOB.ToString().Substring(0,4));
                    if (serviceReq1ADO.PRIORITY != null && serviceReq1ADO.PRIORITY > 0)
                    {
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public ServiceReq1ADO GetServiceReqAdo()
        {
            return serviceReqAdo;
        }
    }
}
