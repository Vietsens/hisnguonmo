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
        private const float SIGN_MIN_SCALE_X = 0.4f;

        /// <summary>Chua le hai ben trong o chu ky de khoi khong dinh net ke.</summary>
        private const float SIGN_CELL_PADDING = 1.5f;

        /// <summary>
        /// Chieu cao hang chu ky so voi mot hang thuong, lay tu mau phieu rptPhieuTDVaCSCap23_QN:
        /// hang "TEN DIEU DUONG" (tableRow4) Weight=1.5699879 / hang thuong Weight=0.8 = 1.9625.
        /// </summary>
        private const float SIGN_ROW_HEIGHT_RATIO = 1.9625f;

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

            /// <summary>Duong co so cua chu nhan (dung khi khong doc duoc luoi).</summary>
            internal float Y;

            /// <summary>Bien tren/duoi thuc te cua hang.</summary>
            internal float Top;
            internal float Bottom;
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

            /// <summary>Luoi o doc tu net ve cua bang. Null neu phieu khong co net (anh scan...).</summary>
            internal SheetGrid Grid;

            //Hinh hoc dung cho phieu nay: uu tien luoi doc duoc, khong co thi dung hang so do tay
            internal float SlotX0 { get { return this.Grid != null ? this.Grid.ColumnX[0] : SLOT_X0; } }
            internal float SlotW { get { return this.Grid != null ? this.Grid.ColumnWidth : SLOT_W; } }
            internal int SlotCount { get { return this.Grid != null ? this.Grid.ColumnCount : SLOT_COUNT; } }
            internal float TableTopY { get { return this.Grid != null ? this.Grid.TableTop : TABLE_TOP; } }
            internal float TableBottomY { get { return this.Grid != null ? this.Grid.TableBottom : TABLE_BOTTOM; } }
            internal float LabelZoneX0 { get { return this.Grid != null ? this.Grid.LabelX0 - 1f : LABEL_X_MIN; } }
            internal float LabelZoneX1 { get { return this.Grid != null ? this.Grid.LabelX1 : CONTENT_X_MAX; } }

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
                for (int slot = 0; slot < baseSheet.SlotCount; slot++)
                {
                    if (!baseSheet.UsedSlots.Contains(slot)) freeSlots.Add(slot);
                }

                if (freeSlots.Count == 0)
                {
                    warning = "Phiếu có giờ sớm nhất đã dùng hết cột, không còn chỗ để gộp thêm.";
                    return null;
                }

                float signRowYBase = baseSheet.SignTop;   //luoi net ve cho bien chinh xac; duong text chi la du phong
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

                        float signRowYSource = source.SignTop;
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
                float sourceSlotX0 = source.SlotX0 + sourceSlot * source.SlotW;
                float targetSlotX0 = baseSheet.SlotX0 + targetSlot * baseSheet.SlotW;

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
                            targetSlotX0, tgtBottom, tgtHeight, baseSheet.SlotW);
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
                        targetSlotX0, targetBottom, targetHeight, baseSheet.SlotW);

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
                //Neo theo MEP COT, khong neo theo chu ngoai cung ben trai cua khoi.
                //Dau ky trong phieu goc von tran ra ngoai o (chuoi "Nguoi Ky: ..." rong hon o),
                //neu neo theo chu thi ca khoi bi day lech khoi o. Cho phep tran ra hai ben mot khoang
                //bang phieu goc va chi nen ngang khi khoi rong hon ca khung tran.
                //Cat DUNG THEO O nhu cac o du lieu khac: khong noi bien, khong dich, khong nen ngang.
                //Neu noi bien thi vung lay se kem ca manh cua o ben canh va NET KE cua phieu goc,
                //dan sang o dich se sinh vach doc la va de len cot truoc.
                float targetSignHeight = Math.Max(1f, signRowYBase - baseSheet.SignBottom);
                float sourceSignHeight = Math.Max(1f, signRowYSource - source.SignBottom);
                float scaleYSign = Math.Min(1f, targetSignHeight / sourceSignHeight);

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MergeColumns sign block: srcSlot={0} tgtSlot={1} srcSignTop={2} srcSignBottom={3} srcH={4} baseSignTop={5} baseSignBottom={6} tgtH={7} srcX0={8} tgtX0={9} cellW={10} scaleY={11}",
                    sourceSlot, targetSlot, signRowYSource, source.SignBottom, sourceSignHeight,
                    signRowYBase, baseSheet.SignBottom, targetSignHeight, sourceSlotX0, targetSlotX0,
                    baseSheet.SlotW, scaleYSign));

                PasteCell(canvas, importedPage, sourceSlotX0, source.SignBottom, sourceSignHeight,
                    targetSlotX0, baseSheet.SignBottom, targetSignHeight, baseSheet.SlotW);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dan noi dung mot o: cat theo o dich, nen doc cho vua chieu cao o dich.</summary>
        private static void PasteCell(PdfContentByte canvas, PdfImportedPage importedPage,
            float sourceX0, float sourceBottom, float sourceHeight,
            float targetX0, float targetBottom, float targetHeight, float targetWidth)
        {
            try
            {
                if (sourceHeight <= 0f || targetHeight <= 0f || targetWidth <= 0f) return;

                float scaleY = targetHeight / sourceHeight;

                //Template co BoundingBox = kich thuoc o dich -> noi dung tu cat theo o, khong tran sang o khac
                PdfTemplate cell = canvas.CreateTemplate(targetWidth, targetHeight);
                cell.AddTemplate(importedPage, 1, 0, 0, scaleY, -sourceX0, -sourceBottom * scaleY);
                canvas.AddTemplate(cell, targetX0, targetBottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay day va chieu cao thuc te cua mot hang. Uu tien bien hang doc tu luoi net ve;
        /// neu chua co thi suy ra tu duong co so cua hang lien truoc.
        /// </summary>
        private static void GetRowRect(List<RowInfo> rows, int index, out float bottom, out float height)
        {
            RowInfo row = rows[index];
            if (row.Top > row.Bottom && row.Bottom > 0f)
            {
                bottom = row.Bottom;
                height = Math.Max(1f, row.Top - row.Bottom);
                return;
            }

            bottom = row.Y - CELL_PADDING_BOTTOM;
            if (index > 0)
                height = Math.Max(1f, rows[index - 1].Y - row.Y);
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

                //Uu tien doc luoi o tu net ve cua bang; khong co net thi do theo chu nhu truoc
                sheet.Grid = EmrDocumentSheetGridReader.Read(filePath);

                if (sheet.Grid != null)
                    BuildRowsFromGrid(sheet);
                else
                    BuildRowsFromText(sheet);

                sheet.UsedSlots = GetUsedSlots(sheet);
                sheet.SortKey = GetSortKey(sheet);

                if (sheet.Grid != null)
                {
                    //Luoi cho biet chinh xac bien hang chu ky - khong phai doan theo chu
                    sheet.SignTop = sheet.Grid.SignTop;
                    sheet.SignBottom = sheet.Grid.SignBottom;
                }
                else
                {
                    sheet.SignTop = sheet.GetRowY(SIGN_ROW_KEY);
                    sheet.SignBottom = GetSignBottom(sheet);
                }

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MergeColumns read sheet: nguon={0} sortKey={1} rows={2} usedSlots=[{3}] slotX0={4} slotW={5} slotCount={6} signTop={7} signBottom={8}",
                    sheet.Grid != null ? "LUOI NET VE" : "DO THEO CHU", sheet.SortKey, sheet.RowList.Count,
                    String.Join(",", sheet.UsedSlots), sheet.SlotX0, sheet.SlotW, sheet.SlotCount,
                    sheet.SignTop, sheet.SignBottom));

                return sheet;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Dung danh sach hang tu luoi net ve: moi khoang giua hai net ngang la mot hang,
        /// nhan cua hang lay tu chu nam trong vung cot nhan cua chinh hang do.
        /// </summary>
        private static void BuildRowsFromGrid(SheetInfo sheet)
        {
            try
            {
                List<float> bounds = sheet.Grid.RowY;

                //Khoang cuoi cung la hang chu ky -> khong tinh vao danh sach hang du lieu
                for (int index = 0; index < bounds.Count - 2; index++)
                {
                    float top = bounds[index];
                    float bottom = bounds[index + 1];
                    if (top - bottom < 2f) continue;

                    //Nhan chi tiet (ben phai) uu tien hon nhan nhom (ben trai)
                    TextItem label = sheet.Items
                        .Where(o => o.Y0 >= bottom - 1f && o.Y0 < top - 1f
                                 && o.X0 >= sheet.LabelZoneX0 && o.X0 <= sheet.LabelZoneX1
                                 && Normalize(o.Text).Length > 0)
                        .OrderByDescending(o => o.X0).FirstOrDefault();

                    string key = label != null ? Normalize(label.Text) : "";

                    sheet.RowList.Add(new RowInfo()
                    {
                        Label = key,
                        Y = label != null ? label.Y0 : bottom,
                        Top = top,
                        Bottom = bottom
                    });

                    if (key.Length > 0)
                    {
                        if (sheet.LabelCount.ContainsKey(key)) sheet.LabelCount[key] = sheet.LabelCount[key] + 1;
                        else sheet.LabelCount[key] = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dung danh sach hang theo chu o cot nhan (dung khi phieu khong co net ve).</summary>
        private static void BuildRowsFromText(SheetInfo sheet)
        {
            try
            {
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

                //Suy ra bien tren/duoi tu duong co so cua hang lien truoc
                for (int index = 0; index < sheet.RowList.Count; index++)
                {
                    RowInfo row = sheet.RowList[index];
                    row.Bottom = row.Y - CELL_PADDING_BOTTOM;
                    row.Top = index > 0 ? sheet.RowList[index - 1].Y - CELL_PADDING_BOTTOM : row.Bottom + ROW_H;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                if (rowY < 0)
                {
                    //Mau phieu khong co hang gio/ngay (phieu khac dang): cot nao co noi dung thi coi la dang dung
                    for (int slot = 0; slot < sheet.SlotCount; slot++)
                    {
                        if (HasContentInSlot(sheet, slot)) result.Add(slot);
                    }
                    return result;
                }

                bool isTimeRow = sheet.GetRowY(TIME_ROW_KEY) >= 0;

                HashSet<int> used = new HashSet<int>();
                foreach (TextItem item in sheet.Items)
                {
                    if (Math.Abs(item.Y0 - rowY) > 3f) continue;
                    if (item.XCenter < sheet.SlotX0) continue;

                    //O phai chua dung dinh dang gio (HH:mm) hoac ngay (dd/MM/yyyy).
                    //Loai cac doan chu rong/khong dung dang -> khong nhan lam cot dang dung.
                    string text = Normalize(item.Text);
                    if (text.Length == 0) continue;
                    if (!HasDigit(text)) continue;
                    if (isTimeRow ? !text.Contains(":") : !text.Contains("/")) continue;

                    int slot = (int)Math.Floor((item.XCenter - sheet.SlotX0) / sheet.SlotW);
                    if (slot >= 0 && slot < sheet.SlotCount) used.Add(slot);
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
                if (signTop < 0) signTop = sheet.TableBottomY;

                float slotX0 = sheet.SlotX0 + slot * sheet.SlotW;
                foreach (TextItem item in sheet.Items)
                {
                    if (item.Y0 < signTop || item.Y0 > sheet.TableTopY) continue;
                    if (item.XCenter < slotX0 || item.XCenter > slotX0 + sheet.SlotW) continue;
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

                //1. Uu tien: suy ra day hang chu ky theo TY LE CHIEU CAO trong mau phieu
                //   (hang chu ky cao gap SIGN_ROW_HEIGHT_RATIO lan mot hang thuong).
                //   Khong dua vao chu thap nhat, vi net chu ky la hinh ve nen khong do duoc bang chu.
                float rowPitch = GetRowPitch(sheet, signTop);
                if (rowPitch > 0f)
                    result = signTop - SIGN_ROW_HEIGHT_RATIO * rowPitch;
                else
                    result = TABLE_BOTTOM;

                //2. Khong duoc cat mat chu: neu con chu nam duoi ket qua thi ha day xuong
                float tableRight = sheet.SlotX0 + sheet.SlotCount * sheet.SlotW;
                float minY = signTop;
                foreach (TextItem item in sheet.Items)
                {
                    if (item.Y0 >= signTop) continue;
                    if (item.XCenter > tableRight) continue;   //panel ben phai
                    if (Normalize(item.Text).Length == 0) continue;
                    if (item.Y0 < minY) minY = item.Y0;
                }
                float lowestTextBottom = minY - CELL_PADDING_BOTTOM;
                if (minY < signTop && lowestTextBottom < result) result = lowestTextBottom;

                //3. Chan bien
                if (result < 0f) result = 0f;
                if (result > signTop - 1f) result = signTop - 1f;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Buoc hang thuong cua bang: lay trung vi khoang cach giua cac hang lien tiep phia tren
        /// hang chu ky. Trung vi de khong bi anh huong boi vai hang cao gap doi.
        /// </summary>
        private static float GetRowPitch(SheetInfo sheet, float signTop)
        {
            try
            {
                List<float> rowYs = sheet.RowList.Where(o => o.Y > signTop)
                                                 .Select(o => o.Y)
                                                 .OrderByDescending(o => o).ToList();
                if (rowYs.Count < 3) return 0f;

                List<float> gaps = new List<float>();
                for (int index = 1; index < rowYs.Count; index++)
                {
                    float gap = rowYs[index - 1] - rowYs[index];
                    if (gap > 1f && gap < 60f) gaps.Add(gap);
                }
                if (gaps.Count == 0) return 0f;

                gaps = gaps.OrderBy(o => o).ToList();
                return gaps[gaps.Count / 2];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0f;
            }
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

                TextItem first = sheet.Items.Where(o => Math.Abs(o.Y0 - rowY) <= 3f && o.XCenter >= sheet.SlotX0)
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
