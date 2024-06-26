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
using HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo
{
    public class DrugInterventionInfoProcessor
    {
        string Url;
        V_HIS_TREATMENT HisTreatment;
        const string ApiCheck = "/api/HIS/CheckDrug";
        const string ApiConfirmation = "/InteractionCheck/InteractionCheckConfirmation?sessionId={0}";

        public DrugInterventionInfoProcessor(string url, V_HIS_TREATMENT treatment)
        {
            this.Url = url;
            this.HisTreatment = treatment;
        }

        public void ReleasePrescription(InPatientPresResultSDO InPrescriptionResultSDO, OutPatientPresResultSDO OutPrescriptionResultSDO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DrugInterventionInfo.ReleasePrescription");
                if (InPrescriptionResultSDO != null || OutPrescriptionResultSDO != null)
                {
                    List<HIS_SERVICE_REQ> serviceReqs = new List<HIS_SERVICE_REQ>();
                    List<HIS_EXP_MEST_MATERIAL> Materials = new List<HIS_EXP_MEST_MATERIAL>();
                    List<HIS_EXP_MEST_MEDICINE> Medicines = new List<HIS_EXP_MEST_MEDICINE>();
                    List<HIS_SERVICE_REQ_MATY> ServiceReqMaties = new List<HIS_SERVICE_REQ_MATY>();
                    List<HIS_SERVICE_REQ_METY> ServiceReqMeties = new List<HIS_SERVICE_REQ_METY>();

                    #region inputData
                    if (InPrescriptionResultSDO != null)
                    {
                        if (InPrescriptionResultSDO.ServiceReqs != null && InPrescriptionResultSDO.ServiceReqs.Count > 0)
                        {
                            serviceReqs.AddRange(InPrescriptionResultSDO.ServiceReqs);
                        }

                        if (InPrescriptionResultSDO.Materials != null && InPrescriptionResultSDO.Materials.Count > 0)
                        {
                            Materials.AddRange(InPrescriptionResultSDO.Materials);
                        }

                        if (InPrescriptionResultSDO.Medicines != null && InPrescriptionResultSDO.Medicines.Count > 0)
                        {
                            Medicines.AddRange(InPrescriptionResultSDO.Medicines);
                        }

                        if (InPrescriptionResultSDO.ServiceReqMaties != null && InPrescriptionResultSDO.ServiceReqMaties.Count > 0)
                        {
                            ServiceReqMaties.AddRange(InPrescriptionResultSDO.ServiceReqMaties);
                        }

                        if (InPrescriptionResultSDO.ServiceReqMeties != null && InPrescriptionResultSDO.ServiceReqMeties.Count > 0)
                        {
                            ServiceReqMeties.AddRange(InPrescriptionResultSDO.ServiceReqMeties);
                        }
                    }

                    if (OutPrescriptionResultSDO != null)
                    {
                        if (OutPrescriptionResultSDO.ServiceReqs != null && OutPrescriptionResultSDO.ServiceReqs.Count > 0)
                        {
                            serviceReqs.AddRange(OutPrescriptionResultSDO.ServiceReqs);
                        }

                        if (OutPrescriptionResultSDO.Materials != null && OutPrescriptionResultSDO.Materials.Count > 0)
                        {
                            Materials.AddRange(OutPrescriptionResultSDO.Materials);
                        }

                        if (OutPrescriptionResultSDO.Medicines != null && OutPrescriptionResultSDO.Medicines.Count > 0)
                        {
                            Medicines.AddRange(OutPrescriptionResultSDO.Medicines);
                        }

                        if (OutPrescriptionResultSDO.ServiceReqMaties != null && OutPrescriptionResultSDO.ServiceReqMaties.Count > 0)
                        {
                            ServiceReqMaties.AddRange(OutPrescriptionResultSDO.ServiceReqMaties);
                        }

                        if (OutPrescriptionResultSDO.ServiceReqMeties != null && OutPrescriptionResultSDO.ServiceReqMeties.Count > 0)
                        {
                            ServiceReqMeties.AddRange(OutPrescriptionResultSDO.ServiceReqMeties);
                        }
                    }
                    #endregion

                    foreach (var serviceReq in serviceReqs)
                    {
                        List<MediMateCheckADO> sendData = new List<MediMateCheckADO>();

                        #region create MediMateCheckADO
                        List<HIS_EXP_MEST_MATERIAL> lstMaterial = Materials.Where(o => o.TDL_SERVICE_REQ_ID == serviceReq.ID).ToList();
                        if (lstMaterial != null && lstMaterial.Count > 0)
                        {
                            foreach (var item in lstMaterial)
                            {
                                MediMateCheckADO mate = new MediMateCheckADO();
                                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MATERIAL_TYPE_ID);
                                if (maty != null)
                                {
                                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMateCheckADO>(mate, maty);
                                    mate.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                                    mate.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                                }

                                mate.AMOUNT = item.AMOUNT;
                                mate.NUM_ORDER = item.NUM_ORDER;

                                sendData.Add(mate);
                            }
                        }

                        List<HIS_EXP_MEST_MEDICINE> lstMedicine = Medicines.Where(o => o.TDL_SERVICE_REQ_ID == serviceReq.ID).ToList();
                        if (lstMedicine != null && lstMedicine.Count > 0)
                        {
                            foreach (var item in lstMedicine)
                            {
                                MediMateCheckADO medi = new MediMateCheckADO();

                                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID);
                                if (mety != null)
                                {
                                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMateCheckADO>(medi, mety);
                                }

                                medi.AMOUNT = item.AMOUNT;
                                medi.NUM_ORDER = item.NUM_ORDER;
                                medi.Sang = item.MORNING;
                                medi.Trua = item.NOON;
                                medi.Chieu = item.AFTERNOON;
                                medi.Toi = item.EVENING;

                                sendData.Add(medi);
                            }
                        }

                        List<HIS_SERVICE_REQ_MATY> lstReqMaty = ServiceReqMaties.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();
                        if (lstReqMaty != null && lstReqMaty.Count > 0)
                        {
                            foreach (var item in lstReqMaty)
                            {
                                MediMateCheckADO reqMaty = new MediMateCheckADO();
                                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                                if (maty != null)
                                {
                                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMateCheckADO>(reqMaty, maty);
                                    reqMaty.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                                    reqMaty.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                                }
                                else
                                {
                                    reqMaty.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                    reqMaty.SERVICE_UNIT_NAME = item.UNIT_NAME;
                                }

                                reqMaty.AMOUNT = item.AMOUNT;
                                reqMaty.NUM_ORDER = item.NUM_ORDER;

                                sendData.Add(reqMaty);
                            }
                        }

                        List<HIS_SERVICE_REQ_METY> lstReqMety = ServiceReqMeties.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();
                        if (lstReqMety != null && lstReqMety.Count > 0)
                        {
                            foreach (var item in lstReqMety)
                            {
                                MediMateCheckADO reqMety = new MediMateCheckADO();

                                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                                if (mety != null)
                                {
                                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMateCheckADO>(reqMety, mety);
                                }
                                else
                                {
                                    reqMety.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                    reqMety.SERVICE_UNIT_NAME = item.UNIT_NAME;

                                    var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == item.MEDICINE_USE_FORM_ID);
                                    if (useForm != null)
                                    {
                                        reqMety.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                                    }
                                }

                                reqMety.AMOUNT = item.AMOUNT;
                                reqMety.NUM_ORDER = item.NUM_ORDER;
                                reqMety.Sang = item.MORNING;
                                reqMety.Trua = item.NOON;
                                reqMety.Chieu = item.AFTERNOON;
                                reqMety.Toi = item.EVENING;

                                sendData.Add(reqMety);
                            }
                        }
                        #endregion

                        if (sendData.Count <= 0)
                            continue;

                        InputCheckDataADO checkData = new InputCheckDataADO();
                        checkData.HisServiceReq = serviceReq;
                        checkData.ListMediMateCheck = sendData;
                        this.SendPrescription(checkData, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public OutputResultADO CheckPrescription(InputCheckDataADO data)
        {
            OutputResultADO result = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DrugInterventionInfo.CheckPrescription");
                result = SendPrescription(data, true);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private OutputResultADO SendPrescription(InputCheckDataADO data, bool isTemporary)
        {
            OutputResultADO result = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(this.Url) && data != null && data.HisServiceReq != null && data.ListMediMateCheck != null && data.ListMediMateCheck.Count > 0)
                {
                    ValidationRequestADO ado = new ValidationRequestADO();
                    ado.isTemporary = isTemporary;
                    ado.sessionID = SessionIdGenerate.SessionId;
                    ado.prescriptionInfo = new DrugPatientInfoADO();
                    ado.prescriptionInfo.info = ProcessPatientInfo(data.HisServiceReq, isTemporary);
                    ado.prescriptionInfo.drugList = ProcessDetail(data.ListMediMateCheck);

                    var apiresult = ApiConsumer.CreateRequest<ValidationResponseADO>(this.Url, ApiCheck, ado);
                    if (apiresult != null)
                    {
                        if (isTemporary && apiresult.needDoctorConfirmation)
                        {
                            string fullViewUrl = "";
                            Uri uri = null;
                            if (Uri.TryCreate(new Uri(this.Url), string.Format(ApiConfirmation, apiresult.prescriptionID), out uri))
                                fullViewUrl = uri.ToString();

                            if (!String.IsNullOrWhiteSpace(fullViewUrl))
                            {
                                ViewForm.FormBrowser form = new ViewForm.FormBrowser(fullViewUrl);
                                form.ShowDialog();
                                apiresult = ApiConsumer.CreateRequest<ValidationResponseADO>(this.Url, ApiCheck, ado);
                                if (apiresult == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("needDoctorConfirmation reCheck false");
                                    return null;
                                }
                            }
                        }

                        result = new OutputResultADO();

                        List<string> message = new List<string>();
                        if (apiresult.issueCategories != null && apiresult.issueCategories.Count > 0)
                        {
                            foreach (var categorie in apiresult.issueCategories)
                            {
                                if (!String.IsNullOrWhiteSpace(categorie.CategoryName))
                                {
                                    message.Add(categorie.CategoryName);
                                }
                            }
                        }

                        result.Message = String.Join("; ", message.Distinct());
                        switch (apiresult.severity)
                        {
                            case DrugEnum.ValidationSeverityLevel.Normal:
                                result.AlertLevel = AlertSeverityLevel.Normal;
                                break;
                            case DrugEnum.ValidationSeverityLevel.Warning:
                                result.AlertLevel = AlertSeverityLevel.Warning;
                                result.Message += " Bạn có muốn tiếp tục?";
                                break;
                            case DrugEnum.ValidationSeverityLevel.Major:
                                result.AlertLevel = AlertSeverityLevel.Warning;
                                result.Message += " Bạn có muốn tiếp tục?";
                                break;
                            case DrugEnum.ValidationSeverityLevel.Contraindicated:
                                result.AlertLevel = AlertSeverityLevel.Contraindicated;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Input Data", data));
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

        private PatientInfoADO ProcessPatientInfo(HIS_SERVICE_REQ serviceReq, bool isTemporary)
        {
            PatientInfoADO result = new PatientInfoADO();
            try
            {
                if (this.HisTreatment != null)
                {
                    result.patientID = this.HisTreatment.TDL_PATIENT_CODE;
                    result.maYTe = this.HisTreatment.TREATMENT_CODE;
                    result.name = this.HisTreatment.TDL_PATIENT_NAME;
                    result.sex = this.HisTreatment.TDL_PATIENT_GENDER_NAME;
                    if (this.HisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        result.serviceType = DrugEnum.EServiceType.Inpatient;
                    }
                    else if (this.HisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU ||
                        this.HisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        result.serviceType = DrugEnum.EServiceType.Outpatient;
                    }
                    else
                    {
                        result.serviceType = DrugEnum.EServiceType.Undefined;
                    }

                    result.dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.HisTreatment.TDL_PATIENT_DOB);

                    result.maDoiTuong = "Không xác định";
                    HIS_PATIENT_TYPE patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.HisTreatment.TDL_PATIENT_TYPE_ID);
                    if (patientType != null)
                    {
                        result.maDoiTuong = patientType.PATIENT_TYPE_NAME;
                    }
                }

                if (serviceReq != null)
                {
                    result.examinationDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME);
                    result.prescriptionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME);

                    List<string> icds = new List<string>();
                    if (!String.IsNullOrWhiteSpace(serviceReq.ICD_CODE))
                    {
                        icds.Add(serviceReq.ICD_CODE);
                    }

                    if (!String.IsNullOrWhiteSpace(serviceReq.ICD_SUB_CODE))
                    {
                        icds.AddRange(serviceReq.ICD_SUB_CODE.Trim(';').Split(';'));
                    }

                    result.icd = string.Join(",", icds.Distinct());

                    HIS_DEPARTMENT department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        result.maKhoa = department.DEPARTMENT_CODE;
                        result.tenKhoa = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        result.phongKham = room.ROOM_NAME;
                    }

                    result.prescriber = new ValueItemADO();
                    if (!String.IsNullOrWhiteSpace(serviceReq.REQUEST_LOGINNAME))
                    {
                        result.prescriber.id = serviceReq.REQUEST_LOGINNAME;
                        result.prescriber.value = serviceReq.REQUEST_USERNAME;
                    }
                    else
                    {
                        result.prescriber.id = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        result.prescriber.value = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }

                    result.prescriptionId = serviceReq.SERVICE_REQ_CODE;
                    //đang kê sẽ không gửi id lên
                    if (isTemporary)
                    {
                        result.prescriptionId = null;
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

        private List<DrugInfoADO> ProcessDetail(List<MediMateCheckADO> listData)
        {
            List<DrugInfoADO> result = null;
            try
            {
                if (listData != null && listData.Count > 0)
                {
                    listData = listData.OrderBy(o => o.NUM_ORDER).ToList();
                    result = new List<DrugInfoADO>();
                    int index = 0;
                    foreach (var data in listData)
                    {
                        DrugInfoADO ado = new DrugInfoADO();
                        ado.index = index;

                        ado.medicationName = data.MEDICINE_TYPE_NAME;
                        if (!String.IsNullOrWhiteSpace(data.MATERIAL_TYPE_MAP_NAME))
                        {
                            ado.medicationName = data.MATERIAL_TYPE_MAP_NAME;
                        }

                        ado.totalQuantity = data.AMOUNT;

                        CheckTutorial(data.Sang, data.Trua, data.Chieu, data.Toi, ref ado);

                        ado.usageUnit = data.SERVICE_UNIT_NAME;
                        ado.packageUnit = data.SERVICE_UNIT_NAME;
                        ado.drugRoute = data.MEDICINE_USE_FORM_NAME;
                        //ado.prescribeDate = null;
                        ado.note = "";

                        index++;
                        result.Add(ado);
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

        private void CheckTutorial(string sang, string trua, string chieu, string toi, ref DrugInfoADO ado)
        {
            try
            {
                int countday = 0;
                decimal maxAmount = 0;
                if (!String.IsNullOrWhiteSpace(sang))
                {
                    countday++;
                    decimal dSang = ProcessNumberInterger(sang);
                    if (dSang > maxAmount)
                    {
                        maxAmount = dSang;
                    }
                }

                if (!String.IsNullOrWhiteSpace(trua))
                {
                    countday++;
                    decimal dTrua = ProcessNumberInterger(trua);
                    if (dTrua > maxAmount)
                    {
                        maxAmount = dTrua;
                    }
                }

                if (!String.IsNullOrWhiteSpace(chieu))
                {
                    countday++;
                    decimal dChieu = ProcessNumberInterger(chieu);
                    if (dChieu > maxAmount)
                    {
                        maxAmount = dChieu;
                    }
                }

                if (!String.IsNullOrWhiteSpace(toi))
                {
                    countday++;
                    decimal dToi = ProcessNumberInterger(toi);
                    if (dToi > maxAmount)
                    {
                        maxAmount = dToi;
                    }
                }

                if (countday > 0)
                {
                    ado.timePerDay = countday;
                }

                if (maxAmount > 0)
                {
                    ado.quantityPerTime = maxAmount;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal ProcessNumberInterger(string sDecimal)
        {
            decimal rs = 0;
            try
            {
                if (String.IsNullOrEmpty(sDecimal) || String.IsNullOrWhiteSpace(sDecimal) || sDecimal.Contains("/"))
                {
                    return 0;
                }

                sDecimal = sDecimal.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                rs = Inventec.Common.TypeConvert.Parse.ToDecimal(sDecimal);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sDecimal), sDecimal)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return rs;
        }
    }
}
