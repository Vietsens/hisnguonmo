using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ServerConfig.Data;

namespace Inventec.UC.ServerConfig.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        List<DataShow> ListData = new List<DataShow>();
        List<Inventec.Common.XmlConfig.ElementNode> ListElement = new List<Common.XmlConfig.ElementNode>();

        private CloseFormConfigSystem _CloseForm;

        internal Template1(CloseFormConfigSystem Close)
        {
            InitializeComponent();
            this._CloseForm = Close;
        }
    }
}
