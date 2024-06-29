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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
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
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;

namespace HIS.Desktop.Plugins.CreateTransReqQR.CreateTransReqQR
{
    public partial class frmCreateTransReqQR : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;
        TransReqQRADO inputTransReq { get; set; }
        List<V_HIS_SERE_SERV_5> sereServByTreatment;
        V_HIS_TREATMENT hisTreatmentView;
        int SetDefaultDepositPrice;
        HIS_TRANS_REQ currentTransReq { get; set; }
        bool IsCheckNode { get; set; }
        public frmCreateTransReqQR(Inventec.Desktop.Common.Modules.Module currentModule, TransReqQRADO ado) : base(currentModule)
        {
            InitializeComponent();

            try
            {
                this.inputTransReq = ado;
                this.currentModule = currentModule;
                this.SetDefaultDepositPrice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT");
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmCreateTransReqQR_Load(object sender, EventArgs e)
        {

            try
            {
                HisConfigCFG.LoadConfig();
                btnCreate.Enabled = false;
                this.InitSereServTree();
                RegisterTimer(GetModuleLink(), "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                StartTimer(GetModuleLink(), "timerInitForm");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void timerInitForm_Tick()
        {
            try
            {
                StopTimer(GetModuleLink(), "timerInitForm");
                LoadTreatment();
                LoadSereServByTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.inputTransReq.TreatmentId;
                hisTreatmentView = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        decimal Amount = 0;
        private async Task LoadSereServByTreatment()
        {
            try
            {
                this.sereServByTreatment = GetSereByTreatmentId();
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                {
                    XtraMessageBox.Show("Hồ sơ không có dịch vụ cần tạm ứng");
                    this.Close();
                    return;
                }
                // bỏ những dịch vụ không thực hiện (IS_NO_EXECUTE), không cho phép thanh toán hoặc tạm ứng (IS_NO_PAY)
                this.sereServByTreatment = this.sereServByTreatment.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_NO_PAY != 1).ToList();
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                    return;


                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                hisSereServBillFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                hisSereServBillFilter.IS_NOT_CANCEL = true;
                var sereServBills = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServBillFilter, param);

                if (sereServBills != null && sereServBills.Count > 0)
                {
                    List<long> SereServBillIds = sereServBills.Select(o => o.SERE_SERV_ID).ToList();
                    // lọc các sereServ đã thanh toán
                    this.sereServByTreatment = this.sereServByTreatment.Where(o => !SereServBillIds.Contains(o.ID)).ToList();
                }

                // kiểm tra có trong his_sere_serv_debt chua neu co roi thi bo qua
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                    return;

                MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                sereServDebtFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                var sereServDebtList = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);
                if (sereServDebtList != null && sereServDebtList.Count > 0)
                {
                    sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                    this.sereServByTreatment = sereServDebtList != null && sereServDebtList.Count > 0
                        ? this.sereServByTreatment.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                        : this.sereServByTreatment;
                }

                //FilterSereServBill(ref this.sereServByTreatment);
                //if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                //    return;               

                param = new CommonParam();
                MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                sereServDepositFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                var sereServDeposits = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                List<V_HIS_SERE_SERV_5> sereServByTreatmentProcess = new List<V_HIS_SERE_SERV_5>();

                this.FilterSereServDepositAndRepay(ref this.sereServByTreatment, sereServDeposits);

                this.ssTreeProcessor.Reload(this.ucSereServTree, this.sereServByTreatment);
                btnCreate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> GetSeSeDepoRePay(long treatmentId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> seseDepoRepays = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisSeseDepoRepayFilter seseDepositRepayFilter = new HisSeseDepoRepayFilter();
                seseDepositRepayFilter.TDL_TREATMENT_ID = treatmentId;
                seseDepoRepays = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, seseDepositRepayFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return seseDepoRepays;
        }

        /// <summary>
        /// lọc các sereServ đã được tạm ứng và hoàn ứng
        /// </summary>
        void FilterSereServDepositAndRepay(ref List<V_HIS_SERE_SERV_5> sereServByTreatmentSDOProcess, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> sereServDepositByTreatments)
        {
            List<V_HIS_SERE_SERV_5> ListSereServByTreatmentSDOResult = new List<V_HIS_SERE_SERV_5>();
            try
            {

                // lấy List sereServDeposit có IS_CANCEL !=1
                //var sereServDepositByTreatments = GetSereServDepositByTreatment(this.treatmentId);
                if (sereServDepositByTreatments != null && sereServDepositByTreatments.Count > 0)
                {
                    var sereServDepositByTreatmentNotCancels = sereServDepositByTreatments.Where(o => o.IS_CANCEL != 1).ToList();
                    if (sereServDepositByTreatmentNotCancels != null && sereServDepositByTreatmentNotCancels.Count > 0)
                    {
                        // lấy list SereServDepositRepays có IS_CANCEL !=1
                        var seseDepoRepays = GetSeSeDepoRePay(this.inputTransReq.TreatmentId);
                        if (seseDepoRepays != null && seseDepoRepays.Count > 0)
                        {
                            var seseDepoRepayNotCancels = seseDepoRepays.Where(o => o.IS_CANCEL != 1).ToList();
                            if (seseDepoRepayNotCancels != null && seseDepoRepayNotCancels.Count > 0)
                            {
                                List<long> seseDepoIds = seseDepoRepayNotCancels.Select(o => o.SERE_SERV_DEPOSIT_ID).ToList();
                                // lấy List sereServDeposit không có trong list SereServDepositRepays
                                var sereServDepositNotContainRepays = sereServDepositByTreatmentNotCancels.Where(o => !seseDepoIds.Contains(o.ID)).ToList();
                                ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess.Where(o => !sereServDepositNotContainRepays.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                            }
                            else
                            {
                                var ListSereServByTreatmentSDOResult1 = sereServByTreatmentSDOProcess.Where(o => !sereServDepositByTreatmentNotCancels.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                                if (ListSereServByTreatmentSDOResult1 != null && ListSereServByTreatmentSDOResult1.Count > 0)
                                    ListSereServByTreatmentSDOResult.AddRange(ListSereServByTreatmentSDOResult1);
                            }
                        }
                        else
                        {
                            var ListSereServByTreatmentSDOResult1 = sereServByTreatmentSDOProcess.Where(o => !sereServDepositByTreatmentNotCancels.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                            if (ListSereServByTreatmentSDOResult1 != null && ListSereServByTreatmentSDOResult1.Count > 0)
                                ListSereServByTreatmentSDOResult.AddRange(ListSereServByTreatmentSDOResult1);
                        }
                    }
                    else
                    {
                        ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess;
                    }
                }
                else if (sereServByTreatmentSDOProcess != null && sereServByTreatmentSDOProcess.Count > 0)
                {
                    ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess;
                }

                // bỏ những dữ liệu trùng

                sereServByTreatmentSDOProcess = (ListSereServByTreatmentSDOResult != null && ListSereServByTreatmentSDOResult.Count > 0) ? ListSereServByTreatmentSDOResult.GroupBy(o => o.ID).Select(g => g.FirstOrDefault()).ToList() : ListSereServByTreatmentSDOResult;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> GetSereByTreatmentId()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                sereServFilter.IS_EXPEND = false;
                var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("/api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, null);
                if (apiData != null && apiData.Count > 0)
                {
                    rs = apiData.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && (o.VIR_TOTAL_PATIENT_PRICE ?? 0) > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        #region InitUC
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
        private void sereServTree_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {

                if (data != null && !e.Node.HasChildren)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.DISCOUNT ?? 0);
                    }
                    if (e.Column.FieldName == "AMOUNT_PLUS_STR")
                    {
                        e.Value = ConvertNumberToString(data.AMOUNT);
                    }
                    if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.TDL_INTRUCTION_TIME);
                    }
                }
                if (data != null && e.Node.HasChildren && data.VIR_TOTAL_PRICE > 0)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                    }
                }
                if (data != null && e.Node.HasChildren && data.VIR_TOTAL_PATIENT_PRICE > 0)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    }
                }
                if (data != null && e.Node.HasChildren && data.AMOUNT > 0)
                {
                    if (e.Column.FieldName == "AMOUNT_PLUS_STR")
                    {
                        e.Value = ConvertNumberToString(data.AMOUNT);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void treeSereServ_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (sender != null && sender is SereServADO && e.Node.HasChildren && e.Column.FieldName == "TDL_SERVICE_NAME")
                {
                    var data = (SereServADO)sender;
                    if (data.IsFather == true)
                        e.Appearance.ForeColor = Color.Red;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void sereServTree_AfterCheck(TreeListNode node, SereServADO data)
        {
            try
            {
                CalCulateTotalAmountDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_CheckAllNode(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.CheckAll();
                        CheckNode(node);
                    }
                }
                CalCulateTotalAmountDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CalCulateTotalAmountDeposit()
        {
            try
            {
                List<SereServADO> listCheckeds = ssTreeProcessor.GetListCheck(ucSereServTree);

                ChangeCheckedNodes(listCheckeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChangeCheckedNodes(List<SereServADO> listCheckeds)
        {
            try
            {
                Amount = 0;
                lblAmount.Text = "0";
                foreach (var item in listCheckeds)
                {
                    if (item != null && (item.IsLeaf ?? false))
                    {
                        decimal totalPatientPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                        Amount += totalPatientPrice;
                    }
                }
                lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(Amount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void sereServTree_BeforeCheck(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node != null)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && !IsCheckNode)
                    {
                        e.CanCheck = false;
                        node.CheckAll();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        childNode.CheckAll();
                        CheckNode(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void sereServTree_ShowingEditorDG(TreeListNode node, object sender)
        {
            try
            {
                var nodeData = node.TreeList.GetDataRecordByNode(node);
                if (nodeData != null)
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
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
                this.ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsShowCheckNode = true;
                ado.isAdvance = true;
                ado.SereServs = this.sereServByTreatment;
                ado.SereServTree_CustomUnboundColumnData = sereServTree_CustomUnboundColumnData;

                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                //ado.SelectImageCollection = this.imageCollection1;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_AfterCheck = sereServTree_AfterCheck;
                ado.SereServTreeForBill_BeforeCheck = sereServTree_BeforeCheck;
                ado.sereServTree_ShowingEditor = sereServTree_ShowingEditorDG;
                ado.SereServTree_CheckAllNode = sereServTree_CheckAllNode;


                ado.LayoutSereServExpend = "Hao phí";
                //Cột Tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn("Tên dịch vụ", "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);
                //Cột Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn("Số lượng", "AMOUNT_PLUS_STR", 60, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                amountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(amountCol);
                //Cột Đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn("Đơn giá", "VIR_PRICE_DISPLAY", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);
                //Cột thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn("Thành tiền", "VIR_TOTAL_PRICE_DISPLAY", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);
                //Cột Đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn("Đồng chi trả", "VIR_TOTAL_HEIN_PRICE_DISPLAY", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn("Bệnh nhân trả", "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);
                //Chiếu khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn("Chiết khấu", "DISCOUNT_DISPLAY", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virDiscountCol);
                //Hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn("Hao phí", "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);
                //
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn("VAT %", "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                SereServTreeColumn serviceCodeCol = new SereServTreeColumn("Mã dịch vụ", "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);
                SereServTreeColumn intructionTime = new SereServTreeColumn("Thời gian chỉ định", "TDL_INTRUCTION_TIME_STR", 130, false);
                intructionTime.VisibleIndex = 11;
                intructionTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(intructionTime);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlTreeSereServ.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void timerReloadTransReq_Tick(object sender, EventArgs e)
        {

            try
            {
                if (currentTransReq != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTransReqFilter sereServFilter = new HisTransReqFilter();
                    sereServFilter.ID = currentTransReq.ID;
                    var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("/api/HisTransReq/Get", ApiConsumers.MosConsumer, sereServFilter, null);

                    if (apiData != null && apiData.Count > 0)
                    {
                        currentTransReq = apiData[0];
                        if (currentTransReq.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)
                        {
                            timerReloadTransReq.Stop();
                            if (currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED)
                            {
                                btnPrint.Enabled = btnNew.Enabled = btnCreate.Enabled = false;
                                if (inputTransReq.DelegtePrint != null)
                                    inputTransReq.DelegtePrint();
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

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = btnNew.Enabled = false;
                CommonParam param = new CommonParam();
                TransReqCreateSDO sdo = new TransReqCreateSDO();
                sdo.TreatmentId = this.inputTransReq.TreatmentId;
                sdo.TransReqType = 2; //Loại yêu cầu thanh toán. Giá trị mặc định 2(Yeu cau thanh toan theo so tien con thieu (co gan voi dich vu))
                sdo.SereServIds = this.sereServByTreatment.Select(o => o.ID).ToList();
                sdo.RequestRoomId = this.currentModule.RoomId;
                sdo.Amount = this.Amount;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                currentTransReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateSDO", ApiConsumers.MosConsumer, sdo, param);
                if (currentTransReq == null)
                {
                    XtraMessageBox.Show(string.Format("Tạo QR tạm ứng thất bại. {0}", param.GetMessage()));
                }
                else
                {
                    btnPrint.Enabled = btnNew.Enabled = true;
                    btnCreate.Enabled = false;
                    ShowQR();
                    timerReloadTransReq.Start();
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
                timerReloadTransReq.Stop();
                if (currentTransReq != null)
                {
                    IsCheckNode = false;
                    CommonParam param = new CommonParam();
                    currentTransReq.TRANS_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTransReq), currentTransReq));
                    currentTransReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/Update", ApiConsumers.MosConsumer, currentTransReq, param);
                    if (currentTransReq != null)
                    {
                        pbQr.Image = null;
                        IsCheckNode = true;
                        btnCreate.Enabled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ShowQR()
        {
            try
            {
                if (currentTransReq == null)
                    return;
                var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(currentTransReq, new List<HIS_CONFIG>() { inputTransReq.ConfigValue }).FirstOrDefault();
                using (var ms = new MemoryStream((byte[])data.Value))
                {
                    pbQr.Image = Image.FromStream(ms);
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
                if (currentTransReq == null) return;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000498", DelegateRunPrinter);
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
                    case "Mps000498":
                        LoadBieuMau(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void LoadBieuMau(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((hisTreatmentView != null ? hisTreatmentView.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                MPS.Processor.Mps000498.PDO.Mps000498PDO rdo = new MPS.Processor.Mps000498.PDO.Mps000498PDO(
                        hisTreatmentView,
                        currentTransReq,
                        new List<HIS_CONFIG>() { inputTransReq.ConfigValue }
                        );
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmCreateTransReqQR_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (currentTransReq != null && currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)
                {
                    if (XtraMessageBox.Show("QR chưa được xử lý. Bạn có muốn hủy hay không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        btnNew_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void PRINT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled && btnPrint.Visible)
                btnPrint_Click(null, null);
        }

        private void NEW_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnNew.Enabled && btnNew.Visible)
                btnNew_Click(null, null);
        }

        private void CREATE_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnCreate.Enabled && btnCreate.Visible)
                btnCreate_Click(null, null);
        }
    }
}
