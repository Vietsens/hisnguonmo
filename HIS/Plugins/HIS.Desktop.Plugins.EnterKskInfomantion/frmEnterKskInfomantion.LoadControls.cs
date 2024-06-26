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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EnterKskInfomantion.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        private void FillDataToControlsForm()
        {
            try
            {
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_EXAM == 1).ToList();
                listExecuteRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_EXAM == 1).ToList();

                InitServiceReqSttCheck();
                InitComboServiceReqStt();
                loadcomboBSketluan();
                InitComboKSKContract();

                InitComboHealthExamRank_ForAll();
                InitCheck(cboDepartmentToSearch, SelectionGrid__DepartmentToSearch);
                InitCombo(cboDepartmentToSearch, listDepartment, "DEPARTMENT_NAME", "ID");
                InitCheck(cboExamRoomToSearch, SelectionGrid__ExamRoomToSearch);
                InitCombo(cboExamRoomToSearch, _DepartmentSearchSelecteds != null && _DepartmentSearchSelecteds.Count > 0 ? listExecuteRoom.Where(o => _DepartmentSearchSelecteds.Select(p => p.ID).Contains(o.DEPARTMENT_ID)).ToList() : null, "EXECUTE_ROOM_NAME", "ID", true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ExamRoomToSearch(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                _ExecuteRoomSearchSelecteds = new List<V_HIS_EXECUTE_ROOM>();
                foreach (V_HIS_EXECUTE_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXECUTE_ROOM_NAME);
                        _ExecuteRoomSearchSelecteds.Add(rv);
                    }
                }
                this.cboExamRoomToSearch.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember, bool isExamRoom = false)
        {
            try
            {
                if(data != null && data is List<V_HIS_EXECUTE_ROOM>)
                {
                    listDepartmentRoom = data as List<V_HIS_EXECUTE_ROOM>;
                }
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                if (isExamRoom)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                    col1.VisibleIndex = 1;
                    col1.Width = 100;
                    col1.Caption = Resources.ResourceMessage.MaPhong;
                }
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(isExamRoom ? "EXECUTE_ROOM_NAME" : DisplayValue);
                col2.VisibleIndex = isExamRoom ? 2 : 1;
                col2.Width = isExamRoom ? 200 : 100;
                col2.Caption = isExamRoom ? Resources.ResourceMessage.TenPhong : Resources.ResourceMessage.TatCa;
                cbo.Properties.PopupFormWidth = 300;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__DepartmentToSearch(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                _DepartmentSearchSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.DEPARTMENT_NAME);
                        _DepartmentSearchSelecteds.Add(rv);
                    }
                }
                List<V_HIS_EXECUTE_ROOM> data = listExecuteRoom;
                if (_DepartmentSearchSelecteds != null && _DepartmentSearchSelecteds.Count > 0)
                {
                    if(_DepartmentSearchSelecteds.Count != listDepartment.Count)
                        data = listExecuteRoom.Where(o => _DepartmentSearchSelecteds.Select(p => p.ID).Contains(o.DEPARTMENT_ID)).ToList();
                }
                else
                {
                    data = null;
                }
                cboExamRoomToSearch.Properties.DataSource = data;
                GridCheckMarksSelection gridCheckMarkExamRoomToSearch = cboExamRoomToSearch.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkExamRoomToSearch != null)
                {
                    gridCheckMarkExamRoomToSearch.SelectAll(cboExamRoomToSearch.Properties.DataSource);
                }
                this.cboDepartmentToSearch.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceReqStt()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>();
                if (datas != null)
                {
                    cboServiceReqStt.Properties.DataSource = datas;
                    cboServiceReqStt.Properties.DisplayMember = "SERVICE_REQ_STT_NAME";
                    cboServiceReqStt.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceReqStt.Properties.View.Columns.AddField("SERVICE_REQ_STT_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = Resources.ResourceMessage.TatCa;
                    cboServiceReqStt.Properties.PopupFormWidth = 200;
                    cboServiceReqStt.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboServiceReqStt.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboServiceReqStt.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboServiceReqStt.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboKSKContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<KSKContractADO> listKSKContract = new List<KSKContractADO>();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_KSK_CONTRACT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        listKSKContract.Add(new KSKContractADO(item));
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", Resources.ResourceMessage.MaHopDong, 50, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", Resources.ResourceMessage.TenCongTy, 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 200);
                ControlEditorLoader.Load(cboKSKContract, listKSKContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitServiceReqSttCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceReqStt.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceReqStt);
                cboServiceReqStt.Properties.Tag = gridCheck;
                cboServiceReqStt.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceReqStt.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceReqStt.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__ServiceReqStt(object sender, EventArgs e)
        {
            try
            {
                serviceReqSttSelecteds = new List<HIS_SERVICE_REQ_STT>();
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        serviceReqSttSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboHealthExamRank_ForAll()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEALTH_EXAM_RANK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboExamCirculationRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamCirculationRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamRepiratoryRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamRepiratoryRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamDigestionRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamDigestionRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamKidneyUrologyRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamKidneyUrologyRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamNeurologicalRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamNeurologicalRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamMuscleBoneRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamMuscleBoneRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamENTRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamENTRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamStomatologyRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamStomatologyRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboEyeRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboEyeRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamOENDRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamOENDRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboMentalRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboMentalRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamDermatologyRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamDermatologyRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboSurgeryRankTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboSurgeryRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamNailRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamMucosaRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamHematopoieticRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamMotionRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamCardiovascularRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamLymphNodesRankTab3, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamCapillaryRankTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboPLSucKhoeTab1, data, controlEditorADO);
                ControlEditorLoader.Load(cboPLSucKhoeTab3, data, controlEditorADO);

                ControlEditorLoader.Load(cboExamObsteticRank, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamOccupationalTherapyRank, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamTraditionalRank, data, controlEditorADO);
                ControlEditorLoader.Load(cboExamNutrionRank, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
