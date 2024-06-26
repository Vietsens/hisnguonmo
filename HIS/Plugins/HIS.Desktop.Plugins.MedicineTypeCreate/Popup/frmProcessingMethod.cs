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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.Popup
{
    public partial class frmProcessingMethod : Form
    {
        List<HIS_PROCESSING_METHOD> lstProcessingMethod { get; set; }
        Action<List<HIS_PROCESSING_METHOD>> SendList { get; set; }
        bool IsProcessing { get; set; }
        public frmProcessingMethod(Action<List<HIS_PROCESSING_METHOD>> SendList, bool IsProcessing)
        {
            InitializeComponent();
            try
            {
                this.IsProcessing = IsProcessing;
                this.SendList = SendList;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmProcessingMethod_Load(object sender, EventArgs e)
        {
            try
            {
                var number = IsProcessing ? 2 : 1;
                lstProcessingMethod = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o =>  o.PROCESSING_METHOD_TYPE == number && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                LoadGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadGrid()
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyWord.Text.ToLower().Trim());
                var query = lstProcessingMethod.AsQueryable();
                query = query.Where(o => o.PROCESSING_METHOD_CODE.ToLower().Contains(keyword)
                                    || Inventec.Common.String.Convert.UnSignVNese(o.PROCESSING_METHOD_NAME.ToLower()).Contains(keyword));
                gridControl1.DataSource = query;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowSelected = gridView1.GetSelectedRows();
                List<HIS_PROCESSING_METHOD> lstSend = new List<HIS_PROCESSING_METHOD>();
                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    foreach (var i in rowSelected)
                    {
                        var row = (HIS_PROCESSING_METHOD)gridView1.GetRow(i);
                        if (row != null)
                        {
                            lstSend.Add(row);
                        }
                    }
                }
                if (SendList != null)
                    SendList(lstSend);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
