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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.HisAccountBookList
{
    public partial class UCHisAccountBookList : HIS.Desktop.Utility.UserControlBase
    {
        #region click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridAccountBookList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGridAccountBookList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam paramCommon = new CommonParam();
            try
            {
                bool success = false;
                positionHandle = -1;
                if (!btnSave.Enabled) return;
                if (!dxValidationProvider.Validate()) return;
                btnSave.Focus();
                WaitingManager.Show();
                if (this.DataAccountBook == null || this.ActionType == GlobalVariables.ActionAdd)
                    this.DataAccountBook = new MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK();
                UpdateDataFromAccountBook(DataAccountBook);
                var apiResult = new BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>(this.ActionType == GlobalVariables.ActionEdit ? HisRequestUriStore.HIS_ACCOUNT_BOOK_UPDATE : HisRequestUriStore.HIS_ACCOUNT_BOOK_CREATE, ApiConsumers.MosConsumer, DataAccountBook, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    
                    success = true;
                    this.DataAccountBook = apiResult;
                    if (!UpdateEINVOICE(apiResult)) success = false;
                    FillDataToControl(DataAccountBook);
                    FillDataToGridAccountBookList();
                    txtAccountBookName.Focus();
                    txtAccountBookName.SelectAll();
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        //CreateHisUserAccountBook(apiResult);
                    }
                    this.ActionType = GlobalVariables.ActionView;
                    EnableButton(this.ActionType);
                    WaitingManager.Hide();
                }
                else
                {
                    RefreshDataUpdate(DataAccountBook);
                }
                WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void CreateHisUserAccountBook(MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK AccountBook)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_USER_ACCOUNT_BOOK UserAccountBook = new MOS.EFMODEL.DataModels.HIS_USER_ACCOUNT_BOOK();
                UserAccountBook.ACCOUNT_BOOK_ID = AccountBook.ID;
                UserAccountBook.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam paramCommon = new CommonParam();
                var apiResult = new BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_USER_ACCOUNT_BOOK>("api/HisUserAccountBook/Create", ApiConsumers.MosConsumer, UserAccountBook, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableButton(ActionType);
                FillDataToControl(null);
                RemoveError();
                txtAccountBookCode.Focus();
                txtAccountBookCode.SelectAll();
                txtKhoGiay.Text = "";
                positionHandle = -1;
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
                DataAccountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                EditGridClick(DataAccountBook);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl(DataAccountBook);
                this.ActionType = GlobalVariables.ActionView;
                EnableButton(GlobalVariables.ActionView);
                RemoveError();
                positionHandle = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshDataUpdate(MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK data)
        {
            try
            {
                if (data != null)
                {
                    MOS.Filter.HisAccountBookViewFilter filter = new MOS.Filter.HisAccountBookViewFilter();
                    filter.ID = data.ID;

                    var list = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());
                    if (list != null && list.Count > 0)
                    {
                        ListAccountBook[ListAccountBook.IndexOf(data)] = list.First();
                        gridControlAccountBook.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveError()
        {
            try
            {
                dxValidationProvider.RemoveControlError(txtAccountBookCode);
                dxValidationProvider.RemoveControlError(txtAccountBookName);
                dxValidationProvider.RemoveControlError(spinCount);
                dxValidationProvider.RemoveControlError(spinFromNumberOrder);
                dxValidationProvider.RemoveControlError(txtTemplateCode);
                dxValidationProvider.RemoveControlError(txtSymbolCode);
                dxValidationProvider.RemoveControlError(chkForDeposit);
                dxValidationProvider.RemoveControlError(chkForBill);
                dxValidationProvider.RemoveControlError(chkForRepay);
                dxValidationProvider.RemoveControlError(chkForDebt);
                dxValidationProvider.RemoveControlError(chkForOtherSale);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region enter order
        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookName.Focus();
                    txtAccountBookName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountBookName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinCount.Enabled)
                    {
                        spinCount.Focus();
                    }
                    else
                        spinFromNumberOrder.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!spinFromNumberOrder.ReadOnly)
                    {
                        spinFromNumberOrder.Focus();
                    }
                    else
                    {
                        txtTemplateCode.Focus();
                        txtTemplateCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFromNumberOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillType.Focus();
                    cboBillType.ShowPopup();
                    //txtTemplateCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBillType.EditValue != null)
                    {
                        cboBillType.Properties.Buttons[1].Visible = true;
                        txtTemplateCode.Focus();
                        txtTemplateCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTemplateCode.Focus();
                    txtTemplateCode.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboBillType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkingShift_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSave.Enabled == true)
                        btnSave.Focus();
                    else btnAdd.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboWorkingShift.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboBillType.ReadOnly == false)
                {
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    {
                        cboBillType.EditValue = null;
                        cboBillType.Properties.Buttons[1].Visible = false;
                        cboBillType.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkingShift_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboWorkingShift.ReadOnly == false)
                {
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    {
                        cboWorkingShift.EditValue = null;
                        cboWorkingShift.Properties.Buttons[1].Visible = false;
                        cboWorkingShift.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkingShift_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboWorkingShift.EditValue != null)
                    {
                        cboWorkingShift.Properties.Buttons[1].Visible = true;
                        if (btnSave.Enabled == true)
                            btnSave.Focus();
                        else btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSymbolCode.Focus();
                    txtSymbolCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSymbolCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEInvoiceSys.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditMaxItemNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotGenOrder.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForDebt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkForOtherSale.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForOtherSale_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboWorkingShift.Focus();
                    cboWorkingShift.ShowPopup();
                    //txtTemplateCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtReleaseTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtReleaseTime.EditValue != null)
                    {
                        spinNumOrder.Focus();
                        spinNumOrder.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtReleaseTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtReleaseTime.DateTime != null)
                    {
                        spinNumOrder.Focus();
                        spinNumOrder.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtPatientTypeId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtPatientTypeId.Text.Trim()))
                    {
                        string code = txtPatientTypeId.Text.Trim();
                        var listData = Base.GlobalStore.ListPatientType.Where(o => o.PATIENT_TYPE_CODE.Contains(code)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            showCbo = false;
                            txtPatientTypeId.Text = listData.First().PATIENT_TYPE_CODE;
                            cboPatientTypeId.EditValue = listData.First().ID;
                            cboPatientTypeId.Properties.Buttons[1].Visible = true;
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboPatientTypeId.Focus();
                        cboPatientTypeId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientTypeId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboPatientTypeId.ReadOnly == false)
                {
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    {
                        cboPatientTypeId.EditValue = null;
                        cboPatientTypeId.Properties.Buttons[1].Visible = false;
                        txtPatientTypeId.Text = "";
                        txtPatientTypeId.Focus();
                        txtPatientTypeId.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientTypeId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboPatientTypeId.EditValue != null)
                    {
                        var data = Base.GlobalStore.ListPatientType.FirstOrDefault(o => o.ID == (long)cboPatientTypeId.EditValue);
                        if (data != null)
                        {
                            txtPatientTypeId.Text = data.PATIENT_TYPE_CODE;
                            cboPatientTypeId.Properties.Buttons[1].Visible = true;
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientTypeId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientTypeId.EditValue != null)
                    {
                        var data = Base.GlobalStore.ListPatientType.FirstOrDefault(o => o.ID == (long)cboPatientTypeId.EditValue);
                        if (data != null)
                        {
                            txtPatientTypeId.Text = data.PATIENT_TYPE_CODE;
                            cboPatientTypeId.Properties.Buttons[1].Visible = true;
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                }
                if (e.KeyCode == Keys.Down)
                {
                    cboPatientTypeId.ShowPopup();
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
                //if (e.KeyCode == Keys.Enter)
                //{
                //    chkForDeposit.Focus();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkForDeposit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkForBill.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkForBill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkForRepay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void chkForRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            btnSave.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        #endregion

        #region Event
        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkForBill_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (chkForBill.Checked)
                //{
                //    chkForDeposit.Checked = false;
                //    chkForRepay.Checked = false;
                //    chkForDebt.Checked = false;
                //    chkForOtherSale.Checked = false;

                //    chkForDeposit.ReadOnly = true;
                //    chkForRepay.ReadOnly = true;
                //    chkForDebt.ReadOnly = true;
                //    chkForOtherSale.ReadOnly = true;

                //}
                //if (chkForBill.Checked == false)
                //{
                //    chkForDeposit.ReadOnly = false;
                //    chkForRepay.ReadOnly = false;
                //    chkForDebt.ReadOnly = false;
                //    chkForOtherSale.ReadOnly = false;

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkForDeposit_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                //if (chkForDeposit.Checked)
                //{
                //    chkForBill.Checked = false;
                //    chkForDebt.Checked = false;
                //    chkForOtherSale.Checked = false;

                //    chkForBill.ReadOnly = true;
                //    chkForDebt.ReadOnly = true;
                //    chkForOtherSale.ReadOnly = true;

                //}
                //if (chkForDeposit.Checked == false)
                //{
                //    chkForBill.ReadOnly = false;
                //    chkForDebt.ReadOnly = false;
                //    chkForOtherSale.ReadOnly = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void chkForRepay_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                //if (chkForRepay.Checked)
                //{
                //    chkForBill.Checked = false;
                //    chkForDebt.Checked = false;
                //    chkForOtherSale.Checked = false;

                //    chkForBill.ReadOnly = true;
                //    chkForDebt.ReadOnly = true;
                //    chkForOtherSale.ReadOnly = true;
                //}
                //if (chkForRepay.Checked == false)
                //{
                //    chkForBill.ReadOnly = false;
                //    chkForDebt.ReadOnly = false;
                //    chkForOtherSale.ReadOnly = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void chkForDebt_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                //if (chkForDebt.Checked)
                //{
                //    chkForBill.Checked = false;
                //    chkForRepay.Checked = false;
                //    chkForDeposit.Checked = false;
                //    chkForOtherSale.Checked = false;

                //    chkForBill.ReadOnly = true;
                //    chkForRepay.ReadOnly = true;
                //    chkForDeposit.ReadOnly = true;
                //    chkForOtherSale.ReadOnly = true;


                //}
                //if (chkForDebt.Checked == false)
                //{
                //    chkForBill.ReadOnly = false;
                //    chkForRepay.ReadOnly = false;
                //    chkForDeposit.ReadOnly = false;
                //    chkForOtherSale.ReadOnly = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForOtherSale_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (chkForOtherSale.Checked)
            //    {
            //        chkForBill.Checked = false;
            //        chkForRepay.Checked = false;
            //        chkForDeposit.Checked = false;
            //        chkForDebt.Checked = false;

            //        chkForBill.ReadOnly = true;
            //        chkForRepay.ReadOnly = true;
            //        chkForDeposit.ReadOnly = true;
            //        chkForDebt.ReadOnly = true;
            //    }

            //    if (chkForOtherSale.Checked == false)
            //    {
            //        chkForBill.ReadOnly = false;
            //        chkForRepay.ReadOnly = false;
            //        chkForDeposit.ReadOnly = false;
            //        chkForDebt.ReadOnly = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemButtonDestroy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK row = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                    if (row != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK data = new MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>(data, row);
                        var apiresult = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_ACCOUNT_BOOK_DELETE, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGridAccountBookList();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void repositoryItemButtonReportDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var accountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                if (accountBook != null)
                {
                    FormReportTime form = new FormReportTime(accountBook);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var accountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();

                List<object> listArgs = new List<object>();
                listArgs.Add(accountBook);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
