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
