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

namespace HIS.UC.PeriousExpMestList.ADO
{
    public class PreServiceReqADO : V_HIS_SERVICE_REQ_7
    {
        public PreServiceReqADO()
            : base()
        { }

        public PreServiceReqADO CreatePreServiceReqADO(HIS_SERVICE_REQ_METY serReqmety, List<V_HIS_SERVICE_REQ_7> serviceReqs)
        {
            PreServiceReqADO result = new PreServiceReqADO();
            var serq7 = serviceReqs.Where(o => o.ID == serReqmety.SERVICE_REQ_ID).FirstOrDefault();

            result.ID_NODE = serq7.ID + ".SERVICE_REQ_METY" + serReqmety.ID;
            result.PARENT_ID_NODE = serq7.ID + ".";

            result.ID = serq7.ID;
            result.SERVICE_REQ_CODE = serq7.SERVICE_REQ_CODE;
            result.INTRUCTION_TIME = serq7.INTRUCTION_TIME;
            result.INTRUCTION_DATE = serq7.INTRUCTION_DATE;
            result.SESSION_CODE = serq7.SESSION_CODE;
            result.MEDI_STOCK_CODE = serq7.MEDI_STOCK_CODE;
            result.MEDI_STOCK_ID = serq7.MEDI_STOCK_ID;
            result.MEDI_STOCK_NAME = serq7.MEDI_STOCK_NAME;
            result.REQUEST_USERNAME = serq7.REQUEST_USERNAME;
            result.REQUEST_LOGINNAME = serq7.REQUEST_LOGINNAME;
            result.TREATMENT_ID = serq7.TREATMENT_ID;
            result.REQUEST_ROOM_ID = serq7.REQUEST_ROOM_ID;
            result.EXECUTE_LOGINNAME = serq7.EXECUTE_LOGINNAME;
            result.EXECUTE_ROOM_ID = serq7.EXECUTE_ROOM_ID;
            result.EXECUTE_USERNAME = serq7.EXECUTE_USERNAME;
            result.SERVICE_REQ_TYPE_ID = serq7.SERVICE_REQ_TYPE_ID;

            result.CODE = serReqmety.MEDICINE_TYPE_NAME;//TODO
            result.NAME = serReqmety.MEDICINE_TYPE_NAME;
            result.AMOUNT = serReqmety.AMOUNT;
            result.TUTORIAL = serReqmety.TUTORIAL;
            result.SERVICE_UNIT_NAME = serReqmety.UNIT_NAME;
            result.MEDICINE_TYPE_ID = serReqmety.MEDICINE_TYPE_ID;
            result.MEDICINE_USE_FORM_ID = serReqmety.MEDICINE_USE_FORM_ID;
            result.NUM_ORDER = serReqmety.NUM_ORDER;
            result.IS_PARENT_1 = false;

            return result;
        }

        public PreServiceReqADO CreatePreServiceReqADO(HIS_SERVICE_REQ_MATY serReqmaty, List<V_HIS_SERVICE_REQ_7> serviceReqs)
        {
            PreServiceReqADO result = new PreServiceReqADO();
            var serq7 = serviceReqs.Where(o => o.ID == serReqmaty.SERVICE_REQ_ID).FirstOrDefault();

            result.ID_NODE = serq7.ID + ".SERVICE_REQ_MATY" + serReqmaty.ID;
            result.PARENT_ID_NODE = serq7.ID + ".";

            result.ID = serq7.ID;
            result.SERVICE_REQ_CODE = serq7.SERVICE_REQ_CODE;
            result.INTRUCTION_TIME = serq7.INTRUCTION_TIME;
            result.INTRUCTION_DATE = serq7.INTRUCTION_DATE;
            result.SESSION_CODE = serq7.SESSION_CODE;
            result.MEDI_STOCK_CODE = serq7.MEDI_STOCK_CODE;
            result.MEDI_STOCK_ID = serq7.MEDI_STOCK_ID;
            result.MEDI_STOCK_NAME = serq7.MEDI_STOCK_NAME;
            result.REQUEST_USERNAME = serq7.REQUEST_USERNAME;
            result.REQUEST_LOGINNAME = serq7.REQUEST_LOGINNAME;
            result.TREATMENT_ID = serq7.TREATMENT_ID;
            result.REQUEST_ROOM_ID = serq7.REQUEST_ROOM_ID;
            result.EXECUTE_LOGINNAME = serq7.EXECUTE_LOGINNAME;
            result.EXECUTE_ROOM_ID = serq7.EXECUTE_ROOM_ID;
            result.EXECUTE_USERNAME = serq7.EXECUTE_USERNAME;
            result.SERVICE_REQ_TYPE_ID = serq7.SERVICE_REQ_TYPE_ID;

            result.CODE = serReqmaty.MATERIAL_TYPE_NAME;//TODO
            result.NAME = serReqmaty.MATERIAL_TYPE_NAME;
            result.AMOUNT = serReqmaty.AMOUNT;
            result.SERVICE_UNIT_NAME = serReqmaty.UNIT_NAME;
            result.MEDICINE_TYPE_ID = serReqmaty.MATERIAL_TYPE_ID;
            result.NUM_ORDER = serReqmaty.NUM_ORDER;
            result.IS_PARENT_1 = false;

            return result;
        }

