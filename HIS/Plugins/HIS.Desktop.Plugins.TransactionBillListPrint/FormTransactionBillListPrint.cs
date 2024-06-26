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
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillListPrint
{
    public partial class FormTransactionBillListPrint : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private string loginName = null;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int start = 0;

        public FormTransactionBillListPrint()
        {
            InitializeComponent();
        }

        public FormTransactionBillListPrint(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTransactionBillListPrint_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeysFromlanguage();
                InitCboBillType();
                SetDefaultValueControl();
                FillDataToGrid();
                CreateThreadLoadTotalPrice(GetFilterTotal());
                Config.HisConfigCFG.LoadConfig();
                TxtKeyWord.Focus();
                TxtKeyWord.SelectAll();
                WaitingManager.Hide();
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
                this.BarNumOrder.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BAR_NUM_ORDER");
                this.BtnPrint.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BTN_PRINT");
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BTN_REFRESH");
                this.BtnSearch.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BTN_SEARCH");
                this.LciBillType.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LCI_BILL_TYPE");
                this.LciNumOrderFrom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LCI_NUM_ORDER_FROM");
                this.LciNumOrderTo.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LCI_NUM_ORDER_TO");
                this.TxtKeyWord.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__TXT_KEYWORD");
                this.LciSymbolCode.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LCI_SYMBOL_CODE");
                this.lciTemplateCode.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LCI_TEMPLATE_CODE");
                this.BtnPrintNow.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BTN_PRINT_NOW");
                this.LcBL_HD_H.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LcBL_HD_H");
                this.LcBL_HD_H.OptionsToolTip.ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__LcBL_HD_H_ToolTip");
                this.gridColumn_Transaction_AccountBookCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_ACCOUNT_BOOK_CODE");
                this.gridColumn_Transaction_AccountBookName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_ACCOUNT_BOOK_NAME");
                this.gridColumn_Transaction_Amount.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_AMOUNT");
                this.gridColumn_Transaction_BillFunAmount.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_BILL_FUN_AMOUNT");
                this.gridColumn_Transaction_Cashier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_CASHIER");
                this.gridColumn_Transaction_CashierRoomName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_CASHIER_ROOM_NAME");
                this.gridColumn_Transaction_CreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_CREATE_TIME");
                this.gridColumn_Transaction_Creator.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_CREATOR");
                this.gridColumn_Transaction_Dob.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_DOB");
                this.gridColumn_Transaction_Exemption.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_EXEMPTION");
                this.gridColumn_Transaction_ExemptionReason.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_EXEMPTION_REASON");
                this.gridColumn_Transaction_GenderName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_GENDER_NAME");
                this.gridColumn_Transaction_KcAmount.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_KC_AMOUNT");
                this.gridColumn_Transaction_Modifier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_MODIFIER");
                this.gridColumn_Transaction_ModifyTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_MODIFI_TIME");
                this.gridColumn_Transaction_NationalTransactionCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__NATIONAL_TRANSACTION_CODE");
                this.gridColumn_Transaction_NumOrder.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__NUM_ORDER");
                this.gridColumn_Transaction_PatientCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_PATIENT_CODE");
                this.gridColumn_Transaction_PayFormName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_PAY_FORM_NAME");
                this.gridColumn_Transaction_Stt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_STT");
                this.gridColumn_Transaction_Tig_TransactionCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_TIG_TRANSACTION_CODE");
                this.gridColumn_Transaction_ThucThu.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_THUC_THU");
                this.gridColumn_Transaction_TransactionCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_TRANSACTION_CODE");
                this.gridColumn_Transaction_TransactionTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_TRANSACTION_TIME");
                this.gridColumn_Transaction_TransactionTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_TRANSACTION_TYPE_NAME");
                this.gridColumn_Transaction_TreatmentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_TREATMENT_CODE");
                this.gridColumn_Transaction_VirPatientName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__GC_PATIENT_NAME");
                this.LcDateFrom.Text = GetLanguageControl
("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__DATE_FROM");
                this.lcDateTo.Text = GetLanguageControl
("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__DATE_TO");





            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void SetDefaultValueControl()
        {
            try
            {
                TxtKeyWord.Text = "";
                TxtSymbolCode.Text = "";
                TxtTemplateCode.Text = "";
                CboBillType.EditValue = 1;
                SpNumOrderFrom.Value = 1;
                SpNumOrderTo.Value = 1;
                lblTongTien.Text = "";
                lblTongTienHuy.Text = "";
                lblTongTienThucNop.Text = "";
                ChkBL_HD_H.Checked = false;
                cboDateFrom.EditValue = DateTime.Now.ToString("dd/MM/yyyy");
                cboDateTo.EditValue = DateTime.Now.ToString("dd/MM/yyyy");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboBillType()
        {
            try
            {
                //1-Thuong;2-Dich vu
                List<ADO.BillTypeADO> lstBillType = new List<ADO.BillTypeADO>();

                lstBillType.Add(new ADO.BillTypeADO() { ID = 1, BILL_TYPE_NAME = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BILL_TYPE_1") });
                lstBillType.Add(new ADO.BillTypeADO() { ID = 2, BILL_TYPE_NAME = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BILL_LIST_PRINT__BILL_TYPE_2") });

                CboBillType.Properties.DataSource = lstBillType;
                CboBillType.Properties.DisplayMember = "BILL_TYPE_NAME";
                CboBillType.Properties.ValueMember = "ID";
                CboBillType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                CboBillType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                CboBillType.Properties.ImmediatePopup = true;
                CboBillType.ForceInitialize();
                CboBillType.Properties.View.Columns.Clear();
                CboBillType.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = CboBillType.Properties.View.Columns.AddField("BILL_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                FillDataToGridTransaction(new CommonParam(0, pagingSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransaction, param, pagingSize, this.gridControl1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisTransactionViewFilter GetFilterTotal()
        {
            HisTransactionViewFilter filter = new HisTransactionViewFilter();
            try
            {
                filter.KEY_WORD = TxtKeyWord.Text;
                filter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filter.NUM_ORDER_FROM = (long)SpNumOrderFrom.Value;
                filter.NUM_ORDER_TO = (long)SpNumOrderTo.Value;
                if (cboDateFrom.EditValue != null)
                {
                    filter.TRANSACTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(cboDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (cboDateTo.EditValue != null)
                {
                    filter.TRANSACTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(cboDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (ChkBL_HD_H.Checked)
                {
                    filter.IS_CANCEL = true;
                }
                if (!String.IsNullOrWhiteSpace(TxtSymbolCode.Text))
                {
                    filter.SYMBOL_CODE__EXACT = TxtSymbolCode.Text;
                }

                if (!String.IsNullOrWhiteSpace(TxtTemplateCode.Text))
                {
                    filter.TEMPLATE_CODE__EXACT = TxtTemplateCode.Text;
                }

                filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = true;

                if (CboBillType.EditValue != null)
                {
                    long type = Inventec.Common.TypeConvert.Parse.ToInt64((CboBillType.EditValue ?? "").ToString());
                    if (type == 2)
                        filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return filter;
        }

        private void CreateThreadLoadTotalPrice(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadTotalPriceNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadTotalPriceNewThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataTotalPrice((HisTransactionViewFilter)param); }));
                }
                else
                {
                    this.LoadDataTotalPrice((HisTransactionViewFilter)param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTotalPrice(HisTransactionViewFilter filter)
        {
            try
            {
                if (filter != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<HisTransactionTotalPriceSDO>("api/HisTransaction/GetTotalPriceSdo", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        lblTongTien.Text = Inventec.Common.Number.Convert.NumberToString(result.TotalPrice, ConfigApplications.NumberSeperator);
                        lblTongTienHuy.Text = Inventec.Common.Number.Convert.NumberToString(result.CancelPrice, ConfigApplications.NumberSeperator);
                        lblTongTienThucNop.Text = Inventec.Common.Number.Convert.NumberToString(result.TotalPrice - result.CancelPrice, ConfigApplications.NumberSeperator);
                    }
                    else
                    {
                        lblTongTien.Text = "";
                        lblTongTienHuy.Text = "";
                        lblTongTienThucNop.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void FillDataToGridTransaction(object param)
        {
            try
            {
                List<V_HIS_TRANSACTION> listTransaction = new List<V_HIS_TRANSACTION>();
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";

                SetFilter(ref filter);

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TRANSACTION>>(HisRequestUriStore.HIS_TRANSACTION_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listTransaction = (List<V_HIS_TRANSACTION>)result.Data;
                    rowCount = (listTransaction == null ? 0 : listTransaction.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }

                gridControl1.BeginUpdate();
                gridControl1.DataSource = listTransaction;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                gridControl1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisTransactionViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = TxtKeyWord.Text;
                filter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filter.NUM_ORDER_FROM = (long)SpNumOrderFrom.Value;
                filter.NUM_ORDER_TO = (long)SpNumOrderTo.Value;
                if (cboDateFrom.EditValue != null)
                {
                    filter.TRANSACTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(cboDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (cboDateTo.EditValue != null)
                {
                    filter.TRANSACTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(cboDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (ChkBL_HD_H.Checked)
                {
                    filter.IS_CANCEL = true;
                }
                if (!String.IsNullOrWhiteSpace(TxtSymbolCode.Text))
                {
                    filter.SYMBOL_CODE__EXACT = TxtSymbolCode.Text;
                }

                if (!String.IsNullOrWhiteSpace(TxtTemplateCode.Text))
                {
                    filter.TEMPLATE_CODE__EXACT = TxtTemplateCode.Text;
                }

                filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = true;

                if (CboBillType.EditValue != null)
                {
                    long type = Inventec.Common.TypeConvert.Parse.ToInt64((CboBillType.EditValue ?? "").ToString());
                    if (type == 2)
                        filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
                CreateThreadLoadTotalPrice(GetFilterTotal());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
                CreateThreadLoadTotalPrice(GetFilterTotal());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TRANSACTION> listDataPrint = GetListSelectedRow();
                if (listDataPrint.Count > 0)
                {
                    PopupMenu menu = new PopupMenu(barManager1);
                    menu.ItemLinks.Clear();
                    List<BarItem> bItems = new List<BarItem>();

                    BarButtonItem itemPrint = new BarButtonItem(barManager1, "Phiếu biên lai thanh toán");
                    itemPrint.ItemClick += new ItemClickEventHandler(this.Print__ItemClick);
                    itemPrint.Tag = PrintProcessor.PrintTypeCode__Mps000148;
                    bItems.Add(itemPrint);

                    if (listDataPrint.Any(o => o.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP))
                    {
                        BarButtonItem itemPrintHDNT = new BarButtonItem(barManager1, "Phiếu xuất bán");
                        itemPrintHDNT.ItemClick += new ItemClickEventHandler(this.Print__ItemClick);
                        itemPrintHDNT.Tag = PrintProcessor.PrintTypeCode__Mps000092;
                        bItems.Add(itemPrintHDNT);
                    }

                    menu.AddItems(bItems.ToArray());
                    menu.ShowPopup(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonDuLieuIn);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_TRANSACTION> GetListSelectedRow()
        {
            List<V_HIS_TRANSACTION> listDataPrint = new List<V_HIS_TRANSACTION>();
            try
            {

                for (int i = 0; i < gridView1.GetSelectedRows().Count(); i++)
                {
                    listDataPrint.Add((V_HIS_TRANSACTION)gridView1.GetRow(gridView1.GetSelectedRows()[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listDataPrint;
        }

        private void Print__ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<V_HIS_TRANSACTION> listDataPrint = GetListSelectedRow();
                string mpsPrintTypeCode = e.Item != null && e.Item.Tag != null ? e.Item.Tag.ToString() : "";
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mpsPrintTypeCode), mpsPrintTypeCode));
                var processprint = new PrintProcessor(listDataPrint);
                if (processprint != null) processprint.Print(false, mpsPrintTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnRefresh_Click(null, null);
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
                BtnPrint_Click(null, null);
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
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CASHIER")
                        {
                            try
                            {
                                e.Value = data.CASHIER_LOGINNAME + (String.IsNullOrEmpty(data.CASHIER_USERNAME) ? "" : " - " + data.CASHIER_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TRANSACTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "THUC_THU_STR")
                        {
                            try
                            {
                                decimal? ado = data.AMOUNT - (data.KC_AMOUNT ?? 0) - (data.TDL_BILL_FUND_AMOUNT ?? 0) - (data.EXEMPTION ?? 0);
                                e.Value = ado;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridView1.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            if (e.Column.FieldName == "STT" || e.Column.FieldName == "CancelTransaction" || e.Column.FieldName == "ChangeLock")
                                return;
                            e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        {
                            e.Appearance.ForeColor = Color.Blue; //Giao dịch thanh toán => Màu xanh nước biển
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrintNow_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TRANSACTION> listDataPrint = GetListSelectedRow();

                if (listDataPrint.Count > 0)
                {
                    var processPrint = new PrintProcessor(listDataPrint);
                    if (processPrint != null) processPrint.Print(true, PrintProcessor.PrintTypeCode__Mps000148);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonDuLieuIn);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrintNowForXuatBan_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TRANSACTION> listDataPrint = GetListSelectedRow();
                if (listDataPrint.Count > 0 && listDataPrint.Any(o => o.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP))
                {
                    var processprint = new PrintProcessor(listDataPrint);
                    if (processprint != null) processprint.Print(true, PrintProcessor.PrintTypeCode__Mps000092);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonDuLieuIn);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrintNow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (BtnPrintNow.Enabled)
                {
                    BtnPrintNow_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrintNowXuatBan_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (BtnPrintNowForXuatBan.Enabled)
                {
                    BtnPrintNowForXuatBan_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBillType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtTemplateCode.Focus();
                    TxtTemplateCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBillType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate)
                {
                    TxtTemplateCode.Focus();
                    TxtTemplateCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtSymbolCode.Focus();
                    TxtSymbolCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtSymbolCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SpNumOrderFrom.Focus();
                    SpNumOrderFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpNumOrderFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SpNumOrderTo.Focus();
                    SpNumOrderTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
