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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.HisExamSchedule.ADO;
using HIS.Desktop.Plugins.HisExamSchedule.SetupWorkRoom;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.HisExamSchedule.HisExamSchedule
{
    public partial class frmHisExamSchedule : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int ActionType = -1;
        int positionHandle = -1;
        List<HIS_DEPARTMENT> lstHisDepartment = new List<HIS_DEPARTMENT>();
        List<HIS_EXAM_SCHEDULE> lstHisExamSchedule = new List<HIS_EXAM_SCHEDULE>();

        List<V_HIS_ROOM> lstHisRoom = new List<V_HIS_ROOM>();
        List<HisExamScheduleADO> lstHisExamScheduleADO = new List<HisExamScheduleADO>();
        List<HisExamScheduleADO> lstHisExamScheduleADODepartment = new List<HisExamScheduleADO>();
        //List<HisExamScheduleADO> lstHisExamScheduleADOArea = new List<HisExamScheduleADO>();
        //List<HisExamScheduleADO> lstHisExamScheduleADOAll = new List<HisExamScheduleADO>();
        
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<HIS_AREA> lstHisArea = new List<HIS_AREA>();

        List<V_HIS_ROOM> room = new List<V_HIS_ROOM>();

        #endregion

        #region Construct
        public frmHisExamSchedule(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
        #endregion

        private void frmHisExamSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                LoadlistRoom();

                LoadcboDepartment();

                LoadcboArea();

                //Load du lieu
                FillDataToGridControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void LoadlistRoom()
        {
            try
            {
                lstHisRoom = new List<V_HIS_ROOM>();
                lstHisRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_EXAM == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                lstHisExamSchedule = new List<HIS_EXAM_SCHEDULE>();
                lstHisExamScheduleADO = new List<HisExamScheduleADO>();
                lstHisExamScheduleADODepartment = new List<HisExamScheduleADO>();

                if (lstHisDepartment != null && lstHisDepartment.Count > 0)
                {
                    List<HIS_DEPARTMENT> DepartmentData = new List<HIS_DEPARTMENT>();
                    if (cboDepartment.EditValue != null)
                    {
                        DepartmentData.AddRange(lstHisDepartment.Where(o => o.ID == (long)cboDepartment.EditValue).ToList());
                    }
                    else
                    {
                        DepartmentData.AddRange(lstHisDepartment);
                    }

                    foreach (var item in DepartmentData)
                    {
                        List<V_HIS_ROOM> room = new List<V_HIS_ROOM>();

                        room = lstHisRoom.Where(o => o.DEPARTMENT_ID == item.ID).ToList();

                        foreach (var item2 in room)
                        {
                            HisExamScheduleADO ExamScheduleADO = new HisExamScheduleADO();
                            ExamScheduleADO.DEPARTMENT_ID = item.ID;

                            ExamScheduleADO.ROOM = item2.ROOM_CODE + " \n" + item2.ROOM_NAME + "\n" + item2.DEPARTMENT_NAME + "\n";

                            ExamScheduleADO.ROOM_ID = item2.ID;

                            lstHisExamSchedule = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXAM_SCHEDULE>().Where(o => o.ROOM_ID == item2.ID).ToList();

                            if (lstHisExamSchedule != null && lstHisExamSchedule.Count > 0)
                            {
                                lstHisExamSchedule = lstHisExamSchedule.OrderBy(o => o.DAY_OF_WEEK).ThenBy(o => o.TIME_FROM).ToList();

                                int T2 = 1, T3 = 1, T4 = 1, T5 = 1, T6 = 1, T7 = 1, CN = 1;

                                foreach (var itemRoom in lstHisExamSchedule)
                                {
                                    var TimeFrom = !String.IsNullOrWhiteSpace(itemRoom.TIME_FROM) && itemRoom.TIME_FROM.Length == 4 ? String.Format("{0}:{1}", itemRoom.TIME_FROM.Substring(0, 2), itemRoom.TIME_FROM.Substring(2, 2)) : "";

                                    var TimeTo = !String.IsNullOrWhiteSpace(itemRoom.TIME_TO) && itemRoom.TIME_TO.Length == 4 ? String.Format("{0}:{1}", itemRoom.TIME_TO.Substring(0, 2), itemRoom.TIME_TO.Substring(2, 2)) : "";

                                    if (itemRoom.DAY_OF_WEEK == 1)
                                    {
                                        ExamScheduleADO.CHUNHAT += CN.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        CN++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 2)
                                    {
                                        ExamScheduleADO.THU2 += T2.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T2++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 3)
                                    {
                                        ExamScheduleADO.THU3 += T3.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T3++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 4)
                                    {
                                        ExamScheduleADO.THU4 += T4.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T4++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 5)
                                    {
                                        ExamScheduleADO.THU5 += T5.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T5++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 6)
                                    {
                                        ExamScheduleADO.THU6 += T6.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T6++;
                                    }
                                    if (itemRoom.DAY_OF_WEEK == 7)
                                    {
                                        ExamScheduleADO.THU7 += T7.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
                                        T7++;
                                    }
                                }
                            }
                            else
                            {
                                ExamScheduleADO.CHUNHAT = null;
                                ExamScheduleADO.THU2 = null;
                                ExamScheduleADO.THU3 = null;
                                ExamScheduleADO.THU4 = null;
                                ExamScheduleADO.THU5 = null;
                                ExamScheduleADO.THU6 = null;
                                ExamScheduleADO.THU7 = null;
                            }

                            lstHisExamScheduleADODepartment.Add(ExamScheduleADO);
                        }
                    }
                }

                if (this.cboArea.EditValue != null)
                {
                    var roomArea = lstHisRoom.Where(o => o.AREA_ID == (long)this.cboArea.EditValue).ToList();

                    if (roomArea != null && roomArea.Count > 0)
                    {
                        foreach (var item in roomArea)
                        {
                            var check = lstHisExamScheduleADODepartment.Where(o => o.ROOM_ID == item.ID).ToList();
                            if (check != null && check.Count > 0)
                            {
                                lstHisExamScheduleADO.AddRange(check);
                            }
                        }
                    }
                }
                else 
                {
                    lstHisExamScheduleADO = lstHisExamScheduleADODepartment;
                }

                gridView1.GridControl.DataSource = lstHisExamScheduleADO;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboArea()
        {
            try
            {
                lstHisArea = new List<HIS_AREA>();

                CommonParam param = new CommonParam();
                HisAreaFilter filer = new HisAreaFilter();
                filer.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.lstHisArea = new BackendAdapter(param).Get<List<HIS_AREA>>("api/HisArea/Get", ApiConsumers.MosConsumer, filer, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("AREA_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("AREA_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("AREA_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboArea, lstHisArea, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboDepartment()
        {
            try
            {
                lstHisDepartment = new List<HIS_DEPARTMENT>();
                lstHisDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDepartment, lstHisDepartment, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
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
                if (cboDepartment.EditValue == null)
                {
                    cboDepartment.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboDepartment.Properties.Buttons[1].Visible = true;
                }
                FillDataToGridControl();
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
                    HisExamScheduleADO pData = (HisExamScheduleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ButtonEdit")
                    {
                        e.RepositoryItem = repositoryItemButtonEdit;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HisExamScheduleADO)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    frmSetupWorkRoom SetupWorkRoom = new frmSetupWorkRoom(rowData.ROOM_ID);
                    SetupWorkRoom.ShowDialog();
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboArea_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboArea.EditValue == null)
                {
                    cboArea.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboArea.Properties.Buttons[1].Visible = true;
                }
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboArea_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboArea.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void FillDataToGridControl_New()
        //{
        //    try
        //    {
        //        lstHisExamSchedule = new List<HIS_EXAM_SCHEDULE>();
        //        lstHisExamScheduleADODepartment = new List<HisExamScheduleADO>();
        //        lstHisExamScheduleADOArea = new List<HisExamScheduleADO>();
        //        lstHisExamScheduleADOAll = new List<HisExamScheduleADO>();

        //        room = new List<V_HIS_ROOM>();

        //        if (lstHisDepartment != null && lstHisDepartment.Count > 0)
        //        {
        //            List<HIS_DEPARTMENT> DepartmentData = new List<HIS_DEPARTMENT>();
        //            if (cboDepartment.EditValue != null)
        //            {
        //                DepartmentData.AddRange(lstHisDepartment.Where(o => o.ID == (long)cboDepartment.EditValue).ToList());
        //            }
        //            else
        //            {
        //                DepartmentData.AddRange(lstHisDepartment);
        //            }

        //            foreach (var item in DepartmentData)
        //            {
        //                room = lstHisRoom.Where(o => o.DEPARTMENT_ID == item.ID).ToList();
                    
        //                foreach (var item2 in room)
        //                {
        //                    HisExamScheduleADO ExamScheduleADO = new HisExamScheduleADO();
        //                    ExamScheduleADO.DEPARTMENT_ID = item.ID;

        //                    ExamScheduleADO.ROOM = item2.ROOM_CODE + " \n" + item2.ROOM_NAME + "\n" + item2.DEPARTMENT_NAME + "\n";

        //                    ExamScheduleADO.ROOM_ID = item2.ID;


        //                    lstHisExamSchedule = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXAM_SCHEDULE>().Where(o => o.ROOM_ID == item2.ID).ToList();

        //                    if (lstHisExamSchedule != null && lstHisExamSchedule.Count > 0)
        //                    {
        //                        lstHisExamSchedule = lstHisExamSchedule.OrderBy(o => o.DAY_OF_WEEK).ThenBy(o => o.TIME_FROM).ToList();

        //                        int T2 = 1, T3 = 1, T4 = 1, T5 = 1, T6 = 1, T7 = 1, CN = 1;

        //                        foreach (var itemRoom in lstHisExamSchedule)
        //                        {
        //                            var TimeFrom = !String.IsNullOrWhiteSpace(itemRoom.TIME_FROM) && itemRoom.TIME_FROM.Length == 4 ? String.Format("{0}:{1}", itemRoom.TIME_FROM.Substring(0, 2), itemRoom.TIME_FROM.Substring(2, 2)) : "";

        //                            var TimeTo = !String.IsNullOrWhiteSpace(itemRoom.TIME_TO) && itemRoom.TIME_TO.Length == 4 ? String.Format("{0}:{1}", itemRoom.TIME_TO.Substring(0, 2), itemRoom.TIME_TO.Substring(2, 2)) : "";

        //                            if (itemRoom.DAY_OF_WEEK == 1)
        //                            {
        //                                ExamScheduleADO.CHUNHAT += CN.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                CN++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 2)
        //                            {
        //                                ExamScheduleADO.THU2 += T2.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T2++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 3)
        //                            {
        //                                ExamScheduleADO.THU3 += T3.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T3++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 4)
        //                            {
        //                                ExamScheduleADO.THU4 += T4.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T4++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 5)
        //                            {
        //                                ExamScheduleADO.THU5 += T5.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T5++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 6)
        //                            {
        //                                ExamScheduleADO.THU6 += T6.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T6++;
        //                            }
        //                            if (itemRoom.DAY_OF_WEEK == 7)
        //                            {
        //                                ExamScheduleADO.THU7 += T7.ToString() + ": " + itemRoom.USERNAME + " (" + TimeFrom + " - " + TimeTo + ")\n";
        //                                T7++;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ExamScheduleADO.CHUNHAT = null;
        //                        ExamScheduleADO.THU2 = null;
        //                        ExamScheduleADO.THU3 = null;
        //                        ExamScheduleADO.THU4 = null;
        //                        ExamScheduleADO.THU5 = null;
        //                        ExamScheduleADO.THU6 = null;
        //                        ExamScheduleADO.THU7 = null;
        //                    }

        //                    lstHisExamScheduleADODepartment.Add(ExamScheduleADO);
        //                }
        //            }
        //        }




        //        if (cboDepartment.EditValue != null && cboArea.EditValue == null)
        //        {
        //            gridView1.GridControl.DataSource = lstHisExamScheduleADODepartment;
        //        }
        //        else if ((cboDepartment.EditValue == null && cboArea.EditValue != null))
        //        {
        //            gridView1.GridControl.DataSource = lstHisExamScheduleADOArea;
        //        }
        //        else if (cboDepartment.EditValue != null && cboArea.EditValue != null)
        //        {
        //            gridView1.GridControl.DataSource = lstHisExamScheduleADOAll;
        //        }
        //        else 
        //        {
        //            gridView1.GridControl.DataSource = lstHisExamScheduleADODepartment;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


    }
}
