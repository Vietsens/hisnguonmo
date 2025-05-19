using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.PrepareAndExport.Popup
{
    public partial class Requirements : Form
    {
        private long medistockId = 0;
        private List<HIS_EXP_MEST> lstAll { get; set; }
        private List<HIS_TREATMENT> allTreatments = new List<HIS_TREATMENT>();
        public Requirements(List<HIS_EXP_MEST> lstAll)
        {
            InitializeComponent();
            this.lstAll = lstAll;
        }

        private void Requirements_Load(object sender, EventArgs e)
        {
            SetIcon();
            LoadListHisTreatment();
        }

        private void LoadListHisTreatment()
        {
            try
            {   
                if(lstAll!=null && lstAll.Count > 0)
                {
                    var treatmentIds = lstAll
                          .Select(x => x.TDL_TREATMENT_ID??0)
                          .Distinct()
                          .ToList();

                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.IDs = treatmentIds;
                    filter.IS_REASON_UNFINISH = 1;
                    filter.IS_CONFIRM_UNFINISH = 0;
                    filter.IS_PAUSE = true;
                    
                    List<HIS_TREATMENT> treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    gridControl2.DataSource = null;
                    if (treatment != null && treatment.Count > 0)
                    {
                        allTreatments = treatment;
                        gridControl2.DataSource = treatment;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void resApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_TREATMENT data = (HIS_TREATMENT)gridView2.GetFocusedRow();

                if (data != null)
                {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        
                        if (gridControl2.DataSource != null)
                        {
                            var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("(api/HisTreatment/ConfirmUnfinish", ApiConsumer.ApiConsumers.MosConsumer, data.ID, param);
                            if (apiresul != null)
                            {
                                success = true;
                                LoadListHisTreatment();
                            }
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void textSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = textSearch.Text?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    gridControl2.DataSource = allTreatments;
                }
                else
                {
                    var filtered = allTreatments
                        .Where(t =>
                            !string.IsNullOrEmpty(t.TREATMENT_CODE) &&
                            t.TREATMENT_CODE.ToLower().Contains(keyword)
                        )
                        .ToList();

                    gridControl2.DataSource = filtered;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
