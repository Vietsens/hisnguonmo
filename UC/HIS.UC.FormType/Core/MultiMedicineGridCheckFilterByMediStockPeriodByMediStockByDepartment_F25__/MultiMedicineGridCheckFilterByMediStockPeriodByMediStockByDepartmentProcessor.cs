using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment
{
    class MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartmentProcessor : ProcessorBase, IProcessorGenerate
    {
        UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment ucMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment;
        object generateRDO;

        internal MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartmentProcessor(V_SAR_RETY_FOFI config, object paramRDO)
            : base(config)
        {
            this.generateRDO = paramRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment(config, generateRDO);
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