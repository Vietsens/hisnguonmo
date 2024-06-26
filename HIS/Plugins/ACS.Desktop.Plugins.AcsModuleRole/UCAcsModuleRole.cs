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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.Data;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using ACS.Desktop.Plugins.AcsModuleRole.entity;
using DevExpress.XtraEditors.Controls;

namespace ACS.Desktop.Plugins.AcsModuleRole
{
    public partial class UCAcsModuleRole : HIS.Desktop.Utility.UserControlBase
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        List<ModuleADO> ListModuleADO = new List<ModuleADO>();
        List<RoleADO> ListRoleADO = new List<RoleADO>();
        List<ACS_MODULE_ROLE> ListModuleRole = new List<ACS_MODULE_ROLE>();
        long ModuleID = 0;
        long RoleID = 0;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;

        public UCAcsModuleRole(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action<Type> delegateRefresh, long numPageSize, string applicationCode, string iconPath, List<ACS_APPLICATION> listAcsApplication)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            ApiConsumers.AcsConsumer = acsConsumer;
            ACS.Desktop.Plugins.AcsModuleRole.RamData.acsAppication = listAcsApplication;
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCAcsModuelRole_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAcsModuleRole();
                WaitingManager.Show();
                grclCheckForModule.Image = imgStock.Images[1];
                grclCheckForModule.ImageAlignment = StringAlignment.Center;
                grclcheck21.Image = imgPatient.Images[1];
                grclcheck21.ImageAlignment = StringAlignment.Center;
                cboChoose.EditValue = "Chức năng";
                LoadStatus();
                LoadComboApplication();
                FilldataToGridModule();
                FilldataToGridRole();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAcsModuleRole()
        {
            try
            {
                CommonParam param2 = new CommonParam();
                AcsModuleRoleFilter filter2 = new AcsModuleRoleFilter();
                ListModuleRole = new BackendAdapter(param2).Get<List<ACS_MODULE_ROLE>>("api/AcsModuleRole/Get", ApiConsumers.AcsConsumer, filter2, param2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboApplication()
        {
            try
            {
                var data = RamData.acsAppication;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboApplication, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadStatus()
        {
            if (cboChoose.Text == "Chức năng")
            {
                grclCheckForRole.OptionsColumn.AllowEdit = true;
                grclcheck21.OptionsColumn.AllowEdit = true;
                grclCheckForModule.OptionsColumn.AllowEdit = false;
                grclcheck22.OptionsColumn.AllowEdit = false;
                check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

            }
            else
            {
                grclCheckForRole.OptionsColumn.AllowEdit = false;
                grclcheck21.OptionsColumn.AllowEdit = false;
                grclCheckForModule.OptionsColumn.AllowEdit = true;
                grclcheck22.OptionsColumn.AllowEdit = true;
                check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            }
        }

        private void FilldataToGridRole()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging2(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(LoadPaging2, param, numPageSize, this.gridControl2);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging2(object param)
        {

            try
            {
                start1 = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start1, limit);
                Inventec.Core.ApiResultObject<List<ACS_ROLE>> apiResult = null;
                AcsRoleFilter filter = new AcsRoleFilter();
                SetFilterNavBar2(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView2.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ACS_ROLE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<ACS_ROLE>)apiResult.Data;

                    if (data != null)
                    {
                        ListRoleADO = new List<RoleADO>();
                        foreach (var item in data)
                        {
                            RoleADO x = new RoleADO();
                            x.ROLE_CODE = item.ROLE_CODE;
                            x.ROLE_NAME = item.ROLE_NAME;
                            x.ID = item.ID;

                            x.checkForModule = false;
                            x.checkForRole = false;

                            ListRoleADO.Add(x);
                        }
                        if (ModuleID != 0)
                        {
                            List<long> ListRoleID = new List<long>();
                            ListRoleID = (from item in (ListModuleRole.Where(o => o.MODULE_ID == ModuleID).ToList()) select item.ROLE_ID).ToList();

                            foreach (var item in ListRoleADO)
                            {
                                foreach (var item1 in ListRoleID)
                                {
                                    if (item.ID == item1)
                                    {
                                        item.checkForModule = true;
                                    }
                                }
                            }
                        }

                        gridView2.GridControl.DataSource = ListRoleADO.OrderByDescending(o => o.checkForModule);
                        rowCount1 = (ListRoleADO == null ? 0 : ListRoleADO.Count);
                        dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView2.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar2(ref AcsRoleFilter filter)
        {
            filter.KEY_WORD = txtKeyword2.Text.Trim();
        }

        private void FilldataToGridModule()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging1(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging1, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging1(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_ACS_MODULE>> apiResult = null;
                AcsModuleViewFilter filter = new AcsModuleViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                grViewModule.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_ACS_MODULE>>("api/AcsModule/GetView", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_ACS_MODULE>)apiResult.Data;

                    if (data != null)
                    {
                        ListModuleADO = new List<ModuleADO>();
                        foreach (var item in data)
                        {
                            ModuleADO x = new ModuleADO();
                            x.MODULE_NAME = item.MODULE_NAME;
                            x.ID = item.ID;
                            x.MODULE_LINK = item.MODULE_LINK;
                            x.APPLICATION_NAME = item.APPLICATION_NAME;
                            x.CheckForModule = false;
                            x.CheckForRole = false;

                            ListModuleADO.Add(x);
                        }
                        if (RoleID != 0)
                        {
                            List<long> ListModuleID = new List<long>();
                            CommonParam param2 = new CommonParam();
                            AcsModuleRoleFilter filter2 = new AcsModuleRoleFilter();
                            ListModuleID = (from item in ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList() select item.MODULE_ID).ToList();

                            foreach (var item in ListModuleADO)
                            {
                                foreach (var item1 in ListModuleID)
                                {
                                    if (item.ID == item1)
                                    {
                                        item.CheckForRole = true;
                                    }
                                }
                            }
                        }

                        grViewModule.GridControl.DataSource = ListModuleADO.OrderByDescending(o => o.CheckForRole);
                        rowCount = (ListModuleADO == null ? 0 : ListModuleADO.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                grViewModule.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref AcsModuleViewFilter filter)
        {
            filter.KEY_WORD = txtKeyword1.Text.Trim();
            if (cboApplication.EditValue != null)
                filter.APPLICATION_ID = (long)cboApplication.EditValue;
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridRole();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridModule();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                grclCheckForModule.Image = imgStock.Images[1];
                grclcheck21.Image = imgPatient.Images[1];
                ModuleID = 0;
                RoleID = 0;
                LoadStatus();
                FilldataToGridRole();
                FilldataToGridModule();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridModule();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRole();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WaitingManager.Show();
            if (ModuleID == 0 && RoleID == 0)
            {
                if (cboChoose.Text == "Chức năng")
                {
                    MessageManager.Show("Chưa chọn chức năng");
                    WaitingManager.Hide();
                    return;
                }
                else
                {
                    MessageManager.Show("Chưa chọn vai trò");
                    WaitingManager.Hide();
                    return;
                }
            }
            if (cboChoose.EditValue == "Chức năng")
            {
                try
                {
                    Boolean success = false;
                    List<ACS_MODULE_ROLE> ListCreate = new List<ACS_MODULE_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_MODULE_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.MODULE_ID == ModuleID).ToList();
                    List<long> listRoleIDOld = new List<long>();
                    List<long> listRoleIDNew = new List<long>();
                    CommonParam param = new CommonParam();
                    if (ListModuleRoleOld != null)
                    {
                        var noCheck = ListRoleADO.Where(o => !o.checkForModule).ToList();
                        var check = ListRoleADO.Where(o => o.checkForModule).ToList();

                        foreach (var item in ListModuleRoleOld)
                        {
                            listRoleIDOld.Add(item.ROLE_ID);
                        }
                        foreach (var item in ListRoleADO.Where(o => o.checkForModule))
                        {
                            listRoleIDNew.Add(item.ID);
                        }
                        foreach (var item in ((ListRoleADO.Where(o => o.checkForModule && !listRoleIDOld.Contains(o.ID))).ToList()))
                        {
                            ACS_MODULE_ROLE ModuleRoleNew = new ACS_MODULE_ROLE();
                            ModuleRoleNew.ROLE_ID = item.ID;
                            ModuleRoleNew.MODULE_ID = ModuleID;
                            ListCreate.Add(ModuleRoleNew);
                        }

                        if (noCheck != null && noCheck.Count > 0)
                        {
                            var delete = ListModuleRoleOld.Where(o => noCheck.Select(p => p.ID).Contains(o.ROLE_ID)).ToList();
                            if (delete != null && delete.Count > 0)
                            {
                                ListDelete.AddRange(delete.Select(o => o.ID).ToList());
                            }
                        }
                    }
                    else
                    {
                        ListCreate.AddRange(ListModuleRoleOld);
                    }

                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<List<ACS_MODULE_ROLE>>("api/AcsModuleRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
                        if (resultData != null)
                        {
                            success = true;
                        }
                    }

                    if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/AcsModuleRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        success = true;
                    }

                    LoadAcsModuleRole();
                    FilldataToGridRole();

                    WaitingManager.Hide();
                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else
            {
                try
                {
                    Boolean success = false;
                    List<ACS_MODULE_ROLE> ListCreate = new List<ACS_MODULE_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_MODULE_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList();
                    List<long> listModuleIDOld = new List<long>();
                    List<long> listModuleIDNew = new List<long>();
                    CommonParam param = new CommonParam();
                    if (ListModuleRoleOld != null && ListModuleRoleOld.Count > 0)
                    {
                        var noCheck = ListModuleADO.Where(o => !o.CheckForRole).ToList();
                        var check = ListModuleADO.Where(o => o.CheckForRole).ToList();

                        foreach (var item in ListModuleRoleOld)
                        {
                            listModuleIDOld.Add(item.MODULE_ID);
                        }
                        foreach (var item in ListModuleADO.Where(o => o.CheckForRole))
                        {
                            listModuleIDNew.Add(item.ID);
                        }
                        foreach (var item in ((ListModuleADO.Where(o => o.CheckForRole && !listModuleIDOld.Contains(o.ID))).ToList()))
                        {
                            ACS_MODULE_ROLE ModuleRoleNew = new ACS_MODULE_ROLE();
                            ModuleRoleNew.MODULE_ID = item.ID;
                            ModuleRoleNew.ROLE_ID = RoleID;
                            ListCreate.Add(ModuleRoleNew);
                        }

                        if (noCheck != null && noCheck.Count > 0)
                        {
                            var delete = ListModuleRoleOld.Where(o => noCheck.Select(p => p.ID).Contains(o.MODULE_ID)).ToList();
                            if (delete != null && delete.Count > 0)
                            {
                                ListDelete.AddRange(delete.Select(o => o.ID).ToList());
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ListModuleADO)
                        {
                            if (item.CheckForRole)
                            {
                                ACS_MODULE_ROLE ModuleRoleNew = new ACS_MODULE_ROLE();
                                ModuleRoleNew.MODULE_ID = item.ID;
                                ModuleRoleNew.ROLE_ID = RoleID;
                                ListCreate.Add(ModuleRoleNew);
                            }
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<ACS_MODULE_ROLE>("api/AcsModuleRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);

                        success = true;
                    }

                    if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/AcsModuleRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        success = true;
                    }

                    LoadAcsModuleRole();
                    FilldataToGridModule();

                    WaitingManager.Hide();
                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            #region
            //try
            //{
            //    WaitingManager.Show();

            //    Boolean success = false;
            //    List<HIS_MEST_PATIENT_TYPE> ListCreate = new List<HIS_MEST_PATIENT_TYPE>();
            //    List<long> ListDelete = new List<long>();
            //    CommonParam param = new CommonParam();
            //    if (ModuleID == 0 && PatientID == 0)
            //    {
            //        if (cboChoose.EditValue == "Kho")
            //        {
            //            MessageBox.Show("chưa chọn kho");
            //            WaitingManager.Hide();
            //            return;
            //        }
            //        else
            //        {
            //            MessageBox.Show("chưa chọn đối tượng");
            //            WaitingManager.Hide();
            //            return;
            //        }
            //    }
            //    if (ModuleID != 0)
            //    {
            //        List<HIS_MEST_PATIENT_TYPE> AcsModuelRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListPatientTypeIDOld = new List<long>();
            //        foreach (var item in AcsModuelRole)
            //        {
            //            if (item.MEDI_STOCK_ID == ModuleID)
            //            {
            //                ListPatientTypeIDOld.Add(item.PATIENT_TYPE_ID);
            //            }
            //        }
            //        foreach (var item in ListRoleADO)
            //        {
            //            if (ListPatientTypeIDOld.Contains(item.ID) && !item.check1)
            //            {
            //                var x = from item1 in AcsModuelRole
            //                        where item1.PATIENT_TYPE_ID == item.ID && item1.MEDI_STOCK_ID == ModuleID
            //                        select item1;
            //                HIS_MEST_PATIENT_TYPE o = x.FirstOrDefault();
            //                long ID = o.ID;
            //                ListDelete.Add(ID);
            //            }
            //            if (!ListPatientTypeIDOld.Contains(item.ID) && item.check1)
            //            {
            //                HIS_MEST_PATIENT_TYPE patientype = new HIS_MEST_PATIENT_TYPE();
            //                patientype.MEDI_STOCK_ID = ModuleID;
            //                patientype.PATIENT_TYPE_ID = item.ID;
            //                ListCreate.Add(patientype);
            //            }
            //        }
            //        if (ListCreate.Count != null && ListCreate.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsModuleRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsModuleRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
            //            if (resultData)
            //            {
            //                success = true;
            //            }
            //        }
            //        else if (ListCreate.Count == 0 && ListDelete.Count == 0)
            //        {
            //            WaitingManager.Hide();
            //            MessageBox.Show("Chưa có thay đổi", "thông báo");

            //            return;
            //        }
            //        if (success)
            //        {
            //            BackendDataWorker.Reset<HIS_MEST_PATIENT_TYPE>();
            //            RefeshGridPatient(ModuleID);
            //        }
            //    }
            //    else
            //    {
            //        List<HIS_MEST_PATIENT_TYPE> AcsModuelRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListModuleIDOld = new List<long>();
            //        foreach (var item in AcsModuelRole)
            //        {
            //            if (item.PATIENT_TYPE_ID == PatientID)
            //            {
            //                ListModuleIDOld.Add(item.MEDI_STOCK_ID);
            //            }
            //        }
            //        foreach (var item in ListModuleADO)
            //        {
            //            if (ListModuleIDOld.Contains(item.ID) && !item.check1)
            //            {
            //                var x = from item1 in AcsModuelRole
            //                        where item1.PATIENT_TYPE_ID == PatientID && item1.MEDI_STOCK_ID == item.ID
            //                        select item1;
            //                HIS_MEST_PATIENT_TYPE o = x.FirstOrDefault();
            //                long ID = o.ID;
            //                ListDelete.Add(ID);
            //            }
            //            if (!ListModuleIDOld.Contains(item.ID) && item.check1)
            //            {
            //                HIS_MEST_PATIENT_TYPE patientype = new HIS_MEST_PATIENT_TYPE();
            //                patientype.MEDI_STOCK_ID = item.ID;
            //                patientype.PATIENT_TYPE_ID = PatientID;
            //                ListCreate.Add(patientype);
            //            }
            //        }
            //        if (ListCreate.Count != null && ListCreate.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsModuleRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsModuleRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
            //            if (resultData)
            //            {
            //                success = true;
            //            }
            //        }
            //        else if (ListCreate.Count == 0 && ListDelete.Count == 0)
            //        {
            //            WaitingManager.Hide();
            //            MessageBox.Show("Chưa có thay đổi", "thông báo");

            //            return;
            //        }
            //        if (success)
            //        {
            //            BackendDataWorker.Reset<HIS_MEST_PATIENT_TYPE>();
            //            RefeshGridModule(PatientID);
            //        }
            //    }
            //    WaitingManager.Hide();
            //    #region Hien thi message thong bao
            //    MessageManager.Show(this.ParentForm, param, success);
            //    #endregion

            //    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
            //    SessionManager.ProcessTokenLost(param);
            //    #endregion

            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
            #endregion
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                ModuleADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ModuleADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "grclCheckForModule")
                    {
                        if (cboChoose.Text != "Chức năng")
                        {
                            e.RepositoryItem = chkForRole;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit1;
                        }
                    }
                    else if (e.Column.FieldName == "grclCheckForRole")
                    {
                        e.RepositoryItem = chkForModule;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    ModuleADO pData = (ModuleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "grclCheckForRole")
                    {
                        e.Value = pData.CheckForModule;
                    }
                    else if (e.Column.FieldName == "grclCheckForModule")
                    {
                        e.Value = pData.CheckForRole;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    RoleADO pData = (RoleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.Value = pData.checkForRole;
                    }
                    else if (e.Column.FieldName == "check21")
                    {
                        e.Value = pData.checkForModule;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check12_Click(object sender, EventArgs e)
        {
            var row = (ModuleADO)grViewModule.GetFocusedRow();
            if (row.CheckForModule == true)
            {
                row.CheckForModule = false;
                ModuleID = 0;
                FilldataToGridRole();
                return;
            }
            foreach (var item in ListModuleADO)
            {
                item.CheckForModule = false;
                if (item.ID == row.ID)
                {
                    ModuleID = row.ID;
                    item.CheckForModule = true;
                }
            }
            gridControl1.DataSource = ListModuleADO;
            gridControl1.RefreshDataSource();
            RefeshGridPatient(ModuleID);
        }

        private void RefeshGridPatient(long ModuleID)
        {
            List<long> listRoleID = new List<long>();
            LoadAcsModuleRole();
            foreach (var item in ListModuleRole)
            {
                if (item.MODULE_ID == ModuleID)
                {
                    listRoleID.Add(item.ROLE_ID);
                }
            }
            foreach (var item in ListRoleADO)
            {
                item.checkForModule = false;
                foreach (var item1 in listRoleID)
                {
                    if (item.ID == item1)
                    {
                        item.checkForModule = true;
                    }
                }
            }
            var data = ListRoleADO.OrderByDescending(o => o.checkForModule);
            gridControl2.DataSource = null;
            gridControl2.DataSource = data;
            gridControl2.RefreshDataSource();
        }

        private void check21_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (RoleADO)gridView2.GetFocusedRow();
                if (row.checkForModule)
                {
                    row.checkForModule = false;
                }
                else
                {
                    row.checkForModule = true;
                }

                foreach (var item in ListRoleADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.checkForModule = row.checkForModule;
                    }
                }
                //    gridControl2.DataSource = ListRoleADO;
                //    gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check22_Click(object sender, EventArgs e)
        {
            var row = (RoleADO)gridView2.GetFocusedRow();
            if (row.checkForRole == true)
            {
                row.checkForRole = false;
                RoleID = 0;
                FilldataToGridModule();
                return;
            }
            foreach (var item in ListRoleADO)
            {
                item.checkForRole = false;
                if (item.ID == row.ID)
                {
                    RoleID = row.ID;
                    item.checkForRole = true;
                }
            }
            gridControl2.DataSource = ListRoleADO;
            gridControl2.RefreshDataSource();
            RefeshGridModule(RoleID);
        }

        private void RefeshGridModule(long RoleID)
        {
            LoadAcsModuleRole();
            List<long> ListModuleID = new List<long>();
            foreach (var item in ListModuleRole)
            {
                if (item.ROLE_ID == RoleID)
                {
                    ListModuleID.Add(item.MODULE_ID);
                }
            }
            foreach (var item in ListModuleADO)
            {
                item.CheckForRole = false;
                foreach (var item1 in ListModuleID)
                {
                    if (item.ID == item1)
                    {
                        item.CheckForRole = true;
                    }
                }
            }
            //var data = 
            // gridControl1.DataSource = null;
            gridControl1.DataSource = ListModuleADO.OrderByDescending(o => o.CheckForRole);
            gridControl1.RefreshDataSource();
        }

        private void check11_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ModuleADO)grViewModule.GetFocusedRow();
                if (row.CheckForRole)
                {
                    row.CheckForRole = false;
                }
                else
                {
                    row.CheckForRole = true;
                }

                foreach (var item in ListModuleADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.CheckForRole = row.CheckForRole;
                    }
                }
                //    gridControl1.DataSource = ListModuleADO;
                //    gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue == "Kho")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "grclCheckForModule")
                        {
                            if (grclCheckForModule.Image == imgStock.Images[1])
                            {
                                grclCheckForModule.Image = imgStock.Images[0];
                                foreach (var item in ListModuleADO)
                                {
                                    item.CheckForRole = true;
                                }
                            }
                            else
                            {
                                grclCheckForModule.Image = imgStock.Images[1];
                                foreach (var item in ListModuleADO)
                                {
                                    item.CheckForRole = false;
                                }
                            }
                            gridControl1.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "check21")
                    {
                        if (cboChoose.Text == "Chức năng")
                        {
                            e.RepositoryItem = check21;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit2;
                        }
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.RepositoryItem = check22;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue != "Chức năng")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check21")
                        {
                            if (grclcheck21.Image == imgPatient.Images[1])
                            {
                                grclcheck21.Image = imgPatient.Images[0];
                                foreach (var item in ListRoleADO)
                                {
                                    item.checkForModule = true;
                                }
                            }
                            else
                            {
                                grclcheck21.Image = imgPatient.Images[1];
                                foreach (var item in ListRoleADO)
                                {
                                    item.checkForModule = false;
                                }
                            }
                            gridControl2.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridModule();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRole();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboApplication_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                FilldataToGridModule();
            }
        }

        private void cboApplication_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboApplication.Properties.Buttons[1].Visible = true;
                    cboApplication.EditValue = null;
                    FilldataToGridModule();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind1.Focus();
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                btnFind2.Focus();
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
