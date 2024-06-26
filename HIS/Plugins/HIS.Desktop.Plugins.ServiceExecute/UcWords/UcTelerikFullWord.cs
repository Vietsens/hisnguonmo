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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinForms.Documents.FormatProviders.Rtf;
using Telerik.WinControls.UI;
using Telerik.WinControls.RichTextEditor.UI;
using Telerik.WinControls;
using Telerik.WinForms.Documents;
using Telerik.WinForms.Documents.TextSearch;
using Telerik.WinForms.Documents.Model;
using Telerik.WinForms.Documents.Layout;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ServiceExecute.UcWords
{
    public partial class UcTelerikFullWord : UserControl
    {
        Action<decimal> ActChangeZoom;

        public UcTelerikFullWord()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
        }

        public UcTelerikFullWord(Action<decimal> actChangeZoom)
            : this()
        {
            this.ActChangeZoom = actChangeZoom;
            this.SetCaptionByLanguageKey();
        }

        public string GetRtfText()
        {
            string result = "";
            try
            {
                RtfFormatProvider provider = new RtfFormatProvider();
                result = provider.Export(radRichTextEditor1.Document);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void SetRtfText(string rtfText)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(rtfText))
                {
                    RtfFormatProvider provider = new RtfFormatProvider();
                    radRichTextEditor1.Document = provider.Import(rtfText);
                }
                else
                {
                    radRichTextEditor1.Document = new Telerik.WinForms.Documents.Model.RadDocument();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radRichTextEditor1_ScaleFactorChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActChangeZoom != null)
                {
                    this.ActChangeZoom((decimal)radRichTextEditor1.ScaleFactor.Height);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void CreateRange(string rangeOld)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(rangeOld))
                {
                    var ps = rangeOld.Split(':');
                    if (ps.Length == 2)
                    {
                        int start = Inventec.Common.TypeConvert.Parse.ToInt32(ps[0]);
                        int length = Inventec.Common.TypeConvert.Parse.ToInt32(ps[1]);
                        if (start > 0 && length > 0)
                        {
                            DocumentPosition po = new DocumentPosition(radRichTextEditor1.Document);
                            po.MoveToDocumentStart();
                            int index = 0;
                            while (index != start)
                            {
                                po.MoveToNext();
                                index++;
                            }

                            var start0 = new TextRange(new DocumentPosition(po, true), new DocumentPosition(po, true));
                            while (index != start + length)
                            {
                                po.MoveToNext();
                                index++;
                            }

                            var end0 = new TextRange(new DocumentPosition(po, true), new DocumentPosition(po, true));

                            BookmarkRangeEnd bmRangeEnd = new BookmarkRangeEnd();
                            BookmarkRangeStart bmRangeStart = (BookmarkRangeStart)bmRangeEnd.CreatePairedStart();
                            bmRangeStart.Name = ServiceExecuteCFG.keyXml;

                            radRichTextEditor1.Document.Selection.AddSelectionStart(start0.StartPosition);
                            radRichTextEditor1.Document.Selection.AddSelectionEnd(start0.EndPosition);
                            radRichTextEditor1.InsertInline(bmRangeStart);

                            radRichTextEditor1.Document.Selection.AddSelectionStart(end0.StartPosition);
                            radRichTextEditor1.Document.Selection.AddSelectionEnd(end0.EndPosition);
                            radRichTextEditor1.InsertInline(bmRangeEnd);
                        }
                    }
                }
                else
                {
                    Telerik.WinForms.Documents.TextSearch.DocumentTextSearch search = new Telerik.WinForms.Documents.TextSearch.DocumentTextSearch(radRichTextEditor1.Document);
                    var rangesB = search.FindAll(ServiceExecuteCFG.keyXml);
                    if (rangesB != null && rangesB.Count() > 1)
                    {
                        //từ vị trí 0 đến vị trí key begin đầu tiên sẽ cho phép nhập.
                        var start0 = rangesB.First();
                        var end0 = rangesB.Last();

                        BookmarkRangeEnd bmRangeEnd = new BookmarkRangeEnd();
                        BookmarkRangeStart bmRangeStart = (BookmarkRangeStart)bmRangeEnd.CreatePairedStart();
                        bmRangeStart.Name = ServiceExecuteCFG.keyXml;

                        foreach (var item in rangesB)
                        {
                            radRichTextEditor1.Document.Selection.AddSelectionStart(item.StartPosition);
                            radRichTextEditor1.Document.Selection.AddSelectionEnd(item.EndPosition);
                            if (item == start0)
                            {
                                radRichTextEditor1.InsertInline(bmRangeStart);
                            }
                            else if (item == end0)
                            {
                                radRichTextEditor1.InsertInline(bmRangeEnd);
                            }
                            else
                            {
                                radRichTextEditor1.Insert(" ");
                            }
                        }
                    }
                    else //không có key mà có vùng đánh dấu thì xóa
                    {
                        var bookmark = this.radRichTextEditor1.Document.GetBookmarkByName(ServiceExecuteCFG.keyXml);
                        if (bookmark != null)
                        {
                            this.radRichTextEditor1.DeleteBookmark(bookmark);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal string GetRange()
        {
            string result = "";
            try
            {
                var bookmark = this.radRichTextEditor1.Document.GetBookmarkByName(ServiceExecuteCFG.keyXml);
                if (bookmark != null)
                {
                    var startPosition = new DocumentPosition(radRichTextEditor1.Document);
                    var endPosition = new DocumentPosition(radRichTextEditor1.Document);

                    radRichTextEditor1.Document.GoToBookmark(bookmark);
                    startPosition.MoveToInline(bookmark.FirstLayoutBox as InlineLayoutBox, 0);
                    endPosition.MoveToInline(bookmark.End.FirstLayoutBox as InlineLayoutBox, 0);

                    int start = 0;
                    DocumentPosition po = new DocumentPosition(radRichTextEditor1.Document);
                    po.MoveToDocumentStart();
                    while (po != startPosition)
                    {
                        po.MoveToNext();
                        start++;
                    }

                    int length = 0;
                    while (po != endPosition && !po.IsPositionAtDocumentEnd)
                    {
                        po.MoveToNext();
                        length++;
                    }

                    result = string.Format("{0}:{1}", start, length);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal string GetDataRange()
        {
            string result = null;
            try
            {
                var bookmark = this.radRichTextEditor1.Document.GetBookmarkByName(ServiceExecuteCFG.keyXml);
                if (bookmark != null)
                {
                    List<string> bookmarksContent = new List<string>();

                    var startPosition = new DocumentPosition(radRichTextEditor1.Document);
                    var endPosition = new DocumentPosition(radRichTextEditor1.Document);

                    radRichTextEditor1.Document.GoToBookmark(bookmark);
                    startPosition.MoveToInline(bookmark.FirstLayoutBox as InlineLayoutBox, 0);
                    endPosition.MoveToInline(bookmark.End.FirstLayoutBox as InlineLayoutBox, 0);

                    while (startPosition != endPosition)
                    {
                        InlineLayoutBox data = startPosition.GetCurrentInlineBox();
                        bookmarksContent.Add(data.Text);
                        startPosition.MoveToNextInlineBox();
                    }

                    result = string.Join("", bookmarksContent);
                }
                else
                {
                    result = this.GetRtfText();
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UcTelerikFullWord
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(UcTelerikFullWord).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.richTextEditorRibbonBar1.ExitButton.Text = Inventec.Common.Resource.Get.Value("UcTelerikFullWord.richTextEditorRibbonBar1.ExitButton.Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                this.richTextEditorRibbonBar1.OptionsButton.Text = Inventec.Common.Resource.Get.Value("UcTelerikFullWord.richTextEditorRibbonBar1.OptionsButton.Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                this.richTextEditorRibbonBar1.Text = Inventec.Common.Resource.Get.Value("UcTelerikFullWord.richTextEditorRibbonBar1.Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                ((Telerik.WinControls.UI.RadRibbonBarElement)(this.richTextEditorRibbonBar1.GetChildAt(0))).Text = Inventec.Common.Resource.Get.Value("((Telerik.WinControls.UI.RadRibbonBarElement)(UcTelerikFullWord.richTextEditorRibbonBar1.GetChildAt(0))).Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(5))).Text = Inventec.Common.Resource.Get.Value("((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(UcTelerikFullWord.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(5))).Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(6))).Text = Inventec.Common.Resource.Get.Value("((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(UcTelerikFullWord.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(6))).Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
                ((Telerik.WinControls.UI.RadApplicationMenuButtonElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(5))).Text = Inventec.Common.Resource.Get.Value("((Telerik.WinControls.UI.RadApplicationMenuButtonElement)(UcTelerikFullWord.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(5))).Text", Resources.ResourceLanguageManager.LanguageResourceUcTelerikFullWord, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
