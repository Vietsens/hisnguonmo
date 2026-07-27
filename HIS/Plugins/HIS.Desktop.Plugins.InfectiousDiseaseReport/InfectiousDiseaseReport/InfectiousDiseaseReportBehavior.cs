/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Behavior: parse args (Module, HIS_TREATMENT, RefeshReference).
 * - Có HIS_TREATMENT  -> mở Form chi tiết (đẩy 1 ca).
 * - Không có          -> mở Form danh sách (đẩy hàng loạt) [TODO].
 */
using HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.InfectiousDiseaseReport
{
    class InfectiousDiseaseReportBehavior : Tool<IDesktopToolContext>, IInfectiousDiseaseReport
    {
        object[] entity;

        internal InfectiousDiseaseReportBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IInfectiousDiseaseReport.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_TREATMENT treatment = null;
                HIS.Desktop.Common.RefeshReference dlgRefresh = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        else if (entity[i] is HIS_TREATMENT)
                            treatment = (HIS_TREATMENT)entity[i];
                        else if (entity[i] is HIS.Desktop.Common.RefeshReference)
                            dlgRefresh = (HIS.Desktop.Common.RefeshReference)entity[i];
                    }
                }

                if (moduleData == null)
                    return null;

                // Master-detail: luôn mở form (danh sách y lệnh bên trái + chi tiết bên phải).
                // - Có HIS_TREATMENT: đổ luôn chi tiết ca đó.
                // - Không có: hiển thị danh sách, người dùng chọn dòng để đổ chi tiết.
                return new frmInfectiousDiseaseReport(moduleData, treatment, dlgRefresh);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
