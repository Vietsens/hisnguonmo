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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.EInvoiceCreate.ADO;
using HIS.Desktop.Plugins.EInvoiceCreate.Config;
using HIS.Desktop.Plugins.EInvoiceCreate.Resources;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EInvoiceCreate
{
    public partial class FormEInvoiceCreate : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        private V_HIS_CASHIER_ROOM cashierRoom;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int start = 0;
        private bool IsExporting;
        private const int MAX_REQ = 100;

        string ModuleLinkName = "HIS.Desktop.Plugins.EInvoiceCreate";

        //tạo thread để lấy tất cả dữ liệu trước khi xử lý
        //các giao dịch chỉ lấy của hồ sơ đã có COUNT_TRANS_NOT_HAS_INVOICE > 0
        //các hồ sơ là khám nên lượng dịch vụ ít.
        //Chỉ lấy các giao dịch thanh toán
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private List<V_HIS_TRANSACTION> ListTransactionTotal = new List<V_HIS_TRANSACTION>();
        private List<HIS_SERE_SERV_BILL> ListSereServBillTotal = new List<HIS_SERE_SERV_BILL>();
        private List<V_HIS_SERE_SERV_5> ListSereServTotal = new List<V_HIS_SERE_SERV_5>();

        public FormEInvoiceCreate()
        {
            InitializeComponent();
        }

        public FormEInvoiceCreate(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            try
            {
                this.Text = currentModule.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormEInvoiceCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                SetCaptionByLanguageKey();
                HisConfigCFG.LoadConfig();
                InitDataCbo();
                LoadAccountBookToLocal();
                InitControlState();
                SetDefaultDataControl();
                SetDefaultAccountBook();//TODO

                FillDataToGrid();

                txtKeyWord.Focus();
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
                if (IsExporting) return;

                ClearDataTotal();

                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                FillDataToGridTransaction(new CommonParam(0, pagingSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransaction, param, pagingSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearDataTotal()
        {
            try
            {
                lblError.Text = "0";
                lblSuccess.Text = "0";
                lblTotalSelect.Text = "0";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTransaction(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 1");
                
                List<V_HIS_TREATMENT_FEE_3> listData = new List<V_HIS_TREATMENT_FEE_3>();
                gridControlTreatment.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTreatmentFeeView3Filter filter = new HisTreatmentFeeView3Filter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                if (SetFilter(ref filter))
                {
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 2");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_FEE_3>>("api/HisTreatment/GetFeeView3", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 3");
                    if (result != null)
                    {
                        rowCount = (result.Data == null ? 0 : result.Data.Count);
                        dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                        listData = result.Data;
                    }

                    gridControlTreatment.BeginUpdate();
                    gridControlTreatment.DataSource = listData;
                    gridControlTreatment.EndUpdate();
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 4");
                } 
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SetFilter(ref HisTreatmentFeeView3Filter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        try
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                        catch (Exception) { }
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string patientCode = txtPatientCode.Text.Trim();
                    if (patientCode.Length < 10)
                    {
                        try
                        {
                            patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                            txtPatientCode.Text = patientCode;
                        }
                        catch (Exception) { }
                    }
                    filter.PATIENT_CODE__EXACT = patientCode;
                }
                else
                {
                    //if ((dtInDate.EditValue == null || dtInDate.DateTime == DateTime.MinValue) && (dtOutDate.EditValue == null || dtOutDate.DateTime == DateTime.MinValue))
                    //{
                    //    XtraMessageBox.Show(ResourceLanguageManager.ThongBaoNhapNgay);
                    //    dtInDate.Focus();
                    //    dtInDate.SelectAll();
                    //    return false;
                    //}

                    if ((dtLastDepositTimeFrom.EditValue == null || dtLastDepositTimeFrom.DateTime == DateTime.MinValue) && (dtLastDepositTimeTo.EditValue == null || dtLastDepositTimeTo.DateTime == DateTime.MinValue))
                    {
                        XtraMessageBox.Show(ResourceLanguageManager.ThongBaoNhapNgay);
                        dtLastDepositTimeFrom.Focus();
                        dtLastDepositTimeFrom.SelectAll();
                        return false;
                    }

                    if (dtLastDepositTimeFrom.EditValue != null && dtLastDepositTimeFrom.DateTime != DateTime.MinValue)
                        filter.LAST_DEPOSIT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtLastDepositTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                    if (dtLastDepositTimeTo.EditValue != null && dtLastDepositTimeTo.DateTime != DateTime.MinValue)
                        filter.LAST_DEPOSIT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtLastDepositTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                    if (dtInDate.EditValue != null && dtInDate.DateTime != DateTime.MinValue)
                        filter.IN_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(dtInDate.DateTime.ToString("yyyyMMdd") + "000000");

                    if (dtOutDate.EditValue != null && dtOutDate.DateTime != DateTime.MinValue)
                        filter.OUT_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(dtOutDate.DateTime.ToString("yyyyMMdd") + "000000");

                    if (cboDepartment.EditValue != null)
                        filter.END_DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());

                    if (cboEndType.EditValue != null)
                        filter.IS_PAUSE = Inventec.Common.TypeConvert.Parse.ToInt64(cboEndType.EditValue.ToString()) == 1;

                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }

                filter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                filter.IS_NEED_INVOICE = true;
                //filter.IS_ACTIVE
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return true;
        }

        private void SetDefaultAccountBook()
        {
            try
            {
                cboAccountBook.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookTransactionBill != null && GlobalVariables.DefaultAccountBookTransactionBill.Count > 0)
                {
                    var lstBook = GlobalVariables.DefaultAccountBookTransactionBill.Where(o => ListAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.First();
                    }
                }

                if (HisConfigCFG.IsAutoSelectAccountBookIfHasOne && accountBook == null && ListAccountBook.Count == 1)
                {
                    accountBook = ListAccountBook.First();
                }

                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
                }
                else
                {
                    spNumOrder.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultDataControl()
        {
            try
            {
                if (cboPayForm.EditValue == null)
                {
                    cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                }
                var now = DateTime.Now;
                dtLastDepositTimeFrom.EditValue = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                dtLastDepositTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                dtInDate.EditValue = null;
                dtOutDate.EditValue = null;
                cboDepartment.EditValue = null;
                cboEndType.EditValue = 1;
                txtKeyWord.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                lblTotalSelect.Text = "0";
                lblSuccess.Text = "0";
                lblError.Text = "0";
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateEBill.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.btnCreateEBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndType.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.cboEndType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLastDepositTimeFrom.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciLastDepositTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLastDepositTimeTo.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciLastDepositTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLastDepositTimeFrom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciLastDepositTimeFrom.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLastDepositTimeTo.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciLastDepositTimeTo.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInDate.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciInDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOutDate.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciOutDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartment.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndType.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciEndType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.cboAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccountBook.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPayForm.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalSelect.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciTotalSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSuccess.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciSuccess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciError.Text = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.lciError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Stt.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_TreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientCode.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_PatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientName.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_PatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_GenderName.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_GenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientDob.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_PatientDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_TotalPatientPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TotalDepositAmount.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_TotalDepositAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TotalBillAmount.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_TotalBillAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TotalRepayAmount.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_TotalRepayAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_InTime.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_InTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_OutTime.Caption = Inventec.Common.Resource.Get.Value("FormEInvoiceCreate.gc_OutTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataCbo()
        {
            try
            {
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>();
                LoadCombo(cboDepartment, department, "ID", "DEPARTMENT_CODE", "DEPARTMENT_NAME");

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>();
                LoadCombo(cboPayForm, payForm, "ID", "PAY_FORM_CODE", "PAY_FORM_NAME");

                List<TypeADO> datas = new List<TypeADO>();
                datas.Add(new TypeADO(1, "Kết thúc điều trị"));
                datas.Add(new TypeADO(2, "Đang điều trị"));

                LoadCombo(cboEndType, datas, "ID", "", "NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBookToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                //TODO               
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                if (ListAccountBook != null && ListAccountBook.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
                    }
                }

                LoadCombo(cboAccountBook, ListAccountBook, "ID", "ACCOUNT_BOOK_CODE", "ACCOUNT_BOOK_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCombo(object control, object dataSource, string value, string code, string name)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrWhiteSpace(code))
                    columnInfos.Add(new ColumnInfo(code, "", 50, 1));

                columnInfos.Add(new ColumnInfo(name, "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, value, columnInfos, false, 250);
                ControlEditorLoader.Load(control, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutDate.Focus();
                    dtOutDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    cboDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEndType.Focus();
                    cboEndType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEndType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyWord.Focus();
                    txtKeyWord.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                }
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
                        FillDataToGrid();
                    }
                    else
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
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
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnCreateEBill_Click(null, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_FEE_3)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
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
                                if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                                }
                                else
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                                }
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

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spNumOrder.EditValue = null;
                spNumOrder.Enabled = false;
                if (cboAccountBook.EditValue != null)
                {
                    var account = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (account != null)
                    {
                        spNumOrder.EditValue = setDataToDicNumOrderInAccountBook(account);
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spNumOrder.Enabled = true;
                            spNumOrder.Focus();
                            spNumOrder.SelectAll();
                        }
                        else
                        {
                            cboPayForm.Focus();
                            cboPayForm.SelectAll();
                        }

                        // thu ngân mở 2 phòng.
                        // sổ ở phòng nào tự động chọn theo phòng đó.
                        if (GlobalVariables.DefaultAccountBookTransactionBill == null)
                        {
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                        }

                        if (GlobalVariables.DefaultAccountBookTransactionBill.Count > 0)
                        {
                            List<V_HIS_ACCOUNT_BOOK> acc = new List<V_HIS_ACCOUNT_BOOK>();
                            acc.AddRange(GlobalVariables.DefaultAccountBookTransactionBill);
                            //add lại sổ để luôn đưa sổ vừa chọn lên đầu.
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                            foreach (var item in acc)
                            {
                                if (item.ID != account.ID)
                                {
                                    GlobalVariables.DefaultAccountBookTransactionBill.Add(item);
                                }
                            }
                        }
                        else
                        {
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                        }
                    }
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

        private void cboAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue == null)
                    {
                        if (spNumOrder.Enabled)
                        {
                            spNumOrder.Focus();
                            spNumOrder.SelectAll();
                        }
                        else
                        {
                            cboPayForm.Focus();
                            cboPayForm.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateEBill_Click(object sender, EventArgs e)
        {
            try
            {
                lblSuccess.Text = "0";
                lblError.Text = "0";
                if (cboAccountBook.EditValue == null || cboPayForm.EditValue == null)
                {
                    List<string> mess = new List<string>();
                    if (cboAccountBook.EditValue == null)
                    {
                        mess.Add(ResourceLanguageManager.BanChuaChonSo);
                    }

                    if (cboPayForm.EditValue == null)
                    {
                        mess.Add(ResourceLanguageManager.BanChuaChonHinhThuc);
                    }

                    XtraMessageBox.Show(String.Join(". ", mess));

                    return;
                }

                var rowHandles = gridViewTreatment.GetSelectedRows();
                List<V_HIS_TREATMENT_FEE_3> listTreatment = new List<V_HIS_TREATMENT_FEE_3>();
                foreach (var i in rowHandles)
                {
                    var row = (V_HIS_TREATMENT_FEE_3)gridViewTreatment.GetRow(i);
                    if (row != null)
                    {
                        listTreatment.Add(row);
                    }
                }

                if (listTreatment.Count <= 0)
                {
                    XtraMessageBox.Show(ResourceLanguageManager.BanChuaChonHoSo);
                    return;
                }

                IsExporting = true;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                ListTransactionTotal = new List<V_HIS_TRANSACTION>();
                ListSereServBillTotal = new List<HIS_SERE_SERV_BILL>();
                ListSereServTotal = new List<V_HIS_SERE_SERV_5>();

                int countSuccess = 0;
                int countError = 0;

                long? numorder = null;
                if (spNumOrder.Enabled)
                {
                    numorder = Inventec.Common.TypeConvert.Parse.ToInt64(spNumOrder.EditValue.ToString());
                }

                CreateThreadLoadDataForElectronicBill(listTreatment);
                Dictionary<string, List<string>> dicMessage = new Dictionary<string, List<string>>();

                foreach (var treat in listTreatment)
                {
                    List<string> error = new List<string>();
                    if (ProcessCreateElectronicBill(treat, numorder, ref  error))
                    {
                        countSuccess += 1;
                        this.Invoke(new MethodInvoker(delegate() { lblSuccess.Text = countSuccess.ToString(); }));
                    }
                    else
                    {
                        countError += 1;
                        this.Invoke(new MethodInvoker(delegate() { lblError.Text = countError.ToString(); }));
                    }

                    if (error != null && error.Count > 0)
                    {
                        foreach (var err in error)
                        {
                            if (!dicMessage.ContainsKey(err))
                                dicMessage[err] = new List<string>();

                            dicMessage[err].Add(treat.TREATMENT_CODE);
                        }
                    }
                }

                UpdateDictionaryNumOrderAccountBook();

                if (dicMessage.Count > 0)
                {
                    foreach (var item in dicMessage)
                    {
                        param.Messages.Add(string.Format("{0}({1})", item.Key, string.Join(", ", item.Value.Distinct().ToList())));
                    }
                }
                else
                {
                    success = true;
                }

                FillDataToGridTransaction(new CommonParam() { Limit = rowCount, Count = dataTotal, });

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                IsExporting = false;
                WaitingManager.Hide();
            }
        }

        private void UpdateDictionaryNumOrderAccountBook()
        {
            try
            {
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));

                if (accountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    CommonParam param = new CommonParam();
                    HisAccountBookFilter filter = new HisAccountBookFilter();
                    filter.ID = accountBook.ID;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, filter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = apiResult.First().NUM_ORDER;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadDataForElectronicBill(List<V_HIS_TREATMENT_FEE_3> listTreatment)
        {
            try
            {
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    List<Task> taskall = new List<Task>();
                    int skip = 0;
                    while (listTreatment.Count - skip > 0)
                    {
                        List<V_HIS_TREATMENT_FEE_3> listData = listTreatment.Skip(skip).Take(MAX_REQ).ToList();
                        skip += MAX_REQ;

                        Task tsSereServ = Task.Factory.StartNew((object obj) =>
                        {
                            GetSereServByTreatment((List<V_HIS_TREATMENT_FEE_3>)obj);
                        }, listData);
                        taskall.Add(tsSereServ);

                        Task tsTreatment = Task.Factory.StartNew((object obj) =>
                        {
                            GetTransactionByTreatment((List<V_HIS_TREATMENT_FEE_3>)obj);
                        }, listData);
                        taskall.Add(tsTreatment);
                    }

                    Task.WaitAll(taskall.ToArray());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessCreateElectronicBill(V_HIS_TREATMENT_FEE_3 row, long? numorder, ref List<string> error)
        {
            bool result = false;
            try
            {
                if (error == null)
                    error = new List<string>();

                List<V_HIS_TRANSACTION> listTran = new List<V_HIS_TRANSACTION>();
                Dictionary<long, List<V_HIS_SERE_SERV_5>> dicSereServs = new Dictionary<long, List<V_HIS_SERE_SERV_5>>();

                if (((row.TOTAL_PATIENT_PRICE ?? 0) > (row.TOTAL_BILL_AMOUNT ?? 0)) &&
                    ((row.TOTAL_DEPOSIT_AMOUNT ?? 0) > ((row.TOTAL_REPAY_AMOUNT ?? 0) + (row.TOTAL_BILL_TRANSFER_AMOUNT ?? 0))) &&
                    row.IS_ACTIVE == 1)
                {
                    HisTransactionBillByDepositSDO sdo = new HisTransactionBillByDepositSDO();
                    sdo.AccountBookId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString());
                    sdo.NumOrder = numorder;
                    sdo.PayformId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString());
                    sdo.TransactionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    sdo.TreatmentId = row.ID;
                    sdo.WorkingRoomId = currentModule.RoomId;
                    if (chkIsSplitByCashierDeposit.Checked)
                        sdo.IsSplitByCashierDeposit = true;
                    CommonParam param = new CommonParam();
                    var apiResult = new BackendAdapter(param).Post<List<HisTransactionBillResultSDO>>("api/HisTransaction/BillByDeposit", ApiConsumers.MosConsumer, sdo, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        foreach (var item in apiResult)
                        {
                            listTran.Add(item.TransactionBill);
                            //tăng numorder lên 1
                            if (numorder.HasValue)
                                numorder += 1;

                            if (item.SereServBills != null && item.SereServBills.Count > 0)
                            {
                                var grBillId = item.SereServBills.GroupBy(o => o.BILL_ID).ToList();
                                foreach (var ssb in grBillId)
                                {
                                    var ss = ListSereServTotal.Where(o => ssb.Select(s => s.SERE_SERV_ID).Contains(o.ID)).ToList();
                                    if (ss != null && ss.Count > 0)
                                    {
                                        if (!dicSereServs.ContainsKey(ssb.Key))
                                            dicSereServs[ssb.Key] = new List<V_HIS_SERE_SERV_5>();
                                        dicSereServs[ssb.Key].AddRange(ss);
                                    }
                                }
                            }  
                        }
                        
                    }
                    else
                    {
                        error.AddRange(param.Messages);
                        return result;
                    }
                }

                var transaction = ListTransactionTotal.Where(o => o.TREATMENT_ID == row.ID && String.IsNullOrWhiteSpace(o.INVOICE_CODE)).ToList();
                if (transaction != null && transaction.Count > 0)
                {
                    listTran.AddRange(transaction);
                    var listSereServBill = ListSereServBillTotal.Where(o => transaction.Select(s => s.ID).Contains(o.BILL_ID)).ToList();
                    if (listSereServBill != null && listSereServBill.Count > 0)
                    {
                        var grBillId = listSereServBill.GroupBy(o => o.BILL_ID).ToList();
                        foreach (var ssb in grBillId)
                        {
                            var ss = ListSereServTotal.Where(o => ssb.Select(s => s.SERE_SERV_ID).Contains(o.ID)).ToList();
                            if (ss != null && ss.Count > 0)
                            {
                                if (!dicSereServs.ContainsKey(ssb.Key))
                                    dicSereServs[ssb.Key] = new List<V_HIS_SERE_SERV_5>();
                                dicSereServs[ssb.Key].AddRange(ss);
                            }
                        }
                    }
                }

                if (listTran != null && listTran.Count > 0)
                {
                    foreach (var item in listTran)
                    {
                        if (dicSereServs.ContainsKey(item.ID))
                        {
                            ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(row, item, dicSereServs[item.ID]);
                            if (electronicBillResult == null || !electronicBillResult.Success)
                            {
                                if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                                {
                                    error.AddRange(electronicBillResult.Messages.Distinct().ToList());
                                }
                                else
                                {
                                    error.Add("Tạo hóa đơn điện tử thất bại.");
                                }
                            }
                            else
                            {
                                //goi api update
                                CommonParam paramUpdate = new CommonParam();
                                HisTransactionInvoiceInfoSDO sdo = new HisTransactionInvoiceInfoSDO();
                                sdo.EinvoiceLoginname = electronicBillResult.InvoiceLoginname;
                                sdo.InvoiceCode = electronicBillResult.InvoiceCode;
                                sdo.InvoiceSys = electronicBillResult.InvoiceSys;
                                sdo.EinvoiceNumOrder = electronicBillResult.InvoiceNumOrder;
                                sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                                sdo.Id = item.ID;
                                sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                                var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                                if (!apiResult && paramUpdate.Messages != null && paramUpdate.Messages.Count > 0)
                                {
                                    error.AddRange(paramUpdate.Messages);
                                }
                            }
                        }
                    }
                }

                if (error.Count > 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(V_HIS_TREATMENT_FEE_3 row, V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV_5> sereServs)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (sereServs == null)
                {
                    result.Success = false;
                    Inventec.Common.Logging.LogSystem.Debug("Khong co dich vu thanh toan nao duoc chon!");
                    return result;
                }

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transaction);

                V_HIS_TREATMENT_FEE treat = new V_HIS_TREATMENT_FEE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_FEE>(treat, row);

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                if (transaction.EXEMPTION.HasValue)
                {
                    dataInput.Discount = transaction.EXEMPTION;
                    dataInput.DiscountRatio = Math.Round(transaction.EXEMPTION.Value / transaction.AMOUNT, 2);
                }
                dataInput.PaymentMethod = transaction.PAY_FORM_NAME;
                dataInput.SereServs = sereServs;
                dataInput.Treatment = treat;
                dataInput.Currency = "VND";
                dataInput.Transaction = tran;
                dataInput.TransactionTime = transaction.TRANSACTION_TIME;
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;

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

        private void GetSereServByTreatment(List<V_HIS_TREATMENT_FEE_3> listTreatment)
        {
            try
            {
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    int skip = 0;
                    while (listTreatment.Count - skip > 0)
                    {
                        List<V_HIS_TREATMENT_FEE_3> listData = listTreatment.Skip(skip).Take(MAX_REQ).ToList();
                        skip += MAX_REQ;

                        CommonParam param = new CommonParam();
                        HisSereServView5Filter filter = new HisSereServView5Filter();
                        filter.TDL_TREATMENT_IDs = listData.Select(s => s.ID).ToList();
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, filter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            ListSereServTotal.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTransactionByTreatment(List<V_HIS_TREATMENT_FEE_3> listTreatment)
        {
            try
            {
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    int skip = 0;
                    while (listTreatment.Count - skip > 0)
                    {
                        List<V_HIS_TREATMENT_FEE_3> listData = listTreatment.Skip(skip).Take(MAX_REQ).ToList();
                        skip += MAX_REQ;

                        CommonParam param = new CommonParam();
                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.TREATMENT_IDs = listData.Select(s => s.ID).ToList();
                        tranFilter.TRANSACTION_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                        tranFilter.IS_CANCEL = false;
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            ListTransactionTotal.AddRange(apiResult);

                            HisSereServBillFilter ssbFilter = new HisSereServBillFilter();
                            ssbFilter.BILL_IDs = apiResult.Select(s => s.ID).ToList();
                            var apiSsbResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssbFilter, param);
                            if (apiSsbResult != null && apiSsbResult.Count > 0)
                            {
                                ListSereServBillTotal.AddRange(apiSsbResult);
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

        private void gridViewTreatment_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                lblTotalSelect.Text = gridViewTreatment.GetSelectedRows().Length + "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
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
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkIsSplitByCashierDeposit.Name)
                        {
                            chkIsSplitByCashierDeposit.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSplitByCashierDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkIsSplitByCashierDeposit.Name && o.MODULE_LINK == this.ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsSplitByCashierDeposit.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIsSplitByCashierDeposit.Name;
                    csAddOrUpdate.VALUE = (chkIsSplitByCashierDeposit.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.ModuleLinkName;
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
    }
}
