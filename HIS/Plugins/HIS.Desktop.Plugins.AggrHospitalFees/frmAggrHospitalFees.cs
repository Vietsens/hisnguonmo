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
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AggrHospitalFees.Base;
using HIS.Desktop.Plugins.AggrHospitalFees.Config;
using HIS.UC.TotalPriceInfo;
using HIS.UC.TotalPriceInfo.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrHospitalFees
{
    public partial class frmAggrHospitalFees : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        TotalPriceInfoProcessor totalPriceProcessor;
        UserControl ucTotalPriceInfo;
        internal long treatmentId;
        PopupMenuProcessor popupMenuProcessor = null;
        V_HIS_TRANSACTION transactionPrint;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;
        int pageIndex = 0;
        public frmAggrHospitalFees()
        {
            InitializeComponent();
        }

        public frmAggrHospitalFees(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
        : base(currentModule)
        {
            InitializeComponent();
            try
            {
                ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAggrHospitalFees_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                SetCaptionByLanguageKey();
                if (this.treatmentId > 0)
                {
                    LoadDataTreatment();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AggrHospitalFees.Resources.Lang", typeof(HIS.Desktop.Plugins.AggrHospitalFees.frmAggrHospitalFees).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAggrHospitalFees.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadDataTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFeeViewFilter filter = new MOS.Filter.HisTreatmentFeeViewFilter();
                filter.ID = this.treatmentId;
                var rs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (rs != null)
                {
                    TotalPriceInfo(rs);
                }
                FillGridTransaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillGridTransaction()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridTransaction(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadGridTransaction, param, pageSize, gridControl1);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGridTransaction(object param)
        {
            try
            {
                gridControl1.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisTransactionViewFilter filter = new MOS.Filter.HisTransactionViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.TREATMENT_ID = this.treatmentId;
                var rs = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null)
                {
                    gridControl1.DataSource = rs.Data;
                    rowCount = (rs.Data == null ? 0 : rs.Data.Count);
                    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TotalPriceInfo(V_HIS_TREATMENT_FEE currentTreatment)
        {
            try
            {
                UC.TotalPriceInfo.ADO.TotalPriceADO adoPrice = new UC.TotalPriceInfo.ADO.TotalPriceADO();
                if (currentTreatment != null)
                {
                    adoPrice.Discount = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_BILL_EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_DISCOUNT ?? 0, ConfigApplications.NumberSeperator);
                    if (currentTreatment.TOTAL_PATIENT_PRICE.HasValue && currentTreatment.TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        decimal discountRatio = 0;
                        if (currentTreatment.TOTAL_DISCOUNT.HasValue)
                        {
                            discountRatio = (currentTreatment.TOTAL_DISCOUNT.Value) / currentTreatment.TOTAL_PATIENT_PRICE.Value;
                        }
                    }
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_BILL_FUND ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_BILL_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString((currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - (currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0), ConfigApplications.NumberSeperator);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_HEIN_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_PATIENT_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_REPAY_AMOUNT ?? 0, ConfigApplications.NumberSeperator);

                    decimal totalReceive = ((currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (currentTreatment.TOTAL_BILL_AMOUNT ?? 0) - (currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (currentTreatment.TOTAL_BILL_FUND ?? 0) - (currentTreatment.TOTAL_REPAY_AMOUNT ?? 0)) - (currentTreatment.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (currentTreatment.TOTAL_BILL_FUND ?? 0) - (currentTreatment.TOTAL_BILL_EXEMPTION ?? 0);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(totalReceive, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_BILL_OTHER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_OTHER_SOURCE_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_OTHER_COPAID_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_DEBT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(currentTreatment.TOTAL_PRICE_EXPEND ?? 0, ConfigApplications.NumberSeperator);
                }
                else
                {

                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.Discount = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString( 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                }

                SetValueToControl(adoPrice);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void SetValueToControl(TotalPriceADO data)
        {
            try
            {
                if (data != null)
                {
                    this.lblDiscount.Text = data.Discount;
                    //this.lblDiscountRatio.Text = data.DiscountRatio;
                    this.lblVirTotalBillPrice.Text = data.TotalBillPrice;
                    this.lblVirTotalBillFundPrice.Text = data.TotalBillFundPrice;
                    this.lblVirTotalBillTransferPrice.Text = data.TotalBillTransferPrice;
                    this.lblVirTotalDepositPrice.Text = data.TotalDepositPrice;
                    this.lblVirTotalServiceDepositPrice.Text = data.TotalServiceDepositPrice;
                    this.lblVirTotalHeinPrice.Text = data.TotalHeinPrice;
                    this.lblVirTotalPatientPrice.Text = data.TotalPatientPrice;
                    this.lblVirTotalPrice.Text = data.TotalPrice;

                    this.lblTotalOtherCopaidPrice.Text = data.TotalOtherCopaidPrice;

                    this.lblVirTotalReceivePrice.Text = data.TotalReceivePrice;
                    this.lblVirTotalRepayPrice.Text = data.TotalRepayPrice;
                    this.lblTotalDiscount.Text = data.TotalDiscount;
                    this.lblTotalOtherBillAmount.Text = data.TotalOtherBillAmount;
                    this.lblVirTotalPriceNoExpend.Text = data.VirTotalPriceNoExpend;
                    this.lblTotalDebtAmount.Text = data.TotalDebtAmount;
                    this.lblOtherSourcePrice.Text = data.TotalOtherSourcePrice;
                    this.lblVirTotalReceiveMorePrice.Text = data.TotalReceiveMorePrice;

                    if (!String.IsNullOrEmpty(data.TotalReceiveMorePrice) && Convert.ToDecimal(data.TotalReceiveMorePrice) <= 0)
                    {
                        this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Blue;
                        layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Blue;
                    }
                    else
                    {
                        this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Red;
                        layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Red;
                    }
                }
                else
                {
                    this.lblDiscount.Text = "0.0000";
                    //this.lblDiscountRatio.Text = "0.00";
                    this.lblVirTotalBillPrice.Text = "0.0000";
                    this.lblVirTotalBillFundPrice.Text = "0.0000";
                    this.lblVirTotalBillTransferPrice.Text = "0.0000";
                    this.lblVirTotalDepositPrice.Text = "0.0000";
                    this.lblVirTotalServiceDepositPrice.Text = "0.0000";
                    this.lblVirTotalHeinPrice.Text = "0.0000";
                    this.lblVirTotalPatientPrice.Text = "0.0000";
                    this.lblVirTotalPrice.Text = "0.0000";

                    this.lblTotalOtherCopaidPrice.Text = "0.0000";

                    this.lblVirTotalReceiveMorePrice.Text = "0.0000";
                    this.lblVirTotalReceivePrice.Text = "0.0000";
                    this.lblVirTotalRepayPrice.Text = "0.0000";
                    this.lblTotalDiscount.Text = "0.0000";
                    this.lblTotalOtherBillAmount.Text = "0.0000";
                    this.lblVirTotalPriceNoExpend.Text = "0.0000";
                    this.lblTotalDebtAmount.Text = "0.0000";
                    this.lblOtherSourcePrice.Text = "0.0000";
                    this.lblVirTotalReceiveMorePrice.Appearance.ForeColor = Color.Blue;
                    layoutVirTotalReceiveMorePrice.AppearanceItemCaption.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && ((IList)((BaseView)sender).DataSource) != null)
                {
                    var dataRow = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;

                    if (e.Column.FieldName == "DOB_str")
                    {
                        e.Value = dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB ?? 0);
                    }else if (e.Column.FieldName == "AMOUNT_str")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.AMOUNT, ConfigApplications.NumberSeperator); ;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_TRANSACTION data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridView1.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ReCancel") {
                        if ((data.IS_CANCEL == null || data.IS_CANCEL != 1) && (data.CANCEL_REQ_STT == null || data.CANCEL_REQ_STT == IMSys.DbConfig.HIS_RS.CANCEL_REQ_STT.ID__REJECT_CANCEL_REQ))
                        {
                            e.RepositoryItem = repReCancelEna;
                        }
                        else
                        {
                            e.RepositoryItem = repReCancelDis;
                        }
                    }
                    else if(e.Column.FieldName == "Cancel")
                    {
                        if ((data.IS_CANCEL == null || data.IS_CANCEL != 1) && data.CANCEL_REQ_STT == IMSys.DbConfig.HIS_RS.CANCEL_REQ_STT.ID__CANCEL_REQ && (data.CANCEL_REQ_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())))
                        {
                            e.RepositoryItem = repCancelEna;
                        }
                        else
                        {
                            e.RepositoryItem = repCancelDis;
                        }
                    }    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repCancelEna_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    CommonParam param = new CommonParam();
                    HisTransactionDeleteCancellationRequestSDO sdo = new HisTransactionDeleteCancellationRequestSDO();
                    sdo.TransactionId = ((V_HIS_TRANSACTION)gridView1.GetFocusedRow()).ID;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/DeleteCancellationRequest", ApiConsumers.MosConsumer, sdo, param);
                    if(rs != null)
                    {
                        LoadDataTreatment();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this, param, rs != null);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
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

        private void repReCancelEna_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                frmReCancel frm = new frmReCancel(currentModule,(V_HIS_TRANSACTION)gridView1.GetFocusedRow(),GetSuccess);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSuccess(bool obj)
        {
            try
            {
                LoadDataTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {

                    transactionPrint = (V_HIS_TRANSACTION)gridView1.GetFocusedRow();
                    if (transactionPrint.CANCEL_REQ_STT != IMSys.DbConfig.HIS_RS.CANCEL_REQ_STT.ID__CANCEL_REQ)
                        return;
                    if (this.barManager1 == null)
                    {
                        this.barManager1 = new BarManager();
                        this.barManager1.Form = this;
                    }
                    if (transactionPrint != null)
                    {
                        this.popupMenuProcessor = new PopupMenuProcessor(transactionPrint, this.barManager1, MouseRightItemClick, currentModule);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.transactionPrint != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.HuyHoaDon:
                            this.PrintHuyHoaDon();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHuyHoaDon()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate("Mps000487", this.DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000487":
                        InPhieuHuyHoaDon(printTypeCode, fileName, ref result);
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

        private void InPhieuHuyHoaDon(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.Processor.Mps000487.PDO.Mps000487PDO rdo = new MPS.Processor.Mps000487.PDO.Mps000487PDO(transactionPrint);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint.TREATMENT_CODE != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridView1.GetRow(e.RowHandle);
                    if (data.IS_CANCEL == 1)
                    {
                        if (e.Column.FieldName == "STT")
                            return;
                        e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold | FontStyle.Strikeout);
                        }
                    }
                    else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                    {
                        e.Appearance.ForeColor = Color.Blue; //Giao dịch thanh toán => Màu xanh nước biển
                        if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                        }
                    }
                    else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        e.Appearance.ForeColor = Color.Green; //Giao dịch tạm ứng => Màu xanh lá cây
                    }
                    else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                    {
                        e.Appearance.ForeColor = Color.Red; //Giao dịch hoàn ứng => Màu đỏ
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
