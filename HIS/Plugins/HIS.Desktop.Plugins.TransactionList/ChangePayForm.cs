using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class ChangePayForm : Form
    {

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TRANSACTION currentTrans;
        long currentRoomID;
        List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
        List<V_HIS_TRANSACTION> listTransactionSelect = new List<V_HIS_TRANSACTION>();
        public delegate void degateloadlstTrans(string value);
        public degateloadlstTrans loadPayForm { get; set; }
        public ChangePayForm(V_HIS_TRANSACTION transactionData, long RoomID, degateloadlstTrans loadPay, List<V_HIS_TRANSACTION> listTransaction)
        {
            InitializeComponent();
            try
            {
                 this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                 this.StartPosition = FormStartPosition.CenterScreen;
                 currentTrans = transactionData;
                 currentRoomID = RoomID;
                 loadPayForm = loadPay;
                 listTransactionSelect = listTransaction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangePayForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadcboPayForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadcboPayForm()
        {
            try
            {
                payForm = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var itemToRemove = payForm.FirstOrDefault(item => item.ID == currentTrans.PAY_FORM_ID);
                if (itemToRemove != null)
                {
                    payForm.Remove(itemToRemove);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayForm, payForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                MOS.SDO.TransactionChangePayFormSDO ado = new MOS.SDO.TransactionChangePayFormSDO();
                if (listTransactionSelect != null && listTransactionSelect.Count > 0)
                {
                    ado.TransactionIds = listTransactionSelect.Select(o => o.ID).ToList();
                }
                else
                { 
                    List<long> lstTransID = new List<long>();
                    lstTransID.Add(currentTrans.ID);
                    ado.TransactionIds = lstTransID;
                }
                ado.PayFormId = Convert.ToInt64(cboPayForm.EditValue);
                ado.RequestRoomId = currentRoomID;
                ado.IsNeedUnlock = true;

                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_TRANSACTION>>("api/HisTransaction/ChangePayForm", ApiConsumers.MosConsumer, ado, param);

                if (rs != null)
                {
                    MessageManager.Show(this,param, true);
                    loadPayForm(cboPayForm.Text);
                }
                else
                {
                    MessageManager.Show(this, param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
