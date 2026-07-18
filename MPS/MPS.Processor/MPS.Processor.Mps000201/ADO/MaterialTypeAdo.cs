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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000201.ADO
{
    public class MaterialTypeAdo : V_HIS_MATERIAL_TYPE
    {
        public string SERVICE_UNIT_NAME_STR { get; set; }
        public Nullable<decimal> IMPORT_PRICE { get; set; }
        public Nullable<decimal> EXPORT_PRICE { get; set; }
        public string CREATE_TIME_STR { get; set; }

        public MaterialTypeAdo() { }

        public MaterialTypeAdo(V_HIS_MATERIAL_TYPE materialType)
        {
            try
            {
                if (materialType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeAdo>(this, materialType);

                    // Cac gia tri dien giai khop voi hien thi tren man hinh Danh sach loai vat tu
                    this.SERVICE_UNIT_NAME_STR = materialType.IMP_UNIT_ID.HasValue ? materialType.IMP_UNIT_NAME : materialType.SERVICE_UNIT_NAME;
                    if (materialType.LAST_IMP_VAT_RATIO != null)
                    {
                        if (materialType.LAST_IMP_PRICE != null)
                        {
                            this.IMPORT_PRICE = materialType.LAST_IMP_PRICE * (1 + materialType.LAST_IMP_VAT_RATIO);
                        }
                    }
                    else
                    {
                        this.IMPORT_PRICE = 0;
                    }
                    if (materialType.LAST_EXP_VAT_RATIO != null)
                    {
                        if (materialType.LAST_EXP_PRICE != null)
                        {
                            this.EXPORT_PRICE = materialType.LAST_EXP_PRICE * (1 + materialType.LAST_EXP_VAT_RATIO);
                        }
                    }
                    else
                    {
                        this.EXPORT_PRICE = 0;
                    }
                    this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(materialType.CREATE_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
