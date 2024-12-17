using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    public partial class frmMessage : HIS.Desktop.Utility.FormBase
    {
        V_HIS_EXP_MEST currentExpMest { get; set; }
        //private Common.RefeshReference delegateRefresh { get; set; }
        public delegate void degateloadExpTime(long? value);
        public degateloadExpTime loadExpTime { get; set; }
        public frmMessage(V_HIS_EXP_MEST cuExpMest, degateloadExpTime LoadTime)
        {
            InitializeComponent();
            try
            {
                this.currentExpMest = cuExpMest;
                this.loadExpTime = LoadTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmMessage_Load(object sender, System.EventArgs e)
        {
            try
            {
                dteTime.DateTime = DateTime.Now;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                long timeDte = 0;
                long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (dteTime.EditValue != null && dteTime.DateTime != DateTime.MinValue)
                {
                    timeDte = Convert.ToInt64(dteTime.DateTime.ToString("yyyyMMddHHmmss"));
                    if (timeDte > timeNow)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Thời gian xuất lớn hơn thời gian hiện tại.",
                       Resources.ResourceMessage.ThongBao,
                       MessageBoxButtons.OK);
                        dteTime.Focus();
                        dteTime.SelectAll();
                        return;
                    }
                    else
                    {
                        loadExpTime(timeDte);
                        this.Close();
                    }
                }
            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
