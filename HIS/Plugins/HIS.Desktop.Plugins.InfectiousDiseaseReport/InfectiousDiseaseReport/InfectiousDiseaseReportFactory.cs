/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport */
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.InfectiousDiseaseReport
{
    class InfectiousDiseaseReportFactory
    {
        internal static IInfectiousDiseaseReport MakeIControl(CommonParam param, object[] data)
        {
            IInfectiousDiseaseReport result = null;
            try
            {
                result = new InfectiousDiseaseReportBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory không khởi tạo được đối tượng."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
