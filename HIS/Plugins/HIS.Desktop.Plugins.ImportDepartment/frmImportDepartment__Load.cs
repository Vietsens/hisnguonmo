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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImportDepartment.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportDepartment
{
    public partial class frmImportDepartment : HIS.Desktop.Utility.FormBase
    {
        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DepartmentADO> ProcessFormatData(List<DepartmentADO> listDepartment)
        {
            List<DepartmentADO> result = new List<DepartmentADO>();
            try
            {
                if (listDepartment != null && listDepartment.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisDepartmentFilter filter = new HisDepartmentFilter();
                    List<HIS_DEPARTMENT> listDepartmentOld = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param);
                    foreach (var item in listDepartment)
                    {
                        if (this.CheckNotNullData(item))
                        {
                            if (!String.IsNullOrEmpty(item.TheoryPatientCountStr) && this.IsNumber(item.TheoryPatientCountStr))
                                item.THEORY_PATIENT_COUNT = Int64.Parse(item.TheoryPatientCountStr);
                            if (!String.IsNullOrEmpty(item.NumOrderStr) && this.IsNumber(item.NumOrderStr))
                                item.NUM_ORDER = Int64.Parse(item.TheoryPatientCountStr);
                            item.BRANCH_ID = this.GetBranchId(item.BRANCH_CODE);
                            item.IsAutoReceivePatient = item.IsAutoReceivePatientStr == "x" ? true : false;
                            item.IsClinical = item.IsClinicalStr == "x" ? true : false;
                            item.ALLOW_TREATMENT_TYPE_NAMEs = ProcessTreatmentType(item);
                            item.Error = this.CheckError(item, listDepartmentOld, listDepartment);
                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string ProcessTreatmentType(DepartmentADO depa)
        {
            string result = "";
            try
            {
                List<string> typeName = new List<string>();
                List<long> typeId = new List<long>();
                if (!String.IsNullOrWhiteSpace(depa.AllowTreatmentTypeCodes))
                {
                    var typeCode = depa.AllowTreatmentTypeCodes.Split(',');
                    foreach (var item in typeCode)
                    {
                        var treatementType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == item);
                        if (treatementType != null)
                        {
                            typeId.Add(treatementType.ID);
                            typeName.Add(treatementType.TREATMENT_TYPE_NAME);
                        }
                    }
                }

                if (typeId.Count > 0)
                {
                    depa.ALLOW_TREATMENT_TYPE_IDS = string.Join(",", typeId);
                }

                result = string.Join(",", typeName);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckNotNullData(DepartmentADO department)
        {
            bool result = true;
            try
            {
                if (String.IsNullOrEmpty(department.BRANCH_CODE)
                    && String.IsNullOrEmpty(department.DEPARTMENT_CODE)
                    && String.IsNullOrEmpty(department.DEPARTMENT_NAME)
                    && String.IsNullOrEmpty(department.IsClinicalStr)
                    && String.IsNullOrEmpty(department.IsAutoReceivePatientStr)
                    && String.IsNullOrEmpty(department.BHYT_CODE)
                    && String.IsNullOrEmpty(department.NumOrderStr)
                    && String.IsNullOrEmpty(department.TheoryPatientCountStr)
                    && String.IsNullOrEmpty(department.G_CODE))
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string CheckError(DepartmentADO department, List<HIS_DEPARTMENT> listDepartmentOld, List<DepartmentADO> listDepartment)
        {
            string error = "";
            try
            {
                #region BRANCH_ID
                if (department.BRANCH_ID == 0)
                {
                    error += "Không tìm thấy mã chi nhánh | ";
                }
                #endregion

                #region DEPARTMENT_CODE
                if (String.IsNullOrEmpty(department.DEPARTMENT_CODE))
                {
                    error += "Mã khoa trống | ";
                }
                else
                {
                    int len = Encoding.UTF8.GetByteCount(department.DEPARTMENT_CODE);
                    if (len > 20)
                    {
                        error += "Mã khoa vượt quá ký tự | ";
                    }
                    else
                    {
                        HIS_DEPARTMENT departmentOld = listDepartmentOld.FirstOrDefault(o => o.DEPARTMENT_CODE.ToUpper() == department.DEPARTMENT_CODE.Trim().ToUpper());
                        if (departmentOld != null)
                        {
                            error += "Mã khoa đã tồn tại trên hệ thống | ";
                        }

                        List<DepartmentADO> listDepartmentTrung = listDepartment.Where(o => o.DEPARTMENT_CODE.ToUpper() == department.DEPARTMENT_CODE.Trim().ToUpper()).ToList();
                        if (listDepartmentTrung.Count > 1)
                        {
                            error += "Mã khoa trong file Import trùng nhau | ";
                        }
                    }
                }
                #endregion

                #region DEPARTMENT_NAME
                if (String.IsNullOrEmpty(department.DEPARTMENT_NAME))
                {
                    error += "Tên khoa trống | ";
                }
                else
                {
                    int len = Encoding.UTF8.GetByteCount(department.DEPARTMENT_NAME);
                    if (len > 100)
                    {
                        error += "Tên khoa vượt quá ký tự | ";
                    }
                }
                #endregion

                #region IS_CLINICAL
                if (!String.IsNullOrEmpty(department.IsClinicalStr))
                {
                    if (department.IsClinicalStr != "x")
                    {
                        error += "Là khoa lâm sàng sai định dạng | ";
                    }

                    if (String.IsNullOrWhiteSpace(department.AllowTreatmentTypeCodes))
                    {
                        error += "Là khoa lâm sàng nhưng chưa có diện diều trị | ";
                    }
                }
                #endregion

                #region BHYT_CODE
                if (!String.IsNullOrEmpty(department.BHYT_CODE))
                {
                    int len = Encoding.UTF8.GetByteCount(department.BHYT_CODE);
                    if (len > 50)
                    {
                        error += "Mã BHYT vượt quá ký tự | ";
                    }
                }
                #endregion

                #region IS_AUTO_RECEIVE_PATIENT
                if (!String.IsNullOrEmpty(department.IsAutoReceivePatientStr))
                {
                    if (department.IsAutoReceivePatientStr != "x")
                    {
                        error += "Tự động tiếp nhận bệnh nhân sai định dạng | ";
                    }
                }
                #endregion

                #region NUM_ORDER
                if (!String.IsNullOrEmpty(department.NumOrderStr))
                {
                    if (!this.IsNumber(department.NumOrderStr))
                    {
                        error += "Sắp xếp sai định dạng | ";
                    }
                }
                #endregion

                #region THEORY_PATIENT_COUNT
                if (!String.IsNullOrEmpty(department.TheoryPatientCountStr))
                {
                    if (!this.IsNumber(department.TheoryPatientCountStr))
                    {
                        error += "Số giường kế hoạch sai định sạng | ";
                    }
                }
                #endregion

                #region G_CODE
                if (String.IsNullOrEmpty(department.G_CODE))
                {
                    error += "Mã đơn vị trống | ";
                }
                else
                {
                    int len = Encoding.UTF8.GetByteCount(department.G_CODE);
                    if (len > 10)
                    {
                        error += "Mã đơn vị quá ký tự | ";
                    }
                    else
                    {
                        List<SDA_GROUP> listSDAGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA_GROUP>();
                        if (listSDAGroup != null && listSDAGroup.Count > 0)
                        {
                            SDA_GROUP sdaGroup = listSDAGroup.FirstOrDefault(o => o.G_CODE.ToUpper() == department.G_CODE.Trim().ToUpper());
                            if (sdaGroup == null)
                            {
                                error += "Mã đơn vị không tồn tại | ";
                            }
                        }
                        else
                        {
                            error += "Không tìm thấy danh sách đơn vị | ";
                        }
                    }
                }
                #endregion

                #region ALLOW_TREATMENT_TYPE_IDs
                if (!String.IsNullOrWhiteSpace(department.AllowTreatmentTypeCodes))
                {
                    if (string.IsNullOrWhiteSpace(department.ALLOW_TREATMENT_TYPE_NAMEs))
                    {
                        error += "Mã diện điều trị không hợp lệ | ";
                    }
                    else
                    {
                        var countCode = department.AllowTreatmentTypeCodes.Split(',');
                        var countName = department.ALLOW_TREATMENT_TYPE_NAMEs.Split(',');
                        if (countCode.Length != countName.Length)
                        {
                            error += "Tồn tại mã diện điều trị không đúng | ";
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                error = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return error;
        }

        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(pText);
        }

        private long GetBranchId(string branchCode)
        {
            long result = 0;
            try
            {
                List<HIS_BRANCH> branchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>();
                if (!String.IsNullOrEmpty(branchCode))
                {
                    HIS_BRANCH branch = branchs.FirstOrDefault(o => o.BRANCH_CODE.ToUpper() == branchCode.Trim().ToUpper());
                    if (branch != null)
                    {
                        result = branch.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void MakeDataDepartment(ref List<HIS_DEPARTMENT> listDepartment, ref List<HIS_DEPARTMENT> listDepartmentError)
        {
            try
            {
                if (listDepartment == null)
                    listDepartment = new List<HIS_DEPARTMENT>();
                if (listDepartmentError == null)
                    listDepartmentError = new List<HIS_DEPARTMENT>();

                if (listDepartmentADO != null && listDepartmentADO.Count > 0)
                {
                    foreach (var item in listDepartmentADO)
                    {
                        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT>(department, item);
                        department.IS_CLINICAL = (short)(item.IsClinical ? 1 : 0);
                        department.IS_AUTO_RECEIVE_PATIENT = (short)(item.IsAutoReceivePatient ? 1 : 0);
                        if (String.IsNullOrEmpty(item.Error))
                        {
                            listDepartment.Add(department);
                        }
                        else
                        {
                            listDepartmentError.Add(department);
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
