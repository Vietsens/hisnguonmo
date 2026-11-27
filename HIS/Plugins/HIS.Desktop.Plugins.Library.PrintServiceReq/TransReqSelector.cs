using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    /// <summary>
    /// Payment request list that was handed in by the caller (vCong 58478).
    /// When the list is of this type every service request sheet of the print uses its only item,
    /// regardless of the "QR per service request" configuration.
    /// </summary>
    internal class ForcedTransReqList : List<HIS_TRANS_REQ>
    {
        internal ForcedTransReqList(HIS_TRANS_REQ transReq)
        {
            if (transReq != null)
                this.Add(transReq);
        }
    }

    /// <summary>
    /// Chooses the payment request whose QR code is printed on a service request sheet.
    /// </summary>
    internal static class TransReqSelector
    {
        internal static HIS_TRANS_REQ Choose(List<HIS_TRANS_REQ> transReqs, long? serviceReqTransReqId)
        {
            try
            {
                if (transReqs == null || transReqs.Count == 0)
                    return null;
                if (transReqs is ForcedTransReqList)
                    return transReqs.FirstOrDefault();
                return Config.HisTranReqQRCodeTreatmentPrint
                    ? transReqs.FirstOrDefault(o => o.ID == serviceReqTransReqId)
                    : transReqs.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
