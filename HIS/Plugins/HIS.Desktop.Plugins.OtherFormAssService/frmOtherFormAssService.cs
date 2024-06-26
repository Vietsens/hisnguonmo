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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OtherFormAssService
{
    public partial class frmOtherFormAssService : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long _ServiceReqId;

        public frmOtherFormAssService()
        {
            InitializeComponent();
        }

        public frmOtherFormAssService(Inventec.Desktop.Common.Modules.Module _currentModule, long _serviceReqId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
                this._ServiceReqId = _serviceReqId;

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

        private void frmOtherFormAssService_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                GetDatas();

                //TODO
                InitData();

                LoadJSonPrintOld();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string printTypeCode = "Mps000278";
        List<SarPrintTypeAdo> printTypes;
        List<SAR_PRINT_TYPE> printTypeByCodes;
        string title;
        private void InitData()
        {
            try
            {
                gridControlForm.DataSource = null;
                if (String.IsNullOrEmpty(this.printTypeCode)) throw new ArgumentNullException("printTypeCode is null");

                this.printTypes = new List<SarPrintTypeAdo>();

                this.printTypeByCodes = RichEditorConfig.PrintTypes.Where(o => (o.PRINT_TYPE_CODE ?? "").ToLower() == this.printTypeCode.ToLower()).ToList();
                if (printTypeByCodes != null && printTypeByCodes.Count > 0)
                {
                    if (!Directory.Exists(System.IO.Path.Combine(FileLocalStore.Rootpath, this.printTypeCode)))
                    {
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_FOLDER_TUONG_UNG_VOI_MA_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + this.printTypeCode, this.printTypeCode));
                        throw new DirectoryNotFoundException("Khong ton tai folder chua bieu mau in: " + System.IO.Path.Combine(FileLocalStore.Rootpath, this.printTypeCode) + " . " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeCode), printTypeCode));
                    }

                    ProcessDirectory(System.IO.Path.Combine(FileLocalStore.Rootpath, this.printTypeCode));
                    ProcessFile(System.IO.Path.Combine(FileLocalStore.Rootpath, this.printTypeCode));

                    if (printTypes == null || printTypes.Count == 0)
                    {
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_BIEU_MAU_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + this.printTypeCode, this.printTypeCode));

                        // to get the location the assembly is executing from
                        //(not necessarily where the it normally resides on disk)
                        // in the case of the using shadow copies, for instance in NUnit tests, 
                        // this will be in a temp directory.
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

                        //To get the location the assembly normally resides on disk or the install directory
                        string path1 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                        Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc file template. path(Location) =" + path + " - path(CodeBase)=" + path1);
                        Inventec.Common.Logging.LogSystem.Debug("FileLocalStore.rootpath =" + FileLocalStore.Rootpath);
                    }

                    gridControlForm.DataSource = printTypes;
                }
                else
                {
                    //Inventec.Common.Logging.LogSystem.Info("Khong lay duoc sarprint type theo dieu kien tim kiem. fileName=" + this.fileName + " - printTypeCode=" + this.printTypeCode);
                    MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_MA_IN, this.printTypeCode));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDirectory(string targetDirectory)
        {
            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        ProcessFile(subdirectory);
                        ProcessDirectory(subdirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessFile(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] fileEntries = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".xls") || s.EndsWith(".doc") || s.EndsWith(".xlsx") || s.EndsWith(".docx")).ToArray();
                    //string[] fileEntries = Directory.GetFiles(path, "*.xls|*.doc|*.xlsx|*.docx", SearchOption.AllDirectories).OrderBy(f => f).ToArray();
                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong ton tai file template trong thu muc. Path = " + path + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                    }
                    else
                    {
                        //foreach (var file in this.currentFileNames)
                        //{
                        //var fileEntrieSearchs = fileEntries.Where(f => f.Contains(file)).ToArray();
                        //if (fileEntrieSearchs != null && fileEntrieSearchs.Count() > 0)
                        //{
                        foreach (var item in fileEntries)
                        {
                            CreatePrintTemplate(item);
                        }
                        //}
                        //else
                        //{
                        //    Inventec.Common.Logging.LogSystem.Info("Tim kiem file template theo cau hinh FILE_PATTERN = " + file + " khong co du lieu. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntrieSearchs), fileEntrieSearchs));
                        //}
                        //}
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong ton tai thu muc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path), path));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreatePrintTemplate(string fileName)
        {
            try
            {
                SarPrintTypeAdo printTemplate = new SarPrintTypeAdo();
                printTemplate.PRINT_TYPE_CODE = this.printTypeCode;
                printTemplate.FILE_NAME = fileName;
                printTemplate.TITLE = this.title;
                try
                {
                    int indexg1_3 = fileName.LastIndexOf("\\");
                    printTemplate.FILE_PATTERN = fileName.Substring(indexg1_3 + 1, fileName.Length - indexg1_3 - 1);
                }
                catch { }

                this.printTypes.Add(printTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewForm_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //var data = (SarPrintTypeAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Create_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                // WaitingManager.Show();
                var data = (SarPrintTypeAdo)gridViewForm.GetFocusedRow();
                if (data != null)
                {
                    TaoBieuMauKhacClick(data);
                }
                // WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                // WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, System.Drawing.Image>();
        CommonParam param = new CommonParam();

        V_HIS_SERVICE_REQ _ServiceReq { get; set; }
        V_HIS_PATIENT _Patient { get; set; }
        V_HIS_TREATMENT _Treatmnet { get; set; }

        private void TaoBieuMauKhacClick(SarPrintTypeAdo _SarPrintTypeAdo)
        {
            try
            {
                dicParamPlus = new Dictionary<string, object>();
                //SarPrintTypeFilter printTypeFilter = new SarPrintTypeFilter();
                //printTypeFilter.PRINT_TYPE_CODE = this.printTypeCode;
                //SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>("api/SarPrintType/Get", ApiConsumers.SarConsumer, printTypeFilter, param).FirstOrDefault();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                SetCommonKey.SetCommonSingleKey(dicParamPlus);

                //   CreateThreadLoadData(this.treatmentId);//LoadData

                addDataToBieuMauKhac(dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this._ServiceReq, dicParamPlus);
                AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(this._Patient, dicParamPlus);


                // richEditorMain.RunPrintTemplate(_SarPrintTypeAdo.PRINT_TYPE_CODE, _SarPrintTypeAdo.FILE_NAME, "Biểu mẫu khác___", UpdateServiceReqJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);

                string extension = Path.GetExtension(_SarPrintTypeAdo.FILE_NAME);
                //if (extension.Equals(".xls") || extension.Equals(".xlsx") || extension.Equals(".xlt") || extension.Equals(".xltx"))
                //{
                //    this.runPrinter(this.printTypeCode, _SarPrintTypeAdo.FILE_NAME);
                //}
                //else
                if (extension.Equals(".doc") || extension.Equals(".docx"))
                {
                    //if (RichEditorStore.isPrintEditor)
                    //{
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._Treatmnet != null ? _Treatmnet.TREATMENT_CODE : ""), this.printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                    frmPrintEditor printEditor = new frmPrintEditor(_SarPrintTypeAdo.FILE_NAME, "Biểu mẫu khác__", UpdateServiceReqJsonPrint, dicParamPlus, this.dicImagePlus, inputADO);
                    printEditor.ShowDialog();
                    //}
                    //else
                    //{
                    //    frmSimplePrintEditor printEditor = new frmSimplePrintEditor(_SarPrintTypeAdo.FILE_NAME, UpdateServiceReqJsonPrint, dicParamPlus, dicImagePlus);
                    //    printEditor.ShowPrintPreview();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void addDataToBieuMauKhac(Dictionary<string, object> dicParamPlus)
        {
            try
            {
                MOS.Filter.HisDepartmentTranViewFilter filterDeparementTran = new HisDepartmentTranViewFilter();
                filterDeparementTran.TREATMENT_ID = _Treatmnet.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> lstDepartmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, filterDeparementTran, param);
                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    dicParamPlus.Add("OPEN_DATE_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatmnet.IN_TIME));
                    dicParamPlus.Add("OPEN_TIME_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_Treatmnet.IN_TIME));
                    if (lstDepartmentTran[lstDepartmentTran.Count - 1] != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_TRAN_CODE", lstDepartmentTran[lstDepartmentTran.Count - 1].DEPARTMENT_CODE);
                        dicParamPlus.Add("DEPARTMENT_TRAN_NAME", lstDepartmentTran[lstDepartmentTran.Count - 1].DEPARTMENT_NAME);

                    }
                    else
                    {
                        dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", null);
                        dicParamPlus.Add("CLOSE_TIME_SEPARATE_STR", null);
                        dicParamPlus.Add("DEPARTMENT_TRAN_CODE", null);
                        dicParamPlus.Add("DEPARTMENT_TRAN_NAME", null);
                        dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", null);
                        dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", null);
                    }
                }
                else
                {
                    dicParamPlus.Add("OPEN_DATE_SEPARATE_STR", null);
                    dicParamPlus.Add("CLOSE_DATE_SEPARATE_STR", null);
                    dicParamPlus.Add("OPEN_TIME_SEPARATE_STR", null);
                    dicParamPlus.Add("CLOSE_TIME_SEPARATE_STR", null);
                    dicParamPlus.Add("DEPARTMENT_TRAN_CODE", null);
                    dicParamPlus.Add("DEPARTMENT_TRAN_NAME", null);
                    dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", null);
                    dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", null);
                }


                if (this._Treatmnet != null)
                {
                    if (!String.IsNullOrEmpty(this._Treatmnet.ICD_NAME))
                    {
                        dicParamPlus.Add("ICD_END_MAIN_TEXT", this._Treatmnet.ICD_CODE + "  -  " + this._Treatmnet.ICD_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_END_MAIN_TEXT", null);
                    }

                    if (!String.IsNullOrEmpty(this._Treatmnet.ICD_TEXT))
                    {
                        dicParamPlus.Add("ICD_END_TEXT", this._Treatmnet.ICD_TEXT);
                        dicParamPlus.Add("ICD_CODE_ICD_TEXT", this._Treatmnet.ICD_CODE + ",  " + this._Treatmnet.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_END_TEXT", null);
                        dicParamPlus.Add("ICD_CODE_ICD_TEXT", null);
                    }
                    if (!String.IsNullOrEmpty(this._Treatmnet.TDL_PATIENT_WORK_PLACE_NAME))
                    {
                        dicParamPlus.Add("WORK_PLACE_NAME_STR", this._Treatmnet.TDL_PATIENT_WORK_PLACE_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("WORK_PLACE_NAME_STR", null);
                    }

                    long ngayDieuTri = 0;
                    ngayDieuTri += HIS.Treatment.DateTime.Calculation.DayOfTreatment(this._Treatmnet.IN_TIME, this._Treatmnet.OUT_TIME ?? 0);
                    dicParamPlus.Add("TOTAL_DAY", ngayDieuTri);
                }

                if (this._ServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(this._ServiceReq.ICD_TEXT))
                    {
                        dicParamPlus.Add("ICD_EXAM_TEXT", this._ServiceReq.ICD_SUB_CODE + " _ " + this._ServiceReq.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_EXAM_TEXT", null);
                    }
                    dicParamPlus.Add("ICD_EXAM_MAIN_TEXT", this._ServiceReq.ICD_CODE + " _ " + this._ServiceReq.ICD_NAME);

                    if (!String.IsNullOrEmpty(this._ServiceReq.ICD_TEXT))
                    {
                        dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", this._ServiceReq.TREATMENT_INSTRUCTION);
                    }
                    else
                    {
                        dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", null);
                    }
                    dicParamPlus.Add("DOB_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ServiceReq.TDL_PATIENT_DOB));
                }
                else
                {
                    dicParamPlus.Add("ICD_EXAM_TEXT", null);
                    dicParamPlus.Add("ICD_EXAM_MAIN_TEXT", null);
                    dicParamPlus.Add("REQUEST_DEPARTMENT_NAME", null);
                    dicParamPlus.Add("HOSPITALIZATION_REASON", null);
                    dicParamPlus.Add("PATHOLOGICAL_HISTORY", null);
                    dicParamPlus.Add("PATHOLOGICAL_HISTORY_FAMILY", null);
                    dicParamPlus.Add("FULL_EXAM", null);
                    dicParamPlus.Add("PART_EXAM", null);
                    dicParamPlus.Add("DESCRIPTION", null);
                    dicParamPlus.Add("PATHOLOGICAL_PROCESS", null);
                    dicParamPlus.Add("REQUEST_ROOM_NAME", null);
                    dicParamPlus.Add("TREATMENT_INSTRUCTION_STR", null);
                }
                if (this._Treatmnet != null)
                {
                    dicParamPlus.Add("ICD_RAVIEN_MAIN_TEXT", this._Treatmnet.TRANSFER_IN_ICD_NAME);//currentTreatmentUpdate.TRANSFER_IN_ICD_CODE + " - " + 

                    //ReView
                    if (this._Treatmnet.ICD_TEXT != null)
                    {
                        dicParamPlus.Add("ICD_RAVIEN_TEXT", this._Treatmnet.ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_RAVIEN_TEXT", null);
                    }
                    dicParamPlus.Add("ICD_MAIN_TEXT_NG", this._Treatmnet.IN_ICD_CODE + " _ " + this._Treatmnet.IN_ICD_NAME);

                    if (this._Treatmnet.IN_ICD_TEXT != null)
                    {
                        dicParamPlus.Add("ICD_TEXT_IN", this._Treatmnet.IN_ICD_TEXT);
                    }
                    else
                    {
                        dicParamPlus.Add("ICD_TEXT_IN", null);
                    }
                }
                else
                {
                    dicParamPlus.Add("ICD_RAVIEN_MAIN_TEXT", null);
                    dicParamPlus.Add("ICD_RAVIEN_TEXT", null);
                    dicParamPlus.Add("ICD_MAIN_TEXT_NG", null);
                    dicParamPlus.Add("ICD_TEXT_IN", null);
                }
                //ReView
                dicParamPlus.Add("CONCLUDE", null);
                dicParamPlus.Add("RESULT_NOTE", null);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool UpdateServiceReqJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current service_req
                List<FileHolder> listFileHolder = new List<FileHolder>();
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                hisServiceReq.ID = this._ServiceReqId;
                var listOldPrintIdOfServiceReqs = GetListPrintIdByServiceReq();
                ProcessServiceReqForUpdateJsonPrint(hisServiceReq, listOldPrintIdOfServiceReqs, sarPrintCreated);
                SaveServiceReq(hisServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        List<long> GetListPrintIdByServiceReq()
        {
            List<long> result = new List<long>();
            try
            {
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = this._ServiceReqId;
                var DataServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                if (DataServiceReqs != null && DataServiceReqs.Count > 0)
                {
                    foreach (var item in DataServiceReqs)
                    {
                        if (!String.IsNullOrEmpty(item.JSON_PRINT_ID))
                        {
                            var arrIds = item.JSON_PRINT_ID.Split(',', ';');
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessServiceReqForUpdateJsonPrint(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ _serviceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
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

                _serviceReq.JSON_PRINT_ID = printIds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReq(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var hisServiceReqWithFileResultSDO = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, hisServiceReq, param);
                if (hisServiceReqWithFileResultSDO != null)
                {
                    success = true;
                    this._ServiceReq.JSON_PRINT_ID = hisServiceReqWithFileResultSDO.JSON_PRINT_ID;
                    LoadJSonPrintOld();
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void GetDatas()
        {
            try
            {
                MOS.Filter.HisServiceReqViewFilter _filter = new HisServiceReqViewFilter();
                _filter.IS_ACTIVE = 1;
                _filter.ID = this._ServiceReqId;
                this._ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, _filter, param).FirstOrDefault();

                MOS.Filter.HisPatientViewFilter _filter1 = new HisPatientViewFilter();
                _filter1.IS_ACTIVE = 1;
                _filter1.ID = this._ServiceReq.TDL_PATIENT_ID;
                this._Patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, _filter1, param).FirstOrDefault();

                MOS.Filter.HisTreatmentViewFilter _filter2 = new HisTreatmentViewFilter();
                _filter2.IS_ACTIVE = 1;
                _filter2.ID = this._ServiceReq.TREATMENT_ID;
                this._Treatmnet = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, _filter2, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadJSonPrintOld()
        {
            try
            {
                gridControlDetail.DataSource = null;
                if (this._ServiceReq != null && !string.IsNullOrEmpty(this._ServiceReq.JSON_PRINT_ID))
                {
                    var prints = new List<SAR.EFMODEL.DataModels.SAR_PRINT>();
                    SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();

                    var printIds = PrintIdByJsonPrint(this._ServiceReq.JSON_PRINT_ID);
                    if (printIds != null && printIds.Count > 0)
                    {
                        printFilter.IDs = printIds;
                        printFilter.ORDER_FIELD = "CREATE_TIME";
                        printFilter.ORDER_DIRECTION = "DESC";
                        prints = new BackendAdapter(new CommonParam())
                        .Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>("api/SarPrint/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, printFilter, new CommonParam());
                    }
                    gridControlDetail.DataSource = prints;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<long> PrintIdByJsonPrint(string json_Print_Id)
        {
            List<long> result = new List<long>();
            try
            {
                var arrIds = json_Print_Id.Split(',', ';');
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal Dictionary<string, object> dicParam;
        internal Dictionary<string, System.Drawing.Image> dicImage;
        Inventec.Common.RichEditor.RichEditorStore richEditorMainV2;
        private void repositoryItemButton__View_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                // WaitingManager.Show();
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewDetail.GetFocusedRow();
                this.richEditorMainV2 = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //richEditorMain.RunPrintEditor(sarPrint, updateDataSuccess);//Sửa
                if (sarPrint != null)
                {
                    List<long> currentPrintIds = new List<long>();
                    currentPrintIds.Add(sarPrint.ID);
                    this.richEditorMainV2.RunPrint(currentPrintIds, dicParam, dicImage, null, ShowPrintPreview);
                }

                // WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                // WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPrintPreview(byte[] CONTENT_B)
        {
            try
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatmnet != null ? _Treatmnet.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                this.richEditorMainV2.ShowPrintPreview(CONTENT_B, null, dicParam, dicImage, true, inputADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool updateDataSuccess(SAR.EFMODEL.DataModels.SAR_PRINT sarPrint)
        {
            LoadJSonPrintOld();
            return true;
        }

        private void gridViewDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SAR_PRINT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.CREATE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewForm_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "create_d")
                {
                    if (this._Treatmnet.IS_PAUSE == 1)
                    {
                        e.RepositoryItem = repositoryItemButton__Create__D;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemButton__Create;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
