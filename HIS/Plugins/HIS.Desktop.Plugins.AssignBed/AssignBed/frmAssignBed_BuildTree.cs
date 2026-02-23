using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignBed.ADO;
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

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {
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

                if (serviceComboADO != null)
                {
                    // ============= CHỈ LẤY LOẠI DỊCH VỤ GIƯỜNG (ID = 8) =============
                    long SERVICE_TYPE_ID_GIUONG = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;

                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceIsleafADOs:" + serviceComboADO.ServiceIsleafADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceAllADOs:" + serviceComboADO.ServiceAllADOs.Count());
                    Inventec.Common.Logging.LogSystem.Debug("count of serviceComboADO.ServiceParentADOs:" + serviceComboADO.ServiceParentADOs.Count());

                    List<long> listRoomIdActives = new List<long>();
                    if (this.currentExecuteRooms != null && this.currentExecuteRooms.Count > 0)
                    {
                        listRoomIdActives = this.currentExecuteRooms.Select(o => o.ROOM_ID).ToList();
                    }

                    bool isRequestRoomReqService = (this.requestRoom.IS_RESTRICT_REQ_SERVICE == 1);
                    var serviceIdHasFilters = serviceComboADO.ServiceRooms
                        .Where(o =>
                            ((o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL && listRoomIdActives.Contains(o.ROOM_ID))
                            || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                            && (!isRequestRoomReqService || (isRequestRoomReqService && o.IS_REQUEST == 1)))
                        .GroupBy(o => o.SERVICE_ID)
                        .ToDictionary(o => o.Key, o => o.ToList());

                    // ============= CHỈ LẤY DỊCH VỤ GIƯỜNG =============
                    this.ServiceIsleafADOs = serviceComboADO.ServiceIsleafADOs.Where(o => serviceIdHasFilters.ContainsKey(o.ID) && o.SERVICE_TYPE_ID == SERVICE_TYPE_ID_GIUONG).ToList();

                    #region ============= BỎ PHẦN THÊM OXY (COMMENT LẠI) =============
                    //thêm oxy vào danh sách dịch vụ
                    //if (Config.HisConfigCFG.AllowAssignOxygen)
                    //{
                    //    List<V_HIS_MEDICINE_TYPE> listOxyen = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_OXYGEN == 1).ToList();
                    //    if (listOxyen != null && listOxyen.Count > 0)
                    //    {
                    //        List<long> serviceIds = listOxyen.Select(s => s.SERVICE_ID).Distinct().ToList();
                    //        List<V_HIS_SERVICE> oxyService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();
                    //        if (oxyService != null && oxyService.Count > 0)
                    //        {
                    //            List<SereServADO> oxys = (from m in oxyService
                    //                                      select new SereServADO(m, this.patientTypeByPT, false, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                    //                ).Distinct()
                    //                .OrderByDescending(o => o.SERVICE_NUM_ORDER)
                    //                .ThenBy(o => o.TDL_SERVICE_NAME)
                    //                .ToList();
                    //
                    //            HIS_SERVICE_TYPE serviceOther = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC);
                    //            if (serviceOther != null)
                    //            {
                    //                oxys.ForEach(o =>
                    //                {
                    //                    o.TDL_SERVICE_TYPE_ID = serviceOther.ID;
                    //                    o.SERVICE_TYPE_ID = serviceOther.ID;
                    //                    o.SERVICE_TYPE_CODE = serviceOther.SERVICE_TYPE_CODE;
                    //                    o.SERVICE_TYPE_NAME = serviceOther.SERVICE_TYPE_NAME;
                    //                    o.IS_MULTI_REQUEST = (short)1;
                    //                });
                    //            }
                    //
                    //            this.ServiceIsleafADOs.AddRange(oxys);
                    //        }
                    //    }
                    //}
                    // ==========================================================
                    #endregion
                    Inventec.Common.Logging.LogSystem.Debug("count of ServiceIsleafADOs sau khi loc chi lay GIUONG:" + this.ServiceIsleafADOs.Count());

                    // ============= CHỈ LẤY DỊCH VỤ GIƯỜNG CHO CÁC DANH SÁCH =============
                    this.ServiceAllADOs = serviceComboADO.ServiceAllADOs.Where(o => o.SERVICE_TYPE_ID == SERVICE_TYPE_ID_GIUONG).ToList();
                    this.ServiceParentADOs = serviceComboADO.ServiceParentADOs.Where(o => o.SERVICE_TYPE_ID == SERVICE_TYPE_ID_GIUONG).ToList();
                    this.ServiceParentADOForGridServices = serviceComboADO.ServiceParentADOs.Where(o => o.SERVICE_TYPE_ID == SERVICE_TYPE_ID_GIUONG).ToList();

                    List<long> lstNotDisplayIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToList().Where(o => o.IS_NOT_DISPLAY_ASSIGN == 1).Select(o => o.ID).ToList();

                    if (lstNotDisplayIds != null && lstNotDisplayIds.Count > 0)
                    {
                        this.ServiceAllADOs = this.ServiceAllADOs.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceParentADOs = this.ServiceParentADOs.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceParentADOForGridServices = this.ServiceParentADOForGridServices.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                        this.ServiceIsleafADOs = this.ServiceIsleafADOs.Where(o => !lstNotDisplayIds.Exists(p => p == o.SERVICE_TYPE_ID)).ToList();
                    }

                    var serviceByID = default(HIS_SERVICE);
                    var serviceByIDSet = default(HIS_SERVICE);

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
                        item.NumberOfTimes = 1;
                        //item.IsGuarantee = false;
                    }

                    // ============= THÊM PHẦN NẠP DỮ LIỆU VÀO GRID TRỰC TIẾP TẠI ĐÂY =============
                    this.gridControlServiceProcess.DataSource = null;

                    var dataMap = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ?
                        this.ServiceIsleafADOs
                            .OrderBy(o => o.SERVICE_TYPE_ID)
                            .ThenByDescending(o => o.SERVICE_NUM_ORDER)
                            .ThenBy(o => o.TDL_SERVICE_NAME)
                            .ToList()
                        : null;
                    if (dataMap != null && dataMap.Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<SereServADO, DataGridAdo>();
                        this.DataGridAdo = AutoMapper.Mapper.Map<List<DataGridAdo>>(this.ServiceIsleafADOs);

                        this.gridControlServiceProcess.DataSource = this.DataGridAdo;
                    }

                    if (this.gridViewServiceProcess != null)
                    {
                        this.gridViewServiceProcess.ClearGrouping();
                        // Có thể bỏ group vì chỉ có 1 loại dịch vụ
                        // this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = 0;
                        // this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Da nap du lieu vao gridControlServiceProcess: " + (this.ServiceIsleafADOs != null ? this.ServiceIsleafADOs.Count : 0) + " dich vu");

                }
                else
                {
                    this.ServiceIsleafADOs = new List<SereServADO>();
                    this.ServiceParentADOs = new List<ServiceADO>();
                    this.ServiceAllADOs = new List<ServiceADO>();
                    this.ServiceParentADOForGridServices = new List<ServiceADO>();
                    Inventec.Common.Logging.LogSystem.Debug("** HIS.Desktop.Plugins.AssignService.AssignService BindTree()** serviceComboADO is null ");
                }

                //var serviceGroupAdds = BackendDataWorker.Get<HIS_SERVICE_GROUP>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue && o.PARENT_SERVICE_ID != null && o.PARENT_SERVICE_ID > 0).ToList();
                //if (serviceGroupAdds != null && serviceGroupAdds.Count > 0)
                //{
                //    if (!this.ServiceParentADOs.Any(o => (o.IsParentServiceId ?? false) == true))
                //    {
                //        foreach (var svgr in serviceGroupAdds)
                //        {
                //            var parentSV = svgr.PARENT_SERVICE_ID > 0 ? this.ServiceParentADOs.Where(o => o.ID == svgr.PARENT_SERVICE_ID).FirstOrDefault() : null;
                //            if (parentSV != null)
                //            {
                //                if (this.ServiceParentADOs.Any(o => o.CONCRETE_ID__IN_SETY == (parentSV.SERVICE_TYPE_ID + "." + (parentSV.ID) + ".PARENT_SERVICE_ID." + svgr.ID)))
                //                {
                //                    Inventec.Common.Logging.LogSystem.Debug("Dich vu " + parentSV.SERVICE_NAME + "(" + parentSV.SERVICE_CODE + ") da duoc cau hinh PARENT_SERVICE_ID voi 1 nhom dich vu khac roi, khong the gan them voi nhom " + svgr.SERVICE_GROUP_NAME);
                //                    continue;
                //                }

                //                ServiceADO serviceADOParent = new ServiceADO();
                //                serviceADOParent.ID = svgr.ID;
                //                serviceADOParent.SERVICE_CODE = svgr.SERVICE_GROUP_CODE;
                //                serviceADOParent.SERVICE_NAME = "--" + svgr.SERVICE_GROUP_NAME;
                //                serviceADOParent.IsParentServiceId = true;
                //                serviceADOParent.IS_LEAF = 1;
                //                serviceADOParent.CONCRETE_ID__IN_SETY = (parentSV.SERVICE_TYPE_ID + "." + (parentSV.ID) + ".PARENT_SERVICE_ID." + svgr.ID);
                //                serviceADOParent.PARENT_ID__IN_SETY = (parentSV.SERVICE_TYPE_ID + "." + (svgr.PARENT_SERVICE_ID));
                //                this.ServiceParentADOs.Add(serviceADOParent);
                //            }
                //        }
                //    }
                //}

                //records = new BindingList<ServiceADO>(this.ServiceParentADOs);
                //this.treeService.DataSource = records;
                //this.treeService.KeyFieldName = "CONCRETE_ID__IN_SETY";
                //this.treeService.ParentFieldName = "PARENT_ID__IN_SETY";
                //this.hideCheckBoxHelper__Service = new HideCheckBoxHelper(this.treeService);
                //UpdateSwithExpendAll();

                if (IsFirstloadConditionService)
                {
                    lstConditionService = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ServiceIsleafADOs.Select(p => p.SERVICE_ID).Contains(o.SERVICE_ID)).ToList();
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
