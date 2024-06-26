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

namespace HIS.Desktop.Plugins.CallPatientCLS
{
    public partial class frmWaitingScreen_QY2 : FormBase
    {
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq;
        int countTimer = 0;
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        static string[] FileName, FilePath;
        static List<HIS_SERVICE_REQ> serviceReqStatics = new List<HIS_SERVICE_REQ>();
        List<HIS_SERVICE_REQ> serviceReqForCls;
        bool isNotInDebt = false;
        Dictionary<int, HIS_SERVICE_REQ> lstPatientAutoRefactor { get; set; }
        public frmWaitingScreen_QY2(Inventec.Desktop.Common.Modules.Module module,
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts, bool _IsNotInDebt)
            : base(module)
        {
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this.isNotInDebt = _IsNotInDebt;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                var emp = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblUserName.Text = string.Format("{0} {1}", emp != null ? (emp.TITLE != null ? emp.TITLE + ": " : "") : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                if (room != null)
                {
                    lblRoomName.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblRoomName.Text = "";
                }
                FillDataToGridWaitingPatient(serviceReqStts);
                GetFilePath();
                #region khai báo timer
                //HIS.Desktop.Utility.MemoryProcessor.RegisterTimer(moduleLink, "timerForScrollListPatient", 5000, timerForHightLightCallPatientLayoutProcess);

                #endregion
                timerAutoRefactorName.Start();
                StartAllTimer();



                lblSrollText.Text = "";
                setFromConfigToControl();
                SetIcon();
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

        private void StartAllTimer()
        {
            try
            {
                timerForScrollTextBottom.Interval = 500;
                timerForScrollTextBottom.Enabled = true;
                timerForScrollTextBottom.Start();

                timerSetDataToGridControl.Interval = 10000;
                timerSetDataToGridControl.Enabled = true;
                timerForHightLightCallPatientLayout.Start();

                timerAutoLoadDataPatient.Interval = (WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000);
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
        private void HightLightCallPatientLayoutProcess()
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
                    //StopTimer(ModuleLink, "timerForHightLightCallPatientLayout");
                    //timerForHightLightCallPatientLayout.Stop();
                    //RegisterTimer(ModuleLink, "timerForScrollTextBottom", 500, ScrollLabel);
                    //StartTimer(ModuleLink, "timerForScrollTextBottom");
                    timerForScrollTextBottom.Interval = 500;
                    timerForScrollTextBottom.Enabled = true;
                    timerForScrollTextBottom.Start();
                    // màu chữ tên tổ chức
                    List<int> organizationColorCodes = WaitingScreenCFG.ORGANIZATION_FORCE_COLOR_CODES;
                    if (organizationColorCodes != null && organizationColorCodes.Count == 3)
                    {
                        lblSrollText.ForeColor = System.Drawing.Color.FromArgb(organizationColorCodes[0], organizationColorCodes[1], organizationColorCodes[2]);
                    }
                    countTimer = 0;
                }
                //layoutPatientCallNow.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void timerForHightLightCallPatientLayoutProcess()
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

        private void timerForHightLightCallPatientLayout_Tick(object sender, EventArgs e)
        {
            try
            {
                Task ts = Task.Factory.StartNew(timerForHightLightCallPatientLayoutProcess);
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
                Task ts = Task.Factory.StartNew(timerForScrollLabel);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerForScrollLabel()
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
            try
            {
                Task ts = Task.Factory.StartNew(ExecuteThreadWaitingPatientToCall);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        void LoadWaitingPatientForWaitingScreen()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ExecuteThreadWaitingPatientToCall));
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
                    layoutControlGroup3.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup4.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup5.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
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
                    lblUserName.ForeColor = System.Drawing.Color.FromArgb(userNameColorCodes[0], userNameColorCodes[1], userNameColorCodes[2]);
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
                }

                // màu chữ của trạng thái yêu cầu là mới
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
                    var data = (HIS_SERVICE_REQ)gridViewWaitingCls.GetRow(e.RowHandle);

                    string patientName = data.TDL_PATIENT_FIRST_NAME;
                    decimal fontSize = Convert.ToDecimal(gridColumnLastName.AppearanceCell.Font.Size);
                    if ((Convert.ToDecimal(patientName.Length) * (decimal)0.7 * fontSize) > gridColumnLastName.Width)
                    {
                        //if ()
                        lstPatientAutoRefactor.Add(e.RowHandle, data);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataToGridWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter hisServiceReqFilter = new HisServiceReqFilter();
                //HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);

                if (room != null)
                {
                    hisServiceReqFilter.EXECUTE_ROOM_ID = room.ID;
                }
                hisServiceReqFilter.HAS_EXECUTE = true;

                hisServiceReqFilter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                List<long> lstServiceReqSTT = new List<long>();
                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
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
                if (isNotInDebt)
                {
                    hisServiceReqFilter.IS_NOT_IN_DEBT = true;
                }

                if (serviceReqStts != null && serviceReqStts.Count > 0)
                {
                    List<long> lstServiceReqSTTFilter = serviceReqStts.Select(o => o.ID).ToList();
                    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTTFilter;
                    if (serviceReqStatics == null)
                    {
                        serviceReqStatics = new List<HIS_SERVICE_REQ>();
                    }
                    serviceReqStatics.Clear();
                    //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HisServiceReqViewSDO>>>(searchMVC);
                    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                    if (result != null)
                    {
                        serviceReqStatics = result;
                        if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                        {
                            int countPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                            // danh sách chờ kết quả cận lâm sàng
                            serviceReqForCls = serviceReqStatics.Take(countPatient).ToList();
                        }
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

        private void SetDataToGridControlWaitingCLSs(List<HIS_SERVICE_REQ> serviceReq1)
        {
            try
            {
                lstPatientAutoRefactor = new Dictionary<int, HIS_SERVICE_REQ>();
                gridControlWaitingCls.BeginUpdate();
                gridControlWaitingCls.DataSource = serviceReq1.ToList();
                gridControlWaitingCls.EndUpdate();
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
                Task ts = Task.Factory.StartNew(StartTheadSetDataToGridControl);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ExecuteThreadSetDataToGridControl()
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

        private void timerAutoRefactorName_Tick(object sender, EventArgs e)
        {
            try
            {
                Task ts = Task.Factory.StartNew(timerForAutoRefactorName);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForAutoRefactorName()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { AutoRefactorName(); }));
                }
                else
                {
                    AutoRefactorName();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void AutoRefactorName()
        {
            try
            {
                if (lstPatientAutoRefactor != null && lstPatientAutoRefactor.Count() > 0)
                {
                    foreach (var item in lstPatientAutoRefactor)
                    {
                        item.Value.TDL_PATIENT_FIRST_NAME =
                            item.Value.TDL_PATIENT_FIRST_NAME = item.Value.TDL_PATIENT_FIRST_NAME.Insert(item.Value.TDL_PATIENT_FIRST_NAME.Length, item.Value.TDL_PATIENT_FIRST_NAME.Substring(0, 1));
                        item.Value.TDL_PATIENT_FIRST_NAME = item.Value.TDL_PATIENT_FIRST_NAME.Substring(1, item.Value.TDL_PATIENT_FIRST_NAME.Length - 1);
                        gridViewWaitingCls.SetRowCellValue(item.Key, "TDL_PATIENT_FIRST_NAME", item.Value.TDL_PATIENT_FIRST_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmWaitingScreen_QY2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerForScrollTextBottom.Enabled = false;
                timerForScrollTextBottom.Stop();
                timerForScrollTextBottom.Dispose();

                timerSetDataToGridControl.Enabled = false;
                timerSetDataToGridControl.Stop();
                timerSetDataToGridControl.Dispose();

                timerAutoLoadDataPatient.Enabled = false;
                timerAutoLoadDataPatient.Stop();
                timerAutoLoadDataPatient.Dispose();

                timerForHightLightCallPatientLayout.Enabled = false;
                timerForHightLightCallPatientLayout.Stop();
                timerForHightLightCallPatientLayout.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void StartTheadSetDataToGridControl()
        {
            SetDataToGridControlWaitingCLSs(serviceReqForCls);
        }

    }
}
