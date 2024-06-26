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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskContract.FormEmployeeAccess
{
    public partial class frmEmployeeAccess : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _moduleData;
        V_HIS_KSK_CONTRACT _kskContract;
        List<ADO.EmployeeADO> ListEmployee;
        List<ADO.EmployeeADO> _currentDataListEmployee;
        #endregion

        #region Construct
        public frmEmployeeAccess()
        {
            InitializeComponent();
        }

        public frmEmployeeAccess(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_KSK_CONTRACT kskContract)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._moduleData = moduleData;
                this._kskContract = kskContract;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmEmployeeAccess_Load(object sender, EventArgs e)
        {
            try
            {
                InitDataToCombo();

                LoadDataGridEmployee();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataGridEmployee()
        {
            try
            {
                if (this.ListEmployee == null || this.ListEmployee.Count == 0)
                {
                    LoadDataEmployees();
                }
                //
                //List<ADO.EmployeeADO> listEmployee_InSelectedDepartment = cboDepartment.EditValue == null ? this.ListEmployee : this.ListEmployee.Where(o =>o.DEPARTMENT_ID == Convert.ToInt64(cboDepartment.EditValue)).ToList();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listEmployee_InSelectedDepartment", listEmployee_InSelectedDepartment));
                CommonParam param = new CommonParam();
                HisKskAccessFilter filter = new HisKskAccessFilter();
                filter.KSK_CONTRACT_ID = this._kskContract.ID;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisKskAccessFilter", filter));
                var apiResult = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_KSK_ACCESS>>("api/HisKskAccess/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult", apiResult));
                List<long> listValidEmployeeID = new List<long>();
                if (apiResult != null && apiResult.Count > 0)
                {
                    listValidEmployeeID = apiResult.Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                        && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE).Select(o => o.EMPLOYEE_ID).ToList() ?? new List<long>();
                }
                List<ADO.EmployeeADO> listDataChecked = new List<ADO.EmployeeADO>();
                List<ADO.EmployeeADO> listDataUnChecked = new List<ADO.EmployeeADO>();
                foreach (var item in this.ListEmployee)
                {
                    if (listValidEmployeeID.Contains(item.ID))
                    {
                        item.isChecked = true;
                        listDataChecked.Add(item);
                    }
                    else
                    {
                        listDataUnChecked.Add(item);
                    }
                }
                this._currentDataListEmployee = new List<ADO.EmployeeADO>();
                this._currentDataListEmployee.AddRange(listDataChecked);
                this._currentDataListEmployee.AddRange(listDataUnChecked);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this._currentDataListEmployee", this._currentDataListEmployee));
                LoadDataGridEmployee(this._currentDataListEmployee);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataGridEmployee(List<ADO.EmployeeADO> listData)
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                var query = listData.AsQueryable();
                query = query.Where(o => (o.LOGINNAME != null && Inventec.Common.String.Convert.UnSignVNese(o.LOGINNAME.ToLower()).Contains(keyword))
                        || (o.USERNAME != null && Inventec.Common.String.Convert.UnSignVNese(o.USERNAME.ToLower()).Contains(keyword))
                        || (o.DOB_ForDisplay != null && Inventec.Common.String.Convert.UnSignVNese(o.DOB_ForDisplay.ToLower()).Contains(keyword))
                        || (o.DEPARTMENT_NAME != null && Inventec.Common.String.Convert.UnSignVNese(o.DEPARTMENT_NAME.ToLower()).Contains(keyword)));
                if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDepartment.EditValue ?? "0").ToString()) > 0)
                {
                    query = query.Where(o => o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDepartment.EditValue ?? "0").ToString()));
                }
                var datas = query.ToList();

                gridViewEmployee.BeginUpdate();
                gridControlEmployee.DataSource = datas;
                for (int i = 0; i < gridViewEmployee.RowCount; i++)
                {
                    ADO.EmployeeADO row = gridViewEmployee.GetRow(i) as ADO.EmployeeADO;
                    if (row != null && row.isChecked)
                    {
                        gridViewEmployee.SelectRow(i);
                    }
                } 
                gridViewEmployee.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataEmployees()
        {
            try
            {
                this.ListEmployee = new List<ADO.EmployeeADO>();
                CommonParam param = new CommonParam();
                HisEmployeeViewFilter filter = new HisEmployeeViewFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "LOGINNAME";
                var apiResult = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    foreach (var item in apiResult)
                    {
                        ADO.EmployeeADO employee = new ADO.EmployeeADO(item);
                        ListEmployee.Add(employee);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataToCombo()
        {
            try
            {
                InitComboDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 250);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.SDO.KskAccessSDO sdo = new MOS.SDO.KskAccessSDO();
                sdo.EmployeeIds = GetSelectedEmployeeIds();
                sdo.KskContractId = this._kskContract.ID;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("KskAccessSDO", sdo));
                var apiResult = new BackendAdapter(param)
                    .Post<MOS.SDO.KskAccessResultSDO>("api/HisKskAccess/AssignEmployee", ApiConsumers.MosConsumer, sdo, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult", apiResult));
                WaitingManager.Hide();
                if (apiResult != null)
                {
                    success = true;
                    LoadDataGridEmployee();
                }

                MessageManager.Show(this, param, success);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<long> GetSelectedEmployeeIds()
        {
            List<long> listEmployeeIds = new List<long>();
            try
            {
                //int[] selectRows = gridViewEmployee.GetSelectedRows();
                //if (selectRows != null && selectRows.Count() > 0)
                //{
                //    for (int i = 0; i < selectRows.Count(); i++)
                //    {
                //        ADO.EmployeeADO employee = (ADO.EmployeeADO)gridViewEmployee.GetRow(selectRows[i]);
                //        if (employee != null)
                //        {
                //            listEmployeeIds.Add(employee.ID);
                //        }
                //    }
                //}
                if (this._currentDataListEmployee != null)
                {
                    listEmployeeIds = _currentDataListEmployee.Where(o=>o.isChecked == true).Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return listEmployeeIds;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlEmployee_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridEmployee(this._currentDataListEmployee);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridEmployee(this._currentDataListEmployee);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEmployee_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                int selectedRowHandle = e.ControllerRow;
                var rowData = (ADO.EmployeeADO)gridViewEmployee.GetRow(selectedRowHandle);
                if (rowData != null)
                {
                    rowData.isChecked = gridViewEmployee.IsRowSelected(selectedRowHandle);
                    int index = this._currentDataListEmployee.FindIndex(o => o.ID == rowData.ID);

                    if (index != -1)
                        this._currentDataListEmployee[index] = rowData;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
