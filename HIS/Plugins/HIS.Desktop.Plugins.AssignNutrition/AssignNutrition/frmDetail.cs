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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
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
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
    public partial class frmDetail : HIS.Desktop.Utility.FormBase
    {
        HisServiceReqListResultSDO serviceReqComboResultSDO;
        const string commonString__true = "1";
        Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> currentSereServs;
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        PopupMenu menu;
        Dictionary<string, object> dicParamPlus { get; set; }
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus { get; set; }
        Dictionary<string, System.Drawing.Image> dicImagePlus { get; set; }


        public frmDetail(HisServiceReqListResultSDO serviceReqComboResultSDO,
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter,
            HisTreatmentWithPatientTypeInfoSDO _currentHisTreatment, Inventec.Desktop.Common.Modules.Module _currentModule)
        {
            InitializeComponent();
            this.serviceReqComboResultSDO = serviceReqComboResultSDO;
            this.currentHisPatientTypeAlter = currentHisPatientTypeAlter;
            this.currentHisTreatment = _currentHisTreatment;
            this.currentModule = _currentModule;
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            SetCaptionByLanguageKey();


            //isLoad = false;
            if (this.serviceReqComboResultSDO != null)
            {
                gridControlServiceReqView.DataSource = this.serviceReqComboResultSDO.ServiceReqs;
                //LayDuLieuChungInCacPhieuChiDinh();
                gridViewServiceReqView__TabService.SelectAll();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                //Resources.ResourceLanguageManager.LanguageResource__frmDetail = new ResourceManager("HIS.Desktop.Plugins.AssignNutrition.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignNutrition.AssignNutrition.frmDetail).Assembly);

                //////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.grcStt__TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcStt__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.repositoryItemButtonTabServiceViewReq_Print.NullText = Inventec.Common.Resource.Get.Value("frmDetail.repositoryItemButtonTabServiceViewReq_Print.NullText", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcServiceReqCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcServiceReqTypeName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqTypeName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcIntructionTime_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcIntructionTime_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcExecuteRoomName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcExecuteRoomName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcCreator_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcCreator_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcTotalAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.grcTotalPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmDetail.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
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
                        //if (data.SERVICE_REQ_TYPE_ID == (serviceReqType__Test != null ? serviceReqType__Test.ID : 0))
                        //{
                        //    e.RepositoryItem = ButtonEditPrintDomSoi;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessingPrintV2(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                switch (printType)
                {
                    case "Mps000275":
                        richEditorMain.RunPrintTemplate("Mps000275", DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000275":
                        Mps000275(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000275(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.serviceReqComboResultSDO != null)
                {
                    var sereServs = this.serviceReqComboResultSDO.SereServs!=null && this.serviceReqComboResultSDO.SereServs.Count > 0 ?  this.serviceReqComboResultSDO.SereServs.Where(p => p.SERVICE_REQ_ID == this.currentServiceReq.ID).ToList() : null;
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(((this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0) ? this.serviceReqComboResultSDO.ServiceReqs.First().TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    MPS.Processor.Mps000275.PDO.Mps000275PDO mps000275PDO = new MPS.Processor.Mps000275.PDO.Mps000275PDO
           (
            this.currentServiceReq,
            sereServs
             );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBtnPrintGridServiceReqView(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                //BarButtonItem itemInPhieuYeuCau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_YEU_CAU", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 1);
                //itemInPhieuYeuCau.Tag = PrintTypeBMK.IN_PHIEU_YEU_CAU;
                //itemInPhieuYeuCau.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                //BarButtonItem itemInPhieuXetNghiemDomSoi = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_XET_NGHIEM_DOM_SOI", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 2);
                //itemInPhieuXetNghiemDomSoi.Tag = PrintTypeBMK.PHIEU_XET_NGHIEM_DOM_SOI;
                //itemInPhieuXetNghiemDomSoi.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                //BarButtonItem itemTaoBieuMauKhac = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_BIEU_MAU_KHAC", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture()), 3);
                //itemTaoBieuMauKhac.Tag = PrintTypeBMK.TAO_BIEU_MAU_KHAC;
                //itemTaoBieuMauKhac.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                //if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                //{
                //    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemInPhieuXetNghiemDomSoi, itemTaoBieuMauKhac });
                //}
                //else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                //{
                //    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemTaoBieuMauKhac });
                //}
                //else
                //{
                //    PrintProcess(PrintTypeBMK.IN_PHIEU_YEU_CAU);
                //}

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                string printTypeCode = "Mps000231";
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

                    MOS.Filter.HisTreatmentView1Filter treatmentViewFilter = new HisTreatmentView1Filter();
                    treatmentViewFilter.ID = currentServiceReq.TREATMENT_ID;
                    var treatment = new BackendAdapter(null).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, null).FirstOrDefault();

                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_1>(treatment, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("AGE", HIS.Desktop.Print.AgeHelper.CalculateAgeFromYear(treatment.TDL_PATIENT_DOB));
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

                string printTypeCode = "Mps000232";

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    string serviceNames = "";
                    foreach (var sereServ in serviceReqComboResultSDO.SereServs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.currentServiceReq, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    MOS.Filter.HisTreatmentView1Filter treatmentViewFilter = new HisTreatmentView1Filter();
                    treatmentViewFilter.ID = currentServiceReq.TREATMENT_ID;
                    var treatment = new BackendAdapter(null).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, null).FirstOrDefault();

                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_1>(treatment, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("AGE", HIS.Desktop.Print.AgeHelper.CalculateAgeFromYear(treatment.TDL_PATIENT_DOB));
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

        private void ButtonEditPrintDomSoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (barManager2 == null)
                {
                    barManager2 = new DevExpress.XtraBars.BarManager();

                }
                barManager2.Form = this;
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

        private void bbtnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewServiceReqView__TabService.PostEditor();

                List<V_HIS_SERVICE_REQ> ServiceReqSelects = new List<V_HIS_SERVICE_REQ>();

                foreach (var item in gridViewServiceReqView__TabService.GetSelectedRows())
                {
                    currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(item);
                    ProcessingPrintV2("Mps000275");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        this.currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "PrintDetail")
                        {
                            ProcessingPrintV2("Mps000275");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
