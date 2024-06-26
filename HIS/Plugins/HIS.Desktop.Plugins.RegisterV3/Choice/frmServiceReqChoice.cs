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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterV3.ADO;

namespace HIS.Desktop.Plugins.RegisterV3
{
    public partial class frmServiceReqChoice : HIS.Desktop.Utility.FormBase
    {
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ> listService = null;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> listSereServ = null;
        List<ServiceReqADO> listData = null;

        public frmServiceReqChoice(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServ, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ> listServiceReqs)
        {
            InitializeComponent();
            try
            {
                if (this.listService == null)
                    listService = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>();
                this.listService = listServiceReqs;
                if (listSereServ == null)
                    this.listSereServ = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                this.listSereServ = sereServ;
                if (listData == null)
                    this.listData = new List<ServiceReqADO>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PopupPatientInformation_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIconFrm();
                this.SetData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetData()
        {
            try
            {
                foreach (var item in this.listService)
                {
                    var sereServTemp = this.listSereServ.SingleOrDefault(x => x.SERVICE_REQ_ID == item.ID);
                    ServiceReqADO ado = new ServiceReqADO();
                    ado.NUMBER_ORDER = item.NUM_ORDER;
                    ado.SERVICE_NAME = sereServTemp.TDL_SERVICE_NAME;
                    ado.EXCUTE_ROOM_NAME = item.EXECUTE_ROOM_NAME;
                    ado.INTRUCTION_DATE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.INTRUCTION_TIME);
                    ado.INTRUCTION_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME);
                    this.listData.Add(ado);
                }
                this.gridControl1.DataSource = this.listData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbntClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.simpleButton1_Click(null, null);
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
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
