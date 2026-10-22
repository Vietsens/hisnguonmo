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
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.ServiceExecute.Base
{
    /// <summary>
    /// Tự tích ảnh theo số key ảnh (IMAGE_DATA_n) của mẫu kết quả - cấu hình AppConfigKeys.IsAutoSelectImageByTemplate.
    /// maxImageKey: số n lớn nhất trong các key IMAGE_DATA_n của mẫu; null = chưa chọn mẫu; 0 = mẫu không có key ảnh.
    /// Chỉ giới hạn khi maxImageKey > 0.
    /// </summary>
    internal class ImageCheckProcess
    {
        /// <summary>
        /// Đếm trên nội dung mẫu vừa nạp, TRƯỚC khi ProcessDescriptionContent thay key bằng ảnh.
        /// Key có thể nằm trong khung (textbox) nên phải duyệt cả Shapes - Document.Text không chứa chữ trong khung.
        /// </summary>
        internal static int? GetMaxImageKey(DevExpress.XtraRichEdit.RichEditControl txtDescription)
        {
            if (txtDescription == null)
                return null;

            Document doc = txtDescription.Document;
            List<string> texts = new List<string>();
            texts.Add(doc.GetText(doc.Range));
            if (doc.Shapes != null)
            {
                foreach (Shape shape in doc.Shapes)
                {
                    if (shape.TextBox != null)
                    {
                        SubDocument textBoxDocument = shape.TextBox.Document;
                        texts.Add(textBoxDocument.GetText(textBoxDocument.Range));
                    }
                }
            }

            int result = 0;
            foreach (string text in texts)
            {
                foreach (System.Text.RegularExpressions.Match match in Regex.Matches(text, @"IMAGE_DATA_(\d+)"))
                {
                    int number;
                    if (int.TryParse(match.Groups[1].Value, out number) && number > result)
                        result = number;
                }
            }
            return result;
        }

        /// <summary>
        /// Ảnh mới thêm: tích và nhận STT cuối nếu chưa đủ số key ảnh.
        /// </summary>
        internal static void CheckNewImage(List<ImageADO> listImage, ImageADO image, int? maxImageKey)
        {
            if (image == null || !(maxImageKey > 0))
                return;

            List<ImageADO> checkedImages = listImage != null ? listImage.Where(o => o != image && o.IsChecked).ToList() : new List<ImageADO>();
            if (checkedImages.Count >= maxImageKey)
                return;

            image.IsChecked = true;
            image.STTImage = (checkedImages.Count > 0 ? checkedImages.Max(o => o.STTImage ?? 0) : 0) + 1;
        }

        internal static bool IsOverImageKey(List<ImageADO> listImage, int? maxImageKey)
        {
            return maxImageKey > 0 && listImage != null && listImage.Count(o => o.IsChecked) > maxImageKey;
        }

        /// <summary>
        /// Đánh lại STT ảnh đang tích thành 1, 2, 3... giữ nguyên thứ tự cũ (không sắp theo tên file).
        /// Dùng khi ảnh rời danh sách tích: bỏ tích, xóa, đổi ảnh vào vùng chọn.
        /// </summary>
        internal static void ReorderCheckedStt(List<ImageADO> listImage)
        {
            if (listImage == null)
                return;

            int stt = 1;
            foreach (var image in listImage.Where(o => o.IsChecked).OrderBy(o => o.STTImage ?? int.MaxValue).ToList())
            {
                image.STTImage = stt;
                stt += 1;
            }
            foreach (var image in listImage.Where(o => !o.IsChecked))
            {
                image.STTImage = null;
            }
        }

        /// <summary>
        /// Đổi sang mẫu ít key ảnh hơn: giữ các ảnh có STT nhỏ nhất, bỏ tích phần thừa. Trả về true nếu có bỏ tích.
        /// </summary>
        internal static bool UncheckOverImageKey(List<ImageADO> listImage, int? maxImageKey)
        {
            if (!IsOverImageKey(listImage, maxImageKey))
                return false;

            ReorderCheckedStt(listImage);
            foreach (var image in listImage.Where(o => o.IsChecked && o.STTImage > maxImageKey))
            {
                image.IsChecked = false;
                image.STTImage = null;
            }
            return true;
        }
    }
}
