using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.DepartmentCombo
{
    class DepartmentComboProcessor : ProcessorBase, IProcessorGenerate
    {
        UCDepartmentCombo ucDepartmentCombo;
        object generateRDO;

        internal DepartmentComboProcessor(V_SAR_RETY_FOFI config, object paramRDO)
            : base(config)
        {
            this.generateRDO = paramRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCDepartmentCombo(config, generateRDO);
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
