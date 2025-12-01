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
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientV4.Config;
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

namespace HIS.Desktop.Plugins.CallPatientV4
{
    public partial class frmWaitingScreen_V4_SeparateScreen : FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        internal static string[] FilePath;
        List<int> gridpatientBodyForceColorCodes;
        int index = 0;
        int rowCount = 0;
        long serviceReqIdOld = 0;
        int countTimer = 0;
        List<long> serviceReqForClsIds = null;
        List<long> serviceReqSttIds = null;
        List<ServiceReq1ADO> tempListServiceReq;
        List<ServiceReq1ADO> tempListServiceReq_NoiTru_hoac_BanNgay;
        List<ServiceReq1ADO> tempListServiceReq_NgoaiTru_hoac_Kham;
        Inventec.Desktop.Common.Modules.Module currentModule;
        //qtcode
        private List<ControlStateRDO> currentControlStateRDO;
        private bool isSeparateInOut = false;
        private bool isSeparateEmergency = false;
        public frmWaitingScreen_V4_SeparateScreen(Inventec.Desktop.Common.Modules.Module module
        , MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts, List<ControlStateRDO> controlStateRDO = null)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("Dữ liệu lưu cache " + controlStateRDO);
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this.currentControlStateRDO = controlStateRDO;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                FillDataToDictionaryWaitingPatient(serviceReqStts);
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                GetFilePath();
                StartAllTimer();
                timerChangeColorRow.Interval = 1000;
                timerChangeColorRow.Enabled = true;
                timerChangeColorRow.Start();//TODO

                SetFromConfigToControl();
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaitingCls.RowCount - 1;
                SetFormFrontOfAll();
                timer1.Interval = 1000;
                timer1.Enabled = true;
                timer1.Start();
                CallPatientDataWorker.DicDelegateCallingPatient[this.room.ID] = (DelegateSelectData)this.nhapNhay;
                SetIcon();
                //qtcode
                LoadControlState();

                // tiêu đề nội trú, ngoại trú
                ApplyTitleConfig();

                // bệnh nhân cấp cứu
                ApplyEmergencyMode();

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

                timerSetDataToGridControl.Interval = (WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000);
                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Start();

                timerAutoLoadDataPatient.Interval = (WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000);
                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Start();

                timerForHightLightCallPatientLayout.Interval = (WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000);
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


        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                ScrollListPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ScrollListPatient()
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
            LoadWaitingPatientForWaitingScreen();
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

