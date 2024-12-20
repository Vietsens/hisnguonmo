using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck
{
    class MultipleRoomComboCheckFilterByDepartmentComboCheckProcessor : ProcessorBase, IProcessorGenerate
    {
        object generateRDO;
        internal MultipleRoomComboCheckFilterByDepartmentComboCheckProcessor(V_SAR_RETY_FOFI config, object generateRDO)
            : base(config)
        {
            this.generateRDO = generateRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCMultipleRoomComboCheckFilterByDepartmentComboCheck(config, generateRDO);
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
