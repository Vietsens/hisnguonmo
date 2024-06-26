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
using Inventec.Common.Repository;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReportCountTreatment.ADO
{
    class TreatmentADO : V_HIS_TREATMENT_4
    {
        public string DEPARTMENT_NAME { get; set; }
        public long DEPARTMENT_ID { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> DepartmentTran { get; set; }
        //public string TREATMENT_TYPE_NAME { get; set; }
        //public long TREATMENT_TYPE_ID { get; set; }

        public string END_DEPARTMENT_NAME { get; set; }
        public string END_DEPARTMENT_CODE { get; set; }

        public TreatmentADO() { }

        public TreatmentADO(V_HIS_TREATMENT_4 hisTreatment, List<V_HIS_DEPARTMENT_TRAN> departmentTran)
        {
            try
            {
                PropertyInfo[] p = Properties.Get<V_HIS_TREATMENT_4>();
                foreach (var item in p)
                {
                    item.SetValue(this, item.GetValue(hisTreatment));
                }

                if (departmentTran != null && departmentTran.Count > 0)
                {
                    this.DepartmentTran = departmentTran;
                    V_HIS_DEPARTMENT_TRAN tran = new V_HIS_DEPARTMENT_TRAN();
                    if (this.CLINICAL_IN_TIME.HasValue)
                    {
                        var tranClini = departmentTran.Where(o => o.DEPARTMENT_IN_TIME >= this.CLINICAL_IN_TIME).ToList();
                        //lấy bản tin chuyển khoa cuối cùng đưa vào khoa hiện tại
                        if (tranClini != null && tranClini.Count > 0)
                        {
                            tran = tranClini
                               .OrderByDescending(o => o.DEPARTMENT_IN_TIME)
                               .ThenByDescending(o => o.ID).FirstOrDefault();
                        }
                        else
                        {
                            //lấy bản tin chuyển khoa cuối cùng để đưa bệnh nhân vào
                            tran = departmentTran
                                   .OrderByDescending(o => !o.DEPARTMENT_IN_TIME.HasValue)
                                   .ThenByDescending(o => o.DEPARTMENT_IN_TIME)
                                   .ThenByDescending(o => o.ID).FirstOrDefault();

                            //nếu bản tin chuyển khoa có thời gian nhỏ hơn thời gian vào điều trị thì không thuộc khoa đó do chưa được tiếp nhận
                            //if (tran.DEPARTMENT_IN_TIME.HasValue && tran.DEPARTMENT_IN_TIME.Value < this.CLINICAL_IN_TIME)
                            //{
                            //    tran = new V_HIS_DEPARTMENT_TRAN();
                            //}
                        }
                    }
                    else
                    {
                        tran = departmentTran
                               .OrderByDescending(o => !o.DEPARTMENT_IN_TIME.HasValue)
                               .ThenByDescending(o => o.DEPARTMENT_IN_TIME)
                               .ThenByDescending(o => o.ID).FirstOrDefault();
                    }
                    this.DEPARTMENT_NAME = tran.DEPARTMENT_NAME;
                    this.DEPARTMENT_ID = tran.DEPARTMENT_ID;
                }

                if (this.END_DEPARTMENT_ID.HasValue)
                {
                    var endDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.END_DEPARTMENT_ID.Value);
                    if (endDepartment != null)
                    {
                        this.END_DEPARTMENT_CODE = endDepartment.DEPARTMENT_CODE;
                        this.END_DEPARTMENT_NAME = endDepartment.DEPARTMENT_NAME;
                    }
                }

                //if (patientTypeAlter != null)
                //{
                //    this.TREATMENT_TYPE_NAME = patientTypeAlter.TREATMENT_TYPE_NAME;
                //    this.TREATMENT_TYPE_ID = patientTypeAlter.TREATMENT_TYPE_ID;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
