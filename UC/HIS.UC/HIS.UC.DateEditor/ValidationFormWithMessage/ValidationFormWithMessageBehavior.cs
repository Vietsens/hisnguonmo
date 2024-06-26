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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.DateEditor.ValidationFormWithMessage
{
    public sealed class ValidationFormWithMessageBehavior : IValidationFormWithMessage
    {
        UserControl control;
        List<string> errorEmpty;
        List<string> errorOther;

        public ValidationFormWithMessageBehavior()
            : base()
        {
        }

        public ValidationFormWithMessageBehavior(CommonParam param, UserControl uc, List<string> errorEmpty, List<string> errorOther)
            : base()
        {
            this.control = uc;
            this.errorEmpty = errorEmpty;
            this.errorOther = errorOther;
        }

        object IValidationFormWithMessage.Run()
        {
            try
            {
                return ((UCDate)this.control).ValidationFormWithMessage(this.errorEmpty, this.errorOther);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
