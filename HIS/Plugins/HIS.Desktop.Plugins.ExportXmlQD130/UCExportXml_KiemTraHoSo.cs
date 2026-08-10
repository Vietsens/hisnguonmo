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
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// PTTK 3142 - muc 3.3: menu chuot phai "Kiem tra ho so" tai danh sach ho so XML130.
    /// - Kiem tra dich vu co thuoc pham vi chuyen mon cua nguoi thuc hien (map HIS_SERVICE_SPECIALITY).
    /// - Chay lai toan bo validate nhu khi xuat XML (goi CreateXmlProcessor.Run, khong ghi file).
    /// - Gop toan bo thong bao hien thi 1 lan; khong loi thi bao "Ho so hop le".
    /// - Khong anh huong luong tao XML.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>Cache theo phien kiem tra: loginname -> danh sach SERVICE_ID duoc phep (null = user khong ap dung check)</summary>
        Dictionary<string, HashSet<long>> dicAllowedServiceIdsByLogin;
        /// <summary>Cache theo phien kiem tra: SPECIALITY_ID -> danh sach SERVICE_ID da map (giam so lan goi api/HisServiceSpeciality/Get)</summary>
        Dictionary<long, HashSet<long>> dicServiceIdsBySpecialityId;
        /// <summary>Cache theo phien kiem tra: SERVICE_ID -> dich vu CO duoc thiet lap map voi it nhat 1 pham vi chuyen mon hay khong.
        /// Dich vu chua thiet lap map nao -> hop le, khong kiem tra.</summary>
        Dictionary<long, bool> dicServiceHasMapping;

        private void MenuItemClick_KiemTraHoSo(object sender, EventArgs e)
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0)
                    return;
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("MenuItemClick_KiemTraHoSo Begin");
                var selection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                this.NewConfig = GetNewConfig();
                dicAllowedServiceIdsByLogin = new Dictionary<string, HashSet<long>>(StringComparer.OrdinalIgnoreCase);
                dicServiceIdsBySpecialityId = new Dictionary<long, HashSet<long>>();
                dicServiceHasMapping = new Dictionary<long, bool>();
                Dictionary<string, List<string>> dicErrorMess = new Dictionary<string, List<string>>();

                int skip = 0;
                while (selection.Count - skip > 0)
                {
                    var limit = selection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    ListSereServ = new List<V_HIS_SERE_SERV_2>();
                    ListEkipUser = new List<HIS_EKIP_USER>();
                    ListBedlog = new List<V_HIS_BED_LOG>();
                    HisTreatments = new List<V_HIS_TREATMENT_12>();
                    ListDhst = new List<HIS_DHST>();
                    HisTrackings = new List<HIS_TRACKING>();
                    HisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                    HisSereServSuin = new List<V_HIS_SERE_SERV_SUIN>();
                    HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                    ListDebates = new List<HIS_DEBATE>();
                    ListBaby = new List<V_HIS_BABY>();
                    ListMedicalAssessment = new List<V_HIS_MEDICAL_ASSESSMENT>();
                    ListHivTreatment = new List<HIS_HIV_TREATMENT>();
                    ListTuberculosisTreat = new List<HIS_TUBERCULOSIS_TREAT>();
                    ListExpMedimateUsed = new List<HIS_EXP_MEDIMATE_USED>();

                    isExportXml = true;
                    CreateThreadGetData(limit);
                    isExportXml = false;

                    ProcessCheckHoSoDetail(dicErrorMess);
                }
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Info("MenuItemClick_KiemTraHoSo End");

                if (dicErrorMess.Count == 0)
                {
                    XtraMessageBox.Show("Hồ sơ hợp lệ", Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in dicErrorMess)
                    {
                        sb.AppendLine(String.Format("- {0}: {1}", item.Key, String.Join(",", item.Value.Distinct())));
                    }
                    XtraMessageBox.Show(sb.ToString(), Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra 1 batch ho so da nap vao cac field data (sau CreateThreadGetData):
        /// check pham vi chuyen mon + goi validate cua thu vien XML130 (Run khong ghi file).
        /// </summary>
        private void ProcessCheckHoSoDetail(Dictionary<string, List<string>> dicErrorMess)
        {
            try
            {
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, List<V_HIS_SERE_SERV_SUIN>> dicSereServSuin = new Dictionary<long, List<V_HIS_SERE_SERV_SUIN>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BABY>> dicBaby = new Dictionary<long, List<V_HIS_BABY>>();
                Dictionary<long, List<HIS_DEBATE>> dicDebate = new Dictionary<long, List<HIS_DEBATE>>();
                Dictionary<long, List<HIS_DHST>> dicDhstList = new Dictionary<long, List<HIS_DHST>>();
                Dictionary<long, HIS_HIV_TREATMENT> dicHivTreatment = new Dictionary<long, HIS_HIV_TREATMENT>();
                Dictionary<long, HIS_TUBERCULOSIS_TREAT> dicTuberculosisTreat = new Dictionary<long, HIS_TUBERCULOSIS_TREAT>();
                Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUserByEkipId = new Dictionary<long, List<HIS_EKIP_USER>>();

                if (ListTuberculosisTreat != null && ListTuberculosisTreat.Count > 0)
                {
                    foreach (var item in ListTuberculosisTreat)
                    {
                        dicTuberculosisTreat[item.TREATMENT_ID] = item;
                    }
                }

                if (ListPatientTypeAlter != null && ListPatientTypeAlter.Count > 0)
                {
                    foreach (var item in ListPatientTypeAlter)
                    {
                        if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID))
                            dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    bool allowZeroPrice = HisConfigCFG.QD_130_BYT__LAY_CA_DVU_0_DONG == "1";
                    if (ListEkipUser != null && ListEkipUser.Count > 0)
                    {
                        foreach (var eu in ListEkipUser)
                        {
                            if (!dicEkipUserByEkipId.ContainsKey(eu.EKIP_ID))
                                dicEkipUserByEkipId[eu.EKIP_ID] = new List<HIS_EKIP_USER>();
                            dicEkipUserByEkipId[eu.EKIP_ID].Add(eu);
                        }
                    }

                    foreach (var sereServ in ListSereServ)
                    {
                        bool addSereServ;
                        if (allowZeroPrice)
                        {
                            addSereServ = (sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (sereServ.PRICE > 0 || sereServ.PRICE == 0))
                                || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        }
                        else
                        {
                            addSereServ = (sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.PRICE > 0)
                                || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        }

                        if (sereServ.AMOUNT > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue && addSereServ)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                        }

                        if (sereServ.EKIP_ID.HasValue && dicEkipUserByEkipId.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            List<HIS_EKIP_USER> ekips;
                            if (dicEkipUserByEkipId.TryGetValue(sereServ.EKIP_ID.Value, out ekips) && ekips.Count > 0)
                            {
                                if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                    dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();
                                dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].AddRange(ekips);
                            }
                        }
                    }
                }

                if (HisSereServTeins != null && HisSereServTeins.Count > 0)
                {
                    foreach (var ssTein in HisSereServTeins)
                    {
                        if (!ssTein.TDL_TREATMENT_ID.HasValue) continue;
                        if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                            dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();
                        dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                    }
                }

                if (HisSereServSuin != null && HisSereServSuin.Count > 0)
                {
                    foreach (var ssSuin in HisSereServSuin)
                    {
                        if (!dicSereServSuin.ContainsKey(ssSuin.TDL_TREATMENT_ID))
                            dicSereServSuin[ssSuin.TDL_TREATMENT_ID] = new List<V_HIS_SERE_SERV_SUIN>();
                        dicSereServSuin[ssSuin.TDL_TREATMENT_ID].Add(ssSuin);
                    }
                }

                if (HisTrackings != null && HisTrackings.Count > 0)
                {
                    foreach (var tracking in HisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();
                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
                    }
                }

                if (ListBaby != null && ListBaby.Count > 0)
                {
                    foreach (var baby in ListBaby)
                    {
                        if (!dicBaby.ContainsKey(baby.TREATMENT_ID))
                            dicBaby[baby.TREATMENT_ID] = new List<V_HIS_BABY>();
                        dicBaby[baby.TREATMENT_ID].Add(baby);
                    }
                }

                if (ListHivTreatment != null && ListHivTreatment.Count > 0)
                {
                    foreach (var hivTreatment in ListHivTreatment.OrderBy(o => o.ID))
                    {
                        dicHivTreatment[hivTreatment.TREATMENT_ID] = hivTreatment;
                    }
                }

                if (HisSereServPttts != null && HisSereServPttts.Count > 0)
                {
                    foreach (var ssPttt in HisSereServPttts)
                    {
                        if (!ssPttt.TDL_TREATMENT_ID.HasValue) continue;
                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                            dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();
                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }

                if (ListDhst != null && ListDhst.Count > 0)
                {
                    foreach (var item in ListDhst)
                    {
                        if (!dicDhstList.ContainsKey(item.TREATMENT_ID))
                            dicDhstList[item.TREATMENT_ID] = new List<HIS_DHST>();
                        dicDhstList[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListBedlog != null && ListBedlog.Count > 0)
                {
                    foreach (var bed in ListBedlog)
                    {
                        if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                            dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();
                        dicBedLog[bed.TREATMENT_ID].Add(bed);
                    }
                }

                if (ListDebates != null && ListDebates.Count > 0)
                {
                    foreach (var item in ListDebates)
                    {
                        if (!dicDebate.ContainsKey(item.TREATMENT_ID))
                            dicDebate[item.TREATMENT_ID] = new List<HIS_DEBATE>();
                        dicDebate[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListExpMedimateUsed != null && ListExpMedimateUsed.Count > 0)
                {
                    foreach (var u in ListExpMedimateUsed)
                    {
                        if (u.EXP_MEST_MEDICINE_ID.HasValue)
                        {
                            var k = u.EXP_MEST_MEDICINE_ID.Value;
                            if (!dicExpUsedByExpMestMedicineId.ContainsKey(k))
                                dicExpUsedByExpMestMedicineId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                            dicExpUsedByExpMestMedicineId[k].Add(u);
                        }
                        if (u.EXP_MEST_MATERIAL_ID.HasValue)
                        {
                            var k = u.EXP_MEST_MATERIAL_ID.Value;
                            if (!dicExpUsedByExpMestMaterialId.ContainsKey(k))
                                dicExpUsedByExpMestMaterialId[k] = new List<HIS_EXP_MEDIMATE_USED>();
                            dicExpUsedByExpMestMaterialId[k].Add(u);
                        }
                    }
                }

                var totalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                var totalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
                var totalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                var totalIcdData = BackendDataWorker.Get<HIS_ICD>();
                var totalServiceData = BackendDataWorker.Get<V_HIS_SERVICE>();
                var totalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
                var totalDepartmentData = HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1"
                    ? BackendDataWorker.Get<HIS_DEPARTMENT>()
                    : null;

                // Du lieu phuc vu check pham vi chuyen mon
                Dictionary<string, HIS_EMPLOYEE> dicEmployeeByLogin = new Dictionary<string, HIS_EMPLOYEE>(StringComparer.OrdinalIgnoreCase);
                foreach (var emp in totalEmployeeData)
                {
                    if (!String.IsNullOrEmpty(emp.LOGINNAME) && !dicEmployeeByLogin.ContainsKey(emp.LOGINNAME))
                        dicEmployeeByLogin[emp.LOGINNAME] = emp;
                }
                Dictionary<string, HIS_SPECIALITY> dicSpecialityByCode = new Dictionary<string, HIS_SPECIALITY>(StringComparer.OrdinalIgnoreCase);
                foreach (var spe in BackendDataWorker.Get<HIS_SPECIALITY>())
                {
                    if (!String.IsNullOrEmpty(spe.SPECIALITY_CODE) && !dicSpecialityByCode.ContainsKey(spe.SPECIALITY_CODE))
                        dicSpecialityByCode[spe.SPECIALITY_CODE] = spe;
                }
                // Nhom y lenh lay nguoi thuc hien theo kip: HST_BHYT_CODE khai bao trong config HIS.QD_130_BYT.NGUOI_THUC_HIEN_OPTION
                // (so truc tiep HST_BHYT_CODE cua dich vu, split ',' — giong Xml3Processor dien NGUOI_THUC_HIEN)
                HashSet<string> configBhytCodes = new HashSet<string>(StringComparer.Ordinal);
                if (!String.IsNullOrEmpty(HisConfigCFG.QD_130_BYT__NGUOI_THUC_HIEN_OPTION))
                {
                    foreach (var code in HisConfigCFG.QD_130_BYT__NGUOI_THUC_HIEN_OPTION.Split(','))
                    {
                        configBhytCodes.Add(code);
                    }
                }
                // Y lenh Giuong: khong kiem tra pham vi chuyen mon (PTTK)
                HashSet<long> bedHeinTypeIds = new HashSet<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L
                };
                // Vat tu: XML3 khong dien NGUOI_THUC_HIEN cho nhom nay -> khong kiem tra
                HashSet<long> materialHeinTypeIds = new HashSet<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL,
                    IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                };

                // Tra truoc: dich vu nao trong batch DA duoc thiet lap map Dich vu - Pham vi chuyen mon.
                // Dich vu chua thiet lap map nao -> hop le, khong kiem tra PVCM.
                EnsureServiceMappingCache(dicSereServ.Values.SelectMany(o => o).Select(o => o.SERVICE_ID).Distinct().ToList());

                foreach (var treatment in HisTreatments)
                {
                    if (!dicSereServ.ContainsKey(treatment.ID))
                    {
                        var errorSereServ = "Hồ sơ không có dịch vụ";
                        if (!dicErrorMess.ContainsKey(errorSereServ))
                        {
                            dicErrorMess[errorSereServ] = new List<string>();
                        }
                        dicErrorMess[errorSereServ].Add(treatment.TREATMENT_CODE);
                        continue;
                    }

                    // 1. Kiem tra pham vi chuyen mon cua nguoi thuc hien (PTTK 3142)
                    CheckPhamViChuyenMon(treatment, dicSereServ[treatment.ID], dicEkipUserByEkipId, dicEmployeeByLogin, dicSpecialityByCode, configBhytCodes, bedHeinTypeIds, materialHeinTypeIds, dicErrorMess);

                    // 2. Chay lai validate cua thu vien xuat XML (khong ghi file, khong ky so)
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    if (dicPatientTypeAlter.ContainsKey(treatment.ID))
                    {
                        ado.ListPatientTypeAlter = dicPatientTypeAlter[treatment.ID];
                    }
                    ado.ListSereServ = dicSereServ[treatment.ID];
                    if (dicDhstList.ContainsKey(treatment.ID))
                    {
                        ado.ListDhst = dicDhstList[treatment.ID];
                    }
                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.ListSereServTein = dicSereServTein[treatment.ID];
                    }
                    if (dicSereServSuin.ContainsKey(treatment.ID))
                    {
                        ado.vSereServSuin = dicSereServSuin[treatment.ID];
                    }
                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.ListSereServPttt = dicSereServPttt[treatment.ID];
                    }
                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.ListBedLog = dicBedLog[treatment.ID];
                    }
                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.ListTracking = dicTracking[treatment.ID];
                    }
                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.ListEkipUser = dicEkipUser[treatment.ID].Distinct().ToList();
                    }
                    if (dicDebate.ContainsKey(treatment.ID))
                    {
                        ado.ListDebate = dicDebate[treatment.ID];
                    }
                    if (dicBaby.ContainsKey(treatment.ID))
                    {
                        ado.ListBaby = dicBaby[treatment.ID];
                    }
                    if (dicHivTreatment.ContainsKey(treatment.ID))
                    {
                        ado.HivTreatment = dicHivTreatment[treatment.ID];
                    }
                    if (dicTuberculosisTreat.ContainsKey(treatment.ID))
                    {
                        ado.TuberculosisTreat = dicTuberculosisTreat[treatment.ID];
                    }
                    ado.TotalMaterialTypeData = totalMaterialTypeData;
                    ado.TotalHeinMediOrgData = totalHeinMediOrgData;
                    ado.TotalConfigData = NewConfig;
                    ado.TotalPatientTypeData = totalPatientTypeData;
                    ado.TotalIcdData = totalIcdData;
                    ado.TotalSericeData = totalServiceData;
                    ado.TotalEmployeeData = totalEmployeeData;

                    var usedList = new List<HIS_EXP_MEDIMATE_USED>();
                    foreach (var ss in ado.ListSereServ)
                    {
                        if (ss.EXP_MEST_MEDICINE_ID.HasValue)
                        {
                            List<HIS_EXP_MEDIMATE_USED> lst;
                            if (dicExpUsedByExpMestMedicineId.TryGetValue(ss.EXP_MEST_MEDICINE_ID.Value, out lst))
                                usedList.AddRange(lst);
                        }
                        if (ss.EXP_MEST_MATERIAL_ID.HasValue)
                        {
                            List<HIS_EXP_MEDIMATE_USED> lst;
                            if (dicExpUsedByExpMestMaterialId.TryGetValue(ss.EXP_MEST_MATERIAL_ID.Value, out lst))
                                usedList.AddRange(lst);
                        }
                    }
                    ado.ListExpMedimateUsed = usedList.GroupBy(x => x.ID).Select(g => g.First()).ToList();

                    if (totalDepartmentData != null)
                    {
                        ado.ListDepartment = totalDepartmentData;
                    }
                    ado.IS_3176 = false;

                    // Goi ham validate rieng cua thu vien (CheckHoSo — gom TAT CA loi, khong build XML, khong ghi file)
                    His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(ado);
                    List<string> checkErrors = xmlProcessor.CheckHoSo();
                    if (checkErrors != null && checkErrors.Count > 0)
                    {
                        foreach (var checkError in checkErrors)
                        {
                            if (!dicErrorMess.ContainsKey(checkError))
                            {
                                dicErrorMess[checkError] = new List<string>();
                            }
                            dicErrorMess[checkError].Add(treatment.TREATMENT_CODE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra dich vu cua ho so co thuoc pham vi chuyen mon cua nguoi thuc hien (PTTK 3142 muc 3.3).
        /// Cach chon nguoi thuc hien MO PHONG dung logic dien truong NGUOI_THUC_HIEN cua Xml3Processor (XML3):
        /// - Y lenh kham (ID__KH): theo PTTK, nguoi thuc hien = EXECUTE_LOGINNAME.
        /// - Giuong (GI_*): khong kiem tra (PTTK); Vat tu (VT_*): XML3 khong dien nguoi thuc hien -> khong kiem tra.
        /// - Y lenh con lai (giong XML3):
        ///   1) HST_BHYT_CODE thuoc config NGUOI_THUC_HIEN_OPTION va co kip -> lay ca kip;
        ///   2) Neu chua co ai -> gom SAMPLER_LOGINNAME + SUBCLINICAL_RESULT_LOGINNAME (tach ';') + EXECUTE_LOGINNAME;
        ///   3) Kip (EKIP_ID) luon duoc cong them neu co;
        ///   4) Van chua co ai va START_TIME null -> lay REQUEST_LOGINNAME.
        ///   Chi tinh nguoi co chung chi hanh nghe (HIS_EMPLOYEE.DIPLOMA) — dung tap nguoi thuc su len truong NGUOI_THUC_HIEN cua XML3.
        /// - Voi moi nguoi thuc hien co SPECIALITY_CODES: dich vu nam ngoai danh sach map thi bao loi kem ma y lenh.
        /// </summary>
        private void CheckPhamViChuyenMon(V_HIS_TREATMENT_12 treatment, List<V_HIS_SERE_SERV_2> sereServs,
            Dictionary<long, List<HIS_EKIP_USER>> dicEkipUserByEkipId,
            Dictionary<string, HIS_EMPLOYEE> dicEmployeeByLogin,
            Dictionary<string, HIS_SPECIALITY> dicSpecialityByCode,
            HashSet<string> configBhytCodes,
            HashSet<long> bedHeinTypeIds,
            HashSet<long> materialHeinTypeIds,
            Dictionary<string, List<string>> dicErrorMess)
        {
            try
            {
                foreach (var sereServ in sereServs)
                {
                    if (!sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue)
                        continue;
                    long heinTypeId = sereServ.TDL_HEIN_SERVICE_TYPE_ID.Value;
                    if (bedHeinTypeIds.Contains(heinTypeId) || materialHeinTypeIds.Contains(heinTypeId))
                        continue;

                    // Dich vu chua duoc thiet lap map voi bat ky pham vi chuyen mon nao -> hop le, bo qua
                    bool serviceHasMapping;
                    if (!dicServiceHasMapping.TryGetValue(sereServ.SERVICE_ID, out serviceHasMapping) || !serviceHasMapping)
                        continue;

                    List<string> executors = new List<string>();
                    if (heinTypeId == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH)
                    {
                        // Y lenh kham: theo PTTK lay EXECUTE_LOGINNAME (khong loc theo CCHN)
                        if (!String.IsNullOrEmpty(sereServ.EXECUTE_LOGINNAME))
                            executors.Add(sereServ.EXECUTE_LOGINNAME);
                    }
                    else
                    {
                        // 1) Nhom thuoc config -> uu tien lay theo kip (giong XML3: IsNguoiThucHien)
                        bool isNguoiThucHienConfig = !String.IsNullOrEmpty(sereServ.HST_BHYT_CODE) && configBhytCodes.Contains(sereServ.HST_BHYT_CODE);
                        if (isNguoiThucHienConfig && sereServ.EKIP_ID.HasValue)
                        {
                            List<HIS_EKIP_USER> ekips;
                            if (dicEkipUserByEkipId.TryGetValue(sereServ.EKIP_ID.Value, out ekips))
                            {
                                foreach (var ekipUser in ekips)
                                {
                                    AddExecutorHasDiploma(executors, ekipUser.LOGINNAME, dicEmployeeByLogin);
                                }
                            }
                        }

                        // 2) Chua co ai -> gom SAMPLER + KTV (tach ';') + EXECUTE (giong XML3)
                        if (executors.Count == 0)
                        {
                            AddExecutorHasDiploma(executors, sereServ.SAMPLER_LOGINNAME, dicEmployeeByLogin);
                            if (!String.IsNullOrWhiteSpace(sereServ.SUBCLINICAL_RESULT_LOGINNAME))
                            {
                                foreach (var login in sereServ.SUBCLINICAL_RESULT_LOGINNAME.Split(';'))
                                {
                                    AddExecutorHasDiploma(executors, login, dicEmployeeByLogin);
                                }
                            }
                            AddExecutorHasDiploma(executors, sereServ.EXECUTE_LOGINNAME, dicEmployeeByLogin);
                        }

                        // 3) Kip luon duoc cong them neu co (giong XML3: nhan Ekip nam ngoai if)
                        if (sereServ.EKIP_ID.HasValue)
                        {
                            List<HIS_EKIP_USER> ekips;
                            if (dicEkipUserByEkipId.TryGetValue(sereServ.EKIP_ID.Value, out ekips))
                            {
                                foreach (var ekipUser in ekips)
                                {
                                    AddExecutorHasDiploma(executors, ekipUser.LOGINNAME, dicEmployeeByLogin);
                                }
                            }
                        }

                        // 4) Van chua co ai va chua co thoi gian bat dau y lenh -> lay tai khoan chi dinh
                        if (executors.Count == 0 && sereServ.START_TIME == null)
                        {
                            AddExecutorHasDiploma(executors, sereServ.REQUEST_LOGINNAME, dicEmployeeByLogin);
                        }
                    }

                    if (executors.Count == 0)
                        continue;

                    foreach (var loginname in executors.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        HashSet<long> allowedServiceIds = GetAllowedServiceIdsByLogin(loginname, dicEmployeeByLogin, dicSpecialityByCode);
                        if (allowedServiceIds == null)
                            continue;
                        if (!allowedServiceIds.Contains(sereServ.SERVICE_ID))
                        {
                            string message = String.Format(
                                "Dịch vụ {0} {1} không thuộc phạm vi chuyên môn của người thực hiện ({2}). (Mã y lệnh: {3})",
                                sereServ.TDL_SERVICE_CODE, sereServ.TDL_SERVICE_NAME, loginname, sereServ.TDL_SERVICE_REQ_CODE);
                            if (!dicErrorMess.ContainsKey(message))
                            {
                                dicErrorMess[message] = new List<string>();
                            }
                            dicErrorMess[message].Add(treatment.TREATMENT_CODE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tra truoc va cache: moi SERVICE_ID co duoc thiet lap map Dich vu - Pham vi chuyen mon hay khong
        /// (goi api/HisServiceSpeciality/Get theo SERVICE_IDs, chi goi cho cac id chua co trong cache).
        /// </summary>
        private void EnsureServiceMappingCache(List<long> serviceIds)
        {
            try
            {
                List<long> missingIds = serviceIds.Where(o => !dicServiceHasMapping.ContainsKey(o)).Distinct().ToList();
                int skip = 0;
                while (missingIds.Count - skip > 0)
                {
                    List<long> chunk = missingIds.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    foreach (var id in chunk)
                    {
                        dicServiceHasMapping[id] = false;
                    }

                    CommonParam param = new CommonParam();
                    ADO.HisServiceSpecialityFilter filter = new ADO.HisServiceSpecialityFilter();
                    filter.SERVICE_IDs = chunk;
                    var serviceSpecialities = new BackendAdapter(param).Get<List<HIS_SERVICE_SPECIALITY>>(
                        "api/HisServiceSpeciality/Get", ApiConsumers.MosConsumer, filter, param);
                    if (serviceSpecialities != null && serviceSpecialities.Count > 0)
                    {
                        foreach (var item in serviceSpecialities)
                        {
                            dicServiceHasMapping[item.SERVICE_ID] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Them 1 tai khoan vao danh sach nguoi thuc hien neu nhan vien ton tai va CO chung chi hanh nghe (DIPLOMA)
        /// — giong dieu kien vao truong NGUOI_THUC_HIEN cua Xml3Processor (GetMaBacSi tra DIPLOMA khac rong).
        /// </summary>
        private void AddExecutorHasDiploma(List<string> executors, string loginname, Dictionary<string, HIS_EMPLOYEE> dicEmployeeByLogin)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(loginname))
                    return;
                loginname = loginname.Trim();
                HIS_EMPLOYEE employee;
                if (dicEmployeeByLogin.TryGetValue(loginname, out employee) && !String.IsNullOrWhiteSpace(employee.DIPLOMA))
                {
                    executors.Add(loginname);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay danh sach SERVICE_ID duoc phep cua 1 tai khoan tu map HIS_SERVICE_SPECIALITY (api/HisServiceSpeciality/Get).
        /// Tra ve null = khong ap dung check (user khong co SPECIALITY_CODES hoac chua map dich vu nao — theo PTTK "Neu A khac null thi" moi kiem tra).
        /// Ket qua duoc cache theo loginname va theo SPECIALITY_ID trong 1 phien "Kiem tra ho so".
        /// </summary>
        private HashSet<long> GetAllowedServiceIdsByLogin(string loginname,
            Dictionary<string, HIS_EMPLOYEE> dicEmployeeByLogin,
            Dictionary<string, HIS_SPECIALITY> dicSpecialityByCode)
        {
            HashSet<long> result = null;
            try
            {
                if (dicAllowedServiceIdsByLogin.TryGetValue(loginname, out result))
                    return result;

                HIS_EMPLOYEE employee;
                if (!dicEmployeeByLogin.TryGetValue(loginname, out employee) || String.IsNullOrEmpty(employee.SPECIALITY_CODES))
                {
                    dicAllowedServiceIdsByLogin[loginname] = null;
                    return null;
                }

                List<long> specialityIds = new List<long>();
                foreach (var code in employee.SPECIALITY_CODES.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()))
                {
                    HIS_SPECIALITY speciality;
                    if (dicSpecialityByCode.TryGetValue(code, out speciality) && !specialityIds.Contains(speciality.ID))
                        specialityIds.Add(speciality.ID);
                }
                if (specialityIds.Count == 0)
                {
                    dicAllowedServiceIdsByLogin[loginname] = null;
                    return null;
                }

                // Chi goi api cho cac SPECIALITY_ID chua co trong cache
                List<long> missingIds = specialityIds.Where(o => !dicServiceIdsBySpecialityId.ContainsKey(o)).ToList();
                if (missingIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    ADO.HisServiceSpecialityFilter filter = new ADO.HisServiceSpecialityFilter();
                    filter.SPECIALITY_IDs = missingIds;
                    var serviceSpecialities = new BackendAdapter(param).Get<List<HIS_SERVICE_SPECIALITY>>(
                        "api/HisServiceSpeciality/Get", ApiConsumers.MosConsumer, filter, param);
                    foreach (var id in missingIds)
                    {
                        dicServiceIdsBySpecialityId[id] = new HashSet<long>();
                    }
                    if (serviceSpecialities != null && serviceSpecialities.Count > 0)
                    {
                        foreach (var item in serviceSpecialities)
                        {
                            HashSet<long> serviceIds;
                            if (dicServiceIdsBySpecialityId.TryGetValue(item.SPECIALITY_ID, out serviceIds))
                            {
                                serviceIds.Add(item.SERVICE_ID);
                            }
                        }
                    }
                }

                HashSet<long> allowed = new HashSet<long>();
                foreach (var id in specialityIds)
                {
                    HashSet<long> serviceIds;
                    if (dicServiceIdsBySpecialityId.TryGetValue(id, out serviceIds))
                    {
                        foreach (var serviceId in serviceIds)
                            allowed.Add(serviceId);
                    }
                }

                // Danh sach A rong = user chua duoc map dich vu nao -> khong ap dung check (theo PTTK)
                result = allowed.Count > 0 ? allowed : null;
                dicAllowedServiceIdsByLogin[loginname] = result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
