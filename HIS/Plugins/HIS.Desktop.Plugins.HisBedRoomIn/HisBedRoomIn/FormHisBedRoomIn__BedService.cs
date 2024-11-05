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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.HisBedRoomIn.ADO;
using HIS.Desktop.Plugins.HisBedRoomIn.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBedRoomIn
{
    public partial class FormHisBedRoomIn : HIS.Desktop.Utility.FormBase
    {
        private List<V_HIS_BED_BSTY> hisBedBstys;
        private List<V_HIS_SERVICE> VHisBedServiceTypes;
        private List<V_HIS_SERVICE_ROOM> ListServiceBedByRooms;
        private List<long> patientTypeIdAls;
        private List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        private V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        private long[] serviceTypeIdAllows = new long[11]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
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
        private string commonString__true = "1";
        HIS_DEPARTMENT currentDepartment = new HIS_DEPARTMENT();
        HisTreatmentWithPatientTypeInfoSDO TreatmentWithPaTyInfo;
        private MOS.SDO.WorkPlaceSDO WorkPlaceSDO;

        private void VisibilityControl()
        {
            try
            {
                if (Config.IsPrimaryPatientType != "1" && Config.IsPrimaryPatientType != "2")
                {
                    LciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Height = this.Height - 26;
                }

                if (Config.IsUsingBedTemp == "1")
                {
                    CboBedService.Properties.Buttons[1].Visible = false;
                    CboPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboPatientType()
        {
            try
            {
                LoadComboEditor(CboPatientType, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME", "ID", null);
                LoadComboEditor(CboPrimaryPatientType, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME", "ID", null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboEditor(DevExpress.XtraEditors.GridLookUpEdit cboEditor, string valueCode, string valueName, string valueId, object data)
        {
            try
            {
                cboEditor.Properties.View.Columns.Clear();
                cboEditor.Properties.DataSource = data;
                cboEditor.Properties.DisplayMember = valueName;
                cboEditor.Properties.ValueMember = valueId;

                cboEditor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEditor.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.ForceInitialize();

                cboEditor.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboEditor.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                cboEditor.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboEditor.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboEditor.Properties.View.Columns.AddField(valueCode);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;
                aColumnCode.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboEditor.Properties.View.Columns.AddField(valueName);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
                aColumnName.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessThreadLoadData()
        {
            System.Threading.Thread bedData = new System.Threading.Thread(LoadDataBedService);
            System.Threading.Thread patientData = new System.Threading.Thread(LoadDataPatientType);
            System.Threading.Thread treatmentData = new System.Threading.Thread(ProcessLoadHistreatment);
            try
            {
                bedData.Start();
                patientData.Start();
                treatmentData.Start();

                bedData.Join();
                patientData.Join();
                treatmentData.Join();
            }
            catch (Exception ex)
            {
                bedData.Abort();
                patientData.Abort();
                treatmentData.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBedService()
        {
            try
            {
                this.hisBedBstys = BackendDataWorker.Get<V_HIS_BED_BSTY>().Where(o => o.IS_ACTIVE == 1).ToList();
                this.VHisBedServiceTypes = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatientType()
        {
            try
            {
                LoadCurrentPatientTypeAlter();
                PatientTypeWithPatientTypeAlter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter filter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = this.treatmentId;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                this.currentHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
                {
                    if (this.currentHisPatientTypeAlter != null)
                    {
                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                        {
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                        }
                    }
                }

                var patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToArray();

                //Lọc các đối tượng thanh toán không có chính sách giá
                this.patientTypeIdAls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIds.Contains(t.PATIENT_TYPE_ID) && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => this.patientTypeIdAls.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadHistreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = this.treatmentId;

                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPaTyInfo = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDefautBedServicePatient()
        {
            try
            {
                CboBedService.EditValue = null;
                CboPatientType.EditValue = null;
                CboPrimaryPatientType.EditValue = null;
                CboPrimaryPatientType.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrice()
        {
            try
            {
                LblPrice.Text = "0";

                if (CboPatientType.EditValue != null && CboBedService.EditValue != null)
                {
                    long instructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(long.Parse(CboBedService.EditValue.ToString()), this.patientTypeIdAls);
                    V_HIS_SERVICE_PATY data_ServicePrice = new V_HIS_SERVICE_PATY();
                    if (CboPrimaryPatientType.EditValue != null)
                    {
                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPrimaryPatientType.EditValue ?? 0).ToString()), null);
                    }
                    else
                    {
                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPatientType.EditValue ?? 0).ToString()), null);
                    }

                    if (data_ServicePrice != null)
                    {
                        LblPrice.Text = Inventec.Common.Number.Convert.NumberToString((data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO)), ConfigApplications.NumberSeperator);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboBedServiceType(ADO.HisBedADO row)
        {
            try
            {
                var currentServiceTypeByBeds = ProcessServiceRoom(row.ID);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentServiceTypeByBeds), currentServiceTypeByBeds));
                LoadComboEditor(CboBedService, "SERVICE_CODE", "SERVICE_NAME", "SERVICE_ID", currentServiceTypeByBeds);
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    currentServiceTypeByBeds = currentServiceTypeByBeds.OrderBy(p => p.SERVICE_CODE).ToList();
                    CboBedService.EditValue = currentServiceTypeByBeds[0].SERVICE_ID;
                }
                else
                {
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERVICE_ROOM> ProcessServiceRoom(long bedId)
        {
            List<V_HIS_SERVICE_ROOM> result = null;
            try
            {
                if (cboBedRoom.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn buồng");
                    return null;
                }
                CommonParam param = new CommonParam();

                var lstBedServiceTypes = hisBedBstys.Where(o => o.BED_ID == bedId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstBedServiceTypes = lstBedServiceTypes.Where(o => o.BED_ROOM_ID == long.Parse(cboBedRoom.EditValue.ToString())).ToList();

                List<long> bedServiceTypeIds = new List<long>();
                if (lstBedServiceTypes != null && lstBedServiceTypes.Count > 0)
                {
                    bedServiceTypeIds = lstBedServiceTypes.Select(p => p.BED_SERVICE_TYPE_ID).ToList();
                }

                //check lại dịch vụ lọc bỏ dịch vụ đã khóa nhưng cấu hình dv giường chưa khóa
                var lstBedServiceTypeByBedId = VHisBedServiceTypes.Where(p => bedServiceTypeIds.Contains(p.ID)).ToList();
                List<long> serviceIds = new List<long>();
                if (lstBedServiceTypeByBedId != null && lstBedServiceTypeByBedId.Count > 0)
                {
                    serviceIds = lstBedServiceTypeByBedId.Select(p => p.ID).ToList();
                }

                result = ListServiceBedByRooms.Where(p => serviceIds.Contains(p.SERVICE_ID)).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, HisBedADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    long intructionTime = dtLogTime.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0 : 0;
                    long treatmentTime = this.TreatmentWithPaTyInfo.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var currentPatientTypeTemps = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas != null && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (Config.IsPrimaryPatientType != commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Config.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                        }
                        else if (Config.IsPrimaryPatientType == "0"
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && patientTypeId != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                        }
                        else if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == Config.PatientTypeId__BHYT
                        && ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > (intructionTime - (intructionTime % 1000000))
                        || (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < (intructionTime - (intructionTime % 1000000))
                        ))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == Config.PatientTypeId__VP);
                        }
                        else
                        {
                            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                        }

                        if (result != null && sereServADO != null)
                        {
                            CboPatientType.EditValue = result.ID;
                        }
                        if (Config.IsPrimaryPatientType == "2")
                        {
                            if (this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID <= 0)
                            {
                                CboPrimaryPatientType.EditValue = null;
                            }
                            else
                            {
                                CboPrimaryPatientType.ReadOnly = true;
                                CboPrimaryPatientType.EditValue = this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID;
                                if (currentPatientTypeTemps.Exists(e => e.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID))
                                {
                                    var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID);
                                    CboPrimaryPatientType.EditValue = priPaty.ID;
                                }
                                else
                                {
                                    CboPrimaryPatientType.ReadOnly = false;
                                    try
                                    {
                                        var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID);
                                        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                        MessageBox.Show(string.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName));
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                    }
                                }
                            }
                        }
                        else if (Config.IsPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && result.ID != sereServADO.BILL_PATIENT_TYPE_ID.Value)
                        {
                            if (currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            {
                                var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                CboPrimaryPatientType.EditValue = priPaty.ID;
                            }
                            else
                            {
                                try
                                {
                                    var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                    MessageBox.Show(String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName));
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                        }
                        else if (Config.IsPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Config.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            CboPrimaryPatientType.EditValue = priPaty.ID;
                        }
                        else
                        {
                            CboPrimaryPatientType.EditValue = null;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO), sereServADO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HisBedADO> ProcessDataBedAdo(List<V_HIS_BED> datas)
        {
            List<HisBedADO> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedADO>();
                    result.AddRange((from r in datas select new HisBedADO(r)).ToList());
                 
                    long? timeFilter = null;
                    if (dtLogTime.EditValue != null && dtLogTime.DateTime != DateTime.MinValue)
                    {
                        timeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                    }

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();
                    MOS.Filter.HisBedLogFilter filter = new MOS.Filter.HisBedLogFilter();
                    filter.BED_IDs = bedIds;
                    if (timeFilter > 0 && timeFilter.HasValue)
                    {
                        filter.START_TIME_TO = timeFilter;
                        filter.FINISH_TIME_FROM__OR__NULL = timeFilter;
                    }
                    CommonParam param = new CommonParam();
                    var dataBedLogs = new BackendAdapter(param).Get<List<HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (dataBedLogs != null && dataBedLogs.Count > 0)
                    {
                        var dataBedLogGroups = dataBedLogs.GroupBy(p => p.BED_ID).Select(p => p.ToList()).ToList();
                        foreach (var itemADO in result)
                        {
                              
                            var dataByBedLogs = dataBedLogs.Where(p => p.BED_ID == itemADO.ID
                             //  && p.SERVICE_REQ_ID.HasValue
                                && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value > timeFilter))).ToList();
                            if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                            {
                                if (itemADO.MAX_CAPACITY.HasValue)
                                {
                                    if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                        itemADO.IsKey = 2;
                                    else
                                        itemADO.IsKey = 1;
                                }
                                else
                                    itemADO.IsKey = 1;
                                itemADO.AMOUNT = dataByBedLogs.Count;
                                itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                            }
                        }
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

        private void ReloadPrimaryPatientType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                if (CboBedService.EditValue != null && BranchDataWorker.HasServicePatyWithListPatientType(long.Parse((CboBedService.EditValue ?? 0).ToString()), this.patientTypeIdAls))
                {
                    long instructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                    long? intructionNumByType = 1;
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(long.Parse(CboBedService.EditValue.ToString()), this.patientTypeIdAls);
                    var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPatientType.EditValue ?? 0).ToString()), null, intructionNumByType);

                    var patyIds = servicePaties.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                    foreach (var item in patyIds)
                    {
                        if (item == long.Parse((CboPatientType.EditValue ?? 0).ToString()))
                            continue;
                        var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), item, null, intructionNumByType);
                        if (itemPaty == null || currentPaty == null || (currentPaty.PRICE * (1 + currentPaty.VAT_RATIO)) >= (itemPaty.PRICE * (1 + itemPaty.VAT_RATIO)))
                            continue;
                        dataCombo.Add(this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == item));
                    }
                }

                CboPrimaryPatientType.Properties.DataSource = dataCombo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadPatientType()
        {
            try
            {
                long intructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                long treatmentTime = this.TreatmentWithPaTyInfo.IN_TIME;
                long serviceId = long.Parse((CboBedService.EditValue ?? 0).ToString());
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
                if (BranchDataWorker.HasServicePatyWithListPatientType(serviceId, this.patientTypeIdAls))
                {
                    var arrPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[serviceId].Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_CODE).Distinct().ToList();
                    dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);
                }

                CboPatientType.Properties.DataSource = dataCombo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
