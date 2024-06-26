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
using MPS.Processor.Mps000078.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000078
{
    class ImpMestADO : Mps000078ADO
    {
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public string TDL_PATIENT_NAME { get; set; }

        public ImpMestADO(V_HIS_IMP_MEST_MEDICINE datas, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved, HIS_IMP_MEST imp)
            : base(new List<V_HIS_IMP_MEST_MEDICINE>() { datas }, _impMestSttId, HisImpMestSttId__Imported, HisImpMestSttId__Approved)
        {
            try
            {
                if (imp != null)
                {
                    this.TDL_PATIENT_ADDRESS = imp.TDL_PATIENT_ADDRESS;
                    this.TDL_PATIENT_CODE = imp.TDL_PATIENT_CODE;
                    this.TDL_PATIENT_DOB = imp.TDL_PATIENT_DOB;
                    this.TDL_PATIENT_FIRST_NAME = imp.TDL_PATIENT_FIRST_NAME;
                    this.TDL_PATIENT_GENDER_NAME = imp.TDL_PATIENT_GENDER_NAME;
                    this.TDL_PATIENT_LAST_NAME = imp.TDL_PATIENT_LAST_NAME;
                    this.TDL_PATIENT_NAME = imp.TDL_PATIENT_NAME;
                }

                if (datas != null)
                {
                    this.REQ_LOGINNAME = datas.REQ_LOGINNAME;
                    this.REQ_USERNAME = datas.REQ_USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ImpMestADO(V_HIS_IMP_MEST_MATERIAL datas, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved, HIS_IMP_MEST imp)
            : base(new List<V_HIS_IMP_MEST_MATERIAL>() { datas }, _impMestSttId, HisImpMestSttId__Imported, HisImpMestSttId__Approved)
        {
            try
            {
                if (imp != null)
                {
                    this.TDL_PATIENT_ADDRESS = imp.TDL_PATIENT_ADDRESS;
                    this.TDL_PATIENT_CODE = imp.TDL_PATIENT_CODE;
                    this.TDL_PATIENT_DOB = imp.TDL_PATIENT_DOB;
                    this.TDL_PATIENT_FIRST_NAME = imp.TDL_PATIENT_FIRST_NAME;
                    this.TDL_PATIENT_GENDER_NAME = imp.TDL_PATIENT_GENDER_NAME;
                    this.TDL_PATIENT_LAST_NAME = imp.TDL_PATIENT_LAST_NAME;
                    this.TDL_PATIENT_NAME = imp.TDL_PATIENT_NAME;
                }

                if (datas != null)
                {
                    this.REQ_LOGINNAME = datas.REQ_LOGINNAME;
                    this.REQ_USERNAME = datas.REQ_USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
