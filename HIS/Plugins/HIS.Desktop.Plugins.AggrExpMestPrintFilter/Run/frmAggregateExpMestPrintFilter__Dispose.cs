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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter
{
    internal partial class frmAggregateExpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isPrint = false;
                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                printType = 0;
                reqRoomIds = null;
                useFormIds = null;
                serviceUnitIds = null;
                departmentId = 0;
                currrentModule = null;
                department = null;
                aggrExpMest = null;
                _AggrExpMests = null;
                MedicineUseForms = null;
                ServiceUnits = null;
                RoomDTO3s = null;
                RoomDTO2s = null;
                _ExpMestMatyReqList = null;
                _ExpMestMetyReqList = null;
                _ExpMestMedicines = null;
                _ExpMestMaterials = null;
                _ExpMests_Print = null;
                IsPrintMps169 = false;
                configKeyOderOption = 0;
                configKeyMERGER_DATA = 0;
                _ExpMestMedi_Others = null;
                _ExpMestMedi_HCHT = null;
                _ExpMestMedi_HCGN = null;
                _ExpMestMedi_PXs = null;
                _ExpMestMedi_TDs = null;
                _ExpMestMedi_HTs = null;
                _ExpMestMedi_GNs = null;
                _ExpMestMedi_Ts = null;
                print.DisposePrintNow();
                HIS.Desktop.Plugins.AggrExpMestPrintFilter.Run.Print.DisposePrint();
                print = null;
                printProcess = null;
                adodata = null;
                CancelPrint = false;
                CountMediMatePrinted = 0;
                TotalMediMatePrint = 0;
                GroupStreamPrint = null;
                richEditorMain = null;
                this.chkPrintNow.CheckedChanged -= new System.EventHandler(this.chkPrintNow_CheckedChanged);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.btnSendRequest.Click -= new System.EventHandler(this.btnSendRequest_Click);
                this.gridViewUseForm.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewUseForm_SelectionChanged);
                this.gridViewServiceUnit.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewServiceUnit_SelectionChanged);
                this.gridViewRoom.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridView3_SelectionChanged);
                this.barButtonItem3.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemClick);
                this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.frmAggregateExpMestPrintFilter_FormClosing);
                this.Load -= new System.EventHandler(this.frmRequestPatientReport_Load);
                gridViewServiceUnit.GridControl.DataSource = null;
                gridControlServiceUnit.DataSource = null;
                gridViewUseForm.GridControl.DataSource = null;
                gridControlUseForm.DataSource = null;
                gridView2.GridControl.DataSource = null;
                gridControl2.DataSource = null;
                gridView1.GridControl.DataSource = null;
                gridControl1.DataSource = null;
                gridViewRoom.GridControl.DataSource = null;
                grdRoom.DataSource = null;
                lciChkPrintNow = null;
                chkPrintNow = null;
                layoutControlItem3 = null;
                layoutControlItem1 = null;
                dtIntructionTimeFrom = null;
                dtIntructionTimeTo = null;
                emptySpaceItem6 = null;
                lciChkIsChemicalSubstance = null;
                chkIsChemicalSustance = null;
                gridColumn17 = null;
                gridColumn16 = null;
                gridColumn15 = null;
                gridColumn3 = null;
                layoutControlItem6 = null;
                layoutControlItem4 = null;
                gridViewServiceUnit = null;
                gridControlServiceUnit = null;
                gridViewUseForm = null;
                gridControlUseForm = null;
                lciChkMaterial = null;
                lciChkMedicine = null;
                chkMedicine = null;
                chkMaterial = null;
                emptySpaceItem1 = null;
                layoutControlItem5 = null;
                layoutControlItem44 = null;
                layoutControlItem43 = null;
                layoutControlItem42 = null;
                layoutControlItem41 = null;
                layoutControlGroup20 = null;
                layoutControlItem40 = null;
                layoutControlItem39 = null;
                layoutControlItem38 = null;
                layoutControlGroup19 = null;
                layoutControlItem37 = null;
                layoutControlGroup18 = null;
                repositoryItemCheckEdit3 = null;
                gridColumn14 = null;
                gridColumn13 = null;
                gridColumn12 = null;
                gridColumn11 = null;
                gridColumn10 = null;
                gridView2 = null;
                gridControl2 = null;
                layoutControl20 = null;
                layoutControlItem36 = null;
                layoutControlItem35 = null;
                layoutControlGroup17 = null;
                dateEdit4 = null;
                dateEdit3 = null;
                layoutControl19 = null;
                layoutControlItem34 = null;
                layoutControlGroup16 = null;
                repositoryItemRadioGroup2 = null;
                repositoryItemCheckEdit2 = null;
                repositoryItemRadioGroup1 = null;
                gridColumn9 = null;
                gridColumn8 = null;
                gridColumn7 = null;
                gridColumn6 = null;
                gridView1 = null;
                gridControl1 = null;
                layoutControl18 = null;
                layoutControl17 = null;
                layoutControlItem33 = null;
                emptySpaceItem5 = null;
                layoutControlItem32 = null;
                layoutControlItem31 = null;
                layoutControlItem30 = null;
                layoutControlItem29 = null;
                layoutControlItem28 = null;
                layoutControlItem27 = null;
                layoutControlGroup15 = null;
                textEdit4 = null;
                textEdit3 = null;
                lookUpEdit2 = null;
                textEdit2 = null;
                labelControl2 = null;
                textEdit1 = null;
                lookUpEdit1 = null;
                layoutControl16 = null;
                emptySpaceItem4 = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                layoutControlGroup14 = null;
                simpleButton2 = null;
                simpleButton1 = null;
                layoutControl15 = null;
                emptySpaceItem3 = null;
                layoutControlGroup13 = null;
                layoutControl14 = null;
                layoutControl13 = null;
                barDockControl4 = null;
                barDockControl3 = null;
                barDockControl2 = null;
                barDockControl1 = null;
                barButtonItem3 = null;
                bar2 = null;
                barManager2 = null;
                dxValidationProvider1 = null;
                dxErrorProvider1 = null;
                layoutControlItem12 = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn1 = null;
                gridViewRoom = null;
                grdRoom = null;
                layoutControlItem2 = null;
                layoutControlItem7 = null;
                layoutControlGroup5 = null;
                layoutControlGroup8 = null;
                layoutControl9 = null;
                layoutControl6 = null;
                lblDescription = null;
                emptySpaceItem2 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                barManager1 = null;
                btnSendRequest = null;
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
