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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private bool CheckExistsParent(ServiceADO serviceADO)
        {
            bool valid = true;
            try
            {
                valid = (serviceADO != null && serviceADO.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                if ((serviceADO.PARENT_ID ?? 0) > 0)
                {
                    valid = valid && ExistsParentIsActive((serviceADO.PARENT_ID ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ExistsParentIsActive(long serviceId)
        {
            bool valid = true;
            try
            {
                if (serviceId > 0)
                {
                    var parent = lstService.FirstOrDefault(o => o.ID == serviceId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    valid = (parent != null);
                    if (valid)
                    {
                        if ((parent.PARENT_ID ?? 0) > 0 && parent.PARENT_ID != parent.ID)
                        {
                            valid = valid && ExistsParentIsActive((parent.PARENT_ID ?? 0));
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private async Task BindTree()
        {
            try
            {
				ServiceComboADO serviceComboADO = null;
				if (ServiceComboDataWorker.DicServiceCombo == null)
					ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboADO>();
				if (ServiceComboDataWorker.DicServiceCombo.ContainsKey(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
				{
					ServiceComboDataWorker.DicServiceCombo.TryGetValue(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, out serviceComboADO);
				}
				else
				{
					serviceComboADO = ServiceComboDataWorker.GetByPatientType(currentHisPatientTypeAlter.PATIENT_TYPE_ID, this.servicePatyInBranchs);

					ServiceComboDataWorker.DicServiceCombo.Add(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, serviceComboADO);
				}
				//await TaskLoadServiceComboData;
                if (serviceComboADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceIsleafADOs:" + serviceComboADO.ServiceIsleafADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceAllADOs:" + serviceComboADO.ServiceAllADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceParentADOs:" + serviceComboADO.ServiceParentADOs.Count());
                    
                    List<long> listRoomIdActives = new List<long>();
                    if (this.currentExecuteRooms != null && this.currentExecuteRooms.Count > 0)
                    {
                        listRoomIdActives = this.currentExecuteRooms.Select(o => o.ROOM_ID).ToList();
                    }

                    //#18160
                    //Kiểm tra, nếu phòng người dùng đang làm việc có check "Giới hạn dịch vụ chỉ định" (is_restrict_req_service trong his_room) thì cần bổ sung điều kiện:
                    //Dịch vụ phải được khai báo trong dữ liệu thiết lập "Phòng - dịch vụ" với loại là "phòng chỉ định" (is_request trong his_service_room)
                    bool isRequestRoomReqService = (this.requestRoom.IS_RESTRICT_REQ_SERVICE == 1);
                    var serviceIdHasFilters = serviceComboADO.ServiceRooms
                        .Where(o =>
                            ((o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL && listRoomIdActives.Contains(o.ROOM_ID))
                            || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                            && (!isRequestRoomReqService || (isRequestRoomReqService && o.IS_REQUEST == 1)))
                        .GroupBy(o => o.SERVICE_ID)
                        .ToDictionary(o => o.Key, o => o.ToList());

                    this.ServiceIsleafADOs = serviceComboADO.ServiceIsleafADOs.Where(o => serviceIdHasFilters.ContainsKey(o.ID)).ToList();

                    //thêm oxy vào danh sách dịch vụ
                    if (Config.HisConfigCFG.AllowAssignOxygen)
                    {
                        List<V_HIS_MEDICINE_TYPE> listOxyen = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_OXYGEN == 1).ToList();
                        if (listOxyen != null && listOxyen.Count > 0)
                        {
                            List<long> serviceIds = listOxyen.Select(s => s.SERVICE_ID).Distinct().ToList();
                            List<V_HIS_SERVICE> oxyService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();
                            if (oxyService != null && oxyService.Count > 0)
                            {
                                List<SereServADO> oxys = (from m in oxyService
                                                          select new SereServADO(m, this.patientTypeByPT, false, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                    ).Distinct()
                                    .OrderByDescending(o => o.SERVICE_NUM_ORDER)
                                    .ThenBy(o => o.TDL_SERVICE_NAME)
                                    .ToList();

                                HIS_SERVICE_TYPE serviceOther = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC);
                                if (serviceOther != null)
                                {
                                    oxys.ForEach(o =>
                                    {
                                        o.TDL_SERVICE_TYPE_ID = serviceOther.ID;
                                        o.SERVICE_TYPE_ID = serviceOther.ID;
                                        o.SERVICE_TYPE_CODE = serviceOther.SERVICE_TYPE_CODE;
                                        o.SERVICE_TYPE_NAME = serviceOther.SERVICE_TYPE_NAME;
                                        o.IS_MULTI_REQUEST = (short)1;
                                    });
                                }

                                this.ServiceIsleafADOs.AddRange(oxys);
                            }
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("count of ServiceIsleafADOs sau khi loc danh sach cac phong duoc phep chi dinh theo cac dieu kien (tạm ngừng chỉ định, giới hạn thời gian hoạt động,...):" + this.ServiceIsleafADOs.Count() + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRequestRoomReqService), isRequestRoomReqService));
                    this.ServiceAllADOs = serviceComboADO.ServiceAllADOs;
                    this.ServiceParentADOs = serviceComboADO.ServiceParentADOs;
                    this.ServiceParentADOForGridServices = serviceComboADO.ServiceParentADOs;

                    //var svCheck = this.ServiceIsleafADOs.Where(o => o.TDL_SERVICE_CODE == "B120019007").FirstOrDefault();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => svCheck), svCheck));

                    //if (!HIS.Desktop.Plugins.AssignService.Config.HisConfigCFG.IsAllowShowingAnapathology)
                    //{
                    //    this.ServiceAllADOs = this.ServiceAllADOs.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                    //    this.ServiceParentADOs = this.ServiceParentADOs.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                    //    this.ServiceParentADOForGridServices = this.ServiceParentADOForGridServices.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                    //    this.ServiceIsleafADOs = this.ServiceIsleafADOs.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                    //}

                    List<long> lstNotDisplayIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToList().Where(o => o.IS_NOT_DISPLAY_ASSIGN == 1).Select(o => o.ID).ToList();

                    if (lstNotDisplayIds !=null && lstNotDisplayIds.Count>0)
					{
                        this.ServiceAllADOs = this.ServiceAllADOs.Where(o => !lstNotDisplayIds.Exists(p=>p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceParentADOs = this.ServiceParentADOs.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceParentADOForGridServices = this.ServiceParentADOForGridServices.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceIsleafADOs = this.ServiceIsleafADOs.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                    }                        

                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        item.AssignNumOrder = null;
                        item.IsChecked = false;
                        item.ShareCount = null;
                        item.AMOUNT = 1;
                        item.PATIENT_TYPE_ID = 0;
                        item.PRICE = 0;
                        item.TDL_EXECUTE_ROOM_ID = 0;
                        item.IsExpend = false;
                        item.InstructionNote = "";
                        item.IsOutKtcFee = ((item.IS_OUT_PARENT_FEE ?? -1) == 1);
                        item.IsKHBHYT = false;
                        item.SERVICE_GROUP_ID_SELECTEDs = null;
                        item.AssignPackagePriceEdit = null;
                        item.AssignSurgPriceEdit = null;
                        item.InstructionNote = "";
                        item.IsNoDifference = false;
                        item.PRIMARY_PATIENT_TYPE_ID = null;
                        item.IsNotChangePrimaryPaty = false;
                        item.ErrorMessageAmount = "";
                        item.ErrorMessageIsAssignDay = "";
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypeAmount = ErrorType.None;
                        item.ErrorTypeIsAssignDay = ErrorType.None;
                        item.ErrorTypePatientTypeId = ErrorType.None;
                        item.PackagePriceId = null;
                        item.SERVICE_CONDITION_ID = null;
                        item.SERVICE_CONDITION_NAME = null;
                        item.OTHER_PAY_SOURCE_ID = null;
                        item.OTHER_PAY_SOURCE_CODE = "";
                        item.OTHER_PAY_SOURCE_NAME = "";
                        item.BedFinishTime = null;
                        item.BedId = null;
                        item.BedStartTime = null;
                        item.IsNotLoadDefaultPatientType = false;
                        item.IsNotUseBhyt = false;
                        item.OldPatientType = 0;
                        item.SereServEkipADO = null;
                        item.TEST_SAMPLE_TYPE_ID = 0;
                        item.TEST_SAMPLE_TYPE_CODE = null;
                        item.TEST_SAMPLE_TYPE_NAME = null;
                        item.SereServEkipADO = null;
                    }
                }
                else
                {
                    this.ServiceIsleafADOs = new List<SereServADO>();
                    this.ServiceParentADOs = new List<ServiceADO>();
                    this.ServiceAllADOs = new List<ServiceADO>();
                    this.ServiceParentADOForGridServices = new List<ServiceADO>();
                    Inventec.Common.Logging.LogSystem.Debug("** HIS.Desktop.Plugins.AssignService.AssignService BindTree()** serviceComboADO is null ");
                }
                var serviceGroupAdds = BackendDataWorker.Get<HIS_SERVICE_GROUP>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue && o.PARENT_SERVICE_ID != null && o.PARENT_SERVICE_ID > 0).ToList();
                if (serviceGroupAdds != null && serviceGroupAdds.Count > 0)
                {
                    if (!this.ServiceParentADOs.Any(o => (o.IsParentServiceId ?? false) == true))
                    {
                        foreach (var svgr in serviceGroupAdds)
                        {
                            var parentSV = svgr.PARENT_SERVICE_ID > 0 ? this.ServiceParentADOs.Where(o => o.ID == svgr.PARENT_SERVICE_ID).FirstOrDefault() : null;
                            if (parentSV != null)
                            {
                                if (this.ServiceParentADOs.Any(o => o.CONCRETE_ID__IN_SETY == (parentSV.SERVICE_TYPE_ID + "." + (parentSV.ID) + ".PARENT_SERVICE_ID." + svgr.ID)))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Dich vu " + parentSV.SERVICE_NAME + "(" + parentSV.SERVICE_CODE + ") da duoc cau hinh PARENT_SERVICE_ID voi 1 nhom dich vu khac roi, khong the gan them voi nhom " + svgr.SERVICE_GROUP_NAME);
                                    continue;
                                }

                                ServiceADO serviceADOParent = new ServiceADO();
                                serviceADOParent.ID = svgr.ID;
                                serviceADOParent.SERVICE_CODE = svgr.SERVICE_GROUP_CODE;
                                serviceADOParent.SERVICE_NAME = "--" + svgr.SERVICE_GROUP_NAME;
                                serviceADOParent.IsParentServiceId = true;
                                serviceADOParent.IS_LEAF = 1;
                                serviceADOParent.CONCRETE_ID__IN_SETY = (parentSV.SERVICE_TYPE_ID + "." + (parentSV.ID) + ".PARENT_SERVICE_ID." + svgr.ID);
                                serviceADOParent.PARENT_ID__IN_SETY = (parentSV.SERVICE_TYPE_ID + "." + (svgr.PARENT_SERVICE_ID));
                                this.ServiceParentADOs.Add(serviceADOParent);
                            }
                        }
                    }
                }
                records = new BindingList<ServiceADO>(this.ServiceParentADOs);
                this.treeService.DataSource = records;
                this.treeService.KeyFieldName = "CONCRETE_ID__IN_SETY";
                this.treeService.ParentFieldName = "PARENT_ID__IN_SETY";
                this.hideCheckBoxHelper__Service = new HideCheckBoxHelper(this.treeService);
                UpdateSwithExpendAll();
                if (IsFirstloadConditionService)
                {
                    lstConditionService = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ServiceIsleafADOs.Select(p=>p.SERVICE_ID).Contains(o.SERVICE_ID)).ToList();                 
                    this.IsFirstloadConditionService = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
