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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentAppointment
{
    // PTTK_3145: Sua lich hen dieu tri va tai kham - popup menu + cot hien thi bo sung
    public partial class frmTreatmentAppointment
    {
        #region Declare
        internal const string MODULE_LINK__APPOINTMENT_INFO = "HIS.Desktop.Plugins.AppointmentInfo";
        internal const string PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010 = "Mps000010";

        DevExpress.XtraBars.PopupMenu popupMenuGrid;
        DevExpress.XtraBars.BarButtonItem bbtnEditAppointment;
        DevExpress.XtraBars.BarButtonItem bbtnPrintAppointment;
        Dictionary<long, string> appointmentRoomNameById;
        #endregion

        #region Init
        private void InitPopupMenuEditAppointment()
        {
            try
            {
                this.bbtnEditAppointment = new DevExpress.XtraBars.BarButtonItem(this.barManager1,
                    GetLangText("frmTreatmentAppointment.bbtnEditAppointment.Caption"));
                this.bbtnEditAppointment.ItemClick += bbtnEditAppointment_ItemClick;

                this.bbtnPrintAppointment = new DevExpress.XtraBars.BarButtonItem(this.barManager1,
                    GetLangText("frmTreatmentAppointment.bbtnPrintAppointment.Caption"));
                this.bbtnPrintAppointment.ItemClick += bbtnPrintAppointment_ItemClick;

                this.popupMenuGrid = new DevExpress.XtraBars.PopupMenu(this.barManager1);
                this.popupMenuGrid.AddItems(new DevExpress.XtraBars.BarItem[] { this.bbtnEditAppointment, this.bbtnPrintAppointment });

                this.gridViewTreatmentAppointment.PopupMenuShowing += gridViewTreatmentAppointment_PopupMenuShowing;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridColumnsExt()
        {
            try
            {
                // 4 cot moi deu dat cuoi grid, theo thu tu them vao
                // (VisibleIndex vuot qua so cot hien co -> DevExpress tu day ve cuoi)
                AddGridColumnExt("APPOINTMENT_EXAM_ROOM_NAMES",
                    GetLangText("frmTreatmentAppointment.gridColumnAppointmentExamRoom.Caption"), 180, 999);
                AddGridColumnExt("END_USERNAME",
                    GetLangText("frmTreatmentAppointment.gridColumnAppointmentCreator.Caption"), 120, 1000);
                AddGridColumnExt("MODIFY_TIME_STR",
                    GetLangText("frmTreatmentAppointment.gridColumnModifyTime.Caption"), 130, 1001);
                AddGridColumnExt("MODIFIER",
                    GetLangText("frmTreatmentAppointment.gridColumnModifier.Caption"), 100, 1002);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddGridColumnExt(string fieldName, string caption, int width, int visibleIndex)
        {
            try
            {
                var col = this.gridViewTreatmentAppointment.Columns.AddField(fieldName);
                col.Caption = caption;
                col.Width = width;
                col.OptionsColumn.AllowEdit = false;
                col.VisibleIndex = visibleIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Goi lai SAU moi lan bind DataSource - dam bao 4 cot moi luon nam cuoi grid
        internal void MoveExtColumnsToEnd()
        {
            try
            {
                // ICD_NAME (Chan doan chinh) dua ve cuoi TRUOC -> ket qua: ICD dung truoc 4 cot moi
                string[] extFields = new string[] { "ICD_NAME", "APPOINTMENT_EXAM_ROOM_NAMES", "END_USERNAME", "MODIFY_TIME_STR", "MODIFIER" };
                foreach (string field in extFields)
                {
                    var col = this.gridViewTreatmentAppointment.Columns.ColumnByFieldName(field);
                    if (col != null && this.gridViewTreatmentAppointment.VisibleColumns.Count > 0)
                    {
                        // Move-to-last theo thu tu duyet -> giu dung thu tu 4 cot
                        col.VisibleIndex = this.gridViewTreatmentAppointment.VisibleColumns.Count - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLangText(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
        #endregion

        #region Display fields
        private void EnsureAppointmentRoomNames()
        {
            try
            {
                if (this.appointmentRoomNameById != null) return;
                this.appointmentRoomNameById = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                    .GroupBy(o => o.ROOM_ID)
                    .ToDictionary(g => g.Key, g => g.First().EXECUTE_ROOM_NAME);
            }
            catch (Exception ex)
            {
                this.appointmentRoomNameById = new Dictionary<long, string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Pre-compute display fields per ADO row (khong tinh trong CustomUnboundColumnData)
        private void ResolveAppointmentDisplayFields(ADO.TreatmentAppointmentADO ado)
        {
            try
            {
                if (ado == null) return;
                ado.MODIFY_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ado.MODIFY_TIME ?? 0);
                if (!String.IsNullOrEmpty(ado.APPOINTMENT_EXAM_ROOM_IDS) && this.appointmentRoomNameById != null)
                {
                    List<string> roomNames = new List<string>();
                    foreach (var idStr in ado.APPOINTMENT_EXAM_ROOM_IDS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        long roomId;
                        string roomName;
                        if (long.TryParse(idStr.Trim(), out roomId) && this.appointmentRoomNameById.TryGetValue(roomId, out roomName))
                        {
                            roomNames.Add(roomName);
                        }
                    }
                    ado.APPOINTMENT_EXAM_ROOM_NAMES = String.Join("; ", roomNames);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Popup menu handlers
        private void gridViewTreatmentAppointment_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row) return;
                var row = this.gridViewTreatmentAppointment.GetFocusedRow() as ADO.TreatmentAppointmentADO;
                bool hasAppointment = (row != null && row.APPOINTMENT_TIME.HasValue && row.APPOINTMENT_TIME.Value > 0);
                this.bbtnEditAppointment.Enabled = hasAppointment;
                this.bbtnPrintAppointment.Enabled = hasAppointment;
                this.popupMenuGrid.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEditAppointment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var row = this.gridViewTreatmentAppointment.GetFocusedRow() as ADO.TreatmentAppointmentADO;
                if (row == null || !row.APPOINTMENT_TIME.HasValue) return;

                // PTTK_3145 3.2: chi bac si da tao lich hen (nguoi ket thuc dieu tri) duoc sua
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrEmpty(row.END_LOGINNAME)
                    || !row.END_LOGINNAME.Equals(loginName, StringComparison.OrdinalIgnoreCase))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessageLang.ChiBacSiTaoLichHenDuocSua,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.ID = row.ID;
                var treatmentViews = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>(
                    HisRequestUriStore.MOSHIS_HIS_TREATMENT_GETVIEW4, ApiConsumers.MosConsumer, filter, param);
                var treatmentView = (treatmentViews != null ? treatmentViews.FirstOrDefault() : null);
                WaitingManager.Hide();
                if (treatmentView == null)
                {
                    MessageManager.Show(this, param, false);
                    SessionManager.ProcessTokenLost(param);
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentView);
                listArgs.Add((RefeshReference)RefreshAfterEditAppointment);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    MODULE_LINK__APPOINTMENT_INFO,
                    this.moduleData != null ? this.moduleData.RoomId : 0,
                    this.moduleData != null ? this.moduleData.RoomTypeId : 0,
                    listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshAfterEditAppointment()
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPrintAppointment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var row = this.gridViewTreatmentAppointment.GetFocusedRow() as ADO.TreatmentAppointmentADO;
                if (row == null || !row.APPOINTMENT_TIME.HasValue) return;

                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, row);
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(
                    hisTreatment, this.moduleData != null ? this.moduleData.RoomId : 0);
                printProcess.Print(PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
