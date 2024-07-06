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
using His.UC.UCHein.Data;
using His.UC.UCHein;
using System.Globalization;
using Inventec.Common.Logging;

namespace His.UC.UCHein.Design.TemplateKskContract1
{
    public partial class Template__KskContract1 : UserControl
    {
        private SetFocusMoveOut _SetFocusMoveOut;
        private SetShortcutKeyDown _SetShortcutKeyDown;
        int positionHandleControl = -1;

        internal DataInitKskContract entity { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> KskContracts { get; set; }

        public Template__KskContract1(DataInitKskContract data)
        {
            try
            {
                InitializeComponent();
                this.entity = data;
                this.KskContracts = data.KskContracts;
                InitDataToControl();
                ValidControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusmoveOut()
        {
            try
            {
                //TODO
                _SetFocusMoveOut();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetValue()
        {
            try
            {
                txtKskContractCode.Text = "";
                cboKskContract.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void InitData(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER HisPatientTypeAlter)
        {
            try
            {
                var HisPatyAlterBhyt = His.UC.UCHein.HisPatientTypeAlter.HisPatientTypeAlterGet.GetById(HisPatientTypeAlter.ID);
                if (HisPatyAlterBhyt != null)
                {
                    //cboKskContract.EditValue = HisPatyAlterBhyt.KSK_CONTRACT_ID;
                    //txtKskContractCode.Text = HisPatyAlterBhyt.KSK_CONTRACT_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetValidationControl()
        {
            try
            {
                IList<Control> invalidControls = dxValidationProvider1.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    dxValidationProvider1.RemoveControlError(invalidControls[i]);
                }
                dxErrorProvider1.ClearErrors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool ValidationControl()
        {
            bool valid = true;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    IList<Control> invalidControls = dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
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

        public void FocusUserControl()
        {
            try
            {
                txtKskContractCode.Focus();
                txtKskContractCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Template__KskContract1_Load(object sender, EventArgs e)
        {
            try
            {
                lblCaptionKskContract.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCHEIN_KSK_CONTRACT_LBL_CAPTION_KSK_CONTRACT", Resources.ResourceLanguageManager.LanguageUCHeinKSKContract, Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
