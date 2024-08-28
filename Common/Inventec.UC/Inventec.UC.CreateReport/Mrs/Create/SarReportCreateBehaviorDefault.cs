using Inventec.Core;
using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Mrs.Create
{
    class SarReportCreateBehaviorDefault : Inventec.UC.CreateReport.Base.BusinessBase, ISarReportCreate
    {
        internal SarReportCreateBehaviorDefault(CommonParam paramCreate, MRS.SDO.CreateReportSDO SarReport)
            : base(paramCreate)
        {
            Data = SarReport;
        }

        private MRS.SDO.CreateReportSDO Data { get; set; }

        public bool Create(MRS.SDO.CreateReportSDO Data)
        {
            Inventec.Core.ApiResultObject<bool> rs = null;
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
                rs = Base.ApiConsumerStore.MrsConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/MrsReport/Create", param, Data);
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
                    //param.HasException = true;
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageUtil.SetMessage(param, Message.Enum.HeThongTBBanQuyenKhongHopLe);
                }
            }
            catch (AggregateException ex)
            {
                //param.HasException = true;
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
