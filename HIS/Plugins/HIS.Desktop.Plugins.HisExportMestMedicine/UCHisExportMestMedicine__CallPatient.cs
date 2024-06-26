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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using System.Threading;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System.Reflection;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Speech;
namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        string[] listReplace = new string[] { "(", ")" };
        Thread threadCallPatient;
        private string callPatientFormName = "";
        public void LoadCallPatientByThread(object param)
        {
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 1");
            if (threadCallPatient != null && (threadCallPatient.ThreadState == ThreadState.WaitSleepJoin || threadCallPatient.ThreadState == ThreadState.Running))
            {
                MessageBox.Show("Đang gọi bệnh nhân", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            threadCallPatient = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataToControlCallPatientThread));
            try
            {
                threadCallPatient.Start(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadCallPatient.Abort();
            }
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 2");

        }
        internal void LoadDataToControlCallPatientThread(object param)
        {
            try
            {

                CallPatient(param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void CallPatient(object param)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 1");
                if (param is V_HIS_EXP_MEST_2)
                {
                    var data = param as V_HIS_EXP_MEST_2;
                    if (data != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 2");
                        V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        string roomName = room != null ? room.ROOM_NAME : null;
                        Inventec.Common.Logging.LogSystem.Debug(roomName);
                        CallPatientByNumOder(data, roomName);
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 3");
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void CallPatientByNumOder(V_HIS_EXP_MEST_2 data, string roomName)
        {
            if (data == null)
            {
                throw new Exception("data is " + data);
            }
            string patientName = data.TDL_PATIENT_NAME;
            for (int i = 0; i < listReplace.Length; i++)
            {
                patientName = patientName.Replace(listReplace[i], "");
            }
            if (CallPatientDataWorker.DicDelegateCallingPatient != null && CallPatientDataWorker.DicDelegateCallingPatient.ContainsKey(this.currentModuleBase.RoomId))
            {
                DelegateSelectData nhapNhay = CallPatientDataWorker.DicDelegateCallingPatient[this.currentModuleBase.RoomId];
                if (nhapNhay != null) nhapNhay(data);
            }
            SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

            string callPatientFormat = Base.HisConfigCFG.CALL_PATIENT_FORMAT;
            List<string> KEY_SINGLE = new List<string>() { "PATIENT_NAME", "PATIENT_DOB", "PATIENT_ADDRESS", "ROOM_NAME" };
            var strCallsplit = callPatientFormat.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (strCallsplit.ToList().Count > 0)
            {
                foreach (var word in strCallsplit)
                {
                    var checkKey = KEY_SINGLE.FirstOrDefault(o => o == word.ToUpper());
                    if (checkKey == null || checkKey.Count() == 0)
                    {
                        var strWordsplit = word.Split(new string[] { ",", ";", ".", "-", ":", "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in strWordsplit)
                        {
                            Inventec.Speech.SpeechPlayer.SpeakSingle(item.Trim());
                        }
                    }
                    else
                    {
                        switch (word)
                        {
                            case "PATIENT_NAME":
                                Inventec.Speech.SpeechPlayer.Speak(patientName);
                                break;
                            case "ROOM_NAME":
                                if(!string.IsNullOrEmpty(roomName))
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(roomName);
                                break;
                            case "PATIENT_DOB":
                                long year = Int64.Parse(data.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                Inventec.Speech.SpeechPlayer.Speak(year);
                                break;
                            case "PATIENT_ADDRESS":
                                Inventec.Speech.SpeechPlayer.Speak(data.TDL_PATIENT_ADDRESS);
                                break;                    
                            default:
                                break;
                        }
                    }
                }
            }

        }
    }
}
