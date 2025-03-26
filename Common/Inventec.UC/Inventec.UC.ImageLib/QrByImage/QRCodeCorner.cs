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

namespace Inventec.UC.ImageLib.QrByImage
{
    class QRCodeCorner
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

        /////////////////////////////////////////////////////////////////////
        // QR corner constructor
        /////////////////////////////////////////////////////////////////////

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

        /////////////////////////////////////////////////////////////////////
        // Test QR corner for validity
        /////////////////////////////////////////////////////////////////////

        internal static QRCodeCorner CreateCorner
                (
                QRCodeFinder TopLeftFinder,
                QRCodeFinder TopRightFinder,
                QRCodeFinder BottomLeftFinder
                )
        {
            // try all three possible permutation of three finders
            for (int Index = 0; Index < 3; Index++)
            {
                // TestCorner runs three times to test all posibilities
                // rotate top left, top right and bottom left
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

                // left line length
                double LeftLineLength = Math.Sqrt(LeftLineDeltaX * LeftLineDeltaX + LeftLineDeltaY * LeftLineDeltaY);

                // the short side must be at least 80% of the long side
                if (Math.Min(TopLineLength, LeftLineLength) < QRDecoder.CORNER_SIDE_LENGTH_DEV * Math.Max(TopLineLength, LeftLineLength))
                    continue;

                // top line vector
                double TopLineSin = TopLineDeltaY / TopLineLength;
                double TopLineCos = TopLineDeltaX / TopLineLength;

                // rotate lines such that top line is parallel to x axis
                // left line after rotation
                double NewLeftX = TopLineCos * LeftLineDeltaX + TopLineSin * LeftLineDeltaY;
                double NewLeftY = -TopLineSin * LeftLineDeltaX + TopLineCos * LeftLineDeltaY;

                // new left line X should be zero (or between +/- 4 deg)
                if (Math.Abs(NewLeftX / LeftLineLength) > QRDecoder.CORNER_RIGHT_ANGLE_DEV)
                    continue;

                // swap top line with left line
                if (NewLeftY < 0)
                {
                    // swap top left with bottom right
                    QRCodeFinder TempFinder = TopRightFinder;
                    TopRightFinder = BottomLeftFinder;
                    BottomLeftFinder = TempFinder;
                }

                return new QRCodeCorner(TopLeftFinder, TopRightFinder, BottomLeftFinder);
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////
        // Test QR corner for validity
        /////////////////////////////////////////////////////////////////////

        internal int InitialVersionNumber()
        {
            // version number based on top line
            double TopModules = 7;

            // top line is mostly horizontal
            if (Math.Abs(TopLineDeltaX) >= Math.Abs(TopLineDeltaY))
            {
                TopModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaX) * 0.5 * (TopLeftFinder.HModule + TopRightFinder.HModule));
            }

            // top line is mostly vertical
            else
            {
                TopModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaY) * 0.5 * (TopLeftFinder.VModule + TopRightFinder.VModule));
            }

            // version number based on left line
            double LeftModules = 7;

            // Left line is mostly vertical
            if (Math.Abs(LeftLineDeltaY) >= Math.Abs(LeftLineDeltaX))
            {
                LeftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaY) * 0.5 * (TopLeftFinder.VModule + BottomLeftFinder.VModule));
            }

            // left line is mostly horizontal
            else
            {
                LeftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaX) * 0.5 * (TopLeftFinder.HModule + BottomLeftFinder.HModule));
            }

            // version (there is rounding in the calculation)
            int Version = ((int)Math.Round(0.5 * (TopModules + LeftModules)) - 15) / 4;

            // not a valid corner
            if (Version < 1 || Version > 40)
                throw new ApplicationException("Corner is not valid (version number must be 1 to 40)");

            // exit with version number
            return Version;
        }
    }
}
