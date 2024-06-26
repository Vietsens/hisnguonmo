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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Modules;


namespace HIS.Desktop.Plugins.HisATCSetUp.ATCSetUp
{
    public partial class frmATCSetUp : HIS.Desktop.Utility.FormBase
    {
        #region ---Declate---
        DelegateReturnMutilObject resultAct;
        List<HIS_ATC> listAtcChecks;
        List<ATCSetUpADO> lsAtcADO;
        List<ATCSetUpADO> listAtcChecked;
        List<HIS_ATC> lstAtcChecked = new List<HIS_ATC>();
        Module moduleCurrent;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        #endregion
        #region ---Contructor---
        public frmATCSetUp()
        {
            InitializeComponent();
        }
        public frmATCSetUp(DelegateReturnMutilObject _resultAtc, List<HIS_ATC> _listAtc, Module module)
        {
            InitializeComponent();
            this.resultAct = _resultAtc;
            this.listAtcChecks = _listAtc;
            this.moduleCurrent = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.Text = (this.moduleCurrent != null ? this.moduleCurrent.text : "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Private Method---
        private void frmATCSetUp_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataTogrilControl();
                LoadDataChecked();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataChecked()
        {
            try
            {
                if (this.listAtcChecks != null && this.listAtcChecks.Count > 0)
                {
                    foreach (var item in this.listAtcChecks)
                    {
                        this.lstAtcChecked.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataTogrilControl()
        {

            try
            {
                int numPageSize;
                if (ucpagin.pagingGrid != null)
                {
                    numPageSize = ucpagin.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadData(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucpagin.Init(LoadData, param, numPageSize);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadData(object data)
        {
            try
            {
                WaitingManager.Show();
                listAtcChecked = new List<ATCSetUpADO>();
                lsAtcADO = new List<ATCSetUpADO>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisAtcFilter hisAtcFilter = new HisAtcFilter();
                hisAtcFilter.KEY_WORD = txtSearch.Text;
                hisAtcFilter.ORDER_FIELD = "MODIFY_TIME";
                hisAtcFilter.ORDER_DIRECTION = "DESC";

                var atc = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<ATCSetUpADO>>(
                    "/api/HisAtc/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    hisAtcFilter,
                    param);

                if (this.listAtcChecks != null && atc.Data.Count > 0)
                {
                    foreach (var item in atc.Data)
                    {
                        var CheckATC = (this.listAtcChecks != null && listAtcChecks.Count > 0) ? listAtcChecks.FirstOrDefault(o => o.ID == item.ID) : null;
                        if (CheckATC != null)
                        {
                            item.check = true;
                        }
                        else
                            item.check = false;
                        this.listAtcChecked.Add(item);
                    }
                }
                else if (atc.Data.Count > 0)
                {
                    foreach (var item in atc.Data)
                    {
                        item.check = false;
                        this.listAtcChecked.Add(item);
                    }
                }

                gridViewATC.BeginUpdate();
                gridControlATC.DataSource = this.listAtcChecked;
                gridViewATC.EndUpdate();
                rowCount1 = (data == null ? 0 : listAtcChecked.Count);
                dataTotal1 = (atc.Param == null ? 0 : atc.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region ---Even---
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataTogrilControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkCheck_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var datarow = (ATCSetUpADO)gridViewATC.GetFocusedRow();
                if (datarow != null)
                {
                    if (this.lstAtcChecked != null && this.lstAtcChecked.Count > 0)
                    {
                        if (datarow.check == false)
                        {
                            bool check = this.lstAtcChecked.Any(o => o.ID == datarow.ID);
                            if (!check)
                            {
                                HIS_ATC data = new HIS_ATC();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ATC>(data, datarow);
                                this.lstAtcChecked.Add(data);
                            }
                        }
                        else
                        {
                            var remove = this.lstAtcChecked.FirstOrDefault(o => o.ID == datarow.ID);
                            if (remove != null)
                            {
                                this.lstAtcChecked.RemoveAll(o => o.ID == datarow.ID);
                            }
                        }
                    }
                    else
                    {
                        this.lstAtcChecked = new List<HIS_ATC>();
                        if (datarow.check == false)
                        {
                            HIS_ATC data = new HIS_ATC();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ATC>(data, datarow);
                            this.lstAtcChecked.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region ---Even Click---

        private void btnChoise_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lstAtcChecked != null && this.resultAct != null)
                {
                    this.resultAct(new List<HIS_ATC>[] { this.lstAtcChecked });
                }
                this.Close();
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
                if (btnSearch.Enabled)
                {
                    simpleButton1_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnChoise.Enabled)
            {
                btnChoise_Click(null, null);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataTogrilControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControl1_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion
    }
}
