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
using ClearCanvas.Dicom;
using Inventec.DicomViewer.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.DicomViewer
{
    public class Generate
    {
        DicomFile df = null;
        DicomAttributeCollection _baseDataSet;
        bool rbColor;
        int rows;
        int columns;
        string outPutFile;
        string inputFileImage;
        SopADO sopADO;
        MemoryStream streamResult;

        public Generate(string _inputFileImage, string _outPutFile, SopADO _sopADO)
        {
            this.inputFileImage = _inputFileImage;
            this.outPutFile = _outPutFile;
            this.sopADO = _sopADO;
        }

        public Generate(string _inputFileImage, MemoryStream _streamResult, SopADO _sopADO)
        {
            this.inputFileImage = _inputFileImage;
            this.streamResult = _streamResult;
            this.sopADO = _sopADO;
        }

        public void Run()
        {
            try
            {
                Bitmap bm = LoadImage(inputFileImage);
                CreateBaseDataSet();
                df = ConvertImage(bm, (this.sopADO.InstanceNumber > 0 ? this.sopADO.InstanceNumber : 1));
                if (!String.IsNullOrEmpty(outPutFile))
                {
                    df.Save(outPutFile, DicomWriteOptions.Default);
                }
                else
                {
                    df.Save(streamResult, DicomWriteOptions.Default);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateBaseDataSet()
        {
            _baseDataSet = new DicomAttributeCollection();

            //Sop Common
            _baseDataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
            //_baseDataSet[DicomTags.SopClassUid].SetStringValue(SopClass.DigitalXRayImageStorageForPresentation.Uid);

            //Patient
            _baseDataSet[DicomTags.PatientId].SetStringValue(this.sopADO.PatientId);
            _baseDataSet[DicomTags.PatientsName].SetStringValue((this.sopADO.PatientsName ?? "").ToString());
            _baseDataSet[DicomTags.PatientsAddress].SetString(0, this.sopADO.PatientPosition);
            _baseDataSet[DicomTags.PatientsBirthDate].SetString(0, this.sopADO.PatientsBirthDate);
            _baseDataSet[DicomTags.PatientsSex].SetStringValue(this.sopADO.PatientsSex);

            //Study
            if (!String.IsNullOrEmpty(this.sopADO.StudyInstanceUid))
                _baseDataSet[DicomTags.StudyInstanceUid].SetStringValue(this.sopADO.StudyInstanceUid);
            else
                _baseDataSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            if (!String.IsNullOrEmpty(this.sopADO.StudyDate))
            {
                _baseDataSet[DicomTags.StudyDate].SetStringValue(this.sopADO.StudyDate);
                _baseDataSet[DicomTags.StudyTime].SetStringValue(this.sopADO.StudyTime);
            }
            else
            {
                _baseDataSet[DicomTags.StudyDate].SetDateTime(0, DateTime.Now);
                _baseDataSet[DicomTags.StudyTime].SetDateTime(0, DateTime.Now);
            }
            if (!String.IsNullOrEmpty(this.sopADO.AccessionNumber))
                _baseDataSet[DicomTags.AccessionNumber].SetStringValue(this.sopADO.AccessionNumber);
            else
                _baseDataSet[DicomTags.AccessionNumber].SetStringValue("ACCEEL");
            _baseDataSet[DicomTags.StudyDescription].SetStringValue(this.sopADO.StudyDescription);
            _baseDataSet[DicomTags.ReferringPhysiciansName].SetNullValue();
            _baseDataSet[DicomTags.StudyId].SetStringValue("STUDY");

            //Sop
            _baseDataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            _baseDataSet[DicomTags.WindowCenter].SetInt32(0, 32768);
            _baseDataSet[DicomTags.WindowWidth].SetInt32(0, 65536);
            _baseDataSet[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");

            //Series
            _baseDataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);           
            if (!String.IsNullOrEmpty(this.sopADO.Modality))
                _baseDataSet[DicomTags.Modality].SetStringValue(this.sopADO.Modality);
            else
                _baseDataSet[DicomTags.Modality].SetStringValue("HC");//Hard Copy
            if (this.sopADO.SeriesNumber > 0)
                _baseDataSet[DicomTags.SeriesNumber].SetStringValue(this.sopADO.SeriesNumber.ToString());
            else
                _baseDataSet[DicomTags.SeriesNumber].SetStringValue("1");

            //SC Equipment
            _baseDataSet[DicomTags.ConversionType].SetStringValue("WSD");

            //General Image
            _baseDataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY\MPR");
            _baseDataSet[DicomTags.PatientOrientation].SetNullValue();

            _baseDataSet[DicomTags.WindowWidth].SetStringValue("");
            _baseDataSet[DicomTags.WindowCenter].SetStringValue("");

            _baseDataSet[DicomTags.SpecificCharacterSet].SetString(0, "ISO_IR 100");
            //_baseDataSet[DicomTags.SoftwareVersions].SetString(0, dicomFile.ImplementationVersionName);
            _baseDataSet[DicomTags.StationName].SetStringValue("");
            _baseDataSet[DicomTags.Manufacturer].SetStringValue("");
            _baseDataSet[DicomTags.ManufacturersModelName].SetStringValue("");

            //Image Pixel
            if (rbColor)
            {
                _baseDataSet[DicomTags.SamplesPerPixel].SetInt32(0, 3);
                _baseDataSet[DicomTags.PhotometricInterpretation].SetStringValue("RGB");
                _baseDataSet[DicomTags.BitsAllocated].SetInt32(0, 8);
                _baseDataSet[DicomTags.BitsStored].SetInt32(0, 8);
                _baseDataSet[DicomTags.HighBit].SetInt32(0, 7);
                _baseDataSet[DicomTags.PixelRepresentation].SetInt32(0, 0);
                _baseDataSet[DicomTags.PlanarConfiguration].SetInt32(0, 0);
            }
            else
            {
                _baseDataSet[DicomTags.SamplesPerPixel].SetInt32(0, 1);
                _baseDataSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
                _baseDataSet[DicomTags.BitsAllocated].SetInt32(0, 8);
                _baseDataSet[DicomTags.BitsStored].SetInt32(0, 8);
                _baseDataSet[DicomTags.HighBit].SetInt32(0, 7);
                _baseDataSet[DicomTags.PixelRepresentation].SetInt32(0, 0);
                _baseDataSet[DicomTags.PlanarConfiguration].SetInt32(0, 0);
            }
        }

        private DicomFile ConvertImage(Bitmap image, int instanceNumber)
        {
            DicomUid sopInstanceUid = DicomUid.GenerateUid();

            string fileName = @"C:\test.dcm";// String.Format("{0}.dcm", sopInstanceUid.UID);
            //fileName = System.IO.Path.Combine(_tempFileDirectory, fileName);

            DicomFile dicomFile = new DicomFile(fileName, new DicomAttributeCollection(), _baseDataSet.Copy());

            //meta info
            dicomFile.MediaStorageSopInstanceUid = sopInstanceUid.UID;
            dicomFile.MediaStorageSopClassUid = SopClass.SecondaryCaptureImageStorageUid;

            //General Image
            dicomFile.DataSet[DicomTags.InstanceNumber].SetInt32(0, instanceNumber);

            DateTime now = DateTime.Now;
            DateTime time = DateTime.MinValue.Add(new TimeSpan(now.Hour, now.Minute, now.Second));

            //SC Image
            dicomFile.DataSet[DicomTags.DateOfSecondaryCapture].SetDateTime(0, now);
            dicomFile.DataSet[DicomTags.TimeOfSecondaryCapture].SetDateTime(0, time);

            //Sop Common
            dicomFile.DataSet[DicomTags.InstanceCreationDate].SetDateTime(0, now);
            dicomFile.DataSet[DicomTags.InstanceCreationTime].SetDateTime(0, time);
            dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid.UID);

            //int rows, columns;
            //Image Pixel
            if (rbColor)
            {
                dicomFile.DataSet[DicomTags.PixelData].Values = GetColorPixelData(image, out rows, out columns);
            }
            else
            {
                dicomFile.DataSet[DicomTags.PixelData].Values = GetMonochromePixelData(image, out rows, out columns);
            }

            //Image Pixel
            dicomFile.DataSet[DicomTags.Rows].SetInt32(0, rows);
            dicomFile.DataSet[DicomTags.Columns].SetInt32(0, columns);

            return dicomFile;
        }

        private byte[] GetMonochromePixelData(Bitmap image, out int rows, out int columns)
        {
            rows = image.Height;
            columns = image.Width;

            //At least one of rows or columns must be even.
            if (rows % 2 != 0 && columns % 2 != 0)
                --columns; //trim the last column.

            int size = rows * columns;
            //byte[] pixelData = MemoryManager.Allocate<byte>(size);
            byte[] pixelData = new byte[size];
            int i = 0;

            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; column++)
                {
                    pixelData[i++] = image.GetPixel(column, row).R;
                }
            }

            return pixelData;
        }

        private byte[] GetColorPixelData(Bitmap image, out int rows, out int columns)
        {
            rows = image.Height;
            columns = image.Width;

            //At least one of rows or columns must be even.
            if (rows % 2 != 0 && columns % 2 != 0)
                --columns; //trim the last column.

            BitmapData data = image.LockBits(new Rectangle(0, 0, columns, rows), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr bmpData = data.Scan0;

            try
            {
                int stride = columns * 3;
                int size = rows * stride;

                //byte[] pixelData = MemoryManager.Allocate<byte>(size);
                byte[] pixelData = new byte[size];

                for (int i = 0; i < rows; ++i)
                    Marshal.Copy(new IntPtr(bmpData.ToInt64() + i * data.Stride), pixelData, i * stride, stride);

                //swap BGR to RGB
                SwapRedBlue(pixelData);
                return pixelData;
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        private Bitmap LoadImage(string file)
        {
            Bitmap image = Image.FromFile(file, true) as Bitmap;
            if (image == null)
                throw new ArgumentException(String.Format("The specified file cannot be loaded as a bitmap {0}.", file));

            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                //Platform.Log(LogLevel.Info, "Attempting to convert non RBG image to RGB ({0}) before converting to Dicom.", file);

                Bitmap old = image;
                using (old)
                {
                    image = new Bitmap(old.Width, old.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawImage(old, 0, 0, old.Width, old.Height);
                    }
                }
            }

            return image;
        }

        private void SwapRedBlue(byte[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 3)
            {
                byte temp = pixels[i];
                pixels[i] = pixels[i + 2];
                pixels[i + 2] = temp;
            }
        }
    }
}
