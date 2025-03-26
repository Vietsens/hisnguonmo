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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.VoiceCommand
{
    class ProcessSpeechWit
    {
        internal ProcessSpeechWit() { }

        internal async Task<ResultCommandADO> RunWithWebRequest(byte[] BA_AudioFile)
        {
            ResultCommandADO resultTDO = new ResultCommandADO();
            if (BA_AudioFile == null || BA_AudioFile.Length == 0)
            {
                resultTDO.message = "Audio data is empty";
                return resultTDO;
            }

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(CommandCFG.WitAI__API);

            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer " + CommandCFG.WitAIAccessToken;
            request.ContentType = "audio/wav";
            request.ContentLength = BA_AudioFile.Length;
            request.GetRequestStream().Write(BA_AudioFile, 0, BA_AudioFile.Length);

            // Process the wit.ai response
            try
            {
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    StreamReader response_stream = new StreamReader(response.GetResponseStream());
                    resultTDO.text = response_stream.ReadToEnd();
                    resultTDO.success = true;
                }
                else
                {
                    resultTDO.message = "Error: " + response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                resultTDO.message = "Error: " + ex.Message;
            }

            return resultTDO;
        }

        internal async Task<ResultCommandADO> Run(byte[] BA_AudioFile)
        {
            ResultCommandADO resultTDO = new ResultCommandADO();
            if (BA_AudioFile == null || BA_AudioFile.Length == 0)
            {
                resultTDO.message = "Audio data is empty";
                return resultTDO;
            }

            ApiConsumers.WITAIConsumer.AddDicHeaderRequest("Authorization", String.Format("Bearer {0}", CommandCFG.WitAIAccessToken));

            try
            {
                resultTDO = ApiConsumers.WITAIConsumer.PostWithBinaryWithouApiParam<ResultCommandADO>("/speech", BA_AudioFile, 0);
                //resultTDO = await ApiConsumers.WITAIConsumer.PostWithBinaryWithouApiParamAsync<ResultTDO>("/speech", BA_AudioFile, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return resultTDO;
        }
    }
}
