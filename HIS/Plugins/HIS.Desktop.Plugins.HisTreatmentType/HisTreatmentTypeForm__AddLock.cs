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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentType
{
    /// <summary>
    /// PT-48590: bo sung thao tac Them va Khoa / mo khoa cho danh muc Dien dieu tri.
    /// </summary>
    public partial class HisTreatmentTypeForm
    {
        #region Declare PT-48590

        private EnumTreatmentTypeFormMode formMode = EnumTreatmentTypeFormMode.Add;
        private bool hasRightAdd;
        private bool hasRightChangeLock;

        /// <summary>
        /// PT-48590 R5: tap ban ghi goc do phan mem cung cap san — khong cho khoa.
        /// Tieu chi la "do phan mem cung cap", khong phai "logic nghiep vu co tham chieu".
        /// </summary>
        private static readonly List<long> ORIGIN_TREATMENT_TYPE_IDS = new List<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM,
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU,
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU,
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY,
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA,
            IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC
        };

        #endregion

        #region Quy uoc trang thai va ban ghi goc

        /// <summary>
        /// PT-48590 R20: CHI ban ghi co trang thai dung bang "dang dung" moi duoc coi la dang dung.
        /// "Da khoa" HOAC rong deu coi nhu da khoa. Khong duoc viet dang phu dinh.
        /// </summary>
        private bool IsInUse(HIS_TREATMENT_TYPE data)
        {
            return data != null && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        }

        private bool IsOriginTreatmentType(HIS_TREATMENT_TYPE data)
        {
            return data != null && ORIGIN_TREATMENT_TYPE_IDS.Contains(data.ID);
        }

        #endregion

        #region Phan quyen cap nut

        /// <summary>
        /// PT-48590 R1: doc quyen tu du lieu quyen da nap san cua phien dang nhap.
        /// Tai khoan toan quyen duoc dung ngay ke ca khi chua khai bao ma dieu khien (B.2.3).
        /// </summary>
        private bool CheckRight(string controlCode)
        {
            bool result = false;
            try
            {
                var acs = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.AcsAuthorizeSDO;
                if (acs == null) return false;
                if (acs.IsFull) return true;
                if (acs.ControlInRoles != null)
                {
                    result = acs.ControlInRoles.Any(o => o.CONTROL_CODE == controlCode);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// PT-48590 B.4.1.1: goi CUOI CUNG trong luong khoi tao man hinh.
        /// Khong co quyen Them thi vao thang che do Sua de nguoi dung khong gap man hinh trang.
        /// </summary>
        private void InitControlRight()
        {
            try
            {
                hasRightAdd = CheckRight(ControlCode.BtnAdd);
                hasRightChangeLock = CheckRight(ControlCode.BtnChangeLock);

                SetFormMode(hasRightAdd
                    ? EnumTreatmentTypeFormMode.Add
                    : EnumTreatmentTypeFormMode.Edit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Che do Them / Sua

        private void SetFormMode(EnumTreatmentTypeFormMode mode)
        {
            try
            {
                this.formMode = mode;
                bool isAddMode = mode == EnumTreatmentTypeFormMode.Add;

                txtCode.Properties.ReadOnly = !isAddMode;
                txtName.Properties.ReadOnly = !isAddMode;

                btnAdd.Enabled = isAddMode && hasRightAdd;
                btnSave.Enabled = !isAddMode && this.currentData != null && IsInUse(this.currentData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// PT-48590 B.4.1.1 tinh huong 4: quay ve che do Them.
        /// BAT BUOC xoa ca ban ghi dang chon trong bo nho, neu khong thi bam Them se mang theo
        /// dinh danh dong cu va bien thanh Sua — ghi de ban ghi dang chon.
        /// </summary>
        private void BackToAddMode()
        {
            try
            {
                this.currentData = null;
                gridView1.ClearSelection();
                SetFormMode(hasRightAdd
                    ? EnumTreatmentTypeFormMode.Add
                    : EnumTreatmentTypeFormMode.Edit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Them moi

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddProcess();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (!btnAdd.Enabled) return;
                if (!dxValidationProvider1.Validate()) return;

                WaitingManager.Show();

                // Nhanh Them dung doi tuong trang, KHONG nap lai ban ghi goc theo dinh danh nhu nhanh Sua.
                HIS_TREATMENT_TYPE createDTO = new HIS_TREATMENT_TYPE();
                UpdateDTOFromDataForm(ref createDTO);
                createDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => createDTO), createDTO));

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT_TYPE>(
                    HisRequestUriStore.MOSHIS_TREATMENT_TYPE_CREATE,
                    ApiConsumers.MosConsumer, createDTO, param);

                bool success = resultData != null;
                if (success)
                {
                    BackendDataWorker.Reset<HIS_TREATMENT_TYPE>();
                    RefeshDataAfterSave(resultData);
                    ResetFormData();
                    BackToAddMode();
                    FillDatagctFormList();
                    SetFocusEditor();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Khoa / mo khoa

        private void btnChangeLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (!hasRightChangeLock) return;

                HIS_TREATMENT_TYPE data = gridView1.GetFocusedRow() as HIS_TREATMENT_TYPE;
                if (data == null) return;

                // R5: loai hinh goc dang dung thi khong co bieu tuong, nhung van chan lai cho chac.
                if (IsInUse(data) && IsOriginTreatmentType(data)) return;

                ChangeLockProcess(data);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeLockProcess(HIS_TREATMENT_TYPE data)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool isLockAction = IsInUse(data);

                var confirmMessage = isLockAction
                    ? LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong
                    : LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong;

                if (XtraMessageBox.Show(
                        MessageUtil.GetMessage(confirmMessage),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                WaitingManager.Show();

                // Dich vu dao trang thai khoa nhan DOI TUONG, chi can dat dinh danh.
                // Gui sai kieu se khong bao loi, chi im lang khong co tac dung.
                HIS_TREATMENT_TYPE changeLockDTO = new HIS_TREATMENT_TYPE();
                changeLockDTO.ID = data.ID;

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT_TYPE>(
                    HisRequestUriStore.MOSHIS_TREATMENT_TYPE_GROUP_CHANGE_LOCK,
                    ApiConsumers.MosConsumer, changeLockDTO, param);

                bool success = resultData != null;
                if (success)
                {
                    BackendDataWorker.Reset<HIS_TREATMENT_TYPE>();
                    FillDatagctFormList();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
