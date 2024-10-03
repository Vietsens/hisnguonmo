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

namespace MPS.Processor.Mps000215
{
    class Mps000215ADODetail : Mps000215ADO
    {
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public string TDL_PATIENT_NAME { get; set; }

        public Mps000215ADODetail(V_HIS_EXP_MEST _BcsExpMest, List<HIS_EXP_MEST_METY_REQ> req, List<V_HIS_MEDICINE_TYPE> _medicineTypes, List<V_HIS_EXP_MEST_MEDICINE> medicines, bool isReplace, HIS_TREATMENT dataPatient)
            : base(_BcsExpMest, req, _medicineTypes, medicines, isReplace)
        {
            if (dataPatient != null)
            {
                this.TDL_PATIENT_ADDRESS = dataPatient.TDL_PATIENT_ADDRESS;
                this.TDL_PATIENT_CODE = dataPatient.TDL_PATIENT_CODE;
                this.TDL_PATIENT_DOB = dataPatient.TDL_PATIENT_DOB;
                this.TDL_PATIENT_FIRST_NAME = dataPatient.TDL_PATIENT_FIRST_NAME;
                this.TDL_PATIENT_GENDER_NAME = dataPatient.TDL_PATIENT_GENDER_NAME;
                this.TDL_PATIENT_LAST_NAME = dataPatient.TDL_PATIENT_LAST_NAME;
                this.TDL_PATIENT_NAME = dataPatient.TDL_PATIENT_NAME;
            }
             
        }

        public Mps000215ADODetail(V_HIS_EXP_MEST _BcsExpMest, List<HIS_EXP_MEST_MATY_REQ> req, List<V_HIS_MATERIAL_TYPE> _materialTypes, List<V_HIS_EXP_MEST_MATERIAL> material, bool isReplace, HIS_TREATMENT dataPatient)
            : base(_BcsExpMest, req, _materialTypes, material, isReplace)
        {
            if (dataPatient != null)
            {
                this.TDL_PATIENT_ADDRESS = dataPatient.TDL_PATIENT_ADDRESS;
                this.TDL_PATIENT_CODE = dataPatient.TDL_PATIENT_CODE;
                this.TDL_PATIENT_DOB = dataPatient.TDL_PATIENT_DOB;
                this.TDL_PATIENT_FIRST_NAME = dataPatient.TDL_PATIENT_FIRST_NAME;
                this.TDL_PATIENT_GENDER_NAME = dataPatient.TDL_PATIENT_GENDER_NAME;
                this.TDL_PATIENT_LAST_NAME = dataPatient.TDL_PATIENT_LAST_NAME;
                this.TDL_PATIENT_NAME = dataPatient.TDL_PATIENT_NAME;
            }
        }
    }
}
