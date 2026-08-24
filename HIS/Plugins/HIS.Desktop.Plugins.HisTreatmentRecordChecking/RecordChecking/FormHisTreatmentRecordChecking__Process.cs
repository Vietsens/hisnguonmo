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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Data processing: build ADO list, fill patient panel, load document types.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        /// <summary>
        /// Fills the record creation time (task 53180 - QT-05).
        /// Kept separate from CREATE_TIME_STR, which holds the business time and therefore
        /// differs per document type (order time, consultation time, infusion time...).
        /// </summary>
        private void SetCreateTime(InfoRecordADO ado, long? createTime)
        {
            try
            {
                if (ado == null) return;
                ado.CREATE_TIME = createTime;
                ado.CREATE_TIME_REAL_STR =
                    Inventec.Common.DateTime.Convert.TimeNumberToTimeString(createTime ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan ma ho so va thong tin benh nhan cho cac dong tu indexFrom tro di.
        /// </summary>
        private void StampTreatmentInfo(int indexFrom, HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment == null || ListDataInfoRecord == null) return;

                for (int i = indexFrom; i < ListDataInfoRecord.Count; i++)
                {
                    ListDataInfoRecord[i].TREATMENT_ID = treatment.ID;
                    ListDataInfoRecord[i].TREATMENT_CODE = treatment.TREATMENT_CODE;
                    ListDataInfoRecord[i].PATIENT_CODE = treatment.TDL_PATIENT_CODE;
                    ListDataInfoRecord[i].PATIENT_NAME = treatment.TDL_PATIENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Lookup dung chung cho MOT lan tim

        /// <summary>Danh muc khoa, dung 1 lan cho ca lan tim.</summary>
        private Dictionary<long, HIS_DEPARTMENT> dicDepartmentCache;
        /// <summary>Danh muc loai yeu cau dich vu, dung 1 lan cho ca lan tim.</summary>
        private Dictionary<long, HIS_SERVICE_REQ_TYPE> dicServiceReqTypeCache;
        /// <summary>Thuoc cua cac ban ghi test thuoc, dung 1 lan cho ca lan tim.</summary>
        private Dictionary<long, V_HIS_MEDICINE> dicMedicineCache;

        /// <summary>
        /// Dung san 3 bang tra cuu cho ca lan tim, TRUOC khi goi ProcessDataADO.
        /// Cach 1 truyen 1 ho so, Cach 2 truyen ca trang - nho vay thuoc chi goi API 1 lot
        /// thay vi moi ho so mot lot, va 2 danh muc chi duyet 1 lan thay vi moi ho so mot lan.
        /// </summary>
        private void PrepareLookupData(List<HisTreatmentForRecordCheckingSDO> listSdo)
        {
            try
            {
                dicDepartmentCache = BuildDepartmentLookup();
                dicServiceReqTypeCache = BuildServiceReqTypeLookup();
                dicMedicineCache = BuildMedicineLookup(listSdo);
            }
            catch (Exception ex)
            {
                dicDepartmentCache = new Dictionary<long, HIS_DEPARTMENT>();
                dicServiceReqTypeCache = new Dictionary<long, HIS_SERVICE_REQ_TYPE>();
                dicMedicineCache = new Dictionary<long, V_HIS_MEDICINE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gom MEDICINE_ID cua tat ca ban ghi test thuoc roi lay thuoc trong MOT lot goi API.
        /// Bo qua ban ghi khong co MEDICINE_ID: cot nay nullable, truy cap .Value truc tiep se nem loi
        /// va lam mat toan bo dong cua ho so do (ProcessDataADO bat exception o ngoai cung).
        /// </summary>
        private Dictionary<long, V_HIS_MEDICINE> BuildMedicineLookup(List<HisTreatmentForRecordCheckingSDO> listSdo)
        {
            Dictionary<long, V_HIS_MEDICINE> result = new Dictionary<long, V_HIS_MEDICINE>();
            try
            {
                if (listSdo == null || listSdo.Count == 0) return result;

                HashSet<long> medicineIds = new HashSet<long>();
                foreach (var sdo in listSdo)
                {
                    if (sdo == null || sdo.MediReacts == null) continue;
                    foreach (var mediReact in sdo.MediReacts)
                    {
                        if (mediReact.MEDICINE_ID.HasValue) medicineIds.Add(mediReact.MEDICINE_ID.Value);
                    }
                }
                if (medicineIds.Count == 0) return result;

                var medicines = GetMedicineById(medicineIds.ToList());
                if (medicines != null)
                {
                    foreach (var medicine in medicines)
                    {
                        if (!result.ContainsKey(medicine.ID)) result.Add(medicine.ID, medicine);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<long, V_HIS_MEDICINE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Giai phong 3 bang tra cuu khi dong form. dicMedicineCache co the giu nhieu ban ghi
        /// khi loc theo bac si tren pham vi rong.
        /// </summary>
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                dicDepartmentCache = null;
                dicServiceReqTypeCache = null;
                dicMedicineCache = null;
                ListDataInfoRecord = null;
                CurrentInfoRecord = null;
                ListDocument = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Cach 1: xu ly SDO cua ho so dang mo.
        /// </summary>
        private void ProcessDataADO()
        {
            PrepareLookupData(new List<HisTreatmentForRecordCheckingSDO> { CurrentTreatment });
            ProcessDataADO(CurrentTreatment);
        }

        /// <summary>
        /// Chuyen 1 SDO tra soat thanh cac dong InfoRecordADO va noi vao ListDataInfoRecord.
        /// Dung chung cho ca 2 cach: Cach 2 goi lan luot cho tung ho so tra ve.
        /// </summary>
        private void ProcessDataADO(HisTreatmentForRecordCheckingSDO CurrentTreatment)
        {
            try
            {
                if (CurrentTreatment != null)
                {
                    int indexFrom = ListDataInfoRecord.Count;

                    // 3 bang tra cuu dung san 1 lan cho ca lan tim - xem PrepareLookupData().
                    // Guard: neu ai do goi thang ham nay ma quen dung san thi dung cho rieng ho so nay.
                    if (dicDepartmentCache == null || dicServiceReqTypeCache == null || dicMedicineCache == null)
                    {
                        PrepareLookupData(new List<HisTreatmentForRecordCheckingSDO> { CurrentTreatment });
                    }
                    Dictionary<long, HIS_DEPARTMENT> dicDepartment = dicDepartmentCache;
                    Dictionary<long, HIS_SERVICE_REQ_TYPE> dicServiceReqType = dicServiceReqTypeCache;

                    if (CurrentTreatment.Cares != null && CurrentTreatment.Cares.Count > 0)
                    {
                        foreach (var care in CurrentTreatment.Cares)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__CARE;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(care.CREATE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_CARE:" + care.ID;
                            ado.TYPE = "";
                            HIS_DEPARTMENT department = GetDepartment(dicDepartment, care.EXECUTE_DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = care.CREATOR;
                            SetCreateTime(ado, care.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Debates != null && CurrentTreatment.Debates.Count > 0)
                    {
                        foreach (var debate in CurrentTreatment.Debates)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(debate.DEBATE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_DEBATE:" + debate.ID;
                            ado.TYPE = "";
                            HIS_DEPARTMENT department = GetDepartment(dicDepartment, debate.DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = debate.CREATOR;
                            SetCreateTime(ado, debate.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Infusions != null && CurrentTreatment.Infusions.Count > 0)
                    {
                        foreach (var infusions in CurrentTreatment.Infusions)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = string.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(infusions.START_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(infusions.FINISH_TIME ?? 0));
                            ado.SEARCH_CODE = "HIS_INFUSION:" + infusions.ID;
                            ado.TYPE = "";
                            ado.DEPARTMENT_NAME = infusions.MEDICINE_TYPE_NAME;
                            ado.CREATOR = infusions.CREATOR;
                            SetCreateTime(ado, infusions.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.MediReacts != null && CurrentTreatment.MediReacts.Count > 0)
                    {
                        foreach (var mediReact in CurrentTreatment.MediReacts)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(mediReact.EXECUTE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_MEDI_REACT:" + mediReact.ID;
                            ado.TYPE = "";
                            V_HIS_MEDICINE medicine = null;
                            if (dicMedicineCache != null && mediReact.MEDICINE_ID.HasValue)
                            {
                                dicMedicineCache.TryGetValue(mediReact.MEDICINE_ID.Value, out medicine);
                            }
                            if (medicine != null)
                            {
                                ado.DEPARTMENT_NAME = medicine.MEDICINE_TYPE_NAME;
                            }
                            ado.CREATOR = mediReact.CREATOR;
                            SetCreateTime(ado, mediReact.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.ServiceReqs != null && CurrentTreatment.ServiceReqs.Count > 0)
                    {
                        foreach (var req in CurrentTreatment.ServiceReqs)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            if (ReqTypeId.Contains(req.SERVICE_REQ_TYPE_ID))
                            {
                                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION;
                            }
                            else
                            {
                                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                            }

                            ado.CODE = req.SERVICE_REQ_CODE;
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(req.INTRUCTION_TIME);
                            ado.SEARCH_CODE = "SERVICE_REQ_CODE:" + req.SERVICE_REQ_CODE;

                            ado.TYPE = "";
                            HIS_SERVICE_REQ_TYPE type = null;
                            if (dicServiceReqType != null)
                            {
                                dicServiceReqType.TryGetValue(req.SERVICE_REQ_TYPE_ID, out type);
                            }
                            if (type != null)
                            {
                                ado.TYPE = type.SERVICE_REQ_TYPE_NAME;
                            }

                            ado.DEPARTMENT_NAME = "";
                            HIS_DEPARTMENT department = GetDepartment(dicDepartment, req.REQUEST_DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = req.REQUEST_LOGINNAME;
                            SetCreateTime(ado, req.CREATE_TIME);
                            ado.REQ_TYPE_STT_ID = req.SERVICE_REQ_STT_ID;

                            ListDataInfoRecord.Add(ado);

                            if (ado.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN)
                            {
                                InfoRecordADO ado1 = new InfoRecordADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<InfoRecordADO>(ado1, ado);
                                ado1.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                                ListDataInfoRecord.Add(ado1);
                            }
                        }
                    }

                    if (CurrentTreatment.Trackings != null && CurrentTreatment.Trackings.Count > 0)
                    {
                        foreach (var tracking in CurrentTreatment.Trackings)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tracking.TRACKING_TIME);
                            ado.SEARCH_CODE = "HIS_TRACKING:" + tracking.ID;
                            ado.TYPE = "";
                            HIS_DEPARTMENT department = GetDepartment(dicDepartment, tracking.DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = tracking.CREATOR;
                            SetCreateTime(ado, tracking.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Transfusions != null && CurrentTreatment.Transfusions.Count > 0)
                    {
                        foreach (var tranfusion in CurrentTreatment.Transfusions)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRANSFUSION;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tranfusion.MEASURE_TIME);
                            ado.SEARCH_CODE = "HIS_TRANSFUSION:" + tranfusion.ID;
                            ado.TYPE = "";
                            //var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == tranfusion.DEPARTMENT_ID);
                            //if (department != null)
                            //{
                            //    ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            //}
                            ado.CREATOR = tranfusion.CREATOR;
                            SetCreateTime(ado, tranfusion.CREATE_TIME);
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    // Gan thong tin ho so / benh nhan cho cac dong vua sinh.
                    // Cach 1 khong dung (chi 1 ho so), Cach 2 dung de hien 3 cot Ma BN / Ten BN / Ma ho so.
                    StampTreatmentInfo(indexFrom, CurrentTreatment.Treatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_MEDICINE> GetMedicineById(List<long> medicineId)
        {
            List<V_HIS_MEDICINE> result = new List<V_HIS_MEDICINE>();
            try
            {
                if (medicineId != null && medicineId.Count > 0)
                {
                    int skip = 0;
                    while (medicineId.Count - skip > 0)
                    {
                        var listId = medicineId.Skip(skip).Take(500).ToList();
                        skip += 500;

                        CommonParam param = new CommonParam();
                        HisMedicineViewFilter filter = new HisMedicineViewFilter();
                        filter.IDs = listId;
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            result.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_MEDICINE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        private void FillDataToControl(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    treatmentId = treatment.ID;
                    LblPatientCode.Text = treatment.TDL_PATIENT_CODE;
                    LblPatientName.Text = treatment.TDL_PATIENT_NAME;

                    LblGender.Text = treatment.TDL_PATIENT_GENDER_NAME;
                    LblHeinNumber.Text = treatment.TDL_HEIN_CARD_NUMBER;
                    LblMediOrg.Text = string.Format("{0} - {1}", treatment.TDL_HEIN_MEDI_ORG_CODE, treatment.TDL_HEIN_MEDI_ORG_NAME);
                    LblHeinTime.Text = string.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_HEIN_CARD_FROM_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_HEIN_CARD_TO_TIME ?? 0));
                    LblAddress.Text = treatment.TDL_PATIENT_ADDRESS;
                    LblMainIcd.Text = string.Format("{0} - {1}", treatment.ICD_CODE, treatment.ICD_NAME);
                    LblSubIcd.Text = string.Format("{0} - {1}", treatment.ICD_SUB_CODE, treatment.ICD_TEXT);
                    LblNote.Text = treatment.APPROVE_FINISH_NOTE;
                    LblIcdYhct.Text = string.Format("{0} - {1}", treatment.TRADITIONAL_ICD_CODE, treatment.TRADITIONAL_ICD_NAME);
                    LblSubIcdYhct.Text = string.Format("{0} - {1}", treatment.TRADITIONAL_ICD_SUB_CODE, treatment.TRADITIONAL_ICD_TEXT);
                    if (treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        LblDob.Text = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        LblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
                    }

                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == treatment.TDL_PATIENT_TYPE_ID);
                    if (patientType != null)
                    {
                        LblPatientType.Text = patientType.PATIENT_TYPE_NAME;
                    }
                    else
                    {
                        LblPatientType.Text = "";
                    }

                    long? sttId = treatment.APPROVAL_STORE_STT_ID;
                    // Trạng thái: 1 = Duyệt (__CHOT), 2 = Chưa đạt (__TU_CHOI), 3 = Đạt

                    // GÁN TRỰC TIẾP Enabled cho cả 4 nút để mỗi lần refresh xác định lại đầy đủ,
                    // không giữ trạng thái enable cũ (tránh bug nút Duyệt vẫn enable sau khi đổi trạng thái).

                    // Không đạt: Chưa chốt (null) hoặc Đạt (3)
                    btnKhongDat.Enabled = (sttId == null || sttId == APPROVAL_STORE_STT_ID__DAT);

                    // Đạt: Chưa chốt (null) hoặc Chưa đạt (2)
                    btnDat.Enabled = (sttId == null || sttId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__TU_CHOI);

                    // Duyệt: có quyền HIS000056 + config≠1 + trạng thái = Đạt (3)
                    btnDuyet.Enabled = (hasPermissionApprove && !isAutoApprovalStore)
                        && sttId == APPROVAL_STORE_STT_ID__DAT;

                    // Hủy duyệt: ((config≠1 && HIS000055) || config=1) + trạng thái = Duyệt (1)
                    btnHuyDuyet.Enabled = ((!isAutoApprovalStore && hasPermissionUnapprove) || isAutoApprovalStore)
                        && sttId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__CHOT;

                    if (sttId == null)
                    {
                        lblStatus.Text = GetLangValue("Status.NotClosed");
                    }
                    else if (sttId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__CHOT)
                    {
                        lblStatus.Text = GetLangValue("Status.Approved");
                    }
                    else if (sttId == APPROVAL_STORE_STT_ID__DAT)
                    {
                        lblStatus.Text = GetLangValue("Status.Qualified");
                    }
                    else
                    {
                        lblStatus.Text = GetLangValue("Status.NotQualified");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridEmrDocumentType()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                var dt = new BackendAdapter(paramCommon).Get<List<EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);
                ListDocumentType = new List<EmrDocumentTypeADO>();
                if (dt != null && dt.Count > 0)
                {
                    dt = dt.OrderByDescending(o => ListTypeId.Contains(o.ID)).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList();

                    foreach (var item in dt)
                    {
                        EmrDocumentTypeADO ado = new EmrDocumentTypeADO(item);

                        ListDocumentType.Add(ado);
                    }
                }

                GcEmrDocumentType.BeginUpdate();
                GcEmrDocumentType.DataSource = ListDocumentType;
                GcEmrDocumentType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                LblPatientCode.Text = "";
                LblPatientName.Text = "";
                LblDob.Text = "";
                LblGender.Text = "";
                LblPatientType.Text = "";
                LblHeinNumber.Text = "";
                LblHeinTime.Text = "";
                LblMediOrg.Text = "";
                LblAddress.Text = "";
                LblMainIcd.Text = "";
                LblSubIcd.Text = "";
                LblIcdYhct.Text = "";
                LblSubIcdYhct.Text = "";
                LblNote.Text = "";
                Gc_InfoRecord.DataSource = null;
                Gc_EmrDocument.DataSource = null;
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;
                btnDuyet.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrDocument()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = TxtTreatmentCode.Text;
                if (!chkIncludeCancelDoc.Checked)
                    filter.IS_DELETE = false;
                ListDocument = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);


                if (ListDocument == null)
                {
                    ListDocument = new List<V_EMR_DOCUMENT>();
                }

                // Ho so moi -> bo luong ky cua ho so truoc.
                ResetEmrSignCache();

                SyncUndefinedDocumentType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Them / xoa dong "Chua xac dinh" o luoi loai van ban theo ListDocument hien tai.
        /// Tach rieng de Cach 2 dung lai, giu luong hien thi giong het Cach 1.
        /// </summary>
        private void SyncUndefinedDocumentType()
        {
            try
            {
                if (ListDocument == null || ListDocumentType == null) return;

                if (ListDocument.Exists(o => !o.DOCUMENT_TYPE_ID.HasValue))
                {
                    if (!ListDocumentType.Exists(o => o.ID == 0))
                    {
                        //thêm dòng loại văn bản chưa xác định
                        EMR_DOCUMENT_TYPE typeOther = new EMR_DOCUMENT_TYPE();
                        typeOther.DOCUMENT_TYPE_NAME = GetLangValue("Common.Undefined");
                        EmrDocumentTypeADO ado = new EmrDocumentTypeADO(typeOther);
                        ListDocumentType.Add(ado);
                    }
                }
                else
                {

                    //xóa dòng loại văn bản Chưa xác định
                    var other = ListDocumentType.Where(o => o.ID == 0).ToList();
                    if (other != null)
                    {
                        foreach (var item in other)
                        {
                            ListDocumentType.Remove(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
