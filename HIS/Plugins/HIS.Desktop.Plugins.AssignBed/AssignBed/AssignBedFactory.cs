using HIS.Desktop.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    class AssignBedFactory
    {
        internal static IAssignBed MakeIAssignBed(CommonParam param, object[] data)
        {
            IAssignBed result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            AssignServiceADO assignServiceADO = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is AssignServiceADO)
                            {
                                assignServiceADO = (AssignServiceADO)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                        }

                        //Bắt buộc các chỗ sử dụng module này phải truyền vào phòng đang làm việc (roomId)
                        if (assignServiceADO == null) throw new NullReferenceException("assignServiceADO");
                        if (moduleData == null) throw new NullReferenceException("moduleData");
                        if (moduleData.RoomId <= 0) throw new NullReferenceException("moduleData.RoomId = " + moduleData.RoomId);

                        result = new AssignBedBehavior(param, (AssignServiceADO)assignServiceADO, moduleData);
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
