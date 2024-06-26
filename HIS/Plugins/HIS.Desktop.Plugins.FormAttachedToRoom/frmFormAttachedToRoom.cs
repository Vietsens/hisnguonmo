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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.WordContent;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase;
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

namespace HIS.Desktop.Plugins.FormAttachedToRoom
{
    public partial class frmFormAttachedToRoom : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<SarPrintTypeAdo> printTypeTemplates;
        SAR_PRINT_TYPE currentPrintType;
        List<SAR.EFMODEL.DataModels.SAR_PRINT> sarPrints;
        internal Dictionary<string, object> dicInputParam;
        V_HIS_ROOM currentRoom;

        Inventec.Common.SignLibrary.ADO.InputADO inputADO;
        internal Dictionary<string, System.Drawing.Image> dicImage;

        Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, System.Drawing.Image>();

        //Inventec.Common.SignLibrary.ADO.InputADO inputADO;

        Inventec.Common.RichEditor.RichEditorStore richEditorMainView;

        public frmFormAttachedToRoom()
        {
            InitializeComponent();
        }

        public frmFormAttachedToRoom(Inventec.Desktop.Common.Modules.Module _currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmFormAttachedToRoom_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                LoadData();
                LoadJSonPrintOld();
                InitWordContentWithInputParam();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewForm_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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

        private void repositoryItemButton__Create_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboRoom.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn phòng xử lý.", "Thông báo");
                    return;
                }

