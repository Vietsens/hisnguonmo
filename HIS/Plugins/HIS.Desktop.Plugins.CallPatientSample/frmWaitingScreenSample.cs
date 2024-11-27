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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
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

namespace HIS.Desktop.Plugins.CallPatientSample
{
    public partial class frmWaitingScreenSample22 : FormBase
    {
        internal V_LIS_SAMPLE lisSample;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        internal V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<long> sampleSttIds;
        internal static string[] FilePath;
        List<int> gridpatientBodyForceColorCodes;
        int index = 0;
        int rowCount = 0;
        bool isSetNum = false;
        bool isTach = false;
        public frmWaitingScreenSample22(Inventec.Desktop.Common.Modules.Module module, V_LIS_SAMPLE sample, List<long> sttIds, V_HIS_ROOM r,bool isTach = false)
            : base(module)
        {
            InitializeComponent();
            this.lisSample = sample;
            this.sampleSttIds = sttIds;
            this.room = r;
            this.isTach = isTach;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                FillDataToDictionaryWaitingPatient();
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                GetFilePath();
                StartAllTimer();
                //SetFromConfigToControl();
                var emp = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0} {1}", emp != null ? (emp.TITLE != null ? emp.TITLE + ": " : "") : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaitingCls.RowCount +gridViewDaLM.RowCount - 1;
                SetFormFrontOfAll();
                SetEnableLayout();
                timer1.Interval = 2000;
                timer1.Enabled = true;
                timer1.Start();
                //RegisterTimer(ModuleLink, "timer1", 2000, SetDataToLabelMoiBenhNhan);
                //StartTimer(ModuleLink, "timer1");
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableLayout()
        {
            try
            {
                this.layoutControlItem14.Visibility = this.layoutControlItem16.Visibility = isTach ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (!isTach)
                {
                    gridControlWaitingCls.Dock = DockStyle.Fill;
                    labelControlChLM.Width = this.Width;
                    labelControlChLM.Text = "Danh sách chờ";
                }
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
                //RegisterTimer(ModuleLink, "timerForScrollListPatient", 2000, timerForScrollListPatientProcess);
                //RegisterTimer(ModuleLink, "timerSetDataToGridControl", WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000, SetDataToGridControlCLS);
                //RegisterTimer(ModuleLink, "timerAutoLoadDataPatient", WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000, LoadWaitingPatientForWaitingScreen);
                //RegisterTimer(ModuleLink, "timerForHightLightCallPatientLayout", WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000, SetDataToCurentCallPatientUsingThread);

                timerForScrollListPatient.Interval = 2000;
                timerForScrollListPatient.Enabled = true;
                timerForScrollListPatient.Start();

                timerSetDataToGridControl.Interval = WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000;
                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Start();

                timerAutoLoadDataPatient.Interval = WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000;
                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Start();

                timerForHightLightCallPatientLayout.Interval = WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000;
                timerForHightLightCallPatientLayout.Enabled = true;
                timerForHightLightCallPatientLayout.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToRoom(V_HIS_ROOM room)
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

        private void timerForScrollListPatientProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ScrollListPatientProcess(); }));
                }
                else
                {
                    ScrollListPatientProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ScrollListPatientProcess()
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

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                Task ts = Task.Factory.StartNew(timerForScrollListPatientProcess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadWaitingPatientForWaitingScreen()
        {
            try
            {
                Task ts = Task.Factory.StartNew(ExecuteThreadWaitingPatientToCall);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ExecuteThreadWaitingPatientToCall()
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { StartTheadWaitingPatientToCall(); }));
                //}
                //else
                //{
                StartTheadWaitingPatientToCall();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void StartTheadWaitingPatientToCall()
        {
            FillDataToDictionaryWaitingPatient();
        }

        private void SetFromConfigToControl()
        {
            try
            {
                organizationName = WaitingScreenCFG.ORGANIZATION_NAME;
                // mau phong xu ly
                List<int> roomNameColorCodes = WaitingScreenCFG.ROOM_NAME_FORCE_COLOR_CODES;
                if (roomNameColorCodes != null && roomNameColorCodes.Count == 3)
                {
                    lblRoomName.ForeColor = System.Drawing.Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                    lblMoiNguoiBenh.ForeColor = System.Drawing.Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                    lblSo.ForeColor = System.Drawing.Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                }

                // màu tên bác sĩ
                List<int> userNameColorCodes = WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES;
                if (userNameColorCodes != null && userNameColorCodes.Count == 3)
                {
                    lblDoctorName.ForeColor = System.Drawing.Color.FromArgb(userNameColorCodes[0], userNameColorCodes[1], userNameColorCodes[2]);
                }

                //mau background
                List<int> parentBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    layoutControlGroup1.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup3.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup4.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup5.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    Root.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroupMoiBenhNhan.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroupMoiBenhNhanSo.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblCoSttNhoHon.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblMoiNguoiBenh.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblPatientName.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblSoThuTuBenhNhan.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblSo.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }

                //màu chữ tên tổ chức
                //List<int> organizationColorCodes = WaitingScreenCFG.ORGANIZATION_FORCE_COLOR_CODES;
                //if (organizationColorCodes != null && organizationColorCodes.Count == 3)
                //{
                //    lblSrollText.ForeColor = System.Drawing.Color.FromArgb(organizationColorCodes[0], organizationColorCodes[1], organizationColorCodes[2]);
                //}
                //gridControlWaitngCls
                //màu nền grid patients
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                //màu nền của header danh sách bệnh nhân
                List<int> gridpatientHeaderBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_HEADER_BACK_COLOR_CODES;
                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnFirstName.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnInstructionTime.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnLastName.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnServiceReqType.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnAddress.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);

                }

                //màu chữ của header danh sách bệnh nhân
                List<int> gridpatientHeaderForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_HEADER_FORCE_COLOR_CODES;
                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnFirstName.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnInstructionTime.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnLastName.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnServiceReqType.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnSTT.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnAddress.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                }

                //màu chữ của body danh sách bệnh nhân
                gridpatientBodyForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES;
                if (gridpatientBodyForceColorCodes != null && gridpatientBodyForceColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnFirstName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnInstructionTime.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb
(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnLastName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb
(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqType.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnAddress.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblPatientName.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblSoThuTuBenhNhan.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                //màu chữ của trạng thái yêu cầu là mới
                newStatusForceColorCodes = WaitingScreenCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;

            }
            catch (Exception ex)
            {
            }
        }

        private void gridViewWatingExams_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "INSTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.INTRUCTION_TIME);
                        }
                        if (e.Column.FieldName == "AGE_DISPLAY")
                        {
                            e.Value = AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_PATIENT_TYPE GetPatientTypeName(long ID)
        {
            try
            {
                if (ID == 0) return new HIS_PATIENT_TYPE();
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(s => s.ID == ID).FirstOrDefault();
                return data ?? new HIS_PATIENT_TYPE();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
        private void gridViewWaitingCls_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LocalStorage.BackendData.ADO.ServiceReq1ADO data = (LocalStorage.BackendData.ADO.ServiceReq1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "AGE_DISPLAY")
                        {
                            e.Value = GetYearOld(data.TDL_PATIENT_DOB);
                        }
                        if(e.Column.FieldName == "PATIENT_TYPE_NAME")
                        {
                            e.Value = data.PAEX_LOGINNAME;// GetPatientTypeName(data.TDL_PATIENT_TYPE_ID??0).PATIENT_TYPE_NAME;
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
                //if (e.RowHandle >= 0)
                //{
                //    bool serviceReqStt = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "CallPatientSTT") ?? "").ToString());
                //    if (serviceReqStt)
                //    {
                //        e.Appearance.Font = new System.Drawing.Font("Arial", 29, FontStyle.Bold);
                //        e.HighPriority = true;
                //        e.Appearance.BackColor = Color.Blue;
                //        e.Appearance.ForeColor = Color.Yellow;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataToDictionaryWaitingPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                LisSampleViewFilter filter = new LisSampleViewFilter();

                if (room != null)
                {
                    filter.SAMPLE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }

                List<long> lstServiceReqSTT = new List<long>();
                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());//20181212121527
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                filter.INTRUCTION_TIME_FROM = startDay;
                filter.INTRUCTION_TIME_TO = endDay;
                filter.ORDER_FIELD = "CALL_SAMPLE_ORDER";
                filter.ORDER_DIRECTION = "ASC";
                filter.SAMPLE_STT_IDs = this.sampleSttIds;
                var result = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, param);
                if (result != null && result.Count > 0)
                {
                    CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertListServiceReq1ToADO(result);
                }
                else
                {
                    CallPatientDataWorker.DicCallPatient[room.ID] = new List<ServiceReq1ADO>();
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> ConnvertListServiceReq1ToADO(List<V_LIS_SAMPLE> serviceReq1s)
        {
            List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO> serviceReq1Ados = new List<LocalStorage.BackendData.ADO.ServiceReq1ADO>();
            try
            {
                serviceReq1s.ForEach(item =>
                {
                    if (item.SAMPLE_STT_ID == 5)
                    {
                        item.SAMPLE_STT_ID = 2;
                    }
                    if (item.SAMPLE_STT_ID == 6) item.SAMPLE_STT_ID = 1;
                });

                var groups = serviceReq1s.GroupBy(g => new { g.CALL_SAMPLE_ORDER, g.PATIENT_CODE, g.FIRST_NAME, g.LAST_NAME, g.DOB,g.PATIENT_TYPE_NAME,g.SAMPLE_STT_ID }).ToList();
                List<ServiceReq1ADO> lisAdos = null;
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.ContainsKey(room.ID))
                {
                    lisAdos = CallPatientDataWorker.DicCallPatient[room.ID];
                }
                foreach (var item in groups)
                {
                    ServiceReq1ADO ado = null;
                    ado = lisAdos != null ? lisAdos.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.SERVICE_REQ_CODE) && item.Any(a => o.SERVICE_REQ_CODE == a.SERVICE_REQ_CODE || o.SERVICE_REQ_CODE.Contains(a.SERVICE_REQ_CODE))) : null;
                    LocalStorage.BackendData.ADO.ServiceReq1ADO serviceReq1Ado = new LocalStorage.BackendData.ADO.ServiceReq1ADO();
                    //serviceReq1Ado.ID = item.ID;
                    serviceReq1Ado.SERVICE_REQ_CODE = String.Join(",", item.Select(s => s.SERVICE_REQ_CODE).ToList());
                    serviceReq1Ado.TDL_PATIENT_CODE = item.FirstOrDefault().PATIENT_CODE;
                    serviceReq1Ado.TDL_PATIENT_NAME = ((item.FirstOrDefault().LAST_NAME ?? "") + " " + (item.FirstOrDefault().FIRST_NAME ?? "")).Trim();
                    serviceReq1Ado.NUM_ORDER = item.FirstOrDefault().CALL_SAMPLE_ORDER;
                    serviceReq1Ado.TDL_PATIENT_DOB = item.FirstOrDefault().DOB ?? 0;
                    serviceReq1Ado.SERVICE_REQ_STT_ID = item.FirstOrDefault().SAMPLE_STT_ID;
                    serviceReq1Ado.PAEX_LOGINNAME = item.FirstOrDefault().PATIENT_TYPE_NAME;
                    serviceReq1Ado.PRIORITY = item.FirstOrDefault().PRIORITY;
                    

                    if (ado != null && ado.CallPatientSTT)
                    {
                        serviceReq1Ado.CallPatientSTT = true;
                    }
                    else
                    {
                        serviceReq1Ado.CallPatientSTT = false;
                    }

                    serviceReq1Ados.Add(serviceReq1Ado);
                }
                serviceReq1Ados = serviceReq1Ados.OrderByDescending(o => o.CallPatientSTT).ToList();
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
                    lblPatientName.Text = serviceReq1ADO.TDL_PATIENT_NAME;
                    lblSoThuTuBenhNhan.Text = serviceReq1ADO.NUM_ORDER + "";
                }
                else if (!isSetNum)
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

        private void SetDataToLabelMoiBenhNhanChild()
        {
            try
            {
                if (ServiceReq1ADOWorker.ServiceReq1ADO != null && ServiceReq1ADOWorker.ServiceReq1ADO.CallPatientSTT)
                {
                    SetDataToCurrentPatientCall(ServiceReq1ADOWorker.ServiceReq1ADO);
                }
                else if (!isSetNum)
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
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {
                    ServiceReq1ADO PatientIsCall = (this.sampleSttIds != null && this.sampleSttIds.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.sampleSttIds.Contains(o.SERVICE_REQ_STT_ID)).FirstOrDefault(o => o.CallPatientSTT) : null;
                    Inventec.Common.Logging.LogSystem.Info("SetDataToCurrentCallPatient() tDu lieu PatientIsCall:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientIsCall), PatientIsCall));

                    if (PatientIsCall != null)
                    {
                        isSetNum = false;
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
                        Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 5");
                        ServiceReq1ADOWorker.ServiceReq1ADO = null;
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

        private void SetDataToGridControlWaitingCLSs()
        {
            try
            {
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {
                    int countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                    if (countPatient == 0)
                        countPatient = 10;
                    if (this.isTach)
                    {
                        var ServiceReqFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => o.SERVICE_REQ_STT_ID == 1 || o.SERVICE_REQ_STT_ID == 6).ToList();
                        if (gridControlWaitingCls.InvokeRequired)
                        {
                            gridControlWaitingCls.Invoke(new MethodInvoker(delegate
                            {
                                gridControlWaitingCls.BeginUpdate();
                                gridControlWaitingCls.DataSource = ServiceReqFilterSTTs;
                                gridControlWaitingCls.EndUpdate();
                            }));
                        }
                        else
                        {
                            gridControlWaitingCls.BeginUpdate();
                            gridControlWaitingCls.DataSource = ServiceReqFilterSTTs;
                            gridControlWaitingCls.EndUpdate();
                        }
                        var ServiceReqFilterSTTsDa = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => o.SERVICE_REQ_STT_ID == 2 || o.SERVICE_REQ_STT_ID == 5).ToList();
                        if (gridControlDaLM.InvokeRequired)
                        {
                            gridControlDaLM.Invoke(new MethodInvoker(delegate
                            {
                                gridControlDaLM.BeginUpdate();
                                gridControlDaLM.DataSource = ServiceReqFilterSTTsDa;
                                gridControlDaLM.EndUpdate();
                            }));
                        }
                        else
                        {
                            gridControlDaLM.BeginUpdate();
                            gridControlDaLM.DataSource = ServiceReqFilterSTTsDa;
                            gridControlDaLM.EndUpdate();
                        }
                        
                    }
                    else
                    {
                        // danh sách chờ kết quả cận lâm sàng
                        var ServiceReqFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.sampleSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList();
                        gridControlWaitingCls.Invoke(new MethodInvoker(delegate
                        {
                            gridControlWaitingCls.BeginUpdate();
                            gridControlWaitingCls.DataSource = ServiceReqFilterSTTs;
                            gridControlWaitingCls.EndUpdate();
                        }));
                        Inventec.Common.Logging.LogSystem.Info("Du lieu DicCallPatient:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()), CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()));
                    }
                    
                    
                }
                else
                {
                    gridControlWaitingCls.Invoke(new MethodInvoker(delegate
                       {
                           gridControlWaitingCls.BeginUpdate();
                           gridControlWaitingCls.DataSource = null;
                           gridControlWaitingCls.EndUpdate();
                       }));
                    gridControlDaLM.Invoke(new MethodInvoker(delegate
                    {
                        gridControlDaLM.BeginUpdate();
                        gridControlDaLM.DataSource = null;
                        gridControlDaLM.EndUpdate();
                    }));
                }
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
                Task ts = Task.Factory.StartNew(executeThreadSetDataToGridControl);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetDataToCurentCallPatientUsingThread()
        {
            try
            {
                Task ts = Task.Factory.StartNew(executeThreadSetDataToCurentCallPatient);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetDataToLabelMoiBenhNhan()
        {
            try
            {
                Task ts = Task.Factory.StartNew(executeThreadSetDataToLabelMoiBenhNhan);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

                //SetDataToCurrentCallPatient();
                SetDataToCurentCallPatientUsingThread();
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

        public void CallNumOrder(int min, int max)
        {
            try
            {
                isSetNum = true;
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.ContainsKey(room.ID))
                {
                    CallPatientDataWorker.DicCallPatient[room.ID].ForEach(o => o.CallPatientSTT = false);
                }
                if (ServiceReq1ADOWorker.ServiceReq1ADO != null)
                {
                    ServiceReq1ADOWorker.ServiceReq1ADO.CallPatientSTT = false;
                }
                if (min == max)
                {
                    lblPatientName.Invoke(new MethodInvoker(delegate
                    {
                        lblPatientName.Text = "CÓ SỐ THỨ TỰ";
                    }));
                    lblSoThuTuBenhNhan.Invoke(new MethodInvoker(delegate
                    {
                        lblSoThuTuBenhNhan.Text = min + "";
                    }));
                }
                else
                {
                    lblPatientName.Invoke(new MethodInvoker(delegate
                    {
                        lblPatientName.Text = "CÓ SỐ THỨ TỰ TỪ";
                    }));
                    lblSoThuTuBenhNhan.Invoke(new MethodInvoker(delegate
                    {
                        lblSoThuTuBenhNhan.Text = min + " - " + max;
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmWaitingScreenSample22_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerAutoLoadDataPatient.Enabled = false;
                timerForHightLightCallPatientLayout.Enabled = false;
                timerForScrollListPatient.Enabled = false;
                timerSetDataToGridControl.Enabled = false;
                timer1.Enabled = false;

                timerAutoLoadDataPatient.Stop();
                timerForHightLightCallPatientLayout.Stop();
                timerForScrollListPatient.Stop();
                timerSetDataToGridControl.Stop();
                timer1.Stop();

                timerAutoLoadDataPatient.Dispose();
                timerForHightLightCallPatientLayout.Dispose();
                timerForScrollListPatient.Dispose();
                timerSetDataToGridControl.Dispose();
                timer1.Dispose();

                ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaitingCls_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                // Lấy dữ liệu của hàng hiện tại bằng e.RowHandle
                LocalStorage.BackendData.ADO.ServiceReq1ADO data = view.GetRow(e.RowHandle) as LocalStorage.BackendData.ADO.ServiceReq1ADO;

                if (data != null)
                {
                    if (data.PRIORITY != null && data.PRIORITY > 0)
                    {
                        // In đậm chữ nếu PRIORITY > 0
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        // Bình thường nếu không thỏa điều kiện
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDaLM_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LocalStorage.BackendData.ADO.ServiceReq1ADO data = (LocalStorage.BackendData.ADO.ServiceReq1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "AGE_DISPLAY")
                        {
                            e.Value = GetYearOld(data.TDL_PATIENT_DOB);
                        }
                        if (e.Column.FieldName == "PATIENT_TYPE_NAME")
                        {
                            e.Value = data.PAEX_LOGINNAME; //Map doi tuong bn vao truong nay //GetPatientTypeName(data.TDL_PATIENT_TYPE_ID ?? 0).PATIENT_TYPE_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDaLM_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                // Lấy dữ liệu của hàng hiện tại bằng e.RowHandle
                LocalStorage.BackendData.ADO.ServiceReq1ADO data = view.GetRow(e.RowHandle) as LocalStorage.BackendData.ADO.ServiceReq1ADO;

                if (data != null)
                {
                    if (data.PRIORITY != null && data.PRIORITY > 0)
                    {
                        // In đậm chữ nếu PRIORITY > 0
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        // Bình thường nếu không thỏa điều kiện
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewWaitingCls_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TDL_PATIENT_NAME")
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                    LocalStorage.BackendData.ADO.ServiceReq1ADO data = (LocalStorage.BackendData.ADO.ServiceReq1ADO)view.GetRow(e.ListSourceRowIndex);

                    if (data != null && data.PRIORITY != null && data.PRIORITY > 0)
                    {
                       
                        e.DisplayText = string.Format("{0}(ƯT)",e.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDaLM_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TDL_PATIENT_NAME")
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                    LocalStorage.BackendData.ADO.ServiceReq1ADO data = (LocalStorage.BackendData.ADO.ServiceReq1ADO)view.GetRow(e.ListSourceRowIndex);

                    if (data != null && data.PRIORITY != null && data.PRIORITY > 0)
                    {

                        e.DisplayText = string.Format("{0}(ƯT)", e.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmWaitingScreenSample22_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
