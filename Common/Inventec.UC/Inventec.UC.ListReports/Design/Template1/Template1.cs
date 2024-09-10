using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ListReports.Base;
using DevExpress.XtraNavBar;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using Inventec.UC.ListReports.Config;

namespace Inventec.UC.ListReports.Design.Template1
{
    internal partial class Template1 : UserControl, IFormCallBack
    {
        public PagingGrid pagingGrid;
        private NavBarGroup navBarGroupReportListStt;
        private WaitDialogForm waitLoad;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        int rowCount = 0;

        private ProcessHasException _HasException;

        private List<SAR.EFMODEL.DataModels.V_SAR_REPORT> ListReport = new List<SAR.EFMODEL.DataModels.V_SAR_REPORT>();

        public Template1(Data.InitData data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SarConsumer = data.sarconsumer;
                ApiConsumerStore.SdaConsumer = data.sdaconsumer;
                ApiConsumerStore.AcsConsumer = data.acsconsumer;
                TokenClientStore.ClientTokenManager = data.clientToken;
                GlobalStore.NumberPage = data.numPage <= 0 ? 100 : data.numPage;
                GlobalStore.pathFileIcon = data.nameIcon;
                GlobalStore.isAdmin = data.isAdmin;
                Base.BusinessBase.TokenCheck();
                pagingGrid = new PagingGrid();
                //Config.Loader.RefreshConfig();
                gridControlListReports.DataSource = ListReport;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
