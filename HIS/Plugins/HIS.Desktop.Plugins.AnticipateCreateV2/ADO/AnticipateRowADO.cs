/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tao du tru v2:
 * - AnticipateRowADO: 1 dong ket qua tra ve tu API GetForAnticipate (Mety/Maty) -
 *   so lieu du tru theo LOAI (gop toan vien). Field trung ten voi
 *   MOS.SDO.HisMediStockMety(Maty)AnticipateResultSDO de deserialize dung.
 * - AnticipateReqFilter: request gui len API (danh sach kho + khoang thoi gian).
 */
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AnticipateCreateV2.ADO
{
    /// <summary>
    /// Filter gui len API GetForAnticipate: danh sach kho + khoang thoi gian (yyyyMMddHHmmss)
    /// + trang thai khoa. Dung chung cho thuoc/vat tu (field type-code/name de trong).
    /// </summary>
    public class AnticipateReqFilter
    {
        public List<long> MEDI_STOCK_IDs { get; set; }
        public long? FROM_TIME { get; set; }
        public long? TO_TIME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public short? IS_ACTIVE { get; set; }
    }

    /// <summary>
    /// 1 dong so lieu du tru theo LOAI (gop toan vien) - dung chung thuoc/vat tu.
    /// Deserialize tu HisMediStockMetyAnticipateResultSDO / HisMediStockMatyAnticipateResultSDO.
    /// (Vat tu khong co ACTIVE_INGR_BHYT_NAME/USE_FORM_NAME -> null.)
    /// </summary>
    public class AnticipateRowADO
    {
        public long MEDICINE_TYPE_ID { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public long MATERIAL_TYPE_ID { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }

        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string USE_FORM_NAME { get; set; }
        public string UNIT_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string SUPPLIER_NAME { get; set; }

        public decimal BID_AMOUNT { get; set; }
        public decimal BID_IMPORTED_AMOUNT { get; set; }
        public decimal BID_REMAIN_AMOUNT { get; set; }

        public decimal OPEN_QUANTITY { get; set; }
        public decimal NEW_IMPORT_QUANTITY { get; set; }
        public decimal USED_QUANTITY { get; set; }
        public decimal CLOSE_QUANTITY { get; set; }

        public decimal MAX_EXPORT_QUANTITY { get; set; }
        public int MAX_EXPORT_MONTH { get; set; }

        public decimal EXP_PRICE_VAT { get; set; }
    }
}
