using HIS.Desktop.Plugins.AssignService.Config;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// vCong 58478: choose the services to collect before creating the payment QR,
    /// then print the service request sheet with the QR just created.
    /// Active only when HIS.Desktop.Plugins.CreateTransReqQR.AllowSelectServiceFromAssignService = 1.
    /// </summary>
    public partial class frmAssignService
    {
        /// <summary>Payment request created from the QR screen for <see cref="transReqQrSource"/>.</summary>
        HIS_TRANS_REQ transReqQrCreated;

        /// <summary>The save result the QR payment request belongs to; a later save makes the QR obsolete.</summary>
        HisServiceReqListResultSDO transReqQrSource;

        /// <summary>Adds the extra arguments read by the QR screen. Does nothing when the configuration is off.</summary>
        private void AddSelectServiceQrArgs(List<object> listArgs)
        {
            try
            {
                if (!HisConfigCFG.AllowSelectServiceFromAssignService || listArgs == null)
                    return;
                List<long> sereServIds = (this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.SereServs != null)
                    ? this.serviceReqComboResultSDO.SereServs.Select(o => o.ID).Distinct().ToList()
                    : new List<long>();
                listArgs.Add(sereServIds);
                listArgs.Add((Action<HIS_TRANS_REQ>)OnTransReqQrCreated);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Called by the QR screen right after the payment request is created.</summary>
        private void OnTransReqQrCreated(HIS_TRANS_REQ transReq)
        {
            try
            {
                if (transReq == null || this.serviceReqComboResultSDO == null)
                    return;
                this.transReqQrCreated = transReq;
                this.transReqQrSource = this.serviceReqComboResultSDO;
                Inventec.Common.Logging.LogSystem.Info("vCong58478 tao QR xong, in phieu chi dinh. TransReqId=" + transReq.ID);

                bool isSeparate = HisConfigCFG.IsSeparateSignAndPrint;
                var checkYCDV = this.lstLoaiPhieu != null
                    ? this.lstLoaiPhieu.FirstOrDefault(o => (isSeparate ? o.Print : o.Check) == true && o.ID == "gridView7_1")
                    : null;
                if (checkYCDV != null)
                    InPhieuYeuCauDichVu(true, null, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Hands the QR payment request to the print library when it belongs to the current save result.</summary>
        private void ApplyTransReqQr(HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor processor)
        {
            try
            {
                if (processor != null && this.transReqQrCreated != null && this.serviceReqComboResultSDO != null
                    && Object.ReferenceEquals(this.transReqQrSource, this.serviceReqComboResultSDO))
                    processor.TransReqForQrCode = this.transReqQrCreated;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
