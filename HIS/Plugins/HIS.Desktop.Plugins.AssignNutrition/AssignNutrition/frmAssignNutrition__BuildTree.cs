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
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
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

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
    public partial class frmAssignNutrition : HIS.Desktop.Utility.FormBase
    {
        private void BindTree()
        {
            try
            {
                ServiceComboNTADO serviceComboADO = null;
                if (ServiceComboNTDataWorker.DicServiceCombo == null)
                    ServiceComboNTDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboNTADO>();
                if (ServiceComboNTDataWorker.DicServiceCombo.ContainsKey(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
                {
                    ServiceComboNTDataWorker.DicServiceCombo.TryGetValue(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, out serviceComboADO);
                }
                else
                {
                    serviceComboADO = ServiceComboNTDataWorker.GetByPatientType(currentHisPatientTypeAlter.PATIENT_TYPE_ID, BranchDataWorker.DicServicePatyInBranch);

                    ServiceComboNTDataWorker.DicServiceCombo.Add(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, serviceComboADO);
                }
                if (serviceComboADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceIsleafADOs:" + serviceComboADO.ServiceIsleafADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceAllADOs:" + serviceComboADO.ServiceAllADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceParentADOs:" + serviceComboADO.ServiceParentADOs.Count());

                    //List<long> listRoomIdActives = new List<long>();
                    //if (this.currentExecuteRooms != null && this.currentExecuteRooms.Count > 0)
                    //{
                    //    listRoomIdActives = this.currentExecuteRooms.Select(o => o.ROOM_ID).ToList();
                    //}

                    //#18160
                    //Kiểm tra, nếu phòng người dùng đang làm việc có check "Giới hạn dịch vụ chỉ định" (is_restrict_req_service trong his_room) thì cần bổ sung điều kiện:
                    //Dịch vụ phải được khai báo trong dữ liệu thiết lập "Phòng - dịch vụ" với loại là "phòng chỉ định" (is_request trong his_service_room)
                    //bool isRequestRoomReqService = (this.requestRoom.IS_RESTRICT_REQ_SERVICE == 1);
                    var serviceIdHasFilters = serviceComboADO.ServiceRooms
                        .GroupBy(o => o.SERVICE_ID)
                        .ToDictionary(o => o.Key, o => o.ToList());

                    this.ServiceIsleafADOs = serviceComboADO.ServiceIsleafADOs.Where(o => serviceIdHasFilters.ContainsKey(o.ID)).ToList();

                    Inventec.Common.Logging.LogSystem.Debug("count of ServiceIsleafADOs sau khi loc danh sach cac phong duoc phep chi dinh theo cac dieu kien (tạm ngừng chỉ định, giới hạn thời gian hoạt động,...):" + this.ServiceIsleafADOs.Count());
                    this.ServiceAllADOs = serviceComboADO.ServiceAllADOs;
                    this.ServiceParentADOs = serviceComboADO.ServiceParentADOs;
                    //this.ServiceParentADOForGridServices = serviceComboADO.ServiceParentADOs;

                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        if (!DicCapacity.ContainsKey(item.ID))
                            DicCapacity.Add(item.ID, item.CAPACITY);
                        item.IsChecked = false;
                        item.AMOUNT = 1;
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
                    //this.ServiceParentADOForGridServices = new List<ServiceADO>();                    
                }
                //SereServADO tt = ServiceIsleafADOs.FirstOrDefault(o => o.TDL_SERVICE_CODE == "TT");
                records = new BindingList<ServiceADO>(this.ServiceParentADOs);
                this.treeService.DataSource = records;
                this.treeService.KeyFieldName = "CONCRETE_ID__IN_SETY";
                this.treeService.ParentFieldName = "PARENT_ID__IN_SETY";
                this.treeService.CollapseAll();
                this.hideCheckBoxHelper__Service = new HideCheckBoxHelper(this.treeService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
