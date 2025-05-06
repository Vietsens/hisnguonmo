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
using ACS.EFMODEL.DataModels;
using Bartender.PrintClient;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.PrintServiceReq;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.Desktop.Plugins.SampleInfo.Config;
using LIS.Desktop.Plugins.SampleInfo.Validation;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.WebApiClient;

namespace LIS.Desktop.Plugins.SampleInfo
{
    public partial class frmSampleInfo : FormBase
    {
        private int positionHandleControl = -1;
        private string serviceReqCode = null;
        private V_LIS_SAMPLE sample = null;
        private V_HIS_ROOM room = null;

        private List<LIS_SAMPLE_TYPE> sampleTypes = null;
        private List<LIS_PATIENT_CONDITION> listPatientCondition;
        private List<HIS_MEDI_ORG> listMediOrg = new List<HIS_MEDI_ORG>();
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        bool isNotLoadWhileChkIsInDebtStateInFirst;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "LIS.Desktop.Plugins.SampleInfo";
        List<HIS_SERVICE> lstHisService { get; set; }
        HIS_SERVICE_REQ serviceReq { get; set; }
        List<HIS_SERVICE_REQ> ListServiceReq { get; set; }
        List<string> serviceNameMess = new List<string>();
        private List<V_LIS_SAMPLE> sampleList;

