using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Login.UCD;
using Inventec.UC.Login.Base;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private LoginInfor _LoginInfor;
        private EventButtonConfig _BtnConfig_Click;

        public Template1(InitUCD data)
        {
            InitializeComponent();
            ApiConsumerStore.SdaConsumer = data.sdaCosumer;
            Inventec.UC.Login.Base.RegistryConstant.SYSTEM_FOLDER = data.SYSTEM_FOLDER;
            Inventec.UC.Login.Base.RegistryConstant.APP_FOLDER = data.APP_FOLDER;
            AppConfig.APPLICATION_CODE = data.APPLICATION_CODE;
        }

    }
}
