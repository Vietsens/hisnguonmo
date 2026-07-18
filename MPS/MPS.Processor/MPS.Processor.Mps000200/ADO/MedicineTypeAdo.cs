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
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000200.ADO
{
    public class MedicineTypeAdo : V_HIS_MEDICINE_TYPE
    {
        public string PARENT_NAME { get; set; }
        public string SERVICE_UNIT_NAME_STR { get; set; }
        public Nullable<decimal> IMPORT_PRICE { get; set; }
        public Nullable<decimal> EXPORT_PRICE { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO_STR { get; set; }
        public string IS_NUTRITION_FOOD_STR { get; set; }
        public string CREATE_TIME_STR { get; set; }

        public MedicineTypeAdo() { }

        public MedicineTypeAdo(V_HIS_MEDICINE_TYPE medicineType)
        {
            try
            {
                if (medicineType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeAdo>(this, medicineType);
                    if (medicineType.PARENT_ID.HasValue)
                    {
                        var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == medicineType.PARENT_ID.Value);
                        if (rs != null)
                        {
                            this.PARENT_NAME = rs.MEDICINE_TYPE_NAME;
                        }
                    }

                    // Cac gia tri dien giai khop voi hien thi tren man hinh Danh sach loai thuoc
                    this.SERVICE_UNIT_NAME_STR = medicineType.IMP_UNIT_ID.HasValue ? medicineType.IMP_UNIT_NAME : medicineType.SERVICE_UNIT_NAME;
                    if (medicineType.LAST_IMP_VAT_RATIO != null)
                    {
                        if (medicineType.LAST_IMP_PRICE != null)
                        {
                            this.IMPORT_PRICE = medicineType.LAST_IMP_PRICE * (1 + medicineType.LAST_IMP_VAT_RATIO);
                        }
                    }
                    else
                    {
                        this.IMPORT_PRICE = 0;
                    }
                    if (medicineType.LAST_EXP_VAT_RATIO != null)
                    {
                        if (medicineType.LAST_EXP_PRICE != null)
                        {
                            this.EXPORT_PRICE = medicineType.LAST_EXP_PRICE * (1 + medicineType.LAST_EXP_VAT_RATIO);
                        }
                    }
                    else
                    {
                        this.EXPORT_PRICE = 0;
                    }
                    if (medicineType.HEIN_LIMIT_RATIO.HasValue)
                    {
                        this.HEIN_LIMIT_RATIO_STR = medicineType.HEIN_LIMIT_RATIO * 100;
                    }
                    this.IS_NUTRITION_FOOD_STR = (medicineType.IS_NUTRITION_FOOD == 1) ? "X" : "";
                    this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(medicineType.CREATE_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
