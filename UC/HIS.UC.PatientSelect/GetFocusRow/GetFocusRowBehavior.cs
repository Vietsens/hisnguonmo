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
using HIS.UC.PatientSelect.ADO;
using HIS.UC.PatientSelect.GetFocusRow;
using HIS.UC.PatientSelect.Reload;
using HIS.UC.PatientSelect.Run;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.PatientSelect.GetFocusRow
{
    class GetFocusRowBehavior : IGetFocusRow
    {
        UserControl entity;

        internal GetFocusRowBehavior(UserControl control)
            : base()
        {
            this.entity = control;
        }

        V_HIS_TREATMENT_BED_ROOM IGetFocusRow.Run()
        {
            V_HIS_TREATMENT_BED_ROOM result = null;
            try
            {
                if (this.entity.GetType() == typeof(UCPatientSelect))
                {
                    result = ((UCPatientSelect)this.entity).GetFocusRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
