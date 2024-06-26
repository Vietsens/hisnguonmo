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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Notify
{
    public partial class frmNotifyAlert : Form
    {
        List<NotifyADO> listNotifyADO;
        public frmNotifyAlert(List<NotifyADO> _listNotifyADO)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.listNotifyADO = _listNotifyADO;

                if (_listNotifyADO != null && _listNotifyADO.Count > 0)
                {
                    foreach (var item in _listNotifyADO)
                    {
                        long? date = null;

                        if (item.CREATE_TIME != null)
                        {
                            var str = item.CREATE_TIME.ToString().Substring(0, 8);
                            date = Inventec.Common.TypeConvert.Parse.ToInt64(str + "000000");
                        }

                        item.MODIFY_TIME = date;
                    }

                    _listNotifyADO = _listNotifyADO.OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                gridControlNotifyAlert.BeginUpdate();
                gridControlNotifyAlert.DataSource = _listNotifyADO;
                gridControlNotifyAlert.EndUpdate();

                advBandedGridViewNotify.ExpandAllGroups();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void frmNotifyAlert_Load(object sender, EventArgs e)
        {

        }

        private void advBandedGridViewNotify_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
            {
                NotifyADO pData = (NotifyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                try
                {
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.MODIFY_TIME ?? 0);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void advBandedGridViewNotify_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var row = (NotifyADO)advBandedGridViewNotify.GetRow(e.RowHandle);
                if (row != null && row.Status == false)
                {
                    e.Appearance.Font = new Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
