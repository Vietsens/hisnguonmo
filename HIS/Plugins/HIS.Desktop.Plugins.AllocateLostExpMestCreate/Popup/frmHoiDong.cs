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
using ACS.Filter;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AllocateLostExpMestCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AllocateLostExpMestCreate.Popup
{
    public partial class frmHoiDong : HIS.Desktop.Utility.FormBase
    {
        List<HIS_EXP_MEST_USER> listExpMestUser;
        List<RoleADO> listRoleADO;
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial;
        List<HIS_EXECUTE_ROLE_USER> listExecuteRoleUser;
        List<HIS_EXECUTE_ROLE> listExecuteRole;
        List<ACS_USER> listUser;
        long expMestId;
        long lostExpMestId;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmHoiDong(List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine, List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial, long expMestId, long lostExpMestId)
        {
            InitializeComponent();
            try
            {
                this.listExpMestMaterial = listExpMestMaterial;
                this.listExpMestMedicine = listExpMestMedicine;
                this.expMestId = expMestId;
                this.lostExpMestId = lostExpMestId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmHoiDong_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataToCombo();
                LoadDataToGrid();
                InitMenuToButtonPrint();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisExecuteRoleFilter executeRoleFilter = new HisExecuteRoleFilter();
                executeRoleFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listExecuteRole = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, executeRoleFilter, param);
                InitComboExecuteRole(Res_GridLookUp_Role, listExecuteRole);

                listUser = new List<ACS_USER>();
                listUser = BackendDataWorker.Get<ACS_USER>();
                InitComboAcsUserRepository(Res_GridLookUp_UserName, listUser);

                HisExecuteRoleUserFilter executeRoleUserFilter = new HisExecuteRoleUserFilter();
                listExecuteRoleUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, executeRoleUserFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToGrid()
        {
            try
            {
                listRoleADO = new List<RoleADO>();
                CommonParam param = new CommonParam();

                HisExpMestUserFilter expMestUserFilter = new HisExpMestUserFilter();
                expMestUserFilter.EXP_MEST_ID = expMestId;
                listExpMestUser = new BackendAdapter(param).Get<List<HIS_EXP_MEST_USER>>("api/HisExpMestUser/Get", ApiConsumers.MosConsumer, expMestUserFilter, param);

                if (listExpMestUser != null && listExpMestUser.Count > 0)
                {
                    foreach (var item in listExpMestUser)
                    {
                        RoleADO roleAdo = new RoleADO(item);
                        roleAdo.Action = true;
                        listRoleADO.Add(roleAdo);
                    }
                }

                if (this.listRoleADO == null || this.listRoleADO.Count == 0)
                {
                    RoleADO roleAdo = new RoleADO();
                    roleAdo.Action = true;
                    listRoleADO.Add(roleAdo);
                }

                RefreshDataSourceGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboAcsUserRepository(RepositoryItemGridLookUpEdit gridLookUp, List<ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(gridLookUp, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboAcsUserGridLookUp(GridLookUpEdit gridLookUp, List<ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(gridLookUp, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboExecuteRole(RepositoryItemGridLookUpEdit gridLookUp, List<HIS_EXECUTE_ROLE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(gridLookUp, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshDataSourceGrid()
        {
            try
            {
                gridControlHoiDong.BeginUpdate();
                gridControlHoiDong.DataSource = listRoleADO;
                gridControlHoiDong.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    WaitingManager.Show();
                    if (this.listRoleADO != null)
                    {
                        var listData = listRoleADO.Where(o => o.EXECUTE_ROLE_ID > 0).ToList();

                        bool success = false;
                        CommonParam param = new CommonParam();
                        List<HIS_EXP_MEST_USER> ExpMestUsers = new List<HIS_EXP_MEST_USER>();
                        MOS.SDO.HisExpMestUserSDO inputAdo = new MOS.SDO.HisExpMestUserSDO();
                        foreach (var item in listData)
                        {
                            HIS_EXP_MEST_USER ado = new HIS_EXP_MEST_USER();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_USER>(ado, item);
                            ado.USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                                 == item.LOGINNAME).USERNAME;
                            ado.EXP_MEST_ID = this.expMestId;
                            ExpMestUsers.Add(ado);
                        }
                        inputAdo.ExpMestUsers = ExpMestUsers;
                        inputAdo.ExpMestId = this.expMestId;
                        var rsOutPut = new BackendAdapter(param).Post<List<HIS_EXP_MEST_USER>>("api/HisExpMestUser/CreateOrReplace", ApiConsumers.MosConsumer, inputAdo, param);
                        if (rsOutPut != null && rsOutPut.Count > 0)
                        {
                            success = true;
                            //btnPrintV2.Enabled = true;
                        }
                        MessageManager.Show(this.ParentForm, param, success);

                    }
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Res_Button_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                RoleADO roleAdo = new RoleADO();
                roleAdo.Action = true;
                listRoleADO.Add(roleAdo);

                RefreshDataSourceGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Res_Button_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (RoleADO)gridViewHoiDong.GetFocusedRow();
                if (row != null)
                {
                    listRoleADO.Remove(row);
                    RefreshDataSourceGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHoiDong_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle > 0)
                {
                    var data = (RoleADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ADD")
                    {
                        if (data.Action == false)
                        {
                            e.RepositoryItem = Res_Button_Add;
                        }
                        else
                        {
                            e.RepositoryItem = Res_Button_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewHoiDong_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RoleADO data = view.GetFocusedRow() as RoleADO;
                List<ACS_USER> acsUser = new List<ACS_USER>();
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (listExecuteRoleUser != null && listExecuteRoleUser.Count > 0)
                        {
                            var rs = listExecuteRoleUser.Where(p => p.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList();
                            if (rs != null && rs.Count > 0)
                            {
                                acsUser = listUser.Where(m => rs.Select(k => k.LOGINNAME).Contains(m.LOGINNAME)).ToList();
                            }
                        }
                    }

                    InitComboAcsUserGridLookUp(editor, acsUser);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewHoiDong_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["LOGINNAME"];
                    var data = (RoleADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (String.IsNullOrEmpty(data.LOGINNAME.Trim()))
                        {
                            e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                        else
                        {
                            var ktra = this.listRoleADO.Where(p => p.LOGINNAME == data.LOGINNAME).ToList();
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

        private void gridViewHoiDong_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region Print

        internal enum PrintType
        {
            PHIEU_XUAT_MAT_MAT
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuXuatMatMat = new DXMenuItem("BIÊN BẢN XÁC NHẬN THUỐC/HÓA CHẤT/VẬT TƯ Y TẾ MẤT/HỎNG/VỠ", new EventHandler(OnClickInPhieuXuatKho));
                itemPhieuXuatMatMat.Tag = PrintType.PHIEU_XUAT_MAT_MAT;
                menu.Items.Add(itemPhieuXuatMatMat);

                ddPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.PHIEU_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__HOI_DONG_PHIEU_XUAT_MAT_MAT__MPS000205, DelegateRunPrinter);
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
                    case HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__HOI_DONG_PHIEU_XUAT_MAT_MAT__MPS000205:
                        InPhieuXuatMatMat(printTypeCode, fileName, ref result);
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

        private void InPhieuXuatMatMat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                MOS.Filter.HisLostExpMestViewFilter lostExpMestFilter = new MOS.Filter.HisLostExpMestViewFilter();
                lostExpMestFilter.ID = this.lostExpMestId;
                var lostExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_LOST_EXP_MEST>>(HIS.Desktop.Plugins.AllocateLostExpMestCreate.HisRequestUriStore.MOSHIS_LOST_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, lostExpMestFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestView1Filter expMestViewFilter = new MOS.Filter.HisExpMestView1Filter();
                expMestViewFilter.ID = this.expMestId;
                var expMest1 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1>>("api/HisExpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestUserViewFilter expMestUserViewFilter = new MOS.Filter.HisExpMestUserViewFilter();
                expMestUserViewFilter.EXP_MEST_ID = this.expMestId;
                var expMestUser = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_USER>>("api/HisExpMestUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestUserViewFilter, param);

                MPS.Processor.Mps000205.PDO.Mps000205PDO pdo = new MPS.Processor.Mps000205.PDO.Mps000205PDO(
                    expMest1,
                 lostExpMest,
                 this.listExpMestMedicine,
                 this.listExpMestMaterial,
                    expMestUser
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
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
