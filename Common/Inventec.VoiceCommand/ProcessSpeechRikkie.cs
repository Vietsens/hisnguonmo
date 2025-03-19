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

namespace Inventec.VoiceCommand
{
    internal class ProcessSpeechRikkie
    {
        internal ProcessSpeechRikkie() { }
        internal async Task<ResultCommandADO> Run(byte[] BA_AudioFile)
        {
            ResultCommandADO resultTDO = new ResultCommandADO();
            if (BA_AudioFile == null || BA_AudioFile.Length == 0)
            {
                resultTDO.messageError = "Audio data is empty";
                return resultTDO;
            }

            ApiConsumers.RIKKEIAIConsumer.AddDicHeaderRequest("Authorization", String.Format("Bearer {0}", CommandCFG.RikkeiAccessToken));

            Dictionary<string, object> dicContent = new Dictionary<string, object>();
            dicContent.Add("idSession", "test123");
            dicContent.Add("audio", BA_AudioFile);
            try
            {
                resultTDO = ApiConsumers.RIKKEIAIConsumer.PostWithFileWithouApiParam<ResultCommandADO>("/api/upload/audio", dicContent, 0);
                //resultTDO = await ApiConsumers.RIKKEIAIConsumer.PostWithFileWithouApiParamAsync<ResultTDO>("/api/upload/audio", dicContent, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            Inventec.Common.Logging.LogSystem.Debug("ProcessSpeechRikkie" + Inventec.Common.Logging.LogUtil.TraceData("RikkeiAccessToken", CommandCFG.RikkeiAccessToken)
                + Inventec.Common.Logging.LogUtil.TraceData("resultTDO", resultTDO));

            return resultTDO;
        }
    }
}
