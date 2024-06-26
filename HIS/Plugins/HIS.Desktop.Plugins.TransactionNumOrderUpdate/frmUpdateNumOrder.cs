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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TransactionNumOrderUpdate.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionNumOrderUpdate
{
    public partial class frmUpdateNumOrder : FormBase
    {
        private V_HIS_TRANSACTION transaction = null;
        private int positionHandleControl = -1;

        public frmUpdateNumOrder(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_TRANSACTION data)
            : base(moduleData)
        {
            InitializeComponent();
            this.transaction = data;
        }

        private void frmUpdateNumOrder_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();                
                if (this.transaction == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Giao dịch không hợp lệ", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    this.Close();
                }

                if (this.transaction != null && this.transaction.IS_NOT_GEN_TRANSACTION_ORDER != 1)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Số biên lai (hóa đơn) tự động sinh số. Không cho phép sửa", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    this.Close();
                }
                this.SetValueControl();
                this.ValidControl();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControl()
        {
            try
            {
                if (this.transaction != null)
                {
                    lblTransactionCode.Text = this.transaction.TRANSACTION_CODE ?? "";
                    lblTransactionType.Text = this.transaction.TRANSACTION_TYPE_NAME ?? "";
                    lblTreatmentCode.Text = this.transaction.TREATMENT_CODE ?? "";
                    lblPatientName.Text = this.transaction.TDL_PATIENT_NAME ?? "";
                    lblGenderName.Text = this.transaction.TDL_PATIENT_GENDER_NAME ?? "";
                    if (this.transaction.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblPatientDob.Text = this.transaction.TDL_PATIENT_DOB.HasValue ? this.transaction.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4) : "";
                    }
                    else
                    {
                        lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.transaction.TDL_PATIENT_DOB ?? 0);
                    }
                    spinNumOrder.Value = this.transaction.NUM_ORDER;
                }
                else
                {
                    btnSave.Enabled = false;
                    spinNumOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                NumOrderValidationRule rule = new NumOrderValidationRule();
                rule.oldNumOder = this.transaction != null ? this.transaction.NUM_ORDER : 0;
                rule.spinNumOrder = spinNumOrder;
                dxValidationProvider1.SetValidationRule(spinNumOrder, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || this.transaction == null) return;
                if (!dxValidationProvider1.Validate()) return;
                btnSave.Enabled = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                HIS_TRANSACTION data = Mapper.Map<HIS_TRANSACTION>(this.transaction);
                data.NUM_ORDER = (long)spinNumOrder.Value;
                var rs = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/UpdateNumOrder", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    this.transaction.NUM_ORDER = rs.NUM_ORDER;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                if (success)
                {
                    this.Close();
                }
                else
                {
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
