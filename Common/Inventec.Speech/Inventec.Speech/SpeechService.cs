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
using Inventec.Common.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Inventec.Speech
{
    class SpeechService
    {
        public static bool GetVoice(string content)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    content = content.Trim().ToLower();

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(SpeechServiceConstant.URI);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, SpeechServiceConstant.TIME_OUT);
                        string requestUri = string.Format(SpeechServiceConstant.REQUEST_PARAMS, Uri.EscapeUriString(content));
                        HttpResponseMessage resp = client.GetAsync(requestUri).Result;
                        if (resp != null && resp.IsSuccessStatusCode)
                        {
                            Stream stream = resp.Content.ReadAsStreamAsync().Result;
                            if (stream != null)
                            {
                                string folderPath = SpeechServiceConstant.VOICE_FOLDER;

                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }

                                SpeechService.SaveMp3ToWavFile(stream, string.Format("{0}\\{1}.wav", folderPath, content));
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return false;
        }

        private static void SaveMp3ToWavFile(Stream stream, string outPath)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(stream))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(outPath, pcm);
                }
            }
        }

        public static bool GetVoiceFpt(string content)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    using (var client = new HttpClient())
                    {
                        string requestedUrl = "/text2speech/v4";
                        var apiKey = System.Configuration.ConfigurationManager.AppSettings["Inventec.Speech.Voice.FPT_ApiKey"];
                        if (String.IsNullOrWhiteSpace(apiKey)) return false;

                        client.DefaultRequestHeaders.Add("api_key", apiKey);
                        client.DefaultRequestHeaders.Add("voice", "female");
                        client.DefaultRequestHeaders.Add("speed", "0");

                        client.BaseAddress = new Uri("http://api.openfpt.vn/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, SpeechServiceConstant.TIME_OUT);
                        StringContent queryString = new StringContent(string.Format("\"{0}\"", content));

                        HttpResponseMessage resp = client.PostAsync(requestedUrl, queryString).Result;
                        if (resp.IsSuccessStatusCode)
                        {
                            string responseData = resp.Content.ReadAsStringAsync().Result;
                            var data = JsonConvert.DeserializeObject<FPT.ApiResponseData>(responseData);
                            if (data != null && !String.IsNullOrWhiteSpace(data.async))
                            {
                                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                                {
                                    using (System.Net.WebClient down = new System.Net.WebClient())
                                    {
                                        using (var stream = new MemoryStream(down.DownloadData(data.async)))
                                        {
                                            if (stream != null)
                                            {
                                                string folderPath = SpeechServiceConstant.VOICE_FOLDER;

                                                if (!Directory.Exists(folderPath))
                                                {
                                                    Directory.CreateDirectory(folderPath);
                                                }

                                                SpeechService.SaveMp3ToWavFile(stream, string.Format("{0}\\{1}.wav", folderPath, content));
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool GetVoiceGoogleTranslate(string content)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    content = content.Trim().ToLower();

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(SpeechServiceConstant.GOOGLE_URI);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, SpeechServiceConstant.TIME_OUT);
                        string requestUri = string.Format(SpeechServiceConstant.GOOGLE_REQUEST_PARAMS, Uri.EscapeUriString(content));
                        HttpResponseMessage resp = client.GetAsync(requestUri).Result;
                        if (resp != null && resp.IsSuccessStatusCode)
                        {
                            Stream stream = resp.Content.ReadAsStreamAsync().Result;
                            if (stream != null)
                            {
                                string folderPath = SpeechServiceConstant.VOICE_FOLDER;
                                try
                                {
                                    if (!Directory.Exists(folderPath))
                                    {
                                        Directory.CreateDirectory(folderPath);
                                    }

                                    SpeechService.SaveMp3ToWavFile(stream, string.Format("{0}\\{1}.wav", folderPath, content));

                                    return true;
                                }
                                catch (Exception exx)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => folderPath), folderPath), exx);
                                }
                            }
                        }
                    }

                    //bổ sung cách tải khác nếu tải ở trên không thành công(không return)
                    string fullRequestUri = string.Format(SpeechServiceConstant.GOOGLE_REQUEST_PARAMS, HttpUtility.UrlEncode(content, Encoding.GetEncoding("utf-8")));
                    string fullUri = string.Format("{0}/{1}", SpeechServiceConstant.GOOGLE_URI, fullRequestUri);
                    WebClient webClient = new WebClient();
                    webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:7.0.1) Gecko/20100101 Firefox/7.0.1";
                    byte[] byteData = webClient.DownloadData(fullUri);
                    if (byteData != null)
                    {
                        MemoryStream ms = new MemoryStream(byteData);
                        string folderPath = SpeechServiceConstant.VOICE_FOLDER;
                        try
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            SpeechService.SaveMp3ToWavFile(ms, string.Format("{0}\\{1}.wav", folderPath, content));

                            return true;
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => folderPath), folderPath), exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return false;
        }
    }
}
