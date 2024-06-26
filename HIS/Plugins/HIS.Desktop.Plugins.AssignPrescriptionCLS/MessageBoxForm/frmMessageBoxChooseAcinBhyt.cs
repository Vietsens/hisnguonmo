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

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.MessageBoxForm
{
    public partial class frmMessageBoxChooseAcinBhyt : DevExpress.XtraEditors.XtraForm
    {
        ChonThuocTrongKhoCungHoatChat chonThuocTrongKhoCungHoatChat;
        public frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat _chonThuocTrongKhoCungHoatChat)
        {
            InitializeComponent();
            this.chonThuocTrongKhoCungHoatChat = _chonThuocTrongKhoCungHoatChat;
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
