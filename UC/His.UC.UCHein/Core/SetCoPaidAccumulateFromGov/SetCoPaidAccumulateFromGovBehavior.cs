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
using His.UC.UCHein;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Core.SetCoPaidAccumulateFromGov
{
    class SetCoPaidAccumulateFromGovBehavior : BeanObjectBase, ISetCoPaidAccumulateFromGov
    {
        System.Windows.Forms.UserControl UC;
        ResultMCCTADO ResultMCCTADO;

        internal SetCoPaidAccumulateFromGovBehavior(CommonParam param, System.Windows.Forms.UserControl uc, ResultMCCTADO resultMcct)
            : base(param)
        {
            UC = uc;
            this.ResultMCCTADO = resultMcct;
        }

        void ISetCoPaidAccumulateFromGov.Run()
        {
            try
            {
                ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)UC).SetCoPaidAccumulateFromGov(ResultMCCTADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
        }
    }
}
