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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.ADO;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Config;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {

        private void FillDataToGridServiceReqKidneyShift()
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_SERVICE_REQ_9> lstserviceReq = new List<V_HIS_SERVICE_REQ_9>();
                this._ServiceReqADOs = new List<ServiceReqADO>();

                gridControlServiceReqKidneyshift.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqView9Filter serviceReqViewFilter = new MOS.Filter.HisServiceReqView9Filter();
                SetServiceReqFilter(ref serviceReqViewFilter);

                lstserviceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERVICE_REQ_9>>(RequestUriStore.HIS_SERVICE_REQ_GETVIEW_9, ApiConsumers.MosConsumer, serviceReqViewFilter, paramCommon);
                lstserviceReq = lstserviceReq.Where(o => o.KIDNEY_SHIFT != null).ToList();
                if (lstserviceReq != null && lstserviceReq.Count > 0)
                {
                    this._ServiceReqADOs.AddRange((from r in lstserviceReq select new ServiceReqADO(r)).ToList());
                }
                this._ServiceReqADOs = this._ServiceReqADOs.OrderBy(o => o.INTRUCTION_DATE).ThenBy(o => o.KIDNEY_TIMES).ThenBy(o => o.MERCHINE_CODE).ThenBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                gridControlServiceReqKidneyshift.BeginUpdate();
                gridControlServiceReqKidneyshift.DataSource = this._ServiceReqADOs;
                gridControlServiceReqKidneyshift.EndUpdate();
                gridViewServiceReqKidneyshift.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewServiceReqKidneyshift.OptionsSelection.EnableAppearanceFocusedRow = false;
                gridViewServiceReqKidneyshift.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridControlServiceReqKidneyshift.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetServiceReqFilter(ref MOS.Filter.HisServiceReqView9Filter serviceReqViewFilter)
        {
            try
            {
                serviceReqViewFilter = serviceReqViewFilter == null ? new MOS.Filter.HisServiceReqView9Filter() : serviceReqViewFilter;
                serviceReqViewFilter.ORDER_DIRECTION = "ASC";
                serviceReqViewFilter.ORDER_FIELD = "INTRUCTION_DATE";
                serviceReqViewFilter.IS_KIDNEY = true;
                if (!String.IsNullOrEmpty(txtKeywordForSearchServiceReqKidneyshift.Text))
                {
                    serviceReqViewFilter.KEY_WORD = txtKeywordForSearchServiceReqKidneyshift.Text;
                }

                if (cboExecuteRoom.EditValue != null)
                    serviceReqViewFilter.EXECUTE_ROOM_ID = (long)cboExecuteRoom.EditValue;

                if (cboCaForSearchServiceReqKidneyshift.EditValue != null)
                    serviceReqViewFilter.KIDNEY_SHIFT = (long)cboCaForSearchServiceReqKidneyshift.EditValue;
                if (cboMarchineForSearchServiceReqKidneyshift.EditValue != null)
                    serviceReqViewFilter.MACHINE_ID = (long)cboMarchineForSearchServiceReqKidneyshift.EditValue;
                if (dateDateForSearchServiceReqKidneyshift.EditValue != null)
                {
                    serviceReqViewFilter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateForSearchServiceReqKidneyshift.DateTime.ToString("yyyyMMdd") + START_TIME);
                    serviceReqViewFilter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateForSearchServiceReqKidneyshift.DateTime.ToString("yyyyMMdd") + END_TIME);
                }
                else
                {
                    if (dateWeekFrom.EditValue != null)
                    {
                        serviceReqViewFilter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dateWeekFrom.DateTime.ToString("yyyyMMdd") + START_TIME);
                    }
                    if (dateWeekTo.EditValue != null)
                    {
                        serviceReqViewFilter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dateWeekTo.DateTime.ToString("yyyyMMdd") + END_TIME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi Tao Du Lieu Bed Room
        /// </summary>
        private void FillDataToGridTreatmentBedRoom()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridTreatmentPagging(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatmentPagging, param, pageSize, gridControlTreatmentBedRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatmentPagging(object param)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
                _TreatmentBedRoomADOs = new List<TreatmentBedRoomADO>();

                gridControlTreatmentBedRoom.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                SetTreatmentBedRoomFilter(ref treatFilter);
                var resultRO = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (resultRO != null)
                {
                    lstTreatmentBedRoom = (List<V_HIS_TREATMENT_BED_ROOM>)resultRO.Data.OrderBy(p => p.TDL_PATIENT_FIRST_NAME).ToList();
                    rowCount = (lstTreatmentBedRoom == null ? 0 : lstTreatmentBedRoom.Count);
                    dataTotal = (resultRO.Param == null ? 0 : resultRO.Param.Count ?? 0);
                }
                if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                {
                    _TreatmentBedRoomADOs.AddRange((from r in lstTreatmentBedRoom select new TreatmentBedRoomADO(r)).ToList());
                }
                _TreatmentBedRoomADOs = _TreatmentBedRoomADOs.OrderBy(o => o.BED_ROOM_NAME).ThenBy(o => o.TDL_PATIENT_FIRST_NAME).ThenBy(o => o.TDL_PATIENT_CODE).ToList();
                gridControlTreatmentBedRoom.BeginUpdate();
                gridControlTreatmentBedRoom.DataSource = _TreatmentBedRoomADOs;
                gridControlTreatmentBedRoom.EndUpdate();
                //gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
                //gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = false;
                gridViewTreatmentBedRoom.BestFitColumns();

                if (_TreatmentBedRoomADOs != null && _TreatmentBedRoomADOs.Count > 0)
                {
                    gridViewTreatmentBedRoom.FocusedRowHandle = 0;
                    RowTreatmentBedRoomRowClick();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridControlTreatmentBedRoom.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTreatmentBedRoomFilter(ref MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter)
        {
            try
            {
                treatFilter = treatFilter == null ? new MOS.Filter.HisTreatmentBedRoomViewFilter() : treatFilter;
                treatFilter.ORDER_DIRECTION = "ASC";
                treatFilter.ORDER_FIELD = "TDL_PATIENT_FIRST_NAME";//TODO   
                if (!chkSearchAllInDepartment.Checked)
                    treatFilter.IS_IN_ROOM = true;
                if (!String.IsNullOrEmpty(txtKeywordForPatientInBedroom.Text))
                {
                    treatFilter.KEYWORD__PATIENT_NAME__TREATMENT_CODE__BED_NAME__PATIENT_CODE = txtKeywordForPatientInBedroom.Text;
                }

                long? bedRoomId = null;
                MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = chkSearchAllInDepartment.Checked ? null : cboBedroomForPatientInBedroom.EditValue != null ? BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ROOM_ID == (long)cboBedroomForPatientInBedroom.EditValue) : null;
                if (data != null)
                {
                    bedRoomId = data.ID;
                }
                treatFilter.BED_ROOM_ID = bedRoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RowTreatmentBedRoomRowClick()
        {
            try
            {
                this.currentTreatmentBedRoomADO = (TreatmentBedRoomADO)this.gridViewTreatmentBedRoom.GetFocusedRow();

                if (this.currentTreatmentBedRoomADO != null)
                {
                    this.treatmentId = this.currentTreatmentBedRoomADO.TREATMENT_ID;
                    this.LoadDataToCurrentTreatmentData(this.treatmentId);
                    this.ProcessDataWithTreatmentWithPatientTypeInfo();
                    this.LoadServicePaty();
                    this.ResetStateControlForm();
                    this.InitComboPatientType(this.currentPatientTypeWithPatientTypeAlter);
                    if (cboServiceForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.ProcessServiceChange(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetStateControlForm()
        {
            try
            {
                this.cboMarchineForAdd.EditValue = null;
                this.txtNoteForAdd.Text = "";
                this.cboPatientType.EditValue = null;
                this.InitComboPatientType(null);
                this.cboExpMestTemplateForAdd.EditValue = null;
                this.actionType = GlobalVariables.ActionAdd;
                this.SetEnableButtonControl(this.actionType);
                this.serviceReqListResultSDO = null;
                this.ResetRequiredField();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm. 1");
                this.InitComboExecuteRoom(cboExecuteRoom);
                this.InitComboService();
                this.InitComboBedRoom();
                this.InitComboExpmestTemplate();
                this.InitComboUser();
                //this.InitComboPatientType(this.currentPatientTypeWithPatientTypeAlter);
                this.InitComboDayOfWeek(cboDayOfWeekForSearchServiceReqKidneyshift);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm. 2");
                this.InitComboCa(cboCaForSearchServiceReqKidneyshift);
                this.InitComboCa(cboCaForAdd);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm. 3");
                this.InitDefaultValueWeekCombo();
                this.InitComboMachine(cboMarchineForAdd, true);
                this.InitComboMachine(cboMarchineForSearchServiceReqKidneyshift, true);
                this.ValidateForm();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultValueWeekCombo()
        {
            try
            {
                //Mặc định set tuần hiện tại
                dateWeekFrom.EditValue = Inventec.Common.DateTime.Get.StartWeekSystemDateTime();
                dateWeekTo.EditValue = Inventec.Common.DateTime.Get.StartWeekSystemDateTime().Value.AddDays(6);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataWithCurrentWorking()
        {
            try
            {
                GlobalDatas.ExpMestTemplates = null;
                this.dicServices = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>()
                    .Where(t => t.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToDictionary(o => o.ID);
                //GlobalDatas globalDataWorker = new GlobalDatas();
                //globalDataWorker.GetTreatments();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCurrentTreatmentData(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.INTRUCTION_TIME = null;
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataWithTreatmentWithPatientTypeInfo()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypes != null)
                {
                    if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PATIENT_TYPE_CODE))
                    {
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentHisTreatment.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentHisTreatment.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentHisTreatment.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentHisTreatment.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentHisTreatment.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.LEVEL_CODE = this.currentHisTreatment.LEVEL_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentHisTreatment.RIGHT_ROUTE_CODE;
                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentHisTreatment.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        this.currentPatientTypeWithPatientTypeAlter = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Info("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisTreatment), currentHisTreatment));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServicePaty()
        {
            try
            {
                LogSystem.Debug("LoadServicePaty. 1");
                this.serviceTypeIdAllows = new long[11]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};
                var patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToArray();

                //Lọc các đối tượng thanh toán không có chính sách giá
                this.patientTypeIdAls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIds.Contains(t.PATIENT_TYPE_ID) && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => this.patientTypeIdAls.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();

                LogSystem.Debug("LoadServicePaty. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceChange(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            long intructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
            long treatmentTime = this.currentHisTreatment.IN_TIME;
            List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
            if (BranchDataWorker.HasServicePatyWithListPatientType(data.ID, this.patientTypeIdAls))
            {
                var arrPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[data.ID].Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_CODE).Distinct().ToList();
                dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);
            }

            this.InitComboPatientType(dataCombo);
            HIS_PATIENT_TYPE patientType = ChoosePatientTypeDefaultlService(data.ID);
            cboPatientType.EditValue = patientType != null ? (long?)patientType.ID : null;
        }

        /// <summary>
        /// Bổ sung: trong trường hợp đối tượng BN là BHYT và chưa đến ngày hiệu lực 
        /// hoặc đã hết hạn sử dụng (thời gian y lệnh ko nằm trong khoảng [từ ngày - đến ngày] của thẻ BHYT), 
        /// thì hiển thị đối tượng thanh toán mặc định là đối tượng viện phí
        /// Ngược lại xử lý như hiện tại: ưu tiên lấy theo đối tượng Bn trước, không có sẽ lấy mặc định theo đối tượng chấp nhận TT đầu tiên tìm thấy
        /// </summary>
        /// <param name="patientTypeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        private HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long serviceId, long? patientTypeAppointmentId = null)
        {
            long patientTypeId = this.currentHisPatientTypeAlter.PATIENT_TYPE_ID;
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    long intructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var currentPatientTypeTemps = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas != null && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (patientTypeAppointmentId.HasValue
                            && patientTypeAppointmentId.Value > 0
                            && currentPatientTypeTemps.Exists(e => e.ID == patientTypeAppointmentId.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeAppointmentId.Value);
                        }
                        else if (HisConfigCFG.IsSetPrimaryPatientType != "1"
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                        }
                        else if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0).Value.Date > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date || Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == HisConfigCFG.PatientTypeId__VP);
                        }
                        else
                        {
                            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                        }
                    }
                }
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
