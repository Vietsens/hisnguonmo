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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PharmacyCashier.ADO;
using HIS.Desktop.Plugins.PharmacyCashier.Config;
using HIS.Desktop.Plugins.PharmacyCashier.Util;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PharmacyCashier
{
    public partial class frmPharmacyCashier : FormBase
    {
        private static long[] serviceTypeIdAllows = new long[11]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        private int positionHandleControl = -1;
        private V_HIS_MEDI_STOCK mediStock = null;

        private List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO> listAsignServiceAdo = new List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO>();

        private List<V_HIS_ACCOUNT_BOOK> listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        private List<V_HIS_CASHIER_ROOM> listCashierRoom = new List<V_HIS_CASHIER_ROOM>();

        private Dictionary<long, Dictionary<long, List<V_HIS_SERVICE_PATY>>> dicServicePaty = new Dictionary<long, Dictionary<long, List<V_HIS_SERVICE_PATY>>>();

        private List<MetyMatyInStockADO> listInStockADOs = new List<MetyMatyInStockADO>();
        private int theRequiredWidth = 900, theRequiredHeight = 130;
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        bool isShowContainerService = false;
        bool isShowContainerServiceForChoose = false;
        bool isShowService = true;

        bool isNotAllowPres = false;
        //bool isHoldPatientAndPresInfo = false;

        private const string THUOC = "TH";
        private const string VAT_TU = "VT";
        private string KEY_FORMAT = "{0}{1}";

        private MediMateADO currentMediMate = null;
        private SereServADO currentServiceAdo = null;
        Dictionary<string, MediMateADO> dicMediMate = new Dictionary<string, MediMateADO>();
        private string clientSessionKey = null;

        private HIS_SERVICE_REQ serviceReq = null;
        private List<HIS_SERVICE_REQ> serviceReqByTreatmentList = null;
        private HIS_TREATMENT treatment = null;
        private HIS_PATIENT patient = null;
        private PharmacyCashierResultSDO resultSdo = null;
        private HIS_TRANSACTION expMestBill = null;
        internal List<SereServADO> listSereServAdo = new List<SereServADO>();

        private bool enableSpinReceipt = true;
        private bool enableSpinInvoicePres = true;
        private bool enableSpinInvoiceService = true;

        private bool isInit = true;

        public frmPharmacyCashier(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            HisConfigCFG.LoadConfig();
        }

        private void frmPharmacyCashier_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadMediStockByModuleData();
                if (this.mediStock != null)
                {
                    this.InitControlState();
                    this.clientSessionKey = Guid.NewGuid().ToString();
                    this.ValidControl();
                    this.BuildInStockContainer();
                    this.LoadDataToComboUser();
                    this.LoadComboAge();
                    this.LoadDataToCboPatientType();
                    this.LoadDataToCboSerivcePatientType();
                    this.SetDefaultPatientType();
                    this.LoadDataToComboCashierRoom();
                    this.SetDefaultCashierRoom();
                    this.LoadDataToCboGender();
                    this.LoadDataToComboPayForm();
                    this.SetDefaultPayForm();
                    this.LoadDataToComboAccountBook();
                    this.SetDefaultAccountBook();
                    this.SetDefaultValueControlPatient();
                    this.SetEnableControlPatientDefault();
                    this.SetDefaultValueControlTransaction();
                    this.SetIntructionTime();
                    this.GenerateMenuPrint();
                    gridColSereServ_Select.Image = imageListIcon.Images[6];
                }
                this.isInit = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_PRINT_NOW)
                        {
                            checkIsPrintNow.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableControlPatientDefault()
        {
            try
            {
                if (this.mediStock != null)
                {
                    txtServiceReqCode.Enabled = true;
                    cboServicePatientType.Enabled = !checkIsVisitor.Checked;
                    txtAssignService.Enabled = !checkIsVisitor.Checked;
                    spinServiceAmount.Enabled = !checkIsVisitor.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlPres(bool isHoldInfo = false)
        {
            try
            {
                txtMetyMatyPres.Enabled = (!this.isNotAllowPres || isHoldInfo);
                spinAmount.Enabled = (!this.isNotAllowPres || isHoldInfo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableAssignServiceControl()
        {
            try
            {
                cboServicePatientType.Enabled = this.treatment != null;
                txtAssignService.Enabled = this.treatment != null;
                spinServiceAmount.Enabled = this.treatment != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlAssignService()
        {
            try
            {
                List<HIS_PATIENT_TYPE> sources = cboServicePatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                if (sources != null && sources.Count > 0)
                {
                    if (this.treatment != null && sources.Any(a => a.ID == this.treatment.TDL_PATIENT_TYPE_ID))
                    {
                        if (cboServicePatientType.EditValue == null || Convert.ToInt64(cboServicePatientType.EditValue) != this.treatment.TDL_PATIENT_TYPE_ID)
                            cboServicePatientType.EditValue = this.treatment.TDL_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        cboServicePatientType.EditValue = null;
                    }
                }
                txtAssignService.Text = "";
                spinServiceAmount.Value = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlPatient()
        {
            try
            {
                txtPatientAddress.Text = "";
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                txtPatientDob.Text = "";
                dtPatientDob.EditValue = null;
                cboPatientGender.EditValue = null;
                txtAge.Text = "";
                cboAge.EditValue = null;
                txtServiceReqCode.Text = "";
                txtTreatmentCode.Text = "";
                if (serviceReq != null)
                {
                    txtTreatmentCode.Text = serviceReq.TDL_TREATMENT_CODE;
                    txtServiceReqCode.Text = serviceReq.SERVICE_REQ_CODE;
                    txtPatientAddress.Text = serviceReq.TDL_PATIENT_ADDRESS ?? "";
                    txtPatientCode.Text = serviceReq.TDL_PATIENT_CODE ?? "";
                    txtPatientName.Text = serviceReq.TDL_PATIENT_NAME ?? "";
                    if (serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);
                    }
                    cboPatientGender.EditValue = serviceReq.TDL_PATIENT_GENDER_ID;
                }
                else if (this.serviceReqByTreatmentList != null && this.serviceReqByTreatmentList.Count > 0)
                {
                    var serviceReqTemp = this.serviceReqByTreatmentList.FirstOrDefault();
                    if (serviceReqTemp != null)
                    {
                        txtTreatmentCode.Text = serviceReqTemp.TDL_TREATMENT_CODE;
                        txtServiceReqCode.Text = "";
                        txtPatientAddress.Text = serviceReqTemp.TDL_PATIENT_ADDRESS ?? "";
                        txtPatientCode.Text = serviceReqTemp.TDL_PATIENT_CODE ?? "";
                        txtPatientName.Text = serviceReqTemp.TDL_PATIENT_NAME ?? "";
                        if (serviceReqTemp.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            txtPatientDob.Text = serviceReqTemp.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReqTemp.TDL_PATIENT_DOB);
                        }
                        cboPatientGender.EditValue = serviceReqTemp.TDL_PATIENT_GENDER_ID;
                    }
                }
                else if (patient != null)
                {
                    txtPatientAddress.Text = patient.VIR_ADDRESS ?? "";
                    txtPatientCode.Text = patient.PATIENT_CODE ?? "";
                    txtPatientName.Text = patient.VIR_PATIENT_NAME ?? "";
                    if (patient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = patient.DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                    }
                    cboPatientGender.EditValue = patient.GENDER_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlTransaction()
        {
            try
            {
                dtTransactionTime.DateTime = DateTime.Now;
                this.SetDefaultCashierRoom();
                this.SetDefaultPayForm();
                this.SetDefaultAccountBook();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIntructionTime()
        {
            try
            {
                if (this.serviceReq != null)
                {
                    dtIntructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.INTRUCTION_TIME).Value;
                }
                else
                {
                    dtIntructionTime.DateTime = DateTime.Now;
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
                decimal totalPricePres = 0;
                decimal totalPriceService = 0;
                if (dicMediMate.Count > 0)
                {
                    foreach (var value in dicMediMate.Values)
                    {
                        if (value.IsNotInStock) continue;
                        totalPricePres += (value.TOTAL_PRICE ?? 0);
                    }
                }
                spinTotalPricePres.Value = totalPricePres;
                if (this.listSereServAdo != null && this.listSereServAdo.Count > 0)
                {
                    totalPriceService = this.listSereServAdo.Sum(s => (s.VIR_TOTAL_PATIENT_PRICE ?? 0));
                }
                spinTotalPriceService.Value = totalPriceService;
                spinTotalPrice.Value = totalPricePres + totalPriceService;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetButtonTransaction()
        {
            try
            {
                if (resultSdo != null)
                {
                    btnCancelExpMest.Enabled = true;
                    btnLeavePres.Enabled = false;
                    btnLeaveService.Enabled = false;
                    btnPrintAndNew.Enabled = false;
                    btnPrintInvoicePres.Enabled = true;
                    ddBtnPrint.Enabled = true;
                }
                else
                {
                    btnCancelExpMest.Enabled = false;
                    btnLeavePres.Enabled = true;
                    btnLeaveService.Enabled = true;
                    btnPrintAndNew.Enabled = true;
                    btnPrintInvoicePres.Enabled = false;
                    ddBtnPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetAllControl()
        {
            try
            {
                this.clientSessionKey = Guid.NewGuid().ToString();
                serviceReq = null;
                this.serviceReqByTreatmentList = null;
                this.isNotAllowPres = false;
                patient = null;
                currentMediMate = null;
                currentServiceAdo = null;
                treatment = null;
                this.checkIsVisitor.Checked = true;
                listSereServAdo = new List<SereServADO>();
                if (this.resultSdo != null)
                {
                    this.SetRecentControl();
                    this.BuildInStockContainer();
                }
                this.ReleaseAllAndResetGrid();
                this.BuildServiceContainer();
                this.resultSdo = null;
                this.expMestBill = null;
                this.SetEnableControlPatientDefault();
                this.SetEnableControlPres();
                this.SetIntructionTime();
                this.SetDefaultValueControlPatient();
                this.SetDefaultValueControlAssignService();
                this.SetDefaultValueControlTransaction();
                this.ResetButtonTransaction();
                this.SetDataSourceGridMetyMaty();
                this.SetDataSourceTreeList();
                this.ResetControlValidError();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool isHold = false;
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        WaitingManager.Show();
                        string expMestCodes = "";
                        string code = String.Format("{0:000000000000}", Convert.ToInt64(txtServiceReqCode.Text.Trim()));
                        this.ResetAllControl();
                        txtServiceReqCode.Text = code;
                        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        serviceReqFilter.SERVICE_REQ_CODE__EXACT = code;
                        serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        serviceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null).FirstOrDefault();
                        if (serviceReq == null)
                        {
                            checkIsVisitor.Checked = true;
                            WaitingManager.Hide();
                            MessageBox.Show(String.Format("Không tìm thấy mã đơn: {0} ", txtServiceReqCode.Text));
                        }

                        if (serviceReq != null)
                        {
                            checkIsVisitor.Checked = false;
                            HisExpMestFilter expFilter = new HisExpMestFilter();
                            expFilter.PRESCRIPTION_ID = this.serviceReq.ID;
                            List<HIS_EXP_MEST> expMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expFilter, null);

                            if (expMests != null && expMests.Count > 0)
                            {
                                expMestCodes = String.Join(",", expMests.Select(s => s.EXP_MEST_CODE).ToList());
                                isNotAllowPres = true;
                            }

                            HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                            treatFilter.ID = serviceReq.TREATMENT_ID;
                            List<HIS_TREATMENT> listTreat = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, null);
                            this.treatment = listTreat != null ? listTreat.FirstOrDefault() : null;
                            if (this.isNotAllowPres)
                            {
                                WaitingManager.Hide();
                                if (XtraMessageBox.Show(String.Format("Đây là toa cũ (Đã được xuất). Bạn có muốn lấy thông tin?"), "Câu hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    isHold = true;
                                }
                                WaitingManager.Show();
                            }
                        }

                        this.TakeBeanByServiceReq(isHold);
                        this.LoadSereServ();
                        this.SetDefaultPatientType(isHold);
                        this.SetDefaultValueControlPatient();
                        this.SetTotalPrice();
                        this.SetEnableControlPres(isHold);
                        WaitingManager.Hide();
                        if (!this.isNotAllowPres || isHold)
                        {
                            txtMetyMatyPres.Focus();
                            txtMetyMatyPres.SelectAll();
                        }
                        if (isHold)
                        {
                            checkIsVisitor.Checked = true;
                            this.serviceReq = null;
                            this.treatment = null;
                            txtServiceReqCode.Text = "";
                            txtTreatmentCode.Text = "";
                        }
                    }
                    else
                    {
                        txtPatientName.Focus();
                        txtPatientName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool isHold = false;
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        WaitingManager.Show();
                        string expMestCodes = "";
                        string code = String.Format("{0:000000000000}", Convert.ToInt64(txtTreatmentCode.Text.Trim()));
                        this.ResetAllControl();
                        txtTreatmentCode.Text = code;
                        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        serviceReqFilter.TREATMENT_CODE__EXACT = code;
                        //serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        serviceReqByTreatmentList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null);
                        if (serviceReqByTreatmentList == null || serviceReqByTreatmentList.Count == 0)
                        {
                            checkIsVisitor.Checked = true;
                            WaitingManager.Hide();
                            MessageBox.Show(String.Format("Không tìm thấy mã đơn tương ứng HSDT: {0} ", txtTreatmentCode.Text));
                        }

                        if (serviceReqByTreatmentList != null && serviceReqByTreatmentList.Count > 0)
                        {
                            checkIsVisitor.Checked = false;
                            HisExpMestFilter expFilter = new HisExpMestFilter();
                            expFilter.PRESCRIPTION_IDs = this.serviceReqByTreatmentList.Select(o => o.ID).Distinct().ToList();
                            List<HIS_EXP_MEST> expMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expFilter, null);

                            if (expMests != null && expMests.Count > 0)
                            {
                                expMestCodes = String.Join(",", expMests.Select(s => s.EXP_MEST_CODE).ToList());
                                isNotAllowPres = true;
                            }

                            HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                            treatFilter.ID = serviceReqByTreatmentList.FirstOrDefault().TREATMENT_ID;
                            List<HIS_TREATMENT> listTreat = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, null);
                            this.treatment = listTreat != null ? listTreat.FirstOrDefault() : null;
                            //if (this.isNotAllowPres)
                            //{
                            //    WaitingManager.Hide();
                            //    if (XtraMessageBox.Show(String.Format("Đây là toa cũ (Đã được xuất). Bạn có muốn lấy thông tin?"), "Câu hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == System.Windows.Forms.DialogResult.Yes)
                            //    {
                            //        isHold = true;
                            //    }
                            //    WaitingManager.Show();
                            //}
                        }

                        this.TakeBeanByServiceReq(isHold);
                        this.LoadSereServ();
                        this.SetDefaultPatientType(isHold);
                        this.SetDefaultValueControlPatient();
                        this.SetTotalPrice();
                        this.SetEnableControlPres(isHold);
                        WaitingManager.Hide();
                        if (!this.isNotAllowPres || isHold)
                        {
                            txtMetyMatyPres.Focus();
                            txtMetyMatyPres.SelectAll();
                        }
                        if (isHold)
                        {
                            checkIsVisitor.Checked = true;
                            this.serviceReqByTreatmentList = null;
                            txtServiceReqCode.Text = "";
                            txtTreatmentCode.Text = "";
                        }
                    }
                    else
                    {
                        txtPatientName.Focus();
                        txtPatientName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsVisitor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //this.ResetAllControl(this.isHoldPatientAndPresInfo);
                //this.isHoldPatientAndPresInfo = false;
                this.SetEnableControlPatientDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboPresUser.Focus();
                    cboPresUser.SelectAll();
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
                    cboPresUser.Focus();
                    cboPresUser.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPresLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboPresUser.Focus();
                        cboPresUser.SelectAll();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboPresUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtPresLoginname.Text = searchResult[0].LOGINNAME;
                            this.txtMetyMatyPres.Focus();
                            this.txtMetyMatyPres.SelectAll();
                        }
                        else
                        {
                            cboPresUser.Focus();
                            cboPresUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPresUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (String.IsNullOrWhiteSpace(txtPatientName.Text))
                    {
                        txtPatientName.Focus();
                        txtPatientName.SelectAll();
                    }
                    else
                    {
                        txtMetyMatyPres.Focus();
                        txtMetyMatyPres.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPresUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPresLoginname.Text = "";
                if (cboPresUser.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboPresUser.EditValue.ToString());
                    if (user != null)
                    {
                        txtPresLoginname.Text = user.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPresUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (checkIsVisitor.Checked && String.IsNullOrWhiteSpace(txtPatientName.Text))
                    {
                        txtPatientName.Focus();
                        txtPatientName.SelectAll();
                    }
                    else
                    {
                        txtMetyMatyPres.Focus();
                        txtMetyMatyPres.SelectAll();
                    }
                }
                else
                {
                    cboPresUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                    {
                        string code = String.Format("{0:0000000000}", Convert.ToInt64(txtPatientCode.Text.Trim()));
                        this.ResetAllControl();
                        txtPatientCode.Text = code;
                        HisPatientFilter filter = new HisPatientFilter();
                        filter.PATIENT_CODE__EXACT = code;
                        List<HIS_PATIENT> pts = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                        patient = pts != null ? pts.FirstOrDefault() : null;
                        if (patient == null)
                        {
                            MessageBox.Show(String.Format("Không tìm thấy bệnh nhân mã: {0} ", code));
                            return;
                        }
                        this.SetDefaultValueControlPatient();
                        txtMetyMatyPres.Focus();
                        txtMetyMatyPres.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPresUser.Focus();
                    cboPresUser.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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
                        this.cboPatientGender.Focus();
                        this.cboPatientGender.ShowPopup();
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
                        this.cboPatientGender.Focus();
                        this.cboPatientGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!checkIsVisitor.Checked)
                {
                    e.Handled = true;
                }
                else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
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
                    cboPatientGender.Focus();
                    cboPatientGender.ShowPopup();
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

        private void cboPatientGender_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPatientAddress.Focus();
                    txtPatientAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientAddress.Focus();
                    txtPatientAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMetyMatyPres.Focus();
                    txtMetyMatyPres.SelectAll();
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboCashierRoom.Focus();
                    if (cboCashierRoom.EditValue == null)
                    {
                        cboCashierRoom.ShowPopup();
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
                    cboCashierRoom.Focus();
                    if (cboCashierRoom.EditValue == null)
                    {
                        cboCashierRoom.ShowPopup();
                    }
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtReceiptAccountBookCode.Focus();
                    txtReceiptAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtReceiptAccountBookCode.Focus();
                    txtReceiptAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReceiptAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtReceiptAccountBookCode.Text))
                    {
                        string key = txtReceiptAccountBookCode.Text.ToLower().Trim();
                        var listData = this.listAccountBook.Where(o => o.BILL_TYPE_ID == 1 && o.ACCOUNT_BOOK_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboReceiptAccountBook.EditValue = listData[0].ID;
                        }
                        else
                        {
                            cboReceiptAccountBook.Focus();
                            cboReceiptAccountBook.ShowPopup();
                        }
                    }
                    else
                    {
                        cboReceiptAccountBook.Focus();
                        if (cboReceiptAccountBook.EditValue == null)
                        {
                            cboReceiptAccountBook.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceiptAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (spinReceiptNumOrder.Enabled)
                    {
                        spinReceiptNumOrder.Focus();
                        spinReceiptNumOrder.SelectAll();
                    }
                    else
                    {
                        txtInvoicePresAccountBookCode.Focus();
                        txtInvoicePresAccountBookCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceiptAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtReceiptAccountBookCode.Text = "";
                if (cboReceiptAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK accBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboReceiptAccountBook.EditValue));
                    if (accBook != null)
                    {
                        txtReceiptAccountBookCode.Text = accBook.ACCOUNT_BOOK_CODE;
                        SetDataToDicNumOrderInAccountBook(accBook, spinReceiptNumOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceiptAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinReceiptNumOrder.Enabled)
                    {
                        spinReceiptNumOrder.Focus();
                        spinReceiptNumOrder.SelectAll();
                    }
                    else
                    {
                        txtInvoicePresAccountBookCode.Focus();
                        txtInvoicePresAccountBookCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinReceiptNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboReceiptAccountBook.EditValue != null)
                    {
                        var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboReceiptAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook.ID, spinReceiptNumOrder);
                    }
                    txtInvoicePresAccountBookCode.Focus();
                    txtInvoicePresAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInvoicePresAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtInvoicePresAccountBookCode.Text))
                    {
                        string key = txtInvoicePresAccountBookCode.Text.ToLower().Trim();
                        var listData = this.listAccountBook.Where(o => o.BILL_TYPE_ID == 2 && o.ACCOUNT_BOOK_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboInvoicePresAccountBook.EditValue = listData[0].ID;
                        }
                        else
                        {
                            cboInvoicePresAccountBook.Focus();
                            cboInvoicePresAccountBook.ShowPopup();
                        }
                    }
                    else
                    {
                        cboInvoicePresAccountBook.Focus();
                        if (cboInvoicePresAccountBook.EditValue == null)
                        {
                            cboInvoicePresAccountBook.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoicePresAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (spinInvoicePresNumOrder.Enabled)
                    {
                        spinInvoicePresNumOrder.Focus();
                        spinInvoicePresNumOrder.SelectAll();
                    }
                    else
                    {
                        txtInvoiceServiceAccountBookCode.Focus();
                        txtInvoiceServiceAccountBookCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoicePresAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtInvoicePresAccountBookCode.Text = "";
                if (cboInvoicePresAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK accBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoicePresAccountBook.EditValue));
                    if (accBook != null)
                    {
                        txtInvoicePresAccountBookCode.Text = accBook.ACCOUNT_BOOK_CODE;
                        SetDataToDicNumOrderInAccountBook(accBook, spinInvoicePresNumOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoicePresAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinInvoicePresNumOrder.Enabled)
                    {
                        spinInvoicePresNumOrder.Focus();
                        spinInvoicePresNumOrder.SelectAll();
                    }
                    else
                    {
                        txtInvoiceServiceAccountBookCode.Focus();
                        txtInvoiceServiceAccountBookCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoicePresNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboInvoicePresAccountBook.EditValue != null)
                    {
                        var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoicePresAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook.ID, spinInvoicePresNumOrder);
                    }
                    txtInvoiceServiceAccountBookCode.Focus();
                    txtInvoiceServiceAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInvoiceServiceAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtInvoiceServiceAccountBookCode.Text))
                    {
                        string key = txtInvoiceServiceAccountBookCode.Text.ToLower().Trim();
                        var listData = this.listAccountBook.Where(o => o.BILL_TYPE_ID == 2 && o.ACCOUNT_BOOK_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboInvoiceServiceAccountBook.EditValue = listData[0].ID;
                        }
                        else
                        {
                            cboInvoiceServiceAccountBook.Focus();
                            cboInvoiceServiceAccountBook.ShowPopup();
                        }
                    }
                    else
                    {
                        cboInvoiceServiceAccountBook.Focus();
                        if (cboInvoiceServiceAccountBook.EditValue == null)
                        {
                            cboInvoiceServiceAccountBook.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceServiceAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (spinInvoiceServiceNumOrder.Enabled)
                    {
                        spinInvoiceServiceNumOrder.Focus();
                        spinInvoiceServiceNumOrder.SelectAll();
                    }
                    else
                    {
                        txtPayFormCode.Focus();
                        txtPayFormCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceServiceAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtInvoiceServiceAccountBookCode.Text = "";
                if (cboInvoiceServiceAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK accBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceServiceAccountBook.EditValue));
                    if (accBook != null)
                    {
                        txtInvoiceServiceAccountBookCode.Text = accBook.ACCOUNT_BOOK_CODE;
                        SetDataToDicNumOrderInAccountBook(accBook, spinInvoiceServiceNumOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceServiceAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinInvoiceServiceNumOrder.Enabled)
                    {
                        spinInvoiceServiceNumOrder.Focus();
                        spinInvoiceServiceNumOrder.SelectAll();
                    }
                    else
                    {
                        txtPayFormCode.Focus();
                        txtPayFormCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoiceServiceNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboInvoiceServiceAccountBook.EditValue != null)
                    {
                        var accountBook = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceServiceAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook.ID, spinInvoiceServiceNumOrder);
                    }
                    txtPayFormCode.Focus();
                    txtPayFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtPayFormCode.Text))
                    {
                        string key = txtPayFormCode.Text.ToLower().Trim();
                        var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboPayForm.EditValue = listData[0].ID;
                        }
                        else
                        {
                            cboPayForm.Focus();
                            cboPayForm.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPayForm.Focus();
                        if (cboPayForm.EditValue == null)
                        {
                            cboPayForm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPayFormCode.Text = "";
                if (cboPayForm.EditValue != null)
                {
                    HIS_PAY_FORM accBook = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                    if (accBook != null)
                    {
                        txtPayFormCode.Text = accBook.PAY_FORM_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtMetyMatyPres_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(txtMetyMatyPres.Bounds.X, txtMetyMatyPres.Bounds.Y + layoutControlItem4.Height, txtMetyMatyPres.Bounds.Width, txtMetyMatyPres.Bounds.Height);
                        popupControlContainerInStock.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
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

        private void txtMetyMatyPres_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var selectADO = (MetyMatyInStockADO)this.gridViewInStockMetyMaty.GetFocusedRow();
                    if (selectADO != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerInStock.HidePopup();
                        MetyMatyTypeInStock_RowClick(selectADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewInStockMetyMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMetyMatyPres.Bounds.X, txtMetyMatyPres.Bounds.Y + layoutControlItem4.Height, txtMetyMatyPres.Bounds.Width, txtMetyMatyPres.Bounds.Height);
                    popupControlContainerInStock.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));

                    gridViewInStockMetyMaty.Focus();
                    gridViewInStockMetyMaty.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMetyMatyPres.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMetyMatyPres_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMetyMatyPres.Text))
                {
                    txtMetyMatyPres.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewInStockMetyMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewInStockMetyMaty.ActiveFilterString = String.Format("[MedicineTypeCode] Like '%{0}%' OR [MedicineTypeCodeUnsign] Like '%{0}%'", txtMetyMatyPres.Text);
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewInStockMetyMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewInStockMetyMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewInStockMetyMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewInStockMetyMaty.FocusedRowHandle = 0;
                        gridViewInStockMetyMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewInStockMetyMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtMetyMatyPres.Bounds.X, txtMetyMatyPres.Bounds.Y + layoutControlItem4.Height, txtMetyMatyPres.Bounds.Width, txtMetyMatyPres.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerInStock.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                            isShow = false;
                        }

                        txtMetyMatyPres.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewInStockMetyMaty.ActiveFilter.Clear();
                    this.currentMediMate = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerInStock.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinAmount.Value > 0 && this.currentMediMate == null)
                    {
                        XtraMessageBox.Show("Bạn chưa chọn thuốc - vật tư", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    if (spinAmount.Value <= 0 && this.currentMediMate != null)
                    {
                        XtraMessageBox.Show("Số lượng phải > 0", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    WaitingManager.Show();
                    if (!TakeBeanProccess(this.currentMediMate.BeanIds, spinAmount.Value, this.currentMediMate))
                    {
                        return;
                    }
                    string key = "";
                    if (currentMediMate.IsMedicine)
                    {
                        key = String.Format(KEY_FORMAT, THUOC, currentMediMate.MEDI_MATE_TYPE_ID);
                    }
                    else if (currentMediMate.IsMaterial)
                    {
                        key = String.Format(KEY_FORMAT, VAT_TU, currentMediMate.MEDI_MATE_TYPE_ID);
                    }
                    dicMediMate[key] = this.currentMediMate;
                    SetDataSourceGridMetyMaty();
                    ResetCurrentMatyMety();
                    this.SetTotalPrice();
                    txtMetyMatyPres.Focus();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMetyMatys_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EXP_AMOUNT")
                {
                    MediMateADO rowAdo = (MediMateADO)gridViewMetyMatys.GetFocusedRow();
                    if (rowAdo != null)
                    {
                        if (rowAdo.EXP_AMOUNT <= 0)
                        {
                            XtraMessageBox.Show("Số lượng phải > 0", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                            rowAdo.EXP_AMOUNT = rowAdo.OLD_AMOUNT;
                            gridControlMetyMatys.RefreshDataSource();
                            return;
                        }
                        WaitingManager.Show();
                        if (!this.TakeBeanProccess(rowAdo.BeanIds, rowAdo.EXP_AMOUNT, rowAdo))
                        {
                            rowAdo.EXP_AMOUNT = rowAdo.OLD_AMOUNT;
                            gridControlMetyMatys.RefreshDataSource();
                            return;
                        }
                        gridControlMetyMatys.RefreshDataSource();
                        this.SetTotalPrice();
                        WaitingManager.Hide();
                    }
                }
                else if (e.Column.FieldName == "IsCheck")
                {
                    this.SetTotalPrice();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMetyMatys_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName != "EXP_AMOUNT")
                {
                    return;
                }
                var index = this.gridViewMetyMatys.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridViewMetyMatys.DataSource as List<MediMateADO>;
                var row = listDatas[index];

                if (row.IsNotInStock || row.IsExceedsAvailable)
                {
                    e.Info.ErrorType = ErrorType.Warning;
                    e.Info.ErrorText = "Không đủ khả dụng";
                }
                else
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMetyMatys_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridViewSereServ_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                var data = (SereServADO)gridViewSereServ.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IsAdd)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
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

        private void gridViewSereServ_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServADO data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                        {
                            e.Value = NumberUtil.ConvertNumberToString(data.VIR_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = NumberUtil.ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                //GridHitInfo hi = view.CalcHitInfo(e.Location);
                //if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                //{
                //    if (hi.InRowCell)
                //    {
                //        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                //        {
                //            view.FocusedRowHandle = hi.RowHandle;
                //            view.FocusedColumn = hi.Column;
                //            view.ShowEditor();
                //            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                //            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                //            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                //            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                //            Rectangle gridGlyphRect =
                //                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                //                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                //                 glyphRect.Width,
                //                 glyphRect.Height);
                //            if (!gridGlyphRect.Contains(e.Location))
                //            {
                //                view.CloseEditor();
                //                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                //                {
                //                    view.SelectCell(hi.RowHandle, hi.Column);
                //                }
                //                else
                //                {
                //                    view.UnselectCell(hi.RowHandle, hi.Column);
                //                }
                //            }
                //            else
                //            {
                //                var dataRow = (SereServADO)gridViewSereServ.GetRow(hi.RowHandle);
                //                if (dataRow != null)
                //                {
                //                    checkEdit.Checked = !checkEdit.Checked;
                //                    view.CloseEditor();
                //                    if (this.listSereServAdo != null && this.listSereServAdo.Count > 0)
                //                    {
                //                        var dataChecks = this.listSereServAdo.Where(p => p.IsCheck == true).ToList();
                //                        isCheckAll = false;
                //                        if (dataChecks != null && dataChecks.Count > 0)
                //                        {
                //                            gridColSereServ_Select.Image = imageListIcon.Images[5];
                //                        }
                //                        else
                //                        {
                //                            gridColSereServ_Select.Image = imageListIcon.Images[6];
                //                        }
                //                    }
                //                    this.SetTotalPrice();
                //                }
                //            }
                //            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                //        }
                //    }
                //    if (hi.HitTest == GridHitTest.Column)
                //    {
                //        if (hi.Column.FieldName == "IsCheck")
                //        {
                //            gridColSereServ_Select.Image = imageListIcon.Images[5];
                //            gridViewSereServ.BeginUpdate();
                //            if (this.listSereServAdo == null)
                //                this.listSereServAdo = new List<SereServADO>();
                //            if (isCheckAll)
                //            {
                //                foreach (var item in this.listSereServAdo)
                //                {
                //                    item.IsCheck = true;
                //                }
                //                isCheckAll = false;
                //            }
                //            else
                //            {
                //                gridColSereServ_Select.Image = imageListIcon.Images[6];
                //                foreach (var item in this.listSereServAdo)
                //                {
                //                    item.IsCheck = false;
                //                }
                //                isCheckAll = true;
                //            }
                //            gridViewSereServ.EndUpdate();
                //            this.SetTotalPrice();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLeavePres_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLeavePres.Enabled) return;
                ReleaseAllAndResetGrid();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLeaveService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLeaveService.Enabled) return;
                this.listSereServAdo = new List<SereServADO>();
                gridColSereServ_Select.Image = imageListIcon.Images[6];
                this.SetDataSourceTreeList();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancelExpMest.Enabled) return;
                List<object> listData = new List<object>();
                ExpMestSaleTranADO ado = new ExpMestSaleTranADO();
                if (this.resultSdo != null && this.expMestBill != null)
                {
                    ado.NumOrder = this.expMestBill.NUM_ORDER;
                    listData.Add(ado);
                }
                else
                    if (this.resultSdo != null && this.resultSdo.ExpMest != null)
                    {
                        ado.ExpMestCode = this.resultSdo.ExpMest.EXP_MEST_CODE;
                        listData.Add(ado);
                    }
                    else if (this.serviceReq != null)
                    {
                        ado.TreatmentCode = this.serviceReq.TDL_TREATMENT_CODE;
                        listData.Add(ado);
                    }
                    else if (this.serviceReqByTreatmentList != null && this.serviceReqByTreatmentList.Count > 0)
                    {
                        ado.TreatmentCode = this.serviceReqByTreatmentList.FirstOrDefault().TDL_TREATMENT_CODE;
                        listData.Add(ado);
                    }
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ExpMestSaleTransactionList", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintInvoicePres_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnPrintInvoicePres.Enabled || this.resultSdo == null || this.resultSdo.ExpMest == null) return;
                if (this.expMestBill == null)
                {
                    if (!dxValidationProviderPrintInvoice.Validate())
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    PharmacyCashierExpInvoiceSDO sdo = new PharmacyCashierExpInvoiceSDO();
                    sdo.CashierRoomId = Convert.ToInt64(cboCashierRoom.EditValue);
                    sdo.InvoiceAccountBookId = Convert.ToInt64(cboInvoicePresAccountBook.EditValue);
                    if (this.enableSpinInvoicePres)
                    {
                        sdo.InvoiceNumOrder = (long)spinInvoicePresNumOrder.Value;
                    }
                    sdo.ExpMestId = this.resultSdo.ExpMest.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;
                    sdo.PayFormId = Convert.ToInt64(cboPayForm.EditValue);
                    HIS_TRANSACTION rs = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisExpMest/PharmacyCashierExpInvoice", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        this.expMestBill = rs;
                        success = true;
                        UpdateDictionaryNumOrderAccountBook(this.expMestBill.ACCOUNT_BOOK_ID, spinInvoicePresNumOrder);
                    }
                    WaitingManager.Hide();
                    if (!success)
                    {
                        MessageManager.Show(this, param, success);
                    }
                    else
                    {
                        this.PrintHoaDonBanThuocMps344();
                    }
                }
                else
                {
                    this.PrintHoaDonBanThuocMps344();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExpMestList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExpMestList.Enabled) return;
                List<object> listData = new List<object>();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ExpMestSaleTransactionList", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintAndNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetTotalPrice();
                positionHandleControl = -1;
                if (!btnPrintAndNew.Enabled || !dxValidationProviderSave.Validate()) return;
                var listErrors = dicMediMate.Count > 0 ? dicMediMate.Select(s => s.Value).Where(o => o.IsNotInStock || o.IsExceedsAvailable).ToList() : null;
                if (listErrors != null && listErrors.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư sau không đủ khả dụng: {0}", String.Join(",", listErrors.Select(s => s.MEDI_MATE_TYPE_NAME).ToList()));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                PharmacyCashierSDO sdo = new PharmacyCashierSDO();
                if (cboReceiptAccountBook.EditValue != null)
                {
                    sdo.RecieptAccountBookId = Convert.ToInt64(cboReceiptAccountBook.EditValue);
                    if (this.enableSpinReceipt)
                    {
                        sdo.RecieptNumOrder = (long)spinReceiptNumOrder.Value;
                    }
                }
                sdo.CashierRoomId = Convert.ToInt64(cboCashierRoom.EditValue);
                sdo.ClientSessionKey = this.clientSessionKey;
                if (cboInvoiceServiceAccountBook.EditValue != null)
                {
                    sdo.InvoiceAccountBookId = Convert.ToInt64(cboInvoiceServiceAccountBook.EditValue);
                    if (this.enableSpinInvoiceService)
                    {
                        sdo.InvoiceNumOrder = (long)spinInvoiceServiceNumOrder.Value;
                    }
                }
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null && !String.IsNullOrWhiteSpace(dateValidObject.OutDate))
                {
                    this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                    this.dtPatientDob.Update();
                    sdo.PatientDob = Convert.ToInt64(this.dtPatientDob.DateTime.ToString("yyyyMMdd") + "000000");
                    if (dateValidObject.HasNotDayDob)
                    {
                        sdo.IsHasNotDayDob = true;
                    }
                }
                if (cboPatientType != null)
                    sdo.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                sdo.PatientAddress = txtPatientAddress.Text;
                if (cboPatientGender.EditValue != null)
                    sdo.PatientGenderId = Convert.ToInt64(cboPatientGender.EditValue);
                sdo.PatientName = txtPatientName.Text;
                sdo.PayFormId = Convert.ToInt64(cboPayForm.EditValue);
                if (this.serviceReq != null)
                    sdo.PrescriptionId = serviceReq.ID;

                if (cboPresUser.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboPresUser.EditValue.ToString());
                    if (user != null)
                    {
                        sdo.PrescriptionReqLoginname = user.LOGINNAME;
                        sdo.PrescriptionReqUsername = user.USERNAME;
                    }
                }
                sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                if (dicMediMate.Count > 0)
                {
                    List<long> materialBeanIds = new List<long>();
                    List<long> medicineBeanIds = new List<long>();
                    List<ExpMaterialTypeSDO> materials = new List<ExpMaterialTypeSDO>();
                    List<ExpMedicineTypeSDO> medicines = new List<ExpMedicineTypeSDO>();
                    foreach (var vl in dicMediMate.Values)
                    {
                        if (vl.IsMaterial)
                        {
                            if (vl.BeanIds != null)
                                materialBeanIds.AddRange(vl.BeanIds);
                            ExpMaterialTypeSDO mate = new ExpMaterialTypeSDO();
                            mate.Amount = vl.EXP_AMOUNT;
                            mate.MaterialTypeId = vl.MEDI_MATE_TYPE_ID;
                            mate.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                            materials.Add(mate);
                        }
                        else if (vl.IsMedicine)
                        {
                            if (vl.BeanIds != null)
                                medicineBeanIds.AddRange(vl.BeanIds);
                            ExpMedicineTypeSDO mate = new ExpMedicineTypeSDO();
                            mate.Amount = vl.EXP_AMOUNT;
                            mate.MedicineTypeId = vl.MEDI_MATE_TYPE_ID;
                            mate.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                            medicines.Add(mate);
                        }
                    }
                    if (materials.Count > 0)
                    {
                        sdo.MaterialBeanIds = materialBeanIds;
                        sdo.Materials = materials;
                    }
                    if (medicines.Count > 0)
                    {
                        sdo.MedicineBeanIds = medicineBeanIds;
                        sdo.Medicines = medicines;
                    }
                }

                if (this.listSereServAdo != null && this.listSereServAdo.Count > 0)
                {
                    List<SereServTranSDO> invoiceSereServs = null;
                    List<AssignServiceExtSDO> invoiceAssigns = null;
                    List<SereServTranSDO> recieptSereServs = null;
                    List<AssignServiceExtSDO> recieptAssigns = null;
                    foreach (var item in this.listSereServAdo)
                    {
                        if (!item.IsAdd)
                        {
                            SereServTranSDO ss = new SereServTranSDO();
                            ss.Price = item.VIR_TOTAL_PATIENT_PRICE ?? 0;
                            ss.SereServId = item.ID;
                            if (item.IsInvoiced)
                            {
                                if (invoiceSereServs == null) invoiceSereServs = new List<SereServTranSDO>();
                                invoiceSereServs.Add(ss);
                            }
                            else
                            {
                                if (recieptSereServs == null) recieptSereServs = new List<SereServTranSDO>();
                                recieptSereServs.Add(ss);
                            }
                        }
                        else
                        {
                            AssignServiceExtSDO ass = new AssignServiceExtSDO();
                            ass.Amount = item.AMOUNT_PLUS ?? 0;
                            ass.IntructionTime = item.TDL_INTRUCTION_TIME;
                            ass.PatientTypeId = item.PATIENT_TYPE_ID;
                            ass.Price = item.PRICE;
                            ass.VatRatio = item.VAT_RATIO;
                            ass.ServiceId = item.SERVICE_ID;
                            if (item.IsInvoiced)
                            {
                                if (invoiceAssigns == null) invoiceAssigns = new List<AssignServiceExtSDO>();
                                invoiceAssigns.Add(ass);
                            }
                            else
                            {
                                if (recieptAssigns == null) recieptAssigns = new List<AssignServiceExtSDO>();
                                recieptAssigns.Add(ass);
                            }
                        }
                    }

                    sdo.InvoiceSereServs = invoiceSereServs;
                    sdo.InvoiceAssignServices = invoiceAssigns;
                    sdo.RecieptSereServs = recieptSereServs;
                    sdo.RecieptAssignServices = recieptAssigns;
                }

                LogSystem.Info(LogUtil.TraceData("InputSDO", sdo));
                var rs = new BackendAdapter(param).Post<PharmacyCashierResultSDO>("api/HisExpMest/PharmacyCashierPay", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    this.resultSdo = rs;
                    this.ResetButtonTransaction();
                    if (this.resultSdo.ServiceInvoices != null && this.resultSdo.ServiceInvoices.Count > 0)
                    {
                        UpdateDictionaryNumOrderAccountBook(this.resultSdo.ServiceInvoices.FirstOrDefault().ACCOUNT_BOOK_ID, this.resultSdo.ServiceInvoices.LastOrDefault().NUM_ORDER);
                    }

                    if (this.resultSdo.ServiceReciepts != null && this.resultSdo.ServiceReciepts.Count > 0)
                    {
                        UpdateDictionaryNumOrderAccountBook(this.resultSdo.ServiceReciepts.FirstOrDefault().ACCOUNT_BOOK_ID, this.resultSdo.ServiceReciepts.LastOrDefault().NUM_ORDER);
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                if (success)
                {
                    this.PrintPhieuXuatBanMps092();
                    this.PrintHoaDonDichVuMps106();
                    this.PrintBienLaiDichVuMps343();
                    this.btnNew_Click(null, null);
                }
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
                WaitingManager.Show();
                this.ResetAllControl();
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderSave_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void dxValidationProviderPrintInvoice_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void gridViewInStockMetyMaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridViewInStockMetyMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MetyMatyInStockADO selectedADO = (MetyMatyInStockADO)this.gridViewInStockMetyMaty.GetFocusedRow();
                    if (selectedADO != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerInStock.HidePopup();
                        MetyMatyTypeInStock_RowClick(selectedADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewInStockMetyMaty.Focus();
                    this.gridViewInStockMetyMaty.FocusedRowHandle = this.gridViewInStockMetyMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewInStockMetyMaty_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                MetyMatyInStockADO selectedADO = (MetyMatyInStockADO)this.gridViewInStockMetyMaty.GetFocusedRow();
                if (selectedADO != null)
                {
                    popupControlContainerInStock.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    MetyMatyTypeInStock_RowClick(selectedADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerInStock_CloseUp(object sender, EventArgs e)
        {
            try
            {
                this.isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                MediMateADO rowData = (MediMateADO)gridViewMetyMatys.GetFocusedRow();
                if (rowData != null)
                {
                    string key = "";
                    if (rowData.IsMedicine)
                    {
                        key = String.Format(KEY_FORMAT, THUOC, rowData.MEDI_MATE_TYPE_ID);
                    }
                    else if (rowData.IsMaterial)
                    {
                        key = String.Format(KEY_FORMAT, VAT_TU, rowData.MEDI_MATE_TYPE_ID);
                    }
                    if (dicMediMate.ContainsKey(key))
                    {
                        dicMediMate.Remove(key);
                        this.SetDataSourceGridMetyMaty();
                        this.SetTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnLeavePres_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnLeavePres_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnLeaveService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnLeaveService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrintAndNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrintAndNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void frmPharmacyCashier_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.ReleaseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFocusServiceReqCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFocusChoicePres_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtMetyMatyPres.Focus();
                txtMetyMatyPres.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTotalPricePres_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTotalPriceService_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTotalPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinReceiptNumOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!this.enableSpinReceipt)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoicePresNumOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!this.enableSpinInvoicePres)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoiceServiceNumOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!this.enableSpinInvoiceService)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientInfo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!checkIsVisitor.Checked)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AgeInfo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
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
                //e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServicePatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtAssignService.Focus();
                    txtAssignService.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServicePatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAssignService.Focus();
                    txtAssignService.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServicePatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.BuildServiceContainer();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAssignService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerService = !isShowContainerService;
                    if (isShowContainerService)
                    {
                        Rectangle buttonBounds = new Rectangle(txtAssignService.Bounds.X, txtAssignService.Bounds.Y + layoutControlItem4.Height, txtAssignService.Bounds.Width, txtAssignService.Bounds.Height);
                        popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
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

        private void txtAssignService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var selectADO = (HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO)this.gridViewAssignService.GetFocusedRow();
                    if (selectADO != null)
                    {
                        isShowContainerService = false;
                        isShowContainerServiceForChoose = true;
                        popupControlContainerService.HidePopup();
                        AssignService_RowClick(selectADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewAssignService.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtAssignService.Bounds.X, txtAssignService.Bounds.Y + layoutControlItem4.Height, txtAssignService.Bounds.Width, txtAssignService.Bounds.Height);
                    popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));

                    gridViewAssignService.Focus();
                    gridViewAssignService.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtAssignService.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAssignService_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtAssignService.Text))
                {
                    txtAssignService.Refresh();
                    if (isShowContainerServiceForChoose)
                    {
                        gridViewAssignService.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerService)
                        {
                            isShowContainerService = true;
                        }

                        //Filter data
                        gridViewAssignService.ActiveFilterString = String.Format("[SERVICE_CODE] Like '%{0}%' OR [SERVICE_CODE_FOR_SEARCH] Like '%{0}%'", txtAssignService.Text);

                        gridViewAssignService.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewAssignService.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewAssignService.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewAssignService.FocusedRowHandle = 0;
                        gridViewAssignService.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewAssignService.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtAssignService.Bounds.X, txtAssignService.Bounds.Y + layoutControlItem4.Height, txtAssignService.Bounds.Width, txtAssignService.Bounds.Height);
                        if (isShowService)
                        {
                            popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                            isShowService = false;
                        }

                        txtAssignService.Focus();
                    }
                    isShowContainerServiceForChoose = false;
                }
                else
                {
                    gridViewAssignService.ActiveFilter.Clear();
                    this.currentMediMate = null;
                    if (!isShowContainerService)
                    {
                        popupControlContainerService.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinServiceAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinServiceAmount.Value > 0 && this.currentServiceAdo == null)
                    {
                        XtraMessageBox.Show("Bạn chưa chọn dịch vụ", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    if (spinServiceAmount.Value <= 0 && this.currentServiceAdo != null)
                    {
                        XtraMessageBox.Show("Số lượng phải > 0", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }

                    this.currentServiceAdo.AMOUNT = spinServiceAmount.Value;
                    this.currentServiceAdo.AMOUNT_PLUS = spinServiceAmount.Value;
                    this.currentServiceAdo.VIR_TOTAL_PRICE = spinServiceAmount.Value * this.currentServiceAdo.VIR_PRICE;
                    this.currentServiceAdo.VIR_TOTAL_PATIENT_PRICE = this.currentServiceAdo.VIR_TOTAL_PRICE;
                    if (!this.currentServiceAdo.IsExists)
                    {
                        this.listSereServAdo.Add(this.currentServiceAdo);
                    }
                    this.SetDataSourceTreeList();
                    this.SetTotalPrice();
                    this.ResetCurrentServiceAdo();
                    txtAssignService.Focus();
                    txtAssignService.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAssignService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO selectedADO = (HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO)this.gridViewAssignService.GetFocusedRow();
                    if (selectedADO != null)
                    {
                        isShowContainerService = false;
                        isShowContainerServiceForChoose = true;
                        popupControlContainerService.HidePopup();
                        AssignService_RowClick(selectedADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewAssignService.Focus();
                    this.gridViewAssignService.FocusedRowHandle = this.gridViewAssignService.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAssignService_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO selectedADO = (HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO)this.gridViewAssignService.GetFocusedRow();
                if (selectedADO != null)
                {
                    popupControlContainerService.HidePopup();
                    isShowContainerService = false;
                    isShowContainerServiceForChoose = true;
                    AssignService_RowClick(selectedADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonFocusAssignService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtAssignService.Focus();
                txtAssignService.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerService_CloseUp(object sender, EventArgs e)
        {
            try
            {
                this.isShowService = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    this.btnNew_Click(null, null);
                    return true;
                }
                else if (keyData == Keys.F4)
                {
                    this.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cboPresUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPresUser.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonSereServ_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServADO rowData = (SereServADO)gridViewSereServ.GetFocusedRow();
                if (rowData != null)
                {
                    this.listSereServAdo.Remove(rowData);
                    this.SetDataSourceTreeList();
                    this.SetTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsPrintNow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_PRINT_NOW && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkIsPrintNow.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_PRINT_NOW;
                    csAddOrUpdate.VALUE = (checkIsPrintNow.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void cboPatientGender_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                if (!checkIsVisitor.Checked)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnTreatmentCodeFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
