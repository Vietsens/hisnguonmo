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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.UC.SecondaryIcd;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.ApprovalSurgery.ADO;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using System.Threading;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {

        internal void FillDataToGridPtttCalendar()
        {
            if (ucPagingPtttCalendar.pagingGrid != null)
            {
                numPageSizePtttCalendar = ucPagingPtttCalendar.pagingGrid.PageSize;
            }
            else
            {
                numPageSizePtttCalendar = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
            }

            FillDataToGridPtttCalendar(new CommonParam(0, (int)numPageSizePtttCalendar));
            CommonParam param = new CommonParam();
            param.Limit = rowCountPtttCalendar;
            param.Count = dataTotalPtttCalendar;
            ucPagingPtttCalendar.Init(FillDataToGridPtttCalendar, param, numPageSizePtttCalendar, gridControlPtttCalendar);
        }

        internal void FillDataToGridPtttCalendar(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSizePtttCalendar = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_PTTT_CALENDAR>> apiResult = new ApiResultObject<List<V_HIS_PTTT_CALENDAR>>();
                HisPtttCalendarViewFilter hisPtttCalendarViewFilter = new HisPtttCalendarViewFilter();
                if (cboDepartment.EditValue != null)
                {
                    hisPtttCalendarViewFilter.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                }



                hisPtttCalendarViewFilter.ORDER_FIELD = "MODIFY_TIME";
                hisPtttCalendarViewFilter.ORDER_DIRECTION = "DESC";
                if (dateTxtTu.EditValue != null)
                {
                    hisPtttCalendarViewFilter.DATE_FROM__FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dateTxtTu.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dateTxtDen.EditValue != null)
                {
                    hisPtttCalendarViewFilter.DATE_TO__TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dateTxtDen.EditValue).ToString("yyyyMMdd") + "235959");
                }

                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_PTTT_CALENDAR>>("api/HisPtttCalendar/GetView", ApiConsumers.MosConsumer, hisPtttCalendarViewFilter, paramCommon);

                gridControlPtttCalendar.DataSource = null;
                if (apiResult != null)
                {
                    rowCountPtttCalendar = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotalPtttCalendar = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlPtttCalendar.DataSource = apiResult.Data;
                    if (apiResult.Data != null && apiResult.Data.Count > 0)
                    {
                        ptttCalendar = apiResult.Data[0];
                        FillDataToGridServiceReq();
                    }
                }

                // gridViewPtttCalendar.OptionsSelection.EnableAppearanceFocusedCell = false;
                //gridViewPtttCalendar.OptionsSelection.EnableAppearanceFocusedRow = false;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        internal void FillDataToGridServiceReq()
        {
            sereServ13Choose = new List<V_HIS_SERE_SERV_13>();
            if (ucPagingServiceReq.pagingGrid != null)
            {
                numPageSizeServiceReq = ucPagingServiceReq.pagingGrid.PageSize;
            }
            else
            {
                numPageSizeServiceReq = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
            }

            FillDataToGridServiceReq(new CommonParam(0, (int)numPageSizeServiceReq));
            CommonParam param = new CommonParam();
            param.Limit = rowCountServiceReq;
            param.Count = dataTotalServiceReq;
            ucPagingServiceReq.Init(FillDataToGridServiceReq, param, numPageSizeServiceReq, gridControlPtttCalendar);
        }

        internal void FillDataToGridServiceReq(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSizeServiceReq = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_SERE_SERV_13>> apiResult = new ApiResultObject<List<V_HIS_SERE_SERV_13>>();
                HisSereServView13Filter hisSereServFilter = new HisSereServView13Filter();
                hisSereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);

                if (cboStatus.EditValue != null && cboStatus.SelectedIndex != 0)
                {
                    List<long> ptttApprovalIds = new List<long>();
                    switch (cboStatus.SelectedIndex)
                    {
                        case 1:
                            ptttApprovalIds.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW);
                            break;
                        case 2:
                            ptttApprovalIds.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED);
                            break;
                        case 3:
                            ptttApprovalIds.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__REJECTED);
                            break;
                        default:
                            break;
                    }
                    hisSereServFilter.PTTT_APPROVAL_STT_IDs = ptttApprovalIds;
                }

                if (rdoGroupCalendar.SelectedIndex == 0)
                {
                    hisSereServFilter.IS_IN_CALENDAR = true;
                    hisSereServFilter.PTTT_CALENDAR_ID = ptttCalendar != null && rdoGroupCalendar.SelectedIndex == 0
    ? (long?)ptttCalendar.ID : -99999;
                }
                else if (rdoGroupCalendar.SelectedIndex == 1)
                {
                    hisSereServFilter.IS_IN_CALENDAR = false;
                    //hisSereServFilter.PTTT_APPROVAL_STT_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED, IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW };
                }

                hisSereServFilter.EXECUTE_DEPARTMENT_ID = room.DEPARTMENT_ID;
                hisSereServFilter.KEY_WORD = txtKeyWord.Text;
                hisSereServFilter.ORDER_FIELD = "INTRUCTION_TIME";
                hisSereServFilter.ORDER_DIRECTION = "DESC";
				if (dteFrom.EditValue != null)
				{
					hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
					Convert.ToDateTime(dteFrom.EditValue).ToString("yyyyMMdd") + "000000");
				}
				if (dteTo.EditValue != null)
				{
					hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
					Convert.ToDateTime(dteTo.EditValue).ToString("yyyyMMdd") + "235959");
				}
				if (cboDepartmentServiceReq.EditValue != null && !string.IsNullOrEmpty(cboDepartmentServiceReq.EditValue.ToString()))
				{
					hisSereServFilter.REQUEST_DEPARTMENT_ID = Int64.Parse(cboDepartmentServiceReq.EditValue.ToString());

				}

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisSereServFilter), hisSereServFilter));
				apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_SERE_SERV_13>>("api/HisSereServ/GetView13", ApiConsumers.MosConsumer, hisSereServFilter, paramCommon);

                gridControlServiceReq.DataSource = null;
                if (apiResult != null)
                {
                    rowCountServiceReq = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotalServiceReq = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlServiceReq.DataSource = apiResult.Data;
                    if (apiResult.Data != null && apiResult.Data.Count > 0)
                    {
                        if (sereServ13Choose != null && sereServ13Choose.Count > 0)
                        {
                            gridViewServiceReq.BeginUpdate();
                            foreach (var item in sereServ13Choose)
                            {
                                V_HIS_SERE_SERV_13 temp = apiResult.Data.FirstOrDefault(o => o.ID == item.ID);
                                if (temp != null)
                                {
                                    int rowHandle = gridViewServiceReq.LocateByValue("SERVICE_REQ_ID", item.ID);
                                    if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                                    {
                                        gridViewServiceReq.SelectRow(rowHandle);
                                    }
                                }
                            }
                            gridViewServiceReq.EndUpdate();
                        }
                    }

                }

                //gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = false;
                //gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = false;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadControlDefault()
        {
            try
            {
                //FillDataToGridPtttCalendar(); //FillDataToGridServiceReq(); FillDataToGridPtttCalendar: Sau khi thay doi du lieu tai cbo khoa thuc hien thi thuc hien
                InitEkipPlanUser();
                cboEmotionless.EditValue = null;
                txtEmotionless.Text = "";
                cboMethod.EditValue = null;
                txtMethod.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDepartment(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDepartment.EditValue = data[0].ID;
                            cboDepartment.Properties.Buttons[1].Visible = true;
                            btnCreateCalendar.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.DEPARTMENT_CODE.ToLower() == searchCode.ToLower());
                            if (search != null)
                            {
                                cboDepartment.EditValue = search.ID;
                                cboDepartment.Properties.Buttons[1].Visible = true;
                                btnCreateCalendar.Focus();
                            }
                            else
                            {
                                cboDepartment.EditValue = null;
                                cboDepartment.Focus();
                                cboDepartment.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitEkipPlanUser()
        {
            try
            {
                gridControlEkipPlanUser.DataSource = null;
                List<EkipPlanUserADO> ekipPlanUserADOs = new List<EkipPlanUserADO>();
                EkipPlanUserADO ekipPlanUserADO = new EkipPlanUserADO();
                ekipPlanUserADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                ekipPlanUserADOs.Add(ekipPlanUserADO);
                gridControlEkipPlanUser.DataSource = ekipPlanUserADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<EkipPlanUserADO> ekipUserAdoTemps = new List<EkipPlanUserADO>();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        EkipPlanUserADO ekipUserAdoTemp = new EkipPlanUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    gridControlEkipPlanUser.DataSource = ekipUserAdoTemps;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfoFromSereServ13(V_HIS_SERE_SERV_13 sereServ13)
        {
            try
            {
                WaitingManager.Show();
                dxValidationProvider1.SetValidationRule(dtPlanTimeFrom, null);
                dxValidationProvider1.SetValidationRule(dtPlanTimeTo, null);
                dxValidationProvider1.SetValidationRule(cboRoom, null);
                if (sereServ13 != null)
                {
                    cboEkipTemp.EditValue = null;
                    LoadInfoFromSereServ13(null);
                    lblTreatmentCode.Text = sereServ13.TDL_TREATMENT_CODE;
                    lblPatientCode.Text = sereServ13.TDL_PATIENT_CODE;
                    lblServiceReqCode.Text = sereServ13.TDL_SERVICE_REQ_CODE;
                    lblPatientName.Text = sereServ13.TDL_PATIENT_NAME;

                    if (sereServ13.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        lblDob.Text = sereServ13.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(sereServ13.TDL_PATIENT_DOB
                            .ToString());
                    lblGender.Text = sereServ13.TDL_PATIENT_GENDER_NAME;

                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                        .FirstOrDefault(o => o.ID == sereServ13.TDL_REQUEST_DEPARTMENT_ID);
                    lblDepartmentReq.Text = department != null ? department.DEPARTMENT_NAME : null;
                    lblLoginnameReq.Text = sereServ13.TDL_REQUEST_LOGINNAME;
                    lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServ13.INTRUCTION_TIME);

                    string icdMain = "";
                    if (!String.IsNullOrEmpty(sereServ13.ICD_CODE))
                    {
                        icdMain = sereServ13.ICD_CODE + " - ";
                    }
                    if (!String.IsNullOrEmpty(sereServ13.ICD_NAME))
                    {
                        icdMain += sereServ13.ICD_NAME;
                    }

                    lblICDMain.Text = icdMain;
                    lblICDExtra.Text = sereServ13.ICD_TEXT;

                    gridControlService.DataSource = new List<V_HIS_SERE_SERV_13> { sereServ13 };

                    if (sereServ13.PLAN_TIME_FROM.HasValue)
                    {
                        dtPlanTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServ13.PLAN_TIME_FROM.Value).Value;
                    }
                    if (sereServ13.PLAN_TIME_TO.HasValue)
                    {
                        dtPlanTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServ13.PLAN_TIME_TO.Value).Value;
                    }
                    HIS_SERVICE_REQ serviceReq = GetServiceReqByID(sereServ13.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        txtPlanningRequest.Text = serviceReq.PLANNING_REQUEST;
                        txtSurgeryNote.Text = serviceReq.SURGERY_NOTE;
                       
                    }



                    //t_2 = new Thread(() =>
                    //{
                        HIS_SERE_SERV_PTTT HisSer = new HIS_SERE_SERV_PTTT();
                        CommonParam param_ = new CommonParam();
                        MOS.Filter.HisSereServPtttFilter filter_ = new HisSereServPtttFilter();
                        filter_.SERE_SERV_ID = sereServ13.ID;
                        HisSer = new BackendAdapter(param_).Get<List<HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, filter_, param_).FirstOrDefault();

                        if (HisSer != null)
                        {
                            txtMANNER.Text = HisSer.MANNER;
                        }
                    //});
                    //t_2.IsBackground = true;
                    //t_2.Start();
                   
                    //t_ = new Thread(() =>
                    //{
                        HIS_TREATMENT HisTreatment = new HIS_TREATMENT();
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.TREATMENT_CODE__EXACT = sereServ13.TDL_TREATMENT_CODE;
                        HisTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                        
                        if (HisTreatment != null)
                        {
                            id = HisTreatment.ID;
                        }
                        //id = BackendDataWorker.Get<HIS_TREATMENT>().FirstOrDefault(o => o.TREATMENT_CODE == sereServ13.TDL_TREATMENT_CODE).ID;
                    //});
                    //t_.IsBackground = true;
                    //t_.Start();
                    LoadGridEkipPlanUser(sereServ13.EKIP_PLAN_ID);
                    LoadComboRoom();
                    LoadComboEmotionless();
                    LoadComboMethod();

                    WaitingManager.Hide();
                }
                else
                {
                    cboEkipTemp.EditValue = null;
                    lblTreatmentCode.Text = null;
                    lblPatientCode.Text = null;
                    lblServiceReqCode.Text = null;
                    lblPatientName.Text = null;
                    lblDob.Text = null;
                    lblGender.Text = null;
                    cboMethod.EditValue = null;
                    txtMethod.Text = null;
                    cboEmotionless.EditValue = null;
                    txtEmotionless.Text = null;
                    lblDepartmentReq.Text = null;
                    lblLoginnameReq.Text = null;
                    lblIntructionTime.Text = null;
                    lblICDMain.Text = null;
                    lblICDExtra.Text = null;
                    dtPlanTimeFrom.EditValue = null;
                    dtPlanTimeTo.EditValue = null;
                    txtPlanningRequest.Text = "";
                    txtSurgeryNote.Text = "";
                    gridControlService.DataSource = null;
                    sereServ13Choose = new List<V_HIS_SERE_SERV_13>();
                    sereServ13 = null;
                    InitEkipPlanUser();
                    txtMANNER.Text = null;
                    cboRoom.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_SERVICE_REQ GetServiceReqByID(long? id)
        {
            HIS_SERVICE_REQ result = null;
            try
            {
                if (id == null)
                    return null;
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadGridEkipPlanUser(long? ekipPlanId)
        {
            try
            {
                if (ekipPlanId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                    hisEkipPlanUserFilter.EKIP_PLAN_ID = ekipPlanId.Value;
                    var lst = new BackendAdapter(param)
            .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);
                    if (lst.Count > 0)
                    {
                        List<EkipPlanUserADO> ekipPlanUserADOs = new List<EkipPlanUserADO>();
                        foreach (var item in lst)
                        {
                            EkipPlanUserADO ado = new EkipPlanUserADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<EkipPlanUserADO>(ado, item);
                            if (item != lst[0])
                            {
                                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            ekipPlanUserADOs.Add(ado);
                        }
                        gridControlEkipPlanUser.DataSource = null;
                        gridControlEkipPlanUser.DataSource = ekipPlanUserADOs;
                    }
                }
                else
                {
                    InitEkipPlanUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal CalculatePlanTime()
        {
            decimal result = 0;
            try
            {
                if (dtPlanTimeFrom.EditValue != null && dtPlanTimeTo.EditValue != null)
                {
                    TimeSpan ts = new TimeSpan();
                    ts = (TimeSpan)(dtPlanTimeTo.DateTime - dtPlanTimeFrom.DateTime);
                    result = (decimal)ts.TotalHours;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnabledControl(bool isShow)
        {
            try
            {
                if (isShow)
                {
                    btnSave.Enabled = true;
                    btnLichSuDieuTri.Enabled = true;
                    btnDichVuDinhKem.Enabled = true;
                    //btnHoSoBenhAn.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnLichSuDieuTri.Enabled = false;
                    btnDichVuDinhKem.Enabled = false;
                    //btnHoSoBenhAn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableApproval(int index)
        {
            try
            {
                if (ptttCalendar != null && !ptttCalendar.APPROVAL_TIME.HasValue)
                {
                    switch (index)
                    {
                        case 0:
                            btnCalendarAdd.Enabled = false;
                            btnCalendarRemove.Enabled = true;
                            break;
                        case 1:
                            btnCalendarAdd.Enabled = true;
                            btnCalendarRemove.Enabled = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadControlRule()
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {

                    controlAcss = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
