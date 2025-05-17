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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System.IO;
using System.Net.NetworkInformation;
using HIS.UC.ServiceRoom.OperatingStatus;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        internal string numOderSelected;
        Dictionary<long, string> numberNames = new Dictionary<long, string>();
        private string USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

        public void InitLoad(RoomExamServiceInitADO ado)
        {
            try
            {
                txtRoomCode.Text = "";

                this.beditRoom.Text = "";
                beditRoom.Properties.Buttons[1].Visible = false;
                if (this.roomExts != null)
                    this.roomExts.ForEach(o => o.IsChecked = false);
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
                    var rData = this.roomExts != null ? this.roomExts.Where(o => o.ROOM_ID == room.ROOM_ID).FirstOrDefault() : null;
                    if (rData != null)
                    {
                        rData.IsChecked = true;
                        if (this.PatientSDO != null && this.PatientSDO.NumOrderIssueId.HasValue)
                        {
                            rData.NumOrderBlock = string.Format("{0} - {1}", GenerateHour(this.PatientSDO.NextExamFromTime), GenerateHour(this.PatientSDO.NextExamToTime));
                        }

                        RoomEditValueChanged();
                    }

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
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 320, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 420);
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

        private void SetCheckAllColumn(bool state)
        {
            try
            {
                DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerRoom.Columns["IsChecked"];
                gridColumnCheck.ImageAlignment = StringAlignment.Center;
                gridColumnCheck.Image = (state ? this.imageCollection1.Images[1] : this.imageCollection1.Images[0]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER_2> executeRooms)
        {
            try
            {
                InitComboRoom(executeRooms, false);
            }
            catch (Exception ex)
            {
                gridViewContainerRoom.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER_2> executeRooms, bool isSyncData)
        {
            try
            {
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER_2>();
                if (isSyncData && this.roomExts != null && this.roomExts.Count > 0)
                    this.roomExts = (from m in executeRooms select new RoomExtADO(m, this.hisRooms, this.roomExts)).ToList();
                else
                    this.roomExts = (from m in executeRooms select new RoomExtADO(m, this.hisRooms)).ToList();
                this.roomExts = OrderRoom(this.roomExts);
                popupHeight = (this.roomExts != null && this.roomExts.Count > 15) ? 400 : 200;
                gridViewContainerRoom.BeginUpdate();
                gridViewContainerRoom.Columns.Clear();
                popupControlContainerRoom.Width = 1100;
                popupControlContainerRoom.Height = popupHeight;
                int columnIndex = 1;
                AddFieldColumnIntoComboRoomExt("IsChecked", " ","", 30, columnIndex++, true, null, true);
                AddFieldColumnIntoComboRoomExt("EXECUTE_ROOM_CODE", col1,"", 90, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("EXECUTE_ROOM_NAME", col2,"", 240, columnIndex++, true);       
                AddFieldColumnIntoComboRoomExt("TOTAL_TODAY_SERVICE_REQ", col3,tol1, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("TOTAL_MORNING_SERE", col13, tol8, 80, -1, false);/// mới
                AddFieldColumnIntoComboRoomExt("TOTAL_AFTERNOON_SERE", col14, tol9, 80, -1, false);
                AddFieldColumnIntoComboRoomExt("TOTAL_TODAY_KNVP_SERE", col15, tol10, 80, -1, false);
                AddFieldColumnIntoComboRoomExt("TOTAL_MORNING_KNVP_SERE", col16, tol11, 80, -1, false);
                AddFieldColumnIntoComboRoomExt("TOTAL_AFTERNOON_KNVP_SERE", col17, tol12, 80, -1, false);///
                AddFieldColumnIntoComboRoomExt("TOTAL_NEW_SERVICE_REQ", col4,tol2, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("TOTAL_END_SERVICE_REQ", col5, tol3, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("TOTAL_WAIT_TODAY_SERVICE_REQ", col6,tol4, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("TOTAL_OPEN_SERVICE_REQ", col7,tol5, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("MAX_REQUEST_BY_DAY", col8,tol6, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("MAX_REQ_BHYT_BY_DAY", col9,tol7, 80, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("RESPONSIBLE_USERNAME_DISPLAY", col10,"", 300, columnIndex++, true, DevExpress.Data.UnboundColumnType.Object);
                AddFieldColumnIntoComboRoomExt("NumOrderBlock", col11,"", 150, columnIndex++, true, null, true);
                AddFieldColumnIntoComboRoomExt("IS_WARN",col12,"", 80, -1, false);
                

                gridViewContainerRoom.GridControl.DataSource = this.roomExts;

                RestoreLayoutForCurrentUser(gridViewContainerRoom);

                gridViewContainerRoom.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerRoom.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveLayoutForCurrentUser(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            if (view == null || string.IsNullOrEmpty(USER))
                return;
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RegisterV2").FirstOrDefault();

            var allowedModules = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MODULELINKS.Split(',');
            if (!string.IsNullOrWhiteSpace(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MODULELINKS) && moduleData != null && !allowedModules.Contains(moduleData.ModuleLink))
                return;

            var ms = new MemoryStream();
            view.SaveLayoutToStream(ms);

            OperatingStatus.OperatingStatus.Status.LayoutPerUser[USER] = ms;
        }

        public void RestoreLayoutForCurrentUser(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            if (view == null || string.IsNullOrEmpty(USER))
                return;

            if (OperatingStatus.OperatingStatus.Status.LayoutPerUser.TryGetValue(USER, out MemoryStream ms))
            {
                ms.Position = 0;
                view.RestoreLayoutFromStream(ms);
            }
        }


        DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit GenerateRepositoryItemCheckEdit()
        {
            DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            return repositoryItemCheckEdit1;
        }

        private void AddFieldColumnIntoComboRoomExt(string FieldName, string Caption,string toolTip ,int Width, int VisibleIndex, bool FixedWidth, DevExpress.Data.UnboundColumnType? UnboundType = null, bool allowEdit = false)
        {
            DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
            col2.FieldName = FieldName;
            col2.Caption = Caption;
            col2.ToolTip = toolTip;
            col2.Width = Width;
            col2.VisibleIndex = VisibleIndex;
            col2.OptionsColumn.FixedWidth = FixedWidth;
            if (UnboundType != null)
                col2.UnboundType = UnboundType.Value;
            col2.OptionsColumn.AllowEdit = allowEdit;
            if (FieldName == "IsChecked")
            {
                col2.ColumnEdit = GenerateRepositoryItemCheckEdit();
                col2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col2.OptionsFilter.AllowFilter = false;
                col2.OptionsFilter.AllowAutoFilter = false;
                col2.ImageAlignment = StringAlignment.Center;
                col2.Image = imageCollection1.Images[0];
                col2.OptionsColumn.AllowEdit = false;
            }
            else if (FieldName == "NumOrderBlock")
            {
                col2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col2.OptionsFilter.AllowFilter = false;
                col2.OptionsFilter.AllowAutoFilter = false;
            }

            col2.OptionsColumn.ShowInCustomizationForm = true;

            gridViewContainerRoom.Columns.Add(col2);
        }

        private void gridViewContainerRoom_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            //if (e.Column != null && e.Column.FieldName == "Chur")
            //{
            //    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            //    if (view != null)
            //    {
            //        view.OptionsCustomization.AllowColumnMoving = true;
            //        view.OptionsCustomization.AllowQuickHideColumns = true;
            //        view.OptionsCustomization.CustomizationFormSearchBoxVisible = true;

            //        // Gọi Column Chooser
            //        view.ShowCustomization();
            //    }
            //}
        }


        private List<RoomExtADO> OrderRoom(List<RoomExtADO> roomList)
        {
            List<RoomExtADO> result = new List<RoomExtADO>();
            try
            {
                if (roomList == null || roomList.Count == 0)
                    return null;

                var roomHastotalDay = roomList.Where(o => o.TOTAL_TODAY_SERVICE_REQ > 0).OrderBy(p => p.EXECUTE_ROOM_NAME).ToList();
                var roomHasNottotalDay = roomList.Where(o => !o.TOTAL_TODAY_SERVICE_REQ.HasValue || o.TOTAL_TODAY_SERVICE_REQ.Value == 0).OrderBy(p => p.EXECUTE_ROOM_NAME).ToList();
                if (roomHastotalDay != null && roomHastotalDay.Count > 0)
                {
                    result.AddRange(roomHastotalDay);
                }
                if (roomHasNottotalDay != null && roomHasNottotalDay.Count > 0)
                {
                    result.AddRange(roomHasNottotalDay);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;

        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1> executeRooms)
        {
            try
            {
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1>();
                this.roomExts = (from m in executeRooms select new RoomExtADO(m, this.hisRooms)).ToList();
                this.roomExts = OrderRoom(this.roomExts);

                popupHeight = (this.roomExts != null && this.roomExts.Count > 15) ? 400 : 200;
                gridViewContainerRoom.BeginUpdate();
                gridViewContainerRoom.Columns.Clear();
                popupControlContainerRoom.Width = 540;
                popupControlContainerRoom.Height = popupHeight;

                int columnIndex = 1;
                AddFieldColumnIntoComboRoomExt("IsChecked", " ","", 30, columnIndex++, true, null, true);
                AddFieldColumnIntoComboRoomExt("EXECUTE_ROOM_CODE", "","", 100, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("EXECUTE_ROOM_NAME", "","", 320, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("AMOUNT_COMBO", "","", 90, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("IS_WARN", col12,"", 80, -1, false);

                gridViewContainerRoom.GridControl.DataSource = this.roomExts;
                gridViewContainerRoom.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerRoom.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomEditValueChanged()
        {
            try
            {
                ProcessDisplayRoomWithData();
                var currentRoomExts = this.roomExts.Where(o => o.IsChecked).ToList();
                if (currentRoomExts != null && currentRoomExts.Count > 0)
                {                    
                    this.beditRoom.Properties.Buttons[1].Visible = true;

                    //Khi người dùng đăng kí vào phòng khám là phòng cấp cứu, và đối tượng BN là BHYT thì PM tự động đổi đối tượng BN thành Đúng tuyến Cấp cứu. Giá trị hiển thị mặc định ở combobox và vẫn cho phép người dùng sửa (xử lý tại issue 8981)
                    //Bên ngoài module sử dụng uc này sẽ phải kiểm tra nếu là đối tượng BHYT thì mới gán giá trị cho action registerPatientWithRightRouteBHYT, ngược lại set null.
                    var curRoomIds = currentRoomExts.Select(t => t.ROOM_ID).ToList();
                    var isroomEmergence = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Any(o => curRoomIds != null && curRoomIds.Contains(o.ROOM_ID) && o.IS_EMERGENCY == 1);
                    if (isroomEmergence
                        && this.registerPatientWithRightRouteBHYT != null)
                    {
                        this.registerPatientWithRightRouteBHYT();
                    }
                }
                this.SetDataSourceCboExamService();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessShowpopupControlContainerRoom(bool focusPopupContainer = false)
        {
            try
            {
                int heightPlus = 0;
                Rectangle bounds = GetClientRectangle(this.Parent, ref heightPlus);
                Rectangle bounds1 = GetAllClientRectangle(this.Parent, ref heightPlus);
                if (bounds == null)
                {
                    bounds = beditRoom.Bounds;
                }
                //xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
                if (bounds1.Height <= 768)
                {
                    if (popupHeight == 400)
                    {
                        heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
                    }
                    else
                        heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
                }
                else
                {
                    if (popupHeight == 400)
                    {
                        heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
                    }
                    else
                        heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
                }

                Rectangle buttonBounds = new Rectangle(beditRoom.Bounds.X + 10, bounds.Y + heightPlus, bounds.Width, bounds.Height);
                Inventec.Common.Logging.LogSystem.Debug("ProcessShowpopupControlContainerRoom____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => buttonBounds), buttonBounds)
                                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightPlus), heightPlus)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => buttonBounds.X), buttonBounds.X)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => buttonBounds.Bottom), buttonBounds.Bottom));

                int xPosition = 0, yPosition = 0;

                //1382, 744   ==> X:155______Bottom:28_
                //1936, 1056  ==> X:159______Bottom:161
                //1552, 840,  ==> X:157______Bottom:-38
                //1296, 1000  ==> X:155______Bottom:109
                //1696, 1026" ==> X:159______Bottom:133
                //1616, 876   ==> X:159______Bottom:-5
                //1456, 876   ==> X:155______Bottom:-5
                //1296, 744   ==> X:155______Bottom:28

                if (bounds1.Width > 1900 && bounds1.Height > 1000)
                {
                    if (buttonBounds.Bottom > 400)
                    {
                        xPosition = 159;
                        yPosition = 445;
                    }
                    else
                    {
                        xPosition = 159;
                        yPosition = 156;
                    }
                }
                else if (bounds1.Width > 1600 && bounds1.Height > 1000)
                {
                    if (buttonBounds.Bottom < 150)
                    {
                        xPosition = 159;
                        yPosition = 144;
                    }
                    else
                    {
                        xPosition = 159;
                        yPosition = 415;
                    }
                }
                else if (bounds1.Width > 1600 && bounds1.Height > 800)
                {
                    if (buttonBounds.Bottom > 300)
                    {
                        xPosition = 159;
                        yPosition = 300;
                    }
                    else
                    {
                        xPosition = 159;
                        yPosition = 150;
                    }
                }
                else if (bounds1.Width > 1500 && bounds1.Height > 800)
                {
                    if (buttonBounds.Bottom > 300)
                    {
                        xPosition = 157;
                        yPosition = 270;
                    }
                    else
                    {
                        xPosition = 157;
                        yPosition = 70;
                    }
                }
                else if (bounds1.Width > 1400 && bounds1.Height > 1000)
                {
                    if (buttonBounds.Bottom > 300)
                    {
                        xPosition = 157;
                        yPosition = 419;
                    }
                    else
                    {
                        xPosition = 157;
                        yPosition = 155;
                    }
                }
                else if (bounds1.Width > 1400 && bounds1.Height > 800)
                {
                    if (buttonBounds.Bottom > 300)
                    {
                        xPosition = 157;
                        yPosition = 270;
                    }
                    else
                    {
                        xPosition = 157;
                        yPosition = 120;
                    }
                }
                else if (bounds1.Width > 1300 && bounds1.Height > 700)
                {
                    if (buttonBounds.Bottom > 200)
                    {
                        xPosition = 155;
                        yPosition = 195;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 20;
                    }
                }
                else if (bounds1.Width > 1250 && bounds1.Height > 950)
                {
                    if (buttonBounds.Bottom > 300)
                    {
                        xPosition = 155;
                        yPosition = 300;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 100;
                    }
                }
                else if (bounds1.Width > 1250 && bounds1.Height > 900)
                {
                    if (buttonBounds.Bottom > 400)
                    {
                        xPosition = 155;
                        yPosition = 300;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 140;
                    }
                }
                else if (bounds1.Width > 1250 && bounds1.Height > 750)
                {
                    if (buttonBounds.Bottom > 200)
                    {
                        xPosition = 155;
                        yPosition = 150;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 10;
                    }
                }
                else if (bounds1.Width > 1250 && bounds1.Height > 700)
                {
                    if (buttonBounds.Bottom > 200)
                    {
                        xPosition = 155;
                        yPosition = 100;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 10;
                    }
                }
                else if (bounds1.Width > 1000 && bounds1.Height > 700)
                {
                    if (buttonBounds.Bottom > 200)
                    {
                        xPosition = 155;
                        yPosition = 180;
                    }
                    else
                    {
                        xPosition = 155;
                        yPosition = 23;
                    }
                }
                else
                {
                    xPosition = buttonBounds.X;
                    yPosition = buttonBounds.Bottom;//TODO
                }

                CheckData();

                popupControlContainerRoom.ShowPopup(new Point(xPosition, yPosition));

                if (focusPopupContainer)
                {
                    gridViewContainerRoom.Focus();
                    gridViewContainerRoom.FocusedRowHandle = 0;
                }
                else
                {
                    this.beditRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckData()
        {
            try
            {
                if (this.roomExts != null && this.roomExts.Count > 0)
                {
                    var hasNumOrder = this.roomExts.Where(o => !String.IsNullOrWhiteSpace(o.NumOrderBlock)).ToList();
                    if (hasNumOrder != null && hasNumOrder.Count > 0 && this.GetIntructionTime != null)
                    {
                        long date = Inventec.Common.TypeConvert.Parse.ToInt64(this.GetIntructionTime().ToString("yyyyMMdd") + "000000");
                        if (dicNumOrderBlock.ContainsKey(hasNumOrder.First().ROOM_ID) && dicNumOrderBlock[hasNumOrder.First().ROOM_ID].Date != date)
                        {
                            hasNumOrder.ForEach(o => o.NumOrderBlock = null);
                            dicNumOrderBlock = new Dictionary<long, Desktop.ADO.ResultChooseNumOrderBlockADO>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessDisplayRoomWithData()
        {
            StringBuilder sbText = new StringBuilder();
            StringBuilder sbCode = new StringBuilder();
            var currentRoomExts = this.roomExts.Where(o => o.IsChecked).ToList();
            if (currentRoomExts != null && currentRoomExts.Count > 0)
            {
                foreach (RoomExtADO rv in currentRoomExts)
                {
                    if (rv != null)
                    {
                        if (sbText.ToString().Length > 0) { sbText.Append(", "); }
                        sbText.Append(rv.EXECUTE_ROOM_NAME);
                        if (sbCode.ToString().Length > 0) { sbCode.Append(", "); }
                        sbCode.Append(rv.EXECUTE_ROOM_CODE);
                    }
                }
            }
            this.beditRoom.Properties.Buttons[1].Visible = (currentRoomExts != null && currentRoomExts.Count > 0);
            isShowContainerMediMatyForChoose = true;
            this.beditRoom.Text = sbText.ToString();
            this.txtRoomCode.Text = sbCode.ToString();
        }

        private void CallModuleNumOrderBlockChooser(RoomExtADO data)
        {
            try
            {
                if (data != null && data.IsBlockNumOrder == 1)
                {
                    List<object> _listObj = new List<object>();
                    HIS.Desktop.ADO.NumOrderBlockChooserADO ado = new Desktop.ADO.NumOrderBlockChooserADO();

                    if (GetIntructionTime != null)
                    {
                        ado.DefaultDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(GetIntructionTime());
                    }
                    else
                    {
                        ado.DefaultDate = Inventec.Common.DateTime.Get.Now() ?? 0;
                    }

                    ado.DefaultRoomId = data.ROOM_ID;
                    ado.DelegateChooseData = ChooseNumOrderBlock;
                    ado.DisableDate = true;
                    ado.DisableRoom = true;

                    string result = null;
                    if (numberNames.TryGetValue(data.ROOM_ID, out result))
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            ado.SelectedTime = Convert.ToInt64(result) / 100;
                        }
                    }

                    ado.ListRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => this.roomExts.Exists(m => m.ROOM_ID == o.ID)).ToList();
                    _listObj.Add(ado);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisNumOrderBlockChooser", 0, 0, _listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseNumOrderBlock(Desktop.ADO.ResultChooseNumOrderBlockADO resultChooseNumOrderBlock)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("1___________________________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultChooseNumOrderBlock), resultChooseNumOrderBlock));
                if (resultChooseNumOrderBlock != null)
                {
                    this.dicNumOrderBlock[resultChooseNumOrderBlock.RoomId] = resultChooseNumOrderBlock;
                    var thisRow = this.roomExts.FirstOrDefault(o => o.ROOM_ID == resultChooseNumOrderBlock.RoomId);
                    if (thisRow != null && resultChooseNumOrderBlock.NumOrderBlock != null)
                    {
                        thisRow.NumOrderBlock = string.Format("{0} - {1}", GenerateHour(resultChooseNumOrderBlock.NumOrderBlock.FROM_TIME), GenerateHour(resultChooseNumOrderBlock.NumOrderBlock.TO_TIME));
                        this.numOderSelected = resultChooseNumOrderBlock.NumOrderBlock.FROM_TIME;
                        if (numberNames.ContainsKey(resultChooseNumOrderBlock.RoomId))
                        {
                            this.numberNames = this.numberNames.ToDictionary(nbm => resultChooseNumOrderBlock.RoomId, nbm => resultChooseNumOrderBlock.NumOrderBlock.FROM_TIME);
                        }
                        else
                        {
                            this.numberNames.Add(resultChooseNumOrderBlock.RoomId, resultChooseNumOrderBlock.NumOrderBlock.FROM_TIME);
                        }
                        thisRow.IsChecked = true;
                    }

                    this.gridControlContainerRoom.RefreshDataSource();
                    ProcessDisplayRoomWithData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
