/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisServiceConsult.ADO
{
    /// <summary>
    /// Row ADO cho grid "Goi tu van".
    /// Mo rong HIS_PACKAGE voi co IS_CHECKED de binh checkbox.
    /// </summary>
    public class PackageGridADO : HIS_PACKAGE
    {
        public bool IS_CHECKED { get; set; }

        public PackageGridADO() { }

        public PackageGridADO(HIS_PACKAGE source)
        {
            if (source == null) return;
            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PACKAGE>(this, source);
        }
    }
}
