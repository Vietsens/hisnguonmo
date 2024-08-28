using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FileConvert
{
    public class Get
    {
        /// 

        /// Validating MIMe type of allowed files from its binary content
        ///
        /// byte array content of the file
        /// is file is a valid mime type return true else return false
        public static string GetFileType(byte[] fileByteContent)
        {
            string ext = "";
            string mimetypeOfFile = string.Empty;

            byte[] buffer = new byte[256];
            using (MemoryStream fs = new MemoryStream(fileByteContent))
            {
                if (fs.Length >= 256)
                    fs.Read(buffer, 0, 256);
                else
                    fs.Read(buffer, 0, (int)fs.Length);
            }
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                mimetypeOfFile = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
            }
            catch (Exception e)
            {

            }

            if (!string.IsNullOrEmpty(mimetypeOfFile))
            {
                switch (mimetypeOfFile.ToLower())
                {
                    case "application/msword": // for .doc  estension
                        ext = ".doc";
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": // for .docx  estension
                        ext = ".docx";
                        break;
                    case "application/vnd.ms-excel": // for .xls  estension
                        ext = ".xls";
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": // for  .xlsx estension
                        ext = ".xlsx";
                        break;
                    case "application/vnd.ms-powerpoint":// for .ppt estension
                        ext = ".ppt";
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":// for .pptx estension
                        ext = ".pptx";
                        break;
                    case "image/jpeg"://jpeg and jpg both
                        ext = ".jpg";
                        break;
                    case "image/pjpeg"://jpeg and jpg both
                        ext = ".jpeg";
                        break;
                    case "image/png":// for .png estension
                        ext = ".png";
                        break;
                    case "image/x-png":// for .png estension
                        ext = ".png";
                        break;
                    case "image/gif":// for .gif estension
                        ext = ".gif";
                        break;
                }
            }

            return ext;

        }

        public static string GetFileType(MemoryStream fileStreamContent)
        {
            string ext = "";
            string mimetypeOfFile = string.Empty;

            byte[] buffer = new byte[256];

            if (fileStreamContent.Length >= 256)
                fileStreamContent.Read(buffer, 0, 256);
            else
                fileStreamContent.Read(buffer, 0, (int)fileStreamContent.Length);

            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                mimetypeOfFile = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
            }
            catch (Exception e)
            {

            }

            if (!string.IsNullOrEmpty(mimetypeOfFile))
            {
                switch (mimetypeOfFile.ToLower())
                {
                    case "application/msword": // for .doc  estension
                        ext = ".doc";
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": // for .docx  estension
                        ext = ".docx";
                        break;
                    case "application/vnd.ms-excel": // for .xls  estension
                        ext = ".xls";
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": // for  .xlsx estension
                        ext = ".xlsx";
                        break;
                    case "application/vnd.ms-powerpoint":// for .ppt estension
                        ext = ".ppt";
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":// for .pptx estension
                        ext = ".pptx";
                        break;
                    case "image/jpeg"://jpeg and jpg both
                        ext = ".jpg";
                        break;
                    case "image/pjpeg"://jpeg and jpg both
                        ext = ".jpeg";
                        break;
                    case "image/png":// for .png estension
                        ext = ".png";
                        break;
                    case "image/x-png":// for .png estension
                        ext = ".png";
                        break;
                    case "image/gif":// for .gif estension
                        ext = ".gif";
                        break;
                }
            }

            return ext;

        }

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        public static string GetMimeFromFile(byte[] byteArray)
        {

            byte[] buffer = new byte[256];
            using (MemoryStream fs = new MemoryStream(byteArray))
            {
                if (fs.Length >= 256)
                    fs.Read(buffer, 0, 256);
                else
                    fs.Read(buffer, 0, (int)fs.Length);
            }
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
