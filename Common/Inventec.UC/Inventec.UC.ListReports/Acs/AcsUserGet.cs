using Inventec.Core;
using Inventec.UC.ListReports.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Acs
{
    class AcsUserGet : Inventec.UC.ListReports.Base.GetBase
    {
        internal static List<ACS.EFMODEL.DataModels.ACS_USER> DATA;
        internal AcsUserGet()
            : base()
        {

        }

        internal AcsUserGet(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal List<ACS.EFMODEL.DataModels.ACS_USER> Get(ACS.Filter.AcsUserFilter searchMVC)
        {
            List<ACS.EFMODEL.DataModels.ACS_USER> result = null;
            try
            {
                if (DATA == null || DATA.Count <= 0)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>("/api/AcsUser/Get", Base.ApiConsumerStore.AcsConsumer, searchMVC, param);
                    //var rs = Base.ApiConsumerStore.AcsConsumer.Get<Inventec.Core.ApiResultObject<List<ACS.EFMODEL.DataModels.ACS_USER>>>("/api/AcsUser/Get", param, searchMVC);
                    if (rs != null)
                    {
                        //result = rs.Data;
                        DATA = rs;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("goi api AcsUser/Get" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    }
                }

                result = DATA;
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
