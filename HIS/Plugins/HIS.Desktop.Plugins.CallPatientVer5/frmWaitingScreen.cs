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
using HIS.Desktop.ApiConsumer;
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

namespace HIS.Desktop.Plugins.CallPatientVer5
{
    public partial class frmWaitingScreen9 : HIS.Desktop.Utility.FormBase
    {
        internal MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1 hisServiceReq;
        int countTimer = 0;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1> datas = null;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1> serviceReqStatics = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1>();
        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        Inventec.Desktop.Common.Modules.Module _module;
        public frmWaitingScreen9(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1 HisServiceReq, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> ServiceReqStts, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.hisServiceReq = HisServiceReq;
            this.serviceReqStts = ServiceReqStts;
            this._module = module;
        }

        private void frmWaitingScreen_Load(object sender, EventArgs e)
        {
            SetIcon();
            SetCaptionByLanguageKey();
            try
            {
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblUserName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().ToUpper());
                if (room != null)
                {
                    lblRoomName.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblRoomName.Text = "";
                }
                FillDataToGridWaitingPatient(serviceReqStts);
                datas = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1>)gridControlWaitingPatient.DataSource;
                GetFilePath();
                #region khai báo timer
                RegisterTimer(ModuleLink, "timerForScrollListPatient", 5000, timerForScrollListPatientProcess);
                RegisterTimer(ModuleLink, "timerForScrollTextBottom", 500, timerForScrollTextBottomProcess);
                RegisterTimer(ModuleLink, "timerAutoLoadDataPatient", WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000, StartTheadWaitingPatientToCall);
                RegisterTimer(ModuleLink, "timerForHightLightCallPatientLayout", WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000, timerForHightLightCallPatientLayoutProcess);

                StartTimer(ModuleLink, "timerForScrollListPatient");
                StartTimer(ModuleLink, "timerForScrollTextBottom");
                #endregion
                //setDataToLabelPaging();
                lblSrollText.Text = "";
                setFromConfigToControl();
                StartTimer(ModuleLink, "timerAutoLoadDataPatient");
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                this.TopMost = true;
                this.Focus();
                InitRestoreLayoutGridViewFromXml(gridViewWaitingPatient);
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
                    layoutControlGroupColor.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }
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

