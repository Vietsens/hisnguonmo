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
    /// <summary>
    /// QR code finder class
    /// </summary>
    class QRCodeFinder
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
}
