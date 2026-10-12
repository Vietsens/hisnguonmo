using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.HisExportMestMedicine.Base
{
    /// <summary>
    /// Ket qua kiem tra phieu voi Cong mua sam (vCong 58605). Trung ten truong voi MOS.SDO.CongMuaSamCheckResultSDO.
    /// </summary>
    internal class CongMuaSamCheckADO
    {
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string Reason { get; set; }
        public bool IsConnected { get; set; }
    }

    /// <summary>
    /// Hoi backend phieu nhap tu NCC / phieu xuat tra NCC con duoc sua, xoa hay khong theo Cong mua sam.
    /// Chi la tien ich khoa nut truoc; backend van kiem tra lai khi thuc hien.
    /// </summary>
    internal static class CongMuaSamChecker
    {
        private const string IS_ENABLED_KEY = "MOS.CONG_MUA_SAM.IS_ENABLED";
        private const string IMP_MEST_CHECK_URI = "api/HisImpMest/CheckCongMuaSam";
        private const string EXP_MEST_CHECK_URI = "api/HisExpMest/CheckCongMuaSam";

        internal static bool IsEnabled
        {
            get
            {
                try
                {
                    return HisConfigs.Get<string>(IS_ENABLED_KEY) == "1";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return false;
                }
            }
        }

        /// <summary>Tra null khi tat tinh nang hoac goi loi (khi do giu nguyen nut nhu hien tai).</summary>
        internal static CongMuaSamCheckADO CheckImpMest(long impMestId)
        {
            return Call(IMP_MEST_CHECK_URI, impMestId);
        }

        internal static CongMuaSamCheckADO CheckExpMest(long expMestId)
        {
            return Call(EXP_MEST_CHECK_URI, expMestId);
        }

        private static CongMuaSamCheckADO Call(string uri, long id)
        {
            if (!IsEnabled) return null;
            try
            {
                CommonParam param = new CommonParam();
                return new BackendAdapter(param).Post<CongMuaSamCheckADO>(uri, ApiConsumers.MosConsumer, id, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
