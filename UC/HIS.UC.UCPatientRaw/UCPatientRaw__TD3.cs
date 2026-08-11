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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCPatientRaw.ClassUCPatientRaw;
using DevExpress.Utils.Menu;
using HIS.UC.UCPatientRaw.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.DelegateRegister;
using DevExpress.XtraLayout.Utils;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        public bool TD3;

        /// <summary>
        /// Cho phep tra cuu thong tin BHYT tren cong BHXH theo so CCCD MA KHONG phu thuoc doi tuong dang chon.
        ///
        /// Mac dinh false = giu nguyen luong cu: chi tra cong khi doi tuong dang chon la BHYT.
        /// Chi man hinh Tiep don 2 gan co nay (va chi khi cau hinh toan vien duoc bat) => cac man hinh khac
        /// dung chung UC nay KHONG phat sinh them luot goi cong BHXH.
        /// Tham chieu: PTTK_XXXXX_Tu_Dong_Chuyen_Doi_Tuong_BHYT_Khi_Co_The.md
        /// </summary>
        public bool IsCheckHeinByCccdWithoutPatientType { get; set; }

        public void SetTD3(bool td3)
        {
            try
            {
                this.TD3 = td3;
                this.layoutControlItem10.Visibility = LayoutVisibility.Never;
                this.layoutControlItem11.Visibility = LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
