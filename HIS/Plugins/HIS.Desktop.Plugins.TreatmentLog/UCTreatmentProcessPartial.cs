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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using DevExpress.Data;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.TreatmentLog.Resources;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.TreatmentLog.Config;
using HIS.Desktop.Plugins.TreatmentLog.Popup.CoTreatment;
using Inventec.Desktop.Common.Modules;

namespace HIS.Desktop.Plugins.TreatmentLog
{
    public partial class UCTreatmentProcessPartial : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        internal long currentTreatmentId = 0;
        long currentRoomId;
        long logTypeId__DepartmentTran = 0;
        long logTypeId__patientTypeAlter = 0;
        V_HIS_ROOM currentRoom = null;
        internal V_HIS_TREATMENT_4 currentTreatment = new V_HIS_TREATMENT_4();
        internal List<V_HIS_DEPARTMENT_TRAN> resultDepartmentTran = new List<V_HIS_DEPARTMENT_TRAN>();
        internal List<V_HIS_PATIENT_TYPE_ALTER> resultPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
        internal List<V_HIS_CO_TREATMENT> resultCoTreatment = new List<V_HIS_CO_TREATMENT>();
        internal List<PatientTypeDepartmentADO> apiResult { get; set; }
        string loginName;
        internal const string BtnEdit = "HIS000036";
        internal const string BtnDelete = "HIS000039";
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        public enum TypeEnum
        {
            typeOne = 1,
            typeTwo = 2,
            typeThree = 3,
        }
        Dictionary<long, List<object>> diction = null;
        List<HIS_DEPARTMENT> Departments { get; set; }
        HIS_EMPLOYEE HisEmployee { get; set; }
        public UCTreatmentProcessPartial(Inventec.Desktop.Common.Modules.Module _module, long _treatmentId, long _currentRoomId)
        {
            InitializeComponent();
            this.currentModule = _module;
            this.currentTreatmentId = _treatmentId;
            this.currentRoomId = _currentRoomId;
        }

