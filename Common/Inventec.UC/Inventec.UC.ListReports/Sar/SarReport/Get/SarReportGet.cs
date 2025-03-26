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
using Inventec.UC.ListReports.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Get
{
    internal class SarReportGet : Inventec.UC.ListReports.Base.GetBase
    {
        internal SarReportGet()
            : base()
        {

        }

        internal SarReportGet(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.V_SAR_REPORT>> GetView(SAR.Filter.SarReportViewFilter searchMVC)
        {
            Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.V_SAR_REPORT>> result = null;

            #region logging input data
            try
            {
                TokenCheck(); Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => searchMVC), searchMVC) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion
            try
            {
                searchMVC.ORDER_FIELD = "MODIFY_TIME";
                searchMVC.ORDER_DIRECTION = "DESC";
                var ro = Base.ApiConsumerStore.SarConsumer.Get<Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.V_SAR_REPORT>>>("/api/SarReport/GetView", param, searchMVC);
                result = ro;
                //if (ro != null)
                //{
                //    param = ro.Param != null ? ro.Param : param;
                //    result = ro.ConvertToResultObject();
                //}
                if (result == null) { LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ro), ro) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => searchMVC), searchMVC)); }
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
