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
using Inventec.UC.EventLogControl.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Sda.EventLog.Get
{
    class SdaEventLogGet : Inventec.UC.EventLogControl.Base.GetBase
    {
        internal SdaEventLogGet()
            : base()
        {

        }

        internal SdaEventLogGet(CommonParam paramGet)
            : base(paramGet)
        {

        }
        internal ResultObject Get(SDA.Filter.SdaEventLogFilter searchMVC)
        {
            var param = new CommonParam();
            return Get(searchMVC, ref param);
        }

        internal ResultObject Get(SDA.Filter.SdaEventLogFilter searchMVC, ref CommonParam paramCommon)
        {
            ResultObject result = null;

            #region logging input data
            try
            {
                //TokenCheck(); Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => searchMVC), searchMVC) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("begin call Api SdaEventLog/Get SdaEventLogFilter: " + Inventec.Common.Logging.LogUtil.TraceData("", searchMVC));
                searchMVC.ORDER_FIELD = "EVENT_TIME";
                searchMVC.ORDER_DIRECTION = "DESC";
                var rs = Base.ApiConsumerStore.SdaConsumer.Get<Inventec.Core.ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>>>("/api/SdaEventLog/Get", param, searchMVC);
                if (rs != null)
                {
                    param = rs.Param != null ? rs.Param : param;
                    paramCommon = rs.Param != null ? rs.Param : param;
                    result = rs.ConvertToResultObject();
                }
                Inventec.Common.Logging.LogSystem.Debug("END call Api SdaEventLog/Get ");
                if (result == null) { LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => searchMVC), searchMVC)); }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(Base.MessageUtil.GetMessage(Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.HasException = true;
                    param.Messages.Add(Base.MessageUtil.GetMessage(Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai));
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    param.Messages.Add(Base.MessageUtil.GetMessage(Message.Enum.HeThongTBBanQuyenKhongHopLe));
                }
            }
            catch (AggregateException ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.Messages.Add(Base.MessageUtil.GetMessage(Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
