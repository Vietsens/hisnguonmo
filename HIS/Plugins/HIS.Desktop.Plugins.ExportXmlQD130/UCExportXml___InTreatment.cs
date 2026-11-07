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
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Viec 60262 (BV Da khoa tinh Dien Bien) - xuat XML 130 cho ho so DANG DIEU TRI (chua ket thuc dieu tri) de kiem tra.
    ///
    /// Truoc day CreateXmlProcessor.Run() cua thu vien tu choi moi ho so chua ket thuc dieu tri ("Ho so chua co thoi gian
    /// khoa vien phi"), vi MOS chi cho khoa vien phi khi da ket thuc dieu tri. Vien bat HIS.QD_130_BYT.EXPORT_XML_IN_TREATMENT = 1
    /// thi cac thao tac di qua ProcessExportXmlDetail (nut Xuat XML, menu Xuat XML, menu "Xuat XML (khong bao gom giam dinh
    /// y khoa)", nut XML 3176, cot Xem XML) sinh ho so nay bang che do ho so dang dieu tri cua thu vien (RunInTreatment, viec 61380):
    /// bo dieu kien khoa vien phi, duyet giam dinh BHYT, ket thuc dieu tri; chua co gio ra vien thi XML1 de trong NGAY_RA,
    /// KET_QUA_DTRI, MA_LOAI_RV, NGAY_TTOAN, NAM_QT, THANG_QT va khong sinh XML7.
    /// Ten tep them duoi _DANG_DIEU_TRI de khong lan voi tep chinh thuc; khong sinh XML12 (giam dinh y khoa) cho ho so nay.
    /// Ho so da ket thuc dieu tri, gui cong, xuat thong tuyen/gop/Excel, kiem tra ho so: nhu cu.
    /// </summary>
    public partial class UCExportXml
    {
        /// <summary>Duoi them vao ten tep XML cua ho so dang dieu tri - tep chi de kiem tra, khong dung gui cong</summary>
        private const string IN_TREATMENT_FILE_SUFFIX = "_DANG_DIEU_TRI";

        /// <summary>
        /// Ho so duoc sinh XML o che do ho so dang dieu tri: vien bat khoa va ho so chua ket thuc dieu tri
        /// (IS_PAUSE khac 1 - gom ca ket thuc luu tam va mo lai ho so van giu gio ra vien).
        /// </summary>
        private bool IsExportXmlInTreatment(V_HIS_TREATMENT_12 treatment)
        {
            bool result = false;
            try
            {
                result = treatment != null
                    && HisConfigCFG.QD_130_BYT__EXPORT_XML_IN_TREATMENT == HisConfigCFG.EXPORT_XML_IN_TREATMENT__ON
                    && treatment.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Sinh XML ho so dang dieu tri. May tram con thu vien His.Bhyt.ExportXml.XML130 cu (truoc 28/10/2026, chua co
        /// RunInTreatment) thi tra null kem ly do ro rang thay vi loi.
        /// </summary>
        private static MemoryStream RunXmlInTreatmentSafe(CreateXmlProcessor xmlProcessor, ref string error)
        {
            MemoryStream result = null;
            try
            {
                result = RunXmlInTreatment(xmlProcessor, ref error);
            }
            catch (MissingMethodException ex)
            {
                LogSystem.Error(ex);
                error = Resources.ResourceMessageLang.XmlDangDieuTriThuVienChuaHoTro;
                result = null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Goi RunInTreatment trong ham rieng, KHONG inline: JIT bien dich ca ham goi, tham chieu thang ham thu vien khong co
        /// se lam hong ca ProcessExportXmlDetail (moi luot Xuat XML) tren may tram con thu vien cu. Tach ra thi chi loi o day.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MemoryStream RunXmlInTreatment(CreateXmlProcessor xmlProcessor, ref string error)
        {
            return xmlProcessor.RunInTreatment(ref error);
        }

        /// <summary>Ten tep XML cua ho so dang dieu tri: chen duoi _DANG_DIEU_TRI truoc phan mo rong</summary>
        private static string GetInTreatmentFileName(string fileName)
        {
            string result = fileName;
            try
            {
                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    result = Path.GetFileNameWithoutExtension(fileName) + IN_TREATMENT_FILE_SUFFIX + Path.GetExtension(fileName);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = fileName;
            }
            return result;
        }
    }
}
