using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboCommune.Data;

namespace Inventec.UC.ComboCommune.Design.Template1
{
    public partial class Template1 : UserControl
    {
        private SetValueComboTHX _SetValueTHX;
        private SetFocusNextControl _FocusControlNext;
        private GetValueComboProvince _ValueProvince;
        private GetValueComboDistrict _ValueDistrict;

        private DataInitCommune DataCommune = new DataInitCommune();
        private List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listData = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();

        public Template1(DataInitCommune Data)
        {
            InitializeComponent();
            this.DataCommune = Data;
            this.listData = Data.ListCommune;
        }

    }
}
