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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestViewDetail.ADO
{
    public class ImpMestBloodSDODetail : V_HIS_IMP_MEST_BLOOD
    {
        public decimal? TotalAmount { get; set; }
        public long BLOOD_TYPE_ID_EDIT { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal? VAT_RATIO_100_OLD { get; set; }
        public decimal IMP_PRICE_OLD { get; set; }

        public string PACKAGE_NUMBER_EDIT { get; set; }
        public long? EXPIRED_DATE_EDIT { get; set; }

        public ImpMestBloodSDODetail(V_HIS_IMP_MEST_BLOOD _data)
        {
            try
            {
                if (_data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_IMP_MEST_BLOOD>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }

                    BLOOD_TYPE_ID_EDIT = _data.BLOOD_TYPE_ID;
                    if (_data.VAT_RATIO != null)
                    {
                        VAT_RATIO_100 = Inventec.Common.Number.Get.RoundCurrency((_data.VAT_RATIO ?? 0) * 100, 2);
                        VAT_RATIO_100_OLD = VAT_RATIO_100;
                    }
                    else
                    {
                        VAT_RATIO_100 = null;
                        VAT_RATIO_100_OLD = null;
                    }

                    IMP_PRICE_OLD = _data.IMP_PRICE;

                    this.PACKAGE_NUMBER_EDIT = _data.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_EDIT = _data.EXPIRED_DATE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
