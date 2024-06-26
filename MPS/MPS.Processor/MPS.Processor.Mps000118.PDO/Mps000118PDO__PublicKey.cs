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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000118.PDO
{
    public partial class Mps000118PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public HIS_DHST hisDhst { get; set; }
        public HIS_SERVICE_REQ HisPrescription { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }
        public HIS_SERVICE_REQ hisServiceReq_Exam { get; set; }
        public Mps000118ADO Mps000118ADO { get; set; }
        public HIS_TREATMENT hisTreatment { get; set; }// lấy thời gian hẹn khám
        public long? _KeyUseForm;

        public HIS_SERVICE_REQ hisServiceReq_CurentExam { get; set; }
    }

    public class Mps000118ADO : V_HIS_PATIENT
    {
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string TITLE { get; set; }
        public string REQUEST_PHONE { get; set; }
        public string EXECUTE_PHONE { get; set; }
        public string MEDI_RECORD_STORE_CODE { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        public decimal? PRES_AMOUNT { get; set; }
    }
}
