using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboProvince.Data;

namespace Inventec.UC.ComboProvince.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private LoadComboDistrictFromProvince _LoadHuyenFromTinh;
        private SetValueCboDistrictAndCboCommune _SetValue;
        private SetFocusCboDistrict _FocusCboHuyen;

        private DataInitProcinve Data;
        private List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listData;

        public Template1(DataInitProcinve Data)
        {
            InitializeComponent();
            this.Data = Data;
            this.listData = Data.listProvince;
            LoadDataToCombo();
        }
    }
}
