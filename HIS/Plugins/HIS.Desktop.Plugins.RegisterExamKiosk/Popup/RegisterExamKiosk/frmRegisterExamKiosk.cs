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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.Detail;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InputSave;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisterExamKiosk;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ServiceRoom;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.PrintServiceReqTreatment;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.Processor.Mps000025.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InformationObject;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Address;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisterExamKiosk
{
    public partial class frmRegisterExamKiosk : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        HisServiceReqExamRegisterResultSDO examServiceReqRegisterResultSDO;
        TileItem tileNew;
        InputSaveADO currentInputSaveAdo;
        List<V_HIS_EXECUTE_ROOM_1> listExecuteRoom;
        HisCardSDO hisCardPatientSdo;
        long requestRoomId;
        List<V_HIS_ROOM_COUNTER_1> listRoomCounter1;
        long treatmentId;
        List<V_HIS_SERVICE_ROOM> listServiceRoom;
        HIS.Desktop.Common.DelegateRefreshData setNull;
        bool IsAppointmentAccept;
        V_HIS_SERVICE_REQ ServiceReqPrint;
        List<V_HIS_SERE_SERV> sereServs;
        Inventec.Desktop.Common.Modules.Module currentModule;
        frmServiceRoom frmServiceRoom;
        List<V_HIS_SERVICE_PATY> listServicePatys;
        List<V_HIS_SERVICE> services;
        List<V_HIS_SERVICE> vlistService;
        long PatientTypeId;
        bool checkPrint;
        long treatmentCurrentID;
        bool hideRecieveLater;
        DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private bool stopThread;
        HisExamRegisterKioskSDO sdoData;
        List<HIS_PATIENT_TYPE> lstPatientType = new List<HIS_PATIENT_TYPE>();
        HisPatientForKioskSDO patientForKioskSDO;
        long PrimaryTypeId = 0;
        string ServiceCode;
        #endregion

        #region Contructor
        public frmRegisterExamKiosk(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
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

        public frmRegisterExamKiosk(InformationObjectADO data, HIS.Desktop.Common.DelegateRefreshData _setNull, Inventec.Desktop.Common.Modules.Module module, DelegateCloseForm_Uc closingForm, long patientTypeId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.hisCardPatientSdo = data.CardInfo;
                this.requestRoomId = module.RoomId;
                this.setNull = _setNull;
                this.patientForKioskSDO = data.PatientForKiosk;
                this.PatientTypeId = patientTypeId;
                this.sdoData = data.ExamRegisterKiosk;
                this.DelegateClose = closingForm;
                this.ServiceCode = data.ServiceCode;
                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRegisterExamKiosk_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("hisCardPatientSdo__1_" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisCardPatientSdo), hisCardPatientSdo));
                LoadPatientype();
                LoadDataGridLookUpEdit(cboCareer, "CAREER_CODE", "Mã", "CAREER_NAME", "Tên", "ID", BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == 1).ToList());
                LoadDataGridLookUpEdit(cboNational, "NATIONAL_CODE", "Mã", "NATIONAL_NAME", "Tên", "NATIONAL_NAME", BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == 1).ToList());

                LoadDataGridLookUpEdit(cboEthenic, "ETHNIC_CODE", "Mã", "ETHNIC_NAME", "Tên", "ETHNIC_CODE", BackendDataWorker.Get<SDA_ETHNIC>().Where(o => o.IS_ACTIVE == 1).ToList());
                WaitingManager.Show();
                LoadVisiblePrimaryPatientType();
                LoadVisibleButton();
                loadInfoKiosk();
                getRoomCounter1();
                getServicePaty();
                getServices();
                SetDefaultControl();
                MapHisCardPatientSdo();
                loadInfoPatient(hisCardPatientSdo);
                getRoomExam();
                if (this.listExecuteRoom != null && this.listExecuteRoom.Count > 0)
                {
                    //Khởi tạo các phòng là phòng xử lý khám trên màn hình kiosk
                    LoadDataTileExamRoom(listExecuteRoom);
                }
                LoadIsNotBHYT();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MapHisCardPatientSdo()
        {
            try
            {
                if (hisCardPatientSdo == null && patientForKioskSDO != null)
                {
                    this.hisCardPatientSdo = new HisCardSDO();

                    var data = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == patientForKioskSDO.GENDER_ID);
                    this.hisCardPatientSdo.PatientCode = patientForKioskSDO.PATIENT_CODE;
                    if (!string.IsNullOrEmpty(patientForKioskSDO.PATIENT_CODE))
                        this.hisCardPatientSdo.PatientId = patientForKioskSDO.ID;
                    this.hisCardPatientSdo.Address = patientForKioskSDO.ADDRESS;
                    this.hisCardPatientSdo.GenderCode = data.GENDER_CODE;
                    this.hisCardPatientSdo.GenderName = data.GENDER_NAME;
                    this.hisCardPatientSdo.Dob = patientForKioskSDO.DOB;
                    this.hisCardPatientSdo.FirstName = patientForKioskSDO.FIRST_NAME;
                    this.hisCardPatientSdo.LastName = patientForKioskSDO.LAST_NAME;
                    this.hisCardPatientSdo.HeinAddress = patientForKioskSDO.HeinAddress;
                    this.hisCardPatientSdo.HeinOrgCode = patientForKioskSDO.HeinMediOrgCode;
                    this.hisCardPatientSdo.HeinOrgName = patientForKioskSDO.HeinMediOrgName;
                    this.hisCardPatientSdo.Phone = patientForKioskSDO.PHONE;
                    this.hisCardPatientSdo.CccdNumber = patientForKioskSDO.CCCD_NUMBER;
                    this.hisCardPatientSdo.CccdDate = patientForKioskSDO.CCCD_DATE;
                    this.hisCardPatientSdo.CccdPlace = patientForKioskSDO.CCCD_PLACE;
                    this.hisCardPatientSdo.CardCode = patientForKioskSDO.CardCode;
                    this.hisCardPatientSdo.CmndDate = patientForKioskSDO.CMND_DATE;
                    this.hisCardPatientSdo.CmndNumber = patientForKioskSDO.CMND_NUMBER;
                    this.hisCardPatientSdo.CmndPlace = patientForKioskSDO.CMND_PLACE;
                    this.hisCardPatientSdo.CommuneName = patientForKioskSDO.COMMUNE_NAME;
                    this.hisCardPatientSdo.DistrictName = patientForKioskSDO.DISTRICT_NAME;
                    this.hisCardPatientSdo.GenderId = patientForKioskSDO.GENDER_ID;
                    this.hisCardPatientSdo.Email = patientForKioskSDO.EMAIL;
                    this.hisCardPatientSdo.EthnicName = patientForKioskSDO.ETHNIC_NAME;
                    this.hisCardPatientSdo.NationalName = patientForKioskSDO.NATIONAL_NAME;
                    this.hisCardPatientSdo.ProvinceName = patientForKioskSDO.PROVINCE_NAME;
                    this.hisCardPatientSdo.ReligionName = patientForKioskSDO.RELIGION_NAME;
                    this.hisCardPatientSdo.VirAddress = patientForKioskSDO.VIR_ADDRESS;
                    this.hisCardPatientSdo.WorkPlace = patientForKioskSDO.WORK_PLACE;
                    this.hisCardPatientSdo.HtAddress = patientForKioskSDO.HT_ADDRESS;
                    this.hisCardPatientSdo.HtCommuneName = patientForKioskSDO.HT_COMMUNE_NAME;
                    this.hisCardPatientSdo.HtProvinceName = patientForKioskSDO.HT_PROVINCE_NAME;
                    this.hisCardPatientSdo.HtDistrictName = patientForKioskSDO.HT_DISTRICT_NAME;
                    this.hisCardPatientSdo.HeinCardNumber = patientForKioskSDO.HeinCardNumber;
                    this.hisCardPatientSdo.HeinCardToTime = patientForKioskSDO.HeinCardToTime;
                    this.hisCardPatientSdo.HeinCardFromTime = patientForKioskSDO.HeinCardFromTime;
                    this.hisCardPatientSdo.ServiceCode = this.ServiceCode;
                    this.hisCardPatientSdo.CareerId = patientForKioskSDO.CAREER_ID;
                    this.hisCardPatientSdo.NationalCode = patientForKioskSDO.NATIONAL_CODE;
                    this.hisCardPatientSdo.MpsNationalCode = patientForKioskSDO.MPS_NATIONAL_CODE;
                    this.hisCardPatientSdo.HtCommuneCode = patientForKioskSDO.HT_COMMUNE_CODE;
                    this.hisCardPatientSdo.HtDistrictCode = patientForKioskSDO.HT_DISTRICT_CODE;
                    this.hisCardPatientSdo.HtProvinceCode = patientForKioskSDO.HT_PROVINCE_CODE; 
                    this.hisCardPatientSdo.EthnicCode = patientForKioskSDO.ETHNIC_CODE;
                }

                if (hisCardPatientSdo != null && !string.IsNullOrEmpty(hisCardPatientSdo.HeinAddress) && string.IsNullOrEmpty(hisCardPatientSdo.Address))
                    hisCardPatientSdo.Address = hisCardPatientSdo.HeinAddress;
                Inventec.Common.Logging.LogSystem.Debug("hisCardPatientSdo__2_" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisCardPatientSdo), hisCardPatientSdo));
                if (hisCardPatientSdo.CareerId == null)
                {
                    var career = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.HIS_CAREER_CODE__BASE));
                    if (career != null)
                        hisCardPatientSdo.CareerId = career.ID;
                }
                if (hisCardPatientSdo != null && (string.IsNullOrEmpty(hisCardPatientSdo.EthnicName) || string.IsNullOrEmpty(hisCardPatientSdo.EthnicCode)))
                {
                    var EthenicKey = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ETHNIC_CODE__BASE) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    if (EthenicKey != null) {
                        hisCardPatientSdo.EthnicName = EthenicKey.ETHNIC_NAME;
                        hisCardPatientSdo.EthnicCode = EthenicKey.ETHNIC_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientype()
        {
            try
            {
                lstPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVisiblePrimaryPatientType()
        {
            try
            {
                if (HisConfigCFG.PrimaryPatientType == 2)
                {
                    layoutControlItem33.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem34.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    var dtParimaryPatientType = lstPatientType.Where(o => o.IS_ADDITION == 1 && o.IS_ACTIVE == 1 && o.ID != this.PatientTypeId).ToList();
                    foreach (var item in dtParimaryPatientType)
                    {
                        RadioGroupItem rdItem = new RadioGroupItem(item.ID, item.PATIENT_TYPE_NAME);
                        this.radioGroup1.Properties.Items.Add(rdItem);
                    }
                    radioGroup1.Properties.Columns = 9;
                    radioGroup1.AutoSizeInLayoutControl = true;
                }
                else
                {
                    layoutControlItem2.Size = new Size(layoutControlItem2.Size.Width, layoutControlItem2.Size.Height - layoutControlItem34.Size.Height * 2);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIsNotBHYT()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadIsNotBHYT ");
                Inventec.Common.Logging.LogSystem.Debug("this.PatientTypeId " + this.PatientTypeId);
                if (this.PatientTypeId != 1 && this.PatientTypeId != 0)
                {
                    var objectNotBHYT = lstPatientType.FirstOrDefault(o => o.ID == this.PatientTypeId);
                    label5.Text = "Đối tượng: ";
                    if (objectNotBHYT != null)
                    {
                        lblCodeCardBhyt.Text = objectNotBHYT.PATIENT_TYPE_NAME;
                        lblPlaceRegister.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadVisibleButton()
        {
            try
            {
                int option = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.RECEPTION_ROOM.KIOSK.SHOW_BUTTON_TRANSACTION_OPTION");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => option), option));

                if (option == 1)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Option 1");
                    //btnThanhtoan.Visible = true;
                    //layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnTamthu.Visible = false;
                }
                else if (option == 2)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Option 2");
                    //btnThanhtoan.Visible = false;
                    btnTamthu.Visible = true;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Option 3");
                    // btnThanhtoan.Visible = false;
                    btnTamthu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Method
        public long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
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

        private void getService(V_HIS_EXECUTE_ROOM_1 data, HisCardSDO hisCard, ref List<V_HIS_SERVICE> listService, bool IsRegisterClick = false)
        {
            try
            {
                //HIS_SERVICE_ROOM
                CommonParam param = new CommonParam();
                this.listServiceRoom = new List<V_HIS_SERVICE_ROOM>();
                List<V_HIS_SERVICE_PATY> servicePatys = new List<V_HIS_SERVICE_PATY>();
                HisServiceRoomFilter filter = new HisServiceRoomFilter();
                List<long> roomServiceIds = new List<long>();
                List<long> patyServiceIds = new List<long>();
                List<long> serviceIds = new List<long>();

                //Dịch vụ mà phòng xử lý
                if (data != null)
                {
                    filter.ROOM_ID = data.ROOM_ID;
                }
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.listServiceRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == data.ROOM_ID).ToList();
                foreach (var item in listServiceRoom)
                {
                    roomServiceIds.Add(item.SERVICE_ID);
                }

                if (this.PatientTypeId != 0)
                {
                    //Chính sách giá dịch vụ tương ứng với loại bệnh nhân  
                    var serviceByPatys = listServicePatys
                        .Where(o => o.PATIENT_TYPE_ID == this.PatientTypeId
                            && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                            && roomServiceIds != null && roomServiceIds.Contains(o.SERVICE_ID)
                        ).ToList();
                    var serviceIdByPatys = serviceByPatys.Select(o => o.SERVICE_ID).Distinct().ToList();
                    if (serviceIdByPatys != null && serviceIdByPatys.Count > 0)
                    {
                        listService = services.Where(o => serviceIdByPatys != null && serviceIdByPatys.Contains(o.ID)).ToList();
                        if (HisConfigCFG.PrimaryPatientType == 2 && IsRegisterClick && PrimaryTypeId > 0)
                        {
                            var serviceByPatysPrimaryPatientType = listServicePatys
                           .Where(o => o.PATIENT_TYPE_ID == this.PrimaryTypeId
                               && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                               && roomServiceIds != null && roomServiceIds.Contains(o.SERVICE_ID)
                           ).ToList();
                            var serviceIdByPatysPrimaryPatientType = serviceByPatysPrimaryPatientType.Select(o => o.SERVICE_ID).Distinct().ToList();
                            serviceByPatys.AddRange(serviceByPatysPrimaryPatientType);
                            if (serviceIdByPatysPrimaryPatientType != null && serviceIdByPatysPrimaryPatientType.Count > 0)
                            {
                                var dicGroup = serviceByPatys.GroupBy(o => o.SERVICE_ID);
                                listService = new List<V_HIS_SERVICE>();
                                foreach (var item in dicGroup)
                                {
                                    var lstValue = dicGroup.FirstOrDefault(o => o.Key == item.Key);
                                    if (lstValue.Count() >= 2 && lstValue.Last().PRICE > lstValue.First().PRICE)
                                    {
                                        listService.Add(services.FirstOrDefault(o => o.ID == item.Key));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var serviceIdByPatys = listServicePatys
                        .Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                            && roomServiceIds != null && roomServiceIds.Contains(o.SERVICE_ID)
                        ).Select(o => o.SERVICE_ID).Distinct().ToList();
                    if (serviceIdByPatys != null && serviceIdByPatys.Count > 0)
                    {
                        listService = services.Where(o => serviceIdByPatys != null && serviceIdByPatys.Contains(o.ID)).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getServicePaty()
        {
            try
            {
                this.listServicePatys = new List<V_HIS_SERVICE_PATY>();
                this.listServicePatys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getServices()
        {
            try
            {
                this.services = new List<V_HIS_SERVICE>();
                this.services = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getServiceReq(long treatmentID)
        {
            try
            {
                if (treatmentID > 0)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    ServiceReqPrint = new V_HIS_SERVICE_REQ();
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
                    sereServs = new List<V_HIS_SERE_SERV>();
                    string month = "";
                    string day = "";

                    //Tháng với ngày trả về không có số 0 đằng trước nên xét trường hợp đáy thì thêm số 0 vào đằng trước
                    if (DateTime.Now.Month.ToString().Length < 2)
                    {
                        month = "0" + DateTime.Now.Month.ToString();
                    }
                    else
                    {
                        month = DateTime.Now.Month.ToString();
                    }
                    if (DateTime.Now.Day.ToString().Length < 2)
                    {
                        day = "0" + DateTime.Now.Day.ToString();
                    }
                    else
                    {
                        day = DateTime.Now.Day.ToString();
                    }
                    var today = DateTime.Now.Year.ToString() + month + day;
                    HisSereServViewFilter filter = new HisSereServViewFilter();
                    filter.SERVICE_REQ_ID = data.ID;
                    filter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(today + "000000");
                    filter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(today + "235959");
                    this.sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_TREATMENT getTreatmentID(HisCardSDO data)
        {
            HIS_TREATMENT rs = new HIS_TREATMENT();
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = data.PatientId;
                    var result = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (result != null && result.Count > 0)
                    {
                        treatmentId = result.OrderByDescending(o => o.ID).FirstOrDefault().ID;
                        rs = result.OrderByDescending(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void getRoomExam()
        {
            try
            {
                stopThread = true;
                CommonParam param = new CommonParam();
                HisExecuteRoomView1Filter filter = new HisExecuteRoomView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_EXAM = true;
                filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                this.listExecuteRoom = new List<V_HIS_EXECUTE_ROOM_1>();
                this.listExecuteRoom = new BackendAdapter(param).Get<List<V_HIS_EXECUTE_ROOM_1>>("api/HisExecuteRoom/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getRoomCounter1()
        {
            try
            {
                stopThread = true;
                this.listRoomCounter1 = new List<V_HIS_ROOM_COUNTER_1>();
                CommonParam param = new CommonParam();
                HisRoomCounter1ViewFilter roomCounterFilter = new HisRoomCounter1ViewFilter();
                this.listRoomCounter1 = new BackendAdapter(param).Get<List<V_HIS_ROOM_COUNTER_1>>("api/HisRoom/GetCounter1View", ApiConsumers.MosConsumer, roomCounterFilter, param);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadInfoPatient(HisCardSDO data)
        {
            try
            {
                if (data != null)
                {
                    lblName.Text = data.LastName + " " + data.FirstName;
                    lblAddress.Text = string.IsNullOrEmpty(data.HeinAddress) ? data.Address : data.HeinAddress;
                    lblCardNumber.Text = data.CardCode;
                    lblGender.Text = data.GenderName;
                    lblCodeCardBhyt.Text = data.HeinCardNumber;
                    lblPlaceRegister.Text = data.HeinOrgName;
                    lblBorn.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.Dob.ToString());
                    if (this.patientForKioskSDO != null && this.patientForKioskSDO.Balance != null)
                    {
                        layoutControlItem35.Visibility = layoutControlItem36.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lblBalance.Text = this.patientForKioskSDO.Balance.ToString();
                    }
                    var _id = (BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == data.CareerId) ?? new HIS_CAREER()).ID;
                    if (_id > 0) cboCareer.EditValue = _id;
                    if (!string.IsNullOrEmpty(data.NationalName))
                    {
                        var national = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.NATIONAL_NAME == data.NationalName);
                        cboNational.EditValue = national != null ? data.NationalName : null;
                    }

                    if (cboNational.EditValue == null || string.IsNullOrEmpty(cboNational.EditValue.ToString()))
                    {
                        var nationalKey = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.NATIONAL_CODE__BASE));
                        if (nationalKey != null)
                            cboNational.EditValue = nationalKey.NATIONAL_NAME;
                    }
                    if (!string.IsNullOrEmpty(data.EthnicName))
                    {
                        var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ETHNIC_NAME == data.EthnicName);
                        cboEthenic.EditValue = ethnic != null ? data.EthnicCode : null;
                    }

                    if (cboEthenic.EditValue == null || string.IsNullOrEmpty(cboEthenic.EditValue.ToString()))
                    {
                        var ethnicKey = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ETHNIC_CODE__BASE));
                        if (ethnicKey != null)
                            cboEthenic.EditValue = ethnicKey.ETHNIC_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadInfoKiosk()
        {
            try
            {
                string branchName = BranchDataWorker.Branch.BRANCH_NAME.ToUpper();
                lblHospitalInfo.Text = "CHÀO MỪNG BẠN ĐẾN VỚI " + branchName;
                lblHospitalInfo1.Text = "Dịch vụ đăng ký khám bệnh tự động bằng thẻ KCB thông minh";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTileExamRoom(List<V_HIS_EXECUTE_ROOM_1> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    data = data.Where(o => o.IS_USE_KIOSK == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    var groupData = data.GroupBy(o => o.ROOM_GROUP_ID).ToList();
                    foreach (var grItem in groupData)
                    {
                        var group = new TileGroup();
                        group.Text = grItem.FirstOrDefault().ROOM_GROUP_NAME ?? "Khác";
                        var listGroupItem = grItem.ToList();
                        listGroupItem = listGroupItem.OrderBy(o => o.NUM_ORDER).ToList();
                        foreach (var item in listGroupItem)
                        {
                            var listSerrvice = new List<V_HIS_SERVICE>();
                            getService(item, hisCardPatientSdo, ref listSerrvice);
                            string AMOUNT = "";
                            string TOTAL = "";
                            if (listRoomCounter1 != null && listRoomCounter1.Count > 0)
                            {
                                var count = listRoomCounter1.FirstOrDefault(o => o.ID == item.ROOM_ID);
                                AMOUNT = ((long)(count.TOTAL_OPEN_TODAY ?? 0)).ToString();
                                TOTAL = ((long)(item.TOTAL_TODAY_SERVICE_REQ ?? 0)).ToString();
                            }
                            if (listSerrvice != null && listSerrvice.Count > 0)
                            {
                                TileItem tileNewtemp = new TileItem();
                                tileNewtemp.Text = item.RESPONSIBLE_USERNAME;
                                tileNewtemp.AppearanceItem.Normal.FontSizeDelta = 1;
                                tileNew = new TileItem();
                                tileNew.Text = "\n" + "\n" + item.EXECUTE_ROOM_NAME + "\n(" + AMOUNT + "/" + TOTAL + ")" + "\n" + "\n" + tileNewtemp.Text;
                                tileNew.AppearanceItem.Normal.FontSizeDelta = 2;
                                tileNew.AppearanceItem.Normal.ForeColor = Color.White;
                                tileNew.TextAlignment = TileItemContentAlignment.TopLeft;
                                tileNew.ItemSize = TileItemSize.Medium;
                                tileNew.Tag = item;
                                Thread.Sleep(10);
                                tileNew.AppearanceItem.Normal.BorderColor = Color.DarkGreen;
                                tileNew.Checked = false;
                                tileNew.Visible = true;
                                tileNew.ItemClick += ItemClick;
                                tileNew.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                                group.Items.Add(tileNew);
                            }
                        }
                        tileControl1.Groups.Add(group);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal long GetSelectedOpionGroup()
        {
            long selectedOpionGroup = -1;
            try
            {
                int iSelectedIndex = this.radioGroup1.SelectedIndex;
                if (iSelectedIndex != -1)
                    selectedOpionGroup = (long)this.radioGroup1.Properties.Items[iSelectedIndex].Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return selectedOpionGroup;
        }

        void ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                ResetLoopCount();
                if (frmServiceRoom != null)
                {
                    frmServiceRoom.Close();
                    frmServiceRoom = null;
                }

                if (cboCareer.EditValue == null || string.IsNullOrEmpty(cboNational.EditValue.ToString()))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập " + (cboCareer.EditValue == null ? "nghề nghiệp" : "quốc tịch"));
                    return;
                }
                hisCardPatientSdo.CareerId = Int64.Parse(cboCareer.EditValue.ToString());
                hisCardPatientSdo.NationalName = cboNational.EditValue.ToString();
                var nal = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == cboNational.EditValue.ToString());
                hisCardPatientSdo.NationalCode = nal != null ? nal.NATIONAL_CODE : "";
                hisCardPatientSdo.MpsNationalCode = nal != null ? nal.MPS_NATIONAL_CODE : "";
                if (cboEthenic.EditValue != null)
                {
                    hisCardPatientSdo.EthnicCode = cboEthenic.EditValue.ToString();
                    var eth = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == cboEthenic.EditValue.ToString());
                    hisCardPatientSdo.EthnicName = eth != null ? eth.ETHNIC_NAME : "";
                }
                PrimaryTypeId = -1;
                if (HisConfigCFG.PrimaryPatientType == 2 && radioGroup1.SelectedIndex == -1)
                {
                    var checkPatientType = lstPatientType.FirstOrDefault(o => o.ID == PatientTypeId);
                    if (checkPatientType != null && checkPatientType.IS_ADDITION_REQUIRED == 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn vui lòng chọn \"Phụ thu\" trước khi tiếp tục.", "Thông báo");
                        return;
                    }
                }
                PrimaryTypeId = GetSelectedOpionGroup();
                //Nếu phòng không có dịch vụ khám thì đưa ra thông báo khoong có dịch vụ nào.
                //Nếu có 1 dịch vụ khám thì hiển thị thông báo Bạn có chắc chắn muốn đăng ký khám
                //Nếu có nhiều dịch vụ thì hiển thị ra popup đẻ người dùng chọn dịch vụ để đăng kí

                this.vlistService = new List<V_HIS_SERVICE>();
                Inventec.Common.Logging.LogSystem.Debug("this.PatientTypeId 1 là : " + this.PatientTypeId);
                getService((V_HIS_EXECUTE_ROOM_1)e.Item.Tag, hisCardPatientSdo, ref vlistService, true);

                if (vlistService != null && vlistService.Count > 1)
                {
                    Inventec.Common.Logging.LogSystem.Debug("this.PatientTypeId là : " + this.PatientTypeId);
                    frmServiceRoom = new frmServiceRoom((V_HIS_EXECUTE_ROOM_1)e.Item.Tag, hisCardPatientSdo, requestRoomId, null, vlistService, this.currentModule, this.PatientTypeId, sdoData);
                    frmServiceRoom.IsPriority = ChkPriority.Checked;
                    frmServiceRoom.IsChroNic = chkChronic.Checked;
                    if (radioGroup1.SelectedIndex != -1)
                    {
                        frmServiceRoom.PrimaryTypeId = GetSelectedOpionGroup();
                    }
                    if (layoutControlItem30.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("IsNotRequireFee 1" + this.hideRecieveLater);
                        SetDefaultRecieveLater();
                        frmServiceRoom.IsRecieveLater = this.hideRecieveLater ? true : false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("IsNotRequireFee 2" + chkRecieveLater.Checked);
                        frmServiceRoom.IsRecieveLater = chkRecieveLater.Checked ? true : false;
                    }
                    frmServiceRoom.ShowDialog();
                }
                else if (vlistService != null && vlistService.Count == 1)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc chắn muốn đăng ký khám?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        btnRegister_Click(null, null, vlistService.FirstOrDefault(), (V_HIS_EXECUTE_ROOM_1)e.Item.Tag);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phòng không có dịch vụ nào");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                lblCardNumber.Text = "";
                lblName.Text = "";
                lblCodeCardBhyt.Text = "";
                lblGender.Text = "";
                lblAddress.Text = "";
                lblPlaceRegister.Text = "";
                lblBorn.Text = "";
                SetDefaultRecieveLater();
                ChkPriority.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultRecieveLater()
        {
            try
            {
                string option = ConfigApplicationWorker.Get<string>("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE");
                Inventec.Common.Logging.LogSystem.Debug("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE" + option);
                if (layoutControlItem30.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    if (!String.IsNullOrEmpty(option))
                    {
                        var lstKey = option.Split(',').ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstKey), lstKey));
                        var patientType = lstPatientType.Where(o => o.ID == this.PatientTypeId).ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientType), patientType));
                        if (patientType != null && patientType.Count > 0)
                        {
                            var checkExist = patientType.Where(o => lstKey.Contains(o.PATIENT_TYPE_CODE)).ToList();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkExist), checkExist));
                            if (checkExist != null && checkExist.Count > 0)
                            {
                                this.hideRecieveLater = true;
                            }
                            else
                            {
                                this.hideRecieveLater = false;
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug("hideRecieveLater" + this.hideRecieveLater);
                    }
                }
                else
                {

                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultRecieveLater option: " + option);
                    if (!String.IsNullOrEmpty(option))
                    {
                        var lstKey = option.Split(',').ToList();
                        var patientType = lstPatientType.Where(o => o.ID == this.PatientTypeId).ToList();
                        if (patientType != null && patientType.Count > 0)
                        {
                            var checkExist = patientType.Where(o => lstKey.Contains(o.PATIENT_TYPE_CODE)).ToList();
                            if (checkExist != null && checkExist.Count > 0)
                            {
                                chkRecieveLater.Checked = true;
                            }
                            else
                            {
                                chkRecieveLater.Checked = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
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

        private void btnRegister_Click(object sender, EventArgs e, V_HIS_SERVICE vhisService, V_HIS_EXECUTE_ROOM_1 executeRoom)
        {
            try
            {
                IsAppointmentAccept = false;
                var currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());

                bool success = false;
                CommonParam param = new CommonParam();
                this.examServiceReqRegisterResultSDO = new HisServiceReqExamRegisterResultSDO();
                HisExamRegisterKioskSDO sdo = new HisExamRegisterKioskSDO();
                sdo.CardSDO = hisCardPatientSdo;
                ProcessRegisterAddress.SplitAddress(sdo.CardSDO);
                Inventec.Common.Logging.LogSystem.Debug("btnRegister_Click 1 là : " + this.PatientTypeId);
                WaitingManager.Show();
                sdo.PatientTypeId = this.PatientTypeId;

                if (sdo.CardSDO.AppointmentServices != null && sdo.CardSDO.AppointmentServices.Count > 0)
                {
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
                            var patientType = lstPatientType.FirstOrDefault(p => p.ID == sdo.PatientTypeId);
                            string typeName = patientType != null ? patientType.PATIENT_TYPE_NAME : "";
                            string messFormat = string.Format("Tồn tại dịch vụ hẹn khám ( {0} ) không có đối tượng thanh toán {1}, bạn có muốn tiếp tục không?", mess, typeName);
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(messFormat, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                    }
                }

                sdo.RoomId = executeRoom.ROOM_ID;
                sdo.ServiceId = vhisService.ID;
                sdo.RequestRoomId = requestRoomId;
                sdo.IsPriority = ChkPriority.Checked;
                sdo.IsChronic = chkChronic.Checked;
                if (radioGroup1.SelectedIndex != -1)
                {
                    sdo.PrimaryPatientTypeId = GetSelectedOpionGroup();
                }

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

                if (layoutControlItem30.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    Inventec.Common.Logging.LogSystem.Debug("IsNotRequireFee 1" + this.hideRecieveLater);
                    string option = ConfigApplicationWorker.Get<string>("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE");
                    Inventec.Common.Logging.LogSystem.Debug("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE" + option);
                    if (!String.IsNullOrEmpty(option))
                    {
                        var lstKey = option.Split(',').ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstKey), lstKey));
                        var patientType = lstPatientType.Where(o => o.ID == this.PatientTypeId).ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientType), patientType));
                        if (patientType != null && patientType.Count > 0)
                        {
                            var checkExist = patientType.Where(o => lstKey.Contains(o.PATIENT_TYPE_CODE)).ToList();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkExist), checkExist));
                            if (checkExist != null && checkExist.Count > 0)
                            {
                                this.hideRecieveLater = true;
                            }
                            else
                            {
                                this.hideRecieveLater = false;
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug("hideRecieveLater" + this.hideRecieveLater);
                    }

                    sdo.IsNotRequireFee = this.hideRecieveLater ? true : false;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("IsNotRequireFee 2" + chkRecieveLater.Checked);
                    sdo.IsNotRequireFee = chkRecieveLater.Checked ? true : false;
                }

                LogSystem.Info(LogUtil.TraceData("Du kieu gui len khi dkk kios:", sdo));
                examServiceReqRegisterResultSDO = new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>("api/HisServiceReq/ExamRegisterKiosk", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                LogSystem.Info(LogUtil.TraceData("Du kieu dang ky kham tra ve", examServiceReqRegisterResultSDO));
                if (examServiceReqRegisterResultSDO != null)
                {
                    //LogSystem.Debug("-----------Update Patient");
                    UpdatePatient(sdo.CardSDO, examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient);
                    hisCardPatientSdo.PatientId = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient.ID;
                    hisCardPatientSdo.PatientCode = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient.PATIENT_CODE;
                    success = true;
                    onClickPrint();

                    if (!ProcessRegisterAddress.CheckAddress(examServiceReqRegisterResultSDO.HisPatientProfile.HisPatient))
                    {
                        MessageBox.Show("Thông tin địa chỉ của bệnh nhân không đầy đủ. Vui lòng bổ sung thông tin tại phòng khám");
                    }
                }

                this.currentInputSaveAdo = null;

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);

                // trường hợp đã check form chuyển tuyến ở trên vẫn có thông báo này
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

        private void UpdatePatient(HisCardSDO cardSdo, HIS_PATIENT patient)
        {
            try
            {
                string address = patient.VIR_ADDRESS ?? patient.TDL_HEIN_CARD_ADDRESS;
                bool updateAddress = !string.IsNullOrWhiteSpace(address) && (string.IsNullOrWhiteSpace(patient.DISTRICT_CODE) || string.IsNullOrWhiteSpace(patient.COMMUNE_CODE) || string.IsNullOrWhiteSpace(patient.PROVINCE_CODE));
                bool updateHtAddress = diffString(patient.HT_COMMUNE_CODE, cardSdo.HtCommuneCode) || diffString(patient.HT_PROVINCE_CODE, cardSdo.HtProvinceCode) || diffString(patient.HT_DISTRICT_CODE, cardSdo.HtDistrictCode);
                if (updateAddress || updateHtAddress)
                {
                    AddressProcessor addressProcessor = new AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                    AddressADO addressADO = addressProcessor.SplitFromFullAddress(address);
                    bool flag2 = addressADO != null && !string.IsNullOrEmpty(addressADO.ProvinceName) && !string.IsNullOrEmpty(addressADO.DistrictName) && !string.IsNullOrEmpty(addressADO.CommuneName);
                    if (flag2)
                    {
                        patient.DISTRICT_CODE = addressADO.DistrictCode;
                        patient.DISTRICT_NAME = addressADO.DistrictName;
                        patient.COMMUNE_CODE = addressADO.CommuneCode;
                        patient.COMMUNE_NAME = addressADO.CommuneName;
                        patient.PROVINCE_CODE = addressADO.ProvinceCode;
                        patient.PROVINCE_NAME = addressADO.ProvinceName;
                        patient.ADDRESS = addressADO.Address;
                    }

                    if (updateHtAddress)
                    {
                        patient.HT_COMMUNE_CODE = cardSdo.HtCommuneCode;
                        patient.HT_DISTRICT_CODE = cardSdo.HtDistrictCode;
                        patient.HT_PROVINCE_CODE = cardSdo.HtProvinceCode;
                        patient.HT_COMMUNE_NAME = cardSdo.HtCommuneName;
                        patient.HT_DISTRICT_NAME = cardSdo.HtDistrictName;
                        patient.HT_PROVINCE_NAME = cardSdo.HtProvinceName;
                    }

                    CommonParam paramPatient = new CommonParam();
                    HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                    patientUpdateSdo.HisPatient = new HIS_PATIENT();
                    patientUpdateSdo.HisPatient = patient;
                    patientUpdateSdo.UpdateTreatment = true;
                    LogSystem.Debug("Update Patient Data: " + LogUtil.TraceData("__HisPatientSdo: ", patientUpdateSdo));
                    var resultData = new BackendAdapter(paramPatient).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, paramPatient);
                    if (resultData != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Update Patient Result" + LogUtil.TraceData("__HisPatientSdo Result: ", resultData));
                    }
                    else
                    {
                        LogSystem.Debug("Update Patient Faild: " + LogUtil.TraceData("__HisPatientSdo: ", patientUpdateSdo));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool diffString(string string1, string string2)
        {
            string check1 = String.IsNullOrWhiteSpace(string1) ? "" : string1.ToLower();
            string check2 = String.IsNullOrWhiteSpace(string2) ? "" : string2.ToLower();

            return check1 != check2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                this.checkPrint = true;
                onClickPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRegisterExamKiosk_KeyUp(object sender, KeyEventArgs e)
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

        private void btnEnd_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                hisCardPatientSdo = null;
                if (frmServiceRoom != null)
                {
                    frmServiceRoom.Close();
                    frmServiceRoom = null;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                if (hisCardPatientSdo.PatientId != null)
                {
                    frmDetail frm = new frmDetail(hisCardPatientSdo.PatientId ?? 0, (HIS.Desktop.Common.DelegateCloseForm_Uc)CloseForm);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRegisterExamKiosk_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.setNull();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        InPhieuYeuCauChiDinhTongHop(PrintCode.PhieuInChiDinhTongHop);
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

        private void InPhieuYeuCauChiDinhTongHop(string printTypeCode)
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
                            HisTreatment.PATIENT_TYPE_CODE = lstPatientType.FirstOrDefault(o => o.ID == this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_FROM_TIME = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                            HisTreatment.HEIN_CARD_NUMBER = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER;
                            HisTreatment.HEIN_CARD_TO_TIME = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                            HisTreatment.HEIN_MEDI_ORG_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                            HisTreatment.LEVEL_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.LEVEL_CODE;
                            HisTreatment.RIGHT_ROUTE_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE;
                            HisTreatment.RIGHT_ROUTE_TYPE_CODE = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                            HisTreatment.TREATMENT_TYPE_CODE = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_CODE;
                        }
                        var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs);
                        PrintServiceReqProcessor.Print(printTypeCode, true);
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
                WaitingManager.Show();
                this.sereServs = new List<V_HIS_SERE_SERV>();
                this.ServiceReqPrint = new V_HIS_SERVICE_REQ();
                V_HIS_PATIENT_TYPE_ALTER patyAlter = new V_HIS_PATIENT_TYPE_ALTER();
                HIS_TREATMENT treatmentPrint = new HIS_TREATMENT();

                if (hisCardPatientSdo.PatientId != null)
                    treatmentPrint = getTreatmentID(hisCardPatientSdo);

                //Nếu là in lại thì checkPrint=true
                if (this.checkPrint)
                {
                    if (treatmentId > 0)
                    {
                        var patientTypeId = string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber) ? GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")) : GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                        var patientType = lstPatientType.FirstOrDefault(o => o.ID == patientTypeId);
                        //BHYT

                        if (hisCardPatientSdo.HeinCardNumber != null)
                        {
                            CommonParam param = new CommonParam();
                            HisPatientTypeAlterViewFilter patiFilter = new HisPatientTypeAlterViewFilter();
                            patiFilter.TREATMENT_ID = treatmentId;
                            patiFilter.ORDER_DIRECTION = "DESC";
                            patiFilter.ORDER_FIELD = "LOG_TIME";

                            var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patiFilter, param);
                            if (patientTypeAlter != null && patientTypeAlter.Count > 0)
                            {
                                patyAlter = patientTypeAlter.OrderByDescending(o => o.LOG_TIME).ToList().FirstOrDefault();
                            }
                            else
                            {
                                patyAlter.HEIN_CARD_NUMBER = hisCardPatientSdo.HeinCardNumber;
                                patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }
                        }
                        else
                        {
                            patyAlter.HEIN_CARD_NUMBER = hisCardPatientSdo.HeinCardNumber;
                            patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        //SERVICE_REQ
                        getServiceReq(treatmentId);

                        //SER_SERE
                        if (ServiceReqPrint != null)
                        {
                            getSereServs(this.ServiceReqPrint);
                        }
                    }
                }
                else
                {
                    if (examServiceReqRegisterResultSDO != null)
                    {
                        treatmentPrint = examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment;
                        var patientTypeId = string.IsNullOrEmpty(hisCardPatientSdo.HeinCardNumber) ? GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")) : GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                        var patientType = lstPatientType.FirstOrDefault(o => o.ID == patientTypeId);
                        //BHYT
                        CommonParam param = new CommonParam();
                        if (examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter != null && hisCardPatientSdo.HeinCardNumber != null)
                        {
                            HisPatientTypeAlterViewFilter patiFilter = new HisPatientTypeAlterViewFilter();
                            patiFilter.ID = examServiceReqRegisterResultSDO.HisPatientProfile.HisPatientTypeAlter.ID;

                            var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patiFilter, param);
                            if (patientTypeAlter != null && patientTypeAlter.Count > 0)
                            {
                                patyAlter = patientTypeAlter.FirstOrDefault();
                            }
                            else
                            {
                                patyAlter.HEIN_CARD_NUMBER = hisCardPatientSdo.HeinCardNumber;
                                patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }
                        }
                        else
                        {
                            patyAlter.HEIN_CARD_NUMBER = hisCardPatientSdo.HeinCardNumber;
                            patyAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        this.sereServs = examServiceReqRegisterResultSDO.SereServs;
                        this.ServiceReqPrint = examServiceReqRegisterResultSDO.ServiceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).OrderBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).FirstOrDefault(); ;
                    }
                }

                if (ServiceReqPrint != null && sereServs != null && sereServs.Count > 0)
                {
                    if (treatmentPrint != null && treatmentPrint.HAS_CARD == 1)
                    {
                        List<HIS_CARD> hisCardList = new List<HIS_CARD>();
                        HisCardFilter cardfilter = new HisCardFilter();
                        cardfilter.PATIENT_ID = treatmentPrint.PATIENT_ID;
                        hisCardList = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cardfilter, null);
                        if (hisCardList != null && hisCardList.Count > 0)
                        {
                            PrintKiosk printKiosk = new PrintKiosk(patyAlter, ServiceReqPrint, sereServs, (DelegateReturnSuccess)DelegateSuccess, printTypeCode, fileName, treatmentPrint, null, null, null, this.checkPrint, hisCardList);
                            printKiosk.RunPrintHasCard();
                        }
                    }
                    else
                    {
                        PrintKiosk printKiosk = new PrintKiosk(patyAlter, ServiceReqPrint, sereServs, (DelegateReturnSuccess)DelegateSuccess, printTypeCode, fileName, treatmentPrint, null, null, null, this.checkPrint);
                        printKiosk.RunPrint();

                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        public void DelegateSuccess(bool success)
        {
            try
            {
                if (success)
                {
                    this.checkPrint = !success;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnThanhtoan_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                CommonParam param = new CommonParam();
                List<MOS.SDO.WorkPlaceSDO> glstWorkPlaceSDO = WorkPlace.WorkPlaceSDO;
                List<long> glstCashierRoomID = glstWorkPlaceSDO.Select(sdo => sdo.CashierRoomId ?? 0).ToList();
                WaitingManager.Show();
                //Check phòng thu ngân
                List<HIS_CASHIER_ROOM> glstCashierRoom = new List<HIS_CASHIER_ROOM>();
                HisCashierRoomFilter casherfilter = new HisCashierRoomFilter();
                casherfilter.IDs = glstCashierRoomID;
                glstCashierRoom = new BackendAdapter(param).Get<List<HIS_CASHIER_ROOM>>(HisRequestUriStore.HIS_CASHIER_ROOM_GET, ApiConsumers.MosConsumer, casherfilter, param);
                if (glstCashierRoom != null && glstCashierRoom.Count > 0)
                {
                    //Check hồ sơ đăng ký
                    if (this.examServiceReqRegisterResultSDO != null)
                        treatmentCurrentID = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment.ID;
                    else
                    {
                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.PATIENT_ID = hisCardPatientSdo.PatientId;
                        treatmentFilter.ORDER_DIRECTION = "DESC";
                        treatmentFilter.ORDER_FIELD = "CREATE_TIME";
                        var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                        if (treatments != null && treatments.Count > 0)
                            treatmentCurrentID = treatments.FirstOrDefault().ID;
                    }
                    List<object> listArgs = new List<object>();
                    listArgs.Add(glstCashierRoom.FirstOrDefault());
                    listArgs.Add(treatmentCurrentID);
                    listArgs.Add((HIS.Desktop.Common.DelegateCloseForm_Uc)CloseForm);
                    if (this.currentModule != null)
                    {
                        CallModule callModule = new CallModule(CallModule.TransactionBill, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.TransactionBill, 0, 0, listArgs);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cần đăng nhập vào phòng thu ngân để thực hiện chức năng", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CloseForm(object data)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTamthu_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                CommonParam param = new CommonParam();
                List<MOS.SDO.WorkPlaceSDO> glstWorkPlaceSDO = WorkPlace.WorkPlaceSDO;
                List<long> glstCashierRoomID = glstWorkPlaceSDO.Select(sdo => sdo.CashierRoomId ?? 0).ToList();
                WaitingManager.Hide();
                //Check phòng thu ngân
                List<HIS_CASHIER_ROOM> glstCashierRoom = new List<HIS_CASHIER_ROOM>();
                HisCashierRoomFilter filter = new HisCashierRoomFilter();
                filter.IDs = glstCashierRoomID;
                glstCashierRoom = new BackendAdapter(param).Get<List<HIS_CASHIER_ROOM>>(HisRequestUriStore.HIS_CASHIER_ROOM_GET, ApiConsumers.MosConsumer, filter, param);
                if (glstCashierRoom != null && glstCashierRoom.Count > 0)
                {
                    //Check hồ sơ đăng ký
                    if (this.examServiceReqRegisterResultSDO != null)
                        treatmentCurrentID = this.examServiceReqRegisterResultSDO.HisPatientProfile.HisTreatment.ID;
                    else
                    {
                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.PATIENT_ID = hisCardPatientSdo.PatientId;
                        treatmentFilter.ORDER_DIRECTION = "DESC";
                        treatmentFilter.ORDER_FIELD = "CREATE_TIME";
                        var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                        if (treatments != null && treatments.Count > 0)
                            treatmentCurrentID = treatments.FirstOrDefault().ID;
                    }
                    List<object> listArgs = new List<object>();
                    listArgs.Add(glstCashierRoom.FirstOrDefault());
                    listArgs.Add(treatmentCurrentID);
                    listArgs.Add((HIS.Desktop.Common.DelegateCloseForm_Uc)CloseForm);
                    if (this.currentModule != null)
                    {
                        CallModule callModule = new CallModule(CallModule.DepositService, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.DepositService, 0, 0, listArgs);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cần đăng nhập vào phòng thu ngân để thực hiện chức năng", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }

                        if (loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate () { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
                Inventec.Common.Logging.LogSystem.Info("ResetLoopCount");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkPriority_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRecieveLater_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChronic_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string captionCode, string name, string captionName, string value, object data)
        {
            try
            {
                comboEdit.Properties.DataSource = data;
                comboEdit.Properties.DisplayMember = name;
                comboEdit.Properties.ValueMember = value;
                comboEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboEdit.Properties.ImmediatePopup = true;
                comboEdit.Properties.View.Columns.Clear();

                GridColumn aColumnCode = comboEdit.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = captionCode;
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;
                aColumnCode.AppearanceCell.FontSizeDelta = 7;
                aColumnCode.AppearanceHeader.FontSizeDelta = 7;

                GridColumn aColumnName = comboEdit.Properties.View.Columns.AddField(name);
                aColumnName.Caption = captionName;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.FontSizeDelta = 7;
                aColumnName.AppearanceHeader.FontSizeDelta = 7;

                comboEdit.Properties.View.ColumnPanelRowHeight = 30;
                comboEdit.Properties.View.RowHeight = 30;
                comboEdit.Properties.PopupFormSize = new Size(600, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
