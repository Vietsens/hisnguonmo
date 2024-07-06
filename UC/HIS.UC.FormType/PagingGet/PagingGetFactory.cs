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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.PagingGet
{
    class PagingGetFactory
    {
        internal static IPagingGet MakeIPagingGet(CommonParam param,string jsonOutput,string keyWord,long? parentKey)
        {
            IPagingGet result = null;
            try
            {
                if (jsonOutput.Contains("PATIENT_ID"))
                {
                    result = new HisPatientPagingGetBehavior(param, keyWord);
                }
                else if (jsonOutput.Contains("TREATMENT_ID"))
                {
                    result = new HisTreatmentPagingGetBehavior(param, keyWord);
                }
                else if (jsonOutput.Contains("TREATMENT_BED_ROOM_ID"))
                {
                    result = new HisTreatmentBedRoomGetBehavior(param, keyWord, parentKey);
                }
                else if (jsonOutput.Contains("MEDICINE_ID"))
                {
                    result = new HisMedicinePagingGetBehavior(param, keyWord, parentKey);
                }
                else if (jsonOutput.Contains("MATERIAL_ID"))
                {
                    result = new HisMaterialPagingGetBehavior(param, keyWord, parentKey);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + jsonOutput, ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
