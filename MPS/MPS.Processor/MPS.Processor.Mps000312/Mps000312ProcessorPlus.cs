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
using MPS.Processor.Mps000312.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000312
{
    public partial class Mps000312Processor : AbstractProcessor
    {
        private void DataInputProcess()
        {
            try
            {
                patientADO = PatientRawToADO(rdo.Treatment);
                patyAlterBHYTADO = PatyAlterBHYTRawToADO(rdo.PatyAlter);

                sereServADOs = new List<SereServADO>();
                sereServPTTTADOs = new List<SereServADO>();
                sereServInPackageOutFeeADOs = new List<SereServADO>();

                #region SereServADOs
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          where r.AMOUNT > 0
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlter, rdo.Department, rdo.MaterialTypes, rdo.HisServiceUnit));

                sereServADOs = sereServADOTemps.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_EXPEND != 1 //&& o.VIR_TOTAL_PRICE > 0
                        && o.AMOUNT > 0).OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();

                if (sereServADOs == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("sereServADOs" + sereServADOs.Where(o => o.PATIENT_TYPE_ID == 1).Sum(o => o.TOTAL_PRICE_BHYT));
                    throw new Exception("sereServADOs is null");
                }
                #endregion

                #region Goi dich vu
                //Cac dich vu trong goi ngoai chi phi
                sereServInPackageOutFeeADOs = new List<SereServADO>();

                //Id cua cac dich vu co goi
                List<long> sereServIdHasPackages = sereServADOs.Where(o => o.PARENT_ID.HasValue).Select(o => o.PARENT_ID.Value).Distinct().ToList();

                //Cac dich vu 
                //1. Ton tai dich vu dinh kem
                //2. Dich vu la pttt hoac ky thuat cao va dich vu khong co goi
                //Hoi ruom ra. Can kiem tra lai cho nay
                sereServPTTTADOs = sereServADOs.Select(o => new SereServADO(o)).Where(o => (sereServIdHasPackages != null && sereServIdHasPackages.Contains(o.ID))
                    || ((o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC)
                            && !sereServIdHasPackages.Contains(o.ID) && (o.PARENT_ID == null || (o.PARENT_ID != null && o.IS_OUT_PARENT_FEE != null)))).ToList();

                foreach (var item in sereServPTTTADOs)
                {
                    if (item.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC)
                    {
                        List<SereServADO> serviceOutPackages = sereServADOs.Select(o => new SereServADO(o))
                            .Where(o => o.PARENT_ID == item.ID && o.IS_OUT_PARENT_FEE == 1
                                && o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT)
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
                            }
                            sereServInPackageOutFeeADOs.AddRange(serviceOutPackages);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Dich vu trong goi ngoai chi phi: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServInPackageOutFeeADOs), sereServInPackageOutFeeADOs));
                }

                sereServADOs = sereServADOs.Where(o => (sereServInPackageOutFeeADOs != null ? !sereServInPackageOutFeeADOs.Select(p => p.ID).Contains(o.ID) : true)
                        && (sereServPTTTADOs != null ? !sereServPTTTADOs.Select(p => p.ID).Contains(o.ID) : true)).Select(o => new SereServADO(o)).ToList();

                sereServADOs = this.ServiceADODistinct(sereServADOs);

                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private PatientADO PatientRawToADO(V_HIS_TREATMENT treatment)
        {
            PatientADO patientADO = new PatientADO();
            try
            {
                if (treatment != null)
                {
                    patientADO.VIR_PATIENT_NAME = treatment.TDL_PATIENT_NAME;
                    patientADO.VIR_ADDRESS = treatment.TDL_PATIENT_ADDRESS;
                    patientADO.DOB = treatment.TDL_PATIENT_DOB;
                    patientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
                    patientADO.AGE = AgeUtil.CalculateFullAge(patientADO.DOB);
                    patientADO.GENDER_NAME = treatment.TDL_PATIENT_GENDER_NAME;
                    if (treatment.TDL_PATIENT_DOB > 0)
                    {
                        patientADO.DOB_YEAR = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patientADO = null;
            }
            return patientADO;
        }

        private PatyAlterBhytADO PatyAlterBHYTRawToADO(HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            PatyAlterBhytADO patyAlterBhytADO = null;
            try
            {
                if (patyAlter != null)
                {
                    patyAlterBhytADO = new PatyAlterBhytADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<PatyAlterBhytADO>(patyAlterBhytADO, patyAlter);

                    patyAlterBhytADO.HEIN_CARD_NUMBER_SEPARATE = SetHeinCardNumberDisplayByNumber(patyAlter.HEIN_CARD_NUMBER);
                    patyAlterBhytADO.HEIN_MEDI_ORG_CODE = patyAlter.HEIN_MEDI_ORG_CODE;
                    patyAlterBhytADO.HEIN_MEDI_ORG_NAME = patyAlter.HEIN_MEDI_ORG_NAME;
                    patyAlterBhytADO.IS_HEIN = "X";
                    patyAlterBhytADO.IS_VIENPHI = "";
                    if (!String.IsNullOrEmpty(patyAlter.HEIN_CARD_NUMBER))
                    {
                        patyAlterBhytADO.HEIN_CARD_NUMBER_1 = patyAlter.HEIN_CARD_NUMBER.Substring(0, 2);
                        patyAlterBhytADO.HEIN_CARD_NUMBER_2 = patyAlter.HEIN_CARD_NUMBER.Substring(2, 1);
                        patyAlterBhytADO.HEIN_CARD_NUMBER_3 = patyAlter.HEIN_CARD_NUMBER.Substring(3, 2);
                        patyAlterBhytADO.HEIN_CARD_NUMBER_4 = patyAlter.HEIN_CARD_NUMBER.Substring(5, 2);
                        patyAlterBhytADO.HEIN_CARD_NUMBER_5 = patyAlter.HEIN_CARD_NUMBER.Substring(7, 3);
                        patyAlterBhytADO.HEIN_CARD_NUMBER_6 = patyAlter.HEIN_CARD_NUMBER.Substring(10, 5);
                    }

                    if (patyAlter.HEIN_CARD_FROM_TIME.HasValue)
                    {
                        patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_FROM_TIME.Value));
                    }

                    if (patyAlter.HEIN_CARD_TO_TIME.HasValue)
                    {
                        patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_TO_TIME.Value));
                    }
                }
                else
                {
                    patyAlterBhytADO.HEIN_CARD_NUMBER_SEPARATE = "";
                    patyAlterBhytADO.HEIN_MEDI_ORG_CODE = "";
                    patyAlterBhytADO.HEIN_MEDI_ORG_NAME = "";
                    patyAlterBhytADO.IS_HEIN = "";
                    patyAlterBhytADO.IS_VIENPHI = "X";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_1 = "";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_2 = "";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_3 = "";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_4 = "";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_5 = "";
                    patyAlterBhytADO.HEIN_CARD_NUMBER_6 = "";
                    patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = "";
                    patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patyAlterBhytADO = null;
            }
            return patyAlterBhytADO;
        }

        private string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
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
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServ.VIR_TOTAL_PATIENT_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
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

        private void ProcessGroupSereServ()
        {
            try
            {
                heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                departmentADOs = new List<DepartmentADO>();
                ptttDepartmentADOs = new List<DepartmentADO>();
                vtttDepartmentADOs = new List<DepartmentADO>();

                if (sereServADOs != null)
                {
                    var heinServiceTypeGroups = sereServADOs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
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
                            heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = heinServiceTypeGroup.First().HEIN_SERVICE_TYPE_NAME;
                        }

                        heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE);
                        heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
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

                            if (heinServiceType != null) departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;

                            departmentADO.TOTAL_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_PRICE);
                            departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
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

                        departmentADO.HEIN_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                        departmentADO.TOTAL_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_PRICE);
                        departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServVTTTDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
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

                        List<SereServADO> sereServVTTTADOInHeinServiceTypes = sereServInPackageOutFeeADOs != null ? sereServInPackageOutFeeADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == heinServiceTypeADO.ID).ToList() : null;

                        heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE)
                             + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_PRICE) : 0);
                        heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE)
                            + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_HEIN_PRICE) : 0);
                        heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                            + (sereServVTTTADOInHeinServiceTypes != null ? sereServVTTTADOInHeinServiceTypes.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) : 0);
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

                            if (heinServiceType != null) departmentADO.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;

                            departmentADO.TOTAL_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_PRICE);
                            departmentADO.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            departmentADO.TOTAL_HEIN_PRICE_DEPARTMENT = sereServPTTTDepartments.Sum(o => o.VIR_TOTAL_HEIN_PRICE) + sereServInPackageOutFeeDepartmentADO.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            ptttDepartmentADOs.Add(departmentADO);
                        }
                    }
                }

                // cả sereServADOs và sereServPTTTADOs đều có dv pttt sẽ bỏ 1 cái
                if (heinServiceTypeADOs != null && heinServiceTypeADOs.Count > 0)
                {
                    heinServiceTypeADOs = heinServiceTypeADOs.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
