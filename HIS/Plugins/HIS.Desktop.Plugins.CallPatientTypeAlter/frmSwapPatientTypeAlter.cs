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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Config;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Loader;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.CallPatientTypeAlter.ADO;
using Inventec.Desktop.CustomControl;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Resources;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class frmSwapPatientTypeAlter : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module module;
        internal List<HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<V_HIS_SERE_SERV_4> lstHisSereServWithTreatment { get; set; }
        internal List<V_HIS_SERE_SERV_4> lstHisSereServWithTreatmentOld = new List<V_HIS_SERE_SERV_4>();
        internal List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        internal List<HIS_SERVICE> lstServiceBySereServ = new List<HIS_SERVICE>();
        long patient_type_id;
        long? patient_primary_patient_type_id;
        long? patient_classify_id;
        PatientTypeDepartmentADO HisTreatmentLogSDO = null;
        List<PatientTypeDepartmentADO> lstTreatmentLog = null;
        List<HIS_PATIENT_TYPE> dataCombo = null;
        HIS.Desktop.Common.DelegateReturnSuccess success;
        Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>> dicSevicepatyAllows = new Dictionary<long, List<V_HIS_SERVICE_PATY>>();
        long keyIsSetPrimaryPatientType = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__MOS_HIS_SERE_SERV_IS_SET_PRIMARY_PATIENT_TYPE));
        List<ServiceConditionADO> lstADO;
        private V_HIS_ROOM currentWorkingRoom;
        public frmSwapPatientTypeAlter(Inventec.Desktop.Common.Modules.Module module, long patient_type_id, long? patient_primary_patient_type_id, long? patient_classify_id, PatientTypeDepartmentADO HisTreatmentLogSDO, List<PatientTypeDepartmentADO> _lstTreatmentLog, List<V_HIS_SERE_SERV_4> _lstSereServ, HIS.Desktop.Common.DelegateReturnSuccess success)
        {
            InitializeComponent();
            this.module = module;
            this.patient_type_id = patient_type_id;
            this.patient_primary_patient_type_id = patient_primary_patient_type_id;
            this.patient_classify_id = patient_classify_id;
            this.HisTreatmentLogSDO = HisTreatmentLogSDO;
            this.lstTreatmentLog = _lstTreatmentLog;
            this.lstHisSereServWithTreatment = _lstSereServ;
            this.success = success;
        }

        private void frmSwapPatientTypeAlter_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                if (this.module != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.module.RoomId);
                }
                List<HIS_SERVICE_CONDITION> lstData = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstADO = new List<ServiceConditionADO>();
                foreach (var item in lstData)
                {
                    ServiceConditionADO ado = new ServiceConditionADO(item);
                    lstADO.Add(ado);
                }
                LoadServiceConditionDefault();
                Config.HisConfigCFG.InitWarningOverCeiling();
                SetCaptionByLanguageKey();
                GetServiceBySereServ();
                LoadDataToGridSereServ();
                LoadDataToPatientTypeRepositoryItemCombo(this.repositoryItemGridLookUpEdit, BackendDataWorker.Get<HIS_PATIENT_TYPE>());
                LoadDataToPatientTypeRepositoryItemComboSurcharge();
                lblUpdatePatientType.Text = string.Format("({0})", BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME);

                currentPatientTypeWithPatientTypeAlter = PatientTypeWithPatientTypeAlter();
                InitComboRespositoryPatientType(currentPatientTypeWithPatientTypeAlter);
                ProcessDataForUpdatePaty();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmSwapPatientTypeAlter
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(frmSwapPatientTypeAlter).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblUpdatePatientType.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.lblUpdatePatientType.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnUpdatePatientType.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.btnUpdatePatientType.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblNote.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.lblNote.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnChooSereServ.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.btnChooSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColServiceName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColAdditionRequire.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColAdditionRequire.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdColumnServiceCondition.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.grdColumnServiceCondition.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdColumnServiceCondition.ToolTip = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.grdColumnServiceCondition.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repServiceCondition.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repServiceCondition.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repositoryItemGridLookUpEdit.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditSurcharge.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repositoryItemGridLookUpEditSurcharge.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditSurchargeDis.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repositoryItemGridLookUpEditSurchargeDis.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repItemServiceCondition.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repItemServiceCondition.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetServiceBySereServ()
        {
            try
            {
                HisServiceFilter serviceFilter = new HisServiceFilter();
                serviceFilter.IDs = lstHisSereServWithTreatment.Select(o => o.SERVICE_ID).ToList();
                lstServiceBySereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, serviceFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToPatientTypeRepositoryItemComboSurcharge()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditSurcharge, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ADDITION == 1).ToList(), controlEditorADO);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditSurchargeDis, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ADDITION == 1).ToList(), controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void LoadDataToPatientTypeRepositoryItemCombo(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemcboPatientType, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridSereServ()
        {
            try
            {
                WaitingManager.Show();
                if (lstTreatmentLog != null)
                {
                    if (this.lstHisSereServWithTreatmentOld == null || this.lstHisSereServWithTreatmentOld.Count() == 0)
                    {
                        foreach (var item in lstHisSereServWithTreatment)
                        {
                            V_HIS_SERE_SERV_4 ss = new V_HIS_SERE_SERV_4();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_4>(ss, item);
                            lstHisSereServWithTreatmentOld.Add(ss);
                        }
                    }

                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = lstHisSereServWithTreatment;
                }
                else
                {
                    this.Close();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_4 data = (V_HIS_SERE_SERV_4)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChooSereServ_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateHisSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateHisSereServ()
        {
            try
            {
                gridViewSereServ.PostEditor();
                lstSereServ = new List<HIS_SERE_SERV>();
                var lstSereServww = (List<V_HIS_SERE_SERV_4>)gridControlSereServ.DataSource;


                decimal? price = null;
                string priceStr = "";
                string treatmentTypeStr = "";
                string configStr = "";
                bool checkConfig = false;

                var lstSereServBHYT = lstSereServww.Where(o => o.PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT).ToList();

                if (lstSereServBHYT != null && lstSereServBHYT.Count > 0)
                {
                    if (lstSereServBHYT.Exists(o => lstADO.Exists(p => p.SERVICE_ID == o.SERVICE_ID) && o.SERVICE_CONDITION_ID == null))
                    {
                        string message = string.Format(ResourceMessage.DichVuBatBuocChonDieuKien, String.Join(",", lstSereServBHYT.Where(o => lstADO.Exists(p => p.SERVICE_ID == o.SERVICE_ID) && o.SERVICE_CONDITION_ID == null).Select(o => o.TDL_SERVICE_NAME)));
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        gridViewSereServ.FocusedRowHandle = lstSereServww.IndexOf(lstSereServBHYT.FirstOrDefault(o => lstADO.Exists(p => p.SERVICE_ID == o.SERVICE_ID) && o.SERVICE_CONDITION_ID == null));
                        gridViewSereServ.FocusedColumn = grdColumnServiceCondition;
                        return;
                    }


                    price = lstSereServBHYT.Sum(o => o.VIR_PRICE);

                    if (this.HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        if (Config.HisConfigCFG.WarningOverCeiling__Exam > 0)
                        {
                            if (price > Config.HisConfigCFG.WarningOverCeiling__Exam)
                            {
                                configStr = Config.HisConfigCFG.WarningOverCeiling__Exam.ToString();
                                checkConfig = true;
                            }
                        }
                    }
                    else if (this.HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        if (Config.HisConfigCFG.WarningOverCeiling__In > 0)
                            if (price > Config.HisConfigCFG.WarningOverCeiling__In)
                            {
                                configStr = Config.HisConfigCFG.WarningOverCeiling__In.ToString();
                                checkConfig = true;
                            }
                    }
                    else if (this.HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        if (Config.HisConfigCFG.WarningOverCeiling__Out > 0)
                        {
                            if (price > Config.HisConfigCFG.WarningOverCeiling__Out)
                            {
                                configStr = Config.HisConfigCFG.WarningOverCeiling__Out.ToString();
                                checkConfig = true;
                            }
                        }
                    }
                }

                if (checkConfig)
                {
                    if (price.HasValue)
                    {
                        priceStr = price.ToString();
                    }

                    treatmentTypeStr = this.HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_NAME;

                    string message = string.Format(ResourceMessage.TongSoTienCuaCacDichVu, treatmentTypeStr, priceStr, configStr);

                    if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }


                bool result = false;
                CommonParam param = new CommonParam();
                HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();

                if (lstSereServww != null && lstSereServww.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_4, HIS_SERE_SERV>();
                    lstSereServ = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(lstSereServww);
                }

                sdo.SereServs = lstSereServ;
                sdo.Field = UpdateField.PATIENT_TYPE_ID;
                sdo.TreatmentId = this.HisTreatmentLogSDO.TREATMENT_ID;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var updateSs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, sdo, param);
                if (updateSs != null)
                {
                    result = true;
                    param.Messages = new List<string>();
                    this.success(true);
                    this.Close();
                }
                #region Show message
                MessageManager.Show(this, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckVuotQuaTran(List<V_HIS_SERE_SERV_4> data, ref decimal? price)
        {
            bool rs = false;
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return rs = false;
            }
            return rs;
        }

        private void gridViewSereServ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var rowHandle = gridViewSereServ.FocusedRowHandle;
                if (gridViewSereServ.SelectedRowsCount > 0)
                {
                    btnChooSereServ.Enabled = true;
                }
                else
                {
                    btnChooSereServ.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataForUpdatePaty()
        {
            try
            {
                var servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                var patienttpecodeAllows = currentPatientTypeWithPatientTypeAlter.Select(o => o.PATIENT_TYPE_CODE).ToList();
                servicePatyInBranchs = servicePatyInBranchs.Where(o => patienttpecodeAllows != null && patienttpecodeAllows.Contains(o.PATIENT_TYPE_CODE)).ToList();

                var setyGroups = servicePatyInBranchs.GroupBy(o => o.SERVICE_ID).ToList();
                if (setyGroups != null && setyGroups.Count > 0)
                {
                    foreach (var item in setyGroups)
                    {
                        dicSevicepatyAllows.Add(item.Key, item.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnUpdatePatientType_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_SERE_SERV_4> newLstSerSev = new List<V_HIS_SERE_SERV_4>();
                string mess = "";
                List<HIS_MEDICINE> MedicineList = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> MaterialList = new List<HIS_MATERIAL>();

                var medicineIdList = lstHisSereServWithTreatment
                    .Where(o => o.MEDICINE_ID.HasValue && o.MEDICINE_ID.Value > 0)
                    .Select(p => p.MEDICINE_ID.Value).Distinct().ToList();

                var materialIdList = lstHisSereServWithTreatment
                    .Where(o => o.MATERIAL_ID.HasValue && o.MATERIAL_ID.Value > 0)
                    .Select(p => p.MATERIAL_ID.Value).Distinct().ToList();

                if (medicineIdList != null && medicineIdList.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = medicineIdList;
                    MedicineList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (materialIdList != null && materialIdList.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = materialIdList;
                    MaterialList = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }

                foreach (var item in lstHisSereServWithTreatment)
                {
                    item.TDL_INTRUCTION_TIME = Int64.Parse(item.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) + "000000");
                }
                foreach (var item in lstHisSereServWithTreatment)
                {

                    var oldPatientTypeId = item.PATIENT_TYPE_ID;
                    var service = lstServiceBySereServ.Where(o => o.ID == item.SERVICE_ID).First();

                    if (item.MEDICINE_ID.HasValue && item.MEDICINE_ID.Value > 0
    && MedicineList != null && MedicineList.Count > 0)
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            var checkMedicine = MedicineList.FirstOrDefault(o => o.ID == item.MEDICINE_ID.Value);
                            var medicineType = checkMedicine != null
                                ? BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == checkMedicine.MEDICINE_TYPE_ID)
                                : null;
                            if (medicineType != null && !String.IsNullOrWhiteSpace(medicineType.ACTIVE_INGR_BHYT_CODE)
                                && (medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT))
                            {
                                if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                {
                                    DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                    long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                    if (exceedDay > 0)
                                        heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);


                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }

                                }
                                else if (HisTreatmentLogSDO != null)
                                {
                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }
                                }

                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else if (item.MATERIAL_ID.HasValue && item.MATERIAL_ID.Value > 0
                        && MaterialList != null && MaterialList.Count > 0)
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            var checkMaterial = MaterialList.FirstOrDefault(o => o.ID == item.MATERIAL_ID.Value);
                            var materialType = checkMaterial != null
                                ? BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == checkMaterial.MATERIAL_TYPE_ID)
                                : null;
                            if (materialType != null && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_CODE)
                                && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_NAME)
                                && (materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL))
                            {
                                if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                {
                                    DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                    long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                    if (exceedDay > 0)
                                        heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);


                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }

                                }
                                else if (HisTreatmentLogSDO != null)
                                {
                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else if (dicSevicepatyAllows.ContainsKey(item.SERVICE_ID))
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            if (!string.IsNullOrEmpty(item.TDL_HEIN_SERVICE_BHYT_CODE) && !string.IsNullOrEmpty(item.TDL_HEIN_SERVICE_BHYT_NAME))
                            {
                                var setyCheck = dicSevicepatyAllows[item.SERVICE_ID];
                                if (setyCheck != null && setyCheck.Count > 0)
                                {
                                    if (setyCheck.FirstOrDefault(o => o.PATIENT_TYPE_ID == patient_type_id) != null)
                                    {
                                        if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                        {
                                            DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                            long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                            if (exceedDay > 0)
                                                heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);

                                            if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                            {
                                                item.PATIENT_TYPE_ID = patient_type_id;
                                                item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);
                                                item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                            }

                                        }
                                        else if (HisTreatmentLogSDO != null)
                                        {
                                            if (item.IS_NOT_USE_BHYT != 1 && (HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && (HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME == null || item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)))
                                            {
                                                item.PATIENT_TYPE_ID = patient_type_id;
                                                item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);
                                                item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else
                    {
                        mess += item.TDL_SERVICE_CODE + ", ";
                    }
                    if (item.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                        item.SERVICE_CONDITION_ID = null;
                    else if (service.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        item.PATIENT_TYPE_ID = oldPatientTypeId;
                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == oldPatientTypeId).PATIENT_TYPE_NAME;
                    }
                    newLstSerSev.Add(item);
                }

                if (!string.IsNullOrEmpty(mess))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DVKhongTheChuyenDoi, mess), ResourceMessage.ThongBao);
                }

                gridControlSereServ.DataSource = null;
                gridControlSereServ.DataSource = newLstSerSev;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? ProcessPrimaryPatientTypeId(V_HIS_SERE_SERV_4 item, HIS_SERVICE service)
        {
            long? primaryPatientType = null;

            try
            {
                if (keyIsSetPrimaryPatientType == 2)
                {
                    primaryPatientType = patient_primary_patient_type_id != patient_type_id ? patient_primary_patient_type_id : null;
                }
                else if (keyIsSetPrimaryPatientType == 1)
                {
                    var setyCheck = dicSevicepatyAllows[item.SERVICE_ID];
                    if (setyCheck != null && setyCheck.Count > 0)
                    {
                        if (service != null && service.BILL_PATIENT_TYPE_ID != null && setyCheck.FirstOrDefault(o => o.PATIENT_TYPE_ID == service.BILL_PATIENT_TYPE_ID) != null
&& item.PATIENT_TYPE_ID != service.BILL_PATIENT_TYPE_ID && (service.APPLIED_PATIENT_TYPE_IDS == null || service.APPLIED_PATIENT_TYPE_IDS.Split(',').ToList().Contains(item.PATIENT_TYPE_ID.ToString()))
&& (service.APPLIED_PATIENT_CLASSIFY_IDS == null || (this.patient_classify_id != null && service.APPLIED_PATIENT_CLASSIFY_IDS.Split(',').ToList().Contains(patient_classify_id.ToString()))))
                        {
                            primaryPatientType = service.BILL_PATIENT_TYPE_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return primaryPatientType;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnChooSereServ_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_SERE_SERV_4 data = view.GetFocusedRow() as V_HIS_SERE_SERV_4;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataIntoPatientTypeCombo(data, editor);
                    editor.EditValue = data != null ? data.PATIENT_TYPE_ID : 0;
                }
                else if (view.FocusedColumn.FieldName == "SERVICE_CONDITION_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    LoadServiceConditionShow(data, editor);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(V_HIS_SERE_SERV_4 data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                var servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                if (servicePatyInBranchs != null && servicePatyInBranchs.Count > 0)
                {
                    var arrPatientTypeCode = servicePatyInBranchs.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.PATIENT_TYPE_CODE).ToList();
                    if (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0)
                    {
                        dataCombo = currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList();

                        if (string.IsNullOrEmpty(data.TDL_HEIN_SERVICE_BHYT_CODE) || string.IsNullOrEmpty(data.TDL_HEIN_SERVICE_BHYT_NAME) && dataCombo != null && dataCombo.Count > 0)
                        {
                            dataCombo = dataCombo.Where(o => o.PATIENT_TYPE_CODE != HisConfigCFG.PatientTypeCode__BHYT).ToList();
                        }

                        var service = lstServiceBySereServ.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                        if (service != null && dataCombo != null && dataCombo.Count > 0 && service.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                            dataCombo = dataCombo.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();

                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                        columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                        ControlEditorLoader.Load(patientTypeCombo, dataCombo, controlEditorADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_PATIENT_TYPE> PatientTypeWithPatientTypeAlter()
        {
            List<HIS_PATIENT_TYPE> result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_PATIENT_TYPE>();

                return result;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitComboRespositoryPatientType(List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 2));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit, currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_SERE_SERV_4 data = null;
                if (e.RowHandle > -1)
                {
                    data = (V_HIS_SERE_SERV_4)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PATIENT_TYPE_ID")
                        {
                            e.RepositoryItem = repositoryItemGridLookUpEdit;
                        }
                        if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                        {
                            var service = lstServiceBySereServ.Where(o => o.ID == data.SERVICE_ID).FirstOrDefault();

                            e.RepositoryItem = repositoryItemGridLookUpEditSurcharge;
                            e.RepositoryItem = this.keyIsSetPrimaryPatientType == 2 || (service != null && service.IS_NOT_CHANGE_BILL_PATY == 1) ? repositoryItemGridLookUpEditSurchargeDis : repositoryItemGridLookUpEditSurcharge;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_SERE_SERV_4)gridViewSereServ.GetFocusedRow();

                GridLookUpEdit dt = sender as GridLookUpEdit;
                var PatientTypeId = Convert.ToInt64(dt.EditValue);
                long? PrimaryPatientTypeId = null;


                // minhnq ---37143


                if (!string.IsNullOrEmpty((gridViewSereServ.GetRowCellValue(gridViewSereServ.FocusedRowHandle, "PRIMARY_PATIENT_TYPE_ID") ?? "").ToString()))
                {
                    PrimaryPatientTypeId = Convert.ToInt64((gridViewSereServ.GetRowCellValue(gridViewSereServ.FocusedRowHandle, "PRIMARY_PATIENT_TYPE_ID") ?? "").ToString());
                }
                long? oldPrimaryTypeId = lstHisSereServWithTreatmentOld.FirstOrDefault(o => o.ID == row.ID).PRIMARY_PATIENT_TYPE_ID;

                var service = lstServiceBySereServ.Where(o => o.ID == row.SERVICE_ID).First();



                Inventec.Common.Logging.LogSystem.Info("PatientTypeId___" + PatientTypeId);
                Inventec.Common.Logging.LogSystem.Info("PrimaryPatientTypeId___" + PrimaryPatientTypeId);
                Inventec.Common.Logging.LogSystem.Info("oldPrimaryTypeId___" + oldPrimaryTypeId);
                if (PatientTypeId == PrimaryPatientTypeId || PatientTypeId == oldPrimaryTypeId)
                {
                    gridViewSereServ.SetRowCellValue(gridViewSereServ.FocusedRowHandle, gridColAdditionRequire, null);
                }
                else if (oldPrimaryTypeId != null && string.IsNullOrEmpty((gridViewSereServ.GetRowCellValue(gridViewSereServ.FocusedRowHandle, "PRIMARY_PATIENT_TYPE_ID") ?? "").ToString()))
                {
                    gridViewSereServ.SetRowCellValue(gridViewSereServ.FocusedRowHandle, gridColAdditionRequire, oldPrimaryTypeId);
                }

                if (keyIsSetPrimaryPatientType == 2)
                {
                    gridViewSereServ.SetRowCellValue(gridViewSereServ.FocusedRowHandle, "PRIMARY_PATIENT_TYPE_ID", PrimaryPatientTypeId != PatientTypeId ? PrimaryPatientTypeId : null);
                }
                else if (keyIsSetPrimaryPatientType == 1)
                {
                    if (service == null || service.BILL_PATIENT_TYPE_ID == null || !HasServicePaty(row.SERVICE_ID, service.BILL_PATIENT_TYPE_ID)
                || patient_classify_id == null
|| PatientTypeId == service.BILL_PATIENT_TYPE_ID || (service.APPLIED_PATIENT_TYPE_IDS != null && !service.APPLIED_PATIENT_TYPE_IDS.Split(',').ToList().Contains(PatientTypeId.ToString()))
|| (service.APPLIED_PATIENT_CLASSIFY_IDS != null && !service.APPLIED_PATIENT_CLASSIFY_IDS.Split(',').ToList().Contains(patient_classify_id.ToString())))
                    {
                        gridViewSereServ.SetRowCellValue(gridViewSereServ.FocusedRowHandle, gridColAdditionRequire, null);
                    }
                    else
                    {
                        gridViewSereServ.SetRowCellValue(gridViewSereServ.FocusedRowHandle, gridColAdditionRequire, service.BILL_PATIENT_TYPE_ID);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool HasServicePaty(long serviceId, long? billPatientTypeId)
        {
            bool hasServicePaty = false;
            try
            {
                var setyCheck = dicSevicepatyAllows[serviceId];
                if (setyCheck != null && setyCheck.Count > 0)
                {
                    hasServicePaty = setyCheck.FirstOrDefault(o => o.PATIENT_TYPE_ID == billPatientTypeId) != null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return hasServicePaty;
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                keyIsSetPrimaryPatientType = 0;
                dicSevicepatyAllows = null;
                success = null;
                dataCombo = null;
                lstTreatmentLog = null;
                HisTreatmentLogSDO = null;
                patient_primary_patient_type_id = null;
                patient_type_id = 0;
                lstServiceBySereServ = null;
                currentPatientTypeWithPatientTypeAlter = null;
                lstHisSereServWithTreatmentOld = null;
                lstHisSereServWithTreatment = null;
                lstSereServ = null;
                module = null;
                this.btnUpdatePatientType.Click -= new System.EventHandler(this.btnUpdatePatientType_Click);
                this.btnChooSereServ.Click -= new System.EventHandler(this.btnChooSereServ_Click);
                this.gridViewSereServ.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewSereServ_CustomRowCellEdit);
                this.gridViewSereServ.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewSereServ_SelectionChanged);
                this.gridViewSereServ.ShownEditor -= new System.EventHandler(this.gridViewSereServ_ShownEditor);
                this.gridViewSereServ.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSereServ_CustomUnboundColumnData);
                this.repositoryItemGridLookUpEdit.EditValueChanged -= new System.EventHandler(this.repositoryItemGridLookUpEdit_EditValueChanged);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.Load -= new System.EventHandler(this.frmSwapPatientTypeAlter_Load);
                gridView2.GridControl.DataSource = null;
                repositoryItemGridLookUpEditSurchargeDis.DataSource = null;
                gridView1.GridControl.DataSource = null;
                repositoryItemGridLookUpEditSurcharge.DataSource = null;
                repositoryItemGridLookUpEdit1View.GridControl.DataSource = null;
                repositoryItemGridLookUpEdit.DataSource = null;
                gridViewSereServ.GridControl.DataSource = null;
                gridControlSereServ.DataSource = null;
                gridView2 = null;
                repositoryItemGridLookUpEditSurchargeDis = null;
                gridView1 = null;
                repositoryItemGridLookUpEditSurcharge = null;
                gridColAdditionRequire = null;
                layoutControlItem6 = null;
                lblUpdatePatientType = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                repositoryItemGridLookUpEdit1View = null;
                repositoryItemGridLookUpEdit = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                lblNote = null;
                btnUpdatePatientType = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                btnChooSereServ = null;
                Root = null;
                layoutControl2 = null;
                gridColPatientTypeName = null;
                gridColAmount = null;
                gridColServiceUnitName = null;
                gridColServiceName = null;
                gridColServiceCode = null;
                gridColSTT = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                gridViewSereServ = null;
                gridControlSereServ = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceConditionShow(V_HIS_SERE_SERV_4 row, GridLookUpEdit repServiceCondition)
        {
            try
            {
                repServiceCondition.Properties.DataSource = lstADO.Where(o => o.SERVICE_ID == row.SERVICE_ID).ToList();

                repServiceCondition.Properties.DisplayMember = "SERVICE_CONDITION_NAME";
                repServiceCondition.Properties.ValueMember = "ID";

                repServiceCondition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repServiceCondition.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repServiceCondition.Properties.ImmediatePopup = true;
                repServiceCondition.Properties.PopupFormSize = new Size(470, repServiceCondition.Properties.PopupFormSize.Height);
                repServiceCondition.Properties.View.Columns.Clear();
                repServiceCondition.Properties.View.OptionsView.RowAutoHeight = true;
                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = repServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_CODE");
                aColumnCode.Caption = ResourceMessage.Ma;
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;
                aColumnCode.ColumnEdit = repConditionCode;
                aColumnCode.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
                aColumnCode.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = repServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_NAME");
                aColumnName.Caption = ResourceMessage.Ten;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.ColumnEdit = repConditionName;
                aColumnName.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
                aColumnName.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                aColumnName.Width = 250;

                DevExpress.XtraGrid.Columns.GridColumn aColumnHein = repServiceCondition.Properties.View.Columns.AddField("HEIN_RATIO");
                aColumnHein.Caption = ResourceMessage.TLTT;
                aColumnHein.DisplayFormat.FormatString = "#,##0";
                aColumnHein.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                aColumnHein.Visible = true;
                aColumnHein.VisibleIndex = 3;
                aColumnHein.Width = 120;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceConditionDefault()
        {
            try
            {
                repServiceCondition.DataSource = lstADO;

                repServiceCondition.DisplayMember = "SERVICE_CONDITION_NAME";
                repServiceCondition.ValueMember = "ID";

                repServiceCondition.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repServiceCondition.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repServiceCondition.ImmediatePopup = true;
                repServiceCondition.PopupFormSize = new Size(470, repServiceCondition.PopupFormSize.Height);
                repServiceCondition.View.Columns.Clear();
                repServiceCondition.View.OptionsView.RowAutoHeight = true;
                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = repServiceCondition.View.Columns.AddField("SERVICE_CONDITION_CODE");
                aColumnCode.Caption = ResourceMessage.Ma;
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;
                aColumnCode.ColumnEdit = repConditionCode;
                aColumnCode.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
                aColumnCode.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = repServiceCondition.View.Columns.AddField("SERVICE_CONDITION_NAME");
                aColumnName.Caption = ResourceMessage.Ten;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.ColumnEdit = repConditionName;
                aColumnName.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
                aColumnName.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                aColumnName.Width = 250;

                DevExpress.XtraGrid.Columns.GridColumn aColumnHein = repServiceCondition.View.Columns.AddField("HEIN_RATIO");
                aColumnHein.Caption = ResourceMessage.TLTT;
                aColumnHein.DisplayFormat.FormatString = "#,##0";
                aColumnHein.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                aColumnHein.Visible = true;
                aColumnHein.VisibleIndex = 3;
                aColumnHein.Width = 120;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var sereServADO = (V_HIS_SERE_SERV_4)this.gridViewSereServ.GetFocusedRow();
                if (e.Column.FieldName == "PATIENT_TYPE_ID")
                {
                    if (sereServADO.PATIENT_TYPE_ID != Config.HisConfigCFG.PatientTypeId__BHYT)
                        sereServADO.SERVICE_CONDITION_ID = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
