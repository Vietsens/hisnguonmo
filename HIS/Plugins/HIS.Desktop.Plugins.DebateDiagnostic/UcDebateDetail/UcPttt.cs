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
using HIS.Desktop.Plugins.DebateDiagnostic.EkipTemp;
using HIS.Desktop.Plugins.DebateDiagnostic.ADO;
using DevExpress.XtraEditors;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Controls.EditorLoader;


namespace HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail
{
    public partial class UcPttt : UserControl
    {
        private List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        private List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        private int positionHandleControl = -1;
        private long treatmentId;
        private long _roomId;
        private long _roomTypeId;
        internal List<AcsUserADO> AcsUserADOList { get; set; }
        private List<HIS_PTTT_METHOD> ListPtttMethod { get; set; }
        List<HIS_EMOTIONLESS_METHOD> ListEmotionMethod { get; set; }
        public List<ACS_USER> UserList;
        public List<V_HIS_EMPLOYEE> EmployeeList;
        public List<HIS_DEPARTMENT> DepartmentList;
        public List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> ExecuteRoleList;
        public UcPttt(long treatmentId, long roomId, long roomTypeId, List<ACS_USER> UserList, List<V_HIS_EMPLOYEE> EmployeeList, List<HIS_DEPARTMENT> DepartmentList, List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> ExecuteRoleList)
        {
            InitializeComponent();
            this.treatmentId = treatmentId;
            this._roomId = roomId;
            this._roomTypeId = roomTypeId;
            this.UserList = UserList;
            this.EmployeeList = EmployeeList;
            this.DepartmentList = DepartmentList;
            this.ExecuteRoleList = ExecuteRoleList;
        }

        private void UcPttt_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ComboEkipTemp(CboEkipTemp);
                ComboExecuteRole();
                ComboAcsUser();
                LoadDataToComboDepartment();
                ComboEmotionlessMothod();
                ComboPPMethod();
                FillDataToInformationSurg();
                ValidationControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Không có thông tin kip thực hiện");
                    return;
                }

