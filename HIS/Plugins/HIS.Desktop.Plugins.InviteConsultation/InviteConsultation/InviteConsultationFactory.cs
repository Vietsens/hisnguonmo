using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InviteConsultation.InviteConsultation
{
    class InviteConsultationFactory
    {
        internal static IinviteConsultation MakeIControl(CommonParam commonParam, object[] data)
        {
            IinviteConsultation result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            L_HIS_TREATMENT_BED_ROOM bed = null;
            HIS_SPECIALIST_EXAM exam = null;
            bool isEdit = false;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is L_HIS_TREATMENT_BED_ROOM)
                            {
                                bed = (L_HIS_TREATMENT_BED_ROOM)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is HIS_SPECIALIST_EXAM)
                            {
                                exam = (HIS_SPECIALIST_EXAM)data[i];
                            }
                            else if (data[i] is bool)
                            {
                                isEdit = (bool)data[i];
                            }
                        }

                        if (moduleData != null)
                        {
                            result = new InviteConsultationBehavior(moduleData, bed, exam, isEdit);
                        }
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
