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
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
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
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisSuggestedPayment.Run
{
    public partial class frmHisSuggestedPayment : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        int action = -1;
        List<ImpMestADO> _ImpMestADOs { get; set; }
        DelegateRefreshData _ref = null;

        bool isCheckAll = true;

        V_HIS_IMP_MEST_PROPOSE _ImpMestPropose { get; set; }
        public List<V_HIS_IMP_MEST_PROPOSE> _ImpMestPropose_ { get; set; }
        public List<V_HIS_IMP_MEST_PAY> _HisImpMestPays { get; set; }
        public List<V_HIS_IMP_MEST> _HisImpMests { get; set; }
        HisImpMestProposeResultSDO outSave = new HisImpMestProposeResultSDO();
        string lblMaDeNghis;
        public frmHisSuggestedPayment()
        {
            InitializeComponent();
        }
        public frmHisSuggestedPayment(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.action = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisSuggestedPayment(Inventec.Desktop.Common.Modules.Module currentModule, DelegateRefreshData _Ref)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._ref = _Ref;
                this.action = 2;
                txtMaNhaCungCap.Enabled = true;
                cboNhaCungCap.Enabled = true;
                btnPrint.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisSuggestedPayment(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_IMP_MEST_PROPOSE _propose, int _action)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._ImpMestPropose = _propose;
                this.action = _action;
                cboNhaCungCap.ReadOnly = true;
                txtMaNhaCungCap.ReadOnly = true;
                btnPrint.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void frmHisSuggestedPayment_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                ValidationControlMaxLength(txtImpMestProposeName, 4000);
                ComboCtyXuatHoaDon();
                ComboNhaCC();
                ComboNguoiDeNghi();
                ComboHopDong();
                InitMenuToButtonPrint();
                SetDefaultFilter();
                LoadDataImpMests();
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        public void ComboNhaCC()
        {
            try
            {
                List<HIS_SUPPLIER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SUPPLIER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboNhaCungCap, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboCtyXuatHoaDon()
        {
            try
            {
                long supplier = 0;
                long contract = 0;
                if (cboNhaCungCap.EditValue != null)
                    supplier = (long)cboNhaCungCap.EditValue;
                if(cboSUPPLIER.EditValue != null)
                    contract = (long)cboSUPPLIER.EditValue;
                List<HIS_SUPPLIER> Suppliers = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (contract != 0 || supplier != 0)
                {
                    HisMedicalContractFilter MedicalContractFilter = new HisMedicalContractFilter();
                    if (supplier != 0)
                        MedicalContractFilter.SUPPLIER_ID = supplier;
                    if(contract != 0)
                        MedicalContractFilter.ID = contract;
                    var MedicalContract = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/Get", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                    var mc = MedicalContract.Where(o => o.DOCUMENT_SUPPLIER_ID != null);
                    if(mc != null)
                        Suppliers = Suppliers.Where(o => mc.ToList().Exists(p=>p.DOCUMENT_SUPPLIER_ID == o.ID)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboDocuSup, Suppliers, controlEditorADO);
                cboDocuSup.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ComboHopDong()
        {
            try
            {
                List<HIS_MEDICAL_CONTRACT> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICAL_CONTRACT>();
                if(cboNhaCungCap.EditValue != null)
                {
                    data = data.Where(o => o.SUPPLIER_ID == (long)cboNhaCungCap.EditValue).ToList();
                   
                }
                if (cboDocuSup.EditValue != null)
                {
                    data = data.Where(o => o.DOCUMENT_SUPPLIER_ID == (long)cboDocuSup.EditValue).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICAL_CONTRACT_CODE", "", 75, 1));
                columnInfos.Add(new ColumnInfo("MEDICAL_CONTRACT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICAL_CONTRACT_NAME", "ID", columnInfos, false, 325);
                ControlEditorLoader.Load(this.cboSUPPLIER, data.ToList(), controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboNguoiDeNghi()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboNguoiDeNghi, data, controlEditorADO);

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                cboNguoiDeNghi.EditValue = loginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool isNotEdit = false;
        private int positionHandleControlLeft;

        private void LoadDataImpMests()
        {
            try
            {
                WaitingManager.Show();
                gridControls.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisImpMestViewFilter _Filter = new MOS.Filter.HisImpMestViewFilter();

                _Filter.ORDER_FIELD = "IMP_MEST_CODE";
                _Filter.ORDER_DIRECTION = "DESC";



                if (this._ImpMestPropose != null)
                {
                    lblMaDeNghi.Text = _ImpMestPropose.IMP_MEST_PROPOSE_CODE;
                    txtImpMestProposeName.Text = _ImpMestPropose.IMP_MEST_PROPOSE_NAME;
                    cboDocuSup.EditValue = _ImpMestPropose.DOCUMENT_SUPPLIER_ID;
                    _HisImpMestPays = new List<V_HIS_IMP_MEST_PAY>();
                    MOS.Filter.HisImpMestPayViewFilter _impMestPayFilter = new MOS.Filter.HisImpMestPayViewFilter();
                    _impMestPayFilter.IMP_MEST_PROPOSE_ID = _ImpMestPropose.ID;
                    //if (cboSUPPLIER.EditValue != null)
                    //{
                    //    _Filter.MEDICAL_CONTRACT_ID = cboSUPPLIER.EditValue;
                    //}
                    _HisImpMestPays = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_PAY>>("api/HisImpMestPay/GetView", ApiConsumers.MosConsumer, _impMestPayFilter, paramCommon);//neu đã phát sinh thanh toán thì k cho sửa
                    isNotEdit = false;
                    if (_HisImpMestPays != null && _HisImpMestPays.Count > 0)
                    {
                        isNotEdit = true;
                        lblDaThanhToan.Text = Inventec.Common.Number.Convert.NumberToString(_HisImpMestPays.Sum(p => p.AMOUNT), HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    }
                    //LoadTatCaRaDeSua
                    if (this.action == 3 || isNotEdit)
                    {
                        // lciGridControl.Enabled = false;
                        btnSave.Enabled = false;
                        _Filter.IMP_MEST_PROPOSE_ID = _ImpMestPropose.ID;
                    }
                    else
                    {
                        _Filter.SUPPLIER_ID = _ImpMestPropose.SUPPLIER_ID;
                    }
                    this.cboNhaCungCap.EditValue = _ImpMestPropose.SUPPLIER_ID;
                    this.cboNguoiDeNghi.EditValue = _ImpMestPropose.PROPOSE_LOGINNAME;
                    lblSoTien.Text = Inventec.Common.Number.Convert.NumberToString(this._ImpMestPropose.TOTAL_PAY ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    //
                }
                else if (cboNhaCungCap.EditValue != null)
                    {
                        _Filter.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboNhaCungCap.EditValue.ToString());
                    }

                if (!String.IsNullOrWhiteSpace(txtImpMestCode.Text))
                {
                    string code = txtImpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpMestCode.Text = code;
                    }
                    _Filter.IMP_MEST_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtDocumentNumber.Text))
                {
                    _Filter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text.Trim();
                }
                else
                {
                    _Filter.KEY_WORD = txtKeyword.Text.Trim();
                    if (dtDocumentDateFrom.EditValue != null && dtDocumentDateFrom.DateTime != DateTime.MinValue)
                    {
                        _Filter.DOCUMENT_DATE_FROM = Convert.ToInt64(dtDocumentDateFrom.DateTime.ToString("yyyMMdd") + "000000");
                    }
                    if (dtDocumentDateTo.EditValue != null && dtDocumentDateTo.DateTime != DateTime.MinValue)
                    {
                        _Filter.DOCUMENT_DATE_TO = Convert.ToInt64(dtDocumentDateTo.DateTime.ToString("yyyMMdd") + "000000");
                    }
                }

                var dataMedi = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                _Filter.MEDI_STOCK_ID = dataMedi.ID;
                if (cboSUPPLIER.EditValue != null)
                {
                    _Filter.MEDICAL_CONTRACT_ID = (long)cboSUPPLIER.EditValue;
                }
                var results = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, _Filter, paramCommon);
                if (cboDocuSup.EditValue != null && results != null && results.Count > 0)
                { 
                    var docSup = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList().Where(o=>o.ID == (long)cboDocuSup.EditValue).FirstOrDefault();
                    results = results.Where(o => o.DOCUMENT_SUPPLIER_CODE == docSup.SUPPLIER_CODE).ToList();
                }
                if (results != null && results.Count > 0)
                {
                    if (this.action == 1)
                    {
                        results = results.Where(p => p.IMP_MEST_PROPOSE_ID == null).ToList();//xet voi tao moi
                    }
                    else if (this.action == 2 && this._ImpMestPropose != null)
                    {
                        results = results.Where(p => p.IMP_MEST_PROPOSE_ID == null || this._ImpMestPropose.ID == p.IMP_MEST_PROPOSE_ID).ToList();
                        _HisImpMests = results.Where(p => this._ImpMestPropose.ID == p.IMP_MEST_PROPOSE_ID).ToList();
                    }
                    else if (this.action == 3 && this._ImpMestPropose != null)
                    {
                        results = results.Where(p => this._ImpMestPropose.ID == p.IMP_MEST_PROPOSE_ID).ToList();
                        _HisImpMests = results.Where(p => this._ImpMestPropose.ID == p.IMP_MEST_PROPOSE_ID).ToList();
                    }

                }
                _ImpMestADOs = new List<ImpMestADO>();

                if (results != null && results.Count > 0)
                {
                    _ImpMestADOs.AddRange((from i in results select new ImpMestADO(i)).ToList());
                    _ImpMestADOs = _ImpMestADOs.OrderBy(p => p.IMP_MEST_PROPOSE_ID ?? 9999).ThenByDescending(p => p.IMP_MEST_CODE).ToList();
                    if (this._ImpMestPropose == null)
                        lblSoTien.Text = Inventec.Common.Number.Convert.NumberToString(results.Sum(p => (p.DOCUMENT_PRICE ?? 0)), HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                }

                gridControls.BeginUpdate();
                gridControls.DataSource = _ImpMestADOs;
                gridControls.EndUpdate();
                gridViews.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViews.OptionsSelection.EnableAppearanceFocusedRow = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void gridViews_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_IMP_MEST data = (V_HIS_IMP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOCUMENT_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOCUMENT_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "THANH_TIEN_STR")
                    {
                        e.Value = data.DOCUMENT_PRICE - data.DISCOUNT;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME.Value);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Search_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNhaCungCap_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboNhaCungCap.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboNhaCungCap.EditValue.ToString()));
                    if (data != null)
                    {
                        txtMaNhaCungCap.Text = data.SUPPLIER_CODE;
                    }
                    txtSUPPLIER.Text = null;
                    cboSUPPLIER.EditValue = null;
                }

                ComboCtyXuatHoaDon();

                ComboHopDong();
                this.LoadDataImpMests();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNhaCungCap_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboNhaCungCap.EditValue = null;
                    txtMaNhaCungCap.Text = "";
                    txtMaNhaCungCap.Focus();
                }
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
                _ImpMestPropose_ = new List<V_HIS_IMP_MEST_PROPOSE>();
                if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                {
                    var datas = this._ImpMestADOs.Where(p => p.IsCheck).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        HisImpMestProposeSDO sdo = new HisImpMestProposeSDO();
                        sdo.ImpMestIds = datas.Select(p => p.ID).Distinct().ToList();
                        sdo.WorkingRoomId = this.currentModule.RoomId;
                        sdo.SupplierId = datas.FirstOrDefault().SUPPLIER_ID ?? 0;
                        sdo.ImpMestProposeName = !string.IsNullOrEmpty(txtImpMestProposeName.Text.Trim()) ? txtImpMestProposeName.Text.Trim() : null;


                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        outSave = new HisImpMestProposeResultSDO();
                        if (this.action == GlobalVariables.ActionAdd)
                        {
                            outSave = new HisImpMestProposeResultSDO();
                            outSave = new BackendAdapter(param).Post<HisImpMestProposeResultSDO>("api/HisImpMestPropose/Create", ApiConsumers.MosConsumer, sdo, param);
                            if (outSave != null)
                            {
                                success = true;
                                this.action = GlobalVariables.ActionEdit;
                                btnPrint.Enabled = true;
                            }
                        }
                        else if (this.action == GlobalVariables.ActionEdit)
                        {
                            sdo.Id = this._ImpMestPropose.ID;
                            outSave = new HisImpMestProposeResultSDO();
                            outSave = new BackendAdapter(param).Post<HisImpMestProposeResultSDO>("api/HisImpMestPropose/Update", ApiConsumers.MosConsumer, sdo, param);
                            if (outSave != null)
                            {
                                success = true;
                                this.action = GlobalVariables.ActionEdit;
                            }
                        }
                        if (success)
                        {
                            if (this._ref != null)
                                this._ref();
                            _HisImpMestPays = outSave.HisImpMestPays;
                            _HisImpMests = outSave.HisImpMests;
                            _ImpMestPropose_ = outSave.ImpMestProposes;
                           
                            if (_ImpMestPropose_!= null && _ImpMestPropose_.Count > 0)
                            {
                                lblMaDeNghi.Text = string.Join(",", _ImpMestPropose_.Select(o => o.IMP_MEST_PROPOSE_CODE));
                                lblMaDeNghi.ToolTip = string.Join(",", _ImpMestPropose_.Select(o => o.IMP_MEST_PROPOSE_CODE));
                                this.layoutControlItem1.OptionsToolTip.ToolTip = string.Join(",", _ImpMestPropose_.Select(o => o.IMP_MEST_PROPOSE_CODE));

                                txtImpMestProposeName.Text = string.Join(",", _ImpMestPropose_.Select(o => o.IMP_MEST_PROPOSE_NAME));
                                cboDocuSup.EditValue = _ImpMestPropose_[0].DOCUMENT_SUPPLIER_ID;
                            }
                            //lblMaDeNghi.Text = _ImpMestPropose.IMP_MEST_PROPOSE_CODE;

                            lblSoTien.Text = Inventec.Common.Number.Convert.NumberToString(_HisImpMests.Sum(p => (p.DOCUMENT_PRICE ?? 0 - p.DISCOUNT ?? 0)), HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            if (_HisImpMestPays != null && _HisImpMestPays.Count > 0)
                            {
                                lblDaThanhToan.Text = Inventec.Common.Number.Convert.NumberToString(_HisImpMestPays.Sum(p => p.AMOUNT), HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu được chọn", "Thông báo");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViews_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_IMP_MEST)gridViews.GetRow(e.RowHandle);
                    // string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "IsCheck")
                    {
                        if (this.action == 3)
                        {
                            e.RepositoryItem = repositoryItemCheck__D;
                        }
                        else
                            e.RepositoryItem = repositoryItemCheck__E;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViews_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.action == 3 || isNotEdit)
                    return;
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                var dataRow = (V_HIS_IMP_MEST)gridViews.GetRow(hi.RowHandle);
                                if (dataRow != null)
                                {
                                    checkEdit.Checked = !checkEdit.Checked;
                                    view.CloseEditor();
                                    if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                                    {
                                        var dataChecks = this._ImpMestADOs.Where(p => p.IsCheck == true).ToList();
                                        if (dataChecks != null && dataChecks.Count > 0)
                                        {
                                            gridColumnIsCheck.Image = imageListIcon.Images[5];
                                        }
                                        else
                                        {
                                            gridColumnIsCheck.Image = imageListIcon.Images[6];
                                        }
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                        else if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit))
                        {
                            //TODO
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnIsCheck.Image = imageListIcon.Images[5];
                            gridViews.BeginUpdate();
                            if (this._ImpMestADOs == null)
                                this._ImpMestADOs = new List<ImpMestADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._ImpMestADOs)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnIsCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._ImpMestADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViews.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!btnPrint.Enabled)
        //            return;
        //        this.PrintProcess(PrintType.IN);


        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void barButton__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaNhaCungCap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadNhaCungCap(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadNhaCungCap(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNhaCungCap.Focus();
                    cboNhaCungCap.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.SUPPLIER_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboNhaCungCap.EditValue = data[0].ID;
                            cboNhaCungCap.Properties.Buttons[1].Visible = true;
                            txtMaNhaCungCap.Focus();
                            txtMaNhaCungCap.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.SUPPLIER_CODE == searchCode);
                            if (search != null)
                            {
                                cboNhaCungCap.EditValue = search.ID;
                                cboNhaCungCap.Properties.Buttons[1].Visible = true;
                                txtMaNhaCungCap.Focus();
                                txtMaNhaCungCap.SelectAll();
                            }
                            else
                            {
                                cboNhaCungCap.EditValue = null;
                                cboNhaCungCap.Focus();
                                cboNhaCungCap.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboNhaCungCap.EditValue = null;
                        cboNhaCungCap.Focus();
                        cboNhaCungCap.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            try
            {
                btnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemDeNghi = new DXMenuItem("Phiếu đề nghị thanh toán", new EventHandler(OnClickIn));
                itemDeNghi.Tag = PrintType.IN__DE_NGHI_THANH_TOAN;
                menu.Items.Add(itemDeNghi);

                DXMenuItem itemBienBanNghiemThu = new DXMenuItem("Phiếu nghiệm thu thanh toán", new EventHandler(OnClickIn));
                itemBienBanNghiemThu.Tag = PrintType.IN__BIEN_BAN_NGHIEM_THU;
                menu.Items.Add(itemBienBanNghiemThu);

                btnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void OnClickIn(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMestCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
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
                if (!btnFind.Enabled) return;
                LoadDataImpMests();
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
                if (!btnRefresh.Enabled) return;
                LoadDataImpMests();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultFilter()
        {
            try
            {
                txtKeyword.Text = "";
                txtImpMestCode.Text = "";
                txtDocumentNumber.Text = "";
                if (this._ImpMestPropose != null)
                {
                    dtDocumentDateFrom.EditValue = null;
                    dtDocumentDateTo.EditValue = null;
                }
                else
                {
                    dtDocumentDateFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    dtDocumentDateTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Find_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void barButton__Refresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        private void cboSUPPLIER_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSUPPLIER.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICAL_CONTRACT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSUPPLIER.EditValue.ToString()));
                    if (data != null)
                    {
                        txtSUPPLIER.Text = data.MEDICAL_CONTRACT_CODE;
                    }

                }

                ComboCtyXuatHoaDon();
                this.LoadDataImpMests();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSUPPLIER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSUPPLIER.EditValue = null;
                    txtSUPPLIER.Text = "";
                    this.LoadDataImpMests();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSUPPLIER_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtSUPPLIER.Text != null)
            //    {
            //        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICAL_CONTRACT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(txtSUPPLIER.Text.ToString()));
            //        if (data != null)
            //        {
            //            if (data.ID != null)
            //            {
            //                cboSUPPLIER.EditValue = data.ID;
            //            }
            //        }
            //        this.LoadDataImpMests();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void txtSUPPLIER_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadHD(strValue);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }
        internal void LoadHD(string searchCode)
        {
            if (searchCode != null)
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICAL_CONTRACT>().Where(o => o.MEDICAL_CONTRACT_CODE.Contains(searchCode)).ToList(); ;
                if (data != null)
                {
                    if (data[0].ID != null)
                    {
                        cboSUPPLIER.EditValue = data[0].ID;
                    }
                }
                this.LoadDataImpMests();
            }
        }

        private void cboDocuSup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboDocuSup.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }
        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {

            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocuSup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    var dataSource = cboDocuSup.Properties.DataSource as List<HIS_SUPPLIER>;
                    if (dataSource != null && dataSource.Count > 0)
                    {
                        if (string.IsNullOrEmpty(txtDocuSup.Text.Trim()))
                        {
                            var item = dataSource.FirstOrDefault(o => o.SUPPLIER_CODE == txtDocuSup.Text.Trim());
                            if (item != null)
                            {
                                cboDocuSup.EditValue = item;
                            }
                            else
                            {
                                cboDocuSup.Focus();
                                cboDocuSup.ShowPopup();
                            }
                        }
                        else
                        {
                            cboDocuSup.Focus();
                            cboDocuSup.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDocuSup.Focus();
                        cboDocuSup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDocuSup_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var dataSource = cboDocuSup.Properties.DataSource as List<HIS_SUPPLIER>;
                if (dataSource != null && dataSource.Count > 0)
                {
                    if (cboDocuSup.EditValue != null)
                    {

                        var item = dataSource.FirstOrDefault(o => o.ID == (long)cboDocuSup.EditValue);
                        txtDocuSup.Text = item.SUPPLIER_CODE;
                    }
                    else
                    {
                        txtDocuSup.Text = null;
                    }
                }
                else
                {
                    txtDocuSup.Text = null;
                }

                ComboHopDong();
                this.LoadDataImpMests();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
