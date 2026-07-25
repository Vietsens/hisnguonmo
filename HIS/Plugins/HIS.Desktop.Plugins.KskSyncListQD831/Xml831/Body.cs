/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>&lt;BODY&gt; — chứa hồ sơ sức khỏe.</summary>
    public class Body
    {
        [XmlElement("HOSOSUCKHOE")]
        public HoSoSucKhoe HoSoSucKhoe { get; set; }
    }
}
