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
using Telerik.WinForms.Documents;
using Telerik.WinForms.Documents.Model;
using Telerik.WinForms.Documents.TextSearch;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    class WordProcess
    {
        public const string keyFull = "<#DocProtected>";
        public const string keyBegin = "<#DocProtectedB>";
        public const string keyEnd = "<#DocProtectedE>";

        internal static long zoomFactor(System.Windows.Forms.Control edit)
        {
            if (edit is DevExpress.XtraRichEdit.RichEditControl)
            {
                return zoomFactor((DevExpress.XtraRichEdit.RichEditControl)edit);
            }
            else if (edit is Telerik.WinControls.UI.RadRichTextEditor)
            {
                return zoomFactor((Telerik.WinControls.UI.RadRichTextEditor)edit);
            }
            else return 100;
        }

        internal static long zoomFactor(DevExpress.XtraRichEdit.RichEditControl txtDescription)
        {
            long result = 100;
            try
            {
                if (txtDescription != null)
                {
                    float zoom = 0;
                    if (txtDescription.Document.Sections[0].Page.Landscape)
                        //zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Height / 3);
                        zoom = (float)(txtDescription.Width) / (txtDescription.Document.Sections[0].Page.Height / 3);
                    else
                        //zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Width / 3);
                        zoom = (float)(txtDescription.Width) / (txtDescription.Document.Sections[0].Page.Width / 3);
                    result = (long)(zoom * 100);
                }
            }
            catch (Exception ex)
            {
                result = 100;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static long zoomFactor(Telerik.WinControls.UI.RadRichTextEditor txtDescription)
        {
            long result = 100;
            try
            {
                if (txtDescription != null)
                {
                    float zoom = 0;
                    if (txtDescription.Document.SectionDefaultPageOrientation == PageOrientation.Landscape)
                        zoom = (float)(txtDescription.Width) / (float)(txtDescription.Document.Sections.First.PageSize.Height);
                    else
                        zoom = (float)(txtDescription.Width) / (float)(txtDescription.Document.Sections.First.PageSize.Width);
                    result = (long)(zoom * 100);
                }
            }
            catch (Exception ex)
            {
                result = 100;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static void InitialProtected(Telerik.WinControls.UI.RadRichTextEditor RichTextEditor, ref string positionProtect)
        {
            try
            {
                Telerik.WinForms.Documents.TextSearch.DocumentTextSearch search = new Telerik.WinForms.Documents.TextSearch.DocumentTextSearch(RichTextEditor.Document);

                var rangesB = search.FindAll(keyBegin);
                var rangesE = search.FindAll(keyEnd);
                if (rangesB != null && rangesB.Count() > 0 && rangesE != null && rangesE.Count() > 0)
                {
                    List<string> position = new List<string>();
                    List<TextRangeProtected> listPt = new List<TextRangeProtected>();

                    foreach (var itemB in rangesB)
                    {
                        var nextB = rangesB.FirstOrDefault(o => o.StartPosition > itemB.StartPosition);
                        TextRange endRange = null;
                        //lấy end cuối cùng 
                        if (nextB != null)//trước begin tiếp theo
                        {
                            endRange = rangesE.LastOrDefault(o => o.EndPosition < nextB.StartPosition);
                        }
                        else
                        {
                            endRange = rangesE.LastOrDefault(o => o.StartPosition > itemB.EndPosition);
                        }

                        if (endRange != null && itemB.StartPosition < endRange.StartPosition)
                        {
                            int start = 0;
                            DocumentPosition po = new DocumentPosition(RichTextEditor.Document);
                            po.MoveToDocumentStart();
                            while (po != itemB.StartPosition)
                            {
                                po.MoveToNext();
                                start++;
                            }

                            int length = 0;
                            while (po != endRange.StartPosition && !po.IsPositionAtDocumentEnd)
                            {
                                po.MoveToNext();
                                length++;
                            }

                            length -= keyBegin.Length;
                            //lấy vị trí trước và thêm vùng ReadOnly sau vì khi thêm luôn làm thay đổi đối tượng trong rangesB dẫn đến sai thuật toán
                            TextRangeProtected r = new TextRangeProtected();
                            r.StartRange = new TextRange(new DocumentPosition(itemB.StartPosition, true), new DocumentPosition(itemB.EndPosition, true));
                            r.EndRange = new TextRange(new DocumentPosition(endRange.StartPosition, true), new DocumentPosition(endRange.EndPosition, true));
                            listPt.Add(r);

                            position.Add(string.Format("{0}:{1}", start, length));
                        }
                    }

                    if (listPt != null && listPt.Count > 0)
                    {
                        ReadOnlyRangeStart rangeStart = new ReadOnlyRangeStart();
                        ReadOnlyRangeEnd rangeEnd = new ReadOnlyRangeEnd();
                        rangeEnd.PairWithStart(rangeStart);
                        foreach (var item in listPt)
                        {
                            RichTextEditor.Document.Selection.AddSelectionStart(item.StartRange.StartPosition);
                            RichTextEditor.Document.Selection.AddSelectionEnd(item.StartRange.EndPosition);
                            RichTextEditor.InsertInline(rangeStart);
                            item.StartRange.StartPosition.Dispose();
                            item.StartRange.EndPosition.Dispose();

                            RichTextEditor.Document.Selection.AddSelectionStart(item.EndRange.StartPosition);
                            RichTextEditor.Document.Selection.AddSelectionEnd(item.EndRange.EndPosition);
                            RichTextEditor.InsertInline(rangeEnd);
                            item.EndRange.StartPosition.Dispose();
                            item.EndRange.EndPosition.Dispose();
                        }
                    }

                    if (position != null && position.Count > 0)
                    {
                        positionProtect = string.Join("|", position);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(positionProtect))
                    {
                        var posi = positionProtect.Split('|');
                        ReadOnlyRangeStart rangeStart = new ReadOnlyRangeStart();
                        ReadOnlyRangeEnd rangeEnd = new ReadOnlyRangeEnd();
                        rangeEnd.PairWithStart(rangeStart);
                        foreach (var item in posi)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var ps = item.Split(':');
                                if (ps.Length == 2)
                                {
                                    int start = Inventec.Common.TypeConvert.Parse.ToInt32(ps[0]);
                                    int length = Inventec.Common.TypeConvert.Parse.ToInt32(ps[1]);

                                    //vị trí được phép nhập dữ liệu
                                    if (start > 0 && length > 0)
                                    {
                                        DocumentPosition po = new DocumentPosition(RichTextEditor.Document);
                                        po.MoveToDocumentStart();
                                        int index = 0;
                                        while (index != start)
                                        {
                                            po.MoveToNext();
                                            index++;
                                        }

                                        TextRange startRange = new TextRange(new DocumentPosition(po, true), new DocumentPosition(po, true));
                                        RichTextEditor.Document.Selection.AddSelectionStart(startRange.StartPosition);
                                        RichTextEditor.Document.Selection.AddSelectionEnd(startRange.EndPosition);
                                        RichTextEditor.InsertInline(rangeStart);
                                        startRange.StartPosition.Dispose();
                                        startRange.EndPosition.Dispose();

                                        while (index != start + length)
                                        {
                                            po.MoveToNext();
                                            index++;
                                        }

                                        TextRange endRange = new TextRange(new DocumentPosition(po, true), new DocumentPosition(po, true));
                                        RichTextEditor.Document.Selection.AddSelectionStart(endRange.StartPosition);
                                        RichTextEditor.Document.Selection.AddSelectionEnd(endRange.EndPosition);
                                        RichTextEditor.InsertInline(rangeEnd);
                                        endRange.StartPosition.Dispose();
                                        endRange.EndPosition.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }

                ClearKey(RichTextEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class TextRangeProtected
        {
            public TextRange StartRange { get; set; }
            public TextRange EndRange { get; set; }
        }

        private static void ClearKey(Telerik.WinControls.UI.RadRichTextEditor RichTextEditor)
        {
            try
            {
                if (RichTextEditor != null)
                {
                    Telerik.WinForms.Documents.TextSearch.DocumentTextSearch search = new Telerik.WinForms.Documents.TextSearch.DocumentTextSearch(RichTextEditor.Document);

                    var rangesB = search.FindAll(keyBegin);
                    var rangesE = search.FindAll(keyEnd);

                    if (rangesB != null && rangesB.Count() > 0)
                    {
                        foreach (var item in rangesB)
                        {
                            TextRange sRange = new TextRange(new DocumentPosition(item.StartPosition, true), new DocumentPosition(item.EndPosition, true));
                            RichTextEditor.Document.Selection.AddSelectionStart(sRange.StartPosition);
                            RichTextEditor.Document.Selection.AddSelectionEnd(sRange.EndPosition);
                            RichTextEditor.Insert(" ");
                            sRange.StartPosition.Dispose();
                            sRange.EndPosition.Dispose();
                        }
                    }

                    if (rangesE != null && rangesE.Count() > 0)
                    {
                        foreach (var item in rangesE)
                        {
                            TextRange sRange = new TextRange(new DocumentPosition(item.StartPosition, true), new DocumentPosition(item.EndPosition, true));
                            RichTextEditor.Document.Selection.AddSelectionStart(sRange.StartPosition);
                            RichTextEditor.Document.Selection.AddSelectionEnd(sRange.EndPosition);
                            RichTextEditor.Insert(" ");
                            sRange.StartPosition.Dispose();
                            sRange.EndPosition.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
