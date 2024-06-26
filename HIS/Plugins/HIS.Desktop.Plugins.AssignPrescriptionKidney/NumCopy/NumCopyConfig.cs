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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.NumCopy
{
    class NumCopyConfig
    {
        private const string numCopy = "CONFIG_KEY__HIS_DESKTOP_PRINT_NOW__NUM_COPY";

        public static List<NumCopyADO> NumCopys
        {
            get
            {
                List<NumCopyADO> list = new List<NumCopyADO>();
                var lstNum = GetTypes(numCopy);
                if (lstNum != null && lstNum.Count > 0)
                {
                    foreach (var item in lstNum)
                    {
                        try
                        {
                            string[] tmp = item.Split(':');
                            if (tmp != null && tmp.Length >= 2)
                            {
                                NumCopyADO n = new NumCopyADO();
                                var patient = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == tmp[0]).ToList();
                                if (patient != null && patient.Count == 1)
                                {
                                    n.id = patient.FirstOrDefault().ID;
                                    n.num = Inventec.Common.TypeConvert.Parse.ToInt32(tmp[1]);
                                    list.Add(n);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
                return list;
            }
        }

        private static List<string> GetTypes(string code)
        {
            List<string> result = new List<string>();
            try
            {
                //ConfigApplicationWorker.Get<string>(code)
                string value = ConfigApplicationWorker.Get<string>(code);
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                result.AddRange(value.Split(new char[] { ',' }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<string>();
            }
            return result;
        }
    }
}
