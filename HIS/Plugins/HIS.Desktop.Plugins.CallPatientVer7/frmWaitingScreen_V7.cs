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
using HIS.Desktop.Plugins.CallPatientVer7;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using MOS.EFMODEL.DataModels;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using System.IO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using System.Threading;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.CallPatientVer7
{
    public partial class frmWaitingScreen_V7 : FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM hisVaccinationExam;
        List<MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM> datas = null;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        private int scrll { get; set; }
        List<int> newStatusForceColorCodes = new List<int>();
        List<VaccinationSttSDO> vaccinationStts;
        internal string[] FilePath;
        int index = 0;
        int rowCount = 0;
        int countTimer = 0;
        List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> tempListVaccinationExam;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        const string moduleLink = "HIS.Desktop.Plugins.CallPatientVer7";

        public frmWaitingScreen_V7(MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM HisVaccinationExam, List<HIS.Desktop.Plugins.CallPatientVer7.VaccinationSttSDO> vaccinationStts)
        {

            InitializeComponent();
            this.hisVaccinationExam = HisVaccinationExam;
            this.vaccinationStts = vaccinationStts;
        }

        private void frmWaitingScreen_V7_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                FillDataToDictionaryWaitingPatient(vaccinationStts);
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                GetFilePath();
                StartAllTimer();
                var emp = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0} {1}", emp != null ? (emp.TITLE !=null? emp.TITLE + ": ":"") : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaitingCls.RowCount - 1;
                SetFormFrontOfAll();
                timer1.Interval = 100;
                timer1.Enabled = true;
                timer1.Start();
                CallPatientDataWorker.DicDelegateCallingPatient[this.room.ID] = (DelegateSelectData)this.nhapNhay;
                SetIcon();
                datas = (List<MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM>)gridControlWaitingCls.DataSource;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFormFrontOfAll()
        {
            try
            {
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                this.TopMost = true;
                this.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDefaultListPatientSTT()
        {
            try
            {
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {
                    foreach (var item in CallPatientDataWorker.DicCallPatient[room.ID])
                    {
                        item.CallPatientSTT = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StartAllTimer()
        {
            try
            {
                timerForScrollListPatient.Interval = 2000;
                timerForScrollListPatient.Enabled = true;
                timerForScrollListPatient.Start();

                timerSetDataToGridControl.Interval = 10000;
                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Start();

                timerAutoLoadDataPatient.Interval = (500);
                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Start();

                timerForHightLightCallPatientLayout.Interval = (2000);
                timerForHightLightCallPatientLayout.Enabled = true;
                timerForHightLightCallPatientLayout.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToRoom(MOS.EFMODEL.DataModels.V_HIS_ROOM room)
        {
            try
            {
                if (room != null)
                {
                    lblRoomName.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblRoomName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void nhapNhay(object data)
        {
            try
            {
                ///tao thread xu ly nhap nhay
                ///
                this.countTimer = 8;
                this.hisVaccinationExam = new HIS_VACCINATION_EXAM();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_VACCINATION_EXAM>(this.hisVaccinationExam, data);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetFilePath()
        {
            try
            {
                if (!string.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO)))
                {
                    FilePath = Directory.GetFiles(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToGridControlWaitingCLSs()
        {
            try
            {
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {
                    int countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                    if (countPatient == 0)
                        countPatient = 10;
                    // danh sách chờ kết quả cận lâm sàng
                    var VaccinationExamFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].OrderBy(o => o.NUM_ORDER).ToList();
                    this.tempListVaccinationExam = new List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO>();
                    this.tempListVaccinationExam = VaccinationExamFilterSTTs;
                    //gridControlWaitingCls.DataSource = null;
                    gridControlWaitingCls.BeginUpdate();
                    gridControlWaitingCls.DataSource = VaccinationExamFilterSTTs;
                    gridControlWaitingCls.EndUpdate();
                }
                else
                {
                    gridControlWaitingCls.BeginUpdate();
                    gridControlWaitingCls.DataSource = null;
                    gridControlWaitingCls.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private List<LocalStorage.BackendData.ADO.ServiceReq1ADO> ConnvertVaccinationExam1ToADO(List<HIS_VACCINATION_EXAM> result)
        {
            List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> vaccinationExam1Ados = new List<LocalStorage.BackendData.ADO.ServiceReq1ADO>();
            try
            {
                foreach (var item in result)
                {
                    LocalStorage.BackendData.ADO.ServiceReq1ADO vaccinationExam1Ado = new LocalStorage.BackendData.ADO.ServiceReq1ADO();
                    //AutoMapper.Mapper.CreateMap<HIS_VACCINATION_EXAM, HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO>();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReq1ADO>(vaccinationExam1Ado, item);
                    vaccinationExam1Ado.VaccinationId = item.ID;
                    vaccinationExam1Ado.TDL_PATIENT_DOB = item.TDL_PATIENT_DOB;
                    vaccinationExam1Ado.TDL_PATIENT_NAME = item.TDL_PATIENT_NAME;
                    vaccinationExam1Ado.TDL_PATIENT_GENDER_NAME = item.TDL_PATIENT_GENDER_NAME;
                    vaccinationExam1Ado.NUM_ORDER = item.NUM_ORDER;
                    vaccinationExam1Ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = item.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                    if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                    {
                        var checkTreatment = CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.VaccinationId == item.ID && o.CallPatientSTT);
                        if (checkTreatment != null)
                        {
                            vaccinationExam1Ado.CallPatientSTT = true;
                        }
                        else
                        {
                            vaccinationExam1Ado.CallPatientSTT = false;
                        }
                    }
                    else
                    {
                        vaccinationExam1Ado.CallPatientSTT = false;
                    }

                    vaccinationExam1Ados.Add(vaccinationExam1Ado);
                }
                vaccinationExam1Ados = vaccinationExam1Ados.OrderByDescending(o => o.CallPatientSTT).ToList();
                //CallPatientDataUpdateDictionary.UpdateDictionaryPatient(room.ID, serviceReq1Ados);
                //CallPatientDataWorker.DicCallPatient[room.ID] = serviceReq1Ados;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return vaccinationExam1Ados;
        }

        private void timerForScrollListPatientProcess()
        {
            try
            {
                index += 1;
                gridViewWaitingCls.FocusedRowHandle = index;
                if (index == rowCount)
                {
                    index = 0;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                ScrollListPatientThread();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ScrollListPatientThread()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadScrollListPatient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteThreadScrollListPatient()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { timerForScrollListPatientProcess(); }));
                }
                else
                {
                    timerForScrollListPatientProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerAutoLoadDataPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadWaitingPatientForWaitingScreen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadWaitingPatientForWaitingScreen()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadWaitingPatientToCall);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteThreadWaitingPatientToCall()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { StartTheadWaitingPatientToCall(); }));
                }
                else
                {
                    StartTheadWaitingPatientToCall();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StartTheadWaitingPatientToCall()
        {
            FillDataToDictionaryWaitingPatient(vaccinationStts);
        }

        private void gridViewWaitingCls_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    //MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM data = (MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    LocalStorage.BackendData.ADO.ServiceReq1ADO data = (LocalStorage.BackendData.ADO.ServiceReq1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)data.TDL_PATIENT_DOB);
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

        private void gridViewWaitingCls_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var result = (ServiceReq1ADO)gridViewWaitingCls.GetRow(e.RowHandle);
                if (result != null && this.hisVaccinationExam != null)
                {
                    if (result.NUM_ORDER == this.hisVaccinationExam.NUM_ORDER)
                    {
                        if (countTimer > 0)
                        {
                            countTimer--;
                            if (countTimer == 7 || countTimer == 5 || countTimer == 3 || countTimer == 1)
                            {
                                e.HighPriority = true;
                                e.Appearance.ForeColor = System.Drawing.Color.Red;
                            }
                            if (countTimer == 6 || countTimer == 4 || countTimer == 2)
                            {
                                e.HighPriority = true;
                                e.Appearance.ForeColor = System.Drawing.Color.White;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        void FillDataToDictionaryWaitingPatient(List<VaccinationSttSDO> vaccinationStts)
        {
            try
            {

                CommonParam param = new CommonParam();
                MOS.Filter.HisVaccinationExamFilter hisVaccinationExamFilter = new HisVaccinationExamFilter();

                if (this.room != null)
                {
                    hisVaccinationExamFilter.EXECUTE_ROOM_ID = room.ID;
                }

                // hisVaccinationExamFilter.HAS_EXECUTE_TIME = null;
                hisVaccinationExamFilter.ORDER_FIELD = "NUM_ORDER";

                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                hisVaccinationExamFilter.ORDER_DIRECTION = "ASC";
                hisVaccinationExamFilter.REQUEST_DATE_TO = endDay;
                hisVaccinationExamFilter.REQUEST_DATE_FROM = startDay;

                if (vaccinationStts != null && vaccinationStts.Count > 0)
                {
                    var list = vaccinationStts.Select(o => o.NAME_STATUS).ToList();
                    if (list.Contains("Chưa khám") && !list.Contains("Đã khám"))
                    {
                        hisVaccinationExamFilter.HAS_EXECUTE_TIME = false;
                    }
                    else if (!list.Contains("Chưa khám") && list.Contains("Đã khám"))
                    {
                        hisVaccinationExamFilter.HAS_EXECUTE_TIME = true;
                    }
                    else
                    {
                        hisVaccinationExamFilter.HAS_EXECUTE_TIME = null;
                    }
                }

                var result = new BackendAdapter(param).Get<List<HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/Get", ApiConsumers.MosConsumer, hisVaccinationExamFilter, param);


                if (result != null && result.Count > 0)
                {
                    //CallPatientDataUpdateDictionary.UpdateDictionaryPatient(room.ID, ConnvertListServiceReq1ToADO(result));
                    CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertVaccinationExam1ToADO(result);
                }
                else
                {
                    CallPatientDataWorker.DicCallPatient[room.ID] = new List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO>();
                }
                lblPatientName.Text = "";
                lblSoThuTuBenhNhan.Text = "";

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        // gan du lieu vao gridcontrol
        private void timerSetDataToGridControl_Tick(object sender, EventArgs e)
        {
            try
            {
                SetDataToGridControlCLS();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDataToGridControlCLS()
        {
            try
            {
                Task.Factory.StartNew(executeThreadSetDataToGridControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void executeThreadSetDataToGridControl()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { StartTheadSetDataToGridControl(); }));
                }
                else
                {
                    StartTheadSetDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void StartTheadSetDataToGridControl()
        {
            SetDataToGridControlWaitingCLSs();
        }

        int countHightLight = 0;
        private void timerForHightLightCallPatientLayoutProcess()
        {
            try
            {
                countHightLight++;
                SetDataToCurentCallPatientUsingThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForHightLightCallPatientLayout_Tick(object sender, EventArgs e)
        {
            try
            {
                timerForHightLightCallPatientLayoutProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDataToCurentCallPatientUsingThread()
        {
            try
            {
                Task.Factory.StartNew(executeThreadSetDataToCurentCallPatient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void executeThreadSetDataToCurentCallPatient()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { SetDataToCurrentCallPatient(); }));
                }
                else
                {
                    SetDataToCurrentCallPatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToCurrentCallPatient()
        {
            try
            {
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {

                    HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO PatientIsCall = (this.vaccinationStts != null && this.vaccinationStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.CallPatientSTT) : null;

                    if (PatientIsCall != null && countHightLight < 20)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 1");
                        if (ServiceReq1ADOWorker.ServiceReq1ADO == null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 2");
                            ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                        }
                        else
                        {
                            if (PatientIsCall.TDL_PATIENT_NAME != ServiceReq1ADOWorker.ServiceReq1ADO.TDL_PATIENT_NAME || PatientIsCall.NUM_ORDER != ServiceReq1ADOWorker.ServiceReq1ADO.NUM_ORDER)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 3");
                                ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 4");
                            }
                        }
                    }
                    else
                    {
                        if (PatientIsCall != null)
                        {
                            PatientIsCall.CallPatientSTT = false;
                            PatientIsCall.IsCalling = true;
                        }
                        Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 5");
                        ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
                        //SetDataToCurrentPatientCall(currentServiceReq1ADO);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 6");
                    ServiceReq1ADOWorker.ServiceReq1ADO = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                SetDataToLabelMoiBenhNhan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDataToLabelMoiBenhNhan()
        {
            try
            {
                Task.Factory.StartNew(executeThreadSetDataToLabelMoiBenhNhan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void executeThreadSetDataToLabelMoiBenhNhan()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { SetDataToLabelMoiBenhNhanChild(); }));
                }
                else
                {
                    SetDataToLabelMoiBenhNhanChild();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToLabelMoiBenhNhanChild()
        {
            try
            {
                if (ServiceReq1ADOWorker.ServiceReq1ADO != null)
                {
                    SetDataToCurrentPatientCall(ServiceReq1ADOWorker.ServiceReq1ADO);
                }
                else
                {
                    SetDataToCurrentPatientCall(null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataToCurrentPatientCall(ServiceReq1ADO serviceReq1ADO)
        {
            try
            {
                if (serviceReq1ADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 7 " + serviceReq1ADO.TDL_PATIENT_NAME + "_____" + serviceReq1ADO.NUM_ORDER);
                    lblPatientName.Text = serviceReq1ADO.TDL_PATIENT_NAME;
                    lblSoThuTuBenhNhan.Text = serviceReq1ADO.NUM_ORDER + "";
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 8");
                    lblPatientName.Text = "";
                    lblSoThuTuBenhNhan.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timerChangeColorRow_Tick(object sender, EventArgs e)
        {
            try
            {
                SetDataForChangeColor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataForChangeColor()
        {
            try
            {
                Task.Factory.StartNew(executeThreadSetDataForChangeColor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void executeThreadSetDataForChangeColor()
        {
            try
            {
                StartTheadSetDataForChangeColor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void StartTheadSetDataForChangeColor()
        {
            SetDataForChangeColorWaitingCLSs();
        }

        private void SetDataForChangeColorWaitingCLSs()
        {
            try
            {
                timerChangeColorRow.Interval = 1000;
                gridControlWaitingCls.BeginUpdate();
                gridControlWaitingCls.DataSource = this.tempListVaccinationExam;
                gridControlWaitingCls.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmWaitingScreen_V7_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerAutoLoadDataPatient.Enabled = false;
                timerForHightLightCallPatientLayout.Enabled = false;
                timerForScrollListPatient.Enabled = false;
                timerSetDataToGridControl.Enabled = false;
                timerChangeColorRow.Enabled = false;
                timer1.Enabled = false;

                timerAutoLoadDataPatient.Stop();
                timerForHightLightCallPatientLayout.Stop();
                timerForScrollListPatient.Stop();
                timerSetDataToGridControl.Stop();
                timerChangeColorRow.Stop();
                timer1.Stop();

                timerAutoLoadDataPatient.Dispose();
                timerForHightLightCallPatientLayout.Dispose();
                timerForScrollListPatient.Dispose();
                timerSetDataToGridControl.Dispose();
                timerChangeColorRow.Dispose();
                timer1.Dispose();

                ServiceReq1ADOWorker.ServiceReq1ADO = new HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
