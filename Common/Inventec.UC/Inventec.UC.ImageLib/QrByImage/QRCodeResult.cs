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
