using System;
using System.Drawing;
using System.IO;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Icon 16x16 nhung san (base64 PNG) — tranh phai them vao .resx.
    /// Eye: nut "Xem du lieu se day" tren luoi. Folder: nut "Thiet lap duong dan xuat xml".
    /// </summary>
    internal static class KskSyncIcons
    {
        private const string EYE_B64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAQklEQVR4nGNgGAWM2IJAN2rBf2zil5clYFWPohGXZoLyulgkcGnAENPFo5mQIUwM1AK6lHiBkkBkxKWQrGhkGJkAANEuN0mFcUKcAAAAAElFTkSuQmCC";
        private const string FOLDER_B64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAARUlEQVR4nGNgGAWMoCCYlif3H1tQZE16BJbHB1hgDD8bORSJTUceUe4CQgDkQhZcLiAEYC5kYqAQMI0awACPBWLjffABANwhEHEQNhqIAAAAAElFTkSuQmCC";

        internal static Image Eye() { return FromB64(EYE_B64); }
        internal static Image Folder() { return FromB64(FOLDER_B64); }

        private static Image FromB64(string b64)
        {
            try { return Image.FromStream(new MemoryStream(Convert.FromBase64String(b64))); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }
    }
}
