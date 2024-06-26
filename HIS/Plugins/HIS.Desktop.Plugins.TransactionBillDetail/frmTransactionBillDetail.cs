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
using HIS.Desktop.ApiConsumer;
using HIS.UC.SereServTree;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillDetail
{
    public partial class frmTransactionBillDetail : HIS.Desktop.Utility.FormBase
    {
        SereServTreeProcessor ssTreeProcessor = null;
        UserControl ucSereServTree = null;

        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        long billId = 0;

        public frmTransactionBillDetail(Inventec.Desktop.Common.Modules.Module module, long data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.billId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                InitSereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
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

        private void InitSereServTree()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = false;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.LayoutSereServExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__LAYOUT_SERE_SERV_EXPEND", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                //virIsExpendCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                //Column mã giao dịch
                SereServTreeColumn INSURANCE_EXPERTISECodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                INSURANCE_EXPERTISECodeCol.VisibleIndex = 11;
                ado.SereServTreeColumns.Add(INSURANCE_EXPERTISECodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBillDetail_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.billId > 0)
                {
                    WaitingManager.Show();
                    FillDataToSereServTree();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToSereServTree()
        {
            try
            {
                HisTransactionViewFilter billFilter = new HisTransactionViewFilter();
                billFilter.ID = this.billId;
                var listBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, billFilter, null);
                if (listBill == null 
                    //|| listBill.Count != 1
                    )
                {
                    throw new Exception("Khong lay duoc HIS_BILL the billId: " + this.billId);
                }
                var hisBill = listBill;

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.billId;
                var listSereServBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);

                if (listSereServBill == null || listSereServBill.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId" + this.billId);
                }
                foreach (var item in listSereServBill)
                {
                    if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                        dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                    dicSereServBill[item.SERE_SERV_ID].Add(item);
                }
                HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                ssFilter.TDL_TREATMENT_ID = hisBill.FirstOrDefault().TREATMENT_ID;
                var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                List<V_HIS_SERE_SERV_5> listSereServ = new List<V_HIS_SERE_SERV_5>();
                foreach (var item in listData)
                {
                    if (dicSereServBill.ContainsKey(item.ID))
                        listSereServ.Add(item);
                }
                ssTreeProcessor.Reload(ucSereServTree, listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        if (dicSereServBill.ContainsKey(data.ID))
                        {
                            var ssBills = dicSereServBill[data.ID];
                            var totalPrice = ssBills.Sum(s => s.PRICE);
                            if (totalPrice == data.VIR_TOTAL_PATIENT_PRICE)
                                e.Appearance.ForeColor = Color.Blue;
                            else
                                e.Appearance.ForeColor = Color.Green;
                        }
                        else if (e.Node.Checked)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
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
