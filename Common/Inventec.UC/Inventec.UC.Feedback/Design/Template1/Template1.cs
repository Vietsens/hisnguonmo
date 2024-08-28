using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;

namespace Inventec.UC.Feedback.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private HasExceptionApi _HasException;
        private CloseForm _Close;

        int positionHandleControl = -1;

        public Template1(Data.DataInitFeedback Data)
        {
            InitializeComponent();
            Inventec.UC.Feedback.Process.ApiConsumerStore.SdaConsumer = Data.sdaConsumer;
            Inventec.UC.Feedback.Process.TokenClient.clientTokenManager = Data.clientTokenManager;
        }
    }
}
