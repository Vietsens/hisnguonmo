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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000296.PDO
{
    public class Mps000296PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000296";

        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public HIS_DHST hisDhst { get; set; }
        public HIS_SERVICE_REQ HisPrescription { get; set; }
        public HIS_SERVICE_REQ hisServiceReq_Exam { get; set; }
        public Mps000296ADO Mps000296ADO { get; set; }
        public HIS_TREATMENT hisTreatment { get; set; }// lấy thời gian hẹn khám
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }

        public Mps000296PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            HIS_TREATMENT treatment,
            Mps000296ADO mps000296ADO)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000296ADO = mps000296ADO;
                this.hisTreatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000296ADO : V_HIS_PATIENT
    {
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string TITLE { get; set; }
    }

    public class ExpMestMedicineSDO : HIS_EXP_MEST_MEDICINE
    {
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_FUNCTIONAL_FOOD { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public long MEDI_STOCK_ID { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public long PATIENT_TYPE_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string CREATOR_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc

        public decimal? CONVERT_RATIO { get; set; }
        public string CONVERT_UNIT_NAME { get; set; }
    }
}
