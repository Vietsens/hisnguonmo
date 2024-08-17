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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.BloodTransfusion.ADO;
using HIS.Desktop.Plugins.BloodTransfusion.Validate;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.BloodTransfusion
{
    public partial class frmBloodTransfusion : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        string currentTreatmentCode;
        HIS.UC.Icd.IcdProcessor IcdProcessor;
        UserControl ucIcd;
        SecondaryIcdProcessor subIcdProcessor;
        UserControl ucSecondaryIcd;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        List<ACS_USER> acsUsers;
        V_HIS_TREATMENT currentTreatment;
        List<TransfusionADO> listTranfusionAdos;

        List<V_HIS_EXP_MEST_BLOOD> currentExpMestBloods;
        List<V_HIS_TRANSFUSION_SUM> currentTransfusionSums;

        V_HIS_EXP_MEST_BLOOD clickExpMestBlood;
        V_HIS_TRANSFUSION_SUM clickTransfusionSum;
        V_HIS_TRANSFUSION_SUM printTransfusionSum;
        V_HIS_ROOM currentRoom;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        public frmBloodTransfusion()
        {
            InitializeComponent();
        }

        public frmBloodTransfusion(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.Text = this.currentModule.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmBloodTransfusion(Inventec.Desktop.Common.Modules.Module module, string treatmentCode)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.currentTreatmentCode = treatmentCode;
                this.Text = this.currentModule.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmBloodTransfusion_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == currentModule.RoomId).FirstOrDefault();

                gridControlExpMestBlood.ToolTipController = this.toolTipController1;
                SetIcon();
                InitComboTransfusionPerson();
                InitIcd();
                InitUcSecondaryIcd();
                if (!string.IsNullOrEmpty(currentTreatmentCode))
                {
                    this.gridControlExpMestBlood.ToolTipController = this.toolTipController1;
                    this.currentTreatment = GetTreatmentByCode(this.currentTreatmentCode, null, null);
                    LoadDataToGridExpMestBlood();
                    FillDataTreatmentToControl();
                }


                //mở từ danh sách điều trị thì không thêm được, đã kết thúc cũng không thêm được

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_TRANSFUSION_SUM> GetTransfusionSum(long? expMestBloodId, V_HIS_TREATMENT treatment)
        {
            List<V_HIS_TRANSFUSION_SUM> rs = null;
            try
            {
                if (treatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                    filter.EXP_MEST_BLOOD_ID = expMestBloodId;
                    filter.TREATMENT_ID = treatment.ID;

                    rs = new BackendAdapter(param).Get<List<V_HIS_TRANSFUSION_SUM>>("api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void LoadDataToGridExpMestBlood()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_EXP_MEST_BLOOD>> apiResult = null;
                HisExpMestBloodViewFilter filter = new HisExpMestBloodViewFilter();
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    var treatmentCode = txtTreatmentCode.Text;
                    if (treatmentCode.Length < 12)
                        treatmentCode = string.Format("{0:000000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(treatmentCode));

                    txtTreatmentCode.Text = treatmentCode;
                    filter.TDL_TREATMENT_CODE__EXACT = treatmentCode;
                }
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    var patientCode = txtPatientCode.Text;
                    if (patientCode.Length < 10)
                        patientCode = string.Format("{0:0000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(patientCode));

                    txtPatientCode.Text = patientCode;
                    filter.TDL_PATIENT_CODE__EXACT = patientCode;
                }
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text;
                }
                filter.IS_EXPORT = true;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "EXP_TIME";

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_EXP_MEST_BLOOD>)apiResult.Data;
                    if (data != null)
                    {
                        this.currentExpMestBloods = data;
                        gridControlExpMestBlood.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid(GridControl grid, object data)
        {
            try
            {
                this.clickTransfusionSum = null;
                this.clickExpMestBlood = null;
                grid.BeginUpdate();
                grid.DataSource = data;
                grid.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_TRANSFUSION> GetTransfusionByTransfusionSumId(long? transfusionSumId)
        {
            List<HIS_TRANSFUSION> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTransfusionFilter filter = new HisTransfusionFilter();
                filter.TRANSFUSION_SUM_ID = transfusionSumId;

                rs = new BackendAdapter(param).Get<List<HIS_TRANSFUSION>>("api/HisTransfusion/Get", ApiConsumers.MosConsumer, filter, param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void LoadDataToGridTransfusion(long? transfusionSumId)
        {
            try
            {
                if (transfusionSumId != null)
                {
                    listTranfusionAdos = new List<TransfusionADO>();
                    //listTranfusionAdos.Add(new TransfusionADO(Inventec.Common.DateTime.Get.Now() ?? 0, 0));
                    var transfusion = GetTransfusionByTransfusionSumId(transfusionSumId);
                    if (transfusion != null && transfusion.Count > 0)
                    {
                        var ados = from r in transfusion select new TransfusionADO(r);
                        listTranfusionAdos.AddRange(ados);
                    }

                    gridControlTransfusion.BeginUpdate();
                    gridControlTransfusion.DataSource = listTranfusionAdos;
                    gridControlTransfusion.EndUpdate();
                }
                else
                {
                    gridControlTransfusion.BeginUpdate();
                    gridControlTransfusion.DataSource = null;
                    gridControlTransfusion.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_TREATMENT GetTreatmentByCode(string treatmentCode, string patientCode, string keyWord)
        {
            V_HIS_TREATMENT rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                if (!string.IsNullOrEmpty(treatmentCode))
                {
                    if (treatmentCode.Length < 12)
                        treatmentCode = string.Format("{0:000000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(treatmentCode));

                    txtTreatmentCode.Text = treatmentCode;
                    filter.TREATMENT_CODE__EXACT = treatmentCode;
                }

                var rsApi = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);

                if (rsApi != null && rsApi.Count > 0)
                {
                    rs = rsApi.FirstOrDefault();
                }
                // dis/en nut luu
                btnSave.Enabled = rs != null && rs.IS_PAUSE != 1 && (rs.LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID || (rs.CO_TREAT_DEPARTMENT_IDS != null && ("," + rs.CO_TREAT_DEPARTMENT_IDS + ",").Contains("," + currentRoom.DEPARTMENT_ID + ",")));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return rs;
            }
            return rs;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboTransfusionPerson()
        {
            try
            {
                acsUsers = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2, true));

                ControlEditorADO ado = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);

                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(cboTransfusionPerson, acsUsers, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitIcd()
        {
            try
            {
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");

                var listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                IcdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.IsColor = false;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                this.ucIcd = (UserControl)this.IcdProcessor.Run(ado);
                if (this.ucIcd != null)
                {
                    this.panelControlIcdMain.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
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
                var listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = "Chẩn đoán phụ:";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlIcdSub.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                txtTransfusionPersonCode.Focus();
                txtTransfusionPersonCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataTreatmentToControl()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    lblPatientCode.Text = this.currentTreatment.TDL_PATIENT_CODE;
                    lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.TDL_PATIENT_DOB);
                    lblPatientGender.Text = this.currentTreatment.TDL_PATIENT_GENDER_NAME;
                    lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME;
                    lblTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataExpMestBloodToControlForm(V_HIS_EXP_MEST_BLOOD expMestBlood, bool edit)
        {
            try
            {
                if (expMestBlood != null)
                {
                    lblAmount.Text = "1";
                    lblBloodAbo.Text = expMestBlood.BLOOD_ABO_CODE;
                    lblBloodCode.Text = expMestBlood.BLOOD_CODE;
                    lblBloodTypeCode.Text = expMestBlood.BLOOD_TYPE_CODE;
                    lblBloodTypeName.Text = expMestBlood.BLOOD_TYPE_NAME;
                    lblExprireDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMestBlood.EXPIRED_DATE ?? 0);
                    lblKqnpCoombos.Text = expMestBlood.COOMBS;
                    lblPatientAbo.Text = expMestBlood.PATIENT_BLOOD_ABO_CODE;
                    lblPersonGive.Text = expMestBlood.GIVE_NAME;
                    lblPhanUngCheo.Text = expMestBlood.PUC;
                    txtPhanUngCheo.Text = expMestBlood.PUC;
                    lblPPOngNghiem.Text = expMestBlood.TEST_TUBE;
                    lblScangelGelcard.Text = expMestBlood.SCANGEL_GELCARD;
                    lblVolume.Text = expMestBlood.VOLUME.ToString();

                    if (edit)
                    {
                        if (this.currentTreatment != null)
                        {
                            if (ucIcd != null && IcdProcessor != null)
                            {
                                IcdInputADO ado = new IcdInputADO();
                                ado.ICD_CODE = this.currentTreatment.ICD_CODE;
                                ado.ICD_NAME = this.currentTreatment.ICD_NAME;

                                IcdProcessor.Reload(ucIcd, ado);
                            }
                            if (ucSecondaryIcd != null && IcdProcessor != null)
                            {
                                SecondaryIcdDataADO ado = new SecondaryIcdDataADO();
                                ado.ICD_SUB_CODE = this.currentTreatment.ICD_SUB_CODE;
                                ado.ICD_TEXT = this.currentTreatment.ICD_TEXT;

                                subIcdProcessor.Reload(ucSecondaryIcd, ado);
                            }
                        }

                        txtTransfusionPersonCode.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        cboTransfusionPerson.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(); ;
                        dtTransfusionTimeFrom.DateTime = DateTime.Now;
                        dtTransfusionTimeTo.DateTime = DateTime.Now;
                        spCount.EditValue = null;
                        spTransfusionVolume.EditValue = expMestBlood.VOLUME;
                        txtDescription.Text = "";
                    }
                }
                else
                {
                    lblBloodAbo.Text = "";
                    lblBloodCode.Text = "";
                    lblBloodTypeCode.Text = "";
                    lblBloodTypeName.Text = "";
                    lblExprireDate.Text = "";
                    lblKqnpCoombos.Text = "";
                    lblPatientAbo.Text = "";
                    lblPersonGive.Text = "";
                    lblPhanUngCheo.Text = "";
                    txtPhanUngCheo.Text = "";
                    lblPPOngNghiem.Text = "";
                    lblScangelGelcard.Text = "";
                    lblVolume.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataTransfusionSumToControlForm(V_HIS_TRANSFUSION_SUM transfusionSum)
        {
            try
            {
                if (transfusionSum != null)
                {
                    if (ucIcd != null && IcdProcessor != null)
                    {
                        IcdInputADO ado = new IcdInputADO();
                        ado.ICD_CODE = transfusionSum.ICD_CODE;
                        ado.ICD_NAME = transfusionSum.ICD_NAME;
                        IcdProcessor.Reload(ucIcd, ado);
                    }


                    if (ucSecondaryIcd != null && IcdProcessor != null)
                    {
                        SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = transfusionSum.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = transfusionSum.ICD_TEXT;

                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }

                    txtTransfusionPersonCode.Text = transfusionSum.EXECUTE_LOGINNAME;
                    cboTransfusionPerson.EditValue = transfusionSum.EXECUTE_LOGINNAME;
                    if (transfusionSum.START_TIME != null)
                        dtTransfusionTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(transfusionSum.START_TIME ?? 0) ?? new DateTime();
                    else
                    {
                        dtTransfusionTimeFrom.EditValue = null;
                    }
                    if (transfusionSum.FINISH_TIME != null)
                        dtTransfusionTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(transfusionSum.FINISH_TIME ?? 0) ?? new DateTime();
                    else
                    {
                        dtTransfusionTimeTo.EditValue = null;
                    }

                    spCount.EditValue = transfusionSum.NUM_ORDER;
                    spTransfusionVolume.EditValue = transfusionSum.TRANSFUSION_VOLUME;
                    txtDescription.Text = transfusionSum.NOTE;
                }
                else
                {
                    if (ucIcd != null && IcdProcessor != null)
                    {
                        IcdProcessor.Reload(ucIcd, null);
                    }
                    if (ucSecondaryIcd != null && IcdProcessor != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, null);
                    }

                    txtTransfusionPersonCode.Text = "";
                    cboTransfusionPerson.EditValue = null;
                    dtTransfusionTimeFrom.EditValue = null;
                    dtTransfusionTimeTo.EditValue = null;
                    spCount.EditValue = null;
                    spTransfusionVolume.EditValue = null;
                    txtDescription.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataSaveFromControl(ref HisTransfusionSumSDO transfusionSumSDO)
        {
            try
            {

                if (cboTransfusionPerson.EditValue != null)
                {
                    var exe = acsUsers.FirstOrDefault(o => o.LOGINNAME == cboTransfusionPerson.EditValue.ToString());
                    if (exe != null)
                    {
                        transfusionSumSDO.ExecuteLoginname = exe.LOGINNAME;
                        transfusionSumSDO.ExecuteUsername = exe.USERNAME;

                    }

                    if (dtTransfusionTimeFrom.EditValue != null)
                    {
                        var from = dtTransfusionTimeFrom.DateTime.ToString("yyyyMMddHHmm");
                        transfusionSumSDO.StartTime = Inventec.Common.TypeConvert.Parse.ToInt64(from + "00");
                    }

                    if (dtTransfusionTimeTo.EditValue != null)
                    {
                        var from = dtTransfusionTimeTo.DateTime.ToString("yyyyMMddHHmm");
                        transfusionSumSDO.FinishTime = Inventec.Common.TypeConvert.Parse.ToInt64(from + "00");
                    }
                    if (spCount.EditValue != null)
                        transfusionSumSDO.NumOrder = Inventec.Common.TypeConvert.Parse.ToInt64(spCount.EditValue.ToString());


                    if (spTransfusionVolume.EditValue != null)
                    {
                        transfusionSumSDO.TransfusionVolume = (long)spTransfusionVolume.Value;

                    }
                    else
                        transfusionSumSDO.TransfusionVolume = null;


                    transfusionSumSDO.Note = txtDescription.Text;
                    if (ucIcd != null && IcdProcessor != null)
                    {
                        var icd = (IcdInputADO)IcdProcessor.GetValue(ucIcd);
                        if (icd != null)
                        {
                            transfusionSumSDO.IcdCode = icd.ICD_CODE;
                            transfusionSumSDO.IcdName = icd.ICD_NAME;

                        }
                    }
                    if (!string.IsNullOrEmpty(txtPhanUngCheo.Text))
                    {
                        transfusionSumSDO.Puc = txtPhanUngCheo.Text;
                    }
                    if (ucSecondaryIcd != null && subIcdProcessor != null)
                    {
                        var subIcd = (SecondaryIcdDataADO)subIcdProcessor.GetValue(ucSecondaryIcd);
                        if (subIcd != null)
                        {

                            transfusionSumSDO.IcdSubCode = subIcd.ICD_SUB_CODE;
                            transfusionSumSDO.IcdText = subIcd.ICD_TEXT;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddItemTransfusion()
        {
            try
            {
                if (this.clickTransfusionSum != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HIS_TRANSFUSION data = new HIS_TRANSFUSION();
                    data.MEASURE_TIME = this.clickTransfusionSum.START_TIME ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                    data.TRANSFUSION_SUM_ID = this.clickTransfusionSum.ID;
                    var rsApi = new BackendAdapter(param).Post<HIS_TRANSFUSION>("api/HisTransfusion/Create", ApiConsumers.MosConsumer, data, param);
                    if (rsApi != null)
                    {
                        success = true;
                        LoadDataToGridTransfusion(this.clickTransfusionSum.ID);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Event grid ExpMest Blood

        private void gridViewExpMestBlood_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_EXP_MEST_BLOOD pData = (V_HIS_EXP_MEST_BLOOD)gridViewExpMestBlood.GetRow(e.RowHandle);
                    if (pData != null)
                    {
                        if (e.Column.FieldName == "TransfusionBlood")
                        {
                            if (this.currentTransfusionSums != null && this.currentTransfusionSums.Count > 0 && this.currentTransfusionSums.Exists(o => o.EXP_MEST_BLOOD_ID == pData.ID))
                            {
                                e.RepositoryItem = Btn_TransfusionBlood_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = Btn_TransfusionBlood_Enable;
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

        private void gridViewExpMestBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "Status")// trạng thái
                        {
                            if (this.currentTransfusionSums != null && this.currentTransfusionSums.Count > 0 && this.currentTransfusionSums.Exists(o => o.EXP_MEST_BLOOD_ID == data.ID))
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else
                            {
                                e.Value = imageList1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "EXP_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXP_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestBlood_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestBlood)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlExpMestBlood.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            var row = (V_HIS_EXP_MEST_BLOOD)view.GetRow(info.RowHandle);
                            string text = "";
                            if (row != null)
                            {
                                if (info.Column.FieldName == "Status")
                                {
                                    if (this.currentTransfusionSums != null && this.currentTransfusionSums.Count > 0 && this.currentTransfusionSums.Exists(o => o.EXP_MEST_BLOOD_ID == row.ID))
                                    {
                                        text = "Đã truyền";
                                    }
                                    else
                                    {
                                        text = "Chưa truyền";
                                    }
                                }
                            }

                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_TransfusionBlood_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_BLOOD)gridViewExpMestBlood.GetFocusedRow();
                if (row != null)
                {
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;

                    //mở từ danh sách điều trị thì không thêm được, đã kết thúc cũng không thêm được
                    if (!(currentTreatment != null && currentTreatment.IS_PAUSE != 1 && (currentTreatment.LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID || (currentTreatment.CO_TREAT_DEPARTMENT_IDS != null && ("," + currentTreatment.CO_TREAT_DEPARTMENT_IDS + ",").Contains("," + currentRoom.DEPARTMENT_ID + ",")))))
                    {
                        btnSave.Enabled = false;
                    }

                    this.clickExpMestBlood = row;
                    FillDataExpMestBloodToControlForm(row, true);
                    ValidateForm(row.VOLUME, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event grid TranfusionSum

        private void gridControlTransfusionSum_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = true;
                var row = (V_HIS_TRANSFUSION_SUM)gridViewTransfusionSum.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    this.clickTransfusionSum = row;
                    this.clickExpMestBlood = this.currentExpMestBloods.FirstOrDefault(o => o.ID == row.EXP_MEST_BLOOD_ID);
                    this.printTransfusionSum = row;
                    // dis/en nut luu
                    btnSave.Enabled = this.currentTreatment != null && this.currentTreatment.IS_PAUSE != 1 && (this.currentTreatment.LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID || (this.currentTreatment.CO_TREAT_DEPARTMENT_IDS != null && ("," + this.currentTreatment.CO_TREAT_DEPARTMENT_IDS + ",").Contains("," + currentRoom.DEPARTMENT_ID + ",")));
                    FillDataTransfusionSumToControlForm(row);
                    FillDataExpMestBloodToControlForm(this.clickExpMestBlood, false);
                    LoadDataToGridTransfusion(row.ID);
                    WaitingManager.Hide();
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransfusionSum_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_TRANSFUSION_SUM pData = (V_HIS_TRANSFUSION_SUM)gridViewTransfusionSum.GetRow(e.RowHandle);
                    if (pData != null)
                    {
                        if (e.Column.FieldName == "Delete")
                        {
                            if (pData.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                            {
                                e.RepositoryItem = Btn_DeleteSum_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = Btn_DeleteSum_Disable;
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

        private void gridViewTransfusionSum_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TRANSFUSION_SUM data = (MOS.EFMODEL.DataModels.V_HIS_TRANSFUSION_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "START_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.START_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXECUTE_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.EXECUTE_LOGINNAME))
                            {
                                e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
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

        private void gridViewTransfusionSum_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var row = (V_HIS_TRANSFUSION_SUM)gridViewTransfusionSum.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (this.currentExpMestBloods != null && this.currentExpMestBloods.Count > 0 && this.currentExpMestBloods.Exists(o => o.ID == row.EXP_MEST_BLOOD_ID))
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_DeleteSum_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_TRANSFUSION_SUM)gridViewTransfusionSum.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        var rs = new BackendAdapter(param).Post<bool>("api/HisTransfusionSum/Delete", ApiConsumers.MosConsumer, row.ID, param);
                        if (rs)
                        {
                            this.currentTreatment = GetTreatmentByCode(row.TREATMENT_CODE, null, null);
                            LoadDataToGridExpMestBlood();
                            this.currentTransfusionSums = GetTransfusionSum(null, this.currentTreatment);
                            LoadDataToGrid(this.gridControlTransfusionSum, this.currentTransfusionSums);
                            LoadDataToGridTransfusion(null);
                        }
                        MessageManager.Show(this, param, rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event grid Transfusion

        private void gridViewTransfusion_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = (TransfusionADO)gridViewTransfusion.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "Action")
                    {
                        e.RepositoryItem = Rep_Button_Delete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransfusion_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridViewTransfusion_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

        private void gridControlTransfusion_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Rep_Button_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void Rep_Button_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TransfusionADO)gridViewTransfusion.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    var rsApi = new BackendAdapter(param).Post<bool>("api/HisTransfusion/Delete", ApiConsumers.MosConsumer, row.ID, param);
                    if (rsApi)
                    {
                        LoadDataToGridTransfusion(this.clickTransfusionSum.ID);
                    }
                    MessageManager.Show(this, param, rsApi);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransfusion_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        //mở từ danh sách điều trị thì không thêm được, đã kết thúc cũng không thêm được
                        if (hi.Column.FieldName == "Action" && currentTreatment != null && currentTreatment.IS_PAUSE != 1 && (currentTreatment.LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID || (currentTreatment.CO_TREAT_DEPARTMENT_IDS != null && ("," + currentTreatment.CO_TREAT_DEPARTMENT_IDS + ",").Contains("," + currentRoom.DEPARTMENT_ID + ","))))
                        {
                            AddItemTransfusion();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Rep_TextEdit_Skin_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gridViewTransfusion_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = (TransfusionADO)gridViewTransfusion.GetRow(e.RowHandle);
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HIS_TRANSFUSION data = new HIS_TRANSFUSION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSFUSION>(data, row);
                    data.MEASURE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.MEASURE_TIME_STR) ?? 0;

                    var rsApi = new BackendAdapter(param).Post<HIS_TRANSFUSION>("api/HisTransfusion/Update", ApiConsumers.MosConsumer, data, param);
                    if (rsApi != null)
                    {
                        success = true;
                        gridViewTransfusion.ClearColumnErrors();
                        //gridViewTransfusion.RefreshRow(e.RowHandle);
                    }
                    //MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransfusion_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                gridViewTransfusion.SetColumnError(gridViewTransfusion.FocusedColumn, e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransfusion_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "MEASURE_TIME_STR")
                {
                    if (e.Value == null)
                    {
                        e.Valid = false;
                        e.ErrorText = "Thời gian đo không được để trống";
                    }
                    else if (this.clickTransfusionSum != null)
                    {
                        if ((Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime)e.Value)) > this.clickTransfusionSum.FINISH_TIME || (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime)e.Value)) < this.clickTransfusionSum.START_TIME)
                        {
                            e.Valid = false;
                            e.ErrorText = "Thời gian đo không hợp lệ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event form

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled)
                btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000271", delegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }

                if (!btnSave.Enabled) return;

                if (this.clickExpMestBlood == null && this.clickTransfusionSum == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin", "Thông báo");
                    return;
                }

                bool success = false;
                CommonParam param = new CommonParam();


                HisTransfusionSumSDO transfusionSumSDO = new HisTransfusionSumSDO();
                UpdateDataSaveFromControl(ref transfusionSumSDO);

                if (this.clickExpMestBlood != null)
                {
                    transfusionSumSDO.ExpMestBloodId = this.clickExpMestBlood.ID;
                }
                transfusionSumSDO.TreatmentId = this.currentTreatment.ID;
                transfusionSumSDO.RequestRoomId = this.currentModule.RoomId;


                var rsApi = new BackendAdapter(param).Post<HIS_TRANSFUSION_SUM>("api/HisTransfusionSum/CreateOrUpdateSdo", ApiConsumers.MosConsumer, transfusionSumSDO, param);
                if (rsApi != null)
                {
                    success = true;
                    btnPrint.Enabled = true;
                    btnSave.Enabled = false;
                    LoadDataToGridExpMestBlood();
                    this.currentTransfusionSums = GetTransfusionSum(null, this.currentTreatment);
                    LoadDataToGrid(this.gridControlTransfusionSum, this.currentTransfusionSums);
                    ValidateForm(null, true);
                    this.printTransfusionSum = this.currentTransfusionSums.FirstOrDefault(o => o.ID == rsApi.ID);
                }

                MessageManager.Show(this, param, success);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransfusionPerson_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTransfusionPerson.EditValue != null)
                    {
                        var data = this.acsUsers.SingleOrDefault(o => o.LOGINNAME == cboTransfusionPerson.EditValue.ToString());
                        if (data != null)
                        {
                            txtTransfusionPersonCode.Text = data.LOGINNAME;
                            txtTransfusionPersonCode.Focus();
                            txtTransfusionPersonCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTransfusionPersonCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtTransfusionPersonCode.Text))
                    {
                        cboTransfusionPerson.EditValue = null;
                        cboTransfusionPerson.Focus();
                        cboTransfusionPerson.ShowPopup();
                    }
                    else
                    {
                        List<ACS_USER> searchs = null;
                        var listData1 = this.acsUsers.Where(o => o.LOGINNAME.ToUpper().Contains(txtTransfusionPersonCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.LOGINNAME.ToUpper() == txtTransfusionPersonCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtTransfusionPersonCode.Text = searchs[0].LOGINNAME;
                            cboTransfusionPerson.EditValue = searchs[0].LOGINNAME;
                            dtTransfusionTimeFrom.Focus();
                            dtTransfusionTimeFrom.SelectAll();
                        }
                        else
                        {
                            cboTransfusionPerson.Focus();
                            cboTransfusionPerson.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransfusionTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTransfusionTimeTo.Focus();
                    dtTransfusionTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTransfusionTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spCount.Focus();
                    spCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTransfusionVolume.Focus();
                    spTransfusionVolume.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spTransfusionVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        #endregion

        #region Validate

        private void ValidateForm(decimal? bloodVolume, bool cancel)
        {
            try
            {
                if (!cancel)
                {
                    ValidateTime();
                    ValidateVolume(bloodVolume);
                }
                else
                {
                    dxValidationProvider1.SetValidationRule(dtTransfusionTimeFrom, null);
                    dxValidationProvider1.SetValidationRule(spTransfusionVolume, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateTime()
        {
            try
            {
                ValidateTransfusionTime validate = new ValidateTransfusionTime();
                validate.dateEditFrom = dtTransfusionTimeFrom;
                validate.dateEditTo = dtTransfusionTimeTo;

                dxValidationProvider1.SetValidationRule(dtTransfusionTimeFrom, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateVolume(decimal? bloodVolume)
        {
            try
            {
                ValidateTransfusionVolume validate = new ValidateTransfusionVolume();
                validate.bloodVolume = bloodVolume;
                validate.spin = spTransfusionVolume;

                dxValidationProvider1.SetValidationRule(spTransfusionVolume, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Print

        private bool delegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.printTransfusionSum != null)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    var expMestBlood = this.currentExpMestBloods.FirstOrDefault(o => o.ID == this.printTransfusionSum.EXP_MEST_BLOOD_ID);

                    var transfusions = GetTransfusionByTransfusionSumId(this.printTransfusionSum.ID);

                    MPS.Processor.Mps000271.PDO.Mps000271ADO ado = new MPS.Processor.Mps000271.PDO.Mps000271ADO();
                    ado.expMestBlood = expMestBlood;
                    ado.transfusions = transfusions;
                    ado.transfusionSum = this.printTransfusionSum;
                    ado.treatment = this.currentTreatment;

                    MPS.Processor.Mps000271.PDO.SingleKey singleKey = new MPS.Processor.Mps000271.PDO.SingleKey();
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (room != null)
                    {
                        singleKey.CURRENT_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        singleKey.CURRENT_ROOM_NAME = room.ROOM_NAME;
                    }

                    singleKey.TEST_SERVICE_NAME = ProcessGetChildService(expMestBlood);

                    WaitingManager.Hide();
                    MPS.Processor.Mps000271.PDO.Mps000271PDO rdo = new MPS.Processor.Mps000271.PDO.Mps000271PDO(ado, singleKey);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : "", printTypeCode, this.currentModule.RoomId);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private string ProcessGetChildService(V_HIS_EXP_MEST_BLOOD expMestBlood)
        {
            string result = "";
            try
            {
                if (expMestBlood.TDL_SERVICE_REQ_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.PARENT_ID = expMestBlood.TDL_SERVICE_REQ_ID.Value;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        HisSereServFilter ssFilter = new HisSereServFilter();
                        ssFilter.SERVICE_REQ_IDs = lstServiceReq.Select(s => s.ID).ToList();
                        var sereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);
                        if (sereServ != null && sereServ.Count > 0)
                        {
                            List<String> serviceNames = new List<string>();
                            serviceNames.AddRange(sereServ.OrderBy(o => o.TDL_INTRUCTION_TIME).ThenBy(o => o.TDL_SERVICE_CODE).Select(s => s.TDL_SERVICE_NAME).ToList());
                            serviceNames = serviceNames.Distinct().ToList();
                            result = string.Join(",", serviceNames);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTreatmentCode.Text) && string.IsNullOrEmpty(txtPatientCode.Text) & string.IsNullOrEmpty(txtKeyword.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn cần nhập Mã điều trị, Mã bệnh nhân hoặc Từ khóa tìm kiếm", "Thông báo");
                    return;
                }
                LoadDataToGridExpMestBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnF1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.SelectAll();
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtPatientCode.SelectAll();
                txtPatientCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.SelectAll();
                txtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestBlood_Click(object sender, EventArgs e)
        {
           
        }

        private void GridViewExpMestBlood_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                btnSave.Enabled = false;
                var row = (V_HIS_EXP_MEST_BLOOD)gridViewExpMestBlood.GetFocusedRow();
                if (row != null)
                {
                    this.currentTreatment = GetTreatmentByCode(row.TDL_TREATMENT_CODE, null, null);
                    this.currentTransfusionSums = GetTransfusionSum(null, this.currentTreatment);
                    LoadDataToGrid(this.gridControlTransfusionSum, this.currentTransfusionSums);
                    FillDataExpMestBloodToControlForm(row, false);
                    btnSave.Enabled = this.currentTreatment != null && this.currentTreatment.IS_PAUSE != 1 && (this.currentTreatment.LAST_DEPARTMENT_ID == currentRoom.DEPARTMENT_ID || (this.currentTreatment.CO_TREAT_DEPARTMENT_IDS != null && ("," + this.currentTreatment.CO_TREAT_DEPARTMENT_IDS + ",").Contains("," + currentRoom.DEPARTMENT_ID + ",")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
