/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisServiceConsult.ADO
{
    /// <summary>
    /// Container ket qua tu van + danh sach goi.
    /// </summary>
    public class HisServiceConsultSDO
    {
        public HIS_SERVICE_CONSULT Consult { get; set; }
        public List<HIS_CONSULT_PACKAGE> Packages { get; set; }
    }
}
