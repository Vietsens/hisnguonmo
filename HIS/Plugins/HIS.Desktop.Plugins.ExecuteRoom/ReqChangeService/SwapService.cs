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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;
using HIS.Desktop.Plugins.ExecuteRoom.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HIS.Desktop.Plugins.ExecuteRoom.ReqChangeService
{
    public partial class SwapService : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private SereServ6ADO currentSereServ;
        private ServiceReqADO ServiceReq;
        private HIS_SERVICE_REQ currentServiceReq;
        private HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        private List<SwapSereServADO> sereServADOs;
        private List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter;
        private List<V_HIS_EXECUTE_ROOM> allDataExecuteRooms;
        private HIS_DEPARTMENT currentDepartment;
        private HIS_SERVICE_CHANGE_REQ curentChange;
        private HIS_SERVICE_CHANGE_REQ reqChangeResult;

        private List<long> patientTypeIdAls;
        private const string commonString__true = "1";
        private bool isNotUseBhyt;
        private bool notSearch;
        private List<HIS_PATIENT_TYPE> lstPatientType;
        internal SwapService(Inventec.Desktop.Common.Modules.Module module, ServiceReqADO serviceReq, SereServ6ADO data, List<HIS_PATIENT_TYPE> lstPatientType)
            : base(module)
        {
            InitializeComponent();
            this.currentSereServ = data;
            this.ServiceReq = serviceReq;
            this.currentModule = module;
            this.lstPatientType = lstPatientType;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SwapService_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("SwapService_Load Begin");
                this.allDataExecuteRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId) ?? new V_HIS_ROOM();
                this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == room.DEPARTMENT_ID) ?? new HIS_DEPARTMENT();
                btnPrint.Enabled = false;
                GetServiceReq();
                CheckChanging();
                LoadCurrentPatientTypeAlter(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME);
                PatientTypeWithPatientTypeAlter();
                LoadDataToPatientType(repositoryItemCboPatientType, currentPatientTypeWithPatientTypeAlter);

                List<HIS_PATIENT_TYPE> patientTypePrimary = new List<HIS_PATIENT_TYPE>();
                if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
                }

                LoadDataToPatientType(repositoryItemCboPrimaryPatientType, patientTypePrimary);
                LoadGridSereServ();
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Info("SwapService_Load End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện SwapService
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Lang", typeof(SwapService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("SwapService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("SwapService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("SwapService.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Selected.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_Selected.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientTypeId.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_PatientTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientTypeId.ToolTip = Inventec.Common.Resource.Get.Value("SwapService.gc_PatientTypeId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCboPatientType.NullText = Inventec.Common.Resource.Get.Value("SwapService.repositoryItemCboPatientType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PrimaryPatientTypeId.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_PrimaryPatientTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PrimaryPatientTypeId.ToolTip = Inventec.Common.Resource.Get.Value("SwapService.gc_PrimaryPatientTypeId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCboPrimaryPatientType.NullText = Inventec.Common.Resource.Get.Value("SwapService.repositoryItemCboPrimaryPatientType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Amount.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Price.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Expend.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_Expend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Expend.ToolTip = Inventec.Common.Resource.Get.Value("SwapService.gc_Expend.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_OutParentFee.Caption = Inventec.Common.Resource.Get.Value("SwapService.gc_OutParentFee.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_OutParentFee.ToolTip = Inventec.Common.Resource.Get.Value("SwapService.gc_OutParentFee.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("SwapService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("SwapService.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnPrint.Caption = Inventec.Common.Resource.Get.Value("SwapService.barBtnPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("SwapService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckChanging()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceChangeReqFilter filter = new HisServiceChangeReqFilter();
                filter.SERE_SERV_ID = currentSereServ.ID;
                var hisServiceCHangeReq = new BackendAdapter(param).Get<List<HIS_SERVICE_CHANGE_REQ>>("api/HisServiceChangeReq/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (hisServiceCHangeReq != null && hisServiceCHangeReq.Count > 0)
                {
                    this.curentChange = hisServiceCHangeReq.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = ServiceReq.ID;
                var hisServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.currentServiceReq = hisServiceReq != null && hisServiceReq.Count > 0 ? hisServiceReq.FirstOrDefault() : new HIS_SERVICE_REQ();
                this.isNotUseBhyt = currentServiceReq.IS_NOT_USE_BHYT == 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);

                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemcboPatientType, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemcboPatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                var vPatientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                if (vPatientTypeAllows != null && vPatientTypeAllows.Count > 0 && currentHisTreatment != null)
                {
                    var patientTypeAllow = vPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == currentHisTreatment.TDL_PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                    if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                    {
                        currentPatientTypeWithPatientTypeAlter = lstPatientType.Where(o => patientTypeAllow.Contains(o.ID)).ToList();

                        if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            currentPatientTypeWithPatientTypeAlter = currentPatientTypeWithPatientTypeAlter.OrderBy(o => o.PRIORITY).ToList();
                            this.patientTypeIdAls = currentPatientTypeWithPatientTypeAlter.Select(s => s.ID).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (intructionTime > 0)
                    filter.INTRUCTION_TIME = intructionTime;
                else
                    filter.INTRUCTION_TIME = Inventec.Common.DateTime.Get.Now() ?? 0;
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridSereServ()
        {
            try
            {
                if (currentSereServ != null)
                {
                    List<V_HIS_SERVICE_ROOM> serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == this.currentModule.RoomId).ToList();
                    List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == currentSereServ.TDL_SERVICE_TYPE_ID && o.ID != currentSereServ.SERVICE_ID && o.IS_ACTIVE == 1).ToList();
                    sereServADOs = new List<SwapSereServADO>();
                    foreach (var serviceRoom in serviceRooms)
                    {
                        var serviceAlow = services.FirstOrDefault(o => o.ID == serviceRoom.SERVICE_ID);
                        if (serviceAlow != null)
                        {
                            if (!BranchDataWorker.HasServicePatyWithListPatientType(serviceAlow.ID, this.patientTypeIdAls))
                                continue;

                            SwapSereServADO model = new SwapSereServADO();

                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERVICE, SwapSereServADO>();
                            model = AutoMapper.Mapper.Map<V_HIS_SERVICE, SwapSereServADO>(serviceAlow);

                            model.ID = 0;
                            model.SERVICE_ID = serviceAlow.ID;
                            model.TDL_SERVICE_CODE = serviceAlow.SERVICE_CODE;
                            model.TDL_SERVICE_NAME = serviceAlow.SERVICE_NAME;
                            model.TDL_SERVICE_TYPE_ID = serviceAlow.SERVICE_TYPE_ID;
                            model.IS_MULTI_REQUEST = serviceAlow.IS_MULTI_REQUEST;
                            model.IsAllowExpend = serviceAlow.IS_ALLOW_EXPEND;
                            model.HEIN_LIMIT_PRICE_OLD = serviceAlow.HEIN_LIMIT_PRICE_OLD;
                            model.HEIN_LIMIT_RATIO_OLD = serviceAlow.HEIN_LIMIT_RATIO_OLD;
                            model.HEIN_LIMIT_PRICE_IN_TIME = serviceAlow.HEIN_LIMIT_PRICE_IN_TIME;
                            model.HEIN_LIMIT_PRICE_INTR_TIME = serviceAlow.HEIN_LIMIT_PRICE_INTR_TIME;
                            model.BILL_PATIENT_TYPE_ID = serviceAlow.BILL_PATIENT_TYPE_ID;

                            model.SERVICE_NAME_HIDDEN = convertToUnSign3(serviceAlow.SERVICE_NAME) + serviceAlow.SERVICE_NAME;
                            model.SERVICE_CODE_HIDDEN = convertToUnSign3(serviceAlow.SERVICE_CODE) + serviceAlow.SERVICE_CODE;

                            if (this.curentChange != null && this.curentChange.ALTER_SERVICE_ID == serviceAlow.ID)
                            {
                                model.Selected = true;
                                model.PATIENT_TYPE_ID = curentChange.PATIENT_TYPE_ID;
                                model.PRIMARY_PATIENT_TYPE_ID = curentChange.PRIMARY_PATIENT_TYPE_ID;
                                model.AMOUNT = this.currentSereServ.AMOUNT;
                                model.IsExpend = this.currentSereServ.IS_EXPEND == 1;
                                model.IsOutKtcFee = this.currentSereServ.IS_OUT_PARENT_FEE == 1;
                            }
                            sereServADOs.Add(model);
                        }
                    }

                    if (sereServADOs != null && sereServADOs.Count > 0)
                    {
                        sereServADOs = sereServADOs.OrderByDescending(o => o.Selected).ThenBy(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_CODE).ToList();
                        gridControl1.DataSource = null;
                        List<SwapSereServADO> sereServADOsData = sereServADOs;
                        gridControl1.DataSource = sereServADOsData;
                        gridControl1.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string convertToUnSign3(string s)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(s))
                    return "";

                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            catch
            {

            }
            return "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (sereServADOs.Exists(o => o.Selected))
                {
                    List<SwapSereServADO> SelectedData = sereServADOs.Where(o => o.Selected).ToList();
                    if (SelectedData != null && SelectedData.Count == 1)
                    {
                        SwapSereServADO data = SelectedData.First();
                        if (!String.IsNullOrWhiteSpace(data.ErrorMessageAmount)
                            || !String.IsNullOrWhiteSpace(data.ErrorMessagePatientTypeId))
                        {
                            return;
                        }

                        CommonParam param = new CommonParam();
                        bool success = false;
                        HisServiceChangeReqSDO sdo = new HisServiceChangeReqSDO();
                        sdo.AlterServiceId = data.SERVICE_ID;
                        sdo.PatientTypeId = data.PATIENT_TYPE_ID;
                        sdo.PrimaryPatientTypeId = data.PRIMARY_PATIENT_TYPE_ID;
                        sdo.SereServId = currentSereServ.ID;
                        sdo.WorkingRoomId = this.currentModule.RoomId;

                        reqChangeResult = new BackendAdapter(param).Post<HIS_SERVICE_CHANGE_REQ>("api/HisServiceChangeReq/Create", ApiConsumers.MosConsumer, sdo, param);
                        if (reqChangeResult != null)
                        {
                            success = true;
                            btnPrint.Enabled = true;
                        }

                        MessageManager.Show(this, param, success);
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_CellValueChanged. 1");
                var sereServADO = (SwapSereServADO)this.gridView1.GetFocusedRow();
                if (sereServADO != null)
                {
                    if (e.Column.FieldName == this.gc_Selected.FieldName
                        || e.Column.FieldName == this.gc_Expend.FieldName
                        || e.Column.FieldName == this.gc_Amount.FieldName
                        )
                    {
                        if (e.Column.FieldName == this.gc_Selected.FieldName)
                        {
                            foreach (var item in sereServADOs)
                            {
                                if (sereServADO.SERVICE_ID == item.SERVICE_ID)
                                {
                                    item.Selected = true;
                                }
                                else
                                {
                                    this.ResetOneService(item);
                                }
                            }
                        }

                        this.gridControl1.RefreshDataSource();
                        if (sereServADO.Selected)
                        {
                            sereServADO.AMOUNT = currentSereServ.AMOUNT;

                            int statusParentFee = currentSereServ.IS_OUT_PARENT_FEE ?? 0;
                            if (statusParentFee == 1)
                            {
                                sereServADO.IsOutKtcFee = true;
                            }
                            else
                            {
                                sereServADO.IsOutKtcFee = false;
                            }

                            int statusExpend = currentSereServ.IS_EXPEND ?? 0;
                            if (statusExpend == 1)
                            {
                                sereServADO.IsExpend = true;
                            }
                            else
                            {
                                sereServADO.IsExpend = false;
                            }

                            //sereServADO.PATIENT_TYPE_ID = currentSereServ.PATIENT_TYPE_ID;
                            //sereServADO.PRIMARY_PATIENT_TYPE_ID = currentSereServ.PRIMARY_PATIENT_TYPE_ID;
                            this.ChoosePatientTypeDefaultlService(this.currentSereServ.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, this.currentSereServ.PRIMARY_PATIENT_TYPE_ID);

                            this.ValidServiceDetailProcessing(sereServADO);
                            //this.ProcessNoDifferenceHeinServicePrice(sereServADO);

                            //if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            //{
                            //    MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //}
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_CellValueChanged. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long PackagePriceId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView1.GetRowCellValue(e.RowHandle, "PackagePriceId") ?? "").ToString());
                    bool Selected = Inventec.Common.TypeConvert.Parse.ToBoolean((gridView1.GetRowCellValue(e.RowHandle, "Selected") ?? "").ToString());
                    bool IsExpend = Inventec.Common.TypeConvert.Parse.ToBoolean((gridView1.GetRowCellValue(e.RowHandle, "IsExpend") ?? "").ToString());
                    bool IsOutKtcFee = Inventec.Common.TypeConvert.Parse.ToBoolean((gridView1.GetRowCellValue(e.RowHandle, "IsOutKtcFee") ?? "").ToString());
                    if (e.Column.FieldName == gc_PatientTypeId.FieldName)
                    {
                        if (PackagePriceId > 0)
                            e.RepositoryItem = this.repositoryItemCboPatientTypeReadOnly;
                        else
                            e.RepositoryItem = this.repositoryItemCboPatientType;
                    }
                    else if (e.Column.FieldName == gc_PrimaryPatientTypeId.FieldName)
                    {
                        if (PackagePriceId > 0)
                            e.RepositoryItem = this.repositoryItemCboPatientTypeReadOnly;
                        else
                            e.RepositoryItem = this.repositoryItemCboPrimaryPatientType;
                    }
                    else if (e.Column.FieldName == gc_Expend.FieldName)
                    {
                        if (Selected && IsExpend)
                            e.RepositoryItem = this.repositoryItemChkExpend;
                        else
                            e.RepositoryItem = this.repositoryItemBtnChkDisable;
                    }
                    else if (e.Column.FieldName == gc_OutParentFee.FieldName)
                    {
                        if (Selected && IsOutKtcFee)
                            e.RepositoryItem = this.repositoryItemChkOutParentFee;
                        else
                            e.RepositoryItem = this.repositoryItemBtnChkDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        SwapSereServADO oneServiceSDO = (SwapSereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.Selected)
                            {
                                e.Value = GetPriceBySurg(oneServiceSDO);
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SwapSereServADO data = view.GetFocusedRow() as SwapSereServADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        var dataSource = editor.Properties.DataSource;
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && data.Selected && !data.BILL_PATIENT_TYPE_ID.HasValue)
                    {
                        this.FillDataIntoPrimaryPatientTypeCombo(data, editor);
                        editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, info.Column));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT" || e.ColumnName == "PATIENT_TYPE_ID" || e.ColumnName == "TDL_SERVICE_NAME")
                {
                    var index = this.gridView1.GetDataSourceRowIndex(e.RowHandle);
                    if (index < 0)
                    {
                        e.Info.ErrorType = ErrorType.None;
                        e.Info.ErrorText = "";
                        return;
                    }
                    var listDatas = this.gridControl1.DataSource as List<SwapSereServADO>;
                    var row = listDatas[index];
                    if (e.ColumnName == "AMOUNT")
                    {
                        if (row.Selected && row.ErrorTypeAmount == ErrorType.Warning)
                        {
                            e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                            e.Info.ErrorText = (string)(row.ErrorMessageAmount);
                        }
                        else
                        {
                            e.Info.ErrorType = (ErrorType)(ErrorType.None);
                            e.Info.ErrorText = "";
                        }
                    }
                    else if (e.ColumnName == "PATIENT_TYPE_ID")
                    {
                        if (row.Selected && row.ErrorTypePatientTypeId == ErrorType.Warning)
                        {
                            e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
                            e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
                        }
                        else
                        {
                            e.Info.ErrorType = (ErrorType)(ErrorType.None);
                            e.Info.ErrorText = "";
                        }
                    }
                    else if (e.ColumnName == "TDL_SERVICE_NAME")
                    {
                        if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
                        {
                            e.Info.ErrorType = (ErrorType)(row.ErrorTypeIsAssignDay);
                            e.Info.ErrorText = (string)(row.ErrorMessageIsAssignDay);
                        }
                        else
                        {
                            e.Info.ErrorType = (ErrorType)(ErrorType.None);
                            e.Info.ErrorText = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.RowCount == 2)
                {
                    var sereServADO = (SwapSereServADO)this.gridView1.GetRow(0);
                    if (sereServADO != null)
                    {
                        sereServADO.Selected = true;
                        if (sereServADO.Selected)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisTreatment.TDL_PATIENT_TYPE_ID ?? 0, sereServADO.SERVICE_ID, sereServADO, this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                            this.ValidServiceDetailProcessing(sereServADO);
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }

                        this.gridControl1.RefreshDataSource();
                        //gridView1.ActiveEditor.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridView1.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (SwapSereServADO)gridView1.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                //if (hi.Column.FieldName == "IsChecked" && (dataRow.IsAllowChecked == false))
                                //{
                                //    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                //    return;
                                //}
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;
                            if (checkEdit.ReadOnly)
                                return;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal? GetPriceBySurg(SwapSereServADO sereServADOOld)
        {
            decimal? resultData = null;
            decimal? heinLimitPrice = null;
            decimal? heinLimitRatio = null;
            try
            {
                long instructionTime = this.currentServiceReq.INTRUCTION_TIME;
                if (sereServADOOld.PATIENT_TYPE_ID != 0 && BranchDataWorker.DicServicePatyInBranch.ContainsKey(sereServADOOld.SERVICE_ID) && instructionTime > 0)
                {
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                    if (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0)
                    {
                        dataCombo = this.allDataExecuteRooms.Where(o => sereServADOOld.TDL_EXECUTE_ROOM_ID == o.ROOM_ID).ToList();
                    }
                    else if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                    {
                        arrExcuteRoomCode = serviceRoomViews.Where(o => sereServADOOld != null && o.SERVICE_ID == sereServADOOld.SERVICE_ID).ToList();
                        V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId) ?? new V_HIS_ROOM();
                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && this.allDataExecuteRooms != null) ?
                            this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID) && o.BRANCH_ID == room.BRANCH_ID).ToList()
                            : null);
                    }

                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault() : null;
                    if (checkExecuteRoom != null)
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                    }
                    else
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    }
                    long? intructionNumByType = null;

                    //List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == sereServADOOld.SERVICE_TYPE_ID).ToList() : null;
                    //intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;

                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(sereServADOOld.SERVICE_ID, this.patientTypeIdAls);

                    V_HIS_SERVICE_PATY oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.currentServiceReq.REQUEST_ROOM_ID, this.currentServiceReq.REQUEST_DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PATIENT_TYPE_ID, null, intructionNumByType, sereServADOOld.PackagePriceId, null);
                    if (sereServADOOld.PRIMARY_PATIENT_TYPE_ID.HasValue)
                    {
                        V_HIS_SERVICE_PATY primary = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.currentServiceReq.REQUEST_ROOM_ID, this.currentServiceReq.REQUEST_DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PRIMARY_PATIENT_TYPE_ID.Value, null, intructionNumByType, sereServADOOld.PackagePriceId, null);
                        if (oneServicePatyPrice == null || primary == null || (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO)) >= (primary.PRICE * (1 + primary.VAT_RATIO)))
                        {
                            if (HisConfigCFG.IsSetPrimaryPatientType != "2")
                            {
                                sereServADOOld.PRIMARY_PATIENT_TYPE_ID = null;
                            }
                        }
                        oneServicePatyPrice = primary;
                    }

                    if (sereServADOOld.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    //&& sereServADOOld.IsNoDifference.HasValue
                    //&& sereServADOOld.IsNoDifference.Value)
                    {
                        this.GetHeinLimitPrice(sereServADOOld, instructionTime, this.currentHisTreatment.IN_TIME, ref heinLimitPrice, ref heinLimitRatio);

                        if (heinLimitPrice.HasValue && heinLimitPrice.Value > 0)
                        {
                            resultData = heinLimitPrice;
                        }
                        else if (heinLimitRatio.HasValue && heinLimitRatio.Value > 0 && oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO) * heinLimitRatio.Value);
                        }
                        else if (oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                        }
                    }
                    else if (oneServicePatyPrice != null)
                    {
                        resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                    }
                }
                else
                {
                    resultData = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return resultData;
        }

        private void GetHeinLimitPrice(SwapSereServADO hisService, long instructionTime, long inTime, ref decimal? heinLimitPrice, ref decimal? heinLimitRatio)
        {
            //neu dich vu khai bao gia tran
            if (hisService.HEIN_LIMIT_PRICE.HasValue || hisService.HEIN_LIMIT_PRICE_OLD.HasValue)
            {
                //neu gia ap dung theo ngay vao vien, thi cac benh nhan vao vien truoc ngay ap dung se lay gia cu
                if (hisService.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                {
                    heinLimitPrice = inTime < hisService.HEIN_LIMIT_PRICE_IN_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ap dung theo ngay chi dinh, thi cac chi dinh truoc ngay ap dung se tinh gia cu
                else if (hisService.HEIN_LIMIT_PRICE_INTR_TIME.HasValue)
                {
                    heinLimitPrice = instructionTime < hisService.HEIN_LIMIT_PRICE_INTR_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ca 2 truong ko co gia tri thi luon lay theo gia moi
                else
                {
                    heinLimitPrice = hisService.HEIN_LIMIT_PRICE;
                }
            }
        }

        private void FillDataIntoPrimaryPatientTypeCombo(SwapSereServADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls))
                {
                    long instructionTime = this.currentServiceReq.INTRUCTION_TIME;
                    long? intructionNumByType = null;
                    //List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == data.SERVICE_TYPE_ID).ToList() : null;
                    //intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls);
                    var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.currentServiceReq.REQUEST_ROOM_ID, this.currentServiceReq.REQUEST_DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.SERVICE_ID, data.PATIENT_TYPE_ID, null, intructionNumByType);

                    var patientTypePrimatyList = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();

                    var patyIds = servicePaties.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                    patyIds = patientTypePrimatyList != null && patientTypePrimatyList.Count > 0 ? patyIds.Where(o => patientTypePrimatyList.Select(p => p.ID).Contains(o)).ToList() : null;
                    if (patyIds != null)
                    {
                        foreach (var item in patyIds)
                        {
                            if (item == data.PATIENT_TYPE_ID)
                                continue;
                            var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.currentServiceReq.REQUEST_ROOM_ID, this.currentServiceReq.REQUEST_DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.SERVICE_ID, item, null, intructionNumByType);
                            if (itemPaty == null || currentPaty == null || (currentPaty.PRICE * (1 + currentPaty.VAT_RATIO)) >= (itemPaty.PRICE * (1 + itemPaty.VAT_RATIO)))
                                continue;
                            dataCombo.Add(this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == item));
                        }
                    }
                }

                this.InitComboPatientType(patientTypeCombo, dataCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(SwapSereServADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null)
                {
                    long intructionTime = this.currentServiceReq.INTRUCTION_TIME;
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
                    if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls))
                    {
                        var arrPatientTypeCode = BranchDataWorker.ServicePatyWithListPatientType(data.SERVICE_ID, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_CODE).Distinct().ToList();
                        dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o =>
                            //(o.IS_ADDITION != (short)1 || o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE) && 
                            arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);
                    }

                    this.InitComboPatientType(patientTypeCombo, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, SwapSereServADO sereServADO, long? primaryPatientTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.1");
                if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    long intructionTime = this.currentServiceReq.INTRUCTION_TIME;
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime)) && ((!sereServADO.PackagePriceId.HasValue && !o.PACKAGE_ID.HasValue) || (sereServADO.PackagePriceId.HasValue && sereServADO.PackagePriceId.Value == o.PACKAGE_ID))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var currentPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                    var primaryPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1 && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                    Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.2");
                    if (patientTypeIdInSePas == null || patientTypeIdInSePas.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.3");
                        var patientTypeIdInSePasCheckForErr = BranchDataWorker.DicServicePatyInBranch[serviceId];
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdInSePasCheckForErr), patientTypeIdInSePasCheckForErr) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdAls), patientTypeIdAls));
                    }
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.4");
                        if (HisConfigCFG.IsSetPrimaryPatientType != commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).FirstOrDefault();
                            Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.6___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID), this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID));
                        }
                        else if (this.currentHisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.HEIN_CARD_FROM_TIME).Value.Date > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date
                        || Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.HEIN_CARD_TO_TIME).Value.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date
                        ))
                        {
                            result = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == HisConfigCFG.PatientTypeId__VP).FirstOrDefault();
                            Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.7");
                        }
                        else
                        {
                            result = ((currentPatientTypeTemps.Exists(t => t.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && t.ID != HisConfigCFG.PatientTypeId__BHYT)))) ? (currentPatientTypeTemps.Where(o => o.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).FirstOrDefault() ?? currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).FirstOrDefault()) : currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).FirstOrDefault());
                            Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.8");
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeTemps), currentPatientTypeTemps)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsSetPrimaryPatientType), HisConfigCFG.IsSetPrimaryPatientType));
                        if (result != null && sereServADO != null)
                        {
                            sereServADO.PATIENT_TYPE_ID = result.ID;
                            sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                            sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                        }
                        if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            if (this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID <= 0)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                            }
                            else
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID;
                                if (primaryPatientTypeTemps.Exists(e => e.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID))
                                {
                                    var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                }
                                else
                                {
                                    try
                                    {
                                        var billPaty = lstPatientType.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                        sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName);
                                        sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                    }

                                }
                            }
                        }
                        else if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value)
                        {
                            if (primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            {
                                var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            }
                            else
                            {
                                try
                                {
                                    var billPaty = lstPatientType.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                    sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName);
                                    sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                        }
                        else if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                            && primaryPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else
                        {
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                        }

                        if (primaryPatientTypeId.HasValue && !sereServADO.PRIMARY_PATIENT_TYPE_ID.HasValue)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == primaryPatientTypeId.Value);
                            if (priPaty != null)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeTemps), currentPatientTypeTemps));
                    }
                }
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ValidServiceDetailProcessing(SwapSereServADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    if (HisConfigCFG.IsSetPrimaryPatientType != "2" || sereServADO.ErrorTypePatientTypeId == ErrorType.None)
                    {
                        bool vlPatientTypeId = (sereServADO.Selected && sereServADO.PATIENT_TYPE_ID <= 0);
                        sereServADO.ErrorMessagePatientTypeId = (vlPatientTypeId ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                        sereServADO.ErrorTypePatientTypeId = (vlPatientTypeId ? ErrorType.Warning : ErrorType.None);
                    }

                    bool vlAmount = (sereServADO.Selected && sereServADO.AMOUNT <= 0);
                    sereServADO.ErrorMessageAmount = (vlAmount ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypeAmount = (vlAmount ? ErrorType.Warning : ErrorType.None);

                    //List<HIS_SERE_SERV> serviceSames = null;
                    //List<SwapSereServADO> serviceSameResult = null;
                    //CheckServiceSameByServiceId(sereServADO, this.currentServiceSames, ref serviceSames, ref serviceSameResult);
                    //var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.First().ToString().Substring(0, 8));

                    //if (existsSereServInDate && (serviceSames != null && serviceSames.Count > 0))
                    //{
                    //    sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuVaDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                    //    sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    //}
                    //else if (existsSereServInDate)
                    //{
                    //    sereServADO.ErrorMessageIsAssignDay = (existsSereServInDate ? ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay : "");
                    //    sereServADO.ErrorTypeIsAssignDay = (existsSereServInDate ? ErrorType.Warning : ErrorType.None);
                    //}
                    //else if (serviceSames != null && serviceSames.Count > 0)
                    //{
                    //    sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                    //    sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    //}
                    //else if (serviceSameResult != null && serviceSameResult.Count > 0)
                    //{
                    //    sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoChe, string.Join("; ", serviceSameResult.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                    //    sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    //}
                    //else
                    //{
                    //    sereServADO.ErrorMessageIsAssignDay = "";
                    //    sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void CheckServiceSameByServiceId(SwapSereServADO sereServADO, List<V_HIS_SERVICE_SAME> serviceSameAll, ref List<HIS_SERE_SERV> result, ref List<SwapSereServADO> resultSelect)
        //{
        //    try
        //    {
        //        result = null;
        //        resultSelect = null;
        //        if (sereServADO != null && serviceSameAll != null && serviceSameAll.Count > 0)
        //        {
        //            //Lay ra cac dich vu cung co che voi dich vu dang duoc chon

        //            //Lay cac dich vu cung co che voi no
        //            List<long> serviceSameId1s = serviceSameAll
        //                .Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.SAME_ID != sereServADO.SERVICE_ID)
        //                .Select(o => o.SAME_ID).ToList();
        //            //Hoac cac dich vu ma no cung co che
        //            List<long> serviceSameId2s = serviceSameAll
        //                .Where(o => o.SAME_ID == sereServADO.SERVICE_ID && o.SERVICE_ID != sereServADO.SERVICE_ID)
        //                .Select(o => o.SERVICE_ID).ToList();

        //            List<long> serviceSameIds = new List<long>();

        //            if (serviceSameId1s != null)
        //            {
        //                serviceSameIds.AddRange(serviceSameId1s);
        //            }
        //            if (serviceSameId2s != null)
        //            {
        //                serviceSameIds.AddRange(serviceSameId2s);
        //            }
        //            result = new List<HIS_SERE_SERV>();

        //            if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count() > 0)
        //            {

        //                long intructionTimeFrom = 0, intructionTimeTo = 0;
        //                DateTime itime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

        //                if (itime != null && itime != DateTime.MinValue)
        //                {
        //                    intructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "000000");
        //                    intructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "235959");
        //                }
        //                else
        //                {
        //                    intructionTimeFrom = (Inventec.Common.DateTime.Get.StartDay() ?? 0);
        //                    intructionTimeTo = (Inventec.Common.DateTime.Get.EndDay() ?? 0);
        //                }

        //                var checkServiceSame = this.sereServWithTreatment.Where(o => (intructionTimeFrom <= o.TDL_INTRUCTION_TIME && o.TDL_INTRUCTION_TIME <= intructionTimeTo) && serviceSameIds.Contains(o.SERVICE_ID));

        //                if (checkServiceSame != null && checkServiceSame.Count() > 0)
        //                {
        //                    var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
        //                    foreach (var serviceSameItems in groupServiceSame)
        //                    {
        //                        result.Add(serviceSameItems.FirstOrDefault());
        //                    }
        //                }
        //                else
        //                {
        //                    result = null;
        //                }
        //            }

        //            List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
        //            if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
        //            {
        //                var checkServiceSame = serviceCheckeds__Send.Where(o => serviceSameIds.Contains(o.SERVICE_ID));
        //                resultSelect = new List<SereServADO>();
        //                if (checkServiceSame != null && checkServiceSame.Count() > 0)
        //                {
        //                    var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
        //                    foreach (var serviceSameItems in groupServiceSame)
        //                    {
        //                        resultSelect.Add(serviceSameItems.FirstOrDefault());
        //                    }
        //                }
        //                else
        //                {
        //                    resultSelect = null;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void ProcessNoDifferenceHeinServicePrice(SwapSereServADO sereServADO)
        //{
        //    try
        //    {
        //        bool finded = false;
        //        if (this.currentHisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
        //            && HisConfigCFG.NoDifference == commonString__true)
        //        {
        //            var headCards = !String.IsNullOrEmpty(HisConfigCFG.HeadCardNumberNoDifference) ? HisConfigCFG.HeadCardNumberNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
        //            if ((headCards != null && !String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER) && headCards.Where(o => this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.StartsWith(o.Trim())).Any())
        //                )
        //            {
        //                sereServADO.IsNoDifference = true;
        //                finded = true;
        //            }

        //            var departmentCodes = !String.IsNullOrEmpty(HisConfigCFG.DepartmentCodeNoDifference) ? HisConfigCFG.DepartmentCodeNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
        //            if (departmentCodes != null && departmentCodes.Contains(this.requestRoom.DEPARTMENT_CODE))
        //            {
        //                sereServADO.IsNoDifference = true;
        //                finded = true;
        //            }
        //            //IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.
        //            var heinService = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServADO.SERVICE_ID);
        //            if (heinService != null)
        //            {
        //                sereServADO.HEIN_LIMIT_PRICE = heinService.HEIN_LIMIT_PRICE;
        //            }

        //            if (!finded)
        //            {
        //                sereServADO.IsNoDifference = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private bool CheckExistServicePaymentLimit(string ServiceCode)
        {
            bool result = false;
            try
            {
                string servicePaymentLimit = HisConfigCFG.ServiceHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(servicePaymentLimit))
                {
                    string[] serviceArr = servicePaymentLimit.Split(',');
                    if (serviceArr != null && serviceArr.Length > 0)
                    {
                        if (serviceArr.Contains(ServiceCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ResetOneService(SwapSereServADO item)
        {
            try
            {
                item.Selected = false;

                item.PATIENT_TYPE_ID = 0;
                item.PATIENT_TYPE_CODE = null;
                item.PATIENT_TYPE_NAME = null;
                item.TDL_EXECUTE_ROOM_ID = 0;

                item.PRICE = 0;

                item.AMOUNT = 0;
                item.IsExpend = false;
                item.IsOutKtcFee = false;

                //item.IsNoDifference = false;
                item.PRIMARY_PATIENT_TYPE_ID = null;
                item.ErrorMessageAmount = "";
                item.ErrorTypeAmount = ErrorType.None;
                item.ErrorMessagePatientTypeId = "";
                item.ErrorTypePatientTypeId = ErrorType.None;
                item.ErrorMessageIsAssignDay = "";
                item.ErrorTypeIsAssignDay = ErrorType.None;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchServiceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notSearch)
                    return;

                this.LoadDataToGrid(false);
                Inventec.Common.Logging.LogSystem.Debug("txtServiceCode_Search_EditValueChanged. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid(bool isResetSearchtext)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 1  isResetSearchtext = " + isResetSearchtext);
                this.gridView1.ClearGrouping();
                List<SwapSereServADO> listSereServADO = new List<SwapSereServADO>();
                if (sereServADOs != null)
                {
                    listSereServADO.AddRange(sereServADOs);
                }

                if (isResetSearchtext)
                {
                    this.notSearch = true;
                    this.txtSearchServiceCode.Text = "";
                    this.txtSearchServiceName.Text = "";
                    this.notSearch = false;
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 5");
                this.gridControl1.DataSource = null;
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 5.1");
                if (!String.IsNullOrWhiteSpace(txtSearchServiceName.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                {
                    listSereServADO = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtSearchServiceName.Text.ToLower().Trim())).ToList();
                }

                if (!String.IsNullOrWhiteSpace(txtSearchServiceCode.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                {
                    listSereServADO = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtSearchServiceCode.Text.ToLower().Trim())).ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 5.2");

                this.gridControl1.DataSource =
                    listSereServADO != null && listSereServADO.Count > 0 ?
                    listSereServADO.OrderBy(o => o.SERVICE_TYPE_ID)
                        .ThenByDescending(o => o.SERVICE_NUM_ORDER)
                        .ThenBy(o => o.TDL_SERVICE_NAME).ToList()
                    : null;

                this.gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchServiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSearchServiceCode.Focus();
                    txtSearchServiceCode.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    gridView1.FocusedColumn = gc_ServiceCode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchServiceName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notSearch)
                    return;

                this.LoadDataToGrid(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnPrint.Enabled && btnPrint.Visible)
                {
                    Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richStore.RunPrintTemplate("Mps000433", this.DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.reqChangeResult == null)
                    return result;

                WaitingManager.Show();

                V_HIS_SERVICE_REQ req = new V_HIS_SERVICE_REQ();
                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                V_HIS_PATIENT_TYPE_ALTER patienTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                V_HIS_SERVICE_CHANGE_REQ reqChange = new V_HIS_SERVICE_CHANGE_REQ();
                List<HIS_SERE_SERV> sereServ = new List<HIS_SERE_SERV>();

                CommonParam param = new CommonParam();

                HisServiceReqViewFilter reqFilter = new HisServiceReqViewFilter();
                reqFilter.ID = reqChangeResult.TDL_SERVICE_REQ_ID;
                var reqApiResult = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, reqFilter, ProcessLostToken, param);
                if (reqApiResult != null && reqApiResult.Count > 0)
                {
                    req = reqApiResult.FirstOrDefault();
                }

                HisTreatmentViewFilter treaFilter = new HisTreatmentViewFilter();
                treaFilter.ID = ServiceReq.TREATMENT_ID;
                var treatApiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treaFilter, ProcessLostToken, param);
                if (treatApiResult != null && treatApiResult.Count > 0)
                {
                    treatment = treatApiResult.FirstOrDefault();
                }

                patienTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, ServiceReq.TREATMENT_ID, param);

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.IDs = new List<long> { (reqChangeResult.ALTER_SERE_SERV_ID ?? 0), reqChangeResult.SERE_SERV_ID };
                var ssApiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, ProcessLostToken, param);
                if (ssApiResult != null && ssApiResult.Count > 0)
                {
                    sereServ.AddRange(ssApiResult);
                }

                HisServiceChangeReqFilter chanFilter = new HisServiceChangeReqFilter();
                chanFilter.ID = reqChangeResult.ID;
                var chanApiResult = new BackendAdapter(param).Get<List<V_HIS_SERVICE_CHANGE_REQ>>("api/HisServiceChangeReq/GetView", ApiConsumers.MosConsumer, chanFilter, ProcessLostToken, param);
                if (chanApiResult != null && chanApiResult.Count > 0)
                {
                    reqChange = chanApiResult.FirstOrDefault();
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(ServiceReq.TDL_TREATMENT_CODE, printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000433.PDO.Mps000433PDO pdo = new MPS.Processor.Mps000433.PDO.Mps000433PDO(treatment, patienTypeAlter, req, reqChange, sereServ);

                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void SwapService_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                lstPatientType = null;
                notSearch = false;
                isNotUseBhyt = false;
                patientTypeIdAls = null;
                reqChangeResult = null;
                curentChange = null;
                currentDepartment = null;
                allDataExecuteRooms = null;
                currentPatientTypeWithPatientTypeAlter = null;
                sereServADOs = null;
                currentHisTreatment = null;
                currentServiceReq = null;
                ServiceReq = null;
                currentSereServ = null;
                currentModule = null;
                this.txtSearchServiceName.EditValueChanged -= new System.EventHandler(this.txtSearchServiceName_EditValueChanged);
                this.barBtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
                this.txtSearchServiceCode.EditValueChanged -= new System.EventHandler(this.txtSearchServiceCode_EditValueChanged);
                this.txtSearchServiceCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtSearchServiceCode_KeyDown);
                this.gridView1.CustomRowColumnError -= new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridView1_CustomRowColumnError);
                this.gridView1.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridView1_CustomDrawGroupRow);
                this.gridView1.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView1_CustomRowCellEdit);
                this.gridView1.ShownEditor -= new System.EventHandler(this.gridView1_ShownEditor);
                this.gridView1.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
                this.gridView1.ColumnFilterChanged -= new System.EventHandler(this.gridView1_ColumnFilterChanged);
                this.gridView1.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
                this.gridView1.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.barBtnPrint.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
                this.Load -= new System.EventHandler(this.SwapService_Load);
                gridView3.GridControl.DataSource = null;
                repositoryItemCboPatientTypeReadOnly.DataSource = null;
                gridView2.GridControl.DataSource = null;
                repositoryItemCboPrimaryPatientType.DataSource = null;
                repositoryItemGridLookUpEdit1View.GridControl.DataSource = null;
                repositoryItemCboPatientType.DataSource = null;
                gridView1.GridControl.DataSource = null;
                gridControl1.DataSource = null;
                barBtnPrint = null;
                layoutControlItem1 = null;
                btnPrint = null;
                colIsOutKtcFeeUnb = null;
                colIsExpendUnb = null;
                repositoryItemBtnChkDisable = null;
                txtSearchServiceCode = null;
                txtSearchServiceName = null;
                layoutControlItem3 = null;
                panelControl1 = null;
                colOUT_PARENT_FEEUnb = null;
                colIS_EXPENDUnb = null;
                colPRICE_DISPLAYUnb = null;
                colAMOUNTUnb = null;
                colPRIMARY_PATIENT_TYPE_IDUnb = null;
                colPATIENT_TYPE_IDUnb = null;
                colTDL_SERVICE_NAMEUnb = null;
                colTDL_SERVICE_CODEUnb = null;
                colSelectedUnb = null;
                gridView3 = null;
                repositoryItemCboPatientTypeReadOnly = null;
                barDockControl4 = null;
                barDockControl3 = null;
                barDockControl2 = null;
                barDockControl1 = null;
                barBtnSave = null;
                bar1 = null;
                barManager1 = null;
                repositoryItemChkOutParentFee = null;
                repositoryItemChkExpend = null;
                repositoryItemSpAmount = null;
                gridView2 = null;
                repositoryItemCboPrimaryPatientType = null;
                repositoryItemGridLookUpEdit1View = null;
                repositoryItemCboPatientType = null;
                repositoryItemChkSelected = null;
                gc_OutParentFee = null;
                gc_Expend = null;
                gc_Price = null;
                gc_Amount = null;
                gc_PrimaryPatientTypeId = null;
                gc_PatientTypeId = null;
                gc_ServiceName = null;
                gc_ServiceCode = null;
                gc_Selected = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                gridView1 = null;
                gridControl1 = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
