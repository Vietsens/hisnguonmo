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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ElectronicBillTotal.ADO;
using HIS.Desktop.Plugins.ElectronicBillTotal.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ElectronicBillTotal
{
    public partial class FormElectronicBillTotal : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private V_HIS_TREATMENT_FEE CurrentTreatment;
        private List<SereServADO> CurrentSereServs;
        private List<HIS_SERE_SERV_BILL> currentSereServsBill;
        private List<V_HIS_TRANSACTION> ListCurrentTransaction;
        private V_HIS_TRANSACTION resultTranBill;

        private bool isNotLoadWhileChangeControlStateInFirst;
        private ControlStateWorker controlStateWorker;
        private List<ControlStateRDO> currentControlStateRDO;

        public FormElectronicBillTotal()
            : base()
        {
            InitializeComponent();
        }

        public FormElectronicBillTotal(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_TREATMENT_FEE treatment, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter)
            : base(moduleData)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.ModuleData = moduleData;
            this.CurrentTreatment = treatment;
            try
            {
                SetIcon();
                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormElectronicBillTotal_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                HisConfigCFG.LoadConfig();
                InitControlState();
                this.ResetControlValue();
                if (this.CurrentTreatment != null && this.CurrentTreatment.TREATMENT_CODE != null)
                {
                    this.txtTreatmentCode.Text = this.CurrentTreatment.TREATMENT_CODE;
                    this.txtTreatmentCode.SelectionStart = this.txtTreatmentCode.Text.Length;
                    this.txtTreatmentCode.DeselectAll();
                }
                RegisterTimer(ModuleData.ModuleLink, "timerLoadData", timerLoadData.Interval, timerLoadData_Tick);
                StartTimer(ModuleData.ModuleLink, "timerLoadData");
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleData.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkHideHddt.Name)
                        {
                            chkHideHddt.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControlValue()
        {
            try
            {
                bBtnExport.Enabled = true;
                btnPrint.Enabled = false;
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ElectronicBillTotal.Resources.Lang", typeof(HIS.Desktop.Plugins.ElectronicBillTotal.FormElectronicBillTotal).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportBill.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.btnExportBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHideHddt.Properties.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.chkHideHddt.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkVienPhi.Properties.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.chkVienPhi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNhaThuoc.Properties.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.chkNhaThuoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatienType.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciPatienType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientAddress.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciPatientAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciPatientDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediOrg.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciMediOrg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCard.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciHeinCard.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinFrom.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciHeinFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinTo.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciHeinTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRightRoute.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciRightRoute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinRatio.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciHeinRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHideHddt.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.lciHideHddt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_Amount.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_Price.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_VatRatio.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_TotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_TotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_TotalHeinPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_TotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.tc_TotalPatientPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bBtnSearch.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.bBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bBtnExport.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.bBtnExport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bBtnNew.Caption = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.bBtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormElectronicBillTotal.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerLoadData_Tick()
        {
            try
            {
                StopTimer(ModuleData.ModuleLink, "timerLoadData");
                this.FillInfoPatient(this.CurrentTreatment);
                this.LoadDataToTreeSereServ();//TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillInfoPatient(V_HIS_TREATMENT_FEE data)
        {
            try
            {
                lblPatientCode.Text = "";
                lblPatientName.Text = "";
                lblPatientDob.Text = "";
                lblGender.Text = "";
                lblPatientAddress.Text = "";
                lblHeinCard.Text = "";
                lblHeinFrom.Text = "";
                lblHeinTo.Text = "";
                lblMediOrg.Text = "";
                lblRightRoute.Text = "";
                lblHeinRatio.Text = "";
                lblPatienType.Text = "";

                if (data != null)
                {
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblPatientAddress.Text = data.TDL_PATIENT_ADDRESS;

                    var LastPatientType = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, CurrentTreatment.ID, null);
                    if (LastPatientType != null)
                    {
                        lblHeinCard.Text = HeinCardHelper.TrimHeinCardNumber(LastPatientType.HEIN_CARD_NUMBER);
                        lblHeinFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        lblHeinTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_TO_TIME ?? 0);
                        lblMediOrg.Text = LastPatientType.HEIN_MEDI_ORG_NAME;
                        lblPatienType.Text = LastPatientType.PATIENT_TYPE_NAME ?? "";
                        string rightRoute = "";
                        if (LastPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_ELECTRONIC_BILL_TOTAL__RIGHT_ROUTE_TRUE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                        }
                        else
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_ELECTRONIC_BILL_TOTAL__RIGHT_ROUTE_FALSE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                        }

                        lblRightRoute.Text = rightRoute ?? "";
                        string ratio = "";
                        if (LastPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(LastPatientType.HEIN_TREATMENT_TYPE_CODE, LastPatientType.HEIN_CARD_NUMBER, LastPatientType.LEVEL_CODE, LastPatientType.RIGHT_ROUTE_CODE);
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }

                        lblHeinRatio.Text = ratio ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private decimal GetTotalPriceOfTreatment()
        //{
        //    decimal result = 0;
        //    try
        //    {
        //        if (this.currentSereServs != null)
        //        {
        //            foreach (var item in this.currentSereServs)
        //            {
        //                if (item.IS_DELETE == 1 || !item.SERVICE_REQ_ID.HasValue || item.IS_EXPEND == 1 || item.IS_NO_EXECUTE == 1 || item.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
        //                    continue;
        //                decimal totalPrice = (item.VIR_TOTAL_HEIN_PRICE ?? 0) + (item.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
        //                result += totalPrice;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        result = 0;
        //    }
        //    return result;
        //}

        private void LoadDataToTreeSereServ()
        {
            try
            {
                currentSereServsBill = new List<HIS_SERE_SERV_BILL>();
                ListCurrentTransaction = new List<V_HIS_TRANSACTION>();
                CurrentSereServs = new List<SereServADO>();

                if (this.CurrentTreatment != null && CurrentTreatment.ID > 0)
                {
                    //lấy danh sách giao dịch
                    if (chkNhaThuoc.Checked)
                    {
                        #region exp_mest
                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.TREATMENT_ID = CurrentTreatment.ID;
                        tranFilter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                        tranFilter.HAS_INVOICE_CODE = false;
                        tranFilter.IS_CANCEL = false;
                        tranFilter.HAS_SALE_TYPE_ID = true;

                        ListCurrentTransaction = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);
                        if (ListCurrentTransaction != null && ListCurrentTransaction.Count > 0)
                        {
                            //chế biến danh sách chi tiết xuất HIS_BILL_GOODS -->  V_HIS_SERE_SERV_5
                            HisBillGoodsFilter billGoodsFilter = new HisBillGoodsFilter();
                            billGoodsFilter.BILL_IDs = ListCurrentTransaction.Select(s => s.ID).ToList();
                            var billGoods = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodsFilter, null);
                            foreach (var item in billGoods)
                            {
                                SereServADO sereServBill = new SereServADO();
                                sereServBill.ID = item.ID;
                                sereServBill.BILL_ID = item.BILL_ID;

                                var bill = ListCurrentTransaction.FirstOrDefault(o => o.ID == item.BILL_ID);
                                if (bill != null)
                                {
                                    sereServBill.BILL_ID = bill.ID;
                                    sereServBill.TRANSACTION_CODE = bill.TRANSACTION_CODE;
                                    sereServBill.TRANSACTION_TIME = bill.TRANSACTION_TIME;
                                    sereServBill.TRANSACTION_AMOUNT = bill.AMOUNT;
                                    sereServBill.CASHIER_LOGINNAME = bill.CASHIER_LOGINNAME;
                                    sereServBill.CASHIER_USERNAME = bill.CASHIER_USERNAME;
                                }

                                sereServBill.AMOUNT = item.AMOUNT;
                                sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                                //sereServBill.TDL_SERVICE_CODE = item.MEDI_MATE_TYPE_CODE;
                                sereServBill.TDL_SERVICE_NAME = item.GOODS_NAME;
                                //sereServBill.DESCRIPTION = item.DESCRIPTION;
                                sereServBill.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                                sereServBill.DISCOUNT = item.DISCOUNT;
                                sereServBill.VIR_PRICE = item.PRICE;
                                sereServBill.VIR_TOTAL_PRICE = sereServBill.VIR_TOTAL_PATIENT_PRICE = (sereServBill.VIR_PRICE ?? 0) * sereServBill.AMOUNT;

                                //var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                                //if (service != null)
                                //{
                                //    sereServBill.TDL_SERVICE_TAX_RATE_TYPE = service.TAX_RATE_TYPE;
                                //}
                                if (item.MEDICINE_TYPE_ID.HasValue)
                                {
                                    var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID.Value);
                                    if (medicine != null)
                                    {
                                        sereServBill.TDL_SERVICE_CODE = medicine.MEDICINE_TYPE_CODE;
                                        sereServBill.TDL_SERVICE_NAME = medicine.MEDICINE_TYPE_NAME;
                                        sereServBill.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                                    }
                                }
                                else if (item.MATERIAL_TYPE_ID.HasValue)
                                {
                                    var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID.Value);
                                    if (material != null)
                                    {
                                        sereServBill.TDL_SERVICE_CODE = material.MATERIAL_TYPE_CODE;
                                        sereServBill.TDL_SERVICE_NAME = material.MATERIAL_TYPE_NAME;
                                        sereServBill.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                                    }
                                }

                                sereServBill.AMOUNT_PLUS = item.AMOUNT;
                                sereServBill.VAT = (item.VAT_RATIO ?? 0) * 100;
                                sereServBill.AMOUNT_DISPLAY = Inventec.Common.Number.Convert.NumberToString(item.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);

                                CurrentSereServs.Add(sereServBill);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region sere_serv
                        var currentSereServ5s = new List<V_HIS_SERE_SERV_5>();

                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.TREATMENT_ID = CurrentTreatment.ID;
                        tranFilter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                        tranFilter.HAS_INVOICE_CODE = false;
                        tranFilter.IS_CANCEL = false;
                        tranFilter.HAS_SALE_TYPE_ID = false;

                        ListCurrentTransaction = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);
                        if (ListCurrentTransaction != null && ListCurrentTransaction.Count > 0)
                        {
                            //lấy chi tiết dựa vào danh sách giao dịch
                            HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                            ssBillFilter.BILL_IDs = ListCurrentTransaction.Select(s => s.ID).ToList();
                            ssBillFilter.IS_NOT_CANCEL = true;
                            currentSereServsBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                            if (currentSereServsBill != null && currentSereServsBill.Count > 0)
                            {
                                int skip = 0;
                                List<long> sereServIds = currentSereServsBill.Select(s => s.SERE_SERV_ID).Distinct().ToList();

                                while (sereServIds.Count - skip > 0)
                                {
                                    var listIds = sereServIds.Skip(skip).Take(100).ToList();
                                    skip += 100;

                                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                                    ssFilter.IDs = listIds;
                                    var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                                    if (hisSereServs != null && hisSereServs.Count > 0)
                                    {
                                        currentSereServ5s.AddRange(hisSereServs);
                                    }
                                }
                            }
                        }

                        Dictionary<long, V_HIS_TRANSACTION> dicTran = new Dictionary<long, V_HIS_TRANSACTION>();
                        if (ListCurrentTransaction != null)
                        {
                            dicTran = ListCurrentTransaction.ToDictionary(o => o.ID, o => o);
                        }

                        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                        if (currentSereServsBill != null && currentSereServsBill.Count > 0)
                        {
                            foreach (var item in currentSereServsBill)
                            {
                                if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                {
                                    dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                                }

                                dicSereServBill[item.SERE_SERV_ID].Add(item);
                            }
                        }

                        foreach (var sereServ in currentSereServ5s)
                        {
                            if (dicSereServBill.ContainsKey(sereServ.ID))
                            {
                                foreach (var ssb in dicSereServBill[sereServ.ID])
                                {
                                    if (dicTran.ContainsKey(ssb.BILL_ID))
                                    {
                                        CurrentSereServs.Add(new SereServADO(sereServ, dicTran[ssb.BILL_ID]));
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }

                BindTreePlus(CurrentSereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindTreePlus(List<SereServADO> sereServs)
        {
            try
            {
                var listSereServs = new List<SereServADO>();
                if (sereServs != null)
                {
                    var listRoot = sereServs.GroupBy(o => o.BILL_ID).ToList();
                    foreach (var rootBill in listRoot)
                    {
                        var listByBill = rootBill.ToList<SereServADO>();
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = listByBill.First().BILL_ID + "";
                        ssRootPaty.BILL_ID = listByBill.First().BILL_ID;
                        ssRootPaty.TDL_SERVICE_CODE = listByBill.First().TRANSACTION_CODE;
                        ssRootPaty.TDL_SERVICE_NAME = string.Format("{0} - {1} ({2})", listByBill.First().CASHIER_LOGINNAME, listByBill.First().CASHIER_USERNAME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(listByBill.First().TRANSACTION_TIME));
                        ssRootPaty.VIR_TOTAL_PATIENT_PRICE = listByBill.First().TRANSACTION_AMOUNT;
                        listSereServs.Add(ssRootPaty);
                        foreach (var item in listByBill)
                        {
                            item.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + item.ID;
                            item.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                            item.IsLeaf = true;
                            listSereServs.Add(item);
                        }
                    }

                    listSereServs = listSereServs.OrderBy(o => o.BILL_ID).ThenBy(o => o.TDL_SERVICE_CODE).ToList();
                }

                var records = new BindingList<SereServADO>(listSereServs);
                trvService.DataSource = records;
                trvService.ExpandAll();

                CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckAllNode(TreeListNodes nodes)
        {
            try
            {
                if (nodes != null)
                {
                    foreach (TreeListNode node in nodes)
                    {
                        node.CheckAll();
                    }
                }
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
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        CurrentTreatment = listTreatment.FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("", CurrentTreatment));
                    }
                    else
                    {
                        WaitingManager.Hide();
                        param.Messages.Add(Resources.ResourceLanguageManager.KhongTimThayMaDieuTri);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVienPhi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVienPhi.Checked)
                {
                    chkNhaThuoc.Checked = false;
                }
                else if (!chkNhaThuoc.Checked)
                {
                    chkVienPhi.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNhaThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNhaThuoc.Checked)
                {
                    chkVienPhi.Checked = false;
                }
                else if (!chkVienPhi.Checked)
                {
                    chkNhaThuoc.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHideHddt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideHddt.Name && o.MODULE_LINK == ModuleData.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHideHddt.Name;
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleData.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (e.Node.Checked)
                {
                    e.Node.UncheckAll();
                }
                else
                {
                    e.Node.CheckAll();
                }

                var node = e.Node;
                while (node != null)
                {
                    //bool valid = false;
                    foreach (DevExpress.XtraTreeList.Nodes.TreeListNode item in node.Nodes)
                    {
                        item.CheckState = e.Node.CheckState;
                        //if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                        //{
                        //    node.CheckState = CheckState.Checked;
                        //}
                        //else
                        //{
                        //    node.CheckState = CheckState.Unchecked;
                        //}
                    }

                    //if (valid)
                    //{
                    //    node.CheckState = CheckState.Checked;
                    //}
                    //else
                    //{
                    //    node.CheckState = CheckState.Unchecked;
                    //}
                    node = node.ParentNode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (e.Node.HasChildren)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                    e.Appearance.BackColor = Color.Khaki;
                }
                else
                {
                    //e.Node.AllowSelect
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    SereServADO data = e.Row as SereServADO;
                    if (data != null)
                    {
                        if (!e.Node.HasChildren)
                        {
                            if (e.Column.FieldName == "AMOUNT_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.AMOUNT);
                            }
                            else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
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
                            else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.DISCOUNT ?? 0);
                            }
                            else if (e.Column.FieldName == "VAT_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.VAT);
                            }
                        }
                        else
                        {
                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_HEIN_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PATIENT_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
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

        private void GetTotalPriceOfChildChoice(SereServADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren)
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

        private void trvService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                if (e.Node.CheckState == CheckState.Indeterminate || e.Node.CheckState == CheckState.Checked)
                {
                    e.Node.CheckState = CheckState.Checked;
                }
                else
                {
                    e.Node.CheckState = CheckState.Unchecked;
                }

                if (e.Node.ParentNode != null)
                {
                    e.Node.ParentNode.CheckState = e.Node.CheckState;
                }

                ChangeEnableSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeEnableSave()
        {
            try
            {
                var listCheck = GetListCheck();
                if (listCheck == null || listCheck.Count <= 0)
                {
                    btnExportBill.Enabled = false;
                }
                else
                {
                    btnExportBill.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CurrentTreatment = null;
                resultTranBill = null;

                LoadSearch();
                FillInfoPatient(CurrentTreatment);

                ResetControlValue();
                LoadDataToTreeSereServ();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string FindTreatmentCode = txtTreatmentCode.Text;
                txtTreatmentCode.Text = "";
                btnSearch_Click(null, null);
                txtTreatmentCode.Text = FindTreatmentCode;
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
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
                if (resultTranBill != null)
                {
                    ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                    dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(resultTranBill.INVOICE_CODE);
                    dataInput.InvoiceCode = resultTranBill.INVOICE_CODE;
                    dataInput.NumOrder = resultTranBill.NUM_ORDER;
                    dataInput.SymbolCode = resultTranBill.SYMBOL_CODE;
                    dataInput.TemplateCode = resultTranBill.TEMPLATE_CODE;
                    dataInput.TransactionTime = resultTranBill.EINVOICE_TIME ?? resultTranBill.TRANSACTION_TIME;
                    dataInput.ENumOrder = resultTranBill.EINVOICE_NUM_ORDER;
                    dataInput.EinvoiceTypeId = resultTranBill.EINVOICE_TYPE_ID;
                    dataInput.Treatment = this.CurrentTreatment;
                    dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();

                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, resultTranBill);
                    dataInput.Transaction = tran;

                    dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                    ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                    if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                    {
                        if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                        {
                            MessageBox.Show("Tải hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                        }
                        return;
                    }

                    DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                    InputADO ado = new InputADO();
                    ado.DeleteWhenClose = true;
                    ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
                    ado.URL = electronicBillResult.InvoiceLink;
                    ViewType.Platform type = ViewType.Platform.Telerik;
                    if (HisConfigCFG.PlatformOption > 0)
                    {
                        type = (ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
                    }

                    viewManager.Run(ado, type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportBill.Enabled)
                    return;

                if (ListCurrentTransaction == null || ListCurrentTransaction.Count <= 0 || CurrentSereServs == null || CurrentSereServs.Count <= 0)
                {
                    XtraMessageBox.Show("Không có thông tin giao dịch", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                var listCheck = GetListCheck();
                if (listCheck == null || listCheck.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn giao dịch", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("check: " + listCheck.Count);
                }

                //lấy các node cha được check
                List<long> billIds = listCheck.Select(s => s.BILL_ID).Distinct().ToList();

                Inventec.Common.Logging.LogSystem.Info("billIds: " + billIds.Count);

                //lấy các giao dịch tương ứng.
                List<V_HIS_TRANSACTION> listTran = ListCurrentTransaction.Where(o => billIds.Contains(o.ID)).OrderByDescending(o => o.TRANSACTION_TIME).ThenBy(o => o.ID).ToList();

                //check hóa đơn viettel cần phải có cùng mẫu số, ký hiệu
                if (CheckTransaction(listTran))
                {
                    return;
                }

                //lấy chi tiết dịch vụ tương ứng với phiếu thanh toán.
                var sereServ5 = CurrentSereServs.Where(o => billIds.Contains(o.BILL_ID)).ToList();

                CommonParam param = new CommonParam();
                bool success = false;

                ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(listTran, sereServ5);
                if (electronicBillResult == null || !electronicBillResult.Success)
                {
                    param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        param.Messages.AddRange(electronicBillResult.Messages);
                    }

                    param.Messages = param.Messages.Distinct().ToList();
                }
                else
                {
                    //goi api update
                    resultTranBill = listTran.FirstOrDefault(o => o.EINVOICE_TYPE_ID.HasValue) ?? listTran.First();

                    CommonParam paramUpdate = new CommonParam();
                    HisTransactionInvoiceListInfoSDO sdo = new HisTransactionInvoiceListInfoSDO();
                    sdo.EinvoiceLoginname = electronicBillResult.InvoiceLoginname;
                    sdo.InvoiceCode = electronicBillResult.InvoiceCode;
                    sdo.InvoiceSys = electronicBillResult.InvoiceSys;
                    sdo.EinvoiceNumOrder = electronicBillResult.InvoiceNumOrder;
                    sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                    sdo.Ids = listTran.Select(s => s.ID).ToList();
                    sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                    var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceListInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                    if (apiResult)
                    {
                        resultTranBill.INVOICE_CODE = electronicBillResult.InvoiceCode;
                        resultTranBill.INVOICE_SYS = electronicBillResult.InvoiceSys;
                        resultTranBill.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                        resultTranBill.TRANSACTION_TIME = Inventec.Common.DateTime.Get.Now() ?? 0;
                        resultTranBill.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                        resultTranBill.EINVOICE_TIME = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                        resultTranBill.INVOICE_LOOKUP_CODE = electronicBillResult.InvoiceLookupCode;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramUpdate), paramUpdate));
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    }

                    success = true;
                    btnPrint.Enabled = true;
                    btnExportBill.Enabled = false;

                    if (!chkHideHddt.Checked)
                    {
                        System.Threading.Thread.Sleep(2000);
                        this.btnPrint_Click(null, null);
                    }
                }

                MessageManager.Show(this, param, success);

                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckTransaction(List<V_HIS_TRANSACTION> listTran)
        {
            try
            {
                List<long> eType = listTran.Where(o => o.EINVOICE_TYPE_ID.HasValue).Select(s => s.EINVOICE_TYPE_ID ?? 0).Distinct().ToList();
                if (eType != null && eType.Count > 1)
                {
                    XtraMessageBox.Show("Các giao dịch tạo trên hệ thống hóa đơn đt khác nhau. Vui lòng kiểm tra lại.", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return true;
                }

                var templateGroup = listTran.Where(o => !String.IsNullOrWhiteSpace(o.TEMPLATE_CODE) || !String.IsNullOrWhiteSpace(o.SYMBOL_CODE)).GroupBy(g => new { g.TEMPLATE_CODE, g.SYMBOL_CODE }).ToList();
                if (templateGroup.Count > 1)//có nhiều hơn 1 bộ mẫu số ký hiệu.
                {
                    List<string> accounts = new List<string>();
                    foreach (var item in templateGroup)
                    {
                        accounts.Add(string.Join(", ", item.Select(s => s.ACCOUNT_BOOK_NAME).Distinct()) + "(" + item.Key.SYMBOL_CODE + item.Key.TEMPLATE_CODE + ")");
                    }

                    string mess = "Các sổ {0} có thông tin mẫu số, ký hiệu khác nhau. Vui lòng chọn lại giao dịch";
                    XtraMessageBox.Show(string.Format(mess, string.Join(", ", accounts)), "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return true;
                }

                var listTranNoPatientAmount = listTran.Where(o => o.AMOUNT - (o.EXEMPTION ?? 0) - (o.TDL_BILL_FUND_AMOUNT ?? 0) <= 0).ToList();
                if (listTranNoPatientAmount != null && listTranNoPatientAmount.Count > 0)
                {
                    string mess = "Các giao dịch {0} không thu tiền bệnh nhân không xuất hóa đơn điên tử ";
                    XtraMessageBox.Show(string.Format(mess, string.Join(", ", listTranNoPatientAmount.Select(s => s.TRANSACTION_CODE).Distinct().ToList())), "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    foreach (TreeListNode node in trvService.Nodes)
                    {
                        SereServADO data = (SereServADO)trvService.GetDataRecordByNode(node);
                        if (listTranNoPatientAmount.Exists(o => o.ID == data.BILL_ID))
                        {
                            trvService.SetNodeCheckState(node, CheckState.Unchecked, true);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(List<V_HIS_TRANSACTION> transactions, List<SereServADO> sereServAdo)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (sereServAdo == null || transactions == null || transactions.Count <= 0)
                {
                    result.Success = false;
                    result.Messages.Add("Không xác định được dịch vụ thanh toán");
                    Inventec.Common.Logging.LogSystem.Error("Khong co dich vu thanh toan nao duoc chon! sereServBills is null");
                    return result;
                }

                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                foreach (var item in sereServAdo)
                {
                    V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServBill, item);
                    sereServBills.Add(sereServBill);
                }

                var tranFirst = transactions.Where(o => o.EINVOICE_TYPE_ID.HasValue).FirstOrDefault() ?? transactions.First();

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transactions.Sum(s => s.AMOUNT);
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = sereServBills.Sum(s => s.DISCOUNT);
                dataInput.DiscountRatio = Math.Round((dataInput.Discount ?? 0) / (dataInput.Amount ?? 0), 2, MidpointRounding.AwayFromZero) * 100;
                dataInput.PaymentMethod = tranFirst.PAY_FORM_NAME;
                dataInput.SereServs = sereServBills;
                dataInput.Treatment = this.CurrentTreatment;
                dataInput.Currency = "VND";

                if (transactions.Count == 1)//nếu chỉ có 1 giao dịch được chọn sẽ tạo như bt
                {
                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactions.First());
                    dataInput.Transaction = tran;
                    dataInput.EinvoiceTypeId = transactions.First().EINVOICE_TYPE_ID;
                }
                else
                    dataInput.ListTransaction = transactions;

                dataInput.SymbolCode = tranFirst.SYMBOL_CODE;
                dataInput.TemplateCode = tranFirst.TEMPLATE_CODE;
                dataInput.EinvoiceTypeId = tranFirst.EINVOICE_TYPE_ID;
                dataInput.TransactionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                List<string> transactionCode = transactions.Select(s => s.TRANSACTION_CODE).ToList();
                Inventec.Common.Logging.LogSystem.Info("transactionCode: " + string.Join(", ", transactionCode));

                WaitingManager.Show();
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<SereServADO> GetListCheck()
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                foreach (TreeListNode node in trvService.Nodes)
                {
                    if (node.Checked)
                    {
                        result.Add((SereServADO)trvService.GetDataRecordByNode(node));
                    }
                    //GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<SereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<SereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((SereServADO)trvService.GetDataRecordByNode(node));
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

        private void bBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnExportBill_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
