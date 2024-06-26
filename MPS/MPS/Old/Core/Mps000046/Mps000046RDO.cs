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

namespace MPS.Core.Mps000046
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000046RDO : RDOBase
    {
        internal List<ExpMestMedicinePrintADO> MedicineExpmestTypeADOs { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST AggrExpMest { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_DEPARTMENT Department { get; set; }
        internal List<long> ServiceUnitIds { get; set; }
        internal List<long> UseFormIds { get; set; }
        internal List<long> RoomIds { get; set; }
        internal bool IsMedicine { get; set; }
        internal bool Ismaterial { get; set; }
        internal bool IsChemicalSustance { get; set; }
        internal bool IsBlood { get; set; }
        internal string keyNameTitles;
        internal long HisExpMestSttId__Approved { get; set; }
        internal long HisExpMestSttId__Draft { get; set; }
        internal long HisExpMestSttId__Exported { get; set; }
        internal long HisExpMestSttId__Rejected { get; set; }
        internal long HisExpMestSttId__Request { get; set; }

        List<V_HIS_PRESCRIPTION_4> listPres { get; set; }
        public Mps000046RDO(
            List<ExpMestMedicinePrintADO> medicineExpmestTypeADOs,
            MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST aggrExpMest,
            MOS.EFMODEL.DataModels.HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            bool isChemicalSustance,
            bool isBlood,
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
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.IsChemicalSustance = isChemicalSustance;
                this.IsBlood = isBlood;
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
                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));

                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST>(AggrExpMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(Department, keyValues, false);

                keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(AggrExpMest.CREATE_TIME ?? 0)));
                keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(AggrExpMest.CREATE_TIME ?? 0)));
                keyValues.Add(new KeyValue(Mps000046ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(AggrExpMest.CREATE_TIME ?? 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
