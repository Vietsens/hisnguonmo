using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
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
        public static bool SendData(HIS_TREATMENT data, ref string message)
        {
            bool result = true;
            try
            {
                if (data == null) return result;

                Configs.LoadConfig();
                if (Configs.IS_CONNECT)
                {
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
                        return result;
                    }

                    //không có bệnh tương ứng thì bỏ qua
                    if (!Utilities.IsBADTD(totalIcds) && !Utilities.IsBATHA(totalIcds))
                    {
                        return result;
                    }

                    HIS_DHST dhst = null;
                    CommonParam param = new CommonParam();
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = data.ID;
                    var HIS_DHSTs = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, param);
                    if (HIS_DHSTs != null && HIS_DHSTs.Count > 0)
                    {
                        dhst = HIS_DHSTs.Where(o => o.BLOOD_PRESSURE_MAX.HasValue && o.BLOOD_PRESSURE_MIN.HasValue).OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault();
                    }

                    List<HIS_SERE_SERV_TEIN> ssTein = null;
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = data.ID;
                    ssFilter.SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN };
                    List<V_HIS_SERE_SERV> V_HIS_SERE_SERVs = new Inventec.Common.Adapter.BackendAdapter(param)
                          .Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
                    if (V_HIS_SERE_SERVs != null && V_HIS_SERE_SERVs.Count > 0)
                    {
                        List<V_HIS_SERE_SERV> list = V_HIS_SERE_SERVs.Where(o => Configs.SERVICE_CODE_DIABETES_MELLITUS.Contains(o.TDL_HEIN_SERVICE_BHYT_CODE)).ToList();
                        if (list != null && list.Count > 0)
                        {
                            HisSereServTeinFilter sstFilter = new HisSereServTeinFilter();
                            sstFilter.SERE_SERV_IDs = list.Select(s => s.ID).ToList();
                            sstFilter.HAS_VALUE = true;
                            ssTein = new Inventec.Common.Adapter.BackendAdapter(param)
                                  .Get<List<HIS_SERE_SERV_TEIN>>("api/HisSereServTein/Get", ApiConsumer.ApiConsumers.MosConsumer, sstFilter, param);
                        }
                    }

                    //cao huyết áp: I10-I15, khi lưu bắt buộc phải có thông tin huyết áp
                    if (Utilities.IsBATHA(totalIcds) && dhst == null)
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

                    //Tạo thread riêng để gửi tránh ảnh hưởng luồng chính 
                    InitThreadSync(data, dhst, V_HIS_SERE_SERVs, ssTein);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                message += ex.Message;
                result = false;
            }
            return result;
        }

        private static void InitThreadSync(HIS_TREATMENT data, HIS_DHST dhst, List<V_HIS_SERE_SERV> V_HIS_SERE_SERVs, List<HIS_SERE_SERV_TEIN> ssTein)
        {
            try
            {
                Model.ProcessData datas = new Model.ProcessData { Treatment = data, Dhst = dhst, V_HIS_SERE_SERVs = V_HIS_SERE_SERVs, HIS_SERE_SERV_TEINs = ssTein };

                Thread thread = new Thread(new ParameterizedThreadStart(SendDataWithoutCheck));
                thread.Start(datas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void SendDataWithoutCheck(object obj)
        {
            try
            {
                Model.ProcessData datas = obj as Model.ProcessData;
                HIS_TREATMENT data = datas.Treatment;
                HIS_DHST dhst = datas.Dhst;
                List<V_HIS_SERE_SERV> V_HIS_SERE_SERVs = datas.V_HIS_SERE_SERVs;
                List<HIS_SERE_SERV_TEIN> ssTein = datas.HIS_SERE_SERV_TEINs;

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

                string thuoc = "";
                CommonParam medicineParam = new CommonParam();
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.TDL_TREATMENT_ID = data.ID;
                List<V_HIS_EXP_MEST_MEDICINE> medicine = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, param);
                if (medicine != null && medicine.Count > 0)
                {
                    // string val = ((TEN_THUOC + " " + HAM_LUONG).Trim() + " " + SO_LUONG.ToString() + " (" + DVT + ")").Trim();                if (SO_NGAY > 0) { val += " - " + SO_NGAY + " ngày"; }
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in medicine)
                    {
                        if (item.AMOUNT - (item.TH_AMOUNT ?? 0) > 0)
                        {
                            sb.AppendFormat("{0} {1} {2} ({3})", item.MEDICINE_TYPE_NAME, item.CONCENTRA, item.AMOUNT - (item.TH_AMOUNT ?? 0), item.SERVICE_UNIT_NAME);

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
                        ncdData.DU_LIEU.THA.CAN_NANG = dhst.WEIGHT + "";
                        ncdData.DU_LIEU.THA.CHIEU_CAO = dhst.HEIGHT + "";
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
                        ncdData.DU_LIEU.DTD.CAN_NANG = dhst.WEIGHT + "";
                        ncdData.DU_LIEU.DTD.CHIEU_CAO = dhst.HEIGHT + "";
                    }
                    if (ssTein != null && ssTein.Count > 0)
                    {
                        ncdData.DU_LIEU.DTD.DUONG_HUYET = ssTein.FirstOrDefault().VALUE;
                    }
                }

                Model.SendData sendData = new Model.SendData()
                {
                    credentials = new Model.credentials() { token = Utilities.NCDToken.response.key },
                    ncdData = new List<Model.NcdData>() { ncdData }
                };

                Model.OImport rsData = ApiConsumers.CreateRequest<Model.OImport>("POST", Configs.API_NCD, "/api/v1/import", sendData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