                frmEkipTemp frm = new frmEkipTemp(ekipUsers, RefeshDataEkipTemp);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                ComboEkipTemp(CboEkipTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboEkipTemp(GridLookUpEdit cbo)
        {
            try
            {
                Action myaction = () =>
                {
                    string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    CommonParam param = new CommonParam();
                    HisEkipTempFilter filter = new HisEkipTempFilter();
                    ekipTemps = new BackendAdapter(param).Get<List<HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);

                    if (ekipTemps != null && ekipTemps.Count > 0)
                    {
                        ekipTemps = ekipTemps.Where(o => o.IS_PUBLIC == 1 || o.CREATOR == logginName).OrderByDescending(o => o.CREATE_TIME).ToList();
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboEmotionlessMothod()
        {
            try
            {
                Action myaction = () =>
                {
                    ListEmotionMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();
                    ListEmotionMethod = ListEmotionMethod != null ? ListEmotionMethod.Where(p => p.IS_ACTIVE == 1 && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;
                };
                Task task = new Task(myaction);
                task.Start();

                await task;


                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(CboEmotionlessMethod, ListEmotionMethod, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboPPMethod()
        {
            try
            {
                Action myaction = () =>
                {
                    ListPtttMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(p => p.IS_ACTIVE == 1).ToList();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(CboMethod, ListPtttMethod, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboAcsUser()
        {
            try
            {
                Action myaction = () =>
                {
                    AcsUserADOList = ProcessAcsUser();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                ControlEditorLoader.Load(cbo_UseName, this.AcsUserADOList, controlEditorADO);

                repositoryItemSearchLookUpEdit1.DataSource = this.AcsUserADOList;
                repositoryItemSearchLookUpEdit1.DisplayMember = "USERNAME";
                repositoryItemSearchLookUpEdit1.ValueMember = "LOGINNAME";

                repositoryItemSearchLookUpEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemSearchLookUpEdit1.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemSearchLookUpEdit1.ImmediatePopup = true;
                repositoryItemSearchLookUpEdit1.View.Columns.Clear();

                GridColumn aColumnCode = repositoryItemSearchLookUpEdit1.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = repositoryItemSearchLookUpEdit1.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumnDepartment = repositoryItemSearchLookUpEdit1.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 3;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboExecuteRole()
        {
            try
            {
                List<HIS_EXECUTE_ROLE> datas = ExecuteRoleList;

                if (datas != null && datas.Count > 0)
                {
                    datas = datas.Where(p => p.IS_ACTIVE == 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPosition, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataToComboDepartment()
        {
            try
            {
                var departmentClinic = DepartmentList.Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(GridLookUpEdit_Department, departmentClinic, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<AcsUserADO> ProcessAcsUser()
        {
            List<AcsUserADO> result = null;
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = UserList.Where(o => o.IS_ACTIVE == 1).ToList();
                List<V_HIS_EMPLOYEE> employeeList = EmployeeList.Where(o => o.IS_ACTIVE == 1).ToList();
                var departmentList = DepartmentList.Where(o => o.IS_ACTIVE == 1).ToList();
                result = new List<AcsUserADO>();

                foreach (var item in employeeList)
                {
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;

                    var checkDepartment = departmentList.FirstOrDefault(o => o.ID == item.DEPARTMENT_ID);
                    if (checkDepartment != null)
                    {
                        user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;
                    }

                    var check = datas.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {
                        user.USERNAME = check.USERNAME;
                        user.MOBILE = check.MOBILE;
                        user.PASSWORD = check.PASSWORD;
                    }

                    result.Add(user);
                }

                result = result.OrderBy(o => o.USERNAME).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ValidationControl()
        {
            try
            {
                ValidationControlMaxLength(TxtKqCls, 4000, false);
                ValidationControlMaxLength(txtInternalMedicineState, 4000, false);
                ValidationControlMaxLength(TxtTreatmentTracking, 4000, false);
                ValidationControlMaxLength(TxtPrognosis, 4000, false);
                ValidationControlMaxLength(memCONCLUSION, 4000, false);
                ValidationControlMaxLength(memPathHis, 4000, false);
                ValidationControlMaxLength(memHosState, 2000, false);
                ValidationControlMaxLength(memBeforeDiagnostic, 2000, false);
                ValidationControlMaxLength(memDiscussion, 2000, false);
                ValidationControlMaxLength(memCareMethod, 2000, false);
                ValidationControlMaxLength(txtTreatmentMethod, 2000, false);
                ValidationControlMaxLength(memCONCLUSION, 2000, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtMethodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    CboMethod.Focus();
                    CboMethod.ShowPopup();
                }
                else
                {
                    var data = ListPtttMethod.Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            CboMethod.EditValue = data[0].ID;
                            checkEditMethod.Focus();
                            checkEditMethod.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                CboMethod.EditValue = search.ID;
                                checkEditMethod.Focus();
                                checkEditMethod.SelectAll();
                            }
                            else
                            {
                                CboMethod.EditValue = null;
                                CboMethod.Focus();
                                CboMethod.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        CboMethod.EditValue = null;
                        CboMethod.Focus();
                        CboMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboMethod_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboMethod.EditValue = null;
                    TxtMethodCode.Text = "";
                    txtPtttMethod.Text = "";
                    TxtMethodCode.Focus();
                    TxtMethodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (CboMethod.EditValue != null)
                    {
                        HIS_PTTT_METHOD data = ListPtttMethod.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboMethod.EditValue.ToString()));
                        if (data != null)
                        {
                            TxtMethodCode.Text = data.PTTT_METHOD_CODE;
                            txtPtttMethod.Text = data.PTTT_METHOD_NAME;
                            checkEditMethod.Focus();
                            checkEditMethod.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboMethod.ClosePopup();
                    if (CboMethod.EditValue != null)
                    {
                        HIS_PTTT_METHOD data = ListPtttMethod.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            TxtMethodCode.Text = data.PTTT_METHOD_CODE;
                            txtPtttMethod.Text = data.PTTT_METHOD_NAME;
                            checkEditMethod.Focus();
                            checkEditMethod.SelectAll();
                        }
                    }
                }
                //else
                //{
                //    CboMethod.ShowPopup();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadEmotionlessMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadEmotionlessMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    CboEmotionlessMethod.Focus();
                    CboEmotionlessMethod.ShowPopup();
                }
                else
                {
                    var data = ListEmotionMethod.Where(o => o.EMOTIONLESS_METHOD_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            CboEmotionlessMethod.EditValue = data[0].ID;
                            DtSurgeryTime.Focus();
                            DtSurgeryTime.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE.ToUpper() == searchCode);
                            if (search != null)
                            {
                                CboEmotionlessMethod.EditValue = search.ID;
                                DtSurgeryTime.Focus();
                                DtSurgeryTime.SelectAll();
                            }
                            else
                            {
                                CboEmotionlessMethod.EditValue = null;
                                CboEmotionlessMethod.Focus();
                                CboEmotionlessMethod.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        CboEmotionlessMethod.EditValue = null;
                        CboEmotionlessMethod.Focus();
                        CboEmotionlessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboEmotionlessMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboEmotionlessMethod.EditValue = null;
                    TxtEmotionlessMethod.Text = "";
                    TxtEmotionlessMethod.Focus();
                    TxtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboEmotionlessMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (CboEmotionlessMethod.EditValue != null)
                    {
                        HIS_EMOTIONLESS_METHOD data = ListEmotionMethod.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboEmotionlessMethod.EditValue.ToString()));
                        if (data != null)
                        {
                            TxtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            DtSurgeryTime.Focus();
                            DtSurgeryTime.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboEmotionlessMethod.EditValue != null)
                    {
                        HIS_EMOTIONLESS_METHOD data = ListEmotionMethod.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboEmotionlessMethod.EditValue.ToString()));
                        if (data != null)
                        {
                            TxtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            DtSurgeryTime.Focus();
                            DtSurgeryTime.SelectAll();
                        }
                    }
                }
                else
                {
                    CboEmotionlessMethod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtSurgeryTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (DtSurgeryTime.EditValue != null)
                {
                    DateTime dt = DtSurgeryTime.DateTime;
                    DtSurgeryTime.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                    TxtPrognosis.Focus();
                    TxtPrognosis.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtSurgeryTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtPrognosis.Focus();
                    TxtPrognosis.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnChooseCls_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(this.treatmentId);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                listArgs.Add(true);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ContentSubclinical", _roomId, _roomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //qtcode chú ý
        private void SelectDataResult(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("SelectDataResult: ", data));
                TxtKqCls.Text = "";
                if (data != null && data is List<HIS.Desktop.ADO.ContentSubclinicalADO>)
                {
                    List<HIS.Desktop.ADO.ContentSubclinicalADO> dienBien = data as List<HIS.Desktop.ADO.ContentSubclinicalADO>;
                    List<string> lstData = new List<string>();


                    var dbXetNghiem = dienBien.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                    var dbother = dienBien.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();

                    if (dbXetNghiem != null && dbXetNghiem.Count > 0)
                    {
                        var groupxn = dbXetNghiem.OrderBy(p => p.NUM_ORDER).GroupBy(s => s.SERVICE_ID).ToList();
                        foreach (var item in groupxn)
                        {
                            List<string> lstData1 = new List<string>();
                            List<HIS.Desktop.ADO.ContentSubclinicalADO> xn = item.ToList();
                            for (int i = 0; i < xn.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(xn[i].TDL_SERVICE_NAME.ToLower()) && !string.IsNullOrWhiteSpace(xn[i].TEST_INDEX_NAME) && xn[i].TDL_SERVICE_NAME.ToLower() == xn[i].TEST_INDEX_NAME.ToLower())
                                    {
                                        lstData1.Add(string.Format("{0}: {1} {2}", xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                    }
                                    else
                                    {
                                        lstData1.Add(string.Format("{0}: {1} {2} {3} ; ", xn[i].TDL_SERVICE_NAME, xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                    }
                                }
                                else
                                {
                                    lstData1.Add(string.Format("{0} {1} {2}", xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                }
                            }
                            lstData.Add(string.Join("; ", lstData1));
                        }
                    }

                    if (dbother != null && dbother.Count > 0)
                    {
                        foreach (var item in dbother)
                        {
                            lstData.Add(string.Format("{0}:{1}", item.TDL_SERVICE_NAME, item.VALUE));
                        }
                    }

                    TxtKqCls.Text = string.Join("\r\n", lstData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetSpaceByName(string p)
        {
            string result = " ";
            try
            {
                if (!string.IsNullOrWhiteSpace(p))
                {
                    for (int i = 0; i < p.Length; i++)
                    {
                        result += " ";
                    }
                }
            }
            catch (Exception ex)
            {
                result = " ";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                if (e.Column.FieldName == "LOGINNAME")
                {
                    this.grdControlInformationSurg.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "LOGINNAME")
                {
                    grdViewInformationSurg.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisEkipUserADO data = view.GetFocusedRow() as HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is Utilities.Extensions.CustomGridLookUpEdit)
                {
                    //SearchLookUpEdit editor = view.ActiveEditor as SearchLookUpEdit;
                    Utilities.Extensions.CustomGridLookUpEdit editor = view.ActiveEditor as Utilities.Extensions.CustomGridLookUpEdit;
                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        var executeRoleUserTemps = executeRoleUsers != null ? executeRoleUsers.Where(o => o.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList() : null;
                        if (executeRoleUserTemps != null && executeRoleUserTemps.Count > 0)
                        {
                            loginNames = executeRoleUserTemps.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }

                    ComboAcsUser(editor, loginNames);
                    grdViewInformationSurg.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(Utilities.Extensions.CustomGridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = this.AcsUserADOList;
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumnDepartment = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Tên khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 3;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
                else
                {
                    HisEkipUserADO participant = new HisEkipUserADO();
                    participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUsers.Add(participant);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);
                        grdControlInformationSurg.DataSource = null;
                        grdControlInformationSurg.DataSource = ekipUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbo_UseName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void WarningValidMessage()
        {
            try
            {
                var invalidControls = dxValidationProvider1.GetInvalidControls();
                List<string> listNameValid = new List<string>();
                if (invalidControls != null && invalidControls.Count > 0)
                {
                    List<MemoEdit> memoEdits = invalidControls.OfType<MemoEdit>().ToList();
                    if (memoEdits != null && memoEdits.Count > 0)
                    {
                        foreach (var item in memoEdits)
                        {
                            if (item.Name == txtInternalMedicineState.Name)
                                listNameValid.Add(xtraTabPage5.Text);
                            else if (item.Name == TxtTreatmentTracking.Name)
                                listNameValid.Add(xtraTabPage10.Text);
                            else if (item.Name == TxtPrognosis.Name)
                                listNameValid.Add(xtraTabPage2.Text);
                            else if (item.Name == memCONCLUSION.Name)
                                listNameValid.Add(xtraTabPage9.Text);
                            else if (item.Name == memPathHis.Name)
                                listNameValid.Add(xtraTabPage3.Text);
                            else if (item.Name == memHosState.Name)
                                listNameValid.Add(xtraTabPage4.Text);
                            else if (item.Name == memBeforeDiagnostic.Name)
                                listNameValid.Add(xtraTabPage6.Text);
                            else if (item.Name == memDiscussion.Name)
                                listNameValid.Add(xtraTabPage1.Text);
                            else if (item.Name == memCareMethod.Name)
                                listNameValid.Add(xtraTabPage8.Text);
                            else if (item.Name == txtTreatmentMethod.Name)
                                listNameValid.Add(xtraTabPage7.Text);
                        }
                    }

                    string warning = String.Join(", ", listNameValid);
                    if (!String.IsNullOrEmpty(warning))
                    {
                        XtraMessageBox.Show(warning + " vượt quá ký tự cho phép.", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal bool ValidControl()
        {
            bool result = false;
            try
            {
                this.positionHandleControl = -1;
                result = !dxValidationProvider1.Validate();
                WarningValidMessage();
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal void DisableControlItem()
        {
            try
            {
                TxtEmotionlessMethod.ReadOnly = true;
                txtInternalMedicineState.ReadOnly = true;
                TxtKqCls.ReadOnly = true;
                TxtTreatmentTracking.ReadOnly = true;
                TxtMethodCode.ReadOnly = true;
                TxtPrognosis.ReadOnly = true;
                txtTreatmentMethod.ReadOnly = true;
                memCONCLUSION.ReadOnly = true;
                CboEmotionlessMethod.ReadOnly = true;
                CboEkipTemp.ReadOnly = true;
                CboMethod.ReadOnly = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void GetData(ref HIS_DEBATE saveData)
        {
            try
            {
                if (saveData == null) saveData = new HIS_DEBATE();

                saveData.SUBCLINICAL_PROCESSES = TxtKqCls.Text;
                saveData.INTERNAL_MEDICINE_STATE = txtInternalMedicineState.Text;
                saveData.TREATMENT_TRACKING = TxtTreatmentTracking.Text;
                saveData.TREATMENT_METHOD = txtTreatmentMethod.Text;

                saveData.PTTT_METHOD_ID = null;
                if (CboMethod.EditValue != null)
                {
                    saveData.PTTT_METHOD_ID = long.Parse(CboMethod.EditValue.ToString());
                }

                if (checkEditMethod.Checked)
                    saveData.PTTT_METHOD_NAME = txtPtttMethod.Text;
                else
                    saveData.PTTT_METHOD_NAME = CboMethod.Text;

                saveData.EMOTIONLESS_METHOD_ID = null;
                if (CboEmotionlessMethod.EditValue != null)
                {
                    saveData.EMOTIONLESS_METHOD_ID = long.Parse(CboEmotionlessMethod.EditValue.ToString());
                }

                saveData.SURGERY_TIME = null;
                if (DtSurgeryTime.EditValue != null && DtSurgeryTime.DateTime != DateTime.MinValue)
                {
                    saveData.SURGERY_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((DtSurgeryTime.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
                }

                saveData.PROGNOSIS = TxtPrognosis.Text;
                if (!string.IsNullOrEmpty(memCONCLUSION.Text))
                {
                    saveData.CONCLUSION = memCONCLUSION.Text;
                }
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_EKIP_USER> lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_EKIP_USER>();
                List<ADO.HisEkipUserADO> lstDebateUser = grdControlInformationSurg.DataSource as List<ADO.HisEkipUserADO>;
                foreach (var item_DebateUser in lstDebateUser)
                {
                    MOS.EFMODEL.DataModels.HIS_DEBATE_EKIP_USER hisDebateUser = new MOS.EFMODEL.DataModels.HIS_DEBATE_EKIP_USER();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE_EKIP_USER>(hisDebateUser, item_DebateUser);
                    var name = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == item_DebateUser.LOGINNAME);
                    if (name != null)
                    {
                        hisDebateUser.LOGINNAME = name.LOGINNAME;
                        if (!string.IsNullOrEmpty(name.USERNAME))
                        {
                            hisDebateUser.USERNAME = name.USERNAME;
                        }

                        lstHisDebateUser.Add(hisDebateUser);
                    }
                }

                if (lstHisDebateUser.Count > 0)
                {
                    saveData.HIS_DEBATE_EKIP_USER = lstHisDebateUser;
                }
                else
                {
                    saveData.HIS_DEBATE_EKIP_USER = null;
                }
                saveData.PATHOLOGICAL_HISTORY = memPathHis.Text.Trim();
                saveData.HOSPITALIZATION_STATE = memHosState.Text.Trim();
                saveData.BEFORE_DIAGNOSTIC = memBeforeDiagnostic.Text.Trim();
                saveData.DISCUSSION = memDiscussion.Text.Trim();
                saveData.CARE_METHOD = memCareMethod.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetData(object data)
        {
            try
            {
                if (data != null)
                {
                    if (data.GetType() == typeof(HIS_DEBATE))
                    {
                        LoadDataDebateDiagnostic((HIS_DEBATE)data);
                    }
                    else if (data.GetType() == typeof(HIS_SERVICE_REQ))
                    {
                        LoadDataDebateDiagnostic((HIS_SERVICE_REQ)data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisDebate)
        {
            try
            {
                
                memPathHis.Text = hisDebate.PATHOLOGICAL_HISTORY;
                string hospitalizationState = hisDebate.FULL_EXAM + "\r\n" + hisDebate.PART_EXAM + "\r\n" + hisDebate.SUBCLINICAL;
                memHosState.Text = hospitalizationState.Trim();
                TxtTreatmentTracking.Text = hisDebate.PATHOLOGICAL_PROCESS;
                txtTreatmentMethod.Text = hisDebate.TREATMENT_INSTRUCTION;
                memCONCLUSION.Text = hisDebate.NEXT_TREATMENT_INSTRUCTION;

                if (!string.IsNullOrEmpty(hisDebate.ICD_CODE))
                {
                    var icd = Base.GlobalStore.HisIcds.Where(o => o.ICD_CODE == hisDebate.ICD_CODE).FirstOrDefault();
                    memBeforeDiagnostic.Text = icd.ICD_NAME;
                }
                else
                {
                    memBeforeDiagnostic.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            try
            {
                if (hisDebate != null)
                {
                    memPathHis.Text = hisDebate.PATHOLOGICAL_HISTORY;
                    memHosState.Text = hisDebate.HOSPITALIZATION_STATE;
                    memBeforeDiagnostic.Text = hisDebate.BEFORE_DIAGNOSTIC;
                    memDiscussion.Text = hisDebate.DISCUSSION;
                    memCareMethod.Text = hisDebate.CARE_METHOD;   
                    TxtKqCls.Text = hisDebate.SUBCLINICAL_PROCESSES;
                    txtInternalMedicineState.Text = hisDebate.INTERNAL_MEDICINE_STATE;
                    TxtTreatmentTracking.Text = hisDebate.TREATMENT_TRACKING;
                    TxtPrognosis.Text = hisDebate.PROGNOSIS;
                    txtTreatmentMethod.Text = hisDebate.TREATMENT_METHOD;
                    if (!string.IsNullOrEmpty(hisDebate.CONCLUSION))
                    {
                        memCONCLUSION.Text = hisDebate.CONCLUSION;
                    }
                    memCONCLUSION.Text = hisDebate.CONCLUSION;
                    DtSurgeryTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.SURGERY_TIME ?? 0);

                    CboMethod.EditValue = hisDebate.PTTT_METHOD_ID;
                    var service = ListPtttMethod.FirstOrDefault(o => o.ID == hisDebate.PTTT_METHOD_ID);
                    if (service != null)
                    {
                        TxtMethodCode.Text = service.PTTT_METHOD_CODE;
                        if ((hisDebate.PTTT_METHOD_NAME ?? " ").ToLower() != (service.PTTT_METHOD_CODE ?? " ").ToLower())
                        {
                            checkEditMethod.Checked = true;
                            txtPtttMethod.Text = hisDebate.PTTT_METHOD_NAME;
                        }
                        else
                        {
                            checkEditMethod.Checked = false;
                        }
                    }

                    CboEmotionlessMethod.EditValue = hisDebate.EMOTIONLESS_METHOD_ID;
                    var method = ListEmotionMethod.FirstOrDefault(o => o.ID == hisDebate.EMOTIONLESS_METHOD_ID);
                    if (method != null)
                    {
                        TxtEmotionlessMethod.Text = method.EMOTIONLESS_METHOD_CODE;
                    }

                    LoadGridEkipUserFromDebate(hisDebate.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGridEkipUserFromDebate(long debateId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDebateEkipUserFilter filter = new HisDebateEkipUserFilter();
                filter.DEBATE_ID = debateId;
                List<HIS_DEBATE_EKIP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<HIS_DEBATE_EKIP_USER>>("api/HisDebateEkipUser/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == ekipTempUser.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        ekipUserAdoTemp.DEPARTMENT_ID = ekipTempUser.DEPARTMENT_ID;
                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }

                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == ekipTempUser.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }

                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal async Task FillDataToInformationSurg()
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();

                Action myaction = () =>
                {
                    string executeRoleDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.EXECUTE_ROLE_DEFAULT");
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRoles = ExecuteRoleList;

                    if (!String.IsNullOrEmpty(executeRoleDefault))
                    {
                        string[] str = executeRoleDefault.Split(',');
                        List<string> executeRoleCodes = new List<string>(str);

                        foreach (var item in executeRoleCodes)
                        {
                            var executeRoleCheck = executeRoles.FirstOrDefault(o => o.EXECUTE_ROLE_CODE == item);
                            if (executeRoleCheck == null)
                                continue;

                            var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == executeRoleCheck.ID && p.IS_ACTIVE == 1);
                            if (dataCheck == null || dataCheck.ID == 0)
                                continue;
                            HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                            ekipUserAdoTemp.EXECUTE_ROLE_ID = executeRoleCheck.ID;

                            if (ekipUserAdoTemps.Count == 0)
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                            else
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    else
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    if (ekipUserAdoTemps == null || ekipUserAdoTemps.Count == 0)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (grdControlInformationSurg.DataSource == null) //146290
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboEkipTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (CboEkipTemp.EditValue != null)
                    {
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboEkipTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboEkipTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        CboEkipTemp.EditValue = null;
                    }
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

        private void CboMethod_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(CboMethod.Text))
                {
                    CboMethod.EditValue = null;
                    txtPtttMethod.Text = "";
                    checkEditMethod.Checked = false;
                }
                else
                {
                    txtPtttMethod.Text = CboMethod.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkEditMethod_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEditMethod.Checked == true)
                {
                    TxtMethodCode.Enabled = false;
                    CboMethod.Visible = false;
                    txtPtttMethod.Visible = true;
                    txtPtttMethod.Text = CboMethod.Text;
                    txtPtttMethod.Focus();
                    txtPtttMethod.SelectAll();
                }
                else if (checkEditMethod.Checked == false)
                {
                    TxtMethodCode.Enabled = true;
                    txtPtttMethod.Visible = false;
                    CboMethod.Visible = true;
                    txtPtttMethod.Text = CboMethod.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkEditMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtEmotionlessMethod.Focus();
                    TxtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPtttMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    checkEditMethod.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtPrognosis_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentMethod.Focus();
                    txtTreatmentMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
