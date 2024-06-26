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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus
    {
        #region Control Detail
        private void txtBloodAboCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtBloodAboCode.Text.Trim()))
                    {
                        string code = txtBloodAboCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.ToLower().Contains(code)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            showCbo = false;
                            txtBloodAboCode.Text = listData.First().BLOOD_ABO_CODE;
                            cboBloodAbo.EditValue = listData.First().ID;
                            cboBloodRh.Focus();
                            cboBloodRh.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboBloodAbo.Focus();
                        cboBloodAbo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodAbo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboBloodRh.Focus();
                    cboBloodRh.SelectAll();
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
                    var data = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodAbo.EditValue));
                    if (data != null)
                    {
                        txtBloodAboCode.Text = data.BLOOD_ABO_CODE;
                    }
                }
                else
                {
                    txtBloodAboCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
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
                //if (cboBloodRh.EditValue != null)
                //{
                //    var data = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodRh.EditValue));
                //    if (data != null)
                //    {
                //        txtBloodRhCode.Text = data.BLOOD_ABO_CODE;
                //    }
                //}
                //else
                //{
                //    txtBloodRhCode.Text = "";
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGiveCode.Focus();
                    txtGiveCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGiveCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGiveName.Focus();
                    txtGiveName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGiveName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackingTime.Focus();
                    txtPackingTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        #region Thời gian đóng gói
        private void txtPackingTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtPackingTime.EditValue = dt;
                        dtPackingTime.Update();
                        dtPackingTime.Focus();
                        dtPackingTime.ShowPopup();
                    }
                    else
                    {
                        dtPackingTime.Visible = true;
                        dtPackingTime.Focus();
                        dtPackingTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Base.ResourceMessageLang.NgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPackingTime.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_Leave(object sender, EventArgs e)
        {
            try
            {
                dtPackingTime.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                txtPackageNumber.Focus();
                txtPackageNumber.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPackingTime.Text))
                    {
                        dtPackingTime.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                        txtPackageNumber.Focus();
                        txtPackageNumber.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtPackingTime.EditValue = dt;
                        dtPackingTime.Update();
                        txtPackageNumber.Focus();
                        txtPackageNumber.SelectAll();
                    }
                    else
                    {
                        dtPackingTime.Visible = true;
                        dtPackingTime.Focus();
                        dtPackingTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPackingTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtPackingTime.Visible = false;
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPackingTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPackingTime.Visible = false;
                    dtPackingTime.Update();
                    txtPackingTime.Text = dtPackingTime.DateTime.ToString("dd/MM/yyyy");
                    //txtPackageNumber.Focus();
                    //txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPackingTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtPackingTime.EditValue != null && dtPackingTime.DateTime != DateTime.MinValue)
                {
                    txtPackingTime.Text = dtPackingTime.DateTime.ToString("dd/MM/yyyy");
                }
                else
                {
                    txtPackingTime.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtPackageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkIsInfect.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsInfect_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsInfect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                dtExpiredDate.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtBloodCode.Text))
                    {
                        if (layoutBtnAdd.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            btnAdd.PerformClick();
                        }
                        else if (layoutBtnUpdate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            btnUpdate.PerformClick();
                        }
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Control Common
        private void txtImpMestType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboImpMestType.Focus();
                    cboImpMestType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMestType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtImpMestType.Text))
                    {
                        string key = txtImpMestType.Text.Trim().ToLower();
                        var listData = listImpMestType.Where(o => o.IMP_MEST_TYPE_CODE.ToLower().Contains(key) || o.IMP_MEST_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboImpMestType.EditValue = listData.First().ID;
                            txtImpSource.Focus();
                            txtImpSource.SelectAll();
                            RemoveControlDxError2();
                        }
                    }
                    if (!valid)
                    {
                        cboImpMestType.Focus();
                        cboImpMestType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMestType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtImpSource.Focus();
                    txtImpSource.SelectAll();
                    AllowImpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMestType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImpMestType.EditValue != null)
                {
                    this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                    if (this.currentImpMestType != null)
                    {
                        txtImpMestType.Text = this.currentImpMestType.IMP_MEST_TYPE_NAME;
                    }
                    else
                    {
                        txtImpMestType.Text = "";
                    }
                }
                else
                {
                    this.currentImpMestType = null;
                    txtImpMestType.Text = "";
                }
                SetControlEnableImMestTypeManu();
                SetDataSourceMediStock();
                FillDataToGridBloodType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediStock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboMediStock.Focus();
                    cboMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediStock.Properties.DataSource == null)
                        SendKeys.Send("{TAB}");
                    else
                    {
                        bool valid = false;
                        if (!String.IsNullOrEmpty(txtMediStock.Text))
                        {
                            var key = txtMediStock.Text.ToLower();
                            var listData = listMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                            if (listData != null && listData.Count == 1)
                            {
                                valid = true;
                                cboMediStock.EditValue = listData.First().ID;
                                txtImpSource.Focus();
                                txtImpSource.SelectAll();
                            }
                        }
                        if (!valid)
                        {
                            cboMediStock.Focus();
                            cboMediStock.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtImpSource.Focus();
                    txtImpSource.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtMediStock.Text = "";
                if (cboMediStock.EditValue != null)
                {
                    var mediStock = listMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                    if (mediStock != null)
                    {
                        txtMediStock.Text = mediStock.MEDI_STOCK_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpSource_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboImpSource.Focus();
                    cboImpSource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpSource_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtImpSource.Text))
                    {
                        string key = txtImpSource.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(o => o.IMP_SOURCE_CODE.ToLower().Contains(key) || o.IMP_SOURCE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboImpSource.EditValue = listData.First().ID;
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboImpSource.Focus();
                        cboImpSource.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtImpSource.Text = "";
                if (cboImpSource.EditValue != null)
                {
                    var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                    if (impSource != null)
                    {
                        txtImpSource.Text = impSource.IMP_SOURCE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void UpdateComboSupplierDataSource(List<long> _ListSupplierId)
        {
            try
            {
                if (_ListSupplierId != null && _ListSupplierId.Count > 0)
                {
                    var _ListSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => _ListSupplierId.Contains(o.ID)).ToList();
                    cboSupplier.Properties.DataSource = _ListSupplier;
                    if (_ListSupplier.Count == 1)
                    {
                        cboSupplier.EditValue = _ListSupplierId.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //try
        //{
        //    WaitingManager.Show();
        //    this.currentBid = null;
        //    if (cboBid.EditValue != null)
        //    {
        //        txtBidNumber.Enabled = false;
        //        txtBidNumOrder.ReadOnly = true;
        //        txtBidNumber.ReadOnly = true;
        //        txtBidNumber.Text = "";
        //        txtBidNumOrder.Text = "";
        //        txtBidNumber.Text = "";
        //        this.currentBid = BackendDataWorker.Get<V_HIS_BID>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBid.EditValue));
        //        cboSupplier.EditValue = null;
        //    }

        //    LoadDataByBid();
        //    SetDataSourceGridControlMediMate();
        //    WaitingManager.Hide();
        //}
        //catch (Exception ex)
        //{
        //    WaitingManager.Hide();
        //    Inventec.Common.Logging.LogSystem.Error(ex);
        //}

        #region Số chứng chừ
        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtDocumentDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        txtDeliever.Focus();
                        txtDeliever.SelectAll();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Base.ResourceMessageLang.NgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                txtDocumentNumber.Focus();
                txtDocumentNumber.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtDocumentNumber.Text))
                    {
                        dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentNumber.Text);
                        txtDocumentNumber.Focus();
                        txtDocumentNumber.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentNumber.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        txtDocumentNumber.Focus();
                        txtDocumentNumber.SelectAll();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.Visible = false;
                    dtDocumentDate.Update();
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                {
                    txtDocumentDate.Text = dtDocumentDate.DateTime.ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDocumentDate.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void txtDeliever_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                    if (spinDiscountRatio.EditValue != null)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else
                    {
                        spinDiscountPrice.Focus();
                        spinDiscountPrice.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscountRatio.EditValue != null && spinDiscountRatio.EditValue != spinDiscountRatio.OldEditValue)
                {
                    var totalPrice = dicBloodAdo.Sum(s => s.Value.IMP_PRICE);
                    spinDiscountPrice.Value = totalPrice * (spinDiscountRatio.Value / 100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinDiscountPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscountPrice.EditValue != null && spinDiscountPrice.EditValue != spinDiscountPrice.OldEditValue)
                {
                    var totalPrice = dicBloodAdo.Sum(s => s.Value.IMP_PRICE);
                    if (totalPrice > 0)
                    {
                        spinDiscountRatio.Value = (spinDiscountPrice.Value / totalPrice) * 100;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPackingTime.EditValue != null && txtPackingTime.Text != "")
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtPackingTime.Text);
                    var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(o => o.ID == this.currentBlood.BLOOD_TYPE_ID).FirstOrDefault();
                    dtExpiredDate.EditValue = dt.Value.AddDays(bloodType.ALERT_EXPIRED_DATE ?? 0);
                    
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
