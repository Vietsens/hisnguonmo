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
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientVer5.Class;
using HIS.Desktop.Plugins.CallPatientVer5.Template;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.UC.Login.UCD;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientVer5
{
    public partial class frmWaitingScreen_QY_New : HIS.Desktop.Utility.FormBase
    {
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        int countTimer = 0;
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> datas = null;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        internal string[] FileName, FilePath;
        internal List<HIS_SERVICE_REQ> serviceReqStatics = new List<HIS_SERVICE_REQ>();
        List<long> serviceReqForClsIds = null;
        List<long> serviceReqSttIds = null;
        int countPatient = 0;
        DisplayOptionADO _displayConfig = null;
        internal long roomId = 0;
        Inventec.Desktop.Common.Modules.Module _module;
        UcCallStt ucCallNow { get; set; }
        UcCallStt ucCallNext { get; set; }

        public frmWaitingScreen_QY_New(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts, DisplayOptionADO ado, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this._displayConfig = ado;
            this._module = module;
            //this.roomId = module.RoomId;
        }

        private void InitUc()
        {
            try
            {
                ucCallNext = new UcCallStt(Temp.Next);
                ucCallNext.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(ucCallNext);
                ucCallNext.Reload(null);


                ucCallNow = new UcCallStt(Temp.Now);
                ucCallNow.Dock = DockStyle.Fill;
                panelControl2.Controls.Add(ucCallNow);
                ucCallNow.Reload(null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                InitUc();
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE.ToUpper() + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());

                //lblRoomName.Text = (room != null) ? (room.ROOM_NAME).ToUpper() : "";
                FillDataToGridWaitingPatient(serviceReqStts);
                UpdateDefaultListPatientSTT();
                //
                GetFilePath();
                //Start all timer
                StartAllTimer();
                lblSrollText.Text = "";
                RegisterTimer(ModuleLink, "timerAutoLoadDataPatient", WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000, StartTheadWaitingPatientToCall);
                StartTimer(ModuleLink, "timerAutoLoadDataPatient");
                timerForHightLightCallPatientLayout.Interval = WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000;
                datas = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>)gridControlWaitingCls.DataSource;
                SetIcon();
                InitRestoreLayoutGridViewFromXml(gridViewWaitingCls);
                InitRestoreLayoutGridViewFromXml(gridViewWatingExams);

                //Set color Form
                setFromConfigToControl();
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
                //RegisterTimer(ModuleLink, "timerForScrollTextBottom", 500, ScrollTextBottomsUsingThread);              
                //StartTimer(ModuleLink, "timerForScrollTextBottom");

                timerForScrollTextBottom.Interval = 500;
                timerForScrollTextBottom.Enabled = true;
                timerForScrollTextBottom.Start();

                //timerForScrollListPatient.Start();
                //timerAutoLoadDataPatient.Start();
                //timerForHightLightCallPatientLayout.Start();


                Timer.Interval = 5000;
                Timer.Enabled = true;
                Timer.Start();

                timer1.Interval = 100;
                timer1.Enabled = true;
                timer1.Start();

                //RegisterTimer(ModuleLink, "Timer", 5000, SetDataToGridControlCLS);
                //RegisterTimer(ModuleLink, "timer1", 100, SetDataToLabelMoiBenhNhan);
                //StartTimer(ModuleLink, "Timer");
                //StartTimer(ModuleLink, "timer1");
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
                HightLightCallPatientLayoutProcessAndSetDataForTimer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteHightLightCallPatientLayoutProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { HightLightCallPatientLayoutProcess(); }));
                }
                else
                {
                    HightLightCallPatientLayoutProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void HightLightCallPatientLayoutProcessUsingThread()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ExecuteHightLightCallPatientLayoutProcess));
            thread.Priority = ThreadPriority.Lowest;
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

        void HightLightCallPatientLayoutProcessAndSetDataForTimer()
        {
            HightLightCallPatientLayoutProcessUsingThread();
            SetDataToCurentCallPatientUsingThread();
        }

        private void HightLightCallPatientLayoutProcess()
        {
            try
            {
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
                    //timerForHightLightCallPatientLayout.Stop();
                    StartTimer(ModuleLink, "timerForScrollTextBottom");
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
            try
            {
                ScrollTextBottomsUsingThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ScrollTextBottomsUsingThread()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(timerForScrollTextBottomProcess));
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

        private void timerForScrollTextBottomProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ScrollLabel(); }));
                }
                else
                {
                    ScrollLabel();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ScrollLabel()
        {
            try
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {

        }

        private void timerAutoLoadDataPatient_Tick(object sender, EventArgs e)
        {
            FillDataToDictionaryWaitingPatient(this.serviceReqStts);
        }
        public void LoadWaitingPatientForWaitingScreen()
        {
            try
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
            catch (Exception exx)
            {
                LogSystem.Error(exx);
            }
        }

        internal void executeThreadWaitingPatientToCall()
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

        private void setFromConfigToControl()
        {
            try
            {

                // mau phong xu ly
                //List<int> roomNameColorCodes = WaitingScreenCFG.ROOM_NAME_FORCE_COLOR_CODES;
                //if (roomNameColorCodes != null && roomNameColorCodes.Count == 3)
                //{
                //    lblRoomName.ForeColor = System.Drawing.Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                //}

                //ten benh vien
                organizationName = WaitingScreenCFG.ORGANIZATION_NAME;

                if (string.IsNullOrEmpty(organizationName))
                {
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                lblDoctorName.Appearance.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeTitle, FontStyle.Bold);
                lblPlease.Appearance.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeTitle, FontStyle.Bold);
                if (string.IsNullOrEmpty(_displayConfig.Content))
                {
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem9.Size = new Size(layoutControlItem9.Width, layoutControlItem9.Height + layoutControlItem11.Height);
                }
                lblDoctorName.Appearance.ForeColor = _displayConfig.ColorTittle;
                lblPlease.Appearance.ForeColor = _displayConfig.ColorTittle;
                lblDoctorName.Appearance.BackColor = _displayConfig.ColorBackround;
                lblPlease.Appearance.BackColor = _displayConfig.ColorBackround;
                //this.BackColor = _displayConfig.ColorBackround;
                lblPlease.Text = _displayConfig.Content;
                ucCallNext.SetConfigDisplay(_displayConfig);
                ucCallNow.SetConfigDisplay(_displayConfig);

                // màu chữ của header danh sách bệnh nhân

                gridColumnUT.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnAge.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnFirstName.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnSTT.AppearanceHeader.ForeColor = _displayConfig.ColorList;

                gridColumnUT.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnAge.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnFirstName.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnSTT.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);

                // màu chữ của body danh sách bệnh nhân

                gridColumnUT.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnAge.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnFirstName.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnSTT.AppearanceCell.ForeColor = _displayConfig.ColorList;

                gridColumnUT.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnAge.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnFirstName.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnSTT.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);




                // màu chữ của header danh sách bệnh nhân

                gridColumnUTExam.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnAgeExam.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnFirstNameExam.AppearanceHeader.ForeColor = _displayConfig.ColorList;
                gridColumnSTTExam.AppearanceHeader.ForeColor = _displayConfig.ColorList;

                gridColumnUTExam.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnAgeExam.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnFirstNameExam.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnSTTExam.AppearanceHeader.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);

                // màu chữ của body danh sách bệnh nhân

                gridColumnUTExam.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnAgeExam.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnFirstNameExam.AppearanceCell.ForeColor = _displayConfig.ColorList;
                gridColumnSTTExam.AppearanceCell.ForeColor = _displayConfig.ColorList;

                gridColumnUTExam.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnAgeExam.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnFirstNameExam.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);
                gridColumnSTTExam.AppearanceCell.Font = new Font(new FontFamily("Arial"), (float)_displayConfig.SizeList, FontStyle.Bold);


                // chiều cao dòng nội dung, tiêu đề ds bn
                gridViewWaitingCls.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWaitingCls.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

                gridViewWatingExams.RowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_NOI_DUNG_DANH_SACH_BENH_NHAN;
                gridViewWatingExams.ColumnPanelRowHeight = (int)WaitingScreenCFG.CHIEU_CAO_DONG_TIEU_DE_DANH_SACH_BENH_NHAN;

                gridViewWaitingCls.OptionsView.ShowColumnHeaders = _displayConfig.IsShowCol;
                gridViewWatingExams.OptionsView.ShowColumnHeaders = _displayConfig.IsShowCol;

                lblWatingCls.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__DANH_SACH_CHO, FontStyle.Bold);
                lblWatingExams.Font = new System.Drawing.Font(new FontFamily("Arial"), WaitingScreenCFG.FONT_SIZE__DANH_SACH_CHO, FontStyle.Bold);

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
                            int namsinh;
                            var birthday = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TDL_PATIENT_DOB);

                            if (birthday != null)
                            {
                                namsinh = Convert.ToDateTime(birthday).Year;
                                e.Value = namsinh.ToString();
                            }
                        }
                        if (e.Column.FieldName == "UT_STR")
                        {
                            long uutien = data.PRIORITY ?? 0;
                            if (uutien > 0)
                            {
                                var priority = BackendDataWorker.Get<HIS_PRIORITY_TYPE>().Where(o => o.ID == uutien).ToList();
                                if (priority != null)
                                    e.Value = "ƯT";
                            }

                        }
                        if (e.Column.FieldName == "PATIENT_FULL_NAME")
                        {
                            e.Value = data.TDL_PATIENT_LAST_NAME + " " + data.TDL_PATIENT_FIRST_NAME;
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
                            int namsinh;
                            var birthday = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TDL_PATIENT_DOB);

                            if (birthday != null)
                            {
                                namsinh = Convert.ToDateTime(birthday).Year;
                                e.Value = namsinh.ToString();
                            }
                        }
                        if (e.Column.FieldName == "UT_STR")
                        {
                            long uutien = data.PRIORITY ?? 0;
                            if (uutien > 0)
                            {
                                var priority = BackendDataWorker.Get<HIS_PRIORITY_TYPE>().Where(o => o.ID == uutien).ToList();
                                if (priority != null)
                                    e.Value = "ƯT";
                            }

                        }
                        if (e.Column.FieldName == "PATIENT_FULL_NAME")
                        {
                            e.Value = data.TDL_PATIENT_LAST_NAME + " " + data.TDL_PATIENT_FIRST_NAME;
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
                    int uutien = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "PRIORITY") ?? "").ToString());
                    if (uutien > 0)
                    {
                        e.HighPriority = true;
                        e.Appearance.ForeColor = _displayConfig.ColorPriority;
                    }
                }
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
                else
                {
                    if (this._module.RoomId != null)
                    {
                        searchMVC.EXECUTE_ROOM_ID = _module.RoomId;
                    }
                }

                if (this._displayConfig.IsNotInDebt)
                {
                    searchMVC.IS_NOT_IN_DEBT = true;
                }

                searchMVC.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                long startDay = Inventec.Common.DateTime.Get.StartDay().Value;
                searchMVC.INTRUCTION_DATE__EQUAL = startDay;
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


                    //Test 20/12/2019
                    //if (serviceReqStatics == null)
                    //{
                    //    serviceReqStatics = new List<HIS_SERVICE_REQ>();
                    //}
                    serviceReqStatics.Clear();
                    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, searchMVC, param);
                    if (result != null)
                    {
                        serviceReqStatics = result;
                        //20/12/2019
                        //CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertListServiceReq1ToADO(result);
                        //
                        if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                        {
                            countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                            if (countPatient == 0)
                                countPatient = 10;
                            // danh sách chờ cận lâm sàng đã xong hết (DEPENDENCIES_COUNT>0 có làm cls, BUSY_COUNT =0 cls đã xong hết
                            var serviceReqForCls = serviceReqStatics.Where(o => o.IS_WAIT_CHILD != 1 && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL).Take(countPatient);
                            //var serviceReqForCls1 = serviceReqStatics.Where(o => o.IS_WAIT_CHILD !=1 && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && o.HAS_CHILD == 1 ).Take(countPatient);
                            // danh sách chờ khám
                            serviceReqForClsIds = serviceReqForCls.Select(o => o.ID).ToList();
                            //serviceReqForClsIds1 = serviceReqForCls1.Select(o => o.ID).ToList();
                            var serviceReqForExams = serviceReqStatics.Where(o => !serviceReqForClsIds.Contains(o.ID) && serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList().Take(countPatient);
                            if (lstPatientCalled != null && lstPatientCalled.Count > 0)
                                serviceReqStatics = serviceReqStatics.Where(o => lstPatientCalled.Exists(p => p.ID == o.ID)).ToList();
                            //gridControlWatingExams.BeginUpdate();
                            //gridControlWatingExams.DataSource = serviceReqForExams;
                            //gridControlWatingExams.EndUpdate();
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

        internal void ShowFormInExtendMonitor(frmWaitingScreen9 control)
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
                foreach (var item in lstPatientCalled)
                {
                    if (!serviceReq1.Exists(o => o.ID == item.ID))
                        serviceReq1.Add(item);
                }
                if (ucCallNext.GetServiceReqAdo() != null && !serviceReq1.Exists(o => o.ID == ucCallNext.GetServiceReqAdo().ID))
                    serviceReq1.Add(ucCallNext.GetServiceReqAdo());

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
                    serviceReqRight = serviceReq1.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL).Take(countPatient).OrderBy(p => p.START_TIME).ToList();
                    List<long> treatmentIdRight = (serviceReqRight != null && serviceReqRight.Count > 0) ? serviceReqRight.Select(o => o.TREATMENT_ID).Distinct().ToList() : new List<long>();
                    foreach (var item in lstPatientCalled)
                    {
                        if (serviceReq1.Exists(o => o.ID == item.ID))
                            serviceReq1.Remove(item);
                    }
                    if (ucCallNow.GetServiceReqAdo() != null)
                        serviceReq1 = serviceReq1.Where(o => o.ID != ucCallNow.GetServiceReqAdo().ID).ToList();
                    if (ucCallNext.GetServiceReqAdo() != null)
                        serviceReq1 = serviceReq1.Where(o => o.ID != ucCallNext.GetServiceReqAdo().ID).ToList();
                    gridlistleft = serviceReq1.Where(o => serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID) && !treatmentIdRight.Contains(o.TREATMENT_ID)).ToList();
                }
                else
                {
                    serviceReqRight = serviceReq1.Where(o => o.IS_WAIT_CHILD != 1 && o.HAS_CHILD == 1 && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL).ToList();
                    foreach (var item in lstPatientCalled)
                    {
                        if (serviceReq1.Exists(o => o.ID == item.ID))
                            serviceReq1.Remove(item);
                    }
                    if (ucCallNow.GetServiceReqAdo() != null)
                        serviceReq1 = serviceReq1.Where(o => o.ID != ucCallNow.GetServiceReqAdo().ID).ToList();
                    if (ucCallNext.GetServiceReqAdo() != null)
                        serviceReq1 = serviceReq1.Where(o => o.ID != ucCallNext.GetServiceReqAdo().ID).ToList();
                    gridlistleft = serviceReq1.Where(o => serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList();
                }
                gridControlWatingExams.BeginUpdate();
                gridControlWatingExams.DataSource = gridlistleft;
                gridControlWaitingCls.DataSource = serviceReqRight;
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
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadSetDataToLabelMoiBenhNhan));
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
        HIS_SERVICE_REQ ServiceReqNext = null;
        List<HIS_SERVICE_REQ> lstPatientCalled = new List<HIS_SERVICE_REQ>();
        private void SetDataToLabelMoiBenhNhanChild()
        {
            try
            {
                ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                ServiceReq1ADO PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0 && CallPatientDataWorker.DicCallPatient.ContainsKey(room.ID)) ? CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.CallPatientSTT) : null;
                serviceReq1ADO = PatientIsCall;
                if (serviceReq1ADO != null)
                {
                    if (lstPatientCalled.Exists(o => o.ID == serviceReq1ADO.ID) && (ucCallNow.GetServiceReqAdo() == null || ucCallNow.GetServiceReqAdo().ID != serviceReq1ADO.ID))
                        serviceReqStatics.Add(serviceReq1ADO);
                    var rowCall = serviceReqStatics.FirstOrDefault(o => o.ID == serviceReq1ADO.ID);
                    if (this.serviceReqStts != null && this.serviceReqStts.Count > 0 && rowCall != null)
                    {
                        serviceReqStatics.Remove(rowCall);
                        SetDataToCurrentPatientCall(serviceReq1ADO);
                        if (!lstPatientCalled.Exists(o => o.ID == rowCall.ID))
                            lstPatientCalled.Add(rowCall);
                        if (serviceReqStatics.Count > 0)
                        {
                            if ((ServiceReqNext != null && lstPatientCalled.Exists(o => o.ID == ServiceReqNext.ID)) || (ServiceReqNext == null && !lstPatientCalled.Exists(o => o.ID == serviceReqStatics.FirstOrDefault().ID)))
                            {
                                ServiceReqNext = serviceReqStatics.FirstOrDefault(o => this.serviceReqStts.Select(s => s.ID).Contains(o.SERVICE_REQ_STT_ID));
                                SetDataToCurrentPatientNextCall(ServiceReqNext);
                            }
                        }
                        else
                        {
                            SetDataToCurrentPatientNextCall(null);
                            ServiceReqNext = null;
                        }
                    }
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
                ucCallNow.Reload(serviceReq1ADO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetDataToCurrentPatientNextCall(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                LocalStorage.BackendData.ADO.ServiceReq1ADO serviceReq1Ado = new LocalStorage.BackendData.ADO.ServiceReq1ADO();
                AutoMapper.Mapper.CreateMap<HIS_SERVICE_REQ, HIS.Desktop.LocalStorage.BackendData.ADO.ServiceReq1ADO>();
                serviceReq1Ado = AutoMapper.Mapper.Map<LocalStorage.BackendData.ADO.ServiceReq1ADO>(serviceReq);
                ucCallNext.Reload(serviceReq1Ado);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        void SetDataToCurentCallPatientUsingThread()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadSetDataToCurentCallPatient));
            thread.Priority = ThreadPriority.Lowest;
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

        void StartTheadSetDataToCurentCallPatient()
        {
            SetDataToCurentCallPatientUsingThread();
        }
        private void SetDataToCurrentCallPatient()
        {
            try
            {
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.Count > 0 && CallPatientDataWorker.DicCallPatient[room.ID] != null && CallPatientDataWorker.DicCallPatient[room.ID].Count > 0)
                {

                    //ServiceReq1ADO PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].Where(o => this.serviceReqStts.Select(p => p.ID).Contains(o.SERVICE_REQ_STT_ID)).FirstOrDefault(o => o.CallPatientSTT) : null;
                    ServiceReq1ADO PatientIsCall = (this.serviceReqStts != null && this.serviceReqStts.Count > 0) ? CallPatientDataWorker.DicCallPatient[room.ID].FirstOrDefault(o => o.CallPatientSTT) : null;
                    ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                    if (PatientIsCall != null)
                    {
                        serviceReq1ADO = PatientIsCall;
                    }
                    else
                    {
                        //ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();
                        //SetDataToCurrentPatientCall(currentServiceReq1ADO);
                    }
                }
                else
                {
                    //ServiceReq1ADOWorker.ServiceReq1ADO = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        void StartTheadWaitingPatientToCall()
        {
            FillDataToDictionaryWaitingPatient(serviceReqStts);
        }
        public async void FillDataToDictionaryWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
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
                if (this._displayConfig.IsNotInDebt)
                {
                    hisServiceReqFilter.IS_NOT_IN_DEBT = true;
                }

                //if (serviceReqStts != null && serviceReqStts.Count > 0)
                //{
                //    List<long> lstServiceReqSTTFilter = serviceReqStts.Select(o => o.ID).ToList();
                //    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTTFilter;
                //}
                var result = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("result 1___________________", result));
                if (result != null && result.Count > 0)
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("result 2___________________", result));
                    CallPatientDataWorker.DicCallPatient[room.ID] = ConnvertListServiceReq1ToADO(result);
                    //gridControlWatingExams.DataSource = serviceReqStatics;
                    serviceReqStatics = result;
                    if (lstPatientCalled != null && lstPatientCalled.Count > 0)
                        serviceReqStatics = serviceReqStatics.Where(o => !lstPatientCalled.Exists(p => p.ID == o.ID)).ToList();
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
                // CallPatientDataWorker.DicCallPatient[room.ID] = serviceReq1Ados;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return serviceReq1Ados;
        }

        private void frmWaitingScreen_QY_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerAutoLoadDataPatient.Enabled = false;
                timerForHightLightCallPatientLayout.Enabled = false;
                timerForScrollListPatient.Enabled = false;
                timerForScrollTextBottom.Enabled = false;
                Timer.Enabled = false;
                timer1.Enabled = false;

                timerAutoLoadDataPatient.Stop();
                timerForHightLightCallPatientLayout.Stop();
                timerForScrollListPatient.Stop();
                timerForScrollTextBottom.Stop();
                Timer.Stop();
                timer1.Stop();

                timerAutoLoadDataPatient.Dispose();
                timerForHightLightCallPatientLayout.Dispose();
                timerForScrollListPatient.Dispose();
                timerForScrollTextBottom.Dispose();
                Timer.Dispose();
                timer1.Dispose();


                //StopTimer(ModuleLink, "timerForScrollListPatient");
                //StopTimer(ModuleLink, "timerAutoLoadDataPatient");
                //StopTimer(ModuleLink, "timerForHightLightCallPatientLayout");
                //StopTimer(ModuleLink, "timerForScrollTextBottom");
                //StopTimer(ModuleLink, "Timer");
                //StopTimer(ModuleLink, "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewWatingExams_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    int uutien = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "PRIORITY") ?? "").ToString());
                    if (uutien > 0)
                    {
                        e.HighPriority = true;
                        e.Appearance.ForeColor = _displayConfig.ColorPriority;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cmsConfig_Click(object sender, EventArgs e)
        {
            try
            {
                frmChooseRoomForWaitingScreen frmChooseRoom = new frmChooseRoomForWaitingScreen(_module, true);
                frmChooseRoom.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
