using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Set.Delegate.RowCellClick
{
    class RowCellClickBehavior : IRowCellClick
    {
        private RowCellClickDelegate Data { get; set; }

        internal RowCellClickBehavior(RowCellClickDelegate data)
        {
            this.Data = data;
        }

        bool IRowCellClick.Set(System.Windows.Forms.UserControl UC)
        {
            bool result = false;
            try
            {
                if (UC != null)
                {
                    if (UC.GetType() == typeof(Design.Template1.Template1))
                    {
                        Design.Template1.Template1 ucReportTypeGroup = (Design.Template1.Template1)UC;
                        result = ucReportTypeGroup.SetDelegateRowCellClick_Click(Data);
                    }

                    if (!result)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
