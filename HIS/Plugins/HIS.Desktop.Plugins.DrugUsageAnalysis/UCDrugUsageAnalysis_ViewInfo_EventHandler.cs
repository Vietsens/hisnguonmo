using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private const string MODULE_LINK__SERVICE_REQ_LIST = "HIS.Desktop.Plugins.ServiceReqList";
        private const string MODULE_LINK__HIS_TRACKING_LIST = "HIS.Desktop.Plugins.HisTrackingList";
        private const string MODULE_LINK__SUMARY_TEST_RESULTS = "HIS.Desktop.Plugins.SumaryTestResults";

        /// <summary>
        /// Lay benh nhan dang duoc chon tren luoi danh sach. Tra ve null neu chua chon benh nhan nao
        /// (co canh bao cho nguoi dung).
        /// </summary>
        private L_HIS_TREATMENT_BED_ROOM GetSelectedTreatmentBedRoom()
        {
            L_HIS_TREATMENT_BED_ROOM result = null;
            try
            {
                result = gridView1.GetFocusedRow() as L_HIS_TREATMENT_BED_ROOM;
                if (result == null || result.TREATMENT_ID <= 0)
                {
                    result = null;
                    MessageBox.Show(
                        Inventec.Common.Resource.Get.Value("UCDrugUsageAnalysis.MsgChuaChonBenhNhan", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void btnDsYlenh_Click(object sender, EventArgs e)
        {
            try
            {
                var treatmentBedRoom = GetSelectedTreatmentBedRoom();
                if (treatmentBedRoom == null) return;

                List<object> listArgs = new List<object>();
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                treatment.ID = treatmentBedRoom.TREATMENT_ID;
                treatment.TREATMENT_CODE = treatmentBedRoom.TREATMENT_CODE;
                listArgs.Add(treatment);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(MODULE_LINK__SERVICE_REQ_LIST, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnToDieuTri_Click(object sender, EventArgs e)
        {
            try
            {
                var treatmentBedRoom = GetSelectedTreatmentBedRoom();
                if (treatmentBedRoom == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentBedRoom.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(MODULE_LINK__HIS_TRACKING_LIST, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSumaryTestResults_Click(object sender, EventArgs e)
        {
            try
            {
                var treatmentBedRoom = GetSelectedTreatmentBedRoom();
                if (treatmentBedRoom == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentBedRoom.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(MODULE_LINK__SUMARY_TEST_RESULTS, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
