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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.CustomControl.CustomGrid;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : UserControlBase
    {

        private async Task AddDataToCombo()
        {
            try
            {
                InitComboAcsUser();
                InitComboMultilCheck(cboService, "SERVICE_NAME", "ID", ServiceList.ToList());
                InitComboGridServiceCheck();
                ComboExecuteRole();
                cboStt.SelectedIndex = 1;
                InitCombo(cboDepartment, "DEPARTMENT_CODE", "DEPARTMENT_NAME", "ID", DepartmentList);
                InitCombo(cboPtttMethod, "PTTT_METHOD_CODE", "PTTT_METHOD_NAME", "ID", PtttMethodList);
                InitCombo(cboEmotionLessMethod, "EMOTIONLESS_METHOD_CODE", "EMOTIONLESS_METHOD_NAME", "ID", EmotionLessMethodList);
                InitCombo(cboPtttMethodReal, "PTTT_METHOD_CODE", "PTTT_METHOD_NAME", "ID", PtttMethodList);
                InitCombo(cboPtttGroup, "PTTT_GROUP_CODE", "PTTT_GROUP_NAME", "ID", PtttGroupList);
                InitCombo(cboEkipUser, "ID", "EKIP_TEMP_NAME", "ID", EkipTempList);
                InitCombo(repDepartment, "DEPARTMENT_CODE", "DEPARTMENT_NAME", "ID", DepartmentList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private async Task ComboExecuteRole()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(repExecute, ExecuteRoleList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboAcsUser()
        {
            try
            {
                repUser.DataSource = this.AcsUserADOList;
                repUser.DisplayMember = "USERNAME";
                repUser.ValueMember = "LOGINNAME";

                repUser.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repUser.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repUser.ImmediatePopup = true;
                repUser.View.Columns.Clear();

                GridColumn aColumnCode = repUser.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = repUser.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumnDOB = repUser.View.Columns.AddField("DOB");
                aColumnDOB.Caption = "DOB";
                aColumnDOB.Visible = true;
                aColumnDOB.VisibleIndex = 3;
                aColumnDOB.Width = 100;

                GridColumn aColumnDIPLOMA = repUser.View.Columns.AddField("DIPLOMA");
                aColumnDOB.Caption = "CCHN";
                aColumnDOB.Visible = true;
                aColumnDOB.VisibleIndex = 4;
                aColumnDOB.Width = 100;



                GridColumn aColumnDepartment = repUser.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 5;
                aColumnDepartment.Width = 200;

                repUser.PopupFormSize = new System.Drawing.Size(700, repUser.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                if (loginNames != null && loginNames.Count > 0)
                {

                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).ToList();

                }
                else
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => o.IS_ACTIVE == 1).ToList();
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

                GridColumn aColumDOB = cbo.Properties.View.Columns.AddField("DOB");
                aColumDOB.Caption = "Ngày sinh";
                aColumDOB.Visible = true;
                aColumDOB.VisibleIndex = 3;
                aColumDOB.Width = 100;

                GridColumn aColumnDIPLOMA = cbo.Properties.View.Columns.AddField("DIPLOMA");
                aColumnDIPLOMA.Caption = "CCHN";
                aColumnDIPLOMA.Visible = true;
                aColumnDIPLOMA.VisibleIndex = 4;
                aColumnDIPLOMA.Width = 100;

                GridColumn aColumnDepartment = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Tên khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 5;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMultilCheck(Inventec.Desktop.CustomControl.CustomGrid.CustomGridLookUpEdit cbo, string displayName, string Value, object lst)
        {
            try
            {
                cbo.Properties.DataSource = lst;
                cbo.Properties.DisplayMember = displayName;
                cbo.Properties.ValueMember = Value;
                DevExpress.XtraGrid.Columns.GridColumn column = cbo.Properties.View.Columns.AddField(displayName);
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = "Tất cả";
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboGridServiceCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboService.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboService.Properties.Tag = gridCheck;
                cboService.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboService.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboService.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_Check(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                serviceSelecteds = new List<V_HIS_SERVICE>();
                if (gridCheckMark != null)
                {
                    List<V_HIS_SERVICE> erSelectedNews = new List<V_HIS_SERVICE>();
                    foreach (V_HIS_SERVICE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.SERVICE_NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.serviceSelecteds = new List<V_HIS_SERVICE>();
                    this.serviceSelecteds.AddRange(erSelectedNews);
                }
                this.cboService.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, string displayCode, string displayName, string Value, object lst)
        {

            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayCode, "Mã", 150, 1));
                columnInfos.Add(new ColumnInfo(displayName, "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayName, Value, columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, lst, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.PopupFormSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
                cbo.Properties.AutoComplete = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCombo(Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit cbo, string displayCode, string displayName, string Value, object lst)
        {

            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayCode, "Mã", 150, 1));
                columnInfos.Add(new ColumnInfo(displayName, "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayName, Value, columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, lst, controlEditorADO);
                cbo.ImmediatePopup = true;
                cbo.PopupFormSize = new System.Drawing.Size(400, cbo.PopupFormSize.Height);
                cbo.AutoComplete = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
