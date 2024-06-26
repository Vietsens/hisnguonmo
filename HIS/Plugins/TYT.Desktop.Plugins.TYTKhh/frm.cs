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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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
using TYT.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.TYTKhh
{
    public partial class frm : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT _Patient { get; set; }

        TYT_KHH _TYT_KHH { get; set; }

        string _PatientCode = "";

        int action = 0;

        public frm()
        {
            InitializeComponent();
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule, TYT.EFMODEL.DataModels.TYT_KHH __TYT_KHH)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this._TYT_KHH = __TYT_KHH;
            this._PatientCode = __TYT_KHH.PATIENT_CODE;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.action = 2;
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this._Patient = _patient;
            this._PatientCode = _patient.PATIENT_CODE;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }

            this.action = 1;
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

        private void frm_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefault();

                FillDataToControl();

                LoadListKhh();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefault()
        {
            try
            {
                chkDocThan.Checked = false;
                chkDatVong.Checked = false;
                chkTrietSan.Checked = false;
                chkThuocVien.Checked = false;
                chkThuocTiem.Checked = false;
                chkThuocCay.Checked = false;
                chkBaoCaoSu.Checked = false;
                txtBienChung.Text = "";
                chkSanPhuTuVong.Checked = false;
                txtGhiChu.Text = "";
                txtNguoiThucHien.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                if (this._TYT_KHH != null && this._TYT_KHH.ID > 0)
                {
                    this.action = GlobalVariables.ActionEdit;
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    if (this._TYT_KHH.IS_SINGLE.HasValue && this._TYT_KHH.IS_SINGLE == 1)
                    {
                        chkDocThan.Checked = true;
                    }
                    else
                        chkDocThan.Checked = false;
                    if (this._TYT_KHH.IS_IUCD.HasValue && this._TYT_KHH.IS_IUCD == 1)
                    {
                        chkDatVong.Checked = true;
                    }
                    else
                        chkDatVong.Checked = false;
                    if (this._TYT_KHH.IS_STERILIZE.HasValue && this._TYT_KHH.IS_STERILIZE == 1)
                    {
                        chkTrietSan.Checked = true;
                    }
                    else
                        chkTrietSan.Checked = false;
                    if (this._TYT_KHH.IS_PILL_V.HasValue && this._TYT_KHH.IS_PILL_V == 1)
                    {
                        chkThuocVien.Checked = true;
                    }
                    else
                        chkThuocVien.Checked = false;
                    if (this._TYT_KHH.IS_PILL_T.HasValue && this._TYT_KHH.IS_PILL_T == 1)
                    {
                        chkThuocTiem.Checked = true;
                    }
                    else
                        chkThuocTiem.Checked = false;
                    if (this._TYT_KHH.IS_PILL_C.HasValue && this._TYT_KHH.IS_PILL_C == 1)
                    {
                        chkThuocCay.Checked = true;
                    }
                    else
                        chkThuocCay.Checked = false;
                    if (this._TYT_KHH.IS_CONDOM.HasValue && this._TYT_KHH.IS_CONDOM == 1)
                    {
                        chkBaoCaoSu.Checked = true;
                    }
                    else
                        chkBaoCaoSu.Checked = false;
                    txtBienChung.Text = this._TYT_KHH.OBSTETRIC_COMPLICATION;
                    if (this._TYT_KHH.IS_DEATH.HasValue && this._TYT_KHH.IS_DEATH == 1)
                    {
                        chkSanPhuTuVong.Checked = true;
                    }
                    else
                        chkSanPhuTuVong.Checked = false;
                    txtNguoiThucHien.Text = this._TYT_KHH.EXECUTE_NAME;
                    txtGhiChu.Text = this._TYT_KHH.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListKhh()
        {
            try
            {
                gridControlKhh.DataSource = null;
                TYT.Filter.TytKhhFilter filter = new Filter.TytKhhFilter();
                if (!string.IsNullOrEmpty(this._PatientCode))
                {
                    filter.PATIENT_CODE__EXACT = this._PatientCode;
                }
                else
                {
                    return;
                }
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var datas = new BackendAdapter(new CommonParam()).Get<List<TYT.EFMODEL.DataModels.TYT_KHH>>("api/TytKhh/Get", ApiConsumers.TytConsumer, filter, null);
                if (datas != null && datas.Count > 0)
                {
                    string branchCode = BranchDataWorker.Branch.BRANCH_CODE;
                    if (!string.IsNullOrEmpty(branchCode))
                    {
                        datas = datas.Where(p => p.BRANCH_CODE == branchCode).ToList();
                        gridControlKhh.DataSource = datas;
                    }
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
                bool success = false;
                CommonParam param = new CommonParam();

                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                btnSave.Focus();
                if (this._TYT_KHH != null && this._TYT_KHH.ID > 0)
                {
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                }
                else
                {
                    if (this._Patient == null || this._Patient.ID <= 0)
                    {
                        MOS.Filter.HisPatientViewFilter _patientFilter = new MOS.Filter.HisPatientViewFilter();
                        _patientFilter.PATIENT_CODE__EXACT = this._PatientCode;
                        this._Patient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, _patientFilter, null).FirstOrDefault();
                    }
                    this._TYT_KHH = new TYT_KHH();
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_KHH>(this._TYT_KHH, this._Patient);
                    this._TYT_KHH.BHYT_NUMBER = this._Patient.TDL_HEIN_CARD_NUMBER;
                    this._TYT_KHH.PERSON_ADDRESS = this._Patient.VIR_ADDRESS;
                    this._TYT_KHH.VIR_PERSON_NAME = this._Patient.VIR_PATIENT_NAME;
                    this._TYT_KHH.ID = 0;
                }
                if (chkDocThan.Checked)
                {
                    this._TYT_KHH.IS_SINGLE = 1;
                }
                else
                    this._TYT_KHH.IS_SINGLE = null;
                if (chkDatVong.Checked)
                {
                    this._TYT_KHH.IS_IUCD = 1;
                }
                else
                    this._TYT_KHH.IS_IUCD = null;
                if (chkTrietSan.Checked)
                {
                    this._TYT_KHH.IS_STERILIZE = 1;
                }
                else
                    this._TYT_KHH.IS_STERILIZE = null;
                if (chkThuocVien.Checked)
                {
                    this._TYT_KHH.IS_PILL_V = 1;
                }
                else
                    this._TYT_KHH.IS_PILL_V = null;
                if (chkThuocTiem.Checked)
                {
                    this._TYT_KHH.IS_PILL_T = 1;
                }
                else
                    this._TYT_KHH.IS_PILL_T = null;
                if (chkThuocCay.Checked)
                {
                    this._TYT_KHH.IS_PILL_C = 1;
                }
                else
                    this._TYT_KHH.IS_PILL_C = null;
                if (chkBaoCaoSu.Checked)
                {
                    this._TYT_KHH.IS_CONDOM = 1;
                }
                else
                    this._TYT_KHH.IS_CONDOM = null;
                if (chkSanPhuTuVong.Checked)
                {
                    this._TYT_KHH.IS_DEATH = 1;
                }
                else
                    this._TYT_KHH.IS_DEATH = null;

                this._TYT_KHH.OBSTETRIC_COMPLICATION = txtBienChung.Text;
                this._TYT_KHH.NOTE = txtGhiChu.Text;

                this._TYT_KHH.EXECUTE_NAME = txtNguoiThucHien.Text;
                this._TYT_KHH.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_KHH _result = new TYT_KHH();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    _result = new BackendAdapter(param).Post<TYT_KHH>("api/TytKhh/Create", ApiConsumers.TytConsumer, this._TYT_KHH, param);
                }
                else
                {
                    _result = new BackendAdapter(param).Post<TYT_KHH>("api/TytKhh/Update", ApiConsumers.TytConsumer, this._TYT_KHH, param);
                }

                if (_result != null && _result.ID > 0)
                {
                    success = true;
                    this._TYT_KHH = _result;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    this.LoadListKhh();
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
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
                this._TYT_KHH = new TYT_KHH();
                this.action = 1;
                this.SetDefault();
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void chkDocThan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDatVong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDatVong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTrietSan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTrietSan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThuocVien.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuocVien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThuocTiem.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuocTiem_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThuocCay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuocCay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBaoCaoSu.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBaoCaoSu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBienChung.Focus();
                    txtBienChung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBienChung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkSanPhuTuVong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSanPhuTuVong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNguoiThucHien.Focus();
                    txtNguoiThucHien.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNguoiThucHien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGhiChu.Focus();
                    txtGhiChu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (TYT_KHH)gridViewKhh.GetFocusedRow();
                if (data != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/TytKhh/Delete", ApiConsumers.TytConsumer, data.ID, param);
                    if (success)
                    {
                        btnRefesh_Click(null, null);
                        this._TYT_KHH = new TYT_KHH();
                        LoadListKhh();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Active_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (TYT_KHH)gridViewKhh.GetFocusedRow();
                if (data != null)
                {
                    TYT_KHH success = new TYT_KHH();
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<TYT_KHH>("api/TytKhh/ChangeLock", ApiConsumers.TytConsumer, data, param);
                    Inventec.Common.Logging.LogSystem.Debug("api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    if (success!=null)
                    {
                        btnRefesh_Click(null, null);
                        this._TYT_KHH = new TYT_KHH();
                        LoadListKhh();
                    }
                    MessageManager.Show(this, param, success!=null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewKhh_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TYT_KHH pData = (TYT_KHH)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex]; if (pData != null && pData.ID > 0)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewKhh_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "DELETE")
                    {
                        TYT_KHH data = (TYT_KHH)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null && data.CREATOR == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_D;
                        }
                    }
                    else if (e.Column.FieldName == "ACTIVE")
                    {
                        TYT_KHH data = (TYT_KHH)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data.IS_ACTIVE == 1)
                        {
                            if (data != null && data.CREATOR == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButton__Active;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton__Active_D;
                            }
                        }
                        else
                        {
                            if (data != null && data.CREATOR == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButtonInActive;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonInActive_D;
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!btnEdit.Enabled)
                return;
            btnSave_Click(null, null);
        }

        private void barButtonItem__Add_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!btnSave.Enabled)
                return;
            btnSave_Click(null, null);
        }

        private void gridViewKhh_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (TYT_KHH)gridViewKhh.GetFocusedRow();
                if (data != null)
                {
                    this._TYT_KHH = data;
                    this.FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                validate(this.txtBienChung, 100);
                validate(this.txtGhiChu, 100);
               

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validate(BaseEdit control, int maxlength)
        {

            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;

                validRule.ErrorText = "Trường dữ liệu vượt quá kí tự cho phép";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
