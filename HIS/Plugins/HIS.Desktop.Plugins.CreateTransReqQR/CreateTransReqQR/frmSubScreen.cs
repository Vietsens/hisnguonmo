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

namespace HIS.Desktop.Plugins.CreateTransReqQR.CreateTransReqQR
{
    public partial class frmSubScreen : Form
    {
        private V_HIS_TREATMENT treatment { get; set; }
        public frmSubScreen(V_HIS_TREATMENT treatment)
        {
            InitializeComponent();

            try
            {
                this.treatment = treatment;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public void dataGet(DataSubScreen data)
        {
            pictureEdit1.Image = data.image;
            lblAmount.Text = "Số tiền: " + "<b><color=red>" + data.amount + "</color></b>";
            lblStt.Text = "Trạng thái: " + "<b>" + data.status + "</b>";
        }

        private void frmSubScreen_Load(object sender, EventArgs e)
        {

            try
            {
                lblPatientName.Text = treatment.TDL_PATIENT_NAME;
                lblGenderName.Text = treatment.TDL_PATIENT_GENDER_NAME;
                lblAddress.Text = treatment.TDL_PATIENT_ADDRESS;
                lblDob.Text = treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
