using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHivGroupPatient.HisHivGroupPatient
{
    class GroupPatientFactory
    {
        internal static IGroupPatient MakeIControl(CommonParam param, object[] data)
        {
            IGroupPatient result = null;
            try
            {
                result = new GroupPatientBehavior(param, data);
                if (result == null)
                {
                    throw new NullReferenceException();
                }
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory không khởi tạo được đối tượng" + data.GetType().ToString() +
                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
