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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Inventec.Common.Logging;

namespace Inventec.Speech
{
    public class SpeechPlayer
    {
        public static string TypeSpeechCFG { get; set; }

        /// <summary>
        /// Phat am 1 cau lien mach
        /// </summary>
        /// <param name="content">Noi dung can phat am</param>
        public static void SpeakSingle(string content)
        {
            List<string> contentList = new List<string>() { content };
            SpeechPlayer.PlayList(contentList);
        }

        /// <summary>
        /// Pham am tach thanh tung tu rieng biet.
        /// </summary>
        /// <param name="content">Chi chap nhat du lieu kieu: string, int, long, decimal</param>
        public static void Speak(params object[] content)
        {
            List<string> wordList = SpeechPlayer.ParseWords(content);
            SpeechPlayer.PlayList(wordList);
        }

        /// <summary>
        /// Pham am mot so lien mach.
        /// </summary>
        /// <param name="content">Chi chap nhat du lieu kieu: int, long, decimal</param>
        public static void SpeakNumber(params object[] content)
        {
            List<string> wordList = SpeechPlayer.ParseWordByNumbers(content);
            SpeechPlayer.PlayList(wordList);
        }

        /// <summary>
        /// Thuc hien phat am bang viec "play" 1 danh sach cac file audio (.wav)
        /// </summary>
        /// <param name="fileList"></param>
        private static void PlayList(List<string> contentList)
        {
            try
            {
                List<string> fileList = SpeechPlayer.GetFiles(contentList);
                if (fileList != null && fileList.Count > 0)
                {
                    //using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
                    //{
                    //    if (synthesizer == null)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error("null");
                    //    }
                    //    PromptBuilder builder = new PromptBuilder();
                    //    foreach (string s in fileList)
                    //    {
                    //        builder.AppendAudio(s);
                    //    }
                    //    synthesizer.Rate = 5;
                    //    synthesizer.Speak(builder);
                    //}
                    foreach (string s in fileList)
                    {
                        System.Media.SoundPlayer snd = new System.Media.SoundPlayer(s);
                        snd.PlaySync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay danh sach file audio dua vao danh sach noi dung can phat am
        /// </summary>
        /// <param name="contentList"></param>
        /// <returns></returns>
        private static List<string> GetFiles(List<string> contentList)
        {
            if (contentList != null && contentList.Count > 0)
            {
                bool isCorrupt = false;
                List<string> fileList = new List<string>();
                foreach (string p in contentList)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        string filePath = Path.Combine(SpeechServiceConstant.VOICE_FOLDER, string.Format("{0}.wav", p));
                        if (!File.Exists(filePath))
                        {
                            bool hasFile = false;
                            if (!String.IsNullOrWhiteSpace(TypeSpeechCFG))
                            {
                                if (TypeSpeechCFG == "1")
                                {
                                    hasFile = SpeechService.GetVoiceGoogleTranslate(p);
                                }
                                else if (TypeSpeechCFG == "2")
                                {
                                    hasFile = SpeechService.GetVoiceFpt(p);
                                }
                                else
                                {
                                    hasFile = SpeechService.GetVoice(p);
                                }
                            }
                            else
                            {
                                hasFile = SpeechService.GetVoice(p);
                            }

                            if (!hasFile)
                            {
                                isCorrupt = true;
                                continue;
                            }
                        }
                        fileList.Add(filePath);
                    }
                }
                return isCorrupt ? null : fileList;
            }
            return null;
        }

        /// <summary>
        /// Tach thanh cac tu rieng biet
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static List<string> ParseWords(params object[] content)
        {
            if (content != null && content.Length > 0)
            {
                List<string> wordList = new List<string>();
                foreach (object t in content)
                {
                    List<string> s = null;
                    if (t.GetType() == typeof(string))
                    {
                        s = SpeechPlayer.SplitText((string)t);
                    }
                    else if (t.GetType() == typeof(int))
                    {
                        s = SpeechPlayer.SplitNumber((int)t);
                    }
                    else if (t.GetType() == typeof(long))
                    {
                        s = SpeechPlayer.SplitNumber((long)t);
                    }
                    else if (t.GetType() == typeof(decimal))
                    {
                        s = SpeechPlayer.SplitNumber((decimal)t);
                    }
                    if (s != null && s.Count > 0)
                    {
                        wordList.AddRange(s);
                    }
                }
                return wordList;
            }
            return null;
        }

        private static List<string> ParseWordByNumbers(params object[] content)
        {
            if (content != null && content.Length > 0)
            {
                List<string> wordList = new List<string>();
                foreach (object t in content)
                {
                    string s = null;
                    s = Common.String.Convert.CurrencyToVneseString(t.ToString()).Trim();
                    if (!String.IsNullOrEmpty(s))
                    {
                        wordList.Add(s);
                    }
                }
                return wordList;
            }
            return null;
        }

        /// <summary>
        /// Tach doan ki tu thanh cac tu rieng biet
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static List<string> SplitText(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = content.Trim().ToLower();
                return content.Split(' ').ToList();
            }
            return null;
        }

        /// <summary>
        /// Tach so thanh cac tu rieng biet
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<string> SplitNumber(int number)
        {
            string content = number.ToString();
            return SpeechPlayer.SplitNumber(content);
        }

        /// <summary>
        /// Tach so thanh cac tu rieng biet
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<string> SplitNumber(decimal number)
        {
            string content = number.ToString();
            return SpeechPlayer.SplitNumber(content);
        }

        /// <summary>
        /// Tach so thanh cac tu rieng biet
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<string> SplitNumber(long number)
        {
            string content = number.ToString();
            return SpeechPlayer.SplitNumber(content);
        }

        /// <summary>
        /// Tach so thanh cac tu rieng biet
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<string> SplitNumber(string number)
        {
            string[] result = new string[number.Length];
            for (int i = 0; i < number.Length; i++)
            {
                result[i] = number.Substring(i, 1);
            }
            return result.ToList();
        }
    }
}
