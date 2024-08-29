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
