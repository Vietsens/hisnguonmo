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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using System.Runtime.InteropServices;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Config;
using System.Threading;
using Inventec.Desktop.Common.Controls.ValidationRule;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2
{
    public partial class UCExpMestSaleCreateV2 : UserControl
    {
        #region declare
        private long roomId;
        private long roomTypeId;
        private int positionHandleControl = -1;
        private HIS_MEDI_STOCK mediStock { get; set; }
        private MediMateTypeADO currentMediMate { get; set; }
        private List<HisMedicineTypeInStockSDO> mediInStocks;
        private List<HisMaterialTypeInStockSDO> mateInStocks;
        private HIS_SERVICE_REQ serviceReq { get; set; }
        private HisExpMestResultSDO expMestResult { get; set; }
        private string clientSessionKey { get; set; }
        private bool savePrint = false;
        public int Action { get; set; }
        private long? expMestId { get; set; }
        private List<long> expMestMedicineIds { get; set; }
        private List<long> expMestMaterialIds { get; set; }
        HIS.Desktop.Plugins.ExpMestSaleCreateV2.Base.GlobalDataStore.ModuleAction moduleAction { get; set; }

        private bool discountFocus { get; set; }
        private bool discountRatioFocus { get; set; }
        private bool discountDetailFocus { get; set; }
        private bool discountDetailRatioFocus { get; set; }

        private bool isShowMessUpdate { get; set; }

        bool isSearchOrderByXHT = false;
        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        bool isShow = true;

        decimal oldDayNum = 1;

        int x = 0;
        int y = 0;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.ExpMestSaleCreateV2";
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook { get; set; }
        #endregion

        private Dictionary<long, MediMateTypeADO> dicMediMateAdo { get; set; }

        public UCExpMestSaleCreateV2(long roomTypeId, long roomId, long? expMestId)
        {
            InitializeComponent();
            try
            {
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
                this.expMestId = expMestId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestSaleCreateV2_Load(object sender, EventArgs e)
        {
            try
            {
                InitModuleAction();
                ValidControl();
                LoadMediStockFromRoomId();
                LoadMediMateFromMediStock();
                LoadDataToComboUser();
                LoadDataToControl();
                InitComboCommon(this.cboAge, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>(), "Id", "MoTa", 0, "", 0);
                this.clientSessionKey = Guid.NewGuid().ToString();
                this.Action = GlobalDataStore.ActionAdd;
                ResetControl();
                HisConfigCFG.LoadConfig();
                InitMenuPrint(null);
                EnableByCheckIsVisitor();
                CheckStockIsBusiness();
                SetEnableControlPriceByCheckBox();
                LoadDataExpMestByEdit();
                SetLabelSave(this.moduleAction);
                InitControlState();
                EventTXH();
                ThreadLoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadData()
        {
            try
            {
                var dataList = BackendDataWorker.Get<HIS_CASHIER_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_USER_ROOM>().Where(p => p.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && (p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList().Select(p => p.ROOM_ID).ToList().Exists(p => p == o.ROOM_ID)).ToList();
                InitComboCommon(this.cboUserRoom, dataList, "ID", "CASHIER_ROOM_NAME", 0, "CASHIER_ROOM_CODE", 0);
                if(dataList != null && dataList.Count > 0)
                    cboUserRoom.EditValue = dataList[0].ID;
                else
                    LoadDataToComboAccountBook();
                var dataPayForm = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                InitComboCommon(this.cboPayForm, dataPayForm, "ID", "PAY_FORM_NAME", 0, "PAY_FORM_CODE", 0);
                if (dataPayForm != null && dataPayForm.Count > 0)
                    cboPayForm.EditValue = dataPayForm[0].ID;
                LoadComboAccountBook();
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
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                List<long> ids = new List<long>();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                List<HIS_CARO_ACCOUNT_BOOK> caroAccountBooks = null;
                if (cboUserRoom.EditValue != null)
                {
                    HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                    caroAccountBookFilter.CASHIER_ROOM_ID = Convert.ToInt64(cboUserRoom.EditValue);
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
                    int count = ids.Count;
                    int step = 0;
                    while (count > 0)
                    {
                        var lstId = ids.Skip(step).Take(100).ToList();
                        HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                        acFilter.IDs = lstId;
                        acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        acFilter.FOR_BILL = true;
                        acFilter.IS_OUT_OF_BILL = false;
                        acFilter.ORDER_DIRECTION = "DESC";
                        acFilter.ORDER_FIELD = "ID";
                        ListAccountBook.AddRange(new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null));
                        step += 100;
                        count -= 100;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadComboAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, ListAccountBook, controlEditorADO);
                if (HisConfigCFG.GetValue("HIS.Desktop.Plugins.TransactionBill.AutoSelectAccountBookIfHasOne") == "1")
                {
                    if (ListAccountBook != null)
                    {
                        if (ListAccountBook.Count == 1)
                            cboAccountBook.EditValue = ListAccountBook[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitModuleAction()
        {
            try
            {
                if (this.expMestId.HasValue && this.expMestId.Value > 0)
                {
                    moduleAction = GlobalDataStore.ModuleAction.EDIT;
                }
                else
                {
                    moduleAction = GlobalDataStore.ModuleAction.ADD;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicineInStock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewMedicineInStock.GetFocusedRow() as HisMedicineTypeInStockSDO;
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new MediMateTypeADO(data);
                    SetControlMediMateClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetControlMediMateClick()
        {
            try
            {

                spinAmount.Focus();
                txtTutorial.Text = "";
                txtNote.Text = "";
                spinProfit.EditValue = null;
                btnAdd.Enabled = true;
                txtTutorial.Enabled = true;
                checkImpExpPrice.Enabled = true;
                checkImpExpPrice.CheckState = CheckState.Unchecked;

                long isCheckImpPrice = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IsCheckImpPrice));
                SHOW_PRICE_SERVICE_PATY show = SHOW_PRICE_SERVICE_PATY.FALSE;
                if (isCheckImpPrice == 1)
                {
                    checkImpExpPrice.CheckState = CheckState.Checked;
                    show = SHOW_PRICE_SERVICE_PATY.TRUE;
                }

                if (this.currentMediMate.IsMedicine)
                {
                    spinDayNum.Enabled = true;
                    spinDayNum.Value = 1;
                    oldDayNum = 1;
                }
                else
                {
                    spinDayNum.Enabled = false;
                    spinDayNum.EditValue = null;
                }

                SetEnableControlPriceByCheckBox(show);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void FromClosingEvent()
        {
            try
            {
                ReleaseAll();
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
                CommonParam param = new CommonParam();
                positionHandleControl = -1;
                bool success = false;
                if (!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null || !CheckDataInput())
                    return;

                if (dicMediMateAdo == null)
                    dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();

                decimal amount = spinAmount.Value;
                List<long> beanIds = null;
                if (dicMediMateAdo.ContainsKey(this.currentMediMate.MEDI_MATE_TYPE_ID))
                {
                    MediMateTypeADO mediMateTypeADO = dicMediMateAdo[this.currentMediMate.MEDI_MATE_TYPE_ID];
                    if (mediMateTypeADO != null)
                    {
                        beanIds = mediMateTypeADO.BeanIds;
                        if (this.Action == GlobalDataStore.ActionAdd)
                        {
                            amount += mediMateTypeADO.EXP_AMOUNT;
                        }
                    }
                }

                if (this.currentMediMate.IsMedicine)
                {
                    TakeBeanMedicineProccess(beanIds, amount, ref success);
                }
                else if (this.currentMediMate.IsMaterial)
                {
                    TakeBeanMaterialProccess(beanIds, amount, ref success);
                }

                //Take false
                if (success == false)
                    return;

                this.currentMediMate.IsNotInStock = false;
                this.currentMediMate.TUTORIAL = txtTutorial.Text;
                this.currentMediMate.NOTE = txtNote.Text;

                decimal? totalPrice = 0;
                if (this.checkImpExpPrice.Checked)
                {
                    this.currentMediMate.EXP_VAT_RATIO = spinExpVatRatio.Value / 100;
                    if (spinExpPrice.EditValue != null && spinExpPrice.Value > 0)
                    {
                        this.currentMediMate.EXP_PRICE = spinExpPrice.Value;
                        totalPrice = (amount * this.currentMediMate.EXP_PRICE) * (1 + this.currentMediMate.EXP_VAT_RATIO);
                    }
                    else
                    {
                        totalPrice = this.currentMediMate.TOTAL_PRICE;
                        this.currentMediMate.EXP_PRICE = totalPrice / amount;
                        if (spinProfit.EditValue != null)
                        {
                            this.currentMediMate.Profit = spinProfit.Value / 100;
                            totalPrice = totalPrice * (1 + spinProfit.Value / 100);
                        }
                    }
                    this.currentMediMate.IsCheckExpPrice = true;
                }
                else
                {
                    totalPrice = this.currentMediMate.TOTAL_PRICE;
                    this.currentMediMate.IsCheckExpPrice = false;
                    this.currentMediMate.EXP_PRICE = totalPrice / amount;
                }

                if (spinDiscountDetailRatio.EditValue != null && spinDiscountDetailRatio.Value > 0)
                {
                    totalPrice = totalPrice * (1 - spinDiscountDetailRatio.Value / 100);
                }

                this.currentMediMate.TOTAL_PRICE = totalPrice;
                this.currentMediMate.EXP_AMOUNT = amount;
                this.currentMediMate.DISCOUNT = spinDiscountDetail.Value;
                this.currentMediMate.DISCOUNT_RATIO = spinDiscountDetailRatio.Value / 100;
                if (spinDayNum.EditValue != null)
                    this.currentMediMate.DayNum = (long?)spinDayNum.Value;
                dicMediMateAdo[this.currentMediMate.MEDI_MATE_TYPE_ID] = this.currentMediMate;
                LoadDataToGridExpMestDetail();
                SetTotalPriceExpMestDetail();
                ResetControlAfterAddClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckDataInput()
        {
            bool result = false;
            try
            {
                if (spinAmount.Value <= this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    this.currentMediMate.IsExceedsAvailable = false;
                    //MessageManager.Show(ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                    //return result;
                }

                if (spinDiscountDetailRatio.EditValue != null && (spinDiscountDetailRatio.Value >= 100 || spinDiscountDetailRatio.Value < 0))
                {
                    MessageBox.Show("Chiết khấu không được âm và phải nhỏ hơn 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return result;
                }

                if (spinExpVatRatio.EditValue != null && (spinExpVatRatio.Value >= 100 || spinExpVatRatio.Value < 0))
                {
                    MessageBox.Show("VAT không được âm và phải nhỏ hơn 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return result;
                }
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void TakeBeanMedicineProccess(List<long> beanIds, decimal amount, ref bool success)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                if (this.currentMediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { this.currentMediMate.ExpMestDetailId.Value };
                }
                List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>("api/HisMedicineBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                if (medicineBeans == null || medicineBeans.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    throw new Exception("Take is correct");
                }
                success = true;

                this.currentMediMate.BeanIds = medicineBeans.Select(o => o.ID).ToList();
                if (!checkImpExpPrice.Checked || spinProfit.EditValue != null)
                {
                    this.currentMediMate.TOTAL_PRICE = medicineBeans.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialProccess(List<long> beanIds, decimal amount, ref bool success)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                if (this.currentMediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { this.currentMediMate.ExpMestDetailId.Value };
                }
                List<HIS_MATERIAL_BEAN> materialBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>("api/HisMaterialBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                if (materialBeans == null || materialBeans.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    throw new Exception("Take is correct");
                }
                success = true;

                this.currentMediMate.BeanIds = materialBeans.Select(o => o.ID).ToList();
                if (!checkImpExpPrice.Checked || (spinProfit.EditValue != null))
                {
                    this.currentMediMate.TOTAL_PRICE = materialBeans.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMedicineProccess(MediMateTypeADO mediMate, decimal amount)
        {
            try
            {
                LogSystem.Info("TAKEN BEAN amount " + amount);
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = mediMate.BeanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = mediMate.MEDI_MATE_TYPE_ID;
                if (mediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { mediMate.ExpMestDetailId.Value };
                }
                List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>("api/HisMedicineBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                mediMate.EXP_AMOUNT = amount;
                if (medicineBeans == null || medicineBeans.Count == 0)
                {
                    mediMate.IsNotInStock = true;
                    return;
                }

                mediMate.IsNotInStock = false;

                mediMate.BeanIds = medicineBeans.Select(o => o.ID).ToList();
                if (!mediMate.IsCheckExpPrice)
                {
                    mediMate.TOTAL_PRICE = medicineBeans.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialProccess(MediMateTypeADO mediMate, decimal amount)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = mediMate.BeanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = mediMate.PATIENT_TYPE_ID;
                takeBeanSDO.TypeId = mediMate.MEDI_MATE_TYPE_ID;
                if (mediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { mediMate.ExpMestDetailId.Value };
                }
                List<HIS_MATERIAL_BEAN> materialBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>("api/HisMaterialBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                if (materialBeans == null || materialBeans.Count == 0)
                {
                    mediMate.IsNotInStock = true;
                    return;
                }
                mediMate.IsNotInStock = false;

                mediMate.BeanIds = materialBeans.Select(o => o.ID).ToList();
                if (!mediMate.IsCheckExpPrice)
                {
                    mediMate.TOTAL_PRICE = materialBeans.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestDetail_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider3.Validate() ||!dxValidationProvider1.Validate() || !CheckIsPrescription() || dicMediMateAdo.Count == 0 || this.mediStock == null || !CheckValiDob())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref success, ref param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckValiDob()
        {
            bool result = true;
            try
            {
                long dtNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (txtPatientDob.Text.Length == 4)
                {
                    int year = Inventec.Common.TypeConvert.Parse.ToInt32(dtNow.ToString().Substring(0, 4));
                    int patientYear = Inventec.Common.TypeConvert.Parse.ToInt32(txtPatientDob.Text);
                    if (year < patientYear)
                    {
                        MessageBox.Show("Năm sinh không được lớn hơn năm hiện tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
                    }
                }
                else
                {
                    dtPatientDob.Text = txtPatientDob.Text;
                    string dateDob = dtPatientDob.DateTime.ToString("yyyyMMdd");
                    string timeDob = "00";
                    long dateDobFull = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                    if (dateDobFull > dtNow)
                    {
                        MessageBox.Show("Ngày sinh không được lớn thời gian hiện tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckIsPrescription()
        {
            bool result = true;
            try
            {
                if (!checkIsVisitor.Checked && (this.serviceReq == null && (!this.expMestId.HasValue || this.expMestId.Value == 0)))
                {
                    MessageBox.Show(String.Format("Không tìm thấy đơn thuốc có mã : {0}", txtPrescriptionCode.Text));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToComboAccountBook();
                UpNumber();
                ReleaseAll();
                ResetAllControl();
                SetControlByExpMest(null);
                //LoadAvailabileMediMateInStock(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSampleForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string key = txtSampleForm.Text.Trim().ToLower();
                    var listData = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.EXP_MEST_TEMPLATE_CODE.ToLower().Contains(key)).ToList();
                    List<HIS_EXP_MEST_TEMPLATE> result = (listData != null ? (listData.Count == 1 ? listData : listData.Where(o => o.EXP_MEST_TEMPLATE_CODE == key).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        txtSampleForm.Text = result.First().EXP_MEST_TEMPLATE_CODE;
                        cboSampleForm.Properties.Buttons[1].Visible = true;
                        cboSampleForm.EditValue = result.First().ID;
                        FillDataToGridDetailByTemplate(result.First());
                        if (txtVirPatientName.Enabled)
                        {
                            txtVirPatientName.Focus();
                            txtVirPatientName.SelectAll();
                        }
                        else
                        {
                            txtKeyworkMedicineInStock.Focus();
                            txtKeyworkMedicineInStock.SelectAll();
                        }
                    }
                    else
                    {
                        cboSampleForm.Focus();
                        cboSampleForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboSampleForm != null)
                {
                    HIS_EXP_MEST_TEMPLATE expMestTemp = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleForm.EditValue.ToString()));
                    if (expMestTemp != null)
                    {
                        txtSampleForm.Text = expMestTemp.EXP_MEST_TEMPLATE_CODE;
                        cboSampleForm.Properties.Buttons[1].Visible = true;
                        FillDataToGridDetailByTemplate(expMestTemp);
                        if (txtVirPatientName.Enabled)
                        {
                            txtVirPatientName.Focus();
                            txtVirPatientName.SelectAll();
                        }
                        else
                        {
                            txtKeyworkMedicineInStock.Focus();
                            txtKeyworkMedicineInStock.SelectAll();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void btnSavePrint_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.savePrint = true;
        //        btnSave_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtKeyworkMedicineInStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (!String.IsNullOrEmpty(txtKeyworkMedicineInStock.Text) && mediInStocks != null)
                    {
                        List<HisMedicineTypeInStockSDO> medicineInStocks = mediInStocks.Where(o =>
                            o.MedicineTypeName.ToUpper().Contains(txtKeyworkMedicineInStock.Text.ToUpper())
                            || o.MedicineTypeCode.ToUpper().Contains(txtKeyworkMedicineInStock.Text.ToUpper())
                            || (!String.IsNullOrWhiteSpace(o.ActiveIngrBhytCode) && o.ActiveIngrBhytCode.ToUpper().Contains(txtKeyworkMedicineInStock.Text.ToUpper()))
                            || (!String.IsNullOrWhiteSpace(o.ActiveIngrBhytName) && o.ActiveIngrBhytName.ToUpper().Contains(txtKeyworkMedicineInStock.Text.ToUpper()))
                            || (!String.IsNullOrWhiteSpace(o.Concentra) && o.Concentra.ToUpper().Contains(txtKeyworkMedicineInStock.Text.ToUpper()))).ToList();
                        gridControlMedicineInStock.DataSource = medicineInStocks;
                    }
                    else
                    {
                        gridControlMedicineInStock.DataSource = mediInStocks;
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyworkMaterialInStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyworkMaterialInStock.Text) && mateInStocks != null)
                {
                    List<HisMaterialTypeInStockSDO> materialInStocks = mateInStocks.Where(o =>
                        o.MaterialTypeName.ToUpper().Contains(txtKeyworkMaterialInStock.Text.ToUpper())
                        || o.MaterialTypeCode.ToUpper().Contains(txtKeyworkMaterialInStock.Text.ToUpper())).ToList();
                    gridControlMaterialInStock.DataSource = materialInStocks;
                }
                else
                {
                    gridControlMaterialInStock.DataSource = mateInStocks;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicineInStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMedicineTypeInStockSDO dataRow = (HisMedicineTypeInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        if (dataRow != null && dataRow.LastExpiredDate.HasValue)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.LastExpiredDate.Value);
                        }
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_STR" && dataRow != null)
                    {
                        //var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == dataRow.Id);
                        //if (dataType != null)
                        //{
                        //    decimal? impPrice = dataType.IMP_PRICE * (1 + (dataType.IMP_VAT_RATIO.HasValue ? dataType.IMP_VAT_RATIO : 0));
                        //    if (impPrice != null && impPrice > 0)
                        //        e.Value = Inventec.Common.Number.Convert.NumberToString(impPrice ?? 0, ConfigApplications.NumberSeperator);

                        //}
                        try
                        {
                            var paty = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString())
                                    && o.SERVICE_ID == dataRow.ServiceId);
                            if (paty != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(paty.PRICE, ConfigApplications.NumberSeperator);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }

                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialInStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMaterialTypeInStockSDO dataRow = (HisMaterialTypeInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            if (dataRow.LastExpiredDate.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.LastExpiredDate.Value);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_STR" && dataRow != null)
                        {
                            //var dataType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == dataRow.Id);
                            //if (dataType != null)
                            //{
                            //    decimal? impPrice = dataType.IMP_PRICE * (1 + (dataType.IMP_VAT_RATIO.HasValue ? dataType.IMP_VAT_RATIO : 0));
                            //    if (impPrice != null && impPrice > 0)
                            //        e.Value = Inventec.Common.Number.Convert.NumberToString(impPrice ?? 0, ConfigApplications.NumberSeperator);
                            //}
                            try
                            {
                                var paty = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString())
                                        && o.SERVICE_ID == dataRow.ServiceId);
                                if (paty != null)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(paty.PRICE, ConfigApplications.NumberSeperator);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }

                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkIsVisitor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                EnableByCheckIsVisitor();
                ValidControlPrescriptionCode();
                txtPrescriptionCode.Text = "";
                txtVirPatientName.Text = "";
                txtAddress.Text = "";
                txtAge.Text = "";
                cboAge.EditValue = null;
                cboGender.EditValue = null;
                txtPatientDob.EditValue = null;
                dtPatientDob.EditValue = null;
                txtMaTHX.Text = "";
                cboTHX.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkImpExpPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                if (chk.Checked)
                {

                    SetEnableControlPriceByCheckBox(SHOW_PRICE_SERVICE_PATY.TRUE);
                    if (chk.EditorContainsFocus)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                }
                else
                {
                    SetEnableControlPriceByCheckBox();
                    if (chk.EditorContainsFocus)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public enum SHOW_PRICE_SERVICE_PATY
        {
            TRUE,
            FALSE
        }

        private void SetEnableControlPriceByCheckBox(SHOW_PRICE_SERVICE_PATY show = SHOW_PRICE_SERVICE_PATY.FALSE)
        {
            try
            {
                spinExpPrice.EditValue = null;
                spinExpVatRatio.EditValue = null;

                if (checkImpExpPrice.Checked)
                {
                    spinExpPrice.Enabled = true;
                    spinExpVatRatio.Enabled = true;
                    spinDiscountDetail.Enabled = true;
                    spinProfit.Enabled = true;
                }
                else
                {
                    spinExpPrice.Enabled = false;
                    spinExpVatRatio.Enabled = false;
                    spinDiscountDetail.Enabled = false;
                    spinDiscountDetailRatio.Enabled = true;
                    spinProfit.Enabled = false;
                }

                if (show == SHOW_PRICE_SERVICE_PATY.FALSE)
                    return;

                if (this.currentMediMate != null && cboPatientType.EditValue != null)
                {
                    var listServicePaty = BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                    if (listServicePaty != null)
                    {
                        var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString())
                            && o.SERVICE_ID == this.currentMediMate.SERVICE_ID);
                        if (paty != null)
                        {
                            spinExpPrice.Value = paty.PRICE;
                            spinExpVatRatio.Value = paty.VAT_RATIO * 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrescriptionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPrescriptionCode.Text))
                    {

                        CommonParam param = new CommonParam();

                        string code = String.Format("{0:000000000000}", Convert.ToInt64(txtPrescriptionCode.Text.Trim()));
                        txtPrescriptionCode.Text = code;
                        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        serviceReqFilter.SERVICE_REQ_CODE__EXACT = code;
                        serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        WaitingManager.Show();
                        serviceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
                        WaitingManager.Hide();
                        if (serviceReq == null)
                            MessageBox.Show(String.Format("Không tìm thấy mã đơn: {0} ", txtPrescriptionCode.Text));
                        else
                        {
                            dtIntructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME)
                                ?? DateTime.Now;
                        }

                        LoadPatientInfoFromPrescription(serviceReq);
                        TakeBeanFromPrescription();

                        txtSampleForm.Focus();
                        txtSampleForm.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialInStock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewMaterialInStock.GetFocusedRow() as HisMaterialTypeInStockSDO;
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new MediMateTypeADO(data);
                    SetControlMediMateClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider3_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    checkIsVisitor.Focus();
                    SetAvaliable0MediMateStock();//xuandv
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboSampleForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    ReleaseAllAndResetGrid();
                    cboSampleForm.Properties.Buttons[1].Visible = false;
                    cboSampleForm.EditValue = null;
                    txtSampleForm.Text = "";
                    txtSampleForm.Focus();
                    txtSampleForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestDetail_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotInStock || data.IsExceedsAvailable)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    string strDob = this.txtPatientDob.Text;
                    this.CalulatePatientAge(strDob);
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                    }
                    else
                    {
                        this.txtMaTHX.Focus();
                        this.txtMaTHX.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPatientDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.Update();
                    this.txtPatientDob.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    this.CalulatePatientAge(this.txtPatientDob.Text);

                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                    }
                    else
                    {
                        this.txtMaTHX.Focus();
                        this.txtMaTHX.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                        if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                        {
                            this.txtAge.Text = this.txtPatientDob.Text;
                            this.cboAge.EditValue = 1;
                            this.txtPatientDob.Text = dateValidObject.Age.ToString();
                        }
                        else if (String.IsNullOrEmpty(dateValidObject.Message))
                        {
                            if (!dateValidObject.HasNotDayDob)
                            {
                                this.txtPatientDob.Text = dateValidObject.OutDate;
                                this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                                this.dtPatientDob.Update();
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu txtPatientDob_PreviewKeyDown");

                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtPatientDob.EditValue = dt;
                        this.dtPatientDob.Update();
                    }
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.ShowPopup();
                    this.dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {

        }

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (!String.IsNullOrWhiteSpace(txtPatientDob.Text))
                //{
                //    txtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(txtPatientDob.Text);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

                txtPatientDob.Focus();
                txtPatientDob.SelectAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtPatientDob.Focus();
                    //txtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtVirPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDayNum.Focus();
                    spinDayNum.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExpVatRatio.Focus();
                    spinExpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkImpExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinExpPrice.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (this.currentMediMate != null)
                    {
                        //if (this.currentMediMate.IsMedicine)
                        //{
                        //    spinDiscountDetail.Focus();
                        //    spinDiscountDetail.SelectAll();
                        //}
                        //else
                        //{
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinExpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscountDetail.Focus();
                    spinDiscountDetail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkIsVisitor.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void checkIsVisitor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPrescriptionCode.Enabled)
                    {
                        txtPrescriptionCode.Focus();
                        txtPrescriptionCode.SelectAll();
                    }
                    else
                    {
                        txtSampleForm.Focus();
                        txtSampleForm.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboSampleForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    //if (txtVirPatientName.Enabled)
                    //{
                    //    txtVirPatientName.Focus();
                    //    txtVirPatientName.SelectAll();
                    //}
                    //else
                    //{
                    //    txtDescription.Focus();
                    //    txtDescription.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboSampleForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboSampleForm.EditValue != null)
                {
                    HIS_EXP_MEST_TEMPLATE expMestTemp = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleForm.EditValue.ToString()));
                    if (expMestTemp != null)
                    {
                        txtSampleForm.Text = expMestTemp.EXP_MEST_TEMPLATE_CODE;
                        cboSampleForm.Properties.Buttons[1].Visible = true;
                        FillDataToGridDetailByTemplate(expMestTemp);
                        if (txtVirPatientName.Enabled)
                        {
                            txtVirPatientName.Focus();
                            txtVirPatientName.SelectAll();
                        }
                        else
                        {
                            txtKeyworkMedicineInStock.Focus();
                            txtKeyworkMedicineInStock.SelectAll();
                        }

                    }
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
                    if (txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else
                    {
                        spinDiscount.Focus();
                        spinDiscount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboGender_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboGender.EditValue != null)
                {
                    //txtPatientDob.Focus();
                    //txtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void spinDiscount_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscount.EditValue == null)
                    {
                        spinDiscountRatio.Focus();
                        spinDiscountRatio.SelectAll();
                    }
                    else
                    {
                        if (txtLoginName.Enabled)
                        {
                            txtLoginName.Focus();
                            txtLoginName.SelectAll();
                        }
                        else if (xtraTabControlMain.SelectedTabPageIndex == 0)
                        {
                            txtKeyworkMedicineInStock.Focus();
                            txtKeyworkMedicineInStock.SelectAll();
                        }
                        else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                        {
                            txtKeyworkMaterialInStock.Focus();
                            txtKeyworkMaterialInStock.SelectAll();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountRatio_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtLoginName.Enabled)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
                else if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    txtKeyworkMedicineInStock.Focus();
                    txtKeyworkMedicineInStock.SelectAll();
                }
                else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    txtKeyworkMaterialInStock.Focus();
                    txtKeyworkMaterialInStock.SelectAll();
                }

            }
        }

        private void spinDiscountDetail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscountDetail.EditValue == null)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountDetailRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountDetail_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscountDetail.ContainsFocus)
                {
                    this.discountDetailFocus = true;
                    this.discountDetailRatioFocus = false;
                    decimal totalPrice = 0;
                    if (spinExpPrice.EditValue != null && spinAmount.EditValue != null)
                    {
                        totalPrice = spinExpPrice.Value * spinAmount.Value;
                    }
                    DiscountDisplayProcess(this.discountDetailFocus, this.discountDetailRatioFocus, spinDiscountDetail, spinDiscountDetailRatio, totalPrice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountDetailRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscountDetailRatio.ContainsFocus)
                {
                    this.discountDetailFocus = false;
                    this.discountDetailRatioFocus = true;
                    decimal totalPrice = 0;
                    if (spinExpPrice.EditValue != null && spinAmount.EditValue != null)
                    {
                        totalPrice = spinExpPrice.Value * spinAmount.Value;
                    }
                    DiscountDisplayProcess(this.discountDetailFocus, this.discountDetailRatioFocus, spinDiscountDetail, spinDiscountDetailRatio, totalPrice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscount.ContainsFocus)
                {
                    this.discountFocus = true;
                    this.discountRatioFocus = false;
                    decimal totalPrice = 0;
                    if (dicMediMateAdo != null)
                    {
                        totalPrice = dicMediMateAdo.Sum(o => o.Value.TOTAL_PRICE ?? 0);
                    }
                    DiscountDisplayProcess(this.discountFocus, this.discountRatioFocus, spinDiscount, spinDiscountRatio, totalPrice);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinDiscountRatio.ContainsFocus)
                {
                    this.discountFocus = false;
                    this.discountRatioFocus = true;
                    decimal totalPrice = 0;
                    if (dicMediMateAdo != null)
                    {
                        totalPrice = dicMediMateAdo.Sum(o => o.Value.TOTAL_PRICE ?? 0);
                    }
                    DiscountDisplayProcess(this.discountFocus, this.discountRatioFocus, spinDiscount, spinDiscountRatio, totalPrice);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DiscountByAmountAndPriceChange();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinExpPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    SpinEdit editor = sender as SpinEdit;
                    if (editor != null && editor.EditValue != null && editor.Value > 0)
                    {
                        spinProfit.EditValue = null;
                    }
                    DiscountByAmountAndPriceChange();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCtrlA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        //private void btnCtrlI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnSavePrint_Click(null, null);
        //}

        private void btnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnCtrlN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void gridControlMedicineInStock_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridViewMedicineInStock.RowCount > 0)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        gridViewMedicineInStock_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyworkMedicineInStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridViewMedicineInStock.RowCount == 1)
                    {
                        gridViewMedicineInStock.SelectRow(0);
                        gridViewMedicineInStock.Focus();
                        gridViewMedicineInStock_Click(null, null);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewMedicineInStock.SelectRow(0);
                    gridViewMedicineInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyworkMaterialInStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (gridViewMaterialInStock.RowCount == 1)
                {
                    gridViewMaterialInStock.SelectRow(0);
                    gridViewMaterialInStock.Focus();
                    gridViewMaterialInStock_Click(null, null);
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                gridViewMaterialInStock.SelectRow(0);
                gridViewMaterialInStock.Focus();
            }
        }

        private void gridControlMaterialInStock_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridViewMaterialInStock.RowCount > 0)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        gridViewMaterialInStock_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItemCtrlF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtraTabControlMain.SelectedTabPageIndex == 0)
            {
                txtKeyworkMedicineInStock.Focus();
                txtKeyworkMedicineInStock.SelectAll();
            }
            else if (xtraTabControlMain.SelectedTabPageIndex == 1)
            {
                txtKeyworkMaterialInStock.Focus();
                txtKeyworkMaterialInStock.SelectAll();
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMateTypeADO mediMateTypeADO = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (mediMateTypeADO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(mediMateTypeADO.EXP_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(mediMateTypeADO.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "PROFIT_DISPLAY")
                        {
                            e.Value = mediMateTypeADO.Profit * 100;
                        }
                        else if (e.Column.FieldName == "DISCOUNT_RATIO_DISPLAY")
                        {
                            e.Value = mediMateTypeADO.DISCOUNT_RATIO * 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_Leave(object sender, EventArgs e)
        {
            try
            {
                //string strDob = "";
                //if (!String.IsNullOrEmpty(txtPatientDob.Text))
                //{
                //    txtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(txtPatientDob.Text);
                //}
                //if (!String.IsNullOrEmpty(txtPatientDob.Text))
                //{
                //    if (txtPatientDob.Text.Length != 8 && txtPatientDob.Text.Length != 4 && txtPatientDob.Text.Length != 10 || !CheckDobFormat(txtPatientDob.Text))
                //    {
                //        //MessageBox.Show("Thông tin ngày sinh không đúng định dạng.", "Thông báo");
                //        txtPatientDob.SelectAll();
                //        txtPatientDob.Focus();
                //        MessageBox.Show("Thông tin ngày sinh không đúng định dạng.", "Thông báo");
                //        return;
                //    }

                //    if (txtPatientDob.Text.Length == 8)
                //    {
                //        strDob = txtPatientDob.Text.Substring(0, 2) + "/" + txtPatientDob.Text.Substring(2, 2) + "/" + txtPatientDob.Text.Substring(4, 4);
                //        if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                //        {
                //            strDob = txtPatientDob.Text.Substring(0, 2) + "/" + txtPatientDob.Text.Substring(2, 2) + "/" + txtPatientDob.Text.Substring(4, 4);
                //            txtPatientDob.Text = strDob;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckDobFormat(string txtDob)
        {
            bool result = true;
            try
            {
                if (!String.IsNullOrEmpty(txtDob))
                {
                    if (txtDob.Length != 8
                        && txtDob.Length != 4
                        && txtDob.Length != 10)
                    {
                        return result = false;
                    }

                    if (txtDob.Length == 8)
                    {
                        string strDob = txtPatientDob.Text.Substring(0, 2) + "/" + txtPatientDob.Text.Substring(2, 2) + "/" + txtPatientDob.Text.Substring(4, 4);
                        if (!(HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date))
                        {
                            return result = false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMateTypeADO mediMateTypeADO = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (mediMateTypeADO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString((mediMateTypeADO.TOTAL_PRICE ?? 0) / mediMateTypeADO.EXP_AMOUNT, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(mediMateTypeADO.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewExpMestMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMateTypeADO mediMateTypeADO = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (mediMateTypeADO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(mediMateTypeADO.EXP_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(mediMateTypeADO.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EnableControlAfterSaveSaleBill(object data)
        {
            try
            {
                if (data != null)
                {
                    btnSaleBill.Enabled = false;
                    btnAdd.Enabled = false;
                    btnSave.Enabled = false;
                    //btnSavePrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaleBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaleBill.Enabled)
                {
                    return;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MedicineSaleBill").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MedicineSaleBill'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(this.expMestResult.ExpMest.ID);
                    listArgs.Add((DelegateSelectData)EnableControlAfterSaveSaleBill);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSallBill_Manager_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaleBill_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDayNum_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDayNum_Leave(object sender, EventArgs e)
        {
            try
            {
                SpinEdit editor = sender as SpinEdit;
                if (editor != null && oldDayNum > 0 && editor.Value != oldDayNum)
                {
                    decimal amountInDay = spinAmount.Value / ((decimal)oldDayNum);
                    spinAmount.Value = amountInDay * editor.Value;
                    oldDayNum = editor.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewExpMestDetail_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    int visibleRowHandle = this.gridViewExpMestDetail.GetVisibleRowHandle(hitInfo.RowHandle);
                    int[] selectedRows = this.gridViewExpMestDetail.GetSelectedRows();
                    if (selectedRows != null && selectedRows.Length > 0 && selectedRows.Contains(visibleRowHandle))
                    {
                        this.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewExpMestDetail_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "Selection"
                    || e.Button == System.Windows.Forms.MouseButtons.Right)
                    return;
                if (e.Column.FieldName == "DELETE")
                {
                    this.DeleteRowExpMestDetail();
                }
                else
                {
                    this.EditRowExpMestDetail();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDayNum_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkImpExpPrice.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinProfit_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SpinEdit editor = sender as SpinEdit;
                if (editor != null && editor.EditValue != null)
                {
                    spinExpPrice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinProfit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExpPrice.Focus();
                    spinExpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintMess.Name)
                        {
                            chkPrintMess.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkGuild.Name)
                        {
                            chkGuild.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkExp.Name)
                        {
                            chkExp.Checked = item.VALUE == "1";
                        }
                    }
                }
                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(moduleLink);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentBySessionControlStateRDO)
                    {
                        if (item.KEY == cboAccountBook.Name && !string.IsNullOrEmpty(item.VALUE))
                        {
                            cboAccountBook.EditValue = Int64.Parse(item.VALUE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void EventTXH()
        {
            try
            {
                isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;

                if (isSearchOrderByXHT)
                {
                    lciTHX.Text = "X/H/T:";
                }
                else
                {
                    lciTHX.Text = "T/H/X:";
                }
                this.InitComboCommon(this.cboTHX, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>(), "ID", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaTHX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                        return;
                    }
                    this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());//Load lai trong TH cbo bi set lai dataSource
                    this.cboTHX.EditValue = null;
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>()
                        .Where(o => (o.SEARCH_CODE_COMMUNE != null
                            && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    if (listResult != null && listResult.Count >= 1)
                    {
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = dataNoCommunes[0].ID;
                            this.txtMaTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            SetAddress(dataNoCommunes[0]);
                        }
                        else if (listResult.Count == 1)
                        {
                            this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID;
                            this.txtMaTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            SetAddress(listResult[0]);
                        }
                        else
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSourceValueTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                if (communeADOs != null)
                    this.InitComboCommon(this.cboTHX, communeADOs, "ID", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.FocusShowpopup(this.cboTHX, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommonUtil(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommonUtil(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboTHX.EditValue = null;
                    this.cboTHX.Properties.Buttons[1].Visible = false;
                    this.txtMaTHX.Text = "";
                    //TODO -- 

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;

                            SetAddress(commune);
                        }
                    }
                    else
                    {
                        this.txtMaTHX.Focus();
                        this.txtMaTHX.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetAddress(HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune)
        {
            try
            {
                string x1 = (String.IsNullOrEmpty(commune.COMMUNE_NAME) ? "" : "" + commune.INITIAL_NAME + " " + commune.COMMUNE_NAME);
                string h1 = (String.IsNullOrEmpty(commune.DISTRICT_INITIAL_NAME) ? "" : (String.IsNullOrEmpty(x1) ? "" : " - ") + commune.DISTRICT_INITIAL_NAME) + (String.IsNullOrEmpty(commune.DISTRICT_NAME) ? "" : " " + commune.DISTRICT_NAME);
                string t1 = (String.IsNullOrEmpty(commune.PROVINCE_NAME) ? "" : " - " + commune.PROVINCE_NAME);
                this.txtAddress.Text = string.Format("{0}{1}{2}", x1, h1, t1);

                this.txtAddress.Focus();
                this.txtAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTHX_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {

                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;

                            SetAddress(commune);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>)this.cboTHX.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                    {
                        if (isSearchOrderByXHT)
                        {
                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : "" + item.INITIAL_NAME + " " + item.COMMUNE_NAME);
                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : (String.IsNullOrEmpty(x1) ? "" : " - ") + item.DISTRICT_INITIAL_NAME) + (String.IsNullOrEmpty(item.DISTRICT_NAME) ? "" : " " + item.DISTRICT_NAME);
                            string t1 = (String.IsNullOrEmpty(item.PROVINCE_NAME) ? "" : " - " + item.PROVINCE_NAME);
                            e.Value = string.Format("{0}{1}{2}", x1, h1, t1);
                        }
                        else
                        {
                            string t1 = item.PROVINCE_NAME;

                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : " - " + item.DISTRICT_INITIAL_NAME);
                            string h2 = !String.IsNullOrEmpty(item.DISTRICT_NAME) ?
                                String.IsNullOrEmpty(h1) ? "- " + item.DISTRICT_NAME : item.DISTRICT_NAME : "";

                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME);

                            e.Value = string.Format("{0}{1} {2}{3}", t1, h1, h2, x1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text)) return;

                string dob = "";
                if (this.txtPatientDob.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtPatientDob.Text);

                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtPatientDob.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                this.txtPatientDob.ErrorText = "";
                if (String.IsNullOrEmpty(this.txtPatientDob.Text.Trim()))
                    return;
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    this.txtAge.Text = this.txtPatientDob.Text;
                    this.cboAge.EditValue = 1;
                    this.txtPatientDob.Text = dateValidObject.Age.ToString();
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtPatientDob.Text = dateValidObject.OutDate;
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }
                else
                {
                    this.txtPatientDob.ErrorText = dateValidObject.Message;
                    e.Cancel = true;
                    return;
                }

                if (
                    ((this.txtPatientDob.EditValue ?? "").ToString() != (this.txtPatientDob.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.txtPatientDob.ErrorText = "";
                    this.CalulatePatientAge(dateValidObject.OutDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalulatePatientAge(string strDob)
        {
            try
            {
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        this.txtAge.EditValue = "";
                        this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    if (nam >= 7)
                    {
                        this.cboAge.EditValue = 1;
                        this.txtAge.Enabled = false;
                        this.cboAge.Enabled = false;
                        this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.cboAge.EditValue = 1;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                                this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                            }
                            else
                            {
                                this.txtAge.EditValue = nam * 12 - 1;
                                this.cboAge.EditValue = 2;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.txtAge.EditValue = nam * 12 + thang;
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.txtAge.EditValue = thang.ToString();
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.txtAge.EditValue = ngay.ToString();
                                this.cboAge.EditValue = 3;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.txtAge.EditValue = "";
                                this.cboAge.EditValue = 4;
                                this.txtAge.Enabled = true;
                                this.cboAge.Enabled = false;
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

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.txtPresUser.Text = searchResult[0].USERNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.dtIntructionTime.Focus();
                        }
                        else
                        {
                            txtPresUser.Focus();
                            txtPresUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPresUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.DropDown)
            {
                isShowContainer = !isShowContainer;
                if (isShowContainer)
                {
                    x = 0;
                    y = 0;
                    Control topParent = this;
                    while (topParent.Parent != null)
                    {
                        x += topParent.Left;
                        y += topParent.Top;
                        topParent = topParent.Parent;
                    }
                    Rectangle buttonBounds = new Rectangle(txtPresUser.Bounds.X + this.x, txtPresUser.Bounds.Y + this.y, txtPresUser.Bounds.Width, txtPresUser.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
                }
            }
        }

        private void txtPresUser_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ACS_USER user = gridViewPopupUser.GetFocusedRow() as ACS_USER;
                    if (user != null)
                    {
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                    }
                    GridUser_RowClick(user);
                    popupControlContainer1.HidePopup();
                    dtIntructionTime.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopupUser.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }
                    x = 0;
                    y = 0;
                    Control topParent = this;
                    while (topParent.Parent != null)
                    {
                        x += topParent.Left;
                        y += topParent.Top;
                        topParent = topParent.Parent;
                    }
                    Rectangle buttonBounds = new Rectangle(txtPresUser.Bounds.X + this.x, txtPresUser.Bounds.Y + this.y, txtPresUser.Bounds.Width, txtPresUser.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
                    gridViewPopupUser.Focus();
                    gridViewPopupUser.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtPresUser.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridUser_RowClick(ACS_USER user)
        {
            try
            {
                if (user != null)
                {
                    txtLoginName.Text = user.LOGINNAME;
                    txtPresUser.Text = user.USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPresUser_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtPresUser.Text) && txtPresUser.IsEditorActive)
                {
                    txtPresUser.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewPopupUser.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        gridViewPopupUser.ActiveFilterString = String.Format("[LOGINNAME] Like '%{0}%' OR [USERNAME] Like '%{0}%'", txtPresUser.Text);

                        gridViewPopupUser.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewPopupUser.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewPopupUser.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewPopupUser.FocusedRowHandle = 0;
                        gridViewPopupUser.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewPopupUser.OptionsFind.HighlightFindResults = true;
                        x = 0;
                        y = 0;
                        Control topParent = this;
                        while (topParent.Parent != null)
                        {
                            x += topParent.Left;
                            y += topParent.Top;
                            topParent = topParent.Parent;
                        }
                        Rectangle buttonBounds = new Rectangle(txtPresUser.Bounds.X + this.x, txtPresUser.Bounds.Y + this.y, txtPresUser.Bounds.Width, txtPresUser.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainer1.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
                            isShow = false;
                        }

                        txtPresUser.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewPopupUser.ActiveFilter.Clear();
                    if (!isShowContainer)
                    {
                        popupControlContainer1.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
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

        private void gridViewPopupUser_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ACS_USER tp = gridViewPopupUser.GetFocusedRow() as ACS_USER;
                    if (tp != null)
                    {
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridUser_RowClick(tp);
                    }
                    popupControlContainer1.HidePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPopupUser_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                ACS_USER tp = gridViewPopupUser.GetFocusedRow() as ACS_USER;
                if (tp != null)
                {
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridUser_RowClick(tp);
                }
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ddBtnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTdlPatientTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTdlPatientWorkPlace.Focus();
                    txtTdlPatientWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTdlPatientWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTdlPatientAccountNumber.Focus();
                    txtTdlPatientAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTdlPatientAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTdlPatientTaxCode.Focus();
                    txtTdlPatientTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintMess_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintMess.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintMess.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintMess.Name;
                    csAddOrUpdate.VALUE = (chkPrintMess.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkGuild_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkGuild.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkGuild.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkGuild.Name;
                    csAddOrUpdate.VALUE = (chkGuild.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkExp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkExp.Checked)
                {
                    lciAccountBook.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPayForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciUserRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    lciPayForm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    lciUserRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidationSingleControl(cboUserRoom, dxValidationProvider3);
                    ValidationSingleControl(cboAccountBook, dxValidationProvider3);
                    ValidationSingleControl(cboPayForm, dxValidationProvider3);
                }
                else
                {
                    lciAccountBook.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPayForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciUserRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationProvider3.SetValidationRule(cboAccountBook, null);
                    dxValidationProvider3.SetValidationRule(cboUserRoom, null);
                    dxValidationProvider3.SetValidationRule(cboPayForm, null);
                }
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;
                LoadDataToComboAccountBook();
                UpNumber();
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkExp.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkExp.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkExp.Name;
                    csAddOrUpdate.VALUE = (chkExp.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }     

        private void UpNumber()
        {
            try
            {
                if (cboAccountBook.EditValue != null && ListAccountBook != null && ListAccountBook.Count > 0)
                {
                    var checkGenTraction = ListAccountBook.FirstOrDefault(o => o.ID.ToString() == cboAccountBook.EditValue.ToString());
                    if (checkGenTraction.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        spnNumber.Enabled = true;
                    else
                        spnNumber.Enabled = false;
                    spnNumber.EditValue = (checkGenTraction.CURRENT_NUM_ORDER ?? 0) + 1;
                }
                else
                    spnNumber.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpNumber();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == cboAccountBook.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = cboAccountBook.EditValue != null ? cboAccountBook.EditValue.ToString() : "";
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = cboAccountBook.Name;
                    csAddOrUpdateValue.VALUE = cboAccountBook.EditValue != null ? cboAccountBook.EditValue.ToString() : "";
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentBySessionControlStateRDO == null)
                        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
            }
            catch (Exception  ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboUserRoom.EditValue != cboUserRoom.OldEditValue)
                {
                    WaitingManager.Show();
                    LoadDataToComboAccountBook();
                    LoadComboAccountBook();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
