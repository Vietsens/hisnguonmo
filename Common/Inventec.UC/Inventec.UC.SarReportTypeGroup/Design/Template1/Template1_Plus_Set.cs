using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Design.Template1
{
    internal partial class Template1
    {
        internal bool SetDelegateRowCellClick_Click(RowCellClickDelegate rowCellClick)
        {
            bool result = false;
            try
            {
                this._rowCellClick = rowCellClick;
                if (this._rowCellClick != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowCellClick), rowCellClick));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateUpdateDataForPanging(UpdateReportTypeGroup updateData)
        {
            bool result = false;
            try
            {
                this._updateData = updateData;
                if (this._updateData != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateData), updateData));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal void SetDataForGridControl(object data)
        {
            try
            {
                gridViewReportTypeGroup.FocusedRowHandle = 0;
                gridControlReportTypeGroup.BeginUpdate();
                gridControlReportTypeGroup.DataSource = data;
                gridControlReportTypeGroup.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
