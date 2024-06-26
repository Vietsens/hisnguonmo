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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggregate.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggregate
{
    public partial class UCExpMestAggregate : UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                moduleLink = null;
                controlStateWorker = null;
                currentControlStateRDO = null;
                isCheckAll = false;
                lstBedRoomIds = null;
                lstRoomSelectedId = null;
                _lstCurrentBedId = null;
                bedSelecteds = null;
                listBed = null;
                currentAggrExpMest = null;
                currentModule = null;
                dataTotalExpM = 0;
                rowCountExpM = 0;
                aggrExpMestId = 0;
                dataTotal = 0;
                rowCount = 0;
                lastColumn = null;
                lastInfo = null;
                lastRowHandle = 0;
                list_exp_mest_id = null;
                _ExpMestChecks = null;
                _ExpMest_PLs = null;
                _ExpMestADOs = null;
                _MediStockSelecteds = null;
                richEditorMain = null;
                department = null;
                lstExpMest = null;
                serviceUnitIds = null;
                reqRoomIds = null;
                useFormIds = null;
                _ExpMestMatyReqList = null;
                _ExpMestMetyReqList = null;
                _ExpMestMedicines = null;
                BedLogList = null;
                vHisTreatmentBedRooms = null;
                patients = null;
                this.chkThuocVTTheoPhieuXuat.CheckedChanged -= new System.EventHandler(this.chkThuocVTTheoPhieuXuat_CheckedChanged);
                this.btnInTraDoiTongHop.Click -= new System.EventHandler(this.btnInTraDoiTongHop_Click);
                this.gridViewAggrRequest.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestReq_CustomUnboundColumnData);
                this.chkPrint.CheckedChanged -= new System.EventHandler(this.chkPrint_CheckedChanged);
                this.gridControlAggrExpMest.Click -= new System.EventHandler(this.gridControlAggrExpMest_Click);
                this.gridViewAggrExpMest.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewAggrExpMest_CustomRowCellEdit);
                this.gridViewAggrExpMest.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewAggrExpMest_SelectionChanged);
                this.gridViewAggrExpMest.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewAggrExpMest_CustomUnboundColumnData);
                this.gridViewAggrExpMest.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewAggrExpMest_MouseDown);
                this.gridViewDetail.CustomDrawCell -= new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridViewDetail_CustomDrawCell);
                this.gridViewDetail.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDetail_CustomUnboundColumnData);
                this.gridViewExpMestReq.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewExpMestReq_RowCellClick);
                this.gridViewExpMestReq.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewExpMestReq_CustomRowCellEdit);
                this.gridViewExpMestReq.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewExpMestReq_SelectionChanged);
                this.gridViewExpMestReq.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestReq_CustomUnboundColumnData);
                this.gridViewExpMestReq.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewExpMestReq_MouseDown);
                this.repositoryItemCheck_E.CheckedChanged -= new System.EventHandler(this.repositoryItemCheck_E_CheckedChanged);
                this.btnAggrExpMest.Click -= new System.EventHandler(this.btnAggrExpMest_Click);
                this.btnRefresh.Click -= new System.EventHandler(this.btnRefresh_Click);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.cboMediStock.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediStock_Closed);
                this.cboMediStock.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboMediStock_CustomDisplayText);
                this.checkPresNormal.CheckedChanged -= new System.EventHandler(this.checkPresNormal_CheckedChanged);
                this.cboBed.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboBed_CustomDisplayText);
                this.cboPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientType_ButtonClick);
                this.cboPatientType.EditValueChanged -= new System.EventHandler(this.cboPatientType_EditValueChanged);
                this.txtKeywordProcess.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeywordProcess_KeyUp);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.UCExpMestAggregate_Load);
                gridView2.GridControl.DataSource = null;
                cboPatientType.Properties.DataSource = null;
                gridView1.GridControl.DataSource = null;
                cboBed.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboMediStock.Properties.DataSource = null;
                gridControlDetail.DataSource = null;
                gridControlExpMestReq.DataSource = null;
                gridViewExpMestReq.GridControl.DataSource = null;
                gridViewDetail.GridControl.DataSource = null;
                gridViewAggrExpMest.GridControl.DataSource = null;
                gridControlAggrExpMest.DataSource = null;
                gridViewAggrRequest.GridControl.DataSource = null;
                gridControlAggregateRequest.DataSource = null;
                emptySpaceItem1 = null;
                layoutControlItem30 = null;
                chkThuocVTTheoPhieuXuat = null;
                layoutControlItem29 = null;
                layoutControlGroup16 = null;
                gridView2 = null;
                cboPatientType = null;
                layoutControl17 = null;
                navBarGroup1 = null;
                navBarGroupControlContainer6 = null;
                gridView1 = null;
                cboBed = null;
                layoutControlItem28 = null;
                layoutControlGroup15 = null;
                layoutControl16 = null;
                navBarGroupControlContainer5 = null;
                navBed = null;
                layoutControlItem27 = null;
                chkPrint = null;
                layoutControlItem8 = null;
                checkIHomePres = null;
                navBarGroupTypePress = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                layoutControlGroup13 = null;
                checkPresNormal = null;
                checkPresKidney = null;
                layoutControl14 = null;
                navBarGroupControlContainer3 = null;
                layoutControlItem5 = null;
                btnInTraDoiTongHop = null;
                imageListIcon = null;
                repositoryItemCheck_D = null;
                repositoryItemCheck_E = null;
                gridColumnCheck = null;
                layoutControlItem26 = null;
                gridLookUpEdit1View = null;
                cboMediStock = null;
                navBarGroupMediStock = null;
                layoutControlGroup3 = null;
                layoutControl4 = null;
                navBarGroupControlContainer1 = null;
                repositoryItemButton__Delete_Disable = null;
                repositoryItemButton__Delete = null;
                gridColDelete = null;
                gridColEditPres = null;
                repositoryItemButtonEdit__Disable = null;
                repositoryItemButtonEdit__Pres = null;
                navIntructionTime = null;
                lciToIntructionTime = null;
                lciFromIntructionTime = null;
                layoutControlGroup14 = null;
                dtFromIntructionTime = null;
                dtToIntructionTime = null;
                layoutControl15 = null;
                navBarGroupControlContainer4 = null;
                imageCollection1 = null;
                repositoryItemButtonEdit_DeleteDisable = null;
                toolTipController1 = null;
                grdColMediSTT = null;
                repositoryItemPictureEditStatu = null;
                repositoryItemImageEditStatusss = null;
                repositoryItemPictureEditStatusExp = null;
                grdColExpReqStatus = null;
                repositoryItemPictureEditStatus = null;
                grdColExpStatus = null;
                navBarFilterProcess = null;
                navBarGroupControlContainerRoom = null;
                chkSynthesized = null;
                chkNotSynthetic = null;
                txtKeywordProcess = null;
                gridControlDetail = null;
                gridControlExpMestReq = null;
                layoutControlItem24 = null;
                ucPagingControlAggrExpMest = null;
                layoutControlItem25 = null;
                ucPagingAggregateRequest = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControlItem9 = null;
                layoutControlItem10 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                Root = null;
                navRoom = null;
                navStatus = null;
                lciCheckNotSynthetic = null;
                lciCheckSynthesized = null;
                layoutControlGroup4 = null;
                layoutControl5 = null;
                navBarGroupControlContainer2 = null;
                btnSearch = null;
                btnRefresh = null;
                layoutControl2 = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                layoutControlGroup2 = null;
                btnAggrExpMest = null;
                layoutControlGroup5 = null;
                layoutControl6 = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                layoutControlGroup6 = null;
                layoutControlItem17 = null;
                layoutControlItem16 = null;
                layoutControlGroup7 = null;
                layoutControlItem20 = null;
                layoutControlGroup9 = null;
                grdColPatientCode = null;
                grdColTreatmentCode = null;
                grdColIntructionTime = null;
                grdColReqUserName = null;
                grdColExpTime = null;
                grdColPatientName = null;
                grdColExecuteRoomName = null;
                grdColServiceReqCode = null;
                grdColStatus = null;
                gridViewExpMestReq = null;
                layoutControl10 = null;
                layoutControlItem21 = null;
                layoutControlGroup10 = null;
                grdColMediBidNumber = null;
                grdColMediAmount = null;
                grdColMediUnitName = null;
                grdColMediTypeName = null;
                grdColMediTypeCode = null;
                gridViewDetail = null;
                layoutControl11 = null;
                layoutControl8 = null;
                layoutControlItem19 = null;
                layoutControlItem18 = null;
                layoutControlGroup8 = null;
                layoutControlItem22 = null;
                layoutControlGroup11 = null;
                grdColExpUserName = null;
                grdColExpTimeDisplay = null;
                grdColExpCreator = null;
                grdColExpCreateTime = null;
                grdColExpStockName = null;
                grdColExpMestCode = null;
                repositoryItemButtonEdit_Print = null;
                grdColExpPrint = null;
                repositoryItemButtonEdit_Delete = null;
                grdColExpDelete = null;
                gridViewAggrExpMest = null;
                gridControlAggrExpMest = null;
                layoutControl12 = null;
                layoutControlItem23 = null;
                layoutControlGroup12 = null;
                grdColExpReqPatientCode = null;
                grdColExpReqTreatmentCode = null;
                grdColExpReqIntructionTime = null;
                grdColExpReqUserName = null;
                grdColExpReqUserTime = null;
                grdColExpReqPatientName = null;
                grdColExpReqRoomName = null;
                grdColExpReqCode = null;
                gridViewAggrRequest = null;
                gridControlAggregateRequest = null;
                layoutControl13 = null;
                layoutControl9 = null;
                layoutControl7 = null;
                layoutControl3 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
