﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inventec.Common.QRCoder
{
  
    // ReSharper disable once InconsistentNaming
    public class BitmapByteQRCode : AbstractQRCode<byte[]>, IDisposable
    {
        public BitmapByteQRCode(QRCodeData data) : base(data) { }


        public override byte[] GetGraphic(int pixelsPerModule)
        {
            var sideLength = this.QrCodeData.ModuleMatrix.Count * pixelsPerModule;
           
            var moduleDark = new byte[] {0x00, 0x00, 0x00};
            var moduleLight = new byte[] { 0xFF, 0xFF, 0xFF};

            List<byte> bmp = new List<byte>();

            //header
            bmp.AddRange(new byte[] { 0x42, 0x4D, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00 });
            
            //width
            bmp.AddRange(IntTo4Byte(sideLength));
            //height
            bmp.AddRange(IntTo4Byte(sideLength));
            
            //header end
            bmp.AddRange(new byte[] { 0x01, 0x00, 0x18, 0x00 });
           
            //draw qr code
            for (var x = sideLength-1; x >= 0; x = x - pixelsPerModule)
            {
                for (int pm = 0; pm < pixelsPerModule; pm++)
                {
                    for (var y = 0; y < sideLength; y = y + pixelsPerModule)
                    {
                        var module =
                            this.QrCodeData.ModuleMatrix[(x + pixelsPerModule)/pixelsPerModule - 1][(y + pixelsPerModule)/pixelsPerModule - 1];
                        for (int i = 0; i < pixelsPerModule; i++)
                        {
                            bmp.AddRange(module ? moduleDark : moduleLight);
                        }
                    }
                    if (sideLength%4 != 0)
                    {
                        for (int i = 0; i < sideLength%4; i++)
                        {
                            bmp.Add(0x00);
                        }
                    }
                }
            }

            //finalize with terminator
            bmp.AddRange(new byte[] { 0x00, 0x00 });

            return bmp.ToArray();
        }
        private byte[] IntTo4Byte(int inp)
        {
            byte[] bytes = new byte[2];
            unchecked
            {
                bytes[1] = (byte)(inp >> 8);
                bytes[0] = (byte)(inp);
            }
            return bytes;
        }

        public void Dispose()
        {
            this.QrCodeData = null;
        }
    }
}
