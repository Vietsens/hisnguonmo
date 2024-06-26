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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmServiceDebateConfirmNew : Form
    {
         Inventec.Desktop.Common.Modules.Module currentModule;
        List<MOS.EFMODEL.DataModels.HIS_DEBATE> lstHisDebate { get; set; }
        List<HIS_SERVICE> lstHisSereServ { get; set; }
        string message = "";
        long treatmentID = 0;
        public frmServiceDebateConfirmNew(Inventec.Desktop.Common.Modules.Module _Module, List<HIS_SERVICE> _lstHisSereServ,
            List<MOS.EFMODEL.DataModels.HIS_DEBATE> _lstHisDebate, string _message, long _treatmentID)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
                this.lstHisSereServ = _lstHisSereServ;
                this.lstHisDebate = _lstHisDebate;
                this.message = _message;
                this.treatmentID = _treatmentID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceDebateConfirmNew_Load(object sender, EventArgs e)
        {
            try
            {
                 memoEdit1.Text = this.message;
                 simpleButton1.Focus();              
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in lstHisSereServ)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(item);
                        listArgs.Add(lstHisDebate);
                        listArgs.Add(treatmentID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
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
