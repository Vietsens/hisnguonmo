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
using ACS.Desktop.Plugins.AcsControlRole.entity;

namespace ACS.Desktop.Plugins.AcsControlRole
{
    public partial class UCAcsControlRole : HIS.Desktop.Utility.UserControlBase
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        List<ControlADO> ListControlADO = new List<ControlADO>();
        List<RoleADO> ListRoleADO = new List<RoleADO>();
        List<ACS_CONTROL_ROLE> ListModuleRole = new List<ACS_CONTROL_ROLE>();
        long ModuleID = 0;
        long RoleID = 0;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;

        public UCAcsControlRole(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action<Type> delegateRefresh, long numPageSize, string
            applicationCode, string iconPath)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            ApiConsumers.AcsConsumer = acsConsumer;
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

        private void UCAcsControlRole_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAcsControlRole();
                WaitingManager.Show();
                grclCheckForControl.Image = imgStock.Images[1];
                grclCheckForControl.ImageAlignment = StringAlignment.Center;
                grclcheck21.Image = imgStock.Images[1];
                grclcheck21.ImageAlignment = StringAlignment.Center;
                cboChoose.EditValue = "Nút";
                LoadStatus();
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

        private void LoadAcsControlRole()
        {
            CommonParam param2 = new CommonParam();
            AcsControlRoleFilter filter2 = new AcsControlRoleFilter();
            ListModuleRole = new BackendAdapter(param2).Get<List<ACS_CONTROL_ROLE>>("api/AcsControlRole/Get", ApiConsumers.AcsConsumer, filter2, param2);
        }

