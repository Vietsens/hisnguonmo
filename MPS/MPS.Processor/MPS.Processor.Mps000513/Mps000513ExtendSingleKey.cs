using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000513
{
    class Mps000513ExtendSingleKey : CommonKey
    {
        internal const string TREATMENT_CODE = "TREATMENT_CODE";
        internal const string PATIENT_CODE = "PATIENT_CODE";

        internal const string TDL_PATIENT_NAME = "TDL_PATIENT_NAME";
        internal const string TDL_PATIENT_DOB = "TDL_PATIENT_DOB";

        internal const string TDL_PATIENT_GENDER_NAME = "TDL_PATIENT_GENDER_NAME";
        internal const string TDL_PATIENT_ADDRESS = "TDL_PATIENT_ADDRESS";
        internal const string ICD_CODE = "ICD_CODE";
        internal const string ICD_NAME = "ICD_NAME";
        internal const string ICD_SUB_CODE = "ICD_SUB_CODE";
        internal const string ICD_TEXT = "ICD_TEXT";
        internal const string BED_ROOM_CODE = "BED_ROOM_CODE";
        internal const string BED_ROOM_NAME = "BED_ROOM_NAME";
        internal const string BED_CODE = "BED_CODE";
        internal const string BED_NAME = "BED_NAME";
        internal const string EXAM_EXECUTE_CONTENT = "EXAM_EXECUTE_CONTENT";
        internal const string EXAM_EXCUTE = "EXAM_EXCUTE";

        internal const string EXAM_EXECUTE_DEPARMENT_CODE = "EXAM_EXECUTE_DEPARTMENT_CODE";
        internal const string EXAM_EXECUTE_DEPARMENT_NAME = "EXAM_EXECUTE_DEPARTMENT_NAME";
        internal const string INVITE_DEPARTMENT_CODE = "INVITE_DEPARTMENT_CODE";
        internal const string INVITE_DEPARTMENT_NAME = "INVITE_DEPARTMENT_NAME";
        internal const string IS_EXAM_ANESTHESIA = "IS_EXAM_ANESTHESIA";
        internal const string IS__EXAM_BED = "IS__EXAM_BED";
    }
}
