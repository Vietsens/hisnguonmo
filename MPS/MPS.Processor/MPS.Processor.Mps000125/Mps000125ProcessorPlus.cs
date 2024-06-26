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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000125.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000125.ADO;

namespace MPS.Processor.Mps000125
{
    public partial class Mps000125Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.PatyAlter);
                dicSereServ = new Dictionary<DicKey.SERE_SERV, List<SereServADO>>();
                dicSereServ[DicKey.SERE_SERV.ALL] = new List<SereServADO>();
                dicSereServ[DicKey.SERE_SERV.HIGHTECH] = new List<SereServADO>();
                dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT] = new List<SereServADO>();
                dicSereServ[DicKey.SERE_SERV.VTTT] = new List<SereServADO>();

                //Dich vu kham, dich vu pttt, ptc gom theo khoa thuc hien


                #region SereServADOs
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlter, rdo.Department, rdo.MaterialTypes));

                dicSereServ[DicKey.SERE_SERV.ALL] = sereServADOTemps
                    .Where(o =>
                        (o.HEIN_CARD_NUMBER != null && o.HEIN_CARD_NUMBER.Equals(rdo.PatyAlter.HEIN_CARD_NUMBER))
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1
                        && (o.VIR_TOTAL_HEIN_PRICE == 0 && o.VIR_TOTAL_PRICE == 0 ? false : true)
                        && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();

                if (dicSereServ[DicKey.SERE_SERV.ALL] == null)
                    throw new Exception("sereServADOs is null");
                #endregion

                #region dịch vụ khám, pttt, ptc gom theo khoa thực hiện
                foreach (var item in dicSereServ[DicKey.SERE_SERV.ALL])
                {
                    if (item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID
                        || item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__SURG_MISU_ID
                        || item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                    {
                        item.TDL_REQUEST_DEPARTMENT_ID = item.TDL_EXECUTE_DEPARTMENT_ID;
                        item.REQUEST_DEPARTMENT_NAME = item.EXECUTE_DEPARTMENT_NAME;
                    }
                }
                #endregion

                //Loc dich
                #region dich vu ktc
                dicSereServ[DicKey.SERE_SERV.HIGHTECH] = dicSereServ[DicKey.SERE_SERV.ALL]
                    .Select(o => new SereServADO(o)).Where(o =>
                        o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID).ToList();

                List<SereServADO> sereServHighTechNotVTTTADOs = new List<SereServADO>();

                if (dicSereServ[DicKey.SERE_SERV.HIGHTECH] != null && dicSereServ[DicKey.SERE_SERV.HIGHTECH].Count > 0)
                {
                    foreach (var item in dicSereServ[DicKey.SERE_SERV.HIGHTECH])
                    {
                        //Stent thứ nhất theo dịch vụ ktc
                        var sereServStent1 = dicSereServ[DicKey.SERE_SERV.ALL].Where(o => o.IS_STENT.HasValue && o.IS_STENT == 1 && o.PARENT_ID == item.ID)
                        .OrderBy(p => p.STENT_ORDER ?? 9999999).ThenBy(p => p.TDL_INTRUCTION_TIME).FirstOrDefault();

                        //Vật tư thay thế là stent thứ nhất hoặc là vttt thì gom với dịch vụ ktc
                        //Nếu là stent thứ 2 thì cho ra ngoài danh mục vttt
                        List<SereServADO> sereServVTTTs = dicSereServ[DicKey.SERE_SERV.ALL].Select(o => new SereServADO(o))
                            .Where(o =>
                    o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID
                    && o.PARENT_ID == item.ID
                    && ((o.IS_STENT ?? 0) != 1 || (sereServStent1 != null ? (o.ID == sereServStent1.ID) : true))).ToList();

                        //Inventec.Common.Logging.LogSystem.Error("Service code:" + item.TDL_SERVICE_CODE + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServVTTTs), sereServVTTTs.Select(o => new { o.TDL_SERVICE_CODE, o.TOTAL_PRICE_BHYT, o.VIR_TOTAL_HEIN_PRICE, o.VIR_TOTAL_PATIENT_PRICE_BHYT,o.REQUEST_DEPARTMENT_NAME,o.TDL_HEIN_SERVICE_TYPE_ID,o.PARENT_ID })));
                        if (sereServVTTTs != null && sereServVTTTs.Count > 0)
                        {
                            sereServVTTTs.ForEach(o => o.TDL_REQUEST_DEPARTMENT_ID = item.TDL_REQUEST_DEPARTMENT_ID);
                            item.TOTAL_PRICE_VTTT = sereServVTTTs.Sum(o => o.TOTAL_PRICE_BHYT);
                            item.TOTAL_HEIN_PRICE_VTTT = sereServVTTTs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            item.TOTAL_PATIENT_PRICE_VTTT = sereServVTTTs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                            item.TOTAL_PRICE_PATIENT_SELF_VTTT = sereServVTTTs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                            dicSereServ[DicKey.SERE_SERV.VTTT].AddRange(sereServVTTTs);
                        }
                        else
                        {
                            sereServHighTechNotVTTTADOs.Add(item);
                        }
                    }
                    dicSereServ[DicKey.SERE_SERV.HIGHTECH] = dicSereServ[DicKey.SERE_SERV.HIGHTECH].Except(sereServHighTechNotVTTTADOs).ToList();
                }
                #endregion

                #region dich vu con lai
                dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT] = dicSereServ[DicKey.SERE_SERV.ALL].Select(o => new SereServADO(o)).Where(o =>
                    (dicSereServ[DicKey.SERE_SERV.VTTT] != null ? !dicSereServ[DicKey.SERE_SERV.VTTT].Select(p => p.ID).Contains(o.ID) : true))
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 999999).ToList();

                var sereServLeftGroups = dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT].GroupBy(o =>
                    new { o.SERVICE_ID, o.PRICE_BHYT, o.TOTAL_HEIN_PRICE_ONE_AMOUNT, o.IS_EXPEND, o.IS_OUT_PARENT_FEE, o.TDL_REQUEST_DEPARTMENT_ID });
                dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT] = new List<SereServADO>();
                HIS_HEIN_SERVICE_TYPE heinServiceTypeSurgMisu = rdo.HeinServiceTypes.FirstOrDefault(o => o.ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__SURG_MISU_ID);
                foreach (var item in sereServLeftGroups)
                {
                    SereServADO sereServ = item.First();
                    //neu dich vu la ktc thì chuyen vao pttt
                    if (heinServiceTypeSurgMisu != null && sereServ.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                    {
                        sereServ.HEIN_SERVICE_TYPE_ID = heinServiceTypeSurgMisu.ID;
                        sereServ.TDL_HEIN_SERVICE_TYPE_ID = heinServiceTypeSurgMisu.ID;
                        sereServ.HEIN_SERVICE_TYPE_CODE = heinServiceTypeSurgMisu.HEIN_SERVICE_TYPE_CODE;
                        sereServ.HEIN_SERVICE_TYPE_NAME = heinServiceTypeSurgMisu.HEIN_SERVICE_TYPE_NAME;
                        sereServ.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeSurgMisu.NUM_ORDER;
                    }
                    sereServ.AMOUNT = item.Sum(o => o.AMOUNT);
                    sereServ.TOTAL_PRICE_BHYT = item.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = item
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = item.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = item.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT].Add(sereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
                #endregion

        internal void ProcessGroupSereServ()
        {
            try
            {
                dicHeinServiceType = new Dictionary<DicKey.HEIN_SERVICE_TYPE, List<HeinServiceTypeADO>>();
                dicDepartment = new Dictionary<DicKey.DEPARTMENT, List<DepartmentADO>>();
                dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.HIGHTECH] = new List<HeinServiceTypeADO>();
                dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.NOT_HIGHTECH] = new List<HeinServiceTypeADO>();
                dicDepartment[DicKey.DEPARTMENT.HIGHTECH] = new List<DepartmentADO>();
                dicDepartment[DicKey.DEPARTMENT.NOT_HIGHTECH] = new List<DepartmentADO>();


                var heinServiceTypeGroups = dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT].GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var heinServiceTypeGroup in heinServiceTypeGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServ = heinServiceTypeGroup.FirstOrDefault();
                    if (sereServ.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.ID = sereServ.HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServ.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                        .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.NOT_HIGHTECH].Add(heinServiceType);

                    var departmentGroups = heinServiceTypeGroup.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID)
                            .GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var departmentGroup in departmentGroups)
                    {
                        List<SereServADO> sereServDepartments = departmentGroup.ToList<SereServADO>();
                        DepartmentADO departmentADO = new DepartmentADO();
                        departmentADO.ID = departmentGroup.First().TDL_REQUEST_DEPARTMENT_ID;
                        departmentADO.DEPARTMENT_CODE = departmentGroup.First().REQUEST_DEPARTMENT_CODE;
                        departmentADO.DEPARTMENT_NAME = departmentGroup.First().REQUEST_DEPARTMENT_NAME;
                        if (heinServiceType.ID.HasValue)
                            departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceType.ID.Value;
                        departmentADO.TOTAL_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_PRICE_BHYT);
                        departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                        departmentADO.TOTAL_PRICE_PATIENT_SELF_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                        dicDepartment[DicKey.DEPARTMENT.NOT_HIGHTECH].Add(departmentADO);
                    }
                }

                var highTechHeinServiceTypeGroups = dicSereServ[DicKey.SERE_SERV.HIGHTECH].GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var highTechHeinServiceTypeGroup in highTechHeinServiceTypeGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServ = highTechHeinServiceTypeGroup.FirstOrDefault();
                    if (sereServ.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.ID = sereServ.HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServ.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = highTechHeinServiceTypeGroup.Sum(o => o.TOTAL_PRICE_VTTT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = highTechHeinServiceTypeGroup.Sum(o => o.TOTAL_HEIN_PRICE_VTTT);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = highTechHeinServiceTypeGroup
                        .Sum(o => o.TOTAL_PATIENT_PRICE_VTTT);
                    heinServiceType.TOTAL_PRICE_PATIENT_SELF_HEIN_SERVICE_TYPE = highTechHeinServiceTypeGroup
                        .Sum(o => o.TOTAL_PRICE_PATIENT_SELF_VTTT);

                    if (dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.NOT_HIGHTECH].Count == 0)
                    {
                        heinServiceType.ROW_POS = 1;
                    }
                    else
                    {
                        heinServiceType.ROW_POS = dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.NOT_HIGHTECH].Count + 1;
                    }

                    dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.HIGHTECH].Add(heinServiceType);

                    var departmentGroups = highTechHeinServiceTypeGroup.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID)
                            .GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var departmentGroup in departmentGroups)
                    {
                        List<SereServADO> sereServDepartments = departmentGroup.ToList<SereServADO>();
                        DepartmentADO departmentADO = new DepartmentADO();
                        departmentADO.ID = departmentGroup.First().TDL_REQUEST_DEPARTMENT_ID;
                        departmentADO.DEPARTMENT_CODE = departmentGroup.First().REQUEST_DEPARTMENT_CODE;
                        departmentADO.DEPARTMENT_NAME = departmentGroup.First().REQUEST_DEPARTMENT_NAME;
                        if (heinServiceType.ID.HasValue)
                            departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceType.ID.Value;

                        departmentADO.TOTAL_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_PRICE_VTTT);
                        departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_PATIENT_PRICE_VTTT);
                        departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_HEIN_PRICE_VTTT) ?? 0;
                        departmentADO.TOTAL_PRICE_PATIENT_SELF_DEPARTMENT = sereServDepartments.Sum(o => o.TOTAL_PRICE_PATIENT_SELF_VTTT);

                        dicDepartment[DicKey.DEPARTMENT.HIGHTECH].Add(departmentADO);
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
