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

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void DepositRequestClick()
        {
            try
            {
                if (btnDepositRequest.Enabled)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                    Inventec.Desktop.Common.Modules.Module moduleDeposit = new Inventec.Desktop.Common.Modules.Module();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RequestDeposit'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.GetTreatmentIdFromResultData());
                       
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.RequestDeposit", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
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

        private void DepositDetail()
        {
            try
            {
                if (this.CheckCashierRoom())
                {
                    //#15524
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowDepositService == 1)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDeposit").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDeposit'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            TransactionDepositADO ado = new TransactionDepositADO(this.GetTreatmentFeeViewByResult(), (long)(this.cboCashierRoom.EditValue ?? 0));
                            listArgs.Add(ado);
                            listArgs.Add(moduleData);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("moduleData is null");
                            }

                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                    else
                    {
                        DepositServiceADO depositServiceADO = new DepositServiceADO();
                        depositServiceADO.hisTreatmentId = this.GetTreatmentViewByResult().ID;

                        if (depositServiceADO.hisTreatmentId == 0)
                            throw new ArgumentNullException("hisTreatmentId is null");

                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DepositService").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DepositService'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.DepositService' is not plugins");

                        List<object> listArgs = new List<object>();
                        depositServiceADO.BRANCH_ID = WorkPlace.GetBranchId();
                        depositServiceADO.CashierRoomId = (long)(this.cboCashierRoom.EditValue ?? 0);
                        listArgs.Add(depositServiceADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

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

        private V_HIS_TREATMENT GetTreatmentViewByResult()
        {
            V_HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.GetTreatmentIdFromResultData();
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result ?? new V_HIS_TREATMENT();
        }

        private V_HIS_TREATMENT_FEE GetTreatmentFeeViewByResult()
        {
            V_HIS_TREATMENT_FEE result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                treatmentFilter.ID = this.GetTreatmentIdFromResultData();
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
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
