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
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void DepositDetail()
        {
            try
            {
                if (this.CheckCashierRoom())
                {
                    if (hisPatientVitaminASDOSave == null
                        || hisPatientVitaminASDOSave.Vaccinations == null
                        || hisPatientVitaminASDOSave.Vaccinations.Count == 0
                        )
                        throw new ArgumentNullException("Vaccinations is null");

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MedicineVaccinBill").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MedicineVaccinBill'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.MedicineVaccinBill' is not plugins");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(hisPatientVitaminASDOSave.Vaccinations.FirstOrDefault());
                        listArgs.Add(moduleData);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloAd);
                        listArgs.Add(Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCashierRoom.EditValue ?? "0").ToString()));

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
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

        private V_HIS_TREATMENT_FEE GetTreatmentViewByResult()
        {
            V_HIS_TREATMENT_FEE result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                treatmentFilter.PATIENT_ID = this.GetTreatmentIdFromResultData();
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result ?? new V_HIS_TREATMENT_FEE();
        }
    }
}
