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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.KskContractTestResultPrint.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using Inventec.Common.Logging;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;

namespace HIS.Desktop.Plugins.KskContractTestResultPrint
{
    public partial class frmKskContractTestResultPrint : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        public PRINT_OPTION PrintOption { get; set; }
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        List<V_HIS_KSK_CONTRACT> kskContractAll;

        public enum PRINT_OPTION
        {
            IN,
            IN_TACH_THEO_NHOM
        }

        internal enum PrintTypeKXN
        {
            IN_KET_QUA_XET_NGHIEM,
        }

        public frmKskContractTestResultPrint(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void frmMobaDepaCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                LoadKeyFrmLanguage();
                LoadDataToCombo();
                kskContractAll = GetKskContract();
                InitKskContract();
                this.gridControlSample.ToolTipController = this.toolTipControllerGrid;
                SetDefaultControl();
                Search();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                cboKskContract.EditValue = kskContractAll.FirstOrDefault().KSK_CONTRACT_CODE;
                txtContractCode.EditValue = kskContractAll.FirstOrDefault().KSK_CONTRACT_CODE;
                dtIntructionTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.Now;
                dtIntructionTimeTo.DateTime = DateTime.Now;
                LoadDataToCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<ComboADO> status = new List<ComboADO>();
                status.Add(new ComboADO(0, "Tất cả"));
                status.Add(new ComboADO(99, "Chưa có kết quả"));
                status.Add(new ComboADO(100, "Có kết quả"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);

                cboStatus.EditValue = status[2].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_KSK_CONTRACT> GetKskContract()
        {
            List<V_HIS_KSK_CONTRACT> result = null;
            try
            {
                MOS.Filter.HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void InitKskContract()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "KSK_CONTRACT_CODE", columnInfos, false, 150);
                ControlEditorLoader.Load(cboKskContract, kskContractAll, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "INSTRUCTION_TIME__STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_GENDER_NAME")
                        {
                            e.Value = data.GENDER_CODE == "01" ? "Nữ" : (data.GENDER_CODE == "02" ? "Nam" : "Không xác định");
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI || statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    var data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (e.RowHandle < 0)
                    return;
                var data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewSample.DataRowCount; i++)
                {
                    var data = (V_LIS_SAMPLE)gridViewSample.GetRow(i);
                    if (data != null)
                    {

                    }
                }
                gridControlSample.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.PrintOption = PRINT_OPTION.IN;
                PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void PrintProcess(PrintTypeKXN printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeKXN.IN_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, DelegateRunPrinterKXN);
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

        bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096:
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
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

        private long LoadGenderId(V_LIS_SAMPLE rowSample)
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.GENDER_CODE))
                {
                    genderId = rowSample.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = rowSample.PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private void LoadBieuMauInKetQuaXetNghiemV2(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<V_LIS_SAMPLE> sampleSelects = new List<V_LIS_SAMPLE>();
                int[] selectRow = gridViewSample.GetSelectedRows();
                if (selectRow == null || selectRow.Length == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("selectRow null");
                    return;
                }
                foreach (var item in selectRow)
                {
                    var selectItem = (V_LIS_SAMPLE)gridViewSample.GetRow(item);
                    if (selectItem != null)
                    {
                        sampleSelects.Add(selectItem);
                    }
                }
                var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                foreach (var rowSample in sampleSelects)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter ServiceReqViewFilter = new HisServiceReqFilter();
                    ServiceReqViewFilter.SERVICE_REQ_CODE__EXACT = rowSample.SERVICE_REQ_CODE;
                    var currentServiceReq = new HIS_SERVICE_REQ();
                    currentServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();
                    if (currentServiceReq == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong lay duoc ServiceReq" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowSample), rowSample));

                        return;
                    }

                    List<TestLisResultADO> lstHisSereServTeinSDO = new List<TestLisResultADO>();

                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = currentServiceReq.TREATMENT_ID;
                    var curentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                    var currentPatientTypeAlter = new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq.TREATMENT_ID, param);

                    LIS.Filter.LisResultViewFilter resultFilter = new LIS.Filter.LisResultViewFilter();
                    resultFilter.SAMPLE_ID = rowSample.ID;
                    var lstCheckPrint = new BackendAdapter(new CommonParam()).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, null);
                    List<V_HIS_TEST_INDEX> currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                    List<V_LIS_RESULT> lstResultPrint = new List<V_LIS_RESULT>();
                    if (lstCheckPrint != null && lstCheckPrint.Count > 0)
                    {
                        List<string> serviceCodes = lstCheckPrint.Select(o => o.SERVICE_CODE).Distinct().ToList();
                        lstResultPrint = lstCheckPrint.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();

                        currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                        var serviceCodeTests = lstCheckPrint.Select(o => o.SERVICE_CODE).Distinct().ToList();
                        currentTestIndexs = testIndex.Where(o => serviceCodeTests.Contains(o.SERVICE_CODE)).ToList();
                    }

                    WaitingManager.Hide();

                    long genderId = LoadGenderId(rowSample);
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.Logging.LogSystem.Debug("LoadBieuMauInKetQuaXetNghiemV2 rowSample.PATIENT_CODE: " + rowSample.PATIENT_CODE);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((curentTreatment != null ? curentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    if (PrintOption == PRINT_OPTION.IN)
                    {
                        MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                                   currentPatientTypeAlter,
                                   curentTreatment,
                                   rowSample,
                                   currentServiceReq,
                                   currentTestIndexs,
                                   lstResultPrint,
                                   BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                                   genderId,
                                   BackendDataWorker.Get<V_HIS_SERVICE>());

                        MPS.ProcessorBase.Core.PrintData PrintData = null;

                        if (HisConfigCFG.IS_USE_SIGN_EMR == "1")
                        {
                            LogSystem.Info("IS_USE_SIGN_EMR = 1");
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName);
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }

                        PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    else if (PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM)
                    {
                        List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                        if (services == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                            return;
                        }

                        Dictionary<long, List<V_LIS_RESULT>> dicServiceTest = new Dictionary<long, List<V_LIS_RESULT>>();
                        foreach (var item in lstCheckPrint)
                        {
                            long key = 0;
                            V_HIS_SERVICE service = services.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                            if (service != null)
                            {
                                if (service.PARENT_ID.HasValue && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT != null && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT.Contains(service.PARENT_ID.Value))
                                {
                                    key = -1;
                                }
                                else
                                {
                                    key = service.PARENT_ID ?? 0;
                                }
                            }

                            if (!dicServiceTest.ContainsKey(key))
                                dicServiceTest[key] = new List<V_LIS_RESULT>();
                            dicServiceTest[key].Add(item);
                        }

                        foreach (var item in dicServiceTest)
                        {
                            V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.Key);
                            List<V_LIS_RESULT> testLisResults = new List<V_LIS_RESULT>();
                            if (item.Value != null && item.Value.Count > 0)
                            {
                                testLisResults = lstCheckPrint.Where(o => item.Value.Select(p => p.SERVICE_CODE).Contains(o.SERVICE_CODE)).ToList();
                            }

                            MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                                   currentPatientTypeAlter,
                                   curentTreatment,
                                   rowSample,
                                   currentServiceReq,
                                   currentTestIndexs,
                                   testLisResults,
                                   BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                                   genderId,
                                   BackendDataWorker.Get<V_HIS_SERVICE>()
                                   );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;

                            if (HisConfigCFG.IS_USE_SIGN_EMR == "1")
                            {
                                LogSystem.Info("IS_USE_SIGN_EMR = 1");
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName);
                            }
                            else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                            }

