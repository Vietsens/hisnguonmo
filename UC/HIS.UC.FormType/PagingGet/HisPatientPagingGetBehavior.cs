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
using HIS.UC.FormType.HisMultiGetString;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.PagingGet
{
    class HisPatientPagingGetBehavior:IPagingGet
    {
        string _keyWord;
        CommonParam _param;
        internal HisPatientPagingGetBehavior(CommonParam param, string keyWord)
        {
            this._keyWord = keyWord;
            this._param = param;
        }

        ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> IPagingGet.Run()
        {
            ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> result = new ApiResultObject<List<DataGet>>();
            try
            {
                ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PATIENT>> apiResult = new ApiResultObject<List<HIS_PATIENT>>();
                object Patient = apiResult;
                HisPatientFilter filter = new HisPatientFilter();
                if (!string.IsNullOrWhiteSpace(_keyWord))
                {
                    string code = _keyWord.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                object ft = filter;
                FormTypeDelegate.PagingGet(this._param, ft, ref Patient);
                apiResult = Patient as ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>;
                if (apiResult != null)
                {
                    result.Data = apiResult.Data.Select(o => new DataGet { ID = o.ID, CODE = o.PATIENT_CODE, NAME = o.VIR_PATIENT_NAME }).ToList();
                    result.Param = apiResult.Param;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) != true)
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
    }
}
