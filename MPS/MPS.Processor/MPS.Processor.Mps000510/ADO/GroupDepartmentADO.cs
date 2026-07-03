/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace MPS.Processor.Mps000510.ADO
{
    /// <summary>
    /// Dòng master gom theo khoa xử lý / phòng xử lý.
    /// Nối với bộ Service qua GROUP_DEPARTMENT_ID (khoa) hoặc GROUP_ROOM_ID (phòng). 
    /// </summary>
    public class GroupDepartmentADO
    {
        public long GROUP_DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public short? IS_CLINICAL { get; set; }

        public long GROUP_ROOM_ID { get; set; }
        public string ROOM_CODE { get; set; }
        public string GROUP_ROOM_CODE { get; set; }
        public string ROOM_NAME { get; set; }

        // Tổng tiền của khoa/phòng
        public decimal TOTAL_PRICE { get; set; }                 // VIR_TOTAL_PRICE_NO_EXPEND
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal TOTAL_HEIN_PRICE { get; set; }            // BHYT trả
        public decimal TOTAL_PATIENT_PRICE { get; set; }         // BN cùng chi trả
        public decimal TOTAL_PATIENT_PRICE_SELF { get; set; }    // BN tự trả
        public decimal OTHER_SOURCE_PRICE { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }

        // Alias trùng tên với bộ HeinServiceType để template dùng chung tên key (cùng giá trị).
        public decimal TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
    }
}
