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

namespace HIS.UC.KskContract.GetValue
{
    public sealed class GetValueBehavior : IGetValue
    {
        UserControl control;
        public GetValueBehavior()
            : base()
        {
        }

        public GetValueBehavior(CommonParam param, UserControl uc)
            : base()
        {
            this.control = uc;
        }

        object IGetValue.Run(TemplateType.ENUM temp)
        {
            object result = null;
            try
            {
                switch (temp)
                {
                    case TemplateType.ENUM.TEMPLATE_1:
                        result = ((UCKskContract__Template1)this.control).GetValue();
                        break;
                    case TemplateType.ENUM.TEMPLATE_2:
                        result = ((UCKskContract__Template2)this.control).GetValue();
                        break;
                    default:
                        break;
                }; 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
    }
}
