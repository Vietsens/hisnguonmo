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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ApproveServiceReqCLS.ADO;
using HIS.Desktop.Plugins.ApproveServiceReqCLS.Config;
using HIS.Desktop.Plugins.ApproveServiceReqCLS.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Nodes;

namespace HIS.Desktop.Plugins.ApproveServiceReqCLS.ApproveServiceReqCLS
{
    public partial class frmApproveServiceReqCLS : HIS.Desktop.Utility.FormBase
    {
        V_HIS_TREATMENT_FEE hisTreatment;
        V_HIS_PATIENT hispatient = null;

        List<V_HIS_SERE_SERV_5> ListSereServ = new List<V_HIS_SERE_SERV_5>();
        List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<long> lstServiceReqId = new List<long>();
        BindingList<SereServADO1> records;
        List<SereServADO1> SereServADOs = new List<SereServADO1>();
        long? treatmentId = null;
        UserControl ucSereServTree = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        private List<long> listCountServiceReq;

        public frmApproveServiceReqCLS(Inventec.Desktop.Common.Modules.Module _Module, V_HIS_TREATMENT_FEE _Treatment)
            : base(_Module)
        {
            InitializeComponent();
            this.currentModule = _Module;
            this.hisTreatment = _Treatment;
            pictureEdit1.Image = imageCheck.Images[1];
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApproveServiceReqCLS_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                SetDefaultValueControl();
                HisConfigCFG.LoadConfig();
                if (this.hisTreatment != null)
                {
                    txtSearch.Text = this.hisTreatment.TREATMENT_CODE;
                    FillDataToControl(this.hisTreatment);
                }


                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeSereServ()
        {
            try
            {
                ListSereServ = new List<V_HIS_SERE_SERV_5>();
                lstServiceReq = new List<HIS_SERVICE_REQ>();
                lstServiceReqId = new List<long>();
                List<HIS_SERE_SERV_BILL> lstSereServBill = new List<HIS_SERE_SERV_BILL>();
                List<HIS_SERE_SERV_DEPOSIT> lstSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                SereServADOs = new List<SereServADO1>();
         
                HisServiceReqFilter SrFilter = new HisServiceReqFilter();
                SrFilter.TREATMENT_ID = this.hisTreatment.ID;

                var ServiceReqresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, SrFilter, null);

                if (ServiceReqresult != null && ServiceReqresult.Count > 0)
                {
                    lstServiceReq = ServiceReqresult.Where(o => o.IS_NOT_REQUIRE_FEE == 1).ToList();
                }

                HisSereServBillFilter BillFilter = new HisSereServBillFilter();
                BillFilter.TDL_TREATMENT_ID = this.hisTreatment.ID;

                lstSereServBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, BillFilter, null);

                HisSereServDepositFilter DepositFilter = new HisSereServDepositFilter();
                DepositFilter.TDL_TREATMENT_ID = this.hisTreatment.ID;

                lstSereServDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, DepositFilter, null);


                HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                ssFilter.TDL_TREATMENT_ID = this.hisTreatment.ID;

                ListSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        foreach (var Sr in lstServiceReq)
                        {
                            var check = ListSereServ.FirstOrDefault(o => o.SERVICE_REQ_ID == Sr.ID);
                            if (check != null)
                            {
                                ListSereServ.Remove(check);
                            }
                        }
                    }

                    if (lstSereServDeposit != null && lstSereServDeposit.Count > 0)
                    {
                        foreach (var Deposit in lstSereServDeposit)
                        {
                            var check = ListSereServ.FirstOrDefault(o => o.SERVICE_REQ_ID == Deposit.TDL_SERVICE_REQ_ID && o.SERVICE_ID == Deposit.TDL_SERVICE_ID);
                            if (check != null)
                            {
                                ListSereServ.Remove(check);
                            }
                        }
                    }

