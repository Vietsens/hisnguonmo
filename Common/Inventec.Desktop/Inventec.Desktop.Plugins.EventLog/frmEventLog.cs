using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.EventLog
{
    public partial class frmEventLog : HIS.Desktop.Utility.FormBase
    {
        UCEventLog uCEventLog;
        private UC.EventLogControl.ProcessHasException exceptionApi;
        private string treatmentCode;
        private string patientCode;
        private string serviceRequestCode;
        private string impMestCode;
        private string expMestCode;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public frmEventLog()
        {
            InitializeComponent();
        }

        public frmEventLog(UC.EventLogControl.ProcessHasException exceptionApi, string treatmentCode, string patientCode, string serviceRequestCode, string impMestCode, string expMestCode, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            // TODO: Complete member initialization
            this.exceptionApi = exceptionApi;
            this.treatmentCode = treatmentCode;
            this.patientCode = patientCode;
            this.serviceRequestCode = serviceRequestCode;
            this.impMestCode = impMestCode;
            this.expMestCode = expMestCode;
            this.moduleData = _moduleData;
        }

        private void frmEventLog_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (treatmentCode != null || patientCode != null || serviceRequestCode != null || impMestCode != null || expMestCode != null)
                {
                    this.uCEventLog = new UCEventLog(exceptionApi, treatmentCode, patientCode, serviceRequestCode, impMestCode, expMestCode, this.moduleData);
                    if (this.uCEventLog != null)
                    {
                        this.panelControl1.Controls.Add(this.uCEventLog);
                        this.uCEventLog.Dock = DockStyle.Fill;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
