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
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ContactDrugstore.ADO;
using HIS.Desktop.Plugins.Library.NationalPharmacyConnect;
using HIS.Desktop.Plugins.Library.NationalPharmacyConnect.JsonADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.ContactDrugstore
{
    public partial class frmContactDrugstore : HIS.Desktop.Utility.FormBase
    {
        internal string typeCodeFind = "Từ khóa tìm kiếm";//Set lại giá trị trong resource
        internal string typeCodeFind__TimKiem = "Từ khóa tìm kiếm";//Set lại giá trị trong resource
        internal string typeCodeFind__Ma = "Mã";//Set lại giá trị trong resource

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        internal List<DataADO> _DataADOs { get; set; }
        internal AccountConnectADO _AccountConnectADO { get; set; }
        V_HIS_MEDI_STOCK _HisMediStock { get; set; }

        ThaoTac _ThaoTac = ThaoTac.TaoMoi;
        NationalPharmacyConnectProcess ConnectProcess { get; set; }

        List<HIS_EXP_MEST> _ExpMestUpdates { get; set; }
        List<HIS_IMP_MEST> _ImpMestUpdates { get; set; }
        List<HIS_TRANSACTION> _TransactionUpdates { get; set; }
        List<long> _ExpMestIdCancels { get; set; }
        List<long> _ImpMestIdCancels { get; set; }
        List<long> _TransactionIdCancels { get; set; }

        List<string> __StrMess { get; set; }

        public frmContactDrugstore(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.currentModule = module;
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

        private void frmContactDrugstore_Load(object sender, EventArgs e)
        {
            try
            {
                _HisMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                InitTypeFind();
                LoadCboType();
                LoadCboStatus();
                SetDataDefault();
                GetValueAccountConnect();
                GetValueAccountConfig();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetValueAccountConnect()
        {
            try
            {
                this._AccountConnectADO = new AccountConnectADO();
                string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ContactDrugstore__Address_Connect_DuocQuocGia");
                Inventec.Common.Logging.LogSystem.Debug("AccountConnect___" + key);
                if (String.IsNullOrEmpty(key))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa khai báo thông tin cấu hình hệ thống", "Thông báo");
                }
                else
                {
                    this._AccountConnectADO = (AccountConnectADO)Newtonsoft.Json.JsonConvert.DeserializeObject<AccountConnectADO>(key);
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("_AccountConnectADO>>>>", _AccountConnectADO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetValueAccountConfig()
        {
            try
            {
                string key = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__CONTACT_DRUGSTORE__ACCOUNT");
                Inventec.Common.Logging.LogSystem.Debug("AccountConfig___" + key);
                if (String.IsNullOrEmpty(key))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa khai báo thông tin cấu hình tài khoản", "Thông báo");
                }
                else
                {
                    string mess = "";
                    var ado = (AccountConnectADO)Newtonsoft.Json.JsonConvert.DeserializeObject<AccountConnectADO>(key);
                    if (ado != null)
                    {
                        if (String.IsNullOrEmpty(ado.Loginname))
                        {
                            mess += "tên đăng nhập";
                        }
                        if (String.IsNullOrEmpty(ado.Password))
                        {
                            if (!String.IsNullOrEmpty(mess))
                                mess += ", ";
                            mess += "mật khẩu";
                        }
                        if (String.IsNullOrEmpty(ado.MaCoSo))
                        {
                            if (!String.IsNullOrEmpty(mess))
                                mess += ", ";
                            mess += "mã cơ sở";
                        }
                        if (!String.IsNullOrEmpty(mess))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa khai báo thông tin " + mess + " trong cấu hình tài khoản", "Thông báo");
                        }
                        this._AccountConnectADO.Loginname = ado.Loginname;
                        this._AccountConnectADO.Password = ado.Password;
                        this._AccountConnectADO.MaCoSo = ado.MaCoSo;
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("_AccountConnectADO__after__added_account_config_keys>>>>", _AccountConnectADO));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPatientCode = new DXMenuItem(typeCodeFind__TimKiem, new EventHandler(btnCodeFind_Click));
                itemPatientCode.Tag = "search";
                menu.Items.Add(itemPatientCode);

                DXMenuItem itemProgramCode = new DXMenuItem(typeCodeFind__Ma, new EventHandler(btnCodeFind_Click));
                itemProgramCode.Tag = "expMestCode";
                menu.Items.Add(itemProgramCode);

                dropDownBtn.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                dropDownBtn.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind = btnMenuCodeFind.Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownBtn_Click(object sender, EventArgs e)
        {
            try
            {
                dropDownBtn.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboType()
        {
            try
            {
                List<FilterADO> _filterAdos = new List<FilterADO>();

                _filterAdos.Add(new FilterADO(1, "01", "Phiếu xuất"));
                _filterAdos.Add(new FilterADO(2, "02", "Phiếu nhập"));
                _filterAdos.Add(new FilterADO(3, "03", "Hóa đơn"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboType, _filterAdos, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboStatus()
        {
            try
            {
                List<FilterADO> _filterAdos = new List<FilterADO>();

                _filterAdos.Add(new FilterADO(1, "01", "Tất cả"));
                _filterAdos.Add(new FilterADO(2, "02", "Đã đẩy"));
                _filterAdos.Add(new FilterADO(3, "03", "Chưa đẩy"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatus, _filterAdos, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                this._DataADOs = new List<DataADO>();
                if (this.cboType.EditValue != null)
                {
                    if ((long)this.cboType.EditValue == 1)
                    {
                        GetExpMestDatas();
                    }
                    else if ((long)this.cboType.EditValue == 2)
                    {
                        GetImpMestDatas();
                    }
                    else if ((long)this.cboType.EditValue == 3)
                    {
                        GetTransactionDatas();
                    }
                }
                else
                {
                    GetExpMestDatas();
                    GetImpMestDatas();
                    GetTransactionDatas();
                }
                this.gridControlData.DataSource = null;
                this.gridControlData.DataSource = this._DataADOs;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestDatas()
        {
            CommonParam param = new CommonParam();
            HisExpMestFilter filter = new HisExpMestFilter();
            if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
            }
            if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            if (this.typeCodeFind == this.typeCodeFind__TimKiem)
            {
                filter.KEY_WORD = this.txtSearch.Text.Trim();
            }
            else
            {
                filter.EXP_MEST_CODE__EXACT = string.Format("{0:000000000000}", Convert.ToInt64(this.txtSearch.Text.Trim()));
            }
            if (this.cboStatus.EditValue != null)
            {
                if ((long)this.cboStatus.EditValue == 2)
                {
                    filter.HAS_NATIONAL_EXP_MEST_CODE = true;
                }
                else if ((long)this.cboStatus.EditValue == 3)
                {
                    filter.HAS_NATIONAL_EXP_MEST_CODE = false;
                }
            }
            filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

            if (this._HisMediStock != null)
            {
                filter.MEDI_STOCK_ID = this._HisMediStock.ID;
            }
            else
                return;

            var _ExpMests = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);
            if (_ExpMests != null && _ExpMests.Count > 0)
            {
                _ExpMests = _ExpMests.Where(p => p.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && p.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK && p.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL).ToList();
                if (_ExpMests != null && _ExpMests.Count > 0)
                {
                    this._DataADOs.AddRange((from m in _ExpMests select new DataADO(m)).ToList());
                }
            }
        }

        private void GetImpMestDatas()
        {
            CommonParam param = new CommonParam();
            HisImpMestFilter filter = new HisImpMestFilter();
            if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
            }
            if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            if (this.typeCodeFind == this.typeCodeFind__TimKiem)
            {
                filter.KEY_WORD = this.txtSearch.Text.Trim();
            }
            else
            {
                filter.IMP_MEST_CODE__EXACT = string.Format("{0:000000000000}", Convert.ToInt64(this.txtSearch.Text.Trim()));
            }
            if (this.cboStatus.EditValue != null)
            {
                if ((long)this.cboStatus.EditValue == 2)
                {
                    filter.HAS_NATIONAL_IMP_MEST_CODE = true;
                }
                else if ((long)this.cboStatus.EditValue == 3)
                {
                    filter.HAS_NATIONAL_IMP_MEST_CODE = false;
                }
            }
            filter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;

            if (this._HisMediStock != null)
            {
                filter.MEDI_STOCK_ID = this._HisMediStock.ID;
            }
            else
                return;

            var _ImpMests = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, filter, param);
            if (_ImpMests != null && _ImpMests.Count > 0)
            {
                _ImpMests = _ImpMests.Where(p => p.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL && p.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                    && p.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT).ToList();
                if (_ImpMests != null && _ImpMests.Count > 0)
                {
                    this._DataADOs.AddRange((from m in _ImpMests select new DataADO(m)).ToList());
                }
            }
        }

        private void GetTransactionDatas()
        {
            CommonParam param = new CommonParam();
            HisTransactionFilter filter = new HisTransactionFilter();
            filter.SALE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP;
            if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
            }
            if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
            {
                filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            if (this.typeCodeFind == this.typeCodeFind__TimKiem)
            {
                filter.KEY_WORD = this.txtSearch.Text.Trim();
            }
            else
            {
                filter.TRANSACTION_CODE__EXACT = string.Format("{0:000000000000}", Convert.ToInt64(this.txtSearch.Text.Trim()));
            }
            if (this.cboStatus.EditValue != null)
            {
                if ((long)this.cboStatus.EditValue == 2)
                {
                    filter.HAS_NATIONAL_TRANSACTION_CODE = true;
                }
                else if ((long)this.cboStatus.EditValue == 3)
                {
                    filter.HAS_NATIONAL_TRANSACTION_CODE = false;
                }
            }
            var _Transactions = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, param);
            if (_Transactions != null && _Transactions.Count > 0)
            {
                this._DataADOs.AddRange((from m in _Transactions select new DataADO(m)).ToList());
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (DataADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TYPE_STR")
                        {
                            string mes = "";
                            if (data.TYPE == 1)
                            {
                                mes = "Phiếu xuất";
                            }
                            else if (data.TYPE == 2)
                            {
                                mes = "Phiếu nhập";
                            }
                            else if (data.TYPE == 3)
                            {
                                mes = "Hóa đơn";
                            }
                            e.Value = mes;
                        }
                    }
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
                this.lblStatus.Text = "";
                this.LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblStatus.Text = "";
                SetDataDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                this.cboType.EditValue = null;
                this.cboStatus.EditValue = null;
                this.txtSearch.Text = "";
                this.dtTimeFrom.EditValue = DateTime.Now;
                this.dtTimeTo.EditValue = DateTime.Now;
                this.typeCodeFind = this.typeCodeFind__TimKiem;
                this.dropDownBtn.Text = this.typeCodeFind__TimKiem;
                this.gridControlData.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewData.RowCount > 0 && gridViewData.SelectedRowsCount > 0)
                {
                    this.btnSend.Enabled = true;
                    this.btnNoSend.Enabled = true;
                }
                else
                {
                    this.btnSend.Enabled = false;
                    this.btnNoSend.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                long TYPE = Inventec.Common.TypeConvert.Parse.ToInt64(gridViewData.GetRowCellValue(e.RowHandle, "TYPE").ToString());
                string _NATIONAL = (string)gridViewData.GetRowCellValue(e.RowHandle, "NATIONAL");
                if (TYPE == 1)
                {
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
                else if (TYPE == 2)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (TYPE == 3)
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
                else if (TYPE == 4)
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
                if (!string.IsNullOrEmpty(_NATIONAL))
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadOnClick(ThaoTac thaotac)
        {
            try
            {
                this._ThaoTac = thaotac;
                this.lblStatus.Text = "";
                this.ConnectProcess = new NationalPharmacyConnectProcess(this._AccountConnectADO.Address, this._AccountConnectADO.Loginname, this._AccountConnectADO.Password);
                this._ExpMestIdCancels = new List<long>();
                this._ImpMestIdCancels = new List<long>();
                this._TransactionIdCancels = new List<long>();
                this._ExpMestUpdates = new List<HIS_EXP_MEST>();
                this._ImpMestUpdates = new List<HIS_IMP_MEST>();
                this._TransactionUpdates = new List<HIS_TRANSACTION>();
                this.lblStatus.Text = "Đang xử lí";
                //ThreadCustomManager.ThreadResultCallBack(ProcessConnect, CallBackLoadChangedStatus);
                ProcessConnect();
                CallBackLoadChangedStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallBackLoadChangedStatus()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ProcessData(); }));
                }
                else
                {
                    ProcessData();
                }

                if (this.__StrMess != null && this.__StrMess.Count > 0)
                {
                    this.__StrMess = this.__StrMess.Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("__StrMess", __StrMess));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                if (this._ThaoTac == ThaoTac.CapNhat || this._ThaoTac == ThaoTac.TaoMoi)
                {
                    if (this._ExpMestUpdates != null && this._ExpMestUpdates.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/UpdateNationalCode", ApiConsumers.MosConsumer, this._ExpMestUpdates, param);
                        rs = data != null && data.Count > 0;
                    }
                    if (this._ImpMestUpdates != null && this._ImpMestUpdates.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_IMP_MEST>>("api/HisImpMest/UpdateNationalCode", ApiConsumers.MosConsumer, this._ImpMestUpdates, param);
                        rs = data != null && data.Count > 0;
                    }
                    if (this._TransactionUpdates != null && this._TransactionUpdates.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_TRANSACTION>>("api/HisTransaction/UpdateNationalCode", ApiConsumers.MosConsumer, this._TransactionUpdates, param);
                        rs = data != null && data.Count > 0;
                    }
                }
                else if (this._ThaoTac == ThaoTac.Xoa)
                {
                    if (this._ExpMestIdCancels != null && this._ExpMestIdCancels.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/CancelNationalCode", ApiConsumers.MosConsumer, this._ExpMestIdCancels, param);
                        rs = data != null && data.Count > 0;
                    }
                    if (this._ImpMestIdCancels != null && this._ImpMestIdCancels.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_IMP_MEST>>("api/HisImpMest/CancelNationalCode", ApiConsumers.MosConsumer, this._ImpMestIdCancels, param);
                        rs = data != null && data.Count > 0;
                    }
                    if (this._TransactionIdCancels != null && this._TransactionIdCancels.Count > 0)
                    {
                        var data = new BackendAdapter(param).Post<List<HIS_TRANSACTION>>("api/HisTransaction/CancelNationalCode", ApiConsumers.MosConsumer, this._TransactionIdCancels, param);
                        rs = data != null && data.Count > 0;
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                string mess = "";
                if (!rs)
                {
                    mess = "Xử lí thất bại";
                }
                else
                {
                    mess = "Xử lí thành công";
                    this.LoadDataToGridControl();
                }

                this.lblStatus.Text = mess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessConnect()
        {
            try
            {
                List<DataADO> _dataChecks = new List<DataADO>();
                var rowHandles = gridViewData.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (DataADO)gridViewData.GetRow(i);
                        if (row != null)
                        {
                            _dataChecks.Add(row);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _dataChecks), _dataChecks));

                if (_dataChecks != null && _dataChecks.Count > 0)
                {
                    if (this._ThaoTac == ThaoTac.Xoa)
                    {
                        var dataNations = _dataChecks.Where(p => string.IsNullOrEmpty(p.NATIONAL)).ToList();
                        if (dataNations != null && dataNations.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại dữ liệu chưa có mã quốc gia.", "Thông báo");
                            return;
                        }
                    }

                    //var dataDonThuocs = _dataChecks.Where(p => p.TYPE == 1 && !CheckXuatBanKhachVangLai(p)).ToList();
                    //var dataPhieuXuats = _dataChecks.Where(p => p.TYPE == 1 && CheckXuatBanKhachVangLai(p)).ToList();
                    var dataPhieuXuats = _dataChecks.Where(p => p.TYPE == 1 && p.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN).ToList();
                    var dataPhieuXuatBans = _dataChecks.Where(p => p.TYPE == 1 && p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN).ToList();
                    var dataPhieuNhaps = _dataChecks.Where(p => p.TYPE == 2).ToList();
                    var dataHoaDons = _dataChecks.Where(p => p.TYPE == 3).ToList();

                    this.__StrMess = new List<string>();

                    if (dataPhieuXuatBans != null && dataPhieuXuatBans.Count > 0)
                    {
                        ProcessHoaDon(dataPhieuXuatBans, false);
                    }
                    if (dataPhieuXuats != null && dataPhieuXuats.Count > 0)
                    {
                        ProcessPhieuXuat(dataPhieuXuats);
                    }
                    if (dataPhieuNhaps != null && dataPhieuNhaps.Count > 0)
                    {
                        ProcessPhieuNhap(dataPhieuNhaps);
                    }
                    if (dataHoaDons != null && dataHoaDons.Count > 0)
                    {
                        ProcessHoaDon(dataHoaDons,true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckXuatBanKhachVangLai(DataADO ado)
        {
            bool result = true;
            try
            {
                if (ado != null)
                {
                    if (ado.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                        || ado.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                       || ado.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT
                        || (ado.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN
                        && ado.PRESCRIPTION_ID.HasValue
                       ))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessDonThuoc(List<DataADO> lstData)
        {
            try
            {
                List<HIS_EXP_MEST_MEDICINE> _HisExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                List<HIS_MEDICINE> _HisMedicines = new List<HIS_MEDICINE>();
                List<HIS_TREATMENT> _HisTreatments = new List<HIS_TREATMENT>();
                int skip = 0;
                while (lstData.Count - skip > 0)
                {
                    var listSub = lstData.Skip(skip).Take(100).ToList();
                    skip += 100;

                    List<long> _expMestIds = listSub.Select(p => p.ID).Distinct().ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                    filter.EXP_MEST_IDs = _expMestIds;
                    var datas = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        _HisExpMestMedicines.AddRange(datas);
                        List<long> _medicineIds = datas.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _medicineIds;
                        var medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        if (medicines != null && medicines.Count > 0)
                        {
                            _HisMedicines.AddRange(medicines);
                        }
                    }

                    List<long> _treatmentIds = listSub.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    MOS.Filter.HisTreatmentFilter filterTreatment = new HisTreatmentFilter();
                    filterTreatment.IDs = _treatmentIds;
                    var treatments = new BackendAdapter(null).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, filterTreatment, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        _HisTreatments.AddRange(treatments);
                    }
                }

                foreach (var item in lstData)
                {
                    bool ICheck = false;
                    string mess = "Mã đơn thuốc " + item.CODE + " thiếu ";
                    DonThuoc donthuoc = new DonThuoc();
                    donthuoc.MaDonThuocCSKCB = item.CODE;//50
                    donthuoc.NgayKeDon = item.TDL_INTRUCTION_TIME != null ? item.TDL_INTRUCTION_TIME.ToString().Substring(0, 12) : "";//12.x
                    if (string.IsNullOrEmpty(donthuoc.NgayKeDon))
                    {
                        ICheck = true;
                        mess += "NgayKeDon - TDL_INTRUCTION_TIME,";
                    }
                    donthuoc.TenNguoiKeDon = item.REQ_USERNAME;
                    var treatment = _HisTreatments.FirstOrDefault(p => p.ID == item.TDL_TREATMENT_ID);
                    if (treatment == null)
                        continue;
                    ThongTinBenh _thongTinBenh = new ThongTinBenh();
                    _thongTinBenh.MaBenh = treatment.ICD_CODE;//15.x
                    if (string.IsNullOrEmpty(_thongTinBenh.MaBenh))
                    {
                        ICheck = true;
                        mess += "MaBenh - ICD_CODE,";
                    }
                    _thongTinBenh.TenBenh = treatment.ICD_NAME;//n.x
                    if (string.IsNullOrEmpty(_thongTinBenh.TenBenh))
                    {
                        ICheck = true;
                        mess += "TenBenh - ICD_NAME,";
                    }
                    donthuoc.ThongTinBenh = _thongTinBenh;

                    ThongTinBenhNhan _thongTinBenhNhan = new ThongTinBenhNhan();
                    _thongTinBenhNhan.DiaChi = item.TDL_PATIENT_ADDRESS;//255.x
                    if (string.IsNullOrEmpty(_thongTinBenhNhan.DiaChi))
                    {
                        ICheck = true;
                        mess += "DiaChi - TDL_PATIENT_ADDRESS,";
                    }
                    _thongTinBenhNhan.GioiTinh = item.TDL_PATIENT_GENDER_ID == (long)1 ? 2 : (item.TDL_PATIENT_GENDER_ID == (long)2 ? 1 : Inventec.Common.TypeConvert.Parse.ToInt32(item.TDL_PATIENT_GENDER_ID.ToString()));//1
                    _thongTinBenhNhan.HoTen = item.TDL_PATIENT_NAME;//50.x
                    if (string.IsNullOrEmpty(_thongTinBenhNhan.HoTen))
                    {
                        ICheck = true;
                        mess += "HoTen - TDL_PATIENT_NAME,";
                    }
                    _thongTinBenhNhan.MaBenhNhan = item.TDL_PATIENT_CODE;//50
                    _thongTinBenhNhan.Tuoi = AgeUtil.CalculateFullAge(item.TDL_PATIENT_DOB ?? 0);
                    donthuoc.ThongTinBenhNhan = _thongTinBenhNhan;

                    donthuoc.ThongTinDonThuoc = new List<ThongTinDonThuoc>();

                    var dataMedicines = _HisExpMestMedicines.Where(p => p.EXP_MEST_ID == item.ID).ToList();
                    if (dataMedicines != null && dataMedicines.Count > 0)
                    {
                        string messThuoc = " ---------- EXP_MEST_ID == " + item.ID + "---";
                        foreach (var itemThuoc in dataMedicines)
                        {
                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemThuoc.TDL_MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            messThuoc += "Mã thuốc " + medicineType.MEDICINE_TYPE_CODE;
                            ThongTinDonThuoc _thongTinDonThuoc = new ThongTinDonThuoc();
                            _thongTinDonThuoc.DonViTinh = medicineType.SERVICE_UNIT_NAME;//50.x
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.DonViTinh))
                            {
                                ICheck = true;
                                messThuoc += "DonViTinh - SERVICE_UNIT_NAME,";
                            }
                            _thongTinDonThuoc.DuongDung = medicineType.MEDICINE_USE_FORM_NAME;
                            _thongTinDonThuoc.HamLuong = medicineType.CONCENTRA;//500.x
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.HamLuong))
                            {
                                ICheck = true;
                                messThuoc += "HamLuong - CONCENTRA,";
                            }
                            _thongTinDonThuoc.LieuDung = !string.IsNullOrEmpty(itemThuoc.TUTORIAL) ? itemThuoc.TUTORIAL : medicineType.MEDICINE_USE_FORM_NAME;//200.x //Voi don xuat ban tam thoi lay duong dung,
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.LieuDung))
                            {
                                ICheck = true;
                                messThuoc += "LieuDung - TUTORIAL,";
                            }
                            _thongTinDonThuoc.MaThuoc = medicineType.MEDICINE_NATIONAL_CODE;//50.x
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.MaThuoc))
                            {
                                ICheck = true;
                                messThuoc += "MaThuoc - MEDICINE_NATIONAL_CODE,";
                            }
                            _thongTinDonThuoc.SoDangKy = medicineType.REGISTER_NUMBER;//50.x
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.SoDangKy))
                            {
                                ICheck = true;
                                messThuoc += "SoDangKy - REGISTER_NUMBER,";
                            }
                            //_thongTinDonThuoc.SoLuong = Inventec.Common.TypeConvert.Parse.ToInt32(itemThuoc.AMOUNT.ToString());
                            _thongTinDonThuoc.SoLuong = (int)Math.Round(itemThuoc.AMOUNT, 0, MidpointRounding.AwayFromZero);
                            //_thongTinDonThuoc.SoLuong = itemThuoc.AMOUNT;
                            _thongTinDonThuoc.TenThuoc = medicineType.MEDICINE_TYPE_NAME;//200.x
                            if (string.IsNullOrEmpty(_thongTinDonThuoc.TenThuoc))
                            {
                                ICheck = true;
                                messThuoc += "TenThuoc - MEDICINE_TYPE_NAME,";
                            }

                            donthuoc.ThongTinDonThuoc.Add(_thongTinDonThuoc);
                        }
                        mess += messThuoc + "_____";
                    }

                    ThongTinDonVi _thongTinDonVi = new ThongTinDonVi();
                    _thongTinDonVi.MaCSKCB = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;//20.x
                    if (string.IsNullOrEmpty(_thongTinDonVi.MaCSKCB))
                    {
                        ICheck = true;
                        mess += "MaCSKCB - HEIN_MEDI_ORG_CODE,";
                    }
                    _thongTinDonVi.TenCSKCB = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(p => p.MEDI_ORG_CODE == _thongTinDonVi.MaCSKCB).MEDI_ORG_NAME;//100.x
                    if (string.IsNullOrEmpty(_thongTinDonVi.TenCSKCB))
                    {
                        ICheck = true;
                        mess += "TenCSKCB - MEDI_ORG_NAME,";
                    }
                    donthuoc.ThongTinDonVi = _thongTinDonVi;

                    if (ICheck)
                    {
                        Inventec.Common.Logging.LogSystem.Error(mess);
                        continue;
                    }

                    if (donthuoc.ThongTinDonThuoc != null && donthuoc.ThongTinDonThuoc.Count > 0)
                        CallDonThuocQuocGia(item.ID, donthuoc);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessHoaDon(List<DataADO> lstData, bool isTransaction)
        {
            try
            {
                List<HIS_EXP_MEST_MEDICINE> _HisExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                List<HIS_MEDICINE> _HisMedicines = new List<HIS_MEDICINE>();
                List<HIS_EXP_MEST> listExpMest = new List<HIS_EXP_MEST>();
                int skip = 0;
                while (lstData.Count - skip > 0)
                {
                    var listSub = lstData.Skip(skip).Take(100).ToList();
                    skip += 100;

                    List<long> transactionIds = listSub.Select(p => p.ID).Distinct().ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestFilter filter = new HisExpMestFilter();
                    if (isTransaction)
                    {
                        filter.BILL_IDs = transactionIds;
                    }
                    else
                    {
                        filter.IDs = transactionIds;
                    }
                    var datas = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        listExpMest.AddRange(datas);

                        MOS.Filter.HisExpMestMedicineFilter medicineFilter = new HisExpMestMedicineFilter();
                        medicineFilter.EXP_MEST_IDs = datas.Select(s => s.ID).ToList();
                        var expMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        if (expMedicines != null && expMedicines.Count > 0)
                        {
                            _HisExpMestMedicines.AddRange(expMedicines);
                            List<long> _medicineIds = expMedicines.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                            MOS.Filter.HisMedicineFilter mediFilter = new HisMedicineFilter();
                            mediFilter.IDs = _medicineIds;
                            var medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, mediFilter, param);
                            if (medicines != null && medicines.Count > 0)
                            {
                                _HisMedicines.AddRange(medicines);
                            }
                        }
                    }
                }

                foreach (var item in lstData)
                {
                    bool ICheck = false;
                    string mess = "Mã Hóa đơn " + item.CODE + " thiếu ";

                    HoaDon hoadon = new HoaDon();
                    hoadon.MaHoaDon = item.CODE;//50.x
                    if (string.IsNullOrEmpty(hoadon.MaHoaDon))
                    {
                        ICheck = true;
                        mess += "MaHoaDon - CODE,";
                    }
                    hoadon.NgayBan = item.TDL_INTRUCTION_TIME != null ? item.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) : "";//12.x
                    if (string.IsNullOrEmpty(hoadon.NgayBan))
                    {
                        ICheck = true;
                        mess += "NgayBan - TDL_INTRUCTION_TIME,";
                    }
                    hoadon.ChiTietHoaDon = new List<ChiTietHoaDon>();
                    HIS_EXP_MEST expMest = null;
                    if (isTransaction)
                    {
                        expMest = listExpMest.FirstOrDefault(o => o.BILL_ID == item.ID);
                    }else
                    {
                        expMest = listExpMest.FirstOrDefault(o => o.ID == item.ID);
                    }

                    if (expMest == null)
                        continue;

                    var dataMedicines = _HisExpMestMedicines.Where(p => p.EXP_MEST_ID == expMest.ID).ToList();
                    if (dataMedicines != null && dataMedicines.Count > 0)
                    {
                        string messThuoc = " ---------- EXP_MEST_ID == " + item.ID + "---";
                        foreach (var itemThuoc in dataMedicines)
                        {
                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemThuoc.TDL_MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            ChiTietHoaDon _chiTietHoaDon = new ChiTietHoaDon();
                            _chiTietHoaDon.DonViTinh = medicineType.SERVICE_UNIT_NAME;//50.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.DonViTinh))
                            {
                                ICheck = true;
                                messThuoc += "DonViTinh - SERVICE_UNIT_NAME,";
                            }
                            _chiTietHoaDon.DuongDung = medicineType.MEDICINE_USE_FORM_NAME;//200
                            _chiTietHoaDon.HamLuong = medicineType.CONCENTRA;//500.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.HamLuong))
                            {
                                ICheck = true;
                                messThuoc += "HamLuong - CONCENTRA,";
                            }
                            _chiTietHoaDon.LieuDung = itemThuoc.DESCRIPTION;//x
                            _chiTietHoaDon.MaThuoc = medicineType.MEDICINE_NATIONAL_CODE;//50.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.MaThuoc))
                            {
                                ICheck = true;
                                messThuoc += "MaThuoc - MEDICINE_NATIONAL_CODE,";
                            }
                            _chiTietHoaDon.SoDangKy = medicineType.REGISTER_NUMBER;
                            //_chiTietHoaDon.SoLuong = Inventec.Common.TypeConvert.Parse.ToInt32(itemThuoc.AMOUNT.ToString());//x
                            _chiTietHoaDon.SoLuong = (int)Math.Round(itemThuoc.AMOUNT, 0, MidpointRounding.AwayFromZero);
                            //_chiTietHoaDon.SoLuong = itemThuoc.AMOUNT;
                            if (_chiTietHoaDon.SoLuong <= 0)
                            {
                                ICheck = true;
                                messThuoc += "SoLuong <=0,";
                            }

                            var medicine = _HisMedicines.FirstOrDefault(p => p.ID == itemThuoc.MEDICINE_ID);
                            _chiTietHoaDon.SoLo = medicine != null ? medicine.PACKAGE_NUMBER : "";//50.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.SoLo))
                            {
                                ICheck = true;
                                messThuoc += "SoLo - PACKAGE_NUMBER,";
                            }
                            _chiTietHoaDon.NgaySanXuat = "";//12
                            _chiTietHoaDon.HanDung =
                            (medicine.EXPIRED_DATE != null
                            && medicine.EXPIRED_DATE > 0)
                            ? medicine.EXPIRED_DATE.ToString().Substring(0, 8) : "";//12.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.HanDung))
                            {
                                ICheck = true;
                                messThuoc += "HanDung - EXPIRED_DATE,";
                            }
                            //_chiTietHoaDon.DonGia = Inventec.Common.TypeConvert.Parse.ToInt32(medicineType.IMP_PRICE != null ? medicineType.IMP_PRICE.ToString() : "0");//x Đơn giá thuốc
                            _chiTietHoaDon.DonGia = (int)Math.Round(itemThuoc.PRICE ?? 0, 0, MidpointRounding.AwayFromZero);
                            //_chiTietHoaDon.DonGia = medicineType.IMP_PRICE ?? 0;
                            if (_chiTietHoaDon.DonGia <= 0)
                            {
                                ICheck = true;
                                messThuoc += "DonGia - PRICE <=0,";
                            }
                            _chiTietHoaDon.ThanhTien = _chiTietHoaDon.DonGia * _chiTietHoaDon.SoLuong;//x Thành tiền
                            if (_chiTietHoaDon.ThanhTien <= 0)
                            {
                                ICheck = true;
                                messThuoc += "ThanhTien <=0,";
                            }
                            _chiTietHoaDon.TyLeQuyDoi = 1;//x Tỷ lệ quy đổi từ đơn vị tính nhập ở trên so với đơn vị cơ bản
                            _chiTietHoaDon.TenThuoc = medicineType.MEDICINE_TYPE_NAME;//500.x
                            if (string.IsNullOrEmpty(_chiTietHoaDon.TenThuoc))
                            {
                                ICheck = true;
                                mess += "TenThuoc - MEDICINE_TYPE_NAME,";
                            }

                            hoadon.ChiTietHoaDon.Add(_chiTietHoaDon);
                        }
                        mess += messThuoc + "_____";
                    }

                    hoadon.MaCoSo = _AccountConnectADO.MaCoSo;// BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;//50.x
                    if (string.IsNullOrEmpty(hoadon.MaCoSo))
                    {
                        ICheck = true;
                        mess += "MaCoSo - HEIN_MEDI_ORG_CODE,";
                    }
                    hoadon.HoTenKhachHang = expMest.TDL_PATIENT_NAME;//50
                    hoadon.HoTenNguoiBan = item.CREATOR;//50
                    hoadon.MaDonThuocQuocGia = item.NATIONAL;//50

                    if (ICheck)
                    {
                        Inventec.Common.Logging.LogSystem.Error(mess);
                        continue;
                    }

                    if (hoadon.ChiTietHoaDon != null && hoadon.ChiTietHoaDon.Count > 0)
                        CallHoaDonQuocGia(item.ID, hoadon, isTransaction);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPhieuXuat(List<DataADO> lstData)
        {
            try
            {
                List<HIS_EXP_MEST_MEDICINE> _HisExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                List<HIS_MEDICINE> _HisMedicines = new List<HIS_MEDICINE>();

                int skip = 0;
                while (lstData.Count - skip > 0)
                {
                    var listSub = lstData.Skip(skip).Take(100).ToList();
                    skip += 100;

                    List<long> _expMestIds = listSub.Select(p => p.ID).Distinct().ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                    filter.EXP_MEST_IDs = _expMestIds;
                    var datas = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        _HisExpMestMedicines.AddRange(datas);
                        List<long> _medicineIds = datas.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _medicineIds;
                        var medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        if (medicines != null && medicines.Count > 0)
                        {
                            _HisMedicines.AddRange(medicines);
                        }
                    }
                }

                foreach (var item in lstData)
                {
                    bool ICheck = false;
                    string mess = "Mã xuất " + item.CODE + " thiếu ";

                    PhieuXuat phieuxuat = new PhieuXuat();
                    phieuxuat.MaPhieu = item.CODE;//50.x 
                    if (string.IsNullOrEmpty(phieuxuat.MaPhieu))
                    {
                        ICheck = true;
                        mess += "MaPhieu - CODE,";
                    }
                    phieuxuat.NgayXuat = (item.TDL_INTRUCTION_TIME != null
                        && item.TDL_INTRUCTION_TIME > 0
                        && item.TDL_INTRUCTION_TIME.ToString().Length > 8) ?
                        item.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) : "";//12.x
                    if (string.IsNullOrEmpty(phieuxuat.NgayXuat))
                    {
                        ICheck = true;
                        mess += "NgayXuat - TDL_INTRUCTION_TIME,";
                    }
                    phieuxuat.MaCoSo = _AccountConnectADO.MaCoSo;// BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;//50.x
                    if (string.IsNullOrEmpty(phieuxuat.MaCoSo))
                    {
                        ICheck = true;
                        mess += "MaCoSo - HEIN_MEDI_ORG_CODE,";
                    }

                    if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                    {
                        phieuxuat.LoaiPhieuXuat = 2;//2, Xuất trả nhà cung cấp,
                    }
                    else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                        || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                        || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                        || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                        || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                        || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        phieuxuat.LoaiPhieuXuat = 4;//nội bộ
                    }
                    else
                    {
                        phieuxuat.LoaiPhieuXuat = 3;//3: Xuất hủy
                    }

                    //phieuxuat.LoaiPhieuXuat = item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC ? 2 : 3;
                    phieuxuat.GhiChu = !string.IsNullOrEmpty(item.DESCRIPTION) ? item.DESCRIPTION : "";//500.x
                    if (item.SUPPLIER_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == item.SUPPLIER_ID);
                        phieuxuat.TenCoSoNhan = data != null ? data.SUPPLIER_NAME : "";//500
                    }

                    phieuxuat.ChiTietPhieuXuat = new List<ChiTietPhieuXuat>();

                    var dataMedicines = _HisExpMestMedicines.Where(p => p.EXP_MEST_ID == item.ID).ToList();
                    if (dataMedicines != null && dataMedicines.Count > 0)
                    {
                        string messThuoc = " ---------- EXP_MEST_ID == " + item.ID + "---";
                        foreach (var itemThuoc in dataMedicines)
                        {
                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemThuoc.TDL_MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            ChiTietPhieuXuat _chiTietPhieuXuat = new ChiTietPhieuXuat();
                            //_chiTietPhieuXuat.DonGia = Inventec.Common.TypeConvert.Parse.ToInt32(medicineType.IMP_PRICE != null ? medicineType.IMP_PRICE.ToString() : "0");//x
                            _chiTietPhieuXuat.DonGia = (int)Math.Round(itemThuoc.PRICE ?? 0, 0, MidpointRounding.AwayFromZero);
                            //_chiTietPhieuXuat.DonGia = medicineType.IMP_PRICE ?? 0;
                            if (_chiTietPhieuXuat.DonGia <= 0)
                            {
                                ICheck = true;
                                messThuoc += "DonGia <= 0 - PRICE,";
                            }
                            _chiTietPhieuXuat.DonViTinh = medicineType.SERVICE_UNIT_NAME;//200.x
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.DonViTinh))
                            {
                                ICheck = true;
                                messThuoc += "DonViTinh - SERVICE_UNIT_NAME,";
                            }

                            _chiTietPhieuXuat.So_Dklh = medicineType.REGISTER_NUMBER;//50.x Số đăng kí lưu hành của thuốc
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.So_Dklh))
                            {
                                ICheck = true;
                                messThuoc += "So_Dklh - REGISTER_NUMBER,";
                            }
                            var medicine = _HisMedicines.FirstOrDefault(p => p.ID == itemThuoc.MEDICINE_ID);
                            _chiTietPhieuXuat.SoLo = medicine != null ? medicine.PACKAGE_NUMBER : "";//50.x
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.SoLo))
                            {
                                ICheck = true;
                                messThuoc += "SoLo - PACKAGE_NUMBER,";
                            }
                            _chiTietPhieuXuat.HanDung =
                            (medicine.EXPIRED_DATE != null
                            && medicine.EXPIRED_DATE > 0)
                            ? medicine.EXPIRED_DATE.ToString().Substring(0, 8) : "";//12.x
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.HanDung))
                            {
                                ICheck = true;
                                messThuoc += "HanDung - EXPIRED_DATE,";
                            }
                            _chiTietPhieuXuat.NgaySanXuat = phieuxuat.NgayXuat;//12
                            _chiTietPhieuXuat.MaThuoc = medicineType.MEDICINE_NATIONAL_CODE;//50.x
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.MaThuoc))
                            {
                                ICheck = true;
                                messThuoc += "MaThuoc - MEDICINE_NATIONAL_CODE,";
                            }
                            //_chiTietPhieuXuat.SoLuong = Inventec.Common.TypeConvert.Parse.ToInt32(itemThuoc.AMOUNT.ToString());//x
                            _chiTietPhieuXuat.SoLuong = (int)Math.Round(itemThuoc.AMOUNT, 0, MidpointRounding.AwayFromZero);
                            //_chiTietPhieuXuat.SoLuong = itemThuoc.AMOUNT;
                            if (_chiTietPhieuXuat.SoLuong <= 0)
                            {
                                ICheck = true;
                                messThuoc += "SoLuong <= 0,";
                            }
                            _chiTietPhieuXuat.TenThuoc = medicineType.MEDICINE_TYPE_NAME;//500.x
                            if (string.IsNullOrEmpty(_chiTietPhieuXuat.TenThuoc))
                            {
                                ICheck = true;
                                messThuoc += "TenThuoc - MEDICINE_TYPE_NAME,";
                            }
                            phieuxuat.ChiTietPhieuXuat.Add(_chiTietPhieuXuat);
                        }
                        mess += messThuoc + "_____";
                    }
                    if (ICheck)
                    {
                        Inventec.Common.Logging.LogSystem.Error(mess);
                        continue;
                    }

                    if (phieuxuat.ChiTietPhieuXuat != null && phieuxuat.ChiTietPhieuXuat.Count > 0)
                        CallPhieuXuatQuocGia(item.ID, phieuxuat);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPhieuNhap(List<DataADO> lstData)
        {
            try
            {
                List<HIS_IMP_MEST_MEDICINE> _HisImpMestMedicines = new List<HIS_IMP_MEST_MEDICINE>();
                List<HIS_MEDICINE> _HisMedicines = new List<HIS_MEDICINE>();
                int skip = 0;
                while (lstData.Count - skip > 0)
                {
                    var listSub = lstData.Skip(skip).Take(100).ToList();
                    skip += 100;

                    List<long> _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestMedicineFilter filter = new HisImpMestMedicineFilter();
                    filter.IMP_MEST_IDs = _impMestIds;
                    var datas = new BackendAdapter(param).Get<List<HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        _HisImpMestMedicines.AddRange(datas);
                        List<long> _medicineIds = datas.Select(p => p.MEDICINE_ID).Distinct().ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _medicineIds;
                        var medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        if (medicines != null && medicines.Count > 0)
                        {
                            _HisMedicines.AddRange(medicines);
                        }
                    }
                }

                foreach (var item in lstData)
                {
                    bool ICheck = false;
                    string mess = "Mã nhập " + item.CODE + " thiếu ";

                    PhieuNhap phieunhap = new PhieuNhap();
                    phieunhap.MaPhieu = item.CODE;//50.x
                    if (string.IsNullOrEmpty(phieunhap.MaPhieu))
                    {
                        ICheck = true;
                        mess += "MaPhieu - IMP_MEST_CODE,";
                    }
                    phieunhap.NgayNhap = item.TDL_INTRUCTION_TIME != null ? item.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) : "";//12.x 
                    if (string.IsNullOrEmpty(phieunhap.NgayNhap))
                    {
                        ICheck = true;
                        mess += "NgayNhap - TDL_INTRUCTION_TIME,";
                    }
                    if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        phieunhap.LoaiPhieuNhap = 1; // 1.Nhập từ nhà cùng cấp 2.Khách trả 3.Nhập tồn
                    }
                    else if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                        || item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL)
                    {
                        phieunhap.LoaiPhieuNhap = 2;
                    }
                    else
                    {
                        phieunhap.LoaiPhieuNhap = 3;
                    }
                    phieunhap.GhiChu = item.DESCRIPTION;
                    if (item.SUPPLIER_ID > 0
                        && item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        var data = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == item.SUPPLIER_ID);
                        phieunhap.TenCoSoCungCap = data != null ? data.SUPPLIER_NAME : "";//500  Tên cơ sở cung cấp thuốc (Nếu là  nhập từ nhà cung cấp)
                    }
                    phieunhap.MaCoSo = _AccountConnectADO.MaCoSo;// BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;//50.x
                    if (string.IsNullOrEmpty(phieunhap.MaCoSo))
                    {
                        ICheck = true;
                        mess += "MaCoSo - HEIN_MEDI_ORG_CODE,";
                    }

                    phieunhap.ChiTietPhieuNhap = new List<ChiTietPhieuNhap>();

                    var dataMedicines = _HisImpMestMedicines.Where(p => p.IMP_MEST_ID == item.ID).ToList();
                    if (dataMedicines != null && dataMedicines.Count > 0)
                    {
                        string messThuoc = " ---------- EXP_MEST_ID == " + item.ID + "---";
                        foreach (var itemThuoc in dataMedicines)
                        {
                            var medicine = _HisMedicines.FirstOrDefault(p => p.ID == itemThuoc.MEDICINE_ID);
                            if (medicine == null)
                                continue;
                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == medicine.MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            ChiTietPhieuNhap _chiTietPhieuNhap = new ChiTietPhieuNhap();
                            _chiTietPhieuNhap.DonViTinh = medicineType.SERVICE_UNIT_NAME;//200.x Tên đơn vị tính nhỏ nhất của thuốc
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.DonViTinh))
                            {
                                ICheck = true;
                                messThuoc += "DonViTinh - SERVICE_UNIT_NAME,";
                            }

                            _chiTietPhieuNhap.So_Dklh = medicineType.REGISTER_NUMBER;//50.x  Số đăng kí lưu hành của thuốc
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.So_Dklh))
                            {
                                ICheck = true;
                                messThuoc += "So_Dklh - REGISTER_NUMBER,";
                            }
                            _chiTietPhieuNhap.SoLo = medicine.PACKAGE_NUMBER;//50.x
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.SoLo))
                            {
                                ICheck = true;
                                messThuoc += "SoLo - PACKAGE_NUMBER,";
                            }
                            _chiTietPhieuNhap.HanDung = (medicine.EXPIRED_DATE != null
                                && medicine.EXPIRED_DATE > 0) ? medicine.EXPIRED_DATE.ToString().Substring(0, 8) : "";//12.x
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.HanDung))
                            {
                                ICheck = true;
                                messThuoc += "HanDung - EXPIRED_DATE,";
                            }
                            _chiTietPhieuNhap.NgaySanXuat = "";
                            _chiTietPhieuNhap.MaThuoc = medicineType.MEDICINE_NATIONAL_CODE;//50.x
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.MaThuoc))
                            {
                                ICheck = true;
                                messThuoc += "MaThuoc - MEDICINE_NATIONAL_CODE,";
                            }
                            //_chiTietPhieuNhap.SoLuong = Inventec.Common.TypeConvert.Parse.ToInt32(itemThuoc.AMOUNT.ToString());//x  Số lượng thuốc quy ra đơn vị tính nhỏ nhất
                            _chiTietPhieuNhap.SoLuong = (int)Math.Round(itemThuoc.AMOUNT, 0, MidpointRounding.AwayFromZero);
                            //_chiTietPhieuNhap.SoLuong = itemThuoc.AMOUNT;
                            if (_chiTietPhieuNhap.SoLuong <= 0)
                            {
                                ICheck = true;
                            _chiTietPhieuNhap.SoLuong = (int)Math.Round(itemThuoc.AMOUNT, 0, MidpointRounding.AwayFromZero);
                                messThuoc += "SoLuong <= 0 - ,";
                            }
                            //_chiTietPhieuNhap.DonGia = Inventec.Common.TypeConvert.Parse.ToInt32(medicineType.IMP_PRICE != null ? medicineType.IMP_PRICE.ToString() : "0");//Đơn giá thuốc
                            //_chiTietPhieuNhap.SoLuong = (int)Math.Round(medicineType.IMP_PRICE ?? 0, 0, MidpointRounding.AwayFromZero);
                            //_chiTietPhieuNhap.DonGia = medicineType.IMP_PRICE ?? 0;
                            _chiTietPhieuNhap.TenThuoc = medicineType.MEDICINE_TYPE_NAME;//500.x
                            if (string.IsNullOrEmpty(_chiTietPhieuNhap.TenThuoc))
                            {
                                ICheck = true;
                                messThuoc += "TenThuoc - MEDICINE_TYPE_NAME,";
                            }

                            phieunhap.ChiTietPhieuNhap.Add(_chiTietPhieuNhap);
                        }
                        mess += messThuoc + "_____";
                    }
                    if (ICheck)
                    {
                        Inventec.Common.Logging.LogSystem.Error(mess);
                        continue;
                    }

                    if (phieunhap.ChiTietPhieuNhap != null && phieunhap.ChiTietPhieuNhap.Count > 0)
                        CallPhieuNhapQuocGia(item.ID, phieunhap);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallDonThuocQuocGia(long _expMestId, DonThuoc donthuoc)
        {
            try
            {
                var _Result_don_thuoc = ConnectProcess.DonThuoc(donthuoc, this._ThaoTac);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Result_don_thuoc), _Result_don_thuoc));
                if (_Result_don_thuoc != null && ConnectProcess.IsSuccessCode(_Result_don_thuoc.Ma))
                {
                    HIS_EXP_MEST ado = new HIS_EXP_MEST();
                    ado.NATIONAL_EXP_MEST_CODE = _Result_don_thuoc.MaDonThuocQuocGia;
                    ado.ID = _expMestId;
                    this._ExpMestUpdates.Add(ado);
                    this._ExpMestIdCancels.Add(_expMestId);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => donthuoc), donthuoc));
                    __StrMess.Add(_Result_don_thuoc.TinNhan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallHoaDonQuocGia(long _transactionId, HoaDon hoadon, bool IsTransaction)
        {
            try
            {
                var _Result_hoa_don = ConnectProcess.HoaDon(hoadon, this._ThaoTac);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Result_hoa_don), _Result_hoa_don));
                if (_Result_hoa_don != null && ConnectProcess.IsSuccessCode(_Result_hoa_don.Ma))
                {
                    if (IsTransaction)
                    {
                        HIS_TRANSACTION ado = new HIS_TRANSACTION();
                        ado.NATIONAL_TRANSACTION_CODE = _Result_hoa_don.MaHoaDonQuocGia;
                        ado.ID = _transactionId;
                        this._TransactionUpdates.Add(ado);
                        this._TransactionIdCancels.Add(_transactionId);
                    }
                    else
                    {
                        HIS_EXP_MEST ado = new HIS_EXP_MEST();
                        ado.NATIONAL_EXP_MEST_CODE = _Result_hoa_don.MaHoaDonQuocGia;
                        ado.ID = _transactionId;
                        this._ExpMestUpdates.Add(ado);
                        this._ExpMestIdCancels.Add(_transactionId);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hoadon), hoadon));
                    __StrMess.Add(_Result_hoa_don.TinNhan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallPhieuXuatQuocGia(long _expMestId, PhieuXuat phieuxuat)
        {
            try
            {
                var _Result_phieu_xuat = ConnectProcess.PhieuXuat(phieuxuat, this._ThaoTac);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Result_phieu_xuat), _Result_phieu_xuat));
                if (_Result_phieu_xuat != null && ConnectProcess.IsSuccessCode(_Result_phieu_xuat.Ma))
                {
                    HIS_EXP_MEST ado = new HIS_EXP_MEST();
                    ado.NATIONAL_EXP_MEST_CODE = _Result_phieu_xuat.MaPhieuXuatQuocGia;
                    ado.ID = _expMestId;
                    this._ExpMestUpdates.Add(ado);
                    this._ExpMestIdCancels.Add(_expMestId);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => phieuxuat), phieuxuat));
                    __StrMess.Add(_Result_phieu_xuat.TinNhan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallPhieuNhapQuocGia(long _impMestId, PhieuNhap phieunhap)
        {
            try
            {
                var _Result_phieu_nhap = ConnectProcess.PhieuNhap(phieunhap, this._ThaoTac);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Result_phieu_nhap), _Result_phieu_nhap));
                if (_Result_phieu_nhap != null && ConnectProcess.IsSuccessCode(_Result_phieu_nhap.Ma))
                {
                    HIS_IMP_MEST ado = new HIS_IMP_MEST();
                    ado.NATIONAL_IMP_MEST_CODE = _Result_phieu_nhap.MaPhieuNhapQuocGia;
                    ado.ID = _impMestId;
                    this._ImpMestUpdates.Add(ado);
                    this._ImpMestIdCancels.Add(_impMestId);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => phieunhap), phieunhap));
                    __StrMess.Add(_Result_phieu_nhap.TinNhan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                this.LoadOnClick(ThaoTac.TaoMoi);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNoSend_Click(object sender, EventArgs e)
        {
            try
            {
                this.LoadOnClick(ThaoTac.Xoa);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
