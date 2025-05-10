using HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExamSpecialist
{
    class HisExamSpecialistProcessor
    {
        class HisSdaLanguageProcessor
        {
            // Khai báo các thông số của module để khi mở ứng dụng có thể load đúng các dll 
            [ExtensionOf(typeof(DesktopRootExtensionPoint),
                "HIS.Desktop.Plugins.ExamSpecialist",
                "Danh mục",
                "Bussiness",
                14,
                "y-lenh.png",
                "A",
                // Tùy vào chức năng đang làm là form hay uc mà điền loại module 
                Module.MODULE_TYPE_ID__FORM,
                true,
                true)]
            public class SdaLanguageProcessor : ModuleBase, IDesktopRoot
            {
                CommonParam param;
                public SdaLanguageProcessor()
                {
                    param = new CommonParam();
                }
                public SdaLanguageProcessor(CommonParam paramBussiness)
                {
                    param = (paramBussiness != null ? paramBussiness : new CommonParam());
                }
                public object Run(object[] arge)
                {
                    object result = null;
                    try
                    {
                        IExamSpecialist behavior = ExamSpecialistFactory.MakeIControl(param, arge);
                        result = behavior != null ? (object)(behavior.Run()) : null;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        result = null;
                        //return result; 
                    }
                    return result;
                }
                public override bool IsEnable()
                {
                    bool result = false;
                    try
                    {
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        return result;
                    }
                    return result;
                }
            }
        }
    }
}
