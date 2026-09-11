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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// Xu ly khi bam tung muc tren menu chuot phai.
    ///
    /// VIET LAI gon tu HIS.Desktop.Plugins.ExecuteRoom\UCExecuteRoom___Popup_Menu_Showing.cs
    /// (1203 dong). Ban goc moi muc lap lai nguyen mot khoi 20 dong giong nhau
    /// de mo plugin; o day gom vao ham OpenPlugin dung chung.
    /// </summary>
    public partial class UCPaanExecuteList
    {
        #region Bien phuc vu menu

        private DevExpress.XtraBars.BarManager barManager1 = null;
        private PaanPopupMenuProcessor paanPopupMenuProcessor = null;

        #endregion

        /// <summary>
        /// Dung menu khi chuot phai vao mot dong tren luoi.
        /// Chep tu UCExecuteRoom.cs:1902.
        /// Khac ban goc: dong luoi la PaanSereServADO (muc dich vu) nen phai lay
        /// y lenh L_HIS_SERVICE_REQ qua lop cau noi truoc.
        /// </summary>
        private void gridViewPaan_PopupMenuShowing(object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = e.HitInfo;
                if (!hi.InRowCell) return;

                int rowHandle = gridViewPaan.GetVisibleRowHandle(hi.RowHandle);
                PaanSereServADO row = gridViewPaan.GetRow(rowHandle) as PaanSereServADO;
                if (row == null || !row.SERVICE_REQ_ID.HasValue) return;

                this.currentServiceReq = GetServiceReqById(row.SERVICE_REQ_ID.Value, false);
                if (this.currentServiceReq == null) return;

                gridViewPaan.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewPaan.OptionsSelection.EnableAppearanceFocusedRow = true;

                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this.ParentForm != null
                        ? this.ParentForm
                        : this.FindForm();

                    // Gan bo icon. Thieu dong nay thi menu hien KHONG CO icon.
                    // Man cu lam y het: UCExecuteRoom.Designer.cs
                    // "this.barManager1.Images = this.imageCollection2;"
                    barManager1.Images = this.imageCollectionMenu;
                }

                paanPopupMenuProcessor = new PaanPopupMenuProcessor(
                    this.currentServiceReq, PaanMenuItemClick, barManager1);

                paanPopupMenuProcessor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dieu huong khi bam mot muc tren menu.</summary>
        private void PaanMenuItemClick(PaanMenuType type)
        {
            try
            {
                L_HIS_SERVICE_REQ sr = this.currentServiceReq;
                if (sr == null) return;

                switch (type)
                {
                    // --- Cac muc dung chung nut o dai duoi ---
                    case PaanMenuType.Execute:
                        btnProcess_Click(null, null);
                        break;

                    case PaanMenuType.UnStart:
                        btnUnStart_Click(null, null);
                        break;

                    case PaanMenuType.RoomTran:
                        btnRoomTran_Click(null, null);
                        break;

                    case PaanMenuType.Bordereau:
                        btnBordereau_Click(null, null);
                        break;

                    case PaanMenuType.ServiceReqList:
                        btnServiceReqList_Click(null, null);
                        break;

                    case PaanMenuType.TreatmentHistory:
                        btnTreatmentHistory_Click(null, null);
                        break;

                    // --- Huy ket thuc ---
                    case PaanMenuType.UnExecute:
                        CancelFinish(sr);
                        break;

                    // --- Cac muc chi truyen TREATMENT_ID ---
                    case PaanMenuType.AggrHospitalFees:
                        OpenPlugin("HIS.Desktop.Plugins.AggrHospitalFees",
                            new List<object> { sr.TREATMENT_ID }, false);
                        break;

                    case PaanMenuType.OtherForms:
                        OpenPlugin("HIS.Desktop.Plugins.OtherForms",
                            new List<object> { sr.TREATMENT_ID }, true);
                        break;

                    case PaanMenuType.Debate:
                        OpenPlugin("HIS.Desktop.Plugins.Debate",
                            new List<object> { sr.TREATMENT_ID }, true);
                        break;

                    case PaanMenuType.AssignPaan:
                        OpenPlugin("HIS.Desktop.Plugins.AssignPaan",
                            new List<object> { sr.TREATMENT_ID }, false);
                        break;

                    case PaanMenuType.AllergyCard:
                        OpenPlugin("HIS.Desktop.Plugins.AllergyCard",
                            new List<object> { sr.TREATMENT_ID }, false);
                        break;

                    case PaanMenuType.ThongTinChuyenDen:
                        OpenPlugin("HIS.Desktop.Plugins.HisTranPatiToInfo",
                            new List<object> { sr.TREATMENT_ID }, false);
                        break;

                    // --- Cac muc co tham so rieng ---
                    case PaanMenuType.DetailMedicalRecord:
                        OpenPlugin("HIS.Desktop.Plugins.EmrDocument",
                            new List<object> { sr.TDL_TREATMENT_CODE }, false);
                        break;

                    case PaanMenuType.SummaryInforTreatmentRecords:
                        OpenPlugin("HIS.Desktop.Plugins.SummaryInforTreatmentRecords",
                            new List<object> { false, sr.TREATMENT_ID }, false);
                        break;

                    case PaanMenuType.SuaYeuCauKham:
                        OpenPlugin("HIS.Desktop.Plugins.UpdateExamServiceReq",
                            new List<object> { sr.ID, true }, false);
                        break;

                    case PaanMenuType.TreatmentList:
                        OpenPlugin("HIS.Desktop.Plugins.TreatmentList",
                            new List<object> { sr.TDL_TREATMENT_CODE, sr }, false);
                        break;

                    case PaanMenuType.BenhAnNgoaiTru:
                        MessageBox.Show(
                            "Chức năng in bệnh án ngoại trú chưa được hỗ trợ trên màn hình này.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    // --- Cac muc can them Module lam tham so ---
                    case PaanMenuType.PhanLoaiBenhNhan:
                        OpenPhanLoaiBenhNhan(sr);
                        break;

                    case PaanMenuType.HivTreatment:
                        OpenWithTreatment("HIS.Desktop.Plugins.HisHivTreatment", sr);
                        break;

                    case PaanMenuType.TuberclusisTreatment:
                        OpenPluginWithModule("HIS.Desktop.Plugins.HisTuberclusisTreatment",
                            new List<object> { sr.TREATMENT_ID });
                        break;

                    case PaanMenuType.Khamsuckhoe:
                        OpenKhamSucKhoe(sr);
                        break;

                    case PaanMenuType.MoiHoiChan:
                        OpenMoiHoiChan(sr);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Ham dung chung mo plugin

        /// <summary>
        /// Mo mot plugin. Gom lai tu ~20 khoi lap giong nhau o ban goc.
        /// </summary>
        private object OpenPlugin(string moduleLink, List<object> listArgs, bool isDialog)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == moduleLink)
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLink);
                    MessageBox.Show("Không tìm thấy chức năng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "Module khong phai plugin hoac thieu ExtensionInfo: " + moduleLink);
                    return null;
                }

                var instance = PluginInstance.GetPluginInstance(
                    PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()),
                    listArgs);

                if (instance == null) throw new ArgumentNullException("instance is null");

                if (isDialog) ((Form)instance).ShowDialog();
                else ((Form)instance).Show();

                return instance;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Mo plugin can nhan them chinh doi tuong Module lam tham so cuoi.
        /// </summary>
        private void OpenPluginWithModule(string moduleLink, List<object> listArgs)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == moduleLink)
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLink);
                    return;
                }

                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) return;

                var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                    moduleData, GetRoomId(), GetRoomTypeId());

                listArgs.Add(moduleWithRoom);

                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                if (instance == null) throw new ArgumentNullException("instance is null");

                ((Form)instance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Cac muc can xu ly rieng

        /// <summary>
        /// "Huy ket thuc". Rut gon tu UCExecuteRoom___Process.cs:59 (CancelFinish),
        /// bo phan tu dong xoa van ban ky EMR (phu thuoc cau hinh rieng man cu).
        /// </summary>
        private void CancelFinish(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput == null) return;

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();

                var result = new BackendAdapter(param).Post<L_HIS_SERVICE_REQ>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_UNFINISH,
                    ApiConsumers.MosConsumer, serviceReqInput.ID, param);

                if (result != null) success = true;

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                if (success)
                {
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
        /// "Phan loai benh nhan". Chep tu Popup_Menu_Showing.cs:412.
        /// </summary>
        private void OpenPhanLoaiBenhNhan(L_HIS_SERVICE_REQ sr)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.UpdatePatientClassify")
                        .FirstOrDefault();

                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.UpdatePatientClassify");
                    return;
                }

                L_HIS_TREATMENT_BED_ROOM data = new L_HIS_TREATMENT_BED_ROOM();
                data.TDL_PATIENT_CLASSIFY_ID = sr.TDL_PATIENT_CLASSIFY_ID;
                data.PATIENT_ID = sr.TDL_PATIENT_ID;

                var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                    moduleData, GetRoomId(), GetRoomTypeId());

                List<object> listArgs = new List<object>();
                listArgs.Add(data);
                listArgs.Add(moduleWithRoom);

                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                if (instance == null) return;

                ((Form)instance).ShowDialog();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mo plugin can doi tuong HIS_TREATMENT day du (HIV/AIDS).
        /// Chep tu Popup_Menu_Showing.cs:447.
        /// </summary>
        private void OpenWithTreatment(string moduleLink, L_HIS_SERVICE_REQ sr)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = sr.TREATMENT_ID;

                var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                if (treatment == null || treatment.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong lay duoc HIS_TREATMENT ID = " + sr.TREATMENT_ID);
                    return;
                }

                OpenPluginWithModule(moduleLink, new List<object> { treatment.FirstOrDefault() });
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Thong tin kham ho so suc khoe". Chep tu Popup_Menu_Showing.cs:506.
        /// Plugin nay nhan V_HIS_SERVICE_REQ chu khong phai L_HIS_SERVICE_REQ.
        /// </summary>
        private void OpenKhamSucKhoe(L_HIS_SERVICE_REQ sr)
        {
            try
            {
                V_HIS_SERVICE_REQ vServiceReq = GetVServiceReqById(sr.ID);
                if (vServiceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong lay duoc V_HIS_SERVICE_REQ ID = " + sr.ID);
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EnterKskInfomantionVer2")
                        .FirstOrDefault();

                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.EnterKskInfomantionVer2");
                    return;
                }

                var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                    moduleData, GetRoomId(), GetRoomTypeId());

                List<object> listArgs = new List<object>();
                listArgs.Add(vServiceReq);
                listArgs.Add(moduleWithRoom);

                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                if (instance == null) return;

                ((Form)instance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Moi hoi chan". Chep tu Popup_Menu_Showing.cs:207.
        /// Plugin nhan (Module, false, V_HIS_SERVICE_REQ).
        /// </summary>
        private void OpenMoiHoiChan(L_HIS_SERVICE_REQ sr)
        {
            try
            {
                V_HIS_SERVICE_REQ vServiceReq = GetVServiceReqById(sr.ID);
                if (vServiceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "MoiHoiChanClick: V_HIS_SERVICE_REQ null, ID = " + sr.ID);
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InviteConsultation")
                        .FirstOrDefault();

                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "khong tim thay moduleLink = HIS.Desktop.Plugins.InviteConsultation");
                    return;
                }

                var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                    moduleData, GetRoomId(), GetRoomTypeId());

                List<object> listArgs = new List<object>();
                listArgs.Add(moduleWithRoom);
                listArgs.Add(false);
                listArgs.Add(vServiceReq);

                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                if (instance == null) return;

                ((Form)instance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
