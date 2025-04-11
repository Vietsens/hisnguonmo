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
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
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
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Config;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using System.Diagnostics;
using WCF.Client;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using MediMateTypeADO = HIS.Desktop.Plugins.ExpMestSaleCreate.ADO.MediMateTypeADO;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public delegate void RefreshCheckPrint();

    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private long roomId;
        private long roomTypeId;
        private int positionHandleControl = -1;
        List<HIS_ICD> currentIcds;
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        private V_HIS_MEDI_STOCK mediStock { get; set; }
        private MediMateTypeADO currentMediMate { get; set; }
        //private List<MediMateTypeADO> cloneCurrentMediMateList { get; set; }
        private MediMateTypeADO currentMediMateFocus { get; set; }
        private List<HisMedicineTypeInStockSDO> mediInStocks;
        private List<HisMaterialTypeInStockSDO> mateInStocks;
        private List<V_HIS_SERVICE_REQ_11> serviceReq { get; set; }
        private HisExpMestSaleListResultSDO resultSDO { get; set; }
        private List<HisExpMestSaleListResultSDO> ListResultSDO { get; set; }
        private string clientSessionKey { get; set; }
        //private RefreshCheckPrint _RefreshCheckPrint;
        private bool savePrint = false;
        public int Action { get; set; }
        private long? expMestId { get; set; }
        private List<long> expMestMedicineIds { get; set; }
        private List<long> expMestMaterialIds { get; set; }
        HIS.Desktop.Plugins.ExpMestSaleCreate.Base.GlobalDataStore.ModuleAction moduleAction { get; set; }

        private bool discountFocus { get; set; }
        private bool discountRatioFocus { get; set; }
        private bool discountDetailFocus { get; set; }
        private bool discountDetailRatioFocus { get; set; }

        bool isSearchOrderByXHT = false;
        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        bool isShowPpp = true;
        bool isTwoPatient = false;
        decimal disCountRatio = 0;
        long autoCheckIcd;
        bool isAutoCheckIcd;
        int x = 0;
        int y = 0;

        decimal oldDayNum = 1;
        string[] icdSeparators = new string[] { ",", ";" };
        //private Dictionary<long, MediMateTypeADO> dicMediMateAdo { get; set; }
        private Dictionary<long, List<MediMateTypeADO>> dicMediMateAdo { get; set; }

        bool IsUsingFunctionKeyInsteadOfCtrlKey = false;
        WcfClient cll;
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        MediMateTypeADO currentMedicineTypeADOForEdit;
        //readonly long taiQuayTrongGioID = 100000;
        //readonly long taiQuayNgoaiGioID = 100001;
        decimal? totalPayPrice = 0;
        decimal totalReceivable = 0;
        public static HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public static List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;

        private HIS_PATIENT Patient = null;
        private HIS_PATIENT_TYPE patientTypeConfig;
        string nameFile = "";
        private List<V_HIS_ACCOUNT_BOOK> listAccountBook = null;
        private bool IsShowDetails = false;
        bool IsDonCu = false;
        public UCExpMestSaleCreate(Inventec.Desktop.Common.Modules.Module moduleData, long? expMestId)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.roomId = moduleData.RoomId;
                this.roomTypeId = moduleData.RoomTypeId;
                this.expMestId = expMestId;
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestSaleCreate_Load(object sender, EventArgs e)
        {
            try
            {
                InitModuleAction();
                InitControlState();
                clientSessionKey = Guid.NewGuid().ToString();
                InitRestoreLayoutTreeListFromXml(treeListMediMate);
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.isAutoCheckIcd = (this.autoCheckIcd == 1);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                ValidControl();
                LoadMediStockFromRoomId();
                if (this.mediStock != null && this.mediStock.IS_BUSINESS != 1)
                {
                    CheckStockIsBusiness();
                    return;
                }

                LoadDataToGridMetyMatyTypeInStock();

                LoadMediMateFromMediStock();

                SetDataGridMediMaty(this.mediInStocks, this.mateInStocks);

                LoadDataToComboUser();
                LoadDataToCboCashierRoom();
                LoadCboBillAccountBook();
                LoadDataToControl();
                InitComboCommon(this.cboAge, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>(), "Id", "MoTa", 0, "", 0);
                this.Action = GlobalDataStore.ActionAdd;
                ResetControl();
                InitMenuPrint(null);
                EnableByCheckIsVisitor();
                // CheckStockIsBusiness();
                SetEnableControlPriceByCheckBox();
                LoadDataExpMestByEdit();

                EventTXH();

                this.IsUsingFunctionKeyInsteadOfCtrlKey = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ExpMestSaleCreate.IsUsingFunctionKeyInsteadOfCtrlKey") == "1" ? true : false;

                SetValueTitleBtn();

                SetLabelSave(this.moduleAction);
                //UCIcdInit();
                InitUcIcd();

                this.SetEnableButtonDebt(this.moduleAction == GlobalDataStore.ModuleAction.EDIT);

                txtPrescriptionCode.Focus();

                spinBaseValue.EditValue = HisConfigCFG.IS_ROUND_PRICE_BASE;
                CheckEnableBtnQR();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void UCIcdInit()
        //{
        //    try
        //    {
        //        DataToComboChuanDoanTD(cboIcds, this.currentIcds);
        //        chkIcd.Enabled = (this.autoCheckIcd != 2);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 596;
                ado.Height = 26;
                ado.AutoCheckIcd = (this.autoCheckIcd == 1);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                //ado.IsColor = false;
                this.ucIcd = (UserControl)icdProcessor.Run(ado);
                if (this.ucIcd != null)
                {
                    this.panelIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                    this.ucIcd.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
                }
                this.icdProcessor.ResetValidationIcd(ucIcd);
                this.icdProcessor.SetRequired(ucIcd, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                txtSubIcdCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DataToComboChuanDoanTD(GridLookUpEdit cbo, List<HIS_ICD> data)
        {
            try
            {

                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonDebt(bool enable)
        {
            try
            {
                btnDebt.Enabled = enable;
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
                if (!btnAdd.Enabled || !dxValidationProvider_Add.Validate() || this.currentMediMate == null || !CheckDataInput())
                    return;
                if (dicMediMateAdo == null)
                    dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();

                if (this.currentMediMateFocus == null && dicMediMateAdo.Count > 0 && !checkIsVisitor.Checked)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn phiếu xuất(Click chuột grid bên phải)", "Thông báo");
                    return;
                }

                this.currentMediMateFocus = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(treeListMediMate.FocusedNode);

                if (this.currentMediMateFocus != null)
                {
                    this.currentMediMate.ClientSessionKey = this.currentMediMateFocus.ClientSessionKey;
                    this.currentMediMate.SERVICE_REQ_CODE = this.currentMediMateFocus.SERVICE_REQ_CODE;
                    this.currentMediMate.PARENT_ID__IN_SETY = this.currentMediMateFocus.SERVICE_REQ_CODE;
                    this.currentMediMate.CONCRETE_ID__IN_SETY = this.currentMediMateFocus.SERVICE_REQ_CODE + "-" + this.currentMediMate.MEDI_MATE_TYPE_ID;
                    this.currentMediMate.EXP_MEST_ID = this.currentMediMateFocus.EXP_MEST_ID;
                    this.currentMediMate.EXP_MEST_CODE = this.currentMediMateFocus.EXP_MEST_CODE;
                    this.currentMediMate.TDL_PATIENT_NAME = this.currentMediMateFocus.TDL_PATIENT_NAME;
                }
                else
                {
                    this.currentMediMate.ClientSessionKey = clientSessionKey;
                    this.currentMediMate.SERVICE_REQ_CODE = "000000000000";
                    this.currentMediMate.PARENT_ID__IN_SETY = this.currentMediMate.SERVICE_REQ_CODE;
                    this.currentMediMate.CONCRETE_ID__IN_SETY = this.currentMediMate.SERVICE_REQ_CODE + "-" + this.currentMediMate.MEDI_MATE_TYPE_ID;
                }

                decimal amount = spinAmount.Value;
                List<long> beanIds = null;
                if (dicMediMateAdo.ContainsKey(this.currentMediMate.MEDI_MATE_TYPE_ID))
                {
                    MediMateTypeADO mediMateTypeADO = dicMediMateAdo[this.currentMediMate.MEDI_MATE_TYPE_ID].FirstOrDefault(p => p.SERVICE_REQ_CODE == this.currentMediMate.SERVICE_REQ_CODE);
                    if (mediMateTypeADO != null)
                    {
                        beanIds = mediMateTypeADO.BeanIds;
                        if (this.Action == GlobalDataStore.ActionAdd)
                        {
                            if (XtraMessageBox.Show(String.Format("Thuốc/vật tư: {0} đã được bổ sung. Bạn có muốn thay thế?", mediMateTypeADO.MEDI_MATE_TYPE_NAME), "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) != DialogResult.Yes)
                            {
                                if (txtMediMatyForPrescription.Enabled)
                                {
                                    txtMediMatyForPrescription.Focus();
                                    txtMediMatyForPrescription.SelectAll();
                                }
                                return;
                            }
                        }
                    }
                }

                if (this.currentMediMate.IsMedicine)
                {
                    TakeBeanMedicineProccess(beanIds, amount, ref success, currentMediMate.ClientSessionKey);
                }
                else if (this.currentMediMate.IsMaterial)
                {
                    TakeBeanMaterialProccess(beanIds, amount, ref success, currentMediMate.ClientSessionKey);
                }

                //Take false
                if (success == false)
                    return;
                //if (this.cloneCurrentMediMateList != null && this.cloneCurrentMediMateList.Count > 0)
                //{
                //    foreach (var mediMate in this.cloneCurrentMediMateList)
                //    {
                //        mediMate.IsClone = true;
                //        UpdateMediMateBeforeAdd(mediMate);

                //    }
                //}
                //else
                //{
                this.currentMediMate.IsClone = null;
                UpdateMediMateBeforeAdd(this.currentMediMate);
                //}

                LoadDataToGridExpMestDetail();
                //SetTotalPriceExpMestDetail();
                ResetControlAfterAddClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateMediMateBeforeAdd(MediMateTypeADO mediMate)
        {
            try
            {
                mediMate.IsNotInStock = false;
                mediMate.TUTORIAL = txtTutorial.Text;
                mediMate.NOTE = txtNote.Text;
                decimal amount = spinAmount.Value;

                decimal? totalPrice = 0;
                if (this.checkImpExpPrice.Checked)
                {
                    mediMate.EXP_VAT_RATIO = spinExpVatRatio.Value / 100;
                    if (spinExpPrice.EditValue != null)
                    // && spinExpPrice.Value > 0)
                    {
                        mediMate.EXP_PRICE = spinExpPrice.Value;
                        totalPrice = (amount * mediMate.EXP_PRICE) * (1 + mediMate.EXP_VAT_RATIO);
                    }
                    else
                    {
                        totalPrice = mediMate.TOTAL_PRICE;
                        mediMate.EXP_PRICE = totalPrice / amount;
                        if (spinProfit.EditValue != null)
                        {
                            mediMate.Profit = spinProfit.Value / 100;
                            totalPrice = totalPrice * (1 + spinProfit.Value / 100);
                        }
                    }

                    mediMate.IsCheckExpPrice = true;
                }
                else
                {
                    totalPrice = mediMate.TOTAL_PRICE;
                    mediMate.IsCheckExpPrice = false;
                    mediMate.EXP_PRICE = totalPrice / amount;
                }

                if (spinDiscountDetailRatio.EditValue != null && spinDiscountDetailRatio.Value > 0)
                {
                    totalPrice = totalPrice * (1 - spinDiscountDetailRatio.Value / 100);
                }

                mediMate.TOTAL_PRICE = totalPrice;
                mediMate.EXP_AMOUNT = amount;
                mediMate.DISCOUNT = spinDiscountDetail.Value;
                mediMate.DISCOUNT_RATIO = spinDiscountDetailRatio.Value / 100;
                if (spinDayNum.EditValue != null)
                    mediMate.DayNum = (long?)spinDayNum.Value;

                var lstmedimate = dicMediMateAdo.Values.SelectMany(s => s).Where(o => o.SERVICE_REQ_CODE == mediMate.SERVICE_REQ_CODE).ToList();
                if (lstmedimate != null && lstmedimate.Count > 0)
                {
                    mediMate.NUM_ORDER = lstmedimate.Max(o => o.NUM_ORDER ?? 0) > 0 ? lstmedimate.Max(o => o.NUM_ORDER ?? 0) + 1 : lstmedimate.Count() + 1;
                }
                else
                {
                    mediMate.NUM_ORDER = 1;
                }

                if (dicMediMateAdo.ContainsKey(mediMate.MEDI_MATE_TYPE_ID))
                {
                    MediMateTypeADO mediMateTypeADO = dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].FirstOrDefault(p => p.SERVICE_REQ_CODE == mediMate.SERVICE_REQ_CODE);
                    if (mediMateTypeADO != null)
                    {
                        dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].Remove(mediMateTypeADO);
                    }
                    dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].Add(mediMate);
                }
                else
                {
                    dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                    dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].Add(mediMate);
                }
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

        private void TakeBeanMedicineProccess(List<long> beanIds, decimal amount, ref bool success, string clientSessionKey)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.ClientSessionKey = clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                if (this.currentMediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { this.currentMediMate.ExpMestDetailId.Value };
                }
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisMedicineBean/Take INPUT takeBeanSDO ", takeBeanSDO));
                List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>("api/HisMedicineBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisMedicineBean/Take OUTPUT medicineBeans ", medicineBeans));
                if (medicineBeans == null || medicineBeans.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    return;
                }
                success = true;

                //if (!checkImpExpPrice.Checked || spinProfit.EditValue != null) nambg #46952
                //{
                //    this.cloneCurrentMediMateList = new List<MediMateTypeADO>();
                //    var groupMedicine = medicineBeans.GroupBy(x => new { x.TDL_MEDICINE_IMP_PRICE, x.TDL_MEDICINE_IMP_VAT_RATIO }).ToList();
                //    if (groupMedicine != null && groupMedicine.Count > 1)
                //    {
                //        foreach (var item in groupMedicine)
                //        {
                //            MediMateTypeADO mappingMediMate = new MediMateTypeADO();
                //            mappingMediMate = AutoMapper.Mapper.Map<MediMateTypeADO>(this.currentMediMate);
                //            mappingMediMate.TOTAL_PRICE = item.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                //            mappingMediMate.MEDI_MATE_ID = item.FirstOrDefault().MEDICINE_ID;
                //            this.cloneCurrentMediMateList.Add(mappingMediMate);
                //        }
                //    }
                //    else
                //    {
                this.currentMediMate.BeanIds = medicineBeans.Select(o => o.ID).ToList();
                this.currentMediMate.TOTAL_PRICE = medicineBeans.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialProccess(List<long> beanIds, decimal amount, ref bool success, string clientSessionKey)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.ClientSessionKey = clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                if (this.currentMediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { this.currentMediMate.ExpMestDetailId.Value };
                }

                //LogSystem.Debug(LogUtil.TraceData("Input Take Biin", takeBeanSDO));

                List<HIS_MATERIAL_BEAN> materialBeans = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>("api/HisMaterialBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                //LogSystem.Debug(LogUtil.TraceData("Outpu Take Biin", materialBeans));

                if (materialBeans == null || materialBeans.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    return;
                }
                success = true;

                //if (!checkImpExpPrice.Checked || (spinProfit.EditValue != null)) nambg #46952
                //{
                //    var groupMaterialBean = materialBeans.GroupBy(x => new { x.TDL_MATERIAL_IMP_PRICE, x.TDL_MATERIAL_IMP_VAT_RATIO }).ToList();
                //    if (groupMaterialBean != null && groupMaterialBean.Count > 1)
                //    {
                //        this.cloneCurrentMediMateList = new List<MediMateTypeADO>();
                //        foreach (var item in groupMaterialBean)
                //        {
                //            MediMateTypeADO mappingMediMate = new MediMateTypeADO();
                //            mappingMediMate = AutoMapper.Mapper.Map<MediMateTypeADO>(this.currentMediMate);
                //            mappingMediMate.TOTAL_PRICE = item.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                //            mappingMediMate.BeanIds = item.Select(o => o.ID).ToList();
                //            mappingMediMate.MEDI_MATE_ID = item.FirstOrDefault().MATERIAL_ID;
                //            this.cloneCurrentMediMateList.Add(mappingMediMate);
                //        }
                //    }
                //    else
                //    {
                this.currentMediMate.BeanIds = materialBeans.Select(o => o.ID).ToList();
                this.currentMediMate.TOTAL_PRICE = materialBeans.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                //    }
                //}
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMedicineProccess(MediMateTypeADO mediMate, decimal amount, string clientSessionKey)
        {
            try
            {
                //LogSystem.Debug("TAKEN BEAN amount " + amount);
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = mediMate.BeanIds;
                takeBeanSDO.ClientSessionKey = clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = mediMate.MEDI_MATE_TYPE_ID;
                if (mediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { mediMate.ExpMestDetailId.Value };
                }
                //Inventec.Common.Logging.LogSystem.Debug("TakeBeanMedicineProccess(MediMateTypeADO mediMate, decimal amount)___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDO), takeBeanSDO));
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

                if (amount <= mediMate.AVAILABLE_AMOUNT)
                {
                    mediMate.IsExceedsAvailable = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialProccess(MediMateTypeADO mediMate, decimal amount, string clientSessionKey)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = mediMate.BeanIds;
                takeBeanSDO.ClientSessionKey = clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()); ;
                takeBeanSDO.TypeId = mediMate.MEDI_MATE_TYPE_ID;
                if (mediMate.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { mediMate.ExpMestDetailId.Value };
                }
                //Inventec.Common.Logging.LogSystem.Debug("TakeBeanMaterialProccess(MediMateTypeADO mediMate, decimal amount)___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDO), takeBeanSDO));
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

                if (amount <= mediMate.AVAILABLE_AMOUNT)
                {
                    mediMate.IsExceedsAvailable = false;
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider_Save.Validate() || !CheckIsPrescription() || dicMediMateAdo == null || this.mediStock == null || !CheckValiDob())
                    return;
                btnSave.Focus();
                if (!IsValiICD())
                {
                    return;
                }
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref success, ref param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsValiICD()
        {
            bool result = true;
            try
            {
                result = (bool)icdProcessor.ValidationIcd(ucIcd);
                Inventec.Common.Logging.LogSystem.Debug("result" + result);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckValiDob()
        {
            bool result = true;
            try
            {
                long dtNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (String.IsNullOrEmpty(txtPatientDob.Text))
                {
                    dtPatientDob.EditValue = null;
                }
                else if (txtPatientDob.Text.Length == 4)
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
                //if (!checkIsVisitor.Checked && (this.serviceReq == null && (!this.expMestId.HasValue || this.expMestId.Value == 0)))
                //{
                //    MessageBox.Show(String.Format("Không tìm thấy đơn thuốc có mã : {0}", txtPrescriptionCode.Text));
                //    result = false;
                //}//xuandv
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ReleaseAll();
                ResetAllControl();
                SetControlByExpMest(null);
                SetAvaliable0MediMateStock();
                //LoadAvailabileMediMateInStock(null);
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.savePrint = true;
                btnSave_Click(null, null);
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
                //ValidControlPrescriptionCode();
                //txtPrescriptionCode.Text = "";
                //txtTreatmentCode.Text = "";
                //txtVirPatientName.Text = "";
                //txtAddress.Text = "";
                //txtPatientCode.Text = "";
                //txtPatientPhone.Text = "";
                //txtAge.Text = "";
                //cboAge.EditValue = null;
                //cboGender.EditValue = null;
                //txtPatientDob.EditValue = null;
                //dtPatientDob.EditValue = null;
                //txtMaTHX.Text = "";
                //cboTHX.EditValue = null;
                ReleaseAll();
                if (dicMediMateAdo != null)
                    dicMediMateAdo.Clear();

                this.moduleAction = GlobalDataStore.ModuleAction.ADD;

                txtPrescriptionCode.Text = "";
                txtTreatmentCode.Text = "";
                txtVirPatientName.Text = "";
                txtMediMatyForPrescription.Text = "";
                txtAge.Text = "";
                txtAddress.Text = "";
                txtDescription.Text = "";
                lblTotalPrice.Text = "";
                lblPayPrice.Text = "";
                lblPresNumber.Text = "";
                txtPatientCode.Text = "";
                txtPatientPhone.Text = "";
                txtMaTHX.Text = "";
                txtPresUser.Text = "";
                txtLoginName.Text = "";
                txtEmail.Text = "";
                treeListMediMate.DataSource = null;
                treeListResult.DataSource = null;
                checkIsVisitor.Enabled = true;
                btnSaleBill.Enabled = true;
                btnDebt.Enabled = false;
                btnAdd.Enabled = true;
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                btnCancelExport.Enabled = false;

                this.serviceReq = null;
                this.expMestId = null;
                this.Patient = null;
                this.expMestMaterialIds = null;
                this.expMestMedicineIds = null;
                this.discountFocus = false;
                this.discountRatioFocus = false;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;

                cboGender.EditValue = null;
                txtPatientDob.EditValue = null;
                dtPatientDob.EditValue = null;
                cboAge.EditValue = null;
                cboTHX.EditValue = null;
                //cboCashierRoom.EditValue = GlobalVariables.ExpmestSaleCreate__CashierRoomId;
                if (cboPayForm.EditValue == null)
                    cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                spinTransferAmount.EditValue = null;

                EnableByCheckIsVisitor();
                if (!checkIsVisitor.Checked)
                {
                    txtPrescriptionCode.Focus();
                    txtPrescriptionCode.SelectAll();
                }
                else
                {
                    txtPatientCode.Focus();
                    txtPatientCode.SelectAll();
                }
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
                    spinDiscountDetailRatio.Enabled = true;
                    spinProfit.Enabled = true;
                }
                else
                {
                    spinExpPrice.Enabled = false;
                    spinExpVatRatio.Enabled = false;
                    spinDiscountDetail.Enabled = false;
                    spinDiscountDetailRatio.Enabled = false;
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
                    if (checkIsVisitor.Visible && checkIsVisitor.Enabled)
                    {
                        checkIsVisitor.Focus();
                    }
                    else if (txtPrescriptionCode.Visible && txtPrescriptionCode.Enabled)
                    {
                        txtPrescriptionCode.Focus();
                        txtPrescriptionCode.SelectAll();
                    }
                    else if (txtTreatmentCode.Visible && txtTreatmentCode.Enabled)
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }

                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }

                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }

                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }

                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }

                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    SetAvaliable0MediMateStock();//xuandv
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

        private void spinDiscount_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscountRatio.Focus();
                    spinDiscountRatio.SelectAll();
                    if (spinDiscount.EditValue == null)
                    {
                        spinDiscountRatio.Focus();
                        spinDiscountRatio.SelectAll();
                    }
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
                        var datas = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                        totalPrice = datas.Sum(p => p.TOTAL_PRICE ?? 0);// dicMediMateAdo.Sum(o => o.Value.TOTAL_PRICE ?? 0);
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
                        var datas = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                        totalPrice = datas.Sum(o => o.TOTAL_PRICE ?? 0);
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

        private void btnCtrlA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void btnCtrlI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSavePrint_Click(null, null);
        }

        private void btnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnCtrlN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void barButtonItemCtrlF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMediMatyForPrescription.Enabled)
                {
                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                            e.Value = Inventec.Common.Number.Convert.NumberToString((mediMateTypeADO.TOTAL_PRICE ?? 0) / mediMateTypeADO.EXP_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
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
                if (data != null)/// && serviceReq != null && serviceReq.Select(s => s.TREATMENT_ID).Distinct().Count() <= 1)
                {
                    btnSaleBill.Enabled = false;
                    btnAdd.Enabled = false;
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    btnDebt.Enabled = false;
                    InitMenuPrint(this.resultSDO.ExpMestSdos.FirstOrDefault().ExpMest);
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
                    if (isTwoPatient)
                    {
                        if (this.ListResultSDO != null && this.ListResultSDO.Count > 1)
                        {
                            for (int i = 0; i < ListResultSDO.Count; i++)
                            {
                                moduleData.RoomId = this.roomId;
                                moduleData.RoomTypeId = this.roomTypeId;
                                List<object> listArgs = new List<object>();
                                listArgs.Add(moduleData);
                                listArgs.Add(this.ListResultSDO[i].ExpMestSdos.Select(p => p.ExpMest).Select(p => p.ID).Distinct().ToList());
                                listArgs.Add((DelegateSelectData)EnableControlAfterSaveSaleBill);
                                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                                if (extenceInstance == null)
                                {
                                    throw new ArgumentNullException("extenceInstance is null");
                                }

                                ((Form)extenceInstance).ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        moduleData.RoomId = this.roomId;
                        moduleData.RoomTypeId = this.roomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        listArgs.Add(this.resultSDO.ExpMestSdos.Select(p => p.ExpMest).Select(p => p.ID).Distinct().ToList());
                        listArgs.Add((DelegateSelectData)EnableControlAfterSaveSaleBill);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
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
                    //spinExpPrice.Focus();
                    //spinExpPrice.SelectAll();

                    if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                this.InitComboCommon(this.cboTHX, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>(), "ID_RAW", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSourceValueTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                if (communeADOs != null)
                    this.InitComboCommon(this.cboTHX, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
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
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? 0).ToString());
                        if (commune != null)
                        {
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;

                            SetAddress(commune);
                        }
                    }
                    else
                    {
                        //this.txtMaTHX.Focus();
                        //this.txtMaTHX.SelectAll();

                        if (txtMaTHX.Visible && txtMaTHX.Enabled)
                        {
                            txtMaTHX.Focus();
                            txtMaTHX.SelectAll();
                        }
                        else if (cboTHX.Visible && cboTHX.Enabled)
                        {
                            cboTHX.Focus();
                            cboTHX.SelectAll();
                        }
                        else if (txtAddress.Visible && txtAddress.Enabled)
                        {
                            txtAddress.Focus();
                            txtAddress.SelectAll();
                        }
                        else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                        {
                            txtPatientPhone.Focus();
                            txtPatientPhone.SelectAll();
                        }
                        else if (txtEmail.Visible && txtEmail.Enabled)
                        {
                            txtEmail.Focus();
                            txtEmail.SelectAll();
                        }
                        else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                        {
                            dtIntructionTime.Focus();
                            dtIntructionTime.SelectAll();
                        }
                        else if (txtLoginName.Visible && txtLoginName.Enabled)
                        {
                            txtLoginName.Focus();
                            txtLoginName.SelectAll();
                        }
                        else if (txtPresUser.Visible && txtPresUser.Enabled)
                        {
                            txtPresUser.Focus();
                            txtPresUser.SelectAll();
                        }
                        else if (txtDescription.Visible && txtDescription.Enabled)
                        {
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                        else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                        {
                            txtMediMatyForPrescription.Focus();
                            txtMediMatyForPrescription.Show();
                        }
                        else if (spinAmount.Visible && spinAmount.Enabled)
                        {
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else if (spinDayNum.Visible && spinDayNum.Enabled)
                        {
                            spinDayNum.Focus();
                            spinDayNum.SelectAll();
                        }
                        else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                        {
                            checkImpExpPrice.Focus();
                        }

                        else if (spinProfit.Visible && spinProfit.Enabled)
                        {
                            spinProfit.Focus();
                            spinProfit.SelectAll();
                        }
                        else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                        {
                            spinExpPrice.Focus();
                            spinExpPrice.SelectAll();
                        }

                        else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                        {
                            spinExpVatRatio.Focus();
                            spinExpVatRatio.SelectAll();
                        }

                        else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }

                        else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                        {
                            spinDiscountDetailRatio.Focus();
                            spinDiscountDetailRatio.SelectAll();
                        }

                        else if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                        }
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
                        gridViewPopupUser.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
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

        private void treeListMediMate_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(e.Node);
                if (data != null && dicMediMateAdo != null)
                {

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo), dicMediMateAdo));
                    if (e.Node.HasChildren)
                    {
                        var datas = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                        datas = datas.Where(p => p.PARENT_ID__IN_SETY == data.CONCRETE_ID__IN_SETY).ToList();
                        foreach (var item in datas)
                        {
                            DeleteRowExpMestDetail(item);
                        }
                    }
                    else
                    {
                        DeleteRowExpMestDetail(data);
                    }

                    LoadDataToGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                //if (e.Node.HasChildren)
                //{
                if (e.Node.Checked)
                {
                    e.Node.UncheckAll();
                }
                else
                {
                    e.Node.CheckAll();
                }
                TreeListNode node = e.Node;
                CheckNodesParent(node);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNodesParent(TreeListNode node)
        {
            if (node != null)
            {
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    bool hasCheck = false;
                    bool allCheck = true;
                    foreach (TreeListNode item in node.Nodes)
                    {
                        if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                        {
                            hasCheck = true;
                        }
                        if (item.CheckState == CheckState.Unchecked || item.CheckState == CheckState.Indeterminate)
                        {
                            allCheck = false;
                        }
                    }
                    if (allCheck)
                    {
                        node.CheckState = CheckState.Checked;
                    }
                    else if (hasCheck)
                    {
                        node.CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        node.CheckState = CheckState.Unchecked;
                    }
                }
            }
        }

        private void treeListMediMate_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(e.Node);
                if (data != null && !e.Node.HasChildren)
                {
                    if (e.Column.FieldName == "AMOUNT_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(data.EXP_AMOUNT ?? 0, 2);
                    }
                    else if (e.Column.FieldName == "EXP_PRICE_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(data.EXP_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "EXP_VAT_RATIO_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.EXP_VAT_RATIO ?? 0) * 100, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE_STR")
                    {
                        e.Value = (data.TOTAL_PRICE.HasValue)
                            ? Inventec.Common.Number.Convert.NumberToString(data.TOTAL_PRICE.Value, ConfigApplications.NumberSeperator)
                            : null;
                    }
                    else if (e.Column.FieldName == "PRICE_WITH_VAT")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.EXP_PRICE ?? 0) * (1 + (data.EXP_VAT_RATIO ?? 0)), ConfigApplications.NumberSeperator);
                    }
                }
                if (e.Column.FieldName == "MEDI_MATE_TYPE_NAME_STR" && data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        int nodeIndex = e.Node.ParentNode.Nodes.IndexOf(e.Node);
                        e.Value = (nodeIndex + 1) + " - " + data.MEDI_MATE_TYPE_NAME;
                    }
                    else
                        e.Value = data.MEDI_MATE_TYPE_NAME;
                }
                if (e.Column.FieldName == "TOTAL_PRICE_STR")
                {
                    if (e.Node.HasChildren)
                    {
                        this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "TOTAL_PRICE_STR");
                        e.Value = (data.TOTAL_PRICE.HasValue)
                            ? Inventec.Common.Number.Convert.NumberToString(data.TOTAL_PRICE.Value, ConfigApplications.NumberSeperator)
                            : null;
                        data.KEY_PRICE_PARENT = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                List<MediMateTypeADO> dataCheckTree = GetListCheck();
                if (dataCheckTree != null && dataCheckTree.Count > 0)
                {
                    this.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MediMateTypeADO> GetListCheck()
        {
            List<MediMateTypeADO> result = new List<MediMateTypeADO>();
            try
            {
                // GetListNodeCheck(ref result, node);
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = new List<MediMateTypeADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MediMateTypeADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MediMateTypeADO)treeListMediMate.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_Click(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                if (hi.Node != null)
                {
                    this.currentMediMateFocus = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(hi.Node);
                    if (!hi.Node.HasChildren)
                    {
                        this.currentMediMate = this.currentMediMateFocus;
                        EditRowExpMestDetail(this.currentMediMateFocus);
                    }
                    else // nambg
                    {
                        var currentMediMateChild = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(hi.Node.ParentNode);
                        this.currentMediMate = currentMediMateChild;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(e.Node);
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.IsNotInStock || data.IsExceedsAvailable)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
                if (e.Node.HasChildren)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren && e.Column.FieldName == "EXP_AMOUNT")
                {
                    e.RepositoryItem = repositoryItemSpinEdit__Amount;
                }
                else if (!e.Node.HasChildren && e.Column.FieldName == "ERROR_VIEW")
                {
                    var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(e.Node);
                    if (data != null && !e.Node.HasChildren)
                    {
                        if (data.IsNotInStock || data.IsExceedsAvailable)
                        {
                            e.RepositoryItem = repositoryItemBtnView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListMediMate_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren && e.Column.FieldName == "EXP_AMOUNT")
                {
                    var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(e.Node);
                    if (data != null)
                    {
                        if (data.IsMedicine)
                        {
                            TakeBeanMedicineProccess(data, data.EXP_AMOUNT ?? 0, data.ClientSessionKey);
                        }
                        else
                        {
                            TakeBeanMaterialProccess(data, data.EXP_AMOUNT ?? 0, data.ClientSessionKey);
                        }
                        SetTotalPriceExpMestDetail();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueTitleBtn()
        {
            try
            {
                if (this.IsUsingFunctionKeyInsteadOfCtrlKey)
                {
                    btnSavePrint.Text = "Lưu in (F9)";
                    btnSave.Text = "Lưu (F5)";
                    btnNew.Text = "Mới (F8)";
                    btnSaleBill.Text = "Xuất hóa đơn (F10)";
                    btnNewExpMest.Text = "Đơn mới (F7)";
                }
                else
                {
                    btnSavePrint.Text = "Lưu in (Ctrl I)";
                    btnSave.Text = "Lưu (Ctrl S)";
                    btnNew.Text = "Mới (Ctrl N)";
                    btnSaleBill.Text = "Xuất hóa đơn (Ctrl T)";
                    btnNewExpMest.Text = "Đơn mới (Ctrl D)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
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
                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X + this.x, txtMediMatyForPrescription.Bounds.Y + this.y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));

                        if (this.currentMedicineTypeADOForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMateTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
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

        private void txtMediMatyForPrescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();

                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
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
                    Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X + this.x, txtMediMatyForPrescription.Bounds.Y + this.y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMediMatyForPrescription.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MetyMatyTypeInStock_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMateTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMateTypeADO>(this.currentMedicineTypeADOForEdit, data);
                    txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDI_MATE_TYPE_NAME;

                    this.currentMediMate = this.currentMedicineTypeADOForEdit;
                    SetControlMediMateClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    txtMediMatyForPrescription.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDI_MATE_TYPE_NAME] Like '%{0}%' OR [MEDI_MATE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%'", txtMediMatyForPrescription.Text);
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        x = 0;
                        y = 0;
                        Control topParent = this;
                        while (topParent.Parent != null)
                        {
                            x += topParent.Left;
                            y += topParent.Top;
                            topParent = topParent.Parent;
                        }
                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X + this.x, txtMediMatyForPrescription.Bounds.Y + this.y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        if (isShowPpp)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
                            isShowPpp = false;
                        }

                        txtMediMatyForPrescription.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                //this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShowPpp = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                //if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                //{
                //    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is MediMatyTypeADO)
                //    {
                //        MediMatyTypeADO data = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                //        if (data != null)
                //        {

                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                //DMediStock1ADO dMediStock = gridViewMediMaty.GetRow(e.RowHandle) as DMediStock1ADO;
                //if (dMediStock != null && (dMediStock.IS_STAR_MARK ?? 0) == 1)
                //{
                //    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                if (medicineTypeADOForEdit != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ResetFocusMediMaty(bool isFocus, bool fucusOnly = false)
        {
            try
            {
                if (!fucusOnly)
                {
                    currentMedicineTypeADOForEdit = null;
                    txtMediMatyForPrescription.Text = "";
                }

                if (isFocus)
                    txtMediMatyForPrescription.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridMetyMatyTypeInStock()
        {
            try
            {
                this.RebuildMediMatyWithInControlContainer();

                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildMediMatyWithInControlContainer()
        {
            try
            {
                // this.InitDataMetyMatyTypeInStockD();//Get data
                popupControlContainerMediMaty.Width = 1130;
                popupControlContainerMediMaty.Height = 200;

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDI_MATE_TYPE_CODE";
                col1.Caption = "Mã";
                col1.Width = 100;
                col1.VisibleIndex = 0;
                col1.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDI_MATE_TYPE_NAME";
                col2.Caption = "Tên";
                col2.Width = 300;
                col2.VisibleIndex = 1;
                col2.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn colNew = new DevExpress.XtraGrid.Columns.GridColumn();
                colNew.FieldName = "ACTIVE_INGR_BHYT_NAME";
                colNew.Caption = "Hoạt chất";
                colNew.Width = 150;
                colNew.VisibleIndex = 2;
                colNew.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(colNew);

                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME";
                col3.Caption = "ĐVT";
                col3.Width = 100;
                col3.VisibleIndex = 3;
                col3.OptionsColumn.AllowEdit = false;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = "Hàm lượng";
                col8.Width = 150;
                col8.VisibleIndex = 4;
                col8.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col8);


                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = "Tên BHYT";
                col7.Width = 160;
                col7.VisibleIndex = 5;
                col7.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col7);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AVAILABLE_AMOUNT";
                col4.Caption = "Khả dụng";
                col4.Width = 150;
                col4.VisibleIndex = 6;
                col4.OptionsColumn.AllowEdit = false;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col4);


                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MANUFACTURER_NAME";
                col10.Caption = "Hãng SX";
                col10.Width = 150;
                col10.VisibleIndex = 12;
                col10.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "NATIONAL_NAME";
                col11.Caption = "Quốc gia";
                col11.Width = 100;
                col11.VisibleIndex = 13;
                col11.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col11);

                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col14);

                gridViewMediMaty.GridControl.DataSource = null;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancelExport.Enabled) return;
                if (this.resultSDO == null || this.resultSDO.ExpMestSdos == null || this.resultSDO.ExpMestSdos.Count <= 0)
                {
                    XtraMessageBox.Show("Không có phiếu xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                var needUnexports = this.resultSDO.ExpMestSdos.Where(o => o.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE).ToList();
                if (needUnexports == null || needUnexports.Count <= 0)
                {
                    XtraMessageBox.Show("Phiếu xuất chưa được thực xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                bool valid = true;
                foreach (var expMest in needUnexports)
                {
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = expMest.ExpMest.ID;
                    sdo.ReqRoomId = this.currentModuleBase.RoomId;

                    var rs = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unexport", ApiConsumers.MosConsumer, sdo, param);
                    if (rs == null)
                    {
                        valid = false;
                        break;
                    }
                    else
                    {
                        expMest.ExpMest.EXP_MEST_STT_ID = rs.EXP_MEST_STT_ID;

                        HisExpMestSDO dataUn = new HisExpMestSDO();
                        dataUn.ExpMestId = expMest.ExpMest.ID;
                        dataUn.ReqRoomId = this.currentModuleBase.RoomId;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, dataUn, param);

                        if (apiresul == null)
                        {
                            valid = false;
                            break;
                        }
                        else
                        {
                            expMest.ExpMest.EXP_MEST_STT_ID = apiresul.EXP_MEST_STT_ID;
                        }
                    }
                }
                success = valid;
                if (success)
                {
                    ReloadDataDicBeforSave();//xuandv
                    LoadDataExpMestByEditMulti(expMestDones, null, false);
                    btnCancelExport.Enabled = false;
                    btnSave.Enabled = true;
                    btnSavePrint.Enabled = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonCancelExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancelExport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestSaleCreate_Leave(object sender, EventArgs e)
        {
            try
            {
                lblTotalPrice.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool ProcessReleaseAllMedi()
        {
            bool result = true;
            try
            {
                var dt = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                if (dt != null && dt.Count > 0)
                {
                    var groupClientSessionKey = dt.GroupBy(o => o.ClientSessionKey).ToList();
                    foreach (var clientSessionKey in groupClientSessionKey)
                    {
                        CommonParam param = new CommonParam();
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMedi => Goi api release all bean, uri = " + RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey));
                        result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                            RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL,
                            ApiConsumers.MosConsumer,
                            clientSessionKey.Key,
                            HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                            param);

                        if (!result)
                        {
                            //MessageManager.Show(param, result);
                            Inventec.Common.Logging.LogSystem.Debug("Goi api release list medicine bean that bai, url = " + RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal bool ProcessReleaseAllMaty()
        {
            bool result = true;
            try
            {
                var dt = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                if (dt != null && dt.Count > 0)
                {
                    var groupClientSessionKey = dt.GroupBy(o => o.ClientSessionKey).ToList();
                    foreach (var clientSessionKey in groupClientSessionKey)
                    {
                        CommonParam param = new CommonParam();
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessReleaseAllMaty => Goi api release all bean, uri = " + RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey));
                        result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(
                            RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL,
                            ApiConsumers.MosConsumer,
                            clientSessionKey.Key,
                            HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                            param);

                        if (!result)
                        {
                            //MessageManager.Show(param, result);
                            Inventec.Common.Logging.LogSystem.Debug("Goi api release list material bean that bai, url = " + RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL + ". Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void treeListResult_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)this.treeListResult.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (String.IsNullOrEmpty(data.PARENT_ID))
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListResult_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                MediMateTypeADO currentRow = e.Row as MediMateTypeADO;
                if (currentRow == null) return;
                if (e.IsGetData && e.Column.FieldName == "TOTAL_PRICE_DISPLAY" && !String.IsNullOrEmpty(currentRow.PARENT_ID))
                {
                    e.Value = Inventec.Common.Number.Convert.NumberToString(currentRow.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                }
                else if (e.IsGetData && e.Column.FieldName == "EXP_PRICE_DISPLAY" && !String.IsNullOrEmpty(currentRow.PARENT_ID))
                {
                    e.Value = Inventec.Common.Number.Convert.NumberToString((currentRow.TOTAL_PRICE ?? 0) / currentRow.EXP_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNewExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                ReleaseAll();
                ResetNewControl();
                SetControlByExpMest(null);

                //LoadAvailabileMediMateInStock(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetNewControl()
        {
            try
            {
                if (btnQr.Visible)
                    btnQr.Enabled = false;
                this.currentMediMateFocus = null;
                this.moduleAction = GlobalDataStore.ModuleAction.ADD;
                txtMediMatyForPrescription.Text = "";
                this.expMestId = null;
                lblTotalPrice.Text = "";
                lblPayPrice.Text = "";
                lblPresNumber.Text = "";
                this.icdProcessor.Reload(ucIcd, null);
                this.icdProcessor.ResetValidate(ucIcd);
                this.icdProcessor.SetRequired(ucIcd, false);
                txtIcd.Text = "";
                //txtIcdCode.Text = "";
                txtSubIcdCode.Text = "";
                treeListMediMate.DataSource = null;
                this.discountFocus = false;
                this.discountRatioFocus = false;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;
                dicMediMateAdo.Clear();
                treeListResult.DataSource = null;
                this.expMestMaterialIds = null;
                this.expMestMedicineIds = null;
                btnSaleBill.Enabled = true;
                btnDebt.Enabled = false;
                btnAdd.Enabled = true;
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                btnCancelExport.Enabled = false;
                //this.expMestResult = new HisExpMestResultSDO();
                //this.expMestResult.ExpMest = new HIS_EXP_MEST();
                this.SetLabelSave(GlobalDataStore.ModuleAction.ADD);
                LoadMediMateFromMediStock();
                spinDiscount.EditValue = null;
                spinTransferAmount.EditValue = null;
                spinDiscountRatio.EditValue = null;
                checkImpExpPrice.Enabled = this.currentMediMate != null ? true : false;
                dtIntructionTime.DateTime = DateTime.Now;
                spinDayNum.EditValue = null;
                oldDayNum = 1;
                ResetControlAfterAddClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPayForm.EditValue != null)
                {
                    var payform = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == (long)cboPayForm.EditValue);
                    if (payform != null)
                    {
                        CheckPayFormTienMatChuyenKhoan(payform);
                    }
                    spinTransferAmount.Text = "";
                    if ((payform.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE))
                    {
                        if (lblPayPrice.Text != null)
                        {

                            //spinTransferAmount.Value = long.Parse(lblPayPrice.Text);
                            spinTransferAmount.Text = lblPayPrice.Text;

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintNow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_PRINT_NOW && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintNow.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_PRINT_NOW;
                    csAddOrUpdate.VALUE = (chkPrintNow.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                WaitingManager.Hide();
                //if (this._RefreshCheckPrint != null)
                //{
                //    this._RefreshCheckPrint();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtPatientDob.EditValue != null)
                {
                    this.dtPatientDob.Update();
                    this.txtPatientDob.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    this.CalulatePatientAge(this.txtPatientDob.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDebt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDebt.Enabled)
                {
                    return;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DrugStoreDebt").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DrugStoreDebt'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    if (this.resultSDO == null || this.resultSDO.ExpMestSdos != null && this.resultSDO.ExpMestSdos.Count > 0)
                    {
                        listArgs.Add(this.resultSDO.ExpMestSdos.Select(p => p.ExpMest).Select(p => p.ID).Distinct().ToList());
                    }
                    else if (dicMediMateAdo.Count > 0)
                    {
                        var dataTrees = dicMediMateAdo.Select(o => o.Value).ToList();
                        var dataCoverts = dataTrees.SelectMany(p => p).Distinct().OrderByDescending(o => o.TOTAL_PRICE).ToList();
                        listArgs.Add(dataCoverts.Where(o => o.EXP_MEST_ID.HasValue).Select(s => s.EXP_MEST_ID.Value).Distinct().ToList());
                    }
                    else
                    {
                        throw new ArgumentNullException("ExpMestIds is null");
                    }
                    listArgs.Add((DelegateSelectData)EnableControlAfterDept);
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

        private void EnableControlAfterDept(object data)
        {
            try
            {
                if (data != null)
                {
                    btnSaleBill.Enabled = false;
                    btnAdd.Enabled = false;
                    btnSave.Enabled = false;
                    btnDebt.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnView_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (dicMediMateAdo != null)
                {
                    var notAvailable = dicMediMateAdo.Values.SelectMany(s => s).Where(o => o.IsNotInStock || o.IsExceedsAvailable).ToList();
                    if (notAvailable != null && notAvailable.Count > 0)
                    {
                        var materials = notAvailable.Where(o => o.IsMaterial).ToList();
                        var medicines = notAvailable.Where(o => o.IsMedicine).ToList();

                        var data = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(treeListMediMate.FocusedNode);
                        FormInStock.FormMediMateInStock form = new FormInStock.FormMediMateInStock(medicines, materials, data);
                        if (form != null)
                        {
                            form.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoShow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == chkAutoShow.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoShow.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoShow.Name;
                    csAddOrUpdate.VALUE = (chkAutoShow.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Immediate || e.CloseMode == PopupCloseMode.Normal)
                {
                    //txtPatientDob.Focus();
                    //txtPatientDob.SelectAll();

                    if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }

                    if (!String.IsNullOrWhiteSpace(txtVirPatientName.Text) && cboGender.EditValue != null && !String.IsNullOrWhiteSpace(txtPatientDob.Text))
                    {
                        ProcessSearchPatientByInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (txtLoginName.Visible && txtLoginName.Enabled)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
                else if (txtPresUser.Visible && txtPresUser.Enabled)
                {
                    txtPresUser.Focus();
                    txtPresUser.SelectAll();
                }
                else if (txtDescription.Visible && txtDescription.Enabled)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
                else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                {
                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.Show();
                }
                else if (spinAmount.Visible && spinAmount.Enabled)
                {
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
                else if (spinDayNum.Visible && spinDayNum.Enabled)
                {
                    spinDayNum.Focus();
                    spinDayNum.SelectAll();
                }
                else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                {
                    checkImpExpPrice.Focus();
                }
                else if (spinProfit.Visible && spinProfit.Enabled)
                {
                    spinProfit.Focus();
                    spinProfit.SelectAll();
                }
                else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                {
                    spinExpPrice.Focus();
                    spinExpPrice.SelectAll();
                }
                else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                {
                    spinExpVatRatio.Focus();
                    spinExpVatRatio.SelectAll();
                }
                else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                {
                    spinDiscountDetail.Focus();
                    spinDiscountDetail.SelectAll();
                }
                else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                {
                    spinDiscountDetailRatio.Focus();
                    spinDiscountDetailRatio.SelectAll();
                }
                else if (txtTutorial.Visible && txtTutorial.Enabled)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
                else if (txtNote.Visible && txtNote.Enabled)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
                else if (btnAdd.Visible && btnAdd.Enabled)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            try
            {
                spinDiscount.Focus();
                spinDiscount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.discountFocus = false;
                    this.discountRatioFocus = true;
                    decimal totalPrice = 0;
                    if (dicMediMateAdo != null)
                    {
                        var datas = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                        totalPrice = datas.Sum(o => o.TOTAL_PRICE ?? 0);
                    }
                    DiscountDisplayProcess(this.discountFocus, this.discountRatioFocus, spinDiscount, spinDiscountRatio, totalPrice);
                    SetTotalPriceExpMestDetail();
                    btnSave.Focus();
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountDetail_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscountDetail.EditValue == null)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                        e.Handled = true;
                    }
                    else
                    {
                        //txtTutorial.Focus();
                        //txtTutorial.SelectAll();
                        if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinExpPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //spinExpVatRatio.Focus();
                    //spinExpVatRatio.SelectAll();

                    if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                        e.Handled = true;
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinProfit_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //spinExpPrice.Focus();
                    //spinExpPrice.SelectAll();

                    if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkImpExpPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                    }
                    else if (this.currentMediMate != null)
                    {
                        if (this.currentMediMate.IsMedicine && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }
                        else
                        {
                            if (spinProfit.Visible && spinProfit.Enabled)
                            {
                                spinProfit.Focus();
                                spinProfit.SelectAll();
                            }
                            else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                            {
                                spinExpPrice.Focus();
                                spinExpPrice.SelectAll();
                            }
                            else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                            {
                                spinExpVatRatio.Focus();
                                spinExpVatRatio.SelectAll();
                            }
                            else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                            {
                                spinDiscountDetail.Focus();
                                spinDiscountDetail.SelectAll();
                            }
                            else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                            {
                                spinDiscountDetailRatio.Focus();
                                spinDiscountDetailRatio.SelectAll();
                            }
                            else if (txtTutorial.Visible && txtTutorial.Enabled)
                            {
                                txtTutorial.Focus();
                                txtTutorial.SelectAll();
                            }
                            else if (txtNote.Visible && txtNote.Enabled)
                            {
                                txtNote.Focus();
                                txtNote.SelectAll();
                            }
                            else if (btnAdd.Visible && btnAdd.Enabled)
                            {
                                btnAdd.Focus();
                                e.Handled = true;
                            }
                        }
                    }
                    else
                    {
                        if (spinProfit.Visible && spinProfit.Enabled)
                        {
                            spinProfit.Focus();
                            spinProfit.SelectAll();
                        }
                        else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                        {
                            spinExpPrice.Focus();
                            spinExpPrice.SelectAll();
                        }
                        else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                        {
                            spinExpVatRatio.Focus();
                            spinExpVatRatio.SelectAll();
                        }
                        else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }
                        else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                        {
                            spinDiscountDetailRatio.Focus();
                            spinDiscountDetailRatio.SelectAll();
                        }
                        else if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDayNum_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //dtIntructionTime.Focus();
                    //dtIntructionTime.SelectAll();

                    if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaTHX_KeyDown(object sender, KeyEventArgs e)
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
                            this.cboTHX.EditValue = dataNoCommunes[0].ID_RAW;
                            this.txtMaTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            SetAddress(dataNoCommunes[0]);
                        }
                        else if (listResult.Count == 1)
                        {
                            this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID_RAW;
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
                        if (cboTHX.Visible && cboTHX.Enabled)
                        {
                            cboTHX.Focus();
                            cboTHX.SelectAll();
                        }
                        else if (txtAddress.Visible && txtAddress.Enabled)
                        {
                            txtAddress.Focus();
                            txtAddress.SelectAll();
                        }
                        else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                        {
                            txtPatientPhone.Focus();
                            txtPatientPhone.SelectAll();
                        }
                        else if (txtEmail.Visible && txtEmail.Enabled)
                        {
                            txtEmail.Focus();
                            txtEmail.SelectAll();
                        }
                        else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                        {
                            dtIntructionTime.Focus();
                            dtIntructionTime.SelectAll();
                        }
                        else if (txtLoginName.Visible && txtLoginName.Enabled)
                        {
                            txtLoginName.Focus();
                            txtLoginName.SelectAll();
                        }
                        else if (txtPresUser.Visible && txtPresUser.Enabled)
                        {
                            txtPresUser.Focus();
                            txtPresUser.SelectAll();
                        }
                        else if (txtDescription.Visible && txtDescription.Enabled)
                        {
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                        else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                        {
                            txtMediMatyForPrescription.Focus();
                            txtMediMatyForPrescription.Show();
                        }
                        else if (spinAmount.Visible && spinAmount.Enabled)
                        {
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else if (spinDayNum.Visible && spinDayNum.Enabled)
                        {
                            spinDayNum.Focus();
                            spinDayNum.SelectAll();
                        }
                        else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                        {
                            checkImpExpPrice.Focus();
                        }
                        else if (spinProfit.Visible && spinProfit.Enabled)
                        {
                            spinProfit.Focus();
                            spinProfit.SelectAll();
                        }
                        else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                        {
                            spinExpPrice.Focus();
                            spinExpPrice.SelectAll();
                        }
                        else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                        {
                            spinExpVatRatio.Focus();
                            spinExpVatRatio.SelectAll();
                        }
                        else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }
                        else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                        {
                            spinDiscountDetailRatio.Focus();
                            spinDiscountDetailRatio.SelectAll();
                        }
                        else if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_KeyDown(object sender, KeyEventArgs e)
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
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                    }
                    else
                    {
                        //this.txtMaTHX.Focus();
                        //this.txtMaTHX.SelectAll();

                        if (txtMaTHX.Visible && txtMaTHX.Enabled)
                        {
                            txtMaTHX.Focus();
                            txtMaTHX.SelectAll();
                        }
                        else if (cboTHX.Visible && cboTHX.Enabled)
                        {
                            cboTHX.Focus();
                            cboTHX.SelectAll();
                        }
                        else if (txtAddress.Visible && txtAddress.Enabled)
                        {
                            txtAddress.Focus();
                            txtAddress.SelectAll();
                        }
                        else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                        {
                            txtPatientPhone.Focus();
                            txtPatientPhone.SelectAll();
                        }
                        else if (txtEmail.Visible && txtEmail.Enabled)
                        {
                            txtEmail.Focus();
                            txtEmail.SelectAll();
                        }
                        else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                        {
                            dtIntructionTime.Focus();
                            dtIntructionTime.SelectAll();
                        }
                        else if (txtLoginName.Visible && txtLoginName.Enabled)
                        {
                            txtLoginName.Focus();
                            txtLoginName.SelectAll();
                        }
                        else if (txtPresUser.Visible && txtPresUser.Enabled)
                        {
                            txtPresUser.Focus();
                            txtPresUser.SelectAll();
                        }
                        else if (txtDescription.Visible && txtDescription.Enabled)
                        {
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                        else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                        {
                            txtMediMatyForPrescription.Focus();
                            txtMediMatyForPrescription.Show();
                        }
                        else if (spinAmount.Visible && spinAmount.Enabled)
                        {
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else if (spinDayNum.Visible && spinDayNum.Enabled)
                        {
                            spinDayNum.Focus();
                            spinDayNum.SelectAll();
                        }
                        else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                        {
                            checkImpExpPrice.Focus();
                        }
                        else if (spinProfit.Visible && spinProfit.Enabled)
                        {
                            spinProfit.Focus();
                            spinProfit.SelectAll();
                        }
                        else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                        {
                            spinExpPrice.Focus();
                            spinExpPrice.SelectAll();
                        }
                        else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                        {
                            spinExpVatRatio.Focus();
                            spinExpVatRatio.SelectAll();
                        }
                        else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }
                        else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                        {
                            spinDiscountDetailRatio.Focus();
                            spinDiscountDetailRatio.SelectAll();
                        }
                        else if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(txtVirPatientName.Text) && cboGender.EditValue != null && !String.IsNullOrWhiteSpace(txtPatientDob.Text))
                    {
                        ProcessSearchPatientByInfo();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsVisitor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (txtPrescriptionCode.Enabled)
                    //{
                    //    txtPrescriptionCode.Focus();
                    //    txtPrescriptionCode.SelectAll();
                    //}
                    //else
                    //{
                    //    //txtSampleForm.Focus();
                    //    //txtSampleForm.SelectAll();

                    if (txtPrescriptionCode.Visible && txtPrescriptionCode.Enabled)
                    {
                        txtPrescriptionCode.Focus();
                        txtPrescriptionCode.SelectAll();
                    }
                    else if (txtTreatmentCode.Visible && txtTreatmentCode.Enabled)
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                    //  }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (checkIsVisitor.Visible && checkIsVisitor.Enabled)
                    {
                        checkIsVisitor.Focus();
                    }
                    else if (txtPrescriptionCode.Visible && txtPrescriptionCode.Enabled)
                    {
                        txtPrescriptionCode.Focus();
                        txtPrescriptionCode.SelectAll();
                    }
                    else if (txtTreatmentCode.Visible && txtTreatmentCode.Enabled)
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscountDetailRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtTutorial.Focus();
                    //txtTutorial.SelectAll();
                    if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                        e.Handled = true;
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtNote.Focus();
                    //txtNote.SelectAll();

                    if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                        e.Handled = true;
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinExpVatRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //spinDiscountDetail.Focus();
                    //spinDiscountDetail.SelectAll();

                    if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                        e.Handled = true;
                    }

                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }

                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPrescriptionCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPrescriptionCode.Text))
                    {
                        ProcessorSearch(true);
                    }

                    if (txtTreatmentCode.Visible && txtTreatmentCode.Enabled)
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }

                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        ProcessorSearchPatient();
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        ProcessorSearch(false);
                    }

                    if (txtPatientCode.Visible && txtPatientCode.Enabled)
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                    else if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }

                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        ProcessorSearchPatient();
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtVirPatientName.Visible && txtVirPatientName.Enabled)
                    {
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                    else if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }

                    if (!String.IsNullOrEmpty(txtPatientCode.Text))  
                    {
                        ProcessorSearchPatient();
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtVirPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //    cboGender.ShowPopup();
                    //    cboGender.Focus();
                    //    cboGender.SelectAll();

                    if (cboGender.Visible && cboGender.Enabled)
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }

                    if (!String.IsNullOrWhiteSpace(txtVirPatientName.Text) && cboGender.EditValue != null && !String.IsNullOrWhiteSpace(txtPatientDob.Text))
                    {
                        ProcessSearchPatientByInfo();
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
                //dùng key up để chọn được tại phần combobox
                if (e.KeyCode == Keys.Enter)
                {

                    if (txtPatientDob.Visible && txtPatientDob.Enabled)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                    else if (txtMaTHX.Visible && txtMaTHX.Enabled)
                    {
                        txtMaTHX.Focus();
                        txtMaTHX.SelectAll();
                    }
                    else if (cboTHX.Visible && cboTHX.Enabled)
                    {
                        cboTHX.Focus();
                        cboTHX.SelectAll();
                    }
                    else if (txtAddress.Visible && txtAddress.Enabled)
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                    else if (txtPatientPhone.Visible && txtPatientPhone.Enabled)
                    {
                        txtPatientPhone.Focus();
                        txtPatientPhone.SelectAll();
                    }
                    else if (txtEmail.Visible && txtEmail.Enabled)
                    {
                        txtEmail.Focus();
                        txtEmail.SelectAll();
                    }
                    else if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }

                    if (!String.IsNullOrWhiteSpace(txtVirPatientName.Text) && cboGender.EditValue != null && !String.IsNullOrWhiteSpace(txtPatientDob.Text))
                    {
                        ProcessSearchPatientByInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTHX_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? 0).ToString());
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

        private void txtPatientPhone_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtIntructionTime.Visible && dtIntructionTime.Enabled)
                    {
                        dtIntructionTime.Focus();
                        dtIntructionTime.SelectAll();
                    }
                    else if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtLoginName.Visible && txtLoginName.Enabled)
                    {
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                    else if (txtPresUser.Visible && txtPresUser.Enabled)
                    {
                        txtPresUser.Focus();
                        txtPresUser.SelectAll();
                    }
                    else if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.Show();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        //txtPresUser.Focus();
                        //txtPresUser.SelectAll();

                        if (txtPresUser.Visible && txtPresUser.Enabled)
                        {
                            txtPresUser.Focus();
                            txtPresUser.SelectAll();
                        }
                        else if (txtDescription.Visible && txtDescription.Enabled)
                        {
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                        else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                        {
                            txtMediMatyForPrescription.Focus();
                            txtMediMatyForPrescription.Show();
                        }
                        else if (spinAmount.Visible && spinAmount.Enabled)
                        {
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else if (spinDayNum.Visible && spinDayNum.Enabled)
                        {
                            spinDayNum.Focus();
                            spinDayNum.SelectAll();
                        }
                        else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                        {
                            checkImpExpPrice.Focus();
                        }
                        else if (spinProfit.Visible && spinProfit.Enabled)
                        {
                            spinProfit.Focus();
                            spinProfit.SelectAll();
                        }
                        else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                        {
                            spinExpPrice.Focus();
                            spinExpPrice.SelectAll();
                        }
                        else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                        {
                            spinExpVatRatio.Focus();
                            spinExpVatRatio.SelectAll();
                        }
                        else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                        {
                            spinDiscountDetail.Focus();
                            spinDiscountDetail.SelectAll();
                        }
                        else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                        {
                            spinDiscountDetailRatio.Focus();
                            spinDiscountDetailRatio.SelectAll();
                        }
                        else if (txtTutorial.Visible && txtTutorial.Enabled)
                        {
                            txtTutorial.Focus();
                            txtTutorial.SelectAll();
                        }
                        else if (txtNote.Visible && txtNote.Enabled)
                        {
                            txtNote.Focus();
                            txtNote.SelectAll();
                        }
                        else if (btnAdd.Visible && btnAdd.Enabled)
                        {
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        List<ACS_USER> data = null;

                        var listLoginName = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(d => d.IS_DOCTOR == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.LOGINNAME).ToList();

                        if (listLoginName != null && listLoginName.Count > 0)
                        {
                            data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper()) && listLoginName.Contains(o.LOGINNAME)).ToList();
                        }

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.txtPresUser.Text = searchResult[0].USERNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.txtDescription.Focus();
                        }
                        else
                        {
                            //txtPresUser.Focus();
                            //txtPresUser.SelectAll();

                            if (txtPresUser.Visible && txtPresUser.Enabled)
                            {
                                txtPresUser.Focus();
                                txtPresUser.SelectAll();
                            }
                            else if (txtDescription.Visible && txtDescription.Enabled)
                            {
                                txtDescription.Focus();
                                txtDescription.SelectAll();
                            }
                            else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                            {
                                txtMediMatyForPrescription.Focus();
                                txtMediMatyForPrescription.Show();
                            }
                            else if (spinAmount.Visible && spinAmount.Enabled)
                            {
                                spinAmount.Focus();
                                spinAmount.SelectAll();
                            }
                            else if (spinDayNum.Visible && spinDayNum.Enabled)
                            {
                                spinDayNum.Focus();
                                spinDayNum.SelectAll();
                            }
                            else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                            {
                                checkImpExpPrice.Focus();
                            }
                            else if (spinProfit.Visible && spinProfit.Enabled)
                            {
                                spinProfit.Focus();
                                spinProfit.SelectAll();
                            }
                            else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                            {
                                spinExpPrice.Focus();
                                spinExpPrice.SelectAll();
                            }
                            else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                            {
                                spinExpVatRatio.Focus();
                                spinExpVatRatio.SelectAll();
                            }
                            else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                            {
                                spinDiscountDetail.Focus();
                                spinDiscountDetail.SelectAll();
                            }
                            else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                            {
                                spinDiscountDetailRatio.Focus();
                                spinDiscountDetailRatio.SelectAll();
                            }
                            else if (txtTutorial.Visible && txtTutorial.Enabled)
                            {
                                txtTutorial.Focus();
                                txtTutorial.SelectAll();
                            }
                            else if (txtNote.Visible && txtNote.Enabled)
                            {
                                txtNote.Focus();
                                txtNote.SelectAll();
                            }
                            else if (btnAdd.Visible && btnAdd.Enabled)
                            {
                                btnAdd.Focus();
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    //txtMediMatyForPrescription.Focus();

                    if (txtDescription.Visible && txtDescription.Enabled)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        txtMediMatyForPrescription.Focus();
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
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

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtMediMatyForPrescription.Visible && txtMediMatyForPrescription.Enabled)
                    {
                        icdProcessor.FocusControl(ucIcd);
                    }
                    else if (spinAmount.Visible && spinAmount.Enabled)
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                    else if (spinDayNum.Visible && spinDayNum.Enabled)
                    {
                        spinDayNum.Focus();
                        spinDayNum.SelectAll();
                    }
                    else if (checkImpExpPrice.Visible && checkImpExpPrice.Enabled)
                    {
                        checkImpExpPrice.Focus();
                    }
                    else if (spinProfit.Visible && spinProfit.Enabled)
                    {
                        spinProfit.Focus();
                        spinProfit.SelectAll();
                    }
                    else if (spinExpPrice.Visible && spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else if (spinExpVatRatio.Visible && spinExpVatRatio.Enabled)
                    {
                        spinExpVatRatio.Focus();
                        spinExpVatRatio.SelectAll();
                    }
                    else if (spinDiscountDetail.Visible && spinDiscountDetail.Enabled)
                    {
                        spinDiscountDetail.Focus();
                        spinDiscountDetail.SelectAll();
                    }
                    else if (spinDiscountDetailRatio.Visible && spinDiscountDetailRatio.Enabled)
                    {
                        spinDiscountDetailRatio.Focus();
                        spinDiscountDetailRatio.SelectAll();
                    }
                    else if (txtTutorial.Visible && txtTutorial.Enabled)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else if (txtNote.Visible && txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else if (btnAdd.Visible && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                    //  }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearchPres_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSelectMultiPrescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSubIcd_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtSubIcdCode.Text, this.txtIcd.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                WaitingManager.Hide();
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtSubIcdCode.Text, this.txtIcd.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtSubIcdCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcd.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSubIcdCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;

                        return;
                    }
                    txtIcd.Focus();
                    txtIcd.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSubIcdCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtSubIcdCode.Text, this.txtIcd.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedIcdsToControl(this.txtSubIcdCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtSubIcdCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtSubIcdCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrIcdExtraCodes), arrIcdExtraCodes));
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());

                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show("Không tìm thấy icd tương ứng với các mã sau: " + strWrongIcdCodes);
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtSubIcdCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtSubIcdCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtSubIcdCode.Focus();
                            this.txtSubIcdCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcd.Text == txtIcd.Properties.NullValuePrompt ? "" : txtIcd.Text);
                txtIcd.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcd.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtSubIcdCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = this.currentIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtIcd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcd_Leave(object sender, EventArgs e)
        {
            try
            {
                txtMediMatyForPrescription.Focus();
                txtMediMatyForPrescription.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCreateBill_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessCreateBillChanged();
                this.SetTotalPriceExpMestDetail();
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHK_CREATE_BILL && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCreateBill.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHK_CREATE_BILL;
                    csAddOrUpdate.VALUE = (chkCreateBill.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCreateBill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && chkCreateBill.Checked)
                {
                    cboBillCashierRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillCashierRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboBillAccountBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillCashierRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillAccountBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillCashierRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.LoadAccountBook();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboPayForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillAccountBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPayForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBillAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinBillNumOrder.EditValue = null;
                spinBillNumOrder.Enabled = false;
                //cboAccountBook.Properties.Buttons[1].Visible = false;
                if (cboBillAccountBook.EditValue != null)
                {
                    //cboAccountBook.Properties.Buttons[1].Visible = true;
                    var account = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboBillAccountBook.EditValue));
                    if (account != null)
                    {
                        spinBillNumOrder.EditValue = setDataToDicNumOrderInAccountBook(account);
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinBillNumOrder.Enabled = true;
                        }

                        // thu ngân mở 2 phòng.
                        // sổ ở phòng nào tự động chọn theo phòng đó.
                        if (GlobalVariables.DefaultAccountBookMedicineSaleBill == null)
                        {
                            GlobalVariables.DefaultAccountBookMedicineSaleBill = new List<V_HIS_ACCOUNT_BOOK>();
                        }

                        if (GlobalVariables.DefaultAccountBookMedicineSaleBill.Count > 0)
                        {
                            List<V_HIS_ACCOUNT_BOOK> acc = new List<V_HIS_ACCOUNT_BOOK>();
                            acc.AddRange(GlobalVariables.DefaultAccountBookMedicineSaleBill);
                            //add lại sổ để luôn đưa sổ vừa chọn lên đầu.
                            GlobalVariables.DefaultAccountBookMedicineSaleBill = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookMedicineSaleBill.Add(account);
                            foreach (var item in acc)
                            {
                                if (item.ID != account.ID)
                                {
                                    GlobalVariables.DefaultAccountBookMedicineSaleBill.Add(item);
                                }
                            }
                        }
                        else
                        {
                            GlobalVariables.DefaultAccountBookMedicineSaleBill.Add(account);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinBillNumOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPayForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCreateBillChanged()
        {
            try
            {
                lciBillCashierRoom.Visibility = chkCreateBill.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciBillAccountBook.Visibility = chkCreateBill.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciBillNumOrder.Visibility = chkCreateBill.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciRoundPrice.Visibility = chkCreateBill.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciTransactionCode.Visibility = chkCreateBill.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciBaseValue.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciTotalReceivable.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem47.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked && IsShowDetails) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBook()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                if (cboBillCashierRoom.EditValue != null)
                {
                    acFilter.CASHIER_ROOM_ID = Convert.ToInt64(cboBillCashierRoom.EditValue);//Kiểm tra sổ còn hay k
                }
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                this.listAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);

                cboBillAccountBook.Properties.DataSource = this.listAccountBook;

                SetDefaultAccountBook();
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
                cboBillAccountBook.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookMedicineSaleBill != null && GlobalVariables.DefaultAccountBookMedicineSaleBill.Count > 0)
                {
                    var lstBook = GlobalVariables.DefaultAccountBookMedicineSaleBill.Where(o => this.listAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.First();
                    }
                }

                if (HisConfigCFG.IS_AUTO_SELECT_ACCOUNT_BOOK_IF_HAS_ONE && accountBook == null && listAccountBook.Count == 1)
                {
                    accountBook = listAccountBook.First();
                }

                if (accountBook != null)
                {
                    cboBillAccountBook.EditValue = accountBook.ID;
                }
                else
                {
                    cboBillAccountBook.EditValue = null;
                    spinBillNumOrder.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
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

                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void chkRoundPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lciBaseValue.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciTotalReceivable.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                spinBaseValue.Enabled = (chkRoundPrice.Checked) ? false : true;
                layoutControlItem47.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked && IsShowDetails) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                this.SetTotalPriceExpMestDetail();

                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHK_ROUND_PRICE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkRoundPrice.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHK_ROUND_PRICE;
                    csAddOrUpdate.VALUE = (chkRoundPrice.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinBaseValue_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.SPIN_BASE_VALUE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = spinBaseValue.Value.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.SPIN_BASE_VALUE;
                    csAddOrUpdate.VALUE = spinBaseValue.Value.ToString();
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                this.SetTotalPriceExpMestDetail();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(long accountBookId, decimal numOrder)
        {
            try
            {
                if (accountBookId > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBookId))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBookId] = numOrder;//spinTongTuDen.Value
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDonCu_Click(object sender, EventArgs e)
        {
            try
            {
                frmDonCu frm;
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    frm = new frmDonCu(true, txtTreatmentCode.Text, this.currentModuleBase, ChooseDonCu);
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    frm = new frmDonCu(false, txtPatientCode.Text, this.currentModuleBase, ChooseDonCu);
                }
                else
                {
                    frm = new frmDonCu(null, "", this.currentModuleBase, ChooseDonCu);
                }
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChooseDonCu(List<V_HIS_EXP_MEST> listData)
        {
            try
            {
                if (listData != null && listData.Count > 0)
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listData), listData));
                    // this.serviceReq = listData;
                    ProcessChooseDonCu(listData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ProcessSelectMultiPrescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetTotalPriceOfChildChoice(MediMateTypeADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (MediMateTypeADO)treeListMediMate.GetDataRecordByNode(item);
                        if (nodeData == null) continue;

                        // if (item.HasChildren)
                        //{
                        if (fieldName == "TOTAL_PRICE_STR")
                        {
                            totalChoicePrice += (nodeData.TOTAL_PRICE ?? 0);
                        }
                        //}
                    }
                }
                if (fieldName == "TOTAL_PRICE_STR")
                {
                    data.TOTAL_PRICE = totalChoicePrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ChkKetNoiPOS_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0) ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHK_KetNoiPos && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (ChkKetNoiPOS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHK_KetNoiPos;
                    csAddOrUpdate.VALUE = (ChkKetNoiPOS.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public bool OpenAppPOS()
        {
            try
            {
                if (IsProcessOpen("WCF"))
                {
                    return true;
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = Application.StartupPath + @"\Integrate\POS.WCFService\WCF.exe";
                    nameFile = startInfo.FileName;
                    Inventec.Common.Logging.LogSystem.Info("FileName " + startInfo.FileName);
                    Process.Start(startInfo);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startInfo), startInfo));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        private bool IsProcessOpen(string name)
        {
            try
            {
                var processByNames = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains(name)).ToList();
                if (processByNames != null && processByNames.Count >= 2)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void btnCauHinh_Click(object sender, EventArgs e)
        {
            try
            {
                OpenAppPOS();
                try
                {
                    cll = new WcfClient();
                }
                catch (Exception ex)
                {
                    ChkKetNoiPOS.Checked = false;
                    XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                    return;
                }
                cll.cauhinh();
            }
            catch (Exception ex)
            {
                ChkKetNoiPOS.Checked = false;
                XtraMessageBox.Show("Cấu hình thất bại", "Thông báo");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControlItem47_Click(object sender, EventArgs e)
        {
            try
            {
                showFormDetails();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal? CalculReceivable(decimal? totalPayPrice)
        {
            decimal? totalReceivable = 0;
            try
            {

                if (chkCreateBill.Checked && chkRoundPrice.Checked && totalPayPrice > 0)
                {
                    int b = (int)spinBaseValue.Value;
                    if (b > 0)
                    {
                        int n = b.ToString().Length;
                        int y = (int)(totalPayPrice % (int)(Math.Pow(10, n)));
                        if (y >= b)
                        {
                            totalReceivable = ((int)(totalPayPrice / (int)Math.Pow(10, n)) + 1) * (int)Math.Pow(10, n);
                        }
                        else
                        {
                            totalReceivable = ((int)(totalPayPrice / (int)Math.Pow(10, n))) * (int)Math.Pow(10, n);
                        }
                    }
                    else
                    {
                        totalReceivable = totalPayPrice;
                    }
                }

            }
            catch (Exception ex)
            {
                return 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return totalReceivable;
        }

        private void layoutControlItem48_Click(object sender, EventArgs e)
        {
            try
            {
                showFormDetails();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void showFormDetails()
        {
            try
            {
                var dataTrees = dicMediMateAdo.Select(o => o.Value).ToList();
                var dataCoverts = dataTrees.SelectMany(p => p).Distinct().OrderByDescending(o => o.TOTAL_PRICE).ToList();
                var dataGroups = dataCoverts.GroupBy(p => p.SERVICE_REQ_CODE).Select(p => p.ToList()).ToList();
                List<PriceDetailsADO> lstADO = new List<PriceDetailsADO>();
                foreach (var items in dataGroups)
                {
                    decimal totalPrice = 0;
                    decimal discountRatio = 0;
                    PriceDetailsADO ado = new PriceDetailsADO();
                    ado.PATIENT_NAME = items[0].TDL_PATIENT_NAME;
                    foreach (var i in items)
                    {
                        if (spinDiscountRatio.EditValue != null)
                        {
                            discountRatio += (i.TOTAL_PRICE ?? 0) * spinDiscountRatio.Value / 100;
                            totalPrice += (i.TOTAL_PRICE ?? 0) - (i.TOTAL_PRICE ?? 0) * spinDiscountRatio.Value / 100;
                        }
                        else
                        {
                            totalPrice += i.TOTAL_PRICE ?? 0;
                        }

                    }
                    ado.PRICE = totalPrice;

                    ado.PRICE_ROUND = Inventec.Common.Number.Convert.NumberToString(CalculReceivable(ado.PRICE) ?? 0, ConfigApplications.NumberSeperator);
                    ado.DiscountRatio = discountRatio;
                    ado.BASE_VALUE = spinDiscountRatio.Value;
                    lstADO.Add(ado);
                }

                frmDetails frm = new frmDetails(currentModule, lstADO.OrderByDescending(o => o.PRICE).ToList(), chkCreateBill.Checked && chkRoundPrice.Checked);
                frm.ShowDialog();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkEditUser_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtLoginName.Enabled = chkEditUser.Checked;
                txtPresUser.Enabled = chkEditUser.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        private void CheckEnableBtnQR()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();

                layoutControlItem50.Visibility = (listConfig != null && listConfig.Count > 0) || (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON)) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                btnQr.Enabled = false;
                InitMenuQr();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitMenuQr()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                if (listConfig != null && listConfig.Count > 0)
                {
                    if (listConfig.Count > 1)
                    {
                        foreach (var item in listConfig)
                        {
                            string key = "";
                            string value = item.KEY;
                            int index = value.IndexOf("Info");
                            if (index > 0)
                            {
                                var shotkey = value.Substring(0, index);
                                string[] parts = shotkey.Split('.');
                                if (parts.Length > 0)
                                {
                                    key = parts[parts.Length - 1];
                                }
                            }
                            else
                            {
                                key = item.KEY;
                            }
                            DXMenuItem dxi = new DXMenuItem(key);
                            dxi.Tag = item;
                            dxi.Click += Dxi_Click;
                            menu.Items.Add(dxi);
                        }
                    }
                }
                if(mediStock == null || string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                    btnQr.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Dxi_Click(object sender, EventArgs e)
        {
            try
            {
                DXMenuItem item = sender as DXMenuItem;
                if (item.Tag is HIS_CONFIG)
                {
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    adoqr.TransReqId = CreateReqType.Transaction;
                    adoqr.ConfigValue = item.Tag as HIS_CONFIG;

                    if (!isTwoPatient && resultSDO != null && resultSDO.Transaction != null)
                    {
                        adoqr.TreatmentId = 0;
                        adoqr.Transaction = resultSDO.Transaction;
                        listArgs.Add(adoqr);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                    else if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                    {
                        foreach (var sdo in ListResultSDO)
                        {
                            if (sdo.Transaction != null)
                            {
                                adoqr.TreatmentId = 0;
                                adoqr.Transaction = sdo.Transaction;
                                listArgs.Add(adoqr);
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
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

        private void btnQr_Click(object sender, EventArgs e)
        {
            try
            {
                if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                {
                    goto End;
                }
                else if (listConfig != null && listConfig.Count > 1)
                {
                    btnQr.ShowDropDown();
                    return;
                }
                else
                {
                    goto End;
                }
            End:
                List<object> listArgs = new List<object>();
                TransReqQRADO adoqr = new TransReqQRADO();
                adoqr.TransReqId = CreateReqType.Transaction;
                if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                {

                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(mediStock.QR_CONFIG_JSON);
                        if(json != null)
                            adoqr.ConfigValue = new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) };
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                        return;
                    }

                }
                else
                    adoqr.ConfigValue = listConfig[0];

                if (!isTwoPatient && resultSDO != null && resultSDO.Transaction != null)
                {
                    adoqr.TreatmentId = mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON) ? resultSDO.Transaction.TREATMENT_ID ?? 0 : 0;
                    adoqr.Transaction = resultSDO.Transaction;
                    listArgs.Add(adoqr);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                else if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                {
                    foreach (var sdo in ListResultSDO)
                    {
                        if (sdo.Transaction != null)
                        {
                            adoqr.TreatmentId = mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON) ? resultSDO.Transaction.TREATMENT_ID ?? 0 : 0;
                            adoqr.Transaction = sdo.Transaction;
                            listArgs.Add(adoqr);
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class BankInfo
    {
        public BankInfo() { }
        public string BANK { get; set; }
        public string VALUE { get; set; }
    }
}
