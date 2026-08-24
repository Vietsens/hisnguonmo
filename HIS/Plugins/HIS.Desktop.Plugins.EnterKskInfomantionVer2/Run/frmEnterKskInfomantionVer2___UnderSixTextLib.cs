/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Trẻ em dưới 6 tuổi" — nút Thư viện mẫu cho ô "Quan sát chung" (mục VI. Khám lâm sàng).
 */
using System;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>
        /// Nút Thư viện mẫu cạnh ô "Quan sát chung" (mục VI) — dùng lại luồng Thư viện văn bản
        /// của tab trên 18 tuổi (keyTextLib = 2): nội dung mẫu đổ thẳng vào ô Quan sát chung.
        /// Tab này không có ô Phân loại riêng theo mục nên truyền null (token PL:Lx bị bỏ qua).
        /// </summary>
        private void btnTextLibClinicalObs8_Click(object sender, EventArgs e)
        {
            try
            {
                OpenTextLibExamResult(this.memClinicalObservation8, "QuanSatChung", null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
