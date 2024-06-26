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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
//using HIS.Desktop.Plugins.AntibioticRequest.Resources;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
//using HIS.Desktop.Plugins.AntibioticRequest.Config;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.AntibioticRequest.ADO;
using static HIS.Desktop.ADO.AntibioticRequestADO;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.Common;
using System.Globalization;
using HIS.Desktop.Plugins.AntibioticRequest.Config;

namespace HIS.Desktop.Plugins.AntibioticRequest.Run
{
    public partial class frmAntibioticRequest : HIS.Desktop.Utility.FormBase
    {
        int lastRowHandle = -1;
        int positionHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        Inventec.Desktop.Common.Modules.Module moduleData;
        AntibioticRequestADO currentAntibioticRequest;
        long InstructionDate;
        List<AntibioticMicrobiADO> lstMicrobiADO = new List<AntibioticMicrobiADO>();
        List<AntibioticNewRegADO> lstNewRegADO = new List<AntibioticNewRegADO>();
        List<AntibioticOldRegADO> lstOldRegADO = new List<AntibioticOldRegADO>();

        List<AntibioticMicrobiADO> lstMicrobiADOTemp = new List<AntibioticMicrobiADO>();
        List<AntibioticNewRegADO> lstNewRegADOTemp = new List<AntibioticNewRegADO>();
        List<AntibioticOldRegADO> lstOldRegADOTemp = new List<AntibioticOldRegADO>();
        List<HIS_ICD> ListHisIcds = new List<HIS_ICD>();
        List<HIS_ICD> lstAllIcd = new List<HIS_ICD>();
        inputType InputType;
        List<IcdADO> icdSubcodeAdoChecks;
        List<HIS_ICD> currentIcds;
        bool isShowGridIcdSub;
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        IcdADO subIcdPopupSelect;
        bool isNotProcessWhileChangedTextSubIcd;
        bool isShowSubIcd = false;
        HIS_ICD IcdSubChoose;
        List<string> icdCodeList = new List<string>();
        bool isShowSubTemp;
        string[] icdSeparators = new string[] { ",", ";" };
        List<HIS_ANTIBIOTIC_MICROBI> currentAntibioticMicrobi = new List<HIS_ANTIBIOTIC_MICROBI>();
        List<HIS_ANTIBIOTIC_OLD_REG> currentAntibioticOldReg = new List<HIS_ANTIBIOTIC_OLD_REG>();
        HIS_DHST currentDhst = new HIS_DHST();
        List<V_HIS_ANTIBIOTIC_NEW_REG> currentAntibioticNewRegView = new List<V_HIS_ANTIBIOTIC_NEW_REG>();
        List<HIS_ACTIVE_INGREDIENT> listActiveIngredients = new List<HIS_ACTIVE_INGREDIENT>();
        HisAntibioticRequestResultSDO resultRequest = new HisAntibioticRequestResultSDO();
        V_HIS_ANTIBIOTIC_REQUEST resultApprove = new V_HIS_ANTIBIOTIC_REQUEST();
        V_HIS_ANTIBIOTIC_REQUEST currentAntibiotiRequestView = new V_HIS_ANTIBIOTIC_REQUEST();
        int numYHdDialysis = 0;
        int numNHdDialysis = 0;
        int numYContinouousDialysis = 0;
        int numNContinouousDialysis = 0;
        int numYAntibioticRequestStt = 0;
        int numNAntibioticRequestStt = 0;

