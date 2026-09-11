/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// Dai nut duoi cung, chep tu man hinh "Xu ly yeu cau kham/cls/pttt"
    /// (HIS.Desktop.Plugins.ExecuteRoom\UCExecuteRoom.cs).
    ///
    /// Cac nut deu lay y lenh (L_HIS_SERVICE_REQ) qua lop cau noi
    /// UCPaanExecuteList___Bridge.cs, vi luoi cua man nay o muc DICH VU.
    /// </summary>
    public partial class UCPaanExecuteList
    {
        #region Bien phuc vu nhom goi benh nhan

        /// <summary>Danh sach id y lenh vua duoc goi, de lam moi lai luoi.</summary>
        private List<long> CheckListCPA = new List<long>();

        /// <summary>Ket noi toi phan mem goi so CPA. Khoi tao khi can dung.</summary>
        private CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;

        #endregion

        #region Nhom thao tac y lenh

        /// <summary>
        /// "Huy bat dau" - dua y lenh tu Dang xu ly ve Chua xu ly.
        /// Chep tu UCExecuteRoom.cs:1960 (btnUnStart_Click_Action).
        /// Bo phan LoadServiceReqCount / LoadSereServCount vi da co ham rieng
        /// o man nay, goi lai o cuoi.
        /// </summary>
        private void btnUnStart_Click(object sender, EventArgs e)
        {
            try
            {
                L_HIS_SERVICE_REQ serviceReq = GetServiceReqOfFocusedRow(true);
                if (serviceReq == null) return;

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();

                var result = new BackendAdapter(param)
                    .Post<L_HIS_SERVICE_REQ>(
                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_UNSTART,
                        ApiConsumers.MosConsumer, serviceReq.ID, param);

                if (result != null && result.ID > 0)
                {
                    success = true;
                    btnUnStart.Enabled = false;
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                if (success)
                {
                    // Trang thai y lenh da doi -> nap lai ca luoi lan so dem.
                    FillDataToGridControl();
                    LoadServiceReqCount();
                    LoadSereServCount();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Danh sach y lenh".
        /// Chep tu UCExecuteRoom.cs:2069 (btnServiceReqList_Click).
        /// </summary>
        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                L_HIS_SERVICE_REQ serviceReq = GetServiceReqOfFocusedRow(true);
                if (serviceReq == null) return;

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList")
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                    return;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = serviceReq.TREATMENT_ID;
                    listArgs.Add(treatment);
                    listArgs.Add(serviceReq.TREATMENT_ID);

                    var extenceInstance = PluginInstance.GetPluginInstance(
                        PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()),
                        listArgs);

                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Lich su dieu tri".
        /// Chep tu UCExecuteRoom.cs:1813 (btnTreatmentHistory_Click).
        /// </summary>
        private void btnTreatmentHistory_Click(object sender, EventArgs e)
        {
            try
            {
                L_HIS_SERVICE_REQ serviceReq = GetServiceReqOfFocusedRow(true);
                if (serviceReq == null) return;

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory")
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                    return;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = serviceReq.TREATMENT_ID;

                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<V_HIS_TREATMENT>>(
                            HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW,
                            ApiConsumers.MosConsumer, treatmentFilter, new CommonParam())
                        .FirstOrDefault();

                    if (treatment != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.patientId = treatment.PATIENT_ID;
                        treatmentHistory.patient_code = treatment.TDL_PATIENT_CODE;
                        listArgs.Add(treatmentHistory);

                        var extenceInstance = PluginInstance.GetPluginInstance(
                            PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()),
                            listArgs);

                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Chuyen phong".
        /// Chep tu UCExecuteRoom.cs:1710 (btnRoomTran_Click).
        ///
        /// LUU Y NGHIEP VU: chuc nang nay von la thao tac cua phong hien tai.
        /// O man danh sach toan vien, benh nhan tren luoi co the thuoc phong khac.
        /// Van giu theo dung yeu cau "giong man cu".
        /// </summary>
        private void btnRoomTran_Click(object sender, EventArgs e)
        {
            try
            {
                L_HIS_SERVICE_REQ serviceReq = GetServiceReqOfFocusedRow(true);
                if (serviceReq == null) return;

                // Neu y lenh dang xu ly thi phai huy bat dau truoc.
                if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    DialogResult myResult = MessageBox.Show(
                        "Bạn có muốn hủy bắt đầu không?", "Xác nhận hủy bắt đầu",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (myResult == DialogResult.Yes)
                    {
                        if (!UnStartEvent(serviceReq)) return;
                    }
                    else
                    {
                        return;
                    }
                }

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess")
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                    return;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq);

                    var extenceInstance = PluginInstance.GetPluginInstance(
                        PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()),
                        listArgs);

                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();

                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Bang ke (F5)".
        /// Chep tu UCExecuteRoom.cs:1634 (btnBordereau_Click).
        /// Nut nay gan RoomId truc tiep vao moduleData thay vi dung
        /// GetModuleWithWorkingRoom - giu nguyen nhu ban goc.
        /// </summary>
        private void btnBordereau_Click(object sender, EventArgs e)
        {
            try
            {
                L_HIS_SERVICE_REQ serviceReq = GetServiceReqOfFocusedRow(true);
                if (serviceReq == null) return;

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau")
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                    return;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    moduleData.RoomId = GetRoomId();
                    moduleData.RoomTypeId = GetRoomTypeId();
                    listArgs.Add(serviceReq.TREATMENT_ID);

                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Goi API huy bat dau. Chep tu UCExecuteRoom.cs:2027 (UnStartEvent).
        /// </summary>
        private bool UnStartEvent(L_HIS_SERVICE_REQ serviceReqInput)
        {
            bool result = false;
            try
            {
                if (serviceReqInput == null) return false;

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();

                var serviceReq = new BackendAdapter(param)
                    .Post<L_HIS_SERVICE_REQ>(
                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_UNSTART,
                        ApiConsumers.MosConsumer, serviceReqInput.ID, param);

                if (serviceReq != null && serviceReq.ID > 0)
                {
                    result = true;
                    success = true;
                    serviceReqInput.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #endregion

        #region Nhom goi benh nhan

        /// <summary>
        /// "Goi (F6)".
        /// Chep tu UCExecuteRoom.cs:2608 (btnCallPatient_Click_Action),
        /// NHUNG CHI GIU NHANH else.
        ///
        /// LY DO BO NHANH chkCPA:
        ///   Nhanh do dung  lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == roomId)
        ///   (UCExecuteRoom.cs:2634) roi truyen thang vao CallPatientChkCPA de doc
        ///   EXECUTE_ROOM_NAME / ADDRESS khi phat loa.
        ///   Man nay lay y lenh TOAN VIEN nen benh nhan tren luoi co the thuoc
        ///   phong khac -> executeRoom sai hoac null -> NullReferenceException.
        ///   Nhanh else chi dung txtGateNumber va txtStepNumber (so cong CPA),
        ///   khong lien quan phong, nen chay dung o moi phong.
        /// </summary>
        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                CallPatient(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "Goi lai (F7)".
        /// Chep tu UCExecuteRoom.cs:2673 (btnRecallPatient_Click_Action).
        /// </summary>
        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCallPatient.Enabled) return;

                string configKeyCallPatientCPA = GetConfigCallPatientCPA();
                if (configKeyCallPatientCPA != "1") return;

                EnsureCallPatientClient();
                if (this.clienttManager == null) return;
                if (String.IsNullOrWhiteSpace(txtGateNumber.Text) || String.IsNullOrWhiteSpace(txtStepNumber.Text)) return;

                this.clienttManager.RecallNumOrderString(
                    txtGateNumber.Text.Trim(), int.Parse(txtStepNumber.Text));

                long[] id = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), false);
                Inventec.Common.Logging.LogSystem.Info(
                    "GetCurrentPatientCall ____Goi lai F7: " + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));

                MarkCalledPatient(id, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "Goi BN nho".
        /// Chep tu UCExecuteRoom.cs:4403 (btnMissCall_Click_Action).
        /// Da kiem: ham goc KHONG dung roomId lan nao nen chay dung o man toan vien.
        /// </summary>
        private void btnMissCall_Click(object sender, EventArgs e)
        {
            try
            {
                CallPatient(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "BN khong vao".
        /// Chep tu UCExecuteRoom.cs:4470 (btnNotEnter_Click_Action).
        ///
        /// DA SUA MOT LOI CO SAN o ban goc: dong 4485-4487 lay
        /// data = ...FirstOrDefault(...) roi truy cap data.TDL_PATIENT_DOB ma
        /// KHONG kiem tra null -> NullReferenceException khi khong tim thay dong.
        /// O day da them kiem tra null.
        /// </summary>
        private void btnNotEnter_Click(object sender, EventArgs e)
        {
            try
            {
                string configKeyCallPatientCPA = GetConfigCallPatientCPA();
                if (configKeyCallPatientCPA != "1") return;

                EnsureCallPatientClient();
                if (this.clienttManager == null) return;
                if (String.IsNullOrWhiteSpace(txtGateNumber.Text)) return;

                long[] id = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), false);
                if (id == null || id.Length == 0) return;

                foreach (var item in id)
                {
                    PaanSereServADO data = (currentData != null)
                        ? currentData.FirstOrDefault(o => o.SERVICE_REQ_ID.HasValue && o.SERVICE_REQ_ID.Value == item)
                        : null;

                    // Ban goc thieu doan kiem tra null nay.
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "BN khong vao: khong tim thay dong tren luoi co SERVICE_REQ_ID = " + item);
                        continue;
                    }

                    CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO info =
                        new CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO();
                    // Ten thuoc tinh dung la ServiceReqId (khong phai Id).
                    info.ServiceReqId = item;
                    info.PatientName = data.TDL_PATIENT_NAME;
                    info.Dob = data.TDL_PATIENT_DOB;

                    this.clienttManager.UpdatePatientToMissingCall(txtGateNumber.Text.Trim(), info);
                }

                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Than chung cho "Goi (F6)" va "Goi BN nho".
        /// isMissCall = true  -> lay danh sach benh nhan NHO.
        /// isMissCall = false -> goi so tiep theo.
        /// </summary>
        private void CallPatient(bool isMissCall)
        {
            try
            {
                string configKeyCallPatientCPA = GetConfigCallPatientCPA();
                if (configKeyCallPatientCPA != "1") return;

                EnsureCallPatientClient();
                if (this.clienttManager == null) return;

                if (String.IsNullOrWhiteSpace(txtGateNumber.Text)) return;

                if (!isMissCall)
                {
                    if (String.IsNullOrWhiteSpace(txtStepNumber.Text)) return;
                    this.clienttManager.CallNumOrderString(
                        txtGateNumber.Text.Trim(), int.Parse(txtStepNumber.Text));
                }

                long[] id = this.clienttManager.GetCurrentPatientCall(
                    txtGateNumber.Text.Trim(), isMissCall);

                Inventec.Common.Logging.LogSystem.Info(
                    (isMissCall ? "Goi BN nho: " : "Goi F6: ")
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));

                MarkCalledPatient(id, isMissCall);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Danh dau cac y lenh vua goi va bao cho backend.
        /// Gop tu CallPatient / CallPatientCountService cua ban goc.
        /// </summary>
        private void MarkCalledPatient(long[] id, bool isMissCall)
        {
            try
            {
                if (id == null || id.Length == 0) return;

                txtCallPatientCPA.ForeColor = isMissCall ? Color.Red : Color.Black;

                CommonParam param = new CommonParam();
                foreach (var item in id)
                {
                    try
                    {
                        new BackendAdapter(param).Post<L_HIS_SERVICE_REQ>(
                            HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_CALL,
                            ApiConsumers.MosConsumer, item, param);
                    }
                    catch (Exception exItem)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exItem);
                    }
                }

                CheckListCPA = id.ToList();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Khoi tao ket noi CPA neu chua co.</summary>
        private void EnsureCallPatientClient()
        {
            try
            {
                if (this.clienttManager == null)
                {
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                }
            }
            catch (Exception ex)
            {
                this.clienttManager = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Doc cau hinh co dung phan mem goi so CPA hay khong.</summary>
        private string GetConfigCallPatientCPA()
        {
            try
            {
                return HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker
                    .Get<string>("HIS.Desktop.Plugins.DangKyTiepDon.GoiBenhNhanBangPhanMemCPA");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        #endregion

        #region Tien ich chung

        /// <summary>
        /// Can lai nhom nut ben phai moi khi dai nut doi kich thuoc.
        ///
        /// LY DO PHAI TU CAN: dat toa do co dinh roi neo Anchor = Right thi khi
        /// cua so rong/hep khac 1100px, nut cuoi ("Bang ke") bi day ra ngoai va
        /// mat chu. Tu tinh lai tu mep phai thi luon vua, du man hinh rong bao nhieu.
        /// </summary>
        private void panelBottom_Resize(object sender, EventArgs e)
        {
            try
            {
                LayoutRightButtons();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xep 6 nut nhom phai sat mep phai, tu phai sang trai.
        /// </summary>
        private void LayoutRightButtons()
        {
            try
            {
                if (panelBottom == null || btnBordereau == null) return;

                const int MARGIN_RIGHT = 6;   // chua le phai
                const int GAP = 4;            // khoang cach giua cac nut
                const int TOP = 5;

                // Thu tu tu PHAI sang TRAI.
                DevExpress.XtraEditors.SimpleButton[] rightGroup = new DevExpress.XtraEditors.SimpleButton[]
                {
                    btnBordereau,
                    btnRoomTran,
                    btnTreatmentHistory,
                    btnServiceReqList,
                    btnProcess,
                    btnUnStart
                };

                int x = panelBottom.Width - MARGIN_RIGHT;

                foreach (var btn in rightGroup)
                {
                    if (btn == null) continue;
                    x = x - btn.Width;
                    btn.Location = new System.Drawing.Point(x, TOP);
                    x = x - GAP;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Id phong lam viec hien tai.</summary>
        private long GetRoomId()
        {
            try
            {
                return currentModule != null ? currentModule.RoomId : 0;
            }
            catch { return 0; }
        }

        /// <summary>Loai phong lam viec hien tai.</summary>
        private long GetRoomTypeId()
        {
            try
            {
                return currentModule != null ? currentModule.RoomTypeId : 0;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Bat/tat cac nut theo dong dang chon.
        /// Chep tu UCExecuteRoom___Load.cs:3181 (InitEnableControl).
        /// </summary>
        private void InitEnableControl()
        {
            try
            {
                PaanSereServADO row = GetFocusedRow();

                if (row == null)
                {
                    btnProcess.Enabled = false;
                    btnBordereau.Enabled = false;
                    btnRoomTran.Enabled = false;
                    btnTreatmentHistory.Enabled = false;
                    btnUnStart.Enabled = false;
                    btnServiceReqList.Enabled = false;
                }
                else
                {
                    // Chi cho Huy bat dau khi y lenh DANG XU LY.
                    btnUnStart.Enabled =
                        (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);

                    btnProcess.Enabled = true;
                    btnBordereau.Enabled = true;
                    btnRoomTran.Enabled = true;
                    btnTreatmentHistory.Enabled = true;
                    btnServiceReqList.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Doi dong chon tren luoi -> cap nhat trang thai nut.</summary>
        private void gridViewPaan_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                InitEnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
