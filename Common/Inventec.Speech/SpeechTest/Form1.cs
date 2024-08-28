using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Globalization;
using Inventec.Speech;

namespace SpeechTest
{
    public class Test
    {
        public string value { get; set; }
        public string description { get; set; }
        public Test(string s)
        {
            value = s;
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            Test t1 = new Test("a");
            Test t2 = new Test("b");
            Test t3 = new Test("c");
            List<Test> t = new List<Test>() { t1, t2, t3 };
            foreach (Test a in t)
            {
                this.decorator(a);
            }
            InitializeComponent();
        }

        private void decorator(Test t)
        {
            t.description = "x";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbMoiBenhNhan.Text))
            {
                SpeechPlayer.SpeakSingle(tbMoiBenhNhan.Text);
            }
            if (!string.IsNullOrWhiteSpace(tbTenBn.Text))
            {
                SpeechPlayer.Speak(tbTenBn.Text);
            }
            if (!string.IsNullOrWhiteSpace(tbCoSoTT.Text))
            {
                SpeechPlayer.SpeakSingle(tbCoSoTT.Text);
            }
            if (!string.IsNullOrWhiteSpace(tbStt.Text))
            {
                SpeechPlayer.Speak(Int32.Parse(tbStt.Text));
            }
            if (!string.IsNullOrWhiteSpace(tbDen.Text))
            {
                SpeechPlayer.SpeakSingle(tbDen.Text);
            }
            if (!string.IsNullOrWhiteSpace(tbTenPhong.Text))
            {
                SpeechPlayer.Speak(tbTenPhong.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
