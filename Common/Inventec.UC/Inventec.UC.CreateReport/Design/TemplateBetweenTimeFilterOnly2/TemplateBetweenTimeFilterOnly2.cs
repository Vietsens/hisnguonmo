using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.CreateReport.Base;
using DevExpress.Utils;
using Inventec.UC.CreateReport.Config;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2
{
    internal partial class TemplateBetweenTimeFilterOnly2 : UserControl
    {
        private ProcessHasException _HasException;
        private CloseContainerForm _CloseContainerForm;
        private GetObjectFilter _GetFilter;

        internal List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> lstReportTemplate;
        private WaitDialogForm waitLoad;
        int positionHandleControl = -1;
        MRS.SDO.CreateReportSDO sarReport;

        public TemplateBetweenTimeFilterOnly2(Inventec.UC.CreateReport.Data.InitData data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SarConsumer = data.sarconsumer;
                ApiConsumerStore.MrsConsumer = data.mrsconsumer;
                TokenClientStore.ClientTokenManager = data.clientToken;
                Base.BusinessBase.TokenCheck();
                //Config.Loader.RefreshConfig();
                LoadDataToReportType(GlobalStore.reportType.REPORT_TYPE_CODE);
                LoadDataToCboReportTemplate(GlobalStore.reportType.ID);
                LoadDefaultDataFromTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
