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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using DevExpress.XtraEditors;
using System.IO;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Modules;
using EMR.Desktop.Plugins.EmrSignDocumentList.Resources;
using EMR.Desktop.Plugins.EmrSignDocumentList.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Utilities.Extensions;
using EMR.SDO;

namespace EMR.Desktop.Plugins.EmrSignDocumentList
{
    public partial class UcEmrSignDocumentList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private string LoggingName = "";

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;

        private V_HIS_ROOM room;
        List<decimal> emrFlowIds = null;
        int documentSignFinished = 0;
        List<SignErrorADO> errorMessageWhileSign = null;
        List<V_EMR_DOCUMENT> docuemntWaitSigns = null;
        List<V_EMR_DOCUMENT> listEmrSelected;
        List<HIS_DEPARTMENT> departmentSelecteds;
        List<EMR_DOCUMENT_TYPE> emrDocumentTypeSelecteds;
        bool isInitializeComponent = false;
        #endregion

        #region ctor
        public UcEmrSignDocumentList()
        {
            InitializeComponent();
            LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
        }

        public UcEmrSignDocumentList(Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private method
        private void UcEmrSignDocumentList_Load(object sender, EventArgs e)
        {
            try
            {
                isInitializeComponent = true;
                this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitCboDepartment();
                InitCboEmrDocumentType();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                LoadEmrSignerFlowByLoginname();

                //Load du lieu
                FillDataToGrid();

                TxtKeyword.Focus();
                isInitializeComponent = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region Init combo
        private void InitCboDepartment()
        {
            InitCheck(cboDepartment, SelectionGrid__cboDepartment);
            InitCombo(cboDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>(), "DEPARTMENT_NAME", "DEPARTMENT_CODE");
        }
        private void InitCboEmrDocumentType()
        {
            InitCheck(cboEmrDocumentType, SelectionGrid__cboEmrDocumentType);
            InitCombo(cboEmrDocumentType, BackendDataWorker.Get<EMR_DOCUMENT_TYPE>(), "DOCUMENT_TYPE_NAME", "DOCUMENT_TYPE_CODE");
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(ValueMember);
                col1.VisibleIndex = 1;
                col1.Width = 50;
                col1.Caption = "ALL";

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = "Tất cả";

                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboDepartment(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> sgSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (MOS.EFMODEL.DataModels.HIS_DEPARTMENT rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.DEPARTMENT_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.departmentSelecteds = new List<HIS_DEPARTMENT>();
                    this.departmentSelecteds.AddRange(sgSelectedNews);
                }

                this.cboDepartment.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboEmrDocumentType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<EMR_DOCUMENT_TYPE> sgSelectedNews = new List<EMR_DOCUMENT_TYPE>();
                    foreach (EMR_DOCUMENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.DOCUMENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.emrDocumentTypeSelecteds = new List<EMR_DOCUMENT_TYPE>();
                    this.emrDocumentTypeSelecteds.AddRange(sgSelectedNews);
                }

                this.cboEmrDocumentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void LoadEmrSignerFlowByLoginname()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(LoggingName))
                {
                    EmrSignerFlowViewFilter filter = new EmrSignerFlowViewFilter();
                    filter.LOGINNAME__EXACT = this.LoggingName;
                    var emrSignerFlows = new BackendAdapter(new CommonParam()).Get<List<V_EMR_SIGNER_FLOW>>(URI.EmrSignerFlow.GET_VIEW, ApiConsumers.EmrConsumer, filter, null);
                    this.emrFlowIds = emrSignerFlows != null ? emrSignerFlows.Select(s => (decimal)s.FLOW_ID).ToList() : null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlDocument);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                btnViewAndSign.Enabled = false;
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>> apiResult = null;
                EMR.Filter.EmrDocumentViewFilter filter = new EMR.Filter.EmrDocumentViewFilter();
                SetFilter(ref filter);
                gridViewDocument.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET_VIEW, ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlDocument.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlDocument.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControlDocument.DataSource = null;
                }
                gridViewDocument.EndUpdate();


                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewDocument.EndUpdate();
            }
        }

        private void SetFilter(ref Filter.EmrDocumentViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (chkDelete.CheckState == CheckState.Unchecked)
                {
                    filter.IS_DELETE = false;
                }

                if (DtTimeFrom.EditValue != null && DtTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (DtTimeTo.EditValue != null && DtTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeTo.DateTime.ToString("yyyyMMdd") + "000000");

                if (!String.IsNullOrEmpty(TxtKeyword.Text))
                {
                    filter.KEY_WORD = TxtKeyword.Text.Trim();
                }

                filter.IS_STORE_TIME_NOT_NULL = false;

                if (rdoStatus__ChoKy.Checked)
                {
                    filter.NEXT_SIGNER__EXACT = LoggingName;
                    filter.IS_REJECTER_NOT_NULL = false;
                    if (room != null)
                    {
                        filter.NEXT_ROOM_CODE__NULL_OR_EXACT = room.ROOM_TYPE_CODE + "___" + room.ROOM_CODE;
                    }
                }
                else if (rdoStatus__TuChoiKy.Checked)
                {
                    filter.REJECTER__EXACT = LoggingName;
                }
                else if (rdoStatus__DaKy.Checked)
                {
                    filter.SIGNERS = LoggingName;
                }
                if(this.departmentSelecteds !=null && this.departmentSelecteds.Count > 0)
                {
                    filter.CURRENT_DEPARTMENT_CODEs = departmentSelecteds.Select(o => o.DEPARTMENT_CODE).ToList();
                }
                if (this.emrDocumentTypeSelecteds != null && this.emrDocumentTypeSelecteds.Count > 0)
                {
                    filter.DOCUMENT_TYPE_IDs = emrDocumentTypeSelecteds.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_EMR_DOCUMENT> GetDocumentSelected()
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                int[] selectRows = gridViewDocument.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var mediMatyTypeADO = (V_EMR_DOCUMENT)gridViewDocument.GetRow(selectRows[i]);
                        result.Add(mediMatyTypeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<ADO.StatusADO> LoadDataToComboStatus()
        {
            List<ADO.StatusADO> Result = new List<ADO.StatusADO>();
            try
            {
                ADO.StatusADO StatusADO_DangKy = new ADO.StatusADO();
                StatusADO_DangKy.ID = 1;
                StatusADO_DangKy.STT_NAME = Resources.ResourceLanguageManager.ChoKy;
                Result.Add(StatusADO_DangKy);

                ADO.StatusADO StatusADO_BiTuChoi = new ADO.StatusADO();
                StatusADO_BiTuChoi.ID = 2;
                StatusADO_BiTuChoi.STT_NAME = Resources.ResourceLanguageManager.TuChoiKy;
                Result.Add(StatusADO_BiTuChoi);

                ADO.StatusADO StatusADO_DaKy = new ADO.StatusADO();
                StatusADO_DaKy.ID = 3;
                StatusADO_DaKy.STT_NAME = Resources.ResourceLanguageManager.DaKy;
                Result.Add(StatusADO_DaKy);
            }
            catch (Exception ex)
            {
                Result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Result;
        }

        private void SetDefaultValueControl()
        {
            try
            {
                DtTimeFrom.DateTime = DateTime.Now;
                DtTimeTo.DateTime = DateTime.Now;
                rdoStatus__ChoKy.Checked = true;
                TxtKeyword.Text = "";
                TxtKeyword.Focus();
                TxtKeyword.SelectAll();
                btnViewAndSign.Enabled = false;
                this.departmentSelecteds = null;
                this.emrDocumentTypeSelecteds = null;
                cboDepartment.EditValue = null;
                cboEmrDocumentType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__BTN_REFRESH");
                this.BtnSearch.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__BTN_SEARCH");
                this.Gc_CreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_CREATE_TIME");
                this.Gc_Creator.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_CREATOR");
                this.Gc_Dob.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_DOB");
                this.Gc_DocumentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_DOCUMENT_CODE");
                this.Gc_DocumentName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_DOCUMENT_NAME");
                this.Gc_Gender.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_GENDER");
                this.Gc_Modifier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_MODIFIER");
                this.Gc_ModifyTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_MODIFY_TIME");
                this.Gc_PatientCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_PATIENT_CODE");
                this.Gc_PatientName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_PATIENT_NAME");
                this.Gc_Sign.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_SIGN");
                this.GC_StoreTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_STORE_TIME");
                this.Gc_TreatmentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_TREATMENT_CODE");
                this.Gc_Url.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__GC_URL");
                this.LciTimeFrom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__LCI_TIME_FROM");
                this.LciTimeTo.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__LCI_TIME_TO");
                this.NavBarCreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__BAR_CREATE_TIME");
                this.NavBarStatus.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__BAR_STARTUS");
                this.repositoryItemBtnSign.Buttons[0].ToolTip = this.repositoryItemBtnSignDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__RP_BTN_SIGN");
                this.TxtKeyword.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_SIGN_DOCUMENT_LIST__TXT_KEYWORD__NULL_VALUE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void TxtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSearch.Focus();
                FillDataToGrid();
                ResetStateSign();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                BtnRefresh.Focus();
                SetDefaultValueControl();
                FillDataToGrid();
                ResetStateSign();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDocument_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR.EFMODEL.DataModels.V_EMR_DOCUMENT data = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
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

        private void gridViewDocument_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)gridViewDocument.GetRow(e.RowHandle);
                    if (data == null) return;
                    if (e.Column.FieldName == "SIGN")
                    {
                        if (
                            (
                                data.NEXT_SIGNER == LoggingName
                                || (data.NEXT_FLOW_ID.HasValue && this.emrFlowIds != null && this.emrFlowIds.Contains(data.NEXT_FLOW_ID.Value)
                                    && (String.IsNullOrWhiteSpace(data.NEXT_ROOM) || data.NEXT_ROOM == (room.ROOM_TYPE_CODE + "___" + room.ROOM_CODE)))
                                || (data.IS_SIGN_PARALLEL == 1 && !String.IsNullOrEmpty(data.UN_SIGNERS) && data.UN_SIGNERS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(LoggingName))
                            )
                            && String.IsNullOrWhiteSpace(data.REJECTER) && (data.COUNT_RESIGN_FAILED == null || data.COUNT_RESIGN_FAILED <= 0))
                        {
                            e.RepositoryItem = repositoryItemBtnSign;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnSignDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDocument_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var row = (V_EMR_DOCUMENT)gridViewDocument.GetRow(hi.RowHandle);
                        if (row != null)
                        {
                            if (hi.Column.FieldName == "DETAIL")
                            {
                                #region ----- DETAIL -----
                                try
                                {
                                    CommonParam param = new CommonParam();
                                    var apiResult = GetEmrDocumentFile(row.ID, false, null, false, ref param);
                                    if (apiResult != null && apiResult.Count > 0)
                                    {
                                        //goi tool view
                                        var temFile = System.IO.Path.Combine(Application.StartupPath + "\\temp\\");
                                        if (!Directory.Exists(temFile)) Directory.CreateDirectory(temFile);

                                        temFile = System.IO.Path.Combine(temFile, string.Format("{0}.pdf", Guid.NewGuid()));
                                        List<string> joinStreams = new List<string>();

                                        List<MemoryStream> documentData = new List<MemoryStream>();
                                        foreach (var item in apiResult)
                                        {
                                            if (item.Extension.ToLower().Equals("pdf"))
                                            {
                                                string pdfAddFile = Utils.GenerateTempFileWithin();
                                                Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                                joinStreams.Add(pdfAddFile);
                                            }
                                        }

                                        Stream currentStream = File.Open(temFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                                        var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                                        var pages = new List<int>();
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                                        foreach (var file in joinStreams)
                                        {
                                            iTextSharp.text.pdf.PdfReader pdfReader = null;
                                            pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                                            pages = new List<int>();
                                            for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                                            {
                                                pages.Add(i);
                                            }
                                            pdfReader.SelectPages(pages);
                                            pdfConcat.AddPages(pdfReader);
                                            pdfReader.Close();
                                        }
                                        try
                                        {
                                            pdfConcat.Close();
                                        }
                                        catch { }

                                        foreach (var file in joinStreams)
                                        {
                                            try
                                            {
                                                File.Delete(file);
                                            }
                                            catch { }
                                        }

                                        SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(row.TREATMENT_CODE, row.DOCUMENT_CODE, row.DOCUMENT_NAME, moduleData.RoomId);

                                        // truyền paper
                                        if (row.WIDTH != null && row.HEIGHT != null && row.RAW_KIND != null)
                                        {
                                            inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(row.PAPER_NAME, (int)row.WIDTH, (int)row.HEIGHT);
                                            if (row.RAW_KIND != null)
                                            {
                                                inputADO.PaperSizeDefault.RawKind = (int)row.RAW_KIND;
                                            }
                                        }
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));

                                        if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
                                            libraryProcessor.ShowPopup(temFile, inputADO);
                                        else
                                        {
                                            XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                        }

                                        if (File.Exists(temFile)) File.Delete(temFile);
                                    }
                                    else
                                    {
                                        MessageManager.Show(this.ParentForm, param, false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "SIGN")
                            {
                                #region ----- SIGN -----
                                try
                                {
                                    if ((row.NEXT_SIGNER == LoggingName || (row.NEXT_FLOW_ID.HasValue && this.emrFlowIds != null && this.emrFlowIds.Contains(row.NEXT_FLOW_ID.Value) && (String.IsNullOrWhiteSpace(row.NEXT_ROOM) || row.NEXT_ROOM == (room.ROOM_TYPE_CODE + "___" + room.ROOM_CODE))) || (row.IS_SIGN_PARALLEL == 1 && !String.IsNullOrEmpty(row.UN_SIGNERS) && row.UN_SIGNERS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(LoggingName))) && String.IsNullOrWhiteSpace(row.REJECTER) && (row.COUNT_RESIGN_FAILED == null || row.COUNT_RESIGN_FAILED <= 0))
                                    {

                                        FormChooseSignType form = new FormChooseSignType(row, moduleData, room);
                                        if (form != null)
                                        {
                                            SignType type = new SignType();
                                            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "1")
                                            {
                                                type = SignType.USB;
                                            }
                                            else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "2")
                                            {
                                                type = SignType.HMS;
                                            }
                                            form.LoadDataForSign(type);
                                        }

                                        BtnSearch_Click(null, null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                                #endregion
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
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(long documentId, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.ID = documentId;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.IsView = null;
            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }
        //private void repositoryItemBtnSign_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        EMR.EFMODEL.DataModels.V_EMR_DOCUMENT row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewDocument.GetFocusedRow();
        //        if (row != null)
        //        {
        //            FormChooseSignType form = new FormChooseSignType(row, moduleData, room);
        //            if (form != null)
        //            {
        //                SignType type = new SignType();
        //                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "1")
        //                {
        //                    type = SignType.USB;
        //                }
        //                else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "2")
        //                {
        //                    type = SignType.HMS;
        //                }
        //                form.LoadDataForSign(type);
        //            }

        //            BtnSearch_Click(null, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Signs()
        {
            try
            {
                if (btnSigns.Enabled)
                {
                    btnSigns_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        //private void repositoryItemBtnView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        EMR.EFMODEL.DataModels.V_EMR_DOCUMENT row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewDocument.GetFocusedRow();
        //        if (row != null)
        //        {
        //            EMR.Filter.EmrVersionFilter versionFilter = new Filter.EmrVersionFilter();
        //            versionFilter.DOCUMENT_ID = row.ID;

        //            var listVersion = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_VERSION>>(EMR.URI.EmrVersion.GET, ApiConsumers.EmrConsumer, versionFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
        //            if (listVersion != null && listVersion.Count > 0)
        //            {
        //                EMR.EFMODEL.DataModels.EMR_VERSION version = new EFMODEL.DataModels.EMR_VERSION();
        //                version = listVersion.OrderByDescending(o => o.ID).FirstOrDefault();
        //                if (version != null && !String.IsNullOrWhiteSpace(version.URL))
        //                {
        //                    //goi tool view
        //                    String temFile = Path.GetTempFileName();
        //                    temFile = temFile.Replace(".tmp", ".pdf");
        //                    using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(version.URL))
        //                    {
        //                        if (stream != null)
        //                        {
        //                            using (var fileStream = new FileStream(temFile, FileMode.Create, FileAccess.Write))
        //                            {
        //                                stream.CopyTo(fileStream);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
        //                        }
        //                    }

        //                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
        //                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(row.TREATMENT_CODE, row.DOCUMENT_CODE, row.DOCUMENT_NAME, moduleData.RoomId);

        //                    // truyền paper
        //                    if (row.WIDTH != null && row.HEIGHT != null && row.RAW_KIND != null)
        //                    {
        //                        inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(row.PAPER_NAME, (int)row.WIDTH, (int)row.HEIGHT);
        //                        if (row.RAW_KIND != null)
        //                        {
        //                            inputADO.PaperSizeDefault.RawKind = (int)row.RAW_KIND;
        //                        }
        //                    }
        //                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));

        //                    if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
        //                        libraryProcessor.ShowPopup(temFile, inputADO);
        //                    else
        //                    {
        //                        XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
        //                    }

        //                    if (File.Exists(temFile)) File.Delete(temFile);
        //                }
        //                else
        //                {
        //                    XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
        //                }
        //            }
        //            else
        //            {
        //                XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void gridViewDocument_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                int rowHandleSelected = gridViewDocument.GetVisibleRowHandle(e.RowHandle);
                var data = (V_EMR_DOCUMENT)gridViewDocument.GetRow(rowHandleSelected);
                if (data != null)
                {

                    if (data != null && data.IS_DELETE == 1)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout;
                    }
                    else if (data.COUNT_RESIGN_FAILED != null && data.COUNT_RESIGN_FAILED > 0)
                    {
                        e.Appearance.ForeColor = Color.Maroon;
                    }
                    else if (data.COUNT_RESIGN_SUCCESS != null && data.COUNT_RESIGN_SUCCESS > 0)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoStatus__ChoKy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInitializeComponent) return;
                //if (rdoStatus__ChoKy.Checked)
                //{
                //    rdoStatus__DaKy.Checked = false;
                //    rdoStatus__TuChoiKy.Checked = false;
                //}
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoStatus__DaKy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (rdoStatus__DaKy.Checked)
                //{
                //    rdoStatus__ChoKy.Checked = false;
                //    rdoStatus__TuChoiKy.Checked = false;
                //}
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoStatus__TuChoiKy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (rdoStatus__TuChoiKy.Checked)
                //{
                //    rdoStatus__ChoKy.Checked = false;
                //    rdoStatus__DaKy.Checked = false;
                //}
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSigns_Click(object sender, EventArgs e)
        {
            try
            {
                ResetStateSign();
                SetStateButtonSign(false);

                var docs = GetDocumentSelected();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docs), docs));
                this.docuemntWaitSigns = docs != null ? docs.Where(o => o.IS_DELETE != 1 && !String.IsNullOrEmpty(o.NEXT_SIGNER) && String.IsNullOrEmpty(o.REJECTER)).ToList() : null;
                if ((this.docuemntWaitSigns == null || this.docuemntWaitSigns.Count == 0))
                {
                    MessageBox.Show(ResourceLanguageManager.BanChuaCoVanBanChoKyNao);
                    SetStateButtonSign(true);
                    return;
                }

                if (docs.Exists(o => o.IS_DELETE == 1 || o.SIGNERS == LoggingName || !String.IsNullOrEmpty(o.REJECTER)))
                {
                    MessageBox.Show(ResourceLanguageManager.ChiChoPhepKyVoiCacVanBanChoKy);
                    SetStateButtonSign(true);
                    return;
                }

                ProcessDocumentSignWithData();
            }
            catch (Exception ex)
            {
                SetStateButtonSign(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDocumentSignWithData()
        {
            try
            {
                WaitingManager.Show();
                //errorMessageWhileSign = new List<SignErrorADO>();
                //Process signs list document selected
                foreach (var item in docuemntWaitSigns)
                {
                    ProcessDocumentSignWithSingleData(item);
                }
                if (errorMessageWhileSign.Count < documentSignFinished && this.documentSignFinished > 0)
                {
                    FillDataToGrid();
                }

                SetStatusDocProcess();
                SetStateButtonSign(true);
                WaitingManager.Hide();
                Inventec.Desktop.Common.Message.MessageManager.Show(ResourceLanguageManager.KetThucXuLy);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetStateButtonSign(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDocumentSignWithSingleData(V_EMR_DOCUMENT document)
        {
            try
            {
                if (document != null)
                {
                    if (errorMessageWhileSign == null)
                        errorMessageWhileSign = new List<SignErrorADO>();

                    EMR.Filter.EmrVersionFilter versionFilter = new Filter.EmrVersionFilter();
                    versionFilter.DOCUMENT_ID = document.ID;

                    var listVersion = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_VERSION>>(EMR.URI.EmrVersion.GET, ApiConsumers.EmrConsumer, versionFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listVersion != null && listVersion.Count > 0)
                    {
                        EMR.EFMODEL.DataModels.EMR_VERSION version = new EFMODEL.DataModels.EMR_VERSION();
                        version = listVersion.OrderByDescending(o => o.ID).FirstOrDefault();
                        if (version != null && !String.IsNullOrWhiteSpace(version.URL))
                        {
                            //goi tool view
                            var temFile = System.IO.Path.Combine(Application.StartupPath + "\\temp\\");
                            if (!Directory.Exists(temFile)) Directory.CreateDirectory(temFile);
                            temFile = System.IO.Path.Combine(temFile, string.Format("{0}.pdf", Guid.NewGuid()));

                            using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(version.URL))
                            {
                                if (stream != null)
                                {
                                    using (var fileStream = new FileStream(temFile, FileMode.Create, FileAccess.Write))
                                    {
                                        stream.CopyTo(fileStream);
                                    }
                                }
                                else
                                {
                                    //XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                    errorMessageWhileSign.Add(new SignErrorADO()
                                    {
                                        CODE = document.DOCUMENT_CODE,
                                        NAME = String.Format("Văn bản mã {0} chưa được thiết lập vị trí ký", document.DOCUMENT_CODE)
                                    });
                                }
                            }

                            SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(document.TREATMENT_CODE, document.DOCUMENT_CODE, document.DOCUMENT_NAME, moduleData.RoomId);
                            inputADO.IsSign = true;
                            inputADO.IsSave = false;
                            inputADO.IsPrint = false;
                            inputADO.IsExport = false;
                            // truyền paper
                            if (document.WIDTH != null && document.HEIGHT != null && document.RAW_KIND != null)
                            {
                                inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(document.PAPER_NAME, (int)document.WIDTH, (int)document.HEIGHT);
                                if (document.RAW_KIND != null)
                                {
                                    inputADO.PaperSizeDefault.RawKind = (int)document.RAW_KIND;
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));

                            if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
                            {
                                string base64FileSign = Inventec.Common.SignLibrary.Utils.FileToBase64String(temFile);
                                var rsSign = libraryProcessor.SignNow(base64FileSign, FileType.Pdf, inputADO, true);
                                this.documentSignFinished += 1;
                                if (rsSign == null || !rsSign.Success)
                                {
                                    if (rsSign != null && !String.IsNullOrEmpty(rsSign.Message))
                                    {
                                        errorMessageWhileSign.Add(new SignErrorADO()
                                        {
                                            CODE = document.DOCUMENT_CODE,
                                            NAME = rsSign.Message
                                        });
                                    }
                                    else
                                    {
                                        errorMessageWhileSign.Add(new SignErrorADO()
                                        {
                                            CODE = document.DOCUMENT_CODE,
                                            NAME = String.Format("Văn bản mã {0} ký thất bại. {1}", document.DOCUMENT_CODE, rsSign.Message)
                                        });
                                    }
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                            }

                            if (File.Exists(temFile)) File.Delete(temFile);
                        }
                        else
                        {
                            //XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                            errorMessageWhileSign.Add(new SignErrorADO()
                            {
                                CODE = document.DOCUMENT_CODE,
                                NAME = String.Format("Văn bản mã {0} chưa được thiết lập vị trí ký", document.DOCUMENT_CODE)
                            });
                        }
                    }
                    else
                    {
                        //XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                        errorMessageWhileSign.Add(new SignErrorADO()
                        {
                            CODE = document.DOCUMENT_CODE,
                            NAME = String.Format("Văn bản mã {0} chưa được thiết lập vị trí ký", document.DOCUMENT_CODE)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetStatusDocProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        lciForbtnErrorSignDetail.Visibility = (errorMessageWhileSign != null && errorMessageWhileSign.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        lblDocProcessedCount.Text = "Đã xử lý: " + documentSignFinished + "/" + this.docuemntWaitSigns.Count;
                        lblDocProcessErrorCount.Text = "Lỗi: " + (this.errorMessageWhileSign != null ? this.errorMessageWhileSign.Count : 0) + "";
                    }));
                }
                else
                {
                    lciForbtnErrorSignDetail.Visibility = (errorMessageWhileSign != null && errorMessageWhileSign.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lblDocProcessedCount.Text = "Đã xử lý: " + documentSignFinished + "/" + this.docuemntWaitSigns.Count;
                    lblDocProcessErrorCount.Text = "Lỗi: " + (this.errorMessageWhileSign != null ? this.errorMessageWhileSign.Count : 0) + "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetStateButtonSign(bool enable)
        {
            btnSigns.Enabled = enable;
        }

        void ResetStateSign()
        {
            try
            {
                SetStateButtonSign(true);
                errorMessageWhileSign = new List<SignErrorADO>();
                lciForbtnErrorSignDetail.Visibility = (errorMessageWhileSign != null && errorMessageWhileSign.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                documentSignFinished = 0;
                lblDocProcessedCount.Text = "";
                lblDocProcessErrorCount.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnErrorSignDetail_Click(object sender, EventArgs e)
        {
            try
            {
                frmSignErrrorDetail frmSignErrrorDetail = new frmSignErrrorDetail(this.errorMessageWhileSign);
                frmSignErrrorDetail.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                listEmrSelected = new List<V_EMR_DOCUMENT>();
                var listIndex = gridViewDocument.GetSelectedRows();
                foreach (var index in listIndex)
                {
                    var treatment = (V_EMR_DOCUMENT)gridViewDocument.GetRow(index);
                    if (treatment != null)
                    {
                        listEmrSelected.Add(treatment);
                    }
                }

                if (listEmrSelected.Count > 0)
                {
                    btnViewAndSign.Enabled = true;
                }
                else
                {
                    btnViewAndSign.Enabled = false;
                }

                gridViewDocument.BeginDataUpdate();
                gridViewDocument.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnViewAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                if (listEmrSelected != null && listEmrSelected.Count > 0)
                {
                    foreach (var item in listEmrSelected)
                    {
                        #region ----- DETAIL -----
                        try
                        {
                            EMR.Filter.EmrVersionFilter versionFilter = new Filter.EmrVersionFilter();
                            versionFilter.DOCUMENT_ID = item.ID;

                            var listVersion = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_VERSION>>(EMR.URI.EmrVersion.GET, ApiConsumers.EmrConsumer, versionFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                            if (listVersion != null && listVersion.Count > 0)
                            {
                                EMR.EFMODEL.DataModels.EMR_VERSION version = new EFMODEL.DataModels.EMR_VERSION();
                                version = listVersion.OrderByDescending(o => o.ID).FirstOrDefault();
                                if (version != null && !String.IsNullOrWhiteSpace(version.URL))
                                {
                                    //goi tool view
                                    var temFile = System.IO.Path.Combine(Application.StartupPath + "\\temp\\");
                                    if (!Directory.Exists(temFile)) Directory.CreateDirectory(temFile);

                                    temFile = System.IO.Path.Combine(temFile, string.Format("{0}.pdf", Guid.NewGuid()));
                                    using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(version.URL))
                                    {
                                        if (stream != null)
                                        {
                                            using (var fileStream = new FileStream(temFile, FileMode.Create, FileAccess.Write))
                                            {
                                                stream.CopyTo(fileStream);
                                            }
                                        }
                                        else
                                        {
                                            XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                        }
                                    }

                                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(item.TREATMENT_CODE, item.DOCUMENT_CODE, item.DOCUMENT_NAME, moduleData.RoomId);

                                    // truyền paper
                                    if (item.WIDTH != null && item.HEIGHT != null && item.RAW_KIND != null)
                                    {
                                        inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(item.PAPER_NAME, (int)item.WIDTH, (int)item.HEIGHT);
                                        if (item.RAW_KIND != null)
                                        {
                                            inputADO.PaperSizeDefault.RawKind = (int)item.RAW_KIND;
                                        }
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));

                                    if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
                                        libraryProcessor.ShowPopup(temFile, inputADO);
                                    else
                                    {
                                        XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                    }

                                    if (File.Exists(temFile)) File.Delete(temFile);
                                }
                                else
                                {
                                    XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                        #endregion
                    }
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_DEPARTMENT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.DEPARTMENT_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrDocumentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (EMR_DOCUMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.DOCUMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
