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

namespace HIS.Desktop.Plugins.CallPatientV6
{
    public partial class frmWaitingScreen_V48 : FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        //internal MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        string[] FilePath;
        List<int> gridpatientBodyForceColorCodes;
        int index = 0;
        int rowCount = 0;
        List<V_HIS_EXECUTE_ROOM> roomSelecteds;
        long planTimeFrom, planTimeTo;
        const string moduleLink = "HIS.Desktop.Plugins.CallPatientV6";

        public frmWaitingScreen_V48(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts, List<V_HIS_EXECUTE_ROOM> roomSelecteds, long planTimeFrom, long planTimeTo)
        {
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this.roomSelecteds = roomSelecteds;
            this.planTimeFrom = planTimeFrom;
            this.planTimeTo = planTimeTo;
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

        private void SetFromConfigToControl()
        {
            try
            {
                organizationName = WaitingScreenCFG.ORGANIZATION_NAME;
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
                    lblLuuY.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblNNhaBNXemTG.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    lblVuiLong.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }

                // màu background grid danh sách bệnh nhân
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnGioMoXong.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnInstructionTime.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnPhong.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    gridColumnPhuongPhapPhauThuat.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
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
                    gridColumnGioMoXong.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnInstructionTime.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnPhong.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnPhuongPhapPhauThuat.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
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
                    gridColumnGioMoXong.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnPhong.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnPhuongPhapPhauThuat.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
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
                    gridColumnPhuongPhapPhauThuat.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnPhong.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnGioMoXong.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnFirstName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnInstructionTime.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb
(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnLastName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb
(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqType.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnGenderName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblLuuY.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblNNhaBNXemTG.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    lblVuiLong.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                //màu chữ của trạng thái yêu cầu là mới
                newStatusForceColorCodes = WaitingScreenCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;
                // cỡ chữ tên phòng và tên bác sĩ
                lblDoctorName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_PHONG_VA_TEN_BAC_SI, FontStyle.Bold);

                // cỡ chữ  tiêu đề danh sách bn
                gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGenderName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);

                gridColumnGioMoXong.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnPhong.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnPhuongPhapPhauThuat.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);

                // cỡ chữ nội dung danh sách BN
                gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGenderName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);

                gridColumnPhuongPhapPhauThuat.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnPhong.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnGioMoXong.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);

                //cỡ chữ tên bệnh nhân đang được gọi
                lblLuuY.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);
                lblNNhaBNXemTG.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);
                lblVuiLong.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI, FontStyle.Bold);

                // chiều cao dòng nội dung, tiêu đề ds bn
                gridViewWaitingCls.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWaitingCls.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

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
                    V_HIS_SERE_SERV_1 data = (V_HIS_SERE_SERV_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "AGE_DISPLAY")
                        {
                            e.Value = AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "TDL_EXECUTE_ROOM_NAME")
                        {
                            var executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == data.TDL_EXECUTE_ROOM_ID);
                            e.Value = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : "";
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_STR")
                        {
                            string spanTimeFrom = data.PLAN_TIME_FROM.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.PLAN_TIME_FROM.Value) : "";

                            string spanTimeTo = data.PLAN_TIME_TO.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.PLAN_TIME_TO.Value) : "";

                            string spanTimeFromStr = !String.IsNullOrWhiteSpace(spanTimeFrom) && spanTimeFrom.Length > 5 ? spanTimeFrom.Substring(spanTimeFrom.Length - 5, 5) : "";

                            string spanTimeToStr = !String.IsNullOrWhiteSpace(spanTimeTo) && spanTimeTo.Length > 5 ? spanTimeTo.Substring(spanTimeTo.Length - 5, 5) : "";

                            e.Value = spanTimeFromStr + " - " + spanTimeToStr;
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
                //GridView View = sender as GridView;
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

        void FillDataToDictionaryWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
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
                lblDoctorName.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));
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
