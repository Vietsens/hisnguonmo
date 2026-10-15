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
using HIS.Desktop.Plugins.Library.XmlPrecheck;
using HIS.Desktop.Plugins.Library.XmlPrecheck.ADO;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XmlPrecheckLang = HIS.Desktop.Plugins.Library.XmlPrecheck.Resources.ResourceMessageLang;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// vCong MDInsight ban 3.0 - Kiem tra loi XML SO BO voi he thong soat loi, dat trong chuoi "Kiem tra ho so".
    ///
    /// Chot 2026-10-13: KHONG them nut rieng. Buoc nay chen vao CUOI chuoi kiem tra cua nut "Kiem tra ho so"
    /// (cung chuoi voi buoc tien giam dinh vCong53286), ngay truoc khi hien danh sach canh bao:
    ///  - Vien khong bat khoa -> thoat som, nut Kiem tra ho so y het hien tai.
    ///  - Ho so chua Luu tam (chua co thoi gian ra vien) -> them MOT dong nhac vao danh sach canh bao, khong goi he ngoai.
    ///  - Da Luu tam -> thu vien dung chung tu quyet gui moi hay chi tra lai (nhu man Xuat XML 130), roi them dong tom tat
    ///    vao danh sach canh bao; co loi thi mo cua so ket qua so bo.
    ///
    /// Ket qua CHI LA SO BO (ho so chua khoa vien phi - QT-32). Khong bao gio chan: moi dong canh bao deu cho bo qua,
    /// khong chen vao luong Luu / Ket thuc that (QT-35).
    ///
    /// Tham chieu: PTTK muc B.4.1.2.
    /// </summary>
    public partial class FormTreatmentFinish
    {
        private XmlPrecheckGate xmlPrecheckGate;
        private XmlPrecheckRunner xmlPrecheckRunner;

        /// <summary>Ket qua gan nhat cua ho so dang mo - doc tu ban ghi khi mo man, cap nhat sau moi luot</summary>
        private MdInsightResultADO xmlPrecheckLatest;

        /// <summary>Da Luu tam thanh cong trong phien nay - dieu kien de co du lieu dung tep (QT-30)</summary>
        private bool xmlPrecheckSavedTempInSession;

        private bool xmlPrecheckRunning;

        #region Khoi tao

        /// <summary>
        /// Goi sau khi da quyet dinh an/hien nut Luu tam. Thieu mot trong ba khoa thi thoat som - khong giu gi,
        /// buoc kiem tra trong chuoi "Kiem tra ho so" se tu bo qua.
        /// </summary>
        private void InitXmlPrecheck()
        {
            try
            {
                if (this.btnSaveTemp == null || !this.btnSaveTemp.Visible)
                {
                    return;
                }

                this.xmlPrecheckGate = XmlPrecheckGate.Read();
                if (!this.xmlPrecheckGate.IsTreatmentFinishEnabled)
                {
                    return;
                }

                this.xmlPrecheckRunner = new XmlPrecheckRunner(this.xmlPrecheckGate.Config);
                this.xmlPrecheckLatest = this.xmlPrecheckRunner.LoadStored(this.currentHisTreatment);

                LogSystem.Info("XmlPrecheck - Da kich hoat kiem tra loi XML so bo trong chuoi Kiem tra ho so, ho so "
                    + this.treatmentId + ".");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.xmlPrecheckRunner = null;
            }
        }

        /// <summary>Goi sau khi Luu tam thanh cong. KHONG tu gui (QT-35) - chi ghi nhan la da co du lieu de dung tep.</summary>
        private void OnXmlPrecheckSaveTempSucceeded()
        {
            this.xmlPrecheckSavedTempInSession = true;
        }

        #endregion

        #region Buoc trong chuoi Kiem tra ho so

        /// <summary>
        /// Buoc kiem tra loi XML so bo trong chuoi "Kiem tra ho so". LUON tra ve true - khong chan gi.
        /// Danh sach canh bao la kieu tham chieu nen them dong o day la ben goi thay duoc (giong buoc tien giam dinh).
        /// </summary>
        private async Task<bool> CheckXmlPrecheck_ForKiemTraHoSo(List<WarningADO> listWarningADO)
        {
            try
            {
                //Vien khong bat (thieu mot trong ba khoa) -> khong phat sinh gi
                if (this.xmlPrecheckRunner == null || this.xmlPrecheckRunning || this.currentHisTreatment == null)
                {
                    return true;
                }

                bool feeLocked = this.currentHisTreatment.FEE_LOCK_TIME.HasValue;
                bool hasOutTime = this.currentHisTreatment.OUT_TIME.HasValue;
                bool hasSent = this.xmlPrecheckLatest != null
                    && !string.IsNullOrEmpty(this.xmlPrecheckLatest.SentFileName)
                    && this.xmlPrecheckLatest.SendTime.HasValue;

                if (feeLocked && !hasSent)
                {
                    //Da khoa vien phi, chua tung gui -> kiem chinh thuc o man Xuat XML 130, khong gui tep so bo nua
                    AddXmlPrecheckWarning(listWarningADO, XmlPrecheckLang.XmlPrecheckDaKhoaVienPhi);
                    return true;
                }

                if (!feeLocked && !this.xmlPrecheckSavedTempInSession && !hasOutTime)
                {
                    //Chua Luu tam -> chua co thoi gian ra vien, khong dung duoc tep. Nhac, khong goi he ngoai
                    AddXmlPrecheckWarning(listWarningADO, XmlPrecheckLang.XmlPrecheckNhacLuuTam);
                    return true;
                }

                MdInsightResultADO result = await RunXmlPrecheckAsync(!feeLocked);
                if (result == null)
                {
                    //Khong chay duoc (loi cau hinh, he ngoai hong, ho so thieu du lieu...) - thu vien da hien hop thong bao;
                    //ghi them ly do vao danh sach canh bao de nguoi dung nhin mot cho la du. Khong ghi trang thai.
                    if (!string.IsNullOrEmpty(this.xmlPrecheckRunner.LastShownMessage))
                    {
                        AddXmlPrecheckWarning(listWarningADO,
                            string.Format(XmlPrecheckLang.XmlPrecheckTomTatDong, this.xmlPrecheckRunner.LastShownMessage));
                    }
                    return true;
                }

                this.xmlPrecheckLatest = result;
                AddXmlPrecheckWarning(listWarningADO, string.Format(XmlPrecheckLang.XmlPrecheckTomTatDong, DescribeXmlPrecheckOutcome(result)));

                if (result.Status == EnumXmlPrecheckStatus.Warning || result.Status == EnumXmlPrecheckStatus.Critical)
                {
                    ShowXmlPrecheckResultWindow(result);
                }

                return true;
            }
            catch (Exception ex)
            {
                //Loi phia HIS khong duoc lam hong chuoi kiem tra
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>Mot lenh - thu vien tu quyet gui moi hay chi tra lai theo trang thai da luu (nhu man Xuat XML 130)</summary>
        private async Task<MdInsightResultADO> RunXmlPrecheckAsync(bool allowSend)
        {
            this.xmlPrecheckRunning = true;

            //Khoa man trong luc chay - cung cach lam voi man Xuat XML 130; hop tien trinh van bam huy duoc
            this.Enabled = false;
            try
            {
                return await this.xmlPrecheckRunner.CheckAutoAsync(this.currentHisTreatment, this, allowSend);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
            finally
            {
                this.Enabled = true;
                this.xmlPrecheckRunning = false;
            }
        }

        /// <summary>Mot dong tom tat cho danh sach canh bao - chu "so bo" phai co o moi nhanh (QT-32)</summary>
        private string DescribeXmlPrecheckOutcome(MdInsightResultADO result)
        {
            switch (result.Status)
            {
                case EnumXmlPrecheckStatus.NoError:
                    return XmlPrecheckLang.XmlPrecheckKhongLoiSoBo;

                case EnumXmlPrecheckStatus.Pending:
                    return XmlPrecheckLang.XmlPrecheckDaGuiChuaCoKetQua;

                case EnumXmlPrecheckStatus.CheckFailed:
                    return XmlPrecheckLang.MdInsightTrangThaiKhongKiemTraDuoc
                        + (string.IsNullOrEmpty(result.Reason) ? "" : ": " + result.Reason);

                case EnumXmlPrecheckStatus.Warning:
                case EnumXmlPrecheckStatus.Critical:
                    return XmlPrecheckLabel.StatusName((short)result.Status)
                        + string.Format(XmlPrecheckLang.XmlPrecheckKetQuaGanNhatSoLoi, result.ErrorNum ?? 0, result.CriticalNum ?? 0)
                        + " - " + XmlPrecheckLang.XmlPrecheckTieuDeSoBo;

                default:
                    return XmlPrecheckLabel.StatusName((short)result.Status);
            }
        }

        private static void AddXmlPrecheckWarning(List<WarningADO> listWarningADO, string description)
        {
            try
            {
                if (listWarningADO == null || string.IsNullOrEmpty(description))
                {
                    return;
                }

                WarningADO warning = new WarningADO();
                //Luon cho bo qua - buoc nay chi canh bao, khong chan (QT-35)
                warning.IsSkippable = true;
                warning.Description = description;
                listWarningADO.Add(warning);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ShowXmlPrecheckResultWindow(MdInsightResultADO result)
        {
            string note = XmlPrecheckLang.XmlPrecheckTieuDeSoBo;

            //Ho so sua sau khi kiem -> noi ro anh chup da cu (QT-33)
            if (this.currentHisTreatment != null
                && XmlPrecheckLabel.IsStale(this.currentHisTreatment.MODIFY_TIME, result.SendTime, result.CheckTime))
            {
                note += " · " + XmlPrecheckLang.XmlPrecheckNhanDuLieuDaDoi;
            }

            this.xmlPrecheckRunner.ShowResultWindow(result, this, XmlPrecheckLang.XmlPrecheckTieuDeSoBo, note);
        }

        #endregion
    }
}
