using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000321.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000321
{
    public partial class Mps000321Processor : AbstractProcessor
    {
        private void DataInputProcessReal()
        {
            List<HIS_PATIENT_TYPE_ALTER> ListPta = rdo.PatientTypeAlterAlls.OrderByDescending(o => o.LOG_TIME).ToList();
            #region SereServ Real Sử dụng cho Bà Rịa 
            var sereServADOTemps = new List<SereServADO>();
            sereServADOTemps.AddRange(from r in rdo.SereServs
                                      select new SereServADO(r, rdo.SereServs, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services,
                                      rdo.Rooms, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisConfigValue, rdo.HisServiceUnit, ListPta,
                                      rdo.Treatment, rdo.ServiceReqs));

            sereServADOTemps = sereServADOTemps
                .Where(o =>
                    o.AMOUNT > 0
                    && o.IS_NO_EXECUTE != 1
                    && (!rdo.HisConfigValue.NotIncludeIsExpend || (rdo.HisConfigValue.NotIncludeIsExpend && o.IS_EXPEND != 1)))
                .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();
            sereServADOsReal = new List<SereServADO>();
            ProcessSereServReal(sereServADOTemps);
            HeinServiceTypeProcessReal();
            HeinServiceTypeProcessRealNotGroupDepartment();
            sereServADOsReal.ForEach(o =>
            {
                if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                    || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                {
                    long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                    o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                    o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                }
            });
            HeinServiceTypeBedProcessReal();
            HeinServiceTypeBedProcessRealNotGroupDepartment();
            GroupDepartmentProcessReal();
            #endregion
        }

        private void ProcessAddKeyList(ref Inventec.Common.FlexCellExport.ProcessObjectTag objectTag)
        {
            try
            {
                objectTag.AddObjectData(store, "HeinServiceTypeReal", heinServiceTypeADOsReal);
                objectTag.AddObjectData(store, "HeinServiceTypeRealNotGroupDepartment", heinServiceTypeADOsRealNotGroupDepartment);
                objectTag.AddObjectData(store, "ServiceReal", sereServADOsReal);
                objectTag.AddObjectData(store, "PatyAlterBHYTReal", patyAlterBHYTADOsReal);
                objectTag.AddObjectData(store, "PatyAlterBHYTAllReal", patyAlterBHYTADOsReal);
                objectTag.AddObjectData(store, "HeinServiceTypeBedReal", HeinServiceTypeBedsReal);
                objectTag.AddObjectData(store, "HeinServiceTypeBedRealNotGroupDepartment", HeinServiceTypeBedsRealNotGroupDepartment);
                objectTag.AddObjectData(store, "ReqExeDepartmentReal", GroupDepartmentsReal);

                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "ServiceReal", "KEY", "KEY_PATY_ALTER_REAL");
                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "HeinServiceTypeReal", "KEY", "KEY_PATY_ALTER_REAL");
                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "HeinServiceTypeBedReal", "KEY", "KEY_PATY_ALTER_REAL");
                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "HeinServiceTypeRealNotGroupDepartment", "KEY", "KEY_PATY_ALTER_REAL");
                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "HeinServiceTypeBedRealNotGroupDepartment", "KEY", "KEY_PATY_ALTER_REAL");
                objectTag.AddRelationship(store, "PatyAlterBHYTReal", "ReqExeDepartmentReal", "KEY", "KEY_PATY_ALTER_REAL");

                objectTag.AddRelationship(store, "ReqExeDepartmentReal", "ServiceReal", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartmentReal", "HeinServiceTypeReal", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartmentReal", "HeinServiceTypeBedReal", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeRealNotGroupDepartment", "ServiceReal", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeRealNotGroupDepartment", "HeinServiceTypeBedRealNotGroupDepartment", "ID", "PARENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeReal", "ServiceReal", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeReal", "HeinServiceTypeBedReal", "ID", "PARENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeBedReal", "ServiceReal", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBedRealNotGroupDepartment", "ServiceReal", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ProcessSereServReal(List<SereServADO> sereServADOTemps)
        {

            try
            {                //sereServ la bhyt, gom nhom
                var sereServBHYTRealGroups = sereServADOTemps
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRIMARY_PRICE,
                        o.PRICE_BHYT,
                        o.SERVICE_PAY_RATE,
                        o.BHYT_PAY_RATE,
                        o.IS_EXPEND,
                        o.NUMBER_OF_FILM,
                        o.KEY_PATY_ALTER_REAL,
                        o.HEIN_SERVICE_TYPE_ID,
                        o.GROUP_DEPARTMENT_ID,
                        o.STENT_ORDER
                    }).ToList();

                ProcessOtherSourceReal(sereServADOTemps);

                foreach (var sereServBHYTGroup in sereServBHYTRealGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.TOTAL_PRICE_PATIENT_SELF_DV = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                    sereServ.TOTAL_PRICE_PATIENT_SELF_TP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);

                    sereServADOsReal.Add(sereServ);

                    if (sereServ.STENT_ORDER.HasValue && sereServ.STENT_ORDER.Value > 1)
                    {
                        decimal quyBHTT = sereServ.VIR_TOTAL_HEIN_PRICE ?? 0;
                        decimal bnCungChiTra = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0;
                        decimal nguonKhac = sereServ.OTHER_SOURCE_PRICE ?? 0;

                        decimal bnHoacNguonKhac = bnCungChiTra > 0 ? bnCungChiTra : nguonKhac;

                        sereServ.TOTAL_PRICE_BHYT = quyBHTT + bnHoacNguonKhac;
                    }
                }
                sereServADOsReal = sereServADOsReal.OrderBy(o => o.STENT_ORDER ?? 0).ThenBy(o => o.SERVICE_NAME).ToList();

                try
                {
                    this.patyAlterBHYTADOsReal = new List<PatyAlterBhytADO>();
                    if (sereServADOsReal != null && sereServADOsReal.Count > 0)
                    {
                        var ssGroup = sereServADOsReal.GroupBy(o => o.KEY_PATY_ALTER_REAL).ToList();
                        foreach (var g in ssGroup)
                        {
                            PatyAlterBhytADO ado = new PatyAlterBhytADO();
                            ado = DataRawProcess.PatyAlterBHYTRawToADO(g.OrderBy(o => o.TDL_INTRUCTION_TIME).First().PatientTypeAlterReal, rdo.PatientTypeAlterAlls, rdo.Treatment, rdo.Branch, rdo.TreatmentTypes, rdo.CurrentPatyAlter, g.ToList());
                            ado.KEY = g.First().KEY_PATY_ALTER_REAL;
                            ado.TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                            ado.TOTAL_PRICE_BHYT = g.Sum(o => o.TOTAL_PRICE_BHYT);
                            ado.TOTAL_PRICE_HEIN = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                            ado.TOTAL_PRICE_PATIENT = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                            ado.TOTAL_PRICE_PATIENT_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                            ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                            ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                            ado.TOTAL_PRICE_OTHER = g.Sum(o => o.OTHER_SOURCE_PRICE);
                            ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                            patyAlterBHYTADOsReal.Add(ado);
                        }
                        if (patyAlterBHYTADOsReal != null && patyAlterBHYTADOsReal.Count > 0)
                        {
                            patyAlterBHYTADOsReal = patyAlterBHYTADOsReal.OrderBy(o => o.LOG_TIME).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void HeinServiceTypeProcessReal()
        {
            try
            {
                HeinServiceTypeBedsReal = new List<HeinServiceTypeADO>();

                heinServiceTypeADOsReal = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOsReal.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER_REAL, o.GROUP_DEPARTMENT_ID }).ToList();

                List<long> parentIdVTs = this.sereServADOsReal.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER_REAL = sereServBHYT.KEY_PATY_ALTER_REAL;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_DV = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV ?? 0);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_TP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP ?? 0);

                    heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PRICE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            //thêm 1 dòng 10. gói vật tư y tế cộng dồn các gói lại
                            HeinServiceTypeADO goi = heinServiceTypeADOsReal.FirstOrDefault(o => o.KEY_PATY_ALTER_REAL == heinServiceType.KEY_PATY_ALTER_REAL && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID);
                            if (goi != null)
                            {
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE += heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE += heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE += heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE += heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF += heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER_REAL = sereServBHYT.KEY_PATY_ALTER_REAL;
                                goi.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                                goi.ID = HeinServiceTypeExt.GOI_VT_Y_TE__ID;
                                goi.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                goi.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE = heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE = heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                heinServiceTypeADOsReal.Add(goi);
                            }

                            //chi tiết gói không kèm stent 2 trở đi
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
                            heinServiceType.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = indexGoiVatTuYTe;

                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent.TDL_HEIN_SERVICE_BHYT_NAME);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            indexGoiVatTuYTe++;
                        }
                        else if (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                        {
                            var lstGiuong = heinServiceTypeADOsReal.Where(o => o.KEY_PATY_ALTER_REAL == heinServiceType.KEY_PATY_ALTER_REAL && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID).ToList();
                            if (lstGiuong != null && lstGiuong.Count > 0)
                                continue;
                            else
                            {
                                heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                                heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                                heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            }
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

                    heinServiceTypeADOsReal.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HeinServiceTypeProcessRealNotGroupDepartment()
        {
            try
            {
                HeinServiceTypeBedsRealNotGroupDepartment = new List<HeinServiceTypeADO>();

                heinServiceTypeADOsRealNotGroupDepartment = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOsReal.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER_REAL }).ToList();

                List<long> parentIdVTs = this.sereServADOsReal.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER_REAL = sereServBHYT.KEY_PATY_ALTER_REAL;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_DV = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV ?? 0);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_TP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP ?? 0);

                    heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PRICE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            //thêm 1 dòng 10. gói vật tư y tế cộng dồn các gói lại
                            HeinServiceTypeADO goi = heinServiceTypeADOsRealNotGroupDepartment.FirstOrDefault(o => o.KEY_PATY_ALTER_REAL == heinServiceType.KEY_PATY_ALTER_REAL && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID);
                            if (goi != null)
                            {
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE += heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE += heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE += heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE += heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF += heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER_REAL = sereServBHYT.KEY_PATY_ALTER_REAL;
                                goi.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                                goi.ID = HeinServiceTypeExt.GOI_VT_Y_TE__ID;
                                goi.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                goi.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE = heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE = heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                heinServiceTypeADOsRealNotGroupDepartment.Add(goi);
                            }

                            //chi tiết gói không kèm stent 2 trở đi
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
                            heinServiceType.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = indexGoiVatTuYTe;

                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent.TDL_HEIN_SERVICE_BHYT_NAME);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            indexGoiVatTuYTe++;
                        }
                        else if (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                        {
                            var lstGiuong = heinServiceTypeADOsRealNotGroupDepartment.Where(o => o.KEY_PATY_ALTER_REAL == heinServiceType.KEY_PATY_ALTER_REAL && o.ID == HeinServiceTypeExt.BED__ID).ToList();
                            if (lstGiuong != null && lstGiuong.Count > 0)
                                continue;
                            else
                            {
                                heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                                heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                                heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            }
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

                    heinServiceTypeADOsRealNotGroupDepartment.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HeinServiceTypeBedProcessReal()
        {
            try
            {
                var sereServBHYTGroups = sereServADOsReal.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER_REAL, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER_REAL = g.First().KEY_PATY_ALTER_REAL;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    if (heinServiceType.PARENT_ID.HasValue && heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_NUM_ORDER;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g
                           .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                    }

                    HeinServiceTypeBedsReal.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void HeinServiceTypeBedProcessRealNotGroupDepartment()
        {
            try
            {
                var sereServBHYTGroups = sereServADOsReal.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER_REAL, o.HEIN_SERVICE_TYPE_PARENT_1_ID }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER_REAL = g.First().KEY_PATY_ALTER_REAL;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    if (heinServiceType.PARENT_ID.HasValue && heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_NUM_ORDER;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g
                           .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                    }

                    HeinServiceTypeBedsRealNotGroupDepartment.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GroupDepartmentProcessReal()
        {
            try
            {
                GroupDepartmentsReal = new List<GroupDepartmentADO>();
                if (sereServADOsReal != null && sereServADOsReal.Count > 0)
                {
                    var ssGroup = sereServADOsReal.GroupBy(o => new { o.KEY_PATY_ALTER_REAL, o.GROUP_DEPARTMENT_ID }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER_REAL = g.First().KEY_PATY_ALTER_REAL;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == g.First().GROUP_DEPARTMENT_ID);
                        if (department != null)
                        {
                            ado.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                            ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                        }

                        ado.DEPARTMENT_IN_TIME = g.Min(s => s.TDL_INTRUCTION_TIME);
                        if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                        {
                            var departments = rdo.DepartmentTrans.Where(o => o.DEPARTMENT_ID == g.Key.GROUP_DEPARTMENT_ID).ToList();
                            if (departments != null && departments.Count > 0)
                            {
                                ado.DEPARTMENT_IN_TIME = departments.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 99999999999999).First().DEPARTMENT_IN_TIME ?? 99999999999999;
                            }
                        }

                        GroupDepartmentsReal.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessOtherSourceReal(List<SereServADO> sereServADOs)
        {
            try
            {
                if (sereServADOs != null && sereServADOs.Count > 0 && rdo.ListOtherPaySource != null && rdo.ListOtherPaySource.Count > 0)
                {
                    this.ListOtherSourceReal = new List<OtherSourceADO>();
                    var otherGroup = sereServADOs.GroupBy(o => o.OTHER_PAY_SOURCE_ID).ToList();
                    foreach (var item in otherGroup)
                    {
                        var otherPaySource = rdo.ListOtherPaySource.FirstOrDefault(o => o.ID == item.Key);
                        if (otherPaySource != null)
                        {
                            OtherSourceADO ado = new OtherSourceADO();
                            ado.OTHER_PAY_SOURCE_CODE = otherPaySource.OTHER_PAY_SOURCE_CODE;
                            ado.OTHER_PAY_SOURCE_NAME = otherPaySource.OTHER_PAY_SOURCE_NAME;
                            ado.TOTAL_PRICE = item.Sum(s => s.OTHER_SOURCE_PRICE ?? 0);
                            ado.TOTAL_PRICE_STR = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(ado.TOTAL_PRICE).ToString());
                            this.ListOtherSourceReal.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ListOtherSourceReal = new List<OtherSourceADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
