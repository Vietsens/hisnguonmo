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
using DevExpress.XtraEditors.ViewInfo;
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

namespace TYT.Desktop.Plugins.TYTFetusExam
{
    public partial class frm : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT _Patient { get; set; }

        TYT_FETUS_EXAM _FETUS_EXAM { get; set; }

        string _patientCode = "";

        int action = 0;

        private int positionHandleControl = -1;

        public frm()
        {
            InitializeComponent();
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule, TYT.EFMODEL.DataModels.TYT_FETUS_EXAM __FETUS_EXAM)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this._FETUS_EXAM = __FETUS_EXAM;
            this._patientCode = __FETUS_EXAM.PATIENT_CODE;
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
            this._patientCode = _patient.PATIENT_CODE;
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

                ValidControl();

                FillDataToControl();

                LoadListFetusExam();

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
                dtExamTime.EditValue = null;
                dtNgayKinhCuoiCung.EditValue = null;
                spinTuanThai.EditValue = null;
                dtDuKienNgaySinh.EditValue = null;
                spinLanCoThai.EditValue = null;
                spinTrongLuong.EditValue = null;
                spinChieuCao.EditValue = null;
                spinHuyetApMin.EditValue = null;
                spinHuyetApMax.EditValue = null;
                spinChieuCaoTuCung.EditValue = null;
                spinVongBung.EditValue = null;
                spinKhungChau.EditValue = null;
                chkThieuMau.Checked = false;
                chkProtein.Checked = false;
                chkXetNghienHIV.Checked = false;
                txtXetNghiemKhac.Text = "";
                txtTienLuongDe.Text = "";
                spinSoMuiUV.EditValue = null;
                chkUongVienSat.Checked = false;
                spinTimThai.EditValue = null;
                txtNgoiThai.Text = "";
                txtGhiChu.Text = "";
                txtTienSu.Text = "";
                dxValidationProvider1.RemoveControlError(txtXetNghiemKhac);
                dxValidationProvider1.RemoveControlError(txtTienSu);
                dxValidationProvider1.RemoveControlError(txtGhiChu);
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
                if (this._FETUS_EXAM != null)
                {
                    this.action = GlobalVariables.ActionEdit;
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    if (this._FETUS_EXAM.EXAM_TIME > 0)
                    {
                        dtExamTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._FETUS_EXAM.EXAM_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (this._FETUS_EXAM.LAST_MENSES_TIME > 0)
                    {
                        dtNgayKinhCuoiCung.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._FETUS_EXAM.LAST_MENSES_TIME ?? 0) ?? DateTime.Now;
                    }

                    spinTuanThai.EditValue = this._FETUS_EXAM.WEEK;
                    if (this._FETUS_EXAM.BORN_TIME > 0)
                    {
                        dtDuKienNgaySinh.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._FETUS_EXAM.BORN_TIME ?? 0) ?? DateTime.Now;
                    }
                    spinLanCoThai.EditValue = this._FETUS_EXAM.FETUS_COUNT;
                    spinTrongLuong.EditValue = this._FETUS_EXAM.WEIGHT;
                    spinChieuCao.EditValue = this._FETUS_EXAM.HEIGHT;
                    spinHuyetApMin.EditValue = this._FETUS_EXAM.BLOOD_PRESSURE_MIN;
                    spinHuyetApMax.EditValue = this._FETUS_EXAM.BLOOD_PRESSURE_MAX;
                    spinChieuCaoTuCung.EditValue = this._FETUS_EXAM.UTERUS;
                    spinVongBung.EditValue = this._FETUS_EXAM.BELLY;
                    spinKhungChau.EditValue = this._FETUS_EXAM.PELVIC;
                    if (this._FETUS_EXAM.BLOOD.HasValue && this._FETUS_EXAM.BLOOD == 1)
                    {
                        chkThieuMau.Checked = true;
                    }
                    else
                        chkThieuMau.Checked = false;
                    if (this._FETUS_EXAM.PROTEIN.HasValue && this._FETUS_EXAM.PROTEIN == 1)
                    {
                        chkProtein.Checked = true;
                    }
                    else
                        chkProtein.Checked = false;

