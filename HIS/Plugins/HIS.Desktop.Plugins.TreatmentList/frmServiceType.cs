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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using LIS.Filter;
using System.Threading;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class frmServiceType : HIS.Desktop.Utility.FormBase
    {
        List<long> lstTreatmentId = new List<long>();
        List<long> listIdServiceReqType = new List<long>();
        List<long> listUnSelected = new List<long>();
        List<HIS_SERE_SERV_EXT> dicSereServExtData = new List<HIS_SERE_SERV_EXT>();
        List<LIS.EFMODEL.DataModels.V_LIS_SAMPLE> LisSamples;
        private long CountSereServPrinted { get; set; }
        private bool CancelPrint { get; set; }
        HIS_WORK_PLACE _WORK_PLACE = new HIS_WORK_PLACE();
        ADO.ChiDinhDichVuADO chiDinhDichVuADO = new ADO.ChiDinhDichVuADO();
        Inventec.Desktop.Common.Modules.Module currentModule;
        private const int TIME_OUT_PRINT_MERGE = 1200;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        private long TotalSereServPrint = 0;
        private long totalSereServ26 = 0;
        private long totalSereServ451 = 0;
        private long totalSereServ461 = 0;
        private long treatmentToPrint = 0;
        private string currentTypeCode26 = "";
        private string currentFileName26 = "";
        private string currentTypeCode451 = "";
        private string currentFileName451 = "";
        private string currentTypeCode461 = "";
        private string currentFileName461 = "";
        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
        List<V_HIS_TREATMENT_4> listTreatment;

        bool isHasMps26 = false;
        public frmServiceType(Inventec.Desktop.Common.Modules.Module moduleData, List<long> _ListtreatmentID, List<V_HIS_TREATMENT_4> lstTreatment)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.listTreatment = lstTreatment;
                this.currentModule = moduleData;
                this.lstTreatmentId = _ListtreatmentID;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceType_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().ToList();
                gridControl1.EndUpdate();
                gridView1.SelectAll();
                btnPrint.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMps000026(string printTypeCode, string fileName, long treatmentId)
        {
            try
            {
                var checkServiceReq = GetServiceReq(treatmentId);
                if (checkServiceReq != null && checkServiceReq.Count > 0
                    && checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList() != null
                    && checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = GetSereServ(treatmentId, checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                    HisServiceReqSDO.ServiceReqs = checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                    HisServiceReqSDO.SereServBills = GetSereServBills(treatmentId, checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                    HisServiceReqSDO.SereServDeposits = GetSereServDeposits(treatmentId, HisServiceReqSDO.SereServs.Select(o => o.ID).ToList());
                    List<V_HIS_BED_LOG> listBedLogs = GetBedLog(treatmentId);
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
                    HisTreatmentFilter ft = new HisTreatmentFilter();
                    ft.ID = treatmentId;
                    var currentTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, ft, param);

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, currentTreatment);

                    if (patientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_ADDRESS = patientTypeAlter.ADDRESS;
                    }


                    chiDinhDichVuADO.treament = currentTreatment.FirstOrDefault();
                    // chiDinhDichVuADO.FirstExamRoomName = room != null ? room.ROOM_NAME : "";
                    chiDinhDichVuADO.ListSereServBill = HisServiceReqSDO.SereServBills;
                    chiDinhDichVuADO.ListSereServDeposit = HisServiceReqSDO.SereServDeposits;
                    chiDinhDichVuADO.patientTypeAlter = patientTypeAlter;

                    ProcessGetLisSample(HisServiceReqSDO.ServiceReqs);
                    dicSereServExtData = GetDicSereServExt(HisServiceReqSDO.SereServs);
                    this.totalSereServ26 += HisServiceReqSDO.SereServs.Count();


                    Mps000026Print(printTypeCode, fileName, chiDinhDichVuADO, HisServiceReqSDO.ServiceReqs, HisServiceReqSDO.SereServs, dicSereServExtData, listBedLogs);

                    //var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                    //WaitingManager.Hide();
                    //PrintServiceReqProcessor.Print(printTypeCode, false);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMps000461(string printTypeCode, string fileName, long treatmentId)
        {
            try
            {
                var checkServiceReq = GetServiceReq(treatmentId);
                if (checkServiceReq == null || checkServiceReq.Count() == 0)
                    return;

                var listServiceReq_XN = checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                if (listServiceReq_XN == null || listServiceReq_XN.Count() == 0)
                    return;

                CommonParam param = new CommonParam();
                var listSereServ = GetSereServ(treatmentId, listServiceReq_XN.Select(o => o.ID).ToList());

                HisTreatmentFilter ft = new HisTreatmentFilter();
                ft.ID = treatmentId;
                var currentTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, ft, param).FirstOrDefault();

                this.totalSereServ461 += listSereServ.Count();

                Mps000461Print(printTypeCode, fileName, currentTreatment, listServiceReq_XN, listSereServ);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ProcessGetLisSample(List<V_HIS_SERVICE_REQ> list)
        {
            try
            {
                if (list != null && list.Count > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    LisSampleViewFilter filter = new LisSampleViewFilter();
                    filter.SERVICE_REQ_CODEs = list.Select(s => s.SERVICE_REQ_CODE).Distinct().ToList();
                    this.LisSamples = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS.EFMODEL.DataModels.V_LIS_SAMPLE>>("api/LisSample/GetView",
                        ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_SERE_SERV_EXT> GetDicSereServExt(List<V_HIS_SERE_SERV> list)
        {
            List<HIS_SERE_SERV_EXT> result = new List<HIS_SERE_SERV_EXT>();
            try
            {
                if (list != null && list.Count > 0)
                {
                    List<HIS_SERE_SERV_EXT> listSereServExt = new List<HIS_SERE_SERV_EXT>();
                    var listssId = list.Select(o => o.ID).ToList();

                    CommonParam paramCommon = new CommonParam();
                    var skip = 0;
                    while (listssId.Count - skip > 0)
                    {
                        var limit = listssId.Skip(skip).Take(100).ToList();
                        skip = skip + 100;
                        MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                        filter.SERE_SERV_IDs = limit;
                        var sereServExt = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        listSereServExt.AddRange(sereServExt);
                    }

                    if (listSereServExt != null && listSereServExt.Count > 0)
                    {
                        result = listSereServExt;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<HIS_SERE_SERV_EXT>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void PrintMps000451(string printTypeCode, string fileName, ref bool result, long treatmentId)
        {
            try
            {
                WaitingManager.Show();
                var checkServiceReq = GetServiceReq(treatmentId);
              
                if (checkServiceReq != null && checkServiceReq.Count > 0
                    && checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList() != null
                    && checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Count > 0)
                {
                    checkServiceReq = checkServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                    List<V_HIS_SERVICE_REQ> lstTemp = new List<V_HIS_SERVICE_REQ>();
                    checkServiceReq = checkServiceReq.Where(o => listIdServiceReqType.Exists(p => p == o.SERVICE_REQ_TYPE_ID)).ToList();
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var KskRank = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    var lstSereServ = GetSereServ(treatmentId, checkServiceReq.Select(o => o.ID).ToList());
                    MPS.Processor.Mps000451.PDO.Mps000451PDO mps000451RDO = new MPS.Processor.Mps000451.PDO.Mps000451PDO(
                    currentTreatment.FirstOrDefault(),
                    checkServiceReq,
                    lstSereServ
                    );
                    WaitingManager.Hide();
                    this.totalSereServ451 += lstSereServ.Count();
                    Print.PrintData(printTypeCode, fileName, mps000451RDO, lstSereServ.Count(), null, SetDataGroup);                  
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintMergeXN_Click(object sender, EventArgs e)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Info("btnPrintMergeXN_Click.Begin()!");
                listIdServiceReqType = new List<long>();
                var rowSelected = gridView1.GetSelectedRows();

                DisposeMemoryStream(this.GroupStreamPrint);
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);

                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    foreach (var i in rowSelected)
                    {
                        var row = (HIS_SERVICE_REQ_TYPE)gridView1.GetRow(i);
                        listIdServiceReqType.Add(row.ID);
                    }
                    if (listIdServiceReqType != null && listIdServiceReqType.Count > 0)
                    {
                        foreach (var item in lstTreatmentId)
                        {
                            this.treatmentToPrint = item;

                            if (listIdServiceReqType.Contains(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN))
                            {
                                if (!string.IsNullOrEmpty(currentFileName461))
                                {
                                    PrintMps000461(this.currentTypeCode461, currentFileName461, item);
                                }
                                else
                                {
                                    richEditorMain.RunPrintTemplate("Mps000461", DelegateRunPrinter);
                                }
                            }

                            if (listIdServiceReqType.Where(o => o != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList() != null &&
                                listIdServiceReqType.Where(o => o != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Count > 0)
                            {
                                bool result = false;
                                if (!string.IsNullOrEmpty(currentFileName451))
                                {
                                    PrintMps000451(this.currentTypeCode451, currentFileName451, ref result, item);
                                }
                                else
                                {
                                    richEditorMain.RunPrintTemplate("Mps000451", DelegateRunPrinter);
                                }
                            }
                        }
                    }
                }
                this.TotalSereServPrint = totalSereServ461 + totalSereServ451;
                PrintMerge();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //Inventec.Common.Logging.LogSystem.Info("btnPrintMergeXN_Click.End()!");
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                listIdServiceReqType = new List<long>();
                var rowSelected = gridView1.GetSelectedRows();
                //Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq IsmergePrint");
                DisposeMemoryStream(this.GroupStreamPrint);
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);

                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    foreach (var i in rowSelected)
                    {
                        var row = (HIS_SERVICE_REQ_TYPE)gridView1.GetRow(i);
                        listIdServiceReqType.Add(row.ID);
                    }
                    if (listIdServiceReqType != null && listIdServiceReqType.Count > 0)
                    {
                        foreach (var item in lstTreatmentId)
                        {
                            this.treatmentToPrint = item;

                            if (listIdServiceReqType.Contains(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN))
                            {
                                if (!string.IsNullOrEmpty(currentFileName26))
                                {
                                    PrintMps000026(this.currentTypeCode26, currentFileName26, item);
                                }
                                else
                                {
                                    richEditorMain.RunPrintTemplate("Mps000026", DelegateRunPrinter);
                                }
                            }

                            if (listIdServiceReqType.Where(o => o != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList() != null &&
                                listIdServiceReqType.Where(o => o != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Count > 0)
                            {
                                bool result = false;
                                if (!string.IsNullOrEmpty(currentFileName451))
                                {
                                    PrintMps000451(this.currentTypeCode451, currentFileName451, ref result, item);
                                }
                                else
                                {
                                    richEditorMain.RunPrintTemplate("Mps000451", DelegateRunPrinter);
                                }
                            }
                        }
                    }
                }

                this.TotalSereServPrint = totalSereServ26 + totalSereServ451;
                PrintMerge();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMerge()
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq IsmergePrint");
                int countTimeOut = 0;
                //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                //Inventec.Common.Logging.LogSystem.Debug("TotalSereServPrint__" + TotalSereServPrint + ",CountSereServPrinted_____" + CountSereServPrinted);
                while (this.TotalSereServPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
                {
                    Thread.Sleep(50);
                    countTimeOut++;
                }

                if (countTimeOut > TIME_OUT_PRINT_MERGE)
                {
                    throw new Exception("TimeOut");
                }
                if (CancelPrint)
                {
                    throw new Exception("Cancel Print");
                }

                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();

                //Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                    adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                printProcess.SetPartialFile(this.GroupStreamPrint);
                printProcess.PrintPreviewShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                EmrDataStore.treatmentCode = (chiDinhDichVuADO != null && chiDinhDichVuADO.treament != null ? chiDinhDichVuADO.treament.TREATMENT_CODE : "");
                switch (printTypeCode)
                {
                    case "Mps000026":
                        this.currentTypeCode26 = printTypeCode;
                        this.currentFileName26 = fileName;
                        PrintMps000026(printTypeCode, fileName, this.treatmentToPrint);
                        break;
                    case "Mps000451":
                        this.currentTypeCode451 = printTypeCode;
                        this.currentFileName451 = fileName;
                        PrintMps000451(printTypeCode, fileName, ref result, this.treatmentToPrint);
                        break;
                    case "Mps000461":
                        this.currentTypeCode461 = printTypeCode;
                        this.currentFileName461 = fileName;
                        PrintMps000461(printTypeCode, fileName, this.treatmentToPrint);
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


        #region SetUpData

        private List<V_HIS_SERVICE_REQ> GetServiceReq(long treatmentId,List<long> ServiceReqTypeIds = null)
        {
            List<V_HIS_SERVICE_REQ> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.HAS_EXECUTE = true;
                filter.TREATMENT_ID = treatmentId;
                if(ServiceReqTypeIds!=null)
                    filter.SERVICE_REQ_TYPE_IDs = ServiceReqTypeIds;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<V_HIS_SERE_SERV> GetSereServ(long treatmentId, List<long> srList)
        {
            List<V_HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.HAS_EXECUTE = true;
                filter.SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<HIS_SERE_SERV_EXT> GetSereServExt(long treatmentId,List<long> ServiceSereIds)
        {
            List<HIS_SERE_SERV_EXT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                filter.SERE_SERV_IDs = ServiceSereIds;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<V_HIS_BED_LOG> GetBedLog(long treatmentId)
        {
            List<V_HIS_BED_LOG> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<HIS_SERE_SERV_BILL> GetSereServBills(long treatmentId, List<long> srList)
        {
            List<HIS_SERE_SERV_BILL> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServBillFilter Filter = new HisSereServBillFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.TDL_SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<HIS_SERE_SERV_DEPOSIT> GetSereServDeposits(long treatmentId, List<long> ssList)
        {
            List<HIS_SERE_SERV_DEPOSIT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServDepositFilter Filter = new HisSereServDepositFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.SERE_SERV_IDs = ssList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void SetDataGroup(int count, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                this.CountSereServPrinted += count;
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_SERVICE_REQ_TYPE)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000026Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, List<V_HIS_SERVICE_REQ> dicServiceReqData, List<V_HIS_SERE_SERV> dicSereServData, List<HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs)
        {
            try
            {
                MPS.Processor.Mps000026.PDO.Mps000026ADO mps000026ADO = new MPS.Processor.Mps000026.PDO.Mps000026ADO();
                mps000026ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                mps000026ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                mps000026ADO.ratio = chiDinhDichVuADO.Ratio;
                mps000026ADO.PatientTypeId__Bhyt = Config.HisConfigCFG.PatientTypeId__BHYT;

                foreach (var serviceReq in dicServiceReqData)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000026.PDO.Mps000026PDO mps000026RDO = null;

                    mps000026ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = null;

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == serviceReq.REQUEST_LOGINNAME);
                    mps000026ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    LIS.EFMODEL.DataModels.V_LIS_SAMPLE lisSample = null;
                    if (this.LisSamples != null && this.LisSamples.Count > 0)
                    {
                        var sample = this.LisSamples.Where(o => o.SERVICE_REQ_CODE == serviceReq.SERVICE_REQ_CODE).ToList();
                        if (sample != null && sample.Count > 0)
                        {
                            lisSample = sample.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }

                    var lstSereServ_26 = new List<MPS.Processor.Mps000026.PDO.Mps000026_ListSereServ>();
                    foreach (var sere in dicSereServData)
                    {
                        if (sere.SERVICE_REQ_ID == serviceReq.ID)
                        {
                            var se26 = new MPS.Processor.Mps000026.PDO.Mps000026_ListSereServ(sere);
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == se26.SERVICE_ID);
                            se26.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se26.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.Count() > 0)
                            {
                                se26.CONCLUDE = dicSereServExtData[Convert.ToInt16(sere.ID)].CONCLUDE;
                                se26.BEGIN_TIME = dicSereServExtData[Convert.ToInt16(sere.ID)].BEGIN_TIME;
                                se26.END_TIME = dicSereServExtData[Convert.ToInt16(sere.ID)].END_TIME;
                                se26.INSTRUCTION_NOTE = dicSereServExtData[Convert.ToInt16(sere.ID)].INSTRUCTION_NOTE;
                                se26.NOTE = dicSereServExtData[Convert.ToInt16(sere.ID)].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                if (Condition != null)
                                {
                                    se26.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se26.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }
                            lstSereServ_26.Add(se26);
                        }
                    }

                    List<long> _ssIds = dicSereServData.Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(p => p.ID == _service.PARENT_ID.Value);
                            mps000026ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }
                    }

                    //Inventec.Common.Logging.LogSystem.Debug("lstSereServ_26.Count: " + (lstSereServ_26 != null ? lstSereServ_26.Count() : 0));

                    List<V_HIS_TRANSACTION> transaction = null;
                    if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                    {
                        List<long> transactionIds = new List<long>();
                        if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Select(s => s.BILL_ID));
                        }

                        if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Select(s => s.DEPOSIT_ID));
                        }

                        transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                    }

                    mps000026RDO = new MPS.Processor.Mps000026.PDO.Mps000026PDO(
                        chiDinhDichVuADO.treament,
                            serviceReq,
                            lstSereServ_26,
                            chiDinhDichVuADO.patientTypeAlter,
                            mps000026ADO,
                            bedLog,
                            null,
                            null,
                            null,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction
                        );
                    Print.PrintData(printTypeCode, fileName, mps000026RDO, lstSereServ_26.Count, null, SetDataGroup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000461Print(string printTypeCode, string fileName, HIS_TREATMENT currentTreatment, List<V_HIS_SERVICE_REQ> dicServiceReqData, List<V_HIS_SERE_SERV> dicSereServData)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("Mps000461Print().Begin!");

                MPS.Processor.Mps000461.PDO.Mps000461PDO mps000461RDO = new MPS.Processor.Mps000461.PDO.Mps000461PDO(
                currentTreatment,
                dicServiceReqData,
                dicSereServData
                );

                Print.PrintData(printTypeCode, fileName, mps000461RDO, dicSereServData.Count, null, SetDataGroup);
                //Inventec.Common.Logging.LogSystem.Debug("Mps000461Print().All!");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //Inventec.Common.Logging.LogSystem.Debug("Mps000461Print().Ended!");
        }

        public void CancelChooseTemplate(string printTypeCode)
        {
            try
            {
                this.CancelPrint = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void btnSplitServiceReq_Click(object sender, EventArgs e)
		{
			try
			{
                var rowSelected = gridView1.GetSelectedRows();
                if (rowSelected == null || rowSelected.Length == 0)
				{
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn cần phải tích chọn loại dịch vụ trước khi in", Resources.ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                List<long> lstServiceReqTypeIds = new List<long>();
                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    foreach (var i in rowSelected)
                    {
                        var row = (HIS_SERVICE_REQ_TYPE)gridView1.GetRow(i);
                        if (row != null)
                        {
                            lstServiceReqTypeIds.Add(row.ID);
                        }
                    }
                }
                if (listTreatment != null && listTreatment.Count > 0)
				{
					listTreatment = listTreatment.OrderBy(o => o.KSK_ORDER).ToList();

					foreach (var item in listTreatment)
					{
						HisServiceReqListResultSDO serviceReq = new HisServiceReqListResultSDO();
						HisTreatmentWithPatientTypeInfoSDO treatmentWithPatientType = new HisTreatmentWithPatientTypeInfoSDO();
						List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

						ProcessDataPrint(item, ref serviceReq, ref treatmentWithPatientType, ref listBedLogs, lstServiceReqTypeIds);

						if (serviceReq != null && treatmentWithPatientType != null && serviceReq.ServiceReqs != null && serviceReq.ServiceReqs.Count > 0)
						{
							serviceReq.ServiceReqs = serviceReq.ServiceReqs.OrderBy(o => o.TREATMENT_CODE).OrderBy(p => p.SERVICE_REQ_CODE).ToList();
							HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor printProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReq, treatmentWithPatientType, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
							printProcessor.SavePrintTreatmentList();
						}
					}
				}
			}
            catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private void ProcessDataPrint(V_HIS_TREATMENT_4 treatment, ref HisServiceReqListResultSDO serviceReq, ref HisTreatmentWithPatientTypeInfoSDO treatmentWithPatientType, ref List<V_HIS_BED_LOG> bedLog,List<long> servicereqTypeIds)
        {
            try
            {
                var serviceReqGet = GetServiceReq(treatment.ID, servicereqTypeIds);
                var sereServGet = GetSereServPrint(treatment.ID, serviceReqGet!=null ? serviceReqGet.Select(o=>o.ID).ToList() : null);
                var sereServExtGet = GetSereServExt(treatment.ID, sereServGet != null ? sereServGet.Select(o=>o.ID).ToList() : null);
                var bedLogGet = GetBedLog(treatment.ID);

                serviceReq.ServiceReqs = serviceReqGet != null ? serviceReqGet : new List<V_HIS_SERVICE_REQ>();
                serviceReq.SereServs = sereServGet != null ? sereServGet : new List<V_HIS_SERE_SERV>();
                serviceReq.SereServExts = sereServExtGet != null ? sereServExtGet : new List<HIS_SERE_SERV_EXT>();

                bedLog = bedLogGet != null ? bedLogGet : new List<V_HIS_BED_LOG>();

                Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(treatmentWithPatientType, treatment);
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                LoadCurrentPatientTypeAlter(treatment.ID, ref patientTypeAlter);
                if (patientTypeAlter != null)
                {
                    treatmentWithPatientType.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                    treatmentWithPatientType.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                    treatmentWithPatientType.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                    treatmentWithPatientType.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                    treatmentWithPatientType.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                    treatmentWithPatientType.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                    treatmentWithPatientType.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                    treatmentWithPatientType.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                    treatmentWithPatientType.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<V_HIS_SERE_SERV> GetSereServPrint(long treatmentId, List<long> ServiceReqIds = null)
        {
            List<V_HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.HAS_EXECUTE = true;
                filter.SERVICE_REQ_IDs = ServiceReqIds;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        public void LoadCurrentPatientTypeAlter(long treatmentId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();

                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceType_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DisposeMemoryStream(this.GroupStreamPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisposeMemoryStream(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> groupStreamPrint)
        {
            try
            {
                if (groupStreamPrint == null || groupStreamPrint.Count == 0)
                    return;
                long numberOfDocumentFileDisposed = 0;
                foreach (var item in groupStreamPrint)
                {
                    if (item != null && item.saveMemoryStream != null)
                    {
                        item.saveMemoryStream.Dispose();
                        item.saveMemoryStream = null;
                        numberOfDocumentFileDisposed++;
                    }
                }
                Inventec.Common.Logging.LogAction.Info("_____DisposeMemoryStream : " + numberOfDocumentFileDisposed + " Document file Disposed");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmServiceType
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmServiceType = new ResourceManager("HIS.Desktop.Plugins.TreatmentList.Resources.Lang", typeof(frmServiceType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServiceType.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintMergeXN.Text = Inventec.Common.Resource.Get.Value("frmServiceType.btnPrintMergeXN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmServiceType.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmServiceType.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmServiceType.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmServiceType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmServiceType.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSplitServiceReq.Text = Inventec.Common.Resource.Get.Value("frmServiceType.btnSplitServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmServiceType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
