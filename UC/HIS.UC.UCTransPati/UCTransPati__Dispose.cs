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
using HIS.UC.UCTransPati.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.Icd.ADO;
using HIS.UC.UCTransPati.Config;

namespace HIS.UC.UCTransPati
{
    public partial class UCTransPati : UserControl
    {
        public void DisposeControl()
        {
            try
            {

                ucIcd = null;
                icdProcessor = null;
                SubIcdProcessor = null;
                dlgFocusNextUserControl = null;
                positionHandleControl = 0;
                autoCheckIcd = null;
                _TextIcdName = null;
                IsObligatoryTranferMediOrg = false;
                this.dtToTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtToTime_Closed);
                this.dtToTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtToTime_PreviewKeyDown);
                this.dtFromTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFromTime_Closed);
                this.dtFromTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtFromTime_PreviewKeyDown);
                this.cboLyDoChuyen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboLyDoChuyen_Closed);
                this.cboLyDoChuyen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboLyDoChuyen_ButtonClick);
                this.cboLyDoChuyen.EditValueChanged -= new System.EventHandler(this.cboLyDoChuyen_EditValueChanged);
                this.cboLyDoChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboLyDoChuyen_PreviewKeyDown);
                this.txtMaLyDoChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaLyDoChuyen_PreviewKeyDown);
                this.cboHinhThucChuyen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHinhThucChuyen_Closed);
                this.cboHinhThucChuyen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboHinhThucChuyen_ButtonClick);
                this.cboHinhThucChuyen.EditValueChanged -= new System.EventHandler(this.cboHinhThucChuyen_EditValueChanged);
                this.cboHinhThucChuyen.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboHinhThucChuyen_KeyUp);
                this.txtMaHinhThucChuyen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaHinhThucChuyen_PreviewKeyDown);
                this.txtMaNoiChuyenDen.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaNoiChuyenDen_PreviewKeyDown);
                this.cboNoiChuyenDen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoiChuyenDen_Closed);
                this.cboNoiChuyenDen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoiChuyenDen_ButtonClick);
                this.cboNoiChuyenDen.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboNoiChuyenDen_KeyUp);
                this.cboChuyenTuyen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboChuyenTuyen_Closed);
                this.cboChuyenTuyen.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboChuyenTuyen_ButtonClick);
                this.txtInCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCTransPati_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridView2.GridControl.DataSource = null;
                lciFordtToTime = null;
                lciFordtFromTime = null;
                dtFromTime = null;
                dtToTime = null;
                lciCboLyDoChuyen = null;
                lciLyDoChuyen = null;
                lciCboHinhThucChuyen = null;
                lciHinhThucChuyen = null;
                lciNoiChuyenDen = null;
                layoutControlItem1 = null;
                cboLyDoChuyen = null;
                cboHinhThucChuyen = null;
                lciChuyenTuyen = null;
                lciSoChuyenVien = null;
                lciBenhChinh = null;
                gridLookUpEdit1View = null;
                cboNoiChuyenDen = null;
                pnlIcd = null;
                txtInCode = null;
                gridView2 = null;
                cboChuyenTuyen = null;
                txtMaHinhThucChuyen = null;
                txtMaLyDoChuyen = null;
                txtMaNoiChuyenDen = null;
                dxErrorProviderControl = null;
                dxValidationProviderControl = null;
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
