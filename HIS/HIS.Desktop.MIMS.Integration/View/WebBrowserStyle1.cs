using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.MIMS.Integration.View
{
    public partial class WebBrowserStyle1 : Form
    {
        public WebBrowserStyle1()
        {
            InitializeComponent();
        }

        public WebBrowserStyle1(string html, string title = "MIMS", Int32? width = 900, Int32? height = 700)
        {
            InitializeComponent();

            this.Text = title;
            if (width.HasValue) this.Width = width.Value;
            if (height.HasValue) this.Height = height.Value;

            webBrowser1.DocumentText = html;
        }
    }
}
