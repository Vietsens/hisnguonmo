using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.ExpMestTypeComboCheck
{
    class ExpMestTypeComboCheckProcessor : ProcessorBase, IProcessorGenerate
    {
        UCExpMestTypeComboCheck ucExpMestTypeComboCheck;
        object generateRDO;

        internal ExpMestTypeComboCheckProcessor(V_SAR_RETY_FOFI config, object paramRDO)
            : base(config)
        {
            this.generateRDO = paramRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCExpMestTypeComboCheck(config, generateRDO);
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
