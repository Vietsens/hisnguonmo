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
using HIS.Desktop.Plugins.OtherFormAssTreatment.Base;
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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.WordContent;
using MPS.ProcessorBase;

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
    {
        private void ProcessDirectory(string targetDirectory, string printTypeCode)
        {
            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        ProcessFile(subdirectory, printTypeCode);
                        ProcessDirectory(subdirectory, printTypeCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessFile(string path, string printTypeCode)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] fileEntries = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".xls") || s.EndsWith(".doc") || s.EndsWith(".xlsx") || s.EndsWith(".docx")).ToArray();
                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong ton tai file template trong thu muc. Path = " + path + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("File template trong thu muc. Path = " + path + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                        foreach (var item in fileEntries)
                        {
                            CreatePrintTemplate(item, printTypeCode);
                        }
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

        private void CreatePrintTemplate(string fileName, string printTypeCode)
        {
            try
            {
                SarPrintTypeAdo printTemplate = new SarPrintTypeAdo();
                printTemplate.PRINT_TYPE_CODE = printTypeCode;
                printTemplate.FILE_NAME = fileName;
                var pt = RichEditorConfig.PrintTypes != null ? RichEditorConfig.PrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode) : null;
                if (pt != null)
                {
                    printTemplate.PRINT_TYPE_NAME = pt.PRINT_TYPE_NAME;
                    printTemplate.ID = pt.ID;
                    printTemplate.TITLE = pt.PRINT_TYPE_NAME;
                }

                try
                {
                    int indexg1_3 = fileName.LastIndexOf("\\");
                    printTemplate.FILE_PATTERN = fileName.Substring(indexg1_3 + 1, fileName.Length - indexg1_3 - 1);

                }
                catch { }

                if (printTemplate.FILE_PATTERN != null && printTemplate.FILE_PATTERN.StartsWith("~$")) return;
                this.printTypeTemplates.Add(printTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetCommonSingleKey()
        {
            try
            {
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._PARENT_ORGANIZATION_NAME, PrintConfig.ParentOrganizationName);
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._ORGANIZATION_NAME, PrintConfig.OrganizationName);
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._ORGANIZATION_ADDRESS, PrintConfig.OrganizationAddress);
                System.DateTime now = System.DateTime.Now;
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_TIME_STR, now.ToString("dd/MM/yyyy HH:mm:ss"));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_DATE_STR, now.ToString("dd/MM/yyyy"));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_MONTH_STR, now.ToString("MM/yyyy"));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_TIME_SEPARATE_STR, GlobalQuery.GetCurrentTimeSeparate(now));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_TIME_SEPARATE_BEGIN_TIME_STR, GlobalQuery.GetCurrentTimeSeparateBeginTime(now));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_MONTH_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToMonthSeparateString(now));
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_LOGINNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_COMPUTER_NAME, System.Environment.MachineName);
                if (PrintConfig.OrganizationLogo != null && PrintConfig.OrganizationLogo.Count() > 0)
                {
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_LOGO, PrintConfig.OrganizationLogo);
                }
                if (PrintConfig.OrganizationLogoUri != null)
                {
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._CURRENT_LOGO_URI, PrintConfig.OrganizationLogoUri);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBieuMauBenhAnYHocCoTruyenClick(SarPrintTypeAdo sarPrintType)
        {
            try
            {
                dicParamPlus = new Dictionary<string, object>();
                //SarPrintTypeFilter printTypeFilter = new SarPrintTypeFilter();
                //printTypeFilter.PRINT_TYPE_CODE = this.printTypeCode;
                //SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>("api/SarPrintType/Get", ApiConsumers.SarConsumer, printTypeFilter, param).FirstOrDefault();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //SetCommonKey.SetCommonSingleKey(dicParamPlus);

                this.SetDicParamPatient(ref dicParamPlus);
                this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);

                //Thoi gian vao vien
                if (this.Treatment != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);

                    dicParamPlus.Add("CLINICAL_IN_TIME_STR", this.Treatment.CLINICAL_IN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.Treatment.CLINICAL_IN_TIME.Value) : "");
                    dicParamPlus.Add("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.Treatment.IN_TIME));

                    dicParamPlus["DOB_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Treatment.TDL_PATIENT_DOB);

                    // minhnq
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._PARENT_ORGANIZATION_NAME, PrintConfig.ParentOrganizationName);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._ORGANIZATION_NAME, PrintConfig.OrganizationName);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, CommonKey._ORGANIZATION_ADDRESS, PrintConfig.OrganizationAddress);

                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = this.Treatment.ID;
                    var ssByTreatments = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                    if (ssByTreatments != null)
                    {
                        dicParamPlus["TOTAL_PRICE"] = string.Format("{0:#,##0.00}", ssByTreatments.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND));
                    }
                    dicParamPlus["LOGINNAME"] = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    dicParamPlus["USERNAME"] = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    HisEmployeeFilter empFilter = new HisEmployeeFilter();
                    empFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var employee = new BackendAdapter(new CommonParam()).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, empFilter, null);
                    if (employee != null)
                    {
                        dicParamPlus["TITLE"] = employee.First().TITLE;
                    }


                    //
                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == Treatment.TDL_TREATMENT_TYPE_ID);
                    if (treatmentType != null)
                    {
                        dicParamPlus["TREATMENT_TYPE_CODE"] = treatmentType.TREATMENT_TYPE_CODE;
                        dicParamPlus["TREATMENT_TYPE_NAME"] = treatmentType.TREATMENT_TYPE_NAME;
                    }
                    else
                    {
                        dicParamPlus["TREATMENT_TYPE_CODE"] = "";
                        dicParamPlus["TREATMENT_TYPE_NAME"] = "";
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Treatment.TDL_PATIENT_TYPE_ID);
                    if (patientType != null)
                    {
                        dicParamPlus["PATIENT_TYPE_CODE"] = patientType.PATIENT_TYPE_CODE;
                        dicParamPlus["PATIENT_TYPE_NAME"] = patientType.PATIENT_TYPE_NAME;
                    }
                    else
                    {
                        dicParamPlus["PATIENT_TYPE_CODE"] = "";
                        dicParamPlus["PATIENT_TYPE_NAME"] = "";
                    }
                    this.SetDicParamTreatment(ref dicParamPlus);
                    //AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);

                    var LastDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.Treatment.LAST_DEPARTMENT_ID);
                    if (LastDepartment != null)
                    {
                        dicParamPlus.Add("LAST_DEPARTMENT_NAME", LastDepartment.DEPARTMENT_NAME);
                        dicParamPlus.Add("LAST_DEPARTMENT_CODE", LastDepartment.DEPARTMENT_CODE);
                    }
                    else
                    {
                        dicParamPlus.Add("LAST_DEPARTMENT_NAME", "");
                        dicParamPlus.Add("LAST_DEPARTMENT_CODE", "");
                    }
                    if (TreatmentBedRooms != null && TreatmentBedRooms.Count > 0)
                    {
                        dicParamPlus.Add("TREATMENT_BED_ROOM_CODE", TreatmentBedRooms.OrderByDescending(o => o.ADD_TIME).First().BED_ROOM_CODE);

                        dicParamPlus.Add("TREATMENT_BED_ROOM_NAME", TreatmentBedRooms.OrderByDescending(o => o.ADD_TIME).First().BED_ROOM_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("TREATMENT_BED_ROOM_CODE", "");

                        dicParamPlus.Add("TREATMENT_BED_ROOM_NAME", "");
                    }

                }
                else
                {
                    V_HIS_TREATMENT temp = new V_HIS_TREATMENT();
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(temp, dicParamPlus);
                    dicParamPlus.Add("CLINICAL_IN_TIME_STR", "");
                    dicParamPlus.Add("IN_TIME_STR", "");
                    dicParamPlus["DOB_STR"] = "";
                }
                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDate());

                string extension = Path.GetExtension(sarPrintType.FILE_NAME);
                //if (extension.Equals(".xls") || extension.Equals(".xlsx") || extension.Equals(".xlt") || extension.Equals(".xltx"))
                //{
                //    this.runPrinter(this.printTypeCode, _SarPrintTypeAdo.FILE_NAME);
                //}
                //else
                if (extension.Equals(".doc") || extension.Equals(".docx"))
                {
                    WaitingManager.Hide();
                    //if (RichEditorStore.isPrintEditor)
                    //{

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), sarPrintType.PRINT_TYPE_CODE, currentModule != null ? currentModule.RoomId : 0);

                    frmPrintEditor printEditor = new frmPrintEditor(sarPrintType.FILE_NAME, "Biểu mẫu khác__", UpdateTreatmentJsonPrint, dicParamPlus, this.dicImagePlus, inputADO);
                    printEditor.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Sai định dạng file . Chỉ hỗ trợ định dạng file .doc, .docx");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> singleValueDictionary, bool isAllowOverrideValue = false)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (!singleValueDictionary.ContainsKey(pi.Name))
                        {
                            singleValueDictionary.Add(pi.Name, pi.GetValue(data) != null ? pi.GetValue(data) : "");
                        }
                        else if (isAllowOverrideValue)
                        {
                            singleValueDictionary[pi.Name] = pi.GetValue(data) != null ? pi.GetValue(data) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessWordAndShowWordContentResult(SarPrintTypeAdo sarPrintType)
        {
            try
            {
                bool success = true;

                Inventec.Common.TemplaterExport.Store templaterExportStore = new Inventec.Common.TemplaterExport.Store();

                Inventec.Common.TemplaterExport.ProcessSingleTag singleTag = new Inventec.Common.TemplaterExport.ProcessSingleTag();
                Inventec.Common.TemplaterExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.TemplaterExport.ProcessBarCodeTag();
                Inventec.Common.TemplaterExport.ProcessObjectTag objectTag = new Inventec.Common.TemplaterExport.ProcessObjectTag();

                success = templaterExportStore.ReadTemplate(System.IO.Path.GetFullPath(sarPrintType.FILE_NAME));
                //success = success && barCodeTag.ProcessData(templaterExportStore, dicImage);
                success = success && singleTag.ProcessData(templaterExportStore, dicParamPlus);

                string resultFile = success ? templaterExportStore.OutFile() : "";
                if (!String.IsNullOrEmpty(resultFile) && File.Exists(resultFile))
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), sarPrintType.PRINT_TYPE_CODE, currentModule != null ? currentModule.RoomId : 0);

                    WordContentProcessor wordContentProcessor = new WordContentProcessor();
                    WordContentADO wordContentADO = new WordContentADO();
                    wordContentADO.EmrInputADO = inputADO;
                    wordContentADO.TemplateKey = dicParamPlus;
                    wordContentADO.FileName = resultFile;
                    SAR_PRINT_TYPE ptRow = null;
                    if (!String.IsNullOrEmpty(sarPrintType.PRINT_TYPE_CODE))
                    {
                        ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == sarPrintType.PRINT_TYPE_CODE);
                    }
                    wordContentADO.SarPrintType = ptRow;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sarPrintType), sarPrintType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ptRow), ptRow));

                    wordContentADO.ActUpdateReference = ActUpdateTreatmentJsonPrint;
                    wordContentADO.TemplateFileName = sarPrintType.FILE_NAME;//TODO

                    wordContentProcessor.ShowForm(wordContentADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool UpdateTreatmentJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.Treatment != null);
                if (valid)
                {
                    HIS_TREATMENT hisTreatment = new HIS_TREATMENT();
                    var listOldPrintIdOfTreatments = GetListPrintIdByTreatment();
                    ProcessTreatmentExecuteForUpdateJsonPrint(hisTreatment, listOldPrintIdOfTreatments, sarPrintCreated);
                    SaveTreatment(hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        void ActUpdateTreatmentJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.Treatment != null);
                if (valid)
                {
                    HIS_TREATMENT hisTreatment = new HIS_TREATMENT();
                    var listOldPrintIdOfTreatments = GetListPrintIdByTreatment();
                    ProcessTreatmentExecuteForUpdateJsonPrint(hisTreatment, listOldPrintIdOfTreatments, sarPrintCreated);
                    SaveTreatment(hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveTreatment(HIS_TREATMENT hisTreatment)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                hisTreatment.ID = this.Treatment.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_UPDATE_JSON, ApiConsumers.MosConsumer, hisTreatment, param);
                WaitingManager.Hide();
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                    LoadData();
                    LoadJSonPrintOld();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessTreatmentExecuteForUpdateJsonPrint(HIS_TREATMENT hisTreatment, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.Treatment != null)
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
                    hisTreatment.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> GetListPrintIdByTreatment()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.Treatment != null)
                {
                    if (!String.IsNullOrEmpty(this.Treatment.JSON_PRINT_ID))
                    {
                        var arrIds = this.Treatment.JSON_PRINT_ID.Split(',', ';');
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
    }
}
