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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using System.Threading;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.CallPatient
{
    public partial class frmCallPatient : HIS.Desktop.Utility.FormBase
    {
        internal MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1 hisServiceReqWithOrderSDO;
        int countTimer = 0;
        int countTimerGridPatient = 0;
        int startIndex = 0;
        bool playNext;
        int countScoll = 0;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1> datas = null;
        public frmCallPatient(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1 _hisServiceReqWithOrderSDO)
        {
            InitializeComponent();
            this.hisServiceReqWithOrderSDO = _hisServiceReqWithOrderSDO;
        }
        private void frmCallPatient_Load(object sender, EventArgs e)
        {
            SetIcon();
            try
            {
                lblUserName.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper();
                SetDefaultLabel();
                CallPatientProcess.FillDataToGridTop5Patient(this);
                CallPatientProcess.FillDataToGridWaitingPatient(this);
                datas = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1>)gridControlWaitingPatient.DataSource;
                CallPatientProcess.GetFilePath();
                CallPatientProcess.playfile(this, startIndex);
                gridViewTop5Patient.Focus();
                timerForScrollListPatient.Start();
                setDataToLabelPaging();
                timerForPlayNext.Start();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultLabel()
        {
            lblRoomName.Text = WorkPlace.GetRoomNames().ToUpper();
            lblPatientName.Text = "";
            lblNumOrder.Text = "MỜI BỆNH NHÂN";
            WindowsMediaPlayerQC.uiMode = "none";

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
        private void gridViewTop5Patient_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle > 0)
                {
                    int serviceReqStt = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                    long priority = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "PRIORITY") ?? "").ToString());
                    bool? DependenciesFinished = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "DependenciesFinished") ?? "").ToString());
                    bool? DiagnosticGroupFinished = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "DiagnosticGroupFinished") ?? "").ToString());
                    if (priority == 1)//TODO
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.FromArgb(237, 28, 36);// mau do
                    }
                    else
                    {
                        //if (serviceReqStt == EXE.LOGIC.Config.HisServiceReqSttCFG.SERVICE_REQ_STT_ID__INPROCESS)
                        if (serviceReqStt ==IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL )
                        {
                            if (DependenciesFinished == true && DiagnosticGroupFinished == true)
                            {
                                e.Appearance.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                e.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 162, 232); // mau xanh duong
                            }
                        }
                        if (serviceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Appearance.ForeColor = System.Drawing.Color.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void WindowsMediaPlayerQC_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            int statuschk = e.newState;  // here the Status return the windows Media Player status where the 8 is the Song or Vedio is completed the playing .

            // Now here i check if the song is completed then i Increment to play the next song

            if (statuschk == 8)
            {
                statuschk = e.newState;

                if (startIndex >= CallPatientProcess.FilePath.Length - 1)
                {
                    startIndex = 0;
                }
                else if (startIndex >= 0 && startIndex < CallPatientProcess.FilePath.Length - 1)
                {
                    startIndex = startIndex + 1;
                }
                playNext = true;
            }
        }
        private void setDataToLabelPaging()
        {
            lblPageForGridReadyPatientGrid.Text = "Từ " + (gridViewWaitingPatient.TopRowIndex + 1) + " đến " + (gridViewWaitingPatient.TopRowIndex + 5) + " trên tổng số " + datas.Count;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatient.frmCallPatient).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblPageForGridReadyPatientGrid.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblPageForGridReadyPatientGrid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTop5PatientToCall.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblTop5PatientToCall.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmCallPatient.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblUserName.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNumOrder.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblPatientName.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblRoomName.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.lblRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCallPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaitingPatient_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle > 0)
                {
                    int serviceReqStt = Inventec.Common.TypeConvert.Parse.ToInt16((View.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                    long priority = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "PRIORITY") ?? "").ToString());
                    bool? DependenciesFinished = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "DependenciesFinished") ?? "").ToString());
                    bool? DiagnosticGroupFinished = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "DiagnosticGroupFinished") ?? "").ToString());
                    if (priority == 1)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.FromArgb(237, 28, 36);// mau do
                    }
                    else
                    {
                        if (serviceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            if (DependenciesFinished == true && DiagnosticGroupFinished == true)
                            {
                                e.Appearance.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                e.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 162, 232); // mau xanh duong
                            }
                        }
                        if (serviceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Appearance.ForeColor = System.Drawing.Color.White;
                        }
                    }
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
                countTimer++;
                //lblPatientName.BeginUpdate();
                if (countTimer == 1 || countTimer == 3 || countTimer == 5 || countTimer == 7 || countTimer == 9)
                {
                    lblPatientName.ForeColor = System.Drawing.Color.FromArgb(40, 255, 40);
                }
                if (countTimer == 10 || countTimer == 2 || countTimer == 4 || countTimer == 6 || countTimer == 8)
                {
                    lblPatientName.ForeColor = Color.White;
                }

                if (countTimer > 10)
                {
                    timerForHightLightCallPatientLayout.Stop();
                    lblPatientName.ForeColor = System.Drawing.Color.FromArgb(40, 255, 40);
                    countTimer = 0;
                }
                //layoutPatientCallNow.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForPlayNext_Tick(object sender, EventArgs e)
        {
            if (playNext == true)
            {
                playNext = false;
                CallPatientProcess.playfile(this, startIndex);
            }
        }
        internal void executeThreadTop5ServiceReq()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { StartThreadLoadTop5ServiceReq(); }));
                }
                else
                {
                    StartThreadLoadTop5ServiceReq();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void StartTheadWaitingPatientToCall()
        {
            CallPatientProcess.FillDataToGridWaitingPatient(this);
        }
        internal void StartThreadLoadTop5ServiceReq()
        {
            CallPatientProcess.FillDataToGridTop5Patient(this);
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
        public void LoadWaitingPatientForWaitingScreen()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadWaitingPatientToCall));
            thread.Priority = ThreadPriority.Highest;
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
        internal void LoadWaitingPatientToCallForTimer()
        {
            LoadWaitingPatientForWaitingScreen();
        }
        private void timerForScrollListPatient_Tick(object sender, EventArgs e)
        {
            try
            {

                if (datas.Count == gridViewWaitingPatient.TopRowIndex + 5)
                {
                    timerForScrollListPatient.Stop();
                    LoadWaitingPatientToCallForTimer();
                    System.Threading.Thread.Sleep(5000);
                    datas = null;
                    datas = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1>)gridControlWaitingPatient.DataSource;
                    gridViewWaitingPatient.TopRowIndex = 0;
                    timerForScrollListPatient.Start();
                    setDataToLabelPaging();
                    return;
                }
                gridViewWaitingPatient.TopRowIndex += 1;
                setDataToLabelPaging();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dataNavigatorListPatient_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        {
            try
            {
                switch (e.Button.ButtonType)
                {
                    case NavigatorButtonType.Append:
                        break;
                    case NavigatorButtonType.CancelEdit:
                        //btnPatientNew_Click(null, null);
                        break;
                    case NavigatorButtonType.Custom:
                        break;
                    case NavigatorButtonType.Edit:
                        break;
                    case NavigatorButtonType.EndEdit:
                        break;
                    case NavigatorButtonType.First:
                        break;
                    case NavigatorButtonType.Last:
                        break;
                    case NavigatorButtonType.Next:
                        break;
                    case NavigatorButtonType.NextPage:
                        break;
                    case NavigatorButtonType.Prev:
                        break;
                    case NavigatorButtonType.PrevPage:
                        break;
                    case NavigatorButtonType.Remove:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                listView1.Items[listView1.Items.Count - 1].EnsureVisible();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
