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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InterconnectionPrescription.InterconnectionPrescription
{
    public partial class frmInterconnectionPrescription : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        const int MaxReq = 500;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<HIS_EMPLOYEE> ListEmployee;
        string SysConfigValue;
        string ClientAppConfigValue;
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
                // Key cũ: dùng cho SendPrescription (Url|HospitalLogin|HospitalPass)

                // Key mới: dùng cho SendQuantitySold (Url|AppKey|AppName|MaCoSo|TenCoSo|SDT|DiaChi)
                ClientAppConfigValue = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(
                    "HIS.Desktop.Plugins.InterconnectionPrescription.ClientAppConfig");
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

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (btnUpdateSale == null) return;
                int[] rows = gridView1.GetSelectedRows();
                btnUpdateSale.Enabled = (rows != null && rows.Length > 0);
                btnUpdateSale.ToolTip = "Cập nhật số lượng đã bán";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ValidateSysConfig(string sysConfig)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(sysConfig))
                    return "Chưa khai báo cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.SysConfig";

                string[] p = sysConfig.Split('|');
                if (p.Length < 3)
                    return "Cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.SysConfig không đúng định dạng: <Url>|<Mã liên thông BV>|<Mật khẩu>";

                if (String.IsNullOrWhiteSpace(p[0])) return "Thiếu Url hệ thống liên thông (SysConfig)";
                if (String.IsNullOrWhiteSpace(p[1])) return "Thiếu mã liên thông bệnh viện (SysConfig)";
                if (String.IsNullOrWhiteSpace(p[2])) return "Thiếu mật khẩu liên thông (SysConfig)";

                return "";
            }
            catch
            {
                return "Cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.SysConfig không hợp lệ";
            }
        }

        private string ValidateClientAppConfig(string clientAppConfig)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(clientAppConfig))
                    return "Chưa khai báo cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.ClientAppConfig";

                string[] p = clientAppConfig.Split('|');
                if (p.Length < 7)
                    return "Cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.ClientAppConfig không đúng định dạng: <Url>|<App-Key>|<App-Name>|<Mã cơ sở>|<Tên cơ sở>|<SĐT>|<Địa chỉ>";

                if (String.IsNullOrWhiteSpace(p[0])) return "Thiếu Url hệ thống liên thông (ClientAppConfig)";
                if (String.IsNullOrWhiteSpace(p[1])) return "Thiếu App-Key (ClientAppConfig)";
                if (String.IsNullOrWhiteSpace(p[2])) return "Thiếu App-Name (ClientAppConfig)";
                if (String.IsNullOrWhiteSpace(p[3])) return "Thiếu Mã cơ sở cung ứng (ClientAppConfig)";

                return "";
            }
            catch
            {
                return "Cấu hình HIS.Desktop.Plugins.InterconnectionPrescription.ClientAppConfig không hợp lệ";
            }
        }

        private void ParseSysConfig(string value, out string url, out string hospitalLogin, out string hospitalPass)
        {
            url = hospitalLogin = hospitalPass = "";

            string[] p = (value ?? "").Split('|');
            if (p.Length < 3) throw new Exception("SysConfig không đúng định dạng");

            url = (p[0] ?? "").Trim();
            hospitalLogin = (p[1] ?? "").Trim();
            hospitalPass = (p[2] ?? "").Trim();
        }

        private void ParseClientAppConfig(
    string value,
    out string urlQuantitySold,
    out string appKey,
    out string appName,
    out string maCoSo,
    out string tenCoSo,
    out string sdt,
    out string diaChi)
        {
            urlQuantitySold = appKey = appName = maCoSo = tenCoSo = sdt = diaChi = "";

            string[] p = (value ?? "").Split('|');
            if (p.Length < 7) throw new Exception("ClientAppConfig không đúng định dạng");

            urlQuantitySold = (p[0] ?? "").Trim();  
            appKey = (p[1] ?? "").Trim();
            appName = (p[2] ?? "").Trim();
            maCoSo = (p[3] ?? "").Trim();
            tenCoSo = (p[4] ?? "").Trim();
            sdt = (p[5] ?? "").Trim();
            diaChi = (p[6] ?? "").Trim();
        }

        private DataInput BuildSendQuantitySoldInput(
    string sysConfigValue,
    string clientAppConfigValue,
    HIS_BRANCH branch,
    List<HIS_EXP_MEST> listExpMest,
    List<HIS_TRANSACTION> listTransaction,
    List<HIS_SERVICE_REQ> listServiceReq,
    List<HIS_SERVICE_REQ_METY> listReqMety)
        {
            string sysUrl, hospitalLogin, hospitalPass;
            ParseSysConfig(sysConfigValue, out sysUrl, out hospitalLogin, out hospitalPass);

            string urlQuantitySold, appKey, appName, maCoSo, tenCoSo, sdt, diaChi;
            ParseClientAppConfig(clientAppConfigValue, out urlQuantitySold, out appKey, out appName, out maCoSo, out tenCoSo, out sdt, out diaChi);

            DataInput input = new DataInput();

            input.Url = sysUrl;                     
            input.UrlQuantitySold = urlQuantitySold; 

            input.HospitalLoginname = hospitalLogin;
            input.HospitalPassword = hospitalPass;

            input.MediOrgCode = (branch != null ? (branch.HEIN_MEDI_ORG_CODE ?? "") : "");

            input.AppKey = appKey;
            input.AppName = appName;

            input.MaCoSoCungUng = maCoSo;
            input.TenCoSoCungUng = tenCoSo;
            input.SDTCoSoCungUng = sdt;
            input.DiaChiCoSoCungUng = diaChi;

            input.ListExpMest = listExpMest ?? new List<HIS_EXP_MEST>();
            input.ListTransaction = listTransaction ?? new List<HIS_TRANSACTION>();
            input.ListServiceReq = listServiceReq ?? new List<HIS_SERVICE_REQ>();
            input.ListReqMety = listReqMety ?? new List<HIS_SERVICE_REQ_METY>();

            input.ListMedicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>() ?? new List<HIS_MEDICINE_TYPE>();
            input.ListEmplyee = BackendDataWorker.Get<HIS_EMPLOYEE>() ?? new List<HIS_EMPLOYEE>();

            return input;
        }


        private List<HIS_EXP_MEST> GetExpMestByTdlServiceReqCodes(List<string> serviceReqCodes)
        {
            List<HIS_EXP_MEST> result = new List<HIS_EXP_MEST>();
            if (serviceReqCodes == null || serviceReqCodes.Count == 0) return result;

            List<string> codes = serviceReqCodes
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            HashSet<string> fetched = new HashSet<string>();

            for (int i = 0; i < codes.Count; i++)
            {
                string code = codes[i];
                if (String.IsNullOrWhiteSpace(code)) continue;
                if (fetched.Contains(code)) continue;

                fetched.Add(code);

                HisExpMestFilter ft = new HisExpMestFilter();
                ft.TDL_SERVICE_REQ_CODE = code;

                List<HIS_EXP_MEST> data = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(
                    "api/HisExpMest/Get", ApiConsumers.MosConsumer, ft, null);

                if (data != null && data.Count > 0)
                    result.AddRange(data);
            }

            // ✅ loại trùng theo ID
            result = result.Where(x => x != null)
                           .GroupBy(x => x.ID)
                           .Select(g => g.First())
                           .ToList();

            return result;
        }



        private List<HIS_TRANSACTION> GetTransactionsAByEligibleExpMest(List<HIS_EXP_MEST> eligible)
        {
            List<HIS_TRANSACTION> result = new List<HIS_TRANSACTION>();
            if (eligible == null || eligible.Count == 0) return result;

            List<long> billIds = eligible
                .Where(x => x != null && x.BILL_ID != null)
                .Select(x => x.BILL_ID.Value)
                .Distinct()
                .ToList();

            if (billIds.Count == 0) return result;

            int step = 0;
            while (billIds.Count - step > 0)
            {
                List<long> ids = billIds.Skip(step).Take(MaxReq).ToList();
                step += MaxReq;

                HisTransactionFilter ft = new HisTransactionFilter();
                ft.IDs = ids;

                List<HIS_TRANSACTION> data = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>(
                    "api/HisTransaction/Get", ApiConsumers.MosConsumer, ft, null);

                if (data != null && data.Count > 0) result.AddRange(data);
            }

            result = result
                .Where(t => t != null
                    && t.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT
                    && (t.IS_CANCEL == null || t.IS_CANCEL != 1)
                    && (t.IS_DELETE == null || t.IS_DELETE != 1)
                    && !String.IsNullOrWhiteSpace(t.INVOICE_CODE)
                    && !String.IsNullOrWhiteSpace(t.INVOICE_SYS))
                .GroupBy(t => t.ID)
                .Select(g => g.First())
                .ToList();

            return result;
        }

        private List<HIS_SERVICE_REQ_METY> GetServiceReqMetyByServiceReqIds(List<long> serviceReqIds)
        {
            List<HIS_SERVICE_REQ_METY> result = new List<HIS_SERVICE_REQ_METY>();
            if (serviceReqIds == null || serviceReqIds.Count == 0) return result;

            int step = 0;
            while (serviceReqIds.Count - step > 0)
            {
                List<long> ids = serviceReqIds.Skip(step).Take(MaxReq).ToList();
                step += MaxReq;

                HisServiceReqMetyFilter ft = new HisServiceReqMetyFilter();
                ft.SERVICE_REQ_IDs = ids;

                List<HIS_SERVICE_REQ_METY> data = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ_METY>>(
                    "api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, ft, null);

                if (data != null && data.Count > 0) result.AddRange(data);
            }

            return result;
        }
        private string BuildXxxFromExpMest(List<HIS_EXP_MEST> expMests, List<string> selectedServiceReqCodes)
        {
            if (expMests == null || expMests.Count == 0)
            {
                if (selectedServiceReqCodes == null) return "";
                return String.Join(", ", selectedServiceReqCodes.Where(s => !String.IsNullOrWhiteSpace(s)).Distinct().ToList());
            }

            List<string> codes = expMests
                .Where(x => x != null && !String.IsNullOrWhiteSpace(x.TDL_SERVICE_REQ_CODE))
                .Select(x => x.TDL_SERVICE_REQ_CODE.Trim())
                .Distinct()
                .ToList();

            if (codes.Count == 0 && selectedServiceReqCodes != null)
                codes = selectedServiceReqCodes.Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList();

            return String.Join(", ", codes);
        }
        private List<HIS_SERVICE_REQ> GetServiceReqByIds(List<long> ids)
        {
            List<HIS_SERVICE_REQ> result = new List<HIS_SERVICE_REQ>();
            if (ids == null || ids.Count == 0) return result;

            int step = 0;
            while (ids.Count - step > 0)
            {
                List<long> batch = ids.Skip(step).Take(MaxReq).ToList();
                step += MaxReq;

                HisServiceReqFilter ft = new HisServiceReqFilter();
                ft.IDs = batch;

                List<HIS_SERVICE_REQ> data = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/Get", ApiConsumers.MosConsumer, ft, null);

                if (data != null && data.Count > 0) result.AddRange(data);
            }

            // loại trùng theo ID
            result = result.Where(x => x != null)
                           .GroupBy(x => x.ID)
                           .Select(g => g.First())
                           .ToList();

            return result;
        }

        private void btnUpdateSale_Click(object sender, EventArgs e)
        {
            try
            {
                // =======================
                // 1) Validate config
                // =======================
                string msgSys = ValidateSysConfig(SysConfigValue);
                if (!String.IsNullOrEmpty(msgSys))
                {
                    XtraMessageBox.Show(msgSys, ResourceLanguageManager.ThongBao);
                    return;
                }

                string msgApp = ValidateClientAppConfig(ClientAppConfigValue);
                if (!String.IsNullOrEmpty(msgApp))
                {
                    XtraMessageBox.Show(msgApp, ResourceLanguageManager.ThongBao);
                    return;
                }

                int[] rowHandles = gridView1.GetSelectedRows();
                if (rowHandles == null || rowHandles.Length == 0) return;

                // =======================
                // 2) Lấy y lệnh từ grid
                // =======================
                List<HIS_SERVICE_REQ> selectedReqs = new List<HIS_SERVICE_REQ>();
                List<long> selectedReqIds = new List<long>();
                List<string> selectedReqCodes = new List<string>();

                for (int i = 0; i < rowHandles.Length; i++)
                {
                    ServiceReqADO ado = gridView1.GetRow(rowHandles[i]) as ServiceReqADO;
                    if (ado == null) continue;
                    if (String.IsNullOrWhiteSpace(ado.SERVICE_REQ_CODE)) continue;

                    string code = ado.SERVICE_REQ_CODE.Trim();

                    HIS_SERVICE_REQ sr = new HIS_SERVICE_REQ();
                    sr.ID = ado.ID;
                    sr.SERVICE_REQ_CODE = code;
                    sr.REQUEST_LOGINNAME = ado.REQUEST_LOGINNAME;

                    selectedReqs.Add(sr);
                    selectedReqIds.Add(sr.ID);
                    selectedReqCodes.Add(code);
                }

                selectedReqIds = selectedReqIds.Distinct().ToList();
                selectedReqCodes = selectedReqCodes.Distinct().ToList();
                if (selectedReqCodes.Count == 0) return;

                WaitingManager.Show();

                // =======================
                // 3) Lấy service_req đầy đủ (để check IS_SENT_ERX)
                // =======================
                List<HIS_SERVICE_REQ> fullReqs = GetServiceReqByIds(selectedReqIds);
                if (fullReqs == null) fullReqs = new List<HIS_SERVICE_REQ>();

                Dictionary<string, HIS_SERVICE_REQ> dicReqByCode =
                    new Dictionary<string, HIS_SERVICE_REQ>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < fullReqs.Count; i++)
                {
                    HIS_SERVICE_REQ r = fullReqs[i];
                    if (r == null) continue;
                    if (String.IsNullOrWhiteSpace(r.SERVICE_REQ_CODE)) continue;

                    string c = r.SERVICE_REQ_CODE.Trim();
                    if (!dicReqByCode.ContainsKey(c))
                        dicReqByCode[c] = r;
                }

                // =======================
                // 4) Lấy phiếu xuất theo TDL_SERVICE_REQ_CODE => Danh sách A
                // =======================
                List<HIS_EXP_MEST> listA = GetExpMestByTdlServiceReqCodes(selectedReqCodes);
                if (listA == null) listA = new List<HIS_EXP_MEST>();

                if (listA.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không tìm thấy phiếu xuất theo y lệnh đã chọn.", ResourceLanguageManager.ThongBao);
                    return;
                }

                // =======================
                // 5) Tạo danh sách B/C/D/E theo thiết kế mới
                // =======================
                long ID_DONE = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                long ID_BAN = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN; // = 8

                // B: DONE + BAN + BILL_ID != null + IS_SENT_SOLD_QTY_ERX != 1
                List<HIS_EXP_MEST> listB = listA.Where(x =>
                    x != null
                    && x.EXP_MEST_STT_ID == ID_DONE
                    && x.EXP_MEST_TYPE_ID == ID_BAN
                    && x.BILL_ID != null
                    && x.IS_SENT_SOLD_QTY_ERX != 1
                ).ToList();

                // C: DONE + BAN + BILL_ID != null + IS_SENT_SOLD_QTY_ERX == 1
                List<HIS_EXP_MEST> listC = listA.Where(x =>
                    x != null
                    && x.EXP_MEST_STT_ID == ID_DONE
                    && x.EXP_MEST_TYPE_ID == ID_BAN
                    && x.BILL_ID != null
                    && x.IS_SENT_SOLD_QTY_ERX == 1
                ).ToList();

                // D: OR
                // 1) EXP_MEST_TYPE_ID != 8
                // 2) EXP_MEST_TYPE_ID = 8 & BILL_ID null & IS_SENT_SOLD_QTY_ERX != 1
                // 3) SERVICE_REQ (code = TDL_SERVICE_REQ_CODE) có IS_SENT_ERX != 1
                List<HIS_EXP_MEST> listD = new List<HIS_EXP_MEST>();
                for (int i = 0; i < listA.Count; i++)
                {
                    HIS_EXP_MEST em = listA[i];
                    if (em == null) continue;

                    bool isD = false;

                    if (em.EXP_MEST_TYPE_ID != ID_BAN)
                    {
                        isD = true;
                    }
                    else
                    {
                        if (em.BILL_ID == null && em.IS_SENT_SOLD_QTY_ERX != 1)
                            isD = true;

                        string reqCode = (em.TDL_SERVICE_REQ_CODE ?? "").Trim();
                        if (!String.IsNullOrWhiteSpace(reqCode))
                        {
                            HIS_SERVICE_REQ r = null;
                            if (dicReqByCode.TryGetValue(reqCode, out r))
                            {
                                if (r == null || r.IS_SENT_ERX != 1) isD = true;
                            }
                            else
                            {
                                isD = true;
                            }
                        }
                        else
                        {
                            isD = true;
                        }
                    }

                    if (isD) listD.Add(em);
                }

                // E: CHƯA DONE + BAN + BILL_ID != null + IS_SENT_SOLD_QTY_ERX != 1
                List<HIS_EXP_MEST> listE = listA.Where(x =>
                    x != null
                    && x.EXP_MEST_STT_ID != ID_DONE
                    && x.EXP_MEST_TYPE_ID == ID_BAN
                    && x.BILL_ID != null
                    && x.IS_SENT_SOLD_QTY_ERX != 1
                ).ToList();

                // =======================
                // 5.1) Ưu tiên D: set mã y lệnh thuộc D
                // =======================
                HashSet<string> dReqCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < listD.Count; i++)
                {
                    string c = (listD[i].TDL_SERVICE_REQ_CODE ?? "").Trim();
                    if (!String.IsNullOrWhiteSpace(c)) dReqCodes.Add(c);
                }

                // =======================
                // ƯU TIÊN ĐẶC BIỆT: nếu A chỉ có 1 phần tử -> ưu tiên show D và return
                // =======================
                if (listA.Count == 1)
                {
                    if (listD.Count > 0)
                    {
                        string xxxD = BuildXxxFromExpMest(listD, selectedReqCodes);
                        if (!String.IsNullOrWhiteSpace(xxxD))
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show(
                                "Đơn thuốc " + xxxD +
                                " không phải phiếu xuất bán hoặc là phiếu xuất bán nhưng chưa được xuất hóa đơn điện tử " +
                                "hoặc chưa đẩy đơn thuốc sang hệ thống liên thông đơn thuốc không cho phép cập nhật số lượng bán",
                                ResourceLanguageManager.ThongBao);
                            return;
                        }
                    }
                }

                // =======================
                // 6) Build thông báo (ghép 1 popup)
                // E -> C -> D (C loại trùng D)
                // =======================
                List<string> lines = new List<string>();

                // E: chỉ CẢNH BÁO (KHÔNG return)
                if (listE.Count > 0)
                {
                    List<string> parts = new List<string>();
                    for (int i = 0; i < listE.Count; i++)
                    {
                        string expCode = (listE[i].EXP_MEST_CODE ?? "").Trim();
                        string reqCode = (listE[i].TDL_SERVICE_REQ_CODE ?? "").Trim();
                        parts.Add(expCode + " (Mã y lệnh: " + reqCode + ")");
                    }
                    lines.Add("Các mã phiếu xuất sau chưa hoàn thành: " + String.Join(", ", parts) + ". Không cho phép cập nhật số lượng bán");
                }

                // C: loại trùng D
                List<HIS_EXP_MEST> listCOnly = listC
                    .Where(x => x != null && !dReqCodes.Contains((x.TDL_SERVICE_REQ_CODE ?? "").Trim()))
                    .ToList();

                if (listCOnly.Count > 0)
                {
                    string xxxC = BuildXxxFromExpMest(listCOnly, selectedReqCodes);
                    if (!String.IsNullOrWhiteSpace(xxxC))
                        lines.Add("Các y lệnh " + xxxC + " đã được cập nhật số lượng bán sang hệ thống liên thông đơn thuốc.");
                }

                // D
                if (listD.Count > 0)
                {
                    string xxxD2 = BuildXxxFromExpMest(listD, selectedReqCodes);
                    if (!String.IsNullOrWhiteSpace(xxxD2))
                        lines.Add("Đơn thuốc " + xxxD2 + " không phải phiếu xuất bán hoặc là phiếu xuất bán nhưng chưa được xuất hóa đơn điện tử hoặc chưa đẩy đơn thuốc sang hệ thống liên thông đơn thuốc không cho phép cập nhật số lượng bán");
                }

                // Nếu không có B -> không có gì để đẩy: chỉ show message rồi return
                if (listB.Count == 0)
                {
                    WaitingManager.Hide();
                    if (lines.Count > 0)
                        XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                // =======================
                // 7) Nếu tồn tại B thì xử lý
                // F: transaction theo BILL_ID của B
                // =======================
                List<HIS_TRANSACTION> listF = GetTransactionsAByEligibleExpMest(listB);
                if (listF == null) listF = new List<HIS_TRANSACTION>();

                if (listF.Count == 0)
                {
                    WaitingManager.Hide();
                    lines.Add("Không tìm thấy hóa đơn điện tử hợp lệ (transaction) để cập nhật số lượng bán.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                HashSet<long> tranIds = new HashSet<long>(listF.Select(t => t.ID));
                List<HIS_EXP_MEST> listB2 = listB
                    .Where(x => x != null && x.BILL_ID != null && tranIds.Contains(x.BILL_ID.Value))
                    .ToList();

                if (listB2.Count == 0)
                {
                    WaitingManager.Hide();
                    lines.Add("Không có phiếu xuất nào có hóa đơn điện tử hợp lệ để cập nhật số lượng bán.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                // =======================
                // 8) G: mety theo SERVICE_REQ_ID của serviceReq tương ứng với B2
                // =======================
                List<string> bReqCodes = listB2
                    .Where(x => x != null && !String.IsNullOrWhiteSpace(x.TDL_SERVICE_REQ_CODE))
                    .Select(x => x.TDL_SERVICE_REQ_CODE.Trim())
                    .Distinct()
                    .ToList();

                List<HIS_SERVICE_REQ> listServiceReq = fullReqs
                    .Where(r => r != null
                                && !String.IsNullOrWhiteSpace(r.SERVICE_REQ_CODE)
                                && bReqCodes.Contains(r.SERVICE_REQ_CODE.Trim()))
                    .ToList();

                if (listServiceReq.Count == 0)
                {
                    WaitingManager.Hide();
                    lines.Add("Không xác định được y lệnh tương ứng với phiếu xuất để cập nhật số lượng bán.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                List<long> reqIds = listServiceReq.Select(r => r.ID).Distinct().ToList();
                List<HIS_SERVICE_REQ_METY> listG = GetServiceReqMetyByServiceReqIds(reqIds);
                if (listG == null) listG = new List<HIS_SERVICE_REQ_METY>();

                if (listG.Count == 0)
                {
                    WaitingManager.Hide();
                    lines.Add("Không có chi tiết thuốc (HIS_SERVICE_REQ_METY) để cập nhật số lượng bán.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                // =======================
                // 9) Build input & call lib (DataInput giữ nguyên)
                // =======================
                DataInput input = BuildSendQuantitySoldInput(
                    SysConfigValue,
                    ClientAppConfigValue,
                    CurrBranch,
                    listB2,
                    listF,
                    listServiceReq,
                    listG
                );
                try
                {
                    LogSystem.Info("=== InterconnectionPrescription.btnUpdateSale - CONFIG CHECK ===");
                    LogSystem.Info("SysConfigValue: " + SysConfigValue);
                    LogSystem.Info("ClientAppConfigValue: " + ClientAppConfigValue);
                    LogSystem.Info("DataInput.Url (SysConfig.Url): " + input.Url);
                    LogSystem.Info("DataInput.UrlQuantitySold (ClientAppConfig.Url): " + input.UrlQuantitySold);
                    LogSystem.Info("DataInput.AppName: " + input.AppName + " | AppKey: " + input.AppKey);
                    LogSystem.Info("DataInput.MaCoSoCungUng: " + input.MaCoSoCungUng);

                    LogSystem.Info(
                        "SendQuantitySold DataInput: " +
                        Inventec.Common.Logging.LogUtil.TraceData("DataInput", input)
                    );

                    LogSystem.Info("SendQuantitySold DataInput.ListExpMest: " +
                                   Inventec.Common.Logging.LogUtil.TraceData("ListExpMest", input.ListExpMest));
                    LogSystem.Info("SendQuantitySold DataInput.ListTransaction: " +
                                   Inventec.Common.Logging.LogUtil.TraceData("ListTransaction", input.ListTransaction));
                    LogSystem.Info("SendQuantitySold DataInput.ListServiceReq: " +
                                   Inventec.Common.Logging.LogUtil.TraceData("ListServiceReq", input.ListServiceReq));
                    LogSystem.Info("SendQuantitySold DataInput.ListReqMety: " +
                                   Inventec.Common.Logging.LogUtil.TraceData("ListReqMety", input.ListReqMety));
                }
                catch (Exception exLogInput)
                {
                    LogSystem.Error("Error when logging DataInput before SendQuantitySold: " + exLogInput);
                }

                DataResult rs = null;
                try
                {
                    rs = new ERXConnectProcessor().SendQuantitySold(input);
                }
                catch (Exception exCall)
                {
                    WaitingManager.Hide();
                    LogSystem.Error(exCall);
                    lines.Add("Cập nhật số lượng bán không thành công. Vui lòng kiểm tra cấu hình hoặc kết nối tới hệ thống liên thông.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                if (rs == null || rs.Datas == null || rs.Datas.Count == 0)
                {
                    WaitingManager.Hide();
                    lines.Add("Cập nhật số lượng bán không thành công. Không nhận được phản hồi từ hệ thống liên thông.");
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                    return;
                }

                // =======================
                // 10) Update DB theo ExpMestCode
                // =======================
                Dictionary<string, HIS_EXP_MEST> dicExpByCode =
                    new Dictionary<string, HIS_EXP_MEST>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < listB2.Count; i++)
                {
                    HIS_EXP_MEST em = listB2[i];
                    if (em == null) continue;
                    if (String.IsNullOrWhiteSpace(em.EXP_MEST_CODE)) continue;

                    string c = em.EXP_MEST_CODE.Trim();
                    if (!dicExpByCode.ContainsKey(c))
                        dicExpByCode[c] = em;
                }

                List<HIS_EXP_MEST> updates = new List<HIS_EXP_MEST>();
                List<string> okReqCodes = new List<string>();
                List<string> failReqCodes = new List<string>();

                for (int i = 0; i < rs.Datas.Count; i++)
                {
                    PrescriptionResult pr = rs.Datas[i];
                    if (pr == null) continue;

                    string expMestCode = (pr.ExpMestCode ?? "").Trim();
                    string reqCode = (pr.ServiceReqCode ?? "").Trim();

                    if (!String.IsNullOrWhiteSpace(reqCode))
                    {
                        if (pr.Success) okReqCodes.Add(reqCode);
                        else failReqCodes.Add(reqCode);
                    }

                    if (String.IsNullOrWhiteSpace(expMestCode)) continue;

                    HIS_EXP_MEST found = null;
                    if (!dicExpByCode.TryGetValue(expMestCode, out found)) continue;
                    if (found == null) continue;

                    HIS_EXP_MEST up = new HIS_EXP_MEST();
                    up.ID = found.ID;
                    up.IS_SENT_SOLD_QTY_ERX = pr.Success ? (short)1 : (short)2;
                    updates.Add(up);
                }

                updates = updates.GroupBy(x => x.ID).Select(g => g.Last()).ToList();

                if (updates.Count > 0)
                {
                    CommonParam p = new CommonParam();
                    int step = 0;
                    while (updates.Count - step > 0)
                    {
                        List<HIS_EXP_MEST> batch = updates.Skip(step).Take(MaxReq).ToList();
                        step += MaxReq;

                        LogSystem.Warn("Call UpdateSentErx input: " + Inventec.Common.Logging.LogUtil.TraceData("batch", batch));

                        new BackendAdapter(p).Post<List<HIS_EXP_MEST>>(
                            "api/HisExpMest/UpdateSentErx",
                            ApiConsumers.MosConsumer,
                            batch,
                            p);
                    }
                }

                if (rs.Messages != null && rs.Messages.Count > 0)
                {
                    LogSystem.Error("SendQuantitySold Messages: " + String.Join(" | ", rs.Messages.Distinct().ToList()));
                }

                WaitingManager.Hide();

                // =======================
                // 11) Thông báo kết quả (ghép chung 1 popup)
                // - Ưu tiên D: loại bỏ code thuộc D khỏi ok/fail
                // =======================
                okReqCodes = okReqCodes
                    .Where(x => !String.IsNullOrWhiteSpace(x) && !dReqCodes.Contains(x.Trim()))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                failReqCodes = failReqCodes
                    .Where(x => !String.IsNullOrWhiteSpace(x) && !dReqCodes.Contains(x.Trim()))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (okReqCodes.Count > 0)
                    lines.Add("Cập nhật số lượng bán thành công cho đơn: " + String.Join(", ", okReqCodes));

                if (failReqCodes.Count > 0)
                {
                    string errDetail = "";
                    if (rs.Messages != null && rs.Messages.Count > 0)
                        errDetail = " (" + String.Join(" | ", rs.Messages.Distinct().ToList()) + ")";

                    lines.Add("Cập nhật số lượng bán KHÔNG thành công cho đơn: " + String.Join(", ", failReqCodes) + errDetail);
                }

                if (lines.Count > 0)
                {
                    XtraMessageBox.Show(String.Join(Environment.NewLine, lines), ResourceLanguageManager.ThongBao);
                }

                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra khi cập nhật số lượng bán.", ResourceLanguageManager.ThongBao);
            }
        }


    }
}
