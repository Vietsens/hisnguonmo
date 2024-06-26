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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServiceChangeReqList.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceChangeReqList.ApprovalCashier
{
    public partial class FormApprovalCashier : FormBase
    {
        #region declare
        private V_HIS_SERVICE_CHANGE_REQ serviceChange;
        private HIS_SERE_SERV SereServToRepay;
        private HIS_SERE_SERV SereServToDeposit;
        private HIS_SERE_SERV_DEPOSIT SereServDeposit;
        private Action RefreshData;
        private List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        private HIS_CASHIER_ROOM currCashierRoom;
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private int positionHandle = -1;
        private HisServiceChangeReqCashierApproveResultSDO ResultChange;
        private bool isPrintNow;
        private string CurrentLoginName;
        V_HIS_ACCOUNT_BOOK accountBookRepay;
        V_HIS_ACCOUNT_BOOK accountBookDeposit;
        #endregion

        #region ctor
        public FormApprovalCashier(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERVICE_CHANGE_REQ data, Action refreshData)
            : base(module)
        {
            InitializeComponent();
            this.ModuleData = module;
            this.serviceChange = data;
            this.RefreshData = refreshData;
        }
        #endregion

        #region load
        private void FormApprovalCashier_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                currCashierRoom = BackendDataWorker.Get<HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.ModuleData.RoomId);
                CurrentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                InitDataCombo();
                InitMenuToButtonPrint();
                LoadDataSereServ();

                LoadDefaultDataControl();

                ValidateControl();

                if (SereServDeposit == null)
                {
                    DisableRepay();
                }

                dtTransactionTime.Focus();
                dtTransactionTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisableRepay()
        {
            try
            {
                cboRepayAccountBook.EditValue = null;
                cboRepayAccountBook.Enabled = false;
                cboRepayPayForm.EditValue = null;
                cboRepayPayForm.Enabled = false;
                spRepayNumOrder.EditValue = null;
                spRepayNumOrder.Enabled = false;
                cboReason.EditValue = null;
                cboReason.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ValidateControl
        private void ValidateControl()
        {
            try
            {
                ValidControl(this.dtTransactionTime);
                ValidControl(this.cboDepositPayForm);
                ValidControl(this.cboDepositAccountBook);

                if (SereServDeposit != null)
                {
                    ValidControl(this.cboRepayAccountBook);
                    ValidControl(this.cboRepayPayForm);
                    ValidControl(this.cboReason);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl(Control editor)
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = editor;
                validate.ErrorText = ResourceLanguageManager.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(editor, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void LoadDefaultDataControl()
        {
            try
            {
                dtTransactionTime.EditValue = DateTime.Now;

                decimal canthu = 0;

                if (SereServToRepay != null)
                {
                    lblOldService.Text = SereServToRepay.TDL_SERVICE_NAME;
                    lblOldPrice.Text = (SereServToRepay.VIR_PRICE ?? 0) + "";
                    lblOldAmount.Text = SereServToRepay.AMOUNT + "";
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.SereServToRepay.PATIENT_TYPE_ID);
                    lblOldPatientType.Text = patientType != null ? patientType.PATIENT_TYPE_NAME : "";
                    if (SereServDeposit != null)
                    {
                        txtRepayAmount.Text = SereServDeposit.AMOUNT + "";
                        txtRepayAmount.Tag = SereServDeposit.AMOUNT;
                        canthu -= SereServDeposit.AMOUNT;
                        cboReason.EditValue = IMSys.DbConfig.HIS_RS.HIS_REPAY_REASON.ID__KHONG_TH;
                        cboRepayPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                    }
                }

                if (SereServToDeposit != null)
                {
                    lblNewService.Text = SereServToDeposit.TDL_SERVICE_NAME;
                    lblNewPrice.Text = (SereServToDeposit.VIR_PRICE ?? 0) + "";
                    lblNewAmount.Text = SereServToDeposit.AMOUNT + "";
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.SereServToDeposit.PATIENT_TYPE_ID);
                    lblNewPatientType.Text = patientType != null ? patientType.PATIENT_TYPE_NAME : "";
                    txtDepositAmount.Text = (SereServToDeposit.VIR_TOTAL_PATIENT_PRICE ?? 0) + "";
                    txtDepositAmount.Tag = SereServToDeposit.VIR_TOTAL_PATIENT_PRICE ?? 0;
                    canthu += (SereServToDeposit.VIR_TOTAL_PATIENT_PRICE ?? 0);
                }

                cboDepositPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                spDepositCkAmount.EditValue = null;

                List<V_HIS_ACCOUNT_BOOK> defaultAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = GlobalVariables.LastAccountBook.Where(o => ListAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        defaultAccountBook.AddRange(lstBook);
                    }
                }

                if (SereServDeposit != null)
                {
                    accountBookRepay = defaultAccountBook.FirstOrDefault(o => o.IS_FOR_REPAY == 1) ?? ListAccountBook.FirstOrDefault(o => o.IS_FOR_REPAY == 1) ?? new V_HIS_ACCOUNT_BOOK();
                    cboRepayAccountBook.EditValue = accountBookRepay.ID;
                }

                accountBookDeposit = defaultAccountBook.FirstOrDefault(o => o.IS_FOR_DEPOSIT == 1) ?? ListAccountBook.FirstOrDefault(o => o.IS_FOR_DEPOSIT == 1) ?? new V_HIS_ACCOUNT_BOOK();
                cboDepositAccountBook.EditValue = accountBookDeposit.ID;

                if (canthu > 0)
                {
                    lblChenhLech.Text = canthu + "";
                    lciChenhLech.Text = "Cần thu thêm:";
                }
                else
                {
                    lblChenhLech.Text = -canthu + "";
                    lciChenhLech.Text = "Cần trả lại:";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCombo()
        {
            try
            {
                var listPayForm = BackendDataWorker.Get<HIS_PAY_FORM>();
                LoadDataToCbo(cboDepositPayForm, listPayForm, "PAY_FORM_CODE", "PAY_FORM_NAME", "ID");
                LoadDataToCbo(cboRepayPayForm, listPayForm, "PAY_FORM_CODE", "PAY_FORM_NAME", "ID");

                var listReason = BackendDataWorker.Get<HIS_REPAY_REASON>();
                LoadDataToCbo(cboReason, listReason, "REPAY_REASON_CODE", "REPAY_REASON_NAME", "ID");

                LoadDataAccountBook();
                var repayAcc = ListAccountBook.Where(o => o.IS_FOR_REPAY == 1).ToList();
                var depositAcc = ListAccountBook.Where(o => o.IS_FOR_DEPOSIT == 1).ToList();
                LoadDataToCbo(cboDepositAccountBook, depositAcc, "ACCOUNT_BOOK_CODE", "ACCOUNT_BOOK_NAME", "ID");
                LoadDataToCbo(cboRepayAccountBook, repayAcc, "ACCOUNT_BOOK_CODE", "ACCOUNT_BOOK_NAME", "ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataAccountBook()
        {
            try
            {
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.LOGINNAME = CurrentLoginName;//Kiểm tra sổ còn hay k
                acFilter.FOR_REPAY = true;
                acFilter.FOR_DEPOSIT = true;
                acFilter.IS_OUT_OF_BILL = false;
                if (currCashierRoom != null)
                {
                    acFilter.CASHIER_ROOM_ID = this.currCashierRoom.ID;//Kiểm tra sổ còn hay k
                }

                var accountBook = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumer.ApiConsumers.MosConsumer, acFilter, null);
                if (accountBook != null && accountBook.Count > 0)
                {
                    accountBook = accountBook.Where(o => o.WORKING_SHIFT_ID == null || o.WORKING_SHIFT_ID == (HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId ?? 0)).ToList();
                    this.ListAccountBook.AddRange(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCbo(GridLookUpEdit cbo, object data, string code, string name, string value)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(code, "", 150, 1));
                columnInfos.Add(new ColumnInfo(name, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, value, columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServ()
        {
            try
            {
                if (serviceChange != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter filter = new HisSereServFilter();
                    filter.IDs = new List<long> { serviceChange.SERE_SERV_ID };
                    if (serviceChange.ALTER_SERE_SERV_ID.HasValue)
                    {
                        filter.IDs.Add(serviceChange.ALTER_SERE_SERV_ID.Value);
                    }

                    var sereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                    if (sereServs != null && sereServs.Count > 0)
                    {
                        foreach (var item in sereServs)
                        {
                            if (item.ID == serviceChange.SERE_SERV_ID)
                            {
                                SereServToRepay = item;
                            }
                            else if (item.ID == serviceChange.ALTER_SERE_SERV_ID)
                            {
                                SereServToDeposit = item;
                            }
                        }
                    }

                    if (SereServToRepay != null)
                    {
                        HisSereServDepositFilter depFilter = new HisSereServDepositFilter();
                        depFilter.SERE_SERV_ID = SereServToRepay.ID;
                        var sereServDep = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, depFilter, param);
                        if (sereServDep != null && sereServDep.Count > 0)
                        {
                            SereServDeposit = sereServDep.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveNPrint.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.btnSaveNPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepositPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("FormApprovalCashier.cboDepositPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepositAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("FormApprovalCashier.cboDepositAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboReason.Properties.NullText = Inventec.Common.Resource.Get.Value("FormApprovalCashier.cboReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRepayPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("FormApprovalCashier.cboRepayPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRepayAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("FormApprovalCashier.cboRepayAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciTransactionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOldService.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciOldService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOldPatientType.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciOldPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOldPrice.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciOldPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOldAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciOldAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayAccountBook.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciRepayAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewService.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciNewService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciRepayAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayPayForm.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciRepayPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReason.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayNumOrder.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciRepayNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayCode.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciRepayCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewPatientType.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciNewPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewPrice.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciNewPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciNewAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositAccountBook.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositPayForm.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositNumOrder.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositCkAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositCkAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepositCode.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChenhLech.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciChenhLech.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Enabled && btnSave.Visible)
                {
                    SaveProcess(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    btnPrint.ShowDropDown();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveNPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSaveNPrint.Enabled)
                {
                    SaveProcess(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRepayAccountBook.Focus();
                    cboRepayAccountBook.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboRepayAccountBook.Focus();
                    cboRepayAccountBook.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboRepayPayForm.Focus();
                    cboRepayPayForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spRepayNumOrder.EditValue = null;
                spRepayNumOrder.Enabled = false;
                accountBookRepay = null;
                if (cboRepayAccountBook.EditValue != null)
                {
                    accountBookRepay = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue));
                    if (accountBookRepay != null)
                    {
                        //nếu chọn sổ tạm ứng trước và chọn sổ hoàn ứng sau
                        //sổ tạm ứng và hoàn ứng cùng 1 sổ thì số hóa đơn của sổ hoàn ứng luôn nhỏ hơn số của sổ tạm ứng
                        decimal numOrder = setDataToDicNumOrderInAccountBook(accountBookRepay);

                        spRepayNumOrder.EditValue = numOrder;
                        if (accountBookRepay.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spRepayNumOrder.Enabled = true;
                        }

                        if (accountBookRepay.ID == (accountBookDeposit != null ? accountBookDeposit.ID : 0))
                        {
                            numOrder += 1;
                            spDepositNumOrder.EditValue = numOrder;
                            if (accountBookDeposit.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                            {
                                spDepositNumOrder.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRepayPayForm.Focus();
                    cboRepayPayForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (spRepayNumOrder.Enabled)
                    {
                        spRepayNumOrder.Focus();
                        spRepayNumOrder.SelectAll();
                    }
                    else
                    {
                        cboReason.Focus();
                        cboReason.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spRepayNumOrder.Enabled)
                    {
                        spRepayNumOrder.Focus();
                        spRepayNumOrder.SelectAll();
                    }
                    else
                    {
                        cboReason.Focus();
                        cboReason.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReason_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReason_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboDepositAccountBook.Focus();
                    cboDepositAccountBook.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepositAccountBook.Focus();
                    cboDepositAccountBook.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboDepositPayForm.Focus();
                    cboDepositPayForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spDepositNumOrder.EditValue = null;
                spDepositNumOrder.Enabled = false;
                accountBookDeposit = null;
                if (cboDepositAccountBook.EditValue != null)
                {
                    accountBookDeposit = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositAccountBook.EditValue));
                    if (accountBookDeposit != null)
                    {
                        //nếu chọn sổ hoàn ứng trước và chọn sổ tạm ứng sau
                        //sổ tạm ứng và hoàn ứng cùng 1 sổ thì tăng số của sổ lên 1
                        decimal numOrder = setDataToDicNumOrderInAccountBook(accountBookDeposit);
                        if (accountBookDeposit.ID == (accountBookRepay != null ? accountBookRepay.ID : 0))
                        {
                            numOrder += 1;
                        }

                        spDepositNumOrder.EditValue = numOrder;
                        if (accountBookDeposit.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spDepositNumOrder.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepositPayForm.Focus();
                    cboDepositPayForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (spDepositNumOrder.Enabled)
                    {
                        spDepositNumOrder.Focus();
                        spDepositNumOrder.SelectAll();
                    }
                    else if (spDepositCkAmount.Enabled)
                    {
                        spDepositCkAmount.Focus();
                        spDepositCkAmount.SelectAll();
                    }
                    else
                    {
                        btnSave.Focus();
                    }

                    HIS_PAY_FORM payForm = null;
                    if (cboDepositPayForm.EditValue != null && cboDepositPayForm.EditValue != cboDepositPayForm.OldEditValue)
                    {
                        payForm = BackendDataWorker.Get<HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositPayForm.EditValue.ToString()));
                    }
                    CheckPayFormTienMatChuyenKhoan(payForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositPayForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spDepositNumOrder.Enabled)
                    {
                        spDepositNumOrder.Focus();
                        spDepositNumOrder.SelectAll();
                    }
                    else if (spDepositCkAmount.Enabled)
                    {
                        spDepositCkAmount.Focus();
                        spDepositCkAmount.SelectAll();
                    }
                    else
                    {
                        btnSave.Focus();
                        e.Handled = true;
                    }

                    HIS_PAY_FORM payForm = null;
                    if (cboDepositPayForm.EditValue != null && cboDepositPayForm.EditValue != cboDepositPayForm.OldEditValue)
                    {
                        payForm = BackendDataWorker.Get<HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositPayForm.EditValue.ToString()));
                    }

                    CheckPayFormTienMatChuyenKhoan(payForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDepositNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spDepositCkAmount.Enabled)
                    {
                        spDepositCkAmount.Focus();
                        spDepositCkAmount.SelectAll();
                    }
                    else
                    {
                        btnSave.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDepositCkAmount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveNPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Process
        private void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                dxValidationProvider1.RemoveControlError(spDepositCkAmount);
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    ValidControl(this.spDepositCkAmount);

                    this.lciDepositCkAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciDepositCkAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciDepositCkAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDepositCkAmount.Enabled = true;
                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    ValidControl(this.spDepositCkAmount);

                    this.lciDepositCkAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositQtAmount.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciDepositCkAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositQtAmount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciDepositCkAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDepositCkAmount.Enabled = true;
                }
                else
                {
                    dxValidationProvider1.SetValidationRule(spDepositCkAmount, null);
                    this.lciDepositCkAmount.Text = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciDepositCkAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormApprovalCashier.lciDepositCkAmount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciDepositCkAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciDepositCkAmount.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess(bool isSaveAndPrint)
        {
            try
            {
                SetEnableButtonSave(false);
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                HisServiceChangeReqCashierApproveSDO sdo = new HisServiceChangeReqCashierApproveSDO();

                UpdateDataFormToSDO(sdo);

                WaitingManager.Show();
                if (CheckValidForSave(param, sdo))
                {
                    CARD.WCF.DCO.WcfRefundDCO repayDCO = null;
                    // thanh toán qua thẻ 
                    var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositPayForm.EditValue));
                    if (payForm == null)
                    {
                        WaitingManager.Hide();
                        return;
                    }

                    // nếu hình thức thanh toán qua thẻ thì gọi WCF tab thẻ (POS)
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        HisTransactionDepositSDO hisTransactionDepositSDO = new HisTransactionDepositSDO();
                        ProcessDataCheckTransaction(hisTransactionDepositSDO);
                        //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                        var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTransaction/CheckDeposit", ApiConsumers.MosConsumer, hisTransactionDepositSDO, param);

                        if (!check)
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                            SessionManager.ProcessTokenLost(param);
                            return;
                        }

                        CARD.WCF.DCO.WcfRefundDCO depositDCO = new CARD.WCF.DCO.WcfRefundDCO();
                        depositDCO.Amount = Convert.ToInt64(sdo.DepositTransferAmount);
                        //DepositDCO.PinCode = this.txtPin.Text.Trim();
                        repayDCO = RepayCard(ref depositDCO);
                        // nếu gọi sang POS trả về false thì kết thúc
                        if (repayDCO == null || (repayDCO.ResultCode == null || !repayDCO.ResultCode.Equals("00")))
                        {
                            success = false;
                            Config.MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                            Inventec.Common.Logging.LogSystem.Info("Output repayDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => repayDCO), repayDCO));

                            //param.Messages.Add(ResourceMessageLang.HoanUngQuaTheThatBai);
                            if (repayDCO != null
                           && !String.IsNullOrWhiteSpace(repayDCO.ResultCode)
                           && mappingErrorTHE.dicMapping != null
                           && mappingErrorTHE.dicMapping.Count > 0
                           && mappingErrorTHE.dicMapping.ContainsKey(repayDCO.ResultCode))
                            {
                                param.Messages.Add(mappingErrorTHE.dicMapping[repayDCO.ResultCode]);
                            }
                            else if (repayDCO != null && String.IsNullOrWhiteSpace(repayDCO.ResultCode))
                            {
                                param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                            }
                            else if (repayDCO != null
                                && !String.IsNullOrWhiteSpace(repayDCO.ResultCode)
                                && mappingErrorTHE.dicMapping != null
                                && mappingErrorTHE.dicMapping.Count > 0
                                && !mappingErrorTHE.dicMapping.ContainsKey(repayDCO.ResultCode)
                                )
                            {
                                param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                            return;
                        }

                        //sdo.TigTransactionCode = repayDCO.TransactionCode;
                        //sdo.TigTransactionTime = repayDCO.TransactionTime;
                    }

                    this.ResultChange = new BackendAdapter(param).Post<HisServiceChangeReqCashierApproveResultSDO>("api/HisServiceChangeReq/CashierApprove", ApiConsumers.MosConsumer, sdo, param);
                    if (this.ResultChange != null)
                    {
                        success = true;
                        btnPrint.Enabled = true;

                        AddLastAccountToLocal();
                        FillDataToControl(this.ResultChange);

                        if (RefreshData != null)
                        {
                            RefreshData();
                        }

                        if (this.ResultChange.Repay != null)
                        {
                            var repayAcc = ListAccountBook.FirstOrDefault(o => o.ID == this.ResultChange.Repay.ACCOUNT_BOOK_ID);
                            UpdateDictionaryNumOrderAccountBook(repayAcc, this.ResultChange.Repay.NUM_ORDER);
                        }

                        if (this.ResultChange.Deposit != null)
                        {
                            var depositAcc = ListAccountBook.FirstOrDefault(o => o.ID == this.ResultChange.Deposit.ACCOUNT_BOOK_ID);
                            UpdateDictionaryNumOrderAccountBook(depositAcc, this.ResultChange.Deposit.NUM_ORDER);
                        }

                        if (isSaveAndPrint)
                        {
                            ProcessPrintData(isSaveAndPrint);
                        }
                    }
                    else
                    {
                        SetEnableButtonSave(true);
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                else
                {
                    SetEnableButtonSave(true);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private bool CheckValidForSave(CommonParam param, HisServiceChangeReqCashierApproveSDO sdo)
        {
            bool result = true;
            try
            {
                if (sdo != null)
                {
                    if (spDepositCkAmount.EditValue != null && spDepositCkAmount.Value > sdo.DepositAmount)
                    {
                        if (sdo.DepositPayFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                        {
                            param.Messages.Add(String.Format(ResourceLanguageManager.SoTienChuyenKhoanLonHonSoTienTamUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spDepositCkAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(sdo.DepositAmount, ConfigApplications.NumberSeperator)));
                        }
                        else if (sdo.DepositPayFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                        {
                            param.Messages.Add(String.Format(ResourceLanguageManager.SoTienQuetTheLonHonSoTienTamUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spDepositCkAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(sdo.DepositAmount, ConfigApplications.NumberSeperator)));
                        }

                        result = false;
                    }

                    if (!result)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(param, false);
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

        private void FillDataToControl(HisServiceChangeReqCashierApproveResultSDO data)
        {
            try
            {
                txtDepositCode.Text = "";
                txtRepayCode.Text = "";
                spDepositNumOrder.EditValue = null;
                spRepayNumOrder.EditValue = null;

                if (data != null)
                {
                    if (data.Deposit != null)
                    {
                        spDepositNumOrder.EditValue = data.Deposit.NUM_ORDER;
                        txtDepositCode.Text = data.Deposit.TRANSACTION_CODE;
                    }

                    if (data.Repay != null)
                    {
                        spRepayNumOrder.EditValue = data.Repay.NUM_ORDER;
                        txtRepayCode.Text = data.Repay.TRANSACTION_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_DEPOSIT == accountBook.IS_FOR_DEPOSIT && o.ID != accountBook.ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }

                var repayAccountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue));
                if (repayAccountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == repayAccountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_DEPOSIT == repayAccountBook.IS_FOR_DEPOSIT && o.ID != repayAccountBook.ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(repayAccountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintData(bool isSaveAndPrint)
        {
            try
            {
                this.isPrintNow = isSaveAndPrint;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000110", DelegateRunPrinter);
                richEditorMain.RunPrintTemplate("Mps000102", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000102":
                        InPhieuTamUng(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000110":
                        InPhieuHoanUng(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (ResultChange == null || ResultChange.Deposit == null || ResultChange.SereServDeposit == null)
                {
                    result = false;
                    return;
                }

                CommonParam param = new CommonParam();
                List<HIS_SERE_SERV_DEPOSIT> dereDetails = new List<HIS_SERE_SERV_DEPOSIT>();
                dereDetails.Add(ResultChange.SereServDeposit);

                V_HIS_TREATMENT_FEE currentHisTreatment = new V_HIS_TREATMENT_FEE();
                HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                feeFilter.ID = SereServToDeposit.TDL_TREATMENT_ID;
                var histreatmentFee = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, param);
                if (histreatmentFee != null && histreatmentFee.Count > 0)
                {
                    currentHisTreatment = histreatmentFee.FirstOrDefault();
                }

                V_HIS_TRANSACTION currentHisTransaction = new V_HIS_TRANSACTION();
                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.ID = ResultChange.Deposit.ID;
                var histran = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                if (histran != null && histran.Count > 0)
                {
                    currentHisTransaction = histran.FirstOrDefault();
                }

                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.ID = ResultChange.SereServDeposit.SERE_SERV_ID;
                var SereServAlls = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, param);
                foreach (var item in SereServAlls)
                {
                    item.VIR_TOTAL_PATIENT_PRICE = ResultChange.SereServDeposit.AMOUNT;
                }

                MPS.Processor.Mps000102.PDO.PatientADO patient = new MPS.Processor.Mps000102.PDO.PatientADO();
                MOS.Filter.HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                patientViewFilter.ID = SereServToDeposit.TDL_PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = new MPS.Processor.Mps000102.PDO.PatientADO(patients.FirstOrDefault());
                }

                //Thông tin bảo hiểm y tế
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                patientTypeFilter.TREATMENT_ID = SereServToDeposit.TDL_TREATMENT_ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, param);
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = patientTypeAlters != null && patientTypeAlters.Count > 0 ? patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault() : null;

                List<V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetHospitalInOut", ApiConsumers.MosConsumer, SereServToDeposit.TDL_TREATMENT_ID, param);

                long? totalDay = null;
                if (currentHisTreatment.TDL_PATIENT_TYPE_ID == Config.HisConfig.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(currentHisTreatment.IN_TIME, currentHisTreatment.OUT_TIME, currentHisTreatment.TREATMENT_END_TYPE_ID, currentHisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(currentHisTreatment.IN_TIME, currentHisTreatment.OUT_TIME, currentHisTreatment.TREATMENT_END_TYPE_ID, currentHisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }

                string departmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentName();
                var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                var sereServHitechs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                var sereServVTTTs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                var sereServNotHitechs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();
                //Cộng các sereServ trong gói vào dv ktc
                foreach (var sereServHitech in sereServHitechADOs)
                {
                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    var sereServVTTTInKtcs = SereServAlls.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                    sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                    sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                    sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                    decimal totalHeinPrice = 0;
                    foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                    {
                        totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                    }
                    sereServHitech.PRICE_BHYT += totalHeinPrice;
                    //sereServHitech.PRICE_BHYT = sereServHitech.PRICE_BHYT * sereServHitech.AMOUNT + sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                    sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                }

                //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                //
                var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                foreach (var sereServVTTTADO in sereServVTTTADOs)
                {
                    var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                    if (sereServADODelete.Count == 0)
                    {
                        sereServDeleteADOs.Add(sereServVTTTADO);
                    }
                }

                foreach (var sereServDelete in sereServDeleteADOs)
                {
                    sereServVTTTADOs.Remove(sereServDelete);
                }

                var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);
                WaitingManager.Hide();

                // tinh muc huong
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.LEVEL_CODE, patientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                Inventec.Common.Logging.LogSystem.Debug("------- KAKA du lieu sereServNotHitechADOs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServNotHitechADOs), sereServNotHitechADOs));
                Inventec.Common.Logging.LogSystem.Debug("------- KAKA du lieu sereServHitechADOs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServHitechADOs), sereServHitechADOs));
                Inventec.Common.Logging.LogSystem.Debug("------- KAKA du lieu sereServVTTTADOs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServVTTTADOs), sereServVTTTADOs));
                Inventec.Common.Logging.LogSystem.Debug("------- KAKA du lieu dereDetails " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dereDetails), dereDetails));
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentHisTreatment != null ? currentHisTreatment.TREATMENT_CODE : ""), printTypeCode, ModuleData.RoomId);

                MPS.Processor.Mps000102.PDO.Mps000102PDO pdo = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                        patient,
                        patientTypeAlter,
                        departmentName,
                        sereServNotHitechADOs,
                        sereServHitechADOs,
                        sereServVTTTADOs,
                        departmentTrans,
                        currentHisTreatment,
                        BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                        currentHisTransaction,
                        dereDetails,
                        totalDay,
                        ratio_text,
                        null
                        );

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                string printer = "";

                if (GlobalVariables.dicPrinter != null && GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printer = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (isPrintNow)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printer) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printer, 1, false, true) { EmrInputADO = inputADO };
                }

                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", ModuleData.RoomId, ModuleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV_12> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();

            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();

                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != Config.HisConfig.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private void InPhieuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (ResultChange == null || ResultChange.Repay == null || ResultChange.SeseDepoRepay == null)
                {
                    result = false;
                    return;
                }

                CommonParam param = new CommonParam();
                WaitingManager.Show();

                V_HIS_TREATMENT_1 HisTreatment = new V_HIS_TREATMENT_1();
                HisTreatmentView1Filter feeFilter = new HisTreatmentView1Filter();
                feeFilter.ID = SereServToDeposit.TDL_TREATMENT_ID;
                var histreatmentFee = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, feeFilter, param);
                if (histreatmentFee != null && histreatmentFee.Count > 0)
                {
                    HisTreatment = histreatmentFee.FirstOrDefault();
                }

                V_HIS_TRANSACTION hisTransaction = new V_HIS_TRANSACTION();
                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.ID = ResultChange.Repay.ID;
                var histran = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                if (histran != null && histran.Count > 0)
                {
                    hisTransaction = histran.FirstOrDefault();
                }

                MPS.Processor.Mps000110.PDO.PatientADO mpsPatientAdo = new MPS.Processor.Mps000110.PDO.PatientADO();
                MOS.Filter.HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                patientViewFilter.ID = SereServToDeposit.TDL_PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    mpsPatientAdo = new MPS.Processor.Mps000110.PDO.PatientADO(patients.FirstOrDefault());
                }

                var dereDetails = new List<HIS_SESE_DEPO_REPAY> { ResultChange.SeseDepoRepay };

                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetHospitalInOut", ApiConsumers.MosConsumer, HisTreatment.ID, param);

                long? totalDay = null;

                if (HisTreatment.TDL_PATIENT_TYPE_ID == Config.HisConfig.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(HisTreatment.IN_TIME, HisTreatment.OUT_TIME, HisTreatment.TREATMENT_END_TYPE_ID, HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(HisTreatment.IN_TIME, HisTreatment.OUT_TIME, HisTreatment.TREATMENT_END_TYPE_ID, HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }
                string departmentName = "";
                // get patient_type_alter 
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterViewFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterViewFilter.TREATMENT_ID = HisTreatment.ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterViewFilter, param);
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = hisTransaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                MPS.Processor.Mps000110.PDO.Mps000110PDO pdo = new MPS.Processor.Mps000110.PDO.Mps000110PDO(
                   mpsPatientAdo,
                   patientTypeAlter,
                   departmentName,
                   dereDetails,
                   departmentTrans,
                   HisTreatment,
                   hisTransaction,
                   totalDay,
                   departmentTran
                   );

                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((HisTreatment != null ? HisTreatment.TREATMENT_CODE : ""), printTypeCode, ModuleData != null ? ModuleData.RoomId : 0);

                if (isPrintNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menuMedicine = new DXPopupMenu();
                //TODO da ngon ngu
                DXMenuItem itemPhieuTamUng = new DXMenuItem("In phiếu tạm ứng theo dịch vụ", new EventHandler(OnClickInPhieu));
                itemPhieuTamUng.Tag = "Mps000102";
                menuMedicine.Items.Add(itemPhieuTamUng);

                DXMenuItem itemPhieuTamUngTheoDichVu = new DXMenuItem("In phiếu hoàn ứng theo dịch vụ", new EventHandler(OnClickInPhieu));
                itemPhieuTamUngTheoDichVu.Tag = "Mps000110";
                menuMedicine.Items.Add(itemPhieuTamUngTheoDichVu);

                btnPrint.DropDownControl = menuMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieu(object sender, EventArgs e)
        {
            try
            {
                this.isPrintNow = false;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                var btnItem = sender as DXMenuItem;
                string type = (string)(btnItem.Tag);
                switch (type)
                {
                    case "Mps000102":
                        richEditorMain.RunPrintTemplate("Mps000102", DelegateRunPrinter);
                        break;
                    case "Mps000110":
                        richEditorMain.RunPrintTemplate("Mps000110", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataCheckTransaction(HisTransactionDepositSDO transactionData)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisTransactionDepositSDO();
                }

                transactionData.Transaction = new HIS_TRANSACTION();
                transactionData.SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();

                HIS_SERE_SERV_DEPOSIT deposit = new HIS_SERE_SERV_DEPOSIT();
                deposit.SERE_SERV_ID = SereServToDeposit.ID;
                if (SereServToDeposit.PATIENT_TYPE_ID == Config.HisConfig.PatientTypeId__BHYT)
                {
                    V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServ, SereServToDeposit);
                    if (Config.HisConfig.SetDefaultDepositPrice == 1)
                    {
                        deposit.AMOUNT = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ) ?? 0;
                    }
                    else if (Config.HisConfig.SetDefaultDepositPrice == 2)
                    {
                        deposit.AMOUNT = SereServToDeposit.VIR_TOTAL_PRICE ?? 0;
                    }
                    else
                    {
                        deposit.AMOUNT = SereServToDeposit.VIR_TOTAL_PATIENT_PRICE ?? 0;
                    }
                }
                else
                {
                    deposit.AMOUNT = SereServToDeposit.VIR_TOTAL_PATIENT_PRICE ?? 0;
                }

                transactionData.SereServDeposits.Add(deposit);

                if (ModuleData != null)
                {
                    transactionData.RequestRoomId = ModuleData.RoomId;
                }

                transactionData.Transaction.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU;
                if (cboDepositAccountBook.EditValue != null)
                {
                    transactionData.Transaction.ACCOUNT_BOOK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositAccountBook.EditValue.ToString());
                    var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositAccountBook.EditValue.ToString()));
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        transactionData.Transaction.NUM_ORDER = (long)(spDepositNumOrder.Value);
                    }
                }

                transactionData.Transaction.AMOUNT = Convert.ToDecimal(txtDepositAmount.Tag);

                if (cboDepositPayForm.EditValue != null)
                {
                    transactionData.Transaction.PAY_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositPayForm.EditValue.ToString());
                }

                if (SereServToRepay != null)
                {
                    transactionData.Transaction.TREATMENT_ID = SereServToRepay.TDL_TREATMENT_ID;
                }

                if (transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spDepositCkAmount.EditValue != null)
                {
                    transactionData.Transaction.TRANSFER_AMOUNT = spDepositCkAmount.Value;
                }
                else if (transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spDepositCkAmount.EditValue != null)
                {
                    transactionData.Transaction.SWIPE_AMOUNT = spDepositCkAmount.Value;
                }

                if (currCashierRoom != null)
                {
                    transactionData.Transaction.CASHIER_ROOM_ID = currCashierRoom.ID;
                }

                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    transactionData.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // gọi sang WCF hoàn ứng qua thẻ
        CARD.WCF.DCO.WcfRefundDCO RepayCard(ref CARD.WCF.DCO.WcfRefundDCO RepayDCO)
        {
            CARD.WCF.DCO.WcfRefundDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = SereServToRepay.TDL_PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    RepayDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Refund(RepayDCO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateDataFormToSDO(HisServiceChangeReqCashierApproveSDO sdo)
        {
            try
            {
                if (cboRepayAccountBook.EditValue != null)
                {
                    var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue.ToString()));
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        sdo.RepayNumOrder = (long)(spRepayNumOrder.Value);
                    }
                    sdo.RepayAccountBookId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositAccountBook.EditValue.ToString());
                }

                if (this.txtRepayAmount.Tag != null)
                {
                    sdo.RepayAmount = Convert.ToInt64(this.txtRepayAmount.Tag);
                }

                if (cboRepayPayForm.EditValue != null)
                {
                    sdo.RepayPayFormId = (Inventec.Common.TypeConvert.Parse.ToInt64(cboRepayPayForm.EditValue.ToString()));
                }

                if (cboReason.EditValue != null)
                {
                    sdo.RepayReasonId = (Inventec.Common.TypeConvert.Parse.ToInt64(cboReason.EditValue.ToString()));
                }

                sdo.RepayTransferAmount = null;

                if (cboDepositAccountBook.EditValue != null)
                {
                    var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositAccountBook.EditValue.ToString()));
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        sdo.DepositNumOrder = (long)(spDepositNumOrder.Value);
                    }
                    sdo.DepositAccountBookId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositAccountBook.EditValue.ToString());
                }

                if (this.txtDepositAmount.Tag != null)
                {
                    sdo.DepositAmount = Convert.ToInt64(this.txtDepositAmount.Tag);
                }

                if (cboDepositPayForm.EditValue != null)
                {
                    sdo.DepositPayFormId = (Inventec.Common.TypeConvert.Parse.ToInt64(cboDepositPayForm.EditValue.ToString()));
                }

                if (spDepositCkAmount.EditValue != null)
                {
                    sdo.DepositTransferAmount = spDepositCkAmount.Value;
                }

                if (SereServToDeposit != null)
                {
                    sdo.NewSereServId = SereServToDeposit.ID;
                }

                if (SereServDeposit != null)
                {
                    sdo.SereServDepositId = SereServDeposit.ID;
                }

                sdo.ServiceChangeReqId = serviceChange.ID;

                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    sdo.TransactionTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");

                sdo.WorkingRoomId = ModuleData.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
                btnSaveNPrint.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook, decimal numOrder)
        {
            try
            {
                if (accountBook != null && GlobalVariables.dicNumOrderInAccountBook != null && GlobalVariables.dicNumOrderInAccountBook.Count > 0 && GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = numOrder;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = 1;
            try
            {
                if (accountBook != null)
                {
                    if (GlobalVariables.dicNumOrderInAccountBook == null || GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (GlobalVariables.dicNumOrderInAccountBook != null && GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new MOS.Filter.HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = (accountBookNew.CURRENT_NUM_ORDER ?? 0);
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }
                            GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                    }
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
