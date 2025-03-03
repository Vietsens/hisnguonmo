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
using ACS.SDO;
using DevExpress.Data;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BedRoomWithIn.ADO;
using HIS.Desktop.Plugins.BedRoomWithIn.Resources;
using HIS.Desktop.Plugins.BedRoomWithIn.ValidationRule;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.BedRoomWithIn;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HIS.Desktop.Plugins.BedRoomWithIn
{
    public partial class frmBedRoomWithIn : HIS.Desktop.Utility.FormBase
    {
        private V_HIS_DEPARTMENT_TRAN currentHisDepartmentTran = null;
        private int positionHandleControl = -1;
        private long treatmentId;
        private HIS_TREATMENT_TYPE currentTreatmentType;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        private HIS_TREATMENT currentTreatment;
        private List<HisBedADO> dataBedADOs;
        private List<HisBedRoomV1> dataHisBedRoomV1ADOs;
        private List<HIS_BED_BSTY> hisBedBstys;
        private List<V_HIS_SERVICE> VHisBedServiceTypes { get; set; }
        private List<V_HIS_SERVICE_ROOM> ListServiceBedByRooms { get; set; }
        private List<long> patientTypeIdAls;
        private List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        private V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;

        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;

        private long[] serviceTypeIdAllows = new long[11]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};
        List<long> listTreatmentType = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU,
                                                                IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU,
                                                                IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY,
                                                                IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA};
        private string commonString__true = "1";
        HIS_DEPARTMENT currentDepartment = new HIS_DEPARTMENT();
        HisTreatmentWithPatientTypeInfoSDO TreatmentWithPaTyInfo;
        private MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        internal IcdProcessor icdYhctProcessor;
        internal UserControl ucIcdYhct;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;
        List<HIS_ICD> currentIcds;
        string doctorLoginname = "";
        string doctorUsername = "";
        const int MaxReq = 500;
        public frmBedRoomWithIn(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentHisDepartmentTran = departmentTran;
                this.currentModule = currentModule;
                this.treatmentId = departmentTran.TREATMENT_ID;
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId);
                VisibilityControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibilityControl()
        {
            try
            {
                if (Config.IsPrimaryPatientType != "1" && Config.IsPrimaryPatientType != "2")
                {
                    LciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Height = this.Height - 26;
                }

                if (Config.IsUsingBedTemp == "1")
                {
                    CboBedService.Properties.Buttons[1].Visible = false;
                    CboPatientType.Properties.Buttons[1].Visible = false;
                }

                if (Config.IsManualInCode && this.currentHisDepartmentTran.IS_HOSPITALIZED == 1)
                {
                    lciTxtInCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciLblInCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ValidationSingleControl(txtInCode);
                }
                else
                {
                    lciTxtInCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciLblInCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmBedRoomWithIn(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.treatmentId = treatmentId;
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_SERVICE_CONDITION> lstConditionService { get; set; }
        private void frmDepartmentTranReceive_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                timerInit.Start();
                SetCaptionByLanguageKey();
                ProcessThreadLoadData();
                SetIconFrm();
                SetIcdDefault();
                LoadComboForm();
                this.Text = (this.currentModule != null) ? this.currentModule.text : null;
                InitDataCboPatientType();
                FillDataToControlForm();
                SetEnableControlTime();
                this.SpNamGhep.EditValue = null;
                LoadDataReasonNt();
                Validation();
                loadDoctor();
                

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataReasonNt()
        {
            try
            {
                
                if (listTreatmentType.Contains(Convert.ToInt64(cboTreatmentType.EditValue)))
                {
                    this.lciReasonNt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LoadDataToCboReasonNt();
                }
                else
                {
                    this.lciReasonNt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Validation()
        {
            try
            {
                if (Config.IsPatientClassify)
                {
                    ValidateGridLookupWithTextEdit(cboPATIENT_CLASSIFY, txtPATIENT_CLASSIFY, dxValidationProvider2);
                    layoutControlItem15.AppearanceItemCaption.ForeColor = Color.Maroon;

                }
                ValidateGridLookupWithTextEdit(cboPatientReceive, txtPatientReceive, dxValidationProvider2);
                layoutControlItem20.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidationSingleControl(dtLogTime, dxValidationProvider2);
                lciTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidateGridLookupWithTextEdit(cboTreatmentType, txtTreatmentTypeCode, dxValidationProvider2);
                if(this.lciReasonNt.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    ValidationSingleControl(txtReasonNt, dxValidationProvider2);
                layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                if (Config.IsRequiredChooseRoom == "1")
                {
                    ValidateGridLookupWithTextEdit(cboBedRoom, txtBedRoomCode, dxValidationProvider2);
                    layoutControlItem5.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitUcSecondaryIcd()
        {
            try
            {
                currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).ToList();

                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), this.currentIcds);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.HisIcds = currentIcds;
                ado.Width = 660;
                ado.Height = 24;
                ado.TextLblIcd = "Chẩn đoán phụ";
                ado.TextNullValue = "Nhấn F1 để chọn chẩn đoán phụ";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                var icdYhct = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ YHCT phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền kèm theo";
                ado.TextNullValue = "Nhấn F1 để chọn chẩn đoán phụ y học cổ truyền";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.panelSubIcdYhct.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCodeYhct()
        {
            string mainCode = "";
            try
            {
                if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void NextForcusSubIcdToDo()
        {
            try
            {
                if (ucSecondaryIcdYhct != null && subIcdYhctProcessor != null)
                {
                    subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {

                var icdValue = this.UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                outPut.ICD_NAME = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                outPut.ICD_CODE = OjecIcd != null ? OjecIcd.ICD_CODE : "";
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        private void NextForcusOut()
        {
            try
            {
                cboBed.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboPatientType()
        {
            try
            {
                LoadComboEditor(CboPatientType, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME", "ID", null);
                LoadComboEditor(CboPrimaryPatientType, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME", "ID", null);
                LoadComboEditor(cboPatientReceive, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME", "ID", patientTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessThreadLoadData()
        {
            //System.Threading.Thread bedData = new System.Threading.Thread(LoadDataBedService);
            System.Threading.Thread patientReceiveData = new System.Threading.Thread(LoadDataPatientReceive);
            System.Threading.Thread patientData = new System.Threading.Thread(LoadDataPatientType);
            System.Threading.Thread treatmentData = new System.Threading.Thread(ProcessLoadHistreatment);
            try
            {
                patientReceiveData.Start();
                patientData.Start();
                treatmentData.Start();

                patientReceiveData.Join();
                patientData.Join();
                treatmentData.Join();
            }
            catch (Exception ex)
            {
                patientReceiveData.Abort();
                patientData.Abort();
                treatmentData.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_PATIENT_TYPE> patientTypes { get; set; }
        private void LoadDataPatientReceive()
        {
            try
            {
                patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NOT_USE_FOR_PATIENT != 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadDataPatientType()
        {
            try
            {
                LoadCurrentPatientTypeAlter();
                PatientTypeWithPatientTypeAlter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsValiICD()
        {
            bool result = true;
            try
            {
                result = (bool)icdProcessor.ValidationIcd(ucIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataBedService()
        {
            try
            {
                this.hisBedBstys = BackendDataWorker.Get<HIS_BED_BSTY>().Where(o => o.IS_ACTIVE == 1).ToList();
                this.VHisBedServiceTypes = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.IS_ACTIVE == 1).ToList();
                this.ListServiceBedByRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o =>
                         (o.ROOM_ID == this.currentModule.RoomId)
                         && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                         && o.IS_ACTIVE == 1).ToList();

                lstConditionService = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && VHisBedServiceTypes.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlTime()
        {
            try
            {
                string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.IS_USING_SERVER_TIME");
                if (key.Trim() == "1")
                {
                    dtLogTime.ReadOnly = true;
                    TimerSDO timeSync = new BackendAdapter(null).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, null);

                    if (timeSync != null)
                    {
                        dtLogTime.DateTime = timeSync.DateNow;
                    }
                }
                else
                {
                    dtLogTime.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboForm()
        {
            try
            {
                long _departmentId = WorkPlace.WorkPlaceSDO.Where(p => p.RoomId == this.currentModule.RoomId).FirstOrDefault().DepartmentId;
                CommonParam paramCommon = new CommonParam();

                MOS.Filter.HisBedRoomView1Filter Filter = new MOS.Filter.HisBedRoomView1Filter();
                Filter.DEPARTMENT_ID = _departmentId;
                Filter.IS_ACTIVE = 1;
                var resultHisBedRoom = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_BED_ROOM_1>>("/api/HisBedRoom/GetView1", ApiConsumers.MosConsumer, Filter, paramCommon);
                if (cboTreatmentType.EditValue != null)
                {
                    var treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentType.EditValue.ToString());
                    currentTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(p => p.ID == treatmentTypeId);
                    List<V_HIS_BED_ROOM_1> lstBedRoomTemp = new List<V_HIS_BED_ROOM_1>();
                    foreach (var item in resultHisBedRoom)
                    {
                        if (string.IsNullOrEmpty(item.TREATMENT_TYPE_IDS))
                        {
                            lstBedRoomTemp.Add(item);
                        }
                        else
                        {
                            var lstIds = item.TREATMENT_TYPE_IDS.Split(',').ToList();
                            if (lstIds.FirstOrDefault(o => o == cboTreatmentType.EditValue.ToString()) != null)
                            {
                                lstBedRoomTemp.Add(item);
                            }
                        }
                    }
                    resultHisBedRoom = lstBedRoomTemp;
                }
                dataHisBedRoomV1ADOs = ProcessDataHisBedRoomV1(resultHisBedRoom);

                HIS_DEPARTMENT _department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _departmentId);

                //  LoadComboEditor(cboBedRoom, "BED_ROOM_CODE", "BED_ROOM_NAME", "ID", BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.DEPARTMENT_ID == _departmentId).ToList());
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("________________resultHisBedRoom___________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => resultHisBedRoom), resultHisBedRoom));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("________________dataHisBedRoomV1ADOs___________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHisBedRoomV1ADOs), dataHisBedRoomV1ADOs));

                LoadDataToComboBedRoom(cboBedRoom, dataHisBedRoomV1ADOs);
                List<HIS_TREATMENT_TYPE> _TreatmentTypes = new List<HIS_TREATMENT_TYPE>();
                if (_department != null && !string.IsNullOrEmpty(_department.ALLOW_TREATMENT_TYPE_IDS))
                {
                    string[] _str = _department.ALLOW_TREATMENT_TYPE_IDS.Split(',');
                    List<long> _Ids = new List<long>();
                    foreach (var item in _str)
                    {
                        _Ids.Add(Inventec.Common.TypeConvert.Parse.ToInt64(item));
                    }
                    Inventec.Common.Logging.LogSystem.Error("1");
                    _TreatmentTypes = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && _Ids.Contains(p.ID)).ToList();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("2");
                    _TreatmentTypes = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList();
                }

                if (this.currentHisDepartmentTran.IS_HOSPITALIZED == 1 && _TreatmentTypes != null && _TreatmentTypes.Count > 0)//#45031
                {
                    Inventec.Common.Logging.LogSystem.Error("3");
                    _TreatmentTypes = _TreatmentTypes.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _TreatmentTypes), _TreatmentTypes));
                LoadComboEditor(cboTreatmentType, "TREATMENT_TYPE_CODE", "TREATMENT_TYPE_NAME", "ID", _TreatmentTypes);
                if (_TreatmentTypes != null && _TreatmentTypes.Count > 0)
                {
                    cboTreatmentType.EditValue = _TreatmentTypes[0].ID;
                    txtTreatmentTypeCode.Text = _TreatmentTypes[0].TREATMENT_TYPE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcdDefault()
        {
            InitUcIcd();
            InitUcIcdYhct();
            InitUcSecondaryIcd();
            InitUcSecondaryIcdYhct();
        }

        private void LoadCboGiuong()
        {
            try
            {
                List<V_HIS_BED> data = new List<V_HIS_BED>();
                if (cboBedRoom.EditValue != null)
                {
                    data = BackendDataWorker.Get<V_HIS_BED>().Where(o => o.BED_ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? 0).ToString()) && o.IS_ACTIVE == 1).ToList();
                }

                dataBedADOs = ProcessDataBedAdo(data);
                dataBedADOs = dataBedADOs.OrderBy(o => o.IsKey).ThenBy(o => o.BED_CODE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 1));
                columnInfos.Add(new ColumnInfo("BED_TYPE_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AMOUNT_STR", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 550);
                ControlEditorLoader.Load(cboBed, dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = DelegateNextFocusIcdYhct;
                ado.Width = 430;
                ado.Height = 24;

                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                ado.IsColor = true;
                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.layoutControlIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateNextFocusIcdYhct()
        {
            try
            {
                if (ucIcdYhct != null)
                {
                    this.icdYhctProcessor.FocusControl(ucIcdYhct);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = DelegateNextFocusIcd;
                ado.Width = 430;
                ado.Height = 24;

                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                this.ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (this.ucIcdYhct != null)
                {
                    this.panelControlIcdYhct.Controls.Add(this.ucIcdYhct);
                    this.ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateNextFocusIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BedRoomWithIn.Resources.Lang", typeof(HIS.Desktop.Plugins.BedRoomWithIn.frmBedRoomWithIn).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.cboBedRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTime.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.barbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmBedRoomWithIn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlForm()
        {
            try
            {
                //ReView

                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = this.treatmentId;
                var _Treatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore
                    .HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                MOS.Filter.HisSereServPtttView1Filter filter_ = new MOS.Filter.HisSereServPtttView1Filter();
                filter_.TDL_TREATMENT_ID = this.treatmentId;
                filter_.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                var HisSereServPtttView_ = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_PTTT_1>>("api/HisSereServPttt/GetView1", ApiConsumers.MosConsumer, filter_, null);
                if (HisSereServPtttView_ != null && HisSereServPtttView_.Count > 0)
                {

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisSereServPtttView_), HisSereServPtttView_));
                    var max = HisSereServPtttView_.Max(x => x.INTRUCTION_DATE);
                    if (max != null)
                    {
                        var maxtime = HisSereServPtttView_.Where(o => o.INTRUCTION_DATE == max);
                        var maxnumber = maxtime.Max(x => x.NUM_ORDER);
                        var maxnumber_ = maxtime.FirstOrDefault(o => o.NUM_ORDER == maxnumber);
                        txtGROUP_NAME.Text = maxnumber_.PTTT_GROUP_NAME;
                    }

                }
                if (_Treatment != null)
                {
                    this.currentTreatment = _Treatment;
                    LoadDataToComboPATIENT_CLASSIFY(cboPATIENT_CLASSIFY, null);
                    if (_Treatment.TDL_PATIENT_TYPE_ID != null)
                    {
                        var ex = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(s => s.ID == _Treatment.TDL_PATIENT_TYPE_ID);
                        cboPatientReceive.EditValue = _Treatment.TDL_PATIENT_TYPE_ID;
                        txtPatientReceive.Text = ex == null ? "": ex.PATIENT_TYPE_CODE;
                    }

                    lblSoVaoVien.Text = _Treatment.IN_CODE;
                    labelName.Text = _Treatment.TDL_PATIENT_NAME;
                    labelGender.Text = _Treatment.TDL_PATIENT_GENDER_NAME;
                    labelAddress.Text = _Treatment.TDL_PATIENT_ADDRESS;
                    if (_Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        labelDOB.Text = _Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        labelDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatment.TDL_PATIENT_DOB).ToString();
                    }
                    cboPATIENT_CLASSIFY.EditValue = _Treatment.TDL_PATIENT_CLASSIFY_ID;
                    txtPATIENT_CLASSIFY.Text = _Treatment.TDL_PATIENT_CLASSIFY_ID != null && _Treatment.TDL_PATIENT_CLASSIFY_ID > 0 && dataClassiFy.FirstOrDefault(o => o.ID == _Treatment.TDL_PATIENT_CLASSIFY_ID) != null ? dataClassiFy.FirstOrDefault(o => o.ID == _Treatment.TDL_PATIENT_CLASSIFY_ID).PATIENT_CLASSIFY_CODE : null;
                    chkIsEmergency.Checked = _Treatment.IS_EMERGENCY == 1;
                    HisPatientFilter filterPatient = new HisPatientFilter();
                    filterPatient.ID = _Treatment.PATIENT_ID;
                    var _Patient = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>(HisRequestUriStore
                    .HIS_PATIENT_GET, ApiConsumers.MosConsumer, filterPatient, null).FirstOrDefault();
                    lblNote.Text = _Patient != null ? _Patient.NOTE : "";
                    toolTipController1.SetToolTip(lblNote, lblNote.Text);
                }

                if (_Treatment.IN_TREATMENT_TYPE_ID != null)
                {
                    var code = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(p => p.ID == _Treatment.IN_TREATMENT_TYPE_ID);
                    cboTreatmentType.EditValue = _Treatment.IN_TREATMENT_TYPE_ID;
                    txtTreatmentTypeCode.Text = code.TREATMENT_TYPE_CODE;
                }

                IcdInputADO inputIcd = new IcdInputADO();
                inputIcd.ICD_NAME = currentHisDepartmentTran.ICD_NAME;
                inputIcd.ICD_CODE = currentHisDepartmentTran.ICD_CODE;

                if (ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, inputIcd);
                }

                IcdInputADO inputIcdYhct = new IcdInputADO();
                inputIcdYhct.ICD_NAME = !String.IsNullOrWhiteSpace(currentHisDepartmentTran.TRADITIONAL_ICD_NAME) ? currentHisDepartmentTran.TRADITIONAL_ICD_NAME : (_Treatment != null ? _Treatment.TRADITIONAL_ICD_NAME : "");
                inputIcdYhct.ICD_CODE = !String.IsNullOrWhiteSpace(currentHisDepartmentTran.TRADITIONAL_ICD_CODE) ? currentHisDepartmentTran.TRADITIONAL_ICD_CODE : (_Treatment != null ? _Treatment.TRADITIONAL_ICD_CODE : "");
                if (ucIcdYhct != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Reload");
                    icdYhctProcessor.Reload(ucIcdYhct, inputIcdYhct);
                }

                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = this.currentHisDepartmentTran.ICD_SUB_CODE;
                subIcd.ICD_TEXT = this.currentHisDepartmentTran.ICD_TEXT;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }

                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcdYhct = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                subIcdYhct.ICD_SUB_CODE = this.currentHisDepartmentTran.TRADITIONAL_ICD_SUB_CODE;
                subIcdYhct.ICD_TEXT = this.currentHisDepartmentTran.TRADITIONAL_ICD_TEXT;
                if (ucSecondaryIcdYhct != null)
                {
                    subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subIcdYhct);
                }
                //load thong tin vao nt
                if (!string.IsNullOrEmpty(this.TreatmentWithPaTyInfo.HOSPITALIZE_REASON_CODE) && !string.IsNullOrEmpty(this.TreatmentWithPaTyInfo.HOSPITALIZE_REASON_NAME))
                {
                    
                    var isExitsReason = this.listReason.FirstOrDefault(s => s.HOSPITALIZE_REASON_CODE == this.TreatmentWithPaTyInfo.HOSPITALIZE_REASON_CODE);
                    if(isExitsReason != null)
                    {
                        cboReasonNt.EditValue = isExitsReason.ID;
                        this.REASON_CODE = isExitsReason.HOSPITALIZE_REASON_CODE;
                        this.REASON_NAME = isExitsReason.HOSPITALIZE_REASON_NAME;
                    }
                }
                else if (!string.IsNullOrEmpty(this.TreatmentWithPaTyInfo.HOSPITALIZE_REASON_NAME))
                {
                    txtReasonNt.Text = this.REASON_NAME = this.TreatmentWithPaTyInfo.HOSPITALIZE_REASON_NAME;
                }
                dtLogTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtLogTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentTypeCode.SelectAll();
                    txtTreatmentTypeCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void txtTreatmentTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDataTreatmentTypeCombo(strValue, false, cboTreatmentType, txtTreatmentTypeCode, txtPatientReceive);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                            txtPatientReceive.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBedRoom.EditValue != null)
                    {
                        // MOS.Filter.HisBedRoomView1Filter
                        MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtBedRoomCode.Text = data.BED_ROOM_CODE;
                            cboBedRoom.Properties.Buttons[1].Visible = true;
                            LoadCboGiuong();
                            icdProcessor.FocusControl(ucIcd);
                        }
                    }
                    else
                    {
                        icdProcessor.FocusControl(ucIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentType.EditValue = null;
                    txtTreatmentTypeCode.Text = "";
                    txtTreatmentTypeCode.Focus();
                    txtTreatmentTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBedRoom.Properties.Buttons[1].Visible = false;
                    cboBedRoom.EditValue = null;
                    txtBedRoomCode.Text = "";
                    txtBedRoomCode.Focus();
                    txtBedRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                            txtBedRoomCode.Focus();
                            txtBedRoomCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBedRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtBedRoomCode.Text = data.BED_ROOM_CODE;
                            cboBedRoom.Properties.Buttons[1].Visible = true;
                            LoadCboGiuong();
                            icdProcessor.FocusControl(ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBedRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDataBedRoomCombo(strValue, false, cboBedRoom, txtBedRoomCode, btnSave);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    
                     ProcessDepartmentTranSaveClick(sender, e);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboBed_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CboBedService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBed.EditValue == null)
                        cboBed.ShowPopup();
                    else
                        CboBedService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<HisBedRoomV1> ProcessDataHisBedRoomV1(List<V_HIS_BED_ROOM_1> datas)
        {
            List<HisBedRoomV1> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedRoomV1>();
                    result.AddRange((from r in datas select new HisBedRoomV1(r)).ToList());
                    long _departmentId = WorkPlace.WorkPlaceSDO.Where(p => p.RoomId == this.currentModule.RoomId).FirstOrDefault().DepartmentId;
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisBedRoomView1Filter Filter = new MOS.Filter.HisBedRoomView1Filter();
                    Filter.DEPARTMENT_ID = _departmentId;
                    Filter.IS_ACTIVE = 1;
                    var resultHisBedRoom = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_BED_ROOM_1>>("/api/HisBedRoom/GetView1", ApiConsumers.MosConsumer, Filter, paramCommon);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dataBedLogs: ", resultHisBedRoom));
                    if (resultHisBedRoom != null && resultHisBedRoom.Count > 0)
                    {
                        foreach (var itemADO in result)
                        {

                            if (resultHisBedRoom != null && resultHisBedRoom.Count > 0)
                            {
                                if (itemADO.PATIENT_COUNT.HasValue)
                                {
                                    if (itemADO.PATIENT_COUNT > itemADO.BED_COUNT)
                                        itemADO.IsKey_ = 2;
                                    else
                                        itemADO.IsKey_ = 1;
                                }
                                else
                                { itemADO.IsKey_ = 1; }

                                itemADO.TT_PATIENT_BED_STR = (int)itemADO.PATIENT_COUNT + "/" + (int)itemADO.BED_COUNT;
                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private List<HisBedADO> ProcessDataBedAdo(List<V_HIS_BED> datas)
        {
            List<HisBedADO> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedADO>();
                    result.AddRange((from r in datas select new HisBedADO(r)).ToList());

                    long? timeFilter = null;
                    if (dtLogTime.EditValue != null && dtLogTime.DateTime != DateTime.MinValue)
                    {
                        timeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                    }

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();

                    int skip = 0;
                    while (bedIds.Count - skip > 0)
                    {
                        var listIds = bedIds.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;

                        MOS.Filter.HisBedLogFilter filter = new MOS.Filter.HisBedLogFilter();
                        filter.BED_IDs = listIds;
                        if (timeFilter.HasValue && timeFilter.Value > 0)
                        {
                            filter.START_TIME_TO = timeFilter;
                            filter.FINISH_TIME_FROM__OR__NULL = timeFilter;
                        }

                        CommonParam param = new CommonParam();
                        var dataBedLogs = new BackendAdapter(param).Get<List<HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dataBedLogs: ", dataBedLogs));
                        if (dataBedLogs != null && dataBedLogs.Count > 0)
                        {
                            var dataBedLogGroups = dataBedLogs.GroupBy(p => p.BED_ID).Select(p => p.ToList()).ToList();
                            foreach (var itemADO in result)
                            {
                                var dataByBedLogs = dataBedLogs.Where(p => p.BED_ID == itemADO.ID && ((!p.FINISH_TIME.HasValue) || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value > timeFilter))).ToList();
                                //Đây là đoạn xử lý chỗ hiển thị 0/1 đó. iss chú có sẵn hết dữ liệu rồi nên k cần làm như thế này đâu
                                // Bước 1 : tạo 1 cái ADO, trong đó sẽ custom lại giá trị hiện thị cột TÔNG/TỔNG
                                // Bước 2 : lấy được danh sách từ dữ liệu V_hIs_bed_room_1 thì xử lý add từng phần từ vào cái ADO đó, sau đó là tạo thành 1 List.
                                // Bước 3: Gắn lại cái list<ADO> đó vào datasource của cbo là xong.
                                // Nhớ phải tạo 1 biến như ở dưới là isKey ấy, để tí nữa còn tô màu và sắp xếp danh sách dựa vào nó.
                                if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                                {
                                    if (itemADO.MAX_CAPACITY.HasValue)
                                    {
                                        if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                            itemADO.IsKey = 2;
                                        else
                                            itemADO.IsKey = 1;
                                    }
                                    else
                                        itemADO.IsKey = 1;

                                    itemADO.AMOUNT = dataByBedLogs.Count;
                                    itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }



        private void gridView_CboBed_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {//Đoạn xử lý tô màu của giường đây
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
                    if (IsKey == 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (IsKey == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && dataBedADOs != null && dataBedADOs.Count > 0)
                {
                    HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "").ToString()));
                    if (row != null)
                    {
                        this.SpNamGhep.EditValue = null;

                        if (row.IsKey == 1)
                        {
                            if (currentTreatmentType != null && currentTreatmentType.IS_NOT_ALLOW_SHARE_BED == 1)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Bệnh nhân {0} không được phép nằm ghép giường", currentTreatmentType.TREATMENT_TYPE_NAME), ResourceLanguageManager.ThongBao);
                                cboBed.EditValue = null;
                                cbo.ShowPopup();
                                return;
                            }
                            else
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceLanguageManager.GiuongDaCoBenhNhanNam, ResourceLanguageManager.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    cboBed.EditValue = null;
                                    cboBed.ShowPopup();
                                    return;
                                }

                                this.SpNamGhep.Value = row.AMOUNT + 1;
                                LoadDataToCboBedServiceType(row);
                            }
                        }
                        else if (row.IsKey == 2)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceLanguageManager.GiuongDaVuotQuaSucChua, ResourceLanguageManager.ThongBao);
                            cboBed.EditValue = null;
                            cboBed.ShowPopup();
                        }

                        LoadDataToCboBedServiceType(row);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void SpNamGhep_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBed.EditValue = null;
                    SpNamGhep.EditValue = null;
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                    CboPrimaryPatientType.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                    CboPrimaryPatientType.ReadOnly = false;
                    cboServiceCondition.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && !CboPrimaryPatientType.ReadOnly)
                {
                    CboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboBedServiceType(ADO.HisBedADO row)
        {
            try
            {
                var currentServiceTypeByBeds = ProcessServiceRoom(row.ID);
                LoadComboEditor(CboBedService, "SERVICE_CODE", "SERVICE_NAME", "SERVICE_ID", currentServiceTypeByBeds);
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    currentServiceTypeByBeds = currentServiceTypeByBeds.OrderBy(p => p.SERVICE_CODE).ToList();
                    CboBedService.EditValue = currentServiceTypeByBeds[0].SERVICE_ID;
                }
                else
                {
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                }

                LoadComboServiceCondition();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, HisBedADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    long intructionTime = dtLogTime.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0 : 0;
                    long treatmentTime = this.currentTreatment.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var currentPatientTypeTemps = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas != null && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (Config.IsPrimaryPatientType != commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Config.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                        }
                        else if (Config.IsPrimaryPatientType == "0"
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && patientTypeId != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                        }
                        else if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == Config.PatientTypeId__BHYT
                        && ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > (intructionTime - (intructionTime % 1000000))
                        || (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < (intructionTime - (intructionTime % 1000000))
                        ))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == Config.PatientTypeId__VP);
                        }
                        else
                        {
                            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                        }

                        if (result != null && sereServADO != null)
                        {
                            CboPatientType.EditValue = result.ID;
                        }

                        var check = patientTypeIdAls.Contains(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);

                        if (check)
                        {
                            CboPatientType.EditValue = this.currentHisPatientTypeAlter.PATIENT_TYPE_ID;
                        }

                        if (Config.IsPrimaryPatientType == "2")
                        {
                            if (this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID <= 0)
                            {
                                CboPrimaryPatientType.EditValue = null;
                            }
                            else
                            {
                                CboPrimaryPatientType.ReadOnly = true;
                                CboPrimaryPatientType.EditValue = this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID;
                                if (currentPatientTypeTemps.Exists(e => e.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID))
                                {
                                    var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID);
                                    CboPrimaryPatientType.EditValue = priPaty.ID;
                                }
                                else
                                {
                                    CboPrimaryPatientType.ReadOnly = false;
                                    try
                                    {
                                        var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.TreatmentWithPaTyInfo.PRIMARY_PATIENT_TYPE_ID);
                                        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                        MessageBox.Show(string.Format(ResourceLanguageManager.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName));
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                    }
                                }
                            }
                        }
                        else if (Config.IsPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && result.ID != sereServADO.BILL_PATIENT_TYPE_ID.Value)
                        {
                            if (currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            {
                                var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                CboPrimaryPatientType.EditValue = priPaty.ID;
                            }
                            else
                            {
                                try
                                {
                                    var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                    MessageBox.Show(String.Format(ResourceLanguageManager.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName));
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                        }
                        else if (Config.IsPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Config.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            CboPrimaryPatientType.EditValue = priPaty.ID;
                        }
                        else
                        {
                            CboPrimaryPatientType.EditValue = null;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO), sereServADO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_SERVICE_ROOM> ProcessServiceRoom(long bedId)
        {
            List<V_HIS_SERVICE_ROOM> result = null;
            try
            {
                if (cboBedRoom.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn buồng");
                    return null;
                }

                CommonParam param = new CommonParam();

                var lstBedServiceTypes = hisBedBstys.Where(o => o.BED_ID == bedId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<HisBedADO> hisBed = dataBedADOs != null && dataBedADOs.Count > 0 ? dataBedADOs.Where(o => o.BED_ROOM_ID == long.Parse(cboBedRoom.EditValue.ToString())).ToList() : null;
                if (hisBed != null && hisBed.Count > 0)
                {
                    lstBedServiceTypes = lstBedServiceTypes.Where(o => hisBed.Select(s => s.ID).Contains(o.BED_ID)).ToList();
                }

                List<long> bedServiceTypeIds = new List<long>();
                if (lstBedServiceTypes != null && lstBedServiceTypes.Count > 0)
                {
                    bedServiceTypeIds = lstBedServiceTypes.Select(p => p.BED_SERVICE_TYPE_ID).ToList();
                }

                var lstBedServiceTypeByBedId = VHisBedServiceTypes.Where(p => bedServiceTypeIds.Contains(p.ID)).ToList();
                List<long> serviceIds = new List<long>();
                if (lstBedServiceTypeByBedId != null && lstBedServiceTypeByBedId.Count > 0)
                {
                    serviceIds = lstBedServiceTypeByBedId.Select(p => p.ID).ToList();
                }

                result = ListServiceBedByRooms.Where(p => serviceIds.Contains(p.SERVICE_ID)).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadCurrentPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter filter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = this.treatmentId;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                this.currentHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
                {
                    if (this.currentHisPatientTypeAlter != null)
                    {
                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                        {
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                        }
                    }
                }

                //var lstPatientTypeAll = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => this.currentPatientTypeWithPatientTypeAlter.Where(p => p.ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).ToList().Exists(t => t.BASE_PATIENT_TYPE_ID == o.ID)).ToList();

                //if (lstPatientTypeAll != null && lstPatientTypeAll.Count > 0)
                //{
                //    this.currentPatientTypeWithPatientTypeAlter.AddRange(lstPatientTypeAll);
                //}

                var patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToArray();

                //Lọc các đối tượng thanh toán không có chính sách giá
                this.patientTypeIdAls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIds.Contains(t.PATIENT_TYPE_ID) && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();

                //MOS.Filter.HisServicePatyViewFilter ServicePatyfilter = new MOS.Filter.HisServicePatyViewFilter();

                //var checkpatientTypeAll = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>("api/HisServicePaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, ServicePatyfilter, null);

                if (this.currentPatientTypeWithPatientTypeAlter != null && this.currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    foreach (var item in this.currentPatientTypeWithPatientTypeAlter)
                    {
                        var checkpatientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();

                        var checkBasePatientTypeId = checkpatientTypeAll.Where(t => t.PATIENT_TYPE_ID == item.BASE_PATIENT_TYPE_ID).ToList();

                        var checkpatientTypeId = (checkBasePatientTypeId != null && checkBasePatientTypeId.Count > 0) ? checkBasePatientTypeId.Where(t => t.INHERIT_PATIENT_TYPE_IDS != null && ("," + t.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + item.ID + ",")).ToList() : null;
                        if (checkpatientTypeId != null && checkpatientTypeId.Count > 0)
                        {
                            patientTypeIdAls.Add(item.ID);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("patientTypeIdAls: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdAls), patientTypeIdAls));

                this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => this.patientTypeIdAls.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadHistreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = this.treatmentId;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPaTyInfo = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboBedService.EditValue != null)
                {
                    ReloadPatientType();
                    ReloadPrimaryPatientType();

                    HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString()));
                    if (row != null)
                    {
                        var bedType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString()));
                        row.BILL_PATIENT_TYPE_ID = bedType != null ? bedType.BILL_PATIENT_TYPE_ID : null;
                        ChoosePatientTypeDefaultlService(currentTreatment.TDL_PATIENT_TYPE_ID ?? 0, Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString()), row);
                    }
                }

                ProcessPrice();
                LoadComboServiceCondition();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboPatientType.EditValue != null)
                {
                    ReloadPrimaryPatientType();
                }
                ProcessPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadPrimaryPatientType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                if (CboBedService.EditValue != null && BranchDataWorker.HasServicePatyWithListPatientType(long.Parse((CboBedService.EditValue ?? 0).ToString()), this.patientTypeIdAls))
                {
                    long instructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                    long? intructionNumByType = 1;
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(long.Parse(CboBedService.EditValue.ToString()), this.patientTypeIdAls);
                    var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPatientType.EditValue ?? 0).ToString()), null, intructionNumByType);

                    var patyIds = servicePaties.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                    foreach (var item in patyIds)
                    {
                        if (item == long.Parse((CboPatientType.EditValue ?? 0).ToString()))
                            continue;
                        var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), item, null, intructionNumByType);
                        if (itemPaty == null || currentPaty == null || (currentPaty.PRICE * (1 + currentPaty.VAT_RATIO)) >= (itemPaty.PRICE * (1 + itemPaty.VAT_RATIO)))
                            continue;
                        dataCombo.Add(this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == item));
                    }
                }

                CboPrimaryPatientType.Properties.DataSource = dataCombo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadPatientType()
        {
            try
            {
                long intructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                long treatmentTime = this.TreatmentWithPaTyInfo.IN_TIME;
                long serviceId = long.Parse((CboBedService.EditValue ?? 0).ToString());
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
                if (BranchDataWorker.HasServicePatyWithListPatientType(serviceId, this.patientTypeIdAls))
                {
                    var arrPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[serviceId].Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_CODE).Distinct().ToList();

                    if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                    {
                        foreach (var item in currentPatientTypeWithPatientTypeAlter)
                        {
                            var arrInheritPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[serviceId].Where(o => (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + item.ID + ","))).ToList();

                            if (arrInheritPatientTypeCode != null && arrInheritPatientTypeCode.Count > 0)
                            {
                                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == item.ID).Select(s => s.PATIENT_TYPE_CODE).ToList();
                                arrPatientTypeCode.AddRange(patientTypes);
                            }
                        }
                    }

                    arrPatientTypeCode = arrPatientTypeCode.Distinct().ToList();

                    dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);

                }

                CboPatientType.Properties.DataSource = dataCombo;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_SERVICE_PATY data_ServicePriceDefault = new V_HIS_SERVICE_PATY();
        private void ProcessPrice()
        {
            try
            {
                LblPrice.Text = "0";
                if (CboPatientType.EditValue != null && CboBedService.EditValue != null)
                {
                    long instructionTime = dtLogTime.EditValue != null ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(long.Parse(CboBedService.EditValue.ToString()), this.patientTypeIdAls);
                    if (CboPrimaryPatientType.EditValue != null)
                    {
                        data_ServicePriceDefault = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPrimaryPatientType.EditValue ?? 0).ToString()), null);
                    }
                    else
                    {
                        data_ServicePriceDefault = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.TreatmentWithPaTyInfo.IN_TIME, long.Parse((CboBedService.EditValue ?? 0).ToString()), long.Parse((CboPatientType.EditValue ?? 0).ToString()), null);
                    }

                    if (data_ServicePriceDefault != null)
                    {
                        LblPrice.Text = Inventec.Common.Number.Convert.NumberToString((data_ServicePriceDefault.PRICE * (1 + data_ServicePriceDefault.VAT_RATIO)), ConfigApplications.NumberSeperator);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CboPatientType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboBedService.EditValue == null)
                        CboBedService.ShowPopup();
                    else
                        CboPatientType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (LciPrimaryPatientType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        SpNamGhep.Focus();
                    }
                    else
                    {
                        CboPrimaryPatientType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboPatientType.EditValue == null)
                        CboPatientType.ShowPopup();
                    else
                    {
                        if (LciPrimaryPatientType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        {
                            SpNamGhep.Focus();
                        }
                        else
                        {
                            CboPrimaryPatientType.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SpNamGhep.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboPatientType.EditValue == null)
                        CboPrimaryPatientType.ShowPopup();
                    else
                    {
                        SpNamGhep.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtLogTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit2View_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey_") ?? "0").ToString());
                    if (IsKey == 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (IsKey == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPATIENT_CLASSIFY_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDataToComboPATIENT_CLASSIFY(cboPATIENT_CLASSIFY, strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPATIENT_CLASSIFY_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    // cboPATIENT_CLASSIFY.Properties.Buttons[1].Visible = false;
                    cboPATIENT_CLASSIFY.EditValue = null;
                    txtPATIENT_CLASSIFY.Text = "";
                    txtPATIENT_CLASSIFY.Focus();
                    //txtPATIENT_CLASSIFY.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPATIENT_CLASSIFY_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPATIENT_CLASSIFY.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY data = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPATIENT_CLASSIFY.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtPATIENT_CLASSIFY.Text = data.PATIENT_CLASSIFY_CODE;
                            txtPATIENT_CLASSIFY.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerInit_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadDataBedService();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick => 1...");
                timerInit.Stop();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTreatmentType.EditValue != null)
                {
                    long _departmentId = WorkPlace.WorkPlaceSDO.Where(p => p.RoomId == this.currentModule.RoomId).FirstOrDefault().DepartmentId;
                    var treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentType.EditValue.ToString());
                    currentTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(p => p.ID == treatmentTypeId);
                    CommonParam paramCommon = new CommonParam();

                    MOS.Filter.HisBedRoomView1Filter Filter = new MOS.Filter.HisBedRoomView1Filter();
                    Filter.DEPARTMENT_ID = _departmentId;
                    Filter.IS_ACTIVE = 1;
                    var resultHisBedRoom = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_BED_ROOM_1>>("/api/HisBedRoom/GetView1", ApiConsumers.MosConsumer, Filter, paramCommon);

                    List<V_HIS_BED_ROOM_1> lstBedRoomTemp = new List<V_HIS_BED_ROOM_1>();
                    foreach (var item in resultHisBedRoom)
                    {
                        if (string.IsNullOrEmpty(item.TREATMENT_TYPE_IDS))
                        {
                            lstBedRoomTemp.Add(item);
                        }
                        else
                        {
                            var lstIds = item.TREATMENT_TYPE_IDS.Split(',').ToList();
                            if (lstIds.FirstOrDefault(o => o == cboTreatmentType.EditValue.ToString()) != null)
                            {
                                lstBedRoomTemp.Add(item);
                            }
                        }
                    }
                    resultHisBedRoom = lstBedRoomTemp;

                    dataHisBedRoomV1ADOs = ProcessDataHisBedRoomV1(resultHisBedRoom);

                    HIS_DEPARTMENT _department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _departmentId);
                    LoadDataToComboBedRoom(cboBedRoom, dataHisBedRoomV1ADOs);

                    if (listTreatmentType.Contains(Convert.ToInt64(cboTreatmentType.EditValue)))
                    {

                        this.lciReasonNt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        if(this.listReason.Count == 0)
                            LoadDataToCboReasonNt();
                    }
                    else
                    {
                        this.lciReasonNt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientReceive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter && patientTypes != null && patientTypes.Count > 0)
                {
                    if (!string.IsNullOrEmpty(txtPatientReceive.Text.Trim()))
                    {
                        txtPatientReceive.Text = txtPatientReceive.Text.Trim();
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == txtPatientReceive.Text.Trim());
                        if (patientType != null)
                        {
                            cboPatientReceive.EditValue = patientType.ID;
                        }
                        else
                        {
                            cboPatientReceive.Focus();
                            cboPatientReceive.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPatientReceive.Focus();
                        cboPatientReceive.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientReceive_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (patientTypes != null && patientTypes.Count > 0)
                {
                    if (cboPatientReceive.EditValue != null)
                    {
                        txtPatientReceive.Text = patientTypes.FirstOrDefault(o => o.ID == Int64.Parse(cboPatientReceive.EditValue.ToString())).PATIENT_TYPE_CODE;
                        txtBedRoomCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }
        ServicePatyConditionADO PatyCondition = new ServicePatyConditionADO();
        List<ServicePatyConditionADO> PatyConditionList = new List<ServicePatyConditionADO>();
        private void InitComboServiceCondition(List<ServicePatyConditionADO> lstServicePaty)
        {

            try
            {
                PatyConditionList = lstServicePaty;
                cboServiceCondition.Properties.DataSource = lstServicePaty;
                cboServiceCondition.Properties.DisplayMember = "SERVICE_CONDITION_NAME";
                cboServiceCondition.Properties.ValueMember = "ID";

                cboServiceCondition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceCondition.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceCondition.Properties.ImmediatePopup = true;
                cboServiceCondition.ForceInitialize();
                cboServiceCondition.Properties.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_NAME");
                aColumnName.Caption = "Tên";
                DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1 = new RepositoryItemMemoEdit();
                repositoryItemMemoEdit1.Appearance.Options.UseTextOptions = true;
                repositoryItemMemoEdit1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
                aColumnName.ColumnEdit = repositoryItemMemoEdit1;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 250;

                DevExpress.XtraGrid.Columns.GridColumn aColumnHein = cboServiceCondition.Properties.View.Columns.AddField("HEIN_RATIO_DISPLAY");
                aColumnHein.Caption = "Tỉ lệ thanh toán";
                aColumnHein.Visible = true;
                aColumnHein.VisibleIndex = 2;
                aColumnHein.Width = 100;

                cboServiceCondition.Properties.PopupFormSize = new Size(500, 320);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void cboServiceCondition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString()));
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (CboBedService.EditValue == null || cboBed.EditValue == null || row == null || row.IS_BED_STRETCHER != 1)
                    {
                        InitComboServiceCondition(null);
                    }
                    cboServiceCondition.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboServiceCondition()
        {

            try
            {
                HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString()));
                if (CboBedService.EditValue == null || cboBed.EditValue == null || row == null || row.IS_BED_STRETCHER != 1)
                {
                    InitComboServiceCondition(null);
                    cboServiceCondition.EditValue = null;
                    PatyCondition = new ServicePatyConditionADO();
                    return;
                }
                var serviceId = long.Parse(CboBedService.EditValue.ToString());
                var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls);
                if (dataCondition != null && dataCondition.Count > 0)
                {
                    dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0).ToList();
                    if (dataCondition != null && dataCondition.Count > 0)
                    {
                        List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                        foreach (var item in dataCondition)
                        {
                            if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_ID == item.SERVICE_CONDITION_ID))
                            {
                                dataConditionTmps.Add(item);
                            }
                        }
                        dataCondition.Clear();
                        dataCondition.AddRange(dataConditionTmps);
                    }
                }
                if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                {
                    dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == serviceId && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                }
                AutoMapper.Mapper.CreateMap<V_HIS_SERVICE_PATY, ServicePatyConditionADO>();
                InitComboServiceCondition(AutoMapper.Mapper.Map<List<ServicePatyConditionADO>>(dataCondition));
                if (dataCondition != null && dataCondition.Count == 1)
                {
                    cboServiceCondition.EditValue = dataCondition[0].ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboServiceCondition_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboServiceCondition.EditValue == null)
                {
                    PatyCondition = new ServicePatyConditionADO();
                    LblPrice.Text = data_ServicePriceDefault != null ? Inventec.Common.Number.Convert.NumberToString((data_ServicePriceDefault.PRICE * (1 + data_ServicePriceDefault.VAT_RATIO)), ConfigApplications.NumberSeperator) : "0";
                }
                else
                {
                    PatyCondition = PatyConditionList.FirstOrDefault(o => o.ID == (long)cboServiceCondition.EditValue);
                    LblPrice.Text = Inventec.Common.Number.Convert.NumberToString((PatyCondition.PRICE * (1 + (PatyCondition.VAT_RATIO))), ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private bool ValidSereServWithCondition()
        {
            bool valid = true;
            try
            {
                if (dataBedADOs == null)
                {
                    LogSystem.Debug("dataBedADOs is null");
                    return valid;
                }
                HisBedADO row = (dataBedADOs ?? new List<HisBedADO>()).FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString()));
                if (row == null || (row != null && row.IS_BED_STRETCHER != 1))
                    return valid;
                if (CboBedService.EditValue != null)
                {
                    string sereServConditionStr = "";
                    var service = (VHisBedServiceTypes ?? new List<V_HIS_SERVICE>()).FirstOrDefault(o=> o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString()));
                    var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(service.ID, new List<long> { (long)cboPatientReceive.EditValue });
                    if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                    {
                        dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == service.ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                    }
                    if (dataCondition != null && dataCondition.Count > 0 && dataCondition.Exists(t => t.SERVICE_CONDITION_ID.HasValue && t.SERVICE_CONDITION_ID > 0) && !dataCondition.Exists(t => t.SERVICE_CONDITION_ID == null || t.SERVICE_CONDITION_ID == 0))
                    {
                        dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == service.ID).ToList();
                        if (dataCondition != null && dataCondition.Count > 0)
                        {
                            List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                            foreach (var itemCon in dataCondition)
                            {
                                if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                {
                                    dataConditionTmps.Add(itemCon);
                                }
                            }
                            dataCondition.Clear();
                            dataCondition.AddRange(dataConditionTmps);
                        }
                    }
                    else
                    {
                        dataCondition = null;
                    }
                    if (dataCondition != null && dataCondition.Count > 0 && PatyCondition.ID <= 0)
                    {
                        sereServConditionStr = service.SERVICE_NAME;
                    }

                    if (!String.IsNullOrEmpty(sereServConditionStr))
                    {
                        MessageBox.Show(string.Format("Dịch vụ {0} chưa được nhập điều kiện", sereServConditionStr), "Thông báo");
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        #region loadDoctor, save Doctor Info
        private List<HIS_EMPLOYEE> listDoctor = new List<HIS_EMPLOYEE>();
        private HIS_TREATMENT updateDTO = new HIS_TREATMENT();
        private void loadDoctor()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, param);
                listDoctor.AddRange(data.Where(s => s.IS_DOCTOR == 1 && !string.IsNullOrEmpty(s.LOGINNAME) && !string.IsNullOrEmpty(s.TDL_USERNAME)).Distinct().ToList());
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Mã", 100, 2));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Tên", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "TDL_USERNAME", columnInfos, false, 300);
                ControlEditorLoader.Load(cbboDoctor, listDoctor, controlEditorADO);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        

        private void cbbDoctor_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cbboDoctor.EditValue != null)
                {
                    var selectedDoctor = listDoctor.Where(s => s.TDL_USERNAME == cbboDoctor.EditValue.ToString()).FirstOrDefault();
                    updateDTO.DOCTOR_LOGINNAME = selectedDoctor.LOGINNAME;
                    updateDTO.DOCTOR_USERNAME = selectedDoctor.TDL_USERNAME;
                    cbbDoctor.Text = selectedDoctor.LOGINNAME;
                    cbboDoctor.Properties.DisplayMember = "TDL_USERNAME";
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
       
        private void cbbDoctor_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        

        private void cbboDoctor_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbboDoctor.EditValue != null)
                {
                    var selectedDoctor = listDoctor.Where(s => s.TDL_USERNAME == cbboDoctor.EditValue.ToString()).FirstOrDefault();
                    if (selectedDoctor != null)
                    {
                        this.doctorLoginname = selectedDoctor.LOGINNAME;
                        this.doctorUsername = selectedDoctor.TDL_USERNAME;
                        cbbDoctor.Text = selectedDoctor.LOGINNAME;
                        cbboDoctor.Properties.DisplayMember = "TDL_USERNAME";
                    } 
                }
                else
                {
                    cbbDoctor.Text = "";
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
       

        private void cbbDoctor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = listDoctor.FirstOrDefault(s => s.LOGINNAME == cbbDoctor.Text);
                    if (data != null)
                    {
                        cbboDoctor.EditValue = data.TDL_USERNAME;
                    }
                }
                if (string.IsNullOrEmpty(cbbDoctor.Text))
                {
                    cbboDoctor.EditValue = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void cbbDoctor_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var data = listDoctor.FirstOrDefault(s => s.LOGINNAME == cbbDoctor.Text);
                if (data != null)
                {
                    cbboDoctor.EditValue = data.TDL_USERNAME;
                }

                if (string.IsNullOrEmpty(cbbDoctor.Text))
                {
                    cbboDoctor.EditValue = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion
        public string REASON_CODE { get; set; }
        public string REASON_NAME { get; set; }
        public List<HIS_HOSPITALIZE_REASON> listReason = new List<HIS_HOSPITALIZE_REASON>();
        private void txtReasonNt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboReasonNt.ShowPopup();
                }
                else if(e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboReasonNt.EditValue = null;
                    txtReasonNt.Text = null;
                    REASON_CODE = REASON_NAME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataToCboReasonNt()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_HOSPITALIZE_REASON>();
                if(data != null)
                {
                    data = data.Where(s => s.IS_ACTIVE == 1).ToList();
                    listReason = data.ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_CODE", "Mã", 80, 1));
                    columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_NAME", "Tên", 190, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("HOSPITALIZE_REASON_NAME", "ID", columnInfos, false, 300);
                    ControlEditorLoader.Load(cboReasonNt, data, controlEditorADO);
                }
            }
            
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReasonNt_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboReasonNt_Closed(object sender, ClosedEventArgs e)
        {
            
        }

        private void txtReasonNt_Validated(object sender, EventArgs e)
        {
            
        }
        private void SetReasonHos()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtReasonNt.Text))
                {
                    var input = txtReasonNt.Text.Trim().ToLower(); // Chuyển input thành chữ thường

                    var validReasons = listReason
                        .Where(s => s.HOSPITALIZE_REASON_CODE.ToLower().Contains(input)  // Kiểm tra chuỗi con trong mã
                                 || s.HOSPITALIZE_REASON_NAME.ToLower().Contains(input)) // Kiểm tra chuỗi con trong tên
                        .ToList();

                    if (validReasons.Count >= 1)
                    {
                        cboReasonNt.Properties.DataSource = validReasons;

                        cboReasonNt.Properties.DisplayMember = "HOSPITALIZE_REASON_NAME";
                        cboReasonNt.Properties.ValueMember = "ID";

                        cboReasonNt.ShowPopup();
                    }
                    else if (validReasons.Count == 1) // Nếu chỉ có một kết quả
                    {
                        cboReasonNt.ClosePopup();
                        this.REASON_NAME = validReasons[0].HOSPITALIZE_REASON_NAME;
                        this.REASON_CODE = validReasons[0].HOSPITALIZE_REASON_CODE;
                    }
                    else
                    {
                        // Nếu không có kết quả phù hợp, giữ nguyên giá trị nhập vào
                        cboReasonNt.ClosePopup();
                        this.REASON_NAME = txtReasonNt.Text.Trim();
                        this.REASON_CODE = null;
                    }
                }
                else
                {
                    // Nếu input rỗng, hiển thị lại toàn bộ danh sách
                    cboReasonNt.Properties.DataSource = listReason;
                    cboReasonNt.ClosePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isSelected = false;
        private void cboReasonNt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboReasonNt.EditValue != null)
                {
                    var exits = listReason.FirstOrDefault(s => s.ID == Convert.ToInt64(cboReasonNt.EditValue));
                    if (exits != null)
                    {
                        txtReasonNt.Text = exits.HOSPITALIZE_REASON_NAME;
                        this.REASON_CODE = exits.HOSPITALIZE_REASON_CODE;
                        this.REASON_NAME = exits.HOSPITALIZE_REASON_NAME;
                        isSelected = true;
                    }
                    cboReasonNt.Properties.DataSource = listReason;
                }
                else
                    isSelected = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReasonNt_DoubleClick(object sender, EventArgs e)
        {
            cboReasonNt.ShowPopup();
        }

        private void txtReasonNt_Leave(object sender, EventArgs e)
        {

            try
            {
                SetReasonHos();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
