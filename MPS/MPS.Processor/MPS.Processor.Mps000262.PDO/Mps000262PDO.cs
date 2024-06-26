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

namespace MPS.Processor.Mps000262.PDO
{
    public partial class Mps000262PDO : RDOBase
    {
        public const string printTypeCode = "Mps000262";

        public Mps000262PDO() { }

        public Mps000262PDO(List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterials, List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicines, List<V_HIS_EXP_MEST> _listExpMest, List<V_HIS_TREATMENT> _listTreatment, List<V_HIS_TREATMENT_BED_ROOM> _listTreatmentBedRoom, V_HIS_EXP_MEST aggExpMest, List<HIS_SERVICE_REQ> listServiceReq)
        {
            try
            {
                this.ListExpMest = _listExpMest;
                this.ListExpMestMaterials = _listExpMestMaterials;
                this.ListExpMestMedicines = _listExpMestMedicines;
                this.ListTreatment = _listTreatment;
                this.ListTreatmentBedRoom = _listTreatmentBedRoom;
                this.AggrExpMest = aggExpMest;
                this.ListServiceReq = listServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
