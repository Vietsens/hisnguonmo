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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.XmlPrecheck.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Library.XmlPrecheck.Base
{
    /// <summary>
    /// vCong MDInsight ban 3.0 - Nap du lieu va dung dau vao bo ket xuat XML 130 cho MOT ho so.
    ///
    /// Day la ban MOT-HO-SO cua duong nap du lieu tai man Xuat XML 130 (LoadXmlBatchData + BuildInputAdoForXml):
    /// cung cac giao dien, cung bo loc dong dich vu, cung cach gom thuoc/vat tu da dung theo tung dong xuat kho.
    /// Khac biet duy nhat: khong loc theo dien doi tuong dong tuyen (man Ket thuc dieu tri khong co bo loc do),
    /// va khong co tuy chon XML 3176.
    ///
    /// ⚠️ Giu dong bo voi UCExportXml___XmlBuilder.cs cua man Xuat XML 130 khi ben do doi quy tac.
    /// </summary>
    internal class XmlPrecheckSingleLoader
    {
        internal const string KEY_QD130_CONNECTION_INFO = "HIS.QD_130_BYT.CONNECTION_INFO";
        internal const string KEY_QD130_XML1_MA_KHOA_OPTION = "HIS.QD_130_BYT.XML1.MA_KHOA_OPTION";

        /// <summary>Ho so vua nap - ban moi nhat tu may chu, khong phai ban dang mo tren man hinh</summary>
        internal V_HIS_TREATMENT_12 Treatment { get; private set; }

        /// <summary>Ly do nghiep vu khi khong dung duoc dau vao (khong chua du lieu benh nhan)</summary>
        internal string FailReason { get; private set; }

        /// <summary>
        /// Nap toan bo du lieu cua ho so va dung dau vao. Tra ve null khi khong dung duoc -
        /// xem <see cref="FailReason"/>.
        /// </summary>
        internal InputADO Load(long treatmentId)
        {
            try
            {
                this.FailReason = null;

                List<long> ids = new List<long> { treatmentId };

                HisTreatmentView12Filter treatmentFilter = new HisTreatmentView12Filter();
                treatmentFilter.IDs = ids;
                List<V_HIS_TREATMENT_12> treatments = Get<V_HIS_TREATMENT_12>("api/HisTreatment/GetView12", treatmentFilter);
                this.Treatment = treatments != null ? treatments.FirstOrDefault(o => o.ID == treatmentId) : null;
                if (this.Treatment == null)
                {
                    this.FailReason = Resources.ResourceMessageLang.MdInsightLyDoKhongKetXuatDuoc;
                    LogSystem.Warn("XmlPrecheck - Khong doc duoc ho so dieu tri " + treatmentId + " (view 12).");
                    return null;
                }

                HisSereServView2Filter sereServFilter = new HisSereServView2Filter();
                sereServFilter.TREATMENT_IDs = ids;
                List<V_HIS_SERE_SERV_2> sereServs = Get<V_HIS_SERE_SERV_2>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, sereServFilter)
                    ?? new List<V_HIS_SERE_SERV_2>();

                //Cung bo loc dong dich vu voi man Xuat XML 130 - va CHI lay dong cua dung ho so nay
                //(man Xuat XML 130 gom theo TDL_TREATMENT_ID sau khi goi API; khong dua vao bo loc cua API)
                List<V_HIS_SERE_SERV_2> usable = sereServs
                    .Where(o => o.AMOUNT > 0
                        && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.TDL_TREATMENT_ID.HasValue && o.TDL_TREATMENT_ID.Value == treatmentId
                        && ((o.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.PRICE > 0)
                            || o.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE))
                    .ToList();

                //Ghi ro tung lop loc de doc log la biet vi sao khong co dong nao (API tra thieu hay du lieu that)
                int ofRecord = sereServs.Count(o => o != null && o.TDL_TREATMENT_ID.HasValue && o.TDL_TREATMENT_ID.Value == treatmentId);
                int withAmount = sereServs.Count(o => o != null && o.TDL_TREATMENT_ID == treatmentId && o.AMOUNT > 0);
                int notExpend = sereServs.Count(o => o != null && o.TDL_TREATMENT_ID == treatmentId && o.AMOUNT > 0
                    && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                LogSystem.Info("XmlPrecheck - Ho so " + treatmentId + ": API tra " + sereServs.Count + " dong dich vu, thuoc ho so "
                    + ofRecord + ", so luong > 0: " + withAmount + ", khong phai hao phi: " + notExpend + ", du dieu kien dung tep: " + usable.Count + ".");

                if (usable.Count == 0)
                {
                    //Khong co dong dich vu nao thi khong dung duoc tep - giong lenh continue cua ban goc
                    this.FailReason = Resources.ResourceMessageLang.XmlPrecheckKhongCoDichVu;
                    LogSystem.Warn("XmlPrecheck - Ho so " + treatmentId + " khong co dong dich vu nao du dieu kien dung tep.");
                    return null;
                }

                XmlBuildContextADO ctx = new XmlBuildContextADO();
                ctx.DicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>> { { treatmentId, usable } };

                //⚠️ Moi danh sach deu loc lai theo ma ho so cua TUNG DONG (nhu man Xuat XML 130 gom theo khoa),
                //KHONG tin bo loc cua API: thuc te api/HisBaby/GetView tra ca dong khong thuoc ho so nay (chay thu 2026-10-13)
                ctx.DicPatientTypeAlter = One(treatmentId, Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetView",
                    new HisPatientTypeAlterViewFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);

                //Thuoc / vat tu da dung - gom theo dong xuat kho cua CHINH ho so nay (khong truyen ca lo)
                ctx.DicExpUsedByExpMestMedicineId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                ctx.DicExpUsedByExpMestMaterialId = new Dictionary<long, List<HIS_EXP_MEDIMATE_USED>>();
                LoadExpMedimateUsed(sereServs, ctx);

                //Ekip theo dong dich vu
                List<long> ekipIds = sereServs.Where(o => o.EKIP_ID.HasValue && o.EKIP_ID.Value != 0)
                    .Select(o => o.EKIP_ID.Value).Distinct().ToList();
                if (ekipIds.Count > 0)
                {
                    List<HIS_EKIP_USER> ekips = Get<HIS_EKIP_USER>("api/HisEkipUser/Get", new HisEkipUserFilter { EKIP_IDs = ekipIds });
                    if (ekips != null && ekips.Count > 0)
                    {
                        ctx.DicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>> { { treatmentId, ekips } };
                    }
                }

                ctx.DicBedLog = One(treatmentId, Get<V_HIS_BED_LOG>("api/HisBedLog/GetView", new HisBedLogViewFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);
                ctx.DicDhstList = One(treatmentId, Get<HIS_DHST>(HisRequestUriStore.HIS_DHST_GET, new HisDhstFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);
                ctx.DicTracking = One(treatmentId, Get<HIS_TRACKING>("api/HisTracking/Get", new HisTrackingFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);
                ctx.DicDebate = One(treatmentId, Get<HIS_DEBATE>("api/HisDebate/Get", new HisDebateFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);
                ctx.DicSereServTein = One(treatmentId, Get<V_HIS_SERE_SERV_TEIN>("api/HisSereServTein/GetView", new HisSereServTeinViewFilter { TDL_TREATMENT_IDs = ids }), o => o.TDL_TREATMENT_ID);
                ctx.DicSereServPttt = One(treatmentId, Get<V_HIS_SERE_SERV_PTTT>("api/HisSereServPttt/GetView", new HisSereServPtttViewFilter { TDL_TREATMENT_IDs = ids }), o => o.TDL_TREATMENT_ID);
                ctx.DicSereServSuin = One(treatmentId, Get<V_HIS_SERE_SERV_SUIN>("api/HisSereServSuin/GetView", new HisSereServSuinViewFilter { TDL_TREATMENT_IDs = ids }), o => o.TDL_TREATMENT_ID);
                ctx.DicBaby = One(treatmentId, Get<V_HIS_BABY>("api/HisBaby/GetView", new HisBabyViewFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);
                ctx.DicMedicalAssessment = One(treatmentId, Get<V_HIS_MEDICAL_ASSESSMENT>("api/HisMedicalAssessment/GetView", new HisMedicalAssessmentViewFilter { TREATMENT_IDs = ids }), o => o.TREATMENT_ID);

                List<HIS_HIV_TREATMENT> hivs = Get<HIS_HIV_TREATMENT>("api/HisHivTreatment/Get", new HisHivTreatmentFilter { TREATMENT_IDs = ids });
                hivs = hivs != null ? hivs.Where(o => o != null && o.TREATMENT_ID == treatmentId).ToList() : null;
                if (hivs != null && hivs.Count > 0)
                {
                    ctx.DicHivTreatment = new Dictionary<long, HIS_HIV_TREATMENT> { { treatmentId, hivs.OrderBy(o => o.ID).Last() } };
                }

                List<HIS_TUBERCULOSIS_TREAT> tbs = Get<HIS_TUBERCULOSIS_TREAT>("api/HisTuberculosisTreat/Get", new HisTuberculosisTreatFilter { TREATMENT_IDs = ids });
                tbs = tbs != null ? tbs.Where(o => o != null && o.TREATMENT_ID == treatmentId).ToList() : null;
                if (tbs != null && tbs.Count > 0)
                {
                    ctx.DicTuberculosisTreat = new Dictionary<long, HIS_TUBERCULOSIS_TREAT> { { treatmentId, tbs.Last() } };
                }

                //Tham so ket noi Cong tiep nhan BHXH: luong nay KHONG gui cong do, nhung bo dung tep van doc
                //mot so tham so tu day nen phai truyen day du - giong man Xuat XML 130
                string[] qd130 = ReadQd130ConnectionParts();
                ctx.TypeXml = qd130[3];
                ctx.ServerInfo = new ServerInfo
                {
                    Address = qd130[0],
                    Username = qd130[1],
                    Password = qd130[2],
                    TypeXml = qd130[3],
                    Xml130Api = qd130[4],
                    XmlGdykApi = qd130[5]
                };

                return BuildInputAdo(ctx, this.Treatment);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.FailReason = Resources.ResourceMessageLang.MdInsightLyDoKhongKetXuatDuoc;
                return null;
            }
        }

        /// <summary>Ban mot-ho-so cua BuildInputAdoForXml tai man Xuat XML 130 - giu nguyen tung buoc</summary>
        private static InputADO BuildInputAdo(XmlBuildContextADO ctx, V_HIS_TREATMENT_12 treatment)
        {
            InputADO ado = new InputADO();
            ado.Treatment = treatment;
            long id = treatment.ID;

            ado.ListPatientTypeAlter = Pick(ctx.DicPatientTypeAlter, id);
            ado.ListSereServ = Pick(ctx.DicSereServ, id);
            ado.ListDhst = Pick(ctx.DicDhstList, id);
            ado.ListSereServTein = Pick(ctx.DicSereServTein, id);
            ado.vSereServSuin = Pick(ctx.DicSereServSuin, id);
            ado.ListSereServPttt = Pick(ctx.DicSereServPttt, id);
            ado.ListBedLog = Pick(ctx.DicBedLog, id);
            ado.ListTracking = Pick(ctx.DicTracking, id);

            List<HIS_EKIP_USER> ekips = Pick(ctx.DicEkipUser, id);
            ado.ListEkipUser = ekips != null ? ekips.Distinct().ToList() : null;

            ado.ListDebate = Pick(ctx.DicDebate, id);
            ado.ListBaby = Pick(ctx.DicBaby, id);
            ado.ListMedicalAssessment = Pick(ctx.DicMedicalAssessment, id);

            if (ctx.DicHivTreatment != null && ctx.DicHivTreatment.ContainsKey(id))
            {
                ado.HivTreatment = ctx.DicHivTreatment[id];
            }

            ado.TotalMaterialTypeData = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
            ado.TotalHeinMediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>();
            ado.TotalConfigData = GetActiveConfigs();
            ado.TotalPatientTypeData = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
            ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
            ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
            ado.TotalEmployeeData = BackendDataWorker.Get<HIS_EMPLOYEE>();
            ado.TotalMachineData = BackendDataWorker.Get<HIS_MACHINE>();

            ado.ListExpMedimateUsed = BuildExpMedimateUsedOfTreatment(ctx, ado.ListSereServ);

            if (HisConfigs.Get<string>(KEY_QD130_XML1_MA_KHOA_OPTION) == "1")
            {
                ado.ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            }

            ado.serverInfo = ctx.ServerInfo;

            if (ctx.DicTuberculosisTreat != null && ctx.DicTuberculosisTreat.ContainsKey(id))
            {
                ado.TuberculosisTreat = ctx.DicTuberculosisTreat[id];
            }

            //Chot 2026-10-14: bat kiem so bo (khoa = 1) thi LUON dung tep theo chuan 3176 - he soat loi kiem theo 3176.
            //Man Ket thuc dieu tri khong co o "XML 3176" nhu man Xuat XML 130; de false thi XML3 ra MA_MAY kieu
            //nhom.nguon.ma CSKCB.serial va lech don gia/ty le -> he ngoai bao loi khong co that.
            ado.IS_3176 = true;

            return ado;
        }

        /// <summary>
        /// Loc danh sach thuoc/vat tu da dung CHI CUA MOT HO SO, theo dung cac dong dich vu cua ho so do -
        /// ban sao cua ham cung ten tai man Xuat XML 130.
        /// </summary>
        private static List<HIS_EXP_MEDIMATE_USED> BuildExpMedimateUsedOfTreatment(
            XmlBuildContextADO ctx, List<V_HIS_SERE_SERV_2> listSereServ)
        {
            List<HIS_EXP_MEDIMATE_USED> usedList = new List<HIS_EXP_MEDIMATE_USED>();
            try
            {
                if (listSereServ == null || listSereServ.Count == 0)
                {
                    return usedList;
                }

                foreach (V_HIS_SERE_SERV_2 sereServ in listSereServ)
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

                return usedList.GroupBy(o => o.ID).Select(g => g.First()).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return usedList;
            }
        }

        /// <summary>
        /// Nap HIS_EXP_MEDIMATE_USED theo dong xuat kho cua ho so - cung quy tac voi ThreadGetSereServ2
        /// tai man Xuat XML 130: thuoc loc theo danh sach id, mau/che pham mau loc tung id.
        /// </summary>
        private static void LoadExpMedimateUsed(List<V_HIS_SERE_SERV_2> sereServs, XmlBuildContextADO ctx)
        {
            try
            {
                Dictionary<long, HIS_EXP_MEDIMATE_USED> usedById = new Dictionary<long, HIS_EXP_MEDIMATE_USED>();

                List<long> medicineIds = sereServs
                    .Where(o => o != null && o.EXP_MEST_MEDICINE_ID.HasValue)
                    .Select(o => o.EXP_MEST_MEDICINE_ID.Value).Distinct().ToList();
                if (medicineIds.Count > 0)
                {
                    Collect(usedById, Get<HIS_EXP_MEDIMATE_USED>("api/HisExpMedimateUsed/Get",
                        new HisExpMedimateUsedFilter { EXP_MEST_MEDICINE_IDs = medicineIds }));
                }

                List<long> materialIds = sereServs
                    .Where(o => o != null && o.EXP_MEST_MATERIAL_ID.HasValue
                        && o.TDL_HEIN_SERVICE_TYPE_ID.HasValue
                        && (o.TDL_HEIN_SERVICE_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU
                            || o.TDL_HEIN_SERVICE_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM))
                    .Select(o => o.EXP_MEST_MATERIAL_ID.Value).Distinct().ToList();
                foreach (long materialId in materialIds)
                {
                    Collect(usedById, Get<HIS_EXP_MEDIMATE_USED>("api/HisExpMedimateUsed/Get",
                        new HisExpMedimateUsedFilter { EXP_MEST_MATERIAL_ID = materialId }));
                }

                foreach (HIS_EXP_MEDIMATE_USED u in usedById.Values)
                {
                    if (u.EXP_MEST_MEDICINE_ID.HasValue)
                    {
                        Add(ctx.DicExpUsedByExpMestMedicineId, u.EXP_MEST_MEDICINE_ID.Value, u);
                    }
                    if (u.EXP_MEST_MATERIAL_ID.HasValue)
                    {
                        Add(ctx.DicExpUsedByExpMestMaterialId, u.EXP_MEST_MATERIAL_ID.Value, u);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("XmlPrecheck - Load HIS_EXP_MEDIMATE_USED error: " + ex);
            }
        }

        private static void Collect(Dictionary<long, HIS_EXP_MEDIMATE_USED> usedById, List<HIS_EXP_MEDIMATE_USED> items)
        {
            if (items == null)
            {
                return;
            }
            foreach (HIS_EXP_MEDIMATE_USED u in items)
            {
                if (u != null)
                {
                    usedById[u.ID] = u;
                }
            }
        }

        private static void Add<T>(Dictionary<long, List<T>> dic, long key, T item)
        {
            List<T> list;
            if (!dic.TryGetValue(key, out list))
            {
                list = new List<T>();
                dic[key] = list;
            }
            list.Add(item);
        }

        /// <summary>
        /// Loc danh sach theo ma ho so cua TUNG DONG roi bo vao tu dien mot khoa; rong thi tra ve null nhu ban goc.
        /// Day la ban mot-ho-so cua cach man Xuat XML 130 gom dong theo TREATMENT_ID / TDL_TREATMENT_ID.
        /// </summary>
        private static Dictionary<long, List<T>> One<T>(long treatmentId, List<T> items, Func<T, long?> treatmentIdOf)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            List<T> mine = items.Where(o => o != null && treatmentIdOf(o) == treatmentId).ToList();
            if (mine.Count == 0)
            {
                return null;
            }
            return new Dictionary<long, List<T>> { { treatmentId, mine } };
        }

        private static List<T> Pick<T>(Dictionary<long, List<T>> dic, long key)
        {
            List<T> list;
            return dic != null && dic.TryGetValue(key, out list) ? list : null;
        }

        private static List<T> Get<T>(string uri, object filter)
        {
            CommonParam param = new CommonParam();
            return new BackendAdapter(param).Get<List<T>>(uri, ApiConsumers.MosConsumer, filter, param);
        }

        /// <summary>Danh sach cau hinh dang hieu luc - doc moi tu may chu nhu man Xuat XML 130</summary>
        private static List<HIS_CONFIG> GetActiveConfigs()
        {
            try
            {
                HisConfigFilter filter = new HisConfigFilter();
                filter.IS_ACTIVE = 1;
                return Get<HIS_CONFIG>("/api/HisConfig/Get", filter);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Doc sau thanh phan cua khoa HIS.QD_130_BYT.CONNECTION_INFO: dia chi, tai khoan, mat khau,
        /// loai tep XML, duong dan XML 130, duong dan XML giam dinh y khoa. Thieu thi tra chuoi rong.
        /// </summary>
        private static string[] ReadQd130ConnectionParts()
        {
            string[] parts = new string[6] { "", "", "", "", "", "" };
            try
            {
                string raw = HisConfigs.Get<string>(KEY_QD130_CONNECTION_INFO);
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return parts;
                }

                string[] source = raw.Split('|');
                for (int i = 0; i < parts.Length && i < source.Length; i++)
                {
                    parts[i] = (source[i] ?? "").Trim();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return parts;
        }
    }
}
