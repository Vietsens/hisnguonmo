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
using DevExpress.Data.WcfLinq;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicineIsUsedPatient.ADO;
using HIS.Desktop.Plugins.MedicineIsUsedPatient.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.MedicineIsUsedPatient
{
    public partial class frmMedicineIsUsedPatient : FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private HIS_TREATMENT currentTreatment;
        #endregion

        public frmMedicineIsUsedPatient(Inventec.Desktop.Common.Modules.Module moduleData, string _treatmentCode)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.ModuleData = moduleData;
                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }
                this.txtTreatmentCode.Text = _treatmentCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmMedicineIsUsedPatient_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                InitComboStatus();

                // Cấu hình cho phép chọn giờ, phút, giây trong repDateEna
                if (repDateEna != null)
                {
                    repDateEna.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    repDateEna.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repDateEna.CalendarTimeProperties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repDateEna.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repDateEna.CalendarTimeProperties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repDateEna.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    repDateEna.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                    repDateEna.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
                    repDateEna.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
                }

                dtIntructionTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;
                dtIntructionTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindTreePlus(List<ExpMestMediMateADO> lstAdo)
        {
            try
            {
                List<ExpMestMediMateADO> SereServADOs = new List<ExpMestMediMateADO>();
                var listRootSety = lstAdo.GroupBy(g => g.SERVICE_REQ_CODE).ToList();
                foreach (var itemGr in listRootSety)
                {
                    var listBySety = itemGr.ToList<ExpMestMediMateADO>();
                    ExpMestMediMateADO ssRootSety = new ExpMestMediMateADO();
                    ssRootSety.MEDIMATE_TYPE_CODE = listBySety.FirstOrDefault().SERVICE_REQ_CODE;
                    ssRootSety.MEDIMATE_TYPE_NAME = listBySety.FirstOrDefault().REQUEST_LOGINNAME + " - " + listBySety.FirstOrDefault().REQUEST_USERNAME + " " + "(TG y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(listBySety.FirstOrDefault().INTRUCTION_TIME) + " - TG thực xuất:" + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(listBySety.FirstOrDefault().EXP_TIME ?? 0) + ')';
                    ssRootSety.IS_USED = false;
                    ssRootSety.CONCRETE_ID__IN_SETY = listBySety.FirstOrDefault().SERVICE_REQ_CODE ?? "";
                    ssRootSety.IS_PARENT = true;
                    if (listBySety.FirstOrDefault().USE_TIME.HasValue)
                    {
                        ssRootSety.SERVICE_UNIT_NAME = "Dự trù: " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(listBySety.FirstOrDefault().USE_TIME ?? 0);
                    }
                    SereServADOs.Add(ssRootSety);

                    int d = 0;
                    foreach (var item in listBySety)
                    {
                        d++;
                        item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                        item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                        SereServADOs.Add(item);
                    }
                }

                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.MEDIMATE_TYPE_NAME).ThenBy(o => o.MEDIMATE_TYPE_CODE).ToList();
                BindingList<ExpMestMediMateADO> records = new BindingList<ExpMestMediMateADO>(SereServADOs);
                treeMedicineIsUsePt.DataSource = records;
                treeMedicineIsUsePt.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void InitComboStatus()
        {
            try
            {
                List<FilterTypeADO> ListStatusAll = new List<FilterTypeADO>();
                FilterTypeADO tatCa = new FilterTypeADO(0, "Tất cả");
                ListStatusAll.Add(tatCa);

                FilterTypeADO daDung = new FilterTypeADO(1, "Đã dùng");
                ListStatusAll.Add(daDung);

                FilterTypeADO chuaDung = new FilterTypeADO(2, "Chưa dùng");
                ListStatusAll.Add(chuaDung);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboIsUse, ListStatusAll, controlEditorADO);
                cboIsUse.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultValueControl();
                this.treeMedicineIsUsePt.ClearNodes();
                currentTreatment = LoadSearch();
                FillInfoPatient(currentTreatment);
                LoadDataToTreeByTreatment(currentTreatment);
                if (currentTreatment == null)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(Resources.ResourceLanguageManager.KhongTimThayMaDieuTri);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeByTreatment(HIS_TREATMENT data)
        {
            List<ExpMestMediMateADO> lstAdo = new List<ExpMestMediMateADO>();
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.EXP_MEST_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT };
                expMestFilter.TDL_TREATMENT_ID = data != null ? data.ID : -1;
                var lstExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, null);
                if (lstExpMest != null && lstExpMest.Count > 0)
                {
                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.IS_ACTIVE = 1;
                    serviceReqFilter.IDs = lstExpMest.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                    if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.INTRUCTION_TIME_FROM = Convert.ToInt64(dtIntructionTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.INTRUCTION_TIME_TO = Convert.ToInt64(dtIntructionTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                    }
                    List<HIS_SERVICE_REQ> lstserviceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null);
                    if (lstserviceReq != null && lstserviceReq.Count > 0)
                    {
                        HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                        expMestMedicineFilter.IS_ACTIVE = 1;
                        expMestMedicineFilter.IS_EXPORT = true;
                        expMestMedicineFilter.EXP_MEST_IDs = lstExpMest.Select(p => p.ID).ToList();
                        expMestMedicineFilter.TDL_SERVICE_REQ_IDs = lstserviceReq.Select(p => p.ID).ToList();
                        if (cboIsUse.EditValue != null)
                        {
                            if ((int)cboIsUse.EditValue == 1)
                            {
                                expMestMedicineFilter.IS_USED = true;
                            }
                            else if ((int)cboIsUse.EditValue == 2)
                            {
                                expMestMedicineFilter.IS_USED = false;
                            }
                        }
                        var lstExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                        if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                        {
                            foreach (var item in lstExpMestMedicine)
                            {
                                ExpMestMediMateADO ado = new ExpMestMediMateADO();
                                //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(ado, item);
                                ado.IS_MATERIAL = false;
                                ado.IS_MEDICINE = true;
                                ado.ID = item.ID;

                                ado.MORNING = item.MORNING;
                                ado.NOON = item.NOON;
                                ado.AFTERNOON = item.AFTERNOON;
                                ado.EVENING = item.EVENING;
                                ado.MORNING_IS_USED = item.MORNING_IS_USED;
                                ado.NOON_IS_USED = item.NOON_IS_USED;
                                ado.AFTERNOON_IS_USED = item.AFTERNOON_IS_USED;
                                ado.EVENING_IS_USED = item.EVENING_IS_USED;

                                if (ado.IS_MEDICINE)
                                {
                                    SetstateCheck(ref ado, item);
                                }


                                ado.MEDIMATE_ID = item.MEDICINE_ID;
                                ado.MEDIMATE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                ado.MEDIMATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.EXP_MEST_MEDI_MATE_ID = item.ID;
                                ado.AMOUNT = item.AMOUNT;
                                ado.IS_USED = item.IS_USED == 1 ? true : false;
                                ado.USED_TIME = item.USED_TIME;
                                ado.EXP_TIME = item.EXP_TIME;
                                HIS_SERVICE_REQ servicereq = (lstserviceReq != null && lstserviceReq.Count > 0) ? lstserviceReq.FirstOrDefault(o => o.ID == item.TDL_SERVICE_REQ_ID) : null;
                                if (servicereq != null)
                                {
                                    ado.INTRUCTION_DATE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(servicereq.INTRUCTION_TIME);
                                    ado.INTRUCTION_TIME = servicereq.INTRUCTION_TIME;
                                    ado.SERVICE_REQ_CODE = servicereq.SERVICE_REQ_CODE;
                                    ado.REQUEST_LOGINNAME = servicereq.REQUEST_LOGINNAME;
                                    ado.REQUEST_USERNAME = servicereq.REQUEST_USERNAME;
                                    ado.USE_TIME = servicereq.USE_TIME;
                                }

                                lstAdo.Add(ado);
                            }
                        }

                        HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                        expMestMaterialFilter.IS_ACTIVE = 1;
                        expMestMaterialFilter.IS_EXPORT = true;
                        expMestMaterialFilter.EXP_MEST_IDs = lstExpMest.Select(p => p.ID).ToList();
                        expMestMaterialFilter.TDL_SERVICE_REQ_IDs = lstserviceReq.Select(p => p.ID).ToList();
                        if (cboIsUse.EditValue != null)
                        {
                            if ((int)cboIsUse.EditValue == 1)
                            {
                                expMestMaterialFilter.IS_USED = true;
                            }
                            else if ((int)cboIsUse.EditValue == 2)
                            {
                                expMestMaterialFilter.IS_USED = false;
                            }
                        }
                        var lstExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                        if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                        {
                            foreach (var item in lstExpMestMaterial)
                            {
                                ExpMestMediMateADO ado = new ExpMestMediMateADO();
                                ado.IS_MATERIAL = true;
                                ado.IS_MEDICINE = false;

                                ado.MEDIMATE_ID = item.MATERIAL_ID;
                                ado.MEDIMATE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                ado.MEDIMATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.EXP_MEST_MEDI_MATE_ID = item.ID;
                                ado.AMOUNT = item.AMOUNT;
                                ado.IS_USED = item.IS_USED == 1 ? true : false;
                                ado.USED_TIME = item.USED_TIME;
                                ado.EXP_TIME = item.EXP_TIME;
                                HIS_SERVICE_REQ servicereq = (lstserviceReq != null && lstserviceReq.Count > 0) ? lstserviceReq.FirstOrDefault(o => o.ID == item.TDL_SERVICE_REQ_ID) : null;
                                if (servicereq != null)
                                {
                                    ado.INTRUCTION_DATE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(servicereq.INTRUCTION_TIME);
                                    ado.INTRUCTION_TIME = servicereq.INTRUCTION_TIME;
                                    ado.SERVICE_REQ_CODE = servicereq.SERVICE_REQ_CODE;
                                    ado.REQUEST_LOGINNAME = servicereq.REQUEST_LOGINNAME;
                                    ado.REQUEST_USERNAME = servicereq.REQUEST_USERNAME;
                                    ado.USE_TIME = servicereq.USE_TIME;
                                }

                                lstAdo.Add(ado);
                            }
                        }
                    }
                }
                BindTreePlus(lstAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetstateCheck(ref ExpMestMediMateADO ado, V_HIS_EXP_MEST_MEDICINE item)
        {
            try
            {
                ado.MORNING_CHK = item.MORNING_IS_USED == 1 || (item.IS_USED == 1 && (item.MORNING != null && int.TryParse(item.MORNING.ToString(), out int morningValue) && morningValue > 0));
                ado.LUNCH_CHK = item.NOON_IS_USED == 1 || (item.IS_USED == 1 && (item.NOON != null && int.TryParse(item.NOON.ToString(), out int noon) && noon > 0));
                ado.AFTERNOON_CHK = item.AFTERNOON_IS_USED == 1 || (item.IS_USED == 1 && (item.AFTERNOON != null && int.TryParse(item.AFTERNOON.ToString(), out int afterNoon) && afterNoon > 0));
                ado.DINNER_CHK = item.EVENING_IS_USED == 1 || (item.IS_USED == 1 && (item.EVENING != null && int.TryParse(item.EVENING.ToString(), out int eve) && eve > 0));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void FillInfoPatient(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    lbPatientCode.Text = data.TDL_PATIENT_CODE;
                    lbPatientName.Text = data.TDL_PATIENT_NAME;
                    lbDateOfBirth.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lbGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lbAddress.Text = data.TDL_PATIENT_ADDRESS;

                    var LastPatientType = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, data.ID, null);
                    if (LastPatientType != null)
                    {
                        lbHeinCard.Text = HeinCardHelper.TrimHeinCardNumber(LastPatientType.HEIN_CARD_NUMBER);
                        lbDateFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        lbDateTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_TO_TIME ?? 0);
                        lbPlaceToTreat.Text = LastPatientType.HEIN_MEDI_ORG_NAME;
                        lbPatientType.Text = LastPatientType.PATIENT_TYPE_NAME ?? "";
                        string rightRoute = "";
                        if (LastPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = "Đúng tuyến";
                        }
                        else
                        {
                            rightRoute = "Trái tuyến";
                        }

                        lblRightRoute.Text = rightRoute ?? "";
                        string ratio = "";
                        if (LastPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(LastPatientType.HEIN_TREATMENT_TYPE_CODE, LastPatientType.HEIN_CARD_NUMBER, LastPatientType.LEVEL_CODE, LastPatientType.RIGHT_ROUTE_CODE);
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }

                        lblHeinRatio.Text = ratio ?? "";
                    }
                }
                else
                {
                    SetDefaultValueControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT LoadSearch()
        {
            HIS_TREATMENT result = new HIS_TREATMENT();
            try
            {
                CommonParam param = new CommonParam();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param)
                            .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        result = listTreatment.FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("____Result Treatment____", result));
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_TREATMENT();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                txtTreatmentCode.Text = "";
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                lbPatientCode.Text = "";
                lbPatientName.Text = "";
                lbHeinCard.Text = "";
                lblHeinRatio.Text = "";
                lbGender.Text = "";
                lbDateFrom.Text = "";
                lbDateOfBirth.Text = "";
                lbDateTo.Text = "";
                lbAddress.Text = "";
                lblRightRoute.Text = "";
                lbPlaceToTreat.Text = "";
                lbPatientType.Text = "";
                treeMedicineIsUsePt.ClearNodes();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWords_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMedicineIsUsePt_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data != null && data is ExpMestMediMateADO)
                {
                    var rowData = ((ExpMestMediMateADO)data);
                    if (!e.Node.HasChildren)
                    {
                        if (e.Column.FieldName == "IS_USED")
                        {
                            e.RepositoryItem = repositoryItemCheckEditUsed;
                        }
                        else if (e.Column.FieldName == "View")
                        {
                            e.RepositoryItem = repView;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void treeMedicineIsUsePt_CustomNodeCellEditForEditing(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data != null && data is ExpMestMediMateADO)
                {
                    if (!e.Node.HasChildren)
                    {
                        var rowData = ((ExpMestMediMateADO)data);
                        if (e.Column.FieldName == "USED_TIME_STR")
                        {
                            if (rowData.IS_USED.HasValue && rowData.IS_USED.Value)
                            {
                                e.RepositoryItem = repositoryUsedTime;
                            }
                            else
                            {
                                e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWords_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditUsed_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            repositoryItemEditValueChanging(sender, e, phaseIsUnUsed: null);
        }
        private void repositoryItemEditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e, long? phaseIsUnUsed = null)
        {
            try
            {
                var usedCheckEdit = sender as DevExpress.XtraEditors.CheckEdit;
                if (usedCheckEdit != null)
                {
                    bool isChecked = e.NewValue is bool isUsed && isUsed;
                    var treeList = usedCheckEdit.Parent as TreeList;
                    var rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ExpMestMediMateADO;

                    var useTime = isChecked ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) : null;
                    if (rowData != null && (!useTime.HasValue || rowData.INTRUCTION_TIME < useTime.Value))
                    {
                        bool success = false;
                        ExpMestMediMateADO dataUpdate = new ExpMestMediMateADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediMateADO>(dataUpdate, rowData);
                        dataUpdate.IS_USED = isChecked;
                        dataUpdate.USED_TIME = useTime;
                        success = this.UpdateHisExpMest(ref dataUpdate, isChecked, phaseIsUnUsed);
                        if (success)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediMateADO>(rowData, dataUpdate);
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        if (useTime.HasValue)
                        {
                            rowData.USED_TIME = useTime;
                            rowData.IS_USED = true;
                        }
                    }

                    //if (useTime.HasValue)
                   //     rowData.USED_TIME = useTime;

                    treeList.RefreshNode(treeList.FocusedNode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool UpdateHisExpMest(ref ExpMestMediMateADO data, bool isChecked, long? phaseIsUnUsed = null)
        {
            bool success = false;
            try
            {
                CommonParam param = new CommonParam();
                if (data != null)
                {
                    if (data.IS_MEDICINE)
                    {
                        HIS_EXP_MEST_MEDICINE lstexpmestmedicine = null;
                        if (isChecked)
                        {
                            var updateIsUsed = new MOS.SDO.HisExpMestMedicineIsUsedSDO();
                            updateIsUsed.ExpMestMedicineId = data.EXP_MEST_MEDI_MATE_ID;
                            updateIsUsed.PhaseIsUsed = phaseIsUnUsed;
                            updateIsUsed.UsedTime = data.USED_TIME;
                            lstexpmestmedicine = new BackendAdapter(param)
                                .Post<HIS_EXP_MEST_MEDICINE>("api/HisExpMestMedicine/Used", ApiConsumers.MosConsumer, updateIsUsed, param);
                        }
                        else
                        {
                            var updateUnUsed = new MOS.SDO.HisExpMestMedicineUnUsedSDO();
                            updateUnUsed.ExpMestMedicineId = data.EXP_MEST_MEDI_MATE_ID;
                            updateUnUsed.PhaseIsUnUsed = phaseIsUnUsed;
                            lstexpmestmedicine = new BackendAdapter(param)
                                .Post<HIS_EXP_MEST_MEDICINE>("api/HisExpMestMedicine/Unused", ApiConsumers.MosConsumer, updateUnUsed, param);
                        }
                        if (lstexpmestmedicine != null)
                        {
                            success = true;
                            data.IS_USED = lstexpmestmedicine.IS_USED == 1;
                            data.USED_TIME = lstexpmestmedicine.USED_TIME;
                            V_HIS_EXP_MEST_MEDICINE item = new V_HIS_EXP_MEST_MEDICINE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(item, lstexpmestmedicine);
                            SetstateCheck(ref data, item);
                        }
                    }
                    else
                    {
                        HIS_EXP_MEST_MATERIAL lstexpmestmaterial = null;
                        var update = new MOS.SDO.HisExpMestMaterialIsUsedSDO();
                        update.ExpMestMaterialId = data.EXP_MEST_MEDI_MATE_ID;
                        update.UsedTime = isChecked ? data.USED_TIME : null;
                        if (isChecked)
                        {
                            lstexpmestmaterial = new BackendAdapter(param)
                                .Post<HIS_EXP_MEST_MATERIAL>("api/HisExpMestMaterial/Used", ApiConsumers.MosConsumer, update, param);
                        }
                        else
                        {
                            lstexpmestmaterial = new BackendAdapter(param)
                                .Post<HIS_EXP_MEST_MATERIAL>("api/HisExpMestMaterial/Unused", ApiConsumers.MosConsumer, update, param);
                        }
                        if (lstexpmestmaterial != null)
                        {
                            success = true;
                            data.IS_USED = lstexpmestmaterial.IS_USED == 1;
                            data.USED_TIME = lstexpmestmaterial.USED_TIME;
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        private void treeMedicineIsUsePt_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void treeMedicineIsUsePt_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data != null && data is ExpMestMediMateADO)
                {
                    var rowData = (ExpMestMediMateADO)data;
                    if (e.Node.HasChildren)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.BackColor = Color.Yellow;
                        e.Appearance.BackColor2 = Color.Yellow;

                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                        if (e.Column.FieldName == "USED_TIME_STR")
                        {
                            if (IsUsedTimeLessThanInstructionTime(rowData))
                            {
                                e.Appearance.BackColor = Color.Maroon;
                                e.Appearance.BackColor2 = Color.Maroon;
                                e.Appearance.ForeColor = Color.White;
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

        private void treeMedicineIsUsePt_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.Name == treeListColumn_IsUsed.Name)
                {
                    //Rectangle checkRect = new Rectangle(e.Bounds.Left + (e.Bounds.Width - 12) / 2, e.Bounds.Top + 2, 12, 12);
                    Rectangle checkRect = GetRectangleUsed(e.Bounds);
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = (DevExpress.XtraTreeList.ViewInfo.ColumnInfo)e.ObjectArgs;
                    info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Cache, repositoryItemCheckAll, checkRect, IsAllSelectedReRun(sender as TreeList));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        protected void DrawCheckBox(GraphicsCache cache, RepositoryItemCheckEdit edit, Rectangle r, bool Checked, string text = null, Font font = null)
        {
            // Xác định kích thước và vị trí của checkbox
            const int checkBoxSize = 16; // Kích thước tiêu chuẩn cho ô checkbox
            Rectangle checkBoxRect = new Rectangle(r.X, r.Y + (r.Height - checkBoxSize) / 2, checkBoxSize, checkBoxSize);

            // Vẽ checkbox
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;

            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = checkBoxRect;
            info.CalcViewInfo();
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, checkBoxRect);
            painter.Draw(args);

            // Vẽ văn bản, nếu được cung cấp
            if (!string.IsNullOrEmpty(text))
            {
                Font drawFont = font ?? new Font("Arial", 10); // Font mặc định nếu không được cung cấp
                Brush textBrush = Brushes.Black; // Màu chữ

                // Xác định vị trí vẽ văn bản, nằm ngay bên phải của checkbox
                Rectangle textRect = new Rectangle(
                    checkBoxRect.Right + 5, // Khoảng cách giữa checkbox và chữ
                    r.Y,
                    r.Width - checkBoxRect.Width - 5,
                    r.Height
                );

                // Vẽ văn bản sử dụng Graphics từ cache
                cache.Graphics.DrawString(text, drawFont, textBrush, textRect);
            }
        }



        private bool IsAllSelectedReRun(TreeList tree)
        {
            List<ExpMestMediMateADO> data = null;
            if (tree.DataSource != null && tree.DataSource is BindingList<ExpMestMediMateADO> bindingList)
            {
                data = bindingList.ToList();
            }

            return data != null && data.Count(o => !o.IS_PARENT) == data.Count(o => !o.IS_PARENT && o.IS_USED == true);
        }


        private bool IsInvalidNodeException(TreeList tree)
        {
            List<ExpMestMediMateADO> data = null;
            if (tree.DataSource != null && tree.DataSource is BindingList<ExpMestMediMateADO> bindingList)
            {
                data = bindingList.ToList();
            }

            return data != null && data.Any(o => !o.IS_PARENT && o.IS_IN_VALID_NODE_EXCEPTION);
        }

        private bool IsUsedTimeLessThanInstructionTime(ExpMestMediMateADO rowData)
        {
            return rowData != null && rowData.USED_TIME.HasValue && rowData.USED_TIME.Value < rowData.INTRUCTION_TIME;
        }
        private Rectangle GetRectangleUsed(Rectangle boundsColumnUsed)
        {
            int checkBoxWidth = 12;
            int checkBoxHeight = 12;
            int checkBoxRightMargin = 30;
            Rectangle checkRect = new Rectangle(
                boundsColumnUsed.Right - checkBoxWidth - checkBoxRightMargin,
                boundsColumnUsed.Top + (boundsColumnUsed.Height - checkBoxHeight) / 2,
                checkBoxWidth,
                checkBoxHeight
            );
            return checkRect;
        }
        bool IsMouseDown = false;
        private void treeMedicineIsUsePt_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                Point pt = new Point(e.X, e.Y);
                TreeListHitInfo hit = tree.CalcHitInfo(pt);
                if (hit.Column != null && hit.Column.Name == treeListColumn_IsUsed.Name)
                {
                    if (IsInvalidNodeException(tree))
                    {
                        return;
                    }
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    //Rectangle checkRect = new Rectangle(info.Bounds.Left + (info.Bounds.Width - 12) / 2, info.Bounds.Top + 2, 12, 12);
                    Rectangle checkRect = GetRectangleUsed(info.Bounds);
                    if (checkRect.Contains(pt))
                    {
                        if (IsMouseDown)
                        {
                            return;
                        }
                        IsMouseDown = true;
                        hit.Column.OptionsColumn.AllowSort = false;
                        bool isUsed = false;
                        if (!IsAllSelectedReRun(tree))
                        {
                            isUsed = true;
                        }
                        string text = isUsed ? "đã dùng" : "chưa dùng";
                        if (MessageBox.Show("Bạn có muốn chuyển tất cả sang " + text + " không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }
                        List<ExpMestMediMateADO> listResultCheck = null;
                        if (tree != null && tree.DataSource != null && tree.DataSource is BindingList<ExpMestMediMateADO> bindingList)
                        {
                            if (isUsed)//Lấy các thuốc, vt của bệnh nhân chưa dùng để chuyển sang đã dùng
                                listResultCheck = bindingList.Where(o => !o.IS_PARENT && (o.IS_USED == null || o.IS_USED == false)).ToList();
                            else
                                listResultCheck = bindingList.Where(o => !o.IS_PARENT && o.IS_USED == true).ToList();
                        }
                        else
                            return;

                        if (listResultCheck != null && listResultCheck.Count > 0)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestUsedSDO sdo = new HisExpMestUsedSDO();
                            if (isUsed)
                            {
                                sdo.UsedTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                            }
                            sdo.ExpMedicineIds = new List<long>();
                            sdo.ExpMaterialIds = new List<long>();
                            List<string> mess = new List<string>();
                            foreach (var item in listResultCheck)
                            {
                                if (item.IS_MEDICINE)
                                {
                                    sdo.ExpMedicineIds.Add(item.EXP_MEST_MEDI_MATE_ID);
                                }
                                else if (item.IS_MATERIAL)
                                {
                                    sdo.ExpMaterialIds.Add(item.EXP_MEST_MEDI_MATE_ID);
                                }
                                if(item.INTRUCTION_TIME > sdo.UsedTime)
                                {
                                    mess.Add("Thời gian dùng của " + item.MEDIMATE_TYPE_NAME+" không được nhỏ hơn thời gian y lệnh ("
                            +
                            Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME)
                            + ")");
                                }    
                            }
                            if (mess != null && mess.Count > 0)
                            {
                                WaitingManager.Hide();
                                XtraMessageBox.Show(string.Join("\r\n",mess.Distinct().ToList()),"Cảnh báo");
                                return;
                            }
                            Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("HisExpMestUsedSDO__", sdo));
                            if (isUsed)
                            {
                                var rs = new BackendAdapter(param).Post<HisExpMestUsedResultSDO>("api/HisExpMest/UsedList", ApiConsumers.MosConsumer, sdo, param);
                                if (rs != null)
                                {
                                    success = true;
                                }
                            }
                            else
                            {
                                var rs = new BackendAdapter(param).Post<HisExpMestUsedResultSDO>("api/HisExpMest/UnUsedList", ApiConsumers.MosConsumer, sdo, param);
                                if (rs != null)
                                {
                                    success = true;
                                }
                            }
                            if (success)
                            {
                                LoadDataToTreeByTreatment(currentTreatment);
                                treeMedicineIsUsePt.RefreshDataSource();
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                    }
                    else
                    {
                        hit.Column.OptionsColumn.AllowSort = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                IsMouseDown = false;
            }
        }
        private void repositoryItemCheckEditMorning_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (repositoryItemCheckEditMorning.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5)
            {
                e.Cancel = true;
            }
            else
            {
                repositoryItemEditValueChanging(sender, e, phaseIsUnUsed: 1);
            }
        }

        private void repositoryItemCheckEditLunch_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (repositoryItemCheckEditLunch.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5)
            {
                e.Cancel = true;
            }
            else
            {
                repositoryItemEditValueChanging(sender, e, phaseIsUnUsed: 2);
            }
        }

        private void repositoryItemCheckEditAfternoon_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (repositoryItemCheckEditAfternoon.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5)
            {
                e.Cancel = true;
            }
            else
            {
                repositoryItemEditValueChanging(sender, e, phaseIsUnUsed: 3);
            }
        }

        private void repositoryItemCheckEditDinner_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (repositoryItemCheckEditDinner.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5)
            {
                e.Cancel = true;
            }
            else
            {
                repositoryItemEditValueChanging(sender, e, phaseIsUnUsed: 4);
            }
        }
        private void repositoryItemCheckEditMorning_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (repositoryItemCheckEditMorning.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5) return;

            //    TreeListNode focusedNode = treeMedicineIsUsePt.FocusedNode;
            //    var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;
            //    bool isChecked = checkEdit != null && checkEdit.Checked;
            //    if (focusedNode != null)
            //    {
            //        var data = treeMedicineIsUsePt.GetDataRecordByNode(focusedNode);
            //        if (data != null)
            //        {
            //            var rowData = (ExpMestMediMateADO)data;
            //            ProcessCheck(1, rowData.ID, isChecked, focusedNode);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemCheckEditLunch_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    TreeListNode focusedNode = treeMedicineIsUsePt.FocusedNode;
            //    if (repositoryItemCheckEditLunch.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5) return;
            //    var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;
            //    bool isChecked = checkEdit != null && checkEdit.Checked;
            //    if (focusedNode != null)
            //    {
            //        var data = treeMedicineIsUsePt.GetDataRecordByNode(focusedNode);
            //        if (data != null)
            //        {
            //            var rowData = (ExpMestMediMateADO)data;

            //            ProcessCheck(2, rowData.ID, isChecked, focusedNode);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemCheckEditAfternoon_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    TreeListNode focusedNode = treeMedicineIsUsePt.FocusedNode;
            //    var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;
            //    if (repositoryItemCheckEditAfternoon.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5) return;
            //    bool isChecked = checkEdit != null && checkEdit.Checked;
            //    if (focusedNode != null)
            //    {
            //        var data = treeMedicineIsUsePt.GetDataRecordByNode(focusedNode);
            //        if (data != null)
            //        {
            //            var rowData = (ExpMestMediMateADO)data;
            //            ProcessCheck(3, rowData.ID, isChecked, focusedNode);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemCheckEditDinner_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    TreeListNode focusedNode = treeMedicineIsUsePt.FocusedNode;
            //    // Lấy trạng thái của checkbox
            //    var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;
            //    if (repositoryItemCheckEditDinner.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style5) return;
            //    bool isChecked = checkEdit != null && checkEdit.Checked;

            //    if (focusedNode != null)
            //    {
            //        var data = treeMedicineIsUsePt.GetDataRecordByNode(focusedNode);
            //        if (data != null)
            //        {
            //            var rowData = (ExpMestMediMateADO)data;
            //            ProcessCheck(4, rowData.ID, isChecked, focusedNode);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private object GetData(string fieldName, TreeListNode focusedNode)
        {
            object res = null;
            try
            {
                res = focusedNode.GetValue(treeMedicineIsUsePt.Columns[fieldName]);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return res;
        }
        private void ProcessCheck(int PhaseIsUnUsed, long ExpMestMedicineId, bool check, TreeListNode focusedNode)
        {
            //try
            //{
            //    var dataSelect = (ExpMestMediMateADO)treeMedicineIsUsePt.GetDataRecordByNode(focusedNode);
            //    CommonParam param = new CommonParam();
            //    bool success = false;
            //    if (check)
            //    {
            //        MOS.SDO.HisExpMestMedicineIsUsedSDO update = new MOS.SDO.HisExpMestMedicineIsUsedSDO();
            //        update.ExpMestMedicineId = ExpMestMedicineId; //ID chi tiết phiếu xuất V_HIS_EXP_MEST_MEDICINE
            //        update.PhaseIsUsed = PhaseIsUnUsed; // 1 - Sáng 2-Trưa 3-Chiều 4-Tối
            //        string api = string.Format("api/HisExpMestMedicine/{0}", (check ? "Used" : "Unused"));
            //        Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData("HisExpMestMedicineIsUsedSDO", update));
            //        var rs = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>(api, ApiConsumers.MosConsumer, update, param);
            //        if (rs != null)
            //        {
            //            Inventec.Common.Logging.LogSystem.Debug("Du lieu api tra ve: " + Inventec.Common.Logging.LogUtil.TraceData("HIS_EXP_MEST_MEDICINE", rs));
            //            success = true;
            //            if (rs.IS_USED == 1)
            //            {
            //                dataSelect.IS_USED = true;

            //            }
            //            V_HIS_EXP_MEST_MEDICINE item = new V_HIS_EXP_MEST_MEDICINE();
            //            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(item, rs);
            //            SetstateCheck(ref dataSelect, item);
            //            treeMedicineIsUsePt.RefreshNode(treeMedicineIsUsePt.FocusedNode);
            //        }
            //    }
            //    else
            //    {
            //        MOS.SDO.HisExpMestMedicineUnUsedSDO update = new MOS.SDO.HisExpMestMedicineUnUsedSDO();
            //        update.ExpMestMedicineId = ExpMestMedicineId; //ID chi tiết phiếu xuất V_HIS_EXP_MEST_MEDICINE
            //        update.PhaseIsUnUsed = PhaseIsUnUsed; // 1 - Sáng 2-Trưa 3-Chiều 4-Tối
            //        string api = string.Format("api/HisExpMestMedicine/{0}", (check ? "Used" : "Unused"));
            //        Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData("HisExpMestMedicineIsUsedSDO", update));
            //        var rs = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>(api, ApiConsumers.MosConsumer, update, param);
            //        if (rs != null)
            //        {
            //            Inventec.Common.Logging.LogSystem.Debug("Du lieu api tra ve: " + Inventec.Common.Logging.LogUtil.TraceData("HIS_EXP_MEST_MEDICINE", rs));
            //            success = true;
            //            if (rs.IS_USED == 1)
            //            {
            //                dataSelect.IS_USED = true;

            //            }
            //            V_HIS_EXP_MEST_MEDICINE item = new V_HIS_EXP_MEST_MEDICINE();
            //            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(item, rs);
            //            SetstateCheck(ref dataSelect, item);
            //            treeMedicineIsUsePt.RefreshNode(treeMedicineIsUsePt.FocusedNode);
            //        }
            //    }

            //    MessageManager.Show(this, param, success);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private void treeMedicineIsUsePt_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "USED_TIME_STR")
                {
                    var node = e.Node;
                    var rowData = treeMedicineIsUsePt.GetDataRecordByNode(node) as ExpMestMediMateADO;
                    if (rowData != null)
                    {
                        if (IsUsedTimeLessThanInstructionTime(rowData))
                        {
                            e.DefaultDraw();
                            var icon = SystemIcons.Warning.ToBitmap();
                            int iconSize = 16;
                            int paddingRight = 4;
                            int x = e.Bounds.Right - iconSize - paddingRight;
                            int y = e.Bounds.Top + (e.Bounds.Height - iconSize) / 2;
                            e.Graphics.DrawImage(icon, x, y, iconSize, iconSize);
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMedicineIsUsePt_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data is ExpMestMediMateADO rowData)
                {
                    if (e.Column.FieldName == "USED_TIME_STR")
                    {
                        if (e.IsGetData)
                        {
                            if (rowData.USED_TIME.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rowData.USED_TIME.Value);
                            }
                            else
                            {
                                e.Value = (DateTime?)null;
                            }
                        }
                        else
                        {
                            if (rowData.USED_TIME.HasValue)
                            {
                                rowData.USED_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime)e.Value);
                            }
                            else
                            {
                                rowData.USED_TIME = (long?)null;
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

        private void treeMedicineIsUsePt_ValidateNode(object sender, DevExpress.XtraTreeList.ValidateNodeEventArgs e)
        {
            try
            {
                var node = e.Node;
                var usedTime = node.GetValue(treeListColumn10);
                var treeList = sender as TreeList;
                var rowData = treeList.GetDataRecordByNode(node) as ExpMestMediMateADO;
                if (rowData != null && usedTime != null)
                {
                    if (IsUsedTimeLessThanInstructionTime(rowData))
                    {
                        e.Valid = false;
                        e.ErrorText = "Thời gian dùng không được nhỏ hơn thời gian y lệnh ("
                            +
                            Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowData.INTRUCTION_TIME)
                            + ")";
                    }
                    treeList.RefreshNode(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void treeMedicineIsUsePt_InvalidNodeException(object sender, DevExpress.XtraTreeList.InvalidNodeExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
                XtraMessageBox.Show(e.Exception.Message);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryUsedTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var usedTimeEdit = sender as DevExpress.XtraEditors.DateEdit;
                var treeList = usedTimeEdit.Parent as TreeList;
                if (usedTimeEdit != null)
                {
                    var rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ExpMestMediMateADO;
                    if (rowData != null)
                    {
                        bool isUsed = rowData.IS_USED.HasValue && rowData.IS_USED.Value;
                        rowData.USED_TIME = isUsed ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(usedTimeEdit.DateTime) : null;
                        if (IsUsedTimeLessThanInstructionTime(rowData))
                        {
                            rowData.IS_IN_VALID_NODE_EXCEPTION = true;
                        }
                        else
                        {
                            rowData.IS_IN_VALID_NODE_EXCEPTION = false;
                            if (isUsed)
                            {
                                UpdateHisExpMest(ref rowData, isUsed);
                            }
                        }
                        treeList.RefreshNode(treeList.FocusedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var treeList = treeMedicineIsUsePt;
                var rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ExpMestMediMateADO;

                if (rowData != null && !rowData.IS_PARENT)
                {
                    // TODO: Khởi tạo danh sách chi tiết thời gian dùng thuốc theo dữ liệu thực tế
                    List<TimeUsed> detailList = new List<TimeUsed>();

                    // Ví dụ: nếu sau này có dữ liệu cụ thể, thêm vào detailList ở đây
                    // detailList.Add(new TimeUsed { ... });
                    if ((rowData.EXP_MEDIMATE_USEDs == null || rowData.EXP_MEDIMATE_USEDs.Count == 0) && !rowData.IsCallExpMestMediMateUsed)
                    {
                        HisExpMedimateUsedFilter filter = new HisExpMedimateUsedFilter();
                        if (rowData.IS_MEDICINE)
                            filter.EXP_MEST_MEDICINE_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        else if (rowData.IS_MATERIAL)
                            filter.EXP_MEST_MATERIAL_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        filter.TDL_TREATMENT_ID = currentTreatment.ID;
                        CommonParam param = new CommonParam();
                        var usedList = new BackendAdapter(param).Get<List<HIS_EXP_MEDIMATE_USED>>("api/HisExpMedimateUsed/Get", ApiConsumers.MosConsumer, filter, param);
                        if (usedList != null && usedList.Count > 0)
                        {
                            rowData.EXP_MEDIMATE_USEDs = usedList;
                        }
                    }

                    if (rowData.EXP_MEDIMATE_USEDs != null && rowData.EXP_MEDIMATE_USEDs.Count > 0)
                    {
                        foreach (var item in rowData.EXP_MEDIMATE_USEDs)
                        {
                            TimeUsed ado = new TimeUsed();
                            if (item.MEDICATION_SESSION.HasValue)
                            {
                                ado.ID = item.ID;
                                switch (item.MEDICATION_SESSION)
                                {
                                    case 1:
                                        ado.TG_MORNING = item.USE_TIME;
                                        ado.SL_MORNING = item.AMOUNT;
                                        break;
                                    case 2:
                                        ado.TG_NOON = item.USE_TIME;
                                        ado.SL_NOON = item.AMOUNT;
                                        break;
                                    case 3:
                                        ado.TG_AFTERNOON = item.USE_TIME;
                                        ado.SL_AFTERNOON = item.AMOUNT;
                                        break;
                                    case 4:
                                        ado.TG_EVENING = item.USE_TIME;
                                        ado.SL_EVENING = item.AMOUNT;
                                        break;
                                    default:
                                        break;
                                }
                                ado.IS_DELETE = true;
                                detailList.Add(ado);
                            }
                        }
                    }
                    if (detailList == null || detailList.Count == 0)
                    {
                        TimeUsed ado = new TimeUsed();
                        ado.IS_ADD = true;
                        ado.IS_DELETE = false;
                        detailList.Add(ado);
                    }
                    else
                    {
                        detailList.LastOrDefault().IS_ADD = true;
                        if (detailList.Count == 1)
                        {
                            detailList.LastOrDefault().IS_DELETE = false;
                        }
                        else
                        {
                            detailList.ForEach(o => o.IS_DELETE = true);
                        }
                    }

                    gridControl1.DataSource = detailList;

                    // Lấy hit-info tại vị trí click trên TreeList
                    // X, Y là tọa độ click tương đối trong TreeList
                    Point clickPoint = treeList.PointToClient(Control.MousePosition);
                    TreeListHitInfo hitInfo = treeList.CalcHitInfo(clickPoint);

                    Rectangle nodeBounds;
                    if (hitInfo != null && hitInfo.Node != null)
                    {
                        // Lấy vùng hiển thị của node hiện tại
                        nodeBounds = hitInfo.Bounds;
                    }
                    else
                    {
                        // Fallback nếu không xác định được node
                        nodeBounds = new Rectangle(0, 50, treeList.Width, treeList.RowHeight);
                    }

                    // Tính điểm hiển thị popup ngay dưới vị trí click
                    int popupX = clickPoint.X;
                    int popupY = nodeBounds.Bottom;

                    // Chuyển sang tọa độ màn hình
                    Point screenPoint = treeList.PointToScreen(new Point(popupX, popupY));
                    popupControlContainer1.ShowPopup(screenPoint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var row = gridView1.GetRow(e.RowHandle) as TimeUsed;
                    if (row != null)
                    {
                        bool isAdd = row.IS_ADD;
                        bool isDelete = row.IS_DELETE;

                        if (e.Column.FieldName == "Add")
                        {
                            if (isAdd)
                            {
                                e.RepositoryItem = repAdd;
                            }
                            else
                            {
                                e.RepositoryItem = null;
                            }
                        }
                        else if (e.Column.FieldName == "Delete")
                        {
                            if (isDelete)
                            {
                                e.RepositoryItem = repDelete;
                            }
                            else
                            {
                                e.RepositoryItem = null;
                            }
                        }
                        else if (e.Column.FieldName == "TG_MORNING_STR")
                        {
                            if (row.TG_MORNING.HasValue || row.SL_MORNING.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repDateEna;
                            }
                            else
                            {
                                e.RepositoryItem = repDateDis;
                            }
                        }
                        else if (e.Column.FieldName == "SL_MORNING")
                        {
                            if (row.TG_MORNING.HasValue || row.SL_MORNING.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repAmountEna;
                            }
                            else
                            {
                                e.RepositoryItem = repAmountDis;
                            }
                        }
                        else if (e.Column.FieldName == "TG_NOON_STR")
                        {
                            if (row.TG_NOON.HasValue || row.SL_NOON.HasValue || (!row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repDateEna;
                            }
                            else
                            {
                                e.RepositoryItem = repDateDis;
                            }
                        }
                        else if (e.Column.FieldName == "SL_NOON")
                        {
                            if (row.TG_NOON.HasValue || row.SL_NOON.HasValue || (!row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repAmountEna;
                            }
                            else
                            {
                                e.RepositoryItem = repAmountDis;
                            }
                        }
                        else if (e.Column.FieldName == "TG_AFTERNOON_STR")
                        {
                            if (row.TG_AFTERNOON.HasValue || row.SL_AFTERNOON.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue))
                            {
                                e.RepositoryItem = repDateEna;
                            }
                            else
                            {
                                e.RepositoryItem = repDateDis;
                            }
                        }
                        else if (e.Column.FieldName == "SL_AFTERNOON")
                        {
                            if (row.TG_AFTERNOON.HasValue || row.SL_AFTERNOON.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_EVENING.HasValue && !row.SL_EVENING.HasValue && !row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue))
                            {
                                e.RepositoryItem = repAmountEna;
                            }
                            else
                            {
                                e.RepositoryItem = repAmountDis;
                            }
                        }
                        else if (e.Column.FieldName == "TG_EVENING_STR")
                        {
                            if (row.TG_EVENING.HasValue || row.SL_EVENING.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repDateEna;
                            }
                            else
                            {
                                e.RepositoryItem = repDateDis;
                            }
                        }
                        else if (e.Column.FieldName == "SL_EVENING")
                        {
                            if (row.TG_EVENING.HasValue || row.SL_EVENING.HasValue || (!row.TG_NOON.HasValue && !row.SL_NOON.HasValue && !row.TG_MORNING.HasValue && !row.SL_MORNING.HasValue && !row.TG_AFTERNOON.HasValue && !row.SL_AFTERNOON.HasValue))
                            {
                                e.RepositoryItem = repAmountEna;
                            }
                            else
                            {
                                e.RepositoryItem = repAmountDis;
                            }
                        }
                    }
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
                    TimeUsed dataRow = (TimeUsed)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "TG_MORNING_STR")
                    {
                        e.Value = dataRow.TG_MORNING.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.TG_MORNING.Value) : (DateTime?)null;
                    }
                    else if (e.Column.FieldName == "TG_NOON_STR")
                    {
                        e.Value = dataRow.TG_NOON.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.TG_NOON.Value) : (DateTime?)null;
                    }
                    else if (e.Column.FieldName == "TG_AFTERNOON_STR")
                    {
                        e.Value = dataRow.TG_AFTERNOON.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.TG_AFTERNOON.Value) : (DateTime?)null;
                    }
                    else if (e.Column.FieldName == "TG_EVENING_STR")
                    {
                        e.Value = dataRow.TG_EVENING.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.TG_EVENING.Value) : (DateTime?)null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repDateEna_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                TimeUsed data = (TimeUsed)gridView1.GetFocusedRow();
                if (data != null)
                {
                    DateEdit dtTime = sender as DateEdit;
                    if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                    {
                        if (gridView1.FocusedColumn.FieldName == "TG_MORNING_STR")
                            data.TG_MORNING = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);
                        else if (gridView1.FocusedColumn.FieldName == "TG_NOON_STR")
                            data.TG_NOON = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);
                        else if (gridView1.FocusedColumn.FieldName == "TG_AFTERNOON_STR")
                            data.TG_AFTERNOON = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);
                        else if (gridView1.FocusedColumn.FieldName == "TG_EVENING_STR")
                            data.TG_EVENING = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);

                    }
                    else
                    {
                        if (gridView1.FocusedColumn.FieldName == "TG_MORNING_STR")
                            data.TG_MORNING = null;
                        else if (gridView1.FocusedColumn.FieldName == "TG_NOON_STR")
                            data.TG_NOON = null;
                        else if (gridView1.FocusedColumn.FieldName == "TG_AFTERNOON_STR")
                            data.TG_AFTERNOON = null;
                        else if (gridView1.FocusedColumn.FieldName == "TG_EVENING_STR")
                            data.TG_EVENING = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    var list = gridControl1.DataSource as IList<TimeUsed>;
                    if (list == null)
                    {
                        return;
                    }

                    var current = gridView1.GetFocusedRow() as TimeUsed;
                    TimeUsed newRow = new TimeUsed
                    {
                        IS_ADD = true,
                        IS_DELETE = list.Count > 0
                    };

                    // Hàng hiện tại không còn nút thêm
                    if (current != null)
                    {
                        current.IS_ADD = false;
                        current.IS_DELETE = true;
                    }

                    list.Add(newRow);
                    gridControl1.RefreshDataSource();

                    int newHandle = gridView1.GetRowHandle(list.Count - 1);
                    if (newHandle >= 0)
                    {
                        gridView1.FocusedRowHandle = newHandle;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {

                var list = gridControl1.DataSource as IList<TimeUsed>;
                if (list == null || list.Count == 0)
                {
                    return;
                }

                var current = gridView1.GetFocusedRow() as TimeUsed;
                if (current == null)
                {
                    return;
                }

                // Nếu bản ghi đã tồn tại trên hệ thống (ID > 0) thì gọi API xóa trước
                if (current.ID > 0)
                {
                    CommonParam param = new CommonParam();
                    var deleteResult = new BackendAdapter(param)
                        .Post<HIS_EXP_MEDIMATE_USED>("api/HisExpMedimateUsed/Delete", ApiConsumers.MosConsumer, current.ID, param);

                    if (deleteResult == null)
                    {
                        MessageManager.Show(this, param, false);
                        return;
                    }
                }

                int currentIndex = list.IndexOf(current);
                if (currentIndex < 0)
                {
                    return;
                }

                list.RemoveAt(currentIndex);

                if (list.Count == 1)
                {
                    list[0].IS_DELETE = false;
                    list[0].IS_ADD = true;
                }else
                    list.LastOrDefault().IS_ADD = true;

                gridControl1.RefreshDataSource();

                if (list.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, list.Count - 1);
                    int newHandle = gridView1.GetRowHandle(newIndex);
                    if (newHandle >= 0)
                    {
                        gridView1.FocusedRowHandle = newHandle;
                        gridView1.FocusedColumn = gridView1.Columns["Add"];
                        // Đảm bảo focus quay lại cột Delete sau khi di chuyển dòng
                        if (gridView1.Columns["Delete"] != null)
                        {
                            gridView1.FocusedColumn = gridView1.Columns["Delete"];
                            gridView1.ShowEditor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy danh sách chi tiết thời gian dùng thuốc từ grid
                var detailList = gridControl1.DataSource as IList<TimeUsed>;
                if (detailList == null || detailList.Count == 0)
                {
                    return;
                }

                // Lấy node đang được focus trên tree và dữ liệu tương ứng
                var treeList = treeMedicineIsUsePt;
                if (treeList == null || treeList.FocusedNode == null)
                {
                    return;
                }

                var rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ExpMestMediMateADO;
                if (rowData == null || rowData.IS_PARENT)
                {
                    return;
                }

                // Chuyển danh sách TimeUsed hiển thị sang danh sách HIS_EXP_MEDIMATE_USED
                var usedEntities = new List<HIS_EXP_MEDIMATE_USED>();
                foreach (var item in detailList)
                {
                    // Bỏ qua các dòng không nhập dữ liệu
                    if (!item.TG_MORNING.HasValue && !item.SL_MORNING.HasValue &&
                        !item.TG_NOON.HasValue && !item.SL_NOON.HasValue &&
                        !item.TG_AFTERNOON.HasValue && !item.SL_AFTERNOON.HasValue &&
                        !item.TG_EVENING.HasValue && !item.SL_EVENING.HasValue)
                    {
                        continue;
                    }

                    // Sáng
                    if (item.TG_MORNING.HasValue || item.SL_MORNING.HasValue)
                    {
                        var used = new HIS_EXP_MEDIMATE_USED
                        {
                            ID = item.ID,
                            TDL_TREATMENT_ID = currentTreatment != null ? currentTreatment.ID : 0,
                            USE_TIME = item.TG_MORNING,
                            AMOUNT = item.SL_MORNING,
                            MEDICATION_SESSION = 1
                        };
                        if (rowData.IS_MEDICINE)
                        {
                            used.EXP_MEST_MEDICINE_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }
                        else if (rowData.IS_MATERIAL)
                        {
                            used.EXP_MEST_MATERIAL_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }

                        usedEntities.Add(used);
                    }

                    // Trưa
                    if (item.TG_NOON.HasValue || item.SL_NOON.HasValue)
                    {
                        var used = new HIS_EXP_MEDIMATE_USED
                        {
                            ID = item.ID,
                            TDL_TREATMENT_ID = currentTreatment != null ? currentTreatment.ID : 0,
                            USE_TIME = item.TG_NOON,
                            AMOUNT = item.SL_NOON,
                            MEDICATION_SESSION = 2
                        };
                        if (rowData.IS_MEDICINE)
                        {
                            used.EXP_MEST_MEDICINE_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }
                        else if (rowData.IS_MATERIAL)
                        {
                            used.EXP_MEST_MATERIAL_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }

                        usedEntities.Add(used);
                    }

                    // Chiều
                    if (item.TG_AFTERNOON.HasValue || item.SL_AFTERNOON.HasValue)
                    {
                        var used = new HIS_EXP_MEDIMATE_USED
                        {
                            ID = item.ID,
                            TDL_TREATMENT_ID = currentTreatment != null ? currentTreatment.ID : 0,
                            USE_TIME = item.TG_AFTERNOON,
                            AMOUNT = item.SL_AFTERNOON,
                            MEDICATION_SESSION = 3
                        };
                        if (rowData.IS_MEDICINE)
                        {
                            used.EXP_MEST_MEDICINE_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }
                        else if (rowData.IS_MATERIAL)
                        {
                            used.EXP_MEST_MATERIAL_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }

                        usedEntities.Add(used);
                    }

                    // Tối
                    if (item.TG_EVENING.HasValue || item.SL_EVENING.HasValue)
                    {
                        var used = new HIS_EXP_MEDIMATE_USED
                        {
                            ID = item.ID,
                            TDL_TREATMENT_ID = currentTreatment != null ? currentTreatment.ID : 0,
                            USE_TIME = item.TG_EVENING,
                            AMOUNT = item.SL_EVENING,
                            MEDICATION_SESSION = 4
                        };
                        if (rowData.IS_MEDICINE)
                        {
                            used.EXP_MEST_MEDICINE_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }
                        else if (rowData.IS_MATERIAL)
                        {
                            used.EXP_MEST_MATERIAL_ID = rowData.EXP_MEST_MEDI_MATE_ID;
                        }

                        usedEntities.Add(used);
                    }
                }


                var notify = usedEntities.Where(o => o.USE_TIME < rowData.INTRUCTION_TIME).ToList();
                if (notify != null && notify.Count > 0)
                {
                   
                    XtraMessageBox.Show("Có thời gian dùng thuốc nhỏ hơn thời gian y lệnh (" + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowData.INTRUCTION_TIME) + "). Vui lòng kiểm tra lại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var checkUsed = usedEntities.Where(o => o.USE_TIME == null || o.AMOUNT == null).ToList();
                if (checkUsed != null && checkUsed.Count > 0)
                {
                    XtraMessageBox.Show("Thời gian hoặc số lượng chưa được nhập. Vui lòng kiểm tra lại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }    

                // Gắn lại danh sách vào node đang focus
                rowData.EXP_MEDIMATE_USEDs = usedEntities;

                // Ẩn popup sau khi lưu tạm vào node
                if (popupControlContainer1 != null && popupControlContainer1.Visible)
                {
                    popupControlContainer1.HidePopup();
                }

                treeList.RefreshNode(treeList.FocusedNode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        /// <summary>
        /// Chuyển chuỗi số lượng (các định dạng: 02; 02.1; 02,1; 2,1; 2.1) sang decimal.
        /// Trả về null nếu chuỗi rỗng hoặc không hợp lệ.
        /// </summary>
        private decimal ParseDecimalFromString(string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0;
                }

                string normalized = value.Trim();

                // Thay thế dấu phẩy bằng dấu chấm để thống nhất
                normalized = normalized.Replace(',', '.');

                decimal result;
                if (decimal.TryParse(
                    normalized,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out result))
                {
                    return result;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave.PerformClick();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateUsedAmounts(treeMedicineIsUsePt.DataSource as BindingList<ExpMestMediMateADO>))
                    return;

                var data = treeMedicineIsUsePt.DataSource as BindingList<ExpMestMediMateADO>;
                if (data == null || data.Count == 0)
                {
                    return;
                }

                List<HisExpMediMateUsedSDO> saveList = new List<HisExpMediMateUsedSDO>();
                foreach (var item in data.Where(o => !o.IS_PARENT))
                {
                    if (item.EXP_MEDIMATE_USEDs == null || item.EXP_MEDIMATE_USEDs.Count == 0)
                    {
                        continue;
                    }

                    foreach (var i in item.EXP_MEDIMATE_USEDs)
                    {
                        HisExpMediMateUsedSDO sdo = new HisExpMediMateUsedSDO();
                        sdo.TreatmentId = currentTreatment.ID;
                        sdo.UseTime = i.USE_TIME;
                        sdo.Amount = i.AMOUNT;
                        sdo.MedicationSession = i.MEDICATION_SESSION;

                        if (item.IS_MEDICINE)
                        {
                            sdo.ExpMestMedicineId = item.EXP_MEST_MEDI_MATE_ID;
                        }
                        else if (item.IS_MATERIAL)
                        {
                            sdo.ExpMestMaterialId = item.EXP_MEST_MEDI_MATE_ID;
                        }

                        sdo.ExpMediMateUsedId = i.ID;
                        saveList.Add(sdo);
                    }
                }

                if (saveList.Count == 0)
                {
                    XtraMessageBox.Show("Chưa nhập nội dung thông tin kê sáng/trưa/chiều/tối của đơn thuốc", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saveList), saveList));
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param)
                    .Post<HisExpMediMateUsedResultSDO>("api/HisExpMedimateUsed/Used", ApiConsumers.MosConsumer, saveList, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result != null && result.HIS_EXP_MEDIMATE_USED != null && result.HIS_EXP_MEDIMATE_USED.Count > 0)
                {
                    // Ánh xạ lại kết quả trả về vào các EXP_MEDIMATE_USEDs của từng dòng trong tree
                    foreach (var row in data.Where(o => !o.IS_PARENT))
                    {
                        if (row.IS_MEDICINE)
                        {
                            var listByRow = result.HIS_EXP_MEDIMATE_USED
                                .Where(x => x.EXP_MEST_MEDICINE_ID == row.EXP_MEST_MEDI_MATE_ID)
                                .ToList();

                            if (listByRow != null && listByRow.Count > 0)
                            {
                                row.EXP_MEDIMATE_USEDs = listByRow;
                            }
                            else
                            {
                                // Nếu không còn bản ghi nào, giữ nguyên hoặc gán list rỗng tùy yêu cầu
                                // row.EXP_MEDIMATE_USEDs = new List<HIS_EXP_MEDIMATE_USED>();
                            }
                        }
                        else if (row.IS_MATERIAL)
                        {
                            var listByRow = result.HIS_EXP_MEDIMATE_USED
                                .Where(x => x.EXP_MEST_MATERIAL_ID == row.EXP_MEST_MEDI_MATE_ID)
                                .ToList();

                            if (listByRow != null && listByRow.Count > 0)
                            {
                                row.EXP_MEDIMATE_USEDs = listByRow;
                            }
                            else
                            {
                                // row.EXP_MEDIMATE_USEDs = new List<HIS_EXP_MEDIMATE_USED>();
                            }
                        }
                    }

                    treeMedicineIsUsePt.RefreshDataSource();
                }

                MessageManager.Show(this, param, result != null && result.HIS_EXP_MEDIMATE_USED != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidateUsedAmounts(BindingList<ExpMestMediMateADO> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return true;
                }

                Dictionary<string, List<string>> dicMess = new Dictionary<string, List<string>>();

                foreach (var rowData in data.Where(o => !o.IS_PARENT))
                {
                    if (rowData == null)
                    {
                        continue;
                    }

                    decimal prescribedMorning = ParseDecimalFromString(rowData.MORNING);
                    decimal prescribedNoon = ParseDecimalFromString(rowData.NOON);
                    decimal prescribedAfternoon = ParseDecimalFromString(rowData.AFTERNOON);
                    decimal prescribedEvening = ParseDecimalFromString(rowData.EVENING);

                    decimal totalMorning = 0;
                    decimal totalNoon = 0;
                    decimal totalAfternoon = 0;
                    decimal totalEvening = 0;

                    if (rowData.EXP_MEDIMATE_USEDs != null && rowData.EXP_MEDIMATE_USEDs.Count > 0)
                    {
                        foreach (var used in rowData.EXP_MEDIMATE_USEDs)
                        {
                            if (!used.MEDICATION_SESSION.HasValue || !used.AMOUNT.HasValue)
                            {
                                continue;
                            }

                            switch (used.MEDICATION_SESSION.Value)
                            {
                                case 1:
                                    totalMorning += used.AMOUNT.Value;
                                    break;
                                case 2:
                                    totalNoon += used.AMOUNT.Value;
                                    break;
                                case 3:
                                    totalAfternoon += used.AMOUNT.Value;
                                    break;
                                case 4:
                                    totalEvening += used.AMOUNT.Value;
                                    break;
                            }
                        }
                    }

                    List<string> mess = new List<string>();
                    if (prescribedMorning > 0 && totalMorning > prescribedMorning)
                    {
                        mess.Add(string.Format(
                            "Tổng số lượng {0} của buổi sáng không được lớn hơn số lượng {1} đã kê của thuốc {2}",
                            totalMorning,
                            prescribedMorning,
                            rowData.MEDIMATE_TYPE_NAME));
                    }

                    if (prescribedNoon > 0 && totalNoon > prescribedNoon)
                    {
                        mess.Add(string.Format(
                            "Tổng số lượng {0} của buổi trưa không được lớn hơn số lượng {1} đã kê của thuốc {2}",
                            totalNoon,
                            prescribedNoon,
                            rowData.MEDIMATE_TYPE_NAME));
                    }

                    if (prescribedAfternoon > 0 && totalAfternoon > prescribedAfternoon)
                    {
                        mess.Add(string.Format(
                            "Tổng số lượng {0} của buổi chiều không được lớn hơn số lượng {1} đã kê của thuốc {2}",
                            totalAfternoon,
                            prescribedAfternoon,
                            rowData.MEDIMATE_TYPE_NAME));
                    }

                    if (prescribedEvening > 0 && totalEvening > prescribedEvening)
                    {
                        mess.Add(string.Format(
                            "Tổng số lượng {0} của buổi tối không được lớn hơn số lượng {1} đã kê của thuốc {2}",
                            totalEvening,
                            prescribedEvening,
                            rowData.MEDIMATE_TYPE_NAME));
                    }


                    if (prescribedMorning == 0
                       && prescribedNoon == 0
                       && prescribedAfternoon == 0
                       && prescribedEvening == 0)
                    {
                        decimal totalUserAmount = totalMorning + totalNoon + totalAfternoon + totalEvening;

                        decimal totalPrescribedAmount = rowData.AMOUNT.HasValue ? rowData.AMOUNT.Value : 0;

                        if (totalPrescribedAmount > 0 && totalUserAmount > totalPrescribedAmount)
                        {
                            mess.Add(string.Format(
                                "Tổng số lượng người dùng nhập cho cả sáng, trưa, chiều, tối (số lượng {0}) không được lớn hơn Tổng số lượng bác sĩ kê đơn (số lượng {1}) cho thuốc {2}",
                                totalUserAmount,
                                totalPrescribedAmount,
                                rowData.MEDIMATE_TYPE_NAME));
                        }
                    }

                    if (mess != null && mess.Count > 0)
                    {
                        if (!dicMess.ContainsKey(rowData.MEDIMATE_TYPE_NAME))
                        {
                            dicMess.Add(rowData.MEDIMATE_TYPE_NAME, mess);
                        }
                        else
                        {
                            dicMess[rowData.MEDIMATE_TYPE_NAME].AddRange(mess);
                        }
                    }
                }

                if (dicMess.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, List<string>> kvp in dicMess)
                    {
                        foreach (string msg in kvp.Value.Where(m => !string.IsNullOrWhiteSpace(m)))
                        {
                            sb.AppendLine(msg);
                        }
                    }

                    if (sb.Length > 0)
                    {
                        XtraMessageBox.Show(sb.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
