using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000504.PDO;
using MPS.ProcessorBase;
using MPS.Processor.Mps000504.ADO;
using MPS.Processor.Mps000504.PDO.Config;
using Newtonsoft.Json;

namespace MPS.Processor.Mps000504
{
    public partial class Mps000504Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.TreatmentView);
                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                var allSereServs = rdo.SereServs;
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, allSereServs, rdo.SereServExts, rdo.HeinServiceTypes,
                                          rdo.Services, rdo.Departments, rdo.Rooms, rdo.medicineTypes, rdo.MedicineLines, rdo.materialTypes, rdo.PatientTypeCFG,
                                          rdo.HisConfigValue, rdo.HisServiceUnit, rdo.TreatmentView,
                                          rdo.ServiceReqs,
                                          rdo.PatientTypeAlterAlls
                                          ));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));

                sereServADOTemps = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.PRICE_BHYT > 0
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999).ToList();

                var sereServBHYTGroups = sereServADOTemps
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRIMARY_PRICE,
                        o.PRICE_BHYT,
                        o.SERVICE_PAY_RATE,
                        o.BHYT_PAY_RATE,
                        o.IS_EXPEND,
                        o.NUMBER_OF_FILM,
                        o.KEY_PATY_ALTER,
                        o.HEIN_SERVICE_TYPE_ID,
                        o.STENT_ORDER
                    }).ToList();

                ProcessOtherSource(sereServADOTemps);
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    // === ĐỐI CHIẾU CỔNG BHYT - GRAIN DÒNG GỐC ===
                    // Cổng làm tròn 2 số TỪNG DÒNG sere_serv GỐC rồi mới cộng (Mrs00826 Xml2/Xml3 không gom nhóm -> Xml1 tổng = Σ Round(dòng,2)).
                    // Đây là nơi các dòng GỐC (sereServADOTemps) lần đầu cộng dồn vào nhóm -> PHẢI làm tròn TỪNG DÒNG GỐC tại đây,
                    // nếu để cộng thô rồi mới tròn ở PatyAlterProcess (tổng-con nhóm) thì SAI GRAIN -> vẫn lệch XML1. 
                    //
                    // BN CÙNG CHI TRẢ (TongBNCCT): cổng KHÔNG làm tròn trực tiếp tiền cùng chi trả từng dòng. Xml2Processor.cs:156 tính:
                    //     TongBNCCT = Round(ThanhTien * TyLeTT/100, 2) - TongBHTT   (= tiền_trong_phạm_vi_TT ĐÃ tròn - quỹ BHYT ĐÃ tròn)
                    // tức HIỆU của 2 số ĐÃ làm tròn. Vì round(a-b) != round(a)-round(b) nên nếu bảng kê làm Σ Round(CCT_raw,2)
                    // (làm tròn độc lập trường raw) sẽ lệch vài xu/dòng cộng dồn -> lệch phần người bệnh cùng chi trả so với cổng.
                    // "tiền trong phạm vi TT" raw của 1 dòng = quỹ BHYT raw + BN cùng chi trả raw (đúng cho CẢ đúng tuyến lẫn
                    // trái tuyến, vì DB đã tính CCT_raw theo đúng mức trái tuyến) => derive khớp cổng:
                    //     cùng chi trả = Σ Round(quỹ+CCT, 2)/dòng  -  Σ Round(quỹ, 2)/dòng.
                    // Phải tính TRƯỚC khi mutate dòng đầu nhóm (sereServ là tham chiếu tới FirstOrDefault) để Sum không đọc trúng
                    // giá trị đã cộng dồn của chính dòng đầu.
                    decimal heinPlusCoPayGroup = sereServBHYTGroup.Sum(o => Math.Round((o.VIR_TOTAL_HEIN_PRICE ?? 0m) + (o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0m), 2, MidpointRounding.AwayFromZero));
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => Math.Round(o.VIR_TOTAL_HEIN_PRICE ?? 0m, 2, MidpointRounding.AwayFromZero));
                    // cùng chi trả DERIVE = Σ Round(quỹ+CCT,2) - Σ Round(quỹ,2) (giữ đẳng thức, khớp cổng round-then-subtract). Đặt SAU dòng quỹ.
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = heinPlusCoPayGroup - (sereServ.VIR_TOTAL_HEIN_PRICE ?? 0m);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => Math.Round(o.TOTAL_PRICE_BHYT, 2, MidpointRounding.AwayFromZero));
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => Math.Round(o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0m, 2, MidpointRounding.AwayFromZero));
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => Math.Round(o.TOTAL_PRICE_PATIENT_SELF, 2, MidpointRounding.AwayFromZero));
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => Math.Round(o.OTHER_SOURCE_PRICE ?? 0m, 2, MidpointRounding.AwayFromZero));
                    sereServ.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => Math.Round(o.TOTAL_PRICE_VP, 2, MidpointRounding.AwayFromZero));
                    // Cột residual: DERIVE từ các tổng ĐÃ tròn (giữ đẳng thức, giống cổng derive TongBNTT) - đặt SAU các cột trực tiếp.
                    sereServ.TOTAL_PRICE_PATIENT_NO_PAY_RATE = (sereServ.VIR_TOTAL_PRICE_NO_EXPEND ?? 0m) - (sereServ.VIR_TOTAL_HEIN_PRICE ?? 0m) - (sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0m) - (sereServ.OTHER_SOURCE_PRICE ?? 0m);
                    decimal patientLeftLine = sereServ.TOTAL_PRICE_VP - (sereServ.VIR_TOTAL_HEIN_PRICE ?? 0m) - (sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0m) - (sereServ.OTHER_SOURCE_PRICE ?? 0m);
                    sereServ.TOTAL_PATIENT_PRICE_LEFT = patientLeftLine < 0 ? 0m : patientLeftLine;
                    sereServADOs.Add(sereServ);

                    if (sereServ.STENT_ORDER.HasValue && sereServ.STENT_ORDER.Value > 1)
                    {
                        decimal quyBHTT = sereServ.VIR_TOTAL_HEIN_PRICE ?? 0;
                        decimal bnCungChiTra = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0;
                        decimal nguonKhac = sereServ.OTHER_SOURCE_PRICE ?? 0;

                        decimal bnHoacNguonKhac = bnCungChiTra > 0 ? bnCungChiTra : nguonKhac;

                        sereServ.TOTAL_PRICE_BHYT = quyBHTT + bnHoacNguonKhac;
                    }
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();

                var SereServCDHA = rdo.SereServs
                    .Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT).ToList();

                if (SereServCDHA != null && SereServCDHA.Count > 0
                    && rdo.Services != null && rdo.Services.Count > 0)
                {
                    var SereServExtCDHA = rdo.SereServExts != null && rdo.SereServExts.Count > 0
                        ? rdo.SereServExts.Where(o => SereServCDHA.Select(p => p.ID).Contains(o.SERE_SERV_ID)).ToList()
                        : null;

                    var diimServices = rdo.Services
                        .Where(o => SereServCDHA.Select(p => p.SERVICE_ID).Contains(o.ID)).ToList();

                    var GroupServiceCDHA = diimServices != null
                        ? diimServices.GroupBy(p => p.DIIM_TYPE_ID).ToList()
                        : null;

                    this.CDHACountList = new List<CDHACount>();
                    if (GroupServiceCDHA != null && GroupServiceCDHA.Count() > 0)
                    {
                        foreach (var group in GroupServiceCDHA)
                        {
                            CDHACount cDHACount = new CDHACount();

                            var diim = rdo.DiimTypesList.FirstOrDefault(o => o.ID == group.FirstOrDefault().DIIM_TYPE_ID) ?? new HIS_DIIM_TYPE();
                            cDHACount.DIIM_TYPE_CODE = diim != null ? diim.DIIM_TYPE_CODE : "";
                            cDHACount.DIIM_TYPE_NAME = diim != null ? diim.DIIM_TYPE_NAME : "";

                            var serviceDiims = diimServices.Where(o => o.DIIM_TYPE_ID == diim.ID).ToList();

                            var sereServ = serviceDiims != null && serviceDiims.Count() > 0 && SereServCDHA != null
                                ? SereServCDHA.Where(o => serviceDiims.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList()
                                : null;

                            if (sereServ != null && sereServ.Count > 0)
                            {
                                var sereSerExt = SereServExtCDHA != null && SereServExtCDHA.Count > 0
                                    ? SereServExtCDHA.Where(o => sereServ.Select(p => p.ID).Contains(o.SERE_SERV_ID)).ToList()
                                    : null;
                                cDHACount.COUNT_DIIM = sereSerExt != null ? sereSerExt.Sum(o => o.NUMBER_OF_FILM) : null;
                            }
                            this.CDHACountList.Add(cDHACount);
                        }
                    }
                }

                this.PatyAlterProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void GroupDisplayProcess()
        {
            try
            {
                this.HeinServiceTypeProcess();

                sereServADOs.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

                this.MedicineLineProcesss();

                this.HeinServiceTypeBedProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void HeinServiceTypeProcess()
        {
            try
            {
                HeinServiceTypeBeds = new List<HeinServiceTypeADO>();

                heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER }).ToList();

                List<long> parentIdVTs = this.sereServADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE ?? 0);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    heinServiceType.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);

                    heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PRICE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            HeinServiceTypeADO goi = heinServiceTypeADOs.FirstOrDefault(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID);
                            if (goi != null)
                            {
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE += heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE += heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE += heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE += heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF += heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT += heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PRICE_VP += heinServiceType.TOTAL_PRICE_VP;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                                goi.ID = HeinServiceTypeExt.GOI_VT_Y_TE__ID;
                                goi.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                goi.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE = heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE = heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT = heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PRICE_VP = heinServiceType.TOTAL_PRICE_VP;
                                heinServiceTypeADOs.Add(goi);
                            }

                            var sereServNoStent = sereServBHYTGroup.Where(o => !o.STENT_ORDER.HasValue).ToList();
                            var stent = sereServBHYTGroup.Where(o => o.STENT_ORDER.HasValue).OrderBy(o => o.STENT_ORDER).FirstOrDefault();
                            if (stent != null)
                            {
                                sereServNoStent.Add(stent);
                            }
                            heinServiceType.TOTAL_PRICE = sereServNoStent.Sum(s => s.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                            heinServiceType.TOTAL_HEIN_PRICE = sereServNoStent.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0);
                            heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                            heinServiceType.TOTAL_PATIENT_PRICE_SELF = sereServNoStent.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent.TDL_HEIN_SERVICE_BHYT_NAME);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            heinServiceType.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = indexGoiVatTuYTe;
                            indexGoiVatTuYTe++;
                        }
                        else
                        {
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue
                        && (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L))
                    {
                        var lstGiuong = heinServiceTypeADOs.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID).ToList();
                        if (lstGiuong != null && lstGiuong.Count > 0)
                            continue;
                        else
                        {
                            heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                            heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }

                    heinServiceTypeADOs.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void HeinServiceTypeBedProcess()
        {
            try
            {
                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID }).ToList();

                long numOrderVTYT = 1;
                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    heinServiceType.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                    if (heinServiceType.PARENT_ID.HasValue && (heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID || heinServiceType.PARENT_ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID))
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        heinServiceType.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        heinServiceType.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        if (g.First().HEIN_SERVICE_TYPE_CHILD_NUM_ORDER.HasValue)
                        {
                            heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_CHILD_NUM_ORDER;
                        }
                        else
                        {
                            heinServiceType.NUM_ORDER = numOrderVTYT;
                            numOrderVTYT += 1;
                        }
                    }

                    HeinServiceTypeBeds.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void MedicineLineProcesss()
        {
            try
            {
                medicineLineADOs = new List<MedicineLineADO>();
                var sereServGroups = sereServADOs
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER }).ToList();
                foreach (var sereServs in sereServGroups)
                {
                    SereServADO sereServADO = sereServs.FirstOrDefault();
                    MedicineLineADO medicineLineADO = new MedicineLineADO();
                    medicineLineADO.ID = sereServADO.MEDICINE_LINE_ID;
                    medicineLineADO.HEIN_SERVICE_TYPE_ID = sereServADO.HEIN_SERVICE_TYPE_ID;
                    medicineLineADO.KEY_PATY_ALTER = sereServADO.KEY_PATY_ALTER;
                    medicineLineADO.MEDICINE_LINE_CODE = sereServADO.MEDICINE_LINE_CODE;
                    medicineLineADO.MEDICINE_LINE_NAME = sereServADO.MEDICINE_LINE_NAME;
                    if (sereServADO.MEDICINE_LINE_ID <= 0 && sereServADO.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        medicineLineADO.MEDICINE_LINE_CODE = "Chưa xác định";
                        medicineLineADO.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = sereServs.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
                        if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                        {
                            medicineLineADO.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                        }
                    }

                    medicineLineADOs.Add(medicineLineADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void PatyAlterProcess()
        {
            try
            {
                this.patyAlterBHYTADOs = new List<PatyAlterBhytADO>();
                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var ssGroup = sereServADOs.GroupBy(o => o.KEY_PATY_ALTER);
                    foreach (var g in ssGroup)
                    {
                        PatyAlterBhytADO ado = new PatyAlterBhytADO();
                        ado = DataRawProcess.PatyAlterBHYTRawToADO(g.First().PatientTypeAlter, rdo.PatientTypeAlterAlls, rdo.TreatmentView, rdo.Branch, rdo.TreatmentTypes, rdo.CurrentPatyAlter, g.ToList());
                        ado.KEY = g.First().KEY_PATY_ALTER;
                        // === ĐỐI CHIẾU CỔNG BHYT ===
                        // Cổng làm tròn 2 số TỪNG DÒNG (AwayFromZero) rồi mới cộng (Mrs00826/Xml2Processor.cs:139-200:
                        // ThanhTien/TongBHTT/TongBNCCT/TongNguonKhac đều Round(.,2); residual TongBNTT được DERIVE từ các số
                        // đã tròn, KHÔNG tự làm tròn). Bảng kê phải cùng grain: cột trực tiếp = Sum(Round(x,2)); cột residual
                        // = derive từ các tổng đã tròn để giữ đẳng thức. Nullable dùng (?? 0m) trước Round (tránh NRE).
                        // Nếu cộng thô raw sẽ lệch cổng (vd 5003841,832 -> .83 thay vì .84). 

                        // --- Cột trực tiếp: làm tròn 2 số/dòng rồi cộng ---
                        ado.TOTAL_PRICE = g.Sum(o => Math.Round(o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0m, 2, MidpointRounding.AwayFromZero));       // tổng chi trong phạm vi giá BHYT (~ ThanhTien)
                        ado.TOTAL_PRICE_BHYT = g.Sum(o => Math.Round(o.TOTAL_PRICE_BHYT, 2, MidpointRounding.AwayFromZero));
                        ado.TOTAL_PRICE_HEIN = g.Sum(o => Math.Round(o.VIR_TOTAL_HEIN_PRICE ?? 0m, 2, MidpointRounding.AwayFromZero));        // T_BHTT (quỹ BHYT trả)
                        // T_BNCCT (BN cùng chi trả): VIR_TOTAL_PATIENT_PRICE_BHYT đã được DERIVE = Round(quỹ+CCT,2)-Round(quỹ,2) theo grain
                        // dòng gốc ở DataInputProcess (khớp cổng: Round(payable,2)-TongBHTT). Ở đây chỉ cộng dồn theo đối tượng thẻ,
                        // Round(.,2) là no-op vì đầu vào đã 2 số lẻ.
                        ado.TOTAL_PRICE_PATIENT = g.Sum(o => Math.Round(o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0m, 2, MidpointRounding.AwayFromZero));
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => Math.Round(o.OTHER_SOURCE_PRICE ?? 0m, 2, MidpointRounding.AwayFromZero));        // T_NGUONKHAC
                        ado.TOTAL_PRICE_VP = g.Sum(o => Math.Round(o.TOTAL_PRICE_VP, 2, MidpointRounding.AwayFromZero));                      // viện phí đầy đủ
                        // Tự trả trong phạm vi được thanh toán: giữ công thức gốc per-line (base NO_EXPEND×tỷ_lệ, đã floor per-line ở SereServADO), chỉ đổi grain 2 số/dòng.
                        ado.TOTAL_PRICE_PATIENT_SELF = g.Sum(o => Math.Round(o.TOTAL_PRICE_PATIENT_SELF, 2, MidpointRounding.AwayFromZero));

                        // --- Cột residual: DERIVE từ các tổng ĐÃ làm tròn ở trên (giống cổng derive TongBNTT) -> giữ đẳng thức. PHẢI đặt SAU các cột trực tiếp. ---
                        // BN tự trả phần còn lại trong phạm vi (không floor, không tỷ lệ): TOTAL_PRICE = HEIN + PATIENT + OTHER + NO_PAY_RATE khớp chính xác.
                        ado.TOTAL_PRICE_PATIENT_NO_PAY_RATE = (ado.TOTAL_PRICE ?? 0m) - (ado.TOTAL_PRICE_HEIN ?? 0m) - (ado.TOTAL_PRICE_PATIENT ?? 0m) - (ado.OTHER_SOURCE_PRICE ?? 0m);
                        // Tổng BN tự trả (gồm cả chênh ngoài phạm vi): VP - HEIN - PATIENT - OTHER, floor 0 -> giữ đẳng thức VP = HEIN + PATIENT + OTHER + LEFT.
                        decimal patientPriceLeft = ado.TOTAL_PRICE_VP - (ado.TOTAL_PRICE_HEIN ?? 0m) - (ado.TOTAL_PRICE_PATIENT ?? 0m) - (ado.OTHER_SOURCE_PRICE ?? 0m);
                        ado.TOTAL_PATIENT_PRICE_LEFT = patientPriceLeft < 0 ? 0m : patientPriceLeft;
                        var typeAlter = g.First().PatientTypeAlter;
                        if (typeAlter != null &&
                            typeAlter.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE
                            && typeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE
                            && rdo.TreatmentView.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            ado.RATIO_STR = ((int)(((g.FirstOrDefault(o => o.HEIN_RATIO.HasValue && !o.STENT_ORDER.HasValue) ?? g.First()).HEIN_RATIO ?? 0) * 100)) + "%";
                        }

                        patyAlterBHYTADOs.Add(ado);
                    }
                    if (patyAlterBHYTADOs != null && patyAlterBHYTADOs.Count > 0)
                    {
                        patyAlterBHYTADOs = patyAlterBHYTADOs.OrderBy(o => o.LOG_TIME).ThenBy(o => o.KEY).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
