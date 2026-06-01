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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Danh mục lý do giao dịch (HIS_TRANSACTION_REASON) dùng cho cboTransactionReason.</summary>
        List<HIS_TRANSACTION_REASON> lstTransactionReason = null;

        /// <summary>
        /// Load danh mục Lý do giao dịch vào cboTransactionReason (GridLookUpEdit, rule FE-COMMON-01).
        /// IS_ACTIVE=1, OrderBy TRANSACTION_REASON_CODE. Cấu hình gridView6 chỉ hiện Mã + Tên.
        /// Lý do giao dịch dùng chung cho cả nhánh biên lai và nhánh hóa đơn.
        /// </summary>
        private void FillDataToReason()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTransactionReasonFilter filter = new HisTransactionReasonFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_FIELD = "TRANSACTION_REASON_CODE";
                filter.ORDER_DIRECTION = "ASC";

                lstTransactionReason = new BackendAdapter(param).Get<List<HIS_TRANSACTION_REASON>>(
                    "api/HisTransactionReason/Get", ApiConsumers.MosConsumer, filter, param);

                if (lstTransactionReason == null)
                    lstTransactionReason = new List<HIS_TRANSACTION_REASON>();

                this.gridView6.OptionsBehavior.AutoPopulateColumns = false;
                cboTransactionReason.Properties.DataSource = lstTransactionReason;
                cboTransactionReason.Properties.DisplayMember = "TRANSACTION_REASON_NAME";
                cboTransactionReason.Properties.ValueMember = "ID";
                cboTransactionReason.Properties.NullText = "";

                if (this.gridView6.Columns.Count == 0)
                {
                    var colCode = this.gridView6.Columns.AddVisible("TRANSACTION_REASON_CODE");
                    colCode.Caption = "Mã";
                    colCode.Width = 60;
                    var colName = this.gridView6.Columns.AddVisible("TRANSACTION_REASON_NAME");
                    colName.Caption = "Tên";
                    colName.Width = 150;
                }
                this.gridView6.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đặt mặc định cboTransactionReason theo diện điều trị hiện tại (rule FE-COMMON-03):
        /// - TDL_TREATMENT_TYPE_ID = HIS_TREATMENT_TYPE.ID__KHAM => Khám ngoại trú
        /// - Còn lại (đã vào điều trị / nội trú) => Điều trị
        /// - Không có treatment context => Khám (an toàn).
        /// Match record bằng TRANSACTION_REASON_NAME (insensitive) — danh mục do user quản lý.
        /// </summary>
        private void SetDefaultReasonByTreatment(V_HIS_TREATMENT_FEE treatmentFee)
        {
            try
            {
                if (lstTransactionReason == null || lstTransactionReason.Count == 0)
                    return;

                bool isExam = true;
                if (treatmentFee != null)
                {
                    long? typeId = treatmentFee.TDL_TREATMENT_TYPE_ID;
                    if (typeId.HasValue && typeId.Value != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        isExam = false;
                    }
                }

                string keyword = isExam ? "Khám" : "Điều trị";
                var matched = lstTransactionReason.FirstOrDefault(o =>
                    !string.IsNullOrEmpty(o.TRANSACTION_REASON_NAME)
                    && o.TRANSACTION_REASON_NAME.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matched != null)
                {
                    cboTransactionReason.EditValue = matched.ID;
                }
                else
                {
                    cboTransactionReason.EditValue = lstTransactionReason[0].ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
