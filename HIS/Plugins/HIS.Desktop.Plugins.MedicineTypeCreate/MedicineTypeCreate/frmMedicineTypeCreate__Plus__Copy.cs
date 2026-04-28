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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.MedicineTypeCreate
{
    public partial class frmMedicineTypeCreate : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Subscribe Click handler cho btnCopy + auto-update Enabled state
        /// theo currentMedicineTypeId. Goi trong Load event sau khi FillDataMedicineTypeToControl chay.
        /// </summary>
        private void WireBtnCopy()
        {
            try
            {
                this.btnCopy.Click -= btnCopy_Click;
                this.btnCopy.Click += btnCopy_Click;

                this.cboMedicineType.EditValueChanged += cboMedicineType_UpdateBtnCopyState;
                this.btnRefresh.Click += btn_UpdateBtnCopyState;
                this.btnSave.Click += btn_UpdateBtnCopyState;

                UpdateBtnCopyState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_UpdateBtnCopyState(object sender, EventArgs e)
        {
            UpdateBtnCopyState();
        }

        private void btn_UpdateBtnCopyState(object sender, EventArgs e)
        {
            UpdateBtnCopyState();
        }

        /// <summary>
        /// Enable btnCopy khi:
        ///   - Form mo o che do Sua (currentMedicineTypeId co value).
        ///   - Da chon thuoc mau qua cboMedicineType (currentMedicineTypeId duoc set boi cboMedicineType_EditValueChanged).
        ///   - Sau khi Save thanh cong (currentMedicineTypeId = resultData.ID).
        /// Disable khi: form trong (Add mode, chua chon thuoc mau) hoac sau Refresh / sau Copy.
        /// </summary>
        private void UpdateBtnCopyState()
        {
            try
            {
                bool canCopy = this.currentMedicineTypeId.HasValue && this.currentMedicineTypeId.Value > 0;
                btnCopy.Enabled = canCopy;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sao chep ban ghi hien tai sang ban ghi moi:
        ///   - Giu nguyen toan bo gia tri tren form (chinh sach gia, doi tuong, kho chan, chong chi dinh, ...).
        ///   - Reset context: ID, currentVHis*DTO, ActionType -> ActionAdd.
        ///   - Clear cboMedicineType + txtMedicineType (chon thuoc mau).
        ///   - Reset ID + SERVICE_ID cua chinh sach gia / doi tuong de Save tao ban ghi moi.
        ///   - Reset oldBlock*Ids / oldContraindicationSelecteds de SaveBlock* tao quan he moi.
        /// Sau khi Sao chep: nguoi dung sua Ma/Ten roi nhan Luu se Create record moi.
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControlMedicineTypeInfo = -1;
                this.currentMedicineTypeId = null;
                this.currentVHisMedicineTypeDTODefault = null;
                this.currentVHisServiceDTODefault = null;
                this.resultData = null;
                this.ActionType = GlobalVariables.ActionAdd;

                ResetTemplateSelectorControls();
                ResetEditModeButtons();
                ResetServicePatyForCopy();
                ResetDepaPatientTypesForCopy();
                ResetBlockAndContraindicationOldIds();

                btnSave.Enabled = true;
                btnRefresh.Enabled = true;

                UpdateBtnCopyState();

                if (txtMedicineTypeCode.Enabled)
                {
                    txtMedicineTypeCode.Focus();
                    txtMedicineTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetTemplateSelectorControls()
        {
            try
            {
                txtMedicineType.Text = "";
                txtMedicineType.Enabled = false;

                this.cboMedicineType.EditValueChanged -= cboMedicineType_EditValueChanged;
                this.cboMedicineType.EditValueChanged -= cboMedicineType_UpdateBtnCopyState;
                try
                {
                    cboMedicineType.EditValue = null;
                }
                finally
                {
                    this.cboMedicineType.EditValueChanged += cboMedicineType_EditValueChanged;
                    this.cboMedicineType.EditValueChanged += cboMedicineType_UpdateBtnCopyState;
                }
                cboMedicineType.Enabled = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetEditModeButtons()
        {
            try
            {
                btnDieuChinhLieu.Enabled = false;
                btnEditInfo.Enabled = false;
                rdoUpdateAll.ReadOnly = true;
                rdoUpdateNotFee.ReadOnly = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetServicePatyForCopy()
        {
            try
            {
                if (this.lsVHisServicePaty != null)
                {
                    foreach (var item in this.lsVHisServicePaty)
                    {
                        item.ID = 0;
                        item.SERVICE_ID = 0;
                        item.Action = GlobalVariables.ActionAdd;
                    }
                }
                this.lsVHisServicePatyBegin = new System.Collections.Generic.List<ADO.VHisServicePatyADO>();
                this.ServicePatyCreate = null;
                this.ServicePatyUpdate = null;
                this.patientServicePatyError = null;
                FillDataToGridConrolServicePaty();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetDepaPatientTypesForCopy()
        {
            try
            {
                if (this.depaPatientTypes != null)
                {
                    foreach (var item in this.depaPatientTypes)
                    {
                        item.ID = 0;
                        item.SERVICE_ID = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetBlockAndContraindicationOldIds()
        {
            try
            {
                this.oldBlockDepartmentIds = null;
                this.oldBlockRoomIds = null;
                this.oldContraindicationSelecteds = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
