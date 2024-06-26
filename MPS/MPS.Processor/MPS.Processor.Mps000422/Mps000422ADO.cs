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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000422
{
    public class Mps000422ADO
    {

        public string BLOOD_TYPE_NAME { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public string BLOOD_HR_CODE { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string TDL_TREATMENT_CODE { get; set; }
        public long AMOUNT { get; set; }
        public string TUI_THU { get; set; }

        public Mps000422ADO(V_HIS_EXP_MEST expMest, V_HIS_EXP_MEST_BLTY_REQ mestBLTYReq, int? index)
        {
            this.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
            this.TDL_PATIENT_DOB = expMest.TDL_PATIENT_DOB;
            this.TDL_PATIENT_GENDER_NAME = expMest.TDL_PATIENT_GENDER_NAME;
            this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
            this.BLOOD_ABO_CODE = mestBLTYReq.BLOOD_ABO_CODE;
            this.BLOOD_HR_CODE = mestBLTYReq.BLOOD_RH_CODE;
            this.REQUEST_DEPARTMENT_NAME = expMest.REQ_DEPARTMENT_NAME;
            this.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
            this.BLOOD_TYPE_NAME = mestBLTYReq.BLOOD_TYPE_NAME;
            this.AMOUNT = mestBLTYReq.AMOUNT;
            if (index.HasValue)
            {
                this.TUI_THU = String.Format("{0}/{1}", index, mestBLTYReq.AMOUNT);
            }
        }
    }
}
