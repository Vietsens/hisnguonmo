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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.InfusionCreate.ADO;
using HIS.Desktop.Plugins.InfusionCreate.Config;
using HIS.Desktop.Plugins.InfusionCreate.Enum;
using HIS.Desktop.Plugins.InfusionCreate.Validation;
using HIS.Desktop.Print;
using HIS.UC.ServiceUnit;
using HIS.UC.ServiceUnit.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfusionCreate
{

    public partial class frmInfusionCreate : HIS.Desktop.Utility.FormBase
    {
        private ServiceUnitProcessor serviceUnitProcessor = null;
        CommonParam param = new CommonParam();
        Inventec.Desktop.Common.Modules.Module moduleData;
        InfusionCreateADO data;
        List<V_HIS_INFUSION> ListInfusion = new List<V_HIS_INFUSION>();
        List<V_HIS_INFUSION> listInfusionsSelected = new List<V_HIS_INFUSION>();
        List<V_HIS_SERE_SERV_9> thissereServ = new List<V_HIS_SERE_SERV_9>();
        List<HIS_SERVICE_REQ> glstServiceReq = new List<HIS_SERVICE_REQ>();
        List<HIS_SERE_SERV> glstSereServ = new List<HIS_SERE_SERV>();
        List<V_HIS_MEDICINE> glstMedicine = new List<V_HIS_MEDICINE>();
        List<ACS_USER> acsUsers = new List<ACS_USER>();
        List<MEDITYPE> glstMediType = new List<MEDITYPE>();
        List<MEDITYPE> glstMediTypeSort = new List<MEDITYPE>();
        List<HIS_EMR_DOCUMENT_STT> lstEmrDocumentStt = new List<HIS_EMR_DOCUMENT_STT>();
        List<ComboSelectMedicineADO> lstAdo = new List<ComboSelectMedicineADO>();
        List<ComboSelectMedicineADO> lstAdoTemp = new List<ComboSelectMedicineADO>();
        List<V_HIS_EXP_MEST_MEDICINE_6> lstExpMestMedicine6 = new List<V_HIS_EXP_MEST_MEDICINE_6>();
        //List<HIS_MEDICINE_TYPE> glstMedicineType = new List<HIS_MEDICINE_TYPE>();
        List<HIS_SPEED_UNIT> glstSpeedUnit = new List<HIS_SPEED_UNIT>();
        bool isNotLoadWhileChangeControlStateInFirst;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.InfusionCreate";
        SDA_CONFIG_APP _currentConfigApp;
        SDA_CONFIG_APP_USER currentConfigAppUser;
        ConfigADO _ConfigADO;

        int positionHandleControl = -1;
        internal int ActionType = GlobalVariables.ActionAdd;
        long IDUpdate = 0;
        private UserControl ucServiceUnit = null;
        int flat = 0;
        private V_HIS_TREATMENT_2 treatment = null;
        HisInfusionSDO infusion = null;

        long idCombo = 0;

        public frmInfusionCreate(Inventec.Desktop.Common.Modules.Module moduleData, InfusionCreateADO data)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.data = data;
            InitUcServiceUnit();

        }

        private void InitUcServiceUnit()
        {
            try
            {
                serviceUnitProcessor = new ServiceUnitProcessor();
                ServiceUnitInitADO ado = new ServiceUnitInitADO();
                ado.DelegateNextFocus = spinSoluong;
                ado.Width = 440;
                ado.Height = 24;
                ado.WidthCustomLayout = 120;
                ado.DataServiceUnits = BackendDataWorker.Get<HIS_SERVICE_UNIT>();

                this.ucServiceUnit = (UserControl)serviceUnitProcessor.Run(ado);

                if (this.ucServiceUnit != null)
                {
                    this.panelControlUCUnitService.Controls.Add(this.ucServiceUnit);
                    this.ucServiceUnit.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoluong()
        {
            if (spinAmount.Enabled = true)
            {
                spinAmount.Focus();
                spinAmount.SelectAll();
            }
            else
            {

            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            bool notHandler = false;
            try
            {

                V_HIS_INFUSION dataInfusion = (V_HIS_INFUSION)gridView1.GetFocusedRow();

                if (dataInfusion != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataInfusion), dataInfusion));

                    if (!String.IsNullOrEmpty(dataInfusion.EMR_DOCUMENT_CODE))
                    {
                        //Kiểm tra xem văn bản có tồn tại file ký không
                        EMR.Filter.EmrDocumentViewFilter filter = new EMR.Filter.EmrDocumentViewFilter();
                        filter.DOCUMENT_CODE__EXACT = dataInfusion.EMR_DOCUMENT_CODE;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                        var apiResult = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, new CommonParam());
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            if (apiResult.Any(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE))
                            {
                                XtraMessageBox.Show(String.Format("Tồn tại văn bản đã ký (mã: {0})", dataInfusion.EMR_DOCUMENT_CODE), "Thông báo");
                                return;
                            }
                        }
                    }

                    if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();

                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_INFUSION_DELETE, ApiConsumers.MosConsumer, dataInfusion.ID, null);
                        WaitingManager.Hide();
                        if (success)
                        {
                            Loaddatatogrid();

                        }

                        WaitingManager.Hide();


                    }
                    else
                    {
                        notHandler = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }

            if (!notHandler)
            {
                #region Show message
                MessageManager.Show(this, param, success);
                param = new CommonParam();
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
        }

        private void frmInfusionCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();
                this.ValidControl();
                Loaddatatogrid();
                acsUsers = BackendDataWorker.Get<ACS_USER>();
                dtNgayChidinh.EditValue = DateTime.Now;
                LoadDataComboUser(this.cboReqUsername, this.cboExeUsername);
                LoadDatatoCombo(this.cboMedicineType, dtNgayChidinh.DateTime);
                LoadComboSpeedUnit();
                SetDefaultValue();
                this.SetDefaultDataToInfusionMedicine();
                LoadConfigHisAcc();
                InitMenuToButtonPrint();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultDataToInfusionMedicine()
        {
            try
            {
                if (grdInfusionMedicine == null
                   || grdInfusionMedicine.DataSource == null)
                {
                    List<ComboSelectMedicineADO> lstSelectMedicineADO = new List<ComboSelectMedicineADO>();

                    ComboSelectMedicineADO selectMedicineADO = new ComboSelectMedicineADO();
                    selectMedicineADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    lstSelectMedicineADO.Add(selectMedicineADO);
                    grdInfusionMedicine.DataSource = lstSelectMedicineADO;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboSelectMedicine()
        {
            try
            {
                lstAdo = new List<ComboSelectMedicineADO>();
                HisExpMestMedicineView6Filter DS1Filter = new HisExpMestMedicineView6Filter();
                DS1Filter.TDL_TREATMENT_ID = this.data.treatmentId;

                if (dtDateInfusion.EditValue != null)
                {
                    DS1Filter.TDL_INTRUCTION_DATE__EQUAL = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDateInfusion.DateTime.Date) ?? 0;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DS1Filter____________________", DS1Filter));
                var lstExpMestMedicine6 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE_6>>("api/HisExpMestMedicine/GetView6", ApiConsumers.MosConsumer, DS1Filter, null);
                List<long> lstPresriptionId = new List<long>();
                if (lstExpMestMedicine6 != null && lstExpMestMedicine6.Count() > 0)
                {
                    lstPresriptionId = lstExpMestMedicine6.Where(o => o.PRESCRIPTION_ID != null && o.PRESCRIPTION_ID > 0).Select(o => o.PRESCRIPTION_ID ?? 0).ToList();
                    foreach (var item in lstExpMestMedicine6)
                    {
                        ComboSelectMedicineADO ado = new ComboSelectMedicineADO(item);
                        lstAdo.Add(ado);
                    }
                }

                HisServiceReqMetyViewFilter DS2Filter = new HisServiceReqMetyViewFilter();
                DS2Filter.TREATMENT_ID = this.data.treatmentId;
                if (dtDateInfusion.EditValue != null)
                {
                    DS2Filter.INTRUCTION_DATE__EQUAL = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDateInfusion.DateTime.Date) ?? 0;
                }
                DS2Filter.SERVICE_REQ_ID__NOT_INs = lstPresriptionId;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DS2Filter____________________", DS2Filter));
                var lstServiceReqMety = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/GetView", ApiConsumers.MosConsumer, DS2Filter, null);
                if (lstServiceReqMety != null && lstServiceReqMety.Count() > 0)
                {
                    foreach (var item in lstServiceReqMety)
                    {
                        ComboSelectMedicineADO ado = new ComboSelectMedicineADO(item);
                        lstAdo.Add(ado);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstAdo________2____________", lstAdo));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("glstMediType________2____________", glstMediType));
                lstAdoTemp = lstAdo;
                if (cboMedicineType.EditValue != null)
                {
                    var department = glstMediType.Where(f => f.ID.Equals(cboMedicineType.EditValue)).FirstOrDefault();
                    if (lstAdo != null && lstAdo.Count > 0)
                    {
                        lstAdo = lstAdo.Where(o => o.MEDICINE_TYPE_CODE != department.MEDICINE_TYPE_CODE).ToList();
                    }
                }
                InitCombo(lstAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo(List<ComboSelectMedicineADO> lstAdo)
        {
            try
            {
                if (lstAdo.Count > 0)
                {
                    long index = -1;
                    List<ComboSelectMedicineADO> listAdoUpdate = lstAdo.Select(obj =>
                    {
                        if (obj.MEDICINE_TYPE_ID == null)
                        {
                            obj.MEDICINE_TYPE_ID = index;
                            index--;
                        }
                        return obj;
                    }).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên", 150, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_CODE", "MEDICINE_TYPE_ID", columnInfos, true, 200);
                    controlEditorADO.ImmediatePopup = true;
                    ControlEditorLoader.Load(cboSelectMedicine, listAdoUpdate, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlAmount();
                ValidControlTime();
                ValidationControlMaxLength(txtNote, 1000, false);
                ValidationControlMaxLength(txtMedicine, 500, true);
                ValidationControlMaxLength(txtPackageNumber, 100, false);



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlMedicine()
        {
            try
            {
                MedicineValidationRule MedicineRule = new MedicineValidationRule();
                MedicineRule.cboMedicineType = this.cboMedicineType;

                dxValidationProvider1.SetValidationRule(this.txtMedicinetype, MedicineRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlAmount()
        {
            try
            {
                AmountValidationRule Amount = new AmountValidationRule();
                Amount.spinAmount = this.spinAmount;
                dxValidationProvider1.SetValidationRule(this.spinAmount, Amount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlTime()
        {
            try
            {
                TimeValidationRule Time = new TimeValidationRule();
                Time.DateEdit1 = this.dtStartTime;
                Time.DateEdit2 = this.dtEndTime;
                dxValidationProvider1.SetValidationRule(this.dtEndTime, Time);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SetDefaultValue()
        {
            try
            {
                txtMedicine.Enabled = true;
                txtPackageNumber.Enabled = true;
                dtExpiredDate.Enabled = true;
                cboMedicineType.EditValue = null;
                spinAmount.EditValue = null;
                txtMedicinetype.Text = "";
                txtNote.Text = "";
                this.dtExpiredDate.EditValue = null;
                this.dtStartTime.DateTime = DateTime.Now;
                this.dtEndTime.DateTime = DateTime.Now;
                this.dtDateInfusion.DateTime = DateTime.Now;
                cboReqUsername.EditValue = null;
                cboExeUsername.EditValue = null;
                spinSpeed.EditValue = null;
                txtMedicine.Text = "";
                txtPackageNumber.Text = "";
                serviceUnitProcessor.Reload(ucServiceUnit, null);
                cboSpeedUnit.EditValue = null;
                spinEditConvertVolumnRatio.EditValue = null;
                spinEditVolumn.EditValue = null;
                spinEditConvertVolumnRatio.Enabled = true;
                dxErrorProvider1.ClearErrors();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                chkSignForPrint.Checked = false;
                chkPrintDocumentSignedForPrint.Enabled = false;
                chkPrintDocumentSignedForPrint.Checked = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //Load combo Người chỉ định và người 
        private void LoadDataComboUser(DevExpress.XtraEditors.GridLookUpEdit lookUpEdit5, DevExpress.XtraEditors.GridLookUpEdit lookUpEdit6)
        {

            //Người chỉ định
            HisUserRoomViewFilter filterUserRoom = new HisUserRoomViewFilter();
            // filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            filterUserRoom.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            filterUserRoom.ROOM_ID = this.moduleData.RoomId;
            var thisUserRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_USER_ROOM>>(HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumers.MosConsumer, filterUserRoom, null);
            List<string> _loginNames = new List<string>();
            _loginNames = thisUserRoom.Select(p => p.LOGINNAME).ToList();
            //lookUpEdit5.Properties.DataSource = acsUsers.Where(o => _loginNames.Contains(o.LOGINNAME)).OrderBy(p => p.LOGINNAME).ToList();
            var data5s = acsUsers.Where(o => _loginNames.Contains(o.LOGINNAME)).OrderBy(p => p.LOGINNAME).ToList();

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("LOGINNAME", "LOGINNAME", 50, 1));
            columnInfos.Add(new ColumnInfo("USERNAME", "USERNAME", 150, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 200);
            ControlEditorLoader.Load(lookUpEdit5, data5s, controlEditorADO);

            List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
            columnInfos2.Add(new ColumnInfo("LOGINNAME", "LOGINNAME", 50, 1));
            columnInfos2.Add(new ColumnInfo("USERNAME", "USERNAME", 150, 2));
            ControlEditorADO controlEditorADO2 = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos2, true, 200);
            ControlEditorLoader.Load(lookUpEdit6, acsUsers, controlEditorADO2);

        }

        private void LoadComboSpeedUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSpeedUnitFilter filter = new HisSpeedUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SPEED_UNIT>>("api/HisSpeedUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                if (data != null && data.Count > 0)
                    glstSpeedUnit.AddRange(data);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SPEED_UNIT_CODE", "", 30, 1));
                columnInfos.Add(new ColumnInfo("SPEED_UNIT_NAME", "", 70, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SPEED_UNIT_NAME", "ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cboSpeedUnit, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load data combo thuốc trong/ngoài kho
        private List<MEDITYPE> FillDataCombo()
        {
            try
            {
                lstExpMestMedicine6 = new List<V_HIS_EXP_MEST_MEDICINE_6>();
                List<MEDITYPE> glstMedi = new List<MEDITYPE>();
                ////Thuốc trong kho
                HisExpMestMedicineView6Filter filter = new HisExpMestMedicineView6Filter();
                filter.TDL_TREATMENT_ID = this.data.treatmentId;
                if (dtNgayChidinh.EditValue != null)
                {
                    long ngayChiDinh = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayChidinh.DateTime.Date) ?? 0;
                    filter.TDL_INTRUCTION_DATE_FROM = ngayChiDinh;
                    filter.TDL_INTRUCTION_DATE_TO = ngayChiDinh;
                }
                else
                {
                    filter.TDL_INTRUCTION_DATE_FROM = null;
                    filter.TDL_INTRUCTION_DATE_TO = null;
                }

                lstExpMestMedicine6 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_6>>("api/HisExpMestMedicine/GetView6", ApiConsumers.MosConsumer, filter, null);

                foreach (V_HIS_EXP_MEST_MEDICINE_6 exp in lstExpMestMedicine6)
                {
                    MEDITYPE medi = new MEDITYPE();
                    medi.ID = exp.ID;
                    medi.INSTRUCTION_TIME = exp.TDL_INTRUCTION_TIME;
                    medi.MEDICINE_ID = exp.MEDICINE_ID;
                    medi.MEDICINE_TYPE_CODE = exp.MEDICINE_TYPE_CODE;
                    medi.MEDICINE_TYPE_NAME = exp.MEDICINE_TYPE_NAME;
                    medi.AMOUNT = exp.AMOUNT;
                    medi.SPEED = exp.SPEED;
                    medi.PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                    medi.SERVICE_UNIT_ID = exp.SERVICE_UNIT_ID;
                    medi.SERVICE_UNIT_NAME = exp.SERVICE_UNIT_NAME;
                    medi.EXPIRED_DATE = exp.EXPIRED_DATE;
                    medi.LOGGINNAME = exp.REQ_LOGINNAME;
                    medi.ngoaikho = false;
                    glstMedi.Add(medi);
                }

                //////Lấy thuốc kê ngoài

                List<V_HIS_SERVICE_REQ_METY> glstMetyReq = null;
                List<V_HIS_EXP_MEST_MEDICINE_6> outMestMedicines = null;
                HisServiceReqMetyViewFilter filter2 = new HisServiceReqMetyViewFilter();
                filter2.TREATMENT_ID = this.data.treatmentId; ;
                if (dtNgayChidinh.EditValue != null)
                {
                    long ngayChiDinh = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayChidinh.DateTime.Date) ?? 0;
                    filter2.INTRUCTION_DATE_FROM = ngayChiDinh;
                    filter2.INTRUCTION_DATE_TO = ngayChiDinh;
                }
                else
                {
                    filter2.INTRUCTION_DATE_FROM = null;
                    filter2.INTRUCTION_DATE_TO = null;
                }
                glstMetyReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/GetView", ApiConsumers.MosConsumer, filter2, null);

                if (glstMetyReq != null && glstMetyReq.Count > 0)
                {
                    HisExpMestMedicineView6Filter hisExpMestMedicineView6Filter2 = new HisExpMestMedicineView6Filter();
                    hisExpMestMedicineView6Filter2.PRESCRIPTION_IDs = glstMetyReq.Select(s => s.SERVICE_REQ_ID).Distinct().ToList();
                    outMestMedicines = new BackendAdapter(this.param).Get<List<V_HIS_EXP_MEST_MEDICINE_6>>("api/HisExpMestMedicine/GetView6", ApiConsumers.MosConsumer, hisExpMestMedicineView6Filter2, null);
                }

                //Lấy thông tin MEDICINE_TYPE
                //HisMedicineTypeFilter filterMedicineType = new HisMedicineTypeFilter();
                //filterMedicineType.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //filterMedicineType.IDs = glstMetyReq.Select(mt => mt.MEDICINE_TYPE_ID ?? 0).ToList();
                //var glstMedicineType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filterMedicineType, null);
                if (outMestMedicines != null && outMestMedicines.Count > 0)
                {
                    foreach (V_HIS_EXP_MEST_MEDICINE_6 exp in outMestMedicines)
                    {
                        MEDITYPE mEDITYPE = new MEDITYPE();
                        mEDITYPE.ID = exp.ID;
                        mEDITYPE.INSTRUCTION_TIME = exp.TDL_INTRUCTION_TIME;
                        mEDITYPE.MEDICINE_ID = exp.MEDICINE_ID;
                        mEDITYPE.MEDICINE_TYPE_CODE = exp.MEDICINE_TYPE_CODE;
                        mEDITYPE.MEDICINE_TYPE_NAME = exp.MEDICINE_TYPE_NAME;
                        mEDITYPE.PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                        mEDITYPE.SERVICE_UNIT_ID = exp.SERVICE_UNIT_ID;
                        mEDITYPE.SERVICE_UNIT_NAME = exp.SERVICE_UNIT_NAME;
                        mEDITYPE.EXPIRED_DATE = exp.EXPIRED_DATE;
                        mEDITYPE.LOGGINNAME = exp.REQ_LOGINNAME;
                        mEDITYPE.AMOUNT = exp.AMOUNT;
                        mEDITYPE.ngoaikho = true;
                        V_HIS_SERVICE_REQ_METY v_HIS_SERVICE_REQ_METY = glstMetyReq.FirstOrDefault(o => o.MEDICINE_TYPE_ID == exp.MEDICINE_TYPE_ID && o.SERVICE_REQ_ID == exp.PRESCRIPTION_ID);
                        if (v_HIS_SERVICE_REQ_METY != null)
                        {
                            mEDITYPE.SPEED = v_HIS_SERVICE_REQ_METY.SPEED;
                        }
                        else
                        {
                            mEDITYPE.SPEED = exp.SPEED;
                        }
                        glstMedi.Add(mEDITYPE);
                    }
                }
                if (glstMetyReq != null && glstMetyReq.Count > 0)
                {
                    foreach (V_HIS_SERVICE_REQ_METY serq in glstMetyReq)
                    {
                        bool hasSale = false;
                        if (serq.MEDICINE_TYPE_ID.HasValue && outMestMedicines != null)
                        {
                            hasSale = outMestMedicines.Any(a => a.PRESCRIPTION_ID == serq.SERVICE_REQ_ID && a.MEDICINE_TYPE_ID == serq.MEDICINE_TYPE_ID);
                        }

                        if (!hasSale)
                        {
                            MEDITYPE mediType = new MEDITYPE();
                            mediType.INSTRUCTION_TIME = serq.INTRUCTION_TIME;
                            mediType.LOGGINNAME = serq.REQUEST_LOGINNAME;
                            mediType.ID = serq.ID;

                            mediType.MEDICINE_TYPE_NAME = serq.MEDICINE_TYPE_NAME;
                            mediType.AMOUNT = serq.AMOUNT;
                            mediType.SPEED = serq.SPEED;
                            mediType.SERVICE_UNIT_NAME = serq.UNIT_NAME;
                            long id = serq.MEDICINE_TYPE_ID ?? 0;
                            if (id > 0)
                            {
                                HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(md => md.ID == id);
                                if (medicineType != null)
                                {
                                    mediType.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                    mediType.SERVICE_UNIT_ID = medicineType.TDL_SERVICE_UNIT_ID;
                                    mediType.EXPIRED_DATE = mediType.EXPIRED_DATE;
                                    mediType.MEDICINE_ID = mediType.MEDICINE_ID;

                                }
                            }
                            mediType.ngoaikho = true;
                            glstMedi.Add(mediType);
                        }

                    }
                }

                if (glstMedi != null)
                {
                    glstMedi = (from m in glstMedi
                                group m by new { m.ID, m.MEDICINE_ID, m.MEDICINE_TYPE_CODE, m.MEDICINE_TYPE_NAME, m.PACKAGE_NUMBER, m.SERVICE_UNIT_ID, m.EXPIRED_DATE, m.SPEED, m.SERVICE_UNIT_NAME, m.INSTRUCTION_TIME, m.LOGGINNAME, m.INSTRUCTION_DATE_STR, m.ngoaikho } into g
                                select new MEDITYPE
                                {
                                    ID = g.Key.ID,
                                    MEDICINE_ID = g.Key.MEDICINE_ID,
                                    MEDICINE_TYPE_CODE = g.Key.MEDICINE_TYPE_CODE,
                                    MEDICINE_TYPE_NAME = g.Key.MEDICINE_TYPE_NAME,
                                    PACKAGE_NUMBER = g.Key.PACKAGE_NUMBER,
                                    SERVICE_UNIT_ID = g.Key.SERVICE_UNIT_ID,
                                    SERVICE_UNIT_NAME = g.Key.SERVICE_UNIT_NAME,
                                    INSTRUCTION_TIME = g.Key.INSTRUCTION_TIME,
                                    LOGGINNAME = g.Key.LOGGINNAME,
                                    EXPIRED_DATE = g.Key.EXPIRED_DATE,
                                    AMOUNT = g.Sum(md => md.AMOUNT),
                                    SPEED = g.Key.SPEED,
                                    ngoaikho = g.Key.ngoaikho

                                }).ToList<MEDITYPE>();
                }

                foreach (MEDITYPE medi in glstMedi)
                {
                    if (medi.EXPIRED_DATE > 0)
                        medi.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(medi.EXPIRED_DATE.ToString()));
                    if (medi.INSTRUCTION_TIME > 0)
                    {
                        medi.INSTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(medi.INSTRUCTION_TIME.ToString()));
                    }
                }

                if (glstMedi != null)
                    glstMedi.Sort((emp1, emp2) => Convert.ToDateTime(emp2.INSTRUCTION_TIME_STR).CompareTo(Convert.ToDateTime(emp1.INSTRUCTION_TIME_STR)));
                return glstMedi;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

        }

        //Load conbo thuốc trong/ngoài kho
        private void LoadDatatoCombo(DevExpress.XtraEditors.GridLookUpEdit lookUpEdit1, DateTime ngaychidinh)
        {
            try
            {
                glstMediType = FillDataCombo();
                lookUpEdit1.Properties.DataSource = glstMediType;
                lookUpEdit1.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                lookUpEdit1.Properties.ValueMember = "ID";
                gridLookUpEdit1View.RowStyle += gridLookUpEdit1View_RowStyle;
                lookUpEdit1.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                lookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                lookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                lookUpEdit1.Properties.ImmediatePopup = true;
                lookUpEdit1.ForceInitialize();
                //lookUpEdit1.Properties.View.Columns.Clear();
                lookUpEdit1.Properties.View.OptionsView.ShowColumnHeaders = true;

                GridColumn aColumnIntructionTime = lookUpEdit1.Properties.View.Columns.AddField("INSTRUCTION_TIME_STR");
                aColumnIntructionTime.Caption = "Thời gian y lệnh";
                aColumnIntructionTime.Visible = true;
                aColumnIntructionTime.VisibleIndex = 1;
                aColumnIntructionTime.Width = 120;

                GridColumn aColumnCode = lookUpEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 2;
                aColumnCode.Width = 50;

                GridColumn aColumnName = lookUpEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 3;
                aColumnName.Width = 100;

                GridColumn aColumnPackageNumber = lookUpEdit1.Properties.View.Columns.AddField("PACKAGE_NUMBER");
                aColumnPackageNumber.Caption = "Số lô";
                aColumnPackageNumber.Visible = true;
                aColumnPackageNumber.VisibleIndex = 4;
                aColumnPackageNumber.Width = 40;


                GridColumn aColumnExpiredDate = lookUpEdit1.Properties.View.Columns.AddField("EXPIRED_DATE_STR");
                aColumnExpiredDate.Caption = "Hạn sử dụng";
                aColumnExpiredDate.Visible = true;
                aColumnExpiredDate.VisibleIndex = 5;
                aColumnExpiredDate.Width = 120;


                GridColumn aColumnSpeed = lookUpEdit1.Properties.View.Columns.AddField("SPEED");
                aColumnSpeed.Caption = "Tốc độ truyền";
                aColumnSpeed.Visible = true;
                aColumnSpeed.VisibleIndex = 6;
                aColumnSpeed.Width = 50;

                GridColumn aColumnLoginName = lookUpEdit1.Properties.View.Columns.AddField("LOGGINNAME");
                aColumnLoginName.Caption = "Người chỉ định";
                aColumnLoginName.Visible = true;
                aColumnLoginName.VisibleIndex = 7;
                aColumnLoginName.Width = 50;




                //SetDefaultControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void gridLookUpEdit1View_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                var data = (MEDITYPE)gridLookUpEdit1View.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.ngoaikho)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Loaddatatogrid()
        {
            try
            {
                HisInfusionViewFilter Infusionfilter = new HisInfusionViewFilter();
                Infusionfilter.INFUSION_SUM_ID = data.InfusionSumId;
                Infusionfilter.ORDER_FIELD = "MODIFY_TIME";
                Infusionfilter.ORDER_DIRECTION = "DESC";
                ListInfusion = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_INFUSION>>(HisRequestUriStore.HIS_INFUSION_GETVIEW, ApiConsumers.MosConsumer, Infusionfilter, param);
                if (ListInfusion != null)
                {
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = ListInfusion;
                    //Lấy thông tin medicine type
                    HisMedicineViewFilter filterMedicine = new HisMedicineViewFilter();
                    filterMedicine.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filterMedicine.IDs = ListInfusion.Select(mt => mt.MEDICINE_ID ?? 0).ToList();
                    glstMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filterMedicine, null);
                    gridControl1.EndUpdate();


                }




            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        //e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "START_TIME_ST")
                    {
                        try
                        {
                            string StartTime = (view.GetRowCellValue(e.ListSourceRowIndex, "START_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(StartTime));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao START_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "FINISH_TIME_ST")
                    {
                        try
                        {
                            string FinishTime = (view.GetRowCellValue(e.ListSourceRowIndex, "FINISH_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(FinishTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            string ExpriredDate = (view.GetRowCellValue(e.ListSourceRowIndex, "EXPIRED_DATE") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(ExpriredDate));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXPIRED_DATE", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MEDICINE_TYPE_CODE_STR")
                    {
                        try
                        {
                            long mediID = Convert.ToInt64(view.GetRowCellValue(e.ListSourceRowIndex, "MEDICINE_ID") ?? 0);
                            if (mediID > 0)
                            {
                                if (glstMedicine != null && glstMedicine.Count > 0)
                                {
                                    e.Value = glstMedicine.Where(me => me.ID == mediID).FirstOrDefault().MEDICINE_TYPE_CODE;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MEDICINE_TYPE_CODE", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EMR_DOCUMENT_STT_NAME_str")
                    {
                        e.Value = view.GetRowCellValue(e.ListSourceRowIndex, "EMR_DOCUMENT_STT_NAME").ToString();
                    }



                }
                //gridControlPatientList.RefreshDataSource();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridView1.PostEditor();
            CommonParam paramCommon = new CommonParam();
            bool success = false;

            try
            {

                if (!btnSave.Enabled && this.ActionType == GlobalVariables.ActionView) return;
                bool isValid = (bool)serviceUnitProcessor.ValidationServiceUnit(ucServiceUnit);
                this.positionHandleControl = -1;

                if (!dxValidationProvider1.Validate() && !isValid)
                    return;
                this.ValidControl();
                if (spinSpeed.EditValue != null && cboSpeedUnit.EditValue == null)
                {
                    ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                    validate.ErrorText = "Nhập tốc độ truyền thì bắt buộc nhập đơn vị tốc độ ";
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(spinSpeed, validate);
                }
                if (spinSpeed.EditValue != null && cboSpeedUnit.EditValue == null)
                {
                    dxErrorProvider1.SetError(spinSpeed, "Nhập tốc độ truyền thì bắt buộc nhập đơn vị tốc độ", ErrorType.Warning);
                    return;
                }
                else if (spinSpeed.EditValue == null)
                {
                    dxErrorProvider1.ClearErrors();
                }


                var lstDataCheck = grdInfusionMedicine.DataSource as List<ComboSelectMedicineADO>;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDataCheck), lstDataCheck));
                if (lstDataCheck != null && lstDataCheck.Count > 0)
                {
                    var dataGridMaxlengthMedicineTypeName = lstDataCheck.Where(o => !String.IsNullOrEmpty(o.MEDICINE_TYPE_NAME) && Encoding.UTF8.GetByteCount(o.MEDICINE_TYPE_NAME) > 500);
                    if (dataGridMaxlengthMedicineTypeName != null && dataGridMaxlengthMedicineTypeName.Count() > 0)
                    {
                        MessageManager.Show(String.Join(", ", dataGridMaxlengthMedicineTypeName.Select(o => o
                            .MEDICINE_TYPE_NAME).ToList()) + " tên thuốc vượt quá độ dài cho phép [500 kí tự]");
                        return;
                    }

                    var dataGridMaxlengthDes = lstDataCheck.Where(o => !String.IsNullOrEmpty(o.PACKAGE_NUMBER) && Encoding.UTF8.GetByteCount(o.PACKAGE_NUMBER) > 100);
                    if (dataGridMaxlengthDes != null && dataGridMaxlengthDes.Count() > 0)
                    {
                        MessageManager.Show(String.Join(", ", dataGridMaxlengthDes.Select(o => o
                            .MEDICINE_TYPE_NAME).ToList()) + " số lô vượt quá độ dài cho phép [100 kí tự]");
                        return;
                    }
                }

                List<HIS_MIXED_MEDICINE> lstDataSend = new List<HIS_MIXED_MEDICINE>();

                foreach (var item in lstDataCheck)
                {
                    if (string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE) && string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME) && item.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                        continue;
                    HIS_MIXED_MEDICINE obj = new HIS_MIXED_MEDICINE();
                    obj.INFUSION_ID = item.INFUSION_ID;
                    obj.MEDICINE_ID = item.MEDICINE_ID;
                    obj.MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID > 0 ? item.MEDICINE_TYPE_ID : null;
                    obj.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                    obj.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                    obj.VOLUME = item.VOLUME ?? 0;
                    obj.AMOUNT = item.AMOUNT;
                    obj.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    lstDataSend.Add(obj);
                }


                HIS_INFUSION infusionsend = new HIS_INFUSION();
                infusionsend.INFUSION_SUM_ID = data.InfusionSumId;
                if (isValid)
                {
                    var serviceUnitData = this.serviceUnitProcessor.GetValue(this.ucServiceUnit);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceUnitData), serviceUnitData));
                    if (serviceUnitData != null && serviceUnitData is HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO)
                    {
                        if (((HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO)serviceUnitData).SERVICE_UNIT_ID != null)
                        {
                            infusionsend.SERVICE_UNIT_ID = ((HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO)serviceUnitData).SERVICE_UNIT_ID;
                            infusionsend.SERVICE_UNIT_NAME = ((HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO)serviceUnitData).SERVICE_UNIT_NAME;
                        }
                        else
                            infusionsend.SERVICE_UNIT_NAME = ((HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO)serviceUnitData).SERVICE_UNIT_NAME;
                    }
                }


                if (cboMedicineType.EditValue != null)
                {
                    var mdType = glstMediType.FirstOrDefault(md => md.ID == Int64.Parse(cboMedicineType.EditValue.ToString()));
                    if (mdType != null && mdType.MEDICINE_ID != null)
                    {
                        infusionsend.MEDICINE_ID = mdType.MEDICINE_ID;
                    }
                }
                var aMount = Inventec.Common.TypeConvert.Parse.ToDecimal(this.spinAmount.Value.ToString());
                if (this.spinAmount.Value <= 0)
                {
                    MessageManager.Show("Số lượng phải lớn hơn 0!");
                    return;
                }

                infusionsend.AMOUNT = aMount;

                if (this.spinSpeed.EditValue == null) infusionsend.SPEED = null;
                else
                {
                    if (spinSpeed.Value <= 0)
                    {
                        MessageManager.Show("Tốc độ phải lớn hơn 0!");
                        return;
                    }
                    else infusionsend.SPEED = spinSpeed.Value;
                }
                infusionsend.REQUEST_USERNAME = this.cboReqUsername.EditValue != null && this.cboReqUsername.Text != "" ? acsUsers.Where(o => o.LOGINNAME == this.cboReqUsername.EditValue.ToString()).First().USERNAME : " ";
                infusionsend.REQUEST_LOGINNAME = this.cboReqUsername.EditValue != null && this.cboReqUsername.Text != "" ? this.cboReqUsername.EditValue.ToString() : " ";
                infusionsend.EXECUTE_USERNAME = this.cboExeUsername.EditValue != null && this.cboExeUsername.Text != "" ? acsUsers.Where(o => o.LOGINNAME == this.cboExeUsername.EditValue.ToString()).First().USERNAME : " ";
                infusionsend.EXECUTE_LOGINNAME = this.cboExeUsername.EditValue != null && this.cboExeUsername.Text != "" ? this.cboExeUsername.EditValue.ToString() : " ";
                infusionsend.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStartTime.DateTime);
                //Convert.ToInt64(dateEdit1.DateTime.ToString("yyyyMMddhhmm") + "00");
                infusionsend.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                //Convert.ToInt64(dateEdit2.DateTime.ToString("yyyyMMddhhmm") + "00");
                if (!String.IsNullOrWhiteSpace(txtNote.Text))
                {
                    infusionsend.NOTE = txtNote.Text.Trim();
                }
                if (dtExpiredDate.EditValue != null)
                    infusionsend.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpiredDate.DateTime);
                if (!String.IsNullOrWhiteSpace(txtMedicine.Text))
                    infusionsend.MEDICINE_TYPE_NAME = txtMedicine.Text.Trim();
                if (!String.IsNullOrWhiteSpace(txtPackageNumber.Text))
                    infusionsend.PACKAGE_NUMBER = txtPackageNumber.Text.Trim();
                infusionsend.VOLUME = spinEditVolumn.Value;
                infusionsend.CONVERT_VOLUME_RATIO = spinEditConvertVolumnRatio.Value;
                if (cboSpeedUnit.EditValue != null)
                {
                    infusionsend.SPEED_UNIT_ID = Convert.ToInt64(cboSpeedUnit.EditValue);
                    var speedUnit = glstSpeedUnit.Where(sp => sp.ID == Convert.ToInt64(cboSpeedUnit.EditValue)).ToList();
                    if (speedUnit != null && speedUnit.Count > 0)
                    {
                        infusionsend.CONVERT_TIME_RATIO = speedUnit.FirstOrDefault().CONVERT_TIME_RATIO;

                    }
                }
                MOS.SDO.HisInfusionSDO sdo = new MOS.SDO.HisInfusionSDO();
                sdo.HisInfusion = infusionsend;
                sdo.HisMixedMedicines = lstDataSend;
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("INPUT DATA____________________", sdo));
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    infusion = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.HisInfusionSDO>(ApiConsumer.HisRequestUriStore.HIS_INFUSION_CREATE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                }
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    infusionsend.ID = IDUpdate;
                    infusion = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.HisInfusionSDO>("/api/HisInfusion/Update", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    if (infusion != null) this.ActionType = GlobalVariables.ActionAdd;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("infusion____________________", infusion));

                if (chkPrint.Checked)
                {
                    ProcessPrint();
                    PrintProcess();
                }
                if (infusion != null)
                {
                    success = true;
                    btnRefesh_Click(null, null);
                    serviceUnitProcessor.Reload(ucServiceUnit, null);
                    Loaddatatogrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                param = new CommonParam();
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

        private void PrintProcess()
        {
            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuTruyenDich_MPS000146, delegateRunPrintTemplte);
        }
        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.infusion.HisInfusion == null)
                    return result;

                List<V_HIS_INFUSION> listData = new List<V_HIS_INFUSION>();
                HisInfusionViewFilter filter = new HisInfusionViewFilter();
                filter.ID = this.infusion.HisInfusion.ID;
                var dt = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INFUSION>>("api/HisInfusion/GetView", ApiConsumers.MosConsumer, filter, null);
                listData.AddRange(dt);

                List<HIS_MIXED_MEDICINE> list = new List<HIS_MIXED_MEDICINE>();
                list.AddRange(this.infusion.HisMixedMedicines);

                HisInfusionSumViewFilter infuFilter = new HisInfusionSumViewFilter();
                infuFilter.ID = this.data.InfusionSumId;
                var data = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INFUSION_SUM>>("api/HisInfusionSum/GetView", ApiConsumers.MosConsumer, infuFilter, null);

                MOS.Filter.HisTreatmentBedRoomViewFilter filterBedRoom = new HisTreatmentBedRoomViewFilter();
                filterBedRoom.TREATMENT_ID = this.data.treatmentId;
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filterBedRoom, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.OrderByDescending(o => o.OUT_TIME.HasValue).ThenByDescending(o => o.ID).FirstOrDefault(o => o.BED_ID.HasValue);
                }
                HisTreatmentView2Filter treatFilter = new HisTreatmentView2Filter();
                treatFilter.ID = this.data.treatmentId;
                var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatFilter, null);
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.treatment = listTreatment.First();
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_EMR_DOCUMENT_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, "", (this.treatment != null ? this.treatment.TREATMENT_CODE : ""));
                    inputADO.DocumentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listData.Min(o => o.START_TIME) ?? 0);
                }


                MPS.Processor.Mps000146.PDO.Mps000146PDO rdo = new MPS.Processor.Mps000146.PDO.Mps000146PDO(
                    data.FirstOrDefault(),
                    this.treatment,
                    listData,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    _TreatmetnbedRoom,
                    list
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (chkSign.Checked)
                {
                    if (chkPrintDocumentSigned.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };

                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void cboMedicineType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMedicineType.EditValue = null;
                    txtMedicinetype.EditValue = null;
                    txtMedicine.EditValue = null;
                    txtMedicine.Enabled = true;
                    txtPackageNumber.Enabled = true;
                    dtExpiredDate.Enabled = true;
                    txtPackageNumber.EditValue = null;
                    dtExpiredDate.EditValue = null;
                    cboExeUsername.EditValue = null;
                    cboReqUsername.EditValue = null;
                    serviceUnitProcessor.Reload(ucServiceUnit, null);
                    spinAmount.EditValue = null;
                    dtNgayChidinh.DateTime = DateTime.Now;
                    glstMediType = FillDataCombo();
                    cboMedicineType.Properties.DataSource = glstMediType;
                    lstAdo = lstAdoTemp;


                    grdInfusionMedicine.DataSource = null;
                    SetDefaultDataToInfusionMedicine();
                    panelControlUCUnitService.Enabled = true;
                    spinAmount.Enabled = true;
                    dtDateInfusion.Enabled = true;
                    cboReqUsername.Enabled = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cboMedicineType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CheckMedicine();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckMedicine()
        {
            try
            {
                if (cboMedicineType.EditValue != null)
                {

                    var department = glstMediType.Where(f => f.ID == Int64.Parse(cboMedicineType.EditValue.ToString())).FirstOrDefault();
                    var medicine = lstExpMestMedicine6.Where(o => o.ID == Int64.Parse(cboMedicineType.EditValue.ToString())).FirstOrDefault();


                    if (medicine != null)
                    {
                        var medi = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(s => s.ID == medicine.MEDICINE_TYPE_ID);
                        if (medi != null) spinEditVolumn.EditValue = medi.VOLUME;
                        if (medicine.MIXED_INFUSION != null)
                        {
                            List<ComboSelectMedicineADO> lstSelectMedicineADO = new List<ComboSelectMedicineADO>();
                            var lstMedicine = lstExpMestMedicine6.Where(o => o.MIXED_INFUSION == medicine.MIXED_INFUSION && o.EXP_MEST_ID == medicine.EXP_MEST_ID);
                            foreach (var item in lstMedicine)
                            {
                                
                                ComboSelectMedicineADO ado = new ComboSelectMedicineADO(item);
                                if (item.ID != medicine.ID)
                                {
                                    if (ado.AMOUNT > 0)
                                    {
                                        ado.isDisableVolume = 1;
                                        ado.isDisableAmount = 0;
                                    }
                                    else if (ado.VOLUME > 0)
                                    {
                                        ado.isDisableVolume = 0;
                                        ado.isDisableAmount = 1;
                                    }
                                    
                                    lstSelectMedicineADO.Add(ado);
                                }

                            }
                            dtDateInfusion.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicine.TDL_INTRUCTION_DATE ?? 0) ?? DateTime.MinValue;
                            grdInfusionMedicine.DataSource = lstSelectMedicineADO;

                            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.InfusionCreate.AllowEditingMixedInfusionInfusion") != "1")
                            {
                                grvInfusionMedicine.OptionsBehavior.Editable = false;
                                panelControlUCUnitService.Enabled = false;
                                spinAmount.Enabled = false;
                                dtDateInfusion.Enabled = false;
                                cboReqUsername.Enabled = false;
                            }
                            else
                            {
                                grvInfusionMedicine.OptionsBehavior.Editable = true;
                                panelControlUCUnitService.Enabled = true;
                                spinAmount.Enabled = true;
                                dtDateInfusion.Enabled = true;
                                cboReqUsername.Enabled = true;
                            }
                        }
                        else
                        {
                            grvInfusionMedicine.OptionsBehavior.Editable = true;
                            grdInfusionMedicine.DataSource = null;
                            SetDefaultDataToInfusionMedicine();
                            dtDateInfusion.Enabled = true;
                            cboReqUsername.Enabled = true;
                            panelControlUCUnitService.Enabled = true;
                            spinAmount.Enabled = true;
                        }
                    }

                        if (department != null)
                        {
                            txtMedicinetype.Text = department.MEDICINE_TYPE_CODE;
                            txtMedicine.Text = department.MEDICINE_TYPE_NAME;
                            if (department.PACKAGE_NUMBER != null)
                                txtPackageNumber.Text = department.PACKAGE_NUMBER;
                            else
                                txtPackageNumber.EditValue = null;
                            spinAmount.EditValue = department.AMOUNT;
                            var medi = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(s => s.MEDICINE_TYPE_CODE == department.MEDICINE_TYPE_CODE);
                            if (medi != null) spinEditVolumn.EditValue = medi.VOLUME;
                            ServiceUnitInputADO serviceUnit = new ServiceUnitInputADO();
                            if (department.SERVICE_UNIT_ID != null && department.SERVICE_UNIT_ID > 0)
                            {
                                serviceUnit.SERVICE_UNIT_ID = department.SERVICE_UNIT_ID;
                                serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            }
                            else
                                serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            serviceUnitProcessor.Reload(ucServiceUnit, serviceUnit);
                            spinSpeed.EditValue = department.SPEED;
                            cboReqUsername.EditValue = department.LOGGINNAME;
                            dtNgayChidinh.EditValue = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)department.INSTRUCTION_TIME);
                            if (department.EXPIRED_DATE != null && department.EXPIRED_DATE > 0)
                                dtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)department.EXPIRED_DATE);
                            else
                                dtExpiredDate.EditValue = null;

                            if (department.ngoaikho == false)
                            {
                                txtMedicine.Enabled = false;
                                txtPackageNumber.Enabled = false;
                                dtExpiredDate.Enabled = false;

                            }
                            else
                            {
                                txtMedicine.Enabled = true;
                                txtPackageNumber.Enabled = true;
                                dtExpiredDate.Enabled = true;
                            }
                            //
                            lstAdo = lstAdoTemp;
                            if (lstAdo != null && lstAdo.Count > 0)
                            {
                                lstAdo = lstAdo.Where(o => o.MEDICINE_TYPE_CODE != department.MEDICINE_TYPE_CODE).ToList();
                            }
                            InitCombo(lstAdo);
                        }
                        SendKeys.Send("{TAB}");
                }

                else
                {
                    cboMedicineType.Focus();
                    //cboMedicineType.ShowPopup();
                    txtMedicine.EditValue = null;
                    txtMedicine.Enabled = true;
                    txtPackageNumber.Enabled = true;
                    lstAdo = lstAdoTemp;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ActionType = GlobalVariables.ActionAdd;
                dxValidationProvider1.RemoveControlError(this.txtMedicinetype);

                dxValidationProvider1.RemoveControlError(this.spinAmount);
                dxValidationProvider1.RemoveControlError(this.dtStartTime);
                dxValidationProvider1.RemoveControlError(this.dtEndTime);
                dxValidationProvider1.RemoveControlError(this.txtMedicine);
                dxValidationProvider1.RemoveControlError(this.txtPackageNumber);
                dxValidationProvider1.RemoveControlError(this.dtExpiredDate);
                Loaddatatogrid();
                SetDefaultValue();
                grdInfusionMedicine.DataSource = null;
                SetDefaultDataToInfusionMedicine();
                dtNgayChidinh.EditValue = DateTime.Now;
                glstMediType = FillDataCombo(); ;
                cboMedicineType.Properties.DataSource = glstMediType;
                if (ucServiceUnit != null)
                {
                    serviceUnitProcessor.Reload(ucServiceUnit, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefesh_Click(null, null);
        }

        private void cboServiceUnitName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                this.spinAmount.Focus();
                this.spinAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    if (cboMedicineType.EditValue != null)
                    {
                        var department = glstMediType.Where(f => f.ID.Equals(cboMedicineType.EditValue)).FirstOrDefault();
                        if (department != null)
                        {
                            if (department.ngoaikho == false)
                            {
                                txtMedicine.Enabled = false;
                                txtPackageNumber.Enabled = false;
                                dtExpiredDate.Enabled = false;
                            }
                            else
                            {
                                txtMedicine.Enabled = true;
                                txtPackageNumber.Enabled = true;
                                dtExpiredDate.Enabled = true;
                            }
                            txtMedicinetype.Text = department.MEDICINE_TYPE_CODE;
                            txtMedicine.Text = department.MEDICINE_TYPE_NAME;
                            txtPackageNumber.Text = department.PACKAGE_NUMBER;
                            spinAmount.EditValue = department.AMOUNT;
                            ServiceUnitInputADO serviceUnit = new ServiceUnitInputADO();
                            if (department.SERVICE_UNIT_ID != null)
                            {
                                serviceUnit.SERVICE_UNIT_ID = department.SERVICE_UNIT_ID;
                                serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            }
                            else
                                serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            serviceUnitProcessor.Reload(ucServiceUnit, serviceUnit);
                            if (department.EXPIRED_DATE != null && department.EXPIRED_DATE > 0)
                                dtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)department.EXPIRED_DATE);
                            spinSpeed.EditValue = department.SPEED;
                            cboReqUsername.EditValue = department.LOGGINNAME;

                        }
                        //cboServiceUnitName.Focus();

                    }
                    else
                    {
                        txtMedicine.EditValue = null;
                        txtMedicine.Enabled = true;
                        txtPackageNumber.Enabled = true;
                        txtMedicine.Focus();

                    }

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnitName_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void spinAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinEditVolumn.SelectAll();
                    spinEditVolumn.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboSpeedUnit.Focus();
                    this.cboSpeedUnit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtEndTime.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateEdit2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtNote.Focus();
                    this.txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnRefesh.Focus();
                e.Handled = true;
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FindByMedicineType(List<MEDITYPE> glst)
        {
            if (String.IsNullOrEmpty(txtMedicinetype.Text))
            {
                cboMedicineType.EditValue = null;
                cboMedicineType.Focus();
                cboMedicineType.ShowPopup();
            }
            else
            {
                string key = Inventec.Common.String.Convert.UnSignVNese(this.txtMedicinetype.Text.ToLower().Trim());

                if (glstMediTypeSort != null && glst.Count > 0)
                {
                    var listData = glst.Where(o =>
                        (!String.IsNullOrEmpty(o.MEDICINE_TYPE_CODE) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_CODE.ToLower()).Contains(key))
                        || (!String.IsNullOrEmpty(o.MEDICINE_TYPE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_NAME.ToLower()).Contains(key))).ToList();
                    if (listData != null && listData.Count == 1)
                    {
                        if (listData.FirstOrDefault().ngoaikho == false)
                        {
                            txtMedicine.Enabled = false;
                            txtPackageNumber.Enabled = false;
                            dtExpiredDate.Enabled = false;
                        }
                        else
                        {
                            txtMedicine.Enabled = true;
                            txtPackageNumber.Enabled = true;
                            dtExpiredDate.Enabled = true;
                        }
                        txtMedicinetype.Text = listData.FirstOrDefault().MEDICINE_TYPE_CODE;
                        cboMedicineType.EditValue = listData.FirstOrDefault().ID;
                        txtMedicine.Text = listData.First().MEDICINE_TYPE_NAME;
                        txtPackageNumber.Text = listData.FirstOrDefault().PACKAGE_NUMBER;
                        spinAmount.EditValue = listData.First().AMOUNT;
                        if (listData.FirstOrDefault().EXPIRED_DATE != null && listData.First().EXPIRED_DATE > 0)
                            dtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)listData.FirstOrDefault().EXPIRED_DATE);

                        spinSpeed.EditValue = listData.FirstOrDefault().SPEED;
                        HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO inputService = new HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO();
                        if (listData.FirstOrDefault().SERVICE_UNIT_ID != null && listData.FirstOrDefault().SERVICE_UNIT_ID > 0)
                        {
                            inputService.SERVICE_UNIT_NAME = listData.FirstOrDefault().SERVICE_UNIT_NAME;
                            inputService.SERVICE_UNIT_ID = listData.FirstOrDefault().SERVICE_UNIT_ID;
                        }
                        else
                            inputService.SERVICE_UNIT_NAME = listData.FirstOrDefault().SERVICE_UNIT_NAME;
                        serviceUnitProcessor.Reload(ucServiceUnit, inputService);
                        cboReqUsername.EditValue = listData.First().LOGGINNAME;
                        cboExeUsername.EditValue = null;
                        SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        glst = glst.Where(o =>
                        (!String.IsNullOrEmpty(o.MEDICINE_TYPE_CODE) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_CODE.ToLower()).Contains(key))
                        || (!String.IsNullOrEmpty(o.MEDICINE_TYPE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_NAME.ToLower()).Contains(key))).ToList();
                        if (glst != null && glst.Count > 0)
                        {
                            cboMedicineType.Properties.DataSource = glst;
                        }
                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                }
            }
        }

        private void txtMedicinetype_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtNgayChidinh.EditValue != null)
                    {
                        glstMediType = FillDataCombo();
                        cboMedicineType.Properties.DataSource = glstMediType;
                        FindByMedicineType(glstMediType);
                    }
                    else
                    {
                        cboMedicineType.Properties.DataSource = glstMediType;
                        FindByMedicineType(glstMediType);

                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboExeUsername.Focus();
                    cboExeUsername.ShowPopup(); ;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void cboExeUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.dtStartTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackageNumber.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackageNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.serviceUnitProcessor.FocusControl(ucServiceUnit);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void dtNgayChidinh_Closed(object sender, ClosedEventArgs e)
        {
            if (cboMedicineType.EditValue == null)
            {
                SetDefaultValue();
                glstMediType = FillDataCombo();
                cboMedicineType.Properties.DataSource = glstMediType;
                txtMedicinetype.Focus();
            }
        }

        private void dtNgayChidinh_TextChanged(object sender, EventArgs e)
        {
            if (cboMedicineType.EditValue == null)
            {
                //SetDefaultValue();
                glstMediType = FillDataCombo();
                cboMedicineType.Properties.DataSource = glstMediType;
                //txtMedicinetype.Focus();
            }
        }

        private void dtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dtNgayChidinh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicinetype.SelectAll();
                    txtMedicinetype.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboSpeedUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSpeedUnit.EditValue = null;
                    spinEditConvertVolumnRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSpeedUnit_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboSpeedUnit.EditValue != null)
                    {
                        var speedUnit = glstSpeedUnit.Where(sp => sp.ID == Convert.ToInt64(cboSpeedUnit.EditValue)).ToList();
                        if (speedUnit != null && speedUnit.Count > 0)
                        {
                            if (speedUnit.FirstOrDefault().CONVERT_VOLUME_RATIO != null)
                            {
                                if (speedUnit.FirstOrDefault().CONVERT_VOLUME_RATIO == 1)
                                    spinEditConvertVolumnRatio.Enabled = false;
                                else
                                    spinEditConvertVolumnRatio.Enabled = true;
                                spinEditConvertVolumnRatio.EditValue = speedUnit.FirstOrDefault().CONVERT_VOLUME_RATIO;
                            }
                            else
                            {
                                spinEditConvertVolumnRatio.EditValue = 20;
                                spinEditConvertVolumnRatio.Enabled = true;
                            }
                        }
                    }
                    SetDefaultEndTime();
                    if (spinEditConvertVolumnRatio.Enabled == true)
                    {
                        spinEditConvertVolumnRatio.SelectAll();
                        spinEditConvertVolumnRatio.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSpeed_EditValueChanged(object sender, EventArgs e)
        {
            if (spinSpeed.EditValue != null && spinSpeed.Value > 0)
            {
                cboSpeedUnit.EditValue = 3;
                spinEditConvertVolumnRatio.EditValue = 20;
                spinEditConvertVolumnRatio.Enabled = true;
            }
            SetDefaultEndTime();
        }

        private void spinEditConvertVolumnRatio_EditValueChanged(object sender, EventArgs e)
        {
            SetDefaultEndTime();
        }

        private void dtStartTime_EditValueChanged(object sender, EventArgs e)
        {
            SetDefaultEndTime();
        }

        private void SetDefaultEndTime()
        {
            try
            {
                if (spinSpeed.EditValue != null && spinSpeed.Value > 0 && spinEditConvertVolumnRatio.EditValue != null && dtStartTime.EditValue != null && spinEditVolumn.EditValue != null && spinEditVolumn.Value > 0)
                {
                    var speedUnit = glstSpeedUnit.Where(sp => sp.ID == Convert.ToInt64(cboSpeedUnit.EditValue)).ToList();
                    if (speedUnit != null && speedUnit.Count > 0)
                    {
                        decimal convert = speedUnit.FirstOrDefault().CONVERT_TIME_RATIO * spinEditConvertVolumnRatio.Value;
                        decimal tocdo = spinSpeed.Value / convert;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tocdo), tocdo));
                        var ekipUsers = grdInfusionMedicine.DataSource as List<ComboSelectMedicineADO>;
                        decimal? girdDungTich = ekipUsers.Sum(o => o.VOLUME);
                        decimal? tgTruyen;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => girdDungTich), girdDungTich));
                        if (girdDungTich > 0)
                        {
                            decimal? dungtich_ = spinEditVolumn.Value + girdDungTich;
                            tgTruyen = dungtich_ / tocdo;
                        }
                        else
                        {
                            tgTruyen = spinEditVolumn.Value / tocdo;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tgTruyen), tgTruyen));

                        dtEndTime.DateTime = dtStartTime.DateTime.AddMinutes(Convert.ToDouble(tgTruyen));

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditVolumn_EditValueChanged(object sender, EventArgs e)
        {
            SetDefaultEndTime();
        }

        private void spinEditVolumn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSpeed.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditConvertVolumnRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSpeedUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (spinEditConvertVolumnRatio.Enabled == true)
                {
                    spinEditConvertVolumnRatio.SelectAll();
                    spinEditConvertVolumnRatio.Focus();
                }
                else
                {
                }
            }
        }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkPrint.Focus();
            }
        }

        #region<Lưu trạng thái checkbox vào máy trạm>
        private void ProcessPrint()
        {
            try
            {
                ConfigADO ado = new ConfigADO();
                //richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (chkPrint.Checked)
                {
                    ado.IsPrint = "1";
                }
                if (chkSign.Checked)
                {
                    ado.IsSign = "1";
                    //richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinter);
                }

                if (chkPrintDocumentSigned.Checked)
                {
                    ado.IsPrintDocumentSigned = "1";
                    // richEditorMain.RunPrintTemplate("Mps000001", DelegateRunPrinter);
                }
                if (chkSignForPrint.Checked)
                {
                    ado.IsSignForPrint = "1";
                }

                if (chkPrintDocumentSignedForPrint.Checked)
                {
                    ado.IsPrintDocumentSignedForPrint = "1";
                }

                if (this._ConfigADO != null && (this._ConfigADO.IsSign != ado.IsSign || this._ConfigADO.IsPrintDocumentSigned != ado.IsPrintDocumentSigned || this._ConfigADO.IsSignForPrint != ado.IsSignForPrint || this._ConfigADO.IsPrintDocumentSignedForPrint != ado.IsPrintDocumentSignedForPrint))
                {
                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(ado);

                    //Update cònig
                    SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                    configAppUserUpdate.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    configAppUserUpdate.VALUE = value;
                    configAppUserUpdate.CONFIG_APP_ID = _currentConfigApp.ID;
                    if (currentConfigAppUser != null)
                        configAppUserUpdate.ID = currentConfigAppUser.ID;
                    string api = configAppUserUpdate.ID > 0 ? "api/SdaConfigAppUser/Update" : "api/SdaConfigAppUser/Create";
                    CommonParam param = new CommonParam();
                    var UpdateResult = new BackendAdapter(param).Post<SDA_CONFIG_APP_USER>(
                            api, ApiConsumers.SdaConsumer, configAppUserUpdate, param);

                    //if (UpdateResult != null)
                    //{
                    //    success = true;
                    //}

                    MessageManager.Show(this.ParentForm, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigHisAcc()
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_INFUSION_CREATE__IS_PRINT_IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = loginName;
                    appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                    currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                    if (currentConfigAppUser != null)
                    {
                        key = currentConfigAppUser.VALUE;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        if (_ConfigADO.IsPrint == "1")
                            chkPrint.Checked = true;
                        else
                            chkPrint.Checked = false;
                        if (_ConfigADO.IsSign == "1")
                            chkSign.Checked = true;
                        else
                            chkSign.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSigned == "1")
                            chkPrintDocumentSigned.Checked = true;
                        else
                            chkPrintDocumentSigned.Checked = false;
                        if (_ConfigADO.IsSignForPrint == "1")
                            chkSignForPrint.Checked = true;
                        else
                            chkSignForPrint.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSignedForPrint == "1")
                            chkPrintDocumentSignedForPrint.Checked = true;
                        else
                            chkPrintDocumentSignedForPrint.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSign.Checked == true)
            {
                chkPrintDocumentSigned.Enabled = true;
            }
            else
            {
                chkPrintDocumentSigned.Checked = false;
                chkPrintDocumentSigned.Enabled = false;
            }
        }

        private void txtMixedMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboReqUsername.Focus();
                    this.cboReqUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Thêm chức năng in
        private void chkSignForPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSignForPrint.Checked == true)
            {
                chkPrintDocumentSignedForPrint.Enabled = true;
            }
            else
            {
                chkPrintDocumentSignedForPrint.Checked = false;
                chkPrintDocumentSignedForPrint.Enabled = false;
            }
        }

        private void btnCboPrint_Click(object sender, EventArgs e)
        {
            btnCboPrint.ShowDropDown();
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuTruyenDich = new DXMenuItem("Phiếu truyền dịch", new EventHandler(OnClickInPhieuChamSoc));
                itemPhieuTruyenDich.Tag = PrintTypeInfusion.IN_PHIEU_TRUYEN_DICH;
                menu.Items.Add(itemPhieuTruyenDich);

                btnCboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuChamSoc(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintTypeInfusion printType = (PrintTypeInfusion)(bbtnItem.Tag);
                ProcessPrint();
                PrintProcess(printType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(PrintTypeInfusion printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeInfusion.IN_PHIEU_TRUYEN_DICH:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuTruyenDich_MPS000146, DelegateRunPrinterInfusion);
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

        private bool DelegateRunPrinterInfusion(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuTruyenDich_MPS000146:
                        LoadBieuMauPhieuTruyenDich_MPS000146(printTypeCode, fileName, ref result);
                        Loaddatatogrid();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }

        private void LoadBieuMauPhieuTruyenDich_MPS000146(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.listInfusionsSelected = new List<V_HIS_INFUSION>();
                int[] selectRows = gridView1.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        this.listInfusionsSelected.Add((V_HIS_INFUSION)gridView1.GetRow(selectRows[i]));
                    }
                }

                if (this.listInfusionsSelected == null || this.listInfusionsSelected.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dữ liệu in", "Thông báo");
                    return;
                }

                List<HIS_MIXED_MEDICINE> lstMixedMedicine = new List<HIS_MIXED_MEDICINE>();
                if (listInfusionsSelected != null && listInfusionsSelected.Count() > 0)
                {
                    foreach (var item in listInfusionsSelected)
                    {
                        HisMixedMedicineFilter medicineFilter = new HisMixedMedicineFilter();
                        medicineFilter.INFUSION_ID = item.ID;
                        var lstHisMixedMedicine = new BackendAdapter(new CommonParam()).Get<List<HIS_MIXED_MEDICINE>>("api/HisMixedMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, null);
                        if (lstHisMixedMedicine != null && lstHisMixedMedicine.Count() > 0)
                        {
                            lstMixedMedicine.AddRange(lstHisMixedMedicine);
                        }
                    }
                }


                WaitingManager.Show();
                HisInfusionSumViewFilter infuFilter = new HisInfusionSumViewFilter();
                infuFilter.ID = this.data.InfusionSumId;
                var data = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INFUSION_SUM>>("api/HisInfusionSum/GetView", ApiConsumers.MosConsumer, infuFilter, null);

                MOS.Filter.HisTreatmentBedRoomViewFilter filterBedRoom = new HisTreatmentBedRoomViewFilter();
                filterBedRoom.TREATMENT_ID = this.data.treatmentId;
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filterBedRoom, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.OrderByDescending(o => o.OUT_TIME).ThenByDescending(o => o.ID).FirstOrDefault(o => o.BED_ID.HasValue);
                }
                HisTreatmentView2Filter treatFilter = new HisTreatmentView2Filter();
                treatFilter.ID = this.data.treatmentId;
                var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatFilter, null);
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.treatment = listTreatment.First();
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_EMR_DOCUMENT_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, "", (this.treatment != null ? this.treatment.TREATMENT_CODE : ""));
                    inputADO.DocumentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listInfusionsSelected.Min(o => o.START_TIME) ?? 0);
                }

                MPS.Processor.Mps000146.PDO.Mps000146PDO rdo = new MPS.Processor.Mps000146.PDO.Mps000146PDO(
                    data.FirstOrDefault(),
                    this.treatment,
                    this.listInfusionsSelected,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    _TreatmetnbedRoom,
                    lstMixedMedicine
                                        );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (chkSignForPrint.Checked)
                {
                    if (chkPrintDocumentSignedForPrint.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };

                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo._ListAdo_________________", rdo._ListAdo));

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDateInfusion_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToComboSelectMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInfusionMedicine_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((grvInfusionMedicine.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
                else if (e.Column.FieldName == "VOLUME")
                {
                    long isEnableVolume = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["isDisableVolume"]) ?? "0").ToString());
                    e.RepositoryItem = isEnableVolume == 0 ? TextEdit__DungTich : TextEdit__DungTichDisable;

                }
                else if (e.Column.FieldName == "AMOUNT")
                {
                    long isEnableAmount = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["isDisableAmount"]) ?? "0").ToString());
                    e.RepositoryItem = isEnableAmount == 0 ? TextEdit__SoLuong : TextEdit__SoLuongDisable;
                }
                else if (e.Column.FieldName == "SERVICE_UNIT_NAME")
                {
                    long isEnableUnitName = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["isDisableUnitName"]) ?? "0").ToString());
                    e.RepositoryItem = isEnableUnitName == 0 ? TextEdit__DVT : TextEdit__DVTDisable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInfusionMedicine_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "MEDICINE")
                {
                    grvInfusionMedicine.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSelectMedicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                grvInfusionMedicine.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                // grvInfusionMedicine.FocusedColumn = grvInfusionMedicine.VisibleColumns[3];
                var meidcinetypeId = grvInfusionMedicine.GetFocusedRowCellValue("MEDICINE_TYPE_ID");
                var data = lstAdo.Where(o => o.MEDICINE_TYPE_ID.Equals(meidcinetypeId)).ToList();
                if (data != null && data.Count > 0)
                {
                    if (meidcinetypeId != null && Convert.ToInt64(meidcinetypeId) > 0)
                        grvInfusionMedicine.SetFocusedRowCellValue("isDisableVolume", 1);
                    else
                        grvInfusionMedicine.SetFocusedRowCellValue("isDisableVolume", 0);

                    if (!string.IsNullOrEmpty(data.FirstOrDefault().SERVICE_UNIT_NAME))
                        grvInfusionMedicine.SetFocusedRowCellValue("isDisableVolume", 1);
                    else
                        grvInfusionMedicine.SetFocusedRowCellValue("isDisableVolume", 0);

                    grvInfusionMedicine.SetFocusedRowCellValue("MEDICINE_TYPE_NAME", data.FirstOrDefault().MEDICINE_TYPE_NAME);
                    grvInfusionMedicine.SetFocusedRowCellValue("PACKAGE_NUMBER", data.FirstOrDefault().PACKAGE_NUMBER);
                    grvInfusionMedicine.SetFocusedRowCellValue("MEDICINE_ID", data.FirstOrDefault().MEDICINE_ID);
                    grvInfusionMedicine.SetFocusedRowCellValue("INFUSION_ID", data.FirstOrDefault().INFUSION_ID);
                    grvInfusionMedicine.SetFocusedRowCellValue("AMOUNT", data.FirstOrDefault().AMOUNT);
                    grvInfusionMedicine.SetFocusedRowCellValue("SERVICE_UNIT_NAME", data.FirstOrDefault().SERVICE_UNIT_NAME);
                }
                //if (meidcinetypeId != null && Convert.ToInt64(meidcinetypeId) > 0)
                //    data.FirstOrDefault().isDisableUnitName = 0;
                //else
                //    data.FirstOrDefault().isDisableUnitName = 1;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                List<ComboSelectMedicineADO> ekipUserAdoTemps = new List<ComboSelectMedicineADO>();
                var ekipUsers = grdInfusionMedicine.DataSource as List<ComboSelectMedicineADO>;
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    ComboSelectMedicineADO ekipUserAdoTemp = new ComboSelectMedicineADO();
                    //ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    ekipUserAdoTemps.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUserAdoTemps.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    grdInfusionMedicine.DataSource = null;
                    grdInfusionMedicine.DataSource = ekipUserAdoTemps;
                }
                else
                {
                    ComboSelectMedicineADO participant = new ComboSelectMedicineADO();
                    //participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUsers.Add(participant);
                    ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    grdInfusionMedicine.DataSource = null;
                    grdInfusionMedicine.DataSource = ekipUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var ekipUsers = grdInfusionMedicine.DataSource as List<ComboSelectMedicineADO>;
                var ekipUser = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);
                        ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        grdInfusionMedicine.DataSource = null;
                        grdInfusionMedicine.DataSource = ekipUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInfusionMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();

                if (e.Column.FieldName == "MEDICINE_TYPE_ID" || e.Column.FieldName == "AMOUNT" || e.Column.FieldName == "VOLUME")
                {
                    this.grdInfusionMedicine.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboMedicine(GridLookUpEdit cbo)
        {
            try
            {

                cbo.Properties.DataSource = lstAdo;
                cbo.Properties.DisplayMember = "MEDICINE_TYPE_CODE";
                cbo.Properties.ValueMember = "MEDICINE_TYPE_ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInfusionMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "MEDICINE_TYPE_CODE")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "MEDICINE_TYPE_ID") ?? "").ToString();
                            if (!string.IsNullOrEmpty(status))
                            {
                                var data = lstAdo.SingleOrDefault(o => o.MEDICINE_TYPE_ID == Int64.Parse(status));
                                e.Value = data.MEDICINE_TYPE_CODE;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot MEDICINE_TYPE_CODE", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInfusionMedicine_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ComboSelectMedicineADO data = view.GetFocusedRow() as ComboSelectMedicineADO;
                if (view.FocusedColumn.FieldName == "MEDICINE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        editor.EditValue = data.MEDICINE_TYPE_ID;
                    }
                    ComboMedicine(editor);
                    grvInfusionMedicine.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TextEdit__DungTich_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();
                if (row != null)
                {
                    TextEdit txt = sender as TextEdit;
                    if (row.MEDICINE_TYPE_ID != null && row.MEDICINE_TYPE_ID > 0)
                    {
                        if (txt.Text != null && txt.Text != "" && Int64.Parse(txt.Text) > 0)
                        {
                            row.isDisableAmount = 1;
                        }
                        else
                        {
                            row.isDisableAmount = 0;
                        }

                        row.VOLUME = Int64.Parse(txt.Text);
                    }
                    else
                    {
                        if (txt.Text != null && txt.Text != "" && Int64.Parse(txt.Text) > 0)
                        {
                            grvInfusionMedicine.SetFocusedRowCellValue("AMOUNT", null);
                            grvInfusionMedicine.SetFocusedRowCellValue("SERVICE_UNIT_NAME", null);
                            row.isDisableAmount = 1;
                            row.isDisableUnitName = 1;
                        }
                        else
                        {
                            row.isDisableAmount = 0;
                        }
                    }
                    //
                    SetDefaultEndTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEdit__TenThuoc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ComboSelectMedicineADO ADO = new ComboSelectMedicineADO();
                var data = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();
                if (data != null && data is ComboSelectMedicineADO)
                {
                    TextEdit txt = sender as TextEdit;

                    ADO = (ComboSelectMedicineADO)data;

                    if (txt.Text != null && txt.Text != "")
                    {
                        ADO.MEDICINE_TYPE_NAME = txt.Text;
                    }
                    else
                    {
                        ADO.MEDICINE_TYPE_NAME = null;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEdit__SoLo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ComboSelectMedicineADO ADO = new ComboSelectMedicineADO();
                var data = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();
                if (data != null && data is ComboSelectMedicineADO)
                {
                    TextEdit txt = sender as TextEdit;

                    ADO = (ComboSelectMedicineADO)data;

                    if (txt.Text != null && txt.Text != "")
                    {
                        ADO.PACKAGE_NUMBER = txt.Text;
                    }
                    else
                    {
                        ADO.PACKAGE_NUMBER = null;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEdit__DungTich_KeyPress(object sender, KeyPressEventArgs e)
        {
            //try
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
            //    {
            //        e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}



        }

        private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Delete")
                {
                    repositoryItemButtonEdit1_Click(null, null);
                }
                else
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    if (dtNgayChidinh.EditValue != null)
                    {
                        dtNgayChidinh.EditValue = null;
                        glstMediType = FillDataCombo();
                        cboMedicineType.Properties.DataSource = glstMediType;
                    }

                    V_HIS_INFUSION row = new V_HIS_INFUSION();
                    row = (V_HIS_INFUSION)gridView1.GetFocusedRow();
                    if (row != null)
                    {

                        IDUpdate = row.ID;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                        if (row.MEDICINE_ID != null)
                        {
                            var mediType = glstMediType.Where(mdi => mdi.MEDICINE_ID == row.MEDICINE_ID).ToList();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediType), mediType));

                            if (mediType != null && mediType.Count > 0)
                                cboMedicineType.EditValue = mediType.FirstOrDefault().ID;
                            var medicine = glstMedicine.Where(md => md.ID == row.MEDICINE_ID).ToList();
                            if (medicine != null && medicine.Count > 0)
                            {
                                txtMedicinetype.Text = medicine.FirstOrDefault().MEDICINE_TYPE_CODE;
                                txtMedicine.Text = medicine.FirstOrDefault().MEDICINE_TYPE_NAME;


                            }
                            txtMedicine.Enabled = false;
                            txtPackageNumber.Enabled = false;
                        }


                        //175827 sửa chức năng tạo thông tin truyền dịch
                        txtPackageNumber.Text = row.PACKAGE_NUMBER;
                        //
                        HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO inputService = new HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO();
                        if (row.SERVICE_UNIT_ID != null && row.SERVICE_UNIT_ID > 0)
                        {
                            inputService.SERVICE_UNIT_NAME = row.SERVICE_UNIT_NAME;
                            inputService.SERVICE_UNIT_ID = row.SERVICE_UNIT_ID;
                        }
                        else
                            inputService.SERVICE_UNIT_NAME = row.SERVICE_UNIT_NAME;
                        serviceUnitProcessor.Reload(ucServiceUnit, inputService);
                        //cboServiceUnitName.EditValue = row.SERVICE_UNIT_ID;
                        spinAmount.EditValue = row.AMOUNT;
                        spinSpeed.EditValue = row.SPEED;
                        dtNgayChidinh.EditValue = null;
                        //dtDateInfusion.EditValue = null;
                        dtStartTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.START_TIME);
                        dtEndTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.FINISH_TIME);
                        if (row.EXPIRED_DATE != null && row.EXPIRED_DATE > 0)
                            dtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.EXPIRED_DATE);
                        else
                            dtExpiredDate.EditValue = null;
                        txtNote.EditValue = row.NOTE;
                        spinEditVolumn.EditValue = row.VOLUME;
                        spinEditConvertVolumnRatio.EditValue = row.CONVERT_VOLUME_RATIO;
                        if (row.SPEED_UNIT_ID != null && row.SPEED_UNIT_ID > 0)
                            cboSpeedUnit.EditValue = row.SPEED_UNIT_ID;

                        ActionType = GlobalVariables.ActionEdit;
                        //CheckMedicine();
                        cboReqUsername.EditValue = row.REQUEST_LOGINNAME;
                        cboExeUsername.EditValue = row.EXECUTE_LOGINNAME;
                        if (row.MEDICINE_ID == null)
                        {

                            cboMedicineType.EditValue = null;
                            txtMedicinetype.Text = "";
                            txtMedicine.Text = row.MEDICINE_TYPE_NAME;
                            txtMedicine.Enabled = true;
                            txtPackageNumber.Enabled = true;

                        }
                        grdInfusionMedicine.DataSource = null;
                        LoadDataToComboSelectMedicine();
                        HisMixedMedicineFilter filter = new HisMixedMedicineFilter();
                        filter.INFUSION_ID = row.ID;
                        var dataMixedMedicine = new BackendAdapter(param).Get<List<HIS_MIXED_MEDICINE>>("api/HisMixedMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataMixedMedicine), dataMixedMedicine));
                        if (dataMixedMedicine != null && dataMixedMedicine.Count > 0)
                        {
                            int count = 0;
                            List<ComboSelectMedicineADO> lstFillGrid = new List<ComboSelectMedicineADO>();
                            foreach (var item in dataMixedMedicine)
                            {
                                ComboSelectMedicineADO ado = new ComboSelectMedicineADO();
                                ado.MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                ado.MEDICINE_ID = item.MEDICINE_ID;
                                ado.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                if (item.MEDICINE_TYPE_ID != null && item.MEDICINE_TYPE_ID > 0)
                                {
                                    ado.MEDICINE_TYPE_CODE = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == item.MEDICINE_TYPE_ID).MEDICINE_TYPE_CODE;
                                }
                                if (item.AMOUNT > 0)
                                {

                                }
                                ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                                ado.VOLUME = item.VOLUME;
                                ado.AMOUNT = item.AMOUNT;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.INFUSION_ID = item.ID;
                                ado.Action = count == 0 ? HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd : HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                if (ado.AMOUNT > 0)
                                {
                                    ado.isDisableVolume = 1;
                                    ado.isDisableAmount = 0;
                                }
                                else if (ado.VOLUME > 0)
                                {
                                    ado.isDisableVolume = 0;
                                    ado.isDisableAmount = 1;
                                }
                                if (ado.VOLUME.ToString().Equals("0,0"))
                                    ado.VOLUME = null;
                                count++;
                                lstFillGrid.Add(ado);
                            }
                            grdInfusionMedicine.DataSource = lstFillGrid;
                        }
                        SetDefaultDataToInfusionMedicine();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdInfusionMedicine_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (grvInfusionMedicine.FocusedColumn.VisibleIndex == grvInfusionMedicine.VisibleColumns.Count - 1)
                    {

                        var data = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();

                        if (data.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                        {

                            btnAdd_ButtonClick(null, null);
                            grvInfusionMedicine.FocusedRowHandle = grvInfusionMedicine.DataRowCount - 1;
                            int visibleIndex = grvInfusionMedicine.FocusedColumn.VisibleIndex;
                            int newVisibleIndex = visibleIndex + 1;
                            if (newVisibleIndex == grvInfusionMedicine.VisibleColumns.Count)
                                newVisibleIndex = 0;
                            grvInfusionMedicine.FocusedColumn = grvInfusionMedicine.VisibleColumns[newVisibleIndex];
                        }
                        else
                        {
                            btnDelete_ButtonClick(null, null);
                        }

                    }
                    else
                    {
                        int visibleIndex = grvInfusionMedicine.FocusedColumn.VisibleIndex;
                        int newVisibleIndex = visibleIndex + 1;
                        if (newVisibleIndex == grvInfusionMedicine.VisibleColumns.Count)
                            newVisibleIndex = 0;
                        grvInfusionMedicine.FocusedColumn = grvInfusionMedicine.VisibleColumns[newVisibleIndex];
                    }
                    //grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.MoveNext(grdViewInformationSurg.FocusedColumn);


                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void TextEdit__SoLuong_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ComboSelectMedicineADO)grvInfusionMedicine.GetFocusedRow();
                if (row != null)
                {
                    TextEdit txt = sender as TextEdit;
                    if (row.MEDICINE_TYPE_ID != null && row.MEDICINE_TYPE_ID > 0)
                    {
                        if (txt.Text != null && txt.Text != "" && Int64.Parse(txt.Text) > 0)
                        {
                            row.isDisableVolume = 1;
                        }
                        else
                        {
                            row.isDisableVolume = 0;
                        }

                    }
                    //
                    else
                    {
                        if (txt.Text != null && txt.Text != "" && Int64.Parse(txt.Text) > 0)
                        {
                            grvInfusionMedicine.SetFocusedRowCellValue("VOLUME", null);
                            row.isDisableUnitName = 0;
                            row.isDisableVolume = 1;
                        }
                        else
                        {
                            row.isDisableUnitName = 1;
                            row.isDisableVolume = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEdit__SoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            //try
            //{
            //    if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || char.ToString(e.KeyChar).Equals(","))
            //    {
            //        e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void TextEdit__DungTich_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    SetDefaultEndTime();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }



    }
}
