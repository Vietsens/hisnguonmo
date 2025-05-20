/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000500.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000500
{
    public class Mps000500Processor : AbstractProcessor
    {
        Mps000500PDO rdo;

        public Mps000500Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000500PDO)rdoBase;
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
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.TDL_PATIENT_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(rdo.currentExam.TDL_PATIENT_DOB))));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.TREATMENT_CODE, rdo.currentExam.TREATMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.PATIENT_CODE, rdo.currentExam.PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.TDL_PATIENT_NAME, rdo.currentExam.TDL_PATIENT_NAME));

                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.currentExam.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.currentExam.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.ICD_CODE, rdo.currentTreatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.ICD_NAME, rdo.currentTreatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.BED_ROOM_CODE, rdo.currentExam.BED_ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.BED_ROOM_NAME, rdo.currentExam.BED_ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.BED_CODE, rdo.currentExam.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.BED_NAME, rdo.currentExam.BED_NAME));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.ICD_SUB_CODE, rdo.currentTreatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.ICD_TEXT, rdo.currentTreatment.ICD_TEXT));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.EXAM_EXECUTE_CONTENT, rdo.currentExam.EXAM_EXECUTE_CONTENT));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.EXAM_EXCUTE, rdo.currentExam.EXAM_EXCUTE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.EXAM_EXECUTE_DEPARMENT_CODE, rdo.currentExam.EXAM_EXECUTE_DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.EXAM_EXECUTE_DEPARMENT_NAME, rdo.currentExam.EXAM_EXECUTE_DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.INVITE_DEPARTMENT_CODE, rdo.currentExam.INVITE_DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000500ExtendSingleKey.INVITE_DEPARTMENT_NAME, rdo.currentExam.INVITE_DEPARTMENT_NAME));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
