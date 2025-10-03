using DevExpress.XtraWaitForm;
using Inventec.Common.WebApiClient;
using HIS.Desktop.Utility;
using Inventec.Common.Integrate;
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
using HIS.Desktop.ApiConsumer;
using Inventec.Core;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction
{
    public partial class frmAdjustmentTransaction : FormBase
    {

        V_HIS_TRANSACTION currentTransaction = new V_HIS_TRANSACTION();
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        public frmAdjustmentTransaction(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION tran) 
            : base(module)
        {
            InitializeComponent();
            this.currentTransaction = tran;
            this.currentModule = module;
        }

        private void FillPatient()
        {
            try
            {
                if (currentTransaction != null)
                {
                    CommonParam common = new CommonParam();
                    MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                    filter.PATIENT_CODE = currentTransaction.TDL_PATIENT_CODE;

                    var patients = new BackendAdapter(common).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, common);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
