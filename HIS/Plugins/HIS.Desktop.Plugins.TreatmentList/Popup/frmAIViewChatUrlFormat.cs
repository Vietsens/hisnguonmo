using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList.Popup
{
    public partial class frmAIViewChatUrlFormat: Form
    {
        public frmAIViewChatUrlFormat(string uri)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(uri))
            {
                wbAIViewChatUrlFormat.ScriptErrorsSuppressed = true;
                wbAIViewChatUrlFormat.Navigate(uri);
            }
        }
    }
}