        private void SetFromConfigToControl()
        {
            try
            {
                organizationName = WaitingScreenCFG.ORGANIZATION_NAME;
                //timerSetDataToGridControl.Interval = 50000;
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
                //List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                //if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                //{
                //    gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                //}


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

                    gridColumnAge2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnLastName2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnSTT2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
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

                    gridColumnAge2.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnLastName2.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnSTT2.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
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

                    gridColumnAge2.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnLastName2.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnSTT2.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
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
                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ data = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaitingCls2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
                var result = (HIS_SERVICE_REQ)gridViewWaitingCls.GetRow(e.RowHandle);

                if (result != null && this.hisServiceReq != null)
                {
                    if (result.NUM_ORDER == this.hisServiceReq.NUM_ORDER)
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
                // qtcode
                if (!isSeparateEmergency) return;
                var data = gridViewWaitingCls.GetRow(e.RowHandle) as ServiceReq1ADO;
                if (data == null) return;

                var dept = BackendDataWorker.Get<V_HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == data.REQUEST_DEPARTMENT_ID);

                if (dept != null && dept.IS_EMERGENCY == 1)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.HighPriority = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridViewWaitingCls2_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                var result = (HIS_SERVICE_REQ)gridViewWaitingCls2.GetRow(e.RowHandle);

                if (result != null && this.hisServiceReq != null)
                {
                    if (result.NUM_ORDER == this.hisServiceReq.NUM_ORDER)
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
                // qtcode

                if (!isSeparateEmergency) return;
                var data = gridViewWaitingCls2.GetRow(e.RowHandle) as ServiceReq1ADO;
                if (data == null) return;

                var dept = BackendDataWorker.Get<V_HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == data.REQUEST_DEPARTMENT_ID);

                if (dept != null && dept.IS_EMERGENCY == 1)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.HighPriority = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        void FillDataToDictionaryWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter hisServiceReqFilter = new HisServiceReqFilter();

                if (room != null)
                {
                    hisServiceReqFilter.EXECUTE_ROOM_ID = room.ID;
                }

                hisServiceReqFilter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                List<long> lstServiceReqSTT = new List<long>();
                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());//20181212121527
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                hisServiceReqFilter.HAS_EXECUTE = true;
                hisServiceReqFilter.INTRUCTION_DATE_FROM = startDay;
                hisServiceReqFilter.INTRUCTION_DATE_TO = endDay;

                hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";
                hisServiceReqFilter.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                hisServiceReqFilter.ORDER_FIELD2 = "PRIORITY";
                hisServiceReqFilter.ORDER_FIELD3 = "NUM_ORDER";

                hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION1 = "ASC";
                hisServiceReqFilter.ORDER_DIRECTION2 = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION3 = "ASC";


                if (serviceReqStts != null && serviceReqStts.Count > 0)
                {
                    List<long> lstServiceReqSTTFilter = serviceReqStts.Select(o => o.ID).ToList();
                    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTTFilter;
                }
                var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                //var result = BackendDataWorker.Get<HIS_SERVICE_REQ>().ToList();
                if (result != null && result.Count > 0)
                {
                    //CallPatientDataUpdateDictionary.UpdateDictionaryPatient(room.ID, ConnvertListServiceReq1ToADO(result));
                    CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertListServiceReq1ToADO(result);
                }
                else
                {
                    CallPatientDataWorker.DicCallPatient[room.ID] = new List<ServiceReq1ADO>();
                }
                //lblPatientName.Text = "";
                //lblSoThuTuBenhNhan.Text = "";
                lblPatientName.ForeColor = Color.Red;
                lblSoThuTuBenhNhan.ForeColor = Color.Red;

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
                    if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                    {
                        var checkTreatment = CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.ID == item.ID && o.CallPatientSTT);
                        if (checkTreatment != null)
                        {
                            serviceReq1Ado.CallPatientSTT = true;
                        }
                        else
                        {
                            serviceReq1Ado.CallPatientSTT = false;
                        }
                    }
                    else
                    {
                        serviceReq1Ado.CallPatientSTT = false;
                    }

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
                lblMoiNguoiBenh.Text = "MỜI NGƯỜI BỆNH";
                lblCoSttNhoHon.Text = "HOẶC NGƯỜI BỆNH CÓ SỐ NHỎ HƠN";
                if (serviceReq1ADO != null)
                {
                    lblPatientName.Text = serviceReq1ADO.TDL_PATIENT_NAME;
                    lblSoThuTuBenhNhan.Text = serviceReq1ADO.NUM_ORDER + "";
                    lblPatientName.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblSoThuTuBenhNhan.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }
                else
                {
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
                if (ServiceReq1ADOWorker.ServiceReq2ADO != null && ServiceReq1ADOWorker.ServiceReq2ADO.Count() > 1)
                {
                    SetDataToCurrentPatientsCall(ServiceReq1ADOWorker.ServiceReq2ADO);
                }
                else if (ServiceReq1ADOWorker.ServiceReq1ADO != null)
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

        private void SetDataToCurrentPatientsCall(List<ServiceReq1ADO> ServiceReq2ADOs)
        {
            try
            {
                if (ServiceReq2ADOs != null && ServiceReq2ADOs.Count() > 1)
                {
                    string sttPatient = "";
                    foreach (var item in ServiceReq2ADOs)
                        sttPatient += (item.NUM_ORDER + ", ");
                    lblSoThuTuBenhNhan.Text = sttPatient.Remove(sttPatient.LastIndexOf(','));
                }
                lblPatientName.Text = " Mời các bệnh nhân có STT sau vào phòng";
                lblPatientName.Font = new System.Drawing.Font("Arial", 28);
                lblMoiNguoiBenh.Text = "";
                lblCoSttNhoHon.Text = "";
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
                    List<ServiceReq1ADO> PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).Where(o => o.CallPatientSTT).ToList() : null;

                    if (PatientIsCall != null && PatientIsCall.Count() > 0)
                    {
                        if (ServiceReq1ADOWorker.ServiceReq1ADO == null)
                        {
                            ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall.FirstOrDefault();
                        }
                        else
                        {
                            ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall.FirstOrDefault();
                        }

                        //else if (currentServiceReq1ADO != null && (PatientIsCall.TDL_PATIENT_NAME != this.currentServiceReq1ADO.TDL_PATIENT_NAME || PatientIsCall.NUM_ORDER != this.currentServiceReq1ADO.NUM_ORDER))
                        //{
                        //    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 3");
                        //    currentServiceReq1ADO = PatientIsCall;
                        //    SetDataToCurrentPatientCall(currentServiceReq1ADO);
                        //}
                        //else
                        //{
                        //    Inventec.Common.Logging.LogSystem.Info("PatientIsCall step 4");
                        //}
                    }
                    else if (PatientIsCall != null && PatientIsCall.Count() > 1)
                        ServiceReq1ADOWorker.ServiceReq2ADO = PatientIsCall;

                    else
                    {
                        ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
                        //SetDataToCurrentPatientCall(currentServiceReq1ADO);
                    }
                }
                else
                {
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

                    // danh sách chờ kết quả cận lâm sàng
                    var ServiceReqFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).OrderBy(o => o.RESULTING_ORDER).ThenBy(o => o.NUM_ORDER).ToList();
                    this.tempListServiceReq = new List<ServiceReq1ADO>();
                    this.tempListServiceReq = ServiceReqFilterSTTs;

                    var ServiceReqFilterSTTs_NoiTru_hoac_BanNgay = ServiceReqFilterSTTs.Where(o => o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY).ToList();
                    var ServiceReqFilterSTTs_NgoaiTru_hoac_Kham = ServiceReqFilterSTTs.Where(o => o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).ToList();

                    this.tempListServiceReq_NoiTru_hoac_BanNgay = ServiceReqFilterSTTs_NoiTru_hoac_BanNgay;
                    this.tempListServiceReq_NgoaiTru_hoac_Kham = ServiceReqFilterSTTs_NgoaiTru_hoac_Kham;

                    //gridControlWaitingCls.DataSource = null;
                    gridControlWaitingCls.BeginUpdate();
                    gridControlWaitingCls.DataSource = ServiceReqFilterSTTs_NoiTru_hoac_BanNgay;
                    gridControlWaitingCls.EndUpdate();

                    gridControlWaitingCls2.BeginUpdate();
                    gridControlWaitingCls2.DataSource = ServiceReqFilterSTTs_NgoaiTru_hoac_Kham;
                    gridControlWaitingCls2.EndUpdate();
                    //Inventec.Common.Logging.LogSystem.Info("Du lieu DicCallPatient:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()), CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()));
                }
                else
                {
                    gridControlWaitingCls.BeginUpdate();
                    gridControlWaitingCls.DataSource = null;
                    gridControlWaitingCls.EndUpdate();

                    gridControlWaitingCls2.BeginUpdate();
                    gridControlWaitingCls2.DataSource = null;
                    gridControlWaitingCls2.EndUpdate();
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
                //if (this.countTimer > 0)
                //{
                //    timerSetDataToGridControl.Interval = 1000;
                //}
                //else
                //{
                //    timerSetDataToGridControl.Interval = 5000;
                //}
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

                ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
                //CallPatientDataWorker.DicDelegateCallingPatient[this.room.ID] = null;
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

        private void nhapNhay(object data)
        {
            try
            {
                ///tao thread xu ly nhap nhay
                ///
                this.countTimer = 8;
                this.hisServiceReq = new HIS_SERVICE_REQ();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(this.hisServiceReq, data);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        void SetDataForChangeColor()
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
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { StartTheadSetDataForChangeColor(); }));
                }
                else
                {
                    StartTheadSetDataForChangeColor();
                }
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
                //gridControlWaitingCls.DataSource = null;
                gridControlWaitingCls.BeginUpdate();
                gridControlWaitingCls.DataSource = this.tempListServiceReq_NoiTru_hoac_BanNgay;
                gridControlWaitingCls.EndUpdate();

