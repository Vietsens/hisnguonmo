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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.RationSumPrint
{
    public partial class frmRationSumPrint : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM> rationSumList;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_SERE_SERV_15> sereServListAll;
        List<V_HIS_SERVICE> serviceListAll;
        List<HIS_SERVICE_REQ> serviceReqAllList;
        List<V_HIS_TREATMENT> ListTreatment;
        List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRoom;

        const long RationGroupNotExistId = -1;

        const int MAX_REQ = 100;
        #endregion

        #region Construct
        public frmRationSumPrint(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                SetPrintTypeToMps();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmRationSumPrint(List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM> data, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.moduleData = moduleData;
                this.rationSumList = data;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RationSumPrint.Resources.Lang", typeof(HIS.Desktop.Plugins.RationSumPrint.frmRationSumPrint).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.btnInPhieuTongHop.Text = Inventec.Common.Resource.Get.Value("frmRationSumPrint.btnInPhieuTongHop.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnInPhieuChiTiet.Text = Inventec.Common.Resource.Get.Value("frmRationSumPrint.btnInPhieuChiTiet.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnInTem.Text = Inventec.Common.Resource.Get.Value("frmRationSumPrint.btnInTem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                #region<GvDepartment>
                this.gridDeparment_DeparmentCode.Caption = Inventec.Common.Resource.Get.Value("frmRationSumPrint.gridDeparment_DeparmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridDeparment_DeparmentName.Caption = Inventec.Common.Resource.Get.Value("frmRationSumPrint.gridDeparment_DeparmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                #endregion

                #region<GvRationGroup>
                this.gridRationGroup_RationGroupCode.Caption = Inventec.Common.Resource.Get.Value("frmRationSumPrint.gridRationGroup_RationGroupCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridRationGroup_RationGroupName.Caption = Inventec.Common.Resource.Get.Value("frmRationSumPrint.gridRationGroup_RationGroupName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                #endregion

                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmRationSumPrint.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRationSumPrint_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();

                //Load du lieu
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //layout
                this.btnInPhieuTongHop.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__CBO_PRINT",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.gridRationGroup_RationGroupCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.gridRationGroup_RationGroupName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.gridDeparment_DeparmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.gridDeparment_DeparmentName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridDepartment();
                FillDataToGridRationGroup();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToGridDepartment()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.rationSumList == null || this.rationSumList.Count == 0)
                {
                    gridControlDepartment.DataSource = null;
                    return;
                }
                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => this.rationSumList.Select(p => p.DEPARTMENT_ID).Contains(o.ID)).ToList();
                gridControlDepartment.BeginUpdate();
                gridControlDepartment.DataSource = null;
                gridControlDepartment.DataSource = departmentList;
                gridViewDepartment.SelectAll();
                gridControlDepartment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRationGroup()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.rationSumList == null || this.rationSumList.Count == 0)
                {
                    gridControlRationGroup.DataSource = null;
                    return;
                }

                // get serviceReq
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.RATION_SUM_IDs = this.rationSumList.Select(o => o.ID).Distinct().ToList();
                serviceReqAllList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/GetView1", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, null);
                if (serviceReqAllList == null || serviceReqAllList.Count == 0)
                    return;

                List<long> treatmentIds = serviceReqAllList.Select(s => s.TREATMENT_ID).Distinct().ToList();
                ProcessGetTreatmentInfo(treatmentIds);

                // get sereServ
                sereServListAll = new List<V_HIS_SERE_SERV_15>();
                int skip = 0;
                while (serviceReqAllList.Count - skip > 0)
                {
                    var listIds = serviceReqAllList.Skip(skip).Take(MAX_REQ).ToList();
                    skip += MAX_REQ;

                    MOS.Filter.HisSereServView15Filter sereServFilter = new MOS.Filter.HisSereServView15Filter();
                    sereServFilter.SERVICE_REQ_IDs = listIds.Select(o => o.ID).Distinct().ToList();
                    var sereServ15 = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_15>>("api/HisSereServ/GetView15", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);
                    if (sereServ15 != null && sereServ15.Count > 0)
                    {
                        sereServListAll.AddRange(sereServ15);
                    }
                }

                if (sereServListAll == null || sereServListAll.Count == 0)
                    return;

                serviceListAll = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => sereServListAll.Exists(e => e.SERVICE_ID == o.ID)).ToList();
                // get rationGroup
                if (serviceListAll == null || serviceListAll.Count == 0)
                    return;
                var rationGroupIds = serviceListAll.Where(p => p.RATION_GROUP_ID.HasValue).Select(o => o.RATION_GROUP_ID.Value).ToList();
                if (rationGroupIds != null && rationGroupIds.Count > 0)
                {
                    MOS.Filter.HisRationGroupFilter rationGroupFilter = new MOS.Filter.HisRationGroupFilter();
                    rationGroupFilter.IDs = rationGroupIds;
                    var rationGroupList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_RATION_GROUP>>("api/HisRationGroup/Get", ApiConsumer.ApiConsumers.MosConsumer, rationGroupFilter, null);
                    // add thêm dòng không xác định
                    HIS_RATION_GROUP rationGroupNotExist = new HIS_RATION_GROUP();
                    rationGroupNotExist.RATION_GROUP_NAME = "Không xác định";
                    rationGroupNotExist.ID = RationGroupNotExistId;
                    rationGroupList.Add(rationGroupNotExist);

                    gridControlRationGroup.BeginUpdate();
                    gridControlRationGroup.DataSource = null;
                    gridControlRationGroup.DataSource = rationGroupList;
                    gridViewRationGroup.SelectAll();
                    gridControlRationGroup.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetTreatmentInfo(List<long> treatmentIds)
        {
            Thread treatment = new Thread(GetTreatmentByIds);
            Thread treatmentBedRoom = new Thread(GetTreatmentBedRomByTreatmentIds);
            try
            {
                treatment.Start(treatmentIds);
                treatmentBedRoom.Start(treatmentIds);
            }
            catch (Exception ex)
            {
                treatment.Abort();
                treatmentBedRoom.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentBedRomByTreatmentIds(object obj)
        {
            try
            {
                if (obj != null)
                {
                    ListTreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
                    List<long> treatmentIds = obj as List<long>;
                    int skip = 0;
                    while (treatmentIds.Count - skip > 0)
                    {
                        var listIds = treatmentIds.Skip(skip).Take(MAX_REQ).ToList();
                        skip += MAX_REQ;

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                        bedFilter.TREATMENT_IDs = listIds;
                        bedFilter.IS_IN_ROOM = true;
                        var treaBedRoom = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("/api/HisTreatmentBedRoom/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, bedFilter, param);
                        if (treaBedRoom != null && treaBedRoom.Count > 0)
                        {
                            ListTreatmentBedRoom.AddRange(treaBedRoom);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentByIds(object obj)
        {
            try
            {
                if (obj != null)
                {
                    ListTreatment = new List<V_HIS_TREATMENT>();
                    List<long> treatmentIds = obj as List<long>;
                    int skip = 0;
                    while (treatmentIds.Count - skip > 0)
                    {
                        var listIds = treatmentIds.Skip(skip).Take(MAX_REQ).ToList();
                        skip += MAX_REQ;

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentViewFilter treaFilter = new MOS.Filter.HisTreatmentViewFilter();
                        treaFilter.IDs = listIds;
                        var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, treaFilter, param);
                        if (treatments != null && treatments.Count > 0)
                        {
                            ListTreatment.AddRange(treatments);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridDepartment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRationGroup_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_RATION_GROUP data = (MOS.EFMODEL.DataModels.HIS_RATION_GROUP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region in
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCode.PRINT_TYPE_CODE__MPS000273:
                        InPhieuTongHop(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__MPS000274:
                        InPhieuChiTiet(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__MPS000371:
                        InTem(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__MPS000497:
                        InGroup(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InGroup(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                var listRatioSumID = rationSumList.Select(s => s.ID).ToList();
                HisSereServRationViewFilter filter = new HisSereServRationViewFilter();
                filter.RATION_SUM_IDs = listRatioSumID;
                var res = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("/api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filter, param);
                List<HIS_DEPARTMENT> departmentSelectList = new List<HIS_DEPARTMENT>();
                List<HIS_RATION_GROUP> rationGroupSelectList = new List<HIS_RATION_GROUP>();
                var selectdepartment = gridViewDepartment.GetSelectedRows();
                if (selectdepartment != null && selectdepartment.Count() > 0)
                {
                    foreach (var item in selectdepartment)
                    {
                        var department = (HIS_DEPARTMENT)gridViewDepartment.GetRow(item);
                        departmentSelectList.Add(department);
                    }
                }

                var selectRationGroup = gridViewRationGroup.GetSelectedRows();
                if (selectRationGroup != null && selectRationGroup.Count() > 0)
                {
                    foreach (var item in selectRationGroup)
                    {
                        var rationGroup = (HIS_RATION_GROUP)gridViewRationGroup.GetRow(item);
                        rationGroupSelectList.Add(rationGroup);
                    }
                }
                if (res != null)
                {
                    var groupRatio = res.GroupBy(s => s.RATION_TIME_ID);
                    foreach (var group in groupRatio)
                    {
                        List<V_HIS_SERE_SERV_RATION> rationlist = new List<V_HIS_SERE_SERV_RATION>();
                        foreach (var ration in group)
                        {
                            rationlist.Add(ration);
                        }
                        List<HIS_RATION_SUM> rationSum = new List<HIS_RATION_SUM>();
                        foreach (var item in rationSumList)
                        {
                            HIS_RATION_SUM temp = new HIS_RATION_SUM();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_RATION_SUM>(temp, item);
                            rationSum.Add(temp);

                        }
                        MPS.Processor.Mps000497.PDO.Mps000497PDO pdo = new MPS.Processor.Mps000497.PDO.Mps000497PDO(rationSum, rationlist, rationGroupSelectList, departmentSelectList);
                        //Inventec.Common.Logging.LogSystem.Debug("_____printDataPDO:"+Inventec.Common.Logging.LogUtil.TraceData("pdo:",pdo));

                        MPS.ProcessorBase.Core.PrintData printData = null;

                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
                        WaitingManager.Hide();
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {

                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InPhieuChiTiet(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                List<HIS_DEPARTMENT> departmentSelectList = new List<HIS_DEPARTMENT>();
                List<HIS_RATION_GROUP> rationGroupSelectList = new List<HIS_RATION_GROUP>();
                var selectdepartment = gridViewDepartment.GetSelectedRows();
                if (selectdepartment != null && selectdepartment.Count() > 0)
                {
                    foreach (var item in selectdepartment)
                    {
                        var department = (HIS_DEPARTMENT)gridViewDepartment.GetRow(item);
                        departmentSelectList.Add(department);
                    }
                }

                var selectRationGroup = gridViewRationGroup.GetSelectedRows();
                if (selectRationGroup != null && selectRationGroup.Count() > 0)
                {
                    foreach (var item in selectRationGroup)
                    {
                        var rationGroup = (HIS_RATION_GROUP)gridViewRationGroup.GetRow(item);
                        rationGroupSelectList.Add(rationGroup);
                    }
                }

                if (departmentSelectList == null || departmentSelectList.Count == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Chưa chọn khoa");
                    return;
                }

                // get ration_sum
                MOS.Filter.HisRationSumViewFilter rationSumFilter = new MOS.Filter.HisRationSumViewFilter();
                if (departmentSelectList != null && departmentSelectList.Count > 0)
                {
                    rationSumFilter.DEPARTMENT_IDs = departmentSelectList.Select(o => o.ID).Distinct().ToList();
                }
                var rationSumList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_RATION_SUM>>("api/HisRationSum/GetView", ApiConsumer.ApiConsumers.MosConsumer, rationSumFilter, null);

                foreach (var rationSum in rationSumList)
                {
                    MPS.Processor.Mps000274.PDO.Mps000274ADO ado = new MPS.Processor.Mps000274.PDO.Mps000274ADO();

                    List<V_HIS_SERE_SERV_15> sereServList = this.sereServListAll.Where(o => o.RATION_SUM_ID == rationSum.ID).ToList();
                    if (rationGroupSelectList != null && rationGroupSelectList.Count > 0)
                    {
                        List<V_HIS_SERVICE> serviceList = new List<V_HIS_SERVICE>();

                        var serviceExist = this.serviceListAll.Where(o => o.RATION_GROUP_ID.HasValue && rationGroupSelectList.Select(p => p.ID).Contains(o.RATION_GROUP_ID.Value)).ToList();
                        if (serviceExist != null && serviceExist.Count > 0)
                            serviceList.AddRange(serviceExist);

                        var rationGroupNotExist = rationGroupSelectList.FirstOrDefault(o => o.ID == RationGroupNotExistId);
                        if (rationGroupNotExist != null)
                        {
                            var serviceNotExist = this.serviceListAll.Where(o => !o.RATION_GROUP_ID.HasValue).ToList();
                            if (serviceNotExist != null && serviceNotExist.Count > 0)
                                serviceList.AddRange(serviceNotExist);
                        }

                        if (serviceList != null && serviceList.Count > 0)
                        {
                            sereServList = sereServList.Where(o => serviceList.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList();
                        }
                    }

                    if (sereServList == null || sereServList.Count == 0)
                        continue;

                    List<V_HIS_TREATMENT> listTreatment = new List<V_HIS_TREATMENT>();
                    List<V_HIS_TREATMENT_BED_ROOM> listTreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
                    List<HIS_RATION_TIME> rationTime = BackendDataWorker.Get<HIS_RATION_TIME>();
                    List<HIS_PATIENT_TYPE> patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>();

                    if (this.ListTreatment != null && this.ListTreatment.Count > 0)
                    {
                        listTreatment = this.ListTreatment.Where(o => sereServList.Exists(e => e.TDL_TREATMENT_ID == o.ID)).ToList();
                    }

                    if (this.ListTreatmentBedRoom != null && this.ListTreatmentBedRoom.Count > 0)
                    {
                        listTreatmentBedRoom = this.ListTreatmentBedRoom.Where(o => sereServList.Exists(e => e.TDL_TREATMENT_ID == o.TREATMENT_ID)).ToList();
                    }

                    MPS.Processor.Mps000274.PDO.Mps000274PDO pdo = new MPS.Processor.Mps000274.PDO.Mps000274PDO(
                        rationSum,
                        sereServList,
                        ado,
                        listTreatment,
                        listTreatmentBedRoom,
                        rationTime,
                        patientType);

                    MPS.ProcessorBase.Core.PrintData printData = null;

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                List<HIS_DEPARTMENT> departmentSelectList = new List<HIS_DEPARTMENT>();
                List<HIS_RATION_GROUP> rationGroupSelectList = new List<HIS_RATION_GROUP>();
                var selectdepartment = gridViewDepartment.GetSelectedRows();
                if (selectdepartment != null && selectdepartment.Count() > 0)
                {
                    foreach (var item in selectdepartment)
                    {
                        var department = (HIS_DEPARTMENT)gridViewDepartment.GetRow(item);
                        departmentSelectList.Add(department);
                    }
                }

                var selectRationGroup = gridViewRationGroup.GetSelectedRows();
                if (selectRationGroup != null && selectRationGroup.Count() > 0)
                {
                    foreach (var item in selectRationGroup)
                    {
                        var rationGroup = (HIS_RATION_GROUP)gridViewRationGroup.GetRow(item);
                        rationGroupSelectList.Add(rationGroup);
                    }
                }

                if (departmentSelectList == null || departmentSelectList.Count == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Chưa chọn khoa");
                    return;
                }

                // get ration_sum
                MOS.Filter.HisRationSumViewFilter rationSumFilter = new MOS.Filter.HisRationSumViewFilter();
                if (departmentSelectList != null && departmentSelectList.Count > 0)
                {
                    rationSumFilter.DEPARTMENT_IDs = departmentSelectList.Select(o => o.ID).Distinct().ToList();
                }
                var rationSumList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_RATION_SUM>>("api/HisRationSum/GetView", ApiConsumer.ApiConsumers.MosConsumer, rationSumFilter, null);

                List<V_HIS_SERE_SERV_15> SereServPrintList = new List<V_HIS_SERE_SERV_15>();
                List<V_HIS_SERVICE_REQ> ServiceReqPrintList = new List<V_HIS_SERVICE_REQ>();
                foreach (var rationSum in rationSumList)
                {
                    List<V_HIS_SERE_SERV_15> sereServList = this.sereServListAll.Where(o => o.RATION_SUM_ID == rationSum.ID).ToList();
                    if (rationGroupSelectList != null && rationGroupSelectList.Count > 0)
                    {
                        var serviceList = this.serviceListAll.Where(o => rationGroupSelectList.Select(p => p.ID).Contains(o.RATION_GROUP_ID ?? -1)).ToList();
                        if (serviceList != null && serviceList.Count > 0)
                        {
                            sereServList = sereServList.Where(o => serviceList.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList();
                        }
                    }

                    if (sereServList == null || sereServList.Count == 0)
                        continue;

                    var serviceReqList = serviceReqAllList.Where(o => o.RATION_SUM_ID == rationSum.ID).ToList();
                    if (serviceReqList != null)
                    {
                        foreach (var serviceReq in serviceReqList)
                        {
                            V_HIS_SERVICE_REQ serviceReqInput = new V_HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(serviceReqInput, serviceReq);
                            ServiceReqPrintList.Add(serviceReqInput);
                            List<V_HIS_SERE_SERV_15> sereServ1List = new List<V_HIS_SERE_SERV_15>();

                            var sereSerMap = sereServList.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();
                            if (sereSerMap == null || sereSerMap.Count == 0)
                                continue;

                            AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_15, V_HIS_SERE_SERV>();

                            sereServ1List = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_15>>(sereSerMap);

                            SereServPrintList.AddRange(sereServ1List);
                        }
                    }
                }

                var treatmentIds = SereServPrintList.Select(s => s.TDL_TREATMENT_ID ?? 0).Distinct().ToList();

                List<HIS_TREATMENT_BED_ROOM> treatmentBedRoom = new List<HIS_TREATMENT_BED_ROOM>();
                List<V_HIS_BED_LOG> bedLog = new List<V_HIS_BED_LOG>();

                var sk = 0;
                while (treatmentIds.Count - sk > 0)
                {
                    var listIDs = treatmentIds.Skip(sk).Take(MAX_REQ).ToList();
                    sk += MAX_REQ;

                    CommonParam param = new CommonParam();
                    var treatBedRoomFilter = new HisTreatmentBedRoomFilter();
                    treatBedRoomFilter.TREATMENT_IDs = listIDs;
                    var tBedRoom = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, treatBedRoomFilter, param);
                    if (tBedRoom != null && tBedRoom.Count > 0)
                    {
                        treatmentBedRoom.AddRange(tBedRoom);
                    }

                    var BedLogFilter = new HisBedLogViewFilter();
                    BedLogFilter.TREATMENT_IDs = listIDs;
                    var bLog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, BedLogFilter, param);
                    if (bLog != null && bLog.Count > 0)
                    {
                        bedLog.AddRange(bLog);
                    }
                }

                //var skip = 0;
                //while (SereServPrintList.Count - skip > 0)
                //{
                //    var sereServSubList = SereServPrintList.Skip(skip).Take(8).ToList();
                //    skip = skip + 8;
                //}

                MPS.Processor.Mps000371.PDO.Mps000371PDO pdo = new MPS.Processor.Mps000371.PDO.Mps000371PDO(
                   ServiceReqPrintList,
                   SereServPrintList,
                   BackendDataWorker.Get<HIS_SERVICE_UNIT>(),
                   treatmentBedRoom,
                   bedLog);

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                List<HIS_DEPARTMENT> departmentSelectList = new List<HIS_DEPARTMENT>();
                List<HIS_RATION_GROUP> rationGroupSelectList = new List<HIS_RATION_GROUP>();
                var selectdepartment = gridViewDepartment.GetSelectedRows();
                if (selectdepartment != null && selectdepartment.Count() > 0)
                {
                    foreach (var item in selectdepartment)
                    {
                        var department = (HIS_DEPARTMENT)gridViewDepartment.GetRow(item);
                        departmentSelectList.Add(department);
                    }
                }

                var selectRationGroup = gridViewRationGroup.GetSelectedRows();
                if (selectRationGroup != null && selectRationGroup.Count() > 0)
                {
                    foreach (var item in selectRationGroup)
                    {
                        var rationGroup = (HIS_RATION_GROUP)gridViewRationGroup.GetRow(item);
                        rationGroupSelectList.Add(rationGroup);
                    }
                }

                if (departmentSelectList == null || departmentSelectList.Count == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Chưa chọn khoa");
                    return;
                }

                // get ration_sum
                MOS.Filter.HisRationSumViewFilter rationSumFilter = new MOS.Filter.HisRationSumViewFilter();
                if (departmentSelectList != null && departmentSelectList.Count > 0)
                {
                    rationSumFilter.DEPARTMENT_IDs = departmentSelectList.Select(o => o.ID).Distinct().ToList();
                }
                var rationSumList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_RATION_SUM>>("api/HisRationSum/GetView", ApiConsumer.ApiConsumers.MosConsumer, rationSumFilter, null);

                foreach (var rationSum in rationSumList)
                {
                    MPS.Processor.Mps000273.PDO.Mps000273ADO ado = new MPS.Processor.Mps000273.PDO.Mps000273ADO();
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_15, V_HIS_SERE_SERV_1>();
                    List<V_HIS_SERE_SERV_1> sereServListAll1 = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_1>>(sereServListAll);
                    List<V_HIS_SERE_SERV_1> sereServList = sereServListAll1.Where(o => o.RATION_SUM_ID == rationSum.ID).ToList(); ;
                    if (rationGroupSelectList != null && rationGroupSelectList.Count > 0)
                    {
                        List<V_HIS_SERVICE> serviceList = new List<V_HIS_SERVICE>();

                        var serviceExist = this.serviceListAll.Where(o => o.RATION_GROUP_ID.HasValue && rationGroupSelectList.Select(p => p.ID).Contains(o.RATION_GROUP_ID.Value)).ToList();
                        if (serviceExist != null && serviceExist.Count > 0)
                            serviceList.AddRange(serviceExist);

                        var rationGroupNotExist = rationGroupSelectList.FirstOrDefault(o => o.ID == RationGroupNotExistId);
                        if (rationGroupNotExist != null)
                        {
                            var serviceNotExist = this.serviceListAll.Where(o => !o.RATION_GROUP_ID.HasValue).ToList();
                            if (serviceNotExist != null && serviceNotExist.Count > 0)
                                serviceList.AddRange(serviceNotExist);
                        }

                        if (serviceList != null && serviceList.Count > 0)
                        {
                            sereServList = sereServList.Where(o => serviceList.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList();
                        }
                    }


                    if (sereServList == null || sereServList.Count == 0)
                        continue;

                    MPS.Processor.Mps000273.PDO.Mps000273PDO pdo = new MPS.Processor.Mps000273.PDO.Mps000273PDO(
                        rationSum,
                        sereServList,
                        ado);

                    MPS.ProcessorBase.Core.PrintData printData = null;

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditor.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000273, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnInTongHop_Click(null, null);
        }
        #endregion

        private void btnInPhieuChiTiet_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000274, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnInTem_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000371, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000497, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        #region Public method

        #endregion
    }
}
