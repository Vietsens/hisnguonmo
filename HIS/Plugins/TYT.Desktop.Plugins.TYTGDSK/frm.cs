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
using ACS.Desktop.ApiConsumer;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;
using TYT.Filter;

namespace TYT.Desktop.Plugins.TYTGDSK
{
    public partial class frm : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        TYT_GDSK Gdsk;

        int action = 0;

        public frm()
        {
            InitializeComponent();
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule)
            : base(_currentModule)
        {
            InitializeComponent();
            this.currentModule = _currentModule;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.action = 2;
        }




        private void frm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


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
        #region MeShow
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
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtBranchCode);
                validatemaxl(txtForm, 100);
                validatemaxl(txtContent, 500);
                validatemaxl(txtTarget, 500);
                validatemaxl(txtMedia, 500);
                validatemaxl(txtAmountOfTime, 500);
                validatemaxl(txtExcuteName, 100);
                validatemaxl(txtNote, 100);
                validatemaxl(txtPlace, 100);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void validatemaxl(BaseEdit control, int maxlength)
        {

            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;

                validRule.ErrorText = "Trường dữ liệu vượt quá kí tự cho phép";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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
                //dicOrderTabIndexControl.Add("txtBedCode", 0);
                //dicOrderTabIndexControl.Add("txtBedName", 1);
                //dicOrderTabIndexControl.Add("lkRoomId", 2);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            { ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("TYT.Desktop.Plugins.TYTGDSK.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTGDSK.frm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frm.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frm.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frm.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frm.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frm.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frm.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frm.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclSTT.Caption = Inventec.Common.Resource.Get.Value("frm.grclSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclBranchCode.Caption = Inventec.Common.Resource.Get.Value("frm.grclBranchCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclBranchName.Caption = Inventec.Common.Resource.Get.Value("frm.grclBranchName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclGdskTime.Caption = Inventec.Common.Resource.Get.Value("frm.grclGdskTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclGdskPlace.Caption = Inventec.Common.Resource.Get.Value("frm.grclGdskPlace.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclContent.Caption = Inventec.Common.Resource.Get.Value("frm.grclContent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclForm.Caption = Inventec.Common.Resource.Get.Value("frm.grclForm.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclTarget.Caption = Inventec.Common.Resource.Get.Value("frm.grclTarget.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCount.Caption = Inventec.Common.Resource.Get.Value("frm.grclCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclMedia.Caption = Inventec.Common.Resource.Get.Value("frm.grclMedia.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclAmountOfTime.Caption = Inventec.Common.Resource.Get.Value("frm.grclAmountOfTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclExecuteName.Caption = Inventec.Common.Resource.Get.Value("frm.grclExecuteName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclNote.Caption = Inventec.Common.Resource.Get.Value("frm.grclNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.textefit.Text = Inventec.Common.Resource.Get.Value("frm.textefit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frm.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frm.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                Inventec.Core.ApiResultObject<List<TYT_GDSK>> apiResult = null;
                TytGdskFilter filter = new TytGdskFilter();

                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<TYT_GDSK>>("api/TytGdsk/Get", ApiConsumers.TytConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<TYT_GDSK>)apiResult.Data;
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
                LogSystem.Error(ex);
            }
        }
        private void SetFilterNavBar(ref TytGdskFilter filter)
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboBranch();
                //InitComboBedTypeId();
                //InitComboBedRoomId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBranch()
        {
            var data = BackendDataWorker.Get<HIS_BRANCH>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "BRANCH_CODE", columnInfos, false, 350);
            ControlEditorLoader.Load(cboBranch, data, controlEditorADO);
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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                txtAmountOfTime.Text = "";
                txtBranchCode.Text = "";
                cboBranch.EditValue = null;
                txtContent.Text = "";
                txtCount.Text = "";
                txtExcuteName.Text = "";
                txtForm.Text = "";
                txtMedia.Text = "";
                txtNote.Text = "";
                txtPlace.Text = "";
                txtTarget.Text = "";
                DateGdskTime.EditValue = null;
                ResetFormData();
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void ResetFormData()
        {
            try
            {
                //if (!lcEditorInfo.IsInitialized) return;
                //lcEditorInfo.BeginUpdate();
                //try
                //{
                //    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                //    {
                //        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                //        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                //        {
                //            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                //            txtApplicationCode.Focus();
                //            txtApplicationCode.SelectAll();
                //            fomatFrm.ResetText();
                //            fomatFrm.EditValue = null;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn(ex);
                //}
                //finally
                //{
                //    lcEditorInfo.EndUpdate();
                //}
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
                FillDataToGridControl();
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
                TYT_GDSK updateDTO = new TYT_GDSK();


                if (this.Gdsk != null && this.Gdsk.ID > 0)
                {
                    LoadCurrent(this.Gdsk.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<TYT_GDSK>("api/TytGdsk/Create", ApiConsumers.TytConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    //sdo.HisRoom.ID = currentData.ROOM_ID;
                    var resultData = new BackendAdapter(param).Post<TYT_GDSK>("api/TytGdsk/Update", ApiConsumers.TytConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    btnReset_Click(null, null);
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

        private void UpdateDTOFromDataForm(ref TYT_GDSK currentDTO)
        {
            try
            {
                currentDTO.BRANCH_CODE = txtBranchCode.Text.Trim();
                currentDTO.AMOUNT_OF_TIME = txtAmountOfTime.Text.Trim();
                currentDTO.AUDIENCE_COUNT = Convert.ToInt32(txtCount.Text.Trim());
                currentDTO.CONTENT = txtContent.Text.Trim();
                currentDTO.EXECUTE_NAME = txtExcuteName.Text.Trim();
                currentDTO.FORM = txtForm.Text.Trim();
                currentDTO.GDSK_PLACE = txtPlace.Text.Trim();
                if (DateGdskTime.EditValue != null)
                {
                    currentDTO.GDSK_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateGdskTime.DateTime);
                }
                currentDTO.MEDIA = txtMedia.Text.Trim();
                currentDTO.NOTE = txtNote.Text.Trim();
                currentDTO.TARGET = txtTarget.Text.Trim();
                ////currentDTO.SAMPLE_ROOM_CODE = txtBedCode.Text.Trim();
                //currentDTO.BED_NAME = txtBedName.Text.Trim();
                //currentDTO.BED_CODE = txtBedCode.Text.Trim();

                ////if (lkBedTypeId.EditValue != null) currentDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkBedTypeId.EditValue ?? "0").ToString());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCurrent(long currentId, ref TYT_GDSK currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                TytGdskFilter filter = new TytGdskFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<TYT_GDSK>>("api/TytGdsk/Get", ApiConsumers.TytConsumer, filter, param).FirstOrDefault();
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
                txtBranchCode.Focus();
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBranchCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtBranchCode.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtBranchCode.Text.ToLower().Trim());
                        var data = BackendDataWorker.Get<HIS_BRANCH>().Where(o => Inventec.Common.String.Convert.UnSignVNese(o.BRANCH_CODE.ToLower()).Contains(key)).ToList();

                        List<HIS_BRANCH> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BRANCH_CODE.ToLower() == txtBranchCode.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboBranch.EditValue = result[0].BRANCH_CODE;
                            txtBranchCode.Text = result[0].BRANCH_CODE;
                            DateGdskTime.Focus();
                            DateGdskTime.ShowPopup();
                        }
                        else
                        {
                            cboBranch.EditValue = null;
                            cboBranch.Focus();
                            cboBranch.ShowPopup();
                        }
                    }
                    else
                    {
                        cboBranch.EditValue = null;
                        cboBranch.Focus();
                        cboBranch.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TYT_GDSK pData = (TYT_GDSK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "GDSK_TIME_Str")
                    {
                        string GDSK_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "GDSK_TIME") ?? "").ToString();
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(GDSK_TIME));
                    }
                    else if (e.Column.FieldName == "BRANCH_NAME")
                    {
                        HIS_BRANCH data = BackendDataWorker.Get<HIS_BRANCH>().SingleOrDefault(o => o.BRANCH_CODE == pData.BRANCH_CODE);
                        e.Value = data.BRANCH_NAME;
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    TYT_GDSK data = (TYT_GDSK)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (TYT_GDSK)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    Gdsk = rowData;
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
        private void ChangedDataRow(TYT_GDSK data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.Gdsk.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToEditorControl(TYT_GDSK data)
        {
            try
            {
                if (data != null)
                {
                    txtBranchCode.EditValue = data.BRANCH_CODE;
                    cboBranch.EditValue = data.BRANCH_CODE;
                    txtPlace.Text = data.GDSK_PLACE;
                    txtForm.EditValue = data.FORM;
                    txtContent.EditValue = data.CONTENT;
                    txtTarget.EditValue = data.TARGET;
                    txtCount.EditValue = data.AUDIENCE_COUNT;
                    txtMedia.EditValue = data.MEDIA;
                    txtAmountOfTime.EditValue = data.AMOUNT_OF_TIME;
                    txtExcuteName.EditValue = data.EXECUTE_NAME;
                    txtNote.EditValue = data.NOTE;
                    if (data.GDSK_TIME != null)
                    {
                        DateGdskTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.GDSK_TIME ?? 0) ?? DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DateGdskTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPlace.Focus();
            }
        }

        private void txtPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtForm.Focus();
            }
        }

        private void txtForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtContent.Focus();
            }
        }

        private void txtContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtTarget.Focus();
            }
        }

        private void txtTarget_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCount.Focus();
            }
        }

        private void txtCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtMedia.Focus();
            }
        }

        private void txtMedia_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtAmountOfTime.Focus();
            }
        }

        private void txtAmountOfTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtExcuteName.Focus();
            }
        }

        private void txtExcuteName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtNote.Focus();
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboBranch_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBranch.EditValue != null && cboBranch.EditValue != cboBranch.OldEditValue)
                    {
                        HIS_BRANCH gt = BackendDataWorker.Get<HIS_BRANCH>().SingleOrDefault(o => o.BRANCH_CODE == cboBranch.EditValue.ToString());
                        if (gt != null)
                        {
                            txtBranchCode.Text = gt.BRANCH_CODE;
                            txtPlace.Focus();
                        }
                    }
                    else
                    {
                        txtPlace.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            TYT_GDSK success = new TYT_GDSK();
            bool notHandler = false;
            try
            {
                var rowData = (TYT_GDSK)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    TYT_GDSK data1 = new TYT_GDSK();
                    data1.ID = rowData.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<TYT_GDSK>("api/TytGdsk/ChangeLock", ApiConsumers.TytConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
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

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            TYT_GDSK success = new TYT_GDSK();
            bool notHandler = false;
            try
            {
                var rowData = (TYT_GDSK)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    TYT_GDSK data1 = new TYT_GDSK();
                    data1.ID = rowData.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<TYT_GDSK>("api/TytGdsk/ChangeLock", ApiConsumers.TytConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                }
                MessageManager.Show(this, param, notHandler);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (TYT_GDSK)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/TytGdsk/Delete", ApiConsumers.TytConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            btnReset_Click(null, null);
                            Gdsk = ((List<TYT_GDSK>)gridView1.DataSource).FirstOrDefault();
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

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

    }
}
