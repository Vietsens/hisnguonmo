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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisKskDriverCreate.ADO;
using HIS.Desktop.Plugins.HisKskDriverCreate.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.KskSignData;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.Run
{
    public partial class FormKskDriver : FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisKskDriverCreate.Resources.Lang", typeof(FormKskDriver).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnF2.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barBtnF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnF3.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barBtnF3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnF4.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barBtnF4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnCtrlF.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barBtnCtrlF.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnCtrlS.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barBtnCtrlS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCtrlR.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignFileCertUtil.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkSignFileCertUtil.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignFileCertUtil.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.chkSignFileCertUtil.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPush.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkAutoPush.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPush.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.chkAutoPush.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupKsk.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.groupKsk.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPositive.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkPositive.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNegative.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkNegative.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMgKhi.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkMgKhi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMgMau.Properties.Caption = Inventec.Common.Resource.Get.Value("FormKskDriver.chkMgMau.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboConclusionName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormKskDriver.cboConclusionName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLicenesClass.Properties.NullText = Inventec.Common.Resource.Get.Value("FormKskDriver.cboLicenesClass.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboConclusion.Properties.NullText = Inventec.Common.Resource.Get.Value("FormKskDriver.cboConclusion.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatient.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclusionTime.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciConclusionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKskDriverCode.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciKskDriverCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclusion.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciConclusion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLicenesClass.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciLicenesClass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclusionName.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.lciConclusionName.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclusionName.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciConclusionName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReasonBadHeathly.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.lciReasonBadHeathly.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReasonBadHeathly.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciReasonBadHeathly.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSickCondition.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciSickCondition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentration.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciConcentration.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDrug.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciDrug.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupPatient.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.groupPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPatientCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPatientName.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCccdNumber.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciCccdNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPlaceCccd.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciPlaceCccd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeCccd.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciTimeCccd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormKskDriver.txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormKskDriver.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSearchTreatmentCode.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciSearchTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSearchServiceReqCode.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciSearchServiceReqCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAutoPush.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.lciAutoPush.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAutoPush.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.lciAutoPush.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormKskDriver.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.txtCccdCmnd.Text = "";
                this.txtKskDriverCode.Text = "";
                this.txtPlaceCccd.Text = "";
                this.txtReasonBadHeathly.Text = "";
                this.txtSickCondition.Text = "";
                this.lblAddress.Text = "";
                this.lblDob.Text = "";
                this.lblGender.Text = "";
                this.lblPatientCode.Text = "";
                this.lblPatientName.Text = "";
                this.cboConclusion.EditValue = null;
                this.cboConclusionName.EditValue = null;
                this.cboLicenesClass.EditValue = null;
                this.chkMgKhi.EditValue = true;
                this.chkMgMau.EditValue = null;
                this.chkNegative.EditValue = null;
                this.chkPositive.EditValue = null;
                this.spConcentration.EditValue = 1;
                this.dtConclusionTime.EditValue = DateTime.Now;
                this.dtTimeCccd.EditValue = null;
                this.chkNegative.Checked = true;
                this.dtAppointmentTime.EditValue = null;
                this.gridViewKskDriver.GridControl.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCombo()
        {
            try
            {
                InitDataComboConclusionName();
                InitDataComboConclusion();
                LoadComboLicenseClass();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitDataComboConclusionName()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboConclusionName, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboConclusion()
        {
            try
            {
                List<TypeADO> listData = new List<TypeADO>();
                listData.Add(new TypeADO(1, "A0-1", ResourceMessage.DuDieuKienSucKhoeLaiXe));
                listData.Add(new TypeADO(2, "A0-2", ResourceMessage.KhongDuDieuKienSucKhoeLaiXe));
                listData.Add(new TypeADO(3, "A0-3", ResourceMessage.DatTieuChuanSucKhoeLaiXeNhungYcKhamLai));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Code", "", 100, 1));
                columnInfos.Add(new ColumnInfo("Name", "", 400, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Code", columnInfos, false, 500);
                ControlEditorLoader.Load(this.cboConclusion, listData, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoPush.Name)
                        {
                            chkAutoPush.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SerialNumber = item.VALUE;

                            chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void SearchDataProcess()
        {
            try
            {
                SetDefaultValueControl();

                if (String.IsNullOrWhiteSpace(txtPatietnCode.Text) && string.IsNullOrWhiteSpace(txtTreatmentCode.Text) && String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    return;
                }

                string error = "";
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                if (!String.IsNullOrWhiteSpace(txtPatietnCode.Text))
                {
                    string code = txtPatietnCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }

                    txtPatietnCode.Text = code;
                    filter.TDL_PATIENT_CODE__EXACT = code;
                    error = string.Format(ResourceMessage.KhongTimThayBenhNhanTheoMa, code);
                }
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }

                    txtTreatmentCode.Text = code;
                    filter.TREATMENT_CODE__EXACT = code;
                    error = string.Format(ResourceMessage.KhongTimThayBenhNhanTheoMaDieuTri, code);
                }
                else if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }

                    txtServiceReqCode.Text = code;
                    filter.SERVICE_REQ_CODE__EXACT = code;
                    error = string.Format(ResourceMessage.KhongTimThayBenhNhanTheoMaYLenh, code);
                }
                gridViewKskDriver.BeginUpdate();
                var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    btnSave.Enabled = true;
                    FillDataByServiceReq(apiResult.First());
                    listKskDrivers = GetListKskDriverByServiceReqId(apiResult.Select(o => o.ID).ToList());
                    gridViewKskDriver.GridControl.DataSource = listKskDrivers;
                }
                else
                {
                    gridViewKskDriver.GridControl.DataSource = null;
                    XtraMessageBox.Show(error);
                }
                gridViewKskDriver.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void LoadDataEdit()
        {
            try
            {
                if (this.kskDriver != null)
                {
                    FillDataByKskDriver(this.kskDriver);
                    kskServiceReq = GetServiceReqById(this.kskDriver.SERVICE_REQ_ID);
                    FillDataByServiceReq(kskServiceReq);
                }
                else if (this.kskServiceReq != null)
                {
                    FillDataByServiceReq(this.kskServiceReq);
                }
                if(this.kskServiceReq != null)
                {
                    txtTreatmentCode.Enabled = false;
                    txtPatietnCode.Enabled = false;
                    txtServiceReqCode.Enabled = false;
                    txtServiceReqCode.Text = kskServiceReq.SERVICE_REQ_CODE;
                    List<long> serviceReqIds = new List<long>();
                    serviceReqIds.Add(this.kskServiceReq.ID);
                    this.listKskDrivers = GetListKskDriverByServiceReqId(serviceReqIds);
                    gridViewKskDriver.BeginUpdate();
                    gridViewKskDriver.GridControl.DataSource = listKskDrivers;
                    gridViewKskDriver.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataByServiceReq(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                this.processServiceReq = serviceReq;
                if (serviceReq != null)
                {
                    V_HIS_PATIENT patient = GetPatientById(serviceReq.TDL_PATIENT_ID);
                    if (patient != null)
                    {
                        this.lblAddress.Text = patient.VIR_ADDRESS;
                        if (patient.IS_HAS_NOT_DAY_DOB == 1)
                        {
                            this.lblDob.Text = patient.DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                        }

                        this.lblGender.Text = patient.GENDER_NAME;
                        this.lblPatientCode.Text = patient.PATIENT_CODE;
                        this.lblPatientName.Text = patient.VIR_PATIENT_NAME;

                        if (!String.IsNullOrWhiteSpace(patient.CCCD_NUMBER))
                        {
                            this.txtCccdCmnd.Text = patient.CCCD_NUMBER;
                            this.txtPlaceCccd.Text = patient.CCCD_PLACE;
                            this.dtTimeCccd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.CCCD_DATE ?? 0);
                        }
                        else if (!String.IsNullOrWhiteSpace(patient.CMND_NUMBER))
                        {
                            this.txtCccdCmnd.Text = patient.CMND_NUMBER;
                            this.txtPlaceCccd.Text = patient.CMND_PLACE;
                            this.dtTimeCccd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.CMND_DATE ?? 0);
                        }
                        else if (!String.IsNullOrWhiteSpace(patient.PASSPORT_NUMBER))
                        {
                            this.txtCccdCmnd.Text = patient.PASSPORT_NUMBER;
                            this.txtPlaceCccd.Text = patient.PASSPORT_PLACE;
                            this.dtTimeCccd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.PASSPORT_DATE ?? 0);
                        }
                    }
                    else
                    {
                        this.lblAddress.Text = serviceReq.TDL_PATIENT_ADDRESS;
                        if (serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            this.lblDob.Text = serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);
                        }

                        this.lblGender.Text = serviceReq.TDL_PATIENT_GENDER_NAME;
                        this.lblPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                        this.lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataByKskDriver(HIS_KSK_DRIVER kskdriver, bool isEdit = true)
        {
            try
            {
                this.kskDriver = kskdriver;
                if (kskdriver != null)
                {
                    btnSave.Enabled = true;
                    this.ActionType = isEdit ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;

                    this.dtConclusionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(kskdriver.CONCLUSION_TIME);
                    this.txtKskDriverCode.Text = isEdit ? kskdriver.KSK_DRIVER_CODE : null;
                    this.txtReasonBadHeathly.Text = kskdriver.REASON_BAD_HEATHLY;
                    this.txtSickCondition.Text = kskdriver.SICK_CONDITION;
                    this.spConcentration.EditValue = kskdriver.CONCENTRATION;
                    this.cboConclusionName.EditValue = kskdriver.CONCLUDER_LOGINNAME;
                    this.dtAppointmentTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(kskdriver.APPOINTMENT_TIME ?? 0);
                    this.cboConclusion.EditValue = kskdriver.CONCLUSION;
                    if (kskdriver.CONCENTRATION_TYPE == 1)
                    {
                        this.chkMgKhi.Checked = true;
                    }
                    else if (kskdriver.CONCENTRATION_TYPE == 2)
                    {
                        this.chkMgMau.Checked = true;
                    }

                    if (kskdriver.DRUG_TYPE == 1)
                    {
                        this.chkNegative.Checked = true;
                    }
                    else if (kskdriver.DRUG_TYPE == 2)
                    {
                        this.chkPositive.Checked = true;
                    }
                    var license = lstLicenseClass.FirstOrDefault(o => o.LICENSE_CLASS_NAME == kskdriver.LICENSE_CLASS);
                    this.cboLicenesClass.EditValue = license != null ? license.ID : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_SERVICE_REQ GetServiceReqById(long serviceReqId)
        {
            HIS_SERVICE_REQ result = null;
            try
            {
                if (serviceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.ID = serviceReqId;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HIS_KSK_DRIVER GetKskDriverByServiceReqId(long serviceReqId)
        {
            HIS_KSK_DRIVER result = null;
            try
            {
                if (serviceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisKskDriverFilter filter = new HisKskDriverFilter();
                    filter.SERVICE_REQ_ID = serviceReqId;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_KSK_DRIVER>>("api/HisKskDriver/Get", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_KSK_DRIVER> GetListKskDriverByServiceReqId(List<long> serviceReqIds)
        {
            List<HIS_KSK_DRIVER> result = null;
            try
            {
                if (serviceReqIds != null && serviceReqIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisKskDriverFilter filter = new HisKskDriverFilter();
                    filter.SERVICE_REQ_IDs = serviceReqIds;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_KSK_DRIVER>>("api/HisKskDriver/Get", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private V_HIS_PATIENT GetPatientById(long patientId)
        {
            V_HIS_PATIENT result = null;
            try
            {
                if (patientId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientViewFilter filter = new HisPatientViewFilter();
                    filter.ID = patientId;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool SaveProcess(ref CommonParam param, ref bool calledApi)
        {
            bool success = false;
            try
            {
                HisKskDriverSDO sdo = new HisKskDriverSDO();
                SetData(sdo);
                if (!String.IsNullOrEmpty(sdo.LicenseClass) && this.listKskDrivers != null && this.listKskDrivers.Count > 0)
                {
                    HIS_KSK_DRIVER ksk = null;
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        ksk = listKskDrivers.FirstOrDefault(o => o.LICENSE_CLASS == sdo.LicenseClass);
                    }
                    if (this.ActionType == GlobalVariables.ActionEdit && this.kskDriver != null)
                    {
                        ksk = listKskDrivers.FirstOrDefault(o => o.LICENSE_CLASS != kskDriver.LICENSE_CLASS && o.LICENSE_CLASS == sdo.LicenseClass);
                    }
                    if (ksk != null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DaTonTaiHoSoSucKhoeHang + ksk.LICENSE_CLASS,
                                ResourceMessage.ThongBao,
                                MessageBoxButtons.OK);
                        dtTimeCccd.Focus();
                        dtTimeCccd.SelectAll();
                        calledApi = false;
                        return false;
                    }
                }
                if (sdo.CmndDate.ToString().Length < 14)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Ngày cấp CCCD/CMND/HC không hợp lệ",
                               ResourceMessage.ThongBao,
                               MessageBoxButtons.OK);
                    return calledApi = false;
                }
                WaitingManager.Show();               
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_KSK_DRIVER>
                    (this.ActionType == GlobalVariables.ActionAdd ?
                    "api/HisKskDriver/Create" :
                    "api/HisKskDriver/Update",
                    ApiConsumer.ApiConsumers.MosConsumer, sdo, SessionManager.ActionLostToken, param);
                calledApi = true;
                if (rs != null)
                {
                    success = true;
                    if (chkSignFileCertUtil.Checked)
                    {
                        List<KskDriverSyncSDO> listKskDriveSync = new List<KskDriverSyncSDO>();
                        if (certificate == null && !String.IsNullOrWhiteSpace(SerialNumber))
                        {
                            certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                            if (certificate == null)
                            {
                                chkSignFileCertUtil.Checked = false;
                                if (MessageBox.Show(Resources.ResourceMessage.KhongLayDuocThongTinChungThuBanCoMuonTiepTuc, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    return false;
                                }
                            }
                        }
                        HisKskDriverViewFilter filter = new HisKskDriverViewFilter();
                        filter.ID = rs.ID;
                        var apiResult = new BackendAdapter(new CommonParam()).Get<List<V_HIS_KSK_DRIVER>>("api/HisKskDriver/GetView", ApiConsumers.MosConsumer, filter, null);
                        if (apiResult != null)
                        {
                            var driver = apiResult.FirstOrDefault();
                            KskDataProcess data = new KskDataProcess();
                            KskDriverSyncSDO kskSdo = new KskDriverSyncSDO();
                            kskSdo.KskDriveId = driver.ID;
                            kskSdo.SyncData = data.MakeData(driver, certificate);
                            listKskDriveSync.Add(kskSdo);
                            if (listKskDriveSync != null && listKskDriveSync.Count > 0)
                            {
                                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisKskDriver/Sync", ApiConsumer.ApiConsumers.MosConsumer, listKskDriveSync, SessionManager.ActionLostToken, param);

                            }
                        }
                    }
                }
                WaitingManager.Hide();
                return success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return success;
            }
        }

        private void SetData(HisKskDriverSDO sdo)
        {
            try
            {
                if (dtTimeCccd.EditValue != null && dtTimeCccd.DateTime != DateTime.MinValue)
                    sdo.CmndDate = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeCccd.DateTime.ToString("yyyyyMMdd") + "000000");

                sdo.CmndNumber = this.txtCccdCmnd.Text.Trim();
                sdo.CmndPlace = this.txtPlaceCccd.Text.Trim();
                sdo.KskDriverCode = this.txtKskDriverCode.Text.Trim();
                sdo.LicenseClass = this.cboLicenesClass.Text.Trim();
                sdo.ReasonBadHealthy = this.txtReasonBadHeathly.Text.Trim();
                sdo.SickCondition = this.txtSickCondition.Text.Trim();
                sdo.Conclusion = this.cboConclusion.EditValue != null ? this.cboConclusion.EditValue.ToString() : "";
                if (this.spConcentration.EditValue != null)
                    sdo.Concentration = this.spConcentration.Value;

                if (chkMgKhi.Checked)
                {
                    sdo.ConcentrationType = 1;
                }
                else if (chkMgMau.Checked)
                {
                    sdo.ConcentrationType = 2;
                }

                if (cboConclusionName.EditValue != null)
                {
                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboConclusionName.EditValue.ToString());
                    if (acsUser != null)
                    {
                        sdo.ConcluderLoginname = acsUser.LOGINNAME;
                        sdo.ConcluderUsername = acsUser.USERNAME;
                    }
                }

                if (dtConclusionTime.EditValue != null && dtConclusionTime.DateTime != DateTime.MinValue)
                    sdo.ConclusionTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtConclusionTime.DateTime.ToString("yyyyMMddHHmmss"));

                if (chkNegative.Checked)
                {
                    sdo.DrugType = 1;
                }
                else if (chkPositive.Checked)
                {
                    sdo.DrugType = 2;
                }

                if (dtAppointmentTime.EditValue != null && dtAppointmentTime.DateTime != DateTime.MinValue)
                    sdo.AppointmentTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtAppointmentTime.DateTime.ToString("yyyyMMddHHmmss"));

                sdo.ServiceReqId = this.processServiceReq.ID;
                sdo.IsAutoSync = chkAutoPush.Checked;
                if (this.ActionType == GlobalVariables.ActionEdit && this.kskDriver != null)
                {
                    sdo.Id = this.kskDriver.ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLicenseClass()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_LICENSE_CLASS>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstLicenseClass = new List<HIS_LICENSE_CLASS>();
                if (data != null && data.Count() > 0)
                {
                    lstLicenseClass = data;
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("LICENSE_CLASS_NAME", "", 300, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("LICENSE_CLASS_NAME", "ID", columnInfos, false, 300);
                    ControlEditorLoader.Load(cboLicenesClass, lstLicenseClass, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
