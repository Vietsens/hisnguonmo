/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    /// <summary>
    /// Viec 56273 (PT-56273, tich hop tinh nang vien Nghe An): Tu dong xuat thuoc, vat tu di kem khi thuc hien DVKT.
    /// - O tich "Tu dong luu" (chkAutoSave) ngay ben trai nut Luu, trang thai nho tren MAY TRAM (SQLite ControlState cua
    ///   client, giong moi checkbox nho trang thai khac cua HIS) qua ControlStateWorker
    ///   (KEY = chkAutoSave.Name, MODULE_LINK = HIS.Desktop.Plugins.AssignPrescriptionCLS) - mau frmAggrExpMestDetail.
    /// - Khi form mo tu luong THUC HIEN DVKT (nut "Tu truc" + menu chuot phai "Ke thuoc/vat tu tieu hao" o Thuc hien dich vu,
    ///   "Ke don tu truc" o Thuc hien xet nghiem, "Ke don can lam sang" (ke moi) o Phong thuc hien: IsCabinet = true + SereServ,
    ///   khong phai sua don) va thuoc/vat tu
    ///   di kem theo HIS_SERVICE_METY/MATY da do len luoi, neu o tich dang bat thi tu goi ProcessSaveData(SAVE) sau khi
    ///   form hien xong VA 2 kiem tra async cua Load (no vien phi, tran BHYT) da tra loi xong - dung nhu nguoi dung bam Luu.
    /// - Ket hop key HIS.Desktop.Plugins.AssignPrescriptionCLS.AutoClose = 1 (HisConfigCFG.IsAutoCloseAfterSave,
    ///   xu ly o frmAssignPrescription__Save.cs) thi form tu dong lai sau khi luu thanh cong.
    /// Ban Nghe An gui sang (Zalo anh Bui Nam 22/09/2026) chi co doan AutoClose + mo ta checkbox; phan checkbox,
    /// nho trang thai, dieu kien nhan dien luong va diem kich hoat tu luu do minh thiet ke.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        #region 56273 - Tu dong luu thuoc/vat tu di kem

        /// <summary>MODULE_LINK luu ControlState - chuoi hang trung ten plugin (tien le frmAggrExpMestDetail).</summary>
        private const string AUTO_SAVE_MODULE_LINK = "HIS.Desktop.Plugins.AssignPrescriptionCLS";

        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        /// <summary>Chan CheckedChanged ghi lai trang thai trong luc khoi phuc o Load.</summary>
        private bool isNotLoadWhileChangeControlStateInFirst;
        /// <summary>
        /// Form duoc mo tu luong THUC HIEN DVKT de xuat thuoc/vat tu di kem: IsCabinet + co dich vu dang thuc hien
        /// (SereServ) + khong phai sua don. Khong bat nham man Ke don CLS mo tu Danh sach y lenh / To dieu tri
        /// (khong co SereServ) hay nhanh Sua don cua Phong thuc hien (co AssignPrescriptionEditADO).
        /// </summary>
        private bool isOpenFromServiceExecute;
        /// <summary>
        /// Task gop 2 kiem tra async ban ra trong Load (CheckWarningOverTotalPatientPrice - no vien phi,
        /// LoadTotalSereServByHeinWithTreatment - tran BHYT; ca 2 deu co the hoi YesNo roi this.Close()).
        /// Gan o FillSomePatientInfoSelectedInFormGeneralAfterLoad (__Load.cs); tu luu phai CHO task nay xong.
        /// </summary>
        private Task afterLoadWarningTask;

        /// <summary>Khoi phuc trang thai o "Tu dong luu" tu ControlState (goi trong Load khi co chan dang bat).</summary>
        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = this.controlStateWorker.GetData(AUTO_SAVE_MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == this.chkAutoSave.Name)
                        {
                            this.chkAutoSave.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chi hien o tich khi mo tu luong thuc hien DVKT; man Ke don CLS thuong / Sua don thi an de khong gay hieu nham
        /// (tick ma khong co gi xay ra). Item an di thi emptySpaceItem3 tren cung hang tu gian ra lap cho.
        /// </summary>
        private void SetAutoSaveVisibility()
        {
            try
            {
                this.lciChkAutoSave.Visibility = this.isOpenFromServiceExecute
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            // Dang khoi phuc trang thai luc Load -> khong ghi de
            if (this.isNotLoadWhileChangeControlStateInFirst)
            {
                return;
            }
            try
            {
                if (this.controlStateWorker == null)
                {
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                }
                if (this.currentControlStateRDO == null)
                {
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                }
                var controlState = this.currentControlStateRDO
                    .FirstOrDefault(o => o.KEY == this.chkAutoSave.Name && o.MODULE_LINK == AUTO_SAVE_MODULE_LINK);
                if (controlState != null)
                {
                    controlState.VALUE = this.chkAutoSave.Checked ? "1" : "";
                }
                else
                {
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = this.chkAutoSave.Name,
                        VALUE = this.chkAutoSave.Checked ? "1" : "",
                        MODULE_LINK = AUTO_SAVE_MODULE_LINK
                    });
                }
                // SetData XOA moi KEY cung MODULE_LINK khong co trong list truyen vao -> luon truyen ca danh sach da GetData
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Goi o cuoi frmAssignPrescription_Load (sau WaitingManager.Hide). Du dieu kien thi tu Luu sau khi form hien
        /// xong (BeginInvoke) va sau khi 2 kiem tra async cua Load (no vien phi / tran BHYT) da xong - tuong duong bam nut
        /// Luu, nen moi kiem tra/hoi trong ProcessSaveData van chay: thieu ICD, canh bao MIMS... thi form van mo cho
        /// nguoi dung xu ly, chi bao them 1 alert nhe "chua tu luu duoc" de nguoi dung biet vi sao form con mo.
        /// Luoi co dong vuot ton kho (Load da canh bao) thi KHONG tu luu de khoi hien 2 hop canh bao lien tiep.
        /// </summary>
        private void TryAutoSaveAttachedMediMaty()
        {
            try
            {
                if (!this.isOpenFromServiceExecute) return;
                if (this.chkAutoSave == null || !this.chkAutoSave.Checked) return;
                if (this.actionType != GlobalVariables.ActionAdd) return;
                if (this.mediMatyTypeADOs == null || this.mediMatyTypeADOs.Count == 0)
                {
                    LogSystem.Info("56273 AutoSave: bo qua vi khong co thuoc/vat tu di kem nao tren luoi (kho mac dinh trong hoac dich vu chua cau hinh HIS_SERVICE_METY/MATY).");
                    return;
                }
                if (!this.btnSave.Enabled) return;
                // Cung dieu kien voi CheckAmoutWarringInStock trong ProcessSaveData: chac chan khong luu duoc, Load da hien canh bao
                if (this.mediMatyTypeADOs.Any(o => (o.AmountAlert ?? 0) > 0
                    || o.ErrorTypeMediMatyBean == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning))
                {
                    LogSystem.Info("56273 AutoSave: bo qua vi co dong vuot ton kho / co canh bao tren luoi - nguoi dung tu sua so luong roi bam Luu.");
                    Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, "", ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem);
                    return;
                }

                LogSystem.Info(String.Format("56273 AutoSave: tu luu {0} dong thuoc/vat tu di kem cua dich vu SERVICE_ID = {1} (SERE_SERV_ID = {2}).",
                    this.mediMatyTypeADOs.Count,
                    this.currentSereServ != null ? this.currentSereServ.SERVICE_ID : 0,
                    this.currentSereServ != null ? this.currentSereServ.ID : 0));
                this.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        // Cho 2 kiem tra async ban ra trong Load xong han (hop hoi "thieu vien phi"/"vuot tran BHYT" da duoc tra
                        // loi, form chua bi Close) roi moi luu - tranh luu xong roi hop hoi moi hien, hoac form bi Dispose
                        // trong luc ProcessSaveData dang chay. Task da xong thi await tra ve ngay.
                        if (this.afterLoadWarningTask != null)
                        {
                            await this.afterLoadWarningTask;
                        }
                        // Form co the da bi dong trong Load (thieu vien phi / canh bao BHYT chon Khong)
                        if (this.IsDisposed || !this.Visible) return;
                        if (this.actionType != GlobalVariables.ActionAdd || !this.btnSave.Enabled) return;
                        this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE.SAVE);
                        // Luu thanh cong -> actionType = ActionView. Con ActionAdd = bi chan boi kiem tra/xac nhan trong ProcessSaveData
                        // (thieu ICD, MIMS, tuong tac...) -> bao nhe de nguoi dung biet form con mo vi chua luu, khong lam gi them.
                        if (!this.IsDisposed && this.actionType == GlobalVariables.ActionAdd)
                        {
                            LogSystem.Info("56273 AutoSave: ProcessSaveData khong luu duoc (bi chan boi kiem tra/xac nhan) - giu form mo de nguoi dung xu ly.");
                            Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, "", ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
