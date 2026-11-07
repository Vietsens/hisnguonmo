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

        internal static string MakeKey(bool isMaterial, long typeId)
        {
            return (isMaterial ? "MA_" : "ME_") + typeId;
        }

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
