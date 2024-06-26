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

namespace HIS.Desktop.Plugins.HisImportMediStock.ADO
{
    public class MediStockImportADO : V_HIS_MEDI_STOCK
    {
        public string AUTO_APPROVE_EXPORT { get; set; }
        public string AUTO_EXECUTE_EXPORT { get; set; }
        public string AUTO_APPROVE_IMPORT { get; set; }
        public string AUTO_EXECUTE_IMPORT { get; set; }

        public string IS_ALLOW_IMP_SUPPLIER_STR { get; set; }
        public string IS_AUTO_CREATE_CHMS_IMP_STR { get; set; }
        public string IS_BLOOD_STR { get; set; }
        public string IS_NEW_MEDICINE_STR { get; set; }
        public string IS_TRADITIONAL_MEDICINE_STR { get; set; }
        public string IS_BUSINESS_STR { get; set; }
        public string IS_CABINET_STR { get; set; }
        public string IS_GOODS_RESTRICT_STR { get; set; }
        public string IS_ODD_STR { get; set; }
        public string IS_SHOW_DDT_STR { get; set; }

        public bool ALLOW_IMP_SUPPLIER { get; set; }
        public bool AUTO_CREATE_CHMS_IMP { get; set; }
        public bool BLOOD { get; set; }
        public bool NEW_MEDICINE { get; set; }
        public bool TRADITIONAL_MEDICINE { get; set; }
        public bool CABINET { get; set; }
        public bool GOODS_RESTRICT { get; set; }
        public bool BUSINESS { get; set; }
        public bool ODD { get; set; }
        public bool SHOW_DDT { get; set; }

        public Dictionary<long, HIS_MEDI_STOCK_EXTY> dicMediStockExty { get; set; }
        public Dictionary<long, HIS_MEDI_STOCK_IMTY> dicMediStockImty { get; set; }

        public string ERROR { get; set; }

        public MediStockImportADO()
        {
        }

        public MediStockImportADO(V_HIS_MEDI_STOCK data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MediStockImportADO>(this, data);
        }
    }
}
