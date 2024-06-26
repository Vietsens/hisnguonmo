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
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using HIS.Desktop.Plugins.HisBloodVolume;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisBloodVolume
{
    public partial class frmBloodVolume : HIS.Desktop.Utility.FormBase
    {
        public frmBloodVolume(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
            SetCaptionByLanguageKey();
        }

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME currentData;
        MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME resultData;
        List<HIS_BLOOD_VOLUME> listBloodVolume;
        int positionHandle = -1;
        int ActionType = -1;
        int rowCount;
        int dataTotal;
        DelegateSelectData delegateSelect = null;
        internal long id;
        int startPage;
        int limit;
        #endregion

        #region private first
        private void FillDataToGridControl()
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
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, numPageSize, gridControl1);

            WaitingManager.Hide();
        }

        private void SaveProcess()
        {
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;
            positionHandle = -1;
            if (!dxValidationProvider.Validate())
                return;

            CommonParam param = new CommonParam();
            bool success = false;
            MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME updateDTO = new MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME();

            WaitingManager.Show();
            if (this.currentData != null && this.currentData.ID > 0)
            {
                updateDTO = currentData;
            }
            if (chkIS.Checked)
            {
                updateDTO.IS_DONATION = 1;
            }
            else
            {
                updateDTO.IS_DONATION = null;
            }
            updateDTO.VOLUME = spinMaxBhytServiceReqPerDay.Value;

            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if (check(updateDTO))
                {
                    resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>
                      (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_CREATE, ApiConsumers.MosConsumer,
                      updateDTO, param);
                }
               
            }
            else if (updateDTO != null)
            {
                 if (check(updateDTO))
                {
                resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>
                  (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_UPDATE, ApiConsumers.MosConsumer,
                  updateDTO, param);
                }
                 
                //if (resultData != null)
                //{
                //  success = true;
                //  FillDataToGridControl();
                //}
            }
            if (resultData != null)
            {
                success = true;
                FillDataToGridControl();
                ResetFormData();
                LoadBloodVolume();
            }
          
            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            spinMaxBhytServiceReqPerDay.Focus();
            spinMaxBhytServiceReqPerDay.SelectAll();
            SessionManager.ProcessTokenLost(param);
        }

        private bool check(HIS_BLOOD_VOLUME updateDTO)
        {
            bool result = true;
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisBloodVolumeFilter filterSearch = new HisBloodVolumeFilter();
                //filterSearch.KEY_WORD = updateDTO.VOLUME.ToString();// vi mos filter khong the chuyen decimal.ToString nen khong the loc theo key word duoc

                var bloodVolume = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>>
                  (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
                if (bloodVolume != null&&bloodVolume.Count>0)
                {
                    if (bloodVolume.Exists(o => o.VOLUME == updateDTO.VOLUME && o.ID != updateDTO.ID))
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Dữ liệu đã tồn tại", "Thông báo");
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ChangedDataRow()
        {
            currentData = new MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME();
            currentData = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetFocusedRow();

            if (currentData != null)
            {
                this.spinMaxBhytServiceReqPerDay.EditValue = currentData.VOLUME;
                if (currentData.IS_DONATION == 1)
                {
                    chkIS.Checked = true;
                }
                else
                {
                    chkIS.Checked = false;
                }
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
            }
            spinMaxBhytServiceReqPerDay.Focus();
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            spinMaxBhytServiceReqPerDay.Focus();
            spinMaxBhytServiceReqPerDay.SelectAll();
            txtSearch.Text = "";
            LoadBloodVolume();
        }

        private void LoadBloodVolume()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodVolumeFilter filter = new HisBloodVolumeFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtSearch.Text;

                this.listBloodVolume = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BLOOD_VOLUME>>
                  (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_GET, ApiConsumers.MosConsumer, filter, param);
                gridControl1.BeginUpdate();
                gridControl1.DataSource = this.listBloodVolume;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                        chkIS.Checked = false;
                          btnEdit.Enabled = false;
                        btnAdd.Enabled = true;
                        spinMaxBhytServiceReqPerDay.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void ValidateForm()
        {
            ValidationGreatThanZeroOrLessThanThoundControl(this.spinMaxBhytServiceReqPerDay);
            WaitingManager.Hide();
        }

   
        private void SetCaptionByLanguageKey()
        {
            try
            {

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBloodVolume.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBloodVolume.frmBloodVolume).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControl1.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.lcEditInfo.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodVolume.cboRank.Properties.NullText", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.bar2.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSearch.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.barSearch.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barEdit.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.barEdit.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barAdd.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.barAdd.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barRefresh.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.barRefresh.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cbbLoginName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodVolume.cbbLoginName.Properties.NullText", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.btnRefresh.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.btnAdd.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.btnEdit.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.checkDoctor.Properties.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.checkDoctor.Properties.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.checkAdmin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.checkAdmin.Properties.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControlItem6.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControlItem7.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControlItem8.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControlItem5.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControlItem15.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.layoutControl2.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.btnImport.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn4.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn1.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn2.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn3.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn5.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn6.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn7.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn8.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn9.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn10.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn11.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.gridColumn12.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.btnSearch.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barImport.Caption = Inventec.Common.Resource.Get.Value("frmBloodVolume.barImport.Caption", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmBloodVolume.Text", HIS.Desktop.Plugins.BloodVolume.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void SetIcon()
        {
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
        #endregion

        #region private second
        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            MOS.Filter.HisBloodVolumeFilter filterSearch = new HisBloodVolumeFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();
            filterSearch.ORDER_FIELD = "MODIFY_TIME";
            filterSearch.ORDER_DIRECTION = "DESC";

            Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>>
              (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            ControlEditValidationRule validRule = new ControlEditValidationRule();
            validRule.editor = control;
            validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }

        private void ValidationGreatThanZeroOrLessThanThoundControl(SpinEdit control)
        {
            ControlGreatThanZeroOrLessThanThoundValidationRule validRule = new ControlGreatThanZeroOrLessThanThoundValidationRule();
            validRule.spin = control;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }

        #endregion

        #region event
        private void frmBloodVolume_Load(object sender, EventArgs e)
        {
            FillDataToGridControl();
            SetDefaultValue();
            this.ActionType = GlobalVariables.ActionAdd;
            EnableControlChanged(this.ActionType);
            ValidateForm();
        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedDataRow();
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
                if (e.RowHandle >= 0)
                {
                    MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME data = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnlock : btnLock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnUndelete;
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
                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME data = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Hoạt động" : "Tạm khóa";
                if (e.Column.FieldName == "JOB")
                {
                  
                }
                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME data = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME resultLock = new MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME();
                bool notHandler = false;

                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME currentLock = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultLock.ID = currentLock.ID;
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>
                      (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_CHANGELOCK, ApiConsumers.MosConsumer, currentLock, param);

                    if (resultLock!=null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME resultUnlock = new MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME();
                bool notHandler = false;

                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME currentUnlock = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultUnlock.ID = currentUnlock.ID;
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>(HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_CHANGELOCK, ApiConsumers.MosConsumer,
                      currentUnlock, param);

                    if (resultUnlock!=null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME currentDelete = (MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                      (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (HIS.Desktop.Plugins.HisBloodVolume.HisRequestUriStore.HIS_BLOOD_VOLUME_DELETE, ApiConsumers.MosConsumer, currentDelete.ID, param);
                        if (success)
                            FillDataToGridControl();
                        MessageManager.Show(this, param, success);
                    }
                }
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
                FillDataToGridControl();
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = new HIS_BLOOD_VOLUME();
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                ResetFormData();
                LoadBloodVolume();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> obj = new List<object>();
                CallModule callModule = new CallModule(CallModule.HisImportBloodVolume, 0, 0, obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

     
        private void spinMaxBhytServiceReqPerDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                   chkIS.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void barImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnImport_Click(null,null);
        }

        private void chkIS_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
