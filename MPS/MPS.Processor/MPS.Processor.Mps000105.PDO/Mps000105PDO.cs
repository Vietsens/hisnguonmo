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

namespace MPS.Processor.Mps000105.PDO
{
    public partial class Mps000105PDO : RDOBase
    {
        public string PatientCode = "";

        public Mps000105PDO() { }

        public Mps000105PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        {
            this._Transaction = transaction;
            //this._Bill = bill;
            this._ListSereServ = sereServs;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._ServiceReq = serviceReq;
        }

        //public Mps000105PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        //{
        //    this._Transaction = transaction;
        //    this._ListSereServ = sereServs;
        //    this._PatyAlterBhyt = patyAlterBhyt;
        //    this._ServiceReq = serviceReq;
        //}

        //public Mps000105PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        //{
        //    this._Transaction = transaction;
        //    this._ListSereServ = sereServs;
        //    this._PatyAlterBhyt = patyAlterBhyt;
        //    this._ServiceReq = serviceReq;
        //}

        public Mps000105PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_SERVICE_REQ serviceReq, HeinCardADO ado)
        {
            this._Transaction = transaction;
            this._ListSereServ = sereServs;

            this._HeinCard = ado;

            this._ServiceReq = serviceReq;
        }

        public Mps000105PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq, V_HIS_PATIENT _patient, string ratioText)
        {
            this._Transaction = transaction;
            //this._Bill = bill;
            this._ListSereServ = sereServs;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._ServiceReq = serviceReq;
            this.patient = _patient;
            this.ratioText = ratioText;
        }
        
    }
}
