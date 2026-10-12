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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    /// <summary>
    /// Viec 58006: chon benh nhan co don chua tao phieu xuat trong ngay ngay tai o "Benh nhan"
    /// (thay thao tac DS (F3) -> loc Chua tao phieu xuat -> tich -> Dong y).
    /// - Chi tao khi bat config; chi dung khi KHONG tich Vang lai (Vang lai -> o ten nhap tay nhu cu).
    /// - O ten benh nhan cu van giu (an di) de moi luong nap don hien co tiep tuc dien vao;
    ///   combobox hien ten benh nhan dang mo qua NullText.
    /// - Nap lai danh sach moi lan mo va sau khi luu thanh cong.
    /// </summary>
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private GridLookUpEdit cboPendingPatient;
        private List<PendingPatientADO> pendingPatients;
        private bool isPendingPatientResetting;

        /// <summary>Tao combobox benh nhan cho xuat (goi trong Load)</summary>
        private void InitPendingPatientCombo()
        {
            try
            {
                if (!HisConfigCFG.IS_SHOW_PENDING_PATIENT_LIST || this.cboPendingPatient != null)
                    return;

                this.cboPendingPatient = new GridLookUpEdit();
                this.cboPendingPatient.Name = "cboPendingPatient";
                this.cboPendingPatient.StyleController = this.layoutControl1;
                this.cboPendingPatient.Visible = false;
                this.cboPendingPatient.Properties.DisplayMember = "DISPLAY";
                this.cboPendingPatient.Properties.ValueMember = "PATIENT_ID";
                this.cboPendingPatient.Properties.NullText = "";
                this.cboPendingPatient.Properties.TextEditStyle = TextEditStyles.Standard;
                this.cboPendingPatient.Properties.ImmediatePopup = true;
                this.cboPendingPatient.Properties.PopupFilterMode = PopupFilterMode.Contains;
                this.cboPendingPatient.Properties.PopupFormSize = new Size(560, 300);
                this.cboPendingPatient.Properties.DataSource = new List<PendingPatientADO>();

                var view = this.cboPendingPatient.Properties.View;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsSelection.EnableAppearanceFocusedCell = false;
                view.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
                AddPendingPatientColumn(view, "PATIENT_CODE", "Mã BN", 90);
                AddPendingPatientColumn(view, "PATIENT_NAME", "Tên bệnh nhân", 200);
                AddPendingPatientColumn(view, "DOB_STR", "Ngày sinh", 80);
                AddPendingPatientColumn(view, "GENDER_NAME", "Giới tính", 70);
                AddPendingPatientColumn(view, "PRES_COUNT", "Số đơn", 60);

                this.cboPendingPatient.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.cboPendingPatient_QueryPopUp);
                this.cboPendingPatient.EditValueChanged += new EventHandler(this.cboPendingPatient_EditValueChanged);
                this.txtVirPatientName.EditValueChanged += new EventHandler(this.txtVirPatientName_SyncPendingPatient);

                this.layoutControl1.Controls.Add(this.cboPendingPatient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddPendingPatientColumn(DevExpress.XtraGrid.Views.Grid.GridView view, string fieldName, string caption, int width)
        {
            GridColumn column = view.Columns.AddField(fieldName);
            column.Caption = caption;
            column.Width = width;
            column.Visible = true;
            column.VisibleIndex = view.VisibleColumns.Count;
        }

        /// <summary>
        /// Dat o "Benh nhan" theo che do: khong Vang lai -> combobox; Vang lai -> o ten nhap tay nhu cu.
        /// Goi cuoi EnableByCheckIsVisitor.
        /// </summary>
        private void SetPendingPatientMode()
        {
            try
            {
                if (this.cboPendingPatient == null)
                    return;

                bool useCombo = !checkIsVisitor.Checked;
                // Gan control moi vao item TRUOC roi moi an control cu: an control khi no con nam trong item
                // thi DevExpress 15.2 tu bat lai -> 2 o cung hien, o ten (khoa, trong) de len combobox.
                this.layoutControl1.BeginUpdate();
                try
                {
                    if (useCombo && this.layoutPatient.Control != this.cboPendingPatient)
                        this.layoutPatient.Control = this.cboPendingPatient;
                    else if (!useCombo && this.layoutPatient.Control == this.cboPendingPatient)
                        this.layoutPatient.Control = this.txtVirPatientName;
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }

                // Chot lai hien/an sau khi layout xong (chay ca khi khong hoan o -> Moi / Don moi tu sua trang thai lech)
                Control shown = useCombo ? (Control)this.cboPendingPatient : this.txtVirPatientName;
                Control hidden = useCombo ? (Control)this.txtVirPatientName : this.cboPendingPatient;
                hidden.Visible = false;
                shown.Visible = true;
                shown.BringToFront();

                this.cboPendingPatient.Enabled = useCombo && btnSearchPres.Enabled;
                ResetPendingPatientEditValue();
                SyncPendingPatientDisplay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay don chua tao phieu xuat hom nay (cung dieu kien DS (F3)), gop theo benh nhan</summary>
        private void LoadPendingPatients()
        {
            try
            {
                if (this.cboPendingPatient == null)
                    return;

                List<PendingPatientADO> result = new List<PendingPatientADO>();
                try
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqView11Filter filter = new HisServiceReqView11Filter();
                    // Cung bo loc mac dinh cua DS (F3): sap theo thoi gian sua don moi nhat
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                    };
                    filter.ROOM_ID_EQUAL_EXECUTE_OR_REQUEST_EQUAL_EXECUTE = this.currentModule.RoomId;
                    filter.IS_HAS_EXP_MEST = false;
                    long today = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                    filter.INTRUCTION_DATE_FROM = today;
                    filter.INTRUCTION_DATE_TO = today;

                    var serviceReqs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_11>>("api/HisServiceReq/GetView11", ApiConsumers.MosConsumer, filter, param);
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        // GroupBy giu thu tu xuat hien dau tien -> benh nhan xep theo don sua moi nhat, nhu DS (F3)
                        result = serviceReqs
                            .GroupBy(o => o.TDL_PATIENT_ID)
                            .Select(g => new PendingPatientADO(g.ToList()))
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    // Loi lay danh sach -> danh sach rong, khong chan thao tac khac cua man
                    result = new List<PendingPatientADO>();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                this.pendingPatients = result;
                this.cboPendingPatient.Properties.DataSource = this.pendingPatients;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Luu thanh cong -> nap lai danh sach, giu nguyen phieu vua luu tren man</summary>
        private void ReloadPendingPatientsAfterSave(bool success)
        {
            try
            {
                if (success && this.cboPendingPatient != null)
                    LoadPendingPatients();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPendingPatient_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadPendingPatients();
        }

        private void cboPendingPatient_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isPendingPatientResetting || this.cboPendingPatient.EditValue == null || this.pendingPatients == null)
                    return;

                long patientId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPendingPatient.EditValue.ToString());
                PendingPatientADO selected = this.pendingPatients.FirstOrDefault(o => o.PATIENT_ID == patientId);
                if (selected == null)
                    return;

                // Chay sau khi popup dong: nap don co the hien WaitingManager / thong bao
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        ResetPendingPatientEditValue();
                        ChoosePrescription(selected.ServiceReqs);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Combobox luon de EditValue = null, ten benh nhan dang mo hien qua NullText
        /// -> hien dung ca khi don nap bang DS (F3) / ma y lenh (benh nhan khong co trong danh sach cho)
        /// </summary>
        private void ResetPendingPatientEditValue()
        {
            if (this.cboPendingPatient == null)
                return;

            this.isPendingPatientResetting = true;
            try
            {
                this.cboPendingPatient.EditValue = null;
                this.cboPendingPatient.IsModified = false;
            }
            finally
            {
                this.isPendingPatientResetting = false;
            }
        }

        private void txtVirPatientName_SyncPendingPatient(object sender, EventArgs e)
        {
            SyncPendingPatientDisplay();
        }

        private void SyncPendingPatientDisplay()
        {
            try
            {
                if (this.cboPendingPatient == null)
                    return;

                this.cboPendingPatient.Properties.NullText = this.txtVirPatientName.Text ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
