using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;

namespace Inventec.UC.ChangePassword.Design.Template2
{
    internal partial class Template2 : UserControl
    {

        private ChangePasswordSuccess _ChangeSuccess;
        private HasExceptionApi _HasException;

        private WaitDialogForm waitLoad;

        public Template2(Data.DataInitChangePass Data)
        {
            InitializeComponent();
            Inventec.UC.ChangePassword.Process.ApiConsumerStore.SdaConsumer = Data.sdaConsumer;
            Inventec.UC.ChangePassword.Process.TokenClient.clientTokenManager = Data.clientTokenManager;
        }

        private void Template2_Load(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
