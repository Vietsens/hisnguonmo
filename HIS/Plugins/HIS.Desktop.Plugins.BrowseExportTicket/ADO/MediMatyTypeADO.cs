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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class MediMatyTypeADO : HisMedicineTypeInStockSDO
    {
        public string ADDITION_INFO { get; set; }
        public decimal YCD_AMOUNT { get; set; } 

        public MediMatyTypeADO()
        {
        }

        public MediMatyTypeADO(HisMedicineTypeInStockSDO _data)
        {
            try
            {
                if (_data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HisMedicineTypeInStockSDO>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }
                }
            }

            catch (Exception)
            {

            }
        }

        public MediMatyTypeADO(HisMaterialTypeInStockSDO _data)
        {
            try
            {
                if (_data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HisMaterialTypeInStockSDO>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }

                    this.MedicineTypeCode = _data.MaterialTypeCode;
                    this.MedicineTypeName = _data.MaterialTypeName;
                }
            }

            catch (Exception)
            {

            }
        }
    }
}
