/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Bhyt.Hssk;
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.ADO;
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Api;
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Config;
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe
{
    /// <summary>
    /// Gửi Giấy chứng sinh mẫu 01 / Giấy báo tử lên Kho dữ liệu ngành Y tế (SYT).
    /// Dữ liệu lấy từ hàm dựng sẵn cho cổng BHXH (HIS.Bhyt.Hssk), gom theo cơ sở KBCB của chi nhánh,
    /// gửi theo lô (đặc tả khuyến nghị 10-20 bản ghi/request). Không lưu trạng thái vào DB:
    /// kho chặn trùng MA_GCS/MA_GBT nên gửi lại an toàn, báo "Đã có trên kho".
    /// </summary>
    public class KdlProcessor
    {
        private const string PATH_BIRTH_01 = "/api/v1/birth-certificates-01";
        private const string PATH_DEATH = "/api/v1/death-certificates";
        private const int BATCH_SIZE = 20;

        private class PendingRecord
        {
            internal KdlResultADO Result;
            internal object Data;
        }

        public static List<KdlResultADO> PushBirth01(List<KdlBirthInputADO> inputs)
        {
            List<KdlResultADO> results = new List<KdlResultADO>();
            try
            {
                Dictionary<string, List<PendingRecord>> pendingByCskcb = new Dictionary<string, List<PendingRecord>>();
                SyncDataProcess hssk = new SyncDataProcess(null, null, null);
                foreach (KdlBirthInputADO input in inputs)
                {
                    KdlResultADO result = new KdlResultADO();
                    result.TreatmentCode = input.Baby.TREATMENT_CODE;
                    result.PatientName = input.Baby.TDL_PATIENT_NAME;
                    results.Add(result);

                    if (input.Baby.BIRTH_CERT_NUM == null || input.Baby.ISSUED_DATE == null)
                    {
                        SetResult(result, KdlResultADO.STATUS__SKIPPED, "Chưa cấp giấy chứng sinh");
                        continue;
                    }
                    if (input.Treatment == null)
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, "Không lấy được hồ sơ điều trị của mẹ");
                        continue;
                    }
                    string error = CheckBranch(input.Branch);
                    if (error != null)
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, error);
                        continue;
                    }

                    HSDLGCS gcs = hssk.GenXmlGCSData(input.Branch, input.Baby, input.Treatment);
                    if (gcs == null || gcs.GIAYCHUNGSINH == null)
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, "Không dựng được dữ liệu giấy chứng sinh");
                        continue;
                    }
                    KdlGcs01Data data = KdlMapper.ToGcs01(gcs.GIAYCHUNGSINH, input.Baby);
                    result.CertCode = data.MA_GCS;
                    AddPending(pendingByCskcb, input.Branch.HEIN_MEDI_ORG_CODE, result, data);
                }
                SendPending(pendingByCskcb, PATH_BIRTH_01);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        public static List<KdlResultADO> PushDeath(List<KdlDeathInputADO> inputs)
        {
            List<KdlResultADO> results = new List<KdlResultADO>();
            try
            {
                Dictionary<string, List<PendingRecord>> pendingByCskcb = new Dictionary<string, List<PendingRecord>>();
                SyncDataProcess hssk = new SyncDataProcess(null, null, null);
                foreach (KdlDeathInputADO input in inputs)
                {
                    KdlResultADO result = new KdlResultADO();
                    result.TreatmentCode = input.Treatment.TREATMENT_CODE;
                    result.PatientName = input.Treatment.TDL_PATIENT_NAME;
                    results.Add(result);

                    if (input.Treatment.DEATH_CERT_NUM == null || input.Treatment.DEATH_ISSUED_DATE == null)
                    {
                        SetResult(result, KdlResultADO.STATUS__SKIPPED, "Chưa cấp giấy báo tử");
                        continue;
                    }
                    if (input.Patient == null)
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, "Không lấy được thông tin bệnh nhân");
                        continue;
                    }
                    string error = CheckBranch(input.Branch);
                    if (error != null)
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, error);
                        continue;
                    }

                    HSDLGBT gbt = hssk.GenXmlGBTData(input.Branch, input.Patient, input.Treatment, input.DeathData);
                    if (gbt == null || gbt.GIAYBAOTU == null || string.IsNullOrEmpty(gbt.GIAYBAOTU.MA_GBT))
                    {
                        SetResult(result, KdlResultADO.STATUS__FAILED, "Không dựng được dữ liệu giấy báo tử");
                        continue;
                    }
                    KdlGbtData data = KdlMapper.ToGbt(gbt.GIAYBAOTU, input.Treatment);
                    result.CertCode = data.MA_GBT;
                    AddPending(pendingByCskcb, input.Branch.HEIN_MEDI_ORG_CODE, result, data);
                }
                SendPending(pendingByCskcb, PATH_DEATH);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        private static string CheckBranch(MOS.EFMODEL.DataModels.HIS_BRANCH branch)
        {
            if (branch == null)
                return "Không xác định được chi nhánh của hồ sơ";
            if (KdlConfig.GetByMaCskcb(branch.HEIN_MEDI_ORG_CODE) == null)
                return string.Format("Chưa khai báo kết nối Kho dữ liệu SYT cho cơ sở {0} (khóa {1})", branch.HEIN_MEDI_ORG_CODE, KdlConfig.CONNECTION_INFO_KEY);
            return null;
        }

        private static void AddPending(Dictionary<string, List<PendingRecord>> pendingByCskcb, string maCskcb, KdlResultADO result, object data)
        {
            if (!pendingByCskcb.ContainsKey(maCskcb))
                pendingByCskcb[maCskcb] = new List<PendingRecord>();
            pendingByCskcb[maCskcb].Add(new PendingRecord { Result = result, Data = data });
        }

        private static void SendPending(Dictionary<string, List<PendingRecord>> pendingByCskcb, string path)
        {
            foreach (var group in pendingByCskcb)
            {
                KdlApiClient client = new KdlApiClient(KdlConfig.GetByMaCskcb(group.Key));
                for (int start = 0; start < group.Value.Count; start += BATCH_SIZE)
                {
                    List<PendingRecord> batch = group.Value.Skip(start).Take(BATCH_SIZE).ToList();
                    List<KdlRecordResult> sent = client.Push(path, batch.Select(o => o.Data).ToList());
                    for (int i = 0; i < batch.Count; i++)
                    {
                        KdlRecordResult r = i < sent.Count ? sent[i] : null;
                        if (r == null)
                            SetResult(batch[i].Result, KdlResultADO.STATUS__FAILED, "Không nhận được kết quả từ Kho dữ liệu");
                        else if (r.Existed)
                            SetResult(batch[i].Result, KdlResultADO.STATUS__EXISTED, r.Message);
                        else if (r.Success)
                            SetResult(batch[i].Result, KdlResultADO.STATUS__SUCCESS, null);
                        else
                            SetResult(batch[i].Result, KdlResultADO.STATUS__FAILED, r.Message);
                    }
                }
            }
        }

        private static void SetResult(KdlResultADO result, int status, string message)
        {
            result.Status = status;
            result.Message = message;
        }
    }
}
