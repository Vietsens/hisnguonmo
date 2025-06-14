using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnalyzeMedicalImage
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
         "HIS.Desktop.Plugins.AnalyzeMedicalImage",
         "Phân tích hình ảnh y khoa",
         "",
         14,
         "",
         "",
         Module.MODULE_TYPE_ID__FORM,
         true,
         true)
      ]

    public class HisAnalyzeMedicallmageProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisAnalyzeMedicallmageProcessor()
        {
            param = new CommonParam();
        }
        public HisAnalyzeMedicallmageProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                long TreatmentId = 0;
                TimeSpan? TimeOut = null;
                DelegateSelectData DelSelectData = null;
                List<HIS_SERE_SERV> SereServs = null;

                if (args.GetType() == typeof(object[]))
                {
                    if (args != null && args.Count() > 0)
                    {
                        for (int i = 0; i < args.Count(); i++)
                        {
                            if (args[i] is AnalyzeImageADO)
                            {
                                TreatmentId = ((AnalyzeImageADO)args[i]).TreatmentId;
                                TimeOut = ((AnalyzeImageADO)args[i]).TimeOut;
                                DelSelectData = ((AnalyzeImageADO)args[i]).DelSelectData;
                                SereServs = ((AnalyzeImageADO)args[i]).SereServs;
                            }
                        }
                        LogSystem.Info("AnalyzeImageADO " + LogUtil.TraceData("AnalyzeImageADO ", args));
                    }
                }
                return result = new HIS.Desktop.Plugins.AnalyzeMedicalImage.Run.frmAnalyzeImage(TreatmentId, TimeOut, DelSelectData, SereServs);
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
