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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using System.Reflection;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
		private List<string> lstModuleLinkApply;

		private void SaveAndAssain()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignService'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AssignService' is not plugins");

                List<object> listArgs = new List<object>();
                AssignServiceADO assignServiceADO = new AssignServiceADO(GetTreatmentIdFromResultData(), 0, 0, null);
                if (this._isPatientAppointmentCode == true && !String.IsNullOrEmpty(this.appointmentCode) && this._TreatmnetIdByAppointmentCode > 0)
                {
                    assignServiceADO.PreviusTreatmentId = this._TreatmnetIdByAppointmentCode;
                }
                assignServiceADO.IsAutoEnableEmergency = true;
                this.GetPatientInfoFromResultData(ref assignServiceADO);
                listArgs.Add(assignServiceADO);

                if(!IsApplyFormClosingOption(moduleData.ModuleLink))
                {
                    Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignService_Click.4");
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);

                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                    {
                        if (GlobalVariables.FormAssignService != null)
                        {
                            GlobalVariables.FormAssignService.WindowState = FormWindowState.Maximized;
                            GlobalVariables.FormAssignService.ShowInTaskbar = true;
                            Type classType = GlobalVariables.FormAssignService.GetType();
                            MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                            methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { currentModule, assignServiceADO });
                            GlobalVariables.FormAssignService.Activate();
                        }
                        else
                        {
                            GlobalVariables.FormAssignService = (Form)PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            GlobalVariables.FormAssignService.ShowInTaskbar = true;
                            if (GlobalVariables.FormAssignService == null) throw new ArgumentNullException("moduleData is null");
                            GlobalVariables.FormAssignService.Show();


                            Type classType = GlobalVariables.FormAssignService.GetType();
                            MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                            methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { true });
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
