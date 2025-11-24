using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd
{
    public class ConnectWhoCndProcessor
    {
        HIS_TREATMENT data;
        HIS_DHST dhst;
        List<V_HIS_EXP_MEST_MEDICINE> medicine;
        List<HIS_SERE_SERV> V_HIS_SERE_SERVs;
        List<HIS_SERE_SERV_TEIN> ssTein;
        bool HasData = false;

        public ConnectWhoCndProcessor(HIS_TREATMENT _data, HIS_DHST _dhst, List<V_HIS_EXP_MEST_MEDICINE> _medicine)
        {
            this.data = _data;
            this.dhst = _dhst;
            this.medicine = _medicine;
        }

        public bool CheckData(ref string message)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ConnectWhoCndProcessor SendData");
                //không có só thẻ BHYT, CCCD thì không đẩy
                if (data == null || String.IsNullOrWhiteSpace(data.TDL_PATIENT_CCCD_NUMBER) || String.IsNullOrWhiteSpace(data.TDL_HEIN_CARD_NUMBER))
                    return result;

                Configs.LoadConfig();
                List<string> totalIcds = new List<string>();
                if (!String.IsNullOrWhiteSpace(data.ICD_CODE))
                {
                    totalIcds.Add(data.ICD_CODE);
                }
                if (!String.IsNullOrWhiteSpace(data.ICD_SUB_CODE))
                {
                    totalIcds.AddRange(data.ICD_SUB_CODE.Split(';'));
                }

                //không có chẩn đoán thì bỏ qua
                if (totalIcds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(string.Format("totalIcds: {0}", totalIcds.Count));
                    return result;
                }

                //không có bệnh tương ứng thì bỏ qua
                if (!Utilities.IsBADTD(totalIcds) && !Utilities.IsBATHA(totalIcds))
                {
                    Inventec.Common.Logging.LogSystem.Info("NOT IN ICD");
                    return result;
                }

                var t1 = Task.Run(() => GetDhst(data, dhst));
                var t2 = Task.Run(() => GetSereServ(data));
                var t3 = Task.Run(() => GetMedicine(data, medicine));
                var t4 = Task.Run(() => GetSereServTein(data));

                Task.WaitAll(t1, t2, t3);

                dhst = t1.Result;
                V_HIS_SERE_SERVs = t2.Result;
                medicine = t3.Result;
                ssTein = t4.Result;

                if (V_HIS_SERE_SERVs != null && V_HIS_SERE_SERVs.Count > 0 && ssTein != null && ssTein.Count > 0)
                {
                    List<HIS_SERE_SERV> list = V_HIS_SERE_SERVs.Where(o => Configs.SERVICE_CODE_DIABETES_MELLITUS.Contains(o.TDL_HEIN_SERVICE_BHYT_CODE)).ToList();
                    if (list != null && list.Count > 0)
                    {
                        //lọc lại kết quả theo dịch vụ
                        ssTein = ssTein.Where(o => list.Select(s => s.ID).Contains(o.SERE_SERV_ID)).ToList();
                    }
                    else
                    {
                        //xóa đi vì không có dịch vụ
                        ssTein = null;
                    }
                }

                if (medicine == null || medicine.Count <= 0)
                {
                    message = "Bệnh nhân chưa có thông tin đơn thuốc";
                    return false;
                }

                //cao huyết áp: I10-I15, khi lưu bắt buộc phải có thông tin huyết áp
                if (Utilities.IsBATHA(totalIcds) && (dhst == null || !dhst.BLOOD_PRESSURE_MAX.HasValue || !dhst.BLOOD_PRESSURE_MIN.HasValue))
                {
                    message = "Bệnh nhân thiếu thông tin huyết áp";
                    return false;
                }

                //đái tháo đường: E10-E14, khi lưu phải có kết quả của đường huyết
                if (Utilities.IsBADTD(totalIcds) && (ssTein == null || ssTein.Count == 0))
                {
                    message = "Bệnh nhân đái tháo đường thiếu kết quả xét nghiệm đường huyết";
                    return false;
                }
                HasData = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                message += ex.Message;
                result = false;
            }
            return result;
        }

        public void SendData()
        {
            try
            {
                if (HasData)
                {
                    Task.Run(() => SendDataWithoutCheck());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SendDataWithoutCheck()
        {
            try
            {
                if (Configs.CheckConnect())
                {
                    Inventec.Common.Logging.LogSystem.Info("SendDataWithoutCheck");

                    List<string> totalIcds = new List<string>();
                    if (!String.IsNullOrWhiteSpace(data.ICD_CODE))
                    {
                        totalIcds.Add(data.ICD_CODE);
                    }
                    if (!String.IsNullOrWhiteSpace(data.ICD_SUB_CODE))
                    {
                        totalIcds.AddRange(data.ICD_SUB_CODE.Split(';'));
                    }

                    Model.NcdData ncdData = new Model.NcdData();

                    CommonParam param = new CommonParam();
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.TREATMENT_ID = data.ID;
                    reqFilter.SERVICE_REQ_STT_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL };
                    reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    List<HIS_SERVICE_REQ> examServiceReq = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, reqFilter, param);

                    List<HIS_SERE_SERV_TEIN> ssTeinHBA1C = null;
                    List<HIS_SERE_SERV> list = V_HIS_SERE_SERVs.Where(o => Configs.SERVICE_CODE_DVHBA1C.Contains(o.TDL_HEIN_SERVICE_BHYT_CODE)).ToList();
                    if (list != null && list.Count > 0)
                    {
                        HisSereServTeinFilter sstFilter = new HisSereServTeinFilter();
                        sstFilter.SERE_SERV_IDs = list.Select(s => s.ID).ToList();
                        sstFilter.HAS_VALUE = true;
                        ssTeinHBA1C = new Inventec.Common.Adapter.BackendAdapter(param)
                              .Get<List<HIS_SERE_SERV_TEIN>>("api/HisSereServTein/Get", ApiConsumer.ApiConsumers.MosConsumer, sstFilter, param);
                    }

                    string thuoc = "";
                    CommonParam medicineParam = new CommonParam();
                    if (medicine != null && medicine.Count > 0)
                    {
                        // string val = ((TEN_THUOC + " " + HAM_LUONG).Trim() + " " + SO_LUONG.ToString() + " (" + DVT + ")").Trim();                if (SO_NGAY > 0) { val += " - " + SO_NGAY + " ngày"; }
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in medicine)
                        {
                            if (item.AMOUNT - (item.TH_AMOUNT ?? 0) > 0)
                            {
                                sb.AppendFormat("{0} {1} {2} ({3})", item.MEDICINE_TYPE_NAME, item.CONCENTRA, item.AMOUNT - (item.TH_AMOUNT ?? 0), item.SERVICE_UNIT_NAME);
                                try
                                {
                                    long NUMBER_USE_DAY = 0;
                                    DateTime? dtIntructionTime = null;
                                    if (item.TDL_INTRUCTION_TIME > 0)
                                        dtIntructionTime = System.DateTime.ParseExact(item.TDL_INTRUCTION_TIME.ToString(), "yyyyMMddHHmmss",
                                             System.Globalization.CultureInfo.InvariantCulture);
                                    if (item.USE_TIME_TO > 0 && dtIntructionTime != null)
                                    {
                                        DateTime dtUserTimeTo = System.DateTime.ParseExact(item.USE_TIME_TO.ToString(), "yyyyMMddHHmmss",
                                                               System.Globalization.CultureInfo.InvariantCulture);
                                        TimeSpan ts = new TimeSpan();
                                        ts = (TimeSpan)(dtUserTimeTo - dtIntructionTime);
                                        if (ts != null && ts.Days >= 0)
                                        {
                                            NUMBER_USE_DAY = ts.Days + 1;
                                        }
                                    }
                                    if (NUMBER_USE_DAY > 0)
                                    {
                                        sb.AppendFormat(" - {0} ngày", NUMBER_USE_DAY);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }

                                sb.Append(";");
                            }
                        }
                        thuoc = sb.ToString();
                    }

                    ncdData.DU_LIEU = new Model.DULIEU();
                    if (Utilities.IsBATHA(totalIcds))
                    {
                        ncdData.DU_LIEU.THA = new Model.THA(data);
                        long startTimeMin = examServiceReq.Min(m => m.START_TIME ?? 99999999999999);
                        ncdData.DU_LIEU.THA.NGAY_KHAM = DateTime.ParseExact(startTimeMin + "", "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        ncdData.DU_LIEU.THA.THUOC = thuoc;
                        if (dhst != null)
                        {
                            ncdData.DU_LIEU.THA.HA_TAM_THU = dhst.BLOOD_PRESSURE_MAX + "";
                            ncdData.DU_LIEU.THA.HA_TAM_TRUONG = dhst.BLOOD_PRESSURE_MIN + "";
                            ncdData.DU_LIEU.THA.CAN_NANG = dhst.WEIGHT;
                            ncdData.DU_LIEU.THA.CHIEU_CAO = dhst.HEIGHT;
                        }
                    }

                    if (Utilities.IsBADTD(totalIcds))
                    {
                        ncdData.DU_LIEU.DTD = new Model.DTD(data);
                        long startTimeMin = examServiceReq.Min(m => m.START_TIME ?? 99999999999999);
                        ncdData.DU_LIEU.DTD.NGAY_KHAM = DateTime.ParseExact(startTimeMin + "", "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        ncdData.DU_LIEU.DTD.THUOC = thuoc;
                        if (dhst != null)
                        {
                            ncdData.DU_LIEU.DTD.HA_TAM_THU = dhst.BLOOD_PRESSURE_MAX + "";
                            ncdData.DU_LIEU.DTD.HA_TAM_TRUONG = dhst.BLOOD_PRESSURE_MIN + "";
                            ncdData.DU_LIEU.DTD.CAN_NANG = dhst.WEIGHT;
                            ncdData.DU_LIEU.DTD.CHIEU_CAO = dhst.HEIGHT;
                        }
                        if (ssTein != null && ssTein.Count > 0)
                        {
                            ncdData.DU_LIEU.DTD.DUONG_HUYET = ssTein.FirstOrDefault().VALUE;
                        }
                        if (ssTeinHBA1C != null && ssTeinHBA1C.Count > 0)
                        {
                            ncdData.DU_LIEU.DTD.HBA1C = ssTeinHBA1C.FirstOrDefault().VALUE;
                        }
                    }

                    Model.SendData sendData = new Model.SendData()
                    {
                        credentials = new Model.credentials() { token = Utilities.NCDToken.response.key, program = Configs.PROGRAM },

                    };
                    if (Configs.PROGRAM.ToLower().Contains("dev"))
                    {
                        sendData.ncdDevData = new List<Model.NcdData>() { ncdData };
                    }
                    else
                    {
                        sendData.ncdData = new List<Model.NcdData>() { ncdData };
                    }

                    Model.OImport rsData = ApiConsumers.CreateRequest<Model.OImport>("POST", Configs.API_NCD, "/api/v1/import", sendData);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Đăng nhập thát bại!");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region GetData
        private static List<V_HIS_EXP_MEST_MEDICINE> GetMedicine(HIS_TREATMENT data, List<V_HIS_EXP_MEST_MEDICINE> medicine)
        {
            CommonParam param = new CommonParam();
            //không truyền vào thì lấy lại
            //kê đơn sẽ truyền đơn đang xử lý vào
            if (medicine == null)
            {
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.TDL_TREATMENT_ID = data.ID;
                medicine = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, param);
            }

            return medicine;
        }

        private static HIS_DHST GetDhst(HIS_TREATMENT data, HIS_DHST dhst)
        {
            CommonParam param = new CommonParam();
            if (dhst == null || !dhst.BLOOD_PRESSURE_MAX.HasValue || !dhst.BLOOD_PRESSURE_MIN.HasValue)
            {
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = data.ID;
                var HIS_DHSTs = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, param);
                if (HIS_DHSTs != null && HIS_DHSTs.Count > 0)
                {
                    dhst = HIS_DHSTs.Where(o => o.BLOOD_PRESSURE_MAX.HasValue && o.BLOOD_PRESSURE_MIN.HasValue).OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault();
                    if (!dhst.WEIGHT.HasValue)
                    {
                        dhst.WEIGHT = HIS_DHSTs.Where(o => o.WEIGHT.HasValue).OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault().WEIGHT;
                    }
                    if (!dhst.HEIGHT.HasValue)
                    {
                        dhst.HEIGHT = HIS_DHSTs.Where(o => o.HEIGHT.HasValue).OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault().HEIGHT;
                    }
                }
            }
            return dhst;
        }

        private static List<HIS_SERE_SERV> GetSereServ(HIS_TREATMENT data)
        {
            CommonParam param = new CommonParam();
            HisSereServViewFilter ssFilter = new HisSereServViewFilter();
            ssFilter.TREATMENT_ID = data.ID;
            ssFilter.SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN };
            List<HIS_SERE_SERV> V_HIS_SERE_SERVs = new Inventec.Common.Adapter.BackendAdapter(param)
                  .Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
            return V_HIS_SERE_SERVs;
        }

        private static List<HIS_SERE_SERV_TEIN> GetSereServTein(HIS_TREATMENT data)
        {
            CommonParam param = new CommonParam();
            HisSereServTeinFilter sstFilter = new HisSereServTeinFilter();
            sstFilter.TDL_TREATMENT_ID = data.ID;
            sstFilter.HAS_VALUE = true;
            List<HIS_SERE_SERV_TEIN> ssTein = new Inventec.Common.Adapter.BackendAdapter(param)
                  .Get<List<HIS_SERE_SERV_TEIN>>("api/HisSereServTein/Get", ApiConsumer.ApiConsumers.MosConsumer, sstFilter, param);
            return ssTein;
        }
        #endregion
    }
}
