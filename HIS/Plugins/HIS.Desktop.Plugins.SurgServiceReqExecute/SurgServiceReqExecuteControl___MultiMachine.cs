using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    /// <summary>
    /// Chon NHIEU may cho 1 dich vu phau thuat thu thuat.
    /// MACHINE_ID/MACHINE_CODE giu nguyen y nghia cu = may DAU TIEN (cac noi khac van doc 1 may),
    /// MACHINE_IDS/MACHINE_CODES = ca danh sach may da tich, ngan cach bang dau CHAM PHAY.
    /// Port tu man Xu ly dich vu can lam sang (HIS.Desktop.Plugins.ServiceExecute).
    /// </summary>
    public partial class SurgServiceReqExecuteControl
    {
        /// <summary>
        /// Ten cot checkbox chon dong do DevExpress TU SINH khi MultiSelectMode = CheckBoxRowSelect.
        /// Cot nay khong nam trong view.Columns nen chi doi chieu duoc bang ten.
        /// </summary>
        private const string CHECKBOX_SELECTOR_COLUMN_NAME = "DX$CheckboxSelectorColumn";

        /// <summary>
        /// Ky tu ngan cach danh sach may. May tram ghi bang dau cham phay,
        /// van chap nhan dau phay de doc duoc du lieu luu truoc day.
        /// </summary>
        private static readonly char[] MACHINE_SEPARATORS = new char[] { ';', ',' };

        /// <summary>
        /// Anh chup danh sach may luc mo popup, de Esc con hoan tac duoc.
        /// null = khong co phien tich nao dang mo.
        /// </summary>
        private List<long> machineOriginIds_MultiMachine;

        /// <summary>
        /// Dang tich lai theo du lieu da luu, khong phai nguoi dung thao tac.
        /// </summary>
        private bool isRestoreCheckedMachine_MultiMachine;

        /// <summary>
        /// Cac may cua dich vu dang xu ly, theo dung thu tu nguoi dung da chon.
        /// </summary>
        private List<long> currentMachineIds_MultiMachine = new List<long>();

        /// <summary>
        /// Tach chuoi ID may (vd "12;15") thanh danh sach, bo trung va bo gia tri khong hop le.
        /// </summary>
        private List<long> ParseMachineIds_MultiMachine(string machineIds)
        {
            List<long> result = new List<long>();
            try
            {
                if (string.IsNullOrWhiteSpace(machineIds)) return result;

                string[] tokens = machineIds.Split(MACHINE_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
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
        /// Cac may da luu cua dich vu: uu tien MACHINE_IDS (nhieu may),
        /// chua co thi lay MACHINE_ID (du lieu cu, truoc khi bo sung tinh nang).
        /// </summary>
        private List<long> GetSavedMachineIds_MultiMachine()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.SereServExt == null) return result;

                result = ParseMachineIds_MultiMachine(this.SereServExt.MACHINE_IDS);
                if (result.Count == 0 && this.SereServExt.MACHINE_ID.HasValue && this.SereServExt.MACHINE_ID.Value > 0)
                {
                    result.Add(this.SereServExt.MACHINE_ID.Value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private HIS_MACHINE GetMachineById_MultiMachine(long machineId)
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
        /// Tra null khi co dong duoc chon ma khong doc duoc dong nao (DataSource khong phai HIS_MACHINE):
        /// tra list rong se bi hieu nham thanh "nguoi dung da bo tich het".
        /// </summary>
        private List<HIS_MACHINE> GetCheckedMachines_MultiMachine(GridView view)
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
        private void ApplyMachinesToExt(HIS_SERE_SERV_EXT ext)
        {
            try
            {
                if (ext == null) return;

                List<long> validIds = new List<long>();
                List<string> codes = new List<string>();
                foreach (long machineId in this.currentMachineIds_MultiMachine)
                {
                    var machine = GetMachineById_MultiMachine(machineId);
                    if (machine == null) continue;//bo may khong resolve duoc de 2 chuoi luon cung so phan tu
                    validIds.Add(machine.ID);
                    codes.Add(machine.MACHINE_CODE ?? "");
                }

                ext.MACHINE_ID = validIds.Count > 0 ? (long?)validIds[0] : null;
                ext.MACHINE_CODE = codes.Count > 0 ? codes[0] : null;
                ext.MACHINE_IDS = validIds.Count > 0 ? String.Join(";", validIds) : null;
                ext.MACHINE_CODES = codes.Count > 0 ? String.Join(";", codes) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hien thi cac may dang chon len o nhap va o ma may.
        /// </summary>
        private void DisplayCurrentMachines_MultiMachine()
        {
            try
            {
                List<string> codes = new List<string>();
                List<string> names = new List<string>();
                foreach (long machineId in this.currentMachineIds_MultiMachine)
                {
                    var machine = GetMachineById_MultiMachine(machineId);
                    if (machine == null) continue;
                    codes.Add(machine.MACHINE_CODE ?? "");
                    names.Add(machine.MACHINE_NAME ?? "");
                }

                if (this.txtMachineCode != null)
                {
                    this.txtMachineCode.Text = String.Join(";", codes);
                }

                if (this.cboMachine != null)
                {
                    //Luon hien ten may qua NullText va de EditValue = null, ke ca khi chi tich 1 may.
                    //Khong duoc de EditValue mang id roi trong cho GridLookUpEdit tu tra ten:
                    //view dang bat MultiSelectMode = CheckBoxRowSelect nen DevExpress dong bo lai EditValue
                    //theo dong focus khi dong popup, ket qua la o nhap bi trang khi chi chon 1 may.
                    this.cboMachine.Properties.NullText = names.Count > 0 ? String.Join("; ", names) : "";
                    this.cboMachine.EditValue = null;
                    this.cboMachine.Properties.Buttons[1].Visible = this.currentMachineIds_MultiMachine.Count > 0;

                    //Doi NullText KHONG tu lam editor ve lai text: EditValue von da la null nen gan null
                    //lan nua khong sinh EditValueChanged, o nhap giu nguyen anh cu va chi doi khi popup
                    //mo lai => nguoi dung phai chon 2 lan moi thay ten may.
                    //UpdateDisplayText() la protected nen goi qua reflection; that bai thi ve lai ca control.
                    ForceRefreshDisplayText_MultiMachine(this.cboMachine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ep editor tinh lai text hien thi tu NullText ngay lap tuc.
        /// Can vi doi rieng Properties.NullText khong lam editor ve lai: no chi doc lai NullText
        /// khi co EditValueChanged hoac khi popup mo lai, nen o nhap se tre mot nhip
        /// (chon may lan 1 khong thay ten, lan 2 moi thay).
        /// UpdateDisplayText() la protected tu TextEdit nen phai goi qua reflection;
        /// neu DevExpress doi API thi rot xuong Invalidate() de it nhat con ve lai control.
        /// </summary>
        private void ForceRefreshDisplayText_MultiMachine(BaseEdit editor)
        {
            try
            {
                if (editor == null) return;

                var method = editor.GetType().GetMethod(
                    "UpdateDisplayText",
                    System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.FlattenHierarchy,
                    null, Type.EmptyTypes, null);

                if (method != null) method.Invoke(editor, null);

                editor.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tich lai dung cac may dang chon khi mo popup.
        /// Phai lam o su kien Popup vi luc do DevExpress moi set dong focus theo gia tri hien tai,
        /// tich truoc do (luc do du lieu combo) se bi xoa.
        /// </summary>
        private void cboMachine_Popup(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null || cbo.Properties == null) return;

                GridView view = cbo.Properties.View as GridView;
                if (view == null) return;

                List<long> machineIds = new List<long>(this.currentMachineIds_MultiMachine);
                this.machineOriginIds_MultiMachine = new List<long>(machineIds);//anh chup de Esc con tra lai duoc

                this.isRestoreCheckedMachine_MultiMachine = true;
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
                    this.isRestoreCheckedMachine_MultiMachine = false;
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
        /// Esc van hoan tac duoc nho anh chup machineOriginIds_MultiMachine.
        /// </summary>
        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (this.isRestoreCheckedMachine_MultiMachine) return;

                GridView view = sender as GridView;
                var machines = GetCheckedMachines_MultiMachine(view);
                //null = khong doc duoc dong nao (DataSource khong phai HIS_MACHINE) => khong ket luan duoc gi,
                //tuyet doi khong hieu thanh "nguoi dung da bo tich het" roi xoa trang may dang co
                if (machines == null) return;

                List<long> newIds = machines.Select(o => o.ID).ToList();

                //may da luu nhung KHONG nam trong danh sach dang hien thi (bi loc theo phong/dich vu)
                //thi khong the tich lai duoc => phai giu nguyen, khong duoc coi la nguoi dung bo tich
                List<long> hiddenIds = new List<long>();
                foreach (long id in this.currentMachineIds_MultiMachine)
                {
                    if (newIds.Contains(id)) continue;
                    if (IsMachineInView_MultiMachine(view, id)) continue;//co tren danh sach ma khong tich => nguoi dung da bo
                    hiddenIds.Add(id);
                }

                //giu may an dung vi tri cu de khong dao thu tu nguoi dung da chon
                this.currentMachineIds_MultiMachine = this.currentMachineIds_MultiMachine
                    .Where(o => hiddenIds.Contains(o))
                    .Concat(newIds.Where(o => !hiddenIds.Contains(o)))
                    .ToList();

                DisplayCurrentMachines_MultiMachine();
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
        private void gridView3_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;

                GridView view = sender as GridView;
                if (view == null) return;

                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                if (hitInfo == null || !hitInfo.InRowCell || !view.IsDataRow(hitInfo.RowHandle)) return;
                if (hitInfo.Column == null) return;
                if (hitInfo.Column.Name == CHECKBOX_SELECTOR_COLUMN_NAME
                    || hitInfo.Column.FieldName == CHECKBOX_SELECTOR_COLUMN_NAME) return;//dang tich nhieu may, giu popup

                //dang tich tu 2 may tro len thi chi dong popup, khong duoc pha lua chon cua nguoi dung
                int[] selected = view.GetSelectedRows();
                if (selected == null || selected.Length <= 1)
                {
                    view.BeginSelection();
                    view.ClearSelection();
                    view.SelectRow(hitInfo.RowHandle);
                    view.EndSelection();
                }

                if (this.cboMachine != null) this.cboMachine.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dang co it nhat 1 may duoc tich hay khong.
        /// Dung thay cho cboMachine.EditValue != null: khi tich tu 2 may tro len EditValue co chu y de null.
        /// </summary>
        private bool HasCheckedMachine_MultiMachine()
        {
            return this.currentMachineIds_MultiMachine != null && this.currentMachineIds_MultiMachine.Count > 0;
        }

        /// <summary>
        /// May co nam trong danh sach dang hien thi tren popup khong.
        /// May da luu nhung bi loc khoi danh sach (theo phong/dich vu) thi khong tich lai duoc,
        /// nen khong duoc coi la nguoi dung da bo tich.
        /// </summary>
        private bool IsMachineInView_MultiMachine(GridView view, long machineId)
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
        private void RestoreMachinesOnCancel_MultiMachine()
        {
            try
            {
                if (this.machineOriginIds_MultiMachine == null) return;

                this.currentMachineIds_MultiMachine = new List<long>(this.machineOriginIds_MultiMachine);
                this.machineOriginIds_MultiMachine = null;
                DisplayCurrentMachines_MultiMachine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xoa het may dang chon (doi sang dich vu chua co du lieu, hoac bam nut xoa).
        /// </summary>
        private void ClearMachines_MultiMachine()
        {
            try
            {
                this.currentMachineIds_MultiMachine = new List<long>();
                if (this.cboMachine != null)
                {
                    this.cboMachine.Properties.NullText = "";
                }
                if (this.txtMachineCode != null)
                {
                    this.txtMachineCode.Text = "";
                }
                DisplayCurrentMachines_MultiMachine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tich lai cac may da luu khi mo lai man hinh xu ly.
        /// Goi sau khi combo may da nap xong du lieu.
        /// </summary>
        private void RestoreSavedMachines_MultiMachine()
        {
            try
            {
                this.currentMachineIds_MultiMachine = GetSavedMachineIds_MultiMachine();
                DisplayCurrentMachines_MultiMachine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
