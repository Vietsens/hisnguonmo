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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MOS.SDO;

namespace MPS.Core.Mps000079
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000079RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string departmentName;
        internal List<SereServKTCADO> sereServKTCADOs { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        internal V_HIS_TREATMENT currentHisTreatment;

        internal List<HIS_EXECUTE_GROUP> executeGroups { get; set; }
        internal List<HIS_HEIN_SERVICE_TYPE> ServiceReports { get; set; }

        internal List<SereServGroupPlusADO> sereServKTCFees { get; set; }
        internal List<SereServGroupPlusADO> sereServExecutes { get; set; }
        internal List<SereServGroupPlusADO> sereServServices { get; set; }

        internal long totalDay { get; set; }

        public Mps000079RDO(
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
            string departmentName,
            List<SereServKTCADO> sereServKTCADOs,
            List<V_HIS_SERE_SERV> sereServs,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT currentHisTreatment,

            List<HIS_HEIN_SERVICE_TYPE> ServiceReports,
            List<HIS_EXECUTE_GROUP> executeGroups,
            long totalDay
            
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.departmentName = departmentName;
                this.sereServKTCADOs = sereServKTCADOs;
                this.sereServs = sereServs;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.executeGroups = executeGroups;
                this.ServiceReports = ServiceReports;
                this.totalDay = totalDay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TOTAL_DAY, totalDay));
                        
                    }

                    //Thời gian vào khoa
                    List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTranTemps = new List<V_HIS_DEPARTMENT_TRAN>();
                    foreach (var departmentTran in departmentTrans)
                    {
                        if (departmentTran != null)
                            departmentTranTemps.Add(departmentTran);
                        
                    }

                    var departmentIns = departmentTranTemps.Where(o => o.IN_OUT == 1).OrderByDescending(o => o.LOG_TIME).ToList();
                    if (departmentIns != null && departmentIns.Count > 0)
                    {
                        var timeDepartmentIn = departmentIns[0].LOG_TIME;
                        keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentIn)));
                    }

                    if (departmentTranTemps.Count == departmentTrans.Count)
                    {
                        var departmentOuts = departmentTrans.Where(o => o.IN_OUT == 2).OrderByDescending(o => o.LOG_TIME).ToList();
                        if (departmentOuts != null && departmentOuts.Count > 0)
                        {
                            var timeDepartmentOut = departmentOuts[0].LOG_TIME;
     
                                keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentOut)));
                        }
                    }
                }

                if (departmentName != null)
                {
                    keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }

               

                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.IS_NOT_HEIN, "X"));
                }

                decimal tongTien = 0;
                decimal tongTienBHYT = 0;
                decimal tongTienBNChiTra = 0;
                foreach (var sereServ in sereServKTCFees)
                {
                    tongTien += sereServ.TOTAL_PRICE_GROUP;
                    tongTienBHYT += sereServ.TOTAL_HEIN_PRICE_SERVICE_GROUP;
                    tongTienBNChiTra += sereServ.TOTAL_PATIENT_PRICE_SERVICE_GROUP;

                }
                keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE, tongTien));
                keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE_HEIN, tongTienBHYT));
                keyValues.Add(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE_PATIENT, tongTienBNChiTra));

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentHisTreatment, keyValues, false);
                


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal void ProcessGroupSereServ()
        {
            try
            {
                sereServKTCADOs = sereServKTCADOs.Where(o=>o.IS_NO_EXECUTE==null).ToList();
                sereServs = sereServs.Where(o=>o.IS_NO_EXECUTE==null).ToList();

                sereServKTCFees = new List<SereServGroupPlusADO>();
                sereServExecutes = new List<SereServGroupPlusADO>();
                sereServServices = new List<SereServGroupPlusADO>();

                //Nhom chi phi trong, ngoai goi theo dich vu KTC
                var sereServKTCGroups = sereServs.GroupBy(o => o.IS_OUT_PARENT_FEE).ToList();
                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    List<V_HIS_SERE_SERV> subSereServKTC = sereServKTCGroup.ToList<V_HIS_SERE_SERV>();

                    SereServGroupPlusADO ktcFeeGroup = new SereServGroupPlusADO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
                    ktcFeeGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServKTC.First());
                    if (ktcFeeGroup.IS_OUT_PARENT_FEE == null)
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ TRONG GÓI PHẪU THUẬT";
                    }
                    else
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ NGOÀI GÓI PHẪU THUẬT";
                    }
                    //Tong tien theo Goi
                    ktcFeeGroup.TOTAL_PRICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;
                    ktcFeeGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    ktcFeeGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    sereServKTCFees.Add(ktcFeeGroup);

                    //Nhom ExecuteGroup
                    var sereServExecuteGroups = subSereServKTC.GroupBy(o => o.EXECUTE_GROUP_ID).ToList();
                    foreach (var sereServExecuteGroup in sereServExecuteGroups)
                    {
                        List<V_HIS_SERE_SERV> subSereServExecutes = sereServExecuteGroup.ToList<V_HIS_SERE_SERV>();
                        SereServGroupPlusADO executeGroup = new SereServGroupPlusADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
                        executeGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServExecutes.First());
                        if (executeGroup.EXECUTE_GROUP_ID == null)
                        {
                            executeGroup.EXECUTE_GROUP_NAME = "Khác";
                        }
                        else
                        {
                            executeGroup.EXECUTE_GROUP_NAME = executeGroups.FirstOrDefault(o => o.ID == executeGroup.EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;
                        }
                        sereServExecutes.Add(executeGroup);

                        //Nhom ServiceGroup
                        var sereServServiceGroups = subSereServExecutes.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var sereServService in sereServServiceGroups)
                        {
                            List<V_HIS_SERE_SERV> subSereServServices = sereServService.ToList<V_HIS_SERE_SERV>();
                            SereServGroupPlusADO serviceGroup = new SereServGroupPlusADO();
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
                            serviceGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServServices.First());
                            if (serviceGroup.HEIN_SERVICE_TYPE_ID == null)
                                serviceGroup.HEIN_SERVICE_TYPE_NAME = "Khác";
                            else
                                serviceGroup.HEIN_SERVICE_TYPE_NAME = "Tiền " + ServiceReports.FirstOrDefault(o => o.ID == serviceGroup.HEIN_SERVICE_TYPE_ID).HEIN_SERVICE_TYPE_NAME;

                            serviceGroup.TOTAL_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;
                            serviceGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                            serviceGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                            sereServServices.Add(serviceGroup);
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
