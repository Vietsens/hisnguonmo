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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BaseCompensationCreate
{
    public partial class UCBaseCompensationCreate : UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                 _currentModule = null;
                _KhoXuatADOiSelecteds = null;
                listExpMestDetails_ = null;
                listExpMestDetails = null;
                listCompensation = null;
                listPresCabinet = null;
                mediStockMatys = null;
                mediStockMetys = null;
                lastInfo = null;
                lastRowHandle = 0;
                expMestPrint = null;
                rowCount = 0;
                dataTotal = 0;
                limit = 0;
                start = 0;
                isCheckAllDetail = false;
                isCheckAllCom = false;
                isCheckAllPres = false;
                isReadOnly = false;
                stock = null;
                IsReasonRequired = false;
                MoreInfo = null;
                ListTreatment = null;
                _ExpMestMaterials = null;
                _ExpMestMedicines = null;
                _ExpMestMatyReqs = null;
                _ExpMestMetyReqs = null;
                _BcsExpMest = null;
                _ExpMestMetyReq_TCs = null;
                _ExpMestMetyReq_LAOs = null;
                _ExpMestMetyReq_KSs = null;
                _ExpMestMetyReq_DTs = null;
                _ExpMestMetyReq_COs = null;
                _ExpMestMetyReq_PXs = null;
                _ExpMestMetyReq_TDs = null;
                _ExpMestMetyReq_DCHTs = null;
                _ExpMestMetyReq_DCGNs = null;
                _ExpMestMetyReq_DC_GN_HTs = null;
                _ExpMestMetyReq_HTs = null;
                _ExpMestMetyReq_GNs = null;
                _ExpMestMetyReq_GN_HTs = null;
                currentAggExpMest = null;
                this.btnRefresh.Click -= new System.EventHandler(this.btnRefresh_Click);
                this.btnFind.Click -= new System.EventHandler(this.btnFind_Click);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.gridViewBcsDetail.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewBcsDetail_CustomUnboundColumnData);
                this.gridViewExpMestBcs.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewExpMestBcs_RowCellClick);
                this.gridViewExpMestBcs.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewExpMestBcs_CustomRowCellEdit);
                this.gridViewExpMestBcs.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestBcs_CustomUnboundColumnData);
                this.repositoryItemButtonPrint.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonPrint_ButtonClick);
                this.repositoryItemButtonDelete_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonDelete_Enable_ButtonClick);
                this.cboKhoXuat.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKhoXuat_ButtonClick);
                this.cboKhoXuat.EditValueChanged -= new System.EventHandler(this.cboKhoXuat_EditValueChanged);
                this.cboKhoXuat.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboKhoXuat_CustomDisplayText);
                this.btnTimKiemS.Click -= new System.EventHandler(this.btnTimKiemS_Click);
                this.gridViewExpMestDetail.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewExpMestDetail_RowCellStyle);
                this.gridViewExpMestDetail.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewExpMestDetail_CustomRowCellEdit);
                this.gridViewExpMestDetail.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewExpMestDetail_CellValueChanged);
                this.gridViewExpMestDetail.InvalidRowException -= new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(this.gridViewExpMestDetail_InvalidRowException);
                this.gridViewExpMestDetail.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestDetail_CustomUnboundColumnData);
                this.gridViewExpMestDetail.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewExpMestDetail_MouseDown);
                this.repositoryItemCheckExpMestDetail.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckExpMestDetail_CheckedChanged);
                this.gridViewCompensation.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewCompensation_CustomRowCellEdit);
                this.gridViewCompensation.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewCompensation_CustomUnboundColumnData);
                this.gridViewCompensation.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewCompensation_MouseDown);
                this.repositoryItemCheckCompensation_Enable.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckCompensation_Enable_CheckedChanged);
                this.gridViewPresCabinet.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewPresCabinet_CustomRowCellEdit);
                this.gridViewPresCabinet.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPresCabinet_CustomUnboundColumnData);
                this.gridViewPresCabinet.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewPresCabinet_MouseDown);
                this.repositoryItemCheckPresCabinet_Enable.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckPresCabinet_Enable_CheckedChanged);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.UCBaseCompensationCreate_Load);
                gridView2.GridControl.DataSource = null;
                gridView1.GridControl.DataSource = null;
                gridViewCompensation.GridControl.DataSource = null;
                gridControlCompensation.DataSource = null;
                gridViewExpMestDetail.GridControl.DataSource = null;
                gridControlExpMestDetail.DataSource = null;
                gridViewExpMestBcs.GridControl.DataSource = null;
                gridControlExpMestBcs.DataSource = null;
                gridViewBcsDetail.GridControl.DataSource = null;
                gridControlBcsDetail.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridViewPresCabinet.GridControl.DataSource = null;
                gridControlPreCabinet.DataSource = null;
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                lciReasonRequired = null;
                gridView2 = null;
                cboReasonRequired = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                btnTimKiemS = null;
                gridView1 = null;
                cboKhoXuat = null;
                gridColumn_PresCabinet_ServiceReqCode = null;
                gridColumn_PresCabinet_TreatmentCode = null;
                gridColumn_PresCabinet_PatientName = null;
                repositoryItemPictureEdit1 = null;
                toolTipController1 = null;
                layoutControlItem10 = null;
                ucPagingExpMestBcs = null;
                repositoryItemSpinReqAmount_Disable = null;
                repositoryItemSpinReqAmount_Enable = null;
                repositoryItemCheckExpMestDetail = null;
                repositoryItemCheckPresCabinet_Disable = null;
                repositoryItemCheckPresCabinet_Enable = null;
                repositoryItemCheckCompensation_Disable = null;
                repositoryItemCheckCompensation_Enable = null;
                repositoryItemButtonPrint = null;
                repositoryItemButtonDelete_Disable = null;
                repositoryItemButtonDelete_Enable = null;
                gridColumn_ExpMestDetail_MediStock = null;
                gridColumn_BcsDetail_Amount = null;
                gridColumn_BcsDetail_UnitName = null;
                gridColumn_BcsDetail_Name = null;
                gridColumn_BcsDetail_Code = null;
                gridColumn_BcsDetail_Stt = null;
                gridColumn_ExpMestDetail_ReqAmount = null;
                gridColumn_ExpMestDetail_TotalAmount = null;
                gridColumn_ExpMestDetail_UnitName = null;
                gridColumn_ExpMestDetail_Name = null;
                gridColumn_ExpMestDetail_Code = null;
                gridColumn_ExpMestDetail_Check = null;
                gridColumn_ExpMestBcs_CreateTime = null;
                gridColumn_ExpMestBcs_MediStock = null;
                gridColumn_ExpMestBcs_ExpMestCode = null;
                gridColumn_ExpMestBcs_Status = null;
                gridColumn_ExpMestBcs_Print = null;
                gridColumn_ExpMestBcs_Delete = null;
                gridColumn_Compensation_CreateTime = null;
                gridColumn_Compensation_ReqUsername = null;
                gridColumn_Compensation_MediStock = null;
                gridColumn_Compensation_ExpMestCode = null;
                gridColumn_Compensation_Check = null;
                imageListIcon = null;
                gridColumn_PresCabinet_IntructionTime = null;
                gridColumn_PresCabinet_ReqUsername = null;
                gridColumn_PresCabinet_ExpMestCode = null;
                gridColumn_PresCabinet_Check = null;
                lciGridCompensation = null;
                layoutControlItem13 = null;
                gridViewCompensation = null;
                gridControlCompensation = null;
                gridViewExpMestDetail = null;
                gridControlExpMestDetail = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                gridViewExpMestBcs = null;
                gridControlExpMestBcs = null;
                gridViewBcsDetail = null;
                gridControlBcsDetail = null;
                lciGridPreCabiet = null;
                lciExpMediStock = null;
                gridLookUpEdit1View = null;
                cboMediStock = null;
                gridViewPresCabinet = null;
                gridControlPreCabinet = null;
                layoutControlItem9 = null;
                btnSave = null;
                layoutControlItem8 = null;
                layoutControlItem7 = null;
                btnFind = null;
                btnRefresh = null;
                layoutControlItem6 = null;
                layoutControlItem4 = null;
                checkIsCompensation = null;
                checkIsNotCompensation = null;
                layoutControlGroup5 = null;
                layoutControl6 = null;
                navBarGroupStatus = null;
                navBarGroupControlContainer2 = null;
                lciExportDateTo = null;
                lciExportDateFrom = null;
                layoutControlGroup4 = null;
                dtExportDateFrom = null;
                dtExportDateTo = null;
                layoutControl5 = null;
                layoutControlItem5 = null;
                navBarGroupControlContainer1 = null;
                navBarGroupExportDate = null;
                navBarControl1 = null;
                lciKeyword = null;
                txtKeyword = null;
                layoutControlItem3 = null;
                layoutControlItem1 = null;
                layoutControlItem2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                Root = null;
                layoutControl2 = null;
                layoutControlGroup3 = null;
                layoutControl4 = null;
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
