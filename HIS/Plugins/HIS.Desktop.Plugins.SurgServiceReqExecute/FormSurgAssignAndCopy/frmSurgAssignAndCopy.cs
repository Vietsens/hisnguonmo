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
using HIS.Desktop.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors.Controls;
using System.Globalization;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.SignLibrary.ADO;
using EMR_MAIN.ChucNangKhac;
using HIS.Desktop.Plugins.SurgServiceReqExecute;
using Inventec.Common.Logging;
using DevExpress.XtraExport;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Collections;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.FormSurgAssignAndCopy
{
    public partial class frmSurgAssignAndCopy : Form //HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_SERVICE_REQ serviceReq;
        V_HIS_TREATMENT treatment;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        internal List<DateTime?> useTimeSelected = new List<DateTime?>();
        DateTime timeSelested;
        bool isInitUcDate;
        List<V_HIS_SERVICE> lstService;
        List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
        UCEkipUser ucEkip;
        long BEGINTIME;
        long ENDTIME;
        List<HisEkipUserADO> ekipAdo;
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 sereServ { get; set; }
        public frmSurgAssignAndCopy()
        {
            InitializeComponent();
        }

        public frmSurgAssignAndCopy(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ serviceReq, V_HIS_TREATMENT treatment, V_HIS_SERE_SERV_5 sereServ, long beginTime, long endTime, List<HisEkipUserADO> hisEkipUserADOs)
            //: base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.moduleData = moduleData;
                this.serviceReq = serviceReq;
                this.treatment = treatment;
                this.sereServ = sereServ;
                this.BEGINTIME = beginTime;
                this.ENDTIME = endTime;
                ekipAdo = hisEkipUserADOs;
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

        private void frmSurgAssignAndCopy_Load(object sender, EventArgs e)
        {
            try
            {
                //lblThongBao.Text = "";
                
                SetDefaultControlProperties();
                LoadServiceFromRam();
                SetDefaultValues();
                ValidateControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControls()
        {
            try
            {
                ValidationSingleControl(timeInstructionTime);
                ValidationInstructionDate();
                ValidTimeSpan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidTimeSpan()
        {
            try
            {
                Validate.ValidationRule.ValidTimeSpan vp = new Validate.ValidationRule.ValidTimeSpan();
                vp.inTime = treatment.IN_TIME;
                vp.outTime = treatment.OUT_TIME;
                vp.timeSpanEdit = timeInstructionTime;
                //vp.dateFromEdit = dtInstructionDateFrom;
                //vp.dateToEdit = dtInstructionDateTo;
                //vp.lciDate = lciInstructionDateFrom;
                vp.calendarControl = calendarInstructionDate;
                vp.lciCa = lciCalendarInstructionDate;
                dxValidationProvider2.SetValidationRule(timeInstructionTime, vp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationInstructionDate()
        {
            try
            {
                Validate.ValidationRule.InstructionDateFromValidationRule validRuleDate = new Validate.ValidationRule.InstructionDateFromValidationRule();
                validRuleDate.isRequired = true;
                //validRuleDate.dateFromEdit = dtInstructionDateFrom;
                //validRuleDate.dateToEdit = dtInstructionDateTo;
                //validRuleDate.lci = lciInstructionDateFrom;
                validRuleDate.inTime = treatment.IN_TIME;
                validRuleDate.outTime = treatment.OUT_TIME;
                validRuleDate.timeSpan = timeInstructionTime;
                //dxValidationProvider1.SetValidationRule(dtInstructionDateFrom, validRuleDate);

                Validate.ValidationRule.InstructionDateCalendarValidationRule validRuleCalendar = new Validate.ValidationRule.InstructionDateCalendarValidationRule();
                validRuleCalendar.isRequired = true;
                validRuleCalendar.calendarControl = calendarInstructionDate;
                validRuleCalendar.lci = lciCalendarInstructionDate;
                validRuleCalendar.inTime = treatment.IN_TIME;
                validRuleCalendar.outTime = treatment.OUT_TIME;
                validRuleCalendar.timeSpan = timeInstructionTime;
                dxValidationProvider1.SetValidationRule(calendarInstructionDate, validRuleCalendar);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlProperties()
        {
            try
            {
                //lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //chkNgayLienTiep.Checked = true;

                this.layoutControlRoot.MinimumSize = new System.Drawing.Size(this.layoutControlRoot.Width, 200);

                this.layoutControlRoot.AutoSize = true;
                this.layoutControlRoot.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.AutoSize = true;
                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValues()
        {
            try
            {
                timeInstructionTime.EditValue = null;
                //dtInstructionDateFrom.EditValue = null;
                //dtInstructionDateTo.EditValue = null;
                calendarInstructionDate.EditValue = null;
                if (this.serviceReq != null && this.serviceReq.ID > 0)
                {
                    var instructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.INTRUCTION_TIME);
                    if (instructionTime.HasValue)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("instructionTime", instructionTime));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("instructionTime.Value.TimeOfDay", instructionTime.Value.TimeOfDay));
                        timeInstructionTime.TimeSpan = instructionTime.Value.TimeOfDay;
                        //dtInstructionDateFrom.DateTime = instructionTime.Value.Date;
                        //dtInstructionDateTo.DateTime = instructionTime.Value.Date;
                        calendarInstructionDate.DateTime = instructionTime.Value.Date;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNgayLienTiep_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (chkNgayLienTiep.Checked)
                //{
                //    lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //    lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //    lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                //else
                //{
                //    lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //    lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //    lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.serviceReq != null && this.serviceReq.ID > 0)
                {
                    bool valid = dxValidationProvider1.Validate();
                    if (!valid)
                        return;
                    valid = valid && dxValidationProvider2.Validate();
                    if (!valid)
                        return;
                    SurgAssignAndCopySDO sdo = new SurgAssignAndCopySDO();

                    
                    if (SetSurgAssignAndCopySDO(ref sdo))
                    {
                        WaitingManager.Show();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"SurgAssignAndCopySDO", sdo));
                        var resultApi = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ__SURG_ASSIGN_AND_COPY, ApiConsumers.MosConsumer, sdo, param);
                        if (resultApi)
                        {
                            success = true;
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
                #region Hien thi message thong bao
                
                #endregion
                if (success)
                {
                    this.Close();
                }

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                //SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);   
            }
        }
        private void LoadServiceFromRam()
        {
            try
            {
                lstService = BackendDataWorker.Get<V_HIS_SERVICE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetSurgAssignAndCopySDO(ref SurgAssignAndCopySDO sdo)
        {
            try
            {
               
                V_HIS_SERVICE currentVHisService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (currentVHisService.ALLOW_SIMULTANEITY != 1)
                {
                    sdo.ServiceReqId = this.serviceReq.ID;
                    sdo.InstructionTimes = new List<long>();
                    sdo.Usetimes = new List<long>();
                    string time = (DateTime.Today.Date + timeInstructionTime.TimeSpan).ToString("HHmm") + "00";

                    List<DateTime> ngayYLenhList = GetDateListFromTextBox(txtNgayYLenh.Text);
                    List<DateTime> ngayDuTruList = GetDateListFromTextBox(txtNgayDuTruTime.Text);

                    sdo.InstructionTimes = ngayYLenhList.Select(date => Convert.ToInt64(date.ToString("yyyyMMdd") + time)).ToList();
                    sdo.Usetimes = ngayDuTruList.Select(date => Convert.ToInt64(date.ToString("yyyyMMdd") + time)).ToList();

                    List<long> mergedList = (sdo.InstructionTimes ?? new List<long>()).Union(sdo.Usetimes ?? new List<long>()).ToList();

                    LogSystem.Info($"kiem tra key ASSIGN_SERVICE_SIMULTANEITY_OPTION {Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION} ");
                    LogSystem.Info($"kiem tra key CHECK_SIMULTANEITY_OPTION {Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION} ");

                    if (Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1"
                        || Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                    {
                        HisSereServCheckExecuteTimesSDO inputSDO = new HisSereServCheckExecuteTimesSDO();
                        
                        CommonParam paramHisServiceReq = new CommonParam();
                        var Login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        LogSystem.Info($"Login {Login}");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"Login", Login));
                        inputSDO.TreatmentId = serviceReq.TREATMENT_ID;
                        LogSystem.Info($"inputSDO.TreatmentId {inputSDO.TreatmentId}, {serviceReq.TREATMENT_ID}");
                        List<string> dsLogin = new List<string> { Login };
                        var dataGrid = ekipAdo;
                        if (dataGrid != null && dataGrid.Count() > 0)
                            foreach (var data in dataGrid)
                            {
                                MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, data);
                                if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                                    ekipUsers.Add(ekipUser);
                            }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"ekipUsers", ekipUsers));
                        List<string> lstLogin = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                        LogSystem.Info($"lstLogin {lstLogin}");
                        List<string> lstLoginValid = new List<string>();
                        foreach (string acc in lstLogin)
                        {
                            if (acc != null)
                            {
                                lstLoginValid.Add(acc);
                            }
                        }
                        if (lstLoginValid.Count == 0)
                        {
                            lstLoginValid.Add(Login);
                        }
                        inputSDO.Loginnames = lstLoginValid;
                        LogSystem.Info($"inputSDO.Loginnames {inputSDO.Loginnames}");
                        string message = "";

                        LogSystem.Info("lay api 1 :/api/HisSereServ/CheckExecuteTimes");
                        foreach (var item in mergedList)   
                        {
                            CommonParam paramHisSereServ = new CommonParam();
                            DateTime? begin = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item);
                            DateTime? begin_BD = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(BEGINTIME);

                            
                            string TimeDisplay = begin?.ToString("yyyyMMdd") ?? "00000000";
                            string strTimeDisplay = begin_BD?.ToString("HHmmss") ?? "000000";
                                                                                              
                            string cong = TimeDisplay + strTimeDisplay;
                                                                        
                            long timeAsLong = long.Parse(cong);

                            DateTime? end = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item);
                            DateTime? end_BD = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ENDTIME);

                            string EndTimeDisplay = end?.ToString("yyyyMMdd") ?? "00000000";
                            string EndstrTimeDisplay = end_BD?.ToString("HHmmss") ?? "000000";

                            string cong1 = EndTimeDisplay + EndstrTimeDisplay;
                            long timeAsLong1 = long.Parse(cong1);

                            inputSDO.ExecuteTime = new ExecuteTime
                            {
                                BeginTime = timeAsLong,
                                EndTime = timeAsLong1
                            };

                            bool success = new BackendAdapter(paramHisSereServ)
                                .Post<bool>("/api/HisSereServ/CheckExecuteTimes", ApiConsumers.MosConsumer, inputSDO, paramHisSereServ);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"/api/HisSereServ/CheckExecuteTimes", inputSDO));
                            if (success == false)
                            {

                                if (Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                                {
                                    XtraMessageBox.Show(paramHisSereServ.GetMessage(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                                else if (Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                                {
                                    DialogResult result = XtraMessageBox.Show(paramHisSereServ.GetMessage(), "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (result == DialogResult.No)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        LogSystem.Info("lay api 2 :/api/HisServiceReq/CheckSereTimes");
                        foreach (var Time in sdo.InstructionTimes)
                        {
                            CommonParam param = new CommonParam();
                            HisServiceReqCheckSereTimesSDO inputSereTimesSDO = new HisServiceReqCheckSereTimesSDO();
                            inputSereTimesSDO.SereTimes = new List<long> { Time }; ;
                            inputSereTimesSDO.TreatmentId = serviceReq.TREATMENT_ID;
                            inputSereTimesSDO.Loginnames = lstLoginValid;

                            bool SereTimesSDO = new BackendAdapter(param)
                                    .Post<bool>("/api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, inputSereTimesSDO, param);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"/api/HisServiceReq/CheckSereTimes", inputSereTimesSDO));

                            if (SereTimesSDO == false)
                            {
                                if (Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                                {
                                    XtraMessageBox.Show(param.GetMessage(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                                else if (Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                                {
                                    DialogResult result = XtraMessageBox.Show(param.GetMessage(), "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (result == DialogResult.No)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    if (Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "1"
                        || Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "2")
                    {
                        HisSurgServiceReqUpdateListSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateListSDO();
                        hisSurgResultSDO.SurgUpdateSDOs = new List<SurgUpdateSDO>();
                        SurgUpdateSDO singleData = new SurgUpdateSDO();
                        singleData.SereServExt = new HIS_SERE_SERV_EXT();
                        singleData.SereServId = sereServ.ID;

                        var Login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var dataGrid = ekipAdo;
                        if (dataGrid != null && dataGrid.Count() > 0)
                            foreach (var data in dataGrid)
                            {
                                MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, data);
                                if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                                    ekipUsers.Add(ekipUser);
                            }
                        singleData.EkipUsers = ekipUsers;
                        

                        LogSystem.Info("lay api 3 :api/HisServiceReq/CheckSurgSimultaneily");
                        foreach (long item in mergedList)
                        {
                            CommonParam paramSurgUpdates = new CommonParam();
                            DateTime? begin = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item);
                            DateTime? begin_BD = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(BEGINTIME);

                            // Chuyển đổi ngày và giờ sang định dạng không có ký tự đặc biệt
                            string TimeDisplay = begin?.ToString("yyyyMMdd") ?? "00000000";
                            string strTimeDisplay = begin_BD?.ToString("HHmmss") ?? "000000"; // Định dạng HHmmss
                                                                                              // Nối hai chuỗi
                            string cong = TimeDisplay + strTimeDisplay; // Ví dụ: "20240327153045"
                                                                        // Chuyển sang long
                            long timeAsLong = long.Parse(cong);

                            DateTime? end = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item);
                            DateTime? end_BD = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ENDTIME);

                            string EndTimeDisplay = end?.ToString("yyyyMMdd") ?? "00000000"; // Định dạng YYYYMMDD
                            string EndstrTimeDisplay = end_BD?.ToString("HHmmss") ?? "000000"; // Định dạng HHmmss

                            string cong1 = EndTimeDisplay + EndstrTimeDisplay; // Ví dụ: "20240327153045"   
                            long timeAsLong1 = long.Parse(cong1);

                            singleData.SereServExt = new HIS_SERE_SERV_EXT
                            {
                                BEGIN_TIME = timeAsLong,
                                END_TIME = timeAsLong1,     
                            };
                            hisSurgResultSDO.SurgUpdateSDOs.Add(singleData);

                            bool resultCheckSurgSimultaneily = new BackendAdapter(paramSurgUpdates).Post<bool>("api/HisServiceReq/CheckSurgSimultaneily", 
                                ApiConsumers.MosConsumer, hisSurgResultSDO, paramSurgUpdates);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"api/HisServiceReq/CheckSurgSimultaneily", hisSurgResultSDO));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"api/HisServiceReq/CheckSurgSimultaneily", resultCheckSurgSimultaneily.ToString()));
                            if (resultCheckSurgSimultaneily == false)
                            {
                                if (Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "1")
                                {
                                    XtraMessageBox.Show(paramSurgUpdates.GetMessage(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                                else if (Config.HisConfigKeys.CHECK_SIMULTANEITY_OPTION == "2")
                                {
                                    DialogResult result = XtraMessageBox.Show(paramSurgUpdates.GetMessage(), "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (result == DialogResult.No)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private List<DateTime> GetDateListFromTextBox(string text)
        {
            List<DateTime> dateList = new List<DateTime>();

            if (!string.IsNullOrEmpty(text))
            {
                string[] dateStrings = text.Split(';');

                foreach (string dateStr in dateStrings)
                {
                    if (DateTime.TryParseExact(dateStr.Trim(), "dd/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        // Gán n?m hi?n t?i ?? tránh l?i
                        date = new DateTime(DateTime.Now.Year, date.Month, date.Day);
                        dateList.Add(date);
                    }
                }
            }

            return dateList;
        }


        private List<DateTime> selectedDates = new List<DateTime>();
        private void txtNgayYLenh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    timeSelested = DateTime.Today.Add((TimeSpan)timeInstructionTime.EditValue);
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelected, timeSelested, (datas, time)
                        => SelectMultiIntructionTime(datas, time, txtNgayYLenh, true), "Ngay y lệnh");
                      frmChooseIntructionTime.ShowDialog();
                }
                else if (txtNgayYLenh.EditValue != null && e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtNgayYLenh.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectMultiIntructionTime(List<DateTime?> datas, DateTime time, TextEdit targetTextBox, bool isIntructionTime)
        {
            try
            {
                if (datas != null && time != DateTime.MinValue)
                {
                    string strTimeDisplay = "";
                    int num = 0;

                    if (isIntructionTime)
                    {
                        this.intructionTimeSelected = datas as List<DateTime?>;
                        this.intructionTimeSelected = this.intructionTimeSelected.OrderBy(o => o.Value).ToList();
                        foreach (var item in this.intructionTimeSelected)
                        {
                            if (item != null && item.Value != DateTime.MinValue)
                            {
                                strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                                num++;
                            }
                        }
                    }
                    else
                    {
                        this.useTimeSelected = datas as List<DateTime?>;
                        this.useTimeSelected = this.useTimeSelected.OrderBy(o => o.Value).ToList();
                        foreach (var item in this.useTimeSelected)
                        {
                            if (item != null && item.Value != DateTime.MinValue)
                            {
                                strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                                num++;
                            }
                        }
                    }

                    if (targetTextBox.Text != strTimeDisplay)
                    {
                        this.isInitUcDate = true;
                        this.timeSelested = time;
                        this.timeInstructionTime.EditValue = this.timeSelested.ToString("HH:mm");
                        targetTextBox.Text = strTimeDisplay;
                        this.isInitUcDate = false;
                    }
                }
                else if (datas == null && time != DateTime.MinValue)
                {
                    string strTimeDisplay = "";
                    int num = 0;

                    if (isIntructionTime)
                    {
                        XtraMessageBox.Show("Thời gian y lệnh không được trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {

                        this.useTimeSelected = null;
                        this.useTimeSelected = this.useTimeSelected.OrderBy(o => o.Value).ToList();
                        foreach (var item in this.useTimeSelected)
                        {
                            if (item != null && item.Value != DateTime.MinValue)
                            {
                                strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                                num++;
                            }
                        }
                    }

                    if (targetTextBox.Text != strTimeDisplay)
                    {
                        this.isInitUcDate = true;
                        this.timeSelested = time;
                        this.timeInstructionTime.EditValue = this.timeSelested.ToString("HH:mm");
                        targetTextBox.Text = strTimeDisplay;
                        this.isInitUcDate = false;
                    }
                }
                //ValidateDateSelections();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtNgayDuTruTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                timeSelested = DateTime.Today.Add((TimeSpan)timeInstructionTime.EditValue);
                frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(useTimeSelected, timeSelested, (datas, time) 
                    => SelectMultiIntructionTime(datas, time, txtNgayDuTruTime, false), "Ngay dự trù");
                frmChooseIntructionTime.ShowDialog();
            }
            else if (txtNgayDuTruTime.EditValue != null && e.Button.Kind == ButtonPredefines.Delete)
            {
                txtNgayDuTruTime.EditValue = null;
            }
        }
    }
}
