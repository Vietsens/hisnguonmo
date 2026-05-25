using Inventec.Desktop.Core.Tools;
using Inventec.Desktop.Core;
using System;
using MOS.EFMODEL.DataModels;
using Inventec.Core;

namespace HIS.Desktop.Plugins.InviteConsultation.InviteConsultation
{
    class InviteConsultationBehavior : Tool<IDesktopToolContext>, IinviteConsultation
    {
        Inventec.Desktop.Common.Modules.Module Module;
        L_HIS_TREATMENT_BED_ROOM bed = null;
        bool isEdit = false;
        HIS_SPECIALIST_EXAM exam = null;
        V_HIS_SERVICE_REQ serviceReq = null;

        internal InviteConsultationBehavior()
            : base()
        {
        }

        internal InviteConsultationBehavior(Inventec.Desktop.Common.Modules.Module moduleData, L_HIS_TREATMENT_BED_ROOM lBedRoom, HIS_SPECIALIST_EXAM hisExam, bool isEdit) : base()
        {
            Module = moduleData;
            bed = lBedRoom;
            exam = hisExam;
            this.isEdit = isEdit;
        }

        internal InviteConsultationBehavior(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ serviceReqData) : base()
        {
            Module = moduleData;
            serviceReq = serviceReqData;
        }

        object IinviteConsultation.Run()
        {
            object result = null;
            try
            {
                if (serviceReq != null)
                {
                    result = new frmInviteConsultation(Module, serviceReq);
                }
                else
                {
                    result = new frmInviteConsultation(Module, bed, exam, isEdit);
                }
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
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