                            PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewSample.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                //CommonParam param = new CommonParam();
                //WaitingManager.Show();
                //List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                //List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                //MOS.EFMODEL.DataModels.HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                //if (this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID != null)
                //{
                //    MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                //    hisExpMestMedicineViewFilter.EXP_MEST_ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                //    expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, null);

                //    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                //    expMestMaterialViewFilter.EXP_MEST_ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                //    expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);

                //    MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                //    expMestFilter.ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                //    expMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null).FirstOrDefault();
                //}

                //MPS.Processor.Mps000214.PDO.Mps000214PDO rdo = new MPS.Processor.Mps000214.PDO.Mps000214PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, expMestMedicines, expMestMaterials, expMest);
                //result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                //if (result)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__CAPTION", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Button

                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout

                //grid Control Medicine
                this.gridColumn_LisSample_PatientCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_PATIENT_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_DepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_REQ_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_ExecuteName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_EXECUTE_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_Dob.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_DOB", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_ServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_InstructionTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_INTRUCTION_TIME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_TREATMENT_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_GenderName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_GENDER_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_PatientName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_PATIENT_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_LisSample_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__GRIDVIEW_SAMPLE_STT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciContractCode.Text = Inventec.Common.Resource.Get.Value("lciContractCode.Text", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("btnSearch.Text", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("btnRefresh.Text", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("btnPrint.Text", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrintTachTheoNhom.Text = Inventec.Common.Resource.Get.Value("btnPrintTachTheoNhom.Text", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciContractCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("lciContractCode.ToolTip", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Search()
        {
            try
            {
                if (cboKskContract.EditValue != null)
                {
                    LIS.Filter.LisSampleViewFilter filter = new LIS.Filter.LisSampleViewFilter();
                    filter.KSK_CONTRACT_CODE__EXACT = cboKskContract.EditValue.ToString();
                  
                    if (cboStatus.EditValue != null)
                    {
                        //Chua có kết quả (Chưa lấy mẫu, đã lấy mẫu, từ chối, chấp nhận)
                        if ((long)cboStatus.EditValue == 99)
                        {
                            filter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                        }
                        //Có kết quả (Đã có kết quả, đã duyệt kết quả, đã trả kết quả)
                        else if ((long)cboStatus.EditValue == 100)
                        {
                            filter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                            };
                        }
                        //Tất cả
                        else
                        {
                            filter.SAMPLE_STT_ID = null;
                        }
                    }
                    if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                    }

                    if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("LisSampleViewFilter: ", filter));
                    var samples = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, filter, null);
                    if (samples != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("samples count" + samples.Count());
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("samples null");
                    }
                    gridControlSample.BeginUpdate();
                    gridControlSample.DataSource = samples;
                    gridControlSample.EndUpdate();
                }
                else
                {
                    MessageBox.Show("Chưa chọn hợp đồng khám sức khỏe");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SetDefaultControl();
            Search();
        }

        private void btnPrintTachTheoNhom_Click(object sender, EventArgs e)
        {
            try
            {
                this.PrintOption = PRINT_OPTION.IN_TACH_THEO_NHOM;
                PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        private void txtContractCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadKskContract(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKskContract(string _medicineUseFormCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT> listResult = new List<V_HIS_KSK_CONTRACT>();
                listResult = this.kskContractAll.Where(o => (o.KSK_CONTRACT_CODE != null && o.KSK_CONTRACT_CODE.StartsWith(_medicineUseFormCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineUseForm, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboKskContract.EditValue = listResult[0].KSK_CONTRACT_CODE;
                    txtContractCode.Text = listResult[0].KSK_CONTRACT_CODE;
                }
                else if (listResult.Count > 1)
                {
                    cboKskContract.EditValue = null;
                    cboKskContract.Focus();
                    cboKskContract.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboKskContract.EditValue = null;
                    cboKskContract.Focus();
                    cboKskContract.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboKskContract.EditValue != null)
                    {
                        var medicineUseForm = this.kskContractAll.SingleOrDefault(o => o.KSK_CONTRACT_CODE == ((cboKskContract.EditValue ?? "").ToString()));
                        if (medicineUseForm != null)
                        {
                            txtContractCode.Text = medicineUseForm.KSK_CONTRACT_CODE;
                            btnSearch.Focus();
                        }
                        else
                        {
                            cboKskContract.Focus();
                            cboKskContract.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboKskContract.Text))
                    {
                        string key = cboKskContract.Text.ToLower();
                        var listData = this.kskContractAll.Where(o => o.KSK_CONTRACT_CODE.ToLower().Contains(key) || o.WORK_PLACE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboKskContract.EditValue = listData.First().KSK_CONTRACT_CODE;
                            txtContractCode.Text = listData.First().KSK_CONTRACT_CODE;
                            btnSearch.Focus();
                        }
                    }
                    if (!valid)
                    {
                        cboKskContract.Focus();
                        cboKskContract.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSample)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSample.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {

                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((V_LIS_SAMPLE)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI || busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                                {
                                    text = "Chưa có kết quả";
                                }
                                else
                                {
                                    text = "Đã có kết quả";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
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
