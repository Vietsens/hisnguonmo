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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class ShortcutReplace
    {
        const string CONFIG_KEY__HIS_IS_USE_SHORTCUT_REPLACE_KEY = "CONFIG_KEY__HIS_IS_USE_SHORTCUT_REPLACE_KEY";
        internal const int LIB_TYPE_DEV = 1;
        internal const int LIB_TYPE_TELERIK = 2;
        static bool? isUseShortcutReplaceKeyBase;
        public static bool IsUseShortcutReplaceKeyBase
        {
            get
            {
                try
                {
                    isUseShortcutReplaceKeyBase = (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>(CONFIG_KEY__HIS_IS_USE_SHORTCUT_REPLACE_KEY) == "1");
                    return isUseShortcutReplaceKeyBase.Value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return false;
            }
        }

        public static string ProcesseShortcutReplace(string key, ref string rtfRepValue)
        {
            string replaceKey = "";
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var textLibs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>().Where(o => o.HOT_KEY == key && (o.CREATOR == loginName || o.IS_PUBLIC == 1)).ToList();
                if (textLibs != null && textLibs.Count > 0)
                {
                    replaceKey = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLibs[0].CONTENT);
                    rtfRepValue = HIS.Desktop.Utility.TextLibHelper.BytesToRtfText(textLibs[0].CONTENT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return replaceKey;
        }

        public static string ProcesseShortcutReplace(string key, ref string rtfRepValue, ref string htmlRepValue, int libType)
        {
            string replaceKey = "";
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var textLibs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>().Where(o => o.HOT_KEY == key && (o.CREATOR == loginName || o.IS_PUBLIC == 1)).ToList();
                if (textLibs != null && textLibs.Count > 0)
                {
                    replaceKey = HIS.Desktop.Utility.TextLibHelper.BytesToStringGeneral(textLibs[0].CONTENT, libType);
                    rtfRepValue = HIS.Desktop.Utility.TextLibHelper.BytesToRtfTextGeneral(textLibs[0].CONTENT, libType);
                    htmlRepValue = HIS.Desktop.Utility.TextLibHelper.BytesToHtmlTextGeneral(textLibs[0].CONTENT, libType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return replaceKey;
        }

        /// <summary>
        /// Sửa chức năng: "Xử lý dịch vụ CLS" (HIS.Desktop.Plugins.ServiceExecute)
        ///Chỉ sử dụng thư viện nội dung trong TH:
        ///- Là mẫu do người dùng tạo (CREATOR của HIS_TEXT_LIB)
        ///- Là mẫu do khoa người dùng làm việc tạo và có check "công khai theo khoa" (HIS_TEXT_LIB có IS_PUBLIC_IN_DEPARTMENT = 1 và DEPARTMENT_ID = khoa tương ứng với tài khoản người dùng được thiết lập trong danh mục tài khoản nhân viên)
        ///- Là mẫu được check "công khai toàn viện" (HIS_TEXT_LIB)có IS_PUBLIC = 1)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rtfRepValue"></param>
        /// <param name="htmlRepValue"></param>
        /// <param name="libType"></param>
        /// <returns></returns>
        public static string ProcesseShortcutReplaceWithModuleServiceExecute(string key, ref string rtfRepValue, ref string htmlRepValue, int libType)
        {
            string replaceKey = "";
            try
            {
                //TODO
                long? departmentId = null;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam paramCo = new CommonParam();
                HisEmployeeFilter hisFilter = new HisEmployeeFilter();
                hisFilter.LOGINNAME__EXACT = loginName;
                var employees = new Inventec.Common.Adapter.BackendAdapter
                    (paramCo).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                    ("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisFilter, paramCo);
                if (employees != null && employees.Count > 0)
                {
                    departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                }
                else
                {
                    departmentId = null;
                }
                List<HIS_TEXT_LIB> textLibs = new List<HIS_TEXT_LIB>();
                if (departmentId != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("textLibs 1");
                    textLibs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>().Where(o => o.HOT_KEY == key && (o.CREATOR == loginName || o.IS_PUBLIC == 1 || o.IS_PUBLIC_IN_DEPARTMENT == 1 || o.DEPARTMENT_ID == departmentId)).ToList();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("textLibs 2");
                    textLibs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>().Where(o => o.HOT_KEY == key && (o.CREATOR == loginName || o.IS_PUBLIC == 1 || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == departmentId))).ToList();
                }
                if (textLibs != null && textLibs.Count > 0)
                {
                    replaceKey = HIS.Desktop.Utility.TextLibHelper.BytesToStringGeneral(textLibs[0].CONTENT, libType);
                    rtfRepValue = HIS.Desktop.Utility.TextLibHelper.BytesToRtfTextGeneral(textLibs[0].CONTENT, libType);
                    htmlRepValue = HIS.Desktop.Utility.TextLibHelper.BytesToHtmlTextGeneral(textLibs[0].CONTENT, libType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return replaceKey;
        }

        public static bool ReplaceValue(System.Windows.Forms.Control c)
        {
            bool result = false;
            try
            {
                if (c != null && !String.IsNullOrEmpty(c.Text.Trim()))
                {
                    string keyRaw = "", keyReplace = "";
                    int firstindex = 0;
                    int libType = LIB_TYPE_DEV;

                    DocumentRange searchRange = null;

                    if (c is RichEditControl)
                    {
                        libType = LIB_TYPE_DEV;
                        RichEditControl ct1 = ((RichEditControl)c);

                        firstindex = ct1.Document.CaretPosition.ToInt();
                        if (firstindex == 0)
                        {
                            keyRaw = string.Empty;
                        }
                        else
                        {
                            string previousSymbol = ct1.Document.GetText(ct1.Document.CreateRange(firstindex - 1, 1));
                            Paragraph currentParagraph = ct1.Document.Paragraphs.Get(ct1.Document.CaretPosition);
                            searchRange = ct1.Document.CreateRange(currentParagraph.Range.Start, firstindex - currentParagraph.Range.Start.ToInt());
                            IRegexSearchResult searchResult = ct1.Document.StartSearch(new Regex(@"\w+", System.Text.RegularExpressions.RegexOptions.RightToLeft), searchRange);
                            if (searchResult.FindNext())
                                keyRaw = ct1.Document.GetText(searchResult.CurrentResult);
                        }
                    }
                    else if (c is Telerik.WinControls.UI.RadRichTextEditor)
                    {
                        libType = LIB_TYPE_TELERIK;
                        Telerik.WinControls.UI.RadRichTextEditor ct1 = ((Telerik.WinControls.UI.RadRichTextEditor)c);
                        Telerik.WinForms.Documents.DocumentPosition p1 = new Telerik.WinForms.Documents.DocumentPosition(ct1.Document.CaretPosition, false);
                        p1.MoveToPrevious();
                        keyRaw = p1.GetCurrentWord();
                        if (!String.IsNullOrEmpty(keyRaw))
                        {
                            keyRaw = keyRaw.Trim();
                        }
                    }
                    else if (c is MemoEdit)
                    {
                        libType = LIB_TYPE_DEV;
                        MemoEdit ct1 = ((MemoEdit)c);
                        firstindex = ct1.SelectionStart;
                        var arrSP1 = c.Text.Substring(0, firstindex).Split(new string[] { " ", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrSP1.Length > 1)
                        {
                            keyRaw = arrSP1[arrSP1.Length - 1];
                        }
                        else
                        {
                            keyRaw = c.Text.Substring(0, firstindex).TrimEnd(' ');
                        }
                    }
                    else
                    {
                        libType = LIB_TYPE_DEV;
                        TextBoxMaskBox ct1 = ((TextBoxMaskBox)c);
                        firstindex = ct1.MaskBoxSelectionStart;
                        var arrSP1 = c.Text.Substring(0, firstindex).Split(new string[] { " ", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrSP1.Length > 1)
                        {
                            keyRaw = arrSP1[arrSP1.Length - 1];
                        }
                        else
                        {
                            keyRaw = c.Text.Substring(0, firstindex).TrimEnd(' ');
                        }
                    }
                                       

                    if (!String.IsNullOrEmpty(keyRaw))
                    {
                        string rtfKeyReplace = "", htmlKeyReplace = "";

                        Inventec.Common.Logging.LogSystem.Debug("HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentModuleSelected.ModuleLink=" + HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentModuleSelected != null ? HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentModuleSelected.ModuleLink : "");
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentModuleSelected != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentModuleSelected.ModuleLink == "HIS.Desktop.Plugins.ServiceExecute")
                            keyReplace = ShortcutReplace.ProcesseShortcutReplaceWithModuleServiceExecute(keyRaw, ref rtfKeyReplace, ref htmlKeyReplace, libType);
                        else
                            keyReplace = ShortcutReplace.ProcesseShortcutReplace(keyRaw, ref rtfKeyReplace, ref htmlKeyReplace, libType);
                        if (!String.IsNullOrEmpty(keyReplace))
                        {
                            if (c is TextBoxMaskBox)
                            {
                                c.Text = c.Text.Replace(keyRaw, keyReplace);
                                TextBoxMaskBox ct = ((TextBoxMaskBox)c);
                                ct.MaskBoxSelectionStart = firstindex - keyRaw.Length + keyReplace.Length;
                                ct.MaskBoxSelectionLength = 0;
                            }
                            else if (c is MemoEdit)
                            {
                                c.Text = c.Text.Replace(keyRaw, keyReplace);
                                MemoEdit ct = ((MemoEdit)c);
                                ct.SelectionStart = firstindex - keyRaw.Length + keyReplace.Length;
                                ct.SelectionLength = 0;
                            }
                            else if (c is RichEditControl)
                            {
                                RichEditControl ct = ((RichEditControl)c);

                                Document document = ct.Document;
                                document.ReplaceAll(keyRaw, "", SearchOptions.CaseSensitive);

                                ct.Document.InsertHtmlText(searchRange.End, htmlKeyReplace, InsertOptions.KeepSourceFormatting);

                                DevExpress.XtraRichEdit.API.Native.DocumentPosition position = ct.Document.CreatePosition(firstindex - keyRaw.Length + keyReplace.Length);
                                ct.Document.CaretPosition = position;
                                ct.ScrollToCaret(-200);
                            }
                            else if (c is Telerik.WinControls.UI.RadRichTextEditor)
                            {
                                RadReplaceTextProcess.RadReplaceText((Telerik.WinControls.UI.RadRichTextEditor)c, keyRaw, htmlKeyReplace, keyReplace);
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ProcessCmdKey." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => c.GetType()), c.GetType()));
                            }

                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
