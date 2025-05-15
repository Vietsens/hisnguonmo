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
using AutoMapper;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionList.ADO;
using HIS.Desktop.Plugins.TransactionList.Base;
using HIS.Desktop.Plugins.TransactionList.Config;
using HIS.Desktop.Plugins.TransactionList.Properties;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.Filter;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.TransactionList.Resources;
using Inventec.Common.Logging;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Desktop.Common.LanguageManager;
using MOS.SDO;
using HIS.Desktop.ADO;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionList : HIS.Desktop.Utility.FormBase
    {
        V_HIS_TREATMENT_FEE treatment = null;
        V_HIS_ACCOUNT_BOOK accountBook = null;
        string loginName = null;
        int perId;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        long typeIdBill = 0;
        long typeIdDeposit = 0;
        long typeIdRepay = 0;
        bool hasEditPermission = false;
        List<V_HIS_TRANSACTION> _transactionList { get; set; }
        //private List<HIS_PERMISSION> _hisPermissionList;
        BarManager baManager = null;
        V_HIS_TRANSACTION transactionPrint;
        List<V_HIS_TRANSACTION> listData;
        PopupMenuProcessor popupMenuProcessor = null;

        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_TRANSACTION_TYPE> ListLoaiGiaoDich;
        List<LoaiHoaDon> ListLoaiHoaDon;
        List<LoaiHoaDon> ListLoaiHoaDon_;
        public static List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        ACS_CONTROL controlDelete;
        List<ACS.SDO.AcsRoleUserSDO> RoleUse;
        List<ACS_CONTROL_ROLE> ControlRule;

        private bool isPermission = false;
        private List<HIS_PERMISSION> hisPermissionList = new List<HIS_PERMISSION>();


        bool isNotLoadWhilecboFilterStateInFirst = true;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TransactionList";
        string strTransactionError = "";
        bool isOpenFromHisTransaction = false;
        public frmTransactionList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();

                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionList(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                
                this.treatment = data;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                }
                this.txtTreatmentCodeFind.Text = this.treatment.TREATMENT_CODE;
                this.txtTreatmentCodeFind.Focus();
                this.txtTreatmentCodeFind.SelectAll();
                isOpenFromHisTransaction = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionList(Inventec.Desktop.Common.Modules.Module module, V_HIS_ACCOUNT_BOOK data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.accountBook = data;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                }
                this.txtAccountBookCodeFind.Text = data.ACCOUNT_BOOK_CODE;
                this.txtAccountBookCodeFind.Focus();
                this.txtAccountBookCodeFind.SelectAll();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                CheckEnableBtnQR();
                LoadAcsControls();
                LoadDataCboFilterType();
                LoadKeyFrmLanguage();
                SetDefaultControlDateTime();
                
                InitCheck(grdLoaiGiaoDich, SelectionGrid__LoaiGiaoDich);
                InitCombo(grdLoaiGiaoDich, BackendDataWorker.Get<HIS_TRANSACTION_TYPE>(), "TRANSACTION_TYPE_NAME", "ID");
                LoaihoadongList();
                InitCheck(grdLoaiHoaDon, SelectionGrid__LoaiHoaDon);
                InitCombo(grdLoaiHoaDon, this.ListLoaiHoaDon_, "Name", "ID");

                if (isOpenFromHisTransaction)
                {
                    SetDefaultValueControlFromHisTran();
                }
                else
                {
                    SetDefaultValueControl();
                }

                InitControlState();
                CheckKeyCauHinh();
                  
                FillDataToGrid();
                GetHisPermission();
                GetHisPermissionLoad();
                btnExportBill.Enabled = false;
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
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == cboFilter.Name)
                        {
                            cboFilter.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(item.VALUE);
                        }

                        if (item.KEY == grdLoaiHoaDon.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                List<int> listIDS = item.VALUE.Split(',').Select(s => int.Parse(s)).ToList();
                                List<LoaiHoaDon> listHDType = ListLoaiHoaDon_.Where(o => listIDS.Any(p => p == o.ID)).ToList();
                                GridCheckMarksSelection gridCheckMark = grdLoaiHoaDon.Properties.Tag as GridCheckMarksSelection;
                                gridCheckMark.ClearSelection(grdLoaiHoaDon.Properties.View);
                                grdLoaiHoaDon.EditValue = null;
                                grdLoaiHoaDon.Focus();
                                this.ListLoaiHoaDon.AddRange(listHDType);
                                gridCheckMark.SelectAll(this.ListLoaiHoaDon);
                            }
                        }
                        if (item.KEY == grdLoaiGiaoDich.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                List<long> listIDS_ = item.VALUE.Split(',').Select(s => long.Parse(s)).ToList();
                                List<HIS_TRANSACTION_TYPE> listTrType = BackendDataWorker.Get<HIS_TRANSACTION_TYPE>().Where(o => listIDS_.Contains(o.ID)).ToList();

                                GridCheckMarksSelection gridCheckMark = grdLoaiGiaoDich.Properties.Tag as GridCheckMarksSelection;
                                gridCheckMark.ClearSelection(grdLoaiGiaoDich.Properties.View);
                                grdLoaiGiaoDich.EditValue = null;
                                grdLoaiGiaoDich.Focus();
                                this.ListLoaiGiaoDich.AddRange(listTrType);
                                gridCheckMark.SelectAll(this.ListLoaiGiaoDich);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhilecboFilterStateInFirst = false;
        }

        private void LoadAcsControls()
        {
            try
            {
                controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                var apiResult = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == this.loginName);
                if (apiResult != null)
                {
                    CommonParam param = new CommonParam();
                    AcsRoleUserFilter roleUserFilter = new AcsRoleUserFilter();
                    roleUserFilter.USER_ID = apiResult.ID;
                    this.RoleUse = new BackendAdapter(param).Get<List<ACS.SDO.AcsRoleUserSDO>>
                      ("api/AcsRoleUser/Get", ApiConsumers.AcsConsumer, roleUserFilter, param);
                }

                CommonParam paramControlRole = new CommonParam();
                AcsControlRoleFilter filter = new AcsControlRoleFilter();
                var controlRule = new BackendAdapter(paramControlRole).Get<List<ACS_CONTROL_ROLE>>
                       ("api/AcsControlRole/Get", ApiConsumers.AcsConsumer, filter, paramControlRole);
                CommonParam parmaControl = new CommonParam();
                this.controlDelete = BackendDataWorker.Get<ACS_CONTROL>().FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnDelete);
                if (this.controlDelete != null && controlRule != null && controlRule.Count > 0)
                {
                    this.ControlRule = controlRule.Where(o => o.CONTROL_ID == this.controlDelete.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboFilterType()
        {
            try
            {
                List<ADO.FilterTypeADO> listFilterType = new List<ADO.FilterTypeADO>();

                listFilterType.Add(new ADO.FilterTypeADO(0, new Base.GlobalStore().TOI_TAO));
                listFilterType.Add(new ADO.FilterTypeADO(1, new Base.GlobalStore().PHONG));
                listFilterType.Add(new ADO.FilterTypeADO(2, new Base.GlobalStore().TAT_CA));

                //if (treatment != null)
                //{
                //    this.treatment = LoadDataToCurrentTreatmentData(this.treatment.ID);
                //}

                cboFilter.Properties.DataSource = listFilterType;
                cboFilter.Properties.DisplayMember = "FilterTypeName";
                cboFilter.Properties.ValueMember = "ID";
                cboFilter.Properties.ForceInitialize();
                cboFilter.Properties.Columns.Clear();
                cboFilter.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterTypeName", "", 200));
                cboFilter.Properties.ShowHeader = false;
                cboFilter.Properties.ImmediatePopup = true;
                cboFilter.Properties.DropDownRows = 5;
                cboFilter.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //HIS.DESKTOP.TRANSACTION_LIST.SHOW_TRANS_OF_OTHER_OPTION
        //- Nếu giá trị = 1 hoặc tài khoản người dùng là admin: Xử lý như hiện tại (vẫn enable cho phép sửa combobox filter người tạo.
        //- Nếu giá trị = 0 (khác 1) và tài khoản người dùng khong phải là admin: Set mặc định combobox người tạo là "Tôi tạo" và đisable không cho phép sửa.
        private void CheckKeyCauHinh()
        {
            try
            {
                var isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                if (HisConfigCFG.TRANSACTION_LIST_SHOW_TRANS_OF_OTHER_OPTION != "1" && isAdmin == false)
                {
                    cboFilter.EditValue = (long)0;
                    cboFilter.ReadOnly = true;
                }
                gridColumn_UnrejectCancellation.VisibleIndex = HisConfigCFG.AllowWhenRequest == "1" ? 6 : -1;

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
                cboFilter.EditValue = (long)0;
                txtKeyword.Text = "";
                txtCodePayQr.Text = "";
                txtTransactionCode.Text = "";
                txtAccountBookCodeFind.Text = "";
                txtTreatmentCodeFind.Text = "";
                SetDefaultControlDateTime();
                //checkTransactionTypeBill.Checked = false;
                //checkTransactionTypeDeposit.Checked = false;
                //checkTransactionTypeRepay.Checked = false;

                //GridCheckMarksSelection gridCheckMark = grdLoaiGiaoDich.Properties.Tag as GridCheckMarksSelection;
                //gridCheckMark.ClearSelection(grdLoaiGiaoDich.Properties.View);
                //GridCheckMarksSelection gridCheckMark_ = grdLoaiHoaDon.Properties.Tag as GridCheckMarksSelection;
                //gridCheckMark_.ClearSelection(grdLoaiGiaoDich.Properties.View);
                cbStatusAll.Checked = true;
                cbStatusAll1.Checked = true;
                lcProcessed.Text = " ";
                lcErrorProcessed.Text = " ";
                btnExportBill.Enabled = false;
                txtErrorProcessed.Text = "";
                txtProcessed.Text = "";
                lciWarning.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlFromHisTran()
        {
            try
            {
                cboFilter.EditValue = (long)0;
                txtKeyword.Text = "";
                txtCodePayQr.Text = "";
                txtTransactionCode.Text = "";
                txtAccountBookCodeFind.Text = "";
                SetDefaultControlDateTime();
                //checkTransactionTypeBill.Checked = false;
                //checkTransactionTypeDeposit.Checked = false;
                //checkTransactionTypeRepay.Checked = false;
                GridCheckMarksSelection gridCheckMark = grdLoaiGiaoDich.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(grdLoaiGiaoDich.Properties.View);
                GridCheckMarksSelection gridCheckMark_ = grdLoaiHoaDon.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark_.ClearSelection(grdLoaiGiaoDich.Properties.View);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlDateTime()
        {
            try
            {
                dtCreateTimeFrom.EditValue = null;
                dtCreateTimeTo.EditValue = null;
                if (accountBook != null || (accountBook == null && treatment == null))
                {
                    dtCreateTimeFrom.DateTime = DateTime.Now;
                    dtCreateTimeTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Phục vụ khi mở từ menu Danh sách Giao dịch
        private void FillDataToGrid()
        {
            try
            {
                
                FillDataToGridTransaction(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransaction, param, (int)ConfigApplications.NumPageSize, this.gridControlTransaction);
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
                List<HisTransactionADO> listTransaction = new List<HisTransactionADO>();
                gridControlTransaction.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtTreatmentCodeFind.Text))
                {
                    string code = txtTreatmentCodeFind.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCodeFind.Text = code;
                        txtTreatmentCodeFind.Focus();
                        txtTreatmentCodeFind.Select();
                    }
                    if (this.treatment == null)
                    {
                        MOS.Filter.HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                        treatmentFilter.TREATMENT_CODE__EXACT = code;
                        var treatmentFeeList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, null);
                        this.treatment = treatmentFeeList != null && treatmentFeeList.Count > 0 ? treatmentFeeList.FirstOrDefault() : null;
                    }

                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!String.IsNullOrEmpty(txtCodePayQr.Text))
                {
                    string codeQr = txtCodePayQr.Text.Trim();
                    if (codeQr.Length < 12)
                    {
                        codeQr = string.Format("{0:000000000000}", Convert.ToInt64(codeQr));
                        txtCodePayQr.Text = codeQr;
                        txtCodePayQr.Focus();
                        txtCodePayQr.Select();
                    }
                    filter.TRANS_REQ_CODE__EXACT = codeQr;

                }
                if (!String.IsNullOrEmpty(txtTransactionCode.Text))
                {
                    string tranCode = txtTransactionCode.Text.Trim();
                    if (tranCode.Length < 12)
                    {
                        tranCode = string.Format("{0:000000000000}", Convert.ToInt64(tranCode));
                        txtTransactionCode.Text = tranCode;
                        txtTransactionCode.Focus();
                        txtTransactionCode.Select();
                    }
                    filter.TRANSACTION_CODE__EXACT = tranCode;
                }
                else if (!String.IsNullOrEmpty(txtAccountBookCodeFind.Text))
                {
                    filter.ACCOUNT_BOOK_CODE__EXACT = txtAccountBookCodeFind.Text.Trim();
                    txtAccountBookCodeFind.Focus();
                    txtAccountBookCodeFind.SelectAll();
                }
                else if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }

                int value = Convert.ToInt32(cboFilter.EditValue);

                //var hiscashierId = hiscashiers.Where(o => o.ROOM_ID == currentModule.RoomId).FirstOrDefault().ID;
                if (value == 0)//tôi tạo
                {
                    filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
                else if (value == 1) // phòng làm việc
                {
                    MOS.Filter.HisCashierRoomFilter hiscashier = new HisCashierRoomFilter();
                    var hiscashiers = new BackendAdapter(new CommonParam()).Get<List<HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, hiscashier, null);


                    if (hiscashiers != null && hiscashiers.Count > 0)
                    {
                        var hiscashierId = hiscashiers.Where(o => o.ROOM_ID == currentModule.RoomId).FirstOrDefault().ID;
                        if (currentModule != null && currentModule.RoomId > 0)
                            filter.CASHIER_ROOM_ID = hiscashierId;
                    }
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (this.ListLoaiHoaDon.Count() == 1 && this.ListLoaiHoaDon.Where(o => o.ID == 1) != null && this.ListLoaiHoaDon.Where(o => o.ID == 1).Count() > 0)
                {
                    filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = true;
                }
                else if (this.ListLoaiHoaDon.Count() == 1 && this.ListLoaiHoaDon.Where(o => o.ID == 2) != null && this.ListLoaiHoaDon.Where(o => o.ID == 2).Count() > 0)
                {
                    filter.BILL_TYPE_IS_NULL_OR_EQUAL_1 = false;
                }

                if (!String.IsNullOrWhiteSpace(txtAccountBookName.Text))
                {
                    filter.ACCOUNT_BOOK_NAME = txtAccountBookName.Text;
                }
                if (!String.IsNullOrWhiteSpace(txtAccountBookSymbol.Text))
                {
                    filter.SYMBOL_CODE__EXACT = txtAccountBookSymbol.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(txtAccountBookTemp.Text))
                {
                    filter.TEMPLATE_CODE__EXACT = txtAccountBookTemp.Text.Trim();
                }

                //if (checkTransactionTypeBill.Checked)
                //{
                //    if (filter.TRANSACTION_TYPE_IDs == null)
                //    {
                //        filter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                //        typeIdBill = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                //        filter.TRANSACTION_TYPE_IDs.Add(typeIdBill);
                //    }
                //    else
                //        filter.TRANSACTION_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT);
                //}

                //if (checkTransactionTypeDeposit.Checked)
                //{
                //    if (filter.TRANSACTION_TYPE_IDs == null)
                //        filter.TRANSACTION_TYPE_IDs = new List<long>();
                //    if (typeIdDeposit <= 0)
                //    {
                //        try
                //        {
                //            var type = BackendDataWorker.Get<HIS_TRANSACTION_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU);
                //            if (type != null)
                //            {
                //                typeIdDeposit = type.ID;
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            Inventec.Common.Logging.LogSystem.Error(ex);
                //        }
                //    }

                //    filter.TRANSACTION_TYPE_IDs.Add(typeIdDeposit);
                //}

                //if (checkTransactionTypeRepay.Checked)
                //{
                //    if (filter.TRANSACTION_TYPE_IDs == null)
                //        filter.TRANSACTION_TYPE_IDs = new List<long>();
                //    if (typeIdRepay <= 0)
                //    {
                //        try
                //        {
                //            var type = BackendDataWorker.Get<HIS_TRANSACTION_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU);
                //            if (type != null)
                //            {
                //                typeIdRepay = type.ID;
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            Inventec.Common.Logging.LogSystem.Error(ex);
                //        }
                //    }

                //    filter.TRANSACTION_TYPE_IDs.Add(typeIdRepay);
                //}


                if (this.ListLoaiGiaoDich != null && this.ListLoaiGiaoDich.Count > 0)
                {
                    filter.TRANSACTION_TYPE_IDs = this.ListLoaiGiaoDich.Select(o => o.ID).ToList();
                }
                if (chkElectronicDone.Checked)
                {
                    filter.HAS_INVOICE_CODE = true;
                }
                else if (chkElectronicNone.Checked)
                {
                    filter.HAS_INVOICE_CODE = false;
                }
                if (cbStatusAll.Checked)
                {
                    filter.IS_CANCEL = null;
                }
                else if (cbStatusCancelled.Checked)
                {
                    filter.IS_CANCEL = true;
                }
                else if (cbStatusNotCancel.Checked)
                {
                    filter.IS_CANCEL = false;
                }
                if (cbStatusAll1.Checked)
                {
                    filter.IS_ACTIVE = null;
                }
                else if (cbStatusNoLook.Checked)
                {
                    filter.IS_ACTIVE = 1;
                }
                else if (cbStatusLook.Checked)
                {
                    filter.IS_ACTIVE = 0;
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TRANSACTION>>(HisRequestUriStore.HIS_TRANSACTION_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);

                _transactionList = result.Data;
                if (result != null)
                {
                    var listData = (List<V_HIS_TRANSACTION>)result.Data;
                    if (listData != null && listData.Count > 0)
                    {
                        Mapper.CreateMap<V_HIS_TRANSACTION, HisTransactionADO>();
                        listTransaction = Mapper.Map<List<HisTransactionADO>>(listData);
                        listTransaction.ForEach(o => o.OLD_NUM_ORDER = o.NUM_ORDER);
                    }
                    rowCount = (listTransaction == null ? 0 : listTransaction.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("result_____tt:", result));
                gridControlTransaction.BeginUpdate();
                gridControlTransaction.DataSource = listTransaction;
                gridControlTransaction.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Phục vụ khi mở từ nút LS Giao dịch
        private void FillDataToGridForOneCode()
        {
            try
            {
                FillDataToGridTransaction(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransactionForOneCode, param, (int)ConfigApplications.NumPageSize, this.gridControlTransaction);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //        private void GetHisPermission() {
        //            try
        //            {
        //                CommonParam param  = new CommonParam();


        //                HisPermissionFilter filter = new HisPermissionFilter()
        //;
        //                filter.IS_ACTIVE = 1;
        //                filter.PERMISSION_TYPE_ID = 2;
        //                filter.LOGINNAME = this.loginName;
        //                filter.EFFECTIVE_DATE__EXACT = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //                var permissionList = new BackendAdapter(param)
        //             .Get<List<HIS_PERMISSION>>("api/HisPermission/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
        //                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("resultttttt2_____:", permissionList));
        //                if (permissionList != null || permissionList.Count == 0)
        //                {
        //                    isPermission = true;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Inventec.Common.Logging.LogSystem.Error(ex);
        //            }
        //        }
        private bool GetHisPermission()
        {
            try
            {
                CommonParam param = new CommonParam();

                var from = Convert.ToInt64(DateTime.Today.ToString("yyyyMMdd000000"));
                var to = Convert.ToInt64(DateTime.Today.ToString("yyyyMMdd235959"));

                HisPermissionFilter filter = new HisPermissionFilter()
                {
                    IS_ACTIVE = 1,
                    PERMISSION_TYPE_ID = 2,
                    LOGINNAME = this.loginName,
                    EFFECTIVE_DATE_FROM = from,
                    EFFECTIVE_DATE_TO = to
                };

                var permissionList = new BackendAdapter(param)
                    .Get<List<HIS_PERMISSION>>("api/HisPermission/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("result_____permission:", permissionList));
                
                return permissionList != null && permissionList.Count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Lỗi khi gọi GetHisPermission: " + ex.ToString());
                return false;
            }
        }

        private List<HIS_PERMISSION> GetHisPermissionLoad()
        {
            try
            {

                List<HIS_PERMISSION> listPermission = new List<HIS_PERMISSION>();
                CommonParam param = new CommonParam();
                HisPermissionFilter filter = new HisPermissionFilter();

                filter.IS_ACTIVE = 1;
                filter.PERMISSION_TYPE_ID = 1;
                filter.LOGINNAME = this.loginName;
                filter.EFFECTIVE_DATE_FROM = long.Parse(dtCreateTimeFrom.DateTime.ToString("yyyyMMddHHmmss"));
                filter.EFFECTIVE_DATE_TO = long.Parse(dtCreateTimeTo.DateTime.ToString("yyyyMMddHHmmss"));


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetHisPermissionLoad - Filter input:", filter));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetHisPermissionLoad - Param input:", param));


                listPermission = new BackendAdapter(new CommonParam())
                    .Get<List<HIS_PERMISSION>>("api/HisPermission/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                   
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("resultttttt_____:", listPermission));
               
                

                return listPermission;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("GetHisPermissionLoad - Exception: " + ex.ToString());
                return new List<HIS_PERMISSION>();
            }
        }





        private void FillDataToGridTransactionForOneCode(object param)
        {
            try
            {
                List<HisTransactionADO> listTransaction = new List<HisTransactionADO>();
                gridControlTransaction.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtTreatmentCodeFind.Text))
                {
                    string code = txtTreatmentCodeFind.Text.Trim();

                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtCodePayQr.Text))
                {
                    string codeQr = txtCodePayQr.Text.Trim();
                    filter.TRANS_REQ_CODE__EXACT = codeQr;
                }
                if (!string.IsNullOrEmpty(txtTransactionCode.Text))
                {
                    string tranCode = txtTransactionCode.Text.Trim();
                    filter.TRANSACTION_CODE__EXACT = tranCode;
                }
                       
                int value = Convert.ToInt32(cboFilter.EditValue);

                //var hiscashierId = hiscashiers.Where(o => o.ROOM_ID == currentModule.RoomId).FirstOrDefault().ID;
                if (value == 0)//tôi tạo
                {
                    filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
                else if (value == 1) // phòng làm việc
                {
                    MOS.Filter.HisCashierRoomFilter hiscashier = new HisCashierRoomFilter();
                    var hiscashiers = new BackendAdapter(new CommonParam()).Get<List<HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, hiscashier, null);

                    if (hiscashiers != null && hiscashiers.Count > 0)
                    {
                        var hiscashierId = hiscashiers.Where(o => o.ROOM_ID == currentModule.RoomId).FirstOrDefault().ID;
                        if (currentModule != null && currentModule.RoomId > 0)
                            filter.CASHIER_ROOM_ID = hiscashierId;
                    }
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }


                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TRANSACTION>>(HisRequestUriStore.HIS_TRANSACTION_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    var listData = (List<V_HIS_TRANSACTION>)result.Data;
                    if (listData != null && listData.Count > 0)
                    {
                        Mapper.CreateMap<V_HIS_TRANSACTION, HisTransactionADO>();
                        listTransaction = Mapper.Map<List<HisTransactionADO>>(listData);
                        listTransaction.ForEach(o => o.OLD_NUM_ORDER = o.NUM_ORDER);
                    }
                    rowCount = (listTransaction == null ? 0 : listTransaction.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTransaction.BeginUpdate();
                gridControlTransaction.DataSource = listTransaction;
                gridControlTransaction.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGrid();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCodeFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCodeFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCodePayQr_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtTransactionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtAccountBookCodeFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void gridViewTransaction_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HisTransactionADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                                if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                                }
                                else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                                {
                                    decimal? ado = data.AMOUNT - (data.KC_AMOUNT ?? 0) - (data.TDL_BILL_FUND_AMOUNT ?? 0) - (data.EXEMPTION ?? 0);
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(ado ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO)
                                {
                                    e.Value = 0;
                                }
                                //decimal? ado = data.AMOUNT - (data.KC_AMOUNT ?? 0) - (data.TDL_BILL_FUND_AMOUNT ?? 0) - (data.EXEMPTION ?? 0);
                                //e.Value = Inventec.Common.Number.Convert.NumberToString(ado ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "ROUNDED_TOTAL_PRICE_STR")
                        {
                            try
                            {
                                if (data.ROUNDED_TOTAL_PRICE != null)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.ROUNDED_TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "KC_AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.KC_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_BILL_FUND_AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.TDL_BILL_FUND_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        else if (e.Column.FieldName == "StatusStr")
                        {
                            try
                            {
                                e.Value = data.IS_CANCEL == 1 ? "Đã hủy" : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXEMPTION_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DIRECTLY_BILLING_STR")
                        {
                            if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                            {
                                if (data.IS_DIRECTLY_BILLING == 1)
                                {
                                    e.Value = "Thu trực tiếp";
                                }
                                else
                                {
                                    e.Value = "Ra viện";
                                }
                            }
                            //else
                            //{
                            //    e.Value = data.TRANSACTION_TYPE_NAME;
                            //}
                        }
                        else if (e.Column.FieldName == gc_SwipeAmount.FieldName)
                        {
                            try
                            {
                                if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.SWIPE_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == gc_TransferAmuont.FieldName)
                        {
                            try
                            {
                                if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CANCEL_TIME_str")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CANCEL_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CANCEL_LOGINNAME_str")
                        {
                            try
                            {
                                e.Value = !string.IsNullOrEmpty(data.CANCEL_LOGINNAME) ? string.Format("{0} - {1}", data.CANCEL_LOGINNAME, data.CANCEL_USERNAME) : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CANCEL_REASON_str")
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(data.CANCEL_REASON) || string.IsNullOrEmpty(data.CANCEL_REASON_NAME))
                                    e.Value = data.CANCEL_REASON ?? data.CANCEL_REASON_NAME;
                                else
                                    e.Value = string.Format("{0}; {1}", data.CANCEL_REASON_NAME, data.CANCEL_REASON);
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

        private void gridViewTransaction_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (HisTransactionADO)gridViewTransaction.GetRow(e.RowHandle);
                    if (data != null)
                    {

                        if (e.Column.FieldName == "BILL_DETAIL")
                        {
                            try
                            {
                                if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && data.TDL_SERE_SERV_DEPOSIT_COUNT > 0))
                                {
                                    e.RepositoryItem = repositoryItemBtnViewBillDetail;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnViewBillDetailDisable;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CancelTransaction")
                        {
                            if ((data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && !string.IsNullOrEmpty(data.INVOICE_CODE)) && HisConfigCFG.Cancel_Option == "1")
                            {
                                e.RepositoryItem = (data.IS_CANCEL != 1 && controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000040")) ? repositoryItemBtnCancelTran : repositoryItemBtnCancelTranDisable;
                            }
                            else
                            {
                                var isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                                var HisPermission = BackendDataWorker.Get<HIS_PERMISSION>();
                                var isPremission = new HIS_PERMISSION();
                                if (HisPermission != null)
                                {
                                    isPremission = BackendDataWorker.Get<HIS_PERMISSION>().Where(o => o.LOGINNAME == this.loginName && o.EFFECTIVE_DATE == data.TRANSACTION_DATE).FirstOrDefault();
                                }
                                else
                                {
                                    isPremission = null;
                                }
                                if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "2")
                                {
                                    if (data.IS_ACTIVE == 1
                                    && data.IS_CANCEL != 1 && !data.DEBT_BILL_ID.HasValue
                                    && ((data.CASHIER_LOGINNAME == this.loginName && data.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000")) || isPremission != null || isAdmin == true))
                                    {
                                        e.RepositoryItem = repositoryItemBtnCancelTran;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemBtnCancelTranDisable;
                                    }

                                }
                                else if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "0" || HisConfigCFG.ALLOW_OTHER_LOGINNAME == "1")
                                {
                                    if (data.IS_ACTIVE == 1
                                    && data.IS_CANCEL != 1
                                    && (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "1" || data.CASHIER_LOGINNAME == this.loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                                    && (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU || (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO && !data.DEBT_BILL_ID.HasValue)))
                                    {
                                        e.RepositoryItem = repositoryItemBtnCancelTran;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemBtnCancelTranDisable;
                                    }
                                }
                                else if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "3" && data.IS_CANCEL != 1)
                                {
                                    e.RepositoryItem = (controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000043")) ? repositoryItemBtnCancelTran : repositoryItemBtnCancelTranDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnCancelTranDisable;
                                }
                            }

                            if (HisConfigCFG.AllowWhenRequest == "1" && data.CANCEL_REQ_STT != 1)
                            {
                                e.RepositoryItem = repositoryItemBtnCancelTranDisable;
                            }

                        }
                        else if (e.Column.FieldName == "ChangeLock")
                        {
                            if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                            {
                                if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    e.RepositoryItem = repositoryItemBtnLock;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnUnLock;
                                }
                            }
                            else
                            {
                                if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    e.RepositoryItem = repositoryItemBtnLockDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnUnlockDisable;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "Restore")
                        {
                            //if (data.IS_CANCEL == 1 && data.TRANSACTION_TYPE_ID == 0 && data.IS_DEBT_COLLECTION == 0 && data.SALE_TYPE_ID == 0 && (controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == ControlCode.BtnRestore)))
                            //{
                            //    e.RepositoryItem = repositoryItemBtnRestore;
                            //}
                            var isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                            var HisPermission = BackendDataWorker.Get<HIS_PERMISSION>();
                            var isPremission = new HIS_PERMISSION();
                            if (HisPermission != null)
                            {
                                isPremission = BackendDataWorker.Get<HIS_PERMISSION>().Where(o => o.LOGINNAME == this.loginName && o.EFFECTIVE_DATE == data.TRANSACTION_DATE).FirstOrDefault();
                            }
                            else
                            {
                                isPremission = null;
                            }
                            if (HisConfigCFG.UNCANCEL_OPTION == "2")
                            {
                                if (data.IS_CANCEL == 1 && data.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO
                                    && data.IS_DEBT_COLLECTION != 1 && data.SALE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_VACCIN
                                    && ((data.CASHIER_LOGINNAME == this.loginName && data.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000")) || isPremission != null || isAdmin == true))
                                {
                                    e.RepositoryItem = repositoryItemBtnRestore;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnRestoreDisable;
                                }
                            }
                            else if (HisConfigCFG.UNCANCEL_OPTION == "1")
                            {
                                if (data.IS_CANCEL == 1 && data.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO && data.IS_DEBT_COLLECTION != 1 && data.SALE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_VACCIN && controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == ControlCode.BtnRestore))
                                {
                                    e.RepositoryItem = repositoryItemBtnRestore;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnRestoreDisable;

                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnRestoreDisable;
                            }

                        }


                        //else if (e.Column.FieldName == "EDIT_INFO_DISPLAY")
                        //{
                        //    if (data.IS_ACTIVE == 1
                        //        && data.IS_CANCEL != 1
                        //        && (data.CASHIER_LOGINNAME == this.loginName
                        //        || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                        //        && (controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000017")))
                        //    {
                        //        e.RepositoryItem = repositoryItemButtonEdit__E;
                        //    }
                        //    else
                        //        e.RepositoryItem = repositoryItemButtonEdit__D;
                        //}
                        //else if (e.Column.FieldName == "EDIT_INFO_DISPLAY")
                        //{
                        //    bool isActive = data.IS_ACTIVE == 1;
                        //    bool isPremission = false;
                        //    bool isNotCancelled = data.IS_CANCEL != 1;
                        //    bool hasControlPermission = controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000017");
                        //    bool isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                        //    bool isSameDay = data.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                        //    bool isCashierOrAdmin = data.CASHIER_LOGINNAME == this.loginName || isAdmin;

                        //    if (isActive && isNotCancelled && hasControlPermission)
                        //    {
                        //        if ((isSameDay && isCashierOrAdmin) || (!isSameDay && isPremission))
                        //        {
                        //            e.RepositoryItem = repositoryItemButtonEdit__E;
                        //        }
                        //        else
                        //        {
                        //            e.RepositoryItem = repositoryItemButtonEdit__D;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        e.RepositoryItem = repositoryItemButtonEdit__D;
                        //    }
                        //}
                        else if (e.Column.FieldName == "EDIT_INFO_DISPLAY")
                        {
                            bool isActive = data.IS_ACTIVE == 1;
                            bool isNotCancelled = data.IS_CANCEL != 1;
                            bool hasControlPermission = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000017") != null;
                            bool isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                            bool isSameDay = data.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                            bool isCashierOrAdmin = data.CASHIER_LOGINNAME == this.loginName || isAdmin;

                            bool isPermission = GetHisPermission();
                           

                            if (isActive && isNotCancelled && hasControlPermission)
                            {
                                if ((isSameDay && isCashierOrAdmin) || (!isSameDay && isPermission))
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit__E;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit__D;
                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__D;
                            }
                        }


                        else if (e.Column.FieldName == "NUM_ORDER")
                        {
                            if (HisConfigCFG.ALLOW_EDIT_NUM_ORDER == "1"
                                && (!data.IS_CANCEL.HasValue || data.IS_CANCEL != 1)
                                && data.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                            {
                                e.RepositoryItem = repositoryItemSpinNumOrder__E;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemTextNumOrder__D;
                            }
                        }
                        else if (e.Column.FieldName == "DeleteTransaction")
                        {
                            if (this.controlDelete != null)
                            {
                                if (this.controlDelete.IS_ANONYMOUS == 1 && data.IS_ACTIVE == 1)
                                {
                                    e.RepositoryItem = btnDELETE;
                                }
                                else
                                {
                                    if (RoleUse != null && ControlRule != null && RoleUse.Count > 0 && ControlRule.Count > 0)
                                    {
                                        var CheckRole = this.RoleUse.Where(o => this.ControlRule.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                                        if ((CheckRole != null && CheckRole.Count > 0 && data.IS_ACTIVE == 1) || data.CREATOR == this.loginName)
                                        {
                                            e.RepositoryItem = btnDELETE;
                                        }
                                        else
                                        {
                                            e.RepositoryItem = btnDisableDelete;
                                        }
                                    }
                                    else
                                        e.RepositoryItem = btnDisableDelete;
                                }
                            }

                        }
                        else if (e.Column.FieldName == "HDDT")
                        {
                            if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && !String.IsNullOrWhiteSpace(data.INVOICE_CODE))
                            {

                                e.RepositoryItem = repositoryItemEnbtnHDDT;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemDisbtnHDDT;
                            }

                        }
                        else if (e.Column.FieldName == "UnrejectCancellation")
                        {
                            if (data.CANCEL_REQ_STT == 2 && this.loginName == data.CANCEL_REQ_REJECT_LOGINNAME && data.IS_CANCEL != 1)
                            {

                                e.RepositoryItem = repositoryItemUnrejectCancellation;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemUnrejectCancellationDis;
                            }

                        }
                        else if (e.Column.FieldName == "QR")
                        {
                            if ((data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU) && data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                                e.RepositoryItem = repQr;
                        }
                        else if (e.Column.FieldName == "ChangePayForm")
                        {
                            if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && string.IsNullOrEmpty(data.BANK_TRANSACTION_CODE))
                                e.RepositoryItem = repositoryItemBtnChangePay;
                        }
                    }
                }
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
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gridViewTransaction_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (HisTransactionADO)gridViewTransaction.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            if (e.Column.FieldName == "STT" || e.Column.FieldName == "CancelTransaction" || e.Column.FieldName == "ChangeLock")
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                this.transactionPrint = null;
                if (hi.InRowCell)
                {
                    transactionPrint = new V_HIS_TRANSACTION();
                    this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                    listData = new List<V_HIS_TRANSACTION>();
                    listData.Add(transactionPrint);
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (this.transactionPrint != null)
                    {
                        this.popupMenuProcessor = new PopupMenuProcessor(this.transactionPrint, this.baManager, MouseRightItemClick, currentModule);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
                 GetHisPermissionLoad();
                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.treatment = null;
                this.accountBook = null;
                SetDefaultValueControl();
                FillDataToGrid();
                txtTreatmentCodeFind.Focus();
                txtTreatmentCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFocusTreatmentCode_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCodeFind.Focus();
                txtTreatmentCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFocusAccountBookCode_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtAccountBookCodeFind.Focus();
                txtAccountBookCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnViewBillDetail_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data != null && data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                {
                    //Bỏ Bill
                    //HisBillFilter billFilter = new HisBillFilter();
                    //billFilter.TRANSACTION_ID = data.ID;
                    //var listBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL>>("api/HisBill/Get", ApiConsumers.MosConsumer, billFilter, null);
                    if (data == null
                        //|| listBill.Count != 1
                        )
                    {
                        throw new NullReferenceException("Khong lay duoc bill theo transactionId:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    }
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillDetail").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBillDetail'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        listArgs.Add(data.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                    }
                }
                if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && data.TDL_SERE_SERV_DEPOSIT_COUNT > 0)
                {
                    if (data == null
                        //|| listBill.Count != 1
                        )
                    {
                        throw new NullReferenceException("Khong lay duoc bill theo transactionId:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    }
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDepositDetail").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDepositDetail'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        listArgs.Add(data.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnCancelTran_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy giao dich null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    return;
                }
                if (treatment != null && treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && (!data.SALE_TYPE_ID.HasValue || data.SALE_TYPE_ID != 2))
                {
                    MessageManager.Show(Base.ResourceMessageLang.XuLyThatBai + Base.ResourceMessageLang.HoSoDieuTriDaDuocKhoaTaiChinh);
                    return;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionCancel").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionCancel'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data);
                    listArgs.Add(this.currentModule);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    if (isOpenFromHisTransaction)
                    {
                        txtTreatmentCodeFind.Text = data.TDL_TREATMENT_CODE;
                        FillDataToGridTransactionForOneCode(new CommonParam(start, limit));
                    }
                    else
                    {
                        FillDataToGridTransaction(new CommonParam(start, limit));
                    }

                    this.RefreshSessionInfo();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
                //}
                //else if (data.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__DEPOSIT))
                //{
                //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDepositCancel").FirstOrDefault();
                //    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDepositCancel'");
                //    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //    {
                //        List<object> listArgs = new List<object>();
                //        listArgs.Add(data);
                //        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                //        if (extenceInstance == null)
                //        {
                //            throw new ArgumentNullException("extenceInstance is null");
                //        }

                //        ((Form)extenceInstance).ShowDialog();
                //        FillDataToGridTransaction(new CommonParam(start, limit));
                //    }
                //    else
                //    {
                //        MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private bool CancelElectronicBill(V_HIS_TRANSACTION transaction)
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (transaction != null && !String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS))
        //        {
        //            string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.ELECTRONIC_BILL__CONFIG);
        //            if (transaction.INVOICE_SYS != ProviderType.VIETTEL)
        //            {
        //                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
        //                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);
        //                dataInput.InvoiceCode = transaction.INVOICE_CODE;
        //                dataInput.NumOrder = transaction.NUM_ORDER;
        //                dataInput.SymbolCode = transaction.SYMBOL_CODE;
        //                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
        //                dataInput.TransactionTime = transaction.TRANSACTION_TIME;
        //                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
        //                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
        //                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);

        //                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillResult), electronicBillResult));
        //                if (electronicBillResult != null && !electronicBillResult.Success)
        //                {

        //                    string mes = "";
        //                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
        //                    {
        //                        foreach (var item in electronicBillResult.Messages)
        //                        {
        //                            mes += item + ";";
        //                        }
        //                    }

        //                    DialogResult myResult;
        //                    myResult = MessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Bạn có muốn tiếp tục hủy giao dịch trên HIS?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        //                    if (myResult != DialogResult.OK)
        //                    {
        //                        result = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private void repositoryItemBtnLock_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data != null && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    if (XtraMessageBox.Show(String.Format("Hệ thống sẽ tự động cập nhật lại thông tin tài khoản thu ngân, phòng thu ngân của giao dịch theo tài khoản và phòng thu ngân trước khi mở khóa. Trong khi giao dịch bị khóa, bệnh nhân sẽ tạm thời không được ghi nhận số tiền thuộc giao dịch tạm ứng (thanh toán) này. Bạn có muốn khóa giao dịch {0} không?", data.TRANSACTION_CODE),
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        ProcessChangeLock(data, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUnLock_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data != null && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    if (XtraMessageBox.Show(String.Format("Hệ thống sẽ tự động cập nhật lại thông tin tài khoản thu ngân, phòng thu ngân của giao dịch theo tài khoản của bạn và phòng bạn đang làm việc. Bạn có muốn mở khóa giao dịch {0} không?", data.TRANSACTION_CODE),
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                         ProcessChangeLock(data, false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChangeLock(V_HIS_TRANSACTION data, bool IsLock)
        {
            try
            {
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    V_HIS_TRANSACTION rs = null;
                    TransactionLockSDO sdoLock = new TransactionLockSDO();
                    sdoLock.TransactionId = data.ID;
                    sdoLock.RequestRoomId = this.currentModule.RoomId;

                    if (IsLock)
                    {

                        if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/DepositLock", ApiConsumers.MosConsumer, data.ID, param);
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/BillLock", ApiConsumers.MosConsumer, data.ID, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/Lock", ApiConsumers.MosConsumer, data.ID, param);
                        }

                    }
                    else
                    {
                        if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/DepositUnlock", ApiConsumers.MosConsumer, sdoLock, param);
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/BillUnlock", ApiConsumers.MosConsumer, sdoLock, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/Unlock", ApiConsumers.MosConsumer, data.ID, param);
                        }

                    }
                    if (rs != null)
                    {
                        success = true;
                        data.IS_ACTIVE = rs.IS_ACTIVE;
                        data.MODIFY_TIME = rs.MODIFY_TIME;
                        data.MODIFIER = rs.MODIFIER;
                        FillDataToGridTransaction(new CommonParam(start, limit));
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                this.Text = Inventec.Common.Resource.Get.Value("frmTransactionList.Text", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());

                //Button
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__BTN_FIND", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__BTN_REFRESH", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                // this.layoutTransactionTypeBill.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__LAYOUT_CHECK__TRANSACTION_TYPE__BILL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                // this.layoutTransactionTypeDeposit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__LAYOUT_CHECK__TRANSACTION_TYPE__DEPOSIT", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                // this.layoutTransactionTypeRepay.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__LAYOUT_CHECK__TRANSACTION_TYPE__REPAY", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl transaction
                this.gridColumn_Transaction_AccountBookCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_ACCOUNT_BOOK_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_ACCOUNT_BOOK_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_CancelTransaction.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_CANCEL_TRANSACTION", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_NationalTransactionCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_NATIONAL_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Cashier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_CASHIER", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierRoomName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_CASHIER_ROOM_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_CREATOR", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Dob.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_DOB", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_GenderName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_GENDER_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_MODIFIER", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_MODIFY_TIME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_NumOrder.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_NUM_ORDER", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_PatientCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_PATIENT_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_PayFormName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_PAY_FORM_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_STT", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TRANSACTION_TYPE_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TREATMENT_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_VIR_PATIENT_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_Tig_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TIG_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransReqCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_TRANS_REQ_CODE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.gridColumn_TRANSACTION_BANK_NAME.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GRID_TRANSACTION__COLUMN_BANK_NAME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Nav Bar Control
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAV_BAR__GROUP_CREATE_TIME", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupTransactionType.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAV_BAR__GROUP_TRANSACTION_TYPE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.navBarGroupBillType.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.navBarGroupBillType.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());


                this.navBarGroupAccountBookInfo.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.navBarGroupAccountBookInfo.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());


                //NUllValue
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_KEY_WORD_NULL_VALUE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemBtnViewBillDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_VIEW_BILL_DETAIL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemBtnViewBillDetailDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_VIEW_BILL_DETAIL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_KEY_WORD_NULL_VALUE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtTreatmentCodeFind.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_TREATMENT_CODE_NULL_VALUE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtAccountBookCodeFind.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_ACCOUNT_BOOK_CODE_NULL_VALUE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.txtAccountBookSymbol.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTransactionList.txtAccountBookSymbol.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.txtAccountBookTemp.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTransactionList.txtAccountBookTemp.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.txtAccountBookName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTransactionList.txtAccountBookName.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());

                this.chkElectronicAll.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__CHECK_ELECTRONIC_ALL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkElectronicDone.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__CHECK_ELECTRONIC_DONE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkElectronicNone.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__CHECK_ELECTRONIC_NONE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barElectronic.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__BAR_ELECTRONIC", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.lcProcessed.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_DA_XU_LY", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.lcErrorProcessed.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_XU_LY_LOI", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnExportBill.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__BTN_EXPORT_BILL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.navTransactionStatus.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cbStatusAll.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_ALL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cbStatusCancelled.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_CANCELLED", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cbStatusNotCancel.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_NOT_CANCEL", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.cbStatusAll1.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_ALL1", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cbStatusNoLook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_NO_LOOK", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cbStatusLook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_STATUS_LOOK", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //minhnq
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn8.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn8.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn6.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn5.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn4.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn3.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn2.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn9.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gc_SwipeAmount.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gc_SwipeAmount.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gc_SwipeAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionList.gc_SwipeAmount.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gc_TransferAmuont.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gc_TransferAmuont.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gc_TransferAmuont.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionList.gc_TransferAmuont.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_DerectlyBilling.Caption = Inventec.Common.Resource.Get.Value("frmTransactionList.gridColumn_Transaction_DerectlyBilling.Caption", Base.ResourceLangManager.LanguageFrmTransactionList, LanguageManager.GetCulture());


                if (HisConfigCFG.AllowWhenRequest == "1")
                {
                    this.repositoryItemBtnCancelTran.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_REQ_CANCEL_TRANSACTION", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.repositoryItemBtnCancelTranDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_REQ_CANCEL_TRANSACTION", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else
                {
                    this.repositoryItemBtnCancelTran.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_CANCEL_TRANSACTION", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.repositoryItemBtnCancelTranDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__REPOSITORY__BTN_CANCEL_TRANSACTION", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit__E_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {

                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionInfoEdit").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TransactionInfoEdit");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefeshDataBefoEdit);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshDataBefoEdit()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "NUM_ORDER") return;
                var data = (HisTransactionADO)gridViewTransaction.GetRow(e.RowHandle);
                if (data != null
                    && HisConfigCFG.ALLOW_EDIT_NUM_ORDER == "1"
                    && (!data.IS_CANCEL.HasValue || data.IS_CANCEL != 1)
                    && data.IS_NOT_GEN_TRANSACTION_ORDER == 1
                    && data.OLD_NUM_ORDER != data.NUM_ORDER)
                {
                    WaitingManager.Show();
                    Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                    HIS_TRANSACTION row = Mapper.Map<HIS_TRANSACTION>(data);
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/UpdateNumOrder", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        data.OLD_NUM_ORDER = rs.NUM_ORDER;
                        data.NUM_ORDER = rs.NUM_ORDER;
                    }
                    else
                    {
                        data.NUM_ORDER = data.OLD_NUM_ORDER;
                    }
                    gridControlTransaction.RefreshDataSource();
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnRestore_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data != null && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    ProcessRestore(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessRestore(V_HIS_TRANSACTION data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                //Review
                String Message = "";

                if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    Message = ResourceMessage.KhongChoPhepHoanHuyChiDuocHoanNoiBo;
                }
                else
                {
                    Message = "Bạn có muốn khôi phục dữ liệu không?";
                }

                if (MessageBox.Show(Message, "THÔNG BÁO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    MOS.SDO.HisTransactionUncancelSDO sdo = new MOS.SDO.HisTransactionUncancelSDO();
                    sdo.TransactionId = data.ID;
                    sdo.RequestRoomId = this.currentModule.RoomId;
                    var transactionListUncancel = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/Uncancel", ApiConsumers.MosConsumer, sdo, param);
                    if (transactionListUncancel != null)
                    {
                        success = true;
                        btnFind_Click(null, null);
                        this.RefreshSessionInfo();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDELETE_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                if (data != null)
                {
                    if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        MessageBox.Show(ResourceMessage.GiaoDichMotTheKhongChoPhepXoa);
                        return;
                    }

                    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        MOS.SDO.HisTransactionDeleteSDO dataupdate = new MOS.SDO.HisTransactionDeleteSDO();
                        dataupdate.TransactionId = data.ID;
                        dataupdate.RequestRoomId = this.currentModule.RoomId;
                        var Result = new BackendAdapter(param).Post<bool>(RequestUri.HIS_TRANSACTION_DELETE, ApiConsumers.MosConsumer, dataupdate, param);
                        if (Result)
                        {
                            FillDataToGrid();
                            BackendDataWorker.Reset<HIS_TRANSACTION>();
                            btnFind_Click(null, null);
                            this.RefreshSessionInfo();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, Result);
                    }

                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshSessionInfo()
        {
            try
            {
                LogSystem.Debug("GlobalVariables.RefreshSessionModule: " + (GlobalVariables.RefreshSessionModule != null).ToString());
                if (GlobalVariables.RefreshSessionModule != null)
                {
                    GlobalVariables.RefreshSessionModule();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkElectronicAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkElectronicAll.Checked)
                {
                    chkElectronicDone.Checked = false;
                    chkElectronicNone.Checked = false;
                }
                else if (!chkElectronicDone.Checked && !chkElectronicNone.Checked)
                {
                    chkElectronicAll.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkElectronicDone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkElectronicDone.Checked)
                {
                    chkElectronicAll.Checked = false;
                    chkElectronicNone.Checked = false;
                }
                else if (!chkElectronicAll.Checked && !chkElectronicNone.Checked)
                {
                    chkElectronicDone.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkElectronicNone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkElectronicNone.Checked)
                {
                    chkElectronicDone.Checked = false;
                    chkElectronicAll.Checked = false;
                }
                else if (!chkElectronicDone.Checked && !chkElectronicAll.Checked)
                {
                    chkElectronicNone.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboFilter_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhilecboFilterStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboFilter.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (cboFilter.EditValue.ToString() ?? "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboFilter.Name;
                    csAddOrUpdate.VALUE = (cboFilter.EditValue.ToString() ?? "");
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

        private void btnExportBill_Click(object sender, EventArgs e)
        {
            try
            {
                strTransactionError = "";
                List<V_HIS_TRANSACTION> listTransaction = new List<V_HIS_TRANSACTION>();
                var rowHandles = gridViewTransaction.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TRANSACTION)gridViewTransaction.GetRow(i);
                        if (row != null)
                        {
                            listTransaction.Add(row);
                        }
                    }
                    var tranLock = listTransaction.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE).ToList();
                    if (tranLock != null && tranLock.Count > 0 && (MessageBox.Show(string.Format("Các giao dịch {0} đang tạm khóa bạn có muốn tiếp tục?", string.Join(",",tranLock.Select(o => o.TRANSACTION_CODE).ToList())), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No))
                    {
                        return;
                    }
                    if (listTransaction.All(o => o.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn giao dịch thanh toán", "Thông báo");
                        return;
                    }

                    else if (listTransaction.Any(o => o.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ hỗ trợ xuất hóa đơn điện tử với giao dịch thanh toán", "Thông báo");
                        return;
                    }
                    else
                    {
                        if (HisConfigCFG.ElectronicBillExportOption == "1")
                        {
                            var listTransactionOther = listTransaction.Where(o => o.CASHIER_LOGINNAME != this.loginName).ToList();
                            int amountTransactionOther = listTransactionOther != null ? listTransactionOther.Count : 0;
                            if (amountTransactionOther > 0)
                            {
                                if (amountTransactionOther < listTransaction.Count)
                                {
                                    if (XtraMessageBox.Show(String.Format("Có {0} giao dịch có mã giao dịch {1} bạn không phải là người tạo nên sẽ không xuất được {2} giao dịch này. Bạn có muốn tiếp tục xuất các giao dịch khác không?", amountTransactionOther, String.Join(", ", listTransactionOther.Select(o => o.TRANSACTION_CODE).ToList()), amountTransactionOther),
                            "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                        return;
                                    else
                                        listTransaction = listTransaction.Where(o => o.CASHIER_LOGINNAME == this.loginName).ToList();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Có {0} giao dịch có mã giao dịch {1} bạn không phải là người tạo nên sẽ không xuất được {2} giao dịch này.", amountTransactionOther, String.Join(", ", listTransactionOther.Select(o => o.TRANSACTION_CODE).ToList()), amountTransactionOther), "Thông báo");
                                    return;
                                }
                            }
                        }
                        var exportedBill = listTransaction.Where(o => o.INVOICE_CODE != null).Select(b => b.TRANSACTION_CODE).ToList();
                        var cancelledBill = listTransaction.Where(o => o.IS_CANCEL == 1).Select(b => b.TRANSACTION_CODE).ToList();
                        if (exportedBill != null && exportedBill.Count > 0)
                        {
                            string messageShow = "Giao dịch ";
                            foreach (var billCode in exportedBill)
                            {
                                messageShow += billCode;
                                if (!(billCode == exportedBill.Last()))
                                {
                                    messageShow += ", ";
                                }
                            }
                            messageShow += " đã được xuất hóa đơn điện tử";
                            DevExpress.XtraEditors.XtraMessageBox.Show(messageShow, "Thông báo");
                            return;
                        }
                        else if (cancelledBill != null && cancelledBill.Count > 0)
                        {
                            string messageShow = "Giao dịch ";
                            foreach (var billCode in cancelledBill)
                            {
                                messageShow += billCode;
                                if (!(billCode == cancelledBill.Last()))
                                {
                                    messageShow += ", ";
                                }
                            }
                            messageShow += " đã bị hủy";
                            DevExpress.XtraEditors.XtraMessageBox.Show(messageShow, "Thông báo");
                            return;
                        }
                        else
                        {
                            WaitingManager.Show();
                            this.lcProcessed.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_DA_XU_LY", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            this.lcErrorProcessed.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__TXT_XU_LY_LOI", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            lciWarning.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                            this.txtProcessed.Text = "0" + "/" + listTransaction.Count.ToString();
                            this.txtErrorProcessed.Text = "0";
                            int processedDoneCount = 0;
                            int processedErrorCount = 0;

                            List<long> listTransactionVnptError = new List<long>();
                            for (int i = 0; i < listTransaction.Count; i++)
                            {
                                CommonParam param = new CommonParam();
                                if (!this.XuatHoaDonDienTu(listTransaction[i], listTransactionVnptError.Exists(o => o == listTransaction[i].ID), ref param))
                                {
                                    bool setError = true;

                                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");
                                    if (listTransaction[i].EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT || (!listTransaction[i].EINVOICE_TYPE_ID.HasValue && serviceConfig.Contains(ProviderType.VNPT)))
                                    {
                                        List<long> ids = listTransactionVnptError.Where(o => o == listTransaction[i].ID).ToList();
                                        if (ids.Count <= 2)
                                        {
                                            setError = false;
                                            //add thêm vào danh sách cũ để tiếp tục lặp và không ảnh hưởng đến vòng lặp như foreach
                                            listTransaction.Add(listTransaction[i]);
                                            listTransactionVnptError.Add(listTransaction[i].ID);
                                        }
                                    }

                                    if (setError)
                                    {
                                        string strApiResult = param.GetMessage();
                                        strTransactionError += listTransaction[i].TRANSACTION_CODE + " - " + strApiResult + System.Environment.NewLine;
                                        processedErrorCount += 1;
                                        this.txtErrorProcessed.Text = processedErrorCount.ToString();
                                    }
                                }
                                else
                                {
                                    processedDoneCount += 1;
                                    this.txtProcessed.Text = processedDoneCount.ToString() + "/" + listTransaction.Count.ToString();
                                }
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this, new CommonParam(), true);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewTransaction.GetSelectedRows().Count() > 0)
                {
                    btnExportBill.Enabled = true;
                }
                else
                {
                    btnExportBill.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbStatusAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusAll.Checked)
                {
                    cbStatusCancelled.Checked = false;
                    cbStatusNotCancel.Checked = false;
                }
                else if (!cbStatusCancelled.Checked && !cbStatusNotCancel.Checked)
                {
                    cbStatusAll.Checked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cbStatusCancelled_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusCancelled.Checked)
                {
                    cbStatusAll.Checked = false;
                    cbStatusNotCancel.Checked = false;
                }
                else if (!cbStatusAll.Checked && !cbStatusNotCancel.Checked)
                {
                    cbStatusCancelled.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbStatusNotCancel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusNotCancel.Checked)
                {
                    cbStatusAll.Checked = false;
                    cbStatusCancelled.Checked = false;
                }
                else if (!cbStatusAll.Checked && !cbStatusCancelled.Checked)
                {
                    cbStatusNotCancel.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtProcessed_Click(object sender, EventArgs e)
        {


        }

        private void txtErrorProcessed_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    e.IsValid = true;
            //    e.ErrorType = ErrorType.Warning;
            //}
            //catch (Exception ex)
            //{
            //    LogSystem.Warn(ex);
            //}
        }

        private void btnWarning_Click(object sender, EventArgs e)
        {
            try
            {
                ExportErrorProcessed frmErrorForm = new ExportErrorProcessed(this.strTransactionError);
                frmErrorForm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
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

        private void gridViewTransaction_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var transactionData = (V_HIS_TRANSACTION)view.GetRow(hi.RowHandle);
                        //string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        //short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(hi.RowHandle, "IS_PAUSE") ?? "").ToString());
                        //string departmentIds = (view.GetRowCellValue(hi.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                        //bool AssignService = false;
                        //bool isfinishButton = false;
                        if (transactionData != null)
                        {
                            if (hi.Column.FieldName == "BILL_DETAIL")
                            {
                                if (transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || (transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && transactionData.TDL_SERE_SERV_DEPOSIT_COUNT > 0))
                                {
                                    repositoryItemBtnViewBillDetail_ButtonClick(transactionData);
                                }
                            }
                            else if (hi.Column.FieldName == "CancelTransaction")
                            {
                                if (HisConfigCFG.AllowWhenRequest == "1" && transactionData.CANCEL_REQ_STT != 1)
                                    return;

                                if ((transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && !string.IsNullOrEmpty(transactionData.INVOICE_CODE)) && HisConfigCFG.Cancel_Option == "1")
                                {
                                    if (transactionData.IS_CANCEL != 1 && controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000040"))
                                    {
                                        repositoryItemBtnCancelTran_ButtonClick(transactionData);
                                    }
                                }
                                else
                                {
                                    var isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);

                                    HIS_PERMISSION isPremission = null;

                                    try
                                    {
                                        var HisPermissionList = GetHisPermissionLoad();
                                        isPremission = HisPermissionList
                                            .FirstOrDefault(o => o.EFFECTIVE_DATE == transactionData.TRANSACTION_DATE);
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("result_____ttzz:", isPremission));
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Error loading HIS_PERMISSION from API: " + ex.ToString());
                                    }


                                    //var HisPermission = BackendDataWorker.Get<HIS_PERMISSION>();
                                    //var isPremission = new HIS_PERMISSION();
                                    //if (HisPermission != null)
                                    //{
                                    //    isPremission = BackendDataWorker.Get<HIS_PERMISSION>().Where(o => o.LOGINNAME == this.loginName && o.EFFECTIVE_DATE == transactionData.TRANSACTION_DATE).FirstOrDefault();
                                    //}
                                    //else
                                    //{
                                    //    isPremission = null;
                                    //}


                                    if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "2")
                                    {
                                        if (transactionData.IS_ACTIVE == 1
                                        && transactionData.IS_CANCEL != 1 && !transactionData.DEBT_BILL_ID.HasValue
                                        && ((transactionData.CASHIER_LOGINNAME == this.loginName && transactionData.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000")) || isPremission != null || isAdmin == true))
                                        {
                                            repositoryItemBtnCancelTran_ButtonClick(transactionData);
                                        }
                                    }
                                    else if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "0" || HisConfigCFG.ALLOW_OTHER_LOGINNAME == "1")
                                    {
                                        if (transactionData.IS_ACTIVE == 1
                                        && transactionData.IS_CANCEL != 1
                                        && (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "1" || transactionData.CASHIER_LOGINNAME == this.loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                                        && (transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU || transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU || (transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO && !transactionData.DEBT_BILL_ID.HasValue)))
                                        {
                                            repositoryItemBtnCancelTran_ButtonClick(transactionData);
                                        }
                                    }
                                    else if (HisConfigCFG.ALLOW_OTHER_LOGINNAME == "3" && transactionData.IS_CANCEL != 1 && controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000043"))
                                    {
                                        repositoryItemBtnCancelTran_ButtonClick(transactionData);
                                    }
                                }

                            }
                            else if (hi.Column.FieldName == "ChangeLock")
                            {
                                if ((transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU || transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT))
                                {
                                    if (transactionData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        repositoryItemBtnLock_ButtonClick(transactionData);
                                    }
                                    else
                                    {
                                        repositoryItemBtnUnLock_ButtonClick(transactionData);
                                    }
                                }
                            }
                            else if (hi.Column.FieldName == "Restore")
                            {
                                var isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);

                                HIS_PERMISSION isPremission = null;

                                try
                                {
                                    var HisPermissionList = GetHisPermissionLoad();
                                    isPremission = HisPermissionList
                                        .FirstOrDefault(o => o.EFFECTIVE_DATE == transactionData.TRANSACTION_DATE);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Error loading HIS_PERMISSION from API in Restore: " + ex.ToString());
                                }
                                    //var HisPermission = BackendDataWorker.Get<HIS_PERMISSION>();
                                    //var isPremission = new HIS_PERMISSION();
                                    //if (HisPermission != null)
                                    //{
                                    //    isPremission = BackendDataWorker.Get<HIS_PERMISSION>().Where(o => o.LOGINNAME == this.loginName && o.EFFECTIVE_DATE == transactionData.TRANSACTION_DATE).FirstOrDefault();
                                    //}
                                    //else
                                    //{
                                    //    isPremission = null;
                                    //}
                                    if (HisConfigCFG.UNCANCEL_OPTION == "2")
                                {
                                    if (transactionData.IS_CANCEL == 1 && transactionData.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO
                                        && transactionData.IS_DEBT_COLLECTION != 1 && transactionData.SALE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_VACCIN
                                        && ((transactionData.CASHIER_LOGINNAME == this.loginName && transactionData.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000")) || isPremission != null || isAdmin == true))
                                    {
                                        repositoryItemBtnRestore_ButtonClick(transactionData);
                                    }
                                }
                                else if (HisConfigCFG.UNCANCEL_OPTION == "1")
                                {
                                    if (transactionData.IS_CANCEL == 1 && transactionData.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO && transactionData.IS_DEBT_COLLECTION != 1 && transactionData.SALE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_VACCIN && controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == ControlCode.BtnRestore))
                                    {
                                        repositoryItemBtnRestore_ButtonClick(transactionData);
                                    }
                                }
                            }
                           
                            else if (hi.Column.FieldName == "EDIT_INFO_DISPLAY")
                            {
                                bool isActive = transactionData.IS_ACTIVE == 1;
                                bool isNotCancelled = transactionData.IS_CANCEL != 1;
                                //bool hasControlPermission = controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000017");
                                bool hasControlPermission =  controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000017") != null;
                                bool isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName);
                                bool isCashierOrAdmin = transactionData.CASHIER_LOGINNAME == this.loginName || isAdmin;

                                bool isSameDay = transactionData.TRANSACTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                                bool isPermission = GetHisPermission();

                                if (isActive && isNotCancelled && hasControlPermission)
                                {
                                    if ((isSameDay && isCashierOrAdmin) || (!isSameDay && isPermission))
                                    {
                                        repositoryItemButtonEdit__E_ButtonClick(transactionData);
                                    }
                                }
                            }

                            //else if (hi.Column.FieldName == "NUM_ORDER")
                            //{
                            //    if (HisConfigCFG.ALLOW_EDIT_NUM_ORDER == "1"
                            //    && (!transactionData.IS_CANCEL.HasValue || transactionData.IS_CANCEL != 1)
                            //    && transactionData.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                            //    {
                            //        e.RepositoryItem = repositoryItemSpinNumOrder__E;
                            //    }
                            //    else
                            //    {
                            //        e.RepositoryItem = repositoryItemTextNumOrder__D;
                            //    }
                            //}
                            else if (hi.Column.FieldName == "DeleteTransaction")
                            {
                                if (this.controlDelete != null)
                                {
                                    if (this.controlDelete.IS_ANONYMOUS == 1 && transactionData.IS_ACTIVE == 1)
                                    {
                                        btnDELETE_ButtonClick(transactionData);
                                    }
                                    else
                                    {
                                        if (RoleUse != null && ControlRule != null && RoleUse.Count > 0 && ControlRule.Count > 0)
                                        {
                                            var CheckRole = this.RoleUse.Where(o => this.ControlRule.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                                            if ((CheckRole != null && CheckRole.Count > 0 && transactionData.IS_ACTIVE == 1) || transactionData.CREATOR == this.loginName)
                                            {
                                                btnDELETE_ButtonClick(transactionData);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (hi.Column.FieldName == "HDDT")
                            {
                                this.transactionPrint = transactionData;
                                listData = new List<V_HIS_TRANSACTION>();
                                listData.Add(transactionPrint);
                                this.InHoaDonDienTu();
                            }
                            else if (hi.Column.FieldName == "UnrejectCancellation")
                            {
                                repositoryItemUnrejectCancellation_ButtonClick(transactionData);
                            }
                            else if (hi.Column.FieldName == "QR")
                            {
                                if ((transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || transactionData.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU) && transactionData.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && transactionData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                                    repQr_ButtonClick(transactionData);
                            }
                            else if (hi.Column.FieldName == "ChangePayForm")
                            {
                                if (transactionData.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && transactionData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && string.IsNullOrEmpty(transactionData.BANK_TRANSACTION_CODE))
                                    btnChangePayForm_ButtonClick(transactionData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

      
        private void btnChangePayForm_ButtonClick(V_HIS_TRANSACTION currentTransaction)
        {
            try
            {
                List<V_HIS_TRANSACTION> listTransactionSelect = new List<V_HIS_TRANSACTION>();
                var rowHandles = gridViewTransaction.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TRANSACTION)gridViewTransaction.GetRow(i);
                        if (row != null)
                        {
                            listTransactionSelect.Add(row);
                        }
                    }
                }

                ChangePayForm frmErrorForm = new ChangePayForm(currentTransaction, currentModule.RoomId, loadPayForm, listTransactionSelect);
                frmErrorForm.ShowDialog();
            }
             catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadPayForm(string value)
        {
            try
            {
                FillDataToGrid();
                //frmTransactionList_Load(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void InitControlState()
        //{
        //    isNotLoadWhilecboFilterStateInFirst = true;
        //    try
        //    {
        //        this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
        //        this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
        //        if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
        //        {
        //            foreach (var item in this.currentControlStateRDO)
        //            {
        //                if (item.KEY == cboFilter.Name)
        //                {
        //                    cboFilter.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(item.VALUE);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    isNotLoadWhilecboFilterStateInFirst = false;
        //}

        //private void cboFilter_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (isNotLoadWhilecboFilterStateInFirst)
        //        {
        //            return;
        //        }
        //        WaitingManager.Show();
        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboFilter.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
        //        if (csAddOrUpdate != null)
        //        {
        //            csAddOrUpdate.VALUE = (cboFilter.EditValue.ToString() ?? "");
        //        }
        //        else
        //        {
        //            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdate.KEY = cboFilter.Name;
        //            csAddOrUpdate.VALUE = (cboFilter.EditValue.ToString() ?? "");
        //            csAddOrUpdate.MODULE_LINK = moduleLink;
        //            if (this.currentControlStateRDO == null)
        //                this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentControlStateRDO.Add(csAddOrUpdate);
        //        }
        //        this.controlStateWorker.SetData(this.currentControlStateRDO);
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        #region _InitCombo__________InitCheck
        private async Task InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__LoaiGiaoDich(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                this.ListLoaiGiaoDich = new List<HIS_TRANSACTION_TYPE>();
                //this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                foreach (HIS_TRANSACTION_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.TRANSACTION_TYPE_NAME);
                    this.ListLoaiGiaoDich.Add(rv);

                }
                grdLoaiGiaoDich.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__LoaiHoaDon(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                ListLoaiHoaDon = new List<LoaiHoaDon>();
                // ListLoaiHoaDon_.Clear();

                //this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                foreach (LoaiHoaDon rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.Name);
                    rv.Check = true;
                    ListLoaiHoaDon.Add(rv);

                }
                grdLoaiHoaDon.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoaihoadongList()
        {
            try
            {
                this.ListLoaiHoaDon_ = new List<LoaiHoaDon>();
                LoaiHoaDon hoadon0 = new LoaiHoaDon();
                hoadon0.ID = 1;
                hoadon0.Name = "HĐ thường";
                hoadon0.Check = false;
                this.ListLoaiHoaDon_.Add(hoadon0);

                LoaiHoaDon hoadon1 = new LoaiHoaDon();
                hoadon1.ID = 2;
                hoadon1.Name = "HĐ dịch vụ";
                hoadon1.Check = false;
                this.ListLoaiHoaDon_.Add(hoadon1);
            }
            catch
            {

            }

        }
        #endregion

        private void grdLoaiGiaoDich_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.ListLoaiGiaoDich != null && this.ListLoaiGiaoDich.Count > 0)
                {
                    foreach (var item in this.ListLoaiGiaoDich)
                    {
                        dayName += item.TRANSACTION_TYPE_NAME;
                        if (!(item == ListLoaiGiaoDich.Last()))
                        {
                            dayName += ", ";
                        }
                    }
                }
                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdLoaiHoaDon_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.ListLoaiHoaDon != null && this.ListLoaiHoaDon.Count > 0)
                {
                    foreach (var item in this.ListLoaiHoaDon)
                    {
                        dayName += item.Name;
                        if (!(item == ListLoaiHoaDon.Last()))
                        {
                            dayName += ", ";
                        }
                    }
                }
                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdLoaiGiaoDich_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {

                List<long> listIDLoaiGiaoDich = this.ListLoaiGiaoDich.Select(o => o.ID).ToList();
                string strIDS = string.Join(",", listIDLoaiGiaoDich);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == grdLoaiGiaoDich.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = grdLoaiGiaoDich.Name;
                    csAddOrUpdate.VALUE = strIDS;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);

                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void grdLoaiHoaDon_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == grdLoaiHoaDon.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                List<long> listIDTType = this.ListLoaiHoaDon.Select(o => o.ID).ToList();
                string strIDS = string.Join(",", listIDTType);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = grdLoaiHoaDon.Name;
                    csAddOrUpdate.VALUE = strIDS;
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemEnbtnHDDT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                transactionPrint = new V_HIS_TRANSACTION();
                this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                listData = new List<V_HIS_TRANSACTION>();
                listData.Add(transactionPrint);
                this.InHoaDonDienTu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                List<object> listArgs = new List<object>();
                listArgs.Add(transactionPrint.ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionDepositDetail", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemUnrejectCancellation_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                if (this.transactionPrint != null)
                {
                    repositoryItemUnrejectCancellation_ButtonClick(this.transactionPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemUnrejectCancellation_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                HisTransactionUnrejectCancellationRequestSDO sdo = new HisTransactionUnrejectCancellationRequestSDO();
                sdo.WorkingRoomId = currentModule.RoomId;
                sdo.TransactionId = data.ID;
                var rs = new BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/UnrejectCancellationRequest", ApiConsumers.MosConsumer, sdo, param);
                success = rs != null;
                if (success)
                {
                    FillDataToGrid();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCodePayQr_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtCodePayQr.Focus();
                txtCodePayQr.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void bbtnCodeTransaction_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtTransactionCode.Focus();
                txtTransactionCode.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void txtCodePayQr_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbLook_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusLook.Checked)
                {
                    cbStatusAll1.Checked = false;
                    cbStatusNoLook.Checked = false;
                }
                else if (!cbStatusAll1.Checked && !cbStatusNoLook.Checked)
                {
                    cbStatusLook.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbStatusAll1_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusAll1.Checked)
                {
                    cbStatusNoLook.Checked = false;
                    cbStatusLook.Checked = false;
                }
                else if (!cbStatusNoLook.Checked && !cbStatusLook.Checked)
                {
                    cbStatusAll1.Checked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cbNoLook_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (cbStatusNoLook.Checked)
                {
                    cbStatusAll1.Checked = false;
                    cbStatusLook.Checked = false;
                }
                else if (!cbStatusAll1.Checked && !cbStatusLook.Checked)
                {
                    cbStatusNoLook.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTransactionCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void repQr_ButtonClick(V_HIS_TRANSACTION data)
        {
            try
            {
                //this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                transactionPrint = data;
                V_HIS_ROOM v_HIS_ROOM = (
            from o in BackendDataWorker.Get<V_HIS_ROOM>()
            where o.ID == this.currentModule.RoomId
            select o).FirstOrDefault<V_HIS_ROOM>();
                if (v_HIS_ROOM != null && !string.IsNullOrEmpty(v_HIS_ROOM.QR_CONFIG_JSON))
                {
                    ItemConfig itemConfig = JsonConvert.DeserializeObject<ItemConfig>(v_HIS_ROOM.QR_CONFIG_JSON);
                    string key = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", itemConfig.BANK);
                    this.selectedConfig = (
                        from o in this.listConfig
                        where o.KEY == key
                        select o).FirstOrDefault<HIS_CONFIG>();
                    this.selectedConfig.VALUE = itemConfig.VALUE;
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    adoqr.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                    adoqr.TransReqId = CreateReqType.Transaction;
                    adoqr.ConfigValue = selectedConfig;
                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionPrint);
                    adoqr.Transaction = tran;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                else if (listConfig != null && listConfig.Count > 0)
                {
                    if (listConfig.Count > 1)
                    {
                        popupMenu1.ClearLinks();
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


                            BarButtonItem btnOption = new BarButtonItem(null, key);
                            btnOption.ItemClick += (s, args) =>
                            {

                                selectedConfig = item;
                                List<object> listArgs = new List<object>();
                                TransReqQRADO adoqr = new TransReqQRADO();
                                adoqr.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                                adoqr.TransReqId = CreateReqType.Transaction;
                                adoqr.ConfigValue = selectedConfig;
                                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionPrint);
                                adoqr.Transaction = tran;
                                listArgs.Add(adoqr);
                                LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                            };
                            popupMenu1.AddItem(btnOption);
                        }
                        popupMenu1.ShowPopup(Cursor.Position);
                    }
                    else
                    {
                        selectedConfig = listConfig[0];
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                        adoqr.TransReqId = CreateReqType.Transaction;
                        adoqr.ConfigValue = selectedConfig;
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionPrint);
                        adoqr.Transaction = tran;
                        listArgs.Add(adoqr);
                        LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repQr_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                if ((transactionPrint.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || transactionPrint.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU) && transactionPrint.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && transactionPrint.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    repQr_ButtonClick(transactionPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnChangePay_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                if (transactionPrint.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && transactionPrint.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && string.IsNullOrEmpty(transactionPrint.BANK_TRANSACTION_CODE))
                    btnChangePayForm_ButtonClick(transactionPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
 