        bool IsFirstLoadForm { get; set; }
        bool IsSaveAndPrint;
        public frmSampleInfo(Inventec.Desktop.Common.Modules.Module module, List<V_LIS_SAMPLE> data)
           : base(module)
        {
            try
            {
                InitializeComponent();
                this.sampleList = data;
                if (data != null && data.Count > 0)
                    sample = data.FirstOrDefault();
                lciBarcode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.Size = new Size(this.Width, this.Height - lciBarcode.Height);
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem1.Visibility = layoutControlItem2.Visibility = layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmSampleInfo(Inventec.Desktop.Common.Modules.Module module, V_LIS_SAMPLE data)
            : base(module)
        {
            InitializeComponent();
            this.sample = data;
            LisConfigCFG.LoadConfig();
        }

        public frmSampleInfo(Inventec.Desktop.Common.Modules.Module module, string reqCode)
            : base(module)
        {
            InitializeComponent();
            this.serviceReqCode = reqCode;
            LisConfigCFG.LoadConfig();
        }

        private void frmSampleInfo_Load(object sender, EventArgs e)
        {
            try
            {
                IsFirstLoadForm = true;
                WaitingManager.Show();
                this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
                GetServiceFromLocal();
                this.ValidControl();
                this.LoadSample();
                this.LoadServiceReq();
                this.InitComboUser();
                this.InitComboSampleType();
                this.InitComboPatientStatus();
                this.InitComboMediOrgCode();
                this.SetControlValue();
                InitControlState();
                this.InitDateTime();
                timer1.Start();
                IsFirstLoadForm = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDateTime()
        {
            try
            {
                List<HIS_SERVICE> serviceDataList = null;
                if (ListServiceReq != null && ListServiceReq.Count > 0)
                {
                    serviceDataList = new List<HIS_SERVICE>();
                    foreach (var sr in ListServiceReq)
                    {
                        var longServiceId = sr.TDL_SERVICE_IDS;
                        if (sampleList != null && sampleList.Count > 0)
                        {
                            var sample = sampleList.FirstOrDefault(o => o.SERVICE_REQ_CODE == sr.SERVICE_REQ_CODE);
                            if (sample != null)
                            {
                                if (longServiceId.Contains(";"))
                                {
                                    var arrList = longServiceId.Split(';').ToList();
                                    serviceDataList.AddRange(lstHisService.Where(o => arrList.Exists(p => p == o.ID.ToString())).ToList());
                                }
                                else
                                {
                                    serviceDataList.AddRange(lstHisService.Where(o => o.ID.ToString() == longServiceId).ToList());
                                }
                            }
                        }
                    }
                    serviceDataList = serviceDataList.Distinct().ToList();
                }
                else if (serviceReq != null && serviceReq.ID > 0)
                {
                    var longServiceId = serviceReq.TDL_SERVICE_IDS;
                    if (!String.IsNullOrEmpty(longServiceId))
                    {
                        if (longServiceId.Contains(";"))
                        {
                            var arrList = longServiceId.Split(';').ToList();
                            serviceDataList = lstHisService.Where(o => arrList.Exists(p => p == o.ID.ToString())).ToList();
                        }
                        else
                        {
                            serviceDataList = lstHisService.Where(o => o.ID.ToString() == longServiceId).ToList();
                        }
                    }
                }
                if (serviceDataList != null && serviceDataList.Count > 0)
                {
                    decimal ESTIMATE_DURATION = serviceDataList.Max(o => o.ESTIMATE_DURATION ?? 0);
                    if (ESTIMATE_DURATION > 0)
                    {
                        DateTime? sampleTime = dtSampleTime.DateTime;
                        TimeSpan estimateDuration = new TimeSpan(0, (int)ESTIMATE_DURATION, 0);
                        DateTime appointmentTime = sampleTime.Value.Add(estimateDuration);

                        dtAppointmentTime.DateTime = appointmentTime;
                    }
                }
                else
                {
                    dtAppointmentTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceFromLocal()
        {
            try
            {
                lstHisService = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.IS_ACTIVE == (short?)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboMediOrgCode()
        {
            try
            {
                HisMediOrgFilter filter = new HisMediOrgFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                this.listMediOrg = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_ORG>>("api/HisMediOrg/Get", ApiConsumers.MosConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_ORG_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MEDI_ORG_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_ORG_NAME", "MEDI_ORG_CODE", columnInfos, false, 400);
                ControlEditorLoader.Load(cboMediOrgCode, this.listMediOrg, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChkIsInDebtStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPreviousStatus.Name)
                        {
                            chkPreviousStatus.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == cboPatientStatus.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                cboPatientStatus.EditValue = item.VALUE;
                            }
                        }
                        else if (item.KEY == chkCloseAfterPrint.Name)
                        {
                            chkCloseAfterPrint.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChkIsInDebtStateInFirst = false;
        }

        private void LoadSample()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(serviceReqCode))
                {
                    LisSampleViewFilter filter = new LisSampleViewFilter();
                    filter.SERVICE_REQ_CODE__EXACT = serviceReqCode;

                    List<V_LIS_SAMPLE> samples = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, null);
                    this.sample = samples != null ? samples.FirstOrDefault() : null;
                }

                if (this.sample == null)
                {
                    XtraMessageBox.Show("Không tìm thấy bệnh phẩm theo mã yêu cầu: " + this.serviceReqCode, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    this.Close();
                }

                if (!(this.sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || this.sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                    || this.sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM))
                {
                    btnSave.Enabled = false;
                    btnSaveAndPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitComboUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("InitComboUser. 1");
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboSampleUser, datas, controlEditorADO);


                ACS_USER data = null;
                if (!String.IsNullOrWhiteSpace(this.sample.SAMPLE_LOGINNAME))
                {
                    data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(this.sample.SAMPLE_LOGINNAME.ToUpper())).FirstOrDefault();
                }
                else
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();
                }
                if (data != null)
                {
                    this.cboSampleUser.EditValue = data.LOGINNAME;
                    this.txtSampleLoginname.Text = data.LOGINNAME;
                }
                Inventec.Common.Logging.LogSystem.Info("InitComboUser. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSampleType()
        {
            try
            {
                LisSampleTypeFilter filter = new LisSampleTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.sampleTypes = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE_TYPE>>("api/LisSampleType/Get", ApiConsumers.LisConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_TYPE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboSampleType, this.sampleTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientStatus()
        {
            try
            {
                LisPatientConditionFilter filter = new LisPatientConditionFilter();
                listPatientCondition = new BackendAdapter(new CommonParam()).Get<List<LIS_PATIENT_CONDITION>>("api/LisPatientCondition/Get", ApiConsumers.LisConsumer, filter, null);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CONDITION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CONDITION_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CONDITION_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPatientStatus, listPatientCondition, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetControlValue()
        {
            try
            {
                lblGenderName.Text = this.sample.GENDER_CODE == "01" ? "Nữ" : "Nam";
                lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.sample.DOB ?? 0);
                lblPatientName.Text = (this.sample.LAST_NAME ?? "") + " " + (this.sample.FIRST_NAME ?? "");
                if (sampleList != null && sampleList.Count > 0)
                {
                    lblServiceReqCode.Text = string.Join(", ", this.sampleList.Select(o => o.SERVICE_REQ_CODE));
                }
                else
                    lblServiceReqCode.Text = this.sample.SERVICE_REQ_CODE;
                lblTreatmentCode.Text = this.sample.TREATMENT_CODE ?? "";
                if (lciBarcode.Visible)
                {
                    txtBarcode.Text = this.sample.BARCODE ?? "";
                    txtBarcode.ReadOnly = LisConfigCFG.IS_AUTO_CREATE_BARCODE == "1";
                }
                dtSampleTime.DateTime = DateTime.Now;
                MessageWarning();
                if (sampleList == null || sampleList.Count == 0)
                {
                    btnPrint.Enabled = !String.IsNullOrWhiteSpace(this.sample.BARCODE);
                    btnPrintAssign.Enabled = false;
                }
                else
                {
                    btnPrint.Enabled = false;
                    btnPrintAssign.Enabled = false;
                    btnSaveAndPrint.Enabled = false;
                }
                if (sampleList == null || (sampleList != null && sampleList.Count > 0 && sampleList.Where(o => o.SAMPLE_TYPE_ID != null).Select(o => o.SAMPLE_TYPE_ID).Distinct().Count() == 1))
                    cboSampleType.EditValue = this.sample.SAMPLE_TYPE_ID;
                if (sampleList == null || (sampleList != null && sampleList.Count > 0 && sampleList.Where(o => !String.IsNullOrWhiteSpace(o.SAMPLE_SENDER_CODE)).Select(o => o.SAMPLE_SENDER_CODE).Distinct().Count() == 1))
                {
                    txtMediOrgCode.Text = this.sample.SAMPLE_SENDER_CODE ?? "";
                    cboMediOrgCode.EditValue = this.sample.SAMPLE_SENDER_CODE;
                }
                if (cboMediOrgCode.EditValue != null)
                {
                    HIS_MEDI_ORG org = this.listMediOrg != null ? this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                    cboMediOrgCode.ToolTip = org.MEDI_ORG_NAME ?? "";
                }
                if (string.IsNullOrEmpty(txtMediOrgCode.Text.Trim()))
                {
                    if (sampleList == null || (sampleList != null && sampleList.Count > 0 && sampleList.Where(o => !String.IsNullOrWhiteSpace(o.SAMPLE_SENDER)).Select(o => o.SAMPLE_SENDER).Distinct().Count() == 1))
                        txtSampleSender.Text = this.sample.SAMPLE_SENDER ?? "";
                }
                string place = "";
                if (sampleList == null || (sampleList != null && sampleList.Count > 0 && sampleList.Where(o => !String.IsNullOrWhiteSpace(o.REQUEST_ROOM_NAME)).Select(o => o.REQUEST_ROOM_NAME).Distinct().Count() == 1))
                {
                    place = sample.REQUEST_ROOM_NAME;
                }
                if (sampleList == null || (sampleList != null && sampleList.Count > 0 && sampleList.Where(o => !String.IsNullOrWhiteSpace(o.REQUEST_DEPARTMENT_NAME)).Select(o => o.REQUEST_DEPARTMENT_NAME).Distinct().Count() == 1))
                {
                    place += " - " + sample.REQUEST_DEPARTMENT_NAME;
                }
                txtAppointmentPlace.Text = place;
                List<long> serviceIds = null;
                //xử lý tự động chọn loại mẫu bệnh phẩm
                if (this.ListServiceReq != null && this.ListServiceReq.Count > 0)
                {
                    serviceIds = GetListIds(string.Join(";", this.ListServiceReq.Where(o => !string.IsNullOrEmpty(o.TDL_SERVICE_IDS)).Select(o => o.TDL_SERVICE_IDS))) ?? new List<long>();
                }
                else if (this.serviceReq != null && !String.IsNullOrWhiteSpace(this.serviceReq.TDL_SERVICE_IDS))
                {
                    serviceIds = GetListIds(this.serviceReq.TDL_SERVICE_IDS) ?? new List<long>();
                }
                if (serviceIds != null && serviceIds.Count > 0)
                {
                    serviceIds = serviceIds.Distinct().ToList();
                    List<V_HIS_SERVICE> listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList() ?? new List<V_HIS_SERVICE>();
                    List<string> listSampleTypeCode = listService.Select(s => s.SAMPLE_TYPE_CODE ?? "").ToList() ?? new List<string>();
                    List<LIS_SAMPLE_TYPE> listSampleType = this.sampleTypes.Where(o => !String.IsNullOrWhiteSpace(o.SAMPLE_TYPE_CODE)
                                                                                    && listSampleTypeCode.Contains(o.SAMPLE_TYPE_CODE)).ToList();
                    if (listSampleType != null
                        && listSampleType.Count == 1
                        && ((sampleList == null && !this.sample.SAMPLE_TYPE_ID.HasValue) || (sampleList.Where(o => o.SAMPLE_TYPE_ID.HasValue).Count() == 0)))
                    {
                        cboSampleType.EditValue = listSampleType[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<long> GetListIds(string str)
        {
            List<long> result = new List<long>();
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                    return result;
                var list = str.Split(';');
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        long id = 0;
                        if (Int64.TryParse(item, out id))
                            result.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ValidControl()
        {
            try
            {
                ValidSampleType();
                ValidSampleUser();
                ValidSampleTime();
                if (lciBarcode.Visible)
                {
                    ValidBarcode();
                }
                ValidSampleSender();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSampleSender()
        {
            try
            {
                SampleSenderValidationRule rule = new SampleSenderValidationRule();
                rule.textEdit = txtSampleSender;
                rule.maxlength = 400;
                rule.isRequired = false;
                dxValidationProvider1.SetValidationRule(txtSampleSender, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSampleUser()
        {
            try
            {
                SampleUserValidationRule rule = new SampleUserValidationRule();
                rule.txtSampleLoginname = txtSampleLoginname;
                rule.cboSampleUser = cboSampleUser;
                dxValidationProvider1.SetValidationRule(txtSampleLoginname, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSampleType()
        {
            try
            {
                SampleTypeValidationRule rule = new SampleTypeValidationRule();
                rule.txtSampleTypeCode = txtSampleTypeCode;
                rule.cboSampleType = cboSampleType;
                dxValidationProvider1.SetValidationRule(txtSampleTypeCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSampleTime()
        {
            try
            {
                SampleTimeValidationRule rule = new SampleTimeValidationRule();
                rule.intructionTime = this.sample.INTRUCTION_TIME ?? 0; ;
                rule.dtSampleTime = dtSampleTime;
                dxValidationProvider1.SetValidationRule(dtSampleTime, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBarcode()
        {
            try
            {
                BarcodeValidationRule rule = new BarcodeValidationRule();
                rule.txtBarcode = txtBarcode;
                dxValidationProvider1.SetValidationRule(txtBarcode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSampleTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtSampleTypeCode.Text))
                    {
                        string txt = txtSampleTypeCode.Text.ToLower().Trim();
                        var listData = this.sampleTypes != null ? this.sampleTypes.Where(o => o.SAMPLE_TYPE_CODE.ToLower().Contains(txt)).ToList() : null;
                        if (listData != null && listData.Count == 1)
                        {
                            cboSampleType.EditValue = listData[0].ID;
                            txtSampleLoginname.Focus();
                        }
                        else
                        {
                            cboSampleType.Focus();
                            cboSampleType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboSampleType.Focus();
                        cboSampleType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtSampleLoginname.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSampleTypeCode.Text = "";
                txtTube.Text = null;
                if (cboSampleType.EditValue != null)
                {
                    var samType = this.sampleTypes.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampleType.EditValue));
                    if (samType != null)
                    {
                        txtSampleTypeCode.Text = samType.SAMPLE_TYPE_CODE;
                        if (!String.IsNullOrEmpty(samType.TUBE_TYPE_NAME))
                        {
                            txtTube.Text = samType.TUBE_TYPE_NAME + ": " + samType.AMOUNT;
                        }
                        else
                        {
                            txtTube.Text = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSampleLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboSampleUser.EditValue = null;
                        cboSampleUser.Focus();
                        cboSampleUser.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboSampleUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtSampleLoginname.Text = searchResult[0].LOGINNAME;
                            this.dtSampleTime.Focus();
                        }
                        else
                        {
                            cboSampleUser.EditValue = null;
                            cboSampleUser.Focus();
                            cboSampleUser.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtSampleTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSampleLoginname.Text = "";
                if (this.cboSampleUser.EditValue != null)
                {
                    ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboSampleUser.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        this.txtSampleLoginname.Text = data.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSampleTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                if (sampleList != null && sampleList.Count > 0)
                {
                    filter.SERVICE_REQ_CODES = sampleList.Select(o => o.SERVICE_REQ_CODE).ToList();
                    ListServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                }
                else if (sample.SERVICE_REQ_CODE != null)
                {
                    filter.SERVICE_REQ_CODE__EXACT = this.sample.SERVICE_REQ_CODE;
                    serviceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ShowMessage()
        {
            try
            {
                if (serviceNameMess != null && serviceNameMess.Count > 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian lấy mẫu của dịch vụ " + String.Join("; ", serviceNameMess) + " lớn hơn thời gian quy định.", "Cảnh báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MessageWarning()
        {
            try
            {
                if (dtSampleTime.EditValue == null || dtSampleTime.DateTime == DateTime.MinValue)
                    return;
                List<HIS_SERVICE> serviceDataList = null;
                var dic = new Dictionary<V_LIS_SAMPLE, string>(new ComparerObject());
                if (ListServiceReq != null && ListServiceReq.Count > 0)
                {
                    serviceDataList = new List<HIS_SERVICE>();
                    foreach (var sr in ListServiceReq)
                    {
                        var longServiceId = sr.TDL_SERVICE_IDS;
                        if (sampleList != null && sampleList.Count > 0)
                        {
                            var sample = sampleList.FirstOrDefault(o => o.SERVICE_REQ_CODE == sr.SERVICE_REQ_CODE);
                            if (sample != null)
                            {
                                if (!dic.ContainsKey(sample))
                                    dic[sample] = longServiceId;
                                if (longServiceId.Contains(";"))
                                {
                                    var arrList = longServiceId.Split(';').ToList();
                                    serviceDataList.AddRange(lstHisService.Where(o => arrList.Exists(p => p == o.ID.ToString())).ToList());
                                }
                                else
                                {
                                    serviceDataList.AddRange(lstHisService.Where(o => o.ID.ToString() == longServiceId).ToList());
                                }
                            }
                        }
                    }
                    serviceDataList = serviceDataList.Distinct().ToList();
                }
                else if (serviceReq != null && serviceReq.ID > 0)
                {
                    var longServiceId = serviceReq.TDL_SERVICE_IDS;
                    if (!String.IsNullOrEmpty(longServiceId))
                    {
                        if (longServiceId.Contains(";"))
                        {
                            var arrList = longServiceId.Split(';').ToList();
                            serviceDataList = lstHisService.Where(o => arrList.Exists(p => p == o.ID.ToString())).ToList();
                        }
                        else
                        {
                            serviceDataList = lstHisService.Where(o => o.ID.ToString() == longServiceId).ToList();
                        }
                    }
                }
                serviceNameMess = new List<string>();
                if (serviceDataList != null && serviceDataList.Count > 0)
                {
                    if (ListServiceReq != null && ListServiceReq.Count > 0)
                    {
                        foreach (var item in serviceDataList)
                        {
                            foreach (var dv in dic)
                            {
                                if ((";" + dv.Value + ";").Contains(";" + item + ";"))
                                {
                                    var Date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dv.Key.INTRUCTION_TIME ?? 0);
                                    var instructionMinute = this.dtSampleTime.DateTime - (Date ?? DateTime.Now);
                                    if (!item.WARNING_SAMPLING_TIME.HasValue || item.WARNING_SAMPLING_TIME > (instructionMinute.Hours * 60 + instructionMinute.Minutes))
                                        continue;
                                    serviceNameMess.Add(item.SERVICE_NAME);
                                }
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sample.INTRUCTION_TIME), this.sample.INTRUCTION_TIME));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtSampleTime.DateTime.ToString("yyyyMMddHHmmss")), this.dtSampleTime.DateTime.ToString("yyyyMMddHHmmss")));
                        var Date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sample.INTRUCTION_TIME ?? 0);
                        var instructionMinute = this.dtSampleTime.DateTime - (Date ?? DateTime.Now);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => instructionMinute), instructionMinute));
                        foreach (var item in serviceDataList)
                        {
                            if (!item.WARNING_SAMPLING_TIME.HasValue || item.WARNING_SAMPLING_TIME > (instructionMinute.Hours * 60 + instructionMinute.Minutes))
                                continue;
                            serviceNameMess.Add(item.SERVICE_NAME);
                        }
                    }
                }
                serviceNameMess = serviceNameMess.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSampleTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsFirstLoadForm) return;
                InitDateTime();
                MessageWarning();
                ShowMessage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.sample == null) return;
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                bool IsNotSave = false;
                success = ProcessSave(ref param, ref IsNotSave);
                WaitingManager.Hide();
                if (!IsNotSave)
                    MessageManager.Show(this, param, success);

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessSave(ref CommonParam param, ref bool IsNotSave)
        {
            try
            {
                LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                sdo.SampleId = this.sample.ID;
                sdo.IsBedRoom = this.currentModuleBase.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG;
                sdo.RequestDepartmentCode = this.room.DEPARTMENT_CODE;
                sdo.RequestRoomCode = this.room.ROOM_CODE;
                if (cboSampleUser.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboSampleUser.EditValue.ToString());
                    if (user != null)
                    {
                        sdo.SampleLoginname = user.LOGINNAME;
                        sdo.SampleUsername = user.USERNAME;
                    }
                }
                sdo.SampleTypeId = Convert.ToInt64(cboSampleType.EditValue);
                if (cboSampleType.EditValue != null)
                {
                    var samType = this.sampleTypes.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampleType.EditValue));
                    if (samType != null)
                    {
                        sdo.TubeTypeName = samType.TUBE_TYPE_NAME;
                    }
                }

                sdo.SampleTime = Convert.ToInt64(dtSampleTime.DateTime.ToString("yyyyMMddHHmmss"));

                sdo.AppointmentTime = (dtAppointmentTime.DateTime != null && dtAppointmentTime.DateTime != DateTime.MinValue) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointmentTime.DateTime) : null;
                if (sdo.AppointmentTime.HasValue && sdo.SampleTime > sdo.AppointmentTime)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Thời gian hẹn trả không được nhỏ hơn thời gian lấy mẫu", "Thông báo");
                    IsNotSave = true;
                    return false;

                }
                WaitingManager.Show();
                if (!string.IsNullOrEmpty(txtAppointmentPlace.Text.Trim()))
                {
                    sdo.AppointmentPlace = txtAppointmentPlace.Text.Trim();
                }
                else
                {
                    sdo.AppointmentPlace = null;
                }

                if (cboMediOrgCode.EditValue != null)
                {
                    HIS_MEDI_ORG org = listMediOrg != null ? listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                    sdo.SampleSenderCode = org != null ? org.MEDI_ORG_CODE : "";
                    sdo.SampleSender = org != null ? org.MEDI_ORG_NAME : "";
                }
                else
                {
                    sdo.SampleSender = txtSampleSender.Text ?? "";
                    sdo.SampleSenderCode = null;
                }

                if (!String.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    sdo.Barcode = txtBarcode.Text.Trim();
                }
                if (cboPatientStatus.EditValue != null)
                {
                    sdo.PatientConditionId = long.Parse(cboPatientStatus.EditValue.ToString());
                }
                else
                {
                    sdo.PatientConditionId = null;
                }
                LisSampleSampleListSDO sdoList = new LisSampleSampleListSDO();
                bool IsSuccess = false;
                if (sampleList != null && sampleList.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<LisSampleSampleListSDO>(sdoList, sdo);
                    sdoList.SampleIds = sampleList.Select(o => o.ID).ToList();
                    sdoList.WorkingRoomId = this.currentModuleBase.RoomId;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdoList), sdoList));
                    var rs = new BackendAdapter(param).Post<List<V_LIS_SAMPLE>>("api/LisSample/SampleList", ApiConsumers.LisConsumer, sdoList, param);
                    if (rs != null)
                    {
                        IsSuccess = true;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumers.LisConsumer, sdo, param);
                    if (rs != null)
                    {
                        IsSuccess = true;
                        this.sample.SAMPLE_STT_ID = rs.SAMPLE_STT_ID;
                        this.sample.SAMPLE_TYPE_ID = rs.SAMPLE_TYPE_ID;
                        this.sample.SAMPLE_TIME = rs.SAMPLE_TIME;
                        this.sample.SAMPLE_LOGINNAME = rs.SAMPLE_LOGINNAME;
                        this.sample.SAMPLE_USERNAME = rs.SAMPLE_USERNAME;
                        this.sample.BARCODE = rs.BARCODE;
                        this.sample.APPOINTMENT_TIME = rs.APPOINTMENT_TIME;
                        this.sample.APPOINTMENT_PLACE = rs.APPOINTMENT_PLACE;
                        this.sample.SAMPLE_ORDER = rs.SAMPLE_ORDER;
                        this.sample.SAMPLE_SENDER = rs.SAMPLE_SENDER;
                        this.sample.SAMPLE_SENDER_CODE = rs.SAMPLE_SENDER_CODE;
                        btnPrint.Enabled = true;
                        Inventec.Common.Logging.LogSystem.Debug("serviceReqCode_________" + this.serviceReqCode);
                        if (this.sample != null && !string.IsNullOrEmpty(this.sample.SERVICE_REQ_CODE))
                            btnPrintAssign.Enabled = true;
                    }
                }
                SaveCheckBoxCacheClient();
                SavePatientStatusCacheClient();
                return IsSuccess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private void SavePatientStatusCacheClient()
        {
            try
            {

                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboPatientStatus.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));

                if (csAddOrUpdate != null)
                {
                    if (chkPreviousStatus.Checked == true)
                    {
                        csAddOrUpdate.VALUE = cboPatientStatus.EditValue.ToString() ?? "";
                    }
                    else
                    {
                        csAddOrUpdate.VALUE = "";
                    }
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboPatientStatus.Name;
                    if (chkPreviousStatus.Checked == true)
                    {
                        csAddOrUpdate.VALUE = cboPatientStatus.EditValue != null ? cboPatientStatus.EditValue.ToString() : "";
                    }
                    else
                    {
                        csAddOrUpdate.VALUE = "";
                    }
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveCheckBoxCacheClient()
        {
            try
            {
                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPreviousStatus.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateChkAfterPrint = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkCloseAfterPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPreviousStatus.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPreviousStatus.Name;
                    csAddOrUpdate.VALUE = (chkPreviousStatus.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                if (csAddOrUpdateChkAfterPrint != null)
                {
                    csAddOrUpdateChkAfterPrint.VALUE = chkCloseAfterPrint.Checked ? "1" : "";
                }
                else
                {
                    csAddOrUpdateChkAfterPrint = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateChkAfterPrint.KEY = chkCloseAfterPrint.Name;
                    csAddOrUpdateChkAfterPrint.VALUE = chkCloseAfterPrint.Checked ? "1" : "";
                    csAddOrUpdateChkAfterPrint.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateChkAfterPrint);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSaveAndPrint.Enabled || !dxValidationProvider1.Validate() || this.sample == null) return;
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                bool IsNotSave = false;
                success = ProcessSave(ref param, ref IsNotSave);
                WaitingManager.Hide();
                if (!IsNotSave)
                {
                    MessageManager.Show(this, param, success);
                    if (success && (sampleList == null || sampleList.Count == 0))
                    {
                        IsSaveAndPrint = true;
                        System.Threading.Thread.Sleep(1000);
                        this.onClickBtnPrintBarCode();
                        IsSaveAndPrint = false;
                        if (chkCloseAfterPrint.Checked)
                            this.Close();
                    }
                }
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.sample == null) return;
                this.onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickBtnPrintBarCode()
        {
            try
            {
                if (LisConfigCFG.PRINT_BARCODE_BY_BARTENDER == "1")
                {
                    this.PrintBarcodeByBartender();
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = this.sample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, null).FirstOrDefault();

                LisSampleViewFilter samleFilter = new LisSampleViewFilter();
                samleFilter.ID = this.sample.ID;
                List<V_LIS_SAMPLE> samples = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, samleFilter, null);
                V_LIS_SAMPLE print = samples != null ? samples.FirstOrDefault() : null;

                LisSampleServiceViewFilter samleServiceFilter = new LisSampleServiceViewFilter();
                samleServiceFilter.SAMPLE_ID = this.sample.ID;
                List<V_LIS_SAMPLE_SERVICE> sampleServices = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE_SERVICE>>("api/LisSampleService/GetView", ApiConsumers.LisConsumer, samleServiceFilter, null);
                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           print,
                           rs,
                           sampleServices
                           );
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (IsSaveAndPrint)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);

                    if (chkCloseAfterPrint.Checked)
                        this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void PrintBarcodeByBartender()
        {
            try
            {
                if (StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {
                    ClientPrintADO ado = new ClientPrintADO();
                    ado.Barcode = this.sample.BARCODE;
                    if (this.sample.DOB.HasValue)
                    {
                        ado.DobYear = this.sample.DOB.Value.ToString().Substring(0, 4);
                        ado.DobAge = MPS.AgeUtil.CalculateFullAge(this.sample.DOB.Value);
                    }
                    ado.ExecuteRoomCode = this.sample.EXECUTE_ROOM_CODE;
                    ado.ExecuteRoomName = this.sample.EXECUTE_ROOM_NAME ?? "";
                    ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(this.sample.EXECUTE_ROOM_NAME ?? "");
                    ado.GenderName = (!String.IsNullOrWhiteSpace(this.sample.GENDER_CODE)) ? (this.sample.GENDER_CODE == "01" ? "Nữ" : "Nam") : "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.PatientCode = this.sample.PATIENT_CODE ?? "";
                    var data = BackendDataWorker.Get<LIS.EFMODEL.DataModels.LIS_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == (sample.SAMPLE_TYPE_ID ?? 0));
                    if (data != null)
                    {
                        ado.SampleTypeCode = data.SAMPLE_TYPE_CODE;
                        ado.SampleTypeName = data.SAMPLE_TYPE_NAME;
                    }
                    ado.PrintTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Get.Now() ?? 0);
                    List<string> name = new List<string>();
                    if (!String.IsNullOrWhiteSpace(this.sample.LAST_NAME))
                    {
                        name.Add(this.sample.LAST_NAME);
                    }
                    if (!String.IsNullOrWhiteSpace(this.sample.FIRST_NAME))
                    {
                        name.Add(this.sample.FIRST_NAME);
                    }
                    ado.PatientName = String.Join(" ", name);
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.RequestDepartmentCode = this.sample.REQUEST_DEPARTMENT_CODE ?? "";
                    ado.RequestDepartmentName = this.sample.REQUEST_DEPARTMENT_NAME ?? "";
                    ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    ado.ServiceReqCode = this.sample.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = this.sample.TREATMENT_CODE;
                    ado.RequestRoomName = this.sample.REQUEST_ROOM_NAME;
                    ado.RequestRoomCode = this.sample.REQUEST_ROOM_CODE;
                    ado.RequestRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestRoomName);
                    ado.SampleTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.sample.SAMPLE_TIME ?? 0);
                    ado.ResultTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.sample.RESULT_TIME ?? 0);
                    ado.AppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.sample.APPOINTMENT_TIME ?? 0);
                    BartenderPrintClientManager client = new BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }
                }
                else
                {
                    LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediOrgCode.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    ProcessOpenModuleMediOrg();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessOpenModuleMediOrg()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMediOrg").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisMediOrg'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisMediOrg' is not plugins");
                List<object> listArgs = new List<object>();
                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                ((Form)extenceInstance).ShowDialog();
                WaitingManager.Show();
                this.InitComboMediOrgCode();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediOrgCode.EditValue != null)
                    {
                        HIS_MEDI_ORG org = this.listMediOrg != null ? this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                        if (org != null)
                        {
                            txtMediOrgCode.Text = org.MEDI_ORG_CODE ?? "";
                            txtSampleSender.Text = "";
                            txtSampleSender.Enabled = false;
                        }
                    }
                    else
                    {
                        txtMediOrgCode.Text = "";
                        txtSampleSender.Focus();
                        txtSampleSender.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMediOrgCode.EditValue != null)
                {
                    HIS_MEDI_ORG org = this.listMediOrg != null ? this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                    if (org != null)
                    {
                        cboMediOrgCode.ToolTip = org.MEDI_ORG_NAME ?? "";
                        txtMediOrgCode.Text = org.MEDI_ORG_CODE ?? "";
                        txtSampleSender.Text = "";
                        txtSampleSender.Enabled = false;
                    }
                }
                else
                {
                    cboMediOrgCode.ToolTip = "";
                    txtMediOrgCode.Text = "";
                    txtSampleSender.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediOrgCode.EditValue != null)
                    {
                        HIS_MEDI_ORG org = this.listMediOrg != null ? this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                        if (org != null)
                        {
                            txtMediOrgCode.Text = org.MEDI_ORG_CODE ?? "";
                            txtSampleSender.Text = "";
                            txtSampleSender.Enabled = false;
                        }
                    }
                    else
                    {
                        txtMediOrgCode.Text = "";
                        txtSampleSender.Enabled = true;
                        txtSampleSender.Focus();
                        txtSampleSender.SelectAll();
                    }
                }
                else if (e.KeyCode != Keys.Tab)
                {
                    cboMediOrgCode.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediOrgCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtMediOrgCode.Text))
                    {
                        string text = txtMediOrgCode.Text.Trim();
                        List<HIS_MEDI_ORG> lstData = this.listMediOrg != null ? this.listMediOrg.Where(o => o.MEDI_ORG_CODE.Contains(text)).ToList() : null;
                        if (lstData != null && lstData.Count == 1)
                        {
                            cboMediOrgCode.EditValue = lstData[0].MEDI_ORG_CODE;
                            txtSampleSender.Text = "";
                            txtSampleSender.Enabled = false;
                        }
                        else
                        {
                            cboMediOrgCode.Focus();
                            cboMediOrgCode.ShowPopup();
                        }
                    }
                    else
                    {
                        cboMediOrgCode.Focus();
                        cboMediOrgCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSampleSender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientStatus.Focus();
                    cboPatientStatus.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintAssign_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.sample.SERVICE_REQ_CODE))
                {
                    WaitingManager.Show();
                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.SERVICE_REQ_CODE = this.sample.SERVICE_REQ_CODE;
                    var _LisServiceReq = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (_LisServiceReq != null && _LisServiceReq.Count() > 0)
                    {
                        var serviceReq = _LisServiceReq.FirstOrDefault();

                        CommonParam param = new CommonParam();
                        HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                        HisServiceReqSDO.SereServs = GetSereServ(serviceReq.TREATMENT_ID, _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                        HisServiceReqSDO.ServiceReqs = _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                        HisServiceReqSDO.SereServBills = GetSereServBills(serviceReq.TREATMENT_ID, _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                        HisServiceReqSDO.SereServDeposits = GetSereServDeposits(serviceReq.TREATMENT_ID, HisServiceReqSDO.SereServs.Select(o => o.ID).ToList());
                        List<V_HIS_BED_LOG> listBedLogs = GetBedLog(serviceReq.TREATMENT_ID);
                        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, serviceReq.TREATMENT_ID, param);
                        HisTreatmentFilter ft = new HisTreatmentFilter();
                        ft.ID = serviceReq.TREATMENT_ID;
                        var currentTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, ft, param);

                        HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, currentTreatment);

                        if (patientTypeAlter != null)
                        {
                            HisTreatment.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                            HisTreatment.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                            HisTreatment.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                            HisTreatment.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                            HisTreatment.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                            HisTreatment.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                            HisTreatment.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                            HisTreatment.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_ADDRESS = patientTypeAlter.ADDRESS;
                        }

                        var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs);
                        WaitingManager.Hide();
                        PrintServiceReqProcessor.Print("Mps000026", false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERE_SERV> GetSereServ(long treatmentId, List<long> srList)
        {
            List<V_HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.HAS_EXECUTE = true;
                filter.SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<HIS_SERE_SERV_BILL> GetSereServBills(long treatmentId, List<long> srList)
        {
            List<HIS_SERE_SERV_BILL> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServBillFilter Filter = new HisSereServBillFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.TDL_SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<HIS_SERE_SERV_DEPOSIT> GetSereServDeposits(long treatmentId, List<long> ssList)
        {
            List<HIS_SERE_SERV_DEPOSIT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServDepositFilter Filter = new HisSereServDepositFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.SERE_SERV_IDs = ssList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<V_HIS_BED_LOG> GetBedLog(long treatmentId)
        {
            List<V_HIS_BED_LOG> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                ShowMessage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtAppointmentTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtAppointmentTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAppointmentPlace_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                txtAppointmentPlace.Text = "";
            }
        }
    }
}
