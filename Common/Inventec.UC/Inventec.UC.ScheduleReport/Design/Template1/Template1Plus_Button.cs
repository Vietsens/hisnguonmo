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
using DevExpress.Utils;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Design.Template1
{
    internal partial class Template1
    {

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                waitLoad = new WaitDialogForm(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));

                SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR DataSchedule = new SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR();
                string hour = spinHour.Value.ToString("00");
                string minutes = spinMinutes.Value.ToString("00");
                long executetime = Inventec.Common.TypeConvert.Parse.ToInt64(dtExecuteDate.DateTime.ToString("yyyyMMdd") + hour + minutes + "59");
                long endTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtEndDate.DateTime.ToString("yyyyMMdd") + "235959");
                DataSchedule.EXECUTE_TIME = executetime;
                DataSchedule.END_TIME = endTime;
                if (checkDaybyDay.Checked)
                {
                    DataSchedule.IS_DAY_REPEAT = IMSys.DbConfig.SAR_RS.SAR_REPORT_CALENDAR.IS_DAY_REPEAT__TRUE;
                }
                else if (checkWeekByWeek.Checked)
                {
                    DataSchedule.IS_WEEK_REPEAT = IMSys.DbConfig.SAR_RS.SAR_REPORT_CALENDAR.IS_WEEK_REPEAT__TRUE;
                }
                else if (checkMonthByMonth.Checked)
                {
                    DataSchedule.IS_MONTH_REPEAT = IMSys.DbConfig.SAR_RS.SAR_REPORT_CALENDAR.IS_MONTH_REPEAT__TRUE;
                }
                else if (checkLastDayOfMonth.Checked)
                {
                    DataSchedule.IS_MONTH_REPEAT = IMSys.DbConfig.SAR_RS.SAR_REPORT_CALENDAR.IS_MONTH_REPEAT__END_MONTH;
                }

                MRS.SDO.CreateReportSDO ReportSDO = _GetReport();
                if (_GetReport != null)
                {
                    if (ReportSDO != null)
                    {
                        DataSchedule.FILTER_TOTAL_JSON = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSDO);
                        DataSchedule.CREATOR = Base.TokenClientStore.ClientTokenManager.GetLoginName();
                        if ((new Sar.Calendar.SarReportCalendarCreate(param, DataSchedule).Create()) != null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Du lieu MRS.SDO.CreateReportSDO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ReportSDO), ReportSDO));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Delegate GetReportSDO null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _GetReport), _GetReport));
                }
                waitLoad.Dispose();

                #region Show Message
                Base.ResultManager.ShowMessage(param, success);
                #endregion

                #region Has Exception
                if (_HasException != null) _HasException(param);
                #endregion
            }
            catch (Exception ex)
            {
                waitLoad.Dispose();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
