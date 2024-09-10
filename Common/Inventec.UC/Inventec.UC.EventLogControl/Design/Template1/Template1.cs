using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.EventLogControl.Base;

namespace Inventec.UC.EventLogControl.Design.Template1
{
    internal partial class Template1 : UserControl, Inventec.UC.EventLogControl.Base.IFormCallBack
    {

        PagingGrid pagingGrid;
        DevExpress.Utils.WaitDialogForm waitLoad;

        private List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG> ListEventLog = new List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>();
        private ProcessHasException _HasException;
        Data.DataInit currentData;

        public Template1(Data.DataInit data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SdaConsumer = data.sdaComsumer;
                GlobalStore.NumPageSize = data.pageNum <= 0 ? 100 : data.pageNum;
                currentData = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
