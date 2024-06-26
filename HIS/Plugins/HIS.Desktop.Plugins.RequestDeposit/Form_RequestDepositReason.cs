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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class Form_RequestDepositReason : Form
    {
        private List<HIS_DEPOSIT_REASON> depositReasons;
        private Action<HIS_DEPOSIT_REASON> getReasonSelected;

        public Form_RequestDepositReason()
        {
            InitializeComponent();
        }

        public Form_RequestDepositReason(List<HIS_DEPOSIT_REASON> depositReasons, Action<HIS_DEPOSIT_REASON> getReasonSelected)
        {
            InitializeComponent();
            this.depositReasons = depositReasons;
            this.getReasonSelected = getReasonSelected;
            SetIconFrm();
        }

        private void Form_RequestDepositReason_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = depositReasons;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    CommonParam param = new CommonParam();
                    HisDepositReasonFilter filter = new HisDepositReasonFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_COMMON = 1;
                    filter.KEY_WORD = txtFind.Text.Trim();
                    depositReasons = new BackendAdapter(param).Get<List<HIS_DEPOSIT_REASON>>("api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, param);
                    gridControl1.DataSource = depositReasons;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                getReasonSelected((HIS_DEPOSIT_REASON)gridView1.GetFocusedRow());
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
