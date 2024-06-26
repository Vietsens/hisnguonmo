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
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using AutoMapper;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using MPS.ADO.TrackingPrint;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class UCBedRoomPartial : UserControlBase
    {

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                IsExpandList = false;
                this.ucViewEmrDocumentReq.ReloadDocument(new List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>());
                this.ucViewEmrDocumentResult.ReloadDocument(new List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>());
                ucAll.DisposeData();
                ucMediMate.DisposeData();
                ucCLS.DisposeData();
                ucOrther.DisposeData();
                ucAll = null;
                ucMediMate = null;
                ucCLS = null;
                ucOrther = null;
                currentBedRoom = null;
                wkRoomTypeId = 0;
                wkRoomId = 0;
                _SereServADORightMouseClick = null;
                IsExpand = false;
                DepartmentID = 0;
                histreatment = null;
                bedRoomAlls = null;
                patientClassifyFilterSelecteds = null;
                bedRoomFilterSelecteds = null;
                executeRoomSelecteds = null;
                lstModuleLinkApply = null;
                IsFormClosingOption = false;
                _TreatmentBedRoomADOs = null;
                ModuleLinkName = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                lastInfo = null;
                lastColumn = null;
                lastRowHandle = 0;
                pageIndex = 0;
                pageSize = 0;
                limit = 0;
                start = 0;
                dataTotal = 0;
                rowCount = 0;
                TreeClickData = null;
                emrMenuPopupProcessor = null;
                bedRoomPopupMenuProcessor = null;
                rowClickByDate = null;
                RowCellClickBedRoom = null;
                treatmentBedRoomRow = null;
                isUseAddedTime = false;
                treatmentCode = null;
                treatmentId = 0;
                currentModule = null;
                OrderBy = false;
                this.btnBedHistory.Click -= new System.EventHandler(this.btnBedHistory_Click);
                this.btnKeDonYHCT.Click -= new System.EventHandler(this.btnKeDonYHCT_Click);
                this.btnInToDieuTri.Click -= new System.EventHandler(this.btnInToDieuTri_Click);
                this.btnHoiChan.Click -= new System.EventHandler(this.btnHoiChan_Click);
                this.btnTuTruc.Click -= new System.EventHandler(this.btnTuTruc_Click);
                this.btnDanhSachYeuCau.Click -= new System.EventHandler(this.btnDanhSachYeuCau_Click);
                this.btnChiDinhDichVu.Click -= new System.EventHandler(this.btnChiDinhDichVu_Click);
                this.btnBangKe.Click -= new System.EventHandler(this.btnBangKe_Click);
                this.btnKeDonThuoc.Click -= new System.EventHandler(this.btnKeDonThuoc_Click);
                this.btnChiDinhMau.Click -= new System.EventHandler(this.btnChiDinhMau_Click);
                this.btnKetThucDieuTri.Click -= new System.EventHandler(this.btnKetThucDieuTri_Click);
                this.btnChuyenKhoa.Click -= new System.EventHandler(this.btnChuyenKhoa_Click);
                this.layoutControlTreeSereServ.GroupExpandChanged -= new DevExpress.XtraLayout.Utils.LayoutGroupEventHandler(this.layoutControlTreeSereServ_GroupExpandChanged);
                this.btnThuGon.Click -= new System.EventHandler(this.btnThuGon_Click);
                this.xtraTabDocument.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabDocument_SelectedPageChanged);
                this.cboFilterByDepartment.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboFilterByDepartment_Closed);
                this.treeListDateTime.FocusedNodeChanged -= new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeListDateTime_FocusedNodeChanged);
                this.cboPATIENT_CLASSIFY.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboPATIENT_CLASSIFY_CustomDisplayText);
                this.cboBedRoomSelect.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboBedRoomSelect_CustomDisplayText);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.txtKeyWord.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
                this.gridViewTreatmentBedRoom.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewTreatmentBedRoom_RowClick);
                this.gridViewTreatmentBedRoom.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewTreatmentBedRoom_RowCellClick);
                this.gridViewTreatmentBedRoom.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridViewTreatmentBedRoom_CustomDrawGroupRow);
                this.gridViewTreatmentBedRoom.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewTreatmentBedRoom_RowCellStyle);
                this.gridViewTreatmentBedRoom.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewTreatmentBedRoom_CustomRowCellEdit);
                this.gridViewTreatmentBedRoom.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewTreatmentBedRoom_PopupMenuShowing);
                this.gridViewTreatmentBedRoom.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatmentBedRoom_CustomUnboundColumnData);
                this.gridViewTreatmentBedRoom.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewTreatmentBedRoom_MouseDown);
                this.repositoryItemBtnMedisoft.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnMedisoft_ButtonClick);
                this.cboPatientFilter.EditValueChanged -= new System.EventHandler(this.cboPatientFilter_EditValueChanged);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.UCBedRoomPartial_Load);
                this.Leave -= new System.EventHandler(this.UCBedRoomPartial_Leave);
                gridView4.GridControl.DataSource = null;
                cboTreatmentStatus.Properties.DataSource = null;
                gridView3.GridControl.DataSource = null;
                cboTreatmentType.Properties.DataSource = null;
                gridView1.GridControl.DataSource = null;
                cboPATIENT_CLASSIFY.Properties.DataSource = null;
                gridView2.GridControl.DataSource = null;
                cboBedRoomSelect.Properties.DataSource = null;
                cboPatientFilter.Properties.DataSource = null;
                gridViewTreatmentBedRoom.GridControl.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboHasCoTreatment.Properties.DataSource = null;
                gridControlTreatmentBedRoom.DataSource = null;
                timer1 = null;
                notifyIcon1 = null;
                gridColumn8 = null;
                gridColumn9 = null;
                layoutControlItem45 = null;
                layoutControlItem44 = null;
                lblTreatmentEndType = null;
                lblTreatmentMethod = null;
                layoutControlItem43 = null;
                gridView4 = null;
                cboTreatmentStatus = null;
                layoutControlItem42 = null;
                chkOBSERVED = null;
                repositoryItemTextEditTDL_PATIENT_NAME_STR_NO = null;
                repositoryItemTextEditTDL_PATIENT_NAME_STR = null;
                layoutControlItem36 = null;
                gridView3 = null;
                cboTreatmentType = null;
                gridColumn7 = null;
                repositoryItemButtonIS_EMERGENCY_STR = null;
                gridColumn6 = null;
                gridColumn5 = null;
                layoutControlItem13 = null;
                treeColDateTime = null;
                treeListDateTime = null;
                layoutControlItem41 = null;
                panelControl4 = null;
                xtraTabPage4 = null;
                panelControl3 = null;
                xtraTabPage3 = null;
                panelControl2 = null;
                xtraTabPage2 = null;
                panelControl1 = null;
                xtraTabPage1 = null;
                xtraTabControl1 = null;
                layoutControlItem39 = null;
                gridView1 = null;
                cboPATIENT_CLASSIFY = null;
                layoutControlItem23 = null;
                lblHeinMediOrgCode = null;
                layoutControlItem38 = null;
                lblHanTu = null;
                layoutControlItem27 = null;
                pictureEditAvatar = null;
                layoutControlItem40 = null;
                gridView2 = null;
                cboBedRoomSelect = null;
                grdColRoomName = null;
                layoutControlItem37 = null;
                chkKhongHienThiKTT = null;
                toolTipController2 = null;
                cboFilterByDepartment = null;
                lciFilterByDepartment = null;
                layoutControlItem19 = null;
                emptySpaceItem1 = null;
                btnThuGon = null;
                cboPatientFilter = null;
                layoutControlItem35 = null;
                layoutControlItem34 = null;
                layoutControlItem33 = null;
                dtFrom = null;
                dtTo = null;
                layoutControlItem32 = null;
                LciGroupEmrDocument1 = null;
                ucViewEmrDocumentResult = null;
                ucViewEmrDocumentReq = null;
                xtraTabDocumentResult = null;
                xtraTabDocumentReq = null;
                xtraTabDocument = null;
                repositoryItemButton_IsUse = null;
                lciLbTreatDoctor = null;
                lbTreatDoctor = null;
                gridViewTreatmentBedRoom = null;
                toolTipController1 = null;
                repositoryItemButton_White = null;
                repositoryItemButton_Come = null;
                repositoryItemButton_Leave = null;
                GrdColIcon = null;
                gridColumn4 = null;
                gridColumn2 = null;
                gridColumn3 = null;
                gridColumn1 = null;
                repositoryItemBtnMedisoft = null;
                gc_MedisoftH = null;
                gc_ClassifyName = null;
                lciApprovalNote = null;
                lblApprovalNote = null;
                layoutControlItem31 = null;
                gridLookUpEdit1View = null;
                cboHasCoTreatment = null;
                layoutControlItem21 = null;
                lblTotalPatietInfo = null;
                grdColAddTime = null;
                layoutControlItem18 = null;
                btnBedHistory = null;
                layoutControlItem6 = null;
                btnKeDonYHCT = null;
                grdColPatientCode = null;
                layoutControlItem30 = null;
                btnInToDieuTri = null;
                layoutControlItem29 = null;
                btnHoiChan = null;
                layoutControlItem28 = null;
                layoutControlItem20 = null;
                lblIcdName = null;
                lblIcdText = null;
                lciTreatmentType = null;
                layoutControlItem7 = null;
                lblHeinCardNumber = null;
                lblTreatmentType = null;
                layoutControlItem10 = null;
                btnTuTruc = null;
                layoutControlItem8 = null;
                btnDanhSachYeuCau = null;
                layoutControlItem9 = null;
                btnChiDinhDichVu = null;
                repositoryEditServiceReq__Disable = null;
                repositoryEditServiceReq__Enable = null;
                repositoryItemPictureEdit1 = null;
                repositoryItemButtonEdit1 = null;
                repositoryItemButton__Send__Disable = null;
                repositoryItemButton__Send = null;
                repositoryItemTextEdit1 = null;
                layoutControlItem17 = null;
                btnBangKe = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                layoutControlItem24 = null;
                layoutControlItem22 = null;
                btnChuyenKhoa = null;
                btnKetThucDieuTri = null;
                btnChiDinhMau = null;
                btnKeDonThuoc = null;
                imageCollection2 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barManager1 = null;
                layoutControlItem14 = null;
                ucPaging1 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                txtKeyWord = null;
                btnSearch = null;
                imageCollection1 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                layoutControlGroup5 = null;
                layoutControl6 = null;
                layoutControlGroup6 = null;
                layoutControlTreeSereServ = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                lciAddress = null;
                lciGenner = null;
                lciDOB = null;
                lciPatientName = null;
                lciPatientCode = null;
                layoutControlGroup4 = null;
                lblPatientCode = null;
                lblPatientName = null;
                lblDOB = null;
                lblGender = null;
                lblAdress = null;
                layoutControl5 = null;
                groupBoxPatientInfo = null;
                layoutControlGroup3 = null;
                layoutControl4 = null;
                layoutControlItem3 = null;
                grdColBedName = null;
                grdColTreatmentCode = null;
                grdColPatientName = null;
                grdColSTT = null;
                gridControlTreatmentBedRoom = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                Root = null;
                layoutControl2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
