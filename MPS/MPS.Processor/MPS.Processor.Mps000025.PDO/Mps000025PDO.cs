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
using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000025.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000025PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public HIS_TREATMENT treatment { get; set; }
        public List<HIS_CARD> ListCard { get; set; }


        public Mps000025PDO() { }

        public Mps000025PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlter
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000025PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            HIS_TREATMENT _Treatment
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.treatment = _Treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000025PDO(
             List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<V_HIS_SERE_SERV> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            string patientTypeName
           )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.patientTypeName = patientTypeName;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000025PDO(
             List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<V_HIS_SERE_SERV> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            string patientTypeName,
            HIS_TREATMENT _Treatment
           )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.patientTypeName = patientTypeName;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.treatment = _Treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000025PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            List<HIS_CARD> _listCard
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.ListCard = _listCard;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000025PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            HIS_TREATMENT _Treatment,
            List<HIS_CARD> _listCard
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.treatment = _Treatment;
                this.ListCard = _listCard;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000025PDO(
             List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<V_HIS_SERE_SERV> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            string patientTypeName,
            List<HIS_CARD> _listCard
           )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.patientTypeName = patientTypeName;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.ListCard = _listCard;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000025PDO(
             List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<V_HIS_SERE_SERV> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlter,
            string patientTypeName,
            HIS_TREATMENT _Treatment,
            List<HIS_CARD> _listCard
           )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlter = PatyAlter;
                this.patientTypeName = patientTypeName;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.treatment = _Treatment;
                this.ListCard = _listCard;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
