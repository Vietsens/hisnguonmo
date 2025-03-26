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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Sar.Calendar
{
    class SarReportCalendarCreate : Inventec.UC.ScheduleReport.Base.BusinessBase
    {
        internal SarReportCalendarCreate(CommonParam paramCreate, SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR data)
            : base(paramCreate)
        {
            Data = data;
        }

        private SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR Data { get; set; }

        internal SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR Create()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR result = null;
            try
            {
                var rs = Base.ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<SAR.EFMODEL.DataModels.SAR_REPORT_CALENDAR>>("/api/SarReportCalendar/Create", param, Data);
                if (rs != null)
                {
                    if (rs.Param != null) { param.Messages.AddRange(rs.Param.Messages); param.BugCodes.AddRange(rs.Param.BugCodes); }
                    if (rs.Success)
                        result = rs.Data;
                }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai));
                }
            }
            catch (AggregateException ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.Messages.Add(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
                //IPF.MANAGER.Base.BugUtil.SetBugCode(param, IPF.LibraryBug.Bug.Enum.CoLoiXayRa);
            }

            #region Logging
            if (result == null)
            {
                LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
            }
            #endregion
            return result;
        }
    }
}
