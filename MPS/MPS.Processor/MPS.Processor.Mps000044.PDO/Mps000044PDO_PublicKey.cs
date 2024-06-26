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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MOS.SDO;
using MPS.Processor.Mps000044.PDO;

namespace MPS.Processor.Mps000044.PDO
{
    /// <summary>
    /// In thuoc
    /// </summary>
    public partial class Mps000044PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public HIS_DHST dhsts { get; set; }
        public HIS_SERVICE_REQ vHisPrescription5 { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }
        public Mps000044ADO Mps000044ADO { get; set; }
        public HIS_TREATMENT hisTreatment { get; set; }// lấy thời gian hẹn khám
        public long? KeyUseForm;
        public HIS_SERVICE_REQ hisServiceReq_CurentExam { get; set; }

    }

    public class Mps000044ADO : V_HIS_PATIENT
    {
        public string MEDI_STOCK_NAME { get; set; }
        public string BED_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string KEY_NAME_TITLES { get; set; }
        public string REQUEST_PHONE { get; set; }
        public string EXECUTE_PHONE { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        public decimal? PRES_AMOUNT { get; set; }
        public string MEDICINE_TYPE_DESCRIPTION { get; set; }

        public decimal? USING_COUNT_NUMBER { get; set; }
    }
}
