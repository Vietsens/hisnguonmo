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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;


namespace HIS.Desktop.Plugins.TreatmentList.ADO
{
    class ExcellDataADO : V_HIS_TREATMENT_4
    {
        public string WORK_PLACE_NAME { get; set; }
        public string EXAM_STOMATOLOGY { get; set; }
        public string EXAM_ENT { get; set; }
        public string DISEASES { get; set; }
        public string EXAM_MUSCLE_BONE { get; set; }
        public string EXAM_DERMATOLOGY { get; set; }
        public string EXAM_RESPIRATORY { get; set; }
        public string TREATMENT_INSTRUCTION { get; set; }
        public string NOTE_SUPERSONIC { get; set; }
        public string NOTE_XRAY { get; set; }
        public string EXAM_EYE { get; set; }
        public string EXAM_SURGERY { get; set; }
        public string NOTE_BLOOD { get; set; }
        public string NOTE_BIOCHEMICAL { get; set; }
        public string EXAM_OEND { get; set; }
        public string NOTE_PROSTASE { get; set; }
        public string EXAM_MENTAL { get; set; }
        public string EXAM_NEUROLOGICAL { get; set; }
        public string EXAM_KIDNEY_UROLOGY { get; set; }
        public string EXAM_DIGESTION { get; set; }
        public string EXAM_CIRCULATION { get; set; }
        public string TDL_PATIENT_DOB_MEN { get; set; }
        public string TDL_PATIENT_DOB_WOM { get; set; }
        public string HEIGH_RANK_NAME { get; set; }
        public string EXAM_CONCLUSION { get; set; }
        public string CONCLUSION { get; set; }
        public decimal? HEIGHT { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? PULSE { get; set; }
        public decimal? BLOOD_PRESSURE_MAX { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public string TDL_PATIENT_POSITION_NAME { get; set; }
        // Cột bổ sung
        public string PHONE { get; set; }                 // SĐT
        public string EXAM_OBSTETRIC { get; set; }         // Sản
        public string OBSTETRIC_ICD_NAME { get; set; }     // Tên ICD sản (HIS_KSK_GENERAL.OBSTETRIC_DISEASE_ICD_NAME)
        public string CONCLUSION_ICD_CODE { get; set; }    // Kết luận khám (ICD) (HIS_KSK_GENERAL.CONCLUSION_ICD_CODE)
        public string CONCLUSION_ICD_NAME { get; set; }    // Kết luận chung tên (ICD) (HIS_KSK_GENERAL.CONCLUSION_ICD_NAME)
        public ExcellDataADO(V_HIS_TREATMENT_4 data, System.Collections.Concurrent.ConcurrentDictionary<long, string> workPlaceCache = null)
        {
            CommonParam paramCommon = new CommonParam();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_4>(this, data);
            // Cột "Số CMND": ưu tiên CCCD → CMND → PASSPORT
            this.TDL_PATIENT_CMND_NUMBER = FirstNotEmpty(
                GetStr(data, "TDL_PATIENT_CCCD_NUMBER"),
                GetStr(data, "TDL_PATIENT_CMND_NUMBER"),
                GetStr(data, "TDL_PATIENT_PASSPORT_NUMBER"));
            // Cột "SĐT": ưu tiên di động → cố định (lấy trong V_HIS_TREATMENT_4)
            this.PHONE = FirstNotEmpty(
                GetStr(data, "TDL_PATIENT_MOBILE"),
                GetStr(data, "TDL_PATIENT_PHONE"));
            if (data.TDL_PATIENT_GENDER_ID == 1)
            {
                TDL_PATIENT_DOB_WOM = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
            }
            else
            {
                TDL_PATIENT_DOB_MEN = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
            }
            if (data.TDL_PATIENT_POSITION_ID != null)
            {
                TDL_PATIENT_POSITION_NAME = BackendDataWorker.Get<HIS_POSITION>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_POSITION_ID).POSITION_NAME;
            }
            if (data.TDL_KSK_CONTRACT_ID != null)
            {
                long contractId = data.TDL_KSK_CONTRACT_ID.Value;
                string cachedWorkPlace;
                // Cả đoàn dùng chung 1 hợp đồng → tra cache theo contract ID để KHÔNG gọi API trùng cho từng bệnh nhân.
                if (workPlaceCache != null && workPlaceCache.TryGetValue(contractId, out cachedWorkPlace))
                {
                    WORK_PLACE_NAME = cachedWorkPlace;
                }
                else
                {
                    HisKskContractViewFilter filter = new HisKskContractViewFilter();
                    filter.ID = data.TDL_KSK_CONTRACT_ID;
                    var dataKskContract = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (dataKskContract != null && dataKskContract.Count > 0)
                        WORK_PLACE_NAME = dataKskContract.FirstOrDefault().WORK_PLACE_NAME;
                    if (workPlaceCache != null)
                        workPlaceCache[contractId] = WORK_PLACE_NAME;
                }
            }

            var dataSr = GetServiceReq(data.ID);
            if (dataSr != null && dataSr.Count > 0)
            {
                long serviceReqId = dataSr.OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault().ID;

                // Gọi API gộp GIỐNG EnterKskInfomantionVer2: 1 call api/HisKskSync/GetKskData lấy toàn bộ
                // dữ liệu KSK của lượt khám (general + trên-18 + nghề nghiệp + DHST...) thay cho nhiều call lẻ.
                MOS.SDO.HisKskDataSDO kskData = GetKskData(serviceReqId, data.ID);

                // HIS_KSK_GENERAL: luôn cần cho các cột ICD (ICD sản + ICD kết luận) và làm fallback phần khám.
                HIS_KSK_GENERAL currentGenaral = (kskData != null && kskData.HisKskGenerals != null)
                    ? kskData.HisKskGenerals.FirstOrDefault() : null;

                // Phần khám lâm sàng + vitals lấy theo loại KSK (dựa KSK_TYPE_ID như EnterKskInfomantionVer2):
                // KSK_TYPE_ID = 2 (trên 18 tuổi / nghề nghiệp) không lấy ở HIS_KSK_GENERAL mà ở bảng chuyên biệt.
                // Xác định bằng bảng nào có bản ghi trong SDO (ưu tiên trên-18 → nghề nghiệp → general).
                HIS_KSK_OVER_EIGHTEEN kskOverEighteen = (kskData != null && kskData.HisKskOverEighteens != null)
                    ? kskData.HisKskOverEighteens.FirstOrDefault() : null;
                HIS_KSK_OCCUPATIONAL kskOccupational = (kskData != null && kskData.HisKskOccupationals != null)
                    ? kskData.HisKskOccupationals.FirstOrDefault() : null;
                object examRecord = (object)kskOverEighteen ?? (object)kskOccupational ?? (object)currentGenaral;
                if (examRecord != null)
                {
                    EXAM_CIRCULATION = Pick(examRecord, currentGenaral, "EXAM_CIRCULATION");
                    EXAM_RESPIRATORY = Pick(examRecord, currentGenaral, "EXAM_RESPIRATORY");
                    EXAM_DIGESTION = Pick(examRecord, currentGenaral, "EXAM_DIGESTION");
                    EXAM_OEND = Pick(examRecord, currentGenaral, "EXAM_OEND");
                    EXAM_MUSCLE_BONE = Pick(examRecord, currentGenaral, "EXAM_MUSCLE_BONE");
                    EXAM_NEUROLOGICAL = Pick(examRecord, currentGenaral, "EXAM_NEUROLOGICAL");
                    EXAM_MENTAL = Pick(examRecord, currentGenaral, "EXAM_MENTAL");
                    EXAM_DERMATOLOGY = Pick(examRecord, currentGenaral, "EXAM_DERMATOLOGY");
                    EXAM_KIDNEY_UROLOGY = Pick(examRecord, currentGenaral, "EXAM_KIDNEY_UROLOGY");
                    EXAM_SURGERY = Pick(examRecord, currentGenaral, "EXAM_SURGERY");
                    EXAM_OBSTETRIC = Pick(examRecord, currentGenaral, "EXAM_OBSTETRIC");
                    // Tên field khác nhau: GENERAL dùng EXAM_EYE/EXAM_ENT/EXAM_STOMATOLOGY,
                    // bảng chuyên biệt dùng EXAM_EYE_DISEASE/EXAM_ENT_DISEASE/EXAM_STOMATOLOGY_DISEASE.
                    EXAM_EYE = Pick(examRecord, currentGenaral, "EXAM_EYE", "EXAM_EYE_DISEASE");
                    EXAM_ENT = Pick(examRecord, currentGenaral, "EXAM_ENT", "EXAM_ENT_DISEASE");
                    EXAM_STOMATOLOGY = Pick(examRecord, currentGenaral, "EXAM_STOMATOLOGY", "EXAM_STOMATOLOGY_DISEASE");
                    DISEASES = Pick(examRecord, currentGenaral, "DISEASES");
                    TREATMENT_INSTRUCTION = Pick(examRecord, currentGenaral, "TREATMENT_INSTRUCTION");
                    NOTE_BLOOD = Pick(examRecord, currentGenaral, "NOTE_BLOOD");
                    NOTE_BIOCHEMICAL = Pick(examRecord, currentGenaral, "NOTE_BIOCHEMICAL");
                    NOTE_PROSTASE = Pick(examRecord, currentGenaral, "NOTE_PROSTASE");
                    NOTE_SUPERSONIC = Pick(examRecord, currentGenaral, "NOTE_SUPERSONIC");
                    NOTE_XRAY = Pick(examRecord, currentGenaral, "NOTE_XRAY");

                    long? healthRankId = GetLong(examRecord, "HEALTH_EXAM_RANK_ID") ?? GetLong(currentGenaral, "HEALTH_EXAM_RANK_ID");
                    if (healthRankId != null)
                    {
                        var heighRank = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().FirstOrDefault(o => o.ID == healthRankId);
                        if (heighRank != null) HEIGH_RANK_NAME = heighRank.HEALTH_EXAM_RANK_NAME;
                    }

                    long? dhstId = GetLong(examRecord, "DHST_ID") ?? GetLong(currentGenaral, "DHST_ID");
                    if (dhstId != null && dhstId > 0)
                    {
                        HIS_DHST currentDhst = (kskData != null && kskData.HisDhsts != null)
                            ? kskData.HisDhsts.FirstOrDefault(o => o != null && o.ID == dhstId.Value) : null;
                        if (currentDhst != null)
                        {
                            HEIGHT = currentDhst.HEIGHT;
                            WEIGHT = currentDhst.WEIGHT;
                            VIR_BMI = currentDhst.VIR_BMI;
                            PULSE = currentDhst.PULSE;
                            BLOOD_PRESSURE_MAX = currentDhst.BLOOD_PRESSURE_MAX;
                            TEMPERATURE = currentDhst.TEMPERATURE;
                            BREATH_RATE = currentDhst.BREATH_RATE;
                        }
                    }
                }

                // Các cột ICD LUÔN lấy từ HIS_KSK_GENERAL (mọi loại KSK đều ghi ICD về bảng general).
                if (currentGenaral != null)
                {
                    OBSTETRIC_ICD_NAME = GetStr(currentGenaral, "OBSTETRIC_DISEASE_ICD_NAME");
                    CONCLUSION_ICD_CODE = GetStr(currentGenaral, "CONCLUSION_ICD_CODE");
                    CONCLUSION_ICD_NAME = GetStr(currentGenaral, "CONCLUSION_ICD_NAME");
                }

                CONCLUSION = dataSr.OrderByDescending(o => o.INTRUCTION_TIME).FirstOrDefault().CONCLUSION;
                EXAM_CONCLUSION = dataSr.OrderByDescending(o => o.INTRUCTION_TIME).FirstOrDefault().EXAM_CONCLUSION;
            }
        }

        /// <summary>
        /// Gọi API gộp api/HisKskSync/GetKskData -> HisKskDataSDO (chứa toàn bộ dữ liệu 1 lượt KSK:
        /// general, trên-18, nghề nghiệp, DHST...). Dùng chung cách lấy dữ liệu như EnterKskInfomantionVer2.
        /// </summary>
        private MOS.SDO.HisKskDataSDO GetKskData(long serviceReqId, long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisKskDataFilter filter = new MOS.Filter.HisKskDataFilter();
                filter.SERVICE_REQ_ID = serviceReqId;
                filter.TREATMENT_ID = treatmentId;
                return new BackendAdapter(param).Get<MOS.SDO.HisKskDataSDO>("api/HisKskSync/GetKskData", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đọc property string đầu tiên có giá trị (theo danh sách tên field ứng viên) — an toàn kiểu qua reflection.</summary>
        private static string GetStr(object o, params string[] names)
        {
            if (o == null) return null;
            foreach (var n in names)
            {
                try
                {
                    var p = o.GetType().GetProperty(n);
                    if (p != null)
                    {
                        var v = p.GetValue(o, null);
                        if (v != null)
                        {
                            string s = v.ToString();
                            if (!string.IsNullOrEmpty(s)) return s;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>Ưu tiên giá trị ở bản ghi chính (bảng chuyên biệt), thiếu thì lấy ở general.</summary>
        private static string Pick(object primary, object fallback, params string[] names)
        {
            string s = GetStr(primary, names);
            return !string.IsNullOrEmpty(s) ? s : GetStr(fallback, names);
        }

        /// <summary>Đọc property kiểu số (long) đầu tiên có giá trị — an toàn kiểu qua reflection.</summary>
        private static long? GetLong(object o, params string[] names)
        {
            if (o == null) return null;
            foreach (var n in names)
            {
                try
                {
                    var p = o.GetType().GetProperty(n);
                    if (p != null)
                    {
                        var v = p.GetValue(o, null);
                        if (v != null) return Convert.ToInt64(v);
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>Trả về giá trị chuỗi đầu tiên khác rỗng.</summary>
        private static string FirstNotEmpty(params string[] values)
        {
            if (values == null) return null;
            foreach (var v in values)
                if (!string.IsNullOrWhiteSpace(v)) return v;
            return null;
        }
        private List<V_HIS_SERVICE_REQ> GetServiceReq(long treatmentId)
        {
            List<V_HIS_SERVICE_REQ> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.HAS_EXECUTE = true;
                filter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }


    }
}
