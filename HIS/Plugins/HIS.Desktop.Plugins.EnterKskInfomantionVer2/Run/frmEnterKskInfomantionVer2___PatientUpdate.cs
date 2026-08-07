/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Nút (icon person) cạnh nút Thư viện văn bản ở "Lý do khám" — mở chức năng Cập nhật thông tin
 * bệnh nhân (HIS.Desktop.Plugins.PatientUpdate). Khi tắt form đó -> nạp lại V_HIS_SERVICE_REQ
 * (view kéo lại TDL_PATIENT_* mới nhất từ HIS_PATIENT) rồi hiển thị lại vùng thông tin bệnh nhân.
 * (Tương tự HIS.Desktop.Plugins.EnterKskInfomantionQD831.)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private void btnPatientUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentServiceReq == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PatientUpdate");
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(currentServiceReq.TDL_PATIENT_ID);
                listArgs.Add(currentServiceReq.TREATMENT_ID);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloadPatientData);

                object instance;
                if (this.currentModule != null)
                    instance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                        HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                else
                    instance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);

                var frm = instance as System.Windows.Forms.Form;
                if (frm != null)
                {
                    frm.ShowDialog(this);
                    // Khi tắt form PatientUpdate -> load lại thông tin bệnh nhân.
                    ReloadPatientData(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nạp lại thông tin bệnh nhân sau khi cập nhật: đọc THẲNG V_HIS_PATIENT theo TDL_PATIENT_ID
        /// (dữ liệu mới nhất từ HIS_PATIENT — vì V_HIS_SERVICE_REQ có thể lưu TDL_PATIENT_* denormalized
        /// không đổi ngay) -> cập nhật lại các trường TDL_PATIENT_* trên currentServiceReq nếu có -> hiển thị lại.
        /// </summary>
        private void ReloadPatientData(object data)
        {
            try
            {
                if (currentServiceReq == null || currentServiceReq.TDL_PATIENT_ID <= 0) return;
                CommonParam param = new CommonParam();
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = currentServiceReq.TDL_PATIENT_ID;
                var list = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(
                    "api/HisPatient/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                var pat = (list != null) ? list.FirstOrDefault() : null;
                if (pat != null)
                {
                    // Cập nhật các trường hiển thị ở vùng thông tin bệnh nhân từ HIS_PATIENT (V_HIS_PATIENT).
                    currentServiceReq.TDL_PATIENT_CODE = pat.PATIENT_CODE;
                    currentServiceReq.TDL_PATIENT_NAME = pat.VIR_PATIENT_NAME;
                    currentServiceReq.TDL_PATIENT_CCCD_NUMBER = pat.CCCD_NUMBER;
                    currentServiceReq.TDL_PATIENT_GENDER_NAME = pat.GENDER_NAME;
                    currentServiceReq.TDL_PATIENT_DOB = pat.DOB;
                    currentServiceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = pat.IS_HAS_NOT_DAY_DOB;
                    currentServiceReq.TDL_PATIENT_AVATAR_URL = pat.AVATAR_URL;
                    // Nhóm máu: lấy luôn từ chính kết quả GetView này (không gọi GetView thêm lần nữa).
                    if (lblBlood != null)
                    {
                        string blood = (pat.BLOOD_ABO_CODE ?? "").Trim();
                        if (!string.IsNullOrWhiteSpace(pat.BLOOD_RH_CODE))
                            blood = (blood + " " + pat.BLOOD_RH_CODE.Trim()).Trim();
                        lblBlood.Text = blood;
                    }
                }
                ShowInformationPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
