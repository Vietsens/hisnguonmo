using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.MediStockComboFilterByDepartmentCombo
{
    class MediStockComboFilterByDepartmentComboProcessor : ProcessorBase, IProcessorGenerate
    {
        object generateRDO;
        internal MediStockComboFilterByDepartmentComboProcessor(V_SAR_RETY_FOFI config, object generateRDO)
            : base(config)
        {
            this.generateRDO = generateRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCMediStockComboFilterByDepartmentCombo(config, generateRDO);
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
