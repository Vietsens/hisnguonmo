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

using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000316.ADO
{
    class MedicineTypeDetailADO : V_HIS_EXP_MEST_MEDICINE
    {
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE { get; set; }
        public long? INTRUCTION_TIME { get; set; }
        public long REQUEST_ROOM_ID { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public long CHECK_MEDI_STOCK { get; set; }
        public long CHECK_REQUEST_ROOM { get; set; }
        //public long? EXPMEST_NUM_ORDER { get; set; }
        //public string SERVICE_UNIT_CODE { get; set; }
        //public string SERVICE_UNIT_NAME { get; set; }

        public MedicineTypeDetailADO(List<HIS_SERVICE_REQ> listHisServiceReq, List<V_HIS_ROOM> VHisRooms, V_HIS_EXP_MEST_MEDICINE VHisExpMestMedicine, V_HIS_EXP_MEST_MATERIAL VHisExpMestMaterial, HIS_SERVICE_REQ_METY VHisServiceReqMety, HIS_SERVICE_REQ_MATY VHisServiceReqMaty)
        {
            if (listHisServiceReq != null && listHisServiceReq.Count > 0)
            {
                if (VHisExpMestMedicine != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeDetailADO>(this, VHisExpMestMedicine);
                    this.MEDI_MATE_TYPE_CODE = VHisExpMestMedicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = VHisExpMestMedicine.MEDICINE_TYPE_NAME;
                    this.MEDI_MATE_TYPE = "V_HIS_EXP_MEST_MEDICINE";
                    //this.TUTORIAL = VHisExpMestMedicine.TUTORIAL;
                    //this.NUM_ORDER = VHisExpMestMedicine.NUM_ORDER;
                    //this.SERVICE_UNIT_CODE = VHisExpMestMedicine.SERVICE_UNIT_CODE;
                    //this.SERVICE_UNIT_NAME = VHisExpMestMedicine.SERVICE_UNIT_NAME;
                    var serviceReq = listHisServiceReq.FirstOrDefault(o => o.ID == VHisExpMestMedicine.TDL_SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        this.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                        if (VHisRooms != null && VHisRooms.Count > 0)
                        {
                            var room = VHisRooms.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                this.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }
                        this.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                        this.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                    }
                }

                else if (VHisExpMestMaterial != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeDetailADO>(this, VHisExpMestMaterial);
                    this.MEDI_MATE_TYPE_CODE = VHisExpMestMaterial.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = VHisExpMestMaterial.MATERIAL_TYPE_NAME;
                    this.MEDI_MATE_TYPE = "V_HIS_EXP_MEST_MATERIAL";
                    //this.TUTORIAL = VHisExpMestMaterial.TUTORIAL;
                    //this.NUM_ORDER = VHisExpMestMaterial.NUM_ORDER;
                    //this.SERVICE_UNIT_CODE = VHisExpMestMaterial.SERVICE_UNIT_CODE;
                    //this.SERVICE_UNIT_NAME = VHisExpMestMaterial.SERVICE_UNIT_NAME;
                    var serviceReq = listHisServiceReq.FirstOrDefault(o => o.ID == VHisExpMestMaterial.TDL_SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        this.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                        if (VHisRooms != null && VHisRooms.Count > 0)
                        {
                            var room = VHisRooms.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                this.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }
                        this.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                        this.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                    }
                }

                else if (VHisServiceReqMety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeDetailADO>(this, VHisServiceReqMety);
                    this.MEDI_MATE_TYPE_CODE = null;
                    this.MEDI_MATE_TYPE_NAME = VHisServiceReqMety.MEDICINE_TYPE_NAME;
                    this.MEDI_MATE_TYPE = "HIS_SERVICE_REQ_METY";
                    //this.TUTORIAL = VHisServiceReqMety.TUTORIAL;
                    //this.NUM_ORDER = VHisServiceReqMety.NUM_ORDER;
                    this.SERVICE_UNIT_CODE = null;
                    this.SERVICE_UNIT_NAME = VHisServiceReqMety.UNIT_NAME;
                    var serviceReq = listHisServiceReq.FirstOrDefault(o => o.ID == VHisServiceReqMety.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        this.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                        if (VHisRooms != null && VHisRooms.Count > 0)
                        {
                            var room = VHisRooms.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                this.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }
                        this.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                        this.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                    }
                }

                else if (VHisServiceReqMaty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeDetailADO>(this, VHisServiceReqMaty);
                    this.MEDI_MATE_TYPE_CODE = null;
                    this.MEDI_MATE_TYPE_NAME = VHisServiceReqMaty.MATERIAL_TYPE_NAME;
                    this.MEDI_MATE_TYPE = "HIS_SERVICE_REQ_MATY";
                    //this.TUTORIAL = VHisServiceReqMaty.TUTORIAL;
                    //this.NUM_ORDER = VHisServiceReqMaty.NUM_ORDER;
                    this.SERVICE_UNIT_CODE = null;
                    this.SERVICE_UNIT_NAME = VHisServiceReqMaty.UNIT_NAME;
                    var serviceReq = listHisServiceReq.FirstOrDefault(o => o.ID == VHisServiceReqMaty.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        this.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                        if (VHisRooms != null && VHisRooms.Count > 0)
                        {
                            var room = VHisRooms.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                this.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }
                        this.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                        this.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                    }
                }
            }
        }
    }
}
