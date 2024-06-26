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

namespace MPS.Processor.Mps000276.PDO
{
    public partial class Mps000276PDO : RDOBase
    {
        public Mps000276PDO(
            List<V_HIS_SERVICE_REQ> _vserviceReqs,
            HIS_TREATMENT _treatment,
            V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter,
            V_HIS_ROOM _vhisRoom)
        {
            this._vServiceReqs = _vserviceReqs;
            this._Treatment = _treatment;
            this._PatientTypeAlter = _patientTypeAlter;
            this._vHisRoom = _vhisRoom;
        }

        public Mps000276PDO(
            List<V_HIS_SERVICE_REQ> _vserviceReqs,
            HIS_TREATMENT _treatment,
            V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter,
            V_HIS_ROOM _vhisRoom,
            List<HIS_SERE_SERV> sereServs,
            List<V_HIS_SERVICE> services,
            List<V_HIS_ROOM> rooms,
            List<V_HIS_CASHIER_ROOM> cashierRooms,
             List<HIS_SERVICE_NUM_ORDER> serviceNumOrders,
            List<V_HIS_DESK> desks)
        {
            this._vServiceReqs = _vserviceReqs;
            this._Treatment = _treatment;
            this._PatientTypeAlter = _patientTypeAlter;
            this._vHisRoom = _vhisRoom;
            this._SereServs = sereServs;
            this._Services = services;
            this._Rooms = rooms;
            this._CashierRooms = cashierRooms;
            this._ServiceNumOrder = serviceNumOrders;
            this._Desks = desks;
        }
        public Mps000276PDO(
    List<V_HIS_SERVICE_REQ> _vserviceReqs,
    HIS_TREATMENT _treatment,
    V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter,
    V_HIS_ROOM _vhisRoom,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq)
        {
            this.lstConfig = lstConfig;
            this.transReq = transReq;
            this._vServiceReqs = _vserviceReqs;
            this._Treatment = _treatment;
            this._PatientTypeAlter = _patientTypeAlter;
            this._vHisRoom = _vhisRoom;
        }

        public Mps000276PDO(
            List<V_HIS_SERVICE_REQ> _vserviceReqs,
            HIS_TREATMENT _treatment,
            V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter,
            V_HIS_ROOM _vhisRoom,
            List<HIS_SERE_SERV> sereServs,
            List<V_HIS_SERVICE> services,
            List<V_HIS_ROOM> rooms,
            List<V_HIS_CASHIER_ROOM> cashierRooms,
             List<HIS_SERVICE_NUM_ORDER> serviceNumOrders,
            List<V_HIS_DESK> desks,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq)
        {
            this.lstConfig = lstConfig;
            this.transReq = transReq;
            this._vServiceReqs = _vserviceReqs;
            this._Treatment = _treatment;
            this._PatientTypeAlter = _patientTypeAlter;
            this._vHisRoom = _vhisRoom;
            this._SereServs = sereServs;
            this._Services = services;
            this._Rooms = rooms;
            this._CashierRooms = cashierRooms;
            this._ServiceNumOrder = serviceNumOrders;
            this._Desks = desks;
        }
    }
}
