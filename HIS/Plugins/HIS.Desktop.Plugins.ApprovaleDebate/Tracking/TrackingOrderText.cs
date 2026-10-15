using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ApprovaleDebate.Tracking
{
    /// <summary>
    /// Dung cot Y lenh cua tab To dieu tri: thuoc, chi dinh can lam sang, y lenh chu cua to.
    /// Khong hien ma y lenh, vat tu va cac loai dich vu khac.
    /// </summary>
    internal static class TrackingOrderText
    {
        private static readonly List<long> SUBCLINICAL_SERVICE_TYPE_IDS = new List<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
        };

        /// <param name="sereServs">Dich vu cua to (D_HIS_SERE_SERV_2 co TRACKING_ID = ID to)</param>
        /// <param name="dicServiceReq">Y lenh theo ID, de doc IS_NOT_SHOW_MEDICINE_TRACKING</param>
        /// <param name="medicalInstruction">Y lenh chu cua to (HIS_TRACKING.MEDICAL_INSTRUCTION)</param>
        internal static string Build(List<DHisSereServ2> sereServs, Dictionary<long, HIS_SERVICE_REQ> dicServiceReq, string medicalInstruction)
        {
            List<string> lines = new List<string>();
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    //Thuoc: gop cung y lenh, cung thuoc, cung don vi, cung cach dung va cong so luong
                    var medicineGroups = sereServs
                        .Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                            && o.IS_NO_EXECUTE != 1
                            && !IsHiddenOnTracking(o, dicServiceReq))
                        .GroupBy(o => new { o.SERVICE_REQ_ID, o.SERVICE_CODE, o.SERVICE_UNIT_NAME, o.TUTORIAL });
                    foreach (var group in medicineGroups)
                    {
                        DHisSereServ2 first = group.First();
                        decimal amount = group.Sum(o => o.AMOUNT ?? 0);
                        StringBuilder line = new StringBuilder(first.SERVICE_NAME);
                        if (!String.IsNullOrWhiteSpace(first.TDL_MEDICINE_CONCENTRA))
                            line.Append(" - ").Append(first.TDL_MEDICINE_CONCENTRA.Trim());
                        line.Append(" x ").Append(amount.ToString("G29"));
                        if (!String.IsNullOrWhiteSpace(first.SERVICE_UNIT_NAME))
                            line.Append(" ").Append(first.SERVICE_UNIT_NAME.Trim());
                        lines.Add(line.ToString());
                        if (!String.IsNullOrWhiteSpace(first.TUTORIAL))
                            lines.Add(first.TUTORIAL.Trim());
                    }

                    //Chi dinh can lam sang: moi loai 1 dong "Ten loai: Dich vu 1; Dich vu 2"
                    List<HIS_SERVICE_TYPE> serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                    foreach (long serviceTypeId in SUBCLINICAL_SERVICE_TYPE_IDS)
                    {
                        List<string> names = sereServs
                            .Where(o => o.TDL_SERVICE_TYPE_ID == serviceTypeId && o.IS_NO_EXECUTE != 1 && !String.IsNullOrWhiteSpace(o.SERVICE_NAME))
                            .Select(o => o.SERVICE_NAME.Trim())
                            .Distinct()
                            .ToList();
                        if (names.Count == 0)
                            continue;
                        HIS_SERVICE_TYPE serviceType = serviceTypes != null ? serviceTypes.FirstOrDefault(o => o.ID == serviceTypeId) : null;
                        string typeName = serviceType != null ? serviceType.SERVICE_TYPE_NAME : "";
                        lines.Add(String.Format("{0}: {1}", typeName, String.Join("; ", names)));
                    }
                }

                if (!String.IsNullOrWhiteSpace(medicalInstruction))
                    lines.Add(medicalInstruction.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return String.Join(Environment.NewLine, lines);
        }

        private static bool IsHiddenOnTracking(DHisSereServ2 sereServ, Dictionary<long, HIS_SERVICE_REQ> dicServiceReq)
        {
            HIS_SERVICE_REQ serviceReq = null;
            return sereServ.SERVICE_REQ_ID.HasValue
                && dicServiceReq != null
                && dicServiceReq.TryGetValue(sereServ.SERVICE_REQ_ID.Value, out serviceReq)
                && serviceReq.IS_NOT_SHOW_MEDICINE_TRACKING == 1;
        }
    }
}
