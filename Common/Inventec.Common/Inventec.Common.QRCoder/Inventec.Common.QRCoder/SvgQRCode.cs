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
using System.Drawing;
using System.Text;

namespace Inventec.Common.QRCoder
{
    public class SvgQRCode : AbstractQRCode<string>, IDisposable
    {
        public SvgQRCode(QRCodeData data) : base(data) { }


        public override string GetGraphic(int pixelsPerModule)
        {
            var viewBox = new Size(pixelsPerModule*this.QrCodeData.ModuleMatrix.Count, pixelsPerModule * this.QrCodeData.ModuleMatrix.Count);
            return this.GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox)
        {
            return this.GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor)
        {
            return this.GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())));
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            return this.GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())), drawQuietZones);
        }

        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true)
        {
            var svgFile = new StringBuilder(@"<svg version=""1.1"" baseProfile=""full"" width="""+viewBox.Width+ @""" height="""+viewBox.Height+ @""" xmlns=""http://www.w3.org/2000/svg"">");
            var unitsPerModule = (int)Math.Floor(Convert.ToDouble(Math.Min(viewBox.Width, viewBox.Height)) / this.QrCodeData.ModuleMatrix.Count);
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * unitsPerModule;
            var offset = drawQuietZones ? 0 : 4 * unitsPerModule;
            var drawableSize = size + offset;
            for (var x = 0; x < drawableSize; x = x + unitsPerModule)
            {
                for (var y = 0; y < drawableSize; y = y + unitsPerModule)
                {
                    var module = this.QrCodeData.ModuleMatrix[(y + unitsPerModule) / unitsPerModule - 1][(x + unitsPerModule) / unitsPerModule - 1];
                    svgFile.AppendLine(@"<rect x="""+(x - offset)+ @""" y=""" + (y - offset) + @""" width=""" + unitsPerModule + @""" height=""" + unitsPerModule + @""" fill=""" + (module ? darkColorHex : lightColorHex) + @""" />");
                }
            }
            svgFile.Append(@"</svg>");
            return svgFile.ToString();
        }

        public void Dispose()
        {
            this.QrCodeData = null;
        }
    }
}
