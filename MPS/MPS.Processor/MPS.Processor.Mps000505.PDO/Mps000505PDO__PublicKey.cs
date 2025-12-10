using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000505.PDO
{
    public partial class Mps000505PDO : RDOBase
    {
    }
    public class Mps000505ADO
    {
        public long? IMP_DATE { get; set; } //ngày nhập IMP_DATE (V_HIS_IMP_MEST)
        public string DOCUMENT_NUMBER { get; set; } //số chứng từ DOCUMENT_NUMBER (V_HIS_IMP_MEST)
        public long? SUPPLIER_ID { get; set; } //id nhà cung cấp SUPPLIER_ID (V_HIS_IMP_MEST)
        public string SUPPLIER_CODE { get; set; } //mã nhà cung cấp SUPPLIER_CODE (HIS_SUPPLIER)
        public string SUPPLIER_NAME { get; set; } //tên nhà cung cấp SUPPLIER_NAME (HIS_SUPPLIER)
        public long MEDI_STOCK_ID { get; set; } // id kho MEDI_STOCK_ID (V_HIS_IMP_MEST)
        public string MEDI_STOCK_CODE { get; set; } //mã kho MEDI_STOCK_CODE (V_HIS_IMP_MEST)
        public string MEDI_STOCK_NAME { get; set; } //tên kho MEDI_STOCK_NAME (V_HIS_IMP_MEST)
        public long? APPROVAL_TIME { get; set; } //thời gian duyệt APPROVAL_TIME (V_HIS_IMP_MEST)

        //public string DOCUMENT_NUMBER { get; set; } // số chứng từ
        public string MEDI_MATE_TYPE_NAME { get; set; } // loại thuốc, vật tư, máu
        public string SERVICE_UNIT_NAME { get; set; } //đơn vị tính
        public string NATIONAL_NAME { get; set; } //tên thuốc
        public string BATCH_REGISTER_NUMBER { get; set; } //số đăng ký
        public string PACKAGE_NUMBER { get; set; } //số kiểm soát số lô
        public string EXPIRED_DATE_STR { get; set; } //ngày hết hạn
        public decimal PRICE { get; set; } //đơn giá
        public decimal AMOUNT { get; set; } //số lượng
        public decimal PRICE_AMOUNT { get; set; } //thành tiền

        //public string BATCH_REGISTER_NUMBER { get; set; }
        public string BATCH_MANUFACTURER_CODE { get; set; }
        public string BATCH_MANUFACTURER_NAME { get; set; }
        public Dictionary<string, decimal> DicMediMate { get; set; }

        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public decimal? VIR_IMP_PRICE { get; set; }

        public string MEDICAL_CONTRACT_CODE { get; set; }
        public string MEDICAL_CONTRACT_NAME { get; set; }
        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string VENTURE_AGREENING { get; set; }
    }

    public class ImpMestMedicineADO
    {
        
    }

    public class MedicalContractADO : V_HIS_MEDICAL_CONTRACT
    {
        public long MEDICINE_ID { get; set; }
        public long MATERIAL_ID { get; set; }
    }
}
