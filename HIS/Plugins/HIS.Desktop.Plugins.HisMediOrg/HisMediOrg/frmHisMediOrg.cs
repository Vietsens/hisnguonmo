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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisMediOrg.HisMediOrg;

namespace HIS.Desktop.Plugins.HisMediOrg
{
    public partial class frmHisMediOrg : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_MEDI_ORG currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisMediOrg(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisMediOrg_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMediOrg.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMediOrg.frmHisMediOrg).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMediOrg.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lciBedTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lciBedTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());             
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediOrg.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lciProvince.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistricts.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lciDistricts.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("frmHisMediOrg.lciCommune.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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


        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                ResetFormData();
                EnableControlChanged(this.ActionType);
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtMediOrgCode", 0);
                dicOrderTabIndexControl.Add("txtMediOrgName", 1);
                dicOrderTabIndexControl.Add("txtProvince", 2);
                dicOrderTabIndexControl.Add("cboProvince", 3);
                dicOrderTabIndexControl.Add("txtDistricts", 4);
                dicOrderTabIndexControl.Add("cboDistricts", 5);
                dicOrderTabIndexControl.Add("txtCommune", 6);
                dicOrderTabIndexControl.Add("cboCommune", 7);
                dicOrderTabIndexControl.Add("txtAddress", 8);
                dicOrderTabIndexControl.Add("txtRankCode", 9);
                dicOrderTabIndexControl.Add("txtLevelCode", 10);

                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void FillDataToControlsForm()
        {
            try
            {
                LoadDataToCombo();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {

                List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResultProvince = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                FillDataToLookupedit(this.cboProvince, "PROVINCE_NAME", "PROVINCE_CODE", "PROVINCE_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>());

                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResultDistricts = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                FillDataToLookupedit(this.cboDistricts, "DISTRICT_NAME", "DISTRICT_CODE", "DISTRICT_CODE", BackendDataWorker.Get<V_SDA_DISTRICT>());

                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResultCommune = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                FillDataToLookupedit(this.cboCommune, "COMMUNE_NAME", "COMMUNE_CODE", "COMMUNE_CODE", BackendDataWorker.Get<V_SDA_COMMUNE>());



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToLookupedit(LookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayMember, "", 100));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>> apiResult = null;
                HisMediOrgFilter filter = new HisMediOrgFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>>(HisRequestUriStore.MOSHIS_MEDI_ORG_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisMediOrgFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_MEDI_ORG pData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
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

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_MEDI_ORG data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_MEDI_ORG data)
        {
            try
            {
                if (data != null)
                {
                    txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                    txtMediOrgName.Text = data.MEDI_ORG_NAME;
                    txtProvince.Text = data.PROVINCE_CODE;
                    cboProvince.EditValue = data.PROVINCE_CODE;
                    txtDistricts.Text = data.DISTRICT_CODE;
                    cboDistricts.EditValue = data.DISTRICT_CODE;
                    txtCommune.Text = data.COMMUNE_CODE;
                    cboCommune.EditValue = data.COMMUNE_CODE;
                    txtAddress.Text = data.ADDRESS;
                    txtLevelCode.Text = data.LEVEL_CODE;
                    txtRankCode.Text = data.RANK_CODE;
                    //spMaxCapacity.EditValue = data.MAX_CAPACITY;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                //TODO
                txtMediOrgCode.Focus();
                txtMediOrgCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtMediOrgCode.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_MEDI_ORG currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediOrgFilter filter = new HisMediOrgFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>>(HisRequestUriStore.MOSHIS_MEDI_ORG_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                ////FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisMediOrgFilter filter = new HisMediOrgFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<HIS_MEDI_ORG>>(HisRequestUriStore.MOSHIS_MEDI_ORG_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_MEDI_ORG_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_MEDI_ORG>)gridControlFormList.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<HIS_MEDI_ORG>();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void btnCancel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.ActionType = GlobalVariables.ActionAdd;
        //        EnableControlChanged(this.ActionType);
        //        positionHandle = -1;
        //        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
        //        ResetFormData();
        //        SetFocusEditor();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnEdit_Click(object sender, EventArgs e)
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

        private void btnAdd_Click(object sender, EventArgs e)
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
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_MEDI_ORG updateDTO = new MOS.EFMODEL.DataModels.HIS_MEDI_ORG();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(HisRequestUriStore.MOSHIS_MEDI_ORG_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(HisRequestUriStore.MOSHIS_MEDI_ORG_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {

                    SetFocusEditor();
                    BackendDataWorker.Reset<HIS_MEDI_ORG>();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_MEDI_ORG data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_MEDI_ORG) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MEDI_ORG currentDTO)
        {
            try
            {
                currentDTO.MEDI_ORG_CODE = txtMediOrgCode.Text.Trim();
                currentDTO.MEDI_ORG_NAME = txtMediOrgName.Text.Trim();
                currentDTO.PROVINCE_CODE = txtProvince.Text != null ? txtProvince.Text : null;
                currentDTO.PROVINCE_NAME = GetProvincesByCombo(cboProvince);
                currentDTO.DISTRICT_CODE = txtDistricts.Text != null ? txtDistricts.Text : null;
                currentDTO.DISTRICT_NAME = GetDistrictsByCombo(cboDistricts);
                currentDTO.COMMUNE_CODE = txtCommune.Text != null ? txtCommune.Text : null;
                currentDTO.COMMUNE_NAME = GetCommuneByCombo(cboCommune);
                currentDTO.RANK_CODE = txtRankCode.Text.Trim();
                currentDTO.LEVEL_CODE = txtLevelCode.Text.Trim();
                currentDTO.MEDI_ORG_NAME = txtMediOrgName.Text.Trim();
                currentDTO.ADDRESS = txtAddress.Text.Trim();

                //currentDTO.MAX_CAPACITY = (long)spMaxCapacity.Value;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetCommuneByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var commune = BackendDataWorker.Get<V_SDA_COMMUNE>();
                    if (commune != null)
                    {
                        var selectedCommune = commune.FirstOrDefault(o => o.COMMUNE_CODE == cbo.EditValue.ToString()) ?? new V_SDA_COMMUNE();
                        edit = selectedCommune.INITIAL_NAME + " " + selectedCommune.COMMUNE_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }

        private string GetDistrictsByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var districts = BackendDataWorker.Get<V_SDA_DISTRICT>();
                    if (districts != null)
                    {
                        var selectedDistrict = districts.FirstOrDefault(o => o.DISTRICT_CODE == cbo.EditValue.ToString()) ?? new V_SDA_DISTRICT();
                        edit = selectedDistrict.INITIAL_NAME + " " + selectedDistrict.DISTRICT_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }

        private string GetProvincesByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var provinces = BackendDataWorker.Get<V_SDA_PROVINCE>();
                    if (provinces != null)
                    {
                        edit = (provinces.FirstOrDefault(o => o.PROVINCE_CODE == cbo.EditValue.ToString()) ?? new V_SDA_PROVINCE()).PROVINCE_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtMediOrgCode);
                ValidationSingleControl(txtMediOrgName);
                ValidationSingleControl(txtLevelCode);
                //ValidationSingleControl1();
                ValidationControlWarnigProvince();
                ValidationControlWarnigDistricts();
                ValidationControlWarnigCommune();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlWarnigCommune()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtCommune;
            validation.cbo = cboCommune;
            dxValidationProviderEditorInfo.SetValidationRule(txtCommune, validation);
        }

        private void ValidationControlWarnigDistricts()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtDistricts;
            validation.cbo = cboDistricts;
            dxValidationProviderEditorInfo.SetValidationRule(txtDistricts, validation);
        }

        private void ValidationControlWarnigProvince()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtProvince;
            validation.cbo = cboProvince;
            dxValidationProviderEditorInfo.SetValidationRule(txtProvince, validation);
        }

        private void ValidationSingleControl1()
        {
            try
            {
                //ValidatespMax validate = new ValidatespMax();
                //validate.spMax = spMaxCapacity;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                //validate.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderEditorInfo.SetValidationRule(spMaxCapacity, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_MEDI_ORG success = new HIS_MEDI_ORG();
            bool notHandler = false;
            try
            {

                HIS_MEDI_ORG data = (HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDI_ORG data1 = new HIS_MEDI_ORG();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_ORG>(HisRequestUriStore.MOSHIS_MEDI_ORG_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                        BackendDataWorker.Reset<HIS_MEDI_ORG>();
                    }
                    MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnGunLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_ORG success = new HIS_MEDI_ORG();
            bool notHandler = false;
            try
            {

                HIS_MEDI_ORG data = (HIS_MEDI_ORG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDI_ORG data1 = new HIS_MEDI_ORG();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_ORG>(HisRequestUriStore.MOSHIS_MEDI_ORG_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                        BackendDataWorker.Reset<HIS_MEDI_ORG>();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_MEDI_ORG data = (HIS_MEDI_ORG)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spMaxCapacity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                HIS_MEDI_ORG data = (HIS_MEDI_ORG)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_MEDI_ORG)).ToString(), false);
                MessageManager.Show(this, param, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                BackendDataWorker.Reset<HIS_MEDI_ORG>();
                MessageManager.Show(this, param, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLevelCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 49 || e.KeyChar > 52))
            {
                if (e.KeyChar == 127 || e.KeyChar == 8)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

        }

        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceCombo(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboDistricts.Properties.DataSource = null;
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboProvince.EditValue = null;
                    FocusShowPopup(cboProvince);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        txtProvince.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtDistricts);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        cboDistricts.Properties.DataSource = null;
                        cboDistricts.EditValue = null;
                        txtDistricts.Text = "";
                        cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboProvince);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusMoveText(TextEdit txt)
        {
            try
            {
                txt.Focus();
                txt.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDistrictsCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                cboDistricts.Properties.DataSource = listResult;
                cboDistricts.Properties.DisplayMember = "RENDERER_DISTRICT_NAME";
                cboDistricts.Properties.ValueMember = "DISTRICT_CODE";
                cboDistricts.Properties.ForceInitialize();

                cboDistricts.Properties.Columns.Clear();
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_CODE", "", 100));
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_DISTRICT_NAME", "", 200));

                cboDistricts.Properties.ShowHeader = false;
                cboDistricts.Properties.ImmediatePopup = true;
                cboDistricts.Properties.DropDownRows = 20;
                cboDistricts.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    txtDistricts.Text = "";
                    cboDistricts.EditValue = null;
                    FocusShowPopup(cboDistricts);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboDistricts.EditValue = listResult[0].DISTRICT_CODE;
                        txtDistricts.Text = listResult[0].DISTRICT_CODE;
                        LoadCommuneCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtCommune);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        txtDistricts.Text = "";
                        cboDistricts.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboDistricts);
                            //PopupProcess.SelectFirstRowPopup(cboDistricts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();
                cboCommune.Properties.DataSource = listResult;
                cboCommune.Properties.DisplayMember = "RENDERER_COMMUNE_NAME";
                cboCommune.Properties.ValueMember = "COMMUNE_CODE";
                cboCommune.Properties.ForceInitialize();

                cboCommune.Properties.Columns.Clear();
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_CODE", "", 100));
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_COMMUNE_NAME", "", 200));

                cboCommune.Properties.ShowHeader = false;
                cboCommune.Properties.ImmediatePopup = true;
                cboCommune.Properties.DropDownRows = 20;
                cboCommune.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    FocusShowPopup(cboCommune);
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        txtCommune.Text = listResult[0].COMMUNE_CODE;

                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboCommune.EditValue = null;
                        FocusShowPopup(cboCommune);
                        // PopupProcess.SelectFirstRowPopup(cboCommune);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowPopup(LookUpEdit cboEditor)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboProvince.EditValue != null && cboProvince.EditValue != cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.PROVINCE_CODE;
                        }
                    }
                    txtDistricts.Text = "";
                    txtDistricts.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDistricts.EditValue != null && cboDistricts.EditValue != cboDistricts.OldEditValue)
                    {
                        string str = cboDistricts.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            txtDistricts.Text = district.DISTRICT_CODE;
                            if (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                cboDistricts.EditValue = district.PROVINCE_CODE;
                                txtProvince.Text = district.PROVINCE_CODE;
                            }
                            LoadCommuneCombo("", district.DISTRICT_CODE, false);
                            txtDistricts.Text = district.DISTRICT_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                        }
                    }
                    FocusMoveText(this.txtCommune);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null && cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboDistricts.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtCommune.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                txtDistricts.Text = commune.DISTRICT_CODE;
                                cboDistricts.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null && String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()))
                                {

                                    txtProvince.Text = district.PROVINCE_CODE;
                                    cboCommune.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDistricts.Properties.Buttons[1].Visible = false;
                if (cboDistricts.EditValue != null)
                {
                    cboDistricts.Properties.Buttons[1].Visible = true;
                    LoadCommuneCombo("", cboDistricts.EditValue.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboCommune.Properties.Buttons[1].Visible = false;
                if (cboCommune.EditValue != null)
                {
                    cboCommune.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProvince.EditValue != null)
                    {
                        string str = cboProvince.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboDistricts);
        }

        private void cboCommune_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboCommune);
        }
        private void GetNotInListValue(object sender, GetNotInListValueEventArgs e, LookUpEdit cbo)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }

                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }

                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }

                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} - {1} {2} - {3}", item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboProvince.Properties.Buttons[1].Visible = false;
                if (cboProvince.EditValue != null)
                {
                    cboProvince.Properties.Buttons[1].Visible = true;
                    LoadDistrictsCombo("", cboProvince.EditValue.ToString(), false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvince_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboProvince.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProvince.EditValue = null;
                    txtProvince.Text = "";
                    cboProvince.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboDistricts.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboDistricts.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboCommune.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboCommune.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceCode = cboProvince.EditValue.ToString();
                    }
                    LoadDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommune_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtCode = "";
                    if (cboDistricts.EditValue != null)
                    {
                        districtCode = cboDistricts.EditValue.ToString();
                    }
                    LoadCommuneCombo(strValue.ToUpper(), districtCode, true);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {

        }
    }
}
