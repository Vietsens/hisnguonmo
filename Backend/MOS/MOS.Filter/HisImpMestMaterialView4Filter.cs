
using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisImpMestMaterialView4Filter : FilterBase
    {
        public List<long> MATERIAL_IDs { get; set; }
        public long? MATERIAL_ID { get; set; }
        public long? IMP_MEST_ID { get; set; }
        public long? MATERIAL_TYPE_ID { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public List<long> IMP_MEST_IDs { get; set; }
        public List<long> MATERIAL_TYPE_IDs { get; set; }
        public List<string> SERIAL_NUMBERs { get; set; }

        public HisImpMestMaterialView4Filter()
            : base()
        {
        }
    }
}
