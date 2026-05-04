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
using HIS.Desktop.Plugins.XMLViewer130.Bhxh;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.XMLViewer130
{
    public partial class frmBhxhSendResult : XtraForm
    {
        public frmBhxhSendResult(BhxhCategoryResultADO result)
        {
            InitializeComponent();
            try
            {
                DisplayResult(result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisplayResult(BhxhCategoryResultADO result)
        {
            try
            {
                if (result == null) return;

                bool isSuccess = result.maKetQua == "200";

                lblStatus.Text = isSuccess ? "Gửi thành công" : "Gửi thất bại";
                lblStatus.ForeColor = isSuccess ? Color.FromArgb(0, 128, 0) : Color.FromArgb(200, 0, 0);

                txtMaKetQua.Text = result.maKetQua ?? "";
                txtMaGiaoDich.Text = result.maGiaoDich ?? "";
                txtThongDiep.Text = result.thongDiep ?? "";

                string thoiGian = result.thoiGianTiepNhan ?? "";
                if (thoiGian.Length == 14)
                {
                    thoiGian = thoiGian.Substring(6, 2) + "/" + thoiGian.Substring(4, 2) + "/" +
                               thoiGian.Substring(0, 4) + "  " + thoiGian.Substring(8, 2) + ":" +
                               thoiGian.Substring(10, 2) + ":" + thoiGian.Substring(12, 2);
                }
                txtThoiGianTiepNhan.Text = thoiGian;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
