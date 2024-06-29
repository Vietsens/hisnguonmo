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
using HIS.UC.TreatmentFinish.ADO;
using HIS.UC.TreatmentFinish.GetValidate;
using HIS.UC.TreatmentFinish.Run;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreatmentFinish.GetValidate
{
    class GetValidateWithMessageBehavior : IGetValidate
    {
        UserControl entity; 
        List<string> errorEmpty;
        List<string> errorOther;

        internal GetValidateWithMessageBehavior(UserControl control, List<string> errorEmpty, List<string> errorOther)
            : base()
        {
            this.entity = control; 
            this.errorEmpty = errorEmpty;
            this.errorOther = errorOther;
        }

        bool IGetValidate.Run()
        {
            bool result = true;
            try
            {
                if (this.entity.GetType() == typeof(UCTreatmentFinish))
                {
                    result = ((UCTreatmentFinish)this.entity).GetValidateWithMessage(this.errorEmpty, this.errorOther);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
