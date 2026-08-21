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
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace HIS.Desktop.Plugins.EmrDocument.Worker
{
    /// <summary>
    /// Ghep ngang cac phieu cung mau thanh mot to: moi lan nhan dinh mot cot.
    ///
    /// Cach map hang: phieu co ca hang nhan CO DINH (Mach, Nhiet do, Huyet ap...) va hang nhan
    /// TU DO do dieu duong tu nhap (moi phieu mot khac, so luong khac nhau). Vi vay:
    ///  - Hang nhan co dinh (nhan xuat hien dung 1 lan o ca hai phieu) dung lam MOC dong bo.
    ///  - Hang nhan tu do giua hai moc duoc ghep THEO THU TU trong cung khoang moc do.
    /// Nho vay hai phieu lech dong/khac so dong van khong bao gio dat du lieu sang nhom khac.
    ///
    /// Dung iTextSharp da co san trong plugin, khong them thu vien moi.
    /// </summary>
    internal class EmrDocumentMergeColumnsWorker
    {
        #region Declare - hinh hoc mau phieu (don vi point, A4 doc 595x842)

        /// <summary>Bien trai cua cot du lieu dau tien.</summary>
        private const float SLOT_X0 = 163.15f;

        /// <summary>Be rong mot cot du lieu.</summary>
        private const float SLOT_W = 46.27f;

        /// <summary>So cot du lieu tren mau phieu.</summary>
        internal const int SLOT_COUNT = 6;

        /// <summary>Day bang du lieu.</summary>
        private const float TABLE_BOTTOM = 38f;

        /// <summary>Dinh bang du lieu.</summary>
        private const float TABLE_TOP = 722f;

        /// <summary>Buoc hang mac dinh (dung cho hang tren cung khi chua biet hang truoc).</summary>
        private const float ROW_H = 11.11f;

        /// <summary>Dai X cua cot nhan nhom.</summary>
        private const float LABEL_X_MIN = 16f, LABEL_X_MAX = 26f;

        /// <summary>Dai X cua cot nhan chi tiet (gom "Ngay, thang" va "Gio, phut").</summary>
        private const float CONTENT_X_MIN = 60f, CONTENT_X_MAX = 80f;

        /// <summary>Ten hang moc de tach khoi chu ky.</summary>
        private const string SIGN_ROW_KEY = "TÊN ĐIỀU DƯỠNG";

        /// <summary>Ten hang chua gio nhan dinh - dung de xac dinh cot dang dung va sap thu tu cot.</summary>
        private const string TIME_ROW_KEY = "Giờ, phút";

        /// <summary>Ten hang chua ngay nhan dinh.</summary>
        private const string DATE_ROW_KEY = "Ngày, tháng";

        /// <summary>Do lech xuong duoi duong co so de lay tron o.</summary>
        private const float CELL_PADDING_BOTTOM = 2.5f;

        /// <summary>Phan noi bien hai ben cot khi lay khoi chu ky (dau ky thuong rong hon o).</summary>
        private const float SIGN_BLOCK_MARGIN_RATIO = 0.35f;

        /// <summary>Ty le nen ngang toi thieu cua khoi chu ky - nen hon nua thi chu qua nho.</summary>
        private const float SIGN_MIN_SCALE_X = 0.7f;

        #endregion

        #region Inner types

        private class TextItem
        {
            internal string Text;
            internal float X0, Y0, X1;

            internal float XCenter { get { return (this.X0 + this.X1) / 2f; } }
        }

        private class RowInfo
        {
            internal string Label;
            internal float Y;
        }

        private class SheetInfo
        {
            internal string FilePath;
            internal List<TextItem> Items = new List<TextItem>();

            /// <summary>Cac hang nhan, sap tu tren xuong duoi.</summary>
            internal List<RowInfo> RowList = new List<RowInfo>();

            /// <summary>So lan xuat hien cua tung nhan - nhan xuat hien 1 lan moi duoc dung lam moc.</summary>
            internal Dictionary<string, int> LabelCount = new Dictionary<string, int>();

            internal List<int> UsedSlots = new List<int>();
            internal string SortKey = "99999999 99:99";

            /// <summary>Duong co so hang "TEN DIEU DUONG" - dinh cua khoi chu ky.</summary>
            internal float SignTop = -1f;

            /// <summary>Day thuc te cua khoi chu ky do tren chinh phieu nay (khong dung hang so).</summary>
            internal float SignBottom = TABLE_BOTTOM;

            internal float GetRowY(string label)
            {
                RowInfo row = this.RowList.FirstOrDefault(o => o.Label == label);
                return row != null ? row.Y : -1f;
            }
        }

        private class TextCollector : IRenderListener
        {
            internal List<TextItem> Items = new List<TextItem>();

            public void RenderText(TextRenderInfo info)
            {
                try
                {
                    string text = info.GetText();
                    if (String.IsNullOrEmpty(text) || text.Trim().Length == 0) return;

                    var descent = info.GetDescentLine();
                    var ascent = info.GetAscentLine();
                    this.Items.Add(new TextItem()
                    {
                        Text = text,
                        X0 = descent.GetStartPoint()[Vector.I1],
                        Y0 = descent.GetStartPoint()[Vector.I2],
                        X1 = ascent.GetEndPoint()[Vector.I1]
                    });
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            public void BeginTextBlock() { }
            public void EndTextBlock() { }
            public void RenderImage(ImageRenderInfo renderInfo) { }
        }

        #endregion

        /// <summary>
        /// Ghep ngang danh sach phieu thanh mot to.
        /// </summary>
        /// <param name="pdfFilePaths">Duong dan cac file PDF phieu goc (>= 2 file).</param>
        /// <param name="warning">Canh bao tra ve cho nguoi dung (rong neu ghep tron ven).</param>
        /// <returns>Noi dung PDF to gop, null neu that bai.</returns>
        internal static byte[] Merge(List<string> pdfFilePaths, out string warning)
        {
            warning = "";
            try
            {
                if (pdfFilePaths == null || pdfFilePaths.Count < 2) return null;

                List<SheetInfo> sheets = new List<SheetInfo>();
                foreach (string path in pdfFilePaths)
                {
                    SheetInfo sheet = ReadSheet(path);
                    if (sheet != null) sheets.Add(sheet);
                }
                if (sheets.Count < 2) return null;

                sheets = sheets.OrderBy(o => o.SortKey).ToList();
                SheetInfo baseSheet = sheets[0];

                List<int> freeSlots = new List<int>();
                for (int slot = 0; slot < SLOT_COUNT; slot++)
                {
                    if (!baseSheet.UsedSlots.Contains(slot)) freeSlots.Add(slot);
                }

                if (freeSlots.Count == 0)
                {
                    warning = "Phiếu có giờ sớm nhất đã dùng hết cột, không còn chỗ để gộp thêm.";
                    return null;
                }

                float signRowYBase = baseSheet.GetRowY(SIGN_ROW_KEY);
                if (signRowYBase < 0)
                {
                    warning = "Không nhận diện được cấu trúc phiếu (thiếu hàng tên điều dưỡng).";
                    return null;
                }

                int totalSkippedRow = 0;
                int notMergedSheet = 0;
                bool isOverSlot = false;

                using (MemoryStream output = new MemoryStream())
                {
                    PdfReader readerBase = new PdfReader(baseSheet.FilePath);
                    PdfStamper stamper = new PdfStamper(readerBase, output);
                    stamper.SetFullCompression();
                    PdfContentByte canvas = stamper.GetOverContent(1);

                    //Cac reader nguon phai giu mo den sau khi dong stamper: noi dung trang import
                    //chi duoc ghi ra khi stamper.Close() -> dong reader som se loi "Cannot access a closed file"
                    List<PdfReader> sourceReaders = new List<PdfReader>();

                    int freeIndex = 0;
                    for (int index = 1; index < sheets.Count; index++)
                    {
                        SheetInfo source = sheets[index];
                        PdfReader readerSource = new PdfReader(source.FilePath);
                        sourceReaders.Add(readerSource);
                        PdfImportedPage importedPage = stamper.GetImportedPage(readerSource, 1);

                        float signRowYSource = source.GetRowY(SIGN_ROW_KEY);
                        if (signRowYSource < 0)
                        {
                            notMergedSheet++;
                            continue;
                        }

                        foreach (int sourceSlot in source.UsedSlots)
                        {
                            if (freeIndex >= freeSlots.Count)
                            {
                                isOverSlot = true;
                                break;
                            }

                            int targetSlot = freeSlots[freeIndex++];
                            int skipped = PasteSlot(canvas, importedPage, baseSheet, source,
                                sourceSlot, targetSlot, signRowYBase, signRowYSource);
                            totalSkippedRow += skipped;
                        }
                    }

                    stamper.Close();
                    readerBase.Close();
                    foreach (PdfReader readerSource in sourceReaders)
                    {
                        try
                        {
                            readerSource.Close();
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }

                    List<string> warns = new List<string>();
                    if (notMergedSheet > 0)
                        warns.Add(String.Format("{0} phiếu không đúng cấu trúc nên chưa được gộp", notMergedSheet));
                    if (isOverSlot)
                        warns.Add("vượt quá số cột của một phiếu nên các lần nhận định còn lại chưa được gộp");
                    if (totalSkippedRow > 0)
                        warns.Add(String.Format("{0} dòng của phiếu sau không còn dòng trống tương ứng trên phiếu nền nên chưa được gộp", totalSkippedRow));

                    if (warns.Count > 0)
                        warning = "Lưu ý: " + String.Join("; ", warns) + ". Vui lòng đối chiếu với phiếu gốc.";

                    return output.ToArray();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                warning = "Không gộp được phiếu. Vui lòng kiểm tra lại hoặc liên hệ quản trị hệ thống.";
                return null;
            }
        }

        /// <summary>
        /// Dan mot cot du lieu tu phieu nguon sang cot dich tren to gop.
        /// Tra ve so dong khong dat duoc (khong con dong trong tuong ung tren phieu nen).
        /// </summary>
        private static int PasteSlot(PdfContentByte canvas, PdfImportedPage importedPage,
            SheetInfo baseSheet, SheetInfo source, int sourceSlot, int targetSlot,
            float signRowYBase, float signRowYSource)
        {
            int skipped = 0;
            try
            {
                float sourceSlotX0 = SLOT_X0 + sourceSlot * SLOT_W;
                float targetSlotX0 = SLOT_X0 + targetSlot * SLOT_W;

                //Chi xet cac hang PHIA TREN hang chu ky; khoi chu ky xu ly rieng ben duoi
                List<RowInfo> sourceRows = source.RowList.Where(o => o.Y > signRowYSource).ToList();
                List<RowInfo> baseRows = baseSheet.RowList.Where(o => o.Y > signRowYBase).ToList();

                //1a. Hai phieu co CUNG SO HANG (vien chuan hoa cot ben trai giong nhau)
                //    -> ghep thang theo thu tu hang, khong can doi chieu nhan. Chinh xac tuyet doi.
                if (sourceRows.Count == baseRows.Count && sourceRows.Count > 0)
                {
                    for (int rowIndex = 0; rowIndex < sourceRows.Count; rowIndex++)
                    {
                        float srcBottom, srcHeight, tgtBottom, tgtHeight;
                        GetRowRect(sourceRows, rowIndex, out srcBottom, out srcHeight);
                        GetRowRect(baseRows, rowIndex, out tgtBottom, out tgtHeight);
                        PasteCell(canvas, importedPage, sourceSlotX0, srcBottom, srcHeight,
                            targetSlotX0, tgtBottom, tgtHeight);
                    }
                    PasteSignBlock(canvas, importedPage, baseSheet, source, sourceSlot, targetSlot,
                        sourceSlotX0, targetSlotX0, signRowYBase, signRowYSource);
                    return 0;
                }

                //1b. Khac so hang: nhan co dinh dong bo, nhan tu do ghep theo thu tu trong cung khoang moc
                int baseIndex = 0;
                for (int sourceIndex = 0; sourceIndex < sourceRows.Count; sourceIndex++)
                {
                    RowInfo sourceRow = sourceRows[sourceIndex];
                    bool isAnchor = IsAnchorLabel(sourceRow.Label, baseSheet, source);

                    int targetIndex = -1;
                    if (isAnchor)
                    {
                        //Dong bo ve dung hang moc tren phieu nen
                        for (int probe = baseIndex; probe < baseRows.Count; probe++)
                        {
                            if (baseRows[probe].Label == sourceRow.Label) { targetIndex = probe; break; }
                        }
                        if (targetIndex < 0) { skipped++; continue; }
                    }
                    else
                    {
                        //Hang nhan tu do: chi dat vao hang tu do ke tiep cua phieu nen
                        if (baseIndex < baseRows.Count && !IsAnchorLabel(baseRows[baseIndex].Label, baseSheet, source))
                            targetIndex = baseIndex;
                        else { skipped++; continue; }
                    }

                    float sourceBottom, sourceHeight, targetBottom, targetHeight;
                    GetRowRect(sourceRows, sourceIndex, out sourceBottom, out sourceHeight);
                    GetRowRect(baseRows, targetIndex, out targetBottom, out targetHeight);

                    PasteCell(canvas, importedPage, sourceSlotX0, sourceBottom, sourceHeight,
                        targetSlotX0, targetBottom, targetHeight);

                    baseIndex = targetIndex + 1;
                }

                //2. Khoi chu ky
                PasteSignBlock(canvas, importedPage, baseSheet, source, sourceSlot, targetSlot,
                    sourceSlotX0, targetSlotX0, signRowYBase, signRowYSource);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return skipped;
        }

        /// <summary>
        /// Dan khoi ten dieu duong + chu ky o chan cot, nen cho vua khung chu ky cua phieu nen.
        /// Day khoi chu ky do tren tung phieu (SignBottom) chu khong dung hang so.
        /// </summary>
        private static void PasteSignBlock(PdfContentByte canvas, PdfImportedPage importedPage,
            SheetInfo baseSheet, SheetInfo source, int sourceSlot, int targetSlot,
            float sourceSlotX0, float targetSlotX0, float signRowYBase, float signRowYSource)
        {
            try
            {
                //Chi lay cac doan chu co TAM nam trong pham vi cot (noi bien mot chut cho dau ky tran ra),
                //de khong bat sang dau ky cua nguoi ky ke ben khi phieu co nhieu nguoi ky.
                float signScanMargin = SLOT_W * SIGN_BLOCK_MARGIN_RATIO;
                float signScanX0 = sourceSlotX0 - signScanMargin;
                float signScanX1 = sourceSlotX0 + SLOT_W + signScanMargin;

                float signBlockX0 = sourceSlotX0;
                float signBlockX1 = sourceSlotX0 + SLOT_W;
                foreach (TextItem item in source.Items)
                {
                    if (item.Y0 >= signRowYSource || item.Y0 < source.SignBottom) continue;
                    if (item.XCenter < signScanX0 || item.XCenter > signScanX1) continue;
                    if (item.X0 < signBlockX0) signBlockX0 = item.X0;
                    if (item.X1 > signBlockX1) signBlockX1 = item.X1;
                }

                //Khoa lai trong pham vi quet: rong hon nua thi cat bo phan ngoai thay vi nen nho chu
                if (signBlockX0 < signScanX0) signBlockX0 = signScanX0;
                if (signBlockX1 > signScanX1) signBlockX1 = signScanX1;

                float signBlockWidth = Math.Max(1f, signBlockX1 - signBlockX0);
                float targetSignHeight = Math.Max(1f, signRowYBase - baseSheet.SignBottom);
                float sourceSignHeight = Math.Max(1f, signRowYSource - source.SignBottom);

                float scaleXSign = Math.Max(SIGN_MIN_SCALE_X, Math.Min(1f, SLOT_W / signBlockWidth));
                float scaleYSign = Math.Min(1f, targetSignHeight / sourceSignHeight);

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MergeColumns sign block: srcSlot={0} tgtSlot={1} srcSignTop={2} srcSignBottom={3} srcH={4} baseSignTop={5} baseSignBottom={6} tgtH={7} blockX0={8} blockW={9} scaleX={10} scaleY={11}",
                    sourceSlot, targetSlot, signRowYSource, source.SignBottom, sourceSignHeight,
                    signRowYBase, baseSheet.SignBottom, targetSignHeight, signBlockX0, signBlockWidth, scaleXSign, scaleYSign));

                PdfTemplate signCell = canvas.CreateTemplate(SLOT_W, targetSignHeight);
                signCell.AddTemplate(importedPage, scaleXSign, 0, 0, scaleYSign,
                    -signBlockX0 * scaleXSign, -source.SignBottom * scaleYSign);
                canvas.AddTemplate(signCell, targetSlotX0, baseSheet.SignBottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dan noi dung mot o: cat theo o nguon, nen dung chieu cao o dich.</summary>
        private static void PasteCell(PdfContentByte canvas, PdfImportedPage importedPage,
            float sourceX0, float sourceBottom, float sourceHeight,
            float targetX0, float targetBottom, float targetHeight)
        {
            try
            {
                if (sourceHeight <= 0f || targetHeight <= 0f) return;

                float scaleY = targetHeight / sourceHeight;

                //Template co BoundingBox = kich thuoc o dich -> noi dung tu cat theo o, khong tran sang o khac
                PdfTemplate cell = canvas.CreateTemplate(SLOT_W, targetHeight);
                cell.AddTemplate(importedPage, 1, 0, 0, scaleY, -sourceX0, -sourceBottom * scaleY);
                canvas.AddTemplate(cell, targetX0, targetBottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lay day va chieu cao thuc te cua mot hang (dua vao hang ngay phia tren).</summary>
        private static void GetRowRect(List<RowInfo> rows, int index, out float bottom, out float height)
        {
            bottom = rows[index].Y - CELL_PADDING_BOTTOM;
            if (index > 0)
                height = Math.Max(1f, rows[index - 1].Y - rows[index].Y);
            else
                height = ROW_H;
        }

        /// <summary>
        /// Nhan co dinh dung lam moc dong bo: xuat hien DUNG MOT LAN o ca phieu nen va phieu nguon.
        /// Nhan tu do dieu duong nhap (moi phieu mot khac) khong thoa dieu kien nay.
        /// </summary>
        private static bool IsAnchorLabel(string label, SheetInfo baseSheet, SheetInfo source)
        {
            try
            {
                int countBase, countSource;
                if (!baseSheet.LabelCount.TryGetValue(label, out countBase)) return false;
                if (!source.LabelCount.TryGetValue(label, out countSource)) return false;
                return countBase == 1 && countSource == 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>Doc mot phieu: lay danh sach text kem toa do, luoi hang va cot dang dung.</summary>
        private static SheetInfo ReadSheet(string filePath)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

                SheetInfo sheet = new SheetInfo() { FilePath = filePath };

                PdfReader reader = new PdfReader(filePath);
                try
                {
                    TextCollector collector = new TextCollector();
                    new PdfReaderContentParser(reader).ProcessContent(1, collector);
                    sheet.Items = MergeSameLine(collector.Items);
                }
                finally
                {
                    reader.Close();
                }

                foreach (TextItem item in sheet.Items)
                {
                    bool isLabel = (item.X0 >= LABEL_X_MIN && item.X0 <= LABEL_X_MAX)
                                || (item.X0 >= CONTENT_X_MIN && item.X0 <= CONTENT_X_MAX);
                    if (!isLabel) continue;
                    if (item.Y0 > TABLE_TOP || item.Y0 < TABLE_BOTTOM) continue;

                    string key = Normalize(item.Text);
                    if (key.Length == 0) continue;

                    sheet.RowList.Add(new RowInfo() { Label = key, Y = item.Y0 });
                    if (sheet.LabelCount.ContainsKey(key)) sheet.LabelCount[key] = sheet.LabelCount[key] + 1;
                    else sheet.LabelCount[key] = 1;
                }
                sheet.RowList = sheet.RowList.OrderByDescending(o => o.Y).ToList();

                sheet.UsedSlots = GetUsedSlots(sheet);
                sheet.SortKey = GetSortKey(sheet);
                sheet.SignTop = sheet.GetRowY(SIGN_ROW_KEY);
                sheet.SignBottom = GetSignBottom(sheet);

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MergeColumns read sheet: sortKey={0} rows={1} usedSlots=[{2}] signTop={3} signBottom={4}",
                    sheet.SortKey, sheet.RowList.Count, String.Join(",", sheet.UsedSlots), sheet.SignTop, sheet.SignBottom));

                return sheet;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Cot dang dung: xac dinh theo hang "Gio, phut" (moi lan nhan dinh deu co gio).
        /// Dung TAM cua doan chu de khong bi tinh lan sang cot ke ben khi chu dai hon o.
        /// </summary>
        private static List<int> GetUsedSlots(SheetInfo sheet)
        {
            List<int> result = new List<int>();
            try
            {
                float rowY = sheet.GetRowY(TIME_ROW_KEY);
                if (rowY < 0) rowY = sheet.GetRowY(DATE_ROW_KEY);
                if (rowY < 0) return result;

                bool isTimeRow = sheet.GetRowY(TIME_ROW_KEY) >= 0;

                HashSet<int> used = new HashSet<int>();
                foreach (TextItem item in sheet.Items)
                {
                    if (Math.Abs(item.Y0 - rowY) > 3f) continue;
                    if (item.XCenter < SLOT_X0) continue;

                    //O phai chua dung dinh dang gio (HH:mm) hoac ngay (dd/MM/yyyy).
                    //Loai cac doan chu rong/khong dung dang -> khong nhan lam cot dang dung.
                    string text = Normalize(item.Text);
                    if (text.Length == 0) continue;
                    if (!HasDigit(text)) continue;
                    if (isTimeRow ? !text.Contains(":") : !text.Contains("/")) continue;

                    int slot = (int)Math.Floor((item.XCenter - SLOT_X0) / SLOT_W);
                    if (slot >= 0 && slot < SLOT_COUNT) used.Add(slot);
                }

                //Chan them: cot khong co bat ky noi dung nao trong vung du lieu thi khong dan
                foreach (int slot in used.OrderBy(o => o))
                {
                    if (HasContentInSlot(sheet, slot)) result.Add(slot);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static bool HasDigit(string text)
        {
            if (String.IsNullOrEmpty(text)) return false;
            foreach (char c in text)
            {
                if (c >= '0' && c <= '9') return true;
            }
            return false;
        }

        /// <summary>Cot co noi dung thuc su trong vung du lieu (tu hang chu ky tro len) hay khong.</summary>
        private static bool HasContentInSlot(SheetInfo sheet, int slot)
        {
            try
            {
                float signTop = sheet.GetRowY(SIGN_ROW_KEY);
                if (signTop < 0) signTop = TABLE_BOTTOM;

                float slotX0 = SLOT_X0 + slot * SLOT_W;
                foreach (TextItem item in sheet.Items)
                {
                    if (item.Y0 < signTop || item.Y0 > TABLE_TOP) continue;
                    if (item.XCenter < slotX0 || item.XCenter > slotX0 + SLOT_W) continue;
                    if (Normalize(item.Text).Length > 0) return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Day thuc te cua khoi chu ky: chu thap nhat nam duoi hang "TEN DIEU DUONG" va con trong
        /// pham vi bang (bo qua chu o panel ben phai va chan trang).
        /// </summary>
        private static float GetSignBottom(SheetInfo sheet)
        {
            float result = TABLE_BOTTOM;
            try
            {
                float signTop = sheet.GetRowY(SIGN_ROW_KEY);
                if (signTop < 0) return result;

                float tableRight = SLOT_X0 + SLOT_COUNT * SLOT_W;
                float minY = signTop;
                foreach (TextItem item in sheet.Items)
                {
                    if (item.Y0 >= signTop) continue;
                    if (item.XCenter > tableRight) continue;   //panel ben phai
                    if (Normalize(item.Text).Length == 0) continue;
                    if (item.Y0 < minY) minY = item.Y0;
                }

                //Tru mot chut de lay tron phan chan chu
                result = Math.Max(0f, minY - CELL_PADDING_BOTTOM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Gop cac doan text cung dong, ke sat nhau thanh mot cum.</summary>
        private static List<TextItem> MergeSameLine(List<TextItem> raw)
        {
            List<TextItem> result = new List<TextItem>();
            try
            {
                List<TextItem> sorted = raw.OrderByDescending(o => Math.Round(o.Y0, 1))
                                           .ThenBy(o => o.X0).ToList();
                foreach (TextItem item in sorted)
                {
                    TextItem last = result.Count > 0 ? result[result.Count - 1] : null;
                    if (last != null && Math.Abs(last.Y0 - item.Y0) <= 2f
                        && (item.X0 - last.X1) <= 3f && (item.X0 - last.X1) >= -3f)
                    {
                        last.Text += item.Text;
                        if (item.X1 > last.X1) last.X1 = item.X1;
                    }
                    else
                    {
                        result.Add(new TextItem() { Text = item.Text, X0 = item.X0, Y0 = item.Y0, X1 = item.X1 });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Khoa sap thu tu cot: theo NGAY roi den GIO nhan dinh (dang "yyyyMMdd HH:mm").
        /// Gop phieu khac ngay van sap dung thu tu thoi gian.
        /// </summary>
        private static string GetSortKey(SheetInfo sheet)
        {
            string dateKey = "99999999";
            string timeKey = "99:99";
            try
            {
                string dateText = GetFirstValueOnRow(sheet, DATE_ROW_KEY);
                if (!String.IsNullOrEmpty(dateText))
                {
                    string[] parts = dateText.Split(new char[] { '/', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && parts[0].Length <= 2 && parts[1].Length <= 2 && parts[2].Length == 4)
                        dateKey = parts[2] + parts[1].PadLeft(2, '0') + parts[0].PadLeft(2, '0');
                }

                string timeText = GetFirstValueOnRow(sheet, TIME_ROW_KEY);
                if (!String.IsNullOrEmpty(timeText) && timeText.Contains(":"))
                    timeKey = timeText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dateKey + " " + timeKey;
        }

        /// <summary>Lay gia tri o dau tien (cot du lieu ben trai nhat) tren mot hang.</summary>
        private static string GetFirstValueOnRow(SheetInfo sheet, string rowLabel)
        {
            try
            {
                float rowY = sheet.GetRowY(rowLabel);
                if (rowY < 0) return null;

                TextItem first = sheet.Items.Where(o => Math.Abs(o.Y0 - rowY) <= 3f && o.XCenter >= SLOT_X0)
                                            .OrderBy(o => o.XCenter).FirstOrDefault();
                return first != null ? Normalize(first.Text) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private static string Normalize(string text)
        {
            if (text == null) return "";
            return String.Join(" ", text.Trim().Split(
                new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
