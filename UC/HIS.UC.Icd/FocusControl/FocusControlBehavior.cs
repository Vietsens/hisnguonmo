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

namespace HIS.UC.Icd.FocusControl
{
    public sealed class FocusControlBehavior : IFocusControl
    {
        UserControl control;
        HIS.UC.Icd.ADO.Template Template;
        public FocusControlBehavior()
            : base()
        {
        }

        public FocusControlBehavior(CommonParam param, UserControl uc, HIS.UC.Icd.ADO.Template template = HIS.UC.Icd.ADO.Template.Default)
            : base()
        {
            this.control = uc;
            this.Template = template;
        }

        void IFocusControl.Run()
        {
            try
            {
                if (this.Template == HIS.UC.Icd.ADO.Template.NoFocus)
                {
                    ((UCIcdNoFocus)this.control).FocusControl();
                }
                else
                    ((UCIcd)this.control).FocusControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}