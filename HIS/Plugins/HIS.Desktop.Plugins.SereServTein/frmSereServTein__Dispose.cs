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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SereServTein
{
    public partial class frmSereServTein : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                lstSereServ = null;
                _SereServNumOders = null;
                lstHisSereServTeinSDO = null;
                lstSereServTein = null;
                HisSereServTeinSDOs = null;
                currentSereServFiles = null;
                currentSereServ = null;
                currentServiceReq = null;
                currentModule = null;
                sereServExt = null;
                currentSarPrint = null;
                imageLoad = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                controlStateWorker = null;
                currentControlStateRDO = null;
                moduleLink = null;
                this.btnVanBanDaKy.Click -= new System.EventHandler(this.btnVanBanDaKy_Click);
                this.chkInTach.CheckedChanged -= new System.EventHandler(this.chkInTach_CheckedChanged);
                this.barButtonItem__Print.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Print_ItemClick);
                this.gridViewSereServTein.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewSereServTein_RowCellStyle_1);
                this.gridViewSereServTein.RowStyle -= new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewSereServTein_RowStyle);
                this.gridViewSereServTein.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewSereServTein_CustomRowCellEdit);
                this.gridViewSereServTein.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSereServTein_CustomUnboundColumnData);
                this.gridViewSereServTein.CustomColumnDisplayText -= new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewSereServTein_CustomColumnDisplayText);
                this.repositoryItemCheckEdit_Enable.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckEdit_Enable_CheckedChanged);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.btnPrintServiceReq.Click -= new System.EventHandler(this.btnPrintServiceReq_Click);
                this.BtnEmr.Click -= new System.EventHandler(this.BtnEmr_Click);
                this.Load -= new System.EventHandler(this.frmSereServTein_Load);
                PacsCFG.DisposePacs();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
