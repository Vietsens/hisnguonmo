using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000508.PDO;
using MPS.Processor.Mps000508.PDO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000508.ADO
{
    public class SereServADO : MPS.Processor.Mps000508.PDO.SereServKey
    {
        public long? HEIN_SERVICE_TYPE_CHILD_NUM_ORDER { get; set; }

        public decimal PRICE_VP { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }

        #region Gom nhóm theo khoa / phòng xử lý (ExeRoom) - tương tự Mps000512 / Mps000304
        public long GROUP_ROOM_ID { get; set; }
        public string GROUP_ROOM_CODE { get; set; }
        public string GROUP_ROOM_NAME { get; set; }
        public long GROUP_DEPARTMENT_ID { get; set; }
        public string GROUP_DEPARTMENT_CODE { get; set; }
        public string GROUP_DEPARTMENT_NAME { get; set; }
        #endregion 

        private static string GetHeinServiceTypeName697(HIS_HEIN_SERVICE_TYPE heinServiceType) 
        {
            try
            {
                if (heinServiceType == null) return null;
                var prop = heinServiceType.GetType().GetProperty("HEIN_SERVICE_TYPE_NAME_697");
                if (prop != null)
                {
                    var v = prop.GetValue(heinServiceType, null) as string;
                    if (!String.IsNullOrWhiteSpace(v)) return v;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private static string ResolveHeinServiceTypeName(HIS_HEIN_SERVICE_TYPE heinServiceType)
        {
            string name697 = GetHeinServiceTypeName697(heinServiceType);
            if (!String.IsNullOrWhiteSpace(name697)) return name697;
            return heinServiceType != null ? heinServiceType.HEIN_SERVICE_TYPE_NAME : null;
        }

        public SereServADO(HIS_SERE_SERV data, List<HIS_SERE_SERV> SereServs, List<HIS_SERE_SERV_EXT> sereServExts,
            List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes, List<V_HIS_SERVICE> services, List<HIS_DEPARTMENT> departments, List<V_HIS_ROOM> rooms, List<HIS_MEDICINE_TYPE> medicineTypes,
            List<HIS_MEDICINE_LINE> medicineLines, List<HIS_MATERIAL_TYPE> materialTypes, PatientTypeCFG patientTypeCFG,
            HisConfigValue hisConfigValue, List<HIS_SERVICE_UNIT> hisServiceUnit, V_HIS_TREATMENT treatment,
            List<HIS_SERVICE_REQ> serviceReqs, List<HIS_PATIENT_TYPE_ALTER> ListPta
            )
        {
            try
            {
                #region Base
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (heinServiceTypes != null && heinServiceTypes.Count > 0 && services != null && services.Count > 0)
                {
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                    if (service != null)
                    {
                        HIS_HEIN_SERVICE_TYPE heinServiceType = heinServiceTypes.FirstOrDefault(o => o.ID == service.HEIN_SERVICE_TYPE_ID);
                        this.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                        this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        this.SERVICE_NAME = service.SERVICE_NAME;
                        this.SERVICE_CODE = service.SERVICE_CODE;

                        this.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        this.ACTIVE_INGR_BHYT_CODE = service.ACTIVE_INGR_BHYT_CODE;
                        this.ACTIVE_INGR_BHYT_NAME = service.ACTIVE_INGR_BHYT_NAME;
                        this.HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                        this.HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
                        this.CONCENTRA = service.CONCENTRA;

                        HIS_SERE_SERV_EXT sereServExt = sereServExts != null ? sereServExts.Where(o => o.SERE_SERV_ID == data.ID).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault() : null;
                        if (sereServExt != null && (sereServExt.NUMBER_OF_FILM ?? 0) > 0)
                            this.NUMBER_OF_FILM = sereServExt.NUMBER_OF_FILM;
                        else
                            this.NUMBER_OF_FILM = service.NUMBER_OF_FILM;

                        if (heinServiceType != null)
                        {
                            if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM
    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeTH = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM);

                                this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.THUOC_TRUYENDICH__ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeTH.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeTH.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.THUOC_TRUYENDICH__NAME;
                            }
                            else if (this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                if (this.PARENT_ID.HasValue
                                    && (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                                    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL))
                                {
                                    HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT);
                                    this.HEIN_SERVICE_TYPE_ID = this.PARENT_ID;
                                    this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.VIR_PARENT_NUM_ORDER;
                                    this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                                    this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                }
                                else if (this.PARENT_ID.HasValue && service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM)
                                {
                                    var parent = SereServs.FirstOrDefault(o => o.ID == this.PARENT_ID.Value);
                                    if (parent == null)
                                    {
                                        LogSystem.Warn("Vat tu trong goi co ParentId nhung khong lay duoc parent. Id: " + this.ID);
                                    }
                                    else if ((parent.TDL_EXECUTE_ROOM_ID == this.TDL_REQUEST_ROOM_ID) || hisConfigValue.IsNotSameDepartment)
                                    {
                                        HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT);
                                        this.HEIN_SERVICE_TYPE_ID = this.PARENT_ID;
                                        this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.VIR_PARENT_NUM_ORDER;
                                        this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                                        this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                    }
                                    else
                                    {
                                        HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM);
                                        this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.VT_Y_TE__ID;
                                        this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.VIR_PARENT_NUM_ORDER;
                                        this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                                        this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.VT_Y_TE__NAME;
                                    }
                                }
                                else
                                {
                                    HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM);
                                    this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.VT_Y_TE__ID;
                                    this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.VIR_PARENT_NUM_ORDER;
                                    this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                                    this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.VT_Y_TE__NAME;
                                }
                            }
                            else if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypePTTT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeTT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT);

                                string namePTTT = ResolveHeinServiceTypeName(heinServiceTypePTTT);
                                string nameTT = ResolveHeinServiceTypeName(heinServiceTypeTT);

                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypePTTT.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypePTTT.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypePTTT.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypePTTT.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = nameTT.First().ToString().ToUpper() + nameTT.ToLower().Substring(1) + ", " + namePTTT.ToLower();
                            }
                            else if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU
                                    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeMAU = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeCPM = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM);

                                string nameMAU = ResolveHeinServiceTypeName(heinServiceTypeMAU);
                                string nameCPM = ResolveHeinServiceTypeName(heinServiceTypeCPM);

                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypeMAU.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeMAU.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeMAU.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypeMAU.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = nameMAU.First().ToString().ToUpper() + nameMAU.ToLower().Substring(1) + ", " + nameCPM.ToLower();
                            }
                            else
                            {
                                this.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceType.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceType.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = ResolveHeinServiceTypeName(heinServiceType);
                            }
                        }

                        if (medicineTypes != null && medicineTypes.Count > 0 && medicineLines != null && medicineLines.Count > 0)
                        {
                            HIS_MEDICINE_TYPE medicineType = medicineTypes.FirstOrDefault(o => o.SERVICE_ID == this.SERVICE_ID);
                            if (medicineType != null && medicineType.MEDICINE_LINE_ID.HasValue)
                            {
                                HIS_MEDICINE_LINE medicineLine = medicineLines.FirstOrDefault(o => o.ID == medicineType.MEDICINE_LINE_ID);
                                if (medicineLine != null)
                                {
                                    this.MEDICINE_LINE_ID = medicineLine.ID;
                                    this.MEDICINE_LINE_CODE = medicineLine.MEDICINE_LINE_CODE;
                                    this.MEDICINE_LINE_NAME = medicineLine.MEDICINE_LINE_NAME;
                                }

                            }
                        }
                    }
                }

                // Dịch vụ không xác định được loại hình BHYT (HEIN_SERVICE_TYPE_ID null: service không có loại, hoặc không tra được danh mục)
                // -> gom vào nhóm "Khác". Gán ID/Name/NumOrder để chảy đúng vào dedup + gom nhóm + quan hệ template (ID khớp HEIN_SERVICE_TYPE_ID).
                if (!this.HEIN_SERVICE_TYPE_ID.HasValue)
                {
                    this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.DV_KHAC__ID;
                    this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.DV_KHAC__NAME;
                    this.HEIN_SERVICE_TYPE_NUM_ORDER = 12; // số lớn để "Khác" xếp cuối; chỉnh nếu cần thứ tự khác
                }

                // Band chi tiết in tên dịch vụ thường (SERVICE_NAME), không in tên BHYT. Đảm bảo SERVICE_NAME luôn có giá trị 
                // (fallback TDL_SERVICE_NAME khi service không tra được danh mục) để không bị trống tên.
                if (string.IsNullOrWhiteSpace(this.SERVICE_NAME))
                    this.SERVICE_NAME = this.TDL_SERVICE_NAME;

                if (rooms != null && rooms.Count > 0)
                {
                    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == data.TDL_EXECUTE_ROOM_ID);
                    if (room != null)
                    {
                        this.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        this.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                    }

                    // Gom theo phòng xử lý - logic giống Mps000304:
                    // dịch vụ Khám (KH) lấy phòng THỰC HIỆN, các dịch vụ còn lại lấy phòng CHỈ ĐỊNH; khoa lấy theo phòng đó.  
                    V_HIS_ROOM groupRoom = this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH
                        ? rooms.FirstOrDefault(o => o.ID == data.TDL_EXECUTE_ROOM_ID)
                        : rooms.FirstOrDefault(o => o.ID == data.TDL_REQUEST_ROOM_ID);
                    if (groupRoom != null)
                    {
                        this.GROUP_ROOM_ID = groupRoom.ID;
                        this.GROUP_ROOM_CODE = groupRoom.ROOM_CODE;
                        this.GROUP_ROOM_NAME = groupRoom.ROOM_NAME;

                        this.GROUP_DEPARTMENT_ID = groupRoom.DEPARTMENT_ID;
                        this.GROUP_DEPARTMENT_CODE = groupRoom.DEPARTMENT_CODE;
                        this.GROUP_DEPARTMENT_NAME = groupRoom.DEPARTMENT_NAME;
                    }
                }
                #endregion

                string keyPaty = "";
                this.PatientTypeAlter = PatientTypeAlterProcessor.GetPatientTypeAlter(data, patientTypeCFG, treatment.TDL_TREATMENT_TYPE_ID ?? 0, ref keyPaty);
                this.KEY_PATY_ALTER = keyPaty;
                if (hisConfigValue != null && hisConfigValue.IsGroupHeinServiceByUseTime)
                {
                    if (this.SERVICE_REQ_ID.HasValue && serviceReqs != null && serviceReqs.Count > 0 && ListPta != null && ListPta.Count > 0)
                    {
                        List<long> donTypeIds = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        HIS_SERVICE_REQ sr = serviceReqs.FirstOrDefault(x => x.ID == this.SERVICE_REQ_ID.Value && donTypeIds.Contains(x.SERVICE_REQ_TYPE_ID));
                        long useTime = (sr != null && sr.USE_TIME.HasValue) ? sr.USE_TIME.Value : 0;
                        if (useTime > 0)
                        {
                            HIS_PATIENT_TYPE_ALTER ptaApplied = ListPta
                                .Where(x =>
                                    (!x.HEIN_CARD_FROM_TIME.HasValue || x.HEIN_CARD_FROM_TIME.Value <= useTime)
                                    && (!x.HEIN_CARD_TO_TIME.HasValue || x.HEIN_CARD_TO_TIME.Value >= useTime))
                                .OrderByDescending(x => x.HEIN_CARD_FROM_TIME ?? 0)
                                .ThenByDescending(x => x.LOG_TIME)
                                .FirstOrDefault();
                            if (ptaApplied != null)
                            {
                                this.PatientTypeAlter = ptaApplied;
                                this.KEY_PATY_ALTER = PatientTypeAlterProcessor.ToString(ptaApplied, this, treatment.TDL_TREATMENT_TYPE_ID ?? 0);
                            }
                        }
                    }
                }
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue && this.AMOUNT > 0)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                // Tỷ lệ thanh toán - GIỐNG Mps000304 (SereServADO.cs:280-294): làm tròn TỶ LỆ 2 số rồi ×100
                // (KHÔNG phải Math.Round(100*tỷ_lệ,2)) -> phần trăm tròn, khớp 304; tránh lệch tiền tự trả ở DV vượt trần.
                decimal t = 0;
                int sp = 2;
                if (this.STENT_ORDER != null)
                {
                    sp = 4;
                }
                if (this.ORIGINAL_PRICE > 0)
                {
                    if (this.HEIN_LIMIT_PRICE.HasValue)
                        t = 100 * Math.Round(this.HEIN_LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), sp);
                    else if (this.LIMIT_PRICE.HasValue)
                        t = 100 * Math.Round(this.LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), sp);
                    else
                        t = 100 * Math.Round(this.PRICE / this.ORIGINAL_PRICE, sp);
                }

                if (this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    || this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    this.SERVICE_PAY_RATE = 100;
                    this.BHYT_PAY_RATE = t;
                }
                else
                {
                    this.SERVICE_PAY_RATE = t;
                    this.BHYT_PAY_RATE = 100;
                }

                this.PRICE_BHYT = PriceBHYTProcess(this, materialTypes);
                this.TOTAL_PRICE_BHYT = (this.PRICE_BHYT * this.AMOUNT) * ((this.BHYT_PAY_RATE ?? 0) / 100) * ((this.SERVICE_PAY_RATE ?? 0) / 100);

                if (this.PRIMARY_PATIENT_TYPE_ID.HasValue)
                {
                    this.PRIMARY_PRICE = this.LIMIT_PRICE;
                }

                if (!this.PRIMARY_PRICE.HasValue)
                    this.PRIMARY_PRICE = this.VIR_PRICE;
                else
                    this.PRIMARY_PRICE = Math.Round((this.PRIMARY_PRICE ?? 0) * (1 + this.VAT_RATIO), 4, MidpointRounding.AwayFromZero);

                if (hisConfigValue != null
                    && !hisConfigValue.IsPriceWithDifference
                    && this.PRIMARY_PRICE > this.PRICE_BHYT
                    && this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                {
                    this.PRIMARY_PRICE = this.PRICE_BHYT;
                }

                this.OTHER_SOURCE_PRICE = (this.OTHER_SOURCE_PRICE ?? 0) * this.AMOUNT;

                this.VIR_TOTAL_PRICE_NO_EXPEND = this.PRIMARY_PRICE * this.AMOUNT;
                this.TOTAL_PRICE_PATIENT_SELF = (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) * ((this.SERVICE_PAY_RATE ?? 0) / 100) - (this.VIR_TOTAL_HEIN_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);
                if (hisConfigValue.IsSurgPriceOption_1 && (this.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || this.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT))
                {
                    TOTAL_PRICE_PATIENT_SELF = (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) - (this.VIR_TOTAL_HEIN_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);
                }

                if (this.TOTAL_PRICE_PATIENT_SELF < 0)
                    this.TOTAL_PRICE_PATIENT_SELF = 0;

                this.TOTAL_PRICE_PATIENT_NO_PAY_RATE = (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) - (this.VIR_TOTAL_HEIN_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);

                this.PRICE_VP = this.VIR_PRICE ?? 0;
                this.TOTAL_PRICE_VP = this.PRICE_VP * this.AMOUNT;

                // Người bệnh tự trả = (tổng viện phí − tổng tiền theo giá BHYT) + (tổng tiền theo giá BHYT − Quỹ BHYT − cùng chi trả − nguồn khác)
                //  = chênh lệch ngoài phạm vi BHYT + phần tự trả trong phạm vi BHYT. KHÔNG nhân tỷ lệ thanh toán.
                decimal chenhLechNgoaiBhyt = this.TOTAL_PRICE_VP - (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                decimal tuTraTrongBhyt = (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) - (this.VIR_TOTAL_HEIN_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);
                this.TOTAL_PATIENT_PRICE_LEFT = chenhLechNgoaiBhyt + tuTraTrongBhyt;

                if (this.TOTAL_PATIENT_PRICE_LEFT < 0)
                    this.TOTAL_PATIENT_PRICE_LEFT = 0;

                var svUnit = hisServiceUnit.FirstOrDefault(o => o.ID == this.TDL_SERVICE_UNIT_ID);
                if (svUnit != null && svUnit.CONVERT_RATIO.HasValue && this.USE_ORIGINAL_UNIT_FOR_PRES != 1 && svUnit.CONVERT_RATIO.Value != 0)
                {
                    var convertUnit = hisServiceUnit.FirstOrDefault(o => o.ID == svUnit.CONVERT_ID);
                    if (convertUnit != null)
                    {
                        this.SERVICE_UNIT_CODE = convertUnit.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = convertUnit.SERVICE_UNIT_NAME;
                    }

                    this.AMOUNT = this.AMOUNT * svUnit.CONVERT_RATIO.Value;
                    this.PRICE = PRICE / svUnit.CONVERT_RATIO.Value;
                    this.PRIMARY_PRICE = (PRIMARY_PRICE ?? 0) / svUnit.CONVERT_RATIO.Value;
                    this.PRICE_BHYT = PRICE_BHYT / svUnit.CONVERT_RATIO.Value;
                    this.PRICE_VP = PRICE_VP / svUnit.CONVERT_RATIO.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bản sao NÔNG. Dùng khi 1 nguồn (sereServADOTemps) nuôi nhiều bộ gom khác grain
        /// (theo phòng / theo khoa) - phải clone trước khi mutate để tránh cộng dồn kép do dùng chung tham chiếu.
        /// </summary>
        public SereServADO Clone()
        {
            return (SereServADO)this.MemberwiseClone();
        }

        private decimal? GetBHYTPayRate(SereServADO s)
        {
            decimal? result = null;
            try
            {
                if (!s.HEIN_LIMIT_PRICE.HasValue || s.ORIGINAL_PRICE > s.HEIN_LIMIT_PRICE)
                {
                    result = 100;
                }
                else
                {
                    result = Math.Round((s.ORIGINAL_PRICE / s.HEIN_LIMIT_PRICE.Value) * 100, 2);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public decimal PriceBHYTProcess(SereServADO ss, List<HIS_MATERIAL_TYPE> materialTypes)
        {
            decimal priceBHYT = 0;
            try
            {
                if (!String.IsNullOrEmpty(ss.HEIN_CARD_NUMBER))
                {
                    if (ss.PatientTypeAlter != null)
                    {
                        if (ss.VIR_TOTAL_HEIN_PRICE > 0)
                        {
                            priceBHYT = Math.Round(ss.ORIGINAL_PRICE * (1 + ss.VAT_RATIO), 4, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            priceBHYT = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                priceBHYT = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return priceBHYT;
        }
    }
}
