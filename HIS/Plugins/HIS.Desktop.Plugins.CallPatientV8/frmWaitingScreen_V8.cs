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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientV8.Class;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientV8
{
    public partial class frmWaitingScreen_V48 : FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        //internal MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<ServiceReqSttADO> serviceReqStts;
        string[] FilePath;
        List<int> gridpatientBodyForceColorCodes;
        int index = 0;
        int rowCount = 0;
        List<V_HIS_EXECUTE_ROOM> roomSelecteds;
        long planTimeFrom, planTimeTo;
        string content;
        const string moduleLink = "HIS.Desktop.Plugins.CallPatientV8";
        Inventec.Desktop.Common.Modules.Module module;
        public frmWaitingScreen_V48(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<ServiceReqSttADO> ServiceReqStts, List<V_HIS_EXECUTE_ROOM> roomSelecteds, long planTimeFrom, long planTimeTo, string content, Inventec.Desktop.Common.Modules.Module module) : base(module)
        {
            InitializeComponent();
            this.module = module;
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this.roomSelecteds = roomSelecteds;
            this.planTimeFrom = planTimeFrom;
            this.planTimeTo = planTimeTo;
            this.content = content;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToDictionaryWaitingPatient(serviceReqStts);
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                GetFilePath();
                StartAllTimer();
                SetFromConfigToControl();
                rowCount = gridViewWaitingCls.RowCount - 1;
                SetFormFrontOfAll();

                timer1.Interval = 2000;
                timer1.Enabled = true;
                timer1.Start();


                BestFitRow();

                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BestFitRow()
        {
            try
            {
                gridColumnAge.BestFit();
                gridColumnSTT.BestFit();
                if(string.IsNullOrEmpty(content))
                {
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    var dt = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lblTitUp.Text = dt[0];
                    if(dt.Count > 1)
                    {
                        lblTitDown.Text = dt[1];
                    }
                    else
                    {
                        lblTitUp.TextAlign = ContentAlignment.TopCenter;
                        layoutControlItem3.MinSize = new Size(layoutControlItem3.MinSize.Width, 50);
                        layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetFromConfigToControl()
        {

            try
            {
                List<int> userNameColorCodes = WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES;
                if (userNameColorCodes != null && userNameColorCodes.Count == 3)
                {
                    lblTime.ForeColor = System.Drawing.Color.FromArgb(userNameColorCodes[0], userNameColorCodes[1], userNameColorCodes[2]);
                }

                // cỡ chữ  tiêu đề danh sách bn
                gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnPhuongPhapPhauThuat.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGioMoXong.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);

                //cỡ chữ nội dung danh sách BN
                gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Regular);
                gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Regular);
                gridColumnAge.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Regular);
                gridColumnPhuongPhapPhauThuat.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Regular);
                gridColumnGioMoXong.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Regular);

                //cỡ chữ tên bệnh nhân đang được gọi
                lblTitUp.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);
                lblTitDown.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);

                // chiều cao dòng nội dung, tiêu đề ds bn
                gridViewWaitingCls.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWaitingCls.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                //if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                //{
                //    foreach (var item in CallPatientDataWorker.DicCallPatient[room.ID])
                //    {
                //        item.CallPatientSTT = false;
                //    }
                //}

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
                timerForScrollListPatient.Interval = (2000);
                timerForScrollListPatient.Enabled = true;
                timerForScrollListPatient.Start();

                timerSetDataToGridControl.Interval = (WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000);
                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Start();

                timerAutoLoadDataPatient.Interval = (WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000);
                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Start();


                timerTimeZone.Interval = 1000;
                timerTimeZone.Enabled = true;
                timerTimeZone.Start();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                ScrollListPatientThread();
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

        void StartTheadWaitingPatientToCall()
        {
            FillDataToDictionaryWaitingPatient(serviceReqStts);
        }

        private void gridViewWaitingCls_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_1 data = (V_HIS_SERE_SERV_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "AGE_DISPLAY")
                        {
                            e.Value = AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }else if(e.Column.FieldName == "DISPLAY_NAME_str")
                        {
                            e.Value = serviceReqStts.FirstOrDefault(o=>o.ID == data.SERVICE_REQ_STT_ID).DISPLAY_NAME;
                        }    
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetYearOld(long dob)
        {
            string yearDob = "";
            try
            {
                if (dob > 0)
                {
                    yearDob = dob.ToString().Substring(0, 4);
                }
            }
            catch (Exception ex)
            {
                yearDob = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return yearDob;
        }

        private void gridViewWaitingCls_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    Color serviceReqStt = serviceReqStts.FirstOrDefault(o => o.ID == Int64.Parse((View.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString())).BackColor;
                    e.Appearance.BackColor = serviceReqStt;
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
                    e.HighPriority = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataToDictionaryWaitingPatient(List<ServiceReqSttADO> serviceReqStts)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewWaitingCls);
                if (this.roomSelecteds == null || this.roomSelecteds.Count() == 0)
                    return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView1Filter hisServiceReqFilter = new HisSereServView1Filter();

                hisServiceReqFilter.SERVICE_TYPE_IDs = new List<long> { 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                };

                if (this.roomSelecteds != null && this.roomSelecteds.Count() > 0)
                {
                    hisServiceReqFilter.EXECUTE_ROOM_IDs = this.roomSelecteds.Select(o => o.ROOM_ID).ToList();
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MUST_BE_APPROVED_BEFORE_PROCESS_SURGERY") == "1")
                    hisServiceReqFilter.IS_NOT_SURGERY__OR__IS__APPROVED__OR__IS_EMERGENCY = true;
                List<long> lstServiceReqSTT = new List<long>();
                hisServiceReqFilter.HAS_EXECUTE = true;
                //hisServiceReqFilter.PLAN_TIME_FROM_FROM = this.planTimeFrom;
                //hisServiceReqFilter.PLAN_TIME_FROM_TO = this.planTimeTo;

                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());//20181212121527
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());

                hisServiceReqFilter.INTRUCTION_TIME_FROM = startDay;
                hisServiceReqFilter.INTRUCTION_TIME_TO = endDay;

                if (serviceReqStts != null && serviceReqStts.Count > 0)
                {
                    List<long> lstServiceReqSTTFilter = serviceReqStts.Select(o => o.ID).ToList();
                    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTTFilter;
                }

                var result = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                Inventec.Common.Logging.LogSystem.Info("Du lieu dau vao serviceCheckeds__Send:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqFilter), hisServiceReqFilter));
                result = result != null && result.Count() > 0 ? result.OrderByDescending(o => o.PLAN_TIME_FROM).ToList() : result;

                int countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                if (countPatient == 0)
                    countPatient = 10;

                // danh sách chờ kết quả cận lâm sàng

                gridControlWaitingCls.BeginUpdate();
                gridControlWaitingCls.DataSource = result.Take(countPatient);
                gridControlWaitingCls.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> ConnvertListServiceReq1ToADO(List<HIS_SERVICE_REQ> serviceReq1s)
        {
            List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> serviceReq1Ados = new List<LocalStorage.BackendData.ADO.ServiceReq1ADO>();
            try
            {
                foreach (var item in serviceReq1s)
                {
                    LocalStorage.BackendData.ADO.ServiceReq1ADO serviceReq1Ado = new LocalStorage.BackendData.ADO.ServiceReq1ADO();
                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_REQ, HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO>();
                    serviceReq1Ado = AutoMapper.Mapper.Map<LocalStorage.BackendData.ADO.ServiceReq1ADO>(item);
                    //if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                    //{
                    //    var checkTreatment = CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.ID == item.ID && o.CallPatientSTT);
                    //    if (checkTreatment != null)
                    //    {
                    //        serviceReq1Ado.CallPatientSTT = true;
                    //    }
                    //    else
                    //    {
                    //        serviceReq1Ado.CallPatientSTT = false;
                    //    }
                    //}
                    //else
                    //{
                    //    serviceReq1Ado.CallPatientSTT = false;
                    //}

                    serviceReq1Ados.Add(serviceReq1Ado);
                }
                serviceReq1Ados = serviceReq1Ados.OrderByDescending(o => o.CallPatientSTT).ToList();
                //CallPatientDataUpdateDictionary.UpdateDictionaryPatient(room.ID, serviceReq1Ados);
                //CallPatientDataWorker.DicCallPatient[room.ID] = serviceReq1Ados;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return serviceReq1Ados;
        }

        private void SetDataToCurrentPatientCall(ServiceReq1ADO serviceReq1ADO)
        {
            try
            {
                if (serviceReq1ADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 7");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 8");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void SetDataToCurrentCallPatient()
        {
            try
            {
                //if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                //{
                //    ServiceReq1ADO PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).FirstOrDefault(o => o.CallPatientSTT) : null;
                //    Inventec.Common.Logging.LogSystem.Info("SetDataToCurrentCallPatient() tDu lieu PatientIsCall:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientIsCall), PatientIsCall));

                //    if (PatientIsCall != null)
                //    {
                //        Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 1");
                //        if (ServiceReq1ADOWorker.ServiceReq1ADO == null)
                //        {
                //            Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 2");
                //            ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                //        }
                //        else
                //        {
                //            if (PatientIsCall.TDL_PATIENT_NAME != ServiceReq1ADOWorker.ServiceReq1ADO.TDL_PATIENT_NAME || PatientIsCall.NUM_ORDER != ServiceReq1ADOWorker.ServiceReq1ADO.NUM_ORDER)
                //            {
                //                Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 3");
                //                ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                //            }
                //            else
                //            {
                //                Inventec.Common.Logging.LogSystem.Debug("PatientIsCall step 4");
                //            }
                //        }

                //        //else if (currentServiceReq1ADO != null && (PatientIsCall.TDL_PATIENT_NAME != this.currentServiceReq1ADO.TDL_PATIENT_NAME || PatientIsCall.NUM_ORDER != this.currentServiceReq1ADO.NUM_ORDER))
                //        //{
                //        //    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 3");
                //        //    currentServiceReq1ADO = PatientIsCall;
                //        //    SetDataToCurrentPatientCall(currentServiceReq1ADO);
                //        //}
                //        //else
                //        //{
                //        //    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 4");
                //        //}
                //    }
                //    else
                //    {
                //        Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 5");
                //        ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
                //        //SetDataToCurrentPatientCall(currentServiceReq1ADO);
                //    }
                //}
                //else
                //{
                //    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 6");
                //    ServiceReq1ADOWorker.ServiceReq1ADO = null;
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataToGridControlWaitingCLSs()
        {
            try
            {
                //if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                //{
                //    int countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                //    if (countPatient == 0)
                //        countPatient = 10;

                //    // danh sách chờ kết quả cận lâm sàng
                //    var ServiceReqFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).ToList();
                //    gridControlWaitingCls.BeginUpdate();
                //    gridControlWaitingCls.DataSource = ServiceReqFilterSTTs;
                //    gridControlWaitingCls.EndUpdate();
                //}
                //else
                //{
                //    gridControlWaitingCls.BeginUpdate();
                //    gridControlWaitingCls.DataSource = null;
                //    gridControlWaitingCls.EndUpdate();
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void GetFilePath()
        {
            try
            {
                FilePath = Directory.GetFiles(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        void StartTheadSetDataToCurentCallPatient()
        {
            SetDataToCurentCallPatientUsingThread();
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

        private void timerForHightLightCallPatientLayout_Tick(object sender, EventArgs e)
        {
            try
            {
                SetDataToCurentCallPatientUsingThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmWaitingScreen_V4_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerAutoLoadDataPatient.Enabled = false;
                timerSetDataToGridControl.Enabled = false;
                timerForScrollListPatient.Enabled = false;
                timerTimeZone.Enabled = false;
                timer1.Enabled = false;

                timerAutoLoadDataPatient.Stop();
                timerSetDataToGridControl.Stop();
                timerForScrollListPatient.Stop();
                timerTimeZone.Stop();
                timer1.Stop();

                timerAutoLoadDataPatient.Dispose();
                timerSetDataToGridControl.Dispose();
                timerForScrollListPatient.Dispose();
                timerTimeZone.Dispose();
                timer1.Dispose();

                ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void gridViewWaitingCls_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                e.Appearance.DrawBackground(e.Cache, e.Bounds);
                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    if (!info.Visible) continue;
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter,
                        info.ElementInfo);
                }
                e.Painter.DrawCaption(e.Info, e.Info.Caption, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), e.Bounds, e.Appearance.GetStringFormat());
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerTimeZoneProcess()
        {
            try
            {
                lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void TimerTimeZoneThread()
        {
            try
            {
                Task.Factory.StartNew(executeThreadTimerTimeZone);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void executeThreadTimerTimeZone()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { timerTimeZoneProcess(); }));
                }
                else
                {
                    timerTimeZoneProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerTimeZone_Tick(object sender, EventArgs e)
        {
            try
            {
                TimerTimeZoneThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
