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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignServiceTest.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTest.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        //Load người chỉ định
        //private void InitComboUser()
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
        //        columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
        //        ControlEditorLoader.Load(cboUser, BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(), controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemcboPatientType_TabService, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemcboPatientType_TabService, currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void InitComboIcd()
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
        //        columnInfos.Add(new ColumnInfo("ICD_NAME", "", 350, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 500);
        //        ControlEditorLoader.Load(cboIcdServiceReq, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>(), controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void InitComboExecuteRoom()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(repositoryItemcboExcuteRoom_TabService, BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(excuteRoomCombo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridCheckMarksSelectionServiceGroup()
        {
            try
            {
                this.cboServiceGroup.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceGroup_CustomDisplayText);
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceGroup.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboServiceGroup__SelectionChange);
                cboServiceGroup.Properties.Tag = gridCheck;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load nhóm dịch vụ, chỉ load ra các nhóm dịch vụ do người dùng tạo hoặc is_public = 1
        private void InitComboServiceGroup()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var serviceGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();
                serviceGroups = serviceGroups.Where(o => (o.IS_PUBLIC ?? -1) == 1 || o.CREATOR.ToLower() == loginName.ToLower()).ToList();
                cboServiceGroup.Properties.DataSource = serviceGroups;
                cboServiceGroup.Properties.DisplayMember = "SERVICE_GROUP_NAME";
                cboServiceGroup.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboServiceGroup.Properties.PopupFormWidth = 320;
                cboServiceGroup.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                this.selectedSeviceGroups = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
