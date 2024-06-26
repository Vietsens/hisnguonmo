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
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.Properties;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                rowCount = 0;
                dataTotal = 0;
                start = 0;
                lastRowHandle = 0;
                lastColumn = null;
                lastInfo = null;
                currentModule = null;
                popupMenuProcessor = null;
                emrMenuPopupProcessor = null;
                ListHisTreatment = null;
                ListICD = null;
                currentTreatment = null;
                module = null;
                currentTreatmentPrint = null;
                loginName = null;
                controlAcs = null;
                controlDelete = null;
                RoleUse = null;
                ControlRule = null;
                _DienDieuTriSelecteds = null;
                _EndDepartmentSelecteds = null;
                DepartmentSelecteds = null;
                _KieuBenhNhanSelecteds = null;
                _TrangThaiSelecteds = null;
                listTreatmentType = null;
                listDepartment = null;
                listKieuBenhNhan = null;
                listTrangThai = null;
                listKskContract = null;
                patientTypeSelecteds = null;
                listPatientType = null;
                causeResult = null;
                isDelete = false;
                typeCodeFind__KeyWork_InDate = null;
                typeCodeFind_InDate = null;
                typeCodeFind__InMonth = null;
                typeCodeFind__InYear = null;
                typeCodeFind__InTime = null;
                typeCodeFind__KeyWork_OutDate = null;
                typeCodeFind_OutDate = null;
                typeCodeFind__OutMonth = null;
                typeCodeFind__OutYear = null;
                typeCodeFind__OutTime = null;
                controlStateWorker = null;
                currentControlStateRDO = null;
                moduleLink = null;
                barManager = null;
                menu = null;
                fileName = null;
                hisTreatments = null;
                ListTemp = null;
                ListTempXN = null;
                lstHeaderColumns = null;
                lstHeaderColumnsXN = null;
                serviceReqExamEndType = null;
                this.btnRecordChecking.Click -= new System.EventHandler(this.btnRecordChecking_Click);
                this.btnDelete.Click -= new System.EventHandler(this.btnDelete_Click);
                this.btnGuiHS.Click -= new System.EventHandler(this.btnGuiHS_Click);
                this.gridView5.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView5_CustomUnboundColumnData);
                this.txtSocialInsuranceNumber.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSocialInsuranceNumber_PreviewKeyDown);
                this.cboKhoaVaoVien.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKhoaVaoVien_Properties_ButtonClick);
                this.cboKhoaVaoVien.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKhoaVaoVien_ButtonClick);
                this.btnXuatXML.Click -= new System.EventHandler(this.btnXuatXML_Click);
                this.btnPrintfKSK.Click -= new System.EventHandler(this.btnPrintfKSK_Click);
                this.BtnPrintHuongDan.Click -= new System.EventHandler(this.BtnPrintHuongDan_Click);
                this.txtPatientName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtPatientName_KeyUp);
                this.txtOutCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtOutCode_PreviewKeyDown);
                this.txtInCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInCode_PreviewKeyDown);
                this.txtStoreCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtStoreCode_PreviewKeyDown);
                this.cboContract.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboContract_Properties_ButtonClick);
                this.cboContract.EditValueChanged -= new System.EventHandler(this.cboContract_EditValueChanged);
                this.cboContract.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboContract_CustomDisplayText);
                this.btnPrintServiceReq.Click -= new System.EventHandler(this.btnPrintServiceReq_Click);
                this.btnImportKsk.Click -= new System.EventHandler(this.btnImportKsk_Click);
                this.cboDienDieuTri.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboDienDieuTri_CustomDisplayText);
                this.cboKieuBenhNhan.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboKieuBenhNhan_CustomDisplayText);
                this.cboTrangThai.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboTrangThai_CustomDisplayText);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.txtPatient.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatient_PreviewKeyDown);
                this.btnRefresh.Click -= new System.EventHandler(this.BtnRefresh_Click);
                this.btnFind.Click -= new System.EventHandler(this.BtnFind);
                this.btnNextDayInHospital.Click -= new System.EventHandler(this.btnNextDayInHospital_Click);
                this.btnPreDayInHospital.Click -= new System.EventHandler(this.btnPreDayInHospital_Click);
                this.dtInHospital.EditValueChanged -= new System.EventHandler(this.dtInHospital_EditValueChanged);
                this.btnNextDayOutHospital.Click -= new System.EventHandler(this.btnNextDayOutHospital_Click);
                this.btnPreDayOutHospital.Click -= new System.EventHandler(this.btnPreDayOutHospital_Click);
                this.dtOutHospital.EditValueChanged -= new System.EventHandler(this.dtOutHospital_EditValueChanged);
                this.cboPatientType.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientType_Properties_ButtonClick);
                this.cboPatientType.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboPatientType_CustomDisplayText);
                this.txtTreatment.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatment_PreviewKeyDown);
                this.gridControlTreatmentList.MouseClick -= new System.Windows.Forms.MouseEventHandler(this.gridControlTreatmentList_MouseClick);
                this.gridViewtreatmentList.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewtreatmentList_RowCellStyle);
                this.gridViewtreatmentList.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewtreatmentList_CustomRowCellEdit);
                this.gridViewtreatmentList.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewtreatmentList_PopupMenuShowing);
                this.gridViewtreatmentList.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewtreatmentList_SelectionChanged);
                this.gridViewtreatmentList.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatmentList_CustomUnboundColumnData);
                this.gridViewtreatmentList.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewtreatmentList_MouseDown);
                this.repositoryItembtnTimeLine.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnTimeLine_btnClick);
                this.repositoryItembtnServicePackgeView.Click -= new System.EventHandler(this.repositoryItembtnServicePackgeView_Click);
                this.repositoryItembtnFixTreatment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnFixTreatment_ButtonClick);
                this.repositoryItembtnServiceReq.Click -= new System.EventHandler(this.repositoryItembtnServiceReq_Click);
                this.repositoryItembtnServiceReqList.Click -= new System.EventHandler(this.repositoryItembtnServiceReqList_Click);
                this.repositoryItembtnEditTreatment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnEditTreatment_btnClick);
                this.repositoryItembtnMergePatient.Click -= new System.EventHandler(this.repositoryItembtnMergePatient_Click);
                this.repositoryItembtnActionHurt.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnActionHurt_btnClick);
                this.repositoryItembtnFinish.Click -= new System.EventHandler(this.repositoryItembtnFinish_Click);
                this.repositoryItembtnUnFinish.Click -= new System.EventHandler(this.repositoryItembtnUnifinish_Click);
                this.repositoryItembtnPaySereServ.Click -= new System.EventHandler(this.repositoryItembtnPaySereServ_Click);
                this.repositoryItembtnEdit_Print.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit_Print_ButtonClick);
                this.BtnDelete_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.BtnDelete_Enable_ButtonClick);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.cboEndDepartment.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEndDepartment_Properties_ButtonClick);
                this.cboEndDepartment.EditValueChanged -= new System.EventHandler(this.cboEndDepartment_EditValueChanged);
                this.cboEndDepartment.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboEndDepartment_CustomDisplayText);
                this.Load -= new System.EventHandler(this.UCTreatmentList_Load);
                this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.UCTreatmentList_KeyDown);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }    
    }
}
