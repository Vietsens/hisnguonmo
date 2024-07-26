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
using ACS.EFMODEL.DataModels;
using HIS.UC.FormType.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    public class HisMultiGetByString
    {
        public static List<DataGet> GetByString(string value, string key)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            List<DataGet> datasuft = null;

            if (value.StartsWith("HIS"))
            {
                try
                {
                    datasuft = HisGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("AOS"))
            {
                try
                {
                    datasuft = AosGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("THE"))
            {
                try
                {
                    datasuft = TheGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("ACS"))
            {
                try
                {
                    datasuft = AcsGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("SAR"))
            {
                try
                {
                    datasuft = SarGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("SDA"))
            {
                try
                {
                    datasuft = SdaGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("\"INPUT_DATA"))
            {
                try
                {
                    datasuft = InputData.Get(value);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (value.StartsWith("\"MRSINPUT_"))
            {
                try
                {
                    datasuft = Input.Get(value);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else
            {
                try
                {
                    datasuft = OtherGetString.Get(value, key);
                    datasuft = datasuft.OrderBy(o => o.NAME).ToList();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            return datasuft;
        }

        internal static List<DataGet> GetByStringLimit(string _FDO, string[] _limitCodes, ref string OutPut0)
        {
            List<DataGet> datasuft = null;
            try
            {
                datasuft = HisMultiGetByString.GetByString(_FDO, null);
                if (datasuft != null && _limitCodes != null)
                {
                    datasuft = datasuft.Where(o => _limitCodes.Contains(o.CODE)).ToList();
                   
                }
                 if (datasuft != null)
                {
                    var HasOutPut0 = datasuft.Where(o => o.IS_OUTPUT0 == true).ToList();
                    if (string.IsNullOrEmpty(OutPut0) && HasOutPut0.Count > 0)
                    {
                        OutPut0 = string.Join(",", HasOutPut0.Select(o => o.CODE).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }
    }

    public class DataGet
    {
        public bool IsChecked { get; set; }
        public long ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public long PARENT { get; set; }
        public long GRAND_PARENT { get; set; }
        public bool? IS_OUTPUT0 { get; set; }
    }
}
