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
using HIS.UC.UCHeniInfo.ADO;
using MOS.SDO;
using HIS.Desktop.Utility;
using HIS.UC.UCHeniInfo.Utils;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.UC.UCHeniInfo.Data;
using Inventec.Common.Adapter;
using HIS.UC.UCHeniInfo.ControlProcess;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCHeniInfo
{
	public partial class UCHeinInfo : UserControlBase
	{
		public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                IsCheckAutoDT = false;
                dataSourceCboHeinRightRouteTemp = null;
                module = null;
                treatmentId = 0;
                currentPatientRaw = null;
                CodeProvince = null;
                ChangeDataByCard = false;
                IsDungTuyenCapCuuByTime = false;
                heinCardData = null;
                isPatientOld = false;
                isCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong = false;
                isShowCheckKhongKTHSD = null;
                isEdit = false;
                isTempQN = false;
                currentPatientSdo = null;
                IsDefaultRightRouteType = false;
                dataHein = null;
                dlgProcessChangePatientDob = null;
                dlgSend3WBhytCode = null;
                updateTranPatiDataByPatientOld = null;
                dlgSetCareerByHeinCardNumber = null;
                dlgDisableBtnTTCT = null;
                dlgShowThongTinChuyenTuyen = null;
                dlgIsEnableEmergency = null;
                dlgGetIsChild = null;
                dlgCheckTT = null;
                dlgAutoCheckCC = null;
                dlgcheckExamHistory = null;
                dlgfillDataPatientSDOToRegisterForm = null;
                dlgProcessFillDataCareerUnder6AgeByHeinCardNumber = null;
                dlgValidationControl = null;
                dlgFocusNextUserControl = null;
                this.btnCheckInfoBHYT.Click -= new System.EventHandler(this.btnCheckInfoBHYT_Click);
                this.chkIsTt46.CheckedChanged -= new System.EventHandler(this.chkIsTt46_CheckedChanged);
                this.chkHasAbsentLetter.CheckedChanged -= new System.EventHandler(this.chkHasAbsentLetter_CheckedChanged);
                this.chkHasWorkingLetter.CheckedChanged -= new System.EventHandler(this.chkHasWorkingLetter_CheckedChanged);
                this.chkHasWorkingLetter.VisibleChanged -= new System.EventHandler(this.chkHasWorkingLetter_VisibleChanged);
                this.txtDu5Nam.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtDu5Nam_ButtonClick);
                this.txtDu5Nam.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDu5Nam_PreviewKeyDown);
                this.dtDu5Nam.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtDu5nam_Closed);
                this.dtDu5Nam.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtDu5Nam_KeyDown);
                this.chkKhongKTHSD.CheckedChanged -= new System.EventHandler(this.chkKhongKTHSD_CheckedChanged);
                this.txtHeinCardFromTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtHeinCardFromTime_ButtonClick);
                this.txtHeinCardFromTime.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtHeinCardFromTime_InvalidValue);
                this.txtHeinCardFromTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHeinCardFromTime_PreviewKeyDown);
                this.txtHeinCardFromTime.Validated -= new System.EventHandler(this.txtHeinCardFromTime_Validated);
                this.dtHeinCardFromTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtHeinCardFromTime_Closed);
                this.dtHeinCardFromTime.EditValueChanged -= new System.EventHandler(this.dtHeinCardFromTime_EditValueChanged);
                this.dtHeinCardFromTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtHeinCardFromTime_KeyDown);
                this.cboNoiSong.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoiSong_Closed);
                this.cboNoiSong.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoiSong_ButtonClick);
                this.cboNoiSong.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboNoiSong_KeyUp);
                this.txtFreeCoPainTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtFreeCoPainTime_ButtonClick);
                this.txtFreeCoPainTime.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtFreeCoPainTime_InvalidValue);
                this.txtFreeCoPainTime.TextChanged -= new System.EventHandler(this.txtFreeCoPainTime_TextChanged);
                this.txtFreeCoPainTime.Click -= new System.EventHandler(this.txtFreeCoPainTime_Click);
                this.txtFreeCoPainTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtFreeCoPainTime_KeyDown);
                this.txtFreeCoPainTime.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtFreeCoPainTime_KeyPress);
                this.txtFreeCoPainTime.Validating -= new System.ComponentModel.CancelEventHandler(this.txtFreeCoPainTime_Validating);
                this.dtFreeCoPainTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFreeCoPainTime_Closed);
                this.dtFreeCoPainTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtFreeCoPainTime_KeyDown);
                this.chkPaid6Month.CheckedChanged -= new System.EventHandler(this.chkPaid6Month_CheckedChanged);
                this.chkPaid6Month.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPaid6Month_PreviewKeyDown);
                this.chkJoin5Year.CheckedChanged -= new System.EventHandler(this.chkJoin5Year_CheckedChanged);
                this.chkJoin5Year.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkJoin5Year_PreviewKeyDown);
                this.cboHeinRightRoute.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHeinRightRoute_Closed);
                this.cboHeinRightRoute.EditValueChanged -= new System.EventHandler(this.cboHeinRightRoute_EditValueChanged);
                this.cboDKKCBBD.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDKKCBBD_Closed);
                this.cboDKKCBBD.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDKKCBBD_KeyUp);
                this.txtMaDKKCBBD.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaDKKCBBD_PreviewKeyDown);
                this.txtAddress.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAddress_PreviewKeyDown);
                this.txtHeinCardToTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtHeinCardToTime_ButtonClick);
                this.txtHeinCardToTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHeinCardToTime_PreviewKeyDown);
                this.dtHeinCardToTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtHeinCardToTime_Closed);
                this.dtHeinCardToTime.EditValueChanged -= new System.EventHandler(this.dtHeinCardToTime_EditValueChanged);
                this.dtHeinCardToTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtHeinCardToTime_KeyDown);
                this.txtSoThe.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSoThe_Properties_ButtonClick);
                this.txtSoThe.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtSoThe_InvalidValue);
                this.txtSoThe.EditValueChanged -= new System.EventHandler(this.txtSoThe_EditValueChanged);
                this.txtSoThe.TextChanged -= new System.EventHandler(this.txtSoThe_TextChanged);
                this.txtSoThe.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtSoThe_KeyDown);
                this.txtSoThe.Leave -= new System.EventHandler(this.txtSoThe_Leave);
                this.cboSoThe.Properties.GetNotInListValue -= new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboSoThe_Properties_GetNotInListValue);
                this.cboSoThe.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSoThe_Closed);
                this.cboSoThe.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboSoThe_KeyUp);
                this.chkHasCardTemp.CheckedChanged -= new System.EventHandler(this.chkHasCardTemp_CheckedChanged);
                this.chkHasCardTemp.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkHasDobCertificate_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCHeinInfo_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                layoutControlItem1 = null;
                btnCheckInfoBHYT = null;
                lciNote = null;
                lciIsTt46 = null;
                lciHasAbsentLetter = null;
                lciIsBhytHolded = null;
                lciHasWorkingLetter = null;
                chkHasWorkingLetter = null;
                chkIsBhytHolded = null;
                chkHasAbsentLetter = null;
                chkIsTt46 = null;
                txtNote = null;
                emptySpaceItem1 = null;
                lci5Y = null;
                dtDu5Nam = null;
                txtDu5Nam = null;
                panel4 = null;
                lciKhongKTHSD = null;
                chkKhongKTHSD = null;
                lciHeinCardFromTime = null;
                dtHeinCardFromTime = null;
                txtHeinCardFromTime = null;
                panel3 = null;
                txtHeinCardToTime = null;
                dxErrorProviderControl = null;
                dxValidationProviderControl = null;
                lciKV = null;
                cboNoiSong = null;
                layoutControlItem13 = null;
                txtMucHuong = null;
                lciFreeCoPainTime = null;
                dtFreeCoPainTime = null;
                txtFreeCoPainTime = null;
                panelControl1 = null;
                lciPaid6Month = null;
                lciJoin5Year = null;
                chkJoin5Year = null;
                chkPaid6Month = null;
                lciHeinRightRoute = null;
                cboHeinRightRoute = null;
                txtMaDKKCBBD = null;
                gridLookUpEdit1View = null;
                cboDKKCBBD = null;
                lciHeinCardAddress = null;
                txtAddress = null;
                lciHeinCardToTime = null;
                lciHeinCardNumber = null;
                lciHasDobCertificate = null;
                dtHeinCardToTime = null;
                panel2 = null;
                chkHasCardTemp = null;
                cboSoThe = null;
                txtSoThe = null;
                panel1 = null;
                gboxHeinCardInformation = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

	}
}
