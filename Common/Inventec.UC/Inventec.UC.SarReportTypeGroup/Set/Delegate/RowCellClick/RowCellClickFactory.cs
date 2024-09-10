using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Set.Delegate.RowCellClick
{
    class RowCellClickFactory
    {
        internal static IRowCellClick MakeIRowCellClick(RowCellClickDelegate data)
        {
            IRowCellClick result = null;
            try
            {
                result = new RowCellClickBehavior(data);
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
