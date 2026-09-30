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
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.GenerateRegisterOrder.ADO;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder.Popup
{
    /// <summary>
    /// Popup chon hinh thuc lay so tren man ki-ot.
    /// Thiet ke: PTTK_54254 muc B.4.1.2.
    ///
    /// Popup co hai buoc: chon hinh thuc, roi nhan thong tin dinh danh.
    /// Ket qua tra ve qua thuoc tinh Identity va IsNoPaper.
    /// </summary>
    public partial class frmChooseIdentity : Form
    {
        #region Declare

        /// <summary>So giay khong thao tac thi tu quay ve buoc chon hinh thuc</summary>
        private const int IDLE_TIMEOUT_SECOND = 15;

        /// <summary>Do dai hop le cua so CCCD</summary>
        private const int LENGTH_CCCD = 12;

        /// <summary>Do dai hop le cua so CMND</summary>
        private const int LENGTH_CMND = 9;

        private EnumIdentityType currentType = EnumIdentityType.None;
        private int idleSecond = 0;

        /// <summary>Thong tin dinh danh da xac dinh. Rong khi nguoi benh chon khong co giay to.</summary>
        public IdentityInfoADO Identity { get; private set; }

        /// <summary>Nguoi benh chon duong lay so khong can giay to</summary>
        public bool IsNoPaper { get; private set; }

        #endregion

        public frmChooseIdentity()
        {
            InitializeComponent();
            try
            {
                this.Identity = null;
                this.IsNoPaper = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load

        private void frmChooseIdentity_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.BuildKeypad();
                this.SetOptionButtonColor();
                this.ShowChooseStep();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap nhan hien thi theo ngon ngu dang dung.
        /// Plugin nay chua co Lang.resx rieng nen dung chung bo Message da co.
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lblTitle.Text = Resources.ResourceMessage.ChonHinhThucLaySo;
                this.btnCccd.Text = Resources.ResourceMessage.QuetQrTheCccd;
                this.btnVneId.Text = Resources.ResourceMessage.QuetQrVneId;
                this.btnCccdManual.Text = Resources.ResourceMessage.NhapTaySoCccd;
                this.btnBhyt.Text = Resources.ResourceMessage.TheBhyt;
                this.btnNoPaper.Text = Resources.ResourceMessage.KhongCoGiayTo;
                this.btnBack.Text = Resources.ResourceMessage.QuayLai;
                this.btnConfirm.Text = Resources.ResourceMessage.XacNhan;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetOptionButtonColor()
        {
            try
            {
                // O khong co giay to de mau khac de nguoi benh phan biet ngay
                this.btnNoPaper.Appearance.BackColor = Color.FromArgb(120, 120, 120);
                this.btnNoPaper.Appearance.Options.UseBackColor = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dung ban phim so tren man hinh cho hai hinh thuc go tay</summary>
        private void BuildKeypad()
        {
            try
            {
                this.tlpKeypad.SuspendLayout();
                string[] keys = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "<" };
                for (int i = 0; i < keys.Length; i++)
                {
                    SimpleButton btn = new SimpleButton();
                    btn.Text = keys[i];
                    btn.Tag = keys[i];
                    btn.Dock = DockStyle.Fill;
                    btn.Margin = new Padding(6);
                    btn.Appearance.Font = new Font("Microsoft Sans Serif", 20F, FontStyle.Bold);
                    btn.Appearance.Options.UseFont = true;
                    btn.Click += this.btnKeypad_Click;
                    this.tlpKeypad.Controls.Add(btn, i % 3, i / 3);
                }
                this.tlpKeypad.ResumeLayout(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Chuyen buoc

        /// <summary>Ve buoc chon hinh thuc, xoa sach thong tin dang giu</summary>
        private void ShowChooseStep()
        {
            try
            {
                this.currentType = EnumIdentityType.None;
                this.Identity = null;
                this.txtInput.Text = "";
                this.lblResult.Text = "";
                this.btnConfirm.Enabled = false;
                this.pnlInput.Visible = false;
                this.pnlChoose.Visible = true;
                this.lblTitle.Text = Resources.ResourceMessage.ChonHinhThucLaySo;
                this.StopIdleTimer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Sang buoc nhan thong tin theo hinh thuc da chon</summary>
        private void ShowInputStep(EnumIdentityType type)
        {
            try
            {
                this.currentType = type;
                this.Identity = null;
                this.txtInput.Text = "";
                this.lblResult.Text = "";
                this.btnConfirm.Enabled = false;

                bool isManual = (type == EnumIdentityType.CccdManual || type == EnumIdentityType.Bhyt);
                this.tlpKeypad.Visible = isManual;
                this.lblGuide.Text = this.GetGuideText(type);

                this.pnlChoose.Visible = false;
                this.pnlInput.Visible = true;
                this.txtInput.Focus();
                this.StartIdleTimer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetGuideText(EnumIdentityType type)
        {
            string result = "";
            try
            {
                switch (type)
                {
                    case EnumIdentityType.Cccd:
                        result = Resources.ResourceMessage.HuongDanQuetQrTheCccd;
                        break;
                    case EnumIdentityType.VneId:
                        result = Resources.ResourceMessage.HuongDanQuetQrVneId;
                        break;
                    case EnumIdentityType.CccdManual:
                        result = Resources.ResourceMessage.HuongDanNhapTaySoCccd;
                        break;
                    case EnumIdentityType.Bhyt:
                        result = Resources.ResourceMessage.HuongDanTheBhyt;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #endregion

        #region Xu ly chon hinh thuc

        private void btnCccd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowInputStep(EnumIdentityType.Cccd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnVneId_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowInputStep(EnumIdentityType.VneId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCccdManual_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowInputStep(EnumIdentityType.CccdManual);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBhyt_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowInputStep(EnumIdentityType.Bhyt);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNoPaper_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsNoPaper = true;
                this.Identity = null;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Xu ly buoc nhan thong tin

        private void btnKeypad_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetIdleTimer();
                SimpleButton btn = sender as SimpleButton;
                if (btn == null || btn.Tag == null)
                {
                    return;
                }

                string key = btn.Tag.ToString();
                string current = this.txtInput.Text ?? "";
                if (key == "C")
                {
                    this.txtInput.Text = "";
                }
                else if (key == "<")
                {
                    this.txtInput.Text = current.Length > 0 ? current.Substring(0, current.Length - 1) : "";
                }
                else
                {
                    this.txtInput.Text = current + key;
                }
                this.lblResult.Text = "";
                this.btnConfirm.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                this.ResetIdleTimer();
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    this.ProcessInput();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doc chuoi vua quet hoac vua go, dung dung bo doc ma QR dung chung cua he thong.
        /// Khong tu viet lai ham tach chuoi.
        /// </summary>
        private void ProcessInput()
        {
            try
            {
                string input = (this.txtInput.Text ?? "").Trim();
                if (String.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                IdentityInfoADO info = null;
                switch (this.currentType)
                {
                    case EnumIdentityType.Cccd:
                    case EnumIdentityType.VneId:
                        info = this.ReadFromCccdQrCode(input);
                        break;
                    case EnumIdentityType.CccdManual:
                        info = this.ReadFromCccdManual(input);
                        break;
                    case EnumIdentityType.Bhyt:
                        info = this.ReadFromHeinCard(input);
                        break;
                    default:
                        break;
                }

                if (info == null)
                {
                    this.ShowInputError();
                    return;
                }

                info.SelectedType = this.currentType;
                info.IssueTime = Inventec.Common.DateTime.Get.Now().ToString();
                this.Identity = info;

                this.lblResult.Appearance.ForeColor = Color.Green;
                this.lblResult.Appearance.Options.UseForeColor = true;
                this.lblResult.Text = String.IsNullOrWhiteSpace(info.PatientName)
                    ? info.GetMaskedNumber()
                    : info.PatientName + Environment.NewLine + info.GetMaskedNumber();
                this.btnConfirm.Enabled = true;
                this.btnConfirm.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.ShowInputError();
            }
        }

        /// <summary>Chuoi QR the can cuoc va chuoi QR tren ung dung VNeID dung chung mot dinh dang</summary>
        private IdentityInfoADO ReadFromCccdQrCode(string input)
        {
            try
            {
                if (!ReadQrCodeCCCD.IsQrCodeCccd(input))
                {
                    return null;
                }

                CccdCardData card = ReadQrCodeCCCD.ReadDataQrCode(input);
                if (card == null || !this.IsValidCccdNumber(card.CardData))
                {
                    return null;
                }

                IdentityInfoADO info = new IdentityInfoADO();
                info.IdentityType = (this.currentType == EnumIdentityType.VneId)
                    ? IdentityTypeCode.VNEID
                    : IdentityTypeCode.CCCD;
                info.InputMode = IdentityInputMode.QR;
                info.IdentityNumber = card.CardData;
                info.OldIdNumber = card.CmndData;
                info.PatientName = card.PatientName;
                info.Dob = card.Dob;
                info.Gender = card.Gender;
                info.Address = card.Address;
                info.IssueDate = card.ReleaseDate;
                info.RawData = input;
                return info;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private IdentityInfoADO ReadFromCccdManual(string input)
        {
            try
            {
                if (!this.IsValidCccdNumber(input))
                {
                    return null;
                }

                IdentityInfoADO info = new IdentityInfoADO();
                info.IdentityType = IdentityTypeCode.CCCD;
                info.InputMode = IdentityInputMode.MANUAL;
                info.IdentityNumber = input;
                return info;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>The BHYT nhan ca chuoi QR quet duoc lan ma the go tay</summary>
        private IdentityInfoADO ReadFromHeinCard(string input)
        {
            try
            {
                IdentityInfoADO info = new IdentityInfoADO();
                info.IdentityType = IdentityTypeCode.BHYT;

                if (input.Contains("|"))
                {
                    HeinCardData card = new ReadQrCodeHeinCard().ReadDataQrCode(input);
                    if (card == null || !this.IsValidHeinCardNumber(card.HeinCardNumber))
                    {
                        return null;
                    }

                    info.InputMode = IdentityInputMode.QR;
                    info.IdentityNumber = card.HeinCardNumber;
                    info.PatientName = card.PatientName;
                    info.Dob = card.Dob;
                    info.Gender = card.Gender;
                    info.Address = card.Address;
                    info.HeinMediOrgCode = card.MediOrgCode;
                    info.HeinFromDate = card.FromDate;
                    info.HeinToDate = card.ToDate;
                    info.RawData = input;
                    return info;
                }

                if (!this.IsValidHeinCardNumber(input))
                {
                    return null;
                }
                info.InputMode = IdentityInputMode.MANUAL;
                info.IdentityNumber = input;
                return info;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>So CCCD 12 so hoac so CMND 9 so, chi chua chu so</summary>
        private bool IsValidCccdNumber(string number)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(number))
                {
                    return false;
                }
                number = number.Trim();
                if (number.Length != LENGTH_CCCD && number.Length != LENGTH_CMND)
                {
                    return false;
                }
                return number.All(Char.IsDigit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>Ma the BHYT dai 10, 15 hoac 17 ky tu</summary>
        private bool IsValidHeinCardNumber(string number)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(number))
                {
                    return false;
                }
                number = number.Trim().Replace(" ", "");
                return number.Length == 10 || number.Length == 15 || number.Length == 17;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void ShowInputError()
        {
            try
            {
                this.Identity = null;
                this.btnConfirm.Enabled = false;
                this.txtInput.Text = "";
                this.lblResult.Appearance.ForeColor = Color.Red;
                this.lblResult.Appearance.Options.UseForeColor = true;
                this.lblResult.Text = (this.currentType == EnumIdentityType.Bhyt)
                    ? Resources.ResourceMessage.MaTheBhytKhongHopLe
                    : Resources.ResourceMessage.SoCccdKhongHopLe;
                this.txtInput.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowChooseStep();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Identity == null || !this.Identity.HasIdentity())
                {
                    return;
                }
                this.IsNoPaper = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Khoang cho khong thao tac

        private void StartIdleTimer()
        {
            try
            {
                this.idleSecond = 0;
                this.tmrIdle.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetIdleTimer()
        {
            this.idleSecond = 0;
        }

        private void StopIdleTimer()
        {
            try
            {
                this.idleSecond = 0;
                this.tmrIdle.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmrIdle_Tick(object sender, EventArgs e)
        {
            try
            {
                this.idleSecond++;
                if (this.idleSecond < IDLE_TIMEOUT_SECOND)
                {
                    return;
                }
                // Khong thao tac qua lau thi tra man ve buoc chon hinh thuc cho nguoi ke tiep
                this.ShowChooseStep();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Phim tat

        private void frmChooseIdentity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Escape)
                {
                    return;
                }

                e.Handled = true;
                if (this.pnlInput.Visible)
                {
                    // Dang o buoc nhan thong tin thi lui ve buoc chon hinh thuc
                    this.ShowChooseStep();
                    return;
                }

                // Dang o buoc chon hinh thuc thi dong popup, duong thoat cho quan tri
                this.IsNoPaper = false;
                this.Identity = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
