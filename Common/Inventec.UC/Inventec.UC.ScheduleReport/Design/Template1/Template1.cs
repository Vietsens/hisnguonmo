using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ScheduleReport.Base;
using DevExpress.Utils;

namespace Inventec.UC.ScheduleReport.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        int positionHandleControl = -1;

        private GetSarReportSDOByDelegate _GetReport;
        private ProcessHasException _HasException;

        private WaitDialogForm waitLoad;

        public Template1( Data.DataInit data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SarConsumer = data.sarConsumer;
                TokenClientStore.ClientTokenManager = data.clientToken;
                this._GetReport = data.getReport;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
