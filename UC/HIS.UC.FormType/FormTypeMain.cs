//using HIS.UC.FormType.GetValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.FormType
{
    public class FormTypeMain
    {
        public static object Run(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI data, object generateRDO)
        {
            object result = null;
            try
            {
                IProcessorGenerate processor = ProcessorFactory.MakeProcessorBase(data, generateRDO);
                if (processor != null)
                {
                    result = processor.Run();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
