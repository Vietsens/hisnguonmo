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
using HIS.UC.ServiceRoom.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utilities.Extensions;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        public void InitLoad(RoomExamServiceInitADO ado)
        {
            try
            {
                txtRoomCode.Text = "";
                cboRoom.EditValue = null;
                txtExamServiceCode.Text = "";
                cboExamService.EditValue = null;
                SetDataInit(ado);
                if (ado != null)
                {
                    this.VisibilityControl();
                    this.InitComboRoom();
                    this.InitComboExamService();
                    this.InitData();
                    this.Init();
                    this.SetCaptionByLanguageKey();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                if (this.sereServExam != null)
                {
                    if (!this.dicExecuteRoom.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    if (!this.dicRoomService.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    var room = this.dicExecuteRoom[this.sereServExam.TDL_EXECUTE_ROOM_ID];
                    var service = this.dicRoomService[this.sereServExam.TDL_EXECUTE_ROOM_ID].FirstOrDefault(o => o.SERVICE_ID == this.sereServExam.SERVICE_ID);
                    if (service != null)
                    {
                        this.serviceId = service.SERVICE_ID;
                    }
                    this.cboRoom.EditValue = room.ROOM_ID;

                    this.sereServExam = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void VisibilityControl()
        {
            try
            {
                if (this.isInit)
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExamService()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 60, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 280, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 340);
                ControlEditorLoader.Load(this.cboExamService, this.currentServiceRooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRoom()
        {
            try
            {
                if (this.roomExamServiceInitADO.HisExecuteRooms != null && this.roomExamServiceInitADO.HisExecuteRooms.Count > 0)
                {
                    InitComboRoom(this.roomExamServiceInitADO.HisExecuteRooms);
                }
                else if (this.roomExamServiceInitADO.LHisRoomCounters != null && this.roomExamServiceInitADO.LHisRoomCounters.Count > 0)
                {
                    InitComboRoom(this.roomExamServiceInitADO.LHisRoomCounters);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitRoomComboByConfig()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitRoomComboByConfig.1");
                this.cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboRoom.Properties);
                this.cboRoom.Properties.Tag = gridCheck;
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboRoom__SelectionChange);
                this.cboRoom.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboRoom_CustomDisplayText);
                GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboRoom.Properties.View);
                }

                //this.cboRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyDown);
                //this.cboRoom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyUp);
                Inventec.Common.Logging.LogSystem.Debug("InitRoomComboByConfig.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER> executeRooms)
        {
            try
            {
                this.cboRoom.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER>();
                this.roomExts = (from m in executeRooms select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();

                cboRoom.Properties.DataSource = this.roomExts;
                cboRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboRoom.Properties.ValueMember = "ROOM_ID";

                AddFieldColumnIntoComboRoom("EXECUTE_ROOM_CODE", "Mã", 100, 1, true);
                AddFieldColumnIntoComboRoom("EXECUTE_ROOM_NAME", "Tên", 400, 2, true);
                AddFieldColumnIntoComboRoom("TOTAL_TODAY_SERVICE_REQ", "Tổng", 100, 3, true);
                AddFieldColumnIntoComboRoom("TOTAL_NEW_SERVICE_REQ", "Chưa", 100, 4, true);
                AddFieldColumnIntoComboRoom("TOTAL_WAIT_TODAY_SERVICE_REQ", "CLS", 100, 5, true);
                AddFieldColumnIntoComboRoom("TOTAL_OPEN_SERVICE_REQ", "Đã", 100, 6, true);
                AddFieldColumnIntoComboRoom("MAX_REQ_BHYT_BY_DAY", "Tối đa", 100, 7, true);
                AddFieldColumnIntoComboRoom("RESPONSIBLE_USERNAME", "Tên bác sỹ", 350, 8, true);

                cboRoom.Properties.PopupFormWidth = 1250;
                cboRoom.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboRoom.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddFieldColumnIntoComboRoom(string FieldName, string Caption, int Width, int VisibleIndex, bool FixedWidth)
        {
            DevExpress.XtraGrid.Columns.GridColumn col2 = cboRoom.Properties.View.Columns.AddField(FieldName);
            col2.VisibleIndex = VisibleIndex;
            col2.Width = Width;
            col2.Caption = Caption;
            col2.OptionsColumn.FixedWidth = FixedWidth;
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1> executeRooms)
        {
            try
            {
                this.cboRoom.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1>();
                this.roomExts = (from m in executeRooms select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();

                cboRoom.Properties.DataSource = this.roomExts;
                cboRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboRoom.Properties.ValueMember = "ROOM_ID";

                AddFieldColumnIntoComboRoom("EXECUTE_ROOM_CODE", "", 100, 1, true);
                AddFieldColumnIntoComboRoom("EXECUTE_ROOM_NAME", "", 500, 2, true);
                AddFieldColumnIntoComboRoom("AMOUNT_COMBO", "", 100, 3, true);

                cboRoom.Properties.PopupFormWidth = 700;
                cboRoom.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboRoom.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomEditValueChanged()
        {
            try
            {
                if (this.currentRoomExts != null && this.currentRoomExts.Count > 0)
                {
                    this.cboRoom.Properties.Buttons[1].Visible = true;
                    var curRoomIds = this.currentRoomExts.Select(t => t.ROOM_ID).ToList();

                    GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null && this.currentRoomExts != null && this.currentRoomExts.Count > 0)
                    {
                        var roomselectedss = this.roomExts.Where(o => curRoomIds.Contains(o.ROOM_ID)).ToList();
                        gridCheckMark.SelectAll(roomselectedss);
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (RoomExtADO rv in currentRoomExts)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.EXECUTE_ROOM_NAME.ToString());
                        }
                    }

                    this.txtRoomCode.Text = sb.ToString();

                    //Khi người dùng đăng kí vào phòng khám là phòng cấp cứu, và đối tượng BN là BHYT thì PM tự động đổi đối tượng BN thành Đúng tuyến Cấp cứu. Giá trị hiển thị mặc định ở combobox và vẫn cho phép người dùng sửa (xử lý tại issue 8981)
                    //Bên ngoài module sử dụng uc này sẽ phải kiểm tra nếu là đối tượng BHYT thì mới gán giá trị cho action registerPatientWithRightRouteBHYT, ngược lại set null.
                    var isroomEmergence = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Any(o => curRoomIds != null && curRoomIds.Contains(o.ROOM_ID) && o.IS_EMERGENCY == 1);
                    if (isroomEmergence
                        && this.registerPatientWithRightRouteBHYT != null)
                    {
                        this.registerPatientWithRightRouteBHYT();
                    }

                    this.currentServiceRooms = hisServiceRooms.Where(o => curRoomIds.Contains(o.ROOM_ID)).ToList();
                }
                this.SetDataSourceCboExamService();
                if (this.serviceId.HasValue)
                {
                    this.cboExamService.EditValue = this.serviceId.Value;
                    this.serviceId = null;
                }
                else if (this.currentServiceRooms != null && this.currentServiceRooms.Count == 1)
                {
                    this.cboExamService.EditValue = this.currentServiceRooms.FirstOrDefault().SERVICE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
