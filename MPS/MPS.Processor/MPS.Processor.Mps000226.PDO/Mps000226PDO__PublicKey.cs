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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000226.PDO
{
    public partial class Mps000226PDO : RDOBase
    {
        public V_HIS_IMP_MEST _ChmsImpMest = null;
        public List<V_HIS_IMP_MEST_BLOOD> _ImpMestBloods = null;
        public List<Mps000226ADO> _ListAdo = null;
        public Mps000226Key _mps000143Key = null;
        public int NumberDigit;

        public class Mps000226Key
        {
            public string EXP_MEDI_STOCK_NAME { get; set; }
            public string EXP_MEDI_STOCK_CODE { get; set; }
        }

        public class Mps000226ADO
        {
            public string SERVICE_UNIT_NAME { get; set; }
            public string PACKAGE_NUMBER { get; set; }
            public string EXPIRED_DATE_STR { get; set; }
            public string BID_NAME { get; set; }
            public string BID_NUMBER { get; set; }
            public string SUPPLIER_NAME { get; set; }
            public decimal? PRICE { get; set; }
            public string BLOOD_ABO_CODE { get; set; }
            public string BLOOD_RH_CODE { get; set; }
            public decimal VOLUME { get; set; }
            public string BLOOD_CODE { get; set; }
            public string BLOOD_TYPE_CODE { get; set; }
            public string BLOOD_TYPE_NAME { get; set; }

            public Mps000226ADO(V_HIS_IMP_MEST_BLOOD blood)
            {
                try
                {
                    if (blood != null)
                    {
                        //this.MEDI_MATE_TYPE_NAME = blood.BLOOD_CODE;
                        this.SERVICE_UNIT_NAME = blood.SERVICE_UNIT_NAME;
                        //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                        this.PACKAGE_NUMBER = blood.PACKAGE_NUMBER;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(blood.EXPIRED_DATE ?? 0);
                        this.PRICE = blood.PRICE;
                        this.BID_NAME = blood.BID_NAME;
                        this.BID_NUMBER = blood.BID_NUMBER;
                        this.SUPPLIER_NAME = blood.SUPPLIER_NAME;
                        this.BLOOD_ABO_CODE = blood.BLOOD_ABO_CODE;
                        this.BLOOD_RH_CODE = blood.BLOOD_RH_CODE;
                        this.VOLUME = blood.VOLUME;
                        this.BLOOD_TYPE_CODE = blood.BLOOD_TYPE_CODE;
                        this.BLOOD_TYPE_NAME = blood.BLOOD_TYPE_NAME;
                        this.BLOOD_CODE = blood.BLOOD_CODE;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }
    }
}
