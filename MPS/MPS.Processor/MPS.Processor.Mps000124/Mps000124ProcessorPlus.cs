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
using MPS.Processor.Mps000124.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000124.ADO;
using AutoMapper;

namespace MPS.Processor.Mps000124
{
    public partial class Mps000124Processor : AbstractProcessor
    {
        const long hightechGroupId = 9999;
        const long VTGroupId = 9998;

        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.PatyAlter);

                sereServADOs = new List<SereServADO>();
                sereServPTTTADOs = new List<SereServADO>();
                sereServInPackageOutFeeADOs = new List<SereServADO>();
                sereServLeftADOs = new List<SereServADO>();

                #region SereServADOs

                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          where r.AMOUNT > 0
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlter, rdo.Department, rdo.MaterialTypes));



                sereServADOs = sereServADOTemps
                    .Where(o =>
                        o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1
                        && o.VIR_TOTAL_PRICE > 0
                        && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();
                if (sereServADOs == null)
                    throw new Exception("sereServADOs is null");

                Inventec.Common.Logging.LogSystem.Error("sereServADOs" + sereServADOs.Where(o => o.PATIENT_TYPE_ID == 1).Sum(o => o.TOTAL_PRICE_BHYT));

                #endregion

                #region dịch vụ khám, pttt, ptc gom theo khoa thực hiện
                foreach (var item in sereServADOs)
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

                #region Goi dich vu

                //Cac dich vu dinh kem
                List<SereServADO> sereServInPackageTotals = new List<SereServADO>();
                //
                //Cac dich vu trong goi ngoai chi phi
                sereServInPackageOutFeeADOs = new List<SereServADO>();
                //
                //Id cua cac dich vu co goi, pttt hoac dvktc
                List<long> sereServIdHasPackages = sereServADOs.Where(o => o.PARENT_ID.HasValue).Select(o => o.PARENT_ID.Value).Distinct().ToList();

                sereServIdHasPackages = sereServADOs.Where(o => sereServIdHasPackages.Contains(o.ID) && (o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__SURG_MISU_ID || o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)).Select(s => s.ID).ToList();
                //
                //Cac dich vu 
                //1. Ton tai dich vu dinh kem
                //2. Dich vu la pttt hoac ky thuat cao va dich vu khong co goi
                //Hoi ruom ra. Can kiem tra lai cho nay
                sereServPTTTADOs = sereServADOs.Select(o => new SereServADO(o)).Where(o =>
                    (sereServIdHasPackages != null && sereServIdHasPackages.Contains(o.ID))
                    || ((o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__SURG_MISU_ID || o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                            && !sereServIdHasPackages.Contains(o.ID) && (o.PARENT_ID == null || (o.PARENT_ID != null && o.IS_OUT_PARENT_FEE != null)))
                    ).ToList();

                //

                foreach (var item in sereServPTTTADOs)
                {
                    //neu la dich vu ky thuat cao thi cong cac vttt trong goi vao dich vu chinh
                    //servicePatyPrpo != null && item.PRICE_POLICY_ID == servicePatyPrpo.PRICE_POLICY_ID
                    if (item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                    {
                        List<SereServADO> serviceOutPackages = sereServADOs.Select(o => new SereServADO(o))
                            .Where(o =>
                                o.PARENT_ID == item.ID
                                && o.IS_OUT_PARENT_FEE == 1
                                && o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID)
                            .ToList();

                        if (serviceOutPackages != null && serviceOutPackages.Count > 0)
                        {
                            //Chuyen dich vu trong goi ngoai chi phi vao cung nhom voi dich vu cha
                            foreach (var serviceOutPackage in serviceOutPackages)
                            {
                                serviceOutPackage.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                                serviceOutPackage.HEIN_SERVICE_TYPE_NAME = item.HEIN_SERVICE_TYPE_NAME;
                                serviceOutPackage.HEIN_SERVICE_TYPE_NUM_ORDER = item.HEIN_SERVICE_TYPE_NUM_ORDER;
                                serviceOutPackage.TDL_REQUEST_DEPARTMENT_ID = item.TDL_EXECUTE_DEPARTMENT_ID;
                                serviceOutPackage.HEIN_SERVICE_TYPE_ID = hightechGroupId;
                            }
                            sereServInPackageOutFeeADOs.AddRange(serviceOutPackages);
                        }
                    }

                    //Nếu PTTT là gói (có package_id)
                    //Đơn giá = giá của PTTT + giá của tất cả dịch vụ, thuốc, VT trong gói (không bị tick HP, không bị tick CPNG) và giá bổ sung (add_price) (lưu ý: giá của PTTT nếu dùng vir_price thì đã bao gồm giá phụ thu "add_price" rồi, nên ko cần cộng thêm add_price nữa)
                    //Đơn giá BHYT = thành tiền bhyt của tất cả dịch vụ, thuốc, VT trong gói (không bị tick HP, không bị tick CPNG)
                    //Đồng thời: không hiển thị bất kì dịch vụ, thuốc, VT nào trong gói ở các mục khác

                    //Nếu PTTT không là gói (không có package_id)
                    //Đơn giá = giá của PTTT
                    //Đơn giá BHYT = Thành tiền BHYT của PTTT
                    //Đồng thời hiển thị dịch vụ, thuốc, VT nào đính kèm PTTT (không bị tick HP) ở các mục khác theo loại dịch vụ BHYT.

                    List<SereServADO> serviceInPackages = sereServADOs.Select(o => new SereServADO(o))
                        .Where(o =>
                            o.PARENT_ID == item.ID
                            && item.PACKAGE_ID.HasValue
                            && o.IS_OUT_PARENT_FEE != 1)
                        //&& o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT)
                        .ToList();

                    if (serviceInPackages != null && serviceInPackages.Count > 0)
                    {
                        decimal totalHeinPrice = 0;
                        decimal? totalToTalHeinPrice = 0;
                        decimal? totalPrice = 0;
                        foreach (var sereServ in serviceInPackages)
                        {
                            totalHeinPrice += (sereServ.AMOUNT * sereServ.PRICE_BHYT);
                            totalToTalHeinPrice += sereServ.VIR_TOTAL_HEIN_PRICE;
                            totalPrice += sereServ.VIR_TOTAL_PRICE;

                            if (item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                            {
                                sereServ.HEIN_SERVICE_TYPE_ID = hightechGroupId;
                            }
                            else
                            {
                                sereServ.HEIN_SERVICE_TYPE_ID = VTGroupId;
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Info("Dich vu pttt: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));

                        Inventec.Common.Logging.LogSystem.Info("Cac dich vu trong goi: " + string.Join(",", serviceInPackages.Select(s => s.ID)));

                        item.VIR_TOTAL_PRICE += totalPrice;
                        item.VIR_PRICE = item.VIR_TOTAL_PRICE / item.AMOUNT;
                        item.TOTAL_PRICE_BHYT += totalHeinPrice;
                        item.PRICE_BHYT = item.TOTAL_PRICE_BHYT / item.AMOUNT;
                        item.VIR_TOTAL_HEIN_PRICE += totalToTalHeinPrice;
                        sereServInPackageTotals.AddRange(serviceInPackages);
                    }

                    item.VIR_TOTAL_PATIENT_PRICE = (item.VIR_TOTAL_PRICE ?? 0) - (item.VIR_TOTAL_HEIN_PRICE ?? 0) - (item.TOTAL_OTHER_SOURCE_PRICE ?? 0);
                    if ((item.VIR_TOTAL_PATIENT_PRICE ?? 0) < 0)
                    {
                        item.VIR_TOTAL_PATIENT_PRICE = 0;
                    }

                    if (item.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID)
                    {
                        item.HEIN_SERVICE_TYPE_ID = hightechGroupId;
                    }
                    else
                    {
                        item.HEIN_SERVICE_TYPE_ID = VTGroupId;
                    }
                }

                sereServLeftADOs = sereServADOs.Select(o => new SereServADO(o))
                    .Where(o =>
                        (sereServInPackageOutFeeADOs != null ? !sereServInPackageOutFeeADOs.Select(p => p.ID)
.Contains(o.ID) : true)
                        && (sereServInPackageTotals != null ? !sereServInPackageTotals.Select(p => p.ID).Contains(o.ID) : true)
                        && (sereServPTTTADOs != null ? !sereServPTTTADOs.Select(p => p.ID).Contains(o.ID) : true)
                        )
                    .ToList();

                Inventec.Common.Logging.LogSystem.Info("dich vu con lai: " + sereServLeftADOs.Count + " |" + string.Join(",", sereServLeftADOs.Select(s => s.ID).ToList()));

                Inventec.Common.Logging.LogSystem.Info("Dich vu trong goi ngoai chi phi: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServInPackageOutFeeADOs), sereServInPackageOutFeeADOs));

                sereServLeftADOs = this.ServiceADODistinct(sereServLeftADOs);

                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SereServADO> ServiceADODistinct(List<SereServADO> servServs)
        {
            List<SereServADO> results = new List<SereServADO>();
            try
            {
                if (servServs != null && servServs.Count > 0)
                {
                    var sereServGroups = servServs.GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRICE_BHYT,
                        o.VIR_PRICE,
                        o.TDL_REQUEST_DEPARTMENT_ID,
                        o.IS_OUT_PARENT_FEE,
                        o.PATIENT_TYPE_ID,
                        o.PARENT_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT
                    }
                    );

                    foreach (var sereServGroup in sereServGroups)
                    {
                        SereServADO sereServ = sereServGroup.FirstOrDefault();
                        sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                        sereServ.VIR_TOTAL_PATIENT_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        sereServ.TOTAL_OTHER_SOURCE_PRICE = sereServGroup.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE ?? 0);
                        results.Add(sereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                results = new List<SereServADO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }

        internal void ProcessGroupSereServ()
        {
            try
            {
                heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                departmentADOs = new List<DepartmentADO>();
                ptttDepartmentADOs = new List<DepartmentADO>();
                vtttDepartmentADOs = new List<DepartmentADO>();

                if (sereServLeftADOs != null)
                {
                    var heinServiceTypeGroups = sereServLeftADOs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                    foreach (var heinServiceTypeGroup in heinServiceTypeGroups)
                    {
                        List<SereServADO> sereServHeinServiceTypes = heinServiceTypeGroup.ToList();
                        HIS_HEIN_SERVICE_TYPE heinServiceType = rdo.HeinServiceTypes.FirstOrDefault(o => o.ID == sereServHeinServiceTypes.First().HEIN_SERVICE_TYPE_ID);
                        HeinServiceTypeADO heinServiceTypeADO = new HeinServiceTypeADO();
                        if (heinServiceType == null)
                            heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = "Khác";
                        else
                        {
                            heinServiceTypeADO.ID = heinServiceType.ID;
                            heinServiceTypeADO.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
                            heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_NAME;
                        }

                        heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE);
                        heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        heinServiceTypeADO.TOTAL_OTHER_SOURCE_PRICE = sereServHeinServiceTypes.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE ?? 0);
                        heinServiceTypeADOs.Add(heinServiceTypeADO);

                        var departmentGroups = sereServHeinServiceTypes.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID)
                            .GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                        foreach (var departmentGroup in departmentGroups)
                        {
                            List<SereServADO> sereServDepartments = departmentGroup.ToList<SereServADO>();
                            HIS_DEPARTMENT department = rdo.Department.FirstOrDefault(o => o.ID == departmentGroup.First().TDL_REQUEST_DEPARTMENT_ID);
                            DepartmentADO departmentADO = new DepartmentADO();
                            if (department != null)
                            {
                                departmentADO.ID = department.ID;
                                departmentADO.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                departmentADO.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }

                            if (heinServiceType != null)
                                departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
                            departmentADO.TOTAL_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_PRICE);
                            departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                            departmentADO.TOTAL_OTHER_SOURCE_PRICE = sereServDepartments.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE ?? 0);
                            departmentADOs.Add(departmentADO);
                        }
                    }
                }

                //VTTT Department
                if (sereServInPackageOutFeeADOs != null && sereServInPackageOutFeeADOs.Count > 0)
                {
                    var serviceVTTTDepartmentGroups = sereServInPackageOutFeeADOs.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID)
                        .GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var serviceVTTTDepartmentGroup in serviceVTTTDepartmentGroups)
                    {
                        List<SereServADO> sereServVTTTDepartments = serviceVTTTDepartmentGroup.ToList<SereServADO>();
                        HIS_DEPARTMENT department = rdo.Department.FirstOrDefault(o => o.ID == serviceVTTTDepartmentGroup.First().TDL_REQUEST_DEPARTMENT_ID);
                        DepartmentADO departmentADO = new DepartmentADO();
                        if (department != null)
                        {
                            departmentADO.ID = department.ID;
                            departmentADO.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                            departmentADO.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                        }
                        departmentADO.HEIN_SERVICE_TYPE_ID = rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__HIGHTECH_ID;
                        departmentADO.TOTAL_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                        departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        departmentADO.TOTAL_OTHER_SOURCE_PRICE = sereServVTTTDepartments.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE ?? 0);
                        vtttDepartmentADOs.Add(departmentADO);
                    }
                }

                //PTTT
                if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
                {
                    var heinServiceTypePTTTGroups = sereServPTTTADOs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                    foreach (var heinServiceTypePTTTGroup in heinServiceTypePTTTGroups)
                    {
                        List<SereServADO> sereServHeinServiceTypes = heinServiceTypePTTTGroup.ToList();
                        //HIS_HEIN_SERVICE_TYPE heinServiceType = rdo.HeinServiceTypes.FirstOrDefault(o => o.ID == sereServHeinServiceTypes.First().HEIN_SERVICE_TYPE_ID);

                        HeinServiceTypeADO heinServiceTypeADO = new HeinServiceTypeADO();
                        //if (heinServiceType == null)
                        //    heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = "Khác";
                        //else
                        //{
                        heinServiceTypeADO.ID = heinServiceTypePTTTGroup.First().HEIN_SERVICE_TYPE_ID;
                        heinServiceTypeADO.HEIN_SERVICE_TYPE_CODE = heinServiceTypePTTTGroup.First().HEIN_SERVICE_TYPE_CODE;
                        heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = heinServiceTypePTTTGroup.First().HEIN_SERVICE_TYPE_NAME;
                        //}

                        List<SereServADO> sereServVTTTADOInHeinServiceTypes = sereServInPackageOutFeeADOs != null ? sereServInPackageOutFeeADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == heinServiceTypeADO.ID).ToList() : null;

                        heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE)
                             + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE) : 0);
                        heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE)
                            + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE) : 0);
                        heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                            + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) : 0);

                        heinServiceTypeADO.TOTAL_OTHER_SOURCE_PRICE = sereServHeinServiceTypes.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE ?? 0)
                            + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE) : 0);
                        heinServiceTypeADOs.Add(heinServiceTypeADO);

                        var ptttDepartmentGroups = sereServHeinServiceTypes.OrderBy(o => o.TDL_EXECUTE_DEPARTMENT_ID)
                        .GroupBy(o => o.TDL_EXECUTE_DEPARTMENT_ID).ToList();
                        foreach (var ptttDepartmentGroup in ptttDepartmentGroups)
                        {
                            List<SereServADO> sereServPTTTDepartments = ptttDepartmentGroup.ToList<SereServADO>();
                            List<long> sereServPTTTDepartmentIds = sereServPTTTDepartments.Select(o => o.ID).ToList();
                            //Dich vu trong goi ngoai chi phi
                            List<SereServADO> sereServInPackageOutFeeDepartmentADO = sereServInPackageOutFeeADOs.Where(o => sereServPTTTDepartmentIds.Contains(o.PARENT_ID.Value)).ToList();

                            HIS_DEPARTMENT department = rdo.Department.FirstOrDefault(o => o.ID == ptttDepartmentGroup.First().TDL_EXECUTE_DEPARTMENT_ID);
                            DepartmentADO departmentADO = new DepartmentADO();
                            if (department != null)
                            {
                                departmentADO.ID = department.ID;
                                departmentADO.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                departmentADO.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }

                            //if (heinServiceType != null)
                            departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceTypePTTTGroup.First().HEIN_SERVICE_TYPE_ID;
                            departmentADO.TOTAL_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_PRICE);
                            departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            departmentADO.TOTAL_OTHER_SOURCE_PRICE = sereServPTTTDepartments.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.TOTAL_OTHER_SOURCE_PRICE);
                            ptttDepartmentADOs.Add(departmentADO);
                        }
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
