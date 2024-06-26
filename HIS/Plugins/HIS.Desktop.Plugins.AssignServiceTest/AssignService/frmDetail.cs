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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTest.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTest.AssignService
{
    public partial class frmDetail : HIS.Desktop.Utility.FormBase
    {
        HisServiceReqListResultSDO serviceReqComboResultSDO;
        HIS_SERVICE_REQ_TYPE serviceReqType__Test;
        HIS_SERVICE_REQ_TYPE serviceReqType__Exam;
        HIS_SERVICE_REQ_TYPE serviceReqType__Diim;
        HIS_SERVICE_REQ_TYPE serviceReqType__Fuex;
        HIS_SERVICE_REQ_TYPE serviceReqType__Endo;
        HIS_SERVICE_REQ_TYPE serviceReqType__Suim;
        HIS_SERVICE_REQ_TYPE serviceReqType__Misu;
        HIS_SERVICE_REQ_TYPE serviceReqType__Surg;
        HIS_SERVICE_REQ_TYPE serviceReqType__Other;
        HIS_SERVICE_REQ_TYPE serviceReqType__Bed;
        HIS_SERVICE_REQ_TYPE serviceReqType__Reha;
        //bool isLoad = false;
        const string commonString__true = "1";
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        ChiDinhDichVuADO chiDinhDichVuADO;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> currentSereServs;
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        PopupMenu menu;
        Dictionary<string, object> dicParamPlus { get; set; }
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus { get; set; }
        Dictionary<string, System.Drawing.Image> dicImagePlus { get; set; }

        Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>> dicServiceReqData;
        Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>> dicSereServData;

        /// <summary>
        /// </summary>
        public enum PrintTypeBMK
        {
            IN_PHIEU_YEU_CAU,
            PHIEU_XET_NGHIEM_DOM_SOI,
            TAO_BIEU_MAU_KHAC
        }

        public frmDetail(HisServiceReqListResultSDO serviceReqComboResultSDO,
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter,
            HisTreatmentWithPatientTypeInfoSDO _currentHisTreatment)
        {
            InitializeComponent();
            this.serviceReqComboResultSDO = serviceReqComboResultSDO;
            this.currentHisPatientTypeAlter = currentHisPatientTypeAlter;
            this.currentHisTreatment = _currentHisTreatment;
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();

            serviceReqType__Test = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
            serviceReqType__Exam = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
            serviceReqType__Diim = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
            serviceReqType__Fuex = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
            serviceReqType__Endo = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
            serviceReqType__Suim = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
            serviceReqType__Misu = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
            serviceReqType__Surg = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
            serviceReqType__Other = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
            serviceReqType__Bed = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
            serviceReqType__Reha = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);

            //isLoad = false;
            if (this.serviceReqComboResultSDO != null)
            {
                gridControlServiceReqView.DataSource = this.serviceReqComboResultSDO.ServiceReqs;
                //LayDuLieuChungInCacPhieuChiDinh();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmDetail = new ResourceManager("HIS.Desktop.Plugins.AssignServiceTest.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignServiceTest.AssignService.frmDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.grcStt__TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcStt__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.repositoryItemButtonTabServiceViewReq_Print.NullText = Inventec.Common.Resource.Get.Value("frmDetail.repositoryItemButtonTabServiceViewReq_Print.NullText", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcServiceReqCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcServiceReqTypeName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqTypeName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcIntructionTime_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcIntructionTime_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcExecuteRoomName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcExecuteRoomName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcCreator_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcCreator_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcTotalAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcTotalPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDetail.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ dataRow = (V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    //else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    //{
                    //    long priority = (dataRow.PRIORITY ?? 0);
                    //    if ((priority == 1))
                    //    {
                    //        e.Value = imageListPriority.Images[0];
                    //    }
                    //}
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PRINT_DOM_SOI")
                    {
                        if (data.SERVICE_REQ_TYPE_ID == (serviceReqType__Test != null ? serviceReqType__Test.ID : 0))
                        {
                            e.RepositoryItem = ButtonEditPrintDomSoi;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonTabServiceViewReq_Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                currentServiceReq = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetFocusedRow();
                if (currentServiceReq != null)
                {
                    currentSereServs = this.serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Exam.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Diim.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Fuex.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Suim.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Misu.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Surg.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Other.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Bed.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Reha.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuYeuCau(string printTypeCode, bool printNow)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    var sdo = new HisServiceReqListResultSDO();
                    sdo.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { currentServiceReq };
                    sdo.SereServs = serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    if (currentServiceReq != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                        bedLogViewFilter.DEPARTMENT_IDs = new List<long>() { currentServiceReq.REQUEST_DEPARTMENT_ID };
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(sdo, currentHisTreatment, bedLogs);
                    PrintServiceReqProcessor.Print(printTypeCode, printNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case
PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP__MPS000027:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBtnPrintGridServiceReqView(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem itemInPhieuYeuCau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_YEU_CAU", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 1);
                itemInPhieuYeuCau.Tag = PrintTypeBMK.IN_PHIEU_YEU_CAU;
                itemInPhieuYeuCau.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                BarButtonItem itemInPhieuXetNghiemDomSoi = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_XET_NGHIEM_DOM_SOI", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 2);
                itemInPhieuXetNghiemDomSoi.Tag = PrintTypeBMK.PHIEU_XET_NGHIEM_DOM_SOI;
                itemInPhieuXetNghiemDomSoi.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                BarButtonItem itemTaoBieuMauKhac = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_BIEU_MAU_KHAC", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 3);
                itemTaoBieuMauKhac.Tag = PrintTypeBMK.TAO_BIEU_MAU_KHAC;
                itemTaoBieuMauKhac.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                {
                    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemInPhieuXetNghiemDomSoi, itemTaoBieuMauKhac });
                }
                else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                {
                    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemTaoBieuMauKhac });
                }
                else
                {
                    PrintProcess(PrintTypeBMK.IN_PHIEU_YEU_CAU);
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(PrintTypeBMK printType)
        {
            try
            {
                switch (printType)
                {
                    case PrintTypeBMK.IN_PHIEU_YEU_CAU:
                        #region aa
                        if (currentServiceReq != null)
                        {
                            currentSereServs = serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();

                            if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Exam.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Diim.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Fuex.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Suim.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Misu.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Surg.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Other.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Bed.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Reha.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                            };
                        }
                        #endregion
                        break;
                    case PrintTypeBMK.PHIEU_XET_NGHIEM_DOM_SOI:
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP__MPS000027);
                        break;
                    case PrintTypeBMK.TAO_BIEU_MAU_KHAC:
                        if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                        {
                            onClickInKetQuaKhacPlus(null, null);
                        }
                        else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                        {
                            onClickTaoPhieuNoiSoiKhacPlus(null, null);
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInKetQuaKhacPlus(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                dicParamPlus = new Dictionary<string, object>();
                dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                dicImagePlus = new Dictionary<string, Image>();

                string printTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__TEST___PHIEU_XET_NGHIEM_KHAC;
                dicParamPlus = new Dictionary<string, object>();
                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    //EXE.LOGIC.HisSereServ.HisSereServLogic sereServLogic = new LOGIC.HisSereServ.HisSereServLogic();
                    MOS.Filter.HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                    sereServFilter.SERVICE_REQ_ID = this.currentServiceReq.ID;
                    var sereSevs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, new CommonParam());
                    //var sereSevs = sereServLogic.GetView<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>(sereServFilter);
                    string serviceNames = "";
                    foreach (var sereServ in sereSevs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.currentServiceReq, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    var currentPatient = PrintGlobalStore.getPatient(currentServiceReq.TREATMENT_ID);
                    AddKeyIntoDictionaryPrint<PatientADO>(currentPatient, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                    dicParamPlus.Add("BED_ROOM_NAME", WorkPlace.GetRoomName());
                    dicParamPlus.Add("BED_NAME", "");
                    dicParamPlus.Add("INSTRUCTION_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("FINISH_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.FINISH_TIME ?? 0));
                    dicParamPlus.Add("FINISH_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReq.FINISH_TIME ?? 0));
                    dicParamPlus.Add("FINISH_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReq.FINISH_TIME ?? 0));
                    WaitingManager.Hide();
                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Phiếu xét nghiệm", UpdateSereServJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data));
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickTaoPhieuNoiSoiKhacPlus(object sender, EventArgs e)
        {
            try
            {
                dicParamPlus = new Dictionary<string, object>();
                dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                dicImagePlus = new Dictionary<string, Image>();

                string printTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__ENDO___PHIEU_NOI_SOI_DAI_TRANG;

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    string serviceNames = "";
                    foreach (var sereServ in serviceReqComboResultSDO.SereServs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.currentServiceReq, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    var currentPatient = PrintGlobalStore.getPatient(currentServiceReq.TREATMENT_ID);
                    AddKeyIntoDictionaryPrint<PatientADO>(currentPatient, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                    //dicParamPlus.Add("BED_ROOM_NAME", EXE.LOGIC.Token.TokenManager.GetRoomName());
                    //dicParamPlus.Add("BED_NAME", "");
                    //dicParamPlus.Add("INSTRUCTION_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("INSTRUCTION_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("INSTRUCTION_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("FINISH_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.hisServiceReq.FINISH_TIME ?? 0));
                    //dicParamPlus.Add("FINISH_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.hisServiceReq.FINISH_TIME ?? 0));
                    //dicParamPlus.Add("FINISH_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.hisServiceReq.FINISH_TIME ?? 0));
                    WaitingManager.Hide();

                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Phiếu nội soi", UpdateSereServJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> GetListPrintIdByServiceReq()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.currentServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(this.currentServiceReq.JSON_PRINT_ID))
                    {
                        var arrIds = this.currentServiceReq.JSON_PRINT_ID.Split(',', ';');
                        if (arrIds != null && arrIds.Length > 0)
                        {
                            foreach (var id in arrIds)
                            {
                                long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                if (printId > 0)
                                {
                                    result.Add(printId);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool UpdateSereServJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.currentServiceReq != null);
                if (valid)
                {
                    List<FileHolder> listFileHolder = new List<FileHolder>();
                    HIS_SERVICE_REQ hisServiceReq = new HIS_SERVICE_REQ();
                    var listOldPrintIdOfSereServs = GetListPrintIdByServiceReq();
                    ProcessServiceReqExecuteForUpdateJsonPrint(hisServiceReq, listOldPrintIdOfSereServs, sarPrintCreated);
                    SaveTestServiceReq(listFileHolder, hisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private void SaveTestServiceReq(List<FileHolder> listFileHolder, HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                HIS_SERVICE_REQ serviceReqUpdateSDO = new HIS_SERVICE_REQ();
                AutoMapper.Mapper.CreateMap<V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>();
                serviceReqUpdateSDO = AutoMapper.Mapper.Map<V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>(this.currentServiceReq);
                hisServiceReq.ID = this.currentServiceReq.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, hisServiceReq, ProcessLostToken, param);
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                }
                WaitingManager.Hide();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide(); ;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessServiceReqExecuteForUpdateJsonPrint(HIS_SERVICE_REQ hisServiceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.currentServiceReq != null)
                {
                    if (jsonPrintId == null)
                    {
                        jsonPrintId = new List<long>();
                    }
                    jsonPrintId.Add(sarPrintCreated.ID);

                    string printIds = "";
                    foreach (var item in jsonPrintId)
                    {
                        printIds += item.ToString() + ",";
                    }

                    hisServiceReq.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void onClickItemPrint(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var bbtnItem1 = e.Item as BarButtonItem;
                PrintTypeBMK type = (PrintTypeBMK)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ButtonEditPrintDomSoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (barManager2 == null)
                {
                    barManager2 = new DevExpress.XtraBars.BarManager();

                }
                barManager2.Form = this;
                currentServiceReq = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetFocusedRow();
                LoadBtnPrintGridServiceReqView(barManager2);
                //if (lstHisServiceReqSDOResult == null || lstHisServiceReqSDOResult.Count <= 0)
                //{
                //    return;
                //}

                //hisServiceReq = (EXE.ADO.HisServiceReqCombo)gridViewServiceReqView__TabService.GetFocusedRow();

                ////In phieu xet nghiem dom soi
                //InPhieuXetNghiemDomSoi();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
