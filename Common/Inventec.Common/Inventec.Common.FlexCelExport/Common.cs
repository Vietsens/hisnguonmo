using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    public class Common
    {
        public static MemoryStream GetStreamFromFile(string pathFile)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                        ms.Position = 0;

                        return ms;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public static byte[] ConverterImageToArray(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }
    }
}
