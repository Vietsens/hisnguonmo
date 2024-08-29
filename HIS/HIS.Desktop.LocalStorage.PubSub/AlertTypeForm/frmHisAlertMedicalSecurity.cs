using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAlertByConfig.HisAlertByConfig
{
    public partial class frmHisAlertMedicalSecurity : HIS.Desktop.Utility.FormBase
    {
        private static frmHisAlertMedicalSecurity instance = null;
        private static readonly object padlock = new object();
        frmHisAlertMedicalSecurity()
        {
            InitializeComponent();

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public static frmHisAlertMedicalSecurity Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new frmHisAlertMedicalSecurity();
                    }
                    return instance;
                }
            }
        }
        public Action<bool> GetSpeech { get; set; }
        public HIS_ALERT Alert { get; set; }
        private void frmHisAlertByConfig_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = Alert.TITLE;
                lblTitle.Text = Alert.CONTENT;
                lblContent.Text = "XIN CẢM ƠN!";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMedical_Click(object sender, EventArgs e)
        {

            try
            {
                var AlertRecei = new BackendAdapter(new CommonParam()).Post<bool>("/api/HisAlert/Receiver", ApiConsumers.MosConsumer, Alert.ID, null);
                if (AlertRecei)
                {
                    instance = null;
                    if (GetSpeech != null)
                        GetSpeech(false);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSecurity_Click(object sender, EventArgs e)
        {

            try
            {
                var AlertReject = new BackendAdapter(new CommonParam()).Post<bool>("/api/HisAlert/Reject", ApiConsumers.MosConsumer, Alert.ID, null);
                if (AlertReject)
                {
                    instance = null;
                    if (GetSpeech != null)
                        GetSpeech(false);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }
}
