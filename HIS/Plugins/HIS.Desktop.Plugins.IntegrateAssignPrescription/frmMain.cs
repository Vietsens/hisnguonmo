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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Utility;
using HIS.WCF.Service.AssignPrescriptionService;
using Inventec.Common.Logging;
using Inventec.Core;
using Microsoft.Win32;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.IntegrateAssignPrescription
{
    public partial class frmMain : FormBase
    {
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupAppValue = "Inventec.IntegrateAssignPrescription.StartupPath";
        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmMain(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }

                if (IsHostOpened())
                {
                    AssignPrescriptionServiceManager.SetDelegate(DelegateAssignPrescription);
                    Inventec.Common.Logging.LogSystem.Info("AssignPrescriptionServiceManager Open Host Success!");
                    btnStopService.Enabled = true;
                    btnStartService.Enabled = false;
                }
                else
                {
                    btnStopService.Enabled = false;
                    btnStartService.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void DelegateAssignPrescription(HIS.WCF.DCO.WcfAssignPrescriptionDCO assignPrescriptionDCO)
        {
            try
            {
                //TODO
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(0, 0, 0);

                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatFilter = new MOS.Filter.HisTreatmentFilter();
                    treatFilter.TREATMENT_CODE__EXACT = assignPrescriptionDCO.TreatmentCode;

                    List<HIS_TREATMENT> lstTreatments = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, paramCommon);
                    if (lstTreatments != null)
                    {
                        var treatment = lstTreatments.First();
                        assignServiceADO.TreatmentCode = treatment.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = treatment.ID;
                        assignServiceADO.PatientDob = treatment.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = treatment.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = treatment.TDL_PATIENT_GENDER_NAME;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetRegistryValue(string rgkey, string keyOfValue)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(rgkey, true);
                return (string)key.GetValue(keyOfValue, "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return String.Empty;
        }

        private bool SetRegistryValue(string rgkey, string key, string value)
        {
            bool success = false;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(rgkey, true);
                registryKey.SetValue(key, value);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private static void SetStartup()
        {
            try
            {
                //Set the application to run at startup
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
                key.SetValue(StartupAppValue, Application.ExecutablePath.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void RemoveStartup()
        {
            try
            {
                //Set the application to run at startup
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
                key.DeleteSubKey(StartupAppValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsHostOpened()
        {
            bool success = false;
            try
            {
                if (AssignPrescriptionServiceManager.IsOpen())
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        private bool CloseHost()
        {
            bool success = false;
            try
            {
                if (AssignPrescriptionServiceManager.CloseHost())
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        private bool OpenHost()
        {
            bool success = false;
            try
            {
                if (AssignPrescriptionServiceManager.OpenHost())
                {
                    //AssignPrescriptionServiceManager.SetDelegate();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        private void bbtnStartService_Click(object sender, EventArgs e)
        {
            try
            {
                //SetRegistryValue(EMRServiceKey, AuStartupServiceValue, "1");
                if (OpenHost())
                {
                    Inventec.Common.Logging.LogSystem.Info("AssignPrescriptionServiceManager Open Host Success!");
                    btnStopService.Enabled = true;
                    btnStartService.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnStopService_Click(object sender, EventArgs e)
        {
            try
            {
                //SetRegistryValue(EMRServiceKey, AuStartupServiceValue, "0");
                if (CloseHost())
                {
                    Inventec.Common.Logging.LogSystem.Info("AssignPrescriptionServiceManager Close Host Success!");
                    btnStopService.Enabled = false;
                    btnStartService.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
