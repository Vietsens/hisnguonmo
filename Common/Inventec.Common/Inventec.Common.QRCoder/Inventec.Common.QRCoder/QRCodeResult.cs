using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.QRCoder
{
    public class QRCodeResult
    {
        /// <summary>
        /// QR Code Data array
        /// </summary>
        public byte[] DataArray;

        /// <summary>
        /// ECI Assignment Value
        /// </summary>
        public int ECIAssignValue;

        /// <summary>
        /// QR Code matrix version
        /// </summary>
        public int QRCodeVersion;

        /// <summary>
        /// QR Code matrix dimension in bits
        /// </summary>
        public int QRCodeDimension;

        /// <summary>
        /// QR Code error correction code (L, M, Q, H)
        /// </summary>
        public ErrorCorrection ErrorCorrection;

        public QRCodeResult
                (
                byte[] DataArray
                )
        {
            this.DataArray = DataArray;
            return;
        }
        public static string ConvertResultToDisplayString
                (
                QRCodeResult[] DataByteArray
                )
        {
            // no QR code
            if (DataByteArray == null) return string.Empty;

            // image has one QR code
            if (DataByteArray.Length == 1) return SingleQRCodeResult(QRDecoder.ByteArrayToStr(DataByteArray[0].DataArray));

            return null;
        }
        private static string SingleQRCodeResult
            (
            string Result
            )
        {
            int Index;
            for (Index = 0; Index < Result.Length && (Result[Index] >= ' ' && Result[Index] <= '~' || Result[Index] >= 160); Index++) ;
            if (Index == Result.Length) return Result;

            StringBuilder Display = new StringBuilder(Result.Substring(0, Index));
            for (; Index < Result.Length; Index++)
            {
                char OneChar = Result[Index];
                if (OneChar >= ' ' && OneChar <= '~' || OneChar >= 160)
                {
                    Display.Append(OneChar);
                    continue;
                }

                if (OneChar == '\r')
                {
                    Display.Append("\r\n");
                    if (Index + 1 < Result.Length && Result[Index + 1] == '\n') Index++;
                    continue;
                }

                if (OneChar == '\n')
                {
                    Display.Append("\r\n");
                    continue;
                }

                Display.Append('¿');
            }
            return Display.ToString();
        }
    }
    internal class QRCodeFinder
    {
        // horizontal scan
        internal int Row;
        internal int Col1;
        internal int Col2;
        internal double HModule;

        // vertical scan
        internal int Col;
        internal int Row1;
        internal int Row2;
        internal double VModule;

        internal double Distance;
        internal double ModuleSize;

        /// <summary>
        /// Constructor during horizontal scan
        /// </summary>
        internal QRCodeFinder
                (
                int Row,
                int Col1,
                int Col2,
                double HModule
                )
        {
            this.Row = Row;
            this.Col1 = Col1;
            this.Col2 = Col2;
            this.HModule = HModule;
            Distance = double.MaxValue;
            return;
        }

        /// <summary>
        /// Match during vertical scan
        /// </summary>
        internal void Match
                (
                int Col,
                int Row1,
                int Row2,
                double VModule
                )
        {
            // test if horizontal and vertical are not related
            if (Col < Col1 || Col >= Col2 || Row < Row1 || Row >= Row2) return;

            // Module sizes must be about the same
            if (Math.Min(HModule, VModule) < Math.Max(HModule, VModule) * QRDecoder.MODULE_SIZE_DEVIATION) return;

            // calculate distance
            double DeltaX = Col - 0.5 * (Col1 + Col2);
            double DeltaY = Row - 0.5 * (Row1 + Row2);
            double Delta = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

            // distance between two points must be less than 2 pixels
            if (Delta > QRDecoder.HOR_VERT_SCAN_MAX_DISTANCE) return;

            // new result is better than last result
            if (Delta < Distance)
            {
                this.Col = Col;
                this.Row1 = Row1;
                this.Row2 = Row2;
                this.VModule = VModule;
                ModuleSize = 0.5 * (HModule + VModule);
                Distance = Delta;
            }
            return;
        }

        /// <summary>
        /// Horizontal and vertical scans overlap
        /// </summary>
        internal bool Overlap
                (
                QRCodeFinder Other
                )
        {
            return Other.Col1 < Col2 && Other.Col2 >= Col1 && Other.Row1 < Row2 && Other.Row2 >= Row1;
        }

        /// <summary>
        /// Finder to string
        /// </summary>
        public override string ToString()
        {
            if (Distance == double.MaxValue)
            {
                return string.Format("Finder: Row: {0}, Col1: {1}, Col2: {2}, HModule: {3:0.00}", Row, Col1, Col2, HModule);
            }

            return string.Format("Finder: Row: {0}, Col: {1}, Module: {2:0.00}, Distance: {3:0.00}", Row, Col, ModuleSize, Distance);
        }
    }
    internal class QRCodeCorner
    {
        internal QRCodeFinder TopLeftFinder;
        internal QRCodeFinder TopRightFinder;
        internal QRCodeFinder BottomLeftFinder;

        internal double TopLineDeltaX;
        internal double TopLineDeltaY;
        internal double TopLineLength;
        internal double LeftLineDeltaX;
        internal double LeftLineDeltaY;
        internal double LeftLineLength;

        private QRCodeCorner
                (
                QRCodeFinder TopLeftFinder,
                QRCodeFinder TopRightFinder,
                QRCodeFinder BottomLeftFinder
                )
        {
            // save three finders
            this.TopLeftFinder = TopLeftFinder;
            this.TopRightFinder = TopRightFinder;
            this.BottomLeftFinder = BottomLeftFinder;

            // top line slope
            TopLineDeltaX = TopRightFinder.Col - TopLeftFinder.Col;
            TopLineDeltaY = TopRightFinder.Row - TopLeftFinder.Row;

            // top line length
            TopLineLength = Math.Sqrt(TopLineDeltaX * TopLineDeltaX + TopLineDeltaY * TopLineDeltaY);

            // left line slope
            LeftLineDeltaX = BottomLeftFinder.Col - TopLeftFinder.Col;
            LeftLineDeltaY = BottomLeftFinder.Row - TopLeftFinder.Row;

            // left line length
            LeftLineLength = Math.Sqrt(LeftLineDeltaX * LeftLineDeltaX + LeftLineDeltaY * LeftLineDeltaY);
            return;
        }

        internal static QRCodeCorner CreateCorner
                (
                QRCodeFinder TopLeftFinder,
                QRCodeFinder TopRightFinder,
                QRCodeFinder BottomLeftFinder
                )
        {
            for (int Index = 0; Index < 3; Index++)
            {
                if (Index != 0)
                {
                    QRCodeFinder Temp = TopLeftFinder;
                    TopLeftFinder = TopRightFinder;
                    TopRightFinder = BottomLeftFinder;
                    BottomLeftFinder = Temp;
                }

                // top line slope
                double TopLineDeltaX = TopRightFinder.Col - TopLeftFinder.Col;
                double TopLineDeltaY = TopRightFinder.Row - TopLeftFinder.Row;

                // left line slope
                double LeftLineDeltaX = BottomLeftFinder.Col - TopLeftFinder.Col;
                double LeftLineDeltaY = BottomLeftFinder.Row - TopLeftFinder.Row;

                // top line length
                double TopLineLength = Math.Sqrt(TopLineDeltaX * TopLineDeltaX + TopLineDeltaY * TopLineDeltaY);

                double LeftLineLength = Math.Sqrt(LeftLineDeltaX * LeftLineDeltaX + LeftLineDeltaY * LeftLineDeltaY);

                if (Math.Min(TopLineLength, LeftLineLength) < QRDecoder.CORNER_SIDE_LENGTH_DEV * Math.Max(TopLineLength, LeftLineLength))
                    continue;

                double TopLineSin = TopLineDeltaY / TopLineLength;
                double TopLineCos = TopLineDeltaX / TopLineLength;

                double NewLeftX = TopLineCos * LeftLineDeltaX + TopLineSin * LeftLineDeltaY;
                double NewLeftY = -TopLineSin * LeftLineDeltaX + TopLineCos * LeftLineDeltaY;

                if (Math.Abs(NewLeftX / LeftLineLength) > QRDecoder.CORNER_RIGHT_ANGLE_DEV)
                    continue;

                if (NewLeftY < 0)
                {
                    QRCodeFinder TempFinder = TopRightFinder;
                    TopRightFinder = BottomLeftFinder;
                    BottomLeftFinder = TempFinder;
                }

                return new QRCodeCorner(TopLeftFinder, TopRightFinder, BottomLeftFinder);
            }
            return null;
        }

        internal int InitialVersionNumber()
        {
            double TopModules = 7;

            if (Math.Abs(TopLineDeltaX) >= Math.Abs(TopLineDeltaY))
            {
                TopModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaX) * 0.5 * (TopLeftFinder.HModule + TopRightFinder.HModule));
            }

            else
            {
                TopModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaY) * 0.5 * (TopLeftFinder.VModule + TopRightFinder.VModule));
            }

            double LeftModules = 7;

            if (Math.Abs(LeftLineDeltaY) >= Math.Abs(LeftLineDeltaX))
            {
                LeftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaY) * 0.5 * (TopLeftFinder.VModule + BottomLeftFinder.VModule));
            }

            else
            {
                LeftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaX) * 0.5 * (TopLeftFinder.HModule + BottomLeftFinder.HModule));
            }

            int Version = ((int)Math.Round(0.5 * (TopModules + LeftModules)) - 15) / 4;

            if (Version < 1 || Version > 40)
                throw new ApplicationException("Corner is not valid (version number must be 1 to 40)");

            return Version;
        }
    }
}
