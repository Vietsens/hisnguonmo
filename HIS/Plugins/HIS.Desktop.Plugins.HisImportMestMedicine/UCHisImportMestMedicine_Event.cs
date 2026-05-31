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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisImportMestMedicine.Base;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    public partial class UCHisImportMestMedicine : UserControlBase
    {
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                //hien thi popup chi tiet
                WaitingManager.Show();

                if (ViewImportMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                {
                    ImpMestViewDetailADO impMestView = new ImpMestViewDetailADO(ViewImportMest.ID, ViewImportMest.IMP_MEST_TYPE_ID, ViewImportMest.IMP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(impMestView);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ImpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ViewImportMest.ID);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrImpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataImportMestList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                WaitingManager.Show();
                V_HIS_IMP_MEST_1 impMest1View = null;
                CommonParam param = new CommonParam();
                HisImpMestView1Filter ipmMestView1Filter = new HisImpMestView1Filter();
                ipmMestView1Filter.ID = ViewImportMest.ID;
                var listImpMestView1 = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_1>>("api/HisImpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, ipmMestView1Filter, param);
                if (listImpMestView1 != null && listImpMestView1.Count > 0)
                {
                    impMest1View = listImpMestView1.FirstOrDefault();
                }

                if (impMest1View != null)
                {
                    if (ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)

                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ViewImportMest.ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataImportMestList);
                        if (impMest1View.IS_BLOOD != 1)
                        {
                            CallModule callModule = new CallModule(CallModule.ManuImpMestUpdate, this.roomId, this.roomTypeId, listArgs);
                        }
                        else
                        {
                            CallModule callModule = new CallModule(CallModule.BloodImpMestUpdate, this.roomId, this.roomTypeId, listArgs);
                        }

                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDiscardEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisImpMest/Delete", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataImportMestList();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDisApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);
                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonActualImportEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                if (VImportMest != null && VImportMest.HAS_IDENTITY_MATERIAL == 1 && HisConfigCFG.IDENTITY_MATERIAL_OPTION == "2")
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.IdentityMaterialInformation").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.IdentityMaterialInformation'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(true);
                        listArgs.Add(VImportMest.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateImpTime)ProcessImpMest);
                        listArgs.Add(moduleData);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                else
                {
                    frmMessage frm = new frmMessage(CheckSayYes, (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow(), (RefeshReference)FillDataImportMestList);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void ProcessImpMest(long? impTime)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (VImportMest != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (data, VImportMest);
                    data.IMP_TIME = impTime;
                    data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataImportMestList();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                CommonParam param = new CommonParam();
            }
        }
        private void CheckSayYes(bool IsYes)
        {
            try
            {
                if (IsYes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map
                            <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            (data, row);
                        data.IMP_TIME = TimeImpFromMessage;
                        data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataImportMestList();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                CommonParam param = new CommonParam();
            }
        }

        // 42727 - Cache log key (giữ để tương thích FillDataImportMestList clear)
        private static readonly System.Collections.Generic.HashSet<long> _loggedImpMestIds = new System.Collections.Generic.HashSet<long>();

        // 42727 - Icon "Tạo GD chi tiền" (đen trắng) enable cho MỌI phiếu chưa tạo hoàn ứng
        // (theo yêu cầu nghiệp vụ: kế toán có thể tạo phiếu chi cho bất kỳ phiếu nhập nào)
        private bool IsAllowOpenRepay(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST data)
        {
            try
            {
                if (data == null) return false;
                return (data.REPAY_ID ?? 0) <= 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        // 42727 - Icon "In phiếu hoàn ứng" (màu) enable khi đã có REPAY_ID
        private bool IsAllowPrintRepay(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST data)
        {
            try
            {
                if (data == null) return false;
                return (data.REPAY_ID ?? 0) > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        // 42727 - Handler khi click icon "In phiếu hoàn ứng"
        private void repositoryItemButtonPrintRepayEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST impMest =
                    (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();

                if (!IsAllowPrintRepay(impMest))
                    return;

                PrintRepayByImpMest(impMest);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // 42727 - Mở Hoàn ứng với thông tin tự điền từ phiếu nhập
        private void repositoryItemButtonRepayEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST impMest =
                    (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();

                OpenRepayByImpMest(impMest);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // 42727 - Mở plugin TransactionRepay với ngữ cảnh phiếu nhập (dùng chung cho icon + menu chuột phải)
        internal void OpenRepayByImpMest(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST impMest)
        {
            try
            {
                if (!IsAllowOpenRepay(impMest))
                    return;

                WaitingManager.Show();

                // 42727 - Cố gắng đọc phiếu xuất bán gốc qua CHMS_EXP_MEST_ID hoặc MOBA_EXP_MEST_ID
                // Nếu phiếu nhập KHÔNG có link → mở form Hoàn ứng trống, user nhập số tiền tay
                long originalExpId = (impMest.CHMS_EXP_MEST_ID ?? 0) > 0
                    ? impMest.CHMS_EXP_MEST_ID.Value
                    : (impMest.MOBA_EXP_MEST_ID ?? 0);

                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST originalExp = null;
                long treatmentId = 0;
                decimal totalAmount = 0;
                if (originalExpId > 0)
                {
                    originalExp = GetOriginalExpMest(originalExpId);
                    if (originalExp != null)
                    {
                        treatmentId = originalExp.TDL_TREATMENT_ID ?? 0;
                        totalAmount = GetOriginalExpMestTotalAmount(originalExpId, originalExp.DISCOUNT ?? 0);
                    }
                }

                // Fallback: dùng treatment ID từ chính phiếu nhập (nếu có)
                if (treatmentId <= 0 && (impMest.TDL_TREATMENT_ID ?? 0) > 0)
                {
                    treatmentId = impMest.TDL_TREATMENT_ID.Value;
                }

                // 42727 - Nếu không có phiếu xuất bán gốc → tính tổng tiền từ chính các dòng thuốc/VT của phiếu nhập
                if (totalAmount <= 0)
                {
                    totalAmount = GetImpMestTotalAmount(impMest.ID);
                }

                // Tìm phòng thu ngân của phòng làm việc hiện tại để truyền vào TransactionRepay
                long cashierRoomId = medistock.DEFAULT_CASHIER_ROOM_ID ?? 0;
                if(cashierRoomId <=0)
                {
                    XtraMessageBox.Show("Chưa thiết lập phòng thu ngân mặc định ở kho");
                    return;
                }    

                // Tìm lý do hoàn ứng "Nhập lại xuất bán" (REPAY_REASON_CODE = "07")
                string repayReasonCode = RepayReasonCode.NhapLaiXuatBan;

                TransactionRepayADO ado = new TransactionRepayADO(treatmentId, cashierRoomId);
                ado.ImpMestId = impMest.ID;
                // Chỉ set AutoAmount khi tính được > 0 (phiếu có link), null thì form để trống/dùng default
                ado.AutoAmount = totalAmount > 0 ? (decimal?)totalAmount : null;
                ado.RepayReasonCode = repayReasonCode;
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .FirstOrDefault(o => o.ModuleLink == CallModule.TransactionRepay);

                if (moduleData == null)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay module TransactionRepay");
                    return;
                }

                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    WaitingManager.Hide();
                    return;
                }

                moduleData.RoomId = this.roomId;
                moduleData.RoomTypeId = this.roomTypeId;

                List<object> listArgs = new List<object>();
                listArgs.Add(ado);
                listArgs.Add(moduleData);

                var instance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                WaitingManager.Hide();

                if (instance == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong khoi tao duoc plugin TransactionRepay");
                    return;
                }

                if (instance is Form)
                {
                    ((Form)instance).ShowDialog();
                    // Sau khi đóng → refresh grid để cập nhật REPAY_ID + bật icon "In phiếu"
                    FillDataImportMestList();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // 42727 - Đọc phiếu xuất bán gốc qua API HisExpMest/GetView để lấy mã điều trị + giảm trừ + thông tin bệnh nhân
        private MOS.EFMODEL.DataModels.V_HIS_EXP_MEST GetOriginalExpMest(long expMestId)
        {
            try
            {
                if (expMestId <= 0) return null;

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter filter = new MOS.Filter.HisExpMestViewFilter();
                filter.ID = expMestId;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                var listExp = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(
                        "api/HisExpMest/GetView",
                        ApiConsumer.ApiConsumers.MosConsumer,
                        filter,
                        param);

                if (listExp == null || listExp.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong tim thay phieu xuat ban goc."
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestId), expMestId));
                    return null;
                }

                return listExp.First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        // 42727 - Tính tổng tiền từ chính phiếu nhập (sum medicine + material) khi không có link CHMS/MOBA
        // Công thức: sum(PRICE * AMOUNT * (1 + VAT_RATIO))
        // Dùng PRICE (giá xuất - giá BN đã trả) chứ không dùng IMP_PRICE (giá nhập NCC)
        private decimal GetImpMestTotalAmount(long impMestId)
        {
            try
            {
                if (impMestId <= 0) return 0;

                decimal total = 0;

                // Thuốc
                CommonParam paramMed = new CommonParam();
                MOS.Filter.HisImpMestMedicineViewFilter filterMed = new MOS.Filter.HisImpMestMedicineViewFilter();
                filterMed.IMP_MEST_ID = impMestId;
                var listMed = new BackendAdapter(paramMed)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>>(
                        "api/HisImpMestMedicine/GetView",
                        ApiConsumer.ApiConsumers.MosConsumer,
                        filterMed,
                        paramMed);
                if (listMed != null && listMed.Count > 0)
                {
                    total += listMed.Sum(o =>
                        (o.PRICE ?? 0) * o.AMOUNT * (1 + (o.VAT_RATIO ?? 0)));
                }

                // Vật tư
                CommonParam paramMat = new CommonParam();
                MOS.Filter.HisImpMestMaterialViewFilter filterMat = new MOS.Filter.HisImpMestMaterialViewFilter();
                filterMat.IMP_MEST_ID = impMestId;
                var listMat = new BackendAdapter(paramMat)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL>>(
                        "api/HisImpMestMaterial/GetView",
                        ApiConsumer.ApiConsumers.MosConsumer,
                        filterMat,
                        paramMat);
                if (listMat != null && listMat.Count > 0)
                {
                    total += listMat.Sum(o =>
                        (o.PRICE ?? 0) * o.AMOUNT * (1 + (o.VAT_RATIO ?? 0)));
                }

                return total;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return 0;
            }
        }

        // 42727 - Tính tổng tiền phiếu xuất bán gốc = sum(PRICE * AMOUNT * (1 + VAT_RATIO) - line DISCOUNT) cho cả medicine + material, trừ DISCOUNT của phiếu
        private decimal GetOriginalExpMestTotalAmount(long expMestId, decimal expMestDiscount)
        {
            try
            {
                if (expMestId <= 0) return 0;

                decimal totalLine = 0;

                // Sum cho thuốc
                CommonParam paramMed = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter filterMed = new MOS.Filter.HisExpMestMedicineViewFilter();
                filterMed.EXP_MEST_ID = expMestId;
                var listMed = new BackendAdapter(paramMed)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>(
                        "api/HisExpMestMedicine/GetView",
                        ApiConsumer.ApiConsumers.MosConsumer,
                        filterMed,
                        paramMed);
                if (listMed != null && listMed.Count > 0)
                {
                    totalLine += listMed.Sum(o =>
                        ((o.PRICE ?? 0) * o.AMOUNT * (1 + (o.VAT_RATIO ?? 0))) - (o.DISCOUNT ?? 0));
                }

                // Sum cho vật tư
                CommonParam paramMat = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter filterMat = new MOS.Filter.HisExpMestMaterialViewFilter();
                filterMat.EXP_MEST_ID = expMestId;
                var listMat = new BackendAdapter(paramMat)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>(
                        "api/HisExpMestMaterial/GetView",
                        ApiConsumer.ApiConsumers.MosConsumer,
                        filterMat,
                        paramMat);
                if (listMat != null && listMat.Count > 0)
                {
                    totalLine += listMat.Sum(o =>
                        ((o.PRICE ?? 0) * o.AMOUNT * (1 + (o.VAT_RATIO ?? 0))) - (o.DISCOUNT ?? 0));
                }

                return totalLine - expMestDiscount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return 0;
            }
        }

        // 42727 - Lấy phòng thu ngân tương ứng với phòng làm việc hiện tại
        private long GetCashierRoomIdForCurrentRoom()
        {
            try
            {
                var cashierRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>()
                    .FirstOrDefault(o => o.ROOM_ID == this.roomId);

                if (cashierRoom != null)
                    return cashierRoom.ID;

                return 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private void repositoryItemButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (EVImportMest, VImportMest);

                    EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresul != null)
                    {
                        success = true;
                        FillDataImportMestList();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
