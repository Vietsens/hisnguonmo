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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Subscribe Click handler cho btnCopy + auto-update Enabled state
        /// theo materialTypeId. Goi trong Load event sau khi SetDataToControl chay.
        /// </summary>
        private void WireBtnCopy()
        {
            try
            {
                this.btnCopy.Click -= btnCopy_Click;
                this.btnCopy.Click += btnCopy_Click;

                this.cboMaterialType.EditValueChanged += cboMaterialType_UpdateBtnCopyState;
                this.btnRefresh.Click += btn_UpdateBtnCopyState;
                this.btnSave.Click += btn_UpdateBtnCopyState;

                UpdateBtnCopyState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboMaterialType_UpdateBtnCopyState(object sender, EventArgs e)
        {
            UpdateBtnCopyState();
        }

        private void btn_UpdateBtnCopyState(object sender, EventArgs e)
        {
            UpdateBtnCopyState();
        }

        /// <summary>
        /// Enable btnCopy khi:
        ///   - Form mo o che do Sua (materialTypeId co value).
        ///   - Da chon vat tu mau qua cboMaterialType (cboMaterialType_EditValueChanged set materialTypeId).
        ///   - Sau khi Save thanh cong:
        ///       * Edit mode: materialTypeId giu nguyen id cu => can cu vao materialTypeId.
        ///       * Add mode:  Material's btnSave_Click KHONG re-assign materialTypeId = resultData.ID
        ///                    nen phai fallback qua resultData (khac voi Medicine flow).
        /// Disable khi: form trong, sau Refresh, hoac sau Copy (btnCopy_Click set ca 2 ve null).
        /// </summary>
        private void UpdateBtnCopyState()
        {
            try
            {
                bool hasMaterialId = this.materialTypeId.HasValue && this.materialTypeId.Value > 0;
                bool hasResultId = this.resultData != null && this.resultData.ID > 0;
                btnCopy.Enabled = hasMaterialId || hasResultId;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sao chep ban ghi hien tai sang ban ghi moi:
        ///   - Giu nguyen toan bo gia tri tren form.
        ///   - Reset context: ID, currentVHis*DTO, ActionType -> ActionAdd.
        ///   - Clear cboMaterialType + txtMaterialType.
        ///   - Reset ID + SERVICE_ID cua chinh sach gia / doi tuong de Save tao ban ghi moi.
        ///   - Reset oldBlock*Ids / oldMaterialTypeMapIds de SaveBlock* tao quan he moi.
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControlMedicineTypeInfo = -1;
                this.materialTypeId = null;
                this.currentVHisMaterialTypeDTODefault = null;
                this.currentVHisServiceDTODefault = null;
                this.HisMaterial = null;
                this.resultData = null;
                this.ActionType = GlobalVariables.ActionAdd;

                ResetTemplateSelectorControls();
                ResetEditModeButtons();
                ResetServicePatyForCopy();
                ResetDepaPatientTypesForCopy();
                ResetBlockAndMapOldIds();

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
                txtMaterialType.Text = "";
                // PTTK 42762: Sau Sao chep -> ve Add mode, combo van enable de chon template khac
                txtMaterialType.Enabled = true;

                this.cboMaterialType.EditValueChanged -= cboMaterialType_EditValueChanged;
                this.cboMaterialType.EditValueChanged -= cboMaterialType_UpdateBtnCopyState;
                try
                {
                    cboMaterialType.EditValue = null;
                }
                finally
                {
                    this.cboMaterialType.EditValueChanged += cboMaterialType_EditValueChanged;
                    this.cboMaterialType.EditValueChanged += cboMaterialType_UpdateBtnCopyState;
                }
                cboMaterialType.Enabled = true;
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
                rdoUpdateAll.Enabled = false;
                rdoUpdateNotFee.Enabled = false;
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
                this.lsVHisServicePatyBegin = new List<ADO.VHisServicePatyADO>();
                this.ServicePatyCreate = null;
                this.ServicePatyUpdate = null;
                var _ = FillDataToGridConrolServicePaty();
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

        private void ResetBlockAndMapOldIds()
        {
            try
            {
                this.oldBlockDepartmentIds = null;
                this.oldBlockRoomIds = null;
                this.oldMaterialTypeMapIds = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
