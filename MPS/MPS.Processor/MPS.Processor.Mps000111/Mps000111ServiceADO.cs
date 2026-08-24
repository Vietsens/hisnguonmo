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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000111
{
    /// <summary>
    /// Mot dong dich vu tren phieu thu thanh toan Mps000111.
    /// Ten cac key trung voi Mps000112 de bieu mau 2 phieu dung chung cach viet tag.
    /// </summary>
    public class Mps000111ServiceADO
    {
        /// <summary>
        /// So thu tu dong
        /// </summary>
        public long NUM_ORDER { get; set; }

        /// <summary>
        /// Ma dich vu
        /// </summary>
        public string SERVICE_CODE { get; set; }

        /// <summary>
        /// Ten dich vu
        /// </summary>
        public string SERVICE_NAME { get; set; }

        /// <summary>
        /// Don vi tinh
        /// </summary>
        public string SERVICE_UNIT_NAME { get; set; }

        /// <summary>
        /// So luong
        /// </summary>
        public decimal AMOUNT { get; set; }

        /// <summary>
        /// Ma loai dich vu
        /// </summary>
        public string SERVICE_TYPE_CODE { get; set; }

        /// <summary>
        /// Loai dich vu
        /// </summary>
        public string SERVICE_TYPE_NAME { get; set; }

        /// <summary>
        /// Don gia
        /// </summary>
        public decimal PRICE { get; set; }

        /// <summary>
        /// Thanh tien
        /// </summary>
        public decimal TOTAL_PRICE { get; set; }

        /// <summary>
        /// Ty le VAT (dang he so, vd 0.05)
        /// </summary>
        public decimal VAT_RATIO { get; set; }

        /// <summary>
        /// Ty le VAT dang phan tram, vd "5%"
        /// </summary>
        public string VAT_RATIO_STR { get; set; }

        /// <summary>
        /// Tien BHYT thanh toan (BHTT)
        /// </summary>
        public decimal TOTAL_HEIN_PRICE { get; set; }

        /// <summary>
        /// Tien benh nhan cung chi tra (BNCCT)
        /// </summary>
        public decimal TOTAL_PATIENT_PRICE_BHYT { get; set; }

        /// <summary>
        /// Tien benh nhan thanh toan (BNTT)
        /// </summary>
        public decimal TOTAL_PATIENT_PRICE { get; set; }

        /// <summary>
        /// So tien da tam ung cho dich vu nay
        /// </summary>
        public decimal DEPOSIT_AMOUNT { get; set; }

        /// <summary>
        /// Ten doi tuong (BHYT / Vien phi ...)
        /// </summary>
        public string PATIENT_TYPE_NAME { get; set; }

        /// <summary>
        /// Tien chiet khau cua dich vu
        /// </summary>
        public decimal DISCOUNT { get; set; }

        /// <summary>
        /// Ly do chiet khau cua dich vu
        /// </summary>
        public string DISCOUNT_REASON { get; set; }

        public Mps000111ServiceADO() { }

        public Mps000111ServiceADO(HIS_SERE_SERV data)
        {
            try
            {
                if (data == null) return;

                this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                this.AMOUNT = data.AMOUNT;
                this.PRICE = data.VIR_PRICE ?? 0;
                this.TOTAL_PRICE = data.VIR_TOTAL_PRICE ?? 0;
                this.VAT_RATIO = data.VAT_RATIO;
                this.VAT_RATIO_STR = (data.VAT_RATIO * 100) + "%";
                this.TOTAL_HEIN_PRICE = data.VIR_TOTAL_HEIN_PRICE ?? 0;
                this.TOTAL_PATIENT_PRICE_BHYT = data.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0;
                this.TOTAL_PATIENT_PRICE = data.VIR_TOTAL_PATIENT_PRICE ?? 0;
                this.DISCOUNT = data.DISCOUNT ?? 0;
                this.DISCOUNT_REASON = data.DISCOUNT_REASON;

                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_TYPE_ID);
                if (serviceType != null)
                {
                    this.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                }

                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                if (serviceUnit != null)
                {
                    this.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                }

                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.PATIENT_TYPE_ID);
                if (patientType != null)
                {
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
