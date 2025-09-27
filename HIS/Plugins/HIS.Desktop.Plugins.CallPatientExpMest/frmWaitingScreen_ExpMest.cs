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
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientExpMest.ADO;
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

namespace HIS.Desktop.Plugins.CallPatientExpMest
{
    public partial class frmWaitingScreen_ExpMest : FormBase
    {
        internal List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> hisExpMest;

        internal MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 currentCall;

        public List<V_HIS_EXP_MEST_2> extraExpMest2 = new List<V_HIS_EXP_MEST_2>();
        List<HIS_EXP_MEST> extraMest = new List<HIS_EXP_MEST>();

        const int STEP_NUMBER_ROW_GRID_SCROLL = 5;
        internal MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK room;
        private int scrll { get; set; }
        string organizationName = "";
        List<int> newStatusForceColorCodes = new List<int>();
        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT> serviceReqStts;
        internal static string[] FilePath;
        List<int> gridpatientBodyForceColorCodes;
        int index = 0;
        int rowCount = 0;
        long serviceReqIdOld = 0;
        int countTimer = 0;
        List<long> serviceReqForClsIds = null;
        List<long> serviceReqSttIds = null;
        List<ExpMestCallADO> tempListServiceReq;
        Inventec.Desktop.Common.Modules.Module module;
        ExpMestCallADO expCall = new ExpMestCallADO();

        public frmWaitingScreen_ExpMest(ExpMestCallADO ado, V_HIS_MEDI_STOCK medistock)
        {
            InitializeComponent();
            expCall = ado;
            room = medistock;
        }

        private void frmWaitingScreen_QY_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                FillDataToDictionaryWaitingPatient(expCall);
                UpdateDefaultListPatientSTT();
                SetDataToGridControlWaitingCLSs();
                GetFilePath();
                StartAllTimer();


                //RegisterTimer(ModuleLink, "timerChangeColorRow", 1000, SetDataForChangeColor);
                //StartTimer(ModuleLink, "timerChangeColorRow");
                timerChangeColorRow.Enabled = true;
                timerChangeColorRow.Interval = 1000;
                timerChangeColorRow.Start();

                SetFromConfigToControl();
                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0}{1}", employee != null && !string.IsNullOrEmpty(employee.TITLE) ? employee.TITLE + ": " : "", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaitingCls.RowCount - 1;
                SetFormFrontOfAll();

                //RegisterTimer(ModuleLink, "timer1", 1000, SetDataToLabelMoiBenhNhan);
                //StartTimer(ModuleLink, "timer1");

                timer1.Enabled = true;
                timer1.Interval = 500;
                timer1.Start();

                CallPatientDataWorker.DicDelegateCallingPatient[this.room.ROOM_ID] = (DelegateSelectData)this.nhapNhay;
                if(extraExpMest2.Count > 0)
                {

                }
                SetIcon();
                InitRestoreLayoutGridViewFromXml(gridViewWaitingCls);

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
                timerForScrollListPatient.Enabled = true;
                timerForScrollListPatient.Interval = 2000;
                timerForScrollListPatient.Start();

                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Interval = (WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000);
                timerSetDataToGridControl.Start();

                timerAutoLoadDataPatient.Enabled = true;
                timerAutoLoadDataPatient.Interval = (WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000);
                timerAutoLoadDataPatient.Start();

                timerForHightLightCallPatientLayout.Enabled = true;
                timerForHightLightCallPatientLayout.Interval = (WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000);
                timerForHightLightCallPatientLayout.Start();

                //RegisterTimer(ModuleLink, "timerForScrollListPatient", 2000, ScrollListPatientForWaitingScreenThread);
                //RegisterTimer(ModuleLink, "timerSetDataToGridControl", WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000, SetDataToGridControlCLS);
                //RegisterTimer(ModuleLink, "timerAutoLoadDataPatient", WaitingScreenCFG.TIMER_FOR_SET_DATA_TO_GRID_PATIENTS * 1000, LoadWaitingPatientForWaitingScreen);
                //RegisterTimer(ModuleLink, "timerForHightLightCallPatientLayout", WaitingScreenCFG.TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT * 1000, SetDataToCurentCallPatientUsingThread);

