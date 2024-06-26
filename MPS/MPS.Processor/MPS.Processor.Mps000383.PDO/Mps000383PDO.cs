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

namespace MPS.Processor.Mps000383.PDO
{
    public class Mps000383PDO : RDOBase
    {
        public List<HIS_SERE_SERV> ListSereServ;
        public List<HIS_TRANSACTION> ListTransaction;
        public List<HIS_SERE_SERV_BILL> ListSereServBill;
        public V_HIS_TREATMENT_FEE Treatment;
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter;
        public List<HIS_DEPARTMENT> Departments;
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes;

        public Mps000383PDO(List<HIS_SERE_SERV> listSereServ,
            List<HIS_TRANSACTION> listTransaction,
            List<HIS_SERE_SERV_BILL> listSereServBill,
            V_HIS_TREATMENT_FEE treatment,
            V_HIS_PATIENT_TYPE_ALTER currentPatyAlter,
            List<HIS_DEPARTMENT> departments,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes
            )
        {
            try
            {
                this.ListSereServ = listSereServ;
                this.ListTransaction = listTransaction;
                this.ListSereServBill = listSereServBill;
                this.Treatment = treatment;
                this.CurrentPatyAlter = currentPatyAlter;
                this.Departments = departments;
                this.HeinServiceTypes = _heinServiceTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
