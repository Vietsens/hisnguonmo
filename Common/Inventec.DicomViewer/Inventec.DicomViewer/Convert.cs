using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.DicomViewer
{
    public class Convert
    {
        public static MemoryStream ConvertFileToStream(string file)
        {
            try
            {
                if (System.IO.File.Exists(file))
                {
                    var memo = new MemoryStream();
                    using (FileStream stream = File.Open(file, FileMode.Open))
                    {
                        stream.CopyTo(memo);
                        memo.Position = 0;
                        return memo;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        public static Stream DicomFileToImageStream(string _dicomFilePath)
        {
            var img = new Dicom.Imaging.DicomImage(_dicomFilePath);
            var stream = ConvertToStream(img.RenderImage(), ImageFormat.Jpeg);
            stream.Position = 0;
            return stream;

        }

        static Stream ConvertToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static MemoryStream ConvertAnotherImageToJpgImageStream(string filename)
        {
            Image myImage = Image.FromFile(filename);
            return ConvertAnotherImageToJpgImageStream(filename, myImage.Width, myImage.Height);
        }

        public static MemoryStream ConvertAnotherImageToJpgImageStream(string filename, int width, int heoght)
        {
            MemoryStream result = new MemoryStream();
            Image myImage = Image.FromFile(filename);

            // Assumes myImage is the PNG you are converting
            using (var b = new Bitmap(myImage.Width, myImage.Height))
            {
                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(myImage, 0, 0);
                }

                // Now save b as a JPEG like you normally would
                myImage.Save(result, System.Drawing.Imaging.ImageFormat.Jpeg);
                return result;
            }
        }
    }
}
