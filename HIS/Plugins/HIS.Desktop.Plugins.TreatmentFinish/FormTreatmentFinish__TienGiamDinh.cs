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
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using HIS.Desktop.Plugins.TreatmentFinish.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// vCong53286 - Doi chieu ho so voi he thong tien giam dinh truoc khi ket thuc dieu tri.
    ///
    /// Day la buoc kiem tra bo sung vao chuoi kiem tra cua chuc nang "Kiem tra ho so" da co.
    /// Khac voi 13 buoc kiem tra hien co o hai diem:
    ///  - Goi ra he thong ben ngoai nen phai chay bat dong bo, tranh treo giao dien.
    ///  - Canh bao sinh ra KHONG cho bo qua (QT-03), nen o "bo qua canh bao" dung chung
    ///    cua man hinh khong vo hieu hoa duoc.
    ///
    /// Tham chieu: PTTK_53286 muc B.4.1
    /// </summary>
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        /// <summary>So dong loi toi da hien thi. Vuot qua thi gom phan con lai vao mot dong tong ket</summary>
        private const int TIEN_GIAM_DINH_MAX_DISPLAY_LINE = 20;

        /// <summary>
        /// Doi chieu ho so hien tai voi he thong tien giam dinh.
        /// Tra ve false khi phai CHAN viec ket thuc dieu tri.
        ///
        /// Khong dung tham so ref cho danh sach canh bao vi phuong thuc bat dong bo
        /// khong nhan tham so ref - danh sach la kieu tham chieu nen thay doi van thay duoc.
        /// </summary>
        private async Task<bool> CheckTienGiamDinh_ForSave(ValidationDataType validationDataType, List<WarningADO> listWarningADO)
        {
            try
            {
                //Vien chua bat cong tac cho buoc nay -> bo qua hoan toan, khong phat sinh luot goi nao
                if (!Config.ConfigKey.IsTienGiamDinhTreatmentFinishEnable)
                {
                    return true;
                }

                //CO Y KHONG kiem tra _isSkipWarningForSave o day.
                //Canh bao cua buoc nay khong cho bo qua - nguoi dung tick nham o "bo qua canh bao"
                //van phai bi chan, tranh ho so sai ra vien roi day len cong giam dinh (QT-03).

                if (this.currentHisTreatment == null
                    || string.IsNullOrWhiteSpace(this.currentHisTreatment.TREATMENT_CODE))
                {
                    return true;
                }

                TienGiamDinhWorker worker = new TienGiamDinhWorker(Config.ConfigKey.TienGiamDinhConnectionInfo);
                if (!worker.IsValidConfig)
                {
                    //Chua dau noi hoac khai thieu -> khong chan, chi ghi nhat ky (da ghi trong worker)
                    return true;
                }

                TienGiamDinhResultADO result = await worker.CheckAsync(
                    this.currentHisTreatment.TREATMENT_CODE, CancellationToken.None);

                if (!string.IsNullOrEmpty(result.RequestId))
                {
                    LogSystem.Info("TienGiamDinh - Ma dieu tri: " + result.TreatmentCode
                        + ". Trang thai: " + result.Status
                        + ". So loi: " + result.TotalErrorCount
                        + ". RequestId: " + result.RequestId);
                }

                switch (result.Status)
                {
                    case EnumTienGiamDinhStatus.CheckFailed:
                        //He ngoai loi thi KHONG chan (QT-03c). "Khong kiem duoc" khac "da kiem va co loi".
                        AddTienGiamDinhFailWarning(validationDataType, listWarningADO, result);
                        return true;

                    case EnumTienGiamDinhStatus.NoError:
                        AddTienGiamDinhNoErrorInfo(validationDataType, listWarningADO);
                        return true;

                    case EnumTienGiamDinhStatus.Warning:
                    case EnumTienGiamDinhStatus.Critical:
                        //Chan moi loi, khong phan biet muc do (QT-03)
                        return AddTienGiamDinhErrorWarning(validationDataType, listWarningADO, result);

                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                //Loi phia HIS thi khong duoc lam tac nghiep vu ra vien
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Ho so co loi -> chan ket thuc dieu tri.
        /// Moi dong loi hien thanh mot canh bao rieng, co tien to nhom de nguoi dung biet phai sua gi.
        /// </summary>
        private bool AddTienGiamDinhErrorWarning(ValidationDataType validationDataType,
            List<WarningADO> listWarningADO, TienGiamDinhResultADO result)
        {
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage)
                {
                    //Dang luu that su -> hien thong bao chan, chi co nut OK, khong cho di tiep
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        BuildTienGiamDinhMessage(result),
                        ResourceMessage.TienGiamDinhTieuDeCanhBao,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                {
                    int shown = 0;
                    foreach (TienGiamDinhErrorADO error in result.Errors)
                    {
                        if (shown >= TIEN_GIAM_DINH_MAX_DISPLAY_LINE)
                        {
                            break;
                        }

                        WarningADO warning = new WarningADO();
                        //Khong cho bo qua - day la diem cot loi cua QT-03
                        warning.IsSkippable = false;
                        warning.Description = GetTienGiamDinhGroupName(error.Group) + ": " + error.Description;
                        listWarningADO.Add(warning);
                        shown++;
                    }

                    int remain = result.TotalErrorCount - shown;
                    if (remain > 0)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = string.Format(
                            ResourceMessage.TienGiamDinhConNLoiKhac, remain);
                        listWarningADO.Add(warning);
                    }

                    //Danh sach loi bi cat bot -> khong biet con loi nao chua thay
                    if (result.IsTruncated)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = ResourceMessage.TienGiamDinhDanhSachLoiBiCatBot;
                        listWarningADO.Add(warning);
                    }
                }

                //Che do gom danh sach: da nhoi canh bao xong thi tra ve true
                //de chuoi kiem tra chay tiep, khong bo sot canh bao cua cac buoc sau.
                //Viec chan thuc su xay ra o che do PopupMessage khi nguoi dung bam ket thuc.
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Khong tra cuu duoc -> canh bao nhung van cho ket thuc (QT-03c).
        /// </summary>
        private void AddTienGiamDinhFailWarning(ValidationDataType validationDataType,
            List<WarningADO> listWarningADO, TienGiamDinhResultADO result)
        {
            try
            {
                string message = result.FailReason == EnumTienGiamDinhFailReason.Unauthorized
                    ? ResourceMessage.TienGiamDinhSaiThongTinXacThuc
                    : ResourceMessage.TienGiamDinhKhongKiemTraDuoc;

                if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                {
                    WarningADO warning = new WarningADO();
                    //Cho bo qua - khong kiem duoc thi khong duoc lam tac viec ra vien
                    warning.IsSkippable = true;
                    warning.Description = message;
                    listWarningADO.Add(warning);
                }
                //Che do PopupMessage: khong lam phien nguoi dung dang luu, chi ghi nhat ky
                LogSystem.Warn("TienGiamDinh - " + message + " Ma dieu tri: " + result.TreatmentCode);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khong co loi -> van phai noi ro chua kiem phan XML,
        /// tranh nguoi dung hieu nham ho so da sach hoan toan (QT-04).
        /// </summary>
        private void AddTienGiamDinhNoErrorInfo(ValidationDataType validationDataType,
            List<WarningADO> listWarningADO)
        {
            try
            {
                if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                {
                    WarningADO warning = new WarningADO();
                    warning.IsSkippable = true;
                    warning.Description = ResourceMessage.TienGiamDinhChuaPhatHienLoi;
                    listWarningADO.Add(warning);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Gom danh sach loi thanh mot doan van ban de hien trong hop thoai chan</summary>
        private string BuildTienGiamDinhMessage(TienGiamDinhResultADO result)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                builder.AppendLine(ResourceMessage.TienGiamDinhChanKetThuc);
                builder.AppendLine();

                int shown = 0;
                foreach (TienGiamDinhErrorADO error in result.Errors)
                {
                    if (shown >= TIEN_GIAM_DINH_MAX_DISPLAY_LINE)
                    {
                        break;
                    }
                    builder.AppendLine("- " + GetTienGiamDinhGroupName(error.Group) + ": " + error.Description);
                    shown++;
                }

                int remain = result.TotalErrorCount - shown;
                if (remain > 0)
                {
                    builder.AppendLine(string.Format(
                        ResourceMessage.TienGiamDinhConNLoiKhac, remain));
                }

                if (result.IsTruncated)
                {
                    builder.AppendLine(ResourceMessage.TienGiamDinhDanhSachLoiBiCatBot);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return builder.ToString();
        }

        /// <summary>Ten nhom loi hien thi cho nguoi dung, lay theo ngon ngu dang dung</summary>
        private string GetTienGiamDinhGroupName(EnumTienGiamDinhErrorGroup group)
        {
            switch (group)
            {
                case EnumTienGiamDinhErrorGroup.OrderCheck:
                    return ResourceMessage.TienGiamDinhNhomSaiSotYLenh;
                case EnumTienGiamDinhErrorGroup.HeinCard:
                    return ResourceMessage.TienGiamDinhNhomLoiTraThe;
                case EnumTienGiamDinhErrorGroup.Xml3176:
                    return ResourceMessage.TienGiamDinhNhomLoiHoSoXml;
                default:
                    return "";
            }
        }
    }
}
