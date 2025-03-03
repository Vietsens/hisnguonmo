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
    class HisTreatmentPagingGetBehavior:IPagingGet
    {
        string _keyWord;
        CommonParam _param;
        internal HisTreatmentPagingGetBehavior(CommonParam param, string keyWord)
        {
            this._keyWord = keyWord;
            this._param = param;
        }

        ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> IPagingGet.Run()
        {
            ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> result = new ApiResultObject<List<DataGet>>();
            try
            {
                ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>> apiResult = new ApiResultObject<List<HIS_TREATMENT>>();
                object Treatment = apiResult;
                HisTreatmentFilter filter = new HisTreatmentFilter();
                if (!string.IsNullOrWhiteSpace(_keyWord))
                {
                    string code = _keyWord.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                object ft = filter;
                FormTypeDelegate.PagingGet(this._param, ft, ref Treatment);
                apiResult = Treatment as ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>;
                if (apiResult != null)
                {
                    result.Data = apiResult.Data.Select(o => new DataGet { ID = o.ID, CODE = o.TREATMENT_CODE, NAME = o.TDL_PATIENT_NAME}).ToList();
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

        private  bool checkDigit(string s)
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
