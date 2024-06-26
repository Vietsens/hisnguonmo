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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmMessageBoxChooseAcinBhyt : DevExpress.XtraEditors.XtraForm
    {
        ChonThuocTrongKhoCungHoatChat chonThuocTrongKhoCungHoatChat;
        public frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat _chonThuocTrongKhoCungHoatChat)
        {
            InitializeComponent();
            this.chonThuocTrongKhoCungHoatChat = _chonThuocTrongKhoCungHoatChat;
            SetCaptionByLanguageKey();
        }

        private void frmMessageBoxChooseMedicineTypeAcin_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.chonThuocTrongKhoCungHoatChat == null)
                {
                    throw new ArgumentNullException("chonThuocTrongKhoCungHoatChat is null");
                }
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMessageBoxChooseAcinBhyt).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseAcinBhyt.lblDescription.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt, LanguageManager.GetCulture());
                this.btnChonThuocCungHoatChat.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseAcinBhyt.btnChonThuocCungHoatChat.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt, LanguageManager.GetCulture());
                this.btnChonThuocNgoaiKho.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseAcinBhyt.btnChonThuocNgoaiKho.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseAcinBhyt.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseAcinBhyt.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseAcinBhyt, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonThuocCungHoatChat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chonThuocTrongKhoCungHoatChat != null)
                {
                    this.chonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe.ThuocCungHoatChat);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonThuocNgoaiKho_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chonThuocTrongKhoCungHoatChat != null)
                {
                    this.chonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe.ThuocNgoaiKho);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chonThuocTrongKhoCungHoatChat != null)
                {
                    this.chonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe.None);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
