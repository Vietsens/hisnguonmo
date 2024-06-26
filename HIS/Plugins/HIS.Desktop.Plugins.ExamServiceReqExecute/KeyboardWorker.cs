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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    [KeyboardAction("FinishShortCut", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "FinishShortCut", KeyStroke = XKeys.Control | XKeys.E)]
    [KeyboardAction("SaveFinishShortCut", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "SaveFinishShortCut", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("AssignService", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "AssignService", KeyStroke = XKeys.F9)]
    [KeyboardAction("AssignPre", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "AssignPre", KeyStroke = XKeys.F8)]
    [KeyboardAction("TreatmentFinish", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "TreatmentFinish", KeyStroke = XKeys.F10)]
    [KeyboardAction("HospitalizeF11", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "HospitalizeF11", KeyStroke = XKeys.F11)]
    [KeyboardAction("ShortCutCtrl0", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl0", KeyStroke = XKeys.Control | XKeys.Digit0)]
    [KeyboardAction("ShortCutCtrl1", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl1", KeyStroke = XKeys.Control | XKeys.Digit1)]
    [KeyboardAction("ShortCutCtrl2", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl2", KeyStroke = XKeys.Control | XKeys.Digit2)]
    [KeyboardAction("ShortCutCtrl3", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl3", KeyStroke = XKeys.Control | XKeys.Digit3)]
    [KeyboardAction("ShortCutCtrl4", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl4", KeyStroke = XKeys.Control | XKeys.Digit4)]
    [KeyboardAction("ShortCutCtrl5", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl5", KeyStroke = XKeys.Control | XKeys.Digit5)]
    [KeyboardAction("ShortCutCtrl6", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl6", KeyStroke = XKeys.Control | XKeys.Digit6)]
    [KeyboardAction("ShortCutCtrl7", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl7", KeyStroke = XKeys.Control | XKeys.Digit7)]
    [KeyboardAction("ShortCutCtrl8", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl8", KeyStroke = XKeys.Control | XKeys.Digit8)]
    [KeyboardAction("ShortCutCtrl9", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrl9", KeyStroke = XKeys.Control | XKeys.Digit9)]
    [KeyboardAction("ShortCutCtrlQ", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlQ", KeyStroke = XKeys.Control | XKeys.Q)]
    [KeyboardAction("ShortCutCtrlW", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlW", KeyStroke = XKeys.Control | XKeys.W)]
    [KeyboardAction("ShortCutCtrlU", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlU", KeyStroke = XKeys.Control | XKeys.U)]
    [KeyboardAction("ShortCutCtrlR", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlR", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("ShortCutCtrlT", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlT", KeyStroke = XKeys.Control | XKeys.T)]
    [KeyboardAction("ShortCutCtrlI", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "ShortCutCtrlI", KeyStroke = XKeys.Control | XKeys.I)]
    [KeyboardAction("FocusIcd", "HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl", "FocusIcd", KeyStroke = XKeys.F2)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<IDesktopToolContext>
    {
        public KeyboardWorker() : base() { }

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
