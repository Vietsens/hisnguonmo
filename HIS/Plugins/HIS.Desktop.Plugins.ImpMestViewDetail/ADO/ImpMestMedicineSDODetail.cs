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
    public class ImpMestMedicineSDODetail : V_HIS_IMP_MEST_MEDICINE
    {
        public decimal? TotalAmount { get; set; }
        public long MEDICINE_TYPE_ID_EDIT { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal? VAT_RATIO_100_OLD { get; set; }
        public bool IS_EDIT { get; set; }
        public decimal AMOUNT_OLD { get; set; }
        public decimal? PRICE_OLD { get; set; }
        public decimal? HienPrice { get; set; }
        public decimal? PriceVP { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal TOTAL_PRICE_OLD { get; set; }

        public string PACKAGE_NUMBER_EDIT { get; set; }
        public long? EXPIRED_DATE_EDIT { get; set; }
        public decimal? TEMPERATURE_OLD { get; set; }

        public ImpMestMedicineSDODetail(V_HIS_IMP_MEST_MEDICINE _data)
        {
            try
            {
                if (_data != null)
                {

                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_IMP_MEST_MEDICINE>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }


                    if (this.TDL_IMP_UNIT_ID.HasValue)
                    {
                        this.AMOUNT = this.IMP_UNIT_AMOUNT ?? 0;
                        this.PRICE = this.IMP_UNIT_PRICE ?? 0;
                        this.SERVICE_UNIT_NAME = this.IMP_UNIT_NAME;
                        this.IMP_PRICE = this.IMP_UNIT_PRICE ?? 0;
                    }

                    MEDICINE_TYPE_ID_EDIT = _data.MEDICINE_TYPE_ID;
                    if (_data.VAT_RATIO != null)
                    {
                        VAT_RATIO_100 = Inventec.Common.Number.Get.RoundCurrency((_data.VAT_RATIO ?? 0) * 100, 2);
                        VAT_RATIO_100_OLD = VAT_RATIO_100;
                    }
                    else
                    {
                        VAT_RATIO_100_OLD = null;
                        VAT_RATIO_100 = null;
                    }
                    this.PRICE_OLD = _data.PRICE;

                    this.PACKAGE_NUMBER_EDIT = _data.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_EDIT = _data.EXPIRED_DATE;
                    if (_data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                                    && _data.DOCUMENT_PRICE.HasValue)
                    {
                        TOTAL_PRICE = _data.DOCUMENT_PRICE.Value;
                    }
                    else
                    {
                        decimal price = (_data.PRICE ?? 0);
                        decimal amount = _data.AMOUNT;
                        TOTAL_PRICE = price * amount * (1 + _data.VAT_RATIO ?? 0);
                    }

                    this.TOTAL_PRICE_OLD = this.TOTAL_PRICE;
                    this.TEMPERATURE_OLD = this.TEMPERATURE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeMedicineTypeId { get; set; }
        public string ErrorMessageMedicineTypeId { get; set; }
    }
}
