/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
