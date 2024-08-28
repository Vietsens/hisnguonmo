using Inventec.Core;
using Inventec.UC.CreateReport.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Mrs.GetFile
{
    class MrsReportGetFile : Inventec.UC.CreateReport.Base.BusinessBase
    {
        internal MrsReportGetFile()
            : base()
        {

        }

        internal MrsReportGetFile(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal FileHolder GetFile(long SarReportId)
        {
            FileHolder result = null;
            try
            {
                result = Base.ApiConsumerStore.MrsConsumer.GetFile("/api/MrsReport/GetFile", param, SarReportId);
                if (result == null) { LogInOut(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SarReportId), SarReportId)); }
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
            return result;
        }

    }
}
