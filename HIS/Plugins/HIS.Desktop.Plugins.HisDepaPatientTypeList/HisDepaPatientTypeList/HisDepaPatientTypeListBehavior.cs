using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.HisDepaPatientTypeList
{
    class HisDepaPatientTypeListBehavior : Tool<IDesktopToolContext>, IHisDepaPatientTypeList
    {
        object[] entity;

        internal HisDepaPatientTypeListBehavior() : base() { }

        internal HisDepaPatientTypeListBehavior(CommonParam param, object[] data) : base()
        {
            entity = data;
        }

        object IHisDepaPatientTypeList.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                DepaPatientTypeInputADO inputAdo = null;
                DelegateSelectData callBack = null;
                long? serviceIdArg = null;
                List<HIS_DEPA_PATIENT_TYPE> depaListArg = null;
                bool[] flagsArg = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        // Boxing rules: long? co value -> box thanh `long`; null -> entry trong object[] = null.
                        // KHONG check `is long?` vi luon false sau boxing.
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        else if (entity[i] is DepaPatientTypeInputADO)
                            inputAdo = (DepaPatientTypeInputADO)entity[i];
                        else if (entity[i] is DelegateSelectData)
                            callBack = (DelegateSelectData)entity[i];
                        else if (entity[i] is long)
                            serviceIdArg = (long)entity[i];
                        else if (entity[i] is List<HIS_DEPA_PATIENT_TYPE>)
                            depaListArg = (List<HIS_DEPA_PATIENT_TYPE>)entity[i];
                        else if (entity[i] is bool[])
                            flagsArg = (bool[])entity[i];
                    }
                }

                if (inputAdo == null)
                {
                    inputAdo = new DepaPatientTypeInputADO
                    {
                        ServiceId = serviceIdArg,
                        DepaPatientTypes = depaListArg ?? new List<HIS_DEPA_PATIENT_TYPE>(),
                        IsCalledApi = flagsArg != null && flagsArg.Length > 0 ? flagsArg[0] : false,
                        IsClickPick = flagsArg != null && flagsArg.Length > 1 ? flagsArg[1] : false
                    };
                }

                result = new frmHisDepaPatientTypeList(moduleData, inputAdo, callBack);
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
