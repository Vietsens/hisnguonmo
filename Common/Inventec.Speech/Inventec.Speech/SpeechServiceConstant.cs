using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Speech
{
    class SpeechServiceConstant
    {
        public static string VOICE_FOLDER = Path.Combine(Application.StartupPath, ConfigurationManager.AppSettings["Inventec.Speech.Voice.Folder"]);
        public const string URI = "https://code.responsivevoice.org/";
        public const string REQUEST_PARAMS = "getvoice.php?t=%27{0}%27&tl=vi&sv=g1&vn=&pitch=0.5&rate=0.5&vol=1&gender=female";
        public static int TIME_OUT = int.Parse(ConfigurationManager.AppSettings["Inventec.Speech.Voice.TimeOut"]);
        public static string GOOGLE_URI = ConfigurationManager.AppSettings["Inventec.Speech.Voice.TranslateUri"] ?? "https://translate.google.com/";
        public static string GOOGLE_REQUEST_PARAMS = ConfigurationManager.AppSettings["Inventec.Speech.Voice.RequestParams"] ?? "translate_tts?ie=UTF-8&tl=vi&client=tw-ob&q={0}";
    }
}
