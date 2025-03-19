/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
