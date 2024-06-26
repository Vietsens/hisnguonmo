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

namespace MPS.Processor.Mps000480.PDO
{
    public partial class Mps000480PDO : RDOBase
    {
        public HIS_EXP_MEST expMest { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> lstMedicine { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> lstMaterial { get; set; }
        public HIS_TREATMENT treatment { get; set; }

        public Mps000480PDO() { }

        public Mps000480PDO(
            HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MEDICINE> _lstMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> _lstMaterial
            )
        {
            try
            {
                this.expMest = _expMest;
                this.lstMedicine = _lstMedicine;
                this.lstMaterial = _lstMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000480PDO(
            HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MEDICINE> _lstMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> _lstMaterial,
            HIS_TREATMENT _treatment
        )
        {
            try
            {
                this.expMest = _expMest;
                this.lstMedicine = _lstMedicine;
                this.lstMaterial = _lstMaterial;
                this.treatment = _treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
