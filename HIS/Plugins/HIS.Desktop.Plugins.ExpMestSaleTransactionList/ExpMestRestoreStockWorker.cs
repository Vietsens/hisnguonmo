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
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList
{
    /// <summary>
    /// Viec 3082: khi huy giao dich/phieu xuat ban da TU DONG THUC XUAT (checkbox "In" + Luu ky),
    /// hoan lai so luong vao kho bang api/HisExpMest/Unexport truoc khi tiep tuc luong huy.
    /// Chi chay khi config SaveSignPrintAutoExport bat.
    /// </summary>
    internal static class ExpMestRestoreStockWorker
    {
        private const string CFG__SAVE_SIGN_PRINT_AUTO_EXPORT = "HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport";

        internal static bool IsEnabled
        {
            get
            {
                try
                {
                    return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CFG__SAVE_SIGN_PRINT_AUTO_EXPORT) == "1";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Neu config bat va phieu dang trang thai DA THUC XUAT (DONE) thi hoi xac nhan
        /// roi goi api/HisExpMest/Unexport hoan kho tung phieu.
        /// </summary>
        internal static void UnexportIfExported(Form ownerForm, List<string> expMestCodes, long reqRoomId)
        {
            try
            {
                if (!IsEnabled)
                    return;
                if (expMestCodes == null)
                    return;
                var codes = expMestCodes.Where(o => !String.IsNullOrWhiteSpace(o)).Select(o => o.Trim()).Distinct().ToList();
                if (codes.Count == 0)
                    return;

                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                filter.EXP_MEST_CODEs = codes;
                var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, null);
                var exporteds = (expMests ?? new List<V_HIS_EXP_MEST>())
                    .Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    .ToList();
                if (exporteds.Count == 0)
                    return;

                if (XtraMessageBox.Show(
                        String.Format("Phiếu xuất {0} đã thực xuất. Bạn có muốn hoàn lại số lượng vào kho không?",
                            String.Join(", ", exporteds.Select(o => o.EXP_MEST_CODE))),
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                CommonParam param = new CommonParam();
                bool success = true;
                WaitingManager.Show();
                foreach (var expMest in exporteds)
                {
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = expMest.ID;
                    sdo.ReqRoomId = reqRoomId;
                    Inventec.Common.Logging.LogSystem.Info("RestoreStock: Call API api/HisExpMest/Unexport"
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    var rs = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unexport", ApiConsumers.MosConsumer, sdo, param);
                    if (rs == null)
                    {
                        success = false;
                        break;
                    }
                }
                WaitingManager.Hide();
                if (!success)
                {
                    MessageManager.Show(ownerForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
