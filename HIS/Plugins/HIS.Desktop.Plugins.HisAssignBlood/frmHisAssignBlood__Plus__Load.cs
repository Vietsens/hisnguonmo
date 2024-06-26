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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAssignBlood.ADO;
using HIS.Desktop.Plugins.HisAssignBlood.Config;
using HIS.UC.DateEditor.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmHisAssignBlood : HIS.Desktop.Utility.FormBase
    {
        private V_HIS_SERVICE_REQ LoadDataToCurrentServiceReqData(HIS_SERVICE_REQ serviceReq)
        {
            V_HIS_SERVICE_REQ serq = new V_HIS_SERVICE_REQ();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(serq, serviceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serq;
        }

        private MOS.EFMODEL.DataModels.V_HIS_TREATMENT LoadDataToCurrentTreatmentData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                filter.ID = treatmentId;

                var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (HisConfigCFG.IsUsingServerTime == commonString__true)
                {
                    filter.INTRUCTION_TIME = null;
                }
                else
                {
                    filter.INTRUCTION_TIME = intructionTime;
                }
                Inventec.Common.Logging.LogSystem.Info("Thoi gian chi dinh" + intructionTime);
                treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).FirstOrDefault();
                if (treatment != null && HisConfigCFG.IsUsingServerTime == commonString__true
                    && treatment.SERVER_TIME > 0)
                {
                    DateInputADO ip = new DateInputADO();
                    ip.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.SERVER_TIME).Value;
                    ip.Dates = new List<DateTime?>() { ip.Time.Date };
                    this.ucDateProcessor.SetValue(this.ucDate, ip);
                    this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private MOS.EFMODEL.DataModels.V_HIS_PATIENT LoadDataToCurrentPatientData(long patientId)
        {
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return patient;
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDeffautlService(long patientTypeId, long serviceId)
        {
            List<long> arrPatientTypeIds = new List<long>();
            // kiểm tra chính sách giá
            if (this.servicePatyInBranchs.ContainsKey(serviceId))
            {
                arrPatientTypeIds = this.servicePatyInBranchs[serviceId].Select(o => o.PATIENT_TYPE_ID).Distinct().OrderByDescending(p => p).ToList();
            }
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                if (this.currentPatientTypeWithPatientTypeAlter != null && this.currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    string inherits = string.Join(",", this.servicePatyInBranchs[serviceId].Select(o => o.INHERIT_PATIENT_TYPE_IDS));
                    var checkExistPatientType = this.currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeIds.Contains(o.ID) || BranchDataWorker.CheckPatientTypeInherit(inherits, new List<long> { o.ID }));
                    if (checkExistPatientType != null && checkExistPatientType.Count() > 0)
                    {
                        //ưu tiên đối tượng bệnh nhân
                        var checkPriority = checkExistPatientType.FirstOrDefault(o => o.ID == patientTypeId);
                        if (checkPriority != null)
                        {
                            result = checkPriority;
                        }
                        else
                        {
                            result = checkExistPatientType.FirstOrDefault();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitCurrentMesstRoom()
        {
            try
            {
                var mediIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => o.IS_BLOOD == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.ID).ToList();
                var roomIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Select(o => o.ID).ToList();
                this.currentMestRoomByRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o =>
                    o.ROOM_ID == GetWorkingRoomId()
                    && (mediIds != null && mediIds.Contains(o.MEDI_STOCK_ID))
                    && (roomIds != null && roomIds.Contains(o.ROOM_ID))
                    && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    ).ToList();

                var departmerts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => WorkPlace.GetBranchId() == 0 || o.BRANCH_ID == WorkPlace.GetBranchId() && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (departmerts == null || departmerts.Count == 0)
                    throw new ArgumentNullException("departmerts is null");

                var departmentIds = departmerts.Select(o => o.ID).Distinct().ToArray();
                this.currentMestRoomByRooms = this.currentMestRoomByRooms.Where(o => departmentIds.Contains(o.DEPARTMENT_ID)).ToList();
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
                // code cu
                //var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID);
                //this.servicePatyInBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                //    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
                //    .GroupBy(o => o.SERVICE_ID)
                //    .ToDictionary(o => o.Key, o => o.ToList());

                var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID);
                var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
                    .ToList();
                this.servicePatyInBranchs = servicePatyTemps
                    .GroupBy(o => o.SERVICE_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());
                //Lọc các đối tượng thanh toán không có chính sách giá
                var patientHasSetyIds = servicePatyTemps.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientHasSetyIds.Contains(o.ID)).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridBloodType(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK mediStock)
        {
            try
            {
                CommonParam param = new CommonParam();
                this.ListBloodTypeADOProcess = new List<BloodTypeADO>();
                gridControlBloodType__BloodPage.DataSource = null;
                if (mediStock != null)
                {
                    gridColumn1.VisibleIndex = -1;
                    gridColumn2.VisibleIndex = -1;
                    bloodGroups = new List<BloodADO>();
                    MOS.Filter.HisBloodViewFilter filter = new MOS.Filter.HisBloodViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.BLOOD_TYPE_IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                    filter.MEDI_STOCK_ID = mediStock.ID;
                    List<BloodADO> lstBloods = new BackendAdapter(param).Get<List<BloodADO>>(RequestUriStore.HIS_BLOOD__GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    if (lstBloods != null && lstBloods.Count > 0)
                    {
                        // lọc các máu không có chính sách giá
                        var servicePaties = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Select(o => o.SERVICE_ID).Distinct().ToList();
                        lstBloods = lstBloods.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && servicePaties.Contains(o.SERVICE_ID)).ToList();
                        IEnumerable<IGrouping<object, BloodADO>> bloodGroupByGroupType = null;
                        if (cboBloodLevel.SelectedIndex == 0 && lstBloods.Count > 0)
                        {
                            MOS.Filter.HisBloodTypeFilter blfilter = new MOS.Filter.HisBloodTypeFilter();
                            blfilter.IDs = lstBloods.Select(o => o.BLOOD_TYPE_ID).Distinct().ToList();
                            var lstBloodGroups = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>("api/HisBloodType/Get", ApiConsumers.MosConsumer, blfilter, ProcessLostToken, param);

                            lstBloods = lstBloods.Where(o=> o.BLOOD_ABO_ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_ABO.ID__O || lstBloodGroups.FirstOrDefault(p => p.ID == (o.BLOOD_TYPE_ID)).PREPARATIONS_BLOOD_ID == 1).ToList();
                        }
                        if (chkShowGroupBlood.Checked)
                        {
                            gridColumn1.VisibleIndex = 2;
                            gridColumn2.VisibleIndex = 3;
                            bloodGroupByGroupType = lstBloods.GroupBy(o => new { o.BLOOD_ABO_ID, o.BLOOD_RH_ID, o.BLOOD_TYPE_ID }, o => o);
                        }
                        else
                        {
                            bloodGroupByGroupType = lstBloods.GroupBy(o => new { o.BLOOD_TYPE_ID }, o => o);
                        }
                        foreach (var bgr in bloodGroupByGroupType)
                        {
                            BloodADO blood = bgr.ToList().First();
                            blood.AMOUNT = bgr.ToList().Count();
                            blood.SERVICE_NAME_HIDDEN = convertToUnSign3(blood.BLOOD_TYPE_NAME) + blood.BLOOD_TYPE_NAME;
                            blood.SERVICE_CODE_HIDDEN = convertToUnSign3(blood.BLOOD_TYPE_CODE) + blood.BLOOD_TYPE_CODE;
                            if (blood.AMOUNT > 0)
                                bloodGroups.Add(blood);
                        }

                        gridControlBloodType__BloodPage.DataSource = bloodGroups;
                    }

                    if (!String.IsNullOrEmpty(param.GetMessage()))
                        MessageManager.Show(param.GetMessage());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Khoi tao va do du lieu vao cac control dang combo/loolup/gridlookup,...
        /// </summary>
        private void FillDataToControlsForm()
        {
            try
            {
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                this.InitComboExecuteGroup();
                this.InitComboBloodRH();
                this.InitComboBloodABO();
                this.InitComboRespositoryBloodABO();
                this.InitComboRespositoryBloodRH();
                this.InitMediStock(null);
                this.InitComboUser();
                MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK ms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                this.LoadDataToGridBloodType(ms);
                this.LoadDataToTrackingCombo();
                this.SetEnableButtonControlBlood();
                this.InitComboRepositoryPatientType(null);
                this.InitBloodADO__RH__FromPatientInfo();
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("HisAssignBlood _ServiceReqEdit: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ServiceReqEdit), this._ServiceReqEdit));
                if (this._ServiceReqEdit != null && this._ServiceReqEdit.ID > 0)
                {
                    this.cboUser.EditValue = this._ServiceReqEdit.REQUEST_LOGINNAME;
                    this.txtLoginName.Text = this._ServiceReqEdit.REQUEST_LOGINNAME;
                }
                else
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList();
                    if (data != null)
                    {
                        this.cboUser.EditValue = data[0].LOGINNAME;
                        this.txtLoginName.Text = data[0].LOGINNAME;
                    }
                }

                //Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
                //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);
                this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
