using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ChangePassword.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Utils;
using Inventec.Core;
using Inventec.UC.ChangePassword.Process;

namespace Inventec.UC.ChangePassword.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private HasExceptionApi _HasException;
        private ChangePasswordSuccess _ChangeSuccess;

        private WaitDialogForm waitLoad;

        public Template1(Data.DataInitChangePass Data)
        {
            InitializeComponent();
            ApiConsumerStore.SdaConsumer = Data.sdaConsumer;
            TokenClient.clientTokenManager = Data.clientTokenManager;
        }

        private void Template1_Load(object sender, EventArgs e)
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
