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

namespace HIS.Desktop.Plugins.MedicalStoreV2.ADO
{
    public class MediRecordADO : V_HIS_MEDI_RECORD_1
    {
        public bool CheckTreatment { get; set; }
        public bool CheckStore { get; set; }

        // Formatted admission/discharge times of the medical record's treatments (backend returns IN_TIME/OUT_TIME
        // as comma-separated yyyyMMddHHmmss values, same as V_HIS_MEDI_RECORD_2).
        public string INTIME_SPLCONCAT { get; set; }
        public string OUTTIME_SPLCONCAT { get; set; }

        public MediRecordADO() { }

        public MediRecordADO(V_HIS_MEDI_RECORD_1 data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MediRecordADO>(this, data);
                //this.CheckTreatment = data.DATA_STORE_ID != null ? true : false;
                if (this.IN_TIME != null)
                {
                    var inT = this.IN_TIME.Split(',');
                    List<string> lst = new List<string>();
                    for (int i = 0; i < inT.Length; i++)
                    {
                        lst.Add(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Int64.Parse(inT[i])));
                    }
                    INTIME_SPLCONCAT = String.Join(", ", lst);
                }
                if (this.OUT_TIME != null)
                {
                    var outT = this.OUT_TIME.Split(',');
                    List<string> lst = new List<string>();
                    for (int i = 0; i < outT.Length; i++)
                    {
                        lst.Add(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Int64.Parse(outT[i])));
                    }
                    OUTTIME_SPLCONCAT = String.Join(", ", lst);
                }
            }
        }
    }
}
