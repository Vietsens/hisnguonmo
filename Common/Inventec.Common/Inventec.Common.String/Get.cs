using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.String
{
    public class Get
    {
        /// <summary>
        /// Lay ve tu dau tien trong chuoi ky tu.
        /// Tu dau tien duoc dinh nghia la:
        /// - Bo qua cac khoang trang o dau chuoi neu co.
        /// - Tinh tu ky tu dau tien khong phai khoang trang.
        /// - Cho den ky tu ngay truoc ky tu dau tien la khoang trang.
        /// - Vi du: "   jfasdf laskf asl" --> "jfasdf".
        /// - Vi du: "   jfasdf, laskf asl" --> "jfasdf,".
        /// - Vi du: "   jfasdf.12312,as12 laskf asl" --> "jfasdf.12312,as12".
        /// Neu chuoi rong hoac toan khoang trang, se tra ve xau rong.
        /// Neu co exception, se tra ve xau rong.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetFirstWord(string text)
        {
            string result = "";
            try
            {
                if (!System.String.IsNullOrWhiteSpace(text))
                {
                    text = text.TrimStart();
                    int length = text.Length;
                    int i = 0;
                    while (i < length)
                    {
                        if (text[i] == ' ') break;
                        result += text[i];
                        i++;
                    }
                }
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        public static string GetAndRemoveFirstWord(ref string text)
        {
            string result = "";
            try
            {
                if (!System.String.IsNullOrWhiteSpace(text))
                {
                    text = text.TrimStart();
                    int length = text.Length;
                    int i = 0;
                    while (i < length)
                    {
                        if (text[i] == ' ') break;
                        result += text[i];
                        i++;
                    }
                    text = text.Remove(0, result.Length); //Cat tu dau tien tim duoc
                    text = text.TrimStart(); //Cat khoang trang tai dau xau
                }
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }
    }
}
