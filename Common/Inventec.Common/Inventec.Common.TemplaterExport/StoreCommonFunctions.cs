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
using Inventec.Common.QRCoder;
using Newtonsoft.Json;
using NGS.Templater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inventec.Common.TemplaterExport
{
    public delegate string EmployeeInfoDelegate(string loginName, string infoKey);

    /// <summary>
    /// Bộ hàm metadata cho template Word, tương ứng bộ user function bên Excel (Inventec.Common.FlexCelExport).
    /// Cú pháp trên biểu mẫu: [[Key]:FuncX-tham_so_1-tham_so_2] hoặc [[Key]:FuncX(tham_so_1,tham_so_2)]
    /// Hàm cần nhiều field (FormatIcd, EmployeeInfo, MergeData) gắn lên tag object và ghi tên field trong tham số.
    /// </summary>
    public partial class Store
    {
        const string FUNC_CONVERT_NUMBER_TO_STRING = "FuncConvertNumberToString";
        const string FUNC_DEC_TO_FRAC = "FuncDecToFrac";
        const string FUNC_CREATE_QR = "FuncCreateQR";
        const string FUNC_CREATE_BARCODE = "FuncCreateBarcode";
        const string FUNC_ROW_NUMBER = "FuncRowNumber";
        const string FUNC_FORMAT_ICD = "FuncFormatIcd";
        const string FUNC_EMPLOYEE_INFO = "FuncEmployeeInfo";
        const string FUNC_MERGE_DATA = "FuncMergeData";

        /// <summary>
        /// Hàm tra thông tin nhân viên theo loginname, do tầng trên (MPS.ProcessorBase) gán — giống FlexCelExport.Store.EmployeeInfoHandler
        /// </summary>
        public static EmployeeInfoDelegate EmployeeInfoHandler { get; set; }

        /// <summary>
        /// Trạng thái giá trị trước đó của từng hàm FuncMergeData{n} (n = 0..19), reset mỗi lần ReadTemplate
        /// </summary>
        private Dictionary<string, string> mergeDataPreviousValues = new Dictionary<string, string>();

        #region Router

        /// <summary>
        /// Các hàm bổ sung để đồng bộ với bộ hàm Excel. Trả về false nếu metadata không khớp hàm nào.
        /// </summary>
        private bool TryCommonFunctionExt(object arg1, string arg2, out object result)
        {
            result = arg1;
            if (arg2 == null) return false;
            if (TryCalcFunction(arg1, arg2, out result)) return true;
            if (arg2.StartsWith(FUNC_CONVERT_NUMBER_TO_STRING)) { result = ConvertNumberToString(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_DEC_TO_FRAC)) { result = DecToFrac(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_CREATE_QR)) { result = CreateQR(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_CREATE_BARCODE)) { result = CreateBarcode(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_ROW_NUMBER)) { result = RowNumber(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_FORMAT_ICD)) { result = FormatIcd(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_EMPLOYEE_INFO)) { result = EmployeeInfo(arg1, arg2); return true; }
            if (arg2.StartsWith(FUNC_MERGE_DATA)) { result = MergeData(arg1, arg2); return true; }
            return false;
        }

        private void ResetCommonFunctionState()
        {
            this.mergeDataPreviousValues = new Dictionary<string, string>();
            ResetCalcState();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Tách tham số sau tên hàm. Hỗ trợ 2 dạng: "FuncX-a-b" và "FuncX(a,b)".
        /// Dạng ngoặc giữ nguyên tham số rỗng và cho phép dấu '-' bên trong tham số.
        /// </summary>
        private static List<string> GetFunctionParameters(string metadata, string funcName)
        {
            List<string> result = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(metadata) || !metadata.StartsWith(funcName)) return result;
                string rest = metadata.Substring(funcName.Length);
                if (rest.StartsWith("("))
                {
                    int end = rest.LastIndexOf(')');
                    rest = end > 0 ? rest.Substring(1, end - 1) : rest.Substring(1);
                    result = rest.Split(new string[] { "," }, StringSplitOptions.None).Select(o => o.Trim()).ToList();
                }
                else
                {
                    result = rest.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToList();
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Đổi object (dòng danh sách) sang dictionary để lấy field theo tên. Trả null nếu argument là giá trị đơn.
        /// </summary>
        private static Dictionary<string, object> ToDictionary(object argument)
        {
            try
            {
                if (argument == null) return null;
                if (argument is Dictionary<string, object>) return (Dictionary<string, object>)argument;
                if (argument is string || argument.GetType().IsPrimitive || argument is decimal) return null;
                var json = JsonConvert.SerializeObject(argument);
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private static object GetFieldValue(Dictionary<string, object> dictionary, string fieldName)
        {
            if (dictionary == null || string.IsNullOrEmpty(fieldName)) return null;
            var key = dictionary.Keys.FirstOrDefault(k => string.Equals(k, fieldName, StringComparison.OrdinalIgnoreCase));
            return key != null ? dictionary[key] : null;
        }

        private static ImageInfo ToImageInfo(System.Drawing.Image image)
        {
            if (image == null) return null;
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            return new ImageInfo(ms, "png", image.Width, image.HorizontalResolution, image.Height, image.VerticalResolution);
        }

        #endregion

        #region Functions

        /// <summary>
        /// [[Key]:FuncConvertNumberToString-numberDigit-convert] — giống FlFuncConvertNumberToString.
        /// numberDigit: số chữ số thập phân (mặc định 0). convert = 1: đổi dấu phẩy/chấm kiểu VN.
        /// </summary>
        static object ConvertNumberToString(object argument, string metadata)
        {
            string result = null;
            try
            {
                if (argument == null) return null;
                decimal value = System.Convert.ToDecimal(argument);
                var parameters = GetFunctionParameters(metadata, FUNC_CONVERT_NUMBER_TO_STRING);
                int numberDigit = 0;
                int convert = 0;
                if (parameters.Count > 0) numberDigit = System.Convert.ToInt32(parameters[0]);
                if (parameters.Count > 1) convert = System.Convert.ToInt32(parameters[1]);
                result = Inventec.Common.Number.Convert.NumberToString(value, numberDigit);
                if (convert == 1)
                {
                    result = result.Replace(",", "_");
                    result = result.Replace(".", ",");
                    result = result.Replace("_", ".");
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
                return argument;
            }
            return result;
        }

        /// <summary>
        /// [[Key]:FuncDecToFrac] — số thập phân về chuỗi phân số, vd 0.125 -> "1/8". Giống FlFuncDecToFrac.
        /// </summary>
        static object DecToFrac(object argument, string metadata)
        {
            try
            {
                if (argument == null) return null;
                return Inventec.Common.Number.Convert.Dec2frac(System.Convert.ToDecimal(argument));
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return argument;
        }

        /// <summary>
        /// [[Key]:FuncCreateQR] hoặc [[Key]:FuncCreateQR-pixelsPerModule] — sinh ảnh QR từ giá trị. Giống FlFuncCreateQR.
        /// </summary>
        static object CreateQR(object argument, string metadata)
        {
            try
            {
                string data = argument != null ? argument.ToString() : null;
                if (string.IsNullOrWhiteSpace(data)) return null;
                int pixelsPerModule = 20;
                var parameters = GetFunctionParameters(metadata, FUNC_CREATE_QR);
                if (parameters.Count > 0) int.TryParse(parameters[0], out pixelsPerModule);
                if (pixelsPerModule <= 0) pixelsPerModule = 20;

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(pixelsPerModule);
                using (var image = System.Drawing.Image.FromStream(new MemoryStream(qrCodeImage)))
                {
                    return ToImageInfo(image);
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return argument;
        }

        /// <summary>
        /// [[Key]:FuncCreateBarcode-width-height-includeLabel] — sinh ảnh barcode CODE128. Giống FlFuncCreateBarcode.
        /// Mặc định 120x40, includeLabel = 0 là có hiện chữ dưới mã (giữ đúng quy ước Excel).
        /// </summary>
        static object CreateBarcode(object argument, string metadata)
        {
            try
            {
                string data = argument != null ? argument.ToString() : null;
                if (string.IsNullOrWhiteSpace(data)) return null;
                var parameters = GetFunctionParameters(metadata, FUNC_CREATE_BARCODE);

                Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(data);
                barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                int width = 120;
                int height = 40;
                if (parameters.Count > 1)
                {
                    int.TryParse(parameters[0], out width);
                    int.TryParse(parameters[1], out height);
                }
                int includeLabel = 0;
                if (parameters.Count > 2)
                {
                    int.TryParse(parameters[2], out includeLabel);
                }
                barcode.IncludeLabel = includeLabel == 0;
                barcode.Width = width > 0 ? width : 120;
                barcode.Height = height > 0 ? height : 40;
                using (var image = barcode.Encode(barcode.EncodedType, data, barcode.Width, barcode.Height))
                {
                    return ToImageInfo(image);
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return argument;
        }

        /// <summary>
        /// [[Key]:FuncRowNumber] — giá trị + 1. Giống FlFuncRowNumber.
        /// </summary>
        static object RowNumber(object argument, string metadata)
        {
            long result = 0;
            try
            {
                if (argument == null) return 0;
                result = System.Convert.ToInt64(argument) + 1;
            }
            catch (Exception ex)
            {
                result = 0;
                Logging.LogSystem.Debug(ex);
            }
            return result;
        }

        /// <summary>
        /// [[Obj]:FuncFormatIcd(code - name,ICD_CODE,ICD_NAME,ICD_SUB_CODE,ICD_TEXT)] — tag gắn lên object, các field đi theo cặp (code, name).
        /// [[ICD_NAME]:FuncFormatIcd(name)] — tag gắn lên 1 giá trị.
        /// Tham số đầu là định dạng chứa "code"/"name". Giống FlFuncFormatIcd (việc 113628).
        /// </summary>
        object FormatIcd(object argument, string metadata)
        {
            try
            {
                var parameters = GetFunctionParameters(metadata, FUNC_FORMAT_ICD);
                string key = parameters.Count > 0 ? parameters[0] : "";
                List<object> args = new List<object>();
                args.Add(key);
                var dictionary = ToDictionary(argument);
                if (dictionary != null)
                {
                    foreach (var fieldName in parameters.Skip(1))
                    {
                        args.Add(GetFieldValue(dictionary, fieldName));
                    }
                }
                else
                {
                    args.Add(argument);
                }
                return FormatIcdCore(args.ToArray());
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return argument;
        }

        /// <summary>
        /// Port nguyên văn từ FlexCelExport.TFlexCelUFFormatIcd.Evaluate
        /// </summary>
        static string FormatIcdCore(object[] parameters)
        {
            const int INDEX_KEY = 0;
            const int INDEX_NEXT_KEY = 1;
            const int MIN_PARAM = 2;
            const string KEY_CODE = "code";
            const string KEY_NAME = "name";
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < MIN_PARAM)
                    throw new ArgumentException("Bad parameters count in call to FuncFormatIcd user-defined function");
                string key = parameters[INDEX_KEY] != null && !string.IsNullOrEmpty(parameters[INDEX_KEY].ToString()) ? parameters[INDEX_KEY].ToString() : string.Format("{0} - {1}", KEY_CODE, KEY_NAME);
                if (!key.Contains(KEY_NAME) && !key.Contains(KEY_CODE))
                    throw new ArgumentException("Bad parameters key in call to FuncFormatIcd user-defined function");
                var args = parameters.Skip(INDEX_NEXT_KEY).Take(Int16.MaxValue).ToList();
                bool isCodeName = key.IndexOf(KEY_CODE) < key.IndexOf(KEY_NAME);
                string formatKey = isCodeName ? key.Replace(KEY_CODE, "{0}").Replace(KEY_NAME, "{1}") : key.Replace(KEY_NAME, "{0}").Replace(KEY_CODE, "{1}");
                List<string> lstResult = new List<string>();
                int CountValue = 0;
                while (args.Count - CountValue > 0)
                {
                    var Tmp = args.Skip(CountValue).Take(MIN_PARAM).ToList();
                    CountValue += MIN_PARAM;
                    if ((Tmp[0] == null || string.IsNullOrEmpty(Tmp[0].ToString())) && (!(Tmp.Count > 1) || (Tmp[1] == null || string.IsNullOrEmpty(Tmp[1].ToString()))))
                        continue;
                    if (Tmp.Count < MIN_PARAM) Tmp.Add("");
                    Tmp[0] = Tmp[0] ?? "";
                    Tmp[1] = Tmp[1] ?? "";
                    if (!string.IsNullOrEmpty(Tmp[0].ToString()) && Tmp[0].ToString().ToUpper().Equals(Tmp[0].ToString()))
                    {
                        List<string> IcdCodes = Tmp[0].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> IcdNames = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (IcdCodes.Count > 0 && IcdNames.Count > 0)
                        {
                            lstResult.AddRange(GetListIcd(IcdCodes, IcdNames, formatKey, isCodeName));
                        }
                        else
                        {
                            foreach (var item in IcdCodes)
                            {
                                lstResult.Add(isCodeName ? string.Format(formatKey, item, null) : string.Format(formatKey, null, item));
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Tmp[0].ToString()))
                        {
                            List<string> IcdNames = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var item in IcdNames)
                            {
                                lstResult.Add(isCodeName ? string.Format(formatKey, null, item) : string.Format(formatKey, item, null));
                            }
                        }
                        else
                        {
                            List<string> IcdNames = Tmp[0].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            List<string> IcdCodes = Tmp[1].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (IcdCodes.Count > 0 && IcdNames.Count > 0)
                            {
                                lstResult.AddRange(GetListIcd(IcdCodes, IcdNames, formatKey, isCodeName));
                            }
                            else
                            {
                                foreach (var item in IcdNames)
                                {
                                    lstResult.Add(isCodeName ? string.Format(formatKey, null, item) : string.Format(formatKey, item, null));
                                }
                            }
                        }
                    }
                }
                result = string.Join("; ", lstResult);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Debug(ex);
            }
            return result;
        }

        static List<string> GetListIcd(List<string> IcdCodes, List<string> IcdNames, string formatKey, bool IsCodeName)
        {
            List<string> lstResult = new List<string>();
            try
            {
                Dictionary<string, List<string>> dicSameText = new Dictionary<string, List<string>>();
                List<string> lstSameText = new List<string>();
                ProcessSameText(IcdNames, ref dicSameText, ref lstSameText);
                IcdNames = IcdNames.Where(o => !lstSameText.Exists(p => p.Equals(o))).ToList();
                int max = IcdCodes.Count > IcdNames.Count ? IcdCodes.Count : IcdNames.Count;
                for (int i = 0; i < max; i++)
                {
                    string code = i <= IcdNames.Count - 1 ? IcdNames[i] : null;
                    string name = i <= IcdCodes.Count - 1 ? IcdCodes[i] : null;
                    if (IsCodeName)
                    {
                        code = name;
                        name = i <= IcdNames.Count - 1 ? IcdNames[i] : null;
                    }
                    lstResult.Add(string.Format(formatKey, code, name));
                    if (dicSameText.Count > 0 && i <= IcdNames.Count - 1 && dicSameText.ContainsKey(IcdNames[i]))
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
                Logging.LogSystem.Debug(ex);
            }
            return lstResult;
        }

        static void ProcessSameText(List<string> IcdNames, ref Dictionary<string, List<string>> dicSameText, ref List<string> lstSameText)
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
                                if (!dicSameText.ContainsKey(IcdNames[i]))
                                    dicSameText[IcdNames[i]] = new List<string>();
                                if (!dicSameText[IcdNames[i]].Exists(o => o.Equals(IcdNames[j + 1])))
                                    dicSameText[IcdNames[i]].Add(IcdNames[j + 1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Kiểm tra 2 chuỗi gần giống nhau với độ chính xác khoảng 70%
        /// </summary>
        static bool CheckSameText(string txt1, string txt2)
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
                Logging.LogSystem.Debug(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// [[LOGINNAME]:FuncEmployeeInfo-TITLE] — tag là loginname, tham số là tên cột V_HIS_EMPLOYEE (mặc định TITLE).
        /// [[Obj]:FuncEmployeeInfo(REQUEST_LOGINNAME,TITLE)] — tag là object, tham số 1 là field chứa loginname.
        /// Giống FlFuncEmployeeInfo; cần tầng trên gán Store.EmployeeInfoHandler.
        /// </summary>
        static object EmployeeInfo(object argument, string metadata)
        {
            try
            {
                if (EmployeeInfoHandler == null || argument == null) return null;
                var parameters = GetFunctionParameters(metadata, FUNC_EMPLOYEE_INFO);
                string loginName = null;
                string infoKey = "TITLE";
                var dictionary = ToDictionary(argument);
                if (dictionary != null)
                {
                    var loginValue = parameters.Count > 0 ? GetFieldValue(dictionary, parameters[0]) : null;
                    loginName = loginValue != null ? loginValue.ToString() : null;
                    if (parameters.Count > 1 && !string.IsNullOrWhiteSpace(parameters[1])) infoKey = parameters[1];
                }
                else
                {
                    loginName = argument.ToString();
                    if (parameters.Count > 0 && !string.IsNullOrWhiteSpace(parameters[0])) infoKey = parameters[0];
                }
                return EmployeeInfoHandler(loginName, infoKey);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// [[Key]:FuncMergeData0] hoặc [[Obj]:FuncMergeData1(FIELD_A,FIELD_B)] — trả về true nếu giá trị trùng với dòng trước (để ẩn giá trị lặp).
        /// Mỗi hậu tố 0..19 giữ trạng thái riêng, giống FlFuncMergeData0..19.
        /// </summary>
        object MergeData(object argument, string metadata)
        {
            try
            {
                if (argument == null) return false;
                string funcName = metadata.Split(new char[] { '-', '(' })[0];
                var parameters = GetFunctionParameters(metadata, funcName);
                string currentValues = string.Empty;
                var dictionary = ToDictionary(argument);
                if (dictionary != null && parameters.Count > 0)
                {
                    List<string> valueList = new List<string>();
                    foreach (var fieldName in parameters)
                    {
                        var value = GetFieldValue(dictionary, fieldName);
                        if (value != null) valueList.Add(value.ToString().Trim());
                    }
                    currentValues = string.Join(";", valueList);
                }
                else
                {
                    currentValues = argument.ToString().Trim();
                }
                if (!string.IsNullOrEmpty(currentValues))
                {
                    string previous = this.mergeDataPreviousValues.ContainsKey(funcName) ? this.mergeDataPreviousValues[funcName] : string.Empty;
                    if (previous == currentValues) return true;
                    this.mergeDataPreviousValues[funcName] = currentValues;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        #endregion
    }
}
