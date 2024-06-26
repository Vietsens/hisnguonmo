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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using System.IO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using EMR.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using EMR.EFMODEL.DataModels;
using DevExpress.XtraEditors.Repository;

namespace HIS.Desktop.Plugins.SereServTemplate
{
    public partial class FormSereServTemplate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        System.Globalization.CultureInfo cultureLang;
        internal int ActionType = 0;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP sereServTemp;
        long service_id = 0;
        List<HIS_SERE_SERV_TEMP> ListDataSource;

        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> service;
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE> serviceType;
        List<MOS.EFMODEL.DataModels.HIS_GENDER> gender;
        List<MOS.EFMODEL.DataModels.V_HIS_ROOM> VHisRoom;
        List<long> SERVICE_TYPE_IDs = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
        };

        const string FileType = "docx";
        const string FileType2 = "doc";
        const string PREFIX = "___";
        const string SpecialCharacters = "\\/:*?\"<>|";

        private object lockObj = new object();
        #endregion

        #region Construct
        public FormSereServTemplate()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                sereServTemp = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormSereServTemplate(Inventec.Desktop.Common.Modules.Module moduleData, long service_id, List<long> serviceTypeIds)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                sereServTemp = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP();

                this.Text = moduleData.text;
                this.service_id = service_id;
                if (serviceTypeIds != null && serviceTypeIds.Count > 0)
                    this.SERVICE_TYPE_IDs = serviceTypeIds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormSereServTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                LoadKeysFromlanguage();
                LoadCboService();
                LoadCboServiceType();
                LoadCboGender();
                LoadCboDocumentTypeCode();
                LoadCboBusinessCodes();
                LoadCboGroupTypeCode();
                LoadCboPhongXuLy();
                SetDefaultValueControl();
                FillDataToGrid();
                ValidControls();

                txtKeyWord.Text = "";
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboBusinessCodes()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboBusinessCodes.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__BusinessCodes);
                CboBusinessCodes.Properties.Tag = gridCheck;
                CboBusinessCodes.Properties.View.OptionsSelection.MultiSelect = true;

                var datas = BackendDataWorker.Get<EMR_BUSINESS>();
                if (datas != null)
                {
                    CboBusinessCodes.Properties.DataSource = datas;
                    CboBusinessCodes.Properties.DisplayMember = "BUSINESS_NAME";
                    CboBusinessCodes.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = CboBusinessCodes.Properties.View.Columns.AddField("BUSINESS_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    CboBusinessCodes.Properties.PopupFormWidth = 200;
                    CboBusinessCodes.Properties.View.OptionsView.ShowColumnHeaders = false;
                    CboBusinessCodes.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = CboBusinessCodes.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(CboBusinessCodes.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__BusinessCodes(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (EMR_BUSINESS rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.BUSINESS_NAME + ",";
                    }
                }
                CboBusinessCodes.Text = typeName;
                CboBusinessCodes.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboDocumentTypeCode()
        {
            try
            {
                var data = BackendDataWorker.Get<EMR_DOCUMENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "DOCUMENT_TYPE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(CboDocumentTypeCode, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboGroupTypeCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_LEAF = true;

                var data = new BackendAdapter(param).Get<List<EMR_DOCUMENT_GROUP>>("api/EmrDocumentGroup/Get", ApiConsumers.EmrConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "DOCUMENT_GROUP_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(CboDocumentGroupCode, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                //layout
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__BTN_ADD",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciConclude.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_CONCLUDE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_DESCRIPTION",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciSereServTempCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_SERE_SERV_TEMP_CODE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciSereServTempName.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_SERE_SERV_TEMP_NAME",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__TXT_KEYWORD__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciService.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_SERVICE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.lciServiceType.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_SERVICE_TYPE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);


                this.lcDescriptionText.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_DESCRIPTION_TEXT",
                   Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                   cultureLang);
                this.lcNote.Text = Inventec.Common.Resource.Get.Value(
                "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__LCI_NOTE",
                Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                cultureLang);
                //gridView
                Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_CREATOR",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_MODIFIER",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_SereServTempCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_SERE_SERV_TEMP_CODE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_SereServTempName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_SERE_SERV_TEMP_NAME",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__GC_STT",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.ButtonDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__TOOL_TIP__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);

                this.BtnExport.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__BTN_EXPORT",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
                this.BtnImport.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_SERE_SERV_TEMP__BTN_IMPORT",
                    Resources.ResourceLanguageManager.LanguageFormSereServTemplate,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtGender.Text = "";
                cboGender.EditValue = null;
                txtConclude.Text = "";
                txtNote.Text = "";
                txtDescriptionText.Text = "";
                txtSereServTempCode.Text = "";
                txtSereServTempName.Text = "";
                txtDescription.Text = "";
                txtServiceType.Text = "";
                cboService.Properties.DataSource = this.service;
                cboService.EditValue = null;
                cboServiceType.EditValue = null;
                CboPhongXuLy.EditValue = null;
                cboServiceType.Properties.Buttons[1].Visible = false;
                GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(cboService.Properties.View);
                this.ActionType = GlobalVariables.ActionAdd;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider, dxErrorProvider1);
                ribbonControlDescription.SelectedPage = homeRibbonPage1;

                if (service_id != 0)
                {
                    if (gridCheckMark != null)
                    {
                        List<V_HIS_SERVICE> ds = cboService.Properties.DataSource as List<V_HIS_SERVICE>;
                        List<V_HIS_SERVICE> selects = new List<V_HIS_SERVICE>();
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == service_id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }

                        gridCheckMark.SelectAll(selects);
                    }
                }

                ChkAutoChooseBusiness.Checked = false;
                CboDocumentTypeCode.EditValue = null;
                CboDocumentGroupCode.EditValue = null;
                CboPhongXuLy.EditValue = null;
                GridCheckMarksSelection gridCheckMarkBusinessCodes = CboBusinessCodes.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkBusinessCodes.ClearSelection(CboBusinessCodes.Properties.View);
                CboBusinessCodes.Text = "";
                ListEmrColumnMappingADO = new List<EmrColumnMappingADO>();
                txtEmrColumMapping.Text = null;
                ListEmrColumnMappingADO.Add(new EmrColumnMappingADO() { action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd });
                gridControl1.DataSource = null;
                gridControl1.DataSource = ListEmrColumnMappingADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboService()
        {
            try
            {
                var listService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                listService = listService.Where(o => SERVICE_TYPE_IDs.Contains(o.SERVICE_TYPE_ID)).ToList();
                this.service = listService.OrderBy(o => o.SERVICE_CODE).ToList();

                InitCheck(cboService, SelectionGrid__Service);
                InitCombo(cboService, service, "SERVICE_NAME", "ID");
                // InitComboService(cboService, service);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboServiceType()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceTypeFilter filter = new MOS.Filter.HisServiceTypeFilter();
                var listServiceType = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERVICE_TYPE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                listServiceType = listServiceType.Where(o => SERVICE_TYPE_IDs.Contains(o.ID)).ToList();
                this.serviceType = listServiceType.OrderBy(o => o.SERVICE_TYPE_CODE).ToList();
                InitComboServiceType(cboServiceType, serviceType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboPhongXuLy()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisRoomViewFilter filter = new MOS.Filter.HisRoomViewFilter();
                filter.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                filter.IS_ACTIVE = 1;
                var listRoom = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>
                    ("api/HisRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                InitComboPhongXuLy(CboPhongXuLy, listRoom.ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboGender()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisGenderFilter filter = new MOS.Filter.HisGenderFilter();
                var listGender = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_GENDER>>
                    (ApiConsumer.HisRequestUriStore.HIS_GENDER_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                this.gender = listGender.OrderBy(o => o.GENDER_CODE).ToList();
                InitComboGender(cboGender, listGender);
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
                ucPaging.Init(GridPaging, param, pagingSize);
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
                this.ListDataSource = new List<HIS_SERE_SERV_TEMP>();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>> apiResult = null;
                MOS.Filter.HisSereServTempFilter filter = new MOS.Filter.HisSereServTempFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>>
                    ("api/HisSereServTemp/GetDynamic", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    this.ListDataSource = apiResult.Data;
                    if (this.ListDataSource != null && this.ListDataSource.Count > 0)
                    {
                        gridControl.DataSource = this.ListDataSource;
                        rowCount = (this.ListDataSource == null ? 0 : this.ListDataSource.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (this.ListDataSource == null ? 0 : this.ListDataSource.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        SetDefaultValueControl();
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisSereServTempFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.ColumnParams = new List<string>();
                filter.ColumnParams.Add("ID");
                filter.ColumnParams.Add("SERE_SERV_TEMP_CODE");
                filter.ColumnParams.Add("SERE_SERV_TEMP_NAME");
                filter.ColumnParams.Add("CREATOR");
                filter.ColumnParams.Add("CREATE_TIME");
                filter.ColumnParams.Add("MODIFIER");
                filter.ColumnParams.Add("MODIFY_TIME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP)gridView.GetFocusedRow();
                if (row != null)
                {
                    SetDataRow(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSereServTempCode.Focus();
                    txtSereServTempCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP)gridView.GetFocusedRow();
                if (row != null)
                {
                    SetDataRow(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceType(GridLookUpEdit cboServiceType, List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE> serviceType)
        {
            cboServiceType.Properties.DataSource = serviceType;
            cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
            cboServiceType.Properties.ValueMember = "ID";
            cboServiceType.Properties.TextEditStyle = TextEditStyles.Standard;
            cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            cboServiceType.Properties.ImmediatePopup = true;
            cboServiceType.ForceInitialize();
            cboServiceType.Properties.View.Columns.Clear();
            cboServiceType.Properties.PopupFormSize = new Size(400, 250);

            GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_CODE");
            aColumnCode.Caption = "Mã";
            aColumnCode.Visible = true;
            aColumnCode.VisibleIndex = 1;
            aColumnCode.Width = 60;

            GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
            aColumnName.Caption = "Tên";
            aColumnName.Visible = true;
            aColumnName.VisibleIndex = 2;
            aColumnName.Width = 340;
        }

        private void InitComboPhongXuLy(GridLookUpEdit ComboPhongXuLy, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> VhisRoom)
        {
            CboPhongXuLy.Properties.DataSource = VhisRoom;
            CboPhongXuLy.Properties.DisplayMember = "ROOM_NAME";
            CboPhongXuLy.Properties.ValueMember = "ID";
            CboPhongXuLy.Properties.TextEditStyle = TextEditStyles.Standard;
            CboPhongXuLy.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            CboPhongXuLy.Properties.ImmediatePopup = true;
            CboPhongXuLy.ForceInitialize();
            CboPhongXuLy.Properties.View.Columns.Clear();
            CboPhongXuLy.Properties.PopupFormSize = new Size(400, 250);

            GridColumn aColumnCode = CboPhongXuLy.Properties.View.Columns.AddField("ROOM_CODE");
            aColumnCode.Caption = "Mã";
            aColumnCode.Visible = true;
            aColumnCode.VisibleIndex = 1;
            aColumnCode.Width = 60;

            GridColumn aColumnName = CboPhongXuLy.Properties.View.Columns.AddField("ROOM_NAME");
            aColumnName.Caption = "Tên";
            aColumnName.Visible = true;
            aColumnName.VisibleIndex = 2;
            aColumnName.Width = 340;
        }

        private void InitComboGender(GridLookUpEdit cboGender, List<MOS.EFMODEL.DataModels.HIS_GENDER> gender)
        {
            cboGender.Properties.DataSource = gender;
            cboGender.Properties.DisplayMember = "GENDER_NAME";
            cboGender.Properties.ValueMember = "ID";
            cboGender.Properties.TextEditStyle = TextEditStyles.Standard;
            cboGender.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            cboGender.Properties.ImmediatePopup = true;
            cboGender.ForceInitialize();
            cboGender.Properties.View.Columns.Clear();
            cboGender.Properties.PopupFormSize = new Size(400, 250);

            GridColumn aColumnCode = cboGender.Properties.View.Columns.AddField("GENDER_CODE");
            aColumnCode.Caption = "Mã";
            aColumnCode.Visible = true;
            aColumnCode.VisibleIndex = 1;
            aColumnCode.Width = 60;

            GridColumn aColumnName = cboGender.Properties.View.Columns.AddField("GENDER_NAME");
            aColumnName.Caption = "Tên";
            aColumnName.Visible = true;
            aColumnName.VisibleIndex = 2;
            aColumnName.Width = 340;
        }

        private void SetDataRow(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP row)
        {
            try
            {
                if (row != null)
                {
                    if (row.DESCRIPTION == null || row.DESCRIPTION.Length <= 0)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisSereServTempFilter filter = new MOS.Filter.HisSereServTempFilter();
                        filter.ID = row.ID;
                        var apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEMP>>("api/HisSereServTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            int index = this.ListDataSource.IndexOf(row);
                            if (index >= 0)
                            {
                                this.ListDataSource[index] = apiResult.FirstOrDefault();
                            }

                            row = apiResult.FirstOrDefault();

                            gridControl.RefreshDataSource();
                        }
                    }

                    this.sereServTemp = row;
                    txtConclude.Text = row.CONCLUDE;
                    txtNote.Text = row.NOTE;
                    txtDescriptionText.Text = row.DESCRIPTION_TEXT;
                    txtSereServTempCode.Text = row.SERE_SERV_TEMP_CODE;
                    txtSereServTempName.Text = row.SERE_SERV_TEMP_NAME;
                    txtDescription.RtfText = Encoding.UTF8.GetString(row.DESCRIPTION);
                    float zoom = 0;
                    if (txtDescription.Document.Sections[0].Page.Landscape)
                        zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Height / 3);
                    else
                        zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Width / 3);
                    txtDescription.ActiveView.ZoomFactor = zoom;
                    this.ActionType = GlobalVariables.ActionEdit;

                    if (row.SERVICE_TYPE_ID.HasValue)
                    {
                        var services = service.Where(o => o.SERVICE_TYPE_ID == row.SERVICE_TYPE_ID).ToList();
                        cboService.Properties.DataSource = services;
                        var aservicetype = this.serviceType.FirstOrDefault(o => o.ID == row.SERVICE_TYPE_ID);
                        if (aservicetype != null)
                        {
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            cboServiceType.EditValue = aservicetype.ID;
                            txtServiceType.Text = aservicetype.SERVICE_TYPE_CODE;
                            //txtService.Text = aservicetype.SERVICE_TYPE_CODE;
                        }
                        else
                        {
                            cboServiceType.Properties.Buttons[1].Visible = false;
                            cboServiceType.EditValue = null;
                            txtServiceType.Text = "";
                        }
                    }
                    else
                    {
                        cboService.Properties.DataSource = service;
                        cboServiceType.Properties.Buttons[1].Visible = false;
                        cboServiceType.EditValue = null;
                        txtServiceType.Text = "";
                    }

                    GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboService.Properties.View);
                        if (!String.IsNullOrWhiteSpace(row.SERVICE_IDS))
                            ProcessSelectService(row, gridCheckMark);
                    }


                    if (row.GENDER_ID.HasValue)
                    {
                        var aGender = this.gender.FirstOrDefault(o => o.ID == row.GENDER_ID);
                        if (aGender != null)
                        {
                            cboGender.Properties.Buttons[1].Visible = true;
                            cboGender.EditValue = aGender.ID;
                            txtGender.Text = aGender.GENDER_CODE;
                        }
                        else
                        {
                            cboGender.Properties.Buttons[1].Visible = false;
                            cboGender.EditValue = null;
                            txtGender.Text = "";
                        }
                    }
                    else
                    {
                        cboGender.Properties.Buttons[1].Visible = false;
                        cboGender.EditValue = null;
                        txtGender.Text = "";
                    }

                    ChkAutoChooseBusiness.Checked = row.IS_AUTO_CHOOSE_BUSINESS == 1;
                    CboDocumentTypeCode.EditValue = row.EMR_DOCUMENT_TYPE_CODE;
                    CboPhongXuLy.EditValue = row.ROOM_ID;
                    CboDocumentGroupCode.EditValue = row.EMR_DOCUMENT_GROUP_CODE;
                    txtEmrColumMapping.Text = row.EMR_COLUMN_MAPPING;
                    ListEmrColumnMappingADO = !string.IsNullOrEmpty(row.EMR_COLUMN_MAPPING) ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmrColumnMappingADO>>(row.EMR_COLUMN_MAPPING) : new List<EmrColumnMappingADO>();

                    if (ListEmrColumnMappingADO == null || ListEmrColumnMappingADO.Count == 0)
                    {
                        ListEmrColumnMappingADO.Add(new EmrColumnMappingADO());
                    }

                    ListEmrColumnMappingADO.ForEach(o => o.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ListEmrColumnMappingADO.FirstOrDefault().action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = ListEmrColumnMappingADO;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = CboBusinessCodes.Properties.Tag as GridCheckMarksSelection;

                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(CboBusinessCodes.Properties.View);
                        if (!String.IsNullOrWhiteSpace(row.EMR_BUSINESS_CODES))
                            ProcessSelectBusiness(row.EMR_BUSINESS_CODES, gridCheckMarkBusinessCodes);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandle = -1;
                if (!btnSave.Enabled) return;
                if (!dxValidationProvider.Validate()) return;
                WaitingManager.Show();
                if (this.ActionType == GlobalVariables.ActionAdd)
                    this.sereServTemp = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP();
                SetData(sereServTemp);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServTemp), sereServTemp));

                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                    <MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>
                    (this.ActionType == GlobalVariables.ActionAdd ?
                    ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_TEMP_CREATE :
                    ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_TEMP_UPDATE,
                    ApiConsumer.ApiConsumers.MosConsumer, sereServTemp, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null)
                {
                    success = true;

                    FillDataToGrid();
                }
                btnAdd_Click(null, null);
                WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                txtSereServTempCode.Focus();
                txtSereServTempCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_TEMP_DELETE, ApiConsumer.ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGrid();
                            gridView_FocusedRowChanged(null, null);
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetData(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP data)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    List<long> ids = new List<long>();
                    foreach (V_HIS_SERVICE rv in gridCheckMark.Selection)
                    {
                        if (rv != null && !ids.Contains(rv.ID))
                            ids.Add(rv.ID);
                    }
                    data.SERVICE_IDS = String.Join(",", ids);
                }
                else
                {
                    data.SERVICE_IDS = null;
                }

                cboService.ResetText();

                if (cboServiceType.EditValue != null)
                {
                    data.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? 0).ToString());
                }
                else
                {
                    data.SERVICE_TYPE_ID = null;
                }

                if (cboGender.EditValue != null)
                {
                    data.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? 0).ToString());
                }
                else
                {
                    data.GENDER_ID = null;
                }
                if (CboPhongXuLy.EditValue != null)
                {
                    data.ROOM_ID = (long)CboPhongXuLy.EditValue;
                }
                else
                {
                    data.ROOM_ID = null;
                }
                data.SERE_SERV_TEMP_CODE = txtSereServTempCode.Text;
                data.SERE_SERV_TEMP_NAME = txtSereServTempName.Text;
                data.CONCLUDE = txtConclude.Text.Trim();
                data.NOTE = txtNote.Text.Trim();
                data.DESCRIPTION_TEXT = txtDescriptionText.Text.Trim();
                data.DESCRIPTION = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                data.EMR_DOCUMENT_TYPE_CODE = CboDocumentTypeCode.EditValue != null ? CboDocumentTypeCode.EditValue.ToString() : null;
                data.EMR_DOCUMENT_GROUP_CODE = CboDocumentGroupCode.EditValue != null ? CboDocumentGroupCode.EditValue.ToString() : null;
                if (CboPhongXuLy.EditValue != null)
                {
                    data.ROOM_ID = (long)CboPhongXuLy.EditValue;
                }
                else
                {
                    data.ROOM_ID = null;
                }

                data.IS_AUTO_CHOOSE_BUSINESS = null;
                if (ChkAutoChooseBusiness.Checked)
                {
                    data.IS_AUTO_CHOOSE_BUSINESS = (short)1;
                }

                GridCheckMarksSelection gridCheckMarkBusiness = CboBusinessCodes.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (EMR_BUSINESS rv in gridCheckMarkBusiness.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.BUSINESS_CODE))
                            codes.Add(rv.BUSINESS_CODE);
                    }

                    data.EMR_BUSINESS_CODES = String.Join(";", codes);
                }
                else
                {
                    data.EMR_BUSINESS_CODES = null;
                }
                data.EMR_COLUMN_MAPPING = !string.IsNullOrEmpty(txtEmrColumMapping.Text.Trim()) ? txtEmrColumMapping.Text.Trim() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region enter
        private void txtSereServTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSereServTempName.Focus();
                    txtSereServTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSereServTempName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceType.Focus();
                    txtServiceType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtServiceType.Text.Trim()))
                    {
                        string code = txtServiceType.Text.Trim();
                        var listData = serviceType.Where(o => o.SERVICE_TYPE_CODE.Equals(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SERVICE_TYPE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            cboServiceType.EditValue = result.First().ID;
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            var services = service.Where(o => o.SERVICE_TYPE_ID == result.First().ID).ToList();
                            cboService.Properties.DataSource = services;
                            //InitCombo(cboService, services, "SERVICE_NAME", "ID");
                            //txtService.Text = "";
                            cboService.EditValue = null;
                            cboService.Focus();
                            cboService.SelectAll();
                            cboService.ShowPopup();
                        }
                    }
                    if (showCbo)
                    {
                        cboServiceType.Focus();
                        cboServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        var aService = serviceType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            txtServiceType.Text = aService.SERVICE_TYPE_CODE;
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            var services = service.Where(o => o.SERVICE_TYPE_ID == aService.ID).ToList();
                            cboService.Properties.DataSource = services;
                            GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                            gridCheckMark.ClearSelection(cboService.Properties.View);
                            //cboService.EditValue = null;
                            cboService.Focus();
                            cboService.SelectAll();
                            cboService.ShowPopup();
                        }
                        else
                            cboServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        var aService = serviceType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            txtServiceType.Text = aService.SERVICE_TYPE_CODE;
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            var services = service.Where(o => o.SERVICE_TYPE_ID == aService.ID).ToList();
                            cboService.Properties.DataSource = services;
                            GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                            gridCheckMark.ClearSelection(cboService.Properties.View);
                            //InitCombo(cboService, services, "SERVICE_NAME", "ID");
                            //txtService.Text = "";
                            cboService.EditValue = null;
                            cboService.ShowPopup();
                        }
                        else
                            cboService.ShowPopup();
                    }
                    else
                    {
                        cboService.ShowPopup();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtServiceType.Text = "";
                    cboServiceType.EditValue = null;
                    cboServiceType.Properties.Buttons[1].Visible = false;
                    cboService.Properties.DataSource = service;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtGender.Text.Trim()))
                    {
                        string code = txtGender.Text.Trim();
                        var listData = gender.Where(o => o.GENDER_CODE.Equals(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.GENDER_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            cboGender.EditValue = result.First().ID;
                            cboGender.Properties.Buttons[1].Visible = true;

                            txtDescriptionText.Focus();
                            txtDescriptionText.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender.EditValue != null)
                    {
                        var aGender = gender.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? 0).ToString()));
                        if (aGender != null)
                        {
                            txtGender.Text = aGender.GENDER_CODE;
                            cboGender.Properties.Buttons[1].Visible = true;

                            txtDescriptionText.Focus();
                            txtDescriptionText.SelectAll();
                        }
                        else
                            cboGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboGender.EditValue != null)
                    {
                        var aGender = gender.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? 0).ToString()));
                        if (aGender != null)
                        {
                            txtGender.Text = aGender.GENDER_CODE;
                            cboGender.Properties.Buttons[1].Visible = true;
                            txtDescriptionText.Focus();
                            txtDescriptionText.SelectAll();
                        }
                        else
                            cboGender.ShowPopup();
                    }
                    else
                    {
                        txtDescriptionText.Focus();
                        txtDescriptionText.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboGender.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtGender.Text = "";
                    cboGender.EditValue = null;
                    cboGender.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboService.EditValue != null)
                    {
                        var aService = service.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboService.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            //txtService.Text = aService.SERVICE_CODE;
                            cboService.Properties.Buttons[1].Visible = true;
                            txtGender.Focus();
                            txtGender.SelectAll();
                        }
                        else
                            cboService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboService.EditValue != null)
                    {
                        var aService = service.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboService.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            //txtService.Text = aService.SERVICE_CODE;
                            cboService.Properties.Buttons[1].Visible = true;
                            txtGender.Focus();
                            txtGender.SelectAll();
                        }
                        else
                            cboGender.ShowPopup();
                    }
                    else
                    {
                        txtGender.Focus();
                        txtGender.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboService.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    //txtService.Text = "";
                    cboService.EditValue = null;
                    cboService.Properties.Buttons[1].Visible = false;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtConclude_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
                if (e.KeyCode == Keys.Down) gridView.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Validation
        private void ValidControls()
        {
            try
            {
                ValidSereServTempCode(txtSereServTempCode, 10, true);
                ValidSereServTempCode(txtSereServTempName, 100, true);
                ValidSereServTempCode(txtDescriptionText, 4000, false);
                ValidSereServTempCode(txtNote, 1000, false);
                ValidSereServTempCode(txtConclude, 1000, false);
                ValidSereServTempCode(txtEmrColumMapping, 4000, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSereServTempCode(BaseEdit control, int maxlength, bool isRequired)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule valid = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxlength;
                valid.IsRequired = isRequired;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion
        #endregion

        #region Shortcut
        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP> listExport = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>();
                for (int i = 0; i < gridView.GetSelectedRows().Count(); i++)
                {
                    listExport.Add((MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP)gridView.GetRow(gridView.GetSelectedRows()[i]));
                }

                if (listExport.Count > 0)
                {
                    string saveFolder = "";

                    FolderBrowserDialog fd = new FolderBrowserDialog();
                    fd.ShowNewFolderButton = true;
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        saveFolder = fd.SelectedPath;

                        WaitingManager.Show();
                        if (!String.IsNullOrWhiteSpace(saveFolder))
                        {
                            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP> listExportNew = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>();
                            List<long> listIds = listExport.Select(s => s.ID).ToList();
                            List<Task> taskall = new List<Task>();
                            int skip = 0;
                            while (listIds.Count - skip > 0)
                            {
                                List<long> ids = listIds.Skip(skip).Take(100).ToList();
                                skip += 100;

                                Task tsData = Task.Factory.StartNew((object obj) =>
                                {
                                    List<long> data = obj as List<long>;

                                    CommonParam param = new CommonParam();
                                    MOS.Filter.HisSereServTempFilter filter = new MOS.Filter.HisSereServTempFilter();
                                    filter.IDs = data;
                                    filter.ColumnParams = new List<string>();
                                    filter.ColumnParams.Add("ID");
                                    filter.ColumnParams.Add("SERE_SERV_TEMP_CODE");
                                    filter.ColumnParams.Add("SERE_SERV_TEMP_NAME");
                                    filter.ColumnParams.Add("DESCRIPTION");
                                    List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP> apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEMP>>("api/HisSereServTemp/GetDynamic", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                    if (apiResult != null && apiResult.Count > 0)
                                    {
                                        lock (lockObj)
                                        {
                                            listExportNew.AddRange(apiResult);
                                        }
                                    }
                                }, ids);
                                taskall.Add(tsData);
                            }

                            Task.WaitAll(taskall.ToArray());

                            foreach (var item in listExportNew)
                            {
                                string code = ProcessFileName(item.SERE_SERV_TEMP_CODE);
                                string name = ProcessFileName(item.SERE_SERV_TEMP_NAME);
                                string fileName = String.Format("{0}{1}{2}.{3}", code, PREFIX, name, FileType);
                                using (DevExpress.XtraRichEdit.RichEditControl export = new DevExpress.XtraRichEdit.RichEditControl())
                                {
                                    export.RtfText = Encoding.UTF8.GetString(item.DESCRIPTION);
                                    export.SaveDocument(Path.Combine(saveFolder, fileName), DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                }
                            }
                        }
                        WaitingManager.Hide();

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, new CommonParam(), true);
                        #endregion
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDuLieuXuat);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessFileName(string name)
        {
            string result = name;
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    foreach (char character in Path.GetInvalidFileNameChars())
                    {
                        name = name.Replace(character.ToString(), string.Empty);
                    }
                    result = name;
                }
            }
            catch (Exception ex)
            {
                result = name;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string folderName = "";

                FolderBrowserDialog fd = new FolderBrowserDialog();
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    folderName = fd.SelectedPath;
                }

                if (!String.IsNullOrWhiteSpace(folderName))
                {
                    WaitingManager.Show();
                    List<string> fileEntries = new List<string>();

                    var fileEntriesDocx = System.IO.Directory.GetFiles(folderName, "*." + FileType, SearchOption.TopDirectoryOnly);
                    if (fileEntriesDocx != null && fileEntriesDocx.Count() > 0)
                    {
                        fileEntries.AddRange(fileEntriesDocx.ToList());
                    }

                    var fileEntriesDoc = System.IO.Directory.GetFiles(folderName, "*." + FileType2, SearchOption.TopDirectoryOnly);
                    if (fileEntriesDoc != null && fileEntriesDoc.Count() > 0)
                    {
                        fileEntries.AddRange(fileEntriesDoc.ToList());
                    }

                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error("Folder khong co file nao: " + folderName);
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.KhongTimThayFileNhap);
                        return;
                    }

                    fileEntries = fileEntries.OrderBy(o => o).ToList();
                    long createSuccess = 0;
                    long createError = 0;
                    foreach (var file in fileEntries)
                    {
                        string fileName = file.Substring(file.LastIndexOf("\\") + 1).ToString();
                        if (!String.IsNullOrWhiteSpace(fileName))
                        {
                            try
                            {
                                string code = "";
                                string name = "";
                                string errorOut = "";
                                CommonParam param = new CommonParam();
                                bool success = false;

                                if (fileName.Contains(PREFIX))
                                {
                                    code = fileName.Substring(0, fileName.LastIndexOf(PREFIX));
                                    name = fileName.Substring(fileName.LastIndexOf(PREFIX) + PREFIX.Length, fileName.LastIndexOf(".") - fileName.LastIndexOf(PREFIX) - PREFIX.Length);
                                }
                                else
                                {
                                    code = fileName.Substring(0, 5);
                                    name = fileName;
                                }

                                var sereServTemp = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP();
                                sereServTemp.SERE_SERV_TEMP_CODE = code;
                                sereServTemp.SERE_SERV_TEMP_NAME = name;
                                try
                                {
                                    using (DevExpress.XtraRichEdit.RichEditControl export = new DevExpress.XtraRichEdit.RichEditControl())
                                    {
                                        export.LoadDocument(file);
                                        sereServTemp.DESCRIPTION = Encoding.UTF8.GetBytes(export.RtfText);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sereServTemp.DESCRIPTION = null;
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }

                                if (sereServTemp.DESCRIPTION == null || String.IsNullOrWhiteSpace(sereServTemp.SERE_SERV_TEMP_CODE))
                                {
                                    if (sereServTemp.DESCRIPTION == null)
                                    {
                                        errorOut = "Khong doc duoc file";
                                    }
                                    else if (String.IsNullOrWhiteSpace(sereServTemp.SERE_SERV_TEMP_CODE))
                                    {
                                        errorOut = "Loi lay ma tu ten file";
                                    }
                                }
                                else
                                {
                                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_TEMP_CREATE, ApiConsumer.ApiConsumers.MosConsumer, sereServTemp, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                    if (apiResult != null)
                                    {
                                        success = true;
                                    }
                                    else
                                    {
                                        errorOut = string.Join(";", param.Messages);
                                    }
                                }

                                if (success)
                                {
                                    createSuccess += 1;
                                    MoveFileSuccess(file);
                                }
                                else
                                {
                                    createError += 1;
                                    MoveFileError(file, errorOut);
                                }
                            }
                            catch (Exception ex)
                            {
                                createError += 1;
                                MoveFileError(file, "Exception: " + ex.Message);
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.ThongBaoImport, createSuccess, createError, fileEntries.Count()));
                }

                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MoveFileError(string file, string errorOut)
        {
            try
            {
                string FolderPath = file.Substring(0, file.LastIndexOf("\\")).ToString();
                string fileName = file.Substring(file.LastIndexOf("\\") + 1).ToString();
                var mapPath = Path.Combine(FolderPath, "that_bai");
                if (!System.IO.Directory.Exists(mapPath))
                    System.IO.Directory.CreateDirectory(mapPath);

                string fullFileName = Path.Combine(mapPath, "log.txt");
                if (!File.Exists(fullFileName))
                {
                    using (StreamWriter sw = new StreamWriter(fullFileName))
                    {
                        sw.WriteLine(" ");
                    }
                }

                List<string> result = new List<string>();
                using (StreamReader sr = new StreamReader(fullFileName))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }

                result.Add(string.Format("{0}: {1}", fileName, errorOut));
                using (StreamWriter sw = new StreamWriter(fullFileName))
                {
                    foreach (var item in result)
                    {
                        sw.WriteLine(item);
                    }
                }

                File.Copy(file, Path.Combine(mapPath, fileName), true);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MoveFileSuccess(string file)
        {
            try
            {
                string FolderPath = file.Substring(0, file.LastIndexOf("\\")).ToString();
                string fileName = file.Substring(file.LastIndexOf("\\") + 1).ToString();
                var mapPath = Path.Combine(FolderPath, "thanh_cong");
                if (!System.IO.Directory.Exists(mapPath))
                    System.IO.Directory.CreateDirectory(mapPath);

                File.Copy(file, Path.Combine(mapPath, fileName), true);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnTemplateKey_Click(object sender, EventArgs e)
        {
            try
            {
                var keyForm = new TemplateKey.PreviewTemplateKey(currentModuleBase);
                if (keyForm != null)
                {
                    keyForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);
                if (hitInfo.InRowCell)
                {
                    gridView.FocusedColumn = hitInfo.Column;
                    gridView.FocusedRowHandle = hitInfo.RowHandle;
                    gridView.ShowEditor();

                    if (hitInfo.Column.Name == Gc_Delete.Name)
                    {
                        ButtonDelete_ButtonClick(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboDocumentTypeCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescriptionText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConclude.Focus();
                    txtConclude.SelectAll();
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

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("SERVICE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã dịch vụ";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField("SERVICE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên dịch vụ";

                cbo.Properties.PopupFormWidth = 320;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
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

        private void SelectionGrid__Service(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (V_HIS_SERVICE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.SERVICE_NAME + ",";
                    }
                }
                cboService.Text = typeName;
                cboService.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectService(HIS_SERE_SERV_TEMP data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<V_HIS_SERVICE> ds = cboService.Properties.DataSource as List<V_HIS_SERVICE>;
                string[] arrays = data.SERVICE_IDS.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<V_HIS_SERVICE> selects = new List<V_HIS_SERVICE>();
                    List<long> ids = new List<long>();
                    foreach (var item in arrays)
                    {
                        long id = Convert.ToInt64(item ?? "0");
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectService(List<V_HIS_SERVICE> data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    gridCheckMark.SelectAll(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                string typeName = "";
                string typeCode = "";
                GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (V_HIS_SERVICE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.SERVICE_NAME + ",";
                        typeCode += rv.SERVICE_CODE + ",";
                    }
                }
                e.DisplayText = typeName;
                txtService.Text = typeCode;
                cboService.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentTypeCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    CboDocumentGroupCode.Focus();
                    CboDocumentGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboDocumentGroupCode.Focus();
                    CboDocumentGroupCode.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    CboDocumentTypeCode.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBusinessCodes_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    ChkAutoChooseBusiness.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBusinessCodes_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (EMR_BUSINESS rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.BUSINESS_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusiness(string BusinessCode, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<EMR_BUSINESS> ds = CboBusinessCodes.Properties.DataSource as List<EMR_BUSINESS>;
                string[] arrays = BusinessCode.Split(';');
                if (arrays != null && arrays.Length > 0)
                {
                    List<EMR_BUSINESS> selects = new List<EMR_BUSINESS>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.BUSINESS_CODE == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentTypeCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboDocumentTypeCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBusinessCodes_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMarkBusinessCodes = CboBusinessCodes.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkBusinessCodes.ClearSelection(CboBusinessCodes.Properties.View);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                if (e.Column.FieldName == "CheckMarkSelection")
                    return;
                else
                {
                    bool check = false;
                    List<V_HIS_SERVICE> dataSourc = new List<V_HIS_SERVICE>();
                    var focus = (V_HIS_SERVICE)cboService.Properties.View.GetFocusedRow();
                    GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;

                    if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                    {
                        foreach (V_HIS_SERVICE rv in gridCheckMark.Selection)
                        {
                            dataSourc.Add(rv);
                        }
                        foreach (var item in dataSourc)
                        {

                            if (item.ID == focus.ID)
                            {
                                check = false;
                                break;
                            }
                            if (item.ID != focus.ID)
                            {
                                check = true;
                            }

                        }
                        if (check == true)
                        {
                            dataSourc.Add(focus);
                        }
                        else if (check == false)
                        {
                            dataSourc.Remove(focus);
                        }
                    }
                    else
                    {
                        dataSourc.Add(focus);
                    }
                    gridCheckMark.SelectAll(dataSourc);

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CboDocumentGroupCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    CboBusinessCodes.Focus();
                    CboBusinessCodes.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboBusinessCodes.Focus();
                    CboBusinessCodes.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    CboDocumentGroupCode.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentGroupCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboDocumentGroupCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDocumentGroupCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboDocumentGroupCode.EditValue != null)
                    CboDocumentGroupCode.Properties.Buttons[1].Visible = true;
                else
                    CboDocumentGroupCode.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtService_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<V_HIS_SERVICE> services = new List<V_HIS_SERVICE>();
                    if (!String.IsNullOrEmpty(txtService.Text))
                    {
                        List<string> lstCode = new List<string>();
                        lstCode = txtService.Text.Split(',').ToList();
                        services = service.Where(o => lstCode.Contains(o.SERVICE_CODE)).ToList();
                        cboService.Properties.DataSource = service;
                    }
                    else
                    {
                        cboService.Properties.DataSource = service;
                        cboServiceType.Properties.Buttons[1].Visible = false;
                        cboServiceType.EditValue = null;
                        txtServiceType.Text = "";
                    }

                    GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboService.Properties.View);
                    }
                    if (services != null && services.Count > 0 && gridCheckMark != null)
                    {
                        ProcessSelectService(services, gridCheckMark);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPhongXuLy_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboPhongXuLy.EditValue = null;
                    // CboPhongXuLy.
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<EmrColumnMappingADO> ListEmrColumnMappingADO = new List<EmrColumnMappingADO>();
        private void btnOk_Click(object sender, EventArgs e)
        {

            try
            {
                SetValueText();
                popupContainerControl1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void SetValueText()
        {

            try
            {
                ListEmrColumnMappingADO = ListEmrColumnMappingADO.Where(o => !string.IsNullOrEmpty(o.EmrColumn) || !string.IsNullOrEmpty(o.Key)).ToList();
                if (ListEmrColumnMappingADO != null && ListEmrColumnMappingADO.Count > 0)
                {
                    var data = Newtonsoft.Json.JsonConvert.SerializeObject(ListEmrColumnMappingADO);
                    txtEmrColumMapping.Text = data.Replace(",\"action\":1", "").Replace(",\"action\":2", "");
                }
                else
                {
                    txtEmrColumMapping.Text = null;
                    ListEmrColumnMappingADO.Add(new EmrColumnMappingADO() { action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void btnEditEmrColumnMapping_Click(object sender, EventArgs e)
        {
            try
            {
                ShowPopupContainer();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ShowPopupContainer()
        {
            try
            {
                gridControl1.DataSource = null;
                gridControl1.DataSource = ListEmrColumnMappingADO;
                Rectangle buttonPosition = new Rectangle(txtEmrColumMapping.Bounds.X, txtEmrColumMapping.Bounds.Y, txtEmrColumMapping.Bounds.Width, txtEmrColumMapping.Bounds.Height);
                popupContainerControl1.ShowPopup(new System.Drawing.Point(buttonPosition.X + 650, buttonPosition.Bottom + 250));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "action")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView5.GetRowCellValue(e.RowHandle, "action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repAdd;
                    }
                    else
                    {
                        e.RepositoryItem = repDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repAdd_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                ListEmrColumnMappingADO.Add(new EmrColumnMappingADO());
                ListEmrColumnMappingADO.ForEach(o => o.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ListEmrColumnMappingADO.FirstOrDefault().action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl1.DataSource = null;
                gridControl1.DataSource = ListEmrColumnMappingADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ListEmrColumnMappingADO.Remove((EmrColumnMappingADO)gridView5.GetFocusedRow());
                ListEmrColumnMappingADO.ForEach(o => o.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ListEmrColumnMappingADO.FirstOrDefault().action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl1.DataSource = null;
                gridControl1.DataSource = ListEmrColumnMappingADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupContainerControl1_CloseUp(object sender, EventArgs e)
        {
            ListEmrColumnMappingADO = gridControl1.DataSource as List<EmrColumnMappingADO>;
            SetValueText();
        }

        private void gridView5_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
