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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utilities.RemoteSupport;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    /// <summary>
    /// vCong57682 - Hop thoai them hoac sua mot noi dung trao doi.
    /// Dong he thong (sinh ra khi chuyen viec) khong di qua man hinh nay.
    /// </summary>
    public partial class frmSupportRequestCmt : Form
    {
        readonly long supportRequestId;
        readonly MOS.SDO.HisSupportRequestCmtViewSDO editingRow;

        public frmSupportRequestCmt(long supportRequestId, MOS.SDO.HisSupportRequestCmtViewSDO editingRow)
        {
            InitializeComponent();
            this.supportRequestId = supportRequestId;
            this.editingRow = editingRow;
        }

        private void frmSupportRequestCmt_Load(object sender, EventArgs e)
        {
            try
            {
                if (editingRow != null)
                {
                    this.Text = "Sửa nội dung trao đổi";
                    memContent.Text = editingRow.CONTENT;
                }
                else
                {
                    this.Text = "Thêm nội dung trao đổi";
                }
                memContent.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (string.IsNullOrWhiteSpace(memContent.Text))
                {
                    XtraMessageBox.Show("Nội dung trao đổi không được để trống", "Thông báo");
                    memContent.Focus();
                    return;
                }

                MOS.SDO.HisSupportRequestCmtSDO sdo = new MOS.SDO.HisSupportRequestCmtSDO();
                sdo.SupportRequestId = supportRequestId;
                sdo.Content = memContent.Text.Trim();
                string url = ApiUrl.HIS_SUPPORT_REQUEST_CMT__CREATE;

                if (editingRow != null)
                {
                    sdo.Id = editingRow.ID;
                    url = ApiUrl.HIS_SUPPORT_REQUEST_CMT__UPDATE;
                }

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_SUPPORT_REQUEST_CMT>(url, ApiConsumers.MosConsumer, sdo, param);
                success = result != null;
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                MessageManager.Show(this, param, false);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
