﻿using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.HeinTreatmentTypeRadio
{
    class HeinTreatmentTypeRadioProcessor : ProcessorBase, IProcessorGenerate
    {
        UCHeinTreatmentTypeRadio ucHeinTreatType;
        object generateRDO;

        internal HeinTreatmentTypeRadioProcessor(V_SAR_RETY_FOFI config, object paramRDO)
            : base(config)
        {
            this.generateRDO = paramRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCHeinTreatmentTypeRadio(config, generateRDO);
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
