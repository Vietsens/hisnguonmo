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
using DevExpress.XtraLayout;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// vCong53286 - Doi chieu ho so voi he thong tien giam dinh tai man Xuat XML 130.
    ///
    /// Hai cho kich hoat, dung chung mot luong tra cuu:
    ///  - Nut "Kiem tra loi": nguoi dung chu dong ra soat, khong ngan thao tac nao.
    ///  - Truoc ca 5 chuc nang xuat: tu dong tra, ho so co loi nghiem trong thi dung ca luot.
    ///
    /// Man nay KHONG co cong tac bat/tat rieng - da dau noi la buoc kiem tra co hieu luc,
    /// dung tai lieu 3136 muc 2 ghi buoc xuat XML "bat buoc - khong tat duoc".
    ///
    /// Tham chieu: PTTK_53286 muc B.4.2
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>Nguong so ho so ma vuot qua thi hoi xac nhan truoc khi chay</summary>
        private const int TIEN_GIAM_DINH_CONFIRM_THRESHOLD = 100;

        /// <summary>So luot goi toi da moi phut ma he ngoai cho phep - dung de uoc luong thoi gian</summary>
        private const int TIEN_GIAM_DINH_RATE_PER_MINUTE = 60;

        /// <summary>
        /// Ket qua tra cuu trong phien, khoa theo ma dieu tri.
        /// Dung lai de tranh goi trung khi nguoi dung bam Kiem tra roi bam Xuat ngay sau do.
        /// </summary>
        private Dictionary<string, TienGiamDinhResultADO> tienGiamDinhResultInSession
            = new Dictionary<string, TienGiamDinhResultADO>();

        /// <summary>Da dau noi he tien giam dinh chua</summary>
        private bool IsTienGiamDinhConfigured
        {
            get
            {
                return new TienGiamDinhWorker(HisConfigCFG.TIEN_GIAM_DINH__CONNECTION_INFO).IsValidConfig;
            }
        }

        /// <summary>
        /// Dat ten hien thi va an/hien nut "Kiem tra loi".
        /// Nut duoc khai bao san trong Designer (canh nut Tim); o day chi quyet dinh co hien hay khong
        /// theo trang thai dau noi - vien chua dung thi man hinh giu nguyen nhu cu.
        /// Goi trong su kien Load cua man hinh.
        /// </summary>
        private void InitTienGiamDinhButton()
        {
            try
            {
                this.btnCheckTienGiamDinh.Text = Resources.ResourceMessageLang.TienGiamDinhNutKiemTraLoi;

                //An ca muc layout chu khong chi an nut, de khong de lai o trong tren hang nut
                this.lciCheckTienGiamDinh.Visibility = this.IsTienGiamDinhConfigured
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void btnCheckTienGiamDinh_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listSelection == null || this.listSelection.Count == 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.TienGiamDinhChuaChonHoSo,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<TienGiamDinhResultADO> results = await CheckTreatmentsAsync(this.listSelection);
                if (results == null)
                {
                    //Nguoi dung khong xac nhan chay
                    return;
                }

                ShowTienGiamDinhResult(results);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tra cuu mot lo ho so. Tra ve null khi nguoi dung khong dong y chay.
        /// Chay tuan tu vi he ngoai chi nhan mot ma moi luot va gioi han tan suat.
        /// </summary>
        private async Task<List<TienGiamDinhResultADO>> CheckTreatmentsAsync(List<V_HIS_TREATMENT_1> treatments)
        {
            List<TienGiamDinhResultADO> results = new List<TienGiamDinhResultADO>();
            try
            {
                if (treatments == null || treatments.Count == 0)
                {
                    return results;
                }

                //Lo lon thi bao truoc thoi gian du kien roi hoi xac nhan - khong chan, chi hoi
                if (treatments.Count > TIEN_GIAM_DINH_CONFIRM_THRESHOLD)
                {
                    int estimateMinute = (int)Math.Ceiling((double)treatments.Count / TIEN_GIAM_DINH_RATE_PER_MINUTE);
                    string confirm = string.Format(
                        Resources.ResourceMessageLang.TienGiamDinhXacNhanLoLon,
                        treatments.Count, estimateMinute);

                    if (XtraMessageBox.Show(confirm, Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return null;
                    }
                }

                TienGiamDinhWorker worker = new TienGiamDinhWorker(HisConfigCFG.TIEN_GIAM_DINH__CONNECTION_INFO);
                if (!worker.IsValidConfig)
                {
                    return results;
                }

                using (frmTienGiamDinhProgress progress = new frmTienGiamDinhProgress(
                    treatments.Count,
                    Resources.ResourceMessageLang.TienGiamDinhTieuDeTienTrinh,
                    Resources.ResourceMessageLang.TienGiamDinhNutHuy))
                {
                    //Khoa man hinh trong luc tra cuu - tranh nguoi dung bam nut xuat khac
                    //hoac doi danh sach tich chon khi vong lap dang chay.
                    this.Enabled = false;
                    progress.Show(this.ParentForm);
                    try
                    {
                        int done = 0;
                        foreach (V_HIS_TREATMENT_1 treatment in treatments)
                        {
                            if (progress.IsCancelled)
                            {
                                LogSystem.Info("TienGiamDinh - Nguoi dung huy giua chung sau "
                                    + done + "/" + treatments.Count + " ho so.");
                                break;
                            }

                            TienGiamDinhResultADO result = await CheckOneTreatmentAsync(
                                worker, treatment, progress.CancelToken);
                            results.Add(result);

                            done++;
                            progress.SetProgress(done, treatments.Count, string.Format(
                                Resources.ResourceMessageLang.TienGiamDinhDangKiemTra,
                                done, treatments.Count));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        //Nguoi dung huy - giu nguyen ket qua cac ho so da tra xong
                        LogSystem.Info("TienGiamDinh - Da huy tra cuu lo.");
                    }
                    finally
                    {
                        progress.Close();
                        this.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return results;
        }

        /// <summary>Tra cuu mot ho so, uu tien dung lai ket qua da co trong phien</summary>
        private async Task<TienGiamDinhResultADO> CheckOneTreatmentAsync(TienGiamDinhWorker worker,
            V_HIS_TREATMENT_1 treatment, CancellationToken cancelToken)
        {
            string treatmentCode = treatment == null ? "" : treatment.TREATMENT_CODE;

            TienGiamDinhResultADO cached;
            if (!string.IsNullOrEmpty(treatmentCode)
                && this.tienGiamDinhResultInSession.TryGetValue(treatmentCode, out cached))
            {
                return cached;
            }

            TienGiamDinhResultADO result = await worker.CheckAsync(treatmentCode, cancelToken);
            result.PatientName = treatment == null ? "" : treatment.TDL_PATIENT_NAME;
            FillTienGiamDinhDisplayName(result);

            //CHI nho ket qua da tra cuu duoc. Ket qua that bai (cong qua tai, qua thoi gian cho,
            //loi he thong) khong duoc nho: nho lai thi lan bam "Kiem tra loi" sau se tra ve
            //dung loi cu ma khong goi lai cong, nguoi dung khong con duong nao kiem lai
            //ngoai viec tai lai ca danh sach.
            if (!string.IsNullOrEmpty(treatmentCode)
                && result.Status != EnumTienGiamDinhStatus.CheckFailed
                && result.Status != EnumTienGiamDinhStatus.NotChecked)
            {
                this.tienGiamDinhResultInSession[treatmentCode] = result;
            }
            return result;
        }

        /// <summary>Gan ten trang thai va ten nhom loi theo ngon ngu dang dung, de binh vao luoi</summary>
        private void FillTienGiamDinhDisplayName(TienGiamDinhResultADO result)
        {
            try
            {
                switch (result.Status)
                {
                    case EnumTienGiamDinhStatus.NoError:
                        result.StatusName = Resources.ResourceMessageLang.TienGiamDinhTrangThaiKhongLoi;
                        break;
                    case EnumTienGiamDinhStatus.Warning:
                        result.StatusName = Resources.ResourceMessageLang.TienGiamDinhTrangThaiCanhBao;
                        break;
                    case EnumTienGiamDinhStatus.Critical:
                        result.StatusName = Resources.ResourceMessageLang.TienGiamDinhTrangThaiLoiNghiemTrong;
                        break;
                    case EnumTienGiamDinhStatus.CheckFailed:
                        result.StatusName = GetTienGiamDinhFailReasonName(result.FailReason);
                        break;
                    default:
                        result.StatusName = "";
                        break;
                }

                foreach (TienGiamDinhErrorADO error in result.Errors)
                {
                    switch (error.Group)
                    {
                        case EnumTienGiamDinhErrorGroup.OrderCheck:
                            error.GroupName = Resources.ResourceMessageLang.TienGiamDinhNhomSaiSotYLenh;
                            break;
                        case EnumTienGiamDinhErrorGroup.HeinCard:
                            error.GroupName = Resources.ResourceMessageLang.TienGiamDinhNhomLoiTraThe;
                            break;
                        case EnumTienGiamDinhErrorGroup.Xml3176:
                            error.GroupName = Resources.ResourceMessageLang.TienGiamDinhNhomLoiHoSoXml;
                            break;
                    }

                    error.SeverityName = error.IsCritical
                        ? Resources.ResourceMessageLang.TienGiamDinhTrangThaiLoiNghiemTrong
                        : Resources.ResourceMessageLang.TienGiamDinhTrangThaiCanhBao;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ten ly do khong tra cuu duoc, tach rieng tung ly do thay vi gop chung mot chu.
        /// Nguoi dung phai phan biet duoc viec nen bam kiem lai (cong qua tai, qua thoi gian cho)
        /// voi viec phai bao ky thuat (sai thong tin xac thuc, loi he thong).
        /// </summary>
        private string GetTienGiamDinhFailReasonName(EnumTienGiamDinhFailReason failReason)
        {
            switch (failReason)
            {
                case EnumTienGiamDinhFailReason.Unauthorized:
                    return Resources.ResourceMessageLang.TienGiamDinhSaiThongTinXacThuc;

                case EnumTienGiamDinhFailReason.RateLimited:
                    return Resources.ResourceMessageLang.TienGiamDinhTrangThaiCongQuaTai;

                case EnumTienGiamDinhFailReason.Timeout:
                    return Resources.ResourceMessageLang.TienGiamDinhTrangThaiQuaThoiGianCho;

                default:
                    //NotConfigured va SystemError - nguoi dung khong tu xu ly duoc, giu chu chung
                    return Resources.ResourceMessageLang.TienGiamDinhTrangThaiKhongKiemTraDuoc;
            }
        }

        private void ShowTienGiamDinhResult(List<TienGiamDinhResultADO> results)
        {
            try
            {
                //Cua so ket qua co nut xuat danh sach loi ra Excel. Ban quyen Aspose chi duoc dang ky
                //ngay truoc cac luong xuat Excel san co, nen phai dang ky o day - neu khong,
                //nguoi dung chua tung xuat Excel trong phien se nhan tep dinh dau ban dung thu.
                try { SetLicenseForAsposeCell(); } catch (Exception exLicense) { LogSystem.Warn(exLicense); }

                using (frmTienGiamDinhResult form = new frmTienGiamDinhResult(results))
                {
                    form.ShowDialog(this.ParentForm);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cong chan truoc khi ket xuat - dung chung cho ca 5 chuc nang xuat.
        /// Tra ve true khi duoc phep xuat.
        ///
        /// Quy tac (PTTK_53286 QT-13 den QT-15):
        ///  - Chua dau noi -> cho xuat, khong phat sinh luot goi nao.
        ///  - Co ho so mang loi nghiem trong -> DUNG CA LUOT, khong sinh tep nao.
        ///  - Co ho so CHUA KIEM TRA DUOC -> cung DUNG CA LUOT. "Khong kiem duoc" khong dong nghia
        ///    "ho so sach", nen khong duoc phep di qua cong nay.
        ///  - Chi co loi nhe -> canh bao, nguoi dung xac nhan thi cho xuat.
        ///
        /// Luu y van hanh: he tien giam dinh chet thi toan bo chuc nang xuat XML dung theo,
        /// ke ca ho so hoan toan sach. Day la lua chon co chu dich cua vien, khong phai thieu sot.
        /// Muon xuat trong luc cong chet thi phai xoa cau hinh HIS.TIEN_GIAM_DINH.CONNECTION_INFO.
        /// </summary>
        private async Task<bool> EnsureTienGiamDinhPassedAsync()
        {
            try
            {
                if (!this.IsTienGiamDinhConfigured)
                {
                    return true;
                }

                if (this.listSelection == null || this.listSelection.Count == 0)
                {
                    return true;
                }

                List<TienGiamDinhResultADO> results = await CheckTreatmentsAsync(this.listSelection);
                if (results == null)
                {
                    //Nguoi dung khong dong y chay kiem tra -> khong xuat
                    return false;
                }

                //Nguoi dung huy giua chung -> chua kiem het, khong cho xuat de tranh lot ho so chua kiem
                if (results.Count < this.listSelection.Count)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.TienGiamDinhHuyGiuaChungKhongXuat,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                int criticalCount = results.Count(o => o.Status == EnumTienGiamDinhStatus.Critical);
                int failedCount = results.Count(o => o.Status == EnumTienGiamDinhStatus.CheckFailed);

                //Chan theo lo: mot ho so khong dat la dung ca luot, tranh tep ra mot nua.
                //Ho so chua kiem duoc bi chan ngang hang voi ho so co loi nghiem trong -
                //chua kiem duoc thi khong ai dam chac no sach.
                if (criticalCount > 0 || failedCount > 0)
                {
                    XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessageLang.TienGiamDinhChanXuat,
                            criticalCount, failedCount),
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    ShowTienGiamDinhResult(results);
                    return false;
                }

                int warningCount = results.Count(o => o.Status == EnumTienGiamDinhStatus.Warning);

                if (warningCount > 0)
                {
                    string message = string.Format(
                        Resources.ResourceMessageLang.TienGiamDinhCanhBaoTruocKhiXuat,
                        warningCount);

                    if (XtraMessageBox.Show(message, Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        ShowTienGiamDinhResult(results);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //Loi phia HIS thi khong duoc lam tac viec ket xuat
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Xoa ket qua da nho trong phien.
        /// Goi khi danh sach ho so duoc tai lai, vi du sau khi loc lai hoac sau khi sua ho so.
        /// </summary>
        private void ClearTienGiamDinhSessionResult()
        {
            try
            {
                this.tienGiamDinhResultInSession.Clear();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
