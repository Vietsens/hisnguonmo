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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.RegisterV2.ADO;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    /// <summary>
    /// Nap san thong tin nguoi benh khi goi so thu tu duoc lay tai man ki-ot.
    /// Thiet ke: PTTK_54254 muc B.4.1.3.
    /// </summary>
    public partial class UCRegister
    {
        /// <summary>Loai dinh danh ghi nhan tai man ki-ot</summary>
        private const string IDENTITY_TYPE__CCCD = "CCCD";
        private const string IDENTITY_TYPE__VNEID = "VNEID";
        private const string IDENTITY_TYPE__BHYT = "BHYT";

        /// <summary>Cach nguoi benh dua thong tin vao tai man ki-ot</summary>
        private const string IDENTITY_INPUT_MODE__QR = "QR";

        /// <summary>
        /// Key cau hinh toan vien bat tinh nang lay so thu tu co xac dinh nguoi benh.
        /// Doc thang tai day thay vi qua lop cau hinh tinh, vi man nay nap cau hinh
        /// tu thu vien dung chung nen lop cau hinh rieng cua plugin khong duoc goi.
        /// </summary>
        private const string CONFIG_KEY__IS_ISSUE_WITH_IDENTITY = "MOS.HIS_REGISTER_REQ.IS_ISSUE_WITH_IDENTITY";

        /// <summary>Vien co bat tinh nang hay khong. Loi doc cau hinh thi coi nhu tat.</summary>
        private bool IsIssueWithIdentityOn()
        {
            try
            {
                string value = HisConfigs.Get<string>(CONFIG_KEY__IS_ISSUE_WITH_IDENTITY);
                return !String.IsNullOrWhiteSpace(value) && value.Trim() == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Kiem tra ba dieu kien roi nap thong tin nguoi benh len man tiep don.
        /// Thieu bat ky dieu kien nao thi giu nguyen hanh vi hien tai, khong lam gi them.
        ///
        /// Dieu kien 1: key cau hinh cua vien dang bat.
        /// Dieu kien 2: buoc nhay bang 1 va ket qua goi tra ve dung mot ban ghi.
        /// Dieu kien 3: ban ghi so duoc goi co chuoi JSON thong tin nguoi benh.
        /// </summary>
        private void ProcessFillPatientFromRegisterReq(List<HIS_REGISTER_REQ> apiResult, long? callStep)
        {
            try
            {
                // Dieu kien 1
                if (!this.IsIssueWithIdentityOn())
                {
                    LogSkip("dieu kien 1: key cau hinh MOS.HIS_REGISTER_REQ.IS_ISSUE_WITH_IDENTITY chua bat");
                    return;
                }

                // Dieu kien 2. Khong nhap buoc nhay thi hieu la 1.
                long step = callStep ?? 1;
                if (step != 1)
                {
                    LogSkip("dieu kien 2: buoc nhay dang la " + step + ", chi nap san khi bang 1");
                    return;
                }
                if (apiResult == null || apiResult.Count != 1)
                {
                    LogSkip("dieu kien 2: ket qua goi tra ve " + (apiResult == null ? 0 : apiResult.Count) + " ban ghi, chi nap san khi dung 1");
                    return;
                }

                // Dieu kien 3
                HIS_REGISTER_REQ registerReq = apiResult.FirstOrDefault();
                if (registerReq == null || String.IsNullOrWhiteSpace(registerReq.IDENTITY_JSON))
                {
                    LogSkip("dieu kien 3: ban ghi so thu tu khong co chuoi JSON dinh danh, tuc la so nay khong lay qua popup ki-ot");
                    return;
                }

                RegisterReqIdentityInfoADO info = this.ParseIdentityJson(registerReq.IDENTITY_JSON);
                if (info == null || String.IsNullOrWhiteSpace(info.IdentityNumber))
                {
                    return;
                }

                // Nut Goi chay tren thread phu (xem CreateThreadCallPatient), ma toan bo
                // phan hoi thoai va nap du lieu ben duoi deu dung toi control giao dien.
                // Khong dua ve thread chinh thi phan mem se van - da gap that khi kiem thu.
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate() { this.FillPatientWithConfirm(info); }));
                }
                else
                {
                    this.FillPatientWithConfirm(info);
                }
            }
            catch (Exception ex)
            {
                // Khong chan thao tac goi so khi nap san that bai
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hoi xac nhan roi nap du lieu. Chi duoc goi tren thread giao dien.
        /// </summary>
        private void FillPatientWithConfirm(RegisterReqIdentityInfoADO info)
        {
            try
            {
                if (!this.ConfirmReplaceCurrentData())
                {
                    this.LogSkip("nhan vien chon khong thay thong tin dang nhap do");
                    return;
                }
                this.FillPatientByIdentityInfo(info);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ghi nhat ky ly do khong nap san, de nguoi kiem thu biet vuong o dau.
        /// Khong ghi noi dung dinh danh ra nhat ky theo quy dinh bao ve du lieu nguoi benh.
        /// </summary>
        private void LogSkip(string reason)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("PTTK_54254 khong nap san thong tin nguoi benh: " + reason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doc chuoi JSON. Khong ghi noi dung dinh danh ra nhat ky theo quy dinh
        /// bao ve du lieu nguoi benh, chi ghi nhan la doc duoc hay khong.
        /// </summary>
        private RegisterReqIdentityInfoADO ParseIdentityJson(string identityJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<RegisterReqIdentityInfoADO>(identityJson);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("ProcessFillPatientFromRegisterReq: khong doc duoc chuoi JSON thong tin nguoi benh.", ex);
            }
            return null;
        }

        /// <summary>
        /// Man tiep don dang co du lieu chua luu thi hoi truoc khi de len.
        /// Man dang trong thi nap thang, khong hoi.
        /// </summary>
        private bool ConfirmReplaceCurrentData()
        {
            try
            {
                var currentPatient = this.ucPatientRaw1.GetValue();
                bool hasData = (currentPatient != null && !String.IsNullOrWhiteSpace(currentPatient.PATIENT_NAME));
                if (!hasData)
                {
                    return true;
                }

                return DevExpress.XtraEditors.XtraMessageBox.Show(
                    ResourceMessage.ThayThongTinDangNhapBangSoVuaGoi,
                    ResourceMessage.TieuDeCuaSoThongBaoLaThongBao,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Dua du lieu trong JSON vao dung luong quet ma hoac tra cuu dang co
        /// o khung Benh nhan (F2). Nho vay giu nguyen moi hanh vi san co:
        /// tim ho so cu theo so dinh danh, tach dia chi, nap the BHYT,
        /// tu chuyen doi tuong va tra cong BHXH theo cau hinh cua vien.
        /// </summary>
        private void FillPatientByIdentityInfo(RegisterReqIdentityInfoADO info)
        {
            try
            {
                bool isQrCode = (info.InputMode == IDENTITY_INPUT_MODE__QR)
                    && !String.IsNullOrWhiteSpace(info.RawData);

                if (isQrCode)
                {
                    // Dua lai dung chuoi quet goc, y het nhu nhan vien vua quet tai quay
                    this.ucPatientRaw1.SearchPatientByCodeOrQrCode(info.RawData);
                    return;
                }

                // Nguoi benh go tay tai ki-ot: chi co so dinh danh, tra cuu theo dung khoa
                string keyTypeFind = (info.IdentityType == IDENTITY_TYPE__BHYT)
                    ? HIS.UC.UCPatientRaw.ResourceMessage.typeCodeFind__SoThe
                    : HIS.UC.UCPatientRaw.ResourceMessage.typeCodeFind__MaCMCC;

                this.ucPatientRaw1.SearchPatientByCodeOrQrCode(info.IdentityNumber, keyTypeFind);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
