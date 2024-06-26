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
using HIS.UC.UCHeniInfo.ADO;
using MOS.SDO;
using HIS.Desktop.Utility;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo : UserControlBase
    {        
        private void CheckHSDAndTECard()
        {
            try
            {
                this.lciKhongKTHSD.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.chkKhongKTHSD.Checked = false;
                if (this.isShowCheckKhongKTHSD == "1" && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                {
                    string heinCardNumber = txtSoThe.Text;
                    heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                    heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                    if (!String.IsNullOrEmpty(heinCardNumber) && heinCardNumber.StartsWith("TE") && this.dtHeinCardToTime.DateTime.Date < DateTime.Now.Date)
                    {
                        this.lciKhongKTHSD.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDelegateCheckTT(DelegateCheckTT _dlgCheckTT)
        {
            try
            {
                if (_dlgCheckTT != null)
                {
                    this.dlgCheckTT = _dlgCheckTT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
