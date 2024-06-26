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
using AutoMapper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DrugStoreDebt.ADO;
using HIS.Desktop.Plugins.DrugStoreDebt.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.DrugStoreDebt
{
    public partial class frmDrugStoreDebt : FormBase
    {
        private int positionHandle = -1;
        private V_HIS_MEDI_STOCK mediStock;
        private List<V_HIS_CASHIER_ROOM> listCashierRoom;
        private List<V_HIS_ACCOUNT_BOOK> listAccountBook;

        private List<long> expMestIds;
        private string expMestCode;

        private List<MediMateADO> listMediMateADO;
        private HisDrugStoreDebtResultSDO resultTranDebt;

        private DelegateSelectData refreshData;

        public frmDrugStoreDebt(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        public frmDrugStoreDebt(Inventec.Desktop.Common.Modules.Module module, List<long> datas, DelegateSelectData _Refresh)
            : base(module)
        {
            InitializeComponent();
            this.expMestIds = datas;
            this.refreshData = _Refresh;
        }

        public frmDrugStoreDebt(Inventec.Desktop.Common.Modules.Module module, string mestCode, DelegateSelectData _Refresh)
            : base(module)
        {
            InitializeComponent();
            this.expMestCode = mestCode;
            this.refreshData = _Refresh;
        }

        private void frmDrugStoreDebt_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadMediStock();
                this.ValidateForm();
                this.LoadDataToComboAccountBook();
                this.LoadDataToComboCashierRoom();
                this.SetDefaultAccountBook();
                this.SetDafaultCashierRoom();
                this.ResetControlValue(true, true);
                this.GenerateDataFromInput();
                if (this.listMediMateADO != null && listMediMateADO.Count > 0)
                {
                    dtTransactionTime.Focus();
                }
                else
                {
                    txtExpMestCode.Focus();
                    txtExpMestCode.SelectAll();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(dtTransactionTime);
                ValidationSingleControl(cboAccountBook);
                ValidationMaxLength(txtDescription, 2000);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc); ;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(BaseEdit control, int length)
        {
            try
            {
                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(Resources.ResourcesMessage.VuotQuaDoDaiChoPhep, length);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediStock()
        {
            try
            {
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        lciNumOrder.Enabled = true;
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = accountBookNew.CURRENT_NUM_ORDER ?? 0;
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            spinNumOrder.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
                        }
                    }
                    else
                    {
                        lciNumOrder.Enabled = true;
                        spinNumOrder.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
                    }
                }
                else
                {
                    spinNumOrder.Value = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                    lciNumOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboCashierRoom()
        {
            try
            {
                long branchId;
                branchId = WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId;
                var userRoomIds = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                    && o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN).Select(s => s.ROOM_ID).ToList();

                this.listCashierRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                this.listCashierRoom = this.listCashierRoom.Where(o => userRoomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashierRoom, this.listCashierRoom, controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    layoutControlGroup1.Enabled = false;
                    MessageBox.Show("Không thanh toán được, mời bạn chọn lại");
                    return;
                }
                this.listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                List<long> ids = new List<long>();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                List<HIS_CARO_ACCOUNT_BOOK> caroAccountBooks = null;
                if (cboCashierRoom.EditValue != null)
                {
                    HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                    caroAccountBookFilter.CASHIER_ROOM_ID = Convert.ToInt64(cboCashierRoom.EditValue);
                    caroAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);
                }
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                ids = ids.Distinct().ToList();
                if (ids != null && ids.Count > 0)
                {
                    HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                    acFilter.IDs = ids;
                    acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    acFilter.FOR_DEBT = true;
                    acFilter.IS_OUT_OF_BILL = false;
                    acFilter.ORDER_DIRECTION = "DESC";
                    acFilter.ORDER_FIELD = "ID";
                    this.listAccountBook = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, this.listAccountBook, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDafaultCashierRoom()
        {
            try
            {
                if (cboCashierRoom.EditValue == null)
                {
                    var data = this.listCashierRoom != null ? this.listCashierRoom.FirstOrDefault() : null;
                    if (data != null)
                    {
                        cboCashierRoom.EditValue = data.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook()
        {
            try
            {
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookDebt != null && GlobalVariables.DefaultAccountBookDebt.Count > 0)
                {
                    var lstBook = this.listAccountBook.Where(o => GlobalVariables.DefaultAccountBookDebt.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinNumOrder.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                if (this.listMediMateADO != null)
                {
                    List<MediMateADO> selecteds = this.listMediMateADO.Where(s => s.IsCheck).ToList();
                    if (selecteds.Count > 0)
                    {
                        totalPrice = selecteds.Sum(o => (o.TOTAL_PRICE ?? 0));
                    }
                }
                spinDebtAmount.Value = totalPrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValue(bool init, bool resetSearch)
        {
            try
            {
                try
                {
                    resultTranDebt = null;
                    this.listMediMateADO = null;
                    dxValidationProvider1.RemoveControlError(dtTransactionTime);
                    if (init)
                    {
                        //spinNumOrder.Value = 0;
                    }
                    else
                    {
                        expMestIds = null;
                        expMestCode = null;
                    }
                    txtDescription.Text = "";
                    spinDebtAmount.Value = 0;
                    txtTransactionCode.Text = "";
                    if (resetSearch)
                    {
                        txtTreatmentCode.Text = "";
                        txtExpMestCode.Text = "";
                    }
                    btnNew.Enabled = true;
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;
                    btnSaveAndPrint.Enabled = true;
                    dtTransactionTime.DateTime = DateTime.Now;
                    this.FillDataGridExpMestDetail();
                    this.SetTotalPrice();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateDataFromInput()
        {
            try
            {
                if (this.expMestIds != null && this.expMestIds.Count > 0)
                {
                    this.listMediMateADO = new List<MediMateADO>();
                    DHisExpMestDetail1Filter filter = new DHisExpMestDetail1Filter();
                    filter.HAS_BILL = false;
                    filter.HAS_DEBT = false;
                    filter.EXP_MEST_IDs = expMestIds;

                    List<D_HIS_EXP_MEST_DETAIL_1> expMestDetails = new BackendAdapter(new CommonParam()).Get<List<D_HIS_EXP_MEST_DETAIL_1>>("api/HisExpMest/GetExpMestDetail1", ApiConsumers.MosConsumer, filter, null);

                    if (expMestDetails != null && expMestDetails.Count > 0)
                    {
                        Mapper.CreateMap<D_HIS_EXP_MEST_DETAIL_1, MediMateADO>();
                        expMestDetails = expMestDetails.OrderBy(o => o.EXP_MEST_CODE).ToList();
                        foreach (D_HIS_EXP_MEST_DETAIL_1 detail in expMestDetails)
                        {
                            MediMateADO ado = Mapper.Map<MediMateADO>(detail);
                            if (ado.VIR_PRICE.HasValue && ado.AMOUNT.HasValue)
                            {
                                ado.TOTAL_PRICE = ((ado.VIR_PRICE.Value * ado.AMOUNT.Value) - (ado.DISCOUNT ?? 0));
                            }
                            ado.EXP_MEST_CODE_PLUS = ado.EXP_MEST_CODE;
                            listMediMateADO.Add(ado);
                        }
                    }
                    this.FillDataGridExpMestDetail();
                }
                else if (!String.IsNullOrWhiteSpace(this.expMestCode))
                {
                    txtExpMestCode.Text = this.expMestCode;
                    this.FindData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataGridExpMestDetail()
        {
            try
            {

                LogSystem.Info("FillDataGridExpMestDetail.1");
                //gridControlExpMestDetail.BeginUpdate();
                LogSystem.Info("FillDataGridExpMestDetail.2");
                gridControlExpMestDetail.DataSource = this.listMediMateADO;
                LogSystem.Info("FillDataGridExpMestDetail.3");
                //gridControlExpMestDetail.EndUpdate();
                gridControlExpMestDetail.RefreshDataSource();
                LogSystem.Info("FillDataGridExpMestDetail.4");
                gridViewExpMestDetail.SelectAll();
                LogSystem.Info("FillDataGridExpMestDetail.5");
                this.SetPatientInfo();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPatientInfo()
        {
            try
            {
                if (this.listMediMateADO != null && this.listMediMateADO.Count > 0)
                {
                    MediMateADO ado = this.listMediMateADO.FirstOrDefault();
                    lblAddress.Text = ado.TDL_PATIENT_ADDRESS ?? "";
                    if (ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1 && ado.TDL_PATIENT_DOB.HasValue)
                    {
                        lblDob.Text = ado.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(ado.TDL_PATIENT_DOB ?? 0) ?? "";
                    }
                    lblGender.Text = ado.TDL_PATIENT_GENDER_NAME ?? "";
                    lblPatientCode.Text = ado.TDL_PATIENT_CODE ?? "";
                    lblPatientName.Text = ado.TDL_PATIENT_NAME ?? "";
                }
                else
                {
                    lblAddress.Text = "";
                    lblDob.Text = "";
                    lblGender.Text = "";
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        this.FindData();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtExpMestCode.Text))
                        this.FindData();
                }
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
                if (!btnFind.Enabled) return;
                FindData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                    var data = (MediMateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PRICE_VAT_STR")
                        {
                            if (data.VIR_PRICE.HasValue)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.VIR_PRICE.Value, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "DISCOUNT_STR")
                        {
                            if (data.DISCOUNT.HasValue)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.DISCOUNT.Value, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE_STR")
                        {
                            if (data.TOTAL_PRICE.HasValue)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.TOTAL_PRICE.Value, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
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

        private void gridViewExpMestDetail_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;
                GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);

                if (hitInfo.Column == null || hitInfo.Column.FieldName != "DX$CheckboxSelectorColumn")
                {
                    return;
                }
                if (hitInfo.HitTest == GridHitTest.RowGroupCheckSelector || hitInfo.RowHandle >= 0 || this.resultTranDebt != null)
                {
                    ((DXMouseEventArgs)e).Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                MediMateADO row = (MediMateADO)gridViewExpMestDetail.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.RowHandle != gridViewExpMestDetail.FocusedRowHandle)
                    {
                        e.Appearance.BackColor = Color.White;
                    }
                    if (row.IS_MEDICINE != 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                int[] selectedIndexs = gridViewExpMestDetail.GetSelectedRows();
                this.listMediMateADO.ForEach(o => o.IsCheck = false);

                foreach (int rowhandler in selectedIndexs)
                {
                    MediMateADO ado = (MediMateADO)gridViewExpMestDetail.GetRow(rowhandler);
                    if (ado != null)
                    {
                        ado.IsCheck = true;
                    }
                }
                gridControlExpMestDetail.RefreshDataSource();
                this.SetTotalPrice();
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
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (spinNumOrder.Enabled)
                    {
                        spinNumOrder.Focus();
                        spinNumOrder.SelectAll();
                    }
                    else
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinNumOrder.Enabled)
                    {
                        spinNumOrder.Focus();
                        spinNumOrder.SelectAll();
                    }
                    else
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var account = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        if (account != null)
                        {
                            GlobalVariables.DefaultAccountBookDebt = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookDebt.Add(account);
                            SetDataToDicNumOrderInAccountBook(account);
                        }
                    }
                    {
                        if (spinNumOrder.Enabled)
                        {
                            spinNumOrder.Focus();
                            spinNumOrder.SelectAll();
                        }
                        else
                        {
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

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        UpdateDictionaryNumOrderAccountBook(accountBook);
                    }
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrder_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            try
            {
                if (cboAccountBook.EditValue != null)
                {
                    var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
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

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!btnSaveAndPrint.Enabled || !dxValidationProvider1.Validate()) return;
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                success = this.ProcessSave(ref param);
                WaitingManager.Hide();
                if (success)
                {
                    this.ProcessPrint();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate()) return;
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                success = this.ProcessSave(ref param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessSave(ref CommonParam param)
        {
            bool success = false;
            try
            {
                gridViewExpMestDetail.PostEditor();
                gridViewExpMestDetail.UpdateCurrentRow();
                List<MediMateADO> selectedData = this.listMediMateADO != null ? this.listMediMateADO.Where(o => o.IsCheck).ToList() : null;
                if (selectedData == null || selectedData.Count == 0)
                {
                    param.Messages.Add(ResourcesMessage.NguoiDungChuaChonPhieuXuatDeChotNo);
                    return success;
                }

                V_HIS_CASHIER_ROOM cashierRoom = this.listCashierRoom.FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue));

                HisTransactionDrugStoreDebtSDO data = new HisTransactionDrugStoreDebtSDO();
                data.Transaction = new HIS_TRANSACTION();
                data.Transaction.CASHIER_ROOM_ID = cashierRoom.ID;
                data.Transaction.PAY_FORM_ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                data.RequestRoomId = cashierRoom.ROOM_ID;
                var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.Transaction.NUM_ORDER = (long)spinNumOrder.Value;
                }
                data.Transaction.AMOUNT = (decimal)spinDebtAmount.EditValue;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.Transaction.TRANSACTION_TIME = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                data.Transaction.DESCRIPTION = txtDescription.Text;
                data.ExpMestIds = selectedData.Select(s => s.EXP_MEST_ID.Value).Distinct().ToList();

                LogSystem.Debug("Dau vao khi goi api: HisTransaction/CreateDrugStoreDebt: " + LogUtil.TraceData("data", data));

                var rs = new BackendAdapter(param).Post<HisDrugStoreDebtResultSDO>("api/HisTransaction/CreateDrugStoreDebt", ApiConsumers.MosConsumer, data, param);

                LogSystem.Debug("Ket qua tra ve khi goi api: HisTransaction/CreateDrugStoreDebt: " + LogUtil.TraceData("rs", rs));
                if (rs != null)
                {
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTranDebt = rs;
                    SetBillSuccessControl();
                    btnPrint.Enabled = true;
                    btnSaveAndPrint.Enabled = false;
                    btnSave.Enabled = false;
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                    if (this.refreshData != null) this.refreshData(rs);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != accountBook.ID).ToList();// && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBillSuccessControl()
        {
            try
            {
                if (this.resultTranDebt != null)
                {
                    txtTransactionCode.Text = this.resultTranDebt.Debt.TRANSACTION_CODE;
                    spinDebtAmount.Value = this.resultTranDebt.Debt.AMOUNT;
                }
                else
                {
                    throw new NullReferenceException("this.resultTranBill is null");
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
                if (!btnPrint.Enabled) return;

                this.ProcessPrint();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000388", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000388":
                        InPhieuXacNhanCongNo(printCode, fileName, ref result);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuXacNhanCongNo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranDebt == null)
                    return;
                WaitingManager.Show();

                MPS.Processor.Mps000388.PDO.Mps000388PDO pdo = new MPS.Processor.Mps000388.PDO.Mps000388PDO(
                    this.resultTranDebt.Debt,
                    this.resultTranDebt.DebtGoods
                    );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranDebt.Debt.TDL_TREATMENT_CODE ?? ""), printTypeCode, currentModuleBase.RoomId);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModuleBase.RoomId, currentModuleBase.RoomTypeId, listArgs);
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
                if (!btnNew.Enabled) return;
                this.ResetControlValue(false, true);
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FindData()
        {
            try
            {
                LogSystem.Info("FindData.1");
                this.ResetControlValue(false, false);
                this.listMediMateADO = new List<MediMateADO>();
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text) || !String.IsNullOrWhiteSpace(txtExpMestCode.Text))
                {
                    LogSystem.Info("FindData.1.1");
                    DHisExpMestDetail1Filter filter = new DHisExpMestDetail1Filter();
                    filter.HAS_BILL = false;
                    filter.HAS_DEBT = false;
                    filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                        filter.TDL_TREATMENT_CODE__EXACT = code;
                    }
                    else if (!String.IsNullOrWhiteSpace(txtExpMestCode.Text))
                    {
                        string code = txtExpMestCode.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtExpMestCode.Text = code;
                        }
                        filter.EXP_MEST_CODE__EXACT = code;
                    }
                    LogSystem.Info("FindData.1.2");
                    List<D_HIS_EXP_MEST_DETAIL_1> expMestDetails = new BackendAdapter(new CommonParam()).Get<List<D_HIS_EXP_MEST_DETAIL_1>>("api/HisExpMest/GetExpMestDetail1", ApiConsumers.MosConsumer, filter, null);
                    LogSystem.Info("FindData.1.3");
                    if (expMestDetails != null && expMestDetails.Count > 0)
                    {
                        Mapper.CreateMap<D_HIS_EXP_MEST_DETAIL_1, MediMateADO>();
                        expMestDetails = expMestDetails.OrderBy(o => o.EXP_MEST_CODE).ToList();
                        foreach (D_HIS_EXP_MEST_DETAIL_1 detail in expMestDetails)
                        {
                            MediMateADO ado = Mapper.Map<MediMateADO>(detail);
                            if (ado.VIR_PRICE.HasValue && ado.AMOUNT.HasValue)
                            {
                                ado.TOTAL_PRICE = ((ado.VIR_PRICE.Value * ado.AMOUNT.Value) - (ado.DISCOUNT ?? 0));
                            }
                            ado.EXP_MEST_CODE_PLUS = ado.EXP_MEST_CODE;
                            listMediMateADO.Add(ado);
                        }
                    }
                }
                LogSystem.Info("FindData.2");
                this.FillDataGridExpMestDetail();
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

        private void barBtnItemSaveAndPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

    }
}
