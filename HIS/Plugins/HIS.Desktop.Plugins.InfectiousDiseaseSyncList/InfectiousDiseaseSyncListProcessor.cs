/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Đồng bộ danh sách ca bệnh truyền nhiễm lên cổng ECDS (mô hình KskSyncList).
 * Chọn nhiều ca -> đẩy hàng loạt; bấm Xem/Sửa -> mở plugin chi tiết InfectiousDiseaseReport.
 */
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.InfectiousDiseaseSyncList",
        "Đồng bộ danh sách ca bệnh truyền nhiễm (ECDS)",
        "Bussiness",
        21,
        "thuoc.png",
        "E",
        Module.MODULE_TYPE_ID__UC,
        true,
        true)]
    public class InfectiousDiseaseSyncListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public InfectiousDiseaseSyncListProcessor()
        {
            param = new CommonParam();
        }

        public InfectiousDiseaseSyncListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                InfectiousDiseaseSyncList.IInfectiousDiseaseSyncList behavior =
                    InfectiousDiseaseSyncList.InfectiousDiseaseSyncListFactory.MakeIControl(param, args);
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
