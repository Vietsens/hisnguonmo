using Inventec.Core;
using Inventec.UC.ListReports.Base;
using Inventec.UC.ListReports.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Update
{
    class SarReportUpdateBehaviorDefault : Inventec.UC.ListReports.Base.BusinessBase, ISarReportUpdate
    {
        internal SarReportUpdateBehaviorDefault(CommonParam paramUpdate, SAR.EFMODEL.DataModels.SAR_REPORT data)
            : base(paramUpdate)
        {
            Data = data;
        }

        private SAR.EFMODEL.DataModels.SAR_REPORT Data { get; set; }

        public SAR.EFMODEL.DataModels.SAR_REPORT Update()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT result = null;

            #region logging input data
            try
            {
                TokenCheck(); Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion
            try
            {
                var rs = Base.ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<SAR.EFMODEL.DataModels.SAR_REPORT>>("/api/SarReport/UpdateNameDescription", param, Data);
                if (rs != null)
                {
                    if (rs.Param != null) { param.Messages.AddRange(rs.Param.Messages); param.BugCodes.AddRange(rs.Param.BugCodes); }
                    result = rs.Data;
                }
                if (result == null) { LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data)); }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                Logging("Co loi khi goi api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode), LogType.Info);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageUtil.SetMessage(param, Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.HasException = true;
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBBanQuyenKhongHopLe);
                }
            }
            catch (AggregateException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageUtil.SetMessage(param, Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //BugUtil.SetBugCode(param, Bug.Enum.CoLoiXayRa);
                result = null;
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
