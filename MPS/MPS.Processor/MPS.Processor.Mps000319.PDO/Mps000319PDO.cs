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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000319.PDO
{
    public class Mps000319PDO: RDOBase
    {
        public HIS_DISPENSE DisPense;
        public HIS_MEDI_STOCK Medistocks;
        public List<V_HIS_IMP_MEST_MATERIAL> ImpMestMaterials = null;
        public List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials = null;

        public List<Mps000319ADO> listAdo = new List<Mps000319ADO>();

        public Mps000319PDO(HIS_DISPENSE _DisPense, HIS_MEDI_STOCK _Medistock, List<V_HIS_IMP_MEST_MATERIAL> _impMaterials, List<V_HIS_EXP_MEST_MATERIAL> _listMaterial)
        {
            this.DisPense = _DisPense;
            this.ExpMestMaterials = _listMaterial;
            this.ImpMestMaterials = _impMaterials;
            this.Medistocks = _Medistock;
        }
    }

    public class Mps000319ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public decimal? TP_AMOUNT { get; set; }
        public decimal? NL_AMOUNT { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string BID_NUMBER { get; set; }
        public long? IMP_EXP_TIME { get; set; }
        public decimal? IMP_EXP_PRICE { get; set; }
        public string BID_NAME { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public long MEDI_MATE_NUM_ORDER { get; set; }

        public long? NUM_ORDER { get; set; }

        public Mps000319ADO()
        {
        }

        public Mps000319ADO(List<V_HIS_IMP_MEST_MATERIAL> listMaterial)
        {
            if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 2;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.TP_AMOUNT = listMaterial.First().AMOUNT;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.IMP_EXP_TIME = listMaterial.First().IMP_TIME;
                    this.IMP_EXP_PRICE = listMaterial.First().PRICE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.BID_NAME = listMaterial.First().BID_NAME;
                    this.BID_NUMBER = listMaterial.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    this.MEDI_MATE_NUM_ORDER = listMaterial.First().MEDICINE_NUM_ORDER ?? 0;
                }
        }

        public Mps000319ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 2;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.BID_NAME = listMaterial.First().BID_NAME;
                    this.IMP_EXP_PRICE = listMaterial.First().PRICE;
                    this.IMP_EXP_TIME = listMaterial.First().EXP_TIME;
                    this.BID_NUMBER = listMaterial.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXPIRED_DATE ?? 0);
                    this.NL_AMOUNT = listMaterial.Sum(o => o.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    this.MEDI_MATE_NUM_ORDER = listMaterial.First().MATERIAL_NUM_ORDER ?? 0;
                }
        }
    }
}
