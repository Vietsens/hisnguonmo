using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.CreateReport.Base;
using DevExpress.XtraNavBar;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using Inventec.UC.CreateReport.Config;
using DevExpress.XtraEditors;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly
{
    internal partial class TemplateBetweenTimeFilterOnly : UserControl
    {

        private ProcessHasException _HasException;
        private CloseContainerForm _CloseContainerForm;

        public TemplateBetweenTimeFilterOnly(Data.InitData data)
        {
            InitializeComponent();
            try
            {
                waitLoad = new WaitDialogForm(MessageUtil.GetMessage(MessageLang.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));
                ApiConsumerStore.SarConsumer = data.sarconsumer;
                ApiConsumerStore.MrsConsumer = data.mrsconsumer;
                TokenClientStore.ClientTokenManager = data.clientToken;
                Base.BusinessBase.TokenCheck();
                //Config.Loader.RefreshConfig();
                LoadDataToReportType(GlobalStore.reportType.REPORT_TYPE_CODE);
                LoadDataToReportTemplate(GlobalStore.reportType.ID);
                LoadDefaultDataFromTime();
                waitLoad.Dispose();
            }
            catch (System.Exception ex)
            {
                waitLoad.Dispose();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
