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
using MOS.SDO;

namespace His.UC.UCHein.Core.UpdateDataFormIntoPatientProfile
{
    class PatientOldUnder6Behavior : BeanObjectBase, IPatientOldUnder6
    {
        Design.TemplateHeinBHYT1.Template__HeinBHYT1 UC;
        bool IsChild;

        internal PatientOldUnder6Behavior(CommonParam param, Design.TemplateHeinBHYT1.Template__HeinBHYT1 uc, bool IsChild)
            : base(param)
        {
            this.UC = uc;
            this.IsChild = IsChild;
        }

        void IPatientOldUnder6.Run()
        {
            try
            {
                UC.FillDataPatientOldYnder6(this.IsChild);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
