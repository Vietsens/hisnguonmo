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
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.HisPatientType.HisPatientType
{
    public partial class frmHisPatientType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        internal long Id;
        List<HIS_OTHER_PAY_SOURCE> hisOtherPaySource;
        List<HIS_OTHER_PAY_SOURCE> dataListOPS;

        List<HIS_TREATMENT_TYPE> hisTreatmentType;
        List<HIS_TREATMENT_TYPE> dataListTreatmentType;
        List<HIS_PATIENT_TYPE> listPatientType = new List<HIS_PATIENT_TYPE>();
        #endregion

        #region Construct
        public frmHisPatientType(Inventec.Desktop.Common.Modules.Module moduleData)
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
                    this.Icon = Icon.ExtractAssociatedIcon
                        (System.IO.Path.Combine
                        (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
        private void frmHisPatientType_Load(object sender, EventArgs e)
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPatientType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPatientType.HisPatientType.frmHisPatientType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColPatientTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColPatientTypeCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColPatientTypeName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDescription.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDescription.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColDescription.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsCopayment.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColIsCopayment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsCopayment.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColIsCopayment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisPatientType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsCopayment.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsCopayment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.lciPatientTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsCopayment.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsCopayment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsNotUseForPatient.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsNotUseForPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKiemTra.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.ckhKiemTra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAddition.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkAddition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDefaultDisplay.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkDefaultDisplay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAssign.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkAssign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheckFinishCls.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkCheckFinishCls.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPres.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkPres.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAddition.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdColAddition.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsNotServBill.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsNotServBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkForSaleExp.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsForSaleExp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMustBeGuaranteed.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkMustBeGuaranteed.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMustBeGuaranteed.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkMustBeGuaranteed.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRation.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsRation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSerNotAllow.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkSerNotAllow.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                this.chkIsAdditionRequire.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsAdditionRequire.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsNotShowKiosk.Text = Inventec.Common.Resource.Get.Value("frmHisPatientType.chkIsNotShowKiosk.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdIsNotShowKiosd.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdIsNotShowKiosd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdIsNotShowKiosd.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdIsNotShowKiosd.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsNotServiceBill.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdIsNotServiceBill.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsNotServiceBill.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPatientType.grdIsNotServiceBill.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                spinPriority.EditValue = null;
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
                dicOrderTabIndexControl.Add("txtSysStse39t5m941tzg576s0uhca7", 0);
                dicOrderTabIndexControl.Add("txtPatientTypeCode", 1);
                dicOrderTabIndexControl.Add("txtPatientTypeName", 2);
                dicOrderTabIndexControl.Add("txtDescription", 3);
                dicOrderTabIndexControl.Add("chkIsCopayment", 4);
                dicOrderTabIndexControl.Add("chkIsNotUseForPatient", 5);
                dicOrderTabIndexControl.Add("chkIsNotShowKiosk", 6);
                dicOrderTabIndexControl.Add("chkAddition", 7);
                dicOrderTabIndexControl.Add("chkAssign", 8);
                dicOrderTabIndexControl.Add("chkPres", 9);
                dicOrderTabIndexControl.Add("chkDefaultDisplay", 10);
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
                //TODO
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

                int numPageSize;
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>> apiResult = null;
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>)apiResult.Data;
                    listPatientType = apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        InitComboBasePatientType(listPatientType);
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
        private void InitComboBasePatientType(List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfo.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfo, false);
                ControlEditorLoader.Load(cboBasePatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboHisTreatmentType()
        {
            try
            {
                dataListTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                cboTreatmentType.Properties.DataSource = dataListTreatmentType;
                cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cboTreatmentType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboTreatmentType.Properties.PopupFormWidth = 200;
                cboTreatmentType.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboTreatmentType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection grdCheck = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (grdCheck != null)
                {
                    grdCheck.ClearSelection(cboTreatmentType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitComboHisTreatmentTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboTreatmentType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__TreatmentType);
                cboTreatmentType.Properties.Tag = gridCheck;
                cboTreatmentType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }
        private void InitComboHisOtherPaySource()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisOtherPaySourceFilter filter = new HisOtherPaySourceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                dataListOPS = new BackendAdapter(param).Get<List<HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboOtherPaySource.Properties.DataSource = dataListOPS;
                cboOtherPaySource.Properties.DisplayMember = "OTHER_PAY_SOURCE_NAME";
                cboOtherPaySource.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboOtherPaySource.Properties.View.Columns.AddField("OTHER_PAY_SOURCE_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboOtherPaySource.Properties.PopupFormWidth = 200;
                cboOtherPaySource.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboOtherPaySource.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection grdCheck = cboOtherPaySource.Properties.Tag as GridCheckMarksSelection;
                if (grdCheck != null)
                {
                    grdCheck.ClearSelection(cboOtherPaySource.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitComboHisOtherPaySourceCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboOtherPaySource.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__OtherPaySource);
                cboOtherPaySource.Properties.Tag = gridCheck;
                cboOtherPaySource.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboOtherPaySource.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboOtherPaySource.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }
        private void SelectionGrid__TreatmentType(object sender, EventArgs e)
        {
            try
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                hisTreatmentType = new List<HIS_TREATMENT_TYPE>();
                if (gridCheckMark != null)
                {
                    List<HIS_TREATMENT_TYPE> sgSelectedNews = new List<HIS_TREATMENT_TYPE>();
                    foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.hisTreatmentType = new List<HIS_TREATMENT_TYPE>();
                    this.hisTreatmentType.AddRange(sgSelectedNews);
                }
                this.cboTreatmentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SelectionGrid__OtherPaySource(object sender, EventArgs e)
        {
            try
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                hisOtherPaySource = new List<HIS_OTHER_PAY_SOURCE>();
                if (gridCheckMark != null)
                {
                    List<HIS_OTHER_PAY_SOURCE> sgSelectedNews = new List<HIS_OTHER_PAY_SOURCE>();
                    foreach (HIS_OTHER_PAY_SOURCE rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.OTHER_PAY_SOURCE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.hisOtherPaySource = new List<HIS_OTHER_PAY_SOURCE>();
                    this.hisOtherPaySource.AddRange(sgSelectedNews);
                }
                this.cboOtherPaySource.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetFilterNavBar(ref HisPatientTypeFilter filter)
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
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
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE pData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "IS_COPAYMENT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_COPAYMENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_COPAYMENT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_NOT_USE_FOR_PATIENT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_NOT_USE_FOR_PATIENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_NOT_USE_FOR_PATIENT_STR", ex);
                        }
                    }
                    //
                    else if (e.Column.FieldName == "IS_NOT_SERVICE_BILL_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_NOT_SERVICE_BILL == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_NOT_SERVICE_BILL", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_FOR_SALE_EXP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_FOR_SALE_EXP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_FOR_SALE_EXP", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_NOT_FOR_KIOSK_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_NOT_FOR_KIOSK == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_NOT_FOR_KIOSK", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ADDITION _STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ADDITION == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_ADDITION _STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ASSIGN_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_CHECK_FEE_WHEN_ASSIGN == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_CHECK_FEE_WHEN_ASSIGN", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PRES_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_CHECK_FEE_WHEN_PRES == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_CHECK_FEE_WHEN_PRES", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DEFAULT_DISPLAY")
                    {
                        e.Value = pData != null && pData.IS_SHOWING_OUT_STOCK_BY_DEF == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_NOT_CHECK_FEE_WHEN_EXP_PRES_STR")
                    {
                        e.Value = pData != null && pData.IS_NOT_CHECK_FEE_WHEN_EXP_PRES == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_NOT_EDIT_ASSIGN_SERVICE ")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_NOT_EDIT_ASSIGN_SERVICE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot  IS_NOT_EDIT_ASSIGN_SERVICE", ex);
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

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    Id = data.ID;
                    txtPatientTypeCode.Text = data.PATIENT_TYPE_CODE;
                    txtPatientTypeName.Text = data.PATIENT_TYPE_NAME;
                    txtDescription.Text = data.DESCRIPTION;
                    chkIsCopayment.Checked = (data.IS_COPAYMENT == 1 ? true : false);
                    if (data.PRIORITY != null)
                    {
                        spinPriority.EditValue = data.PRIORITY;

                    }
                    else spinPriority.EditValue = null;
                    chkIsNotUseForPatient.Checked = (data.IS_NOT_USE_FOR_PATIENT == 1 ? true : false);
                    chkIsNotShowKiosk.Checked = (data.IS_NOT_FOR_KIOSK == 1 ? true : false);
                    chkAddition.Checked = (data.IS_ADDITION == 1 ? true : false);
                    chkAssign.Checked = (data.IS_CHECK_FEE_WHEN_ASSIGN == 1 ? true : false);
                    chkCheckFinishCls.Checked = (data.IS_CHECK_FINISH_CLS_WHEN_PRES == 1 ? true : false);
                    chkIsNotServBill.Checked = (data.IS_NOT_SERVICE_BILL == 1 ? true : false);
                    chkForSaleExp.Checked = (data.IS_FOR_SALE_EXP == 1 ? true : false);
                    chkPres.Checked = (data.IS_CHECK_FEE_WHEN_PRES == 1 ? true : false);
                    chkIsAdditionRequire.Checked = (data.IS_ADDITION_REQUIRED == 1 ? true : false);
                    chkKiemTra.Checked = (data.IS_NOT_CHECK_FEE_WHEN_EXP_PRES == 1 ? true : false);
                    chkSerNotAllow.Checked = (data.IS_NOT_EDIT_ASSIGN_SERVICE == 1 ? true : false);

                    chkDefaultDisplay.Checked = (data.IS_SHOWING_OUT_STOCK_BY_DEF == 1 ? true : false);
                    chkMustBeGuaranteed.Checked = (data.MUST_BE_GUARANTEED == 1 ? true : false);
                    chkIsRation.Checked = (data.IS_RATION == 1 ? true : false);
                    Inventec.Common.Logging.LogSystem.Warn("Start 1");
                    var list = listPatientType.Where(o => o.ID != data.ID).ToList();
                    InitComboBasePatientType(list);
                    if (data.BASE_PATIENT_TYPE_ID != null)
                    {
                        cboBasePatientType.EditValue = data.BASE_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        cboBasePatientType.EditValue = null;
                    }

                    List<HIS_OTHER_PAY_SOURCE> checkData = new List<HIS_OTHER_PAY_SOURCE>();
                    if (!string.IsNullOrEmpty(data.OTHER_PAY_SOURCE_IDS))
                    {
                        string[] lstIds = data.OTHER_PAY_SOURCE_IDS.Split(',');
                        foreach (var item in lstIds)
                        {
                            var otherS = dataListOPS.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(item));
                            if (otherS != null)
                                checkData.Add(otherS);
                        }
                    }
                    else
                    {
                        cboOtherPaySource.EditValue = null;
                        GridCheckMarksSelection grdCheck = cboOtherPaySource.Properties.Tag as GridCheckMarksSelection;
                        if (grdCheck != null)
                        {
                            grdCheck.ClearSelection(cboOtherPaySource.Properties.View);
                        }
                    }
                    SetValueOther(this.cboOtherPaySource, checkData, dataListOPS);
                    hisOtherPaySource = checkData;
                    cboOtherPaySource.Focus();

                    List<HIS_TREATMENT_TYPE> checkTreatmentTypeData = new List<HIS_TREATMENT_TYPE>();
                    if (!string.IsNullOrEmpty(data.TREATMENT_TYPE_IDS))
                    {
                        string[] lstIds = data.TREATMENT_TYPE_IDS.Split(',');
                        foreach (var item in lstIds)
                        {
                            var otherS = dataListTreatmentType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(item));
                            if (otherS != null)
                                checkTreatmentTypeData.Add(otherS);
                        }
                    }
                    else
                    {
                        cboTreatmentType.EditValue = null;
                        GridCheckMarksSelection grdCheck = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                        if (grdCheck != null)
                        {
                            grdCheck.ClearSelection(cboTreatmentType.Properties.View);
                        }
                    }
                    SetValueTreatmentType(this.cboTreatmentType, checkTreatmentTypeData, dataListTreatmentType);
                    hisTreatmentType = checkTreatmentTypeData;
                    cboTreatmentType.Focus();


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueTreatmentType(GridLookUpEdit gridLookUpEdit, List<HIS_TREATMENT_TYPE> listSelect, List<HIS_TREATMENT_TYPE> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueOther(GridLookUpEdit gridLookUpEdit, List<HIS_OTHER_PAY_SOURCE> listSelect, List<HIS_OTHER_PAY_SOURCE> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
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

                    GridCheckMarksSelection grdCheck = cboOtherPaySource.Properties.Tag as GridCheckMarksSelection;
                    if (grdCheck != null)
                    {
                        grdCheck.ClearSelection(cboOtherPaySource.Properties.View);
                    }

                    GridCheckMarksSelection grdCheckTreatmentType = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    if (grdCheckTreatmentType != null)
                    {
                        grdCheckTreatmentType.ClearSelection(cboTreatmentType.Properties.View);
                    }
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            SetCaptionByLanguageKey();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                FillDataToGridControl();
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_DELETE, ApiConsumers.MosConsumer, rowData, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
                        FillDataToGridControl();
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
                chkCheckFinishCls.Checked = false;
                chkIsRation.Checked = false;
                chkIsAdditionRequire.Checked = false;
                chkSerNotAllow.Checked = false;
                SetFocusEditor();
                InitComboBasePatientType(listPatientType);
                txtPatientTypeCode.Focus();
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
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        btnRefesh_Click(null, null);

                    }
                }
                else
                {
                    if (updateDTO.ID != null)
                    {
                        updateDTO.ID = Id;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;

                            FillDataToGridControl();
                        }
                    }
                }
                cboOtherPaySource.Focus();
                cboTreatmentType.Focus();
                txtKeyword.Focus();
                if (success)
                {
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
                    SetFocusEditor();
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

        private void LookPatientType()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                WaitingManager.Show();
                var updateDTO = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();

                //if (this.currentData != null && this.currentData.ID > 0)
                //{
                //    LoadCurrent(this.currentData.ID, ref updateDTO);
                //}
                if (updateDTO != null)
                {
                    updateDTO.IS_ACTIVE = null;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(HisRequestUriStore.MOSHIS_PATIENT_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }
                if (success)
                {
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
                    SetFocusEditor();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE currentDTO)
        {
            try
            {
                currentDTO.PATIENT_TYPE_CODE = txtPatientTypeCode.Text.Trim();
                currentDTO.PATIENT_TYPE_NAME = txtPatientTypeName.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
                currentDTO.IS_COPAYMENT = (short)(chkIsCopayment.Checked ? 1 : 0);
                if (spinPriority.EditValue != null)
                {
                    currentDTO.PRIORITY = Convert.ToByte(spinPriority.EditValue);
                }
                else
                {
                    currentDTO.PRIORITY = null;
                }
                currentDTO.IS_NOT_USE_FOR_PATIENT = (short)(chkIsNotUseForPatient.Checked ? 1 : 0);
                if (chkIsNotServBill.Checked)
                {
                    currentDTO.IS_NOT_SERVICE_BILL = 1;
                }
                else
                {
                    currentDTO.IS_NOT_SERVICE_BILL = null;
                }
                if (chkForSaleExp.Checked)
                {
                    currentDTO.IS_FOR_SALE_EXP = 1;
                }
                else
                {
                    currentDTO.IS_FOR_SALE_EXP = null;
                }
                if (chkAddition.Checked)
                {
                    currentDTO.IS_ADDITION = 1;
                    currentDTO.IS_NOT_SERVICE_BILL = null;
                }
                else
                {
                    currentDTO.IS_ADDITION = 0;
                }
                if (chkKiemTra.Checked)
                {
                    currentDTO.IS_NOT_CHECK_FEE_WHEN_EXP_PRES = 1;
                }
                else
                {
                    currentDTO.IS_NOT_CHECK_FEE_WHEN_EXP_PRES = null;
                }
                if (chkCheckFinishCls.Checked)
                    currentDTO.IS_CHECK_FINISH_CLS_WHEN_PRES = 1;
                else
                    currentDTO.IS_CHECK_FINISH_CLS_WHEN_PRES = null;
                if (chkIsRation.Checked)
                {
                    currentDTO.IS_RATION = 1;
                }
                else
                {
                    currentDTO.IS_RATION = null;
                }
                if (chkIsAdditionRequire.Checked)
                {
                    currentDTO.IS_ADDITION_REQUIRED = 1;
                }
                else
                {
                    currentDTO.IS_ADDITION_REQUIRED = null;
                }
                if (chkSerNotAllow.Checked)
                {
                    currentDTO.IS_NOT_EDIT_ASSIGN_SERVICE = 1;
                }
                else
                {
                    currentDTO.IS_NOT_EDIT_ASSIGN_SERVICE = null;
                }

                currentDTO.IS_CHECK_FEE_WHEN_ASSIGN = (short)(chkAssign.Checked ? 1 : 0);
                currentDTO.IS_CHECK_FEE_WHEN_PRES = (short)(chkPres.Checked ? 1 : 0);
                currentDTO.IS_NOT_FOR_KIOSK = (short)(chkIsNotShowKiosk.Checked ? 1 : 0);
                if (chkDefaultDisplay.Checked)
                {
                    currentDTO.IS_SHOWING_OUT_STOCK_BY_DEF = 1;
                }
                else
                {
                    currentDTO.IS_SHOWING_OUT_STOCK_BY_DEF = null;
                }
                if (chkMustBeGuaranteed.Checked)
                {
                    currentDTO.MUST_BE_GUARANTEED = 1;
                }
                else
                {
                    currentDTO.MUST_BE_GUARANTEED = null;
                }

                // if(cboOtherPaySource.EditValue !=null)

                if (hisOtherPaySource != null && hisOtherPaySource.Count > 0)
                {
                    currentDTO.OTHER_PAY_SOURCE_IDS = string.Join(",", hisOtherPaySource.Select(o => o.ID));
                }
                else
                {
                    currentDTO.OTHER_PAY_SOURCE_IDS = null;
                }
                if (hisTreatmentType != null && hisTreatmentType.Count > 0)
                {
                    currentDTO.TREATMENT_TYPE_IDS = string.Join(",", hisTreatmentType.Select(o => o.ID));
                }
                else
                {
                    currentDTO.TREATMENT_TYPE_IDS = null;
                }
                if (cboBasePatientType.EditValue != null)
                {
                    currentDTO.BASE_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBasePatientType.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.BASE_PATIENT_TYPE_ID = null;
                }
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
                ValidationControlMaxLength(txtPatientTypeCode, 6, true);
                ValidationControlMaxLength(txtPatientTypeName, 100, true);
                ValidationControlMaxLength(txtDescription, 500, false);

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

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validate);
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

                //Set status cbo
                if (chkAddition.Checked)
                {
                    chkIsNotServBill.Enabled = false;
                }
                InitComboHisOtherPaySource();
                InitComboHisOtherPaySourceCheck();

                InitComboHisTreatmentType();
                InitComboHisTreatmentTypeCheck();
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

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_PATIENT_TYPE data = (HIS_PATIENT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else
                            e.RepositoryItem = btnGEdit_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientTypeName.Focus();
                    txtPatientTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPriority.Focus();
                    spinPriority.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPriority_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsCopayment.Properties.FullFocusRect = true;
                    chkIsCopayment.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void chkIsCopayment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNotUseForPatient.Properties.FullFocusRect = true;
                    chkIsNotUseForPatient.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void chkIsNotUseForPatient_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNotShowKiosk.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                LockUnlockPatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                LockUnlockPatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //CommonParam param = new CommonParam();
            //HIS_PATIENT_TYPE hisDepertments = new HIS_PATIENT_TYPE();
            //bool notHandler = false;
            //try
            //{
            //    HIS_PATIENT_TYPE dataDepartment = (HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
            //    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        HIS_PATIENT_TYPE data1 = new HIS_PATIENT_TYPE();
            //        data1.ID = dataDepartment.ID;
            //        WaitingManager.Show();
            //        hisDepertments = new BackendAdapter(param).Post<HIS_PATIENT_TYPE>("api/HisPatientType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
            //        WaitingManager.Hide();
            //        if (hisDepertments != null) FillDataToGridControl();
            //        btnEdit.Enabled = false;
            //    }
            //    notHandler = true;
            //    BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
            //    MessageManager.Show(this.ParentForm, param, notHandler);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        // khóa, bỏ khóa
        private void LockUnlockPatientType()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var updateDTO = (HIS_PATIENT_TYPE)gridviewFormList.GetFocusedRow();
                var result = new BackendAdapter(param).Post<HIS_PATIENT_TYPE>("api/HisPatientType/ChangeLock", ApiConsumers.MosConsumer, updateDTO, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                    SetFocusEditor();
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAddition_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAssign.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDefaultDisplay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (btnAdd.Enabled)
            //        {
            //            btnAdd.Focus();

            //        }
            //        else
            //        {
            //            btnEdit.Focus();
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKiemTra.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsNotShowKiosk_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAddition.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOtherPaySource_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gr = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gr == null) return;
                foreach (HIS_OTHER_PAY_SOURCE rv in gr.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(rv.OTHER_PAY_SOURCE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBasePatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBasePatientType.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOtherPaySource_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAssign_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPres.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPres_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDefaultDisplay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAddition_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAddition.Checked)
                {
                    chkIsNotServBill.Enabled = false;
                    chkIsNotServBill.Checked = false;
                }
                else
                {
                    chkIsNotServBill.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKiemTra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkForSaleExp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForSaleExp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMustBeGuaranteed.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMustBeGuaranteed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Click(object sender, EventArgs e)
        {

        }

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gr = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gr == null) return;
                foreach (HIS_TREATMENT_TYPE rv in gr.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
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
