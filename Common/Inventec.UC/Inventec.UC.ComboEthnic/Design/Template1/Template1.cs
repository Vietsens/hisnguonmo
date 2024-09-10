using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboEthnic.Data;

namespace Inventec.UC.ComboEthnic.Design.Template1
{
    public partial class Template1 : UserControl
    {

        private FocusNextControl _FocusNext;

        private DataInitEthnic Data;
        private List<SDA.EFMODEL.DataModels.SDA_ETHNIC> listEthnic;
        private SDA.EFMODEL.DataModels.SDA_ETHNIC defaultEthnic;

        public Template1(Data.DataInitEthnic Data)
        {
            InitializeComponent();
            this.Data = Data;
            this.listEthnic = Data.listDanToc;
            LoadDataToComboDanToc();
        }

    }
}
