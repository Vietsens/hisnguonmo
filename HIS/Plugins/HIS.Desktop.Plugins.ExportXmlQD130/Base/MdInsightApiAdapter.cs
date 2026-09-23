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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Goi hai giao dien nghiep vu cua HIS de luu va doc lai ket qua soat loi.
    ///
    /// Viec goi he thong MDInsight do <see cref="MdInsightWorker"/> dam nhiem - lop nay
    /// KHONG cham toi he ngoai, chi lam viec voi may chu HIS.
    ///
    /// ⚠️ Truong chi tiet loi chua ho ten, ma benh nhan va chan doan.
    /// Tuyet doi khong ghi noi dung tra ve ra nhat ky - quy tac QT-28.
    /// Tham chieu: PTTK muc B.3.2.1 va B.3.2.2.
    /// </summary>
    public static class MdInsightApiAdapter
    {
        private const string URI_UPDATE_RESULT = "api/HisTreatment/UpdateXmlPrecheckResult";
        private const string URI_GET_DETAIL = "api/HisTreatment/GetXmlPrecheckDetail";

        /// <summary>So ho so toi da moi luot ghi nhan - vuot thi tu chia nho (PTTK muc B.3.2.1)</summary>
        public const int MAX_SAVE_PER_CALL = 200;

        /// <summary>So ho so toi da moi luot doc - vuot thi tu chia nho (PTTK muc B.3.2.2)</summary>
        public const int MAX_READ_PER_CALL = 50;

        /// <summary>
        /// Ghi nhan ket qua cua mot nhom ho so. Tu chia nho khi vuot han muc moi luot goi.
        /// </summary>
        /// <param name="results">Ket qua da duoc tram tinh san (so luong loi, trang thai tong the)</param>
        /// <param name="failMessages">Nhan ve ly do cua nhung ho so ghi nhan that bai</param>
        /// <returns>True khi toan bo ho so deu duoc ghi nhan</returns>
        public static bool SaveResults(List<MdInsightResultADO> results, out Dictionary<long, string> failMessages)
        {
            failMessages = new Dictionary<long, string>();

            try
            {
                if (results == null || results.Count == 0)
                {
                    return true;
                }

                bool allSuccess = true;

                for (int i = 0; i < results.Count; i += MAX_SAVE_PER_CALL)
                {
                    int size = Math.Min(MAX_SAVE_PER_CALL, results.Count - i);
                    List<MdInsightResultADO> chunk = results.GetRange(i, size);

                    if (!SaveOneChunk(chunk, failMessages))
                    {
                        allSuccess = false;
                    }
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        private static bool SaveOneChunk(List<MdInsightResultADO> chunk, Dictionary<long, string> failMessages)
        {
            CommonParam param = new CommonParam();

            try
            {
                HisTreatmentUpdateXmlPrecheckResultSDO request = new HisTreatmentUpdateXmlPrecheckResultSDO
                {
                    XmlPrecheckResults = chunk.Select(ToResultADO).ToList()
                };

                //Nhat ky DAU VAO - CHI ghi ma dot dieu tri, trang thai va so dem.
                //TUYET DOI khong ghi truong chi tiet loi: no chua ho ten, ma benh nhan, chan doan (QT-28).
                if (LogSystem.IsDebugEnabled())
                {
                    LogSystem.Debug("MdInsight GUI DI___" + URI_UPDATE_RESULT + "___"
                        + chunk.Count + " ho so___"
                        + String.Join(", ", request.XmlPrecheckResults.Select(o =>
                            o.TreatmentId + ":tt=" + o.XmlPrecheckResult
                            + ",loi=" + (o.XmlPrecheckErrNum.HasValue ? o.XmlPrecheckErrNum.ToString() : "-")
                            + ",nt=" + (o.XmlPrecheckCrtNum.HasValue ? o.XmlPrecheckCrtNum.ToString() : "-")
                            + ",dai=" + (o.XmlPrecheckDesc == null ? 0 : o.XmlPrecheckDesc.Length))));
                }

                HisTreatmentXmlPrecheckSaveResultSDO response =
                    new BackendAdapter(param).Post<HisTreatmentXmlPrecheckSaveResultSDO>(
                        URI_UPDATE_RESULT, ApiConsumers.MosConsumer, request, param);

                if (LogSystem.IsDebugEnabled())
                {
                    LogSystem.Debug("MdInsight NHAN VE___" + URI_UPDATE_RESULT + "___"
                        + (response == null
                            ? "(khong tra ve du lieu)"
                            : "Success=" + response.Success
                              + ", Message=" + (response.Message ?? "")
                              + ", that bai=" + (response.FailList == null ? 0 : response.FailList.Count)));
                }

                if (response == null)
                {
                    LogSystem.Error("MdInsightApiAdapter - Giao dien ghi nhan ket qua khong tra ve du lieu. So ho so: "
                        + chunk.Count);
                    return false;
                }

                if (response.FailList != null)
                {
                    foreach (HisTreatmentXmlPrecheckFailADO fail in response.FailList)
                    {
                        if (fail == null)
                        {
                            continue;
                        }
                        failMessages[fail.TreatmentId] = fail.Message;
                    }
                }

                if (!response.Success)
                {
                    //Message la thong bao nghiep vu, khong chua du lieu benh nhan
                    LogSystem.Warn("MdInsightApiAdapter - Ghi nhan ket qua khong tron ven. Ly do: "
                        + (response.Message ?? "") + ". So ho so that bai: "
                        + (response.FailList == null ? 0 : response.FailList.Count));
                }

                return response.Success;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Doc lai ket qua da luu cua mot tap ho so. Tu chia nho khi vuot han muc moi luot goi.
        ///
        /// Luong "Lay ket qua" dung giao dien nay de biet TEN TEP da gui va THOI DIEM GUI -
        /// hai du kien de tra lai he ngoai va de tinh han giu trang thai Chua co ket qua.
        /// </summary>
        public static Dictionary<long, HisTreatmentXmlPrecheckDetailADO> GetDetails(List<long> treatmentIds)
        {
            Dictionary<long, HisTreatmentXmlPrecheckDetailADO> result
                = new Dictionary<long, HisTreatmentXmlPrecheckDetailADO>();

            try
            {
                if (treatmentIds == null || treatmentIds.Count == 0)
                {
                    return result;
                }

                List<long> distinctIds = treatmentIds.Distinct().ToList();

                for (int i = 0; i < distinctIds.Count; i += MAX_READ_PER_CALL)
                {
                    int size = Math.Min(MAX_READ_PER_CALL, distinctIds.Count - i);
                    ReadOneChunk(distinctIds.GetRange(i, size), result);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        private static void ReadOneChunk(
            List<long> ids, Dictionary<long, HisTreatmentXmlPrecheckDetailADO> target)
        {
            CommonParam param = new CommonParam();

            try
            {
                HisTreatmentGetXmlPrecheckDetailSDO request = new HisTreatmentGetXmlPrecheckDetailSDO
                {
                    TreatmentIds = ids
                };

                if (LogSystem.IsDebugEnabled())
                {
                    LogSystem.Debug("MdInsight GUI DI___" + URI_GET_DETAIL + "___"
                        + ids.Count + " ho so___" + String.Join(", ", ids));
                }

                List<HisTreatmentXmlPrecheckDetailADO> response =
                    new BackendAdapter(param).Post<List<HisTreatmentXmlPrecheckDetailADO>>(
                        URI_GET_DETAIL, ApiConsumers.MosConsumer, request, param);

                //Noi dung tra ve CO du lieu benh nhan - chi ghi ma dot dieu tri va trang thai
                if (LogSystem.IsDebugEnabled())
                {
                    LogSystem.Debug("MdInsight NHAN VE___" + URI_GET_DETAIL + "___"
                        + (response == null
                            ? "(khong tra ve du lieu)"
                            : response.Count + " ho so___" + String.Join(", ", response.Select(o =>
                                o.TreatmentId + ":tt=" + (o.XmlPrecheckResult.HasValue
                                    ? o.XmlPrecheckResult.ToString() : "-")))));
                }

                if (response == null)
                {
                    LogSystem.Warn("MdInsightApiAdapter - Giao dien doc ket qua khong tra ve du lieu. So ho so hoi: "
                        + ids.Count);
                    return;
                }

                foreach (HisTreatmentXmlPrecheckDetailADO item in response)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    target[item.TreatmentId] = item;
                }

                //CHI ghi so luong - noi dung tra ve co du lieu benh nhan (quy tac QT-28)
                LogSystem.Info("MdInsightApiAdapter - Da doc ket qua da luu cua " + response.Count + " ho so.");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyen ket qua cua tram sang khuon du lieu cua giao dien ghi nhan.
        ///
        /// Quy tac ghi de cua may chu: truong KHONG co gia tri thi giu nguyen gia tri cu.
        /// Vi vay chi dien thoi diem gui o luot ghi nhan da gui, va chi dien cac so dem
        /// khi that su co ket qua - PTTK muc B.3.2.1 buoc 5 va 6.
        /// </summary>
        private static HisTreatmentXmlPrecheckResultADO ToResultADO(MdInsightResultADO source)
        {
            return new HisTreatmentXmlPrecheckResultADO
            {
                TreatmentId = source.TreatmentId,
                XmlPrecheckFileName = source.SentFileName,
                XmlPrecheckSendTime = source.SendTime,
                XmlPrecheckTime = source.CheckTime,
                XmlPrecheckResult = (short)source.Status,
                XmlPrecheckErrNum = source.ErrorNum,
                XmlPrecheckCrtNum = source.CriticalNum,
                XmlPrecheckDesc = source.Status == EnumXmlPrecheckStatus.Pending
                                  || source.Status == EnumXmlPrecheckStatus.CheckFailed
                    ? MdInsightDescCodec.EncodeReason(source.Reason)
                    : EncodeErrors(source)
            };
        }

        private static string EncodeErrors(MdInsightResultADO source)
        {
            int truncated;
            string encoded = MdInsightDescCodec.Encode(source.Errors, out truncated);

            source.IsTruncated = truncated > 0;
            source.TruncatedCount = truncated;

            return encoded;
        }
    }
}
