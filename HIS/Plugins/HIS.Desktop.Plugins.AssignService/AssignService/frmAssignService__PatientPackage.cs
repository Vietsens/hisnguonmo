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
                    this.isPatientPackageColumnHooked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData_PatientPackage(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                if (this.gColPatientPackageName == null || e.Column != this.gColPatientPackageName) return;

                var source = ((BaseView)sender).DataSource as System.Collections.IList;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;

                var row = source[e.ListSourceRowIndex] as SereServADO;
                if (row == null) return;

                string name;
                if (this.patientPackageNameByServiceId.TryGetValue(row.SERVICE_ID, out name))
                    e.Value = name;
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

                var frm = new HIS.Desktop.Plugins.AssignService.PatientPackage.frmPatientPackage(
                    pid, this.OnPatientPackageServicesSelected, this.currentModule);
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

                    if (this.currentHisTreatment != null && this.currentHisTreatment.GUARANTEE_CODE != null)
                        service.IsGuarantee = true;

                    this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, service.SERVICE_ID, service);

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
