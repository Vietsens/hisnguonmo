using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboNational.Data;

namespace Inventec.UC.ComboNational.Design.Template1
{
    public partial class Template1 : UserControl
    {
        private FocusNextControl _FocusNext;

        private DataInitNational DataQuocTich;
        private List<SDA.EFMODEL.DataModels.SDA_NATIONAL> listNational;
        private SDA.EFMODEL.DataModels.SDA_NATIONAL defaultNational;

        public Template1(DataInitNational Data)
        {
            InitializeComponent();
            this.DataQuocTich = Data;
            this.listNational = Data.listData;
            LoadDataToComboQuocGia();
        }

    }
}
