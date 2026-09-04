using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000499
{
    class Mps000499ExtendSingleKey : CommonKey
    {
        internal const string DHST_LOGINNAME = "DHST_LOGINNAME";
        // ----- Kết luận theo bệnh (ICD-10) — lấy từ HIS_KSK_GENERAL (UC dùng chung mọi tab KSK) -----
        // GENERAL.CONCLUSION_ICD_TYPE: 1=Chưa phát hiện bất thường, 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định
        internal const string CONCLUSION_ICD_NONE_X = "CONCLUSION_ICD_NONE_X";
        internal const string CONCLUSION_ICD_PRELIM_X = "CONCLUSION_ICD_PRELIM_X";
        internal const string CONCLUSION_ICD_FINAL_X = "CONCLUSION_ICD_FINAL_X";
        internal const string CONCLUSION_ICD_CODE = "CONCLUSION_ICD_CODE";
        internal const string CONCLUSION_ICD_NAME = "CONCLUSION_ICD_NAME";
    }
}
