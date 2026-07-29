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
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ServiceDefaultPaty.frmServiceDefaultPaty
{
    /// <summary>
    /// PT-44730. Declares the default patient type of a service by patient type and by
    /// additional (co-payment) patient type. Layout follows the screen
    /// "Thiết lập đối tượng thanh toán cho dịch vụ đi kèm" (module 8202).
    /// </summary>
    public partial class frmServiceDefaultPaty : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        List<V_HIS_SERVICE> listService;
        List<HIS_PATIENT_TYPE> listPatientType;
        List<HIS_PATIENT_TYPE> listPrimaryPatientType;

        V_HIS_SERVICE selectedService;
        ServiceDefaultPatyViewDTO currentData;

        int actionType = -1;
        #endregion

        public frmServiceDefaultPaty(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            if (this.moduleData != null) this.Text = this.moduleData.text;
            this.SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmServiceDefaultPaty_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCombo();
                SetCaptionByLanguageKey();
                SetDefaultValue();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmServiceDefaultPaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F) btnSearch.PerformClick();
                if (e.Control && e.KeyCode == Keys.S) btnEdit.PerformClick();
                if (e.Control && e.KeyCode == Keys.N) btnSave.PerformClick();
                if (e.Control && e.KeyCode == Keys.R) btnReset.PerformClick();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #region Language
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.ServiceDefaultPaty.Resources.Lang", typeof(frmServiceDefaultPaty).Assembly);

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciServiceCode.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.lciServiceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.lciPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrimaryPatientType.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.lciPrimaryPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDefaultPatientType.Text = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.lciDefaultPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtSearchValue.Properties.NullText = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.txtSearchValue.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullValuePrompt = this.txtSearchValue.Properties.NullText;
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrimaryPatientType.Properties.NullText = this.cboPatientType.Properties.NullText;

                this.gcServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcServiceName.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcPatientType.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcPrimaryPatientType.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcPrimaryPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcDefaultPatientType.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcDefaultPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCreator.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcModifier.Caption = Inventec.Common.Resource.Get.Value("frmServiceDefaultPaty.gcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Init data
        /// <summary>
        /// Services that may be assigned — medicine, material, blood, ration and package rows are not
        /// assigned through this screen, same exclusion as the reference screen of module 8202.
        /// </summary>
        private void LoadDataToCombo()
        {
            try
            {
                listService = BackendDataWorker.Get<V_HIS_SERVICE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                    .OrderBy(o => o.SERVICE_CODE)
                    .ToList();

                List<ColumnInfo> columnServices = new List<ColumnInfo>();
                columnServices.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnServices.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorLoader.Load(cboServiceName, listService, new ControlEditorADO("SERVICE_NAME", "ID", columnServices, false, 350));

                listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_RATION != 1)
                    .OrderBy(o => o.PATIENT_TYPE_CODE)
                    .ToList();

                // Đối tượng phụ thu chỉ gồm các đối tượng được tích "phụ thu"
                listPrimaryPatientType = listPatientType.Where(o => o.IS_ADDITION == 1).ToList();

                List<ColumnInfo> columnPatientTypes = new List<ColumnInfo>();
                columnPatientTypes.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnPatientTypes.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));

                ControlEditorLoader.Load(cboPatientType, listPatientType, new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnPatientTypes, false, 350));
                ControlEditorLoader.Load(cboDefaultPatientType, listPatientType, new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnPatientTypes, false, 350));
                ControlEditorLoader.Load(cboPrimaryPatientType, listPrimaryPatientType, new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnPatientTypes, false, 350));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                dxErrorProvider1.ClearErrors();

                txtServiceCode.Text = "";
                cboServiceName.EditValue = null;
                cboPatientType.EditValue = null;
                cboPrimaryPatientType.EditValue = null;
                cboDefaultPatientType.EditValue = null;

                this.currentData = null;
                this.selectedService = null;

                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                this.actionType = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load grid
        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                    pageSize = ucPaging.pagingGrid.PageSize;
                else
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.grcListConfig);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                ServiceDefaultPatyFilter filter = new ServiceDefaultPatyFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                SetFilter(ref filter);

                grcListConfig.BeginUpdate();
                try
                {
                    var apiResult = new BackendAdapter(paramCommon).GetRO<List<ServiceDefaultPatyViewDTO>>(
                        ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_GET_VIEW,
                        ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null && apiResult.Data != null)
                    {
                        grvListConfig.GridControl.DataSource = apiResult.Data;
                        rowCount = apiResult.Data.Count;
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        grcListConfig.DataSource = null;
                        MessageManager.Show(this, paramCommon, false);
                    }
                }
                finally
                {
                    grcListConfig.EndUpdate();
                }

                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref ServiceDefaultPatyFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearchValue.Text.Trim()))
                    filter.KEY_WORD = txtSearchValue.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillDataToEditControl()
        {
            try
            {
                if (this.currentData == null) return;

                dxErrorProvider1.ClearErrors();
                btnEdit.Enabled = true;
                btnSave.Enabled = false;
                this.actionType = GlobalVariables.ActionEdit;

                cboServiceName.EditValue = this.currentData.SERVICE_ID;
                cboPatientType.EditValue = this.currentData.PATIENT_TYPE_ID;
                cboPrimaryPatientType.EditValue = this.currentData.PRIMARY_PATIENT_TYPE_ID;
                cboDefaultPatientType.EditValue = this.currentData.DEFAULT_PATIENT_TYPE_ID;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region Grid events
        private void grvListConfig_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    ServiceDefaultPatyViewDTO data = (ServiceDefaultPatyViewDTO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "PATIENT_TYPE_NAME_STR")
                    {
                        e.Value = data.PATIENT_TYPE_ID.HasValue ? data.PATIENT_TYPE_NAME : Resources.ResourceMessage.TatCa;
                    }
                    else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_NAME_STR")
                    {
                        e.Value = data.PRIMARY_PATIENT_TYPE_ID.HasValue ? data.PRIMARY_PATIENT_TYPE_NAME : Resources.ResourceMessage.TatCa;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void grvListConfig_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ServiceDefaultPatyViewDTO data = (ServiceDefaultPatyViewDTO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data == null) return;

                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnlock);

                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnEDelete : btnDDelete);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void grvListConfig_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (ServiceDefaultPatyViewDTO)grvListConfig.GetFocusedRow();
                if (rowData != null)
                {
                    this.currentData = rowData;
                    FillDataToEditControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Value changed
        private void cboServiceName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceName.EditValue == null)
                {
                    this.selectedService = null;
                    txtServiceCode.Text = "";
                    return;
                }

                this.selectedService = listService.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceName.EditValue.ToString()));
                txtServiceCode.Text = this.selectedService != null ? this.selectedService.SERVICE_CODE : "";
                if (this.selectedService != null) dxErrorProvider1.SetError(cboServiceName, "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_Validated(object sender, EventArgs e)
        {
            try
            {
                FindServiceByCode();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) FindServiceByCode();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FindServiceByCode()
        {
            try
            {
                if (string.IsNullOrEmpty(txtServiceCode.Text.Trim())) return;

                string code = txtServiceCode.Text.Trim();
                var service = listService.FirstOrDefault(o => o.SERVICE_CODE == code || o.SERVICE_CODE == code.ToUpper());
                if (service != null)
                {
                    cboServiceName.EditValue = service.ID;
                    txtServiceCode.Text = service.SERVICE_CODE;
                    cboPatientType.Focus();
                }
                else
                {
                    txtServiceCode.Text = "";
                    cboServiceName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) cboPrimaryPatientType.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) cboDefaultPatientType.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboDefaultPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (btnEdit.Enabled) btnEdit.Focus();
                    else btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboDefaultPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDefaultPatientType.EditValue != null) dxErrorProvider1.SetError(cboDefaultPatientType, "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Validate
        /// <summary>
        /// Service and default patient type are required; the condition pair must not duplicate
        /// another row. The server checks the same rules again.
        /// </summary>
        private bool IsValidData()
        {
            bool result = true;
            try
            {
                dxErrorProvider1.ClearErrors();

                if (cboServiceName.EditValue == null)
                {
                    dxErrorProvider1.SetError(cboServiceName,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                        ErrorType.Warning);
                    result = false;
                }

                if (cboDefaultPatientType.EditValue == null)
                {
                    dxErrorProvider1.SetError(cboDefaultPatientType,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                        ErrorType.Warning);
                    result = false;
                }

                if (result && IsDuplicatedConfig())
                {
                    dxErrorProvider1.SetError(cboServiceName, Resources.ResourceMessage.CauHinhNayDaTonTai, ErrorType.Warning);
                    result = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Duplicate = same service + same patient type + same additional patient type,
        /// empty conditions compared as empty. The row being edited is skipped.
        /// </summary>
        private bool IsDuplicatedConfig()
        {
            bool result = false;
            try
            {
                var dataSource = grcListConfig.DataSource as List<ServiceDefaultPatyViewDTO>;
                if (dataSource == null || dataSource.Count == 0) return false;

                long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceName.EditValue.ToString());
                long? patientTypeId = cboPatientType.EditValue != null
                    ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()) : null;
                long? primaryPatientTypeId = cboPrimaryPatientType.EditValue != null
                    ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString()) : null;

                long currentId = (this.actionType == GlobalVariables.ActionEdit && this.currentData != null) ? this.currentData.ID : 0;

                result = dataSource.Any(o => o.ID != currentId
                    && o.SERVICE_ID == serviceId
                    && o.PATIENT_TYPE_ID == patientTypeId
                    && o.PRIMARY_PATIENT_TYPE_ID == primaryPatientTypeId);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                this.actionType = GlobalVariables.ActionAdd;
                if (!IsValidData()) return;
                ProcessSave();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled || this.currentData == null) return;
                this.actionType = GlobalVariables.ActionEdit;
                if (!IsValidData()) return;
                ProcessSave();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ProcessSave()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                bool success = false;

                ServiceDefaultPatyDTO updateDTO = new ServiceDefaultPatyDTO();
                UpdateDataDTO(ref updateDTO);

                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));

                ServiceDefaultPatyDTO resultData = null;
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    resultData = new BackendAdapter(param).Post<ServiceDefaultPatyDTO>(
                        ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_CREATE,
                        ApiConsumers.MosConsumer, updateDTO, param);
                }
                else
                {
                    updateDTO.ID = this.currentData.ID;
                    resultData = new BackendAdapter(param).Post<ServiceDefaultPatyDTO>(
                        ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_UPDATE,
                        ApiConsumers.MosConsumer, updateDTO, param);
                }

                success = resultData != null;
                if (success)
                {
                    SetDefaultValue();
                    FillDataToControl();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void UpdateDataDTO(ref ServiceDefaultPatyDTO updateDTO)
        {
            try
            {
                updateDTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceName.EditValue.ToString());
                updateDTO.DEFAULT_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDefaultPatientType.EditValue.ToString());
                updateDTO.PATIENT_TYPE_ID = cboPatientType.EditValue != null
                    ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()) : null;
                updateDTO.PRIMARY_PATIENT_TYPE_ID = cboPrimaryPatientType.EditValue != null
                    ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString()) : null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void txtSearchValue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Lock / Delete
        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ProcessChangeLock(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ProcessChangeLock(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Toggles the active state, then refreshes the grid keeping the current page.</summary>
        private void ProcessChangeLock(HIS.Desktop.LibraryMessage.Message.Enum confirmMessage)
        {
            CommonParam param = new CommonParam();
            try
            {
                var rowData = (ServiceDefaultPatyViewDTO)grvListConfig.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(confirmMessage),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<ServiceDefaultPatyDTO>(
                    ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_CHANGE_LOCK,
                    ApiConsumers.MosConsumer, rowData.ID, param);

                bool success = result != null;
                if (success)
                {
                    SetDefaultValue();
                    ReloadCurrentPage();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnEDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                var rowData = (ServiceDefaultPatyViewDTO)grvListConfig.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                WaitingManager.Show();
                bool success = new BackendAdapter(param).Post<bool>(
                    ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_DELETE,
                    ApiConsumers.MosConsumer, rowData.ID, param);

                if (success)
                {
                    SetDefaultValue();
                    FillDataToControl();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>Refreshes the grid without jumping back to the first page.</summary>
        private void ReloadCurrentPage()
        {
            try
            {
                int pageSize = ucPaging.pagingGrid != null
                    ? ucPaging.pagingGrid.PageSize
                    : ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                LoadPaging(new CommonParam(startPage, pageSize));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                listService = null;
                listPatientType = null;
                listPrimaryPatientType = null;
                selectedService = null;
                currentData = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
