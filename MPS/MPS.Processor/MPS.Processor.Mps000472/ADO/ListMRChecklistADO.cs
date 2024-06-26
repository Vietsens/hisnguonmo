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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000472.ADO
{
    class ListMRChecklistADO
    {
        public short? IS_SELF_CHECK { get; set; }
        public long MR_CHECK_ITEM_ID { get; set; }
        public long MR_CHECK_SUMMARY_ID { get; set; }
        public string NOTE { get; set; }

        public Dictionary<string, string> DicExecuteRole { get; set; }

        public ListMRChecklistADO() { }

        public ListMRChecklistADO(List<MRChecklistADO> data)
        {
            try
            {
                DicExecuteRole = new Dictionary<string, string>();
                foreach (var item in data)
                {
                    this.IS_SELF_CHECK = data.FirstOrDefault().IS_SELF_CHECK;
                    this.MR_CHECK_ITEM_ID = data[0].MR_CHECK_ITEM_ID;
                    this.MR_CHECK_SUMMARY_ID = data[0].MR_CHECK_SUMMARY_ID;

                    string ColumnName = String.Format("IS_CHECKER_CHECK{0}", item.Stt);

                    string RowData = item.IS_CHECKER_CHECK != null ? item.IS_CHECKER_CHECK.ToString() : "";

                    DicExecuteRole.Add(ColumnName, RowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
