using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    /// <summary>
    /// Chon NHIEU may cho 1 dich vu phau thuat thu thuat (ban 2).
    /// MACHINE_ID/MACHINE_CODE giu nguyen y nghia cu = may DAU TIEN (cac noi khac van doc 1 may),
    /// MACHINE_IDS/MACHINE_CODES = ca danh sach may da tich, ngan cach bang dau CHAM PHAY.
    /// Port tu man Xu ly dich vu can lam sang (HIS.Desktop.Plugins.ServiceExecute).
    /// </summary>
    public partial class UCSurgServiceReqExecute2
    {
        /// <summary>
        /// Ten cot checkbox chon dong do DevExpress TU SINH khi MultiSelectMode = CheckBoxRowSelect.
        /// Cot nay khong nam trong view.Columns nen chi doi chieu duoc bang ten.
        /// </summary>
        private const string CHECKBOX_SELECTOR_COLUMN_NAME_MM = "DX$CheckboxSelectorColumn";

        /// <summary>
        /// Ky tu ngan cach danh sach may. May tram ghi bang dau cham phay,
        /// van chap nhan dau phay de doc duoc du lieu luu truoc day.
        /// </summary>
        private static readonly char[] MACHINE_SEPARATORS_MM = new char[] { ';', ',' };

        /// <summary>
        /// Cac may cua dich vu dang xu ly, theo dung thu tu nguoi dung da chon.
        /// </summary>
        private List<long> currentMachineIds_MM = new List<long>();

        /// <summary>
        /// Dang tich lai theo du lieu da luu, khong phai nguoi dung thao tac.
        /// </summary>
        private bool isRestoreCheckedMachine_MM;

        /// <summary>
        /// Anh chup danh sach may luc mo popup, de Esc con hoan tac duoc.
        /// null = khong co phien tich nao dang mo.
        /// </summary>
        private List<long> machineOriginIds_MM;

        /// <summary>
        /// Dang co it nhat 1 may duoc tich hay khong.
        /// Dung thay cho cboMachine.EditValue != null: khi tich tu 2 may tro len EditValue co chu y de null.
        /// </summary>
        private bool HasCheckedMachine_MM()
        {
            return this.currentMachineIds_MM != null && this.currentMachineIds_MM.Count > 0;
        }

        /// <summary>
        /// May co nam trong danh sach dang hien thi tren popup khong.
        /// May da luu nhung bi loc khoi danh sach (theo phong/dich vu) thi khong tich lai duoc,
        /// nen khong duoc coi la nguoi dung da bo tich.
        /// </summary>
        private bool IsMachineInView_MM(GridView view, long machineId)
        {
            try
            {
                if (view == null) return false;

                for (int rowHandle = 0; rowHandle < view.RowCount; rowHandle++)
                {
                    var machine = view.GetRow(rowHandle) as HIS_MACHINE;
                    if (machine != null && machine.ID == machineId) return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Bam Esc: tra danh sach may ve dung trang thai truoc khi mo popup.
        /// </summary>
        private void CboMachine_v45072_Closed_MultiMachine(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Cancel)
                {
                    if (this.machineOriginIds_MM == null) return;

                    this.currentMachineIds_MM = new List<long>(this.machineOriginIds_MM);
                    this.machineOriginIds_MM = null;
                    DisplayCurrentMachines_MM();
                }
                else
                {
                    //dong popup binh thuong => bo anh chup, khong Esc lan sau lai hoan tac nham
                    this.machineOriginIds_MM = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tach chuoi ID may (vd "12;15") thanh danh sach, bo trung va bo gia tri khong hop le.
        /// </summary>
        private List<long> ParseMachineIds_MM(string machineIds)
        {
            List<long> result = new List<long>();
            try
            {
                if (string.IsNullOrWhiteSpace(machineIds)) return result;

                string[] tokens = machineIds.Split(MACHINE_SEPARATORS_MM, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    long machineId;
                    if (long.TryParse(token.Trim(), out machineId) && machineId > 0 && !result.Contains(machineId))
                    {
                        result.Add(machineId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Cac may da luu cua 1 ban ghi ext: uu tien MACHINE_IDS (nhieu may),
        /// chua co thi lay MACHINE_ID (du lieu cu, truoc khi bo sung tinh nang).
        /// </summary>
        private List<long> GetSavedMachineIds_MM(HIS_SERE_SERV_EXT ext)
        {
            List<long> result = new List<long>();
            try
            {
                if (ext == null) return result;

                result = ParseMachineIds_MM(ext.MACHINE_IDS);
                if (result.Count == 0 && ext.MACHINE_ID.HasValue && ext.MACHINE_ID.Value > 0)
                {
                    result.Add(ext.MACHINE_ID.Value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private HIS_MACHINE GetMachineById_MM(long machineId)
        {
            try
            {
                if (machineId <= 0) return null;

                var allMachines = BackendDataWorker.Get<HIS_MACHINE>();
                return allMachines != null ? allMachines.FirstOrDefault(o => o.ID == machineId) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Danh sach may dang duoc tich tren popup, theo dung thu tu dong hien thi.
        /// Tra null khi co dong duoc chon ma khong doc duoc dong nao: tra list rong
        /// se bi hieu nham thanh "nguoi dung da bo tich het".
        /// </summary>
        private List<HIS_MACHINE> GetCheckedMachines_MM(GridView view)
        {
            List<HIS_MACHINE> result = new List<HIS_MACHINE>();
            try
            {
                if (view == null) return result;

                int[] rowHandles = view.GetSelectedRows();
                if (rowHandles == null || rowHandles.Length == 0) return result;

                bool anyCast = false;
                foreach (int rowHandle in rowHandles.OrderBy(o => o))
                {
                    var machine = view.GetRow(rowHandle) as HIS_MACHINE;
                    if (machine == null) continue;
                    anyCast = true;
                    if (machine.ID <= 0) continue;
                    if (result.Any(o => o.ID == machine.ID)) continue;
                    result.Add(machine);
                }

                if (!anyCast) return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Gan may vao ext truoc khi luu.
        /// Gan VO DIEU KIEN ke ca khi khong con may nao: neu chi gan trong nhanh "co may"
        /// thi chuoi may cu se dong lai vinh vien, bo tich het ma van luu ten may cu.
        /// </summary>
        private void ApplyMachinesToExt_MM(HIS_SERE_SERV_EXT ext)
        {
            try
            {
                if (ext == null) return;

                List<long> validIds = new List<long>();
                List<string> codes = new List<string>();
                foreach (long machineId in this.currentMachineIds_MM)
                {
                    var machine = GetMachineById_MM(machineId);
                    if (machine == null) continue;//bo may khong resolve duoc de 2 chuoi luon cung so phan tu
                    validIds.Add(machine.ID);
                    codes.Add(machine.MACHINE_CODE ?? "");
                }

                ext.MACHINE_ID = validIds.Count > 0 ? (long?)validIds[0] : null;
                ext.MACHINE_CODE = codes.Count > 0 ? codes[0] : null;
                ext.MACHINE_IDS = validIds.Count > 0 ? String.Join(";", validIds) : null;
                ext.MACHINE_CODES = codes.Count > 0 ? String.Join(";", codes) : null;

                Inventec.Common.Logging.LogSystem.Debug("ApplyMachinesToExt_MM: MACHINE_IDS=" + (ext.MACHINE_IDS ?? "")
                    + ", MACHINE_CODES=" + (ext.MACHINE_CODES ?? ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hien thi cac may dang chon len o nhap.
        /// </summary>
        private void DisplayCurrentMachines_MM()
        {
            try
            {
                if (cboMachine_v45072 == null) return;

                List<string> names = new List<string>();
                foreach (long machineId in this.currentMachineIds_MM)
                {
                    var machine = GetMachineById_MM(machineId);
                    if (machine == null) continue;
                    names.Add(machine.MACHINE_NAME ?? "");
                }

                //EditValue chi mang duoc 1 gia tri nen khi tich tu 2 may tro len phai hien ten
                //ca danh sach qua NullText
                if (this.currentMachineIds_MM.Count > 1)
                {
                    cboMachine_v45072.Properties.NullText = String.Join("; ", names);
                    cboMachine_v45072.EditValue = null;
                }
                else
                {
                    cboMachine_v45072.Properties.NullText = "";
                    cboMachine_v45072.EditValue = this.currentMachineIds_MM.Count == 1
                        ? (object)this.currentMachineIds_MM[0]
                        : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tich lai dung cac may dang chon khi mo popup.
        /// Phai lam o su kien Popup vi luc do DevExpress moi set dong focus theo gia tri hien tai.
        /// </summary>
        private void CboMachine_v45072_Popup_MultiMachine(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null || cbo.Properties == null) return;

                GridView view = cbo.Properties.View as GridView;
                if (view == null) return;

                List<long> machineIds = new List<long>(this.currentMachineIds_MM);
                this.machineOriginIds_MM = new List<long>(machineIds);//anh chup de Esc con tra lai duoc

                this.isRestoreCheckedMachine_MM = true;
                try
                {
                    view.BeginSelection();
                    view.ClearSelection();
                    for (int rowHandle = 0; rowHandle < view.RowCount; rowHandle++)
                    {
                        var machine = view.GetRow(rowHandle) as HIS_MACHINE;
                        if (machine == null || !machineIds.Contains(machine.ID)) continue;
                        view.SelectRow(rowHandle);
                    }
                    view.EndSelection();
                }
                finally
                {
                    this.isRestoreCheckedMachine_MM = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ghi trang thai tich NGAY khi nguoi dung tich, khong doi den luc dong popup:
        /// bam sang cho khac deu tra ve CloseMode = Cancel, doi den Closed thi ca phien tich bi vut di.
        /// </summary>
        private void MachineView_v45072_SelectionChanged_MultiMachine(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (this.isRestoreCheckedMachine_MM) return;

                GridView view = sender as GridView;
                var machines = GetCheckedMachines_MM(view);
                //null = khong doc duoc dong nao => khong ket luan duoc gi,
                //tuyet doi khong hieu thanh "nguoi dung da bo tich het" roi xoa trang may dang co
                if (machines == null) return;

                List<long> newIds = machines.Select(o => o.ID).ToList();

                //may da luu nhung KHONG nam trong danh sach dang hien thi (bi loc theo phong/dich vu)
                //thi khong the tich lai duoc => phai giu nguyen, khong duoc coi la nguoi dung bo tich
                List<long> hiddenIds = new List<long>();
                foreach (long id in this.currentMachineIds_MM)
                {
                    if (newIds.Contains(id)) continue;
                    if (IsMachineInView_MM(view, id)) continue;//co tren danh sach ma khong tich => nguoi dung da bo
                    hiddenIds.Add(id);
                }

                //giu may an dung vi tri cu de khong dao thu tu nguoi dung da chon
                this.currentMachineIds_MM = this.currentMachineIds_MM
                    .Where(o => hiddenIds.Contains(o))
                    .Concat(newIds.Where(o => !hiddenIds.Contains(o)))
                    .ToList();

                DisplayCurrentMachines_MM();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Giu nguyen thao tac cu: click vao 1 dong may la chon may do va dong popup.
        /// Khi bat MultiSelect, DevExpress khong tu dong popup khi click dong nua nen phai tu dong o day;
        /// rieng click vao cot checkbox thi GIU popup de con tich tiep may khac.
        /// </summary>
        private void MachineView_v45072_MouseUp_MultiMachine(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;

                GridView view = sender as GridView;
                if (view == null) return;

                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                if (hitInfo == null || !hitInfo.InRowCell || !view.IsDataRow(hitInfo.RowHandle)) return;
                if (hitInfo.Column == null) return;
                if (hitInfo.Column.Name == CHECKBOX_SELECTOR_COLUMN_NAME_MM
                    || hitInfo.Column.FieldName == CHECKBOX_SELECTOR_COLUMN_NAME_MM) return;//dang tich nhieu may, giu popup

                //dang tich tu 2 may tro len thi chi dong popup, khong duoc pha lua chon cua nguoi dung
                int[] selected = view.GetSelectedRows();
                if (selected == null || selected.Length <= 1)
                {
                    view.BeginSelection();
                    view.ClearSelection();
                    view.SelectRow(hitInfo.RowHandle);
                    view.EndSelection();
                }

                if (cboMachine_v45072 != null) cboMachine_v45072.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tich lai cac may da luu khi mo lai man hinh xu ly.
        /// </summary>
        private void RestoreSavedMachines_MM(HIS_SERE_SERV_EXT ext)
        {
            try
            {
                this.currentMachineIds_MM = GetSavedMachineIds_MM(ext);
                DisplayCurrentMachines_MM();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xoa het may dang chon (nut xoa tren o chon may).
        /// </summary>
        private void ClearMachines_MM()
        {
            try
            {
                this.currentMachineIds_MM = new List<long>();
                if (cboMachine_v45072 != null)
                {
                    cboMachine_v45072.Properties.NullText = "";
                }
                DisplayCurrentMachines_MM();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
