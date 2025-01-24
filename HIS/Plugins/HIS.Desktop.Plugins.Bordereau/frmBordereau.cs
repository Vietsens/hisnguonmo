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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Common.TypeConvert;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.IsAdmin;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.Config;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.Bordereau.ChooseCondition;
using DevExpress.Utils;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        internal long treatmentId;
        internal V_HIS_TREATMENT currentTreatment { get; set; }
        internal List<HIS_SERE_SERV_DEPOSIT> sereServDeposits { get; set; }
        internal long currentDepartmentId { get; set; }
        internal HIS.Desktop.Plugins.Bordereau.Base.DelegateWorker.UpdatePatientType updatePatientType;
        internal List<V_HIS_PATIENT_TYPE_ALTER> currentHisPatientTypeAlters = null;
        internal List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter { get; set; }
        internal List<HIS_SERVICE_TYPE> ServiceTypes { get; set; }
        internal List<HIS_DEPARTMENT> Departments { get; set; }
        internal List<V_HIS_SERVICE> Services { get; set; }
        internal List<HIS_PATIENT_TYPE> PatientTypes { get; set; }
        internal List<SereServADO> sereServIsPackages { get; set; }
        internal List<HIS_OTHER_PAY_SOURCE> OtherPaySources { get; set; }
        internal List<HIS_PACKAGE> packages { get; set; }

        internal V_HIS_PATIENT patient { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        internal List<V_HIS_ROOM> rooms { get; set; }
        internal long totalDayTreatment = 0;
        internal string userNameReturnResult { get; set; }
        internal string statusTreatmentOut { get; set; }
        internal List<SereServADO> SereServADOs { get; set; }
        internal List<HIS_SERE_SERV_BILL> SereServBills { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        internal List<HIS_DEPARTMENT> DepartmentPremissionEdits { get; set; }
        HIS_DEPARTMENT currentDepartment = null;
        internal long? equipmentId;
        internal long? numOrder;
        internal PrintOption.PayType payOption = PrintOption.PayType.ALL;

        public string AllowAssignOffListMedicineMaterialHeinCardNumberPrefix { get; private set; }

        bool AllowCheckIsNoExecute = false;
        bool AutoClosePrintAndForm = false;
        string cboPayTypeDefault = null;
        bool IsSetPrimaryPatientType = false;
        bool IsAllowNoExecuteForPaid = false;

        /// <summary>
        /// Chi get ve de check trang thai xet nghiem
        /// Co cau hinh cho phep get hay khong.
        /// Muc dich hieu nang
        /// </summary>
        List<HIS_SERVICE_REQ> serviceReqs { get; set; }
        List<HIS_MEDICINE> MedicineList = new List<HIS_MEDICINE>();
        List<HIS_MATERIAL> MaterialList = new List<HIS_MATERIAL>();
        internal PopupMenu menu;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.Bordereau";

        public frmBordereau()
        {
            InitializeComponent();
        }

        public frmBordereau(Inventec.Desktop.Common.Modules.Module _module, long _treatmentId) : base(_module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = _treatmentId;
                this.currentModule = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmBordereau_Load(object sender, EventArgs e)
        {
            try
            {
                InitLanguage();
                LoadFromSdaConfig();
                VisableColumnInGrid();
                InitControlState();
                LoadCurrentTreatmentData();
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                this.gridControlBordereau.ToolTipController = this.toolTipController1;

                ServiceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>(false, true);
                //HeinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                Services = BackendDataWorker.Get<V_HIS_SERVICE>(false, true);
                PatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>(false, true);
                Departments = BackendDataWorker.Get<HIS_DEPARTMENT>(false, true);
                this.currentDepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId).DepartmentId;
                this.GetCurrentDepartment(this.currentDepartmentId);
                this.currentPatientTypeWithPatientTypeAlter = BackendDataWorker.Get<HIS_PATIENT_TYPE>(false, true);
                repositoryItemTxtReadOnly.ReadOnly = true;
                repositoryItemTxtReadOnly.Enabled = false;
                ////Cap nhat doi tuong benh nhan
                ////updatePatientType = UpdatePatientTypeProcess.UpdatePatientType;//xuandv comment k sd
                //this.LoadPeopleReturnResult();
                //this.LoadServiceReq();
                //this.LoadDataToBorderauAndPrint();
                ////this.currentPatientTypeWithPatientTypeAlter = this.PatientTypeWithPatientTypeAlter();
                //this.InitComboRespositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                //this.InitComboRespositoryPrimaryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                //this.InitComboRespositorySereServPackage();
                //this.LoadPatientInfo();
                //this.InitTreatmentFun();
                this.AddBarManager(this.barManager1);
                timer1.Enabled = true;
                timer1.Interval = 100;
                timer1.Start();
                repositoryItemCheckEditIsNoExecute_Disable.Enabled = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void LoadPicUpdateQR()
        {
            try
            {
                decimal totalDepositPrice = 0;
                if (lblTotalDepositPrice != null && !string.IsNullOrEmpty(lblTotalDepositPrice.Text))
                {
                     totalDepositPrice = Decimal.Parse(lblTotalDepositPrice.Text);
                }
                if (currentTreatment.IS_ACTIVE == 0 && totalDepositPrice > 0)
                {
                    picUpdateQR.Enabled = true;
                }
                else
                {
                    picUpdateQR.Enabled = false;
                }
            }
             catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.moduleLink);
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkAssignBlood)
                        {
                            chkAssignBlood.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetCurrentDepartment(long departmentId)
        {
            try
            {
                if (departmentId > 0)
                {
                    this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();

                LoadPeopleReturnResultV2();

                LoadServiceReqV2();

                //LoadDataToBorderauAndPrintV2();
                LoadTotalPriceDataToTestServiceReqV2();

                LoadAndFillDataToReposPatientType();

                Inventec.Common.Logging.LogSystem.Debug("LoadDataToBorderauAndPrintV2>>>>>1");
                LoadDataToBorderauAndPrintV2();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToBorderauAndPrintV2>>>>>2");

                //InitComboRespositoryPatientTypeV2(this.currentPatientTypeWithPatientTypeAlter);

                //InitComboRespositoryPrimaryPatientTypeV2(this.currentPatientTypeWithPatientTypeAlter);

                //InitComboRespositorySereServPackageV2();

                initLoadPakageTackV2();

                this.LoadPatientInfo();

                this.InitTreatmentFun();

                LoadMedicineMaterial();

                LoadComboPayType();

                LoadcboHSBA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public enum ComlumnType
        {
            NO_EXECUTE,
            EXPEND,
            EXPEND_TYPE_ID,
            IS_OUT_PARENT_FEE,
            IS_FUN_ACCEPT,
            NO_CHECK_COLUMN
        }

        private bool CheckPremissionEdit(SereServADO data, ComlumnType comlumnType, ref string mess)
        {
            bool vali = true;
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                List<long> departmentPremissionIds = DepartmentPremissionEdits != null ? DepartmentPremissionEdits.Select(o => o.ID).ToList() : null;
                bool isPay = IsPay(data.ID);
                bool checkDeposit = CheckIsDeposit(data.ID);

                if ((departmentPremissionIds == null || !departmentPremissionIds.Contains(currentDepartmentId)) && currentTreatment.IS_ACTIVE == 0)
                {
                    vali = false;
                    return vali;
                }//TODO truoc khi sưa currentTreatment.IS_PAUSE == 1

                if (!CheckLoginAdmin.IsAdmin(loginName)
                && ((currentDepartmentId != data.TDL_REQUEST_DEPARTMENT_ID
                && currentDepartmentId != data.TDL_EXECUTE_DEPARTMENT_ID
                && (departmentPremissionIds == null || !departmentPremissionIds.Contains(currentDepartmentId)))))
                {
                    vali = false;
                    mess = String.Format("({0}) {1} dịch vụ có khoa hiện tại không phải là khoa yêu cầu hoặc khoa xử lý", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                    return vali;
                }

                if (isPay)
                {
                    vali = false;
                    mess = String.Format("({0}) {1} dịch vụ đã thanh toán", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                    return vali;
                }

                if (checkDeposit)
                {
                    if (comlumnType == ComlumnType.NO_EXECUTE)
                    {
                        if (!this.IsAllowNoExecuteForPaid)
                        {
                            vali = false;
                            mess = String.Format("({0}) {1} dịch vụ đã tạm ứng", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                            return vali;
                        }

                        if (data.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            vali = false;
                            mess = String.Format("({0}) {1} dịch vụ đã được xử lý", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                            return vali;
                        }

                        if (data.TDL_REQUEST_LOGINNAME != loginName && !CheckLoginAdmin.IsAdmin(loginName))
                        {
                            vali = false;
                            mess = String.Format("({0}) {1} tài khoản không có quyền thực hiện", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                            return vali;
                        }
                    }
                    else
                    {
                        if (departmentPremissionIds == null || !departmentPremissionIds.Contains(currentDepartmentId))
                        {
                            if (comlumnType == ComlumnType.EXPEND)
                            {
                                vali = false;
                                mess = String.Format("({0}) {1} dịch vụ đã tạm ứng không cho phép sửa tại khoa không được cấu hình", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                                return vali;
                            }
                        }
                    }
                }

                //if (this.currentTreatment.IS_PAUSE == 1 && (comlumnType == ComlumnType.NO_EXECUTE || comlumnType == ComlumnType.EXPEND || comlumnType == ComlumnType.IS_OUT_PARENT_FEE))
                //{
                //    vali = false;
                //    mess = "Hồ sơ đã kết thúc điều trị";
                //    return vali;
                //}

                if (comlumnType == ComlumnType.IS_OUT_PARENT_FEE)
                {
                    if (!data.PARENT_ID.HasValue)
                    {
                        vali = false;
                        mess = String.Format("({0}) {1} dịch vụ không phải là dịch vụ đính kèm", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                        return vali;
                    }
                }

                if (comlumnType == ComlumnType.EXPEND)
                {
                    if ((data.IS_ALLOW_EXPEND ?? 0) != 1
                        && data.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        && data.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        vali = false;
                        mess = String.Format("({0}) {1} dịch vụ không có quyền thực hiện chức năng này", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                        return vali;
                    }
                }

                if (comlumnType == ComlumnType.EXPEND_TYPE_ID)
                {
                    if (!CheckExpendTypeOfData(data))
                    {
                        vali = false;
                        mess = String.Format("({0}) {1} dịch vụ không có quyền thực hiện chức năng này", data.TDL_SERVICE_REQ_CODE, data.SERVICE_NAME);
                        return vali;
                    }
                }

            }
            catch (Exception ex)
            {
                vali = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vali;
        }

        private bool IsPay(long sereServId)
        {
            bool result = false;
            try
            {
                HIS_SERE_SERV_BILL sereServBill = SereServBills != null ? SereServBills.FirstOrDefault(o => o.SERE_SERV_ID == sereServId && (o.IS_CANCEL == null || o.IS_CANCEL == 0)) : null;
                if (sereServBill != null)
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// La dich vụ tạm ứng
        /// </summary>
        /// <param name="sereServId"></param>
        /// <returns></returns>
        private bool CheckIsDeposit(long sereServId)
        {
            bool result = false;
            try
            {
                HIS_SERE_SERV_DEPOSIT sereServDeposit = sereServDeposits != null ? sereServDeposits.FirstOrDefault(o => o.SERE_SERV_ID == sereServId && (o.IS_CANCEL == null || o.IS_CANCEL == 0)) : null;
                if (sereServDeposit != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExpendTypeOfData(SereServADO data)
        {
            bool valid = true;
            try
            {
                valid = (data != null && (data.IS_EXPEND ?? 0) == 1
                          && !data.PARENT_ID.HasValue);
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckServiceExistChild(SereServADO data)
        {
            bool result = false;
            try
            {
                SereServADO sereServADO = this.SereServADOs.FirstOrDefault((SereServADO o) => o.PARENT_ID == data.ID);
                if (sereServADO != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewBordereau_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SereServADO data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewBordereau.GetDataSourceRowIndex(e.RowHandle);
                    data = (SereServADO)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string mess = null;
                        if (e.Column.FieldName == "PATIENT_TYPE_ID")
                        {
                            if ((data.PACKAGE_ID.HasValue && data.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1) || data.isAssignBlood)
                            {
                                e.RepositoryItem = repositoryItemLookUpEdit_PatientType_Disable;
                            }
                            else if (this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                            {
                                e.RepositoryItem = repositoryItemGridLookUpEdit_PatientType;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemLookUpEdit_PatientType_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                        {
                            if (data.isAssignBlood || (data.PACKAGE_ID.HasValue && data.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1))
                            {
                                e.RepositoryItem = repositoryItemGridLookUpEditPrimaryPatientType_Disabled;
                            }
                            else if (this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                            {
                                e.RepositoryItem = repositoryItemGridLookUpEditPrimaryPatientType;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemGridLookUpEditPrimaryPatientType_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "PARENT_ID")
                        {
                            if (!data.isAssignBlood && this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                            {
                                e.RepositoryItem = this.repositoryItemLookUpEditSereServPackage_Enabled;
                            }
                            else
                            {
                                e.RepositoryItem = this.repositoryItemLookUpEditSereServPackage_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "OTHER_PAY_SOURCE_ID")
                        {
                            if (!data.isAssignBlood && this.currentTreatment.IS_ACTIVE != 0 && this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                            {
                                e.RepositoryItem = this.repositoryItemGridLookUpEdit_OtherPaySource;
                            }
                            else
                            {
                                e.RepositoryItem = this.repositoryItemGridLookUpEdit_OtherPaySource_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "EQUIPMENT_SET_NAME__NUM_ORDER")
                        {
                            var serviceType = Services.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                            if (serviceType == null) return;
                            if (!data.isAssignBlood && this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess)
                                && serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {

                                e.RepositoryItem = this.repositoryItemButtonEditEquipment;
                            }
                            else
                            {
                                e.RepositoryItem = this.repositoryItemButtonEditEquipment_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "STENT_ORDER")
                        {
                            if (this.CheckIsStent(data.SERVICE_ID))
                            {
                                if (!data.isAssignBlood && this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                                {
                                    e.RepositoryItem = this.repositoryItemSpinEditStentOrder;
                                }
                                else
                                {
                                    e.RepositoryItem = this.repositoryItemSpinEditStentOrder_Disabled;
                                }
                            }
                            else
                            {
                                e.RepositoryItem.ReadOnly = true;
                            }
                        }
                        else if (e.Column.FieldName == "SHARE_COUNT")
                        {
                            if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                            {
                                if (!data.isAssignBlood && this.CheckPremissionEdit(data, ComlumnType.NO_CHECK_COLUMN, ref mess))
                                {
                                    e.RepositoryItem = this.repositoryItemSpinEditShareCount;
                                }
                                else
                                {
                                    e.RepositoryItem = this.repositoryItemSpinEditShareCount_Disabled;
                                }
                            }
                            else
                            {
                                e.RepositoryItem.ReadOnly = true;
                            }
                        }
                        else if (e.Column.FieldName == "IsExpend")
                        {
                            if (data.isAssignBlood || (data.PACKAGE_ID.HasValue && data.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1))
                            {
                                e.RepositoryItem = repositoryItemChkIsExpend_Disable;
                            }
                            else if (this.CheckPremissionEdit(data, ComlumnType.EXPEND, ref mess))
                            {
                                e.RepositoryItem = repositoryItemChkIsExpend;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemChkIsExpend_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "ExpendTypeId")
                        {
                            //Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha" 
                            if (!data.isAssignBlood && this.CheckPremissionEdit(data, ComlumnType.EXPEND_TYPE_ID, ref mess))
                            {
                                e.RepositoryItem = repositoryItem_expend_type_id_enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItem_expend_type_id_disable;
                            }
                        }
                        else if (e.Column.FieldName == "IsNoExecute")
                        {
                            if (data.isAssignBlood || (data.PACKAGE_ID.HasValue && data.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1))
                            {
                                e.RepositoryItem = repositoryItemCheckEditIsNoExecute_Disable;
                            }
                            else if (this.CheckPremissionEdit(data, ComlumnType.NO_EXECUTE, ref mess))
                            {
                                var serviceType = Services.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                                if (serviceType == null) return;
                                if (serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    || serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                    || serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU || serviceType.IS_DISALLOWANCE_NO_EXECUTE == 1)
                                {
                                    e.RepositoryItem = repositoryItemCheckEditIsNoExecute_Disable;


                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemCheckEditIsNoExecute;
                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheckEditIsNoExecute_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "IsNotUseBHYT")
                        {
                            if (data != null)
                            {
                                if (data.isAssignBlood || data.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT || this.currentTreatment.IS_ACTIVE == 0)
                                {
                                    e.RepositoryItem = repositoryItemCheckEditIsNotUseBHYT_Disable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemCheckEditIsNotUseBHYT;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "IsOutKtcFee")
                        {
                            if ((data.PACKAGE_ID.HasValue && data.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1) || data.isAssignBlood)
                            {
                                e.RepositoryItem = repositoryItemChkOutKtcFee_Disable;
                            }
                            else if (this.CheckPremissionEdit(data, ComlumnType.IS_OUT_PARENT_FEE, ref mess))
                            {
                                e.RepositoryItem = repositoryItemChkOutKtcFee_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemChkOutKtcFee_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                        {
                            //#15544
                            //- Chỉ hiển thị icon này, nếu dịch vụ là phẫu thuật, hoặc dịch vụ có cấu hình "Gói dịch vụ" (có package_id), và:
                            //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá phẫu thuật" --> hiển thị icon "sửa" ở ô "Giá" 
                            //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá gói" --> hiển thị icon "sửa" ở ô "Giá gói"   
                            bool isEditCtrol = false;
                            if (SereServCFG.AllowAssignPrice)
                            {
                                long packageId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewBordereau.GetRowCellValue(e.RowHandle, "PACKAGE_ID") ?? "0").ToString());
                                if (packageId > 0 && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_PACKAGE_PRICE == 1)
                                {
                                    isEditCtrol = false;
                                }
                                else
                                {
                                    long serviceTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewBordereau.GetRowCellValue(e.RowHandle, "SERVICE_TYPE_ID") ?? "0").ToString());
                                    //string strIsChecked = (gridViewBordereau.GetRowCellValue(e.RowHandle, "IsChecked") ?? "0").ToString();
                                    //bool IsChecked = (strIsChecked == "1" || strIsChecked.ToLower() == "true");

                                    if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_SURGERY_PRICE == 1)
                                    {
                                        isEditCtrol = true;
                                    }
                                }
                            }

                            e.RepositoryItem = isEditCtrol && !data.isAssignBlood ? repositoryItembtnEditDonGia_TextDisable : this.repositoryItemTxtReadOnly;
                        }
                        else if (e.Column.FieldName == "PACKAGE_PRICE_DISPLAY")
                        {
                            //#15544
                            //- Chỉ hiển thị icon này, nếu dịch vụ là phẫu thuật, hoặc dịch vụ có cấu hình "Gói dịch vụ" (có package_id), và:
                            //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá phẫu thuật" --> hiển thị icon "sửa" ở ô "Giá" 
                            //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá gói" --> hiển thị icon "sửa" ở ô "Giá gói"   
                            //string strIsChecked = (gridViewBordereau.GetRowCellValue(e.RowHandle, "IsChecked") ?? "0").ToString();
                            //bool IsChecked = (strIsChecked == "1" || strIsChecked.ToLower() == "true");

                            bool isEditCtrol = false;
                            if (SereServCFG.AllowAssignPrice)
                            {
                                long packageId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewBordereau.GetRowCellValue(e.RowHandle, "PACKAGE_ID") ?? "0").ToString());

                                if (packageId > 0 && this.currentDepartment != null && this.currentDepartment.ALLOW_ASSIGN_PACKAGE_PRICE == 1)
                                {
                                    isEditCtrol = true;
                                }
                            }

                            e.RepositoryItem = isEditCtrol && !data.isAssignBlood ? this.repositoryItembtnEditGiaGoi_TextDisable : this.repositoryItemTxtReadOnly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServADO sereserv = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (sereserv != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereserv.TDL_INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "IsExpend")
                        {
                            int statusId = sereserv.IS_EXPEND ?? 0;
                            if (statusId == 1)
                            {
                                e.Value = repositoryItemChkIsExpend.ValueChecked;
                            }
                            else
                            {
                                e.Value = repositoryItemChkIsExpend.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "ExpendTypeId")
                        {
                            long expTypeId = sereserv.EXPEND_TYPE_ID ?? 0;
                            if (expTypeId == 1)
                            {
                                e.Value = repositoryItem_expend_type_id_enable.ValueChecked;
                            }
                            else
                            {
                                e.Value = repositoryItem_expend_type_id_enable.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "IsNoExecute")
                        {
                            int statusId = sereserv.IS_NO_EXECUTE ?? 0;
                            // HIS_SERVICE service = new HIS_SERVICE();
                            // service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == sereserv.SERVICE_ID);
                            // int IS_DISALLOWANCE_NO_EXECUTE_ = service.IS_DISALLOWANCE_NO_EXECUTE ?? 0;
                            //  Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() =>service), service));
                            //Inventec.Common.Logging.LogSystem.Info("\r\nIS_DISALLOWANCE_NO_EXECUTE___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IS_DISALLOWANCE_NO_EXECUTE_), IS_DISALLOWANCE_NO_EXECUTE_));
                            if (statusId == 1)
                            {
                                e.Value = repositoryItemCheckEditIsNoExecute.ValueChecked;
                            }
                            else
                            {
                                //e.Value = null;

                                //if(IS_DISALLOWANCE_NO_EXECUTE_ == 1)
                                e.Value = repositoryItemCheckEditIsNoExecute.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "IsNotUseBHYT")
                        {
                            var IsNotUseBHYT = (sereserv.IS_NOT_USE_BHYT ?? 0) == 1;
                            if (IsNotUseBHYT)
                            {
                                e.Value = repositoryItemCheckEditIsNotUseBHYT.ValueChecked;
                            }
                            else
                            {
                                e.Value = repositoryItemCheckEditIsNotUseBHYT.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "IsFundAccepted")
                        {
                            int funAccept = sereserv.IS_FUND_ACCEPTED ?? 0;
                            if (funAccept == 1)
                            {
                                e.Value = repositoryItemchkIsFundAccepted.ValueChecked;
                            }
                            else
                            {
                                e.Value = repositoryItemchkIsFundAccepted.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "IsOutKtcFee")
                        {
                            int statusId = sereserv.IS_OUT_PARENT_FEE ?? 0;
                            if (statusId == 1)
                            {
                                e.Value = repositoryItemChkOutKtcFee_Enable.ValueChecked;
                            }
                            else
                            {

                                //e.Value = null;
                                e.Value = repositoryItemChkOutKtcFee_Enable.ValueUnchecked;
                            }
                        }
                        else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                        {
                            if (sereserv.AssignSurgPriceEdit.HasValue && sereserv.AssignSurgPriceEdit > 0)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.AssignSurgPriceEdit ?? 0, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.VIR_PRICE_NO_EXPEND ?? 0, ConfigApplications.NumberSeperator);
                            }
                        }
                        else if (e.Column.FieldName == "PACKAGE_PRICE_DISPLAY")
                        {
                            if (sereserv.AssignPackagePriceEdit.HasValue && sereserv.AssignPackagePriceEdit > 0)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.AssignPackagePriceEdit ?? 0, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.PACKAGE_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            }
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.VIR_TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_TEMP_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.VIR_TOTAL_PATIENT_PRICE_TEMP ?? 0, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "OTHER_SOURCE_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(sereserv.OTHER_SOURCE_PRICE ?? 0, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                SereServADO sereServ = (SereServADO)gridViewBordereau.GetRow(e.RowHandle);
                bool isPaid = false;
                if (sereServ != null && SereServBills != null && SereServBills.Count() > 0)
                {
                    isPaid = SereServBills.Where(o => o.IS_CANCEL == null || o.IS_CANCEL == 0).Select(o => o.SERE_SERV_ID).Contains(sereServ.ID);
                }

                if (isPaid)
                    e.Appearance.ForeColor = Color.Blue;
                else
                    e.Appearance.ForeColor = Color.Black;

                if (sereServ != null && sereServ.AMOUNT_TEMP != null && sereServ.AMOUNT_TEMP > 0)
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

                if (sereServ != null && sereServ.isAssignBlood)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SereServADO data = view.GetFocusedRow() as SereServADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    FillDataIntoPatientTypeCombo(data, editor, data.PRIMARY_PATIENT_TYPE_ID,true);
                    if (data != null)
                    {
                        if (data.PATIENT_TYPE_ID != null)
                        {
                            editor.EditValue = data.PATIENT_TYPE_ID;
                        }
                    }

                }
                else if (view.FocusedColumn.FieldName == "PARENT_ID" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;

                    FillDataIntoSereServPackageCombo(editor, data);
                    if (data != null)
                    {
                        if (data.PARENT_ID != null)
                        {
                            editor.EditValue = data.PARENT_ID;
                            if (editor.Properties.Buttons.Count > 1)
                                editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityButtonClick;
                        }
                    }
                }
                else if (view.FocusedColumn.FieldName == "EQUIPMENT_SET_ID" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;

                    if (data != null)
                    {
                        if (data.PARENT_ID != null)
                        {
                            editor.EditValue = data.PARENT_ID;
                            if (editor.Properties.Buttons.Count > 1)
                                editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityButtonClick;
                        }
                    }

                }
                else if (view.FocusedColumn.FieldName == "IsExpend" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    if (this.currentTreatment.IS_ACTIVE == 0)//IS_PAUSE == 1
                    {
                        editor.ReadOnly = true;
                        editor.Enabled = false;
                    }

                }
                else if (view.FocusedColumn.FieldName == "ExpendTypeId" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;

                    if (!(data.IS_EXPEND == 1 && data.PARENT_ID == null))
                    {
                        editor.ReadOnly = true;
                        editor.Enabled = false;
                    }

                }
                else if (view.FocusedColumn.FieldName == "IsOutKtcFee" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    if (this.currentTreatment.IS_ACTIVE == 0)//IS_PAUSE == 1
                    {

                        editor.ReadOnly = true;
                        editor.Enabled = false;
                    }
                }
                else if (view.FocusedColumn.FieldName == "IsNoExecute" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    if (this.currentTreatment.IS_ACTIVE == 0)//IS_PAUSE == 1
                    {
                        editor.ReadOnly = true;
                        editor.Enabled = false;
                    }

                }
                else if (view.FocusedColumn.FieldName == "IsNotUseBHYT" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    if (this.currentTreatment.IS_ACTIVE == 0)//IS_PAUSE == 1
                    {
                        editor.ReadOnly = true;
                        editor.Enabled = false;
                    }
                    if (data != null)
                    {
                        if (data.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            editor.ReadOnly = true;
                            editor.Enabled = false;
                        }
                    }
                }
                else if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    FillDataIntoPatientTypeCombo(data, editor, data.PATIENT_TYPE_ID);
                    if (data != null)
                    {
                        data.oldValuePrimaryPatientType = data.PRIMARY_PATIENT_TYPE_ID;
                        if (data.PRIMARY_PATIENT_TYPE_ID.HasValue)
                        {
                            editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID.Value;
                            if (editor.Properties.Buttons.Count > 1)
                                editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityGridLookUpButtonClick;
                        }
                    }

                }

                else if (view.FocusedColumn.FieldName == "OTHER_PAY_SOURCE_ID" && view.ActiveEditor is GridLookUpEdit)
                {

                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataIntoOtherPaySourceCombo(editor);
                    if (data != null)
                    {
                        if (data.OTHER_PAY_SOURCE_ID.HasValue)
                        {
                            editor.EditValue = data.OTHER_PAY_SOURCE_ID.Value;
                            if (editor.Properties.Buttons.Count > 1)
                                editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityGridLookUpButtonClick;
                        }
                    }

                }
                else if (view.FocusedColumn.FieldName == "SERVICE_CONDITION_ID" && view.ActiveEditor is GridLookUpEdit)
                {

                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataIntoOtherConditionCombo(editor, data.SERVICE_ID);
                    if (data != null)
                    {
                        if (data.PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            if (editor.Properties.Buttons.Count > 1)
                                editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityGridLookUpButtonClick;
                        }
                        else if (data.SERVICE_CONDITION_ID.HasValue && data.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            editor.EditValue = data.SERVICE_CONDITION_ID.Value;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataIntoOtherConditionCombo(DevExpress.XtraEditors.GridLookUpEdit cbo, long serviceId)
        {


            try
            {
                var dataCondition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.SERVICE_ID == serviceId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "Mã", 80, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_NAME", "Tên", 300, 2));
                columnInfos.Add(new ColumnInfo("HEIN_RATIO_STR", "Tỉ lệ thanh toán", 80, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, true);
                List<ServiceConditionADO> serviceConditionADOs = (from r in dataCondition select new ServiceConditionADO(r)).ToList();
                cbo.Properties.DataSource = serviceConditionADOs;
                cbo.Properties.DisplayMember = controlEditorADO.DisplayMember;
                cbo.Properties.ValueMember = controlEditorADO.ValueMember;
                cbo.Properties.TextEditStyle = TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = controlEditorADO.ImmediatePopup;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                foreach (ColumnInfo columnInfo in controlEditorADO.ColumnInfos)
                {
                    HorzAlignment hAlignment = HorzAlignment.Default;
                    switch (columnInfo.horzAlignment)
                    {
                        case ColumnInfo.HorzAlignment.Default:
                            hAlignment = HorzAlignment.Default;
                            break;
                        case ColumnInfo.HorzAlignment.Near:
                            hAlignment = HorzAlignment.Near;
                            break;
                        case ColumnInfo.HorzAlignment.Center:
                            hAlignment = HorzAlignment.Center;
                            break;
                        case ColumnInfo.HorzAlignment.Far:
                            hAlignment = HorzAlignment.Far;
                            break;
                    }

                    GridColumn gridColumn = cbo.Properties.View.Columns.AddField(columnInfo.fieldName);
                    gridColumn.Caption = columnInfo.caption;
                    gridColumn.Visible = columnInfo.visible;
                    gridColumn.VisibleIndex = columnInfo.VisibleIndex;
                    gridColumn.Width = ((columnInfo.width == 0) ? 100 : columnInfo.width);
                    gridColumn.AppearanceCell.TextOptions.HAlignment = hAlignment;
                    gridColumn.OptionsColumn.FixedWidth = columnInfo.FixedWidth;
                    gridColumn.ColumnEdit = repositoryItemMemoEdit;
                }
                cbo.Properties.View.OptionsView.RowAutoHeight = true;
                cbo.Properties.View.OptionsView.ColumnAutoWidth = true;
                cbo.Properties.View.OptionsView.ShowIndicator = false;
                cbo.Properties.View.OptionsView.ShowGroupPanel = false;
                cbo.Properties.PopupFormSize = new Size(controlEditorADO.PopupWidth, 200);
                cbo.Properties.View.OptionsView.ShowColumnHeaders = controlEditorADO.ShowHeader;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoOtherPaySourceCombo(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                var datas = OtherPaySources != null ? OtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            //try
            //{
            //    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick. 1");
            //    SereServADO sereServADO = this.gridViewBordereau.GetFocusedRow() as SereServADO;
            //    string mess = "";
            //    if (sereServADO != null)
            //    {
            //        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();

            //        if (e.Column.FieldName == "IsNoExecute")
            //        {
            //            if (!this.CheckPremissionEdit(sereServADO, ComlumnType.NO_EXECUTE, ref mess))
            //            {
            //                if (!String.IsNullOrEmpty(mess))
            //                    MessageManager.Show(mess);
            //                Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false");
            //                return;
            //            }

            //            if (sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
            //                    && sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
            //                    && sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
            //            {
            //                if (sereServADO.IS_NO_EXECUTE == 1)
            //                {
            //                    List<long> serviceIds = new List<long> { sereServADO.SERVICE_ID };
            //                    List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(currentTreatment.PATIENT_ID, serviceIds);
            //                    if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
            //                    {
            //                        string sereServMinDurationStr = "";
            //                        foreach (var item in sereServWithMinDurations)
            //                        {
            //                            sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
            //                        }

            //                        if (MessageBox.Show(string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} .Bạn có muốn tiếp tục?", sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
            //                            return;
            //                    }
            //                }

            //                sereServ.ID = sereServADO.ID;
            //                if (sereServADO.IS_NO_EXECUTE == 1)
            //                    sereServ.IS_NO_EXECUTE = null;
            //                else
            //                    sereServ.IS_NO_EXECUTE = 1;

            //                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
            //                sereServs.Add(sereServ);
            //                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
            //                hisSereServPayslipSDO.Field = UpdateField.IS_NO_EXECUTE;
            //                hisSereServPayslipSDO.SereServs = sereServs;
            //                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
            //                this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
            //            }
            //        }
            //        else if (e.Column.FieldName == "IsExpend")
            //        {
            //            Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick. 2");
            //            if (!this.CheckPremissionEdit(sereServADO, ComlumnType.EXPEND, ref mess))
            //            {
            //                if (!String.IsNullOrEmpty(mess))
            //                    MessageManager.Show(mess);
            //                Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false");
            //                return;
            //            }
            //            Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick. 3");
            //            sereServ.ID = sereServADO.ID;

            //            if (sereServADO.IS_EXPEND == 1)
            //                sereServ.IS_EXPEND = null;
            //            else
            //                sereServ.IS_EXPEND = 1;

            //            List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
            //            sereServs.Add(sereServ);
            //            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
            //            hisSereServPayslipSDO.Field = UpdateField.IS_EXPEND;
            //            hisSereServPayslipSDO.SereServs = sereServs;
            //            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
            //            this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
            //        }
            //        else if (e.Column.FieldName == "ExpendTypeId" && CheckExpendTypeOfData(sereServADO))
            //        {
            //            Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick. 4");
            //            if (!this.CheckPremissionEdit(sereServADO, ComlumnType.EXPEND_TYPE_ID, ref mess))
            //            {
            //                if (!String.IsNullOrEmpty(mess))
            //                    MessageManager.Show(mess);
            //                Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false");
            //                return;
            //            }
            //            Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick. 5");
            //            sereServ.ID = sereServADO.ID;

            //            if (sereServADO.EXPEND_TYPE_ID == 1)
            //                sereServ.EXPEND_TYPE_ID = null;
            //            else
            //                sereServ.EXPEND_TYPE_ID = 1;

            //            List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
            //            sereServs.Add(sereServ);
            //            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
            //            hisSereServPayslipSDO.Field = UpdateField.EXPEND_TYPE_ID;
            //            hisSereServPayslipSDO.SereServs = sereServs;
            //            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
            //            this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
            //            Inventec.Common.Logging.LogSystem.Debug("UpdatePayslipInfoProcess. call");
            //        }
            //        else if (e.Column.FieldName == "IsOutKtcFee")
            //        {
            //            if (!this.CheckPremissionEdit(sereServADO, ComlumnType.IS_OUT_PARENT_FEE, ref mess))
            //            {
            //                if (!String.IsNullOrEmpty(mess))
            //                    MessageManager.Show(mess);
            //                Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false");
            //                return;
            //            }
            //            sereServ.ID = sereServADO.ID;
            //            if (sereServADO.IS_OUT_PARENT_FEE == 1)
            //                sereServ.IS_OUT_PARENT_FEE = null;
            //            else
            //                sereServ.IS_OUT_PARENT_FEE = 1;

            //            List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
            //            sereServs.Add(sereServ);
            //            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
            //            hisSereServPayslipSDO.Field = UpdateField.IS_OUT_PARENT_FEE;
            //            hisSereServPayslipSDO.SereServs = sereServs;
            //            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
            //            this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
            //        }
            //    }
            //    else
            //    {
            //        Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_RowCellClick.sereServADO is null");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogSystem.Warn(ex);
            //}
        }

        private void gridViewBordereau_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    int visibleRowHandle = this.gridViewBordereau.GetVisibleRowHandle(hitInfo.RowHandle);
                    int[] selectedRows = this.gridViewBordereau.GetSelectedRows();
                    if (selectedRows != null && selectedRows.Length > 0 && selectedRows.Contains(visibleRowHandle))
                    {

                        this.InitMenu();

                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                SereServADO sereServADO = this.gridViewBordereau.GetFocusedRow() as SereServADO;
                if (sereServADO != null)
                {
                    if (e.Column.FieldName == "STENT_ORDER")
                    {
                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.STENT_ORDER;
                        this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    }
                    else if (e.Column.FieldName == "SHARE_COUNT")
                    {
                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.SHARE_COUNT;
                        this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    }
                    //else if (e.Column.FieldName == "ExpendTypeId")
                    //{
                    //    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                    //    HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                    //    hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                    //    hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                    //    hisSereServPayslipSDO.Field = UpdateField.EXPEND_TYPE_ID;
                    //    this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    //}
                    else if (e.Column.FieldName == "PARENT_ID")
                    {
                        SereServADO ss = this.SereServADOs.FirstOrDefault(o => o.ID == sereServADO.ID);
                        LookUpEdit editor = ((GridView)sender).ActiveEditor as LookUpEdit;
                        if (ss.PARENT_ID == sereServADO.ID)
                            return;

                        if (sereServADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            && sereServADO.PARENT_ID.HasValue)
                        {
                            DialogResult myResult;
                            //int rowHandle = gridViewBordereau.FocusedRowHandle;
                            //GridColumn column = gridViewBordereau.FocusedColumn;
                            object oldValue = editor.OldEditValue;
                            //gridViewBordereau.SetRowCellValue(rowHandle, column, oldValue);
                            myResult = MessageBox.Show("Dịch vụ này là dịch vụ phẫu thuật. Bạn có chắc chắn muốn gán vào gói?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
                            {
                                sereServADO.PARENT_ID = oldValue != null ? (long?)oldValue : null;
                            }
                        }

                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.PARENT_ID;
                        this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    }
                    else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                    {
                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.PRIMARY_PATIENT_TYPE_ID;
                        var result = this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                        if (!result)
                        {
                            gridControlBordereau.BeginUpdate();
                            long? oldPatientTypeId = sereServADO.oldValuePrimaryPatientType;
                            gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                            //Load lai du lieu cu
                            foreach (var item in this.SereServADOs)
                            {
                                if (item.ID == sereServ.ID)
                                {
                                    item.PRIMARY_PATIENT_TYPE_ID = oldPatientTypeId;
                                    break;
                                }
                            }
                            gridControlBordereau.RefreshDataSource();
                            gridControlBordereau.EndUpdate();
                        }
                    }
                    //else if (e.Column.FieldName == "OTHER_PAY_SOURCE_ID")
                    //{
                    //    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                    //    HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                    //    hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                    //    hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                    //    hisSereServPayslipSDO.Field = UpdateField.OTHER_PAY_SOURCE_ID;
                    //    this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    //}
                    else if (e.Column.FieldName == "EQUIPMENT_SET_NAME__NUM_ORDER")
                    {
                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.EQUIPMENT_SET_ORDER__AND__EQUIPMENT_SET_ID;
                        this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_ShowFilterPopupListBox(object sender, FilterPopupListBoxEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "PARENT_ID")
                {
                    foreach (var item in e.ComboBox.Items)
                    {
                        FilterItem filterItem = item as FilterItem;
                        if (filterItem == null)
                            continue;
                        SereServADO sereServADO = this.SereServADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(filterItem.Text));
                        if (sereServADO != null)
                            filterItem.Text = sereServADO.SERVICE_REQ_CODE___SERVICE_CODE;
                    }
                }
                else if (e.Column.FieldName == "PATIENT_TYPE_ID")
                {
                    foreach (var item in e.ComboBox.Items)
                    {
                        FilterItem filterItem = item as FilterItem;
                        if (filterItem == null)
                            continue;
                        SereServADO sereServADO = this.SereServADOs.FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(filterItem.Text));
                        if (sereServADO != null)
                            filterItem.Text = sereServADO.PATIENT_TYPE_NAME;
                    }
                }
                else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                {
                    foreach (var item in e.ComboBox.Items)
                    {
                        FilterItem filterItem = item as FilterItem;
                        if (filterItem == null)
                            continue;
                        SereServADO sereServADO = this.SereServADOs.FirstOrDefault(o => o.PRIMARY_PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(filterItem.Text));
                        if (sereServADO != null)
                            filterItem.Text = sereServADO.PATIENT_TYPE_NAME;
                    }
                }
                else if (e.Column.FieldName == "OTHER_PAY_SOURCE_ID")
                {
                    foreach (var item in e.ComboBox.Items)
                    {
                        FilterItem filterItem = item as FilterItem;
                        if (filterItem == null)
                            continue;
                        SereServADO sereServADO = this.SereServADOs.FirstOrDefault(o => o.OTHER_PAY_SOURCE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(filterItem.Text));
                        if (sereServADO != null)
                            filterItem.Text = sereServADO.OTHER_PAY_SOURCE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBordereau_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.0");
                    string mess = "";
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.0.1");

                    if (hi != null && hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.0.1.1");
                        if (hi.RowHandle >= 0) //(hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit) || hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemTextEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.0.2");
                            int rowHandle = gridViewBordereau.GetVisibleRowHandle(hi.RowHandle);
                            var sereServADO = (SereServADO)gridViewBordereau.GetRow(rowHandle);
                            HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                            sereServ.PRIMARY_PATIENT_TYPE_ID = sereServADO.PRIMARY_PATIENT_TYPE_ID;
                            sereServ.SERVICE_CONDITION_ID = sereServADO.SERVICE_CONDITION_ID;
                            //if (hi.Column.FieldName == "ExpendTypeId")
                            //{
                            //    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.1 return");
                            //    //Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha"
                            //    if (sereServADO.isAssignBlood || (sereServADO != null && !(sereServADO.IS_EXPEND == 1
                            //   && !sereServADO.PARENT_ID.HasValue)))
                            //    {
                            //        (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            //        return;
                            //    }
                            //}
                            //else 
                            if (hi.Column.FieldName == "IsNoExecute")
                            {
                                if (sereServADO.isAssignBlood) return;
                                var serviceType = Services.FirstOrDefault(o => o.ID == sereServADO.SERVICE_ID);
                                if (serviceType.IS_DISALLOWANCE_NO_EXECUTE == 1)
                                {
                                    return;
                                }
                                if (sereServADO.PACKAGE_ID.HasValue && sereServADO.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("PACKAGE_IS_NOT_FIXED_SERVICE!=1. return");
                                    return;
                                }
                                if (!this.CheckPremissionEdit(sereServADO, ComlumnType.NO_EXECUTE, ref mess))
                                {
                                    if (!String.IsNullOrEmpty(mess))
                                        MessageManager.Show(mess);
                                    Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false return");
                                    return;
                                }

                                if (sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                        && sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                        && sereServADO.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                                {
                                    if (sereServADO.IS_NO_EXECUTE == 1)
                                    {
                                        List<long> serviceIds = new List<long> { sereServADO.SERVICE_ID };
                                        List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(currentTreatment.PATIENT_ID, serviceIds);
                                        if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
                                        {
                                            string sereServMinDurationStr = "";
                                            foreach (var item in sereServWithMinDurations)
                                            {
                                                sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                                            }

                                            if (MessageBox.Show(string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} .Bạn có muốn tiếp tục?", sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                                Inventec.Common.Logging.LogSystem.Debug("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép:  return");
                                            return;
                                        }
                                    }

                                    sereServ.ID = sereServADO.ID;
                                    if (sereServADO.IS_NO_EXECUTE == 1)
                                        sereServ.IS_NO_EXECUTE = null;
                                    else
                                        sereServ.IS_NO_EXECUTE = 1;

                                    List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                    sereServs.Add(sereServ);
                                    HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                    hisSereServPayslipSDO.Field = UpdateField.IS_NO_EXECUTE;
                                    hisSereServPayslipSDO.SereServs = sereServs;
                                    hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                    if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                        (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                }
                            }
                            else if (hi.Column.FieldName == "IsNotUseBHYT")
                            {
                                if (sereServADO.isAssignBlood) return;
                                if (sereServADO.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT || this.currentTreatment.IS_ACTIVE == 0)
                                {
                                    return;
                                }

                                sereServ.ID = sereServADO.ID;
                                if (sereServADO.IS_NOT_USE_BHYT == 1)
                                    sereServ.IS_NOT_USE_BHYT = null;
                                else
                                    sereServ.IS_NOT_USE_BHYT = 1;

                                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                sereServs.Add(sereServ);
                                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                hisSereServPayslipSDO.Field = UpdateField.IS_NOT_USE_BHYT;
                                hisSereServPayslipSDO.SereServs = sereServs;
                                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            }
                            else if (hi.Column.FieldName == "IsExpend")
                            {
                                if (sereServADO.isAssignBlood) return;
                                if (sereServADO.PACKAGE_ID.HasValue && sereServADO.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("PACKAGE_IS_NOT_FIXED_SERVICE!=1. return");
                                    return;
                                }
                                if (!this.CheckPremissionEdit(sereServADO, ComlumnType.EXPEND, ref mess))
                                {
                                    if (!String.IsNullOrEmpty(mess))
                                        MessageManager.Show(mess);
                                    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.2.2 CheckPremissionEdit false return");
                                    return;
                                }
                                sereServ.ID = sereServADO.ID;

                                if (sereServADO.IS_EXPEND == 1 && sereServADO.EXPEND_TYPE_ID == 1)
                                {
                                    MessageManager.Show("Yêu cầu bỏ hao phí tiền giường trước khi bỏ hao phí");
                                    Inventec.Common.Logging.LogSystem.Debug("gridViewBordereau_MouseDown.2.3 return____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO), sereServADO));
                                    return;
                                }

                                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();

                                if (sereServADO.IS_EXPEND == 1)
                                    sereServ.IS_EXPEND = null;
                                else
                                    sereServ.IS_EXPEND = 1;
                                sereServs = new List<HIS_SERE_SERV>();
                                sereServs.Add(sereServ);
                                hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                hisSereServPayslipSDO.Field = UpdateField.IS_EXPEND;
                                hisSereServPayslipSDO.SereServs = sereServs;
                                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            }
                            else if (hi.Column.FieldName == "ExpendTypeId" && CheckExpendTypeOfData(sereServADO))
                            {
                                if (sereServADO.isAssignBlood) return;
                                if (!this.CheckPremissionEdit(sereServADO, ComlumnType.EXPEND_TYPE_ID, ref mess))
                                {
                                    if (!String.IsNullOrEmpty(mess))
                                        MessageManager.Show(mess);
                                    Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false return");
                                    return;
                                }
                                sereServ.ID = sereServADO.ID;

                                if (sereServADO.EXPEND_TYPE_ID == 1)
                                    sereServ.EXPEND_TYPE_ID = null;
                                else
                                    sereServ.EXPEND_TYPE_ID = 1;

                                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                sereServs.Add(sereServ);
                                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                hisSereServPayslipSDO.Field = UpdateField.EXPEND_TYPE_ID;
                                hisSereServPayslipSDO.SereServs = sereServs;
                                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                Inventec.Common.Logging.LogSystem.Debug("UpdatePayslipInfoProcess. call");
                            }
                            else if (hi.Column.FieldName == "IsOutKtcFee")
                            {
                                if (sereServADO.isAssignBlood) return;
                                Inventec.Common.Logging.LogSystem.Debug("FieldName=IsOutKtcFee");
                                if (sereServADO.PACKAGE_ID.HasValue && sereServADO.PACKAGE_IS_NOT_FIXED_SERVICE != (short)1)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("PACKAGE_IS_NOT_FIXED_SERVICE!=1. return");
                                    return;
                                }
                                if (!this.CheckPremissionEdit(sereServADO, ComlumnType.IS_OUT_PARENT_FEE, ref mess))
                                {
                                    if (!String.IsNullOrEmpty(mess))
                                        MessageManager.Show(mess);
                                    Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false return");
                                    return;
                                }
                                sereServ.ID = sereServADO.ID;
                                if (sereServADO.IS_OUT_PARENT_FEE == 1)
                                    sereServ.IS_OUT_PARENT_FEE = null;
                                else
                                    sereServ.IS_OUT_PARENT_FEE = 1;

                                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                sereServs.Add(sereServ);
                                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                hisSereServPayslipSDO.Field = UpdateField.IS_OUT_PARENT_FEE;
                                hisSereServPayslipSDO.SereServs = sereServs;
                                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            }
                            else if (hi.Column.FieldName == "IsFundAccepted")
                            {
                                if (sereServADO.isAssignBlood) return;
                                if (!this.CheckPremissionEdit(sereServADO, ComlumnType.IS_FUN_ACCEPT, ref mess))
                                {
                                    if (!String.IsNullOrEmpty(mess))
                                        MessageManager.Show(mess);
                                    Inventec.Common.Logging.LogSystem.Debug("CheckPremissionEdit=false return");
                                    return;
                                }
                                sereServ.ID = sereServADO.ID;
                                if (sereServADO.IS_FUND_ACCEPTED == 1)
                                    sereServ.IS_FUND_ACCEPTED = null;
                                else
                                    sereServ.IS_FUND_ACCEPTED = 1;

                                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                sereServs.Add(sereServ);
                                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                                hisSereServPayslipSDO.Field = UpdateField.IS_FUND_ACCEPTED;
                                hisSereServPayslipSDO.SereServs = sereServs;
                                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                                if (this.UpdatePayslipInfoProcess(hisSereServPayslipSDO))
                                    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hi.RowHandle), hi.RowHandle) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hi.Column.FieldName), hi.Column.FieldName));
                        }
                    }
                    else
                    {
                        //if (hi != null)
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hi.RowHandle), hi.RowHandle) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hi.Column.FieldName), hi.Column.FieldName));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reposityGridLookUpButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridLookUpEdit editor = sender as GridLookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void reposityButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    LookUpEdit editor = sender as LookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool isReturningBackOldValue = false;
        private void repositoryItemGridLookUpEdit_PatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isReturningBackOldValue)
                {
                    isReturningBackOldValue = false;
                    return;
                }
                HIS_SERE_SERV vSereServ = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewBordereau.GetFocusedRow();
                if (vSereServ != null)
                {
                    HIS_SERE_SERV sereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(sereServ, vSereServ);
                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);

                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    if (edit == null) return;
                    if (edit.EditValue != null)
                    {
                        var pt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(false, true)
                            .FirstOrDefault(o =>
                                (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString())
                                || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));

                        var oldPt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>()
                            .FirstOrDefault(o =>
                                (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.OldEditValue ?? "").ToString())
                                || o.PATIENT_TYPE_NAME == (edit.OldEditValue ?? "").ToString()));

                        if (pt != null)
                        {
                            if (pt.PATIENT_TYPE_CODE == HisPatientTypeCFG.PATIENT_TYPE_CODE__BHYT && sereServ.IS_NOT_USE_BHYT == 1)
                            {
                                string message = String.Format("Không cho phép đổi sang ĐTTT BHYT do dịch vụ {0} - {1}(Mã y lệnh: {2}) đã được người chỉ định tích 'Không hưởng BHYT'", service.SERVICE_CODE, service.SERVICE_NAME, sereServ.TDL_SERVICE_REQ_CODE);
                                DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                                isReturningBackOldValue = true;
                                edit.EditValue = oldPt != null ? (long?)oldPt.ID : null;
                                return;
                            }
                            long? ptBillPatientTYpe = sereServ.PRIMARY_PATIENT_TYPE_ID;
                            if (service != null && service.BILL_PATIENT_TYPE_ID != null && service.IS_NOT_CHANGE_BILL_PATY == 1
                                && (string.IsNullOrEmpty(service.APPLIED_PATIENT_TYPE_IDS) || IsContainString(service.APPLIED_PATIENT_TYPE_IDS, pt.ID.ToString()))
                                && ((oldPt != null && oldPt.ID == service.BILL_PATIENT_TYPE_ID && pt.ID != oldPt.ID)
                                || IsContainString(service.APPLIED_PATIENT_TYPE_IDS, pt.ID.ToString()))
                                && (string.IsNullOrEmpty(service.APPLIED_PATIENT_CLASSIFY_IDS) || IsContainString(service.APPLIED_PATIENT_CLASSIFY_IDS, this.currentTreatment.TDL_PATIENT_CLASSIFY_ID != null ? this.currentTreatment.TDL_PATIENT_CLASSIFY_ID.ToString() : "-1"))
                                && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.IS_SET_PRIMARY_PATIENT_TYPE) == "1"
                                )
                            {
                                string message = String.Format("Dịch vụ {0} bắt buộc sử dụng với đối tượng {1}. Phần mềm sẽ tự động cập nhật đối tượng phụ thu sang {1}.\nBạn có muốn tiếp tục không?", service.SERVICE_NAME, oldPt.PATIENT_TYPE_NAME);
                                if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                                {
                                    //long oldPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                                    //gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                                    ////Load lai du lieu cu
                                    //foreach (var item in this.SereServADOs)
                                    //{
                                    //    if (item.ID == sereServ.ID)
                                    //    {
                                    //        item.PATIENT_TYPE_ID = oldPatientTypeId;
                                    //        break;
                                    //    }
                                    //}
                                    btnFind_Click(null, null);
                                    return;
                                }

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => service.BILL_PATIENT_TYPE_ID), service.BILL_PATIENT_TYPE_ID));
                                ptBillPatientTYpe = service.BILL_PATIENT_TYPE_ID;

                            }
                            if (Convert.ToInt64(edit.EditValue.ToString()) != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                            {
                                sereServ.SERVICE_CONDITION_ID = null;
                                vSereServ.SERVICE_CONDITION_ID = null;
                            }
                            sereServ.PRIMARY_PATIENT_TYPE_ID = ptBillPatientTYpe;
                            sereServ.PATIENT_TYPE_ID = pt.ID;
                            List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                            list.Add(sereServ);
                            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                            hisSereServPayslipSDO.Field = UpdateField.PATIENT_TYPE_ID;
                            hisSereServPayslipSDO.SereServs = list;
                            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;

                            bool result = this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                            if (!result)
                            {
                                gridControlBordereau.BeginUpdate();
                                long oldPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                                gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                                //Load lai du lieu cu
                                foreach (var item in this.SereServADOs)
                                {
                                    if (item.ID == sereServ.ID)
                                    {
                                        item.PATIENT_TYPE_ID = oldPatientTypeId;
                                        break;
                                    }
                                }
                                gridControlBordereau.RefreshDataSource();
                                gridControlBordereau.EndUpdate();
                            }
                            else
                            {
                                if ((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) || !string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS)))
                                {
                                    List<string> lstPt = new List<string>();
                                    List<string> lstOldPt = new List<string>();
                                    if (!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS))
                                        lstPt = pt.OTHER_PAY_SOURCE_IDS.Split(',').ToList();
                                    if (!string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                        lstOldPt = oldPt.OTHER_PAY_SOURCE_IDS.Split(',').ToList();
                                    if (((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                        || (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && !string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                        || lstPt.Where(o => lstOldPt.Exists(p => p == o)).ToList() == null
                                            || lstPt.Where(o => lstOldPt.Exists(p => p == o)).ToList().Count == 0)
                                        && ((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServ.OTHER_PAY_SOURCE_ID != null && lstPt.FirstOrDefault(o => Int64.Parse(o) == sereServ.OTHER_PAY_SOURCE_ID) == null)
                                                || (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServ.OTHER_PAY_SOURCE_ID != null)
                                                || (!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServ.OTHER_PAY_SOURCE_ID == null)))
                                    {
                                        string message = String.Format("Bạn có muốn thay đổi thông tin nguồn khác chi trả của dịch vụ {0} hay không?", sereServ.TDL_SERVICE_NAME);
                                        if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                                        {
                                            return;
                                        }
                                        List<HIS_SERE_SERV> listOtherPay = new List<HIS_SERE_SERV>();
                                        if (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS))
                                            sereServ.OTHER_PAY_SOURCE_ID = null;
                                        else
                                            sereServ.OTHER_PAY_SOURCE_ID = Int64.Parse(lstPt[0]);
                                        listOtherPay.Add(sereServ);
                                        HisSereServPayslipSDO hisSereServOtherPayslipSDO = new HisSereServPayslipSDO();
                                        hisSereServOtherPayslipSDO.Field = UpdateField.OTHER_PAY_SOURCE_ID;
                                        hisSereServOtherPayslipSDO.SereServs = listOtherPay;
                                        hisSereServOtherPayslipSDO.TreatmentId = currentTreatment.ID;
                                        UpdatePayslipInfoProcess(hisSereServOtherPayslipSDO);
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
        }

        private void repositoryItemLookUpEditSereServPackage_Enabled_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditEquipment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServADO sereServADO = gridViewBordereau.GetFocusedRow() as SereServADO;
                if (sereServADO != null)
                {
                    frmEquipmentSet frm = new frmEquipmentSet(sereServADO.EQUIPMENT_SET_ID, sereServADO.EQUIPMENT_SET_ORDER);
                    frm.ShowDialog();
                    if (frm.success)
                    {
                        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                        sereServ.ID = sereServADO.ID;
                        sereServ.EQUIPMENT_SET_ID = frm.equipmentId;
                        sereServ.EQUIPMENT_SET_ORDER = frm.numOrder;
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        hisSereServPayslipSDO.Field = UpdateField.EQUIPMENT_SET_ORDER__AND__EQUIPMENT_SET_ID;
                        this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEditPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS_SERE_SERV vSereServ = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewBordereau.GetFocusedRow();
                if (vSereServ != null)
                {
                    HIS_SERE_SERV sereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(sereServ, vSereServ);

                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    if (edit == null) return;
                    if (edit.EditValue != null)
                    {
                        var pt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(false, true)
                            .FirstOrDefault(o =>
                                (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString())
                                || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                        if (pt != null)
                        {
                            sereServ.PRIMARY_PATIENT_TYPE_ID = pt.ID;
                            List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                            list.Add(sereServ);
                            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                            hisSereServPayslipSDO.Field = UpdateField.PRIMARY_PATIENT_TYPE_ID;
                            hisSereServPayslipSDO.SereServs = list;
                            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                            bool result = this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                            if (!result)
                            {
                                gridControlBordereau.BeginUpdate();
                                long oldPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                                gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                                //Load lai du lieu cu
                                foreach (var item in this.SereServADOs)
                                {
                                    if (item.ID == sereServ.ID)
                                    {
                                        item.PRIMARY_PATIENT_TYPE_ID = oldPatientTypeId;
                                        break;
                                    }
                                }
                                gridControlBordereau.RefreshDataSource();
                                gridControlBordereau.EndUpdate();
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
        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadLoginName(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoginName.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(false, true).FirstOrDefault(o => o.LOGINNAME == cboLoginName.EditValue.ToString());
                        if (data != null)
                        {
                            txtLoginName.Text = data.USERNAME;
                            txtLoginName.Focus();
                            FillDataToButtonPrint();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(false, true).FirstOrDefault(o => o.LOGINNAME == cboLoginName.EditValue.ToString());
                    if (data != null)
                    {
                        txtLoginName.Text = data.USERNAME;
                        txtLoginName.Focus();
                    }
                }
                else
                {
                    cboLoginName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //List<SereServADO> sereServADOTemps = this.SereServADOs.Where(o =>
                //    o.SERVICE_CODE.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                //    || o.SERVICE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                //    || o.SERVICE_TYPE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                //    ).ToList();
                //this.gridControlBordereau.DataSource = sereServADOTemps;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<SereServADO> sereServADODisplay = new List<SereServADO>();

                    List<SereServADO> sereServADOTemps = this.SereServADOs.Where(o =>
                        o.SERVICE_CODE.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                        || o.SERVICE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                        || o.SERVICE_TYPE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                        ).ToList();
                    if (sereServADOTemps != null && sereServADOTemps.Count > 0)
                    {
                        if (chkAmount.Checked)
                        {
                            sereServADOTemps = sereServADOTemps.Where(o => o.AMOUNT > 0).ToList();
                        }

                        sereServADODisplay.AddRange(sereServADOTemps);
                        if (chkAssignBlood.Checked)
                        {
                            HisExpMestBltyReqView2Filter ft = new HisExpMestBltyReqView2Filter();
                            ft.TDL_TREATMENT_ID = this.currentTreatment.ID;
                            ft.EXP_MEST_STT_IDs = new List<long> {
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE };
                            var dt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, ft, null);

                            foreach (var item in dt)
                            {
                                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                                SereServADO ado = new SereServADO(item, service);
                                sereServADODisplay.Add(ado);
                            }

                        }
                    }

                    this.gridControlBordereau.DataSource = sereServADODisplay;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                List<SereServADO> sereServADODisplay = new List<SereServADO>();

                List<SereServADO> sereServADOTemps = this.SereServADOs.Where(o =>
               o.SERVICE_CODE.ToUpper().Contains(this.txtKeyword.Text.ToUpper())
                || (!string.IsNullOrEmpty(o.SERVICE_NAME) && o.SERVICE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper()))
                || (!string.IsNullOrEmpty(o.SERVICE_TYPE_NAME) && o.SERVICE_TYPE_NAME.ToUpper().Contains(this.txtKeyword.Text.ToUpper()))
                || (!string.IsNullOrEmpty(o.TDL_SERVICE_REQ_CODE) && o.TDL_SERVICE_REQ_CODE.ToUpper().Contains(this.txtKeyword.Text.ToUpper()))
                || (!string.IsNullOrEmpty(o.TDL_HEIN_SERVICE_BHYT_CODE) && o.TDL_HEIN_SERVICE_BHYT_CODE.ToUpper().Contains(this.txtKeyword.Text.ToUpper()))).ToList();

                if (this.dtFrom.EditValue != null)
                {
                    long instructionFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime?(this.dtFrom.DateTime.AddHours(00).AddMinutes(00).AddSeconds(00))) ?? 0L;
                    sereServADOTemps = sereServADOTemps.Where(o => o.TDL_INTRUCTION_TIME >= instructionFrom).ToList();

                }
                if (this.dtTo.EditValue != null)
                {

                    long instructionTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime?(this.dtTo.DateTime.AddHours(23).AddMinutes(59).AddSeconds(59))) ?? 0L;
                    sereServADOTemps = sereServADOTemps.Where(o => o.TDL_INTRUCTION_TIME <= instructionTo).ToList();

                }

                if (sereServADOTemps != null && sereServADOTemps.Count > 0)
                {

                    if (chkAmount.Checked)
                    {
                        sereServADOTemps = sereServADOTemps.Where(o => o.AMOUNT > 0).ToList();
                    }

                    sereServADODisplay.AddRange(sereServADOTemps);
                    if (chkAssignBlood.Checked)
                    {
                        HisExpMestBltyReqView2Filter ft = new HisExpMestBltyReqView2Filter();
                        ft.TDL_TREATMENT_ID = this.currentTreatment.ID;
                        ft.EXP_MEST_STT_IDs = new List<long> {
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE };
                        var dt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, ft, null);

                        foreach (var item in dt)
                        {
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            SereServADO ado = new SereServADO(item, service);
                            sereServADODisplay.Add(ado);
                        }

                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("sereServADODisplay__:", sereServADODisplay));
                this.gridControlBordereau.DataSource = sereServADODisplay;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItemFind_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkAmount_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.ShowDropDown();
        }

        private void repositoryItembtnEditDonGia_TextDisable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServADO ssADO = (SereServADO)gridViewBordereau.GetFocusedRow();
                if (ssADO != null)
                {
                    frmPriceEdit frmPriceEdit = new frmPriceEdit(ssADO, UpdateSurgPrice, PriceEditType.EditTypeSurgPrice, GetPriceBypackage, GetPriceBySurg);
                    frmPriceEdit.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnEditGiaGoi_TextDisable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServADO ssADO = (SereServADO)gridViewBordereau.GetFocusedRow();
                if (ssADO != null)
                {
                    frmPriceEdit frmPriceEdit = new frmPriceEdit(ssADO, UpdatePackagePrice, PriceEditType.EditTypePackagePrice, GetPriceBypackage, GetPriceBySurg);
                    frmPriceEdit.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal? GetPriceBypackage(SereServADO sereServADOOld)
        {
            return sereServADOOld.PACKAGE_PRICE;
        }

        private decimal? GetPriceBySurg(SereServADO sereServADOOld)
        {
            return sereServADOOld.VIR_PRICE_NO_EXPEND;
        }

        void UpdateSurgPrice(SereServADO data)
        {
            try
            {
                if (this.gridViewBordereau.IsEditing)
                    this.gridViewBordereau.CloseEditor();

                if (this.gridViewBordereau.FocusedRowModified)
                    this.gridViewBordereau.UpdateCurrentRow();


                HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, data);
                sereServ.USER_PRICE = data.AssignSurgPriceEdit;
                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                hisSereServPayslipSDO.Field = UpdateField.USER_PRICE;
                this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);

                this.gridControlBordereau.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void UpdatePackagePrice(SereServADO data)
        {
            try
            {
                if (this.gridViewBordereau.IsEditing)
                    this.gridViewBordereau.CloseEditor();

                if (this.gridViewBordereau.FocusedRowModified)
                    this.gridViewBordereau.UpdateCurrentRow();


                HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, data);
                sereServ.PACKAGE_PRICE = data.AssignPackagePriceEdit;
                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                hisSereServPayslipSDO.SereServs = new List<HIS_SERE_SERV> { sereServ };
                hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                hisSereServPayslipSDO.Field = UpdateField.PACKAGE_PRICE;
                this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);

                this.gridControlBordereau.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int lastRowHandle = -1;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlBordereau)
                {
                    ToolTipDetail(gridControlBordereau, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ToolTipDetail(DevExpress.XtraGrid.GridControl gridControl, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle)
                    {
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        decimal? _amountTemp = (decimal?)view.GetRowCellValue(lastRowHandle, "AMOUNT_TEMP");
                        if (_amountTemp != null && _amountTemp > 0)
                        {
                            text = "Chi phí tạm tính";
                        }
                        lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_OtherPaySource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS_SERE_SERV vSereServ = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewBordereau.GetFocusedRow();
                if (vSereServ != null)
                {
                    HIS_SERE_SERV sereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(sereServ, vSereServ);

                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    if (edit == null) return;
                    if (edit.EditValue != null)
                    {
                        var pt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>(false, true)
                            .FirstOrDefault(o =>
                                (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString())
                                || o.OTHER_PAY_SOURCE_NAME == (edit.EditValue ?? "").ToString()));
                        if (pt != null)
                        {
                            sereServ.OTHER_PAY_SOURCE_ID = pt.ID;
                            List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                            list.Add(sereServ);
                            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                            hisSereServPayslipSDO.Field = UpdateField.OTHER_PAY_SOURCE_ID;
                            hisSereServPayslipSDO.SereServs = list;
                            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                            bool result = this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                            if (result)
                            {
                                gridControlBordereau.BeginUpdate();
                                long oldOtherPaySourceId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                                gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                                //Load lai du lieu cu
                                foreach (var item in this.SereServADOs)
                                {
                                    if (item.ID == sereServ.ID)
                                    {
                                        item.OTHER_PAY_SOURCE_ID = oldOtherPaySourceId;
                                        break;
                                    }
                                }
                                gridControlBordereau.RefreshDataSource();
                                gridControlBordereau.EndUpdate();
                            }
                            else
                            {
                                gridControlBordereau.BeginUpdate();
                                gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                                //Load lai du lieu cu
                                foreach (var item in this.SereServADOs)
                                {
                                    if (item.ID == sereServ.ID)
                                    {
                                        item.OTHER_PAY_SOURCE_ID = null;
                                        break;
                                    }
                                }
                                gridControlBordereau.RefreshDataSource();
                                gridControlBordereau.EndUpdate();
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

        private void repositoryItemGridLookUpEdit_OtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                try
                {
                    HIS_SERE_SERV vSereServ = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewBordereau.GetFocusedRow();
                    if (vSereServ != null)
                    {
                        HIS_SERE_SERV sereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(sereServ, vSereServ);

                        GridLookUpEdit edit = sender as GridLookUpEdit;
                        if (edit == null) return;

                        sereServ.OTHER_PAY_SOURCE_ID = null;
                        List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                        list.Add(sereServ);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.Field = UpdateField.OTHER_PAY_SOURCE_ID;
                        hisSereServPayslipSDO.SereServs = list;
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        bool result = this.UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                        if (!result)
                        {
                            gridControlBordereau.BeginUpdate();
                            long oldOtherPaySourceId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                            gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                            //Load lai du lieu cu
                            foreach (var item in this.SereServADOs)
                            {
                                if (item.ID == sereServ.ID)
                                {
                                    item.OTHER_PAY_SOURCE_ID = oldOtherPaySourceId;
                                    break;
                                }
                            }
                            gridControlBordereau.RefreshDataSource();
                            gridControlBordereau.EndUpdate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void btnFind_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayType.EditValue != null)
                    {
                        this.payOption = (PrintOption.PayType)cboPayType.EditValue;
                        FillDataToButtonPrint();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPayType.EditValue != null)
                    {
                        this.payOption = (PrintOption.PayType)cboPayType.EditValue;
                        FillDataToButtonPrint();
                    }
                }
                else
                {
                    cboPayType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.cboPayType && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboPayType.EditValue.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.cboPayType;
                    csAddOrUpdate.VALUE = cboPayType.EditValue.ToString();
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

                this.payOption = (PrintOption.PayType)cboPayType.EditValue;
                FillDataToButtonPrint();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHSBA_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboHSBA.EditValue != null)
                {
                    WaitingManager.Show();
                    treatmentId = Int32.Parse(cboHSBA.EditValue.ToString());

                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter fl = new HisTreatmentViewFilter();
                    fl.ID = treatmentId;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, fl, param);
                    if (data != null && data.Count > 0)
                    {
                        currentTreatment = data.FirstOrDefault();
                    }
                    bool check = currentTreatment.IS_PAUSE == 1 ? false : true;

                    //        DisableControl(check);


                    LoadServiceReqV2();
                    LoadTotalPriceDataToTestServiceReqV2();
                    LoadAndFillDataToReposPatientType();
                    LoadDataToBorderauAndPrintV2();
                    initLoadPakageTackV2();

                    this.LoadPatientInfo();

                    this.InitTreatmentFun();

                    LoadMedicineMaterial();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DisableControl(bool check)
        {
            try
            {
                gridViewBordereau.OptionsSelection.MultiSelect = check;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private bool IsContainString(string arrStr, string str)
        {
            bool result = false;
            try
            {
                if (arrStr.Contains(","))
                {
                    var arr = arrStr.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result = arr[i] == str;
                        if (result) break;
                    }
                }
                else
                {
                    result = arrStr == str;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewBordereau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                gridViewBordereau.RefreshRow(gridViewBordereau.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAssignBlood_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkAssignBlood && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkAssignBlood.Checked ? "1" : "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkAssignBlood;
                    csAddOrUpdate.VALUE = chkAssignBlood.Checked ? "1" : "";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                btnFind_Click(null, null);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_Condition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                SereServADO vSereServ = (SereServADO)gridViewBordereau.GetFocusedRow();
                if (vSereServ != null)
                {
                    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, vSereServ);

                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    if (edit == null) return;
                    if (edit.EditValue != null)
                    {
                        var pt = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString()) || o.SERVICE_CONDITION_NAME == (edit.EditValue ?? "").ToString());
                        if (pt != null)
                        {
                            sereServ.SERVICE_CONDITION_ID = pt.ID;
                            vSereServ.SERVICE_CONDITION_ID = pt.ID;
                            List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                            list.Add(sereServ);
                            HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                            hisSereServPayslipSDO.Field = UpdateField.SERVICE_CONDITION_ID;
                            hisSereServPayslipSDO.SereServs = list;
                            hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                            List<HIS_SERE_SERV> sereServResults = new List<HIS_SERE_SERV>();
                            if (vSereServ.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                || vSereServ.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || vSereServ.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                || vSereServ.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                            {
                                WaitingManager.Show();
                                CommonParam param = new CommonParam();
                                sereServResults = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, hisSereServPayslipSDO, param);
                                if (sereServResults != null && sereServResults.Count() > 0)
                                {
                                    success = true;
                                    ReloadDataToGridAndPrint(sereServResults);
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this, param, success);
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

        private void repositoryItemGridLookUpEdit_Condition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                try
                {
                    HIS_SERE_SERV vSereServ = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewBordereau.GetFocusedRow();
                    if (vSereServ != null)
                    {
                        HIS_SERE_SERV sereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(sereServ, vSereServ);

                        GridLookUpEdit edit = sender as GridLookUpEdit;
                        if (edit == null) return;

                        sereServ.SERVICE_CONDITION_ID = null;
                        List<HIS_SERE_SERV> list = new List<HIS_SERE_SERV>();
                        list.Add(sereServ);
                        HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                        hisSereServPayslipSDO.Field = UpdateField.SERVICE_CONDITION_ID;
                        hisSereServPayslipSDO.SereServs = list;
                        hisSereServPayslipSDO.TreatmentId = this.currentTreatment.ID;
                        List<HIS_SERE_SERV> sereServResults = new List<HIS_SERE_SERV>();
                        if (vSereServ.PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            sereServResults = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, hisSereServPayslipSDO, param);
                            if (sereServResults != null && sereServResults.Count() > 0)
                            {
                                success = true;
                                ReloadDataToGridAndPrint(sereServResults);
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                        }

                        if (sereServResults == null || sereServResults.Count() == 0)
                        {
                            gridControlBordereau.BeginUpdate();
                            long? oldConditionId = edit.OldEditValue != null ? (long?)Convert.ToInt64(edit.OldEditValue.ToString()) : null;
                            //gridViewBordereau.FocusedColumn = gridViewBordereau.Columns[1];
                            //Load lai du lieu cu
                            foreach (var item in this.SereServADOs)
                            {
                                if (item.ID == sereServ.ID)
                                {
                                    item.SERVICE_CONDITION_ID = oldConditionId;
                                    break;
                                }
                            }
                            gridControlBordereau.RefreshDataSource();
                            gridControlBordereau.EndUpdate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void picUpdateQR_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TransReqCreateSDO adoqr = new TransReqCreateSDO();
                adoqr.TreatmentId = currentTreatment.ID;
                adoqr.TransReqType = 3;
                adoqr.RequestRoomId = currentModule.RoomId;

                HIS_TRANS_REQ rs = new BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateSDO", ApiConsumers.MosConsumer, adoqr, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    MessageManager.Show(this, param, true);
                    //lblTotalDepositPrice.Text = 
                }
                else
                {
                    MessageManager.Show(this, param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
