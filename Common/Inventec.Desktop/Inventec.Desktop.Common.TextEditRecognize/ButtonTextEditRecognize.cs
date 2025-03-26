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
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.TextEditRecognize
{
    public class ButtonTextEditRecognize : ButtonEdit
    {
        Action<string> actUpdateInputAfterRegconize;
        public void SetParamUpdateInputAfterRegconize(Action<string> __actUpdateInputAfterRegconize, string wit_Ai_Access_Token, int timereplay)
        {
            this.actUpdateInputAfterRegconize = __actUpdateInputAfterRegconize;
            Inventec.Common.WitAI.Vitals.Constan.SetConstan(wit_Ai_Access_Token, timereplay);
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData == Keys.F8)
            {
                Inventec.Common.WitAI.Form1 f1 = new Inventec.Common.WitAI.Form1(actUpdateInputAfterRegconize);
                f1.ShowDialog();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
