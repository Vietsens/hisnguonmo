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
using HIS.UC.KskContract.Run;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.KskContract.InFocus
{
    public sealed class InFocusBehavior : IInFocus
    {
        UserControl control;
        public InFocusBehavior()
            : base()
        {
        }

        public InFocusBehavior(CommonParam param, UserControl uc)
            : base()
        {
            this.control = uc;
        }

        void IInFocus.Run(TemplateType.ENUM temp)
        {
            try
            {
                switch (temp)
                {
                    case TemplateType.ENUM.TEMPLATE_1:
                        ((UCKskContract__Template1)this.control).InFocus();
                        break;
                    case TemplateType.ENUM.TEMPLATE_2:
                        ((UCKskContract__Template2)this.control).InFocus();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
