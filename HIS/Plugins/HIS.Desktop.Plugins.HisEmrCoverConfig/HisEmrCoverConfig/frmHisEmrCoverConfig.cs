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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisEmrCoverConfig.HisEmrCoverConfig
{
    public partial class frmHisEmrCoverConfig : HIS.Desktop.Utility.FormBase
    {
        #region declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int positionHandle = -1;
        int ActionType = -1;
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG currentData;
        #endregion

        #region contructor
        public frmHisEmrCoverConfig(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
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

        private void frmHisEmrCoverConfig_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboDepartment();
                InitComboExecuteRoom();
                InitComboTreatmentType();
                InitComboEmrCoverType();
                SetCaptionByLanguageKey();
                SetDefaultValue();
                FillDataToGridControl();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateForm()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboTreatmentType, txtTreatmentType);
                ValidateGridLookupWithTextEdit(cboEmrCoverType, txtEmrCoverType);
                
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
                dxValidationProvider.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
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

                LoadPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPage, param, numPageSize, this.gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        #region private method

        public void LoadPage(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG>> apiResult = null;
                HisEmrCoverConfigFilter filter = new HisEmrCoverConfigFilter();
                SetFilterNavBar(ref filter);
                //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridControl.DataSource = null;
                gridViewHisEmrCoverConfig.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG>>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG>)apiResult.Data;
                     if (data != null)
                     {
                         gridControl.DataSource = data;
                         gridViewHisEmrCoverConfig.GridControl.DataSource = data;
                         rowCount = (data == null ? 0 : data.Count);
                         dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                     }
                }
                gridViewHisEmrCoverConfig.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisEmrCoverConfig.Resources.Lang", typeof(HIS.Desktop.Plugins.HisEmrCoverConfig.HisEmrCoverConfig.frmHisEmrCoverConfig).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEmrCoverType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.cboEmrCoverType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.cboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txt2.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.txt2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                //this.Text = Inventec.Common.Resource.Get.Value("frmHisEmrCoverConfig.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteRoom()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                var data = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExecuteRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTreatmentType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                var data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTreatmentType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboEmrCoverType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                var data = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                columnInfos.Add(new ColumnInfo("EMR_COVER_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EMR_COVER_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMR_COVER_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboEmrCoverType, data, controlEditorADO);
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

        private void SetDefaultValue()
        {
            try
            {

                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
                //dxValidationProviderEditorInfo.RemoveControlError(cboRoom);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            txtDepartment.Focus();
                            txtExecuteRoom.ReadOnly = false;
                            cboExecuteRoom.ReadOnly = false;
                            txtDepartment.ReadOnly = false;
                            cboDepartment.ReadOnly = false;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            dxValidationProvider.RemoveControlError(txtDepartment);
                            dxValidationProvider.RemoveControlError(txtExecuteRoom);
                            dxValidationProvider.RemoveControlError(txtTreatmentType);
                            dxValidationProvider.RemoveControlError(txtEmrCoverType);
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

        #endregion

        #region event click

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
            try
            {
                if (cboDepartment.EditValue == null && cboExecuteRoom.EditValue == null)
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.
                   Show(Resources.ResourceMessage.NhapMotTrongHai, Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        return;
                }
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;
                positionHandle = -1;
                if (!dxValidationProvider.Validate())
                    return;
                CommonParam param = new CommonParam();
                bool success = false;
                V_HIS_EMR_COVER_CONFIG updateDTO = new V_HIS_EMR_COVER_CONFIG();

                WaitingManager.Show();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                //if (cboRoom.EditValue != null) updateDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? "0").ToString());
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));


                if (ActionType == GlobalVariables.ActionAdd)
                {

                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMR_COVER_CONFIG>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }

                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO.ROOM_ID), updateDTO.ROOM_ID));
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMR_COVER_CONFIG>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                        //ResetFormData();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UpdateDTOFromDataForm(ref V_HIS_EMR_COVER_CONFIG updateDTO)
        {
            try
            {
                if (cboDepartment.EditValue != null && cboDepartment.EditValue.ToString() != "")
                {
                    updateDTO.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue).ToString());
                }
                else { updateDTO.DEPARTMENT_ID = null; }

                if (cboExecuteRoom.EditValue != null && cboExecuteRoom.EditValue.ToString() != "")
                {
                    updateDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue).ToString());
                }
                else { updateDTO.ROOM_ID = null; }

                if (cboTreatmentType.EditValue != null && cboTreatmentType.EditValue.ToString() != "")
                {
                    updateDTO.TREATMENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue).ToString());
                }
                else { updateDTO.TREATMENT_TYPE_ID = 0; }

                if (cboEmrCoverType.EditValue != null && cboEmrCoverType.EditValue.ToString() != "")
                {
                    updateDTO.EMR_COVER_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboEmrCoverType.EditValue).ToString());
                }
                else { updateDTO.EMR_COVER_TYPE_ID = 0; }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmrCoverConfigFilter filter = new HisEmrCoverConfigFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG>>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }
        #endregion

        #region event
        private void gridViewHisEmrCoverConfig_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG pData = (MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME) ?? "";
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "DEPARTMENT_NAME_STR")
                    {
                        try
                        {
                            if (pData.DEPARTMENT_ID != null)
                            {
                                var departmentName = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == pData.DEPARTMENT_ID).FirstOrDefault().DEPARTMENT_NAME;
                                e.Value = departmentName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "ROOM_NAME_STR")
                    {
                        try
                        {
                            if (pData.ROOM_ID != null)
                            {
                                var roomName = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == pData.ROOM_ID).FirstOrDefault().ROOM_NAME;
                                e.Value = roomName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "TREATMENT_TYPE_NAME_STR")
                    {
                        try
                        {
                            if (pData.TREATMENT_TYPE_ID != null)
                            {
                                var treatmentName = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.ID == pData.TREATMENT_TYPE_ID).FirstOrDefault().TREATMENT_TYPE_NAME;
                                e.Value = treatmentName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EMR_COVER_TYPE_NAME_STR")
                    {
                        try
                        {
                            if (pData.EMR_COVER_TYPE_ID != null)
                            {
                                var emrTypeName = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.ID == pData.EMR_COVER_TYPE_ID).FirstOrDefault().EMR_COVER_TYPE_NAME;
                                e.Value = emrTypeName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHisEmrCoverConfig_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_EMR_COVER_CONFIG data = (V_HIS_EMR_COVER_CONFIG)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnLock : btnLock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDisableDelete;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null && cboDepartment.EditValue.ToString() != "")
                    {
                        var dept = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == (long)cboDepartment.EditValue);
                        if (dept != null)
                        {
                            txtDepartment.Text = dept.DEPARTMENT_CODE;
                            txtExecuteRoom.ReadOnly = true;
                            cboExecuteRoom.ReadOnly = true;
                            txtExecuteRoom.Text = "";
                            cboExecuteRoom.EditValue = null;
                            txtTreatmentType.Focus();
                            txtTreatmentType.SelectAll();
                        }
                        else
                        {
                            cboDepartment.Focus();
                            //cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        txtExecuteRoom.Focus();
                        //cboRoom.ShowPopup();
                        txtExecuteRoom.ReadOnly = false;
                        cboExecuteRoom.ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue != null && cboExecuteRoom.EditValue.ToString() != "")
                    {
                        var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == (long)cboExecuteRoom.EditValue);
                        if (room != null)
                        {
                            txtExecuteRoom.Text = room.ROOM_CODE;
                            txtDepartment.ReadOnly = true;
                            cboDepartment.ReadOnly = true;
                            txtDepartment.Text = "";
                            cboDepartment.EditValue = null;
                            txtTreatmentType.Focus();
                            txtTreatmentType.SelectAll();
                        }
                        else
                        {
                            cboExecuteRoom.Focus();
                            //cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        txtTreatmentType.Focus();
                        txtDepartment.ReadOnly = false;
                        cboDepartment.ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null && cboTreatmentType.EditValue.ToString() != "")
                    {
                        var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == (long)cboTreatmentType.EditValue);
                        if (treatmentType != null)
                        {
                            txtTreatmentType.Text = treatmentType.TREATMENT_TYPE_CODE;
                            txtEmrCoverType.Focus();
                            txtEmrCoverType.SelectAll();
                        }
                        else
                        {
                            cboTreatmentType.Focus();
                            //cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        txtEmrCoverType.Focus();
                        //cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboEmrCoverType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEmrCoverType.EditValue != null && cboEmrCoverType.EditValue.ToString() != "")
                    {
                        var emrType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.ID == (long)cboEmrCoverType.EditValue);
                        if (emrType != null)
                        {
                            txtEmrCoverType.Text = emrType.EMR_COVER_TYPE_CODE;
                            if (ActionType == GlobalVariables.ActionAdd)
                            {
                                btnAdd.Focus();
                            }
                            if (ActionType == GlobalVariables.ActionEdit)
                            {
                                btnEdit.Enabled = true;
                                btnEdit.Focus();
                            }
                        }
                        else
                        {
                            cboEmrCoverType.Focus();
                            //cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        cboEmrCoverType.Focus();
                        //cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtDepartment.Text))
                    {
                        var dept = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE.ToUpper() == txtDepartment.Text.ToUpper());
                        if (dept != null)
                        {
                            cboDepartment.EditValue = dept.ID;
                            txtDepartment.Text = dept.DEPARTMENT_CODE;

                            txtExecuteRoom.Focus();
                            txtExecuteRoom.SelectAll();
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtExecuteRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtExecuteRoom.Text))
                    {
                        var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ROOM_CODE.ToUpper() == txtExecuteRoom.Text.ToUpper());
                        if (room != null)
                        {
                            cboExecuteRoom.EditValue = room.ID;
                            txtExecuteRoom.Text = room.ROOM_CODE;

                            txtTreatmentType.Focus();
                            txtTreatmentType.SelectAll();
                        }
                        else
                        {
                            cboExecuteRoom.Focus();
                            cboExecuteRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtTreatmentType.Text))
                    {
                        var treatment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE.ToUpper() == txtTreatmentType.Text.ToUpper());
                        if (treatment != null)
                        {
                            cboTreatmentType.EditValue = treatment.ID;
                            txtTreatmentType.Text = treatment.TREATMENT_TYPE_CODE;

                            txtEmrCoverType.Focus();
                            txtEmrCoverType.SelectAll();
                        }
                        else
                        {
                            cboTreatmentType.Focus();
                            cboTreatmentType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboTreatmentType.Focus();
                        cboTreatmentType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtEmrCoverType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtEmrCoverType.Text))
                    {
                        var treatment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.EMR_COVER_TYPE_CODE.ToUpper() == txtEmrCoverType.Text.ToUpper());
                        if (treatment != null)
                        {
                            cboTreatmentType.EditValue = treatment.ID;
                            txtEmrCoverType.Text = treatment.EMR_COVER_TYPE_CODE;
                            if (ActionType == GlobalVariables.ActionAdd)
                            {
                                btnAdd.Focus();
                            }
                            if (ActionType == GlobalVariables.ActionEdit)
                            {
                                btnEdit.Focus();
                            }
                        }
                        else
                        {
                            cboTreatmentType.Focus();
                            cboTreatmentType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboTreatmentType.Focus();
                        cboTreatmentType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewHisEmrCoverConfig_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DELETE")
                {
                    btnDelete_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EMR_COVER_CONFIG result = new V_HIS_EMR_COVER_CONFIG();
            bool success = false;
            try
            {

                V_HIS_EMR_COVER_CONFIG data = (V_HIS_EMR_COVER_CONFIG)gridViewHisEmrCoverConfig.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_EMR_COVER_CONFIG>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EMR_COVER_CONFIG result = new V_HIS_EMR_COVER_CONFIG();
            bool success = false;
            try
            {
                V_HIS_EMR_COVER_CONFIG data = (V_HIS_EMR_COVER_CONFIG)gridViewHisEmrCoverConfig.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_EMR_COVER_CONFIG>(HisRequestUriStore.HIS_EMR_COVER_CONFIG_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var Data = (V_HIS_EMR_COVER_CONFIG)gridViewHisEmrCoverConfig.GetFocusedRow();
                CommonParam param = new CommonParam();
                try
                {
                    bool success = false;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (Data != null)
                        {
                            WaitingManager.Show();
                            var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<bool>
                                (HisRequestUriStore.HIS_EMR_COVER_CONFIG_DELETE, ApiConsumer.ApiConsumers.MosConsumer, Data.ID, param);
                            if (apiresul)
                            {
                                success = true;
                                FillDataToGridControl();
                            }
                            WaitingManager.Hide();
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG data)
        {
            try
            {
                
                if (data != null)
                {
                    txtExecuteRoom.ReadOnly = false;
                    cboExecuteRoom.ReadOnly = false;
                    txtDepartment.ReadOnly = false;
                    cboDepartment.ReadOnly = false;
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    this.currentData = data;
                    positionHandle = -1;
                    //Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    //Inventec.Common.Logging.LogSystem.Warn("log 4");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(V_HIS_EMR_COVER_CONFIG data)
        {
            try
            {
                if (data != null)
                {
                    cboDepartment.EditValue = data.DEPARTMENT_ID;
                    cboExecuteRoom.EditValue = data.ROOM_ID;
                    cboTreatmentType.EditValue = data.TREATMENT_TYPE_ID;
                    cboEmrCoverType.EditValue = data.EMR_COVER_TYPE_ID;
                    var departmentName = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == data.DEPARTMENT_ID).FirstOrDefault();
                    if (departmentName != null)
                    {
                        txtDepartment.Text = departmentName.DEPARTMENT_CODE;
                    }
                    else 
                    {
                        txtDepartment.Text = "";
                    }

                    var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == data.ROOM_ID).FirstOrDefault();
                    if (executeRoom != null)
                    {
                        txtExecuteRoom.Text = executeRoom.ROOM_CODE;
                    }
                    else
                    {
                        txtExecuteRoom.Text = "";
                    }
                    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.ID == data.TREATMENT_TYPE_ID).FirstOrDefault();
                    if (treatmentType != null)
                    {
                       txtTreatmentType.Text = treatmentType.TREATMENT_TYPE_CODE;
                    }
                    else
                    {
                        txtTreatmentType.Text = "";
                    }
                    var emrCoverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.ID == data.EMR_COVER_TYPE_ID).FirstOrDefault();
                    if (emrCoverType != null)
                    {
                        txtEmrCoverType.Text =  emrCoverType.EMR_COVER_TYPE_CODE;
                    }
                    else
                    {
                        txtEmrCoverType.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHisEmrCoverConfig_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_EMR_COVER_CONFIG)gridViewHisEmrCoverConfig.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
                Inventec.Common.Logging.LogSystem.Warn("Log 1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilterNavBar(ref HisEmrCoverConfigFilter filter)
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

        private void cboDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                    txtDepartment.Text = "";
                    txtDepartment.Focus();
                    txtExecuteRoom.ReadOnly = false;
                    cboExecuteRoom.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExecuteRoom.EditValue = null;
                    txtExecuteRoom.Text = "";
                    txtExecuteRoom.Focus();
                    txtDepartment.ReadOnly = false;
                    cboDepartment.ReadOnly = false;
                }
                   

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentType.EditValue = null;
                    txtTreatmentType.Text = "";
                    txtTreatmentType.Focus();
                }
                    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrCoverType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEmrCoverType.EditValue = null;
                    txtEmrCoverType.Text = "";
                    txtEmrCoverType.Focus();
                }
                    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




    }
}
