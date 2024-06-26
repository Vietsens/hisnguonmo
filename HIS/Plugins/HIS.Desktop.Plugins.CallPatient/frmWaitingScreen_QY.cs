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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatient
{
    public partial class frmWaitingScreen_QY1 : HIS.Desktop.Utility.FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        int countTimer = 0;
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> datas = null;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        internal static string[] FileName, FilePath;
        internal static List<HIS_SERVICE_REQ> serviceReqStatics = new List<HIS_SERVICE_REQ>();
        List<long> serviceReqForClsIds = null;
        List<long> serviceReqSttIds = null;
        int countPatient = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmWaitingScreen_QY1(Inventec.Desktop.Common.Modules.Module module, MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts)
        : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                lblRoomName.Text = (room != null) ? (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper() : "";
                FillDataToGridWaitingPatient(serviceReqStts);
                GetFilePath();


                timerForScrollTextBottom.Interval = 500;
                timerForScrollTextBottom.Enabled = true;
                timerForScrollTextBottom.Start();

                timerForScrollListPatient.Start();

                
                Timer.Interval = 1000;
                Timer.Enabled = true;
                Timer.Start();
                CallPatientDataWorker.DicDelegateCallingPatient[this.room.ID] = (DelegateSelectData)this.nhapNhay;
                lblSrollText.Text = "";
                setFromConfigToControl();

                timerAutoLoadDataPatient.Interval = WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000;
                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Tick += new System.EventHandler(this.timerAutoLoadDataPatient_Tick);
                timerAutoLoadDataPatient.Start();
                datas = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>)gridControlWaitingCls.DataSource;
                SetIcon();
                InitRestoreLayoutGridViewFromXml(gridViewWaitingCls);
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
                countTimer++;
                //lblSrollText.BeginUpdate();
                if (countTimer == 1 || countTimer == 3 || countTimer == 5 || countTimer == 7 || countTimer == 9)
                {
                    lblSrollText.ForeColor = System.Drawing.Color.FromArgb(40, 255, 40);
                }
                if (countTimer == 10 || countTimer == 2 || countTimer == 4 || countTimer == 6 || countTimer == 8)
                {
                    lblSrollText.ForeColor = Color.White;
                }

                if (countTimer > 10)
                {
                    timerForHightLightCallPatientLayout.Stop();
                    timerForScrollTextBottom.Start();
                    // màu chữ tên tổ chức
                    List<int> organizationColorCodes = WaitingScreenCFG.ORGANIZATION_FORCE_COLOR_CODES;
                    if (organizationColorCodes != null && organizationColorCodes.Count == 3)
                    {
                        lblSrollText.ForeColor = System.Drawing.Color.FromArgb(organizationColorCodes[0], organizationColorCodes[1], organizationColorCodes[2]);
                    }
                    countTimer = 0;
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

        private void timerForScrollTextBottom_Tick(object sender, EventArgs e)
        {
            ScrollLabel();
        }

        private void ScrollLabel()
        {
            string strString = "                                                                                          " + organizationName + "                                                                                          ";
            int lengthStr = strString.Length;

            scrll = scrll + 1;
            int iLmt = strString.Length - scrll;
            if (iLmt < lengthStr - 150)
            {
                scrll = 0;
            }
            string str = strString.Substring(scrll, lengthStr - 150);
            lblSrollText.Text = str;
        }

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {

        }

        private void timerAutoLoadDataPatient_Tick(object sender, EventArgs e)
        {
            LoadWaitingPatientForWaitingScreen();
        }

        public void LoadWaitingPatientForWaitingScreen()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadWaitingPatientToCall));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }
        internal void executeThreadWaitingPatientToCall()
        {
            try
            {
                StartTheadWaitingPatientToCall();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void StartTheadWaitingPatientToCall()
        {
            FillDataToGridWaitingPatient(serviceReqStts);
        }
        private void setFromConfigToControl()
        {
            try
            {

                // mau phong xu ly
                List<int> roomNameColorCodes = WaitingScreenCFG.ROOM_NAME_FORCE_COLOR_CODES;
                if (roomNameColorCodes != null && roomNameColorCodes.Count == 3)
                {
                    lblRoomName.ForeColor = System.Drawing.Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                }

                //ten benh vien
                organizationName = WaitingScreenCFG.ORGANIZATION_NAME;

                //mau background
                List<int> parentBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    layoutControlGroup1.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup2.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup3.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup4.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup5.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup6.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    Root.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }
                ////Mau background phong xu ly
                //List<int> roomtBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                //if (roomtBackColorCodes != null && roomtBackColorCodes.Count == 3)
                //{
                //    layoutControlItem1.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                //}
                // màu chữ bác sĩ
                List<int> userNameColorCodes = WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES;
                if (userNameColorCodes != null && userNameColorCodes.Count == 3)
                {
                    lblDoctorName.ForeColor = System.Drawing.Color.FromArgb(userNameColorCodes[0], userNameColorCodes[1], userNameColorCodes[2]);
                }

                // màu chữ tiêu đề danh sách chờ khám
                List<int> waitingExamColorCodes = WaitingScreenCFG.WAITING_EXAM_FORCE_COLOR_CODES;
                if (waitingExamColorCodes != null && waitingExamColorCodes.Count == 3)
                {
                    lblWatingExams.ForeColor = System.Drawing.Color.FromArgb(waitingExamColorCodes[0], waitingExamColorCodes[1], waitingExamColorCodes[2]);
                }

                // màu chữ tiêu đề danh sách chờ khám
                List<int> waitingClsColorCodes = WaitingScreenCFG.WAITING_CLS_FORCE_COLOR_CODES;
                if (waitingClsColorCodes != null && waitingClsColorCodes.Count == 3)
                {
                    lblWatingCls.ForeColor = System.Drawing.Color.FromArgb(waitingClsColorCodes[0], waitingClsColorCodes[1], waitingClsColorCodes[2]);
                }

                // màu chữ tên tổ chức
                List<int> organizationColorCodes = WaitingScreenCFG.ORGANIZATION_FORCE_COLOR_CODES;
                if (organizationColorCodes != null && organizationColorCodes.Count == 3)
                {
                    lblSrollText.ForeColor = System.Drawing.Color.FromArgb(organizationColorCodes[0], organizationColorCodes[1], organizationColorCodes[2]);
                }
                // gridControlWaitngCls
                // màu nền grid patients
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                // màu nền của header danh sách bệnh nhân
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
                    gridColumn1.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                }

                // màu chữ của header danh sách bệnh nhân
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
                    gridColumn1.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                }

                // màu chữ của body danh sách bệnh nhân
                List<int> gridpatientBodyForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES;
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
                    gridColumn1.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                // gridControlWaitingExam

                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridViewWatingExams.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                // màu nền của header danh sách bệnh nhân

                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    gridColumnAgeExam.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnFirstNameExam.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnLastNameExam.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnSTTExam.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumn5.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumn6.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumn7.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                }

                // màu chữ của header danh sách bệnh nhân

                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    gridColumnAgeExam.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnFirstNameExam.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnLastNameExam.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumnSTTExam.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumn5.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumn6.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumn7.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                }

                // màu chữ của body danh sách bệnh nhân

                if (gridpatientBodyForceColorCodes != null && gridpatientBodyForceColorCodes.Count == 3)
                {
                    gridColumnAgeExam.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnFirstNameExam.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnLastNameExam.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnSTTExam.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumn5.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumn6.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumn7.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);

                }

                // màu chữ của trạng thái yêu cầu là mới
                newStatusForceColorCodes = WaitingScreenCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;

                // cỡ chữ tên phòng và tên bác sĩ
                lblRoomName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_PHONG_VA_TEN_BAC_SI, FontStyle.Bold);
                lblDoctorName.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TEN_PHONG_VA_TEN_BAC_SI, FontStyle.Bold);

                // cỡ chữ  tiêu đề danh sách bn
                gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnFirstName.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn1.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);


                gridColumnSTTExam.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastNameExam.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnFirstNameExam.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAgeExam.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn5.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn6.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn7.AppearanceHeader.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__TIEU_DE_DS_BENH_NHAN, FontStyle.Bold);

                // cỡ chữ nội dung danh sách BN
                gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnFirstName.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAge.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn1.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);

                gridColumnSTTExam.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnLastNameExam.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnFirstNameExam.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumnAgeExam.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn5.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn6.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                gridColumn7.AppearanceCell.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN, FontStyle.Bold);
                // chiều cao dòng nội dung, tiêu đề ds bn
                gridViewWaitingCls.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWaitingCls.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

                gridViewWatingExams.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWatingExams.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

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

        private void gridViewWaitingCls_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    int serviceReqStt = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString());
                    if (serviceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && newStatusForceColorCodes != null && newStatusForceColorCodes.Count == 3)
                    {
                        e.Appearance.Font = new System.Drawing.Font("Tahoma", 22);
                        e.HighPriority = true;
                        e.Appearance.ForeColor = System.Drawing.Color.FromArgb(newStatusForceColorCodes[0], newStatusForceColorCodes[1], newStatusForceColorCodes[2]);
                    }
                }
                var result = (HIS_SERVICE_REQ)gridViewWaitingCls.GetRow(e.RowHandle);

                if (result != null && this.hisServiceReq != null)
                {

                    if (result.NUM_ORDER == this.hisServiceReq.NUM_ORDER)
                    {
                        countTimer--;
                        if (countTimer == 7 || countTimer == 5 || countTimer == 3 || countTimer == 1)
                        {
                            e.HighPriority = true;
                            e.Appearance.ForeColor = System.Drawing.Color.Red;
                        }
                        //if (countTimer == 6 || countTimer == 4 || countTimer == 2 || countTimer == 0)
                        //{
                        //    e.HighPriority = true;
                        //    e.Appearance.ForeColor = System.Drawing.Color.White;
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lblRoomName_Click(object sender, EventArgs e)
        {

        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatient.frmWaitingScreen_QY1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDoctorName.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.lblUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblRoomName.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.lblRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblWatingCls.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.lblWatingCls.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblWatingExams.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.lblWatingExams.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTTExam.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnSTTExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLastNameExam.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnLastNameExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFirstNameExam.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnFirstNameExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAgeExam.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnAgeExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLastName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnLastName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFirstName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnFirstName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAge.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnAge.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqStt.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnServiceReqStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionTime.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnInstructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqType.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.gridColumnServiceReqType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter searchMVC = new MOS.Filter.HisServiceReqFilter();

                if (room != null)
                {
                    searchMVC.EXECUTE_ROOM_ID = room.ID;
                }

                searchMVC.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                searchMVC.INTRUCTION_DATE_FROM = startDay;
                searchMVC.INTRUCTION_DATE_TO = endDay;
                searchMVC.ORDER_FIELD = "INTRUCTION_DATE";
                searchMVC.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                searchMVC.ORDER_FIELD2 = "PRIORITY";
                searchMVC.ORDER_FIELD3 = "NUM_ORDER";
                searchMVC.HAS_EXECUTE = true;

                searchMVC.ORDER_DIRECTION = "DESC";
                searchMVC.ORDER_DIRECTION1 = "ASC";
                searchMVC.ORDER_DIRECTION2 = "DESC";
                searchMVC.ORDER_DIRECTION3 = "ASC";


                if (serviceReqStts != null && serviceReqStts.Count > 0)
                {
                    serviceReqSttIds = serviceReqStts.Select(o => o.ID).ToList();
                    //searchMVC.SERVICE_REQ_STT_IDs = serviceReqSttIds;
                    if (serviceReqStatics == null)
                    {
                        serviceReqStatics = new List<HIS_SERVICE_REQ>();
                    }
                    serviceReqStatics.Clear();
                    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, searchMVC, param);
                    if (result != null)
                    {
                        serviceReqStatics = result;
                        if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                        {
                            countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                            if (countPatient == 0)
                                countPatient = 10;
                            // danh sách chờ cận lâm sàng đã xong hết (DEPENDENCIES_COUNT>0 có làm cls, BUSY_COUNT =0 cls đã xong hết

                            var serviceReqForCls = serviceReqStatics.Where(o => o.IS_WAIT_CHILD != 1 && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL).Take(countPatient);
                            // danh sách chờ khám
                            serviceReqForClsIds = serviceReqForCls.Select(o => o.ID).ToList();
                            var serviceReqForExams = serviceReqStatics.Where(o => !serviceReqForClsIds.Contains(o.ID) && serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList().Take(countPatient);
                        }
                        else
                        {
                            gridControlWatingExams.BeginUpdate();
                            gridControlWatingExams.DataSource = null;
                            gridControlWatingExams.EndUpdate();
                        }
                    }
                    else
                    {
                        gridControlWatingExams.BeginUpdate();
                        gridControlWatingExams.DataSource = null;
                        gridControlWatingExams.EndUpdate();
                    }
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

        internal void ShowFormInExtendMonitor(frmWaitingScreen1 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    Show();
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.None;
                    Left = sc[1].Bounds.Width;
                    Top = sc[1].Bounds.Height;
                    StartPosition = FormStartPosition.Manual;
                    Location = sc[1].Bounds.Location;
                    Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    Location = p;
                    WindowState = FormWindowState.Maximized;
                    Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataToGridControlWaitingCLSs(List<HIS_SERVICE_REQ> serviceReq1)
        {
            try
            {
                if (serviceReq1 == null || serviceReq1.Count == 0)
                {
                    gridControlWatingExams.DataSource = null;
                    gridControlWaitingCls.DataSource = null;
                    return;
                }

                List<HIS_SERVICE_REQ> serviceReqRight = new List<HIS_SERVICE_REQ>();
                List<HIS_SERVICE_REQ> gridlistleft = new List<HIS_SERVICE_REQ>();

                if (WaitingScreenCFG.PatientNotCLS == "1" && serviceReq1 != null && serviceReq1.Count > 0)
                {
                    serviceReqRight = serviceReq1.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && o.RESULTING_ORDER.HasValue).Take(countPatient).OrderBy(p => p.START_TIME).ToList();
                    List<long> treatmentIdRight = (serviceReqRight != null && serviceReqRight.Count > 0) ? serviceReqRight.Select(o => o.TREATMENT_ID).Distinct().ToList() : new List<long>();
                    gridlistleft = serviceReq1.Where(o => serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID) && !treatmentIdRight.Contains(o.TREATMENT_ID)).ToList().Take(countPatient).ToList();
                }
                else
                {
                    serviceReqRight = serviceReq1.Where(o => o.IS_WAIT_CHILD != 1 && o.HAS_CHILD == 1 && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && o.RESULTING_ORDER.HasValue).Take(countPatient).ToList();
                    gridlistleft = serviceReq1.Where(o => !serviceReqForClsIds.Contains(o.ID) && serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList().Take(countPatient).ToList();
                }
                gridControlWatingExams.BeginUpdate();
                gridControlWatingExams.DataSource = gridlistleft.ToList();
                gridControlWaitingCls.DataSource = serviceReqRight.OrderBy(o => o.RESULTING_ORDER).ToList();
                gridControlWatingExams.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void GetFilePath()
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

        public void SetDataToGridControlCLS()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadSetDataToGridControl));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        internal void executeThreadSetDataToGridControl()
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
        internal void StartTheadSetDataToGridControl()
        {
            SetDataToGridControlWaitingCLSs(serviceReqStatics);
        }

        private void nhapNhay(object data)
        {
            try
            {
                ///tao thread xu ly nhap nhay
                ///
                countTimer = 8;
                this.hisServiceReq = new HIS_SERVICE_REQ();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(this.hisServiceReq, data);
                //timerEffectiveCallingPatient_Tick(null, null);
                timerSetDataToGridControl_Tick(data, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerEffectiveCallingPatient_Tick(object sender, EventArgs e)
        {

            // ChangeEffective();
        }

        private void gridViewWatingExams_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridViewWatingExams_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    int serviceReqStt = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString());
                    if (serviceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && newStatusForceColorCodes != null && newStatusForceColorCodes.Count == 3)
                    {
                        e.Appearance.Font = new System.Drawing.Font("Tahoma", 22);
                        e.HighPriority = true;
                        e.Appearance.ForeColor = System.Drawing.Color.FromArgb(newStatusForceColorCodes[0], newStatusForceColorCodes[1], newStatusForceColorCodes[2]);
                    }
                }
                var result = (HIS_SERVICE_REQ)gridViewWatingExams.GetRow(e.RowHandle);
                if (result != null && this.hisServiceReq != null)
                {

                    if (result.NUM_ORDER == this.hisServiceReq.NUM_ORDER)
                    {
                        countTimer--;
                        if (countTimer == 7 || countTimer == 5 || countTimer == 3 || countTimer == 1)
                        {
                            e.HighPriority = true;
                            e.Appearance.ForeColor = System.Drawing.Color.Red;
                        }
                        //if (countTimer == 6 || countTimer == 4 || countTimer == 2 || countTimer == 0)
                        //{
                        //    e.HighPriority = true;
                        //    e.Appearance.ForeColor = System.Drawing.Color.White;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

        }
    }
}