        RefeshReference refreshData = null;
        private enum inputType
        {
            OPEN_FROM_LIST_ANTIBIOTIC_REQUEST,
            CREATE_ANTIBIOTIC_REQUEST,
            EDIT_ANTIBIOTIC_REQUEST
        }
        public frmAntibioticRequest(Inventec.Desktop.Common.Modules.Module moduleData, AntibioticRequestADO adoSend, RefeshReference fr) : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentAntibioticRequest = adoSend;
                this.refreshData = fr;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentAntibioticRequest), currentAntibioticRequest));

                this.moduleData = moduleData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void frmAntibioticRequest_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                FillIcdToGrid();
                LoadDafaultGrid();
                SetValidMaxLengthControl();
                FillDataControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void FillIcdToGrid()
        {
            try
            {
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                ReloadIcdSubContainerByCodeChanged();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ReloadIcdSubContainerByCodeChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.1");
                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds select new IcdADO(m, codes)).ToList();
                customGridControlSubIcdName.DataSource = null;
                customGridControlSubIcdName.DataSource = this.icdSubcodeAdoChecks;
                gvIcdSubCode.GridControl.DataSource = null;
                gvIcdSubCode.GridControl.DataSource = this.currentIcds.ToList();
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataControl()
        {
            try
            {
                EnabelControlLayoutRequest(false);
                lstAllIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                if (this.currentAntibioticRequest != null)
                {
                    InstructionDate = this.currentAntibioticRequest.InstructionDate;
                    if (this.currentAntibioticRequest.processType != null && this.currentAntibioticRequest.processType == ProcessType.Request && (this.currentAntibioticRequest.AntibioticRequest == null
                        || (this.currentAntibioticRequest.AntibioticRequest != null && (this.currentAntibioticRequest.AntibioticRequest.ANTIBIOTIC_REQUEST_STT == 1 || this.currentAntibioticRequest.AntibioticRequest.ANTIBIOTIC_REQUEST_STT == 3))))
                    {
                        EnabelControlDHST(true);
                        EnabelControlLayoutRequest(true);
                    }

                    if (this.currentAntibioticRequest.AntibioticRequest != null)
                    {
                        VisibleLayout();
                    }
                    if (this.currentAntibioticRequest.processType != null && this.currentAntibioticRequest.processType == ProcessType.Approval)
                    {
                        EnabelControlLayoutApproval(true);
                    }
                    else
                    {
                        EnabelControlLayoutApproval(false);
                    }

                    if (this.currentAntibioticRequest.processType == null)
                    {
                        btnSave.Enabled = false;
                    }

                    FillDataToGridAntibioticMicrobi();
                    FillDataToGridAntibioticOldReg();
                    FillDataToGridAntibioticNewReg();

                    //this.currentAntibioticRequest.AntibioticRequest = GetAntibiticRequest();

                    spnCrCl.EditValue = null;
                    spnHeight.EditValue = null;
                    spnTemperature.EditValue = null;
                    spnWeight.EditValue = null;


                    if (this.currentAntibioticRequest.AntibioticRequest != null)
                    {
                        btnPrint.Enabled = true;
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_ANTIBIOTIC_REQUEST>(this.currentAntibiotiRequestView, this.currentAntibioticRequest.AntibioticRequest);

                        lblDepartmentRequest.Text = this.currentAntibioticRequest.AntibioticRequest.REQUEST_DEPARTMENT_NAME;
                        dteTimeRequest.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentAntibioticRequest.AntibioticRequest.REQUEST_TIME) ?? DateTime.Now;
                        lblPatientCode.Text = this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_CODE;
                        lblPatientName.Text = this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_NAME;
                        if (this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            lblPatientDob.Text = this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_DOB);
                        }
                        lblPatientGenderName.Text = this.currentAntibioticRequest.AntibioticRequest.TDL_PATIENT_GENDER_NAME;
                        if (this.currentAntibioticRequest.AntibioticRequest.DHST_ID > 0)
                        {
                            CommonParam param = new CommonParam();
                            HisDhstFilter filter = new HisDhstFilter();
                            filter.ID = this.currentAntibioticRequest.AntibioticRequest.DHST_ID;
                            var dataDhst = new BackendAdapter(param)
            .Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                this.currentDhst = dataDhst.First();
                                spnHeight.EditValue = dataDhst.First().HEIGHT != null ? dataDhst.First().HEIGHT.ToString() : "";
                                spnWeight.EditValue = dataDhst.First().WEIGHT != null ? dataDhst.First().WEIGHT.ToString() : "";
                                spnTemperature.EditValue = dataDhst.First().TEMPERATURE != null ? dataDhst.First().TEMPERATURE.ToString() : "";
                            }
                        }
                        txtAllergy.Text = this.currentAntibioticRequest.AntibioticRequest.ALLERGY;
                        chkInfectionShock.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_INFECTION_SHOCK == 1;
                        chkCommunityPneumonia.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_COMMUNITY_PNEUMONIA == 1;
                        chkHospitalPneumonia.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_HOSPITAL_PNEUMONIA == 1;
                        chkVentilatorPneumonia.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_VENTILATOR_PNEUMONIA == 1;
                        chkMeningitis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_MENINGITIS == 1;
                        chkUrinaryInfection.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_URINARY_INFECTION == 1;
                        chkAbdominalInfection.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_ABDOMINAL_INFECTION == 1;
                        chkSepsis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_SEPSIS == 1;
                        chkSkinInfection.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_SKIN_INFECTION == 1;
                        chkOtherInfection.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_OTHER_INFECTION == 1;
                        memInfectionEntry.Text = this.currentAntibioticRequest.AntibioticRequest.INFECTION_ENTRY;

                        txtIcdSubCode.Text = this.currentAntibioticRequest.AntibioticRequest.ICD_SUB_CODE;
                        txtIcdText.Text = this.currentAntibioticRequest.AntibioticRequest.ICD_TEXT;
                        memClinalCondition.Text = this.currentAntibioticRequest.AntibioticRequest.CLINICAL_CONDITION;
                        txtWhiteBloodCell.Text = this.currentAntibioticRequest.AntibioticRequest.WHITE_BLOOD_CELL;
                        txtCrp.Text = this.currentAntibioticRequest.AntibioticRequest.CRP;
                        txtPct.Text = this.currentAntibioticRequest.AntibioticRequest.PCT;
                        memSubclinicalResult.Text = this.currentAntibioticRequest.AntibioticRequest.SUBCLINICAL_RESULT;
                        spnCrCl.EditValue = this.currentAntibioticRequest.AntibioticRequest.CRCL != null ? this.currentAntibioticRequest.AntibioticRequest.CRCL.ToString() : "";
                        chkYHdDialysis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_HD_DIALYSIS == 1;
                        chkNHdDialysis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_HD_DIALYSIS == 0;

                        chkYContinouousDialysis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_CONTINUOUS_DIALYSIS == 1;
                        chkNContinouousDialysis.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_CONTINUOUS_DIALYSIS == 0;
                        memNoMicrobiologyReason.Text = this.currentAntibioticRequest.AntibioticRequest.NO_MICROBIOLOGY_REASON;
                        chkIsLessResponsiveRegimen.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_LESS_RESPONSIVE_REGIMEN == 1;
                        chkIsDrugResistance.Checked = this.currentAntibioticRequest.AntibioticRequest.IS_DRUG_RESISTANCE == 1;
                        if (!string.IsNullOrEmpty(this.currentAntibioticRequest.AntibioticRequest.ADR_ANTIBIOTIC_NAME))
                        {
                            txtAdrAntibioticName.Text = this.currentAntibioticRequest.AntibioticRequest.ADR_ANTIBIOTIC_NAME;
                            chkIsAdrAntibiotic.Checked = true;
                        }

                        memOtherReason.Text = this.currentAntibioticRequest.AntibioticRequest.OTHER_REASON;
                        chkYAntibioticRequestStt.Checked = this.currentAntibioticRequest.AntibioticRequest.ANTIBIOTIC_REQUEST_STT == 2;
                        chkNAntibioticRequestStt.Checked = this.currentAntibioticRequest.AntibioticRequest.ANTIBIOTIC_REQUEST_STT == 3;
                        memOtherOption.Text = this.currentAntibioticRequest.AntibioticRequest.OTHER_OPINION;

                    }
                    else
                    {
                        lblDepartmentRequest.Text = BackendDataWorker.Get<HIS_DEPARTMENT>().First(p => p.ID == BackendDataWorker.Get<HIS_ROOM>().First(o => o.ID == moduleData.RoomId).DEPARTMENT_ID).DEPARTMENT_NAME;
                        dteTimeRequest.DateTime = DateTime.Now;
                        lblPatientCode.Text = this.currentAntibioticRequest.PatientCode;
                        lblPatientName.Text = this.currentAntibioticRequest.PatientName;
                        if (this.currentAntibioticRequest.IsHasNotDayDob)
                        {
                            lblPatientDob.Text = this.currentAntibioticRequest.Dob.ToString().Substring(0, 4);
                        }
                        else
                        {
                            lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentAntibioticRequest.Dob);
                        }
                        lblPatientGenderName.Text = this.currentAntibioticRequest.GenderName;
                        if (this.currentAntibioticRequest.Height != null)
                            spnHeight.EditValue = this.currentAntibioticRequest.Height;
                        if (this.currentAntibioticRequest.Weight != null)
                            spnWeight.EditValue = this.currentAntibioticRequest.Weight;
                        if (this.currentAntibioticRequest.Temperature != null)
                            spnTemperature.EditValue = this.currentAntibioticRequest.Temperature;


                        txtIcdSubCode.Text = this.currentAntibioticRequest.IcdSubCode;
                        txtIcdText.Text = this.currentAntibioticRequest.IcdText;

                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnabelControlDHST(bool isEnable)
        {
            try
            {
                spnHeight.Enabled = isEnable;
                spnWeight.Enabled = isEnable;
                spnTemperature.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnabelControlLayoutApproval(bool isEnable)
        {
            try
            {
                labelControl8.Enabled = isEnable;
                layoutControlItem54.Enabled = isEnable;
                layoutControlItem55.Enabled = isEnable;
                chkNAntibioticRequestStt.Enabled = isEnable;
                memOtherOption.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnabelControlLayoutRequest(bool isEnable)
        {
            try
            {
                dteTimeRequest.Enabled = isEnable;
                spnTemperature.Enabled = isEnable;
                spnWeight.Enabled = isEnable;
                label1.Enabled = isEnable;
                label2.Enabled = isEnable;
                label3.Enabled = isEnable;
                label5.Enabled = isEnable;
                layoutControlItem36.Enabled = isEnable;
                spnHeight.Enabled = isEnable;

                txtAllergy.Enabled = isEnable;
                chkInfectionShock.Enabled = isEnable;
                chkCommunityPneumonia.Enabled = isEnable;
                chkHospitalPneumonia.Enabled = isEnable;

                chkVentilatorPneumonia.Enabled = isEnable;
                chkMeningitis.Enabled = isEnable;
                chkUrinaryInfection.Enabled = isEnable;
                chkAbdominalInfection.Enabled = isEnable;

                chkSepsis.Enabled = isEnable;
                chkSkinInfection.Enabled = isEnable;
                chkOtherInfection.Enabled = isEnable;
                memInfectionEntry.Enabled = isEnable;

                txtIcdSubCode.Enabled = isEnable;
                txtIcdText.Enabled = isEnable;
                memClinalCondition.Enabled = isEnable;
                txtWhiteBloodCell.Enabled = isEnable;

                txtCrp.Enabled = isEnable;
                txtPct.Enabled = isEnable;
                memSubclinicalResult.Enabled = isEnable;
                spnCrCl.Enabled = isEnable;

                chkYHdDialysis.Enabled = isEnable;
                chkNHdDialysis.Enabled = isEnable;
                chkYContinouousDialysis.Enabled = isEnable;
                chkNContinouousDialysis.Enabled = isEnable;

                gridViewAntibioticMicrobi.OptionsBehavior.Editable = isEnable;
                memNoMicrobiologyReason.Enabled = isEnable;
                chkYContinouousDialysis.Enabled = isEnable;
                chkNContinouousDialysis.Enabled = isEnable;

                gridViewAntibioticOldReg.OptionsBehavior.Editable = isEnable;
                chkIsLessResponsiveRegimen.Enabled = isEnable;
                chkIsDrugResistance.Enabled = isEnable;
                chkIsAdrAntibiotic.Enabled = isEnable;
                txtAdrAntibioticName.Enabled = isEnable;

                memOtherReason.Enabled = isEnable;
                gridViewAntibioticNewReg.OptionsBehavior.Editable = isEnable;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void VisibleLayout()
        {
            try
            {
                layoutControlItem50.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem54.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem54.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem55.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem56.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDafaultGrid()
        {
            try
            {
                LoadDefaultGridAntibioticMicrobi();
                LoadDefaultGridAntibioticNewReg();
                LoadDefaultGridAntibioticOldReg();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (!txtIcdText.Enabled) return;
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                    {
                        if (txtIcdSubCode.Text.Contains(";"))
                        {
                            this.ListHisIcds = lstAllIcd.Where(o => txtIcdSubCode.Text.Split(';').ToList().Exists(p => p == o.ICD_CODE)).ToList();
                        }
                        else
                        {
                            this.ListHisIcds = lstAllIcd.Where(o => o.ICD_CODE == txtIcdSubCode.Text.Trim()).ToList();
                        }

                    }
                    frmSubIcd frm = new frmSubIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.ListHisIcds);
                    WaitingManager.Hide();
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                else
                {
                    txtIcdSubCode.Text = null;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
                else
                {
                    txtIcdText.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (!txtIcdSubCode.Enabled) return;
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                    {
                        if (txtIcdSubCode.Text.Contains(";"))
                        {
                            this.ListHisIcds = lstAllIcd.Where(o => txtIcdSubCode.Text.Split(';').ToList().Exists(p => p == o.ICD_CODE)).ToList();
                        }
                        else
                        {
                            this.ListHisIcds = lstAllIcd.Where(o => o.ICD_CODE == txtIcdSubCode.Text.Trim()).ToList();
                        }

                    }
                    frmSubIcd frm = new frmSubIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.ListHisIcds);
                    WaitingManager.Hide();
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkYHdDialysis_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNHdDialysis.Checked)
                {
                    chkNHdDialysis.Checked = false;
                }
                chkYHdDialysis.Checked = numYHdDialysis % 2 == 0 ? true : false;
                numYHdDialysis++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNHdDialysis_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkYHdDialysis.Checked)
                {
                    chkYHdDialysis.Checked = false;
                }
                chkNHdDialysis.Checked = numNHdDialysis % 2 == 0 ? true : false;
                numNHdDialysis++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkYContinouousDialysis_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNContinouousDialysis.Checked)
                {
                    chkNContinouousDialysis.Checked = false;
                }
                chkYContinouousDialysis.Checked = numYContinouousDialysis % 2 == 0 ? true : false;
                numYContinouousDialysis++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNContinouousDialysis_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkYContinouousDialysis.Checked)
                {
                    chkYContinouousDialysis.Checked = false;
                }
                chkNContinouousDialysis.Checked = numNContinouousDialysis % 2 == 0 ? true : false;
                numNContinouousDialysis++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (this.currentAntibioticRequest.processType == null)
                    return;
                if (!btnSave.Enabled)
                    return;
                positionHandle = -1;
                if (!(dxValidationProvider1.Validate() && CheckValidationGrid()))
                    return;
                RemoveRowEmpty();
                if (this.currentAntibioticRequest.processType == ProcessType.Request)
                {

                    if (lstNewRegADOTemp == null || lstNewRegADOTemp.Count == 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có kháng sinh yêu cầu", "Thông báo", MessageBoxButtons.OK);
                        return;

                    }
                    if (HisConfigCFG.PolicyOption == "2")
                    {
                        var lstNewRegError = lstNewRegADOTemp.Where(o => o.START_DATE == null || o.START_DATE == 0 || o.USE_DAY == null || o.USE_DAY == 0).ToList();
                        if (lstNewRegError != null && lstNewRegError.Count > 0 && listActiveIngredients != null && listActiveIngredients.Count > 0)
                        {
                            var lstNewRegErrorId = lstNewRegError.Select(o => o.ACTIVE_INGREDIENT_ID).Distinct().ToList();
                            List<string> antibioticNames = new List<string>();
                            foreach (var id in lstNewRegErrorId)
                            {
                                HIS_ACTIVE_INGREDIENT activeIngredient = listActiveIngredients.FirstOrDefault(o => o.ID == id);
                                if (activeIngredient != null)
                                    antibioticNames.Add(activeIngredient.ACTIVE_INGREDIENT_NAME);
                            }
                            XtraMessageBox.Show(String.Format("Kháng sinh {0} nhập thiếu thông tin bắt đầu hoặc thời gian điều trị.", String.Join(", ", antibioticNames)), "Thông báo");
                            return;
                        }
                    }
                    WaitingManager.Show();
                    HisAntibioticRequestSDO sdo = new HisAntibioticRequestSDO();
                    List<HIS_ANTIBIOTIC_MICROBI> listAntibioticMicrobi = GetListAntibioticMicrobi();
                    List<HIS_ANTIBIOTIC_OLD_REG> listAntibioticOldReg = GetListAntibioticOldReg();
                    List<HIS_ANTIBIOTIC_NEW_REG> listAntibioticNewReg = GetListAntibioticNewReg();
                    sdo.AntibioticMicrobis = listAntibioticMicrobi != null && listAntibioticMicrobi.Count > 0 ? listAntibioticMicrobi : null;
                    sdo.AntibioticOldRegs = listAntibioticOldReg != null && listAntibioticOldReg.Count > 0 ? listAntibioticOldReg : null;
                    sdo.AntibioticNewRegs = listAntibioticNewReg != null && listAntibioticNewReg.Count > 0 ? listAntibioticNewReg : null;
                    HIS_DHST dhst = GetDhst();
                    if (dhst != null)
                    {
                        sdo.Height = dhst.HEIGHT;
                        sdo.Weight = dhst.WEIGHT;
                        sdo.Temperature = dhst.TEMPERATURE;
                    }
                    HIS_ANTIBIOTIC_REQUEST antibiticRequest = GetAntibiticRequest();
                    sdo.AntibioticRequest = antibiticRequest != null ? antibiticRequest : null;
                    sdo.WorkingRoomId = moduleData.RoomId;

                    if (this.currentAntibioticRequest.ExpMestId > 0)
                        sdo.ExpMestId = this.currentAntibioticRequest.ExpMestId;

                    Inventec.Common.Logging.LogSystem.Debug("INPUT____Request" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    resultRequest = new BackendAdapter(param).Post<HisAntibioticRequestResultSDO>("api/HisAntibioticRequest/Request", ApiConsumers.MosConsumer, sdo, param);
                    Inventec.Common.Logging.LogSystem.Debug("OUTPUT____Request" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultRequest), resultRequest));

                    if (resultRequest != null)
                    {
                        btnPrint.Enabled = true;
                        success = true;
                        this.currentAntibiotiRequestView = resultRequest.AntibioticRequest;
                        this.currentAntibioticNewRegView = resultRequest.AntibioticNewRegs;
                        this.currentAntibioticMicrobi = resultRequest.AntibioticMicrobis;
                        this.currentAntibioticOldReg = resultRequest.AntibioticOldRegs;
                        if (resultRequest.AntibioticRequest != null && resultRequest.AntibioticRequest.DHST_ID > 0)
                        {
                            HisDhstFilter filter = new HisDhstFilter();
                            filter.ID = currentAntibiotiRequestView.DHST_ID;
                            var dataDhst = new BackendAdapter(param)
            .Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                this.currentDhst = dataDhst.First();
                            }
                        }
                        WaitingManager.Hide();
                    }

                }
                else if (this.currentAntibioticRequest.processType == ProcessType.Approval)
                {
                    if (!chkYAntibioticRequestStt.Checked && !chkNAntibioticRequestStt.Checked)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Yêu cầu kháng sinh chưa được phê duyệt", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                    HisAntibioticRequestApproveSDO sdo = new HisAntibioticRequestApproveSDO();
                    if (this.currentAntibioticRequest.AntibioticRequest.ID > 0)
                        sdo.AntibioticRequestId = this.currentAntibioticRequest.AntibioticRequest.ID;
                    sdo.OtherOpinion = memOtherOption.Text;
                    if (chkYAntibioticRequestStt.Checked)
                        sdo.IsApproved = true;
                    if (chkNAntibioticRequestStt.Checked)
                        sdo.IsApproved = false;
                    Inventec.Common.Logging.LogSystem.Debug("INPUT____Approve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    resultApprove = new BackendAdapter(param).Post<V_HIS_ANTIBIOTIC_REQUEST>("api/HisAntibioticRequest/Approve", ApiConsumers.MosConsumer, sdo, param);
                    Inventec.Common.Logging.LogSystem.Debug("OUTPUT____Approve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultApprove), resultApprove));
                    if (resultApprove != null)
                    {
                        this.currentAntibiotiRequestView = resultApprove;
                        success = true;
                        WaitingManager.Hide();
                    }
                }
                if (refreshData != null)
                {
                    this.refreshData();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private HIS_ANTIBIOTIC_REQUEST GetAntibiticRequest()
        {
            HIS_ANTIBIOTIC_REQUEST obj = new HIS_ANTIBIOTIC_REQUEST();
            try
            {
                if (this.currentAntibioticRequest.AntibioticRequest != null)
                    obj.ID = this.currentAntibioticRequest.AntibioticRequest.ID;
                if (this.currentAntibiotiRequestView != null)
                    obj.ID = this.currentAntibiotiRequestView.ID;
                obj.REQUEST_TIME = (dteTimeRequest.EditValue != null && dteTimeRequest.DateTime != DateTime.MinValue) ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteTimeRequest.DateTime) ?? 0 : 0;
                obj.ALLERGY = txtAllergy.Text;
                obj.IS_INFECTION_SHOCK = chkInfectionShock.Checked ? (short?)1 : 0;
                obj.IS_COMMUNITY_PNEUMONIA = chkCommunityPneumonia.Checked ? (short?)1 : 0;
                obj.IS_HOSPITAL_PNEUMONIA = chkHospitalPneumonia.Checked ? (short?)1 : 0;
                obj.IS_VENTILATOR_PNEUMONIA = chkVentilatorPneumonia.Checked ? (short?)1 : 0;
                obj.IS_MENINGITIS = chkMeningitis.Checked ? (short?)1 : 0;
                obj.IS_URINARY_INFECTION = chkUrinaryInfection.Checked ? (short?)1 : 0;
                obj.IS_ABDOMINAL_INFECTION = chkAbdominalInfection.Checked ? (short?)1 : 0;
                obj.IS_SEPSIS = chkSepsis.Checked ? (short?)1 : 0;
                obj.IS_SKIN_INFECTION = chkSkinInfection.Checked ? (short?)1 : 0;
                obj.IS_OTHER_INFECTION = chkOtherInfection.Checked ? (short?)1 : 0;
                obj.INFECTION_ENTRY = memInfectionEntry.Text;
                obj.ICD_SUB_CODE = txtIcdSubCode.Text;
                obj.ICD_TEXT = txtIcdText.Text;
                obj.CLINICAL_CONDITION = memClinalCondition.Text;
                obj.WHITE_BLOOD_CELL = txtWhiteBloodCell.Text;
                obj.CRP = txtCrp.Text;
                obj.PCT = txtPct.Text;
                obj.SUBCLINICAL_RESULT = memSubclinicalResult.Text;
                if (spnCrCl.EditValue != null && spnCrCl.Value > 0)
                    obj.CRCL = Inventec.Common.Number.Get.RoundCurrency(spnCrCl.Value, 2);
                obj.IS_HD_DIALYSIS = null;
                if (chkYHdDialysis.Checked)
                    obj.IS_HD_DIALYSIS = 1;
                if (chkNHdDialysis.Checked)
                    obj.IS_HD_DIALYSIS = 0;
                obj.IS_CONTINUOUS_DIALYSIS = null;
                if (chkYContinouousDialysis.Checked)
                    obj.IS_CONTINUOUS_DIALYSIS = 1;
                if (chkNContinouousDialysis.Checked)
                    obj.IS_CONTINUOUS_DIALYSIS = 0;
                obj.NO_MICROBIOLOGY_REASON = memNoMicrobiologyReason.Text;
                obj.IS_LESS_RESPONSIVE_REGIMEN = chkIsLessResponsiveRegimen.Checked ? (short?)1 : 0;
                obj.IS_DRUG_RESISTANCE = chkIsDrugResistance.Checked ? (short?)1 : 0;
                obj.ADR_ANTIBIOTIC_NAME = txtAdrAntibioticName.Text;
                obj.OTHER_REASON = memOtherReason.Text;
                obj.ANTIBIOTIC_REQUEST_STT = 1;
                if (chkYAntibioticRequestStt.Checked)
                    obj.ANTIBIOTIC_REQUEST_STT = 2;
                if (chkNAntibioticRequestStt.Checked)
                    obj.ANTIBIOTIC_REQUEST_STT = 3;
                obj.OTHER_OPINION = memOtherOption.Text;

            }
            catch (Exception ex)
            {
                obj = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetDhst()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (spnHeight.EditValue != null && spnHeight.Value > 0)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight.Value, 2);
                if (spnWeight.EditValue != null && spnWeight.Value > 0)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight.Value, 2);
                if (spnTemperature.EditValue != null && spnTemperature.Value > 0)
                    obj.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spnTemperature.Value, 2);
            }
            catch (Exception ex)
            {
                obj = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private List<HIS_ANTIBIOTIC_MICROBI> GetListAntibioticMicrobi()
        {
            List<HIS_ANTIBIOTIC_MICROBI> lst = new List<HIS_ANTIBIOTIC_MICROBI>();
            try
            {
                foreach (var item in lstMicrobiADOTemp)
                {
                    HIS_ANTIBIOTIC_MICROBI obj = new HIS_ANTIBIOTIC_MICROBI();
                    obj.SPECIMENS = item.SPECIMENS;
                    if (item.IMPLANTION_TIME != null && item.IMPLANTION_TIME.ToString().Length < 9)
                        obj.IMPLANTION_TIME = Int64.Parse(item.IMPLANTION_TIME.ToString() + "000000");
                    else
                        obj.IMPLANTION_TIME = item.IMPLANTION_TIME;
                    if (item.RESULT_TIME != null && item.RESULT_TIME.ToString().Length < 9)
                        obj.RESULT_TIME = Int64.Parse(item.RESULT_TIME.ToString() + "000000");
                    else
                        obj.RESULT_TIME = item.RESULT_TIME;
                    obj.RESULT = item.RESULT;
                    lst.Add(obj);
                }
            }
            catch (Exception ex)
            {
                lst = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lst;
        }
        private List<HIS_ANTIBIOTIC_OLD_REG> GetListAntibioticOldReg()
        {
            List<HIS_ANTIBIOTIC_OLD_REG> lst = new List<HIS_ANTIBIOTIC_OLD_REG>();
            try
            {
                foreach (var item in lstOldRegADOTemp)
                {
                    HIS_ANTIBIOTIC_OLD_REG obj = new HIS_ANTIBIOTIC_OLD_REG();
                    obj.ANTIBIOTIC_REQUEST_ID = item.ANTIBIOTIC_REQUEST_ID;
                    obj.ANTIBIOTIC_NAME = item.ANTIBIOTIC_NAME;
                    lst.Add(obj);
                }
            }
            catch (Exception ex)
            {
                lst = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lst;
        }

        private List<HIS_ANTIBIOTIC_NEW_REG> GetListAntibioticNewReg()
        {
            List<HIS_ANTIBIOTIC_NEW_REG> lst = new List<HIS_ANTIBIOTIC_NEW_REG>();
            try
            {
                foreach (var item in lstNewRegADOTemp)
                {
                    HIS_ANTIBIOTIC_NEW_REG obj = new HIS_ANTIBIOTIC_NEW_REG();
                    obj.ANTIBIOTIC_REQUEST_ID = item.ANTIBIOTIC_REQUEST_ID;
                    obj.ACTIVE_INGREDIENT_ID = item.ACTIVE_INGREDIENT_ID;
                    obj.CONCENTRA = item.CONCENTRA;
                    obj.DOSAGE = item.DOSAGE;
                    obj.PERIOD = item.PERIOD;
                    if (item.USE_DAY != null)
                    {
                        obj.USE_DAY = Inventec.Common.Number.Get.RoundCurrency(item.USE_DAY ?? 0, 4);
                    }
                    else
                    {
                        obj.USE_DAY = null;
                    }
                    if (item.START_DATE != null && item.START_DATE > 0)
                    {
                        obj.START_DATE = Convert.ToInt64(item.START_DATE.ToString().Substring(0, 8) + "000000");
                    }
                    else
                    {
                        obj.START_DATE = null;
                    }
                    if (item.END_DATE != null && item.END_DATE > 0)
                    {
                        obj.END_DATE = Convert.ToInt64(item.END_DATE.ToString().Substring(0, 8) + "000000");
                    }
                    else
                    {
                        obj.END_DATE = null;
                    }
                    obj.USE_FORM = item.USE_FORM;
                    lst.Add(obj);
                }
            }
            catch (Exception ex)
            {
                lst = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lst;
        }

        private void RemoveRowEmpty()
        {
            try
            {
                if (lstMicrobiADO != null && lstMicrobiADO.Count > 0)
                {
                    lstMicrobiADOTemp = lstMicrobiADO.Where(o => !string.IsNullOrEmpty(o.SPECIMENS)
                                                        || (o.IMPLANTION_TIME != null && o.IMPLANTION_TIME > 0)
                                                        || (o.RESULT_TIME != null && o.RESULT_TIME > 0)
                                                        || !string.IsNullOrEmpty(o.RESULT)
                                                        ).ToList();
                }
                if (lstOldRegADO != null && lstOldRegADO.Count > 0)
                {
                    lstOldRegADOTemp = lstOldRegADO.Where(o => !string.IsNullOrEmpty(o.ANTIBIOTIC_NAME)
                                                        ).ToList();
                }
                if (lstNewRegADO != null && lstNewRegADO.Count > 0)
                {
                    lstNewRegADOTemp = lstNewRegADO.Where(o => !string.IsNullOrEmpty(o.CONCENTRA)
                                                        || !string.IsNullOrEmpty(o.DOSAGE)
                                                        || !string.IsNullOrEmpty(o.PERIOD)
                                                        || (o.USE_DAY != null && o.USE_DAY > 0)
                                                        || !string.IsNullOrEmpty(o.USE_FORM)
                                                        || (o.START_DATE != null && o.START_DATE > 0)
                                                        || (o.END_DATE != null && o.END_DATE > 0)
                                                        ).ToList();
                    if (lstNewRegADOTemp != null && lstNewRegADOTemp.Count > 0)
                    {
                        lstNewRegADOTemp = lstNewRegADOTemp.Where(o => !o.ANTIBIOTIC_DELETE).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckValidationGrid()
        {
            bool valid = false;
            try
            {
                var checkGridMicrobi = lstMicrobiADO.Where(o => o.ErrorTypeResult == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning || o.ErrorTypeSpecimens == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning).ToList();
                if (checkGridMicrobi != null && checkGridMicrobi.Count > 0)
                    return valid;

                var checkGridNewReg = lstNewRegADO.Where(o => o.ErrorTypeConcentra == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning || o.ErrorTypePeriod == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning
                 || o.ErrorTypeUseForm == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning || o.ErrorTypeDosage == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning).ToList();
                if (checkGridNewReg != null && checkGridNewReg.Count > 0)
                    return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void bbtnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate("Mps000462", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCrCl_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
                TextEdit txt = sender as TextEdit;
                if (!string.IsNullOrEmpty(txt.Text) && txt.Text.IndexOf(".") > -1 && (e.KeyChar == '.'
                    || (txt.Text.Length - 3 == txt.Text.IndexOf(".") && char.IsDigit(e.KeyChar) && txt.SelectionStart > txt.Text.IndexOf("."))))
                    e.Handled = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!isShowGridIcdSub)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainer1.HidePopup();
                        memClinalCondition.Focus();
                    }
                    else
                    {
                        this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as IcdADO;
                        if (this.subIcdPopupSelect != null)
                        {
                            isShowContainerMediMaty = false;
                            isShowContainerMediMatyForChoose = true;

                            this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                            this.customGridControlSubIcdName.RefreshDataSource();

                            SetCheckedSubIcdsToControl();
                            popupControlContainer1.HidePopup();
                            isShowGridIcdSub = false;
                            txtIcdText.Focus();
                            txtIcdText.SelectionStart = txtIcdText.Text.Length;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    ShowPopupContainerIcsSub();
                    customGridViewSubIcdName.Focus();
                    customGridViewSubIcdName.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey)
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    ShowPopupContainerIcsSub();
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    e.Handled = true;
                }
                else if (e.KeyCode != null)
                {
                    isShowGridIcdSub = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetCheckedSubIcdsToControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                string strIcdSubText = "";
                if (txtIcdText.Text.LastIndexOf(";") > -1)
                {
                    strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                }
                else
                    strIcdSubText = txtIcdText.Text;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strIcdSubText), strIcdSubText));

                string icdNames = IcdUtil.seperator;
                string icdCodes = IcdUtil.seperator;
                string icdName__Olds = txtIcdText.Text;
                var checkList = this.icdSubcodeAdoChecks.Where(o => o.IsChecked == true).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkList), checkList));
                foreach (var item in checkList)
                {
                    icdCodes += item.ICD_CODE + IcdUtil.seperator;
                    icdNames += item.ICD_NAME + IcdUtil.seperator;
                }
                Inventec.Common.Logging.LogSystem.Debug(
                   Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdNames), icdNames)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCodes), icdCodes)
                   );
                string newtxtIcdText = ProcessIcdNameChanged(icdName__Olds, icdNames);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => newtxtIcdText), newtxtIcdText));

                txtIcdText.Text = newtxtIcdText;
                txtIcdSubCode.Text = icdCodes;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strIcdSubText), strIcdSubText));
                if (!String.IsNullOrEmpty(strIcdSubText))
                {
                    txtIcdText.Text = newtxtIcdText.Substring(0, newtxtIcdText.LastIndexOf(IcdUtil.seperator + strIcdSubText + IcdUtil.seperator) + 1);
                }
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = this.currentIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void ShowPopupContainerIcsSub()
        {
            try
            {
                popupControlContainer1.Width = 600;
                popupControlContainer1.Height = 250;
                customGridViewSubIcdName.Focus();
                customGridViewSubIcdName.FocusedRowHandle = 0;
                Rectangle buttonBounds = new Rectangle(layoutControlItem29.Bounds.X, layoutControlItem29.Bounds.Y, layoutControlItem29.Bounds.Width, layoutControlItem29.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 280, buttonBounds.Bottom - 65));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotProcessWhileChangedTextSubIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtIcdText_TextChanged____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotProcessWhileChangedTextSubIcd), isNotProcessWhileChangedTextSubIcd));
                    return;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    string strIcdSubText = "";

                    if (txtIcdText.Text.LastIndexOf(";") > -1)
                    {
                        strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                    }
                    else
                        strIcdSubText = txtIcdText.Text;
                    if (isShowContainerMediMatyForChoose)
                    {
                        customGridViewSubIcdName.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        customGridViewSubIcdName.ActiveFilterString = String.Format("[ICD_NAME] Like '%{0}%'", strIcdSubText.ToLower());
                        customGridViewSubIcdName.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                        customGridViewSubIcdName.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        customGridViewSubIcdName.OptionsFind.HighlightFindResults = true;

                        if (isShow && isShowGridIcdSub)
                        {
                            ShowPopupContainerIcsSub();
                            isShow = false;
                            isShowGridIcdSub = false;
                        }

                        txtIcdText.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    this.subIcdPopupSelect = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainer1.HidePopup();
                    }
                    else
                    {
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (isShowSubIcd)
                    {
                        SetFocusGrid();
                    }
                    else
                    {

                        isShowGridIcdSub = false;
                        popupControlContainer2.HidePopup();
                        if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                        {
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            isShowGridIcdSub = false;
                        }
                        ReloadIcdSubContainerByCodeChanged();
                        txtIcdText.Focus();
                        txtIcdText.SelectionStart = txtIcdText.Text.Length;

                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gvIcdSubCode.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtIcdSubCode.Bounds.X, txtIcdSubCode.Bounds.Y, txtIcdSubCode.Bounds.Width, txtIcdSubCode.Bounds.Height);
                    popupControlContainer2.ShowPopup(new Point(buttonBounds.X + 280, buttonBounds.Bottom + 55));
                    gvIcdSubCode.Focus();
                    gvIcdSubCode.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.KeyCode != null)
                {
                    isShowSubIcd = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFocusGrid()
        {
            try
            {
                gvIcdSubCode.Focus();
                gvIcdSubCode.FocusedRowHandle = 0;
                this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                if (this.IcdSubChoose != null)
                {
                    popupControlContainer2.HidePopup();
                    gvIcdSubCode.ActiveFilter.Clear();
                    LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                    txtIcdSubCode.Focus();
                    txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                    popupControlContainer2.HidePopup();
                    isShowSubIcd = false;
                }
                else
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void LoadSubIcd(string icdSubCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(icdSubCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(icdSubCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == icdSubCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {

                        if (txtIcdSubCode.Text.Contains(";"))
                        {
                            var arrText = txtIcdSubCode.Text.Split(';');
                            string txt = "";
                            for (int i = 0; i < arrText.Length - 1; i++)
                            {
                                txt += arrText[i] + ";";
                            }
                            txtIcdSubCode.Text = txt + result.First().ICD_CODE + ";";
                            icdCodeList.Add(result.First().ICD_CODE);
                        }
                        else
                        {
                            txtIcdSubCode.Text = result.First().ICD_CODE + ";";
                            icdCodeList.Add(result.First().ICD_CODE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtIcdSubCode.Focus();
                            this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                            isShowGridIcdSub = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    SetCheckedSubIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as IcdADO;
                    if (this.subIcdPopupSelect != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;

                        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                        this.customGridControlSubIcdName.RefreshDataSource();

                        SetCheckedSubIcdsToControl();
                        popupControlContainer1.HidePopup();

                        txtIcdText.Focus();
                        txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridViewSubIcdName_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.1");
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.2");
                        if (hi.Column.FieldName == "IsChecked" && hi.Column.RealColumnEdit != null && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.3");
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.4");
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdSubCode_TextChanged(object sender, EventArgs e)
        {
            try
            {

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    if (!isShowSubIcd)
                        return;
                    txtIcdSubCode.Refresh();
                    string keyWord = "";
                    if (txtIcdSubCode.Text.Contains(";"))
                    {
                        var arrText = txtIcdSubCode.Text.Split(';');
                        keyWord = arrText[arrText.Length - 1];
                    }
                    else
                    {
                        keyWord = txtIcdSubCode.Text;
                    }
                    gvIcdSubCode.ActiveFilterString = String.Format("[ICD_CODE] Like '%{0}%'", keyWord);
                    gvIcdSubCode.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                    gvIcdSubCode.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                    gvIcdSubCode.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                    gvIcdSubCode.FocusedRowHandle = 0;
                    gvIcdSubCode.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                    gvIcdSubCode.OptionsFind.HighlightFindResults = true;

                    Rectangle buttonBounds = new Rectangle(txtIcdSubCode.Bounds.X, txtIcdSubCode.Bounds.Y, txtIcdSubCode.Bounds.Width, txtIcdSubCode.Bounds.Height);
                    if (isShowSubIcd)
                    {
                        popupControlContainer2.ShowPopup(new Point(buttonBounds.X + 280, buttonBounds.Bottom + 55));
                        isShowSubTemp = true;
                    }
                    txtIcdSubCode.Focus();

                }
                else
                {
                    icdCodeList = new List<string>();
                    gvIcdSubCode.ActiveFilter.Clear();
                    this.IcdSubChoose = null;
                    if (!isShowGridIcdSub)
                    {
                        popupControlContainer2.HidePopup();
                    }
                    else
                    {
                        gvIcdSubCode.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer2_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShowSubIcd = false;
                isShowSubTemp = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gvIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                    if (this.IcdSubChoose != null)
                    {
                        popupControlContainer2.HidePopup();
                        isShowSubIcd = false;
                        gvIcdSubCode.ActiveFilter.Clear();
                        LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gvIcdSubCode_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                if (this.IcdSubChoose != null)
                {
                    popupControlContainer2.HidePopup();
                    isShowSubIcd = false;
                    gvIcdSubCode.ActiveFilter.Clear();
                    LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                    txtIcdSubCode.Focus();
                    txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkYAntibioticRequestStt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNAntibioticRequestStt.Checked)
                    chkNAntibioticRequestStt.Checked = false;
                chkYAntibioticRequestStt.Checked = numYAntibioticRequestStt % 2 == 0 ? true : false;
                numYAntibioticRequestStt++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNAntibioticRequestStt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkYAntibioticRequestStt.Checked)
                    chkYAntibioticRequestStt.Checked = false;
                chkNAntibioticRequestStt.Checked = numNAntibioticRequestStt % 2 == 0 ? true : false;
                numNAntibioticRequestStt++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAntibioticNewReg_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    AntibioticNewRegADO data = (AntibioticNewRegADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "START_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.START_DATE ?? 0);
                        }
                        else if (e.Column.FieldName == "END_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.END_DATE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repStartDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                AntibioticNewRegADO data = (AntibioticNewRegADO)gridViewAntibioticNewReg.GetFocusedRow();
                if (data != null)
                {
                    foreach (var item in lstNewRegADO)
                    {
                        if (data.ACTIVE_INGREDIENT_ID == item.ACTIVE_INGREDIENT_ID)
                        {
                            DateEdit dtTime = sender as DateEdit;
                            if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                            {
                                item.START_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);
                                if (data.USE_DAY != null && data.USE_DAY > 0)
                                {
                                    DateTime endDate = dtTime.DateTime.AddDays((double)(data.USE_DAY.Value - 1));
                                    item.END_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(endDate);
                                }
                                else
                                    item.END_DATE = null;
                            }
                            else
                            {
                                item.START_DATE = null;
                                item.END_DATE = null;
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
    }
}
