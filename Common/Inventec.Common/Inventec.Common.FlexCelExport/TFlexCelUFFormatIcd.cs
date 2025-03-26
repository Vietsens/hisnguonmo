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
using FlexCel.Report;
using Inventec.Common.Logging;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    /// <summary>
    /// 113628
    /// </summary>
    class TFlexCelUFFormatIcd : TFlexCelUserFunction
    {
        const int INDEX_KEY = 0;
        const int INDEX_NEXT_KEY = 1;
        const int MIN_PARAM = 2;
        const string KEY_CODE = "code";
        const string KEY_NAME = "name";
        public TFlexCelUFFormatIcd()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = System.String.Empty;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("TFlexCelUFFormatIcd");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parameters), parameters));
                if (parameters == null || parameters.Length < MIN_PARAM)
                    throw new ArgumentException("Bad parameters count in call to FuncFormatIcd user-defined function");
                string key = !string.IsNullOrEmpty(parameters[INDEX_KEY].ToString()) ? parameters[INDEX_KEY].ToString() : string.Format("{0} - {1}", KEY_CODE, KEY_NAME);
                if (!key.Contains(KEY_NAME) && !key.Contains(KEY_CODE))
                    throw new ArgumentException("Bad parameters key in call to FuncFormatIcd user-defined function");
                var args = parameters.Skip(INDEX_NEXT_KEY).Take(Int16.MaxValue).ToList();
                string formatKey = key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME) ? key.Replace(KEY_CODE, "{0}").Replace(KEY_NAME, "{1}") : key.Replace(KEY_NAME, "{0}").Replace(KEY_CODE, "{1}");
                List<string> lstResult = new List<string>();
                int CountValue = 0;
                while (args.Count - CountValue > 0)
                {
                    var Tmp = args.Skip(CountValue).Take(MIN_PARAM).ToList();
                    CountValue += MIN_PARAM;
                    if ((Tmp[0] == null || string.IsNullOrEmpty(Tmp[0].ToString())) && (!(Tmp.Count > 1) || (Tmp[1] == null || string.IsNullOrEmpty(Tmp[1].ToString()))))
                        continue;
                    Tmp[0] = Tmp[0] ?? "";
                    Tmp[1] = Tmp[1] ?? "";
                    if (!string.IsNullOrEmpty(Tmp[0].ToString()) && Tmp[0].ToString().ToUpper().Equals(Tmp[0].ToString()))
                    {
                        List<string> IcdCodes = Tmp[0].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> IcdNames = new List<string>();
                        if (Tmp.Count > 1)
                            IcdNames = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (IcdCodes != null && IcdCodes.Count > 0 && IcdNames != null && IcdNames.Count > 0)
                        {
                            lstResult.AddRange(GetListIcd(IcdCodes, IcdNames, formatKey, key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME)));
                        }
                        else
                        {
                            foreach (var item in IcdCodes)
                            {
                               lstResult.Add(key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME) ? string.Format(formatKey, item, null) : string.Format(formatKey, null, item));
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Tmp[0].ToString()))
                        {
                            List<string> IcdNames = new List<string>();
                            if (Tmp.Count > 1)
                                IcdNames = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var item in IcdNames)
                            {
                                lstResult.Add(key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME) ? string.Format(formatKey, null, item) : string.Format(formatKey, item, null));
                            }
                        }
                        else
                        {
                            List<string> IcdNames = Tmp[0].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            List<string> IcdCodes = new List<string>();
                            if (Tmp.Count > 1)
                                IcdCodes = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (IcdCodes != null && IcdCodes.Count > 0 && IcdNames != null && IcdNames.Count > 0)
                            {
                                lstResult.AddRange(GetListIcd(IcdCodes, IcdNames, formatKey, key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME)));
                            }
                            else
                            {
                                foreach (var item in IcdNames)
                                {
                                    lstResult.Add(key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME) ? string.Format(formatKey, null, item) : string.Format(formatKey, item, null));
                                }
                            }
                        }
                    }
                }
                result = string.Join("; ", lstResult);
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }

        /// <summary>
        /// formatKey: Định dạng
        /// IsCodeName: Định dạng theo codename hay namecode để gắn lại dữ liệu
        /// </summary>
        private List<string> GetListIcd(List<string> IcdCodes, List<string> IcdNames, string formatKey, bool IsCodeName)
        {
            List<string> lstResult = new List<string>();
            try
            {
                Dictionary<string, List<string>> dicSameText = new Dictionary<string, List<string>>();
                List<string> lstSameText = new List<string>();
                //Xủ lý lấy các chẩn đoán gần giống vào 1 danh sách
                ProcessSameText(IcdNames, ref dicSameText, ref lstSameText);
                //Lọc bỏ các phần tử theo danh sách lấy được ở trên
                IcdNames = IcdNames.Where(o => !lstSameText.Exists(p => p.Equals(o))).ToList();
                int max = IcdCodes.Count > IcdNames.Count ? IcdCodes.Count : IcdNames.Count;
                for (int i = 0; i < max; i++)
                {
                    //Thêm các chẩn đoán gốc
                    string code = i <= IcdNames.Count - 1 ? IcdNames[i] : null;
                    string name = i <= IcdCodes.Count - 1 ? IcdCodes[i] : null;
                    if (IsCodeName)
                    {
                        code = name;
                        name = i <= IcdNames.Count - 1 ? IcdNames[i] : null;
                    }
                    lstResult.Add(string.Format(formatKey, code, name));
                    //Thêm các chẩn đoán gần giống với chẩn đoán gốc ở trên
                    if (dicSameText != null && dicSameText.Count > 0 && dicSameText.ContainsKey(IcdNames[i]))
                    {
                        foreach (var item in dicSameText[IcdNames[i]])
                        {
                            code = item;
                            name = i <= IcdCodes.Count - 1 ? IcdCodes[i] : null;
                            if (IsCodeName)
                            {
                                code = name;
                                name = item;
                            }
                            lstResult.Add(string.Format(formatKey, code, name));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
            return lstResult;
        }

        /// <summary>
        /// Duyệt danh sách tên chẩn đoán, gom các tên chẩn đoán gần giống nhau (key là từ gốc, value là danh sách các từ gần giống)
        /// </summary>
        private void ProcessSameText(List<string> IcdNames, ref Dictionary<string, List<string>> dicSameText, ref List<string> lstSameText)
        {
            try
            {
                for (int i = 0; i < IcdNames.Count; i++)
                {
                    for (int j = i; j < IcdNames.Count; j++)
                    {
                        if (j <= IcdNames.Count - 2)
                        {
                            if (CheckSameText(IcdNames[i], IcdNames[j + 1]))
                            {
                                lstSameText.Add(IcdNames[j + 1]);
                                if (dicSameText.ContainsKey(IcdNames[i]))
                                {
                                    if (!dicSameText[IcdNames[i]].Exists(o => o.Equals(IcdNames[j + 1])))
                                    {
                                        dicSameText[IcdNames[i]].Add(IcdNames[j + 1]);
                                    }
                                }
                                else
                                {
                                    if (!dicSameText.ContainsKey(IcdNames[i]))
                                        dicSameText[IcdNames[i]] = new List<string>();
                                    if (!dicSameText[IcdNames[i]].Exists(o => o.Equals(IcdNames[j + 1])))
                                        dicSameText[IcdNames[i]].Add(IcdNames[j + 1]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Kiểm tra 2 chuỗi ký tự bất kỳ với độ chính xác khoảng 70%
        /// </summary>
        private bool CheckSameText(string txt1, string txt2)
        {
            try
            {
                int ratioSameText = 70;
                string NotSpaceTxt1 = txt1.Replace(" ", "").ToUpper();
                string NotSpaceTxt2 = txt2.Replace(" ", "").ToUpper();
                int count = 0;
                for (int i = 0; i < NotSpaceTxt1.Length; i++)
                {
                    if (i <= NotSpaceTxt2.Length - 1 && NotSpaceTxt1[i].Equals(NotSpaceTxt2[i]))
                        count++;
                    else
                    {
                        var value = (int)((decimal)(count) / (decimal)(NotSpaceTxt2.Length) * 100);
                        return value >= ratioSameText;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                return false;
            }
            return true;
        }
    }
}