        public PreServiceReqADO CreatePreServiceReqADO(V_HIS_EXP_MEST_MEDICINE data, List<V_HIS_SERVICE_REQ_7> serviceReqs)
        {
            PreServiceReqADO result = new PreServiceReqADO();
            var serq7 = serviceReqs.Where(o => o.ID == data.TDL_SERVICE_REQ_ID).FirstOrDefault();

            result.ID_NODE = serq7.ID + ".EXP_MEST_MEDICINE" + data.ID + "." + data.MEDICINE_TYPE_CODE;
            result.PARENT_ID_NODE = serq7.ID + ".";

            result.ID = serq7.ID;
            result.SERVICE_REQ_CODE = serq7.SERVICE_REQ_CODE;
            result.INTRUCTION_TIME = serq7.INTRUCTION_TIME;
            result.INTRUCTION_DATE = serq7.INTRUCTION_DATE;
            result.SESSION_CODE = serq7.SESSION_CODE;
            result.MEDI_STOCK_CODE = serq7.MEDI_STOCK_CODE;
            result.MEDI_STOCK_ID = serq7.MEDI_STOCK_ID;
            result.MEDI_STOCK_NAME = serq7.MEDI_STOCK_NAME;
            result.REQUEST_USERNAME = serq7.REQUEST_USERNAME;
            result.REQUEST_LOGINNAME = serq7.REQUEST_LOGINNAME;
            result.TREATMENT_ID = serq7.TREATMENT_ID;
            result.REQUEST_ROOM_ID = serq7.REQUEST_ROOM_ID;
            result.EXECUTE_LOGINNAME = serq7.EXECUTE_LOGINNAME;
            result.EXECUTE_ROOM_ID = serq7.EXECUTE_ROOM_ID;
            result.EXECUTE_USERNAME = serq7.EXECUTE_USERNAME;
            result.SERVICE_REQ_TYPE_ID = serq7.SERVICE_REQ_TYPE_ID;

            result.CODE = data.MEDICINE_TYPE_CODE;
            result.NAME = data.MEDICINE_TYPE_NAME;
            result.AMOUNT = data.AMOUNT;
            result.TUTORIAL = data.TUTORIAL;
            result.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
            result.MEDICINE_TYPE_ID = data.MEDICINE_TYPE_ID;
            result.NUM_ORDER = data.NUM_ORDER;
            result.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
            result.TYPE = 1;
            result.IS_PARENT_1 = false;

            return result;
        }

        public PreServiceReqADO CreatePreServiceReqADO(V_HIS_EXP_MEST_MATERIAL data, List<V_HIS_SERVICE_REQ_7> serviceReqs)            
        {
            PreServiceReqADO result = new PreServiceReqADO();
            var serq7 = serviceReqs.Where(o => o.ID == data.TDL_SERVICE_REQ_ID).FirstOrDefault();

            result.ID_NODE = serq7.ID + ".EXP_MEST_MATERIAL" + data.ID + "." + data.MATERIAL_TYPE_CODE;
            result.PARENT_ID_NODE = serq7.ID + ".";

            result.ID = serq7.ID;
            result.SERVICE_REQ_CODE = serq7.SERVICE_REQ_CODE;
            result.INTRUCTION_TIME = serq7.INTRUCTION_TIME;
            result.INTRUCTION_DATE = serq7.INTRUCTION_DATE;
            result.SESSION_CODE = serq7.SESSION_CODE;
            result.MEDI_STOCK_CODE = serq7.MEDI_STOCK_CODE;
            result.MEDI_STOCK_ID = serq7.MEDI_STOCK_ID;
            result.MEDI_STOCK_NAME = serq7.MEDI_STOCK_NAME;
            result.REQUEST_USERNAME = serq7.REQUEST_USERNAME;
            result.REQUEST_LOGINNAME = serq7.REQUEST_LOGINNAME;
            result.TREATMENT_ID = serq7.TREATMENT_ID;
            result.REQUEST_ROOM_ID = serq7.REQUEST_ROOM_ID;
            result.EXECUTE_LOGINNAME = serq7.EXECUTE_LOGINNAME;
            result.EXECUTE_ROOM_ID = serq7.EXECUTE_ROOM_ID;
            result.EXECUTE_USERNAME = serq7.EXECUTE_USERNAME;
            result.SERVICE_REQ_TYPE_ID = serq7.SERVICE_REQ_TYPE_ID;

            result.CODE = data.MATERIAL_TYPE_CODE;
            result.NAME = data.MATERIAL_TYPE_NAME;
            result.AMOUNT = data.AMOUNT;
            result.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
            result.MEDICINE_TYPE_ID = data.MATERIAL_TYPE_ID;
            result.NUM_ORDER = data.NUM_ORDER;
            result.TYPE = 2;
            result.IS_PARENT_1 = false;

            return result;
        }

        public string ID_NODE { get; set; }
        public string PARENT_ID_NODE { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public long TYPE { get; set; }// 1 thuốc - 2 vật tư - 3 máu       
        public decimal AMOUNT { get; set; }
        public string TUTORIAL { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long? MEDICINE_USE_FORM_ID { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }

        public bool IS_PARENT_1 { get; set; }
    }
}
