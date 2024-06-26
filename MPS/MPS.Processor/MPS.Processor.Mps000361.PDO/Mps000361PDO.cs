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

namespace MPS.Processor.Mps000361.PDO
{
    public class Mps000361PDO : RDOBase
    {
        public V_HIS_TREATMENT_FEE _Treatment;
        //public V_HIS_TRANSACTION _transaction;
        public V_HIS_DEPARTMENT _department;
        public List<V_HIS_TRANSACTION> ListTransaction;

        public Mps000361PDO(V_HIS_TREATMENT_FEE treatment, List<V_HIS_TRANSACTION> transactions, V_HIS_DEPARTMENT department)
        {
            this._Treatment = treatment;
            this.ListTransaction = transactions;
            this._department = department;
        }
    }
}
