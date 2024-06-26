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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SummaryInforTreatmentRecords
{
    public partial class frmSummaryInforTreatmentRecords : HIS.Desktop.Utility.FormBase
    {
        private void LoadDataDefault()
        {
            try
            {
                FillDataTreatment();
                FillDataDepartmentTran();
                FillDataPatientTyleAlter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this.treatmentId;
                currentTreatment = new HIS_TREATMENT();
                currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                if (currentTreatment != null)
                {
                    lblMedi_treatment_code.Text = currentTreatment.TREATMENT_CODE;

                    lbl_open_time_medi.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME);//thoi gian vao vien
                    lbl_close_time_medi.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.OUT_TIME ?? 0);//thoi gian ra vien
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataDepartmentTran()
        {
            try
            {
                CommonParam param = new CommonParam();
                V_HIS_DEPARTMENT_TRAN LastDepartmentTran = new V_HIS_DEPARTMENT_TRAN();
                MOS.Filter.HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = this.treatmentId;
                departmentTranFilter.ORDER_FIELD = "LOG_TIME";
                departmentTranFilter.ORDER_DIRECTION = "DESC";
                LastDepartmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, departmentTranFilter, param).FirstOrDefault();
                if (LastDepartmentTran != null)
                {
                    if (LastDepartmentTran.IN_OUT == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IN_OUT__IN)
                    {
                        lblDepartment.Text = LastDepartmentTran.DEPARTMENT_NAME; //Lấy bản ghi sau cùng và kiểm tra nếu nhận vào khoa thì ok. Ngược lại thì trả
                    }
                    else
                    {
                        if (LastDepartmentTran.NEXT_DEPARTMENT_ID != null)
                        {
                            lblDepartment.Text = "Chờ tiếp nhận: " + LastDepartmentTran.NEXT_DEPARTMENT_NAME;
                        }
                        else
                        {
                            lblDepartment.Text = LastDepartmentTran.DEPARTMENT_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataPatientTyleAlter()
        {
            try
            {
                V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter patientTypeAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                patientTypeAlterFilter.TreatmentId = treatmentId;
                patientTypeAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                PatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                if (PatientTypeAlter != null)
                {
                    lblPatient_type_name.Text = PatientTypeAlter.PATIENT_TYPE_NAME;
                    lbl_LogTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatientTypeAlter.LOG_TIME);
                    var rightRouter = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(PatientTypeAlter.RIGHT_ROUTE_CODE);
                    lblHein_route_name.Text = (rightRouter != null ? rightRouter.HeinRightRouteName : "");

                    lbl_hein_card_number.Text = HIS.Desktop.Utility.HeinCardHelper.TrimHeinCardNumber(PatientTypeAlter.HEIN_CARD_NUMBER);
                    lbl_heincard_address.Text = PatientTypeAlter.ADDRESS;
                    lbl_hein_card_from_date.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0);
                    lbl_hein_card_to_date.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0);
                    lblMediOrgName.Text = (PatientTypeAlter.HEIN_MEDI_ORG_CODE + " - " + PatientTypeAlter.HEIN_MEDI_ORG_NAME);
                    if (PatientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE)
                        lbl_JOIN_5_YEAR.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords_JOIN_5_YEAR_TRUE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    else
                        lbl_JOIN_5_YEAR.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords_JOIN_5_YEAR_FALSE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                    if (PatientTypeAlter.PAID_6_MONTH == MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE)
                        lbl_PAID_6_MONTH.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords_JOIN_5_YEAR_TRUE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    else
                        lbl_PAID_6_MONTH.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords_JOIN_5_YEAR_FALSE", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
