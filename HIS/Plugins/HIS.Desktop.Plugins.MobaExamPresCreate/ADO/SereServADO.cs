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

namespace HIS.Desktop.Plugins.MobaExamPresCreate.ADO
{
    public class SereServADO
    {
        internal List<V_HIS_SERE_SERV_3> SereServs { get; set; }

        public bool IsCheck { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal TH_AMOUNT { get; set; }
        public decimal AVAI_AMOUNT { get; set; }
        public decimal EXP_AMOUNT { get; set; }
        public long? MEDICINE_ID { get; set; }
        public decimal TH_AMOUNT_OLD { get; set; }
        public long? MATERIAL_ID { get; set; }
        public bool IS_MEDICINE { get; set; }
        public decimal PRICE { get; set; }
        public decimal VAT_RATIO { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public long MATERIAL_TYPE_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public string PACKAGE_NUMBER { get; set; }

        public SereServADO(List<V_HIS_SERE_SERV_3> listSereServ, List<V_HIS_EXP_MEST_MEDICINE> medicines)
        {
            this.SereServs = listSereServ;
            this.SERVICE_CODE = listSereServ.FirstOrDefault().TDL_SERVICE_CODE;
            this.SERVICE_NAME = listSereServ.FirstOrDefault().TDL_SERVICE_NAME;
            this.SERVICE_UNIT_NAME = listSereServ.FirstOrDefault().SERVICE_UNIT_NAME;
            this.MEDICINE_ID = listSereServ.FirstOrDefault().MEDICINE_ID;
            this.MATERIAL_ID = listSereServ.FirstOrDefault().MATERIAL_ID;
            if (medicines != null && medicines.Count > 0)
            {
                this.TH_AMOUNT_OLD = medicines.Sum(s => s.TH_AMOUNT ?? 0);
                this.EXPIRED_DATE = medicines.FirstOrDefault().EXPIRED_DATE;
                this.MEDICINE_TYPE_ID = medicines.FirstOrDefault().MEDICINE_TYPE_ID;
                this.PACKAGE_NUMBER = medicines.FirstOrDefault().PACKAGE_NUMBER;
            }
            this.EXP_AMOUNT = listSereServ.Sum(s => s.AMOUNT);
            this.AVAI_AMOUNT = this.EXP_AMOUNT - this.TH_AMOUNT_OLD;
            this.PRICE = listSereServ.FirstOrDefault().PRICE;
            this.VAT_RATIO = listSereServ.FirstOrDefault().VAT_RATIO * 100;
            if (this.MEDICINE_ID.HasValue)
            {
                this.IMP_PRICE = listSereServ.FirstOrDefault().MEDICINE_IMP_PRICE ?? 0;
                this.IMP_VAT_RATIO = (listSereServ.FirstOrDefault().MEDICINE_IMP_VAT_RATIO ?? 0) * 100;
            }
            else
            {
                this.IMP_PRICE = listSereServ.FirstOrDefault().MATERIAL_IMP_PRICE ?? 0;
                this.IMP_VAT_RATIO = (listSereServ.FirstOrDefault().MATERIAL_IMP_VAT_RATIO ?? 0) * 100;
            }
            this.TOTAL_PRICE = this.EXP_AMOUNT * this.PRICE * (1 + (this.VAT_RATIO / (decimal)100));
        }

        public SereServADO(List<V_HIS_SERE_SERV_3> listSereServ, List<V_HIS_EXP_MEST_MATERIAL> materials)
        {
            this.SereServs = listSereServ;
            this.SERVICE_CODE = listSereServ.FirstOrDefault().TDL_SERVICE_CODE;
            this.SERVICE_NAME = listSereServ.FirstOrDefault().TDL_SERVICE_NAME;
            this.SERVICE_UNIT_NAME = listSereServ.FirstOrDefault().SERVICE_UNIT_NAME;
            this.MEDICINE_ID = listSereServ.FirstOrDefault().MEDICINE_ID;
            this.MATERIAL_ID = listSereServ.FirstOrDefault().MATERIAL_ID;
            if (materials != null && materials.Count > 0)
            {
                this.TH_AMOUNT_OLD = materials.Sum(s => s.TH_AMOUNT ?? 0);
                this.EXPIRED_DATE = materials.FirstOrDefault().EXPIRED_DATE;
                this.MATERIAL_TYPE_ID = materials.FirstOrDefault().MATERIAL_TYPE_ID;
                this.PACKAGE_NUMBER = materials.FirstOrDefault().PACKAGE_NUMBER;
            }
            this.EXP_AMOUNT = listSereServ.Sum(s => s.AMOUNT);
            this.AVAI_AMOUNT = this.EXP_AMOUNT - this.TH_AMOUNT_OLD;
            this.PRICE = listSereServ.FirstOrDefault().PRICE;
            this.VAT_RATIO = listSereServ.FirstOrDefault().VAT_RATIO * 100;
            if (this.MEDICINE_ID.HasValue)
            {
                this.IMP_PRICE = listSereServ.FirstOrDefault().MEDICINE_IMP_PRICE ?? 0;
                this.IMP_VAT_RATIO = (listSereServ.FirstOrDefault().MEDICINE_IMP_VAT_RATIO ?? 0) * 100;
            }
            else
            {
                this.IMP_PRICE = listSereServ.FirstOrDefault().MATERIAL_IMP_PRICE ?? 0;
                this.IMP_VAT_RATIO = (listSereServ.FirstOrDefault().MATERIAL_IMP_VAT_RATIO ?? 0) * 100;
            }
            this.TOTAL_PRICE = this.EXP_AMOUNT * this.PRICE * (1 + (this.VAT_RATIO / (decimal)100));
        }
    }
}
