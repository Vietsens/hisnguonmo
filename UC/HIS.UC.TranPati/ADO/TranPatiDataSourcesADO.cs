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

namespace HIS.UC.TranPati.ADO
{
    public class TranPatiDataSourcesADO
    {
        public List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> HisMediOrgs { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> HisTranPatiReasons { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> HisTranPatiForms { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH> HisTranPatiTechs { get; set; }
        public MOS.EFMODEL.DataModels.HIS_TREATMENT CurrentHisTreatment { get; set; }

        public string UsedMedicine { get; set; }
        public DelegateNextFocus DelegateNextFocus { get; set; }
    }
}
