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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.ManuImpMestEdit.Validation;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using System.Collections;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.ManuImpMestEdit
{
    public partial class FormManuImpMestEdit : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        long manuImpMestId;
        //Inventec.Desktop.Common.Modules.Module module;
        System.Globalization.CultureInfo cultureLang;
        int positionHandle = -1;
        bool isShowContainerSupplier = false;
        bool isShowContainerSupplierForChoose = false;
        bool isShow = true;
        internal int d = 0;

        List<HIS_SUPPLIER> listSupplier = new List<HIS_SUPPLIER>();
        List<V_HIS_BID> _HisBidBySuppliers { get; set; }
        HIS_SUPPLIER currentSupplier = null;
        MOS.EFMODEL.DataModels.HIS_IMP_MEST manuImpMest;
        internal HIS_SUPPLIER currentSupplierForEdit;
        HIS_IMP_MEST _currentImpMest = new HIS_IMP_MEST();
        #endregion

        #region Construct
        public FormManuImpMestEdit(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                manuImpMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormManuImpMestEdit(long manuImpId, Inventec.Desktop.Common.Modules.Module module)
            : this(module)
        {
            try
            {
                this.manuImpMestId = manuImpId;
                this.Text = module.text;
                LoadImpMest(this.manuImpMestId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMest(long _impMest)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpMestFilter _impMestFilter = new HisImpMestFilter();
                _impMestFilter.ID = _impMest;

                var dataImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, _impMestFilter, param).FirstOrDefault();

                if (dataImpMest == null)
                    return;

                this._currentImpMest = dataImpMest;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormManuImpMestEdit_Load(object sender, EventArgs e)
        {
            try
            {
                listSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();

                SetIcon();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                ValidControls();

                GetFontSizeForm();


                LoadDataToComboSupplier(listSupplier);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
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

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDeliverer.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DELIVERER",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DESCRIPTION",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDiscount.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DISCOUNT",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDiscountRatio.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DISCOUNT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDocumentDate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DOCUMENT_DATE",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
                this.lciDocumentNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DOCUMENT_NUMBER",
                    Resources.ResourceLanguageManager.LanguageFormManuImpMestEdit,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                if (manuImpMestId != null)
                {
                    MOS.Filter.HisImpMestFilter filter = new MOS.Filter.HisImpMestFilter();
                    filter.ID = manuImpMestId;
                    var manuImpMests = new Inventec.Common.Adapter.BackendAdapter
                        (new Inventec.Core.CommonParam()).Get
                        <List<MOS.EFMODEL.DataModels.HIS_IMP_MEST>>
                        (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (manuImpMests != null && manuImpMests.Count > 0)
                    {

                        manuImpMest = manuImpMests.FirstOrDefault();
                        if (manuImpMest.SUPPLIER_ID != null)
                        {

                            txtNhaCC.Text = listSupplier.Where(p => p.ID == manuImpMest.SUPPLIER_ID).Distinct().First().SUPPLIER_NAME;
                        }
                        txtDeliverer.Text = manuImpMest.DELIVERER;
                        txtDescription.Text = manuImpMest.DESCRIPTION;
                        txtDocumentNumber.Text = manuImpMest.DOCUMENT_NUMBER;
                        dtDocumentDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(manuImpMest.DOCUMENT_DATE.ToString());
                        spinDiscount.EditValue = manuImpMest.DISCOUNT;
                        spinDiscountRatio.EditValue = manuImpMest.DISCOUNT_RATIO * 100;
                        spDocumentPrice.EditValue = manuImpMest.DOCUMENT_PRICE;
                        txtBillSignature.Text = manuImpMest.INVOICE_SYMBOL;
                        if (manuImpMest.IMP_TIME != null) { 
                        dteImpTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(manuImpMest.IMP_TIME ?? 0) ?? DateTime.Now;
                        dteImpTime.Enabled = true;
                    }
                        if (manuImpMest.APPROVAL_TIME != null)
                        {
                            dteApprovalTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(manuImpMest.APPROVAL_TIME ?? 0) ?? DateTime.Now;
                            dteApprovalTime.Enabled = true;
                        }
                        Inventec.Common.Logging.LogSystem.Debug("manuImpMest.INVOICE_SYMBOL: " + manuImpMest.INVOICE_SYMBOL);
                    }
                }
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
                CommonParam paramCommon = new CommonParam();
                btnSave.Focus();
                bool success = false;
                positionHandle = -1;
                if (!dxValidationProvider.Validate())
                    return;

                WaitingManager.Show();
                if (!String.IsNullOrEmpty(txtDocumentNumber.Text))
                {
                    if (CheckDocumentNumber(txtDocumentNumber.Text, txtBillSignature.Text))
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.SoChungTuDaDuocSuDung, Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            return;
                    }

                }

                bool checkUpdateNhaCC = Setvalue(ref manuImpMest);
                if (checkUpdateNhaCC)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => manuImpMest), manuImpMest));
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>("api/HisImpMest/ManuUpdateInfo", ApiConsumer.ApiConsumers.MosConsumer, manuImpMest, paramCommon);

                    if (apiresul != null)
                    {
                        success = true;
                        this.Close();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                if (!checkUpdateNhaCC)
                {
                    Inventec.Common.Logging.LogSystem.Error("Thông tin nhà cung cấp không đúng");
                    MessageBox.Show("Thông tin nhà cung cấp không đúng!", "Xử lý thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    popupControlContainerSupplier.HidePopup();
                }
                else
                    MessageManager.Show(this, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckDocumentNumber(string documentNumber, string billSignature)
        {
            bool result = false;
            try
            {
                MOS.Filter.HisImpMestViewFilter manuImpMestViewFilter = new MOS.Filter.HisImpMestViewFilter();
                manuImpMestViewFilter.DOCUMENT_NUMBER__EXACT = documentNumber;
                manuImpMestViewFilter.SUPPLIER_ID = this.manuImpMest.SUPPLIER_ID;
                var manuImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, manuImpMestViewFilter, new CommonParam());
                if (manuImpMests != null && manuImpMests.Count > 0 && !string.IsNullOrEmpty(documentNumber))
                {
                    foreach (var item in manuImpMests)
                    {
                        if (!String.IsNullOrEmpty(billSignature))
                        {
                            if (item.DOCUMENT_NUMBER.Equals(documentNumber.Trim()) && item.ID != this.manuImpMest.ID && item.SUPPLIER_ID == this.manuImpMest.SUPPLIER_ID && item.INVOICE_SYMBOL == billSignature)
                            {
                                result = true;
                                break;
                            }

                        }
                        else
                        {
                            if (item.DOCUMENT_NUMBER.Equals(documentNumber.Trim()) && item.ID != this.manuImpMest.ID && item.SUPPLIER_ID == this.manuImpMest.SUPPLIER_ID && (item.INVOICE_SYMBOL == null || item.INVOICE_SYMBOL == ""))
                            {
                                result = true;
                                break;
                            }
                        }

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

        private bool Setvalue(ref MOS.EFMODEL.DataModels.HIS_IMP_MEST manuImpMest)
        {
            try
            {
                bool checkNhaCC = true;
                manuImpMest.ID = this.manuImpMestId;
                manuImpMest.DELIVERER = txtDeliverer.Text;
                if (spDocumentPrice.EditValue != null)
                {
                    manuImpMest.DOCUMENT_PRICE = spDocumentPrice.Value;
                }
                else
                {
                    manuImpMest.DOCUMENT_PRICE = null;
                }
                manuImpMest.DESCRIPTION = txtDescription.Text;
                manuImpMest.DISCOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinDiscount.EditValue.ToString());
                manuImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                {
                    manuImpMest.DOCUMENT_DATE = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
                }
                manuImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                manuImpMest.INVOICE_SYMBOL = txtBillSignature.Text;
                if (txtNhaCC.Text != "")
                {
                    var rs = listSupplier.Where(p => p.SUPPLIER_NAME == txtNhaCC.Text);
                    if (rs != null && rs.Count() > 0)
                    {
                        var result = rs.FirstOrDefault();

                        manuImpMest.SUPPLIER_ID = result.ID;
                    }
                    else
                    {
                        checkNhaCC = false;
                    }

                }
                if(dteImpTime.EditValue!=null && dteImpTime.DateTime != DateTime.MinValue)
                    manuImpMest.IMP_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteImpTime.DateTime);
                if (dteApprovalTime.EditValue != null && dteApprovalTime.DateTime != DateTime.MinValue)
                    manuImpMest.APPROVAL_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteApprovalTime.DateTime);
                return checkNhaCC;
            }

            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void ValidControls()
        {
            ValidDiscount();
            ValidDiscountRatio();
            ValidNhaCC();
            ValidDate();
        }

		private void ValidDate()
		{
			try
			{
                MatchDateValidationRule rule = new MatchDateValidationRule();
                rule.dteImpTime = dteImpTime;
                rule.dteApprovalTime = dteApprovalTime;
                rule.ErrorText = "Thời gian nhập không được nhỏ hơn thời gian duyệt";
                rule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteImpTime, rule);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

		private void ValidDiscount()
        {
            try
            {
                DiscountValidationRule DiscountValidationRule = new DiscountValidationRule();
                DiscountValidationRule.spinDiscount = spinDiscount;
                DiscountValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                DiscountValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(spinDiscount, DiscountValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidNhaCC()
        {
            try
            {
                NhaCCDValidationRule nhaCCValidationRule = new NhaCCDValidationRule();
                nhaCCValidationRule.txt = txtNhaCC;
                nhaCCValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                nhaCCValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtNhaCC, nhaCCValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidDiscountRatio()
        {
            try
            {
                DiscountRatioValidationRule DiscountRatioValidationRule = new DiscountRatioValidationRule();
                DiscountRatioValidationRule.spinDiscountRatio = spinDiscountRatio;
                DiscountRatioValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                DiscountRatioValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(spinDiscountRatio, DiscountRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Enter
        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.Focus();
                    dtDocumentDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                spDocumentPrice.Focus();
                spDocumentPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDeliverer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscount.Focus();
                    spinDiscount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNhaCC.Focus();
                    txtNhaCC.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDocumentPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBillSignature.Focus();
                    txtBillSignature.SelectAll();
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
                    if (btnSave.Enabled)
                    {
                        btnSave.Select();
                        btnSave.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region Shortcut
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                popupControlContainerSupplier.HidePopup();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtNhaCC_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    //if (this._currentImpMest != null)
                    //    return;
                    isShowContainerSupplier = !isShowContainerSupplier;
                    if (isShowContainerSupplier)
                    {
                        Rectangle buttonBounds = new Rectangle(txtNhaCC.Bounds.X + 5 + this.Location.X, txtNhaCC.Bounds.Y + lciNhaCC.Height + this.Location.Y, txtNhaCC.Bounds.Width, txtNhaCC.Bounds.Height);
                        popupControlContainerSupplier.ShowPopup(new Point(buttonBounds.X + d / 3, buttonBounds.Bottom + d));

                        if (this.currentSupplierForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HIS_SUPPLIER>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlNhaCC.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].ID == this.currentSupplierForEdit.ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewNhaCC.FocusedRowHandle = rowIndex;
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetFontSizeForm()
        {
            var demSize = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize();
            if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize1025)
            {
                d = 11;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize975)
            {
                d = 10;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize925)
            {
                d = 8;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize875)
            {
                d = 7;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize825)
            {
                d = 5;
            }
        }

        private void txtNhaCC_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDeliverer.Focus();
                    txtDeliverer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCC_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this._currentImpMest != null)
                //    return;
                if (!String.IsNullOrEmpty(txtNhaCC.Text))
                {
                    txtNhaCC.Refresh();
                    if (isShowContainerSupplierForChoose)
                    {
                        gridViewNhaCC.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerSupplier)
                        {
                            isShowContainerSupplier = true;
                        }

                        //Filter data
                        gridViewNhaCC.ActiveFilterString = "[SUPPLIER_NAME] Like '%" + txtNhaCC.Text
                            + "%' OR [SUPPLIER_CODE] Like '%" + txtNhaCC.Text + "%'";
                        gridViewNhaCC.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewNhaCC.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewNhaCC.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewNhaCC.FocusedRowHandle = 0;
                        gridViewNhaCC.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewNhaCC.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtNhaCC.Bounds.X + 5 + this.Location.X, txtNhaCC.Bounds.Y + this.Location.Y + lciNhaCC.Height, txtNhaCC.Bounds.Width, txtNhaCC.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerSupplier.ShowPopup(new Point(buttonBounds.X + d / 3, buttonBounds.Bottom + d));
                            isShow = false;
                        }

                        txtNhaCC.Focus();
                    }
                    isShowContainerSupplierForChoose = false;
                }
                else
                {
                    gridViewNhaCC.ActiveFilter.Clear();
                    // this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerSupplier)
                    {
                        popupControlContainerSupplier.HidePopup();
                    }
                }
                // this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboSupplier(List<HIS_SUPPLIER> data)
        {
            try
            {
                RebuildSupllierWithInControlContainer(data);//xuandv
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RebuildSupllierWithInControlContainer(List<HIS_SUPPLIER> data)
        {
            try
            {
                gridViewNhaCC.BeginUpdate();
                gridViewNhaCC.Columns.Clear();
                popupControlContainerSupplier.Width = txtNhaCC.Bounds.Width;
                popupControlContainerSupplier.Height = 200;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "SUPPLIER_CODE";
                col2.Caption = "Mã";
                col2.Width = 100;
                col2.VisibleIndex = 1;
                gridViewNhaCC.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "SUPPLIER_NAME";
                col1.Caption = "Tên";
                col1.Width = 350;
                col1.VisibleIndex = 2;
                gridViewNhaCC.Columns.Add(col1);

                gridViewNhaCC.GridControl.DataSource = data;
                gridViewNhaCC.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void Supplier_RowClick(object data)
        {
            try
            {
                this.currentSupplierForEdit = new HIS_SUPPLIER();
                this.currentSupplierForEdit = data as HIS_SUPPLIER;
                if (this.currentSupplierForEdit != null)
                {
                    this.txtNhaCC.Text = this.currentSupplierForEdit.SUPPLIER_NAME;
                }

                WaitingManager.Show();


                this.currentSupplier = null;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }

        public static CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private static ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }


        private void popupControlContainerSupplier_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCC_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (gridViewMediMaty.RowCount == 1)
                    //{
                    var medicineTypeADOForEdit = this.gridViewNhaCC.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerSupplier = false;
                        isShowContainerSupplierForChoose = true;
                        popupControlContainerSupplier.HidePopup();

                        Supplier_RowClick(medicineTypeADOForEdit);
                    }

                    //if (checkOutBid.Enabled && checkOutBid.Visible)
                    //{
                    //    checkOutBid.Focus();
                    //    checkOutBid.SelectAll();
                    //}
                }
                else if (e.KeyCode == Keys.Down)
                {
                    //if (this._currentImpMest != null)
                    //    return;
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewNhaCC.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtNhaCC.Bounds.X + 5 + this.Location.X, txtNhaCC.Bounds.Y + this.Location.Y + lciNhaCC.Height, txtNhaCC.Bounds.Width, txtNhaCC.Bounds.Height);
                    popupControlContainerSupplier.ShowPopup(new Point(buttonBounds.X + d / 3, buttonBounds.Bottom + d));
                    gridViewNhaCC.Focus();
                    gridViewNhaCC.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtNhaCC.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNhaCC_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void gridViewNhaCC_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var row = this.gridViewNhaCC.GetFocusedRow();
                    if (row != null)
                    {
                        isShowContainerSupplier = false;
                        isShowContainerSupplierForChoose = true;
                        popupControlContainerSupplier.HidePopup();

                        Supplier_RowClick(row);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewNhaCC.Focus();
                    this.gridViewNhaCC.FocusedRowHandle = this.gridViewNhaCC.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNhaCC_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.OptionsFind.HighlightFindResults && !txtNhaCC.Text.Equals(string.Empty))
            {
                CriteriaOperator op = ConvertFindPanelTextToCriteriaOperator(txtNhaCC.Text, view, false);
                if (op is GroupOperator)
                {
                    string findText = txtNhaCC.Text;
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }
                else if (op is FunctionOperator)
                {
                    FunctionOperator func = op as FunctionOperator;
                    CriteriaOperator colNameOperator = func.Operands[0];
                    string colName = colNameOperator.LegacyToString().Replace("[", string.Empty).Replace("]", string.Empty);
                    if (!e.Column.FieldName.StartsWith(colName)) return;

                    CriteriaOperator valueOperator = func.Operands[1];
                    string findText = valueOperator.LegacyToString().ToLower().Replace("'", "");
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }
            }
        }

        private void gridViewNhaCC_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

        }

        private void gridViewNhaCC_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                var row = this.gridViewNhaCC.GetFocusedRow();
                if (row != null)
                {
                    popupControlContainerSupplier.HidePopup();
                    isShowContainerSupplier = false;
                    isShowContainerSupplierForChoose = true;
                    Supplier_RowClick(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spDocumentPrice.Focus();
                    spDocumentPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBillSignature_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscountRatio.Focus();
                    spinDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
