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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.InterconnectionPrescription.ADO;
using HIS.Desktop.Plugins.InterconnectionPrescription.Resources;
using HIS.ERXConnect;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InterconnectionPrescription.InterconnectionPrescription
{
    public partial class frmInterconnectionPrescription : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        const int MaxReq = 100;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<HIS_EMPLOYEE> ListEmployee;
        string SysConfigValue;
        string LoginName;
        HIS_BRANCH CurrBranch;
        #endregion

        #region Construct
        public frmInterconnectionPrescription(Inventec.Desktop.Common.Modules.Module moduleData, long data)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.currentModule = moduleData;
                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void frmInterconnectionPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                ProcessLoadData();
                LoadCboStatus();
                SetDataDefault();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadData()
        {
            try
            {
                ListEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>();
                LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = ListEmployee.FirstOrDefault(p => p.LOGINNAME == LoginName);
                chkAll.Enabled = employee != null && employee.IS_ADMIN == (short)1;
                CurrBranch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                SysConfigValue = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.InterconnectionPrescription.SysConfig");

                if (String.IsNullOrWhiteSpace(CurrBranch.HEIN_MEDI_ORG_CODE))
                {
                    XtraMessageBox.Show(ResourceLanguageManager.ChuaKhaiBaoThongTinMaCoSo, ResourceLanguageManager.ThongBao);
                    btnPost.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HIS_SERVICE_REQ>> apiResult = null;
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                SetFilterNavBar(ref filter);

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ__GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>)apiResult.Data;
                    if (data != null)
                    {
                        List<ServiceReqADO> dataRepaired = new List<ServiceReqADO>();
                        data.ForEach(o => dataRepaired.Add(new ServiceReqADO(o)));

                        gridView1.GridControl.DataSource = dataRepaired;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisServiceReqFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }

                if (cboStatus.EditValue != null)
                {
                    filter.SERVICE_REQ_STT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboStatus.EditValue.ToString());
                }

                if (!chkAll.Checked)
                {
                    filter.REQUEST_LOGINNAME__EXACT = LoginName;
                }

                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_DATE_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_DATE_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                };
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                this.txtSearch.Text = "";
                this.cboStatus.EditValue = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
                this.dtTimeFrom.EditValue = DateTime.Now;
                this.dtTimeTo.EditValue = DateTime.Now;
                this.chkAll.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboStatus()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_STT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_STT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboStatus, data, controlEditorADO);
                this.cboStatus.EditValue = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPost.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.btnPost.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStatus.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.barBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnRefresh.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.barBtnRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInterconnectionPrescription.chkAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataDefault();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPost.Enabled)
                    return;
                if (string.IsNullOrWhiteSpace(SysConfigValue))
                {
                    XtraMessageBox.Show(ResourceLanguageManager.NoAddress, ResourceLanguageManager.ThongBao);
                    return;
                }

                if (SysConfigValue.Split('|').Count() < 3)
                {
                    XtraMessageBox.Show(ResourceLanguageManager.ErrorErxConfig, ResourceLanguageManager.ThongBao);
                    return;
                }

                var rowHandles = gridView1.GetSelectedRows();
                List<HIS_SERVICE_REQ> listServiceReq = new List<HIS_SERVICE_REQ>();
                List<long> serviceReqIds = new List<long>();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (ServiceReqADO)gridView1.GetRow(i);
                        if (row != null)
                        {
                            serviceReqIds.Add(row.ID);
                        }
                    }
                }
                if (serviceReqIds.Count() > 0)
                {
                    int step = 0;
                    while (serviceReqIds.Count() - step > 0)
                    {
                        var ids = serviceReqIds.Skip(step).Take(MaxReq).ToList();
                        HisServiceReqFilter serviceReqFt = new HisServiceReqFilter();
                        serviceReqFt.IDs = ids;
                        var listServiceReqTmp = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFt, null);
                        if (listServiceReqTmp != null && listServiceReqTmp.Count > 0)
                            listServiceReq.AddRange(listServiceReqTmp);
                        step += MaxReq;
                    }
                }

                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    var lstWarning = listServiceReq.Where(o => o.IS_SENT_ERX == 1);
                    if (lstWarning != null && lstWarning.Count() > 0 && XtraMessageBox.Show(String.Format(Resources.ResourceLanguageManager.CacYLenhDaDuocDongBoBanCoMuonTiepTucKhong, String.Join(",", lstWarning.Select(o => o.SERVICE_REQ_CODE))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No) return;

                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    DataResult sendResult = new ProcessSendToErx(listServiceReq, ListEmployee, SysConfigValue, CurrBranch.HEIN_MEDI_ORG_CODE).Send();

                    List<long> serviceReqIdToSend = new List<long>();

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sendResult), sendResult));
                    if (sendResult != null)
                    {
                        success = sendResult.Success;
                        var failedDatas = sendResult.Datas.Where(o => !o.Success).ToList();
                        var successDatas = sendResult.Datas.Where(o => o.Success).ToList();

                        var serviceReqSent = listServiceReq.Where(o => successDatas.Select(i => i.ServiceReqCode).Contains(o.SERVICE_REQ_CODE)
                                                                    && failedDatas.Select(i => i.ServiceReqCode).Contains(o.SERVICE_REQ_CODE) == false).ToList();
                        serviceReqIdToSend = serviceReqSent.Select(s => s.ID).ToList();

                        List<HIS_SERVICE_REQ> updates = new List<HIS_SERVICE_REQ>();
                        if (serviceReqIdToSend != null && serviceReqIdToSend.Count > 0)
                        {
                            foreach (var item in serviceReqIdToSend)
                            {
                                updates.Add(new HIS_SERVICE_REQ() { ID = item, IS_SENT_ERX = 1 });
                            }
                        }

                        if (failedDatas != null && failedDatas.Count > 0)
                        {
                            var groupByServiceReqCode = failedDatas.GroupBy(o => o.ServiceReqCode).ToList();
                            foreach (var group in groupByServiceReqCode)
                            {
                                string errorMessage = String.Join("; ", group.ToList().Select(o => String.Join(",", o.ErrorMessage)));
                                var failedServiceReq = listServiceReq.FirstOrDefault(o => o.SERVICE_REQ_CODE == group.Key);
                                if (failedServiceReq != null)
                                {
                                    failedServiceReq.IS_SENT_ERX = 2;
                                    failedServiceReq.ERX_DESC = errorMessage;
                                    updates.Add(new HIS_SERVICE_REQ() { ID = failedServiceReq.ID, IS_SENT_ERX = 2, ERX_DESC = errorMessage });
                                }
                            }
                        }

                        int step = 0;
                        while (updates.Count() - step > 0)
                        {
                            var objs = updates.Skip(step).Take(MaxReq).ToList();
                            step += MaxReq;
                            var apiResult = new BackendAdapter(param).Post<List<HIS_SERVICE_REQ>>("api/HisServiceReq/UpdateSentErx", ApiConsumers.MosConsumer, objs, param);
                            if (apiResult != null)
                            {
                                bool checkSucces = false;
                                foreach (var updatedServiceReq in apiResult)
                                {
                                    if (updatedServiceReq.IS_SENT_ERX == 1)
                                    {
                                        checkSucces = true;
                                        break;
                                    }
                                }

                                success = checkSucces;
                            }
                        }
                    }

                    if (sendResult.Datas != null && sendResult.Datas.Count > 0)
                    {
                        param.Messages.AddRange(sendResult.Messages.Distinct().ToList());
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);

                    LoadDataToGridControl();
                }
                else if (rowHandles != null && rowHandles.Count() > 0)
                {
                    XtraMessageBox.Show(ResourceLanguageManager.NoPrescription_2, ResourceLanguageManager.ThongBao);
                }
                else
                {
                    XtraMessageBox.Show(ResourceLanguageManager.NoPrescription, ResourceLanguageManager.ThongBao);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            WaitingManager.Hide();
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        else if (e.Column.FieldName == "STATUS_SENT_STR")
                        {
                            if (data.IS_SENT_ERX == 1)
                            {
                                e.Value = "Đẩy thành công";
                            }
                            else if (data.IS_SENT_ERX == 2)
                            {
                                e.Value = "Đẩy lỗi";
                            }
                            else if (data.IS_SENT_ERX != 1)
                            {
                                e.Value = "Chưa đẩy";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short? data = (short?)view.GetRowCellValue(e.RowHandle, "IS_SENT_ERX");
                    if (e.Column.FieldName == "STATUS_SENT_STR")
                    {
                        if (data == 1)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (data == 2)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
