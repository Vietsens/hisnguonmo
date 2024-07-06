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

namespace His.UC.UCHein.Get.ExpriedTimeHeinCardBhyt
{
    class ExpriedTimeHeinCardBhytBehavior : BeanObjectBase, IExpriedTimeHeinCardBhyt
    {
        Design.TemplateHeinBHYT1.Template__HeinBHYT1 UC;
        long alertExpriedTimeHeinCardBhyt;
        long resultDayAlert;

        internal ExpriedTimeHeinCardBhytBehavior(CommonParam param, Design.TemplateHeinBHYT1.Template__HeinBHYT1 uc, long alertExpriedCard, ref long resultdayAlert)
            : base(param)
        {
            UC = uc;
            alertExpriedTimeHeinCardBhyt = alertExpriedCard;
            resultDayAlert = resultdayAlert;
        }

        long IExpriedTimeHeinCardBhyt.Run()
        {
            long result = 0;
            try
            {
                result = UC.GetExpriedTimeHeinCardBhyt(alertExpriedTimeHeinCardBhyt, ref resultDayAlert);
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
