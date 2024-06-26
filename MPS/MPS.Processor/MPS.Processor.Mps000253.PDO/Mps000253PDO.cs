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

namespace MPS.Processor.Mps000253.PDO
{
    public partial class Mps000253PDO : RDOBase
    {
        public List<V_HIS_ALLERGENIC> allergenic = null;
        public V_HIS_PATIENT patient = null;

        public Mps000253PDO(
            V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient,
            V_HIS_ALLERGY_CARD _allergyCard,
            Mps000253ADO _mps000253Ado,
            List<V_HIS_ALLERGENIC> _allergenic
            )
        {
            try
            {
                this.mps000253ADO = _mps000253Ado;
                this.treatment = _treatment;
                this.allergyCard = _allergyCard;
                this.allergenic = _allergenic;
                this.patient = _patient;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
