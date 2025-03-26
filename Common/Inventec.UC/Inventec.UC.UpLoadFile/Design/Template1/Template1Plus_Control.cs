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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.UpLoadFile.Design.Template1
{
    internal partial class Template1
    {
        private void gridViewUpLoadFile_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    Data.DataShowGridControl data = (Data.DataShowGridControl)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (status == 99)
                {
                    timer1.Enabled = false;
                    status = 9;
                    gridViewUpLoadFile.SetRowCellValue(index, "FILE_STATUS", 99);
                    gridViewUpLoadFile.RefreshRowCell(index, gridViewUpLoadFile.Columns["FILE_STATUS"]);
                }
                else
                {
                    gridViewUpLoadFile.SetRowCellValue(index, "FILE_STATUS", status + 1);
                    gridViewUpLoadFile.RefreshRowCell(index, gridViewUpLoadFile.Columns["FILE_STATUS"]);
                    status = status + 10;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
     
    }
}
