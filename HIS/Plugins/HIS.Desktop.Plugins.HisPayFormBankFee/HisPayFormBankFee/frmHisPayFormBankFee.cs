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
using HIS.Desktop.Plugins.HisPayFormBankFee.Validtion;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisPayFormBankFee.HisPayFormBankFee
{
    public partial class frmHisPayFormBankFee : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;

        /// <summary>Giá trị ID đại diện cho "Tất cả ngân hàng" (BANK_ID = null trong DB)</summary>
        const long ALL_BANK_ID = 0;

        HIS_PAY_FORM_BANK_FEE currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;

        /// <summary>Tra cứu tên hình thức thanh toán theo ID — build 1 lần, dùng O(1)</summary>
        Dictionary<long, string> payFormNameDict = new Dictionary<long, string>();

        /// <summary>Tra cứu tên ngân hàng theo ID — build 1 lần, dùng O(1)</summary>
        Dictionary<long, string> bankNameDict = new Dictionary<long, string>();

        List<HIS_PAY_FORM> listPayFormCombo;
        List<HIS_BANK> listBankCombo;

        /// <summary>Toan bo cau hinh tai tu server (cache 1 lan) — loc/tim kiem phia client tren danh sach nay</summary>
        List<HIS_PAY_FORM_BANK_FEE> allData;

        /// <summary>Nut xoa DISABLE (icon den) — hien tren dong DA KHOA, khong cho xoa</summary>
        DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGDeleteDisable;

        string statusActiveText = "Hoạt động";
        string statusLockedText = "Tạm khóa";
        string allBankText = "(Tất cả ngân hàng)";
        #endregion

        #region Construct
        public frmHisPayFormBankFee(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                SetIcon();
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load
        private void frmHisPayFormBankFee_Load(object sender, EventArgs e)
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
            InitComboData();
            BuildLookupDictionary();
            InitGridActionIcons();
            SetCaptionByLanguageKey();
            ValidateForm();
            SetDefaultValue();
            EnableControlChanged(this.ActionType);
            FillDataToControl();
            SetDefaultFocus();
        }


        /// <summary>
        /// Set icon nut Xoa = icon DO tu DevExpress Image Gallery (images/edit/delete_16x16.png).
        /// Lock/Unlock dung icon hmenu (o khoa) khai bao trong Designer.
        /// (PNG cua HisFileType trong repo bi hong do git autocrlf nen khong dung lai duoc.)
        /// </summary>
        private void InitGridActionIcons()
        {
            try
            {
                // Nut xoa DO (cho dong dang hoat dong)
                System.Drawing.Image imgDeleteRed = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/delete_16x16.png");
                if (imgDeleteRed != null && btnGDelete.Buttons.Count > 0)
                {
                    btnGDelete.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                    btnGDelete.Buttons[0].Image = imgDeleteRed;
                }

                // Nut xoa DEN (xam) disable cho dong da khoa — khong cho xoa
                btnGDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
                btnGDeleteDisable.AutoHeight = false;
                btnGDeleteDisable.AllowFocused = false;
                btnGDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                System.Drawing.Image imgDeleteGray = DevExpress.Images.ImageResourceCache.Default.GetImage("grayscaleimages/edit/delete_16x16.png");
                btnGDeleteDisable.Buttons.Clear();
                btnGDeleteDisable.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
                {
                    Image = imgDeleteGray
                });
                gridControlFee.RepositoryItems.Add(btnGDeleteDisable);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboData()
        {
            try
            {
                // Hinh thuc thanh toan: chi lay HT yeu cau ngan hang (IS_REQUIRED_BANK = 1) va dang hoat dong
                listPayFormCombo = BackendDataWorker.Get<HIS_PAY_FORM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_REQUIRED_BANK == 1)
                    .OrderBy(o => o.PAY_FORM_NAME)
                    .ToList();

                cboPayForm.Properties.DataSource = listPayFormCombo;
                cboPayForm.Properties.ValueMember = "ID";
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.NullText = "";
                cboPayForm.Properties.Columns.Clear();
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "Mã", 80));
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "Tên hình thức thanh toán", 200));
                cboPayForm.Properties.PopupFormWidth = 320;

                // Ngan hang: them dong "(Tat ca ngan hang)" = BANK_ID null
                listBankCombo = new List<HIS_BANK>();
                listBankCombo.Add(new HIS_BANK() { ID = ALL_BANK_ID, BANK_CODE = "", BANK_NAME = allBankText });
                listBankCombo.AddRange(BackendDataWorker.Get<HIS_BANK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.NUM_ORDER ?? 0)
                    .ThenBy(o => o.BANK_NAME));

                cboBank.Properties.DataSource = listBankCombo;
                cboBank.Properties.ValueMember = "ID";
                cboBank.Properties.DisplayMember = "BANK_NAME";
                cboBank.Properties.NullText = allBankText;
                cboBank.Properties.Columns.Clear();
                cboBank.Properties.Columns.Add(new LookUpColumnInfo("BANK_CODE", "Mã", 80));
                cboBank.Properties.Columns.Add(new LookUpColumnInfo("BANK_NAME", "Tên ngân hàng", 200));
                cboBank.Properties.PopupFormWidth = 320;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BuildLookupDictionary()
        {
            try
            {
                payFormNameDict = BackendDataWorker.Get<HIS_PAY_FORM>()
                    .GroupBy(o => o.ID)
                    .ToDictionary(g => g.Key, g => g.First().PAY_FORM_NAME);

                bankNameDict = BackendDataWorker.Get<HIS_BANK>()
                    .GroupBy(o => o.ID)
                    .ToDictionary(g => g.Key, g => g.First().BANK_NAME);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPayFormBankFee.Resources.Lang", typeof(frmHisPayFormBankFee).Assembly);

                this.gcStt.Caption = GetLang("frmHisPayFormBankFee.gcStt.Caption", this.gcStt.Caption);
                this.gcPayForm.Caption = GetLang("frmHisPayFormBankFee.gcPayForm.Caption", this.gcPayForm.Caption);
                this.gcBank.Caption = GetLang("frmHisPayFormBankFee.gcBank.Caption", this.gcBank.Caption);
                this.gcFeeRate.Caption = GetLang("frmHisPayFormBankFee.gcFeeRate.Caption", this.gcFeeRate.Caption);
                this.gcFeeName.Caption = GetLang("frmHisPayFormBankFee.gcFeeName.Caption", this.gcFeeName.Caption);
                this.gcStatus.Caption = GetLang("frmHisPayFormBankFee.gcStatus.Caption", this.gcStatus.Caption);
                this.gcCreateTime.Caption = GetLang("frmHisPayFormBankFee.gcCreateTime.Caption", this.gcCreateTime.Caption);
                this.gcCreator.Caption = GetLang("frmHisPayFormBankFee.gcCreator.Caption", this.gcCreator.Caption);
                this.gcModifyTime.Caption = GetLang("frmHisPayFormBankFee.gcModifyTime.Caption", this.gcModifyTime.Caption);
                this.gcModifier.Caption = GetLang("frmHisPayFormBankFee.gcModifier.Caption", this.gcModifier.Caption);

                this.lciPayForm.Text = GetLang("frmHisPayFormBankFee.lciPayForm.Text", this.lciPayForm.Text);
                this.lciBank.Text = GetLang("frmHisPayFormBankFee.lciBank.Text", this.lciBank.Text);
                this.lciFeeRate.Text = GetLang("frmHisPayFormBankFee.lciFeeRate.Text", this.lciFeeRate.Text);
                this.lciFeeName.Text = GetLang("frmHisPayFormBankFee.lciFeeName.Text", this.lciFeeName.Text);

                this.txtKeyWord.Properties.NullValuePrompt = GetLang("frmHisPayFormBankFee.txtKeyWord.Properties.NullValuePrompt", this.txtKeyWord.Properties.NullValuePrompt);

                this.btnSearch.Text = GetLang("frmHisPayFormBankFee.btnSearch.Text", this.btnSearch.Text);
                this.btnAdd.Text = GetLang("frmHisPayFormBankFee.btnAdd.Text", this.btnAdd.Text);
                this.btnEdit.Text = GetLang("frmHisPayFormBankFee.btnEdit.Text", this.btnEdit.Text);
                this.btnReset.Text = GetLang("frmHisPayFormBankFee.btnReset.Text", this.btnReset.Text);

                this.bbtnAdd.Caption = GetLang("frmHisPayFormBankFee.bbtnAdd.Caption", this.bbtnAdd.Caption);
                this.bbtnEdit.Caption = GetLang("frmHisPayFormBankFee.bbtnEdit.Caption", this.bbtnEdit.Caption);
                this.bbtnReset.Caption = GetLang("frmHisPayFormBankFee.bbtnReset.Caption", this.bbtnReset.Caption);
                this.bbtnSearch.Caption = GetLang("frmHisPayFormBankFee.bbtnSearch.Caption", this.bbtnSearch.Caption);
                this.bbtnFocusDefault.Caption = GetLang("frmHisPayFormBankFee.bbtnFocusDefault.Caption", this.bbtnFocusDefault.Caption);

                this.statusActiveText = GetLang("frmHisPayFormBankFee.Status.Active.Text", this.statusActiveText);
                this.statusLockedText = GetLang("frmHisPayFormBankFee.Status.Locked.Text", this.statusLockedText);
                this.allBankText = GetLang("frmHisPayFormBankFee.cboBank.AllItem.Text", this.allBankText);
                this.cboBank.Properties.NullText = this.allBankText;

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

        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationPayForm validPayForm = new ValidationPayForm();
                validPayForm.cboPayForm = cboPayForm;
                validPayForm.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboPayForm, validPayForm);

                ValidationFeeRate validFeeRate = new ValidationFeeRate();
                validFeeRate.spinFeeRate = spinFeeRate;
                validFeeRate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spinFeeRate, validFeeRate);

                ValidationFeeName validFeeName = new ValidationFeeName();
                validFeeName.txtFeeName = txtFeeName;
                validFeeName.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtFeeName, validFeeName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load grid (client-side filter + paging)
        private void FillDataToControl()
        {
            FillDataToControl(true);
        }

        /// <summary>
        /// Tai du lieu len luoi.
        /// reloadFromServer = true: lay lai toan bo tu API (dung khi mo form, sau khi luu/khoa).
        /// reloadFromServer = false: chi loc lai tren cache allData (dung khi tim kiem).
        /// </summary>
        private void FillDataToControl(bool reloadFromServer)
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

                if (reloadFromServer || allData == null)
                {
                    ReloadAllDataFromServer();
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControlFee);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay TOAN BO cau hinh (sap xep MODIFY_TIME giam dan) — cache vao allData de loc client.</summary>
        private void ReloadAllDataFromServer()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisPayFormBankFeeFilter filter = new HisPayFormBankFeeFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var data = new BackendAdapter(param).Get<List<HIS_PAY_FORM_BANK_FEE>>(HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_GET, ApiConsumers.MosConsumer, filter, param);
                allData = data ?? new List<HIS_PAY_FORM_BANK_FEE>();
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                allData = new List<HIS_PAY_FORM_BANK_FEE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Phan trang in-memory tren danh sach da loc theo tu khoa.</summary>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                List<HIS_PAY_FORM_BANK_FEE> filtered = FilterByKeyword(allData, txtKeyWord.Text);
                dataTotal = filtered.Count;

                List<HIS_PAY_FORM_BANK_FEE> page = (limit > 0)
                    ? filtered.Skip(startPage).Take(limit).ToList()
                    : filtered;

                gridViewFee.BeginUpdate();
                try
                {
                    gridViewFee.GridControl.DataSource = page;
                    rowCount = page.Count;
                }
                finally
                {
                    gridViewFee.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Loc theo tu khoa tren: ten hinh thuc thanh toan, ten ngan hang, ten phu phi.
        /// (Backend KEY_WORD khong tim duoc ten HT/NH vi nam o bang khac → loc client.)
        /// </summary>
        private List<HIS_PAY_FORM_BANK_FEE> FilterByKeyword(List<HIS_PAY_FORM_BANK_FEE> source, string keyword)
        {
            if (source == null) return new List<HIS_PAY_FORM_BANK_FEE>();
            if (string.IsNullOrWhiteSpace(keyword)) return source;

            string kw = keyword.Trim().ToLower();
            return source.Where(o =>
            {
                string payFormName;
                payFormNameDict.TryGetValue(o.PAY_FORM_ID, out payFormName);

                string bankName = (o.BANK_ID == null) ? allBankText : null;
                if (o.BANK_ID != null) bankNameDict.TryGetValue(o.BANK_ID.Value, out bankName);

                return (!string.IsNullOrEmpty(payFormName) && payFormName.ToLower().Contains(kw))
                    || (!string.IsNullOrEmpty(bankName) && bankName.ToLower().Contains(kw))
                    || (!string.IsNullOrEmpty(o.FEE_NAME) && o.FEE_NAME.ToLower().Contains(kw));
            }).ToList();
        }
        #endregion

        #region UI state
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
                this.currentData = null;
                txtKeyWord.Text = "";
                cboPayForm.EditValue = null;
                cboBank.EditValue = ALL_BANK_ID;
                spinFeeRate.EditValue = null;
                txtFeeName.Text = "";
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
                this.currentData = null;
                cboPayForm.EditValue = null;
                cboBank.EditValue = ALL_BANK_ID;
                spinFeeRate.EditValue = null;
                txtFeeName.Text = "";
                cboPayForm.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Save (Create khi them, Update khi sua)
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                if (!dxValidationProvider1.Validate())
                    return;

                // Chan ten phu phi > 200 ky tu (kiem tra tuong minh, dam bao luon chay khi bam Luu)
                if (Inventec.Common.String.CountVi.Count(txtFeeName.Text.Trim()) > HIS.Desktop.Plugins.HisPayFormBankFee.Validtion.ValidationFeeName.MAX_LENGTH)
                {
                    dxErrorProvider1.SetError(txtFeeName, Resources.ResourceMessage.TenPhuPhiVuotQuaGioiHan, ErrorType.Warning);
                    XtraMessageBox.Show(Resources.ResourceMessage.TenPhuPhiVuotQuaGioiHan,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFeeName.Focus();
                    return;
                }
                dxErrorProvider1.SetError(txtFeeName, "", ErrorType.None);

                if (IsDuplicatePair(param))
                {
                    dxErrorProvider1.SetError(cboBank, Resources.ResourceMessage.CauHinhPhuPhiDaTonTai, ErrorType.Warning);
                    XtraMessageBox.Show(Resources.ResourceMessage.CauHinhPhuPhiDaTonTai,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                dxErrorProvider1.SetError(cboBank, "", ErrorType.None);

                WaitingManager.Show();

                bool isEdit = (this.ActionType == GlobalVariables.ActionEdit && this.currentData != null && this.currentData.ID > 0);

                HIS_PAY_FORM_BANK_FEE updateDTO = new HIS_PAY_FORM_BANK_FEE();
                if (isEdit)
                {
                    // Sua: load ban ghi hien tai (giu ID + audit) roi cap nhat
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                else
                {
                    // Them moi: ID de backend tu sinh
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                }

                UpdateDTOFromDataForm(ref updateDTO);

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));

                // Backend /Update bat buoc ID > 0 => Them moi PHAI goi /Create
                string uri = isEdit
                    ? HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_UPDATE
                    : HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_CREATE;

                var resultData = new BackendAdapter(param).Post<HIS_PAY_FORM_BANK_FEE>(uri, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToControl();
                    if (!isEdit)
                    {
                        ResetFormData();
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(
                    "SaveProcess that bai." + Inventec.Common.Logging.LogUtil.TraceData("ActionType", (object)this.ActionType), ex);
            }
        }

        private bool IsDuplicatePair(CommonParam param)
        {
            bool isDuplicate = false;
            try
            {
                long payFormId = Inventec.Common.TypeConvert.Parse.ToInt64((cboPayForm.EditValue ?? "0").ToString());
                long bankSelected = Inventec.Common.TypeConvert.Parse.ToInt64((cboBank.EditValue ?? "0").ToString());
                long? bankId = bankSelected <= ALL_BANK_ID ? (long?)null : bankSelected;
                long currentId = (this.currentData != null) ? this.currentData.ID : 0;

                HisPayFormBankFeeFilter filter = new HisPayFormBankFeeFilter();
                filter.PAY_FORM_ID = payFormId;

                var existed = new BackendAdapter(param).Get<List<HIS_PAY_FORM_BANK_FEE>>(HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_GET, ApiConsumers.MosConsumer, filter, param);
                if (existed != null)
                {
                    isDuplicate = existed.Any(o => o.PAY_FORM_ID == payFormId
                        && o.BANK_ID == bankId
                        && o.ID != currentId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isDuplicate;
        }

        private void UpdateDTOFromDataForm(ref HIS_PAY_FORM_BANK_FEE updateDTO)
        {
            try
            {
                updateDTO.PAY_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPayForm.EditValue ?? "0").ToString());

                long bankSelected = Inventec.Common.TypeConvert.Parse.ToInt64((cboBank.EditValue ?? "0").ToString());
                updateDTO.BANK_ID = bankSelected <= ALL_BANK_ID ? (long?)null : bankSelected;

                if (spinFeeRate.EditValue == null || string.IsNullOrWhiteSpace(spinFeeRate.Text))
                {
                    updateDTO.FEE_RATE = null;
                }
                else
                {
                    updateDTO.FEE_RATE = Inventec.Common.TypeConvert.Parse.ToDecimal(spinFeeRate.EditValue.ToString());
                }

                updateDTO.FEE_NAME = txtFeeName.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_PAY_FORM_BANK_FEE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPayFormBankFeeFilter filter = new HisPayFormBankFeeFilter();
                filter.ID = currentId;
                var data = new BackendAdapter(param).Get<List<HIS_PAY_FORM_BANK_FEE>>(HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (data != null)
                {
                    currentDTO = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Khoa/mo khoa qua /ChangeLock — backend tu toggle IS_ACTIVE (xu ly duoc ca ban ghi dang khoa).</summary>
        private void ToggleActive(HIS_PAY_FORM_BANK_FEE rowData)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (rowData == null) return;

                WaitingManager.Show();
                var resultData = new BackendAdapter(param).Post<HIS_PAY_FORM_BANK_FEE>(HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                bool success = resultData != null;
                if (success)
                {
                    FillDataToControl();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Fill data to editor
        private void ChangedDataRow(HIS_PAY_FORM_BANK_FEE data)
        {
            try
            {
                if (data != null)
                {
                    this.currentData = data;
                    FillDataToEditorControl(data);

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    // Khong cho sua khi du lieu da bi khoa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_PAY_FORM_BANK_FEE data)
        {
            try
            {
                if (data != null)
                {
                    cboPayForm.EditValue = data.PAY_FORM_ID;
                    cboBank.EditValue = data.BANK_ID ?? ALL_BANK_ID;
                    spinFeeRate.EditValue = data.FEE_RATE;
                    txtFeeName.Text = data.FEE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button events
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Tim kiem: chi loc lai tren cache (khong tai lai server)
                FillDataToControl(false);
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
                this.ActionType = GlobalVariables.ActionAdd;
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
                this.ActionType = GlobalVariables.ActionEdit;
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
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
        #endregion

        #region Grid events
        private void gridControlFee_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFee_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
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

        private void gridViewFee_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    HIS_PAY_FORM_BANK_FEE data = (HIS_PAY_FORM_BANK_FEE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    bool isActive = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    if (e.Column.FieldName == "LOCK")
                    {
                        // Dang hoat dong -> icon MO KHOA (click de khoa); Da khoa -> icon KHOA (click de mo)
                        e.RepositoryItem = (isActive ? btnGUnLock : btnGLock);
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        // Dang hoat dong -> nut xoa DO (xoa duoc); Da khoa -> nut xoa DEN disable (khong xoa)
                        e.RepositoryItem = (isActive ? btnGDelete : (btnGDeleteDisable ?? btnGDelete));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        /// <summary>To mau cot Trang thai: Hoat dong = xanh, Tam khoa = do.</summary>
        private void gridViewFee_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "STATUS_STR")
                {
                    HIS_PAY_FORM_BANK_FEE data = (HIS_PAY_FORM_BANK_FEE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = System.Drawing.Color.Red;
                    else
                        e.Appearance.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewFee_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_PAY_FORM_BANK_FEE pData = (HIS_PAY_FORM_BANK_FEE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (pData == null) return;

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "PAY_FORM_NAME_STR")
                    {
                        string name;
                        e.Value = payFormNameDict.TryGetValue(pData.PAY_FORM_ID, out name) ? name : "";
                    }
                    else if (e.Column.FieldName == "BANK_NAME_STR")
                    {
                        if (pData.BANK_ID == null)
                        {
                            e.Value = allBankText;
                        }
                        else
                        {
                            string name;
                            e.Value = bankNameDict.TryGetValue(pData.BANK_ID.Value, out name) ? name : "";
                        }
                    }
                    else if (e.Column.FieldName == "STATUS_STR")
                    {
                        e.Value = (pData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? statusActiveText : statusLockedText;
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

        // Icon KHOA (hien khi ban ghi DANG KHOA) -> click de MO KHOA
        private void btnGLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ToggleActive(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Icon MO KHOA (hien khi ban ghi DANG HOAT DONG) -> click de KHOA
        private void btnGUnLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ToggleActive(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Cot Xoa -> xoa mem ban ghi
        private void btnGDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_PAY_FORM_BANK_FEE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Editor key navigation
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
                    gridViewFee.Focus();
                    gridViewFee.FocusedRowHandle = 0;
                    var rowData = (HIS_PAY_FORM_BANK_FEE)gridViewFee.GetFocusedRow();
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

        private void cboPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) cboBank.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) spinFeeRate.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFeeRate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFeeName.Focus();
                    txtFeeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFeeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                        btnAdd.Focus();
                    else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
