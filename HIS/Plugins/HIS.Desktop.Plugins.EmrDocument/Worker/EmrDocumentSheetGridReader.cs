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
using iTextSharp.text.io;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace HIS.Desktop.Plugins.EmrDocument.Worker
{
    /// <summary>
    /// Luoi o cua mot phieu, doc tu chinh cac NET VE cua bang trong file PDF.
    /// Nho vay khong phai khai bao hay do tay toa do cho tung mau phieu.
    /// </summary>
    internal class SheetGrid
    {
        /// <summary>Bien cac cot du lieu, tang dan. So cot = so bien - 1.</summary>
        internal List<float> ColumnX = new List<float>();

        /// <summary>Bien cac hang, giam dan (tu tren xuong duoi).</summary>
        internal List<float> RowY = new List<float>();

        internal float LabelX0, LabelX1;
        internal float TableTop, TableBottom;
        internal float SignTop, SignBottom;

        internal int ColumnCount { get { return Math.Max(0, this.ColumnX.Count - 1); } }
        internal float ColumnWidth { get { return this.ColumnX.Count > 1 ? this.ColumnX[1] - this.ColumnX[0] : 0f; } }
    }

    /// <summary>
    /// Doc net ve cua bang trong file PDF de dung luoi o.
    /// Dung PdfContentParser cua iTextSharp (ban 5.5.3 khong co API doc net cap cao),
    /// tu doc cac lenh ve: re (hinh chu nhat), m/l (duong thang), cm (ma tran), q/Q (ngan xep).
    /// </summary>
    internal class EmrDocumentSheetGridReader
    {
        #region Declare

        /// <summary>Do day toi da de coi mot hinh chu nhat la net ke.</summary>
        private const float LINE_MAX_THICKNESS = 2.5f;

        /// <summary>Chieu dai toi thieu de coi la net ke cua bang.</summary>
        private const float LINE_MIN_LENGTH = 8f;

        /// <summary>Sai so gop cac net cung mot vi tri.</summary>
        private const float CLUSTER_TOLERANCE = 1.5f;

        /// <summary>Sai so khi kiem tra cac cot cach nhau deu nhau.</summary>
        private const float PITCH_TOLERANCE = 1.2f;

        /// <summary>So net toi thieu tai mot vi tri de coi la net ke cua bang (bo net le).</summary>
        private const int MIN_LINE_AT_POSITION = 3;

        /// <summary>So cot du lieu toi thieu de coi la bang nhan dinh.</summary>
        private const int MIN_DATA_COLUMN = 2;

        #endregion

        private class Segment
        {
            internal float X0, Y0, X1, Y1;
            internal bool IsVertical { get { return Math.Abs(this.X1 - this.X0) <= LINE_MAX_THICKNESS && Math.Abs(this.Y1 - this.Y0) > LINE_MIN_LENGTH; } }
            internal bool IsHorizontal { get { return Math.Abs(this.Y1 - this.Y0) <= LINE_MAX_THICKNESS && Math.Abs(this.X1 - this.X0) > LINE_MIN_LENGTH; } }
            internal float XCenter { get { return (this.X0 + this.X1) / 2f; } }
            internal float YCenter { get { return (this.Y0 + this.Y1) / 2f; } }
        }

        private class Cluster
        {
            internal float Value;
            internal int Count;
        }

        /// <summary>Ma tran bien doi: x' = a*x + c*y + e ; y' = b*x + d*y + f</summary>
        private class Matrix
        {
            internal float A = 1f, B = 0f, C = 0f, D = 1f, E = 0f, F = 0f;

            internal Matrix Clone()
            {
                return new Matrix() { A = this.A, B = this.B, C = this.C, D = this.D, E = this.E, F = this.F };
            }

            /// <summary>Nhan ma tran moi vao truoc ma tran hien tai (dung nhu toan tu cm cua PDF).</summary>
            internal void Concat(float a, float b, float c, float d, float e, float f)
            {
                float na = a * this.A + b * this.C;
                float nb = a * this.B + b * this.D;
                float nc = c * this.A + d * this.C;
                float nd = c * this.B + d * this.D;
                float ne = e * this.A + f * this.C + this.E;
                float nf = e * this.B + f * this.D + this.F;
                this.A = na; this.B = nb; this.C = nc; this.D = nd; this.E = ne; this.F = nf;
            }

            internal float TransformX(float x, float y) { return this.A * x + this.C * y + this.E; }
            internal float TransformY(float x, float y) { return this.B * x + this.D * y + this.F; }
        }

        /// <summary>
        /// Doc luoi o cua trang dau tien. Tra ve null neu khong doc duoc net ve nao
        /// (vi du phieu la anh scan) - khi do ben goi tu quay ve cach do theo chu.
        /// </summary>
        internal static SheetGrid Read(string pdfFilePath)
        {
            try
            {
                List<Segment> segments = ReadSegments(pdfFilePath);
                if (segments == null || segments.Count == 0) return null;

                List<Cluster> verticals = BuildClusters(segments.Where(o => o.IsVertical).Select(o => o.XCenter).ToList());
                List<Cluster> horizontals = BuildClusters(segments.Where(o => o.IsHorizontal).Select(o => o.YCenter).ToList());

                verticals = verticals.Where(o => o.Count >= MIN_LINE_AT_POSITION).OrderBy(o => o.Value).ToList();
                horizontals = horizontals.Where(o => o.Count >= MIN_LINE_AT_POSITION).OrderByDescending(o => o.Value).ToList();

                if (verticals.Count < MIN_DATA_COLUMN + 1 || horizontals.Count < 5) return null;

                List<float> columnX = FindEvenColumnBand(verticals.Select(o => o.Value).ToList());
                if (columnX == null || columnX.Count < MIN_DATA_COLUMN + 1) return null;

                SheetGrid grid = new SheetGrid();
                grid.ColumnX = columnX;
                grid.RowY = horizontals.Select(o => o.Value).ToList();
                grid.TableTop = grid.RowY[0];
                grid.TableBottom = grid.RowY[grid.RowY.Count - 1];
                grid.SignTop = grid.RowY[grid.RowY.Count - 2];
                grid.SignBottom = grid.TableBottom;
                grid.LabelX0 = verticals[0].Value;
                grid.LabelX1 = columnX[0];

                if (grid.TableTop - grid.TableBottom < 50f) return null;

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MergeColumns grid: cols={0} colX0={1} colW={2} tableTop={3} tableBottom={4} signTop={5} rows={6} labelX=[{7}..{8}] segments={9}",
                    grid.ColumnCount, grid.ColumnX[0], grid.ColumnWidth, grid.TableTop, grid.TableBottom,
                    grid.SignTop, grid.RowY.Count, grid.LabelX0, grid.LabelX1, segments.Count));

                return grid;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Doc toan bo net ve (duong thang va hinh chu nhat mong) cua trang 1.</summary>
        private static List<Segment> ReadSegments(string pdfFilePath)
        {
            List<Segment> result = new List<Segment>();
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(pdfFilePath);
                byte[] content = ContentByteUtils.GetContentBytesForPage(reader, 1);
                if (content == null || content.Length == 0) return result;

                PdfContentParser parser = new PdfContentParser(new PRTokeniser(
                    new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(content))));

                Matrix current = new Matrix();
                Stack<Matrix> stack = new Stack<Matrix>();
                float curX = 0f, curY = 0f;

                List<PdfObject> operands = new List<PdfObject>();
                while (parser.Parse(operands).Count > 0)
                {
                    PdfLiteral op = operands[operands.Count - 1] as PdfLiteral;
                    if (op == null) continue;
                    string name = op.ToString();

                    if (name == "q")
                    {
                        stack.Push(current.Clone());
                    }
                    else if (name == "Q")
                    {
                        if (stack.Count > 0) current = stack.Pop();
                    }
                    else if (name == "cm" && operands.Count >= 7)
                    {
                        current.Concat(GetFloat(operands[0]), GetFloat(operands[1]), GetFloat(operands[2]),
                                       GetFloat(operands[3]), GetFloat(operands[4]), GetFloat(operands[5]));
                    }
                    else if (name == "re" && operands.Count >= 5)
                    {
                        float x = GetFloat(operands[0]), y = GetFloat(operands[1]);
                        float w = GetFloat(operands[2]), h = GetFloat(operands[3]);

                        float ax = current.TransformX(x, y), ay = current.TransformY(x, y);
                        float bx = current.TransformX(x + w, y + h), by = current.TransformY(x + w, y + h);

                        Segment seg = new Segment()
                        {
                            X0 = Math.Min(ax, bx), Y0 = Math.Min(ay, by),
                            X1 = Math.Max(ax, bx), Y1 = Math.Max(ay, by)
                        };
                        if (seg.IsVertical || seg.IsHorizontal) result.Add(seg);

                        curX = ax; curY = ay;
                    }
                    else if (name == "m" && operands.Count >= 3)
                    {
                        float x = GetFloat(operands[0]), y = GetFloat(operands[1]);
                        curX = current.TransformX(x, y);
                        curY = current.TransformY(x, y);
                    }
                    else if (name == "l" && operands.Count >= 3)
                    {
                        float x = GetFloat(operands[0]), y = GetFloat(operands[1]);
                        float nx = current.TransformX(x, y), ny = current.TransformY(x, y);

                        Segment seg = new Segment()
                        {
                            X0 = Math.Min(curX, nx), Y0 = Math.Min(curY, ny),
                            X1 = Math.Max(curX, nx), Y1 = Math.Max(curY, ny)
                        };
                        if (seg.IsVertical || seg.IsHorizontal) result.Add(seg);

                        curX = nx; curY = ny;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                if (reader != null)
                {
                    try { reader.Close(); }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }
            }
            return result;
        }

        private static float GetFloat(PdfObject obj)
        {
            PdfNumber number = obj as PdfNumber;
            return number != null ? (float)number.DoubleValue : 0f;
        }

        /// <summary>Gop cac gia tri gan nhau thanh mot vi tri, kem so lan xuat hien.</summary>
        private static List<Cluster> BuildClusters(List<float> values)
        {
            List<Cluster> result = new List<Cluster>();
            try
            {
                foreach (float value in values.OrderBy(o => o))
                {
                    Cluster last = result.Count > 0 ? result[result.Count - 1] : null;
                    if (last != null && Math.Abs(last.Value - value) <= CLUSTER_TOLERANCE)
                    {
                        //Trung binh dong de vi tri on dinh hon
                        last.Value = (last.Value * last.Count + value) / (last.Count + 1);
                        last.Count = last.Count + 1;
                    }
                    else
                    {
                        result.Add(new Cluster() { Value = value, Count = 1 });
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
        /// Tim day bien cot CACH NHAU DEU NHAU dai nhat - do chinh la vung cac cot nhan dinh.
        /// Cac net khac (mep bang, vach chia cot nhan, mep panel) khong deu nen bi loai.
        /// </summary>
        private static List<float> FindEvenColumnBand(List<float> xs)
        {
            List<float> best = null;
            try
            {
                for (int start = 0; start < xs.Count - MIN_DATA_COLUMN; start++)
                {
                    for (int next = start + 1; next < xs.Count; next++)
                    {
                        float pitch = xs[next] - xs[start];
                        if (pitch < 10f) continue;

                        List<float> run = new List<float>() { xs[start], xs[next] };
                        float expected = xs[next] + pitch;
                        for (int probe = next + 1; probe < xs.Count; probe++)
                        {
                            if (Math.Abs(xs[probe] - expected) <= PITCH_TOLERANCE)
                            {
                                run.Add(xs[probe]);
                                expected = xs[probe] + pitch;
                            }
                        }

                        if (run.Count >= MIN_DATA_COLUMN + 1 && (best == null || run.Count > best.Count))
                            best = run;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return best;
        }
    }
}