                    if (this._FETUS_EXAM.IS_HIV_TEST.HasValue && this._FETUS_EXAM.IS_HIV_TEST == 1)
                    {
                        chkXetNghienHIV.Checked = true;
                    }
                    else
                        chkXetNghienHIV.Checked = false;


                    txtXetNghiemKhac.Text = this._FETUS_EXAM.OTHER_TEST;
                    txtTienLuongDe.Text = this._FETUS_EXAM.BORN_GUESS;
                    spinSoMuiUV.EditValue = this._FETUS_EXAM.UV_COUNT;
                    if (this._FETUS_EXAM.IS_FOLIC.HasValue && this._FETUS_EXAM.IS_FOLIC == 1)
                    {
                        chkUongVienSat.Checked = true;
                    }
                    else
                        chkUongVienSat.Checked = false;

                    spinTimThai.EditValue = this._FETUS_EXAM.FETUS_HEART;
                    txtNgoiThai.Text = this._FETUS_EXAM.FETUS_POSITION;
                    txtTienSu.Text = this._FETUS_EXAM.HEALTH_HISTORY;
                    txtGhiChu.Text = this._FETUS_EXAM.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListFetusExam()
        {
            try
            {
                gridControlFetusExam.DataSource = null;
                TYT.Filter.TytFetusExamFilter filter = new Filter.TytFetusExamFilter();
                if (!string.IsNullOrEmpty(this._patientCode))
                {
                    filter.PATIENT_CODE__EXACT = this._patientCode;
                }
                else
                {
                    return;
                }
                filter.ORDER_FIELD = "EXAM_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var datas = new BackendAdapter(new CommonParam()).Get<List<TYT.EFMODEL.DataModels.TYT_FETUS_EXAM>>("api/TytFetusExam/Get", ApiConsumers.TytConsumer, filter, null);
                if (datas != null && datas.Count > 0)
                {
                    string branchCode = BranchDataWorker.Branch.BRANCH_CODE;
                    if (!string.IsNullOrEmpty(branchCode))
                    {
                        datas = datas.Where(p => p.BRANCH_CODE == branchCode).ToList();
                        gridControlFetusExam.DataSource = datas;
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
                btnSave.Focus();
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                if (this._FETUS_EXAM != null && this._FETUS_EXAM.ID > 0)
                {
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                }
                else
                {
                    if (this._Patient == null || this._Patient.ID <= 0)
                    {
                        MOS.Filter.HisPatientViewFilter _patientFilter = new MOS.Filter.HisPatientViewFilter();
                        _patientFilter.PATIENT_CODE__EXACT = this._patientCode;
                        this._Patient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, _patientFilter, null).FirstOrDefault();
                    }
                    this._FETUS_EXAM = new TYT_FETUS_EXAM();
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_FETUS_EXAM>(this._FETUS_EXAM, this._Patient);
                    this._FETUS_EXAM.PERSON_ADDRESS = this._Patient.VIR_ADDRESS;
                    this._FETUS_EXAM.ID = 0;
                    this._FETUS_EXAM.BHYT_NUMBER = this._Patient.TDL_HEIN_CARD_NUMBER;
                    this._FETUS_EXAM.VIR_PERSON_NAME = this._Patient.VIR_PATIENT_NAME;
                }
                if (dtExamTime.EditValue != null)
                {
                    this._FETUS_EXAM.EXAM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtExamTime.DateTime);
                }
                else
                {
                    this._FETUS_EXAM.LAST_MENSES_TIME = null;
                }
                if (dtNgayKinhCuoiCung.EditValue != null)
                {
                    this._FETUS_EXAM.LAST_MENSES_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtNgayKinhCuoiCung.DateTime);
                }
                else
                {
                    this._FETUS_EXAM.LAST_MENSES_TIME = null;
                }
                if (spinTuanThai.EditValue != null)
                {
                    this._FETUS_EXAM.WEEK = (long)spinTuanThai.Value;
                }
                else
                {
                    this._FETUS_EXAM.WEEK = null;
                }
                if (dtDuKienNgaySinh.EditValue != null)
                {
                    this._FETUS_EXAM.BORN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtDuKienNgaySinh.DateTime);
                }
                else
                {
                    this._FETUS_EXAM.BORN_TIME = null;
                }
                if (spinLanCoThai.EditValue != null)
                {
                    this._FETUS_EXAM.FETUS_COUNT = (long)spinLanCoThai.Value;
                }
                else
                {
                    this._FETUS_EXAM.FETUS_COUNT = null;
                }
                if (spinTrongLuong.EditValue != null)
                {
                    this._FETUS_EXAM.WEIGHT = (decimal)spinTrongLuong.Value;
                }
                else
                {
                    this._FETUS_EXAM.WEIGHT = null;
                }
                if (spinChieuCao.EditValue != null)
                {
                    this._FETUS_EXAM.HEIGHT = (decimal)spinChieuCao.Value;
                }
                else
                {
                    this._FETUS_EXAM.HEIGHT = null;
                }
                if (spinHuyetApMin.EditValue != null)
                {
                    this._FETUS_EXAM.BLOOD_PRESSURE_MIN = (decimal)spinHuyetApMin.Value;
                }
                else
                {
                    this._FETUS_EXAM.BLOOD_PRESSURE_MIN = null;
                }
                if (spinHuyetApMax.EditValue != null)
                {
                    this._FETUS_EXAM.BLOOD_PRESSURE_MAX = (decimal)spinHuyetApMax.Value;
                }
                else
                {
                    this._FETUS_EXAM.BLOOD_PRESSURE_MAX = null;
                }
                if (spinChieuCaoTuCung.EditValue != null)
                {
                    this._FETUS_EXAM.UTERUS = (decimal)spinChieuCaoTuCung.Value;
                }
                else
                {
                    this._FETUS_EXAM.UTERUS = null;
                }
                if (spinVongBung.EditValue != null)
                {
                    this._FETUS_EXAM.BELLY = (decimal)spinVongBung.Value;
                }
                else
                {
                    this._FETUS_EXAM.BELLY = null;
                }
                if (spinKhungChau.EditValue != null)
                {
                    this._FETUS_EXAM.PELVIC = (decimal)spinKhungChau.Value;
                }
                else
                {
                    this._FETUS_EXAM.PELVIC = null;
                }
                if (chkThieuMau.Checked)
                {
                    this._FETUS_EXAM.BLOOD = 1;
                }
                else
                {
                    this._FETUS_EXAM.BLOOD = 2;
                }
                if (chkProtein.Checked)
                {
                    this._FETUS_EXAM.PROTEIN = 1;
                }
                else
                {
                    this._FETUS_EXAM.PROTEIN = 2;
                }
                if (chkXetNghienHIV.Checked)
                {
                    this._FETUS_EXAM.IS_HIV_TEST = 1;
                }
                else
                {
                    this._FETUS_EXAM.IS_HIV_TEST = null;
                }
                this._FETUS_EXAM.OTHER_TEST = txtXetNghiemKhac.Text;
                this._FETUS_EXAM.BORN_GUESS = txtTienLuongDe.Text;
                if (spinSoMuiUV.EditValue != null)
                {
                    this._FETUS_EXAM.UV_COUNT = (long)spinSoMuiUV.Value;
                }
                else
                {
                    this._FETUS_EXAM.UV_COUNT = null;
                }
                if (chkUongVienSat.Checked)
                {
                    this._FETUS_EXAM.IS_FOLIC = 1;
                }
                else
                {
                    this._FETUS_EXAM.IS_FOLIC = null;
                }
                if (spinTimThai.EditValue != null)
                {
                    this._FETUS_EXAM.FETUS_HEART = (long)spinTimThai.Value;
                }
                else
                {
                    this._FETUS_EXAM.FETUS_HEART = null;
                }
                this._FETUS_EXAM.FETUS_POSITION = txtNgoiThai.Text;
                this._FETUS_EXAM.HEALTH_HISTORY = txtTienSu.Text;
                this._FETUS_EXAM.NOTE = txtGhiChu.Text;