        private void LoadStatus()
        {
            if (cboChoose.EditValue == "Nút")
            {
                grclCheckForRole.OptionsColumn.AllowEdit = true;
                grclcheck21.OptionsColumn.AllowEdit = true;
                grclCheckForControl.OptionsColumn.AllowEdit = false;
                grclcheck22.OptionsColumn.AllowEdit = false;
                check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

            }
            else
            {
                grclCheckForRole.OptionsColumn.AllowEdit = false;
                grclcheck21.OptionsColumn.AllowEdit = false;
                grclCheckForControl.OptionsColumn.AllowEdit = true;
                grclcheck22.OptionsColumn.AllowEdit = true;
                chkForRole.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
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
                ucPaging2.Init(LoadPaging2, param, numPageSize, this.gridControl1);
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
                            x.CheckForControl = false;
                            x.checkForRole = false;

                            ListRoleADO.Add(x);
                        }
                        if (ModuleID != 0)
                        {
                            List<long> ListRoleID = new List<long>();
                            ListRoleID = (from item in (ListModuleRole.Where(o => o.CONTROL_ID == ModuleID).ToList()) select item.ROLE_ID).ToList();

                            foreach (var item in ListRoleADO)
                            {
                                item.CheckForControl = false;
                                foreach (var item1 in ListRoleID)
                                {
                                    if (item.ID == item1)
                                    {
                                        item.CheckForControl = true;
                                    }
                                }
                            }
                        }

                        gridView2.GridControl.DataSource = ListRoleADO.OrderByDescending(o => o.CheckForControl);
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
                ucPaging1.Init(LoadPaging1, param, numPageSize, this.gridControl2);
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
                Inventec.Core.ApiResultObject<List<ACS_CONTROL>> apiResult = null;
                AcsControlFilter filter = new AcsControlFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                grViewModule.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ACS_CONTROL>>("api/AcsControl/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<ACS_CONTROL>)apiResult.Data;

                    if (data != null)
                    {
                        ListControlADO = new List<ControlADO>();
                        foreach (var item in data)
                        {
                            ControlADO x = new ControlADO();
                            x.CONTROL_NAME = item.CONTROL_NAME;
                            x.ID = item.ID;
                            x.CONTROL_CODE = item.CONTROL_CODE;
                            x.CheckForControl = false;
                            x.CheckForRole = false;

                            ListControlADO.Add(x);
                        }
                        if (RoleID != 0)
                        {
                            List<long> ListModuleID = new List<long>();
                            CommonParam param2 = new CommonParam();
                            AcsControlRoleFilter filter2 = new AcsControlRoleFilter();
                            ListModuleID = (from item in ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList() select item.CONTROL_ID).ToList();

                            foreach (var item in ListControlADO)
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
                        }

                        grViewModule.GridControl.DataSource = ListControlADO.OrderByDescending(o => o.CheckForRole);
                        rowCount = (ListControlADO == null ? 0 : ListControlADO.Count);
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

        private void SetFilterNavBar(ref AcsControlFilter filter)
        {
            filter.KEY_WORD = txtKeyword1.Text.Trim();
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
                grclCheckForControl.Image = imgStock.Images[1];
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
                if (cboChoose.EditValue == "Nút")
                {
                    MessageManager.Show("chưa chọn Nút");
                    WaitingManager.Hide();
                    return;
                }
                else
                {
                    MessageManager.Show("chưa chọn đối tượng");
                    WaitingManager.Hide();
                    return;
                }
            }
            if (cboChoose.EditValue == "Nút")
            {
                try
                {
                    Boolean success = false;
                    List<ACS_CONTROL_ROLE> ListCreate = new List<ACS_CONTROL_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_CONTROL_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.CONTROL_ID == ModuleID).ToList();
                    List<long> listRoleIDOld = new List<long>();
                    List<long> listRoleIDNew = new List<long>();
                    CommonParam param = new CommonParam();
                    if (ListModuleRoleOld != null && ListModuleRoleOld.Count > 0)
                    {
                        foreach (var item in ListModuleRoleOld)
                        {
                            listRoleIDOld.Add(item.ROLE_ID);
                        }
                        foreach (var item in ListRoleADO.Where(o => o.CheckForControl))
                        {
                            listRoleIDNew.Add(item.ID);
                        }
                        foreach (var item in ((ListRoleADO.Where(o => o.CheckForControl && !listRoleIDOld.Contains(o.ID))).ToList()))
                        {
                            ACS_CONTROL_ROLE ModuleRoleNew = new ACS_CONTROL_ROLE();
                            ModuleRoleNew.ROLE_ID = item.ID;
                            ModuleRoleNew.CONTROL_ID = ModuleID;
                            ListCreate.Add(ModuleRoleNew);
                        }
                        foreach (var item in listRoleIDOld.Where(o => !listRoleIDNew.Contains(o)))
                        {
                            var x = ListModuleRoleOld.Where(o => o.ROLE_ID == item).ToList().FirstOrDefault();
                            ListDelete.Add(x.ID);
                        }
                    }
                    else
                    {
                        foreach (var item in ListRoleADO)
                        {
                            if (item.CheckForControl)
                            {
                                ACS_CONTROL_ROLE ModuleRoleNew = new ACS_CONTROL_ROLE();
                                ModuleRoleNew.CONTROL_ID = ModuleID;
                                ModuleRoleNew.ROLE_ID = item.ID;
                                ListCreate.Add(ModuleRoleNew);
                            }
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<ACS_CONTROL_ROLE>("api/AcsControlRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
                        success = true;
                    }

                    else if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/AcsControlRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chưa có thay đổi", "thông báo");

                        return;
                    }
                    WaitingManager.Hide();
                    RefeshGridRole(ModuleID);
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
                    List<ACS_CONTROL_ROLE> ListCreate = new List<ACS_CONTROL_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_CONTROL_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList();
                    List<long> listModuleIDOld = new List<long>();
                    List<long> listModuleIDNew = new List<long>();
                    CommonParam param = new CommonParam();
                    if (ListModuleRoleOld != null && ListModuleRoleOld.Count > 0)
                    {
                        foreach (var item in ListModuleRoleOld)
                        {
                            listModuleIDOld.Add(item.CONTROL_ID);
                        }
                        foreach (var item in ListControlADO.Where(o => o.CheckForRole))
                        {
                            listModuleIDNew.Add(item.ID);
                        }
                        foreach (var item in ((ListControlADO.Where(o => o.CheckForRole && !listModuleIDOld.Contains(o.ID))).ToList()))
                        {
                            ACS_CONTROL_ROLE ModuleRoleNew = new ACS_CONTROL_ROLE();
                            ModuleRoleNew.CONTROL_ID = item.ID;
                            ModuleRoleNew.ROLE_ID = RoleID;
                            ListCreate.Add(ModuleRoleNew);
                        }
                        foreach (var item in listModuleIDOld.Where(o => !listModuleIDNew.Contains(o)))
                        {
                            var x = ListModuleRoleOld.Where(o => o.CONTROL_ID == item).ToList().FirstOrDefault();
                            ListDelete.Add(x.ID);
                        }
                    }
                    else
                    {
                        foreach (var item in ListControlADO)
                        {
                            if (item.CheckForRole)
                            {
                                ACS_CONTROL_ROLE ModuleRoleNew = new ACS_CONTROL_ROLE();
                                ModuleRoleNew.CONTROL_ID = item.ID;
                                ModuleRoleNew.ROLE_ID = RoleID;
                                ListCreate.Add(ModuleRoleNew);
                            }
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<ACS_CONTROL_ROLE>("api/AcsControlRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);

                        success = true;
                    }

                    else if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/AcsControlRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chưa có thay đổi", "thông báo");

                        return;
                    }
                    RefeshGridControl(RoleID);
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
            //        List<HIS_MEST_PATIENT_TYPE> AcsControlRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListPatientTypeIDOld = new List<long>();
            //        foreach (var item in AcsControlRole)
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
            //                var x = from item1 in AcsControlRole
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
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsControlRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsControlRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
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
            //            RefeshGridRole(ModuleID);
            //        }
            //    }
            //    else
            //    {
            //        List<HIS_MEST_PATIENT_TYPE> AcsControlRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListModuleIDOld = new List<long>();
            //        foreach (var item in AcsControlRole)
            //        {
            //            if (item.PATIENT_TYPE_ID == PatientID)
            //            {
            //                ListModuleIDOld.Add(item.MEDI_STOCK_ID);
            //            }
            //        }
            //        foreach (var item in ListControlADO)
            //        {
            //            if (ListModuleIDOld.Contains(item.ID) && !item.check1)
            //            {
            //                var x = from item1 in AcsControlRole
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
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsControlRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsControlRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
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
            //            RefeshGridControl(PatientID);
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
                ControlADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ControlADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "grclCheckForControl")
                    {
                        if (cboChoose.EditValue != "Nút")
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
                        e.RepositoryItem = chkForControl;
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
                    ControlADO pData = (ControlADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "grclCheckForRole")
                    {
                        e.Value = pData.CheckForControl;
                    }
                    else if (e.Column.FieldName == "grclCheckForControl")
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
                        e.Value = pData.CheckForControl;
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
            var row = (ControlADO)grViewModule.GetFocusedRow();
            if (row.CheckForControl == true)
            {
                row.CheckForControl = false;
                ModuleID = 0;
                FilldataToGridRole();
                return;
            }
            foreach (var item in ListControlADO)
            {
                item.CheckForControl = false;
                if (item.ID == row.ID)
                {
                    ModuleID = row.ID;
                    item.CheckForControl = true;
                }
            }
            gridControl1.DataSource = ListControlADO;
            gridControl1.RefreshDataSource();
            RefeshGridRole(ModuleID);
        }

        private void RefeshGridRole(long ControlID)
        {
            List<long> listRoleID = new List<long>();
            LoadAcsControlRole();
            foreach (var item in ListModuleRole)
            {
                if (item.CONTROL_ID == ControlID)
                {
                    listRoleID.Add(item.ROLE_ID);
                }
            }
            foreach (var item in ListRoleADO)
            {
                item.CheckForControl = false;
                foreach (var item1 in listRoleID)
                {
                    if (item.ID == item1)
                    {
                        item.CheckForControl = true;
                    }
                }
            }
            var data = ListRoleADO.OrderByDescending(o => o.CheckForControl);
            gridControl2.DataSource = null;
            gridControl2.DataSource = data;
            gridControl2.RefreshDataSource();
        }

        private void check21_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (RoleADO)gridView2.GetFocusedRow();
                if (row.CheckForControl)
                {
                    row.CheckForControl = false;
                }
                else
                {
                    row.CheckForControl = true;
                }

                foreach (var item in ListRoleADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.CheckForControl = row.CheckForControl;
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
            RefeshGridControl(RoleID);
        }

        private void RefeshGridControl(long RoleID)
        {
            LoadAcsControlRole();
            List<long> ListModuleID = new List<long>();
            foreach (var item in ListModuleRole)
            {
                if (item.ROLE_ID == RoleID)
                {
                    ListModuleID.Add(item.CONTROL_ID);
                }
            }
            foreach (var item in ListControlADO)
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
            gridControl1.DataSource = ListControlADO.OrderByDescending(o => o.CheckForRole);
            gridControl1.RefreshDataSource();
        }

        private void check11_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ControlADO)grViewModule.GetFocusedRow();
                if (row.CheckForRole)
                {
                    row.CheckForRole = false;
                }
                else
                {
                    row.CheckForRole = true;
                }

                foreach (var item in ListControlADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.CheckForRole = row.CheckForRole;
                    }
                }
                //    gridControl1.DataSource = ListControlADO;
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
                        if (hi.Column.FieldName == "check11")
                        {
                            if (grclCheckForControl.Image == imgStock.Images[1])
                            {
                                grclCheckForControl.Image = imgStock.Images[0];
                                foreach (var item in ListControlADO)
                                {
                                    item.CheckForRole = true;
                                }
                            }
                            else
                            {
                                grclCheckForControl.Image = imgStock.Images[1];
                                foreach (var item in ListControlADO)
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
                        if (cboChoose.EditValue == "Nút")
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
                if (cboChoose.EditValue != "Nút")
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
                                    item.CheckForControl = true;
                                }
                            }
                            else
                            {
                                grclcheck21.Image = imgPatient.Images[1];
                                foreach (var item in ListRoleADO)
                                {
                                    item.CheckForControl = false;
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
    }
}
