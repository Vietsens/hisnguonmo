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
using MPS.Processor.Mps000128.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000128.ADO;

namespace MPS.Processor.Mps000128
{
    public partial class Mps000128Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.PatyAlter);
                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();

                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlter));


                sereServKTCADO = new SereServADO(rdo.SereServKTC, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlter);

                #region dịch vụ khám, pttt, ptc gom theo khoa thực hiện
                foreach (var item in sereServADOTemps)
                {
                    if (item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID
                        || item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__SURG_MISU_ID
                        || item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                    {
                        item.TDL_REQUEST_DEPARTMENT_ID = item.TDL_EXECUTE_DEPARTMENT_ID;
                    }
                }
                #endregion


                //sereServ la bhyt, gom nhom
                var sereServInPackageGroups = sereServADOTemps
                    .Where(o =>
                        o.PARENT_ID == rdo.SereServKTC.ID
                        && o.AMOUNT > 0
                        && o.IS_OUT_PARENT_FEE != 1
                        && o.IS_NO_EXECUTE != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.IS_EXPEND,
                        o.PRICE_BHYT,
                        o.TDL_REQUEST_DEPARTMENT_ID
                    }).ToList();

                foreach (var sereServInPackageGroup in sereServInPackageGroups)
                {
                    SereServADO sereServ = sereServInPackageGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServInPackageGroup.Sum(o => o.AMOUNT);
                    if (rdo.SereServKTC.TDL_EXECUTE_DEPARTMENT_ID == sereServInPackageGroup.First().TDL_REQUEST_DEPARTMENT_ID)
                    {

                        sereServ.VIR_PRICE = sereServ.VIR_PRICE_NO_EXPEND;
                        sereServ.VIR_TOTAL_PRICE = sereServInPackageGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        sereServ.TOTAL_PRICE_EXPEND = null;
                    }
                    else
                    {
                        sereServ.VIR_PRICE = sereServ.VIR_PRICE;
                        sereServ.VIR_TOTAL_PRICE = sereServInPackageGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServ.TOTAL_PRICE_EXPEND = sereServInPackageGroup.Sum(o => o.TOTAL_PRICE_EXPEND);
                    }
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServInPackageGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServInPackageGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServInPackageGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    
                    sereServADOs.Add(sereServ);
                }
                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void HeinServiceTypeProcess()
        {
            try
            {
                heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                departmentADOs = new List<DepartmentADO>();
                var sereServDepartmentGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();

                foreach (var sereServDepartmentGroup in sereServDepartmentGroups)
                {

                    SereServADO sereServDepartment = sereServDepartmentGroup.FirstOrDefault();
                    HIS_DEPARTMENT department = rdo.Department.FirstOrDefault(o => o.ID == sereServDepartment.TDL_REQUEST_DEPARTMENT_ID);
                    DepartmentADO departmentADO = new DepartmentADO();
                    if (department != null)
                    {
                        departmentADO.ID = department.ID;
                        departmentADO.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                        departmentADO.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    departmentADO.TOTAL_PRICE_DEPARTMENT = sereServDepartmentGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServDepartmentGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServDepartmentGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    departmentADO.TOTAL_PRICE_EXPEND_DEPARTMENT = sereServDepartmentGroup.Sum(o => o.TOTAL_PRICE_EXPEND) ?? 0;
                    departmentADOs.Add(departmentADO);

                    var sereServGroups = sereServDepartmentGroup.OrderBy(o => o.TDL_HEIN_SERVICE_TYPE_ID ?? 999999)
                            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();

                    foreach (var sereServGroup in sereServGroups)
                    {
                        HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                        if (sereServGroup.First().TDL_HEIN_SERVICE_TYPE_ID.HasValue)
                        {
                            heinServiceType.ID = sereServGroup.First().TDL_HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServGroup.First().HEIN_SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                        }
                        heinServiceType.REQUEST_DEPARTMENT_ID = sereServDepartment.TDL_REQUEST_DEPARTMENT_ID;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServGroup
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        heinServiceType.TOTAL_PRICE_EXPEND_HEIN_SERVICE_TYPE = sereServGroup.Sum(o => o.TOTAL_PRICE_EXPEND??0);
                        heinServiceTypeADOs.Add(heinServiceType);
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
