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

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    /// <summary>
    /// Viec 3082: sau khi HUY HOA DON (huy giao dich + HDDT) cua phieu xuat ban da tu dong thuc xuat,
    /// he thong tu dong goi api/HisExpMest/Unexport (hoan kho) roi api/HisExpMest/Unapprove (huy duyet)
    /// de phieu tro ve trang thai YEU CAU (vang) — vien tu xoa phieu bang tay sau.
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
        /// Neu config bat: voi tung phieu xuat ban cua giao dich vua huy —
        /// phieu dang HOAN THANH (da thuc xuat) thi Unexport (hoan kho),
        /// sau do phieu dang DA DUYET thi Unapprove de ve trang thai YEU CAU.
        /// Khong xoa phieu (theo tai lieu 3082: vien tu xoa bang tay).
        /// </summary>
        internal static void RestoreAfterCancelInvoice(Form ownerForm, List<string> expMestCodes, long reqRoomId)
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
                RestoreAfterCancelInvoice(ownerForm, filter, reqRoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Overload theo ma giao dich (BILL_ID) — dung cho man Danh sach giao dich,
        /// noi dong luoi khong co san ma phieu xuat.
        /// </summary>
        internal static void RestoreAfterCancelInvoiceByBillId(Form ownerForm, long billId, long reqRoomId)
        {
            try
            {
                if (!IsEnabled)
                    return;
                if (billId <= 0)
                    return;

                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                filter.BILL_ID = billId;
                RestoreAfterCancelInvoice(ownerForm, filter, reqRoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void RestoreAfterCancelInvoice(Form ownerForm, HisExpMestViewFilter filter, long reqRoomId)
        {
            try
            {
                var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, null);
                var needProcesses = (expMests ?? new List<V_HIS_EXP_MEST>())
                    .Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    .Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                        || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    .ToList();
                if (needProcesses.Count == 0)
                    return;

                CommonParam param = new CommonParam();
                bool success = true;
                WaitingManager.Show();
                foreach (var expMest in needProcesses)
                {
                    long currentSttId = expMest.EXP_MEST_STT_ID;

                    // 1. Huy thuc xuat -> hoan lai so luong vao kho
                    if (currentSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        HisExpMestSDO unexportSdo = new HisExpMestSDO();
                        unexportSdo.ExpMestId = expMest.ID;
                        unexportSdo.ReqRoomId = reqRoomId;
                        Inventec.Common.Logging.LogSystem.Info("RestoreAfterCancelInvoice: Call API api/HisExpMest/Unexport"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => unexportSdo), unexportSdo));
                        var unexportResult = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unexport", ApiConsumers.MosConsumer, unexportSdo, param);
                        if (unexportResult == null)
                        {
                            success = false;
                            break;
                        }
                        // Response cua Unexport khong phan anh trang thai moi -> doc lai trang thai that tu server
                        currentSttId = GetCurrentSttId(expMest.ID, unexportResult.EXP_MEST_STT_ID);
                    }

                    // 2. Huy duyet -> phieu ve trang thai YEU CAU (vang)
                    if (currentSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        HisExpMestSDO unapproveSdo = new HisExpMestSDO();
                        unapproveSdo.ExpMestId = expMest.ID;
                        unapproveSdo.ReqRoomId = reqRoomId;
                        Inventec.Common.Logging.LogSystem.Info("RestoreAfterCancelInvoice: Call API api/HisExpMest/Unapprove"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => unapproveSdo), unapproveSdo));
                        var unapproveResult = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumers.MosConsumer, unapproveSdo, param);
                        if (unapproveResult == null)
                        {
                            success = false;
                            break;
                        }
                        GetCurrentSttId(expMest.ID, 0);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info(String.Format(
                            "RestoreAfterCancelInvoice: phieu {0} dang o trang thai {1}, khong goi Unapprove",
                            expMest.EXP_MEST_CODE, currentSttId));
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

        /// <summary>
        /// Doc lai trang thai hien tai cua phieu tu server (sau khi goi Unexport).
        /// Tra ve defaultSttId neu khong doc duoc.
        /// </summary>
        private static long GetCurrentSttId(long expMestId, long defaultSttId)
        {
            try
            {
                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                filter.ID = expMestId;
                var expMest = new BackendAdapter(new CommonParam())
                    .Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, null)
                    .FirstOrDefault();
                if (expMest != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(String.Format(
                        "RestoreAfterCancelInvoice: trang thai phieu {0} sau Unexport = {1} ({2})",
                        expMest.EXP_MEST_CODE, expMest.EXP_MEST_STT_ID, expMest.EXP_MEST_STT_NAME));
                    return expMest.EXP_MEST_STT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultSttId;
        }
    }
}
