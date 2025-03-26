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
    }
}
