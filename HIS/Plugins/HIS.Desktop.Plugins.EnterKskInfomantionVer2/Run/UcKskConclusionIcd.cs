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
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// UserControl TÁI SỬ DỤNG: "Kết luận theo bệnh (ICD - 10)".
    /// Gồm 3 lựa chọn (1=Chưa phát hiện bất thường, 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định)
    /// + ô chọn mã ICD (nhúng UCSecondaryIcd) + nút "..." mở popup chọn bệnh.
    /// Map 1-1 sang HIS_KSK_GENERAL: CONCLUSION_ICD_TYPE / CONCLUSION_ICD_CODE / CONCLUSION_ICD_NAME.
    /// Dùng chung cho mọi tab KSK. Sau khi add vào host panel, GỌI <see cref="InitUc"/>.
    /// </summary>
    public partial class UcKskConclusionIcd : DevExpress.XtraEditors.XtraUserControl
    {
        private SecondaryIcdProcessor subIcdProcessor;
        private UserControl ucSecondaryIcd;
        private bool isInited = false;
        // 2 ô TextEdit bên trong UCSecondaryIcd (ô mã + ô tên bệnh) — để chặn F1 + đổi placeholder khi disable.
        private DevExpress.XtraEditors.TextEdit innerIcdNameEdit;
        private DevExpress.XtraEditors.TextEdit innerIcdCodeEdit;
        private const string ICD_PROMPT_ENABLED = "Nhấn F1 để chọn bệnh";
        private const string ICD_PROMPT_DISABLED = "Chỉ dùng khi kết luận rõ theo mã ICD";

        public UcKskConclusionIcd()
        {
            InitializeComponent();
        }

        /// <summary>Nhúng UCSecondaryIcd + áp hành vi (bỏ chọn radio, readonly theo lựa chọn). Gọi 1 lần.</summary>
        public void InitUc()
        {
            if (isInited) return;
            try
            {
                // RadioGroup: bỏ border + cho bỏ chọn (chuột phải / Delete / click lại ô đang chọn)
                this.rdoIcdConclusion.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                this.rdoIcdConclusion.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
                this.rdoIcdConclusion.Properties.Appearance.Options.UseBackColor = true;
                this.rdoIcdConclusion.EditValue = null;
                this.rdoIcdConclusion.KeyDown -= Rdo_KeyDown; this.rdoIcdConclusion.KeyDown += Rdo_KeyDown;
                this.rdoIcdConclusion.MouseDown -= Rdo_MouseDown; this.rdoIcdConclusion.MouseDown += Rdo_MouseDown;
                this.rdoIcdConclusion.MouseWheel -= Rdo_MouseWheel; this.rdoIcdConclusion.MouseWheel += Rdo_MouseWheel;
                this.rdoIcdConclusion.EditValueChanged -= rdoIcdConclusion_EditValueChanged;
                this.rdoIcdConclusion.EditValueChanged += rdoIcdConclusion_EditValueChanged;

                // Nhúng UCSecondaryIcd (chọn mã ICD-10) — mẫu chuẩn (AssignNutrition...)
                this.subIcdProcessor = new SecondaryIcdProcessor(
                    new CommonParam(),
                    BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                SecondaryIcdInitADO ado = new SecondaryIcdInitADO();
                ado.Width = (this.pnlSecondaryIcd.Width > 0) ? this.pnlSecondaryIcd.Width : 345;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ:";
                ado.TextSize = 30;
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcd = (UserControl)this.subIcdProcessor.Run(ado);
                if (this.ucSecondaryIcd != null)
                {
                    this.pnlSecondaryIcd.Controls.Add(this.ucSecondaryIcd);
                    this.ucSecondaryIcd.Dock = DockStyle.Fill;
                    // Lấy 2 ô TextEdit bên trong UCSecondaryIcd để điều khiển enable/placeholder (chặn F1).
                    this.innerIcdNameEdit = FindEditByName(this.ucSecondaryIcd, "txtIcdText");
                    this.innerIcdCodeEdit = FindEditByName(this.ucSecondaryIcd, "txtIcdSubCode");
                }
                isInited = true;
                UpdateState();
                RelayoutByWidth();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RelayoutByWidth();
        }

        /// <summary>
        /// Radio hiển thị HÀNG NGANG khi đủ rộng (≥720px), ngược lại xuống DỌC (panel hẹp như cột phải trẻ &lt;6t).
        /// Đồng thời dời ô chọn ICD + nút xuống dưới hàng radio cho khớp.
        /// </summary>
        private void RelayoutByWidth()
        {
            try
            {
                if (rdoIcdConclusion == null || pnlSecondaryIcd == null || btnChooseIcd == null) return;
                int w = this.ClientSize.Width;
                bool horizontal = (w >= 720);
                rdoIcdConclusion.Properties.Columns = horizontal ? 3 : 1;
                int rdoH = horizontal ? 26 : 70;
                if (rdoIcdConclusion.Height != rdoH) rdoIcdConclusion.Height = rdoH;
                int gap = horizontal ? 4 : 6;
                int rowTop = rdoIcdConclusion.Top + rdoH + gap;
                pnlSecondaryIcd.Top = rowTop;
                btnChooseIcd.Top = rowTop;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #region ===== Public API (map HIS_KSK_GENERAL) =====

        /// <summary>
        /// Đổi tiêu đề khung. Mặc định "Kết luận theo bệnh (ICD - 10)"; tab Khám lâm sàng HCM
        /// dùng lại cụm này cho từng chuyên khoa nên đặt tiêu đề là tên chuyên khoa.
        /// </summary>
        public void SetCaption(string caption)
        {
            try
            {
                if (this.grpIcd != null && !string.IsNullOrEmpty(caption)) this.grpIcd.Text = caption;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Bỏ khung + tiêu đề của cụm và dồn nội dung lên trên — dùng khi cụm được đặt SẴN TRONG
        /// khung của mục khác (tab Khám lâm sàng HCM), tránh khung lồng khung.
        /// Sau khi gọi, chiều cao vừa đủ khoảng 62px.
        /// </summary>
        public void SetFrameless()
        {
            try
            {
                if (this.grpIcd != null)
                {
                    this.grpIcd.ShowCaption = false;
                    this.grpIcd.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                }
                if (this.rdoIcdConclusion != null) this.rdoIcdConclusion.Top = 2;
                RelayoutByWidth();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Cố định bề rộng hàng 3 lựa chọn (bỏ neo phải) để khi cụm được kéo rộng thì CHỈ ô chọn mã ICD
        /// dài ra, còn 3 lựa chọn vẫn nằm gần nhau. Gọi sau khi đã đặt Size cho cụm.
        /// </summary>
        public void SetCompactOptions(int optionsWidth)
        {
            try
            {
                if (this.rdoIcdConclusion == null) return;
                this.rdoIcdConclusion.Anchor = System.Windows.Forms.AnchorStyles.Top
                                             | System.Windows.Forms.AnchorStyles.Left;
                if (optionsWidth > 0) this.rdoIcdConclusion.Width = optionsWidth;
                RelayoutByWidth();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>1=Chưa phát hiện, 2=Sơ bộ, 3=Xác định (null nếu chưa chọn).</summary>
        public long? GetConclusionIcdType()
        {
            return GetRadioValue(this.rdoIcdConclusion);
        }

        public string GetConclusionIcdCode()
        {
            var d = GetSecondaryIcdValue();
            return d != null ? d.ICD_SUB_CODE : null;
        }

        public string GetConclusionIcdName()
        {
            var d = GetSecondaryIcdValue();
            return d != null ? d.ICD_TEXT : null;
        }

        /// <summary>Đổ dữ liệu vào UC.</summary>
        public void SetData(long? icdType, string icdCode, string icdName)
        {
            try
            {
                SetRadioValue(this.rdoIcdConclusion, icdType);
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                {
                    SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                    data.ICD_SUB_CODE = icdCode;
                    data.ICD_TEXT = icdName;
                    this.subIcdProcessor.Reload(this.ucSecondaryIcd, data);
                }
                UpdateState();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Gom giá trị ICD-10 vào bản ghi HIS_KSK_GENERAL của lượt khám.</summary>
        public void FillToGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            try
            {
                long? t = GetConclusionIcdType();
                g.CONCLUSION_ICD_TYPE = t.HasValue ? (short?)t.Value : (short?)null;
                // Chỉ lưu mã/tên ICD khi chẩn đoán sơ bộ (2) hoặc xác định (3); ngược lại để null.
                bool needIcd = (t == 2 || t == 3);
                g.CONCLUSION_ICD_CODE = needIcd ? NullIfEmpty(GetConclusionIcdCode()) : null;
                g.CONCLUSION_ICD_NAME = needIcd ? NullIfEmpty(GetConclusionIcdName()) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ ICD-10 từ bản ghi HIS_KSK_GENERAL vào UC.</summary>
        public void LoadFromGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            SetData(g.CONCLUSION_ICD_TYPE, g.CONCLUSION_ICD_CODE, g.CONCLUSION_ICD_NAME);
        }

        #endregion

        #region ===== Internal =====

        private SecondaryIcdDataADO GetSecondaryIcdValue()
        {
            try
            {
                if (this.subIcdProcessor == null || this.ucSecondaryIcd == null) return null;
                return this.subIcdProcessor.GetValue(this.ucSecondaryIcd) as SecondaryIcdDataADO;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Chỉ cho nhập/chọn mã ICD khi tích "Chẩn đoán sơ bộ" (2) hoặc "Chẩn đoán xác định" (3).
        /// Khi không cần: DISABLE ô CĐ (F1 không mở popup vì control bị tắt) + đổi placeholder hướng dẫn + xóa mã đã chọn.
        /// </summary>
        private void UpdateState()
        {
            try
            {
                long? v = GetRadioValue(this.rdoIcdConclusion);
                bool needIcd = (v == 2 || v == 3);
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                    this.subIcdProcessor.ReadOnly(this.ucSecondaryIcd, !needIcd);
                if (this.btnChooseIcd != null) this.btnChooseIcd.Enabled = needIcd;
                // Tooltip nhắc cách bỏ chọn khi radio đang được tích.
                this.rdoIcdConclusion.ToolTip = (v != null) ? "Click chuột phải để bỏ tích chọn" : "";

                // Ô CĐ chỉ enable (và F1 chỉ mở popup) khi chọn chẩn đoán sơ bộ/xác định.
                string prompt = needIcd ? ICD_PROMPT_ENABLED : ICD_PROMPT_DISABLED;
                if (this.innerIcdNameEdit != null)
                {
                    this.innerIcdNameEdit.Enabled = needIcd;
                    this.innerIcdNameEdit.Properties.NullValuePrompt = prompt;
                }
                if (this.innerIcdCodeEdit != null)
                {
                    this.innerIcdCodeEdit.Enabled = needIcd;
                    this.innerIcdCodeEdit.Properties.NullValuePrompt = prompt;
                }
                // Bỏ mã ICD khi không còn dùng → placeholder hiển thị + không lưu nhầm.
                if (!needIcd && this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                    this.subIcdProcessor.Reload(this.ucSecondaryIcd, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tìm TextEdit con theo Name bên trong UCSecondaryIcd (đệ quy).</summary>
        private DevExpress.XtraEditors.TextEdit FindEditByName(Control root, string name)
        {
            try
            {
                if (root == null) return null;
                foreach (Control c in root.Controls)
                {
                    DevExpress.XtraEditors.TextEdit te = c as DevExpress.XtraEditors.TextEdit;
                    if (te != null && te.Name == name) return te;
                    DevExpress.XtraEditors.TextEdit found = FindEditByName(c, name);
                    if (found != null) return found;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        private void rdoIcdConclusion_EditValueChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnChooseIcd_Click(object sender, EventArgs e)
        {
            try
            {
                string subCode = "", text = "";
                var cur = GetSecondaryIcdValue();
                if (cur != null)
                {
                    subCode = cur.ICD_SUB_CODE ?? "";
                    text = cur.ICD_TEXT ?? "";
                }
                int pageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                // Dùng frmSubIcd (nạp từ cache HIS_ICD đã warm sẵn ở InitUc) GIỐNG F1 -> mở nhanh, KHÔNG chạm
                // V_HIS_ICD (view lạnh + nặng khiến lần đầu ~2s). frmSubIcd trả cả mã + tên qua delegate (mã;/tên;).
                var swDots = System.Diagnostics.Stopwatch.StartNew();
                frmSubIcd frm = new frmSubIcd(
                    new HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run.DelegateRefeshIcdChandoanphu(DlgChooseIcd),
                    subCode, text, pageSize, new System.Collections.Generic.List<HIS_ICD>());
                LogSystem.Debug("KskIcdOpen[Dots]: new frmSubIcd=" + swDots.ElapsedMilliseconds + "ms");
                frm.ShowDialog();
                LogSystem.Debug("KskIcdOpen[Dots]: total(incl ShowDialog shown)=" + swDots.ElapsedMilliseconds + "ms");
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void DlgChooseIcd(string icdCodes, string icdNames)
        {
            try
            {
                if (this.subIcdProcessor == null || this.ucSecondaryIcd == null) return;
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                this.subIcdProcessor.Reload(this.ucSecondaryIcd, data);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // Chặn lăn chuột làm đổi lựa chọn RadioGroup (tránh nhảy index khi scroll).
        private void Rdo_MouseWheel(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs he = e as HandledMouseEventArgs;
            if (he != null) he.Handled = true;
        }

        // Bỏ chọn RadioGroup: Delete/Backspace
        private void Rdo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                RadioGroup rg = sender as RadioGroup;
                if (rg != null) rg.EditValue = null;
            }
        }

        // Bỏ chọn RadioGroup: chuột phải, hoặc chuột trái vào đúng ô đang chọn
        private void Rdo_MouseDown(object sender, MouseEventArgs e)
        {
            RadioGroup rg = sender as RadioGroup;
            if (rg == null) return;
            if (e.Button == MouseButtons.Right) { rg.EditValue = null; return; }
            if (e.Button == MouseButtons.Left)
            {
                object prev = rg.EditValue;
                if (prev == null) return;
                rg.BeginInvoke(new System.Action(delegate ()
                {
                    try { if (object.Equals(rg.EditValue, prev)) rg.EditValue = null; }
                    catch (Exception ex) { LogSystem.Warn(ex); }
                }));
            }
        }

        private long? GetRadioValue(RadioGroup rdo)
        {
            try
            {
                if (rdo != null && rdo.EditValue != null && rdo.EditValue != System.DBNull.Value)
                    return Convert.ToInt64(rdo.EditValue);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        private void SetRadioValue(RadioGroup rdo, long? value)
        {
            try
            {
                if (rdo == null) return;
                if (value == null) { rdo.EditValue = null; return; }
                // Gán đúng KIỂU value của item (int/long) để chọn đúng radio.
                foreach (DevExpress.XtraEditors.Controls.RadioGroupItem it in rdo.Properties.Items)
                {
                    if (it.Value != null && Convert.ToInt64(it.Value) == value.Value)
                    {
                        rdo.EditValue = it.Value;
                        return;
                    }
                }
                rdo.EditValue = (long)value.Value;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        #endregion
    }
}
