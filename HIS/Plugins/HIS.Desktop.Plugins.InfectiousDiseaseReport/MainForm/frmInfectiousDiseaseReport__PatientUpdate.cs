/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Cập nhật thông tin bệnh nhân: mở chức năng HIS.Desktop.Plugins.PatientUpdate cho bệnh nhân của điều trị hiện tại.
 * Tham khảo HIS.Desktop.Plugins.EnterKskInfomantionVer2 (btnPatientUpdate_Click).
 * Đóng form -> nạp lại các trường lấy từ thông tin bệnh nhân trên form (họ tên/ngày sinh/giới tính/CCCD/SĐT/...).
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        /// <summary>ModuleLink của chức năng cập nhật thông tin bệnh nhân.</summary>
        private const string PatientUpdateModuleLink = "HIS.Desktop.Plugins.PatientUpdate";

        /// <summary>
        /// Mở chức năng cập nhật thông tin bệnh nhân (PatientUpdate) cho bệnh nhân của điều trị hiện tại.
        /// Args: [long PatientId, long TreatmentId, DelegateSelectData callback] — đúng thứ tự PatientUpdateBehavior parse.
        /// ShowModule dùng ShowDialog (chặn) -> sau khi đóng form sẽ nạp lại thông tin bệnh nhân.
        /// </summary>
        private void btnPatientUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (treatment == null || treatment.PATIENT_ID <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("PatientUpdate: treatment/PATIENT_ID khong hop le.");
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(treatment.PATIENT_ID);   // long #1 -> PatientId
                listArgs.Add(treatment.ID);           // long #2 -> TreatmentId
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloadPatientData);

                long roomId = this.moduleData != null ? this.moduleData.RoomId : 0;
                long roomTypeId = this.moduleData != null ? this.moduleData.RoomTypeId : 0;

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    PatientUpdateModuleLink, roomId, roomTypeId, listArgs);

                // Đảm bảo nạp lại sau khi đóng form (kể cả khi người dùng không bấm lưu -> callback không chạy).
                ReloadPatientData(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Callback sau khi cập nhật/đóng form bệnh nhân: đọc lại V_HIS_PATIENT, đồng bộ các trường
        /// denormalized trên treatment (tên/ngày sinh/giới tính) rồi nạp lại toàn bộ thông tin hành chính trên form.
        /// </summary>
        private void ReloadPatientData(object data)
        {
            try
            {
                if (treatment == null || treatment.PATIENT_ID <= 0) return;

                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisPatientViewFilter { ID = treatment.PATIENT_ID };
                var list = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(
                    HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                var p = (list != null && list.Count > 0) ? list[0] : null;
                if (p != null)
                {
                    // Đồng bộ để FillHanhChinhTab (lấy tên/ngày sinh/giới tính từ treatment) hiển thị giá trị mới.
                    treatment.TDL_PATIENT_NAME = p.VIR_PATIENT_NAME;
                    treatment.TDL_PATIENT_DOB = p.DOB;
                    treatment.TDL_PATIENT_GENDER_ID = p.GENDER_ID;
                    this.patient = p;
                }

                // Nạp lại họ tên/ngày sinh/tuổi/giới tính + hành chính (CCCD/SĐT/dân tộc/nghề/địa chỉ).
                FillHanhChinhTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
