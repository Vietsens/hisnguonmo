using HIS.UC.FormType.DepartmentCombo;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace HIS.UC.FormType
{
    abstract class ProcessorBase
    {
        protected V_SAR_RETY_FOFI config;
        protected Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

        internal ProcessorBase(V_SAR_RETY_FOFI configData)
        {
            this.config = configData;
        }

        //abstract internal object GetValue();

    }
}
