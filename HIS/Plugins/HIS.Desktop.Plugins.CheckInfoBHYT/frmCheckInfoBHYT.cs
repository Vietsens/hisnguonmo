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
using DevExpress.XtraGrid.Views.Base;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CheckInfoBHYT.ADO;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using HIS.Desktop.ADO;
using System.Text;

namespace HIS.Desktop.Plugins.CheckInfoBHYT
{
    public partial class frmCheckInfoBHYT : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        HIS.Desktop.Common.DelegateRefreshData _dlg = null;
        CheckInfoBhytADO checkInfoBhytADO = null;
        long _TreatmentId;
        HIS_PATIENT _HisPatient { get; set; }
        HIS_TREATMENT _HisTreatment { get; set; }
        ResultDataADO rsDataBHYT { get; set; }
        ResultHistoryLDO _ResultHistoryLDO { get; set; }
        HIS_EMPLOYEE currentEmployee = null;
        HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }

        Dictionary<string, HIS_MEDI_ORG> dicMediOrg;

        string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";

        public frmCheckInfoBHYT(Inventec.Desktop.Common.Modules.Module module)
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

        public frmCheckInfoBHYT(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                this.currentModule = module;
                this._TreatmentId = _treatmentId;
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

        public frmCheckInfoBHYT(Inventec.Desktop.Common.Modules.Module module, CheckInfoBhytADO _ado)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.currentModule = module;
                this.checkInfoBhytADO = _ado;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCheckInfoBHYT_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                BHXHLoginCFG.LoadConfig();
                checkConfig();
                CheckEmploy();
                this.dicMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().ToDictionary(o => o.MEDI_ORG_CODE, o => o);
                LoadHisTreatment();
                LoadHisPatient();
                LoadPatientTypeAlter();
                SetEnableControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControl()
        {
            try
            {
                if (this._TreatmentId == null || this._TreatmentId == 0)
                {
                    btnUpdatePatient.Enabled = false;
                    btnUpdateBHYT.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHisPatient()
        {
            try
            {
                if (this._HisTreatment != null)
                {
                    _HisPatient = new HIS_PATIENT();
                    MOS.Filter.HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this._HisTreatment.PATIENT_ID;
                    var datas = new BackendAdapter(null).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                    if (datas != null && datas.Count > 0)
                    {
                        this._HisPatient = datas.FirstOrDefault();
                    }
                }
                else if (this.checkInfoBhytADO != null)
                {
                    this._HisPatient = new HIS_PATIENT();
                    _HisPatient.DOB = checkInfoBhytADO.TDL_DOB;
                    _HisPatient.VIR_PATIENT_NAME = checkInfoBhytADO.TDL_PATIENT_NAME;
                    _HisPatient.GENDER_ID = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.GENDER_NAME.Equals(checkInfoBhytADO.TDL_GENDER_NAME)).ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHisTreatment()
        {
            try
            {
                if (this._TreatmentId != null && this._TreatmentId > 0)
                {
                    _HisTreatment = new HIS_TREATMENT();
                    MOS.Filter.HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = this._TreatmentId;
                    var datas = new BackendAdapter(null).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    if (datas != null && datas.Count > 0)
                    {
                        this._HisTreatment = datas.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<string> connectInfors = new List<string>();
        string api = "";
        string nameCb = "";
        string cccdCb = "";
        private void checkConfig()
        {
            try
            {
                HisConfigCHECKHEINCARD.LoadConfig();
                string connect_infor = HisConfigCHECKHEINCARD.CHECK_HEIN_CARD_BHXH__API;
                if (!string.IsNullOrEmpty(connect_infor))
                {
                    connectInfors = connect_infor.Split('|').ToList();
                    api = connectInfors[0];
                    nameCb = connectInfors[1];
                    cccdCb = connectInfors[2];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void CheckEmploy()
        {
            try
            {
                if (currentEmployee == null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEmployeeFilter hisEmployeeFilter = new HisEmployeeFilter();
                    hisEmployeeFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    currentEmployee = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisEmployeeFilter, param).FirstOrDefault();
                }
            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            
        }
        private async void LoadPatientTypeAlter()
        {
            try
            {
                string name = "";
                string cccd = "";
                if (string.IsNullOrEmpty(nameCb))
                {
                    name = currentEmployee.TDL_USERNAME;
                }
                else
                {
                    name = nameCb;
                }
                if (string.IsNullOrEmpty(cccdCb))
                {
                    cccd = currentEmployee.IDENTIFICATION_NUMBER;
                }
                else
                {
                    cccd = cccdCb;
                }

                if (this._TreatmentId != null && this._TreatmentId > 0)
                {
                    gridControlBHYT.DataSource = null;
                    List<PatientTypeAlterADO> _PatientTypeAlterADOs = new List<PatientTypeAlterADO>();
                    MOS.Filter.HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_ID = this._TreatmentId;
                    string key = HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                    var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == key.Trim());
                    if (patientType != null)
                    {
                        filter.PATIENT_TYPE_ID = patientType.ID;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT :   null");
                        return;
                    }
                    filter.ORDER_DIRECTION = "ASC";
                    filter.ORDER_FIELD = "LOG_TIME";
                    var datas = new BackendAdapter(null).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, null);

                    if (datas != null && datas.Count > 0)
                    {
                        foreach (var item in datas)
                        {
                            PatientTypeAlterADO ado = new PatientTypeAlterADO(item);
                            await CheckTTFull(item, name, cccd,api);

                            if (rsDataBHYT != null)
                            {
                                ado.ResultDataADO = rsDataBHYT;
                            }
                            _PatientTypeAlterADOs.Add(ado);
                        }
                    }
                    gridControlBHYT.BeginUpdate();
                    gridControlBHYT.DataSource = _PatientTypeAlterADOs;
                    gridControlBHYT.EndUpdate();

                    Process(_PatientTypeAlterADOs.FirstOrDefault());
                }
                else if (this.checkInfoBhytADO != null)
                {
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlert = new V_HIS_PATIENT_TYPE_ALTER();
                    patientTypeAlert.HEIN_CARD_NUMBER = this.checkInfoBhytADO.TDL_HEIN_CARD_NUMBER;
                    PatientTypeAlterADO ado = new PatientTypeAlterADO(patientTypeAlert);
                    await CheckTTFull(patientTypeAlert, name, cccd,api);
                    if (rsDataBHYT != null)
                    {
                        ado.ResultDataADO = rsDataBHYT;
                    }
                    gridControlBHYT.BeginUpdate();
                    //gridControlBHYT.DataSource = _PatientTypeAlterADOs;
                    gridControlBHYT.EndUpdate();
                    Process(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBHYT_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (PatientTypeAlterADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_FROM_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_TO_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_TO_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "JOIN_5_YEAR_STR")
                        {
                            if (!string.IsNullOrEmpty(data.JOIN_5_YEAR) && data.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE)
                                e.Value = "X";
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == "PAID_6_MONTH_STR")
                        {
                            if (!string.IsNullOrEmpty(data.PAID_6_MONTH) && data.PAID_6_MONTH == MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE)
                                e.Value = "X";
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == "JOIN_5_YEAR_TIME_STR")
                        {
                            if (data.JOIN_5_YEAR_TIME != null)
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.JOIN_5_YEAR_TIME ?? 0);
                            else
                                e.Value = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBHYT_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var dataRow = (PatientTypeAlterADO)gridViewBHYT.GetFocusedRow();
                if (dataRow != null)
                {
                    Process(dataRow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Process(PatientTypeAlterADO data)
        {
            try
            {
                this._ResultHistoryLDO = null;
                this._PatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (data.ResultDataADO != null && data.ResultDataADO.ResultHistoryLDO != null)
                {
                    if (data.ResultDataADO.ResultHistoryLDO.success)
                    {
                        bool enablePatient = true;
                        bool enableBhyt = true;
                        //Đưa về cùng 1 định dạng tên để so sánh do hệ thống để tên viết hoa còn bhyt gửi về có viết thường
                        enablePatient = enablePatient && (this._HisPatient.VIR_PATIENT_NAME.ToLower().Trim() == data.ResultDataADO.ResultHistoryLDO.hoTen.ToLower().Trim());

                        if (data.ResultDataADO.ResultHistoryLDO.ngaySinh.Length == 4)
                        {
                            enablePatient = enablePatient && (this._HisPatient.DOB.ToString().Substring(0, 4) == data.ResultDataADO.ResultHistoryLDO.ngaySinh);
                        }
                        else
                        {
                            enablePatient = enablePatient && (Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._HisPatient.DOB) == data.ResultDataADO.ResultHistoryLDO.ngaySinh);
                        }

                        var genDer = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == this._HisPatient.GENDER_ID);
                        enablePatient = enablePatient && genDer != null && (genDer.GENDER_NAME == data.ResultDataADO.ResultHistoryLDO.gioiTinh);

                        enableBhyt = enableBhyt && (Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_FROM_TIME ?? 0) == data.ResultDataADO.ResultHistoryLDO.gtTheTu);
                        enableBhyt = enableBhyt && (Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_TO_TIME ?? 0) == data.ResultDataADO.ResultHistoryLDO.gtTheDen);
                        enableBhyt = enableBhyt && (data.HEIN_MEDI_ORG_CODE == data.ResultDataADO.ResultHistoryLDO.maDKBD);
                        enableBhyt = enableBhyt && (data.ADDRESS == data.ResultDataADO.ResultHistoryLDO.diaChi);
                        enableBhyt = enableBhyt && data.HEIN_CARD_NUMBER == data.ResultDataADO.ResultHistoryLDO.maThe;
                        enableBhyt = enableBhyt && (string.IsNullOrEmpty(data.ResultDataADO.ResultHistoryLDO.ngayDu5Nam) || Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.JOIN_5_YEAR_TIME ?? 0) == data.ResultDataADO.ResultHistoryLDO.ngayDu5Nam);

                        btnUpdateBHYT.Enabled = !enableBhyt;
                        btnUpdatePatient.Enabled = !enablePatient;

                        //btnPrintScreen.Enabled = !enableBhyt || !enablePatient;

                        if (!enableBhyt || !enablePatient)
                            lblMessenger.Text = "Có sự sai khác thông tin thẻ và thông tin bệnh nhân trên cổng Bảo hiểm y tế. Nhấn nút bên cạnh để cập nhập lại thông tin.";
                        else
                            lblMessenger.Text = "";

                        if (this._TreatmentId == null || this._TreatmentId == 0)
                        {
                            btnUpdateBHYT.Enabled = false;
                            btnUpdatePatient.Enabled = false;
                            lblMessenger.Text = "";
                        }

                        this._ResultHistoryLDO = data.ResultDataADO.ResultHistoryLDO;

                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(this._PatientTypeAlter, data);
                    }

                    //Tách chuỗi trả về thành các dòng thông tin 
                    string ghichu = data.ResultDataADO.ResultHistoryLDO.ghiChu;

                    string text = "";
                    string[] listNote = ghichu.Split(new char[] { ')', '(', '.', '!' }, StringSplitOptions.RemoveEmptyEntries);
                    List<ExamHistoryLDO> glstExamHistory = data.ResultDataADO.ResultHistoryLDO.dsLichSuKCB2018;
                    foreach (string note in listNote)
                    {
                        if (!note.Equals(" "))
                            text = text + note + " !\r\n";
                    }

                    if (data.ResultDataADO.ResultHistoryLDO.maKetQua == "101")
                        text = String.Format("{0} - {1}", data.ResultDataADO.ResultHistoryLDO.message, text);
                    txtViewInfoCheck.Text = text;

                    LoadDataGridControl(data.ResultDataADO.ResultHistoryLDO);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.ResultDataADO.ResultHistoryLDO), data.ResultDataADO.ResultHistoryLDO));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridControl(ResultHistoryLDO _resultHistoryLDO)
        {
            try
            {
                gridControlHistoryExam.DataSource = null;
                if (_resultHistoryLDO.dsLichSuKCB2018 != null)
                {
                    var query = _resultHistoryLDO.dsLichSuKCB2018.OrderByDescending(o => o.ngayRa).AsQueryable();
                    //if (_resultHistoryLDO.dsLichSuKCB2018.Count > 5)
                    //{
                    //    query = query.Skip(0).Take(5);
                    //}

                    gridControlHistoryExam.DataSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHistoryExam_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExamHistoryLDO data = (ExamHistoryLDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = 1 + e.ListSourceRowIndex;
                        }
                        else if (e.Column.FieldName == "tinhTrang_str")
                        {
                            if (data.tinhTrang == "1")
                                e.Value = "Ra viện";
                            else if (data.tinhTrang == "2")
                                e.Value = "Chuyển viện";
                            else if (data.tinhTrang == "3")
                                e.Value = "Trốn viện";
                            else if (data.tinhTrang == "4")
                                e.Value = "Xin ra viện";
                        }
                        else if (e.Column.FieldName == "kqDieuTri_str")
                        {
                            if (data.kqDieuTri == "1")
                                e.Value = "Khỏi";
                            else if (data.kqDieuTri == "2")
                                e.Value = "Đỡ";
                            else if (data.kqDieuTri == "3")
                                e.Value = "Không thay đổi";
                            else if (data.kqDieuTri == "4")
                                e.Value = "Nặng hơn";
                            else if (data.kqDieuTri == "5")
                                e.Value = "Tử vong";
                        }
                        else if (e.Column.FieldName == "lyDoVV_str")
                        {
                            if (data.lyDoVV == "1")
                                e.Value = "Đúng tuyến";
                            else if (data.lyDoVV == "2")
                                e.Value = "Cấp cứu";
                            else if (data.lyDoVV == "3")
                                e.Value = "Trái tuyến";
                        }
                        else if (e.Column.FieldName == "ngayVao_str" && !String.IsNullOrEmpty(data.ngayVao))
                        {
                            e.Value = TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayVao));
                        }
                        else if (e.Column.FieldName == "ngayRa_str" && !String.IsNullOrEmpty(data.ngayRa))
                        {
                            e.Value = TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayRa));
                        }
                        else if (e.Column.FieldName == "cskcbbd_name")
                        {
                            if (this.dicMediOrg != null && this.dicMediOrg.ContainsKey(data.maCSKCB))
                            {
                                e.Value = this.dicMediOrg[data.maCSKCB].MEDI_ORG_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static string TimeNumberToTimeStringWithoutSecond(long time)
        {
            string result = null;
            try
            {
                string text = time.ToString();
                if (text != null)
                {
                    if (text.Length >= 12)
                    {
                        return new StringBuilder().Append(text.Substring(6, 2)).Append("/").Append(text.Substring(4, 2))
                            .Append("/")
                            .Append(text.Substring(0, 4))
                            .Append(" ")
                            .Append(text.Substring(8, 2))
                            .Append(":")
                            .Append(text.Substring(10, 2))
                            .ToString();
                    }

                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void btnUpdatePatient_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            // bool success = false;
            try
            {
                if (this._HisPatient != null && this._ResultHistoryLDO != null)
                {
                    HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                    MOS.EFMODEL.DataModels.HIS_PATIENT currentPatientDTO = _HisPatient;

                    try
                    {
                        int idx = this._ResultHistoryLDO.hoTen.Trim().LastIndexOf(" ");
                        if (idx > -1)
                        {
                            currentPatientDTO.FIRST_NAME = this._ResultHistoryLDO.hoTen.Trim().Substring(idx).Trim();
                            currentPatientDTO.LAST_NAME = this._ResultHistoryLDO.hoTen.Trim().Substring(0, idx).Trim();
                        }
                        else
                        {
                            currentPatientDTO.FIRST_NAME = this._ResultHistoryLDO.hoTen.Trim();
                            currentPatientDTO.LAST_NAME = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
                    }
                    var genDer = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.GENDER_NAME == this._ResultHistoryLDO.gioiTinh.Trim());
                    if (genDer != null)
                    {
                        currentPatientDTO.GENDER_ID = genDer.ID;
                    }
                    try
                    {
                        string[] str = this._ResultHistoryLDO.ngaySinh.Trim().Split('/');
                        currentPatientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(str[2] + str[1] + str[0] + "000000");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ngay sinh BN: ", ex);
                    }

                    List<object> listArgs = new List<object>();
                    V_HIS_PATIENT ado = new V_HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(ado, this._HisPatient);
                    listArgs.Add(ado);
                    listArgs.Add((RefeshReference)RefeshTreatment);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    //patientUpdateSdo.HisPatient = currentPatientDTO;
                    //// patientUpdateSdo.UpdateTreatment = true;

                    //var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, param);
                    //if (resultData != null)
                    //{
                    //    success = true;
                    //    btnUpdatePatient.Enabled = false;
                    //    WaitingManager.Hide();
                    //}
                }
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            // MessageManager.Show(this, param, success);
        }

        private void RefeshTreatment()
        {
            try
            {
                LoadHisPatient();
                btnUpdatePatient.Enabled = false;
                lblMessenger.Text = "Cập nhật thông tin bệnh nhân thành công";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUpdateBHYT_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            HisPatientTypeAlterAndTranPatiSDO SDO = new HisPatientTypeAlterAndTranPatiSDO();
            SDO.PatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
            try
            {
                if (this._PatientTypeAlter != null)
                {
                    HIS_PATIENT_TYPE_ALTER _alter = new HIS_PATIENT_TYPE_ALTER();
                    _alter = this._PatientTypeAlter;
                    try
                    {
                        string[] str = this._ResultHistoryLDO.gtTheTu.Trim().Split('/');
                        _alter.HEIN_CARD_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(str[2] + str[1] + str[0] + "000000");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi gtTheTuMoi: ", ex);
                    }
                    try
                    {
                        string[] str = this._ResultHistoryLDO.gtTheDen.Trim().Split('/');
                        _alter.HEIN_CARD_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(str[2] + str[1] + str[0] + "000000");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi gtTheDenMoi: ", ex);
                    }
                    _alter.HEIN_MEDI_ORG_CODE = _ResultHistoryLDO.maDKBD;
                    _alter.ADDRESS = _ResultHistoryLDO.diaChi;
                    _alter.HEIN_CARD_NUMBER = _ResultHistoryLDO.maThe;
                    if (!string.IsNullOrEmpty(this._ResultHistoryLDO.ngayDu5Nam))
                    {
                        try
                        {
                            string[] str = this._ResultHistoryLDO.ngayDu5Nam.Trim().Split('/');
                            _alter.JOIN_5_YEAR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(str[2] + str[1] + str[0] + "000000");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ngayDu5Nam: ", ex);
                        }
                    }

                    SDO.PatientTypeAlter = this._PatientTypeAlter;


                    var resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisPatientTypeAlterAndTranPatiSDO>("api/HisPatientTypeAlter/Update", ApiConsumers.MosConsumer, SDO, param);
                    if (resultPatientTypeAlter != null)
                    {
                        success = true;
                        btnUpdateBHYT.Enabled = false;
                        lblMessenger.Text = "Cập nhật thông tin thẻ thành công";
                        LoadPatientTypeAlter();
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
        }

        private void btnPrintScreen_Click(object sender, EventArgs e)
        {

            string maBHYT = _HisTreatment.TDL_HEIN_CARD_NUMBER;
            string treatmentCode = _HisTreatment.TREATMENT_CODE;
            string fileName = this.saveFileDialog1.FileName = treatmentCode + "_" + maBHYT;


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Thread.Sleep(500);
                //ScreenCapture sc = new ScreenCapture();
                //Image img = sc.CaptureScreen();
                var directoryPath = Path.GetDirectoryName(saveFileDialog1.FileName);
                FullScreenshot(saveFileDialog1.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Lưu ảnh thành công !" + directoryPath);
                this.Show();

            }
        }
        private void FullScreenshot(String filepath, ImageFormat format)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(filepath, format);
            }
        }

    }
}
