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