        private void UCTreatmentProcessPartial_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SetCaptionByLanguageKey();
                Init();
                MeShow();
                GetControlAcs();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                }
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentLog.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentLog.UCTreatmentProcessPartial).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.treeListColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepartmentTran.Text = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.btnDepartmentTran.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPatientTypeAlter.Text = Inventec.Common.Resource.Get.Value("UCTreatmentProcessPartial.btnPatientTypeAlter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async void LoadDataToGridTreatmentLog(V_HIS_TREATMENT_4 treatment)
        {
            try
            {
                if (treatment != null)
                {
                    apiResult = new List<PatientTypeDepartmentADO>();
                    var apiResultNotNull = new List<PatientTypeDepartmentADO>();
                    var apiResultNull = new List<PatientTypeDepartmentADO>();
                    var apiResultNotOrder = new List<PatientTypeDepartmentADO>();
                    var apiResultGroup = new List<PatientTypeDepartmentADO>();

                    CommonParam param = new CommonParam();
                    //Get DepartmentTran
                    HisDepartmentTranViewFilter departmentfilter = new HisDepartmentTranViewFilter();
                    diction = new Dictionary<long, List<object>>();
                    departmentfilter.TREATMENT_ID = treatment.ID;
                    departmentfilter.ORDER_FIELD = "MODIFY_TIME";
                    departmentfilter.ORDER_DIRECTION = "DESC";
                    resultDepartmentTran = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, departmentfilter, param);
                    if (resultDepartmentTran != null && resultDepartmentTran.Count > 0)
                    {
                        foreach (var item in resultDepartmentTran)
                        {
                            if (!diction.ContainsKey(item.DEPARTMENT_IN_TIME ?? 0))
                            {
                                diction[item.DEPARTMENT_IN_TIME ?? 0] = new List<object>();
                            }
                            diction[item.DEPARTMENT_IN_TIME ?? 0].Add(item);
                        }
                    }
                    //Get PatientTypeAlter
                    HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                    patientTypeFilter.TREATMENT_ID = treatment.ID;
                    patientTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                    patientTypeFilter.ORDER_DIRECTION = "DESC";
                    resultPatientTypeAlter = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_VIEW, ApiConsumers.MosConsumer, patientTypeFilter, param);
                    if (resultPatientTypeAlter != null && resultPatientTypeAlter.Count > 0)
                    {
                        foreach (var item in resultPatientTypeAlter)
                        {
                            if (!diction.ContainsKey(item.LOG_TIME))
                            {
                                diction[item.LOG_TIME] = new List<object>();
                            }
                            diction[item.LOG_TIME].Add(item);
                        }
                    }

                    //Get HisCOTreatment
                    if (resultDepartmentTran != null && resultDepartmentTran.Count > 0)
                    {
                        HisCoTreatmentViewFilter coTreatmentFilter = new HisCoTreatmentViewFilter();
                        coTreatmentFilter.DEPARTMENT_TRAN_IDs = resultDepartmentTran.Select(o => o.ID).ToList();
                        coTreatmentFilter.ORDER_FIELD = "MODIFY_TIME";
                        coTreatmentFilter.ORDER_DIRECTION = "DESC";

                        resultCoTreatment = await new BackendAdapter(param).GetAsync<List<V_HIS_CO_TREATMENT>>("api/HisCoTreatment/GetView", ApiConsumers.MosConsumer, coTreatmentFilter, param);
                        if (resultCoTreatment != null && resultCoTreatment.Count > 0)
                        {
                            foreach (var item in resultCoTreatment)
                            {
                                if (!diction.ContainsKey(item.CREATE_TIME ?? 0))
                                {
                                    diction[item.CREATE_TIME ?? 0] = new List<object>();
                                }
                                diction[item.CREATE_TIME ?? 0].Add(item);
                            }
                        }
                    }

                    diction = diction.OrderByDescending(o => o.Key).ToDictionary(p => p.Key, p => p.Value);

                    if (diction != null && diction.Count > 0)
                    {
                        foreach (var dic in diction)
                        {
                            foreach (var item in dic.Value)
                            {
                                PatientTypeDepartmentADO a = new ADO.PatientTypeDepartmentADO();
                                if (item.GetType() == typeof(V_HIS_PATIENT_TYPE_ALTER))
                                {
                                    a.patientTypeAlter = (V_HIS_PATIENT_TYPE_ALTER)item;
                                    a.CREATE_TIME = a.patientTypeAlter.CREATE_TIME;
                                    a.LOG_TIME = a.patientTypeAlter.LOG_TIME;
                                    a.MODIFY_TIME = a.patientTypeAlter.MODIFY_TIME;
                                    a.TREATMENT_ID = a.patientTypeAlter.TREATMENT_ID;
                                    a.CREATOR = a.patientTypeAlter.CREATOR;
                                    a.MODIFIER = a.patientTypeAlter.MODIFIER;
                                    a.type = (long)TypeEnum.typeOne;
                                    a.Id = a.patientTypeAlter.DEPARTMENT_TRAN_ID.ToString();
                                }
                                else if (item.GetType() == typeof(V_HIS_DEPARTMENT_TRAN))
                                {
                                    a.departmentTran = (V_HIS_DEPARTMENT_TRAN)item;
                                    a.CREATE_TIME = a.departmentTran.CREATE_TIME;
                                    a.LOG_TIME = a.departmentTran.DEPARTMENT_IN_TIME ?? 0;
                                    a.MODIFY_TIME = a.departmentTran.MODIFY_TIME;
                                    a.TREATMENT_ID = a.departmentTran.TREATMENT_ID;
                                    a.CREATOR = a.departmentTran.CREATOR;
                                    a.MODIFIER = a.departmentTran.MODIFIER;
                                    a.type = (long)TypeEnum.typeTwo;
                                    a.Id = a.departmentTran.ID.ToString();
                                    a.IsActiveDepartmentTran = a.departmentTran.IS_ACTIVE;
                                }
                                else if (item.GetType() == typeof(V_HIS_CO_TREATMENT))
                                {
                                    a.coTreatment = (V_HIS_CO_TREATMENT)item;
                                    a.CREATE_TIME = a.coTreatment.CREATE_TIME;
                                    a.LOG_TIME = a.coTreatment.CREATE_TIME ?? 0;
                                    a.MODIFY_TIME = a.coTreatment.MODIFY_TIME;
                                    a.TREATMENT_ID = a.coTreatment.TDL_TREATMENT_ID;
                                    a.CREATOR = a.coTreatment.CREATOR;
                                    a.MODIFIER = a.coTreatment.MODIFIER;
                                    a.type = (long)TypeEnum.typeThree;
                                    a.Id = a.coTreatment.DEPARTMENT_TRAN_ID.ToString();
                                }

                                a.IS_ACTIVE = treatment.IS_ACTIVE;
                                apiResultNotOrder.Add(a);
                            }
                        }

                        var group = apiResultNotOrder.GroupBy(o => o.Id).ToList();
                        foreach (var item in group)
                        {
                            var listById = item.ToList<PatientTypeDepartmentADO>();
                            var patientAdo = new PatientTypeDepartmentADO();
                            patientAdo = listById.FirstOrDefault(o => o.type == (long)TypeEnum.typeTwo);
                            if (patientAdo != null)
                            {
                                apiResultGroup.Add(patientAdo);
                            }
                            int dem = 0;
                            foreach (var adoItem in listById)
                            {
                                if (adoItem.type == (long)TypeEnum.typeOne || adoItem.type == (long)TypeEnum.typeThree)
                                {
                                    dem++;
                                    adoItem.Id = patientAdo.Id + "_" + dem;
                                    adoItem.ParentId = patientAdo.Id;
                                    apiResultGroup.Add(adoItem);
                                }
                            }
                        }
                        apiResultNotNull = apiResultGroup.Where(o => o.LOG_TIME > 0).ToList();
                        apiResultNull = apiResultGroup.Where(o => o.LOG_TIME == 0).ToList();

                        apiResultNull = apiResultNull.OrderBy(o => o.CREATE_TIME).ThenBy(p => p.Id).ToList();
                        apiResultNotNull = apiResultNotNull.OrderBy(o => o.LOG_TIME).ThenBy(p => p.Id).ToList();

                        apiResult.AddRange(apiResultNotNull);
                        apiResult.AddRange(apiResultNull);

                        treeListTreatmentLog.BeginUpdate();
                        treeListTreatmentLog.KeyFieldName = "Id";
                        treeListTreatmentLog.ParentFieldName = "ParentId";
                        treeListTreatmentLog.DataSource = apiResult;
                        treeListTreatmentLog.ExpandAll();
                        treeListTreatmentLog.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void Init()
        {
            CommonParam param = new CommonParam();
            try
            {
                currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentRoomId);
                Departments = BackendDataWorker.Get<HIS_DEPARTMENT>().ToList();
                HisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == loginName);
                logTypeId__DepartmentTran = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_LOG_TYPE.ID__MKDT;  /*DEPARTmenttran*/
                logTypeId__patientTypeAlter = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_LOG_TYPE.ID__KDT; /*PATIENT_TYPE_ALTER*/
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.ID = currentTreatmentId;
                currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, filter, param).ToList().FirstOrDefault();

                if (this.currentTreatment != null)
                {
                    LoadDataToGridTreatmentLog(this.currentTreatment);
                    SetEnableButton();
                }
                else
                {
                    treeListTreatmentLog.DataSource = null;
                    btnDepartmentTran.Enabled = false;
                    btnPatientTypeAlter.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButton()
        {
            try
            {
                if (HisConfigCFG.CASHIER_ROOM == "1")
                {
                    btnPatientTypeAlter.Enabled = ((HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment) || this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN)) && IsTreatmentPause();
                }
                else
                {
                    btnPatientTypeAlter.Enabled = ((HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment))) && IsTreatmentPause();
                }

                btnDepartmentTran.Enabled = (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment)) && IsTreatmentPause();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            CommonParam param = new CommonParam();
            try
            {
                logTypeId__DepartmentTran = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_LOG_TYPE.ID__MKDT; /*DEPARTMENT_TRAN*/
                logTypeId__patientTypeAlter = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_LOG_TYPE.ID__KDT; /*PATIENT_TYPE_ALTER*/
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var treatmentLog = (PatientTypeDepartmentADO)treeListTreatmentLog.GetDataRecordByNode(treeListTreatmentLog.FocusedNode);
                if (treatmentLog != null)
                {
                    bool isView = false;
                    if (treatmentLog.type == (long)TypeEnum.typeTwo)
                    {
                        isView = (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment));

                        if ((!isView || IsTreatmentPause(this.currentTreatment)) || treatmentLog.departmentTran.DEPARTMENT_IN_TIME == null)
                        {
                            isView = false;
                        }
                        List<object> listArgs = new List<object>();
                        TransDepartmentADO transDepartmenADO = new TransDepartmentADO(currentTreatment.ID);
                        transDepartmenADO.TreatmentId = currentTreatment.ID;
                        transDepartmenADO.DepartmentId = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentRoomId).First().DEPARTMENT_ID;
                        listArgs.Add(treatmentLog.departmentTran);
                        listArgs.Add(this.currentRoomId);
                        listArgs.Add(transDepartmenADO);
                        listArgs.Add(isView);
                        listArgs.Add((RefeshReference)Init);
                        CallModule callModule = new CallModule(CallModule.TransDepartment, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                    else if (treatmentLog.type == (long)TypeEnum.typeOne)
                    {
                        if (HisConfigCFG.CASHIER_ROOM == "1")
                        {
                            isView = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment) || this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN;
                        }
                        else
                        {
                            isView = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment);
                        }
                        if (!isView || IsTreatmentPause(this.currentTreatment))
                        {
                            isView = false;
                        }
                        treatmentLog.IS_ACTIVE = isView ? (short)1 : (short)0;
                        try
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(treatmentLog);
                            listArgs.Add(this.currentModule);
                            listArgs.Add(isView);
                            listArgs.Add(apiResult);
                            listArgs.Add((RefeshReference)Init);
                            CallModule callModule = new CallModule(CallModule.CallPatientTypeAlter, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (treatmentLog.type == (long)TypeEnum.typeThree)
                    {
                        frmCoTreatment frm = new frmCoTreatment(treatmentLog.coTreatment, (HIS.Desktop.Common.DelegateRefreshData)Init, currentModule);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool IsViewForm1(V_HIS_TREATMENT_4 data)
        {
            bool result = false;
            try
            {
                if (!IsTreatmentPause(data) && IsStayingDepartment(data))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        bool IsTreatmentPause(V_HIS_TREATMENT_4 data)
        {
            bool result = false;
            try
            {
                result = (data.IS_PAUSE == 1 && data.IS_ACTIVE == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        bool IsStayingDepartment(V_HIS_TREATMENT_4 data)
        {
            bool result = false;
            try
            {
                result = (NowDepartmentOfTreatment(data) == BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentModule.RoomId).Select(p => p.DEPARTMENT_ID).First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        long NowDepartmentOfTreatment(V_HIS_TREATMENT_4 data)
        {
            long result = 0;
            string strResult = "";
            List<string> DepartmentIds = data.DEPARTMENT_IDS.Split(',').ToList();
            strResult = DepartmentIds[DepartmentIds.Count - 1];
            DepartmentIds = strResult.Split('_').ToList();
            strResult = DepartmentIds[0];
            result = Convert.ToInt64(strResult);
            return result;

        }
        bool IsViewForm(V_HIS_DEPARTMENT_TRAN data)
        {
            bool result = false;
            try
            {
                if (
                     IsTreatmentPause() && IsDepartmentOut(data))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsViewFormButton()
        {
            bool result = false;
            try
            {
                if (
                     IsTreatmentPause() && DepartmentInTimeButton() && IsStayingDepartment(this.currentTreatment))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool DepartmentInTimeDelete(V_HIS_DEPARTMENT_TRAN data)
        {
            bool result = false;
            try
            {
                var departmentIntime = apiResult.Where(o => o.type == (long)TypeEnum.typeTwo).Select(o => o.departmentTran).ToList();
                if (departmentIntime != null && departmentIntime.Select(o => o.DEPARTMENT_IN_TIME).Contains(null) && data.DEPARTMENT_IN_TIME == null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return result;
        }

        bool DepartmentInTimeButton()
        {
            bool result = false;
            try
            {
                var departmentIntime = apiResult.Where(o => o.type == (long)TypeEnum.typeTwo).Select(o => o.departmentTran).ToList();
                if (departmentIntime != null)
                {
                    if (departmentIntime.Count == 1)
                    {
                        var department = departmentIntime.FirstOrDefault();
                        if (department != null && department.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                            return result = true;
                    }

                    if (departmentIntime.Select(o => o.DEPARTMENT_IN_TIME).Contains(null))
                        result = false;
                    else
                    {
                        var departmentTranLast = departmentIntime.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                        if (departmentTranLast != null)
                        {
                            if (departmentTranLast != null && departmentTranLast.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return result;
        }

        bool IsTreatmentPause()
        {
            bool result = false;
            try
            {
                result = !(this.currentTreatment.IS_PAUSE == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsDepartmentOut(V_HIS_DEPARTMENT_TRAN data)
        {
            bool result = false;
            try
            {
                var departmentIntime = apiResult.Where(o => o.type == (long)TypeEnum.typeTwo).Select(o => o.departmentTran).ToList();
                if (departmentIntime != null)
                {
                    if (departmentIntime.Count == 1)
                    {
                        var department = departmentIntime.FirstOrDefault();
                        if (department != null && department.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                            result = true;
                    }
                    else
                    {
                        if (data.DEPARTMENT_IN_TIME == null)
                        {
                            var department = departmentIntime.FirstOrDefault(o => o.ID == data.PREVIOUS_ID);
                            if (department != null)
                            {
                                if (department != null && department.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            if (departmentIntime.Select(o => o.DEPARTMENT_IN_TIME).Contains(null))
                            {
                                result = false;
                            }
                            else
                            {
                                var departmentTranLast = departmentIntime.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                                if (departmentTranLast != null)
                                {
                                    if (departmentTranLast != null && departmentTranLast.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                                    {
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        string GetDetailHeinInfo(PatientTypeDepartmentADO data)
        {
            StringBuilder info = new StringBuilder();
            try
            {
                if (data.patientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    if (data.patientTypeAlter.HEIN_CARD_NUMBER != null)
                    {
                        info.Append(". Số thẻ BHYT: " + SetHeinCardNumberDisplayByNumber(SetHeinCardNumberDisplayByNumber(data.patientTypeAlter.HEIN_CARD_NUMBER)) + ". Thời hạn: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return info.ToString();
        }
        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        private void btnPatientTypeAlter_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)Init);
                listArgs.Add(true);
                listArgs.Add(currentTreatmentId);
                listArgs.Add(apiResult);
                CallModule callModule = new CallModule(CallModule.CallPatientTypeAlter, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepartmentTran_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                TransDepartmentADO transDepartmenADO = new TransDepartmentADO(currentTreatment.ID);
                transDepartmenADO.TreatmentId = currentTreatment.ID;
                transDepartmenADO.DepartmentId = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentRoomId).First().DEPARTMENT_ID;
                listArgs.Add(this.currentRoomId);
                listArgs.Add(transDepartmenADO);
                listArgs.Add((RefeshReference)Init);
                CallModule callModule = new CallModule(CallModule.TransDepartment, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            bool notHandler = false;
            try
            {
                long treatmentId = this.currentTreatmentId;
                var dataTreatmentLog = (PatientTypeDepartmentADO)treeListTreatmentLog.GetDataRecordByNode(treeListTreatmentLog.FocusedNode);

                if (dataTreatmentLog != null)
                {
                    if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (dataTreatmentLog.type == (long)TypeEnum.typeTwo)
                        {
                            WaitingManager.Show();
                            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TREATMENT_LOG_DELETE_DEPARTMENT, ApiConsumers.MosConsumer, dataTreatmentLog.departmentTran.ID, null);

                            if (success)
                            {
                                Init();
                            }
                            WaitingManager.Hide();
                        }
                        else if (dataTreatmentLog.type == (long)TypeEnum.typeOne)
                        {
                            DeletePatientTypeAlterSDO sdo = new DeletePatientTypeAlterSDO();
                            sdo.PatientTypeAlterId = dataTreatmentLog.patientTypeAlter.ID;
                            sdo.RequestRoomId = this.currentModule.RoomId;

                            if (this.resultDepartmentTran != null && this.resultDepartmentTran.Count > 0)
                            {
                                var ntgt = resultPatientTypeAlter.Where(o => (o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                                    || o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)).ToList();
                                if (ntgt != null && ntgt.Count > 0)
                                {
                                    var min = ntgt.Where(o => o.LOG_TIME != null && o.LOG_TIME == ntgt.Min(p => p.LOG_TIME)).OrderBy(m => m.LOG_TIME).ToList();
                                    if ((
                                        (min.Count == 1 && min.FirstOrDefault().ID == dataTreatmentLog.patientTypeAlter.ID)
                                        || (min.Count > 1 && min.FirstOrDefault(o => o.ID == min.Min(p => p.ID)).ID == dataTreatmentLog.patientTypeAlter.ID)
                                        )
                                        && this.currentTreatment != null && !string.IsNullOrEmpty(this.currentTreatment.IN_CODE)
                                        && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.BenhNhanDaDuocCapSoVaoVien, this.currentTreatment.IN_CODE), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes
                                        )
                                    {
                                        sdo.IsDeleteInCode = true;
                                    }
                                    else
                                    {
                                        sdo.IsDeleteInCode = false;
                                    }
                                }
                            }

                            WaitingManager.Show();
                            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TREATMENT_LOG_DELETE_PATIENT_TYPE_ALTER, ApiConsumers.MosConsumer, sdo, param);

                            if (success)
                            {
                                Init();
                            }
                            WaitingManager.Hide();
                        }
                        else if (dataTreatmentLog.type == (long)TypeEnum.typeThree)
                        {
                            WaitingManager.Show();
                            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisCoTreatment/Delete", ApiConsumers.MosConsumer, dataTreatmentLog.coTreatment.ID, param);
                            WaitingManager.Hide();
                            if (success)
                            {
                                Init();
                            }
                            WaitingManager.Hide();
                        }
                    }
                    else
                    {
                        notHandler = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }

            if (!notHandler)
            {
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            SetEnableButton();
        }

        private void gridControlTreatment_Click(object sender, EventArgs e)
        {

        }

        private bool CheckHasSoon(PatientTypeDepartmentADO data)
        {
            bool rs = false;
            try
            {
                rs = apiResult.Exists(o => (o.type == (long)TypeEnum.typeOne) && o.ParentId == data.Id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return rs = false;
            }
            return rs;
        }

        private void treeListTreatmentLog_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var rowData = treeListTreatmentLog.GetDataRecordByNode(e.Node);
                if (rowData != null && rowData is PatientTypeDepartmentADO)
                {
                    PatientTypeDepartmentADO data = rowData as PatientTypeDepartmentADO;
                    bool isDisableDelButton = false;
                    if (e.Column.FieldName == "Delete")
                    {
                        if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == BtnDelete) != null)
                        {
                            if (data.type != 3)
                            {
                                isDisableDelButton = ((HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(this.currentTreatment))
                                    && !CheckHasSoon(data));

                                e.RepositoryItem = isDisableDelButton ? Btn_Delete_Enable : Btn_Delete_Disable;
                            }
                            else
                            {
                                if (data.coTreatment.START_TIME == null)
                                {
                                    if (IsStayingDepartment(this.currentTreatment))
                                    {
                                        e.RepositoryItem = Btn_Delete_Enable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_Delete_Disable;
                                    }
                                }
                                else
                                {
                                    if (IsStayingDepartment(this.currentTreatment))
                                    {
                                        e.RepositoryItem = Btn_Delete_Disable;
                                    }
                                    else if ((data.coTreatment.DEPARTMENT_ID == BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentModule.RoomId).Select(p => p.DEPARTMENT_ID).First()))
                                    {
                                        e.RepositoryItem = Btn_Delete_Enable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_Delete_Disable;
                                    }
                                }
                            }
                        }
                        else
                        {
                            e.RepositoryItem = Btn_Delete_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "Lock")
                    {
                        if (data.type == (long)TypeEnum.typeTwo)
                        {
                            var department = Departments.FirstOrDefault(o => o.ID == data.departmentTran.DEPARTMENT_ID);
                            bool IsEnable = (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || (currentRoom.DEPARTMENT_ID == department.ID)) && department.IS_CLINICAL == 1;
                            if (data.IsActiveDepartmentTran.HasValue)
                            {
                                if (IsEnable)
                                {
                                    e.RepositoryItem = data.IsActiveDepartmentTran == 1 ? repActiveEnable : repActiveDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = data.IsActiveDepartmentTran == 1 ? repActEGray : repActDGray;
                                }
                            }
                        }
                    }
                    else if (e.Column.FieldName == "Edit")
                    {
                        if ((HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) ||
                            IsStayingDepartment(this.currentTreatment) ||
                            (this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN) && HisConfigCFG.CASHIER_ROOM == "1")
                            && (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == BtnEdit) != null))
                        {
                            e.RepositoryItem = Btn_Edit_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_Edit_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListTreatmentLog_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    PatientTypeDepartmentADO data = (PatientTypeDepartmentADO)e.Row;

                    if (data != null)
                    {
                        if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64((data.CREATE_TIME ?? 0).ToString()));

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64((data.MODIFY_TIME ?? 0).ToString()));

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "TREATMENT_LOG_TYPE_STR")
                        {
                            e.Value = data.type;
                            if (data.type == (long)TypeEnum.typeOne)
                            {
                                string primary = "";
                                if (HisConfigCFG.IsSetPrimaryPatientType == "2" && data.patientTypeAlter.PRIMARY_PATIENT_TYPE_ID.HasValue)
                                {
                                    var paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.patientTypeAlter.PRIMARY_PATIENT_TYPE_ID.Value);
                                    primary = paty != null ? " (Đối tượng phụ thu: " + paty.PATIENT_TYPE_NAME + ")" : null;

                                }
                                if (data.patientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    e.Value = data.patientTypeAlter.PATIENT_TYPE_NAME + " - " + data.patientTypeAlter.HEIN_CARD_NUMBER + primary;
                                else
                                    e.Value = data.patientTypeAlter.PATIENT_TYPE_NAME;
                            }
                            else if (data.type == (long)TypeEnum.typeTwo)
                            {
                                e.Value = data.departmentTran.DEPARTMENT_NAME;
                            }
                            else
                            {
                                e.Value = "Điều trị kết hợp ( " + data.coTreatment.DEPARTMENT_NAME + " )";
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            if (data.type == (long)TypeEnum.typeThree)
                            {
                                if (data.coTreatment.START_TIME == null)
                                    e.Value = "";
                                else
                                {
                                    if (data.coTreatment.FINISH_TIME == null)
                                    {
                                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.START_TIME ?? 0);
                                    }
                                    else
                                    {
                                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.START_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.FINISH_TIME ?? 0);
                                    }
                                }
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DETAIL_DATA")
                        {
                            if (data.patientTypeAlter != null)
                            {
                                if (data.patientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    e.Value = data.patientTypeAlter.TREATMENT_TYPE_NAME + ". " + data.patientTypeAlter.HEIN_CARD_NUMBER + " (" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) + ")";
                                else
                                    e.Value = data.patientTypeAlter.TREATMENT_TYPE_NAME + ". ";
                            }
                        }
                        else if (e.Column.FieldName == "LOG_TIME_DISPLAY")
                        {
                            if (data.type == (long)TypeEnum.typeThree)
                            {
                                if (data.coTreatment.START_TIME == null)
                                    e.Value = "";
                                else
                                {
                                    if (data.coTreatment.FINISH_TIME == null)
                                    {
                                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.START_TIME ?? 0);
                                    }
                                    else
                                    {
                                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.START_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.coTreatment.FINISH_TIME ?? 0);
                                    }
                                }
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.LOG_TIME);
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

        private void repActiveEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_DEPARTMENT_TRAN success = null;
            bool notHandler = false;
            try
            {
                long treatmentId = this.currentTreatmentId;
                var dataTreatmentLog = (PatientTypeDepartmentADO)treeListTreatmentLog.GetDataRecordByNode(treeListTreatmentLog.FocusedNode);

                if (dataTreatmentLog != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bạn có muốn thực hiện chốt chi phí khoa {0} không", dataTreatmentLog.departmentTran.DEPARTMENT_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        HIS_DEPARTMENT_TRAN coTreatment = new HIS_DEPARTMENT_TRAN();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT_TRAN>(coTreatment, dataTreatmentLog.departmentTran);
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/ChangeLock", ApiConsumers.MosConsumer, coTreatment, null);

                        if (success != null)
                        {
                            Init();
                        }
                        WaitingManager.Hide();
                    }
                    else
                    {
                        notHandler = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }

            if (!notHandler)
            {
                #region Show message
                MessageManager.Show(this.ParentForm, param, success != null);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
        }

        private void repActiveDisable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_DEPARTMENT_TRAN success = null;
            try
            {
                long treatmentId = this.currentTreatmentId;
                var dataTreatmentLog = (PatientTypeDepartmentADO)treeListTreatmentLog.GetDataRecordByNode(treeListTreatmentLog.FocusedNode);

                if (dataTreatmentLog != null)
                {
                    WaitingManager.Show();
                    HIS_DEPARTMENT_TRAN coTreatment = new HIS_DEPARTMENT_TRAN();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT_TRAN>(coTreatment, dataTreatmentLog.departmentTran);
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/ChangeLock", ApiConsumers.MosConsumer, coTreatment, null);

                    if (success != null)
                    {
                        Init();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
            #region Show message
            MessageManager.Show(this.ParentForm, param, success != null);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion
        }
    }
}
