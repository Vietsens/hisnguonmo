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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Design.Template1
{
    internal partial class Template1
    {
        internal void SetInitPaging()
        {
            try
            {
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridControl, param, pageSize, this.gridControlReportType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FillDataToGridControl(object param)
        {
            try
            {
                Data.SearchData data = new Data.SearchData();
                SetSearchData(data);
                CommonParam resultParam = new CommonParam();
                if (_updateData != null)
                {
                    gridViewReportType.BeginUpdate();
                    gridViewReportType.GridControl.DataSource = _updateData(data, param, ref resultParam);
                    gridViewReportType.EndUpdate();
                    rowCount = resultParam.Limit ?? 0;
                    dataTotal = resultParam.Count ?? 0;
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
