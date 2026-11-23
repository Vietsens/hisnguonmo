using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.ADO
{
    /// <summary>
    /// Medicine type or material type (chemicals included) to pick into the anticipate
    /// </summary>
    class ItemTypeADO
    {
        public string KEY { get; set; }
        public long TYPE_ID { get; set; }
        public bool IS_MATERIAL { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_NAME { get; set; }
        public string DISPLAY { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKING { get; set; }
        public string KIND_NAME { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public List<long> SUPPLIER_IDS { get; set; }
        public short? IS_BUSINESS { get; set; }
        public short? IS_DRUG_STORE { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }

        internal static string MakeKey(bool isMaterial, long typeId)
        {
            return (isMaterial ? "MA_" : "ME_") + typeId;
        }

        /// <summary>
        /// Cung quy tac loc mat hang cua man Nhap thuoc vat tu (ImpMestCreate) theo loai kho: kinh doanh, dong thuoc, nha thuoc.
        /// Mat hang khong qua duoc thi dat duoc nhung khong nhap duoc vao kho
        /// </summary>
        internal static bool IsImportableToStock(ItemTypeADO item, V_HIS_MEDI_STOCK stock)
        {
            if (stock == null)
            {
                return true;
            }
            if (stock.IS_BUSINESS == 1)
            {
                if (item.IS_BUSINESS != 1)
                {
                    return false;
                }
                if (stock.IS_SHOW_DRUG_STORE == 1 ? item.IS_DRUG_STORE != 1 : item.IS_DRUG_STORE != null)
                {
                    return false;
                }
            }
            else if (item.IS_BUSINESS == 1)
            {
                return false;
            }
            if (item.IS_MATERIAL || item.MEDICINE_LINE_ID == null || (stock.IS_NEW_MEDICINE == 1 && stock.IS_TRADITIONAL_MEDICINE == 1))
            {
                return true;
            }
            if (stock.IS_NEW_MEDICINE == 1)
            {
                return item.MEDICINE_LINE_ID == MEDICINE_LINE_ID__TTD;
            }
            if (stock.IS_TRADITIONAL_MEDICINE == 1)
            {
                return item.MEDICINE_LINE_ID == MEDICINE_LINE_ID__CP_YHCT || item.MEDICINE_LINE_ID == MEDICINE_LINE_ID__VT_YHCT;
            }
            return true;
        }

        // = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE (plugin khong tham chieu IMSys)
        private const long MEDICINE_LINE_ID__TTD = 1;
        private const long MEDICINE_LINE_ID__CP_YHCT = 2;
        private const long MEDICINE_LINE_ID__VT_YHCT = 3;

        internal static List<long> ParseIds(string ids)
        {
            List<long> result = new List<long>();
            if (String.IsNullOrWhiteSpace(ids))
            {
                return result;
            }
            foreach (string s in ids.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                long id;
                if (long.TryParse(s.Trim(), out id) && !result.Contains(id))
                {
                    result.Add(id);
                }
            }
            return result;
        }
    }
}
