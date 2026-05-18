using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000507
{
    class Mps000507ExtendSingleKey : CommonKey
    {
        internal const string DHST_LOGINNAME = "DHST_LOGINNAME";
        internal const string BMI_VALUE = "BMI_VALUE";
        internal const string BLOOD_PRESSURE_DISPLAY = "BLOOD_PRESSURE_DISPLAY";
        internal const string HEALTH_EXAM_RANK_NAME = "HEALTH_EXAM_RANK_NAME";
        internal const string CONCLUSION_DATE_DISPLAY = "CONCLUSION_DATE_DISPLAY";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";

        // Username keys (resolve từ V_HIS_EMPLOYEE.LOGINNAME → TDL_USERNAME)
        internal const string EXAM_CIRCULATION_USERNAME = "EXAM_CIRCULATION_USERNAME";
        internal const string EXAM_RESPIRATORY_USERNAME = "EXAM_RESPIRATORY_USERNAME";
        internal const string EXAM_DIGESTION_USERNAME = "EXAM_DIGESTION_USERNAME";
        internal const string EXAM_KIDNEY_UROLOGY_USERNAME = "EXAM_KIDNEY_UROLOGY_USERNAME";
        internal const string EXAM_NEUROLOGICAL_USERNAME = "EXAM_NEUROLOGICAL_USERNAME";
        internal const string EXAM_MUSCLE_BONE_USERNAME = "EXAM_MUSCLE_BONE_USERNAME";
        internal const string EXAM_ENT_USERNAME = "EXAM_ENT_USERNAME";
        internal const string EXAM_STOMATOLOGY_USERNAME = "EXAM_STOMATOLOGY_USERNAME";
        internal const string EXAM_EYE_USERNAME = "EXAM_EYE_USERNAME";
        internal const string EXAM_OEND_USERNAME = "EXAM_OEND_USERNAME";
        internal const string EXAM_MENTAL_USERNAME = "EXAM_MENTAL_USERNAME";
        internal const string EXAM_DERMATOLOGY_USERNAME = "EXAM_DERMATOLOGY_USERNAME";
        internal const string EXAM_SURGERY_USERNAME = "EXAM_SURGERY_USERNAME";
        internal const string EXAM_OBSTETRIC_USERNAME = "EXAM_OBSTETRIC_USERNAME";
        internal const string EXAM_SUBCLINICAL_USERNAME = "EXAM_SUBCLINICAL_USERNAME";
    }
}