                gridControlWaitingCls2.BeginUpdate();
                gridControlWaitingCls2.DataSource = this.tempListServiceReq_NgoaiTru_hoac_Kham;
                gridControlWaitingCls2.EndUpdate();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null) return;

                // Checkbox phân biệt nội trú, ngoại trú
                Inventec.Common.Logging.LogSystem.Debug("thangdq1 ds state: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", this.currentControlStateRDO));
                Inventec.Common.Logging.LogSystem.Debug("thangdq1 key state noi ngoai: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru).Select(o => o.KEY)));
                Inventec.Common.Logging.LogSystem.Debug("thangdq1 modulink noi ngoai: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru).Select(o => o.MODULE_LINK)));
                //Inventec.Common.Logging.LogSystem.Debug("thangdq1 modulink current: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", this.currentModule.ModuleLink));
                var separateInOut = this.currentControlStateRDO
                    .FirstOrDefault(o => o.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru && o.MODULE_LINK == "HIS.Desktop.Plugins.CallPatientV4");
                isSeparateInOut = (separateInOut != null && separateInOut.VALUE == "1");

                // Checkbox phân biệt cấp cứu
                var separateEmergency = this.currentControlStateRDO
                    .FirstOrDefault(o => o.KEY == ControlStateConstant.chkSeparatePatientCapCuu && o.MODULE_LINK == "HIS.Desktop.Plugins.CallPatientV4");
                isSeparateEmergency = (separateEmergency != null && separateEmergency.VALUE == "1");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void ApplyTitleConfig()
        {
            try
            {
                if (!isSeparateInOut || this.currentControlStateRDO == null)
                {
                    lblTitleNoiTru.Visible = false;
                    lblTitleNgoaiTru.Visible = false;
                    return;
                }

                string titleNoiTru = GetControlStateValue("CallPatientV4_TitleNoiTru") ?? "";
                string titleNgoaiTru = GetControlStateValue("CallPatientV4_TitleNgoaiTru") ?? "";

                int fontSize = 28;
                string sizeStr = GetControlStateValue("CallPatientV4_TitleFontSize");
                if (!string.IsNullOrEmpty(sizeStr)) int.TryParse(sizeStr, out fontSize);

                Color color = Color.Red;
                string colorStr = GetControlStateValue("CallPatientV4_TitleFontColor");
                if (!string.IsNullOrEmpty(colorStr))
                {
                    var rgb = colorStr.Split(',').Select(int.Parse).ToArray();
                    if (rgb.Length == 3) color = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                }

                lblTitleNoiTru.Text = titleNoiTru;
                lblTitleNgoaiTru.Text = titleNgoaiTru;
                lblTitleNoiTru.Font = new Font("Arial", fontSize, FontStyle.Bold);
                lblTitleNgoaiTru.Font = new Font("Arial", fontSize, FontStyle.Bold);
                lblTitleNoiTru.ForeColor = color;
                lblTitleNgoaiTru.ForeColor = color;
                int newHeight = fontSize * 2;

                layoutControlItem15.MinSize = new Size(0, newHeight);
                layoutControlItem15.MaxSize = new Size(0, newHeight);
                layoutControlItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                lblTitleNoiTru.Visible = !string.IsNullOrWhiteSpace(titleNoiTru);
                lblTitleNgoaiTru.Visible = !string.IsNullOrWhiteSpace(titleNgoaiTru);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private string GetControlStateValue(string key)
        {
            try
            {
                var item = this.currentControlStateRDO
                //.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == this.currentModule.ModuleLink);
                .FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == "HIS.Desktop.Plugins.CallPatientV4");
                return item?.VALUE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void ApplyEmergencyMode()
        {
            try
            {
                // Kiểm tra có bệnh nhân cấp cứu nào không
                bool hasEmergencyPatient = false;
                Inventec.Common.Logging.LogSystem.Debug("Đang kiểm tra xem có bệnh nhân cấp cứu nào không");
                var allPatients = new List<ServiceReq1ADO>();
                if (CallPatientDataWorker.DicCallPatient != null &&
                    CallPatientDataWorker.DicCallPatient.ContainsKey(this.room.ID))
                {
                    allPatients.AddRange(CallPatientDataWorker.DicCallPatient[this.room.ID]);
                }

                if (allPatients.Count > 0)
                {
                    hasEmergencyPatient = allPatients.Any(p =>
                    {
                        var dept = BackendDataWorker.Get<V_HIS_DEPARTMENT>()
                            .FirstOrDefault(d => d.ID == p.REQUEST_DEPARTMENT_ID);
                        return dept != null && dept.IS_EMERGENCY == 1;
                    });
                }

                bool enableEmergencyMode = isSeparateEmergency && hasEmergencyPatient;

                if (enableEmergencyMode)
                {
                    LogSystem.Debug("Có ít nhất 1 bn cấp cứu");
                  
                    gridColumnSTT.Width = 100;
                    gridColumnAge.Width = 120;
                    gridColumnAge.Caption = "NS";

                    gridColumnSTT2.Width = 100;
                    gridColumnAge2.Width = 120;
                    gridColumnAge2.Caption = "NS";

                    var col = gridViewWaitingCls.Columns.ColumnByFieldName("ICON_DISPLAY");
                    if (col != null)
                    {
                        col.Visible = true;
                        col.VisibleIndex = 1;
                    }

                    var col2 = gridViewWaitingCls2.Columns.ColumnByFieldName("ICON_DISPLAY2");
                    if (col2 != null)
                    {
                        col2.Visible = true;
                        col2.VisibleIndex = 1;
                    }
                }
                else
                {

                    gridColumnSTT.Width = 150;
                    gridColumnAge.Width = 242;
                    gridColumnAge.Caption = "Năm sinh";

                 
                    gridColumnSTT2.Width = 150;
                    gridColumnAge2.Width = 242;
                    gridColumnAge2.Caption = "Năm sinh";
               
                    var col = gridViewWaitingCls.Columns.ColumnByFieldName("ICON_DISPLAY");
                    if (col != null)
                    {
                        col.Visible = false;
                        col.VisibleIndex = -1;
                    }

                    var col2 = gridViewWaitingCls2.Columns.ColumnByFieldName("ICON_DISPLAY2");
                    if (col2 != null)
                    {
                        col2.Visible = false;
                        col2.VisibleIndex = -1;
                    }
                }

                gridViewWaitingCls.RefreshData();
                if (isSeparateInOut) gridViewWaitingCls2.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewWaitingCls_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {

            if (e.Column.FieldName == "ICON_DISPLAY")
            {
                var data = (ServiceReq1ADO)gridViewWaitingCls.GetRow(e.RowHandle);
                var dept = BackendDataWorker.Get<V_HIS_DEPARTMENT>()
                    .FirstOrDefault(d => d.ID == data.REQUEST_DEPARTMENT_ID);

                if (dept != null && dept.IS_EMERGENCY == 1)
                {
                    e.RepositoryItem = repositoryItemButtonEdit1;
                }
                else
                {
                    e.RepositoryItem = null;
                }
            }
        }

        private void gridViewWaitingCls2_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "ICON_DISPLAY2")
            {
                var data = (ServiceReq1ADO)gridViewWaitingCls2.GetRow(e.RowHandle);
                var dept = BackendDataWorker.Get<V_HIS_DEPARTMENT>()
                    .FirstOrDefault(d => d.ID == data.REQUEST_DEPARTMENT_ID);

                if (dept != null && dept.IS_EMERGENCY == 1)
                {
                    e.RepositoryItem = repositoryItemButtonEdit2;
                }
                else
                {
                    e.RepositoryItem = null;
                }
            }
        }
    }
}
