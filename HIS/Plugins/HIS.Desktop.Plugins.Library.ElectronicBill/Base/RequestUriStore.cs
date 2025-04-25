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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class RequestUriStore
    {
        internal const string MobifoneLogin = "/api/Account/Login";
        internal const string MobifoneGetDataReferencesByRefId = "/api/System/GetDataReferencesByRefId?refId=RF00059";
        internal const string MobifoneSaveListHoadon78 = "/api/Invoice68/SaveListHoadon78";
        internal const string MobifoneSignInvoiceCertFile68 = "/api/Invoice68/SignInvoiceCertFile68";
        internal const string MobifoneInHoadon = "/api/Invoice68/inHoadon?id={0}&type=PDF&inchuyendoi={1}";//api/Invoice68/inHoadon?id={hdon_id}&type=PDF&inchuyendoi=true
        internal const string MobifoneuploadCanceledInv = "/api/Invoice68/uploadCanceledInv?id={0}";//api/Invoice68/uploadCanceledInv?id={hdon_id}

        //CYBERBILL
        internal const string CyberbillLogin = "api/services/hddtws/Authentication/GetToken";
        internal const string CyberbillGuiHoadonGoc = "api/services/hddtws/GuiHoadon/GuiHoadonGoc";
        internal const string CyberbillKyHoaDonHSM = "api/services/hddtws/XuLyHoaDon/KyHoaDonHSM";
        internal const string CyberbillChuyenDoiHoaDon = "api/services/hddtws/QuanLyHoaDon/ChuyenDoiHoaDon"; //"api/services/hddtws/GuiHoaDon/ChuyenDoiHoaDon";
        internal const string CyberbillTaiHoaDon = "api/services/hddtws/GuiHoaDon/TaiHoaDonPDF";
        internal const string CyberbillHuyHoaDon = "api/services/hddtws/GuiHoaDon/GuiHoadonHuyBo";
        internal static string CombileUrl(params string[] data)
        {
            string result = "";
            List<string> pathUrl = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                pathUrl.Add(data[i].Trim('/'));
            }

            result = string.Join("/", pathUrl);
            return result;
        }
    }
}
