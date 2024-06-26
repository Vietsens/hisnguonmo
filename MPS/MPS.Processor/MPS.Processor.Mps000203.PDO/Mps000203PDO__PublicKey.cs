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

namespace MPS.Processor.Mps000203.PDO
{
    public partial class Mps000203PDO : RDOBase
    {
        public V_HIS_EXP_MEST _ExpMest;
        public Mps000203ADO _mps000203ADO;
    }


    public class Mps000203ADO
    {
        public string EXP_REASON_NAME { get; set; }
        public string IMP_MEDI_STOCK_NAME { get; set; }
        public string IMP_MEDI_STOCK_CODE { get; set; }
    }
    //public class Mps000198ADO
    //{
    //    public long TYPE_ID { get; set; }
    //    public long MEDI_MATE_TYPE_ID { get; set; }

    //    public string MEDI_MATE_TYPE_CODE { get; set; }
    //    public string MEDI_MATE_TYPE_NAME { get; set; }
    //    public string SERVICE_UNIT_CODE { get; set; }
    //    public string SERVICE_UNIT_NAME { get; set; }
    //    public decimal TOTAL_AMOUNT { get; set; }
    //    public decimal VOLUME { get; set; }
    //    public string PACKING_TYPE_NAME { get; set; }
    //    public string BLOOD_ADO_CODE { get; set; }
    //    public string BLOOD_RH_CODE { get; set; }
    //    public long? NUM_ORDER { get; set; }

    //    public Mps000198ADO()
    //    {
    //    }

    //    public Mps000198ADO(V_HIS_EXP_MEST_BLTY Blood)
    //    {
    //        try
    //        {
    //            if (Blood != null)
    //            {
    //                this.TYPE_ID = 1;
    //                this.MEDI_MATE_TYPE_CODE = Blood.BLOOD_TYPE_CODE;
    //                this.MEDI_MATE_TYPE_ID = Blood.BLOOD_TYPE_ID;
    //                this.MEDI_MATE_TYPE_NAME = Blood.BLOOD_TYPE_NAME;
    //                this.SERVICE_UNIT_CODE = Blood.SERVICE_UNIT_CODE;
    //                this.SERVICE_UNIT_NAME = Blood.SERVICE_UNIT_NAME;
    //                this.BLOOD_ADO_CODE = Blood.BLOOD_ABO_CODE;
    //                this.BLOOD_RH_CODE = Blood.BLOOD_RH_CODE;                    
    //                this.NUM_ORDER = Blood.NUM_ORDER;
    //                this.VOLUME = Blood.VOLUME;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Error(ex);
    //        }
    //    }        
    //}
}
