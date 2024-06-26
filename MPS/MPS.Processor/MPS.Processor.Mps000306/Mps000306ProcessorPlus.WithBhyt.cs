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
using MPS.Processor.Mps000306.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000306
{
    public partial class Mps000306Processor
    {
        private List<PatyAlterBhytADO> patyAlterBHYTADOs_WithBhyt;
        private List<HeinServiceTypeADO> heinServiceTypeADOs_WithBhyt;
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_WithBhyt;
        private List<GroupDepartmentADO> GroupDepartments_WithBhyt;

        private void DataInputProcess_WithBhyt()
        {
            try
            {
                sereServADOs_WithBhyt = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();

                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.Departments, rdo.medicineTypes, rdo.MedicineLines, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisServiceUnit, rdo.HisConfigValue, null, true));

                List<SereServADO> sereServAdoFees = new List<SereServADO>();
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                         o.AMOUNT > 0
                        && (o.PATIENT_TYPE_ID != rdo.PatientTypeCFG.PATIENT_TYPE__BHYT || (o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT && o.PRIMARY_PATIENT_TYPE_ID.HasValue))
                        && o.IS_NO_EXECUTE != 1
                        && o.PRICE > 0
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRICE,
                        o.IS_EXPEND,
                        o.GROUP_DEPARTMENT_ID
                    }).ToList();

                foreach (var sereServGroup in sereServGroups)
                {
                    SereServADO sereServ = sereServGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.OTHER_SOURCE_PRICE = sereServGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = (sereServ.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) - (sereServ.OTHER_SOURCE_PRICE ?? 0);
                    sereServ.TOTAL_PATIENT_PRICE_LEFT = sereServGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    sereServ.TOTAL_PRICE_VP = sereServGroup.Sum(o => o.TOTAL_PRICE_VP);
                    sereServADOs_WithBhyt.Add(sereServ);
                }

                this.PatyAlterProcess_WithBhyt();
                this.GroupDisplayProcess_WithBhyt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatyAlterProcess_WithBhyt()
        {
            try
            {
                this.patyAlterBHYTADOs_WithBhyt = new List<PatyAlterBhytADO>();
                if (sereServADOs_WithBhyt != null && sereServADOs_WithBhyt.Count > 0)
                {
                    var ssGroup = sereServADOs_WithBhyt.GroupBy(o => o.KEY_PATY_ALTER);
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
                        ado.TOTAL_PRICE_OTHER = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);

                        patyAlterBHYTADOs_WithBhyt.Add(ado);
                    }

                    if (patyAlterBHYTADOs_WithBhyt != null && patyAlterBHYTADOs_WithBhyt.Count > 0)
                    {
                        patyAlterBHYTADOs_WithBhyt = patyAlterBHYTADOs_WithBhyt.OrderBy(o => o.LOG_TIME).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GroupDisplayProcess_WithBhyt()
        {
            try
            {
                this.HeinServiceTypeProcess_WithBhyt();

                sereServADOs_WithBhyt.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

                this.HeinServiceTypeBedProcess_WithBhyt();

                this.GroupDepartmentProcess_WithBhyt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HeinServiceTypeProcess_WithBhyt()
        {
            try
            {
                heinServiceTypeADOs_WithBhyt = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOs_WithBhyt.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.GROUP_DEPARTMENT_ID__DepaRoom = sereServBHYT.GROUP_DEPARTMENT_ID__DepaRoom;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_CODE = sereServBHYT.GROUP_DEPARTMENT_ROOM_CODE;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_NAME = sereServBHYT.GROUP_DEPARTMENT_ROOM_NAME;
                    heinServiceType.GROUP_ROOM_ID__ExeRoom = sereServBHYT.GROUP_ROOM_ID__ExeRoom;
                    heinServiceType.GROUP_ROOM_CODE = sereServBHYT.GROUP_ROOM_CODE;
                    heinServiceType.GROUP_ROOM_NAME = sereServBHYT.GROUP_ROOM_NAME;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup
                       .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    heinServiceType.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    heinServiceType.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue
                        && (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT))
                    {
                        var lstGiuong = heinServiceTypeADOs_WithBhyt.Where(o => o.GROUP_DEPARTMENT_ID == heinServiceType.GROUP_DEPARTMENT_ID && o.ID == HeinServiceTypeExt.BED__ID).ToList();
                        if (lstGiuong != null && lstGiuong.Count > 0)
                            continue;
                        else
                        {
                            heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                            heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }

                    heinServiceTypeADOs_WithBhyt.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HeinServiceTypeBedProcess_WithBhyt()
        {
            try
            {
                HeinServiceTypeBeds_WithBhyt = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOs_WithBhyt.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    heinServiceType.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                    if (heinServiceType.PARENT_ID.HasValue && heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_NUM_ORDER;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g
                           .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        heinServiceType.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        heinServiceType.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    }

                    HeinServiceTypeBeds_WithBhyt.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GroupDepartmentProcess_WithBhyt()
        {
            try
            {
                GroupDepartments_WithBhyt = new List<GroupDepartmentADO>();
                if (sereServADOs_WithBhyt != null && sereServADOs_WithBhyt.Count > 0)
                {
                    var ssGroup = sereServADOs_WithBhyt.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID }).ToList();
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
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == g.First().GROUP_DEPARTMENT_ID);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        if (department != null)
                        {
                            ado.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                            ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                        }

                        GroupDepartments_WithBhyt.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
