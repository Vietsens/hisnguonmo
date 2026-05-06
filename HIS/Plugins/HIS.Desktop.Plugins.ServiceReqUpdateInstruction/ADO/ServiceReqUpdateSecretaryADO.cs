/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Code-first wrapper for HIS_SERVICE_REQ to include SECRETARY_LOGINNAME / SECRETARY_USERNAME.
 * Remove this ADO once MOS.EFMODEL.HIS_SERVICE_REQ natively includes those two columns —
 * then revert btnSave_Click to post HIS_SERVICE_REQ directly.
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction.ADO
{
    public class ServiceReqUpdateSecretaryADO : HIS_SERVICE_REQ
    {
        public string SECRETARY_LOGINNAME { get; set; }
        public string SECRETARY_USERNAME { get; set; }
    }
}
