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
using MOS.SDO;

namespace MPS.Core.Mps000169
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000169RDO : RDOBase
    {
        internal List<ExpMestMedicinePrintADO> MedicineExpmestTypeADOs { get; set; }
        internal V_HIS_AGGR_EXP_MEST AggrExpMest { get; set; }
        internal HIS_DEPARTMENT Department { get; set; }
        internal List<V_HIS_PRESCRIPTION> listPrescription { get; set; }
        internal string keyNameTitles;
        internal long HisExpMestSttId__Approved;
        internal long HisExpMestSttId__Draft;
        internal long HisExpMestSttId__Exported;
        internal long HisExpMestSttId__Rejected;
        internal long HisExpMestSttId__Request;
        List<V_HIS_PRESCRIPTION_4> listPres { get; set; }

        public Mps000169RDO(
            List<ExpMestMedicinePrintADO> medicineExpmestTypeADOs,
            V_HIS_AGGR_EXP_MEST aggrExpMest,
            HIS_DEPARTMENT Department,
            List<V_HIS_PRESCRIPTION> listPrescription,
            string keyNameTitles,
            long HisExpMestSttId__Approved,
            long HisExpMestSttId__Draft,
            long HisExpMestSttId__Exported,
            long HisExpMestSttId__Rejected,
            long HisExpMestSttId__Request,
            List<V_HIS_PRESCRIPTION_4> listPres
            )
        {
            try
            {
                this.MedicineExpmestTypeADOs = medicineExpmestTypeADOs;
                this.AggrExpMest = aggrExpMest;
                this.Department = Department;
                this.listPrescription = listPrescription;
                this.keyNameTitles = keyNameTitles;
                this.HisExpMestSttId__Approved = HisExpMestSttId__Approved;
                this.HisExpMestSttId__Draft = HisExpMestSttId__Draft;
                this.HisExpMestSttId__Exported = HisExpMestSttId__Exported;
                this.HisExpMestSttId__Rejected = HisExpMestSttId__Rejected;
                this.HisExpMestSttId__Request = HisExpMestSttId__Request;
                this.listPres = listPres;
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
                if (listPres != null && listPres.Count > 0)
                {
                    var minTime = listPres.Min(p => p.INTRUCTION_TIME);
                    var maxTime = listPres.Max(p => p.INTRUCTION_TIME);
                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_AGGR_EXP_MEST>(AggrExpMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_DEPARTMENT>(Department, keyValues, false);

                keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(AggrExpMest.CREATE_TIME ?? 0)));
                keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(AggrExpMest.CREATE_TIME ?? 0)));
                keyValues.Add(new KeyValue(Mps000169ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(AggrExpMest.CREATE_TIME ?? 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