                this._FETUS_EXAM.DOCTOR_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this._FETUS_EXAM.DOCTOR_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                this._FETUS_EXAM.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_FETUS_EXAM _result = new TYT_FETUS_EXAM();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_EXAM>("api/TytFetusExam/Create", ApiConsumers.TytConsumer, this._FETUS_EXAM, param);
                }
                else
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_EXAM>("api/TytFetusExam/Update", ApiConsumers.TytConsumer, this._FETUS_EXAM, param);
                }

                if (_result != null && _result.ID > 0)
                {
                    success = true;
                    this._FETUS_EXAM = _result;
                    this.LoadListFetusExam();
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
                this._FETUS_EXAM = new TYT_FETUS_EXAM();
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

        private void dtNgayKinhCuoiCung_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtNgayKinhCuoiCung.EditValue != null)
                    {
                        spinTuanThai.Focus();
                        spinTuanThai.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayKinhCuoiCung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTuanThai.Focus();
                    spinTuanThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTuanThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDuKienNgaySinh.Focus();
                    dtDuKienNgaySinh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDuKienNgaySinh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtDuKienNgaySinh.EditValue != null)
                    {
                        spinLanCoThai.Focus();
                        spinLanCoThai.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDuKienNgaySinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLanCoThai.Focus();
                    spinLanCoThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinLanCoThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTrongLuong.Focus();
                    spinTrongLuong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTrongLuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChieuCao.Focus();
                    spinChieuCao.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinChieuCao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHuyetApMin.Focus();
                    spinHuyetApMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHuyetApMin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHuyetApMax.Focus();
                    spinHuyetApMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHuyetApMax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChieuCaoTuCung.Focus();
                    spinChieuCaoTuCung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinChieuCaoTuCung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinVongBung.Focus();
                    spinVongBung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinVongBung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinKhungChau.Focus();
                    spinKhungChau.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinKhungChau_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThieuMau.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThieuMau_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkProtein.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkProtein_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkXetNghienHIV.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkXetNghienHIV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtXetNghiemKhac.Focus();
                    txtXetNghiemKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtXetNghiemKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTienLuongDe.Focus();
                    txtTienLuongDe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTienLuongDe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoMuiUV.Focus();
                    spinSoMuiUV.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoMuiUV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkUongVienSat.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUongVienSat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTimThai.Focus();
                    spinTimThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTimThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNgoiThai.Focus();
                    txtNgoiThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgoiThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTienSu.Focus();
                    txtTienSu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExamTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtExamTime.EditValue != null)
                    {
                        dtNgayKinhCuoiCung.Focus();
                        dtNgayKinhCuoiCung.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExamTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dtExamTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtExamTime.EditValue == null)
                    {
                        dtExamTime.Focus();
                        dtExamTime.ShowPopup();
                    }
                    else
                    {
                        dtNgayKinhCuoiCung.Focus();
                        dtNgayKinhCuoiCung.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTienSu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void dtDuKienNgaySinh_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void spinTrongLuong_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void spinKhungChau_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gridViewFetusExam_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TYT_FETUS_EXAM pData = (TYT_FETUS_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex]; if (pData != null && pData.ID > 0)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXAM_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.EXAM_TIME ?? 0);
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

        private void gridViewFetusExam_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (TYT_FETUS_EXAM)gridViewFetusExam.GetFocusedRow();
                if (data != null)
                {
                    this._FETUS_EXAM = data;
                    this.FillDataToControl();
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
                var data = (TYT_FETUS_EXAM)gridViewFetusExam.GetFocusedRow();
                if (data != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/TytFetusExam/Delete", ApiConsumers.TytConsumer, data.ID, param);
                    if (success)
                    {
                        btnRefesh_Click(null, null);
                        this._FETUS_EXAM = new TYT_FETUS_EXAM();
                        LoadListFetusExam();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled)
                    return;
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Edit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!btnSave.Enabled)
                return;
            btnSave_Click(null, null);
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

        private void ValidControl()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule rule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                rule.maxLength = 100;
                rule.editor = txtXetNghiemKhac;
                dxValidationProvider1.SetValidationRule(txtXetNghiemKhac, rule);

                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule rule1 = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                rule1.maxLength = 100;
                rule1.editor = txtTienSu;
                dxValidationProvider1.SetValidationRule(txtTienSu, rule1);

                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule rule2 = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                rule2.maxLength = 100;
                rule2.editor = txtGhiChu;
                dxValidationProvider1.SetValidationRule(txtGhiChu, rule2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFetusExam_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "DELETE")
                    {
                        TYT_FETUS_EXAM data = (TYT_FETUS_EXAM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null && data.CREATOR == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_D;
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
