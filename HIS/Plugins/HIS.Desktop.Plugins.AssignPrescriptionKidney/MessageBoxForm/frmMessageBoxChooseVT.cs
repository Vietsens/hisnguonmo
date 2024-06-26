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

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.MessageBoxForm
{
    public partial class frmMessageBoxChooseVT : DevExpress.XtraEditors.XtraForm
    {
        ChonVTTrongKho chonVTTrongKho;
        public frmMessageBoxChooseVT(ChonVTTrongKho _chonThuocTrongKhoCungHoatChat)
        {
            InitializeComponent();
            this.chonVTTrongKho = _chonThuocTrongKhoCungHoatChat;
        }

        private void frmMessageBoxChooseMedicineTypeAcin_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.chonVTTrongKho == null)
                {
                    throw new ArgumentNullException("chonThuocTrongKhoCungHoatChat is null");
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
                if (this.chonVTTrongKho != null)
                {
                    this.chonVTTrongKho(EnumOptionChonVatTuThayThe.VatTuNgoaiKho);
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
                if (this.chonVTTrongKho != null)
                {
                    this.chonVTTrongKho(EnumOptionChonVatTuThayThe.None);
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
