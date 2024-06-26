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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCareTemp
{
    public partial class FormCareTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private System.Globalization.CultureInfo cultureLang;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HIS_CARE_TEMP CareSend;
        private List<HIS_CARE_TYPE> LstHisCareType = new List<HIS_CARE_TYPE>();
        private List<ADO.CareTempDetailADO> ListCareTempDetailADO;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private int positionHandleControl = -1;
        private HIS_CARE_TEMP CareSave;
        private bool IsAdmin;
        private string LogginName;
        #endregion

        #region ctor
        public FormCareTemp(Inventec.Desktop.Common.Modules.Module moduleData, HIS_CARE_TEMP data)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.CareSend = data;
                this.Text = moduleData.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                LogginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                IsAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(LogginName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region method
        #region Load
        private void FormCareTemp_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                LoadDataCareDetail();

                LoadDataToComboCareType(repositoryItemGridCareTypeName, LstHisCareType);

                SetDefaultDataControl();

                FillDataToGridLeft();

                FillDataToControlRight(CareSend);

                EnableControlChanged(GlobalVariables.ActionAdd);

                InitValidateColtrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlRight(HIS_CARE_TEMP CareSend)
        {
            try
            {
                if (CareSend != null)
                {
                    this.TxtCareTempCode.Text = CareSend.CARE_TEMP_CODE;
                    this.TxtCareTempName.Text = CareSend.CARE_TEMP_NAME;
                    this.txtAwareness.Text = CareSend.AWARENESS;
                    this.txtCareDescription.Text = CareSend.CARE_DESCRIPTION;
                    this.txtDejecta.Text = CareSend.DEJECTA;
                    this.txtEducation.Text = CareSend.EDUCATION;
                    this.txtInstructionDescription.Text = CareSend.INSTRUCTION_DESCRIPTION;
                    this.txtMucocutaneous.Text = CareSend.MUCOCUTANEOUS;
                    this.txtNutrition.Text = CareSend.NUTRITION;
                    this.txtSanitary.Text = CareSend.SANITARY;
                    this.txtTutorial.Text = CareSend.TUTORIAL;
                    this.txtUrine.Text = CareSend.URINE;

                    if (CareSend.HAS_ADD_MEDICINE.HasValue && CareSend.HAS_ADD_MEDICINE.Value == 1)
                        chkHasAddMedicine.Checked = true;
                    else
                        chkHasAddMedicine.Checked = false;

                    if (CareSend.HAS_MEDICINE.HasValue && CareSend.HAS_MEDICINE.Value == 1)
                        chkHasMedicine.Checked = true;
                    else
                        chkHasMedicine.Checked = false;

                    if (CareSend.HAS_TEST.HasValue && CareSend.HAS_TEST.Value == 1)
                        chkHasTest.Checked = true;
                    else
                        chkHasTest.Checked = false;

                    if (CareSend.HAS_REHABILITATION.HasValue && CareSend.HAS_REHABILITATION.Value == 1)
                        chkHasRehabilitaion.Checked = true;
                    else
                        chkHasRehabilitaion.Checked = false;

                    if (CareSend.IS_PUBLIC.HasValue && CareSend.IS_PUBLIC.Value == 1)
                        ChkIsPublic.Checked = true;
                    else
                        ChkIsPublic.Checked = false;

                    ListCareTempDetailADO = new List<ADO.CareTempDetailADO>();

                    if (CareSend.HIS_CARE_TEMP_DETAIL != null && CareSend.HIS_CARE_TEMP_DETAIL.Count > 0)
                    {
                        foreach (var item in CareSend.HIS_CARE_TEMP_DETAIL)
                        {
                            ADO.CareTempDetailADO hisCareDetailSDO = new ADO.CareTempDetailADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.CareTempDetailADO>(hisCareDetailSDO, item);
                            hisCareDetailSDO.Action = GlobalVariables.ActionEdit;
                            ListCareTempDetailADO.Add(hisCareDetailSDO);
                        }
                    }
                    else if (CareSend.ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisCareTempDetailFilter careTempDetailFilter = new MOS.Filter.HisCareTempDetailFilter();
                        careTempDetailFilter.CARE_TEMP_ID = CareSend.ID;
                        var lstHisCareDetail = new BackendAdapter(param).Get<List<HIS_CARE_TEMP_DETAIL>>("api/HisCareTempDetail/Get", ApiConsumers.MosConsumer, careTempDetailFilter, param);

                        if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
                        {
                            foreach (var item_CareDetail in lstHisCareDetail)
                            {
                                ADO.CareTempDetailADO hisCareDetailSDO = new ADO.CareTempDetailADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.CareTempDetailADO>(hisCareDetailSDO, item_CareDetail);
                                hisCareDetailSDO.Action = GlobalVariables.ActionEdit;
                                ListCareTempDetailADO.Add(hisCareDetailSDO);
                            }
                        }
                    }

                    gridControlCareDetail.BeginUpdate();
                    if (ListCareTempDetailADO != null && ListCareTempDetailADO.Count > 0)
                    {
                        ListCareTempDetailADO.First().Action = GlobalVariables.ActionAdd;
                        gridControlCareDetail.DataSource = ListCareTempDetailADO;
                    }
                    else
                    {
                        ADO.CareTempDetailADO hisCareDetailSDO = new ADO.CareTempDetailADO();
                        hisCareDetailSDO.Action = GlobalVariables.ActionAdd;
                        ListCareTempDetailADO.Add(hisCareDetailSDO);
                        gridControlCareDetail.DataSource = null;
                        gridControlCareDetail.DataSource = ListCareTempDetailADO;
                    }
                    gridControlCareDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridLeft()
        {
            try
            {
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlCareTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                MOS.Filter.HisCareTempFilter filter = new MOS.Filter.HisCareTempFilter();
                filter.KEY_WORD = TxtKeyword.Text.Trim();
                if (!IsAdmin)
                    filter.DATA_DOMAIN_FILTER = true;

                var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HIS_CARE_TEMP>>("api/HisCareTemp/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (apiResult != null)
                {
                    var data = apiResult.Data;

                    gridControlCareTemp.BeginUpdate();
                    if (data != null && data.Count > 0)
                    {
                        List<ADO.CareTempADO> listData = new List<ADO.CareTempADO>();
                        foreach (var item in data)
                        {
                            listData.Add(new ADO.CareTempADO(item));
                        }
                        gridControlCareTemp.DataSource = listData;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlCareTemp.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    gridControlCareTemp.EndUpdate();
                }
                WaitingManager.Hide();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridControlCareTemp.EndUpdate();
            }
        }

        private void SetDefaultDataControl()
        {
            try
            {
                this.CareSave = new HIS_CARE_TEMP();
                TxtCareTempCode.Text = "";
                TxtCareTempName.Text = "";
                txtAwareness.Text = "";
                txtCareDescription.Text = "";
                txtDejecta.Text = "";
                txtEducation.Text = "";
                txtInstructionDescription.Text = "";
                txtMucocutaneous.Text = "";
                txtNutrition.Text = "";
                txtSanitary.Text = "";
                txtTutorial.Text = "";
                txtUrine.Text = "";
                chkHasAddMedicine.Checked = false;
                chkHasMedicine.Checked = false;
                chkHasTest.Checked = false;
                chkHasRehabilitaion.Checked = false;
                ChkIsPublic.Checked = false;

                TxtCareTempCode.Focus();
                TxtCareTempCode.SelectAll();

                InitCareDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.BtnAdd.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__BTN_ADD");
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__BTN_REFRESH");
                this.btnSave.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__BTN_SAVE");
                this.GcTemp_Awareness.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_AWARENESS");
                this.GcTemp_CareDescription.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_CARE_DESCRIPTION");
                this.GcTemp_Dejecta.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_DEJECTA");
                this.GcTemp_Education.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_EDUCATION");
                this.GcTemp_InstructionDescription.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_INSTRUCTION_DESCRIPTION");
                this.GcTemp_Mucocutaneous.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_MUCOCUTANEOUS");
                this.GcTemp_Nutrition.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_NUTRITION");
                this.GcTemp_Sanitary.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_SANITARY");
                this.GcTemp_STT.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_STT");
                this.GcTemp_Tutorial.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_TUTORIAL");
                this.GcTemp_Urine.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_URINE");
                this.GcTemp_CreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_CREATE_TIME");
                this.GcTemp_Creator.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_CREATOR");
                this.GcTemp_Modifier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_MODIFIER");
                this.GcTemp_ModifyTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_MODIFY_TIME");
                this.grdColCareTypeContent.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_CARE_TYPE_CONTENT");
                this.grdColCareTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_CARE_TYPE_NAME");
                this.grcTrackCare.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GROUP_TRACK_CARE");
                this.groupControl2.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GROUP_DESCRIPTION");
                this.LciAwareness.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_AWARENESS");
                this.LciCareDescription.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_CARE_DESCRIPTION");
                this.lciDejecta.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_DEJECTA");
                this.lciEducation.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_EDUCATION");
                this.lciHasAddMedicine.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_HAS_ADD_MEDICINE");
                this.lciHasMedicine.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_HAS_MEDICINE");
                this.lciHasTest.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_HAS_TEST");
                this.LciInstructionDescription.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_INSTRUCTION_DESCRIPTION");
                this.lciMucocutaneous.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_MUCOCUTANEOUS");
                this.lciNutrition.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_NUTRITION");
                this.lciSanitary.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_SANITARY");
                this.lciTutorial.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_TUTORIAL");
                this.lciUrine.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_URINE");
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__RP_BTN_DELETE");
                this.repositoryItemBtnDeleteDisable.Buttons[0].ToolTip = this.repositoryItemBtnDelete.Buttons[0].ToolTip;
                this.repositoryItemBtnLock.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__RP_BTN_UNLOCK");
                this.repositoryItemBtnUnLock.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__RP_BTN_LOCK");
                this.repositoryItemBtnAdd.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__RP_BTN_ADD");
                this.repositoryItemBtnRemove.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__RP_BTN_REMOVE");
                this.TxtKeyword.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__TXT_KEYWORD__NULL_VALUE");
                this.LciCode.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_CARE_TEMP_CODE");
                this.LciName.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_CARE_TEMP_NAME");
                this.GcTemp_CareTempCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_CARE_TEMP_CODE");
                this.GcTemp_CareTempName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_CARE_TEMP_NAME");
                this.LciIsPublic.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__LCI_IS_PUBLIC");
                this.GcTemp_Public.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_CARE_TEMP__GC_TEMP_PUBLIC");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageFormCareTemp, cultureLang);
        }

        private void LoadDataCareDetail()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTypeFilter hisCareTypeFilter = new MOS.Filter.HisCareTypeFilter();
                hisCareTypeFilter.ORDER_DIRECTION = "DESC";
                hisCareTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                LstHisCareType = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, hisCareTypeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboCareType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboCareType, List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE> data)
        {
            try
            {
                cboCareType.DataSource = data;
                cboCareType.DisplayMember = "CARE_TYPE_NAME";
                cboCareType.ValueMember = "ID";
                cboCareType.NullText = "";

                cboCareType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboCareType.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboCareType.ImmediatePopup = true;
                cboCareType.View.Columns.Clear();

                GridColumn aColumnCode = cboCareType.View.Columns.AddField("CARE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboCareType.View.Columns.AddField("CARE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 400;

                cboCareType.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoi tao Du Lieu Grid CARE_DETAIL
        /// </summary>
        private void InitCareDetail()
        {
            try
            {
                this.ListCareTempDetailADO = new List<ADO.CareTempDetailADO>();
                ADO.CareTempDetailADO hisCareDetail = new ADO.CareTempDetailADO();
                hisCareDetail.Action = GlobalVariables.ActionAdd;
                this.ListCareTempDetailADO.Add(hisCareDetail);
                gridControlCareDetail.BeginUpdate();
                gridControlCareDetail.DataSource = null;
                gridControlCareDetail.DataSource = this.ListCareTempDetailADO;
                gridControlCareDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Grid
        private void gridControlCareDetail_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        if (btnSave.Enabled)
                            btnSave.Focus();
                        else if (BtnAdd.Enabled)
                            BtnAdd.Focus();
                        //grid.SelectNextControl(btnAdd, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.CareTempDetailADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.CareTempDetailADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnAddDelete")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemBtnAdd;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnRemove;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.CareTempDetailADO data = view.GetFocusedRow() as ADO.CareTempDetailADO;
                if (view.FocusedColumn.FieldName == "CARE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        if (editor.EditValue == null)//xemlai...
                        {
                            string error = GetError(data, gridViewCareDetail.FocusedRowHandle, gridViewCareDetail.FocusedColumn);
                            if (error == string.Empty) return;
                            gridViewCareDetail.SetColumnError(gridViewCareDetail.FocusedColumn, error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareTemp_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (ADO.CareTempADO)gridViewCareTemp.GetFocusedRow();
                if (row != null)
                {
                    HIS_CARE_TEMP data = new HIS_CARE_TEMP();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CARE_TEMP>(data, row);
                    CareSave = data;
                    FillDataToControlRight(data);
                    EnableControlChanged(GlobalVariables.ActionEdit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareTemp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        long createTime = long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString());
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(createTime);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        long modifyTime = long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString());
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(modifyTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareTemp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                short IS_ACTIVE = short.Parse((view.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "0").ToString());
                string CREATOR = (view.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IS_ACTIVE_LOCK")
                    {
                        e.RepositoryItem = (IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? repositoryItemBtnLock : repositoryItemBtnUnLock);
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        if (IsAdmin)
                        {
                            e.RepositoryItem = repositoryItemBtnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = LogginName != CREATOR ? repositoryItemBtnDeleteDisable : (IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? repositoryItemBtnDeleteDisable : repositoryItemBtnDelete);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region PreviewKeyDown
        private void TxtCareTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtCareTempName.Focus();
                    TxtCareTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtCareTempName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkIsPublic.Focus();
                    ChkIsPublic.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkIsPublic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSanitary.Focus();
                    txtSanitary.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSanitary_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAwareness.Focus();
                    txtAwareness.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAwareness_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMucocutaneous.Focus();
                    txtMucocutaneous.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMucocutaneous_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEducation.Focus();
                    txtEducation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEducation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUrine.Focus();
                    txtUrine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUrine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDejecta.Focus();
                    txtDejecta.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDejecta_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHasMedicine.Focus();
                    chkHasMedicine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHasAddMedicine.Focus();
                    chkHasAddMedicine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasAddMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHasTest.Focus();
                    chkHasTest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasTest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHasRehabilitaion.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridLeft();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Click
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
                BtnAdd_Click(null, null);
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
                BtnRefresh_Click(null, null);
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
                btnSave.Focus();
                if (!btnSave.Enabled) return;

                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                if (gridViewCareDetail.IsEditing)
                    gridViewCareDetail.CloseEditor();

                if (gridViewCareDetail.FocusedRowModified)
                    gridViewCareDetail.UpdateCurrentRow();

                if (gridViewCareDetail.HasColumnErrors)
                    return;

                if (Check())
                {
                    SaveCareProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                BtnAdd.Focus();
                if (!BtnAdd.Enabled) return;

                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                if (gridViewCareDetail.IsEditing)
                    gridViewCareDetail.CloseEditor();

                if (gridViewCareDetail.FocusedRowModified)
                    gridViewCareDetail.UpdateCurrentRow();

                if (gridViewCareDetail.HasColumnErrors)
                    return;

                if (Check())
                {
                    SaveCareProcess();
                }
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
                SetDefaultDataControl();
                EnableControlChanged(GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ADO.CareTempDetailADO hisCareDetail = new ADO.CareTempDetailADO();
                hisCareDetail.Action = GlobalVariables.ActionEdit;
                this.ListCareTempDetailADO.Add(hisCareDetail);
                gridControlCareDetail.DataSource = null;
                gridControlCareDetail.DataSource = this.ListCareTempDetailADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnRemove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var hisCareDetail = (ADO.CareTempDetailADO)gridViewCareDetail.GetFocusedRow();
                if (hisCareDetail != null)
                {
                    this.ListCareTempDetailADO.Remove(hisCareDetail);
                    gridControlCareDetail.DataSource = null;
                    gridControlCareDetail.DataSource = this.ListCareTempDetailADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridCareTypeName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareTypeAdd").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CareTypeAdd");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        moduleData.RoomId = this.moduleData.RoomId;
                        moduleData.RoomTypeId = this.moduleData.RoomTypeId;
                        listArgs.Add(moduleData);
                        listArgs.Add((HIS.Desktop.Common.DelegateReturnSuccess)ProcessAfterSave);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.CareTempADO)gridViewCareTemp.GetFocusedRow();
                if (row != null)
                {
                    ChangeLock(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUnLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.CareTempADO)gridViewCareTemp.GetFocusedRow();
                if (row != null)
                {
                    ChangeLock(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.CareTempADO)gridViewCareTemp.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceLanguageManager.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceLanguageManager.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                          (param).Post<bool>
                          ("api/HisCareTemp/Delete", ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGridLeft();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
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
            }
        }
        #endregion

        #region Validate
        private void InitValidateColtrol()
        {
            try
            {
                ValidateRule.TextEditValidationRule code = new ValidateRule.TextEditValidationRule();
                code.required = true;
                code.txtEdit = TxtCareTempCode;
                code.maxLenght = 6;
                code.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                code.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(TxtCareTempCode, code);


                ValidateRule.TextEditValidationRule name = new ValidateRule.TextEditValidationRule();
                name.required = true;
                name.txtEdit = TxtCareTempName;
                name.maxLenght = 100;
                name.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                name.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(TxtCareTempName, name);

                SetValidateControl(txtAwareness, false, 100);
                SetValidateControl(txtCareDescription, false, 3000);
                SetValidateControl(txtDejecta, false, 200);
                SetValidateControl(txtEducation, false, 200);
                SetValidateControl(txtInstructionDescription, false, 3000);
                SetValidateControl(txtMucocutaneous, false, 200);
                SetValidateControl(txtNutrition, false, 200);
                SetValidateControl(txtSanitary, false, 200);
                SetValidateControl(txtTutorial, false, 200);
                SetValidateControl(txtUrine, false, 200);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidateControl(TextEdit txt, bool required, long lenght)
        {
            try
            {
                ValidateRule.TextEditValidationRule rule = new ValidateRule.TextEditValidationRule();
                rule.required = required;
                rule.txtEdit = txt;
                rule.maxLenght = lenght;
                rule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txt, rule);
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
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Processor
        private void ChangeLock(ADO.CareTempADO row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                          (param).Post<HIS_CARE_TEMP>
                          ("api/HisCareTemp/ChangeLock", ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGridLeft();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAfterSave(bool succsess)
        {
            try
            {
                if (!succsess)
                    return;
                LoadDataCareDetail();
                LoadDataToComboCareType(repositoryItemGridCareTypeName, LstHisCareType);
                grdColCareTypeName.ColumnEdit = repositoryItemGridCareTypeName;
                var data = (ADO.CareTempDetailADO)gridViewCareDetail.GetFocusedRow();
                if (gridViewCareDetail.EditingValue != null)
                {
                    data.CARE_TYPE_ID = LstHisCareType[0].ID;
                    //xem lại
                }
                else
                {
                    data.CARE_TYPE_ID = LstHisCareType[0].ID;
                }
                gridViewCareDetail.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveCareProcess()
        {
            try
            {
                ProcessDataCare(ref CareSave);
                SaveDataCare(CareSave);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();
                if (this.ListCareTempDetailADO == null || this.ListCareTempDetailADO.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListCareTempDetailADO), ListCareTempDetailADO));

                var groupCareType = from p in this.ListCareTempDetailADO
                                    group p by p.CARE_TYPE_ID into g
                                    select new { Key = g.Key, CareDetail = g.ToList() };
                if (groupCareType != null && groupCareType.Count() > 0)
                {
                    foreach (var item in groupCareType)
                    {
                        if (item.CareDetail.Count > 1)
                        {
                            result = false;
                            param.Messages.Add(Resources.ResourceLanguageManager.ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc);
                            break;
                        }
                    }
                }

                #region Show message
                MessageManager.Show(param, null);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessDataCare(ref HIS_CARE_TEMP hisCare)
        {
            try
            {
                if (hisCare == null)
                {
                    hisCare = new HIS_CARE_TEMP();
                }
                hisCare.HIS_CARE_TEMP_DETAIL = new List<HIS_CARE_TEMP_DETAIL>();

                hisCare.CARE_TEMP_CODE = TxtCareTempCode.Text.Trim();
                hisCare.CARE_TEMP_NAME = TxtCareTempName.Text.Trim();

                hisCare.AWARENESS = txtAwareness.Text.Trim();
                hisCare.DEJECTA = txtDejecta.Text.Trim();
                hisCare.EDUCATION = txtEducation.Text.Trim();
                hisCare.MUCOCUTANEOUS = txtMucocutaneous.Text.Trim();
                hisCare.NUTRITION = txtNutrition.Text.Trim();
                hisCare.SANITARY = txtSanitary.Text.Trim();
                hisCare.TUTORIAL = txtTutorial.Text.Trim();
                hisCare.URINE = txtUrine.Text.Trim();
                hisCare.INSTRUCTION_DESCRIPTION = txtInstructionDescription.Text;
                hisCare.CARE_DESCRIPTION = txtCareDescription.Text;

                if (chkHasAddMedicine.Checked)
                    hisCare.HAS_ADD_MEDICINE = (short)1;
                else
                    hisCare.HAS_ADD_MEDICINE = null;

                if (chkHasMedicine.Checked)
                    hisCare.HAS_MEDICINE = (short)1;
                else
                    hisCare.HAS_MEDICINE = null;

                if (chkHasTest.Checked)
                    hisCare.HAS_TEST = (short)1;
                else
                    hisCare.HAS_TEST = null;

                if (chkHasRehabilitaion.Checked)
                    hisCare.HAS_REHABILITATION = (short)1;
                else
                    hisCare.HAS_REHABILITATION = null;

                if (ChkIsPublic.Checked)
                    hisCare.IS_PUBLIC = (short)1;
                else
                    hisCare.IS_PUBLIC = null;

                if (this.ListCareTempDetailADO != null)
                {
                    foreach (var item in this.ListCareTempDetailADO)
                    {
                        if (item.CARE_TYPE_ID > 0 && !String.IsNullOrEmpty(item.CONTENT))
                        {
                            HIS_CARE_TEMP_DETAIL detail = new HIS_CARE_TEMP_DETAIL();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CARE_TEMP_DETAIL>(detail, item);
                            hisCare.HIS_CARE_TEMP_DETAIL.Add(detail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveDataCare(HIS_CARE_TEMP hisCare)
        {
            if (hisCare == null) throw new ArgumentNullException("hisCare is null");
            HIS_CARE_TEMP rsHisCare = null;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                if (hisCare.ID > 0)
                {
                    if (IsAdmin || hisCare.CREATOR == LogginName)
                    {
                        rsHisCare = new BackendAdapter(param).Post<HIS_CARE_TEMP>("api/HisCareTemp/Update", ApiConsumers.MosConsumer, hisCare, param);
                    }
                    else
                    {
                        MessageBox.Show(Resources.ResourceLanguageManager.BanKhongCoQuyenSuaNoiDung);
                        return;
                    }
                }
                else
                    rsHisCare = new BackendAdapter(param).Post<HIS_CARE_TEMP>("api/HisCareTemp/Create", ApiConsumers.MosConsumer, hisCare, param);

                if (rsHisCare != null && rsHisCare.ID > 0)
                {
                    success = true;
                    this.CareSave = rsHisCare;
                    EnableControlChanged(GlobalVariables.ActionEdit);
                    FillDataToGridLeft();
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetError(ADO.CareTempDetailADO data, int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "CARE_TYPE_ID")
                {
                    if (data == null)
                        return string.Empty;

                    if (data.CARE_TYPE_ID <= 0)
                    {
                        return "Không có thông tin loại chăm sóc.";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return string.Empty;
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnSave.Enabled = (action == GlobalVariables.ActionEdit);
                BtnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void chkHasRehabilitaion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNutrition.Focus();
                    txtNutrition.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
