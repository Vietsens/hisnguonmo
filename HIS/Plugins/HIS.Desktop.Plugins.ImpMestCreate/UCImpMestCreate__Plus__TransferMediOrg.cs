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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using HIS.UC.MediOrgPicker;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate
    {
        internal const int TransferMediOrgCodeMaxLength = 10;
        internal ButtonEdit txtTransferMediOrgCode;
        private LayoutControlItem lciTransferMediOrgCode;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider transferMediOrgErrorProvider;

        /// <summary>
        /// Tao ButtonEdit "CSKCB chuyen" + LayoutControlItem va chen vao layoutControlGroup5
        /// (nhom chua spnTemperature, cboMedicineUseForm, ...). Goi tu constructor sau InitializeComponent().
        /// </summary>
        /// <summary>
        /// Cot "CSKCB chuyen" o cuoi grid danh sach thuoc da them ben phai.
        /// FieldName = "TRANSFER_MEDI_ORG_CODE" — bind thang vao service ADO.
        /// </summary>
        private DevExpress.XtraGrid.Columns.GridColumn gridColTransferMediOrgCode;

        /// <summary>
        /// Them cot "CSKCB chuyen" vao cuoi gridViewImpMestDetail neu chua co.
        /// Goi tu InitTransferMediOrgCodeControl() (sau InitializeComponent).
        /// </summary>
        private void EnsureTransferMediOrgCodeGridColumn()
        {
            try
            {
                if (gridViewImpMestDetail == null) return;
                if (gridColTransferMediOrgCode != null) return;
                // Khong them lai neu da co cot cung FieldName (vi du da chen tu lan truoc).
                foreach (DevExpress.XtraGrid.Columns.GridColumn c in gridViewImpMestDetail.Columns)
                {
                    if (c.FieldName == "TRANSFER_MEDI_ORG_CODE") { gridColTransferMediOrgCode = c; return; }
                }

                gridColTransferMediOrgCode = new DevExpress.XtraGrid.Columns.GridColumn();
                gridColTransferMediOrgCode.Caption = "CSKCB chuyển";
                gridColTransferMediOrgCode.FieldName = "TRANSFER_MEDI_ORG_CODE";
                gridColTransferMediOrgCode.Name = "gridColumn_ImpMestDetail_TransferMediOrgCode";
                gridColTransferMediOrgCode.OptionsColumn.AllowEdit = false;
                gridColTransferMediOrgCode.Visible = true;
                gridColTransferMediOrgCode.Width = 120;
                gridColTransferMediOrgCode.VisibleIndex = gridViewImpMestDetail.Columns.Count;
                gridViewImpMestDetail.Columns.Add(gridColTransferMediOrgCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void InitTransferMediOrgCodeControl()
        {
            try
            {
                EnsureTransferMediOrgCodeGridColumn();
                if (txtTransferMediOrgCode != null) return;

                txtTransferMediOrgCode = new ButtonEdit();
                txtTransferMediOrgCode.Name = "txtTransferMediOrgCode";
                // Khong dat MaxLength: cho phep go nhieu hon 10 ky tu, validate hien thi
                // canh bao + chan luu o cho khac.
                txtTransferMediOrgCode.Properties.Buttons.Clear();
                txtTransferMediOrgCode.Properties.Buttons.AddRange(new EditorButton[]
                {
                    new EditorButton(ButtonPredefines.Plus)
                });
                txtTransferMediOrgCode.ButtonClick += TxtTransferMediOrgCode_ButtonClick;
                txtTransferMediOrgCode.EditValueChanged += TxtTransferMediOrgCode_EditValueChanged;

                lciTransferMediOrgCode = new LayoutControlItem();
                lciTransferMediOrgCode.Name = "lciTransferMediOrgCode";
                lciTransferMediOrgCode.Text = "CSKCB chuyển:";
                lciTransferMediOrgCode.Control = txtTransferMediOrgCode;
                lciTransferMediOrgCode.AppearanceItemCaption.Options.UseTextOptions = true;
                lciTransferMediOrgCode.AppearanceItemCaption.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Far;
                lciTransferMediOrgCode.TextAlignMode = TextAlignModeItem.CustomSize;
                lciTransferMediOrgCode.TextSize = new System.Drawing.Size(80, 20);
                lciTransferMediOrgCode.TextToControlDistance = 5;
                lciTransferMediOrgCode.MinSize = new System.Drawing.Size(150, 24);
                lciTransferMediOrgCode.MaxSize = new System.Drawing.Size(0, 24);
                lciTransferMediOrgCode.SizeConstraintsType =
                    DevExpress.XtraLayout.SizeConstraintsType.Custom;

                if (this.layoutControl1 != null)
                {
                    this.layoutControl1.Controls.Add(txtTransferMediOrgCode);
                }

                // Vi tri mong muon: cung hang voi "Nhiet do" + "So lan TSD tinh gia",
                // cot 3 (thang hang voi "Dang bao che" o tren).
                // -> Chen vao layoutControlGroup6 (chua TSD tinh gia), ngay ben phai layoutControlItem34.
                if (this.layoutControlGroup6 != null && this.layoutControlItem34 != null)
                {
                    this.layoutControlGroup6.AddItem(lciTransferMediOrgCode);
                    lciTransferMediOrgCode.Move(this.layoutControlItem34,
                        DevExpress.XtraLayout.Utils.InsertType.Right);
                }
                else if (this.layoutControlGroup5 != null)
                {
                    // Fallback: neu khong tim duoc anchor thi van add vao group goc.
                    this.layoutControlGroup5.AddItem(lciTransferMediOrgCode);
                }

                // Realtime validation: tao DXErrorProvider rieng de hien icon canh bao
                // ngay khi user go (EditValueChanged), khong cho icon dung nhau voi nut "+".
                transferMediOrgErrorProvider =
                    new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
                transferMediOrgErrorProvider.ContainerControl = this;
                transferMediOrgErrorProvider.SetIconAlignment(
                    txtTransferMediOrgCode,
                    System.Windows.Forms.ErrorIconAlignment.MiddleRight);
                // Trigger ngay lan dau de clear / set neu da co text san.
                UpdateTransferMediOrgErrorState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTransferMediOrgCode_EditValueChanged(object sender, EventArgs e)
        {
            UpdateTransferMediOrgErrorState();
        }

        /// <summary>
        /// Set/clear icon canh bao tren control dua tren do dai hien tai.
        /// Goi tu EditValueChanged va sau khi load gia tri tu ado.
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
                        string.Format("Mã CSKCB chuyển tối đa {0} ký tự", TransferMediOrgCodeMaxLength),
                        DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                }
                else
                {
                    transferMediOrgErrorProvider.SetError(txtTransferMediOrgCode, "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        /// Validate do dai TRANSFER_MEDI_ORG_CODE truoc khi commit. Tra ve true neu hop le.
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
                        string.Format("Mã CSKCB chuyển tối đa {0} ký tự", TransferMediOrgCodeMaxLength),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTransferMediOrgCode.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        /// <summary>
        /// Hien thi gia tri TRANSFER_MEDI_ORG_CODE cua ado hien tai len control.
        /// Goi sau khi user chon mot dong tu cay thuoc.
        /// </summary>
        internal void LoadTransferMediOrgCodeFromAdo()
        {
            try
            {
                if (txtTransferMediOrgCode == null) return;
                txtTransferMediOrgCode.Text = this.currrentServiceAdo == null
                    ? string.Empty
                    : (this.currrentServiceAdo.TRANSFER_MEDI_ORG_CODE ?? string.Empty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ghi gia tri tren control vao ado. Goi truoc khi luu hoac truoc khi them dong.
        /// </summary>
        internal void CommitTransferMediOrgCodeToAdo()
        {
            try
            {
                if (txtTransferMediOrgCode == null || this.currrentServiceAdo == null) return;
                string value = (txtTransferMediOrgCode.Text ?? string.Empty).Trim();
                this.currrentServiceAdo.TRANSFER_MEDI_ORG_CODE =
                    string.IsNullOrEmpty(value) ? null : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
