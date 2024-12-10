using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.ServiceGroupCombo_F32__
{
    class ServiceGroupComboProcessor  : ProcessorBase, IProcessorGenerate
    {
        object entity;
        internal ServiceGroupComboProcessor(V_SAR_RETY_FOFI config, object generateRDO)
            : base(config)
        {
            this.entity = generateRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCServiceGroupCombo(config, entity);
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
