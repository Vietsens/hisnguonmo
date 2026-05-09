/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
namespace HIS.Desktop.Plugins.ServiceExecute.ADO
{
    /// <summary>
    /// JSON ADO mapping anh xa key tai khoan <-> key anh chu ky, luu trong
    /// HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG.
    /// Cau truc JSON: [{"LoginnameKey":"REQ_LOGINNAME","SignatureKey":"REQ_LOGINNAME_SIGNATURE"}, ...]
    /// Tham khao plugin HIS.Desktop.Plugins.ServiceReqResultView (cung feature).
    /// </summary>
    internal class GenSignatureByKeyCFGADO
    {
        public string LoginnameKey { get; set; }
        public string SignatureKey { get; set; }
    }
}
