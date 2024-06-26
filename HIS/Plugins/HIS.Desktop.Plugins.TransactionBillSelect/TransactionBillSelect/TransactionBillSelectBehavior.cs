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
using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Plugins.TransactionBillSelect;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Utility;

namespace Inventec.Desktop.Plugins.TransactionBillSelect.TransactionBillSelect
{
    public sealed class TransactionBillSelectBehavior : Tool<IDesktopToolContext>, ITransactionBillSelect
    {
        V_HIS_TREATMENT_FEE treatment = null;
        List<V_HIS_SERE_SERV_5> listSereServ = null;
        V_HIS_PATIENT_TYPE_ALTER lastPatientType = null;
        Inventec.Desktop.Common.Modules.Module Module;
        bool? IsBill = null;
        object[] entity;
        public TransactionBillSelectBehavior()
            : base()
        {

        }
        internal TransactionBillSelectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, V_HIS_TREATMENT_FEE data, List<V_HIS_SERE_SERV_5> listSereServ, V_HIS_PATIENT_TYPE_ALTER lastPatientType, bool? isBill)
            : base()
        {
            this.Module = module;
            this.treatment = data;
            this.listSereServ = listSereServ;
            this.lastPatientType = lastPatientType;
            this.IsBill = isBill;
        }
        internal TransactionBillSelectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, bool? isBill)
            : base()
        {
            this.Module = module;
            this.IsBill = isBill;
        }

        object ITransactionBillSelect.Run()
        {
            object result = null;
            try
            {
                string BillSelect = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.TransactionBillSelect");
                if (BillSelect == "2")
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillTwoInOne").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TransactionBillTwoInOne");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        if (treatment != null)
                            listArgs.Add(treatment);

                        if (listSereServ != null)
                            listArgs.Add(listSereServ);

                        if (lastPatientType != null)
                            listArgs.Add(lastPatientType);

                        if (IsBill != null)
                        {
                            listArgs.Add(IsBill);
                        }

                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, Module.RoomId, Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        return extenceInstance;
                    }
                }
                else
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TransactionBill");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        if (treatment != null)
                            listArgs.Add(treatment);

                        if (listSereServ != null)
                            listArgs.Add(listSereServ);

                        if (lastPatientType != null)
                            listArgs.Add(lastPatientType);

                        if (IsBill != null)
                            listArgs.Add(IsBill);

                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, Module.RoomId, Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        return extenceInstance;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

    }
}
