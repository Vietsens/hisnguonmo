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
using System.Windows.Forms;
using Inventec.Core;
namespace HIS.UC.WorkPlace.ResetValidate
{
    class ResetValidateBehavior : IResetValidate
    {
         UserControl control;

        public ResetValidateBehavior()
            : base()
        {
        }

        public ResetValidateBehavior(CommonParam param, UserControl uc)
            : base()
        {
            this.control = uc;
        }

        void IResetValidate.Run()
        {
            try
            {
                ((UCWorkPlaceCombo1)this.control).ResetValidation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }
    }
}