                // màu nền grid patients
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridViewWaitingPatient.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                // màu nền của header danh sách bệnh nhân
                List<int> gridpatientHeaderBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_HEADER_BACK_COLOR_CODES;
                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    gridColumnAge.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnFirstName.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnInstructionTime.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    gridColumnSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
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
                    gridColumnSTT.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
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
                    gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqStt.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb
(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnServiceReqType.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                // màu chữ của phân trang
                List<int> pagingForceColorCodes = WaitingScreenCFG.PAGING_FORCE_COLOR_CODES;
                if (pagingForceColorCodes != null && pagingForceColorCodes.Count == 3)
                {
                    //lblPageForGridReadyPatientGrid.ForeColor = System.Drawing.Color.FromArgb(pagingForceColorCodes[0], pagingForceColorCodes[1], pagingForceColorCodes[2]);
                }

                // màu chữ của trạng thái yêu cầu là mới
                newStatusForceColorCodes = WaitingScreenCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaitingPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ data = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void timerForHightLightCallPatientLayoutProcess()
        {
            try
            {
                countTimer++;
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
                timerForHightLightCallPatientLayoutProcess();
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
                    StopTimer(ModuleLink, "timerForHightLightCallPatientLayout");
                    StartTimer(ModuleLink, "timerForHightLightCallPatientLayout");
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

        private void timerForScrollTextBottom_Tick(object sender, EventArgs e)
        {
            try
            {
                timerForScrollTextBottomProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {
                timerForScrollListPatientProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ScrollListPatientProcess()
        {
            try
            {
                if (datas != null && datas.Count == gridViewWaitingPatient.TopRowIndex + 10)
                {
                    StopTimer(ModuleLink, "timerForScrollListPatient");
                    LoadWaitingPatientToCallForTimer();
                    //System.Threading.Thread.Sleep(5000);
                    datas = null;
                    datas = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1>)gridControlWaitingPatient.DataSource;
                    gridViewWaitingPatient.TopRowIndex = 0;
                    StartTimer(ModuleLink, "timerForScrollListPatient");
                    //setDataToLabelPaging();
                    return;
                }
                gridViewWaitingPatient.TopRowIndex += 1;
                //setDataToLabelPaging();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadWaitingPatientToCallForTimer()
        {
            LoadWaitingPatientForWaitingScreen();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        internal void StartTheadWaitingPatientToCall()
        {
            FillDataToGridWaitingPatient(serviceReqStts);
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientVer5.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatientVer5.frmWaitingScreen9).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblUserName.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.lblUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblRoomName.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.lblRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLastName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnLastName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFirstName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnFirstName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAge.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnAge.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqStt.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnServiceReqStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionTime.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnInstructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqType.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.gridColumnServiceReqType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridWaitingPatient(frmWaitingScreen9 control, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqView3Filter searchMVC = new MOS.Filter.HisServiceReqView3Filter();
                //HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);
                if (room != null)
                {
                    searchMVC.EXECUTE_ROOM_ID = room.ID;
                }
                List<long> lstServiceReqSTT = new List<long>();
                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                searchMVC.INTRUCTION_TIME_FROM = startDay;
                searchMVC.INTRUCTION_TIME_TO = endDay;
                searchMVC.ORDER_FIELD = "INTRUCTION_DATE";
                searchMVC.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                searchMVC.ORDER_FIELD2 = "PRIORITY";
                searchMVC.ORDER_FIELD3 = "NUM_ORDER";
                searchMVC.ORDER_FIELD4 = "BUSY_COUNT";

                searchMVC.ORDER_DIRECTION = "DESC";
                searchMVC.ORDER_DIRECTION1 = "ASC";
                searchMVC.ORDER_DIRECTION2 = "DESC";
                searchMVC.ORDER_DIRECTION3 = "ASC";
                searchMVC.ORDER_DIRECTION4 = "ASC";
                if (serviceReqStts != null && serviceReqStts.Count > 0)
                {
                    List<long> serviceReqSttIds = serviceReqStts.Select(o => o.ID).ToList();
                    searchMVC.SERVICE_REQ_STT_IDs = serviceReqSttIds;

                    if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                    {
                        serviceReqStatics.Clear();
                    }
                    //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HIS_SERVICE_REQ>>>(searchMVC);
                    var result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO, ApiConsumers.MosConsumer, searchMVC, param);
                    if (result != null)
                    {
                        //serviceReqStatics = result.Data;
                        gridControlWaitingPatient.BeginUpdate();
                        gridControlWaitingPatient.DataSource = null;
                        gridControlWaitingPatient.DataSource = serviceReqStatics;
                        gridControlWaitingPatient.EndUpdate();
                    }
                    else
                    {
                        gridControlWaitingPatient.BeginUpdate();
                        gridControlWaitingPatient.DataSource = null;
                        gridControlWaitingPatient.EndUpdate();
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

        internal void FillDataToGridWaitingPatient(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisServiceReqViewFilter searchMVC = new MOS.Filter.HisServiceReqViewFilter();
                ////HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);

                //if (room != null)
                //{
                //    searchMVC.EXECUTE_ROOM_ID = room.ID;
                //}
                //List<long> lstServiceReqSTT = new List<long>();
                //long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                //long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                //searchMVC.INTRUCTION_DATE_FROM = startDay;
                //searchMVC.INTRUCTION_DATE_TO = endDay;
                //searchMVC.ORDER_FIELD = "INTRUCTION_DATE";
                //searchMVC.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                //searchMVC.ORDER_FIELD2 = "PRIORITY";
                //searchMVC.ORDER_FIELD3 = "NUM_ORDER";
                //searchMVC.ORDER_FIELD4 = "BUSY_COUNT";

                //searchMVC.ORDER_DIRECTION = "DESC";
                //searchMVC.ORDER_DIRECTION1 = "ASC";
                //searchMVC.ORDER_DIRECTION2 = "DESC";
                //searchMVC.ORDER_DIRECTION3 = "ASC";
                //searchMVC.ORDER_DIRECTION4 = "ASC";


                //if (serviceReqStts != null && serviceReqStts.Count > 0)
                //{
                //    List<long> serviceReqSttIds = serviceReqStts.Select(o => o.ID).ToList();
                //    if (serviceReqStatics == null)
                //    {
                //        serviceReqStatics = new List<HIS_SERVICE_REQ>();
                //    }
                //    serviceReqStatics.Clear();
                //    //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HIS_SERVICE_REQ>>>(searchMVC);
                //    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO, ApiConsumers.MosConsumer, searchMVC, param);
                //    if (result != null)
                //    {
                //        serviceReqStatics = result;
                //        if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                //        {
                //            int countPatient = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                //            // danh sách chờ kết quả cận lâm sàng
                //            var serviceReqForCls = serviceReqStatics.Where(o => o.DEPENDENCIES_COUNT > 0 && o.BUSY_COUNT == 0 && o.SERVICE_REQ_STT_ID == HisServiceReqSttCFG.SERVICE_REQ_STT_ID__INPROCESS).Take(countPatient);
                //            gridControlWaitingCls.BeginUpdate();
                //            gridControlWaitingCls.DataSource = serviceReqForCls.ToList();
                //            gridControlWaitingCls.EndUpdate();
                //            // danh sách chờ khám
                //            List<long> serviceReqForClsIds = serviceReqForCls.Select(o => o.ID).ToList();
                //            var serviceReqForExams = serviceReqStatics.Where(o => !serviceReqForClsIds.Contains(o.ID) && serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList().Take(countPatient);
                //            //var serviceReqForExams = serviceReqStatics.Skip(0).Take(4);

                //            gridControlWatingExams.BeginUpdate();
                //            gridControlWatingExams.DataSource = serviceReqForExams.ToList();
                //            gridControlWatingExams.EndUpdate();
                //        }
                //        else
                //        {
                //            gridControlWatingExams.BeginUpdate();
                //            gridControlWatingExams.DataSource = null;
                //            gridControlWatingExams.EndUpdate();
                //        }
                //    }
                //    else
                //    {
                //        gridControlWatingExams.BeginUpdate();
                //        gridControlWatingExams.DataSource = null;
                //        gridControlWatingExams.EndUpdate();
                //    }
                //}

                //#region Process has exception
                //SessionManager.ProcessTokenLost(param);
                //#endregion
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
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = secondScreen.Bounds.Width;
                    control.Top = secondScreen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = secondScreen.Bounds.Location;
                    Point p = new Point(secondScreen.Bounds.Location.X, secondScreen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
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
                //FilePath = Directory.GetFiles(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
