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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Print;
using HIS.UC.ListDepositRequest;
using HIS.UC.ListDepositRequest.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class frmDepositRequest : Form
    {
        ListDepositRequestProcessor listDepositReqProcessor = null;
        UserControl ucRequestDeposit = null;
        MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ adepositreq;
        List<V_HIS_DEPOSIT_REQ> listDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        List<V_HIS_DEPOSIT_REQ> currentlistDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        V_HIS_DEPOSIT_REQ depositReq = new V_HIS_DEPOSIT_REQ();
        V_HIS_DEPOSIT_REQ currentdepositReq = new V_HIS_DEPOSIT_REQ();
        ListDepositRequestInitADO listDepositRequestADO = new ListDepositRequestInitADO();
        internal MOS.EFMODEL.DataModels.V_HIS_DEPOSIT HisDeposit { get; set; }
        private V_HIS_DEPOSIT_REQ depositReqPrint { get; set; }
        int positionHandleLeft = -1;
        internal int action = -1;
        SendResultToOtherForm sendResultToOtherForm;
        bool isPrintNow = false;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentID;
        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;

        int positionHandleControl = -1;
        public frmDepositRequest()
        {
            InitializeComponent();
            //initUCDepositRequestPanel();
        }

        public frmDepositRequest(Inventec.Desktop.Common.Modules.Module module, List<V_HIS_DEPOSIT_REQ> treatmentID)
        {
            InitializeComponent();
            try
            {
                InitListDepositReqGrid();
                this.currentModule = module;
                roomId = module.RoomId;
                roomTypeId = module.RoomTypeId;
                this.currentlistDepositReq = treatmentID;
                FillDataToGrid();
                ValidControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void frmDepositRequest_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();
                CommonParam param = new CommonParam();
                positionHandle = -1;
                //InitMenuToButtonPrint();
                //if (HisTreatment != null)
                //{
                //    FillDataToGrid();
                //}
                loadPayForm();
                loadAccountBook();
                InitComboAccountBook();
                InitComboPayForm();
                SetDefaultAccountBookForUser();
                SetDefaultPayFormForUser();
                //SetDefaultCashierRoom();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            ////initUCDepositRequestPanel();
            //InitListDepositReqGrid();
            //loadPayForm();
            //loadAccountBook();
            //InitComboAccountBook();
            //InitComboPayForm();
            ////getDataDepositReq(this.treatmentID);

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //FillDataToGrid();
                Inventec.Desktop.Common.Message.WaitingManager.Show();

                if (!String.IsNullOrEmpty(txtReqCode.Text))
                {
                    if (checkDigit(txtReqCode.Text))

                        FillDataToGrid();
                    else
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.ExamReqEdit_MaYeuCauHoacMaDieuTriKhongHopLe));
                }
                else
                {
                    FillDataToGrid();
                }
                SetDefaultAccountBookForUser();
                SetDefaultPayFormForUser();
                Grid_RowCellClick(currentdepositReq);
                //else
                //{
                //    lblStatus.Text = "";
                //    cboExamServiceType.EditValue = "";
                //    cboExecuteRoom.EditValue = "";
                //}
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleLeft == -1)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleLeft > edit.TabIndex)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadAccountBookCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK accountBook = ListAccountBook.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + (int)(accountBook.CURRENT_NUM_ORDER ?? 0);
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    loadPayFormCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayForm.EditValue != null && cboPayForm.EditValue != cboPayForm.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM commune = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtPayFormCode.Text = commune.PAY_FORM_CODE;
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void txtReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void Grid_PrintClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                //depositReqPrint = data;
                if (data != null)
                {
                    depositReqPrint = data;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess(true);
                Grid_PrintClick(currentdepositReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //btnPrintRP(depositReqPrint);
                Grid_PrintClick(currentdepositReq);
                //SaveProcess(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        void Regcode(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F2)
                {
                    txtReqCode.Focus();
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
                    btnSearch.Focus();
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
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtReqCode_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtReqCode_Click(object sender, EventArgs e)
        {
            txtReqCode.Focus();
        }
    }
}