                //StartTimer(ModuleLink, "timerForScrollListPatient");
                //StartTimer(ModuleLink, "timerSetDataToGridControl");
                //StartTimer(ModuleLink, "timerAutoLoadDataPatient");
                //StartTimer(ModuleLink, "timerForHightLightCallPatientLayout");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToRoom(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK room)
        {
            try
            {
                if (room != null)
                {
                    lblRoomName.Text = (room.MEDI_STOCK_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
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
                LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
            FillDataToDictionaryWaitingPatient(expCall);
        }

        private void SetFromConfigToControl()
        {
            try
            {
                this.lblRoomName.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);
                this.lblRoomName.ForeColor = ColorTranslator.FromHtml(expCall.colorText);
                this.lblRoomName.BackColor = ColorTranslator.FromHtml(expCall.bgColorText);

                this.lblDoctorName.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);
                this.lblDoctorName.ForeColor = ColorTranslator.FromHtml(expCall.colorText);
                this.lblDoctorName.BackColor = ColorTranslator.FromHtml(expCall.bgColorText);

                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceHeader.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextTitle);
                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceHeader.ForeColor = ColorTranslator.FromHtml(expCall.colorTextTitle);
                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceHeader.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceHeader.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceHeader.ForeColor = ColorTranslator.FromHtml(expCall.colorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceHeader.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceHeader.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceHeader.ForeColor = ColorTranslator.FromHtml(expCall.colorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceHeader.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceHeader.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceHeader.ForeColor = ColorTranslator.FromHtml(expCall.colorTextTitle);
                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceHeader.Font = new Font("Times New Roman", expCall.sizeText, FontStyle.Bold);

                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceCell.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextList);
                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceCell.ForeColor = ColorTranslator.FromHtml(expCall.colorTextList);
                gridViewWaitingCls.Columns["NUM_ORDER_STR"].AppearanceCell.Font = new Font("Times New Roman", expCall.sizeTextList, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceCell.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceCell.ForeColor = ColorTranslator.FromHtml(expCall.colorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_NAME"].AppearanceCell.Font = new Font("Times New Roman", expCall.sizeTextList, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceCell.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceCell.ForeColor = ColorTranslator.FromHtml(expCall.colorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_DOB_STR"].AppearanceCell.Font = new Font("Times New Roman", expCall.sizeTextList, FontStyle.Bold);

                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceCell.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceCell.ForeColor = ColorTranslator.FromHtml(expCall.colorTextList);
                gridViewWaitingCls.Columns["TDL_PATIENT_ADDRESS"].AppearanceCell.Font = new Font("Times New Roman", expCall.sizeTextList, FontStyle.Bold);

                gridViewWaitingCls.Appearance.Empty.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextList);
                gridViewWaitingCls.Appearance.Empty.BackColor2 = ColorTranslator.FromHtml(expCall.bgColorTextList);

                lblMoiNguoiBenh.Text = expCall.ContentTitle.ToUpper();
                lblCoSttNhoHon.Text = expCall.NoteTitle.ToUpper();

                lblMoiNguoiBenh.Font = new Font("Times New Roman", expCall.sizeTextContent, FontStyle.Bold);
                lblMoiNguoiBenh.ForeColor = ColorTranslator.FromHtml(expCall.colorTextContent);
                lblMoiNguoiBenh.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextContent);

                lblCoSttNhoHon.Font = new Font("Times New Roman", expCall.sizeTextContent, FontStyle.Bold);
                lblCoSttNhoHon.ForeColor = ColorTranslator.FromHtml(expCall.colorTextContent);
                lblCoSttNhoHon.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextContent);

                lblSo.Font = new Font("Times New Roman", expCall.sizeTextContent, FontStyle.Bold);
                lblSo.ForeColor = ColorTranslator.FromHtml(expCall.colorTextContent);
                lblSo.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextContent);

