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
using His.UC.UCHein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Set.ResetValueControl
{
    class ResetValueControlKskBehavior :BeanObjectBase,  IResetValueControl
    {
        System.Windows.Forms.UserControl UC;

        internal ResetValueControlKskBehavior(CommonParam param, System.Windows.Forms.UserControl uc)
            : base(param)
        {
            UC = uc;
        }

        void IResetValueControl.Run()
        {
            try
            {
                ((Design.TemplateKskContract1.Template__KskContract1)UC).ResetValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
        }
    }
}
