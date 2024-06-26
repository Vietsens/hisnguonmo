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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000338.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000338.PDO.Config;
using Newtonsoft.Json;
using MPS.Processor.Mps000338.ADO;
using HIS.Desktop.LocalStorage.BackendData;
namespace MPS.Processor.Mps000338
{
    public partial class Mps000338Processor : AbstractProcessor
    {
        List<ServiceTypeADO> GroupPatientTypes = new List<ServiceTypeADO>();
        List<ServiceTypeADO> GroupIsExpends = new List<ServiceTypeADO>();
        List<V_HIS_SERE_SERV> ListSereServIsExpend = new List<V_HIS_SERE_SERV>();
        List<V_HIS_SERE_SERV> ListSereServPatientType = new List<V_HIS_SERE_SERV>();
        List<PatientTypeADO> PatientTypes = new List<PatientTypeADO>();
        decimal totalPricePatientType = 0, totalPriceIsExpend = 0, totalPrice = 0;

        private void DataInputProcess()
        {
            try
            {
                if (this.rdo.ListSereServ == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("MPS.Processor.Mps000338 DataInputProcess this.rdo.ListSereServ is NULL or empty");
                    return;
                }

                //Nhóm theo hao phí
                GroupIsExpends = new List<ServiceTypeADO>();
                ListSereServIsExpend = this.rdo.ListSereServ.Where(o => o.IS_EXPEND.HasValue && o.IS_EXPEND.Value == 1).ToList();
                var GroupSereServIsExpend = ListSereServIsExpend.OrderBy(p => p.SERVICE_TYPE_NAME)
                                    .GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                foreach (var item in GroupSereServIsExpend)
                {
                    ServiceTypeADO ado = new ServiceTypeADO();
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, ServiceTypeADO>();
                    ado = AutoMapper.Mapper.Map<ServiceTypeADO>(item.FirstOrDefault());
                    ado.TOTAL_AMOUNT = item.Sum(o => o.AMOUNT * o.PRICE);
                    GroupIsExpends.Add(ado);
                }
                totalPriceIsExpend = ListSereServIsExpend.Sum(o => o.AMOUNT * o.PRICE);

                // Nhóm theo đối tượng
                GroupPatientTypes = new List<ServiceTypeADO>();
                ListSereServPatientType = this.rdo.ListSereServ.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList();

                totalPricePatientType = ListSereServPatientType.Sum(o => o.AMOUNT * o.PRICE);

                var groupPatientType = ListSereServPatientType.OrderBy(p => p.PATIENT_TYPE_NAME)
                                    .GroupBy(o => o.PATIENT_TYPE_ID).ToList();

                foreach (var item in groupPatientType)
                {
                    PatientTypeADO ado = new PatientTypeADO();
                    ado.ID = item.FirstOrDefault().PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_NAME = item.FirstOrDefault().PATIENT_TYPE_NAME;
                    ado.PATIENT_TYPE_CODE = item.FirstOrDefault().PATIENT_TYPE_CODE;
                    this.PatientTypes.Add(ado);
                }

                var GroupSereServPatientType = ListSereServPatientType.OrderBy(p => p.SERVICE_TYPE_NAME)
                                   .GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                foreach (var item in GroupSereServPatientType)
                {
                    ServiceTypeADO ado = new ServiceTypeADO();
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, ServiceTypeADO>();
                    ado = AutoMapper.Mapper.Map<ServiceTypeADO>(item.FirstOrDefault());
                    ado.TOTAL_AMOUNT = item.Sum(o => o.AMOUNT * o.PRICE);
                    GroupPatientTypes.Add(ado);
                }

                this.totalPrice = this.totalPriceIsExpend + this.totalPricePatientType;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

       
    }
}
