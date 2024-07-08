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
using HIS.Desktop.Plugins.Library.CheckHeinGOV;

namespace His.UC.UCHein.Core.SetResultDataADO
{
    class SetResultDataADOBehavior :BeanObjectBase,  ISetResultDataADO
    {
        System.Windows.Forms.UserControl UC;
        ResultDataADO ResultDataADO;
        internal SetResultDataADOBehavior(CommonParam param, System.Windows.Forms.UserControl uc, ResultDataADO ResultDataADO)
            : base(param)
        {
            UC = uc;
            this.ResultDataADO = ResultDataADO;
        }

        void ISetResultDataADO.Run()
        {
            try
            {
                ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)UC).SetRsDataADO(ResultDataADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
        }
    }
}
