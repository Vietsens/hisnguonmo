using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Development.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        public Template1(string contentDonVi, string contentPhatTrien)
        {
            InitializeComponent();
            this.txtDonVi.Text = contentDonVi;
            this.labelControl1.Text = contentPhatTrien;
        }
    }
}
