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

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using VAIS.ASR.Lib;


namespace Inventec.VoiceCommand
{
    class ProcessSpeechVais
    {
        Mic2Wss objMic2Wss = new Mic2Wss();
        internal ProcessSpeechVais() {
            objMic2Wss.MicNumber = 0; // select Mic
        }
        internal async Task<ResultCommandADO> Run(byte[] BA_AudioFile)
        {
            ResultCommandADO resultTDO = new ResultCommandADO();
            if (BA_AudioFile == null || BA_AudioFile.Length == 0)
            {
                resultTDO.messageError = "Audio data is empty";
                return resultTDO;
            }

            //ApiConsumers.VAISConsumer.AddDicHeaderRequest("Authorization", String.Format("Bearer {0}", CommandCFG.VaisAccessToken));
            //ApiConsumers.VAISConsumer.AddDicHeaderRequest("api-key", CommandCFG.Vais__WssUrl);

            try
            {
                objMic2Wss.WssUrl = CommandCFG.Vais__WssUrl;             
                await objMic2Wss.StartListening();

                while (objMic2Wss.IsListening())
                {
                    VaisAsrResult result = await objMic2Wss.Transcribe();
                    resultTDO.text = result.Transcript;
                    resultTDO.message = result.Transcript;
                    resultTDO.success = true;
                    resultTDO.status = 2;
                    //txtChunkText.Text = result.Transcript;
                    //if (result.IsFinal)
                    //{
                    //    txtTranscript.AppendText(txtChunkText.Text + System.Environment.NewLine);
                    //    txtChunkText.Text = "";

                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return resultTDO;
        }


        //internal async Task<ResultCommandADO> RunWithWebRequest(string audio_id)
        //{
        //    Inventec.Common.Logging.LogSystem.Warn("RunWithWebRequest1");

        //    ResultCommandADO resultTDO = new ResultCommandADO();
        //    if (string.IsNullOrEmpty(audio_id.ToString()))
        //    {
        //        resultTDO.message = "Audio data is empty";
        //        return resultTDO;
        //    }
        //    Inventec.Common.Logging.LogSystem.Warn("RunWithWebRequest2");


        //    System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(CommandCFG.Vais__API_Get);

        //    request.Method = "GET";
        //    request.Headers["Headers"] = "api-key " + CommandCFG.VaisAccessToken;
        //    request.Headers["Params"] = "audio_id " + audio_id;

        //    Inventec.Common.Logging.LogSystem.Info("tạo HttpWebRequest request: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => request), request));

        //    // Process the wit.ai response
        //    try
        //    {
        //        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            StreamReader response_stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //            resultTDO.text = response_stream.ReadToEnd();
        //            resultTDO.success = true;
        //        }
        //        else
        //        {
        //            resultTDO.message = "Error: " + response.StatusCode.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        resultTDO.message = "Error: " + ex.Message;
        //    }
        //    Inventec.Common.Logging.LogSystem.Warn("RunWithWebRequest3");
        //    return resultTDO;
        //}
    }
}
