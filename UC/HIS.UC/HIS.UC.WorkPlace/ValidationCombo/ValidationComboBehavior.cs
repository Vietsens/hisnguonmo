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

namespace HIS.UC.WorkPlace.ValidationCombo
{
    class ValidationComboBehavior : IValidationCombo
    {
        UserControl control;
        HIS.UC.WorkPlace.WorkPlaceProcessor.Template temp;
        private CommonParam param;
        private UserControl userControl;

        public ValidationComboBehavior()
            : base()
        {
        }

        public ValidationComboBehavior(CommonParam param, UserControl uc, HIS.UC.WorkPlace.WorkPlaceProcessor.Template temp = WorkPlaceProcessor.Template.Combo1)
            : base()
        {
            this.control = uc;
            this.temp = temp;
        }


        object IValidationCombo.Run()
        {
            try
            {                
                return ((UCWorkPlaceCombo1)this.control).ValidationCombo();              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
