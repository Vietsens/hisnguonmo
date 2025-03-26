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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Inventec.Common.FlexCellExport
{
    public class ProcessWatermarkTag
    {
        public bool ProcessData(Store store, List<string> dicData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (dicData == null || dicData.Count == 0) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                Font font = new System.Drawing.Font("arial", 40);

                FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(true);
                xls.Open(store.TemplateStream);
               

                foreach (var item in dicData)
                {
                    System.Drawing.Image imgWtrmrk = DrawText(item, font, System.Drawing.Color.LightCoral, System.Drawing.Color.White, xls.PrintPaperDimensions.Height, xls.PrintPaperDimensions.Width);
                    FlexCel.Core.THeaderAndFooter  theaderAndFooter = new FlexCel.Core.THeaderAndFooter();
                    //theaderAndFooter.ScaleWithDoc
                    xls.SetPageHeaderAndFooter(theaderAndFooter);
                }

                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private static System.Drawing.Image DrawText(string text, System.Drawing.Font font, Color textColor, Color backColor, double height, double width)
        {

            //create a bitmap image with specified width and height

            Image img = new Bitmap((int)width, (int)height);

            Graphics drawing = Graphics.FromImage(img);

            //get the size of text

            SizeF textSize = drawing.MeasureString(text, font);

            //set rotation point

            drawing.TranslateTransform(((int)width - textSize.Width) / 2, ((int)height - textSize.Height) / 2);

            //rotate text

            drawing.RotateTransform(-45);

            //reset translate transform

            drawing.TranslateTransform(-((int)width - textSize.Width) / 2, -((int)height - textSize.Height) / 2);

            //paint the background

            drawing.Clear(backColor);

            //create a brush for the text

            Brush textBrush = new SolidBrush(textColor);

            //draw text on the image at center position

            drawing.DrawString(text, font, textBrush, ((int)width - textSize.Width) / 2, ((int)height - textSize.Height) / 2);

            drawing.Save();

            return img;

        }

    }
}