                    if (lstSereServBill != null && lstSereServBill.Count > 0)
                    {
                        foreach (var Bill in lstSereServBill)
                        {
                            var check = ListSereServ.FirstOrDefault(o => o.SERVICE_REQ_ID == Bill.TDL_SERVICE_REQ_ID && o.SERVICE_ID == Bill.TDL_SERVICE_ID);
                            if (check != null)
                            {
                                ListSereServ.Remove(check);
                            }
                        }
                    }
                }

                if (ListSereServ != null)
                {
                    var sereServs = (from r in ListSereServ select new SereServADO1(r)).ToList();

                    if (sereServs != null && sereServs.Count > 0)
                    {
                        listCountServiceReq = new List<long>();
                        var listGroupBySety = sereServs.GroupBy(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        foreach (var group in listGroupBySety)
                        {
                            var listSub = group.ToList<SereServADO1>();
                            SereServADO1 ssRootSety = new SereServADO1();
                            //ssRootSety.TDL_SERVICE_NAME = listSub.First().TDL_SERVICE_NAME;
                            ssRootSety.CONCRETE_ID__IN_SETY = listSub.First().SERVICE_REQ_ID + "";
                            //ssRootSety.AMOUNT_PLUS = listSub.Sum(o => o.AMOUNT);
                 
                            var result = ServiceReqresult.FirstOrDefault(p => p.ID == listSub.First().SERVICE_REQ_ID.Value && p.IS_NOT_REQUIRE_FEE != 1);

                            if (result != null)
                            {
                                ssRootSety.TDL_SERVICE_CODE = result != null ? result.SERVICE_REQ_CODE : "";
                                //ssRootSety.PARENT_ID__IN_SETY = listSub.First().SERVICE_REQ_ID.ToString();
                                //ssRootSety.AMOUNT = listSub.Sum(o => o.AMOUNT);
                                ssRootSety.VIR_TOTAL_PRICE = listSub.Sum(o => o.VIR_TOTAL_PRICE);
                                ssRootSety.VIR_TOTAL_HEIN_PRICE = listSub.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                                ssRootSety.VIR_TOTAL_PATIENT_PRICE = listSub.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);

                                SereServADOs.Add(ssRootSety);

                                foreach (var item in listSub)
                                {
                                    item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                    item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                    item.IsLeaf = true;
                                    SereServADOs.Add(item);
                                }

                                listCountServiceReq.Add(group.Key);
                            }
                        }
                    }
                    if (SereServADOs != null)
                    {
                        SereServADOs = SereServADOs.OrderBy(o => o.SERVICE_REQ_ID).ThenByDescending(o => o.TDL_SERVICE_CODE).ToList();
                    }
                }

                records = new BindingList<SereServADO1>(SereServADOs);
                if (records != null)
                {
                    treeList1.DataSource = records;
                    if (records.Count > 0)
                    {
                        pictureEdit1.Image = imageCheck.Images[0];
                    }
                    else
                    {
                        pictureEdit1.Image = imageCheck.Images[1];
                    }
                    treeList1.CheckAll();
                    treeList1.ExpandAll();
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtSearch.Text = "";
                lblPatientCode.Text = "";
                lblPatienType.Text = "";
                lblRightRoute.Text = "";
                lblPatientName.Text = "";
                lblHeinCard.Text = "";
                lblHeinRatio.Text = "";
                lblGender.Text = "";
                lblHeinFrom.Text = "";
                lblHeinTo.Text = "";
                lblDOB.Text = "";
                lblAddress.Text = "";
                lblMediOrg.Text = "";

                records = new BindingList<SereServADO1>();
                pictureEdit1.Image = imageCheck.Images[1];
                treeList1.DataSource = null;
                treeList1.ExpandAll();
                txtSearch.Focus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl(V_HIS_TREATMENT_FEE data)
        {
            try
            {
                lblPatientCode.Text = "";
                lblPatientName.Text = "";
                lblDOB.Text = "";
                lblGender.Text = "";
                lblAddress.Text = "";
                lblHeinCard.Text = "";
                lblHeinFrom.Text = "";
                lblHeinTo.Text = "";
                lblMediOrg.Text = "";
                lblPatienType.Text = "";
                lblRightRoute.Text = "";
                lblHeinRatio.Text = "";
                treeList1.DataSource = null;
                pictureEdit1.Image = imageCheck.Images[1];
                if (data != null)
                {
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblAddress.Text = data.TDL_PATIENT_ADDRESS;
                     this.resultPatientType = new BackendAdapter(new CommonParam())
.Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, data.ID, null);
                    
                    if (resultPatientType != null)
                    {
                        lblHeinCard.Text = HIS.Desktop.Utility.HeinCardHelper.TrimHeinCardNumber(resultPatientType.HEIN_CARD_NUMBER);
                        lblHeinFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        lblHeinTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_TO_TIME ?? 0);
                        lblMediOrg.Text = resultPatientType.HEIN_MEDI_ORG_NAME;
                        lblPatienType.Text = resultPatientType.PATIENT_TYPE_NAME ?? "";
                        string rightRoute = "";
                        if (resultPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = "Đúng tuyến";
                                //Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_APPROVESERVICEREQCLS__RIGHT_ROUTE_TRUE", ResourceLangManager.LanguagefrmApproveServiceReqCLS, LanguageManager.GetCulture());
                        }
                        else
                        {
                            rightRoute = "Trái tuyến";
                                //Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_APPROVESERVICEREQCLS__RIGHT_ROUTE_FALSE", ResourceLangManager.LanguagefrmApproveServiceReqCLS, LanguageManager.GetCulture());
                        }
                        lblRightRoute.Text = rightRoute ?? "";
                       
                        string ratio = "";

                        if (resultPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            //HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(resultPatientType.HEIN_TREATMENT_TYPE_CODE, resultPatientType.HEIN_CARD_NUMBER, resultPatientType.LEVEL_CODE, resultPatientType.RIGHT_ROUTE_CODE, (data.TOTAL_HEIN_PRICE ?? 0 + data.TOTAL_PATIENT_PRICE_BHYT ?? 0));
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }
                        lblHeinRatio.Text = ratio ?? "";
                    }

                    LoadDataToTreeSereServ();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Không thể tách thẻ BHYT");
            }
            return result;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                treatmentId = null;
                hisTreatment = null;

                LoadSearch();
                FillDataToControl(hisTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                if (!String.IsNullOrEmpty(txtSearch.Text))
                {
                    hisTreatment = new V_HIS_TREATMENT_FEE();
                    string code = txtSearch.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtSearch.Text = code;
                    }
                    filter.TREATMENT_CODE = txtSearch.Text;
                    var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null)
                    {
                        hisTreatment = listTreatment.FirstOrDefault();
                        treatmentId = hisTreatment.ID;
                        Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("", hisTreatment));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Không tìm thấy mã điều trị");
                        param.Messages.Add("Không tìm thấy mã điều trị");
                        return;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("NULL SEARCH");
                }
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
                SetDefaultValueControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                
                if (e.IsGetData && e.Row != null)
                {
                    SereServADO1 rowData = e.Row as SereServADO1;
                    if (rowData != null )
                    {
                        if (!e.Node.HasChildren)
                        {

                            if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(rowData.VIR_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {

                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {

                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {

                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VAT_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(rowData.VAT);
                            }
                        }
                        else
                        {
 
                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_HEIN_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_PATIENT_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(rowData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is SereServADO1)
                {
                    var rowData = data as SereServADO1;

                    if (rowData != null && e.Node.HasChildren)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                        if (e.Node.Nodes != null)
                        {
                            e.Appearance.BackColor = Color.Khaki;

                        }

                        
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTotalPriceOfChildChoice(SereServADO1 data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO1)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren && item.Checked)
                        {
                   
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                        else if (item.HasChildren)
                        {
                         
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                    }
                }
                if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                {
                   
                    data.VIR_TOTAL_PRICE = totalChoicePrice;
                    }
                else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                {
                    
                    data.VIR_TOTAL_HEIN_PRICE = totalChoicePrice;
                   }
                else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                {
                   
                    data.VIR_TOTAL_PATIENT_PRICE = totalChoicePrice;
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

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool? success = false;
                CommonParam param = new CommonParam();
                success = SaveProcess(ref param);
                WaitingManager.Hide();
                if (success == true)
                {
                    MessageManager.Show(this, param, success);
                    treeList1.DataSource = null;
                    pictureEdit1.Image = imageCheck.Images[0];
                    LoadDataToTreeSereServ();
                }
                else
                {
                    MessageManager.Show(this, param, success);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool? SaveProcess(ref CommonParam param)
        {
            bool? success = false;
            try
            {
                
                var listData = this.GetListCheck();
                if (listData == null || listData.Count == 0)
                {
                    return success;
                }
                else
                {                    
                    var list = listData.Where(o => o.SERVICE_REQ_ID.HasValue).Select(o => o.SERVICE_REQ_ID.Value).Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Debug("Dau vao khi goi api: HisServiceReq/UpdateIsNotRequireFee: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => list), list));
                    var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_REQ>>("api/HisServiceReq/UpdateIsNotRequireFee", ApiConsumers.MosConsumer, list, param);
                    if (rs != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("HisServiceReq/UpdateIsNotRequireFee rs != null");
                        success = true;
                        Inventec.Common.Logging.LogSystem.Debug("Dau ra khi goi api: HisServiceReq/UpdateIsNotRequireFee: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("HisServiceReq/UpdateIsNotRequireFee rs == null");
                        success = false;
                    }
                }

            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        private List<SereServADO1> GetListCheck()
        {
            List<SereServADO1> result = new List<SereServADO1>();
            try
            {
                foreach (TreeListNode node in treeList1.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void GetListNodeCheck(ref List<SereServADO1> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((SereServADO1)treeList1.GetDataRecordByNode(node));
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

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void treeList1_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    //var data = (SereServADO1)treeList1.GetDataRecordByNode(e.Node);
                    if (!e.Node.HasChildren)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void pictureEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureEdit1.Image == imageCheck.Images[1])
                {
                    pictureEdit1.Image = imageCheck.Images[0];

                    if (this.SereServADOs != null)
                    {
                        treeList1.CheckAll();
                    }

                }
                else
                {
                    pictureEdit1.Image = imageCheck.Images[1];

                    if (this.SereServADOs != null)
                    {
                        treeList1.UncheckAll();
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_AfterCheckNode(object sender, NodeEventArgs e)
        {
            try
            {
                

                int dem = 0;
                foreach (TreeListNode node in treeList1.Nodes)
                {
                    if (node.HasChildren && node.Checked)
                    {
                        dem++;
                       
                    }                  
                }
                Inventec.Common.Logging.LogSystem.Error(listCountServiceReq.Count().ToString());
                if (dem == this.listCountServiceReq.Count())
                {
                    pictureEdit1.Image = imageCheck.Images[0];
                }
                else
                {
                    pictureEdit1.Image = imageCheck.Images[1];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    
                    e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                    if (e.Node.Checked)
                    {
                        e.Node.UncheckAll();
                    }
                    else
                    {
                        e.Node.CheckAll();
                    }
                    while (e.Node.ParentNode != null)
                    {
                       ;
                        bool valid = false;
                        foreach (DevExpress.XtraTreeList.Nodes.TreeListNode item in e.Node.ParentNode.Nodes)
                        {
                            if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                            {
                                valid = true;
                                break;
                            }
                        }
                        if (valid)
                        {
                            e.Node.ParentNode.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            e.Node.ParentNode.CheckState = CheckState.Unchecked;
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
}
