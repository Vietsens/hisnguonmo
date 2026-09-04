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
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.SelectedExam
{
    /// <summary>
    /// Cua so hien thi danh sach cong kham nguoi benh da chon trong mot luot dang ky tai kiosk.
    /// Cho phep xoa tung cong kham, chon them phong kham hoac dang ky toan bo danh sach.
    /// Danh sach duoc sua truc tiep tren tham chieu truyen vao nen man chon phong doc lai duoc ngay.
    /// </summary>
    public partial class frmSelectedExamList : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        /// <summary>Danh sach cong kham da chon, phan tu dau tien la cong kham chinh</summary>
        private List<ExamSelectionADO> examSelections;

        private Inventec.Desktop.Common.Modules.Module currentModule;

        /// <summary>Lua chon cuoi cung cua nguoi benh tren cua so nay</summary>
        public EnumSelectedExamAction ActionResult { get; private set; }

        #endregion

        #region Constructor - Load

        public frmSelectedExamList(List<ExamSelectionADO> examSelections, Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.examSelections = examSelections;
                this.currentModule = module;
                this.ActionResult = EnumSelectedExamAction.Close;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
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

        private void frmSelectedExamList_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();

                if (this.currentModule != null)
                {
                    RegisterTimer(this.currentModule.ModuleLink, "timerCloseFormSelectedExam", timerCloseForm.Interval, timerCloseForm_Tick);
                    StopTimer(this.currentModule.ModuleLink, "timerCloseFormSelectedExam");
                    timerCloseForm.Enabled = false;
                    timerCloseForm.Enabled = true;
                    StopTimer(this.currentModule.ModuleLink, "timerCloseFormSelectedExam");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        /// <summary>Do danh sach cong kham len luoi va cap nhat trang thai nut dang ky</summary>
        private void FillDataToGrid()
        {
            try
            {
                gridViewSelectedExam.BeginUpdate();
                try
                {
                    grdSelectedExam.DataSource = null;
                    grdSelectedExam.DataSource = this.examSelections;
                }
                finally
                {
                    gridViewSelectedExam.EndUpdate();
                }

                int count = (this.examSelections != null) ? this.examSelections.Count : 0;
                btnRegister.Text = (count > 0) ? String.Format("ĐĂNG KÝ ({0})", count) : "ĐĂNG KÝ";
                btnRegister.Enabled = count > 0;
                btnRegister.BackColor = (count > 0) ? Color.SeaGreen : Color.DarkGray;
                lblNote.Visible = count > 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Xoa mot cong kham khoi danh sach theo vi tri dong tren luoi</summary>
        private void DeleteExamSelection(int rowHandle)
        {
            try
            {
                var selection = gridViewSelectedExam.GetRow(rowHandle) as ExamSelectionADO;
                if (selection == null || this.examSelections == null) return;

                if (XtraMessageBoxConfirmDelete(selection) != DialogResult.Yes) return;

                this.examSelections.Remove(selection);

                Inventec.Common.Logging.LogSystem.Info("Kiosk xoa cong kham da chon____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selection.RoomId), selection.RoomId)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selection.ServiceId), selection.ServiceId));

                FillDataToGrid();
                ResetCloseFormTimer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private DialogResult XtraMessageBoxConfirmDelete(ExamSelectionADO selection)
        {
            return DevExpress.XtraEditors.XtraMessageBox.Show(
                String.Format("Bạn có muốn bỏ phòng khám {0} khỏi danh sách không?", selection.RoomName),
                "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>Moi thao tac cua nguoi benh deu cho bo dem tu dong dong man hinh chay lai tu dau</summary>
        private void ResetCloseFormTimer()
        {
            try
            {
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CloseWithAction(EnumSelectedExamAction action)
        {
            try
            {
                this.ActionResult = action;
                if (this.currentModule != null)
                {
                    StopTimer(this.currentModule.ModuleLink, "timerCloseFormSelectedExam");
                }
                timerCloseForm.Enabled = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void gridViewSelectedExam_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column != null && e.Column.FieldName == "NUM_ORDER_STR")
                {
                    e.Value = (e.ListSourceRowIndex + 1).ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DeleteExamSelection(gridViewSelectedExam.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddMore_Click(object sender, EventArgs e)
        {
            try
            {
                CloseWithAction(EnumSelectedExamAction.AddMore);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.examSelections == null || this.examSelections.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất một phòng khám.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CloseWithAction(EnumSelectedExamAction.Register);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                CloseWithAction(EnumSelectedExamAction.Close);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSelectedExamList_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    CloseWithAction(EnumSelectedExamAction.Close);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Nguoi benh khong thao tac trong thoi gian cho: dong cua so, khong dang ky</summary>
        private void timerCloseForm_Tick()
        {
            try
            {
                NameForm.CloseOtherForm();

                if (this.currentModule != null)
                {
                    StopTimer(this.currentModule.ModuleLink, "timerCloseFormSelectedExam");
                }
                timerCloseForm.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
