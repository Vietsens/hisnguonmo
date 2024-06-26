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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.KskContract.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.KskContract.Run
{
    public partial class UCKskContract__Template1 : UserControl
    {
        List<V_HIS_KSK_CONTRACT> listKskContract { get; set; }
        public long positionHandle = -1;
        KskContractInput kskContractInput;

        public UCKskContract__Template1(KskContractInput kskContractInput)
        {
            InitializeComponent();
            try
            {
                this.kskContractInput = kskContractInput;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void UCKskContract_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                ValidateControl();
                InitComboContract();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCKskContract__Template1
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.KskContract.Resources.Lang", typeof(UCKskContract__Template1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKskContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.cboKskContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCKskContract__Template1.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidateControl()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = cboKskContract;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(cboKskContract, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboKskContract.EditValue != null)
                {
                    var contract = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboKskContract.EditValue.ToString()));
                    if (contract != null)
                    {
                        lblExpiryDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EXPIRY_DATE ?? 0);
                        lblEffectDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EFFECT_DATE ?? 0);
                        lblWorkPlaceName.Text = contract.WORK_PLACE_NAME;
                        lblPaymentRatio.Text = Convert.ToInt64(contract.PAYMENT_RATIO * 100).ToString() + "%";

                        if (this.kskContractInput != null && this.kskContractInput.DeleOutFocus != null)
                        {
                            this.kskContractInput.DeleOutFocus();
                        }
                    }
                    else
                    {
                        lblExpiryDate.Text = "";
                        lblEffectDate.Text = "";
                        lblWorkPlaceName.Text = "";
                        lblPaymentRatio.Text = "";
                    }
                }
                else
                {
                    lblExpiryDate.Text = "";
                    lblEffectDate.Text = "";
                    lblWorkPlaceName.Text = "";
                    lblPaymentRatio.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string text = "";
                if (cboKskContract.EditValue != null && listKskContract != null && listKskContract.Count > 0)
                {
                    var ksk = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboKskContract.EditValue.ToString()));
                    if (ksk != null)
                    {
                        text = ksk.KSK_CONTRACT_CODE + " - " + ksk.WORK_PLACE_NAME;
                    }
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cboKskContract.EditorContainsFocus && this.kskContractInput != null && this.kskContractInput.DeleOutFocus != null)
                {
                    this.kskContractInput.DeleOutFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskContract_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void cboKskContract_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboKskContract.EditValue != null)
                {
                    if (cboKskContract.EditorContainsFocus && this.kskContractInput != null && this.kskContractInput.DeleOutFocus != null)
                    {
                        this.kskContractInput.DeleOutFocus();
                    }
                }
                else
                {
                    cboKskContract.ShowPopup();
                }
            }
        }
    }
}