                var data = (SarPrintTypeAdo)gridViewListFile.GetFocusedRow();
                if (data != null)
                {
                    if (data.PRINT_TYPE_CODE.ToLower() == "Mps000477".ToLower())
                    {
                        TaoMauPhieuGanTheoPhong(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoMauPhieuGanTheoPhong(SarPrintTypeAdo data)
        {
            string extension = Path.GetExtension(data.FILE_NAME);
            if (extension.Equals(".doc") || extension.Equals(".docx"))
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", data.PRINT_TYPE_CODE, currentRoom != null ? currentRoom.ID : 0);

                frmPrintEditor printEditor = new frmPrintEditor(data.FILE_NAME, "Mps000477", CreateRoomJsonPrint, dicParamPlus, this.dicImagePlus, inputADO, data.ID);
                printEditor.ShowDialog();
            }
            else
            {
                MessageBox.Show("Sai định dạng file . Chỉ hỗ trợ định dạng file .doc, .docx");
            }
        }

        void CreateClickByNew(SarPrintTypeAdo data)
        {
            //TODO
            dicParamPlus = new Dictionary<string, object>();
            if (dicInputParam != null && dicInputParam.Count > 0)
                dicParamPlus = dicInputParam;
            //this.SetDicParamPatient(ref dicParamPlus);
            this.SetCommonSingleKey();
            //this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);
            if (this.currentRoom != null)
            {
                TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_ROOM>(currentRoom, dicParamPlus);
            }

            ProcessWordAndShowWordContentResult(data);
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", sarPrintType.PRINT_TYPE_CODE, currentRoom != null ? currentRoom.ID : 0);

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

                    wordContentADO.ActUpdateReference = ActUpdateRoomJsonPrint;
                    wordContentADO.TemplateFileName = sarPrintType.FILE_NAME;//TODO

                    wordContentProcessor.ShowForm(wordContentADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ActUpdateRoomJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.currentRoom != null);
                if (valid)
                {
                    V_HIS_ROOM hisRoom = new V_HIS_ROOM();
                    var listOldPrintIdOfRooms = GetListPrintIdByRoom();
                    ProcessRoomExecuteForUpdateJsonPrint(ref hisRoom, listOldPrintIdOfRooms, sarPrintCreated);
                    SaveRoom(hisRoom);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCommonSingleKey()
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

        private void repositoryItemButton__View_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewDetail.GetFocusedRow();
                if (sarPrint != null)
                {
                    ViewClick(sarPrint);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ViewClick(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                string printTypeCode = "Mps000477";
                this.richEditorMainView = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (sarPrint != null)
                {
                    inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentRoom != null ? currentRoom.ROOM_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                    List<long> currentPrintIds = new List<long>();
                    currentPrintIds.Add(sarPrint.ID);
                    this.richEditorMainView.RunPrint(currentPrintIds, dicInputParam, dicImage, null, ShowPrintPreview);
                }
            }
        }

        private void ShowPrintPreview(byte[] CONTENT_B)
        {
            try
            {
                this.richEditorMainView.ShowPrintPreview(CONTENT_B, null, dicInputParam, dicImage, true, this.inputADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewDetail.GetFocusedRow();
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

        void EditClick(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }

                var inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentRoom != null ? currentRoom.ROOM_CODE : ""), ptRow != null ? ptRow.PRINT_TYPE_CODE : "", currentModule != null ? currentModule.RoomId : 0);

                frmPrintEditor printEditor = new frmPrintEditor(sarPrint, UpdateRoomJsonPrint, inputADO);
                printEditor.ShowDialog();
            }
        }

        bool UpdateRoomJsonPrint(SAR_PRINT sarPrintCreated)
        {
            bool rs = false;
            try
            {
                rs = sarPrintCreated != null;
                MessageManager.Show(this,new CommonParam(), rs);
                LoadJSonPrintOld();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void gridViewListFile_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SarPrintTypeAdo data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewListFile.GetDataSourceRowIndex(e.RowHandle);
                    data = (SarPrintTypeAdo)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ACTION_CREATE")
                        {
                            if (this.currentRoom != null && (data.PRINT_TYPE_CODE.ToLower() == "Mps000477".ToLower()
                                ))
                            {
                                e.RepositoryItem = repositoryItemButton__Create;
                            }
                            else if (this.currentRoom == null || (this.currentRoom.IS_PAUSE == (short)1 && data.PRINT_TYPE_CODE.ToLower() == "Mps000477".ToLower()))
                            {
                                e.RepositoryItem = repositoryItemButton__Create__Disabled;
                            }
                            else if (this.currentRoom != null)
                            {
                                e.RepositoryItem = repositoryItemButton__Create;
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

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewDetail.GetFocusedRow();
                if (sarPrint != null)
                {
                    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        var rs = new BackendAdapter(param).Post<bool>("api/SarPrint/Delete", ApiConsumers.SarConsumer, sarPrint, param);
                        if (rs)
                        {
                            success = true;
                            LoadJSonPrintOld();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        bool CreateRoomJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrinmetCreated)
        {
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sarPrinmetCreated__", sarPrinmetCreated));
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.currentRoom != null);
                if (valid)
                {
                    V_HIS_ROOM hisRoom = new V_HIS_ROOM();
                    var listOldPrintIdOfRoom = GetListPrintIdByRoom();
                    ProcessRoomExecuteForUpdateJsonPrint(ref hisRoom, listOldPrintIdOfRoom, sarPrinmetCreated);
                    SaveRoom(hisRoom);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        List<long> GetListPrintIdByRoom()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.currentRoom != null)
                {
                    if (!String.IsNullOrEmpty(this.currentRoom.JSON_PRINT_ID))
                    {
                        var arrIds = this.currentRoom.JSON_PRINT_ID.Split(',', ';');
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

        private void ProcessRoomExecuteForUpdateJsonPrint(ref V_HIS_ROOM room, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
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
                room.JSON_PRINT_ID = printIds;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveRoom(V_HIS_ROOM hisRoom)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                HisRoomSDO sdo = new HisRoomSDO();
                sdo.RoomId = currentRoom.ID;
                sdo.JsonPrintId = hisRoom.JSON_PRINT_ID;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisRoomSDO__", sdo));
                var resultSDO = new BackendAdapter(param).Post<HIS_ROOM>("api/HisRoom/UpdateJsonPrintId", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                if (resultSDO != null)
                {
                    success = true;
                    this.currentRoom.JSON_PRINT_ID = resultSDO.JSON_PRINT_ID;
                    LoadData();
                    LoadJSonPrintOld();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void cboRoomType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit txt = sender as GridLookUpEdit;
                if (txt.EditValue != null)
                    InitComboRoom(cboRoom, BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.ROOM_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(txt.EditValue.ToString())).ToList());
                else
                {
                    InitComboRoom(cboRoom, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoom2(object obj)
        {
            try
            {

                cboRoom2.Properties.View.Columns.Clear();

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboRoom2.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboRoom2);
                cboRoom2.Properties.Tag = gridCheck;
                cboRoom2.Properties.View.OptionsSelection.MultiSelect = true;
                

                cboRoom2.Properties.DataSource = obj;
                cboRoom2.Properties.DisplayMember = "ROOM_NAME";
                cboRoom2.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboRoom2.Properties.View.Columns.AddField("ROOM_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboRoom2.Properties.View.Columns.AddField("ROOM_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboRoom2.Properties.PopupFormWidth = 200;
                cboRoom2.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboRoom2.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboRoom2.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboRoom2.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__cboRoom2(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (V_HIS_ROOM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;

                        typeName += er.ROOM_NAME + ",";
                    }
                    cboRoom2.Text = typeName;
                    cboRoom2.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoom(GridLookUpEdit cbo, object dataSource)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbo, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentRoom = cboRoom.EditValue != null ? BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == Convert.ToInt64(cboRoom.EditValue)).FirstOrDefault() : null;
                LoadListFileToGrid();
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
                this.sarPrints = new List<SAR.EFMODEL.DataModels.SAR_PRINT>();

                SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();

                if (this.currentPrintType != null)
                {
                    printFilter.PRINT_TYPE_ID = this.currentPrintType.ID;
                    printFilter.ORDER_FIELD = "CREATE_TIME";
                    printFilter.ORDER_DIRECTION = "DESC";
                    if (dtFromTime.EditValue != null && dtToTime.EditValue != null)
                    {
                        printFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");
                        printFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtToTime.DateTime.ToString("yyyyMMdd") + "235959");
                    }

                    this.sarPrints = new BackendAdapter(new CommonParam())
                    .Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>("api/SarPrint/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, printFilter, new CommonParam());

                    GridCheckMarksSelection gridCheckMarkBusiness = cboRoom2.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<V_HIS_ROOM> rooms = new List<V_HIS_ROOM>();
                        foreach (V_HIS_ROOM rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !rooms.Contains(rv))
                                rooms.Add(rv);
                        }


                        var printIds = PrintIdByJsonPrint(rooms);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("printIds__", printIds));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sarPrints__", sarPrints));
                        sarPrints = printIds.Count() > 0 ? sarPrints.Where(o => printIds.Contains(o.ID)).ToList() : null;
                    }

                    gridControlDetail.DataSource = this.sarPrints;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void cboRoomType2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRoomType2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboRoom2.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.ClearSelection(cboRoom2.Properties.View);
                    cboRoom2.EditValue = null;
                    cboRoom2.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                LoadJSonPrintOld();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom2_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (V_HIS_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.ROOM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomType2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit txt = sender as GridLookUpEdit;
                if (txt.EditValue != null)
                    InitComboRoom2(BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(txt.EditValue.ToString())).ToList());
                else
                {
                    InitComboRoom2(null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
