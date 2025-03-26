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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventec.Common.String
{
    public class Check
    {
        const string specialUnicodeText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
        public static bool IsLetterOrDigitOrUnicode(string text)
        {
            bool valid = false;
            try
            {
                Regex r = new Regex("^[a-zA-Z0-9--\\|/^!~?#@_&';:.,()&*]*$");
                var arrTextChar = text.ToArray();
                valid = (arrTextChar.Where(o => specialUnicodeText.Contains(o.ToString()) || r.IsMatch(o.ToString()) || o.ToString().Trim() == "").Count() == arrTextChar.Count() || r.IsMatch(text));
            }
            catch (Exception ex)
            {
                valid = false;
            }
            return valid;
        }
    }
}
