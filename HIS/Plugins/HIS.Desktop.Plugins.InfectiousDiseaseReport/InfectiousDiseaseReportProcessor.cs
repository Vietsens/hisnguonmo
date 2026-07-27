/* IVT
 * @Project : hisnguonmo
 * Plugin: HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Báo cáo ca bệnh truyền nhiễm lên cổng giám sát quốc gia (ECDS).
 */
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.InfectiousDiseaseReport",   // Plugin ID (ModuleLink)
        "Báo cáo ca bệnh truyền nhiễm (ECDS)",           // Tên hiển thị
        "Bussiness",                                      // Nhóm chức năng
        20,                                               // Độ ưu tiên
        "thuoc.png",                                      // Icon
        "E",                                              // Group
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class InfectiousDiseaseReportProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public InfectiousDiseaseReportProcessor()
        {
            param = new CommonParam();
        }

        public InfectiousDiseaseReportProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                InfectiousDiseaseReport.IInfectiousDiseaseReport behavior =
                    InfectiousDiseaseReport.InfectiousDiseaseReportFactory.MakeIControl(param, args);
                result = behavior != null ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            return true;
        }
    }
}
