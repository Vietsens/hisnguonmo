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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.Location;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.AssignService
{
    public partial class frmDetailsSereServ : Form
    {
        List<HIS_SERE_SERV> dataList { get; set; }
        RefeshReference refeshReference { get; set; }
        Action<bool> CloseForm { get; set; }
        public frmDetailsSereServ(List<HIS_SERE_SERV> data, RefeshReference refeshReference)
        {
            InitializeComponent();
            try
            {
                this.dataList = data;
                this.refeshReference = refeshReference;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDetailsSereServ_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = dataList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDetailsSereServ_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                refeshReference();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                refeshReference();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_STR")
                        {
                            try
                            {
                                e.Value = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.ID == data.TDL_EXECUTE_ROOM_ID).Select(o => o.EXECUTE_ROOM_NAME).FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_ROOM_NAME_STR")
                        {
                            try
                            {
                                e.Value = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == data.TDL_REQUEST_ROOM_ID).Select(o => o.ROOM_NAME).FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
