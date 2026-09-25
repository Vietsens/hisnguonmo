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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientSample.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientSample
{
    /// <summary>
    /// Man hinh cho lay mau ban _NA: mot luoi danh sach cho + mot label benh nhan dang duoc goi.
    /// Khac ban hien tai o cho bo 3 luoi, cho cau hinh co chu, va lam noi bat dong dang goi.
    /// Du lieu van lay tu api/LisSample/GetView va cache tinh CallPatientDataWorker nhu ban hien tai,
    /// khong dung bang HIS_TREATMENT_SAMPLE_DESK nen khong phai them bang/view nao vao CSDL.
    /// </summary>
    public partial class frmWaitingScreenSample_NA : FormBase
    {
        internal V_LIS_SAMPLE lisSample;
        internal V_HIS_ROOM room;
        private List<long> sampleSttIds;
        private bool isTach = false;

        private string organizationName = "";
        private List<int> newStatusForceColorCodes = new List<int>();
        private List<int> gridpatientBodyForceColorCodes;
        private int rowCount = 0;

        /// <summary>
        /// Thoi diem goi cua tung luot, dong vai tro cot CALL_TIME cua ban goc.
        /// Chi song trong phien lam viec - dung voi cach CallPatientDataWorker dang hoat dong.
        /// </summary>
        private Dictionary<string, long> dicCallTime = new Dictionary<string, long>();

        private List<SampleWaitingNaADO> waitings = new List<SampleWaitingNaADO>();

        public frmWaitingScreenSample_NA(Inventec.Desktop.Common.Modules.Module module, V_LIS_SAMPLE sample, List<long> sttIds, V_HIS_ROOM r, bool isTach = false)
            : base(module)
        {
            InitializeComponent();
            lblCallPatient.Text = "";
            this.lisSample = sample;
            this.room = r;
            this.sampleSttIds = sttIds;
            this.isTach = isTach;
        }

        private void frmWaitingScreenSample_NA_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataToRoom(this.room);
                LoadWaitingPatients();
                SetDataToGridControl();
                StartAllTimer();
                SetFromConfigToControl();
                var emp = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                lblDoctorName.Text = string.Format("{0} {1}",
                    emp != null ? (emp.TITLE != null ? emp.TITLE + ": " : "") : "",
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper());
                rowCount = gridViewWaiting.RowCount - 1;
                SetFormFrontOfAll();
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void StartAllTimer()
        {
            try
            {
                if (WaitingScreenSampleNaCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS > 0)
                {
                    timerSetDataToGridControl.Interval = WaitingScreenSampleNaCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000;
                }
                timerSetDataToGridControl.Enabled = true;
                timerSetDataToGridControl.Start();
                timerCalling.Enabled = true;
                timerCalling.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
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
                LogSystem.Error(ex);
            }
        }

        private void SetFromConfigToControl()
        {
            try
            {
                organizationName = WaitingScreenSampleNaCFG.ORGANIZATION_NAME;

                List<int> roomNameColorCodes = WaitingScreenSampleNaCFG.ROOM_NAME_FORCE_COLOR_CODES;
                if (roomNameColorCodes != null && roomNameColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(roomNameColorCodes[0], roomNameColorCodes[1], roomNameColorCodes[2]);
                    lblRoomName.Appearance.ForeColor = color;
                    lblMoiNguoiBenh.Appearance.ForeColor = color;
                    labelControl1.Appearance.ForeColor = color;
                }
                int roomNameSize = WaitingScreenSampleNaCFG.ROOM_NAME_SIZE;
                if (roomNameSize > 0)
                {
                    lblRoomName.Appearance.Font = new Font("Arial", roomNameSize, FontStyle.Bold);
                }

                List<int> userNameColorCodes = WaitingScreenSampleNaCFG.USER_NAME_FORCE_COLOR_CODES;
                if (userNameColorCodes != null && userNameColorCodes.Count == 3)
                {
                    lblDoctorName.Appearance.ForeColor = Color.FromArgb(userNameColorCodes[0], userNameColorCodes[1], userNameColorCodes[2]);
                }
                int userNameSize = WaitingScreenSampleNaCFG.USER_NAME_SIZE;
                if (userNameSize > 0)
                {
                    lblDoctorName.Appearance.Font = new Font("Arial", userNameSize, FontStyle.Bold);
                }

                List<int> parentBackColorCodes = WaitingScreenSampleNaCFG.PARENT_BACK_COLOR_CODES;
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    layoutControlGroup1.AppearanceGroup.BackColor = color;
                    layoutControlGroup3.AppearanceGroup.BackColor = color;
                    layoutControlGroup5.AppearanceGroup.BackColor = color;
                    Root.AppearanceGroup.BackColor = color;
                    lblMoiNguoiBenh.BackColor = color;
                }

                List<int> gridPatientBackColorCodes = WaitingScreenSampleNaCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                if (gridPatientBackColorCodes != null && gridPatientBackColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(gridPatientBackColorCodes[0], gridPatientBackColorCodes[1], gridPatientBackColorCodes[2]);
                    gridViewWaiting.Appearance.Row.BackColor = color;
                    gridViewWaiting.Appearance.Row.BackColor2 = color;
                    gridViewWaiting.Appearance.Empty.BackColor = color;
                }

                List<int> gridPatientHeaderBackColorCodes = WaitingScreenSampleNaCFG.GRID_PATIENTS_HEADER_BACK_COLOR_CODES;
                if (gridPatientHeaderBackColorCodes != null && gridPatientHeaderBackColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(gridPatientHeaderBackColorCodes[0], gridPatientHeaderBackColorCodes[1], gridPatientHeaderBackColorCodes[2]);
                    gridViewWaiting.Appearance.HeaderPanel.BackColor = color;
                    gridViewWaiting.Appearance.HeaderPanel.BackColor2 = color;
                }

                List<int> gridPatientHeaderForceColorCodes = WaitingScreenSampleNaCFG.GRID_PATIENTS_HEADER_FORCE_COLOR_CODES;
                if (gridPatientHeaderForceColorCodes != null && gridPatientHeaderForceColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(gridPatientHeaderForceColorCodes[0], gridPatientHeaderForceColorCodes[1], gridPatientHeaderForceColorCodes[2]);
                    foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridViewWaiting.Columns)
                    {
                        column.AppearanceHeader.ForeColor = color;
                    }
                }

                gridpatientBodyForceColorCodes = WaitingScreenSampleNaCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES;
                if (gridpatientBodyForceColorCodes != null && gridpatientBodyForceColorCodes.Count == 3)
                {
                    Color color = Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridViewWaiting.Columns)
                    {
                        column.AppearanceCell.ForeColor = color;
                    }
                }

                int patientBodySize = WaitingScreenSampleNaCFG.PATIENT_BODY_SIZE;
                if (patientBodySize > 0)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridViewWaiting.Columns)
                    {
                        column.AppearanceCell.Font = new Font("Arial", patientBodySize, FontStyle.Bold);
                    }
                }

                newStatusForceColorCodes = WaitingScreenSampleNaCFG.NEW_STATUS_REQUEST_FORCE_COLOR_CODES;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap danh sach benh nhan cho lay mau cua phong, va danh dau ai dang duoc goi
        /// theo co CallPatientSTT ma man hinh phong lay mau ghi vao CallPatientDataWorker.
        /// </summary>
        private void LoadWaitingPatients()
        {
            try
            {
                if (this.room == null)
                    return;

                CommonParam param = new CommonParam();
                LisSampleViewFilter filter = new LisSampleViewFilter();
                filter.SAMPLE_ROOM_CODE__EXACT = this.room.ROOM_CODE;
                filter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                filter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                filter.SAMPLE_STT_IDs = this.sampleSttIds;
                filter.ORDER_FIELD = "CALL_SAMPLE_ORDER";
                filter.ORDER_DIRECTION = "ASC";

                var samples = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);
                if (samples == null)
                {
                    samples = new List<V_LIS_SAMPLE>();
                }

                List<ServiceReq1ADO> calleds = null;
                if (CallPatientDataWorker.DicCallPatient != null && CallPatientDataWorker.DicCallPatient.ContainsKey(this.room.ID))
                {
                    calleds = CallPatientDataWorker.DicCallPatient[this.room.ID];
                }

                long now = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                List<SampleWaitingNaADO> result = new List<SampleWaitingNaADO>();
                var groups = samples.GroupBy(g => new { g.PATIENT_CODE, g.CALL_SAMPLE_ORDER }).ToList();
                foreach (var group in groups)
                {
                    var first = group.First();
                    SampleWaitingNaADO ado = new SampleWaitingNaADO();
                    ado.KEY = first.PATIENT_CODE + "|" + first.CALL_SAMPLE_ORDER;
                    ado.TDL_PATIENT_CODE = first.PATIENT_CODE;
                    ado.TDL_PATIENT_NAME = ((first.LAST_NAME ?? "") + " " + (first.FIRST_NAME ?? "")).Trim();
                    ado.FIRST_NAME = first.FIRST_NAME;
                    ado.TDL_PATIENT_DOB = first.DOB ?? 0;
                    ado.TDL_PATIENT_ADDRESS = first.ADDRESS;
                    ado.NUM_ORDER = first.CALL_SAMPLE_ORDER;
                    ado.SERVICE_REQ_STT_ID = first.SAMPLE_STT_ID;
                    ado.SERVICE_REQ_STT_NAME = first.SAMPLE_STT_NAME;
                    ado.INSTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(first.INTRUCTION_TIME ?? 0);
                    ado.IS_CALLING = calleds != null
                        && calleds.Any(o => o.CallPatientSTT && o.TDL_PATIENT_CODE == first.PATIENT_CODE);

                    if (ado.IS_CALLING && !dicCallTime.ContainsKey(ado.KEY))
                    {
                        dicCallTime[ado.KEY] = now;
                    }
                    ado.CALL_TIME = dicCallTime.ContainsKey(ado.KEY) ? dicCallTime[ado.KEY] : 0;

                    result.Add(ado);
                }

                this.waitings = result
                    .OrderByDescending(o => o.CALL_TIME)
                    .ThenBy(o => o.NUM_ORDER ?? 0)
                    .Take(WaitingScreenSampleNaCFG.SO_BENH_NHAN_HIEN_THI)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Do du lieu vao luoi. Dong dang goi duoc lam nhap nhay bang cach cu moi nhip timer
        /// lai doi qua lai giua ban day du va ban da xoa bot truong.
        /// </summary>
        private void SetDataToGridControl()
        {
            try
            {
                if (this.waitings == null || this.waitings.Count <= 0)
                {
                    gridControlWaiting.Invoke(new MethodInvoker(delegate
                    {
                        gridControlWaiting.BeginUpdate();
                        gridControlWaiting.DataSource = null;
                        gridControlWaiting.EndUpdate();
                    }));
                    return;
                }

                List<SampleWaitingNaADO> blinked = new List<SampleWaitingNaADO>();
                foreach (var item in this.waitings)
                {
                    if (item.IS_CALLING)
                    {
                        blinked.Add(new SampleWaitingNaADO
                        {
                            KEY = item.KEY,
                            CALL_TIME = item.CALL_TIME,
                            TDL_PATIENT_NAME = item.TDL_PATIENT_NAME,
                            TDL_PATIENT_DOB = item.TDL_PATIENT_DOB,
                            TDL_PATIENT_ADDRESS = item.TDL_PATIENT_ADDRESS
                        });
                    }
                    else
                    {
                        blinked.Add(item);
                    }
                }

                int second = DateTime.Now.Second;
                int step = timerSetDataToGridControl.Interval / 1000;
                if (step <= 0)
                {
                    step = 1;
                }
                bool showBlinked = ((second - second % step) / step) % 2 == 1;
                List<SampleWaitingNaADO> dataSource = showBlinked ? blinked : this.waitings;

                gridControlWaiting.Invoke(new MethodInvoker(delegate
                {
                    gridControlWaiting.BeginUpdate();
                    gridControlWaiting.DataSource = dataSource;
                    gridControlWaiting.EndUpdate();
                }));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataLabelPatient()
        {
            try
            {
                var calling = this.waitings != null
                    ? this.waitings.Where(o => o.IS_CALLING).OrderByDescending(o => o.CALL_TIME).FirstOrDefault()
                    : null;
                if (calling != null)
                {
                    lblCallPatient.Text = (calling.TDL_PATIENT_NAME ?? "").ToUpper() + " " + GetYearOld(calling.TDL_PATIENT_DOB);
                }
                else
                {
                    lblCallPatient.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
            return yearDob;
        }

        private void gridViewWaiting_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SampleWaitingNaADO data = (SampleWaitingNaADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && e.Column.FieldName == "AGE_DISPLAY")
                    {
                        e.Value = GetYearOld(data.TDL_PATIENT_DOB);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewWaiting_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    bool isCalling = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(e.RowHandle, "IS_CALLING") ?? "").ToString());
                    if (isCalling)
                    {
                        e.Appearance.Font = new Font("Arial", 29, FontStyle.Bold);
                        e.HighPriority = true;
                        e.Appearance.BackColor = Color.Blue;
                        e.Appearance.ForeColor = Color.Yellow;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerSetDataToGridControl_Tick(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadSetDataToGridControl);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ExecuteThreadSetDataToGridControl()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        LoadWaitingPatients();
                        SetDataToGridControl();
                    }));
                }
                else
                {
                    LoadWaitingPatients();
                    SetDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerCalling_Tick(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadCalling);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ExecuteThreadCalling()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { SetDataLabelPatient(); }));
                }
                else
                {
                    SetDataLabelPatient();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Man hinh phong lay mau goi qua reflection khi bam goi so tiep theo.
        /// </summary>
        public void CallNumOrder(int min, int max)
        {
            try
            {
                LoadWaitingPatients();
                SetDataToGridControl();
                SetDataLabelPatient();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmWaitingScreenSample_NA_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerSetDataToGridControl.Enabled = false;
                timerSetDataToGridControl.Stop();
                timerCalling.Enabled = false;
                timerCalling.Stop();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
