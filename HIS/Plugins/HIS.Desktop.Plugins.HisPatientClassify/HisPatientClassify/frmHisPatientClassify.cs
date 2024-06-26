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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.UC.Paging;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.HisPatientClassify.Validtion;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Text.RegularExpressions;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Logging;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisPatientClassify.HisPatientClassify
{
    public partial class frmHisPatientClassify : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        List<HIS_BHYT_WHITELIST> bhytWhiteList { get; set; }
        List<HIS_MILITARY_RANK> militaryRanks { get; set; }

        #endregion

        #region Construct
        public frmHisPatientClassify(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void frmHisPatientClassify_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Show()
        {
            SetDefaultValue();
            //Focus default
            SetDefaultFocus();
            EnableControlChanged(this.ActionType);
            InitComboHisBhytWhiteListCheck();
            InitComboHisBhytWhiteList();

            InitComboMilitaryCheck();
            InitComboMilitary();


            FillDataToControl();

            // kiem tra du lieu nhap vao
            ValidateForm();
            //set ngon ngu
            SetCaptionByLanguagekey();

            InitCboPatientType();
            InitCboOtherPaySource();


            //

            //Set tabindex control
            InitTabIndex();
        }

        private void InitComboMilitaryCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMilitary.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__Military);
                cboMilitary.Properties.Tag = gridCheck;
                cboMilitary.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMilitary.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMilitary.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__Military(object sender, EventArgs e)
        {
            try
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                militaryRanks = new List<HIS_MILITARY_RANK>();
                if (gridCheckMark != null)
                {
                    List<HIS_MILITARY_RANK> sgSelectedNews = new List<HIS_MILITARY_RANK>();
                    foreach (HIS_MILITARY_RANK rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.MILITARY_RANK_CODE.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.militaryRanks = new List<HIS_MILITARY_RANK>();
                    this.militaryRanks.AddRange(sgSelectedNews);
                }
                this.cboMilitary.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboMilitary()
        {
            try
            {
                cboMilitary.Properties.DataSource = BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboMilitary.Properties.DisplayMember = "MILITARY_RANK_NAME";
                cboMilitary.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboMilitary.Properties.View.Columns.AddField("MILITARY_RANK_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboMilitary.Properties.View.Columns.AddField("MILITARY_RANK_NAME");
                col3.VisibleIndex = 1;
                col3.Width = 200;
                col3.Caption = "";
                cboMilitary.Properties.PopupFormWidth = 300;
                cboMilitary.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboMilitary.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection grdCheck = cboMilitary.Properties.Tag as GridCheckMarksSelection;
                if (grdCheck != null)
                {
                    grdCheck.ClearSelection(cboMilitary.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboHisBhytWhiteListCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBhytWhiteList.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__BhytWhiteList);
                cboBhytWhiteList.Properties.Tag = gridCheck;
                cboBhytWhiteList.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboBhytWhiteList.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBhytWhiteList.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__BhytWhiteList(object sender, EventArgs e)
        {
            try
            {
                this.bhytWhiteList = new List<HIS_BHYT_WHITELIST>();
                string sb = "";
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_BHYT_WHITELIST> sgSelectedNews = new List<HIS_BHYT_WHITELIST>();
                    foreach (HIS_BHYT_WHITELIST rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (rv == null) continue;
                        sb += rv.BHYT_WHITELIST_CODE + ",";
                        sgSelectedNews.Add(rv);
                    }
                    this.bhytWhiteList.AddRange(sgSelectedNews);
                }
                this.cboBhytWhiteList.Text = sb;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboHisBhytWhiteList()
        {
            try
            {
                cboBhytWhiteList.Properties.DataSource = BackendDataWorker.Get<HIS_BHYT_WHITELIST>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboBhytWhiteList.Properties.DisplayMember = "BHYT_WHITELIST_CODE";
                cboBhytWhiteList.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboBhytWhiteList.Properties.View.Columns.AddField("BHYT_WHITELIST_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboBhytWhiteList.Properties.PopupFormWidth = 200;
                cboBhytWhiteList.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboBhytWhiteList.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection grdCheck = cboBhytWhiteList.Properties.Tag as GridCheckMarksSelection;
                if (grdCheck != null)
                {
                    grdCheck.ClearSelection(cboBhytWhiteList.Properties.View);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitCboOtherPaySource()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisOtherPaySourceFilter filter = new HisOtherPaySourceFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(new CommonParam()).Get<List<HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", ApiConsumers.MosConsumer, filter, null);
                if (data != null && data.Count() > 0)
                {
                    List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                    columnInfo.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", "", 100, 1));
                    columnInfo.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfo, false);
                    ControlEditorLoader.Load(cboOtherPaySource, data, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null);
                if (data != null && data.Count() > 0)
                {
                    List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                    columnInfo.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                    columnInfo.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfo, false);
                    ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl3);
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

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPatientClassify.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPatientClassify.HisPatientClassify.frmHisPatientClassify).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisPatientClassify.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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

        #region validate
        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(txtPatientClassifyCode, dxValidationProvider1);
                ValidationControlTextEditPatientClassifyCode();
                ValidationControlTextEditPatientClassifyName();
                //ValidationSingleControl(txtPatientClassifyName, dxValidationProvider1);
                ValidationSingleControl(colorDisplayColor, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditPatientClassifyCode()
        {
            try
            {
                ValidMaxlengthtxtPatientClassifyCode validRule = new ValidMaxlengthtxtPatientClassifyCode();
                validRule.txtPatientClassifyCode = txtPatientClassifyCode;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPatientClassifyCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditPatientClassifyName()
        {
            try
            {
                ValidMaxlengthtxtPatientClassifyName validRule = new ValidMaxlengthtxtPatientClassifyName();
                validRule.txtPatientClassifyName = txtPatientClassifyName;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPatientClassifyName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion



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
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>> apiResult = null;
                HisPatientClassifyFilter filter = new HisPatientClassifyFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>)apiResult.Data;
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

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisPatientClassifyFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.txtSearch.Text = "";
                this.txtPatientClassifyCode.Text = "";
                this.txtPatientClassifyName.Text = "";
                this.colorDisplayColor.Text = "";
                this.cboPatientType.EditValue = null;
                this.cboOtherPaySource.EditValue = null;
                this.chkIsPoli.Checked = false;

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
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY updateDTO = new MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY();


                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;

                        FillDataToControl();
                    }
                }

                if (success)
                {
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

        private void ResetFormData()
        {
            try
            {
                txtPatientClassifyCode.Enabled = true;
                txtPatientClassifyName.Enabled = true;
                cboPatientType.EditValue = null;
                colorDisplayColor.Enabled = true;

                if (!layoutControl3.IsInitialized) return;
                layoutControl3.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl3.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl3.EndUpdate();
                }
                txtPatientClassifyCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY updateDTO)
        {
            try
            {
                updateDTO.PATIENT_CLASSIFY_CODE = txtPatientClassifyCode.Text.Trim();
                updateDTO.PATIENT_CLASSIFY_NAME = txtPatientClassifyName.Text.Trim();
                updateDTO.PATIENT_TYPE_ID = cboPatientType.EditValue != null ? (long?)cboPatientType.EditValue : null;
                updateDTO.IS_POLICE = chkIsPoli.Checked ? (short?)1 : null;

                updateDTO.DISPLAY_COLOR = colorDisplayColor.Text;

                updateDTO.OTHER_PAY_SOURCE_ID = cboOtherPaySource.EditValue != null ? (long?)cboOtherPaySource.EditValue : null;

                if (this.bhytWhiteList != null && this.bhytWhiteList.Count > 0)
                {
                    updateDTO.BHYT_WHITELIST_IDS = string.Join(",", bhytWhiteList.Select(o => o.ID));
                }
                else
                {
                    updateDTO.BHYT_WHITELIST_IDS = null;
                }
                if (this.militaryRanks != null && this.militaryRanks.Count > 0)
                {
                    updateDTO.MILITARY_RANK_IDS = string.Join(",", militaryRanks.Select(o => o.ID));
                }
                else
                {
                    updateDTO.MILITARY_RANK_IDS = null;
                }
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData("UpdateDTOFromDataForm", updateDTO));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientClassifyFilter filter = new HisPatientClassifyFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    txtPatientClassifyCode.Text = currentData.PATIENT_CLASSIFY_CODE;
                    txtPatientClassifyName.Text = currentData.PATIENT_CLASSIFY_NAME;
                    cboPatientType.EditValue = currentData.PATIENT_TYPE_ID;
                    chkIsPoli.Checked = currentData.IS_POLICE == 1 ? true : false;
                    cboOtherPaySource.EditValue = currentData.OTHER_PAY_SOURCE_ID;
                    List<int> parentColor = new List<int>();
                    parentColor = GetColorValues(currentData.DISPLAY_COLOR);

                    colorDisplayColor.Color = Color.FromArgb(parentColor[0], parentColor[1], parentColor[2]);

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ClearComboMulti(GridLookUpEdit grd)
        {
            grd.EditValue = null;
            GridCheckMarksSelection gridCheckMarkBusinessCodes = grd.Properties.Tag as GridCheckMarksSelection;
            gridCheckMarkBusinessCodes.ClearSelection(grd.Properties.View);
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY data)
        {
            try
            {
                if (data != null)
                {
                    txtPatientClassifyCode.Text = data.PATIENT_CLASSIFY_CODE;
                    txtPatientClassifyName.Text = data.PATIENT_CLASSIFY_NAME;
                    cboPatientType.EditValue = data.PATIENT_TYPE_ID;
                    chkIsPoli.Checked = data.IS_POLICE == 1 ? true : false;

                    List<int> parentColor = new List<int>();
                    parentColor = GetColorValues(currentData.DISPLAY_COLOR);

                    colorDisplayColor.Color = Color.FromArgb(parentColor[0], parentColor[1], parentColor[2]);

                    // cboBhytWhiteList
                    GridCheckMarksSelection gridCheckBhytWhiteList = cboBhytWhiteList.Properties.Tag as GridCheckMarksSelection;
                    gridCheckBhytWhiteList.ClearSelection(cboBhytWhiteList.Properties.View);
                    if (!String.IsNullOrWhiteSpace(data.BHYT_WHITELIST_IDS) && cboBhytWhiteList.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.BHYT_WHITELIST_IDS, gridCheckBhytWhiteList);
                    }
                    else
                    {
                        ClearComboMulti(cboBhytWhiteList);
                    }
                    // cbo
                    GridCheckMarksSelection gridCheckMilitaryRank = cboMilitary.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMilitaryRank.ClearSelection(cboMilitary.Properties.View);
                    if (!String.IsNullOrWhiteSpace(data.MILITARY_RANK_IDS) && cboMilitary.Properties.Tag != null)
                    {
                        ProcessSelectBusinessComboMilitary(data.MILITARY_RANK_IDS, gridCheckMilitaryRank);
                    }
                    else
                    {
                        ClearComboMulti(cboMilitary);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusinessComboMilitary(string p, GridCheckMarksSelection gridCheckMilitaryRank)
        {
            try
            {
                List<HIS_MILITARY_RANK> ds = cboMilitary.Properties.DataSource as List<HIS_MILITARY_RANK>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_MILITARY_RANK> selects = new List<HIS_MILITARY_RANK>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMilitaryRank.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckBhytWhiteList)
        {
            try
            {
                List<HIS_BHYT_WHITELIST> ds = cboBhytWhiteList.Properties.DataSource as List<HIS_BHYT_WHITELIST>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_BHYT_WHITELIST> selects = new List<HIS_BHYT_WHITELIST>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckBhytWhiteList.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region event

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                cboOtherPaySource.EditValue = null;
                ClearComboMulti(cboBhytWhiteList);
                ClearComboMulti(cboMilitary);
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                ResetFormData();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
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
                btnReset_Click(null, null);
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
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientClassifyCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientClassifyName.Focus();
                    txtPatientClassifyName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientClassifyName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    colorDisplayColor.Focus();
                    colorDisplayColor.SelectAll();
                    colorDisplayColor.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void colorDisplayColor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientType.Select();
                    cboPatientType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void colorDisplayColor_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();

                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
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

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
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

        private static List<int> GetColorValues(string code)
        {
            List<int> result = new List<int>();
            try
            {
                //string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                //string pattern = ",";
                //Regex myRegex = new Regex(pattern);
                //string[] Codes = myRegex.Split(value);

                string[] Codes = code.Split(',');

                //if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_PATIENT_CLASSIFY data = (HIS_PATIENT_CLASSIFY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGlock : btnGunlock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGdelete : btnEnable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_PATIENT_CLASSIFY pData = (HIS_PATIENT_CLASSIFY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PATIENT_TYPE_NAME")
                    {
                        try
                        {
                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.ID == pData.PATIENT_TYPE_ID);
                            if (patientType != null && patientType.Count() > 0)
                            {
                                e.Value = patientType.FirstOrDefault().PATIENT_TYPE_NAME;
                            }
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

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
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

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                HIS_PATIENT_CLASSIFY data = (HIS_PATIENT_CLASSIFY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
                else if (e.Column.FieldName == "DISPLAY_COLOR_STR")
                {
                    List<int> parentColor = new List<int>();

                    parentColor = GetColorValues(data.DISPLAY_COLOR);

                    e.Appearance.BackColor = Color.FromArgb(parentColor[0], parentColor[1], parentColor[2]);
                }
            }
        }

        private void btnGlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PATIENT_CLASSIFY success = new HIS_PATIENT_CLASSIFY();
            bool notHandler = false;
            try
            {

                HIS_PATIENT_CLASSIFY data = (HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_PATIENT_CLASSIFY data1 = new HIS_PATIENT_CLASSIFY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_PATIENT_CLASSIFY>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
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

        private void btnGunlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PATIENT_CLASSIFY success = new HIS_PATIENT_CLASSIFY();
            bool notHandler = false;
            try
            {
                HIS_PATIENT_CLASSIFY data = (HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_PATIENT_CLASSIFY data1 = new HIS_PATIENT_CLASSIFY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_PATIENT_CLASSIFY>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
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

        private void btnGdelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_PATIENT_CLASSIFY)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_PATIENT_CLASSIFY_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<HIS_PATIENT_CLASSIFY>)gridView1.DataSource).FirstOrDefault();


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


        #endregion

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsPoli.Select();
                    chkIsPoli.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboOtherPaySource_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboOtherPaySource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOtherPaySource_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cboBhytWhiteList_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gr = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gr == null) return;
                foreach (HIS_BHYT_WHITELIST rv in gr.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(rv.BHYT_WHITELIST_CODE.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitary_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gr = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gr == null) return;
                foreach (HIS_MILITARY_RANK rv in gr.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(rv.MILITARY_RANK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBhytWhiteList_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    ClearComboMulti(cboBhytWhiteList);
                    this.bhytWhiteList = new List<HIS_BHYT_WHITELIST>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMilitary_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    ClearComboMulti(cboMilitary);
                    this.militaryRanks = new List<HIS_MILITARY_RANK>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBhytWhiteList_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                e.Handled = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitary_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                e.Handled = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
