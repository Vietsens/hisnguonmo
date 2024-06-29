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
using HIS.UC.KskContract.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.KskContract.Run
{
    public sealed class RunBehavior : IRun
    {
        KskContractInput kskContractInput;

        public RunBehavior()
            : base()
        {
        }

        public RunBehavior(CommonParam param, KskContractInput kskContractInput)
            : base()
        {
            this.kskContractInput = kskContractInput;
        }

        object IRun.Run(TemplateType.ENUM temp)
        {
            object result = null;
            try
            {

                switch (temp)
                {
                    case TemplateType.ENUM.TEMPLATE_1:
                        result = new UCKskContract__Template1(this.kskContractInput);
                        break;
                    case TemplateType.ENUM.TEMPLATE_2:
                        result = new UCKskContract__Template2(this.kskContractInput);
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
