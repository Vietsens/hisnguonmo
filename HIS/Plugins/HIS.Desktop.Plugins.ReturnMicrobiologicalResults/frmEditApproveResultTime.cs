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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class frmEditApproveResultTime : Form
    {
        long lisSampleId;
        Action<bool> refesh;
        public frmEditApproveResultTime(Action<bool> refesh, long lisSampleId)
        {
            InitializeComponent();
            this.refesh = refesh;
            this.lisSampleId = lisSampleId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtApproveResultTime.EditValue != null)
                {
                    bool success = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    LisSampleApproveResultSDO lisSampleApproveResultSDO = new LisSampleApproveResultSDO();
                    lisSampleApproveResultSDO.SampleId = lisSampleId;
                    lisSampleApproveResultSDO.ApproveTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtApproveResultTime.DateTime);
                    var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/ApproveResult", ApiConsumer.ApiConsumers.LisConsumer, lisSampleApproveResultSDO, param);
                    if (rs != null)
                    {
                        success = true;
                        this.refesh(success);
                        this.Close();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void frmEditApproveResultTime_Load(object sender, EventArgs e)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            dtApproveResultTime.DateTime = DateTime.Now;
        }
    }
}
