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
using DevExpress.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using HIS.Desktop.Plugins.TransDepartment.Loader;
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
using HIS.UC.Icd;

namespace HIS.Desktop.Plugins.TransDepartment
{
    public partial class frmDepartmentTran : HIS.Desktop.Utility.FormBase
    {

        HIS_PATIENT_TYPE_ALTER thisPatientAlter = new HIS_PATIENT_TYPE_ALTER();

        private void LoadDataFillForm()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                var resultdata = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (resultdata != null)
                {
                    if (!string.IsNullOrEmpty(resultdata.ICD_CODE))
                    {
                        HIS.UC.Icd.ADO.IcdInputADO inputAdo = new HIS.UC.Icd.ADO.IcdInputADO();
                        //inputAdo.ICD_ID = resultdata.ICD_ID;
                        inputAdo.ICD_CODE = resultdata.ICD_CODE;
                        inputAdo.ICD_NAME = resultdata.ICD_NAME;

                        if (icdProcessor != null && ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, inputAdo);
                        }
                    }
                    if (!string.IsNullOrEmpty(resultdata.ICD_SUB_CODE))
                    {
                        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                        ado.ICD_SUB_CODE = resultdata.ICD_SUB_CODE;
                        ado.ICD_TEXT = resultdata.ICD_TEXT;

                        if (subIcdProcessor != null && ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCboChuanDoanDT()
        {
            try
            {
                var listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 424;
                ado.Height = 24;
                ado.IsColor = true;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.AutoCheckIcd") == 1;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControl1.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                   // ((UCIcd)this.ucIcd).ValidationICD(10, 500);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefault()
        {
            //dtLogTime.EditValue = DateTime.Now;
            CommonParam param = new CommonParam();

            HisPatientTypeAlterFilter filter = new HisPatientTypeAlterFilter();
            if (this.departmentTran != null)
            {
                filter.DEPARTMENT_TRAN_ID = this.departmentTran.ID;
            }
            else
            {
                filter.TREATMENT_ID = this.treatmentId;
            }
            filter.ORDER_DIRECTION = "DESC";
            filter.ORDER_FIELD = "CREATE_TIME";

            thisPatientAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>(HIS.Desktop.Plugins.TransDepartment.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET, ApiConsumers.MosConsumer, filter, param).First();
            lbTreatmentType.Text = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.ID == thisPatientAlter.TREATMENT_TYPE_ID).First().TREATMENT_TYPE_NAME;

        }
        private void LoadDepartmentCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.Properties.DataSource = listDepartments;
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
                else
                {
                    var data = listDepartments.Where(o => o.DEPARTMENT_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
                    if (data != null)
                    {
                        if (data != null && data.Count == 1)
                        {
                            cboDepartment.EditValue = data[0].ID;
                            txtDepartmentCode.Text = data[0].DEPARTMENT_CODE;
                            chkAutoLeave.Focus();
                        }
                        else if (data != null && data.Count == 0)
                        {
                            cboDepartment.Properties.DataSource = listDepartments;
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                        else if (data != null && data.Count > 1)
                        {
                            cboDepartment.Properties.DataSource = data;
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDepartment.Properties.DataSource = listDepartments;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
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
