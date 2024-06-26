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
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CustomerRequest
{
    public partial class frmThemBinhLuan : Form
    {
        YCKH currentYCKH = new YCKH();
        BINH_LUAN currentBinhLuan ;
        HIS.Desktop.Common.DelegateReturnSuccess dataSelect;
        bool isNew;
        public frmThemBinhLuan(HIS.Desktop.Common.DelegateReturnSuccess dataSelect_, YCKH __YCKH, BINH_LUAN _BINH_LUAN, bool _isNew)
        {
            try
            {
                InitializeComponent();
                this.currentYCKH = __YCKH;
                this.dataSelect = dataSelect_;
                this.currentBinhLuan = _BINH_LUAN;
                this.isNew = _isNew;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmThemBinhLuan(HIS.Desktop.Common.DelegateReturnSuccess dataSelect_, YCKH __YCKH)
        {
            try
            {
                InitializeComponent();
                this.currentYCKH = __YCKH;
                this.dataSelect = dataSelect_;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmThemBinhLuan_Load(object sender, EventArgs e)
        {
            try
            {
                if (currentBinhLuan != null)
                {
                    txtNoiDungBinhLuan.Text = currentBinhLuan.nội_dung;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                BINH_LUAN updateDTO = new BINH_LUAN();
                updateDTO.nội_dung = txtNoiDungBinhLuan.Text;
                string uri = isNew ? "ords/vietsens/binhluan/yckh/" + currentYCKH.id : "ords/vietsens/binhluan/binhluan/" + currentBinhLuan.id;
                var apiEditComment = isNew ? HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.PostWithouApiParam<string>(uri, updateDTO, 0, null):HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.PutWithouApiParam<string>(uri, updateDTO, 0, null);
                if (string.IsNullOrEmpty(apiEditComment))
                {
                    dataSelect(true);
                    this.Close();
                    MessageManager.Show(this.ParentForm, param, true);
                }
                else
                {
                    MessageBox.Show(apiEditComment, "Thông báo");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
