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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBhytBlacklist.HisBhytBlacklist
{
    public partial class frmHisBhytBlacklist : Form
    {
        internal DevExpress.XtraEditors.TextEdit txtSoThe;
        internal List<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST> blacklist;
        internal List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST> whitelist;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtSoThe != null);
                //Neu doi tuong benh nhan la Bhyt
                valid = valid && (!String.IsNullOrEmpty(txtSoThe.Text) && !String.IsNullOrEmpty(txtSoThe.Text.Trim()));
                if (valid)
                {
                    string currentValue = txtSoThe.Text.Replace(" ", "").ToUpper();
                    string heincardNumber = TrimHeinCardNumber(currentValue);
                    valid = valid && (heincardNumber.Length == 15);
                    if (!valid)
                    {
                        this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                    }
                    else //Ktra Thẻ hợp lệ là thẻ: có đầu mã thẻ nằm trong danh sách được khai báo trong HIS_BHYT_WHITE_LIST và ko nằm trong d/s các thẻ trong HIS_BHYT_BLACK_LIST
                    {
                        string heinCardNumberCode = heincardNumber.Substring(0, 3).ToString();
                        var lstWhite = whitelist.Where(p => p.BHYT_WHITELIST_CODE == heinCardNumberCode).ToList();
                        if (lstWhite == null)
                        {
                            foreach (var itemBlack in blacklist)
                            {
                                if (heincardNumber.StartsWith(itemBlack.HEIN_CARD_NUMBER))
                                {
                                    this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                                    valid = valid && false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                            valid = valid && false;
                        }
                    }
                }
                else
                    this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //return valid;
        }
        internal static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
