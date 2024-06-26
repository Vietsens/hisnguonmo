/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

using DevExpress.XtraEditors.DXErrorProvider;

using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Logging;

using System.Resources;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.Filter;

namespace HIS.Desktop.Plugins.DrugInterventionInfo
{
    public partial class frmDrugInterventionInfo : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        string defaultAddress = "";
        string configAddress = "";
        HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ();
        long id;
        public frmDrugInterventionInfo(Inventec.Desktop.Common.Modules.Module _moduleData, long _id):base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                this.id = _id;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDrugInterventionInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                 txtAdress.Text = defaultAddress;
                configAddress = HisConfigs.Get<string>("MOS.HIS_DRUG_INTERVENTION.CONNECTION_INFO");
                CommonParam param = new CommonParam();
                if (id > 0)
                {
                    HisServiceReqFilter srfilter = new MOS.Filter.HisServiceReqFilter();
                    srfilter.ID = id;
                    var dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srfilter, param);
                    if (dataServiceReq != null && dataServiceReq.Count > 0)
                    {
                        currentServiceReq = dataServiceReq.FirstOrDefault();
                        HisDrugInterventionFilter filter = new MOS.Filter.HisDrugInterventionFilter();
                        filter.SERVICE_REQ_ID = currentServiceReq.ID;
                        var data = new BackendAdapter(param).Get<List<HIS_DRUG_INTERVENTION>>("api/HisDrugIntervention/Get", ApiConsumers.MosConsumer, filter, param);
                        gridControl1.BeginUpdate();
                        if (data != null && data.Count > 0)
                        {
                            gridControl1.DataSource = data;

                        }
                        else
                        {
                            gridControl1.DataSource = null;
                        }
                        gridControl1.EndUpdate();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_DRUG_INTERVENTION pData = (MOS.EFMODEL.DataModels.HIS_DRUG_INTERVENTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "PHARMACIST_USERNAME_str")
                    {
                        e.Value = pData.PHARMACIST_USERNAME.ToUpper();
                    }

                    else if (e.Column.FieldName == "INTERVENTION_TIME_str")
                    {
                        try
                        {

                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                }

                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_DRUG_INTERVENTION data = (HIS_DRUG_INTERVENTION)gridView1.GetFocusedRow();
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.SESSION_CODE))
                    {
                        txtAdress.Text = defaultAddress + configAddress + "/InterventionCard?SessionId=" + data.SESSION_CODE;
                        webBrowser1.Navigate(txtAdress.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentServiceReq.SESSION_CODE))
                {
                    txtAdress.Text = defaultAddress + configAddress + "/InteractionCheck/InteractionCheckConfirmation?sessionId=" + currentServiceReq.SESSION_CODE;
                    webBrowser1.Navigate(txtAdress.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentServiceReq.SESSION_CODE))
                {
                    txtAdress.Text = defaultAddress + configAddress + "/DrugCheck?sessionId=" + currentServiceReq.SESSION_CODE;
                    webBrowser1.Navigate(txtAdress.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
