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
using HIS.UC.UCPatientRaw.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        public void DisposeControl()
        {
            try
            {
                nameYear = null;
                nameColumn = null;
                codeColumn = null;
                BhytWhiteListtId = null;
                BhytCode = null;
                IsLoadFromSearchTxtCode = false;
                IsEmergency = false;
                ReleaseDateQrCccd = null;
                AddressFromQrCccd = null;
                CccdCardFromQrCccd = null;
                dataHeinCardFromQrCccd = null;
                baseNameControl = null;
                branch = null;
                currentNameControl = null;
                APP_CODE__EXACT = null;
                MODULE_LINK = null;
                currentHideControls = null;
                isShow = false;
                isShowContainerForChoose = false;
                isShowContainer = false;
                dlgShowOtherPaySource = null;
                dataEthnic = null;
                paties = null;
                primaryPatientTypes = null;
                ResultDataADO = null;
                isAlertTreatmentEndInDay = false;
                _UCPatientRawADO = null;
                isTemp_QN = false;
                isGKS = false;
                positionHandleControl = 0;
                isChild = false;
                isReadQrCode = false;
                isNotPatientDayDob = false;
                isDobTextEditKeyEnter = false;
                typeReceptionForm = null;
                typeCodeFind = null;
                currentPatientType = null;
                currentSearchedPatients = null;
                currentPatientSDO = null;
                this.dlgSendPatientSdo(currentPatientSDO);
                cardSearch = null;
                qrCodeBHYTHeinCardData = null;
                isDefault = false;
                dlgSendPatientSdo = null;
                dlgProcessChangePatientDob = null;
                dlgSendPatientName = null;
                dlgShowControlGuaranteeLoginname = null;
                dlgShowControlHrmKskCodeNotValid = null;
                dlgShowControlHrmKskCode = null;
                dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw = null;
                dlgResetRegisterForm = null;
                dlgGetIntructionTime = null;
                dlgVisibleUCHein = null;
                actInitExamServiceRoomByAppoimentTime = null;
                dlgSearchPatient1 = null;
                dlgEnableSave = null;
                dlgCheckTT = null;
                isVisible = null;
                dlgSetFocusWhenPatientIsChild = null;
                enableLciBenhNhanMoi = null;
                isEnable = null;
                dlgSetValidation = null;
                dlgFocusToUCRelativeWhenPatientIsChild = null;
                dlgFocusNextUserControl = null;
                PatientTypeEditValueChanged = null;
                currentPatientClassify = null;
                currentMilitaryRank = null;
                currentWorkPlace = null;
                currentPosition = null;
                dataWhiteList = null;
                dataClassify = null;
                dataMilitaryRank = null;
                dataWorkPlace = null;
                dataPosition = null;
                employeeCode = null;
                patientCode = null;
                patientId = 0;
                patientTD3 = null;
                lstSend = null;
                TD3 = false;
                this.txtPosition.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPosition_ButtonClick);
                this.txtPosition.TextChanged -= new System.EventHandler(this.txtPosition_TextChanged);
                this.txtPosition.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPosition_KeyDown);
                this.txtPosition.Leave -= new System.EventHandler(this.txtPosition_Leave);
                this.barManager1.HighlightedLinkChanged -= new DevExpress.XtraBars.HighlightedLinkChangedEventHandler(this.barManager1_HighlightedLinkChanged);
                this.txtMilitaryRank.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtMilitaryRank_ButtonClick);
                this.txtMilitaryRank.TextChanged -= new System.EventHandler(this.txtMilitaryRank_TextChanged);
                this.txtMilitaryRank.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtMilitaryRank_KeyDown);
                this.txtMilitaryRank.Leave -= new System.EventHandler(this.txtMilitaryRank_Leave);
                this.txtWorkPlace.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtWorkPlace_ButtonClick);
                this.txtWorkPlace.TextChanged -= new System.EventHandler(this.txtWorkPlace_TextChanged);
                this.txtWorkPlace.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtWorkPlace_KeyDown);
                this.txtWorkPlace.Leave -= new System.EventHandler(this.txtWorkPlace_Leave);
                this.txtPatientClassify.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPatientClassify_ButtonClick);
                this.txtPatientClassify.TextChanged -= new System.EventHandler(this.txtPatientClassify_TextChanged);
                this.txtPatientClassify.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPatientClassify_KeyDown);
                this.txtPatientClassify.Leave -= new System.EventHandler(this.txtPatientClassify_Leave);
                this.popupControlContainer1.CloseUp -= new System.EventHandler(this.popupControlContainer1_CloseUp);
                this.gridControlPopUp.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridControlPopUp_KeyDown);
                this.gridViewPopUp.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewPopUp_RowClick);
                this.txtEthnicCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtEthnicCode_PreviewKeyDown);
                this.cboEthnic.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEthnic_Closed);
                this.cboEthnic.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboEthnic_KeyUp);
                this.cboPrimaryPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPrimaryPatientType_Closed);
                this.cboPrimaryPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPrimaryPatientType_ButtonClick);
                this.cboPrimaryPatientType.EditValueChanged -= new System.EventHandler(this.cboPrimaryPatientType_EditValueChanged);
                this.txtPrimaryPatientTypeCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPrimaryPatientTypeCode_PreviewKeyDown);
                this.txtPatientTypeCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientTypeCode_PreviewKeyDown);
                this.cboPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
                this.cboPatientType.EditValueChanged -= new System.EventHandler(this.cboPatientType_EditValueChanged);
                this.cboPatientType.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboPatientType_KeyDown);
                this.txtCareerCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCareerCode_PreviewKeyDown);
                this.cboCareer.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCareer_Closed);
                this.cboCareer.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboCareer_KeyUp);
                this.txtAge.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAge_PreviewKeyDown);
                this.txtPatientDob.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPatientDob_ButtonClick);
                this.txtPatientDob.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtPatientDob_InvalidValue);
                this.txtPatientDob.Click -= new System.EventHandler(this.txtPatientDob_Click);
                this.txtPatientDob.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPatientDob_KeyPress);
                this.txtPatientDob.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientDob_PreviewKeyDown);
                this.txtPatientDob.Validating -= new System.ComponentModel.CancelEventHandler(this.txtPatientDob_Validating);
                this.txtPatientDob.Validated -= new System.EventHandler(this.txtPatientDob_Validated);
                this.dtPatientDob.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtPatientDob_Closed);
                this.dtPatientDob.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtPatientDob_KeyDown);
                this.cboGender.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboGender_Closed);
                this.cboGender.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboGender_KeyDown);
                this.txtPatientName.EditValueChanged -= new System.EventHandler(this.txtPatientName_EditValueChanged);
                this.txtPatientName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientName_PreviewKeyDown);
                this.txtPatientCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPatientCode_KeyDown);
                this.dxValidationProviderControl.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderControl_ValidationFailed);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.UCPatientRaw_Load);
                gridViewPopUp.GridControl.DataSource = null;
                gridControlPopUp.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridView1.GridControl.DataSource = null;
                lciPositionNew = null;
                txtPosition = null;
                lciMilitaryRankNew = null;
                txtMilitaryRank = null;
                lciWorkPlaceNameNew = null;
                txtWorkPlace = null;
                lciPatientClassifyNew = null;
                txtPatientClassify = null;
                gridViewPopUp = null;
                gridControlPopUp = null;
                popupControlContainer1 = null;
                lciForcboEthnic = null;
                lciFortxtEthnicCode = null;
                cboEthnic = null;
                txtEthnicCode = null;
                barManager1 = null;
                barDockControlTop = null;
                barDockControlBottom = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                toolTipController1 = null;
                lciComboPrimaryPatientType = null;
                lciPrimaryPatientType = null;
                txtPrimaryPatientTypeCode = null;
                gridLookUpEdit1View = null;
                cboPrimaryPatientType = null;
                dxErrorProviderControl = null;
                cboCareer = null;
                txtCareerCode = null;
                cboAge = null;
                txtAge = null;
                cboGender = null;
                cboPatientType = null;
                txtPatientCode = null;
                dxValidationProviderControl = null;
                layoutControlItem10 = null;
                layoutControlItem11 = null;
                gridView1 = null;
                txtPatientTypeCode = null;
                lciNgheNghiep = null;
                lciMaNgheNghiep = null;
                layoutControlItem6 = null;
                layoutControlItem7 = null;
                lciPatientDob = null;
                dtPatientDob = null;
                txtPatientDob = null;
                panel1 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                txtPatientName = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                btnCodeFind = null;
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
