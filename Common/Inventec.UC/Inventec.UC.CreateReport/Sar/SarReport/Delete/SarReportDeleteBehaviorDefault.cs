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
using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Delete
{
    class SarReportDeleteBehaviorDefault : Inventec.UC.CreateReport.Base.BusinessBase, ISarReportDelete
    {
        internal SarReportDeleteBehaviorDefault(CommonParam paramGet, SAR.EFMODEL.DataModels.SAR_REPORT data)
            : base(paramGet)
        {
            Data = data;
        }

        private SAR.EFMODEL.DataModels.SAR_REPORT Data { get; set; }

        public bool Delete()
        {
            bool result = false;

            #region logging input data
            try
            {
                TokenCheck(); Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion
            try
            {
                var rs = Base.ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/SarReport/Delete", param, Data);
                if (rs != null)
                {
                    param = rs.Param != null ? rs.Param : param;
                    if (rs.Success) result = rs.Data;
                }
                if (!result) { LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data)); }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                param.HasException = true;
                Logging("Co loi khi goi api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode), LogType.Info);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageUtil.SetMessage(param, Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBBanQuyenKhongHopLe);
                }
            }
            catch (AggregateException ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageUtil.SetMessage(param, Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //BugUtil.SetBugCode(param, Bug.Enum.CoLoiXayRa);
                result = false;
            }

            #region logging system data
            try
            {
                MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                if (param.HasException) { LogInOut(); }
            }
            catch { }
            #endregion
            return result;
        }
    }
}
