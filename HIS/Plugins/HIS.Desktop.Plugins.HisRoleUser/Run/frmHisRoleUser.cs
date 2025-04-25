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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisRoleUser.ADO;
using HIS.Desktop.Plugins.HisRoleUser.Print;
using HIS.Desktop.Plugins.HisRoleUser.Validation;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisRoleUser.Run
{
    public partial class frmHisRoleUser : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<RoleUserAdo> listRoleUserAdo = new List<RoleUserAdo>();

        long ImpMestId;
        long IMP_MEST_TYPE_ID;
        long ImpMestSttId;
        List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials;
        List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines;
        List<V_HIS_IMP_MEST_BLOOD> impMestBloods;
        V_HIS_IMP_MEST impMest;
        Inventec.Desktop.Common.Modules.Module moduleData;

        internal List<HIS_EXECUTE_ROLE> listExecuteRole = new List<HIS_EXECUTE_ROLE>();
        internal List<HIS_EXECUTE_ROLE_USER> listExecuteRoleUser = new List<HIS_EXECUTE_ROLE_USER>();
        internal List<HIS_IMP_USER_TEMP> listImpUserTemp = new List<HIS_IMP_USER_TEMP>();

        public frmHisRoleUser()
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
        }

        public frmHisRoleUser(
            Inventec.Desktop.Common.Modules.Module currentModule,
            long impMestId,
            long IMP_MEST_TYPE_ID,
            long _impMestSttId,
            List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials,
            List<V_HIS_IMP_MEST_BLOOD> impMestBloods
            )
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.ImpMestId = impMestId;
                this.IMP_MEST_TYPE_ID = IMP_MEST_TYPE_ID;
                this.ImpMestSttId = _impMestSttId;
                this.impMestMaterials = impMestMaterials;
                this.impMestMedicines = impMestMedicines;
                this.impMestBloods = impMestBloods;
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisRoleUser.Resources.Lang", typeof(HIS.Desktop.Plugins.HisRoleUser.Run.frmHisRoleUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisRoleUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintV2.Text = Inventec.Common.Resource.Get.Value("frmHisRoleUser.btnPrintV2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisRoleUser.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem_Save.Caption = Inventec.Common.Resource.Get.Value("frmHisRoleUser.barButtonItem_Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem_Print.Caption = Inventec.Common.Resource.Get.Value("frmHisRoleUser.barButtonItem_Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisRoleUser.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisRoleUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemLookUp__ExecuteRoleName.NullText = Inventec.Common.Resource.Get.Value("frmHisRoleUser.repositoryItemLookUp__ExecuteRoleName.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisRoleUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemLookUp__ExecuteRoleUser.NullText = Inventec.Common.Resource.Get.Value("frmHisRoleUser.repositoryItemLookUp__ExecuteRoleUser.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisRoleUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisRoleUser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisRoleUser(Inventec.Desktop.Common.Modules.Module currentModule, long impMestId, long IMP_MEST_TYPE_ID)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.ImpMestId = impMestId;
                this.IMP_MEST_TYPE_ID = IMP_MEST_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisRoleUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                SetIcon();
                ValidateMaxLength(txtImpMestNumber, 30);
                ValidateMaxLength(mmConclude, 2000);
                if (this.ImpMestId > 0)
                {
                    LoadDataToPrint();
                }
                InitMenuToButtonPrint();
                LoadDataToCombo();
                InitData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateMaxLength(BaseControl control, int maxlength)
        {
            try
            {
                ValidationMaxLength valid = new ValidationMaxLength();
                valid.textEdit = control;
                valid.maxLength = maxlength;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataToPrint()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (impMestMedicines == null || impMestMedicines.Count == 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.ImpMestId;
                    impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                    impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                }
                if (impMestMaterials == null || impMestMaterials.Count == 0)
                {
                    MOS.Filter.HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.ImpMestId;
                    impMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                    impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);
                }
                if (impMestBloods == null || impMestBloods.Count == 0)
                {
                    MOS.Filter.HisImpMestBloodViewFilter bloodFilter = new HisImpMestBloodViewFilter();
                    bloodFilter.IMP_MEST_ID = this.ImpMestId;
                    impMestBloods = new List<V_HIS_IMP_MEST_BLOOD>();
                    impMestBloods = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>(HisRequestUriStore.HIS_IMP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitData()
        {
            try
            {
                this.listRoleUserAdo = new List<RoleUserAdo>();
                if (this.ImpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestFilter impMestFilter = new MOS.Filter.HisImpMestFilter();
                    impMestFilter.ID = this.ImpMestId;
                    var rsImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, param).FirstOrDefault();
                    if (rsImpMest != null)
                    {
                        txtImpMestNumber.Text = rsImpMest.IMP_MEST_NUMBER;
                        mmConclude.Text = rsImpMest.CONCLUDE;
                    }
                    MOS.Filter.HisImpMestUserFilter userFilter = new MOS.Filter.HisImpMestUserFilter();
                    userFilter.IMP_MEST_ID = this.ImpMestId;
                    var rsImpMestUser = new BackendAdapter(param).Get<List<HIS_IMP_MEST_USER>>("api/HisImpMestUser/Get", ApiConsumers.MosConsumer, userFilter, param);
                    if (rsImpMestUser != null && rsImpMestUser.Count > 0)
                    {
                        foreach (var item in rsImpMestUser)
                        {
                            RoleUserAdo RoleUserAdo = new RoleUserAdo(item);
                            this.listRoleUserAdo.Add(RoleUserAdo);
                        }
                    }
                }
                if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    RoleUserAdo RoleUserAdo = new RoleUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                }
                else
                    btnPrintV2.Enabled = true;
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExecuteRoleFilter roleFilter = new MOS.Filter.HisExecuteRoleFilter();
                listExecuteRole = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE>>(HisRequestUriStore.HIS_EXECUTE_ROLE_GET, ApiConsumers.MosConsumer, roleFilter, param);
                InitComboLookUp(this.repositoryItemLookUp__ExecuteRoleName, listExecuteRole);

                MOS.Filter.HisExecuteRoleUserFilter roleUserFilter = new MOS.Filter.HisExecuteRoleUserFilter();
                listExecuteRoleUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, roleUserFilter, param);
                ComboAcsUser(this.repositoryItemLookUp__ExecuteRoleUser, BackendDataWorker.Get<ACS_USER>());

                MOS.Filter.HisImpUserTempFilter impUserTempFilter = new MOS.Filter.HisImpUserTempFilter();
                listImpUserTemp = new BackendAdapter(param).Get<List<HIS_IMP_USER_TEMP>>("api/HisImpUserTemp/Get", ApiConsumers.MosConsumer, impUserTempFilter, param);
                ComboImpUserTemp(this.cboImpUserTemp, listImpUserTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboImpUserTemp(GridLookUpEdit cbo, List<HIS_IMP_USER_TEMP> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ID", "", 150, -1));
                columnInfos.Add(new ColumnInfo("IMP_USER_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("IMP_USER_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data.OrderByDescending(o => o.CREATE_TIME).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo, List<ACS.EFMODEL.DataModels.ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLookUp(RepositoryItemLookUpEdit cbo, List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoleUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RoleUserAdo data = null;
                if (e.RowHandle > 0)
                {
                    data = (RoleUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ADD")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemButton__Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                RoleUserAdo RoleUserAdo = new RoleUserAdo();
                RoleUserAdo.Action = GlobalVariables.ActionEdit;
                this.listRoleUserAdo.Add(RoleUserAdo);
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (RoleUserAdo)gridViewRoleUser.GetFocusedRow();
                if (row != null)
                {
                    this.listRoleUserAdo.Remove(row);
                    if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                    {
                        RoleUserAdo RoleUserAdo = new RoleUserAdo();
                        this.listRoleUserAdo.Add(RoleUserAdo);
                    }
                    gridControlRoleUser.DataSource = null;
                    gridControlRoleUser.DataSource = this.listRoleUserAdo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemLookUp__ExecuteRoleName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.repositoryItemLookUp__ExecuteRoleUser.AllowFocused = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                if (gridViewRoleUser.HasColumnErrors)
                    return;
                if (this.listRoleUserAdo != null)
                {
                    WaitingManager.Show();
                    var listData = listRoleUserAdo.Where(o => o.EXECUTE_ROLE_ID > 0).ToList();

                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS_IMP_MEST_USER> ImpMestUsers = new List<HIS_IMP_MEST_USER>();
                    MOS.SDO.HisImpMestUserSDO inputAdo = new MOS.SDO.HisImpMestUserSDO();
                    foreach (var item in listData)
                    {
                        HIS_IMP_MEST_USER ado = new HIS_IMP_MEST_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_IMP_MEST_USER>(ado, item);
                        ado.USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == item.LOGINNAME).USERNAME;
                        ado.IMP_MEST_ID = this.ImpMestId;
                        ImpMestUsers.Add(ado);
                    }
                    inputAdo.ImpMestUsers = ImpMestUsers;
                    inputAdo.ImpMestId = this.ImpMestId;
                    inputAdo.ImpMestNumber = txtImpMestNumber.Text.Trim();
                    inputAdo.Conclude = mmConclude.Text.Trim();
                    var rsOutPut = new BackendAdapter(param).Post<List<HIS_IMP_MEST_USER>>("api/HisImpMestUser/CreateOrReplace", ApiConsumers.MosConsumer, inputAdo, param);
                    if (rsOutPut != null && rsOutPut.Count > 0)
                    {
                        success = true;
                        btnPrintV2.Enabled = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewRoleUser.PostEditor();
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrintV2.Enabled)
                {
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoleUser_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                gridViewRoleUser.ClearColumnErrors();
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RoleUserAdo data = view.GetFocusedRow() as RoleUserAdo;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;

                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        var rs = listExecuteRoleUser.Where(p => p.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList();
                        if (rs != null && rs.Count > 0)
                        {
                            loginNames = rs.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }

                    ComboAcsUser(editor, loginNames);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(LookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintType
        {
            BIEN_BAN_KIEM_NHAP_TU_NCC,
            BIEN_BAN_KIEM_NHAP_TU_NCC_MAU,
            PHIEU_NHAP_THUOC_VAT_TU_TU_NCC,
            PHIEU_NHAP_MAU_TU_NCC,
            BAR_CODE,
            PHIEU_NHAP_THU_HOI,
            PHIEU_NHAP_CHUYEN_KHO,
            PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                //WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                // nhap tu nha cung cap
                if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    if ((this.impMestMedicines != null && this.impMestMedicines.Count > 0) || (this.impMestMaterials != null && this.impMestMaterials.Count > 0))
                    {
                        DXMenuItem itemBienBankiemNhapNCC = new DXMenuItem("Biên bản kiểm nhập từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemBienBankiemNhapNCC.Tag = PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC;
                        menu.Items.Add(itemBienBankiemNhapNCC);
                        if (this.ImpMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                        {
                            DXMenuItem itemPhieuNhapTuNCC = new DXMenuItem("Phiếu nhập thuốc, vật tư từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                            itemPhieuNhapTuNCC.Tag = PrintType.PHIEU_NHAP_THUOC_VAT_TU_TU_NCC;
                            menu.Items.Add(itemPhieuNhapTuNCC);
                        }
                    }
                    else if (this.impMestBloods != null && this.impMestBloods.Count > 0)
                    {
                        DXMenuItem itemPhieuNhapMauTuNCC = new DXMenuItem("Phiếu nhập máu từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemPhieuNhapMauTuNCC.Tag = PrintType.PHIEU_NHAP_MAU_TU_NCC;
                        menu.Items.Add(itemPhieuNhapMauTuNCC);

                        DXMenuItem itemBienBanKiemNhapMau = new DXMenuItem("Biên bản kiểm nhập từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                        itemBienBanKiemNhapMau.Tag = PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC_MAU;
                        menu.Items.Add(itemBienBanKiemNhapMau);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("impMestMedicines va impMestMaterials va impMestBloods bi null");
                    }
                    //todo
                    //DXMenuItem itemBarCode = new DXMenuItem("In barcode", new EventHandler(OnClickInPhieuNhap));
                    //itemBarCode.Tag = PrintType.BAR_CODE;
                    //menu.Items.Add(itemBarCode);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                {
                    DXMenuItem itemPhieuNhapThuHoi = new DXMenuItem("Phiếu nhập thu hồi", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapThuHoi.Tag = PrintType.PHIEU_NHAP_THU_HOI;
                    menu.Items.Add(itemPhieuNhapThuHoi);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                {
                    DXMenuItem itemPhieuNhapChuyenKho = new DXMenuItem("Phiếu nhập chuyển kho", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKho.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO;
                    menu.Items.Add(itemPhieuNhapChuyenKho);

                    DXMenuItem itemPhieuNhapChuyenKhoThuocGayNghienHuongThan = new DXMenuItem("Phiếu nhập chuyển kho thuốc gây nghiện, hướng thần", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKhoThuocGayNghienHuongThan.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN;
                    menu.Items.Add(itemPhieuNhapChuyenKhoThuocGayNghienHuongThan);

                    DXMenuItem itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan = new DXMenuItem("Phiếu nhập chuyển kho không phải thuốc gây nghiện, hướng thần", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan.Tag = PrintType.PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN;
                    menu.Items.Add(itemPhieuNhapChuyenKhoKhongPhaiThuocGayNghienHuongThan);
                }
                else if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    DXMenuItem itemPhieuNhapKiemKeDauKyKhac = new DXMenuItem("Phiếu nhập kiểm kê, đầu kỳ, khác", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapKiemKeDauKyKhac.Tag = PrintType.PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC;
                    menu.Items.Add(itemPhieuNhapKiemKeDauKyKhac);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay PrintTypeCode");
                }

                btnPrintV2.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuNhap(object sender, EventArgs e) // Xử lý khi người dùng chọn một mục trong menu "In"
        {
            try
            {
                LoadSpecificImpMest();
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load các phiếu nhập cụ thể dựa vào loại nhập đang chọn
        private void LoadSpecificImpMest()
        {
            try
            {
                if (this.impMest == null)
                {
                    MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                    impMestViewFilter.ID = this.ImpMestId;
                    var impMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam());
                    if (impMests != null && impMests.Count > 0)
                    {
                        this.impMest = impMests.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(PrintType printType) //Gọi mẫu in tương ứng (ví dụ: Mps000085, Mps000149).
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_THUOC_VAT_TU_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_TU_NCC__MPS000141, DelegateRunPrinter);
                        break;
                    case PrintType.BAR_CODE:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__BARCODE__MPS000142, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_THU_HOI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO__MPS000143, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN__MPS000142, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN__MPS000145, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_MAU_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_NHAP_MAU_TU_NCC_MPS000149, DelegateRunPrinter);
                        break;
                    case PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC_MAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085, DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStoreV2.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC__MPS000199, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085:
                        if (this.impMestBloods != null && this.impMestBloods.Count > 0)
                        {
                            InBienBanKiemNhapTuNhaCungCapMau(printTypeCode, fileName, ref result);
                        }
                        else
                        {
                            InBienBanKiemNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                        }
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_TU_NCC__MPS000141:
                        InPhieuNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO__MPS000143:
                        InPhieuNhapChuyenKho(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN__MPS000142:
                        InPhieuNhapChuyenKhoThuocGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_CHUYEN_KHO_THUOC_KHONG_PHAI_GAY_NGHIEN_HUONG_THAN__MPS000145:
                        InPhieuNhapChuyenKhoThuocKhongPhaiGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084:
                        InPhieuNhapThuHoi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_NHAP_MAU_TU_NCC_MPS000149:
                        InPhieuNhapMauTuNCC(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStoreV2.PRINT_TYPE_CODE__BIEUMAU__PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC__MPS000199:
                        InPhieuNhapKiemKeDauKyKhac(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void InBienBanKiemNhapTuNhaCungCapMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                // Lấy dữ liệu máu
                MOS.Filter.HisImpMestBloodViewFilter bloodFilter = new MOS.Filter.HisImpMestBloodViewFilter();
                bloodFilter.IMP_MEST_ID = this.ImpMestId;

                var impMestBloods = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>(
                    HisRequestUriStore.HIS_IMP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);

                // Lấy thông tin người dùng trong phiếu nhập
                MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                userFilter.IMP_MEST_ID = this.ImpMestId;
                var impMestUsers = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>(
                    "/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                impMestUsers = impMestUsers.OrderBy(o => o.ID).ToList();


                // Lấy thông tin phiếu nhập như mã phiếu, ngày nhập, trạng thái, ...
                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.ImpMestId;
                var impMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>(
                    HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, impMestViewFilter, param).FirstOrDefault();

                // Chuẩn bị dữ liệu cho báo cáo Mps000085
                MPS.Processor.Mps000085.PDO.Mps000085PDO pdo = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                    impMest,
                    null, // Không có thuốc
                    null, // Không có vật tư
                    impMestUsers,
                    null, // Không có dữ liệu thuốc
                    null, // Không có dữ liệu vật tư
                    null,
                    null,
                    impMestBloods
                );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InPhieuNhapMauTuNCC(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                // WaitingManager.Show();
                MPS.Processor.Mps000149.PDO.Mps000149PDO pdo = new MPS.Processor.Mps000149.PDO.Mps000149PDO(
                 this.impMest,
                 this.impMestBloods
                );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapThuHoi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisImpMestFilter hisImpMestFilter = new MOS.Filter.HisImpMestFilter();
                hisImpMestFilter.ID = this.ImpMestId;
                var ImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, hisImpMestFilter, param).FirstOrDefault();
                this.impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                MOS.Filter.HisImpMestMedicineFilter impMestMedicineViewFilter = new HisImpMestMedicineFilter();
                impMestMedicineViewFilter.IMP_MEST_ID = this.ImpMestId;
                this.impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                this.impMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_ID = this.ImpMestId;
                this.impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);

                MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                expMestMaterialViewFilter.EXP_MEST_ID = ImpMest.MOBA_EXP_MEST_ID;
                var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, param);

                MPS.Processor.Mps000084.PDO.Mps000084PDO pdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(
                this.impMest,
                this.impMestMedicines,
                this.impMestMaterials,
                expMestMedicines,
                expMestMaterials
                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapChuyenKhoThuocKhongPhaiGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                //WaitingManager.Show();
                MPS.Processor.Mps000145.PDO.Mps000145PDO pdo = new MPS.Processor.Mps000145.PDO.Mps000145PDO(
                 this.impMest,
                 this.impMestMedicines
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapChuyenKhoThuocGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                // WaitingManager.Show();
                MPS.Processor.Mps000142.PDO.Mps000142PDO pdo = new MPS.Processor.Mps000142.PDO.Mps000142PDO(
                 this.impMest,
                 this.impMestMedicines
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapChuyenKho(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.impMest == null)
                    return;

                if ((this.impMestMaterials != null && this.impMestMaterials.Count > 0) || (this.impMestMedicines != null && this.impMestMedicines.Count > 0))
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    MOS.Filter.HisExpMestViewFilter hisExpMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                    hisExpMestViewFilter.ID = this.impMest.CHMS_EXP_MEST_ID;
                    var expMestView = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, hisExpMestViewFilter, null);

                    MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key mps000143Key = new MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key();

                    if (expMestView != null && expMestView.Count > 0)
                    {
                        mps000143Key.EXP_MEDI_STOCK_CODE = expMestView.FirstOrDefault().MEDI_STOCK_CODE;
                        mps000143Key.EXP_MEDI_STOCK_NAME = expMestView.FirstOrDefault().MEDI_STOCK_NAME;
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                    WaitingManager.Hide();

                    long keyPrintType = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_TRA);

                    if (keyPrintType == 1)
                    {
                        mps000143Key.KEY_NAME_TITLES = "";
                        MPS.Processor.Mps000143.PDO.Mps000143PDO rdo = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, this.impMestMedicines, this.impMestMaterials, mps000143Key, ConfigApplications.NumberSeperator);
                        PrintData(printTypeCode, fileName, rdo, false, inputADO, ref result);
                    }
                    else
                    {
                        var _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

                        if (this.impMestMedicines != null && this.impMestMedicines.Count > 0)
                        {
                            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);

                            _ImpMestMedi_Ts = this.impMestMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                            _ImpMestMedi_GNs = this.impMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                            _ImpMestMedi_HTs = this.impMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                            _ImpMestMedi_TDs = this.impMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                            _ImpMestMedi_PXs = this.impMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();

                            _ImpMestMedi_Others = this.impMestMedicines.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                                p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                        }

                        #region thuoc thuong
                        if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "THUỐC THƯỜNG";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ts = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_Ts, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_Ts, false, inputADO, ref result);
                        }
                        #endregion

                        #region Gay nghien, huong than
                        if ((_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0) || (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0))
                        {
                            long keyPrintTypeHTGN = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                            if (keyPrintTypeHTGN == 1)
                            {
                                List<V_HIS_IMP_MEST_MEDICINE> DataGroups = new List<V_HIS_IMP_MEST_MEDICINE>();

                                if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                {
                                    DataGroups.AddRange(_ImpMestMedi_GNs);
                                }

                                if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                {
                                    DataGroups.AddRange(_ImpMestMedi_HTs);
                                }

                                mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN, HƯỚNG THẦN";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNHTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, DataGroups, null, mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_GNHTs, false, inputADO, ref result);
                            }
                            else
                            {
                                if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                {
                                    mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN";
                                    MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_GNs, null, mps000143Key, ConfigApplications.NumberSeperator);
                                    PrintData(printTypeCode, fileName, rdo_GNs, false, inputADO, ref result);
                                }

                                if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                {
                                    mps000143Key.KEY_NAME_TITLES = "HƯỚNG THẦN";
                                    MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_HTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_HTs, null, mps000143Key, ConfigApplications.NumberSeperator);
                                    PrintData(printTypeCode, fileName, rdo_HTs, false, inputADO, ref result);
                                }
                            }
                        }
                        #endregion

                        #region thuoc doc
                        if (_ImpMestMedi_TDs != null && _ImpMestMedi_TDs.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "ĐỘC";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_TDs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_TDs, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_TDs, false, inputADO, ref result);
                        }
                        #endregion

                        #region thuoc phong xa
                        if (_ImpMestMedi_PXs != null && _ImpMestMedi_PXs.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "PHÓNG XẠ";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_PXs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_PXs, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_PXs, false, inputADO, ref result);
                        }
                        #endregion

                        #region thuoc khac
                        if (_ImpMestMedi_Others != null && _ImpMestMedi_Others.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "KHÁC";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ks = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, _ImpMestMedi_Others, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_Ks, false, inputADO, ref result);
                        }
                        #endregion

                        #region vat tu
                        if (this.impMestMaterials != null && this.impMestMaterials.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "VẬT TƯ";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_VTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.impMest, null, this.impMestMaterials, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_VTs, false, inputADO, ref result);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                List<HIS_MEDICINE> _Medicines = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> _Materials = new List<HIS_MATERIAL>();
                if (this.impMestMedicines != null && this.impMestMedicines.Count > 0)
                {
                    List<long> _MedicineIds = impMestMedicines.Select(p => p.MEDICINE_ID).ToList();
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = _MedicineIds;
                    _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);

                }
                if (impMestMaterials != null && impMestMaterials.Count > 0)
                {
                    List<long> _MaterialIds = impMestMaterials.Select(p => p.MATERIAL_ID).ToList();
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = _MaterialIds;
                    _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                }

                MPS.Processor.Mps000141.PDO.Mps000141PDO pdo = new MPS.Processor.Mps000141.PDO.Mps000141PDO(
                 this.impMest,
                 this.impMestMedicines,
                 this.impMestMaterials,
                 _Medicines,
                 _Materials,
                 BackendDataWorker.Get<HIS_IMP_SOURCE>()
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuNhapKiemKeDauKyKhac(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.ImpMestId;
                var impMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam());
                if (impMests != null && impMests.Count > 0)
                {
                    this.impMest = impMests.FirstOrDefault();
                }
                MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                userFilter.IMP_MEST_ID = this.ImpMestId;
                var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();
                if (this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK
                    || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK
                    || this.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.ImpMestId;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    #region -- Bo
                    //List<HIS_MEDICINE> _Medicines = new List<HIS_MEDICINE>();
                    //List<HIS_MATERIAL> _Materials = new List<HIS_MATERIAL>();
                    //List<HIS_BLOOD> _Bloods = new List<HIS_BLOOD>();
                    //if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    //{
                    //    List<long> _MedicineIds = hisImpMestMedicines.Select(p => p.MEDICINE_ID).ToList();
                    //    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    //    medicineFilter.IDs = _MedicineIds;
                    //    _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);

                    //}
                    //if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    //{
                    //    List<long> _MaterialIds = hisImpMestMaterials.Select(p => p.MATERIAL_ID).ToList();
                    //    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    //    materialFilter.IDs = _MaterialIds;
                    //    _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    //}
                    //if (impMestBloods != null && impMestBloods.Count > 0)
                    //{
                    //    List<long> _BloodIds = impMestBloods.Select(p => p.BLOOD_ID).ToList();
                    //    MOS.Filter.HisBloodFilter bloodFilter = new HisBloodFilter();
                    //    bloodFilter.IDs = _BloodIds;
                    //    _Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodFilter, param);
                    //}
                    #endregion

                    MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                        this.impMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        impMestBloods,
                        _ImpMestUser
                        );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienBanKiemNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                userFilter.IMP_MEST_ID = this.ImpMestId;
                var rs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                rs = rs.OrderBy(p => p.ID).ToList();

                V_HIS_IMP_MEST impMests = new V_HIS_IMP_MEST();
                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.ImpMestId;
                var dataImpMests = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam());
                if (dataImpMests != null && dataImpMests.Count > 0)
                {
                    impMests = dataImpMests.FirstOrDefault();
                }

                List<HIS_MEDICINE> _MedicineDatas = new List<HIS_MEDICINE>();
                if (this.impMestMedicines != null && this.impMestMedicines.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter mediFilter = new HisMedicineFilter();
                    mediFilter.IDs = this.impMestMedicines.Select(p => p.MEDICINE_ID).Distinct().ToList();
                    _MedicineDatas = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, new CommonParam());
                }

                List<HIS_MATERIAL> _MaterialDatas = new List<HIS_MATERIAL>();
                if (this.impMestMaterials != null && this.impMestMaterials.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter mateFilter = new HisMaterialFilter();
                    mateFilter.IDs = this.impMestMaterials.Select(p => p.MATERIAL_ID).Distinct().ToList();
                    _MaterialDatas = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, new CommonParam());
                }
                HIS_SUPPLIER _Supplier = new HIS_SUPPLIER();
                if (impMests != null && impMests.SUPPLIER_ID > 0)
                {
                    MOS.Filter.HisSupplierFilter supplierFilter = new HisSupplierFilter();
                    supplierFilter.ID = impMests.SUPPLIER_ID;
                    var _SupplierDatas = new BackendAdapter(new CommonParam()).Get<List<HIS_SUPPLIER>>(HisRequestUriStore.HIS_SUPPLIER_GET, ApiConsumer.ApiConsumers.MosConsumer, supplierFilter, new CommonParam());
                    if (_SupplierDatas != null && _SupplierDatas.Count > 0)
                    {
                        _Supplier = _SupplierDatas.FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000085.PDO.Mps000085PDO mps0000085RDO = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                    impMests,
                 this.impMestMedicines,
                 this.impMestMaterials,
                 rs,
                 _MedicineDatas,
                 _MaterialDatas,
                 _Supplier
                  );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoleUser_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["LOGINNAME"];
                    var data = (RoleUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (String.IsNullOrEmpty(data.LOGINNAME))
                        {
                            e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                        else
                        {
                            var ktra = this.listRoleUserAdo.Where(p => p.LOGINNAME == data.LOGINNAME).ToList();
                            if (ktra != null && ktra.Count > 1)
                            {
                                e.Valid = false;
                                view.SetColumnError(onOrderCol, "'" + BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == data.LOGINNAME).USERNAME + "'" + " đã được gán vai trò");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoleUser_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                frmHisImpUserTempCreate frm = new frmHisImpUserTempCreate(listRoleUserAdo.Where(o => o.EXECUTE_ROLE_ID > 0).ToList(), RefeshImpUserTemp);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshImpUserTemp()
        {
        }

        private void cboImpUserTemp_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboImpUserTemp.EditValue != null)
                    {
                        WaitingManager.Show();
                        HIS_IMP_USER_TEMP data = (cboImpUserTemp.Properties.DataSource as List<HIS_IMP_USER_TEMP>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboImpUserTemp.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.ProcessChoiceImpUserTemp(data);
                        }
                        else
                        {
                            txtImpMestNumber.Text = null;
                            mmConclude.Text = null;
                        }
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtImpMestNumber.Text = null;
                        mmConclude.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChoiceImpUserTemp(HIS_IMP_USER_TEMP data)
        {
            try
            {
                txtImpMestNumber.Text = data.IMP_MEST_NUMBER;
                mmConclude.Text = data.CONCLUDE;
                this.ProcessGetImpUserTempDt(this.GetImpMestUserTempDtByImpUserTempId(data.ID));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetImpUserTempDt(List<HIS_IMP_USER_TEMP_DT> listImpUserTempDt)
        {
            try
            {
                listRoleUserAdo = new List<RoleUserAdo>();
                foreach (var item in listImpUserTempDt)
                {
                    RoleUserAdo rdo = new RoleUserAdo();
                    rdo.EXECUTE_ROLE_ID = item.EXECUTE_ROLE_ID;
                    rdo.LOGINNAME = item.LOGINNAME;
                    rdo.USERNAME = item.USERNAME;
                    listRoleUserAdo.Add(rdo);
                }
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP_DT> GetImpMestUserTempDtByImpUserTempId(long ImpUserTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpUserTempDtFilter filter = new HisImpUserTempDtFilter();
                filter.IMP_USER_TEMP_ID = ImpUserTempId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<HIS_IMP_USER_TEMP_DT>>("api/HisImpUserTempDt/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void cboImpUserTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboImpUserTemp.EditValue != null)
                    {
                        WaitingManager.Show();
                        HIS_IMP_USER_TEMP data = (cboImpUserTemp.Properties.DataSource as List<HIS_IMP_USER_TEMP>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboImpUserTemp.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.ProcessChoiceImpUserTemp(data);
                        }
                        else
                        {
                            txtImpMestNumber.Text = null;
                            mmConclude.Text = null;
                            gridControlRoleUser.DataSource = null;
                        }
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtImpMestNumber.Text = null;
                        mmConclude.Text = null;
                        gridControlRoleUser.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintData(string printTypeCode, string fileName, object data, bool printNow, Inventec.Common.SignLibrary.ADO.InputADO inputADO, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
