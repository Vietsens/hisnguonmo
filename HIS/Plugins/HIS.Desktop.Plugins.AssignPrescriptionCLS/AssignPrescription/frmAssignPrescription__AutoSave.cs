/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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
    ///   form hien xong VA 3 tac vu async cua Load (thuoc da ke trong ngay, no vien phi, tran BHYT) da xong, nguoi dung
    ///   khong chon dong form o 2 hop hoi, va dich vu CHUA co thuoc/vat tu di kem (tranh xuat kho trung khi mo Tu truc lan 2)
    ///   - dung nhu nguoi dung bam Luu.
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
        /// Task gop 3 tac vu async ban ra trong Load: LoadDataSereServWithTreatment (thuoc da ke trong ngay - can cho canh bao
        /// khang sinh/trung thuoc trong ProcessSaveData), CheckWarningOverTotalPatientPrice (no vien phi) va
        /// LoadTotalSereServByHeinWithTreatment (tran BHYT; 2 cai sau co the hoi YesNo roi this.Close()).
        /// Gan o FillSomePatientInfoSelectedInFormGeneralAfterLoad (__Load.cs); tu luu phai CHO task nay xong.
        /// </summary>
        private Task afterLoadWarningTask;
        /// <summary>
        /// Mot trong 2 kiem tra async cua Load (no vien phi / tran BHYT) da yeu cau dong form (nguoi dung chon Khong).
        /// this.Close() co the bi FormClosing huy (hop hoi "thuoc chua luu") nen khong the chi dua vao IsDisposed/Visible:
        /// co nay dat TRUOC this.Close() o __Load.cs, tu luu kiem sau khi await -> khong luu cai don nguoi dung vua tu choi.
        /// </summary>
        private bool isCancelledByAfterLoadWarning;
        /// <summary>
        /// Lan tu luu gan nhat: ProcessSaveData da qua het kiem tra va thuc su goi backend (isave.Run, dat o __Save.cs).
        /// false = bi chan o khau kiem tra (thieu ICD, MIMS, ton kho...) -> bao alert "chua tu luu duoc";
        /// true = backend da tra loi (thanh cong hoac loi) va MessageManager.Show da hien -> khong bao chong them.
        /// </summary>
        private bool lastSaveReachedBackend;
        /// <summary>Dang trong loi goi ProcessSaveData do TU LUU phat ra (de ProcessSaveData phan biet voi bam Luu tay).</summary>
        private bool isAutoSaveCalling;
        /// <summary>
        /// Nguoi dung da bam Luu tay (bat ky nut luu nao) trong luc tu luu con dang cho cac tac vu Load -> tu luu KHONG goi
        /// ProcessSaveData lan 2 (tranh hien lai cac hop hoi nguoi dung vua tra loi). Gan o dau ProcessSaveData khi !isAutoSaveCalling.
        /// </summary>
        private bool isManualSaveAttempted;

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
        /// Luoi co dong vuot ton kho (Load chi to do tren luoi, CheckAmoutWarringInStock chac chan se chan) thi KHONG goi luu,
        /// chi bao alert nhe. Dich vu DA CO thuoc/vat tu di kem con hieu luc (mo Tu truc lan 2) cung KHONG tu luu de tranh xuat kho trung.
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
                // Dung bieu thuc cua CheckAmoutWarringInStock (__ValidDataForSave.cs, ke ca bo loc DataType): chac chan se bi chan khi luu
                if (this.mediMatyTypeADOs.Any(o => ((o.AmountAlert ?? 0) > 0
                        || o.ErrorTypeMediMatyBean == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning)
                    && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                        || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)))
                {
                    LogSystem.Info("56273 AutoSave: bo qua vi co dong vuot ton kho / co canh bao tren luoi - nguoi dung tu sua so luong roi bam Luu.");
                    Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, "", ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem);
                    return;
                }

                LogSystem.Info(String.Format("56273 AutoSave: du dieu kien tu luu {0} dong thuoc/vat tu di kem cua dich vu SERVICE_ID = {1} (SERE_SERV_ID = {2}) - cho cac tac vu async cua Load xong.",
                    this.mediMatyTypeADOs.Count,
                    this.currentSereServ != null ? this.currentSereServ.SERVICE_ID : 0,
                    this.currentSereServ != null ? this.currentSereServ.ID : 0));
                this.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        // Cho 3 tac vu async ban ra trong Load xong han (thuoc da ke trong ngay da nap; hop hoi "thieu vien phi"/
                        // "vuot tran BHYT" da duoc tra loi) roi moi luu - tranh luu xong roi hop hoi moi hien, mat canh bao khang sinh
                        // trong ngay, hoac form bi Dispose trong luc ProcessSaveData dang chay. Task da xong thi await tra ve ngay.
                        if (this.afterLoadWarningTask != null)
                        {
                            await this.afterLoadWarningTask;
                        }
                        else
                        {
                            LogSystem.Warn("56273 AutoSave: afterLoadWarningTask == null (FillSomePatientInfoSelectedInFormGeneralAfterLoad loi truoc khi gan) - tu luu chay khong cho cac tac vu Load.");
                        }
                        // Nguoi dung chon Khong o hop hoi no vien phi / tran BHYT -> form dang dong, hoac lenh dong bi FormClosing huy
                        // (hop hoi "thuoc chua luu") -> tuyet doi khong luu cai don nguoi dung vua tu choi.
                        if (this.isCancelledByAfterLoadWarning)
                        {
                            LogSystem.Info("56273 AutoSave: bo qua vi canh bao sau Load (no vien phi / tran BHYT) yeu cau dong form.");
                            // FormClosing hoi "thuoc chua luu" va nguoi dung giu form lai -> form van mo: bao nhe de biet vi sao khong tu luu
                            if (!this.IsDisposed && this.Visible)
                            {
                                Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, "", ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem);
                            }
                            return;
                        }
                        if (this.IsDisposed || !this.Visible) return;
                        if (this.actionType != GlobalVariables.ActionAdd || !this.btnSave.Enabled) return;
                        // Nguoi dung da bam Luu tay trong luc cho -> khong goi lai (moi hop hoi/kiem tra da hien voi nguoi dung roi)
                        if (this.isManualSaveAttempted)
                        {
                            LogSystem.Info("56273 AutoSave: bo qua vi nguoi dung da bam Luu tay trong luc cho cac tac vu Load.");
                            return;
                        }
                        // Dich vu da co thuoc/vat tu di kem con hieu luc (mo Tu truc lan 2 cho cung dich vu) -> khong tu luu them de tranh xuat kho trung.
                        // null = khong kiem tra duoc (loi API) -> cung khong tu luu nhung bao dung su that: "chua tu luu duoc", khong noi "da co thuoc".
                        bool? hasAttached = this.HasAttachedMediMateAlready();
                        if (hasAttached != false)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, "",
                                hasAttached == true ? ResourceMessage.DichVuDaCoThuocVatTuDiKemKhongTuLuu : ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem);
                            return;
                        }

                        LogSystem.Info(String.Format("56273 AutoSave: goi ProcessSaveData(SAVE) tu luu {0} dong thuoc/vat tu di kem cua dich vu SERVICE_ID = {1} (SERE_SERV_ID = {2}).",
                            this.mediMatyTypeADOs.Count,
                            this.currentSereServ != null ? this.currentSereServ.SERVICE_ID : 0,
                            this.currentSereServ != null ? this.currentSereServ.ID : 0));
                        this.lastSaveReachedBackend = false;
                        this.isAutoSaveCalling = true;
                        try
                        {
                            this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE.SAVE);
                        }
                        finally
                        {
                            this.isAutoSaveCalling = false;
                        }
                        // Luu thanh cong -> actionType = ActionView. Con ActionAdd + chua toi backend = bi chan boi kiem tra/xac nhan trong
                        // ProcessSaveData (thieu ICD, MIMS, tuong tac...) -> bao nhe de nguoi dung biet form con mo vi chua luu.
                        // Da toi backend ma loi thi MessageManager.Show trong ProcessSaveData da hien loi, khong bao chong them.
                        if (!this.IsDisposed && this.actionType == GlobalVariables.ActionAdd && !this.lastSaveReachedBackend)
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

        /// <summary>
        /// Dich vu dang thuc hien da co thuoc/vat tu di kem con hieu luc chua: HIS_SERE_SERV con voi PARENT_ID = SereServ.ID,
        /// loai Thuoc/Vat tu, khong IS_DELETE, khong IS_NO_EXECUTE - CUNG dinh nghia voi thu vien CheckRequireMediMate (viec 57799)
        /// de 2 viec nhin thay cung mot su that. PARENT_ID lay qua GetSereServInKip() - cung nguon voi luong luu (SaveCreateBehavior).
        /// Tra ve: true = da co; false = chua co; null = KHONG KIEM TRA DUOC (loi API/exception) -> nguoi goi khong tu luu (de khong lo
        /// xuat kho trung) nhung phai bao "chua tu luu duoc" chu khong duoc noi "da co thuoc".
        /// </summary>
        private bool? HasAttachedMediMateAlready()
        {
            try
            {
                long parentId = this.GetSereServInKip();
                if (parentId <= 0) return false;

                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.PARENT_IDs = new List<long>() { parentId };
                filter.TDL_SERVICE_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                };
                var children = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (children == null && param.HasException)
                {
                    LogSystem.Warn("56273 AutoSave: api/HisSereServ/Get loi khi kiem tra thuoc/vat tu di kem da co - khong tu luu."
                        + Inventec.Common.Logging.LogUtil.TraceData("SERE_SERV_ID", parentId)
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                    return null;
                }
                int existing = children != null ? children.Count(o => o != null && o.IS_DELETE != 1 && o.IS_NO_EXECUTE != 1) : 0;
                if (existing > 0)
                {
                    LogSystem.Info(String.Format("56273 AutoSave: bo qua vi dich vu SERE_SERV_ID = {0} da co {1} dong thuoc/vat tu di kem con hieu luc (tranh xuat kho trung).",
                        parentId, existing));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        #endregion
    }
}
