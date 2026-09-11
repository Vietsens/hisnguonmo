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
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// CAU NOI GIUA HAI MUC DU LIEU.
    ///
    /// VAN DE:
    ///   Man hinh cu "Xu ly yeu cau kham/cls/pttt" co luoi o muc Y LENH,
    ///   dong luoi la ServiceReqADO : L_HIS_SERVICE_REQ.
    ///   Man hinh nay co luoi o muc DICH VU (vi So GPBL, Ket luan, Kip deu nam
    ///   o muc dich vu), dong luoi la PaanSereServADO : V_HIS_SERE_SERV_GPBL.
    ///
    ///   Toan bo cac nut va menu chuot phai chep tu man cu deu nhan tham so
    ///   kieu L_HIS_SERVICE_REQ. Neu sua het chu ky cua chung thi phai dong vao
    ///   hang nghin dong, rat de sai.
    ///
    /// CACH GIAI QUYET:
    ///   Giu nguyen code chep tu man cu. Khi nguoi dung bam nut hoac chuot phai,
    ///   lay ban ghi L_HIS_SERVICE_REQ tuong ung qua API roi truyen vao.
    ///   Da doi chieu: ca 17 truong ma cac nut/menu can (TREATMENT_ID,
    ///   TDL_TREATMENT_CODE, SERVICE_REQ_STT_ID, EXE_SERVICE_MODULE_ID,
    ///   TDL_PATIENT_CLASSIFY_ID, ...) deu co san tren L_HIS_SERVICE_REQ.
    ///
    ///   Co bo nho dem trong phien de khong goi API lap lai khi nguoi dung
    ///   bam nhieu lan tren cung mot dong.
    /// </summary>
    public partial class UCPaanExecuteList
    {
        /// <summary>
        /// Bo nho dem y lenh da lay, theo SERVICE_REQ_ID.
        /// Xoa moi khi nap lai luoi de tranh dung du lieu cu.
        /// </summary>
        private Dictionary<long, L_HIS_SERVICE_REQ> serviceReqCache = new Dictionary<long, L_HIS_SERVICE_REQ>();

        /// <summary>Y lenh cua dong dang duoc chuot phai / dang chon.</summary>
        private L_HIS_SERVICE_REQ currentServiceReq = null;

        /// <summary>
        /// Lay y lenh day du tuong ung voi dong dang chon tren luoi.
        /// Tra ve null neu khong lay duoc (da ghi log, da bao nguoi dung).
        /// </summary>
        private L_HIS_SERVICE_REQ GetServiceReqOfFocusedRow(bool showMessageIfNull)
        {
            try
            {
                PaanSereServADO row = GetFocusedRow();
                if (row == null)
                {
                    if (showMessageIfNull)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "Vui lòng chọn một dòng trong danh sách!", "Thông báo",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                    return null;
                }

                if (!row.SERVICE_REQ_ID.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SERVICE_REQ_ID rong tai SERE_SERV_ID = " + row.ID);
                    return null;
                }

                return GetServiceReqById(row.SERVICE_REQ_ID.Value, showMessageIfNull);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Lay mot y lenh theo ID, uu tien lay trong bo nho dem.
        /// </summary>
        private L_HIS_SERVICE_REQ GetServiceReqById(long serviceReqId, bool showMessageIfNull)
        {
            try
            {
                if (serviceReqCache.ContainsKey(serviceReqId))
                {
                    return serviceReqCache[serviceReqId];
                }

                CommonParam param = new CommonParam();
                HisServiceReqLViewFilter filter = new HisServiceReqLViewFilter();
                filter.ID = serviceReqId;

                var data = new BackendAdapter(param).Get<List<L_HIS_SERVICE_REQ>>(
                    PaanRequestUriStore.HIS_SERVICE_REQ_GET_LVIEW,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                L_HIS_SERVICE_REQ result = (data != null) ? data.FirstOrDefault() : null;

                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong lay duoc y lenh co ID = " + serviceReqId);
                    if (showMessageIfNull)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "Không lấy được thông tin y lệnh!", "Thông báo",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                    return null;
                }

                serviceReqCache[serviceReqId] = result;
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Lay y lenh cua TAT CA cac dong dang duoc tich chon tren luoi.
        /// Phuc vu cac chuc nang lam viec tren nhieu dong
        /// (Tra ket qua tong hop, Ke thuoc/vat tu, Chon may xu ly).
        /// </summary>
        private List<L_HIS_SERVICE_REQ> GetServiceReqOfSelectedRows()
        {
            List<L_HIS_SERVICE_REQ> result = new List<L_HIS_SERVICE_REQ>();
            try
            {
                if (gridViewPaan == null) return result;

                int[] handles = gridViewPaan.GetSelectedRows();
                if (handles == null || handles.Length == 0) return result;

                // Mot y lenh co the co nhieu dich vu tren luoi, phai loc trung.
                List<long> seen = new List<long>();

                foreach (int h in handles)
                {
                    if (h < 0) continue;

                    PaanSereServADO row = gridViewPaan.GetRow(h) as PaanSereServADO;
                    if (row == null || !row.SERVICE_REQ_ID.HasValue) continue;

                    long id = row.SERVICE_REQ_ID.Value;
                    if (seen.Contains(id)) continue;
                    seen.Add(id);

                    L_HIS_SERVICE_REQ sr = GetServiceReqById(id, false);
                    if (sr != null) result.Add(sr);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Xoa bo nho dem. Goi moi khi nap lai luoi.
        /// </summary>
        private void ClearServiceReqCache()
        {
            try
            {
                if (serviceReqCache != null) serviceReqCache.Clear();
                currentServiceReq = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
