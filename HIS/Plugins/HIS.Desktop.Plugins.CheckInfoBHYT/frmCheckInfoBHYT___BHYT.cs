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
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using MOS.EFMODEL.DataModels;
using System;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using System.Collections.Generic;
using HIS.Desktop.LocalStorage.BackendData;
using System.Linq;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Logging;


namespace HIS.Desktop.Plugins.CheckInfoBHYT
{
    public partial class frmCheckInfoBHYT : HIS.Desktop.Utility.FormBase
    {
        private async Task CheckTTFull(V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, string nameCb, string cccdCb, string api, string newapi)
        {
            rsDataBHYT = new ResultDataADO();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên cán bộ:{0}", nameCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("CCCD cán bộ:{0}", cccdCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên api:{0}", api));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên api 2:{0}", newapi));


                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                apiInsuranceExpertise.ApiEgw = api;
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = _patientTypeAlter.HEIN_CARD_NUMBER;
                checkHistoryLDO.ngaySinh = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_HisPatient.DOB);
                checkHistoryLDO.hoTen = _HisPatient.VIR_PATIENT_NAME;
                checkHistoryLDO.cccdCb = cccdCb;
                checkHistoryLDO.hoTenCb = nameCb;
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    rsDataBHYT.ResultHistoryLDO = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsDataBHYT.ResultHistoryLDO), rsDataBHYT.ResultHistoryLDO));
                    if (HisConfigCFG.WarningInvalidCheckHistoryHeinCard
                        && rsDataBHYT.ResultHistoryLDO.dsLichSuKT2018 != null
                        && rsDataBHYT.ResultHistoryLDO.dsLichSuKT2018.Count > 0
                        && rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018 != null
                        && rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018.Count > 0)
                    {
                        long maxNgayRa = 0;

                        foreach (var p in rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018)
                        {
                            long ngayRa;
                            if (long.TryParse(p.ngayRa, out ngayRa) && ngayRa > maxNgayRa)
                                maxNgayRa = ngayRa;
                        }

                        var otherChecks = new List<dynamic>();
                        foreach (var o in rsDataBHYT.ResultHistoryLDO.dsLichSuKT2018)
                        {
                            long thoiGianKT;
                            if (!long.TryParse(o.thoiGianKT, out thoiGianKT))
                                continue;
                            string userKT = o.userKT ?? "";
                            string currentMediOrgCode = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;

                            if (thoiGianKT > maxNgayRa && !string.IsNullOrEmpty(userKT) && userKT.IndexOf(currentMediOrgCode) < 0 && (o.maLoi == "000" || o.maLoi == "001" || o.maLoi == "002" || o.maLoi == "003"))
                            {
                                otherChecks.Add(o);
                            }
                        }

                        var grouped = new Dictionary<string, dsLichSuKT2018>();
                        foreach (var check in otherChecks)
                        {
                            string userKT = check.userKT ?? "";
                            long thoiGianKT;
                            if (!long.TryParse(check.thoiGianKT, out thoiGianKT))
                                continue;

                            if (!grouped.ContainsKey(userKT) || long.Parse(grouped[userKT].thoiGianKT) < thoiGianKT)
                            {
                                grouped[userKT] = check;
                            }
                        }

                        var filteredChecks = new List<dsLichSuKT2018>(grouped.Values);
                        if (filteredChecks.Count > 0)
                        {
                            var mediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>().ToList();
                            var errorDetails = new List<string>();

                            foreach (var check in filteredChecks)
                            {
                                string userKT = check.userKT ?? "";
                                string mediOrgCode = userKT.Length >= 5 ? userKT.Substring(0, 5) : userKT;
                                var mediOrg = mediOrgs.FirstOrDefault(m => m.MEDI_ORG_CODE == mediOrgCode);

                                long thoiGianKT;
                                long.TryParse(check.thoiGianKT, out thoiGianKT);
                                string timeStrRaw = thoiGianKT.ToString();

                                if (timeStrRaw.Length == 12)
                                {
                                    DateTime dt = DateTime.ParseExact(timeStrRaw, "yyyyMMddHHmm", null);
                                    string timeStr = dt.ToString("dd/MM/yyyy HH:mm");
                                    string detail = "";

                                    if (mediOrg != null && !string.IsNullOrEmpty(mediOrg.MEDI_ORG_NAME))
                                    {
                                        detail = string.Format("{0} ({1}) [{2}]", mediOrg.MEDI_ORG_NAME, mediOrg.MEDI_ORG_CODE, timeStr);
                                    }
                                    else
                                    {
                                        detail = string.Format("Tài khoản {0} [{1}]", userKT, timeStr);
                                    }
                                    errorDetails.Add(detail);
                                }
                            }

                            // Map filteredChecks sang ExamHistoryLDO
                            var mappedList = filteredChecks.Select(item =>
                            {
                                string orgCode5 = (item.userKT != null && item.userKT.Length >= 5)
                                    ? item.userKT.Substring(0, 5)
                                    : null;

                                return new ExamHistoryLDO
                                {
                                    maCSKCB = orgCode5, // chỉ lấy tên
                                    ngayVao = item.thoiGianKT,
                                    ngayRa = null,
                                    tinhTrang = "5"
                                };
                            }).OrderByDescending(x => x.ngayVao).ToList();

                            // Gộp vào dsLichSuKCB2018: kiểm tra thẻ trước, sau đó dữ liệu cũ
                            var oldList = rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018 ?? new List<ExamHistoryLDO>();
                            var newList = mappedList.Concat(oldList).ToList();

                            rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018 = newList;

                            rsDataBHYT.ResultHistoryLDO.message = "Thẻ BHYT có thông tin kiểm tra thẻ tại " + string.Join("; ", errorDetails.ToArray());
                            rsDataBHYT.ResultHistoryLDO.maKetQua = "8888";
                            return;
                        }
                    }
                    if (rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018 == null)
                    {
                        LogSystem.Debug("Khong co lich su KCB 2018, kiem tra voi api moi");
                        ResultHistoryLDO rsIns2 = new ResultHistoryLDO();
                        if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                            || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                            || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                        {
                            apiInsuranceExpertise.ApiEgw = newapi;
                            rsIns2 = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsIns2), rsIns2));
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                            rsIns2 = null;
                        }
                        if (rsIns2 != null && rsIns2.dsLichSuKCB2025 != null && rsIns2.dsLichSuKCB2025.Count > 0 && rsIns2.success == true && String.IsNullOrEmpty(rsIns2.message))
                        {
                            rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018 = new List<ExamHistoryLDO>();
                            foreach (var item in rsIns2.dsLichSuKCB2025)
                            {
                                rsDataBHYT.ResultHistoryLDO.dsLichSuKCB2018.Add(item);
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsDataBHYT.ResultHistoryLDO), rsDataBHYT.ResultHistoryLDO));
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }
            }
            catch (Exception ex)
            {
                rsDataBHYT = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public class GenderConvert
        {
            public static string TextToNumber(string ge)
            {
                return (ge == "Nữ") ? "2" : "1";
            }

            public static string HisToHein(string ge)
            {
                return (ge == "1") ? "2" : "1";
            }

            public static long HeinToHisNumber(string ge)
            {
                return (ge == "1" ? IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE : IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);
            }
        }
    }
}
