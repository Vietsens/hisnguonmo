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

namespace MPS.Processor.Mps000234.PDO
{
    public partial class Mps000234PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public HIS_DHST hisDhst { get; set; }
        public HIS_SERVICE_REQ HisPrescription { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }
        public HIS_SERVICE_REQ hisServiceReq_Exam { get; set; }
        public Mps000234ADO Mps000234ADO { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock { get; set; }

        public HIS_SERVICE_REQ hisServiceReq_CurentExam { get; set; }
    }

    public class Mps000234ADO : V_HIS_PATIENT
    {
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string TITLE { get; set; }
        public string NUMBER_ORDER_OF_DAY { get; set; }
        public virtual V_HIS_TREATMENT HisTreatment { get; set; }//virtual sẽ không bị add vào danh sách key đơn
        public string REQUEST_PHONE { get; set; }
        public string EXECUTE_PHONE { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        //public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        public string CREATOR_NAME { get; set; }
        public decimal? PRES_AMOUNT { get; set; }
        public string MEDICINE_TYPE_DESCRIPTION { get; set; }
        public decimal? USING_COUNT_NUMBER { get; set; }
    }
}
