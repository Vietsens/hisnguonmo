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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000512.PDO;
using MPS.Processor.Mps000512.PDO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000512.ADO
{
    public class SereServADO : HIS_SERE_SERV
    {
        public long SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public long? HEIN_SERVICE_TYPE_NUM_ORDER { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public long? NUMBER_OF_FILM { get; set; }

        public int ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_PATIENT_SELF { get; set; }
        public decimal RADIO_SERIVCE { get; set; }
        public decimal? TOTAL_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_HEIN_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PRICE_ROOM { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_ROOM { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ROOM { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; }

        public long? MEDICINE_LINE_ID { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }

        public HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public string KEY_PATY_ALTER { get; set; }
        public decimal? SERVICE_PAY_RATE { get; set; }
        public decimal? BHYT_PAY_RATE { get; set; }
        public long? HEIN_SERVICE_TYPE_PARENT_1_ID { get; set; } //Cap 1 "Giuong"

        public decimal PRICE_VP { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public long? HEIN_SERVICE_TYPE_CHILD_NUM_ORDER { get; set; }
        public long IS_PAID { get; set; }

        public bool IsHide { get; set; }

        // ===== Chiều gom nhóm khoa / phòng (YC: lấy theo khoa-phòng, port từ Mps000510) =====
        public long GROUP_DEPARTMENT_ID { get; set; }
        public string GROUP_DEPARTMENT_CODE { get; set; }
        public string GROUP_DEPARTMENT_NAME { get; set; }
        public long GROUP_ROOM_ID { get; set; }
        public string GROUP_ROOM_CODE { get; set; }
        public string GROUP_ROOM_NAME { get; set; }

        // Giá gói (giá gói - "PACKAGE_PRICE") đã có sẵn trên HIS_SERE_SERV (kế thừa) và được copy từ dòng gốc,
        // nên template dùng trực tiếp {PACKAGE_PRICE} trên bộ Service. Không khai báo lại để tránh che cột gốc.

        /// <summary>
        /// Dựng 1 dòng dịch vụ. Mọi tra cứu danh mục đi qua <paramref name="lk"/> (Dictionary O(1)) thay cho
        /// FirstOrDefault O(n) của Mps000302 — kết quả tính toán giữ nguyên 100%.
        /// </summary>
        public SereServADO(HIS_SERE_SERV data, SereServLookup lk, PatientTypeCFG patientTypeCFG, HisConfigValue hisConfigValue,
            V_HIS_TREATMENT treatment, List<HIS_PATIENT_TYPE_ALTER> ListPta, bool groupSuatAn)
        {
            try
            {
                #region Base
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (lk.HeinTypeById.Count > 0 && lk.ServiceById.Count > 0)
                {
                    V_HIS_SERVICE service = SereServLookup.Get(lk.ServiceById, data.SERVICE_ID);
                    if (service != null)
                    {
                        HIS_HEIN_SERVICE_TYPE heinServiceType = SereServLookup.Get(lk.HeinTypeById, service.HEIN_SERVICE_TYPE_ID);
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

                        //Set tong so film
                        HIS_SERE_SERV_EXT sereServExt = lk.SereServExtBySereServId[Convert.ToInt64(data.ID)].OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                        if (sereServExt != null && (sereServExt.NUMBER_OF_FILM ?? 0) > 0)
                            this.NUMBER_OF_FILM = sereServExt.NUMBER_OF_FILM;
                        else
                            this.NUMBER_OF_FILM = service.NUMBER_OF_FILM;

                        //Custom lại nhóm không có trong danh mục trong cơ sở dữ liệu
                        if (heinServiceType != null)
                        {
                            if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM
    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeTH = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM);

                                this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.THUOC_TRUYENDICH__ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeTH.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.THUOC_TRUYENDICH__NAME;
                            }
                            else if (this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !this.PARENT_ID.HasValue)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM);
                                this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.VT_Y_TE__ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.VT_Y_TE__NAME;
                            }
                            else if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypePTTT = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeTT = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT);
                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypePTTT.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypePTTT.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypePTTT.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypePTTT.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceTypeTT.HEIN_SERVICE_TYPE_NAME.First().ToString().ToUpper() + heinServiceTypeTT.HEIN_SERVICE_TYPE_NAME.ToLower().Substring(1) + ", " + heinServiceTypePTTT.HEIN_SERVICE_TYPE_NAME.ToLower();
                            }
                            else if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU
                                    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeMAU = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeCPM = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM);
                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypeMAU.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeMAU.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeMAU.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypeMAU.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceTypeMAU.HEIN_SERVICE_TYPE_NAME.First().ToString().ToUpper() + heinServiceTypeMAU.HEIN_SERVICE_TYPE_NAME.ToLower().Substring(1) + ", " + heinServiceTypeCPM.HEIN_SERVICE_TYPE_NAME.ToLower();
                            }
                            else
                            {
                                this.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceType.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_NAME;
                            }
                        }
                        if (this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && this.PARENT_ID.HasValue)
                        {
                            HIS_HEIN_SERVICE_TYPE heinServiceTypeVT = SereServLookup.Get(lk.HeinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT);
                            this.HEIN_SERVICE_TYPE_ID = this.PARENT_ID;
                            this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeVT.NUM_ORDER;
                            this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                        }
                        if (lk.MedicineTypeByServiceId.Count > 0 && lk.MedicineLineById.Count > 0)
                        {
                            HIS_MEDICINE_TYPE medicineType = SereServLookup.Get(lk.MedicineTypeByServiceId, this.SERVICE_ID);
                            if (medicineType != null && medicineType.MEDICINE_LINE_ID.HasValue)
                            {
                                HIS_MEDICINE_LINE medicineLine = SereServLookup.Get(lk.MedicineLineById, medicineType.MEDICINE_LINE_ID);
                                if (medicineLine != null)
                                {
                                    this.MEDICINE_LINE_ID = medicineLine.ID;
                                    this.MEDICINE_LINE_CODE = medicineLine.MEDICINE_LINE_CODE;
                                    this.MEDICINE_LINE_NAME = medicineLine.MEDICINE_LINE_NAME;
                                }
                            }
                        }

                        if (groupSuatAn && service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN && lk.PatientTypeById.Count > 0)
                        {
                            var patientType = SereServLookup.Get(lk.PatientTypeById, this.PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                //gán giá trị âm để không trùng dịch vụ khác
                                this.SERVICE_ID = -patientType.ID;
                                this.HEIN_SERVICE_BHYT_CODE = patientType.PATIENT_TYPE_CODE;
                                this.HEIN_SERVICE_BHYT_NAME = patientType.PATIENT_TYPE_NAME;
                                this.SERVICE_CODE = patientType.PATIENT_TYPE_CODE;
                                this.SERVICE_NAME = patientType.PATIENT_TYPE_NAME;
                                this.TDL_SERVICE_CODE = patientType.PATIENT_TYPE_CODE;
                                this.TDL_SERVICE_NAME = patientType.PATIENT_TYPE_NAME;
                            }
                        }
                    }
                }

                if (lk.RoomById.Count > 0)
                {
                    V_HIS_ROOM room = SereServLookup.Get(lk.RoomById, data.TDL_EXECUTE_ROOM_ID);
                    if (room != null)
                    {
                        this.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        this.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        this.GROUP_ROOM_ID = room.ID;
                        this.GROUP_ROOM_CODE = room.ROOM_CODE;
                        this.GROUP_ROOM_NAME = room.ROOM_NAME;
                    }
                }

                //Khoa xử lý (khoa thực hiện) - phục vụ lọc/gom nhóm theo khoa (giống Mps000510)
                if (lk.DeptById.Count > 0)
                {
                    HIS_DEPARTMENT executeDepartment = SereServLookup.Get(lk.DeptById, data.TDL_EXECUTE_DEPARTMENT_ID);
                    if (executeDepartment != null)
                    {
                        this.GROUP_DEPARTMENT_ID = executeDepartment.ID;
                        this.GROUP_DEPARTMENT_CODE = executeDepartment.DEPARTMENT_CODE;
                        this.GROUP_DEPARTMENT_NAME = executeDepartment.DEPARTMENT_NAME;
                    }
                }
                #endregion

                string keyPaty = "";
                this.PatientTypeAlter = PatientTypeAlterProcessor.GetPatientTypeAlter(data, patientTypeCFG, treatment.TDL_TREATMENT_TYPE_ID ?? 0, ref keyPaty);
                this.KEY_PATY_ALTER = keyPaty;

                //142913 theo đúng việc. Nếu có đối tượng khác BHYT mà có thông tin JSON_PATIENT_TYPE_ALTER thì xóa thông tin PatientTypeAlter
                if (data.PATIENT_TYPE_ID != patientTypeCFG.PATIENT_TYPE__BHYT && PatientTypeAlter != null)
                {
                    this.KEY_PATY_ALTER = null;
                    this.PatientTypeAlter = null;
                }

                if (hisConfigValue != null && hisConfigValue.IsMergeServiceNotHein)
                {
                    if (String.IsNullOrWhiteSpace(this.KEY_PATY_ALTER) && ListPta.Count > 0)
                    {
                        var pta = ListPta.FirstOrDefault(o => o.LOG_TIME <= this.TDL_INTRUCTION_TIME);
                        if (pta == null) pta = ListPta.First();

                        this.KEY_PATY_ALTER = PatientTypeAlterProcessor.ToString(pta, data, treatment.TDL_TREATMENT_TYPE_ID ?? 0);
                        this.PatientTypeAlter = pta;
                    }
                }
                if (hisConfigValue != null && hisConfigValue.IsGroupHeinServiceByUseTime)
                {
                    if (this.SERVICE_REQ_ID.HasValue && lk.ServiceReqById.Count > 0 && ListPta != null && ListPta.Count > 0)
                    {
                        HIS_SERVICE_REQ sr = SereServLookup.Get(lk.ServiceReqById, this.SERVICE_REQ_ID.Value);
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
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                decimal? t = null;
                if (this.HEIN_LIMIT_PRICE.HasValue)
                {
                    t = 100 * Math.Round(this.HEIN_LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), 2);
                }
                else if (this.LIMIT_PRICE.HasValue)
                {
                    t = 100 * Math.Round(this.LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), 2);
                }
                else
                {
                    t = 100 * Math.Round(this.PRICE / this.ORIGINAL_PRICE, 2);
                }

                //Ty le thanh toan dich vu
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

                this.PRICE_BHYT = PriceBHYTProcess(this);
                this.TOTAL_PRICE_BHYT = (this.PRICE_BHYT * this.AMOUNT) * ((this.BHYT_PAY_RATE ?? 0) / 100) * ((this.SERVICE_PAY_RATE ?? 0) / 100);

                if (!this.PRIMARY_PRICE.HasValue)
                    this.PRIMARY_PRICE = this.VIR_PRICE;
                else
                    this.PRIMARY_PRICE = Math.Round((this.PRIMARY_PRICE ?? 0) * (1 + this.VAT_RATIO), 4, MidpointRounding.AwayFromZero);

                //Có lấy giá chênh lệch vào đơn giá BV của loại dịch vụ: khám trong bảng kê 6556
                //this.PRIMARY_PRICE > this.PRICE_BHYT -> Co chenh lech
                if (hisConfigValue != null
                    && !hisConfigValue.IsPriceWithDifference
                    && this.PRIMARY_PRICE > this.PRICE_BHYT
                    && this.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                {
                    this.PRIMARY_PRICE = this.PRICE_BHYT;
                }

                this.OTHER_SOURCE_PRICE = (this.OTHER_SOURCE_PRICE ?? 0) * this.AMOUNT;

                if (this.IS_EXPEND == 1 && !groupSuatAn)
                {
                    this.PRIMARY_PRICE = 0;
                }

                this.VIR_TOTAL_PRICE_NO_EXPEND = this.PRIMARY_PRICE * this.AMOUNT;
                this.TOTAL_PRICE_PATIENT_SELF = (this.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) * ((this.SERVICE_PAY_RATE ?? 0) / 100) - (this.VIR_TOTAL_HEIN_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);
                if (this.TOTAL_PRICE_PATIENT_SELF < 0) this.TOTAL_PRICE_PATIENT_SELF = 0;

                this.PRICE_VP = this.VIR_PRICE ?? 0;
                this.TOTAL_PRICE_VP = this.VIR_TOTAL_PRICE ?? 0;
                this.TOTAL_PATIENT_PRICE_LEFT = (this.VIR_TOTAL_PATIENT_PRICE ?? 0) - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) - (this.OTHER_SOURCE_PRICE ?? 0);

                if (this.SERVICE_PAY_RATE < 100 && this.SERVICE_PAY_RATE > 0)
                {
                    //nếu là khám có tỉ lệ thanh toán khác 100 thì tính lại đơn giá
                    if (this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH
                        || this.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                    {
                        this.PRICE_VP = this.PRICE_VP / ((this.SERVICE_PAY_RATE ?? 0) / 100);
                    }
                    else if (HeinServiceTypeExt.HEIN_BED__IDs.Contains(this.HEIN_SERVICE_TYPE_ID ?? 0))
                    {
                        //giường nằm ghép không có giới hạn thì tính lại giá.
                        if (this.SHARE_COUNT.HasValue && !this.HEIN_LIMIT_PRICE.HasValue)
                        {
                            this.PRICE_VP = this.PRICE_VP / ((this.SERVICE_PAY_RATE ?? 0) / 100);
                        }
                    }
                }

                if (this.TOTAL_PATIENT_PRICE_LEFT < 0)
                    this.TOTAL_PATIENT_PRICE_LEFT = 0;

                //có đơn vị quy đổi thì gán lại số lượng giá, đơn vị
                //Nếu trường này khác 1 thì xử lý như hiện tại, tức là: nếu có đơn vị chuyển đổi thì hiển thị theo đơn vị chuyển đổi, nếu ko có đơn vị chuyển đổi thì hiển thị theo đơn vị tính gốc
                var svUnit = SereServLookup.Get(lk.ServiceUnitById, this.TDL_SERVICE_UNIT_ID);
                if (svUnit != null && svUnit.CONVERT_RATIO.HasValue && this.USE_ORIGINAL_UNIT_FOR_PRES != 1 && svUnit.CONVERT_RATIO.Value != 0)
                {
                    var convertUnit = SereServLookup.Get(lk.ServiceUnitById, svUnit.CONVERT_ID);
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

                //Trạng thái đã thanh toán (bill chưa huỷ) hoặc đã tạm ứng còn hiệu lực — tra cứu O(1).
                if (lk.PaidBillSereServIds.Contains(Convert.ToInt64(data.ID)))
                {
                    this.IS_PAID = 1;
                }
                else
                {
                    foreach (HIS_SERE_SERV_DEPOSIT dep in lk.DepositBySereServId[Convert.ToInt64(data.ID)])
                    {
                        long depId = Convert.ToInt64(dep.ID);
                        bool valid = lk.DepositIdsWithCanceledRepay.Contains(depId) || !lk.DepositIdsWithRepay.Contains(depId);
                        if (valid)
                        {
                            this.IS_PAID = 1;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public decimal PriceBHYTProcess(SereServADO ss)
        {
            decimal priceBHYT = 0;
            try
            {
                //Get PATIENT_TYPE_ALTER
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
