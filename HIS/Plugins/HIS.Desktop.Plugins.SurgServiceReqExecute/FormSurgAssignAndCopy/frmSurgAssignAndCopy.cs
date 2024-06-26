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

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.FormSurgAssignAndCopy
{
    public partial class frmSurgAssignAndCopy : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_SERVICE_REQ serviceReq;
        V_HIS_TREATMENT treatment;
        public frmSurgAssignAndCopy()
        {
            InitializeComponent();
        }

        public frmSurgAssignAndCopy(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ serviceReq, V_HIS_TREATMENT treatment)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.moduleData = moduleData;
                this.serviceReq = serviceReq;
                this.treatment = treatment;
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
                SetDefaultControlProperties();
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
                //ValidationSingleControl(timeInstructionTime);
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
                vp.dateFromEdit = dtInstructionDateFrom;
                vp.dateToEdit = dtInstructionDateTo;
                vp.lciDate = lciInstructionDateFrom;
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
                validRuleDate.dateFromEdit = dtInstructionDateFrom;
                validRuleDate.dateToEdit = dtInstructionDateTo;
                validRuleDate.lci = lciInstructionDateFrom;
                validRuleDate.inTime = treatment.IN_TIME;
                validRuleDate.outTime = treatment.OUT_TIME;
                validRuleDate.timeSpan = timeInstructionTime;
                dxValidationProvider1.SetValidationRule(dtInstructionDateFrom, validRuleDate);

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
                lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                chkNgayLienTiep.Checked = true;

                this.layoutControlRoot.MinimumSize = new System.Drawing.Size(this.layoutControlRoot.Width, 120);

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
                dtInstructionDateFrom.EditValue = null;
                dtInstructionDateTo.EditValue = null;
                calendarInstructionDate.EditValue = null;
                if (this.serviceReq != null && this.serviceReq.ID > 0)
                {
                    var instructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.INTRUCTION_TIME);
                    if (instructionTime.HasValue)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("instructionTime", instructionTime));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("instructionTime.Value.TimeOfDay", instructionTime.Value.TimeOfDay));
                        timeInstructionTime.TimeSpan = instructionTime.Value.TimeOfDay;
                        dtInstructionDateFrom.DateTime = instructionTime.Value.Date;
                        dtInstructionDateTo.DateTime = instructionTime.Value.Date;
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
                if (chkNgayLienTiep.Checked)
                {
                    lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciInstructionDateFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciInstructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCalendarInstructionDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
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
                    SetSurgAssignAndCopySDO(ref sdo);

                    var resultApi = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ__SURG_ASSIGN_AND_COPY, ApiConsumers.MosConsumer, sdo, param);
                    if (resultApi)
                    {
                        success = true;
                    }
                }
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
                if (success)
                {
                    this.Close();
                }

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSurgAssignAndCopySDO(ref SurgAssignAndCopySDO sdo)
        {
            try
            {
                sdo.ServiceReqId = this.serviceReq.ID;
                sdo.InstructionTimes = new List<long>();
                string time = (DateTime.Today.Date + timeInstructionTime.TimeSpan).ToString("HHmm") + "00";
                if (chkNgayLienTiep.Checked)
                {
                    var start = dtInstructionDateFrom.DateTime.Date;
                    var end = dtInstructionDateTo.DateTime.Date;
                    var instructionTimes = new List<string>();
                    if (end.Date >= start.Date)
                    {
                        for (var dt = start; dt <= end; dt = dt.AddDays(1))
                        {
                            sdo.InstructionTimes.Add(Convert.ToInt64(dt.ToString("yyyyMMdd") + time));
                        }
                    }
                }
                else
                {
                    sdo.InstructionTimes.Add(Convert.ToInt64(calendarInstructionDate.DateTime.ToString("yyyyMMdd") + time));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
