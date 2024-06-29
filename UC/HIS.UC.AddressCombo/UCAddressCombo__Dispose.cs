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
using HIS.UC.AddressCombo.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {

        public void DisposeControl()
        {
            try
            {
                workingCommuneADO = null;
                isSearchOrderByXHT = false;
                positionHandleControl = 0;
                isPatientBHYT = false;
                isOldPatient = false;
                isReadCard = false;
                dlgSendCodeProvince = null;
                dlgSendCardSDO = null;
                dlgSetAddressUCProvinceOfBirth = null;
                dlgSetAddressUCHein = null;
                dlgFocusNextUserControl = null;
                this.cboTHX.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTHXFilter_Closed);
                this.cboTHX.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTHXFilter_ButtonClick);
                this.cboTHX.EditValueChanged -= new System.EventHandler(this.cboTHX_EditValueChanged);
                this.cboTHX.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboTHXFilter_KeyUp);
                this.txtPhone.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPhone_KeyPress);
                this.txtPhone.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPhone_PreviewKeyDown);
                this.txtAddress2.EditValueChanged -= new System.EventHandler(this.txtAddress2_EditValueChanged);
                this.txtAddress2.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAddress2_PreviewKeyDown);
                this.cboDistrict.Properties.GetNotInListValue -= new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboDistrict_Properties_GetNotInListValue);
                this.cboDistrict.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDistrict_Closed);
                this.cboDistrict.EditValueChanged -= new System.EventHandler(this.cboDistrict_EditValueChanged);
                this.cboDistrict.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDistrict_KeyUp);
                this.txtDistrictCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDistrictCode_PreviewKeyDown);
                this.txtAddress.EditValueChanged -= new System.EventHandler(this.txtAddress_EditValueChanged);
                this.txtAddress.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAddress_PreviewKeyDown);
                this.cboCommune.Properties.GetNotInListValue -= new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboCommune_Properties_GetNotInListValue);
                this.cboCommune.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCommune_Closed);
                this.cboCommune.EditValueChanged -= new System.EventHandler(this.cboCommune_EditValueChanged);
                this.cboCommune.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboCommune_KeyUp);
                this.txtCommuneCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCommuneCode_PreviewKeyDown);
                this.cboProvince.Properties.GetNotInListValue -= new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboProvince_Properties_GetNotInListValue);
                this.cboProvince.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvince_Closed);
                this.cboProvince.EditValueChanged -= new System.EventHandler(this.cboProvince_EditValueChanged);
                this.cboProvince.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboProvince_KeyUp);
                this.txtProvinceCode.EditValueChanged -= new System.EventHandler(this.txtProvinceCode_EditValueChanged);
                this.txtProvinceCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtProvinceCode_PreviewKeyDown);
                this.txtMaTHX.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaTHX_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCAddressCombo_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                layoutControlItem1 = null;
                gridLookUpEdit1View = null;
                cboTHX = null;
                emptySpaceItem1 = null;
                lciPhone = null;
                txtPhone = null;
                lciAddress2 = null;
                txtAddress2 = null;
                dxErrorProviderControl = null;
                dxValidationProviderControl = null;
                lciDistrict = null;
                layoutControlItem8 = null;
                txtDistrictCode = null;
                cboDistrict = null;
                lciAddress = null;
                txtAddress = null;
                lciCommune = null;
                layoutControlItem5 = null;
                txtCommuneCode = null;
                cboCommune = null;
                lciProvince = null;
                layoutControlItem3 = null;
                txtProvinceCode = null;
                cboProvince = null;
                lciTHX = null;
                txtMaTHX = null;
                lcgAddress = null;
                lcUCAddressCombo = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