                lblPatientName.Font = new Font("Times New Roman", expCall.sizeTextCalling, FontStyle.Bold);
                lblPatientName.ForeColor = ColorTranslator.FromHtml(expCall.colorTextCalling);
                lblPatientName.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextCalling);

                lblSoThuTuBenhNhan.Font = new Font("Times New Roman", expCall.sizeTextCalling, FontStyle.Bold);
                lblSoThuTuBenhNhan.ForeColor = ColorTranslator.FromHtml(expCall.colorTextCalling);
                lblSoThuTuBenhNhan.BackColor = ColorTranslator.FromHtml(expCall.bgColorTextCalling);

            }
            catch (Exception ex)
            {
            }
        }

        private void gridViewWaitingCls_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                HIS_EXP_MEST pData = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "NUM_ORDER_STR")
                    {
                        e.Value = pData.PRES_NUMBER ?? (pData.NUM_ORDER ?? null);
                    }
                    if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        e.Value = GetYearOld((long)pData.TDL_PATIENT_DOB);
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
              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        void FillDataToDictionaryWaitingPatient(ExpMestCallADO serviceReqStts)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter hisExpMestFilter = new HisExpMestFilter();
                if(serviceReqStts.expMestSttADOs.Count > 0)
                    hisExpMestFilter.EXP_MEST_TYPE_IDs = serviceReqStts.expMestTypeADOs.Select(o => o.ID).ToList();

                if (serviceReqStts.expMestTypeADOs.Count > 0)
                    hisExpMestFilter.EXP_MEST_STT_IDs = serviceReqStts.expMestSttADOs.Select(o => o.ID).ToList();
                if (room != null)
                {
                    hisExpMestFilter.MEDI_STOCK_ID = room.ID;
                }

                List<long> lstServiceReqSTT = new List<long>();
                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());//20181212121527
                long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                hisExpMestFilter.CREATE_TIME_FROM = startDay;
                hisExpMestFilter.CREATE_TIME_TO = endDay;

                hisExpMestFilter.ORDER_FIELD = "CREATE_TIME";

                hisExpMestFilter.ORDER_DIRECTION = "ASC";


                var result = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, hisExpMestFilter, param);
                hisExpMest = result;
                extraMest = new List<HIS_EXP_MEST>();
                foreach (var item in hisExpMest)
                {
                    bool exists = extraExpMest2.Any(x => x.ID == item.ID);
                    if (!exists)
                    {
                        extraMest.Add(item);
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

        private void SetDataToCurrentPatientCall(V_HIS_EXP_MEST_2 serviceReq1ADO)
        {
            try
            {
                if (serviceReq1ADO != null)
                {
                    lblPatientName.Text = serviceReq1ADO.TDL_PATIENT_NAME;
                    lblSoThuTuBenhNhan.Text = serviceReq1ADO.PRES_NUMBER + "" ?? (serviceReq1ADO.NUM_ORDER + "" ?? "");

                    if (countTimer > 0)
                    {
                        countTimer--;
                        if (countTimer == 7 || countTimer == 5 || countTimer == 3 || countTimer == 1)
                        {
                            lblPatientName.Appearance.ForeColor = System.Drawing.Color.Red;

                            lblSoThuTuBenhNhan.ForeColor = System.Drawing.Color.Red;
                        }
                        if (countTimer == 6 || countTimer == 4 || countTimer == 2)
                        {
                            lblPatientName.Appearance.ForeColor = System.Drawing.Color.White;

                            lblSoThuTuBenhNhan.ForeColor = System.Drawing.Color.White;
                        }
                    }
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
                if (currentCall != null && currentCall.ID > 0)
                {
                    SetDataToCurrentPatientCall(currentCall);
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

        private void SetDataToGridControlWaitingCLSs()
        {
            try
            {
                gridControlWaitingCls.BeginUpdate();
                gridControlWaitingCls.DataSource = extraMest;
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
                ServiceReq1ADOWorker.ServiceReq1ADO = new ServiceReq1ADO();

                timerAutoLoadDataPatient.Enabled = false;
                timerAutoLoadDataPatient.Stop();

                timerForHightLightCallPatientLayout.Enabled = false;
                timerForHightLightCallPatientLayout.Stop();

                timerForScrollListPatient.Enabled = false;
                timerForScrollListPatient.Stop();

                timerSetDataToGridControl.Enabled = false;
                timerSetDataToGridControl.Stop();

                timerChangeColorRow.Enabled = false;
                timerChangeColorRow.Stop();


                timer1.Enabled = false;
                timer1.Stop();

                timerAutoLoadDataPatient.Dispose();
                timerForHightLightCallPatientLayout.Dispose();
                timerForScrollListPatient.Dispose();
                timerSetDataToGridControl.Dispose();
                timerChangeColorRow.Dispose();
                timer1.Dispose();

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
                this.currentCall = new V_HIS_EXP_MEST_2();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_2>(this.currentCall, (V_HIS_EXP_MEST_2)data);
                extraExpMest2.Add(this.currentCall);
                extraMest = new List<HIS_EXP_MEST>();
                foreach (var item in hisExpMest)
                {
                    bool exists = extraExpMest2.Any(x => x.ID == item.ID);
                    if (!exists && !extraMest.Any(x =>x.ID == item.ID))
                    {
                        extraMest.Add(item);
                    }
                    else
                    {
                        extraMest.RemoveAll(x => x.ID == item.ID);
                    }

                }
                SetDataToGridControlWaitingCLSs();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaitingCls_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
        }

        private void frmWaitingScreen_ExpMest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void frmWaitingScreen_ExpMest_KeyDown(object sender, KeyEventArgs e)
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
