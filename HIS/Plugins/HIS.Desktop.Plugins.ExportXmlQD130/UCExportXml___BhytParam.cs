using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Chuan du lieu dau ra sua doi QD 3176, dot 2 - tran 6 thang luong co so o XML1 (truong 52, 53).
    /// Nap bang HIS_BHYT_PARAM (luong co so theo moc thoi gian) 1 lan khi mo man hinh va giao cho thu vien XML130
    /// (BhytParamStore.Data). Khong co du lieu thi thu vien bo qua buoc tran va ghi log Warn.
    /// </summary>
    public partial class UCExportXml
    {
        /// <summary>Nap HIS_BHYT_PARAM chay nen, khong lam cham mo man hinh; khong co thao tac giao dien.</summary>
        private void InitBhytParamForXml3176()
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CommonParam param = new CommonParam();
                        HisBhytParamFilter filter = new HisBhytParamFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        List<HIS_BHYT_PARAM> data = new BackendAdapter(param).Get<List<HIS_BHYT_PARAM>>("api/HisBhytParam/Get", ApiConsumers.MosConsumer, filter, param);
                        LogSystem.Info("ExportXmlQD130 - nap HIS_BHYT_PARAM (tran 6 thang luong co so XML1 3176): " + (data != null ? data.Count : 0) + " ban ghi"
                            + LogUtil.TraceData("BhytParam", data));
                        SetBhytParamStoreSafe(data);
                    }
                    catch (Exception exTask)
                    {
                        LogSystem.Warn(exTask);
                    }
                });
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// May tram con thu vien XML130 cu (chua co BhytParamStore): chi loi trong ham nay, khong anh huong cac thao tac khac.
        /// </summary>
        private static void SetBhytParamStoreSafe(List<HIS_BHYT_PARAM> data)
        {
            try
            {
                SetBhytParamStore(data);
            }
            catch (MissingMemberException ex)
            {
                LogSystem.Warn("ExportXmlQD130 - thu vien His.Bhyt.ExportXml.XML130 chua ho tro tran 6 thang luong co so (ban cu)", ex);
            }
            catch (TypeLoadException ex)
            {
                LogSystem.Warn("ExportXmlQD130 - thu vien His.Bhyt.ExportXml.XML130 chua ho tro tran 6 thang luong co so (ban cu)", ex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetBhytParamStore(List<HIS_BHYT_PARAM> data)
        {
            His.Bhyt.ExportXml.XML130.BhytParamStore.Data = data;
        }
    }
}
