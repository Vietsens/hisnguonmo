using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.CallPatientTypeAlter;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;


using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.CallPatientTypeAlter;
using MOS.SDO;


namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public sealed class PatientTypeAlterBehavior : Tool<IDesktopToolContext>, IPatientTypeAlter
    {
        Inventec.Desktop.Common.Modules.Module module;
        PatientTypeDepartmentADO HisTreatmentLogSDO;
        long treatmentId;
        bool? isView;
        RefeshReference RefeshReference;
        List<PatientTypeDepartmentADO> lstTreatmentLog = null;

        public PatientTypeAlterBehavior()
            : base()
        {
        }

        public PatientTypeAlterBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module module, PatientTypeDepartmentADO HisTreatmentLogSDO, long treatmentId, bool? isView, List<PatientTypeDepartmentADO> _lstTreatmentLog, RefeshReference RefeshReference)
            : base()
        {
            this.module = module;
            this.HisTreatmentLogSDO = HisTreatmentLogSDO;
            this.treatmentId = treatmentId;
            this.isView = isView;
            this.RefeshReference = RefeshReference;
            this.lstTreatmentLog = _lstTreatmentLog;
        }

        object IPatientTypeAlter.Run()
        {
            try
            {
                if (treatmentId != 0) return new frmPatientTypeAlter(module, treatmentId, isView, lstTreatmentLog, RefeshReference);
                else return new frmPatientTypeAlter(module, HisTreatmentLogSDO, isView, lstTreatmentLog, RefeshReference);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}
