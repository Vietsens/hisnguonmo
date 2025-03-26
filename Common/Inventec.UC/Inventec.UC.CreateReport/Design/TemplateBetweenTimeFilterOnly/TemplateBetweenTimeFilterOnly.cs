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
