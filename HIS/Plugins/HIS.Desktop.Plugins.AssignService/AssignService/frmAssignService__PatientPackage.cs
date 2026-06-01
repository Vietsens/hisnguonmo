using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignService.ADO;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService
    {
        private DevExpress.XtraGrid.Columns.GridColumn gColPatientPackageName;

        private readonly Dictionary<long, string> patientPackageNameByServiceId = new Dictionary<long, string>();

        private readonly Dictionary<long, long> patientPackageIdByServiceId = new Dictionary<long, long>();

        private readonly Dictionary<long, decimal> patientPackageUnitPriceByServiceId = new Dictionary<long, decimal>();

        private bool isPatientPackageColumnHooked = false;

        private void SetupPatientPackageColumn()
        {
            try
            {
                if (this.gColPatientPackageName == null && this.gridViewServiceProcess.Columns["PatientPackageNameUnbound"] == null)
                {
                    this.gColPatientPackageName = this.gridViewServiceProcess.Columns.AddField("PatientPackageNameUnbound");
                    this.gColPatientPackageName.Name = "gColPatientPackageName";
                    this.gColPatientPackageName.Caption = "Gói bệnh nhân";
                    this.gColPatientPackageName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    this.gColPatientPackageName.OptionsColumn.AllowEdit = false;
                    this.gColPatientPackageName.OptionsColumn.ReadOnly = true;
                    this.gColPatientPackageName.Width = 140;
                    this.gColPatientPackageName.Visible = true;

                    var conditionCol = this.gridViewServiceProcess.Columns["SERVICE_CONDITION_NAME"];
                    this.gColPatientPackageName.VisibleIndex = (conditionCol != null)
                        ? conditionCol.VisibleIndex + 1
                        : this.gridViewServiceProcess.Columns.Count;

                    try
                    {
                        string caption = Inventec.Common.Resource.Get.Value(
                            "frmAssignService.gColPatientPackageName.Caption",
                            Resources.ResourceLanguageManager.LanguageResource,
                            LanguageManager.GetCulture());
                        if (!string.IsNullOrEmpty(caption))
                            this.gColPatientPackageName.Caption = caption;
                    }
                    catch (Exception ex) { LogSystem.Warn(ex); }
                }

                if (!this.isPatientPackageColumnHooked)
                {
                    this.gridViewServiceProcess.CustomUnboundColumnData += this.gridViewServiceProcess_CustomUnboundColumnData_PatientPackage;
                    if (this.treeService != null)
                        this.treeService.AfterCheckNode += this.treeService_AfterCheckNode_CleanupPatientPackage;
                    this.gridViewServiceProcess.CellValueChanged += this.gridViewServiceProcess_CellValueChanged_CleanupPatientPackage;
                    this.isPatientPackageColumnHooked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void treeService_AfterCheckNode_CleanupPatientPackage(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            this.CleanupOrphanPatientPackageMappings();
        }

        private void gridViewServiceProcess_CellValueChanged_CleanupPatientPackage(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                if (e.Column.FieldName != "IsChecked") return;
                var row = this.gridViewServiceProcess.GetRow(e.RowHandle) as SereServADO;
                if (row != null && !row.IsChecked)
                {
                    this.patientPackageNameByServiceId.Remove(row.SERVICE_ID);
                    this.patientPackageIdByServiceId.Remove(row.SERVICE_ID);
                    this.patientPackageUnitPriceByServiceId.Remove(row.SERVICE_ID);
                    this.gridViewServiceProcess.RefreshData();
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void CleanupOrphanPatientPackageMappings()
        {
            try
            {
                if (this.ServiceIsleafADOs == null) return;
                var checkedIds = new HashSet<long>(this.ServiceIsleafADOs.Where(o => o.IsChecked).Select(o => o.SERVICE_ID));

                var orphanNames = this.patientPackageNameByServiceId.Keys.Where(k => !checkedIds.Contains(k)).ToList();
                foreach (var k in orphanNames) this.patientPackageNameByServiceId.Remove(k);

                var orphanIds = this.patientPackageIdByServiceId.Keys.Where(k => !checkedIds.Contains(k)).ToList();
                foreach (var k in orphanIds) this.patientPackageIdByServiceId.Remove(k);

                var orphanPrices = this.patientPackageUnitPriceByServiceId.Keys.Where(k => !checkedIds.Contains(k)).ToList();
                foreach (var k in orphanPrices) this.patientPackageUnitPriceByServiceId.Remove(k);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData_PatientPackage(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                if (e.Column == null) return;

                var source = ((BaseView)sender).DataSource as System.Collections.IList;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;

                var row = source[e.ListSourceRowIndex] as SereServADO;
                if (row == null) return;

                if (this.gColPatientPackageName != null && e.Column == this.gColPatientPackageName)
                {
                    string name;
                    if (this.patientPackageNameByServiceId.TryGetValue(row.SERVICE_ID, out name))
                        e.Value = name;
                    return;
                }

                if (e.Column.FieldName == "PRICE_DISPLAY" && row.IsChecked)
                {
                    decimal unitPrice;
                    if (this.patientPackageUnitPriceByServiceId.TryGetValue(row.SERVICE_ID, out unitPrice))
                        e.Value = unitPrice;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnPatientPackage_Click(object sender, EventArgs e)
        {
            try
            {
                long pid = (this.currentTreatment != null) ? this.currentTreatment.PATIENT_ID : 0;
                if (pid <= 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonBenhNhanDeChonGoi,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var allowedIds = this.ServiceIsleafADOs != null
                    ? new HashSet<long>(this.ServiceIsleafADOs.Select(o => o.SERVICE_ID))
                    : new HashSet<long>();

                var frm = new HIS.Desktop.Plugins.AssignService.PatientPackage.frmPatientPackage(
                    pid, this.OnPatientPackageServicesSelected, this.currentModule, allowedIds);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void OnPatientPackageServicesSelected(List<PatientPackageDtADO> selectedDts)
        {
            try
            {
                if (selectedDts == null || selectedDts.Count == 0 || this.ServiceIsleafADOs == null)
                    return;

                bool hasService = false;
                StringBuilder notFoundServices = new StringBuilder();

                foreach (var dt in selectedDts)
                {
                    if (dt.SERVICE_ID == null) continue;

                    var service = this.ServiceIsleafADOs.FirstOrDefault(o => o.SERVICE_ID == dt.SERVICE_ID);
                    if (service == null)
                    {
                        if (notFoundServices.Length > 0) notFoundServices.Append(", ");
                        notFoundServices.Append(dt.SV_SERVICE_CODE);
                        continue;
                    }

                    service.IsChecked = true;
                    service.AMOUNT = dt.AmountThisTime > 0 ? dt.AmountThisTime : 1;

                    this.patientPackageNameByServiceId[service.SERVICE_ID] = dt.PATIENT_PACKAGE_NAME;
                    if (dt.PATIENT_PACKAGE_ID > 0)
                        this.patientPackageIdByServiceId[service.SERVICE_ID] = dt.PATIENT_PACKAGE_ID;
                    this.patientPackageUnitPriceByServiceId[service.SERVICE_ID] = dt.UNIT_PRICE;

                    if (this.currentHisTreatment != null && this.currentHisTreatment.GUARANTEE_CODE != null)
                        service.IsGuarantee = true;

                    long defaultPatientTypeId = this.currentHisPatientTypeAlter != null
                        ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0;
                    long patientTypeIdForService = defaultPatientTypeId;
                    if (dt.PATIENT_PACKAGE_PATIENT_TYPE_ID.HasValue
                        && dt.PATIENT_PACKAGE_PATIENT_TYPE_ID.Value > 0
                        && dt.PATIENT_PACKAGE_PATIENT_TYPE_ID.Value != defaultPatientTypeId
                        && HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.HasServicePatyWithListPatientType(
                                service.SERVICE_ID,
                                new List<long> { dt.PATIENT_PACKAGE_PATIENT_TYPE_ID.Value }))
                    {
                        patientTypeIdForService = dt.PATIENT_PACKAGE_PATIENT_TYPE_ID.Value;
                    }
                    this.ChoosePatientTypeDefaultlService(patientTypeIdForService, service.SERVICE_ID, service);

                    if (!VerifyCheckFeeWhileAssign())
                    {
                        this.ResetOneService(service);
                        service.IsChecked = false;
                        break;
                    }

                    this.FillDataOtherPaySourceDataRow(service);
                    this.SetAssignNumOrder(service);

                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomList = null;
                    FilterExecuteRoom(service, ref executeRoomList);
                    long executeRoomId = this.SetPriorityRequired(executeRoomList);
                    if (executeRoomId <= 0)
                        executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
                    if (service.TDL_EXECUTE_ROOM_ID <= 0)
                        service.TDL_EXECUTE_ROOM_ID = executeRoomId;

                    this.ValidServiceDetailProcessing(service);
                    hasService = true;
                }

                var sereServADOs = this.ServiceIsleafADOs.Where(o => o.IsChecked)
                    .OrderByDescending(o => o.SERVICE_NUM_ORDER)
                    .ThenBy(o => o.TDL_SERVICE_NAME).ToList();

                if (hasService && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() != "true")
                    this.toggleSwitchDataChecked.EditValue = true;

                this.ValidOnlyShowNoticeService(sereServADOs);
                this.SetEnableButtonControl(this.actionType);
                this.SetDefaultSerServTotalPrice();
                this.VerifyWarningOverCeiling();

                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = sereServADOs;

                if (notFoundServices.Length > 0)
                {
                    XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.DichVuTrongGoiKhongCoTrongDanhMuc, notFoundServices.ToString()),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
