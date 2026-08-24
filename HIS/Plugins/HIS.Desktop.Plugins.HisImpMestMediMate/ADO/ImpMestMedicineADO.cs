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
    class ImpMestMedicineADO : ImpMediMateBaseADO
    {
        /// <summary>Id lo thuoc - dung de tra Nguon nhap</summary>
        public long MEDICINE_ID { get; set; }

        /// <summary>Ma thong tu</summary>
        public string BYT_NUM_ORDER { get; set; }

        /// <summary>Hoat chat</summary>
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public ImpMestMedicineADO() { }

        public ImpMestMedicineADO(V_HIS_IMP_MEST_MEDICINE data)
        {
            try
            {
                this.IMP_MEST_ID = data.IMP_MEST_ID;
                this.IMP_MEST_CODE = data.IMP_MEST_CODE;
                this.IMP_TIME = data.IMP_TIME;
                this.DOCUMENT_NUMBER = data.DOCUMENT_NUMBER;
                this.MEDICINE_ID = data.MEDICINE_ID;
                this.TYPE_CODE = data.MEDICINE_TYPE_CODE;
                this.TYPE_NAME = BuildDisplayName(data.MEDICINE_TYPE_NAME, data.CONCENTRA);
                this.BYT_NUM_ORDER = !string.IsNullOrWhiteSpace(data.MEDICINE_BYT_NUM_ORDER)
                    ? data.MEDICINE_BYT_NUM_ORDER
                    : data.BYT_NUM_ORDER;
                this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.AMOUNT = data.AMOUNT;
                this.SUPPLIER_NAME = data.SUPPLIER_NAME;
                this.MEDI_STOCK_ID = data.MEDI_STOCK_ID;
                this.MANUFACTURER_NAME = !string.IsNullOrWhiteSpace(data.MEDICINE_MANUFACTURER_NAME)
                    ? data.MEDICINE_MANUFACTURER_NAME
                    : data.MANUFACTURER_NAME;
                this.NATIONAL_NAME = data.NATIONAL_NAME;
                this.IMP_PRICE = data.IMP_PRICE;
                this.IMP_VAT_RATIO = data.IMP_VAT_RATIO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Goi sau khi da ghep xong ten kho / nguon nhap</summary>
        internal void RebuildKeyWord()
        {
            BuildKeyWord(
                this.IMP_MEST_CODE,
                this.DOCUMENT_NUMBER,
                this.BYT_NUM_ORDER,
                this.TYPE_CODE,
                this.TYPE_NAME,
                this.ACTIVE_INGR_BHYT_NAME,
                this.SERVICE_UNIT_NAME,
                this.SUPPLIER_NAME,
                this.MEDI_STOCK_NAME,
                this.IMP_SOURCE_NAME,
                this.MANUFACTURER_NAME,
                this.NATIONAL_NAME
            );
        }
    }
}
