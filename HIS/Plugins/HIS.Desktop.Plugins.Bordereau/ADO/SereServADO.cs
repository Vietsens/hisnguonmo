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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Bordereau.ADO
{
    public class SereServADO : HIS_SERE_SERV
    {
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_DEPARTMENT_CODE { get; set; }
        public long? SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PARENT_SERVICE_CODE { get; set; }
        public string SERVICE_REQ_CODE___SERVICE_CODE { get; set; }
        public string INSTRUCTION_TIME___SERVICE_REQ_CODE { get; set; }
        public string SERVICE_REQ_CODE___SERVICE_NAME { get; set; }
        public string SERVICE_REQ_CODE___SERVICE_CODE_NAME { get; set; }
        public string EQUIPMENT_SET_NAME__NUM_ORDER { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
        public long? SERVICE_REQ_STT_ID { get; set; }
        public long? LIS_STT_ID { get; set; }
        public short? IS_ALLOW_EXPEND { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public decimal? AssignSurgPriceEdit { get; set; }
        public decimal? AssignPackagePriceEdit { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string PACKAGE_CODE { get; set; }
        public short? PACKAGE_IS_NOT_FIXED_SERVICE { get; set; }
        public long? oldValuePrimaryPatientType { get; set; }

        public bool isAssignBlood { get; set; }
        public SereServADO()
        { }

        public SereServADO(SereServADO data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SereServADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (this.SERVICE_CONDITION_ID.HasValue)
                {
                    HIS_SERVICE_CONDITION serviceCondition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == (this.SERVICE_CONDITION_ID ?? 0));
                    this.SERVICE_CONDITION_NAME = serviceCondition != null ? serviceCondition.SERVICE_CONDITION_NAME : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_EXP_MEST_BLTY_REQ_2 data, V_HIS_SERVICE service)
        {
            try
            {
                if (data != null)
                {
                    this.TDL_SERVICE_REQ_CODE = data.SERVICE_REQ_CODE;
                    this.TDL_INTRUCTION_TIME = data.INTRUCTION_TIME;
                    if (service != null)
                    {
                        this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        this.SERVICE_CODE = service.SERVICE_CODE;
                        this.TDL_HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                        this.SERVICE_NAME = service.SERVICE_NAME;
                        this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                    }
                    if (this.SERVICE_CONDITION_ID.HasValue)
                    {
                        HIS_SERVICE_CONDITION serviceCondition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == (this.SERVICE_CONDITION_ID ?? 0));
                        this.SERVICE_CONDITION_NAME = serviceCondition != null ? serviceCondition.SERVICE_CONDITION_NAME : null;
                    }
                    this.PATIENT_TYPE_ID = data.PATIENT_TYPE_ID ?? 0;
                    this.PRIMARY_PATIENT_TYPE_ID = null;
                    this.AMOUNT = data.AMOUNT;
                    this.VIR_PRICE = null;
                    this.PACKAGE_PRICE = null;
                    this.VIR_TOTAL_PRICE = null;
                    this.isAssignBlood = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(HIS_SERE_SERV sereServ, List<HIS_SERE_SERV> sereServs, List<HIS_DEPARTMENT> departments, List<V_HIS_SERVICE> services)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SereServADO>();
                foreach (var item in pi)
                {
                    if (item.GetValue(sereServ) != null)
                        item.SetValue(this, (item.GetValue(sereServ)));
                }
                //Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, sereServ);
                if (this.SERVICE_CONDITION_ID.HasValue)
                {
                    HIS_SERVICE_CONDITION serviceCondition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == (this.SERVICE_CONDITION_ID ?? 0));
                    this.SERVICE_CONDITION_NAME = serviceCondition != null ? serviceCondition.SERVICE_CONDITION_NAME : null;
                }

                V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service != null)
                {
                    this.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                    this.SERVICE_CODE = service.SERVICE_CODE;
                    this.SERVICE_NAME = service.SERVICE_NAME;
                    this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                }

                if (this.PARENT_ID.HasValue)
                {
                    HIS_SERE_SERV sereServParent = sereServs.FirstOrDefault(o => o.ID == this.PARENT_ID);
                    V_HIS_SERVICE serviceParent = sereServParent != null ? services.FirstOrDefault(o => o.ID == sereServParent.SERVICE_ID) : null;
                    this.PARENT_SERVICE_CODE = serviceParent != null ? serviceParent.SERVICE_CODE : null;
                }

                HIS_DEPARTMENT department = departments.FirstOrDefault(o => o.ID == this.TDL_REQUEST_DEPARTMENT_ID);
                if (department != null)
                {
                    this.REQUEST_DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                    this.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                }

                HIS_EQUIPMENT_SET equipmentSet = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true).FirstOrDefault(o => o.ID == (this.EQUIPMENT_SET_ID ?? 0));
                if (equipmentSet != null)
                {
                    string numOrder = this.EQUIPMENT_SET_ORDER.HasValue ? String.Format("({0})", this.EQUIPMENT_SET_ORDER.Value) : "";
                    this.EQUIPMENT_SET_NAME__NUM_ORDER = String.Format("{0} {1}", equipmentSet.EQUIPMENT_SET_NAME, numOrder);
                }

                this.SERVICE_REQ_CODE___SERVICE_CODE = String.Format("{0} - {1}", this.TDL_SERVICE_REQ_CODE, this.SERVICE_CODE);
                this.INSTRUCTION_TIME___SERVICE_REQ_CODE = String.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.TDL_INTRUCTION_TIME), this.TDL_SERVICE_REQ_CODE);
                this.SERVICE_REQ_CODE___SERVICE_NAME = String.Format("({0}) {1}", this.TDL_SERVICE_REQ_CODE, this.SERVICE_NAME);
                this.SERVICE_REQ_CODE___SERVICE_CODE_NAME = String.Format("{0} - {1} - {2}", this.TDL_SERVICE_REQ_CODE, this.SERVICE_CODE, this.SERVICE_NAME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
