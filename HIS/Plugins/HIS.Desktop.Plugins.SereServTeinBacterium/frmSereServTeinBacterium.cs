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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SereServTeinBacterium.ADO;
using HIS.Desktop.Plugins.SereServTeinBacterium.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SereServTeinBacterium
{
    public partial class frmSereServTeinBacterium : HIS.Desktop.Utility.FormBase
    {
        internal List<V_HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders { get; set; }
        internal List<ADO.HisSereServTeinSDO> lstSereServTein { get; set; }
        internal List<ADO.HisSereServTeinSDO> lstSereServExtNoTein { get; set; }
        internal List<HIS_SERE_SERV_EXT> lstSereServExt { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> currentSereServFiles;
        internal MOS.EFMODEL.DataModels.HIS_SERE_SERV currentSereServ;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt;
        SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        List<ADO.ImageADO> imageLoad;

        const string IN_KET_QUA_KHANG_SINH_DO_PRINT_TYPE_CODE = "Mps000341";

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public frmSereServTeinBacterium(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.HIS_SERE_SERV sereServ)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentSereServ = sereServ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSereServTein_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                InitControlState();
                SetCaptionByLanguageKey();
                if (this.currentSereServ != null && this.currentSereServ.ID > 0)
                {
                    LoadDataBySereServId();
                    LoadDataToGridV2();
                }
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SereServTeinBacterium.Resources.Lang", typeof(HIS.Desktop.Plugins.SereServTeinBacterium.frmSereServTeinBacterium).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModule.ModuleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSign.Name)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private async Task LoadDataToGridV2()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.1");
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                if ((this.currentSereServ.SERVICE_REQ_ID ?? 0) <= 0)
                {
                    MOS.Filter.HisSereServFilter filter1 = new MOS.Filter.HisSereServFilter();
                    filter1.ID = this.currentSereServ.ID;
                    this.currentSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter1, param).FirstOrDefault();
                }

                MOS.Filter.HisSereServViewFilter filter = new MOS.Filter.HisSereServViewFilter();
                filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                filter.ORDER_DIRECTION = "DESC";
                filter.SERVICE_REQ_ID = this.currentSereServ.SERVICE_REQ_ID;
                filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                this.lstSereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.2");
                List<long> sereServIds = new List<long>();
                if (this.lstSereServ != null && this.lstSereServ.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.3");

                    #region sereSerTein
                    sereServIds = this.lstSereServ.Select(p => p.ID).ToList();
                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = sereServIds;
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    sereSerTeinFilter.ORDER_DIRECTION = "DESC";

                    this.lstSereServTein = await new BackendAdapter(param).GetAsync<List<ADO.HisSereServTeinSDO>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);
                    if (this.lstSereServTein != null && this.lstSereServTein.Count > 0)
                    {
                        this.lstSereServTein = this.lstSereServTein.Where(o => !String.IsNullOrWhiteSpace(o.BACTERIUM_CODE) && !String.IsNullOrWhiteSpace(o.ANTIBIOTIC_RESISTANCE_CODE)).ToList();
                    }

                    foreach (var item in this.lstSereServTein)
                    {
                        var checkSereServ = this.lstSereServ.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                        if (checkSereServ != null)
                        {
                            item.SERVICE_CODE = checkSereServ.TDL_SERVICE_CODE;
                            item.SERVICE_NAME = checkSereServ.TDL_SERVICE_NAME;
                        }
                    }
                    #endregion

                    #region sereServExt

                    HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                    sereServExtFilter.SERE_SERV_IDs = sereServIds;
                    sereServExtFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sereServExtFilter.ORDER_FIELD = "NUM_ORDER";
                    sereServExtFilter.ORDER_DIRECTION = "DESC";

                    lstSereServExt = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, null);
                    if (lstSereServExt != null && lstSereServExt.Count() > 0)
                    {
                        this.lstSereServExtNoTein = new List<HisSereServTeinSDO>();
                        var sereServExts = lstSereServExt.Where(o => !lstSereServTein.Exists(p => p.SERE_SERV_ID == o.SERE_SERV_ID)).ToList();
                        foreach (var item in sereServExts)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServExts____", item));
                            var sereServ = this.lstSereServ.Where(o => o.ID == item.SERE_SERV_ID).FirstOrDefault();
                            HisSereServTeinSDO sdo = new HisSereServTeinSDO(item, sereServ);
                            lstSereServExtNoTein.Add(sdo);
                        }


                        // this.lstSereServExt = this.lstSereServExt.Where(o => !String.IsNullOrWhiteSpace(o.BACTERIUM_CODE) && !String.IsNullOrWhiteSpace(o.ANTIBIOTIC_RESISTANCE_CODE)).ToList();
                    }

                    #endregion

                    List<ADO.HisSereServTeinSDO> dataSouce = new List<ADO.HisSereServTeinSDO>();
                    if (lstSereServTein != null && lstSereServTein.Count() > 0)
                    {
                        var GroupServices = this.lstSereServTein.GroupBy(o => o.SERVICE_CODE).ToList();
                        foreach (var groupService in GroupServices)
                        {
                            ADO.HisSereServTeinSDO parentService = new ADO.HisSereServTeinSDO();
                            parentService.PARENT_ID = groupService.FirstOrDefault().ID.ToString();
                            parentService.CHILD_ID = groupService.FirstOrDefault().SERVICE_CODE + "." + groupService.FirstOrDefault().ID;
                            parentService.TEST_INDEX_NAME = groupService.FirstOrDefault().SERVICE_NAME;
                            parentService.TEST_INDEX_CODE = groupService.FirstOrDefault().SERVICE_CODE;
                            if (this.lstSereServExt != null && this.lstSereServExt.Count() > 0)
                            {
                                var sereServExt = this.lstSereServExt.Where(o => o.SERE_SERV_ID == groupService.FirstOrDefault().SERE_SERV_ID);
                                if (sereServExt != null)
                                {
                                    parentService.MICROCOPY_RESULT = sereServExt.FirstOrDefault().MICROCOPY_RESULT;
                                    parentService.IMPLANTION_RESULT = sereServExt.FirstOrDefault().IMPLANTION_RESULT;
                                    parentService.CONCLUDE = sereServExt.FirstOrDefault().CONCLUDE;
                                }

                            }
                            parentService.IS_PARENT = 1;
                            dataSouce.Add(parentService);

                            var groupBacterium = groupService.GroupBy(o => o.BACTERIUM_CODE).ToList();
                            foreach (var BacteriumGrp in groupBacterium)
                            {
                                ADO.HisSereServTeinSDO parentBacterium = new ADO.HisSereServTeinSDO();
                                parentBacterium.PARENT_ID = parentService.CHILD_ID;
                                parentBacterium.CHILD_ID = BacteriumGrp.FirstOrDefault().BACTERIUM_CODE + "." + BacteriumGrp.FirstOrDefault().ID + ".";
                                parentBacterium.TEST_INDEX_NAME = BacteriumGrp.FirstOrDefault().BACTERIUM_NAME;
                                parentBacterium.TEST_INDEX_CODE = BacteriumGrp.FirstOrDefault().BACTERIUM_CODE;
                                parentBacterium.IS_PARENT = 1;
                                parentBacterium.BACTERIUM_AMOUNT_DENSITY = BacteriumGrp.FirstOrDefault().BACTERIUM_AMOUNT ?? BacteriumGrp.FirstOrDefault().BACTERIUM_DENSITY;
                                parentBacterium.BACTERIUM_NOTE = BacteriumGrp.FirstOrDefault().BACTERIUM_NOTE;

                                dataSouce.Add(parentBacterium);

                                foreach (var item in BacteriumGrp)
                                {
                                    ADO.HisSereServTeinSDO ChildBacterium = new ADO.HisSereServTeinSDO();
                                    ChildBacterium.CHILD_ID = item.BACTERIUM_CODE + "." + item.ID + "." + item.SERVICE_CODE + ".";
                                    ChildBacterium.PARENT_ID = parentBacterium.CHILD_ID;
                                    ChildBacterium.TEST_INDEX_NAME = item.ANTIBIOTIC_RESISTANCE_NAME;
                                    ChildBacterium.TEST_INDEX_CODE = item.ANTIBIOTIC_RESISTANCE_CODE;
                                    ChildBacterium.VALUE = item.VALUE;
                                    ChildBacterium.SRI_CODE = item.SRI_CODE;
                                    dataSouce.Add(ChildBacterium);
                                }
                            }
                        }
                    }


                    if (lstSereServExtNoTein != null && lstSereServExtNoTein.Count() > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstSereServExtNoTein", lstSereServExtNoTein));
                        var GroupServicesBySereServExt = this.lstSereServExtNoTein.GroupBy(o => o.SERVICE_CODE).ToList();
                        foreach (var groupServiceBySereServExt in GroupServicesBySereServExt)
                        {
                            ADO.HisSereServTeinSDO parentService = new ADO.HisSereServTeinSDO();
                            parentService.PARENT_ID = groupServiceBySereServExt.FirstOrDefault().ID.ToString();
                            parentService.CHILD_ID = groupServiceBySereServExt.FirstOrDefault().SERVICE_CODE + "." + groupServiceBySereServExt.FirstOrDefault().ID;
                            parentService.TEST_INDEX_NAME = groupServiceBySereServExt.FirstOrDefault().SERVICE_NAME;
                            parentService.TEST_INDEX_CODE = groupServiceBySereServExt.FirstOrDefault().SERVICE_CODE;
                            parentService.MICROCOPY_RESULT = groupServiceBySereServExt.FirstOrDefault().MICROCOPY_RESULT;
                            parentService.IMPLANTION_RESULT = groupServiceBySereServExt.FirstOrDefault().IMPLANTION_RESULT;
                            parentService.CONCLUDE = groupServiceBySereServExt.FirstOrDefault().CONCLUDE;
                            parentService.IS_PARENT = 1;
                            dataSouce.Add(parentService);
                        }
                    }


                    // treeList
                    var records = new BindingList<ADO.HisSereServTeinSDO>(dataSouce);
                    this.treeList1.RefreshDataSource();
                    this.treeList1.DataSource = null;
                    this.treeList1.DataSource = records;
                    this.treeList1.KeyFieldName = "CHILD_ID";
                    this.treeList1.ParentFieldName = "PARENT_ID";
                    this.treeList1.ExpandAll();
                }

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        long GetServiceNumOrder(V_HIS_SERE_SERV sereServNumOder)
        {
            try
            {
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == sereServNumOder.SERVICE_ID).FirstOrDefault();
                return (service != null ? (service.NUM_ORDER ?? 0) : 99999);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintTypeTest.IN_KET_QUA_KHANG_SINH_DO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintTypeTest
        {
            IN_KET_QUA_KHANG_SINH_DO,
            IN_PHIEU_KET_QUA
        }

        void PrintProcess(PrintTypeTest printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeTest.IN_KET_QUA_KHANG_SINH_DO:
                        richEditorMain.RunPrintTemplate(IN_KET_QUA_KHANG_SINH_DO_PRINT_TYPE_CODE, DelegateRunPrinterTest);
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

        bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case IN_KET_QUA_KHANG_SINH_DO_PRINT_TYPE_CODE:
                        LoadBieuMauInKetQuaKhangSinhDo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA__MPS000015:
                        LoadBieuMauPhieuYCInKetQua(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauInKetQuaKhangSinhDo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    LIS.Filter.LisSampleViewFilter sampleFilter = new LIS.Filter.LisSampleViewFilter();
                    sampleFilter.SERVICE_REQ_CODE__EXACT = lstSereServ.FirstOrDefault().TDL_SERVICE_REQ_CODE;
                    var samples = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, sampleFilter, null);
                    List<V_LIS_SAMPLE_SERVICE> vsampleServices = null;
                    V_LIS_SAMPLE sample = new V_LIS_SAMPLE();
                    List<V_LIS_RESULT> lisResult = new List<V_LIS_RESULT>();
                    if (samples != null && samples.Count() > 0)
                    {
                        sample = samples.FirstOrDefault();
                        LIS.Filter.LisSampleServiceViewFilter sampleServiceFilter = new LIS.Filter.LisSampleServiceViewFilter();
                        sampleServiceFilter.SAMPLE_ID = sample.ID;
                        sampleServiceFilter.SERVICE_REQ_CODE__EXACT = lstSereServ.FirstOrDefault().TDL_SERVICE_REQ_CODE;
                        sampleServiceFilter.SERVICE_CODE__EXACT = lstSereServ.FirstOrDefault().TDL_SERVICE_CODE;
                        vsampleServices = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE_SERVICE>>("api/LisSampleService/GetView", ApiConsumer.ApiConsumers.LisConsumer, sampleServiceFilter, null);

                        LIS.Filter.LisResultViewFilter resultFilter = new LIS.Filter.LisResultViewFilter();
                        resultFilter.SAMPLE_ID = samples.FirstOrDefault().ID; 
                        lisResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);
                    }

                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.ID = lstSereServ.FirstOrDefault().SERVICE_REQ_ID;
                    var ServiceReqPrint = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                    List<LIS_PATIENT_CONDITION> lstpatientCondition;
                    CommonParam paramCommonCondition = new CommonParam();
                    LisPatientConditionFilter conditionFilter = new LisPatientConditionFilter();
                    lstpatientCondition = new BackendAdapter(paramCommonCondition).Get<List<LIS_PATIENT_CONDITION>>("api/LisPatientCondition/Get", ApiConsumer.ApiConsumers.LisConsumer, conditionFilter, paramCommonCondition);

                    List<HIS_TREATMENT> treatmentApiResult = null;
                    CommonParam paramCo = new CommonParam();
                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.TREATMENT_CODE__EXACT = lstSereServ.FirstOrDefault().TDL_TREATMENT_CODE;
                    treatmentFilter.ORDER_DIRECTION = "DESC";
                    treatmentFilter.ORDER_FIELD = "MODIFY_TIME";
                    treatmentApiResult = new BackendAdapter(paramCo).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, paramCo);
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    if (treatmentApiResult != null && treatmentApiResult.Count > 0)
                    {
                        treatment = treatmentApiResult.FirstOrDefault();
                    }

                    HIS_PATIENT patient = null;
                    if (treatment != null)
                    {
                        HisPatientFilter patiFilter = new HisPatientFilter();
                        patiFilter.ID = treatment.PATIENT_ID;
                        var lstPatient = new BackendAdapter(paramCo).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, patiFilter, paramCo);
                        patient = lstPatient != null ? lstPatient.FirstOrDefault() : null;
                    }

                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((lstSereServ != null ? lstSereServ.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    LIS_SAMPLE_SERVICE sampleService = new LIS_SAMPLE_SERVICE();
                    if (vsampleServices != null && vsampleServices.Count() > 0)
                    {
                        var vsampleService = vsampleServices.FirstOrDefault();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE_SERVICE>(sampleService, vsampleService);
                    }

                    LIS_MACHINE machine = new LIS_MACHINE();
                    if(sampleService !=null && sampleService.MACHINE_ID.HasValue)
                    {
                        machine = BackendDataWorker.Get<LIS_MACHINE>().FirstOrDefault(o => o.ID == sampleService.MACHINE_ID);
                    }

                    MPS.Processor.Mps000341.PDO.Mps000341PDO mps000341RDO = new MPS.Processor.Mps000341.PDO.Mps000341PDO(
                               sample,
                               sampleService,
                               machine,
                               lisResult,
                               ServiceReqPrint,
                               treatment,
                               patient,
                               lstpatientCondition);

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (chkSign.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO };
                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                    //PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
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
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //if (xtraTabControl1.SelectedTabPageIndex == 0)
                //{
                btnPrint_Click(null, null);
                //}
                //else
                //{
                btnPrintServiceReq_Click(null, null);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SereServClickRow(V_HIS_SERE_SERV_4 sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    WaitingManager.Show();
                    sereServExt = GetSereServExtBySereServId(sereServ.ID);
                    WaitingManager.Hide();
                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        bool isSense = false;
                        if (!String.IsNullOrWhiteSpace(sereServExt.JSON_PRINT_ID))
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(sereServExt.JSON_PRINT_ID) && sereServExt.JSON_PRINT_ID.Contains("studyID"))
                                {
                                    var studyID = sereServExt.JSON_PRINT_ID.Split(':');
                                    if (studyID != null && studyID.Count() == 2)
                                    {
                                        if (PacsCFG.PACS_ADDRESS != null && PacsCFG.PACS_ADDRESS.Count > 0)
                                        {
                                            var address = PacsCFG.PACS_ADDRESS.FirstOrDefault(o => o.RoomCode == sereServ.EXECUTE_ROOM_CODE);
                                            if (address != null)
                                            {
                                                HIS_PATIENT patient = GetPatientById(sereServ.TDL_PATIENT_ID);
                                                //string url = string.Format("http://{0}:{1}/rpacsSENSE/pacsSENSE?patientID={2}&studyUID={3}", address.Address, "8080", patient.PATIENT_CODE, studyID[1]);
                                                string url = string.Format("http://{0}?patientID={1}&studyUID={2}", address.Address, patient.PATIENT_CODE, studyID[1]);
                                                Inventec.Common.Logging.LogSystem.Info("url: " + url);
                                                System.Diagnostics.Process.Start(url);

                                                isSense = true;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isSense = false;
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        if (isSense)
                        {
                            this.Close();
                        }
                        else
                        {
                            //this.ActionType = GlobalVariables.ActionEdit;
                            ProcessLoadSereServExtDescriptionPrint(sereServExt);
                        }
                    }
                    else
                    {
                        //this.ActionType = GlobalVariables.ActionAdd;
                    }
                    //ProcessLoadSereServExt(sereServ, ref sereServExt);
                    //ProcessLoadSereServFile(sereServ);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetPatientById(long? patientId)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                if (patientId.HasValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                    filter.ID = patientId;
                    var rs = new Inventec.Common.Adapter.BackendAdapter
                           (paramCommon).Get<List<HIS_PATIENT>>
                           (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                    if (rs != null && rs.Count > 0)
                    {
                        result = rs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_PATIENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT GetSereServExtBySereServId(long sereServId)
        {
            MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                filter.SERE_SERV_ID = sereServId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessLoadSereServFile(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_4 sereServ)
        {
            try
            {
                currentSereServFiles = GetSereServFilesBySereServId(sereServ.ID);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        imageLoad = new List<ADO.ImageADO>();
                        if (stream != null && stream.Length > 0)
                        {
                            ADO.ImageADO tileNew = new ADO.ImageADO();
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = true;
                            tileNew.IMAGE_DISPLAY = Image.FromStream(stream);
                            imageLoad.Add(tileNew);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBySereServId()
        {
            try
            {
                if (this.currentSereServ == null) throw new ArgumentNullException("currentSereServ is null");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServView4Filter filter = new MOS.Filter.HisSereServView4Filter();
                filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                filter.ORDER_DIRECTION = "DESC";
                filter.ID = this.currentSereServ.ID;
                var rs = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_4>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    SereServClickRow(rs[0]);
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(long sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERE_SERV_ID = sereServId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("/api/HisSereServFile/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessLoadSereServExtDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (sereServExt != null && sereServExt.ID > 0)
                {
                    currentSarPrint = GetListPrintByDescriptionPrint(sereServExt);
                    if (currentSarPrint != null && currentSarPrint.ID > 0)
                    {
                        btnPrint.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<long> GetListPrintIdBySereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                {
                    var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadBieuMauPhieuYCInKetQua(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.ID = this.currentSereServ.ID;
                List<HIS_SERE_SERV> _SereServs = new List<HIS_SERE_SERV>();
                _SereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>
                   (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param);
                long _treatmentId = 0;
                long _serviceReqId = 0;
                List<MPS.Processor.Mps000015.PDO.Mps000015ADO> _Mps000015ADOs = new List<MPS.Processor.Mps000015.PDO.Mps000015ADO>();
                if (_SereServs != null && _SereServs.Count > 0)
                {
                    _treatmentId = _SereServs[0].TDL_TREATMENT_ID ?? 0;
                    _serviceReqId = _SereServs[0].SERVICE_REQ_ID ?? 0;

                    //_Mps000015ADOs.AddRange((from r in _SereServs select new MPS.Processor.Mps000015.PDO.Mps000015ADO(r, txtNote.Text, txtConclude.Text)).ToList());

                }

                //Lấy thông tin thẻ BHYT
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = _treatmentId;
                hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                var PatyAlterBhyt = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, param);

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = _serviceReqId;
                var ServiceReqPrint = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                MPS.Processor.Mps000015.PDO.SingleKeys _SingleKeys = new MPS.Processor.Mps000015.PDO.SingleKeys();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                if (PatyAlterBhyt != null)
                {
                    string levelCode = PatyAlterBhyt.LEVEL_CODE;
                    ratio_text = GetDefaultHeinRatioForView(PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, levelCode, PatyAlterBhyt.RIGHT_ROUTE_CODE);
                }

                _SingleKeys.Ratio = ratio_text;

                MOS.Filter.HisTreatmentBedRoomFilter bedRoomFilter = new HisTreatmentBedRoomFilter();
                bedRoomFilter.TREATMENT_ID = _treatmentId;
                var _TreatmentBedRoom = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("/api/HisTreatmentBedRoom/Get", ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();


                if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ID > 0)
                {
                    var bedName = BackendDataWorker.Get<HIS_BED>().FirstOrDefault(p => p.ID == _TreatmentBedRoom.BED_ID);
                    _SingleKeys.BED_NAME = bedName != null ? bedName.BED_NAME : null;
                }
                if (ServiceReqPrint != null)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_DEPARTMENT_ID);
                    _SingleKeys.REQUEST_DEPARTMENT_NAME = depart != null ? depart.DEPARTMENT_NAME : null;

                    var roomName = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_ROOM_ID);
                    _SingleKeys.REQUEST_ROOM_NAME = roomName != null ? roomName.ROOM_NAME : null;
                }
                if (_SereServs != null && _SereServs[0].TDL_SERVICE_TYPE_ID > 0)
                {
                    var typeName = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == _SereServs[0].TDL_SERVICE_TYPE_ID);
                    _SingleKeys.SERVICE_TYPE_NAME = typeName != null ? typeName.SERVICE_TYPE_NAME : null;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ServiceReqPrint != null ? ServiceReqPrint.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000015.PDO.Mps000015PDO mps000015RDO = new MPS.Processor.Mps000015.PDO.Mps000015PDO(
                    PatyAlterBhyt,
                    ServiceReqPrint,
                    _Mps000015ADOs,
                    _SingleKeys
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = this.currentSereServ.SERVICE_REQ_ID;
                var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                long? finishTime = null;
                if (lstServiceReq != null && lstServiceReq.Count > 0)
                {
                    finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                }
                var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
                //printDocument.RtfText = txtDescription.RtfText;
                var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");
                if (!String.IsNullOrWhiteSpace(tgkt))
                {
                    foreach (var section in printDocument.Document.Sections)
                    {
                        if (HideTimePrint != "1")
                        {
                            section.Margins.HeaderOffset = 50;
                            section.Margins.FooterOffset = 50;
                            var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                            //xóa header nếu có dữ liệu
                            myHeader.Delete(myHeader.Range);

                            myHeader.InsertText(myHeader.CreatePosition(0),
                                String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
                                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                            myHeader.Fields.Update();
                            section.EndUpdateHeader(myHeader);
                        }

                        string finishTimeStr = "";

                        if (finishTime.HasValue)
                        {
                            finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                        }

                        var rangeSeperators = printDocument.Document.FindAll(
                            tgkt,
                            DevExpress.XtraRichEdit.API.Native.SearchOptions.None);

                        if (rangeSeperators != null && rangeSeperators.Length > 0)
                        {
                            for (int i = 0; i < rangeSeperators.Length; i++)
                                printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
                        }
                    }

                }

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printDocument.Print();
                }
                else
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void BtnEmr_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtDescription.Text != "" && this.currentSereServ.SERVICE_REQ_ID.HasValue)
        //        {
        //            MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
        //            filter.ID = this.currentSereServ.SERVICE_REQ_ID;
        //            var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
        //            long? finishTime = null;
        //            if (lstServiceReq != null && lstServiceReq.Count > 0)
        //            {
        //                finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
        //            }
        //            var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
        //            printDocument.RtfText = txtDescription.RtfText;
        //            var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
        //            string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");
        //            if (!String.IsNullOrWhiteSpace(tgkt))
        //            {
        //                foreach (var section in printDocument.Document.Sections)
        //                {
        //                    if (HideTimePrint != "1")
        //                    {
        //                        section.Margins.HeaderOffset = 50;
        //                        section.Margins.FooterOffset = 50;
        //                        var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
        //                        //xóa header nếu có dữ liệu
        //                        myHeader.Delete(myHeader.Range);

        //                        myHeader.InsertText(myHeader.CreatePosition(0),
        //                            String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
        //                            Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
        //                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
        //                        myHeader.Fields.Update();
        //                        section.EndUpdateHeader(myHeader);
        //                    }

        //                    string finishTimeStr = "";

        //                    if (finishTime.HasValue)
        //                    {
        //                        finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
        //                    }

        //                    var rangeSeperators = printDocument.Document.FindAll(
        //                        tgkt,
        //                        DevExpress.XtraRichEdit.API.Native.SearchOptions.None);

        //                    if (rangeSeperators != null && rangeSeperators.Length > 0)
        //                    {
        //                        for (int i = 0; i < rangeSeperators.Length; i++)
        //                            printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
        //                    }
        //                }
        //            }

        //            SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

        //            SignType type = new SignType();
        //            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "1")
        //            {
        //                type = SignType.USB;
        //            }
        //            else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "2")
        //            {
        //                type = SignType.HMS;
        //            }

        //            InputADO inputADO = new InputADO(null, false, null, type);
        //            inputADO.DTI = ConfigSystems.URI_API_ACS + "|" + ConfigSystems.URI_API_EMR + "|" + ConfigSystems.URI_API_FSS;
        //            inputADO.IsSave = false;
        //            inputADO.IsSign = true;//set true nếu cần gọi ký
        //            inputADO.IsReject = true;
        //            inputADO.IsPrint = false;
        //            inputADO.IsExport = false;
        //            inputADO.DlgOpenModuleConfig = OpenSignConfig;

        //            String temFile = System.IO.Path.GetTempFileName();
        //            temFile = temFile.Replace(".tmp", ".pdf");
        //            printDocument.ExportToPdf(temFile);

        //            libraryProcessor.ShowPopup(temFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...

        //            System.IO.File.Delete(temFile);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void OpenSignConfig(EMR.TDO.DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", currentModule.RoomId, currentModule.RoomTypeId, _listObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (ADO.HisSereServTeinSDO)this.treeList1.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (data.IS_PARENT == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
