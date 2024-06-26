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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.CreateReport.ValidationReport;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public partial class frmMediCardByDateReport : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                MediMateId = null;
                RoomId = 0;
                _ReportFilter = null;
                mediStock = null;
                BloodType = null;
                MaterialType = null;
                MedicineType = null;
                positionHandleControl = 0;
                ReportTypeCode = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dtTimeTo.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeTo_Closed);
                this.dtTimeFrom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeFrom_Closed);
                this.barButtonItem__Save.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Save_ItemClick);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmMediCardByDateReport_Load);
                lciExpiredDate = null;
                lblExpiredDate = null;
                lciPackageNumber = null;
                lblPackageNumber = null;
                dxValidationProvider1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem__Save = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem2 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                btnSave = null;
                layoutControlItem10 = null;
                layoutControlItem9 = null;
                lciMediMateName = null;
                lciMediMateCode = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                lblMediStockCode = null;
                lblMediStockName = null;
                lblReportTypeCode = null;
                lblReportTypeName = null;
                dtTimeFrom = null;
                dtTimeTo = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
