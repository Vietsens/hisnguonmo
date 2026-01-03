using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region GridView Event Handlers

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var data = e.Row as V_MCH_EXAM_SERVICE;
                    if (data == null) return;

                    if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        if (data.IN_TIME > 0)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.EXECUTE_TIME ?? 0);
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXECUTE_USER")
                    {
                        // Kết hợp LOGINNAME - USERNAME
                        if (!string.IsNullOrWhiteSpace(data.EXECUTE_LOGINNAME) && !string.IsNullOrWhiteSpace(data.EXECUTE_USERNAME))
                        {
                            e.Value = string.Format("{0} - {1}", data.EXECUTE_LOGINNAME, data.EXECUTE_USERNAME);
                        }
                        else if (!string.IsNullOrWhiteSpace(data.EXECUTE_USERNAME))
                        {
                            e.Value = data.EXECUTE_USERNAME;
                        }
                        else if (!string.IsNullOrWhiteSpace(data.EXECUTE_LOGINNAME))
                        {
                            e.Value = data.EXECUTE_LOGINNAME;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = gridView1.GetRow(e.RowHandle) as MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE;
                if (data != null)
                {
                    if (e.Column.FieldName == "SYNC_STATUS_STR")
                    {
                        if (data.SYNC_STATUS == 0)
                        {
                            e.RepositoryItem = repY;
                        }
                        else if (data.SYNC_STATUS == 2)
                        {
                            e.RepositoryItem = repN;
                        }
                    }
                    else if (e.Column.FieldName == "Edit")
                    {
                        if (Treatment != null && data.TREATMENT_CODE == (lblTreatmentCode.Text.Trim()))
                        {
                            e.RepositoryItem = repEditEna;
                        }
                        else
                        {
                            e.RepositoryItem = repEditDis;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        if (Treatment != null && data.TREATMENT_CODE == (lblTreatmentCode.Text.Trim()))
                        {
                            e.RepositoryItem = repDelEna;
                        }
                        else
                        {
                            e.RepositoryItem = repDelDis;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Repository Button Click Events

        private void repCopyEna_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var examServiceToCopy = (V_MCH_EXAM_SERVICE)gridView1.GetRow(gridView1.FocusedRowHandle);

                if (examServiceToCopy == null || examServiceToCopy.ID <= 0)
                {
                    XtraMessageBox.Show("Không tìm thấy thông tin đợt khám", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                WaitingManager.Show();

                ClearAllTabsData();
                ResetDataModels();

                bool loadSuccess = LoadExamServiceDetailDataForCopy(examServiceToCopy);

                WaitingManager.Hide();

                if (!loadSuccess)
                {
                    XtraMessageBox.Show("Không thể tải dữ liệu. Vui lòng thử lại!", "Lỗi",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                SwitchToTabAndFillData(examServiceToCopy.EXAM_SERVICE_TYPE_ID);
                SetDefaultExamDateAndUser(examServiceToCopy);
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void repEditEna_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ExamServiceEdit = (V_MCH_EXAM_SERVICE)gridView1.GetRow(gridView1.FocusedRowHandle);

                if (ExamServiceEdit == null || ExamServiceEdit.ID <= 0)
                {
                    XtraMessageBox.Show("Không tìm thấy thông tin đợt khám", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                WaitingManager.Show();

                ClearAllTabsData();
                ResetDataModels();
                LoadMch(ExamServiceEdit);
                bool loadSuccess = LoadExamServiceDetailData(ExamServiceEdit);

                WaitingManager.Hide();

                if (!loadSuccess)
                {
                    XtraMessageBox.Show("Không thể tải dữ liệu. Vui lòng thử lại!", "Lỗi",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                SwitchToTabAndFillData(ExamServiceEdit.EXAM_SERVICE_TYPE_ID);
                SetDefaultExamDateAndUser(ExamServiceEdit);
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void repDelEna_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //Không sử dụng xóa ở chức năng
                var examServiceToDelete = (V_MCH_EXAM_SERVICE)gridView1.GetRow(gridView1.FocusedRowHandle);

                if (examServiceToDelete == null || examServiceToDelete.ID <= 0)
                {
                    XtraMessageBox.Show("Không tìm thấy thông tin đợt khám cần xóa", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                System.Windows.Forms.DialogResult result = XtraMessageBox.Show(
                    string.Format("Bạn có chắc chắn muốn xóa thông tin đợt khám này?\nLoại khám: {0}\nMã hồ sơ: {1}",
                                  examServiceToDelete.EXAM_SERVICE_TYPE_NAME,
                                  examServiceToDelete.TREATMENT_CODE),
                    "Xác nhận xóa",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);

                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                WaitingManager.Show();

                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                bool deleteSuccess = false;

                WaitingManager.Hide();

                if (deleteSuccess)
                {
                    bool isRelatedData = IsDataRelatedToDeletedExamService(examServiceToDelete);

                    if (isRelatedData)
                    {
                        ClearAllTabsData();
                        ResetDataModels();
                        ExamServiceEdit = null;

                        Inventec.Common.Logging.LogSystem.Info("Đã clear dữ liệu tabs do xóa exam service liên quan - ID: " + examServiceToDelete.ID);
                    }


                    XtraMessageBox.Show("Xóa thông tin đợt khám thành công!", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
                else
                {
                    string errorMessage = "Xóa thất bại.";
                    if (param.Messages != null && param.Messages.Count > 0)
                    {
                        errorMessage += " " + string.Join(", ", param.Messages);
                    }

                    XtraMessageBox.Show(errorMessage, "Lỗi",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private bool IsDataRelatedToDeletedExamService(V_MCH_EXAM_SERVICE deletedExamService)
        {
            try
            {
                if (deletedExamService == null) return false;

                if (ExamServiceEdit != null && ExamServiceEdit.ID == deletedExamService.ID)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - ExamServiceEdit.ID matches: " + deletedExamService.ID);
                    return true;
                }

                if (ExamService != null && ExamService.ID == deletedExamService.ID)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - ExamService.ID matches: " + deletedExamService.ID);
                    return true;
                }

                if (_examService != null && _examService.ID == deletedExamService.ID)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _examService.ID matches: " + deletedExamService.ID);
                    return true;
                }

                bool hasRelatedChildData = false;

                if (_screening != null && _screening.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _screening.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }
                else if (_antenatalVisit != null && _antenatalVisit.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _antenatalVisit.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }
                else if (_birthInfo != null && _birthInfo.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _birthInfo.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }
                else if (_child != null && _child.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _child.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }
                else if (_contraception != null && _contraception.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _contraception.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }
                else if (_abortion != null && _abortion.EXAM_SERVICE_ID == deletedExamService.ID)
                {
                    hasRelatedChildData = true;
                    Inventec.Common.Logging.LogSystem.Debug("Data is related - _abortion.EXAM_SERVICE_ID matches: " + deletedExamService.ID);
                }

                if (hasRelatedChildData)
                {
                    return true;
                }

                Inventec.Common.Logging.LogSystem.Debug("Data is NOT related to deleted exam service ID: " + deletedExamService.ID);
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        #endregion
    }
}
