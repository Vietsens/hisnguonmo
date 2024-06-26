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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.IdentityMaterialInformation.ADO
{
    public class IMaterialADO : V_HIS_IMP_MEST_MATERIAL
    {
        public bool? IsAllowEditSerialNumber { get; set; }
        public string SerialNumberSeedCode { get; set; }
        public int? REUSE_COUNT { get; set; }
        public string MATERIAL_SIZE { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeSerialNumber { get; set; }
        public string ErrorMessageSerialNumber { get; set; }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeSize { get; set; }
        public string ErrorMessageSize { get; set; }
        public IMaterialADO(V_HIS_IMP_MEST_MATERIAL data, List<HIS_MATERIAL> mate)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<IMaterialADO>(this, data);
                this.REUSE_COUNT = (int?)(data.MAX_REUSE_COUNT ?? data.MATY_MAX_REUSE_COUNT);
                if(mate != null && mate.Count > 0)
                {
                    var material = mate.FirstOrDefault(o => o.ID == data.MATERIAL_ID);
                    this.MATERIAL_SIZE = material != null ? material.MATERIAL_SIZE : null;
                }
            }
        }
    }
}
