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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportCountTreatment
{
    public partial class FormReportCountTreatment : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private long TimeFrom;
        private long TimeTo;
        //private List<HIS_SERVICE_REQ> ListServiceReq;
        //private List<ADO.TreatmentADO> ListTreatement;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private const string moduleLink = "HIS.Desktop.Plugins.ReportCountTreatment";

        private List<ADO.DepartmentADO> ListDepartment;
        //private Get.GetTreatmentInfo TreatmentInfo;
        private bool IsKham, IsNoiTru, IsNgoaiTru, IsBanNgay;
        public Dictionary<string, List<HIS_BED>> dicBedGroupString = new Dictionary<string, List<HIS_BED>>();
        public FormReportCountTreatment()
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormReportCountTreatment(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                this.InitRestoreLayoutGridViewFromXml(gridViewDetail);
                this.InitRestoreLayoutGridViewFromXml(gridViewExamTotal);
                this.InitRestoreLayoutGridViewFromXml(gridViewTotal);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormReportCountTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControlsProperties();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                InitComboBranch();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //radioGroupDienDieuTri.SelectedIndex = 1;    //Nội trú

                ThreadFillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlsProperties()
        {
            try
            {
                panelControl2.BackColor = this.BackColor;
                radioGroupDichVu.BackColor = this.BackColor;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBranch()
        {
            try
            {
                CboBranch.Properties.DataSource = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>();
                CboBranch.Properties.DisplayMember = "BRANCH_NAME";
                CboBranch.Properties.ValueMember = "ID";
                CboBranch.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                CboBranch.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                CboBranch.Properties.ImmediatePopup = true;
                CboBranch.ForceInitialize();
                CboBranch.Properties.View.Columns.Clear();
                CboBranch.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = CboBranch.Properties.View.Columns.AddField("BRANCH_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = CboBranch.Properties.View.Columns.AddField("BRANCH_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadFillDataToGrid()
        {
            System.Threading.Thread fill = new System.Threading.Thread(LoadDataByNewThread);
            try
            {
                fill.Start();
            }
            catch (Exception ex)
            {
                fill.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { FillDataToGrid(); }));
                }
                else
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                BtnView.Enabled = true;
                CboDepartment.EditValue = null;
                CboFilter.EditValue = null;
                EndableGridControl();
                if (dtTimeFrom.EditValue != null && dtTimeTo.EditValue != null)
                {
                    //TimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(dateEdit.DateTime.AddDays(-1).ToString("yyyyMMddHHmm") + "00");
                    //TimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dateEdit.DateTime.ToString("yyyyMMddHHmm") + "00");
                    //TimeTo -= 1;//trừ 1 giây;
                    TimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    TimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMddHHmm") + "00");

                    if (CboBranch.EditValue == null)
                    {
                        MessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonChiNhanh);
                        return;
                    }
                    var dichVuSelected = radioGroupDichVu.EditValue != null ? Convert.ToInt32(radioGroupDichVu.EditValue) : -1;

                    if (dichVuSelected == (int)Enum.DichVu.Kham
                        || dichVuSelected == (int)Enum.DichVu.CanLamSang)
                    {
                        ProcessDataServiceReqToGrid(TimeFrom, TimeTo);
                    }
                    else
                    {
                        ProcessDataTretmentToGrid(TimeFrom, TimeTo);
                    }
                }
                else
                {
                    SetDefaultDataGrid();
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataServiceReqToGrid(long timeFrom, long timeTo)
        {
            try
            {
                List<long> serviceReqTypeids = new List<long>();
                var dichVuSelected = radioGroupDichVu.EditValue != null ? Convert.ToInt32(radioGroupDichVu.EditValue) : -1;

                if (dichVuSelected == (int)Enum.DichVu.CanLamSang)
                {
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
                }
                else if (dichVuSelected == (int)Enum.DichVu.Kham)
                {
                    serviceReqTypeids.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                }

                List<long> roomIds = new List<long>();
                var branchId = long.Parse((CboBranch.EditValue ?? "0").ToString());
                var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == branchId && o.IS_ACTIVE == 1).ToList();
                List<V_HIS_ROOM> listRoom = new List<V_HIS_ROOM>();
                if (department != null && department.Count > 0)
                {
                    listRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => department.Select(s => s.ID).Contains(o.DEPARTMENT_ID) && o.IS_ACTIVE == 1).ToList();
                    if (listRoom != null && listRoom.Count > 0)
                    {
                        roomIds = listRoom.Select(s => s.ID).ToList();
                    }
                }

                InitComboType(0);

                this.ListDepartment = new List<ADO.DepartmentADO>();

                if (dichVuSelected == (int)Enum.DichVu.Kham)
                {
                    var ListServiceReq = new Get.GetServiceReq(timeFrom, timeTo, serviceReqTypeids, roomIds).Get();
                    if (ListServiceReq != null && ListServiceReq.Count > 0)
                    {
                        var listTreatment = new Get.GetTreatment(timeFrom, timeTo, null).GetTreatmentIn(0, ListServiceReq.Select(s => s.TREATMENT_ID).ToList());
                        if (listTreatment != null && listTreatment.Count > 0)
                        {
                            var listClnTreatment = listTreatment.Where(o => o.CLINICAL_IN_TIME.HasValue && o.CLINICAL_IN_TIME >= timeFrom && o.CLINICAL_IN_TIME <= timeTo).ToList();
                            //listTreatment = listTreatment.Where(o => (!o.CLINICAL_IN_TIME.HasValue || o.CLINICAL_IN_TIME < timeFrom || o.CLINICAL_IN_TIME > timeTo) && (o.IS_PAUSE != 1 || !o.OUT_TIME.HasValue || o.OUT_TIME > timeFrom)).ToList();

                            listTreatment = listTreatment.Where(o => (!o.IS_PAUSE.HasValue || o.IS_PAUSE != 1) || (!o.OUT_TIME.HasValue || o.OUT_TIME > timeFrom)).ToList();
                            if (listTreatment != null && listTreatment.Count > 0)
                            {
                                ListServiceReq = ListServiceReq.Where(o => listTreatment.Select(s => s.ID).Contains(o.TREATMENT_ID)).ToList();
                                if (ListServiceReq != null && ListServiceReq.Count > 0)
                                {
                                    var dicTreatmentInfo = new Get.GetTreatmentInfo(listTreatment.Select(o => o.ID).ToList()).Get();
                                    var listTreatmentAdo = ProcessDataADO(listTreatment, dicTreatmentInfo);
                                    var dicTreatmentClnInfo = new Get.GetTreatmentInfo(listClnTreatment.Select(o => o.ID).ToList()).Get();
                                    var listTreatmentClnAdo = ProcessDataADO(listClnTreatment, dicTreatmentClnInfo);

                                    var listRoomIds = ListServiceReq.Select(s => s.EXECUTE_ROOM_ID).Distinct().ToList();
                                    listRoom = listRoom.Where(o => listRoomIds.Contains(o.ID)).ToList();
                                    InitComboDepartment(listRoom);
                                    ProcessDataGridDetail(ListServiceReq, listTreatmentAdo, timeFrom, timeTo);
                                    ProcessDataGridTotalKham(ListServiceReq, listTreatmentAdo, listTreatmentClnAdo, timeFrom, timeTo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var ListServiceReq = new Get.GetServiceReq(timeFrom, timeTo, serviceReqTypeids, roomIds).Get();
                    if (ListServiceReq != null && ListServiceReq.Count > 0)
                    {
                        var listTreatment = new Get.GetTreatment(timeFrom, timeTo, null).GetTreatmentIn(0, ListServiceReq.Select(s => s.TREATMENT_ID).ToList());
                        if (listTreatment != null && listTreatment.Count > 0)
                        {
                            // lấy các bệnh nhân chưa kết thúc điều trị hoặc có thời gian kết thúc lớn hơn thời gian đến
                            listTreatment = listTreatment.Where(o => (!o.IS_PAUSE.HasValue || o.IS_PAUSE != 1) || (!o.OUT_TIME.HasValue || o.OUT_TIME > timeFrom)).ToList();

                            if (listTreatment != null && listTreatment.Count > 0)
                            {
                                ListServiceReq = ListServiceReq.Where(o => listTreatment.Select(s => s.ID).Contains(o.TREATMENT_ID)).ToList();
                                if (ListServiceReq != null && ListServiceReq.Count > 0)
                                {
                                    var dicTreatmentInfo = new Get.GetTreatmentInfo(listTreatment.Select(o => o.ID).ToList()).Get();
                                    var listTreatmentAdo = ProcessDataADO(listTreatment, dicTreatmentInfo);

                                    var listRoomIds = ListServiceReq.Select(s => s.EXECUTE_ROOM_ID).Distinct().ToList();
                                    listRoom = listRoom.Where(o => listRoomIds.Contains(o.ID)).ToList();
                                    InitComboDepartment(listRoom);
                                    ProcessDataGridDetail(ListServiceReq, listTreatmentAdo, timeFrom, timeTo);
                                    ProcessDataGridTotalCls(ListServiceReq, listTreatmentAdo, timeFrom, timeTo);
                                }
                            }
                        }
                    }
                }

                ProcessDataControl();

                if (this.ListDepartment == null || this.ListDepartment.Count <= 0)
                {
                    SetDefaultDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboType(int p)
        {
            try
            {
                List<ADO.FilterADO> listData = new List<ADO.FilterADO>();

                if (p == 0)//cls
                {
                    ADO.FilterADO COUNT_OLD = new ADO.FilterADO();
                    COUNT_OLD.ID = 1;
                    COUNT_OLD.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_OLD");
                    listData.Add(COUNT_OLD);

                    ADO.FilterADO COUNT_IN = new ADO.FilterADO();
                    COUNT_IN.ID = 2;
                    COUNT_IN.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_NEW");
                    listData.Add(COUNT_IN);

                    ADO.FilterADO COUNT_OUT = new ADO.FilterADO();
                    COUNT_OUT.ID = 3;
                    COUNT_OUT.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_DONE");
                    listData.Add(COUNT_OUT);

                    ADO.FilterADO COUNT_TRAN = new ADO.FilterADO();
                    COUNT_TRAN.ID = 4;
                    COUNT_TRAN.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_IN");
                    listData.Add(COUNT_TRAN);

                    ADO.FilterADO COUNT_END_DEPARTMENT = new ADO.FilterADO();
                    COUNT_END_DEPARTMENT.ID = 5;
                    COUNT_END_DEPARTMENT.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_OUT__TOOL_TIP");
                    listData.Add(COUNT_END_DEPARTMENT);

                    ADO.FilterADO COUNT_CURR = new ADO.FilterADO();
                    COUNT_CURR.ID = 6;
                    COUNT_CURR.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TOTAL_REQUEST");
                    listData.Add(COUNT_CURR);

                    ADO.FilterADO COUNT_END = new ADO.FilterADO();
                    COUNT_END.ID = 7;
                    COUNT_END.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TOTAL_EXECUTE");
                    listData.Add(COUNT_END);
                }
                else if (p == 1)//treat
                {
                    ADO.FilterADO COUNT_OLD = new ADO.FilterADO();
                    COUNT_OLD.ID = 1;
                    COUNT_OLD.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_CU__TOOL_TIP");
                    listData.Add(COUNT_OLD);

                    ADO.FilterADO COUNT_IN = new ADO.FilterADO();
                    COUNT_IN.ID = 2;
                    COUNT_IN.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_VAO__TOOL_TIP");
                    listData.Add(COUNT_IN);

                    ADO.FilterADO COUNT_OUT = new ADO.FilterADO();
                    COUNT_OUT.ID = 3;
                    COUNT_OUT.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_RA__TOOL_TIP");
                    listData.Add(COUNT_OUT);

                    ADO.FilterADO COUNT_DEATH = new ADO.FilterADO();
                    COUNT_DEATH.ID = 4;
                    COUNT_DEATH.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_DEATH");
                    listData.Add(COUNT_DEATH);

                    ADO.FilterADO COUNT_TRAN = new ADO.FilterADO();
                    COUNT_TRAN.ID = 5;
                    COUNT_TRAN.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TRAN_PATI");
                    listData.Add(COUNT_TRAN);

                    ADO.FilterADO COUNT_6T = new ADO.FilterADO();
                    COUNT_6T.ID = 6;
                    COUNT_6T.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_6T");
                    listData.Add(COUNT_6T);

                    ADO.FilterADO COUNT_80T = new ADO.FilterADO();
                    COUNT_80T.ID = 7;
                    COUNT_80T.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_80T");
                    listData.Add(COUNT_80T);

                    ADO.FilterADO COUNT_CC = new ADO.FilterADO();
                    COUNT_CC.ID = 8;
                    COUNT_CC.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CC");
                    listData.Add(COUNT_CC);

                    ADO.FilterADO COUNT_H = new ADO.FilterADO();
                    COUNT_H.ID = 9;
                    COUNT_H.TYPE = "Giường H";
                    listData.Add(COUNT_H);

                    ADO.FilterADO COUNT_T = new ADO.FilterADO();
                    COUNT_T.ID = 10;
                    COUNT_T.TYPE = "Giường T";
                    listData.Add(COUNT_T);

                    ADO.FilterADO COUNT_FEMALE = new ADO.FilterADO();
                    COUNT_FEMALE.ID = 11;
                    COUNT_FEMALE.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_FEMALE");
                    listData.Add(COUNT_FEMALE);

                    ADO.FilterADO COUNT_END_DEPARTMENT = new ADO.FilterADO();
                    COUNT_END_DEPARTMENT.ID = 12;
                    COUNT_END_DEPARTMENT.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_END_DEPA__TOOL_TIP");
                    listData.Add(COUNT_END_DEPARTMENT);

                    ADO.FilterADO COUNT_CURR = new ADO.FilterADO();
                    COUNT_CURR.ID = 13;
                    COUNT_CURR.TYPE = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_HIEN_CO");
                    listData.Add(COUNT_CURR);
                }


                CboFilter.Properties.DataSource = listData;
                CboFilter.Properties.DisplayMember = "TYPE";
                CboFilter.Properties.ValueMember = "ID";
                CboFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                CboFilter.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                CboFilter.Properties.ImmediatePopup = true;
                CboFilter.ForceInitialize();
                CboFilter.Properties.View.Columns.Clear();
                CboFilter.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = CboFilter.Properties.View.Columns.AddField("TYPE");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboDepartment(object listData)
        {
            try
            {
                string displayMember = "";
                string valueMember = "";
                string code = "";
                string name = "";

                if (listData is List<V_HIS_ROOM>)
                {
                    displayMember = "ROOM_NAME";
                    valueMember = "ID";
                    code = "ROOM_CODE";
                    name = "ROOM_NAME";
                }
                else if (listData is List<HIS_DEPARTMENT>)
                {
                    displayMember = "DEPARTMENT_NAME";
                    valueMember = "ID";
                    code = "DEPARTMENT_CODE";
                    name = "DEPARTMENT_NAME";
                }

                CboDepartment.Properties.DataSource = listData;
                CboDepartment.Properties.DisplayMember = displayMember;
                CboDepartment.Properties.ValueMember = valueMember;
                CboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                CboDepartment.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                CboDepartment.Properties.ImmediatePopup = true;
                CboDepartment.ForceInitialize();
                CboDepartment.Properties.View.Columns.Clear();
                CboDepartment.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = CboDepartment.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = CboDepartment.Properties.View.Columns.AddField(name);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region kham, cls
        private void ProcessDataControl()
        {
            try
            {
                var dichVuSelected = radioGroupDichVu.EditValue != null ? Convert.ToInt32(radioGroupDichVu.EditValue) : -1;

                long old = 0, intreat = 0, outtreat = 0, currtreat = 0;

                if (ListDepartment != null && ListDepartment.Count > 0)
                {
                    //lấy các yêu cầu khám chưa kết thúc hoặc kết thúc > timeto
                    old = ListDepartment.Sum(o => o.COUNT_OLD);

                    //số yêu cầu khám tạo trong khoảng timeFrom đến timeTo
                    intreat = ListDepartment.Sum(o => o.COUNT_IN);

                    //số yêu cầu khám kết thúc trong khoảng timeFrom đến timeTo
                    if (dichVuSelected == (int)Enum.DichVu.Kham
                        || dichVuSelected == (int)Enum.DichVu.CanLamSang)
                        outtreat = ListDepartment.Sum(o => o.COUNT_OUT);
                    else
                        outtreat = ListDepartment.Sum(o => o.COUNT_END_DEPARTMENT);

                    //số yêu cầu khám chưa kết thúc hoặc kết thúc sau thời điểm timeTo
                    currtreat = ListDepartment.Sum(o => o.COUNT_CURR);
                }

                this.TxtOld.Text = old.ToString();
                this.TxtIn.Text = intreat.ToString();
                this.TxtOut.Text = outtreat.ToString();
                this.TxtCurrent.Text = currtreat.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridTotalCls(List<HIS_SERVICE_REQ> listServiceReq, List<ADO.TreatmentADO> listTreatment, long timeFrom, long timeTo)
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    ListDepartment = new List<ADO.DepartmentADO>();

                    var roomGroup = listServiceReq.GroupBy(o => o.EXECUTE_ROOM_ID).ToList();
                    foreach (var groups in roomGroup)
                    {
                        var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == groups.First().EXECUTE_ROOM_ID);
                        if (room != null)
                        {
                            ADO.DepartmentADO ado = new ADO.DepartmentADO();
                            ado.DEPARTMENT_NAME = room.ROOM_NAME;
                            ado.DEPARTMENT_ID = room.ID;

                            var treatmentIds = groups.Select(s => s.TREATMENT_ID).Distinct().ToList();

                            var reqIn = groups.Where(o => o.INTRUCTION_TIME >= timeFrom && o.INTRUCTION_TIME <= timeTo).ToList();
                            if (reqIn != null && reqIn.Count > 0)
                            {
                                ado.COUNT_IN = reqIn.Count();
                                ado.TreatmentIn = listTreatment.Where(o => reqIn.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                            }

                            var old = groups.Where(o => o.INTRUCTION_TIME < timeFrom || (o.FINISH_TIME.HasValue && o.FINISH_TIME > timeTo)).ToList();
                            if (old != null && old.Count > 0)
                            {
                                ado.COUNT_OLD = old.Count;
                                ado.TreatmentOld = listTreatment.Where(o => old.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                            }

                            var reqout = groups.Where(o => o.FINISH_TIME.HasValue && o.FINISH_TIME >= timeFrom && o.FINISH_TIME <= timeTo).ToList();
                            if (reqout != null && reqout.Count > 0)
                            {
                                ado.COUNT_OUT = reqout.Count();
                                ado.TreatmentOut = listTreatment.Where(o => reqout.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                            }

                            var tran = listTreatment.Where(o => o.CLINICAL_IN_TIME.HasValue && o.CLINICAL_IN_TIME >= timeFrom && o.CLINICAL_IN_TIME <= timeTo && treatmentIds.Contains(o.ID)).ToList();
                            if (tran != null && tran.Count > 0)
                            {
                                ado.COUNT_TRAN = tran.Count;
                                ado.TreatmentTran = tran;
                            }

                            var endDepa = listTreatment.Where(o => o.IS_PAUSE == 1 && o.OUT_TIME >= timeFrom && o.OUT_TIME <= timeTo && treatmentIds.Contains(o.ID)).ToList();
                            if (endDepa != null && endDepa.Count > 0)
                            {
                                ado.COUNT_END_DEPARTMENT = endDepa.Count;
                                ado.TreatmentEndDepartment = endDepa;
                            }

                            var curr = listTreatment.Where(o => (o.IS_PAUSE != 1 || o.OUT_TIME >= timeTo) && treatmentIds.Contains(o.ID)).ToList();
                            if (curr != null && curr.Count > 0)
                            {
                                ado.COUNT_CURR = curr.Count;
                                ado.TreatmentCurr = curr;
                            }

                            ListDepartment.Add(ado);
                        }
                    }

                    if (ListDepartment != null && ListDepartment.Count > 0)
                    {
                        ListDepartment = ListDepartment.OrderByDescending(o => o.COUNT_IN).ThenByDescending(o => o.COUNT_OUT).ThenByDescending(o => o.COUNT_OLD).ThenByDescending(o => o.COUNT_CURR).ToList();
                    }

                    gridControlExamTotal.BeginUpdate();
                    gridControlExamTotal.DataSource = null;
                    gridControlExamTotal.DataSource = ListDepartment;
                    gridControlExamTotal.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridTotalKham(List<HIS_SERVICE_REQ> listServiceReq, List<ADO.TreatmentADO> listTreatment, List<ADO.TreatmentADO> listClnTreatment, long timeFrom, long timeTo)
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    ListDepartment = new List<ADO.DepartmentADO>();

                    var roomGroup = listServiceReq.GroupBy(o => o.EXECUTE_ROOM_ID).ToList();
                    foreach (var groups in roomGroup)
                    {
                        var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == groups.First().EXECUTE_ROOM_ID);
                        if (room != null)
                        {
                            ADO.DepartmentADO ado = new ADO.DepartmentADO();
                            ado.DEPARTMENT_NAME = room.ROOM_NAME;
                            ado.DEPARTMENT_ID = room.ID;

                            var treatmentIds = groups.Select(s => s.TREATMENT_ID).Distinct().ToList();

                            //chỉ định cũ
                            var treatOld = listTreatment.Where(o => (o.IS_PAUSE != 1 || o.OUT_TIME >= timeFrom) && treatmentIds.Contains(o.ID)).ToList();
                            if (treatOld != null && treatOld.Count > 0)
                            {
                                ado.COUNT_OLD = treatOld.Select(s => s.PATIENT_ID).Distinct().Count();
                                ado.TreatmentOld = treatOld;
                            }

                            //chỉ định mới
                            var treatIn = listTreatment.Where(o => o.IN_TIME >= timeFrom && o.IN_TIME <= timeTo && treatmentIds.Contains(o.ID)).ToList();
                            if (treatIn != null && treatIn.Count > 0)
                            {
                                ado.COUNT_IN = treatIn.Select(s => s.PATIENT_ID).Distinct().Count();
                                ado.TreatmentIn = treatIn;
                            }

                            //hoàn thành
                            var treatOut = listTreatment.Where(o => o.OUT_TIME >= timeFrom && o.OUT_TIME <= timeTo && treatmentIds.Contains(o.ID)).ToList();
                            if (treatOut != null && treatOut.Count > 0)
                            {
                                ado.COUNT_OUT = treatOut.Select(s => s.PATIENT_ID).Distinct().Count();
                                ado.TreatmentOut = treatOut;
                            }

                            //nhập viện
                            var tran = listClnTreatment.Where(o => o.CLINICAL_IN_TIME.HasValue && o.CLINICAL_IN_TIME >= timeFrom && o.CLINICAL_IN_TIME <= timeTo && o.IN_ROOM_ID == room.ID).ToList();
                            if (tran != null && tran.Count > 0)
                            {
                                ado.COUNT_TRAN = tran.Count;
                                ado.TreatmentTran = tran;
                            }

                            //kết thúc điều trị
                            var endDepa = listTreatment.Where(o => o.IS_PAUSE == 1 && o.OUT_TIME.HasValue && o.OUT_TIME >= timeFrom && o.OUT_TIME <= timeTo && treatmentIds.Contains(o.ID)).ToList();
                            if (endDepa != null && endDepa.Count > 0)
                            {
                                ado.COUNT_END_DEPARTMENT = endDepa.Count;
                                ado.TreatmentEndDepartment = endDepa;
                            }

                            // tổng số BN phải xử lý trong thời gian lọc. Bao gồm chưa xử lý, đang xử lý, đã đóng. Không tính y lệnh không thực hiện, bị gạch/xóa.
                            var curr = groups.Where(o => o.IS_NO_EXECUTE != 1 && o.INTRUCTION_TIME >= timeFrom && o.INTRUCTION_TIME <= timeTo).ToList();
                            if (curr != null && curr.Count > 0)
                            {
                                ado.COUNT_CURR = curr.Count;
                                ado.TreatmentCurr = listTreatment.Where(o => curr.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                            }

                            // tổng số BN đã tác động. Bao gồm đang xử lý và đã đóng.
                            var dth = groups.Where(o => o.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && o.INTRUCTION_TIME >= timeFrom && o.INTRUCTION_TIME <= timeTo).ToList();
                            if (dth != null && dth.Count > 0)
                            {
                                ado.COUNT_DEATH = dth.Count;
                                ado.TreatmentCurr = listTreatment.Where(o => dth.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                            }

                            ListDepartment.Add(ado);
                        }
                    }

                    if (ListDepartment != null && ListDepartment.Count > 0)
                    {
                        ListDepartment = ListDepartment.OrderByDescending(o => o.COUNT_IN).ThenByDescending(o => o.COUNT_OUT).ThenByDescending(o => o.COUNT_OLD).ThenByDescending(o => o.COUNT_CURR).ToList();
                    }

                    gridControlExamTotal.BeginUpdate();
                    gridControlExamTotal.DataSource = null;
                    gridControlExamTotal.DataSource = ListDepartment;
                    gridControlExamTotal.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridDetail(List<HIS_SERVICE_REQ> listServiceReq, List<ADO.TreatmentADO> listTreatment, long timeFrom, long timeTo)
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0 && listTreatment != null && listTreatment.Count > 0)
                {
                    List<HIS_SERVICE_REQ> current = listServiceReq.Where(o => o.INTRUCTION_TIME >= timeFrom && o.INTRUCTION_TIME <= timeTo).ToList();
                    var listTreatmentAdo = listTreatment.Where(o => current.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();

                    gridControlDetail.BeginUpdate();
                    gridControlDetail.DataSource = null;
                    gridControlDetail.DataSource = listTreatmentAdo;
                    gridControlDetail.EndUpdate();
                }
                else
                {
                    gridControlDetail.BeginUpdate();
                    gridControlDetail.DataSource = null;
                    gridControlDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListTreatmentServiceReqAdo(List<ADO.DepartmentADO> listDepartmentTran, ref List<ADO.TreatmentADO> listDataGridDetail)
        {
            try
            {
                if (listDepartmentTran != null && listDepartmentTran.Count > 0)
                {
                    if (listDataGridDetail == null) listDataGridDetail = new List<ADO.TreatmentADO>();

                    List<ADO.DepartmentADO> filterDepartment = new List<ADO.DepartmentADO>();
                    if (CboDepartment.EditValue != null)
                    {
                        var departmentId = (long)(CboDepartment.EditValue ?? "0");
                        var depa = listDepartmentTran.FirstOrDefault(o => o.DEPARTMENT_ID == departmentId);
                        filterDepartment.Add(depa);
                    }
                    else
                    {
                        filterDepartment.AddRange(listDepartmentTran);
                    }

                    foreach (var item in filterDepartment)
                    {
                        if (CboFilter.EditValue != null)
                        {
                            long type = (long)(CboFilter.EditValue ?? "0");

                            if (type == 1 && item.TreatmentOld != null)//COUNT_OLD
                            {
                                listDataGridDetail.AddRange(item.TreatmentOld);
                            }
                            else if (type == 2 && item.TreatmentIn != null)//COUNT_IN
                            {
                                listDataGridDetail.AddRange(item.TreatmentIn);
                            }
                            else if (type == 3 && item.TreatmentOut != null)//COUNT_OUT
                            {
                                listDataGridDetail.AddRange(item.TreatmentOut);
                            }
                            else if (type == 4 && item.TreatmentTran != null)//COUNT_TRAN
                            {
                                listDataGridDetail.AddRange(item.TreatmentTran);
                            }
                            else if (type == 5 && item.TreatmentEndDepartment != null)//COUNT_END_DEPARTMENT
                            {
                                listDataGridDetail.AddRange(item.TreatmentEndDepartment);
                            }
                            else if (type == 6 && item.TreatmentCurr != null)//Req
                            {
                                listDataGridDetail.AddRange(item.TreatmentCurr);
                            }
                            else if (type == 7 && item.TreatmentDeath != null)//execute
                            {
                                listDataGridDetail.AddRange(item.TreatmentDeath);
                            }
                        }
                        else
                        {
                            if (item.TreatmentOld != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentOld);
                            }
                            if (item.TreatmentIn != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentIn);
                            }
                            if (item.TreatmentOut != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentOut);
                            }
                            if (item.TreatmentTran != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentTran);
                            }
                            if (item.TreatmentEndDepartment != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentEndDepartment);
                            }
                            if (item.TreatmentCurr != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentCurr);
                            }
                            if (item.TreatmentDeath != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentDeath);
                            }
                        }
                    }

                    listDataGridDetail = listDataGridDetail.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTretmentToGrid(long timeFrom, long timeTo)
        {
            try
            {
                List<long> typeChk = new List<long>();
                if (chkKham.Checked)
                    typeChk.Add(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM);
                if (chkNoiTru.Checked)
                    typeChk.Add(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU);
                if (chkNgoaiTru.Checked)
                    typeChk.Add(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU);
                if (chkBanNgay.Checked)
                    typeChk.Add(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY);

                var listTreatmentTotal = new Get.GetTreatment(timeFrom, timeTo, typeChk).GetTotalTreatment();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTreatmentTotal.Count), listTreatmentTotal.Count));
                if (listTreatmentTotal != null && listTreatmentTotal.Count > 0)
                {
                    listTreatmentTotal = listTreatmentTotal.Where(o => (o.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && o.CLINICAL_IN_TIME.HasValue)
                                                                    || (o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && o.IN_TIME > 0)).ToList();
                    if (listTreatmentTotal != null && listTreatmentTotal.Count > 0)
                    {
                        var TreatmentInfo = new Get.GetTreatmentInfo(listTreatmentTotal.Select(o => o.ID).ToList());
                        var dicTreatmentInfo = TreatmentInfo.Get();
                        var ListTreatement = ProcessDataADO(listTreatmentTotal, dicTreatmentInfo);

                        var branchId = long.Parse((CboBranch.EditValue ?? "0").ToString());
                        var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == branchId).ToList();

                        if (department != null && department.Count > 0)
                        {
                            ListTreatement = ListTreatement.Where(o => department.Select(s => s.ID).Contains(o.DEPARTMENT_ID)).ToList();
                            if (ListTreatement != null && ListTreatement.Count > 0)
                            {
                                department = department.Where(o => ListTreatement.Select(s => s.DEPARTMENT_ID).Distinct().Contains(o.ID)).ToList();
                            }


                            var TreatmentBedRoomInfo = new Get.GetTreatmentInfo(listTreatmentTotal.Select(o => o.ID).ToList(), timeFrom, timeTo, department).GetTreatmentBedRoom();

                            InitComboDepartment(department);
                            InitComboType(1);

                            if (ListTreatement != null && ListTreatement.Count > 0)
                            {
                                ProcessDataGridDetail(ListTreatement, TreatmentInfo.ListDepartmentTran, timeFrom, timeTo);
                                ProcessDataGridTotal(ListTreatement, TreatmentInfo.ListDepartmentTran, TreatmentBedRoomInfo, timeFrom, timeTo);
                                ProcessDataControl();
                                //ProcessDataControl(ListTreatement, timeFrom, timeTo);
                            }
                            else
                            {
                                SetDefaultDataGrid();
                            }
                        }
                    }
                }
                else
                {
                    SetDefaultDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void SetDefaultDataGrid()
        {
            try
            {
                gridControlDetail.BeginUpdate();
                gridControlDetail.DataSource = null;
                gridControlDetail.EndUpdate();
                gridControlExamTotal.BeginUpdate();
                gridControlExamTotal.DataSource = null;
                gridControlExamTotal.EndUpdate();
                gridControlTotal.BeginUpdate();
                gridControlTotal.DataSource = null;
                gridControlTotal.EndUpdate();

                this.TxtOld.Text = "";
                this.TxtIn.Text = "";
                this.TxtOut.Text = "";
                this.TxtCurrent.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EndableGridControl()
        {
            try
            {
                var dichVuSelected = radioGroupDichVu.EditValue != null ? Convert.ToInt32(radioGroupDichVu.EditValue) : -1;

                if (dichVuSelected == (int)Enum.DichVu.CanLamSang || dichVuSelected == (int)Enum.DichVu.Kham)
                {
                    gridControlExamTotal.Visible = true;
                    gridControlExamTotal.Dock = DockStyle.Fill;
                    gridControlTotal.Visible = false;
                }
                else
                {
                    gridControlTotal.Visible = true;
                    gridControlTotal.Dock = DockStyle.Fill;
                    gridControlExamTotal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region noi tru, ngoai tru
        private void ProcessDataGridTotal(List<ADO.TreatmentADO> listTreatmentAdo, List<V_HIS_DEPARTMENT_TRAN> listDepartmentTran, Dictionary<long, HIS_TREATMENT_BED_ROOM> dicTreatmentBedRoomInfo, long timeFrom, long timeTo)
        {
            try
            {
                if (listTreatmentAdo != null && listTreatmentAdo.Count > 0 && timeFrom > 0 && timeTo > 0)
                {
                    Dictionary<long, ADO.DepartmentADO> dicAdo = new Dictionary<long, ADO.DepartmentADO>();
                    ListDepartment = new List<ADO.DepartmentADO>();

                    Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>> dicDepartmentTranIn = new Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>>();
                    Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>> dicDepartmentTranOut = new Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>>();

                    foreach (var treatment in listTreatmentAdo)
                    {
                        if (treatment.ID == 150352)
                        {

                        }
                        long? treatmentInTime = 0;
                        if (treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            treatmentInTime = treatment.IN_TIME;
                        }
                        else
                        {
                            treatmentInTime = treatment.CLINICAL_IN_TIME;
                        }

                        ADO.DepartmentADO ado = new ADO.DepartmentADO();
                        if (dicAdo.ContainsKey(treatment.DEPARTMENT_ID))
                        {
                            ado = dicAdo[treatment.DEPARTMENT_ID];
                        }
                        else
                        {
                            ado.DEPARTMENT_ID = treatment.DEPARTMENT_ID;
                            ado.DEPARTMENT_NAME = treatment.DEPARTMENT_NAME;
                        }

                        if (treatmentInTime < timeTo && (!treatment.OUT_TIME.HasValue || treatment.OUT_TIME > timeTo || (treatment.IS_PAUSE ?? 0) != 1))
                        {
                            ado.COUNT_CURR += 1;
                            if (ado.TreatmentCurr == null) ado.TreatmentCurr = new List<ADO.TreatmentADO>();
                            ado.TreatmentCurr.Add(treatment);
                        }

                        if (treatment.IS_PAUSE == 1 && treatment.OUT_TIME.HasValue && treatment.OUT_TIME >= timeFrom && treatment.OUT_TIME <= timeTo
                            && treatment.END_DEPARTMENT_ID == ado.DEPARTMENT_ID)
                        {
                            ado.COUNT_END_DEPARTMENT += 1;
                            if (ado.TreatmentEndDepartment == null) ado.TreatmentEndDepartment = new List<ADO.TreatmentADO>();
                            ado.TreatmentEndDepartment.Add(treatment);
                        }

                        if (treatment.OUT_TIME.HasValue && treatment.OUT_TIME >= timeFrom && treatment.OUT_TIME <= timeTo && treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                        {
                            ado.COUNT_DEATH += 1;
                            if (ado.TreatmentDeath == null) ado.TreatmentDeath = new List<ADO.TreatmentADO>();
                            ado.TreatmentDeath.Add(treatment);
                        }

                        if (treatment.OUT_TIME.HasValue && treatment.OUT_TIME >= timeFrom && treatment.OUT_TIME <= timeTo && treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                        {
                            ado.COUNT_TRAN += 1;
                            if (ado.TreatmentTran == null) ado.TreatmentTran = new List<ADO.TreatmentADO>();
                            ado.TreatmentTran.Add(treatment);
                        }

                        if (treatment.IN_TIME >= timeFrom && treatment.IN_TIME <= timeTo && treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                        {
                            ado.COUNT_FEMALE += 1;
                            if (ado.TreatmentFemale == null) ado.TreatmentFemale = new List<ADO.TreatmentADO>();
                            ado.TreatmentFemale.Add(treatment);
                        }

                        if (treatment.IN_TIME >= timeFrom && treatment.IN_TIME <= timeTo && treatment.IS_EMERGENCY.HasValue && treatment.IS_EMERGENCY.Value == 1)
                        {
                            ado.COUNT_CC += 1;
                            if (ado.TreatmentCc == null) ado.TreatmentCc = new List<ADO.TreatmentADO>();
                            ado.TreatmentCc.Add(treatment);
                        }

                        if (dicTreatmentBedRoomInfo != null && dicTreatmentBedRoomInfo.Count > 0 && dicTreatmentBedRoomInfo.ContainsKey(treatment.ID))
                        {
                            var BedRoom = dicTreatmentBedRoomInfo[treatment.ID];
                            if (BedRoom.BED_ID != null)
                            {
                                var Beds = BackendDataWorker.Get<HIS_BED>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == BedRoom.BED_ID).FirstOrDefault();
                                if (Beds != null)
                                {
                                    if (GetFisrtChar(Beds.BED_CODE) == "H")
                                    {
                                        ado.COUNT_BED_H += 1;
                                        if (ado.BedH == null) ado.BedH = new List<ADO.TreatmentADO>();
                                        ado.BedH.Add(treatment);
                                    }
                                    else if (GetFisrtChar(Beds.BED_CODE) == "T")
                                    {
                                        ado.COUNT_BED_T += 1;
                                        if (ado.BedT == null) ado.BedT = new List<ADO.TreatmentADO>();
                                        ado.BedT.Add(treatment);
                                    }
                                }
                            }
                        }

                        if (treatment.IN_TIME >= timeFrom && treatment.IN_TIME <= timeTo && CheckYear(treatment.TDL_PATIENT_DOB, treatment.IN_TIME, 6, false))
                        {
                            ado.COUNT_6T += 1;
                            if (ado.TreatmentTreEm == null) ado.TreatmentTreEm = new List<ADO.TreatmentADO>();
                            ado.TreatmentTreEm.Add(treatment);
                        }

                        if (treatment.IN_TIME >= timeFrom && treatment.IN_TIME <= timeTo && CheckYear(treatment.TDL_PATIENT_DOB, treatment.IN_TIME, 80, true))
                        {
                            ado.COUNT_80T += 1;
                            if (ado.TreatmentCaoTuoi == null) ado.TreatmentCaoTuoi = new List<ADO.TreatmentADO>();
                            ado.TreatmentCaoTuoi.Add(treatment);
                        }

                        var listTranIn = treatment.DepartmentTran.Where(o => (o.DEPARTMENT_IN_TIME < treatmentInTime ? treatmentInTime : o.DEPARTMENT_IN_TIME) >= timeFrom && (o.DEPARTMENT_IN_TIME < treatmentInTime ? treatmentInTime : o.DEPARTMENT_IN_TIME) <= timeTo && !treatment.DepartmentTran.Exists(p => p.PREVIOUS_ID == o.ID && p.DEPARTMENT_IN_TIME <= treatmentInTime)).ToList();
                        if (listTranIn != null && listTranIn.Count > 0)
                        {
                            foreach (var item in listTranIn)
                            {
                                if (!dicDepartmentTranIn.ContainsKey(item.DEPARTMENT_ID))
                                    dicDepartmentTranIn[item.DEPARTMENT_ID] = new List<V_HIS_DEPARTMENT_TRAN>();
                                dicDepartmentTranIn[item.DEPARTMENT_ID].Add(item);
                            }
                        }

                        var tranPrevious = treatment.DepartmentTran.Where(o => o.DEPARTMENT_IN_TIME >= timeFrom && o.DEPARTMENT_IN_TIME <= timeTo && o.PREVIOUS_ID.HasValue).ToList();
                        if (tranPrevious != null && tranPrevious.Count > 0)
                        {
                            foreach (var tranPre in tranPrevious)
                            {
                                var tran = listDepartmentTran.FirstOrDefault(o => tranPre.PREVIOUS_ID == o.ID);
                                if (tran != null)
                                {
                                    if (!dicDepartmentTranOut.ContainsKey(tran.DEPARTMENT_ID))
                                        dicDepartmentTranOut[tran.DEPARTMENT_ID] = new List<V_HIS_DEPARTMENT_TRAN>();
                                    dicDepartmentTranOut[tran.DEPARTMENT_ID].Add(tranPre);
                                }
                            }
                        }

                        //if (treatment.CLINICAL_IN_TIME.HasValue)
                        //{
                        //    var listTran = treatment.DepartmentTran.Where(o => o.DEPARTMENT_IN_TIME >= timeFrom && o.DEPARTMENT_IN_TIME <= timeTo).ToList();
                        //    if (listTran != null && listTran.Count > 0)
                        //    {
                        //        var tranin = listTran.Where(o => o.DEPARTMENT_ID == ado.DEPARTMENT_ID).ToList();
                        //        if (tranin != null && tranin.Count > 0)
                        //        {
                        //            ado.COUNT_IN += 1;
                        //            if (ado.TreatmentIn == null) ado.TreatmentIn = new List<ADO.TreatmentADO>();
                        //            ado.TreatmentIn.Add(treatment);
                        //        }
                        //    }
                        //}

                        //var tranPrevious = treatment.DepartmentTran.Where(o => o.DEPARTMENT_IN_TIME >= timeFrom && o.DEPARTMENT_IN_TIME <= timeTo && o.PREVIOUS_ID.HasValue).ToList();
                        //if (tranPrevious != null && tranPrevious.Count > 0)
                        //{
                        //    foreach (var tranPre in tranPrevious)
                        //    {
                        //        var tran = listDepartmentTran.FirstOrDefault(o => o.DEPARTMENT_ID == ado.DEPARTMENT_ID && tranPre.PREVIOUS_ID == o.ID);
                        //        if (tran != null)
                        //        {
                        //            ado.COUNT_OUT += 1;
                        //            if (ado.TreatmentOut == null) ado.TreatmentOut = new List<ADO.TreatmentADO>();
                        //            ado.TreatmentOut.Add(treatment);
                        //        }
                        //    }
                        //}

                        var oldDepartment = treatment.DepartmentTran.Where(o => (o.DEPARTMENT_IN_TIME < treatmentInTime ? treatmentInTime : o.DEPARTMENT_IN_TIME) <= timeFrom && !treatment.DepartmentTran.Exists(p => p.PREVIOUS_ID == o.ID && p.DEPARTMENT_IN_TIME <= treatmentInTime)).ToList();
                        if (oldDepartment != null && oldDepartment.Count > 0)
                        {
                            var tran = oldDepartment
                                .OrderByDescending(o => o.DEPARTMENT_IN_TIME)
                                .ThenByDescending(o => o.ID).FirstOrDefault();

                            if (tran.DEPARTMENT_ID == ado.DEPARTMENT_ID)
                            {
                                ado.COUNT_OLD += 1;
                                if (ado.TreatmentOld == null) ado.TreatmentOld = new List<ADO.TreatmentADO>();
                                ado.TreatmentOld.Add(treatment);
                            }
                            else
                            {
                                ADO.DepartmentADO adoOld = new ADO.DepartmentADO();
                                if (dicAdo.ContainsKey(tran.DEPARTMENT_ID))
                                {
                                    adoOld = dicAdo[tran.DEPARTMENT_ID];
                                }
                                else
                                {
                                    adoOld.DEPARTMENT_ID = tran.DEPARTMENT_ID;
                                    adoOld.DEPARTMENT_NAME = tran.DEPARTMENT_NAME;
                                }

                                adoOld.COUNT_OLD += 1;
                                if (adoOld.TreatmentOld == null) adoOld.TreatmentOld = new List<ADO.TreatmentADO>();

                                var oldtreatment = new ADO.TreatmentADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.TreatmentADO>(oldtreatment, treatment);

                                oldtreatment.DEPARTMENT_ID = tran.DEPARTMENT_ID;
                                oldtreatment.DEPARTMENT_NAME = tran.DEPARTMENT_NAME;
                                adoOld.TreatmentOld.Add(oldtreatment);

                                dicAdo[tran.DEPARTMENT_ID] = adoOld;
                            }
                        }

                        dicAdo[treatment.DEPARTMENT_ID] = ado;
                    }

                    if (dicAdo != null && dicAdo.Count > 0)
                    {
                        ListDepartment = dicAdo.Values.ToList();
                    }

                    if (ListDepartment != null && ListDepartment.Count > 0)
                    {
                        foreach (var item in ListDepartment)
                        {
                            if (dicDepartmentTranIn.ContainsKey(item.DEPARTMENT_ID))
                            {
                                var lstTreatmentIds = dicDepartmentTranIn[item.DEPARTMENT_ID].Select(s => s.TREATMENT_ID).ToList();
                                if (lstTreatmentIds != null)
                                {
                                    var lstTreatIn = listTreatmentAdo.Where(o => lstTreatmentIds.Contains(o.ID)).ToList();
                                    if (lstTreatIn != null && lstTreatIn.Count > 0)
                                    {
                                        item.COUNT_IN = lstTreatIn.Count;
                                        item.TreatmentIn = new List<ADO.TreatmentADO>();
                                        item.TreatmentIn.AddRange(lstTreatIn);
                                    }
                                }
                            }

                            if (dicDepartmentTranOut.ContainsKey(item.DEPARTMENT_ID))
                            {
                                var lstTreatmentIds = dicDepartmentTranOut[item.DEPARTMENT_ID].Select(s => s.TREATMENT_ID).ToList();
                                if (lstTreatmentIds != null)
                                {
                                    var lstTreatOut = listTreatmentAdo.Where(o => lstTreatmentIds.Contains(o.ID)).ToList();
                                    if (lstTreatOut != null && lstTreatOut.Count > 0)
                                    {
                                        item.COUNT_OUT = lstTreatOut.Count;
                                        item.TreatmentOut = new List<ADO.TreatmentADO>();
                                        item.TreatmentOut.AddRange(lstTreatOut);
                                    }
                                }
                            }
                        }

                        ListDepartment = ListDepartment.OrderBy(o => o.DEPARTMENT_NAME).ToList();
                    }

                    gridControlTotal.BeginUpdate();
                    gridControlTotal.DataSource = null;
                    gridControlTotal.DataSource = ListDepartment;
                    gridControlTotal.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataControl(List<ADO.TreatmentADO> listTreatmentAdo, long timeFrom, long timeTo)
        {
            try
            {
                long old = 0, intreat = 0, outtreat = 0, currtreat = 0;

                if (listTreatmentAdo != null && listTreatmentAdo.Count > 0)
                {
                    old = listTreatmentAdo.Count(o => o.CLINICAL_IN_TIME < timeFrom);
                    intreat = listTreatmentAdo.Count(o => o.CLINICAL_IN_TIME >= timeFrom && o.CLINICAL_IN_TIME <= timeTo);
                    outtreat = listTreatmentAdo.Count(o => o.CLINICAL_IN_TIME.HasValue && o.OUT_TIME.HasValue && o.OUT_TIME >= timeFrom && o.OUT_TIME <= timeTo);
                    currtreat = old + (intreat - outtreat);
                }

                this.TxtOld.Text = old.ToString();
                this.TxtIn.Text = intreat.ToString();
                this.TxtOut.Text = outtreat.ToString();
                this.TxtCurrent.Text = currtreat.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridDetail(List<ADO.TreatmentADO> listTreatmentAdo, List<V_HIS_DEPARTMENT_TRAN> listDepartmentTran, long timeFrom, long timeTo)
        {
            try
            {
                if (listTreatmentAdo != null && listTreatmentAdo.Count > 0 && timeFrom > 0 && timeTo > 0)
                {
                    var listDataGridDetail = listTreatmentAdo.Where(o => o.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && o.CLINICAL_IN_TIME >= timeFrom && o.CLINICAL_IN_TIME <= timeTo
                                                                    || o.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && o.IN_TIME >= timeFrom && o.IN_TIME <= timeTo).ToList();

                    gridControlDetail.BeginUpdate();
                    gridControlDetail.DataSource = null;
                    gridControlDetail.DataSource = listDataGridDetail;
                    gridControlDetail.EndUpdate();
                }
                else
                {
                    gridControlDetail.BeginUpdate();
                    gridControlDetail.DataSource = null;
                    gridControlDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void ProcessListTreatmentAdo(List<ADO.DepartmentADO> listDepartmentTran, ref List<ADO.TreatmentADO> listDataGridDetail)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                if (listDepartmentTran != null && listDepartmentTran.Count > 0)
                {
                    if (listDataGridDetail == null) listDataGridDetail = new List<ADO.TreatmentADO>();

                    List<ADO.DepartmentADO> filterDepartment = new List<ADO.DepartmentADO>();
                    if (CboDepartment.EditValue != null)
                    {
                        var departmentId = (long)(CboDepartment.EditValue ?? "0");
                        var depa = listDepartmentTran.FirstOrDefault(o => o.DEPARTMENT_ID == departmentId);
                        filterDepartment.Add(depa);
                    }
                    else
                    {
                        filterDepartment.AddRange(listDepartmentTran);
                    }

                    foreach (var item in filterDepartment)
                    {
                        if (CboFilter.EditValue != null)
                        {
                            long type = (long)(CboFilter.EditValue ?? "0");

                            if (type == 1 && item.TreatmentOld != null)//COUNT_OLD
                            {
                                listDataGridDetail.AddRange(item.TreatmentOld);
                            }
                            else if (type == 2 && item.TreatmentIn != null)//COUNT_IN
                            {
                                listDataGridDetail.AddRange(item.TreatmentIn);
                            }
                            else if (type == 3 && item.TreatmentOut != null)//COUNT_OUT
                            {
                                listDataGridDetail.AddRange(item.TreatmentOut);
                            }
                            else if (type == 4 && item.TreatmentDeath != null)//COUNT_DEATH
                            {
                                listDataGridDetail.AddRange(item.TreatmentDeath);
                            }
                            else if (type == 5 && item.TreatmentTran != null)//COUNT_TRAN
                            {
                                listDataGridDetail.AddRange(item.TreatmentTran);
                            }
                            else if (type == 6 && item.TreatmentTreEm != null)//COUNT_6T
                            {
                                listDataGridDetail.AddRange(item.TreatmentTreEm);
                            }
                            else if (type == 7 && item.TreatmentCaoTuoi != null)//COUNT_80T
                            {
                                listDataGridDetail.AddRange(item.TreatmentCaoTuoi);
                            }
                            else if (type == 8 && item.TreatmentCc != null)//COUNT_CC
                            {
                                listDataGridDetail.AddRange(item.TreatmentCc);
                            }
                            else if (type == 9 && item.BedH != null)//COUNT_FEMALE
                            {
                                listDataGridDetail.AddRange(item.BedH);
                            }
                            else if (type == 10 && item.BedT != null)//COUNT_END_DEPARTMENT
                            {
                                listDataGridDetail.AddRange(item.BedT);
                            }
                            else if (type == 11 && item.TreatmentFemale != null)//COUNT_FEMALE
                            {
                                listDataGridDetail.AddRange(item.TreatmentFemale);
                            }
                            else if (type == 12 && item.TreatmentEndDepartment != null)//COUNT_END_DEPARTMENT
                            {
                                listDataGridDetail.AddRange(item.TreatmentEndDepartment);
                            }
                            else if (type == 13 && item.TreatmentCurr != null)//COUNT_CURR
                            {
                                listDataGridDetail.AddRange(item.TreatmentCurr);
                            }
                        }
                        else
                        {
                            if (item.TreatmentOld != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentOld);
                            }
                            if (item.TreatmentIn != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentIn);
                            }
                            if (item.TreatmentOut != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentOut);
                            }
                            if (item.TreatmentDeath != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentDeath);
                            }
                            if (item.TreatmentTran != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentTran);
                            }
                            if (item.TreatmentTreEm != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentTreEm);
                            }
                            if (item.TreatmentCaoTuoi != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentCaoTuoi);
                            }
                            if (item.TreatmentCc != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentCc);
                            }
                            if (item.BedH != null)
                            {
                                listDataGridDetail.AddRange(item.BedH);
                            }
                            if (item.BedT != null)
                            {
                                listDataGridDetail.AddRange(item.BedT);
                            }
                            if (item.TreatmentFemale != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentFemale);
                            }
                            if (item.TreatmentEndDepartment != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentEndDepartment);
                            }
                            if (item.TreatmentCurr != null)
                            {
                                listDataGridDetail.AddRange(item.TreatmentCurr);
                            }
                        }
                    }

                    listDataGridDetail = listDataGridDetail.Distinct().ToList();
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckYear(long dob, long inTime, long age, bool greater)
        {
            bool result = false;
            try
            {
                if (dob > 0 && inTime > 0)
                {
                    var yearDob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob) ?? DateTime.MinValue;
                    var yearInTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inTime) ?? DateTime.MinValue;
                    TimeSpan timeDifference = yearInTime - yearDob;

                    double Age = timeDifference.TotalDays / 365.2425;

                    if (greater) if (Age > age) result = true;
                        else if (Age < age) result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<ADO.TreatmentADO> ProcessDataADO(List<V_HIS_TREATMENT_4> listTreatmentTotal, Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>> dicTreatmentInfo)
        {
            List<ADO.TreatmentADO> result = new List<ADO.TreatmentADO>();
            try
            {
                if (listTreatmentTotal != null && listTreatmentTotal.Count > 0 && dicTreatmentInfo != null)
                {
                    foreach (var item in listTreatmentTotal)
                    {
                        if (dicTreatmentInfo.ContainsKey(item.ID))
                        {
                            var departmentTran = dicTreatmentInfo[item.ID].Where(o => o.DEPARTMENT_IN_TIME <= TimeTo).ToList();
                            if (departmentTran == null || departmentTran.Count <= 0)
                            {
                                departmentTran = dicTreatmentInfo[item.ID];
                            }
                            result.Add(new ADO.TreatmentADO(item, departmentTran));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<ADO.TreatmentADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public string GetFisrtChar(string data)
        {
            return data.ToUpper().First().ToString();
        }

        private void SetDefaultValueControl()
        {
            try
            {
                CboBranch.EditValue = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                //DateTime time7h = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
                //dateEdit.DateTime = time7h;
                TxtCurrent.Text = "0";
                TxtIn.Text = "0";
                TxtOld.Text = "0";
                TxtOut.Text = "0";
                chkNoiTru.Checked = true;
                BtnView.Enabled = false;
                var Beds = BackendDataWorker.Get<HIS_BED>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                dicBedGroupString = Beds.Where(o => o.BED_NAME.ToUpper().StartsWith("H") || o.BED_NAME.ToUpper().StartsWith("T")).ToList().GroupBy(o => GetFisrtChar(o.BED_NAME)).ToDictionary(o => o.Key, o => o.ToList());
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.BtnLoc.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__BTN_LOC");
                this.Gc_Address.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_ADDRESS");
                this.Gc_BnCu.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_CU");
                this.Gc_BnCu.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_CU__TOOL_TIP");
                this.Gc_BnRa.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_RA");
                this.Gc_BnRa.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_RA__TOOL_TIP");
                this.Gc_BnVao.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_VAO");
                this.Gc_BnVao.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_BN_VAO__TOOL_TIP");
                this.Gc_ClinicalInTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CLINNICAL_IN_TIME");
                this.Gc_CreateTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CREATE_TIME");
                this.Gc_Creator.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CREATOR");
                this.Gc_CurrentDepartment.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CURRENT_DEPARTMENT");
                this.Gc_DepartmentName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_DEPARTMENT_NAME");
                this.Gc_Dob.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_DOB");
                this.Gc_EndCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_END_CODE");
                this.Gc_GenderName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_GENDER_NAME");
                this.Gc_HeinCardNumber.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_HEIN_CARD_NUMBER");
                this.Gc_HienCo.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_HIEN_CO");
                this.Gc_HienCo.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_HIEN_CO__TOOL_TIP");
                this.Gc_IcdName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_ICD_NAME");
                this.Gc_IcdSubName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_ICD_SUB_NAME");
                this.Gc_InOrder.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_IN_ORDER");
                this.Gc_InTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_IN_TIME");
                this.Gc_Modifier.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_MODIFIER");
                this.Gc_ModifyTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_MODIFY_TIME");
                this.Gc_OutTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_OUT_TIME");
                this.Gc_PatientCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_PATIENT_CODE");
                this.Gc_PatientName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_PATIENT_NAME");
                this.Gc_StoreCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_STORE_CODE");
                this.Gc_Stt.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_STT");
                this.Gc_TreatmentCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_CODE");
                this.Gc_TreatmentTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_TYPE_NAME");
                this.LciCurrent.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_CURRENT");
                this.LciIn.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_IN");
                //this.LciKham.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_KHAM");
                //this.LciNgoaiTru.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_NGOAI_TRU");
                //this.LciNoiTru.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_NOI_TRU");
                this.LciOld.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_OLD");
                this.LciOut.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_OUT");

                this.Gc_6t.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_6T");
                this.Gc_80T.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_80T");
                this.Gc_Cc.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_CC");
                this.Gc_death.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_DEATH");
                this.Gc_Female.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_FEMALE");
                this.Gc_Tran.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TRAN_PATI");
                this.LciBranch.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_BRANCH");
                //this.LciDay.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_DAY");
                this.lciTimeFrom.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_TIME_FROM");
                this.lciTimeTo.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__LCI_TIME_TO");

                this.Gc_End_Depa.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_END_DEPA");
                this.Gc_End_Depa.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_END_DEPA__TOOL_TIP");

                this.Gc_TreatIn.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_IN");
                this.Gc_End.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_OUT");
                this.Gc_RoomName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_ROOM_NAME");
                this.Gc_TurnDone.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_DONE");
                this.Gc_TurnNew.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_NEW");
                this.Gc_TurnOld.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TURN_OLD");
                this.Gc_End.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TREATMENT_OUT__TOOL_TIP");
                this.Gc_TotalExecute.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TOTAL_EXECUTE");
                this.Gc_TotalReq.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_REPORT_COUNT_TREATMENT__GC_TOTAL_REQUEST");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void BtnLoc_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "DOB_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListDepartment != null && ListDepartment.Count > 0)
                {
                    var listDataGridDetail = new List<ADO.TreatmentADO>();
                    var dichVuSelected = radioGroupDichVu.EditValue != null ? Convert.ToInt32(radioGroupDichVu.EditValue) : -1;

                    if (dichVuSelected == (int)Enum.DichVu.Kham
                        || dichVuSelected == (int)Enum.DichVu.CanLamSang)
                    {
                        ProcessListTreatmentServiceReqAdo(ListDepartment, ref listDataGridDetail);
                    }
                    else
                    {
                        //lọc danh sách hồ sơ theo cbo khoa và cbo điều kiện.
                        ProcessListTreatmentAdo(ListDepartment, ref listDataGridDetail);
                    }

                    gridControlDetail.BeginUpdate();
                    gridControlDetail.DataSource = null;
                    gridControlDetail.DataSource = listDataGridDetail;
                    gridControlDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboDepartment.EditValue = null;
                    CboDepartment.Text = null;
                    CboDepartment.Reset();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboFilter_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboFilter.EditValue = null;
                    CboFilter.Text = null;
                    CboFilter.Reset();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                DateTime timeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                DateTime timeFrom = timeTo.AddDays(-1);
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == dtTimeFrom.Name)
                        {
                            if (!String.IsNullOrWhiteSpace(item.VALUE))
                            {
                                timeFrom = timeFrom.AddHours(long.Parse(item.VALUE.Substring(0, 2)));
                                timeFrom = timeFrom.AddMinutes(long.Parse(item.VALUE.Substring(2, 2)));
                            }
                        }
                        else if (item.KEY == dtTimeTo.Name)
                        {
                            if (!String.IsNullOrWhiteSpace(item.VALUE))
                            {
                                timeTo = timeTo.AddHours(long.Parse(item.VALUE.Substring(0, 2)));
                                timeTo = timeTo.AddMinutes(long.Parse(item.VALUE.Substring(2, 2)));
                            }
                        }
                    }
                }

                dtTimeFrom.EditValue = timeFrom;
                dtTimeTo.EditValue = timeTo;

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == dtTimeFrom.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = dtTimeFrom.DateTime.ToString("HHmm") + "00";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = dtTimeFrom.Name;
                    csAddOrUpdate.VALUE = dtTimeFrom.DateTime.ToString("HHmm") + "00";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == dtTimeTo.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = dtTimeTo.DateTime.ToString("HHmm") + "00";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = dtTimeTo.Name;
                    csAddOrUpdate.VALUE = dtTimeTo.DateTime.ToString("HHmm") + "00";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroupDienDieuTri_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //    if (radioGroupDienDieuTri.EditValue != null)
                //    {
                //        if (radioGroupDichVu.EditValue != null)
                //            radioGroupDichVu.SelectedIndex = -1;

                //        BtnView.Enabled = false;
                //    }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroupDichVu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioGroupDichVu.EditValue != null)
                {
                    chkKham.Checked = chkNoiTru.Checked = chkNgoaiTru.Checked = chkBanNgay.Checked = false;
                    BtnView.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckStatus(CheckEdit chk)
        {
            try
            {
                if (chk.Checked)
                {
                    BtnView.Enabled = false;
                    radioGroupDichVu.SelectedIndex = -1;
                }
                if (!IsNoiTru && !IsKham && !IsNgoaiTru && !IsBanNgay && radioGroupDichVu.SelectedIndex == -1)
                {
                    chk.Checked = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKham_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                IsKham = chkKham.Checked;
                CheckStatus(chkKham);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNoiTru_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                IsNoiTru = chkNoiTru.Checked;
                CheckStatus(chkNoiTru);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNgoaitru_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                IsNgoaiTru = chkNgoaiTru.Checked;
                CheckStatus(chkNgoaiTru);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBanNgay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                IsBanNgay = chkBanNgay.Checked;
                CheckStatus(chkBanNgay);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

