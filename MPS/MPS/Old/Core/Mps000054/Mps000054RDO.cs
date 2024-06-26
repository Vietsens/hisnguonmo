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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;

namespace MPS.Core.Mps000054
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000054RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal HIS_DHST dhsts { get; set; }
        internal V_HIS_PRESCRIPTION serviceReq { get; set; }
        internal List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines { get; set; }
        string bedRoomName;
        string medi_stock_name;
        string serviceReqCode;

        public Mps000054RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            TreatmentADO currentHisTreatment,
            HIS_DHST dhsts,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string medi_stock_name,
            string serviceReqCode
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentHisTreatment = currentHisTreatment;
                this.dhsts = dhsts;
                this.serviceReq = serviceReq;
                this.expMestMedicines = expMestMedicines;
                this.medi_stock_name = medi_stock_name;
                this.serviceReqCode = serviceReqCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (!string.IsNullOrEmpty(medi_stock_name))
                {
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.MEDI_STOCK_NAME, medi_stock_name));
                }
                if (!string.IsNullOrEmpty(serviceReqCode))
                {
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.SERVICE_REQ_TH, serviceReqCode));
                }
                if (expMestMedicines != null)
                {
                    var aa = expMestMedicines.FirstOrDefault();
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.EXP_MEST_CODE, aa.EXP_MEST_CODE));
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.USE_DATE_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMestMedicines.Max(o => o.USE_TIME_TO ?? 0))));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.EXP_MEST_CODE, ""));
                }
                if (serviceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000054ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.USE_TIME ?? 0)));

                }
                if (dhsts!= null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(dhsts, keyValues, false); 
                }
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PRESCRIPTION>(serviceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
