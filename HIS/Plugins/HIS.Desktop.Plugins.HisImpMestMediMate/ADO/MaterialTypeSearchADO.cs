/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.ADO
{
    /// <summary>
    /// Du lieu cho o tra danh muc vat tu: go duoc Ma vat tu / Ten vat tu.
    /// Vat tu khong co hoat chat nen khong co tieu chi hoat chat.
    /// </summary>
    class MaterialTypeSearchADO
    {
        public long ID { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }

        public MaterialTypeSearchADO() { }

        public MaterialTypeSearchADO(V_HIS_MATERIAL_TYPE data)
        {
            try
            {
                this.ID = data.ID;
                this.MATERIAL_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                this.MATERIAL_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
