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
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisIcd
{
    public partial class frmHisIcd : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_ICD currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisIcd(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisIcd_Load(object sender, EventArgs e)
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
                //Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisIcd.Resources.Lang", typeof(HIS.Desktop.Plugins.HisIcd.frmHisIcd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.gridColumnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdCode.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdName.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdNameEn.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdNameEn.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdNameEn.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdNameEn.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdNameCommon.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdNameCommon.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdNameCommon.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdNameCommon.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdGroupId.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdGroupId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdGroupId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIcdGroupId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsHeinNds.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIsHeinNds.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsHeinNds.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIsHeinNds.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisIcd.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkIcdGroupId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisIcd.lkIcdGroupId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsHeinNds.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsHeinNds.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdCode.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIcdCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdName.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIcdName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdNameEn.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIcdNameEn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdNameCommon.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIcdNameCommon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdGroupId.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIcdGroupId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsHeinNds.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.lciIsHeinNds.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsCause.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsCause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRequireCause.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsRequireCause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkYhct.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.chkYhct.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsLatentTuberculosis.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsLatentTuberculosis.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsLatentTuberculosis.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsLatentTuberculosis.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                //
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
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboIcdGroupId();
                InitComboAgeType();
                InitComboGender();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboIcdGroupId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisIcdGroupFilter filter = new HisIcdGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_ICD_GROUP>>("api/HisIcdGroup/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ICD_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkIcdGroupId, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboAgeType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_AGE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("AGE_TYPE_CODE", "", 40, 1));
                columnInfos.Add(new ColumnInfo("AGE_TYPE_NAME", "", 80, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("AGE_TYPE_NAME", "ID", columnInfos, false, 120);
                ControlEditorLoader.Load(cboAgeType, data, controlEditorADO);
                cboAgeType.Properties.AllowNullInput = DefaultBoolean.True;
                cboAgeType.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboGender()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboGender, data, controlEditorADO);
                cboGender.Properties.AllowNullInput = DefaultBoolean.True;
                cboGender.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_ICD>> apiResult = null;
                HisIcdFilter filter = new HisIcdFilter();
                SetFilterNavBar(ref filter);

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ICD>>(HisRequestUriStore.MOSHIS_ICD_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_ICD>)apiResult.Data;
                    if (data != null)
                    {
                        //dnNavigation.DataSource = data;
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

        private void SetFilterNavBar(ref HisIcdFilter filter)
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ICD)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ICD)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.V_HIS_ICD pData = (MOS.EFMODEL.DataModels.V_HIS_ICD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_HEIN_NDS_CHECK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_HEIN_NDS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_HEIN_NDS_CHECK", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_REQUIRE_CAUSE_CHECK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_REQUIRE_CAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_REQUIRE_CAUSE_CHECK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_REQUIRE_CAUSE_CHECK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_REQUIRE_CAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_REQUIRE_CAUSE_CHECK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_CAUSE_CHECK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_CAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_CAUSE_CHECK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SWORD_CHK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_SWORD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot yhct IS_SWORD_CHK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SUBCODE_CHK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_SUBCODE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot yhct IS_SUBCODE_CHK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_COVID_CHK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_COVID == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot yhct IS_COVID_CHK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_YHCT")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_TRADITIONAL == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot yhct IS_YHCT", ex);
                        }
                    }
                    else if (e.Column.FieldName == "UNABLE_FOR_TREATMENT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.UNABLE_FOR_TREATMENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat UNABLE_FOR_TREATMENT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DO_NOT_USE_HEIN_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.DO_NOT_USE_HEIN == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot khong su dung cho đoi tuong BHYT DO_NOT_USE_HEIN_STR", ex);
                        }
                    }
                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_ICD data = (V_HIS_ICD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : DeleteDisable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                V_HIS_ICD data = (V_HIS_ICD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }


        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_ICD)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ICD)gridviewFormList.GetFocusedRow();
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_ICD data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_ICD data)
        {
            try
            {
                if (data != null)
                {
                    txtIcdCode.Text = data.ICD_CODE;
                    txtIcdName.Text = data.ICD_NAME;
                    txtIcdNameEn.Text = data.ICD_NAME_EN;
                    txtIcdNameCommon.Text = data.ICD_NAME_COMMON;
                    lkIcdGroupId.EditValue = data.ICD_GROUP_ID;
                    chkIsHeinNds.Checked = (data.IS_HEIN_NDS == 1 ? true : false);
                    chkIsCause.Checked = (data.IS_CAUSE == 1 ? true : false);
                    chkIsLatentTuberculosis.Checked = data.IS_LATENT_TUBERCULOSIS == 1;
                    chkIsRequireCause.Checked = (data.IS_REQUIRE_CAUSE == 1 ? true : false);
                    chkYhct.Checked = (data.IS_TRADITIONAL == 1 ? true : false);
                    chkChanDieuTri.Checked = (data.UNABLE_FOR_TREATMENT == 1 ? true : false);
                    chkNotUseHein.Checked = (data.DO_NOT_USE_HEIN == 1 ? true : false);
                    txtAttachIcd.Text = data.ATTACH_ICD_CODES;
                    spnAgeFrom.EditValue = data.AGE_FROM;
                    spnAgeTo.EditValue = data.AGE_TO;
                    cboAgeType.EditValue = data.AGE_TYPE_ID;
                    cboGender.EditValue = data.GENDER_ID;
                    chkIsSword.Checked = (data.IS_SWORD == 1 ? true : false);
                    chkIsSubcode.Checked = (data.IS_SUBCODE == 1 ? true : false);
                    chkIsCovid.Checked = (data.IS_COVID == 1 ? true : false);

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

        private void ResetFormData()
        {
            try
            {
                txtIcdCode.Text = "";
                txtIcdName.Text = "";
                txtIcdNameCommon.Text = "";
                txtIcdNameEn.Text = "";
                lkIcdGroupId.EditValue = null;
                chkIsCause.Checked = false;
                chkIsLatentTuberculosis.Checked = false;
                chkIsHeinNds.Checked = false;
                chkIsRequireCause.Checked = false;
                chkYhct.Checked = false;
                chkChanDieuTri.Checked = false;
                chkNotUseHein.Checked = false;
                txtAttachIcd.Text = null;
                spnAgeFrom.EditValue = null;
                spnAgeTo.EditValue = null;
                cboAgeType.EditValue = null;
                cboGender.EditValue = null;
                chkIsSword.Checked = false;
                chkIsSubcode.Checked = false;
                chkIsCovid.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_ICD currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisIcdFilter filter = new HisIcdFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_ICD>>(HisRequestUriStore.MOSHIS_ICD_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                SetDefaultValue();
                //FillDataToGridControl();
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_ICD)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_ICD_DELETE, ApiConsumers.MosConsumer, rowData, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        currentData = ((List<V_HIS_ICD>)gridControlFormList.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                if (spnAgeFrom.EditValue != null || spnAgeTo.EditValue != null)
                {
                    ValidationAge(cboAgeType, "loại tuổi", "tuổi từ, tuổi đến");
                    dxValidationProviderEditorInfo.SetValidationRule(spnAgeFrom, null);
                    if (spnAgeFrom.EditValue != null && spnAgeTo.EditValue != null)
                    {
                        if ((long)spnAgeFrom.Value > (long)spnAgeTo.Value)
                        {
                            XtraMessageBox.Show("Tuổi từ phải nhỏ hơn tuổi đến", "Thông báo");
                            return;
                        }
                    }
                }
                else if (cboAgeType.EditValue != null)
                {
                    ValidationAge(spnAgeFrom, "tuổi từ hoặc tuổi đến", "loại tuổi");
                    dxValidationProviderEditorInfo.SetValidationRule(cboAgeType, null);
                }
                else
                {
                    dxValidationProviderEditorInfo.SetValidationRule(spnAgeFrom, null);
                }

                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_ICD updateDTO = new MOS.EFMODEL.DataModels.HIS_ICD();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("api/HisIcd/Create INPUT updateDTO: ", updateDTO));
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ICD>(HisRequestUriStore.MOSHIS_ICD_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("api/HisIcd/Update INPUT updateDTO: ", updateDTO));
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ICD>(HisRequestUriStore.MOSHIS_ICD_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_ICD data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_ICD) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_ICD)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_ICD>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_ICD currentDTO)
        {
            try
            {
                currentDTO.ICD_CODE = txtIcdCode.Text.Trim();
                currentDTO.ICD_NAME = txtIcdName.Text.Trim();
                currentDTO.ICD_NAME_EN = txtIcdNameEn.Text.Trim();
                currentDTO.ICD_NAME_COMMON = txtIcdNameCommon.Text.Trim();
                if (lkIcdGroupId.EditValue != null) currentDTO.ICD_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkIcdGroupId.EditValue ?? "0").ToString());
                if (chkIsHeinNds.Checked)
                {
                    currentDTO.IS_HEIN_NDS = 1;
                }
                else
                {
                    currentDTO.IS_HEIN_NDS = null;
                }
                if (chkIsCause.Checked)
                {
                    currentDTO.IS_CAUSE = 1;
                }
                else
                {
                    currentDTO.IS_CAUSE = null;
                }
                if (chkIsRequireCause.Checked)
                {
                    currentDTO.IS_REQUIRE_CAUSE = 1;
                }
                else
                {
                    currentDTO.IS_REQUIRE_CAUSE = null;
                }
                if (chkYhct.Checked)
                {
                    currentDTO.IS_TRADITIONAL = 1;
                }
                else
                {
                    currentDTO.IS_TRADITIONAL = null;
                }
                if (chkChanDieuTri.Checked)
                {
                    currentDTO.UNABLE_FOR_TREATMENT = 1;
                }
                else
                {
                    currentDTO.UNABLE_FOR_TREATMENT = null;
                }
                currentDTO.IS_LATENT_TUBERCULOSIS = chkIsLatentTuberculosis.Checked ? (short?)1 : null;
                if (chkNotUseHein.Checked)
                {
                    currentDTO.DO_NOT_USE_HEIN = 1;
                }
                else
                {
                    currentDTO.DO_NOT_USE_HEIN = null;
                }
                currentDTO.ATTACH_ICD_CODES = txtAttachIcd.Text;
                if (cboAgeType.EditValue != null)
                {
                    currentDTO.AGE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboAgeType.EditValue.ToString());
                }
                else
                {
                    currentDTO.AGE_TYPE_ID = null;
                }
                if (spnAgeFrom.EditValue != null)
                {
                    currentDTO.AGE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(spnAgeFrom.Value.ToString());
                }
                else
                {
                    currentDTO.AGE_FROM = null;
                }
                if (spnAgeTo.EditValue != null)
                {
                    currentDTO.AGE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(spnAgeTo.Value.ToString());
                }
                else
                {
                    currentDTO.AGE_TO = null;
                }
                if (cboGender.EditValue != null)
                {
                    currentDTO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                }
                else
                {
                    currentDTO.GENDER_ID = null;
                }
                currentDTO.IS_SWORD = chkIsSword.Checked ? (short?)1 : null;
                currentDTO.IS_SUBCODE = chkIsSubcode.Checked ? (short?)1 : null;
                currentDTO.IS_COVID = chkIsCovid.Checked ? (short?)1 : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtIcdCode);
                ValidationSingleControl(txtIcdName);
                ValidationMaxLength(txtAttachIcd, 2000, false);

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
        private void ValidationAge(BaseEdit control, string text1, string text2)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format("Bắt buộc nhập {0} trong trường hợp nhập thông tin {1}", text1, text2);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationMaxLength(Control control, int? maxLength, bool required = false)
        {
            try
            {
                ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.IsRequired = required;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
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
                btnCancel_Click(null, null);
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

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_ICD success = new HIS_ICD();
            //bool notHandler = false;
            try
            {
                V_HIS_ICD data = (V_HIS_ICD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_ICD data1 = new HIS_ICD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ICD>(HisRequestUriStore.MOSHIS_ICD_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_ICD success = new HIS_ICD();
            //bool notHandler = false;
            try
            {
                V_HIS_ICD data = (V_HIS_ICD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_ICD data1 = new HIS_ICD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ICD>(HisRequestUriStore.MOSHIS_ICD_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdName.Focus();
                    txtIcdName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtIcdName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdNameEn.Focus();
                    txtIcdNameEn.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtIcdNameEn_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdNameCommon.Focus();
                    txtIcdNameCommon.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtIcdNameCommon_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkIcdGroupId.Focus();
                    if (lkIcdGroupId.EditValue == null)
                    {
                        lkIcdGroupId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void lkIcdGroupId_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAttachIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsCause_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsCause.Checked = !chkIsCause.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsHeinNds.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsRequireCause_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsRequireCause.Checked = !chkIsRequireCause.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkYhct.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsHeinNds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsHeinNds.Checked = !chkIsHeinNds.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsRequireCause.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_SERVICE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_ICD)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_ICD_GROUP)).ToString(), false);
            MessageBox.Show("Xử lý thành công", "Thông báo");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            BackendDataWorker.Reset<V_HIS_SERVICE>();
            BackendDataWorker.Reset<HIS_SERVICE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
            BackendDataWorker.Reset<ServiceComboADO>();
            BackendDataWorker.Reset<HIS_ICD>();
            MessageBox.Show("Xử lý thành công", "Thông báo");
        }

        private void chkYhct_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkYhct.Checked = !chkYhct.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkChanDieuTri.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisIcdImport").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisIcdImport");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkChanDieuTri_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkChanDieuTri.Checked = !chkChanDieuTri.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotUseHein.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsLatentTuberculosis_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsLatentTuberculosis.Checked = !chkIsLatentTuberculosis.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsCause.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkNotUseHein_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkNotUseHein.Checked = !chkNotUseHein.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsCovid.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spnAgeFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void spnAgeTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAttachIcd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    frmAttachIcd FormSecondaryIcd = new frmAttachIcd(stringIcds, this.txtAttachIcd.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    FormSecondaryIcd.ShowDialog();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    spnAgeFrom.Focus();
                    spnAgeFrom.SelectAll();

                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void stringIcds(string icdCode)
        {
            try
            {
                txtAttachIcd.Text = icdCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnAgeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnAgeTo.Focus();
                    spnAgeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spnAgeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAgeType.Focus();
                    cboAgeType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboAgeType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGender_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSword.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsSword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsSword.Checked = !chkIsSword.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSubcode.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsSubcode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsSubcode.Checked = !chkIsSubcode.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsLatentTuberculosis.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsCovid_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsCovid.Checked = !chkIsCovid.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtAttachIcd_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != (char)Keys.Enter && e.KeyChar != (char)Keys.F1)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIsSword_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsSword.Checked)
                {
                    this.lciAttachIcd.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationMaxLength(txtAttachIcd, 2000, true); ;
                }
                else
                {
                    this.lciAttachIcd.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProviderEditorInfo.SetValidationRule(txtAttachIcd, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAgeType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAgeType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAgeType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboAgeType.EditValue != null)
                {
                    cboAgeType.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboAgeType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGender_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGender.EditValue != null)
                {
                    cboGender.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboGender.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGender_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lkIcdGroupId_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                List<object> listArgs = new List<object>();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisIcdGroup", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
        }
    }
}
