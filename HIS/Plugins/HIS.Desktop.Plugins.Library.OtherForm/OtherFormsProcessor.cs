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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.OtherForm
{
    public class OtherFormsProcessor
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        PatientADO currentPatient { get; set; }
        V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        HIS_DHST dhst { get; set; }
        HIS_TRAN_PATI_REASON tranPatiReason { get; set; }
        HIS_TRAN_PATI_FORM tranPatiForm { get; set; }
        V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        V_HIS_TREATMENT currentTreatmentUpdate { get; set; }
        V_HIS_TREATMENT_BED_ROOM treatmentbedRoom { get; set; }
        V_HIS_SERVICE_REQ serviceReq { get; set; }
        V_HIS_BABY baby { get; set; }

        Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, System.Drawing.Image>();
        internal CommonParam param = new CommonParam();

        public OtherFormsProcessor()
        {
            // InitializeComponent();
        }

        public OtherFormsProcessor(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
        {
            try
            {
                WaitingManager.Hide();
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                //  TaoBieuMauKhacClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RunProcess()
        {
            try
            {
                TaoBieuMauKhacClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBieuMauKhacClick()
        {
            try
            {
                dicParamPlus = new Dictionary<string, object>();
                SarPrintTypeFilter printTypeFilter = new SarPrintTypeFilter();
                printTypeFilter.PRINT_TYPE_CODE = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__TAO_BIEU_MAU_KHAC__MPS000133;
                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>("api/SarPrintType/Get", ApiConsumers.SarConsumer, printTypeFilter, param).FirstOrDefault();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                SetCommonKey.SetCommonSingleKey(dicParamPlus);
                //LoadDataToBMKProcess();
                CreateThreadLoadData(this.treatmentId);
                addDataToBieuMauKhac(dicParamPlus);
                AddKeyIntoDictionaryPrint<PatientADO>(currentPatient, dicParamPlus);
                //if (!String.IsNullOrEmpty(currentPatient.VIR_ADDRESS))
                //{
                //    dicParamPlus.Add("VIR_ADDRESS_STR", currentPatient.VIR_ADDRESS);
                //}
                //else
                //{
                //    dicParamPlus.Add("VIR_ADDRESS_STR", null);
                //}
                if (!String.IsNullOrEmpty(currentPatient.VIR_ADDRESS))
                {
                    dicParamPlus.Add("VIR_ADDRESS_STR", CalculateFullAge(currentPatient.DOB));
                }
                else
                {
                    dicParamPlus.Add("VIR_ADDRESS_STR", null);
                }
                if (currentPatient.DOB != null)
                {
                    dicParamPlus.Add("AGE_WITH_CAPTION", CalculateFullAge(currentPatient.DOB));
                }
                else
                {
                    dicParamPlus.Add("AGE_WITH_CAPTION", null);
                }

                Inventec.Common.Logging.LogSystem.Info("dhst: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dhst), dhst));

                AddKeyIntoDictionaryPrint<V_HIS_PATIENT_TYPE_ALTER>(PatyAlterBhyt, dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(serviceReq, dicParamPlus);
                
                AddKeyIntoDictionaryPrint<HIS_TRAN_PATI_REASON>(tranPatiReason, dicParamPlus);
                AddKeyIntoDictionaryPrint<HIS_TRAN_PATI_FORM>(tranPatiForm, dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(currentTreatmentUpdate, dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_DEPARTMENT_TRAN>(departmentTran, dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_BABY>(baby, dicParamPlus);

                AddKeyIntoDictionaryPrint<HIS_DHST>(dhst, dicParamPlus);

                if (currentTreatmentUpdate != null)
                {
                    dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(this.currentTreatmentUpdate.TDL_PATIENT_DOB, "", "", "", "", this.currentTreatmentUpdate.IN_TIME));
                    dicParamPlus.Add("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatmentUpdate.IN_TIME));
                }

                dicParamPlus.Add("USERNAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                dicParamPlus.Add("LOGINNAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                if (baby != null)
                {
                    if (!String.IsNullOrEmpty(baby.GENDER_NAME))
                    {
                        dicParamPlus.Add("BABY_GENDER_NAME", baby.GENDER_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("BABY_GENDER_NAME", null);
                    }
                    if (baby.WEIGHT.HasValue)
                    {
                        dicParamPlus.Add("BABY_WEIGHT", baby.WEIGHT);
                    }
                    else
                    {
                        dicParamPlus.Add("BABY_WEIGHT", null);
                    }

                    if (baby.BORN_TIME.HasValue)
                    {
                        dicParamPlus.Add("BORN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(baby.BORN_TIME.Value));
                        dicParamPlus.Add("AGE_BABY_WITH_CAPTION", CalculateFullAge(baby.BORN_TIME.Value));
                    }
                    else
                    {
                        dicParamPlus.Add("BORN_TIME_STR", null);
                        dicParamPlus.Add("AGE_BABY_WITH_CAPTION", null);
                    }
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatmentUpdate != null ? currentTreatmentUpdate.TREATMENT_CODE : ""), "Mps000133", currentModule != null ? currentModule.RoomId : 0);
                richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Biểu mẫu khác___", UpdateTreatmentJsonPrint, GetListPrintIdByTreatment, dicParamPlus, dicImagePlus, inputADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal string CalculateFullAge(long dob)
        {
            string result = System.String.Empty;
            try
            {
                string caption__Tuoi = Resources.ResourceMessage.Tuoi;
                string caption__ThangTuoi = Resources.ResourceMessage.ThangTuoi;
                string caption__NgayTuoi = Resources.ResourceMessage.NgayTuoi;
                string caption__GioTuoi = Resources.ResourceMessage.GioTuoi;

                if (dob > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        result = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay = diff__month.Ticks;
                        System.DateTime newDate = new System.DateTime(tongsogiay);
                        int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                        if (month == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            result = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                            if (month < 72)
                            {
                                result = (month + " " + caption__ThangTuoi);
                            }
                            //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                            else
                            {
                                int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                result = (year + " " + caption__Tuoi);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);

                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data));
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBMKProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                // Lấy thông tin bệnh nhân
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.treatmentId;
                currentTreatmentUpdate = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                currentPatient = PrintGlobalStore.getPatient(this.treatmentId);

                //Thông tin BHYT
                PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = this.treatmentId;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                PatyAlterBhyt = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);


                serviceReq = new V_HIS_SERVICE_REQ();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                serviceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                //Thong tin SereServ
                //if (examServiceReq != null)
                //{
                //    MOS.Filter.HisSereServViewFilter sereServFilter = new MOS.Filter.HisSereServViewFilter();
                //    sereServFilter.SERVICE_REQ_ID = examServiceReq.SERVICE_REQ_ID;
                //    sereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();
                //}

                //Dấu hiệu sinh tồn
                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = this.treatmentId;
                dhstFilter.ORDER_DIRECTION = "DESC";
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                
                dhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();

                //THông tin khoa
                MOS.Filter.HisDepartmentTranViewFilter filterDeparementTran = new HisDepartmentTranViewFilter();
                filterDeparementTran.TREATMENT_ID = treatmentId;
                filterDeparementTran.ORDER_DIRECTION = "DESC";
                filterDeparementTran.ORDER_FIELD = "DEPARTMENT_IN_TIME";
                departmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, filterDeparementTran, param).FirstOrDefault();

                //tretment_bed_room_name
                MOS.Filter.HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                bedRoomFilter.TREATMENT_ID = treatmentId;
                bedRoomFilter.ORDER_FIELD = "CREATE_TIME";
                bedRoomFilter.ORDER_DIRECTION = "DESC";
                treatmentbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                treatmentbedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addDataToBieuMauKhac(Dictionary<string, object> dicParamPlus)
        {
            try
            {
                MOS.Filter.HisTreatmentFilter treatmentfilter = new HisTreatmentFilter();
                treatmentfilter.ID = treatmentId;
                List<HIS_TREATMENT> lstTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentfilter, param);


                MOS.Filter.HisDepartmentTranViewFilter filterDeparementTran = new HisDepartmentTranViewFilter();
                filterDeparementTran.TREATMENT_ID = treatmentId;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> lstDepartmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, filterDeparementTran, param);
                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    
                    dicParamPlus.Add("OPEN_DATE_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstTreatment[0].IN_TIME));
                    dicParamPlus.Add("OPEN_TIME_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(lstTreatment[0].IN_TIME));
                    if (lstDepartmentTran[lstDepartmentTran.Count - 1] != null)
                    {
                        if (lstTreatment[lstTreatment.Count - 1].OUT_TIME.HasValue)
                        {
                            dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(lstTreatment[lstTreatment.Count - 1].OUT_TIME.Value));
                            dicParamPlus.Add("CLOSE_TIME_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(lstTreatment[lstTreatment.Count - 1].OUT_TIME.Value));
                        }
                        else
                        {
                            dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", null);
                        }

                        dicParamPlus.Add("DEPARTMENT_TRAN_CODE", lstDepartmentTran[lstDepartmentTran.Count - 1].DEPARTMENT_CODE);
                        dicParamPlus.Add("DEPARTMENT_TRAN_NAME", lstDepartmentTran[lstDepartmentTran.Count - 1].DEPARTMENT_NAME);


                        if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                        {
                            dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", PatyAlterBhyt.HEIN_CARD_NUMBER);
                            dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0));
                        }
                        else
                        {
                            dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", null);
                            dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", null);
                        }
                    }
                    else
                    {
                        dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", null);
                        dicParamPlus.Add("CLOSE_TIME_SEPARATE_STR", null);
                        dicParamPlus.Add("DEPARTMENT_TRAN_CODE", null);
                        dicParamPlus.Add("DEPARTMENT_TRAN_NAME", null);
                        dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", null);
                        dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", null);
                    }
                }
                else
                {
                    dicParamPlus.Add("OPEN_DATE_SEPARATE_STR", null);
                    dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", null);
                    dicParamPlus.Add("OPEN_TIME_SEPARATE_STR", null);
                    dicParamPlus.Add("CLOSE_TIME_SEPARATE_STR", null);
                    dicParamPlus.Add("DEPARTMENT_TRAN_CODE", null);
                    dicParamPlus.Add("DEPARTMENT_TRAN_NAME", null);
                    dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", null);
                    dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", null);
                    // dicParamPlus.Add("TOTAL_DAY", "");
                }


                if (currentTreatmentUpdate != null)
                {
                    if (!String.IsNullOrEmpty(currentTreatmentUpdate.ICD_NAME))
                    {
                        dicParamPlus.Add("ICD_END_MAIN_TEXT", currentTreatmentUpdate.ICD_CODE + "  -  " + currentTreatmentUpdate.ICD_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_END_MAIN_TEXT", null);
                    }

                    if (!String.IsNullOrEmpty(currentTreatmentUpdate.ICD_TEXT))
                    {
                        dicParamPlus.Add("ICD_END_TEXT", currentTreatmentUpdate.ICD_TEXT);
                        dicParamPlus.Add("ICD_CODE_ICD_TEXT", currentTreatmentUpdate.ICD_CODE + ",  " + currentTreatmentUpdate.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_END_TEXT", null);
                        dicParamPlus.Add("ICD_CODE_ICD_TEXT", null);
                    }
                    if (!String.IsNullOrEmpty(currentTreatmentUpdate.TDL_PATIENT_WORK_PLACE_NAME))
                    {
                        dicParamPlus.Add("WORK_PLACE_NAME_STR", currentTreatmentUpdate.TDL_PATIENT_WORK_PLACE_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("WORK_PLACE_NAME_STR", null);
                    }
                    if (currentTreatmentUpdate.OUT_TIME != null && currentTreatmentUpdate.OUT_TIME > 0)
                    {
                        long? ngayDieuTri = 0;
                        string PatienTypeCode_BHYT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                        long endTime = currentTreatmentUpdate.OUT_TIME ?? 0;
                        long inTime = currentTreatmentUpdate.IN_TIME;
                        if (this.PatyAlterBhyt != null && this.PatyAlterBhyt.PATIENT_TYPE_CODE == PatienTypeCode_BHYT)
                        {
                            ngayDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, endTime, currentTreatmentUpdate.TREATMENT_END_TYPE_ID, currentTreatmentUpdate.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                        }
                        else
                        {
                            ngayDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, endTime, currentTreatmentUpdate.TREATMENT_END_TYPE_ID, currentTreatmentUpdate.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                        }
                        dicParamPlus.Add("TOTAL_DAY", ngayDieuTri);
                    }
                    
                }
                if (treatmentbedRoom != null)
                {
                    dicParamPlus.Add("BED_ROOM_NAME_END", treatmentbedRoom.BED_ROOM_NAME);
                    dicParamPlus.Add("BED_NAME_END", treatmentbedRoom.BED_NAME);
                }
                else
                {
                    dicParamPlus.Add("BED_ROOM_NAME_END", null);
                    dicParamPlus.Add("BED_NAME_END", null);
                }
                if (serviceReq != null)
                {
                    if (!String.IsNullOrEmpty(serviceReq.ICD_TEXT))
                    {
                        dicParamPlus.Add("ICD_EXAM_TEXT", serviceReq.ICD_SUB_CODE + " _ " + serviceReq.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_EXAM_TEXT", null);
                    }
                    dicParamPlus.Add("ICD_EXAM_MAIN_TEXT", serviceReq.ICD_CODE + " _ " + serviceReq.ICD_NAME);

                    if (!String.IsNullOrEmpty(serviceReq.ICD_TEXT))
                    {
                        dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", serviceReq.TREATMENT_INSTRUCTION);
                    }
                    else
                    {
                        dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", null);
                    }
                }
                else
                {
                    dicParamPlus.Add("ICD_EXAM_TEXT", null);
                    dicParamPlus.Add("ICD_EXAM_MAIN_TEXT", null);
                    dicParamPlus.Add("REQUEST_DEPARTMENT_NAME", null);
                    dicParamPlus.Add("HOSPITALIZATION_REASON", null);
                    dicParamPlus.Add("PATHOLOGICAL_HISTORY", null);
                    dicParamPlus.Add("PATHOLOGICAL_HISTORY_FAMILY", null);
                    dicParamPlus.Add("FULL_EXAM", null);
                    dicParamPlus.Add("PART_EXAM", null);
                    dicParamPlus.Add("DESCRIPTION", null);
                    dicParamPlus.Add("PATHOLOGICAL_PROCESS", null);
                    dicParamPlus.Add("REQUEST_ROOM_NAME", null);
                    dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", null);
                }
                if (currentTreatmentUpdate != null)
                {
                    dicParamPlus.Add("ICD_RAVIEN_MAIN_TEXT", currentTreatmentUpdate.TRANSFER_IN_ICD_NAME);//currentTreatmentUpdate.TRANSFER_IN_ICD_CODE + " - " + 

                    //ReView
                    if (currentTreatmentUpdate.ICD_TEXT != null)
                    {
                        dicParamPlus.Add("ICD_RAVIEN_TEXT", currentTreatmentUpdate.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_RAVIEN_TEXT", null);
                    }
                    dicParamPlus.Add("ICD_MAIN_TEXT_NG", currentTreatmentUpdate.IN_ICD_CODE + " _ " + currentTreatmentUpdate.IN_ICD_NAME);

                    if (currentTreatmentUpdate.IN_ICD_TEXT != null)
                    {
                        dicParamPlus.Add("ICD_TEXT_IN", currentTreatmentUpdate.IN_ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_TEXT_IN", null);
                    }
                }
                else
                {
                    dicParamPlus.Add("ICD_RAVIEN_MAIN_TEXT", null);
                    dicParamPlus.Add("ICD_RAVIEN_TEXT", null);
                    dicParamPlus.Add("ICD_MAIN_TEXT_NG", null);
                    dicParamPlus.Add("ICD_TEXT_IN", null);
                }
                //ReView
                dicParamPlus.Add("CONCLUDE", null);
                dicParamPlus.Add("RESULT_NOTE", null);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool UpdateTreatmentJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current treatment
                bool valid = true;
                valid = valid && (this.currentTreatmentUpdate != null);
                if (valid)
                {
                    List<FileHolder> listFileHolder = new List<FileHolder>();
                    MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                    hisTreatment.ID = this.currentTreatmentUpdate.ID;
                    var listOldPrintIdOfTreatments = GetListPrintIdByTreatment();
                    ProcessTreatmentForUpdateJsonPrint(hisTreatment, listOldPrintIdOfTreatments, sarPrintCreated);
                    SaveTreatment(hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        List<long> GetListPrintIdByTreatment()
        {
            List<long> result = new List<long>();
            try
            {
                List<V_HIS_TREATMENT> treatments = new List<V_HIS_TREATMENT>();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.currentTreatmentUpdate.ID;
                var treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).SingleOrDefault();
                if (treatment != null)
                    treatments.Add(treatment);
                if (treatments != null && treatments.Count > 0)
                {
                    foreach (var item in treatments)
                    {
                        if (!String.IsNullOrEmpty(item.JSON_PRINT_ID))
                        {
                            var arrIds = item.JSON_PRINT_ID.Split(',', ';');
                            if (arrIds != null && arrIds.Length > 0)
                            {
                                foreach (var id in arrIds)
                                {
                                    long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                    if (printId > 0)
                                    {
                                        result.Add(printId);
                                    }
                                }
                            }
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

        private void ProcessTreatmentForUpdateJsonPrint(MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.currentTreatmentUpdate != null)
                {
                    if (jsonPrintId == null)
                    {
                        jsonPrintId = new List<long>();
                    }
                    jsonPrintId.Add(sarPrintCreated.ID);

                    string printIds = "";
                    foreach (var item in jsonPrintId)
                    {
                        printIds += item.ToString() + ",";
                    }

                    hisTreatment.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveTreatment(MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_UPDATE_JSON, ApiConsumers.MosConsumer, hisTreatment, param);
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                }

                #region Show message
                MessageManager.Show(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        ////


        /// <summary>
        /// Create Thread Load Data
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData(object param)
        {
            Thread threadTreatment = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataTreatmentNewThread));
            Thread threadServiceReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataPatientNewThread));
            Thread threadSereServ = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataPatyAlterBhytNewThread));
            Thread thread1 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataDepartmentNewThread));
            Thread threadBaby = new System.Threading.Thread(LoadDataBaby);

            //threadTreatment.Priority = ThreadPriority.Normal;
            //threadServiceReq.Priority = ThreadPriority.Normal;
            //threadSereServ.Priority = ThreadPriority.Highest;
            //thread1.Priority = ThreadPriority.Normal;
            try
            {
                threadSereServ.Start(param);
                threadTreatment.Start(param);
                threadServiceReq.Start(param);
                thread1.Start(param);
                threadBaby.Start();

                threadTreatment.Join();
                threadServiceReq.Join();
                threadSereServ.Join();
                thread1.Join();
                threadBaby.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadTreatment.Abort();
                threadServiceReq.Abort();
                threadSereServ.Abort();
                thread1.Abort();
            }
        }

        private void LoadDataTreatmentNewThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { LoadDataTreatmentWithTreatment((long)param); }));
                //}
                //else
                //{
                LoadDataTreatment((long)param);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(long data)
        {
            try
            {
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = data;
                currentTreatmentUpdate = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                MOS.Filter.HisTranPatiReasonFilter tranPatiReasonFilter = new MOS.Filter.HisTranPatiReasonFilter();
                if (currentTreatmentUpdate.TRAN_PATI_REASON_ID.HasValue)
                {
                    tranPatiReasonFilter.ID = currentTreatmentUpdate.TRAN_PATI_REASON_ID.Value;
                    tranPatiReason = new BackendAdapter(param).Get<List<HIS_TRAN_PATI_REASON>>(HisRequestUriStore.HIS_TRAN_PATI_REASON_GET, ApiConsumers.MosConsumer, tranPatiReasonFilter, param).FirstOrDefault();
                }

                MOS.Filter.HisTranPatiFormFilter tranPatiFormFilter = new MOS.Filter.HisTranPatiFormFilter();
                if (currentTreatmentUpdate.TRAN_PATI_FORM_ID.HasValue)
                {
                    tranPatiFormFilter.ID = currentTreatmentUpdate.TRAN_PATI_FORM_ID.Value;
                    tranPatiForm = new BackendAdapter(param).Get<List<HIS_TRAN_PATI_FORM>>(HisRequestUriStore.HIS_TRAN_PATI_FORM_GET, ApiConsumers.MosConsumer, tranPatiFormFilter, param).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatientNewThread(object param)
        {
            try
            {
                LoadDataPatient((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatient(long data)
        {
            try
            {
                currentPatient = PrintGlobalStore.getPatient(data);
                //Dấu hiệu sinh tồn
                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = data;
                dhstFilter.ORDER_DIRECTION = "DESC";
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                dhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatyAlterBhytNewThread(object param)
        {
            try
            {
                LoadDataPatyAlterBhyt((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatyAlterBhyt(long data)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = data;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                PatyAlterBhyt = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);


                serviceReq = new V_HIS_SERVICE_REQ();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.TREATMENT_ID = data;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                serviceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDepartmentNewThread(object param)
        {
            try
            {
                LoadDataDepartment((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDepartment(long data)
        {
            try
            {
                //THông tin khoa
                MOS.Filter.HisDepartmentTranViewFilter filterDeparementTran = new HisDepartmentTranViewFilter();
                filterDeparementTran.TREATMENT_ID = data;
                filterDeparementTran.ORDER_DIRECTION = "DESC";
                filterDeparementTran.ORDER_FIELD = "DEPARTMENT_IN_TIME";
                departmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, filterDeparementTran, param).FirstOrDefault();

                //tretment_bed_room_name
                MOS.Filter.HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                bedRoomFilter.TREATMENT_ID = data;
                bedRoomFilter.ORDER_FIELD = "CREATE_TIME";
                bedRoomFilter.ORDER_DIRECTION = "DESC";
                treatmentbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                treatmentbedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBaby()
        {
            //Thong tin tre so sinh
            MOS.Filter.HisBabyViewFilter babyFilter = new HisBabyViewFilter();
            babyFilter.TREATMENT_ID = this.treatmentId;
            baby = new BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, param).FirstOrDefault();
        }
    }
}
