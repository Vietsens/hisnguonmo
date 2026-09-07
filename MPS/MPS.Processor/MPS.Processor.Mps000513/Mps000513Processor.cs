using Inventec.Core;
using MPS.Processor.Mps000513.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000513
{
    public class Mps000513Processor : AbstractProcessor
    {
        Mps000513PDO rdo;

        public Mps000513Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000513PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName)); 
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);

                result = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.TDL_PATIENT_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(rdo.currentExam.TDL_PATIENT_DOB))));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.TREATMENT_CODE, rdo.currentExam.TREATMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.PATIENT_CODE, rdo.currentExam.PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.TDL_PATIENT_NAME, rdo.currentExam.TDL_PATIENT_NAME));

                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.currentExam.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.currentExam.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.ICD_CODE, rdo.currentTreatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.ICD_NAME, rdo.currentTreatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.BED_ROOM_CODE, rdo.currentExam.BED_ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.BED_ROOM_NAME, rdo.currentExam.BED_ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.BED_CODE, rdo.currentExam.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.BED_NAME, rdo.currentExam.BED_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.ICD_SUB_CODE, rdo.currentTreatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.ICD_TEXT, rdo.currentTreatment.ICD_TEXT));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.EXAM_EXECUTE_CONTENT, rdo.currentExam.EXAM_EXECUTE_CONTENT));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.EXAM_EXCUTE, rdo.currentExam.EXAM_EXCUTE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.EXAM_EXECUTE_DEPARMENT_CODE, rdo.currentExam.EXAM_EXECUTE_DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.EXAM_EXECUTE_DEPARMENT_NAME, rdo.currentExam.EXAM_EXECUTE_DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.INVITE_DEPARTMENT_CODE, rdo.currentExam.INVITE_DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.INVITE_DEPARTMENT_NAME, rdo.currentExam.INVITE_DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.IS_EXAM_ANESTHESIA, rdo.currentExam.IS_EXAM_ANESTHESIA));
                    SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.IS__EXAM_BED, rdo.currentExam.IS__EXAM_BED));

                    if (rdo.currentExam.INVITE_TIME != null)
                    {
                        SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.INTRUCTION_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)rdo.currentExam.INVITE_TIME)));
                    }
                    if (rdo.currentExam.EXAM_TIME != null)
                    {
                        SetSingleKey(new KeyValue(Mps000513ExtendSingleKey.EXAM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)rdo.currentExam.EXAM_TIME)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
