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
using AutoMapper;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000025.PDO;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InputSave;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ServiceRoom
{
    public partial class frmServiceRoom : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        HisServiceReqExamRegisterResultSDO examServiceReqRegisterResultSDO;
        V_HIS_EXECUTE_ROOM_1 vhisExecuteRoom;
        HisCardSDO hisCardPatientSdo;
        long requestRoomId;
        TileItem tileNew;
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module currentModule;
        InputSaveADO currentInputSaveAdo;
        V_HIS_SERVICE_REQ ServiceReqPrint;
        List<V_HIS_SERE_SERV> sereServs;
        V_HIS_PATIENT patient;
        List<V_HIS_SERVICE> vlistService;
        bool IsAppointmentAccept;
        public bool IsPriority;
        public bool IsRecieveLater;
        public bool? IsChroNic;
        public long PrimaryTypeId;
        long patientType;
        HisExamRegisterKioskSDO sdoData;
        #endregion

        #region Contructor
        public frmServiceRoom(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmServiceRoom(V_HIS_EXECUTE_ROOM_1 data, Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.vhisExecuteRoom = data;
            this.currentModule = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmServiceRoom(V_HIS_EXECUTE_ROOM_1 data, HisCardSDO hisCardPatientSdo, Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT patient, List<V_HIS_SERVICE> vlistService)
        {
            InitializeComponent();
            try
            {
                this.vhisExecuteRoom = data;
                this.hisCardPatientSdo = hisCardPatientSdo;
                this.currentModule = module;
                this.patient = patient;
                this.vlistService = vlistService;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmServiceRoom(V_HIS_EXECUTE_ROOM_1 data, HisCardSDO hisCardPatientSdo, long roomId, V_HIS_PATIENT patient, List<V_HIS_SERVICE> vlistService, Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.vhisExecuteRoom = data;
                this.hisCardPatientSdo = hisCardPatientSdo;
                this.requestRoomId = roomId;
                this.patient = patient;
                this.vlistService = vlistService;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmServiceRoom(V_HIS_EXECUTE_ROOM_1 data, HisCardSDO hisCardPatientSdo, long roomId, V_HIS_PATIENT patient, List<V_HIS_SERVICE> vlistService, Inventec.Desktop.Common.Modules.Module module, long _patientType)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.vhisExecuteRoom = data;
                this.hisCardPatientSdo = hisCardPatientSdo;
                this.requestRoomId = roomId;
                this.patient = patient;
                this.vlistService = vlistService;
                this.patientType = _patientType;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmServiceRoom(V_HIS_EXECUTE_ROOM_1 data, HisCardSDO hisCardPatientSdo, long roomId, V_HIS_PATIENT patient, List<V_HIS_SERVICE> vlistService, Inventec.Desktop.Common.Modules.Module module, long _patientType, HisExamRegisterKioskSDO sdoData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.vhisExecuteRoom = data;
                this.hisCardPatientSdo = hisCardPatientSdo;
                this.requestRoomId = roomId;
                this.patient = patient;
                this.vlistService = vlistService;
                this.patientType = _patientType;
                this.sdoData = sdoData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceRoom_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //Hiển thị các dịch vụ của phòng đã chọn trước đó
                LoadDataTileServiceRoom(vlistService);
                RegisterTimer(currentModule.ModuleLink, "timerCloseForm", timerCloseForm.Interval, timerCloseForm_Tick);
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Method
        private void getServiceReq(long treatmentID)
        {
            try
            {
                if (treatmentID > 0)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    this.ServiceReqPrint = new V_HIS_SERVICE_REQ();
                    filter.TREATMENT_ID = treatmentID;
                    filter.ORDER_FIELD = "CREATE_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    var listServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {
                        this.ServiceReqPrint = listServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).OrderBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getSereServs(V_HIS_SERVICE_REQ data)
        {
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    this.sereServs = new List<V_HIS_SERE_SERV>();
                    HisSereServViewFilter filter = new HisSereServViewFilter();
                    filter.SERVICE_REQ_ID = data.ID;
                    this.sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        private void getTreatmentID(HisCardSDO data)
        {
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = data.PatientId;
                    filter.ORDER_FIELD = "CREATE_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    var result = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (result != null && result.Count > 0)
                    {
                        this.treatmentId = result.FirstOrDefault().ID;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTileServiceRoom(List<V_HIS_SERVICE> data)
        {
            try
            {      //Khởi tạo động các tile để hiển thị
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        tileNew = new TileItem();
                        tileNew.Text = item.SERVICE_NAME;
                        tileNew.AppearanceItem.Normal.ForeColor = Color.White;
                        tileNew.AppearanceItem.Normal.FontSizeDelta = 2;
                        tileNew.TextAlignment = TileItemContentAlignment.MiddleCenter;
                        tileNew.ItemSize = TileItemSize.Medium;
                        tileNew.Tag = item;
                        Thread.Sleep(10);
                        tileNew.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                        tileNew.Checked = false;
                        tileNew.Visible = true;
                        tileNew.ItemClick += ItemClick;
                        tileNew.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                        tileGroup2.Items.Add(tileNew);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc chắn muốn đăng ký khám?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var vhisService = new V_HIS_SERVICE();
                    vhisService = (V_HIS_SERVICE)e.Item.Tag;
                    btnRegister_Click(null, null, vhisService);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void timerCloseForm_Tick()
        {
            try
            {
                NameForm.CloseOtherForm();

                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //btnRegister_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InputSaveForm(object data)
        {
            try
            {
                this.currentInputSaveAdo = (InputSaveADO)data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnRegister_Click(object sender, EventArgs e, V_HIS_SERVICE vhisService)
        {
            try
            {
                var currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());

                //if (!string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber) && hisCardPatientSdo.RightRouteCode == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE && currentBranch != null && (currentBranch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL || currentBranch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE))
                //{
                //    frmInputSave1 frm = new frmInputSave1((DelegateSelectData)InputSaveForm);
                //    frm.ShowDialog();
                //}

                bool success = false;
                CommonParam param = new CommonParam();
                this.examServiceReqRegisterResultSDO = new HisServiceReqExamRegisterResultSDO();
                WaitingManager.Show();
                HisExamRegisterKioskSDO sdo = new HisExamRegisterKioskSDO();
                sdo.CardSDO = hisCardPatientSdo;

                //sdo.PatientTypeId = string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber) ? GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")) : GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                if (this.patientType != null && this.patientType != 0)
                {
                    sdo.PatientTypeId = this.patientType;
                }

                if (sdo.CardSDO != null && sdo.CardSDO.AppointmentServices != null && sdo.CardSDO.AppointmentServices.Count > 0)
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn được chỉ định dịch vụ CLS khi hẹn khám, bạn có thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        List<ServiceReqDetailSDO> serviceReqDetailSDOs = new List<ServiceReqDetailSDO>();
                        var awhrfaw = BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>().Where(p => p.PATIENT_TYPE_ID == sdo.PatientTypeId).ToList();
                        string mess = "";
                        foreach (var item in sdo.CardSDO.AppointmentServices)
                        {
                            long _serviceId = 0;
                            ServiceReqDetailSDO serviceReqDetailSDO = new ServiceReqDetailSDO();
                            serviceReqDetailSDO.Amount = item.Amount;
                            serviceReqDetailSDO.ServiceId = item.ServiceId;
                            if (item.PatientTypeId > 0)
                            {
                                if (awhrfaw != null && awhrfaw.Exists(p => p.PATIENT_TYPE_ALLOW_ID == item.PatientTypeId))
                                {
                                    //Co Chuyen doi doi tuong thanh toan
                                    serviceReqDetailSDO.PatientTypeId = item.PatientTypeId ?? 0;
                                }
                                else
                                {
                                    //Ktra xem dich vu co doi tuong thanh toan trung voi doi tuong benh nhan hay khong?

                                    var datas = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(p => p.SERVICE_ID == item.ServiceId).ToList();
                                    if (datas != null && datas.Exists(p => p.PATIENT_TYPE_ID == sdo.PatientTypeId))
                                    {
                                        serviceReqDetailSDO.PatientTypeId = sdo.PatientTypeId;
                                    }
                                    else
                                    {
                                        var dataService = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(p => p.ID == item.ServiceId);
                                        if (dataService != null)
                                        {
                                            mess += dataService.SERVICE_NAME + ", ";
                                        }
                                        _serviceId = item.ServiceId;
                                    }
                                }
                            }
                            else
                                serviceReqDetailSDO.PatientTypeId = sdo.PatientTypeId;
                            if (item.ServiceId != _serviceId)
                                serviceReqDetailSDOs.Add(serviceReqDetailSDO);
                        }
                        sdo.AdditionalServices = serviceReqDetailSDOs;
                        IsAppointmentAccept = true;

                        if (!string.IsNullOrEmpty(mess))
                        {
                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == sdo.PatientTypeId);
                            string typeName = patientType != null ? patientType.PATIENT_TYPE_NAME : "";
                            string messFormat = string.Format("Tồn tại dịch vụ hẹn khám ( {0} ) không có đối tượng thanh toán {1}, bạn có muốn tiếp tục không?", mess, typeName);
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(messFormat, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }

                    }
                }

                WaitingManager.Show();

                sdo.RoomId = vhisExecuteRoom.ROOM_ID;
                sdo.RequestRoomId = requestRoomId;
                sdo.ServiceId = vhisService.ID;
                sdo.IsPriority = this.IsPriority;
                sdo.IsNotRequireFee = this.IsRecieveLater;
                sdo.IsChronic = this.IsChroNic;
                if (PrimaryTypeId > 0)
                {
                    sdo.PrimaryPatientTypeId = this.PrimaryTypeId;
                }
                //if (this.currentInputSaveAdo != null && !string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber))
                //{
                //    sdo.TransferInMediOrgCode = this.currentInputSaveAdo.MediOrgCode;
                //    sdo.TransferInMediOrgName = this.currentInputSaveAdo.MediOrgName;
                //    sdo.TransferInIcdCode = this.currentInputSaveAdo.IcdCode;
                //    sdo.TransferInIcdName = this.currentInputSaveAdo.IcdName;
                //    sdo.RightRouteTypeCode = this.currentInputSaveAdo.RightRouteTypeCode;
                //    sdo.TransferInCode = this.currentInputSaveAdo.InCode;
                //    sdo.IsTransferIn = true;
                //}

                if (sdoData != null)
                {
                    sdo.RightRouteTypeCode = sdoData.RightRouteTypeCode;
                    sdo.RightRouteCode = sdoData.RightRouteCode;
                    sdo.TransferInMediOrgCode = sdoData.TransferInMediOrgCode;
                    sdo.TransferInMediOrgName = sdoData.TransferInMediOrgName;
                    sdo.TransferInCode = sdoData.TransferInCode;
                    sdo.TransferInIcdCode = sdoData.TransferInIcdCode;
                    sdo.TransferInIcdName = sdoData.TransferInIcdName;
                    sdo.TransferInCmkt = sdoData.TransferInCmkt;
                    sdo.TransferInFormId = sdoData.TransferInFormId;
                    sdo.TransferInReasonId = sdoData.TransferInReasonId;
                    sdo.TransferInTimeTo = sdoData.TransferInTimeTo;
                    sdo.TransferInTimeFrom = sdoData.TransferInTimeFrom;
                    sdo.IsTransferIn = true;
                }

                LogSystem.Info(LogUtil.TraceData("Du kieu gui len khi dkk kios:", sdo));
                this.examServiceReqRegisterResultSDO = new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>("api/HisServiceReq/ExamRegisterKiosk", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                LogSystem.Info(LogUtil.TraceData("Du kieu dang ky kham tra ve", examServiceReqRegisterResultSDO));
                if (examServiceReqRegisterResultSDO != null)
                {
                    hisCardPatientSdo.PatientId = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient.ID;
                    hisCardPatientSdo.PatientCode = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient.PATIENT_CODE;
                    success = true;
                    onClickPrint();
                }

                this.currentInputSaveAdo = null;

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                //if (success && hisCardPatientSdo.RightRouteCode == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                //{

                //    string keyCheckBHYTTraiTuyen = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.RegisterExamKiosk.ThongBaoTrongTruongHopTraiTuyen");
                //    if (keyCheckBHYTTraiTuyen != null && keyCheckBHYTTraiTuyen != "")
                //    {
                //        MessageBox.Show(keyCheckBHYTTraiTuyen, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }
                //}
                if (success && !string.IsNullOrEmpty(hisCardPatientSdo.ServiceCode))
                {
                    string serviceCode = hisCardPatientSdo.ServiceCode;
                    this.hisCardPatientSdo = new BackendAdapter(param).Get<HisCardSDO>("api/HisCard/GetCardSdoByCode", ApiConsumer.ApiConsumers.MosConsumer, serviceCode, param);
                    if (hisCardPatientSdo.RightRouteCode == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                    {
                        string keyCheckBHYTTraiTuyen = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.RegisterExamKiosk.ThongBaoTrongTruongHopTraiTuyen");
                        if (keyCheckBHYTTraiTuyen != null && keyCheckBHYTTraiTuyen != "")
                        {
                            MessageBox.Show(keyCheckBHYTTraiTuyen, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileControl1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StartTimer(currentModule.ModuleLink, "timerCloseForm");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void panelControl1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StartTimer(currentModule.ModuleLink, "timerCloseForm");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnExit_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StartTimer(currentModule.ModuleLink, "timerCloseForm");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void tileControl1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StartTimer(currentModule.ModuleLink, "timerCloseForm");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void panelControl1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timerCloseForm");
                timerCloseForm.Enabled = false;
                timerCloseForm.Enabled = true;
                StartTimer(currentModule.ModuleLink, "timerCloseForm");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Print
        void onClickPrint()
        {
            try
            {
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_PHIEU_YEU_CAU_KHAM;
                PrintProcess(type);
                if (this.IsAppointmentAccept)
                {
                    this.InPhieuHuoangDanBenhNhan();
                    //PrintProcess(PrintTypeCare.IN_PHIEU_CHI_DINH_TONG_HOP);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeCare
        {
            IN_PHIEU_YEU_CAU_KHAM,
            IN_PHIEU_CHI_DINH_TONG_HOP,
        }

        private void InPhieuYeuCauChiDinhTongHop()
        {
            try
            {
                if (this.IsAppointmentAccept)
                {
                    if (this.examServiceReqRegisterResultSDO != null)
                    {

                        HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                        HisServiceReqSDO.SereServExts = this.examServiceReqRegisterResultSDO.SereServExts;
                        HisServiceReqSDO.SereServs = this.examServiceReqRegisterResultSDO.SereServs;
                        HisServiceReqSDO.ServiceReqs = this.examServiceReqRegisterResultSDO.ServiceReqs;

                        List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();
                        HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                        if (this.examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment != null)
                        {
                            CommonParam param = new CommonParam();
                            HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                            bedLogFilter.TREATMENT_ID = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment.ID;
                            var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                            if (resultBedlog != null)
                            {
                                listBedLogs = resultBedlog;
                            }
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, this.examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment);
                            HisTreatment.PATIENT_TYPE_CODE = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_FROM_TIME = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                            HisTreatment.HEIN_CARD_NUMBER = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER;
                            HisTreatment.HEIN_CARD_TO_TIME = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                            HisTreatment.HEIN_MEDI_ORG_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                            HisTreatment.LEVEL_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.LEVEL_CODE;
                            HisTreatment.RIGHT_ROUTE_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE;
                            HisTreatment.RIGHT_ROUTE_TYPE_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                            HisTreatment.TREATMENT_TYPE_CODE = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_CODE; ;

                        }

                        var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs);
                        PrintServiceReqProcessor.SaveNPrint(false);
                        this.IsAppointmentAccept = false;

                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_PHIEU_YEU_CAU_KHAM:
                        richEditorMain.RunPrintTemplate(PrintCode.PhieuInYeuCauKham, DelegateRunPrinterCare);
                        break;
                    case PrintTypeCare.IN_PHIEU_CHI_DINH_TONG_HOP:
                        InPhieuYeuCauChiDinhTongHop();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintCode.PhieuInYeuCauKham:
                        LoadBieuMauPhieuYeuCauKham(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauPhieuYeuCauKham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (examServiceReqRegisterResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var treatment = new HIS_TREATMENT();
                    WaitingManager.Show();
                    this.sereServs = new List<V_HIS_SERE_SERV>();
                    this.ServiceReqPrint = new V_HIS_SERVICE_REQ();

                    //getTreatmentID(hisCardPatientSdo);

                    //var patientTypeId = string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber) ? GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")) : GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));

                    var patientTypeId = this.patientType;

                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                    //BHYT

                    V_HIS_PATIENT_TYPE_ALTER patyAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    HIS_TREATMENT treatmentPrint = new HIS_TREATMENT();
                    List<HIS_SERE_SERV_DEPOSIT> SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();
                    List<HIS_SERE_SERV_BILL> SereServBills = new List<HIS_SERE_SERV_BILL>();
                    List<V_HIS_TRANSACTION> Transactions = new List<V_HIS_TRANSACTION>();

                    if (examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment != null)
                    {
                        treatmentPrint = examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment;
                    }

                    if (examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter != null && hisCardPatientSdo.HeinCardNumber != null)
                    {
                        HisPatientTypeAlterViewFilter patiFilter = new HisPatientTypeAlterViewFilter();
                        //patiFilter.ID = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.ID;
                        patiFilter.ID = patientTypeId;

                        var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patiFilter, param);
                        if (patientTypeAlter != null && patientTypeAlter.Count > 0)
                        {
                            patyAlter = patientTypeAlter.FirstOrDefault();
                        }
                        else
                        {
                            patyAlter.HEIN_CARD_NUMBER = patientType.ID == HisConfigCFG.PATIENT_TYPE_ID__BHYT ? hisCardPatientSdo.HeinCardNumber : "";
                            patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }
                    }
                    else
                    {
                        patyAlter.HEIN_CARD_NUMBER = patientType.ID == HisConfigCFG.PATIENT_TYPE_ID__BHYT ? hisCardPatientSdo.HeinCardNumber : "";
                        patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                    }

                    this.sereServs = examServiceReqRegisterResultSDO.SereServs;
                    this.ServiceReqPrint = examServiceReqRegisterResultSDO.ServiceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).OrderBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).FirstOrDefault();
                    SereServDeposits = examServiceReqRegisterResultSDO.SereServDeposits;
                    SereServBills = examServiceReqRegisterResultSDO.SereServBills;
                    Transactions = examServiceReqRegisterResultSDO.Transactions;
                    if (ServiceReqPrint == null)
                    {
                        ServiceReqPrint = new V_HIS_SERVICE_REQ();
                        ServiceReqPrint.TDL_PATIENT_ADDRESS = hisCardPatientSdo != null ? hisCardPatientSdo.Address : null;
                        ServiceReqPrint.TDL_PATIENT_PHONE = hisCardPatientSdo != null ? hisCardPatientSdo.Phone : null;

                    }
                    if (treatmentPrint != null && treatmentPrint.HAS_CARD == 1)
                    {
                        List<HIS_CARD> hisCardList = new List<HIS_CARD>();
                        HisCardFilter cardfilter = new HisCardFilter();
                        cardfilter.PATIENT_ID = treatmentPrint.PATIENT_ID;
                        hisCardList = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cardfilter, null);
                        if (hisCardList != null && hisCardList.Count > 0)
                        {
                            PrintKiosk printKiosk = new PrintKiosk(patyAlter, ServiceReqPrint, sereServs, (DelegateReturnSuccess)DelegetSuccess, printTypeCode, fileName, treatmentPrint, SereServDeposits, SereServBills, Transactions, false, hisCardList);
                            printKiosk.RunPrintHasCard();
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patyAlter), patyAlter));
                        PrintKiosk printKiosk = new PrintKiosk(patyAlter, ServiceReqPrint, sereServs, (DelegateReturnSuccess)DelegetSuccess, printTypeCode, fileName, treatmentPrint, SereServDeposits, SereServBills, Transactions, false);
                        //printKiosk.PrintMps25();
                        printKiosk.RunPrint();

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHuoangDanBenhNhan()
        {
            try
            {
                if (this.examServiceReqRegisterResultSDO != null)
                {

                    var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(this.examServiceReqRegisterResultSDO.ServiceReqs, currentModule != null ? this.currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print("Mps000276", true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }

        }

        public void DelegetSuccess(bool success)
        {
            try
            {
                if (success)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion
    }
}
