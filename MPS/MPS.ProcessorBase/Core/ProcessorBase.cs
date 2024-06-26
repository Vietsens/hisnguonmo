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
using Inventec.Core;
using MPS.ProcessorBase.Core.PrintException;
//using MPS.ProcessorBase.EmrBusiness;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MPS.ProcessorBase.Core
{
    public class ProcessorBase
    {
        protected RDOBase rdoBase;
        protected SAR_PRINT_TYPE printType;
        protected string fileName;
        protected Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();
        protected Dictionary<string, Image> dicImageReplaceBarcode = null;
        protected MPS.ProcessorBase.PrintConfig.PreviewType previewType;
        protected MPS.ProcessorBase.PrintConfig.CallPrintType callPrintType;
        protected string printerName;
        protected string printTypeCode;
        protected string saveFilePath;
        protected MemoryStream saveMemoryStream;
        protected PrintData printDataBase;
        protected Inventec.Common.FlexCelPrint.DelegateEventLog eventLog;
        protected Inventec.Common.FlexCelPrint.DelegatePrintLog printLog;
        protected Inventec.Common.FlexCelPrint.DelegateShowPrintLog ShowLog;
        protected Inventec.Common.FlexCelPrint.DelegateGetNumOrderPrint getNumOrderPrint;
        const string ERROR_MPS__PRINT_TYPE__NOT_FOUND = "ERR.MPS.01";
        protected int numCopy = 1;
        bool isPreview = false;
        bool isAllowExport = true;
        bool isAllowEditTemplateFile = true;
        protected bool isUserWordContent = true;
        CommonParam param;
        protected string treatmentCodeOfPatient;
        protected string currentBussinessCode;
        protected MPS.ProcessorBase.PrintConfig.TemplateType templateType;
        protected List<string> printTypeBusinessCodes = null;
        bool isSingleCopy = false;
        protected Action showTutorialModule;
        bool isPrintExceptionReason = false;

        internal ProcessorBase(CommonParam param, PrintData printData)
        {
            if (param == null)
                this.param = new CommonParam();
            else
                this.param = param;
            this.printDataBase = printData;
            this.rdoBase = (RDOBase)printData.data;
            this.printTypeCode = printData.printTypeCode;
            this.previewType = printData.previewType;
            this.callPrintType = printData.callPrintType;
            this.printerName = printData.printerName;
            this.fileName = printData.fileName;
            this.numCopy = (printData.numCopy <= 0 ? 1 : printData.numCopy);
            this.saveFilePath = printData.saveFilePath;
            this.saveMemoryStream = printData.saveMemoryStream;
            this.isPreview = printData.isPreview;
            this.eventLog = printData.eventLog;
            this.isUserWordContent = printData.isUserWordContent;

            if (PrintConfig.IsExportExcel.HasValue)
            {
                this.isAllowExport = PrintConfig.IsExportExcel.Value;
            }
            else
                this.isAllowExport = printData.isAllowExport;

            if (PrintConfig.IsAllowEditTemplateFile.HasValue)
            {
                this.isAllowEditTemplateFile = PrintConfig.IsAllowEditTemplateFile.Value;
            }
            else
                this.isAllowEditTemplateFile = true;

            //không in mã in bị khóa
            this.printType = PrintConfig.PrintTypes != null ? PrintConfig.PrintTypes.Where(o => o.IS_ACTIVE == 1 && o.PRINT_TYPE_CODE.ToLower() == printData.printTypeCode.ToLower()).FirstOrDefault() : null;

            if (this.printType == null)
            {
                param.BugCodes.Add(ERROR_MPS__PRINT_TYPE__NOT_FOUND);
                throw new PrintTypeNotFoundException(printData.printTypeCode);
            }

            this.ProcessBussinessCode();
            this.ProcessPreviewType();

            //mẫu có khai báo "Không cho phép in lại" và tài khoản đăng nhập ko thuộc các tài khoản được khai báo "cho phép in lại"
            //"không cho phép in ngoại trừ các tài khoản"
            if (printType.DO_NOT_ALLOW_REPRINT == 1 || printType.DO_NOT_ALLOW_PRINT == 1)
            {
                this.isAllowExport = false;
                if (!String.IsNullOrEmpty(printType.REPRINT_EXCEPTION_LOGINNAME) || !String.IsNullOrEmpty(printType.PRINT_EXCEPTION_LOGINNAME))
                {
                    string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if ((!String.IsNullOrEmpty(printType.REPRINT_EXCEPTION_LOGINNAME) && printType.REPRINT_EXCEPTION_LOGINNAME.Contains(loginname)) || (!String.IsNullOrEmpty(printType.PRINT_EXCEPTION_LOGINNAME) && printType.PRINT_EXCEPTION_LOGINNAME.Contains(loginname)))
                    {
                        this.isAllowExport = true;
                    }
                }
            }
            printData.isAllowExport = this.isAllowExport;

            if (printType.IS_SINGLE_COPY == 1)
            {
                this.isSingleCopy = true;
            }
            if (printData.ShowTutorialModule != null)
            {
                this.showTutorialModule = printData.ShowTutorialModule;
            }
            else
            {
                this.showTutorialModule = PrintConfig.ShowTutorialModule;
            }
            if (printType.IS_PRINT_EXCEPTION_REASON == 1)
            {
                this.isPrintExceptionReason = true;
            }
        }

        void ProcessPreviewType()
        {
            try
            {
                int previewTypeByPrintType = 0;//printType.PreviewType////TODO
                if (printType.IS_DIGITAL_SIGN.HasValue && printType.IS_DIGITAL_SIGN == 1 && (this.previewType == null || (this.previewType != null && this.previewType != MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow && this.previewType != MPS.ProcessorBase.PrintConfig.PreviewType.EmrCreateDocument && this.previewType != MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow)))
                {
                    previewTypeByPrintType = 4;//EmrShow
                }

                if (previewTypeByPrintType > 0)
                {
                    if (this.printDataBase.EmrInputADO != null)
                    {
                        switch (previewTypeByPrintType)
                        {
                            case 1:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                                break;
                            case 2:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                                break;
                            case 3:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                                break;
                            case 4:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow;
                                break;
                            case 5:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                                break;
                            case 6:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                                break;
                            case 7:
                                this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Bieu in chua duoc tich hop ky dien tu____" + Inventec.Common.Logging.LogUtil.TraceData("printTypeCode", this.printTypeCode) + Inventec.Common.Logging.LogUtil.TraceData("printDataBase.EmrInputADO", this.printDataBase.EmrInputADO));
                        Inventec.Common.SignLibrary.Integrate.MessageManager.Show("Biểu in chưa tích hợp với hệ thống ký điện tử.");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessBussinessCode()
        {
            try
            {
                this.currentBussinessCode = "";
                this.printTypeBusinessCodes = null;
                if (printType != null && !String.IsNullOrEmpty(printType.BUSINESS_CODES))
                {
                    this.printTypeBusinessCodes = GetBussinessCode(printType.BUSINESS_CODES);
                    if (printType.IS_AUTO_CHOOSE_BUSINESS == 1 && this.printTypeBusinessCodes != null && this.printTypeBusinessCodes.Count > 0)
                    {
                        if (this.printTypeBusinessCodes.Count == 1)
                        {
                            this.currentBussinessCode = this.printTypeBusinessCodes[0];
                        }
                        //else
                        //{
                        //    var businesss = (PrintConfig.EmrBusiness != null && PrintConfig.EmrBusiness.Count > 0) ? PrintConfig.EmrBusiness.Where(o => this.printTypeBusinessCodes.Contains(o.BUSINESS_CODE)).ToList() : null;

                        //    if (businesss != null && businesss.Count > 0)
                        //    {
                        //        frmChooseBusiness frmChooseBusiness = new frmChooseBusiness(ChooseBusinessClick, businesss);
                        //        frmChooseBusiness.ShowDialog();
                        //    }
                        //    else
                        //    {
                        //        Inventec.Common.Logging.LogSystem.Warn("Bieu mau duoc cau hinh nghiep vu ky nao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => businesss), businesss) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printType), printType) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBussinessCode), currentBussinessCode) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeBusinessCodes), printTypeBusinessCodes));
                        //    }
                        //}
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printType), printType) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBussinessCode), currentBussinessCode) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeBusinessCodes), printTypeBusinessCodes));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ChooseBusinessClick(EMR.EFMODEL.DataModels.EMR_BUSINESS dataBusiness)
        {
            this.currentBussinessCode = dataBusiness != null ? dataBusiness.BUSINESS_CODE : "";
        }

        private List<string> GetBussinessCode(string businessCodes)
        {
            List<string> results = new List<string>();
            try
            {
                var rs = businessCodes.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (rs != null && rs.Count() > 0)
                {
                    results = rs.Where(o => o != "[" && o != "]").ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        //protected bool PrintPreviewWord(MemoryStream streamResult, string fileTemplate, Dictionary<string, object> TemplateKey)
        //{
        //    try
        //    {
        //        if (streamResult != null)
        //        {
        //            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileResult), fileResult));
        //            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, PrintConfig.URI_API_SAR, PrintConfig.Language, PrintConfig.TemnplatePathFolder);
        //            richEditorMain.ShowPrintPreview(Utils.StreamToByte(streamResult), null, TemplateKey, dicImageReplaceBarcode, false, (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("streamResult is null");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        return false;
        //    }
        //    return true;
        //}

        protected bool PrintPreviewWord(string fileResult, string fileTemplate, Dictionary<string, object> TemplateKey, bool isUserWordContent)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("PrintPreviewWord.1");
                if (File.Exists(fileResult))
                {
                    if (isUserWordContent)
                    {
                        Inventec.Common.WordContent.WordContentProcessor wordContentProcessor = new Inventec.Common.WordContent.WordContentProcessor();
                        wordContentProcessor.ShowForm(new Inventec.Common.WordContent.WordContentADO()
                        {
                            EmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO,
                            FileName = fileResult,
                            TemplateFileName = fileTemplate,
                            TemplateKey = TemplateKey
                        });
                    }
                    else
                    {
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, PrintConfig.URI_API_SAR, PrintConfig.Language, PrintConfig.TemnplatePathFolder);
                        richEditorMain.RunPrintEditor(fileTemplate, printType.PRINT_TYPE_NAME, null, TemplateKey, null, (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileResult), fileResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileTemplate), fileTemplate));
                    //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, PrintConfig.URI_API_SAR, PrintConfig.Language, PrintConfig.TemnplatePathFolder);

                    //string saveFilePathTmp = Utils.GenerateTempFileWithin(fileResult, ".pdf");
                    //if (new Inventec.Common.FileConvert.ConvertByPDFNet().DocToPdfByPdfTron(fileResult, saveFilePathTmp))
                    //{
                    //    saveFilePath = saveFilePathTmp;
                    //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saveFilePath), saveFilePath));
                    //    richEditorMain.ShowPrintPreview(fileTemplate, saveFilePath, TemplateKey, (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
                    //}

                   
                    //richEditorMain.RunPrintTemplate(printType.PRINT_TYPE_CODE, fileResult, printType.PRINT_TYPE_NAME, null, null, TemplateKey, null, (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
                    
                }
                else
                {
                    throw new ArgumentNullException("streamResult is null");
                }
                Inventec.Common.Logging.LogSystem.Debug("PrintPreviewWord.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        protected bool PrintPreview(MemoryStream streamResult, string fileTemplate, Dictionary<string, object> TemplateKey)
        {
            try
            {
                if (streamResult != null && streamResult.Length > 0)
                {
                    streamResult.Position = 0;
                    switch (this.callPrintType)
                    {
                        case PrintConfig.CallPrintType.Flexcel:
                            Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(streamResult, this.printerName, fileTemplate, this.numCopy, (this.isPreview && previewType != MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow), this.isAllowExport, TemplateKey, this.eventLog, printDataBase.eventPrint, this.printDataBase.EmrInputADO, printLog, ShowLog, this.getNumOrderPrint, this.isAllowEditTemplateFile, this.isSingleCopy, previewType == MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, this.isPrintExceptionReason);
                            printProcess.SetDelegateOpenTutorial(this.showTutorialModule);
                            switch (previewType)
                            {
                                case MPS.ProcessorBase.PrintConfig.PreviewType.Show:
                                    printProcess.PrintPreviewShow();
                                    break;
                                case MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog:
                                    printProcess.PrintPreview();
                                    break;
                                case MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow:
                                    printProcess.Print();
                                    if (this.printDataBase != null && printDataBase.eventPrint != null)
                                        printDataBase.eventPrint();
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    throw new ArgumentNullException("streamResult is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        protected void SetCommonSingleKey()
        {
            try
            {
                singleValueDictionary.Add(CommonKey._PARENT_ORGANIZATION_NAME, PrintConfig.ParentOrganizationName);
                singleValueDictionary.Add(CommonKey._MEDI_ORG_CODE, PrintConfig.MediOrgCode);
                singleValueDictionary.Add(CommonKey._ORGANIZATION_NAME, PrintConfig.OrganizationName);
                singleValueDictionary.Add(CommonKey._ORGANIZATION_ADDRESS, PrintConfig.OrganizationAddress);
                System.DateTime now = System.DateTime.Now;
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_STR, now.ToString("dd/MM/yyyy HH:mm:ss"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_STR, now.ToString("dd/MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_STR, now.ToString("MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_STR, GlobalQuery.GetCurrentTimeSeparate(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_BEGIN_TIME_STR, GlobalQuery.GetCurrentTimeSeparateBeginTime(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToMonthSeparateString(now));
                singleValueDictionary.Add(CommonKey._CURRENT_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                singleValueDictionary.Add(CommonKey._CURRENT_LOGINNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                singleValueDictionary.Add(CommonKey._CURRENT_COMPUTER_NAME, System.Environment.MachineName);
                if (PrintConfig.OrganizationLogo != null && PrintConfig.OrganizationLogo.Count() > 0)
                {
                    singleValueDictionary.Add(CommonKey._CURRENT_LOGO, PrintConfig.OrganizationLogo);
                }
                if (PrintConfig.OrganizationLogoUri != null)
                {
                    singleValueDictionary.Add(CommonKey._CURRENT_LOGO_URI, PrintConfig.OrganizationLogoUri);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetSingleKey(string key, object value)
        {
            try
            {
                if (!singleValueDictionary.ContainsKey(key))
                {
                    singleValueDictionary.Add(key, value);
                }
                else
                {
                    if (singleValueDictionary[key] == null) singleValueDictionary[key] = value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetSingleKey(KeyValue value)
        {
            try
            {
                if (!singleValueDictionary.ContainsKey(value.KEY))
                {
                    singleValueDictionary.Add(value.KEY, value.VALUE);
                }
                else
                {
                    if (singleValueDictionary[value.KEY] == null) singleValueDictionary[value.KEY] = value.VALUE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetNumOrderKey(long numOrder)
        {
            try
            {
                if (numOrder > 0) singleValueDictionary[CommonKey._CURRENT_NUM_ORDER_PRINT] = numOrder;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void AddObjectKeyIntoListkey<T>(T data)
        {
            try
            {
                AddObjectKeyIntoListkey(data, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddObjectKeyIntoListkey<T>(T data, bool isOveride)
        {
            try
            {
                AddObjectKeyIntoListkeyWithPrefix(data, null, isOveride);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddObjectKeyIntoListkeyWithPrefix<T>(T data, string prefix, bool isOveride)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (pi.GetGetMethod().IsVirtual) continue;

                        string keyName = "";

                        //có tiền tố thì không add 2 key nữa
                        if (!String.IsNullOrWhiteSpace(prefix))
                        {
                            keyName = prefix + pi.Name;
                        }
                        else if (pi.Name.StartsWith("TDL_"))
                        {
                            keyName = pi.Name.Substring(4);
                        }
                        else
                        {
                            keyName = pi.Name;
                        }

                        if (!singleValueDictionary.ContainsKey(keyName))
                        {
                            singleValueDictionary.Add(keyName, (data != null ? pi.GetValue(data) : null));
                        }
                        else
                        {
                            if (isOveride)
                                singleValueDictionary[keyName] = (data != null ? pi.GetValue(data) : null);
                        }

                        //add cả key TDL_ vào biểu mẫu
                        if (keyName != pi.Name && pi.Name.StartsWith("TDL_") && String.IsNullOrWhiteSpace(prefix))
                        {
                            if (!singleValueDictionary.ContainsKey(pi.Name))
                            {
                                singleValueDictionary.Add(pi.Name, (data != null ? pi.GetValue(data) : null));
                            }
                            else
                            {
                                if (isOveride)
                                    singleValueDictionary[pi.Name] = (data != null ? pi.GetValue(data) : null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected string LogDataServiceReq(string treatmentCode, string serviceReqCode, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                if (!String.IsNullOrWhiteSpace(serviceReqCode))
                {
                    result += string.Format("SERVICE_REQ_CODE: {0}. ", serviceReqCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        protected string LogDataExpMest(string treatmentCode, string expMestCode, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                if (!String.IsNullOrWhiteSpace(expMestCode))
                {
                    result += string.Format("EXP_MEST_CODE: {0}. ", expMestCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        protected string LogDataImpMest(string treatmentCode, string impMestCode, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                if (!String.IsNullOrWhiteSpace(impMestCode))
                {
                    result += string.Format("IMP_MEST_CODE: {0}. ", impMestCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        protected string LogDataTransaction(string treatmentCode, string transactionCode, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                if (!String.IsNullOrWhiteSpace(transactionCode))
                {
                    result += string.Format("TRANSACTION_CODE: {0}. ", transactionCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
