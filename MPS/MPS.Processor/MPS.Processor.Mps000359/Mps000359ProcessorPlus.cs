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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000359.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000359
{
    public partial class Mps000359Processor : AbstractProcessor
    {
        private void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                this.sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                var sereServADOTemps2 = new List<SereServADO>();
                var allSereServs = rdo.SereServs;
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, allSereServs, rdo.SereServExts, rdo.HeinServiceTypes,
                                              rdo.Services, rdo.Rooms, rdo.medicineTypes, rdo.materialTypes, rdo.PatientTypeCFG,
                                              rdo.HisConfigValue, rdo.HisServiceUnit, rdo.ListOtherPaySource, rdo.ListSereServBills,
                                              rdo.ListSereServDeposits, rdo.ListSeseDepoRepays));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));
                //sereServ la hao phi gom nhom
                var sereServBHYTGroups = sereServADOTemps
                    .Where(o =>
                         o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID != rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.IS_NO_EXECUTE != 1
                        && o.PRICE > 0
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.VIR_PRICE_NO_EXPEND,
                        o.IS_EXPEND,
                        o.GROUP_DEPARTMENT_ID,
                        o.PATIENT_TYPE_ID
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.TOTAL_PRICE_PATIENT_NO_PAY_RATE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE);
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.IS_PAID = sereServBHYTGroup.Min(o => o.IS_PAID);//tất cả thanh toán min sẽ là 1 nếu có 1 dv chưa thanh toán min sẽ là 0
                    sereServ.IS_DEPOSIT = sereServBHYTGroup.Min(o => o.IS_DEPOSIT);//tất cả thanh toán min sẽ là 1 nếu có 1 dv chưa thanh toán min sẽ là 0
                    this.sereServADOs.Add(sereServ);
                }

                sereServADOTemps2.AddRange(from r in rdo.SereServs
                                           select new SereServADO(r, allSereServs, rdo.SereServExts, rdo.HeinServiceTypes,
                                               rdo.Services, rdo.Rooms, rdo.medicineTypes, rdo.materialTypes, rdo.PatientTypeCFG,
                                               rdo.HisConfigValue, rdo.HisServiceUnit, rdo.ListOtherPaySource, rdo.ListSereServBills,
                                               rdo.ListSereServDeposits, rdo.ListSeseDepoRepays));
                //sereServ la hao phi gom nhom
                var sereServBHYTGroups2 = sereServADOTemps2
                    .Where(o =>
                         o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID != rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.IS_NO_EXECUTE != 1
                        && o.PRICE >= 0
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.VIR_PRICE_NO_EXPEND,
                        o.IS_EXPEND,
                        o.GROUP_DEPARTMENT_ID,
                        o.PATIENT_TYPE_ID
                    }).ToList();

                this.sereServNoPriceADOs = new List<SereServADO>();
                foreach (var sereServBHYTGroup in sereServBHYTGroups2)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.TOTAL_PRICE_PATIENT_NO_PAY_RATE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE);
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.IS_PAID = sereServBHYTGroup.Min(o => o.IS_PAID);//tất cả thanh toán min sẽ là 1 nếu có 1 dv chưa thanh toán min sẽ là 0
                    sereServ.IS_DEPOSIT = sereServBHYTGroup.Min(o => o.IS_DEPOSIT);//tất cả thanh toán min sẽ là 1 nếu có 1 dv chưa thanh toán min sẽ là 0
                    this.sereServNoPriceADOs.Add(sereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<PatyAlterBhytADO> PatyAlterProcess(List<SereServADO> sereServADOs)
        {
            List<PatyAlterBhytADO> result = new List<PatyAlterBhytADO>();
            try
            {
                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var ssGroup = sereServADOs.GroupBy(o => new { o.KEY_PATY_ALTER, o.PATIENT_TYPE_ID });
                    foreach (var g in ssGroup)
                    {
                        PatyAlterBhytADO ado = new PatyAlterBhytADO();
                        ado = DataRawProcess.PatyAlterBHYTRawToADO(g.First().PatientTypeAlter, rdo.PatientTypeAlterAlls, rdo.Treatment, rdo.Branch, rdo.TreatmentTypes, rdo.CurrentPatyAlter);
                        ado.KEY = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_PRICE_HEIN = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PRICE_PATIENT = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PRICE_PATIENT_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.TOTAL_PRICE_PATIENT_NO_PAY_RATE = g.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.PATIENT_TYPE_ID = g.First().PATIENT_TYPE_ID;

                        result.Add(ado);
                    }

                    if (result != null && result.Count > 0)
                    {
                        result = result.OrderBy(o => o.LOG_TIME).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void GroupDisplayProcess()
        {
            try
            {
                this.patyAlterBHYTADOs = this.PatyAlterProcess(this.sereServADOs);

                this.GroupPatientType = this.GroupPatientTypeProcess(this.sereServADOs);

                this.GroupDepartments = this.GroupDepartmentProcess(this.sereServADOs);

                this.heinServiceTypeADOs = this.HeinServiceTypeProcess(this.sereServADOs);

                this.sereServADOs.ForEach(o =>
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

                this.HeinServiceTypeBeds = this.HeinServiceTypeBedProcess(this.sereServADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GroupDisplayProcess(List<SereServADO> sereServAdos, ref List<PatyAlterBhytADO> patyAlterBHYTADOs, ref List<HeinServiceTypeADO> heinServiceTypeADOs, ref List<HeinServiceTypeADO> HeinServiceTypeBeds, ref List<GroupDepartmentADO> GroupDepartments, ref List<GroupPatientTypeADO> GroupPatientType)
        {
            try
            {
                patyAlterBHYTADOs = this.PatyAlterProcess(sereServAdos);

                GroupPatientType = this.GroupPatientTypeProcess(sereServAdos);

                GroupDepartments = this.GroupDepartmentProcess(sereServAdos);

                heinServiceTypeADOs = this.HeinServiceTypeProcess(sereServAdos);

                sereServAdos.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

                HeinServiceTypeBeds = this.HeinServiceTypeBedProcess(sereServAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<GroupPatientTypeADO> GroupPatientTypeProcess(List<SereServADO> sereServADOs)
        {
            List<GroupPatientTypeADO> result = new List<GroupPatientTypeADO>();
            try
            {
                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var ssGroup = sereServADOs.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupPatientTypeADO ado = new GroupPatientTypeADO();
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.PATIENT_TYPE_ID = g.First().PATIENT_TYPE_ID;
                        if (rdo.HisPatientType != null)
                        {
                            HIS_PATIENT_TYPE patientType = rdo.HisPatientType.FirstOrDefault(o => o.ID == g.First().PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                ado.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                                ado.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }
                        }

                        result.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Gom nhóm theo loại dịch vụ thuật toán xử lý như sau
        /// - Giường
        /// Đây là xử lý gom nhóm loại cha là Giường nên sẽ không lấy các nhóm giường con (giường nội trú và ngoại trú, ...) 
        /// tạo 1 đối tượng là giường được gán ID hashcode trong class HeinServiceTypeExt
        /// - Thuốc
        /// Các thuốc trong danh mục, ngoài danh mục, cũng gom vào và tạo 1 đối tượng thuốc trong HeinServiceTypeExt
        /// - Vật tư
        /// --Vật tư y tế
        /// Tương tự thuốc (gom vào và tạo đối tượng với ID, NAME hashcode)
        /// --Gói vật tư
        /// Vì cần gom nhóm theo dịch vụ là gói nên ta gán HEIN_SERVICE_TYPE_ID của dịch vụ bằng chính PARENT_ID của các dịch vụ con
        /// từ đó để tách loại dịch vụ
        /// - Các dịch vụ khác vẫn lấy theo HEIN_SERVICE_TYPE_ID trong danh mục
        /// 
        /// </summary>
        private List<HeinServiceTypeADO> HeinServiceTypeProcess(List<SereServADO> sereServADOs)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.PATIENT_TYPE_ID }).ToList();

                List<long> parentIdVTs = this.sereServADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYTGroup.First().GROUP_DEPARTMENT_ID;
                    heinServiceType.PATIENT_TYPE_ID = sereServBHYTGroup.First().PATIENT_TYPE_ID;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE ?? 0);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);

                    int indexGoiVatTuYTe = 1;
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent.TDL_HEIN_SERVICE_BHYT_NAME);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
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
                        var lstGiuong = result.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID).ToList();
                        if (lstGiuong != null && lstGiuong.Count > 0)
                            continue;
                        else
                        {
                            heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                            heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }

                    result.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<HeinServiceTypeADO> HeinServiceTypeBedProcess(List<SereServADO> sereServADOs)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID, o.PATIENT_TYPE_ID }).ToList();

                long numOrderVTYT = 1;
                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                    heinServiceType.PATIENT_TYPE_ID = g.First().PATIENT_TYPE_ID;

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

                    result.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private List<GroupDepartmentADO> GroupDepartmentProcess(List<SereServADO> sereServADOs)
        {
            List<GroupDepartmentADO> result = new List<GroupDepartmentADO>();
            try
            {
                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var ssGroup = sereServADOs.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.PATIENT_TYPE_ID }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.PATIENT_TYPE_ID = g.First().PATIENT_TYPE_ID;
                        if (rdo.Departments != null)
                        {
                            HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == g.First().GROUP_DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                        }

                        result.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
