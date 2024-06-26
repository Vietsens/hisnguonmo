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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.IsAdmin;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LibraryMessage;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.HisSereServPtttTemp
{
    public partial class frmSereServPtttTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        internal int ActionType = 0; // 1: tao moi, 2: update
        int positionHandle = -1;
        string LoggingName = "";
        MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT_TEMP currentSereServPtttTemp;

        List<HIS_SERE_SERV_TEMP> ListDataSource;

        #region khai báo data các combobox
        List<HIS_PTTT_GROUP> lstPtttGroup = new List<HIS_PTTT_GROUP>();
        List<HIS_PTTT_METHOD> lstPtttMethod = new List<HIS_PTTT_METHOD>();
        List<HIS_PTTT_CONDITION> lstPtttCondition = new List<HIS_PTTT_CONDITION>();
        List<HIS_PTTT_CATASTROPHE> lstPtttCatastrophe = new List<HIS_PTTT_CATASTROPHE>();
        List<HIS_PTTT_HIGH_TECH> lstPtttHighTech = new List<HIS_PTTT_HIGH_TECH>();
        List<HIS_PTTT_PRIORITY> lstPtttPriority = new List<HIS_PTTT_PRIORITY>();
        List<HIS_PTTT_TABLE> lstPtttTable = new List<HIS_PTTT_TABLE>();
        List<HIS_EMOTIONLESS_METHOD> lstEmotionlessMethod = new List<HIS_EMOTIONLESS_METHOD>();
        List<HIS_BLOOD_RH> lstBloodRh = new List<HIS_BLOOD_RH>();
        List<HIS_BLOOD_ABO> lstBloodAbo = new List<HIS_BLOOD_ABO>();
        List<HIS_DEATH_WITHIN> lstDeathWithin = new List<HIS_DEATH_WITHIN>();
        List<HIS_EMOTIONLESS_RESULT> lstEmotionlessResult = new List<HIS_EMOTIONLESS_RESULT>();
        #endregion

        const string SpecialCharacters = "\\/:*?\"<>|";

        private object lockObj = new object();
        #endregion

        public frmSereServPtttTemp()
        {
            InitializeComponent();
            LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
        }

        private void frmSereServPtttTemp_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetDefaultControl();
                CreateThreadLoadData();
                FillDataToGrid();
                LoadComboBox();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                validatePtttTempCode(txtSereServPtttTempCode);
                validatePtttTempName(txtSereServPtttTempName);
                validMaxlength(txtManner, 3000);
                validMaxlength(txtConclude, 4000);
                validMaxlength(txtDescription, 4000);
                validMaxlength(txtNote, 3000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void validatePtttTempCode(TextEdit control)
        {
            try
            {
                ValidatePtttTempCode validate = new ValidatePtttTempCode();
                validate.txt = control;
                //validate.IsRequired = true;
                //validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validatePtttTempName(TextEdit control)
        {
            try
            {
                ValidatePtttTempName validate = new ValidatePtttTempName();
                validate.txt = control;
                //validate.IsRequired = true;
                //validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validMaxlength(MemoEdit control, int? maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                //validate.IsRequired = true;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(DevExpress.XtraEditors.TextEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                currentSereServPtttTemp = new HIS_SERE_SERV_PTTT_TEMP();
                positionHandle = -1;
                ActionType = 1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                btnSave.Text = "Lưu (Ctrl S)";

                txtSereServPtttTempCode.Text = "";
                chkIsPublic.Checked = false;
                chkIsPublicInDepartment.Checked = false;
                txtSereServPtttTempName.Text = "";
                cboPtttGroup.EditValue = null;
                cboPtttMethod.EditValue = null;
                cboRealPtttMethod.EditValue = null;
                cboEmotionlessMethod.EditValue = null;
                cboPtttCondition.EditValue = null;
                cboPtttCatastrophe.EditValue = null;
                cboPtttPriority.EditValue = null;
                cboPtttTable.EditValue = null;
                cboEmotionlessMethodSecond.EditValue = null;
                cboBloodAbo.EditValue = null;
                cboBloodRh.EditValue = null;
                cboEmotionlessResult.EditValue = null;
                cboPtttHighTech.EditValue = null;
                cboDeathWithin.EditValue = null;
                txtManner.Text = "";
                txtConclude.Text = "";
                txtDescription.Text = "";
                txtNote.Text = "";
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
                HisSereServPtttTempFilter filter = new HisSereServPtttTempFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                setFilter(ref filter);
                var lstSereServPtttTemp = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_PTTT_TEMP>>("api/HisSereServPtttTemp/Get", ApiConsumers.MosConsumer, filter, null);
                grdSereServPtttTemp.DataSource = null;
                grdSereServPtttTemp.DataSource = lstSereServPtttTemp;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void setFilter(ref HisSereServPtttTempFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboBox()
        {
            try
            {
                FillDataToLookUpEdit(cboPtttGroup, "PTTT_GROUP_NAME", "ID", "PTTT_GROUP_CODE", lstPtttGroup);
                FillDataToLookUpEdit(cboPtttMethod, "PTTT_METHOD_NAME", "ID", "PTTT_METHOD_CODE", lstPtttMethod);
                FillDataToLookUpEdit(cboRealPtttMethod, "PTTT_METHOD_NAME", "ID", "PTTT_METHOD_CODE", lstPtttMethod);
                FillDataToLookUpEdit(cboEmotionlessMethod, "EMOTIONLESS_METHOD_NAME", "ID", "EMOTIONLESS_METHOD_CODE", lstEmotionlessMethod);
                FillDataToLookUpEdit(cboPtttCondition, "PTTT_CONDITION_NAME", "ID", "PTTT_CONDITION_CODE", lstPtttCondition);
                FillDataToLookUpEdit(cboPtttCatastrophe, "PTTT_CATASTROPHE_NAME", "ID", "PTTT_CATASTROPHE_CODE", lstPtttCatastrophe);
                FillDataToLookUpEdit(cboPtttPriority, "PTTT_PRIORITY_NAME", "ID", "PTTT_PRIORITY_CODE", lstPtttPriority);
                FillDataToLookUpEdit(cboPtttTable, "PTTT_TABLE_NAME", "ID", "PTTT_TABLE_CODE", lstPtttTable);

                FillDataToLookUpEdit(cboEmotionlessMethodSecond, "EMOTIONLESS_METHOD_NAME", "ID", "EMOTIONLESS_METHOD_CODE", lstEmotionlessMethod);
                FillDataToLookUpEdit(cboBloodAbo, "BLOOD_ABO_CODE", "ID", "BLOOD_ABO_CODE", lstBloodAbo);
                FillDataToLookUpEdit(cboBloodRh, "BLOOD_RH_CODE", "ID", "BLOOD_RH_CODE", lstBloodRh);
                FillDataToLookUpEdit(cboEmotionlessResult, "EMOTIONLESS_RESULT_NAME", "ID", "EMOTIONLESS_RESULT_CODE", lstEmotionlessResult);
                FillDataToLookUpEdit(cboPtttHighTech, "PTTT_HIGH_TECH_NAME", "ID", "PTTT_HIGH_TECH_CODE", lstPtttHighTech);
                FillDataToLookUpEdit(cboDeathWithin, "DEATH_WITHIN_NAME", "ID", "DEATH_WITHIN_CODE", lstDeathWithin);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            Thread threadPtttGroup = new System.Threading.Thread(LoadDataPtttGroupNewThread);
            Thread threadPtttCondition = new System.Threading.Thread(LoadDataPtttConditionNewThread);
            Thread threadPtttMethod = new System.Threading.Thread(LoadDataPtttMethodNewThread);
            Thread threadPtttCatastrophe = new System.Threading.Thread(LoadDataPtttCatastropheNewThread);
            Thread threadPtttHighTech = new System.Threading.Thread(LoadDataPtttHighTechNewThread);
            Thread threadPtttPriority = new System.Threading.Thread(LoadDataPtttPriorityNewThread);
            Thread threadPtttTable = new System.Threading.Thread(LoadDataPtttTableNewThread);
            Thread threadEmotionlessMethod = new System.Threading.Thread(LoadDataEmotionlessMethodNewThread);
            Thread threadBloodRh = new System.Threading.Thread(LoadDataBloodRhNewThread);
            Thread threadBloodAbo = new System.Threading.Thread(LoadDataBloodAboNewThread);
            Thread threadDeathWithin = new System.Threading.Thread(LoadDataDeathWithinNewThread);
            Thread threadEmotionlessResult = new System.Threading.Thread(LoadDataEmotionlessResultNewThread);
            try
            {
                threadPtttGroup.Start();
                threadPtttCondition.Start();
                threadPtttMethod.Start();
                threadPtttCatastrophe.Start();
                threadPtttHighTech.Start();
                threadPtttPriority.Start();
                threadPtttTable.Start();
                threadEmotionlessMethod.Start();
                threadBloodRh.Start();
                threadBloodAbo.Start();
                threadDeathWithin.Start();
                threadEmotionlessResult.Start();





                threadPtttGroup.Join();
                threadPtttCondition.Join();
                threadPtttMethod.Join();
                threadPtttCatastrophe.Join();
                threadPtttHighTech.Join();
                threadPtttPriority.Join();
                threadPtttTable.Join();
                threadEmotionlessMethod.Join();
                threadBloodRh.Join();
                threadBloodAbo.Join();
                threadDeathWithin.Join();
                threadEmotionlessResult.Join();
            }
            catch (Exception ex)
            {
                threadPtttGroup.Abort();
                threadPtttCondition.Abort();
                threadPtttMethod.Abort();
                threadPtttCatastrophe.Abort();
                threadPtttHighTech.Abort();
                threadPtttPriority.Abort();
                threadPtttTable.Abort();
                threadEmotionlessMethod.Abort();
                threadBloodRh.Abort();
                threadBloodAbo.Abort();
                threadDeathWithin.Abort();
                threadEmotionlessResult.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodAboNewThread()
        {
            try
            {
                HisBloodAboFilter bloodAboFilter = new HisBloodAboFilter();
                bloodAboFilter.IS_ACTIVE = 1;
                lstBloodAbo = new BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_ABO>>("api/HisBloodAbo/Get", ApiConsumers.MosConsumer, bloodAboFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmotionlessResultNewThread()
        {
            try
            {
                HisEmotionlessResultFilter emotionlessResultFilter = new HisEmotionlessResultFilter();
                emotionlessResultFilter.IS_ACTIVE = 1;
                lstEmotionlessResult = new BackendAdapter(new CommonParam()).Get<List<HIS_EMOTIONLESS_RESULT>>("api/HisEmotionlessResult/Get", ApiConsumers.MosConsumer, emotionlessResultFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDeathWithinNewThread()
        {
            try
            {
                HisDeathWithinFilter deathWithinFilter = new HisDeathWithinFilter();
                deathWithinFilter.IS_ACTIVE = 1;
                lstDeathWithin = new BackendAdapter(new CommonParam()).Get<List<HIS_DEATH_WITHIN>>("api/HisDeathWithin/Get", ApiConsumers.MosConsumer, deathWithinFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodRhNewThread()
        {
            try
            {
                HisBloodRhFilter bloodRhFilter = new HisBloodRhFilter();
                bloodRhFilter.IS_ACTIVE = 1;
                lstBloodRh = new BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_RH>>("api/HisBloodRh/Get", ApiConsumers.MosConsumer, bloodRhFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmotionlessMethodNewThread()
        {
            try
            {
                HisEmotionlessMethodFilter emotionlessMethodFilter = new HisEmotionlessMethodFilter();
                emotionlessMethodFilter.IS_ACTIVE = 1;
                lstEmotionlessMethod = new BackendAdapter(new CommonParam()).Get<List<HIS_EMOTIONLESS_METHOD>>("api/HisEmotionlessMethod/Get", ApiConsumers.MosConsumer, emotionlessMethodFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttTableNewThread()
        {
            try
            {
                HisPtttTableFilter ptttTableFilter = new HisPtttTableFilter();
                ptttTableFilter.IS_ACTIVE = 1;
                lstPtttTable = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_TABLE>>("api/HisPtttTable/Get", ApiConsumers.MosConsumer, ptttTableFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttPriorityNewThread()
        {
            try
            {
                HisPtttPriorityFilter ptttPriorityFilter = new HisPtttPriorityFilter();
                ptttPriorityFilter.IS_ACTIVE = 1;
                lstPtttPriority = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_PRIORITY>>("api/HisPtttPriority/Get", ApiConsumers.MosConsumer, ptttPriorityFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttHighTechNewThread()
        {
            try
            {
                HisPtttHighTechFilter ptttHighTechFilter = new HisPtttHighTechFilter();
                ptttHighTechFilter.IS_ACTIVE = 1;
                lstPtttHighTech = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_HIGH_TECH>>("api/HisPtttHighTech/Get", ApiConsumers.MosConsumer, ptttHighTechFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttCatastropheNewThread()
        {
            try
            {
                HisPtttCatastropheFilter ptttCatastropheFilter = new HisPtttCatastropheFilter();
                ptttCatastropheFilter.IS_ACTIVE = 1;
                lstPtttCatastrophe = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_CATASTROPHE>>("api/HisPtttCatastrophe/Get", ApiConsumers.MosConsumer, ptttCatastropheFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttMethodNewThread()
        {
            try
            {
                HisPtttMethodFilter ptttMethodFilter = new HisPtttMethodFilter();
                ptttMethodFilter.IS_ACTIVE = 1;
                lstPtttMethod = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, ptttMethodFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttConditionNewThread()
        {
            try
            {
                HisPtttConditionFilter ptttCondition = new HisPtttConditionFilter();
                ptttCondition.IS_ACTIVE = 1;
                lstPtttCondition = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_CONDITION>>("api/HisPtttCondition/Get", ApiConsumers.MosConsumer, ptttCondition, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttGroupNewThread()
        {
            try
            {
                HisPtttGroupFilter ptttGroupFilter = new HisPtttGroupFilter();
                ptttGroupFilter.IS_ACTIVE = 1;
                lstPtttGroup = new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_GROUP>>("api/HisPtttGroup/Get", ApiConsumers.MosConsumer, ptttGroupFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        void FillDataToLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayCodeMember, "", 150, 1));
                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void grvSereServPtttTemp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "Is_Public_In_Department_chk")
                        {
                            e.Value = data.IS_PUBLIC_IN_DEPARTMENT == 1 ? true : false;
                        }
                        else if (e.Column.FieldName == "Is_Public_chk")
                        {
                            e.Value = data.IS_PUBLIC == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvSereServPtttTemp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_SERE_SERV_PTTT_TEMP data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_SERE_SERV_PTTT_TEMP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    string creator = data.CREATOR;
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) ? repositoryItemBtnDeleteEn : repositoryItemBtnDeleteDis);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteEn_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT_TEMP)grvSereServPtttTemp.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisSereServPtttTemp/Delete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGrid();
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

        private void grdSereServPtttTemp_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentSereServPtttTemp = (HIS_SERE_SERV_PTTT_TEMP)grvSereServPtttTemp.GetFocusedRow();
                if (this.currentSereServPtttTemp != null)
                {

                    ChangedDataRow(this.currentSereServPtttTemp);
                    if (this.currentSereServPtttTemp.CREATOR == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                    {
                        this.ActionType = 2;
                        btnSave.Text = "Sửa (Ctrl U)";
                    }
                    else
                    {
                        this.ActionType = 1;
                        btnSave.Text = "Lưu (Ctrl S)";
                    }
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow(HIS_SERE_SERV_PTTT_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtSereServPtttTempCode.Text = data.SERE_SERV_PTTT_TEMP_CODE;
                    txtSereServPtttTempName.Text = data.SERE_SERV_PTTT_TEMP_NAME;
                    cboPtttGroup.EditValue = data.PTTT_GROUP_ID;
                    cboPtttMethod.EditValue = data.PTTT_METHOD_ID;
                    cboRealPtttMethod.EditValue = data.REAL_PTTT_METHOD_ID;
                    cboEmotionlessMethod.EditValue = data.EMOTIONLESS_METHOD_ID;
                    cboPtttCondition.EditValue = data.PTTT_CONDITION_ID;
                    cboPtttCatastrophe.EditValue = data.PTTT_CATASTROPHE_ID;
                    cboPtttPriority.EditValue = data.PTTT_PRIORITY_ID;
                    cboPtttTable.EditValue = data.PTTT_TABLE_ID;
                    cboEmotionlessMethodSecond.EditValue = data.EMOTIONLESS_METHOD_SECOND_ID;
                    cboBloodAbo.EditValue = data.BLOOD_ABO_ID;
                    cboBloodRh.EditValue = data.BLOOD_RH_ID;
                    cboEmotionlessResult.EditValue = data.EMOTIONLESS_RESULT_ID;
                    cboPtttHighTech.EditValue = data.PTTT_HIGH_TECH_ID;
                    cboDeathWithin.EditValue = data.DEATH_WITHIN_ID;
                    txtManner.Text = data.MANNER;
                    txtConclude.Text = data.CONCLUDE;
                    txtDescription.Text = data.DESCRIPTION;
                    txtNote.Text = data.NOTE;
                    chkIsPublic.Checked = data.IS_PUBLIC == 1;
                    chkIsPublicInDepartment.Checked = data.IS_PUBLIC_IN_DEPARTMENT == 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttGroup.Text))
                    {
                        var data = lstPtttGroup.Where(o => o.PTTT_GROUP_CODE.ToLower().Contains(txtPtttGroup.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttGroup.EditValue = data.FirstOrDefault().ID;
                            txtPtttGroup.Text = data.FirstOrDefault().PTTT_GROUP_CODE;
                        }
                    }
                    else
                    {
                        cboPtttGroup.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttGroup.EditValue != null)
                {
                    txtPtttGroup.Text = lstPtttGroup.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttGroup.EditValue.ToString())).First().PTTT_GROUP_CODE;
                }
                else
                {
                    txtPtttGroup.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttMethod.EditValue != null)
                {
                    txtPtttMethod.Text = lstPtttMethod.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttMethod.EditValue.ToString())).First().PTTT_METHOD_CODE;
                }
                else if (cboPtttMethod.EditValue == null)
                {
                    txtPtttMethod.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRealPtttMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRealPtttMethod.EditValue != null)
                {
                    txtRealPtttMethod.Text = lstPtttMethod.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRealPtttMethod.EditValue.ToString())).First().PTTT_METHOD_CODE;
                }
                else
                {
                    txtRealPtttMethod.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEmotionlessMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEmotionlessMethod.EditValue != null)
                {
                    txtEmotionlessMethod.Text = lstEmotionlessMethod.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessMethod.EditValue.ToString())).First().EMOTIONLESS_METHOD_CODE;
                }
                else
                {
                    txtEmotionlessMethod.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttCondition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttCondition.EditValue != null)
                {
                    txtPtttCondition.Text = lstPtttCondition.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttCondition.EditValue.ToString())).First().PTTT_CONDITION_CODE;
                }
                else
                {
                    txtPtttCondition.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttCatastrophe_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttCatastrophe.EditValue != null)
                {
                    txtPtttCatastrophe.Text = lstPtttCatastrophe.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttCatastrophe.EditValue.ToString())).First().PTTT_CATASTROPHE_CODE;
                }
                else
                {
                    txtPtttCatastrophe.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttPriority_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttPriority.EditValue != null)
                {
                    txtPtttPriority.Text = lstPtttPriority.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttPriority.EditValue.ToString())).First().PTTT_PRIORITY_CODE;
                }
                else
                {
                    txtPtttPriority.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttTable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttTable.EditValue != null)
                {
                    txtPtttTable.Text = lstPtttTable.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttTable.EditValue.ToString())).First().PTTT_TABLE_CODE;
                }
                else
                {
                    txtPtttTable.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEmotionlessMethodSecond_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEmotionlessMethodSecond.EditValue != null)
                {
                    txtEmotionlessMethodSecond.Text = lstEmotionlessMethod.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessMethodSecond.EditValue.ToString())).First().EMOTIONLESS_METHOD_CODE;
                }
                else
                {
                    txtEmotionlessMethodSecond.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodAbo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBloodAbo.EditValue != null)
                {
                    txtBloodAbo.Text = lstBloodAbo.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodAbo.EditValue.ToString())).First().BLOOD_ABO_CODE;
                }
                else
                {
                    txtBloodAbo.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodRh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBloodRh.EditValue != null)
                {
                    txtBloodRh.Text = lstBloodRh.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodRh.EditValue.ToString())).First().BLOOD_RH_CODE;
                }
                else
                {
                    txtBloodRh.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEmotionlessResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEmotionlessResult.EditValue != null)
                {
                    txtEmotionlessResult.Text = lstEmotionlessResult.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessResult.EditValue.ToString())).First().EMOTIONLESS_RESULT_CODE;
                }
                else
                {
                    txtEmotionlessResult.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttHighTech_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttHighTech.EditValue != null)
                {
                    txtPtttHighTech.Text = lstPtttHighTech.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttHighTech.EditValue.ToString())).First().PTTT_HIGH_TECH_CODE;
                }
                else
                {
                    txtPtttHighTech.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathWithin_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDeathWithin.EditValue != null)
                {
                    txtDeathWithin.Text = lstDeathWithin.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathWithin.EditValue.ToString())).First().DEATH_WITHIN_CODE;
                }
                else
                {
                    txtDeathWithin.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttMethod.Text))
                    {
                        var data = lstPtttMethod.Where(o => o.PTTT_METHOD_CODE.ToLower().Contains(txtPtttMethod.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttMethod.EditValue = data.FirstOrDefault().ID;
                            txtPtttMethod.Text = data.FirstOrDefault().PTTT_METHOD_CODE;
                        }
                    }
                    else
                    {
                        cboPtttMethod.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRealPtttMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtRealPtttMethod.Text))
                    {
                        var data = lstPtttMethod.Where(o => o.PTTT_METHOD_CODE.ToLower().Contains(txtRealPtttMethod.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboRealPtttMethod.EditValue = data.FirstOrDefault().ID;
                            txtRealPtttMethod.Text = data.FirstOrDefault().PTTT_METHOD_CODE;
                        }
                    }
                    else
                    {
                        cboRealPtttMethod.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtEmotionlessMethod.Text))
                    {
                        var data = lstEmotionlessMethod.Where(o => o.EMOTIONLESS_METHOD_CODE.ToLower().Contains(txtEmotionlessMethod.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboEmotionlessMethod.EditValue = data.FirstOrDefault().ID;
                            txtEmotionlessMethod.Text = data.FirstOrDefault().EMOTIONLESS_METHOD_CODE;
                        }
                    }
                    else
                    {
                        cboEmotionlessMethod.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttCondition.Text))
                    {
                        var data = lstPtttCondition.Where(o => o.PTTT_CONDITION_CODE.ToLower().Contains(txtPtttCondition.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttCondition.EditValue = data.FirstOrDefault().ID;
                            txtPtttCondition.Text = data.FirstOrDefault().PTTT_CONDITION_CODE;
                        }
                    }
                    else
                    {
                        cboPtttCondition.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttCatastrophe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttCatastrophe.Text))
                    {
                        var data = lstPtttCatastrophe.Where(o => o.PTTT_CATASTROPHE_CODE.ToLower().Contains(txtPtttCatastrophe.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttCatastrophe.EditValue = data.FirstOrDefault().ID;
                            txtPtttCatastrophe.Text = data.FirstOrDefault().PTTT_CATASTROPHE_CODE;
                        }
                    }
                    else
                    {
                        cboPtttCatastrophe.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttPriority_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttPriority.Text))
                    {
                        var data = lstPtttPriority.Where(o => o.PTTT_PRIORITY_CODE.ToLower().Contains(txtPtttPriority.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttPriority.EditValue = data.FirstOrDefault().ID;
                            txtPtttPriority.Text = data.FirstOrDefault().PTTT_PRIORITY_CODE;
                        }
                    }
                    else
                    {
                        cboPtttPriority.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttTable_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttTable.Text))
                    {
                        var data = lstPtttTable.Where(o => o.PTTT_TABLE_CODE.ToLower().Contains(txtPtttTable.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttTable.EditValue = data.FirstOrDefault().ID;
                            txtPtttTable.Text = data.FirstOrDefault().PTTT_TABLE_CODE;
                        }
                    }
                    else
                    {
                        cboPtttTable.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEmotionlessMethodSecond_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtEmotionlessMethodSecond.Text))
                    {
                        var data = lstEmotionlessMethod.Where(o => o.EMOTIONLESS_METHOD_CODE.ToLower().Contains(txtEmotionlessMethodSecond.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboEmotionlessMethodSecond.EditValue = data.FirstOrDefault().ID;
                            txtEmotionlessMethodSecond.Text = data.FirstOrDefault().EMOTIONLESS_METHOD_CODE;
                        }
                    }
                    else
                    {
                        cboEmotionlessMethodSecond.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodAbo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtBloodAbo.Text))
                    {
                        var data = lstBloodAbo.Where(o => o.BLOOD_ABO_CODE.ToLower().Contains(txtBloodAbo.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboBloodAbo.EditValue = data.FirstOrDefault().ID;
                            txtBloodAbo.Text = data.FirstOrDefault().BLOOD_ABO_CODE;
                        }
                    }
                    else
                    {
                        cboBloodAbo.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodRh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtBloodRh.Text))
                    {
                        var data = lstBloodRh.Where(o => o.BLOOD_RH_CODE.ToLower().Contains(txtBloodRh.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboBloodRh.EditValue = data.FirstOrDefault().ID;
                            txtBloodRh.Text = data.FirstOrDefault().BLOOD_RH_CODE;
                        }
                    }
                    else
                    {
                        cboBloodRh.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEmotionlessResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtEmotionlessResult.Text))
                    {
                        var data = lstEmotionlessResult.Where(o => o.EMOTIONLESS_RESULT_CODE.ToLower().Contains(txtEmotionlessResult.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboEmotionlessResult.EditValue = data.FirstOrDefault().ID;
                            txtEmotionlessResult.Text = data.FirstOrDefault().EMOTIONLESS_RESULT_CODE;
                        }
                    }
                    else
                    {
                        cboEmotionlessResult.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttHighTech_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttHighTech.Text))
                    {
                        var data = lstPtttHighTech.Where(o => o.PTTT_HIGH_TECH_CODE.ToLower().Contains(txtPtttHighTech.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboPtttHighTech.EditValue = data.FirstOrDefault().ID;
                            txtPtttHighTech.Text = data.FirstOrDefault().PTTT_HIGH_TECH_CODE;
                        }
                    }
                    else
                    {
                        cboPtttHighTech.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDeathWithin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtDeathWithin.Text))
                    {
                        var data = lstDeathWithin.Where(o => o.DEATH_WITHIN_CODE.ToLower().Contains(txtDeathWithin.Text.ToLower())).ToList();
                        if (data != null && data.Count() > 0)
                        {
                            cboDeathWithin.EditValue = data.FirstOrDefault().ID;
                            txtDeathWithin.Text = data.FirstOrDefault().DEATH_WITHIN_CODE;
                        }
                    }
                    else
                    {
                        cboDeathWithin.EditValue = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                HIS_SERE_SERV_PTTT_TEMP updateDTO = new HIS_SERE_SERV_PTTT_TEMP();
                if (this.currentSereServPtttTemp != null && this.currentSereServPtttTemp.ID > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT_TEMP>(updateDTO, currentSereServPtttTemp);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (this.ActionType == 1)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_SERE_SERV_PTTT_TEMP>("api/HisSereServPtttTemp/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {

                        success = true;
                        FillDataToGrid();
                        txtSereServPtttTempCode.Text = "";
                        txtSereServPtttTempName.Text = "";
                        SetDefaultControl();

                    }
                }
                else
                {
                    if (this.currentSereServPtttTemp.ID > 0)
                    {
                        updateDTO.ID = this.currentSereServPtttTemp.ID;
                    }
                    var resultData = new BackendAdapter(param).Post<HIS_SERE_SERV_PTTT_TEMP>("api/HisSereServPtttTemp/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_SERE_SERV_PTTT_TEMP>();
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

        private void UpdateDTOFromDataForm(ref HIS_SERE_SERV_PTTT_TEMP updateDTO)
        {
            try
            {
                updateDTO.ID = 0;
                updateDTO.SERE_SERV_PTTT_TEMP_CODE = txtSereServPtttTempCode.Text.ToString();
                updateDTO.SERE_SERV_PTTT_TEMP_NAME = txtSereServPtttTempName.Text.ToString();
                if (chkIsPublic.Checked)
                    updateDTO.IS_PUBLIC = 1;
                else
                    updateDTO.IS_PUBLIC = null;
                if (chkIsPublicInDepartment.Checked)
                    updateDTO.IS_PUBLIC_IN_DEPARTMENT = 1;
                else
                    updateDTO.IS_PUBLIC_IN_DEPARTMENT = null;

                if (cboPtttGroup.EditValue != null)
                    updateDTO.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttGroup.EditValue.ToString());
                else
                    updateDTO.PTTT_GROUP_ID = null;

                if (cboPtttMethod.EditValue != null)
                    updateDTO.PTTT_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttMethod.EditValue.ToString());
                else
                    updateDTO.PTTT_METHOD_ID = null;

                if (cboRealPtttMethod.EditValue != null)
                    updateDTO.REAL_PTTT_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRealPtttMethod.EditValue.ToString());
                else
                    updateDTO.REAL_PTTT_METHOD_ID = null;

                if (cboPtttCondition.EditValue != null)
                    updateDTO.PTTT_CONDITION_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttCondition.EditValue.ToString());
                else
                    updateDTO.PTTT_CONDITION_ID = null;
                if (cboPtttCatastrophe.EditValue != null)
                    updateDTO.PTTT_CATASTROPHE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttCatastrophe.EditValue.ToString());
                else
                    updateDTO.PTTT_CATASTROPHE_ID = null;
                if (cboPtttHighTech.EditValue != null)
                    updateDTO.PTTT_HIGH_TECH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttHighTech.EditValue.ToString());
                else
                    updateDTO.PTTT_HIGH_TECH_ID = null;
                if (cboPtttPriority.EditValue != null)
                    updateDTO.PTTT_PRIORITY_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttPriority.EditValue.ToString());
                else
                    updateDTO.PTTT_PRIORITY_ID = null;
                if (cboPtttTable.EditValue != null)
                    updateDTO.PTTT_TABLE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttTable.EditValue.ToString());
                else
                    updateDTO.PTTT_TABLE_ID = null;
                if (cboEmotionlessMethod.EditValue != null)
                    updateDTO.EMOTIONLESS_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessMethod.EditValue.ToString());
                else
                    updateDTO.EMOTIONLESS_METHOD_ID = null;
                if (cboEmotionlessMethodSecond.EditValue != null)
                    updateDTO.EMOTIONLESS_METHOD_SECOND_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessMethodSecond.EditValue.ToString());
                else
                    updateDTO.EMOTIONLESS_METHOD_SECOND_ID = null;
                if (cboBloodAbo.EditValue != null)
                    updateDTO.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodAbo.EditValue.ToString());
                else
                    updateDTO.BLOOD_ABO_ID = null;
                if (cboBloodRh.EditValue != null)
                    updateDTO.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodRh.EditValue.ToString());
                else
                    updateDTO.BLOOD_RH_ID = null;
                if (cboDeathWithin.EditValue != null)
                    updateDTO.DEATH_WITHIN_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathWithin.EditValue.ToString());
                else
                    updateDTO.DEATH_WITHIN_ID = null;
                if (cboEmotionlessResult.EditValue != null)
                    updateDTO.EMOTIONLESS_RESULT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboEmotionlessResult.EditValue.ToString());
                else
                    updateDTO.EMOTIONLESS_RESULT_ID = null;
                updateDTO.MANNER = txtManner.Text;
                updateDTO.CONCLUDE = txtConclude.Text;
                updateDTO.DESCRIPTION = txtDescription.Text;
                updateDTO.NOTE = txtNote.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        //private void txtSereServPtttTempCode_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    try
        //    {
        //        if ((sender as TextBox).SelectionStart == 0)
        //            e.Handled = (e.KeyChar == (char)Keys.Space);
        //        else
        //            e.Handled = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

    }
}
