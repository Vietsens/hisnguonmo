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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.OtherFormAssServiceReq.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.WordContent;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ProcessorBase;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OtherFormAssServiceReq
{
    public partial class frmOtherFormAssServiceReq : FormBase
    {
        private OtherFormAssServiceReqADO inputADO;

        private HIS_TREATMENT treatment = null;
        private HIS_SERVICE_REQ serviceReq = null;
        private SAR_PRINT_TYPE currentPrintType = null;
        private List<SarPrintTypeAdo> printTypeTemplates = null;
        private List<SAR_PRINT> prints = null;


        public frmOtherFormAssServiceReq(Inventec.Desktop.Common.Modules.Module module, OtherFormAssServiceReqADO _InputADO)
            : base(module)
        {
            InitializeComponent();
            this.inputADO = _InputADO;
        }

        private void frmOtherFormAssServiceReq_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadData();
                LoadListFileToGrid();
                LoadJSonPrintOld();
                InitWordContentWithInputParam();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                if (this.inputADO != null)
                {
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.ID = this.inputADO.ServiceReqId;
                    List<HIS_SERVICE_REQ> lstServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, null);
                    this.serviceReq = lstServiceReq != null ? lstServiceReq.FirstOrDefault() : null;

                    if (this.serviceReq != null)
                    {
                        HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                        treatFilter.ID = this.serviceReq.TREATMENT_ID;
                        List<HIS_TREATMENT> lstTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, null);
                        this.treatment = lstTreatment != null ? lstTreatment.FirstOrDefault() : null;

                    }
                }

                if (RichEditorConfig.PrintTypes != null && RichEditorConfig.PrintTypes.Count > 0 && this.inputADO != null && !String.IsNullOrEmpty(this.inputADO.PrintTypeCode))
                {
                    this.currentPrintType = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == this.inputADO.PrintTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadListFileToGrid()
        {
            try
            {
                gridControlTemplate.DataSource = null;
                List<string> PrintTypeCodeList = new List<string>();

                if (this.inputADO != null && !String.IsNullOrEmpty(this.inputADO.PrintTypeCode))
                {
                    PrintTypeCodeList.Add(this.inputADO.PrintTypeCode);
                }

                this.printTypeTemplates = new List<SarPrintTypeAdo>();

                foreach (var item in PrintTypeCodeList)
                {
                    var printTypeByCodes = RichEditorConfig.PrintTypes.Where(o => (o.PRINT_TYPE_CODE ?? "").ToLower() == item.ToLower()).ToList();
                    if (printTypeByCodes != null && printTypeByCodes.Count > 0)
                    {
                        if (!Directory.Exists(System.IO.Path.Combine(FileLocalStore.Rootpath, item)))
                        {
                            MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_FOLDER_TUONG_UNG_VOI_MA_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + item, item));
                            throw new DirectoryNotFoundException("Khong ton tai folder chua bieu mau in: " + System.IO.Path.Combine(FileLocalStore.Rootpath, item) + " . " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                        }

                        int? hasFolderAndFoundedFile = null;
                        ProcessDirectoryRoot(System.IO.Path.Combine(FileLocalStore.Rootpath, item), item, ref hasFolderAndFoundedFile);
                        if (hasFolderAndFoundedFile == null || (hasFolderAndFoundedFile.HasValue && hasFolderAndFoundedFile == 1))
                            ProcessFile(System.IO.Path.Combine(FileLocalStore.Rootpath, item), item);

                        if (printTypeTemplates == null || printTypeTemplates.Count == 0)
                        {
                            MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_BIEU_MAU_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + item, item));
                            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            string path1 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                            Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc file template. path(Location) ="
                                + path + " - path(CodeBase)=" + path1);
                            Inventec.Common.Logging.LogSystem.Debug("FileLocalStore.rootpath =" + FileLocalStore.Rootpath);
                        }
                    }
                    else
                    {
                        //Inventec.Common.Logging.LogSystem.Info("Khong lay duoc sarprint type theo dieu kien tim kiem. fileName=" + this.fileName + " - printTypeCode=" + this.printTypeCode);
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_MA_IN, item));
                    }
                }
                gridControlTemplate.DataSource = printTypeTemplates;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDirectoryRoot(string targetDirectory, string printTypeCode, ref int? hasFolderAndFoundedFile)
        {
            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (hasFolderAndFoundedFile == null)
                        {
                            hasFolderAndFoundedFile = 1;
                        }

                        //#19105
                        //Hiện tại: Trong bệnh viện có nhiều biểu mẫu in ấn, mỗi khoa sẽ có các biểu mẫu in ấn đặc thù với từng khoa.
                        //VD: Đơn thuốc nhưng ở khoa A sử dụng biểu mẫu khác, khoa B dùng biểu mẫu khác.
                        //Bất cập: Có rất nhiều biểu mẫu ở các khoa. Mỗi khoa sử dụng một biểu mẫu riêng.
                        //Nếu dựng thêm AUP thì không thể kiểm soát hết được version vì sẽ có nhiều AUP.
                        //Mong muốn: Đội code có giải pháp để đáp ứng việc này.
                        //TODO
                        //bool isFindedTemplate = false;
                        if (RichEditorConfig.WorkingRoom != null)
                        {
                            string depaCode = subdirectory.Substring(subdirectory.LastIndexOf("\\") + 1);
                            if (RichEditorConfig.WorkingRoom.DEPARTMENT_CODE == depaCode)
                            {
                                hasFolderAndFoundedFile = 2;
                                ProcessFile(subdirectory, printTypeCode);
                                ProcessDirectory(subdirectory, printTypeCode);

                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => depaCode), depaCode)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => subdirectory), subdirectory)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RichEditorConfig.WorkingRoom), RichEditorConfig.WorkingRoom));
                            }
                        }
                        else
                        {
                            ProcessFile(subdirectory, printTypeCode);
                            ProcessDirectory(subdirectory, printTypeCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
            .Where(s => s.EndsWith(".docx") || s.EndsWith(".doc")).ToArray();
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

        private void LoadJSonPrintOld()
        {
            try
            {
                gridControlPrint.DataSource = null;
                this.prints = new List<SAR_PRINT>();
                if (this.serviceReq != null && !string.IsNullOrWhiteSpace(this.serviceReq.JSON_PRINT_ID) && this.currentPrintType != null)
                {
                    SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();

                    var printIds = PrintIdByJsonPrint(this.serviceReq.JSON_PRINT_ID);
                    if (printIds != null && printIds.Count > 0)
                    {
                        printFilter.IDs = printIds;
                        printFilter.ORDER_FIELD = "CREATE_TIME";
                        printFilter.ORDER_DIRECTION = "DESC";
                        this.prints = new BackendAdapter(new CommonParam()).Get<List<SAR_PRINT>>("api/SarPrint/Get", ApiConsumers.SarConsumer, printFilter, new CommonParam());
                    }
                    if (this.prints != null && this.prints.Count > 0)
                    {
                        this.prints = this.prints.Where(o => (o.PRINT_TYPE_ID.HasValue && o.PRINT_TYPE_ID.Value == this.currentPrintType.ID)).ToList();
                    }

                    gridControlPrint.DataSource = this.prints;
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
                        long printId = Convert.ToInt64(id);
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

        private void InitWordContentWithInputParam()
        {
            try
            {
                if ((this.prints == null || this.prints.Count == 0) && this.printTypeTemplates != null && this.printTypeTemplates.Count == 1)
                {
                    CreateClick(this.printTypeTemplates[0]);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CreateClick(SarPrintTypeAdo data)
        {
            Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
            if (this.inputADO != null && this.inputADO.DicParam != null && this.inputADO.DicParam.Count > 0)
                dicParamPlus = this.inputADO.DicParam;
            this.SetCommonSingleKey(dicParamPlus);

            ProcessWordAndShowWordContentResult(data, dicParamPlus);
        }

        protected void SetCommonSingleKey(Dictionary<string, object> dicParamPlus)
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

        private void ProcessWordAndShowWordContentResult(SarPrintTypeAdo sarPrintType, Dictionary<string, object> dicParamPlus)
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((serviceReq != null ? serviceReq.TDL_TREATMENT_CODE : ""), sarPrintType.PRINT_TYPE_CODE, currentModuleBase != null ? currentModuleBase.RoomId : 0);

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

                    wordContentADO.ActUpdateReference = ActUpdateServiceReqJsonPrint;
                    wordContentADO.TemplateFileName = sarPrintType.FILE_NAME;//TODO

                    wordContentProcessor.ShowForm(wordContentADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonCreate_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                SarPrintTypeAdo data = (SarPrintTypeAdo)gridViewTemplate.GetFocusedRow();
                if (data != null)
                {
                    this.CreateClick(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTemplate_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
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

        private void gridViewTemplate_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SarPrintTypeAdo data = (SarPrintTypeAdo)gridViewTemplate.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CREATE")
                        {
                            if (this.serviceReq == null || this.treatment == null)
                            {
                                e.RepositoryItem = repositoryItemButtonCreate_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonCreate_Enable;
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

        private void gridViewPrint_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPrint_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SAR_PRINT data = (SAR_PRINT)gridViewPrint.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EDIT")
                        {
                            if (this.serviceReq == null || this.treatment == null)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_Enable;
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

        private void repositoryItemButtonView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR_PRINT)gridViewPrint.GetFocusedRow();
                if (sarPrint != null)
                {
                    this.ViewClick(sarPrint);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR_PRINT)gridViewPrint.GetFocusedRow();
                if (sarPrint != null)
                {
                    EditClick(sarPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ViewClick(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                Inventec.Common.SignLibrary.ADO.InputADO signADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.serviceReq != null ? this.serviceReq.TDL_TREATMENT_CODE : ""), this.inputADO.PrintTypeCode, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);

                Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
                if (inputADO.DicParam != null && inputADO.DicParam.Count > 0)
                    dicParamPlus = inputADO.DicParam;
                this.SetCommonSingleKey(dicParamPlus);

                WordContentProcessor wordContentProcessor = new WordContentProcessor();
                WordContentADO wordContentADO = new WordContentADO();
                wordContentADO.EmrInputADO = signADO;
                wordContentADO.TemplateKey = dicParamPlus;
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }
                wordContentADO.SarPrintType = ptRow;
                wordContentADO.OldSarPrint = sarPrint;
                wordContentADO.IsViewOnly = true;
                wordContentProcessor.ShowForm(wordContentADO);
            }
        }

        void EditClick(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                Inventec.Common.SignLibrary.ADO.InputADO signADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.serviceReq != null ? this.serviceReq.TDL_TREATMENT_CODE : ""), this.inputADO.PrintTypeCode, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);

                Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
                if (inputADO.DicParam != null && inputADO.DicParam.Count > 0)
                    dicParamPlus = inputADO.DicParam;
                this.SetCommonSingleKey(dicParamPlus);

                WordContentProcessor wordContentProcessor = new WordContentProcessor();
                WordContentADO wordContentADO = new WordContentADO();
                wordContentADO.EmrInputADO = signADO;
                wordContentADO.TemplateKey = dicParamPlus;
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }
                wordContentADO.SarPrintType = ptRow;
                wordContentADO.OldSarPrint = sarPrint;
                wordContentADO.ActUpdateReference = ActUpdateServiceReqJsonPrint;
                wordContentADO.TemplateFileName = sarPrint.TITLE;

                wordContentProcessor.ShowForm(wordContentADO);

                //frmPrintEditor printEditor = new frmPrintEditor(sarPrint, UpdateTreatmentJsonPrint);
                //printEditor.ShowDialog();
            }
        }

        void ActUpdateServiceReqJsonPrint(SAR_PRINT sarPrintCreated)
        {
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.serviceReq != null);
                if (valid)
                {
                    HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                    List<long> jsonPrintIds = GetListPrintIdByTreatment();
                    ProcessTreatmentExecuteForUpdateJsonPrint(req, jsonPrintIds, sarPrintCreated);
                    SaveServiceReq(req);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReq(HIS_SERVICE_REQ req)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                req.ID = this.serviceReq.ID;
                var rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, req, param);
                if (rs != null)
                {
                    LoadData();
                    LoadJSonPrintOld();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessTreatmentExecuteForUpdateJsonPrint(HIS_SERVICE_REQ req, List<long> jsonPrintId, SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.serviceReq != null)
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
                    req.JSON_PRINT_ID = printIds;
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
            if (this.serviceReq != null && !String.IsNullOrEmpty(this.serviceReq.JSON_PRINT_ID))
            {
                var arrIds = this.serviceReq.JSON_PRINT_ID.Split(',', ';');
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
            return result;
        }
    }
}
