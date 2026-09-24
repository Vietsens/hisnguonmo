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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.OutStockPresQr
{
    /// <summary>
    /// Tao yeu cau thanh toan cho phan thuoc/vat tu mua ngoai ngay sau khi luu don,
    /// lam co so cap ma QR in len don thuoc.
    ///
    /// KHONG rang buoc ngan hang nao: o day chi tao yeu cau thanh toan. Viec cap chuoi ma QR
    /// do thu vien in dam nhiem, dua tren nhom cau hinh HIS.Desktop.Plugins.PaymentQrCode.*
    /// - doi ngan hang chi can doi cau hinh, khong sua ma nguon.
    ///
    /// Nghiep vu nay KHONG duoc phep chan viec luu don: moi loi deu chi ghi nhat ky,
    /// khong hien thong bao, khong nem ngoai le ra ngoai.
    /// </summary>
    class OutStockPresQrProcessor
    {
        /// <summary>
        /// Tra ve yeu cau thanh toan vua tao, hoac null neu khong du dieu kien.
        /// </summary>
        internal static HIS_TRANS_REQ Create(OutPatientPresResultSDO result, long? drugStoreId, long reqRoomId)
        {
            try
            {
                if (result == null || !drugStoreId.HasValue || drugStoreId.Value <= 0)
                {
                    return null;
                }

                long serviceReqId = GetOutStockServiceReqId(result);
                if (serviceReqId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Don khong co hang mua ngoai. Khong tao yeu cau thanh toan.");
                    return null;
                }

                OutStockPresTransReqInput sdo = new OutStockPresTransReqInput();
                sdo.ServiceReqId = serviceReqId;
                sdo.DrugStoreId = drugStoreId.Value;
                sdo.ReqRoomId = reqRoomId;

                CommonParam param = new CommonParam();
                HIS_TRANS_REQ transReq = new BackendAdapter(param).Post<HIS_TRANS_REQ>(
                    RequestUriStore.HIS_TRANS_REQ__CREATE_BY_OUT_STOCK_PRESCRIPTION,
                    ApiConsumers.MosConsumer, sdo, param);

                if (transReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong tao duoc yeu cau thanh toan cho don thuoc mua ngoai. Don van luu binh thuong.");
                    return null;
                }

                //Gan lien ket vao ket qua dang nam trong bo nho.
                //Thu vien in doc TRANS_REQ_ID tu day de lay ma QR dat len bieu in;
                //khong gan thi lan in ngay sau khi luu se khong co ma.
                UpdateTransReqIdInResult(result, serviceReqId, transReq.ID);

                Inventec.Common.Logging.LogSystem.Info("Da tao yeu cau thanh toan cho don thuoc mua ngoai. TRANS_REQ_CODE: " + transReq.TRANS_REQ_CODE);
                return transReq;
            }
            catch (Exception ex)
            {
                //Khong chan luu don
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Gan lien ket yeu cau thanh toan vao y lenh trong ket qua luu don,
        /// de thu vien in lay duoc ma QR ngay o lan in dau tien.
        /// </summary>
        private static void UpdateTransReqIdInResult(OutPatientPresResultSDO result, long serviceReqId, long transReqId)
        {
            try
            {
                if (result.ServiceReqs == null || result.ServiceReqs.Count == 0)
                {
                    return;
                }

                HIS_SERVICE_REQ serviceReq = result.ServiceReqs.FirstOrDefault(o => o.ID == serviceReqId);
                if (serviceReq != null)
                {
                    serviceReq.TRANS_REQ_ID = transReqId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Y lenh chua hang mua ngoai cua ket qua luu don.
        /// </summary>
        private static long GetOutStockServiceReqId(OutPatientPresResultSDO result)
        {
            List<long> serviceReqIds = new List<long>();

            if (result.ServiceReqMeties != null && result.ServiceReqMeties.Count > 0)
            {
                serviceReqIds.AddRange(result.ServiceReqMeties.Select(o => o.SERVICE_REQ_ID));
            }

            if (result.ServiceReqMaties != null && result.ServiceReqMaties.Count > 0)
            {
                serviceReqIds.AddRange(result.ServiceReqMaties.Select(o => o.SERVICE_REQ_ID));
            }

            serviceReqIds = serviceReqIds.Distinct().ToList();

            if (serviceReqIds.Count > 1)
            {
                Inventec.Common.Logging.LogSystem.Warn("Ket qua luu don co nhieu hon mot y lenh chua hang mua ngoai. Chi tao ma QR cho y lenh dau tien.");
            }

            return serviceReqIds.Count > 0 ? serviceReqIds.First() : 0;
        }

        /// <summary>
        /// Du lieu gui len may chu. Khai bao tai cho de khong phu thuoc vao thu vien dung chung
        /// phai dung lai truoc khi bien dich plugin nay.
        /// Ten thuoc tinh phai trung voi doi tuong dau vao cua may chu.
        /// </summary>
        private class OutStockPresTransReqInput
        {
            public long ServiceReqId { get; set; }
            public long DrugStoreId { get; set; }
            public long ReqRoomId { get; set; }
        }
    }
}
