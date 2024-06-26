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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Security.Cryptography;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.EkipTemp;
using HIS.Desktop.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Config;
using MOS.SDO;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Utility;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : UserControlBase
    {
        public void FillDataToGrid(List<HisEkipUserADO> lst)
        {
            try
            {
                if (lst != null && lst.Count > 0)
                {
                    int index = 0;
                    lst.ForEach(o =>
                    {
                        o.IsMinus = true;
                        if (index == 0)
                            o.IsPlus = true;
                        else
                            o.IsPlus = false;
                        index++;
                    });
                }
                hisEkipUserADOs = lst;
                grdControlInformationSurg.DataSource = new List<HisEkipUserADO>();
                grdControlInformationSurg.DataSource = hisEkipUserADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void FillDataToGridDepartment()
        {
            try
            {
                var dataEkipList = (List<HisEkipUserADO>)grdControlInformationSurg.DataSource;
                if (dataEkipList != null && dataEkipList.Count > 0)
                {
                    Parallel.ForEach(dataEkipList.Where(f => f.ID >= 0), l => l.DEPARTMENT_ID = this.DepartmentId);
                    FillDataToGrid(dataEkipList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridView2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();

                if (e.Column.FieldName == "LOGINNAME")
                {
                    SetDepartment(data);
                    this.grdControlInformationSurg.RefreshDataSource();
                }
                else if (e.Column.FieldName == "EXECUTE_ROLE_ID")
                {
                    int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                    int newVisibleIndex = visibleIndex + 1;
                    grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDepartment(HisEkipUserADO data)
        {
            try
            {
                if (data == null || (data.DEPARTMENT_ID.HasValue && data.DEPARTMENT_ID.Value > 0))
                    return;
                if (DepartmentId != null && DepartmentId > 0)
                {
                    data.DEPARTMENT_ID = DepartmentId;

                }
                else
                {
                    data.DEPARTMENT_ID = null;
                    data.DEPARTMENT_NAME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboDepartment.EditValue != null)
                {
                    DepartmentId = Int64.Parse(cboDepartment.EditValue.ToString());
                    dteStart.Focus();
                }
                else
                {
                    DepartmentId = null;
                }
                FillDataToGridDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void grdViewInformationSurg_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnAdd")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    bool IsPlus = Inventec.Common.TypeConvert.Parse.ToBoolean((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "IsPlus") ?? "").ToString());
                    if (IsPlus)
                    {
                        e.RepositoryItem = repPlus;
                    }
                }
                else if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    bool IsPlus = Inventec.Common.TypeConvert.Parse.ToBoolean((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "IsMinus") ?? "").ToString());
                    if (IsPlus)
                    {
                        e.RepositoryItem = repMinus;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status && o.IS_ACTIVE == 1);
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

        private void grdViewInformationSurg_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
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

        private void grdViewInformationSurg_ShownEditor(object sender, EventArgs e)
        {

            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisEkipUserADO data = view.GetFocusedRow() as HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
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
                    SetDepartment(data);
                    grdViewInformationSurg.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repPlus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;

                HisEkipUserADO participant = new HisEkipUserADO();
                ekipUsers.Add(participant);
                grdControlInformationSurg.DataSource = null;
                int index = 0;
                ekipUsers.ForEach(o =>
                {
                    o.IsMinus = true;
                    if (index == 0)
                        o.IsPlus = true;
                    else
                        o.IsPlus = false;
                    index++;
                });
                grdControlInformationSurg.DataSource = ekipUsers;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repMinus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 1)
                    {
                        ekipUsers.Remove(ekipUser);
                    }
                    else if (ekipUsers.Count == 1)
                    {
                        ekipUsers = new List<HisEkipUserADO>();
                        ekipUsers.Add(new HisEkipUserADO());
                    }
                    FillDataToGrid(ekipUsers);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveEkip_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasInvalid = (hisEkipUserADOs == null || hisEkipUserADOs.Count == 0 || hisEkipUserADOs
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0).FirstOrDefault() != null) ? true : false;
                if (hasInvalid)
                {
                    XtraMessageBox.Show("Vui lòng nhập thông tin kíp thực hiện");
                    return;
                }

                var groupLoginname = hisEkipUserADOs.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                if (groupLoginname != null && groupLoginname.Count > 0)
                {
                    List<string> messError = new List<string>();
                    foreach (var item in groupLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        XtraMessageBox.Show(string.Join("\n", messError), "Thông báo");
                        return;
                    }
                }

                frmEkipTemp frm = new frmEkipTemp(hisEkipUserADOs, () =>
                {
                    InitDataEkipTempCombo();
                    InitCombo(cboEkipUser, "ID", "EKIP_TEMP_NAME", "ID", EkipTempList);
                }, this.moduleData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboDepartment.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboPtttMethod_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboPtttMethod.EditValue != null)
                {
                    var dta = PtttMethodList.FirstOrDefault(o => o.ID == Int64.Parse(cboPtttMethod.EditValue.ToString()));
                    if (dta != null)
                        txtPtttMethod.Text = dta.PTTT_METHOD_CODE;
                    txtEmotionLessMethod.Focus();
                    txtEmotionLessMethod.SelectAll();
                }
                else
                {
                    txtPtttMethod.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboPtttMethod_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboPtttMethod.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEmotionLessMethod_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboEmotionLessMethod.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEmotionLessMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEmotionLessMethod.EditValue != null)
                {
                    var dta = EmotionLessMethodList.FirstOrDefault(o => o.ID == Int64.Parse(cboEmotionLessMethod.EditValue.ToString()));
                    if (dta != null)
                        txtEmotionLessMethod.Text = dta.EMOTIONLESS_METHOD_CODE;
                    txtPtttMethodReal.Focus();
                    txtPtttMethodReal.SelectAll();
                }
                else
                {
                    txtEmotionLessMethod.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttMethodReal_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboEmotionLessMethod.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttMethodReal_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttMethodReal.EditValue != null)
                {
                    var dta = PtttMethodRealList.FirstOrDefault(o => o.ID == Int64.Parse(cboPtttMethodReal.EditValue.ToString()));
                    if (dta != null)
                        txtPtttMethodReal.Text = dta.PTTT_METHOD_CODE;
                    txtPtttGroup.Focus();
                    txtPtttGroup.SelectAll();
                }
                else
                {
                    txtPtttMethodReal.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboPtttGroup.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttGroup.EditValue != null)
                {
                    var dta = PtttGroupList.FirstOrDefault(o => o.ID == Int64.Parse(cboPtttGroup.EditValue.ToString()));
                    if (dta != null)
                        txtPtttGroup.Text = dta.PTTT_GROUP_CODE;
                    cboEkipUser.Focus();
                    cboEkipUser.ShowPopup();
                }
                else
                {
                    txtPtttGroup.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEkipUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboEkipUser.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEkipUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (cboEkipUser.EditValue != null)
                {
                    var data = this.EkipTempList.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipUser.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        LoadGridEkipUserFromTemp(data.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<string> loginNames = ekipTempUsers.Select(o => o.LOGINNAME).ToList();
                    List<ACS.EFMODEL.DataModels.ACS_USER> isActive = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => loginNames.Exists(p => p == o.LOGINNAME)).ToList();
                    List<string> isActiveLoginName = isActive.Where(o => o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
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
                        if (isActiveLoginName.Contains(ekipTempUser.LOGINNAME))
                        {
                            SetDepartment(ekipUserAdoTemp);
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                }
                if (ekipUserAdoTemps == null || ekipUserAdoTemps.Count == 0)
                    ekipUserAdoTemps.Add(new HisEkipUserADO());
                FillDataToGrid(ekipUserAdoTemps);
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
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttMethod.Text.Trim()))
                    {
                        var dta = PtttMethodList.FirstOrDefault(o => o.PTTT_METHOD_CODE == txtPtttMethod.Text.Trim());
                        if (dta != null)
                            cboPtttMethod.EditValue = dta.ID;
                        else
                        {
                            cboPtttMethod.Focus();
                            cboPtttMethod.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPtttMethod.Focus();
                        cboPtttMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void txtEmotionLessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtEmotionLessMethod.Text.Trim()))
                    {
                        var dta = EmotionLessMethodList.FirstOrDefault(o => o.EMOTIONLESS_METHOD_CODE == txtEmotionLessMethod.Text.Trim());
                        if (dta != null)
                            cboEmotionLessMethod.EditValue = dta.ID;
                        else
                        {
                            cboEmotionLessMethod.Focus();
                            cboEmotionLessMethod.ShowPopup();
                        }
                    }
                    else
                    {
                        cboEmotionLessMethod.Focus();
                        cboEmotionLessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttMethodReal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttMethodReal.Text.Trim()))
                    {
                        var dta = PtttMethodRealList.FirstOrDefault(o => o.PTTT_METHOD_CODE == txtPtttMethodReal.Text.Trim());
                        if (dta != null)
                            cboPtttMethodReal.EditValue = dta.ID;
                        else
                        {
                            cboPtttMethodReal.Focus();
                            cboPtttMethodReal.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPtttMethodReal.Focus();
                        cboPtttMethodReal.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtPtttGroup.Text.Trim()))
                    {
                        var dta = PtttGroupList.FirstOrDefault(o => o.PTTT_GROUP_CODE == txtPtttGroup.Text.Trim());
                        if (dta != null)
                            cboPtttGroup.EditValue = dta.ID;
                        else
                        {
                            cboPtttGroup.Focus();
                            cboPtttGroup.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPtttGroup.Focus();
                        cboPtttGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdControlInformationSurg_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdViewInformationSurg.FocusedColumn.VisibleIndex != 0)
                {
                    if (grdViewInformationSurg.FocusedColumn.VisibleIndex == grdViewInformationSurg.VisibleColumns.Count - 1)
                    {
                        var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                        if (data.IsPlus)
                        {
                            repPlus_ButtonClick(null, null);
                            grdViewInformationSurg.FocusedRowHandle = grdViewInformationSurg.DataRowCount - 1;
                            int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                            int newVisibleIndex = visibleIndex + 1;
                            if (newVisibleIndex == grdViewInformationSurg.VisibleColumns.Count)
                                newVisibleIndex = 0;
                            grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                        }
                    }
                    else if (grdViewInformationSurg.FocusedColumn.VisibleIndex == grdViewInformationSurg.VisibleColumns.Count - 2)
                    {
                        repMinus_ButtonClick(null, null);
                    }
                    else
                    {
                        int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                        int newVisibleIndex = visibleIndex + 1;
                        if (newVisibleIndex == grdViewInformationSurg.VisibleColumns.Count)
                            newVisibleIndex = 0;
                        grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                    }
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            bool valid = true;
            try
            {
                if (HisConfigCFG.IsNotRequiredPtttExecuteRole && !CheckCountEkipUser())
                {
                    XtraMessageBox.Show("Vui lòng nhập kíp thực hiện", "Thông báo");
                    return;
                }
                if (!CheckAccountWithRole()) return;
                this.positionHandle = -1;
                valid = valid && dxValidationProviderEditorInfo.Validate();
                valid = valid && ((this.isAllowEditInfo && this.isStartTimeMustBeGreaterThanInstructionTime) ? this.ValidStartDatePTTT() : true);
                valid = valid && ValidateHisService_MaxTotalProcessTime();
                if (!valid) return;
                HisSurgServiceReqUpdateSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateSDO();
                SurgUpdateSDO singleData = new SurgUpdateSDO();
                singleData.SereServPttt = new HIS_SERE_SERV_PTTT();
                singleData.SereServExt = new HIS_SERE_SERV_EXT();
                singleData.EkipUsers = new List<HIS_EKIP_USER>();
                var ekipUserCheck = ProcessEkipUser(singleData);
                if (!ekipUserCheck)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu ekip trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                WaitingManager.Show();
                hisSurgResultSDO.SereServId = this.currentRow.ID;
                if (singleData.EkipUsers != null)
                    hisSurgResultSDO.EkipUsers = singleData.EkipUsers;
                HIS_SERE_SERV_PTTT pttt = new HIS_SERE_SERV_PTTT();
                pttt.SERE_SERV_ID = currentRow.ID;
                pttt.TDL_TREATMENT_ID = currentRow.TDL_TREATMENT_ID;
                pttt.PTTT_GROUP_ID = cboPtttGroup.EditValue != null ? ToNullableLong(cboPtttGroup.EditValue.ToString()) : null;
                pttt.PTTT_METHOD_ID = cboPtttMethod.EditValue != null ? ToNullableLong(cboPtttMethod.EditValue.ToString()) : null;
                pttt.REAL_PTTT_METHOD_ID = cboPtttMethodReal.EditValue != null ? ToNullableLong(cboPtttMethodReal.EditValue.ToString()) : null;
                pttt.EMOTIONLESS_METHOD_SECOND_ID = cboEmotionLessMethod.EditValue != null ? ToNullableLong(cboEmotionLessMethod.EditValue.ToString()) : null;
                hisSurgResultSDO.SereServPttt = pttt;
                HIS_SERE_SERV_EXT ext = new HIS_SERE_SERV_EXT();
                ext.SERE_SERV_ID = currentRow.ID;
                ext.BEGIN_TIME = dteStart.EditValue != null && dteStart.DateTime != DateTime.MinValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteStart.DateTime) : null;
                ext.END_TIME = dteFinish.EditValue != null && dteFinish.DateTime != DateTime.MinValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteFinish.DateTime) : null;
                hisSurgResultSDO.SereServExt = ext;
                hisSurgResultSDO.IsFinished = true;
                CommonParam param = new CommonParam();
                WaitingManager.Hide();
                if (this.ValidateData(hisSurgResultSDO))
                {
                    WaitingManager.Show();
                    currentHisSurgResultSDO = new BackendAdapter(param)
                   .Post<MOS.SDO.HisSurgServiceReqUpdateSDO>("api/HisServiceReq/SurgUpdate", ApiConsumers.MosConsumer, hisSurgResultSDO, param);
                    WaitingManager.Hide();
                    if (currentHisSurgResultSDO != null)
                    {
                        success = true;
                        btnSearch.PerformClick();
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private bool ValidateData(HisSurgServiceReqUpdateSDO hisSuimResultSDO)
        {
            bool result = false;
            try
            {

                if (hisSuimResultSDO != null && hisSuimResultSDO.EkipUsers != null && hisSuimResultSDO.EkipUsers.Count > 0)
                {
                    List<HIS_EKIP_USER> hasInvalid = hisSuimResultSDO.EkipUsers
                        .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                            || o.EXECUTE_ROLE_ID <= 0).Distinct().ToList();
                    if (hasInvalid != null && hasInvalid.Count > 0)
                    {
                        List<HIS_EXECUTE_ROLE> datas = null;
                        if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                        {
                            datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                        }
                        else
                        {
                            CommonParam paramCommon = new CommonParam();
                            dynamic filter = new System.Dynamic.ExpandoObject();
                            datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                            if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                        }
                        var executeRoleNull = datas.Where(o => hasInvalid.Select(p => p.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                        if (executeRoleNull == null)
                            result = true;

                        if (this.currentHisService != null && this.currentHisService.IS_OUT_OF_MANAGEMENT == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            string mess = String.Format("Bạn chưa nhập thông tin tương ứng với (các) vai trò: {0}. Bạn có muốn thực hiện không?", String.Join(", ", executeRoleNull.Select(o => o.EXECUTE_ROLE_NAME).ToArray()));

                            if (MessageBox.Show(mess, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return false;
                            else
                                result = true;
                        }

                    }

                    List<string> messError = new List<string>();
                    var grLoginname = hisSuimResultSDO.EkipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                    foreach (var item in grLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        MessageBox.Show(string.Join("\n", messError), "Thông báo");
                        return false;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private long? ToNullableLong(string s)
        {
            long i;
            if (long.TryParse(s, out i)) return i;
            return null;
        }
        private bool ProcessEkipUser(SurgUpdateSDO hisSurgResultSDO)
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                if (hisEkipUserADOs != null && hisEkipUserADOs.Count > 0)
                {
                    foreach (var item in hisEkipUserADOs)
                    {

                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = AcsUserADOList.SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;
                            ekipUsers.Add(ekipUser);
                        }
                    }
                }

                var groupEkipUser = ekipUsers.GroupBy(x => new { x.LOGINNAME, x.EXECUTE_ROLE_ID });
                foreach (var item in groupEkipUser)
                {
                    if (item.Count() >= 2)
                    {
                        return false;
                    }
                }
                hisSurgResultSDO.EkipUsers = ekipUsers;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool ValidStartDatePTTT()
        {
            bool valid = true;
            try
            {
                var dtIntructime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentRow.INTRUCTION_TIME);

                if (dteStart.EditValue != null)
                {
                    if (dtIntructime != null && Int64.Parse(dtIntructime.Value.ToString("yyyyMMddHHmm")) > Int64.Parse(dteStart.DateTime.ToString("yyyyMMddHHmm")))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian bắt đầu phải lớn hơn thời gian y lệnh", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                    }
                }

                if (dteFinish.EditValue != null)
                {
                    if (dtIntructime != null && Int64.Parse(dtIntructime.Value.ToString("yyyyMMddHHmm")) > Int64.Parse(dteFinish.DateTime.ToString("yyyyMMddHHmm")))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian kết thúc không được nhỏ hơn thời gian y lệnh", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private bool ValidateHisService_MaxTotalProcessTime()
        {
            bool valid = true;
            try
            {
                if (Config.HisConfigCFG.ProcessTimeMustBeLessThanMaxTotalProcessTime != "1"
                    && Config.HisConfigCFG.ProcessTimeMustBeLessThanMaxTotalProcessTime != "2")
                    return valid;
                var intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentRow.INTRUCTION_TIME);
                if (intructionTime == null)
                    return valid;
                var intructionTime_ToMinute = intructionTime.Value.AddSeconds(-intructionTime.Value.Second);
                TimeSpan processTime = (dteFinish.DateTime - intructionTime_ToMinute);
                Dictionary<long, List<V_HIS_SERVICE>> dicInvalidServices = new Dictionary<long, List<V_HIS_SERVICE>>();

                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == this.currentRow.SERVICE_ID);
                if (service != null && service.MAX_TOTAL_PROCESS_TIME.HasValue && service.MAX_TOTAL_PROCESS_TIME.Value > 0
                        && processTime.TotalMinutes > service.MAX_TOTAL_PROCESS_TIME.Value && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || !("," + service.TOTAL_TIME_EXCEPT_PATY_IDS + ",").Contains("," + currentRow.PATIENT_TYPE_ID.ToString() + ",")))
                {
                    dicInvalidServices.Add(service.MAX_TOTAL_PROCESS_TIME.Value, new List<V_HIS_SERVICE>() { service });
                }

                if (dicInvalidServices.Count > 0)
                {
                    string message = "";
                    List<string> listmsg = new List<string>();
                    if (Config.HisConfigCFG.ProcessTimeMustBeLessThanMaxTotalProcessTime == "1")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format("Không cho phép trả kết quả dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}"
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentRow.INTRUCTION_TIME));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        valid = false;
                        XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (Config.HisConfigCFG.ProcessTimeMustBeLessThanMaxTotalProcessTime == "2")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format("Không cho phép trả kết quả dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}"
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentRow.INTRUCTION_TIME));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        if (XtraMessageBox.Show(String.Format("{0} {1}", message, ". Bạn có muốn tiếp tục không?"), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private bool CheckCountEkipUser()
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                if (hisEkipUserADOs != null && hisEkipUserADOs.Count() > 0)
                    foreach (var item in hisEkipUserADOs)
                    {
                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);
                        if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                            ekipUsers.Add(ekipUser);
                    }
                if (ekipUsers.Count == 0)
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CheckAccountWithRole()
        {
            bool result = true;
            List<string> mess = new List<string>();
            try
            {
                if (hisEkipUserADOs != null && hisEkipUserADOs.Count() > 0)
                {
                    var lstRole = hisEkipUserADOs.Select(o => o.EXECUTE_ROLE_ID).Distinct().ToList();
                    foreach (var item in lstRole)
                    {
                        var role = ExecuteRoleList.FirstOrDefault(o => o.ID == item);
                        var users = hisEkipUserADOs.Where(o => o.EXECUTE_ROLE_ID == item);
                        if (role != null && role.IS_SINGLE_IN_EKIP == 1 && users != null && users.Count() > 1)
                        {
                            mess.Add(role.EXECUTE_ROLE_NAME);
                        }
                    }
                    if (mess.Count() > 0)
                    {
                        result = false;
                        MessageBox.Show(String.Format("Không được phép nhập nhiều hơn 1 tài khoản đối với vai trò {0}", string.Join(",", mess), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void cboService_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.serviceSelecteds != null && this.serviceSelecteds.Count > 0)
                {
                    foreach (var item in this.serviceSelecteds)
                    {
                        roomName += item.SERVICE_NAME + "; ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboPtttMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            cboPtttMethod_EditValueChanged(null, null);
        }

        private void cboEmotionLessMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            cboEmotionLessMethod_EditValueChanged(null, null);
        }

        private void cboPtttMethodReal_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            cboPtttMethodReal_EditValueChanged(null, null);
        }

        private void cboPtttGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            cboPtttGroup_EditValueChanged(null, null);
        }

        private void repUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];
                var loginName = grdViewInformationSurg.GetFocusedRowCellValue("LOGINNAME");
                var data = AcsUserADOList.Where(o => o.LOGINNAME.Equals(loginName)).ToList();
                if (data != null && data.Count > 0)
                {

                    if (!string.IsNullOrEmpty(data.FirstOrDefault().DEPARTMENT_NAME))
                    {
                        var departmentId = DepartmentList.Where(o => o.DEPARTMENT_NAME.Equals(data.FirstOrDefault().DEPARTMENT_NAME)).ToList();
                        if (departmentId != null && departmentId.Count > 0)
                        {

                            grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", departmentId.FirstOrDefault().ID);
                        }
                        else
                        {
                            grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", null);
                        }
                    }
                }
                else
                {
                    grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
