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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.OtherFormAssTreatment.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.WordContent;
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

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long TreatmentId;
        V_HIS_PATIENT Patient { get; set; }
        V_HIS_TREATMENT Treatment { get; set; }
        List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRooms { get; set; }
        List<SarPrintTypeAdo> printTypeTemplates;
        OtherFormAssTreatmentInputADO otherFormAssTreatmentInputADO;
        SAR_PRINT_TYPE currentPrintType;
        List<SAR.EFMODEL.DataModels.SAR_PRINT> sarPrints;
        internal Dictionary<string, object> dicInputParam;

        Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, System.Drawing.Image>();

        Inventec.Common.SignLibrary.ADO.InputADO inputADO;

        Inventec.Common.RichEditor.RichEditorStore richEditorMainView;
        internal Dictionary<string, System.Drawing.Image> dicImage;

        public frmOtherFormAssTreatment()
        {
            InitializeComponent();
        }

        public frmOtherFormAssTreatment(Inventec.Desktop.Common.Modules.Module _currentModule, OtherFormAssTreatmentInputADO inputADO)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
                this.otherFormAssTreatmentInputADO = inputADO;
                this.TreatmentId = inputADO != null ? inputADO.TreatmentId : 0;
                this.dicInputParam = inputADO != null ? inputADO.DicParam : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmOtherFormAssTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                LoadData();
                LoadListFileToGrid();
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
                var data = (SarPrintTypeAdo)gridViewListFile.GetFocusedRow();
                if (data != null)
                {
                    if (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000277.ToLower())
                    {
                        TaoBieuMauBenhAnYHocCoTruyenClick(data);
                    }
                    else if (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000380.ToLower())
                    {
                        TaoBieuMauMps000380Click(data);
                    }
                    else if (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000379.ToLower())
                    {
                        TaoBieuMauMps000379Click(data);
                    }
                    else if (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000378.ToLower())
                    {
                        TaoBieuMauMps000378Click(data);
                    }
                    else if (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000377.ToLower())
                    {
                        TaoBieuMauMps000377Click(data);
                    }
                    else
                    {
                        CreateClickByNew(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
            if (this.Treatment != null)
            {
                TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);
                dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(this.Treatment.TDL_PATIENT_DOB, "", "", "", "", this.Treatment.IN_TIME));
            }

            if (this.Patient != null)
            {
                this.SetDicParamPatient(ref dicParamPlus);
            }

            ProcessWordAndShowWordContentResult(data);
        }

        void CreateClickByOld(SarPrintTypeAdo data)
        {
            string extension = Path.GetExtension(data.FILE_NAME);
            if (extension.Equals(".doc") || extension.Equals(".docx"))
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), data.PRINT_TYPE_CODE, currentModule != null ? currentModule.RoomId : 0);

                frmPrintEditor printEditor = new frmPrintEditor(data.FILE_NAME, "Biểu mẫu khác__", UpdateTreatmentJsonPrint, dicParamPlus, this.dicImagePlus, inputADO);
                printEditor.ShowDialog();
            }
            else
            {
                MessageBox.Show("Sai định dạng file . Chỉ hỗ trợ định dạng file .doc, .docx");
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
                    var otherFormAssTreatment__FormType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.OtherFormAssTreatment.Base.HisConfigKeys.HIS_CONFIG_KEY__OtherFormAssTreatment__FormType);
                    if ((!String.IsNullOrEmpty(otherFormAssTreatment__FormType) && otherFormAssTreatment__FormType == "1") || (sarPrint.PRINT_TYPE_ID ?? 0) > 0)
                    {
                        ViewClickByNew(sarPrint);
                    }
                    else
                        ViewClickByOld(sarPrint);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ViewClickByOld(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                string printTypeCode = PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000277;
                if (otherFormAssTreatmentInputADO != null && !String.IsNullOrWhiteSpace(otherFormAssTreatmentInputADO.PrintTypeCode))
                    printTypeCode = otherFormAssTreatmentInputADO.PrintTypeCode;
                this.richEditorMainView = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (sarPrint != null)
                {
                    inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
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

        void ViewClickByNew(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                string printTypeCode = PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000277;
                if (otherFormAssTreatmentInputADO != null && !String.IsNullOrWhiteSpace(otherFormAssTreatmentInputADO.PrintTypeCode))
                    printTypeCode = otherFormAssTreatmentInputADO.PrintTypeCode;
                this.inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                dicParamPlus = new Dictionary<string, object>();
                if (dicInputParam != null && dicInputParam.Count > 0)
                    dicParamPlus = dicInputParam;
                this.SetDicParamPatient(ref dicParamPlus);
                this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);
                if (this.Treatment != null)
                {
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);
                }

                WordContentProcessor wordContentProcessor = new WordContentProcessor();
                WordContentADO wordContentADO = new WordContentADO();
                wordContentADO.EmrInputADO = this.inputADO;
                wordContentADO.TemplateKey = dicParamPlus;
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }
                wordContentADO.SarPrintType = ptRow;
                wordContentADO.OldSarPrint = sarPrint;
                //wordContentADO.ActUpdateReference = ActUpdateTreatmentJsonPrint;
                //wordContentADO.TemplateFileName = sarPrint.TITLE;
                wordContentADO.IsViewOnly = true;

                wordContentProcessor.ShowForm(wordContentADO);
                //this.richEditorMainView.RunPrint(currentPrintIds, dicParam, dicImage, null, ShowPrintPreview);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewDetail.GetFocusedRow();
                if (sarPrint != null)
                {
                    var otherFormAssTreatment__FormType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.OtherFormAssTreatment.Base.HisConfigKeys.HIS_CONFIG_KEY__OtherFormAssTreatment__FormType);
                    if ((!String.IsNullOrEmpty(otherFormAssTreatment__FormType) && otherFormAssTreatment__FormType == "1") || (sarPrint.PRINT_TYPE_ID ?? 0) > 0)
                    {
                        EditClickByNew(sarPrint);
                    }
                    else
                        EditClickByOld(sarPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void EditClickByOld(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }

                var inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), ptRow != null ? ptRow.PRINT_TYPE_CODE : "", currentModule != null ? currentModule.RoomId : 0);

                frmPrintEditor printEditor = new frmPrintEditor(sarPrint, UpdateTreatmentJsonPrint, inputADO);
                printEditor.ShowDialog();
            }
        }

        void EditClickByNew(SAR_PRINT sarPrint)
        {
            if (sarPrint != null)
            {
                dicParamPlus = new Dictionary<string, object>();
                if (dicInputParam != null && dicInputParam.Count > 0)
                    dicParamPlus = dicInputParam;
                this.SetDicParamPatient(ref dicParamPlus);
                this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);
                if (this.Treatment != null)
                {
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);
                }

                WordContentProcessor wordContentProcessor = new WordContentProcessor();
                WordContentADO wordContentADO = new WordContentADO();
                if (this.inputADO == null)
                {
                    SAR_PRINT_TYPE pt = null;
                    if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                    {
                        pt = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                    }

                    this.inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), pt != null ? pt.PRINT_TYPE_CODE : "", currentModule != null ? currentModule.RoomId : 0);
                }

                wordContentADO.EmrInputADO = this.inputADO;
                wordContentADO.TemplateKey = dicParamPlus;
                SAR_PRINT_TYPE ptRow = null;
                if (sarPrint.PRINT_TYPE_ID.HasValue && sarPrint.PRINT_TYPE_ID > 0)
                {
                    ptRow = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.ID == sarPrint.PRINT_TYPE_ID);
                }
                wordContentADO.SarPrintType = ptRow;
                wordContentADO.OldSarPrint = sarPrint;
                wordContentADO.ActUpdateReference = ActUpdateTreatmentJsonPrint;
                wordContentADO.TemplateFileName = sarPrint.TITLE;

                wordContentProcessor.ShowForm(wordContentADO);

                //frmPrintEditor printEditor = new frmPrintEditor(sarPrint, UpdateTreatmentJsonPrint);
                //printEditor.ShowDialog();
            }
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
                            if (this.Treatment != null && (data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000380.ToLower()
                                || data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000379.ToLower()
                                || data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000378.ToLower()
                                || data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000377.ToLower()))
                            {
                                e.RepositoryItem = repositoryItemButton__Create;
                            }
                            else if (this.Treatment == null || (this.Treatment.IS_PAUSE == (short)1 && data.PRINT_TYPE_CODE.ToLower() == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000379.ToLower()))
                            {
                                e.RepositoryItem = repositoryItemButton__Create__Disabled;
                            }
                            else if (this.Treatment != null)
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
                if (sarPrint != null && this.Treatment != null)
                {
                    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS_TREATMENT hisTreatment = new HIS_TREATMENT();

                        hisTreatment.JSON_PRINT_ID = this.Treatment.JSON_PRINT_ID.Replace(sarPrint.ID + ",", "");
                        //hàm tự gán id theo this.Treatment
                        SaveTreatment(hisTreatment);
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
