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

namespace HIS.Desktop.Plugins.CallPatientV5
{
    public partial class frmWaitingScreen_V47 : FormBase
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
        const string moduleLink = "HIS.Desktop.Plugins.CallPatientV5";

        public frmWaitingScreen_V47(Inventec.Desktop.Common.Modules.Module module, MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts)
            : base(module)
        {
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                FillDataToDictionaryWaitingPatient(serviceReqStts);
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                //Load thông tin mong muốn
                LoadMessage();
                GetFilePath();
                StartAllTimer();
                SetFromConfigToControl();
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaitingCls.RowCount - 1;
                SetFormFrontOfAll();
                //RegisterTimer(moduleLink, "timer1", 2000, SetDataToLabelMoiBenhNhan);
                //StartTimer(moduleLink, "timer1");
                timer1.Interval = 2000;
                timer1.Enabled = true;
                timer1.Start();
                BestFitRow();
                SetIcon();
                InitRestoreLayoutGridViewFromXml(gridViewWaitingCls);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMessage()
        {
            try
            {
                string message = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.MESSAGE);
                float fontSize = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<float>(AppConfigKeys.FONT_SIZE__MESSAGE);
                if (fontSize <= 0)
                {
                    fontSize = 15;
                }

                lblMessage.Text = message.ToUpper();
                lblMessage.Font = new System.Drawing.Font(new FontFamily("Arial"), fontSize, FontStyle.Bold);
                timer2.Interval = 20;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BestFitRow()
        {
            try
            {
                gridColumnAge.BestFit();
                gridColumnGenderName.BestFit();
                gridColumnSTT.BestFit();
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
                //RegisterTimer(moduleLink, "timerForScrollListPatient", 2000, ScrollListPatientForWaitingScreenThread);
                //RegisterTimer(moduleLink, "timerSetDataToGridControl", WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000, SetDataToGridControlCLS);
                //RegisterTimer(moduleLink, "timerAutoLoadDataPatient", WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000, LoadWaitingPatientForWaitingScreen);
                //RegisterTimer(moduleLink, "timerForHightLightCallPatientLayout", WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000, SetDataToCurentCallPatientUsingThread);
                //RegisterTimer(moduleLink, "timer2", 20, Timer2Thread);

                //StartTimer(moduleLink, "timerForScrollListPatient");
                //StartTimer(moduleLink, "timerSetDataToGridControl");
                //StartTimer(moduleLink, "timerAutoLoadDataPatient");
                //StartTimer(moduleLink, "timerForHightLightCallPatientLayout");
                //StartTimer(moduleLink, "timer2");

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

                if (!String.IsNullOrEmpty(lblMessage.Text))
                {
                    timer2.Interval = 20;
                    timer2.Enabled = true;
                    timer2.Start();
                }
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
                    lblRoomName.Text = (room.ROOM_NAME).ToUpper();
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

        void ScrollListPatientForWaitingScreenThread()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadScrollListPatientForWaitingScreen);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadScrollListPatientForWaitingScreen()
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
                ScrollListPatientForWaitingScreenThread();
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
                    layoutControlGroup4.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup5.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    Root.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroupMoiBenhNhan.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroupMoiBenhNhanSo.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblMoiNguoiBenh.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblSoThuTuBenhNhan.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblSo.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }

                // màu background grid danh sách bệnh nhân
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnFirstName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnGenderName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnInstructionTime.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnLastName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnServiceReqType.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnSTT.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    //gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    //gridViewWaitingCls.Appearance.Empty.BackColor2 = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridViewWaitingCls.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
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
                    gridColumnGenderName.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);

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
                    gridColumnGenderName.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
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
                    gridColumnGenderName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblPatientName.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblSoThuTuBenhNhan.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                //màu chữ của trạng thái yêu cầu là mới
                newStatusForceColorCodes = WaitingScreenCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;
                // cỡ chữ tên phòng và tên bác sĩ
                lblRoomName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_PHONG_VA_TEN_BAC_SI, FontStyle.Bold);
                lblDoctorName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_PHONG_VA_TEN_BAC_SI, FontStyle.Bold);


                // cỡ chữ  tiêu đề danh sách bn
                gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGenderName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);

                // cỡ chữ nội dung danh sách BN
                gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGenderName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);

                //cỡ chữ tên bệnh nhân đang được gọi
                lblPatientName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);
                lblSoThuTuBenhNhan.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);

                // chiều cao dòng nội dung, tiêu đề ds bn
                gridViewWaitingCls.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWaitingCls.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                if (result != null && result.Count > 0)
                {
                    //CallPatientDataUpdateDictionary.UpdateDictionaryPatient(room.ID, ConnvertListServiceReq1ToADO(result));
                    CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertListServiceReq1ToADO(result);
                }
                else
                {
                    CallPatientDataWorker.DicCallPatient[room.ID] = new List<ServiceReq1ADO>();
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
                if (serviceReq1ADO != null)
                {
                    lblPatientName.Text = serviceReq1ADO.TDL_PATIENT_NAME;
                    lblSoThuTuBenhNhan.Text = serviceReq1ADO.NUM_ORDER + "";
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
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {
                    ServiceReq1ADO PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).FirstOrDefault(o => o.CallPatientSTT) : null;
                    Inventec.Common.Logging.LogSystem.Info("SetDataToCurrentCallPatient() tDu lieu PatientIsCall:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientIsCall), PatientIsCall));

                    if (PatientIsCall != null)
                    {
                        if (ServiceReq1ADOWorker.ServiceReq1ADO == null)
                        {
                            ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                        }
                        else
                        {
                            if (PatientIsCall.TDL_PATIENT_NAME != ServiceReq1ADOWorker.ServiceReq1ADO.TDL_PATIENT_NAME || PatientIsCall.NUM_ORDER != ServiceReq1ADOWorker.ServiceReq1ADO.NUM_ORDER)
                            {
                                ServiceReq1ADOWorker.ServiceReq1ADO = PatientIsCall;
                            }
                            else
                            {
                            }
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
                    var ServiceReqFilterSTTs = CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).ToList();
                    gridControlWaitingCls.BeginUpdate();
                    gridControlWaitingCls.DataSource = ServiceReqFilterSTTs;
                    gridControlWaitingCls.EndUpdate();
                    Inventec.Common.Logging.LogSystem.Info("Du lieu DicCallPatient:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()), CallPatientDataWorker.DicCallPatient[room.ID].Take(countPatient).ToList()));
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
                timerForHightLightCallPatientLayout.Enabled = false;
                timerForScrollListPatient.Enabled = false;
                timerSetDataToGridControl.Enabled = false;
                timer1.Enabled = false;
                timer2.Enabled = false;

                timerAutoLoadDataPatient.Stop();
                timerForHightLightCallPatientLayout.Stop();
                timerForScrollListPatient.Stop();
                timerSetDataToGridControl.Stop();
                timer1.Stop();
                timer2.Stop();

                ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();

                timerAutoLoadDataPatient.Dispose();
                timerForHightLightCallPatientLayout.Dispose();
                timerForScrollListPatient.Dispose();
                timerSetDataToGridControl.Dispose();
                timer1.Dispose();
                timer2.Dispose();
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

        int i = 10;
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                Timer2Thread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void timer2Process()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timer2Process.1" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lblMessage.Left), lblMessage.Left) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lblMessage.Width), lblMessage.Width) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ClientRectangle.Left), ClientRectangle.Left));
                if (lblMessage.Left + lblMessage.Width <= Convert.ToInt32(this.ClientRectangle.Left))
                {
                    lblMessage.Left = this.ClientRectangle.Right;
                }
                else
                {
                    lblMessage.Left -= 4;
                }

                Inventec.Common.Logging.LogSystem.Debug("timer2Process.2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lblMessage.Left), lblMessage.Left) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ClientRectangle.Left), ClientRectangle.Left));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Timer2Thread()
        {
            try
            {
                Task.Factory.StartNew(ExecuteTimer2UsingThread);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteTimer2UsingThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { timer2Process(); }));
                }
                else
                {
                    timer2Process();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
