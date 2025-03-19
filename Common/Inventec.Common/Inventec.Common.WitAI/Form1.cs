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
using Inventec.Common.WitAI.Vitals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Inventec.Common.WitAI
{
    public partial class Form1 : Form
    {
        Action<string> actSave;
        // O_NLP.RootObject is a class that contains the data interpreted from wit.ai
        Objects.O_NLP.RootObject oNLP = new Objects.O_NLP.RootObject();

        // NLP_Processing is the code that processes the response from wit.ai
        Vitals.NLP.NLP_Processing vitNLP = new Vitals.NLP.NLP_Processing();

        // Winmm.dll is used for recording speech
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        // Variables used for speech recording
        private bool recording = false;
        private string speechfilename = "";
        // Set a timer to make sure recording doesn't exceed 10 seconds
        private System.Timers.Timer speechTimer = new System.Timers.Timer();

        public Form1(Action<string> __actSave)
        {
            InitializeComponent();

            speechTimer = new System.Timers.Timer();
            speechTimer.Elapsed += new ElapsedEventHandler(OnTimedSpeechEvent);
            speechTimer.Interval = Inventec.Common.WitAI.Vitals.Constan.TimeReplay;
            this.actSave = __actSave;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                tbContent.Focus();
                RecognizeStart();
                if (RegconizeWorker.GetRegconizeVoice())
                {
                    chkRepeat.Checked = true;
                }
                speechTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Async + wait keeps the GUI thread responsive
        public async void StartProcessing(string text, int type)
        {
            try
            {
                string modtext = Vitals.NLP.Pre_NLP_Processing.preprocessText(text);

                string nlp_text = "";

                if (type == 0)
                {
                    nlp_text = await vitNLP.ProcessWrittenText(modtext);
                }
                else
                {
                    nlp_text = await vitNLP.ProcessSpokenText(text);
                }

                // If the audio file doesn't contain anything, or wit.ai doesn't understand it, a code 400 will be returned
                if (nlp_text.Contains("The remote server returned an error: (400) Bad Request"))
                {
                    lblYou.Text = "Sorry, didn't get that. Could you please repeat yourself?";
                    //btnSave.Enabled = true;
                    //tbContent.Enabled = true;
                    lblYou.Text += nlp_text;
                    return;
                }

                lblYou.Text = nlp_text;

                oNLP = Vitals.NLP.Post_NLP_Processing.ParseData(nlp_text);

                // This codeblock dynamically casts the intent to the corresponding class
                // Check README.txt in Vitals.Brain
                string sentence = "";
                if (oNLP != null)
                {
                    if (oNLP.entities != null && !String.IsNullOrEmpty(oNLP.entities.intent))
                    {
                        Assembly objAssembly;
                        objAssembly = Assembly.GetExecutingAssembly();

                        Type classType = objAssembly.GetType("Inventec.Common.WitAI.Vitals.Brain." + oNLP.outcome.entities);

                        object obj = Activator.CreateInstance(classType);

                        MethodInfo mi = classType.GetMethod("makeSentence");

                        object[] parameters = new object[1];
                        parameters[0] = oNLP;

                        mi = classType.GetMethod("makeSentence");
                        sentence = (string)mi.Invoke(obj, parameters);
                    }
                    else
                    {
                        sentence = oNLP._text;
                    }
                }

                // Show what was deducted from the sentence
                if (!String.IsNullOrEmpty(sentence))
                {
                    tbContent.Text += (String.IsNullOrEmpty(tbContent.Text) ? sentence : " " + sentence);
                }
                //else
                //{
                //    tbContent.Text += (" 123");
                //}

                if (RegconizeWorker.GetRegconizeVoice())
                {
                    RecognizeStart();
                }

                //btnSave.Enabled = true;
                //tbContent.Enabled = true;
            }
            catch (Exception ex)
            {
                //btnSave.Enabled = true;
                //tbContent.Enabled = true;

                lblYou.Text = "Sorry, no idea what's what. Try again later please!" + Environment.NewLine + Environment.NewLine + "I bumped onto this error: " + ex.Message;
            }
        }

        // After 10 seconds, this timer gets called if the user doesn't click 'stop'
        private void OnTimedSpeechEvent(object source, ElapsedEventArgs e)
        {
            if (!chkRepeat.Checked)
                speechTimer.Enabled = false;

            if (!recording)
            {
                RecognizeStart();
                return;
            }

            // Replace with this.InvokeRequired for WinForms
            // Contact me at sam@tabnw.org if you need help
            if (this.InvokeRequired)
            {
                //Dispatcher.Invoke(() => OnTimedSpeechEvent(source, e));
                this.Invoke(new MethodInvoker(delegate { OnTimedSpeechEvent(source, e); }));
                return;
            }
            else
            {
                recording = false;
                mciSendString("pause recsound", null, 0, 0);
                mciSendString("save recsound " + speechfilename, null, 0, 0);
                mciSendString("close recsound ", null, 0, 0);

                //Computer c = new Computer();
                //c.Audio.Stop();

                //btnSave.Text = "record";
                //btnSave.Enabled = false;
                //tbContent.Enabled = false;

                StartProcessing(speechfilename, 1);

                lblYou.Text = "";
                tbContent.Text = "";
                lblYou.Text = "Recording...";
                tbContent.Focus();
            }
        }

        // Generate temp random string name
        // Thanks RCIX @ stackoverflow.com :)
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private void btnSaveContent_Click(object sender, EventArgs e)
        {
            if (this.actSave != null)
            {
                this.actSave(tbContent.Text);
            }
            this.Close();
        }

        private void tbYou_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbContent.Text.Length > 0)
            {
                //btnSave.Enabled = false;
                //tbContent.Enabled = false;

                StartProcessing(tbContent.Text, 0);

                lblYou.Text = "You said: " + tbContent.Text;
                tbContent.Text = "";
                lblYou.Text = "Recording...";
                tbContent.Focus();
            }
        }

        private void btnCopyToClipboast_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbContent.Text);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbContent.Text = "";
            lblYou.Text = "";
            btnSaveContent.Enabled = true;
            tbContent.Enabled = true;
            recording = false;

            RecognizeStart();

            if (RegconizeWorker.GetRegconizeVoice())
            {
                if (!chkRepeat.Checked) chkRepeat.Checked = true;
                speechTimer.Enabled = true;
            }
            tbContent.Focus();
        }

        private void chkRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRepeat.Checked)
            {
                RegconizeWorker.ChangeRegconizeVoice(true);
                recording = false;
                RecognizeStart();
                speechTimer.Enabled = true;
            }
            else
            {
                RegconizeWorker.ChangeRegconizeVoice(false);
                speechTimer.Enabled = false;
            }
        }

        private void RecognizeStart()
        {
            try
            {
                if (!recording)
                {
                    recording = true;
                    string tempfile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), RandomString(8));
                    speechfilename = tempfile;

                    mciSendString("open new Type waveaudio Alias recsound", null, 0, 0);
                    mciSendString("record recsound", null, 0, 0);

                    speechTimer.Enabled = true;
                    lblYou.Text = "Recording...";
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
