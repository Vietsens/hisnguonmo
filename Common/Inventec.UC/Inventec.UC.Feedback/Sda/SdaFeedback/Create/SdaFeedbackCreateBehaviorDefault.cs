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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.UC.Feedback.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    class SdaFeedbackCreateBehaviorDefault : Process.BusinessBase, ISdaFeedbackCreateBehavior
    {
        internal SdaFeedbackCreateBehaviorDefault(CommonParam paramCreate, SDA.EFMODEL.DataModels.SDA_FEEDBACK Feedback)
            : base(paramCreate)
        {
            Data = Feedback;
        }

        private SDA.EFMODEL.DataModels.SDA_FEEDBACK Data { get; set; }

        public SDA.EFMODEL.DataModels.SDA_FEEDBACK Create()
        {
            SDA.EFMODEL.DataModels.SDA_FEEDBACK result = null;
            Inventec.Core.ApiResultObject<SDA.EFMODEL.DataModels.SDA_FEEDBACK> rs = null;
            try
            {
                rs = Process.ApiConsumerStore.SdaConsumer.Post<ApiResultObject<SDA.EFMODEL.DataModels.SDA_FEEDBACK>>("/api/SdaFeedback/Create", param, Data);
                if (rs != null)
                {
                    if (rs.Param != null) { param.Messages.AddRange(rs.Param.Messages); param.BugCodes.AddRange(rs.Param.BugCodes); }
                    if (rs.Success)
                    {
                        result = rs.Data;
                    }
                }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                param.HasException = true;
                Logging("Có lỗi khi gọi api trả về " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode), LogType.Info);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageUtil.SetMessage(param, Message.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageUtil.SetMessage(param, Message.Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageUtil.SetMessage(param, Message.Message.Enum.HeThongTBBanQuyenKhongHopLe);
                }
            }
            catch (AggregateException ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageUtil.SetMessage(param, Message.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.BugCodes.Add("Có lỗi xẩy ra (001)");
                result = null;
            }

            if (result == null) { LogInOut(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data)); } return result;
        }
    }
}
