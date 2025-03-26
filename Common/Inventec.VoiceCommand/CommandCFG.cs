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
    public class CommandCFG
    {
        internal const int BufferMilliseconds = 500;//200;
        internal const int SampleRate = 16000;
        internal const int ChannelCount = 1;
        internal const int BytesPerSample = 2;
        internal const int BytesPerSecond = SampleRate * ChannelCount * BytesPerSample;

        public static string RikkeiAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE2MDA2ODcwMTYsIm5iZiI6MTYwMDY4NzAxNiwianRpIjoiZTcyNjcyZjItMjRhYi00NGU0LWIwN2MtNWYyMDRlYTBmODNmIiwiZXhwIjoxNjE2MjM5MDE2LCJpZGVudGl0eSI6InVzZXIxIiwiZnJlc2giOmZhbHNlLCJ0eXBlIjoiYWNjZXNzIn0.B56Ukk20ytqvlx_hWXbxe_-0feSgqrb6IBTeAEJPcEM";
        public static string WitAIAccessToken = "76OUAHDBMT2FGHSC2ECOY5GRQKFPCWO2";//L35S77RR5JM6GMG35CZIU76DTVQNOTZK

        public static string VaisAccessToken = "14a37fbc-382e-11ea-bc1d-0242ac140007";

        public static int apiTypeAI = 3;
        public const int apiTypeAI__WITAI = 1;
        public const int apiTypeAI__RIKKEIAI = 2;
        public const int apiTypeAI__VAIS = 3;


        public static string RikkeiAI__URI = "http://stt-api.rikkei.org";
        public static string WitAI__URI = "https://api.wit.ai";
        public static string WitAI__API = "https://api.wit.ai/speech";
        public static string Vais__WssUrl = "wss://nhaplieu.vais.vn";
        public static string Vais__APIKEY = "example_api";
        public static int Vais__BufferMillisecond = 800;
        public static int Vais__SampleRate = 16000;
        public static string Vais__ModelName = "general";
        public static string Vais__version = "v1";


    }
}
