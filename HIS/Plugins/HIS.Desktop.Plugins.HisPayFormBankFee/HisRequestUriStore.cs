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

namespace HIS.Desktop.Plugins.HisPayFormBankFee
{
    public class HisRequestUriStore
    {
        // Backend: /Update bat buoc ID > 0 (VerifyId), KHONG phai upsert.
        // => Them moi dung /Create, sua dung /Update.
        internal const string HIS_PAY_FORM_BANK_FEE_GET = "/api/HisPayFormBankFee/Get";
        internal const string HIS_PAY_FORM_BANK_FEE_CREATE = "/api/HisPayFormBankFee/Create";
        internal const string HIS_PAY_FORM_BANK_FEE_UPDATE = "/api/HisPayFormBankFee/Update";
        internal const string HIS_PAY_FORM_BANK_FEE_DELETE = "/api/HisPayFormBankFee/Delete";
        // Khoa/mo khoa: dung ChangeLock (toggle) — KHONG dung Update vi backend chan sua ban ghi da khoa
        internal const string HIS_PAY_FORM_BANK_FEE_CHANGE_LOCK = "/api/HisPayFormBankFee/ChangeLock";
    }
}
