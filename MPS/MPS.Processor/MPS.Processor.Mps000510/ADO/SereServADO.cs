/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000510.ADO
{
    /// <summary>
    /// Một dòng dịch vụ "phẳng" của bảng kê.
    /// Kế thừa thẳng V_HIS_SERE_SERV_2 nên đã có sẵn toàn bộ cột của view
    /// (TDL_SERVICE_NAME, các cột VIR_TOTAL_*, MEDICINE_LINE_ID, OTHER_SOURCE_PRICE...).
    /// Vì view đã denormalize sẵn, ta KHÔNG cần join tay với Services/HeinServiceTypes/Rooms...
    /// như Mps000281 — chỉ tra cứu tên qua Dictionary (O(1)) cho vài cột view không có.
    /// Mọi chiều gom nhóm (loại hình DV, dòng thuốc, khoa, phòng) nằm ngay trên dòng này
    /// dưới dạng cột, để template tự group/filter — không cần dataset phụ + AddRelationship.
    /// </summary>
    public class SereServADO : V_HIS_SERE_SERV_2
    {
        // ===== Hiển thị dịch vụ (view không có sẵn tên loại dịch vụ) =====
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? SERVICE_TYPE_ID2 { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }

        // ===== Loại hình dịch vụ BHYT (gom đặc biệt thuốc/VT/PTTT/máu) =====
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_NAME_697 { get; set; }
        public long? HEIN_SERVICE_TYPE_NUM_ORDER { get; set; }
        public long? HEIN_SERVICE_TYPE_CHILD_NUM_ORDER { get; set; }
        public long? HEIN_SERVICE_TYPE_PARENT_1_ID { get; set; } // cấp 1 "Giường"

        // ===== Phòng thực hiện =====
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }

        // ===== Dòng thuốc (view có sẵn MEDICINE_LINE_ID, chỉ thiếu code/name) =====
        public string MEDICINE_LINE_CODE { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }

        // ===== Chiều gom nhóm khoa / phòng (YC: lấy theo khoa-phòng) =====
        public long GROUP_DEPARTMENT_ID { get; set; }
        public string GROUP_DEPARTMENT_CODE { get; set; }
        public string GROUP_DEPARTMENT_NAME { get; set; }
        public long GROUP_ROOM_ID { get; set; }
        public string GROUP_ROOM_CODE { get; set; }
        public string GROUP_ROOM_NAME { get; set; }

        // ===== Giá trị tính toán phục vụ in =====
        public decimal PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal PRICE_VP { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_PRICE_PATIENT_SELF { get; set; }
        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public decimal? SERVICE_PAY_RATE { get; set; }
        public decimal RADIO_SERIVCE { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; }

        public SereServADO() { }

        public SereServADO(
            V_HIS_SERE_SERV_2 data,
            Dictionary<long, HIS_HEIN_SERVICE_TYPE> heinTypeById,
            Dictionary<long, V_HIS_ROOM> roomById,
            Dictionary<long, HIS_DEPARTMENT> deptById,
            Dictionary<long, HIS_MEDICINE_LINE> medLineById,
            Dictionary<long, HIS_SERVICE_UNIT> unitById,
            Dictionary<long, V_HIS_SERVICE> serviceById,
            Dictionary<long, HIS_MEDICINE_TYPE> medicineTypeByServiceId)
        {
            try
            {
                // 1) Copy toàn bộ cột của view sang chính object này (kế thừa nên set được trực tiếp)
                var pis = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV_2>();
                foreach (var pi in pis)
                {
                    pi.SetValue(this, pi.GetValue(data));
                }

                // 2) Tên dịch vụ: lấy ngay từ cột TDL_* của view
                this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                this.SERVICE_TYPE_ID2 = data.TDL_SERVICE_TYPE_ID;
                V_HIS_SERVICE service = TryGet(serviceById, data.SERVICE_ID);
                if (service != null)
                {
                    this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                }

                // 3) Loại hình DV BHYT (gom đặc biệt) — tra cứu O(1) qua dictionary
                ResolveHeinServiceType(data.TDL_HEIN_SERVICE_TYPE_ID, heinTypeById);

                // 4) Phòng thực hiện
                V_HIS_ROOM room = TryGet(roomById, data.TDL_EXECUTE_ROOM_ID);
                if (room != null)
                {
                    this.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                    this.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                }

                // 5) Dòng thuốc: ưu tiên cột view, nếu null thì fallback giống Mps000281
                //    (tra medicineTypes theo SERVICE_ID -> MEDICINE_LINE_ID) để không bị "Chưa xác định"
                long? medicineLineId = data.MEDICINE_LINE_ID;
                if (!medicineLineId.HasValue || medicineLineId.Value <= 0)
                {
                    HIS_MEDICINE_TYPE mt = TryGet(medicineTypeByServiceId, data.SERVICE_ID);
                    if (mt != null && mt.MEDICINE_LINE_ID.HasValue)
                        medicineLineId = mt.MEDICINE_LINE_ID;
                }
                if (medicineLineId.HasValue && medicineLineId.Value > 0)
                {
                    this.MEDICINE_LINE_ID = medicineLineId; // ghi đè cột view để các bước gom sau dùng đúng id
                    HIS_MEDICINE_LINE line = TryGet(medLineById, medicineLineId.Value);
                    if (line != null)
                    {
                        this.MEDICINE_LINE_CODE = line.MEDICINE_LINE_CODE;
                        this.MEDICINE_LINE_NAME = line.MEDICINE_LINE_NAME;
                    }
                }

                // 6) Cột gom nhóm khoa / phòng — PORT NGUYÊN nhánh ExeRoom của Mps000306 (SereServADO.cs:353-378):
                //    Phòng: dịch vụ KHÁM (KH) lấy phòng THỰC HIỆN (TDL_EXECUTE_ROOM_ID); 
                //           còn lại lấy phòng CHỈ ĐỊNH (TDL_REQUEST_ROOM_ID).
                //    KHOA lấy THEO CHÍNH PHÒNG đã resolve (room.DEPARTMENT_*) — KHÔNG lấy khoa rời từ
                //    TDL_*_DEPARTMENT_ID. Nhờ vậy khoa/phòng LUÔN khớp (phòng thuộc đúng khoa), đúng như
                //    306 lên được phòng. Không có gate IS_CLINICAL.
                bool isKham = this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;
                long groupRoomId = isKham ? data.TDL_EXECUTE_ROOM_ID : data.TDL_REQUEST_ROOM_ID;
                V_HIS_ROOM groupRoom = TryGet(roomById, groupRoomId);
                if (groupRoom != null)
                {
                    this.GROUP_ROOM_ID = groupRoom.ID;
                    // Template ẩn dòng phòng khi GROUP_ROOM_CODE rỗng. Nhiều phòng (buồng lâm sàng)
                    // có TÊN nhưng MÃ rỗng -> fallback sang tên/ID để không bị ẩn (mã này chỉ dùng
                    // cho điều kiện ẩn, KHÔNG hiển thị; template in ROOM_NAME). 
                    this.GROUP_ROOM_CODE = !string.IsNullOrEmpty(groupRoom.ROOM_CODE)
                        ? groupRoom.ROOM_CODE
                        : (!string.IsNullOrEmpty(groupRoom.ROOM_NAME) ? groupRoom.ROOM_NAME : groupRoom.ID.ToString());
                    this.GROUP_ROOM_NAME = groupRoom.ROOM_NAME;

                    // Khoa LẤY THEO PHÒNG (giống ExeRoom 306: room.DEPARTMENT_*)
                    this.GROUP_DEPARTMENT_ID = groupRoom.DEPARTMENT_ID;
                    this.GROUP_DEPARTMENT_CODE = groupRoom.DEPARTMENT_CODE;
                    this.GROUP_DEPARTMENT_NAME = groupRoom.DEPARTMENT_NAME;
                }
                else
                {
                    // Không resolve được phòng -> vẫn gán khoa từ TDL để dòng khoa hiển thị (phòng để trống) 
                    long groupDeptId = isKham ? data.TDL_EXECUTE_DEPARTMENT_ID : data.TDL_REQUEST_DEPARTMENT_ID;
                    this.GROUP_DEPARTMENT_ID = groupDeptId;
                    HIS_DEPARTMENT dept = TryGet(deptById, groupDeptId);
                    if (dept != null)
                    {
                        this.GROUP_DEPARTMENT_CODE = dept.DEPARTMENT_CODE;
                        this.GROUP_DEPARTMENT_NAME = dept.DEPARTMENT_NAME;
                    }
                }

                // [DIAG] TODO XÓA SAU KHI FIX: soi vì sao 511 không lên phòng
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "[Mps000510][DIAG] SV='{0}' isKham={1} execRoom={2} reqRoom={3} chosen={4} resolved={5} roomByIdHasChosen={6} roomByIdCount={7} execDept={8} reqDept={9} => GROUP_ROOM_ID={10} GROUP_ROOM_NAME='{11}' GROUP_DEPARTMENT_ID={12}",
                    this.SERVICE_NAME, isKham, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, groupRoomId,
                    groupRoom != null, (roomById != null && roomById.ContainsKey(groupRoomId)), (roomById != null ? roomById.Count : -1),
                    data.TDL_EXECUTE_DEPARTMENT_ID, data.TDL_REQUEST_DEPARTMENT_ID,
                    this.GROUP_ROOM_ID, this.GROUP_ROOM_NAME, this.GROUP_DEPARTMENT_ID)
                    + string.Format(" rawRoomCode='{0}' GROUP_ROOM_CODE(final)='{1}'",
                        (groupRoom != null ? groupRoom.ROOM_CODE : "<noRoom>"), this.GROUP_ROOM_CODE));

                // 7) Tính giá (các cột nguồn VIR_*, HEIN_LIMIT_PRICE... đã có sẵn trên view)
                ComputePrices();

                // 8) Quy đổi đơn vị tính (nếu có)
                ApplyConvertUnit(unitById);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static T TryGet<T>(Dictionary<long, T> dic, long key) where T : class
        {
            T v;
            if (dic != null && key > 0 && dic.TryGetValue(key, out v))
                return v;
            return null;
        }

        private void ResolveHeinServiceType(long? heinServiceTypeId, Dictionary<long, HIS_HEIN_SERVICE_TYPE> heinTypeById)
        {
            if (!heinServiceTypeId.HasValue || heinTypeById == null || heinTypeById.Count == 0)
                return;

            HIS_HEIN_SERVICE_TYPE heinServiceType = TryGet(heinTypeById, heinServiceTypeId.Value);
            if (heinServiceType == null)
                return;

            long id = heinServiceTypeId.Value;

            if (id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL)
            {
                HIS_HEIN_SERVICE_TYPE th = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM) ?? heinServiceType;
                this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.THUOC_TRUYENDICH__ID;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = th.VIR_PARENT_NUM_ORDER;
                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = th.NUM_ORDER;
                this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.THUOC_TRUYENDICH__NAME;
                this.HEIN_SERVICE_TYPE_NAME_697 = th.HEIN_SERVICE_TYPE_NAME_697;
            }
            else if (id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT)
            {
                HIS_HEIN_SERVICE_TYPE vt = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM) ?? heinServiceType;
                this.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.VT_Y_TE__ID;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = vt.VIR_PARENT_NUM_ORDER;
                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = vt.NUM_ORDER;
                this.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.VT_Y_TE__NAME;
                this.HEIN_SERVICE_TYPE_NAME_697 = vt.HEIN_SERVICE_TYPE_NAME_697;
            }
            else if (id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT)
            {
                HIS_HEIN_SERVICE_TYPE pttt = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT);
                HIS_HEIN_SERVICE_TYPE tt = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT);
                if (pttt != null && tt != null)
                {
                    this.HEIN_SERVICE_TYPE_ID = pttt.ID;
                    this.HEIN_SERVICE_TYPE_NUM_ORDER = pttt.VIR_PARENT_NUM_ORDER;
                    this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = pttt.NUM_ORDER;
                    this.HEIN_SERVICE_TYPE_CODE = pttt.HEIN_SERVICE_TYPE_CODE;
                    this.HEIN_SERVICE_TYPE_NAME = UpperFirst(tt.HEIN_SERVICE_TYPE_NAME) + ", " + (pttt.HEIN_SERVICE_TYPE_NAME ?? "").ToLower();
                    this.HEIN_SERVICE_TYPE_NAME_697 = UpperFirst(tt.HEIN_SERVICE_TYPE_NAME_697) + ", " + (pttt.HEIN_SERVICE_TYPE_NAME_697 ?? "").ToLower();
                    return;
                }
                SetDefaultHeinServiceType(heinServiceType);
            }
            else if (id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU
                || id == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM)
            {
                HIS_HEIN_SERVICE_TYPE mau = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU);
                HIS_HEIN_SERVICE_TYPE cpm = TryGet(heinTypeById, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM);
                if (mau != null && cpm != null)
                {
                    this.HEIN_SERVICE_TYPE_ID = mau.ID;
                    this.HEIN_SERVICE_TYPE_NUM_ORDER = mau.VIR_PARENT_NUM_ORDER;
                    this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = mau.NUM_ORDER;
                    this.HEIN_SERVICE_TYPE_CODE = mau.HEIN_SERVICE_TYPE_CODE;
                    this.HEIN_SERVICE_TYPE_NAME = UpperFirst(mau.HEIN_SERVICE_TYPE_NAME) + ", " + (cpm.HEIN_SERVICE_TYPE_NAME ?? "").ToLower();
                    this.HEIN_SERVICE_TYPE_NAME_697 = UpperFirst(mau.HEIN_SERVICE_TYPE_NAME_697) + ", " + (cpm.HEIN_SERVICE_TYPE_NAME_697 ?? "").ToLower();
                    return;
                }
                SetDefaultHeinServiceType(heinServiceType);
            }
            else
            {
                SetDefaultHeinServiceType(heinServiceType);
            }
        }

        private void SetDefaultHeinServiceType(HIS_HEIN_SERVICE_TYPE heinServiceType)
        {
            this.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
            this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceType.VIR_PARENT_NUM_ORDER;
            this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceType.NUM_ORDER;
            this.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
            this.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_NAME;
            this.HEIN_SERVICE_TYPE_NAME_697 = heinServiceType.HEIN_SERVICE_TYPE_NAME_697;
        }

        private static string UpperFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return s.First().ToString().ToUpper() + s.ToLower().Substring(1);
        }

        private void ComputePrices()
        {
            this.PRICE_BHYT = 0;
            this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;

            if (this.VIR_TOTAL_HEIN_PRICE.HasValue && this.AMOUNT != 0)
                this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;

            this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0
                ? (this.HEIN_LIMIT_PRICE.HasValue
                    ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100
                    : (this.PRICE / this.ORIGINAL_PRICE) * 100)
                : 0;

            if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;

            decimal? rate = null;
            if (this.ORIGINAL_PRICE > 0)
            {
                if (this.HEIN_LIMIT_PRICE.HasValue)
                    rate = 100 * Math.Round(this.HEIN_LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), 2);
                else if (this.LIMIT_PRICE.HasValue)
                    rate = 100 * Math.Round(this.LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO)), 2);
                else
                    rate = 100 * Math.Round(this.PRICE / this.ORIGINAL_PRICE, 2);
            }
            this.SERVICE_PAY_RATE = Math.Round(rate ?? 0, 0);

            // OTHER_SOURCE_PRICE trên view là đơn giá nguồn khác -> nhân số lượng
            this.OTHER_SOURCE_PRICE = (this.OTHER_SOURCE_PRICE ?? 0) * this.AMOUNT;

            if (this.PRIMARY_PATIENT_TYPE_ID.HasValue)
            {
                this.PRICE = (this.LIMIT_PRICE ?? 0);
                this.VIR_TOTAL_PRICE_NO_EXPEND = (this.PRICE * this.AMOUNT) * ((this.SERVICE_PAY_RATE ?? 0) / 100);
            }

            this.VIR_TOTAL_PATIENT_PRICE_BHYT = 0;

            this.PRICE_VP = this.VIR_PRICE ?? 0;
            this.TOTAL_PRICE_VP = this.PRICE_VP * this.AMOUNT;
            this.TOTAL_PATIENT_PRICE_LEFT = (this.TOTAL_PRICE_VP) * ((this.SERVICE_PAY_RATE ?? 0) / 100)
                - (this.VIR_TOTAL_HEIN_PRICE ?? 0)
                - (this.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0)
                - (this.OTHER_SOURCE_PRICE ?? 0);
            if (this.TOTAL_PATIENT_PRICE_LEFT < 0)
                this.TOTAL_PATIENT_PRICE_LEFT = 0;
        }

        private void ApplyConvertUnit(Dictionary<long, HIS_SERVICE_UNIT> unitById)
        {
            HIS_SERVICE_UNIT svUnit = TryGet(unitById, this.TDL_SERVICE_UNIT_ID);
            if (svUnit == null || !svUnit.CONVERT_RATIO.HasValue || svUnit.CONVERT_RATIO.Value == 0 || this.USE_ORIGINAL_UNIT_FOR_PRES == 1)
                return;

            HIS_SERVICE_UNIT convertUnit = svUnit.CONVERT_ID.HasValue ? TryGet(unitById, svUnit.CONVERT_ID.Value) : null;
            if (convertUnit != null)
            {
                this.SERVICE_UNIT_CODE = convertUnit.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = convertUnit.SERVICE_UNIT_NAME;
            }

            this.AMOUNT = this.AMOUNT * svUnit.CONVERT_RATIO.Value;
            this.PRICE = this.PRICE / svUnit.CONVERT_RATIO.Value;
            this.PRIMARY_PRICE = (this.PRIMARY_PRICE ?? 0) / svUnit.CONVERT_RATIO.Value;
            this.PRICE_BHYT = this.PRICE_BHYT / svUnit.CONVERT_RATIO.Value;
            this.PRICE_VP = this.PRICE_VP / svUnit.CONVERT_RATIO.Value;
        }
    }
}
