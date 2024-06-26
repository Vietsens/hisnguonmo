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
using HIS.Desktop.Plugins.CompareBhytInfo.XMLCheck.XMLADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace HIS.Desktop.Plugins.CompareBhytInfo.XMLCheck
{
    class CheckProcessor
    {
        XML1DataADO DataXML1 = new XML1DataADO();
        List<XML2DetailADO> ListDataXML2 = new List<XML2DetailADO>();
        List<XML3DetailADO> ListDataXML3 = new List<XML3DetailADO>();
        NumberStyles style = NumberStyles.Any;

        string FileName = "";

        public CheckProcessor(string directoryFile)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(directoryFile))
                {
                    FileName = directoryFile;
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(directoryFile);

                    DecodeAndReplaceWithNode(xmldoc);

                    ProcessDataXML1(xmldoc);
                    ProcessDataDetail(xmldoc);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DecodeAndReplaceWithNode(XmlDocument xmldoc)
        {
            try
            {
                if (xmldoc != null)
                {
                    XmlNodeList nodeList = xmldoc.GetElementsByTagName("NOIDUNGFILE");
                    string InnertText = string.Empty;

                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        InnertText = nodeList[i].InnerText;
                        string DeCodeValue = Base64Decode(InnertText);
                        string outPut = RemoveFistLine(DeCodeValue);
                        ReplaceNodeByNode(xmldoc, nodeList[i], outPut);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string Base64Decode(string base64EncodedData)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(base64EncodedData))
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                    result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string RemoveFistLine(string input)
        {
            string output = "";
            try
            {
                if (String.IsNullOrWhiteSpace(input) || input.Length == 0)
                {
                    return output;
                }

                int n = 1;
                string[] lines = input.Split(Environment.NewLine.ToCharArray()).Skip(n).ToArray();

                output = string.Join(Environment.NewLine, lines);
                output = RemoveEmptyLines(output);
            }
            catch (Exception ex)
            {
                output = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return output;
        }

        private string RemoveEmptyLines(string lines)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(lines) || lines.Length == 0)
                {
                    return "";
                }

                return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return lines;
            }
        }

        private void ReplaceNodeByNode(XmlDocument doc, XmlNode oldElem, string newValue)
        {
            try
            {
                if (doc == null || oldElem == null)
                {
                    return;
                }

                var root = doc.GetElementsByTagName("GIAMDINHHS")[0];
                if (root == null)
                {
                    return;
                }

                var newElem = doc.CreateElement(oldElem.Name);
                if (newElem == null)
                {
                    return;
                }

                //XDocument.Parse(newValue);
                XmlDocument XmlDocumentNew = new XmlDocument();
                XmlDocumentNew.LoadXml(newValue);
                if (oldElem.ParentNode != null)
                {
                    oldElem.ParentNode.ReplaceChild(newElem, oldElem);
                    newElem.AppendChild(newElem.OwnerDocument.ImportNode(XmlDocumentNew.FirstChild, true));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataXML1(XmlDocument xmldoc)
        {
            try
            {
                XmlNodeList xnList = xmldoc.SelectNodes("//TONG_HOP");
                if (xnList != null && xnList.Count > 0)
                {
                    foreach (XmlNode item in xnList)
                    {
                        DataXML1.MA_LK = item["MA_LK"] != null ? item["MA_LK"].InnerText : "";
                        DataXML1.MA_BN = item["MA_BN"] != null ? item["MA_BN"].InnerText : "";
                        DataXML1.HO_TEN = item["HO_TEN"] != null ? item["HO_TEN"].InnerText : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataDetail(XmlDocument xmldoc)
        {
            try
            {
                FillDataToXML2(xmldoc);
                FillDataToXML3(xmldoc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToXML2(XmlDocument xml)
        {
            try
            {
                ListDataXML2 = new List<XML2DetailADO>();
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_THUOC/CHI_TIET_THUOC");
                if (xnList == null) return;

                foreach (XmlNode xn in xnList)
                {
                    Decimal soluong, tBhtt, tBncct, tBntt, tNgoaiDs, tNguonKhac, DonGia, ThanhTien;
                    XML2DetailADO xML2DetailData = new XML2DetailADO();

                    xML2DetailData.DON_VI_TINH = xn["DON_VI_TINH"] != null ? xn["DON_VI_TINH"].InnerText : "";
                    xML2DetailData.DUONG_DUNG = xn["DUONG_DUNG"] != null ? xn["DUONG_DUNG"].InnerText : "";
                    xML2DetailData.HAM_LUONG = xn["HAM_LUONG"] != null ? xn["HAM_LUONG"].InnerText : "";
                    xML2DetailData.LIEU_DUNG = xn["LIEU_DUNG"] != null ? xn["LIEU_DUNG"].InnerText : "";
                    xML2DetailData.MA_BAC_SI = xn["MA_BAC_SI"] != null ? xn["MA_BAC_SI"].InnerText : "";
                    xML2DetailData.MA_BENH = xn["MA_BENH"] != null ? xn["MA_BENH"].InnerText : "";
                    xML2DetailData.MA_KHOA = xn["MA_KHOA"] != null ? xn["MA_KHOA"].InnerText : "";
                    xML2DetailData.MA_LK = xn["MA_LK"] != null ? xn["MA_LK"].InnerText : "";
                    xML2DetailData.MA_NHOM = xn["MA_NHOM"] != null ? xn["MA_NHOM"].InnerText : "";
                    xML2DetailData.MA_PTTT = xn["MA_PTTT"] != null ? xn["MA_PTTT"].InnerText : "";
                    xML2DetailData.MA_THUOC = xn["MA_THUOC"] != null ? xn["MA_THUOC"].InnerText : "";
                    xML2DetailData.MUC_HUONG = xn["MUC_HUONG"] != null ? xn["MUC_HUONG"].InnerText : "";
                    xML2DetailData.NGAY_YL = xn["NGAY_YL"] != null ? xn["NGAY_YL"].InnerText : "";
                    xML2DetailData.PHAM_VI = xn["PHAM_VI"] != null ? xn["PHAM_VI"].InnerText : "";
                    xML2DetailData.SO_DANG_KY = xn["SO_DANG_KY"] != null ? xn["SO_DANG_KY"].InnerText : "";

                    if (xn["DON_GIA"] != null && Decimal.TryParse((xn["DON_GIA"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out DonGia))
                        xML2DetailData.DON_GIA = DonGia;

                    if (xn["SO_LUONG"] != null && Decimal.TryParse((xn["SO_LUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out soluong))
                        xML2DetailData.SO_LUONG = soluong;

                    xML2DetailData.STT = xn["STT"] != null ? xn["STT"].InnerText : "";

                    if (xn["T_BHTT"] != null && Decimal.TryParse((xn["T_BHTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBhtt))
                        xML2DetailData.T_BHTT = tBhtt;

                    if (xn["T_BNCCT"] != null && Decimal.TryParse((xn["T_BNCCT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBncct))
                        xML2DetailData.T_BNCCT = tBncct;

                    if (xn["T_BNTT"] != null && Decimal.TryParse((xn["T_BNTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBntt))
                        xML2DetailData.T_BNTT = tBntt;

                    if (xn["T_NGOAIDS"] != null && Decimal.TryParse((xn["T_NGOAIDS"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNgoaiDs))
                        xML2DetailData.T_NGOAIDS = tNgoaiDs;

                    if (xn["T_NGUONKHAC"] != null && Decimal.TryParse((xn["T_NGUONKHAC"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNguonKhac))
                        xML2DetailData.T_NGUONKHAC = tNguonKhac;

                    if (xn["THANH_TIEN"] != null && Decimal.TryParse((xn["THANH_TIEN"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out ThanhTien))
                        xML2DetailData.THANH_TIEN = ThanhTien;

                    xML2DetailData.TEN_THUOC = xn["TEN_THUOC"] != null ? xn["TEN_THUOC"].InnerText : "";
                    xML2DetailData.TT_THAU = xn["TT_THAU"] != null ? xn["TT_THAU"].InnerText : "";
                    xML2DetailData.TYLE_TT = xn["TYLE_TT"] != null ? xn["TYLE_TT"].InnerText : "";

                    ListDataXML2.Add(xML2DetailData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToXML3(XmlDocument xml)
        {
            try
            {
                ListDataXML3 = new List<XML3DetailADO>();
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_DVKT/CHI_TIET_DVKT");
                if (xnList == null) return;

                foreach (XmlNode xn in xnList)
                {
                    Decimal soluong, tBhtt, tBncct, tBntt, tNgoaiDs, tNguonKhac, DonGia, ThanhTien, tTranTT, mucHuong;

                    XML3DetailADO xML3DetailData = new XML3DetailADO();
                    xML3DetailData.DON_VI_TINH = xn["DON_VI_TINH"] != null ? xn["DON_VI_TINH"].InnerText : "";
                    xML3DetailData.GOI_VTYT = xn["GOI_VTYT"] != null ? xn["GOI_VTYT"].InnerText : "";
                    xML3DetailData.MA_BAC_SI = xn["MA_BAC_SI"] != null ? xn["MA_BAC_SI"].InnerText : "";
                    xML3DetailData.MA_BENH = xn["MA_BENH"] != null ? xn["MA_BENH"].InnerText : "";
                    xML3DetailData.MA_DICH_VU = xn["MA_DICH_VU"] != null ? xn["MA_DICH_VU"].InnerText : "";
                    xML3DetailData.MA_GIUONG = xn["MA_GIUONG"] != null ? xn["MA_GIUONG"].InnerText : "";
                    xML3DetailData.MA_KHOA = xn["MA_KHOA"] != null ? xn["MA_KHOA"].InnerText : "";
                    xML3DetailData.MA_LK = xn["MA_LK"] != null ? xn["MA_LK"].InnerText : "";
                    xML3DetailData.MA_NHOM = xn["MA_NHOM"] != null ? xn["MA_NHOM"].InnerText : "";
                    xML3DetailData.MA_PTTT = xn["MA_PTTT"] != null ? xn["MA_PTTT"].InnerText : "";
                    xML3DetailData.MA_VAT_TU = xn["MA_VAT_TU"] != null ? xn["MA_VAT_TU"].InnerText : "";
                    xML3DetailData.NGAY_KQ = xn["NGAY_KQ"] != null ? xn["NGAY_KQ"].InnerText : "";
                    xML3DetailData.NGAY_YL = xn["NGAY_YL"] != null ? xn["NGAY_YL"].InnerText : "";
                    xML3DetailData.PHAM_VI = xn["PHAM_VI"] != null ? xn["PHAM_VI"].InnerText : "";

                    if (xn["MUC_HUONG"] != null && Decimal.TryParse((xn["MUC_HUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out mucHuong))
                        xML3DetailData.MUC_HUONG = mucHuong;

                    if (xn["DON_GIA"] != null && Decimal.TryParse((xn["DON_GIA"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out DonGia))
                        xML3DetailData.DON_GIA = DonGia;

                    if (xn["SO_LUONG"] != null && Decimal.TryParse((xn["SO_LUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out soluong))
                        xML3DetailData.SO_LUONG = soluong;

                    xML3DetailData.STT = xn["STT"].InnerText;

                    if (xn["T_BHTT"] != null && Decimal.TryParse((xn["T_BHTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBhtt))
                        xML3DetailData.T_BHTT = tBhtt;

                    if (xn["T_BNCCT"] != null && Decimal.TryParse((xn["T_BNCCT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBncct))
                        xML3DetailData.T_BNCCT = tBncct;

                    if (xn["T_BNTT"] != null && Decimal.TryParse((xn["T_BNTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBntt))
                        xML3DetailData.T_BNTT = tBntt;

                    if (xn["T_NGOAIDS"] != null && Decimal.TryParse((xn["T_NGOAIDS"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNgoaiDs))
                        xML3DetailData.T_NGOAIDS = tNgoaiDs;

                    if (xn["T_NGUONKHAC"] != null && Decimal.TryParse((xn["T_NGUONKHAC"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNguonKhac))
                        xML3DetailData.T_NGUONKHAC = tNguonKhac;

                    if (xn["THANH_TIEN"] != null && Decimal.TryParse((xn["THANH_TIEN"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out ThanhTien))
                        xML3DetailData.THANH_TIEN = ThanhTien;

                    if (xn["T_TRANTT"] != null && Decimal.TryParse((xn["T_TRANTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tTranTT))
                        xML3DetailData.T_TRANTT = tTranTT;

                    xML3DetailData.STT = xn["STT"] != null ? xn["STT"].InnerText : "";

                    xML3DetailData.TEN_DICH_VU = xn["TEN_DICH_VU"] != null ? xn["TEN_DICH_VU"].InnerText : "";
                    xML3DetailData.TEN_VAT_TU = xn["TEN_VAT_TU"] != null ? xn["TEN_VAT_TU"].InnerText : "";
                    xML3DetailData.TT_THAU = xn["TT_THAU"] != null ? xn["TT_THAU"].InnerText : "";
                    xML3DetailData.TYLE_TT = xn["TYLE_TT"] != null ? xn["TYLE_TT"].InnerText : "";
                    ListDataXML3.Add(xML3DetailData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CheckError(List<ADO.BhytMedicineADO> ListThuocADO, List<ADO.BhytMaterialADO> ListVatTuADO, List<ADO.BhytServiceADO> ListDvktADO, ref ADO.XmlFileInfoADO result)
        {
            try
            {
                if (result == null)
                {
                    result = new ADO.XmlFileInfoADO();
                }

                if (DataXML1 != null)
                {
                    result.HO_TEN = DataXML1.HO_TEN;
                    result.MA_BN = DataXML1.MA_BN;
                    result.MA_LK = DataXML1.MA_LK;
                    result.FileName = FileName;
                }

                if (DataXML1 != null || ListDataXML2 != null || ListDataXML3 != null)
                {
                    List<string> error = new List<string>();

                    #region XML 2
                    if (ListDataXML2 != null && ListDataXML2.Count > 0 && ListThuocADO != null && ListThuocADO.Count > 0)
                    {
                        List<string> notDM = new List<string>();
                        foreach (var item in ListDataXML2)
                        {
                            //máu sẽ có HST_BHYT_CODE khác với thuốc.
                            //import sẽ để mã máu chung cột với mã thuốc.
                            var listThuoc = ListThuocADO.Where(o => o.ACTIVE_INGR_BHYT_CODE.Trim().ToLower() == item.MA_THUOC.ToLower() && o.HST_BHYT_CODE.Trim().ToLower() == item.MA_NHOM.ToLower()).ToList();
                            if (listThuoc != null && listThuoc.Count > 0)
                            {
                                bool valid = false;
                                foreach (var thuoc in listThuoc)
                                {
                                    List<string> ttThau = new List<string>();
                                    if (!string.IsNullOrEmpty(thuoc.BID_NUMBER)) ttThau.Add(thuoc.BID_NUMBER.Trim());

                                    if (!string.IsNullOrEmpty(thuoc.BID_PACKAGE_CODE)) ttThau.Add(thuoc.BID_PACKAGE_CODE.Trim());

                                    if (!string.IsNullOrEmpty(thuoc.BID_GROUP_CODE)) ttThau.Add(thuoc.BID_GROUP_CODE.Trim());

                                    if (String.Join(";", ttThau).ToLower() == item.TT_THAU.ToLower() && thuoc.HEIN_SERVICE_TYPE_NAME.Trim().ToLower() == item.TEN_THUOC.ToLower() &&
                                    thuoc.CONCENTRA.Trim().ToLower() == item.HAM_LUONG.ToLower() && thuoc.REGISTER_NUMBER.Trim().ToLower() == item.SO_DANG_KY.ToLower() &&
                                    thuoc.SERVICE_UNIT_NAME.Trim().ToLower() == item.DON_VI_TINH.ToLower() &&
                                    Math.Round(thuoc.PRICE, 3, MidpointRounding.AwayFromZero) == Math.Round(item.DON_GIA, 3, MidpointRounding.AwayFromZero))
                                    {
                                        valid = true;
                                        break;
                                    }
                                }

                                if (!valid)
                                {
                                    notDM.Add(item.MA_THUOC);
                                }
                            }
                            else
                            {
                                notDM.Add(item.MA_THUOC);
                            }
                        }

                        if (notDM.Count > 0)
                        {
                            error.Add(string.Format("Chi tiết thuốc {0} không có trong danh mục", string.Join(",", notDM.Distinct().ToList())));
                        }
                    }
                    #endregion

                    #region XML 3
                    if (ListDataXML3 != null && ListDataXML3.Count > 0)
                    {
                        var xmlVatTu = ListDataXML3.Where(o => !String.IsNullOrWhiteSpace(o.MA_VAT_TU)).ToList();
                        var xmlDichVu = ListDataXML3.Where(o => String.IsNullOrWhiteSpace(o.MA_VAT_TU)).ToList();

                        #region Vat Tu
                        if (xmlVatTu != null && xmlVatTu.Count > 0 && ListVatTuADO != null && ListVatTuADO.Count > 0)
                        {
                            List<string> notDM = new List<string>();
                            foreach (var item in xmlVatTu)
                            {
                                var listVatTu = ListVatTuADO.Where(o => o.HEIN_SERVICE_TYPE_CODE.Trim().ToLower() == item.MA_VAT_TU.ToLower()).ToList();
                                if (listVatTu != null && listVatTu.Count > 0)
                                {
                                    bool valid = false;
                                    foreach (var vatTu in listVatTu)
                                    {
                                        List<string> ttThauVt = new List<string>();
                                        ttThauVt.Add(vatTu.BID_YEAR.Trim());
                                        if (!string.IsNullOrEmpty(vatTu.BID_PACKAGE_CODE)) ttThauVt.Add(vatTu.BID_PACKAGE_CODE.Trim());
                                        else ttThauVt.Add("01");

                                        ttThauVt.Add(vatTu.BID_NUMBER.Trim());

                                        if (String.Join(".", ttThauVt).ToLower() == item.TT_THAU.ToLower() && vatTu.HEIN_SERVICE_TYPE_NAME.Trim().ToLower() == item.TEN_VAT_TU.ToLower() &&
                                            vatTu.SERVICE_UNIT_NAME.Trim().ToLower() == item.DON_VI_TINH.ToLower() && vatTu.HST_BHYT_CODE.Trim().ToLower() == item.MA_NHOM.ToLower() &&
                                            Math.Round(vatTu.PRICE, 3, MidpointRounding.AwayFromZero) == Math.Round(item.DON_GIA, 3, MidpointRounding.AwayFromZero))
                                        {
                                            valid = true;
                                            break;
                                        }
                                    }

                                    if (!valid)
                                    {
                                        notDM.Add(item.MA_VAT_TU);
                                    }
                                }
                                else
                                {
                                    notDM.Add(item.MA_VAT_TU);
                                }
                            }

                            if (notDM.Count > 0)
                            {
                                error.Add(string.Format("Chi tiết vật tư {0} không có trong danh mục", string.Join(",", notDM.Distinct().ToList())));
                            }
                        }
                        #endregion

                        #region Dich Vu
                        if (xmlDichVu != null && xmlDichVu.Count > 0 && ListDvktADO != null && ListDvktADO.Count > 0)
                        {
                            List<string> notDM = new List<string>();
                            foreach (var item in xmlDichVu)
                            {
                                var listDvkt = ListDvktADO.Where(o => o.HEIN_SERVICE_TYPE_CODE.Trim().ToLower() == item.MA_DICH_VU.ToLower()).ToList();
                                if (listDvkt != null && listDvkt.Count > 0)
                                {
                                    bool valid = false;
                                    foreach (var dvkt in listDvkt)
                                    {
                                        if (dvkt.HEIN_SERVICE_TYPE_NAME.Trim().ToLower() == item.TEN_DICH_VU.ToLower() &&
                                            dvkt.SERVICE_UNIT_NAME.Trim().ToLower() == item.DON_VI_TINH.ToLower() && dvkt.HST_BHYT_CODE.Trim().ToLower() == item.MA_NHOM.ToLower() &&
                                            Math.Round(dvkt.PRICE, 3, MidpointRounding.AwayFromZero) == Math.Round(item.DON_GIA, 3, MidpointRounding.AwayFromZero))
                                        {
                                            valid = true;
                                            break;
                                        }
                                    }

                                    if (!valid)
                                    {
                                        notDM.Add(item.MA_DICH_VU);
                                    }
                                }
                                else
                                {
                                    notDM.Add(item.MA_DICH_VU);
                                }
                            }

                            if (notDM.Count > 0)
                            {
                                error.Add(string.Format("Chi tiết dịch vụ {0} không có trong danh mục", string.Join(",", notDM.Distinct().ToList())));
                            }
                        }
                        #endregion
                    }
                    #endregion

                    if (error.Count > 0)
                    {
                        result.ERROR = string.Join(",", error);
                    }
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
