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
using His.UC.UCHein.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Init
{
    class InitBhytBehavior : BeanObjectBase, IInitAsync
    {
        DataInitHeinBhyt entity;

        internal InitBhytBehavior(CommonParam param, DataInitHeinBhyt data)
            : base(param)
        {
            entity = data;
        }

        async Task<object> IInitAsync.Run()
        {
            object result = null;
            try
            {
                if (entity.Template == MainHisHeinBhyt.TEMPLATE__BHYT1)
                {
                    result = new His.UC.UCHein.Design.TemplateHeinBHYT1.Template__HeinBHYT1(entity);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
            return result;
        }
    }
}
