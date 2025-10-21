using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScaned
{
    public partial class frmListEmrDocumentScaned : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;

        string creator = "";
        string fileName = "";
        string filePath = "";
        string fileExtension = "";
        string url = "";
        private string tempDocFilePathPDF = null;
        private string tempDocFilePathDoc = null;

        internal string treatmentCode;

        EMR.Desktop.Plugins.ListEmrDocumentScaned.UC.UCDocument UcDocument = new UC.UCDocument();
        public frmListEmrDocumentScaned(Inventec.Desktop.Common.Modules.Module module, string tmCode) : base(module)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = module;

                this.treatmentCode = tmCode;

                if (this.treatmentCode != "")
                {
                    txtSearch.Text = this.treatmentCode;
                    txtSearch.Enabled = false;
                }
                try
                {
                    string iconPath = System.IO.Path.Combine
                        (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings
                        ["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void frmListEmrDocumentScaned_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
                SetDefaultFocus();
                SetResourceByLanguageKey();
                ValidateForm();
                //this.ActionType = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationControlTextEditLanguageName();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditLanguageName()
        {
            try
            {
                EMR.Desktop.Plugins.ListEmrDocumentScaned.Validation.ValidationTextEditName validRule = new EMR.Desktop.Plugins.ListEmrDocumentScaned.Validation.ValidationTextEditName();
                validRule.txtTen = txtTen;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTen, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetResourceByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResources = new ResourceManager("EMR.Desktop.Plugins.ListEmrDocumentScaned.Resources.Lang", typeof
                (EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScaned.frmListEmrDocumentScaned).Assembly);
                //layout
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.LayoutControl1.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.LayoutControl2.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.lcNote.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.lcNote.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnQuet.Text = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.btnQuet.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //bar button
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //column
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmListEmrDocumentScaned.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtNote.Text = "";
                txtTen.Text = "";
                this.panelControl1.Controls.Clear();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(Object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED>> apiResult = null;
                EmrDocumentScanedFilter filter = new EmrDocumentScanedFilter();
                //filter.KEY_WORD = txtSearch.Text.Trim();
                if (txtSearch.Text != null && !String.IsNullOrEmpty(txtSearch.Text))
                {
                    if (this.treatmentCode == "" || this.treatmentCode == null || String.IsNullOrEmpty(this.treatmentCode))
                        filter.TREATMENT_CODE__EXACT = txtSearch.Text.Trim();
                    else
                    {
                        filter.TREATMENT_CODE__EXACT = this.treatmentCode;
                    }
                }
                else
                    return;

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginDataUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED>>(RequestUriStore.EMR_GET,
                    ApiConsumers.EmrConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionEdit;
                var rowData = (EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveStreamToDocFile(Stream streamSource, string outputFilePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    streamSource.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Error saving stream to .doc file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangedDataRow(EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED data)
        {
            try
            {
                if (data != null)
                {
                    FillDataEditorControl(data);
                    //if (label2.Text == "") this.panelControl1.Controls.Clear();
                    // Xóa file tạm cũ nếu có
                    if ((!string.IsNullOrEmpty(tempDocFilePathDoc) && File.Exists(tempDocFilePathDoc)) ||
                            (!string.IsNullOrEmpty(tempDocFilePathPDF) && File.Exists(tempDocFilePathPDF)))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(tempDocFilePathDoc) && File.Exists(tempDocFilePathDoc))
                                File.Delete(tempDocFilePathDoc);
                        }
                        catch { }
                        tempDocFilePathDoc = null;
                    }
                    string output = Utils.GenerateTempFileWithin();
                    string path = Utils.GenerateTempFolderWithin();
                    var streamSource = Inventec.Fss.Client.FileDownload.GetFile(data.URL);
                    streamSource.Position = 0;
                    Utils.ByteToFile(Utils.StreamToByte(streamSource), output);
                    InitDocument();
                    UcDocument.LoadDocument(output);
                    //string extension = ".doc";
                    //if (!string.IsNullOrEmpty(data.FILE_NAME))
                    //{
                    //    extension = Path.GetExtension(data.FILE_NAME).ToLower();
                    //    if (extension != ".doc" && extension != ".docx")
                    //        extension = ".doc";
                    //}
                    string outputFilePath = Path.Combine(path, Guid.NewGuid().ToString());

                    //SaveStreamToDocFile(streamSource, outputFilePath);
                    //InitDocument();
                    //UcDocument.LoadDocument(outputFilePath);

                    positionHandle = -1;
                    tempDocFilePathDoc = outputFilePath;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataEditorControl(EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED data)
        {
            try
            {
                if (data != null)
                {
                    txtNote.Text = data.NOTE;
                    txtTen.Text = data.FILE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionEdit;
                var rowData = (EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled)
                    return;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED updateDTO = new EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED>(RequestUriStore.EMR_CREATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED>(RequestUriStore.EMR_UPDATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateDTOFromDataForm(ref EMR_DOCUMENT_SCANED currentDTO)
        {
            try
            {
                currentDTO.FILE_NAME = txtTen.Text;
                currentDTO.NOTE = txtNote.Text;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        private void LoadCurrent(long currentID, ref EMR_DOCUMENT_SCANED currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentScanedFilter filter = new EmrDocumentScanedFilter();
                filter.ID = currentID;
                currentDTO = new BackendAdapter(param).Get<List<EMR_DOCUMENT_SCANED>>(RequestUriStore.EMR_GET, ApiConsumers.EmrConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED pData = (EMR.EFMODEL.DataModels.EMR_DOCUMENT_SCANED)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "COL_STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDocument()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.UcDocument != null)
                {
                    this.UcDocument.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.UcDocument);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnQuet_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                string tmCode = txtSearch.Text.ToString();
                CommonParam param = new CommonParam();
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    DialogResult rs = DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Không có mã hồ sơ để quét văn bản.",
                        Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(
                            Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK
                    );

                    if (rs == DialogResult.OK)
                    {
                        return;
                    }
                }

                WaitingManager.Show();
                EmrDocumentScanedFilter filter = new EmrDocumentScanedFilter();
                var apiResult = new BackendAdapter(param).Post<List<EMR_DOCUMENT_SCANED>>(
                                RequestUriStore.EMR_SCAN,
                                ApiConsumers.EmrConsumer,
                                tmCode,
                                param);
                if (apiResult != null)
                {
                    success = true;
                    FillDataToControl();
                    SetDefaultValue();
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
