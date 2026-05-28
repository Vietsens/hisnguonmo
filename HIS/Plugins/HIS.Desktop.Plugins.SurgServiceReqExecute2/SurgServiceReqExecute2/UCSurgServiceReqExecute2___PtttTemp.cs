using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utilities;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : HIS.Desktop.Utility.UserControlBase
    {
        private void LoadDataToComboPtttTemp_v45072()
        {
            try
            {
                if (cboPtttTemp_v45072 == null) return;

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                long? departmentId = null;
                var room = BackendDataWorker.Get<V_HIS_ROOM>()
                    .FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                if (room != null) departmentId = room.DEPARTMENT_ID;

                currentPtttTemps_v45072 = BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && (o.IS_PUBLIC == 1
                            || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == departmentId)
                            || o.CREATOR == loginName))
                    .ToList();

                var columnInfos = new List<ColumnInfo>
                {
                    new ColumnInfo("SERE_SERV_PTTT_TEMP_CODE", "Mã mẫu", 120, 1),
                    new ColumnInfo("SERE_SERV_PTTT_TEMP_NAME", "Tên mẫu", 250, 2),
                };
                var controlEditorADO = new ControlEditorADO(
                    "SERE_SERV_PTTT_TEMP_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPtttTemp_v45072, currentPtttTemps_v45072, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPtttTemp_v45072_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPtttTemp_v45072 == null || cboPtttTemp_v45072.EditValue == null) return;
                long id = 0;
                if (!long.TryParse(cboPtttTemp_v45072.EditValue.ToString(), out id) || id <= 0) return;

                var temp = (currentPtttTemps_v45072 != null
                            ? currentPtttTemps_v45072.FirstOrDefault(o => o.ID == id)
                            : null)
                           ?? BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>().FirstOrDefault(o => o.ID == id);
                if (temp == null) return;

                if (cboEmotionLess_v45072 != null) cboEmotionLess_v45072.EditValue = temp.EMOTIONLESS_METHOD_ID;
                if (txtManner_v45072 != null) txtManner_v45072.Text = temp.MANNER ?? "";
                if (txtConclude_v45072 != null) txtConclude_v45072.Text = temp.CONCLUDE ?? "";
                if (txtDescription_v45072 != null) txtDescription_v45072.Text = temp.DESCRIPTION ?? "";
                if (txtNote_v45072 != null) txtNote_v45072.Text = temp.NOTE ?? "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_SERE_SERV_PTTT_TEMP GetDataForTemp_v45072()
        {
            try
            {
                bool allEmpty = true;
                if (cboEmotionLess_v45072 != null && cboEmotionLess_v45072.EditValue != null) allEmpty = false;
                if (txtManner_v45072 != null && !string.IsNullOrWhiteSpace(txtManner_v45072.Text)) allEmpty = false;
                if (txtConclude_v45072 != null && !string.IsNullOrWhiteSpace(txtConclude_v45072.Text)) allEmpty = false;
                if (txtDescription_v45072 != null && !string.IsNullOrWhiteSpace(txtDescription_v45072.Text)) allEmpty = false;
                if (txtNote_v45072 != null && !string.IsNullOrWhiteSpace(txtNote_v45072.Text)) allEmpty = false;
                if (allEmpty) return null;

                var data = new HIS_SERE_SERV_PTTT_TEMP();
                if (cboEmotionLess_v45072 != null && cboEmotionLess_v45072.EditValue != null)
                {
                    long emoId;
                    if (long.TryParse(cboEmotionLess_v45072.EditValue.ToString(), out emoId))
                        data.EMOTIONLESS_METHOD_ID = emoId;
                }
                data.MANNER = txtManner_v45072 != null ? (txtManner_v45072.Text ?? "").Trim() : "";
                data.CONCLUDE = txtConclude_v45072 != null ? (txtConclude_v45072.Text ?? "").Trim() : "";
                data.DESCRIPTION = txtDescription_v45072 != null ? (txtDescription_v45072.Text ?? "").Trim() : "";
                data.NOTE = txtNote_v45072 != null ? (txtNote_v45072.Text ?? "").Trim() : "";
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void BtnSavePtttTemp_v45072_Click(object sender, EventArgs e)
        {
            try
            {
                var dataTemp = GetDataForTemp_v45072();
                if (dataTemp == null)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoNoiDungLuuMau,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool opened = OpenFormPtttTempByReflection_v45072(dataTemp);
                if (!opened)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChucNangLuuMauChuaKhaDung,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                BackendDataWorker.Reset<HIS_SERE_SERV_PTTT_TEMP>();
                LoadDataToComboPtttTemp_v45072();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool OpenFormPtttTempByReflection_v45072(HIS_SERE_SERV_PTTT_TEMP dataTemp)
        {
            try
            {
                var asm = System.Reflection.Assembly.Load(ModuleLinkString.SurgServiceReqExecute);
                if (asm == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Assembly.Load tra ve null cho " + ModuleLinkString.SurgServiceReqExecute);
                    return false;
                }
                var t = asm.GetType("HIS.Desktop.Plugins.SurgServiceReqExecute.PtttTemp.FormPtttTemp");
                if (t == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong tim thay type FormPtttTemp trong assembly " + ModuleLinkString.SurgServiceReqExecute);
                    return false;
                }
                var frm = Activator.CreateInstance(t, new object[] { this.moduleData, dataTemp }) as System.Windows.Forms.Form;
                if (frm == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong khoi tao duoc instance FormPtttTemp.");
                    return false;
                }
                frm.ShowDialog();
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
    }
}
