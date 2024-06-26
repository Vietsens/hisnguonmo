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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class frmChooseMedistock : Form
    {
        List<long> medistockIdList = null;
        HIS.Desktop.Common.DelegateSelectData delegateSelectData = null;
        public frmChooseMedistock()
        {
            InitializeComponent();
        }

        public frmChooseMedistock(List<long> _medistockIdList, HIS.Desktop.Common.DelegateSelectData _delegateSelectData)
        {
            InitializeComponent();
            this.medistockIdList = _medistockIdList;
            this.delegateSelectData = _delegateSelectData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmChooseMedistock_Load(object sender, EventArgs e)
        {
            try
            {
                var medistockList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => this.medistockIdList.Contains(o.ID)).ToList();
                gridControlMedistock.BeginUpdate();
                gridControlMedistock.DataSource = medistockList;
                gridControlMedistock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> selectMedistocks = new List<long>();
        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.delegateSelectData != null)
                {
                    var selectData = gridViewMedistock.GetSelectedRows();

                    foreach (var item in selectData)
                    {
                        var medistock = (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK)gridViewMedistock.GetRow(item);
                        if (medistock != null)
                        {
                            selectMedistocks.Add(medistock.ID);
                        }
                    }

                    this.delegateSelectData(selectMedistocks);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseMedistock_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.delegateSelectData(selectMedistocks);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
