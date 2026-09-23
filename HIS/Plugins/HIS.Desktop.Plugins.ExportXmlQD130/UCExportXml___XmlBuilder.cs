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
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Dung doi tuong dau vao cho thu vien sinh tep XML.
    ///
    /// Truoc day doan nay nam ngay trong ProcessSyncTreatment. Tach ra de luong soat loi MDInsight
    /// dung LAI DUNG mot logic, khong chep sang noi khac roi de hai ban lech nhau.
    /// Giu nguyen tung buoc va tung dieu kien cua ban goc - day la thay doi ve noi dat code,
    /// KHONG phai thay doi hanh vi.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>
        /// Dung <see cref="InputADO"/> cho MOT ho so dieu tri.
        /// </summary>
        /// <param name="ctx">Du lieu dung chung ca lo, nap mot lan truoc vong lap</param>
        /// <param name="treatment">Ho so dang xu ly</param>
        /// <param name="sendXml12">Co sinh tep XML 12 cho ho so nay hay khong</param>
        /// <returns>
        /// Doi tuong dau vao, hoac <c>null</c> khi ho so KHONG co dong dich vu nao -
        /// ben goi phai bo qua ho so do (tuong duong lenh continue cua ban goc).
        /// </returns>
        internal InputADO BuildInputAdoForXml(XmlBuildContextADO ctx, V_HIS_TREATMENT_12 treatment, out bool sendXml12)
        {
            sendXml12 = true;

            if (ctx == null || treatment == null)
            {
                return null;
            }

            InputADO ado = new InputADO();
            ado.Treatment = treatment;

            if (ctx.DicPatientTypeAlter != null && ctx.DicPatientTypeAlter.ContainsKey(treatment.ID))
            {
                ado.ListPatientTypeAlter = ctx.DicPatientTypeAlter[treatment.ID];
            }

            //Khong co dong dich vu nao thi khong dung duoc tep - ben goi bo qua ho so nay
            if (ctx.DicSereServ == null || !ctx.DicSereServ.ContainsKey(treatment.ID))
            {
                return null;
            }

            ado.ListSereServ = ctx.DicSereServ[treatment.ID];

            if (ctx.DicDhstList != null && ctx.DicDhstList.ContainsKey(treatment.ID))
            {
                ado.ListDhst = ctx.DicDhstList[treatment.ID];
            }

            if (ctx.DicSereServTein != null && ctx.DicSereServTein.ContainsKey(treatment.ID))
            {
                ado.ListSereServTein = ctx.DicSereServTein[treatment.ID];
            }

            if (ctx.DicSereServSuin != null && ctx.DicSereServSuin.ContainsKey(treatment.ID))
            {
                ado.vSereServSuin = ctx.DicSereServSuin[treatment.ID];
            }

            if (ctx.DicSereServPttt != null && ctx.DicSereServPttt.ContainsKey(treatment.ID))
            {
                ado.ListSereServPttt = ctx.DicSereServPttt[treatment.ID];
            }

            if (ctx.DicBedLog != null && ctx.DicBedLog.ContainsKey(treatment.ID))
            {
                ado.ListBedLog = ctx.DicBedLog[treatment.ID];
            }

            if (ctx.DicTracking != null && ctx.DicTracking.ContainsKey(treatment.ID))
            {
                ado.ListTracking = ctx.DicTracking[treatment.ID];
            }

            if (ctx.DicEkipUser != null && ctx.DicEkipUser.ContainsKey(treatment.ID))
            {
                ado.ListEkipUser = ctx.DicEkipUser[treatment.ID].Distinct().ToList();
            }

            if (ctx.DicDebate != null && ctx.DicDebate.ContainsKey(treatment.ID))
            {
                ado.ListDebate = ctx.DicDebate[treatment.ID];
            }

            if (ctx.DicBaby != null && ctx.DicBaby.ContainsKey(treatment.ID))
            {
                ado.ListBaby = ctx.DicBaby[treatment.ID];
            }

            if (ctx.DicMedicalAssessment != null && ctx.DicMedicalAssessment.ContainsKey(treatment.ID))
            {
                ado.ListMedicalAssessment = ctx.DicMedicalAssessment[treatment.ID];
            }
            else
            {
                sendXml12 = false;
            }

            sendXml12 = !string.IsNullOrEmpty(ctx.TypeXml)
                ? ctx.TypeXml.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains("12")
                  && ado.ListMedicalAssessment != null && ado.ListMedicalAssessment.Count > 0
                : false;

            if (ctx.DicHivTreatment != null && ctx.DicHivTreatment.ContainsKey(treatment.ID))
            {
                ado.HivTreatment = ctx.DicHivTreatment[treatment.ID];
            }

            ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
            ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
            ado.TotalConfigData = NewConfig;
            ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
            ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
            ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
            ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
            ado.TotalMachineData = BackendDataWorker.Get<HIS_MACHINE>();

            ado.ListExpMedimateUsed = BuildExpMedimateUsedOfTreatment(ctx, ado.ListSereServ);

            if (HisConfigCFG.QD_130_BVT_XML1_MA_KHOA_OPTION == "1")
            {
                ado.ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            }

            ado.serverInfo = ctx.ServerInfo;

            if (ctx.DicTuberculosisTreat != null && ctx.DicTuberculosisTreat.ContainsKey(treatment.ID))
            {
                ado.TuberculosisTreat = ctx.DicTuberculosisTreat[treatment.ID];
            }

            ado.IS_3176 = ReadXml3176CheckedSafe();

            return ado;
        }

        /// <summary>
        /// Loc danh sach thuoc/vat tu da dung CHI CUA MOT HO SO, theo dung cac dong dich vu cua ho so do.
        ///
        /// ⚠️ Tuyet doi khong truyen ca lo: viec do khop se nhan cheo giua cac ho so
        /// (du phong theo ma thuoc / ma vat tu khong duy nhat), lam so luong bi cong don va
        /// thoi phong T_TONGCHI_BV / T_TONGCHI_BH tren tep XML sinh ra.
        /// </summary>
        private List<HIS_EXP_MEDIMATE_USED> BuildExpMedimateUsedOfTreatment(
            XmlBuildContextADO ctx, List<V_HIS_SERE_SERV_2> listSereServ)
        {
            List<HIS_EXP_MEDIMATE_USED> usedList = new List<HIS_EXP_MEDIMATE_USED>();

            try
            {
                if (listSereServ == null || listSereServ.Count == 0)
                {
                    return usedList;
                }

                foreach (var sereServ in listSereServ)
                {
                    if (sereServ == null)
                    {
                        continue;
                    }

                    List<HIS_EXP_MEDIMATE_USED> found;

                    if (sereServ.EXP_MEST_MEDICINE_ID.HasValue
                        && ctx.DicExpUsedByExpMestMedicineId != null
                        && ctx.DicExpUsedByExpMestMedicineId.TryGetValue(sereServ.EXP_MEST_MEDICINE_ID.Value, out found))
                    {
                        usedList.AddRange(found);
                    }

                    if (sereServ.EXP_MEST_MATERIAL_ID.HasValue
                        && ctx.DicExpUsedByExpMestMaterialId != null
                        && ctx.DicExpUsedByExpMestMaterialId.TryGetValue(sereServ.EXP_MEST_MATERIAL_ID.Value, out found))
                    {
                        usedList.AddRange(found);
                    }
                }

                //Mot dong co the duoc gom vao tu ca hai phia - khu trung theo ma dong
                return usedList.GroupBy(o => o.ID).Select(g => g.First()).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return usedList;
            }
        }

        /// <summary>
        /// Doc trang thai o chon "XML 3176" an toan khi dang chay o luong nen.
        /// Giu nguyen cach doc cua ban goc de khong doi hanh vi khi nguoi dung bat/tat giua chung.
        /// </summary>
        private bool ReadXml3176CheckedSafe()
        {
            bool isChecked = false;

            try
            {
                if (chkXML3176.InvokeRequired)
                {
                    chkXML3176.Invoke(new MethodInvoker(delegate { isChecked = chkXML3176.Checked; }));
                }
                else
                {
                    isChecked = chkXML3176.Checked;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return isChecked;
        }

        /// <summary>
        /// Nap toan bo du lieu can thiet de dung tep XML cho MOT LO ho so, roi gom lai
        /// thanh <see cref="XmlBuildContextADO"/>.
        ///
        /// Truoc day doan nay nam ngay trong ProcessSyncTreatment. Tach ra de luong soat loi
        /// MDInsight dung LAI DUNG mot duong nap du lieu - day la thay doi ve noi dat code,
        /// KHONG phai thay doi hanh vi.
        /// </summary>
        internal XmlBuildContextADO LoadXmlBatchData(
            List<V_HIS_TREATMENT_1> limit, string typeXml, string username, string password,
            string address, string xml130Api, string xmlGdykApi)
        {
                        #region
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
                        CreateThreadGetData(limit);
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
                        Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>> dicMedicalAssessment = new Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>>();
                        Dictionary<long, HIS_HIV_TREATMENT> dicHivTreatment = new Dictionary<long, HIS_HIV_TREATMENT>();
                        Dictionary<long, HIS_TUBERCULOSIS_TREAT> dicTuberculosisTreat = new Dictionary<long, HIS_TUBERCULOSIS_TREAT>();
                        Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                        Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> dicExpUsedByExpMestMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();

                        if (ListExpMedimateUsed != null && ListExpMedimateUsed.Count > 0)
                        {
                            foreach (var u in ListExpMedimateUsed)
                            {
                                if (u == null) continue;

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

                        if (ListTuberculosisTreat != null && ListTuberculosisTreat.Count > 0)
                        {
                            foreach (var item in ListTuberculosisTreat)
                            {
                                if (!dicTuberculosisTreat.ContainsKey(item.TREATMENT_ID))
                                    dicTuberculosisTreat[item.TREATMENT_ID] = new HIS_TUBERCULOSIS_TREAT();
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
                            foreach (var sereServ in ListSereServ)
                            {
                                if (sereServ.AMOUNT > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue && ((sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.PRICE > 0) || sereServ.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE))
                                {
                                    if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                        dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                                    dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                                }

                                if (sereServ.EKIP_ID.HasValue && ListEkipUser != null && ListEkipUser.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                                {
                                    var ekips = ListEkipUser.Where(o => o.EKIP_ID == sereServ.EKIP_ID).ToList();
                                    if (ekips != null && ekips.Count > 0)
                                    {
                                        foreach (var item in ekips)
                                        {
                                            if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                                dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();

                                            dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].Add(item);
                                        }
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
                            ListHivTreatment = ListHivTreatment.OrderBy(o => o.ID).ToList();
                            foreach (var hivTreatment in ListHivTreatment)
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
                        if (ListMedicalAssessment != null && ListMedicalAssessment.Count > 0)
                        {
                            foreach (var item in ListMedicalAssessment)
                            {
                                if (!dicMedicalAssessment.ContainsKey(item.TREATMENT_ID))
                                    dicMedicalAssessment[item.TREATMENT_ID] = new List<V_HIS_MEDICAL_ASSESSMENT>();

                                dicMedicalAssessment[item.TREATMENT_ID].Add(item);
                            }
                        }
                        #endregion

                        //vCong XXXXX - Dau noi MDInsight: gom du lieu dung chung ca lo mot lan o day,
                        //de luong nay va luong soat loi MDInsight dung CHUNG ham dung ado
                        //(xem UCExportXml___XmlBuilder.cs). Truoc day doan dung ado nam ngay trong vong lap.
                        XmlBuildContextADO xmlBuildContext = new XmlBuildContextADO
                        {
                            DicPatientTypeAlter = dicPatientTypeAlter,
                            DicSereServ = dicSereServ,
                            DicDhstList = dicDhstList,
                            DicSereServTein = dicSereServTein,
                            DicSereServSuin = dicSereServSuin,
                            DicSereServPttt = dicSereServPttt,
                            DicBedLog = dicBedLog,
                            DicTracking = dicTracking,
                            DicEkipUser = dicEkipUser,
                            DicDebate = dicDebate,
                            DicBaby = dicBaby,
                            DicMedicalAssessment = dicMedicalAssessment,
                            DicHivTreatment = dicHivTreatment,
                            DicTuberculosisTreat = dicTuberculosisTreat,
                            DicExpUsedByExpMestMedicineId = dicExpUsedByExpMestMedicineId,
                            DicExpUsedByExpMestMaterialId = dicExpUsedByExpMestMaterialId,
                            TypeXml = typeXml,
                            ServerInfo = new ServerInfo()
                            {
                                Username = username,
                                Password = password,
                                Address = address,
                                TypeXml = typeXml,
                                Xml130Api = xml130Api,
                                XmlGdykApi = xmlGdykApi
                            }
                        };

            return xmlBuildContext;
        }
    }
}
