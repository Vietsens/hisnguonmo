/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using HIS.UC.MediOrgPicker;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.BloodList
{
    public partial class frmBloodUpdate
    {
        internal const int TransferMediOrgCodeMaxLength = 10;

        private ButtonEdit txtTransferMediOrgCode;
        private LayoutControlItem lciTransferMediOrgCode;
        private DXErrorProvider transferMediOrgErrorProvider;

        /// <summary>
        /// Tao ButtonEdit "CSKCB chuyen" + LayoutControlItem va chen vao layoutControlGroup1
        /// cung hang voi "Ten nguoi cho". Goi tu constructor sau InitializeComponent().
        /// </summary>
        internal void InitTransferMediOrgCodeControl()
        {
            try
            {
                if (txtTransferMediOrgCode != null) return;

                // 1. Thu hep "Ten nguoi cho" tu width 609 -> 390 de chua "CSKCB chuyen" o cot 4
                if (this.lciGiverName != null)
                {
                    this.lciGiverName.Size = new Size(390, 24);
                }

                // 2. Tao ButtonEdit
                txtTransferMediOrgCode = new ButtonEdit();
                txtTransferMediOrgCode.Name = "txtTransferMediOrgCode";
                txtTransferMediOrgCode.Properties.Buttons.Clear();
                txtTransferMediOrgCode.Properties.Buttons.AddRange(new EditorButton[]
                {
                    new EditorButton(ButtonPredefines.Plus)
                });
                txtTransferMediOrgCode.ButtonClick += TxtTransferMediOrgCode_ButtonClick;
                txtTransferMediOrgCode.EditValueChanged += TxtTransferMediOrgCode_EditValueChanged;

                // 3. Tao LayoutControlItem o vi tri (625, 96), kich thuoc 219x24
                lciTransferMediOrgCode = new LayoutControlItem();
                lciTransferMediOrgCode.Name = "lciTransferMediOrgCode";
                lciTransferMediOrgCode.Text = "CSKCB chuyển:";
                lciTransferMediOrgCode.Control = txtTransferMediOrgCode;
                lciTransferMediOrgCode.AppearanceItemCaption.Options.UseTextOptions = true;
                lciTransferMediOrgCode.AppearanceItemCaption.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Far;
                lciTransferMediOrgCode.TextAlignMode = TextAlignModeItem.CustomSize;
                lciTransferMediOrgCode.TextSize = new Size(90, 20);
                lciTransferMediOrgCode.TextToControlDistance = 5;
                lciTransferMediOrgCode.OptionsToolTip.ToolTip =
                    "Mã cơ sở khám chữa bệnh chuyển tuyến (xuất XML130 BHYT - tối đa 10 ký tự)";

                if (this.layoutControl1 != null)
                {
                    this.layoutControl1.Controls.Add(txtTransferMediOrgCode);
                }

                // 4. Chen vao layoutControlGroup1 (Root) ngay ben phai lciGiverName
                if (this.layoutControlGroup1 != null && this.lciGiverName != null)
                {
                    this.layoutControlGroup1.AddItem(lciTransferMediOrgCode);
                    lciTransferMediOrgCode.Move(this.lciGiverName,
                        DevExpress.XtraLayout.Utils.InsertType.Right);
                }

                // 5. Tao DXErrorProvider hien icon canh bao realtime ben canh control
                transferMediOrgErrorProvider = new DXErrorProvider();
                transferMediOrgErrorProvider.ContainerControl = this;
                transferMediOrgErrorProvider.SetIconAlignment(
                    txtTransferMediOrgCode,
                    ErrorIconAlignment.MiddleRight);
                UpdateTransferMediOrgErrorState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Set caption cho lciTransferMediOrgCode theo ngon ngu hien tai.
        /// Goi tu SetCaptionByLanguageKey().
        /// </summary>
        internal void SetCaptionByLanguageKeyTransferMediOrg()
        {
            try
            {
                if (lciTransferMediOrgCode == null) return;
                if (Resources.ResourceLanguageManager.LanguageResource == null) return;

                lciTransferMediOrgCode.Text = Inventec.Common.Resource.Get.Value(
                    "frmBloodUpdate.lciTransferMediOrgCode.Text",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtTransferMediOrgCode_EditValueChanged(object sender, EventArgs e)
        {
            UpdateTransferMediOrgErrorState();
        }

        /// <summary>
        /// Set/clear icon canh bao realtime tren control dua tren do dai hien tai.
        /// </summary>
        private void UpdateTransferMediOrgErrorState()
        {
            try
            {
                if (transferMediOrgErrorProvider == null || txtTransferMediOrgCode == null) return;
                string text = (txtTransferMediOrgCode.Text ?? string.Empty).Trim();
                if (text.Length > TransferMediOrgCodeMaxLength)
                {
                    transferMediOrgErrorProvider.SetError(
                        txtTransferMediOrgCode,
                        Resources.ResourceMessage.MaCSKCBChuyenToiDa10KyTu,
                        ErrorType.Warning);
                }
                else
                {
                    transferMediOrgErrorProvider.SetError(txtTransferMediOrgCode, "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtTransferMediOrgCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button == null || e.Button.Kind != ButtonPredefines.Plus) return;
                string current = txtTransferMediOrgCode.Text ?? string.Empty;
                string picked = MediOrgPickerProcessor.Pick(current);
                if (!string.IsNullOrEmpty(picked))
                {
                    txtTransferMediOrgCode.Text = picked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Validate do dai TRANSFER_MEDI_ORG_CODE truoc khi luu. Tra ve true neu hop le.
        /// </summary>
        internal bool ValidateTransferMediOrgCode()
        {
            try
            {
                if (txtTransferMediOrgCode == null) return true;
                string value = (txtTransferMediOrgCode.Text ?? string.Empty).Trim();
                if (value.Length > TransferMediOrgCodeMaxLength)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.MaCSKCBChuyenToiDa10KyTu,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTransferMediOrgCode.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        /// <summary>
        /// Hien thi gia tri TRANSFER_MEDI_ORG_CODE cua lo mau hien tai len control.
        /// Goi sau khi load currentBlood.
        ///
        /// Luu y: V_HIS_BLOOD (view) KHONG co cot TRANSFER_MEDI_ORG_CODE — chi co o HIS_BLOOD (table).
        /// PTTK chi them cot vao bang nen cot khong xuat hien o view. Vi vay phai goi API
        /// api/HisBlood/Get de lay HIS_BLOOD (table) day du, dong nhat voi pattern Save
        /// tai frmBloodUpdate.cs (GetHisBlood + CommitTransferMediOrgCodeToBlood + Update).
        /// </summary>
        internal void LoadTransferMediOrgCodeFromBlood(V_HIS_BLOOD blood)
        {
            try
            {
                if (txtTransferMediOrgCode == null) return;
                if (blood == null || blood.ID <= 0)
                {
                    txtTransferMediOrgCode.Text = string.Empty;
                    return;
                }

                // V_HIS_BLOOD thieu cot — load tu HIS_BLOOD (table) qua API. Reuse GetHisBlood (private partial).
                HIS_BLOOD hisBlood = new HIS_BLOOD();
                GetHisBlood(blood.ID, ref hisBlood);

                txtTransferMediOrgCode.Text = hisBlood == null
                    ? string.Empty
                    : (hisBlood.TRANSFER_MEDI_ORG_CODE ?? string.Empty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ghi gia tri tren control vao HIS_BLOOD truoc khi goi api/HisBlood/Update.
        /// </summary>
        internal void CommitTransferMediOrgCodeToBlood(HIS_BLOOD hisBlood)
        {
            try
            {
                if (txtTransferMediOrgCode == null || hisBlood == null) return;
                string value = (txtTransferMediOrgCode.Text ?? string.Empty).Trim();
                hisBlood.TRANSFER_MEDI_ORG_CODE = string.IsNullOrEmpty(value) ? null : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
