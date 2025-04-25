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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using SDA.EFMODEL.DataModels;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using DevExpress.XtraExport;

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
        #endregion
        List<HIS_MEDI_STOCK> mediStockSeleteds;
        string[] mediStockNew;
        List<HIS_SPECIALITY> specialitySeleteds;
        string[] specialityNew;
        List<HIS_MEDI_ORG> mediOrgSeleteds;
        string[] mediOrgNew;
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
            if(!string.IsNullOrEmpty(txtEmployeeCode.Text.Trim()))
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
            if(!string.IsNullOrEmpty(txtDiplomaPlace.Text.Trim()))
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
                updateDTOEmployee.SPECIALITY_CODES = string.Join(";", specialitySeleteds.Select(o=>o.SPECIALITY_CODE).ToList());
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

            if (chkIsNeedSignInstead.Checked == true)
            {
                updateDTOEmployee.IS_NEED_SIGN_INSTEAD = 1;
            }
            else
            {
                updateDTOEmployee.IS_NEED_SIGN_INSTEAD = null;
            }
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

                    List<string> displayText= new List<string>();
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
                    SetValueSpeciality(this.cboSpecialityCodes, this.specialitySeleteds, BackendDataWorker.Get<HIS_SPECIALITY>());
                    filldatatocboSpeciality(currentDataEmp);
                    cboTypeOfTime.EditValue = currentDataEmp.TYPE_OF_TIME;
                    cboBranch.EditValue = currentDataEmp.BRANCH_ID;
                    SetValueMediOrgCodes(this.cboMediOrgCodes, this.mediOrgSeleteds, BackendDataWorker.Get<HIS_MEDI_ORG>());
                    filldatatocboMediOrgCodes(currentDataEmp);

                }
                txtLoginName.Focus();
            }
            catch (Exception e)
            {
                Inventec.Common.Logging.LogSystem.Warn(e);
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

                //HIS_EMPLOYEE data
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>> apiResultEmployee = null;

                apiResultEmployee = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                      (HisRequestUriStore.HIS_EMPLOYEE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                gridViewFormList.GridControl.DataSource = null;
                if (apiResultEmployee != null)
                {
                    var dataEmployee = (List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>)apiResultEmployee.Data;
                    //gan du lieu len gridview
                    gridViewFormList.GridControl.DataSource = dataEmployee;

                    rowCount = (dataEmployee == null ? 0 : dataEmployee.Count);
                    dataTotal = (apiResultEmployee.Param == null ? 0 : apiResultEmployee.Param.Count ?? 0);
                }
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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

                throw;
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

                throw;
            }
        }

        private async Task LoadPosition()
        {
            try
            {
                lstPosition = new List<ObjectCombo>() {
                    new ObjectCombo() { id = 1, name = "Người chịu trách nhiệm chuyên môn" },
                    new ObjectCombo() { id = 2, name = "Trưởng khoa" },
                    new ObjectCombo() { id = 3, name = "Người chịu trách nhiệm chuyên môn kiêm Trưởng khoa" }
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
                lstCareer = BackendDataWorker.Get<HIS_CAREER_TITLE>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCareer), lstCareer));
                List <ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_TITLE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("CAREER_TITLE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_TITLE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCareerTitle, lstCareer, controlEditorADO);
                cboCareerTitle.Properties.ImmediatePopup = true;
                cboCareerTitle.Properties.PopupFormSize = new Size(350, cboCareerTitle.Properties.PopupFormSize.Height);
                if(cboCareerTitle.EditValue != null && (lstCareer == null || lstCareer.FirstOrDefault(o=>o.ID == Int64.Parse(cboCareerTitle.EditValue.ToString())) == null))
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

                throw;
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
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? btnUnlock : btnLock;
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
                HIS_EMPLOYEE data = (HIS_EMPLOYEE)gridViewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;


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

        private void gridViewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    WaitingManager.Show();
                }
                else
                {
                    var rowData = (HIS_EMPLOYEE)gridViewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
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
                if(e.Button.Kind == ButtonPredefines.Delete)
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
                var apiResultEmployee = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                      (HisRequestUriStore.HIS_EMPLOYEE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if(apiResultEmployee != null && apiResultEmployee.Count > 0)
                {
                    apiResultEmployee = apiResultEmployee.Where(o => !string.IsNullOrEmpty(o.DIPLOMA)).ToList();
                }
                if (apiResultEmployee == null || apiResultEmployee.Count == 0)
                    return;
                List<XML> lstXml = new List<XML>();
                int count = 1;
                foreach (var currentData in apiResultEmployee)
                {
                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == currentData.BRANCH_ID);

                    XML xml = new XML();
                    xml.STT = count++;
                    xml.MA_CSKCB = branch != null ? branch.HEIN_MEDI_ORG_CODE : "";
                    xml.HO_TEN = this.ConvertStringToXmlDocument(currentData.TDL_USERNAME ?? "");
                    xml.GIOI_TINH = currentData.GENDER_ID == 1 ? "2" : (currentData.GENDER_ID == 2 ? "1" : "3");
                    xml.MA_DANTOC = currentData.ETHNIC_CODE;
                    xml.NGAY_SINH = currentData.DOB != null ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentData.DOB ?? 0) ?? DateTime.MinValue).ToString("yyyyMMdd") : "";
                    xml.SO_CCCD = currentData.IDENTIFICATION_NUMBER ?? currentData.SOCIAL_INSURANCE_NUMBER ?? "";
                    xml.CHUCDANH_NN = "";
                    if (lstCareer != null && lstCareer.Count > 0)
                        xml.CHUCDANH_NN = currentData.CAREER_TITLE_ID != null ? (lstCareer.FirstOrDefault(o=> o.ID == currentData.CAREER_TITLE_ID.Value) != null ? lstCareer.FirstOrDefault(o => o.ID == currentData.CAREER_TITLE_ID.Value).CAREER_TITLE_CODE : "") : "";
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
                bool Sucess = CreatedXmlFile(lstXml, false, true, path);
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
    }
}
