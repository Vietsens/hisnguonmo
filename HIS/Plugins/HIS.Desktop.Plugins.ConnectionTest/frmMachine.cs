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
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmMachine : Form
    {
        List<LisMachineAdo> machineSources = new List<LisMachineAdo>();
        Action<long?> machineId;
        public frmMachine(List<LisMachineAdo> machineSources, Action<long?> machineId)
        {
            InitializeComponent();

            try
            {
                this.machineId = machineId;
                this.machineSources = machineSources;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmMachine_Load(object sender, EventArgs e)
        {

            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE_TEIN", "Đã xử lý", 100, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 100, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 450);
                ControlEditorLoader.Load(cboMachine, machineSources, controlEditorADO);
                cboMachine.Properties.ImmediatePopup = true;
                cboMachine.Properties.PopupFormSize = new System.Drawing.Size(450, cboMachine.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                if(machineId != null)
                {
                    machineId(cboMachine.EditValue != null ? (long?)cboMachine.EditValue : null);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.PerformClick();
        }
    }
}
