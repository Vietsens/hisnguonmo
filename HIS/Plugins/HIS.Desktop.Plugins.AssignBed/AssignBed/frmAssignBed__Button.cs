using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignBed.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {
        //private void btnNew_Click(object sender, EventArgs e)
        //{
            
        //}

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");

                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                treatment.ID = this.treatmentId;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreateBill_Click(object sender, EventArgs e)
        {
            try
            {
                // get treatment
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT_FEE currentTreatment = null;
                MOS.Filter.HisTreatmentFeeViewFilter treatmentViewFilter = new HisTreatmentFeeViewFilter();
                treatmentViewFilter.ID = this.treatmentId;
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentTreatment = treatments.FirstOrDefault();
                }

                // get sereServs
                //- Lấy các dịch vụ đã chỉ định mà chưa thanh toán (ko thuộc sere_SErv_bill).
                //- Áp dụng cho các dịch vụ viện phí (Không load các dịch vụ có đối tượng thanh toán là BHYT)
                //- Lấy các dịch vụ có creator là người đăng nhập.
                //- Mở form thanh toán như của thu ngân.
                //- Phòng thanh toán là phòng thu ngân mà người dùng đang mở cùng với phòng xử lý (giải pháp như tiếp đón).
                MOS.Filter.HisSereServView5Filter sereServViewFilter = new HisSereServView5Filter();
                sereServViewFilter.TDL_TREATMENT_ID = this.treatmentId;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, sereServViewFilter, param);
                // get sereServBills
                if (sereServs == null || sereServs.Count == 0)
                {
                    return;
                }
                MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.SERE_SERV_IDs = sereServs.Select(p => p.ID).Distinct().ToList();
                var sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);
                if (sereServBills != null && sereServBills.Count > 0)
                {
                    sereServs = sereServs.Where(o => !sereServBills.Select(p => p.SERE_SERV_ID).Distinct().ToList().Contains(o.ID)).ToList();
                }
                // lọc các dịch vụ viện phí, các dịch vụ có creator là người đăng nhập
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sereServs = sereServs.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && o.CREATOR == loginName).ToList();

                if (!btnCreateBill.Enabled || currentTreatment == null)
                    return;
                if (cboCashierRoom.EditValue == null)
                {
                    //MessageBox.Show(ResourceMessage.ChuaChonPhongThuNgan, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue.ToString()));
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
                if (sereServs == null || sereServs.Count == 0)
                {
                    //MessageBox.Show(ResourceMessage.HSDTKhongCoHoacDaThanhToanDichVu, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBill'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = cashierRoom.ROOM_ID;
                    moduleData.RoomTypeId = cashierRoom.ROOM_TYPE_ID;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment);
                    listArgs.Add(moduleData);
                    listArgs.Add(sereServs);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, cashierRoom.ROOM_ID, cashierRoom.ROOM_TYPE_ID), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    //FillDataToControlBySelectTreatment(true);
                    //txtFindTreatmentCode.Focus();
                    //txtFindTreatmentCode.SelectAll();
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDepositService.Enabled || this.treatmentId == 0)
                    return;
                if (cboCashierRoom.EditValue == null)
                {
                    MessageBox.Show("Chưa chọn phòng thu ngân");
                    return;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DepositService").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DepositService'");

                V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
                List<V_HIS_SERE_SERV_5> listSereServ5 = new List<V_HIS_SERE_SERV_5>();
                MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = this.treatmentId;
                var treatmentFeeList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, null);
                if (treatmentFeeList != null && treatmentFeeList.Count > 0)
                {
                    treatmentFee = treatmentFeeList.FirstOrDefault();
                }

                if (this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.SereServs != null && this.serviceReqComboResultSDO.SereServs.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, V_HIS_SERE_SERV_5>();
                    listSereServ5 = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_5>>(this.serviceReqComboResultSDO.SereServs);
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.currentModule.RoomId;
                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    DepositServiceADO ado = new DepositServiceADO();
                    ado.hisTreatment = treatmentFee;
                    ado.BRANCH_ID = WorkPlace.GetBranchId();
                    ado.CashierRoomId = Int64.Parse(cboCashierRoom.EditValue.ToString());
                    ado.IsDepositAll = true;
                    ado.returnSuccess = returnData;

                    ado.SereServs = listSereServ5;
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    //FillDataToControlBySelectTreatment(true);
                    //txtFindTreatmentCode.Focus();
                    //txtFindTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnQRPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentWorkingRoom != null && !string.IsNullOrEmpty(currentWorkingRoom.QR_CONFIG_JSON))
                {
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(currentWorkingRoom.QR_CONFIG_JSON);
                        if (json != null)
                        {
                            adoqr.ConfigValue = new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) };
                            adoqr.BankName = json.BANK;
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                        return;
                    }
                    adoqr.TreatmentId = this.treatmentId;
                    adoqr.TransReqId = 0;
                    adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    if (listConfig != null)
                    {
                        if (listConfig.Count > 1)
                        {
                            //popupMenu1.ClearLinks();
                            foreach (var item in listConfig)
                            {
                                string key = "";
                                string value = item.KEY;
                                int index = value.IndexOf("Info");
                                if (index > 0)
                                {
                                    var shotkey = value.Substring(0, index);
                                    string[] parts = shotkey.Split('.');
                                    if (parts.Length > 0)
                                    {
                                        key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                                    }
                                }
                                else
                                {
                                    key = item.KEY;
                                }


                                BarButtonItem btnOption = new BarButtonItem(null, key);
                                btnOption.ItemClick += (s, args) =>
                                {

                                    selectedConfig = item;
                                    List<object> listArgs = new List<object>();
                                    TransReqQRADO adoqr = new TransReqQRADO();
                                    adoqr.TreatmentId = this.treatmentId;
                                    adoqr.ConfigValue = selectedConfig;
                                    adoqr.TransReqId = 0;
                                    adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;
                                    adoqr.BankName = key;
                                    listArgs.Add(adoqr);
                                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                                };
                                popupMenu1.AddItem(btnOption);
                            }

                            popupMenu1.ShowPopup(Control.MousePosition);
                        }
                        else
                        {
                            selectedConfig = listConfig[0];
                            List<object> listArgs = new List<object>();
                            TransReqQRADO adoqr = new TransReqQRADO();
                            adoqr.TreatmentId = this.treatmentId;
                            adoqr.ConfigValue = selectedConfig;
                            adoqr.TransReqId = 0;
                            adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;

                            string key = "";
                            string value = selectedConfig.KEY;
                            int index = value.IndexOf("Info");
                            if (index > 0)
                            {
                                var shotkey = value.Substring(0, index);
                                string[] parts = shotkey.Split('.');
                                if (parts.Length > 0)
                                {
                                    key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                                }
                            }
                            else
                            {
                                key = selectedConfig.KEY;
                            }

                            adoqr.BankName = key;
                            listArgs.Add(adoqr);
                            LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi thuc hien thanh toan QR tam thu: " + ex);
            }
        }

        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                popupControlContainer1.ShowPopup(new Point(btnConfiguration.Bounds.X + 255, btnConfiguration.Bounds.Bottom + 590));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void returnData(bool success)
        {
            try
            {
                if (success && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                {
                    Parallel.ForEach(this.serviceReqComboResultSDO.ServiceReqs.Where(f => f.ID > 0), l => l.IS_COLLECTED = 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        private HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void CheckEnableBtnQR()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();

                btnQRPay.Enabled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
