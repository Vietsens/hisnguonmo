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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport;
using EMR.WCF.DCO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.EmpUser.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.SettingSignInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Newtonsoft.Json;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.EmpUser
{ 
    public partial class frmEmpUser : FormBase
    {

        public frmEmpUser(Inventec.Desktop.Common.Modules.Module moduleData, Action<Type> delegateRefresh)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
        } 
        #region global list
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckIsSelectedDisabled;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        MOS.EFMODEL.DataModels.HIS_EMPLOYEE currentDataEmployee = null;
        MOS.EFMODEL.DataModels.HIS_EMPLOYEE resultDataEmployee = null;
        ACS.EFMODEL.DataModels.ACS_USER currentDataUser = null;
        ACS.EFMODEL.DataModels.ACS_USER resultDataUser = null;
        List<HIS_DEPARTMENT> listDepartment;
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        Action<Type> delegateRefresh;
        int rowCount;
        int dataTotal;
        int positionHandle = -1;
        int ActionType = -1;
        int startPage;
        int limit;
        private Dictionary<long, bool> selectedEmployees = new Dictionary<long, bool>();
        private bool isCheckAllEmployee = false;
        #endregion
        List<HIS_MEDI_STOCK> mediStockSeleteds;
        string[] mediStockNew;
        List<HIS_SPECIALITY> specialitySeleteds;
        string[] specialityNew;
        List<HIS_MEDI_ORG> mediOrgSeleteds;
        string[] mediOrgNew;
        List<HIS_DEPARTMENT> departmentSeleteds;
        string[] departmentNew;
        #region function
        private void FillDataToGridControl()
        {
            WaitingManager.Show();
            int numPageSize = 0;
            if (ucPaging.pagingGrid != null)
            {
                numPageSize = ucPaging.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }
            LoadPaging(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging.Init(LoadPaging, param, numPageSize, gridControl2);

            WaitingManager.Hide();
        }

        private void setDataFromLayoutEdit__Employee(HIS_EMPLOYEE updateDTOEmployee)
        {
            updateDTOEmployee.LOGINNAME = txtLoginName.Text.Trim();
            updateDTOEmployee.TDL_USERNAME = txtUserName.Text.Trim();
            updateDTOEmployee.DIPLOMA = txtDiploma.Text.Trim();

            if (txtVCong.Text.Length > 0)// thêm chỗ này
            {
                updateDTOEmployee.VCONG_LOGINNAME = txtVCong.Text.Trim();
            }
            else
            {
                updateDTOEmployee.VCONG_LOGINNAME = null;
            }
            if (!string.IsNullOrEmpty(txtEmployeeCode.Text.Trim()))
            {
                updateDTOEmployee.EMPLOYEE_CODE = txtEmployeeCode.Text.Trim();
            }
            else
            {
                updateDTOEmployee.EMPLOYEE_CODE = null;
            }

            if (txtEmail.Text.Length > 0)
                updateDTOEmployee.TDL_EMAIL = txtEmail.Text.Trim();
            else
                updateDTOEmployee.TDL_EMAIL = null;

            if (txtMobile.Text.Length > 0)
                updateDTOEmployee.TDL_MOBILE = txtMobile.Text.Trim();
            else
                updateDTOEmployee.TDL_MOBILE = null;

            if (dtDOB.EditValue != null && dtDOB.DateTime != DateTime.MinValue)
            {
                updateDTOEmployee.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dtDOB.DateTime.ToString("yyyyMMdd") + "000000");
            }
            else
            {
                updateDTOEmployee.DOB = null;
            }

            if (cbbRank.EditValue != null)
                updateDTOEmployee.MEDICINE_TYPE_RANK = Inventec.Common.TypeConvert.Parse.ToInt64(cbbRank.EditValue.ToString());
            else
                updateDTOEmployee.MEDICINE_TYPE_RANK = null;

            if (this.spinMaxBhytServiceReqPerDay.EditValue != null && this.spinMaxBhytServiceReqPerDay.EditValue.ToString() != "")
                updateDTOEmployee.MAX_BHYT_SERVICE_REQ_PER_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(spinMaxBhytServiceReqPerDay.EditValue.ToString());
            else
                updateDTOEmployee.MAX_BHYT_SERVICE_REQ_PER_DAY = null;
            if (this.spnMaxServiceReqPerDay.EditValue != null && this.spnMaxServiceReqPerDay.EditValue.ToString() != "")
                updateDTOEmployee.MAX_SERVICE_REQ_PER_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(spnMaxServiceReqPerDay.EditValue.ToString());
            else
                updateDTOEmployee.MAX_SERVICE_REQ_PER_DAY = null;

            if (checkAdmin.Checked == true)
            {
                updateDTOEmployee.IS_ADMIN = 1;
            }
            else
            {
                updateDTOEmployee.IS_ADMIN = null;
            }
            if (chkWorkOnly.Checked == true)
            {
                updateDTOEmployee.IS_SERVICE_REQ_EXAM = 1;
            }
            else
            {
                updateDTOEmployee.IS_SERVICE_REQ_EXAM = null;
            }

            if (checkDoctor.Checked == true)
            {
                updateDTOEmployee.IS_DOCTOR = 1;
            }
            else
            {
                updateDTOEmployee.IS_DOCTOR = null;
            }

            if (chkIsNurse.Checked == true)
            {
                updateDTOEmployee.IS_NURSE = 1;
            }
            else
            {
                updateDTOEmployee.IS_NURSE = null;
            }
            updateDTOEmployee.ACCOUNT_NUMBER = txtAccountNumber.Text.Trim();
            updateDTOEmployee.BANK = txtBank.Text.Trim();
            if (cboDepartment.EditValue != null && cboDepartment.EditValue.ToString() != "")
                updateDTOEmployee.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
            else
            {
                updateDTOEmployee.DEPARTMENT_ID = null;
            }
            List<long> mediStockIds = mediStockSeleteds.Select(o => o.ID).ToList();
            updateDTOEmployee.DEFAULT_MEDI_STOCK_IDS = string.Join(",", mediStockIds);

            // kho mac dinh
            //if (cboDefaultMediStockIds.EditValue != null)
            //    updateDTOEmployee.DEFAULT_MEDI_STOCK_IDS = cboDefaultMediStockIds.EditValue.ToString();
            //else
            //    updateDTOEmployee.DEPARTMENT_ID = null;


            if (chkAllowUpdateOtherSclinical.Checked == true)
            {
                updateDTOEmployee.ALLOW_UPDATE_OTHER_SCLINICAL = 1;
            }
            else
            {
                updateDTOEmployee.ALLOW_UPDATE_OTHER_SCLINICAL = null;
            }
            if (chkAllowBlockConcurrentCLS.Checked == true)
            {
                updateDTOEmployee.DO_NOT_ALLOW_SIMULTANEITY = 1;
            }
            else
            {
                updateDTOEmployee.DO_NOT_ALLOW_SIMULTANEITY = null;
            }
            if (txtERXName.Text.Length > 0)
                updateDTOEmployee.ERX_LOGINNAME = txtERXName.Text.Trim();
            else
                updateDTOEmployee.ERX_LOGINNAME = null;
            if (txtERXPassword.Text.Length > 0)
                updateDTOEmployee.ERX_PASSWORD = txtERXPassword.Text.Trim();
            else
                updateDTOEmployee.ERX_PASSWORD = null;
            if (txtTitle.Text.Length > 0)
                updateDTOEmployee.TITLE = txtTitle.Text.Trim();
            else
                updateDTOEmployee.TITLE = null;

            if (!string.IsNullOrEmpty(txtSocialInsuranceNumber.Text))
                updateDTOEmployee.SOCIAL_INSURANCE_NUMBER = txtSocialInsuranceNumber.Text;
            else
                updateDTOEmployee.SOCIAL_INSURANCE_NUMBER = null;
            if (chkIsLimitSchedule.Checked == true)
            {
                updateDTOEmployee.IS_LIMIT_SCHEDULE = 1;
            }
            else
            {
                updateDTOEmployee.IS_LIMIT_SCHEDULE = null;
            }
            if (cboGender.EditValue != null)
                updateDTOEmployee.GENDER_ID = Int16.Parse(cboGender.EditValue.ToString());
            else
                updateDTOEmployee.GENDER_ID = null;
            if (cboEthenic.EditValue != null)
                updateDTOEmployee.ETHNIC_CODE = cboEthenic.EditValue.ToString();
            else
                updateDTOEmployee.ETHNIC_CODE = null;
            if (dteDiploma.EditValue != null && dteDiploma.DateTime != DateTime.MinValue)
                updateDTOEmployee.DIPLOMA_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dteDiploma.DateTime.ToString("yyyyMMdd") + "000000");
            else
                updateDTOEmployee.DIPLOMA_DATE = null;
            if (!string.IsNullOrEmpty(txtDiplomaPlace.Text.Trim()))
                updateDTOEmployee.DIPLOMA_PLACE = txtDiplomaPlace.Text.Trim();
            else
                updateDTOEmployee.DIPLOMA_PLACE = null;
            if (!string.IsNullOrEmpty(txtIdentificationNumber.Text.Trim()))
                updateDTOEmployee.IDENTIFICATION_NUMBER = txtIdentificationNumber.Text.Trim();
            else
                updateDTOEmployee.IDENTIFICATION_NUMBER = null;
            if (cboCareerTitle.EditValue != null)
                updateDTOEmployee.CAREER_TITLE_ID = Int16.Parse(cboCareerTitle.EditValue.ToString());
            else
                updateDTOEmployee.CAREER_TITLE_ID = null;
            if (cboPostion.EditValue != null)
                updateDTOEmployee.POSITION = Int16.Parse(cboPostion.EditValue.ToString());
            else
                updateDTOEmployee.POSITION = null;

            if (specialitySeleteds != null && specialitySeleteds.Count > 0)
                updateDTOEmployee.SPECIALITY_CODES = string.Join(";", specialitySeleteds.Select(o => o.SPECIALITY_CODE).ToList());
            else
                updateDTOEmployee.SPECIALITY_CODES = null;

            if (cboTypeOfTime.EditValue != null)
                updateDTOEmployee.TYPE_OF_TIME = Int16.Parse(cboTypeOfTime.EditValue.ToString());
            else
                updateDTOEmployee.TYPE_OF_TIME = null;

            if (cboBranch.EditValue != null)
                updateDTOEmployee.BRANCH_ID = Int64.Parse(cboBranch.EditValue.ToString());
            else
                updateDTOEmployee.BRANCH_ID = null;

            if (mediOrgSeleteds != null && mediOrgSeleteds.Count > 0)
                updateDTOEmployee.MEDI_ORG_CODES = string.Join(";", mediOrgSeleteds.Select(o => o.MEDI_ORG_CODE).ToList());
            else
                updateDTOEmployee.MEDI_ORG_CODES = null;

            if (departmentSeleteds != null && departmentSeleteds.Count > 0)
                updateDTOEmployee.DEPARTMENT_CODES = string.Join(";", departmentSeleteds.Select(o => o.DEPARTMENT_CODE).ToList());
            else
                updateDTOEmployee.DEPARTMENT_CODES = null;

            if (chkIsNeedSignInstead.Checked == true)
            {
                updateDTOEmployee.IS_NEED_SIGN_INSTEAD = 1;
            }
            else
            {
                updateDTOEmployee.IS_NEED_SIGN_INSTEAD = null;
            }
            // Phạm vi CMBS
            if (!string.IsNullOrWhiteSpace(txtCMBS.Text))
                updateDTOEmployee.PRACTICE_SCOPE_DECISION = txtCMBS.Text.Trim();
            else
                updateDTOEmployee.PRACTICE_SCOPE_DECISION = null;

            // Dịch vụ khác: OTHER_SERVICE_CODES = list SERVICE_CODE, phân cách ';'
            if (cboOtherService.EditValue != null)
            {
                // giả định EditValue là list ID hoặc SERVICE_CODE, bạn chọn kiểu phù hợp.
                // Ở đây dùng DataSource là List<HIS_SERVICE> và multi-select bằng GridCheckMarksSelection,
                // nên ta đọc từ selection, không dùng EditValue.
                var repo = cboOtherService.Properties;
                var gridCheckMarks = repo.Tag as GridCheckMarksSelection;
                if (gridCheckMarks != null)
                {
                    var selectedServices = gridCheckMarks.Selection.Cast<HIS_SERVICE>().ToList();
                    if (selectedServices.Any())
                        updateDTOEmployee.OTHER_SERVICE_CODES = string.Join(";", selectedServices.Select(s => s.SERVICE_CODE));
                    else
                        updateDTOEmployee.OTHER_SERVICE_CODES = null;
                }
                else
                {
                    updateDTOEmployee.OTHER_SERVICE_CODES = null;
                }
            }
            else
            {
                updateDTOEmployee.OTHER_SERVICE_CODES = null;
            }

            // Văn bản phân công
            if (!string.IsNullOrWhiteSpace(txtVBPC.Text))
                updateDTOEmployee.ASSIGNMENT_DOCUMENT = txtVBPC.Text.Trim();
            else
                updateDTOEmployee.ASSIGNMENT_DOCUMENT = null;

            // TG ĐK theo ngày
            if (!string.IsNullOrWhiteSpace(txtTGDKDate.Text))
                updateDTOEmployee.WORKING_SCHEDULE = txtTGDKDate.Text.Trim();
            else
                updateDTOEmployee.WORKING_SCHEDULE = null;

            // TG ĐK theo tuần
            if (!string.IsNullOrWhiteSpace(txtTGDKWeek.Text))
                updateDTOEmployee.WEEK_WORK_DAYS = txtTGDKWeek.Text.Trim();
            else
                updateDTOEmployee.WEEK_WORK_DAYS = null;

            // Cơ sở KCB CGKT: TRANSFER_MEDI_ORG_CODE = MEDI_ORG_CODE (HIS_MEDI_ORG)
            if (cboCGKT.EditValue != null)
            {
                var mediOrgId = Inventec.Common.TypeConvert.Parse.ToInt64(cboCGKT.EditValue.ToString());
                var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.ID == mediOrgId);
                updateDTOEmployee.TRANSFER_MEDI_ORG_CODE = mediOrg != null ? mediOrg.MEDI_ORG_CODE : null;
            }
            else
            {
                updateDTOEmployee.TRANSFER_MEDI_ORG_CODE = null;
            }

            // Quyết định CGKT
            if (!string.IsNullOrWhiteSpace(txtDecisionCGKT.Text))
                updateDTOEmployee.TECH_TRANSFER_DECISIONS = txtDecisionCGKT.Text.Trim();
            else
                updateDTOEmployee.TECH_TRANSFER_DECISIONS = null;

            // TG hiệu lực từ (FROM_TIME) - nếu chọn ngày thì disable 'đến' (xử lý UI ở event riêng)
            if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                updateDTOEmployee.FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
            else
                updateDTOEmployee.FROM_TIME = null;

            // TG hiệu lực đến (TO_TIME)
            if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                updateDTOEmployee.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "000000");
            else
                updateDTOEmployee.TO_TIME = null;
        }

        private void filldatatocboMediStock(HIS_EMPLOYEE data)
        {
            try
            {
                if (data.DEFAULT_MEDI_STOCK_IDS != null)
                {
                    mediStockSeleteds = new List<HIS_MEDI_STOCK>();
                    cboDefaultMediStockIds.Text = "";
                    SetValueMediStock(this.cboDefaultMediStockIds, this.mediStockSeleteds, BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList());
                    string mediStockIds = data.DEFAULT_MEDI_STOCK_IDS;
                    mediStockNew = mediStockIds.Split(',');
                    if (mediStockNew.Count() == 1)
                    {
                        mediStockSeleteds = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ID == (Convert.ToInt32(data.DEFAULT_MEDI_STOCK_IDS))).ToList();
                        cboDefaultMediStockIds.Text = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (Convert.ToInt32(data.DEFAULT_MEDI_STOCK_IDS))).MEDI_STOCK_NAME;
                    }
                    else
                    {
                        string cboDefaultMediStockIdsText = "";
                        for (int i = 0; i < mediStockNew.Count(); i++)
                        {
                            //int m = int.Parse(mediStockNew[i]);
                            string m = (mediStockNew[i]);
                            List<HIS_MEDI_STOCK> ICDLoad = new List<HIS_MEDI_STOCK>();
                            ICDLoad = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ID == Convert.ToInt32(m)).ToList();
                            if (cboDefaultMediStockIdsText.Length > 0)
                                cboDefaultMediStockIdsText = cboDefaultMediStockIdsText + "," + BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (data.ID)).ID;
                            foreach (HIS_MEDI_STOCK a in ICDLoad)
                            {
                                mediStockSeleteds.Add(a);
                            }
                        }

                        cboDefaultMediStockIds.Text = cboDefaultMediStockIdsText;
                    }

                }
                else
                {
                    mediStockSeleteds = new List<HIS_MEDI_STOCK>();
                    cboDefaultMediStockIds.Text = "";
                    SetValueMediStock(this.cboDefaultMediStockIds, this.mediStockSeleteds, BackendDataWorker.Get<HIS_MEDI_STOCK>());

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void filldatatocboSpeciality(HIS_EMPLOYEE data)
        {
            try
            {
                if (data.SPECIALITY_CODES != null)
                {
                    specialitySeleteds = new List<HIS_SPECIALITY>();
                    cboSpecialityCodes.Text = "";
                    SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());
                    string mediStockIds = data.SPECIALITY_CODES;
                    specialityNew = mediStockIds.Split(';');

                    List<string> displayText = new List<string>();
                    for (int i = 0; i < specialityNew.Count(); i++)
                    {
                        string m = (specialityNew[i]);
                        List<HIS_SPECIALITY> ICDLoad = new List<HIS_SPECIALITY>();
                        ICDLoad = BackendDataWorker.Get<HIS_SPECIALITY>().Where(o => o.SPECIALITY_CODE == m).ToList();
                        foreach (HIS_SPECIALITY a in ICDLoad)
                        {
                            displayText.Add(a.SPECIALITY_CODE);
                            specialitySeleteds.Add(a);
                        }
                    }

                    cboSpecialityCodes.Text = string.Join(";", displayText);

                }
                else
                {
                    specialitySeleteds = new List<HIS_SPECIALITY>();
                    cboSpecialityCodes.Text = "";
                    SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void filldatatocboMediOrgCodes(HIS_EMPLOYEE data)
        {
            try
            {
                if (data.MEDI_ORG_CODES != null)
                {
                    mediOrgSeleteds = new List<HIS_MEDI_ORG>();
                    cboMediOrgCodes.Text = "";
                    SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());
                    string mediStockIds = data.MEDI_ORG_CODES;
                    mediOrgNew = mediStockIds.Split(';');

                    List<string> displayText = new List<string>();
                    for (int i = 0; i < mediOrgNew.Count(); i++)
                    {
                        string m = (mediOrgNew[i]);
                        List<HIS_MEDI_ORG> ICDLoad = new List<HIS_MEDI_ORG>();
                        ICDLoad = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.MEDI_ORG_CODE == m).ToList();
                        foreach (HIS_MEDI_ORG a in ICDLoad)
                        {
                            displayText.Add(a.MEDI_ORG_CODE);
                            mediOrgSeleteds.Add(a);
                        }
                    }

                    cboMediOrgCodes.Text = string.Join(";", displayText);

                }
                else
                {
                    mediOrgSeleteds = new List<HIS_MEDI_ORG>();
                    cboMediOrgCodes.Text = "";
                    SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void filldatatocboDepartmentTT12(HIS_EMPLOYEE data)
        {
            try
            {
                if (data.DEPARTMENT_CODES != null)
                {
                    departmentSeleteds = new List<HIS_DEPARTMENT>();
                    cboDepartmentTT12.Text = "";
                    string departmentCodes = data.DEPARTMENT_CODES;
                    var allDepts = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                    string[] parts = departmentCodes.Split(';');

                    List<string> displayText = new List<string>();
                    int i = 0;
                    while (i < parts.Length)
                    {
                        bool found = false;
                        for (int j = parts.Length - 1; j >= i; j--)
                        {
                            string candidate = string.Join(";", parts, i, j - i + 1).Trim();
                            var dept = allDepts.FirstOrDefault(o => o.DEPARTMENT_CODE == candidate);
                            if (dept != null)
                            {
                                displayText.Add(dept.DEPARTMENT_CODE);
                                departmentSeleteds.Add(dept);
                                i = j + 1;
                                found = true;
                                break;
                            }
                        }
                        if (!found) i++;
                    }

                    SetValueDepartmentTT12(this.cboDepartmentTT12, this.departmentSeleteds, allDepts);
                    cboDepartmentTT12.Text = string.Join(";", displayText);

                }
                else
                {
                    departmentSeleteds = new List<HIS_DEPARTMENT>();
                    cboDepartmentTT12.Text = "";
                    SetValueDepartmentTT12(this.cboDepartmentTT12, this.departmentSeleteds, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList());

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;
            positionHandle = -1;
            validMalengthList(15, specialitySeleteds);
            validMalengthList(30, null, mediOrgSeleteds);
            if (!dxValidationProvider1.Validate())
                return;
            //ValidateForm();
            CommonParam param = new CommonParam();
            bool success = false;
            HIS_EMPLOYEE updateDTO_Employee = new HIS_EMPLOYEE();

            WaitingManager.Show();
            try
            {
                if (this.currentDataEmployee != null) Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EMPLOYEE>(updateDTO_Employee, currentDataEmployee);

                setDataFromLayoutEdit__Employee(updateDTO_Employee);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO_Employee), updateDTO_Employee));
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO_Employee.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    resultDataEmployee = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                      (HisRequestUriStore.HIS_EMPLOYEE_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, updateDTO_Employee, param);
                    if (resultDataEmployee != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }
                else
                {
                    if (currentDataUser != null)
                    {
                        resultDataEmployee = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                          (HisRequestUriStore.HIS_EMPLOYEE_UPDATE, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                          updateDTO_Employee, param);
                        if (resultDataEmployee != null)
                        {
                            success = true;
                            //Ghi nhat ky hoat dong
                            List<string> rtDataEmployee = DetailedCompare(currentDataEmployee, resultDataEmployee);
                            if (rtDataEmployee != null && rtDataEmployee.Count() > 0)
                            {
                                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                string message = string.Format("Sửa tài khoản nhân viên. LOGINNAME: {0}. {1}", resultDataEmployee.LOGINNAME, string.Join(",", rtDataEmployee));
                                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                eventlog.Create(login, null, true, message);
                            }
                            //
                            FillDataToGridControl();
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                WaitingManager.Hide();

                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    string[] mess = param.GetMessage().Split('.');
                    List<string> lst = new List<string>(mess);
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (i < lst.Count - 1)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(lst[i] + "     " + lst[i + 1]);
                            if (lst[i].Trim() == lst[i + 1].Trim())
                            {
                                lst.Remove(lst[i + 1]);
                            }
                            Inventec.Common.Logging.LogSystem.Warn(lst.Count() + "   ");
                        }
                    }
                    param.Messages = lst;

                    MessageManager.Show(this, param, success);
                }
            }
            ResetFormData();
        }

        private List<string> DetailedCompare(HIS_EMPLOYEE val1, HIS_EMPLOYEE val2)
        {
            List<string> rs = new List<string>();
            try
            {
                if ((val1.TDL_EMAIL ?? "") != (val2.TDL_EMAIL ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Email", val1.TDL_EMAIL, val2.TDL_EMAIL));
                }
                if ((val1.TDL_USERNAME ?? "") != (val2.TDL_USERNAME ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Họ tên", val1.TDL_USERNAME, val2.TDL_USERNAME));
                }
                if (val1.DOB != val2.DOB)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Ngày sinh", Inventec.Common.DateTime.Convert.TimeNumberToDateString(val1.DOB ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(val2.DOB ?? 0)));
                }
                if ((val1.TDL_MOBILE ?? "") != (val2.TDL_MOBILE ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Điện thoại", val1.TDL_MOBILE, val2.TDL_MOBILE));
                }
                if (val1.MEDICINE_TYPE_RANK != val2.MEDICINE_TYPE_RANK)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Hạng thuốc kê đơn", val1.MEDICINE_TYPE_RANK, val2.MEDICINE_TYPE_RANK));
                }
                if (val1.MAX_BHYT_SERVICE_REQ_PER_DAY != val2.MAX_BHYT_SERVICE_REQ_PER_DAY)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Số lượt BHYT/ngày", val1.MAX_BHYT_SERVICE_REQ_PER_DAY, val2.MAX_BHYT_SERVICE_REQ_PER_DAY));
                }
                if (val1.MAX_SERVICE_REQ_PER_DAY != val2.MAX_SERVICE_REQ_PER_DAY)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "MSố BN/ngày", val1.MAX_SERVICE_REQ_PER_DAY, val2.MAX_SERVICE_REQ_PER_DAY));
                }
                if ((val1.DIPLOMA ?? "") != (val2.DIPLOMA ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Chứng chỉ", val1.DIPLOMA, val2.DIPLOMA));
                }
                if (val1.IS_DOCTOR != val2.IS_DOCTOR)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Là bác sĩ", val1.IS_DOCTOR == 1 ? "Có check" : "Không check", val2.IS_DOCTOR == 1 ? "Có check" : "Không check"));
                }
                if (val1.IS_ADMIN != val2.IS_ADMIN)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Là admin", val1.IS_ADMIN == 1 ? "Có check" : "Không check", val2.IS_ADMIN == 1 ? "Có check" : "Không check"));
                }
                if (val1.IS_NURSE != val2.IS_NURSE)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Là y tá", val1.IS_NURSE == 1 ? "Có check" : "Không check", val2.IS_NURSE == 1 ? "Có check" : "Không check"));
                }
                if ((val1.ACCOUNT_NUMBER ?? "") != (val2.ACCOUNT_NUMBER ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Số tài khoản", val1.ACCOUNT_NUMBER, val2.ACCOUNT_NUMBER));
                }
                if ((val1.BANK ?? "") != (val2.BANK ?? ""))
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Ngân hàng", val1.BANK, val2.BANK));
                }
                if (val1.DEPARTMENT_ID != val2.DEPARTMENT_ID)
                {
                    var department1 = this.listDepartment.Where(o => o.ID == val1.DEPARTMENT_ID).FirstOrDefault();
                    var department2 = this.listDepartment.Where(o => o.ID == val2.DEPARTMENT_ID).FirstOrDefault();
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Khoa", department1 != null ? department1.DEPARTMENT_NAME : "", department2 != null ? department2.DEPARTMENT_NAME : ""));
                }
                if ((val1.DEFAULT_MEDI_STOCK_IDS ?? "") != (val2.DEFAULT_MEDI_STOCK_IDS ?? ""))
                {
                    string defaultMediStockName1 = "";
                    string defaultMediStockName2 = "";
                    if (val1.DEFAULT_MEDI_STOCK_IDS != null)
                    {
                        var defaultMediStock1 = val1.DEFAULT_MEDI_STOCK_IDS.Split(',').ToList();
                        foreach (var item in defaultMediStock1)
                        {
                            var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ID.ToString() == item).FirstOrDefault();
                            if (mediStock != null)
                                defaultMediStockName1 += mediStock.MEDI_STOCK_NAME + ", ";
                        }
                    }
                    if (val2.DEFAULT_MEDI_STOCK_IDS != null)
                    {
                        var defaultMediStock2 = val2.DEFAULT_MEDI_STOCK_IDS.Split(',').ToList();
                        foreach (var item in defaultMediStock2)
                        {
                            var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ID.ToString() == item).FirstOrDefault();
                            if (mediStock != null)
                                defaultMediStockName2 += mediStock.MEDI_STOCK_NAME + ", ";
                        }
                    }
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Kho mặc định", defaultMediStockName1, defaultMediStockName2));

                }
                if (val1.ALLOW_UPDATE_OTHER_SCLINICAL != val2.ALLOW_UPDATE_OTHER_SCLINICAL)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Sửa KQ CLS", val1.ALLOW_UPDATE_OTHER_SCLINICAL, val2.ALLOW_UPDATE_OTHER_SCLINICAL));
                }
                if (val1.DO_NOT_ALLOW_SIMULTANEITY != val2.DO_NOT_ALLOW_SIMULTANEITY)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Chặn thực hiện CLS cùng lúc", val1.DO_NOT_ALLOW_SIMULTANEITY, val2.DO_NOT_ALLOW_SIMULTANEITY));
                }
                if (val1.TITLE != val2.TITLE)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Chức danh", val1.TITLE, val2.TITLE));
                }
                if (val1.ERX_LOGINNAME != val2.ERX_LOGINNAME)
                {
                    rs.Add(string.Format("{0}: {1} ==> {2}", "Tên đăng nhập ERX", val1.ERX_LOGINNAME, val2.ERX_LOGINNAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }
        private void SetOtherServiceForEmployee(string otherServiceCodes)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboOtherService.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark == null)
                    return;

                gridCheckMark.Selection.Clear();

                if (string.IsNullOrWhiteSpace(otherServiceCodes))
                {
                    cboOtherService.Text = string.Empty;
                    return;
                }

                var allServices = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboOtherService.Properties.DataSource = allServices;

                var codes = otherServiceCodes
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                var selected = allServices.Where(s => codes.Contains(s.SERVICE_CODE)).ToList();
                gridCheckMark.Selection.AddRange(selected);

                // Hiển thị SERVICE_CODE ngăn cách ';'
                cboOtherService.Text = string.Join(";", selected.Select(s => s.SERVICE_CODE));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChangedDataRow(HIS_EMPLOYEE currentDataEmp)
        {
            try
            {
                if (this.currentDataEmployee != null)
                {
                    HIS_EMPLOYEE emp = new HIS_EMPLOYEE();
                    //set edit layout
                    txtVCong.Text = currentDataEmp.VCONG_LOGINNAME;
                    txtLoginName.Text = currentDataEmp.LOGINNAME;
                    txtUserName.Text = currentDataEmp.TDL_USERNAME;
                    txtMobile.Text = currentDataEmp.TDL_MOBILE;

                    txtEmployeeCode.Text = currentDataEmp.EMPLOYEE_CODE;

                    if (currentDataEmp.DOB != null)
                    {
                        dtDOB.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentDataEmp.DOB ?? 0) ?? DateTime.MinValue;
                    }
                    else
                    {
                        dtDOB.EditValue = null;
                    }
                    txtEmail.Text = currentDataEmp.TDL_EMAIL;
                    cbbRank.EditValue = currentDataEmp.MEDICINE_TYPE_RANK;
                    this.spinMaxBhytServiceReqPerDay.EditValue = currentDataEmp.MAX_BHYT_SERVICE_REQ_PER_DAY;
                    this.spnMaxServiceReqPerDay.EditValue = currentDataEmp.MAX_SERVICE_REQ_PER_DAY;
                    txtDiploma.Text = currentDataEmp.DIPLOMA;
                    if (currentDataEmp.IS_DOCTOR != null && currentDataEmp.IS_DOCTOR == 1)
                        checkDoctor.Checked = true;
                    else
                        checkDoctor.Checked = false;
                    if (currentDataEmp.IS_SERVICE_REQ_EXAM != null && currentDataEmp.IS_SERVICE_REQ_EXAM == 1)
                    {
                        chkWorkOnly.Checked = true;
                    }
                    else
                        chkWorkOnly.Checked = false;
                    if (currentDataEmp.IS_ADMIN != null && currentDataEmp.IS_ADMIN == 1)
                        checkAdmin.Checked = true;
                    else
                        checkAdmin.Checked = false;
                    if (currentDataEmp.IS_NURSE != null && currentDataEmp.IS_NURSE == 1)
                        chkIsNurse.Checked = true;
                    else
                        chkIsNurse.Checked = false;
                    txtAccountNumber.Text = currentDataEmp.ACCOUNT_NUMBER;
                    txtBank.Text = currentDataEmp.BANK;
                    chkIsLimitSchedule.Checked = currentDataEmp.IS_LIMIT_SCHEDULE == 1 ? true : false;
                    cboDepartment.EditValue = currentDataEmp.DEPARTMENT_ID;
                    txtERXName.Text = currentDataEmp.ERX_LOGINNAME;
                    txtERXPassword.Text = currentDataEmp.ERX_PASSWORD;
                    SetValueMediStock(this.cboDefaultMediStockIds, this.mediStockSeleteds, BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList());
                    filldatatocboMediStock(currentDataEmp);
                    if (currentDataEmp.ALLOW_UPDATE_OTHER_SCLINICAL != null && currentDataEmp.ALLOW_UPDATE_OTHER_SCLINICAL == 1)
                        chkAllowUpdateOtherSclinical.Checked = true;
                    else
                        chkAllowUpdateOtherSclinical.Checked = false;
                    if (currentDataEmp.DO_NOT_ALLOW_SIMULTANEITY != null && currentDataEmp.DO_NOT_ALLOW_SIMULTANEITY == 1)
                    {
                        chkAllowBlockConcurrentCLS.Checked = true;
                    }
                    else
                    {
                        chkAllowBlockConcurrentCLS.Checked = false;
                    }
                    if (currentDataEmp.IS_NEED_SIGN_INSTEAD != null && currentDataEmp.IS_NEED_SIGN_INSTEAD == 1)
                    {
                        chkIsNeedSignInstead.Checked = true;
                    }
                    else
                    {
                        chkIsNeedSignInstead.Checked = false;
                    }

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    txtTitle.Text = currentDataEmp.TITLE;
                    txtSocialInsuranceNumber.Text = currentDataEmp.SOCIAL_INSURANCE_NUMBER;

                    // set Action
                    btnEdit.Enabled = (currentDataEmp.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                      (dxValidationProvider1, dxErrorProvider1);

                    cboGender.EditValue = currentDataEmp.GENDER_ID;
                    cboEthenic.EditValue = currentDataEmp.ETHNIC_CODE;
                    if (currentDataEmp.DIPLOMA_DATE.HasValue)
                        dteDiploma.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentDataEmp.DIPLOMA_DATE ?? 0) ?? DateTime.MinValue;
                    else
                        dteDiploma.EditValue = null;
                    txtDiplomaPlace.Text = currentDataEmp.DIPLOMA_PLACE;
                    txtIdentificationNumber.Text = currentDataEmp.IDENTIFICATION_NUMBER;
                    cboCareerTitle.EditValue = currentDataEmp.CAREER_TITLE_ID;
                    cboPostion.EditValue = currentDataEmp.POSITION;
                    // Phạm vi CMBS
                    txtCMBS.Text = currentDataEmp.PRACTICE_SCOPE_DECISION;

                    // Dịch vụ khác: OTHER_SERVICE_CODES -> chọn trong cboOtherService
                    SetOtherServiceForEmployee(currentDataEmp.OTHER_SERVICE_CODES);

                    // Văn bản phân công
                    txtVBPC.Text = currentDataEmp.ASSIGNMENT_DOCUMENT;

                    // TG ĐK theo ngày
                    txtTGDKDate.Text = currentDataEmp.WORKING_SCHEDULE;

                    // TG ĐK theo tuần
                    txtTGDKWeek.Text = currentDataEmp.WEEK_WORK_DAYS;

                    // Cơ sở KCB CGKT
                    if (!string.IsNullOrEmpty(currentDataEmp.TRANSFER_MEDI_ORG_CODE))
                    {
                        var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>()
                            .FirstOrDefault(o => o.MEDI_ORG_CODE == currentDataEmp.TRANSFER_MEDI_ORG_CODE);
                        cboCGKT.EditValue = mediOrg != null ? (object)mediOrg.ID : null;
                    }
                    else
                    {
                        cboCGKT.EditValue = null;
                    }

                    // Quyết định CGKT
                    txtDecisionCGKT.Text = currentDataEmp.TECH_TRANSFER_DECISIONS;

                    // TG hiệu lực từ
                    if (currentDataEmp.FROM_TIME.HasValue && currentDataEmp.FROM_TIME.Value > 0)
                        dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentDataEmp.FROM_TIME.Value) ?? DateTime.MinValue;
                    else
                        dtTimeFrom.EditValue = null;

                    // TG hiệu lực đến
                    if (currentDataEmp.TO_TIME.HasValue && currentDataEmp.TO_TIME.Value > 0)
                        dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentDataEmp.TO_TIME.Value) ?? DateTime.MinValue;
                    else
                        dtTimeTo.EditValue = null;
                    SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());
                    filldatatocboSpeciality(currentDataEmp);
                    cboTypeOfTime.EditValue = currentDataEmp.TYPE_OF_TIME;
                    cboBranch.EditValue = currentDataEmp.BRANCH_ID;
                    SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());
                    filldatatocboMediOrgCodes(currentDataEmp);
                    SetValueDepartmentTT12(this.cboDepartmentTT12, this.departmentSeleteds, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList());
                    filldatatocboDepartmentTT12(currentDataEmp);

                }
                txtLoginName.Focus();
            }
            catch (Exception e)
            {
                Inventec.Common.Logging.LogSystem.Warn(e);
            }
        }
        private void SelectionGrid__OtherService(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (object item in gridCheckMark.Selection)
                    {
                        HIS_SERVICE s = item as HIS_SERVICE;
                        if (s != null)
                        {
                            if (sb.Length > 0)
                                sb.Append(";");
                            sb.Append(s.SERVICE_CODE);
                        }
                    }
                }
                cboOtherService.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            txtLoginName.Focus();
            txtLoginName.SelectAll();
            txtSearch.Text = "";
            spinMaxBhytServiceReqPerDay.EditValue = null;
            spnMaxServiceReqPerDay.EditValue = null;
            FillDataToGridControl();
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized)
                    return;
                lcEditInfo.BeginUpdate();
                mediStockSeleteds = new List<HIS_MEDI_STOCK>();
                specialitySeleteds = new List<HIS_SPECIALITY>();
                mediOrgSeleteds = new List<HIS_MEDI_ORG>();
                departmentSeleteds = new List<HIS_DEPARTMENT>();
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                    }
                }

                txtEmployeeCode.Text = null;


                txtVCong.Text = "";
                txtLoginName.Text = "";
                txtUserName.Text = "";
                txtEmail.Text = "";
                txtMobile.Text = "";
                txtDiploma.Text = "";
                cbbRank.Reset();
                checkAdmin.Checked = false;
                checkDoctor.Checked = false;
                chkIsNurse.Checked = false;
                txtAccountNumber.Text = "";
                txtBank.Text = "";
                txtERXName.Text = "";
                txtERXPassword.Text = "";
                cboDepartment.Reset();
                cboDefaultMediStockIds.Reset();
                SetValueMediStock(this.cboDefaultMediStockIds, this.mediStockSeleteds, BackendDataWorker.Get<HIS_MEDI_STOCK>());
                cboDefaultMediStockIds.Focus();
                chkAllowUpdateOtherSclinical.Checked = false;
                chkAllowBlockConcurrentCLS.Checked = false;
                spinMaxBhytServiceReqPerDay.Reset();
                spnMaxServiceReqPerDay.Reset();
                positionHandle = -1;
                txtLoginName.Enabled = true;
                txtLoginName.Focus();
                chkWorkOnly.Checked = false;
                txtSocialInsuranceNumber.Text = null;
                chkIsLimitSchedule.Checked = false;
                chkIsNeedSignInstead.Checked = false;
                //set Action
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                cboGender.EditValue = null;
                cboEthenic.EditValue = null;
                dteDiploma.EditValue = null;
                txtDiplomaPlace.Text = null;
                txtIdentificationNumber.Text = null;
                cboCareerTitle.EditValue = null;
                cboPostion.EditValue = null;
                cboSpecialityCodes.Reset();
                SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());
                cboSpecialityCodes.Focus();
                cboTypeOfTime.EditValue = null;
                cboBranch.EditValue = null;
                cboMediOrgCodes.Reset();
                SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());
                cboMediOrgCodes.Focus();
                cboDepartmentTT12.Reset();
                SetValueDepartmentTT12(this.cboDepartmentTT12, this.departmentSeleteds, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList());
                cboDepartmentTT12.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }

        }
        #endregion

        #region validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtLoginName, dxValidationProvider1, "", ValidLoginname);
                ValidationSingleControl(txtUserName, dxValidationProvider1, "", validUsername);
                ValidationEmail(txtEmail);
                ValidationGreatThanZeroControl(this.spinMaxBhytServiceReqPerDay);
                ValidationGreatThanZeroControl(this.spnMaxServiceReqPerDay);
                validMalength(this.txtVCong, 50);
                validMalength(this.txtERXName, 100);
                validMalength(this.txtERXName, 100);
                validMalength(this.txtERXPassword, 400);
                validMalength(this.txtTitle, 100);
                validMalength(this.txtDiploma, 50);
                validMalength(this.txtDiplomaPlace, 50);
                validMalength(this.txtIdentificationNumber, 15);
                ValidationBhxh(this.txtSocialInsuranceNumber);

                validMalength(this.txtEmployeeCode, 20);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool validUsername()
        {
            bool valid = true;
            int? textLength = Inventec.Common.String.CountVi.Count(txtUserName.Text);
            if (txtUserName.Text.Equals(""))
                valid = false;
            if (textLength == null || textLength >= 50)
                valid = false;
            return valid;
        }

        bool ValidLoginname()
        {
            bool valid = true;
            int? tlength = Inventec.Common.String.CountVi.Count(txtLoginName.Text);
            if (txtLoginName.Text.Equals(""))
                valid = false;
            if (tlength == null || tlength >= 50)
                valid = false;
            return valid;
        }

        private void ValidationBhxh(TextEdit txt)
        {
            try
            {
                ValidateBhxh validRule = new ValidateBhxh();
                validRule.txt = txt;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                ApiResultObject<List<HIS_EMPLOYEE>> apiResultEmployee = null;

                apiResultEmployee = new BackendAdapter(paramCommon).GetRO<List<HIS_EMPLOYEE>>
                      (HisRequestUriStore.HIS_EMPLOYEE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                gridViewFormList.GridControl.DataSource = null;
                if (apiResultEmployee != null)
                {
                    var dataEmployee = apiResultEmployee.Data ?? new List<HIS_EMPLOYEE>();

                    // Map sang EmployeeADO và giữ trạng thái IsSelected nếu đã chọn trước đó
                    var adoList = new List<EmployeeADO>();
                    foreach (var emp in dataEmployee)
                    {
                        if (emp == null) continue;

                        var ado = new EmployeeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<EmployeeADO>(ado, emp);

                        bool IsSelected;
                        if (selectedEmployees != null && selectedEmployees.TryGetValue(emp.ID, out IsSelected))
                            ado.IsSelected = IsSelected;
                        else
                            ado.IsSelected = false;

                        adoList.Add(ado);
                    }

                    gridViewFormList.GridControl.DataSource = adoList;

                    rowCount = adoList.Count;
                    dataTotal = (apiResultEmployee.Param == null ? 0 : apiResultEmployee.Param.Count ?? 0);
                }

                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ValidationEmail(TextEdit control)
        {
            if (control.Text != null || control.Text.Equals(""))
            {
                ValidateEmail validRule = new ValidateEmail();
                validRule.txt = control;
                validRule.ErrorText = "E-mail sai định dạng";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
        }

        private void validMalength(TextEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength validate = new ValidateMaxLength();
                validate.txt = control;
                validate.count = maxLength;
                //validate.IsRequired = true;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void validMalengthList(int maxLength, List<HIS_SPECIALITY> lstSpe = null, List<HIS_MEDI_ORG> lstMedi = null)
        {
            try
            {
                ValidateMaxLengthList validate = new ValidateMaxLengthList();
                validate.count = maxLength;
                if (lstSpe != null && lstSpe.Count > 0)
                {
                    validate.listString = lstSpe;
                    dxValidationProvider1.SetValidationRule(cboSpecialityCodes, validate);
                }
                else
                {
                    validate.listString = lstMedi;
                    dxValidationProvider1.SetValidationRule(cboMediOrgCodes, validate);
                }
                //validate.IsRequired = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationGreatThanZeroControl(SpinEdit control)
        {
            ControlGreatThanZeroValidationRule validRule = new ControlGreatThanZeroValidationRule();
            validRule.spin = control;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(control, validRule);
        }
        #endregion

        #region init data,icon,Language
        private void InitComboRank()
        {
            try
            {
                List<RankADO> listRank = new List<RankADO>();
                listRank.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__1));
                listRank.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__2));
                listRank.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__3));
                listRank.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__4));
                listRank.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__5));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("RANK", "", 184, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO("RANK", "ID", columnInfos, false, 184);
                ControlEditorLoader.Load(cbbRank, listRank, controlEditorADO);
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
                //var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                this.listDepartment = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 10, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                HIS.Desktop.Plugins.EmpUser.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmpUser.Resources.Lang", typeof(HIS.Desktop.Plugins.EmpUser.frmEmpUser).Assembly);
                //Inventec.Common.Resource.Get getValue;

                this.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLOCK.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnLOCK.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDEFAULTPASS.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnDEFAULTPASS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnROLEUSER.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnROLEUSER.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLOGINNAME.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnLOGINNAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnUSERNAME.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnUSERNAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEMAIL.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnEMAIL.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMOBILE.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnMOBILE.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMEDICINE_TYPE_RANK.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnMEDICINE_TYPE_RANK.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDIPLOMA.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnDIPLOMA.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIS_DOCTOR.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnIS_DOCTOR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIS_ADMIN.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnIS_ADMIN.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMAX_BHYT_SERVICE_REQ_PER_DAY.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnMAX_BHYT_SERVICE_REQ_PER_DAY.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumnSTATUS.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnSTATUS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCREATE_TIME_STR.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnCREATE_TIME_STR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMODIFY_TIME_STR.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnMODIFY_TIME_STR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMODIFIER.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnMODIFIER.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCREATOR.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnCREATOR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIS_NURSE.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnIS_NURSE.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAccountNumber.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnAccountNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBank.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnBank.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDepartMent.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnDepartMent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDefaultMediStock.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnDefaultMediStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDefaultMediStock.ToolTip = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnDefaultMediStock.ToolTip ", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumnAllowUpdateOtherSclinical.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnAllowUpdateOtherSclinical.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAllowUpdateOtherSclinical.ToolTip = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnAllowUpdateOtherSclinical.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemSearch.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemEdit.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemAdd.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemRedo.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsNurse.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciIsNurse.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBank.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciBank.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartment.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDefaultMediStockIds.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciDefaultMediStockIds.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAllowUpdateOtherSclinical.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciAllowUpdateOtherSclinical.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDefaultMediStockIds.ToolTip = Inventec.Common.Resource.Get.Value("frmEmpUser.cboDefaultMediStockIds.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowUpdateOtherSclinical.ToolTip = Inventec.Common.Resource.Get.Value("frmEmpUser.chkAllowUpdateOtherSclinical.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowBlockConcurrentCLS.ToolTip = Inventec.Common.Resource.Get.Value("frmEmpUser.chkAllowBlockConcurrentCLS.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEmpUser.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDefaultMediStockIds.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEmpUser.cboDefaultMediStockIds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSpinMaxBhytServiceReqPerDay.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.lciSpinMaxBhytServiceReqPerDay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void frmEmpUser_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                repositoryItemCheckIsSelectedDisabled = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
                repositoryItemCheckIsSelectedDisabled.Enabled = false;
                repositoryItemCheckIsSelectedDisabled.ReadOnly = true;
                repositoryItemCheckIsSelectedDisabled.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style1;

                LoadComboEdit();
                InitComboRank();
                InitComboDepartment();
                InitCheck(cboDefaultMediStockIds, SelectionGrid__MEDISTOCK_NAME);
                InitComboMediStock(cboDefaultMediStockIds, BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList(), null, "MEDI_STOCK_NAME", "ID");
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                SetCaptionByLanguageKey();
                ValidateForm();
                SetDefaultValue();
                InitControlState();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        List<HIS_GENDER> lstGender { get; set; }
        List<SDA_ETHNIC> lstEthnic { get; set; }
        List<HIS_CAREER_TITLE> lstCareer { get; set; }
        List<ObjectCombo> lstPosition { get; set; }
        List<ObjectCombo> lstTypeOfTime { get; set; }
        List<HIS_BRANCH> lstBranch { get; set; }
        //139187 V+
        private async Task LoadComboEdit()
        {
            try
            {
                LoadBranch();
                LoadGender();
                LoadEthnic();
                LoadCareerTitle();
                LoadPosition();
                LoadTypeOfTime();
                InitComboMediStock(cboSpecialityCodes, BackendDataWorker.Get<HIS_SPECIALITY>().ToList(), "SPECIALITY_CODE", "SPECIALITY_NAME", "ID");
                InitCheck(cboSpecialityCodes, SelectionGrid__SpecialityCodes);

                InitComboMediStock(cboMediOrgCodes, BackendDataWorker.Get<HIS_MEDI_ORG>().ToList(), "MEDI_ORG_CODE", "MEDI_ORG_NAME", "ID");
                InitCheck(cboMediOrgCodes, SelectionGrid__MediOrgCodes);
                
                InitComboMediStock(cboDepartmentTT12, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList(), "DEPARTMENT_CODE", "DEPARTMENT_NAME", "ID");
                InitCheck(cboDepartmentTT12, SelectionGrid__DepartmentTT12);

                //// Dịch vụ khác
                var services = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1).ToList();
                InitComboOtherService(cboOtherService, services, "SERVICE_CODE", "SERVICE_NAME", "ID", "HEIN_SERVICE_BHYT_CODE", "Mã BHYT");
                cboOtherService.Properties.View.OptionsView.ShowAutoFilterRow = true;
                InitCheck(cboOtherService, SelectionGrid__OtherService);

                // Cơ sở KCB CGKT (single-select)
                var mediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> cgktCols = new List<ColumnInfo>();
                cgktCols.Add(new ColumnInfo("MEDI_ORG_CODE", "Mã", 100, 1));
                cgktCols.Add(new ColumnInfo("MEDI_ORG_NAME", "Tên", 250, 2));
                ControlEditorADO cgktEditor = new ControlEditorADO("MEDI_ORG_NAME", "ID", cgktCols, false, 350);
                ControlEditorLoader.Load(cboCGKT, mediOrgs, cgktEditor);
                cboCGKT.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SelectionGrid__MediOrgCodes(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_MEDI_ORG> sgSelectedNews = new List<HIS_MEDI_ORG>();
                    foreach (HIS_MEDI_ORG rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.MEDI_ORG_CODE.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.mediOrgSeleteds = new List<HIS_MEDI_ORG>();
                    this.mediOrgSeleteds.AddRange(sgSelectedNews);

                }
                this.cboMediOrgCodes.Text = sb.ToString();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadTypeOfTime()
        {
            try
            {
                lstTypeOfTime = new List<ObjectCombo>() {
                    new ObjectCombo() { id = 1, name = "Toàn thời gian" },
                    new ObjectCombo() { id = 2, name = "Bán thời gian" }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("id", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("name", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("name", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTypeOfTime, lstTypeOfTime, controlEditorADO);
                cboTypeOfTime.Properties.ImmediatePopup = true;
                cboTypeOfTime.Properties.PopupFormSize = new Size(350, cboTypeOfTime.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task LoadBranch()
        {
            try
            {
                lstBranch = BackendDataWorker.Get<HIS_BRANCH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBranch, lstBranch, controlEditorADO);
                cboBranch.Properties.ImmediatePopup = true;
                cboBranch.Properties.PopupFormSize = new Size(350, cboBranch.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SelectionGrid__SpecialityCodes(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_SPECIALITY> sgSelectedNews = new List<HIS_SPECIALITY>();
                    foreach (HIS_SPECIALITY rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.SPECIALITY_CODE.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.specialitySeleteds = new List<HIS_SPECIALITY>();
                    this.specialitySeleteds.AddRange(sgSelectedNews);

                }
                this.cboSpecialityCodes.Text = sb.ToString();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadPosition()
        {
            try
            {
                lstPosition = new List<ObjectCombo>() {
                    new ObjectCombo() { id = 1, name = "Người chịu trách nhiệm chuyên môn" },
                    new ObjectCombo() { id = 2, name = "Trưởng khoa" },
                    new ObjectCombo() { id = 3, name = "Người chịu trách nhiệm chuyên môn kiêm Trưởng khoa" },
                    new ObjectCombo() { id = 4, name = "Người đứng đầu cơ sở khám bệnh, chữa bệnh hoặc người được người đứng đầu cơ sở khám bệnh, chữa bệnh ủy quyền ký giấy chuyển tuyến khám bệnh, chữa bệnh, giấy ra viện, giấy hẹn khám lại" },
                    new ObjectCombo() { id = 5, name = "Người hành nghề được giao phụ trách khoa trong trường hợp không có trưởng khoa" },
                    new ObjectCombo() { id = 6, name = "Người được ủy quyền chịu trách nhiệm chuyên môn kỹ thuật theo khoản 11 Điều 27 Nghị định số 96/2023/NĐCP" }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("id", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("name", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("name", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPostion, lstPosition, controlEditorADO);
                cboPostion.Properties.ImmediatePopup = true;
                cboPostion.Properties.PopupFormSize = new Size(350, cboPostion.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadCareerTitle()
        {
            try
            {
                lstCareer = BackendDataWorker.Get<HIS_CAREER_TITLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCareer), lstCareer));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_TITLE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("CAREER_TITLE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_TITLE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCareerTitle, lstCareer, controlEditorADO);
                cboCareerTitle.Properties.ImmediatePopup = true;
                cboCareerTitle.Properties.PopupFormSize = new Size(350, cboCareerTitle.Properties.PopupFormSize.Height);
                if (cboCareerTitle.EditValue != null && (lstCareer == null || lstCareer.FirstOrDefault(o => o.ID == Int64.Parse(cboCareerTitle.EditValue.ToString())) == null))
                {
                    cboCareerTitle.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task LoadGender()
        {
            try
            {
                lstGender = BackendDataWorker.Get<HIS_GENDER>().ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboGender, lstGender, controlEditorADO);
                cboGender.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private async Task LoadEthnic()
        {
            try
            {
                lstEthnic = BackendDataWorker.Get<SDA_ETHNIC>().ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ETHNIC_CODE", columnInfos, true, 200);
                ControlEditorLoader.Load(cboEthenic, lstEthnic, controlEditorADO);
                cboEthenic.Properties.ImmediatePopup = true;
                cboEthenic.Properties.PopupFormSize = new Size(200, cboEthenic.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDefaultMediStockIds_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(" ; ");
                    }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboMediStock(GridLookUpEdit cbo, object data, string DisplayCode, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                if (!string.IsNullOrEmpty(DisplayCode))
                {
                    DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(DisplayCode);
                    col1.VisibleIndex = 1;
                    col1.Width = 100;
                    col1.Caption = "Mã";
                }

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = !string.IsNullOrEmpty(DisplayCode) ? 2 : 1;
                col2.Width = 450;
                col2.Caption = "Tên";

                cbo.Properties.PopupFormWidth = 550;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                    ////
                }

                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void InitComboOtherService(GridLookUpEdit cbo, object data, string DisplayCode, string DisplayValue, string ValueMember, string additionalColumnField, string additionalColumnCaption)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                int visibleIndex = 1;

                if (!string.IsNullOrEmpty(DisplayCode))
                {
                    DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(DisplayCode);
                    col1.VisibleIndex = visibleIndex++;
                    col1.Width = 100;
                    col1.Caption = "Mã";
                    col1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                }

                // Thêm cột bổ sung
                if (!string.IsNullOrEmpty(additionalColumnField))
                {
                    DevExpress.XtraGrid.Columns.GridColumn colAdditional = cbo.Properties.View.Columns.AddField(additionalColumnField);
                    colAdditional.VisibleIndex = visibleIndex++;
                    colAdditional.Width = 150;
                    colAdditional.Caption = additionalColumnCaption;
                    colAdditional.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                }

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = visibleIndex;
                col2.Width = 350;
                col2.Caption = "Tên";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;

                cbo.Properties.PopupFormWidth = 700;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }

                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__MEDISTOCK_NAME(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_MEDI_STOCK> sgSelectedNews = new List<HIS_MEDI_STOCK>();
                    foreach (HIS_MEDI_STOCK rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.MEDI_STOCK_NAME.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.mediStockSeleteds = new List<HIS_MEDI_STOCK>();
                    this.mediStockSeleteds.AddRange(sgSelectedNews);

                }
                this.cboDefaultMediStockIds.Text = sb.ToString();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SelectionGrid__DepartmentTT12(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> sgSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.DEPARTMENT_CODE.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.departmentSeleteds = new List<HIS_DEPARTMENT>();
                    this.departmentSeleteds.AddRange(sgSelectedNews);
                }
                this.cboDepartmentTT12.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetValueMediStock(GridLookUpEdit grdLookUpEdit, List<HIS_MEDI_STOCK> listSelect, List<HIS_MEDI_STOCK> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    //EmrBusinessFilter filter = new EmrBusinessFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;


                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.MEDI_STOCK_CODE == p.MEDI_STOCK_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();

                    gridCheckMark.Selection.AddRange(selectFilter);


                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueSpeciality(GridLookUpEdit grdLookUpEdit, List<HIS_SPECIALITY> listSelect, List<HIS_SPECIALITY> listAll)
        {
            try
            {
                if (listSelect != null)
                {

                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.SPECIALITY_CODE == p.SPECIALITY_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();

                    gridCheckMark.Selection.AddRange(selectFilter);


                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueMediOrgCodes(GridLookUpEdit grdLookUpEdit, List<HIS_MEDI_ORG> listSelect, List<HIS_MEDI_ORG> listAll)
        {
            try
            {
                if (listSelect != null)
                {

                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.MEDI_ORG_CODE == p.MEDI_ORG_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();

                    gridCheckMark.Selection.AddRange(selectFilter);


                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueDepartmentTT12(GridLookUpEdit grdLookUpEdit, List<HIS_DEPARTMENT> listSelect, List<HIS_DEPARTMENT> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.DEPARTMENT_CODE == p.DEPARTMENT_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();

                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //this.ActionType = GlobalVariables.ActionAdd;
                this.currentDataEmployee = new HIS_EMPLOYEE();
                positionHandle = -1;
                //FillDataToGridControl();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUserName.Focus();
                    txtUserName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDOB.Focus();
                    dtDOB.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMobile.Focus();
                    txtMobile.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMobile_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiploma.Focus();
                    txtDiploma.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiploma_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDoctor.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkDoctor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNurse.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkAdmin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cbbRank.Focus();
                    cbbRank.ShowPopup();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbRank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMaxBhytServiceReqPerDay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMaxBhytServiceReqPerDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountNumber.Focus();
                    txtAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnRefresh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    HIS_EMPLOYEE data = (HIS_EMPLOYEE)gridViewFormList.GetRow(e.RowHandle);

                    // Cột LOCK
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? btnUnlock : btnLock;

                    // Cột IsSelected: checkbox mờ nếu dòng bị khóa
                    if (e.Column.FieldName == "IsSelected")
                    {
                        if (data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItemCheckIsSelectedDisabled; // Checkbox mờ
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckIsSelected; // Checkbox bình thường
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = gridViewFormList.GetRow(e.ListSourceRowIndex) as EmployeeADO;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.Column.FieldName == "IsSelected" && e.IsGetData)
                {
                    if (data == null)
                    {
                        e.Value = false;
                    }
                    else if (data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        e.Value = false;
                    }
                    else
                    {
                        e.Value = data.IsSelected;
                    }
                    return;
                }
                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Hoạt động" : "Tạm khóa";

                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }

                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
                if (e.Column.FieldName == "DOB_STR")
                {
                    string DobStr = (view.GetRowCellValue(e.ListSourceRowIndex, "DOB") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(DobStr));
                }
                if (e.Column.FieldName == "DEPARTMENT_NAME")
                {
                    if (data.DEPARTMENT_ID != null)
                    {
                        e.Value = this.listDepartment.Where(o => o.ID == data.DEPARTMENT_ID).FirstOrDefault().DEPARTMENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectAllEmployees(bool IsSelected)
        {
            try
            {
                var data = gridControl2.DataSource as List<EmployeeADO>;
                if (data == null || data.Count == 0)
                    return;

                foreach (var emp in data)
                {
                    if (emp == null) continue;

                    if (emp.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        emp.IsSelected = false;
                        selectedEmployees[emp.ID] = false;
                        continue;
                    }

                    emp.IsSelected = IsSelected;
                    selectedEmployees[emp.ID] = IsSelected;
                }

                gridViewFormList.RefreshData();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void gridViewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var hitInfo = gridViewFormList.CalcHitInfo(gridControl2.PointToClient(Control.MousePosition));
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsSelected")
                {
                    var rowData = gridViewFormList.GetRow(hitInfo.RowHandle) as EmployeeADO;
                    if (rowData != null && rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        rowData.IsSelected = !rowData.IsSelected;
                        selectedEmployees[rowData.ID] = rowData.IsSelected;
                        gridViewFormList.RefreshRow(hitInfo.RowHandle);

                        var data = gridControl2.DataSource as List<EmployeeADO>;
                        if (data != null && data.Any(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                        {
                            isCheckAllEmployee = data
                                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                .All(o => o.IsSelected);
                        }
                        else
                        {
                            isCheckAllEmployee = false;
                        }
                        gridViewFormList.InvalidateColumnHeader(hitInfo.Column);
                    }
                    return;
                }
                if (backgroundWorker1.IsBusy)
                {
                    WaitingManager.Show();
                }
                else
                {
                    var empAdo = gridViewFormList.GetFocusedRow() as EmployeeADO;
                    //var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
                    if (empAdo != null)
                    {
                        var rowData = new HIS_EMPLOYEE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EMPLOYEE>(rowData, empAdo);
                        CommonParam param = new CommonParam();
                        ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                        acsFilter.LOGINNAME = rowData.LOGINNAME;
                        currentDataUser = new BackendAdapter(param).Get<List<ACS_USER>>
                            (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, acsFilter, param).FirstOrDefault();
                        if (currentDataUser != null)
                        {
                            currentDataEmployee = rowData;
                            ChangedDataRow(rowData);
                            SetValueMediStock(this.cboDefaultMediStockIds, this.mediStockSeleteds, BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList());
                            SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());
                            SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());
                            txtLoginName.Enabled = false;
                        }
                        else
                        {
                            if (MessageBox.Show("Tài khoản này chưa có tài khoản đăng nhập. Bạn có muốn tạo tài khoản đăng nhập để tiếp tục xử lý không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                bool success = false;
                                ACS_USER updateDTO_User = new ACS_USER();
                                WaitingManager.Show();
                                updateDTO_User.LOGINNAME = rowData.LOGINNAME;
                                updateDTO_User.USERNAME = rowData.TDL_USERNAME;
                                updateDTO_User.EMAIL = rowData.TDL_EMAIL;
                                updateDTO_User.MOBILE = rowData.TDL_MOBILE;
                                updateDTO_User.IS_ACTIVE = IS_ACTIVE_TRUE;
                                resultDataUser = new BackendAdapter(param).Post<ACS.EFMODEL.DataModels.ACS_USER>(HisRequestUriStore.ACS_USER_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO_User, param);
                                if (resultDataUser != null)
                                {
                                    success = true;
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, success);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                HIS_EMPLOYEE data = (HIS_EMPLOYEE)gridViewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemRedo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
            try
            {
                CommonParam param = new CommonParam();
                ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                acsFilter.LOGINNAME = rowData.LOGINNAME;
                currentDataUser = new BackendAdapter(param).Get<List<ACS_USER>>
                    (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, acsFilter, param).FirstOrDefault();
                if (currentDataUser != null)
                {
                    ChangedDataRow(rowData);
                    bool notHandler = false;
                    if (MessageBox.Show("Bạn có muốn bỏ khóa dữ liệu không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var resultLock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                            (HisRequestUriStore.HIS_EMPLOYEE_CHANGELOCK, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (resultLock != null)
                        {
                            //Ghi nhat ky hoat dong
                            string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            string message = "Mở khóa tài khoản nhân viên. LOGINNAME: " + resultLock.LOGINNAME;
                            SdaEventLogCreate eventlog = new SdaEventLogCreate();
                            eventlog.Create(login, null, true, message);
                            //
                            notHandler = true;
                            FillDataToGridControl();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, notHandler);
                    }
                }
                else
                {
                    if (MessageBox.Show("Tài khoản này chưa có tài khoản đăng nhập. Bạn có muốn tạo tài khoản đăng nhập để tiếp tục xử lý không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ChangedDataRow(rowData);
                        bool success = false;
                        ACS_USER updateDTO_User = new ACS_USER();
                        WaitingManager.Show();
                        updateDTO_User.LOGINNAME = rowData.LOGINNAME;
                        updateDTO_User.USERNAME = rowData.TDL_USERNAME;
                        updateDTO_User.EMAIL = rowData.TDL_EMAIL;
                        updateDTO_User.MOBILE = rowData.TDL_MOBILE;
                        updateDTO_User.IS_ACTIVE = IS_ACTIVE_TRUE;
                        resultDataUser = new BackendAdapter(param).Post<ACS.EFMODEL.DataModels.ACS_USER>(HisRequestUriStore.ACS_USER_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO_User, param);
                        if (resultDataUser != null)
                        {
                            success = true;
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {

            var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
            try
            {
                CommonParam param = new CommonParam();
                ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                acsFilter.LOGINNAME = rowData.LOGINNAME;
                currentDataUser = new BackendAdapter(param).Get<List<ACS_USER>>
                    (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, acsFilter, param).FirstOrDefault();
                if (currentDataUser != null)
                {
                    ChangedDataRow(rowData);
                    bool notHandler = false;
                    if (MessageBox.Show("Bạn có muốn khóa dữ liệu không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var resultUnlock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                            (HisRequestUriStore.HIS_EMPLOYEE_CHANGELOCK, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);

                        if (resultUnlock != null)
                        {
                            //Ghi nhat ky hoat dong
                            string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            string message = "Khóa tài khoản nhân viên. LOGINNAME: " + resultUnlock.LOGINNAME; ;
                            SdaEventLogCreate eventlog = new SdaEventLogCreate();
                            eventlog.Create(login, null, true, message);
                            //
                            notHandler = true;
                            FillDataToGridControl();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, notHandler);
                    }
                }
                else
                {
                    if (MessageBox.Show("Tài khoản này chưa có tài khoản đăng nhập. Bạn có muốn tạo tài khoản đăng nhập để tiếp tục xử lý không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ChangedDataRow(rowData);
                        bool success = false;
                        ACS_USER updateDTO_User = new ACS_USER();
                        WaitingManager.Show();
                        updateDTO_User.LOGINNAME = rowData.LOGINNAME;
                        updateDTO_User.USERNAME = rowData.TDL_USERNAME;
                        updateDTO_User.EMAIL = rowData.TDL_EMAIL;
                        updateDTO_User.MOBILE = rowData.TDL_MOBILE;
                        updateDTO_User.IS_ACTIVE = IS_ACTIVE_TRUE;
                        resultDataUser = new BackendAdapter(param).Post<ACS.EFMODEL.DataModels.ACS_USER>(HisRequestUriStore.ACS_USER_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO_User, param);
                        if (resultDataUser != null)
                        {
                            success = true;
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    FillDataToGridControl();
                    //ResetFormData();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void cbbRank_Properties_Leave(object sender, EventArgs e)
        {
            //if (cbbGCode.EditValue.ToString() == null)
            //    cbbGCode.Reset();-
        }

        private void btnDefaultPass_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
            try
            {
                CommonParam param = new CommonParam();
                ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                acsFilter.LOGINNAME = rowData.LOGINNAME;
                currentDataUser = new BackendAdapter(param).Get<List<ACS_USER>>
                    (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, acsFilter, param).FirstOrDefault();
                if (currentDataUser != null)
                {
                    ChangedDataRow(rowData);
                    if (MessageBox.Show("Bạn có muốn reset mật khẩu không?",
                      "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        var resultDefault = new BackendAdapter(param).Post<bool>(HisRequestUriStore.ACS_USER_RESET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, currentDataUser, param);

                        if (resultDefault)
                        {
                            success = true;
                            var data = (List<HIS_EMPLOYEE>)gridControl2.DataSource;
                            foreach (var item in data)
                            {
                                if (item.LOGINNAME == rowData.LOGINNAME)
                                {
                                    item.ID = rowData.ID;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            gridViewFormList.BeginUpdate();
                            gridControl2.DataSource = data;
                            gridViewFormList.EndUpdate();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                        txtLoginName.Focus();
                        txtLoginName.SelectAll();
                    }
                }
                else
                {
                    if (MessageBox.Show("Tài khoản này chưa có tài khoản đăng nhập. Bạn có muốn tạo tài khoản đăng nhập để tiếp tục xử lý không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ChangedDataRow(rowData);
                        bool success = false;
                        ACS_USER updateDTO_User = new ACS_USER();
                        WaitingManager.Show();
                        updateDTO_User.LOGINNAME = rowData.LOGINNAME;
                        updateDTO_User.USERNAME = rowData.TDL_USERNAME;
                        updateDTO_User.EMAIL = rowData.TDL_EMAIL;
                        updateDTO_User.MOBILE = rowData.TDL_MOBILE;
                        updateDTO_User.IS_ACTIVE = IS_ACTIVE_TRUE;
                        resultDataUser = new BackendAdapter(param).Post<ACS.EFMODEL.DataModels.ACS_USER>(HisRequestUriStore.ACS_USER_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO_User, param);
                        if (resultDataUser != null)
                        {
                            success = true;
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            WaitingManager.Hide();
        }

        private void btnRoleUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
            try
            {

                CommonParam param = new CommonParam();
                ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                acsFilter.LOGINNAME = rowData.LOGINNAME;
                currentDataUser = new BackendAdapter(param).Get<List<ACS_USER>>
                    (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, acsFilter, param).FirstOrDefault();
                if (currentDataUser != null)
                {
                    ChangedDataRow(rowData);
                    RoleUsers.frmRoleUsers frmRoleUser = new RoleUsers.frmRoleUsers(currentDataUser);
                    frmRoleUser.ShowDialog();
                }
                else
                {
                    if (MessageBox.Show("Tài khoản này chưa có tài khoản đăng nhập. Bạn có muốn tạo tài khoản đăng nhập để tiếp tục xử lý không?",
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ChangedDataRow(rowData);
                        bool success = false;
                        ACS_USER updateDTO_User = new ACS_USER();
                        WaitingManager.Show();
                        updateDTO_User.LOGINNAME = rowData.LOGINNAME;
                        updateDTO_User.USERNAME = rowData.TDL_USERNAME;
                        updateDTO_User.EMAIL = rowData.TDL_EMAIL;
                        updateDTO_User.MOBILE = rowData.TDL_MOBILE;
                        updateDTO_User.IS_ACTIVE = IS_ACTIVE_TRUE;
                        resultDataUser = new BackendAdapter(param).Post<ACS.EFMODEL.DataModels.ACS_USER>(HisRequestUriStore.ACS_USER_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO_User, param);
                        if (resultDataUser != null)
                        {
                            success = true;
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {

                //List<object> args = new List<object>();
                //HIS.Desktop.Plugins.AcsUser.CallModule call = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, 0, 0, args);
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)FillDataToGridControl);
                if (this.moduleData != null)
                {
                    HIS.Desktop.Plugins.AcsUser.CallModule callModule = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    HIS.Desktop.Plugins.AcsUser.CallModule callModule = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, 0, 0, listArgs);
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                btnImport.Focus();
                WaitingManager.Show();
                btnImport_Click(null, null);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbRank_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        spinMaxBhytServiceReqPerDay.Focus();
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void chkAllowUpdateOtherSclinical_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowBlockConcurrentCLS.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNurse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    checkAdmin.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    txtBank.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    cboDefaultMediStockIds.Focus();
                    cboDefaultMediStockIds.ShowPopup();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDefaultMediStockIds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    chkAllowUpdateOtherSclinical.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtERXPassword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txtERXName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtERXPassword.Focus();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void dtDOB_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dtDOB.Properties.Buttons[1].Visible = false;
                if (dtDOB.EditValue != null)
                {
                    dtDOB.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbRank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cbbRank.EditValue = null;
                    this.cbbRank.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboDepartment.EditValue = null;
                    this.cboDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbRank_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cbbRank.Properties.Buttons[1].Visible = false;
                if (cbbRank.EditValue != null)
                {
                    cbbRank.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.Properties.Buttons[1].Visible = false;
                if (cboDepartment.EditValue != null)
                {
                    cboDepartment.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDefaultMediStockIds_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                GridCheckMarksSelection gridCheckMark = cboDefaultMediStockIds.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDefaultMediStockIds.Properties.View);
                }
                cboDefaultMediStockIds.EditValue = null;
                cboDefaultMediStockIds.Focus();
            }
        }

        private void spinMaxBhytServiceReqPerDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spnMaxServiceReqPerDay.Enabled = spinMaxBhytServiceReqPerDay.EditValue == null;
                chkWorkOnly.Enabled = chkWorkOnly.Checked = spinMaxBhytServiceReqPerDay.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnMaxServiceReqPerDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinMaxBhytServiceReqPerDay.Enabled = spnMaxServiceReqPerDay.EditValue == null;
                chkWorkOnly.Enabled = chkWorkOnly.Checked = spnMaxServiceReqPerDay.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkWorkOnly_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountNumber.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkWorkOnly_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                chkWorkOnly.Enabled = spinMaxBhytServiceReqPerDay.EditValue != null || spnMaxServiceReqPerDay.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtDOB.Properties.Buttons[1].Visible = false;
                    dtDOB.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAllowBlockConcurrentCLS_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtERXName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSpecialityCodes_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                //if (gridCheckMark == null) return;
                //foreach (HIS_SPECIALITY rv in gridCheckMark.Selection)
                //{
                //    if (sb.ToString().Length > 0)
                //    {
                //        sb.Append(";");
                //    }
                //    sb.Append(rv.SPECIALITY_CODE.ToString());
                //}
                //e.DisplayText = sb.ToString();

                e.DisplayText = "";
                string roomName = "";
                if (this.specialitySeleteds != null && this.specialitySeleteds.Count > 0)
                {
                    foreach (var item in this.specialitySeleteds)
                    {
                        roomName += item.SPECIALITY_CODE + ";";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboSpecialityCodes_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboSpecialityCodes.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboSpecialityCodes.Properties.View);
                    }
                    cboSpecialityCodes.EditValue = null;
                    cboSpecialityCodes.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboMediOrgCodes_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                //if (gridCheckMark == null) return;
                //foreach (HIS_MEDI_ORG rv in gridCheckMark.Selection)
                //{
                //    if (sb.ToString().Length > 0)
                //    {
                //        sb.Append(";");
                //    }
                //    sb.Append(rv.MEDI_ORG_CODE.ToString());
                //}
                //e.DisplayText = sb.ToString();

                e.DisplayText = "";
                string roomName = "";
                if (this.mediOrgSeleteds != null && this.mediOrgSeleteds.Count > 0)
                {
                    foreach (var item in this.mediOrgSeleteds)
                    {
                        roomName += item.MEDI_ORG_CODE + ";";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboMediOrgCodes_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboMediOrgCodes.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboMediOrgCodes.Properties.View);
                    }
                    cboMediOrgCodes.EditValue = null;
                    cboMediOrgCodes.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboEthenic_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEthenic.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboCareerTitle_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCareerTitle.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareerTitle").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CareerTitle");
                        return;
                    }
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        listArgs.Add((RefeshReference)(() =>
                        {
                            LoadCareerTitle();
                        }));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboPostion_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPostion.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboTypeOfTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTypeOfTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboBranch_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBranch.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {

            try
            {
                string savePath = "";
                FolderBrowserDialog fbd1 = new FolderBrowserDialog();
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    savePath = fbd1.SelectedPath;
                }
                if (String.IsNullOrEmpty(savePath))
                    return;

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                string folderPath = null;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    folderPath = fbd.SelectedPath;
                }
                if (folderPath == null) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                //var apiResultEmployee = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                //      (HisRequestUriStore.HIS_EMPLOYEE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                //if(apiResultEmployee != null && apiResultEmployee.Count > 0)
                //{
                //    apiResultEmployee = apiResultEmployee.Where(o => !string.IsNullOrEmpty(o.DIPLOMA)).ToList();
                //}
                //if (apiResultEmployee == null || apiResultEmployee.Count == 0)
                //    return;
                //List<XML> lstXml = new List<XML>();
                //int count = 1;

                var adoList = gridControl2.DataSource as List<EmployeeADO>;
                if (adoList == null || adoList.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không có dữ liệu để xuất XML.", "Thông báo");
                    return;
                }

                var selected = adoList.Where(x => x.IsSelected && x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (selected.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không có nhân viên nào được chọn để xuất XML.", "Thông báo");
                    return;
                }

                List<XML> lstXml = new List<XML>();
                int count = 1;

                foreach (var currentData in selected)
                {
                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == currentData.BRANCH_ID);

                    XML xml = new XML();
                    xml.STT = count++;
                    xml.MA_CSKCB = branch != null ? branch.HEIN_MEDI_ORG_CODE : "";
                    xml.HO_TEN = this.ConvertStringToXmlDocument(currentData.TDL_USERNAME ?? "");
                    //xml.GIOI_TINH = currentData.GENDER_ID == 1 ? "2" : (currentData.GENDER_ID == 2 ? "1" : "3");
                    xml.GIOI_TINH = currentData.GENDER_ID == 1 ? "1" : (currentData.GENDER_ID == 2 ? "2" : "3");
                    xml.MA_DANTOC = currentData.ETHNIC_CODE;
                    xml.NGAY_SINH = currentData.DOB != null ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentData.DOB ?? 0) ?? DateTime.MinValue).ToString("yyyyMMdd") : "";
                    xml.SO_CCCD = currentData.IDENTIFICATION_NUMBER ?? currentData.SOCIAL_INSURANCE_NUMBER ?? "";
                    xml.CHUCDANH_NN = "";
                    if (lstCareer != null && lstCareer.Count > 0)
                        xml.CHUCDANH_NN = currentData.CAREER_TITLE_ID != null ? (lstCareer.FirstOrDefault(o => o.ID == currentData.CAREER_TITLE_ID.Value) != null ? lstCareer.FirstOrDefault(o => o.ID == currentData.CAREER_TITLE_ID.Value).CAREER_TITLE_CODE : "") : "";
                    xml.VI_TRI = currentData.POSITION != null ? currentData.POSITION.Value.ToString() : "";
                    xml.MA_CCHN = this.ConvertStringToXmlDocument(currentData.DIPLOMA ?? "");
                    xml.NGAYCAP_CCHN = currentData.DIPLOMA_DATE != null ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentData.DIPLOMA_DATE ?? 0) ?? DateTime.MinValue).ToString("yyyyMMdd") : "";
                    xml.NOICAP_CCHN = this.ConvertStringToXmlDocument(currentData.DIPLOMA_PLACE ?? "");
                    xml.PHAMVI_CM = currentData.SPECIALITY_CODES ?? "";
                    xml.THOIGIAN_DK = currentData.TYPE_OF_TIME != null ? currentData.TYPE_OF_TIME.Value.ToString() : "";
                    xml.CSKCB_KHAC = currentData.MEDI_ORG_CODES ?? "";
                    lstXml.Add(xml);
                }


                var fileName = string.Format("XML_{0}", new string[] { DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss") });
                var path = string.Format("{0}/{1}.xml", folderPath, fileName);

                string fullFileName = String.Format("MayCLS_{0}.xml", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string saveFilePath = String.Format("{0}/{1}", savePath, fullFileName);
                bool Sucess = CreatedXmlFile(lstXml, false, true, path);
                if (chkSign.Checked)
                {
                    SignFile(fullFileName, saveFilePath);
                }
                WaitingManager.Hide();
                if (Sucess)
                {
                    XtraMessageBox.Show("Lưu file xml thành công", "Thông báo");
                    if (XtraMessageBox.Show("Bạn có muốn mở file?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(path);
                }
                else
                {
                    XtraMessageBox.Show("Lưu file xml thất bại", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }
        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO() { HsmSerialNumber = SettingSignADO.SerialNumber, HsmType = SettingSignADO.Id, HsmUserCode = SettingSignADO.Name, Password = SettingSignADO.Password, SecretKey = SettingSignADO.SercetKey, IdentityNumber = SettingSignADO.CccdNumber };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private string RemoveByteOrderMark(string XML)
        {
            string byteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (XML.StartsWith(byteOrderMark))
            {
                XML = XML.Remove(0, byteOrderMark.Length);
            }
            return XML;
        }
        public byte[] StringToBytes(string input)
        {
            if (input == null) return null;
            return Encoding.UTF8.GetBytes(input);
        }
        private string ReadFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(RemoveByteOrderMark(Encoding.UTF8.GetString(File.ReadAllBytes(filePath))));
                        return Convert.ToBase64String(StringToBytes(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes))));
                    }
                    catch (Exception)
                    {
                        xmlDocument.LoadXml(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                        return Convert.ToBase64String(StringToBytes(Encoding.UTF8.GetString(fileBytes)));
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {

                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        public bool SignFile(string fullFileName, string saveFilePath)
        {
            try
            {
                if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                {
                    MessageBox.Show("Không có thông tin Usb Token ký số");
                    return false;
                }
                else
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                    Directory.CreateDirectory(tempFolderPath);
                    string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                    File.Create(tempFilePath).Close();
                    string pathAfterFileSign = null;
                    WcfSignDCO wcfSignDCO = null;
                    if (SettingSignADO.IsHsm)
                    {
                        var xmlBase64 = SourceFileSignApi(ReadFileContent(saveFilePath));
                        if (string.IsNullOrEmpty(xmlBase64))
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                            return false;
                        }
                        var xmlBytes = Convert.FromBase64String(xmlBase64);
                        File.WriteAllBytes(tempFilePath, xmlBytes);
                        pathAfterFileSign = tempFilePath;
                    }
                    else
                    {
                        wcfSignDCO = new WcfSignDCO
                        {
                            SerialNumber = SettingSignADO.SerialNumber,
                            OutputFile = tempFilePath,
                            PIN = "",
                            SourceFile = saveFilePath,
                            fieldSigned = "CHUKYDONVI"
                        };
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        if (!VerifyServiceSignProcessorIsRunning())
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Service ký số không chạy");
                        }
                        var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                        if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Ký file thất bại: " + (wcfSignResultDCO != null ? wcfSignResultDCO.Message : ""));
                            return false;
                        }
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                    }
                    if (!string.IsNullOrEmpty(pathAfterFileSign) && File.Exists(pathAfterFileSign))
                    {
                        File.Copy(pathAfterFileSign, saveFilePath, true);
                    }
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                    if (Directory.Exists(tempFolderPath) && Directory.GetFiles(tempFolderPath).Length == 0 && Directory.GetDirectories(tempFolderPath).Length == 0)
                    {
                        Directory.Delete(tempFolderPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }
        public bool CreatedXmlFile<T>(T input, bool displayNamspacess, bool saveFile, string path)
        {
            bool rs = false;
            string xmlFile = null;
            try
            {
                var enc = Encoding.UTF8;
                using (var ms = new MemoryStream())
                {
                    var xmlNamespaces = new XmlSerializerNamespaces();
                    if (displayNamspacess)
                    {
                        xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                        xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    }
                    else
                        xmlNamespaces.Add("", "");

                    var xmlWriterSettings = new XmlWriterSettings
                    {
                        CloseOutput = false,
                        Encoding = enc,
                        OmitXmlDeclaration = false,
                        Indent = true
                    };
                    using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                    {
                        var s = new XmlSerializer(typeof(T));
                        s.Serialize(xw, input, xmlNamespaces);
                    }
                    xmlFile = enc.GetString(ms.ToArray());
                }

                if (saveFile)
                {
                    using (var file = new StreamWriter(path))
                    {
                        file.Write(xmlFile);
                    }
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = false;
            }
            return rs;
        }
        private XmlCDataSection ConvertStringToXmlDocument(string data)
        {
            XmlCDataSection result;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<book genre='novel' ISBN='1-861001-57-5'>" + "<title>Pride And Prejudice</title>" + "</book>");
            result = doc.CreateCDataSection(RemoveXmlCharError(data));
            return result;
        }
        private string RemoveXmlCharError(string data)
        {
            string result = "";
            try
            {
                StringBuilder s = new StringBuilder();
                if (!String.IsNullOrWhiteSpace(data))
                {
                    foreach (char c in data)
                    {
                        if (!System.Xml.XmlConvert.IsXmlChar(c)) continue;
                        s.Append(c);
                    }
                }

                result = s.ToString();
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        SettingSignADO SettingSignADO;
        private bool isNotLoadWhileChangeControlStateInFirst;
        //private bool isCheckAll = false;
        private void isChkSignFileCertUtil()
        {
            try
            {
                if (chkSign.Checked == true)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = (SettingSignADO)result;
                    });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSign.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == this.moduleData.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    csAddOrUpdate.MODULE_LINK = this.moduleData.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                isChkSignFileCertUtil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSign.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSign.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                chkSign.Checked = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXmlTT12_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;

                // Chọn thư mục lưu XML TT12
                string folderPath = null;
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        folderPath = fbd.SelectedPath;
                    }
                }
                if (string.IsNullOrEmpty(folderPath))
                    return;

                WaitingManager.Show();

                // Lấy danh sách trên lưới (EmployeeADO)
                var adoList = gridControl2.DataSource as List<EmployeeADO>;
                if (adoList == null || adoList.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không có dữ liệu nhân viên để xuất XML TT12.", "Thông báo");
                    return;
                }

                // Chỉ lấy các bản ghi đã tick chọn và đang hoạt động
                var selected = adoList
                    .Where(x => x.IsSelected && x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Cast<HIS_EMPLOYEE>() // EmployeeADO kế thừa HIS_EMPLOYEE
                    .ToList();

                if (selected == null || selected.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không có bản ghi nào được chọn để xuất XML TT12.", "Thông báo");
                    return;
                }

                // Sinh ADO TT12 dựa trên HIS_EMPLOYEE và thiết kế 24 chỉ tiêu
                List<XMLData.EmployeeTT12Ado> listXmlAdos = GenerateXmlTT12Ado(selected);
                if (listXmlAdos == null || listXmlAdos.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không sinh được dữ liệu XML TT12.", "Thông báo");
                    return;
                }

                // Map sang model XMLEmployeeTT12DetailData theo 24 thẻ XML
                List<XMLData.XMLEmployeeTT12DetailData> listXmlDetails = new List<XMLData.XMLEmployeeTT12DetailData>();
                MapADOToXmlTT12(listXmlAdos, ref listXmlDetails);

                XMLData.XMLEmployeeTT12Data xmlData = new XMLData.XMLEmployeeTT12Data
                {
                    DANHSACH_DMNHANLUCKBCB = new XMLData.XMLEmployeeTT12ListData
                    {
                        Id = "Id-" + Guid.NewGuid().ToString(),
                        DMNHANLUCKBCB = listXmlDetails
                    },
                    ChuKyDonVi = string.Empty
                };

                string fullFileName = string.Format("NhanVien_TT12_{0}.xml", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string saveFilePath = Path.Combine(folderPath, fullFileName);

                // Serialize ra file theo XMLEmployeeTT12Data/XMLEmployeeTT12DetailData
                success = CreatedXmlFile(xmlData, true, true, saveFilePath);

                // Nếu cần ký số thì tái sử dụng hạ tầng ký đã có (USB Token / HSM)
                if (success && chkSign.Checked)
                {
                    if (!SignFile(fullFileName, saveFilePath))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký file XML TT12 thất bại.");
                    }
                }

                WaitingManager.Hide();

                CommonParam param = new CommonParam();
                MessageManager.Show(this, param, success);

                if (success)
                {
                    if (XtraMessageBox.Show("Lưu file XML TT12 thành công. Bạn có muốn mở file?", "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(saveFilePath);
                        }
                        catch (Exception exOpen)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exOpen);
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("Lưu file XML TT12 thất bại.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<XMLData.EmployeeTT12Ado> GenerateXmlTT12Ado(List<HIS_EMPLOYEE> listEmployees)
        {
            List<XMLData.EmployeeTT12Ado> result = new List<XMLData.EmployeeTT12Ado>();
            try
            {
                if (listEmployees == null || listEmployees.Count == 0)
                    return result;

                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>() ?? new List<HIS_DEPARTMENT>();
                var branches = BackendDataWorker.Get<HIS_BRANCH>() ?? new List<HIS_BRANCH>();
                var careerTitles = BackendDataWorker.Get<HIS_CAREER_TITLE>() ?? new List<HIS_CAREER_TITLE>();
                var services = BackendDataWorker.Get<HIS_SERVICE>() ?? new List<HIS_SERVICE>();
                var genders = BackendDataWorker.Get<HIS_GENDER>() ?? new List<HIS_GENDER>();

                int count = 1;
                foreach (var employee in listEmployees)
                {
                    List<HIS_DEPARTMENT> employeeDepartments;
                    if (!string.IsNullOrEmpty(employee.DEPARTMENT_CODES))
                    {
                        employeeDepartments = new List<HIS_DEPARTMENT>();
                        string[] parts = employee.DEPARTMENT_CODES.Split(';');
                        int idx = 0;
                        while (idx < parts.Length)
                        {
                            bool found = false;
                            for (int j = parts.Length - 1; j >= idx; j--)
                            {
                                string candidate = string.Join(";", parts, idx, j - idx + 1).Trim();
                                var dept = departments.FirstOrDefault(o => o.DEPARTMENT_CODE == candidate);
                                if (dept != null)
                                {
                                    employeeDepartments.Add(dept);
                                    idx = j + 1;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) idx++;
                        }
                    }
                    else 
                    {
                        var dept = departments.FirstOrDefault(o => o.ID == employee.DEPARTMENT_ID);
                        employeeDepartments = dept != null ? new List<HIS_DEPARTMENT> { dept } : new List<HIS_DEPARTMENT>();
                    }
                    var branch = branches.FirstOrDefault(o => o.ID == employee.BRANCH_ID);
                    var careerTitle = careerTitles.FirstOrDefault(o => o.ID == employee.CAREER_TITLE_ID);

                    var xmlTT12 = new XMLData.EmployeeTT12Ado();

                    // 1. STT
                    xmlTT12.STT = count;

                    // 2. MA_KHOA
                    xmlTT12.MA_KHOA = string.Join(";",employeeDepartments.Select(o => o.BHYT_CODE ?? string.Empty));

                    // 3. TEN_KHOA
                    xmlTT12.TEN_KHOA = string.Join(";",employeeDepartments.Select(o => o.DEPARTMENT_NAME ?? string.Empty));

                    // 4. HO_TEN
                    xmlTT12.HO_TEN = employee.TDL_USERNAME ?? string.Empty;

                    // 5. GIOI_TINH - Lookup GENDER_CODE từ HIS_GENDER và đảo mapping
                    if (employee.GENDER_ID.HasValue)
                    {
                        var gender = genders.FirstOrDefault(o => o.ID == employee.GENDER_ID.Value);
                        if (gender != null && !string.IsNullOrEmpty(gender.GENDER_CODE))
                        {
                            if (gender.GENDER_CODE == "01")
                                xmlTT12.GIOI_TINH = "2";// Nữ trong DB → "02" trong XML
                            else if (gender.GENDER_CODE == "02")
                                xmlTT12.GIOI_TINH = "1"; // Nam trong DB → "01" trong XML
                            else if (gender.GENDER_CODE == "03")
                                xmlTT12.GIOI_TINH = "3";// Không xác định giữ nguyên
                            else
                                xmlTT12.GIOI_TINH = string.Empty;
                        }
                        else
                        {
                            xmlTT12.GIOI_TINH = string.Empty;
                        }
                    }
                    else
                    {
                        xmlTT12.GIOI_TINH = string.Empty;
                    }

                    // 6. SO_DINH_DANH
                    xmlTT12.SO_DINH_DANH = employee.IDENTIFICATION_NUMBER ?? string.Empty;

                    // 7. CHUCDANH_NN (mã nghề nghiệp)
                    xmlTT12.CHUCDANH_NN = careerTitle != null
                        ? (careerTitle.CAREER_TITLE_CODE ?? string.Empty)
                        : string.Empty;

                    // 8. VI_TRI
                    xmlTT12.VI_TRI = employee.POSITION.HasValue
                        ? employee.POSITION.Value.ToString()
                        : string.Empty;

                    // 9. MACCHN
                    xmlTT12.MACCHN = employee.DIPLOMA ?? string.Empty;

                    // 10. NGAYCAP_CCHN (yyyyMMdd)
                    xmlTT12.NGAYCAP_CCHN = ToDate8(employee.DIPLOMA_DATE);

                    // 11. NOICAP_CCHN
                    xmlTT12.NOICAP_CCHN = employee.DIPLOMA_PLACE ?? string.Empty;

                    // 12. PHAMVI_CM
                    xmlTT12.PHAMVI_CM = employee.SPECIALITY_CODES ?? string.Empty;

                    // 13. PHAMVI_CMBS
                    xmlTT12.PHAMVI_CMBS = employee.PRACTICE_SCOPE_DECISION ?? string.Empty;

                    // 14. DVKT_KHAC (từ OTHER_SERVICE_CODES -> HEIN_SERVICE_BHYT_CODE, ghép ;)
                    xmlTT12.DVKT_KHAC = BuildOtherServiceHeinCodes(employee.OTHER_SERVICE_CODES, services);

                    // 15. VB_PHANCONG
                    xmlTT12.VB_PHANCONG = employee.ASSIGNMENT_DOCUMENT ?? string.Empty;

                    // 16. THOIGIAN_DK (1/2)
                    xmlTT12.THOIGIAN_DK = employee.TYPE_OF_TIME.HasValue
                        ? employee.TYPE_OF_TIME.Value.ToString()
                        : string.Empty;

                    // 17. THOIGIAN_NGAY
                    xmlTT12.THOIGIAN_NGAY = employee.WORKING_SCHEDULE ?? string.Empty;

                    // 18. THOIGIAN_TUAN
                    xmlTT12.THOIGIAN_TUAN = employee.WEEK_WORK_DAYS ?? string.Empty;

                    // 19. CSKCB_KHAC
                    xmlTT12.CSKCB_KHAC = employee.MEDI_ORG_CODES ?? string.Empty;

                    // 20. CSKCB_CGKT
                    xmlTT12.CSKCB_CGKT = employee.TRANSFER_MEDI_ORG_CODE ?? string.Empty;

                    // 21. QD_CGKT
                    xmlTT12.QD_CGKT = employee.TECH_TRANSFER_DECISIONS ?? string.Empty;

                    // 22. TU_NGAY
                    xmlTT12.TU_NGAY = ToDate8(employee.FROM_TIME);

                    // 23. DEN_NGAY
                    xmlTT12.DEN_NGAY = ToDate8(employee.TO_TIME);

                    // 24. MA_CSKCB
                    xmlTT12.MA_CSKCB = branch != null
                        ? (branch.HEIN_MEDI_ORG_CODE ?? string.Empty)
                        : string.Empty;

                    result.Add(xmlTT12);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private string ToDate8(long? timeNumber)
        {
            if (!timeNumber.HasValue || timeNumber.Value <= 0)
                return string.Empty;

            var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
            return dt.HasValue ? dt.Value.ToString("yyyyMMdd") : string.Empty;
        }

        private string BuildOtherServiceHeinCodes(string otherServiceCodes, List<HIS_SERVICE> services)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(otherServiceCodes) || services == null || services.Count == 0)
                    return string.Empty;

                var codes = otherServiceCodes
                    .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                if (codes.Count == 0)
                    return string.Empty;

                List<string> heinCodes = new List<string>();
                foreach (var code in codes)
                {
                    var srv = services.FirstOrDefault(o => o.SERVICE_CODE == code);
                    if (srv != null && !string.IsNullOrEmpty(srv.HEIN_SERVICE_BHYT_CODE))
                    {
                        heinCodes.Add(srv.HEIN_SERVICE_BHYT_CODE);
                    }
                }

                return string.Join(";", heinCodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        public void MapADOToXmlTT12(List<XMLData.EmployeeTT12Ado> listAdo,
                            ref List<XMLData.XMLEmployeeTT12DetailData> datas)
        {
            try
            {
                if (datas == null)
                    datas = new List<XMLData.XMLEmployeeTT12DetailData>();

                if (listAdo == null || listAdo.Count == 0)
                    return;

                foreach (var ado in listAdo)
                {
                    var detail = new XMLData.XMLEmployeeTT12DetailData
                    {
                        STT = ado.STT,
                        MA_KHOA = ado.MA_KHOA,
                        TEN_KHOA = ado.TEN_KHOA,
                        HO_TEN = ado.HO_TEN,
                        GIOI_TINH = ado.GIOI_TINH,
                        SO_DINH_DANH = ado.SO_DINH_DANH,
                        CHUCDANH_NN = ado.CHUCDANH_NN,
                        VI_TRI = ado.VI_TRI,
                        MACCHN = ado.MACCHN,
                        NGAYCAP_CCHN = ado.NGAYCAP_CCHN,
                        NOICAP_CCHN = ado.NOICAP_CCHN,
                        PHAMVI_CM = ado.PHAMVI_CM,
                        PHAMVI_CMBS = ado.PHAMVI_CMBS,
                        DVKT_KHAC = ado.DVKT_KHAC,
                        VB_PHANCONG = ado.VB_PHANCONG,
                        THOIGIAN_DK = ado.THOIGIAN_DK,
                        THOIGIAN_NGAY = ado.THOIGIAN_NGAY,
                        THOIGIAN_TUAN = ado.THOIGIAN_TUAN,
                        CSKCB_KHAC = ado.CSKCB_KHAC,
                        CSKCB_CGKT = ado.CSKCB_CGKT,
                        QD_CGKT = ado.QD_CGKT,
                        TU_NGAY = ado.TU_NGAY,
                        DEN_NGAY = ado.DEN_NGAY,
                        MA_CSKCB = ado.MA_CSKCB
                    };

                    datas.Add(detail);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    dtTimeTo.Enabled = false;
                }
                else
                {
                    dtTimeTo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    dtTimeFrom.Enabled = false;
                }
                else
                {
                    dtTimeFrom.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFormList_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view.FocusedColumn.FieldName == "IsSelected")
                {
                    var data = (HIS_EMPLOYEE)view.GetFocusedRow();
                    if (data != null && data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        // Dòng khóa: không cho tick
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "IsSelected")
                {
                    e.Info.InnerElements.Clear();
                    e.Info.Caption = string.Empty;

                    e.Painter.DrawObject(e.Info);

                    Rectangle checkRect = e.Bounds;
                    int size = 14;
                    checkRect.X += (e.Bounds.Width - size) / 2;
                    checkRect.Y += (e.Bounds.Height - size) / 2;
                    checkRect.Width = size;
                    checkRect.Height = size;

                    DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
                    DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
                    DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit edit;

                    edit = (DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit)repositoryItemCheckIsSelected; // repositoryChk bạn đã kéo vào form
                    painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)edit.CreatePainter();
                    info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)edit.CreateViewInfo();
                    info.EditValue = isCheckAllEmployee;
                    info.Bounds = checkRect;
                    info.CalcViewInfo(e.Graphics);
                    DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args =
                        new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics), checkRect);
                    painter.Draw(args);
                    args.Cache.Dispose();

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

                if (hitInfo.InColumn && hitInfo.Column != null && hitInfo.Column.FieldName == "IsSelected")
                {
                    // Toggle trạng thái check-all
                    isCheckAllEmployee = !isCheckAllEmployee;

                    // Áp dụng cho dữ liệu (bỏ qua dòng khóa)
                    SelectAllEmployees(isCheckAllEmployee);

                    // Vẽ lại header
                    view.InvalidateColumnHeader(hitInfo.Column);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsSelected")
                {
                    var data = gridControl2.DataSource as List<EmployeeADO>;
                    if (data != null && data.Count > 0)
                    {
                        // Cập nhật lại selectedEmployees cho dòng vừa đổi (phòng trường hợp edit trực tiếp)
                        var row = gridViewFormList.GetRow(e.RowHandle) as EmployeeADO;
                        if (row != null)
                        {
                            selectedEmployees[row.ID] = row.IsSelected;
                        }

                        // Tính lại trạng thái header: tất cả dòng active đều được tick?
                        bool allActiveChecked = data
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            .All(o => o.IsSelected);

                        isCheckAllEmployee = allActiveChecked;

                        gridViewFormList.InvalidateColumnHeader(gridViewFormList.Columns["IsSelected"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOtherService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    // Clear selection trong GridCheckMarksSelection
                    var gridCheckMark = cboOtherService.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboOtherService.Properties.View);
                    }

                    // Xóa text hiển thị
                    cboOtherService.EditValue = null;
                    cboOtherService.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOtherService_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                var gridCheckMark = cboOtherService.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark == null)
                {
                    e.DisplayText = string.Empty;
                    return;
                }

                var codes = new List<string>();
                foreach (object item in gridCheckMark.Selection)
                {
                    HIS_SERVICE s = item as HIS_SERVICE;
                    if (s != null && !string.IsNullOrEmpty(s.SERVICE_CODE))
                    {
                        codes.Add(s.SERVICE_CODE);
                    }
                }

                // Hiển thị: CODE1, CODE2, CODE3
                e.DisplayText = string.Join(", ", codes);
                cboOtherService.ToolTip = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCGKT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    // Clear selection trong GridCheckMarksSelection
                    var gridCheckMark = cboCGKT.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboCGKT.Properties.View);
                    }

                    // Xóa text hiển thị
                    cboCGKT.EditValue = null;
                    cboCGKT.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentTT12_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboDepartmentTT12.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDepartmentTT12.Properties.View);
                    }
                    cboDepartmentTT12.EditValue = null;
                    cboDepartmentTT12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentTT12_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string departmentName = "";
                if (this.departmentSeleteds != null && this.departmentSeleteds.Count > 0)
                {
                    foreach (var item in this.departmentSeleteds)
                    {
                        departmentName += item.DEPARTMENT_CODE + ";";
                    }
                }
                e.DisplayText = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartmentTT12_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                // Khi dropdown đóng, đồng bộ lại giá trị từ grid selection
                GridCheckMarksSelection gridCheckMark = cboDepartmentTT12.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.Selection.Count > 0)
                {
                    List<HIS_DEPARTMENT> sgSelectedNews = new List<HIS_DEPARTMENT>();
                    StringBuilder sb = new StringBuilder();
                    
                    foreach (HIS_DEPARTMENT item in gridCheckMark.Selection)
                    {
                        if (item != null)
                        {
                            if (sb.Length > 0)
                                sb.Append(";");
                            sb.Append(item.DEPARTMENT_CODE);
                            sgSelectedNews.Add(item);
                        }
                    }
                    
                    this.departmentSeleteds = sgSelectedNews;
                    cboDepartmentTT12.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
