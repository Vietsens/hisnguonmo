using Inventec.Core;
using Inventec.UC.CreateReport.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReportType.Get
{
    internal class SarReportTypeGet : Inventec.UC.CreateReport.Base.GetBase
    {
        internal SarReportTypeGet()
            : base()
        {

        }

        internal SarReportTypeGet(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> Get(SAR.Filter.SarReportTypeFilter searchMVC)
        {
            List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> result = null;

            #region logging input data
            try
            {
                TokenCheck(); Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => searchMVC), searchMVC) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion
            try
            {
                searchMVC.ORDER_FIELD = "REPORT_TYPE_CODE";
                searchMVC.ORDER_DIRECTION = "ASC";
                var ro = Base.ApiConsumerStore.SarConsumer.Get<Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>>("/api/SarReportType/Get", param, searchMVC);
                if (ro != null)
                {
                    param = ro.Param != null ? ro.Param : param;
                    result = ro.Data;
                }
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
