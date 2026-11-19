using MOS.EFMODEL.DataModels;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{
    /// <summary>
    /// Việc 62129: thiết lập cảnh báo chuyển khám theo ICD của phòng khám chuyên đề.
    /// Giữ 4 giá trị trên form, SetDataExecuteRoom gán lại vào HIS_EXECUTE_ROOM khi lưu
    /// (api Update ghi nguyên đối tượng, không gán sẽ xóa trắng thiết lập).
    /// </summary>
    public partial class frmHisExecuteRoom
    {
        string addExamIcdCodes;
        string addExamWarningRoomIds;
        short? addExamIcdOption;
        string addExamTimes;

        private void SetAddExamWarning(V_HIS_EXECUTE_ROOM data)
        {
            addExamIcdCodes = data != null ? data.ADD_EXAM_ICD_CODES : null;
            addExamWarningRoomIds = data != null ? data.ADD_EXAM_WARNING_ROOM_IDS : null;
            addExamIcdOption = data != null ? data.ADD_EXAM_ICD_OPTION : null;
            addExamTimes = data != null ? data.ADD_EXAM_TIMES : null;
            ShowAddExamWarningSummary();
        }

        private void ShowAddExamWarningSummary()
        {
            int icdCount = String.IsNullOrWhiteSpace(addExamIcdCodes) ? 0 : addExamIcdCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length;
            btnAddExamWarning.Text = "Thiết lập cảnh báo chuyển khám" + (icdCount > 0 ? " (" + icdCount + " mã ICD)" : "");
        }

        private void FillAddExamWarning(HIS_EXECUTE_ROOM executeRoom)
        {
            executeRoom.ADD_EXAM_ICD_CODES = addExamIcdCodes;
            executeRoom.ADD_EXAM_WARNING_ROOM_IDS = addExamWarningRoomIds;
            executeRoom.ADD_EXAM_ICD_OPTION = addExamIcdOption;
            executeRoom.ADD_EXAM_TIMES = addExamTimes;
        }

        private void btnAddExamWarning_Click(object sender, EventArgs e)
        {
            try
            {
                using (frmAddExamWarning frm = new frmAddExamWarning(txtExecuteRoomName.Text, addExamIcdCodes, addExamWarningRoomIds, addExamIcdOption, addExamTimes))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        addExamIcdCodes = frm.IcdCodes;
                        addExamWarningRoomIds = frm.WarningRoomIds;
                        addExamIcdOption = frm.IcdOption;
                        addExamTimes = frm.Times;
                        ShowAddExamWarningSummary();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
