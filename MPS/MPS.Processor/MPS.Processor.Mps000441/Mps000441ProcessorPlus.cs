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
using MPS.Processor.Mps000441.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000441
{
    public partial class Mps000441Processor : AbstractProcessor
    {
        private void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);

                List<HIS_PATIENT_TYPE_ALTER> ListPta = rdo.PatientTypeAlterAlls.OrderByDescending(o => o.LOG_TIME).ToList();

                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.SereServs, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisConfigValue, rdo.HisServiceUnit, rdo.Departments, rdo.ListSereServBill, ListPta));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));

                sereServADOTemps = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_NO_EXECUTE != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();

                //sereServ la bhyt, gom nhom
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
                        o.GROUP_DEPARTMENT_ID
                    }).ToList();

                ProcessOtherSource(sereServADOTemps);

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
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
                    sereServ.TOTAL_EXPEND_PRICE = sereServBHYTGroup.Sum(o => o.TOTAL_EXPEND_PRICE);

                    sereServADOs.Add(sereServ);
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();

                #region sereServADOsNoDepa
                sereServADOsNoDepa = new List<SereServADO>();
                var sereServADOTempNoDepas = new List<SereServADO>();
                sereServADOTempNoDepas.AddRange(from r in rdo.SereServs
                                                select new SereServADO(r, rdo.SereServs, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisConfigValue, rdo.HisServiceUnit, rdo.Departments, rdo.ListSereServBill, ListPta, true));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));

                sereServADOTempNoDepas = sereServADOTempNoDepas
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_NO_EXECUTE != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();

                //sereServ la bhyt, gom nhom
                var sereServBHYTGroupNoDepas = sereServADOTempNoDepas
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
                        o.GROUP_DEPARTMENT_ID
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroupNoDepas)
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
                    sereServ.TOTAL_EXPEND_PRICE = sereServBHYTGroup.Sum(o => o.TOTAL_EXPEND_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                    sereServ.TOTAL_PRICE_PATIENT_SELF_TOTAL = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

                    if (sereServ.IS_EXPEND != 1)
                    {
                        sereServ.GROUP_DEPARTMENT_ID = "";
                        sereServ.GROUP_DEPARTMENT_ROOM_CODE = "";
                        sereServ.GROUP_DEPARTMENT_ROOM_NAME = "";
                    }

                    sereServADOsNoDepa.Add(sereServ);
                }

                sereServADOsNoDepa = sereServADOsNoDepa.OrderBy(o => o.SERVICE_NAME).ToList();
                #endregion

                #region sereServADOsExpend
                sereServADOsExpend = new List<SereServADO>();
                var sereServADOTempExpends = new List<SereServADO>();
                sereServADOTempExpends.AddRange(from r in rdo.SereServs
                                                select new SereServADO(r, rdo.SereServs, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisConfigValue, rdo.HisServiceUnit, rdo.Departments, rdo.ListSereServBill, ListPta));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServADOTempExpends__" + Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTempExpends), sereServADOTempExpends));

                sereServADOTempExpends = sereServADOTempExpends
                    .Where(o =>
                        o.IS_EXPEND == 1
                        && o.AMOUNT > 0
                        && o.IS_NO_EXECUTE != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();

                //sereServ la bhyt, gom nhom
                var sereServBHYTGroupExpends = sereServADOTempExpends
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRIMARY_PRICE,
                        o.PRICE_BHYT,
                        o.SERVICE_PAY_RATE,
                        o.BHYT_PAY_RATE,
                        o.NUMBER_OF_FILM,
                        o.KEY_PATY_ALTER,
                        o.HEIN_SERVICE_TYPE_ID,
                        o.GROUP_DEPARTMENT_ID
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroupExpends)
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
                    sereServ.TOTAL_EXPEND_PRICE = sereServBHYTGroup.Sum(o => o.TOTAL_EXPEND_PRICE);

                    sereServADOsExpend.Add(sereServ);
                }

                sereServADOsExpend = sereServADOsExpend.OrderBy(o => o.SERVICE_NAME).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GroupDisplayProcess()
        {
            try
            {
                this.patyAlterBHYTADOs = this.PatyAlterProcess(sereServADOs);

                this.heinServiceTypeADOs = this.HeinServiceTypeProcess(sereServADOs);

                sereServADOs.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

                this.HeinServiceTypeBeds = this.HeinServiceTypeBedProcess(sereServADOs);

                this.GroupDepartments = this.GroupDepartmentProcess(sereServADOs);

                this.GroupExpends = this.GroupExpendProcess(sereServADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GroupDisplayProcess(List<SereServADO> sereServAdos, ref List<PatyAlterBhytADO> patyAlterBHYTADOs, ref List<HeinServiceTypeADO> heinServiceTypeADOs, ref List<HeinServiceTypeADO> HeinServiceTypeBeds, ref List<GroupDepartmentADO> GroupDepartments, ref List<GroupDepartmentADO> GroupExpends)
        {
            try
            {
                patyAlterBHYTADOs = this.PatyAlterProcess(sereServAdos);

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

                GroupDepartments = this.GroupDepartmentProcess(sereServAdos);

                GroupExpends = this.GroupExpendProcess(sereServAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GroupDisplayProcessExpend(List<SereServADO> sereServAdos, ref List<PatyAlterBhytADO> patyAlterBHYTADOs, ref List<HeinServiceTypeADO> heinServiceTypeADOs, ref List<HeinServiceTypeADO> HeinServiceTypeBeds, ref List<GroupDepartmentADO> GroupDepartments, ref List<GroupDepartmentADO> GroupExpends)
        {
            try
            {
                patyAlterBHYTADOs = this.PatyAlterProcess(sereServAdos);

                heinServiceTypeADOs = this.HeinServiceTypeProcessExpendType(sereServAdos);

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

                HeinServiceTypeBeds = this.HeinServiceTypeBedProcessExpendType(sereServAdos);

                GroupDepartments = this.GroupDepartmentProcessExpendType(sereServAdos);

                GroupExpends = this.GroupExpendProcessExpendType(sereServAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<PatyAlterBhytADO> PatyAlterProcess(List<SereServADO> sereServAdo)
        {
            List<PatyAlterBhytADO> result = new List<PatyAlterBhytADO>();
            try
            {
                if (sereServAdo != null && sereServAdo.Count > 0)
                {
                    var ssGroup = sereServAdo.GroupBy(o => o.KEY_PATY_ALTER).ToList();
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
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.TOTAL_PRICE_OTHER = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);

                        ado.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        ado.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

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
        private List<HeinServiceTypeADO> HeinServiceTypeProcess(List<SereServADO> sereServAdo)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServAdo.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.IS_EXPEND }).ToList();

                List<long> parentIdVTs = sereServAdo.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.IS_EXPEND = sereServBHYT.IS_EXPEND;
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
                    heinServiceType.TOTAL_EXPEND_PRICE = sereServBHYTGroup.Sum(o => o.TOTAL_EXPEND_PRICE);

                    heinServiceType.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_TOTAL = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            //thêm 1 dòng 10. gói vật tư y tế cộng dồn các gói lại
                            HeinServiceTypeADO goi = result.FirstOrDefault(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID);
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
                                goi.TOTAL_EXPEND_PRICE += heinServiceType.TOTAL_EXPEND_PRICE;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
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
                                goi.TOTAL_EXPEND_PRICE += heinServiceType.TOTAL_EXPEND_PRICE;
                                result.Add(goi);
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
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent != null ? sereServParent.TDL_HEIN_SERVICE_BHYT_NAME : "");
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            indexGoiVatTuYTe++;
                        }
                        else if (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                        {
                            var lstGiuong = result.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID && o.IS_EXPEND == heinServiceType.IS_EXPEND).ToList();
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

                    result.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private List<HeinServiceTypeADO> HeinServiceTypeProcessExpendType(List<SereServADO> sereServAdo)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServAdo.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.ExpendType }).ToList();

                List<long> parentIdVTs = sereServAdo.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.ExpendType = sereServBHYT.ExpendType;
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
                    heinServiceType.TOTAL_EXPEND_PRICE = sereServBHYTGroup.Sum(o => o.TOTAL_EXPEND_PRICE);

                    heinServiceType.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_TOTAL = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            //thêm 1 dòng 10. gói vật tư y tế cộng dồn các gói lại
                            HeinServiceTypeADO goi = result.FirstOrDefault(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID);
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
                                goi.TOTAL_EXPEND_PRICE += heinServiceType.TOTAL_EXPEND_PRICE;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
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
                                goi.TOTAL_EXPEND_PRICE += heinServiceType.TOTAL_EXPEND_PRICE;
                                result.Add(goi);
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
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent != null ? sereServParent.TDL_HEIN_SERVICE_BHYT_NAME : "");
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            indexGoiVatTuYTe++;
                        }
                        else if (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                        {
                            var lstGiuong = result.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID && o.ExpendType == heinServiceType.ExpendType).ToList();
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

                    result.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<HeinServiceTypeADO> HeinServiceTypeBedProcess(List<SereServADO> sereServAdo)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServAdo.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID, o.IS_EXPEND }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                    heinServiceType.IS_EXPEND = g.First().IS_EXPEND;

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
                        heinServiceType.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        heinServiceType.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);
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
        private List<HeinServiceTypeADO> HeinServiceTypeBedProcessExpendType(List<SereServADO> sereServAdo)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServAdo.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID, o.ExpendType }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                    heinServiceType.ExpendType = g.First().ExpendType;

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
                        heinServiceType.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        heinServiceType.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        heinServiceType.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);
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

        private List<GroupDepartmentADO> GroupDepartmentProcess(List<SereServADO> sereServAdo)
        {
            List<GroupDepartmentADO> result = new List<GroupDepartmentADO>();
            try
            {
                if (sereServAdo != null && sereServAdo.Count > 0)
                {
                    var ssGroup = sereServAdo.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.IS_EXPEND }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.IS_EXPEND = g.First().IS_EXPEND;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.DEPARTMENT_CODE = g.First().GROUP_DEPARTMENT_ROOM_CODE;
                        ado.DEPARTMENT_NAME = g.First().GROUP_DEPARTMENT_ROOM_NAME;
                        ado.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        ado.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

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
        private List<GroupDepartmentADO> GroupDepartmentProcessExpendType(List<SereServADO> sereServAdo)
        {
            List<GroupDepartmentADO> result = new List<GroupDepartmentADO>();
            try
            {
                if (sereServAdo != null && sereServAdo.Count > 0)
                {
                    var ssGroup = sereServAdo.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID}).ToList();
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
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.DEPARTMENT_CODE = g.First().GROUP_DEPARTMENT_ROOM_CODE;
                        ado.DEPARTMENT_NAME = g.First().GROUP_DEPARTMENT_ROOM_NAME;
                        ado.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        ado.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

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

        private List<GroupDepartmentADO> GroupExpendProcess(List<SereServADO> sereServAdo)
        {
            List<GroupDepartmentADO> result = new List<GroupDepartmentADO>();
            try
            {
                if (sereServAdo != null && sereServAdo.Count > 0)
                {
                    var ssGroup = sereServAdo.GroupBy(o => new { o.KEY_PATY_ALTER, o.IS_EXPEND }).ToList();
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
                        ado.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.IS_EXPEND = g.First().IS_EXPEND;
                        ado.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        ado.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

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

        private List<GroupDepartmentADO> GroupExpendProcessExpendType(List<SereServADO> sereServAdo)
        {
            List<GroupDepartmentADO> result = new List<GroupDepartmentADO>();
            try
            {
                if (sereServAdo != null && sereServAdo.Count > 0)
                {
                    var ssGroup = sereServAdo.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.ExpendType }).ToList();
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
                        ado.TOTAL_EXPEND_PRICE = g.Sum(o => o.TOTAL_EXPEND_PRICE);
                        ado.TOTAL_PRICE_PATIENT_SELF_DV = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_DV);
                        ado.TOTAL_PRICE_PATIENT_SELF_TP = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TP);
                        ado.ExpendType = g.First().ExpendType;
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND_TOTAL);
                        ado.TOTAL_PRICE_PATIENT_SELF_TOTAL = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_TOTAL);

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
