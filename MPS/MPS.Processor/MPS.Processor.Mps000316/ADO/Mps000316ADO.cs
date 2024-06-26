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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000316.ADO
{
    class Mps000316ADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        public string START_TIME_STR { get; set; }
        public string FINISH_TIME_STR { get; set; }

        public string TEST_INDEX_UNIT_NAME { get; set; }
        public string TEST_INDEX_RANGE { get; set; }
        public string VALUE_STR { get; set; }
        public string VALUE_RANGE { get; set; }
        public string RESULT_CODE { get; set; }
        public string HIGH_OR_LOW { get; set; }
        public decimal? MIN_VALUE { get; set; }
        public decimal? MAX_VALUE { get; set; }
        public string IMPORTANT { get; set; }

        public string CONCLUDE { get; set; }
        public string DESCRIPTION { get; set; }
        public string NOTE { get; set; }
        public long? NUMBER_OF_FILM { get; set; }
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string BEGIN_TIME_STR { get; set; }
        public string END_TIME_STR { get; set; }

        public string TUTORIAL { get; set; }

        public long SERVICE_NUM_ODER { get; set; }

        public Mps000316ADO() { }

        public Mps000316ADO(MOS.EFMODEL.DataModels.HIS_SERE_SERV data, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> listService)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000316ADO>(this, data);
                
                var serv = listService.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                if (serv != null)
                {
                    this.SERVICE_NUM_ODER = serv.NUM_ORDER ?? 9999;
                    this.SERVICE_TYPE_CODE = serv.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serv.SERVICE_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = serv.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = serv.SERVICE_UNIT_NAME;
                    this.TDL_HEIN_SERVICE_BHYT_CODE = serv.HEIN_SERVICE_BHYT_CODE;
                    this.TDL_HEIN_SERVICE_BHYT_NAME = serv.HEIN_SERVICE_BHYT_NAME;
                    this.TDL_HEIN_SERVICE_TYPE_ID = serv.HEIN_SERVICE_TYPE_ID;
                    //this.TUTORIAL = 
                }
            }
        }

        public Mps000316ADO(MOS.EFMODEL.DataModels.HIS_SERE_SERV data, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> listService, List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE> listMedi, List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL> listMate)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000316ADO>(this, data);
                var serv = listService.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                var medi = new MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE();
                var mate = new MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL();     
                if (serv != null)
                {
                    this.SERVICE_NUM_ODER = serv.NUM_ORDER ?? 9999;
                    this.SERVICE_TYPE_CODE = serv.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serv.SERVICE_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = serv.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = serv.SERVICE_UNIT_NAME;
                    this.TDL_HEIN_SERVICE_BHYT_CODE = serv.HEIN_SERVICE_BHYT_CODE;
                    this.TDL_HEIN_SERVICE_BHYT_NAME = serv.HEIN_SERVICE_BHYT_NAME;
                    this.TDL_HEIN_SERVICE_TYPE_ID = serv.HEIN_SERVICE_TYPE_ID;
                    if (data.EXP_MEST_MEDICINE_ID != null)
                    {
                        medi = listMedi.FirstOrDefault(o => o.ID == data.EXP_MEST_MEDICINE_ID);
                        this.TUTORIAL = medi.TUTORIAL;
                    }
                    if (data.EXP_MEST_MATERIAL_ID != null)
                    {
                        mate = listMate.FirstOrDefault(o => o.ID == data.EXP_MEST_MATERIAL_ID);
                        this.TUTORIAL = mate.TUTORIAL;
                    }
                }
            }
        }
    }
}
