using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboDistrict.Data;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1 : UserControl
    {
        private LoadComboCommuneFromDistrict _LoadComboCommune;
        private SetValueComboCommune _SetValueCommune;
        private SetFocusComboCommune _FocusComboCommune;
        private GetValueComboProvince _GetValueProvince;

        private DataInitDistrict DataDistrict;
        private List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listData;

        public Template1(DataInitDistrict Data)
        {
            InitializeComponent();
            this.DataDistrict = Data;
            this.listData = Data.listDistrict;
        }

    }
}
