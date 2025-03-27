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
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCMaHoNgheo : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;
        int positionHandleControl = -1;
        public bool val = true;

        #endregion

        #region Contructor - Load

        public UCMaHoNgheo()
            : base("UCPlusInfo", "UCMaHoNgheo")
        {
            try
            {
                InitializeComponent(); 
                
                this.txtHoNgheoCode.TabIndex = this.TabIndex;
                this.ValidHNCode();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCMaHoNgheo_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCMaHoNgheo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCMaHoNgheo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMaHoNgheo.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaHoNgheo.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCMaHoNgheo.lciMaHoNgheo.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaHoNgheo.Text = Inventec.Common.Resource.Get.Value("UCMaHoNgheo.lciMaHoNgheo.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciMaHoNgheo.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCMaHoNgheo", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void txtHoNgheoCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.txtHoNgheoCode.Text.Length > 0)
                    ValidHNCode();
                else
                    this.dxValidationProviderMaHN.SetValidationRule(txtHoNgheoCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHoNgheoCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private void ValidHNCode()
        {
            try
            {
                Validate_MaHN_Control oDateRule = new Validate_MaHN_Control();
                oDateRule.txtHNCode = txtHoNgheoCode;
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderMaHN.SetValidationRule(txtHoNgheoCode, oDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (!this.dxValidationProviderMaHN.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderMaHN.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        #endregion

        #region Get - Set Value

        internal string GetValue()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtHoNgheoCode.Text))
                    return this.txtHoNgheoCode.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValue(long patientID,string maHoNgheo)
        {
            try
            {
                if (!String.IsNullOrEmpty(maHoNgheo))
                    this.txtHoNgheoCode.Text = maHoNgheo;
                else if (patientID != null && patientID > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeAlterFilter filter = new MOS.Filter.HisPatientTypeAlterFilter();
                    filter.TDL_PATIENT_ID = patientID;
                    var patient = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get",ApiConsumers.MosConsumer,filter,param);

                    if (patient != null)
                        this.txtHoNgheoCode.Text = patient[0].HNCODE;
                }
                else
                    this.txtHoNgheoCode.Text = "";
                //this.txtHoNgheoCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void txtHoNgheoCode_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (this.txtHoNgheoCode.Text.Length > 0)
            //        ValidHNCode();
            //    else
            //        this.dxValidationProviderMaHN.SetValidationRule(txtHoNgheoCode, null);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtHoNgheoCode_Validated(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                this.val = dxValidationProviderMaHN.Validate();
                bool validPatientInfo = dxValidationProviderMaHN.Validate();
                if (!validPatientInfo)
                {
                    IList<Control> invalidControls = dxValidationProviderMaHN.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Info((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void DisposeControl()
        {
            try
            {
                val = false;
                positionHandleControl = 0;
                dlgFocusNextUserControl = null;
                this.txtHoNgheoCode.EditValueChanged -= new System.EventHandler(this.txtHoNgheoCode_EditValueChanged);
                this.txtHoNgheoCode.TextChanged -= new System.EventHandler(this.txtHoNgheoCode_TextChanged);
                this.txtHoNgheoCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtHoNgheoCode_KeyDown);
                this.txtHoNgheoCode.Validated -= new System.EventHandler(this.txtHoNgheoCode_Validated);
                this.Load -= new System.EventHandler(this.UCMaHoNgheo_Load);
                dxValidationProviderMaHN = null;
                lciMaHoNgheo = null;
                txtHoNgheoCode = null;
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
