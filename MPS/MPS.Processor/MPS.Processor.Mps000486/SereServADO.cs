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

namespace MPS.Processor.Mps000486
{
    public class SereServADO : V_HIS_SERE_SERV_2
    {
        public short? IS_OUT_STOCK { get; set; }
        public long? MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string UNIT_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
        public decimal? SPEED{ get; set; }
        public long? DAY_COUNT{ get; set; }
        public string MORNING{ get; set; }
        public string NOON{ get; set; }
        public string AFTERNOON{ get; set; }
        public string EVENING{ get; set; }
        public long? HTU_ID{ get; set; }
        public short? IS_SUB_PRES{ get; set; }
        public decimal? PRES_AMOUNT{ get; set; }
        public string MANUFACTURER_CODE{ get; set; }
        public string MANUFACTURER_NAME{ get; set; }
        public string HEIN_SERVICE_TYPE_CODE{ get; set; }
        public string HEIN_SERVICE_TYPE_NAME{ get; set; }
        public string MEDICINE_LINE_CODE{ get; set; }
        public string MEDICINE_LINE_NAME{ get; set; }
        public string HEIN_SERVICE_BHYT_CODE{ get; set; }
        public string HEIN_SERVICE_BHYT_NAME{ get; set; }
        public decimal? VAT_RATIO { get; set; }

        public SereServADO(V_HIS_SERE_SERV_2 data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_2>(this, data);
            this.MEDI_MATE_TYPE_CODE = data.TDL_SERVICE_CODE;
            this.MEDI_MATE_TYPE_NAME = data.TDL_SERVICE_NAME;
            this.VAT_RATIO = data.VAT_RATIO;
        }
        public SereServADO(HIS_SERVICE_REQ_METY data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_2>(this, data);
            this.IS_OUT_STOCK = 1;
            this.SERVICE_REQ_ID = data.SERVICE_REQ_ID;
            this.MEDI_MATE_TYPE_ID = data.MEDICINE_TYPE_ID;
            this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
            this.AMOUNT = data.AMOUNT;
            this.SERVICE_UNIT_NAME = data.UNIT_NAME;
            this.NUM_ORDER = data.NUM_ORDER;
            this.TUTORIAL = data.TUTORIAL;
            this.MEDICINE_USE_FORM_ID = data.MEDICINE_USE_FORM_ID;
            this.USE_TIME_TO = data.USE_TIME_TO;
            this.PRICE = data.PRICE ?? 0;
            this.SPEED = data.SPEED;
            this.DAY_COUNT = data.DAY_COUNT;
            this.MORNING = data.MORNING;
            this.NOON = data.NOON;
            this.AFTERNOON = data.AFTERNOON;
            this.EVENING = data.EVENING;
            this.HTU_ID = data.HTU_ID;
            this.IS_SUB_PRES = data.IS_SUB_PRES;
            this.TDL_TREATMENT_ID = data.TDL_TREATMENT_ID;
            this.PRES_AMOUNT = data.PRES_AMOUNT;
        }
        public SereServADO(HIS_SERVICE_REQ_MATY data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_2>(this, data);
            this.IS_OUT_STOCK = 1;
            this.SERVICE_REQ_ID = data.SERVICE_REQ_ID;
            this.MEDI_MATE_TYPE_ID = data.MATERIAL_TYPE_ID;
            this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
            this.AMOUNT = data.AMOUNT;
            this.SERVICE_UNIT_NAME = data.UNIT_NAME;
            this.NUM_ORDER = data.NUM_ORDER;
            this.TUTORIAL = data.TUTORIAL;
            this.PRICE = data.PRICE ?? 0;
            this.IS_SUB_PRES = data.IS_SUB_PRES;
            this.TDL_TREATMENT_ID = data.TDL_TREATMENT_ID;
            this.PRES_AMOUNT = data.PRES_AMOUNT;
        }
    }
}
