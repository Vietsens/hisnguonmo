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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisCurrency.Validtion;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCurrency.HisCurrency
{
    public partial class frmHisCurrency : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_CURRENCY currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;
        string statusActiveText = "Hoạt động";
        string statusLockedText = "Đã khóa";
        string msgDuplicateCode = "Mã ngoại tệ đã tồn tại";
        #endregion

        #region Construct
        public frmHisCurrency(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisCurrency_Load(object sender, EventArgs e)
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
            SetDefaultFocus();
            EnableControlChanged(this.ActionType);
            SetCaptionByLanguageKey();
            SetHintLabels();
            FillDataToControl();
            ValidateForm();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCurrency.Resources.Lang", typeof(frmHisCurrency).Assembly);

                this.gcStt.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCurrencyCode.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcCurrencyCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCurrencyName.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcCurrencyName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcExchangeRate.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcExchangeRate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcExchangeRateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcExchangeRateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcStatus.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.gcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciCurrencyCode.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lciCurrencyCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCurrencyName.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lciCurrencyName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExchangeRate.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lciExchangeRate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExchangeRateTime.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lciExchangeRateTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciModifyTime.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lciModifyTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lblHintCode.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lblHintCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblHintRate.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.lblHintRate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReload.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.btnReload.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisCurrency.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisCurrency.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                string activeText = Inventec.Common.Resource.Get.Value("frmHisCurrency.statusActive.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                string lockedText = Inventec.Common.Resource.Get.Value("frmHisCurrency.statusLocked.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(activeText)) statusActiveText = activeText;
                if (!string.IsNullOrEmpty(lockedText)) statusLockedText = lockedText;

                string dupText = Inventec.Common.Resource.Get.Value("frmHisCurrency.msgDuplicateCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(dupText)) msgDuplicateCode = dupText;

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmHisCurrency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetHintLabels()
        {
            try
            {
                this.lblHintCode.Appearance.ForeColor = Color.Gray;
                this.lblHintRate.Appearance.ForeColor = Color.Gray;
                this.lblModifyTime.Appearance.ForeColor = Color.Gray;
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
                ValidationControlCurrencyCode();
                ValidationControlCurrencyName();
                ValidationControlExchangeRate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogTime.Warn(ex);
            }
        }

        private void ValidationControlCurrencyCode()
        {
            try
            {
                ValidCurrencyCode validRule = new ValidCurrencyCode();
                validRule.txtCurrencyCode = txtCurrencyCode;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtCurrencyCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlCurrencyName()
        {
            try
            {
                ValidCurrencyName validRule = new ValidCurrencyName();
                validRule.txtCurrencyName = txtCurrencyName;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtCurrencyName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlExchangeRate()
        {
            try
            {
                ValidExchangeRate validRule = new ValidExchangeRate();
                validRule.spinExchangeRate = spinExchangeRate;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spinExchangeRate, validRule);
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
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControlCurrency);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_CURRENCY>> apiResult = null;
                HisCurrencyFilter filter = new HisCurrencyFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewCurrency.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_CURRENCY>>(HisRequestUriStore.HIS_CURRENCY_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_CURRENCY>)apiResult.Data;
                    if (data != null)
                    {
                        // Lọc client-side theo mã/tên để tìm kiếm chắc chắn hoạt động (danh mục nhỏ)
                        string keyWord = txtKeyWord.Text.Trim();
                        if (!string.IsNullOrEmpty(keyWord))
                        {
                            data = data.Where(o =>
                                ((o.CURRENCY_CODE ?? "").IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                                || ((o.CURRENCY_NAME ?? "").IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0))
                                .ToList();
                        }

                        gridViewCurrency.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewCurrency.EndUpdate();

                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisCurrencyFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyWord.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogTime.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyWord.Text = "";
                txtCurrencyCode.Text = "";
                txtCurrencyName.Text = "";
                spinExchangeRate.EditValue = null;
                dtExchangeRateTime.EditValue = null;
                lblModifyTime.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

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

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                txtKeyWord.Text = "";
                FillDataToControl();
                SetDefaultFocus();
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
                if (!dxValidationProvider1.Validate())
                    return;

                // Kiểm tra trùng mã ngoại tệ (FE chặn trước khi gọi API)
                long currentId = (this.currentData != null ? this.currentData.ID : 0);
                if (IsDuplicateCode(txtCurrencyCode.Text.Trim(), currentId))
                {
                    XtraMessageBox.Show(msgDuplicateCode,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCurrencyCode.Focus();
                    txtCurrencyCode.SelectAll();
                    return;
                }

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_CURRENCY updateDTO = new MOS.EFMODEL.DataModels.HIS_CURRENCY();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CURRENCY>(HisRequestUriStore.HIS_CURRENCY_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CURRENCY>(HisRequestUriStore.HIS_CURRENCY_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                    }
                }

                WaitingManager.Hide();

                MessageManager.Show(this, param, success);

                SessionManager.ProcessTokenLost(param);
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                currentData = null;
                txtCurrencyCode.Text = "";
                txtCurrencyName.Text = "";
                spinExchangeRate.EditValue = null;
                dtExchangeRateTime.EditValue = null;
                lblModifyTime.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                txtCurrencyCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_CURRENCY updateDTO)
        {
            try
            {
                updateDTO.CURRENCY_CODE = txtCurrencyCode.Text.Trim();
                updateDTO.CURRENCY_NAME = txtCurrencyName.Text.Trim();
                updateDTO.EXCHANGE_RATE = Convert.ToDecimal(spinExchangeRate.EditValue);

                if (dtExchangeRateTime.EditValue != null && !string.IsNullOrWhiteSpace(dtExchangeRateTime.Text))
                {
                    updateDTO.EXCHANGE_RATE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtExchangeRateTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    updateDTO.EXCHANGE_RATE_TIME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_CURRENCY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCurrencyFilter filter = new HisCurrencyFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CURRENCY>>(HisRequestUriStore.HIS_CURRENCY_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra mã ngoại tệ đã tồn tại ở bản ghi khác (chưa xóa) hay chưa.
        /// </summary>
        private bool IsDuplicateCode(string code, long currentId)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) return false;

                CommonParam param = new CommonParam();
                HisCurrencyFilter filter = new HisCurrencyFilter();
                filter.CURRENCY_CODE = code;
                var list = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CURRENCY>>(HisRequestUriStore.HIS_CURRENCY_GET, ApiConsumers.MosConsumer, filter, param);
                if (list != null)
                {
                    return list.Any(o => o.ID != currentId
                        && string.Equals((o.CURRENCY_CODE ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
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
                btnReset_Click(null, null);
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

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlCurrency_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_CURRENCY)gridViewCurrency.GetFocusedRow();
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_CURRENCY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    // Disable nút sửa nếu dữ liệu đã bị khóa
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_CURRENCY data)
        {
            try
            {
                if (data != null)
                {
                    txtCurrencyCode.Text = data.CURRENCY_CODE;
                    txtCurrencyName.Text = data.CURRENCY_NAME;
                    spinExchangeRate.EditValue = data.EXCHANGE_RATE;

                    if (data.EXCHANGE_RATE_TIME != null && data.EXCHANGE_RATE_TIME > 0)
                    {
                        dtExchangeRateTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)data.EXCHANGE_RATE_TIME);
                    }
                    else
                    {
                        dtExchangeRateTime.EditValue = null;
                    }

                    lblModifyTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCurrency_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    MOS.EFMODEL.DataModels.HIS_CURRENCY data = (MOS.EFMODEL.DataModels.HIS_CURRENCY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnLock);
                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewCurrency_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_CURRENCY pData = (MOS.EFMODEL.DataModels.HIS_CURRENCY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "STATUS_STR")
                    {
                        e.Value = pData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? statusActiveText : statusLockedText;
                    }
                    else if (e.Column.FieldName == "EXCHANGE_RATE_TIME_STR")
                    {
                        if (pData.EXCHANGE_RATE_TIME != null && pData.EXCHANGE_RATE_TIME > 0)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.EXCHANGE_RATE_TIME);
                        else
                            e.Value = "";
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCurrency_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                GridView view = sender as GridView;
                MOS.EFMODEL.DataModels.HIS_CURRENCY data = (MOS.EFMODEL.DataModels.HIS_CURRENCY)view.GetRow(e.RowHandle);
                if (data != null && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    e.Appearance.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCurrency_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                if (e.Column != gcStatus) return;

                GridView view = sender as GridView;
                MOS.EFMODEL.DataModels.HIS_CURRENCY data = (MOS.EFMODEL.DataModels.HIS_CURRENCY)view.GetRow(e.RowHandle);
                if (data == null) return;

                // Hoạt động = xanh lá, Đã khóa = đỏ
                e.Appearance.ForeColor = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    ? Color.Green
                    : Color.Red;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_CURRENCY)gridViewCurrency.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockProcess(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong);
        }

        private void btnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockProcess(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong);
        }

        private void ChangeLockProcess(HIS.Desktop.LibraryMessage.Message.Enum confirmMessage)
        {
            CommonParam param = new CommonParam();
            bool notHandler = false;
            try
            {
                MOS.EFMODEL.DataModels.HIS_CURRENCY data = (MOS.EFMODEL.DataModels.HIS_CURRENCY)gridViewCurrency.GetFocusedRow();
                if (data == null) return;

                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(confirmMessage), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var success = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CURRENCY>(HisRequestUriStore.HIS_CURRENCY_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
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

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            CommonParam param = new CommonParam();
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_CURRENCY)gridViewCurrency.GetFocusedRow();
                if (rowData == null) return;

                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_CURRENCY_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (success)
                    {
                        FillDataToControl();
                        currentData = ((List<MOS.EFMODEL.DataModels.HIS_CURRENCY>)gridControlCurrency.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    gridViewCurrency.Focus();
                    gridViewCurrency.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_CURRENCY)gridViewCurrency.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCurrencyCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCurrencyName.Focus();
                    txtCurrencyName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCurrencyName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExchangeRate.Focus();
                    spinExchangeRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
