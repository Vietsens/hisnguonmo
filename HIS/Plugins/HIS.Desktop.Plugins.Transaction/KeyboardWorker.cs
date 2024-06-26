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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Transaction
{
    [KeyboardAction("BtnFind", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnFind", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("BtnInvoice", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnInvoice", KeyStroke = XKeys.Control | XKeys.I)]
    [KeyboardAction("BtnDeposit", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnDeposit", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("BtnDepositService", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnDepositService", KeyStroke = XKeys.F4)]
    [KeyboardAction("BtnRepay", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnRepay", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("BtnRepayService", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnRepayService", KeyStroke = XKeys.Control | XKeys.J)]
    [KeyboardAction("BtnBill", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnBill", KeyStroke = XKeys.F6)]
    [KeyboardAction("BtnBillNotKc", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnBillNotKc", KeyStroke = XKeys.F7)]
    [KeyboardAction("BtnMienGiam", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnMienGiam", KeyStroke = XKeys.Control | XKeys.M)]
    [KeyboardAction("BtnLock", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnLock", KeyStroke = XKeys.Control | XKeys.L)]
    [KeyboardAction("BtnLockHistory", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnLockHistory", KeyStroke = XKeys.Control | XKeys.H)]
    [KeyboardAction("BtnBordereau", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnBordereau", KeyStroke = XKeys.F5)]
    [KeyboardAction("BtnTranList", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnTranList", KeyStroke = XKeys.Control | XKeys.G)]
    [KeyboardAction("FocusKeyword", "HIS.Desktop.Plugins.Transaction.UCTransaction", "FocusKeyword", KeyStroke = XKeys.F2)]
    [KeyboardAction("FocusPatientCode", "HIS.Desktop.Plugins.Transaction.UCTransaction", "FocusPatientCode", KeyStroke = XKeys.F3)]

    [KeyboardAction("BtnCallPatient", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnCallPatient", KeyStroke = XKeys.F8)]
    [KeyboardAction("BtnRecallPatient", "HIS.Desktop.Plugins.Transaction.UCTransaction", "BtnRecallPatient", KeyStroke = XKeys.F9)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]

    [KeyboardAction("TemporaryLock", "HIS.Desktop.Plugins.Transaction.UCTransaction", "TemporaryLock", KeyStroke = XKeys.Control|XKeys.B)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public partial class KeyboardWorker : Tool<IDesktopToolContext>
    {
        public KeyboardWorker()
            : base()
        {
        }

        public override IActionSet Actions
        {
            get
            {
                return base.Actions;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
