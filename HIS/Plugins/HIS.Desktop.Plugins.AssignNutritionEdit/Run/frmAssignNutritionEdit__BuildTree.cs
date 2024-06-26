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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutritionEdit.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit.Run
{
    public partial class frmAssignNutritionEdit : HIS.Desktop.Utility.FormBase
    {
        private void BindTree()
        {
            try
            {
                ServiceComboNTADO serviceComboADO = null;
                if (ServiceComboNTDataWorker.DicServiceCombo == null)
                    ServiceComboNTDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboNTADO>();
                if (ServiceComboNTDataWorker.DicServiceCombo.ContainsKey(currentServiceReq.ID))
                {
                    ServiceComboNTDataWorker.DicServiceCombo.TryGetValue(this.currentServiceReq.ID, out serviceComboADO);
                }
                else
                {
                    serviceComboADO = ServiceComboNTDataWorker.GetByPatientType(currentServiceReq.TDL_PATIENT_TYPE_ID ?? 0, BranchDataWorker.DicServicePatyInBranch,currentServiceReq);

                    ServiceComboNTDataWorker.DicServiceCombo.Add(this.currentServiceReq.ID, serviceComboADO);
                }
                if (serviceComboADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceIsleafADOs:" + serviceComboADO.ServiceIsleafADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceAllADOs:" + serviceComboADO.ServiceAllADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceParentADOs:" + serviceComboADO.ServiceParentADOs.Count());             
                    var serviceIdHasFilters = serviceComboADO.ServiceRooms
                        .GroupBy(o => o.SERVICE_ID)
                        .ToDictionary(o => o.Key, o => o.ToList());

                    this.ServiceIsleafADOs = serviceComboADO.ServiceIsleafADOs.Where(o => serviceIdHasFilters.ContainsKey(o.ID)).ToList();

                    Inventec.Common.Logging.LogSystem.Debug("count of ServiceIsleafADOs sau khi loc danh sach cac phong duoc phep chi dinh theo cac dieu kien (tạm ngừng chỉ định, giới hạn thời gian hoạt động,...):" + this.ServiceIsleafADOs.Count());
                    this.ServiceAllADOs = serviceComboADO.ServiceAllADOs;
                    this.ServiceParentADOs = serviceComboADO.ServiceParentADOs;
                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        if (!DicCapacity.ContainsKey(item.ID))
                            DicCapacity.Add(item.ID, item.CAPACITY);
                        item.IsChecked = false;
                        item.AMOUNT = 0;
                        item.PATIENT_TYPE_ID = 0;
                        item.PRICE = 0;
                        item.ROOM_ID = 0;
                        item.NOTE = "";
                        item.CAPACITY = null;
                        item.SERVICE_GROUP_ID_SELECTEDs = null;
                        item.ErrorMessageAmount = "";
                        item.ErrorMessageIsAssignDay = "";
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypeAmount = ErrorType.None;
                        item.ErrorTypeIsAssignDay = ErrorType.None;
                        item.ErrorTypePatientTypeId = ErrorType.None;
                    }
                }
                else
                {
                    this.ServiceIsleafADOs = new List<SSServiceADO>();
                    this.ServiceParentADOs = new List<ServiceADO>();
                    this.ServiceAllADOs = new List<ServiceADO>();                
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
