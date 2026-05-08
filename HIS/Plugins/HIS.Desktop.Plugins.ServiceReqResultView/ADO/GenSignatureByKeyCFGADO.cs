/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
namespace HIS.Desktop.Plugins.ServiceReqResultView.ADO
{
    /// <summary>
    /// JSON ADO mapping ánh xạ key tài khoản ↔ key ảnh chữ ký, lưu trong
    /// HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG.
    /// Cấu trúc JSON: [{"LoginnameKey":"REQ_LOGINNAME","SignatureKey":"REQ_LOGINNAME_SIGNATURE"}, ...]
    /// </summary>
    internal class GenSignatureByKeyCFGADO
    {
        public string LoginnameKey { get; set; }
        public string SignatureKey { get; set; }
    }
}
