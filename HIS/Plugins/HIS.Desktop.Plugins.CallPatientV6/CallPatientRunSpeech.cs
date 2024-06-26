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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientV6
{
    class CallPatientRunSpeech
    {
        internal static void CallPatientByNumOder(string patientName, long numOder, string examRoomName)
        {
            try
            {
                string moiBenhNhanStr = WaitingScreenCFG.CALL_PATIENT_MOI_BENH_NHAN_STR;
                string coSoSttStr = WaitingScreenCFG.CALL_PATIENT_CO_STT_STR;
                string denStr = WaitingScreenCFG.CALL_PATIENT_DEN_STR;

                Inventec.Speech.SpeechPlayer.SpeakSingle(moiBenhNhanStr);
                Inventec.Speech.SpeechPlayer.Speak(patientName);
                Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                Inventec.Speech.SpeechPlayer.Speak(numOder);
                Inventec.Speech.SpeechPlayer.SpeakSingle(denStr);
                Inventec.Speech.SpeechPlayer.SpeakSingle(examRoomName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
